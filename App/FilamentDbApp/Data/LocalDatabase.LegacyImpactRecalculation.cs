using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using FilamentDbApp.Models;
using FilamentDbApp.Services.Calculations;
using Microsoft.Data.Sqlite;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    private static readonly JsonSerializerOptions ImpactAuditJson = new() { WriteIndented = true };
    private bool _legacyImpactRawEditBackupCreated;

    public List<ExperimentalMeasurementRecord> PrepareExperimentalGraphForSave(IEnumerable<ExperimentalMeasurementRecord> measurements)
    {
        using var connection = new SqliteConnection(new SqliteConnectionStringBuilder(ConnectionString)
            { Mode = SqliteOpenMode.ReadOnly, Pooling = false }.ToString());
        connection.Open();
        var prepared = new List<ExperimentalMeasurementRecord>();
        var requiresBackup = false;
        foreach (var row in measurements)
        {
            if (row.MeasurementType != "Impact") { prepared.Add(row); continue; }
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT * FROM ExperimentalMeasurements WHERE ExperimentalMeasurementId=$id;";
            command.Parameters.AddWithValue("$id", row.ExperimentalMeasurementId);
            using var reader = command.ExecuteReader();
            if (!reader.Read()) { prepared.Add(row); continue; }
            string Text(string name) => reader[name]?.ToString() ?? "";
            var sameInputs = Text("MeasurementType") == row.MeasurementType && Text("Orientation") == row.Orientation &&
                Text("RawUnit") == row.RawUnit && Text("ResultUnit") == row.ResultUnit &&
                Text("ExperimentalRunId") == row.ExperimentalRunId &&
                row.SampleValues().Select((raw, i) => raw == Text("Sample" + (i + 1))).All(equal => equal);
            var supported = row.Orientation is "Flat" or "Upright" && row.RawUnit == "%" && row.ResultUnit == "kJ/m²";
            if (!sameInputs) requiresBackup = true;
            if (!sameInputs && supported) { prepared.Add(row); continue; }
            var clone = JsonSerializer.Deserialize<ExperimentalMeasurementRecord>(JsonSerializer.Serialize(row))!;
            clone.ResultAverage = Text("ResultAverage"); clone.ResultStdDev = Text("ResultStdDev");
            clone.ResultCv = Text("ResultCv"); clone.ResultCount = Text("ResultCount");
            clone.ResultConfidence = Text("ResultConfidence"); prepared.Add(clone);
        }
        if (requiresBackup && !_legacyImpactRawEditBackupCreated)
        {
            CreateManualBackupNow();
            _legacyImpactRawEditBackupCreated = true;
        }
        return prepared;
    }

    public LegacyImpactRecalculationAudit PreviewLegacyImpactRecalculation()
    {
        using var connection = new SqliteConnection(new SqliteConnectionStringBuilder(ConnectionString)
            { Mode = SqliteOpenMode.ReadOnly, Pooling = false }.ToString());
        connection.Open();
        using var transaction = connection.BeginTransaction(deferred: true);
        return BuildLegacyImpactAudit(connection, transaction);
    }

    public LegacyImpactRecalculationAudit ApplyLegacyImpactRecalculation(LegacyImpactRecalculationAudit preview)
    {
        ArgumentNullException.ThrowIfNull(preview);
        var current = PreviewLegacyImpactRecalculation();
        if (current.SourceFingerprint != preview.SourceFingerprint)
            throw new InvalidOperationException("Impact raw data or calibration changed. Create a new preview.");
        if (!current.Groups.Any(x => x.Ready))
            throw new InvalidOperationException("No Impact groups can be recalculated; review pending calibration/readings.");

        var backup = CreateManualBackupNow();
        current.BackupPath = backup.FullName;
        using (var stream = IOFile.OpenRead(backup.FullName))
            current.BackupSha256 = Convert.ToHexString(SHA256.HashData(stream));
        using (var backupConnection = new SqliteConnection(new SqliteConnectionStringBuilder
            { DataSource = backup.FullName, Mode = SqliteOpenMode.ReadOnly, Pooling = false }.ToString()))
        {
            backupConnection.Open();
            if (BuildLegacyImpactAudit(backupConnection, null).SourceFingerprint != current.SourceFingerprint)
                throw new InvalidOperationException("Impact changed during backup. No results were updated; preview again.");
        }
        current.AuditPath = backup.FullName + ".impact-recalculation.json";
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction(deferred: false);
        if (BuildLegacyImpactAudit(connection, transaction).SourceFingerprint != current.SourceFingerprint)
            throw new InvalidOperationException("Impact changed after backup. No results were updated; preview again.");
        foreach (var group in current.Groups.Where(x => x.Kind == "Experimental" && x.Ready))
        {
            static string Number(double? value) => value?.ToString("0.###", CultureInfo.InvariantCulture) ?? "";
            var afterValues = new Dictionary<string, string>
            {
                ["ResultAverage"] = Number(group.After.Average), ["ResultStdDev"] = Number(group.After.StdDev),
                ["ResultCv"] = Number(group.After.CvPercent), ["ResultCount"] = group.After.Count.ToString(CultureInfo.InvariantCulture),
                ["ResultConfidence"] = group.After.Confidence?.ToString(CultureInfo.InvariantCulture) ?? ""
            };
            if (afterValues.All(pair => group.PersistedBefore.TryGetValue(pair.Key, out var before) && before == pair.Value)) continue;
            using var update = connection.CreateCommand();
            update.Transaction = transaction;
            update.CommandText = """
                UPDATE ExperimentalMeasurements SET ResultAverage=$average,ResultStdDev=$std,
                ResultCv=$cv,ResultCount=$count,ResultConfidence=$confidence
                WHERE ExperimentalMeasurementId=$id AND MeasurementType='Impact';
                """;
            update.Parameters.AddWithValue("$average", Number(group.After.Average));
            update.Parameters.AddWithValue("$std", Number(group.After.StdDev));
            update.Parameters.AddWithValue("$cv", Number(group.After.CvPercent));
            update.Parameters.AddWithValue("$count", group.After.Count.ToString(CultureInfo.InvariantCulture));
            update.Parameters.AddWithValue("$confidence", group.After.Confidence?.ToString(CultureInfo.InvariantCulture) ?? "");
            update.Parameters.AddWithValue("$id", group.Id);
            if (update.ExecuteNonQuery() != 1) throw new InvalidOperationException("Impact row identity changed.");
            current.UpdatedExperimentalRows++;
        }
        current.ApplyState = "Prepared; database commit not yet confirmed";
        WriteLegacyImpactAudit(current);
        transaction.Commit();
        current.ApplyState = "Committed; native results are runtime derived, raw readings and timestamps unchanged";
        try { WriteLegacyImpactAudit(current); }
        catch (Exception ex) when (ex is System.IO.IOException or UnauthorizedAccessException)
        { current.ApplyState += "; audit completion marker could not be written: " + ex.Message; }
        return current;
    }

    private static void WriteLegacyImpactAudit(LegacyImpactRecalculationAudit audit)
    {
        var temporaryPath = audit.AuditPath + ".tmp";
        IOFile.WriteAllText(temporaryPath, JsonSerializer.Serialize(audit, ImpactAuditJson));
        IOFile.Move(temporaryPath, audit.AuditPath, overwrite: true);
    }

    private static LegacyImpactRecalculationAudit BuildLegacyImpactAudit(SqliteConnection connection, SqliteTransaction? transaction)
    {
        List<Dictionary<string, string?>> Read(string sql)
        {
            using var command = connection.CreateCommand(); command.Transaction = transaction; command.CommandText = sql;
            using var reader = command.ExecuteReader();
            var rows = new List<Dictionary<string, string?>>();
            while (reader.Read())
            {
                var row = new Dictionary<string, string?>();
                for (var i = 0; i < reader.FieldCount; i++) row[reader.GetName(i)] = reader.IsDBNull(i) ? null : reader.GetValue(i).ToString();
                rows.Add(row);
            }
            return rows;
        }
        var settings = Read("SELECT * FROM NativeSettingsRows WHERE Section='Impact' ORDER BY Parameter,Unit;");
        var native = Read("SELECT * FROM NativeImpactSamples ORDER BY MaterialId,Orientation,SampleNumber;");
        var experimental = Read("""
            SELECT m.*, e.MaterialId AS AuditMaterialId FROM ExperimentalMeasurements m
            LEFT JOIN ExperimentalRuns r ON r.ExperimentalRunId=m.ExperimentalRunId
            LEFT JOIN MaterialExperiments e ON e.MaterialExperimentId=r.MaterialExperimentId
            WHERE m.MeasurementType='Impact' ORDER BY m.ExperimentalMeasurementId;
            """);
        var audit = new LegacyImpactRecalculationAudit { CalibrationRows = settings };
        audit.SourceFingerprint = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
            JsonSerializer.Serialize(new { settings, native, experimental }))));
        static double? Parse(string? raw) => double.TryParse(raw?.Trim().Replace(',', '.'), NumberStyles.Float,
            CultureInfo.InvariantCulture, out var value) && double.IsFinite(value) ? value : null;
        double? Setting(string name, string unit)
        {
            var matches = settings.Where(x => x["Parameter"] == name && x["Unit"] == unit).ToList();
            var value = matches.Count == 1 ? Parse(matches[0]["Value"]) : null;
            if (value is not > 0) audit.CalibrationIssues.Add($"Missing/invalid unique {name} ({unit}).");
            return value is > 0 ? value : null;
        }
        var energy = Setting("Available Joules", "J");
        var area = Setting("Net cross-section area", "mm²");
        var angle = Setting("No-sample rebound angle", "degrees");
        // Old displayed values require the actual old maximum, not an inferred calibration.
        var oldMaximum = Setting("Max possible impact", "kJ/m²");
        var oldArea = Setting("Net cross-section area", "m²");
        if (energy.HasValue && area.HasValue && angle.HasValue)
        {
            audit.Settings = new(energy.Value, area.Value, angle.Value);
            if (LegacyImpactCalculationService.TryCalculate(0, audit.Settings) is null)
            { audit.CalibrationIssues.Add("Angular calibration is invalid."); audit.Settings = null; }
            static bool Matches(double actual, double confirmed) => Math.Abs(actual - confirmed) <= Math.Abs(confirmed) * 1e-10;
            if (!Matches(energy.Value, 2.743860924) || !Matches(area.Value, 48.1603) || !Matches(angle.Value, 105.411))
            {
                audit.CalibrationIssues.Add("Current calibration differs from the owner-confirmed 2026-09-28 calibration; new historical attestation is required.");
                audit.Settings = null;
            }
        }
        LegacyImpactRecalculationReading Reading(int number, string? raw)
        {
            var reading = new LegacyImpactRecalculationReading { SampleNumber = number, Raw = raw };
            if (string.IsNullOrWhiteSpace(raw)) { reading.PendingReason = "Blank sample slot"; return reading; }
            var percent = Parse(raw);
            if (!percent.HasValue || percent is < 0 or > 100) { reading.PendingReason = "Invalid raw needle percentage"; return reading; }
            reading.Calculation = LegacyImpactCalculationService.TryCalculate(percent.Value, audit.Settings);
            if (reading.Calculation is null) reading.PendingReason = "Calibration unavailable/invalid";
            if (reading.Calculation is not null && oldMaximum.HasValue && oldArea.HasValue)
            {
                var oldValue = oldMaximum.Value * reading.Calculation.Fraction / oldArea.Value / 1000d;
                if (double.IsFinite(oldValue)) reading.OldDisplayedKjM2 = oldValue;
                else { reading.Calculation = null; reading.PendingReason = "Old calculation overflow; calibration requires review"; }
            }
            return reading;
        }
        static LegacyImpactRecalculationSummary Summary(IEnumerable<double?> input)
        {
            var statistics = new StatisticsService(); var values = input.ToArray();
            var count = statistics.CountNumeric(values); var mean = statistics.Average(values);
            var sd = statistics.StandardDeviationSample(values);
            return new(mean, sd, statistics.CoefficientOfVariation(sd, mean) * 100, count, statistics.ConfidenceFromSampleCount(count));
        }
        void Finish(LegacyImpactRecalculationGroup group)
        {
            group.Before = Summary(group.Readings.Select(x => x.OldDisplayedKjM2));
            group.After = Summary(group.Readings.Select(x => x.Calculation?.ImpactKjM2));
            group.Ready = group.After.Count > 0 && audit.CalibrationIssues.Count == 0 &&
                group.Readings.All(x => string.IsNullOrWhiteSpace(x.Raw) || x.Calculation is not null);
            if (!group.Ready) group.PendingReason = audit.CalibrationIssues.Count > 0 ? "Calibration requires review" :
                group.After.Count == 0 ? "No valid readings" : "Invalid readings require review; existing results retained";
            if (group.Orientation is not ("Flat" or "Upright"))
            { group.Ready = false; group.PendingReason = "Unsupported orientation; existing results retained"; }
            audit.Groups.Add(group);
        }
        foreach (var rows in native.GroupBy(x => (Id: x["MaterialId"]!, Orientation: x["Orientation"]!)))
            Finish(new() { Kind = "Native", Id = rows.Key.Id, MaterialId = rows.Key.Id, Orientation = rows.Key.Orientation,
                Readings = rows.Select(x => Reading(int.Parse(x["SampleNumber"]!, CultureInfo.InvariantCulture), x["RawValue"])).ToList() });
        foreach (var row in experimental)
        {
            var group = new LegacyImpactRecalculationGroup { Kind = "Experimental", Id = row["ExperimentalMeasurementId"]!,
                MaterialId = row["AuditMaterialId"] ?? "", Orientation = row["Orientation"] ?? "",
                Readings = Enumerable.Range(1, 10).Select(i => Reading(i, row["Sample" + i])).ToList(),
                PersistedBefore = row.Where(x => x.Key.StartsWith("Result", StringComparison.Ordinal)).ToDictionary(x => x.Key, x => x.Value) };
            Finish(group);
            if (row["RawUnit"] != "%" || row["ResultUnit"] != "kJ/m²" || string.IsNullOrWhiteSpace(group.MaterialId))
            { group.Ready = false; group.PendingReason = "Unknown experimental units or material identity; retained unchanged"; }
        }
        return audit;
    }
}
