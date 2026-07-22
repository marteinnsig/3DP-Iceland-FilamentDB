# Dependency Map

## Allowed dependencies

```text
Database Platform → no downstream dependency
Engineering Platform → Database Platform
Verification Center → Database Platform + Engineering Platform + Website Platform
Website Platform → Engineering Platform verified outputs
Reporting Platform → Engineering Platform verified outputs + optional Website visualization outputs
AI Platform → verified platform outputs only
```

## Blocked dependencies

- Website Platform must not read raw measurement rows for calculations.
- Reporting Platform must not recalculate tensile, impact, stiffness, consistency, or ratings.
- AI Platform must not invent engineering values or bypass Material Summary.
- Future platforms must not create parallel calculation engines.

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
