# v69.0.0 Legacy Impact Unit Correction — candidate evidence

Status: candidate; accepted application remains v68.0.3. Owner runtime acceptance is pending.
The owner confirms historical legacy readings used 2.743860924 J, 48.1603 mm² and 105.411 degrees.
This attestation does not authorize a different future calibration. No owner database was mutated for testing.

## Calculation and scope

For raw needle percentage p and no-sample angle a (degrees):

```
reboundDegrees = a × (1 - p / 100)
fraction = 1 - (1 - cos(reboundDegrees × π / 180)) / (1 - cos(a × π / 180))
absorbedEnergyJ = fraction × availableEnergyJ
impactKjM2 = absorbedEnergyJ × 1000 / netAreaMm2
```

The former path treated the already area-normalized maximum as Joules and divided by area again.
The corrected maximum at 100% is 56.97350149396911 kJ/m². Zero remains a measured zero.
The angular relationship and existing machine-loss-adjusted Available Joules are unchanged.
This fixes dimensional conversion; the rig's physical angle/loss calibration still needs its own instrumentation documentation.
That documentation question does not block correcting the unit error. No values are fitted to Izod or Charpy.
Direct Izod/Charpy input, statistics and historical records remain independent and unchanged.
The existing engineering Impact normalization reference (1000 kJ/m²) is deliberately unchanged.
Legacy Impact scores and dependent Overall rankings therefore change with the corrected data; no score retuning masks the correction.

## Preserved assumptions and outstanding rig documentation

The percentage is a linear position on the existing angular scale, not an energy percentage.
At p=0 the model uses the full 105.411-degree no-sample rebound angle and zero absorption;
at p=100 it uses zero rebound angle and all available energy. Cosine receives radians, never degrees.
The existing energy relationship is 0.523 kg × 9.81 m/s² × (0.63 - 0.07) m = 2.8731528 J,
then × (1 - 4.5/100) = 2.743860924 J. Machine loss is applied once upstream, not again during conversion.
The saved Available Joules value is the calculation input; changing mass/loss metadata does not silently recompute it.
Legacy specimen net area is 10 × 8 - 6.668 × 4.775 = 48.1603 mm², distinct from Izod/Charpy specimens.

For a future physical calibration review, the owner would need to supply:
- The pointer-scale definition or a photo documenting what its 0% and 100% marks physically represent.
- How 105.411 degrees was measured and its reference direction (vertical, release position or another datum).
- How the 4.5% machine loss was measured and whether it is a constant fraction across the full swing.

These are documentation questions, not reasons to change the accepted angular model in this unit-only correction.
The owner has already confirmed the historical applicability of the current settings; that question is resolved.

## Trace from existing seed measurements

Source: `.private/legacy-impact-audit-probe/evidence/audit.json`, Native MAT0001, Flat, sample 2.

| Quantity | Value |
|---|---:|
| Preserved raw text | `50,00` |
| Rebound degrees | 52.7055 |
| Rebound radians | 0.9198845089098714 |
| Fraction | 0.6886504439072887 |
| Absorbed energy J | 1.8895610433324632 |
| Former displayed kJ/m² | 814.6715675519962 |
| Corrected kJ/m² | 39.234827094774396 |

Old path: 56.97350149396911 kJ/m² × fraction / 0.0000481603 m² / 1000 = the incorrectly labelled 814.671568.
Correct path: 1.8895610433324632 J × 1000 / 48.1603 mm² = 39.234827094774396 kJ/m².

For the same ten-sample Flat set:

| Statistic | Former | Corrected |
|---|---:|---:|
| Mean kJ/m² | 826.4289194561852 | 39.80106468968571 |
| Sample SD kJ/m² | 77.45366581049736 | 3.730191781533294 |
| CV % | 9.372090446866764 | 9.37209044686676 |
| Count / Confidence | 10 / 10 | 10 / 10 |

Tiny CV rounding differences are floating-point precision; unit scaling changes mean/SD, not relative variation or counts.

## Disposable audit and recovery evidence

The unchanged canonical seed has SHA-256 `CEF1F9D5142578BBFE91D37A79BF11A6255CE550B8CA388D8582BF2D7D332D09`.
Read-only seed projection covers 211 materials, 421 native groups, 4172 valid readings and zero pending groups.
These are seed counts, not a claim about the active owner database.

The disposable fixture adds test cases: audit totals 212 materials, 422 native groups, 4193 slots,
4175 valid readings, 17 blanks and one invalid reading. One experimental row updates and one group remains pending.
Backup SHA-256: `F79E229BF1B6F019C9E47E064D1BCD850F258E7AB80A8F0320506D1213E92A38`.
Source fingerprint: `7E0B459710AB6BEE8C790BEAFF55E67B8D751A4ED71110267EDE117E74F4B949`.
The retained verification JSON reports PASS for backup/hash, raw/notes/time identity, stale-preview rejection,
invalid/future calibration rejection, unknown units/orientation pending, cache protection and repeated-apply idempotence.

Native Impact results are runtime derived. Experimental views project corrected results read-only; ordinary graph saves
preserve stored historical derived values when raw input is unchanged. The explicit Tools action owns audited cache updates.
Unknown units or orientations remain pending. Missing/invalid calibration has no fallback and Save Settings rejects it.
Historical generated reports and exports are retained; new output consumes corrected summaries.

## Verification and delivery

- Final Debug and Release: zero warnings/errors, including Help and nonfinite raw-validation changes.
- Initial disposable smoke and Full Data Verification: 479/479 PASS.
- Profile: `C:\Users\maddi\AppData\Local\Temp\3DPIceland-Automation\20260928142358-ebfa54b2`.
- CRUD PASS: profile 20260928142921-e1f5fcf7; exact business-state recovery.
- Reports PASS: profile 20260928143224-bdf0c9f7; 2103 catalog/root artifacts validated and hashed.
- Generated MAT0001 public report metadata matches corrected Flat mean 39.80106468968572 and Upright 6.4941805847971255.
- Reports assembly SHA-256: 945D3C34DB8EDF2F97D2F84F7C4F1ACAAA1B441349A7764FE1751063ECED0796.
- A final nonfinite raw-validation-only change follows that export run; its calculation/output path is unchanged.
- Final assembly SHA-256: 21CDBDED6ACBDCB820192A5AE7E13C44E6C6866BA2E523123881DE941AB3A088; final-binary smoke and Full Verification PASS 479/479 (profile 20260928144034-c3736129).
- Help coverage: PASS, 808 candidates, 63 menu items, 429 columns and 12 runtime surfaces.
- Dependency vulnerability gate reports no vulnerable packages; release documentation and diff checks pass.
- Owner readability and runtime acceptance remain pending.

The running owner application locks its normal Release directory; that directory remains untouched for this handoff.
.NET candidate is retained at `.private/v69-legacy-impact/App`, with the explicitly requested ZIP beside it.
No Production promotion, FTPS upload or live report publication is included.
Owner steps: `Docs/V69_LEGACY_IMPACT_ACCEPTANCE.md`.
