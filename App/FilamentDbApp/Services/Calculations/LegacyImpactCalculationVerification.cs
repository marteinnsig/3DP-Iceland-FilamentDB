namespace FilamentDbApp.Services.Calculations;

public static class LegacyImpactCalculationVerification
{
    public static bool RunVerification()
    {
        var settings = new LegacyImpactSettings(2.743860924, 48.1603, 105.411);
        static bool Near(double actual, double expected) =>
            double.IsFinite(actual) && Math.Abs(actual - expected) <= 1e-11 * Math.Max(1, Math.Abs(expected));
        var zero = LegacyImpactCalculationService.TryCalculate(0, settings);
        var full = LegacyImpactCalculationService.TryCalculate(100, settings);
        var midpoint = LegacyImpactCalculationService.TryCalculate(50, settings);
        if (zero is null || full is null || midpoint is null ||
            !Near(zero.Fraction, 0) || !Near(zero.ReboundDegrees, 105.411) ||
            !Near(zero.AbsorbedEnergyJ, 0) || !Near(zero.ImpactKjM2, 0) ||
            !Near(full.Fraction, 1) || !Near(full.ReboundDegrees, 0) || !Near(full.ReboundRadians, 0) ||
            !Near(full.AbsorbedEnergyJ, 2.743860924) || !Near(full.ImpactKjM2, 56.973501493969088)) return false;

        // Independent literals calculated with PowerShell System.Math, not the service under test.
        if (!Near(midpoint.Fraction, 0.68865044390728869) ||
            !Near(midpoint.ReboundDegrees, 52.7055) || !Near(midpoint.ReboundRadians, 0.91988450890987139) ||
            !Near(midpoint.AbsorbedEnergyJ, 1.8895610433324632) ||
            !Near(midpoint.ImpactKjM2, 39.234827094774396)) return false;

        foreach (var invalid in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity, -1d, 101d })
            if (LegacyImpactCalculationService.TryCalculate(invalid, settings) is not null) return false;
        foreach (var invalid in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity, -1d, 0d })
        {
            if (LegacyImpactCalculationService.TryCalculate(50, settings with { AvailableEnergyJ = invalid }) is not null ||
                LegacyImpactCalculationService.TryCalculate(50, settings with { NetAreaMm2 = invalid }) is not null ||
                LegacyImpactCalculationService.TryCalculate(50, settings with { NoSampleAngleDegrees = invalid }) is not null)
                return false;
        }
        foreach (var invalidAngle in new[] { 360d, 361d, 720d, double.Epsilon })
            if (LegacyImpactCalculationService.TryCalculate(50, settings with { NoSampleAngleDegrees = invalidAngle }) is not null)
                return false;
        if (LegacyImpactCalculationService.TryCalculate(50, null) is not null ||
            LegacyImpactCalculationService.TryCalculate(100, settings with { NetAreaMm2 = double.Epsilon }) is not null)
            return false;

        var doubleEnergy = LegacyImpactCalculationService.TryCalculate(50, settings with { AvailableEnergyJ = settings.AvailableEnergyJ * 2 });
        var doubleArea = LegacyImpactCalculationService.TryCalculate(50, settings with { NetAreaMm2 = settings.NetAreaMm2 * 2 });
        return doubleEnergy is not null && doubleArea is not null &&
            Near(doubleEnergy.ImpactKjM2, midpoint.ImpactKjM2 * 2) &&
            Near(doubleArea.ImpactKjM2, midpoint.ImpactKjM2 / 2) &&
            LegacyImpactCalculationService.TryCalculate(100, settings with { NoSampleAngleDegrees = 270 }) is not null &&
            LegacyImpactCalculationService.TryCalculate(50, settings, out var result) && result == midpoint &&
            !LegacyImpactCalculationService.TryCalculate(double.NaN, settings, out var invalidResult) && invalidResult is null &&
            VerifyResultsServiceParity();
    }

    public static bool VerifyResultsServiceParity()
    {
        static bool Serializable(ImpactResults value)
        {
            try
            {
                var json = System.Text.Json.JsonSerializer.Serialize(value);
                var restored = System.Text.Json.JsonSerializer.Deserialize<ImpactResults>(json);
                return restored == value;
            }
            catch (Exception ex) when (ex is ArgumentException or System.Text.Json.JsonException)
            { return false; }
        }
        static bool Near(double? actual, double expected) => actual.HasValue && double.IsFinite(actual.Value) &&
            Math.Abs(actual.Value - expected) <= 1e-11 * Math.Max(1, Math.Abs(expected));
        static bool Empty(MeasurementSetResult set) => set.SampleCount == 0 && set.Average is null &&
            set.StandardDeviation is null && set.CoefficientOfVariation is null && set.Confidence is null;
        var service = new ResultsService();
        var samples = new[] { "0", "50", "100", "", "NB", "NaN", "Infinity", "-1", "101" };
        var result = service.CalculateImpact(samples, ["50"], 105.411, 48.1603, 2.743860924);
        // Independent aggregate literals for converted samples 0, 39.234827094774396, 56.973501493969088.
        if (!Near(result.Upright.Average, 32.069442862914492) ||
            !Near(result.Upright.StandardDeviation, 29.154794057126598) ||
            !Near(result.Upright.CoefficientOfVariation, 0.90911445458385465) ||
            result.Upright.SampleCount != 3 || result.Upright.Confidence != 3 ||
            !Near(result.Flat.Average, 39.234827094774396) || result.Flat.SampleCount != 1 ||
            result.Flat.StandardDeviation is not null || result.Flat.CoefficientOfVariation is not null ||
            result.Flat.Confidence != 1 || !Near(result.NoSampleAngleDegrees, 105.411) ||
            !Near(result.NetCrossSectionAreaM2, 0.0000481603) ||
            !Near(result.MaxPossibleImpact, 56.973501493969088) || !Near(result.AvailableEnergyJ, 2.743860924) ||
            !Serializable(result)) return false;

        var scaled = service.CalculateImpact(samples, [], 105.411, 48.1603, 5.487721848);
        if (!Near(scaled.Upright.Average, 64.138885725828984) ||
            !Near(scaled.Upright.StandardDeviation, 58.309588114253196) ||
            !Near(scaled.Upright.CoefficientOfVariation, result.Upright.CoefficientOfVariation!.Value) ||
            !Empty(scaled.Flat)) return false;
        var zero = service.CalculateImpact(["0"], ["0", "0"], 105.411, 48.1603, 2.743860924);
        if (!Near(zero.Upright.Average, 0) || zero.Upright.SampleCount != 1 ||
            zero.Upright.CoefficientOfVariation is not null || !Near(zero.Flat.StandardDeviation, 0) ||
            zero.Flat.CoefficientOfVariation is not null) return false;
        foreach (var invalid in new[] { double.NaN, double.PositiveInfinity, double.NegativeInfinity, -1d, 0d })
        {
            var invalidEnergy = service.CalculateImpact(samples, ["50"], 105.411, 48.1603, invalid);
            var invalidArea = service.CalculateImpact(samples, ["50"], 105.411, invalid, 2.743860924);
            var invalidAngle = service.CalculateImpact(samples, ["50"], invalid, 48.1603, 2.743860924);
            if (!Empty(invalidEnergy.Upright) || !Empty(invalidEnergy.Flat) ||
                !Empty(invalidArea.Upright) || !Empty(invalidArea.Flat) ||
                !Empty(invalidAngle.Upright) || !Empty(invalidAngle.Flat) ||
                !Serializable(invalidEnergy) || !Serializable(invalidArea) || !Serializable(invalidAngle)) return false;
        }
        return true;
    }
}
