# Architecture

Current version: v41.6.0 INTERNAL-REPEATABILITY-CALIBRATION - Internal Repeatability Calibration

## Core Rules

- SQLite is the Source of Truth.
- MaterialID is the canonical engineering key.
- Engineering calculations exist in exactly one location: the Engineering Platform.
- Downstream systems consume verified platform outputs only.
- Material Summary is the engineering data contract for downstream systems.
- Verification Center is the release gate.
- Architecture grows by extension, never by replacement.

## Platform Flow

SQLite
  -> Native Database Platform
  -> Engineering Platform
  -> Material Summary Engine
  -> Verification Center
  -> Website Platform
  -> Reporting Platform (future)
  -> AI Platform (future)

## Website Platform Services

- WebsiteDataPipeline: consumes verified Material Summary outputs.
- WebsiteChartGeneratorService: owns chart payload rows.
- WebsiteRadarGeneratorService: owns radar payload validation and averages.
- WebsiteHtmlRendererService: owns template DATA injection and render validation.
- WebsiteVerificationService: owns website validation and publish readiness.
