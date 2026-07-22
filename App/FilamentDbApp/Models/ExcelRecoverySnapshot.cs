namespace FilamentDbApp.Models;

public sealed class ExcelRecoverySnapshot
{
    public const string CurrentFormatVersion = "1";
    public string FormatVersion { get; init; } = CurrentFormatVersion;
    public int SourceSchemaVersion { get; init; }
    public string ExportedAtUtc { get; init; } = string.Empty;
    public List<ExcelRecoveryTable> Tables { get; init; } = new();
}

public sealed class ExcelRecoveryTable
{
    public string TableName { get; init; } = string.Empty;
    public string SheetName { get; init; } = string.Empty;
    public List<string> Columns { get; init; } = new();
    public List<List<object?>> Rows { get; init; } = new();
    public string Sha256 { get; set; } = string.Empty;
}

public sealed class ExcelRecoveryRestoreResult
{
    public required string RecoveryBackupPath { get; init; }
    public int TablesRestored { get; init; }
    public long RowsRestored { get; init; }
    public int MaterialsRestored { get; init; }
}
