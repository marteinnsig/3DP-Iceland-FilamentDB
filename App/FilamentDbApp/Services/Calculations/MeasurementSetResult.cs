namespace FilamentDbApp.Services.Calculations;

public sealed record MeasurementSetResult(
    double? Average,
    double? StandardDeviation,
    double? CoefficientOfVariation,
    int SampleCount,
    int? Confidence,
    RatingResult CompletenessRating);
