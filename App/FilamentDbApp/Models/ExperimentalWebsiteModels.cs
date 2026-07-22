namespace FilamentDbApp.Models;

public sealed record ExperimentalWebsiteRunInput(
    string ExperimentalRunId,
    string Label,
    string Status,
    bool IsBaseline,
    double? ParameterValue,
    double? TensileStrength,
    double? TensileStrengthCv,
    double? LayerAdhesionStrength,
    double? LayerAdhesionStrengthCv,
    double? ImpactFlat,
    double? ImpactFlatCv,
    double? ImpactUpright,
    double? ImpactUprightCv,
    double? Stiffness);

public sealed record ExperimentalWebsiteSeriesInput(
    string MaterialExperimentId,
    string MaterialId,
    string MaterialName,
    string ExperimentName,
    string ParameterUnit,
    string Notes,
    IReadOnlyList<ExperimentalWebsiteRunInput> Runs);

public sealed class ExperimentalWebsitePayload
{
    public IReadOnlyList<ExperimentalWebsiteSeriesPayload> Series { get; init; } = Array.Empty<ExperimentalWebsiteSeriesPayload>();
}

public sealed class ExperimentalWebsiteSeriesPayload
{
    public string SeriesId { get; init; } = string.Empty;
    public string MaterialId { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string ExperimentName { get; init; } = string.Empty;
    public string ParameterUnit { get; init; } = string.Empty;
    public string Notes { get; init; } = string.Empty;
    public string BaselineLabel { get; init; } = string.Empty;
    public string Quality { get; init; } = string.Empty;
    public string QualityDetail { get; init; } = string.Empty;
    public int CompletedRuns { get; init; }
    public int MissingResults { get; init; }
    public ExperimentalWebsiteBestPayload? BestTensile { get; init; }
    public ExperimentalWebsiteBestPayload? BestImpact { get; init; }
    public ExperimentalWebsiteBestPayload? BestStiffness { get; init; }
    public ExperimentalWebsiteBestPayload? Recommended { get; init; }
    public IReadOnlyList<ExperimentalWebsiteRunPayload> Runs { get; init; } = Array.Empty<ExperimentalWebsiteRunPayload>();
}

public sealed record ExperimentalWebsiteBestPayload(string Label, double Value);

public sealed class ExperimentalWebsiteRunPayload
{
    public string RunId { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Status { get; init; } = string.Empty;
    public bool IsBaseline { get; init; }
    public double? ParameterValue { get; init; }
    public double? TensileStrength { get; init; }
    public double? TensileStrengthCv { get; init; }
    public double? LayerAdhesionStrength { get; init; }
    public double? LayerAdhesionStrengthCv { get; init; }
    public double? ImpactFlat { get; init; }
    public double? ImpactFlatCv { get; init; }
    public double? ImpactUpright { get; init; }
    public double? ImpactUprightCv { get; init; }
    public double? Stiffness { get; init; }
    public int Rank { get; init; }
    public double? OverallScore { get; init; }
    public IReadOnlyDictionary<string, double?> BaselineIndex { get; init; } = new Dictionary<string, double?>();
}

public sealed class ExperimentalWebsiteVerificationResult
{
    public bool Passed { get; init; }
    public bool IdentityValid { get; init; }
    public bool BaselinesValid { get; init; }
    public bool ValuesFinite { get; init; }
    public bool RankingsAligned { get; init; }
    public bool ChartPayloadComplete { get; init; }
    public int SeriesCount { get; init; }
    public int RunCount { get; init; }
    public IReadOnlyList<string> Errors { get; init; } = Array.Empty<string>();
}
