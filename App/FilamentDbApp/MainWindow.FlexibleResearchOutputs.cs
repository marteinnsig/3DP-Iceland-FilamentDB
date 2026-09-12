using FilamentDbApp.Models;
using FilamentDbApp.Services;

namespace FilamentDbApp;

public partial class MainWindow
{
    // Read-only planning candidates; priorities describe editorial readiness, never material performance.
    private static List<VideoPlannerRow> BuildFlexibleResearchCandidates(
        IReadOnlyList<FlexibleMaterialOpportunity> opportunities, IReadOnlyList<VideoPlannerRow> metadata) => opportunities
        .GroupBy(item => item.MaterialId, StringComparer.OrdinalIgnoreCase)
        .Select(group => group.OrderByDescending(item => item.ComparisonCandidates.Count > 0)
            .ThenBy(item => item.CategoryKey, StringComparer.Ordinal).ThenBy(item => item.ComparisonKey, StringComparer.Ordinal).First())
        .Select(item => (Item: item, Source: metadata.FirstOrDefault(row => string.Equals(row.MaterialId, item.MaterialId, StringComparison.OrdinalIgnoreCase))))
        .Where(pair => pair.Source is not null).Select(pair =>
        {
            var item = pair.Item;
            var source = pair.Source!;
            var facts = FlexibleOpportunityText(item);
            var title = item.Label + " · " + item.Topic;
            return new VideoPlannerRow
            {
                MaterialId = item.MaterialId, Label = item.Label, Manufacturer = source.Manufacturer,
                MaterialType = source.MaterialType, BaseMaterial = source.BaseMaterial, Category = source.Category,
                ProductLine = source.ProductLine, Reinforcement = source.Reinforcement, Variant = source.Variant,
                HasVideo = source.HasVideo, HasFlexible = true, HasMechanical = false,
                Status = "Flexible ready · " + (source.HasVideo ? "Has video" : "No video"),
                Priority = source.HasVideo ? 40 : 70, ThumbnailScore = source.HasVideo ? 40 : 70,
                ThumbnailGrade = "Measured", ThumbnailPattern = "Measured result",
                ThumbnailMainText = item.Topic, SuggestedTitle = title, SuggestedAngle = item.Guidance,
                SuggestionCategory = "Flexible measurement", TalkingPoints = facts, DataReason = facts,
                Standout = item.MeasuredSummary, ThumbnailRecommendationReason = "Editorial priority from saved measurements and video coverage; not a material score.",
                ThumbnailVisualLayout = "Material sample beside the measured result and its unit; include the test condition.",
                ThumbnailRiskNote = "Use the recorded condition and units. Do not imply a universal material winner.",
                ThumbnailPrompt = title + "\n" + facts,
                ComparisonIdea = string.Join("; ", item.ComparisonCandidates.Select(peer => peer.Label)),
                Notes = "Live candidate; use Save Flexible idea to preserve a chosen topic snapshot."
            };
        }).OrderByDescending(row => row.Priority).ThenBy(row => row.Label, StringComparer.CurrentCultureIgnoreCase).ToList();

    private static List<ComparisonDiscoveryRow> BuildFlexibleDiscovery(IReadOnlyList<FlexibleMaterialOpportunity> opportunities)
    {
        var seen = new HashSet<(string Group, string First, string Second)>();
        var rows = new List<ComparisonDiscoveryRow>();
        foreach (var item in opportunities)
        foreach (var peer in item.ComparisonCandidates)
        {
            var ids = new[] { item.MaterialId.ToUpperInvariant(), peer.MaterialId.ToUpperInvariant() }.OrderBy(id => id, StringComparer.Ordinal).ToArray();
            if (!seen.Add((item.ComparisonKey, ids[0], ids[1]))) continue;
            var title = item.Label + " vs " + peer.Label + " · " + item.Topic;
            var facts = FlexibleOpportunityText(item);
            rows.Add(new ComparisonDiscoveryRow { Score = 60, ComparisonType = "Flexible · matching conditions",
                VideoAngle = title, MaterialA = item.Label, MaterialB = peer.Label, ThumbnailText = item.Topic,
                Reason = "Measured pair; editorial priority only. " + facts, CopyBlock = title + "\n" + facts });
        }
        return rows;
    }

    private static void AppendFlexibleResearchOutputs(IReadOnlyList<FlexibleMaterialOpportunity> opportunities,
        IReadOnlyList<VideoPlannerRow> flexible, List<ComparisonDiscoveryRow> comparisons,
        List<ChannelGapRow> gaps, List<PlaylistDiscoveryRow> playlists)
    {
        comparisons.AddRange(BuildFlexibleDiscovery(opportunities));
        foreach (var row in flexible.Where(row => !row.HasVideo))
            gaps.Add(new ChannelGapRow { GapScore = 70, GapType = "Flexible measurement without video",
                Material = row.Label, Manufacturer = row.Manufacturer, MaterialFamily = row.BaseMaterial,
                VideoIdea = row.SuggestedTitle, ThumbnailText = row.ThumbnailMainText, Coverage = "No recorded material video",
                Reason = row.DataReason, CopyBlock = row.SuggestedTitle + "\n" + row.DataReason });
        if (flexible.Count == 0) return;
        var available = flexible.Where(row => row.HasVideo).Select(row => row.Label).ToArray();
        var missing = flexible.Where(row => !row.HasVideo).Select(row => row.SuggestedTitle).ToArray();
        var facts = string.Join("\n\n", flexible.Select(row => row.SuggestedTitle + "\n" + row.DataReason));
        playlists.Add(new PlaylistDiscoveryRow { Score = 60, PlaylistName = "Flexible materials — measured results",
            PlaylistType = "Flexible measurement", Coverage = $"{available.Length} with video / {flexible.Count} measured materials",
            Videos = string.Join("; ", available), MissingVideos = string.Join("; ", missing),
            Reason = "Separate measured topics; matching conditions are required for comparisons. Editorial priority is not a material score.",
            CopyBlock = "Flexible materials — measured results\n" + facts });
    }

    private static bool VerifyFlexibleResearchOutputs()
    {
        var summary = new FlexibleMetricGroupSummary(FlexibleMetricKind.CompressionForce, "Compression Force at 20% Strain",
            "test setup", "30 s hold · 2 mm displacement · cycle 1", "force-30", 9, 12, 1, 2, 11, 13, "N", 0);
        var opportunities = FlexibleMaterialIntelligenceService.Build(new[]
        {
            new FlexibleRankingMaterial("A", "Material A", new("A", new[] { summary }, 1, 9, 0)),
            new FlexibleRankingMaterial("B", "Material B", new("B", new[] { summary with { Mean = 15 } }, 1, 9, 0))
        });
        var metadata = new[] { new VideoPlannerRow { MaterialId = "A", Manufacturer = "Maker A", BaseMaterial = "TPU" },
            new VideoPlannerRow { MaterialId = "B", Manufacturer = "Maker B", BaseMaterial = "TPE", HasVideo = true } };
        var candidates = BuildFlexibleResearchCandidates(opportunities, metadata);
        var comparisons = new List<ComparisonDiscoveryRow>(); var gaps = new List<ChannelGapRow>(); var playlists = new List<PlaylistDiscoveryRow>();
        AppendFlexibleResearchOutputs(opportunities, candidates, comparisons, gaps, playlists);
        var plan = BuildContentCalendarRows(candidates, 30);
        return candidates.Count == 2 && candidates.All(row => row.HasFlexible && !row.HasMechanical && row.OverallScore is null)
            && candidates.All(row => row.TalkingPoints.Contains("30 s hold", StringComparison.Ordinal))
            && comparisons.Count == 1 && gaps.Count == 1 && gaps[0].Material == "Material A" && playlists.Count == 1
            && plan.Count == 2 && plan.All(row => row.Reason.Contains("30 s hold", StringComparison.Ordinal))
            && BuildFlexibleResearchCandidates(opportunities, Array.Empty<VideoPlannerRow>()).Count == 0
            && metadata.All(row => !row.HasFlexible && row.SuggestedTitle.Length == 0);
    }
}
