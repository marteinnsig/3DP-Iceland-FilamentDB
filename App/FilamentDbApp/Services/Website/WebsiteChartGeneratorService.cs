using FilamentDbApp.Services.Calculations;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace FilamentDbApp.Services.Website;

public sealed class WebsiteChartGeneratorService
{
    public string BuildDataJson(IEnumerable<WebsiteChartMaterialInput> materials)
    {
        var payload = BuildPayload(materials);
        var options = new JsonSerializerOptions
        {
            DefaultIgnoreCondition = JsonIgnoreCondition.Never,
            WriteIndented = false
        };
        return JsonSerializer.Serialize(payload, options);
    }

    public WebsiteChartPayload BuildPayload(IEnumerable<WebsiteChartMaterialInput> materials)
    {
        var source = materials.ToList();
        var izodRows = new List<Dictionary<string, object?>>();
        var charpyRows = new List<Dictionary<string, object?>>();
        var tensileRows = new List<Dictionary<string, object?>>();
        var impactRows = new List<Dictionary<string, object?>>();
        var stiffnessRows = new List<Dictionary<string, object?>>();
        var thermalRows = new List<Dictionary<string, object?>>();

        foreach (var material in source)
        {
            var common = new Dictionary<string, object?>(material.CommonFields);
            var summary = material.Summary;
            var profile = new EngineeringScoringService().BuildProfile(summary, FiniteCommonNumber(common, "thermalResultTemperatureC"));
            common["izodMeanKjM2"] = summary.Izod?.MeanKjM2;
            common["charpyMeanKjM2"] = summary.Charpy?.MeanKjM2;
            common["izodScore"] = profile.IzodScore;
            common["charpyScore"] = profile.CharpyScore;
            common["tensileScore"] = profile.TensileScore;
            common["impactScore"] = profile.ImpactScore;
            common["legacyImpactRadarPercent"] = profile.LegacyImpactRadarPercent;
            common["legacyImpactRadarSource"] = profile.LegacyImpactRadarSource;
            common["impactFamilyScore"] = profile.ImpactFamilyScore;
            common["stiffnessScore"] = profile.StiffnessScore;
            common["consistencyScore"] = profile.ConsistencyScore;
            common["layerAdhesionScore"] = profile.LayerAdhesionScore;
            common["overallScore"] = profile.OverallScore;
            common["scoreCoverage"] = profile.CoverageSummary;
            common["isOverallComparable"] = profile.IsOverallComparable;
            common["scorePolicyVersion"] = profile.ScorePolicyVersion;
            izodRows.Add(BuildPendulumRow(common, summary.Izod, profile.IzodScore));
            charpyRows.Add(BuildPendulumRow(common, summary.Charpy, profile.CharpyScore));

            tensileRows.Add(new Dictionary<string, object?>(common)
            {
                ["upright"] = NumberValue(summary.Tensile?.Upright.Average),
                ["flat"] = NumberValue(summary.Tensile?.Flat.Average),
                ["uprightErr"] = NumberValue(summary.Tensile?.Upright.StandardDeviation),
                ["flatErr"] = NumberValue(summary.Tensile?.Flat.StandardDeviation),
                ["uprightCv"] = NumberValue(summary.Tensile?.Upright.CoefficientOfVariation),
                ["flatCv"] = NumberValue(summary.Tensile?.Flat.CoefficientOfVariation),
                ["uprightSamples"] = NumberValue(summary.Tensile?.Upright.SampleCount),
                ["flatSamples"] = NumberValue(summary.Tensile?.Flat.SampleCount),
                ["uprightConfidence"] = NumberValue(summary.Tensile?.Upright.Confidence),
                ["flatConfidence"] = NumberValue(summary.Tensile?.Flat.Confidence)
            });

            impactRows.Add(new Dictionary<string, object?>(common)
            {
                ["upright"] = NumberValue(summary.Impact?.Upright.Average),
                ["flat"] = NumberValue(summary.Impact?.Flat.Average),
                ["uprightErr"] = NumberValue(summary.Impact?.Upright.StandardDeviation),
                ["flatErr"] = NumberValue(summary.Impact?.Flat.StandardDeviation),
                ["uprightCv"] = NumberValue(summary.Impact?.Upright.CoefficientOfVariation),
                ["flatCv"] = NumberValue(summary.Impact?.Flat.CoefficientOfVariation),
                ["uprightSamples"] = NumberValue(summary.Impact?.Upright.SampleCount),
                ["flatSamples"] = NumberValue(summary.Impact?.Flat.SampleCount),
                ["uprightConfidence"] = NumberValue(summary.Impact?.Upright.Confidence),
                ["flatConfidence"] = NumberValue(summary.Impact?.Flat.Confidence)
            });

            stiffnessRows.Add(new Dictionary<string, object?>(common)
            {
                ["value"] = NumberValue(summary.Stiffness?.ModulusMpa)
            });

            thermalRows.Add(new Dictionary<string, object?>(common)
            {
                ["value"] = FiniteCommonNumber(common, "thermalResultTemperatureC")
            });
        }

        return new WebsiteChartPayload(tensileRows, impactRows, stiffnessRows, thermalRows) { Izod = izodRows, Charpy = charpyRows };
    }

    private static Dictionary<string, object?> BuildPendulumRow(Dictionary<string, object?> common,
        PendulumMethodResults? result, double? score) => new(common)
    {
        ["value"] = result?.MeanKjM2,
        ["standardDeviation"] = result?.Statistics.SampleStdDev,
        ["coefficientOfVariation"] = result?.Statistics.CoefficientOfVariation,
        ["samples"] = result?.Statistics.ValidCount ?? 0,
        ["confidence"] = result?.Statistics.Confidence,
        ["measuredDate"] = result?.Date,
        ["score"] = score,
        ["referenceMaximumKjM2"] = result?.ReferenceMaximumKjM2,
        ["referencePolicyVersion"] = result?.ReferencePolicyVersion,
        ["scoreStatus"] = result?.ScoreStatus ?? "Not measured"
    };

    private static double? FiniteCommonNumber(IReadOnlyDictionary<string, object?> fields, string key)
    {
        if (!fields.TryGetValue(key, out var raw) || raw is null) return null;
        var value = raw switch
        {
            double number => number,
            float number => number,
            decimal number => (double)number,
            int number => number,
            long number => number,
            _ => double.NaN
        };
        return double.IsFinite(value) ? value : null;
    }

    private static double? NumberValue(double? value) => value.HasValue && double.IsFinite(value.Value) ? value.Value : null;

    private static double? NumberValue(int? value) => value.HasValue ? value.Value : null;
}

public sealed record WebsiteChartMaterialInput(
    IReadOnlyDictionary<string, object?> CommonFields,
    MaterialResults Summary);

public sealed record WebsiteChartPayload(
    [property: JsonPropertyName("tensile")] IReadOnlyList<Dictionary<string, object?>> Tensile,
    [property: JsonPropertyName("impact")] IReadOnlyList<Dictionary<string, object?>> Impact,
    [property: JsonPropertyName("stiffness")] IReadOnlyList<Dictionary<string, object?>> Stiffness,
    [property: JsonPropertyName("thermal")] IReadOnlyList<Dictionary<string, object?>> Thermal)
{
    [JsonPropertyName("izod")] public IReadOnlyList<Dictionary<string, object?>> Izod { get; init; } = Array.Empty<Dictionary<string, object?>>();
    [JsonPropertyName("charpy")] public IReadOnlyList<Dictionary<string, object?>> Charpy { get; init; } = Array.Empty<Dictionary<string, object?>>();
}
