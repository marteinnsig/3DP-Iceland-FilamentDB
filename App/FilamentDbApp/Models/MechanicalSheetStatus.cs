namespace FilamentDbApp.Models;

public sealed class MechanicalSheetStatus
{
    public required string TestType { get; init; }
    public required string SheetName { get; init; }
    public bool HasRowsForMaterial { get; init; }
    public int NonEmptyValues { get; init; }
}
