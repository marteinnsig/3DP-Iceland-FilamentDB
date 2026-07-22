namespace FilamentDbApp.Services;

public sealed record ConsistencyCalibrationRating(
    double Score,
    string Label,
    string Interpretation,
    int Stars);

/// <summary>
/// Canonical 3DPIceland internal comparative repeatability scale.
/// It describes repeatability inside the in-house test workflow; it is not an
/// industry standard, an accredited uncertainty statement or an accuracy correction.
/// </summary>
public static class ConsistencyCalibrationService
{
    public const string ScaleName = "3DPIceland internal comparative repeatability scale";
    public const double ReviewCvPercent = 30.0;
    public const double HighVariationCvPercent = 40.0;

    public static double? CalculateScore(IEnumerable<double?> cvPercentValues, IEnumerable<double?> sampleCounts)
    {
        var cvValues = cvPercentValues
            .Where(value => value.HasValue && double.IsFinite(value.Value) && value.Value >= 0)
            .Select(value => value!.Value)
            .ToList();
        if (cvValues.Count == 0) return null;

        var samples = sampleCounts
            .Where(value => value.HasValue && double.IsFinite(value.Value) && value.Value >= 0)
            .Select(value => value!.Value)
            .ToList();
        var averageSamples = samples.Count == 0 ? (double?)null : samples.Average();
        var samplePenalty = averageSamples.HasValue ? Math.Max(0.0, (10.0 - averageSamples.Value) * 1.5) : 0.0;
        return Math.Clamp(100.0 - cvValues.Average() - samplePenalty, 0.0, 100.0);
    }

    public static ConsistencyCalibrationRating InterpretScore(double? score)
    {
        var value = Math.Clamp(score ?? 0, 0, 100);
        return value switch
        {
            >= 90 => new(value, "Excellent repeatability", "The measured specimens are exceptionally repeatable within the 3DPIceland workflow.", 5),
            >= 85 => new(value, "Very good repeatability", "The measured specimens are very repeatable within the 3DPIceland workflow.", 4),
            >= 80 => new(value, "Good repeatability", "The measured specimens show good practical repeatability within the 3DPIceland workflow.", 4),
            >= 70 => new(value, "Moderate repeatability", "The result is usable for internal comparison, with noticeable specimen or fixture variation.", 3),
            >= 60 => new(value, "Low repeatability", "Variation is substantial and small ranking differences should be treated cautiously.", 2),
            _ => new(value, "Very low repeatability", "Variation or sample coverage is poor; review the test set and equipment context before relying on small differences.", 1)
        };
    }

    public static string ScoreBandSummary =>
        "90-100 excellent; 85-89.9 very good; 80-84.9 good; 70-79.9 moderate; 60-69.9 low; below 60 very low.";
}
