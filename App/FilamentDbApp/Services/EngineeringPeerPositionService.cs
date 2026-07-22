using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

public sealed class EngineeringPeerPositionService
{
    public EngineeringPeerInsight Analyze(string materialId, IReadOnlyList<EngineeringPeerCandidate> source)
    {
        var candidates = source
            .Where(item => !string.IsNullOrWhiteSpace(item.MaterialId))
            .GroupBy(item => item.MaterialId.Trim(), StringComparer.OrdinalIgnoreCase)
            .Select(group => group.First())
            .ToList();
        var selected = candidates.FirstOrDefault(item =>
            item.MaterialId.Equals(materialId, StringComparison.OrdinalIgnoreCase));
        if (selected is null)
        {
            return new EngineeringPeerInsight
            {
                ManufacturerPositionSummary = "Selected material is not present in the active comparison dataset.",
                CategoryPositionSummary = "Category position is unavailable for the current selection.",
                ActiveDatasetMaterialCount = candidates.Count,
                UsesExistingScoreProfiles = true
            };
        }

        var manufacturer = BuildPosition(
            selected,
            candidates.Where(item => SameGroup(item.Manufacturer, selected.Manufacturer)).ToList(),
            selected.Manufacturer,
            "manufacturer");
        var category = BuildPosition(
            selected,
            candidates.Where(item => SameGroup(item.Category, selected.Category)).ToList(),
            selected.Category,
            "category");

        return new EngineeringPeerInsight
        {
            ManufacturerPositionSummary = manufacturer.Summary,
            CategoryPositionSummary = category.Summary,
            ManufacturerRank = manufacturer.Rank,
            ManufacturerPeerCount = manufacturer.Count,
            CategoryRank = category.Rank,
            CategoryPeerCount = category.Count,
            ActiveDatasetMaterialCount = candidates.Count,
            UsesExistingScoreProfiles = true
        };
    }

    private static PositionResult BuildPosition(
        EngineeringPeerCandidate selected,
        IReadOnlyList<EngineeringPeerCandidate> group,
        string groupName,
        string groupType)
    {
        if (string.IsNullOrWhiteSpace(groupName))
        {
            return new PositionResult(null, 0, $"{Capitalize(groupType)} position is unavailable because the material has no {groupType} classification.");
        }

        var ranked = group
            .Where(item => item.OverallScore.HasValue)
            .OrderByDescending(item => item.OverallScore)
            .ThenBy(item => item.Label, StringComparer.OrdinalIgnoreCase)
            .ToList();
        if (!selected.OverallScore.HasValue || ranked.Count == 0)
        {
            return new PositionResult(null, ranked.Count, $"{groupName}: no comparable overall engineering score is available.");
        }

        var index = ranked.FindIndex(item => item.MaterialId.Equals(selected.MaterialId, StringComparison.OrdinalIgnoreCase));
        if (index < 0)
        {
            return new PositionResult(null, ranked.Count, $"{groupName}: selected material has no ranked peer position.");
        }

        var average = ranked.Average(item => item.OverallScore!.Value);
        var delta = selected.OverallScore.Value - average;
        var comparison = Math.Abs(delta) < 0.05
            ? "matches"
            : delta > 0
                ? $"is {delta:0.#} points above"
                : $"is {Math.Abs(delta):0.#} points below";
        var label = groupType == "manufacturer" ? "Manufacturer position" : "Category position";
        return new PositionResult(
            index + 1,
            ranked.Count,
            $"{label}: #{index + 1} of {ranked.Count} for {groupName}; overall {selected.OverallScore.Value:0.#}/100 {comparison} the {average:0.#}/100 group average in the active dataset.");
    }

    private static bool SameGroup(string first, string second) =>
        !string.IsNullOrWhiteSpace(first) &&
        !string.IsNullOrWhiteSpace(second) &&
        first.Trim().Equals(second.Trim(), StringComparison.OrdinalIgnoreCase);

    private static string Capitalize(string value) =>
        value.Length == 0 ? value : char.ToUpperInvariant(value[0]) + value[1..];

    private sealed record PositionResult(int? Rank, int Count, string Summary);
}
