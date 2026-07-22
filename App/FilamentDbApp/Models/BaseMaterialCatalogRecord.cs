namespace FilamentDbApp.Models;

public sealed class BaseMaterialCatalogRecord
{
    public string BaseMaterial { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string SortOrder { get; set; } = string.Empty;
    public string NozzleTemperatureMinC { get; set; } = string.Empty;
    public string NozzleTemperatureRecommendedC { get; set; } = string.Empty;
    public string NozzleTemperatureMaxC { get; set; } = string.Empty;
    public string BedTemperatureMinC { get; set; } = string.Empty;
    public string BedTemperatureRecommendedC { get; set; } = string.Empty;
    public string BedTemperatureMaxC { get; set; } = string.Empty;
    public string PrintSpeedMinMmPerS { get; set; } = string.Empty;
    public string PrintSpeedRecommendedMmPerS { get; set; } = string.Empty;
    public string PrintSpeedMaxMmPerS { get; set; } = string.Empty;
    public string CoolingMinPercent { get; set; } = string.Empty;
    public string CoolingRecommendedPercent { get; set; } = string.Empty;
    public string CoolingMaxPercent { get; set; } = string.Empty;
    public string CoolingGuidance { get; set; } = string.Empty;
    public string DryingTemperatureC { get; set; } = string.Empty;
    public string DryingTimeHours { get; set; } = string.Empty;
    public string EnclosureRequirement { get; set; } = string.Empty;
    public string PrinterProfileReference { get; set; } = string.Empty;
    public string SlicerProfileReference { get; set; } = string.Empty;
    public string ProfileId { get; set; } = string.Empty;
    public string ProfileKind { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
}
