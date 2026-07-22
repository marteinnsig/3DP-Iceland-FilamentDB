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
        var tensileRows = new List<Dictionary<string, object?>>();
        var impactRows = new List<Dictionary<string, object?>>();
        var stiffnessRows = new List<Dictionary<string, object?>>();

        foreach (var material in materials)
        {
            var common = new Dictionary<string, object?>(material.CommonFields);
            var summary = material.Summary;

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
        }

        return new WebsiteChartPayload(tensileRows, impactRows, stiffnessRows);
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
    [property: JsonPropertyName("stiffness")] IReadOnlyList<Dictionary<string, object?>> Stiffness);
