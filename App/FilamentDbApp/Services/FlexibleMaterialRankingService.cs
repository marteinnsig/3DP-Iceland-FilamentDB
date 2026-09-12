using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

/// <summary>Separate, condition-matched Flexible comparisons; never consumes or changes Overall.</summary>
public static class FlexibleMaterialRankingService
{
    public static IReadOnlyList<FlexibleRankingCategory> Categories { get; } = new[]
    {
        new FlexibleRankingCategory("retention", "Force retention", FlexibleMetricKind.ForceRetention, false, true,
            "Higher retained force under the same hold conditions.", "Highest force retention"),
        new FlexibleRankingCategory("force-reduction", "Force reduction", FlexibleMetricKind.ForceReduction, true, true,
            "Lower force reduction from 10 to 30 seconds under the same conditions.", "Lowest force reduction"),
        new FlexibleRankingCategory("recovery", "Recovery residual height loss", FlexibleMetricKind.ResidualHeightLoss, true, true,
            "Lower residual height loss after the same compression, hold and rest.", "Lowest residual height loss"),
        new FlexibleRankingCategory("compression-force", "Compression force", FlexibleMetricKind.CompressionForce, false, false,
            "Higher to lower measured force; a larger value is not universally better.", null),
        new FlexibleRankingCategory("compression-stress", "Apparent compressive stress", FlexibleMetricKind.ApparentCompressiveStress, false, false,
            "Higher to lower measured apparent stress; a larger value is not universally better.", null),
        new FlexibleRankingCategory("shore-a", "Shore A hardness", FlexibleMetricKind.ShoreA, false, false,
            "Higher to lower Shore A hardness; a larger value is not universally better.", null),
        new FlexibleRankingCategory("shore-d", "Shore D hardness", FlexibleMetricKind.ShoreD, false, false,
            "Higher to lower Shore D hardness; a larger value is not universally better.", null)
    };

    public static IReadOnlyList<FlexibleRankingGroup> Build(
        IEnumerable<FlexibleRankingMaterial> materials, string? categoryKey = null)
    {
        var categories = Categories.Where(category => categoryKey is null || category.Key == categoryKey)
            .ToDictionary(category => category.MetricKind);
        var candidates = materials.Where(material => !string.IsNullOrWhiteSpace(material.MaterialId) &&
                string.Equals(material.MaterialId.Trim(), material.Evidence.MaterialId.Trim(), StringComparison.OrdinalIgnoreCase))
            .SelectMany(material => material.Evidence.Groups
                .Where(summary => categories.ContainsKey(summary.MetricKind) && !string.IsNullOrWhiteSpace(summary.ComparisonKey))
                .Select(summary => new FlexibleRankingRow(material.MaterialId.Trim(), material.Label, summary, null)));
        var result = new List<FlexibleRankingGroup>();
        // The full saved method and condition identity remain internal and must not be serialized publicly.
        foreach (var group in candidates.GroupBy(row => new
        {
            row.Summary.ComparisonKey, row.Summary.MetricKind, row.Summary.MethodGroup,
            row.Summary.Condition, row.Summary.Unit
        }))
        {
            var category = categories[group.Key.MetricKind];
            // A repeated UI material cannot inflate the peer count. Conflicting copies have no safe winner.
            var unique = group.GroupBy(row => row.MaterialId, StringComparer.OrdinalIgnoreCase)
                .Where(rows => rows.Select(row => row.Summary).Distinct().Count() == 1)
                .Select(rows => rows.OrderBy(row => row.Label, StringComparer.Ordinal).First()).ToArray();
            var valid = unique.Where(IsMeasured).ToArray();
            var canRank = valid.Length >= 2;
            var ordered = unique.OrderBy(row => IsMeasured(row) ? 0 : 1)
                .ThenBy(row => IsMeasured(row) ? (category.LowerIsBetter ? row.Summary.Mean!.Value : -row.Summary.Mean!.Value) : 0)
                .ThenBy(row => row.Label, StringComparer.OrdinalIgnoreCase)
                .ThenBy(row => row.MaterialId, StringComparer.OrdinalIgnoreCase)
                .Select(row => row with
                {
                    Rank = canRank && IsMeasured(row)
                        ? 1 + valid.Count(peer => category.LowerIsBetter
                            ? peer.Summary.Mean!.Value < row.Summary.Mean!.Value
                            : peer.Summary.Mean!.Value > row.Summary.Mean!.Value)
                        : null
                }).ToArray();
            if (ordered.Length == 0) continue;
            result.Add(new FlexibleRankingGroup(category, group.Key.ComparisonKey, ordered[0].Summary.Metric,
                group.Key.MethodGroup, group.Key.Condition, group.Key.Unit, valid.Length, ordered));
        }
        return result.OrderBy(group => group.Category.Key, StringComparer.Ordinal)
            .ThenBy(group => group.Condition, StringComparer.Ordinal)
            .ThenBy(group => group.ComparisonKey, StringComparer.Ordinal).ToArray();
    }

    private static bool IsMeasured(FlexibleRankingRow row) => row.Summary.SpecimenCount > 0 &&
        row.Summary.Mean is double value && double.IsFinite(value);

    public static bool VerifyContract()
    {
        FlexibleMetricGroupSummary Summary(string key, double? value, int n = 3,
            FlexibleMetricKind kind = FlexibleMetricKind.ResidualHeightLoss) =>
            new(kind, "Fixture metric", "method", "condition", key, n, value, null, null, value, value, "%", 0);
        FlexibleRankingMaterial Material(string id, params FlexibleMetricGroupSummary[] summaries) =>
            new(id, id, new(id, summaries, 1, 3, 0));
        var rows = Build(new[]
        {
            Material("A", Summary("same", 0)), Material("A", Summary("same", 0)),
            Material("B", Summary("same", 0)), Material("C", Summary("same", 2)),
            Material("D", Summary("same", null)), Material("E", Summary("same", double.NaN)),
            Material("F", Summary("same", 1, 0)), Material("ALONE", Summary("other", 1)),
            Material("AMBIG", Summary("same", 1)), Material("AMBIG", Summary("same", 5)),
            new FlexibleRankingMaterial("WRONG", "Wrong", new("OTHER", new[] { Summary("same", -1) }, 1, 3, 0))
        });
        var matched = rows.Single(group => group.ComparisonKey == "same");
        var alone = rows.Single(group => group.ComparisonKey == "other");
        var directional = Build(new[]
        {
            Material("A", Summary("retention", 90, kind: FlexibleMetricKind.ForceRetention),
                Summary("force", 20, kind: FlexibleMetricKind.CompressionForce)),
            Material("B", Summary("retention", 80, kind: FlexibleMetricKind.ForceRetention),
                Summary("force", 30, kind: FlexibleMetricKind.CompressionForce))
        });
        var isolated = Build(new[]
        {
            Material("A", Summary("same", 1)), Material("B", Summary("same", 2) with { MethodGroup = "different" }),
            Material("C", Summary("same", 3) with { Condition = "other time" }),
            Material("D", Summary("same", 4, kind: FlexibleMetricKind.ShoreA)),
            Material("E", Summary("same", 5, kind: FlexibleMetricKind.ShoreD))
        });
        return matched.ComparableMaterialCount == 3 && matched.Rows.Count == 6 && matched.AwardWinners.Count == 2 &&
            matched.Rows.Single(row => row.MaterialId == "A").Rank == 1 &&
            matched.Rows.Single(row => row.MaterialId == "C").Rank == 3 &&
            matched.Rows.Single(row => row.MaterialId == "A").Summary.SpecimenCount == 3 &&
            matched.Rows.Where(row => new[] { "D", "E", "F" }.Contains(row.MaterialId)).All(row => row.Rank is null) &&
            !alone.CanRank && alone.AwardWinners.Count == 0 && alone.Rows.Single().Rank is null &&
            directional.Single(group => group.Category.Key == "retention").AwardWinners.Single().MaterialId == "A" &&
            directional.Single(group => group.Category.Key == "compression-force").Rows[0].MaterialId == "B" &&
            directional.Single(group => group.Category.Key == "compression-force").AwardWinners.Count == 0 &&
            isolated.Count == 5 && isolated.All(group => !group.CanRank) &&
            Build(new[] { Material("A", Summary("same", 1)) }, "unknown").Count == 0;
    }
}

public sealed record FlexibleRankingMaterial(string MaterialId, string Label, FlexibleMaterialEvidenceSnapshot Evidence);
public sealed record FlexibleRankingCategory(string Key, string Label, FlexibleMetricKind MetricKind,
    bool LowerIsBetter, bool AllowsAward, string Interpretation, string? AwardTitle);
public sealed record FlexibleRankingRow(string MaterialId, string Label, FlexibleMetricGroupSummary Summary, int? Rank);
public sealed record FlexibleRankingGroup(FlexibleRankingCategory Category, string ComparisonKey, string Metric,
    string MethodGroup, string Condition, string Unit, int ComparableMaterialCount, IReadOnlyList<FlexibleRankingRow> Rows)
{
    public bool CanRank => ComparableMaterialCount >= 2;
    public IReadOnlyList<FlexibleRankingRow> AwardWinners => CanRank && Category.AllowsAward
        ? Rows.Where(row => row.Rank == 1).ToArray() : Array.Empty<FlexibleRankingRow>();
}
