namespace FilamentDbApp.Services.Calculations;

public sealed record ImpactResults(
    MeasurementSetResult Upright,
    MeasurementSetResult Flat,
    double NoSampleAngleDegrees,
    double NetCrossSectionAreaM2,
    double MaxPossibleImpact,
    DateTime CalculatedAtUtc);
