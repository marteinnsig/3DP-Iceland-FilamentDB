## v67.0.5 - Flexible Session Templates

Candidate implementation; owner runtime acceptance pending.
New sessions prepare 10 compression specimens with 10/30 s compression and one Recovery row each, plus one separate Shore coupon.
Shore coupon: 50 x 50 mm, default 8 mm thickness, five empty location readings at 10 s. Force/recovered height/hardness stay blank.
Settings own sample count, 230 C, extrusion 1.1, 2 walls/5 top/3 bottom, Shore dwell/thickness; saved defaults remain snapshots.
Eight new settings use existing Settings Manager validation/commit workflow; Flexible shared click/keyboard/save behavior is retained.
Correct existing print layers (2/5/3) confirms all-session scope, creates a verified Manual Backup, then saves transactionally.
Only print layers change on existing specimens; geometry, temperatures, extrusion and raw results remain unchanged.
Full Verification extends template/statistics and actual SQLite graph persistence/rollback tests; no new tester mutation authority.
Help and control inventory cover the template/defaults and correction action. No schema/seed refresh or website publication.
Owner runtime handoff (actual application, not Help):
1. Where: Flexible Material Testing. Action: Add Test Session, choose the intended Material in its session row.
   Expected: 10 Compression specimens + Shore Specimen 1; 20 blank compression rows, 10 Recovery rows and five Shore readings.
   Do not click Delete Test Session, Delete Specimen or Delete Selected Reading.
2. Where: selected Compression specimen > Compression/Recovery. Action: enter an intended force or TVL value with one click.
   Expected: immediate typing, Tab/Shift+Tab/arrows preserve focus and save; untouched templates contribute no measured results.
3. Where: Shore Specimen 1 > Shore Hardness. Action: inspect five locations and enter only actual readings; select actual A/D scale.
   Expected: 10 s, 8 mm; five readings form one independent specimen. Restart preserves entered values and all prepared rows.
4. Where: Settings Manager > Flexible Material Testing. Action: inspect defaults; change an intended future default and Save Settings.
   Expected: new rows use saved settings; existing rows retain their snapshots. Invalid values are rejected.
5. Where: Flexible Material Testing, buttons above specimen list. Action: Correct existing print layers (2/5/3), then Yes.
   Expected: verified backup path in status; all existing Flexible specimens show 2/5/3, measured values unchanged.
   This specific correction is authorized. Do not click adjacent Delete controls or Restore.

Tester assessment: factory blank/n/override checks and in-memory SQLite persistence/forced-failure rollback run in Full Verification.
The existing disposable smoke runner owns verification; its CRUD mutation authority is not expanded. Manual input/visual acceptance
remains required. No removed input surface or compatibility fallback; existing defaults and reading entry points remain supported.
Initial disposable run 20260912153434-96d7e46b passed the new template/persistence contract but failed the older exact five-settings
check. That check now expects five retained plus eight new settings and validates the new names/defaults; no behavior gate removed.

Final candidate verification: Debug/Release zero warnings/errors; Help 803/803 and release-documentation/diff audits PASS.
Disposable smoke profile 20260912153714-70198f24: Full Verification 464/464 PASS, including template and SQLite rollback checks.
Exact baseline/final database SHA-256: 1D0A209E39A8CCF147291456E9C3AF722B4F7F9A2D1DACE9B9A8073B6D215330.
Exact baseline/final business hash: 3E50F76D28278A24D9AF6DB0C72919074422D6336E4848FC1211928946A2E263.
Evidence retained in .private/v67-session-templates. Normal Release directory updated; owner runtime acceptance pending.
Existing owner specimens have not been bulk-corrected yet: the authorized backup-first action is ready in the UI.

Owner acceptance 2026-09-12: new session template is correct; bulk print-layer correction succeeded.
The owner then requested removal of the temporary button and legacy display column, delivered in v67.0.6.
The correction handoff above is historical and must not be repeated after upgrading to v67.0.6.

Owner accepted final session ordering on 2026-09-12; screenshot and final review confirm retired controls absent.
v67.0.5-v67.0.7 complete.
