using ClosedXML.Excel;
using System.Data;

namespace FilamentDbApp.Services;

public sealed class ExcelMaterialImporter
{
    private static readonly string[] PreferredSheetNames =
    {
        "00 Materials",
        "Materials",
        "Material List",
        "00 materials"
    };

    public DataTable ImportMaterialsSheet(string workbookPath)
    {
        using var workbook = new XLWorkbook(workbookPath);
        var worksheet = FindMaterialWorksheet(workbook);
        var headerRow = FindHeaderRow(worksheet);
        var used = worksheet.RangeUsed() ?? throw new InvalidOperationException("The selected workbook appears to be empty.");
        var lastRow = used.RangeAddress.LastAddress.RowNumber;

        // Use the header row to determine the real material columns.
        // This prevents hidden/blank areas of the sheet from changing the import,
        // and guarantees columns like G: "Variant / Finish" are included.
        var lastColumn = worksheet.Row(headerRow)
            .CellsUsed()
            .Select(c => c.Address.ColumnNumber)
            .DefaultIfEmpty(used.RangeAddress.LastAddress.ColumnNumber)
            .Max();

        var table = new DataTable("Materials");
        var columnNames = new List<string>();

        for (var col = 1; col <= lastColumn; col++)
        {
            var rawHeader = worksheet.Cell(headerRow, col).GetFormattedString().Trim();
            var columnName = string.IsNullOrWhiteSpace(rawHeader) ? $"Column {col}" : rawHeader;
            columnName = MakeUniqueColumnName(columnName, columnNames);
            columnNames.Add(columnName);
            table.Columns.Add(columnName, typeof(string));
        }

        for (var row = headerRow + 1; row <= lastRow; row++)
        {
            var values = new object[columnNames.Count];
            var hasAnyValue = false;

            for (var col = 1; col <= lastColumn; col++)
            {
                var value = worksheet.Cell(row, col).GetFormattedString().Trim();
                values[col - 1] = value;
                if (!string.IsNullOrWhiteSpace(value)) hasAnyValue = true;
            }

            if (hasAnyValue)
            {
                table.Rows.Add(values);
            }
        }

        return table;
    }

    private static string MakeUniqueColumnName(string baseName, List<string> existing)
    {
        var name = baseName;
        var counter = 2;
        while (existing.Any(x => string.Equals(x, name, StringComparison.OrdinalIgnoreCase)))
        {
            name = $"{baseName} {counter}";
            counter++;
        }
        return name;
    }

    private static IXLWorksheet FindMaterialWorksheet(XLWorkbook workbook)
    {
        foreach (var name in PreferredSheetNames)
        {
            var sheet = workbook.Worksheets.FirstOrDefault(s => string.Equals(s.Name, name, StringComparison.OrdinalIgnoreCase));
            if (sheet is not null) return sheet;
        }

        var scored = workbook.Worksheets
            .Select(sheet => new { Sheet = sheet, Score = ScoreWorksheet(sheet) })
            .OrderByDescending(x => x.Score)
            .FirstOrDefault();

        if (scored is null || scored.Score == 0)
        {
            throw new InvalidOperationException("Could not find a material list sheet. Expected a sheet like '00 Materials'.");
        }

        return scored.Sheet;
    }

    private static int ScoreWorksheet(IXLWorksheet sheet)
    {
        var used = sheet.RangeUsed();
        if (used is null) return 0;

        var maxRows = Math.Min(10, used.RangeAddress.LastAddress.RowNumber);
        var maxCols = Math.Min(40, used.RangeAddress.LastAddress.ColumnNumber);
        var score = 0;

        for (var row = 1; row <= maxRows; row++)
        {
            for (var col = 1; col <= maxCols; col++)
            {
                var text = sheet.Cell(row, col).GetFormattedString().Trim();
                if (string.Equals(text, "Material ID", StringComparison.OrdinalIgnoreCase)) score += 10;
                if (string.Equals(text, "Manufacturer", StringComparison.OrdinalIgnoreCase)) score += 5;
                if (string.Equals(text, "Base Material", StringComparison.OrdinalIgnoreCase)) score += 5;
                if (string.Equals(text, "Reinforcement", StringComparison.OrdinalIgnoreCase)) score += 5;
            }
        }

        return score;
    }

    private static int FindHeaderRow(IXLWorksheet worksheet)
    {
        var used = worksheet.RangeUsed() ?? throw new InvalidOperationException("The material sheet appears to be empty.");
        var lastRowToCheck = Math.Min(20, used.RangeAddress.LastAddress.RowNumber);
        var lastColToCheck = Math.Min(60, used.RangeAddress.LastAddress.ColumnNumber);

        for (var row = 1; row <= lastRowToCheck; row++)
        {
            for (var col = 1; col <= lastColToCheck; col++)
            {
                var text = worksheet.Cell(row, col).GetFormattedString().Trim();
                if (string.Equals(text, "Material ID", StringComparison.OrdinalIgnoreCase))
                {
                    return row;
                }
            }
        }

        throw new InvalidOperationException("Could not find the header row in the material sheet. Expected a 'Material ID' column.");
    }
}
