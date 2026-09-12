using FilamentDbApp.Models;
using System.Data;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private void RenderSelectedFlexibleHandoffs(DataRow? row)
    {
        var materialId = row is null ? string.Empty : GetCell(row, "Material ID", "MaterialID").Trim();
        _flexibleMaterialEvidence.TryGetValue(materialId, out var evidence);
        var label = row is null ? "Select a material" : _detailService.BuildTitle(row);
        SelectedFlexibleVideoExpander.Header = $"Flexible video brief — {label}";
        SelectedFlexibleRecommendationExpander.Header = $"Flexible guidance — {label}";
        SelectedFlexibleVideoPanel.Children.Clear();
        SelectedFlexibleRecommendationPanel.Children.Clear();
        if (evidence?.HasResults != true)
        {
            var message = row is null ? "Select a material in Materials." : "No comparable Flexible results in active test sessions.";
            SelectedFlexibleVideoPanel.Children.Add(FlexibleDashboardText(message));
            SelectedFlexibleRecommendationPanel.Children.Add(FlexibleDashboardText(message));
            return;
        }
        var scopeIds = GetCanonicalVisibleMaterialRows().Select(item => GetCell(item, "Material ID", "MaterialID").Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var peers = FindFlexibleDetailPeers(evidence, _flexibleMaterialEvidence.Values.Where(item => scopeIds.Contains(item.MaterialId)));
        var peerLabels = peers.Take(5).Select(item => FlexibleMaterialDisplayName(item.MaterialId)).ToList();
        var peersText = peerLabels.Count == 0 ? "No matching comparison material in the current Materials filter."
            : "Matched comparison candidates: " + string.Join("; ", peerLabels) +
              (peers.Count > 5 ? $"; +{peers.Count - 5} more" : string.Empty) + ". Use Compare to inspect each matching condition.";
        SelectedFlexibleVideoPanel.Children.Add(FlexibleDashboardText(BuildFlexibleVideoAngle(label, evidence), bold: true));
        SelectedFlexibleVideoPanel.Children.Add(FlexibleDashboardText(peersText, muted: true));
        SelectedFlexibleVideoPanel.Children.Add(BuildFlexibleDashboardContent(evidence));
        foreach (var guidance in BuildFlexibleUseGuidance(evidence))
            SelectedFlexibleRecommendationPanel.Children.Add(FlexibleDashboardText(guidance));
        SelectedFlexibleRecommendationPanel.Children.Add(FlexibleDashboardText(peersText, muted: true));
        SelectedFlexibleRecommendationPanel.Children.Add(BuildFlexibleDashboardContent(evidence));
    }

    private static IReadOnlyList<FlexibleMaterialEvidenceSnapshot> FindFlexibleDetailPeers(
        FlexibleMaterialEvidenceSnapshot selected, IEnumerable<FlexibleMaterialEvidenceSnapshot> candidates)
    {
        var keys = selected.Groups.Where(group => group.Mean.HasValue).Select(group => group.ComparisonKey).ToHashSet(StringComparer.Ordinal);
        return candidates.Where(candidate => !string.Equals(candidate.MaterialId, selected.MaterialId, StringComparison.OrdinalIgnoreCase) &&
                candidate.Groups.Any(group => group.Mean.HasValue && keys.Contains(group.ComparisonKey)))
            .GroupBy(candidate => candidate.MaterialId, StringComparer.OrdinalIgnoreCase).Select(group => group.First())
            .OrderBy(candidate => candidate.MaterialId, StringComparer.OrdinalIgnoreCase).ToList();
    }

    private static string BuildFlexibleVideoAngle(string label, FlexibleMaterialEvidenceSnapshot evidence)
    {
        var kinds = evidence.Groups.Where(group => group.Mean.HasValue).Select(group => group.MetricKind).ToHashSet();
        var topics = new List<string>();
        if (kinds.Overlaps(new[] { FlexibleMetricKind.CompressionForce, FlexibleMetricKind.ApparentCompressiveStress })) topics.Add("compression load");
        if (kinds.Overlaps(new[] { FlexibleMetricKind.ForceRetention, FlexibleMetricKind.ForceReduction })) topics.Add("force change during the hold");
        if (kinds.Contains(FlexibleMetricKind.ResidualHeightLoss)) topics.Add("height recovery after unloading");
        if (kinds.Overlaps(new[] { FlexibleMetricKind.ShoreA, FlexibleMetricKind.ShoreD })) topics.Add("measured Shore hardness");
        return $"Video angle: {label} — {string.Join(", ", topics)}. Show the recorded conditions and independent sample count with each result.";
    }

    private static IReadOnlyList<string> BuildFlexibleUseGuidance(FlexibleMaterialEvidenceSnapshot evidence)
    {
        var kinds = evidence.Groups.Where(group => group.Mean.HasValue).Select(group => group.MetricKind).ToHashSet();
        var notes = new List<string>();
        if (kinds.Overlaps(new[] { FlexibleMetricKind.CompressionForce, FlexibleMetricKind.ApparentCompressiveStress }))
            notes.Add("Compression load: compare the measured force with the load your part needs at the recorded compression. Higher force means more resistance, not universal superiority.");
        if (kinds.Overlaps(new[] { FlexibleMetricKind.ForceRetention, FlexibleMetricKind.ForceReduction }))
            notes.Add("Sustained compression: retention describes how much force remains over the measured hold interval; compare identical intervals and displacement.");
        if (kinds.Contains(FlexibleMetricKind.ResidualHeightLoss))
            notes.Add("Shape recovery: lower residual height loss means closer return to the initial height after the recorded rest. Match compression, hold, rest and cycle.");
        if (kinds.Overlaps(new[] { FlexibleMetricKind.ShoreA, FlexibleMetricKind.ShoreD }))
            notes.Add("Hardness: choose the measured scale/value for your desired firmness. Shore A and Shore D are separate measurements.");
        notes.Add("These results guide comparison; they do not establish fatigue life or application suitability. Overall remains unchanged.");
        return notes;
    }

    private static bool RunFlexibleDetailHandoffContractVerification()
    {
        var group = new FlexibleMetricGroupSummary(FlexibleMetricKind.ResidualHeightLoss, "Recovery", "method", "60 s rest",
            "same-condition", 1, 1.4, null, null, 1.4, 1.4, "%", 0);
        var selected = new FlexibleMaterialEvidenceSnapshot("A", new[] { group }, 1, 1, 0);
        var match = selected with { MaterialId = "B" };
        var other = selected with { MaterialId = "C", Groups = new[] { group with { ComparisonKey = "other-rest" } } };
        var empty = selected with { MaterialId = "D", Groups = new[] { group with { Mean = null, SpecimenCount = 0 } } };
        var peers = FindFlexibleDetailPeers(selected, new[] { selected, match, match, other, empty });
        return peers.Count == 1 && peers[0].MaterialId == "B" &&
            BuildFlexibleVideoAngle("Sample", selected).Contains("height recovery", StringComparison.Ordinal) &&
            !BuildFlexibleVideoAngle("Sample", selected).Contains("hardness", StringComparison.Ordinal) &&
            BuildFlexibleUseGuidance(selected).Any(note => note.StartsWith("Shape recovery:", StringComparison.Ordinal)) &&
            !BuildFlexibleUseGuidance(selected).Any(note => note.StartsWith("Compression load:", StringComparison.Ordinal));
    }
}
