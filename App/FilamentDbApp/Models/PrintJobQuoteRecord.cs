namespace FilamentDbApp.Models;

public sealed class PrintJobQuoteRecord
{
    public string QuoteId { get; set; } = string.Empty;
    public string QuoteNumber { get; set; } = string.Empty;
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string PreparedBy { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialLabelSnapshot { get; set; } = string.Empty;
    public string MaterialCostProvenance { get; set; } = string.Empty;
    public string PrinterId { get; set; } = string.Empty;
    public string PrinterLabelSnapshot { get; set; } = string.Empty;
    public string QuoteCurrency { get; set; } = "ISK";
    public string FinalPriceQuoteCurrency { get; set; } = string.Empty;
    public string FinalPriceIsk { get; set; } = string.Empty;
    public string CalculationVersion { get; set; } = "v1";
    public string SnapshotJson { get; set; } = string.Empty;
}

public sealed record PrintJobQuoteInput(
    decimal GramsPerPart,
    decimal Quantity,
    decimal MaterialEfficiencyFactor,
    decimal MaterialCostPerKg,
    decimal IskPerMaterialCurrencyUnit,
    decimal PrintHours,
    decimal PrinterCostIskPerHour,
    decimal PrintPostProcessingMinutes,
    decimal CustomerConsultingMinutes,
    decimal PartsDesignMinutes,
    decimal LaborRateIskPerHour,
    decimal AdditionalCostIsk,
    decimal TargetMarginPercent,
    decimal IskPerQuoteCurrencyUnit);

public sealed record PrintJobQuoteCalculation(
    bool IsValid,
    IReadOnlyList<string> Errors,
    decimal? RequiredGrams,
    decimal? MaterialCostIsk,
    decimal? PrinterCostIsk,
    decimal? PrintPostProcessingLaborCostIsk,
    decimal? CustomerConsultingCostIsk,
    decimal? PartsDesignCostIsk,
    decimal? TotalLaborCostIsk,
    decimal? LandedCostIsk,
    decimal? FinalPriceIsk,
    decimal? FinalPriceQuoteCurrency);
