using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

public sealed class PrinterRateService
{
    public PrinterRateResult Calculate(PrinterRateInput input)
    {
        var errors = new List<string>();
        RequireNonNegative(input.PurchaseCost, "Purchase cost", errors);
        RequireNonNegative(input.AdditionalUpfrontCost, "Additional upfront cost", errors);
        RequireNonNegative(input.AnnualMaintenance, "Annual maintenance", errors);
        RequirePositive(input.EstimatedLifeYears, "Estimated life", errors);
        if (input.UptimePercent <= 0 || input.UptimePercent > 100)
            errors.Add("Uptime percent must be greater than 0 and at most 100.");
        RequireNonNegative(input.AveragePowerWatts, "Average power", errors);
        RequirePositive(input.IskPerCostCurrencyUnit, "ISK conversion rate", errors);
        RequireNonNegative(input.ElectricityIskPerKwh, "Electricity cost", errors);
        RequirePositive(input.BufferFactor, "Buffer factor", errors);
        if (errors.Count > 0)
            return new PrinterRateResult(false, errors, null, null, null, null);

        var productiveHours =
            8760m * (input.UptimePercent / 100m) * input.EstimatedLifeYears;
        var lifetimeCostSourceCurrency =
            input.PurchaseCost +
            input.AdditionalUpfrontCost +
            input.AnnualMaintenance * input.EstimatedLifeYears;
        var capitalPerHour =
            lifetimeCostSourceCurrency *
            input.IskPerCostCurrencyUnit /
            productiveHours;
        var electricityPerHour =
            input.AveragePowerWatts / 1000m * input.ElectricityIskPerKwh;
        var totalPerHour =
            (capitalPerHour + electricityPerHour) * input.BufferFactor;

        return new PrinterRateResult(
            true,
            Array.Empty<string>(),
            capitalPerHour,
            electricityPerHour,
            totalPerHour,
            productiveHours);
    }

    private static void RequireNonNegative(
        decimal value,
        string label,
        ICollection<string> errors)
    {
        if (value < 0) errors.Add(label + " cannot be negative.");
    }

    private static void RequirePositive(
        decimal value,
        string label,
        ICollection<string> errors)
    {
        if (value <= 0) errors.Add(label + " must be greater than zero.");
    }
}
