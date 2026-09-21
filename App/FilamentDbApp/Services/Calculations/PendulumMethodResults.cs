using FilamentDbApp.Models;

namespace FilamentDbApp.Services.Calculations;

public sealed record PendulumMethodResults(
    PendulumImpactSummary Statistics,
    double? ReferenceMaximumKjM2,
    string Date)
{
    public double? MeanKjM2 => Statistics.Mean;
    public bool HasResults => Statistics.ValidCount > 0 && MeanKjM2 is double mean && double.IsFinite(mean) && mean >= 0;
    public double? Score => HasResults && ReferenceMaximumKjM2 is double maximum && double.IsFinite(maximum) && maximum > 0
        ? Math.Clamp(MeanKjM2!.Value / maximum * 100d, 0d, 100d) : null;
}

public sealed record PendulumMaterialProjection(PendulumMethodResults? Izod, PendulumMethodResults? Charpy);
