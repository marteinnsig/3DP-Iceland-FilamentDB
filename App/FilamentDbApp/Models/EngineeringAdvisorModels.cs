namespace FilamentDbApp.Models;

public sealed class EngineeringAdvisorCandidate
{
    public string MaterialId { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string RecommendationType { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public string BaseMaterial { get; init; } = string.Empty;
    public double RecommendationScore { get; init; }
    public double? PricePerKg { get; init; }
    public EngineeringScoreProfile Profile { get; init; } = new();
}

public sealed class EngineeringAdvisorInsight
{
    public string ConfidenceLabel { get; init; } = string.Empty;
    public string ConfidenceSummary { get; init; } = string.Empty;
    public string EvidenceSummary { get; init; } = string.Empty;
    public string TradeOffSummary { get; init; } = string.Empty;
    public string ComparisonSummary { get; init; } = string.Empty;
    public double? ComparisonScoreDelta { get; init; }
    public string ClearestLeadAxis { get; init; } = string.Empty;
    public double? ClearestLeadDelta { get; init; }
    public string ClearestTradeOffAxis { get; init; } = string.Empty;
    public double? ClearestTradeOffDelta { get; init; }
    public int CoveredAxes { get; init; }
    public int TotalAxes { get; init; }
}

public sealed class EngineeringConsistencyInsight
{
    public string StatusLabel { get; init; } = string.Empty;
    public string RepeatabilitySummary { get; init; } = string.Empty;
    public string OutlierReviewSummary { get; init; } = string.Empty;
    public double? AverageCvPercent { get; init; }
    public double? HighestCvPercent { get; init; }
    public string HighestVariationSet { get; init; } = string.Empty;
    public int MeasurementSetCount { get; init; }
    public int CvSetCount { get; init; }
    public int AdequateSampleSetCount { get; init; }
    public int ReviewFlagCount { get; init; }
    public double? ConsistencyScore { get; init; }
    public bool UsesVerifiedMaterialSummary { get; init; }
    public bool DirectSpecimenOutlierDetectionAvailable { get; init; }
}

public sealed class EngineeringContextInsight
{
    public string PriceSummary { get; init; } = string.Empty;
    public string InventoryStatus { get; init; } = string.Empty;
    public string InventorySummary { get; init; } = string.Empty;
    public string ManufacturerSummary { get; init; } = string.Empty;
    public bool UsesCanonicalPricing { get; init; }
    public bool UsesInventoryEngineResults { get; init; }
    public bool UsesManufacturerRecords { get; init; }
}

public sealed class EngineeringPeerCandidate
{
    public string MaterialId { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public string Category { get; init; } = string.Empty;
    public double? OverallScore { get; init; }
}

public sealed class EngineeringPeerInsight
{
    public string ManufacturerPositionSummary { get; init; } = string.Empty;
    public string CategoryPositionSummary { get; init; } = string.Empty;
    public int? ManufacturerRank { get; init; }
    public int ManufacturerPeerCount { get; init; }
    public int? CategoryRank { get; init; }
    public int CategoryPeerCount { get; init; }
    public int ActiveDatasetMaterialCount { get; init; }
    public bool UsesExistingScoreProfiles { get; init; }
}

public sealed class EngineeringAlternativeInsight
{
    public string Kind { get; init; } = string.Empty;
    public string Label { get; init; } = string.Empty;
    public double RecommendationScore { get; init; }
    public string ScoreText => $"{RecommendationScore:0}/100";
    public double? ScoreDelta { get; init; }
    public double? PricePerKg { get; init; }
    public string PriceText => PricePerKg.HasValue ? $"${PricePerKg:0.00}/kg" : "n/a";
    public double? PriceDeltaPercent { get; init; }
    public string Summary { get; init; } = string.Empty;
    public string GainSummary { get; init; } = string.Empty;
    public string TradeOffSummary { get; init; } = string.Empty;
}

public sealed class EngineeringIntelligenceHandoff
{
    public string ReportSummary { get; init; } = string.Empty;
    public string VideoPlannerSummary { get; init; } = string.Empty;
    public string SourceStatement { get; init; } = string.Empty;
    public bool UsesExistingEngineeringInsights { get; init; }
    public bool RecalculatesMeasurementsOrScores { get; init; }
}
