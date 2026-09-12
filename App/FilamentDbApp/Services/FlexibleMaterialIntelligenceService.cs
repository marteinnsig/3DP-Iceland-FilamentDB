using FilamentDbApp.Models;
using FilamentDbApp.Services.Reporting;
using System.Globalization;

namespace FilamentDbApp.Services;

/// <summary>Read-only, measured Flexible topics for research, video planning and recommendations.</summary>
public static class FlexibleMaterialIntelligenceService
{
    public static IReadOnlyList<FlexibleMaterialOpportunity> Build(IEnumerable<FlexibleRankingMaterial> materials)
    {
        var opportunities = new List<FlexibleMaterialOpportunity>();
        foreach (var group in FlexibleMaterialRankingService.Build(materials))
        {
            var measured = group.Rows.Where(row => row.Summary.SpecimenCount > 0 &&
                    row.Summary.Mean is double value && double.IsFinite(value))
                .Select(row => new FlexibleComparisonCandidate(row.MaterialId, row.Label,
                    FlexibleReportEvidenceService.Build(new(row.MaterialId, [row.Summary], 0, 0, 0))[0]))
                .OrderBy(row => row.Label, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.MaterialId, StringComparer.OrdinalIgnoreCase).ToArray();
            foreach (var material in measured)
            {
                var peers = measured.Where(peer => !string.Equals(peer.MaterialId, material.MaterialId,
                    StringComparison.OrdinalIgnoreCase)).ToArray();
                var result = material.Result;
                var guidance = Guidance(group.Category.MetricKind) + (peers.Length > 0
                    ? $" {peers.Length} measured comparison candidate(s) share this exact test setup and condition."
                    : " No other measured material shares this exact test setup and condition; comparison is unavailable.");
                opportunities.Add(new(material.MaterialId, material.Label, Topic(group.Category.MetricKind),
                    $"{result.Metric}: mean {N(result.Mean)} {result.Unit}; n = {result.SpecimenCount} independent specimens; " +
                    $"SD {N(result.StandardDeviation)}; CV {N(result.CoefficientOfVariation)}%; " +
                    $"range {N(result.Minimum)}–{N(result.Maximum)}. {result.Condition}." +
                    (result.NotReachedCount > 0 ? $" {result.NotReachedCount} reading(s) did not reach the target." : string.Empty),
                    guidance, group.Category.Key, group.Category.MetricKind, result.GroupId, result, peers));
            }
        }
        return opportunities.OrderBy(item => item.Label, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.MaterialId, StringComparer.OrdinalIgnoreCase)
            .ThenBy(item => item.CategoryKey, StringComparer.Ordinal)
            .ThenBy(item => item.ComparisonKey, StringComparer.Ordinal).ToArray();
    }

    private static string N(double? value) => value is double number && double.IsFinite(number)
        ? number.ToString("0.###", CultureInfo.InvariantCulture) : "—";

    private static string Topic(FlexibleMetricKind kind) => kind switch
    {
        FlexibleMetricKind.CompressionForce => "Force needed to compress the material",
        FlexibleMetricKind.ApparentCompressiveStress => "Apparent stress during compression",
        FlexibleMetricKind.ForceRetention => "Force retained during a compression hold",
        FlexibleMetricKind.ForceReduction => "Force reduction between 10 and 30 seconds",
        FlexibleMetricKind.ResidualHeightLoss => "Height recovery after compression",
        FlexibleMetricKind.ShoreA => "Measured Shore A hardness",
        FlexibleMetricKind.ShoreD => "Measured Shore D hardness",
        _ => "Flexible measurement"
    };

    private static string Guidance(FlexibleMetricKind kind) => kind switch
    {
        FlexibleMetricKind.CompressionForce => "Use the measured load to discuss compression resistance at this displacement; higher force is not universally better.",
        FlexibleMetricKind.ApparentCompressiveStress => "Discuss apparent compressive stress at this strain and specimen geometry; higher stress is not universally better.",
        FlexibleMetricKind.ForceRetention => "Discuss how much initial force remains during the hold; a higher percentage means more retained force under these conditions.",
        FlexibleMetricKind.ForceReduction => "Discuss the force change from 10 to 30 seconds; a lower reduction means less force lost during that interval.",
        FlexibleMetricKind.ResidualHeightLoss => "Discuss recovery after the recorded rest; a lower residual height loss means closer return to the initial height.",
        FlexibleMetricKind.ShoreA => "Discuss the measured Shore A indentation hardness; compare within Shore A only. Harder is not universally better.",
        FlexibleMetricKind.ShoreD => "Discuss the measured Shore D indentation hardness; compare within Shore D only. Harder is not universally better.",
        _ => "Describe the recorded comparative measurement."
    };

    public static bool VerifyContract()
    {
        FlexibleMetricGroupSummary Summary(string key, double? mean, FlexibleMetricKind kind = FlexibleMetricKind.ForceRetention) =>
            new(kind, "Force retention at 20% Strain", "PRIVATE-PRINT-NOTES", "10–30 s · 2 mm displacement · cycle 1",
                key, 3, mean, null, null, mean, mean, "%", 0);
        FlexibleRankingMaterial Material(string id, params FlexibleMetricGroupSummary[] summaries) =>
            new(id, id, new(id, summaries, 1, 3, 0));
        var source = new[]
        {
            Material("A", Summary("PRIVATE-KEY", 0), Summary("other-time", 50) with { Condition = "10–60 s · 2 mm displacement · cycle 1" }),
            Material("A", Summary("PRIVATE-KEY", 0)), Material("B", Summary("PRIVATE-KEY", 0)),
            Material("MISSING", Summary("PRIVATE-KEY", null)), Material("NONFINITE", Summary("PRIVATE-KEY", double.NaN)),
            Material("EMPTY", Summary("PRIVATE-KEY", 5) with { SpecimenCount = 0 }),
            Material("METHOD", Summary("PRIVATE-KEY", 10) with { MethodGroup = "other method" }),
            Material("SHOREA", Summary("shore", 10, FlexibleMetricKind.ShoreA)),
            Material("SHORED", Summary("shore", 10, FlexibleMetricKind.ShoreD)),
            Material("FORCE", Summary("force", 10, FlexibleMetricKind.CompressionForce)),
            Material("INVALID", Summary("invalid", 10) with { Metric = "PRIVATE-METRIC", Condition = "PRIVATE-CONDITION" })
        };
        var result = Build(source);
        var matched = result.Single(item => item.MaterialId == "A" && item.ComparisonCandidates.Count > 0);
        var isolated = result.Single(item => item.MaterialId == "METHOD");
        var narratives = string.Join("\n", result.Select(item => item.Topic + item.MeasuredSummary + item.Guidance +
            item.ComparisonKey + System.Text.Json.JsonSerializer.Serialize(item.Result)));
        return result.Count(item => item.MaterialId == "A") == 2 &&
            matched.ComparisonCandidates.Single().MaterialId == "B" && matched.Result.Mean == 0 &&
            matched.Result.SpecimenCount == 3 && matched.Result.StandardDeviation is null &&
            matched.MeasuredSummary.Contains("mean 0 %", StringComparison.Ordinal) &&
            matched.MeasuredSummary.Contains("SD —", StringComparison.Ordinal) &&
            !result.Any(item => new[] { "MISSING", "NONFINITE", "EMPTY" }.Contains(item.MaterialId)) &&
            isolated.ComparisonCandidates.Count == 0 && isolated.Guidance.Contains("comparison is unavailable", StringComparison.Ordinal) &&
            result.Where(item => item.MaterialId.StartsWith("SHORE", StringComparison.Ordinal)).All(item => item.ComparisonCandidates.Count == 0) &&
            result.Single(item => item.MaterialId == "FORCE").Guidance.Contains("not universally better", StringComparison.Ordinal) &&
            !narratives.Contains("PRIVATE", StringComparison.Ordinal) &&
            System.Text.Json.JsonSerializer.Serialize(Build(source)) == System.Text.Json.JsonSerializer.Serialize(result) &&
            source[0].Evidence.Groups[0].Mean == 0 && source[0].Evidence.Groups[0].MethodGroup == "PRIVATE-PRINT-NOTES";
    }

}

public sealed record FlexibleComparisonCandidate(string MaterialId, string Label, PublicFlexibleMetricGroup Result);

// ComparisonKey is the safe hashed group identity, never the internal freeform method key.
public sealed record FlexibleMaterialOpportunity(string MaterialId, string Label, string Topic, string MeasuredSummary,
    string Guidance, string CategoryKey, FlexibleMetricKind MetricKind, string ComparisonKey, PublicFlexibleMetricGroup Result,
    IReadOnlyList<FlexibleComparisonCandidate> ComparisonCandidates);
