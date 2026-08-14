using System.Text.Json.Serialization;

namespace FilamentDbApp.Services.Website;

public sealed class WebsiteRadarGeneratorService
{
    private static readonly string[] RequiredRadarFields =
    {
        "materialId",
        "baseMaterial",
        "type",
        "reinforcement",
        "label",
        "thermalResultTemperatureC",
        "thermalScore",
        "thermalMethodVersion",
        "thermalLimitation"
    };

    public WebsiteRadarVerificationResult VerifyRadarPayload(WebsiteChartPayload chartPayload)
    {
        var result = new WebsiteRadarVerificationResult();

        result.TensileRows = chartPayload.Tensile.Count;
        result.ImpactRows = chartPayload.Impact.Count;
        result.StiffnessRows = chartPayload.Stiffness.Count;
        result.ThermalRows = chartPayload.Thermal.Count;
        result.MaterialIds = CountDistinctMaterialIds(chartPayload.Tensile);
        result.MaterialAverageGroups = CountMaterialAverageGroups(chartPayload.Tensile);
        result.ReinforcementAverageGroups = CountReinforcementAverageGroups(chartPayload.Tensile);
        result.SelectedRadarRows = CountRowsWithRequiredFields(chartPayload.Tensile);
        result.NormalizationInputsAvailable = HasAnyMetric(chartPayload.Tensile, "upright", "flat") ||
                                              HasAnyMetric(chartPayload.Impact, "upright", "flat") ||
                                              HasAnyMetric(chartPayload.Stiffness, "value") ||
                                              HasAnyMetric(chartPayload.Thermal, "value");
        result.ThermalContractFieldsPresent = chartPayload.Tensile.All(row =>
            row.ContainsKey("thermalResultTemperatureC") && row.ContainsKey("thermalScore") &&
            row.ContainsKey("thermalMethodVersion") && row.ContainsKey("thermalLimitation"));
        result.RendererPayloadValid = result.TensileRows == result.ImpactRows &&
                                      result.TensileRows == result.StiffnessRows &&
                                      result.TensileRows == result.ThermalRows &&
                                      result.SelectedRadarRows == result.TensileRows &&
                                      result.MaterialIds == result.TensileRows &&
                                      result.NormalizationInputsAvailable &&
                                      result.ThermalContractFieldsPresent;
        result.Passed = result.RendererPayloadValid &&
                        result.MaterialAverageGroups > 0 &&
                        result.ReinforcementAverageGroups > 0;

        return result;
    }

    private static int CountRowsWithRequiredFields(IReadOnlyList<Dictionary<string, object?>> rows)
    {
        return rows.Count(row => RequiredRadarFields.All(field => row.ContainsKey(field)) && !string.IsNullOrWhiteSpace(row["materialId"]?.ToString()));
    }

    private static int CountDistinctMaterialIds(IReadOnlyList<Dictionary<string, object?>> rows)
    {
        return rows
            .Select(row => row.TryGetValue("materialId", out var value) ? value?.ToString()?.Trim() : string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
    }

    private static int CountMaterialAverageGroups(IReadOnlyList<Dictionary<string, object?>> rows)
    {
        return rows
            .Select(row => row.TryGetValue("type", out var type) ? type?.ToString()?.Trim() : string.Empty)
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
    }

    private static int CountReinforcementAverageGroups(IReadOnlyList<Dictionary<string, object?>> rows)
    {
        return rows
            .Select(row =>
            {
                var type = row.TryGetValue("type", out var typeValue) ? typeValue?.ToString()?.Trim() : string.Empty;
                var reinforcement = row.TryGetValue("reinforcement", out var reinforcementValue) ? reinforcementValue?.ToString()?.Trim() : string.Empty;
                return (string.IsNullOrWhiteSpace(type) ? "__unknown__" : type) + "|" + (string.IsNullOrWhiteSpace(reinforcement) ? "__none__" : reinforcement);
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Count();
    }

    private static bool HasAnyMetric(IReadOnlyList<Dictionary<string, object?>> rows, params string[] metricKeys)
    {
        return rows.Any(row => metricKeys.Any(key => row.TryGetValue(key, out var value) && IsNumeric(value)));
    }

    private static bool IsNumeric(object? value)
    {
        return value switch
        {
            null => false,
            int => true,
            long => true,
            float f => float.IsFinite(f),
            double d => double.IsFinite(d),
            decimal m => true,
            _ => double.TryParse(value.ToString(), out var parsed) && double.IsFinite(parsed)
        };
    }
}

public sealed class WebsiteRadarVerificationResult
{
    public bool Passed { get; set; }
    public int TensileRows { get; set; }
    public int ImpactRows { get; set; }
    public int StiffnessRows { get; set; }
    public int ThermalRows { get; set; }
    public int MaterialIds { get; set; }
    public int SelectedRadarRows { get; set; }
    public int MaterialAverageGroups { get; set; }
    public int ReinforcementAverageGroups { get; set; }
    public bool NormalizationInputsAvailable { get; set; }
    public bool RendererPayloadValid { get; set; }
    public bool ThermalContractFieldsPresent { get; set; }
}
