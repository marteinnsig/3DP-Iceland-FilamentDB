## v67.0.7 - Flexible Session Ordering

Owner reported Session header sorting 1, 10, 2. Session startup and Session header toggling now use numeric label order.
Reuse the accepted specimen comparer, extended for session records, with ascending/descending adapter regression coverage.
Commit active Flexible edits before changing the session view. No saved labels, identities, dates or measurements are rewritten.
Canonical click/keyboard/deferred-save behavior remains; no new editor, AutomationId, schema or tester mutation authority.
Help covers startup/header sorting. Owner runtime ordering acceptance pending; v67.0.6 visual closure remains pending.
Owner runtime, actual application: open Flexible Material Testing. Inspect the Session list: 1, 2, ... 9, 10.
Click Session twice: descending then ascending numeric order; selected session and readings remain associated.
If making an intended edit, test one-click typing and Tab/Shift+Tab/arrows, then sort; values must save without a transaction error.
Do not click Delete Test Session, Delete Specimen or Delete Selected Reading. No Help-window action is needed for this check.
Final candidate: Debug/Release zero warnings/errors, Help 801/801, documentation/diff checks PASS.
Disposable smoke 20260912165525-62152996: Full Verification 465/465 PASS, including numeric session/specimen adapters.
Exact baseline/final database SHA-256: 15889CF1756DAA5E08E55C28AB3166D80C9AA23B302C4C4208E7CE656C2ECB73.
Exact baseline/final business hash: 3E50F76D28278A24D9AF6DB0C72919074422D6336E4848FC1211928946A2E263.
Evidence retained in .private/v67-session-ordering. Release directory updated; owner sorting/visual acceptance pending.

Owner accepted final session ordering on 2026-09-12; screenshot and final review confirm retired controls absent.
v67.0.5-v67.0.7 complete.
