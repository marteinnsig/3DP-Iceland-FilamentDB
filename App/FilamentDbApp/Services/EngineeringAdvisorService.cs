using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

/// <summary>
/// Creates deterministic explanations from an existing engineering score profile.
/// This service never owns or recalculates tensile, impact, stiffness, consistency,
/// layer-adhesion or overall engineering values.
/// </summary>
public sealed class EngineeringAdvisorService
{
    private const int TotalAxisCount = 5;
    private readonly EngineeringValueIndexService _valueIndexService = new();

    public EngineeringAdvisorInsight Explain(
        EngineeringAdvisorCandidate candidate,
        EngineeringAdvisorCandidate? comparisonCandidate = null)
    {
        ArgumentNullException.ThrowIfNull(candidate);

        var axes = GetAxes(candidate.Profile);
        var available = axes.Where(axis => axis.Score.HasValue).ToList();
        var missing = axes.Where(axis => !axis.Score.HasValue).Select(axis => axis.Name).ToList();
        var strongest = available.OrderByDescending(axis => axis.Score).FirstOrDefault();
        var weakest = available.OrderBy(axis => axis.Score).FirstOrDefault();
        var confidence = BuildConfidence(candidate.Profile, available.Count);
        var comparison = BuildComparison(candidate, comparisonCandidate);

        var evidence = available.Count == 0
            ? $"Recommendation score {candidate.RecommendationScore:0}/100; no engineering-axis evidence is available."
            : $"Strongest available axis: {strongest.Name} {strongest.Score:0}/100. Recommendation score: {candidate.RecommendationScore:0}/100."
                + (candidate.Profile.OverallScore.HasValue ? $" Overall profile: {candidate.Profile.OverallScore:0}/100." : string.Empty);

        var tradeOff = available.Count == 0
            ? "No engineering axes are available, so trade-offs cannot yet be assessed."
            : $"Lowest available axis: {weakest.Name} {weakest.Score:0}/100."
                + (missing.Count == 0 ? " All five engineering axes are represented." : $" Missing evidence: {string.Join(", ", missing)}.");

        return new EngineeringAdvisorInsight
        {
            ConfidenceLabel = confidence.Label,
            ConfidenceSummary = confidence.Summary,
            EvidenceSummary = evidence,
            TradeOffSummary = tradeOff,
            ComparisonSummary = comparison.Summary,
            ComparisonScoreDelta = comparison.ScoreDelta,
            ClearestLeadAxis = comparison.LeadAxis,
            ClearestLeadDelta = comparison.LeadDelta,
            ClearestTradeOffAxis = comparison.TradeOffAxis,
            ClearestTradeOffDelta = comparison.TradeOffDelta,
            CoveredAxes = available.Count,
            TotalAxes = TotalAxisCount
        };
    }

    public IReadOnlyList<EngineeringAlternativeInsight> FindAlternatives(
        EngineeringAdvisorCandidate selected,
        IEnumerable<EngineeringAdvisorCandidate> candidates,
        int maximumResults = 3)
    {
        ArgumentNullException.ThrowIfNull(selected);
        ArgumentNullException.ThrowIfNull(candidates);
        if (maximumResults <= 0) return Array.Empty<EngineeringAlternativeInsight>();

        var available = candidates
            .Where(candidate => !IsSameMaterial(selected, candidate))
            .Where(candidate => candidate.RecommendationType.Equals(selected.RecommendationType, StringComparison.OrdinalIgnoreCase))
            .GroupBy(CandidateIdentity, StringComparer.OrdinalIgnoreCase)
            .Select(group => group.OrderByDescending(candidate => candidate.RecommendationScore).First())
            .ToList();
        if (available.Count == 0) return Array.Empty<EngineeringAlternativeInsight>();

        var chosen = new List<(string Kind, EngineeringAdvisorCandidate Candidate)>();
        var used = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var closest = available
            .OrderByDescending(candidate => SameFamily(selected, candidate))
            .ThenBy(candidate => ProfileDistance(selected.Profile, candidate.Profile))
            .ThenBy(candidate => Math.Abs(selected.RecommendationScore - candidate.RecommendationScore))
            .FirstOrDefault();
        AddChoice(chosen, used, "Closest alternative", closest);

        var valueGem = available
            .Where(candidate => selected.PricePerKg is > 0 && candidate.PricePerKg is > 0)
            .Where(candidate => candidate.Profile.OverallScore.HasValue)
            .Where(candidate => candidate.RecommendationScore >= selected.RecommendationScore - 12)
            .Where(candidate => candidate.PricePerKg!.Value <= selected.PricePerKg!.Value * 0.95)
            .OrderByDescending(candidate => _valueIndexService.Calculate(
                candidate.Profile.OverallScore,
                candidate.PricePerKg,
                BuildComparisonScope(candidate)).Value)
            .ThenByDescending(candidate => selected.PricePerKg!.Value - candidate.PricePerKg!.Value)
            .FirstOrDefault(candidate => !used.Contains(CandidateIdentity(candidate)));
        AddChoice(chosen, used, "Value hidden gem", valueGem);

        var specialist = available
            .Select(candidate => new { Candidate = candidate, Lead = StrongestAxisDelta(candidate.Profile, selected.Profile) })
            .Where(item => item.Lead.Delta is > 5 && item.Candidate.RecommendationScore >= selected.RecommendationScore - 15)
            .OrderByDescending(item => item.Lead.Delta)
            .ThenByDescending(item => item.Candidate.RecommendationScore)
            .Select(item => item.Candidate)
            .FirstOrDefault(candidate => !used.Contains(CandidateIdentity(candidate)));
        AddChoice(chosen, used, "Specialist alternative", specialist);

        foreach (var candidate in available
                     .Where(candidate => !used.Contains(CandidateIdentity(candidate)))
                     .OrderByDescending(candidate => SameFamily(selected, candidate))
                     .ThenBy(candidate => Math.Abs(selected.RecommendationScore - candidate.RecommendationScore))
                     .ThenBy(candidate => ProfileDistance(selected.Profile, candidate.Profile)))
        {
            AddChoice(chosen, used, "Comparable alternative", candidate);
            if (chosen.Count >= maximumResults) break;
        }

        return chosen
            .Take(maximumResults)
            .Select(item => BuildAlternativeInsight(item.Kind, selected, item.Candidate))
            .ToList();
    }

    private EngineeringAlternativeInsight BuildAlternativeInsight(
        string kind,
        EngineeringAdvisorCandidate selected,
        EngineeringAdvisorCandidate alternative)
    {
        var scoreDelta = alternative.RecommendationScore - selected.RecommendationScore;
        var priceDelta = selected.PricePerKg is > 0 && alternative.PricePerKg.HasValue
            ? (alternative.PricePerKg.Value - selected.PricePerKg.Value) / selected.PricePerKg.Value * 100.0
            : (double?)null;
        var strongestGain = StrongestAxisDelta(alternative.Profile, selected.Profile);
        var strongestTradeOff = StrongestAxisDelta(selected.Profile, alternative.Profile);
        var valueIndex = _valueIndexService.Calculate(
            alternative.Profile.OverallScore,
            alternative.PricePerKg,
            BuildComparisonScope(alternative));

        var gain = strongestGain.Delta is > 0.5
            ? $"Gains {strongestGain.Delta:0.0} points in {strongestGain.Axis}."
            : scoreDelta > 0.05
                ? $"Recommendation score is {scoreDelta:0.0} points higher."
                : priceDelta is < -0.5
                    ? $"MSRP is {Math.Abs(priceDelta.Value):0}% lower."
                    : "Offers a closely comparable verified score profile.";
        var tradeOff = strongestTradeOff.Delta is > 0.5
            ? $"Gives up {strongestTradeOff.Delta:0.0} points in {strongestTradeOff.Axis}."
            : scoreDelta < -0.05
                ? $"Recommendation score is {Math.Abs(scoreDelta):0.0} points lower."
                : "No material axis trade-off is visible in the available profile.";

        var summary = kind switch
        {
            "Value hidden gem" when priceDelta.HasValue => $"Comparable performance with {Math.Abs(priceDelta.Value):0}% lower MSRP per kg.",
            "Specialist alternative" => $"A focused option when {strongestGain.Axis.ToLowerInvariant()} matters more than the balanced result.",
            "Closest alternative" => $"Closest available profile in the current filtered {selected.RecommendationType} group.",
            _ => $"Comparable option from the current filtered {selected.RecommendationType} group."
        };

        return new EngineeringAlternativeInsight
        {
            Kind = kind,
            Label = alternative.Label,
            RecommendationScore = alternative.RecommendationScore,
            ScoreDelta = scoreDelta,
            PricePerKg = alternative.PricePerKg,
            PriceDeltaPercent = priceDelta,
            Summary = summary,
            GainSummary = gain,
            TradeOffSummary = tradeOff,
            ValueIndex = valueIndex
        };
    }

    private static string BuildComparisonScope(EngineeringAdvisorCandidate candidate) =>
        !string.IsNullOrWhiteSpace(candidate.BaseMaterial)
            ? $"current filtered {candidate.BaseMaterial.Trim()} group"
            : $"current filtered {candidate.RecommendationType.Trim()} group";

    private static void AddChoice(
        ICollection<(string Kind, EngineeringAdvisorCandidate Candidate)> chosen,
        ISet<string> used,
        string kind,
        EngineeringAdvisorCandidate? candidate)
    {
        if (candidate is null) return;
        var identity = CandidateIdentity(candidate);
        if (!used.Add(identity)) return;
        chosen.Add((kind, candidate));
    }

    private static string CandidateIdentity(EngineeringAdvisorCandidate candidate) =>
        string.IsNullOrWhiteSpace(candidate.MaterialId) ? candidate.Label.Trim() : candidate.MaterialId.Trim();

    private static bool IsSameMaterial(EngineeringAdvisorCandidate left, EngineeringAdvisorCandidate right) =>
        CandidateIdentity(left).Equals(CandidateIdentity(right), StringComparison.OrdinalIgnoreCase);

    private static bool SameFamily(EngineeringAdvisorCandidate left, EngineeringAdvisorCandidate right) =>
        !string.IsNullOrWhiteSpace(left.BaseMaterial) &&
        left.BaseMaterial.Equals(right.BaseMaterial, StringComparison.OrdinalIgnoreCase);

    private static double ProfileDistance(EngineeringScoreProfile left, EngineeringScoreProfile right)
    {
        var differences = GetAxes(left)
            .Zip(GetAxes(right), (a, b) => a.Score.HasValue && b.Score.HasValue
                ? Math.Abs(a.Score.Value - b.Score.Value)
                : (double?)null)
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .ToList();
        return differences.Count == 0 ? double.MaxValue : differences.Average();
    }

    private static (string Axis, double? Delta) StrongestAxisDelta(EngineeringScoreProfile left, EngineeringScoreProfile right)
    {
        return GetAxes(left)
            .Zip(GetAxes(right), (a, b) => (Axis: a.Name, Delta: a.Score.HasValue && b.Score.HasValue ? a.Score.Value - b.Score.Value : (double?)null))
            .Where(item => item.Delta.HasValue)
            .OrderByDescending(item => item.Delta)
            .FirstOrDefault();
    }

    private static IReadOnlyList<(string Name, double? Score)> GetAxes(EngineeringScoreProfile profile) =>
    [
        ("Tensile", profile.TensileScore),
        ("Impact", profile.ImpactScore),
        ("Stiffness", profile.StiffnessScore),
        ("Consistency", profile.ConsistencyScore),
        ("Layer adhesion", profile.LayerAdhesionScore)
    ];

    private static (string Label, string Summary) BuildConfidence(EngineeringScoreProfile profile, int coveredAxes)
    {
        var consistency = profile.ConsistencyScore;
        var label = coveredAxes switch
        {
            5 when consistency >= 75 => "High evidence coverage",
            >= 4 when consistency >= 55 => "Moderate evidence coverage",
            >= 4 => "Moderate coverage / variable results",
            _ => "Limited evidence coverage"
        };

        var consistencyText = consistency.HasValue
            ? $" Consistency score: {consistency:0}/100."
            : " Consistency evidence is unavailable.";

        return (label, $"{coveredAxes}/{TotalAxisCount} engineering axes available.{consistencyText} This is an advisor evidence indicator, not statistical confidence.");
    }

    private static (string Summary, double? ScoreDelta, string LeadAxis, double? LeadDelta, string TradeOffAxis, double? TradeOffDelta) BuildComparison(
        EngineeringAdvisorCandidate candidate,
        EngineeringAdvisorCandidate? comparisonCandidate)
    {
        if (comparisonCandidate is null)
        {
            return ("No comparable alternative is available in the current filtered recommendation group.", null, string.Empty, null, string.Empty, null);
        }

        var delta = candidate.RecommendationScore - comparisonCandidate.RecommendationScore;
        var direction = delta switch
        {
            > 0.05 => $"leads by {delta:0.0} points",
            < -0.05 => $"trails by {Math.Abs(delta):0.0} points",
            _ => "is effectively tied"
        };

        var axisComparisons = GetAxes(candidate.Profile)
            .Zip(GetAxes(comparisonCandidate.Profile), (current, other) => new
            {
                current.Name,
                Delta = current.Score.HasValue && other.Score.HasValue
                    ? current.Score.Value - other.Score.Value
                    : (double?)null
            })
            .Where(item => item.Delta.HasValue)
            .ToList();

        var clearestLead = axisComparisons.OrderByDescending(item => item.Delta).FirstOrDefault();
        var clearestTradeOff = axisComparisons.OrderBy(item => item.Delta).FirstOrDefault();
        var details = new List<string>();

        if (clearestLead?.Delta > 0.5)
        {
            details.Add($"clearest axis lead: {clearestLead.Name} +{clearestLead.Delta:0.0}");
        }

        if (clearestTradeOff?.Delta < -0.5)
        {
            details.Add($"clearest axis trade-off: {clearestTradeOff.Name} {clearestTradeOff.Delta:0.0}");
        }

        var suffix = details.Count == 0 ? string.Empty : $" ({string.Join("; ", details)})";
        return (
            $"Compared with {comparisonCandidate.Label}, this recommendation {direction}{suffix}.",
            delta,
            clearestLead?.Name ?? string.Empty,
            clearestLead?.Delta,
            clearestTradeOff?.Name ?? string.Empty,
            clearestTradeOff?.Delta);
    }
}
