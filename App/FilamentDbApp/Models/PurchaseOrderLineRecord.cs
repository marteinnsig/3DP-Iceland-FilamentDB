namespace FilamentDbApp.Models;

public sealed class PurchaseOrderLineRecord
{
    public string PurchaseOrderLineId { get; set; } = string.Empty;
    public string PurchaseOrderId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string InventoryCategory { get; set; } = "Filament";
    public string MaterialDisplayName { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Sku { get; set; } = string.Empty;
    public string Quantity { get; set; } = "1";
    public string ReceivedQuantity { get; set; } = "0";
    public string ReceivingStatus { get; set; } = "Not checked";
    public string StorageLocation { get; set; } = string.Empty;
    public string UnitPrice { get; set; } = string.Empty;
    public string DiscountAmount { get; set; } = string.Empty;
    public string UnitWeightG { get; set; } = string.Empty;
    public bool IncludeInCostAllocation { get; set; } = true;
    public string ManualShippingAllocation { get; set; } = string.Empty;
    public string ManualTaxAllocation { get; set; } = string.Empty;
    public string ManualCustomsAllocation { get; set; } = string.Empty;
    public string ManualFeesAllocation { get; set; } = string.Empty;
    public string NetLineCost { get; set; } = string.Empty;
    public string AllocatedShipping { get; set; } = string.Empty;
    public string AllocatedTax { get; set; } = string.Empty;
    public string AllocatedCustoms { get; set; } = string.Empty;
    public string AllocatedFees { get; set; } = string.Empty;
    public string LandedLineCost { get; set; } = string.Empty;
    public string LandedUnitCost { get; set; } = string.Empty;
    public string LandedCostPerKg { get; set; } = string.Empty;
    public string AllocationStatus { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
