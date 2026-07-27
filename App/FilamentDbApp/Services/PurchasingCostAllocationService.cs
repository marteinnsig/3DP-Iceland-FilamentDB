using System.Globalization;
using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

public sealed class PurchasingCostAllocationService
{
    public sealed record Result(
        bool IsValid,
        string Status,
        string ValidationDetails,
        decimal ItemsTotal,
        decimal Shipping,
        decimal Tax,
        decimal Customs,
        decimal Fees,
        decimal LandedTotal,
        string EffectiveShippingMethod);

    public Result Calculate(
        PurchaseOrderRecord order,
        IReadOnlyList<PurchaseOrderLineRecord> lines,
        decimal landedCostConversionRate = 1m)
    {
        if (landedCostConversionRate <= 0)
            return new(
                false,
                "Validation failed",
                "Landed-cost conversion rate must be positive.",
                0, 0, 0, 0, 0, 0,
                order.ShippingAllocationMethod);

        var workingLines = lines.Select(CloneForCalculation).ToList();
        var included = workingLines.Where(x => x.IncludeInCostAllocation).ToList();
        foreach (var l in workingLines) Clear(l);
        if (included.Count == 0)
            return new(false, "Validation failed", "No purchase lines are included in cost allocation.", 0, 0, 0, 0, 0, 0, order.ShippingAllocationMethod);
        foreach (var l in included) l.NetLineCost = F(Net(l));
        var shipping=Num(order.SupplierShipping); var tax=Num(order.SupplierTax)+Num(order.ImportVat); var customs=Num(order.CustomsDuty); var fees=Num(order.ClearanceFee)+Num(order.OtherFees);
        var effective=order.ShippingAllocationMethod;
        if (effective=="Automatic") effective = included.All(x => Weight(x)>0) ? "By weight" : "By line value";

        // A manually selected weight method is a strict instruction. Never silently
        // fall back to equal or value allocation when an included line lacks weight.
        var validationIssues = new List<string>();
        ValidateWeightMethod(included, shipping, order.ShippingAllocationMethod, "shipping", validationIssues);
        ValidateWeightMethod(included, tax, order.TaxAllocationMethod, "tax", validationIssues);
        ValidateWeightMethod(included, customs, order.CustomsAllocationMethod, "customs", validationIssues);
        ValidateWeightMethod(included, fees, order.FeeAllocationMethod, "fees", validationIssues);
        if (validationIssues.Count > 0)
        {
            var details = string.Join(Environment.NewLine, validationIssues.Distinct());
            foreach (var line in included) line.AllocationStatus = "Blocked: missing required weight";
            return new(false, "Validation failed", details, included.Sum(Net), shipping, tax, customs, fees, 0, effective);
        }

        var issues=new List<string>();
        Allocate(included, shipping, effective, x=>x.ManualShippingAllocation, (x,v)=>x.AllocatedShipping=F(v), issues, "shipping");
        Allocate(included, tax, order.TaxAllocationMethod, x=>x.ManualTaxAllocation, (x,v)=>x.AllocatedTax=F(v), issues, "tax");
        Allocate(included, customs, order.CustomsAllocationMethod, x=>x.ManualCustomsAllocation, (x,v)=>x.AllocatedCustoms=F(v), issues, "customs");
        Allocate(included, fees, order.FeeAllocationMethod, x=>x.ManualFeesAllocation, (x,v)=>x.AllocatedFees=F(v), issues, "fees");
        foreach(var l in included){var landedInvoice=Net(l)+Num(l.AllocatedShipping)+Num(l.AllocatedTax)+Num(l.AllocatedCustoms)+Num(l.AllocatedFees); var landed=landedInvoice*landedCostConversionRate; l.LandedLineCost=F(landed); var q=Qty(l); l.LandedUnitCost=q>0?F(landed/q):""; var kg=Weight(l)/1000m; l.LandedCostPerKg=kg>0?F(landed/kg):""; l.AllocationStatus=issues.Count==0?"Ready":string.Join("; ",issues.Distinct());}
        var item=included.Sum(Net);
        var status = issues.Count==0 ? "Ready" : string.Join("; ",issues.Distinct());
        if (issues.Count == 0)
        {
            for (var index = 0; index < lines.Count; index++)
                CopyCalculatedFields(workingLines[index], lines[index]);
        }
        return new(issues.Count==0, status, issues.Count==0 ? BuildSuccessDetails(order.ShippingAllocationMethod, effective) : status, item, shipping, tax, customs, fees, (item+shipping+tax+customs+fees)*landedCostConversionRate, effective);
    }

    private static PurchaseOrderLineRecord CloneForCalculation(PurchaseOrderLineRecord source) =>
        new()
        {
            PurchaseOrderLineId = source.PurchaseOrderLineId,
            PurchaseOrderId = source.PurchaseOrderId,
            Description = source.Description,
            Quantity = source.Quantity,
            UnitPrice = source.UnitPrice,
            DiscountAmount = source.DiscountAmount,
            UnitWeightG = source.UnitWeightG,
            IncludeInCostAllocation = source.IncludeInCostAllocation,
            ManualShippingAllocation = source.ManualShippingAllocation,
            ManualTaxAllocation = source.ManualTaxAllocation,
            ManualCustomsAllocation = source.ManualCustomsAllocation,
            ManualFeesAllocation = source.ManualFeesAllocation
        };

    private static void CopyCalculatedFields(PurchaseOrderLineRecord source, PurchaseOrderLineRecord target)
    {
        target.NetLineCost = source.NetLineCost;
        target.AllocatedShipping = source.AllocatedShipping;
        target.AllocatedTax = source.AllocatedTax;
        target.AllocatedCustoms = source.AllocatedCustoms;
        target.AllocatedFees = source.AllocatedFees;
        target.LandedLineCost = source.LandedLineCost;
        target.LandedUnitCost = source.LandedUnitCost;
        target.LandedCostPerKg = source.LandedCostPerKg;
        target.AllocationStatus = source.AllocationStatus;
    }

    static void ValidateWeightMethod(List<PurchaseOrderLineRecord> lines, decimal total, string? method, string label, List<string> issues)
    {
        if (total == 0 || !string.Equals(method, "By weight", StringComparison.OrdinalIgnoreCase)) return;
        var missing = lines.Where(x => Weight(x) <= 0).ToList();
        if (missing.Count == 0) return;
        var names = string.Join(", ", missing.Select(x => string.IsNullOrWhiteSpace(x.Description) ? x.PurchaseOrderLineId : x.Description));
        issues.Add($"Weight allocation cannot be performed for {label}. Missing unit weight: {names}. Enter the missing weight, exclude the line from allocation, or choose another allocation method.");
    }

    static string BuildSuccessDetails(string? selectedShippingMethod, string effectiveShippingMethod)
    {
        if (string.Equals(selectedShippingMethod, "Automatic", StringComparison.OrdinalIgnoreCase))
            return $"PASS – Automatic shipping allocation selected {effectiveShippingMethod}. All required allocation inputs are available.";
        return $"PASS – {effectiveShippingMethod} allocation inputs are valid.";
    }
    static void Allocate(List<PurchaseOrderLineRecord> lines, decimal total, string? method, Func<PurchaseOrderLineRecord,string> manual, Action<PurchaseOrderLineRecord,decimal> set, List<string> issues,string label){
        if(total==0){foreach(var l in lines)set(l,0);return;} method=string.IsNullOrWhiteSpace(method)?"By line value":method;
        if(method=="Manual"){var vals=lines.Select(x=>Num(manual(x))).ToList(); if(Math.Abs(vals.Sum()-total)>0.02m)issues.Add($"Manual {label} does not match total"); for(int i=0;i<lines.Count;i++)set(lines[i],vals[i]); return;}
        List<decimal> basis = method=="By weight" ? lines.Select(Weight).ToList() : method=="Equal per line" ? lines.Select(_=>1m).ToList() : lines.Select(Net).ToList();
        if(method=="By weight" && basis.Any(x=>x<=0)){issues.Add($"Weight required for {label}"); foreach(var l in lines)set(l,0);return;}
        var sum=basis.Sum(); if(sum<=0){issues.Add($"No allocation basis for {label}");return;} decimal assigned=0; for(int i=0;i<lines.Count;i++){var v=i==lines.Count-1?total-assigned:Math.Round(total*basis[i]/sum,2,MidpointRounding.AwayFromZero);assigned+=v;set(lines[i],v);}
    }
    static decimal Net(PurchaseOrderLineRecord l)=>Math.Max(0,Num(l.UnitPrice)*Qty(l)-Num(l.DiscountAmount));
    static decimal Qty(PurchaseOrderLineRecord l)=>Num(l.Quantity)>0?Num(l.Quantity):0;
    static decimal Weight(PurchaseOrderLineRecord l)=>Num(l.UnitWeightG)*Qty(l);
    static decimal Num(string? s)=>decimal.TryParse(s,NumberStyles.Any,CultureInfo.CurrentCulture,out var a)?a:decimal.TryParse(s?.Replace(',','.'),NumberStyles.Any,CultureInfo.InvariantCulture,out a)?a:0;
    static string F(decimal v)=>v.ToString("0.00",CultureInfo.CurrentCulture);
    static void Clear(PurchaseOrderLineRecord l){l.NetLineCost=l.AllocatedShipping=l.AllocatedTax=l.AllocatedCustoms=l.AllocatedFees=l.LandedLineCost=l.LandedUnitCost=l.LandedCostPerKg=l.AllocationStatus="";}
}
