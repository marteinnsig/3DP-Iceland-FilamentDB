namespace FilamentDbApp.Models;

public sealed class NativeMaterialRecord
{
    public string MaterialID { get; set; } = string.Empty;
    public long? ManufacturerId { get; set; }
    public string Manufacturer { get; set; } = string.Empty;
    public string ProductLine { get; set; } = string.Empty;
    public string MarketingName { get; set; } = string.Empty;
    public long? BaseMaterialId { get; set; }
    public string BaseMaterial { get; set; } = string.Empty;
    public string MaterialCategory { get; set; } = string.Empty;
    public string VariantFinish { get; set; } = string.Empty;
    public string Reinforcement { get; set; } = string.Empty;
    public string Color { get; set; } = string.Empty;
    public string DiameterMm { get; set; } = string.Empty;
    public string SpoolWeightG { get; set; } = string.Empty;
    public string ManufacturerSku { get; set; } = string.Empty;
    public string InventoryId { get; set; } = string.Empty;
    public string PurchaseId { get; set; } = string.Empty;
    public string PurchasedFrom { get; set; } = string.Empty;
    public string SupplierUrl { get; set; } = string.Empty;
    public string PurchaseDate { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public string InventoryStatus { get; set; } = "Unopened";
    public string Quantity { get; set; } = "1";
    public string RemainingWeightG { get; set; } = string.Empty;
    public string PurchasePriceAmount { get; set; } = string.Empty;
    public string PurchaseCurrency { get; set; } = "ISK";
    public string ShippingAmount { get; set; } = string.Empty;
    public string VatAmount { get; set; } = string.Empty;
    public string MsrpAmount { get; set; } = string.Empty;
    public string MsrpCurrency { get; set; } = "ISK";
    public string MsrpUsd { get; set; } = string.Empty;
    public string LandedCostAmount { get; set; } = string.Empty;
    public string LandedCostCurrency { get; set; } = "ISK";
    public string LandedCostUsd { get; set; } = string.Empty;
    public string MsrpUsdPerKg { get; set; } = string.Empty;
    public string LandedCostUsdPerKg { get; set; } = string.Empty;
    public string PriceCheckedDate { get; set; } = string.Empty;
    public string NozzleTemperatureMinC { get; set; } = string.Empty;
    public string NozzleTemperatureRecommendedC { get; set; } = string.Empty;
    public string NozzleTemperatureMaxC { get; set; } = string.Empty;
    public string BedTemperatureMinC { get; set; } = string.Empty;
    public string BedTemperatureRecommendedC { get; set; } = string.Empty;
    public string BedTemperatureMaxC { get; set; } = string.Empty;
    public string PrintSpeedMinMmPerS { get; set; } = string.Empty;
    public string PrintSpeedRecommendedMmPerS { get; set; } = string.Empty;
    public string PrintSpeedMaxMmPerS { get; set; } = string.Empty;
    public string CoolingRequirement { get; set; } = string.Empty;
    public string DryingTimeHours { get; set; } = string.Empty;
    public string EnclosureRequirement { get; set; } = string.Empty;
    public string PrinterProfileReference { get; set; } = string.Empty;
    public string SlicerProfileReference { get; set; } = string.Empty;
    public string PrintingProfileId { get; set; } = string.Empty;
    public string PrintingProfileKind { get; set; } = "Manufacturer baseline";
    public string CoolingMinPercent { get; set; } = string.Empty;
    public string CoolingRecommendedPercent { get; set; } = string.Empty;
    public string CoolingMaxPercent { get; set; } = string.Empty;
    public string DryingTemperatureC { get; set; } = string.Empty;
    public string SlicerIdentity { get; set; } = string.Empty;
    public string SlicerVersion { get; set; } = string.Empty;
    public string PrintingSettingsProvenance { get; set; } = string.Empty;
    public string PrintingSettingsSourceUrl { get; set; } = string.Empty;
    public string PrintingSettingsCheckedDate { get; set; } = string.Empty;
    public string PrintingSettingsValidationNote { get; set; } = string.Empty;
    public string ManufacturerWebsite { get; set; } = string.Empty;
    public string YouTubeReviewUrl { get; set; } = string.Empty;
    public string ThumbnailFilename { get; set; } = string.Empty;
    public string Video { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string TestedStatus { get; set; } = string.Empty;
    public string InTensile { get; set; } = string.Empty;
    public string InImpact { get; set; } = string.Empty;
    public string InStiffness { get; set; } = string.Empty;
    public string SortOrder { get; set; } = string.Empty;
    public string SourcePriority { get; set; } = string.Empty;
    public string WebsiteDisplayName { get; set; } = string.Empty;
    public string MaterialKey { get; set; } = string.Empty;
    public bool PublishPublicReports { get; set; }
    public bool PublishPublicTestDetails { get; set; }
    public bool IsArchived { get; set; }
}
