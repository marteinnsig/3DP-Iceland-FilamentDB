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

The final owner-approved source workbook for v61.0.3 uses SHA-256
`5CC2742C6DEA382CDCCC9D260135DB3377DFC9B754D9106230B6C8CCC3AE58CE`.
The earlier v61.0.1 preview recorded different workbook bytes with SHA-256
`6EBDA00A000B53A686F6AE13B5EEE3FEBFE6E6C786ECF7908FF644C7E90D67EA`;
the owner explicitly confirms the current OneDrive workbook as the canonical
measurement source on 2026-08-14 before any v61.0.3 population.
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

## v61.0.4 accepted thermal analytics contract

Code and canonical-seed research on 2026-08-14 establishes these current
contracts:

- `EngineeringScoringService` normalizes tensile, impact and stiffness against
  fixed physical references and clamps each result to 0-100.
- Existing Overall is the unweighted average of whichever of the five current
  axes are available. Adding a sixth value to that average would silently
  change historical scores, ranks, awards and downstream saved outputs.
- Rankings use the selected axis directly, with Overall as the default. Missing
  values are excluded from that axis rather than converted to zero.
- The accepted thermal population covers 191 of 221 active Materials. Global
  coverage is sufficient for a separate axis, but several material families
  have only one to three results. Family-relative normalization would therefore
  be unstable and misleading.

The owner-accepted versioned contract is
`3dp-thermal-analytics-fixture-v1`:

- Raw result remains the primary evidence in °C with its immutable fixture
  method version and limitation wording.
- Thermal Score = `clamp(ResultTemperatureC / 200 °C * 100, 0, 100)`.
- The fixed 200 °C reference follows the existing absolute-reference score
  pattern and preserves headroom for future high-temperature materials such as
  PPS. It never moves when Materials are added, archived or filtered.
- The current accepted dataset produces 22.0-85.5 points; no result saturates at
  100. Family averages remain ordered by absolute observed thermal
  resistance rather than by within-family percentile.
- Missing thermal measurement means no Thermal Score, rank or percentile. It is
  never zero and does not reduce another score.
- Thermal ties use the existing deterministic label/MaterialID ordering after
  equal unrounded score and raw °C values.
- Require at least two measured peers for a displayed rank/percentile. A lone
  measured result may show raw °C and score but not `#1 of 1` context.
- Keep the legacy five-axis Overall and its historical meaning unchanged in
  v61. Thermal becomes an independent selectable ranking/radar/award axis.
- Do not introduce a second thermal-inclusive Overall in v61. Any future
  composite requires a new named/versioned score rather than relabeling Overall.

This contract avoids dataset-dependent global/family min-max normalization,
percentile-as-score and missing-as-zero behavior. It is the single contract for
all v61.0.5-v61.0.7 consumers; those increments may not create local formulas.

Automation assessment: this research/documentation increment changes no runtime
contract, control or Help destination, so AutomationRunner and Help coverage do
not change in v61.0.4. v61.0.5 must add deterministic
formula, fixed-reference, saturation, missing-value, peer-count, tie and legacy
Overall invariance checks before UI integration.

The owner selects the 200 °C reference on 2026-08-14 specifically to retain
headroom above the current 171 °C maximum for future PPS and other
high-temperature material measurements. The remaining normalization,
missing-data, peer-count and legacy Overall rules are accepted with it.
