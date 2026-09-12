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
    public string InitialHeightMm { get; set; } = "10";
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

public sealed class CompressionPointRecord : System.ComponentModel.INotifyPropertyChanged
{
    private string _forceRetentionPercent = string.Empty;
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    public string ForceRetentionPercent
    {
        get => _forceRetentionPercent;
        set
        {
            if (_forceRetentionPercent == value) return;
            _forceRetentionPercent = value;
            PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(nameof(ForceRetentionPercent)));
        }
    }

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

public sealed class RecoveryMeasurementRecord : System.ComponentModel.INotifyPropertyChanged
{
    private string _initialHeightMm = string.Empty;
    private string _heightAfterRestMm = string.Empty;
    private string _residualHeightLossPercent = string.Empty;
    private string? _tvlContactOffsetInput;
    private bool _preservePendingTvlOffset;
    public event System.ComponentModel.PropertyChangedEventHandler? PropertyChanged;
    private void Notify(string property) =>
        PropertyChanged?.Invoke(this, new System.ComponentModel.PropertyChangedEventArgs(property));

    public string RecoveryMeasurementId { get; set; } = string.Empty;
    public string SpecimenId { get; set; } = string.Empty;
    public int CycleNumber { get; set; } = 1;
    public string InitialHeightMm
    {
        get => _initialHeightMm;
        set
        {
            // Initial-height corrections preserve the canonical recovered height. Keep invalid pending input for correction.
            var pendingInvalid = _preservePendingTvlOffset;
            _initialHeightMm = value;
            if (!pendingInvalid) _tvlContactOffsetInput = null;
            else ApplyTvlInput();
            Notify(nameof(InitialHeightMm));
            Notify(nameof(TvlContactOffsetMm));
        }
    }
    public string HeightAfterRestMm
    {
        get => _heightAfterRestMm;
        set
        {
            _heightAfterRestMm = value;
            _tvlContactOffsetInput = null; // Explicit direct-height entry supersedes the alternate TVL input.
            _preservePendingTvlOffset = false;
            Notify(nameof(HeightAfterRestMm));
            Notify(nameof(TvlContactOffsetMm));
        }
    }
    // Alternate input representation only. SQLite's two saved heights remain canonical, including historical records.
    public string TvlContactOffsetMm
    {
        get => _tvlContactOffsetInput ??
            (TryDecimal(_initialHeightMm, out var initial) && initial > 0m &&
             TryDecimal(_heightAfterRestMm, out var after) && after >= 0m
                ? (initial - after).ToString("G29", System.Globalization.CultureInfo.CurrentCulture) : string.Empty);
        set
        {
            _tvlContactOffsetInput = value;
            _preservePendingTvlOffset = TvlContactOffsetError.Length != 0;
            ApplyTvlInput();
            Notify(nameof(TvlContactOffsetMm));
        }
    }
    public string TvlContactOffsetError
    {
        get
        {
            if (string.IsNullOrWhiteSpace(_tvlContactOffsetInput)) return string.Empty;
            if (!TryDecimal(_tvlContactOffsetInput, out var offset)) return "Enter a numeric TVL contact offset in mm.";
            if (!TryDecimal(_initialHeightMm, out var initial) || initial <= 0m)
                return "Enter a positive Recovery initial height before the TVL contact offset.";
            try
            {
                if (initial - offset < 0m) return "TVL contact offset cannot exceed the initial height.";
                return string.Empty;
            }
            catch (OverflowException) { return "TVL contact offset is outside the supported numeric range."; }
        }
    }
    // Release pending offset ownership only after the complete valid cell edit has been persisted.
    public void AcceptInputCommit()
    {
        if (TvlContactOffsetError.Length == 0) _preservePendingTvlOffset = false;
    }
    private void ApplyTvlInput()
    {
        if (_tvlContactOffsetInput is null || TvlContactOffsetError.Length != 0) return;
        if (string.IsNullOrWhiteSpace(_tvlContactOffsetInput)) _heightAfterRestMm = string.Empty;
        else if (TryDecimal(_initialHeightMm, out var initial) && TryDecimal(_tvlContactOffsetInput, out var offset))
            _heightAfterRestMm = (initial - offset).ToString("G29", System.Globalization.CultureInfo.CurrentCulture);
        Notify(nameof(HeightAfterRestMm));
    }
    private static bool TryDecimal(string value, out decimal parsed) => decimal.TryParse(
        value.Trim().Replace(',', '.'), System.Globalization.NumberStyles.Float,
        System.Globalization.CultureInfo.InvariantCulture, out parsed);

    public string RestTimeSeconds { get; set; } = string.Empty;
    public string CompressionPercent { get; set; } = string.Empty;
    public string CompressionHoldSeconds { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
    public string ResidualHeightLossPercent
    {
        get => _residualHeightLossPercent;
        set { _residualHeightLossPercent = value; Notify(nameof(ResidualHeightLossPercent)); }
    }
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
    string Mean, string StandardDeviation, string CoefficientOfVariation,
    string Minimum, string Maximum, string Unit, int NotReachedCount);

public sealed record TpuCompressionPublicSummary(
    int SpecimenCount30Seconds, double Mean30SecondsN, double SampleStandardDeviation30SecondsN, double CoefficientOfVariation30Seconds,
    int SpecimenCount10Seconds, double Mean10SecondsN, double SampleStandardDeviation10SecondsN, double CoefficientOfVariation10Seconds);

public enum FlexibleMetricKind
{
    CompressionForce, ApparentCompressiveStress, ForceRetention, ForceReduction, ResidualHeightLoss, ShoreA, ShoreD
}

// Internal evidence only: MethodGroup and ComparisonKey contain saved freeform method metadata.
// Public exports must use an explicit safe projection, never serialize this record directly.
public sealed record FlexibleMetricGroupSummary(
    FlexibleMetricKind MetricKind, string Metric, string MethodGroup, string Condition, string ComparisonKey,
    int SpecimenCount, double? Mean, double? StandardDeviation, double? CoefficientOfVariation,
    double? Minimum, double? Maximum, string Unit, int NotReachedCount);

public sealed record FlexibleMaterialEvidenceSnapshot(
    string MaterialId, IReadOnlyList<FlexibleMetricGroupSummary> Groups, int SessionCount,
    int SpecimenCount, int LegacyRelaxationPointCount)
{
    public bool HasResults => Groups.Any(group => group.SpecimenCount > 0);
}
