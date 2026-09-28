using FilamentDbApp.Models;
using System.Globalization;
using FilamentDbApp.Services.Calculations;

namespace FilamentDbApp.Services;

public sealed class EngineeringScoringService
{
    // Fixed references; legacy Impact is excluded from every Overall input.
    private const double TensileReferenceMpa = 80.0;
    public const string PolicyVersion = "engineering-score-v70-1";
    private const double StiffnessReferenceMpa = 3000.0;

    public EngineeringScoreProfile BuildProfile(
        TensileTestResult? tensile,
        IReadOnlyList<TestSummaryMetric> metrics,
        double? thermalResultTemperatureC = null)
    {
        var tensileFlat = ParseMetric(tensile?.FlatMpa);
        var tensileUpright = ParseMetric(tensile?.UprightMpa);
        var tensileMean = AverageAvailable(tensileFlat, tensileUpright);
        var tensileScore = Normalize(tensileMean, TensileReferenceMpa);
        double? impactScore = null; // Historical Impact has no modern engineering score.

        var stiffness = ParseMetric(FindMetric(metrics, "Stiffness", "Modulus")?.MetricValue);
        var stiffnessScore = Normalize(stiffness, StiffnessReferenceMpa);

        var izodScore = ReadIndependentScore(metrics, "Izod");
        var charpyScore = ReadIndependentScore(metrics, "Charpy");
        var consistencyScore = ConsistencyCalibrationService.CalculateScore(
            new double?[]
            {
                ParseCvPercent(tensile?.CvFlat), ParseCvPercent(tensile?.CvUpright),
                ReadIndependentCv(metrics, "Izod"), ReadIndependentCv(metrics, "Charpy")
            },
            new double?[]
            {
                PositiveSampleCount(ParseMetric(tensile?.SamplesFlat)), PositiveSampleCount(ParseMetric(tensile?.SamplesUpright)),
                ReadIndependentSamples(metrics, "Izod"), ReadIndependentSamples(metrics, "Charpy")
            });
        var layerAdhesionScore = LayerAdhesionScore(tensileUpright, tensileFlat);

        var impactFamily = CombineImpactScores(izodScore, charpyScore);
        var overall = CompleteOverall(tensileScore, stiffnessScore, consistencyScore, layerAdhesionScore, impactFamily);
        var thermal = ThermalAnalyticsService.Project(thermalResultTemperatureC);

        return new EngineeringScoreProfile
        {
            TensileScore = tensileScore,
            ImpactScore = impactScore,
            LegacyImpactRadarPercent = LegacyImpactRadarService.Calculate(
                ParseMetric(FindMetric(metrics, "Impact", "Upright kJ")?.MetricValue),
                PositiveSampleCount(ParseMetric(FindMetric(metrics, "Impact", "Upright Samples")?.MetricValue)) is null ? 0 : 1,
                ParseMetric(FindMetric(metrics, "Impact", "Flat kJ")?.MetricValue),
                PositiveSampleCount(ParseMetric(FindMetric(metrics, "Impact", "Flat Samples")?.MetricValue)) is null ? 0 : 1,
                ParseMetric(FindMetric(metrics, "Impact", "Radar maximum kJ/m²")?.MetricValue)),
            ImpactFamilyScore = impactFamily,
            IzodScore = izodScore,
            CharpyScore = charpyScore,
            StiffnessScore = stiffnessScore,
            ConsistencyScore = consistencyScore,
            LayerAdhesionScore = layerAdhesionScore,
            ThermalScore = thermal?.Score,
            ThermalResultTemperatureC = thermal?.ResultTemperatureC,
            MeasuredImpactMethodCount = (ReadIndependentSamples(metrics, "Izod").HasValue ? 1 : 0) + (ReadIndependentSamples(metrics, "Charpy").HasValue ? 1 : 0),
            OverallScore = overall
        };
    }

    public EngineeringScoreProfile BuildProfile(MaterialResults? summary, double? thermalResultTemperatureC = null)
    {
        if (summary is null) return new EngineeringScoreProfile();

        var tensileFlat = summary.Tensile?.Flat.SampleCount > 0 ? summary.Tensile.Flat.Average : null;
        var tensileUpright = summary.Tensile?.Upright.SampleCount > 0 ? summary.Tensile.Upright.Average : null;
        var stiffness = summary.HasStiffnessResults ? summary.Stiffness?.ModulusMpa : null;

        var tensileScore = Normalize(AverageAvailable(tensileFlat, tensileUpright), TensileReferenceMpa);
        double? impactScore = null; // Historical Impact is reference-only.
        var stiffnessScore = Normalize(stiffness, StiffnessReferenceMpa);
        var izodScore = summary.Izod?.Score;
        var charpyScore = summary.Charpy?.Score;
        var consistencyScore = ConsistencyCalibrationService.CalculateScore(
            new double?[]
            {
                ToCvPercent(summary.Tensile?.Flat),
                ToCvPercent(summary.Tensile?.Upright),
                summary.Izod?.HasResults == true ? summary.Izod.Statistics.CvPercent : null,
                summary.Charpy?.HasResults == true ? summary.Charpy.Statistics.CvPercent : null
            },
            new double?[]
            {
                PositiveSampleCount(summary.Tensile?.Flat.SampleCount),
                PositiveSampleCount(summary.Tensile?.Upright.SampleCount),
                summary.Izod?.HasResults == true ? PositiveSampleCount(summary.Izod.Statistics.ValidCount) : null,
                summary.Charpy?.HasResults == true ? PositiveSampleCount(summary.Charpy.Statistics.ValidCount) : null
            });
        var layerAdhesionScore = LayerAdhesionScore(tensileUpright, tensileFlat);
        var impactFamily = CombineImpactScores(izodScore, charpyScore);
        var overall = CompleteOverall(tensileScore, stiffnessScore, consistencyScore, layerAdhesionScore, impactFamily);
        var thermal = ThermalAnalyticsService.Project(thermalResultTemperatureC);

        return new EngineeringScoreProfile
        {
            TensileScore = tensileScore,
            ImpactScore = impactScore,
            LegacyImpactRadarPercent = LegacyImpactRadarService.Calculate(summary.Impact?.Upright.Average,
                summary.Impact?.Upright.SampleCount ?? 0, summary.Impact?.Flat.Average,
                summary.Impact?.Flat.SampleCount ?? 0, summary.Impact?.MaxPossibleImpact),
            ImpactFamilyScore = impactFamily,
            IzodScore = izodScore,
            CharpyScore = charpyScore,
            StiffnessScore = stiffnessScore,
            ConsistencyScore = consistencyScore,
            LayerAdhesionScore = layerAdhesionScore,
            ThermalScore = thermal?.Score,
            ThermalResultTemperatureC = thermal?.ResultTemperatureC,
            MeasuredImpactMethodCount = (summary.HasIzodResults ? 1 : 0) + (summary.HasCharpyResults ? 1 : 0),
            OverallScore = overall
        };
    }

    public static double? CombineImpactScores(double? izod, double? charpy) =>
        izod is >= 0 and <= 100 && charpy is >= 0 and <= 100 ? (izod.Value + charpy.Value) / 2d : null;

    private static double? CompleteOverall(params double?[] families) =>
        families.All(value => value is >= 0 and <= 100) ? families.Average(value => value!.Value) : null;

    private static double? ReadIndependentScore(IEnumerable<TestSummaryMetric> metrics, string method)
    {
        var rows = metrics.Where(x => string.Equals(x.TestType, method, StringComparison.OrdinalIgnoreCase)).ToArray();
        var policy = rows.FirstOrDefault(x => x.MetricName == "Score policy")?.MetricValue;
        var reference = ParseMetric(rows.FirstOrDefault(x => x.MetricName == "Score reference")?.MetricValue);
        var mean = ParseMetric(rows.FirstOrDefault(x => x.MetricName == "Mean kJ/m²")?.MetricValue);
        if (string.IsNullOrWhiteSpace(policy) || reference is not > 0 || mean is not >= 0 ||
            !double.IsFinite(reference.Value) || !double.IsFinite(mean.Value) || ReadIndependentSamples(rows, method) is null) return null;
        return Math.Clamp(mean.Value / reference.Value * 100d, 0d, 100d);
    }
    private static double? ToCvPercent(MeasurementSetResult? result)
    {
        return result is { SampleCount: > 0, CoefficientOfVariation: { } cv } && double.IsFinite(cv)
            ? Math.Abs(cv) * 100.0
            : null;
    }

    private static TestSummaryMetric? FindMetric(IEnumerable<TestSummaryMetric> metrics, params string[] terms)
    {
        return metrics.FirstOrDefault(metric =>
        {
            var haystack = $"{metric.TestType} {metric.MetricName} {metric.SourceColumn}";
            return terms.All(term => haystack.Contains(term, StringComparison.OrdinalIgnoreCase));
        });
    }

    private static double? Normalize(double? value, double reference)
    {
        if (!value.HasValue || reference <= 0) return null;
        return Math.Clamp(value.Value / reference * 100.0, 0.0, 100.0);
    }

    private static double? LayerAdhesionScore(double? uprightMpa, double? flatMpa)
    {
        if (!uprightMpa.HasValue || !flatMpa.HasValue || flatMpa.Value <= 0) return null;
        // Website radar uses the layer-adhesion ratio as a 0-100 normalized axis. The full website
        // version normalizes against the visible dataset; in the app foundation we clamp the
        // physical ratio to 0-100 until the complete comparison dataset is available.
        return Math.Clamp((uprightMpa.Value / flatMpa.Value) * 100.0, 0.0, 100.0);
    }

    private static double? PositiveSampleCount(double? value) => value is > 0 && double.IsFinite(value.Value) ? value : null;

    private static double? ReadIndependentSamples(IEnumerable<TestSummaryMetric> metrics, string method) =>
        PositiveSampleCount(ParseMetric(metrics.FirstOrDefault(x =>
            string.Equals(x.TestType, method, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.MetricName, "Samples", StringComparison.OrdinalIgnoreCase))?.MetricValue));

    private static double? ReadIndependentCv(IEnumerable<TestSummaryMetric> metrics, string method)
    {
        if (ReadIndependentSamples(metrics, method) is null) return null;
        // Direct instrument summaries store actual percent, including values below 1%.
        var value = ParseMetric(metrics.FirstOrDefault(x =>
            string.Equals(x.TestType, method, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.MetricName, "CV", StringComparison.OrdinalIgnoreCase))?.MetricValue);
        return value is >= 0 && double.IsFinite(value.Value) ? value : null;
    }
    private static double? AverageAvailable(params double?[] values)
    {
        var available = values.Where(v => v.HasValue && double.IsFinite(v.Value)).Select(v => v!.Value).ToList();
        return available.Count == 0 ? null : available.Average();
    }

    private static double? ParseCvPercent(string? value)
    {
        var parsed = ParseMetric(value);
        if (!parsed.HasValue) return null;
        return Math.Abs(parsed.Value) <= 1.0 && (value?.Contains('%') != true)
            ? parsed.Value * 100.0
            : parsed.Value;
    }

    private static double? ParseMetric(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var cleaned = value
            .Trim()
            .Replace("MPa", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("kJ/m²", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("kJ/m2", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("%", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        if (cleaned.Contains(',') && !cleaned.Contains('.'))
        {
            var normalized = cleaned.Replace(',', '.');
            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var commaNumber)) return commaNumber;
        }

        var cultures = new[]
        {
            CultureInfo.CurrentCulture,
            CultureInfo.GetCultureInfo("is-IS"),
            CultureInfo.GetCultureInfo("da-DK"),
            CultureInfo.GetCultureInfo("en-US")
        };

        foreach (var culture in cultures)
        {
            if (double.TryParse(cleaned, NumberStyles.Float, culture, out var number)) return number;
        }

        return double.TryParse(cleaned, NumberStyles.Float, CultureInfo.InvariantCulture, out var invariantNumber)
            ? invariantNumber
            : null;
    }
}
