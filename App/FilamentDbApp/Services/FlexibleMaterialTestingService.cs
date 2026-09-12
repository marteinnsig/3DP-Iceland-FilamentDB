using FilamentDbApp.Models;
using System.Globalization;

namespace FilamentDbApp.Services;

public sealed class FlexibleMaterialTestingService
{
    public const string CompressionMethodName = "3DPIceland Labs TPU Compression Test";
    public const string CompressionMethodVersion = "3DP-TPU-COMP-v1.0";
    public const string CompressionMethodNotes = "Comparative in-house test; SAUTER TVL + FK 500; Z compression; approximately 15 s manual approach; not ASTM D575 or ISO 7743.";

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

    public static double? CalculateForceReductionPercent(string? force10SecondsN, string? force30SecondsN)
    {
        var first = ParseOptional(force10SecondsN);
        var later = ParseOptional(force30SecondsN);
        return first > 0d && later >= 0d ? (first - later) / first * 100d : null;
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
            if (point.TargetReached && ParseOptional(point.ForceN).HasValue && ParseOptional(point.DisplacementMm) is null)
                errors.Add("A reached compression reading with force requires a displacement; enter the measured displacement or clear Target reached for a force-limited outcome.");
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
            if (row.TvlContactOffsetError.Length != 0) errors.Add(row.TvlContactOffsetError);
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
                    ? $"Compression Force at {target:0.##}% Strain — {FormatHold(point.HoldTimeSeconds)} (N)"
                    : string.Empty;
        }
        // Use factual, constant-displacement holds only. Numeric keys accept equivalent localized input.
        // Calculated-property notification refreshes dependent rows without rebinding the active editor.
        foreach (var point in compression) point.ForceRetentionPercent = string.Empty;
        foreach (var result in CalculateCompressionRetentions(compression))
            result.Point.ForceRetentionPercent = Format(result.Retention);
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
            row.ResidualHeightLossPercent = row.TvlContactOffsetError.Length == 0
                ? Format(CalculateResidualHeightLossPercent(row.InitialHeightMm, row.HeightAfterRestMm)) : string.Empty;
    }

    private static IEnumerable<(CompressionPointRecord Point, CompressionPointRecord Reference, double Retention)>
        CalculateCompressionRetentions(IEnumerable<CompressionPointRecord> compression)
    {
        var retentionPoints = compression.Where(x => x.TargetReached && x.CycleNumber > 0 &&
            ParseOptional(x.TargetStrainPercent) > 0d && ParseOptional(x.TargetStrainPercent) <= 100d &&
            ParseOptional(x.DisplacementMm) > 0d && ParseOptional(x.HoldTimeSeconds) >= 0d && ParseOptional(x.ForceN) >= 0d);
        foreach (var group in retentionPoints.GroupBy(x => new
        {
            x.SpecimenId, x.CycleNumber,
            Target = ParseOptional(x.TargetStrainPercent), Displacement = ParseOptional(x.DisplacementMm)
        }))
        {
            var ordered = group.OrderBy(x => ParseOptional(x.HoldTimeSeconds)).ToList();
            var reference = ordered[0];
            var referenceTime = ParseOptional(reference.HoldTimeSeconds);
            // Duplicate earliest readings are ambiguous; do not choose one arbitrarily.
            if (ordered.Count(x => ParseOptional(x.HoldTimeSeconds) == referenceTime) != 1) continue;
            foreach (var point in ordered.Skip(1))
                if (CalculateForceRetentionPercent(point.ForceN, reference.ForceN) is double retention)
                    yield return (point, reference, retention);
        }
    }

    public static IReadOnlyList<FlexibleComparisonRow> BuildComparisons(
        IReadOnlyCollection<FlexibleTestSpecimenRecord> specimens,
        IReadOnlyCollection<CompressionPointRecord> compression,
        IReadOnlyCollection<ShoreHardnessReadingRecord> shore,
        IReadOnlyCollection<RecoveryMeasurementRecord>? recovery = null) =>
        BuildMetricGroups(specimens, compression, shore, recovery).Select(x => new FlexibleComparisonRow(
            x.Metric, x.MethodGroup, x.Condition, x.SpecimenCount, Format(x.Mean, "0.###"),
            Format(x.StandardDeviation, "0.###"), Format(x.CoefficientOfVariation, "0.###"),
            Format(x.Minimum, "0.###"), Format(x.Maximum, "0.###"), x.Unit, x.NotReachedCount)
            { LocationStatistics = FormatShoreSpecimens(x.ShoreSpecimens) }).ToList();

    internal static IReadOnlyList<FlexibleMetricGroupSummary> BuildMetricGroups(
        IReadOnlyCollection<FlexibleTestSpecimenRecord> specimens,
        IReadOnlyCollection<CompressionPointRecord> compression,
        IReadOnlyCollection<ShoreHardnessReadingRecord> shore, IReadOnlyCollection<RecoveryMeasurementRecord>? recovery = null)
    {
        var specimenById = specimens.Where(x => !string.IsNullOrWhiteSpace(x.SpecimenId)).GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() == 1).ToDictionary(x => x.Key, x => x.Single(), StringComparer.OrdinalIgnoreCase);
        compression = compression.Where(x => specimenById.ContainsKey(x.SpecimenId)).ToArray();
        shore = shore.Where(x => specimenById.ContainsKey(x.SpecimenId)).ToArray();
        var rows = new List<FlexibleMetricGroupSummary>();
        var compressionGroups = compression.Where(x => x.CycleNumber > 0 && ParseOptional(x.TargetStrainPercent) is > 0d and <= 100d && ParseOptional(x.HoldTimeSeconds) >= 0d)
            .GroupBy(x =>
            {
                specimenById.TryGetValue(x.SpecimenId, out var s);
                return new { Method = MethodKey(s), Target = NumericKey(x.TargetStrainPercent), Hold = NumericKey(x.HoldTimeSeconds), Displacement = NumericKey(x.DisplacementMm), x.CycleNumber };
            });
        foreach (var group in compressionGroups)
        {
            var notReached = group.Count(x => !x.TargetReached);
            var specimenForces = group.Where(x => x.TargetReached && ParseOptional(x.ForceN) >= 0d)
                .GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Average(p => ParseOptional(p.ForceN)!.Value)).ToList();
            AddSummary(rows, $"Compression Force at {group.Key.Target}% Strain", group.Key.Method,
                $"{group.Key.Hold} s hold · {group.Key.Displacement} mm displacement · cycle {group.Key.CycleNumber}", specimenForces, "N", notReached);
            var specimenStresses = group.Where(x => x.TargetReached && CalculateApparentStressMpa(x.ForceN, specimenById[x.SpecimenId].DiameterMm).HasValue)
                .GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Average(p => CalculateApparentStressMpa(p.ForceN, specimenById[p.SpecimenId].DiameterMm)!.Value)).ToList();
            AddSummary(rows, $"Apparent Compressive Stress at {group.Key.Target}% Strain", group.Key.Method,
                $"{group.Key.Hold} s hold · {group.Key.Displacement} mm displacement · cycle {group.Key.CycleNumber}", specimenStresses, "MPa", notReached);
        }
        var retentionGroups = CalculateCompressionRetentions(compression)
            .Where(x => specimenById.ContainsKey(x.Point.SpecimenId))
            .GroupBy(x => new
            {
                Method = MethodKey(specimenById[x.Point.SpecimenId]),
                Target = NumericKey(x.Point.TargetStrainPercent),
                Displacement = NumericKey(x.Point.DisplacementMm),
                x.Point.CycleNumber,
                ReferenceTime = NumericKey(x.Reference.HoldTimeSeconds),
                LaterTime = NumericKey(x.Point.HoldTimeSeconds)
            });
        foreach (var group in retentionGroups)
        {
            var perSpecimen = group.GroupBy(x => x.Point.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Average(reading => reading.Retention)).ToList();
            AddSummary(rows, $"Force retention at {group.Key.Target}% Strain", group.Key.Method,
                $"{group.Key.ReferenceTime}–{group.Key.LaterTime} s · {group.Key.Displacement} mm displacement · cycle {group.Key.CycleNumber}",
                perSpecimen, "%", 0);
        }
        var reductionGroups = compression
            .Where(x => x.TargetReached && x.CycleNumber == 1 && ParseOptional(x.ForceN) >= 0d && ParseOptional(x.TargetStrainPercent) is > 0d and <= 100d)
            .GroupBy(x =>
            {
                specimenById.TryGetValue(x.SpecimenId, out var specimen);
                return new { Method = MethodKey(specimen), Target = NumericKey(x.TargetStrainPercent), Displacement = NumericKey(x.DisplacementMm) };
            });
        foreach (var group in reductionGroups)
        {
            var reductions = group.GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .Select(specimenRows =>
                {
                    var at10 = specimenRows.Where(x => ParseOptional(x.HoldTimeSeconds) == 10d).ToList();
                    var at30 = specimenRows.Where(x => ParseOptional(x.HoldTimeSeconds) == 30d).ToList();
                    return at10.Count == 1 && at30.Count == 1 && ParseOptional(at10[0].DisplacementMm) > 0d &&
                        ParseOptional(at10[0].DisplacementMm) == ParseOptional(at30[0].DisplacementMm)
                        ? CalculateForceReductionPercent(at10[0].ForceN, at30[0].ForceN) : null;
                })
                .Where(x => x.HasValue).Select(x => x!.Value).ToList();
            AddSummary(rows, $"Force reduction from 10 s to 30 s at {group.Key.Target}% Strain", group.Key.Method,
                $"first compression only · {group.Key.Displacement} mm displacement", reductions, "%", 0);
        }
        var shoreGroups = shore.Where(x => ParseOptional(x.HardnessValue) is >= 0d and <= 100d && x.ShoreScale is "A" or "D")
            .GroupBy(x =>
            {
                specimenById.TryGetValue(x.SpecimenId, out var s);
                return new { Method = MethodKey(s), x.ShoreScale, Time = NumericKey(x.ReadingTimeSeconds), Thickness = NumericKey(x.SpecimenThicknessMm) };
            });
        foreach (var group in shoreGroups)
        {
            var locations = group.GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .OrderBy(x => x.Key, StringComparer.OrdinalIgnoreCase)
                .Select(x =>
                {
                    var values = x.Select(p => ParseOptional(p.HardnessValue)!.Value).ToArray();
                    var mean = values.Average();
                    double? sd = values.Length < 2 ? null : Math.Sqrt(values.Sum(v => Math.Pow(v - mean, 2)) / (values.Length - 1));
                    return new ShoreSpecimenStatistics(x.Key, specimenById[x.Key].SpecimenLabel, values.Length,
                        mean, sd, mean == 0 ? null : sd / Math.Abs(mean) * 100);
                }).ToArray();
            var independentSpecimens = locations.Select(x => x.Mean).ToList();
            AddSummary(rows, $"Shore {group.Key.ShoreScale} Hardness", group.Key.Method,
                $"{group.Key.Time} s reading · {group.Key.Thickness} mm", independentSpecimens, $"Shore {group.Key.ShoreScale}", 0);
            rows[^1] = rows[^1] with { ShoreSpecimens = locations };
        }
        var recoveryGroups = (recovery ?? []).Where(x => specimenById.ContainsKey(x.SpecimenId) &&
            x.CycleNumber > 0 && x.TvlContactOffsetError.Length == 0 && ParseOptional(x.CompressionPercent) is > 0d and <= 100d &&
            ParseOptional(x.CompressionHoldSeconds) >= 0d && ParseOptional(x.RestTimeSeconds) >= 0d &&
            CalculateResidualHeightLossPercent(x.InitialHeightMm, x.HeightAfterRestMm).HasValue)
            .GroupBy(x => new { Method = MethodKey(specimenById[x.SpecimenId]),
                Compression = NumericKey(x.CompressionPercent), Hold = NumericKey(x.CompressionHoldSeconds),
                Rest = NumericKey(x.RestTimeSeconds), Height = NumericKey(x.InitialHeightMm), x.CycleNumber });
        foreach (var group in recoveryGroups)
        {
            var values = group.GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase)
                .Select(x => x.Average(p => CalculateResidualHeightLossPercent(p.InitialHeightMm, p.HeightAfterRestMm)!.Value)).ToList();
            AddSummary(rows, "Residual height loss after recovery", group.Key.Method,
                $"{group.Key.Compression}% compression · {group.Key.Hold} s compressed hold · {group.Key.Rest} s rest · {group.Key.Height} mm initial height · cycle {group.Key.CycleNumber}",
                values, "%", 0);
        }
        return rows.OrderBy(x => x.Metric, StringComparer.CurrentCultureIgnoreCase).ThenBy(x => x.MethodGroup).ToList();
    }

    public static TpuCompressionPublicSummary? BuildPublicMethodV1Summary(
        string materialId,
        IReadOnlyCollection<FlexibleTestSessionRecord> sessions,
        IReadOnlyCollection<FlexibleTestSpecimenRecord> specimens,
        IReadOnlyCollection<CompressionPointRecord> compression)
    {
        var sessionIds = sessions.Where(x => x.IsActive && string.Equals(x.MaterialID, materialId, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.FlexibleTestSessionId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var eligibleSpecimens = specimens.Where(x => sessionIds.Contains(x.FlexibleTestSessionId) &&
            string.Equals(x.MethodVersion, CompressionMethodVersion, StringComparison.Ordinal) &&
            ParseOptional(x.DiameterMm) == 9d && ParseOptional(x.InitialHeightMm) == 10d &&
            ParseOptional(x.InfillPercent) == 100d && string.Equals(x.InfillPattern?.Trim(), "Rectilinear", StringComparison.OrdinalIgnoreCase))
            .Select(x => x.SpecimenId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        List<double> ValuesAt(double hold) => compression.Where(x => eligibleSpecimens.Contains(x.SpecimenId) && x.CycleNumber == 1 &&
                x.TargetReached && ParseOptional(x.TargetStrainPercent) == 20d && ParseOptional(x.HoldTimeSeconds) == hold && ParseOptional(x.ForceN).HasValue)
            .GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase).Select(x => x.Average(p => ParseOptional(p.ForceN)!.Value)).ToList();
        var at30 = ValuesAt(30d);
        if (at30.Count == 0) return null;
        var at10 = ValuesAt(10d);
        static double Sd(IReadOnlyList<double> values) => values.Count < 2 ? double.NaN : Math.Sqrt(values.Sum(x => Math.Pow(x - values.Average(), 2d)) / (values.Count - 1));
        static double Cv(IReadOnlyList<double> values) { var sd = Sd(values); return values.Count < 2 || values.Average() == 0d ? double.NaN : sd / values.Average() * 100d; }
        return new TpuCompressionPublicSummary(at30.Count, at30.Average(), Sd(at30), Cv(at30),
            at10.Count, at10.Count == 0 ? double.NaN : at10.Average(), Sd(at10), Cv(at10));
    }

    private static string MethodKey(FlexibleTestSpecimenRecord? s) => s is null ? "Unknown method" :
        $"{s.Shape} {NumericKey(s.DiameterMm)} x {NumericKey(s.InitialHeightMm)} mm; " +
        $"{NumericKey(s.InfillPercent)}% {s.InfillPattern}; nozzle {NumericKey(s.NozzleDiameterMm)} mm; layer {NumericKey(s.LayerHeightMm)} mm; " +
        $"walls {NumericKey(s.Perimeters)}; top/bottom {NumericKey(s.TopLayers)}/{NumericKey(s.BottomLayers)}; " +
        $"temp {NumericKey(s.PrintTemperatureC)} C; EM {NumericKey(s.ExtrusionMultiplier)}; settings {Normalize(s.PrintSettings)}; {s.MethodVersion}; " +
        $"thickness {NumericKey(s.ThicknessMm)} mm; method notes {Normalize(s.MethodNotes)}";

    private static void AddSummary(List<FlexibleMetricGroupSummary> rows, string metric, string method, string condition,
        IReadOnlyList<double> values, string unit, int notReached)
    {
        var finite = values.Where(double.IsFinite).ToArray();
        if (finite.Length == 0 && notReached == 0) return;
        double? mean = finite.Length == 0 ? null : finite.Average();
        double? sd = finite.Length < 2 ? null : Math.Sqrt(finite.Sum(x => Math.Pow(x - mean!.Value, 2d)) / (finite.Length - 1));
        double? cv = mean is null or 0d ? null : sd / Math.Abs(mean.Value) * 100d;
        if (mean.HasValue && !double.IsFinite(mean.Value)) mean = null;
        if (sd.HasValue && !double.IsFinite(sd.Value)) sd = null;
        if (cv.HasValue && !double.IsFinite(cv.Value)) cv = null;
        var kind = metric.StartsWith("Compression Force", StringComparison.Ordinal) ? FlexibleMetricKind.CompressionForce :
            metric.StartsWith("Apparent", StringComparison.Ordinal) ? FlexibleMetricKind.ApparentCompressiveStress :
            metric.StartsWith("Force retention", StringComparison.Ordinal) ? FlexibleMetricKind.ForceRetention :
            metric.StartsWith("Force reduction", StringComparison.Ordinal) ? FlexibleMetricKind.ForceReduction :
            metric.StartsWith("Residual", StringComparison.Ordinal) ? FlexibleMetricKind.ResidualHeightLoss :
            unit == "Shore A" ? FlexibleMetricKind.ShoreA : FlexibleMetricKind.ShoreD;
        var key = string.Concat(new[] { "flexible-v1", kind.ToString(), metric, method, condition, unit }
            .Select(value => value.Length.ToString(CultureInfo.InvariantCulture) + ":" + value));
        rows.Add(new FlexibleMetricGroupSummary(kind, metric, method, condition, key, finite.Length,
            mean, sd, cv, finite.Length == 0 ? null : finite.Min(), finite.Length == 0 ? null : finite.Max(), unit, notReached));
    }

    private static string Normalize(string? value) => string.IsNullOrWhiteSpace(value) ? "not recorded" : value.Trim();
    public static string FormatShoreSpecimens(IReadOnlyList<ShoreSpecimenStatistics> specimens) =>
        string.Join("\n", specimens.Select(s => $"{s.SpecimenLabel}: {s.ReadingCount} readings; mean {Format(s.Mean, "0.###")}; location SD {Format(s.StandardDeviation, "0.###")}; location CV {Format(s.CoefficientOfVariation, "0.###")}%"));
    private static string NumericKey(string? value) => ParseOptional(value) is double parsed
        ? parsed.ToString("R", CultureInfo.InvariantCulture)
        : Normalize(value);
    private static string Format(double? value, string format = "0.00") => value.HasValue ? value.Value.ToString(format, CultureInfo.CurrentCulture) : string.Empty;
    private static string FormatHold(string? seconds) => ParseOptional(seconds) is double value ? $"{value:0.##} s hold" : "hold not recorded";
    private static void ValidateParent(string id, HashSet<string> ids, List<string> errors) { if (!ids.Contains(id)) errors.Add("A flexible-test child row references a missing specimen."); }
    private static void ValidatePositiveOptional(string? value, string name, List<string> errors) { if (!string.IsNullOrWhiteSpace(value) && ParseOptional(value) is not > 0d) errors.Add($"{name} must be greater than zero or blank."); }
    private static void ValidateNonNegativeOptional(string? value, string name, List<string> errors) { if (!string.IsNullOrWhiteSpace(value) && ParseOptional(value) is not >= 0d) errors.Add($"{name} must be zero or greater, or blank."); }
    private static void ValidateRangeOptional(string? value, double min, double max, string name, List<string> errors) { var parsed = ParseOptional(value); if (!string.IsNullOrWhiteSpace(value) && (!parsed.HasValue || parsed < min || parsed > max)) errors.Add($"{name} must be {min:0.##}-{max:0.##} or blank."); }
}
