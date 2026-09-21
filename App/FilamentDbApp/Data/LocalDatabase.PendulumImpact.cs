using FilamentDbApp.Models;
using FilamentDbApp.Services;
using Microsoft.Data.Sqlite;
using System.Globalization;
using System.IO;
using System.Text.Json;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    private static void EnsurePendulumImpactSchema(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
CREATE TABLE IF NOT EXISTS PendulumImpactRuns (
 RunId TEXT PRIMARY KEY, MaterialID TEXT NOT NULL, Method TEXT NOT NULL CHECK(Method IN ('Izod','Charpy')),
 RecordJson TEXT NOT NULL, CreatedAtUtc TEXT NOT NULL, UpdatedAtUtc TEXT NOT NULL,
 FOREIGN KEY(MaterialID) REFERENCES NativeMaterialManagerRows(MaterialID) ON DELETE RESTRICT
);
CREATE INDEX IF NOT EXISTS IX_PendulumImpactRuns_Material ON PendulumImpactRuns(MaterialID,Method);
CREATE TABLE IF NOT EXISTS PendulumImpactSpecimens (
 SpecimenId TEXT PRIMARY KEY, RunId TEXT NOT NULL, RecordJson TEXT NOT NULL,
 CreatedAtUtc TEXT NOT NULL, UpdatedAtUtc TEXT NOT NULL,
 FOREIGN KEY(RunId) REFERENCES PendulumImpactRuns(RunId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_PendulumImpactSpecimens_Run ON PendulumImpactSpecimens(RunId);
""";
        command.ExecuteNonQuery();
    }

    public (List<PendulumImpactRunRecord> Runs, List<PendulumImpactSpecimenRecord> Specimens) LoadPendulumImpactGraph()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return ReadPendulumImpactGraph(connection);
    }

    private static (List<PendulumImpactRunRecord> Runs, List<PendulumImpactSpecimenRecord> Specimens) ReadPendulumImpactGraph(SqliteConnection connection, SqliteTransaction? transaction = null)
    {
        var runs = new List<PendulumImpactRunRecord>();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "SELECT RunId,MaterialID,Method,RecordJson FROM PendulumImpactRuns ORDER BY CreatedAtUtc,RunId;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = JsonSerializer.Deserialize<PendulumImpactRunRecord>(reader.GetString(3)) ?? throw new InvalidDataException("Invalid pendulum run payload.");
                if (row.RunId != reader.GetString(0) || row.MaterialID != reader.GetString(1) || row.Method != reader.GetString(2))
                    throw new InvalidDataException("Pendulum run identity does not match canonical SQLite keys.");
                runs.Add(row);
            }
        }
        var specimens = new List<PendulumImpactSpecimenRecord>();
        using (var command = connection.CreateCommand())
        {
            command.Transaction = transaction;
            command.CommandText = "SELECT SpecimenId,RunId,RecordJson FROM PendulumImpactSpecimens ORDER BY RunId,SpecimenId;";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = JsonSerializer.Deserialize<PendulumImpactSpecimenRecord>(reader.GetString(2)) ?? throw new InvalidDataException("Invalid pendulum specimen payload.");
                if (row.SpecimenId != reader.GetString(0) || row.RunId != reader.GetString(1)) throw new InvalidDataException("Pendulum specimen identity does not match canonical SQLite keys.");
                specimens.Add(row);
            }
        }
        return (runs, specimens);
    }

    public void SynchronizePendulumImpactGraph(IReadOnlyCollection<PendulumImpactRunRecord> runs, IReadOnlyCollection<PendulumImpactSpecimenRecord> specimens)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        WritePendulumImpactGraph(connection, runs, specimens);
    }

    private static void WritePendulumImpactGraph(SqliteConnection connection, IReadOnlyCollection<PendulumImpactRunRecord> runs, IReadOnlyCollection<PendulumImpactSpecimenRecord> specimens)
    {
        var errors = PendulumImpactService.Validate(runs, specimens);
        if (errors.Count > 0) throw new InvalidOperationException(string.Join(Environment.NewLine, errors));
        using (var foreignKeys = connection.CreateCommand()) { foreignKeys.CommandText = "PRAGMA foreign_keys=ON;"; foreignKeys.ExecuteNonQuery(); }
        using var transaction = connection.BeginTransaction();
        var existing = ReadPendulumImpactGraph(connection, transaction);
        var existingRuns = existing.Runs.ToDictionary(row => row.RunId, StringComparer.Ordinal);
        var existingSpecimens = existing.Specimens.ToDictionary(row => row.SpecimenId, StringComparer.Ordinal);
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        foreach (var row in runs)
        {
            if (existingRuns.TryGetValue(row.RunId, out var saved))
            {
                // UI submits a detached whole-graph snapshot; stale client timestamps are not edits.
                row.CreatedAtUtc = saved.CreatedAtUtc;
                row.UpdatedAtUtc = saved.UpdatedAtUtc;
                if (JsonSerializer.Serialize(row) == JsonSerializer.Serialize(saved)) continue;
            }
            if (string.IsNullOrWhiteSpace(row.CreatedAtUtc)) row.CreatedAtUtc = now;
            row.UpdatedAtUtc = now;
            Upsert(connection, transaction, """
INSERT INTO PendulumImpactRuns(RunId,MaterialID,Method,RecordJson,CreatedAtUtc,UpdatedAtUtc)
VALUES($id,$material,$method,$json,$created,$updated)
ON CONFLICT(RunId) DO UPDATE SET MaterialID=excluded.MaterialID,Method=excluded.Method,RecordJson=excluded.RecordJson,UpdatedAtUtc=excluded.UpdatedAtUtc;
""", ("$id", row.RunId), ("$material", row.MaterialID), ("$method", row.Method), ("$json", JsonSerializer.Serialize(row)), ("$created", row.CreatedAtUtc), ("$updated", now));
        }
        foreach (var row in specimens)
        {
            if (existingSpecimens.TryGetValue(row.SpecimenId, out var saved))
            {
                row.CreatedAtUtc = saved.CreatedAtUtc;
                row.UpdatedAtUtc = saved.UpdatedAtUtc;
                if (JsonSerializer.Serialize(row) == JsonSerializer.Serialize(saved)) continue;
            }
            if (string.IsNullOrWhiteSpace(row.CreatedAtUtc)) row.CreatedAtUtc = now;
            row.UpdatedAtUtc = now;
            Upsert(connection, transaction, """
INSERT INTO PendulumImpactSpecimens(SpecimenId,RunId,RecordJson,CreatedAtUtc,UpdatedAtUtc)
VALUES($id,$run,$json,$created,$updated)
ON CONFLICT(SpecimenId) DO UPDATE SET RunId=excluded.RunId,RecordJson=excluded.RecordJson,UpdatedAtUtc=excluded.UpdatedAtUtc;
""", ("$id", row.SpecimenId), ("$run", row.RunId), ("$json", JsonSerializer.Serialize(row)), ("$created", row.CreatedAtUtc), ("$updated", now));
        }
        DeleteRowsMissingFromSnapshot(connection, transaction, "PendulumImpactSpecimens", "SpecimenId", specimens.Select(x => x.SpecimenId));
        DeleteRowsMissingFromSnapshot(connection, transaction, "PendulumImpactRuns", "RunId", runs.Select(x => x.RunId));
        transaction.Commit();
    }

    public static bool RunPendulumImpactPersistenceVerification()
    {
        // Isolated in-memory SQLite fixture: no owner database or profile is opened.
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using (var setup = connection.CreateCommand())
        {
            setup.CommandText = "CREATE TABLE NativeMaterialManagerRows(MaterialID TEXT PRIMARY KEY); INSERT INTO NativeMaterialManagerRows VALUES('TEST'); CREATE TABLE NativeImpactSamples(Value TEXT); INSERT INTO NativeImpactSamples VALUES('legacy-raw');";
            setup.ExecuteNonQuery();
        }
        EnsurePendulumImpactSchema(connection);
        var run = new PendulumImpactRunRecord
        {
            RunId = "RUN", MaterialID = "TEST", Method = "Izod", Label = "Ólokið próf", Date = "2026-09-21",
            TargetCount = "10", Equipment = "Hongtuo HT-5D-CM", StandardReference = "ISO 180", StandardEdition = "Unconfirmed",
            Conformity = "Unverified", SpecimenType = "Profile α", NotchType = "Notched", NotchPreparation = "Machined",
            NominalLengthMm = "80", NominalWidthMm = "10", NominalThicknessMm = "4", NominalLigamentMm = "8",
            Orientation = "Long side against bed; layer direction recorded separately", DiagramPath = "fixture.png",
            DiagramBase64 = Convert.ToBase64String([1, 2, 3]), Printer = "private printer", NozzleMm = "0.4", LayerHeightMm = "0.2",
            Walls = "2", TopLayers = "3", BottomLayers = "3", InfillPercent = "100", InfillPattern = "Rectilinear",
            PrintTemperatureC = "230", BedTemperatureC = "60", Drying = "As recorded", Conditioning = "Sealed bag",
            TestTemperatureC = "21.5", HumidityPercent = "45", Setup = "Clamping details", Notes = "Raw profile preserved"
        };
        var specimen = new PendulumImpactSpecimenRecord
        {
            RunId = "RUN", SpecimenId = "SPEC", Label = "A1", PrintBatch = "Batch A", EnergyJ = "1,25", HammerJ = "2.75",
            LengthMm = "80.02", WidthMm = "10.02", ThicknessMm = "4.01", RemainingLigamentMm = "8.02", NotchDepthMm = "2",
            BreakType = "Complete", Status = "Excluded", ExclusionReason = "Fixture movement", Notes = "raw, unchanged", SettingsOverrides = "batch-specific drying"
        };
        WritePendulumImpactGraph(connection, [run], [specimen]);
        var first = ReadPendulumImpactGraph(connection);
        if (JsonSerializer.Serialize(run) != JsonSerializer.Serialize(first.Runs.Single()) || JsonSerializer.Serialize(specimen) != JsonSerializer.Serialize(first.Specimens.Single())) return false;
        using (var backup = new SqliteConnection("Data Source=:memory:"))
        {
            backup.Open(); connection.BackupDatabase(backup);
            using var restored = new SqliteConnection("Data Source=:memory:"); restored.Open(); backup.BackupDatabase(restored);
            var recovered = ReadPendulumImpactGraph(restored);
            if (JsonSerializer.Serialize(first.Runs) != JsonSerializer.Serialize(recovered.Runs) ||
                JsonSerializer.Serialize(first.Specimens) != JsonSerializer.Serialize(recovered.Specimens)) return false;
            using var integrity = restored.CreateCommand(); integrity.CommandText = "PRAGMA integrity_check;";
            if ((string?)integrity.ExecuteScalar() != "ok") return false;
        }
        var unrelatedRun = new PendulumImpactRunRecord { RunId = "OTHER-RUN", MaterialID = "TEST", Method = "Charpy", Notes = "untouched run" };
        var unrelatedSpecimen = new PendulumImpactSpecimenRecord { RunId = unrelatedRun.RunId, SpecimenId = "OTHER-SPEC", Notes = "untouched specimen" };
        WritePendulumImpactGraph(connection, [run, unrelatedRun], [specimen, unrelatedSpecimen]);
        string UnrelatedBytes()
        {
            using var read = connection.CreateCommand();
            read.CommandText = "SELECT (SELECT RecordJson||'|'||CreatedAtUtc||'|'||UpdatedAtUtc FROM PendulumImpactRuns WHERE RunId='OTHER-RUN')||'|'||(SELECT RecordJson||'|'||CreatedAtUtc||'|'||UpdatedAtUtc FROM PendulumImpactSpecimens WHERE SpecimenId='OTHER-SPEC');";
            return (string)read.ExecuteScalar()!;
        }
        var unrelatedBefore = UnrelatedBytes();
        unrelatedRun.CreatedAtUtc = unrelatedRun.UpdatedAtUtc = "stale client timestamp";
        unrelatedSpecimen.CreatedAtUtc = unrelatedSpecimen.UpdatedAtUtc = "stale client timestamp";
        run.Label = "Edited run";
        specimen.Notes = "Edited specimen";
        WritePendulumImpactGraph(connection, [run, unrelatedRun], [specimen, unrelatedSpecimen]);
        if (UnrelatedBytes() != unrelatedBefore || ReadPendulumImpactGraph(connection).Runs.Single(x => x.RunId == "RUN").Label != "Edited run") return false;
        WritePendulumImpactGraph(connection, [run], [specimen]);
        var unchangedRun = JsonSerializer.Serialize(ReadPendulumImpactGraph(connection).Runs.Single());
        run.CreatedAtUtc = run.UpdatedAtUtc = "stale after UI clone";
        specimen.EnergyJ = "";
        WritePendulumImpactGraph(connection, [run], [specimen]);
        var cleared = ReadPendulumImpactGraph(connection);
        if (cleared.Specimens.Single().EnergyJ != "" || JsonSerializer.Serialize(cleared.Runs.Single()) != unchangedRun) return false;
        var rejected = false;
        try { WritePendulumImpactGraph(connection, [new() { RunId = "BAD", MaterialID = "MISSING", Method = "Charpy" }], []); }
        catch (SqliteException) { rejected = true; }
        if (!rejected || ReadPendulumImpactGraph(connection).Runs.Single().RunId != "RUN") return false;
        // Direct instrument readings coexist with the historical energy graph, without unit reinterpretation.
        var directRun = new PendulumImpactRunRecord { RunId = "DIRECT", MaterialID = "TEST", Method = "Charpy", InputMode = "DirectStrength" };
        var directRows = Enumerable.Range(1, 10).Select(i => new PendulumImpactSpecimenRecord
            { RunId = directRun.RunId, SpecimenId = "DIRECT-" + i, Label = i.ToString("00", CultureInfo.InvariantCulture), StrengthKjM2Raw = i == 1 ? "125,75" : i == 2 ? "NB" : "" }).ToArray();
        var legacyGraph = ReadPendulumImpactGraph(connection);
        var legacyBeforeDirect = JsonSerializer.Serialize(legacyGraph.Runs) + JsonSerializer.Serialize(legacyGraph.Specimens);
        WritePendulumImpactGraph(connection, [run, directRun], directRows.Prepend(specimen).ToArray());
        var directSaved = ReadPendulumImpactGraph(connection);
        if (directSaved.Runs.Single(x => x.RunId == "DIRECT").InputMode != "DirectStrength" ||
            directSaved.Specimens.Single(x => x.SpecimenId == "DIRECT-1").StrengthKjM2Raw != "125,75" ||
            directSaved.Specimens.Single(x => x.SpecimenId == "DIRECT-2").StrengthKjM2Raw != "NB" ||
            directSaved.Specimens.Count(x => x.RunId == "DIRECT" && x.StrengthKjM2Raw == "") != 8) return false;
        directRows[0].StrengthKjM2Raw = "";
        WritePendulumImpactGraph(connection, [run, directRun], directRows.Prepend(specimen).ToArray());
        var directCleared = ReadPendulumImpactGraph(connection);
        if (directCleared.Specimens.Single(x => x.SpecimenId == "DIRECT-1").StrengthKjM2Raw != "" ||
            PendulumImpactService.Summarize(directRun, directCleared.Specimens).ValidCount != 0) return false;
        WritePendulumImpactGraph(connection, [run], [specimen]);
        var legacyAfterDirect = ReadPendulumImpactGraph(connection);
        if (JsonSerializer.Serialize(legacyAfterDirect.Runs) + JsonSerializer.Serialize(legacyAfterDirect.Specimens) != legacyBeforeDirect) return false;
        WritePendulumImpactGraph(connection, [], []);
        using var legacy = connection.CreateCommand(); legacy.CommandText = "SELECT Value FROM NativeImpactSamples;";
        return ReadPendulumImpactGraph(connection).Runs.Count == 0 && (string?)legacy.ExecuteScalar() == "legacy-raw" &&
            RunPendulumExcelRecoveryVerification();
    }

    private static bool RunPendulumExcelRecoveryVerification()
    {
        var root = IOPath.GetFullPath(IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-PendulumRecovery-" + Guid.NewGuid().ToString("N")));
        IODirectory.CreateDirectory(root);
        try
        {
            var database = new LocalDatabase(IOPath.Combine(root, "recovery.sqlite"));
            using (var connection = new SqliteConnection(database.ConnectionString))
            {
                connection.Open();
                using var seed = connection.CreateCommand();
                seed.CommandText = "INSERT INTO NativeMaterialManagerRows(MaterialId,UpdatedAtUtc) VALUES('RECOVERY-MATERIAL','2026-09-21T00:00:00Z');";
                seed.ExecuteNonQuery();
            }
            var run = new PendulumImpactRunRecord { RunId = "RECOVERY-RUN", MaterialID = "RECOVERY-MATERIAL", Method = "Charpy", InputMode = "DirectStrength", DiagramBase64 = Convert.ToBase64String([4, 5, 6]) };
            var specimen = new PendulumImpactSpecimenRecord { SpecimenId = "RECOVERY-SPECIMEN", RunId = run.RunId, Label = "01", EnergyJ = "0,125", StrengthKjM2Raw = "125,75", Notes = "Exact raw value" };
            var specimens = Enumerable.Range(2, 9).Select(i => new PendulumImpactSpecimenRecord { RunId = run.RunId,
                SpecimenId = "RECOVERY-" + i, Label = i.ToString("00", CultureInfo.InvariantCulture) }).Prepend(specimen).ToArray();
            database.SynchronizePendulumImpactGraph([run], specimens);
            var current = database.CreateExcelRecoverySnapshot();
            if (!current.Tables.Select(x => x.TableName).SequenceEqual(ExcelRecoveryTableNames, StringComparer.Ordinal)) return false;
            database.RestoreExcelRecoverySnapshot(current);
            var restored = database.LoadPendulumImpactGraph();
            if (restored.Runs.Single().DiagramBase64 != run.DiagramBase64 || restored.Runs.Single().InputMode != "DirectStrength" ||
                restored.Specimens.Single(x => x.SpecimenId == specimen.SpecimenId).EnergyJ != specimen.EnergyJ ||
                restored.Specimens.Single(x => x.SpecimenId == specimen.SpecimenId).StrengthKjM2Raw != "125,75" || restored.Specimens.Count != 10) return false;
            var older = new ExcelRecoverySnapshot { SourceSchemaVersion = 44, ExportedAtUtc = current.ExportedAtUtc,
                Tables = current.Tables.Where(table => table.TableName is not ("PendulumImpactRuns" or "PendulumImpactSpecimens")).ToList() };
            database.RestoreExcelRecoverySnapshot(older);
            var legacyRestored = database.LoadPendulumImpactGraph();
            return legacyRestored.Runs.Count == 0 && legacyRestored.Specimens.Count == 0 &&
                database.LoadNativeMaterialManagerRows().Single().MaterialID == "RECOVERY-MATERIAL";
        }
        catch { return false; }
        finally
        {
            SqliteConnection.ClearAllPools();
            // Only this exact generated fixture directory is eligible; no recursive directory operation.
            if (IOPath.GetDirectoryName(root) == IOPath.GetFullPath(IOPath.GetTempPath()).TrimEnd(IOPath.DirectorySeparatorChar) &&
                IOPath.GetFileName(root).StartsWith("3DPIceland-PendulumRecovery-", StringComparison.Ordinal))
            {
                foreach (var file in IODirectory.EnumerateFiles(root)) IOFile.Delete(file);
                IODirectory.Delete(root);
            }
        }
    }
}
