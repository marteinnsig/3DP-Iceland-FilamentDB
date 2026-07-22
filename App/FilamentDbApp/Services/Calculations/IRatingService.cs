namespace FilamentDbApp.Services.Calculations;

public interface IRatingService
{
    RatingResult BuildReliabilityRating(double? flatCvPercent, double? uprightCvPercent, double? flatSamples, double? uprightSamples);
    RatingResult BuildCompletenessRating(int testedSamples, int expectedSamples = 10);
    int? StarCountFromConfidence(int? confidence, int maximumConfidence = 10);
    string FormatStars(int? stars, int maximumStars = 5);
    double? NormalizeToScore(double? value, double? minimum, double? maximum, bool higherIsBetter = true);
}
