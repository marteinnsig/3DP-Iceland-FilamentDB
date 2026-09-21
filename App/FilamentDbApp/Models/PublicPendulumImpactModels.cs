namespace FilamentDbApp.Models;

/// <summary>Detached aggregate projection. No printer, specimen IDs, private notes or override text.</summary>
public sealed record PublicPendulumImpactGroup(string GroupId, string Method, int RunNumber,
    string Date, string Condition, string Batch, string Orientation, string SpecimenType,
    string NotchType, string StandardReference, string Conformity, PendulumImpactSummary Statistics)
{
    public string InputMode { get; init; } = "Energy";
}
