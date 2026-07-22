# Database Engine Design

Milestone 4 introduces the first structured SQLite database engine. The app is still read-only, but the import is no longer only a temporary table for the UI.

## Current import scope

The importer now reads these workbook sheets when they exist:

- `00 Materials`
- `01 Tensile Measurements`
- `02 Impact Measurements`
- `03 Stiffness Measurements`
- `06 Website Export`

`00 Materials` remains the source of truth for the material browser UI.

## Main tables

### Materials

Normalized material master table keyed by `MaterialId`.

Important fields include manufacturer, product line, base material, category, variant/finish, reinforcement, color, spool data, URLs, tested status, website display name, and material key.

### MaterialAttributes

Stores every imported field from `00 Materials` as key/value data. This protects us when new Excel columns are added later.

### Manufacturers

Unique manufacturer list derived from `00 Materials`.

### LookupValues

Reusable lookup values for manufacturer, base material, category, variant/finish, reinforcement, and color.

### ExcelSheets / ExcelSheetColumns / ExcelSheetRows / ExcelSheetCells

A generic workbook snapshot layer. This preserves imported worksheet structure without forcing every sheet into a final domain model immediately.

This gives us a safe bridge from Excel to the future app-native database.

### TestSummaryValues

First version of the test summary layer. It imports calculated summary metrics from `06 Website Export`, including tensile, impact, stiffness, and rating values.

This is the first step toward driving charts and website export from the app database instead of directly from Excel.

## Why this design

The workbook contains both raw measurement sheets and calculated/export sheets. Importing everything directly into final domain tables too early would make the app fragile.

This milestone therefore creates two layers:

1. **Generic workbook snapshot** — preserves imported workbook data safely.
2. **Structured engine tables** — starts building the permanent data model for materials, lookups, manufacturers, and summary metrics.

Future milestones can gradually move more logic from the generic snapshot into dedicated domain tables.

## Next database milestones

- Add typed raw measurement tables for tensile samples.
- Add typed raw measurement tables for impact samples.
- Add typed stiffness/modulus table.
- Add calculation engine for averages, standard deviation, CV %, sample counts, and confidence.
- Validate app calculations against Excel calculated values.
- Drive the Mechanical tab from `TestSummaryValues`.
