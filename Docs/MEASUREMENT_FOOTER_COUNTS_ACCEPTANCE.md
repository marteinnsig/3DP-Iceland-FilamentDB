# Measurement footer correction — 2026-10-11

Tensile, Impact and Stiffness previously counted one placeholder row for every material.
They now reuse the existing measurement-presence predicates used by test status and tab summaries.
Count a material row once when either orientation contains a numeric raw sample; Stiffness uses revolutions/degrees.
Zero is a reading; blanks and calculated fields alone are not readings. Materials remains the total catalog.
Izod/Charpy count measured materials, Heat counts measured rows, Flexible sessions counts all sessions.
Counts remain independent of filters. Existing save/status callbacks refresh the footer; no editor or save behavior changes.

Full Verification gains a regression for blank, zero and orientation handling. Existing disposable smoke verification owns it;
no new scenario authorization or AutomationId is required. Help start-here and footer tooltip describe the corrected units.

Owner runtime acceptance received 2026-10-11: owner confirms the counts are correct and authorizes Full Verification and closure.

Validation: Debug/Release PASS with zero warnings/errors; Help coverage, documentation audit and diff check PASS.
Disposable smoke using C:\Seed-Database\filamentdb.sqlite failed at CaptureWindow/Graphics.CopyFromScreen
with Win32Exception (6), "The handle is invalid", before Full Verification. Retain the failed profile.
Interactive desktop rerun PASS: smoke and Full Data Verification 483/483, no failures or not-applicable checks.
Measured material footer counts regression PASS. Database byte hash and business-state hash unchanged after shutdown.
Evidence: Reports/MeasurementFooter/2026-10-11 (run-result.json, verification.json/TXT and screenshots).
Accepted profile: 20261011154457-00d6048d, retained temporarily for review. Failed profile remains retained.
Exact tested normal Release EXE SHA-256: F5BC791D155A3F25113EFD7C2826E5D587194CAD2A65B382C403C3BE0A2CE4FE.
Exact tested normal Release DLL SHA-256: 00D9B9A48ACFD3D2743A64FA1401DDA428BF57620F452E37DB2DD0E4D6B77E11.
The tested working candidate includes existing pending Materials autosave changes; this closure commits only the footer correction.

1. **Where:** normal application, bottom status bar. **Action:** restart the candidate and inspect counts.
   **Expected result:** Materials includes all materials; Tensile/Impact/Stiffness exclude materials without raw readings.
   **Do not click:** Restore, Recalculate, Production, FTPS or Delete.
2. **Where:** Tensile Measurements, then Impact and Stiffness tabs. **Action:** compare each With samples summary with its footer count.
   **Expected result:** the totals agree. Filtering a tab does not change the footer total.
3. **Where:** Help window, Start-to-finish workflow. **Action:** read Bottom status bar.
   **Expected result:** the explanation distinguishes materials with readings from all catalog materials and sessions.

No owner database mutation is needed for this read-only acceptance.
