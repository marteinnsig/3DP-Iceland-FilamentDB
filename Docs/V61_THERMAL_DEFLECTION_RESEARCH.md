# v61 Thermal Deflection Measurement Foundation Research

Date: 2026-08-14

## Owner method contract

The result is a 3DPIceland fixture-specific probe-indicated temperature at a
2.00 mm mid-span deflection endpoint. It is not ASTM D648 or ISO 75 HDT.

- Specimen: 127 x 12.7 x 3.2 mm, printed flat.
- Clear support span: 110 mm.
- Centered moving load: nominal 54 g M20 nut, approximately 0.530 N.
- The central bolt does not add specimen load.
- Sensor: nearby under-specimen BlueDOT probe, thermapen.co.uk, FCC ID
  `2A167 BlueDot`; no user calibration option.
- Oven profile: 25 °C ambient start; observed checkpoints 50 °C at 1:50,
  100 °C at 3:26, 150 °C at 4:35, 200 °C at 6:53 and 250 °C at 10:30.
- The ramp is non-linear. The result is nearby probe temperature, not internal
  specimen temperature.
- One test and one recorded result per MaterialID.

Each saved result must reference an immutable centrally governed method version.
Changing a future method must create a new version; it must not reinterpret
historical values.

## Source workbook assessment

Read-only source: `Export fyrir hitamælingar.xlsx`, sheet `Sheet1`, table
`A1:J222`.

- 221 data rows and 221 unique MaterialIDs; no duplicate identifiers.
- 191 numeric `Hitamæling` values and 30 blank values.
- Numeric range 44-171 after the owner-confirmed MAT0107 correction; median 56.
- No formulas and no spreadsheet formula errors.
- Identity columns are descriptive evidence only. Canonical identity must be
  resolved from SQLite by exact MaterialID during import.
- The workbook does not provide measured date, notes, method version, raw trial
  series, sensor reading history or uncertainty. These fields must not be
  inferred.
- Workbook values are treated as candidate °C results because the owner linked
  the file to this method. The governed import preview must label the unit and
  require explicit confirmation before persistence.
- The initial `MAT0107 = 21` value was below the documented 25 °C ambient start.
  The owner confirmed it was an entry error and corrected it to 51 °C on
  2026-08-14. The corrected workbook has no remaining below-ambient values.

The final v61.0.1 preview against the configured 221-Material owner database
and corrected canonical tester seed uses workbook SHA-256
`6EBDA00A000B53A686F6AE13B5EEE3FEBFE6E6C786ECF7908FF644C7E90D67EA`.
It resolves all 221 exact MaterialIDs, classifies 191 inserts and 30 blanks,
reports zero issues and permits apply. The earlier 20-unknown-ID result came
from a stale 201-Material tester seed and is superseded; no data was written.

## Canonical data contract

Use an additive one-row-per-MaterialID native dataset. The persisted result is
a locale-invariant numeric Celsius value with optional ISO measured date and
notes, plus an immutable method-version reference and source provenance.

The centrally governed method record owns specimen geometry, orientation,
span, load, endpoint, sensor identity, calibration limitation, heating context
and observed ramp checkpoints. Material identity remains read-only projection
from canonical Materials data and is never copied as measurement truth.

Import must preview inserts, updates, unchanged rows, blanks and rejected rows.
It must fail closed on duplicate/unknown MaterialID, non-numeric values,
non-finite values, values outside the governed plausible range or unresolved
blocking anomalies. Blank workbook cells do not clear accepted SQLite results.

## Scope boundaries

v61 owns SQLite migration, native entry/filter/reset/clear/restart behavior,
method traceability, Excel round trip, Help, deterministic disposable automation
and Verification. Existing Tensile, Impact and Stiffness inputs/calculations are
unchanged.

Public reports, website publication, rankings and Engineering Score integration
remain out of scope pending separate owner approval. The source workbook remains
unaltered and is not a canonical runtime dependency.

## Automation and acceptance assessment

This changes a meaningful deterministic runtime contract. The existing safe
disposable measurement workflow should be extended to prove exact MaterialID
binding, locale input, auto-save, clear/cancel behavior, restart persistence,
method snapshot, workbook round trip and exact business-state recovery. Owner
acceptance remains manual for wording, grid usability and method readability.
