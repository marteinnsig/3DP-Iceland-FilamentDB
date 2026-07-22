# Material Database

The Material Database is the core of the project.

## Responsibilities

- Import filament data from the Excel workbook.
- Normalize material records for display and analysis.
- Show material details, identities, links and mechanical summaries.
- Provide data to rankings, reports, website exports and YouTube planning.

## Data flow

```text
Excel workbook
  → Excel import services
  → Material records / workbook import models
  → Local cache/database
  → UI, reports, rankings and planning engines
```

## Related files

- `FilamentDbApp/Models/MaterialRecord.cs`
- `FilamentDbApp/Models/WorkbookImportData.cs`
- `FilamentDbApp/Services/ExcelWorkbookImporter.cs`
- `FilamentDbApp/Services/ExcelMaterialImporter.cs`
- `FilamentDbApp/Services/MaterialDetailService.cs`
- `FilamentDbApp/Services/MaterialFilterService.cs`
