namespace FilamentDbApp.Services.Calculations;

public sealed record StiffnessResults(
    double? DeflectionMm,
    double? ModulusMpa,
    RatingResult CompletenessRating,
    DateTime CalculatedAtUtc);
