using ClosedXML.Excel;
using System.Globalization;
using System.Security.Cryptography;

namespace FilamentDbApp.Services;

public static class ThermalDeflectionMethodContract
{
    public const string Version = "3dp-thermal-deflection-fixture-v1";
    public const string SnapshotJson = """
        {"methodVersion":"3dp-thermal-deflection-fixture-v1","result":"nearby BlueDOT probe-indicated temperature at 2.00 mm mid-span deflection","standardClaim":"3DPIceland fixture-specific; not ASTM D648 or ISO 75 HDT","specimen":{"lengthMm":127.0,"widthMm":12.7,"thicknessMm":3.2,"orientation":"flat"},"fixture":{"clearSpanMm":110.0,"movingLoadG":54.0,"nominalLoadN":0.530,"load":"centered M20 nut","centralBoltAddsSpecimenLoad":false},"sensor":{"name":"BlueDOT","vendor":"thermapen.co.uk","fccId":"2A167 BlueDot","location":"nearby under specimen beside central assembly","userCalibration":false},"heating":{"environment":"oven","ambientStartC":25.0,"ramp":"non-linear observed","checkpoints":[{"temperatureC":50,"elapsed":"00:01:50"},{"temperatureC":100,"elapsed":"00:03:26"},{"temperatureC":150,"elapsed":"00:04:35"},{"temperatureC":200,"elapsed":"00:06:53"},{"temperatureC":250,"elapsed":"00:10:30"}]},"testsPerMaterial":1,"unit":"degC"}
        """;

    public static string SnapshotSha256 =>
        Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(SnapshotJson)));
}

public sealed record ThermalDeflectionImportRow(
    int SourceRow,
    string MaterialId,
    double TemperatureC,
    string Action);

public sealed record ThermalDeflectionImportIssue(
    int SourceRow,
    string MaterialId,
    string Detail);

public sealed class ThermalDeflectionImportPreview
{
    public string SourcePath { get; init; } = "";
    public string SourceFileName { get; init; } = "";
    public string SourceSha256 { get; init; } = "";
    public string WorksheetName { get; init; } = "";
    public int SourceRows { get; init; }
    public int BlankResults { get; init; }
    public IReadOnlyList<ThermalDeflectionImportRow> Rows { get; init; } = [];
    public IReadOnlyList<ThermalDeflectionImportIssue> Issues { get; init; } = [];
    public int Inserts => Rows.Count(row => row.Action == "Insert");
    public int Updates => Rows.Count(row => row.Action == "Update");
    public int Unchanged => Rows.Count(row => row.Action == "Unchanged");
    public bool CanApply => Issues.Count == 0 && Rows.Count > 0;
}

public sealed record ThermalDeflectionImportApplyResult(
    int Inserted,
    int Updated,
    int Unchanged,
    int BlankIgnored,
    string MethodVersion,
    string SourceSha256);

public sealed class ThermalDeflectionWorkbookImportService
{
    public ThermalDeflectionImportPreview Preview(
        string workbookPath,
        IReadOnlySet<string> canonicalMaterialIds,
        IReadOnlyDictionary<string, double> existingResults)
    {
        if (string.IsNullOrWhiteSpace(workbookPath) || !IOFile.Exists(workbookPath))
            throw new System.IO.FileNotFoundException("Thermal deflection workbook was not found.", workbookPath);
        if (!string.Equals(IOPath.GetExtension(workbookPath), ".xlsx", StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Thermal deflection import requires an .xlsx workbook.");

        var fullPath = IOPath.GetFullPath(workbookPath);
        var sourceHash = Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(fullPath)));
        using var workbook = new XLWorkbook(fullPath);
        var candidates = workbook.Worksheets
            .Select(sheet => new
            {
                Sheet = sheet,
                MaterialColumn = FindHeader(sheet, "MaterialID"),
                ResultColumn = FindHeader(sheet, "Hitamæling")
            })
            .Where(item => item.MaterialColumn > 0 && item.ResultColumn > 0)
            .ToList();
        if (candidates.Count != 1)
            throw new InvalidOperationException("Workbook must contain exactly one sheet with MaterialID and Hitamæling headers on row 1.");

        var selected = candidates[0];
        var rows = new List<ThermalDeflectionImportRow>();
        var issues = new List<ThermalDeflectionImportIssue>();
        var seenIds = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        var blankResults = 0;
        var lastRow = selected.Sheet.LastRowUsed()?.RowNumber() ?? 1;
        for (var sourceRow = 2; sourceRow <= lastRow; sourceRow++)
        {
            var materialId = selected.Sheet.Cell(sourceRow, selected.MaterialColumn).GetString().Trim();
            var resultCell = selected.Sheet.Cell(sourceRow, selected.ResultColumn);
            if (string.IsNullOrWhiteSpace(materialId) && resultCell.IsEmpty()) continue;
            if (string.IsNullOrWhiteSpace(materialId))
            {
                issues.Add(new(sourceRow, "", "MaterialID is required when Hitamæling has a value."));
                continue;
            }
            if (!seenIds.Add(materialId))
            {
                issues.Add(new(sourceRow, materialId, "Duplicate MaterialID in workbook."));
                continue;
            }
            if (!canonicalMaterialIds.Contains(materialId))
            {
                issues.Add(new(sourceRow, materialId, "MaterialID does not exist in canonical SQLite Materials."));
                continue;
            }
            if (resultCell.IsEmpty() || string.IsNullOrWhiteSpace(resultCell.GetString()))
            {
                blankResults++;
                continue;
            }
            if (!TryReadTemperature(resultCell, out var temperature))
            {
                issues.Add(new(sourceRow, materialId, "Hitamæling must be a finite numeric Celsius value."));
                continue;
            }
            if (temperature < 25 || temperature > 300)
            {
                issues.Add(new(sourceRow, materialId, "Hitamæling must be between 25 and 300 °C for method v1."));
                continue;
            }
            var action = !existingResults.TryGetValue(materialId, out var existing)
                ? "Insert"
                : Math.Abs(existing - temperature) < 0.0000001
                    ? "Unchanged"
                    : "Update";
            rows.Add(new(sourceRow, materialId, temperature, action));
        }

        return new ThermalDeflectionImportPreview
        {
            SourcePath = fullPath,
            SourceFileName = IOPath.GetFileName(fullPath),
            SourceSha256 = sourceHash,
            WorksheetName = selected.Sheet.Name,
            SourceRows = Math.Max(0, lastRow - 1),
            BlankResults = blankResults,
            Rows = rows,
            Issues = issues
        };
    }

    private static int FindHeader(IXLWorksheet worksheet, string expected)
    {
        var lastColumn = worksheet.Row(1).LastCellUsed()?.Address.ColumnNumber ?? 0;
        var matches = Enumerable.Range(1, lastColumn)
            .Where(column => string.Equals(
                worksheet.Cell(1, column).GetString().Trim(),
                expected,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
        return matches.Count == 1 ? matches[0] : 0;
    }

    private static bool TryReadTemperature(IXLCell cell, out double value)
    {
        if (cell.TryGetValue<double>(out value)) return double.IsFinite(value);
        var text = cell.GetString().Trim();
        foreach (var culture in new[] { CultureInfo.InvariantCulture, CultureInfo.GetCultureInfo("is-IS"), CultureInfo.CurrentCulture })
        {
            if (double.TryParse(text, NumberStyles.Float, culture, out value) && double.IsFinite(value)) return true;
        }
        value = 0;
        return false;
    }
}
