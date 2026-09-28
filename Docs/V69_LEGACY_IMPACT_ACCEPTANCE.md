# v69.0.0 Legacy Impact Unit Correction — acceptance

Owner runtime accepted on 2026-09-28: the owner completed Legacy Impact Recalculation and confirmed corrected values in normal Release.
The owner confirms historical legacy Impact samples use the stated settings. This increment includes a requested ZIP.

## Read-only inspection

1. **Where:** main application header. **Action:** open the candidate. **Expected result:** v69.0.0 Legacy Impact Unit Correction.
2. **Where:** actual Impact Measurements tab. **Action:** select an existing measured material.
   **Expected result:** raw percentages, notes and date unchanged; mean and SD use corrected kJ/m² units.
3. **Where:** actual Izod and Charpy tabs. **Action:** inspect an existing measured row in each.
   **Expected result:** direct instrument values and statistics unchanged.
4. **Where:** Help window. **Action:** search `Impact Measurements reference` and `Tools validation reference`.
   **Expected result:** explicit J/mm² formula and backup/recalculation boundary match the application.
   **Do not click:** unrelated Restore, Production or FTPS controls.

## Explicit repair acceptance

5. **Where:** actual application menu Tools → Legacy Impact Recalculation.... **Action:** open the preview.
   **Expected result:** current settings and correction scope appear before mutation. **Action:** choose No first.
   **Expected result:** cancellation leaves data unchanged.
6. **Where:** same Tools command. **Action:** reopen, review settings and choose Yes only for the intended database.
   **Expected result:** backup succeeds before derived recalculation; completion identifies retained JSON audit evidence.
   Raw percentages, notes and measured dates remain intact. A backup or validation failure stops the operation.
7. **Where:** Impact Measurements, Material Detail, Rankings Dashboard and experimental results where present.
   **Action:** inspect the same material and restart the application.
   **Expected result:** corrected legacy results agree across current views; direct Izod/Charpy stay unchanged.

No historical report file is overwritten by this correction. Website/publication acceptance is outside this local test.
Automated build, regression, recovery and Help evidence will be recorded before handoff; owner acceptance remains separate.
