## v67.0.6 - Flexible Control Cleanup

Owner accepted the v67.0.5 session template and successful existing 2/5/3 print-layer correction on 2026-09-12.
Remove the completed correction button/handler and Legacy Experimental Run display column from Flexible Material Testing.
LegacyExperimentalRunId remains owned by saved model/database migration/read/write and historical roundtrip verification.
The shared SQLite graph writer and forced-failure rollback tests remain supported persistence contracts, not retired UI code.
No replacement editor, new input behavior, schema or owner-data mutation. Help and inventory remove the retired destinations.
Retirement gate checks handler/column absence; disposable smoke retains its existing authorization. Owner visual acceptance pending.
Review covered XAML, handler and callers, field model, graph persistence/migration, reporting consumers, tests, Help and documents.
No UI caller or state remains for the correction. Legacy metadata must roundtrip supported historical databases; no removal planned.
Owner runtime: open the actual Flexible Material Testing tab. Inspect the specimen toolbar and session headers.
Expected: correction button and Legacy Experimental Run column absent; session/template and normal input remain available.
Read-only inspection only; do not click Delete controls. No Help navigation or data changes are needed for this check.
Final candidate: Debug/Release zero warnings/errors; Help 801/801; release documentation and diff audits PASS.
Disposable smoke profile 20260912155030-144fe9c2: Full Verification 465/465 PASS, including retirement and historical roundtrips.
Exact baseline/final database SHA-256: 95ED75DD6FAB499009A3C64070376CB4772294ABCA8211926517C7975BD1EFE2.
Exact baseline/final business hash: 3E50F76D28278A24D9AF6DB0C72919074422D6336E4848FC1211928946A2E263.
Evidence: .private/v67-control-cleanup. Normal Release directory updated; owner visual acceptance pending.

Owner accepted final session ordering on 2026-09-12; screenshot and final review confirm retired controls absent.
v67.0.5-v67.0.7 complete.
