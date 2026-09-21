using FilamentDbApp.Models;
using FilamentDbApp.Services.Calculations;
using FilamentDbApp.Services.Reporting;

namespace FilamentDbApp.Services;

public static class PendulumImpactProjectionVerification
{
    public static bool RunVerification()
    {
        var runs = new List<PendulumImpactRunRecord>();
        var specimens = new List<PendulumImpactSpecimenRecord>();
        void Add(string material, string method, params string[] samples)
        {
            var id = material + method;
            runs.Add(new() { RunId = id, MaterialID = material, Method = method, InputMode = "DirectStrength", Date = "2026-09-21" });
            for (var index = 0; index < samples.Length; index++)
                specimens.Add(new() { SpecimenId = id + index, RunId = id, StrengthKjM2Raw = samples[index] });
        }
        Add("A", "Izod", "100", "300", "", "NB");
        Add("A", "Charpy", "50");
        Add("B", "Izod", "0");
        Add("B", "Charpy", "100");
        Add("Outside", "Izod", "1000");
        Add("Empty", "Izod", "", "NB");
        runs.Add(new() { RunId = "legacy", MaterialID = "Missing", Method = "Charpy", InputMode = "Energy", NotchType = "Notched" });
        specimens.Add(new() { RunId = "legacy", EnergyJ = "1", HammerJ = "2", WidthMm = "10", ThicknessMm = "4", RemainingLigamentMm = "8", BreakType = "Complete" });
        var projected = PendulumImpactProjectionService.Build(runs, specimens, ["a", "B", "Empty", "Missing"]);
        bool Near(double? actual, double expected) => actual is double value && Math.Abs(value - expected) < 1e-8;
        var a = projected["A"];
        if (projected.Count != 4 || projected.ContainsKey("Outside") || !Near(a.Izod?.MeanKjM2, 200) || !Near(a.Izod?.Score, 100) ||
            !Near(a.Charpy?.Score, 50) || !Near(projected["b"].Izod?.Score, 0) || a.Izod?.Statistics.ValidCount != 2 ||
            !Near(a.Izod?.Statistics.SampleStdDev, Math.Sqrt(20000)) || !Near(a.Izod?.Statistics.CvPercent, Math.Sqrt(20000) / 2)) return false;
        if (projected["Empty"].Izod?.HasResults != false || projected["Empty"].Izod?.Score is not null ||
            projected["Missing"].Charpy is not null || projected["Missing"].Izod is not null) return false;
        var zeroOnly = PendulumImpactProjectionService.Build(runs, specimens, ["B"])["B"].Izod;
        if (zeroOnly?.HasResults != true || zeroOnly.Score is not null || zeroOnly.Statistics.SampleStdDev is not null) return false;
        var noResults = new MaterialResults("Missing", null, null, null, DateTime.UnixEpoch);
        var partial = noResults with { Izod = a.Izod };
        if (noResults.ResultModuleCount != 0 || noResults.HasAnyResults || partial.ResultModuleCount != 1 ||
            !partial.HasIzodResults || partial.HasCharpyResults || partial.IsCompleteEngineeringSummary) return false;
        var rating = new RatingResult(1, "Fixture", "Fixture");
        var measured = new MeasurementSetResult(1, null, null, 1, 1, rating);
        var complete = new MaterialResults("A", new(measured, measured, 1, DateTime.UnixEpoch),
            new(measured, measured, 1, 1, 1, DateTime.UnixEpoch), new(1, 1, rating, DateTime.UnixEpoch), DateTime.UnixEpoch)
            { Izod = a.Izod, Charpy = a.Charpy, HeatTemperatureC = 0 };
        if (!complete.IsCompleteEngineeringSummary || complete.ResultModuleCount != 6 ||
            (complete with { Charpy = null }).IsCompleteEngineeringSummary ||
            (complete with { Izod = null }).ResultModuleCount != 5 ||
            (complete with { HeatTemperatureC = double.NaN }).HasHeatResults) return false;
        var inputs = new[] { new ReportingMaterialInput("A", new Dictionary<string, object?>(), partial) };
        var pipeline = new ReportingDataPipelineService();
        return pipeline.Verify(inputs).Passed && pipeline.BuildPayload(inputs).Rows.Single().IzodMeanKjM2 == 200;
    }
}
