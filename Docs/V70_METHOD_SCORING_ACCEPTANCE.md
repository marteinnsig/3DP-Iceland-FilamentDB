# v70.0.0 Method-aware Engineering Scoring — acceptance

Candidate only; v69.0.0 remains the runtime-accepted application. Owner explicitly defers numeric Izod/Charpy references until more materials have been measured (2026-09-28).
Default method scores and Overall are therefore unavailable; this is intentional and does not remove measured kJ/m².
An approved pair of method-specific references will require a later versioned policy release, not a Settings edit here.
No owner database write, schema change, Production, FTPS, new editable input or ZIP is required for this increment.

## Búið/Breytingar

- Retain raw legacy Impact Flat/Upright and corrected physical results as historical reference; retire its score.
- Remove legacy Impact from Consistency, Overall and current advisor/recommendation weighting.
- Keep Izod and Charpy independent. Fixed references cannot change with filters or be inferred from one measured material.
- Require five equal Overall families: Tensile, combined Izod/Charpy impact, Stiffness, Consistency and Layer Adhesion.
- Require both method scores for the impact family. Thermal remains independent; missing measurements are never zero.
- Show score coverage, pending references and eligible-profile counts. Available axes stay visible without Overall.
- Preserve all measurements and input workflows. Measurement completeness and score eligibility remain separate concepts.

## What to Test — read-only inspection in the actual application

1. **Where:** application title. **Action:** close the existing app, then open `.private/v70-impact-scoring/App/3DPIcelandFilamentDB.exe` from the repository.
   **Expected result:** v70.0.0 Method-aware Engineering Scoring. **Do not click:** Update, Restore, Production or FTPS.
2. **Where:** Materials, then select a material with saved Izod/Charpy results; open Material Detail → Mechanical.
   **Action:** compare its measured means with the corresponding Izod Measurements and Charpy Measurements tabs.
   **Expected result:** the same instrument kJ/m² and statistics; no changed raw reading.
3. **Where:** Material Detail → Charts. **Action:** inspect Izod, Charpy, legacy Impact and Overall.
   **Expected result:** method references are pending, method scores and Overall are unavailable; other eligible axes remain.
4. **Where:** Material Detail → Analytics. **Action:** select the material in Radar score groups and read Score coverage.
   **Expected result:** explicit pending/incomplete coverage; missing axes have gaps and no fabricated zero marker.
   Change Chart Mode to a grouped view. The group reports eligible/total profiles; it never implies every member is complete.
5. **Where:** Rankings Dashboard. **Action:** select Overall, Izod and Charpy, then a measured non-impact metric.
   **Expected result:** pending-reference scores have no rankings; supported measured non-impact rankings remain usable.
6. **Where:** Help window, opened from the Help menu. **Action:** search `Material Detail — Charts reference` and
   `Material Detail — Analytics reference` inside Help. **Expected result:** fixed references, five families, gaps and coverage
   agree with the actual read-only views. This step searches Help; it does not edit Settings.

## Automated acceptance boundary

Extend existing Full Verification contracts for fixed-reference/filter invariance, no singleton automatic100, equal-family
weighting, missing/zero semantics, retired legacy isolation and score propagation. Existing smoke/ranking navigation and
local report export remain disposable-profile operations. No new editable surface means no new typing acceptance contract.
Debug/Release, Help inventory/drift, documentation and Full Verification evidence must pass before final runtime handoff.
Owner runtime/readability acceptance remains pending; numeric references are deliberately deferred by the owner, not an incomplete v70 implementation. No candidate is canonical by implication.

## Owner reference decision: deferred

Owner reports nominal hammer energies: Charpy 2 J and Izod 2.75 J. With the stated 4 x 8 mm
net section (32 mm²), energy x 1000 / area gives theoretical ceilings of 62.5 and 85.9375 kJ/m².
At 80% nominal energy these are 50 and 68.75 kJ/m². They are instrument-range-derived candidates,
not ISO material-score definitions or verified instrument calibration. Owner defers fixed
100-point references until more materials, including strong legacy performers, have been measured; neither pair is enabled automatically. Any approved
references should remain fixed when a hammer is changed so scores retain their meaning.
Reference: https://www.instron.com/fr/resources/blog/2022/january/5-steps-to-selecting-hammers-for-your-pendulum-impact-testing-machine/
Owner acceptance 2026-09-28: approved the radar and requested promotion to the accepted application. Exact tested v70.0.1 bytes (54 manifest files) are now in App/FilamentDbApp/bin/Release/net9.0-windows; DLL SHA-256 F34FB9B2C5B56A1959CD877DDDF3914ECCEFB0F10F3AE723F0C227279EEFE80F. Full Verification 481/481 and reports 2,103 artifacts PASS; no rebuild or data mutation. Numeric method references remain deliberately deferred. Application acceptance does not publish installer/update packages; those packages have not received exact-byte acceptance.

