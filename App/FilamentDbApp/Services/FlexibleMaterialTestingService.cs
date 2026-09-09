using FilamentDbApp.Models;
using System.Globalization;

namespace FilamentDbApp.Services;

public sealed class FlexibleMaterialTestingService
{
    public static double? ParseOptional(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;
        var normalized = value.Trim().Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) &&
               double.IsFinite(parsed) ? parsed : null;
    }

    public static double? CalculateStrainPercent(string? displacementMm, string? initialHeightMm)
    {
        var displacement = ParseOptional(displacementMm);
        var height = ParseOptional(initialHeightMm);
        return displacement >= 0d && height > 0d ? displacement / height * 100d : null;
    }

    public static double? CalculateGrossCylinderAreaMm2(string? diameterMm)
    {
        var diameter = ParseOptional(diameterMm);
        return diameter > 0d ? Math.PI * diameter.Value * diameter.Value / 4d : null;
    }

    public static double? CalculateApparentStressMpa(string? forceN, string? diameterMm)
    {
        var force = ParseOptional(forceN);
        var area = CalculateGrossCylinderAreaMm2(diameterMm);
        return force >= 0d && area > 0d ? force / area : null;
    }

    public static double? CalculateForceRetentionPercent(string? laterForceN, string? referenceForceN)
    {
        var later = ParseOptional(laterForceN);
        var reference = ParseOptional(referenceForceN);
        return later >= 0d && reference > 0d ? later / reference * 100d : null;
    }

    public static double? CalculateResidualHeightLossPercent(string? initialHeightMm, string? heightAfterRestMm)
    {
        var initial = ParseOptional(initialHeightMm);
        var after = ParseOptional(heightAfterRestMm);
        return initial > 0d && after >= 0d ? (initial - after) / initial * 100d : null;
    }

    public static IReadOnlyList<string> Validate(
        IEnumerable<FlexibleTestSpecimenRecord> specimens,
        IEnumerable<CompressionPointRecord> compression,
        IEnumerable<StressRelaxationPointRecord> relaxation,
        IEnumerable<RecoveryMeasurementRecord> recovery,
        IEnumerable<ShoreHardnessReadingRecord> shore)
    {
        var errors = new List<string>();
        var specimenIds = specimens.Select(x => x.SpecimenId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var specimen in specimens)
        {
            if (string.IsNullOrWhiteSpace(specimen.SpecimenId) || string.IsNullOrWhiteSpace(specimen.FlexibleTestSessionId))
                errors.Add("Every flexible-test specimen requires stable specimen and test-session identities.");
            ValidatePositiveOptional(specimen.DiameterMm, "Specimen diameter", errors);
            ValidatePositiveOptional(specimen.InitialHeightMm, "Specimen initial height", errors);
            ValidatePositiveOptional(specimen.ThicknessMm, "Specimen thickness", errors);
            ValidateRangeOptional(specimen.InfillPercent, 0d, 100d, "Infill", errors);
        }
        foreach (var point in compression)
        {
            ValidateParent(point.SpecimenId, specimenIds, errors);
            ValidateNonNegativeOptional(point.DisplacementMm, "Compression displacement", errors);
            ValidateNonNegativeOptional(point.ForceN, "Compression force", errors);
            ValidateNonNegativeOptional(point.HoldTimeSeconds, "Compression hold time", errors);
            ValidateRangeOptional(point.TargetStrainPercent, 0d, 100d, "Target strain", errors);
            if (point.TargetReached && ParseOptional(point.DisplacementMm) is null)
                errors.Add("A reached compression target requires a displacement; leave it blank only when Target reached is cleared.");
        }
        foreach (var point in relaxation)
        {
            ValidateParent(point.SpecimenId, specimenIds, errors);
            ValidateRangeOptional(point.CompressionPercent, 0d, 100d, "Relaxation compression", errors);
            ValidateNonNegativeOptional(point.ElapsedTimeSeconds, "Relaxation time", errors);
            ValidateNonNegativeOptional(point.ForceN, "Relaxation force", errors);
        }
        foreach (var row in recovery)
        {
            ValidateParent(row.SpecimenId, specimenIds, errors);
            ValidatePositiveOptional(row.InitialHeightMm, "Recovery initial height", errors);
            ValidateNonNegativeOptional(row.HeightAfterRestMm, "Recovered height", errors);
            ValidateNonNegativeOptional(row.RestTimeSeconds, "Recovery rest time", errors);
            ValidateRangeOptional(row.CompressionPercent, 0d, 100d, "Recovery compression", errors);
            ValidateNonNegativeOptional(row.CompressionHoldSeconds, "Compression hold duration", errors);
        }
        foreach (var row in shore)
        {
            ValidateParent(row.SpecimenId, specimenIds, errors);
            if (row.ShoreScale is not ("A" or "D")) errors.Add("Shore scale must be A or D.");
            ValidateRangeOptional(row.HardnessValue, 0d, 100d, "Shore hardness", errors);
            ValidateNonNegativeOptional(row.ReadingTimeSeconds, "Shore reading time", errors);
            ValidatePositiveOptional(row.SpecimenThicknessMm, "Shore specimen thickness", errors);
        }
        return errors.Distinct(StringComparer.Ordinal).ToList();
    }

    public static void Recalculate(
        IReadOnlyCollection<FlexibleTestSpecimenRecord> specimens,
        IReadOnlyCollection<CompressionPointRecord> compression,
        IReadOnlyCollection<StressRelaxationPointRecord> relaxation,
        IReadOnlyCollection<RecoveryMeasurementRecord> recovery)
    {
        var specimenById = specimens.ToDictionary(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase);
        foreach (var point in compression)
        {
            specimenById.TryGetValue(point.SpecimenId, out var specimen);
            var strain = CalculateStrainPercent(point.DisplacementMm, specimen?.InitialHeightMm);
            var stress = CalculateApparentStressMpa(point.ForceN, specimen?.DiameterMm);
            point.StrainPercent = Format(strain);
            point.ApparentStressMpa = Format(stress, "0.000");
            var target = ParseOptional(point.TargetStrainPercent);
            point.ResultLabel = !point.TargetReached && target.HasValue
                ? $"Target {target:0.##}% not reached"
                : target.HasValue && ParseOptional(point.ForceN).HasValue
                    ? $"Compression Force at {target:0.##}% Strain ({FormatHold(point.HoldTimeSeconds)})"
                    : string.Empty;
        }
        foreach (var group in relaxation.GroupBy(x => new { x.SpecimenId, x.CycleNumber, x.CompressionPercent }))
        {
            var ordered = group.Where(x => ParseOptional(x.ElapsedTimeSeconds).HasValue && ParseOptional(x.ForceN).HasValue)
                .OrderBy(x => ParseOptional(x.ElapsedTimeSeconds)).ToList();
            var reference = ordered.FirstOrDefault();
            foreach (var point in group)
                point.ForceRetentionPercent = reference is null || ReferenceEquals(point, reference)
                    ? string.Empty
                    : Format(CalculateForceRetentionPercent(point.ForceN, reference.ForceN));
        }
        foreach (var row in recovery)
            row.ResidualHeightLossPercent = Format(CalculateResidualHeightLossPercent(row.InitialHeightMm, row.HeightAfterRestMm));
    }

    public static IReadOnlyList<FlexibleComparisonRow> BuildComparisons(
        IReadOnlyCollection<FlexibleTestSpecimenRecord> specimens,
        IReadOnlyCollection<CompressionPointRecord> compression,
        IReadOnlyCollection<ShoreHardnessReadingRecord> shore)
    {
        var specimenById = specimens.ToDictionary(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase);
        var rows = new List<FlexibleComparisonRow>();
        var compressionGroups = compression.Where(x => ParseOptional(x.TargetStrainPercent).HasValue)
            .GroupBy(x =>
            {
                specimenById.TryGetValue(x.SpecimenId, out var s);
                return new { Method = MethodKey(s), Target = NumericKey(x.TargetStrainPercent), Hold = NumericKey(x.HoldTimeSeconds), x.CycleNumber };
            });
        foreach (var group in compressionGroups)
        {
            var notReached = group.Count(x => !x.TargetReached);
            var specimenForces = group.Where(x => x.TargetReached && ParseOptional(x.ForceN).HasValue)
                .GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Average(p => ParseOptional(p.ForceN)!.Value)).ToList();
            AddSummary(rows, $"Compression Force at {group.Key.Target}% Strain", group.Key.Method,
                $"{group.Key.Hold} s hold · cycle {group.Key.CycleNumber}", specimenForces, "N", notReached);
            var specimenStresses = group.Where(x => x.TargetReached && ParseOptional(x.ApparentStressMpa).HasValue)
                .GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Average(p => ParseOptional(p.ApparentStressMpa)!.Value)).ToList();
            AddSummary(rows, $"Apparent Compressive Stress at {group.Key.Target}% Strain", group.Key.Method,
                $"{group.Key.Hold} s hold · cycle {group.Key.CycleNumber}", specimenStresses, "MPa", notReached);
        }
        var shoreGroups = shore.Where(x => ParseOptional(x.HardnessValue).HasValue && x.ShoreScale is "A" or "D")
            .GroupBy(x =>
            {
                specimenById.TryGetValue(x.SpecimenId, out var s);
                return new { Method = MethodKey(s), x.ShoreScale, Time = NumericKey(x.ReadingTimeSeconds), Thickness = NumericKey(x.SpecimenThicknessMm) };
            });
        foreach (var group in shoreGroups)
        {
            var independentSpecimens = group.GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Average(p => ParseOptional(p.HardnessValue)!.Value)).ToList();
            AddSummary(rows, $"Shore {group.Key.ShoreScale} Hardness", group.Key.Method,
                $"{group.Key.Time} s reading · {group.Key.Thickness} mm", independentSpecimens, $"Shore {group.Key.ShoreScale}", 0);
        }
        return rows.OrderBy(x => x.Metric, StringComparer.CurrentCultureIgnoreCase).ThenBy(x => x.MethodGroup).ToList();
    }

    private static string MethodKey(FlexibleTestSpecimenRecord? s) => s is null ? "Unknown method" :
        $"{s.Shape} {NumericKey(s.DiameterMm)} x {NumericKey(s.InitialHeightMm)} mm; " +
        $"{NumericKey(s.InfillPercent)}% {s.InfillPattern}; nozzle {NumericKey(s.NozzleDiameterMm)} mm; layer {NumericKey(s.LayerHeightMm)} mm; " +
        $"walls {NumericKey(s.Perimeters)}; top/bottom {NumericKey(s.TopLayers)}/{NumericKey(s.BottomLayers)}; " +
        $"temp {NumericKey(s.PrintTemperatureC)} C; EM {NumericKey(s.ExtrusionMultiplier)}; settings {Normalize(s.PrintSettings)}; {s.MethodVersion}";

    private static void AddSummary(List<FlexibleComparisonRow> rows, string metric, string method, string condition,
        IReadOnlyList<double> values, string unit, int notReached)
    {
        if (values.Count == 0 && notReached == 0) return;
        var mean = values.Count == 0 ? string.Empty : values.Average().ToString("0.###", CultureInfo.CurrentCulture);
        var sd = values.Count < 2 ? string.Empty : Math.Sqrt(values.Sum(x => Math.Pow(x - values.Average(), 2d)) / (values.Count - 1))
            .ToString("0.###", CultureInfo.CurrentCulture);
        rows.Add(new FlexibleComparisonRow(metric, method, condition, values.Count, mean, sd, unit, notReached));
    }

    private static string Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? "not recorded" : value.Trim();
    private static string NumericKey(string? value) => ParseOptional(value) is double parsed
        ? parsed.ToString("G17", CultureInfo.InvariantCulture)
        : Normalize(value);
    private static string Format(double? value, string format = "0.00") => value.HasValue ? value.Value.ToString(format, CultureInfo.CurrentCulture) : string.Empty;
    private static string FormatHold(string? seconds) => ParseOptional(seconds) is double value ? $"{value:0.##} s hold" : "hold not recorded";
    private static void ValidateParent(string id, HashSet<string> ids, List<string> errors) { if (!ids.Contains(id)) errors.Add("A flexible-test child row references a missing specimen."); }
    private static void ValidatePositiveOptional(string? value, string name, List<string> errors) { if (!string.IsNullOrWhiteSpace(value) && ParseOptional(value) is not > 0d) errors.Add($"{name} must be greater than zero or blank."); }
    private static void ValidateNonNegativeOptional(string? value, string name, List<string> errors) { if (!string.IsNullOrWhiteSpace(value) && ParseOptional(value) is not >= 0d) errors.Add($"{name} must be zero or greater, or blank."); }
    private static void ValidateRangeOptional(string? value, double min, double max, string name, List<string> errors) { var parsed = ParseOptional(value); if (!string.IsNullOrWhiteSpace(value) && (!parsed.HasValue || parsed < min || parsed > max)) errors.Add($"{name} must be {min:0.##}-{max:0.##} or blank."); }
}
