using FilamentDbApp.Models;
using FilamentDbApp.Services;
using FilamentDbApp.Services.Reporting;
using Microsoft.Win32;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private bool _updatingFlexibleRankings;
    private readonly Dictionary<string, IReadOnlyList<(string Scope, FlexibleRankingGroup Group)>> _flexibleRankingViews = new();

    private void FlexibleRankingCategory_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_updatingFlexibleRankings && IsLoaded && sender is ComboBox box && box.Tag is string surface)
            UpdateFlexibleRankings(surface);
    }

    private (ComboBox? Filter, StackPanel? Panel, ComboBox? Base, ComboBox? Manufacturer, ComboBox? Reinforcement) FlexibleRankingControls(string surface) => surface switch
    {
        "categories" => (FlexibleCategoryCategoryFilter, FlexibleCategoryPanel, CategoryRankingBaseMaterialFilter, CategoryRankingManufacturerFilter, CategoryRankingReinforcementFilter),
        "awards" => (FlexibleAwardCategoryFilter, FlexibleAwardPanel, AwardsBaseMaterialFilter, AwardsManufacturerFilter, AwardsReinforcementFilter),
        _ => (FlexibleRankingCategoryFilter, FlexibleRankingPanel, RankingBaseMaterialFilter, RankingManufacturerFilter, RankingReinforcementFilter)
    };

    private void UpdateFlexibleRankings(string surface)
    {
        var controls = FlexibleRankingControls(surface);
        if (_updatingFlexibleRankings || controls.Filter is null || controls.Panel is null) return;
        _updatingFlexibleRankings = true;
        try
        {
            if (controls.Filter.Items.Count == 0)
            {
                controls.Filter.Items.Add(new ComboBoxItem { Content = "All Flexible categories", Tag = "all" });
                foreach (var category in FlexibleMaterialRankingService.Categories.Where(category => surface != "awards" || category.AllowsAward))
                    controls.Filter.Items.Add(new ComboBoxItem { Content = category.Label, Tag = category.Key });
                controls.Filter.SelectedIndex = 0;
            }
            var categoryKey = (controls.Filter.SelectedItem as ComboBoxItem)?.Tag as string;
            var materialFilter = SelectedComboText(controls.Base!);
            var manufacturerFilter = SelectedComboText(controls.Manufacturer!);
            var reinforcementFilter = SelectedComboText(controls.Reinforcement!);
            var rows = GetCanonicalVisibleMaterialRows().Where(row =>
                FlexibleScopeMatch(GetCell(row, "Base Material"), materialFilter) &&
                FlexibleScopeMatch(GetCell(row, "Manufacturer", "Brand"), manufacturerFilter) &&
                (string.IsNullOrWhiteSpace(reinforcementFilter) || reinforcementFilter == "All" ||
                    ReinforcementFilterMatches(GetCell(row, "Reinforcement"), reinforcementFilter))).ToList();
            var grouping = surface == "categories" ? SelectedComboText(CategoryRankingGroupFilter) : "";
            var groups = BuildFlexibleRankingGroups(rows, categoryKey == "all" ? null : categoryKey, grouping);
            _flexibleRankingViews[surface] = groups;
            controls.Panel.Children.Clear();
            controls.Panel.Children.Add(BuildFlexibleRankingContent(groups, surface == "awards", FlexibleViewLimit(surface)));
        }
        finally { _updatingFlexibleRankings = false; }
    }

    private static bool FlexibleScopeMatch(string value, string? filter) => string.IsNullOrWhiteSpace(filter) || filter == "All" ||
        string.Equals(value, filter, StringComparison.OrdinalIgnoreCase);

    private int? FlexibleViewLimit(string surface) => surface switch
    {
        "rankings" => RankingLimit(),
        "categories" => CategoryRankingLimit(SelectedComboText(CategoryRankingLimitFilter)),
        _ => null
    };

    private IReadOnlyList<(string Scope, FlexibleRankingGroup Group)> BuildFlexibleRankingGroups(
        IReadOnlyList<DataRow> rows, string? categoryKey = null, string? grouping = null)
    {
        var scopes = rows.GroupBy(row => grouping switch
        {
            "Winners by base material" => GetCell(row, "Base Material"),
            "Winners by manufacturer" => GetCell(row, "Manufacturer", "Brand"),
            _ => "Materials in scope"
        });
        return scopes.Where(scope => !string.IsNullOrWhiteSpace(scope.Key) && scope.Key != "—").SelectMany(scope => FlexibleMaterialRankingService.Build(scope.Select(row =>
        {
            var id = GetCell(row, "Material ID", "MaterialID").Trim();
            _flexibleMaterialEvidence.TryGetValue(id, out var evidence);
            return evidence is null ? null : new FlexibleRankingMaterial(id, _detailService.BuildTitle(row), evidence);
        }).OfType<FlexibleRankingMaterial>(), categoryKey).Select(group => (scope.Key, group))).ToList();
    }

    private static StackPanel BuildFlexibleRankingContent(
        IReadOnlyList<(string Scope, FlexibleRankingGroup Group)> groups, bool awardsOnly, int? limit)
    {
        var panel = new StackPanel();
        var eligible = groups.Where(item => !awardsOnly || item.Group.Category.AllowsAward).ToList();
        if (eligible.Count == 0)
        {
            panel.Children.Add(FlexibleDashboardText(awardsOnly ? "No measured Flexible award categories in this scope." : "No saved Flexible results in this scope."));
            return panel;
        }
        foreach (var method in eligible.GroupBy(item => (item.Scope, item.Group.MethodGroup)))
        {
            panel.Children.Add(FlexibleDashboardText(method.Key.Scope, bold: true));
            panel.Children.Add(new Expander { Header = "Test setup", Content = FlexibleDashboardText(method.Key.MethodGroup, muted: true), Margin = new Thickness(0, 3, 0, 6) });
            foreach (var item in method)
            {
                var group = item.Group;
                panel.Children.Add(FlexibleDashboardText((awardsOnly ? group.Category.AwardTitle : group.Metric) + " · " + group.Condition, bold: true));
                panel.Children.Add(FlexibleDashboardText(group.Category.Interpretation + $" {group.ComparableMaterialCount} measured materials.", muted: true));
                if (!group.CanRank)
                    panel.Children.Add(FlexibleDashboardText("Rank / award unavailable: two materials with matching conditions are required.", muted: true));
                var rows = awardsOnly ? group.AwardWinners : group.Rows.Where(row => !limit.HasValue || row.Rank is null || row.Rank <= limit).ToArray();
                if (rows.Count == 0) continue;
                var table = new Grid { HorizontalAlignment = HorizontalAlignment.Left, Margin = new Thickness(0, 3, 0, 12) };
                foreach (var width in new[] { 60d, 320d, 105d, 45d, 85d, 75d, 135d })
                    table.ColumnDefinitions.Add(new ColumnDefinition { Width = new GridLength(width) });
                AddFlexibleDashboardRow(table, new[] { awardsOnly ? "Place" : "Rank", "Material", "Mean (" + group.Unit + ")", "n", "SD", "CV %", "Range" }, true);
                foreach (var row in rows)
                {
                    var summary = row.Summary;
                    AddFlexibleDashboardRow(table, new[] { row.Rank?.ToString(CultureInfo.CurrentCulture) ?? "—", row.Label,
                        FlexibleEvidenceNumber(summary.Mean), summary.SpecimenCount.ToString(CultureInfo.CurrentCulture),
                        FlexibleEvidenceNumber(summary.StandardDeviation), FlexibleEvidenceNumber(summary.CoefficientOfVariation),
                        summary.Minimum.HasValue ? $"{FlexibleEvidenceNumber(summary.Minimum)}–{FlexibleEvidenceNumber(summary.Maximum)}" : "—" }, false);
                }
                panel.Children.Add(table);
            }
        }
        return panel;
    }

    private void ExportFlexibleRankingCsv_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string surface }) return;
        UpdateFlexibleRankings(surface);
        if (!_flexibleRankingViews.TryGetValue(surface, out var groups) || !groups.Any(item =>
                surface == "awards" ? item.Group.AwardWinners.Count > 0 : item.Group.Rows.Count > 0))
        {
            MessageBox.Show(this, "No Flexible rows are available for this export.", "Export Flexible CSV", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }
        var dialog = new SaveFileDialog { Title = "Export Flexible CSV", Filter = "CSV file (*.csv)|*.csv", FileName = "3dp-flexible-" + surface + ".csv" };
        if (dialog.ShowDialog(this) != true) return;
        IOFile.WriteAllText(dialog.FileName, FlexibleRankingExportService.RenderCsv(groups, surface == "awards", FlexibleViewLimit(surface)), System.Text.Encoding.UTF8);
    }
}
