using FilamentDbApp.Models;
using FilamentDbApp.Services;
using System.Globalization;

namespace FilamentDbApp;

public partial class MainWindow
{
    // Detached summaries are rebuilt only from successfully persisted inputs. Invalid/unfinished
    // edits must never leak through navigation into material details or later downstream consumers.
    private IReadOnlyDictionary<string, FlexibleMaterialEvidenceSnapshot> _flexibleMaterialEvidence =
        new Dictionary<string, FlexibleMaterialEvidenceSnapshot>(StringComparer.OrdinalIgnoreCase);

    private void RefreshSavedFlexibleMaterialEvidence()
    {
        var graph = _database.LoadFlexibleTestingGraph();
        _flexibleMaterialEvidence = graph.Sessions.Select(session => session.MaterialID.Trim())
            .Where(id => id.Length > 0).Distinct(StringComparer.OrdinalIgnoreCase)
            .ToDictionary(id => id, id => FlexibleMaterialEvidenceService.Build(id,
                graph.Sessions, graph.Specimens, graph.Compression, graph.Relaxation,
                graph.Recovery, graph.Shore), StringComparer.OrdinalIgnoreCase);
    }

    private MaterialDetailGroup BuildFlexibleMaterialDetailGroup(string materialId)
    {
        _flexibleMaterialEvidence.TryGetValue(materialId, out var evidence);
        return BuildFlexibleMaterialDetailGroup(evidence);
    }

    private static MaterialDetailGroup BuildFlexibleMaterialDetailGroup(FlexibleMaterialEvidenceSnapshot? evidence)
    {
        var fields = new List<MaterialDetailField>();
        fields.Add(new("Results", evidence?.HasResults == true
            ? $"{evidence.Groups.Count} result groups · {evidence.SpecimenCount} specimens · {evidence.SessionCount} active sessions"
            : "No comparable Flexible results in active test sessions."));
        if (evidence?.Groups.Count > 0)
            fields.Add(new("View results", "Mechanical → Engineering Dashboard → Flexible Material Testing"));
        return new("Flexible Material Testing", fields);
    }

    private void RenderFlexibleEngineeringDashboard(string? materialId)
    {
        _flexibleMaterialEvidence.TryGetValue(materialId ?? string.Empty, out var evidence);
        DashboardFlexiblePanel.Children.Clear();
        DashboardFlexiblePanel.Children.Add(BuildFlexibleDashboardContent(evidence));
    }

    private static System.Windows.Controls.StackPanel BuildFlexibleDashboardContent(FlexibleMaterialEvidenceSnapshot? evidence)
    {
        var panel = new System.Windows.Controls.StackPanel();
        if (evidence is null || evidence.Groups.Count == 0)
        {
            panel.Children.Add(FlexibleDashboardText("No comparable Flexible results in active test sessions."));
            return panel;
        }
        panel.Children.Add(FlexibleDashboardText(
            $"{evidence.SessionCount} active sessions · {evidence.SpecimenCount} specimens · n = independent specimens", muted: true));
        var methods = evidence.Groups.GroupBy(group => group.MethodGroup, StringComparer.Ordinal).ToList();
        for (var methodIndex = 0; methodIndex < methods.Count; methodIndex++)
        {
            var method = methods[methodIndex];
            var setup = new System.Windows.Controls.Expander
            {
                Header = methods.Count == 1 ? "Test setup" : $"Test setup {methodIndex + 1}",
                IsExpanded = false, Margin = new System.Windows.Thickness(0, 10, 0, 4),
                Content = FlexibleDashboardText(method.Key, muted: true)
            };
            System.Windows.Automation.AutomationProperties.SetAutomationId(setup, $"FlexibleDashboardSetup{methodIndex + 1}");
            panel.Children.Add(setup);
            var table = new System.Windows.Controls.Grid { HorizontalAlignment = System.Windows.HorizontalAlignment.Left };
            foreach (var width in new[] { 330d, 220d, 95d, 45d, 80d, 65d, 115d })
                table.ColumnDefinitions.Add(new System.Windows.Controls.ColumnDefinition
                {
                    Width = new System.Windows.GridLength(width)
                });
            AddFlexibleDashboardRow(table, new[] { "Condition", "Result", "Mean", "n", "SD", "CV %", "Range" }, true);
            foreach (var condition in method.GroupBy(group => group.Condition, StringComparer.Ordinal))
            {
                var first = true;
                foreach (var group in condition)
                {
                    var label = group.Metric.Replace("Compression Force at ", "Force · ", StringComparison.Ordinal)
                        .Replace("Apparent Compressive Stress at ", "Apparent stress · ", StringComparison.Ordinal)
                        .Replace("Force retention at ", "Retention · ", StringComparison.Ordinal)
                        .Replace("Force reduction from 10 s to 30 s at ", "Force reduction · ", StringComparison.Ordinal)
                        .Replace(" Strain", "", StringComparison.Ordinal)
                        .Replace("Residual height loss after recovery", "Residual height loss", StringComparison.Ordinal);
                    if (group.NotReachedCount > 0) label += $" · {group.NotReachedCount} unreached reading(s)";
                    AddFlexibleDashboardRow(table, new[]
                    {
                        first ? condition.Key : "", $"{label} ({group.Unit})", FlexibleEvidenceNumber(group.Mean),
                        group.SpecimenCount.ToString(CultureInfo.CurrentCulture), FlexibleEvidenceNumber(group.StandardDeviation),
                        FlexibleEvidenceNumber(group.CoefficientOfVariation),
                        group.Minimum.HasValue ? $"{FlexibleEvidenceNumber(group.Minimum)}–{FlexibleEvidenceNumber(group.Maximum)}" : "—"
                    }, false);
                    first = false;
                }
            }
            panel.Children.Add(new System.Windows.Controls.ScrollViewer
            {
                HorizontalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Auto,
                HorizontalContentAlignment = System.Windows.HorizontalAlignment.Left,
                VerticalScrollBarVisibility = System.Windows.Controls.ScrollBarVisibility.Disabled, Content = table
            });
        }
        if (evidence.LegacyRelaxationPointCount > 0)
            panel.Children.Add(FlexibleDashboardText($"Historical relaxation: {evidence.LegacyRelaxationPointCount} readings in Saved Stress Relaxation.", muted: true));
        return panel;
    }

    private static void AddFlexibleDashboardRow(System.Windows.Controls.Grid table, string[] values, bool header)
    {
        var rowIndex = table.RowDefinitions.Count;
        table.RowDefinitions.Add(new System.Windows.Controls.RowDefinition { Height = System.Windows.GridLength.Auto });
        for (var column = 0; column < values.Length; column++)
        {
            var text = FlexibleDashboardText(values[column], bold: header, muted: header);
            text.Margin = new System.Windows.Thickness(0);
            var cell = new System.Windows.Controls.Border
            {
                Padding = new System.Windows.Thickness(6),
                Background = header ? System.Windows.Media.Brushes.AliceBlue : System.Windows.Media.Brushes.Transparent,
                BorderBrush = System.Windows.Media.Brushes.LightGray,
                BorderThickness = new System.Windows.Thickness(0, 0, 0, 0.5), Child = text
            };
            System.Windows.Controls.Grid.SetRow(cell, rowIndex);
            System.Windows.Controls.Grid.SetColumn(cell, column);
            table.Children.Add(cell);
        }
    }

    private static System.Windows.Controls.TextBlock FlexibleDashboardText(string text, bool bold = false, bool muted = false) => new()
    {
        Text = text, TextWrapping = System.Windows.TextWrapping.Wrap,
        FontWeight = bold ? System.Windows.FontWeights.SemiBold : System.Windows.FontWeights.Normal,
        Foreground = muted ? System.Windows.Media.Brushes.SlateGray : System.Windows.Media.Brushes.Black,
        Margin = new System.Windows.Thickness(0, 3, 0, 3)
    };

    private static string FlexibleEvidenceNumber(double? value) =>
        value?.ToString("0.###", CultureInfo.CurrentCulture) ?? "—";
}