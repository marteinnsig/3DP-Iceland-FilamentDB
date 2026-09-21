using FilamentDbApp.Models;
using FilamentDbApp.Services.Calculations;

namespace FilamentDbApp.Services;

/// <summary>Direct instrument results and method-specific comparison references for a canonical material cohort.</summary>
public static class PendulumImpactProjectionService
{
    public static IReadOnlyDictionary<string, PendulumMaterialProjection> Build(
        IEnumerable<PendulumImpactRunRecord> runs,
        IEnumerable<PendulumImpactSpecimenRecord> specimens,
        IEnumerable<string> materialIds)
    {
        var ids = materialIds.Where(id => !string.IsNullOrWhiteSpace(id)).Select(id => id.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var specimenLookup = specimens.ToLookup(row => row.RunId, StringComparer.Ordinal);
        var results = runs.Where(run => PendulumImpactService.IsDirect(run) && ids.Contains(run.MaterialID.Trim()))
            .Select(run => (Id: run.MaterialID.Trim(), run.Method, Result: new PendulumMethodResults(
                PendulumImpactService.Summarize(run, specimenLookup[run.RunId]), null, run.Date)))
            .Where(row => PendulumImpactService.Methods.Contains(row.Method))
            .ToArray();
        double? Maximum(string method) => results.Where(row => row.Method == method && row.Result.HasResults)
            .Select(row => row.Result.MeanKjM2).DefaultIfEmpty(null).Max();
        var izodMaximum = Maximum("Izod");
        var charpyMaximum = Maximum("Charpy");
        var grouped = results.ToLookup(row => row.Id, StringComparer.OrdinalIgnoreCase);
        return ids.ToDictionary(id => id, id =>
        {
            PendulumMethodResults? Method(string method, double? maximum)
            {
                // Persistence guarantees one direct row per material/method. Never merge duplicate historical runs.
                var matches = grouped[id].Where(row => row.Method == method).ToArray();
                return matches.Length == 1 ? matches[0].Result with { ReferenceMaximumKjM2 = maximum } : null;
            }
            return new PendulumMaterialProjection(Method("Izod", izodMaximum), Method("Charpy", charpyMaximum));
        }, StringComparer.OrdinalIgnoreCase);
    }
}
