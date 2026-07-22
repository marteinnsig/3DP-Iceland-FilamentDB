using FilamentDbApp.Models;
using System.Data;

namespace FilamentDbApp.Services;

public sealed class MaterialDetailService
{
    private static readonly string[] KeyFieldNames =
    {
        "Material ID",
        "Manufacturer",
        "Product Line",
        "Marketing Name",
        "Base Material",
        "Type",
        "Reinforcement",
        "Variant / Finish",
        "Color",
        "Website Display Name",
        "YouTube URL",
        "YouTube Available"
    };


    private static readonly string[] GroupOrder =
    {
        "Basic Information",
        "Supplier / Purchase",
        "Pricing",
        "Material Information",
        "Test Information",
        "Website",
        "Notes",
        "Other"
    };

    private static readonly Dictionary<string, string[]> PreferredFieldOrder = new(StringComparer.OrdinalIgnoreCase)
    {
        ["Basic Information"] = new[] { "Material ID", "Material Name", "Manufacturer", "Brand", "Product Line", "Marketing Name", "Base Material", "Type", "Material Type", "Category", "Reinforcement", "Variant / Finish", "Variant", "Finish", "Color", "Colour", "Diameter" },
        ["Supplier / Purchase"] = new[] { "Inventory ID", "Purchase ID", "Purchased From", "Supplier", "Vendor", "Supplier URL", "Purchase Date", "Order Number", "Batch", "Batch Number", "Lot", "Storage Location", "Inventory Status", "Quantity", "Spool Weight", "Remaining Weight", "Net Weight", "Weight" },
        ["Pricing"] = new[] { "Purchase Price", "Purchase Currency", "Shipping", "VAT", "MSRP Amount", "MSRP Currency", "MSRP USD", "MSRP USD/kg", "Landed Cost Amount", "Landed Cost Currency", "Landed Cost USD", "Landed Cost USD/kg", "Price Checked", "Price Checked Date" },
        ["Material Information"] = new[] { "Density", "Nozzle", "Nozzle Size", "Print Temperature", "Printing Temperature", "Bed Temperature", "Chamber Temperature", "Drying", "Drying Temperature", "Drying Time", "Spool Material" },
        ["Test Information"] = new[] { "Tested Status", "Test Status", "Tensile", "Impact", "Stiffness", "Samples", "Rating", "Confidence" },
        ["Website"] = new[] { "Website Display Name", "Website Hidden", "Website Visible", "Manufacturer Website", "Manufacturer Web", "YouTube URL", "YouTube Review URL", "YouTube Available", "Thumbnail URL" },
        ["Notes"] = new[] { "Notes", "Technical Notes", "Tech Notes", "Test Notes" }
    };

    private static readonly (string ColumnName, string Label)[] LinkFields =
    {
        ("Manufacturer Website", "Open manufacturer website"),
        ("Manufacturer Web", "Open manufacturer website"),
        ("YouTube Review URL", "Open YouTube review"),
        ("YouTube URL", "Open YouTube review"),
        ("Thumbnail URL", "Open thumbnail/media URL")
    };

    public string BuildTitle(DataRow row)
    {
        return DataTableHelpers.FirstValue(row,
            "Website Display Name",
            "Material Name",
            "Marketing Name",
            "Material",
            "Product Line",
            "Material ID");
    }

    public string BuildSubtitle(DataRow row)
    {
        var parts = new[]
        {
            DataTableHelpers.FirstValue(row, "Manufacturer", "Brand"),
            DataTableHelpers.FirstValue(row, "Base Material", "Type", "Material Type"),
            DataTableHelpers.FirstValue(row, "Variant / Finish", "Variant", "Finish"),
            DataTableHelpers.FirstValue(row, "Color", "Colour")
        };

        return string.Join(" • ", parts.Where(v => !string.IsNullOrWhiteSpace(v)));
    }

    public IReadOnlyList<MaterialDetailField> BuildKeyFields(DataRow row)
    {
        return KeyFieldNames
            .Select(name => new MaterialDetailField(name, DataTableHelpers.FirstValue(row, name)))
            .Where(field => !string.IsNullOrWhiteSpace(field.Value))
            .ToList();
    }

    public IReadOnlyList<MaterialDetailField> BuildAllFields(DataRow row)
    {
        return row.Table.Columns.Cast<DataColumn>()
            .Select(column => new MaterialDetailField(column.ColumnName, row[column]?.ToString() ?? string.Empty))
            .Where(field => !string.IsNullOrWhiteSpace(field.Value))
            .ToList();
    }


    public IReadOnlyList<MaterialDetailGroup> BuildGroupedFields(DataRow row)
    {
        var grouped = GroupOrder.ToDictionary(name => name, _ => new List<MaterialDetailField>(), StringComparer.OrdinalIgnoreCase);

        foreach (DataColumn column in row.Table.Columns)
        {
            if (IsLegacyPerMaterialPrintingField(column.ColumnName)) continue;
            var value = row[column]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(value)) continue;

            var groupName = ClassifyField(column.ColumnName);
            grouped[groupName].Add(new MaterialDetailField(column.ColumnName, value));
        }

        return GroupOrder
            .Select(groupName => new MaterialDetailGroup(groupName, OrderFields(groupName, grouped[groupName])))
            .Where(group => group.Fields.Count > 0)
            .ToList();
    }

    private static IReadOnlyList<MaterialDetailField> OrderFields(string groupName, List<MaterialDetailField> fields)
    {
        if (!PreferredFieldOrder.TryGetValue(groupName, out var preferred))
        {
            return fields.OrderBy(field => field.Key, StringComparer.OrdinalIgnoreCase).ToList();
        }

        var positions = preferred
            .Select((name, index) => new { name, index })
            .ToDictionary(item => item.name, item => item.index, StringComparer.OrdinalIgnoreCase);

        return fields
            .OrderBy(field => positions.TryGetValue(field.Key, out var index) ? index : int.MaxValue)
            .ThenBy(field => field.Key, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private static string ClassifyField(string columnName)
    {
        var name = columnName.Trim();
        var lower = name.ToLowerInvariant();

        if (lower.Contains("note") || lower.Contains("comment") || lower.Contains("remark")) return "Notes";
        if (lower.Contains("website") || lower.Contains("youtube") || lower.Contains("thumbnail") || lower.Contains("url") || lower.Contains("web ")) return "Website";
        if (lower.Contains("msrp") || lower.Contains("price") || lower.Contains("cost") || lower.Contains("currency") || lower.Contains("landed")) return "Pricing";
        if (lower.Contains("supplier") || lower.Contains("vendor") || lower.Contains("purchase") || lower.Contains("batch") || lower.Contains("lot") || lower.Contains("country") || lower.Contains("spool weight") || lower.Contains("net weight")) return "Supplier / Purchase";
        if (lower.Contains("test") || lower.Contains("tensile") || lower.Contains("impact") || lower.Contains("stiffness") || lower.Contains("sample") || lower.Contains("rating") || lower.Contains("confidence") || lower.Contains("cv") || lower.Contains("std dev")) return "Test Information";
        if (lower.Contains("temperature") || lower.Contains("dry") || lower.Contains("density") || lower.Contains("nozzle") || lower.Contains("diameter") || lower.Contains("chamber") || lower.Contains("bed temp") || lower.Contains("print temp")) return "Material Information";
        if (lower.Contains("material id") || lower.Contains("material name") || lower.Contains("manufacturer") || lower.Contains("brand") || lower.Contains("product line") || lower.Contains("marketing name") || lower.Contains("base material") || lower == "type" || lower.Contains("material type") || lower.Contains("category") || lower.Contains("reinforcement") || lower.Contains("variant") || lower.Contains("finish") || lower.Contains("color") || lower.Contains("colour")) return "Basic Information";

        return "Other";
    }

    private static bool IsLegacyPerMaterialPrintingField(string columnName)
    {
        var lower = columnName.Trim().ToLowerInvariant();
        return lower.Contains("printing profile") || lower.Contains("printing settings") ||
               lower.Contains("nozzle temperature") || lower.Contains("bed temperature") ||
               lower.Contains("print speed") || lower.StartsWith("cooling ") ||
               lower.Contains("drying temperature") || lower.Contains("drying time") ||
               lower.Contains("enclosure requirement") || lower.Contains("printer profile") ||
               lower.Contains("slicer identity") || lower.Contains("slicer version") || lower.Contains("slicer profile");
    }

    public IReadOnlyList<MaterialLink> BuildLinks(DataRow row)
    {
        var links = new List<MaterialLink>();

        foreach (var (columnName, label) in LinkFields)
        {
            var url = DataTableHelpers.FirstValue(row, columnName);
            if (string.IsNullOrWhiteSpace(url)) continue;
            if (!Uri.TryCreate(url, UriKind.Absolute, out _)) continue;

            if (links.Any(link => string.Equals(link.Url, url, StringComparison.OrdinalIgnoreCase))) continue;
            links.Add(new MaterialLink(label, url));
        }

        return links;
    }
}
