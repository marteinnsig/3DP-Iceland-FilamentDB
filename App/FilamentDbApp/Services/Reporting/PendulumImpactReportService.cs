using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace FilamentDbApp.Services.Reporting;

/// <summary>One saved run and condition at a time; never participates in legacy scores.</summary>
public static class PendulumImpactReportService
{
    public static IReadOnlyList<PublicPendulumImpactGroup> Build(string materialId,
        IEnumerable<PendulumImpactRunRecord> runs, IEnumerable<PendulumImpactSpecimenRecord> specimens)
    {
        var result = new List<PublicPendulumImpactGroup>();
        var saved = specimens.ToArray();
        var number = 0;
        foreach (var run in runs.Where(r => string.Equals(r.MaterialID.Trim(), materialId.Trim(), StringComparison.OrdinalIgnoreCase))
                     .Where(r => PendulumImpactService.Methods.Contains(r.Method)).OrderBy(r => r.RunId, StringComparer.Ordinal))
        {
            number++;
            var conditions = new Dictionary<string, int>(StringComparer.Ordinal);
            var batches = new Dictionary<string, int>(StringComparer.Ordinal);
            var runGroups = PendulumImpactService.BuildSummaries(run, saved);
            if (runGroups.Count == 0)
                runGroups = [new PendulumImpactGroupSummary("Unmeasured", "All batches", PendulumImpactService.Summarize(run, []))];
            foreach (var group in runGroups)
            {
                if (!conditions.ContainsKey(group.Condition)) conditions.Add(group.Condition, conditions.Count + 1);
                var batch = group.PrintBatch;
                if (batch is not ("All batches" or "Unspecified batch"))
                {
                    if (!batches.ContainsKey(batch)) batches.Add(batch, batches.Count + 1);
                    batch = "Print batch " + batches[batch];
                }
                var identity = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(
                    run.RunId + "\n" + group.Condition + "\n" + group.PrintBatch)));
                result.Add(new(identity, run.Method + " Impact Strength", number, run.Date,
                    "Condition " + conditions[group.Condition], batch, run.Orientation, run.SpecimenType,
                    run.NotchType, (run.StandardReference + " " + run.StandardEdition).Trim(),
                    "Unverified comparative measurement; standard reference is not confirmed conformity", group.Summary)
                    { InputMode = run.InputMode });
            }
        }
        return result;
    }

    public static string RenderText(IReadOnlyList<PublicPendulumImpactGroup> groups) => string.Join("\n\n", groups.Select(g =>
        g.InputMode == PendulumImpactService.DirectStrengthInputMode ? RenderDirectText(g) :
        $"{g.Method} · run {g.RunNumber} · {g.Date} · {g.Condition} · {g.Batch}\n" +
        $"{g.Orientation}; specimen: {g.SpecimenType}; notch: {g.NotchType}\n" +
        $"Standard reference: {g.StandardReference}; {g.Conformity}\n" +
        $"Measured {g.Statistics.MeasuredCount}/{g.Statistics.TargetCount} run target; valid complete-break n={g.Statistics.ValidCount}; " +
        $"mean {N(g.Statistics.Mean)} kJ/m²; sample SD {N(g.Statistics.SampleStdDev)} kJ/m²; CV {N(g.Statistics.CvPercent)}%; " +
        $"min {N(g.Statistics.Minimum)}; max {N(g.Statistics.Maximum)} kJ/m²\n" +
        $"No break {g.Statistics.NoBreakCount}; invalid {g.Statistics.InvalidCount}; excluded {g.Statistics.ExcludedCount}. " +
        "Conditions and runs are separate. Batch rows overlap their condition total; do not sum them."));

    private static string RenderDirectText(PublicPendulumImpactGroup g) =>
        $"{g.Method} · {g.Date}\nInstrument readings in kJ/m²\n" +
        $"Mean {N(g.Statistics.Mean)} kJ/m²; Std. Dev {N(g.Statistics.SampleStdDev)} kJ/m²; " +
        $"CV {N(g.Statistics.CvPercent)}%; Samples {g.Statistics.ValidCount}; Confidence {N(g.Statistics.Confidence)}\n" +
        $"No break {g.Statistics.NoBreakCount}; invalid {g.Statistics.InvalidCount}; excluded {g.Statistics.ExcludedCount}.\n" +
        "Comparative measurements; standard conformity unverified.";

    public static string RenderHtml(IReadOnlyList<PublicPendulumImpactGroup> groups) => groups.Count == 0 ? "" :
        "<section class=\"pendulum-impact-results\"><h2>Izod and Charpy Impact Strength</h2>" +
        string.Concat(groups.Select(g => "<div style=\"break-inside:avoid;overflow-wrap:anywhere;white-space:normal\"><p>" +
            WebUtility.HtmlEncode(RenderText([g])).Replace("\n", "<br>", StringComparison.Ordinal) + "</p></div>")) + "</section>";

    private static string N(double? value) => value is double n && double.IsFinite(n)
        ? n.ToString("0.###", CultureInfo.InvariantCulture) : "n/a";

    public static bool VerifyContract()
    {
        var runs = new[] { new PendulumImpactRunRecord { RunId = "a", MaterialID = "M", Method = "Izod", NotchType = "Unnotched", Printer = "PRIVATE-PRINTER", Notes = "PRIVATE-NOTES" },
            new PendulumImpactRunRecord { RunId = "b", MaterialID = "M", Method = "Charpy", NotchType = "Unnotched" } };
        var specimens = runs.Select(r => new PendulumImpactSpecimenRecord { RunId = r.RunId, EnergyJ = "1", HammerJ = r.Method == "Izod" ? "2.75" : "2", WidthMm = "10", ThicknessMm = "4", BreakType = "Complete", SettingsOverrides = "PRIVATE-OVERRIDE", PrintBatch = "PRIVATE-BATCH" }).ToArray();
        var groups = Build("M", runs, specimens);
        var text = RenderText(groups);
        var input = new ReportingMaterialInput("M", new Dictionary<string, object?>(),
            new global::FilamentDbApp.Services.Calculations.MaterialResults("M", null, null, null, DateTime.UnixEpoch))
            { PendulumResults = groups };
        var payload = new ReportingDataPipelineService().BuildPayload([input]);
        var report = new ReportGeneratorService().BuildReport(payload);
        var empty = Build("M", [runs[0]], []);
        return groups.Count == 4 && groups.Select(g => g.Method).Distinct().Count() == 2 &&
            groups.All(g => g.Statistics.Mean == 25 && g.Statistics.ValidCount == 1 && g.Statistics.SampleStdDev is null) &&
            !System.Text.Json.JsonSerializer.Serialize(groups).Contains("PRIVATE-", StringComparison.Ordinal) &&
            text.Contains("sample SD", StringComparison.Ordinal) && text.Contains("not confirmed conformity", StringComparison.Ordinal) &&
            Build("OTHER", runs, specimens).Count == 0 && RenderHtml(groups).Contains("Izod Impact Strength", StringComparison.Ordinal) &&
            empty.Count == 1 && empty[0].Statistics.MeasuredCount == 0 && empty[0].Statistics.TargetCount == 10 &&
            !payload.Rows.Single().IsComplete && report.MaterialReports.Single().PendulumResults.SequenceEqual(groups) &&
            report.MaterialReports.Single().Sections.Single(s => s.SectionType == ReportingSectionType.PendulumMetrics).Fields["Results"] == text &&
            !report.MaterialReports.Single().Sections.Any(s => s.SectionType == ReportingSectionType.MechanicalMetric) &&
            VerifyDirectContract();
    }

    private static bool VerifyDirectContract()
    {
        var run = new PendulumImpactRunRecord { RunId = "direct", MaterialID = "M", Method = "Izod",
            InputMode = PendulumImpactService.DirectStrengthInputMode };
        var readings = new[] { "120", "140", "NB", "" }.Select((value, i) => new PendulumImpactSpecimenRecord
            { RunId = run.RunId, SpecimenId = "direct-" + i, StrengthKjM2Raw = value }).ToArray();
        var groups = Build("M", [run], readings);
        var text = RenderText(groups);
        return groups.Count == 1 && groups[0].Statistics.Mean == 130 &&
            groups[0].Statistics.ValidCount == 2 && groups[0].Statistics.NoBreakCount == 1 &&
            groups[0].Statistics.Confidence == 2 && groups[0].InputMode == PendulumImpactService.DirectStrengthInputMode &&
            text.Contains("Instrument readings in kJ/m²", StringComparison.Ordinal) &&
            text.Contains("Samples 2; Confidence 2", StringComparison.Ordinal) &&
            !text.Contains("run target", StringComparison.Ordinal) && !text.Contains("complete-break", StringComparison.Ordinal) &&
            !text.Contains("Batch rows", StringComparison.Ordinal) &&
            Build("M", [run], []).Single().Statistics.Mean is null;
    }
}
