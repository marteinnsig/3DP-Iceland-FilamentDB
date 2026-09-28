using FilamentDbApp.Models;

namespace FilamentDbApp.Services.Calculations;

/// <summary>The historical radar percentage is a display projection, never a modern score input.</summary>
public static class LegacyImpactRadarVerification
{
    public static bool RunVerification()
    {
        static bool Near(double? value, double expected) => value is double actual && Math.Abs(actual - expected) < 1e-9;
        const double maximum = 56.97350149396911;
        if (!Near(LegacyImpactRadarService.Calculate(0, 10, 0, 10, maximum), 0) ||
            !Near(LegacyImpactRadarService.Calculate(maximum, 10, maximum, 10, maximum), 100) ||
            !Near(LegacyImpactRadarService.Calculate(25, 2, 75, 10, 100), 50) ||
            !Near(LegacyImpactRadarService.Calculate(maximum / 4, 2, maximum * .75, 10, maximum), 50) ||
            !Near(LegacyImpactRadarService.Calculate(null, 0, maximum / 2, 10, maximum), 50) ||
            !Near(LegacyImpactRadarService.Calculate(maximum / 2, 10, null, 0, maximum), 50) ||
            !Near(LegacyImpactRadarService.Calculate(maximum * 2, 10, null, 0, maximum), 100)) return false;
        foreach (var invalid in new double?[] { null, 0, -1, double.NaN, double.PositiveInfinity, double.NegativeInfinity })
            if (LegacyImpactRadarService.Calculate(25, 10, 75, 10, invalid) is not null) return false;
        foreach (var invalidMean in new double?[] { null, -1, double.NaN, double.PositiveInfinity, double.NegativeInfinity })
        {
            if (LegacyImpactRadarService.Calculate(invalidMean, 10, null, 0, maximum) is not null ||
                !Near(LegacyImpactRadarService.Calculate(invalidMean, 10, maximum / 2, 10, maximum), 50)) return false;
        }
        if (LegacyImpactRadarService.Calculate(25, 0, 75, -1, 100) is not null ||
            !Near(LegacyImpactRadarService.Calculate(25, 0, 75, 10, 100), 75)) return false;
        // Unrelated stronger material evaluations cannot change this fixed-calibration result.
        var original = LegacyImpactRadarService.Calculate(25, 10, 75, 10, 100);
        _ = LegacyImpactRadarService.Calculate(1000, 10, 1000, 10, 100);
        if (LegacyImpactRadarService.Calculate(25, 10, 75, 10, 100) != original) return false;

        var rating = new RatingResult(1, "Fixture", "Fixture");
        MeasurementSetResult Set(double mean, int count = 10, double cv = .1) => new(mean, mean * cv, cv, count, 10, rating);
        PendulumMethodResults Method(double mean, double reference) => new(
            new(10, 10, 10, 0, 0, 0, mean, mean * .1, 10, mean, mean), reference, "2026-09-28")
            { ReferencePolicyVersion = "test-v1" };
        var summary = new MaterialResults("fixture", new(Set(20), Set(40), 1, DateTime.UnixEpoch),
            new(Set(25, 2), Set(75), 105.411, .0000481603, 100, DateTime.UnixEpoch),
            new(1, 1500, rating, DateTime.UnixEpoch), DateTime.UnixEpoch)
            { Izod = Method(100, 200), Charpy = Method(25, 100) };
        var service = new EngineeringScoringService();
        var profile = service.BuildProfile(summary);
        if (!Near(profile.LegacyImpactRadarPercent, 50) || profile.ImpactScore is not null || !Near(profile.OverallScore, 53)) return false;
        foreach (var impact in new ImpactResults?[] { null,
            new(Set(0, 1, 0), Set(0, 1, 0), 105.411, .0000481603, 100, DateTime.UnixEpoch),
            new(Set(100, 100, 9), Set(100, 100, 9), 105.411, .0000481603, 100, DateTime.UnixEpoch) })
        {
            var changed = service.BuildProfile(summary with { Impact = impact });
            if (changed.ImpactScore is not null || changed.OverallScore != profile.OverallScore ||
                changed.ConsistencyScore != profile.ConsistencyScore || changed.ScoredComponentCount != profile.ScoredComponentCount) return false;
        }

        TestSummaryMetric Metric(string name, string value) => new()
            { TestType = "Impact", MetricName = name, MetricValue = value, SourceSheet = "Fixture", SourceColumn = name };
        var metrics = new List<TestSummaryMetric>
        {
            Metric("Upright kJ", "25"), Metric("Flat kJ", "75"),
            Metric("Upright Samples", "2"), Metric("Flat Samples", "10"), Metric("Radar maximum kJ/m²", "100")
        };
        var metricProfile = service.BuildProfile(null, metrics);
        var impactOnly = service.BuildProfile(new MaterialResults("fixture", null, summary.Impact, null, DateTime.UnixEpoch));
        if (metricProfile.LegacyImpactRadarPercent != profile.LegacyImpactRadarPercent ||
            metricProfile.LegacyImpactRadarPercent != impactOnly.LegacyImpactRadarPercent ||
            metricProfile.OverallScore is not null || metricProfile.ImpactScore is not null || metricProfile.ConsistencyScore is not null) return false;
        if (service.BuildProfile(null, metrics.Where(x => x.MetricName != "Radar maximum kJ/m²").ToArray()).LegacyImpactRadarPercent is not null ||
            service.BuildProfile(null, metrics.Where(x => !x.MetricName.Contains("Samples", StringComparison.Ordinal)).ToArray()).LegacyImpactRadarPercent is not null) return false;
        metrics[0] = Metric("Upright kJ", "0");
        metrics[1] = Metric("Flat kJ", "0");
        return Near(service.BuildProfile(null, metrics).LegacyImpactRadarPercent, 0);
    }
}
