namespace FilamentDbApp.Services.Calculations;

public sealed class RatingService : IRatingService
{
    private readonly IStatisticsService _statisticsService;

    public RatingService(IStatisticsService? statisticsService = null)
    {
        _statisticsService = statisticsService ?? new StatisticsService();
    }

    public RatingResult BuildReliabilityRating(double? flatCvPercent, double? uprightCvPercent, double? flatSamples, double? uprightSamples)
    {
        var sampleValues = new[] { flatSamples, uprightSamples }
            .Where(value => value.HasValue && value.Value > 0)
            .Select(value => value!.Value)
            .ToList();

        var cvValues = new[] { flatCvPercent, uprightCvPercent }
            .Where(value => value.HasValue && value.Value >= 0)
            .Select(value => value!.Value)
            .ToList();

        if (sampleValues.Count == 0 && cvValues.Count == 0)
        {
            return new RatingResult(0, string.Empty, "No reliability data imported for this test yet.");
        }

        var score = global::FilamentDbApp.Services.ConsistencyCalibrationService.CalculateScore(
            cvValues.Select(value => (double?)value),
            sampleValues.Select(value => (double?)value));
        var rating = global::FilamentDbApp.Services.ConsistencyCalibrationService.InterpretScore(score);
        return new RatingResult(rating.Stars, rating.Label, rating.Interpretation);
    }

    public RatingResult BuildCompletenessRating(int testedSamples, int expectedSamples = 10)
    {
        if (expectedSamples <= 0) expectedSamples = 10;
        var clampedSamples = Math.Clamp(testedSamples, 0, expectedSamples);

        if (clampedSamples <= 0)
        {
            return new RatingResult(0, string.Empty, "No tested samples recorded yet.");
        }

        var completion = (double)clampedSamples / expectedSamples;
        var stars = Math.Clamp((int)Math.Ceiling(completion * 5), 1, 5);

        return stars switch
        {
            5 => new RatingResult(5, "Complete", "The expected sample set is complete."),
            4 => new RatingResult(4, "Mostly complete", "Most expected samples are present."),
            3 => new RatingResult(3, "Partially complete", "The test has a usable but incomplete sample set."),
            2 => new RatingResult(2, "Limited", "The test has limited sample coverage."),
            _ => new RatingResult(1, "Very limited", "The test is based on very few samples.")
        };
    }

    public int? StarCountFromConfidence(int? confidence, int maximumConfidence = 10)
    {
        if (!confidence.HasValue || confidence.Value <= 0) return null;
        maximumConfidence = Math.Max(1, maximumConfidence);

        var normalized = Math.Clamp((double)confidence.Value / maximumConfidence, 0, 1);
        return Math.Clamp((int)Math.Ceiling(normalized * 5), 1, 5);
    }

    public string FormatStars(int? stars, int maximumStars = 5)
    {
        if (!stars.HasValue || stars.Value <= 0) return "—";

        maximumStars = Math.Max(1, maximumStars);
        var clampedStars = Math.Clamp(stars.Value, 0, maximumStars);
        return new string('★', clampedStars) + new string('☆', maximumStars - clampedStars);
    }

    public double? NormalizeToScore(double? value, double? minimum, double? maximum, bool higherIsBetter = true)
    {
        if (!value.HasValue || !minimum.HasValue || !maximum.HasValue) return null;
        if (double.IsNaN(value.Value) || double.IsInfinity(value.Value)) return null;
        if (double.IsNaN(minimum.Value) || double.IsInfinity(minimum.Value)) return null;
        if (double.IsNaN(maximum.Value) || double.IsInfinity(maximum.Value)) return null;

        var range = maximum.Value - minimum.Value;
        if (Math.Abs(range) < double.Epsilon) return null;

        var normalized = (value.Value - minimum.Value) / range;
        normalized = Math.Clamp(normalized, 0, 1);
        return higherIsBetter ? normalized : 1 - normalized;
    }
}
