## v67.0.4 - Flexible List Layout and Ordering

Owner-requested bounded UI correction after accepted v67 closure. Sessions grow up to 625 px (formerly 125 px).
The specimen grid is 292 px with 24 px rows and a 32 px header, leaving room for 10 full rows including scrollbars.
An outer page scroller preserves access to the 360 px reading-tab viewport on shorter windows; grids stay height-bounded.
Specimen labels use natural numeric order initially and when toggling the Specimen header. Long numeric labels cannot overflow.
Sorting reuses the existing cell/row/collection-view commit sequence before applying the view comparer. No record is renamed.
Canonical shared click/keyboard handlers, deferred save and calculated-cell refresh remain. No input field or AutomationId added.
Full Verification adds a pure ascending/descending numeric regression; no pixel-only tester assertions or wider scenario authority.
Help explains list capacities, scrolling and header sort. Owner accepted the corrected UI and requested closure on 2026-09-12.

Owner review (actual application, not the Help window):
1. Where: Navigate > Flexible Material Testing. Action: select a session containing 10 specimens.
   Expected: Session list grows to its limit; page scrolling reveals a specimen list with 10 full rows.
   Do not click Delete Test Session, Delete Specimen or Delete Selected Reading.
2. Where: specimen grid. Action: inspect initial order, then click Specimen twice.
   Expected: 1, 2, ... 9, 10 ascending; header toggles numeric ascending/descending while preserving the selected specimen.
3. Where: an intended specimen edit in that grid. Action: click once, type, then use Tab, Shift+Tab and arrow keys.
   Expected: established editing/focus behavior; sorting afterward does not lose the saved edit or trigger a transaction error.
4. Where: same page. Action: scroll down to Compression/Recovery/Shore Hardness.
   Expected: reading tabs remain reachable and show the selected specimen's readings.

Final verification: Debug/Release zero warnings/errors. Disposable smoke and Full Verification 463/463 PASS.
Profile `20260912142615-fe3b7951`; exact database recovery SHA-256:
`FF381B6CFE6FA1B2757884730CBD4589CEF6524C06E68E67FF4F333298894744`.
Business-state recovery: `E8BCF04B15DCFC77D4E5228FC96DB76A257887F40C67EA3BB08170F05AE2CC88`.
Help 802/802 and documentation/diff checks PASS. Normal Release directory contains the owner-accepted v67.0.4.
Offscreen WPF fixture used the actual Flexible tab XAML with handlers detached and the virtualization-only style omitted.
Rendered 30 synthetic sessions and 20 specimens: Session viewport 625 px; initial specimen viewport 420 px with 15 complete visible rows.
The fixture checks layout, not interactive editing. Owner accepted the final UI; no separate key-by-key result was reported.
Evidence/fixture retained in `.private/v67-list-layout`. Owner data and the live website remain unchanged by this UI increment.

Owner refinement: use 10 specimen rows and remove the gap above reading tabs. Auto-sized rows and top alignment prevent centering.

Refinement verification: specimen height 292 px; gap from reading buttons to tabs is 6 px at 950 and 1500 px window heights.
Debug and Release pass with zero warnings/errors. Help and documentation audits pass; owner accepted the corrected layout on 2026-09-12.
No behavior/tester contract changed in this refinement, so the previous 463/463 result is retained without another full run.
