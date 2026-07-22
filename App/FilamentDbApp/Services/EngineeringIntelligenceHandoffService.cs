using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

public sealed class EngineeringIntelligenceHandoffService
{
    public const string GovernanceStatement =
        "Source: Verified Material Summary and existing Engineering Intelligence outputs. This handoff does not recalculate measurements or engineering scores.";

    public EngineeringIntelligenceHandoff Build(
        EngineeringAdvisorInsight? advisor,
        EngineeringConsistencyInsight? consistency,
        EngineeringContextInsight? context,
        EngineeringPeerInsight? peer,
        IReadOnlyList<EngineeringAlternativeInsight>? alternatives)
    {
        var sections = new List<string>();
        Add(sections, "Evidence", advisor?.EvidenceSummary);
        Add(sections, "Coverage", Join(advisor?.ConfidenceLabel, advisor?.ConfidenceSummary));
        Add(sections, "Repeatability", Join(consistency?.StatusLabel, consistency?.RepeatabilitySummary));
        Add(sections, "Outlier review", consistency?.OutlierReviewSummary);
        Add(sections, "Price", context?.PriceSummary);
        Add(sections, "Inventory", Join(context?.InventoryStatus, context?.InventorySummary));
        Add(sections, "Manufacturer", context?.ManufacturerSummary);
        Add(sections, "Manufacturer position", peer?.ManufacturerPositionSummary);
        Add(sections, "Category position", peer?.CategoryPositionSummary);

        if (alternatives is { Count: > 0 })
        {
            sections.Add("Alternatives: " + string.Join("; ", alternatives.Select(item =>
                $"{item.Kind} - {item.Label} ({item.ScoreText}, {item.PriceText})")));
        }

        var summary = sections.Count == 0
            ? "No governed Engineering Intelligence context is available for this material."
            : string.Join(Environment.NewLine, sections);

        return new EngineeringIntelligenceHandoff
        {
            ReportSummary = summary,
            VideoPlannerSummary = summary + Environment.NewLine + GovernanceStatement,
            SourceStatement = GovernanceStatement,
            UsesExistingEngineeringInsights = true,
            RecalculatesMeasurementsOrScores = false
        };
    }

    private static void Add(ICollection<string> sections, string label, string? value)
    {
        if (!string.IsNullOrWhiteSpace(value)) sections.Add($"{label}: {value.Trim()}");
    }

    private static string Join(string? first, string? second) =>
        string.Join(" - ", new[] { first, second }.Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => value!.Trim()));
}
