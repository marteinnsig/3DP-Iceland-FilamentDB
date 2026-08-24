# Native Material Management

## v27.3

This milestone adds the first native material management surface for replacing the Excel `00 Materials` workflow.

## Source

- Excel workbook: `Material_Database_v13.5.xlsm`
- Sheet: `00 Materials`

## Visible App Features

A new **Material Manager** tab is added to the WPF app.

## Imported Defaults

- Materials imported from Excel defaults: 176

## Actions

- Save Materials
- Load Materials
- Restore Excel Defaults
- Add Material
- Duplicate Selected
- Archive Selected
- Delete Selected
- Refresh Summary

## Local Storage

Native materials are saved locally as:

`native-materials-manager.json`

## Important Notes

This is a local editable copy of `00 Materials`.
It does not yet replace the main SQLite materials table or write back to Excel.

The goal is to establish the native material lifecycle before native measurement entry and calculation migration.

## Next Step

v27.4 should connect the Material Manager more deeply to lookups and validation:

- Base material lookup from Settings Manager
- Category defaults
- Material key generation
- Duplicate detection
- Safer archive/delete rules

## v27.3.2 YouTube Review URL support

The native Material Manager now includes the Excel `YouTube Review URL` column as a first-class editable field.

Supported import/sync header aliases:

- `YouTube Review URL`
- `YouTube URL`
- `Youtube URL`
- `YouTube Link`
- `Video URL`

The existing `Video` column remains the Yes/No status flag, while `YouTube Review URL` stores the actual linked review video URL.


## v27.3.5 JSON + SQLite transition storage

This milestone keeps the safe JSON working-copy flow from v27.3.x, but also writes the same native Material Manager rows into SQLite.

### Storage behavior

- `native-materials-manager.json` remains the editable working-copy / backup file.
- SQLite now has a dedicated `NativeMaterialManagerRows` table for native material manager data.
- Save Materials writes to both JSON and SQLite.
- Load Materials prefers the JSON working copy, then falls back to SQLite, then imported Excel/default rows.
- Sync Imported Materials now marks the Material Manager as dirty so the user must explicitly save the refreshed native copy.

### Why this transition exists

Excel is still the source of truth until native measurements and the native calculation engine are complete. This build prepares the application for native modules that need to read material identities from SQLite without removing the JSON safety net yet.

### Website export rule reminder

Archived rows remain visible in Material Manager. Future native website export should use only active rows where `IsArchived == false`.

## v27.3.6 Native Computed Fields Engine

The Material Manager now owns the same kind of automatic field logic that previously lived in Excel formulas.

Computed/read-only fields:

- Video
- Tested Status (derived from Tensile, Impact, Stiffness and Heat coverage)
- In Heat (read-only; Yes only while a valid canonical Heat Deflection result exists)
- Sort Order
- Source Priority
- Website Display Name
- Material Key

Editable source fields remain user-editable. When a source field changes, the computed fields are recalculated by the application so stale display names and stale video/tested statuses are avoided.
