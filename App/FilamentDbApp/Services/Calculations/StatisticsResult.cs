namespace FilamentDbApp.Services.Calculations;

public sealed record StatisticsResult(
    int SampleCount,
    double? Average,
    double? StandardDeviationSample,
    double? CoefficientOfVariation,
    int? Confidence);
