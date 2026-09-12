using FilamentDbApp.Models;
using System.Globalization;

namespace FilamentDbApp;

public partial class MainWindow
{
    private sealed record FlexibleDetailComparisonRow(
        FlexibleMetricGroupSummary Group, IReadOnlyList<IReadOnlyList<FlexibleMetricGroupSummary>> Values, int PeerCount);

    private static IReadOnlyList<FlexibleDetailComparisonRow> BuildFlexibleDetailComparisonRows(
        IReadOnlyList<(string Label, FlexibleMaterialEvidenceSnapshot? Evidence)> selections)
    {
        var groups = selections.Where(selection => selection.Evidence is not null)
            .SelectMany(selection => selection.Evidence!.Groups)
            .GroupBy(FlexibleComparisonIdentity)
            .Select(group => group.First());
        return groups.Select(group =>
        {
            var values = selections.Select(selection => (IReadOnlyList<FlexibleMetricGroupSummary>)
                (selection.Evidence?.Groups.Where(candidate => FlexibleComparisonIdentity(candidate) == FlexibleComparisonIdentity(group))
                    .ToArray() ?? Array.Empty<FlexibleMetricGroupSummary>())).ToList();
            var peers = selections.Select((selection, index) => (selection.Evidence, Values: values[index]))
                .Where(item => item.Evidence is not null && !string.IsNullOrWhiteSpace(item.Evidence.MaterialId)
                    && item.Values.Any(value => value.Mean is double mean && double.IsFinite(mean) && value.SpecimenCount > 0))
                .Select(item => item.Evidence!.MaterialId.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Count();
            return new FlexibleDetailComparisonRow(group, values, peers);
        }).ToList();
    }

    // Owner comparison policy: specimen metadata does not separate selected materials.
    // Keep distinct saved summaries intact instead of pooling potentially overlapping specimens.
    private static (FlexibleMetricKind Kind, string Metric, string Condition, string Unit) FlexibleComparisonIdentity(
        FlexibleMetricGroupSummary group) => (group.MetricKind, group.Metric, group.Condition, group.Unit);

    private static string FlexibleComparisonValues(IReadOnlyList<FlexibleMetricGroupSummary> groups) =>
        groups.Count == 0 ? "—" : string.Join("\n\n", groups.Select(FlexibleComparisonCell));

    private static string FlexibleComparisonCell(FlexibleMetricGroupSummary? group)
    {
        if (group?.Mean is not double mean || !double.IsFinite(mean) || group.SpecimenCount <= 0)
            return "—";
        return $"{FlexibleEvidenceNumber(mean)} {group.Unit}\nn = {group.SpecimenCount.ToString(CultureInfo.CurrentCulture)}"
            + $"\nSD {FlexibleEvidenceNumber(group.StandardDeviation)} · CV {FlexibleEvidenceNumber(group.CoefficientOfVariation)}%"
            + $"\nRange {FlexibleEvidenceNumber(group.Minimum)}–{FlexibleEvidenceNumber(group.Maximum)}"
            + (group.NotReachedCount > 0 ? $" · {group.NotReachedCount} unreached" : string.Empty);
    }

    private static System.Windows.Controls.StackPanel BuildFlexibleComparisonContent(
        IReadOnlyList<(string Label, FlexibleMaterialEvidenceSnapshot? Evidence)> selections)
    {
        var panel = new System.Windows.Controls.StackPanel();
        var rows = BuildFlexibleDetailComparisonRows(selections);
        if (rows.Count == 0)
        {
            panel.Children.Add(FlexibleDashboardText("No comparable Flexible results in active test sessions."));
            return panel;
        }
        panel.Children.Add(FlexibleDashboardText("Mean · n = independent specimens. Rows align by measurement and condition.", muted: true));
        var table = new System.Windows.Controls.Grid { HorizontalAlignment = System.Windows.HorizontalAlignment.Left };
        foreach (var width in new[] { 300d, 210d }.Concat(selections.Select(_ => 155d)))
            table.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition { Width = new System.Windows.GridLength(width) });
        AddFlexibleDashboardRow(table, new[] { "Condition", "Result" }.Concat(selections.Select(selection => selection.Label)).ToArray(), true);
        foreach (var condition in rows.GroupBy(row => row.Group.Condition, StringComparer.Ordinal))
        {
            var first = true;
            foreach (var row in condition)
            {
                var label = row.Group.Metric.Replace("Compression Force at ", "Force · ", StringComparison.Ordinal)
                    .Replace("Apparent Compressive Stress at ", "Apparent stress · ", StringComparison.Ordinal)
                    .Replace("Force retention at ", "Retention · ", StringComparison.Ordinal)
                    .Replace("Force reduction from 10 s to 30 s at ", "Force reduction · ", StringComparison.Ordinal)
                    .Replace(" Strain", "", StringComparison.Ordinal)
                    .Replace("Residual height loss after recovery", "Residual height loss", StringComparison.Ordinal);
                if (row.PeerCount < 2) label += " †";
                AddFlexibleDashboardRow(table, new[] { first ? condition.Key : "", label }
                    .Concat(row.Values.Select(FlexibleComparisonValues)).ToArray(), false);
                first = false;
            }
        }
        panel.Children.Add(new System.Windows.Controls.ScrollViewer
        {
            HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
            HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left,
            VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Disabled, Content = table
        });
        if (rows.Any(row => row.PeerCount < 2))
            panel.Children.Add(FlexibleDashboardText("† No matching peer among the selected materials. — = no measured result for these conditions.", muted: true));
        else if (rows.Any(row => row.Values.Any(values => values.Count == 0 || values.All(value => value.Mean is null || value.SpecimenCount <= 0))))
            panel.Children.Add(FlexibleDashboardText("— = no measured result for these conditions.", muted: true));
        return panel;
    }

    private static bool RunFlexibleComparisonContractVerification()
    {
        var force = new FlexibleMetricGroupSummary(FlexibleMetricKind.CompressionForce, "Compression Force at 20% Strain",
            "method-1", "10 s", "force-10", 2, 120, 1, 1, 119, 121, "N", 0);
        var later = force with { Condition = "30 s", ComparisonKey = "force-30", Mean = 110 };
        var otherMethod = force with { MethodGroup = "method-2", ComparisonKey = "other-force-10", Mean = 130 };
        var first = new FlexibleMaterialEvidenceSnapshot("MAT-A", new[] { force, later, otherMethod }, 1, 2, 0);
        var second = new FlexibleMaterialEvidenceSnapshot("MAT-B", new[] { force with { Mean = 100, MethodGroup = "different notes and sample dimensions", SpecimenCount = 9 } }, 1, 2, 0);
        var rows = BuildFlexibleDetailComparisonRows(new (string, FlexibleMaterialEvidenceSnapshot?)[]
            { ("A", first), ("B", second), ("C", null) });
        var matched = rows.Single(row => row.Group.ComparisonKey == "force-10");
        var unmatched = rows.Single(row => row.Group.ComparisonKey == "force-30");
        var duplicate = BuildFlexibleDetailComparisonRows(new (string, FlexibleMaterialEvidenceSnapshot?)[]
            { ("A", first), ("B", first with { MaterialId = "mat-a" }) });
        var passed = rows.Count == 2 && matched.PeerCount == 2 && unmatched.PeerCount == 1
            && unmatched.Values[1].Count == 0 && matched.Values[2].Count == 0
            && matched.Values[0].Count == 2 && matched.Values[0][1].Mean == 130
            && duplicate.All(row => row.PeerCount == 1)
            && matched.Values[1].Single().SpecimenCount == 9
            && FlexibleComparisonValues(matched.Values[0]).Contains("130 N", StringComparison.Ordinal)
            && FlexibleComparisonCell(null) == "—"
            && FlexibleComparisonCell(force with { Mean = null }) == "—"
            && FlexibleComparisonCell(force with { Mean = 0 }).StartsWith("0 N", StringComparison.Ordinal)
            && force.Mean == 120 && later.Mean == 110;
        return passed;
    }
}
