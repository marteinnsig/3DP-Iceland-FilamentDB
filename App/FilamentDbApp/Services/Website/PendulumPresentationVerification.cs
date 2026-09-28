using FilamentDbApp.Models;
using FilamentDbApp.Services.Calculations;
using FilamentDbApp.Services.Reporting;

namespace FilamentDbApp.Services.Website;

public static class PendulumPresentationVerification
{
    public static bool VerifyContract()
    {
        PendulumMethodResults Result(double mean) => new(
            new PendulumImpactSummary(2, 10, 2, 0, 0, 0, mean, 0, 0, mean, mean), 50, "2026-09-21") { ReferencePolicyVersion = "verification-only-50" };
        WebsiteChartMaterialInput Input(string id, PendulumMethodResults? result) => new(
            new Dictionary<string, object?> { ["materialId"] = id },
            new MaterialResults(id, null, null, null, DateTime.UnixEpoch) { Izod = result });
        var payload = new WebsiteChartGeneratorService().BuildPayload(new[]
        {
            Input("LOW", Result(25)), Input("HIGH", Result(50)), Input("MISSING", null), Input("ZERO", Result(0))
        });
        var scopeAndMissing = Equals(payload.Izod[0]["score"], 50d) && Equals(payload.Izod[1]["score"], 100d) &&
            payload.Izod[2]["value"] is null && payload.Izod[2]["score"] is null &&
            Equals(payload.Izod[3]["value"], 0d) && Equals(payload.Izod[3]["score"], 0d) &&
            payload.Charpy.All(x => x["value"] is null && x["score"] is null) &&
            payload.Izod.Where(x => x["value"] is not null).All(x => Equals(x["referenceMaximumKjM2"], 50d));
        var alone = new WebsiteChartGeneratorService().BuildPayload(new[] { Input("LOW", Result(25)) });
        var pending = new WebsiteChartGeneratorService().BuildPayload(new[]
        { Input("PENDING", Result(25) with { ReferencePolicyVersion = "", ReferenceMaximumKjM2 = null }) });
        scopeAndMissing &= Equals(alone.Izod[0]["score"], payload.Izod[0]["score"]) &&
            pending.Izod[0]["score"] is null && pending.Izod[0]["overallScore"] is null &&
            Equals(pending.Izod[0]["value"], 25d) && Equals(pending.Izod[0]["isOverallComparable"], false) &&
            pending.Izod[0]["scoreCoverage"] is string && payload.Impact.All(x => x["impactScore"] is null);
        MeasurementSetResult Set(double mean) => new(mean, 1, 0.5, 10, 10, new RatingResult(5, "fixture", "fixture"));
        var historicalInput = Input("LEGACY", Result(25));
        var historical = new WebsiteChartGeneratorService().BuildPayload(new[]
        {
            historicalInput with { Summary = historicalInput.Summary with
            { Impact = new ImpactResults(Set(20), Set(40), 105.411, 0.0000481603, 100, DateTime.UnixEpoch) } }
        });
        scopeAndMissing &= Equals(historical.Izod[0]["legacyImpactRadarPercent"], 30d) &&
            historical.Izod[0]["impactScore"] is null &&
            Equals(historical.Izod[0]["overallScore"], alone.Izod[0]["overallScore"]) &&
            Equals(historical.Izod[0]["consistencyScore"], alone.Izod[0]["consistencyScore"]) &&
            Equals(historical.Izod[0]["isOverallComparable"], alone.Izod[0]["isOverallComparable"]) &&
            Equals(historical.Izod[0]["scoreCoverage"], alone.Izod[0]["scoreCoverage"]);
        var model = new PublicMaterialEngineeringReportModel
        {
            MaterialId = "PENDULUM-PUBLIC-PROBE", IzodScore = "50/100", CharpyScore = "n/a",
            LegacyImpactRadarPercent = 30,
            VerifiedMeasurements = new PublicVerifiedMeasurementsModel
            {
                Izod = new PublicMeasurementSetModel { Average = 25, SampleCount = 2, StandardDeviation = 0, Confidence = 2 }
            }
        };
        var service = new PublicReportPublishingService();
        var publication = service.Build(model, DateTime.UnixEpoch, "verification", "verification");
        var scores = new EngineeringScoringService();
        var baseline = new MaterialResults("SCORE", null, null, null, DateTime.UnixEpoch);
        var independent = scores.BuildProfile(baseline with { Izod = Result(150) with { ReferenceMaximumKjM2 = 150 } });
        var metricProfile = scores.BuildProfile(null, new[]
        {
            new TestSummaryMetric { TestType = "Izod", MetricName = "Score", MetricValue = "25", SourceSheet = "Izod", SourceColumn = "Score" },
            new TestSummaryMetric { TestType = "Charpy", MetricName = "Score", MetricValue = "NaN", SourceSheet = "Charpy", SourceColumn = "Score" }
        });
        return scopeAndMissing && independent.IzodScore == 100 && independent.CharpyScore is null &&
            independent.OverallScore is null && scores.BuildProfile(baseline).OverallScore is null &&
            metricProfile.IzodScore is null && metricProfile.CharpyScore is null &&
            metricProfile.OverallScore is null && VerifyScoringContract() &&
            service.Verify(model, publication).Passed &&
            publication.Html.Contains(">Izod</text>", StringComparison.Ordinal) &&
            publication.Html.Contains(">Charpy</text>", StringComparison.Ordinal) &&
            publication.Html.Contains(">Legacy Impact %</text>", StringComparison.Ordinal) &&
            publication.Html.Contains("30 % of legacy rig capacity", StringComparison.Ordinal) &&
            !publication.Html.Contains("<polygon class=\"radar-poly-selected\"", StringComparison.Ordinal) &&
            System.Text.RegularExpressions.Regex.Matches(publication.Html, "<circle class=\"radar-poly-selected\"").Count == 2 &&
            publication.Html.Contains("versioned method references", StringComparison.Ordinal) &&
            publication.MetadataJson.Contains("\"CharpyScore\": \"n/a\"", StringComparison.Ordinal);
    }

    private static bool VerifyScoringContract()
    {
        var scoring = new EngineeringScoringService();
        var empty = new MaterialResults("PENDULUM-SCORING", null, null, null, DateTime.UnixEpoch);
        var method = new PendulumMethodResults(
            new PendulumImpactSummary(10, 10, 10, 0, 0, 0, 25, 0.125, 0.5, 24, 26), 50, "2026-09-21") { ReferencePolicyVersion = "verification-only-50" };
        var measured = empty with { Izod = method };
        var profile = scoring.BuildProfile(measured);
        TestSummaryMetric Metric(string test, string name, string value) => new()
        { TestType = test, MetricName = name, MetricValue = value, SourceSheet = test, SourceColumn = name };
        var metrics = scoring.BuildProfile(null, new[]
        {
            Metric("Izod", "Mean kJ/m²", "25"), Metric("Izod", "Score reference", "50"),
            Metric("Izod", "Score policy", "verification-only-50"), Metric("Izod", "CV", "0.5"), Metric("Izod", "Samples", "10"),
            Metric("Charpy", "Score", "NaN"), Metric("Charpy", "CV", "75"), Metric("Charpy", "Samples", "0")
        });
        var zero = scoring.BuildProfile(empty with { Izod = method with
        { Statistics = method.Statistics with { Mean = 0, CvPercent = null } } });
        var both = scoring.BuildProfile(measured with { Charpy = method with
        { Statistics = method.Statistics with { Mean = 50, CvPercent = 1.5 } } });
        var repeatability = new EngineeringConsistencyService().Analyze(measured);
        return profile.IzodScore == 50 && profile.ConsistencyScore == 99.5 && profile.OverallScore is null &&
            metrics.IzodScore == profile.IzodScore && metrics.ConsistencyScore == profile.ConsistencyScore && metrics.OverallScore == profile.OverallScore &&
            zero.IzodScore == 0 && zero.ConsistencyScore is null && zero.OverallScore is null &&
            both.ConsistencyScore == 99 && both.OverallScore is null && both.ImpactFamilyScore == 75 &&
            scoring.BuildProfile(measured, 150).OverallScore == profile.OverallScore &&
            repeatability.MeasurementSetCount == 1 && repeatability.AverageCvPercent == 0.5 &&
            repeatability.ConsistencyScore == 99.5;
    }
}
