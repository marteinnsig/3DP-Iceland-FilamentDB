# Materials autosave status and periodic retry

State: accepted 2026-10-11. Owner confirms save, restart, five-minute retry, focus and Help.
Debug/Release, Help/docs, disposable CRUD and Full Verification 482/482 PASS on 2026-10-05.
2026-10-11 refresh: solution Debug/Release zero warnings/errors; Help/documentation/diff gates PASS.
Full Data Verification 483/483 PASS in disposable CRUD profile 20261011180607-c8ed4367.
Evidence: Reports/MaterialAutosave/2026-10-11. CRUD autosave/restart/recovery PASS; exact baseline/final business state restored.
Normal Release contains the exact final tested candidate; see Reports/MATERIAL_AUTOSAVE_EVIDENCE.md.

## Meaning and scope

The old `Unsaved material edit pending` text indicates the in-memory Materials dirty flag,
not whether all raw measurements failed to save. Measurement commits can save raw values
while leaving derived Materials coverage/TestedStatus dirty. Statistics refresh also hid
previous save errors behind this generic message.

Keep the accepted 450 ms committed-edit save and close-time guards. Add a five-minute
Materials-only retry for dirty committed rows; skip an active edit debounce and restore shutdown.
The retry does not force an editor commit, rebind grids or refresh input filters. It never
bypasses validation. Retain module save errors across statistics refresh and unrelated saves;
clear measurement failures only after the shared SQLite transaction succeeds. Last saved
is the last reported operation, not a promise that active editor text has been committed.

## Automated acceptance

Existing authorized CRUD runs on a copy of `C:\Seed-Database\filamentdb.sqlite`.
Its MaterialID guard also owns the new regression: save temporary notes through the actual
timer callback, reject a missing Product Line, retain measurement and Materials errors across
statistics refresh, recover after correction and restore original notes. Existing restart
checks and exact business-state recovery remain. No new scenario authorization or schema.
The callback test verifies production save behavior without a five-minute sleep; real timer
scheduling and focus remain owner runtime checks. Full Verification runs in that disposable profile.

## Owner runtime handoff

1. **Where:** Main application → Materials tab. **Action:** Make an intended valid edit and
   press Tab. **Expected:** Save completes shortly after leaving the cell; a persistent failure
   names the affected material/field and its full text is available by hovering over the status.
   **Do not click:** Delete, Restore, Recalculate, Production or FTPS.
2. **Where:** Bottom status bar of the actual app, after a normal measurement edit has left
   `Materials changes pending`. **Action:** Leave the app open for five minutes with the UI idle.
   **Expected:** Pending committed Materials data saves, or `Materials not saved` gives a reason.
   If typing in another cell, the timer must not move focus or close that editor; commit that
   cell yourself before expecting its text to be saved. **Do not click:** destructive/live actions.
3. **Where:** Main app window. **Action:** Close normally and reopen after the valid edit.
   **Expected:** The edit remains. If `Auto-Save Blocked` appears, choose **No** to keep the app
   open and correct the problem; do not choose Yes to discard an unverified pending change.
4. **Where:** Help window → Start here → Start-to-finish workflow. **Action:** Read
   `Bottom status bar`. **Expected:** It explains committed edits, retry, errors and close behavior.
   This step is reading Help, not operating a measurement or recovery control.
