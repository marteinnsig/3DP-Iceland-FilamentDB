# v68.0.1 — Direct Izod and Charpy Entry

Status: owner runtime accepted on 2026-09-21: layout looks good, entry is fast and input works normally.
Canonical/runtime-accepted release is v68.0.1. This replaces the unaccepted v68.0.0 entry workflow.
The owner no longer requests ZIP handoffs; subsequent candidates use the continuing local runnable application.

## Changed workflow

Izod Measurements and Charpy Measurements each show one automatic row for every visible material.
Each row has ten direct instrument readings in kJ/m², Test Notes and Measured date.
Mean, sample Std Dev, CV %, Samples and Confidence are read-only. No run creation or energy conversion is required.
CV % is 100 times sample SD divided by mean; blank or NB entries are not numeric zeros.
Confidence follows the existing sample-count indicator, capped at 10; it is not a statistical confidence interval.
SD needs at least two numeric samples; CV also needs a nonzero mean. A genuine instrument zero remains a numeric sample.

Settings Manager > Izod / Charpy holds Specimen length 80 mm, Specimen width 10 mm,
Specimen thickness 4 mm and Notch depth 2 mm: remaining section 4 x 8 mm.
Defaults are captured when a material/method row is first saved; later settings edits do not recalculate readings.
Saved legacy energy-based records retain their units, metadata and report/recovery interpretation.
The old unaccepted run selectors, run creation and metadata editors have been removed from these tabs.

## Acceptance and Help assessment

The shared Fast Materials/Tensile renderer owns cell activation, focus, commit and keyboard navigation.
Detached changes validate and save to SQLite before canonical values change. Post-edit cell refresh preserves source identity.
Both tabs share the existing visible-material filters. Numeric samples persist exactly as entered without section conversion.
Help and the coverage ledger must describe the replacement grids and Settings fields in this same increment.

The existing smoke and authorized disposable CRUD scenarios own the new runtime contracts.
CRUD enters instrument values through the actual Fast callback, checks invalid-input rollback, edits/clears values,
restarts, checks independent methods/statistics and removes only its fixture with exact unrelated-state recovery.
The runner targets IzodImpactMeasurementsGrid and CharpyImpactMeasurementsGrid.
Debug/Release, 472/472 Full Data Verification, Help/documentation/security gates and disposable CRUD passed.
Exact final-byte evidence: Reports/V68_DIRECT_CANDIDATE_EVIDENCE.md.
No seed refresh is required solely for an additive migration with retained legacy coverage.
Owner accepted visual layout, speed and normal input on 2026-09-21. The checklist below remains historical test guidance.

## Read-only owner inspection

These steps operate the actual application unless explicitly labeled Help window.
Do not enter the illustrative numbers below into permanent owner measurements.

1. **Where:** Navigate > Measurements & Testing > Izod Measurements.
   **Action:** open the tab and inspect a known material row.
   **Expected result:** the row already exists; ten Sample columns, Test Notes, Measured date and summary columns appear.
   There are no New run, run selector or specimen-metadata controls.
2. **Where:** Navigate > Measurements & Testing > Charpy Measurements.
   **Action:** open the tab and locate the same material.
   **Expected result:** its own independent ten-reading row appears automatically, including when no readings exist.
3. **Where:** Navigate > Configuration > Settings Manager; rows in section Izod / Charpy.
   **Action:** read Specimen length, Specimen width, Specimen thickness and Notch depth.
   **Expected result:** initial defaults are 80, 10, 4 and 2 mm respectively; geometry is absent from measurement entry.
   **Do not click:** reset/recalculate actions; do not change settings during this read-only inspection.
4. **Where:** either measurement tab > Help > Help for Current View (F1), which opens the separate Help window.
   **Action:** read Izod and Charpy Measurements inside that Help window, then close the Help window.
   **Expected result:** direct kJ/m² entry, ten slots, blanks, save timing, summary units and Settings location are clear.

## Owner input test — intended readings or disposable test profile

Use actual intended readings on the owner profile. Use the disposable candidate profile for the example numbers and clearing.
The illustrative statistical check is optional when testing real readings; do not overwrite an existing measured row.

1. **Where:** actual application > Navigate > Measurements & Testing > Izod Measurements,
   intended material row > Sample 1.
   **Action:** click the cell once and immediately type the first reading, then press Tab.
   **Expected result:** the first character appears immediately; the value is saved and Sample 2 accepts typing immediately.
   No second click or pause should be required. Report any delay rather than accepting it as normal.
2. **Where:** the same row > Sample 2.
   **Action:** enter the next reading; use Tab, Shift+Tab and the arrow keys across sample cells and between material rows.
   **Expected result:** predictable movement, correct material ownership and immediate typing; identity/summary cells are skipped.
   No later refresh steals focus from the destination editor.
3. **Where:** disposable profile only, same material row and first two sample cells.
   **Action:** enter 10 and 20, leaving the remaining cells blank; commit with Tab.
   **Expected result:** Mean 15, Std Dev about 7.071, CV % about 47.140, Samples 2 and Confidence 2.
   Blank cells remain blank. Enter NB in Sample 3 only when testing the optional no-break entry.
   **Expected result:** NB persists without changing the two-sample mean/count.
4. **Where:** disposable profile only > same row > Sample 2.
   **Action:** clear its text and press Tab; then enter -1 and try Tab.
   **Expected result:** clearing reduces Samples to 1 and blanks SD/CV; negative input is rejected without overwriting saved data.
   **Action:** press Escape to cancel the rejected input.
   **Expected result:** the last saved value remains, and normal navigation works again.
5. **Where:** Navigate > Measurements & Testing > Charpy Measurements, same material row.
   **Action:** enter this method's intended readings, commit, return to Izod Measurements.
   **Expected result:** each method keeps its own readings and summary; existing Impact Measurements retains its prior data.
6. **Where:** the measurement row > Test Notes and Measured date, then the application window close button.
   **Action:** enter intended notes/date, commit with Tab, close and reopen the candidate from the same profile.
   **Expected result:** exact samples, blanks, notes and date survive restart under the correct material and method.
7. **Where:** Navigate > Materials & Setup > Materials and its existing material filters, then both measurement tabs.
   **Action:** use an ordinary visibility filter, inspect both tabs, then restore the original filter.
   **Expected result:** each tab follows the same visible-material selection; returning rows retain their saved readings.
   **Do not click:** Delete, Restore, Production, application publication or FTPS controls during this checklist.

Return the result for single-click typing, Tab/Shift+Tab, arrows, save/restart and layout in both tabs.
Owner acceptance is pending; this document does not authorize Production promotion or publication.
