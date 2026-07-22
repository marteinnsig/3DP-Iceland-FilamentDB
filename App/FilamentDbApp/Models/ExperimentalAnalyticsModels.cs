namespace FilamentDbApp.Models;

public sealed record ExperimentalAnalyticsInput(
    string ExperimentalRunId,
    string RunLabel,
    bool IsBaseline,
    double? TensileUpright,
    double? TensileFlat,
    double? ImpactUpright,
    double? ImpactFlat,
    double? Stiffness);

public sealed record ExperimentalRankedRun(
    string ExperimentalRunId,
    string RunLabel,
    int Rank,
    double OverallScore);

public sealed record ExperimentalBestResult(string RunLabel, double Value);

public sealed class ExperimentalAnalyticsResult
{
    public string BaselineLabel { get; init; } = string.Empty;
    public ExperimentalBestResult? BestTensileUpright { get; init; }
    public ExperimentalBestResult? BestTensileFlat { get; init; }
    public ExperimentalBestResult? BestImpactUpright { get; init; }
    public ExperimentalBestResult? BestImpactFlat { get; init; }
    public ExperimentalBestResult? BestStiffness { get; init; }
    public ExperimentalRankedRun? RecommendedRun { get; init; }
    public IReadOnlyList<ExperimentalRankedRun> RankedRuns { get; init; } = Array.Empty<ExperimentalRankedRun>();
}
