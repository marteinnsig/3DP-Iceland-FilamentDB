namespace FilamentDbApp.Services.Calculations;

/// <summary>Display-only rig-capacity reference, never a modern material score.</summary>
public static class LegacyImpactRadarService
{
    public const string Description = "Legacy Impact: mean of available Flat/Upright kJ/m² divided by the matching rig maximum × 100. Percent of rig capacity, not a material-quality score; excluded from Overall, Consistency and recommendations. Radar is capped at 100%.";

    public static double? Calculate(double? upright, int uprightSamples, double? flat, int flatSamples, double? maximumKjM2)
    {
        if (maximumKjM2 is not > 0 || !double.IsFinite(maximumKjM2.Value)) return null;
        var values = new[] { (upright, uprightSamples), (flat, flatSamples) }
            .Where(item => item.Item2 > 0 && item.Item1 is >= 0 && double.IsFinite(item.Item1.Value))
            .Select(item => item.Item1!.Value).ToArray();
        if (values.Length == 0) return null;
        var mean = values.Sum(value => value / values.Length);
        return Math.Clamp(mean / maximumKjM2.Value * 100d, 0d, 100d);
    }
}
