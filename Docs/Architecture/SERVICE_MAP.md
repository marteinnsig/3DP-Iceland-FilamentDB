# Service Map

## Engineering Platform

- StatisticsService - shared statistics and sample consistency calculations.
- RatingService - confidence/rating logic.
- ResultsService - tensile, impact, stiffness, and material result calculations.
- Material Summary Engine - one verified summary per MaterialID.

## Website Platform

- WebsiteDataPipeline - converts verified summaries into website-ready rows.
- WebsiteChartGeneratorService - owns chart payload generation.
- WebsiteRadarGeneratorService - owns radar payload generation and average groups.
- WebsiteHtmlRendererService - owns template DATA injection and render validation.
- WebsiteVerificationService - owns website validation and publish readiness.

## Future Reporting Platform

- ReportDataPipeline
- ReportTemplateRenderer
- PdfExportService
- ReportVerificationService

## Future AI Platform

- RecommendationService
- ExplanationService
- AiContextBuilder
- AiVerificationService

## v30 Reporting Platform update

REPORT-001 introduces `ReportingDataPipelineService` as the first Reporting Platform service. It consumes verified `MaterialResults` / Material Summary outputs only. Reporting Platform is downstream of Engineering Platform and must not consume raw measurement rows or implement independent engineering calculations.

Reporting Platform current service map:

- `ReportingDataPipelineService` - converts verified Material Summary outputs into reporting payload rows for future report generators, PDF renderers, certificates, and verification suites.

Current Reporting Platform status: STARTED / REPORT-001.


## v30.1 Reporting Platform update

REPORT-002 introduces `ReportGeneratorService`. The service converts verified Reporting Data Pipeline rows into native report models and report sections. It does not render PDF files yet and does not consume raw measurement rows. Future PDF and certificate generators must consume these report models.

Reporting Platform current service map:

```text
ReportingDataPipelineService
        |
        v
ReportGeneratorService
        |
        v
Future PDF Renderer / Certificate Generator
```

Current Reporting Platform status: ACTIVE / REPORT-002.


## v30.2 Reporting Platform update

REPORT-003 introduces `ReportPdfRendererService` downstream of `ReportGeneratorService`. The flow is now `ReportingDataPipelineService -> ReportGeneratorService -> ReportPdfRendererService`. PDF rendering consumes report models only, keeps rendering separate from report model generation, and does not read raw tensile, impact, or stiffness measurement rows.

Current Reporting Platform status: ACTIVE / REPORT-003.

## v30.3 Reporting Platform update

REPORT-004 introduces `ReportCertificateGeneratorService` downstream of `ReportPdfRendererService`. The flow is now `ReportingDataPipelineService -> ReportGeneratorService -> ReportPdfRendererService -> ReportCertificateGeneratorService`. Certificate generation consumes verified report models and native PDF payload metadata only, keeps certificate payload generation separate from PDF rendering, and does not read raw tensile, impact, or stiffness measurement rows.

Current Reporting Platform status: ACTIVE / REPORT-004.


## REPORT-005 Native Report Templates

REPORT-005 introduces `ReportTemplateService` between verified report model generation and downstream rendering. Template payloads are built from `ReportingReportModel` only and preserve the rule that Reporting Platform systems consume verified Material Summary outputs through the reporting pipeline. Current Reporting Platform status: ACTIVE / REPORT-005.
