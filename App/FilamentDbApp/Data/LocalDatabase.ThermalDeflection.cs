using ClosedXML.Excel;
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
        string MethodVersion);

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
            SELECT MaterialId, ResultTemperatureC, MeasuredDate, TestNotes, MethodVersion
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
                reader.GetString(4)));
        return rows;
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

    public ThermalDeflectionImportApplyResult ApplyThermalDeflectionImport(ThermalDeflectionImportPreview preview)
    {
        if (!preview.CanApply) throw new InvalidOperationException("Thermal deflection preview contains blocking issues or no measured rows.");
        var verifiedPreview = new ThermalDeflectionWorkbookImportService().Preview(
            preview.SourcePath,
            GetCanonicalThermalDeflectionMaterialIds(),
            GetThermalDeflectionResults());
        if (!verifiedPreview.CanApply ||
            !string.Equals(verifiedPreview.SourceSha256, preview.SourceSha256, StringComparison.Ordinal) ||
            verifiedPreview.BlankResults != preview.BlankResults ||
            !verifiedPreview.Rows.SequenceEqual(preview.Rows))
            throw new InvalidOperationException("Thermal deflection workbook or canonical state changed after preview; preview again before apply.");

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        EnsureThermalDeflectionMethodV1(connection, transaction);
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        foreach (var row in verifiedPreview.Rows.Where(row => row.Action != "Unchanged"))
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = """
                INSERT INTO NativeThermalDeflectionMeasurements
                    (MaterialId, ResultTemperatureC, MeasuredDate, TestNotes, MethodVersion,
                     SourceFileName, SourceSha256, ImportedAtUtc, UpdatedAtUtc)
                VALUES ($id, $value, NULL, NULL, $method, $file, $sha, $now, $now)
                ON CONFLICT(MaterialId) DO UPDATE SET
                    ResultTemperatureC=excluded.ResultTemperatureC,
                    MethodVersion=excluded.MethodVersion,
                    SourceFileName=excluded.SourceFileName,
                    SourceSha256=excluded.SourceSha256,
                    ImportedAtUtc=excluded.ImportedAtUtc,
                    UpdatedAtUtc=excluded.UpdatedAtUtc;
                """;
            command.Parameters.AddWithValue("$id", row.MaterialId);
            command.Parameters.AddWithValue("$value", row.TemperatureC);
            command.Parameters.AddWithValue("$method", ThermalDeflectionMethodContract.Version);
            command.Parameters.AddWithValue("$file", preview.SourceFileName);
            command.Parameters.AddWithValue("$sha", preview.SourceSha256);
            command.Parameters.AddWithValue("$now", now);
            command.ExecuteNonQuery();
        }
        transaction.Commit();
        return new(
            verifiedPreview.Inserts,
            verifiedPreview.Updates,
            verifiedPreview.Unchanged,
            verifiedPreview.BlankResults,
            ThermalDeflectionMethodContract.Version,
            verifiedPreview.SourceSha256);
    }

    public static bool RunThermalDeflectionImportContractVerification()
    {
        var root = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-ThermalImport-" + Guid.NewGuid().ToString("N"));
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
                    INSERT INTO NativeMaterialManagerRows(MaterialId, UpdatedAtUtc) VALUES ('MAT-THERMAL-2', 'verify');
                    """;
                command.ExecuteNonQuery();
            }
            var workbookPath = IOPath.Combine(root, "thermal.xlsx");
            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Sheet1");
                sheet.Cell(1, 1).Value = "MaterialID";
                sheet.Cell(1, 2).Value = "Hitamæling";
                sheet.Cell(2, 1).Value = "MAT-THERMAL-1";
                sheet.Cell(2, 2).Value = 51;
                sheet.Cell(3, 1).Value = "MAT-THERMAL-2";
                workbook.SaveAs(workbookPath);
            }
            var service = new ThermalDeflectionWorkbookImportService();
            var preview = service.Preview(
                workbookPath,
                database.GetCanonicalThermalDeflectionMaterialIds(),
                database.GetThermalDeflectionResults());
            var applied = database.ApplyThermalDeflectionImport(preview);
            var stored = database.GetThermalDeflectionResults();
            var repeat = service.Preview(
                workbookPath,
                database.GetCanonicalThermalDeflectionMaterialIds(),
                stored);
            var invalidPath = IOPath.Combine(root, "thermal-invalid.xlsx");
            using (var workbook = new XLWorkbook())
            {
                var sheet = workbook.Worksheets.Add("Sheet1");
                sheet.Cell(1, 1).Value = "MaterialID";
                sheet.Cell(1, 2).Value = "Hitamæling";
                sheet.Cell(2, 1).Value = "MAT-THERMAL-1";
                sheet.Cell(2, 2).Value = 24;
                sheet.Cell(3, 1).Value = "MAT-THERMAL-2";
                sheet.Cell(3, 2).Value = "not-a-number";
                sheet.Cell(4, 1).Value = "MAT-UNKNOWN";
                sheet.Cell(4, 2).Value = 60;
                sheet.Cell(5, 1).Value = "MAT-THERMAL-1";
                sheet.Cell(5, 2).Value = 52;
                workbook.SaveAs(invalidPath);
            }
            var invalid = service.Preview(
                invalidPath,
                database.GetCanonicalThermalDeflectionMaterialIds(),
                stored);
            var invalidApplyBlocked = false;
            try { database.ApplyThermalDeflectionImport(invalid); }
            catch (InvalidOperationException) { invalidApplyBlocked = true; }
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
                "MAT-THERMAL-2", 60, "2026-08-14", "Manual verification");
            var manual = database.GetThermalDeflectionMeasurements()
                .Single(row => row.MaterialId == "MAT-THERMAL-2");
            database.SaveManualThermalDeflectionMeasurement(
                "MAT-THERMAL-2", 61, "2026-08-15", "Updated verification");
            var manualUpdated = database.GetThermalDeflectionMeasurements()
                .Single(row => row.MaterialId == "MAT-THERMAL-2");
            database.SaveManualThermalDeflectionMeasurement(
                "MAT-THERMAL-2", null, null, null);
            return preview.CanApply && preview.SourceRows == 2 && preview.Inserts == 1 &&
                   preview.BlankResults == 1 && applied.Inserted == 1 &&
                   stored.Count == 1 && stored["MAT-THERMAL-1"] == 51 &&
                   repeat.CanApply && repeat.Unchanged == 1 && repeat.Inserts == 0 &&
                   !invalid.CanApply && invalid.Issues.Count == 4 &&
                   invalidApplyBlocked && immutableMethodBlocked &&
                   manual.ResultTemperatureC == 60 && manual.MeasuredDate == "2026-08-14" &&
                   manual.MethodVersion == ThermalDeflectionMethodContract.Version &&
                   manualUpdated.ResultTemperatureC == 61 && manualUpdated.TestNotes == "Updated verification" &&
                   database.GetThermalDeflectionResults().Count == 1;
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
