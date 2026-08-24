using FilamentDbApp.Services;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    public sealed record ThermalDeflectionMeasurementRecord(
        string MaterialId,
        double ResultTemperatureC,
        string? MeasuredDate,
        string? TestNotes,
        string MethodVersion,
        string SourceFileName,
        string SourceSha256,
        string ImportedAtUtc,
        string UpdatedAtUtc);

    private static void EnsureThermalDeflectionMethodV1(
        SqliteConnection connection,
        SqliteTransaction? transaction = null)
    {
        using (var insert = connection.CreateCommand())
        {
            insert.Transaction = transaction;
            insert.CommandText = """
                INSERT OR IGNORE INTO ThermalDeflectionMethods
                    (MethodVersion, MethodSnapshotJson, MethodSnapshotSha256, CreatedAtUtc)
                VALUES ($version, $json, $sha, $now);
                """;
            insert.Parameters.AddWithValue("$version", ThermalDeflectionMethodContract.Version);
            insert.Parameters.AddWithValue("$json", ThermalDeflectionMethodContract.SnapshotJson);
            insert.Parameters.AddWithValue("$sha", ThermalDeflectionMethodContract.SnapshotSha256);
            insert.Parameters.AddWithValue("$now", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            insert.ExecuteNonQuery();
        }
        using var verify = connection.CreateCommand();
        verify.Transaction = transaction;
        verify.CommandText = """
            SELECT MethodSnapshotJson, MethodSnapshotSha256
            FROM ThermalDeflectionMethods
            WHERE MethodVersion=$version;
            """;
        verify.Parameters.AddWithValue("$version", ThermalDeflectionMethodContract.Version);
        using var reader = verify.ExecuteReader();
        if (!reader.Read() ||
            !string.Equals(reader.GetString(0), ThermalDeflectionMethodContract.SnapshotJson, StringComparison.Ordinal) ||
            !string.Equals(reader.GetString(1), ThermalDeflectionMethodContract.SnapshotSha256, StringComparison.Ordinal))
            throw new InvalidOperationException("Thermal deflection method v1 snapshot drifted from the governed contract.");
    }

    public IReadOnlySet<string> GetCanonicalThermalDeflectionMaterialIds()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT MaterialId FROM NativeMaterialManagerRows WHERE TRIM(MaterialId) <> '';";
        using var reader = command.ExecuteReader();
        var result = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        while (reader.Read()) result.Add(reader.GetString(0).Trim());
        return result;
    }

    public IReadOnlyDictionary<string, double> GetThermalDeflectionResults()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT MaterialId, ResultTemperatureC FROM NativeThermalDeflectionMeasurements;";
        using var reader = command.ExecuteReader();
        var result = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
        while (reader.Read()) result.Add(reader.GetString(0), reader.GetDouble(1));
        return result;
    }

    public IReadOnlyList<ThermalDeflectionMeasurementRecord> GetThermalDeflectionMeasurements()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT MaterialId, ResultTemperatureC, MeasuredDate, TestNotes, MethodVersion,
                   SourceFileName, SourceSha256, ImportedAtUtc, UpdatedAtUtc
            FROM NativeThermalDeflectionMeasurements
            ORDER BY MaterialId;
            """;
        using var reader = command.ExecuteReader();
        var rows = new List<ThermalDeflectionMeasurementRecord>();
        while (reader.Read())
            rows.Add(new(
                reader.GetString(0), reader.GetDouble(1),
                reader.IsDBNull(2) ? null : reader.GetString(2),
                reader.IsDBNull(3) ? null : reader.GetString(3),
                reader.GetString(4), reader.GetString(5), reader.GetString(6),
                reader.GetString(7), reader.GetString(8)));
        return rows;
    }

    public IReadOnlyList<ThermalDeflectionExportRow> GetThermalDeflectionExportRows()
    {
        var measurements = GetThermalDeflectionMeasurements()
            .ToDictionary(row => row.MaterialId, StringComparer.OrdinalIgnoreCase);
        return GetCanonicalThermalDeflectionMaterialIds()
            .OrderBy(materialId => materialId, StringComparer.OrdinalIgnoreCase)
            .Select(materialId => measurements.TryGetValue(materialId, out var row)
                ? new ThermalDeflectionExportRow(
                    materialId, row.ResultTemperatureC, row.MeasuredDate, row.TestNotes,
                    row.MethodVersion, row.SourceFileName, row.SourceSha256,
                    row.ImportedAtUtc, row.UpdatedAtUtc)
                : new ThermalDeflectionExportRow(materialId, null, null, null, null, null, null, null, null))
            .ToList();
    }

    public void SaveManualThermalDeflectionMeasurement(
        string materialId,
        double? resultTemperatureC,
        string? measuredDate,
        string? testNotes)
    {
        if (string.IsNullOrWhiteSpace(materialId))
            throw new ArgumentException("MaterialID is required.", nameof(materialId));
        if (resultTemperatureC is < 25 or > 300 ||
            resultTemperatureC.HasValue && !double.IsFinite(resultTemperatureC.Value))
            throw new ArgumentOutOfRangeException(nameof(resultTemperatureC));

        CreateThrottledAutomaticBackupBeforeWrite();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        if (!resultTemperatureC.HasValue)
        {
            using var delete = connection.CreateCommand();
            delete.Transaction = transaction;
            delete.CommandText = "DELETE FROM NativeThermalDeflectionMeasurements WHERE MaterialId=$id;";
            delete.Parameters.AddWithValue("$id", materialId.Trim());
            delete.ExecuteNonQuery();
            transaction.Commit();
            return;
        }

        EnsureThermalDeflectionMethodV1(connection, transaction);
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO NativeThermalDeflectionMeasurements
                (MaterialId, ResultTemperatureC, MeasuredDate, TestNotes, MethodVersion,
                 SourceFileName, SourceSha256, ImportedAtUtc, UpdatedAtUtc)
            VALUES ($id, $value, $date, $notes, $method, 'Manual entry', $manual, $now, $now)
            ON CONFLICT(MaterialId) DO UPDATE SET
                ResultTemperatureC=excluded.ResultTemperatureC,
                MeasuredDate=excluded.MeasuredDate,
                TestNotes=excluded.TestNotes,
                MethodVersion=excluded.MethodVersion,
                SourceFileName=excluded.SourceFileName,
                SourceSha256=excluded.SourceSha256,
                UpdatedAtUtc=excluded.UpdatedAtUtc;
            """;
        command.Parameters.AddWithValue("$id", materialId.Trim());
        command.Parameters.AddWithValue("$value", resultTemperatureC.Value);
        command.Parameters.AddWithValue("$date", (object?)measuredDate ?? DBNull.Value);
        command.Parameters.AddWithValue("$notes", string.IsNullOrWhiteSpace(testNotes) ? DBNull.Value : testNotes.Trim());
        command.Parameters.AddWithValue("$method", ThermalDeflectionMethodContract.Version);
        command.Parameters.AddWithValue("$manual", ThermalDeflectionMethodContract.SnapshotSha256);
        command.Parameters.AddWithValue("$now", now);
        command.ExecuteNonQuery();
        transaction.Commit();
    }

    public bool ThermalDeflectionPopulationHasValidProvenance()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT
                COALESCE(SUM(CASE WHEN MethodVersion<>$method THEN 1 ELSE 0 END), 0),
                COALESCE(SUM(CASE WHEN ResultTemperatureC < 25 OR ResultTemperatureC > 300 THEN 1 ELSE 0 END), 0),
                COALESCE(SUM(CASE WHEN MaterialId NOT IN (SELECT MaterialId FROM NativeMaterialManagerRows) THEN 1 ELSE 0 END), 0),
                COALESCE(SUM(CASE WHEN NOT (
                    SourceSha256=$acceptedSource OR
                    (SourceFileName='Manual entry' AND SourceSha256=$manualSource)
                ) THEN 1 ELSE 0 END), 0)
            FROM NativeThermalDeflectionMeasurements;
            """;
        command.Parameters.AddWithValue("$acceptedSource", "5CC2742C6DEA382CDCCC9D260135DB3377DFC9B754D9106230B6C8CCC3AE58CE");
        command.Parameters.AddWithValue("$manualSource", ThermalDeflectionMethodContract.SnapshotSha256);
        command.Parameters.AddWithValue("$method", ThermalDeflectionMethodContract.Version);
        using var reader = command.ExecuteReader();
        return reader.Read() &&
               reader.GetInt32(0) == 0 &&
               reader.GetInt32(1) == 0 &&
               reader.GetInt32(2) == 0 &&
               reader.GetInt32(3) == 0;
    }

    public static bool RunThermalDeflectionPersistenceContractVerification()
    {
        var root = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-ThermalPersistence-" + Guid.NewGuid().ToString("N"));
        IODirectory.CreateDirectory(root);
        try
        {
            var database = new LocalDatabase(IOPath.Combine(root, "filamentdb.sqlite"));
            using (var connection = new SqliteConnection(database.ConnectionString))
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO NativeMaterialManagerRows(MaterialId, UpdatedAtUtc) VALUES ('MAT-THERMAL-1', 'verify');
                    """;
                command.ExecuteNonQuery();
            }
            var immutableMethodBlocked = false;
            try
            {
                using var connection = new SqliteConnection(database.ConnectionString);
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                    UPDATE ThermalDeflectionMethods
                    SET MethodSnapshotJson='drift'
                    WHERE MethodVersion=$version;
                    """;
                command.Parameters.AddWithValue("$version", ThermalDeflectionMethodContract.Version);
                command.ExecuteNonQuery();
            }
            catch (SqliteException) { immutableMethodBlocked = true; }
            database.SaveManualThermalDeflectionMeasurement(
                "MAT-THERMAL-1", 60, "2026-08-14", "Manual verification");
            var manual = database.GetThermalDeflectionMeasurements()
                .Single(row => row.MaterialId == "MAT-THERMAL-1");
            var manualPopulationReady = database.ThermalDeflectionPopulationHasValidProvenance();
            using (var connection = new SqliteConnection(database.ConnectionString))
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = "UPDATE NativeThermalDeflectionMeasurements SET SourceSha256='invalid-provenance';";
                command.ExecuteNonQuery();
            }
            var invalidProvenanceBlocked = !database.ThermalDeflectionPopulationHasValidProvenance();
            database.SaveManualThermalDeflectionMeasurement(
                "MAT-THERMAL-1", 61, "2026-08-15", "Updated verification");
            var manualUpdated = database.GetThermalDeflectionMeasurements()
                .Single(row => row.MaterialId == "MAT-THERMAL-1");
            database.SaveManualThermalDeflectionMeasurement(
                "MAT-THERMAL-1", null, null, null);
            return immutableMethodBlocked &&
                   manualPopulationReady && invalidProvenanceBlocked &&
                   manual.ResultTemperatureC == 60 && manual.MeasuredDate == "2026-08-14" &&
                   manual.MethodVersion == ThermalDeflectionMethodContract.Version &&
                   manualUpdated.ResultTemperatureC == 61 && manualUpdated.TestNotes == "Updated verification" &&
                   database.GetThermalDeflectionResults().Count == 0;
        }
        catch
        {
            return false;
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            if (IODirectory.Exists(root)) IODirectory.Delete(root, true);
        }
    }
}
