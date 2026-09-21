# v68.0.2 — Izod and Charpy Results Integration

Status: implemented; Debug/Release, 475/475 Verification, disposable CRUD and Help gates PASS.
Owner result/status/radar acceptance received with v68.0.3 on 2026-09-21. Evidence: Reports/V68_RESULTS_INTEGRATION_EVIDENCE.md.
The owner accepted v68.0.1 layout, speed and normal entry on 2026-09-21.
Future handoffs use the continuing local application directory; the owner does not need another ZIP.

## Scope

Materials gains derived, read-only In Izod and In Charpy fields. Tested Status requires six methods:
Tensile, Impact, Stiffness, Heat, Izod and Charpy. None is Not tested, one to five is Partially tested,
and all six is Fully tested. Numeric zero is a reading; blank, NB and an empty prepared row are not numeric results.
Flexible coverage remains independent. Existing saved raw measurements and energy history retain their meaning.

Material Detail shows selected-material Izod/Charpy summaries above every inner tab. Mechanical shows sample statistics;
Charts, Analytics and Compare expose separate method results and normalized comparison axes.
Each method normalizes against its own comparison cohort; the plotted score does not convert the raw kJ/m² reading.
Overall retains its existing formula pending an explicit owner decision about weighting the additional methods.

## Help and automated acceptance

Help must explain membership, six-method coverage, raw units, missing values, comparative normalization and save/refresh timing.
The shared summary and method cards have stable AutomationIds. Existing authorized disposable CRUD and smoke scenarios
verify independent flags, status transitions, saved/restarted evidence and available detail/radar axes.
This is a changed runtime contract, so tester and Full Data Verification were updated in this increment.
Visual clipping, legibility and retained immediate typing require owner acceptance; text/reflection checks cannot replace it.
No owner database fixture writes, Production promotion, FTPS publication or new destructive authority is included.

## Read-only owner inspection

1. **Where:** restart the updated application, then open Materials.
   **Action:** inspect In Izod, In Charpy and Tested Status on known measured materials without resetting the column layout.
   **Expected result:** In Izod and In Charpy immediately follow In Flexible; Fully tested requires all six methods.
   **Do not click:** Delete, Restore, Recalculate, Production or FTPS controls.
2. **Where:** select that material, then Material Detail > General, Printing Profile, Mechanical, Charts, Analytics,
   Compare, Video Planner, Recommendations and Notes.
   **Action:** switch between the inner tabs.
   **Expected result:** the selected MaterialID's summary remains visible.
   General > Test Information includes both flags and no Other group.
3. **Where:** Material Detail > Mechanical, then Charts, then Analytics.
   **Action:** compare method statistics to their measurement tabs and inspect the separate radar axes.
   **Expected result:** raw values agree; Izod and Charpy stay separate; normalization context is visible; no text is clipped.
4. **Where:** Material Detail > Compare.
   **Action:** select measured materials with the existing comparison selectors.
   **Expected result:** only Izod mean (kJ/m²) and Charpy mean (kJ/m²) appear for these methods; no SD, CV or Confidence rows.
5. **Where:** Help > Help for Current View (F1), in the separate Help window.
   **Action:** read Materials field help and the Izod/Charpy, Mechanical, Charts and Analytics topics.
   **Expected result:** six-method status, units, cohort comparison and unchanged Overall are clear.

## Input and refresh acceptance

Use intended real readings only on the owner profile; illustrative fixture changes belong in a disposable tester profile.
**Where:** Izod Measurements or Charpy Measurements in the actual application.
**Action:** enter an intended reading and commit with Tab, then inspect Materials and the selected Material Detail.
**Expected result:** saved values update coverage and evidence; a single click still accepts typing immediately and keyboard
movement remains normal. On a disposable profile, clearing the last numeric reading removes only that method's membership.
**Do not click:** Delete, Restore, Production or FTPS. Do not clear real owner readings merely to test a status transition.
