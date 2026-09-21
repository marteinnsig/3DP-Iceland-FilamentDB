using System.ComponentModel;

namespace FilamentDbApp.Models;

public abstract class PendulumImpactRecord : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;
    public void Refresh() => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(string.Empty));
}

public sealed class PendulumImpactRunRecord : PendulumImpactRecord
{
    public string RunId { get; set; } = "";
    public string MaterialID { get; set; } = "";
    public string Method { get; set; } = "Izod";
    // Missing in historical JSON: energy readings must never become direct strength readings.
    public string InputMode { get; set; } = "Energy";
    public string Label { get; set; } = "";
    public string Date { get; set; } = "";
    public string TargetCount { get; set; } = "10";
    public string Equipment { get; set; } = "Hongtuo HT-5D-CM";
    public string StandardReference { get; set; } = "";
    public string StandardEdition { get; set; } = "";
    public string Conformity { get; set; } = "Unverified comparative measurement";
    public string SpecimenType { get; set; } = "";
    public string NotchType { get; set; } = "Unconfirmed";
    public string NotchPreparation { get; set; } = "";
    public string NominalLengthMm { get; set; } = "";
    public string NominalWidthMm { get; set; } = "";
    public string NominalThicknessMm { get; set; } = "";
    public string NominalLigamentMm { get; set; } = "";
    public string Orientation { get; set; } = "Printed on long side; bed-facing surface and layer/notch directions unconfirmed";
    public string DiagramPath { get; set; } = "";
    public string DiagramBase64 { get; set; } = "";
    public string Printer { get; set; } = "";
    public string NozzleMm { get; set; } = "";
    public string LayerHeightMm { get; set; } = "";
    public string Walls { get; set; } = "";
    public string TopLayers { get; set; } = "";
    public string BottomLayers { get; set; } = "";
    public string InfillPercent { get; set; } = "100";
    public string InfillPattern { get; set; } = "";
    public string PrintTemperatureC { get; set; } = "";
    public string BedTemperatureC { get; set; } = "";
    public string Drying { get; set; } = "";
    public string Conditioning { get; set; } = "";
    public string TestTemperatureC { get; set; } = "";
    public string HumidityPercent { get; set; } = "";
    public string Setup { get; set; } = "";
    public string Notes { get; set; } = "";
    public string CreatedAtUtc { get; set; } = "";
    public string UpdatedAtUtc { get; set; } = "";
}

public sealed class PendulumImpactSpecimenRecord : PendulumImpactRecord
{
    public string SpecimenId { get; set; } = "";
    public string RunId { get; set; } = "";
    public string Label { get; set; } = "";
    public string PrintBatch { get; set; } = "";
    public string EnergyJ { get; set; } = "";
    public string StrengthKjM2Raw { get; set; } = "";
    public string HammerJ { get; set; } = "";
    public string LengthMm { get; set; } = "";
    public string WidthMm { get; set; } = "";
    public string ThicknessMm { get; set; } = "";
    public string RemainingLigamentMm { get; set; } = "";
    public string NotchDepthMm { get; set; } = "";
    public string BreakType { get; set; } = "Unmeasured";
    public string Status { get; set; } = "Valid";
    public string ExclusionReason { get; set; } = "";
    public string SettingsOverrides { get; set; } = "";
    public string Notes { get; set; } = "";
    public string CreatedAtUtc { get; set; } = "";
    public string UpdatedAtUtc { get; set; } = "";
}

public sealed record PendulumImpactResult(double? StrengthKjM2, string Explanation);
public sealed record PendulumImpactSummary(int MeasuredCount, int TargetCount, int ValidCount,
    int NoBreakCount, int InvalidCount, int ExcludedCount, double? Mean, double? SampleStdDev,
    double? CvPercent, double? Minimum, double? Maximum)
{
    public double? CoefficientOfVariation => CvPercent / 100d;
    public int? Confidence => new Services.Calculations.StatisticsService().ConfidenceFromSampleCount(ValidCount);
}
public sealed record PendulumImpactGroupSummary(string Condition, string PrintBatch, PendulumImpactSummary Summary);
