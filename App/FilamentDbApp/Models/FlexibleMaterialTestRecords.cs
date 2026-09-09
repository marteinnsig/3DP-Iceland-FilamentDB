namespace FilamentDbApp.Models;

public sealed class FlexibleTestSessionRecord
{
    public string FlexibleTestSessionId { get; set; } = string.Empty;
    public string MaterialID { get; set; } = string.Empty;
    public string MaterialDisplayName { get; set; } = string.Empty;
    public string SessionLabel { get; set; } = string.Empty;
    public string MeasuredDate { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string LegacyExperimentalRunId { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
}

public sealed class FlexibleTestSpecimenRecord
{
    public string SpecimenId { get; set; } = string.Empty;
    public string FlexibleTestSessionId { get; set; } = string.Empty;
    public string ExperimentalRunId { get; set; } = string.Empty;
    public string SpecimenLabel { get; set; } = string.Empty;
    public string IntendedTest { get; set; } = "Compression";
    public string Shape { get; set; } = "Cylinder";
    public string DiameterMm { get; set; } = "9";
    public string InitialHeightMm { get; set; } = "9";
    public string ThicknessMm { get; set; } = "9";
    public string MassG { get; set; } = string.Empty;
    public string InfillPercent { get; set; } = "100";
    public string InfillPattern { get; set; } = "Rectilinear";
    public string NozzleDiameterMm { get; set; } = "0.4";
    public string LayerHeightMm { get; set; } = "0.20";
    public string Perimeters { get; set; } = "2";
    public string TopLayers { get; set; } = "3";
    public string BottomLayers { get; set; } = "3";
    public string PrintTemperatureC { get; set; } = string.Empty;
    public string ExtrusionMultiplier { get; set; } = string.Empty;
    public string PrintSettings { get; set; } = string.Empty;
    public string MethodVersion { get; set; } = "TPU-COMP-v1";
    public string MethodNotes { get; set; } = "Comparative in-house test; not ASTM/ISO.";
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
}

public sealed class CompressionPointRecord
{
    public string CompressionPointId { get; set; } = string.Empty;
    public string SpecimenId { get; set; } = string.Empty;
    public int CycleNumber { get; set; } = 1;
    public string TargetStrainPercent { get; set; } = string.Empty;
    public bool TargetReached { get; set; } = true;
    public string DisplacementMm { get; set; } = string.Empty;
    public string ForceN { get; set; } = string.Empty;
    public string HoldTimeSeconds { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
    public string StrainPercent { get; set; } = string.Empty;
    public string ApparentStressMpa { get; set; } = string.Empty;
    public string ResultLabel { get; set; } = string.Empty;
}

public sealed class StressRelaxationPointRecord
{
    public string RelaxationPointId { get; set; } = string.Empty;
    public string SpecimenId { get; set; } = string.Empty;
    public int CycleNumber { get; set; } = 1;
    public string CompressionPercent { get; set; } = "25";
    public string ElapsedTimeSeconds { get; set; } = string.Empty;
    public string ForceN { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
    public string ForceRetentionPercent { get; set; } = string.Empty;
}

public sealed class RecoveryMeasurementRecord
{
    public string RecoveryMeasurementId { get; set; } = string.Empty;
    public string SpecimenId { get; set; } = string.Empty;
    public int CycleNumber { get; set; } = 1;
    public string InitialHeightMm { get; set; } = string.Empty;
    public string HeightAfterRestMm { get; set; } = string.Empty;
    public string RestTimeSeconds { get; set; } = string.Empty;
    public string CompressionPercent { get; set; } = string.Empty;
    public string CompressionHoldSeconds { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
    public string ResidualHeightLossPercent { get; set; } = string.Empty;
}

public sealed class ShoreHardnessReadingRecord
{
    public string ShoreReadingId { get; set; } = string.Empty;
    public string SpecimenId { get; set; } = string.Empty;
    public string ShoreScale { get; set; } = "A";
    public string HardnessValue { get; set; } = string.Empty;
    public string ReadingTimeSeconds { get; set; } = string.Empty;
    public string SpecimenThicknessMm { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
}

public sealed record FlexibleComparisonRow(
    string Metric, string MethodGroup, string Condition, int SpecimenCount,
    string Mean, string StandardDeviation, string Unit, int NotReachedCount);
