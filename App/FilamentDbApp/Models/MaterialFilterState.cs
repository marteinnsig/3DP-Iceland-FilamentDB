namespace FilamentDbApp.Models;

using System.Text;

public enum MaterialFilterFacet
{
    Manufacturer,
    BaseMaterial,
    VariantFinish,
    Reinforcement,
    Color,
    ProductLine
}

public sealed record MaterialFacetSelection(string Key, string Label);

public sealed class MaterialFilterState
{
    public string SearchText { get; set; } = string.Empty;
    public List<MaterialFacetSelection> Manufacturers { get; set; } = new();
    public List<MaterialFacetSelection> BaseMaterials { get; set; } = new();
    public List<MaterialFacetSelection> VariantFinishes { get; set; } = new();
    public List<MaterialFacetSelection> Reinforcements { get; set; } = new();
    public List<MaterialFacetSelection> Colors { get; set; } = new();
    public List<MaterialFacetSelection> ProductLines { get; set; } = new();

    public IReadOnlyList<MaterialFacetSelection> GetSelections(MaterialFilterFacet facet) =>
        facet switch
        {
            MaterialFilterFacet.Manufacturer => Manufacturers,
            MaterialFilterFacet.BaseMaterial => BaseMaterials,
            MaterialFilterFacet.VariantFinish => VariantFinishes,
            MaterialFilterFacet.Reinforcement => Reinforcements,
            MaterialFilterFacet.Color => Colors,
            MaterialFilterFacet.ProductLine => ProductLines,
            _ => Array.Empty<MaterialFacetSelection>()
        };

    public void SetSelections(
        MaterialFilterFacet facet,
        IEnumerable<MaterialFacetSelection>? selections)
    {
        var normalized = MaterialFilterProjectionService.NormalizeSelections(selections);
        switch (facet)
        {
            case MaterialFilterFacet.Manufacturer:
                Manufacturers = normalized;
                break;
            case MaterialFilterFacet.BaseMaterial:
                BaseMaterials = normalized;
                break;
            case MaterialFilterFacet.VariantFinish:
                VariantFinishes = normalized;
                break;
            case MaterialFilterFacet.Reinforcement:
                Reinforcements = normalized;
                break;
            case MaterialFilterFacet.Color:
                Colors = normalized;
                break;
            case MaterialFilterFacet.ProductLine:
                ProductLines = normalized;
                break;
        }
    }

    public void Clear(MaterialFilterFacet facet) =>
        SetSelections(facet, Array.Empty<MaterialFacetSelection>());

    public void ClearAll()
    {
        SearchText = string.Empty;
        foreach (var facet in Enum.GetValues<MaterialFilterFacet>())
        {
            Clear(facet);
        }
    }

    public MaterialFilterState Normalize()
    {
        SearchText = SearchText?.Trim() ?? string.Empty;
        foreach (var facet in Enum.GetValues<MaterialFilterFacet>())
        {
            SetSelections(facet, GetSelections(facet));
        }

        return this;
    }

    public MaterialFilterState Clone() =>
        new MaterialFilterState
        {
            SearchText = SearchText,
            Manufacturers = Manufacturers?.ToList() ?? new(),
            BaseMaterials = BaseMaterials?.ToList() ?? new(),
            VariantFinishes = VariantFinishes?.ToList() ?? new(),
            Reinforcements = Reinforcements?.ToList() ?? new(),
            Colors = Colors?.ToList() ?? new(),
            ProductLines = ProductLines?.ToList() ?? new()
        }.Normalize();
}

public sealed record MaterialFilterCandidate(
    string MaterialId,
    bool IsArchived,
    string SearchText,
    IReadOnlyDictionary<MaterialFilterFacet, string> FacetKeys);

public sealed record VisibleMaterialScopeSnapshot(
    IReadOnlyList<string> MaterialIds,
    string ScopeHash,
    string FilterDescription)
{
    public int Count => MaterialIds.Count;
}

public static class MaterialFilterProjectionService
{
    public const string UnlinkedKey = "unlinked";

    public static string LinkedIdKey(long id) => $"id:{id}";

    public static string ExactValueKey(string? value) =>
        $"value:{NormalizeValue(value)}";

    public static List<MaterialFacetSelection> NormalizeSelections(
        IEnumerable<MaterialFacetSelection>? selections) =>
        (selections ?? Array.Empty<MaterialFacetSelection>())
        .Where(selection =>
            selection is not null &&
            !string.IsNullOrWhiteSpace(selection.Key))
        .Select(selection => new MaterialFacetSelection(
            NormalizeSelectionKey(selection.Key),
            selection.Label?.Trim() ?? string.Empty))
        .GroupBy(selection => selection.Key, StringComparer.Ordinal)
        .Select(group => group.First())
        .OrderBy(selection => selection.Label, StringComparer.OrdinalIgnoreCase)
        .ThenBy(selection => selection.Key, StringComparer.Ordinal)
        .ToList();

    public static IReadOnlyList<string> ProjectMaterialIds(
        IEnumerable<MaterialFilterCandidate> candidates,
        MaterialFilterState state,
        bool includeArchived)
    {
        ArgumentNullException.ThrowIfNull(candidates);
        ArgumentNullException.ThrowIfNull(state);

        var normalizedState = state.Clone();
        var selectedKeys = Enum.GetValues<MaterialFilterFacet>()
            .ToDictionary(
                facet => facet,
                facet => normalizedState.GetSelections(facet)
                    .Select(selection => selection.Key)
                    .ToHashSet(StringComparer.Ordinal));

        return candidates
            .Where(candidate =>
                !string.IsNullOrWhiteSpace(candidate.MaterialId) &&
                (includeArchived || !candidate.IsArchived) &&
                MatchesSearch(candidate, normalizedState.SearchText) &&
                Enum.GetValues<MaterialFilterFacet>().All(facet =>
                    selectedKeys[facet].Count == 0 ||
                    candidate.FacetKeys.TryGetValue(facet, out var candidateKey) &&
                    selectedKeys[facet].Contains(candidateKey)))
            .Select(candidate => candidate.MaterialId.Trim())
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(materialId => materialId, StringComparer.OrdinalIgnoreCase)
            .ThenBy(materialId => materialId, StringComparer.Ordinal)
            .ToList();
    }

    public static string NormalizeSelectionKey(string? key)
    {
        var trimmed = key?.Trim() ?? string.Empty;
        if (trimmed.StartsWith("value:", StringComparison.OrdinalIgnoreCase))
        {
            return "value:" + NormalizeValue(trimmed["value:".Length..]);
        }

        if (trimmed.StartsWith("id:", StringComparison.OrdinalIgnoreCase))
        {
            return "id:" + trimmed["id:".Length..].Trim();
        }

        return string.Equals(
            trimmed,
            UnlinkedKey,
            StringComparison.OrdinalIgnoreCase)
            ? UnlinkedKey
            : trimmed.ToLowerInvariant();
    }

    public static string NormalizeValue(string? value) =>
        string.Join(
                " ",
                (value ?? string.Empty)
                .Normalize(NormalizationForm.FormC)
                .Split(
                    (char[]?)null,
                    StringSplitOptions.RemoveEmptyEntries |
                    StringSplitOptions.TrimEntries))
            .ToUpperInvariant();

    public static MaterialFilterContractVerificationResult RunContractVerification()
    {
        static IReadOnlyDictionary<MaterialFilterFacet, string> Facets(
            string manufacturer,
            string baseMaterial,
            string variant,
            string reinforcement,
            string color,
            string productLine) =>
            new Dictionary<MaterialFilterFacet, string>
            {
                [MaterialFilterFacet.Manufacturer] = manufacturer,
                [MaterialFilterFacet.BaseMaterial] = baseMaterial,
                [MaterialFilterFacet.VariantFinish] = variant,
                [MaterialFilterFacet.Reinforcement] = reinforcement,
                [MaterialFilterFacet.Color] = color,
                [MaterialFilterFacet.ProductLine] = productLine
            };

        var candidates = new[]
        {
            new MaterialFilterCandidate(
                "MAT-A",
                false,
                "Maker A Alpha PLA Matte Blue",
                Facets("id:1", "id:10", "value:MATTE", "value:", "value:BLUE", "value:ALPHA")),
            new MaterialFilterCandidate(
                "MAT-B",
                false,
                "Maker B Beta PLA Silk Yellow",
                Facets("id:2", "id:10", "value:SILK", "value:", "value:YELLOW", "value:BETA")),
            new MaterialFilterCandidate(
                "MAT-C",
                false,
                "Maker C Gamma PETG Matte Black CF",
                Facets("id:3", "id:11", "value:MATTE", "value:CF", "value:BLACK", "value:GAMMA")),
            new MaterialFilterCandidate(
                "MAT-ARCHIVED",
                true,
                "Maker A Archive PLA Matte Blue",
                Facets("id:1", "id:10", "value:MATTE", "value:", "value:BLUE", "value:ARCHIVE")),
            new MaterialFilterCandidate(
                "MAT-UNLINKED",
                false,
                "Legacy PLA Natural",
                Facets(UnlinkedKey, UnlinkedKey, "value:", "value:", "value:NATURAL", "value:LEGACY"))
        };

        var emptyState = new MaterialFilterState();
        var active = ProjectMaterialIds(candidates, emptyState, includeArchived: false);
        var manager = ProjectMaterialIds(candidates, emptyState, includeArchived: true);

        var state = new MaterialFilterState();
        state.SetSelections(
            MaterialFilterFacet.Manufacturer,
            new[]
            {
                new MaterialFacetSelection("id:1", "Maker A"),
                new MaterialFacetSelection("ID:2", "Maker B"),
                new MaterialFacetSelection(" id:2 ", "Maker B duplicate")
            });
        var manufacturerOr = ProjectMaterialIds(candidates, state, includeArchived: false);
        state.SetSelections(
            MaterialFilterFacet.BaseMaterial,
            new[] { new MaterialFacetSelection("id:10", "PLA") });
        state.SetSelections(
            MaterialFilterFacet.Color,
            new[]
            {
                new MaterialFacetSelection("value:blue", "Blue"),
                new MaterialFacetSelection("value:yellow", "Yellow")
            });
        var crossFacetAnd = ProjectMaterialIds(candidates, state, includeArchived: false);
        state.SearchText = "Beta";
        var searchAnd = ProjectMaterialIds(candidates, state, includeArchived: false);
        state.Clear(MaterialFilterFacet.Color);
        var clearOne = ProjectMaterialIds(candidates, state, includeArchived: false);
        state.ClearAll();
        var clearAll = ProjectMaterialIds(candidates, state, includeArchived: false);

        var unlinkedState = new MaterialFilterState();
        unlinkedState.SetSelections(
            MaterialFilterFacet.Manufacturer,
            new[] { new MaterialFacetSelection(UnlinkedKey, "Unlinked manufacturers") });
        var unlinked = ProjectMaterialIds(candidates, unlinkedState, includeArchived: false);

        var passed =
            active.SequenceEqual(new[] { "MAT-A", "MAT-B", "MAT-C", "MAT-UNLINKED" }) &&
            manager.Contains("MAT-ARCHIVED", StringComparer.OrdinalIgnoreCase) &&
            manufacturerOr.SequenceEqual(new[] { "MAT-A", "MAT-B" }) &&
            crossFacetAnd.SequenceEqual(new[] { "MAT-A", "MAT-B" }) &&
            searchAnd.SequenceEqual(new[] { "MAT-B" }) &&
            clearOne.SequenceEqual(new[] { "MAT-B" }) &&
            clearAll.SequenceEqual(active) &&
            unlinked.SequenceEqual(new[] { "MAT-UNLINKED" }) &&
            state.GetSelections(MaterialFilterFacet.Manufacturer).Count == 0;

        return new MaterialFilterContractVerificationResult(
            passed,
            passed
                ? "OR-within, AND-between, search, archived, unlinked and Clear contracts pass"
                : "One or more deterministic Materials filter contracts failed");
    }

    private static bool MatchesSearch(
        MaterialFilterCandidate candidate,
        string searchText) =>
        string.IsNullOrWhiteSpace(searchText) ||
        candidate.SearchText.Contains(searchText, StringComparison.OrdinalIgnoreCase);
}

public sealed record MaterialFilterContractVerificationResult(
    bool Passed,
    string Details);
