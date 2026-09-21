## v67.0.8 - Shore Template Defaults

Owner reports empty Shore rows with blank time and inherited 10 mm thickness despite new templates already using 10 s/8 mm.
Add Shore Reading now uses the dedicated saved Shore defaults rather than inheriting the selected parent specimen thickness.
Once per database, unused rows with blank hardness/time and blank or legacy 9/10 mm thickness receive 10 s/8 mm.
A verified Manual Backup precedes changes; row updates and AppMeta completion marker commit together. No schema version change.
Measured rows and other custom values are excluded; subsequent deliberate edits are not reset on restart.
Full Verification tests correction scope, preservation and idempotence; existing editor/keyboard/save handlers are unchanged.
Help covers defaults and one-time repair. Owner app was open; builds use isolated ArtifactsPath until Release can be updated.
Owner runtime acceptance pending. No GitHub push or publication; the previous push approval question remains unresolved.
Owner runtime (actual application, not Help): after the Release update, reopen Flexible Material Testing and select the old empty
Shore rows shown in the report. Expected: Reading time s=10 and Thickness mm=8; hardness remains blank.
Select the intended specimen and Add Shore Reading only if another actual reading is needed. Expected: the saved Shore defaults.
Close/reopen after intended edits: custom edits persist; repair does not repeat. Do not click Delete or Restore controls.
The verified backup is retained in the configured database backup folder when a repair is needed.
Independent read-only review confirmed scope/atomicity and identified startup failure handling. Repair failures now leave original
rows accessible with a red status and an unset completion marker. Injected SQL failure verifies full rollback and no marker.

Final candidate: normal Debug/Release zero warnings/errors; Help 801/801 and documentation/diff audits PASS.
Disposable smoke 20260912170558-8c239b14: Full Verification 466/466 PASS, including repair scope, rollback and idempotence.
Exact baseline/final database SHA-256: 384149A60DA7836E71EFECE2208566AA99BC161D97BF5D0D8F84694804858F78.
Exact baseline/final business hash: 2BA7E47D106B59B77AC10BC97508C96AD49C05095CB9EBA13778DDBD26B77FEF.
Evidence: .private/v67-shore-defaults. Owner app closed during work; normal Release updated. Repair runs at next owner startup.
Owner runtime acceptance pending; no commit/push attempted. Isolated build artifacts removed after all consumers completed.

Owner runtime acceptance: 2026-09-12, confirmed corrected Shore display looks good after restart.
Implementation complete. Commit/push remain pending resolution of the earlier automatic-review approval block.
