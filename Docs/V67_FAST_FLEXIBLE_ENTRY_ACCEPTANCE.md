## v67.0.10 - Fast Flexible Entry

The Flexible tab used WPF DataGrid property-change bindings and synchronous graph/evidence work on each cell completion.
It now shares the Materials/Tensile painted table and overlay editor across sessions, specimens and all four reading types.
Typed text stays local until commit; detached validation protects coupled Recovery heights and source event subscribers.
Calculated values refresh on commit. Graph/evidence saves coalesce after 0.8 s with no editor active; parent switches,
reading actions and close flush pending changes. Rejected input remains available for correction; save failures block navigation.
Natural labels, same-parent selection, read-only skipping and specimen IDs are preserved. Defaults/statistics/schema are unchanged.
Help and Full Verification own the six fast hosts, detached TVL validation, numeric navigation and rejection/retry contracts.
The existing disposable smoke scenario executes these checks; no new owner-data or destructive automation authority.
Hidden DataGrid column/selection bridges have an explicit v67.0.11 retirement after owner accepts this replacement.
Debug/Release and owner runtime evidence are recorded below. No commit/push or publication; prior push approval remains unresolved.

Owner test - actual application, not the Help window:
1. Where: Flexible Material Testing > intended session > Shore Specimen 1 > Shore Hardness.
   Action: click Hardness value once and enter an actual reading, then Tab/Shift+Tab and Up/Down.
   Expected: immediate typing, predictable focus, five distinct readings retained. Do not click Delete controls.
2. Where: same tab > a Compression specimen > Compression, then Recovery.
   Action: enter actual Force N and TVL contact offset, use Tab across the row and Shift+Tab back.
   Expected: calculated cells are skipped; values update after commit; no long pause before typing.
3. Where: session/specimen tables. Action: change a note, select another session, then return.
   Expected: selected specimen remains when clicking another cell in the same parent; readings stay with their original specimen.
4. Where: Shore Hardness > Hardness value. Action: type nonnumeric text and attempt to switch session.
   Expected: red validation status, input remains for correction and parent/readings stay together. Press Escape to cancel.
5. Where: Flexible Material Testing. Action: finish an actual edit with Enter, close/reopen app, revisit the same specimen.
   Expected: saved value persists. If saving fails, close is blocked and the status explains why.
6. Where: Help window > search Flexible Material Testing. Action: read input/save guidance.
   Expected: it matches the actual controls above. No Restore, Recalculate, FTPS or Delete action is part of this test.

Final v67.0.10 candidate evidence (2026-09-12):
Debug and Release builds: zero warnings/errors. Help coverage 801/801, documentation and git diff audits PASS.
Disposable final smoke 20260912180523-813d1d7c: Full Verification 468/468 PASS, including the new fast-input contract.
Baseline/final database SHA-256: B3447DB0420DB2039C868ECB3F366C8479032EFB7FBB75A933FF002EDBC3B09C.
Baseline/final business hash: 2BA7E47D106B59B77AC10BC97508C96AD49C05095CB9EBA13778DDBD26B77FEF.
Evidence retained in .private/v67-fast-flexible-entry. artifacts is empty; normal Release remains runnable.
Independent review corrected same-parent selection reset, material-name sorting and detached Recovery event subscriptions.
No real owner UI timing is claimed from these deterministic checks. Owner input/switch/restart acceptance remains pending.
No owner data mutation, commit, push or publication in this increment. v67.0.11 retirement waits for runtime acceptance.

2026-09-12 owner follow-up: typing improved, but first character in each cell still felt delayed.
The Flexible editor now retains its templated TextBox between cells and focuses it before returning from activation.
Flexible Tab/arrow destinations activate synchronously instead of waiting behind Dispatcher Input/ContextIdle callbacks.
Other Materials/Tensile callers keep their accepted default behavior. Rejection still retains the same raw input for correction.
The existing Verification contract now checks repeated activation reuses one editor, alongside rejection/retry and ownership.
Help assessment: existing single-click/keyboard/save guidance remains accurate; no label, unit, default or save contract changes.
This candidate supersedes the prior v67.0.10 bytes; first-character timing still requires owner runtime confirmation.

First-character correction final evidence: disposable smoke 20260912181341-0bd7db20, Full Verification 468/468 PASS.
Baseline/final SQLite SHA-256: 24367E0657072EE774CC13B81EFC7F487C60F69378588A5E20EA69EF3CFBE9CF.
Baseline/final business hash: 2BA7E47D106B59B77AC10BC97508C96AD49C05095CB9EBA13778DDBD26B77FEF.
Evidence: .private/v67-fast-flexible-entry/first-character. Normal Release updated; artifacts empty.
Owner first-character timing test remains pending; no commit/push/publication or owner database mutation.

2026-09-12 regression correction: owner reports blue selection border with no editable text after immediate-focus change.
Reverted the unaccepted reusable editor and synchronous activation to the prior working Materials/Tensile lifecycle.
ScrollChanged previously closed an editor on layout/extent changes too; forced UpdateLayout exposed this close-before-focus path.
Flexible now commits on actual horizontal/vertical scroll only. Other renderer callers retain their existing behavior.
Regression fixture invokes extent-only and actual-offset ScrollChanged events: the first retains the editor, the second commits.
Prior editor-reuse evidence is superseded; it did not prove visible focus usability. Owner single-click typing acceptance is required.
Help assessment: unchanged input/save/keyboard contract; no new controls or units. v67.0.11 retirement remains pending.

Editor-restoration final evidence: disposable smoke 20260912182018-6378fb7a; Full Verification 468/468 PASS.
Includes layout-only ScrollChanged retaining the active editor and real scrolling committing it.
Baseline/final SQLite SHA-256: 8F96C38CEF897FB75044148A87F50A6ABFD91470AFEFA4598226CB2478111322.
Baseline/final business hash: 2BA7E47D106B59B77AC10BC97508C96AD49C05095CB9EBA13778DDBD26B77FEF.
Evidence: .private/v67-fast-flexible-entry/editor-restoration. Debug/Release zero warnings/errors; Help/docs/diff PASS.
Normal Release updated; artifacts empty. Owner typing acceptance pending; no commit/push/publication or owner data mutation.

2026-09-12 timing clarification: owner confirms immediate typing works after a 2-3 s pause following click.
New activation gives Flexible TextBox focus synchronously without UpdateLayout, template forcing or editor reuse.
The existing guarded deferred callback remains only as a fallback when immediate keyboard focus is unavailable.
Automation-only Verification now opens a transient loaded WPF fixture and sends text composition immediately after activation.
It checks keyboard focus, first-digit replacement and commit, using only a synthetic row; no owner data or system input injection.
The fixture is gated by AutomationRuntimeProfile.IsActive. Normal owner Verification does not open this transient test window.
Help assessment: existing single-click/save/keyboard contract remains unchanged. Owner immediate-click typing still required.

Loaded-focus final evidence: disposable smoke 20260912182755-602c25ec; Full Verification 468/468 PASS.
Automation-only loaded WPF fixture passed immediate keyboard focus, first-digit text composition and explicit commit.
Baseline/final SQLite SHA-256: C3BF2E3E98D23E412FA2859886FE9107EC7041B657B62F2A23057239CD2A3C74.
Baseline/final business hash: 2BA7E47D106B59B77AC10BC97508C96AD49C05095CB9EBA13778DDBD26B77FEF.
Evidence: .private/v67-fast-flexible-entry/loaded-focus. Debug/Release zero warnings/errors; Help/docs/diff PASS.
Normal Release updated; artifacts empty. Owner immediate-click timing acceptance remains pending. No commit/push/publication.

2026-09-12 owner decision: roll back the last immediate-focus change and stop further latency work.
Owner observes the first-character delay in Materials too and accepts retaining that delay with working typing.
Removed only the latest synchronous-focus block and its misleading loaded-focus fixture; restored prior deferred focus.
The earlier layout-only ScrollChanged protection, selection isolation and save contracts remain intact.
The isolated fixture had passed but did not represent the owner's full nested app surface; no timing resolution is claimed.
No new input controls/defaults or Help behavior. Existing rejection/scroll/ownership Verification remains.
Do not proceed to further focus optimization or v67.0.11 bridge retirement in this batch; owner explicitly stopped that work.

Owner rollback final evidence: disposable smoke 20260912183352-f481d9e6; Full Verification 468/468 PASS.
Baseline/final SQLite SHA-256: 482B2283089DBFCEE0C9FD53ABB313BA98CFAA484826A699E0F919A4CCFDB6C4.
Baseline/final business hash: 2BA7E47D106B59B77AC10BC97508C96AD49C05095CB9EBA13778DDBD26B77FEF.
Evidence: .private/v67-fast-flexible-entry/owner-rollback. Debug/Release, Help/docs/diff PASS. artifacts empty.
Normal Release contains the restored deferred-focus editor. Further latency and bridge-retirement work stopped by owner.
No commit/push/publication or owner data mutation. Earlier synchronous-focus candidates are superseded by this rollback.
