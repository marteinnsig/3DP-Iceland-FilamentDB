namespace FilamentDbApp.Services.Calculations;

/// <summary>Versioned references are independent of the visible material cohort.
/// Null means no approved reference exists; measured strength remains available.</summary>
public sealed record ImpactScoreReferencePolicy(string Version, double? IzodKjM2, double? CharpyKjM2)
{
    // Owner-approved comparison references (2026-09-29), not ISO ratings or hammer limits.
    public static ImpactScoreReferencePolicy Current { get; } = new("impact-reference-v2-80-kjm2", 80, 80);

    public double? ReferenceFor(string method)
    {
        var value = method switch { "Izod" => IzodKjM2, "Charpy" => CharpyKjM2, _ => null };
        return !string.IsNullOrWhiteSpace(Version) && value is > 0 && double.IsFinite(value.Value) ? value : null;
    }
}
