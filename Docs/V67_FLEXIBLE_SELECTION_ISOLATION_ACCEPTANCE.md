## v67.0.9 - Flexible Selection Isolation

Owner reports Shore values appearing to move between sessions. Old parent CurrentCell could override a new selected row during
reentrant binding, while keyboard-only parent cell movement was ignored. Reading edits were also committed after parent changes.
Guard parent transitions, prefer added/selected rows, synchronize current cells and handle keyboard parent navigation.
Commit and validate the outgoing specimen before switching; reject stale/out-of-session specimens for reading actions.
Saved measurement IDs and values are not reassigned. Existing incorrect historical associations are not guessed or rewritten.
Full Verification adds synthetic STA WPF two-session/same-label Shore selection and ID/value isolation checks using real helpers.
The fixture does not claim actual owner edit-validation or restart coverage; existing SQLite contracts remain and owner test follows.
Help and existing AutomationId ownership updated; no new UI field, schema, seed or automated mutation authority.
Owner switch/edit/restart acceptance pending. Previous GitHub push approval block remains unresolved; no push attempted.
Owner runtime, actual application:
1. Where: Flexible Material Testing > choose Session A > Shore Specimen 1 > Shore Hardness.
   Action: note the actual saved values. Choose Session B and its Shore specimen, then return to A.
   Expected: each specimen displays its own values, even though both labels are Shore Specimen 1.
2. Where: session/specimen grids. Action: navigate with arrow keys and click row headers, then switch back.
   Expected: selected session, specimen and Shore readings stay aligned. Do not click any Delete controls.
3. Where: intended Shore entry. Action: enter one actual value with one click, use Tab/Shift+Tab/arrows, change session and return.
   Expected: value remains with its original specimen. Close/reopen and confirm it still belongs to that specimen.
4. Where: any invalid pending entry. Action: attempt a parent switch.
   Expected: the old parent/readings remain together with an actionable status; correct the value before switching.
No restore, bulk correction or reassignment is authorized or needed for these checks. The tests are in the actual app, not Help.

Independent review traced the stale-CurrentCell reentrant callback and missing parent keyboard selection. Fixes address both.
The canonical shared one-click/editor/deferred-save handlers remain; outgoing edits now finish before parent views change.
Final candidate: Debug/Release zero warnings/errors; Help 801/801; documentation and diff audits PASS.
Disposable smoke 20260912174014-cb93c4aa: Full Verification 467/467 PASS, including synthetic WPF selection isolation.
Exact baseline/final database SHA-256: 3C4F9F5BC17BAB4A72AE753CE1AE478E77C91551F6438601654B0A03B1B82A10.
Exact baseline/final business hash: 2BA7E47D106B59B77AC10BC97508C96AD49C05095CB9EBA13778DDBD26B77FEF.
Evidence retained in .private/v67-selection-isolation. Normal Release updated; owner switch/edit/restart acceptance pending.
No reassignment or repair of owner measurement associations performed. No push; earlier automatic-review block unresolved.
