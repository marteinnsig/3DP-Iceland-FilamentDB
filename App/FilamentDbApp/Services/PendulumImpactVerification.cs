using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

public static class PendulumImpactVerification
{
    public static bool RunCalculationVerification()
    {
        var run = new PendulumImpactRunRecord { RunId = "I", MaterialID = "M", Method = "Izod", NotchType = "Notched" };
        var first = new PendulumImpactSpecimenRecord { RunId = "I", SpecimenId = "1", EnergyJ = "1", HammerJ = "2.75", WidthMm = "10", ThicknessMm = "4", RemainingLigamentMm = "8", BreakType = "Complete" };
        var second = new PendulumImpactSpecimenRecord { RunId = "I", SpecimenId = "2", EnergyJ = "2", HammerJ = "2.75", WidthMm = "10", ThicknessMm = "4", RemainingLigamentMm = "8", BreakType = "Complete" };
        bool Near(double? actual, double expected) => actual.HasValue && Math.Abs(actual.Value - expected) < 1e-9;
        if (!Near(PendulumImpactService.Calculate(run, first).StrengthKjM2, 31.25)) return false;
        var summary = PendulumImpactService.Summarize(run, [first, second]);
        if (!Near(summary.Mean, 46.875) || !Near(summary.SampleStdDev, 22.09708691207961) || summary.ValidCount != 2 || summary.TargetCount != 10) return false;
        if (PendulumImpactService.Summarize(run, [first]).SampleStdDev is not null) return false;
        var pending = Enumerable.Range(3, 9).Select(x => new PendulumImpactSpecimenRecord { RunId = "I", SpecimenId = x.ToString() }).ToArray();
        var incomplete = PendulumImpactService.Summarize(run, pending.Prepend(first));
        if (incomplete.ValidCount != 1 || incomplete.MeasuredCount != 1 || incomplete.TargetCount != 10 || !Near(incomplete.Mean, 31.25)) return false;
        if (PendulumImpactService.BuildSummaries(run, pending.Prepend(first)).Count(x => x.PrintBatch == "All batches") != 1) return false;
        second.BreakType = "No break";
        summary = PendulumImpactService.Summarize(run, [first, second]);
        if (summary.NoBreakCount != 1 || summary.ValidCount != 1 || !Near(summary.Mean, 31.25)) return false;
        second.BreakType = "Complete"; second.Status = "Excluded"; second.ExclusionReason = "Fixture slip";
        if (PendulumImpactService.Summarize(run, [first, second]).ValidCount != 1 || second.EnergyJ != "2") return false;
        second.Status = "Invalid";
        if (PendulumImpactService.Summarize(run, [first, second]).InvalidCount != 1) return false;
        second.Status = "Valid"; second.SettingsOverrides = "Different orientation";
        if (PendulumImpactService.Summarize(run, [first, second]).ValidCount != 0 || PendulumImpactService.BuildSummaries(run, [first, second]).Count(x => x.PrintBatch == "All batches") != 2) return false;
        second.SettingsOverrides = ""; first.PrintBatch = "A"; second.PrintBatch = "B";
        if (PendulumImpactService.Summarize(run, [first, second], "A").ValidCount != 1) return false;
        first.EnergyJ = "0"; second.EnergyJ = "0";
        summary = PendulumImpactService.Summarize(run, [first, second]);
        if (summary.CvPercent is not null || summary.Mean != 0 || summary.SampleStdDev != 0) return false;
        foreach (var energy in new[] { "NaN", "Infinity", "-1", "3", "" })
        {
            first.EnergyJ = energy;
            if (PendulumImpactService.Calculate(run, first).StrengthKjM2 is not null) return false;
        }
        first.EnergyJ = "1,25";
        if (!Near(PendulumImpactService.Calculate(run, first).StrengthKjM2, 39.0625)) return false;
        run.NotchType = "Unconfirmed";
        if (PendulumImpactService.Calculate(run, first).StrengthKjM2 is not null) return false;
        run.NotchType = "Unnotched";
        if (PendulumImpactService.Calculate(run, first).StrengthKjM2 is not null) return false;
        first.RemainingLigamentMm = "";
        if (!Near(PendulumImpactService.Calculate(run, first).StrengthKjM2, 31.25)) return false;
        run.Method = "Charpy";
        if (PendulumImpactService.Calculate(run, first).StrengthKjM2 is not null) return false;
        first.HammerJ = "2";
        if (!Near(PendulumImpactService.Calculate(run, first).StrengthKjM2, 31.25)) return false;
        first.RunId = "OTHER";
        if (PendulumImpactService.Calculate(run, first).StrengthKjM2 is not null || PendulumImpactService.Summarize(run, [first]).ValidCount != 0) return false;
        first.RunId = "I"; first.ThicknessMm = "";
        return PendulumImpactService.Calculate(run, first).StrengthKjM2 is null && run.NominalThicknessMm == "" && RunDirectStrengthVerification();
    }

    private static bool RunDirectStrengthVerification()
    {
        // Old saved records do not contain InputMode: deserialization must preserve the energy contract.
        var old = System.Text.Json.JsonSerializer.Deserialize<PendulumImpactRunRecord>("{\"RunId\":\"old\",\"Method\":\"Izod\"}");
        if (old?.InputMode != "Energy") return false;
        foreach (var method in PendulumImpactService.Methods)
        {
            var run = new PendulumImpactRunRecord { RunId = "DIRECT", MaterialID = "M", Method = method, InputMode = "DirectStrength" };
            var rows = Enumerable.Range(1, 10).Select(i => new PendulumImpactSpecimenRecord
                { RunId = run.RunId, SpecimenId = i.ToString(), Label = i.ToString("00"), EnergyJ = "2", HammerJ = "5", ThicknessMm = "4", WidthMm = "10" }).ToArray();
            if (PendulumImpactService.Validate([run], rows).Count != 0 || PendulumImpactService.Validate([run], rows.Take(9)).Count == 0) return false;
            var duplicate = new PendulumImpactRunRecord { RunId = "DUPLICATE", MaterialID = run.MaterialID, Method = method, InputMode = "DirectStrength" };
            var duplicateRows = Enumerable.Range(1, 10).Select(i => new PendulumImpactSpecimenRecord
                { RunId = duplicate.RunId, SpecimenId = "DUP" + i, Label = i.ToString("00") });
            if (!PendulumImpactService.Validate([run, duplicate], rows.Concat(duplicateRows)).Any(error => error.Contains("one row per material"))) return false;
            var blank = PendulumImpactService.Summarize(run, rows);
            if (blank.ValidCount != 0 || blank.MeasuredCount != 0 || blank.Mean is not null || blank.Confidence is not null) return false;
            rows[0].StrengthKjM2Raw = "150,5";
            var single = PendulumImpactService.Summarize(run, rows);
            if (single.Mean != 150.5 || single.ValidCount != 1 || single.SampleStdDev is not null || single.CvPercent is not null || single.Confidence != 1) return false;
            rows[1].StrengthKjM2Raw = "250.5";
            rows[2].StrengthKjM2Raw = "NB";
            rows[3].StrengthKjM2Raw = "No break";
            var mixed = PendulumImpactService.Summarize(run, rows);
            var statistics = new Calculations.StatisticsService();
            var expectedMean = statistics.Average([150.5, 250.5]);
            var expectedSd = statistics.StandardDeviationSample([150.5, 250.5]);
            if (mixed.Mean != expectedMean || mixed.SampleStdDev != expectedSd || mixed.ValidCount != 2 || mixed.MeasuredCount != 4 ||
                mixed.NoBreakCount != 2 || mixed.InvalidCount != 0 || mixed.Confidence != 2 ||
                Math.Abs(mixed.CoefficientOfVariation!.Value - statistics.CoefficientOfVariation(expectedSd, expectedMean)!.Value) > 1e-12) return false;
            if (PendulumImpactService.BuildSummaries(run, rows).Single(x => x.PrintBatch == "All batches").Summary.ValidCount != 2) return false;
            foreach (var invalid in new[] { "-1", "NaN", "Infinity", "1e999", "junk", "12junk" })
            {
                rows[0].StrengthKjM2Raw = invalid;
                if (PendulumImpactService.Calculate(run, rows[0]).StrengthKjM2.HasValue || PendulumImpactService.Summarize(run, rows).InvalidCount != 1 ||
                    PendulumImpactService.Validate([run], rows).Count == 0) return false;
            }
            rows[0].StrengthKjM2Raw = rows[1].StrengthKjM2Raw = "0";
            var zero = PendulumImpactService.Summarize(run, rows);
            if (zero.ValidCount != 2 || zero.Mean != 0 || zero.SampleStdDev != 0 || zero.CvPercent is not null) return false;
            foreach (var row in rows) row.StrengthKjM2Raw = "200";
            var full = PendulumImpactService.Summarize(run, rows);
            if (full.ValidCount != 10 || full.Confidence != 10 || full.Mean != 200 || full.SampleStdDev != 0 || full.CvPercent != 0) return false;
            // Identical raw graph with Energy mode still uses its original geometry and hammer rules.
            run.InputMode = "Energy";
            if (PendulumImpactService.Calculate(run, rows[0]).StrengthKjM2.HasValue || rows[0].EnergyJ != "2" || rows[0].StrengthKjM2Raw != "200") return false;
        }
        return true;
    }
}
