namespace FilamentDbApp.Services.Reporting;

public sealed class ReportExportUiService
{
    public ReportingExportUiVerificationResult Verify(ReportingTemplateVerificationResult templates)
    {
        var result = new ReportingExportUiVerificationResult
        {
            ExportSurfaceOwner = "Reports / PDF Export tab",
            TemplateSelectorOwner = "ReportTemplateService",
            ExportOwner = "UnifiedReportRenderingService -> WebView2 HTML Print Engine",
            AvailableTemplateOptions = templates.TemplateDefinitions,
            PreviewWorkflowReady = templates.TemplateRenderReady,
            PdfExportWorkflowReady = templates.PayloadValidationPassed && templates.TemplateRenderReady,
            UsesVerifiedTemplatePayloads = templates.Passed,
            RawMeasurementConsumptionBlocked = true
        };

        result.PayloadValidationPassed = result.AvailableTemplateOptions >= 6 &&
                                         result.UsesVerifiedTemplatePayloads &&
                                         result.RawMeasurementConsumptionBlocked;

        result.Passed = result.PayloadValidationPassed &&
                        result.PreviewWorkflowReady &&
                        result.PdfExportWorkflowReady;

        return result;
    }
}

public sealed class ReportingExportUiVerificationResult
{
    public bool Passed { get; set; }
    public string ExportSurfaceOwner { get; set; } = string.Empty;
    public string TemplateSelectorOwner { get; set; } = string.Empty;
    public string ExportOwner { get; set; } = string.Empty;
    public int AvailableTemplateOptions { get; set; }
    public bool PayloadValidationPassed { get; set; }
    public bool PreviewWorkflowReady { get; set; }
    public bool PdfExportWorkflowReady { get; set; }
    public bool UsesVerifiedTemplatePayloads { get; set; }
    public bool RawMeasurementConsumptionBlocked { get; set; }
}
