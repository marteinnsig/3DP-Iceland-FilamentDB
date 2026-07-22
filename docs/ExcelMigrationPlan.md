# Excel Migration Plan

## v27.0 - Excel Settings & Input Workflow Audit

This document is the initial migration audit for moving the Excel workbook logic into the WPF application.

Source workbook:

- Material_Database_v13.5.xlsm
- VBA project present: Yes

## Hard Requirements

The following requirements must be preserved before native calculation migration starts:

1. `04 Settings` must become a native editable app screen.
2. The Material Catalog Lookup must be editable in the app.
3. Base Material must be addable/editable in the app.
4. Input validation and conditional-format sanity checks from `01`, `02`, and `03` must be preserved.
5. Machine measurement limits must show critical/red warnings in the app.
6. Material lifecycle must support Add, Duplicate, Archive, and Delete.
7. Excel remains the reference/source-of-truth until each calculation category is verified.

## Workbook Sheet Audit

- 00 Materials: A1:Z178, rows 178, cells 4604, formulas 1584, conditional rules 0
- 00 Add Filament: A1:E21, rows 21, cells 105, formulas 0, conditional rules 0
- 01 Tensile Measurements: A1:AP177, rows 177, cells 7082, formulas 3168, conditional rules 7
- 02 Impact Measurements: A1:BJ177, rows 177, cells 10974, formulas 6688, conditional rules 5
- 03 Stiffness Measurements: A1:N177, rows 177, cells 2478, formulas 1760, conditional rules 0
- 04 Settings: A1:J41, rows 41, cells 410, formulas 0, conditional rules 0
- 06 Website Export: A1:AG177, rows 177, cells 5841, formulas 5781, conditional rules 0
- Version 10 Notes: A1:A8, rows 8, cells 8, formulas 0, conditional rules 0

## 04 Settings Audit

Settings rows detected: 35

- General / Gravity: 9.81 m/s² (Impact, stiffness)
- General / Maximum samples per orientation: 10 samples (All)
- Impact / Hammer mass: 0.52300000000000002 kg (Impact)
- Impact / Hammer start height: 0.63 m (Impact)
- Impact / Hammer impact height: 7.0000000000000007E-2 m (Impact)
- Impact / Hammer drop height: 0.56000000000000005 m (Impact)
- Impact / Machine loss correction: 4.5 % (Impact)
- Impact / No-sample rebound angle: 105.411 degrees (Impact)
- Impact / Outer sample width: 10 mm (Impact)
- Impact / Outer sample height: 8 mm (Impact)
- Impact / Inner hollow width: 6.6680000000000001 mm (Impact)
- Impact / Inner hollow height: 4.7750000000000004 mm (Impact)
- Impact / Outer area: 80 mm² (Impact)
- Impact / Inner area: 31.839700000000001 mm² (Impact)
- Impact / Net cross-section area: 48.160299999999992 mm² (Impact)
- Impact / Net cross-section area: 4.8160299999999987E-5 m² (Impact)
- Impact / Emax: 2.8731528000000002 J (Impact)
- Impact / Available Joules: 2.7438609239999998 J (Impact)
- Impact / Max possible impact: 56.97350149396911 kJ/m² (Impact)
- Tensile / Outer sample width: 3 mm (Tensile)
- Tensile / Outer sample height: 2.5 mm (Tensile)
- Tensile / Inner hollow width: 1.28 mm (Tensile)
- Tensile / Inner hollow height: 0.78 mm (Tensile)
- Tensile / Outer area: 7.5 mm² (Tensile)
- Tensile / Inner area: 0.99840000000000007 mm² (Tensile)
- Tensile / Net cross-section area: 6.5015999999999998 mm² (Tensile)
- Stiffness / Specimen thickness: 3.2 mm (Stiffness)
- Stiffness / Specimen width: 12.7 mm (Stiffness)
- Stiffness / Mass setting: 547 g (Stiffness)
- Stiffness / Thread pitch: 23.99 TPI (Stiffness)
- Stiffness / mm per revolution: 1.0587744893705711 mm/rev (Stiffness)
- Stiffness / Span length: 118.2 mm (Stiffness)
- Stiffness / Load: 5.47 N (Stiffness)
- Stiffness / Moment: 161.63849999999999 N·mm (Stiffness)
- Stiffness / Second moment of area: 34.679466666666677 mm^4 (Stiffness)

## Material Catalog Lookup / Base Materials

Base material rows detected: 26

- PLA: Standard, sort 10
- PLA+: Standard, sort 11
- PLA Pro: Standard, sort 12
- PLA-HS: Standard, sort 13
- HTPLA: Standard, sort 14
- PLA-LW: Specialty, sort 15
- PLA Soft: Flexible, sort 16
- PETG: Standard, sort 20
- PCTG: Standard, sort 25
- PET: Engineering, sort 27
- PVB: Specialty, sort 28
- ABS: Engineering, sort 30
- ASA: Engineering, sort 35
- HIPS: Engineering, sort 38
- PC: Engineering, sort 40
- PC/PBT: Engineering, sort 42
- PMMA: Engineering, sort 45
- PP: Engineering, sort 50
- PA6: Engineering, sort 60
- PA11: Engineering, sort 61
- PA12: Engineering, sort 62
- TPU: Flexible, sort 70
- TPE: Flexible, sort 71
- PEBA: Flexible, sort 72
- PEEK: High Performance, sort 90
- PEI: High Performance, sort 91

## Input Sheet Headers

### 01 Tensile Measurements
- Material ID
- Manufacturer
- Product Line
- Marketing Name
- Base Material
- Material Category
- Variant / Finish
- Reinforcement
- Color
- Upright 1
- Upright 2
- Upright 3
- Upright 4
- Upright 5
- Upright 6
- Upright 7
- Upright 8
- Upright 9
- Upright 10
- Flat 1
- Flat 2
- Flat 3
- Flat 4
- Flat 5
- Flat 6
- Flat 7
- Flat 8
- Flat 9
- Flat 10
- MPa - Upright
- MPa - Flat
- Std Dev - Upright
- Std Dev - Flat
- CV % - Upright
- CV % - Flat
- Samples - Upright
- Samples - Flat
- Confidence - Upright
- Confidence - Flat
- Test Notes

### 02 Impact Measurements
- Material ID
- Manufacturer
- Product Line
- Marketing Name
- Base Material
- Material Category
- Variant / Finish
- Reinforcement
- Color
- Upright needle % 1
- Upright needle % 2
- Upright needle % 3
- Upright needle % 4
- Upright needle % 5
- Upright needle % 6
- Upright needle % 7
- Upright needle % 8
- Upright needle % 9
- Upright needle % 10
- Flat needle % 1
- Flat needle % 2
- Flat needle % 3
- Flat needle % 4
- Flat needle % 5
- Flat needle % 6
- Flat needle % 7
- Flat needle % 8
- Flat needle % 9
- Flat needle % 10
- Upright kJ/m² 1
- Upright kJ/m² 2
- Upright kJ/m² 3
- Upright kJ/m² 4
- Upright kJ/m² 5
- Upright kJ/m² 6
- Upright kJ/m² 7
- Upright kJ/m² 8
- Upright kJ/m² 9
- Upright kJ/m² 10
- Flat kJ/m² 1
- Flat kJ/m² 2
- Flat kJ/m² 3
- Flat kJ/m² 4
- Flat kJ/m² 5
- Flat kJ/m² 6
- Flat kJ/m² 7
- Flat kJ/m² 8
- Flat kJ/m² 9
- Flat kJ/m² 10
- kJ/m² - Upright
- kJ/m² - Flat
- Std Dev - Upright
- Std Dev - Flat
- CV % - Upright
- CV % - Flat
- Samples - Upright
- Samples - Flat
- Confidence - Upright
- Confidence - Flat
- Test Notes

### 03 Stiffness Measurements
- Material ID
- Manufacturer
- Product Line
- Marketing Name
- Base Material
- Material Category
- Variant / Finish
- Reinforcement
- Color
- Revolutions
- Degrees
- Deflection mm
- Modulus MPa
- Test Notes

## Conditional Formatting / Sanity Check Audit

These rules are not decoration. They represent measurement consistency and machine-range validation behavior that must be recreated as native app validation.

### 01 Tensile Measurements
- Rule 1: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `greaterThan`, formula `505`
- Rule 2: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `301; 505`
- Rule 3: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `201; 300`
- Rule 4: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `101; 200`
- Rule 5: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `31; 100`
- Rule 6: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `1; 10`
- Rule 7: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `11; 30`
### 02 Impact Measurements
- Rule 1: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `greaterThan`, formula `100`
- Rule 2: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `21; 30`
- Rule 3: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `31; 100`
- Rule 4: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `11; 20`
- Rule 5: range `J1:S1048576 U1:AD1048576`, type `cellIs`, operator `between`, formula `1; 10`
### 03 Stiffness Measurements
- No conditional formatting rules detected in workbook XML.

## v27 Migration Roadmap

### v27.0 - Excel Settings & Input Workflow Audit

- Add visible app audit screen.
- Document workbook settings, input sheets, formula counts and conditional formatting.
- Establish hard migration requirements.

### v27.1 - Native Settings Editor

- Create editable settings UI based on `04 Settings`.
- Persist settings locally.
- Include General, Impact, Tensile, and Stiffness settings.

### v27.2 - Material Catalog Manager

- Add Material Catalog Lookup management.
- Add Base Material creation/edit/archive.
- Validate category/sort order.

### v27.3 - Input Sheet Validation Engine

- Recreate conditional formatting/sanity check logic for `01`, `02`, and `03`.
- Show warning/critical status for measurement consistency and machine limits.

### v27.4 - Material Lifecycle

- Native Add Material.
- Duplicate Material.
- Archive Material.
- Delete Material with confirmation/safety rules.

### v27.5 - First Native Calculation Engine

- Migrate the first calculation category only after settings and validation are native.
- Compare WPF output against Excel output before marking complete.

## Completion Rule

No Excel calculation engine should be considered migrated until its native output matches the Excel workbook on known test rows and the relevant settings and validation checks are already native.
