# v70.0.1 Legacy Impact Radar Reference — acceptance

Candidate only; the normal v69.0.0 Release remains runtime accepted. v70.0.0 implementation/evidence remains preserved.
The owner requests legacy Impact as a separate visual radar reference and defers numeric Izod/Charpy references until more
materials are measured. No schema, raw-data, owner-database, modern weighting or publication change is authorized here.

## Búið/Breytingar

- Legacy radar reference = mean of available corrected Flat/Upright means / matching rig maximum × 100.
- Rig maximum = AvailableEnergyJ × 1000 / NetAreaMm2; accepted settings give about 56.9735 kJ/m².
- A missing orientation is omitted rather than zero; absent measurements/invalid calibration give no visual reference.
- 100% denotes the rig ceiling. Partial break, specimen bending and hammer drag can increase observed energy loss;
  this reference is not standardized material strength or a reliable clean-break ranking near the ceiling.
- Legacy reference cannot affect Overall, score coverage, Consistency, modern recommendations or method scores.
- Izod/Charpy raw means stay visible; their numeric scoring references and Overall remain pending by owner choice.
- Existing editing, raw percentages, notes, dates and historical exports remain unchanged. No new editable surface or ZIP.

## What to Test — read-only inspection in the actual application

1. **Where:** candidate executable at `C:\3DPIceland-App-Codex\.private\v70-legacy-radar\App\3DPIcelandFilamentDB.exe`.
   **Action:** open the application. **Expected result:** v70.0.1 Legacy Impact Radar Reference in the title.
   **Do not click:** Recalculate, Restore, Update, Production or FTPS; this inspection needs none of them.
2. **Where:** Materials → select an existing material → Material Detail → Charts.
   **Action:** inspect Legacy Impact. **Expected result:** a separate rig-reference percentage, not a modern score.
   Compare against the corrected Flat/Upright means in Impact Measurements using the formula above.
3. **Where:** Material Detail → Analytics → Radar score groups.
   **Action:** select the same material. **Expected result:** its Legacy Impact reference is visible; other missing axes
   remain gaps. Read Score coverage: legacy reference does not increase modern coverage or unlock Overall.
4. **Where:** Material Detail → Mechanical, and the Izod Measurements / Charpy Measurements tabs.
   **Action:** inspect saved measurements. **Expected result:** unchanged raw instrument values and statistics.
   Izod/Charpy scores still await approved references; the new legacy reference does not substitute for them.
5. **Where:** Rankings Dashboard and Material Detail → Recommendations.
   **Action:** inspect Overall and impact-related guidance. **Expected result:** no modern winner or recommendation
   is created from the legacy radar reference; incomplete modern evidence stays unavailable.
6. **Where:** Help window opened from the Help menu.
   **Action:** search `Impact Measurements reference`, then `Material Detail — Charts reference` inside Help.
   **Expected result:** the mean/orientation formula, rig ceiling and partial-break/drag limitations are explained.

## Automated acceptance

Full Verification should cover mean-of-orientations, one-orientation fallback, zero/missing/invalid calibration, matched
capacity, and invariance of all modern scores/coverage when legacy readings change. Existing disposable smoke and report
scenarios cover read-only navigation and downstream projection. No new input workflow requires typing acceptance.
Debug/Release, Help/documentation gates and Full Verification 481/481 PASS. Owner visual/readability acceptance remains pending. Evidence: Reports/V70_LEGACY_RADAR_EVIDENCE.md.

Owner acceptance 2026-09-28: approved the radar and requested promotion to the accepted application. Exact tested v70.0.1 bytes (54 manifest files) are now in App/FilamentDbApp/bin/Release/net9.0-windows; DLL SHA-256 F34FB9B2C5B56A1959CD877DDDF3914ECCEFB0F10F3AE723F0C227279EEFE80F. Full Verification 481/481 and reports 2,103 artifacts PASS; no rebuild or data mutation. Numeric method references remain deliberately deferred. Application acceptance does not publish installer/update packages; those packages have not received exact-byte acceptance.

