# Platform Evolution

This document records how the 3DPIceland project evolved from a workbook-driven workflow into a platform-based engineering system.

## Early phase - Excel workbook

The project began as an Excel-based filament testing database. Separate sheets tracked materials, tensile measurements, impact measurements, stiffness measurements, settings, and website export data.

The workbook was valuable because it proved the testing model, formulas, and public dataset concept.

## Automation phase

Macros were introduced to reduce manual work:

- Add material
- Duplicate material
- Archive material
- Delete material
- Export website HTML
- Maintain shared material identity across sheets

This phase revealed the need for stronger identity, repeatability, and regression safety.

## Desktop database phase

The WPF application introduced a native desktop environment, SQLite persistence, material management, import/export support, dashboards, and native editing workflows.

This moved the project beyond Excel automation into application architecture.

## v27 - Native Database Platform

Version 27 established the database foundation:

- SQLite as Source of Truth
- MaterialID as primary engineering key
- Native material management
- Native measurement input workflows
- Import/export preservation
- Regression-safe incremental architecture

## v28 - Engineering Platform

Version 28 centralized engineering calculations:

- StatisticsService
- RatingService
- ResultsService
- Tensile calculation migration
- Impact calculation migration
- Stiffness calculation migration
- Material Summary Engine
- Verification Center as quality gate

The critical architectural change was that downstream systems must consume verified Material Summary outputs, not raw measurements.

## v29 - Website Platform

Version 29 turned the website into the first downstream platform:

- Native Website Data Pipeline
- Native Chart Generator
- Native Radar Generator
- Native HTML Renderer
- Website Verification Suite
- Publish readiness validation
- Platform architecture documentation

The website no longer owns engineering calculations. It visualizes verified engineering outputs.

## v30 - Reporting Platform

Version 30 will introduce report generation as a new downstream platform. It will build on the same verified Material Summary source used by Website Platform.

## Long-term direction

The project is now the 3DPIceland Engineering Platform:

```text
Excel proof-of-concept
  ↓
Workbook automation
  ↓
Desktop database
  ↓
Native Database Platform
  ↓
Engineering Platform
  ↓
Website Platform
  ↓
Reporting Platform
  ↓
AI / API / Automation / Mobile / Cloud
```

## Guiding principle

Each platform extends the verified foundation. New platforms add responsibility; they do not replace or duplicate existing platform ownership.
