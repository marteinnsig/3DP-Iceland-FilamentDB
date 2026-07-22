# Decisions

## REPORT-005 Native Report Templates

Report templates are a separate layer from report model generation and PDF rendering.

Approved flow:

`ReportingDataPipelineService -> ReportGeneratorService -> ReportTemplateService -> ReportPdfRendererService`

Templates consume verified report models only. They do not read raw measurement rows and do not perform engineering calculations.
