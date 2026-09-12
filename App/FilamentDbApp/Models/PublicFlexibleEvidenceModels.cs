namespace FilamentDbApp.Models;

/// <summary>Explicit public aggregate allowlist; no raw method metadata or specimen identity.</summary>
public sealed record PublicFlexibleMetricGroup(
    string GroupId, string Metric, string Condition, int SpecimenCount,
    double? Mean, double? StandardDeviation, double? CoefficientOfVariation,
    double? Minimum, double? Maximum, string Unit, int NotReachedCount)
{
    public string MethodId { get; init; } = string.Empty;
    public IReadOnlyList<PublicShoreSpecimenStatistics> ShoreSpecimens { get; init; } = [];
}

// Group-local number only: public output does not expose saved specimen IDs or freeform labels.
public sealed record PublicShoreSpecimenStatistics(int SpecimenNumber, int ReadingCount,
    double? Mean, double? StandardDeviation, double? CoefficientOfVariation);
