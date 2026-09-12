using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Text;

namespace FilamentDbApp.Services.Reporting;

/// <summary>Pure exports of already scoped Flexible rankings; never changes scores or persists data.</summary>
public static class FlexibleRankingExportService
{
    public static string RenderCsv(IEnumerable<(string Scope, FlexibleRankingGroup Group)> groups,
        bool awardsOnly = false, int? limit = null)
    {
        var output = new StringBuilder();
        output.AppendLine("Scope,Category,Interpretation,Method ID,Comparison ID,Condition,Metric,Material,Rank,Mean,Unit,n,SD,CV %,Minimum,Maximum,Not reached");
        foreach (var group in Project(groups, awardsOnly, limit))
            foreach (var row in group.Rows)
                output.AppendLine(string.Join(",", new[] { group.Scope, group.Title, group.Interpretation,
                    row.Result.MethodId, row.Result.GroupId, row.Result.Condition, row.Result.Metric, row.Label,
                    Rank(row.Rank), Number(row.Result.Mean), row.Result.Unit, row.Result.SpecimenCount.ToString(CultureInfo.InvariantCulture),
                    Number(row.Result.StandardDeviation), Number(row.Result.CoefficientOfVariation),
                    Number(row.Result.Minimum), Number(row.Result.Maximum), row.Result.NotReachedCount.ToString(CultureInfo.InvariantCulture)
                }.Select(Csv)));
        return output.ToString();
    }

    public static string RenderHtml(IEnumerable<(string Scope, FlexibleRankingGroup Group)> groups,
        bool awardsOnly = false, int? limit = null)
    {
        var output = new StringBuilder();
        foreach (var group in Project(groups, awardsOnly, limit))
        {
            var first = group.Rows[0].Result;
            output.Append("<section class=\"flexible-ranking-export\"><h3>").Append(H(group.Scope)).Append(" · ")
                .Append(H(group.Title)).Append("</h3><p>").Append(H(group.Interpretation))
                .Append("</p><p>").Append(H(first.Metric)).Append(" · ").Append(H(first.Condition))
                .Append(" · Method ").Append(H(first.MethodId[..12])).Append("</p>")
                .Append("<div style=\"max-width:100%;overflow-x:auto\"><table style=\"width:auto;max-width:100%;font-size:0.85em;border-collapse:collapse\"><thead><tr><th style=\"padding:4px 8px;text-align:left\">Material</th><th style=\"padding:4px 8px;text-align:left\">Rank</th><th style=\"padding:4px 8px;text-align:left\">Mean</th><th style=\"padding:4px 8px;text-align:left\">Unit</th><th style=\"padding:4px 8px;text-align:left\">n</th><th style=\"padding:4px 8px;text-align:left\">SD</th><th style=\"padding:4px 8px;text-align:left\">CV %</th><th style=\"padding:4px 8px;text-align:left\">Range</th><th style=\"padding:4px 8px;text-align:left\">Not reached</th></tr></thead><tbody>");
            foreach (var row in group.Rows)
            {
                var values = new[] { row.Label, Rank(row.Rank), Number(row.Result.Mean), row.Result.Unit,
                    row.Result.SpecimenCount.ToString(CultureInfo.InvariantCulture), Number(row.Result.StandardDeviation),
                    Number(row.Result.CoefficientOfVariation), Range(row.Result), row.Result.NotReachedCount.ToString(CultureInfo.InvariantCulture) };
                output.Append("<tr style=\"break-inside:avoid;page-break-inside:avoid\">");
                foreach (var value in values) output.Append("<td style=\"padding:4px 8px;max-width:22em;overflow-wrap:anywhere\">").Append(H(Display(value))).Append("</td>");
                output.Append("</tr>");
            }
            output.Append("</tbody></table></div></section>");
        }
        return output.ToString();
    }

    public static string RenderText(IEnumerable<(string Scope, FlexibleRankingGroup Group)> groups,
        bool awardsOnly = false, int? limit = null)
    {
        var output = new StringBuilder();
        foreach (var group in Project(groups, awardsOnly, limit))
        {
            var first = group.Rows[0].Result;
            output.AppendLine($"{group.Scope} · {group.Title}").AppendLine(group.Interpretation)
                .AppendLine($"{first.Metric}; {first.Condition}; method {first.MethodId[..12]}");
            foreach (var row in group.Rows)
                output.AppendLine($"{row.Label}: rank {Display(Rank(row.Rank))}; mean {Display(Number(row.Result.Mean))} {row.Result.Unit}; n {row.Result.SpecimenCount}; SD {Display(Number(row.Result.StandardDeviation))}; CV {Display(Number(row.Result.CoefficientOfVariation))}%; range {Range(row.Result)}; not reached {row.Result.NotReachedCount}.");
            output.AppendLine();
        }
        return output.ToString();
    }

    private static IReadOnlyList<ExportGroup> Project(IEnumerable<(string Scope, FlexibleRankingGroup Group)> groups,
        bool awardsOnly, int? limit)
    {
        var output = new List<ExportGroup>();
        foreach (var (scope, group) in groups)
        {
            var category = FlexibleMaterialRankingService.Categories.SingleOrDefault(value => value.Key == group.Category.Key);
            if (category is null || (awardsOnly && (!category.AllowsAward || !group.CanRank))) continue;
            var selected = (awardsOnly ? group.AwardWinners : group.Rows)
                .Where(row => !limit.HasValue || (limit > 0 && (!row.Rank.HasValue || row.Rank <= limit)));
            var rows = selected.Select(row => new ExportRow(row.Label, row.Rank,
                FlexibleReportEvidenceService.Build(new(row.MaterialId, [row.Summary], 0, 0, 0))[0])).ToArray();
            if (rows.Length == 0) continue;
            output.Add(new(scope, awardsOnly ? category.AwardTitle ?? category.Label : category.Label,
                category.Interpretation + " n = independent specimens. Overall unchanged.", rows));
        }
        return output;
    }

    private static string Number(double? value) => value is double number && double.IsFinite(number)
        ? number.ToString("0.###", CultureInfo.InvariantCulture) : string.Empty;
    private static string Rank(int? rank) => rank?.ToString(CultureInfo.InvariantCulture) ?? string.Empty;
    private static string Display(string value) => value.Length == 0 ? "—" : value;
    private static string Range(PublicFlexibleMetricGroup row) => $"{Display(Number(row.Minimum))}–{Display(Number(row.Maximum))}";
    private static string H(string value) => WebUtility.HtmlEncode(value);
    private static string Csv(string value)
    {
        // Material/scope labels are user data; never turn them into spreadsheet formula cells.
        var trimmed = value.TrimStart();
        if (trimmed.Length > 0 && "=+@".Contains(trimmed[0])) value = "'" + value;
        else if (trimmed.StartsWith('-') && !double.TryParse(trimmed, NumberStyles.Float, CultureInfo.InvariantCulture, out _)) value = "'" + value;
        return "\"" + value.Replace("\"", "\"\"") + "\"";
    }
    private sealed record ExportRow(string Label, int? Rank, PublicFlexibleMetricGroup Result);
    private sealed record ExportGroup(string Scope, string Title, string Interpretation, IReadOnlyList<ExportRow> Rows);

    public static bool VerifyContract()
    {
        FlexibleRankingMaterial Material(string id, double? mean, string key = "PRIVATE-COMPARISON",
            FlexibleMetricKind kind = FlexibleMetricKind.ResidualHeightLoss) => new(id, id,
            new(id, [new(kind, "Residual height loss after recovery", "PRIVATE-METHOD-NOTES",
                "20% compression · 30 s compressed hold · 60 s rest · 10 mm initial height · cycle 1",
                key, mean.HasValue ? 3 : 0, mean, null, null, mean, mean, "%", 0)], 1, 3, 0));
        var ranking = FlexibleMaterialRankingService.Build([Material("TIE-A", 0), Material("TIE-B", 0),
            Material("THIRD", 2), Material("MISSING", null), Material("ALONE", 1, "private-other"),
            Material("FORCE-A", 1, "force", FlexibleMetricKind.CompressionForce),
            Material("FORCE-B", 2, "force", FlexibleMetricKind.CompressionForce)]);
        var scoped = ranking.Select(group => ("TPU scope", group)).ToArray();
        var csv = RenderCsv(scoped);
        var text = RenderText(scoped);
        var html = RenderHtml(scoped);
        var winners = RenderCsv(scoped, awardsOnly: true, limit: 1);
        var missing = csv.Split('\n').Single(line => line.Contains("MISSING", StringComparison.Ordinal));
        return csv.Contains("\"TPU scope\"", StringComparison.Ordinal) &&
            !csv.Contains("PRIVATE", StringComparison.Ordinal) && !text.Contains("PRIVATE", StringComparison.Ordinal) &&
            !html.Contains("PRIVATE", StringComparison.Ordinal) &&
            missing.Contains("\"MISSING\",\"\",\"\"", StringComparison.Ordinal) &&
            csv.Contains("\"TIE-A\",\"1\",\"0\"", StringComparison.Ordinal) &&
            winners.Contains("TIE-A", StringComparison.Ordinal) && winners.Contains("TIE-B", StringComparison.Ordinal) &&
            !winners.Contains("THIRD", StringComparison.Ordinal) && !winners.Contains("ALONE", StringComparison.Ordinal) &&
            !winners.Contains("FORCE-", StringComparison.Ordinal) &&
            text.Contains("a larger value is not universally better", StringComparison.Ordinal) &&
            RenderCsv(scoped, limit: 25).Contains("ALONE", StringComparison.Ordinal) &&
            RenderCsv(scoped, limit: 25).Contains("MISSING", StringComparison.Ordinal) &&
            RenderHtml(scoped, limit: 0).Length == 0 && RenderText([], awardsOnly: true).Length == 0 &&
            Csv("=HYPERLINK(\"example\")").StartsWith("\"'=", StringComparison.Ordinal);
    }
}
