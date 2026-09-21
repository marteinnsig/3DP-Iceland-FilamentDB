using System.Globalization;
using FilamentDbApp.Models;
using FilamentDbApp.Services.Calculations;

namespace FilamentDbApp.Services;

/// <summary>Independent pendulum measurements. No legacy Impact score or nominal geometry fallback.</summary>
public static class PendulumImpactService
{
    public const string DirectStrengthInputMode = "DirectStrength";
    public static bool IsDirect(PendulumImpactRunRecord run) => run.InputMode == DirectStrengthInputMode;
    public static bool IsNoBreak(string? raw) => string.Equals(raw?.Trim(), "NB", StringComparison.OrdinalIgnoreCase) ||
        string.Equals(raw?.Trim(), "No break", StringComparison.OrdinalIgnoreCase);
    public static bool HasReading(PendulumImpactRunRecord run, PendulumImpactSpecimenRecord row) =>
        IsDirect(run) ? !string.IsNullOrWhiteSpace(row.StrengthKjM2Raw) : !string.IsNullOrWhiteSpace(row.EnergyJ) || row.BreakType != "Unmeasured";
    public static readonly string[] Methods = ["Izod", "Charpy"];
    public static readonly string[] NotchTypes = ["Unconfirmed", "Notched", "Unnotched"];
    public static readonly string[] BreakTypes = ["Unmeasured", "Complete", "Partial", "Hinge", "No break"];
    public static readonly string[] Statuses = ["Valid", "Invalid", "Excluded"];
    public static double? Parse(string? text)
    {
        if (string.IsNullOrWhiteSpace(text)) return null;
        var normalized = text.Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var n) && double.IsFinite(n) ? n : null;
    }
    public static string Format(double? value) => value?.ToString("0.###", CultureInfo.InvariantCulture) ?? "";
    public static string[] HammerChoices(string method) => method == "Izod" ? ["2.75", "5.5"] : method == "Charpy" ? ["2", "4", "5"] : [];
    public static string[] BreakChoices(string method) => method == "Izod" ? ["Unmeasured", "Complete", "Partial", "No break"] : BreakTypes;

    public static PendulumImpactResult Calculate(PendulumImpactRunRecord run, PendulumImpactSpecimenRecord specimen)
    {
        PendulumImpactResult Reject(string why) => new(null, why);
        if (!Methods.Contains(run.Method) || specimen.RunId != run.RunId) return Reject("Unknown method or mismatched run");
        if (specimen.Status != "Valid") return Reject(specimen.Status + ": " + specimen.ExclusionReason);
        if (IsDirect(run))
        {
            if (IsNoBreak(specimen.StrengthKjM2Raw)) return Reject("No break; excluded from numeric statistics");
            var strength = Parse(specimen.StrengthKjM2Raw);
            return strength is >= 0 ? new(strength, "Instrument reading in kJ/m²; no conversion") :
                Reject(string.IsNullOrWhiteSpace(specimen.StrengthKjM2Raw) ? "Unmeasured" : "Enter a finite nonnegative kJ/m² reading or NB");
        }
        if (run.InputMode != "Energy") return Reject("Unknown input mode");
        if (specimen.BreakType != "Complete") return Reject(specimen.BreakType + "; excluded from complete-break statistics");
        if (run.NotchType is not ("Notched" or "Unnotched")) return Reject("Confirm specimen notch state before calculation");
        var energy = Parse(specimen.EnergyJ);
        var hammer = Parse(specimen.HammerJ);
        var width = Parse(specimen.WidthMm);
        var thickness = Parse(specimen.ThicknessMm);
        var ligament = Parse(specimen.RemainingLigamentMm);
        if (energy is null || energy < 0) return Reject("Measured energy must be finite and nonnegative");
        if (hammer is null || !HammerChoices(run.Method).Any(x => Parse(x) == hammer)) return Reject("Select a supported hammer for this method");
        if (energy > hammer) return Reject("Absorbed energy exceeds hammer capacity");
        if (width is null or <= 0 || thickness is null or <= 0) return Reject("Measured width and thickness must be positive");
        if (!string.IsNullOrWhiteSpace(specimen.LengthMm) && Parse(specimen.LengthMm) is not > 0) return Reject("Measured length must be finite and positive when provided");
        if (run.NotchType == "Notched" && (ligament is null or <= 0 || ligament >= width))
            return Reject("Measured remaining ligament must be positive and smaller than outer width");
        if (run.NotchType == "Unnotched" && !string.IsNullOrWhiteSpace(specimen.RemainingLigamentMm) && ligament != width)
            return Reject("Unnotched specimen remaining width must equal its outer width or be left blank");
        if (!string.IsNullOrWhiteSpace(specimen.NotchDepthMm))
        {
            var depth = Parse(specimen.NotchDepthMm);
            if (depth is null or < 0 || (run.NotchType == "Notched" && (depth <= 0 || Math.Abs(width.Value - ligament!.Value - depth.Value) > 0.02)))
                return Reject("Measured notch depth conflicts with measured width and remaining ligament");
            if (run.NotchType == "Unnotched" && depth != 0) return Reject("Unnotched specimen cannot have a positive notch depth");
        }
        var area = thickness.Value * (run.NotchType == "Notched" ? ligament!.Value : width.Value);
        var result = energy.Value / area * 1000d;
        return double.IsFinite(result) && area > 0 ? new(result, "E [J] / measured area [mm²] × 1000 = kJ/m²; comparative, conformity unverified") : Reject("Geometry or result outside finite numeric range");
    }

    public static string Condition(PendulumImpactRunRecord run, PendulumImpactSpecimenRecord s) =>
        IsDirect(run) ? $"{run.Method} | direct kJ/m² | {run.RunId}" :
        $"{run.Method} | {run.SpecimenType} | {run.NotchType} | {run.Orientation} | {run.Setup} | hammer: {Format(Parse(s.HammerJ))} J | overrides: {s.SettingsOverrides.Trim()}";

    public static PendulumImpactSummary Summarize(PendulumImpactRunRecord run, IEnumerable<PendulumImpactSpecimenRecord> specimens, string? batch = null)
    {
        var rows = specimens.Where(x => x.RunId == run.RunId && (batch is null || x.PrintBatch == batch)).ToArray();
        // A caller must explicitly separate different specimen-level print/conditioning/setup overrides.
        var compatible = rows.Where(x => Calculate(run, x).StrengthKjM2.HasValue).Select(x => Condition(run, x)).Distinct(StringComparer.Ordinal).Count() <= 1;
        var values = compatible ? rows.Select(x => Calculate(run, x).StrengthKjM2).Where(x => x.HasValue).Select(x => x!.Value).ToArray() : [];
        var n = values.Length;
        var statistics = new StatisticsService();
        var numericValues = values.Select(x => (double?)x).ToArray();
        var mean = statistics.Average(numericValues);
        var sd = statistics.StandardDeviationSample(numericValues);
        var cv = statistics.CoefficientOfVariation(sd, mean) * 100d;
        return new(rows.Count(x => HasReading(run, x)),
            int.TryParse(run.TargetCount, out var target) && target > 0 ? target : 10, n,
            rows.Count(x => IsDirect(run) ? IsNoBreak(x.StrengthKjM2Raw) : x.BreakType == "No break"),
            rows.Count(x => x.Status == "Invalid" || (x.Status == "Valid" &&
                (IsDirect(run) ? HasReading(run, x) && !IsNoBreak(x.StrengthKjM2Raw) : x.BreakType == "Complete") && Calculate(run, x).StrengthKjM2 is null)),
            rows.Count(x => x.Status == "Excluded"), mean, sd, cv, n == 0 ? null : values.Min(), n == 0 ? null : values.Max());
    }

    public static IReadOnlyList<PendulumImpactGroupSummary> BuildSummaries(PendulumImpactRunRecord run, IEnumerable<PendulumImpactSpecimenRecord> specimens)
    {
        var result = new List<PendulumImpactGroupSummary>();
        foreach (var condition in specimens.Where(x => x.RunId == run.RunId && HasReading(run, x)).GroupBy(x => Condition(run, x)))
        {
            result.Add(new(condition.Key, "All batches", Summarize(run, condition)));
            if (IsDirect(run)) continue;
            foreach (var batch in condition.GroupBy(x => x.PrintBatch))
                result.Add(new(condition.Key, string.IsNullOrWhiteSpace(batch.Key) ? "Unspecified batch" : batch.Key, Summarize(run, batch)));
        }
        return result;
    }

    public static IReadOnlyList<string> Validate(IEnumerable<PendulumImpactRunRecord> runs, IEnumerable<PendulumImpactSpecimenRecord> specimens)
    {
        var r = runs.ToArray(); var s = specimens.ToArray(); var errors = new List<string>();
        if (r.Any(x => string.IsNullOrWhiteSpace(x.RunId) || string.IsNullOrWhiteSpace(x.MaterialID) || !Methods.Contains(x.Method)) || r.Select(x => x.RunId).Distinct(StringComparer.OrdinalIgnoreCase).Count() != r.Length)
            errors.Add("Runs require unique IDs, canonical MaterialID and an Izod/Charpy method.");
        if (s.Any(x => string.IsNullOrWhiteSpace(x.SpecimenId) || !r.Any(y => y.RunId == x.RunId)) || s.Select(x => x.SpecimenId).Distinct(StringComparer.OrdinalIgnoreCase).Count() != s.Length)
            errors.Add("Specimens require unique IDs and a matching run.");
        if (r.Where(IsDirect).GroupBy(x => (x.MaterialID.ToUpperInvariant(), x.Method)).Any(group => group.Count() > 1))
            errors.Add("Direct measurements require one row per material and method.");
        foreach (var run in r)
        {
            if (run.InputMode is not ("Energy" or DirectStrengthInputMode)) errors.Add(run.Label + ": unsupported input mode.");
            if (IsDirect(run))
            {
                var directRows = s.Where(x => x.RunId == run.RunId).ToArray();
                if (run.TargetCount != "10" || directRows.Length != 10 ||
                    !directRows.Select(x => x.Label).OrderBy(x => x, StringComparer.Ordinal)
                        .SequenceEqual(Enumerable.Range(1, 10).Select(x => x.ToString("00", CultureInfo.InvariantCulture))))
                    errors.Add(run.Label + ": direct measurements require exactly ten sample slots numbered 01–10.");
            }
            if (!int.TryParse(run.TargetCount, out var target) || target < 1 || target > 10000) errors.Add(run.Label + ": target count must be 1–10000.");
            if (!NotchTypes.Contains(run.NotchType)) errors.Add(run.Label + ": unsupported notch state.");
            if (!string.IsNullOrWhiteSpace(run.Date) && !DateTime.TryParseExact(run.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)) errors.Add(run.Label + ": date must use yyyy-MM-dd.");
            void Number(string label, string text, double? minimum = null, double? maximum = null, bool integer = false, bool strictlyPositive = false)
            {
                if (string.IsNullOrWhiteSpace(text)) return;
                var n = Parse(text);
                if (n is null || (minimum.HasValue && n < minimum) || (maximum.HasValue && n > maximum) || (integer && n != Math.Truncate(n.Value)) || (strictlyPositive && n <= 0))
                    errors.Add(run.Label + ": " + label + " must be a finite value in its valid range, or blank when unknown.");
            }
            Number("nominal length", run.NominalLengthMm, strictlyPositive: true);
            Number("nominal width", run.NominalWidthMm, strictlyPositive: true);
            Number("nominal thickness", run.NominalThicknessMm, strictlyPositive: true);
            Number("nominal remaining ligament", run.NominalLigamentMm, strictlyPositive: true);
            Number("nozzle diameter", run.NozzleMm, strictlyPositive: true);
            Number("layer height", run.LayerHeightMm, strictlyPositive: true);
            Number("walls", run.Walls, 0, integer: true);
            Number("top layers", run.TopLayers, 0, integer: true);
            Number("bottom layers", run.BottomLayers, 0, integer: true);
            Number("infill", run.InfillPercent, 0, 100);
            Number("humidity", run.HumidityPercent, 0, 100);
            Number("print temperature", run.PrintTemperatureC);
            Number("bed temperature", run.BedTemperatureC);
            Number("test temperature", run.TestTemperatureC);
            if (run.DiagramBase64.Length > 5_592_408) errors.Add(run.Label + ": diagram exceeds 4 MiB.");
            else if (run.DiagramBase64.Length > 0)
            {
                try { if (Convert.FromBase64String(run.DiagramBase64).Length > 4 * 1024 * 1024) errors.Add(run.Label + ": diagram exceeds 4 MiB."); }
                catch (FormatException) { errors.Add(run.Label + ": diagram is not valid base64."); }
            }
        }
        foreach (var row in s)
        {
            if (!Statuses.Contains(row.Status) || !BreakTypes.Contains(row.BreakType)) errors.Add(row.Label + ": unknown validity or break classification.");
            var parent = r.FirstOrDefault(x => x.RunId == row.RunId);
            if (parent is not null && IsDirect(parent) && !string.IsNullOrWhiteSpace(row.StrengthKjM2Raw) &&
                !IsNoBreak(row.StrengthKjM2Raw) && Parse(row.StrengthKjM2Raw) is not >= 0)
                errors.Add(row.Label + ": enter a finite nonnegative kJ/m² reading, NB, or leave it blank.");
            if (parent is not null && !BreakChoices(parent.Method).Contains(row.BreakType)) errors.Add(row.Label + ": break classification is not supported for " + parent.Method + ".");
            if (row.Status is "Invalid" or "Excluded" && string.IsNullOrWhiteSpace(row.ExclusionReason)) errors.Add(row.Label + ": record the invalid/excluded reason.");
        }
        return errors;
    }
}
