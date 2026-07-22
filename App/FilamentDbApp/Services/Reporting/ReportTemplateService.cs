namespace FilamentDbApp.Services.Reporting;

public sealed class ReportTemplateService
{
    private static readonly IReadOnlyList<ReportingTemplateDefinition> DefaultTemplates = new List<ReportingTemplateDefinition>
    {
        new("material-summary", "Material Summary Report", ReportingTemplateType.MaterialSummary, new[] { ReportingSectionType.MaterialOverview, ReportingSectionType.EngineeringSummary, ReportingSectionType.MechanicalMetric }),
        new("engineering", "Engineering Report", ReportingTemplateType.Engineering, new[] { ReportingSectionType.MaterialOverview, ReportingSectionType.EngineeringSummary, ReportingSectionType.MechanicalMetric }),
        new("comparison", "Comparison Report", ReportingTemplateType.Comparison, new[] { ReportingSectionType.MaterialOverview, ReportingSectionType.EngineeringSummary, ReportingSectionType.MechanicalMetric }),
        new("manufacturer", "Manufacturer Report", ReportingTemplateType.Manufacturer, new[] { ReportingSectionType.MaterialOverview, ReportingSectionType.EngineeringSummary }),
        new("test-session", "Test Session Report", ReportingTemplateType.TestSession, new[] { ReportingSectionType.MaterialOverview, ReportingSectionType.EngineeringSummary, ReportingSectionType.MechanicalMetric }),
        new("printing-recommendation", "Printing Recommendation Report", ReportingTemplateType.PrintingRecommendation, new[] { ReportingSectionType.MaterialOverview, ReportingSectionType.EngineeringSummary, ReportingSectionType.MechanicalMetric })
    };

    public ReportingTemplatePayload BuildTemplates(ReportingReportModel reportModel)
    {
        var materialReports = reportModel.MaterialReports
            .Where(report => !string.IsNullOrWhiteSpace(report.MaterialId))
            .ToList();

        var templateOutputs = DefaultTemplates
            .Select(template => BuildTemplateOutput(template, materialReports))
            .ToList();

        return new ReportingTemplatePayload(
            Templates: DefaultTemplates,
            Outputs: templateOutputs,
            SourceReportCount: reportModel.MaterialReports.Count,
            MaterialReportsAvailable: materialReports.Count,
            GeneratedAtUtc: DateTime.UtcNow);
    }

    public ReportingTemplateVerificationResult Verify(ReportingReportModel reportModel)
    {
        var payload = BuildTemplates(reportModel);
        var expectedTemplates = DefaultTemplates.Count;

        var result = new ReportingTemplateVerificationResult
        {
            InputReports = reportModel.MaterialReports.Count,
            TemplateDefinitions = payload.Templates.Count,
            TemplateOutputs = payload.Outputs.Count,
            MaterialReportsAvailable = payload.MaterialReportsAvailable,
            PayloadValidationPassed = payload.Outputs.Count == expectedTemplates &&
                                      payload.Outputs.All(output => output.MaterialIds.Count == payload.MaterialReportsAvailable && output.RequiredSections.Count > 0),
            TemplateRenderReady = payload.MaterialReportsAvailable == reportModel.MaterialReports.Count &&
                                  payload.Outputs.All(output => output.ReadyForRender),
            HasMaterialSummaryTemplate = payload.Templates.Any(template => template.TemplateType == ReportingTemplateType.MaterialSummary),
            HasEngineeringTemplate = payload.Templates.Any(template => template.TemplateType == ReportingTemplateType.Engineering),
            HasComparisonTemplate = payload.Templates.Any(template => template.TemplateType == ReportingTemplateType.Comparison),
            HasManufacturerTemplate = payload.Templates.Any(template => template.TemplateType == ReportingTemplateType.Manufacturer),
            HasTestSessionTemplate = payload.Templates.Any(template => template.TemplateType == ReportingTemplateType.TestSession),
            HasPrintingRecommendationTemplate = payload.Templates.Any(template => template.TemplateType == ReportingTemplateType.PrintingRecommendation)
        };

        result.Passed = result.InputReports > 0 &&
                        result.TemplateDefinitions == expectedTemplates &&
                        result.TemplateOutputs == expectedTemplates &&
                        result.MaterialReportsAvailable == result.InputReports &&
                        result.PayloadValidationPassed &&
                        result.TemplateRenderReady &&
                        result.HasMaterialSummaryTemplate &&
                        result.HasEngineeringTemplate &&
                        result.HasComparisonTemplate &&
                        result.HasManufacturerTemplate &&
                        result.HasTestSessionTemplate &&
                        result.HasPrintingRecommendationTemplate;

        return result;
    }

    private static ReportingTemplateOutput BuildTemplateOutput(ReportingTemplateDefinition template, IReadOnlyList<ReportingMaterialReportModel> reports)
    {
        var materialIds = reports.Select(report => report.MaterialId).ToList();
        // RELEASE-003B: newly added materials can legitimately be partial or empty while
        // measurements are still being collected. Template readiness must validate that every
        // report has the canonical render shell (identity + engineering summary), not require
        // mechanical metric sections for materials that do not have measurements yet.
        var readyForRender = reports.Count > 0 && reports.All(report =>
            report.Sections.Any(section => section.SectionType == ReportingSectionType.MaterialOverview) &&
            report.Sections.Any(section => section.SectionType == ReportingSectionType.EngineeringSummary));

        return new ReportingTemplateOutput(
            TemplateKey: template.TemplateKey,
            TemplateName: template.TemplateName,
            TemplateType: template.TemplateType,
            MaterialIds: materialIds,
            RequiredSections: template.RequiredSections,
            Source: "Verified Report Models",
            RenderOwner: "ReportPdfRendererService",
            ReadyForRender: readyForRender);
    }
}

public sealed record ReportingTemplatePayload(
    IReadOnlyList<ReportingTemplateDefinition> Templates,
    IReadOnlyList<ReportingTemplateOutput> Outputs,
    int SourceReportCount,
    int MaterialReportsAvailable,
    DateTime GeneratedAtUtc);

public sealed record ReportingTemplateDefinition(
    string TemplateKey,
    string TemplateName,
    ReportingTemplateType TemplateType,
    IReadOnlyList<ReportingSectionType> RequiredSections);

public sealed record ReportingTemplateOutput(
    string TemplateKey,
    string TemplateName,
    ReportingTemplateType TemplateType,
    IReadOnlyList<string> MaterialIds,
    IReadOnlyList<ReportingSectionType> RequiredSections,
    string Source,
    string RenderOwner,
    bool ReadyForRender);

public enum ReportingTemplateType
{
    MaterialSummary,
    Engineering,
    Comparison,
    Manufacturer,
    TestSession,
    PrintingRecommendation
}

public sealed class ReportingTemplateVerificationResult
{
    public bool Passed { get; set; }
    public int InputReports { get; set; }
    public int TemplateDefinitions { get; set; }
    public int TemplateOutputs { get; set; }
    public int MaterialReportsAvailable { get; set; }
    public bool PayloadValidationPassed { get; set; }
    public bool TemplateRenderReady { get; set; }
    public bool HasMaterialSummaryTemplate { get; set; }
    public bool HasEngineeringTemplate { get; set; }
    public bool HasComparisonTemplate { get; set; }
    public bool HasManufacturerTemplate { get; set; }
    public bool HasTestSessionTemplate { get; set; }
    public bool HasPrintingRecommendationTemplate { get; set; }
}
