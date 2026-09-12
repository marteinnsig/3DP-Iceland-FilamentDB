using FilamentDbApp.Models;
using FilamentDbApp.Services.Reporting;
using System.Text.Json;

namespace FilamentDbApp.Services.Website;

public sealed record FlexibleWebsiteMaterialInput(
    string MaterialId, IReadOnlyDictionary<string, object?> Fields, FlexibleMaterialEvidenceSnapshot? Evidence);

/// <summary>Website-only allowlist over saved public-safe aggregates; no pooling or score inference.</summary>
public sealed class FlexibleWebsiteService
{
    private const string ResourceName = "FilamentDbApp.Assets.Website.FlexibleTestingPortal.html";
    private const string DataToken = "__FLEXIBLE_DATA__";
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNamingPolicy = JsonNamingPolicy.CamelCase };

    public string BuildPage(IEnumerable<FlexibleWebsiteMaterialInput> materials)
    {
        using var stream = typeof(FlexibleWebsiteService).Assembly.GetManifestResourceStream(ResourceName)
            ?? throw new InvalidOperationException("The Flexible Testing website resource is unavailable.");
        using var reader = new System.IO.StreamReader(stream);
        var template = reader.ReadToEnd();
        if (!template.Contains(DataToken, StringComparison.Ordinal))
            throw new InvalidOperationException("The Flexible Testing website data placeholder is missing.");
        // The default JSON encoder escapes HTML delimiters, including any closing script text in catalog labels.
        return template.Replace(DataToken, JsonSerializer.Serialize(BuildRows(materials), JsonOptions), StringComparison.Ordinal);
    }

    public IReadOnlyList<FlexibleWebsiteRow> BuildRows(IEnumerable<FlexibleWebsiteMaterialInput> materials)
    {
        var rows = new List<FlexibleWebsiteRow>();
        var identities = new HashSet<string>(StringComparer.Ordinal);
        foreach (var material in materials)
        {
            var id = material.MaterialId.Trim();
            if (id.Length == 0 || material.Evidence is null ||
                !string.Equals(id, material.Evidence.MaterialId.Trim(), StringComparison.Ordinal)) continue;
            var fields = material.Fields;
            foreach (var group in FlexibleReportEvidenceService.Build(material.Evidence))
            {
                if (group.Unit == "N" || !identities.Add(id + "\n" + group.GroupId)) continue;
                rows.Add(new FlexibleWebsiteRow(id, Text(fields, "label"), Text(fields, "category"),
                    Text(fields, "type"), Text(fields, "variant"), Text(fields, "reinforcement"), Text(fields, "color"),
                    Text(fields, "manufacturer"), Text(fields, "productLine"), Text(fields, "brand"),
                    Text(fields, "marketingName"), Number(fields, "msrpUsdPerKg"), group.GroupId,
                    group.Metric, group.Condition, group.Unit, group.Mean, group.SpecimenCount,
                    group.StandardDeviation, group.CoefficientOfVariation, group.NotReachedCount)
                    { ShoreSpecimens = group.ShoreSpecimens });
            }
        }
        return rows;
    }

    private static string Text(IReadOnlyDictionary<string, object?> fields, string key) =>
        fields.TryGetValue(key, out var value) && value is string text ? text : string.Empty;

    private static double? Number(IReadOnlyDictionary<string, object?> fields, string key)
    {
        if (!fields.TryGetValue(key, out var value)) return null;
        var number = value switch
        {
            double d => d, float f => f, decimal d => (double)d, int i => i, long l => l, _ => double.NaN
        };
        return double.IsFinite(number) ? number : null;
    }

    public static bool VerifyContract()
    {
        var retained = new FlexibleMetricGroupSummary(FlexibleMetricKind.ForceRetention,
            "Force retention at 20% Strain", "PRIVATE-WEBSITE-METHOD-NOTES", "10–30 s · 2 mm displacement · cycle 1",
            "PRIVATE-WEBSITE-GROUP-KEY", 9, 92.262, 0.443, 0.481, 91.628, 92.889, "%", 0);
        var force = retained with { MetricKind = FlexibleMetricKind.CompressionForce,
            Metric = "Compression Force at 20% Strain", ComparisonKey = "force", Unit = "N" };
        var missing = retained with { ComparisonKey = "missing", Mean = double.NaN, StandardDeviation = null,
            CoefficientOfVariation = double.PositiveInfinity, Condition = "PRIVATE-WEBSITE-CONDITION" };
        var zero = retained with { ComparisonKey = "zero", Mean = 0, StandardDeviation = 0, CoefficientOfVariation = 0 };
        var fields = new Dictionary<string, object?> { ["label"] = "Synthetic </script> filament", ["type"] = "TPU",
            ["msrpUsdPerKg"] = double.PositiveInfinity, ["privateNotes"] = "PRIVATE-WEBSITE-CATALOG" };
        var input = new FlexibleWebsiteMaterialInput("FLEX-TEST", fields,
            new("FLEX-TEST", [retained, force, missing, zero, retained], 1, 9, 0));
        var rows = new FlexibleWebsiteService().BuildRows([input, input,
            input with { MaterialId = "WRONG-MATERIAL" }, input with { Evidence = null }]);
        var json = JsonSerializer.Serialize(rows, JsonOptions);
        using var document = JsonDocument.Parse(json);
        var properties = document.RootElement[0].EnumerateObject().Select(property => property.Name).ToHashSet();
        return rows.Count == 3 && rows[0].Mean == retained.Mean && rows[0].SpecimenCount == 9 &&
            rows[0].CoefficientOfVariation == 0.481 && rows[0].StandardDeviation == 0.443 &&
            rows[0].MsrpUsdPerKg is null && rows[1].Mean is null && rows[1].StandardDeviation is null &&
            rows[1].CoefficientOfVariation is null && rows[1].Condition == "Recorded condition unavailable" &&
            rows[2].Mean == 0 && rows[2].StandardDeviation == 0 && rows[2].CoefficientOfVariation == 0 &&
            rows.All(row => row.Unit != "N" && row.GroupId.Length == 64) && properties.Count == 22 &&
            !properties.Overlaps(["minimum", "maximum", "methodId", "methodGroup", "privateNotes"]) &&
            !json.Contains("PRIVATE-WEBSITE", StringComparison.Ordinal) &&
            !json.Contains("</script>", StringComparison.OrdinalIgnoreCase);
    }
}

public sealed record FlexibleWebsiteRow(
    string MaterialId, string Label, string Category, string Type, string Variant, string Reinforcement,
    string Color, string Manufacturer, string ProductLine, string Brand, string MarketingName, double? MsrpUsdPerKg,
    string GroupId, string Metric, string Condition, string Unit, double? Mean, int SpecimenCount,
    double? StandardDeviation, double? CoefficientOfVariation, int NotReachedCount)
{
    public IReadOnlyList<PublicShoreSpecimenStatistics> ShoreSpecimens { get; init; } = [];
}
