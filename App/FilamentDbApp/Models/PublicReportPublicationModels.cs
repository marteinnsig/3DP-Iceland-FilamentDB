namespace FilamentDbApp.Models;

public sealed class PublicMaterialEngineeringReportModel
{
    public string MaterialId { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public string ProductLine { get; init; } = string.Empty;
    public string BaseMaterial { get; init; } = string.Empty;
    public string MaterialCategory { get; init; } = string.Empty;
    public string VariantFinish { get; init; } = string.Empty;
    public string Reinforcement { get; init; } = string.Empty;
    public string Color { get; init; } = string.Empty;
    public string TestCoverage { get; init; } = string.Empty;
    public string OverallScore { get; init; } = "n/a";
    public string TensileScore { get; init; } = "n/a";
    public string ImpactScore { get; init; } = "n/a";
    public string StiffnessScore { get; init; } = "n/a";
    public string ConsistencyScore { get; init; } = "n/a";
    public string LayerAdhesionScore { get; init; } = "n/a";
    public string BestAxis { get; init; } = string.Empty;
    public string MsrpUsdPerKg { get; init; } = string.Empty;
    public string ManufacturerWebsite { get; init; } = string.Empty;
    public string VideoReviewUrl { get; init; } = string.Empty;
    public int VerifiedEngineeringAxes { get; init; }
    public string EngineeringSummary { get; init; } = string.Empty;
    public string ExecutiveReview { get; init; } = string.Empty;
    public string BestFeature { get; init; } = string.Empty;
    public string WeakestFeature { get; init; } = string.Empty;
    public string OverallRank { get; init; } = string.Empty;
    public string OverallPercentile { get; init; } = string.Empty;
    public IReadOnlyList<string> RecommendedApplications { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Strengths { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Limitations { get; init; } = Array.Empty<string>();
    public IReadOnlyList<string> Tradeoffs { get; init; } = Array.Empty<string>();
    public IReadOnlyList<PublicMaterialPeerModel> PeerContext { get; init; } = Array.Empty<PublicMaterialPeerModel>();
    public PublicEngineeringScoreProfile MaterialAverage { get; init; } = new();
    public PublicEngineeringScoreProfile ManufacturerAverage { get; init; } = new();
    public PublicVerifiedMeasurementsModel VerifiedMeasurements { get; init; } = new();
    public IReadOnlyList<PublicMetricPositionModel> MetricPositions { get; init; } = Array.Empty<PublicMetricPositionModel>();
    public IReadOnlyList<string> DecisionGuidance { get; init; } = Array.Empty<string>();
    public IReadOnlyList<PublicAlternativeModel> BetterAlternatives { get; init; } = Array.Empty<PublicAlternativeModel>();
}

public sealed class PublicVerifiedMeasurementsModel
{
    public PublicMeasurementSetModel TensileUpright { get; init; } = new();
    public PublicMeasurementSetModel TensileFlat { get; init; } = new();
    public PublicMeasurementSetModel ImpactUpright { get; init; } = new();
    public PublicMeasurementSetModel ImpactFlat { get; init; } = new();
    public double? StiffnessModulusMpa { get; init; }
    public double? StiffnessDeflectionMm { get; init; }
}

public sealed class PublicMeasurementSetModel
{
    public double? Average { get; init; }
    public double? StandardDeviation { get; init; }
    public double? CoefficientOfVariation { get; init; }
    public int SampleCount { get; init; }
    public int? Confidence { get; init; }
}

public sealed class PublicMetricPositionModel
{
    public string Metric { get; init; } = string.Empty;
    public string Score { get; init; } = "n/a";
    public string Rank { get; init; } = string.Empty;
    public string Percentile { get; init; } = string.Empty;
}

public sealed class PublicAlternativeModel
{
    public string MaterialId { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public string OverallScore { get; init; } = "n/a";
    public string TensileScore { get; init; } = "n/a";
    public string ImpactScore { get; init; } = "n/a";
}

public sealed class PublicEngineeringScoreProfile
{
    public string Label { get; init; } = string.Empty;
    public string OverallScore { get; init; } = "n/a";
    public string TensileScore { get; init; } = "n/a";
    public string ImpactScore { get; init; } = "n/a";
    public string StiffnessScore { get; init; } = "n/a";
    public string ConsistencyScore { get; init; } = "n/a";
    public string LayerAdhesionScore { get; init; } = "n/a";
}

public sealed class PublicMaterialPeerModel
{
    public string MaterialId { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public string OverallScore { get; init; } = "n/a";
    public string TensileScore { get; init; } = "n/a";
    public string ImpactScore { get; init; } = "n/a";
}

public sealed class PublicReportPublicationResult
{
    public string RelativeDirectory { get; init; } = string.Empty;
    public string Html { get; init; } = string.Empty;
    public string Manifest { get; init; } = string.Empty;
    public string MetadataJson { get; init; } = string.Empty;
    public string PreviewIndexHtml { get; init; } = string.Empty;
}

public sealed class PublicReportPublicationVerificationResult
{
    public bool Passed { get; init; }
    public bool MaterialIdPathPassed { get; init; }
    public bool PublicFieldAllowlistPassed { get; init; }
    public bool SensitiveFieldExclusionPassed { get; init; }
    public bool StableArtifactLinksPassed { get; init; }
    public bool MethodologyLinksPassed { get; init; }
    public bool RichContentPassed { get; init; }
    public bool RadarLayoutPassed { get; init; }
    public bool BrandingPassed { get; init; }
    public string Detail { get; init; } = string.Empty;
}

public sealed class PublicComparisonReportModel
{
    public string PresetSlug { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string BaseMaterial { get; init; } = string.Empty;
    public string ScopeDescription { get; init; } = string.Empty;
    public IReadOnlyList<PublicComparisonMaterialModel> Materials { get; init; } = Array.Empty<PublicComparisonMaterialModel>();
}

public sealed class PublicComparisonMaterialModel
{
    public string MaterialId { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public string ProductLine { get; init; } = string.Empty;
    public string BaseMaterial { get; init; } = string.Empty;
    public string Reinforcement { get; init; } = string.Empty;
    public string TestCoverage { get; init; } = string.Empty;
    public int VerifiedEngineeringAxes { get; init; }
    public string OverallScore { get; init; } = "n/a";
    public string TensileScore { get; init; } = "n/a";
    public string ImpactScore { get; init; } = "n/a";
    public string StiffnessScore { get; init; } = "n/a";
    public string ConsistencyScore { get; init; } = "n/a";
    public string LayerAdhesionScore { get; init; } = "n/a";
    public string MsrpUsdPerKg { get; init; } = string.Empty;
}

public sealed class PublicComparisonPublicationResult
{
    public string RelativeDirectory { get; init; } = string.Empty;
    public string Html { get; init; } = string.Empty;
    public string Manifest { get; init; } = string.Empty;
    public string MetadataJson { get; init; } = string.Empty;
}

public sealed class PublicComparisonVerificationResult
{
    public bool Passed { get; init; }
    public string Detail { get; init; } = string.Empty;
}

public sealed class PublicManufacturerReportModel
{
    public string ManufacturerSlug { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public string ManufacturerWebsite { get; init; } = string.Empty;
    public int ProductLines { get; init; }
    public int MaterialTypes { get; init; }
    public int MaterialsWithResults { get; init; }
    public int CompleteProfiles { get; init; }
    public int MaterialsWithMsrp { get; init; }
    public int MaterialsWithVideo { get; init; }
    public int PublicBenchmarkManufacturers { get; init; }
    public string GlobalManufacturerRank { get; init; } = "n/a";
    public string AverageOverallScore { get; init; } = "n/a";
    public string PortfolioLeader { get; init; } = string.Empty;
    public string StrongestAxis { get; init; } = string.Empty;
    public IReadOnlyList<PublicManufacturerCategoryPositionModel> CategoryPositions { get; init; } = Array.Empty<PublicManufacturerCategoryPositionModel>();
    public IReadOnlyList<PublicManufacturerMaterialModel> Materials { get; init; } = Array.Empty<PublicManufacturerMaterialModel>();
}

public sealed class PublicManufacturerCategoryPositionModel
{
    public string BaseMaterial { get; init; } = string.Empty;
    public string Position { get; init; } = "n/a";
    public string AverageOverallScore { get; init; } = "n/a";
    public int ScoredProducts { get; init; }
}

public sealed class PublicManufacturerMaterialModel
{
    public string MaterialId { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string ProductLine { get; init; } = string.Empty;
    public string BaseMaterial { get; init; } = string.Empty;
    public string Reinforcement { get; init; } = string.Empty;
    public string TestCoverage { get; init; } = string.Empty;
    public int EngineeringAxes { get; init; }
    public string OverallScore { get; init; } = "n/a";
    public string TensileScore { get; init; } = "n/a";
    public string ImpactScore { get; init; } = "n/a";
    public string StiffnessScore { get; init; } = "n/a";
    public string ConsistencyScore { get; init; } = "n/a";
    public string LayerAdhesionScore { get; init; } = "n/a";
    public string StrongestAxis { get; init; } = string.Empty;
    public string MsrpUsdPerKg { get; init; } = string.Empty;
    public string ProductUrl { get; init; } = string.Empty;
    public string VideoReviewUrl { get; init; } = string.Empty;
}

public sealed class PublicManufacturerPublicationResult
{
    public string RelativeDirectory { get; init; } = string.Empty;
    public string Html { get; init; } = string.Empty;
    public string Manifest { get; init; } = string.Empty;
    public string MetadataJson { get; init; } = string.Empty;
}

public sealed class PublicTestSessionReportModel
{
    public string MaterialId { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public string SummaryStatus { get; init; } = string.Empty;
    public int ResultModules { get; init; }
    public int SpecimenResultRecords { get; init; }
    public bool PublicDetailsApproved { get; init; }
    public PublicVerifiedMeasurementsModel VerifiedMeasurements { get; init; } = new();
    public IReadOnlyList<PublicTestModuleQualityModel> QualityRows { get; init; } = Array.Empty<PublicTestModuleQualityModel>();
    public IReadOnlyList<PublicTestRawInputModel> RawInputs { get; init; } = Array.Empty<PublicTestRawInputModel>();
    public IReadOnlyList<PublicTestNoteModel> ApprovedNotes { get; init; } = Array.Empty<PublicTestNoteModel>();
}
public sealed class PublicTestModuleQualityModel { public string Module { get; init; } = string.Empty; public string Orientation { get; init; } = string.Empty; public string Average { get; init; } = "n/a"; public string StandardDeviation { get; init; } = "n/a"; public string CoefficientOfVariation { get; init; } = "n/a"; public int Samples { get; init; } public string Confidence { get; init; } = "n/a"; public string Validation { get; init; } = string.Empty; }
public sealed class PublicTestRawInputModel { public string Module { get; init; } = string.Empty; public string InputSet { get; init; } = string.Empty; public string RecordedValues { get; init; } = "n/a"; }
public sealed class PublicTestNoteModel { public string Module { get; init; } = string.Empty; public string Note { get; init; } = string.Empty; }
public sealed class PublicTestSessionPublicationResult { public string RelativeDirectory { get; init; } = string.Empty; public string Html { get; init; } = string.Empty; public string Manifest { get; init; } = string.Empty; public string MetadataJson { get; init; } = string.Empty; }
public sealed class PublicPrintingRecommendationReportModel
{
    public string MaterialId { get; init; } = string.Empty; public string MaterialName { get; init; } = string.Empty; public string Manufacturer { get; init; } = string.Empty; public string BaseMaterial { get; init; } = string.Empty; public string TestCoverage { get; init; } = string.Empty; public int EngineeringAxes { get; init; }
    public string OverallScore { get; init; } = "n/a"; public string TensileScore { get; init; } = "n/a"; public string ImpactScore { get; init; } = "n/a"; public string StiffnessScore { get; init; } = "n/a"; public string ConsistencyScore { get; init; } = "n/a"; public string LayerAdhesionScore { get; init; } = "n/a"; public string OverallRank { get; init; } = string.Empty; public string MsrpUsdPerKg { get; init; } = string.Empty;
    public IReadOnlyList<string> RecommendedApplications { get; init; } = Array.Empty<string>(); public IReadOnlyList<string> Strengths { get; init; } = Array.Empty<string>(); public IReadOnlyList<string> Limitations { get; init; } = Array.Empty<string>(); public IReadOnlyList<string> Tradeoffs { get; init; } = Array.Empty<string>(); public IReadOnlyList<string> WorkflowChecks { get; init; } = Array.Empty<string>(); public IReadOnlyList<string> DecisionGuidance { get; init; } = Array.Empty<string>(); public IReadOnlyList<PublicAlternativeModel> Alternatives { get; init; } = Array.Empty<PublicAlternativeModel>(); public string ManufacturerWebsite { get; init; } = string.Empty;
}
public sealed class PublicPrintingRecommendationPublicationResult { public string RelativeDirectory { get; init; } = string.Empty; public string Html { get; init; } = string.Empty; public string Manifest { get; init; } = string.Empty; public string MetadataJson { get; init; } = string.Empty; }

public sealed class PublicMaterialSummaryReportModel
{
    public int PublicMaterials { get; init; }
    public int FullyTested { get; init; }
    public int PartiallyTested { get; init; }
    public int NoTestResults { get; init; }
    public int Manufacturers { get; init; }
    public int MaterialTypes { get; init; }
    public int MaterialsWithResults { get; init; }
    public IReadOnlyList<PublicSummaryCoverageModel> Coverage { get; init; } = Array.Empty<PublicSummaryCoverageModel>();
    public IReadOnlyList<PublicSummaryDistributionModel> MaterialTypeDistribution { get; init; } = Array.Empty<PublicSummaryDistributionModel>();
    public IReadOnlyList<PublicSummaryDistributionModel> ManufacturerDistribution { get; init; } = Array.Empty<PublicSummaryDistributionModel>();
    public IReadOnlyList<PublicMaterialSummaryRowModel> Materials { get; init; } = Array.Empty<PublicMaterialSummaryRowModel>();
}

public sealed class PublicSummaryCoverageModel
{
    public string Module { get; init; } = string.Empty;
    public int Materials { get; init; }
    public string Coverage { get; init; } = "0%";
}

public sealed class PublicSummaryDistributionModel
{
    public string Label { get; init; } = string.Empty;
    public int Materials { get; init; }
}

public sealed class PublicMaterialSummaryRowModel
{
    public string MaterialId { get; init; } = string.Empty;
    public string MaterialName { get; init; } = string.Empty;
    public string Manufacturer { get; init; } = string.Empty;
    public string BaseMaterial { get; init; } = string.Empty;
    public string TestCoverage { get; init; } = string.Empty;
    public int EngineeringAxes { get; init; }
    public string OverallScore { get; init; } = "n/a";
    public string TensileScore { get; init; } = "n/a";
    public string ImpactScore { get; init; } = "n/a";
    public string StiffnessScore { get; init; } = "n/a";
    public string ConsistencyScore { get; init; } = "n/a";
    public string LayerAdhesionScore { get; init; } = "n/a";
}

public sealed class PublicMaterialSummaryPublicationResult
{
    public string RelativeDirectory { get; init; } = string.Empty;
    public string Html { get; init; } = string.Empty;
    public string Manifest { get; init; } = string.Empty;
    public string MetadataJson { get; init; } = string.Empty;
}

public sealed class PublicEngineeringReportPackageModel
{
    public int PublicMaterials { get; init; }
    public IReadOnlyList<PublicReportCatalogEntryModel> Reports { get; init; } = Array.Empty<PublicReportCatalogEntryModel>();
}

public sealed class PublicReportCatalogEntryModel
{
    public string ReportType { get; init; } = string.Empty;
    public string Title { get; init; } = string.Empty;
    public string ScopeType { get; init; } = string.Empty;
    public string ScopeId { get; init; } = string.Empty;
    public string Html { get; init; } = string.Empty;
    public string Pdf { get; init; } = string.Empty;
    public string Metadata { get; init; } = string.Empty;
}

public sealed class PublicEngineeringReportPackageResult
{
    public string IndexHtml { get; init; } = string.Empty;
    public string Manifest { get; init; } = string.Empty;
    public string CatalogJson { get; init; } = string.Empty;
}
