using System.Globalization;

namespace FilamentDbApp.Services;

public sealed class InventoryEngineService
{
    public InventorySummary Calculate(IEnumerable<InventoryItemInput> source)
    {
        var items = source.Where(x => !x.IsArchived).Select(CalculateItem).ToList();
        var values = items.Where(x => x.EstimatedValue.HasValue)
            .GroupBy(x => string.IsNullOrWhiteSpace(x.Currency) ? "Unknown" : x.Currency.Trim().ToUpperInvariant())
            .ToDictionary(g => g.Key, g => g.Sum(x => x.EstimatedValue!.Value));
        var averageCostPerKg = items
            .Where(x => x.CostPerKg.HasValue)
            .GroupBy(x => string.IsNullOrWhiteSpace(x.Currency) ? "Unknown" : x.Currency.Trim().ToUpperInvariant())
            .ToDictionary(g => g.Key, g => g.Average(x => x.CostPerKg!.Value));

        return new InventorySummary
        {
            Items = items,
            MaterialRows = items.Count,
            TotalSpools = items.Sum(x => x.Quantity),
            UnopenedSpools = items.Where(x => x.Status == "Unopened").Sum(x => x.Quantity),
            OpenedSpools = items.Where(x => x.Status == "Opened").Sum(x => x.Quantity),
            EmptySpools = items.Where(x => x.Status == "Empty").Sum(x => x.Quantity),
            TotalRemainingWeightG = items.Sum(x => x.EffectiveRemainingWeightG),
            TotalCapacityWeightG = items.Sum(x => x.TotalCapacityWeightG),
            EstimatedValueByCurrency = values,
            AverageCostPerKgByCurrency = averageCostPerKg,
            IssueCount = items.Count(x => !x.IsValid),
            CompleteRows = items.Count(x => x.IsComplete)
        };
    }

    private static InventoryItemResult CalculateItem(InventoryItemInput input)
    {
        var issues = new List<string>();
        var status = NormalizeStatus(input.Status, issues);
        var quantity = ParseNonNegative(input.Quantity, "Quantity", issues, defaultValue: 1, wholeNumber: true);
        var spoolWeight = ParseNonNegative(input.SpoolWeightG, "Spool weight", issues);
        var enteredRemaining = ParseNonNegative(input.RemainingWeightG, "Remaining weight", issues);
        var purchasePrice = ParseNonNegative(input.PurchasePriceAmount, "Purchase price", issues);
        var perSpoolCapacity = spoolWeight.GetValueOrDefault();
        var capacity = perSpoolCapacity * quantity;

        decimal effectiveRemaining;
        if (status == "Empty")
        {
            effectiveRemaining = 0;
            if (enteredRemaining.GetValueOrDefault() > 0) issues.Add("Empty status requires 0 g remaining");
        }
        else if (status == "Unopened")
        {
            // Inventory status is authoritative: an unopened spool is always valued
            // at full remaining capacity. A conflicting entered value is retained in
            // storage but surfaced as validation instead of reducing inventory value.
            effectiveRemaining = capacity;
            if (enteredRemaining.HasValue && capacity > 0 && enteredRemaining.Value != perSpoolCapacity)
                issues.Add("Unopened status requires full spool capacity remaining");
        }
        else
        {
            // Remaining Weight is entered per spool. Quantity scales the total
            // remaining inventory weight and value instead of diluting the ratio.
            effectiveRemaining = enteredRemaining.GetValueOrDefault() * quantity;
        }

        if (perSpoolCapacity > 0 && enteredRemaining.GetValueOrDefault() > perSpoolCapacity)
            issues.Add("Remaining weight per spool exceeds spool weight");
        if (!spoolWeight.HasValue || spoolWeight.Value <= 0) issues.Add("Missing spool weight");
        if (string.IsNullOrWhiteSpace(input.StorageLocation)) issues.Add("Missing storage location");
        if (!purchasePrice.HasValue) issues.Add("Missing purchase price");
        if (status == "Opened" && !enteredRemaining.HasValue) issues.Add("Missing remaining weight for opened spool");

        decimal? remainingPercent = capacity > 0 ? Math.Clamp(effectiveRemaining / capacity * 100m, 0m, 100m) : null;
        decimal? estimatedValue = purchasePrice.HasValue && capacity > 0
            ? purchasePrice.Value * quantity * Math.Clamp(effectiveRemaining / capacity, 0m, 1m)
            : null;
        decimal? costPerKg = purchasePrice.HasValue && perSpoolCapacity > 0
            ? purchasePrice.Value / (perSpoolCapacity / 1000m)
            : null;

        return new InventoryItemResult
        {
            InventoryItemId = input.InventoryItemId,
            MaterialID = input.MaterialID,
            DisplayName = input.DisplayName,
            Manufacturer = input.Manufacturer,
            BaseMaterial = input.BaseMaterial,
            StorageLocation = input.StorageLocation,
            Status = status,
            Quantity = quantity,
            SpoolWeightG = spoolWeight,
            EffectiveRemainingWeightG = effectiveRemaining,
            TotalCapacityWeightG = capacity,
            RemainingPercent = remainingPercent,
            EstimatedValue = estimatedValue,
            CostPerKg = costPerKg,
            Currency = input.Currency,
            Validation = issues.Count == 0 ? "OK" : string.Join("; ", issues),
            IsComplete = quantity > 0 && spoolWeight.HasValue && !string.IsNullOrWhiteSpace(input.StorageLocation)
        };
    }

    private static string NormalizeStatus(string? value, List<string> issues)
    {
        var status = string.IsNullOrWhiteSpace(value) ? "Unopened" : value.Trim();
        if (status is "Unopened" or "Opened" or "Empty") return status;
        issues.Add("Invalid inventory status");
        return status;
    }

    private static int ParseNonNegative(string? value, string label, List<string> issues, int defaultValue = 0, bool wholeNumber = false)
    {
        if (string.IsNullOrWhiteSpace(value)) return defaultValue;
        if (!decimal.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) || parsed < 0)
        {
            issues.Add(label + " is invalid");
            return defaultValue;
        }
        if (wholeNumber && parsed != decimal.Truncate(parsed)) issues.Add(label + " must be a whole number");
        return (int)decimal.Truncate(parsed);
    }

    private static decimal? ParseNonNegative(string? value, string label, List<string> issues)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (!decimal.TryParse(value.Replace(',', '.'), NumberStyles.Number, CultureInfo.InvariantCulture, out var parsed) || parsed < 0)
        {
            issues.Add(label + " is invalid");
            return null;
        }
        return parsed;
    }
}

public sealed class InventoryItemInput
{
    public string InventoryItemId { get; init; } = "";
    public string MaterialID { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Manufacturer { get; init; } = "";
    public string BaseMaterial { get; init; } = "";
    public string StorageLocation { get; init; } = "";
    public string Status { get; init; } = "";
    public string Quantity { get; init; } = "";
    public string SpoolWeightG { get; init; } = "";
    public string RemainingWeightG { get; init; } = "";
    public string PurchasePriceAmount { get; init; } = "";
    public string Currency { get; init; } = "";
    public bool IsArchived { get; init; }
}

public sealed class InventoryItemResult
{
    public string InventoryItemId { get; init; } = "";
    public string MaterialID { get; init; } = "";
    public string DisplayName { get; init; } = "";
    public string Manufacturer { get; init; } = "";
    public string BaseMaterial { get; init; } = "";
    public string StorageLocation { get; init; } = "";
    public string Status { get; init; } = "";
    public int Quantity { get; init; }
    public decimal? SpoolWeightG { get; init; }
    public decimal EffectiveRemainingWeightG { get; init; }
    public decimal TotalCapacityWeightG { get; init; }
    public decimal? RemainingPercent { get; init; }
    public decimal? EstimatedValue { get; init; }
    public decimal? CostPerKg { get; init; }
    public string Currency { get; init; } = "";
    public string Validation { get; init; } = "";
    public bool IsValid => Validation == "OK";
    public bool IsComplete { get; init; }
    public string RemainingDisplay => $"{EffectiveRemainingWeightG:N0} g";
    public string RemainingPercentDisplay => RemainingPercent.HasValue ? $"{RemainingPercent.Value:N1}%" : "—";
    public string EstimatedValueDisplay => EstimatedValue.HasValue ? $"{EstimatedValue.Value:N2} {Currency}" : "—";
    public string CostPerKgDisplay => CostPerKg.HasValue ? $"{CostPerKg.Value:N2} {Currency}/kg" : "—";
    public bool IsLowStock => Status == "Opened" && RemainingPercent.HasValue && RemainingPercent.Value < 20m;
    public bool IsMediumStock => Status == "Opened" && RemainingPercent.HasValue && RemainingPercent.Value >= 20m && RemainingPercent.Value <= 50m;
}

public sealed class InventorySummary
{
    public IReadOnlyList<InventoryItemResult> Items { get; init; } = Array.Empty<InventoryItemResult>();
    public int MaterialRows { get; init; }
    public int TotalSpools { get; init; }
    public int UnopenedSpools { get; init; }
    public int OpenedSpools { get; init; }
    public int EmptySpools { get; init; }
    public decimal TotalRemainingWeightG { get; init; }
    public decimal TotalCapacityWeightG { get; init; }
    public int IssueCount { get; init; }
    public int CompleteRows { get; init; }
    public IReadOnlyDictionary<string, decimal> EstimatedValueByCurrency { get; init; } = new Dictionary<string, decimal>();
    public IReadOnlyDictionary<string, decimal> AverageCostPerKgByCurrency { get; init; } = new Dictionary<string, decimal>();
    public decimal RemainingPercent => TotalCapacityWeightG > 0 ? TotalRemainingWeightG / TotalCapacityWeightG * 100m : 0m;
}
