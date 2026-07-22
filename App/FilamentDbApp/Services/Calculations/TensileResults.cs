namespace FilamentDbApp.Services.Calculations;

public sealed record TensileResults(
    MeasurementSetResult Upright,
    MeasurementSetResult Flat,
    double CrossSectionAreaMm2,
    DateTime CalculatedAtUtc);
