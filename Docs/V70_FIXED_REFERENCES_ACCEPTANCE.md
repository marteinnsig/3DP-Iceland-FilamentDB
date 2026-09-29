# v70.0.2 Fixed Izod and Charpy Score References

## Búið / Breytingar

Owner confirmed 80 kJ/m² for both methods on 2026-09-29. Each score is mean / 80 × 100, capped at 100.
0 -> 0; 3.218 -> 4.0225; 40 -> 50; 80 -> 100; 120 -> 100 points. The raw 120 kJ/m² remains 120.
The same policy drives app radar, rankings, Overall and exports. Legacy Impact and five-family weights are unchanged.
Missing results stay blank. Overall still requires all five families, including both Izod and Charpy.
No recalculation, data migration or editing is needed. This is a comparison scale, not an ISO rating or hammer limit.
Future reference changes require an explicit policy update; filters and newly measured stronger materials do not change it automatically.

## What to Test

1. **Where:** `.private/v70-fixed-references/App/3DPIcelandFilamentDB.exe`.
   **Action:** open the candidate. **Expected:** title shows v70.0.2 Fixed Izod and Charpy Score References.
   **Do not click:** Recalculate, Restore, Production, Update or FTPS; none is needed for this read-only check.
2. **Where:** Materials → select an Izod/Charpy-measured material → Material Detail → Analytics.
   **Action:** select its row under Radar score groups. **Expected:** both measured methods have points;
   a mean of 40 kJ/m² gives 50/100. Missing methods remain gaps; Legacy Impact stays unchanged.
3. **Where:** Material Detail → Charts and Mechanical.
   **Action:** compare the method means and scores. **Expected:** unchanged means, score = mean / 80 × 100,
   capped at 100. Rounding of a displayed mean can cause a small difference from the unrounded calculation.
4. **Where:** Rankings Dashboard → metric selector → Izod, then Charpy, then Overall.
   **Action:** inspect the displayed values. **Expected:** method scores use the fixed scale; Overall requires
   all five families. Filtering the visible materials must not rescale a material's score.
5. **Where:** Help menu → Help window.
   **Action:** search `Izod`, then `Rankings Dashboard` inside Help. **Expected:** 80 kJ/m² references and missing-data rules.

Normal Release remains accepted v70.0.1 until owner acceptance of this candidate. No ZIP or live publication.

Owner runtime acceptance 2026-09-29: fixed 80 kJ/m² scores and enlarged radar accepted.
Exact v70.0.3 build promoted to App/FilamentDbApp/bin/Release/net9.0-windows without rebuilding.
All 54 target files match the accepted manifest; prior Release files backed up in .private/v70-large-radar/pre-promotion-release.
DLL SHA-256: 0D5C7B37488BD6124E3D4D31B63191FAD1562CBA2826105500E824D0CFEDA6C8.
Debug/Release and Help/documentation gates PASS. v70.0.2 Full Verification 481/481 and reports PASS remain
calculation evidence; v70.0.3 cosmetic scaling is owner visually accepted, not claimed as a fresh Full Verification run.
No owner-data mutation or external publication. Installer/update packages remain outside this local promotion.
