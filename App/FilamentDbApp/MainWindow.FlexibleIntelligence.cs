using FilamentDbApp.Services;
using System.Data;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private bool _refreshingFlexibleOpportunities;
    private sealed record FlexibleOpportunityChoice(string Display, FlexibleMaterialOpportunity Opportunity);
    private (ComboBox? Choice, StackPanel? Panel) FlexibleOpportunityControls(string surface) => surface switch
    {
        "Video" => (FlexibleVideoChoice, FlexibleVideoOpportunityPanel),
        "Recommendation" => (FlexibleRecommendationChoice, FlexibleRecommendationOpportunityPanel),
        _ => (FlexibleResearchChoice, FlexibleResearchOpportunityPanel)
    };

    private IReadOnlyList<FlexibleMaterialOpportunity> FlexibleOpportunities(IReadOnlyList<DataRow> rows) =>
        FlexibleMaterialIntelligenceService.Build(rows.Select(row =>
        {
            var id = GetCell(row, "Material ID", "MaterialID").Trim();
            return _flexibleMaterialEvidence.TryGetValue(id, out var evidence)
                ? new FlexibleRankingMaterial(id, _detailService.BuildTitle(row), evidence) : null;
        }).OfType<FlexibleRankingMaterial>());

    private void UpdateFlexibleOpportunities(string surface)
    {
        var controls = FlexibleOpportunityControls(surface);
        if (_refreshingFlexibleOpportunities || controls.Choice is null || controls.Panel is null) return;
        _refreshingFlexibleOpportunities = true;
        try
        {
            var selected = (controls.Choice.SelectedItem as FlexibleOpportunityChoice)?.Opportunity;
            var source = GetCanonicalVisibleMaterialRows();
            if (surface == "Video")
            {
                var ids = BuildCanonicalVisiblePlannerRows().Where(row =>
                    (VideoNoYoutubeOnlyCheck?.IsChecked != true || !row.HasVideo) &&
                    FlexibleTextFilter(row.MaterialType, SelectedComboText(VideoCategoryFilter)) &&
                    FlexibleScopeMatch(row.Manufacturer, SelectedComboText(VideoManufacturerFilter)) &&
                    FlexibleTextFilter(row.MaterialType, SelectedComboText(VideoBaseMaterialFilter)))
                    .Select(row => row.MaterialId).ToHashSet(StringComparer.OrdinalIgnoreCase);
                source = source.Where(row => ids.Contains(GetCell(row, "Material ID", "MaterialID").Trim())).ToList();
            }
            else if (surface == "Recommendation")
            {
                var ids = BuildCanonicalVisiblePlannerRows().Where(row =>
                    (FlexibleScopeMatch(row.Category, SelectedComboText(RecommendationCategoryFilter)) ||
                     FlexibleTextFilter(row.MaterialType, SelectedComboText(RecommendationCategoryFilter))) &&
                    (FlexibleScopeMatch(row.BaseMaterial, SelectedComboText(RecommendationBaseMaterialFilter)) ||
                     FlexibleTextFilter(row.MaterialType, SelectedComboText(RecommendationBaseMaterialFilter))))
                    .Select(row => row.MaterialId).ToHashSet(StringComparer.OrdinalIgnoreCase);
                source = source.Where(row => ids.Contains(GetCell(row, "Material ID", "MaterialID").Trim())).ToList();
            }
            var choices = FlexibleOpportunities(source).Select(opportunity => new FlexibleOpportunityChoice(
                opportunity.Label + " · " + opportunity.Topic, opportunity)).ToList();
            controls.Choice.ItemsSource = choices;
            controls.Choice.SelectedItem = choices.FirstOrDefault(item => item.Opportunity.MaterialId == selected?.MaterialId &&
                item.Opportunity.ComparisonKey == selected?.ComparisonKey) ?? choices.FirstOrDefault();
            RenderFlexibleOpportunity(surface);
        }
        finally { _refreshingFlexibleOpportunities = false; }
    }

    private static bool FlexibleTextFilter(string value, string? filter) => string.IsNullOrWhiteSpace(filter) || filter == "All" ||
        value.Contains(filter, StringComparison.OrdinalIgnoreCase);

    private void FlexibleOpportunity_Changed(object sender, SelectionChangedEventArgs e)
    {
        if (!_refreshingFlexibleOpportunities && sender is ComboBox { Tag: string surface }) RenderFlexibleOpportunity(surface);
    }

    private void RenderFlexibleOpportunity(string surface)
    {
        var controls = FlexibleOpportunityControls(surface);
        if (controls.Panel is null) return;
        controls.Panel.Children.Clear();
        var opportunity = (controls.Choice?.SelectedItem as FlexibleOpportunityChoice)?.Opportunity;
        controls.Panel.Children.Add(BuildFlexibleOpportunityContent(opportunity));
    }

    private static StackPanel BuildFlexibleOpportunityContent(FlexibleMaterialOpportunity? opportunity)
    {
        var panel = new StackPanel();
        if (opportunity is null)
        {
            panel.Children.Add(FlexibleDashboardText("No measured Flexible opportunities in this scope."));
            return panel;
        }
        panel.Children.Add(FlexibleDashboardText(opportunity.Label + " · " + opportunity.Topic, bold: true));
        panel.Children.Add(FlexibleDashboardText("Method " + opportunity.Result.MethodId[..8], muted: true));
        panel.Children.Add(FlexibleDashboardText(opportunity.MeasuredSummary));
        panel.Children.Add(FlexibleDashboardText(opportunity.Guidance, muted: true));
        foreach (var peer in opportunity.ComparisonCandidates)
            panel.Children.Add(FlexibleDashboardText($"Compare with {peer.Label}: {FlexibleEvidenceNumber(peer.Result.Mean)} {peer.Result.Unit}, n={peer.Result.SpecimenCount}.", muted: true));
        return panel;
    }

    private static string FlexibleOpportunityText(FlexibleMaterialOpportunity opportunity) =>
        opportunity.Label + " — " + opportunity.Topic + "\n" + opportunity.MeasuredSummary + "\n" + opportunity.Guidance +
        "\nMethod " + opportunity.Result.MethodId + "\n" + string.Join("\n", opportunity.ComparisonCandidates.Select(peer =>
            $"Compare with {peer.Label}: {FlexibleEvidenceNumber(peer.Result.Mean)} {peer.Result.Unit}; n={peer.Result.SpecimenCount}."));

    private void CopyFlexibleOpportunity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is Button { Tag: string surface } && FlexibleOpportunityControls(surface).Choice?.SelectedItem is FlexibleOpportunityChoice choice)
            Clipboard.SetText(FlexibleOpportunityText(choice.Opportunity));
    }

    private static VideoPlannerRow FlexibleIdeaSnapshot(FlexibleMaterialOpportunity opportunity) => new()
    {
        MaterialId = opportunity.MaterialId, Label = opportunity.Label, SuggestionCategory = "Flexible measurement",
        SuggestedTitle = opportunity.Label + " — " + opportunity.Topic,
        SuggestedAngle = opportunity.Guidance, TalkingPoints = FlexibleOpportunityText(opportunity),
        DataReason = opportunity.MeasuredSummary, ComparisonIdea = string.Join("; ", opportunity.ComparisonCandidates.Select(peer => peer.Label)),
        Series = "Flexible materials", ProductionStatus = "Idea", Notes = "Saved Flexible measurement snapshot."
    };

    private void SaveFlexibleOpportunity_Click(object sender, RoutedEventArgs e)
    {
        if (sender is not Button { Tag: string surface } || FlexibleOpportunityControls(surface).Choice?.SelectedItem is not FlexibleOpportunityChoice choice) return;
        if (RecommendationVideoIdeaList is DataGrid grid)
        {
            CloseOpenWorkflowComboBoxes(grid);
            if (!grid.CommitEdit(DataGridEditingUnit.Cell, true) || !grid.CommitEdit(DataGridEditingUnit.Row, true))
            {
                MessageBox.Show(this, "Finish or correct the current idea edit before saving another idea.", "Save Flexible idea", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
        }
        var snapshot = FlexibleIdeaSnapshot(choice.Opportunity);
        if (_recommendationVideoIdeas.Any(row => row.MaterialId == snapshot.MaterialId && row.TalkingPoints == snapshot.TalkingPoints)) return;
        var next = new[] { snapshot }.Concat(_recommendationVideoIdeas).ToList();
        try
        {
            _database.SaveVideoIdeas(next.Select(VideoPlannerRowToRecord));
            _recommendationVideoIdeas.Clear();
            _recommendationVideoIdeas.AddRange(next);
            if (RecommendationVideoIdeaList is not null) RecommendationVideoIdeaList.ItemsSource = next;
            UpdateProductionDashboard();
            FlexibleOpportunityControls(surface).Panel?.Children.Add(FlexibleDashboardText("Saved in Video Planner → Video ideas from recommendations.", bold: true));
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, "Could not save the Flexible idea: " + ex.Message, "Save Flexible idea", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private string BuildFlexibleIntelligenceText(IReadOnlyList<DataRow> rows, int? limit = 40)
    {
        var opportunities = FlexibleOpportunities(rows);
        if (opportunities.Count == 0) return "";
        var shown = limit.HasValue ? opportunities.Take(limit.Value).ToList() : opportunities.ToList();
        return "\nFLEXIBLE OPPORTUNITIES\n" + $"Showing {shown.Count} of {opportunities.Count} measured topics in the processed scope. Overall unchanged.\n" +
            string.Join("\n\n", shown.Select(FlexibleOpportunityText));
    }

    private string BuildFlexibleIntelligenceReportHtml(IReadOnlyList<DataRow> rows)
    {
        var text = BuildFlexibleIntelligenceText(rows, null);
        return text.Length == 0 ? "" : "<section><pre style=\"white-space:pre-wrap;overflow-wrap:anywhere;font-family:inherit\">" + Html(text) + "</pre></section>";
    }
    private string BuildFlexibleCoverageText()
    {
        var opportunities = FlexibleOpportunities(GetCanonicalVisibleMaterialRows());
        return $"Flexible: {opportunities.Select(item => item.MaterialId).Distinct(StringComparer.OrdinalIgnoreCase).Count()} measured materials; " +
            $"{opportunities.Count} measured topics; {opportunities.Count(item => item.ComparisonCandidates.Count > 0)} with matching peers. See YouTube Research → Flexible opportunities.";
    }

    private static bool VerifyFlexibleIdeaSnapshot()
    {
        var result = new FilamentDbApp.Models.PublicFlexibleMetricGroup(new string('a', 64), "Force retention at 20% Strain",
            "10–30 s · 2 mm displacement · cycle 1", 3, 97, null, null, 97, 97, "%", 0) { MethodId = new string('b', 64) };
        var opportunity = new FlexibleMaterialOpportunity("FLEX", "Fixture", "Retention", "mean 97%; n 3", "Compare identical conditions",
            "retention", FilamentDbApp.Models.FlexibleMetricKind.ForceRetention, result.GroupId, result, Array.Empty<FlexibleComparisonCandidate>());
        var saved = VideoPlannerRowToRecord(FlexibleIdeaSnapshot(opportunity));
        var changed = opportunity with { MeasuredSummary = "mean 80%; n 3" };
        var loaded = VideoPlannerRowFromRecord(saved);
        return loaded.TalkingPoints.Contains("mean 97%", StringComparison.Ordinal) && !loaded.TalkingPoints.Contains(changed.MeasuredSummary, StringComparison.Ordinal) &&
            loaded.MaterialId == "FLEX" && loaded.OverallScore is null && saved.SuggestionCategory == "Flexible measurement";
    }
}
