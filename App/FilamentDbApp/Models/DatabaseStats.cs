namespace FilamentDbApp.Models;

public sealed class DatabaseStats
{
    public int Materials { get; init; }
    public int Manufacturers { get; init; }
    public int ImportedSheets { get; init; }
    public int ImportedRows { get; init; }
    public int ImportedCells { get; init; }
    public int TestSummaryValues { get; init; }
    public string? LastImportedFile { get; init; }
    public string? LastImportedAtUtc { get; init; }
}
