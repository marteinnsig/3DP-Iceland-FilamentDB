namespace FilamentDbApp.Models;

public sealed class PurchaseOrderRecord
{
    public string PurchaseOrderId { get; set; } = string.Empty;
    public string Supplier { get; set; } = string.Empty;
    public string OrderNumber { get; set; } = string.Empty;
    public string PurchaseDate { get; set; } = string.Empty;
    public string Currency { get; set; } = "ISK";
    public string ExchangeRate { get; set; } = "1";
    public string TaxTreatment { get; set; } = "VAT included in invoice";
    public string ShippingMethod { get; set; } = string.Empty;
    public string TrackingNumber { get; set; } = string.Empty;
    public string SupplierItemsTotal { get; set; } = string.Empty;
    public string SupplierShipping { get; set; } = string.Empty;
    public string ShippingAllocationMethod { get; set; } = "Automatic";
    public string TaxAllocationMethod { get; set; } = "By line value";
    public string CustomsAllocationMethod { get; set; } = "By line value";
    public string FeeAllocationMethod { get; set; } = "By line value";
    public string SupplierTax { get; set; } = string.Empty;
    public string SupplierInvoiceTotal { get; set; } = string.Empty;
    public string ImportVat { get; set; } = string.Empty;
    public string CustomsDuty { get; set; } = string.Empty;
    public string ClearanceFee { get; set; } = string.Empty;
    public string OtherFees { get; set; } = string.Empty;
    public string CostStatus { get; set; } = "Draft";
    public string LifecycleStatus { get; set; } = "Draft";
    public string ReceivedDate { get; set; } = string.Empty;
    public string InvoiceFile { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
}
