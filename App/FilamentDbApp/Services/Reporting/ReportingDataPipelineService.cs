using FilamentDbApp.Services.Calculations;

namespace FilamentDbApp.Services.Reporting;

public sealed class ReportingDataPipelineService
{
    public ReportingDataPipelinePayload BuildPayload(IEnumerable<ReportingMaterialInput> materials)
    {
        var rows = materials
            .Where(material => !string.IsNullOrWhiteSpace(material.MaterialId))
            .Select(material => new ReportingMaterialSummaryRow(
                material.MaterialId.Trim(),
                Text(material.CommonFields, "label"),
                Text(material.CommonFields, "manufacturer"),
                Text(material.CommonFields, "productLine"),
                Text(material.CommonFields, "baseMaterial"),
                Text(material.CommonFields, "reinforcement"),
                Text(material.CommonFields, "color"),
                Number(material.Summary.Tensile?.Flat.Average),
                Number(material.Summary.Tensile?.Upright.Average),
                Number(material.Summary.Impact?.Flat.Average),
                Number(material.Summary.Impact?.Upright.Average),
                Number(material.Summary.Stiffness?.ModulusMpa),
                IsComplete(material.Summary)))
            .ToList();

        return new ReportingDataPipelinePayload(rows);
    }

    public ReportingDataPipelineVerificationResult Verify(IEnumerable<ReportingMaterialInput> materials)
    {
        var materialList = materials.ToList();
        var payload = BuildPayload(materialList);

        var materialIds = payload.Rows
            .Select(row => row.MaterialId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        var result = new ReportingDataPipelineVerificationResult
        {
            InputRows = materialList.Count,
            ReportRows = payload.Rows.Count,
            CompleteRows = payload.Rows.Count(row => row.IsComplete),
            PartialRows = payload.Rows.Count(row => !row.IsComplete),
            MaterialIdCoverage = payload.Rows.Count > 0 && materialIds.Count == payload.Rows.Count,
            NoDuplicateMaterialIds = materialIds.Count == materialIds.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            UsesVerifiedMaterialSummary = materialList.Count > 0 && materialList.All(row => row.Summary is not null)
        };

        result.Passed = result.InputRows > 0 &&
                        result.ReportRows == result.InputRows &&
                        result.MaterialIdCoverage &&
                        result.NoDuplicateMaterialIds &&
                        result.UsesVerifiedMaterialSummary &&
                        result.CompleteRows > 0;

        return result;
    }

    private static string? Text(IReadOnlyDictionary<string, object?> fields, string key)
    {
        return fields.TryGetValue(key, out var value) ? value?.ToString() : null;
    }

    private static double? Number(double? value)
    {
        return value.HasValue && double.IsFinite(value.Value) ? value.Value : null;
    }

    private static bool IsComplete(MaterialResults summary)
    {
        return summary.IsCompleteEngineeringSummary;
    }
}

public sealed record ReportingMaterialInput(
    string MaterialId,
    IReadOnlyDictionary<string, object?> CommonFields,
    MaterialResults Summary);

public sealed record ReportingDataPipelinePayload(
    IReadOnlyList<ReportingMaterialSummaryRow> Rows);

public sealed record ReportingMaterialSummaryRow(
    string MaterialId,
    string? Label,
    string? Manufacturer,
    string? ProductLine,
    string? BaseMaterial,
    string? Reinforcement,
    string? Color,
    double? TensileFlatMpa,
    double? TensileUprightMpa,
    double? ImpactFlatKjM2,
    double? ImpactUprightKjM2,
    double? StiffnessMpa,
    bool IsComplete);

public sealed class ReportingDataPipelineVerificationResult
{
    public bool Passed { get; set; }
    public int InputRows { get; set; }
    public int ReportRows { get; set; }
    public int CompleteRows { get; set; }
    public int PartialRows { get; set; }
    public bool MaterialIdCoverage { get; set; }
    public bool NoDuplicateMaterialIds { get; set; }
    public bool UsesVerifiedMaterialSummary { get; set; }
}
