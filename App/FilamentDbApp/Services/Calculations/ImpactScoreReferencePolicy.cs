namespace FilamentDbApp.Services.Calculations;

/// <summary>Versioned references are independent of the visible material cohort.
/// Null means no approved reference exists; measured strength remains available.</summary>
public sealed record ImpactScoreReferencePolicy(string Version, double? IzodKjM2, double? CharpyKjM2)
{
    public static ImpactScoreReferencePolicy Current { get; } = new("impact-reference-v1-unconfigured", null, null);

    public double? ReferenceFor(string method)
    {
        var value = method switch { "Izod" => IzodKjM2, "Charpy" => CharpyKjM2, _ => null };
        return !string.IsNullOrWhiteSpace(Version) && value is > 0 && double.IsFinite(value.Value) ? value : null;
    }
}
