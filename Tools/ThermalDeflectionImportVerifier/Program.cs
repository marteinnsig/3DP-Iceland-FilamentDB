using FilamentDbApp.Services;
using Microsoft.Data.Sqlite;
using System.Text.Json;
using IOFile = System.IO.File;
using IOPath = System.IO.Path;

if (args.Length != 2)
{
    Console.Error.WriteLine("Usage: ThermalDeflectionImportVerifier <workbook.xlsx> <database.sqlite>");
    return 64;
}

var workbookPath = IOPath.GetFullPath(args[0]);
var databasePath = IOPath.GetFullPath(args[1]);
if (!IOFile.Exists(databasePath))
{
    Console.Error.WriteLine("Database was not found: " + databasePath);
    return 66;
}

var materialIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
var existing = new Dictionary<string, double>(StringComparer.OrdinalIgnoreCase);
using (var connection = new SqliteConnection(
           $"Data Source={databasePath};Mode=ReadOnly;Pooling=False"))
{
    connection.Open();
    using (var materials = connection.CreateCommand())
    {
        materials.CommandText = "SELECT MaterialId FROM NativeMaterialManagerRows WHERE TRIM(MaterialId) <> '';";
        using var reader = materials.ExecuteReader();
        while (reader.Read()) materialIds.Add(reader.GetString(0).Trim());
    }
    using (var table = connection.CreateCommand())
    {
        table.CommandText = "SELECT COUNT(*) FROM sqlite_schema WHERE type='table' AND name='NativeThermalDeflectionMeasurements';";
        if (Convert.ToInt32(table.ExecuteScalar()) > 0)
        {
            using var results = connection.CreateCommand();
            results.CommandText = "SELECT MaterialId, ResultTemperatureC FROM NativeThermalDeflectionMeasurements;";
            using var reader = results.ExecuteReader();
            while (reader.Read()) existing.Add(reader.GetString(0), reader.GetDouble(1));
        }
    }
}

var preview = new ThermalDeflectionWorkbookImportService().Preview(
    workbookPath,
    materialIds,
    existing);
Console.WriteLine(JsonSerializer.Serialize(new
{
    schema = "3dpiceland.thermal-deflection-import-preview.v1",
    preview.SourceFileName,
    preview.SourceSha256,
    preview.WorksheetName,
    preview.SourceRows,
    measuredRows = preview.Rows.Count,
    preview.BlankResults,
    preview.Inserts,
    preview.Updates,
    preview.Unchanged,
    preview.CanApply,
    issues = preview.Issues
}, new JsonSerializerOptions { WriteIndented = true }));
return preview.CanApply ? 0 : 2;
