namespace FilamentDbApp.Models;

public sealed class PrinterProfileRecord
{
    public string PrinterId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Manufacturer { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public string CostCurrency { get; set; } = "ISK";
    public string PurchaseCostAmount { get; set; } = string.Empty;
    public string AdditionalUpfrontCostAmount { get; set; } = string.Empty;
    public string AnnualMaintenanceAmount { get; set; } = string.Empty;
    public string EstimatedLifeYears { get; set; } = "2";
    public string UptimePercent { get; set; } = "50";
    public string AveragePowerWatts { get; set; } = "150";
    public string BufferOverride { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Notes { get; set; } = string.Empty;
    public string Provenance { get; set; } = "Manual";
    public string UpdatedAtUtc { get; set; } = string.Empty;
    public string DisplayLabel => $"{Name} ({PrinterId})".Trim();
}

public sealed record PrinterRateInput(
    decimal PurchaseCost,
    decimal AdditionalUpfrontCost,
    decimal AnnualMaintenance,
    decimal EstimatedLifeYears,
    decimal UptimePercent,
    decimal AveragePowerWatts,
    decimal IskPerCostCurrencyUnit,
    decimal ElectricityIskPerKwh,
    decimal BufferFactor);

public sealed record PrinterRateResult(
    bool IsValid,
    IReadOnlyList<string> Errors,
    decimal? CapitalCostIskPerHour,
    decimal? ElectricityCostIskPerHour,
    decimal? TotalPrinterCostIskPerHour,
    decimal? ProductiveLifetimeHours);
