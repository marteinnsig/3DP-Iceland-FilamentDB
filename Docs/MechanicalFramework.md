# Mechanical Test Framework

Version v0.11 adds the first application-level mechanical test framework.

## What it does now

For the selected material, the Mechanical tab shows:

- Imported mechanical workbook sheets that contain the material ID.
- Number of non-empty imported cell values for that material in each test sheet.
- Summary metrics imported from `06 Website Export`.

## Current sources

- `01 Tensile Measurements`
- `02 Impact Measurements`
- `03 Stiffness Measurements`
- `06 Website Export`

## Why this step exists

Before importing every raw sample into strongly typed tables, the app now proves that it can:

1. Link imported workbook sheets to a selected material.
2. Read mechanical summary metrics from the database engine.
3. Display the data in a dedicated Mechanical tab.

## Next step

The next step is a structured Tensile importer:

- Upright sample columns.
- Flat sample columns.
- App-side calculated averages, standard deviations, CV %, and sample counts.
- Comparison against values from `06 Website Export`.

## v0.12 Tensile Import

The app now imports the `01 Tensile Measurements` worksheet into dedicated SQLite tables:

- `TensileResults` stores per-material summary values such as MPa, standard deviation, CV, sample counts, confidence, and notes.
- `TensileSamples` stores the raw Upright/Flat sample values for sample numbers 1-10.

The Mechanical tab shows the Tensile summary for the selected material. Raw sample display is stored in the database and can be surfaced in a later UI pass.
