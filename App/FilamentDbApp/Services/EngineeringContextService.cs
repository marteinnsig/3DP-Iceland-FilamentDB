using FilamentDbApp.Models;
using System.Globalization;

namespace FilamentDbApp.Services;

public sealed class EngineeringContextService
{
    public EngineeringContextInsight Analyze(
        string materialId,
        string manufacturerName,
        double? pricePerKg,
        InventorySummary inventory,
        IReadOnlyList<ManufacturerRecord> manufacturers)
    {
        var inventoryRows = inventory.Items
            .Where(item => item.MaterialID.Equals(materialId, StringComparison.OrdinalIgnoreCase))
            .ToList();
        var totalSpools = inventoryRows.Sum(item => item.Quantity);
        var remainingWeightG = inventoryRows.Sum(item => item.EffectiveRemainingWeightG);
        var locations = inventoryRows
            .Select(item => item.StorageLocation?.Trim())
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();

        var inventoryStatus = inventoryRows.Count == 0
            ? "Inventory not tracked"
            : remainingWeightG > 0
                ? "In stock"
                : "Out of stock";
        var inventorySummary = inventoryRows.Count == 0
            ? "No canonical inventory spool is linked to this MaterialID."
            : $"{totalSpools} spool{(totalSpools == 1 ? string.Empty : "s")} linked; {remainingWeightG:0} g remaining" +
              (locations.Count == 0 ? "." : $" across {string.Join(", ", locations)}.");

        var manufacturer = manufacturers.FirstOrDefault(item => item.IsActive &&
            (item.Name.Equals(manufacturerName, StringComparison.OrdinalIgnoreCase) ||
             item.DisplayName.Equals(manufacturerName, StringComparison.OrdinalIgnoreCase)));
        var manufacturerSummary = manufacturer is null
            ? string.IsNullOrWhiteSpace(manufacturerName)
                ? "Manufacturer is not identified for this material."
                : $"{manufacturerName}: no active manufacturer intelligence record is available."
            : BuildManufacturerSummary(manufacturer);

        return new EngineeringContextInsight
        {
            PriceSummary = pricePerKg.HasValue
                ? $"Public MSRP reference: ${pricePerKg.Value.ToString("0.00", CultureInfo.InvariantCulture)} USD/kg."
                : "Public MSRP reference is not available for this material.",
            InventoryStatus = inventoryStatus,
            InventorySummary = inventorySummary,
            ManufacturerSummary = manufacturerSummary,
            UsesCanonicalPricing = true,
            UsesInventoryEngineResults = true,
            UsesManufacturerRecords = true
        };
    }

    private static string BuildManufacturerSummary(ManufacturerRecord manufacturer)
    {
        var identity = string.IsNullOrWhiteSpace(manufacturer.DisplayName)
            ? manufacturer.Name.Trim()
            : manufacturer.DisplayName.Trim();
        var details = new[]
        {
            string.IsNullOrWhiteSpace(manufacturer.Country) ? null : manufacturer.Country.Trim(),
            string.IsNullOrWhiteSpace(manufacturer.EngineeringFocus) ? null : manufacturer.EngineeringFocus.Trim(),
            string.IsNullOrWhiteSpace(manufacturer.Strengths) ? null : manufacturer.Strengths.Trim()
        }.Where(value => !string.IsNullOrWhiteSpace(value));
        var context = string.Join(" · ", details);
        return string.IsNullOrWhiteSpace(context) ? identity : $"{identity}: {context}";
    }
}
