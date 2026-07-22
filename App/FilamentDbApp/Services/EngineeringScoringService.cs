using FilamentDbApp.Models;
using System.Globalization;
using FilamentDbApp.Services.Calculations;

namespace FilamentDbApp.Services;

public sealed class EngineeringScoringService
{
    // These references are intentionally temporary app-side references until the full website
    // ranking/radar dataset is imported. The radar axes and consistency/layer-adhesion logic
    // now match the website model: Tensile, Impact, Stiffness, Consistency, Layer Adhesion.
    private const double TensileReferenceMpa = 80.0;
    private const double ImpactReferenceKjM2 = 1000.0;
    private const double StiffnessReferenceMpa = 3000.0;

    public EngineeringScoreProfile BuildProfile(TensileTestResult? tensile, IReadOnlyList<TestSummaryMetric> metrics)
    {
        var tensileFlat = ParseMetric(tensile?.FlatMpa);
        var tensileUpright = ParseMetric(tensile?.UprightMpa);
        var tensileMean = AverageAvailable(tensileFlat, tensileUpright);
        var tensileScore = Normalize(tensileMean, TensileReferenceMpa);

        var impactFlat = ParseMetric(FindMetric(metrics, "Impact", "Flat", "kJ")?.MetricValue);
        var impactUpright = ParseMetric(FindMetric(metrics, "Impact", "Upright", "kJ")?.MetricValue);
        var impactMean = AverageAvailable(impactFlat, impactUpright);
        var impactScore = Normalize(impactMean, ImpactReferenceKjM2);

        var stiffness = ParseMetric(FindMetric(metrics, "Stiffness", "Modulus")?.MetricValue);
        var stiffnessScore = Normalize(stiffness, StiffnessReferenceMpa);

        var consistencyScore = ConsistencyScore(
            ParseCvPercent(tensile?.CvFlat),
            ParseCvPercent(tensile?.CvUpright),
            ParseCvPercent(FindMetric(metrics, "Impact", "Flat", "CV")?.MetricValue),
            ParseCvPercent(FindMetric(metrics, "Impact", "Upright", "CV")?.MetricValue),
            ParseMetric(tensile?.SamplesFlat),
            ParseMetric(tensile?.SamplesUpright),
            ParseMetric(FindMetric(metrics, "Impact", "Flat", "Samples")?.MetricValue),
            ParseMetric(FindMetric(metrics, "Impact", "Upright", "Samples")?.MetricValue));

        var layerAdhesionScore = LayerAdhesionScore(tensileUpright, tensileFlat);

        var overall = AverageAvailable(tensileScore, impactScore, stiffnessScore, consistencyScore, layerAdhesionScore);

        return new EngineeringScoreProfile
        {
            TensileScore = tensileScore,
            ImpactScore = impactScore,
            StiffnessScore = stiffnessScore,
            ConsistencyScore = consistencyScore,
            LayerAdhesionScore = layerAdhesionScore,
            OverallScore = overall
        };
    }

    public EngineeringScoreProfile BuildProfile(MaterialResults? summary)
    {
        if (summary is null) return new EngineeringScoreProfile();

        var tensileFlat = summary.Tensile?.Flat.SampleCount > 0 ? summary.Tensile.Flat.Average : null;
        var tensileUpright = summary.Tensile?.Upright.SampleCount > 0 ? summary.Tensile.Upright.Average : null;
        var impactFlat = summary.Impact?.Flat.SampleCount > 0 ? summary.Impact.Flat.Average : null;
        var impactUpright = summary.Impact?.Upright.SampleCount > 0 ? summary.Impact.Upright.Average : null;
        var stiffness = summary.HasStiffnessResults ? summary.Stiffness?.ModulusMpa : null;

        var tensileScore = Normalize(AverageAvailable(tensileFlat, tensileUpright), TensileReferenceMpa);
        var impactScore = Normalize(AverageAvailable(impactFlat, impactUpright), ImpactReferenceKjM2);
        var stiffnessScore = Normalize(stiffness, StiffnessReferenceMpa);
        var consistencyScore = ConsistencyCalibrationService.CalculateScore(
            new double?[]
            {
                ToCvPercent(summary.Tensile?.Flat),
                ToCvPercent(summary.Tensile?.Upright),
                ToCvPercent(summary.Impact?.Flat),
                ToCvPercent(summary.Impact?.Upright)
            },
            new double?[]
            {
                summary.Tensile?.Flat.SampleCount,
                summary.Tensile?.Upright.SampleCount,
                summary.Impact?.Flat.SampleCount,
                summary.Impact?.Upright.SampleCount
            });
        var layerAdhesionScore = LayerAdhesionScore(tensileUpright, tensileFlat);
        var overall = AverageAvailable(tensileScore, impactScore, stiffnessScore, consistencyScore, layerAdhesionScore);

        return new EngineeringScoreProfile
        {
            TensileScore = tensileScore,
            ImpactScore = impactScore,
            StiffnessScore = stiffnessScore,
            ConsistencyScore = consistencyScore,
            LayerAdhesionScore = layerAdhesionScore,
            OverallScore = overall
        };
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

    private static double? ConsistencyScore(
        double? tensileFlatCvPercent,
        double? tensileUprightCvPercent,
        double? impactFlatCvPercent,
        double? impactUprightCvPercent,
        double? tensileFlatSamples,
        double? tensileUprightSamples,
        double? impactFlatSamples,
        double? impactUprightSamples)
    {
        var cvValues = new[] { tensileFlatCvPercent, tensileUprightCvPercent, impactFlatCvPercent, impactUprightCvPercent }
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .Where(v => double.IsFinite(v))
            .ToList();

        if (cvValues.Count == 0) return null;

        var sampleValues = new[] { tensileFlatSamples, tensileUprightSamples, impactFlatSamples, impactUprightSamples }
            .Where(v => v.HasValue)
            .Select(v => v!.Value)
            .Where(v => double.IsFinite(v))
            .ToList();

        // Preserves the established website formula while centralizing its ownership.
        return ConsistencyCalibrationService.CalculateScore(
            cvValues.Select(value => (double?)value),
            sampleValues.Select(value => (double?)value));
    }

    private static double? AverageAvailable(params double?[] values)
    {
        var available = values.Where(v => v.HasValue).Select(v => v!.Value).ToList();
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
