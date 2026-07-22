using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

/// <summary>
/// Builds transparent per-series analytics from already calculated native results.
/// It deliberately owns no tensile, impact or stiffness formulas.
/// </summary>
public sealed class ExperimentalAnalyticsService
{
    private const double TensileWeight = 0.40d;
    private const double ImpactWeight = 0.40d;
    private const double StiffnessWeight = 0.20d;

    public ExperimentalAnalyticsResult Analyze(IReadOnlyList<ExperimentalAnalyticsInput> runs)
    {
        if (runs.Count == 0) return new ExperimentalAnalyticsResult();

        var scored = runs.Select(run => new
        {
            Run = run,
            Tensile = AverageAvailable(run.TensileUpright, run.TensileFlat),
            Impact = AverageAvailable(run.ImpactUpright, run.ImpactFlat),
            run.Stiffness
        }).ToList();

        var tensileMax = scored.Max(x => x.Tensile ?? 0d);
        var impactMax = scored.Max(x => x.Impact ?? 0d);
        var stiffnessMax = scored.Max(x => x.Stiffness ?? 0d);

        var ranked = scored.Select(x =>
        {
            var weighted = 0d;
            var availableWeight = 0d;
            AddNormalized(x.Tensile, tensileMax, TensileWeight, ref weighted, ref availableWeight);
            AddNormalized(x.Impact, impactMax, ImpactWeight, ref weighted, ref availableWeight);
            AddNormalized(x.Stiffness, stiffnessMax, StiffnessWeight, ref weighted, ref availableWeight);
            var score = availableWeight <= 0d ? 0d : weighted / availableWeight * 100d;
            return new { x.Run.ExperimentalRunId, x.Run.RunLabel, Score = score };
        })
        .OrderByDescending(x => x.Score)
        .ThenBy(x => x.RunLabel, StringComparer.CurrentCultureIgnoreCase)
        .Select((x, index) => new ExperimentalRankedRun(x.ExperimentalRunId, x.RunLabel, index + 1, x.Score))
        .ToList();

        return new ExperimentalAnalyticsResult
        {
            BaselineLabel = runs.FirstOrDefault(x => x.IsBaseline)?.RunLabel ?? string.Empty,
            BestTensileUpright = Best(runs, x => x.TensileUpright),
            BestTensileFlat = Best(runs, x => x.TensileFlat),
            BestImpactUpright = Best(runs, x => x.ImpactUpright),
            BestImpactFlat = Best(runs, x => x.ImpactFlat),
            BestStiffness = Best(runs, x => x.Stiffness),
            RecommendedRun = ranked.FirstOrDefault(x => x.OverallScore > 0d),
            RankedRuns = ranked
        };
    }

    private static void AddNormalized(double? value, double maximum, double weight, ref double weighted, ref double availableWeight)
    {
        if (!value.HasValue || maximum <= 0d) return;
        weighted += Math.Clamp(value.Value / maximum, 0d, 1d) * weight;
        availableWeight += weight;
    }

    private static double? AverageAvailable(double? first, double? second)
    {
        if (first.HasValue && second.HasValue) return (first.Value + second.Value) / 2d;
        return first ?? second;
    }

    private static ExperimentalBestResult? Best(IEnumerable<ExperimentalAnalyticsInput> runs, Func<ExperimentalAnalyticsInput, double?> selector)
    {
        var winner = runs.Select(x => new { Run = x, Value = selector(x) })
            .Where(x => x.Value.HasValue)
            .OrderByDescending(x => x.Value)
            .FirstOrDefault();
        return winner is null ? null : new ExperimentalBestResult(winner.Run.RunLabel, winner.Value!.Value);
    }
}
