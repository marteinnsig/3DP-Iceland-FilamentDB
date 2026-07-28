# Decisions

## DEMO-002 Fictional Identity Surface

The public demo fictionalizes Manufacturer, Product Line, Marketing Name,
Color and Variant/Finish. Generic Base Material and `CF`/`GF` reinforcement
remain because they are required for useful engineering comparisons.

Every identity is derived from one immutable public-safe atomic spec. Source
labels, display names, keys, summaries, reports, collections and cached hashes
are never copied. Downstream consumers receive only the completed fictional
graph and must recompute their own labels, groups and evidence.

## DEMO-001 Real Measurements Under Fictional Public Identities

The v56 public demo uses only explicitly owner-approved real measurement sets
under disclosed fictional material, manufacturer and product-family identities.
This is pseudonymization, not anonymization; every retained material requires
owner allowlist approval because distinctive measurement patterns can support
re-identification.

The demo is built fresh from a positive allowlist. It is never produced by
copying the owner database and deleting known private tables afterward. The
private identity mapping never ships, the canonical tester seed stays separate,
and Production/FTPS require later explicit authority.

## REPORT-005 Native Report Templates

Report templates are a separate layer from report model generation and PDF rendering.

Approved flow:

`ReportingDataPipelineService -> ReportGeneratorService -> ReportTemplateService -> ReportPdfRendererService`

Templates consume verified report models only. They do not read raw measurement rows and do not perform engineering calculations.
