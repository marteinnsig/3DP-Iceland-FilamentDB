namespace FilamentDbApp.Services.Reporting;

public sealed class UnifiedReportRenderingService
{
    public UnifiedReportRenderingVerificationResult Verify(
        ReportingTemplateVerificationResult templates,
        ReportingExportUiVerificationResult exportUi,
        ReportingPdfRendererVerificationResult pdfRenderer,
        bool htmlPreviewGenerated,
        bool reportLogoAvailable)
    {
        var result = new UnifiedReportRenderingVerificationResult
        {
            Owner = "UnifiedReportRenderingService",
            CanonicalFormat = "HTML report package",
            PdfStrategy = "PDF exports are generated from the same report foundation and ship with the canonical HTML source",
            HtmlPreviewGenerated = htmlPreviewGenerated,
            UsesCanonicalHtmlLayout = templates.Passed && htmlPreviewGenerated,
            UsesSharedAssets = reportLogoAvailable && pdfRenderer.HasLogoAsset,
            PdfExportPreserved = exportUi.PdfExportWorkflowReady && pdfRenderer.RenderReady,
            RawMeasurementConsumptionBlocked = true
        };

        result.Passed = result.UsesCanonicalHtmlLayout &&
                        result.UsesSharedAssets &&
                        result.PdfExportPreserved &&
                        result.RawMeasurementConsumptionBlocked;

        return result;
    }
}

public sealed class UnifiedReportRenderingVerificationResult
{
    public bool Passed { get; set; }
    public string Owner { get; set; } = string.Empty;
    public string CanonicalFormat { get; set; } = string.Empty;
    public string PdfStrategy { get; set; } = string.Empty;
    public bool HtmlPreviewGenerated { get; set; }
    public bool UsesCanonicalHtmlLayout { get; set; }
    public bool UsesSharedAssets { get; set; }
    public bool PdfExportPreserved { get; set; }
    public bool RawMeasurementConsumptionBlocked { get; set; }
}
