using FilamentDbApp.Models;
using FilamentDbApp.Services;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace FilamentDbApp;

public partial class MainWindow
{
    private static IReadOnlyList<(string Scope, FlexibleRankingGroup Group)> BuildFlexibleAnalyticsGroups(
        IEnumerable<(string Scope, FlexibleRankingMaterial Material)> materials) => materials
        .GroupBy(item => item.Scope, StringComparer.OrdinalIgnoreCase)
        .OrderBy(group => group.Key, StringComparer.CurrentCultureIgnoreCase)
        .SelectMany(scope => FlexibleMaterialRankingService.Build(scope.Select(item => item.Material))
            .Select(group => (scope.Key, group))).ToArray();

    private void UpdateFlexibleAnalytics(IReadOnlyList<DataRow> rows, string mode)
    {
        if (FlexibleAnalyticsPanel is null) return;
        var materials = rows.Select(row =>
        {
            var id = GetCell(row, "Material ID", "MaterialID").Trim();
            _flexibleMaterialEvidence.TryGetValue(id, out var evidence);
            var scope = mode == "samples" ? "Materials in scope" : GroupKeyForMode(new AnalyticsMaterialScore
            {
                Label = _detailService.BuildTitle(row),
                Manufacturer = DataTableHelpers.FirstValue(row, "Manufacturer", "Brand") ?? "",
                ProductLine = DataTableHelpers.FirstValue(row, "Product Line", "ProductLine") ?? "",
                BaseMaterial = DataTableHelpers.FirstValue(row, "Base Material", "Type", "Material Type") ?? "",
                Variant = DataTableHelpers.FirstValue(row, "Variant / Finish", "Variant", "Finish") ?? "",
                Reinforcement = DataTableHelpers.FirstValue(row, "Reinforcement", "Reinforment") ?? ""
            }, mode);
            return (Scope: scope, Material: evidence is null ? null : new FlexibleRankingMaterial(id, _detailService.BuildTitle(row), evidence));
        }).Where(item => item.Material is not null).Select(item => (item.Scope, item.Material!));
        FlexibleAnalyticsPanel.Children.Clear();
        FlexibleAnalyticsPanel.Children.Add(BuildFlexibleAnalyticsContent(BuildFlexibleAnalyticsGroups(materials)));
    }

    private static (double Min, double Max) FlexibleAnalyticsAxis(FlexibleRankingGroup group)
    {
        var values = group.Rows.Where(row => row.Summary.SpecimenCount > 0 &&
            row.Summary.Mean is double value && double.IsFinite(value)).Select(row => row.Summary.Mean!.Value).ToArray();
        return values.Length == 0 ? (0, 0) : (Math.Min(0, values.Min()), Math.Max(0, values.Max()));
    }

    private static StackPanel BuildFlexibleAnalyticsContent(IReadOnlyList<(string Scope, FlexibleRankingGroup Group)> groups)
    {
        var panel = new StackPanel();
        if (groups.Count == 0)
        {
            panel.Children.Add(FlexibleDashboardText("No saved Flexible results in the visible Materials scope."));
            return panel;
        }
        panel.Children.Add(FlexibleDashboardText("Chart Mode groups materials; each row keeps its own mean and specimen statistics. Bar scales apply only within each test condition.", muted: true));
        foreach (var scope in groups.GroupBy(item => (item.Scope, item.Group.MethodGroup)))
        {
            panel.Children.Add(FlexibleDashboardText(scope.Key.Scope, bold: true));
            panel.Children.Add(new Expander { Header = "Test setup", Content = FlexibleDashboardText(scope.Key.MethodGroup, muted: true), Margin = new Thickness(0, 2, 0, 6) });
            foreach (var item in scope)
            {
                var group = item.Group;
                var axis = FlexibleAnalyticsAxis(group);
                panel.Children.Add(FlexibleDashboardText(group.Metric + " · " + group.Condition, bold: true));
                panel.Children.Add(FlexibleDashboardText(group.ComparableMaterialCount == 0 ? "No measured values for this condition."
                    : $"{group.ComparableMaterialCount} measured materials · bar axis {FlexibleEvidenceNumber(axis.Min)}–{FlexibleEvidenceNumber(axis.Max)} {group.Unit}", muted: true));
                var table = new Grid { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 2, 0, 12) };
                foreach (var width in new[] { 300d, 190d, 100d, 45d, 75d, 75d, 120d })
                    table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width) });
                AddFlexibleDashboardRow(table, new[] { "Material", "Mean · " + group.Unit, "Value", "n", "SD", "CV %", "Range" }, true);
                foreach (var row in group.Rows.OrderBy(row => row.Label, StringComparer.CurrentCultureIgnoreCase))
                {
                    var value = row.Summary;
                    var measured = value.SpecimenCount > 0 && value.Mean is double mean && double.IsFinite(mean);
                    AddFlexibleDashboardRow(table, new[] { row.Label + (value.NotReachedCount > 0 ? $" · {value.NotReachedCount} unreached" : ""), "", measured ? FlexibleEvidenceNumber(value.Mean) : "—",
                        value.SpecimenCount.ToString(CultureInfo.CurrentCulture), FlexibleEvidenceNumber(value.StandardDeviation),
                        FlexibleEvidenceNumber(value.CoefficientOfVariation), value.Minimum.HasValue
                            ? $"{FlexibleEvidenceNumber(value.Minimum)}–{FlexibleEvidenceNumber(value.Maximum)}" : "—" }, false);
                    if (!measured) continue;
                    const double width = 180;
                    var span = axis.Max - axis.Min;
                    // Normalize only the drawing coordinates, never the measured result or its units.
                    var zero = span > 0 ? -axis.Min / span * width : 0;
                    var end = span > 0 ? (value.Mean!.Value - axis.Min) / span * width : zero;
                    var chart = new Canvas { Width = width, Height = 22, Margin = new Thickness(4), ToolTip = FlexibleEvidenceNumber(value.Mean) + " " + group.Unit };
                    chart.Children.Add(new System.Windows.Shapes.Line { X1 = zero, X2 = zero, Y1 = 0, Y2 = 22, Stroke = Brushes.SlateGray, StrokeThickness = 1 });
                    var bar = new System.Windows.Shapes.Rectangle { Width = Math.Abs(end - zero), Height = 12, Fill = Brushes.SteelBlue };
                    Canvas.SetLeft(bar, Math.Min(zero, end)); Canvas.SetTop(bar, 5); chart.Children.Add(bar);
                    Grid.SetColumn(chart, 1); Grid.SetRow(chart, table.RowDefinitions.Count - 1); table.Children.Add(chart);
                }
                panel.Children.Add(new ScrollViewer { Content = table, HorizontalScrollBarVisibility = ScrollBarVisibility.Auto,
                    VerticalScrollBarVisibility = ScrollBarVisibility.Disabled, HorizontalContentAlignment = HorizontalAlignment.Left });
            }
        }
        return panel;
    }

    private static bool VerifyFlexibleAnalyticsContract()
    {
        FlexibleMetricGroupSummary Summary(string key, double? mean, int n = 2) => new(FlexibleMetricKind.CompressionForce,
            "Force", "method", key, key, n, mean, 1, 2, mean, mean, "N", 0);
        FlexibleRankingMaterial Material(string id, params FlexibleMetricGroupSummary[] groups) => new(id, id, new(id, groups, 1, 2, 0));
        var a = Material("A", Summary("10s", 0), Summary("30s", 4));
        var groups = BuildFlexibleAnalyticsGroups(new[] { ("Maker A", a), ("Maker A", a),
            ("Maker A", Material("B", Summary("10s", 10))), ("Maker B", Material("C", Summary("10s", 20))),
            ("Maker A", Material("D", Summary("10s", null))), ("Maker A", Material("E", Summary("10s", 30, 0))),
            ("Maker A", Material("F", Summary("other", -2))) });
        var first = groups.Single(item => item.Scope == "Maker A" && item.Group.ComparisonKey == "10s").Group;
        return groups.Count == 4 && first.ComparableMaterialCount == 2 && first.Rows.Count == 4
            && FlexibleAnalyticsAxis(first) == (0d, 10d)
            && FlexibleAnalyticsAxis(groups.Single(item => item.Group.ComparisonKey == "other").Group) == (-2d, 0d)
            && BuildFlexibleAnalyticsGroups(Array.Empty<(string, FlexibleRankingMaterial)>()).Count == 0
            && first.Rows.Single(row => row.MaterialId == "A").Summary.Mean == 0
            && a.Evidence.Groups[0].SpecimenCount == 2;
    }
}
