using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Reporting;

/// <summary>Public-safe aggregates shared by private and public report rendering.</summary>
public static class FlexibleReportEvidenceService
{
    private const string Number = @"(?:[+-]?(?:\d+(?:\.\d*)?|\.\d+)(?:[Ee][+-]?\d+)?|not recorded)";
    public static IReadOnlyList<PublicFlexibleMetricGroup> Build(FlexibleMaterialEvidenceSnapshot? snapshot) =>
        snapshot?.Groups.Select(group => new PublicFlexibleMetricGroup(
            Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(group.ComparisonKey))).ToLowerInvariant(),
            SafeMetric(group), SafeCondition(group), Math.Max(0, group.SpecimenCount),
            Finite(group.Mean), Finite(group.StandardDeviation), Finite(group.CoefficientOfVariation),
            Finite(group.Minimum), Finite(group.Maximum), Unit(group.MetricKind), Math.Max(0, group.NotReachedCount))
            { MethodId = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(group.MethodGroup))).ToLowerInvariant() })
            .ToArray() ?? [];

    private static double? Finite(double? value) => value is double number && double.IsFinite(number) ? number : null;
    private static string Unit(FlexibleMetricKind kind) => kind switch
    {
        FlexibleMetricKind.CompressionForce => "N", FlexibleMetricKind.ApparentCompressiveStress => "MPa",
        FlexibleMetricKind.ShoreA => "Shore A", FlexibleMetricKind.ShoreD => "Shore D", _ => "%"
    };
    private static string SafeMetric(FlexibleMetricGroupSummary group)
    {
        var (label, pattern) = group.MetricKind switch
        {
            FlexibleMetricKind.CompressionForce => ("Compression force", $@"Compression Force at {Number}% Strain"),
            FlexibleMetricKind.ApparentCompressiveStress => ("Apparent compressive stress", $@"Apparent Compressive Stress at {Number}% Strain"),
            FlexibleMetricKind.ForceRetention => ("Force retention", $@"Force retention at {Number}% Strain"),
            FlexibleMetricKind.ForceReduction => ("Force reduction", $@"Force reduction from 10 s to 30 s at {Number}% Strain"),
            FlexibleMetricKind.ResidualHeightLoss => ("Residual height loss after recovery", "Residual height loss after recovery"),
            FlexibleMetricKind.ShoreA => ("Shore A hardness", "Shore A Hardness"),
            _ => ("Shore D hardness", "Shore D Hardness")
        };
        return Matches(group.Metric, pattern) ? group.Metric : label;
    }
    private static string SafeCondition(FlexibleMetricGroupSummary group)
    {
        var pattern = group.MetricKind switch
        {
            FlexibleMetricKind.CompressionForce or FlexibleMetricKind.ApparentCompressiveStress => $@"{Number} s hold · {Number} mm displacement · cycle \d+",
            FlexibleMetricKind.ForceRetention => $@"{Number}–{Number} s · {Number} mm displacement · cycle \d+",
            FlexibleMetricKind.ForceReduction => $@"first compression only · {Number} mm displacement",
            FlexibleMetricKind.ResidualHeightLoss => $@"{Number}% compression · {Number} s compressed hold · {Number} s rest · {Number} mm initial height · cycle \d+",
            _ => $@"{Number} s reading · {Number} mm"
        };
        return Matches(group.Condition, pattern) ? group.Condition : "Recorded condition unavailable";
    }
    private static bool Matches(string value, string pattern) => value.Length <= 512 &&
        Regex.IsMatch(value, $@"\A(?:{pattern})\z", RegexOptions.CultureInvariant, TimeSpan.FromMilliseconds(100));
    private static string MethodLabel(PublicFlexibleMetricGroup group) =>
        Regex.IsMatch(group.MethodId, "^[a-f0-9]{64}$", RegexOptions.CultureInvariant) ? group.MethodId[..8] : "Unspecified";
    private static string H(string value) => WebUtility.HtmlEncode(value);
    private static string N(double? value) => Finite(value)?.ToString("0.###", CultureInfo.InvariantCulture) ?? "—";

    public static string RenderHtml(IReadOnlyList<PublicFlexibleMetricGroup> groups)
    {
        if (groups.Count == 0) return string.Empty;
        var rows = string.Concat(groups.Select((g, index) =>
            $"<tr style=\"break-inside:avoid;page-break-inside:avoid\"><td style=\"font-size:0.85em;white-space:nowrap\">{MethodLabel(g)}</td><td>{H(g.Metric)} ({H(g.Unit)})</td><td style=\"max-width:24em;white-space:normal;overflow-wrap:anywhere\">{H(g.Condition)}</td><td>{N(g.Mean)}</td><td>{g.SpecimenCount}</td><td>{N(g.StandardDeviation)}</td><td>{N(g.CoefficientOfVariation)}</td><td>{N(g.Minimum)}–{N(g.Maximum)}</td><td>{g.NotReachedCount}</td></tr>"));
        return "<section class=\"flexible-evidence\"><h2>Flexible Material Testing</h2><p>Comparative in-house measurements; n = independent specimens. Separate test groups; Overall unchanged.</p>" +
            "<div style=\"max-width:100%;overflow-x:auto\"><table style=\"width:auto;max-width:100%;font-size:0.85em\"><thead><tr><th>Method</th><th>Result</th><th>Condition</th><th>Mean</th><th>n</th><th>SD</th><th>CV %</th><th>Range</th><th>Not reached</th></tr></thead><tbody>" + rows + "</tbody></table></div></section>";
    }
    public static string RenderText(IReadOnlyList<PublicFlexibleMetricGroup> groups) => groups.Count == 0 ? string.Empty :
        "Flexible Material Testing\nComparative in-house measurements; n = independent specimens. Overall unchanged.\n" +
        string.Join("\n", groups.Select((g, i) => $"Method {MethodLabel(g)}: {g.Metric} ({g.Unit}); {g.Condition}; mean {N(g.Mean)}; n {g.SpecimenCount}; SD {N(g.StandardDeviation)}; CV {N(g.CoefficientOfVariation)}%; range {N(g.Minimum)}–{N(g.Maximum)}; not reached {g.NotReachedCount}."));

    /// <summary>Pure synthetic acceptance: all six public templates receive the same safe aggregate.</summary>
    public static bool VerifyPublishersContract()
    {
        var source = new FlexibleMetricGroupSummary(FlexibleMetricKind.CompressionForce,
            "Compression Force at 20% Strain", "PRIVATE-FLEX-PRINTER-NOTES",
            "30 s hold · 2 mm displacement · cycle 1", "PRIVATE-FLEX-COMPARISON-KEY",
            3, 123.456, 2.345, 1.9, 120, 126, "N", 1);
        var groups = Build(new("PRIVATE-FLEX-SPECIMEN", [source], 1, 3, 0));
        var at = new DateTime(2026, 9, 12, 12, 0, 0, DateTimeKind.Utc);
        const string version = "verification";
        const string title = "Synthetic Flexible report";
        var engineering = new PublicReportPublishingService().Build(new()
        {
            MaterialId = "MAT-FLEX-TEST", MaterialName = "Synthetic flexible material", FlexibleResults = groups
        }, at, version, title);
        var comparison = new PublicComparisonReportPublishingService().Build(new()
        {
            PresetSlug = "synthetic-flexible", Title = title,
            Materials = [new() { MaterialId = "MAT-FLEX-TEST", MaterialName = "Synthetic flexible material", FlexibleResults = groups },
                new() { MaterialId = "MAT-NO-FLEX", MaterialName = "Synthetic unmeasured peer" }]
        }, at, version, title);
        var manufacturer = new PublicManufacturerReportPublishingService().Build(new()
        {
            ManufacturerSlug = "synthetic-flexible", Manufacturer = "Synthetic manufacturer",
            Materials = [new() { MaterialId = "MAT-FLEX-TEST", MaterialName = "Synthetic flexible material", FlexibleResults = groups }]
        }, at, version, title);
        var summary = new PublicMaterialSummaryReportPublishingService().Build(new()
        {
            PublicMaterials = 1,
            Materials = [new() { MaterialId = "MAT-FLEX-TEST", MaterialName = "Synthetic flexible material", FlexibleResults = groups }]
        }, at, version, title);
        var session = new PublicTestSessionReportPublishingService().Build(new()
        {
            MaterialId = "MAT-FLEX-TEST", MaterialName = "Synthetic flexible material", FlexibleResults = groups,
            PublicDetailsApproved = false
        }, at, version, title);
        var recommendation = new PublicPrintingRecommendationReportPublishingService().Build(new()
        {
            MaterialId = "MAT-FLEX-TEST", MaterialName = "Synthetic flexible material", FlexibleResults = groups
        }, at, version, title);
        (string Html, string Json)[] reports =
        [
            (engineering.Html, engineering.MetadataJson), (comparison.Html, comparison.MetadataJson),
            (manufacturer.Html, manufacturer.MetadataJson), (summary.Html, summary.MetadataJson),
            (session.Html, session.MetadataJson), (recommendation.Html, recommendation.MetadataJson)
        ];
        var expectedProperties = new[] { "GroupId", "MethodId", "Metric", "Condition", "SpecimenCount", "Mean",
            "StandardDeviation", "CoefficientOfVariation", "Minimum", "Maximum", "Unit", "NotReachedCount" };
        using var dto = JsonDocument.Parse(JsonSerializer.Serialize(groups[0]));
        var closedShape = dto.RootElement.EnumerateObject().Select(x => x.Name).OrderBy(x => x, StringComparer.Ordinal)
            .SequenceEqual(expectedProperties.OrderBy(x => x, StringComparer.Ordinal));
        return VerifyContract() && closedShape && reports.All(report =>
            report.Html.Contains("Flexible Material Testing", StringComparison.Ordinal) &&
            report.Html.Contains("123.456", StringComparison.Ordinal) &&
            report.Html.Contains("2.345", StringComparison.Ordinal) &&
            report.Json.Contains("\"FlexibleResults\"", StringComparison.Ordinal) &&
            report.Json.Contains("\"Mean\": 123.456", StringComparison.Ordinal) &&
            !report.Html.Contains("PRIVATE-FLEX", StringComparison.Ordinal) &&
            !report.Json.Contains("PRIVATE-FLEX", StringComparison.Ordinal));
    }
    public static bool VerifyContract()
    {
        var source = new FlexibleMetricGroupSummary(FlexibleMetricKind.CompressionForce,
            "Compression Force at 20% Strain", "private printer notes", "30 s hold · 2 mm displacement · cycle 1",
            "private-key", 2, 123, null, null, 120, 126, "N", 0);
        var groups = Build(new("internal-material-id", [source, source with { ComparisonKey = "other-private-key", Condition = "secret.sqlite", Mean = double.NaN }], 1, 2, 0));
        var json = JsonSerializer.Serialize(groups);
        return groups.Count == 2 && groups[0].GroupId != groups[1].GroupId && groups[1].Mean is null &&
            groups[0].StandardDeviation is null && groups[0].Condition == source.Condition &&
            !json.Contains("private", StringComparison.Ordinal) && !json.Contains("secret", StringComparison.Ordinal) &&
            !json.Contains("internal-material-id", StringComparison.Ordinal) &&
            RenderHtml(groups).Contains("Flexible Material Testing", StringComparison.Ordinal) &&
            RenderText(groups).Contains("mean 123", StringComparison.Ordinal) && RenderHtml([]).Length == 0;
    }
}
