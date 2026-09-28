using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

public static class EngineeringAdvisorVerification
{
    public static bool VerifyPendulumContract()
    {
        var service = new EngineeringAdvisorService();
        static EngineeringAdvisorCandidate Candidate(string id, EngineeringScoreProfile profile) => new()
        {
            MaterialId = id,
            Label = id,
            RecommendationType = "Impact-resistant parts",
            RecommendationScore = 75,
            Profile = profile
        };

        var selected = Candidate("selected", new EngineeringScoreProfile { IzodScore = 80, CharpyScore = 20, ImpactScore = 100 });
        var compared = Candidate("compared", new EngineeringScoreProfile { IzodScore = 50, CharpyScore = 60, ImpactScore = 0 });
        var insight = service.Explain(selected, compared);
        if (insight.CoveredAxes != 2 || insight.TotalAxes != 6 ||
            insight.ConfidenceLabel != "Limited evidence coverage" ||
            !insight.EvidenceSummary.Contains("Izod 80", StringComparison.Ordinal) ||
            insight.ClearestLeadAxis != "Izod" || insight.ClearestLeadDelta != 30 ||
            insight.ClearestTradeOffAxis != "Charpy" || insight.ClearestTradeOffDelta != -40)
            return false;

        var missing = service.Explain(Candidate("missing", new EngineeringScoreProfile()));
        var zero = service.Explain(Candidate("zero", new EngineeringScoreProfile { IzodScore = 0 }));
        var thermalOnly = service.Explain(Candidate("thermal", new EngineeringScoreProfile { ThermalScore = 90 }));
        var legacyOnly = service.Explain(Candidate("legacy", new EngineeringScoreProfile { ImpactScore = 100 }));
        if (missing.CoveredAxes != 0 || zero.CoveredAxes != 1 || thermalOnly.CoveredAxes != 0 ||
            legacyOnly.CoveredAxes != 0 || legacyOnly.EvidenceSummary != missing.EvidenceSummary ||
            legacyOnly.TradeOffSummary != missing.TradeOffSummary ||
            !zero.TradeOffSummary.Contains("Charpy", StringComparison.Ordinal))
            return false;

        var full = service.Explain(Candidate("full", new EngineeringScoreProfile
        {
            TensileScore = 80, ImpactScore = 80, IzodScore = 80, CharpyScore = 80,
            StiffnessScore = 80, ConsistencyScore = 80, LayerAdhesionScore = 80
        }));
        if (full.CoveredAxes != 6 || full.ConfidenceLabel != "High evidence coverage" ||
            !full.TradeOffSummary.Contains("All six modern", StringComparison.Ordinal))
            return false;

        var partial = service.Explain(Candidate("partial", new EngineeringScoreProfile
        {
            TensileScore = 80, IzodScore = 80, StiffnessScore = 80, ConsistencyScore = 80, LayerAdhesionScore = 80
        }));
        if (partial.CoveredAxes != 5 || partial.ConfidenceLabel != "Moderate evidence coverage" ||
            !partial.TradeOffSummary.Contains("Missing evidence: Charpy", StringComparison.Ordinal)) return false;

        // Pendulum-only profiles must influence similarity and specialist suggestions.
        var near = Candidate("near", new EngineeringScoreProfile { IzodScore = 79, CharpyScore = 19, ImpactScore = 0 });
        var specialist = Candidate("specialist", new EngineeringScoreProfile { IzodScore = 95, CharpyScore = 20, ImpactScore = 100 });
        var alternatives = service.FindAlternatives(selected, [compared, near, specialist]);
        return alternatives.Count == 3 && alternatives[0].Label == "near" &&
            alternatives.Any(item => item.Kind == "Specialist alternative" && item.Label == "compared" &&
                item.GainSummary.Contains("Charpy", StringComparison.Ordinal)) &&
            selected.Profile.OverallScore is null;
    }
}
