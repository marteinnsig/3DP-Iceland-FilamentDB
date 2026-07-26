namespace FilamentDbApp.Models;

public sealed class InventorySpoolRecord
{
    public string DisplayLabel =>
        string.IsNullOrWhiteSpace(RemainingWeightG)
            ? InventoryItemId
            : $"{InventoryItemId} — {RemainingWeightG} g remaining";

    public string InventoryItemId { get; set; } = string.Empty;
    public string MaterialId { get; set; } = string.Empty;
    public string MaterialDisplayName { get; set; } = string.Empty;
    public string Status { get; set; } = "Unopened";
    public string Quantity { get; set; } = "1";
    public string SpoolWeightG { get; set; } = string.Empty;
    public string RemainingWeightG { get; set; } = string.Empty;
    public string StorageLocation { get; set; } = string.Empty;
    public string BatchNumber { get; set; } = string.Empty;
    public string PurchaseId { get; set; } = string.Empty;
    public string PurchaseOrderLineId { get; set; } = string.Empty;
    public string PurchasedFrom { get; set; } = string.Empty;
    public string PurchaseDate { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string PurchasePriceAmount { get; set; } = string.Empty;
    public string PurchaseCurrency { get; set; } = "ISK";
    public string ShippingAmount { get; set; } = string.Empty;
    public string VatAmount { get; set; } = string.Empty;
    public string CustomsAmount { get; set; } = string.Empty;
    public string OtherFeesAmount { get; set; } = string.Empty;
    public string LandedCostAmount { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
