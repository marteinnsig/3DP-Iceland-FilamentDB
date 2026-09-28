using System.Globalization;
using FilamentDbApp.Models;

namespace FilamentDbApp.Services.Calculations;

/// <summary>Numeric contracts for fixed method references and five-family Overall scoring.</summary>
public static class ImpactScorePolicyVerification
{
    public static bool RunVerification()
    {
        static bool Near(double? value, double expected) => value is double actual && Math.Abs(actual - expected) < 1e-9;
        var policy = new ImpactScoreReferencePolicy("test-v1", 200, 100);
        PendulumMethodResults Method(double mean, string method) => new(
            new(10, 10, 10, 0, 0, 0, mean, mean * .1, 10, mean, mean), policy.ReferenceFor(method), "2026-09-28")
            { ReferencePolicyVersion = policy.Version };
        var izod = Method(100, "Izod");
        var charpy = Method(25, "Charpy");
        if (!Near(izod.Score, 50) || !Near(charpy.Score, 25) ||
            (izod with { ReferencePolicyVersion = "" }).Score is not null) return false;
        foreach (var invalid in new double?[] { null, 0, -1, double.NaN, double.PositiveInfinity, double.NegativeInfinity })
        {
            var rejected = new ImpactScoreReferencePolicy("test-invalid", invalid, invalid);
            if (rejected.ReferenceFor("Izod") is not null || rejected.ReferenceFor("Charpy") is not null ||
                (izod with { ReferenceMaximumKjM2 = invalid }).Score is not null) return false;
        }
        if (new ImpactScoreReferencePolicy(" ", 200, 100).ReferenceFor("Izod") is not null ||
            policy.ReferenceFor("Unknown") is not null ||
            ImpactScoreReferencePolicy.Current.ReferenceFor("Izod") is not null ||
            ImpactScoreReferencePolicy.Current.ReferenceFor("Charpy") is not null) return false;
        var unconfigured = izod with { ReferenceMaximumKjM2 = ImpactScoreReferencePolicy.Current.ReferenceFor("Izod"),
            ReferencePolicyVersion = ImpactScoreReferencePolicy.Current.Version };
        if (!unconfigured.HasResults || unconfigured.MeanKjM2 != 100 || unconfigured.Score is not null ||
            unconfigured.Statistics.ValidCount != 10) return false;

        var rating = new RatingResult(1, "Fixture", "Fixture");
        MeasurementSetResult Set(double mean, double cv = .1, int count = 10) => new(mean, mean * cv, cv, count, 10, rating);
        var summary = new MaterialResults("fixture", new(Set(20), Set(40), 1, DateTime.UnixEpoch),
            new(Set(56), Set(56), 105.411, .0000481603, 56.9735, DateTime.UnixEpoch),
            new(1, 1500, rating, DateTime.UnixEpoch), DateTime.UnixEpoch) { Izod = izod, Charpy = charpy };
        var service = new EngineeringScoringService();
        var profile = service.BuildProfile(summary);
        // (37.5 tensile + 50 stiffness + 90 consistency + 50 adhesion + 37.5 impact family) / 5.
        if (!Near(profile.OverallScore, 53) || !Near(profile.ImpactFamilyScore, 37.5) ||
            profile.ImpactScore is not null || profile.ScoredComponentCount != 5 ||
            profile.MeasuredImpactMethodCount != 2 || profile.ScoredImpactMethodCount != 2 || !profile.IsOverallComparable) return false;
        foreach (var legacy in new ImpactResults?[] { null,
            new(Set(0, 0, 1), Set(1000, 9, 100), 10, 1, 1000, DateTime.UnixEpoch),
            new(Set(double.NaN, double.NaN, 0), Set(double.PositiveInfinity, 3, 300), null, null, null, DateTime.UnixEpoch) })
        {
            var changed = service.BuildProfile(summary with { Impact = legacy });
            if (changed.OverallScore != profile.OverallScore || changed.ConsistencyScore != profile.ConsistencyScore ||
                changed.ScoredComponentCount != 5 || changed.ImpactScore is not null) return false;
        }
        var missing = service.BuildProfile(summary with { Charpy = null });
        if (missing.OverallScore is not null || missing.ImpactFamilyScore is not null || missing.IsOverallComparable ||
            missing.ScoredComponentCount != 4 || missing.MeasuredImpactMethodCount != 1 || missing.ScoredImpactMethodCount != 1) return false;
        var noReference = service.BuildProfile(summary with { Izod = unconfigured });
        if (noReference.OverallScore is not null || noReference.MeasuredImpactMethodCount != 2 || noReference.ScoredImpactMethodCount != 1) return false;
        if (service.BuildProfile(summary with { Tensile = null }).OverallScore is not null ||
            service.BuildProfile(summary with { Stiffness = null }).OverallScore is not null) return false;
        var zeros = service.BuildProfile(summary with { Izod = Method(0, "Izod"), Charpy = Method(0, "Charpy") });
        if (!Near(zeros.ImpactFamilyScore, 0) || !Near(zeros.OverallScore, 45.5) || !zeros.IsOverallComparable) return false;

        var tensile = new TensileTestResult { UprightMpa = "20", FlatMpa = "40", CvUpright = "10%", CvFlat = "10%",
            SamplesUpright = "10", SamplesFlat = "10" };
        TestSummaryMetric Metric(string method, string name, string value) => new()
            { TestType = method, MetricName = name, MetricValue = value, SourceSheet = "Fixture", SourceColumn = name };
        var metrics = new List<TestSummaryMetric> { Metric("Stiffness", "Modulus", "1500") };
        foreach (var item in new[] { ("Izod", izod), ("Charpy", charpy) })
        {
            metrics.Add(Metric(item.Item1, "Mean kJ/m²", item.Item2.MeanKjM2!.Value.ToString(CultureInfo.InvariantCulture)));
            metrics.Add(Metric(item.Item1, "Samples", "10"));
            metrics.Add(Metric(item.Item1, "CV", "10"));
            metrics.Add(Metric(item.Item1, "Score reference", item.Item2.ReferenceMaximumKjM2!.Value.ToString(CultureInfo.InvariantCulture)));
            metrics.Add(Metric(item.Item1, "Score policy", policy.Version));
            metrics.Add(Metric(item.Item1, "Score", "99")); // Retired cached score must be ignored.
        }
        var fromMetrics = service.BuildProfile(tensile, metrics);
        if (!Near(fromMetrics.OverallScore, 53) || fromMetrics.ConsistencyScore != profile.ConsistencyScore ||
            fromMetrics.MeasuredImpactMethodCount != 2 || fromMetrics.ScoredComponentCount != 5) return false;
        metrics.AddRange(new[] { Metric("Impact", "Flat", "999999"), Metric("Impact", "Upright", "999999"),
            Metric("Impact", "CV Flat", "999"), Metric("Impact", "Samples Flat", "1") });
        if (service.BuildProfile(tensile, metrics).OverallScore != fromMetrics.OverallScore) return false;
        foreach (var key in new[] { "Score reference", "Score policy", "Samples" })
        {
            var incomplete = metrics.Where(x => !(x.TestType == "Charpy" && x.MetricName == key)).ToArray();
            if (service.BuildProfile(tensile, incomplete).OverallScore is not null) return false;
        }
        return true;
    }
}
