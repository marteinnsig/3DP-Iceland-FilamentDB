namespace FilamentDbApp.Models;

public sealed class ExperimentalSeriesResultRow
{
    public string ExperimentalRunId { get; init; } = string.Empty;
    public string RunLabel { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public int Rank { get; init; }
    public string RankDisplay => Rank > 0 ? $"#{Rank}" : string.Empty;
    public string OverallScore { get; init; } = string.Empty;
    public bool IsBaseline { get; init; }
    public string BaselineMarker => IsBaseline ? "★ Baseline" : string.Empty;
    public string TensileUprightAverage { get; init; } = string.Empty;
    public string TensileUprightDelta { get; init; } = string.Empty;
    public string TensileUprightCv { get; init; } = string.Empty;
    public string TensileFlatAverage { get; init; } = string.Empty;
    public string TensileFlatDelta { get; init; } = string.Empty;
    public string TensileFlatCv { get; init; } = string.Empty;
    public string ImpactUprightAverage { get; init; } = string.Empty;
    public string ImpactUprightDelta { get; init; } = string.Empty;
    public string ImpactUprightCv { get; init; } = string.Empty;
    public string ImpactFlatAverage { get; init; } = string.Empty;
    public string ImpactFlatDeltaDisplay { get; init; } = string.Empty;
    public string ImpactFlatCv { get; init; } = string.Empty;
    public string StiffnessAverage { get; init; } = string.Empty;
    public string StiffnessDelta { get; init; } = string.Empty;
}
