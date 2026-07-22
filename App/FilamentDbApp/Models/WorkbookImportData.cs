using System.Data;

namespace FilamentDbApp.Models;

public sealed class WorkbookImportData
{
    public required DataTable Materials { get; init; }
    public required IReadOnlyList<ImportedSheetData> Sheets { get; init; }
    public required string SourceFileName { get; init; }
    public required string SourcePath { get; init; }
}

public sealed class ImportedSheetData
{
    public required string SheetName { get; init; }
    public required string Purpose { get; init; }
    public int HeaderRow { get; init; }
    public int RowCount { get; init; }
    public int ColumnCount { get; init; }
    public required DataTable Table { get; init; }
}
