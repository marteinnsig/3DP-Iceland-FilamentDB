using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

public sealed class PrintJobQuoteService
{
    public PrintJobQuoteCalculation Calculate(PrintJobQuoteInput input)
    {
        var errors = new List<string>();
        Positive(input.GramsPerPart, "Grams per part", errors);
        Positive(input.Quantity, "Quantity", errors);
        Positive(input.MaterialEfficiencyFactor, "Material efficiency factor", errors);
        NonNegative(input.MaterialCostPerKg, "Material cost per kg", errors);
        Positive(input.IskPerMaterialCurrencyUnit, "Material currency rate", errors);
        NonNegative(input.PrintHours, "Print hours", errors);
        NonNegative(input.PrinterCostIskPerHour, "Printer hourly cost", errors);
        NonNegative(input.PrintPostProcessingMinutes, "Print/post-processing minutes", errors);
        NonNegative(input.CustomerConsultingMinutes, "Customer consulting minutes", errors);
        NonNegative(input.PartsDesignMinutes, "Parts design minutes", errors);
        NonNegative(input.LaborRateIskPerHour, "Labor hourly rate", errors);
        NonNegative(input.AdditionalCostIsk, "Additional cost", errors);
        if (input.TargetMarginPercent < 0 || input.TargetMarginPercent >= 100)
            errors.Add("Target margin percent must be at least 0 and below 100.");
        Positive(input.IskPerQuoteCurrencyUnit, "Quote currency rate", errors);
        if (errors.Count > 0)
            return new(false, errors, null, null, null, null, null, null, null,
                null, null, null);

        var requiredGrams =
            input.GramsPerPart * input.Quantity * input.MaterialEfficiencyFactor;
        var materialCostIsk =
            requiredGrams / 1000m *
            input.MaterialCostPerKg *
            input.IskPerMaterialCurrencyUnit;
        var printerCostIsk = input.PrintHours * input.PrinterCostIskPerHour;
        var printPostProcessingLaborCostIsk =
            input.PrintPostProcessingMinutes / 60m * input.LaborRateIskPerHour;
        var customerConsultingCostIsk =
            input.CustomerConsultingMinutes / 60m * input.LaborRateIskPerHour;
        var partsDesignCostIsk =
            input.PartsDesignMinutes / 60m * input.LaborRateIskPerHour;
        var totalLaborCostIsk =
            printPostProcessingLaborCostIsk +
            customerConsultingCostIsk +
            partsDesignCostIsk;
        var landedCostIsk =
            materialCostIsk + printerCostIsk + totalLaborCostIsk +
            input.AdditionalCostIsk;
        var finalPriceIsk =
            landedCostIsk / (1m - input.TargetMarginPercent / 100m);
        return new(
            true, Array.Empty<string>(), requiredGrams, materialCostIsk,
            printerCostIsk, printPostProcessingLaborCostIsk,
            customerConsultingCostIsk, partsDesignCostIsk, totalLaborCostIsk,
            landedCostIsk, finalPriceIsk,
            finalPriceIsk / input.IskPerQuoteCurrencyUnit);
    }

    private static void Positive(decimal value, string label, ICollection<string> errors)
    {
        if (value <= 0) errors.Add(label + " must be greater than zero.");
    }

    private static void NonNegative(decimal value, string label, ICollection<string> errors)
    {
        if (value < 0) errors.Add(label + " cannot be negative.");
    }
}
