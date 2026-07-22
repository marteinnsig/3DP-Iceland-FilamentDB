using ClosedXML.Excel;
using FilamentDbApp.Models;
using System.Data;
using System.IO;

namespace FilamentDbApp.Services;

public sealed class ExcelWorkbookImporter
{
    private static readonly HashSet<string> SheetsToImport = new(StringComparer.OrdinalIgnoreCase)
    {
        "00 Materials",
        "01 Tensile Measurements",
        "02 Impact Measurements",
        "03 Stiffness Measurements",
        "06 Website Export"
    };

    public WorkbookImportData ImportWorkbook(string workbookPath)
    {
        using var workbook = new XLWorkbook(workbookPath);
        var materialsImporter = new ExcelMaterialImporter();
        var materials = materialsImporter.ImportMaterialsSheet(workbookPath);

        var sheets = new List<ImportedSheetData>();
        foreach (var worksheet in workbook.Worksheets.Where(w => SheetsToImport.Contains(w.Name)))
        {
            var sheetData = ImportWorksheet(worksheet);
            if (sheetData.Table.Columns.Count > 0)
            {
                sheets.Add(sheetData);
            }
        }

        return new WorkbookImportData
        {
            Materials = materials,
            Sheets = sheets,
            SourceFileName = Path.GetFileName(workbookPath),
            SourcePath = workbookPath
        };
    }

    private static ImportedSheetData ImportWorksheet(IXLWorksheet worksheet)
    {
        var used = worksheet.RangeUsed();
        if (used is null)
        {
            return new ImportedSheetData
            {
                SheetName = worksheet.Name,
                Purpose = ClassifySheet(worksheet.Name),
                HeaderRow = 0,
                RowCount = 0,
                ColumnCount = 0,
                Table = new DataTable(worksheet.Name)
            };
        }

        var headerRow = FindHeaderRow(worksheet);
        if (headerRow == 0)
        {
            return new ImportedSheetData
            {
                SheetName = worksheet.Name,
                Purpose = ClassifySheet(worksheet.Name),
                HeaderRow = 0,
                RowCount = 0,
                ColumnCount = 0,
                Table = new DataTable(worksheet.Name)
            };
        }

        var lastRow = used.RangeAddress.LastAddress.RowNumber;
        var lastColumn = worksheet.Row(headerRow)
            .CellsUsed()
            .Select(c => c.Address.ColumnNumber)
            .DefaultIfEmpty(used.RangeAddress.LastAddress.ColumnNumber)
            .Max();

        var table = new DataTable(worksheet.Name);
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

        return new ImportedSheetData
        {
            SheetName = worksheet.Name,
            Purpose = ClassifySheet(worksheet.Name),
            HeaderRow = headerRow,
            RowCount = table.Rows.Count,
            ColumnCount = table.Columns.Count,
            Table = table
        };
    }

    private static int FindHeaderRow(IXLWorksheet worksheet)
    {
        var used = worksheet.RangeUsed();
        if (used is null) return 0;

        var lastRowToCheck = Math.Min(20, used.RangeAddress.LastAddress.RowNumber);
        var lastColToCheck = Math.Min(100, used.RangeAddress.LastAddress.ColumnNumber);

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

        return 0;
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

    private static string ClassifySheet(string sheetName)
    {
        if (sheetName.Contains("Tensile", StringComparison.OrdinalIgnoreCase)) return "Mechanical test - tensile";
        if (sheetName.Contains("Impact", StringComparison.OrdinalIgnoreCase)) return "Mechanical test - impact";
        if (sheetName.Contains("Stiffness", StringComparison.OrdinalIgnoreCase)) return "Mechanical test - stiffness";
        if (sheetName.Contains("Website", StringComparison.OrdinalIgnoreCase)) return "Website export summary";
        if (sheetName.Contains("Materials", StringComparison.OrdinalIgnoreCase)) return "Material master data";
        return "Imported workbook sheet";
    }
}
