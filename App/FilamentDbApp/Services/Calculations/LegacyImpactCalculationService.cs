namespace FilamentDbApp.Services.Calculations;

public sealed record LegacyImpactSettings(
    double AvailableEnergyJ,
    double NetAreaMm2,
    double NoSampleAngleDegrees);

public sealed record LegacyImpactCalculationResult(
    double Fraction,
    double ReboundDegrees,
    double ReboundRadians,
    double AbsorbedEnergyJ,
    double ImpactKjM2);

/// <summary>
/// Converts the legacy needle percentage with its original angular relationship.
/// Settings name the physical units explicitly; no persistence or scale migration occurs here.
/// </summary>
public static class LegacyImpactCalculationService
{
    public static LegacyImpactCalculationResult? TryCalculate(double percent, LegacyImpactSettings? settings)
    {
        if (settings is null || !double.IsFinite(percent) || percent is < 0 or > 100 ||
            !double.IsFinite(settings.AvailableEnergyJ) || settings.AvailableEnergyJ <= 0 ||
            !double.IsFinite(settings.NetAreaMm2) || settings.NetAreaMm2 <= 0 ||
            !double.IsFinite(settings.NoSampleAngleDegrees) || settings.NoSampleAngleDegrees is <= 0 or >= 360)
            return null;

        // Preserve legacy angles above 180 degrees rather than silently clamping them.
        // One full revolution is excluded because the angular reference becomes singular.
        var referenceRadians = Math.PI * settings.NoSampleAngleDegrees / 180d;
        var denominator = 1 - Math.Cos(referenceRadians);
        if (!double.IsFinite(denominator) || denominator <= 0) return null;

        var reboundDegrees = settings.NoSampleAngleDegrees * (1 - percent / 100d);
        var reboundRadians = Math.PI * reboundDegrees / 180d;
        var fraction = 1 - (1 - Math.Cos(reboundRadians)) / denominator;
        var absorbedEnergyJ = settings.AvailableEnergyJ * fraction;
        var impactKjM2 = absorbedEnergyJ / settings.NetAreaMm2 * 1000d;
        if (!double.IsFinite(fraction) || !double.IsFinite(reboundDegrees) || !double.IsFinite(reboundRadians) ||
            !double.IsFinite(absorbedEnergyJ) || !double.IsFinite(impactKjM2)) return null;

        return new(fraction, reboundDegrees, reboundRadians, absorbedEnergyJ, impactKjM2);
    }

    public static bool TryCalculate(double percent, LegacyImpactSettings? settings,
        out LegacyImpactCalculationResult? result)
    {
        result = TryCalculate(percent, settings);
        return result is not null;
    }
}
