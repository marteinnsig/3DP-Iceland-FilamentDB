using FilamentDbApp.Models;
using FilamentDbApp.Services.Calculations;
using FilamentDbApp.Services.Reporting;

namespace FilamentDbApp.Services.Website;

public static class PendulumPresentationVerification
{
    public static bool VerifyContract()
    {
        PendulumMethodResults Result(double mean) => new(
            new PendulumImpactSummary(2, 10, 2, 0, 0, 0, mean, 0, 0, mean, mean), 9999, "2026-09-21");
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
            payload.Izod.All(x => Equals(x["referenceMaximumKjM2"], 50d));
        var model = new PublicMaterialEngineeringReportModel
        {
            MaterialId = "PENDULUM-PUBLIC-PROBE", IzodScore = "50/100", CharpyScore = "n/a",
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
            independent.OverallScore == 94 && scores.BuildProfile(baseline).OverallScore is null &&
            metricProfile.IzodScore == 25 && metricProfile.CharpyScore is null &&
            metricProfile.OverallScore == 25 && VerifyScoringContract() &&
            service.Verify(model, publication).Passed &&
            publication.Html.Contains(">Izod</text>", StringComparison.Ordinal) &&
            publication.Html.Contains(">Charpy</text>", StringComparison.Ordinal) &&
            publication.Html.Contains("maximum measured mean in the public comparison cohort", StringComparison.Ordinal) &&
            publication.MetadataJson.Contains("\"CharpyScore\": \"n/a\"", StringComparison.Ordinal);
    }

    private static bool VerifyScoringContract()
    {
        var scoring = new EngineeringScoringService();
        var empty = new MaterialResults("PENDULUM-SCORING", null, null, null, DateTime.UnixEpoch);
        var method = new PendulumMethodResults(
            new PendulumImpactSummary(10, 10, 10, 0, 0, 0, 25, 0.125, 0.5, 24, 26), 50, "2026-09-21");
        var measured = empty with { Izod = method };
        var profile = scoring.BuildProfile(measured);
        TestSummaryMetric Metric(string test, string name, string value) => new()
        { TestType = test, MetricName = name, MetricValue = value, SourceSheet = test, SourceColumn = name };
        var metrics = scoring.BuildProfile(null, new[]
        {
            Metric("Izod", "Score", "50"), Metric("Izod", "CV", "0.5"), Metric("Izod", "Samples", "10"),
            Metric("Charpy", "Score", "NaN"), Metric("Charpy", "CV", "75"), Metric("Charpy", "Samples", "0")
        });
        var zero = scoring.BuildProfile(empty with { Izod = method with
        { Statistics = method.Statistics with { Mean = 0, CvPercent = null } } });
        var both = scoring.BuildProfile(measured with { Charpy = method with
        { Statistics = method.Statistics with { Mean = 50, CvPercent = 1.5 } } });
        var repeatability = new EngineeringConsistencyService().Analyze(measured);
        return profile.IzodScore == 50 && profile.ConsistencyScore == 99.5 && profile.OverallScore == 74.75 &&
            metrics.ConsistencyScore == profile.ConsistencyScore && metrics.OverallScore == profile.OverallScore &&
            zero.IzodScore == 0 && zero.ConsistencyScore is null && zero.OverallScore == 0 &&
            both.ConsistencyScore == 99 && both.OverallScore == 83 &&
            scoring.BuildProfile(measured, 150).OverallScore == profile.OverallScore &&
            repeatability.MeasurementSetCount == 1 && repeatability.AverageCvPercent == 0.5 &&
            repeatability.ConsistencyScore == 99.5;
    }
}
