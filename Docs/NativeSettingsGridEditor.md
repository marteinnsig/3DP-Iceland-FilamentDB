# Native Settings Grid Editor

## v27.2

This milestone replaces the v27.1 text-based Settings Manager with native editable grids.

## Source

- Excel workbook: `Material_Database_v13.5.xlsm`
- Sheet: `04 Settings`

## Visible App Features

Settings Manager now contains two editable grids:

1. Measurement / Calculation Settings
2. Base Material Catalog / Lookup

## Imported Defaults

- Settings rows: 35
- Base material rows: 26

## Editable Fields

### Settings

- Section
- Parameter
- Value
- Unit
- Used By
- Notes

### Base Materials

- Base Material
- Category
- Sort Order

## Actions

- Save Settings
- Load Settings
- Restore Excel Defaults
- Add Setting Row
- Delete Selected Setting
- Add Base Material
- Delete Selected Base Material

## Local Storage

Saved settings are stored locally as:

`native-settings-manager.json`

## Purpose

This is the first native replacement for the Excel `04 Settings` sheet.

Future calculation engines must read from these native settings instead of directly depending on Excel.

## Current canonical command ownership

The v27.2 action and storage list above is historical. In v44.7.11:

- `Save Settings` persists General Settings, Deployment Settings and Base
  Material Catalog through their separate SQLite owners.
- `Reload Saved Settings` reloads General and Deployment values from SQLite
  after default-No confirmation; Base Material Catalog is unchanged.
- `Restore Built-in Defaults` replaces and saves General Settings only.
- `Reset Columns` resets the two machine-local Settings Manager layouts and
  does not change canonical data.
- Legacy JSON remains migration compatibility input only.
