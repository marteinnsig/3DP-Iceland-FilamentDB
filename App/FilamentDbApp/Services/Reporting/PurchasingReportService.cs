using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Text;

namespace FilamentDbApp.Services.Reporting;

public sealed class PurchasingReportService
{
    private static decimal Num(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return 0m;

        var normalized = value.Trim()
            .Replace(" ", string.Empty)
            .Replace("\u00A0", string.Empty);

        // Purchasing values are persisted as text and may contain either Icelandic
        // decimal commas or invariant decimal points. Parsing with CurrentCulture
        // first is unsafe in is-IS because a value such as "149.70" can be read
        // as 14,970 (the point is treated as a thousands separator).
        if (normalized.Contains(',') && normalized.Contains('.'))
        {
            var decimalSeparator = normalized.LastIndexOf(',') > normalized.LastIndexOf('.') ? ',' : '.';
            normalized = decimalSeparator == ','
                ? normalized.Replace(".", string.Empty).Replace(',', '.')
                : normalized.Replace(",", string.Empty);

            return decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var mixed)
                ? mixed
                : 0m;
        }

        if (normalized.Contains('.'))
        {
            if (decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var invariantPoint))
                return invariantPoint;
        }

        if (normalized.Contains(','))
        {
            var commaDecimal = normalized.Replace(',', '.');
            if (decimal.TryParse(commaDecimal, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var invariantComma))
                return invariantComma;
        }

        if (decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var invariant))
            return invariant;
        if (decimal.TryParse(normalized, NumberStyles.Number | NumberStyles.AllowLeadingSign, CultureInfo.CurrentCulture, out var current))
            return current;
        return 0m;
    }

    private static decimal InvoiceRate(PurchaseOrderRecord order) =>
        Num(order.ExchangeRate) > 0 ? Num(order.ExchangeRate) : 1m;
    internal static decimal LandedIskRate(PurchaseOrderRecord order)
    {
        var conversionRate = Num(order.LandedCostConversionRate);
        return conversionRate > 0 ? InvoiceRate(order) / conversionRate : 0m;
    }
    private static string Money(decimal value) => value.ToString("N2", CultureInfo.CurrentCulture);
    private static string H(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
    private static string D(string? value) => H(ApplicationDateCodec.FormatForDisplay(value));
    private static string D(DateTime value) => H(ApplicationDateCodec.FormatForDisplay(value));
    private static decimal OrderLandedIsk(PurchaseOrderRecord order, IEnumerable<PurchaseOrderLineRecord> lines)
        => lines.Where(x => x.PurchaseOrderId == order.PurchaseOrderId).Sum(x => Num(x.LandedLineCost)) * LandedIskRate(order);

    public bool IsPurchasingReport(string key) => key is "inventory-report" or "purchase-report" or "supplier-report" or "low-stock-report" or "inventory-verification-report" or "purchasing-intelligence-report";

    public PurchasingReportResult Build(string key, IReadOnlyList<InventorySpoolRecord> inventory, IReadOnlyList<PurchaseOrderRecord> orders, IReadOnlyList<PurchaseOrderLineRecord> lines, DateTime generatedAt)
    {
        return key switch
        {
            "purchase-report" => Purchase(orders, lines, generatedAt),
            "supplier-report" => Supplier(orders, lines, generatedAt),
            "low-stock-report" => LowStock(inventory, generatedAt),
            "inventory-verification-report" => Verification(inventory, orders, lines, generatedAt),
            "purchasing-intelligence-report" => Intelligence(inventory, orders, lines, generatedAt),
            _ => Inventory(inventory, generatedAt)
        };
    }

    public PurchasingReportVerification Verify(IReadOnlyList<InventorySpoolRecord> inventory, IReadOnlyList<PurchaseOrderRecord> orders, IReadOnlyList<PurchaseOrderLineRecord> lines)
    {
        var keys = new[] { "inventory-report", "purchase-report", "supplier-report", "low-stock-report", "inventory-verification-report", "purchasing-intelligence-report" };
        var results = keys.Select(k => Build(k, inventory, orders, lines, DateTime.UtcNow)).ToList();
        return new PurchasingReportVerification(results.Count == 6 && results.All(x => !string.IsNullOrWhiteSpace(x.Text) && x.Html.Contains("<!doctype html>", StringComparison.OrdinalIgnoreCase)), results.Count);
    }

    private PurchasingReportResult Inventory(IReadOnlyList<InventorySpoolRecord> rows, DateTime at)
    {
        var active = rows.Where(x => !string.Equals(x.Status, "Empty", StringComparison.OrdinalIgnoreCase)).ToList();
        var totalSpools = rows.Sum(x => Math.Max(1m, Num(x.Quantity)));
        var remaining = rows.Sum(x => Num(x.RemainingWeightG) * Math.Max(1m, Num(x.Quantity)));
        var valueGroups = rows.GroupBy(x => string.IsNullOrWhiteSpace(x.LandedCostCurrency) ? "ISK" : x.LandedCostCurrency.Trim().ToUpperInvariant())
            .Select(g => new { Currency = g.Key, Value = g.Sum(x => Num(x.LandedCostAmount) * Math.Max(1m, Num(x.Quantity))) }).OrderBy(x => x.Currency).ToList();
        var status = rows.GroupBy(x => string.IsNullOrWhiteSpace(x.Status) ? "Unknown" : x.Status).Select(g => (g.Key, Count:g.Sum(x => Math.Max(1m, Num(x.Quantity))))).OrderByDescending(x=>x.Count).ToList();
        var text=new StringBuilder(); Header(text,"Inventory Report",at); text.AppendLine($"Inventory rows: {rows.Count}"); text.AppendLine($"Total spools: {totalSpools:N0}"); text.AppendLine($"Active rows: {active.Count}"); text.AppendLine($"Remaining weight: {remaining:N0} g"); text.AppendLine(); text.AppendLine("Value by currency:"); foreach(var g in valueGroups) text.AppendLine($"- {g.Currency}: {Money(g.Value)}"); text.AppendLine(); text.AppendLine("Status totals:"); foreach(var s in status) text.AppendLine($"- {s.Key}: {s.Count:N0}");
        var opened = rows.Where(x => string.Equals(x.Status, "Opened", StringComparison.OrdinalIgnoreCase)).Sum(x => Math.Max(1m, Num(x.Quantity)));
        var unopened = rows.Where(x => string.Equals(x.Status, "Unopened", StringComparison.OrdinalIgnoreCase)).Sum(x => Math.Max(1m, Num(x.Quantity)));
        var suppliers = rows.Where(x => !string.IsNullOrWhiteSpace(x.PurchasedFrom)).Select(x => x.PurchasedFrom.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count();
        var totalValueLabel = valueGroups.Count == 1 ? $"{Money(valueGroups[0].Value)} {H(valueGroups[0].Currency)}" : $"{valueGroups.Count} currencies";
        var body=$"<div class='section-title'>Inventory Overview</div><div class='cards'><div><b>Estimated value</b><span>{totalValueLabel}</span></div><div><b>Remaining weight</b><span>{remaining / 1000m:N2} kg</span></div><div><b>Total spools</b><span>{totalSpools:N0}</span></div><div><b>Opened</b><span>{opened:N0}</span></div><div><b>Unopened</b><span>{unopened:N0}</span></div><div><b>Suppliers</b><span>{suppliers}</span></div></div><h2>Value by currency</h2>{Table(new[]{"Currency","Estimated landed value"},valueGroups.Select(x=>new[]{H(x.Currency),Money(x.Value)}))}<h2>Inventory status</h2>{Table(new[]{"Status","Spools"},status.Select(x=>new[]{H(x.Key),x.Count.ToString("N0")}))}<h2>Inventory detail</h2>{Table(new[]{"Material","Status","Qty","Remaining g","Location","Supplier","Landed cost","Currency"},rows.OrderBy(x=>x.MaterialDisplayName).Select(x=>new[]{H(x.MaterialDisplayName),H(x.Status),H(x.Quantity),H(x.RemainingWeightG),H(x.StorageLocation),H(x.PurchasedFrom),H(x.LandedCostAmount),H(x.LandedCostCurrency)}))}";
        return Result("inventory-report","Inventory Report",text.ToString(),body,at,rows.Count);
    }

    private PurchasingReportResult Purchase(IReadOnlyList<PurchaseOrderRecord> orders, IReadOnlyList<PurchaseOrderLineRecord> lines, DateTime at)
    {
        var landed=orders.Sum(o=>OrderLandedIsk(o,lines)); var shipping=orders.Sum(o=>Num(o.SupplierShipping)*InvoiceRate(o)); var vat=orders.Sum(o=>(Num(o.SupplierTax)+Num(o.ImportVat))*InvoiceRate(o)); var customs=orders.Sum(o=>Num(o.CustomsDuty)*InvoiceRate(o));
        var text=new StringBuilder(); Header(text,"Purchase Report",at); text.AppendLine($"Orders: {orders.Count}"); text.AppendLine($"Order lines: {lines.Count}"); text.AppendLine($"Total landed cost: {Money(landed)} ISK"); text.AppendLine($"Shipping: {Money(shipping)} ISK"); text.AppendLine($"Tax / VAT: {Money(vat)} ISK"); text.AppendLine($"Customs: {Money(customs)} ISK");
        var averageOrder = orders.Count == 0 ? 0m : landed / orders.Count;
        var largestOrder = orders.Select(o => OrderLandedIsk(o, lines)).DefaultIfEmpty(0m).Max();
        var body=$"<div class='section-title'>Purchasing Overview</div><div class='cards'><div><b>Total orders</b><span>{orders.Count}</span></div><div><b>Total spend</b><span>{Money(landed)} ISK</span></div><div><b>Shipping</b><span>{Money(shipping)} ISK</span></div><div><b>Tax / VAT</b><span>{Money(vat)} ISK</span></div><div><b>Average order</b><span>{Money(averageOrder)} ISK</span></div><div><b>Largest order</b><span>{Money(largestOrder)} ISK</span></div></div><h2>Purchase totals</h2>{Table(new[]{"Metric","ISK"},new[]{new[]{"Shipping",Money(shipping)},new[]{"Tax / VAT",Money(vat)},new[]{"Customs",Money(customs)},new[]{"Total landed cost",Money(landed)}})}<h2>Orders</h2>{Table(new[]{"Date","Supplier","Order no.","Currency","Invoice total","Landed ISK","Status"},orders.OrderByDescending(x=>x.PurchaseDate).Select(o=>new[]{D(o.PurchaseDate),H(o.Supplier),H(o.OrderNumber),H(o.Currency),H(o.SupplierInvoiceTotal),Money(OrderLandedIsk(o,lines)),H(o.LifecycleStatus)}))}";
        return Result("purchase-report","Purchase Report",text.ToString(),body,at,orders.Count);
    }

    private PurchasingReportResult Supplier(IReadOnlyList<PurchaseOrderRecord> orders, IReadOnlyList<PurchaseOrderLineRecord> lines, DateTime at)
    {
        var groups=orders.GroupBy(x=>string.IsNullOrWhiteSpace(x.Supplier)?"Unknown supplier":x.Supplier.Trim()).Select(g=>new { Supplier=g.Key, Orders=g.Count(), First=g.Min(x=>x.PurchaseDate), Last=g.Max(x=>x.PurchaseDate), Landed=g.Sum(o=>OrderLandedIsk(o,lines)), Shipping=g.Sum(o=>Num(o.SupplierShipping)*InvoiceRate(o)), Lines=g.Sum(o=>lines.Count(l=>l.PurchaseOrderId==o.PurchaseOrderId)) }).OrderByDescending(x=>x.Landed).ToList();
        var text=new StringBuilder(); Header(text,"Supplier Report",at); text.AppendLine($"Suppliers: {groups.Count}"); text.AppendLine($"Orders: {orders.Count}"); foreach(var g in groups) text.AppendLine($"- {g.Supplier}: {g.Orders} orders, {Money(g.Landed)} ISK landed cost");
        var topSupplier = groups.FirstOrDefault();
        var totalSupplierSpend = groups.Sum(x => x.Landed);
        var supplierAverage = orders.Count == 0 ? 0m : totalSupplierSpend / orders.Count;
        var body=$"<div class='section-title'>Supplier Overview</div><div class='cards'><div><b>Top supplier</b><span>{H(topSupplier?.Supplier ?? "—")}</span></div><div><b>Top supplier spend</b><span>{Money(topSupplier?.Landed ?? 0m)} ISK</span></div><div><b>Suppliers</b><span>{groups.Count}</span></div><div><b>Total orders</b><span>{orders.Count}</span></div><div><b>Total spend</b><span>{Money(totalSupplierSpend)} ISK</span></div><div><b>Average order</b><span>{Money(supplierAverage)} ISK</span></div></div><h2>Supplier performance</h2>{Table(new[]{"Supplier","Orders","Lines","First purchase","Last purchase","Shipping ISK","Landed ISK","Average order ISK"},groups.Select(g=>new[]{H(g.Supplier),g.Orders.ToString(),g.Lines.ToString(),D(g.First),D(g.Last),Money(g.Shipping),Money(g.Landed),Money(g.Orders==0?0:g.Landed/g.Orders)}))}";
        return Result("supplier-report","Supplier Report",text.ToString(),body,at,groups.Count);
    }

    private PurchasingReportResult LowStock(IReadOnlyList<InventorySpoolRecord> rows, DateTime at)
    {
        var low=rows.Where(x=>string.Equals(x.Status,"Empty",StringComparison.OrdinalIgnoreCase)||Num(x.RemainingWeightG)<=250m||Num(x.Quantity)<=0m).OrderBy(x=>Num(x.RemainingWeightG)).ToList();
        var text=new StringBuilder(); Header(text,"Low Stock Report",at); text.AppendLine($"Items requiring review: {low.Count}"); text.AppendLine("Threshold: empty, quantity <= 0, or remaining weight <= 250 g."); foreach(var x in low) text.AppendLine($"- {x.MaterialDisplayName}: {x.Status}, {x.RemainingWeightG} g remaining");
        var body=$"<div class='cards'><div><b>Needs review</b><span>{low.Count}</span></div><div><b>Out of stock</b><span>{low.Count(x=>string.Equals(x.Status,"Empty",StringComparison.OrdinalIgnoreCase)||Num(x.Quantity)<=0)}</span></div><div><b>Under 100 g</b><span>{low.Count(x=>Num(x.RemainingWeightG)<=100)}</span></div></div><div class='note'>Low-stock threshold: 250 g remaining, empty status, or zero quantity.</div><h2>Reorder review</h2>{Table(new[]{"Material","Status","Quantity","Remaining g","Location","Last supplier","Last purchase"},low.Select(x=>new[]{H(x.MaterialDisplayName),H(x.Status),H(x.Quantity),H(x.RemainingWeightG),H(x.StorageLocation),H(x.PurchasedFrom),D(x.PurchaseDate)}))}";
        return Result("low-stock-report","Low Stock Report",text.ToString(),body,at,low.Count);
    }

    private PurchasingReportResult Intelligence(IReadOnlyList<InventorySpoolRecord> inventory, IReadOnlyList<PurchaseOrderRecord> orders, IReadOnlyList<PurchaseOrderLineRecord> lines, DateTime at)
    {
        var orderById = orders.ToDictionary(x => x.PurchaseOrderId, StringComparer.OrdinalIgnoreCase);
        var calculatedLines = lines.Where(x => Num(x.LandedLineCost) > 0m && orderById.ContainsKey(x.PurchaseOrderId)).ToList();
        var totalSpend = orders.Sum(o => OrderLandedIsk(o, lines));
        var totalShipping = orders.Sum(o => Num(o.SupplierShipping) * InvoiceRate(o));
        var averageOrder = orders.Count == 0 ? 0m : totalSpend / orders.Count;
        var shippingShare = totalSpend <= 0m ? 0m : totalShipping / totalSpend * 100m;

        var supplierRows = orders
            .GroupBy(x => string.IsNullOrWhiteSpace(x.Supplier) ? "Unknown supplier" : x.Supplier.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new
            {
                Supplier = g.Key,
                Orders = g.Count(),
                Spend = g.Sum(o => OrderLandedIsk(o, lines)),
                Shipping = g.Sum(o => Num(o.SupplierShipping) * InvoiceRate(o)),
                LastPurchase = g.Select(o => ParseDate(o.PurchaseDate)).Where(d => d.HasValue).Select(d => d!.Value).DefaultIfEmpty().Max()
            })
            .OrderByDescending(x => x.Spend)
            .ToList();

        var topSupplier = supplierRows.FirstOrDefault();
        var topSupplierShare = totalSpend <= 0m || topSupplier is null ? 0m : topSupplier.Spend / totalSpend * 100m;

        var monthlyRows = orders
            .Select(o => new { Order = o, Date = ParseDate(o.PurchaseDate) })
            .Where(x => x.Date.HasValue)
            .GroupBy(x => x.Date!.Value.ToString("yyyy-MM"))
            .Select(g => new { Month = g.Key, Orders = g.Count(), Spend = g.Sum(x => OrderLandedIsk(x.Order, lines)) })
            .OrderByDescending(x => x.Month)
            .ToList();

        var categoryRows = calculatedLines
            .GroupBy(x => string.IsNullOrWhiteSpace(x.InventoryCategory) ? "Other" : x.InventoryCategory.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g => new
            {
                Category = g.Key,
                Lines = g.Count(),
                Spend = g.Sum(x => Num(x.LandedLineCost) * LandedIskRate(orderById[x.PurchaseOrderId]))
            })
            .OrderByDescending(x => x.Spend)
            .ToList();

        var materialRows = calculatedLines
            .Where(x => !string.IsNullOrWhiteSpace(x.MaterialId) || !string.IsNullOrWhiteSpace(x.MaterialDisplayName))
            .GroupBy(x => !string.IsNullOrWhiteSpace(x.MaterialId) ? x.MaterialId.Trim() : x.MaterialDisplayName.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(g =>
            {
                var purchases = g.Select(x => new
                {
                    Line = x,
                    Order = orderById[x.PurchaseOrderId],
                    UnitIsk = Num(x.LandedUnitCost) * LandedIskRate(orderById[x.PurchaseOrderId])
                }).Where(x => x.UnitIsk > 0m).ToList();
                var latest = purchases.OrderByDescending(x => ParseDate(x.Order.PurchaseDate) ?? DateTime.MinValue).FirstOrDefault();
                return new
                {
                    Material = g.Select(x => x.MaterialDisplayName).FirstOrDefault(x => !string.IsNullOrWhiteSpace(x)) ?? g.Key,
                    Purchases = purchases.Count,
                    Lowest = purchases.Select(x => x.UnitIsk).DefaultIfEmpty(0m).Min(),
                    Average = purchases.Select(x => x.UnitIsk).DefaultIfEmpty(0m).Average(),
                    Highest = purchases.Select(x => x.UnitIsk).DefaultIfEmpty(0m).Max(),
                    LastSupplier = latest?.Order.Supplier ?? string.Empty,
                    LastDate = latest?.Order.PurchaseDate ?? string.Empty
                };
            })
            .Where(x => x.Purchases > 0)
            .OrderByDescending(x => x.Purchases)
            .ThenBy(x => x.Material)
            .ToList();

        var recommendations = new List<(string Priority, string Recommendation, string Reason)>();
        var lowStock = inventory.Where(x => string.Equals(x.Status, "Empty", StringComparison.OrdinalIgnoreCase) || Num(x.Quantity) <= 0m || Num(x.RemainingWeightG) <= 250m).ToList();
        if (lowStock.Count > 0)
            recommendations.Add(("High", "Review low-stock materials", $"{lowStock.Count} inventory item(s) are empty, zero quantity, or at/below 250 g."));
        if (topSupplierShare >= 60m && supplierRows.Count > 1)
            recommendations.Add(("Medium", "Review supplier concentration", $"{topSupplier!.Supplier} represents {topSupplierShare:N1}% of landed purchasing spend."));
        var missingCosts = lines.Count(x => x.IncludeInCostAllocation && Num(x.LandedLineCost) <= 0m);
        if (missingCosts > 0)
            recommendations.Add(("High", "Complete landed-cost calculation", $"{missingCosts} included purchase line(s) have no persisted landed cost."));
        var volatileMaterials = materialRows.Where(x => x.Purchases >= 2 && x.Lowest > 0m && (x.Highest - x.Lowest) / x.Lowest >= 0.15m).ToList();
        if (volatileMaterials.Count > 0)
            recommendations.Add(("Medium", "Compare historical material prices", $"{volatileMaterials.Count} material(s) have at least 15% spread between lowest and highest landed unit cost."));
        var unlinkedInventory = inventory.Count(x => string.IsNullOrWhiteSpace(x.PurchaseOrderLineId));
        if (unlinkedInventory > 0)
            recommendations.Add(("Low", "Link legacy inventory to purchase history", $"{unlinkedInventory} inventory row(s) are not linked to a purchase-order line."));
        if (recommendations.Count == 0)
            recommendations.Add(("Info", "No immediate purchasing action", "Current purchasing and inventory data produced no rule-based alerts."));

        var text = new StringBuilder();
        Header(text, "Purchasing Intelligence Report", at);
        text.AppendLine($"Total spend: {Money(totalSpend)} ISK");
        text.AppendLine($"Average order: {Money(averageOrder)} ISK");
        text.AppendLine($"Shipping share: {shippingShare:N1}%");
        text.AppendLine($"Top supplier: {topSupplier?.Supplier ?? "—"} ({topSupplierShare:N1}% of spend)");
        text.AppendLine($"Recommendations: {recommendations.Count}");
        foreach (var item in recommendations) text.AppendLine($"- [{item.Priority}] {item.Recommendation}: {item.Reason}");

        var body = $"<div class='section-title'>Purchasing Intelligence Overview</div>" +
            $"<div class='cards'><div><b>Total spend</b><span>{Money(totalSpend)} ISK</span></div><div><b>Average order</b><span>{Money(averageOrder)} ISK</span></div><div><b>Shipping share</b><span>{shippingShare:N1}%</span></div><div><b>Top supplier</b><span>{H(topSupplier?.Supplier ?? "—")}</span></div><div><b>Supplier share</b><span>{topSupplierShare:N1}%</span></div><div><b>Actions</b><span>{recommendations.Count}</span></div></div>" +
            $"<h2>Decision guidance</h2>{Table(new[] { "Priority", "Recommendation", "Reason" }, recommendations.Select(x => new[] { H(x.Priority), H(x.Recommendation), H(x.Reason) }))}" +
            $"<h2>Supplier intelligence</h2>{Table(new[] { "Supplier", "Orders", "Landed spend ISK", "Shipping ISK", "Spend share", "Last purchase" }, supplierRows.Select(x => new[] { H(x.Supplier), x.Orders.ToString(), Money(x.Spend), Money(x.Shipping), totalSpend <= 0m ? "0.0%" : $"{x.Spend / totalSpend * 100m:N1}%", x.LastPurchase == default ? "" : D(x.LastPurchase) }))}" +
            $"<h2>Monthly spend</h2>{Table(new[] { "Month", "Orders", "Landed spend ISK" }, monthlyRows.Select(x => new[] { H(x.Month), x.Orders.ToString(), Money(x.Spend) }))}" +
            $"<h2>Spend by category</h2>{Table(new[] { "Category", "Lines", "Landed spend ISK", "Spend share" }, categoryRows.Select(x => new[] { H(x.Category), x.Lines.ToString(), Money(x.Spend), totalSpend <= 0m ? "0.0%" : $"{x.Spend / totalSpend * 100m:N1}%" }))}" +
            $"<h2>Material price history</h2>{Table(new[] { "Material", "Purchases", "Lowest unit ISK", "Average unit ISK", "Highest unit ISK", "Last supplier", "Last purchase" }, materialRows.Select(x => new[] { H(x.Material), x.Purchases.ToString(), Money(x.Lowest), Money(x.Average), Money(x.Highest), H(x.LastSupplier), D(x.LastDate) }))}";

        return Result("purchasing-intelligence-report", "Purchasing Intelligence Report", text.ToString(), body, at, recommendations.Count);
    }

    private static DateTime? ParseDate(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        if (ApplicationDateCodec.TryParseStored(value, out var exact)) return exact;
        if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var current)) return current;
        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.None, out var invariant)) return invariant;
        return null;
    }

    private PurchasingReportResult Verification(IReadOnlyList<InventorySpoolRecord> rows, IReadOnlyList<PurchaseOrderRecord> orders, IReadOnlyList<PurchaseOrderLineRecord> lines, DateTime at)
    {
        var orderIds=orders.Select(x=>x.PurchaseOrderId).ToHashSet(StringComparer.OrdinalIgnoreCase); var lineIds=lines.Select(x=>x.PurchaseOrderLineId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var issues=new List<(string,string,string)>();
        foreach(var x in rows){if(Num(x.RemainingWeightG)<0)issues.Add((x.InventoryItemId,"Negative remaining weight",x.RemainingWeightG)); if(string.IsNullOrWhiteSpace(x.MaterialId))issues.Add((x.InventoryItemId,"Missing material link",x.MaterialDisplayName)); if(string.IsNullOrWhiteSpace(x.PurchaseCurrency)||string.IsNullOrWhiteSpace(x.LandedCostCurrency))issues.Add((x.InventoryItemId,"Missing currency",x.MaterialDisplayName)); if(!string.IsNullOrWhiteSpace(x.PurchaseOrderLineId)&&!lineIds.Contains(x.PurchaseOrderLineId))issues.Add((x.InventoryItemId,"Orphan purchase-line link",x.PurchaseOrderLineId));}
        foreach(var x in lines){if(!orderIds.Contains(x.PurchaseOrderId))issues.Add((x.PurchaseOrderLineId,"Orphan purchase-order line",x.PurchaseOrderId)); if(string.Equals(x.AllocationStatus,"Calculated",StringComparison.OrdinalIgnoreCase)&&string.IsNullOrWhiteSpace(x.LandedLineCost))issues.Add((x.PurchaseOrderLineId,"Calculated line missing landed cost",x.Description));}
        foreach(var g in rows.Where(x=>!string.IsNullOrWhiteSpace(x.BatchNumber)).GroupBy(x=>x.BatchNumber,StringComparer.OrdinalIgnoreCase).Where(g=>g.Count()>1))issues.Add((g.Key,"Duplicate batch number",$"{g.Count()} rows"));
        var text=new StringBuilder(); Header(text,"Inventory Verification Report",at); text.AppendLine($"Overall: {(issues.Count==0?"PASS":"REVIEW")}"); text.AppendLine($"Issues: {issues.Count}"); foreach(var i in issues)text.AppendLine($"- {i.Item1}: {i.Item2} ({i.Item3})");
        var body=$"<div class='cards'><div><b>Overall</b><span>{(issues.Count==0?"PASS":"REVIEW")}</span></div><div><b>Inventory rows</b><span>{rows.Count}</span></div><div><b>Issues</b><span>{issues.Count}</span></div></div><h2>Verification findings</h2>{(issues.Count==0?"<div class='pass'>No purchasing or inventory integrity issues were found.</div>":Table(new[]{"Record","Issue","Detail"},issues.Select(i=>new[]{H(i.Item1),H(i.Item2),H(i.Item3)})))}";
        return Result("inventory-verification-report","Inventory Verification Report",text.ToString(),body,at,issues.Count);
    }

    private static void Header(StringBuilder sb,string title,DateTime at){sb.AppendLine("3DPIceland Engineering Platform");sb.AppendLine(title);sb.AppendLine(new string('=',title.Length));sb.AppendLine($"Generated: {at:yyyy-MM-dd HH:mm:ss}");sb.AppendLine();}
    private static string Table(IEnumerable<string> headers,IEnumerable<string[]> rows){var h=string.Join("",headers.Select(x=>$"<th>{H(x)}</th>"));var r=string.Join("",rows.Select(row=>"<tr>"+string.Join("",row.Select(x=>$"<td>{x}</td>"))+"</tr>"));if(string.IsNullOrEmpty(r))r=$"<tr><td colspan='{headers.Count()}'>No records</td></tr>";return $"<table><thead><tr>{h}</tr></thead><tbody>{r}</tbody></table>";}
    private static PurchasingReportResult Result(string key,string title,string text,string body,DateTime at,int count)=>new(key,title,text,HtmlDocument(title,body,at),count);
    private static string HtmlDocument(string title,string body,DateTime at)=>$@"<!doctype html><html><head><meta charset='utf-8'><title>{H(title)}</title><style>@page{{size:A4 landscape;margin:14mm}}body{{font-family:Segoe UI,Arial;color:#0f172a;margin:0}}header{{border-bottom:3px solid #2563eb;margin-bottom:18px;padding-bottom:10px}}h1{{margin:0;font-size:28px}}h2{{margin-top:24px}}.meta{{color:#64748b}}.section-title{{font-size:18px;font-weight:700;color:#1e3a8a;margin-top:4px}}.cards{{display:grid;grid-template-columns:repeat(3,1fr);gap:12px;margin:18px 0}}.cards div{{border:1px solid #cbd5e1;border-radius:8px;padding:14px;background:#f8fafc}}.cards b{{display:block;color:#475569}}.cards span{{display:block;font-size:22px;font-weight:700;margin-top:5px}}table{{width:100%;border-collapse:collapse;font-size:11px}}th,td{{border:1px solid #cbd5e1;padding:7px;text-align:left}}th{{background:#e2e8f0}}tr:nth-child(even){{background:#f8fafc}}.note{{padding:12px;background:#eff6ff;border-left:4px solid #2563eb}}.pass{{padding:18px;background:#ecfdf5;border:1px solid #10b981;border-radius:8px;font-weight:700}}</style></head><body><header><h1>{H(title)}</h1><div class='meta'>3DPIceland Engineering Platform • Generated {at:yyyy-MM-dd HH:mm:ss}</div></header>{body}</body></html>";
}

public sealed record PurchasingReportResult(string Key,string Title,string Text,string Html,int RecordCount);
public sealed record PurchasingReportVerification(bool Passed,int ReportsGenerated);
