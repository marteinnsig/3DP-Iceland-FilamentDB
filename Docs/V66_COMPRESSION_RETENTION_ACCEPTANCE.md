# v66.0.0 Compression Force Retention — owner acceptance

Candidate only. Canonical release remains v65.0.0 until owner acceptance and release closure.

## Implemented boundary

Compression displays read-only Force retention % from actual timed holds within the same specimen, Cycle, target strain and measured
Displacement mm. The earliest unique factual time is the reference; later force / reference force x 100 is shown. The reference stays
blank. Unreached/missing values, ambiguous earliest times and zero reference force produce blank results. Use a separate Cycle for
separate loading, and keep physical displacement fixed during each hold series. Raw values and schema are unchanged.

Add Relaxation Point and its handler are removed. Saved Stress Relaxation appears only for specimens with historical rows, retaining
calculation for supported historical records; its SQLite, model, load/save, export/recovery and Materials-membership callers remain
supported. Historical data is not automatically copied into Compression, which would invent displacement or duplicate results.

The accepted Tensile/shared input handlers and Flexible immediate editor path are retained. Dependent retention property notification
updates other rows after commit without replacing ItemsSource or moving focus. Full Verification extends the existing calculation
contract and checks the read-only column. No new tester scenario or expanded authorization is needed.

## Read-only owner inspection

1. Where: launch App/FilamentDbApp/bin/Release/net9.0-windows/3DPIcelandFilamentDB.exe from the repository.
   Action: open the top-level Flexible Material Testing tab, select the relevant session in the top table and specimen below it,
   then open Compression. Expected: Force retention % is visible; Add Relaxation Point is absent.
   Do not click: Delete Test Session, Delete Specimen or Delete Selected Reading.
2. Where: the same Compression table, on a specimen with factual 10 s and 30 s measurements at the same target/displacement/Cycle.
   Action: compare displayed retention with 30 s force divided by 10 s force times 100. Expected: the 10 s reference is blank and the
   30 s percentage agrees. Example only: 100 N then 85 N gives 85%; do not enter these invented values into owner measurements.
3. Where: Flexible Material Testing, select a specimen with no historical relaxation readings.
   Action: inspect the tab row. Expected: no Stress Relaxation tab. Selecting a specimen with historical readings shows
   Saved Stress Relaxation; open it to inspect previous values.
   If there are no historical rows, report saved-row inspection as not applicable. Do not click any Delete action.
4. Where: Help menu > Documentation, in the separate Help window.
   Action: search for Force retention and open Flexible Material Testing. Expected: the reference rule, blank cases, fixed displacement,
   additional Compression points and retained historical tab are explained. This is Help inspection, not a measurement operation.

## Owner input acceptance — re-enter factual values only

5. Where: Flexible Material Testing > Compression, on the same existing measured specimen.
   Action: note an existing Force N value; click that cell once, select its text and re-enter that exact factual value. Press Tab,
   Shift+Tab and the arrow keys between editable cells. Expected: immediate typing works, calculated cells are skipped and focus
   remains in the destination editor. Do not replace measurements with example numbers or click any Delete action.
6. Where: the same table after committing the value, then after a normal app close/reopen.
   Action: inspect the force and retention. Expected: factual values remain saved and retention is recalculated consistently.
   If a real correction to the earliest force is needed, entering that factual correction must immediately update later retention
   after commit without navigating away. Do not fabricate a correction merely for this check.

Return whether these checks pass; this acceptance is required before commit/push and major closure.
## Comparable Results correction

7. Where: Flexible Material Testing > Comparable Results, with the measured session selected.
   Action: locate Force retention at 20% Strain (or your recorded target). Expected: unit %, actual time interval such as 10–30 s,
   displacement and cycle. Mean, sample SD, CV, min/max and independent specimen n summarize matching methods only.
   The mean is the average of each specimen's retention, not the ratio of pooled forces. Do not alter raw data for this inspection.
8. Where: Help > Documentation, in the separate Help window.
   Action: search Force retention and open Flexible Material Testing. Expected: conditional saved-tab visibility and comparison
   grouping are described. Do not operate measurement or Delete controls for this Help-only step.
## v66.0.1 Recovery TVL entry

TVL must retain its original zero at initial specimen contact. Positive readings mean downward travel (height loss).
The saved recovered height remains canonical; old rows display an equivalent derived TVL offset without claiming original TVL use.

1. Where: the Release app > Flexible Material Testing. Select the session and specimen from your actual recovery measurement.
   Action: open Recovery and select the existing recovery row (or Add Recovery if it has not been recorded).
   Expected: Compression % defaults to 20 for new rows; Initial height mm matches the factual pre-test height. Correct it first if needed.
   Do not click: Delete Test Session, Delete Specimen or Delete Selected Reading.
2. Where: that Recovery row > TVL contact offset mm.
   Action: enter your actual 0,14 reading and use Tab to commit. Set Rest time s to the actual 60 s.
   Expected: Height after rest becomes initial height minus 0.14 mm and Residual height loss updates.
   For initial 9 mm: 8.86 mm and 1.56%; for initial 10 mm: 9.86 mm and 1.40%. Use your actual initial height.
3. Where: the same row.
   Action: re-enter the same factual TVL value using one click, then Tab/Shift+Tab and arrow keys.
   Expected: typing starts immediately, the destination editor retains focus, and values remain consistent without navigating away.
   Direct Height after rest is an alternative entry; using it updates the equivalent TVL field.
4. Where: Recovery after a normal close and reopen of the app.
   Action: inspect that same row. Expected: saved heights/rest time and the equivalent 0.14 mm TVL value persist.
5. Where: Help > Documentation, in the separate Help window.
   Action: search TVL contact offset and open Flexible Material Testing.
   Expected: retained zero, sign, direct-height alternative, historical derived offsets, validation and save timing are explained.
   Do not operate any Delete or other measurement controls for this Help-only check.

Invalid-text/too-large-offset rejection and historical roundtrip are deterministic Verification checks; do not insert fictitious test
measurements into owner data. Negative offsets mean height above the initial contact position. Blank clears the recovered height.
## v66.0.2 Recovery compressed-hold setting

1. Where: Settings Manager, in the settings table under Flexible Material Testing.
   Action: find Default recovery compressed hold. Expected: initial value 30, unit s. Enter your actual preferred hold time if different
   and click Save Settings. Do not click Restore Built-in Defaults or Reload Saved Settings for this check.
2. Where: Flexible Material Testing > Recovery, with the intended session/specimen selected.
   Action: Add Recovery for a real measurement that needs recording. Expected: Compressed hold s copies the setting; Rest time s
   remains 60 and Compression % remains 20. Fill factual measurement values. Do not click Delete actions or create invented results.
3. Where: Recovery rows already recorded before changing the setting.
   Action: inspect them. Expected: their original Compressed hold s values remain unchanged.
4. Where: Settings Manager after normal app close/reopen.
   Action: inspect Default recovery compressed hold. Expected: your saved setting persists.
5. Where: Help > Documentation, in the separate Help window.
   Action: search Default recovery compressed hold. Expected: setting location, seconds, 30 s initial value and new-row-only effect.
## v66.0.3 Canonical Flexible results and Material Detail

Read-only review (no changes to measurement values required):

1. Where: launch the Release application at App/FilamentDbApp/bin/Release/net9.0-windows/3DPIcelandFilamentDB.exe.
   Action: confirm the header shows v66.0.3. Expected: Canonical Flexible Evidence and Material Details candidate.
2. Where: Flexible Material Testing > test session table.
   Action: select an active session with your existing measurements; open Comparable Results below the specimen table.
   Expected: Compression, retention and any complete Recovery results appear with their conditions, mean and independent n.
   Do not click: Delete Test Session, Delete Specimen or Delete Selected Reading.
3. Where: Materials > select that same MaterialID > Material Detail > Mechanical > Engineering Dashboard > Flexible Material Testing.
   Action: inspect the result table; expand Test setup once to inspect the method. Expected: no repeated method prose.
   Expected: the same calculations appear. If several active sessions share the exact method/conditions, their independent
   specimens are combined here; Comparable Results remains scoped to one selected session. Inactive sessions are excluded.
   For the existing 0.14 mm height loss, expect 1.556% at initial height 9 mm, or 1.4% at 10 mm. Use the factual initial height.
   A single specimen shows n=1 and no sample SD/CV (an em dash), not fabricated zero variability. Long labels wrap legibly.
4. Where: Materials > select a different material without Flexible readings > Material Detail > Mechanical.
   Action: inspect Flexible Material Testing. Expected: no comparable results message; the prior material's results do not remain.
5. Where: Help > Documentation, in the separate Help window.
   Action: search Flexible Material Testing and Material Detail General.
   Expected: Help explains active sessions, selected-session versus material scope, independent n, Recovery and unchanged Overall.
   Do not operate measurement/Delete/Restore/Production controls for this Help-only step.
6. Where: the same material and Mechanical Flexible section after normal application close/reopen.
   Action: inspect saved results. Expected: identical results and conditions, with no repeat measurement required.

Optional factual edit review, only when an actual recorded measurement needs correction:

7. Where: Flexible Material Testing > correct session/specimen > Compression or Recovery.
   Action: correct that factual value, then Tab to commit. Expected: normal single-click typing and keyboard focus remain intact.
   Return to Material Detail > Mechanical. Expected: the successfully saved correction is reflected without restarting the application.
   Do not create fictitious measurements or click Delete/Restore for this check.

Report/PDF, rankings/categories/awards, research/AI and comparison/chart integration are later recorded increments v66.0.4-.8.
Their absence in this first candidate is tracked work, not release completion. Also review the pending v66.0.2 setting steps above.
General should now show only a compact Flexible summary and the Mechanical location. In Mechanical, test switching between
materials and expanding Test setup; table values must stay with the correct material. Do not click Delete, Restore or Production.

Wide-window review: Material Detail > Mechanical > Flexible Material Testing. Enlarge the window; columns should stay
compact and left-aligned, with long conditions wrapping. Reduce window width to check the horizontal scrollbar. No data edits needed.

## v66.0.3.1 Material Detail handoff review

1. Where: Materials. Action: select the material with your saved Flexible measurements, then open Material Detail > Compare.
   Action: click Use Selected for A, then choose another material in B.
   Expected: Flexible comparison aligns only identical conditions; unmatched rows have a dagger and missing cells show an em dash.
   Expand Test setup for the full method. Numeric zero stays zero; selecting the same material twice does not create a peer.
2. Where: Material Detail > Video Planner > Flexible video brief (above the existing filters).
   Action: expand if collapsed. Expected: header identifies the material selected in Materials; topics reflect its measured tests,
   with saved result tables and comparison candidates. Scroll inside the brief for more results. Test setup is expandable.
   Do not click Clear Ideas, Save Idea or other planning actions; this review needs no saved idea changes.
3. Where: Material Detail > Recommendations > Flexible guidance (above selected-material intelligence).
   Action: expand if collapsed. Expected: measured facts, use-specific guidance and matching candidates for the same material.
   The existing global winner lists and Overall are separate. Do not click Send to Video Planner for this read-only check.
4. Where: Materials. Action: select an unrelated material with no Flexible readings; return to Video Planner and Recommendations.
   Expected: the header follows the new material and shows no comparable results, without retaining the previous material's data.
5. Where: Help > Documentation (separate Help window). Action: search Material Detail Compare, Video Planner and Recommendations.
   Expected: each topic explains the new Flexible panel, selected-material scope and saved-only evidence.

No Delete, Restore, Production, FTPS, external AI send or fictitious measurements are needed for this review.
## v66.0.4 Reports and PDF review

1. Where: Materials. Action: select an existing material with saved Flexible results.
   Expected: Material Detail > Mechanical shows the values to compare with the export. Do not edit or delete readings.
2. Where: Reports / PDF Export. Action: set Report template to Material Engineering Report and Report scope to
   Selected Material Only; click Refresh Preview. Expected: Flexible Material Testing lists that material's conditions and results.
3. Where: Reports / PDF Export. Action: Choose Folder for a local review folder, then click Export Current Report and Open Folder.
   Expected: exported HTML, text and PDF include the same mean, n, SD, CV and range; long tables and final rows remain readable.
   Do not click: any public build, Production, FTPS, Restore or Delete action for this local review.
4. Where: Materials, then Reports / PDF Export. Action: select a material without Flexible readings and refresh the preview.
   Expected: no Flexible values from the previous material remain. No fictitious results or zero statistics appear.
5. Where: Reports / PDF Export. Action: repeat local preview/export for another relevant engineering template.
   Expected: Flexible evidence belongs only to materials included in that report; existing Overall values remain unchanged.
6. Where: Help > Documentation, inside the separate Help window. Action: search Reports controls and fields and Flexible Material Testing.
   Expected: report scope, saved data, aggregate statistics, local generation and public-safe boundaries are explained.

Public templates are approved, but live publication is not part of this increment. Owner HTML/PDF acceptance remains required.

## v66.0.5 Flexible rankings, categories and awards

1. Where: Materials. Action: clear restrictive search/filters so your measured Flexible materials are visible.
   Expected: at least two materials are available for comparison. Do not add invented measurements or use Delete.
2. Where: Navigate > Rankings Dashboard > Flexible Material Testing.
   Action: choose Force retention in Flexible category. Expected: results separated by method and conditions, with mean, n and rank.
   A rank appears only with two matching measured materials. Unmatched values remain visible without a rank. Overall stays unchanged.
3. Where: the same tab. Action: select one Base Material or Manufacturer, then Reset Filters.
   Expected: Flexible rows follow the scope; reset restores All Flexible categories. Existing score Metric controls its separate grid.
4. Where: Navigate > Category Rankings > Flexible Material Testing.
   Action: choose Recovery residual height loss; try Winners by base material or Winners by manufacturer in View.
   Expected: lower residual loss ranks first only within matching conditions and each group; equal values share a rank.
5. Where: Navigate > Awards & Winners > Flexible Material Testing.
   Action: inspect the applicable retention/reduction/recovery category. Expected: two measured peers are required; tied winners remain.
   Force, apparent stress and Shore do not receive a best-material award.
6. Where: any of these Flexible sections. Action: Export Flexible CSV to a local review file and open it.
   Expected: the same scoped rows, conditions, native units, rank and specimen statistics; missing values remain empty.
   Use Export Flexible CSV for this check, not the adjacent Export CSV for the existing score list. No publishing actions are needed.
7. Where: Materials. Action: filter to an unrelated material without Flexible measurements, then revisit the three ranking tabs.
   Expected: no previous material's Flexible results remain. Clear the filter when finished.
8. Where: Help > Documentation, in the separate Help window. Action: search Rankings Dashboard, Category Rankings and Awards.
   Expected: Flexible selectors, matching conditions, ties, scope and CSV behavior are documented.

## v66.0.6 Research, planning and AI

Read-only review:

1. Where: reopen the Release app; Navigate > YouTube Research.
   Action: expand Flexible opportunities — filtered materials and choose a measured topic.
   Expected: correct material, method ID, condition, mean/n/statistics and only exact-condition comparison peers. Choices show only material and topic on one line; condition and method remain in the result panel.
2. Where: Materials > Material Detail > Video Planner, then Recommendations.
   Action: expand Flexible opportunities — filtered materials (the dataset section, separate from the selected-material brief).
   Expected: choices follow the named category/base/manufacturer/no-video filters. Flexible-only materials do not need Overall.
3. Where: any new opportunity section. Action: Copy Flexible brief and paste into a local text editor for inspection.
   Expected: selected facts/peers/method are included. This is local copying; do not paste/send externally for this check.
4. Where: Navigate > Dashboard Insights. Action: inspect the first summary line. Expected: Flexible coverage follows Materials filters.
5. Where: AI Assistant. Action: use the existing local Generate From Template or material-intelligence generation action.
   Expected: Flexible topics appear within processed scope with any omitted-topic count. Existing saved sessions remain unchanged.
   Optional read-only: Preview OpenAI Payload includes safe Flexible aggregates. Do not click Generate with OpenAI for this check.
6. Where: Help > Documentation, separate Help window. Action: search YouTube Research, Video Planner, Recommendations and AI Assistant.
   Expected: dataset scope, local copy, explicit save and the external consent boundary are documented.

Optional actual saved-idea acceptance (changes planning records only):

7. Where: Material Detail > Video Planner > Video ideas from recommendations. Action: note the existing saved ideas.
   In Materials, filter so the previously selected material disappears, then choose a visible material with measured Flexible data.
   Return to the Flexible opportunities section, select a useful factual topic and click Save Flexible idea once.
   Expected: the new snapshot appears in Video ideas from recommendations and every previous idea remains. Do not click Clear ideas.
8. Where: the saved-idea grid. If a factual Notes edit is needed, click once, type and use Tab/Shift+Tab/arrows normally.
   Action: save another useful Flexible idea. Expected: the prior edit commits, focus behaves normally and existing ideas remain intact.
9. Where: the saved-idea grid after normal app close/reopen. Action: inspect the new idea and previous ideas.
   Expected: saved facts persist; later source changes do not silently rewrite those snapshots.

No measurement creation/deletion, Restore, Production, FTPS or external AI request is part of this review.

### v66.0.6 owner dropdown correction

Where: YouTube Research, then Material Detail > Video Planner and Recommendations > Flexible opportunities — filtered materials.
Action: choose a long Recovery entry, open the dropdown and scroll; use keyboard arrows and Enter to select another entry.
Expected: material/topic stays on one line; long names use ellipsis and tooltip. The open list scrolls; matching facts update.
This is read-only. Do not click Save Flexible idea, Clear ideas or external generation for this layout check.

Owner accepted the single-line dropdown layout on 2026-09-12 with screenshot evidence.
This confirms layout only; the remaining v66.0.6 research/intelligence checklist is still pending.

### v66.0.7 owner review — read-only

1. Where: Materials. Action: filter to materials with saved Flexible results, then open Material Details > Analytics.
   Expected: Flexible Material Testing shows those materials even if they have no Overall score; units and statistics match saved results.
2. Where: Material Details > Analytics > Chart Mode. Action: choose Average by manufacturer, then Average by material type.
   Expected: Flexible rows group under the chosen headings; each material keeps its own n/mean/SD/CV/range, without pooled averages.
3. Where: the Flexible Material Testing section on Analytics. Action: inspect a long Recovery condition, expand Test setup, scroll.
   Expected: labels readable; every chart has its own native-unit scale. Measured zero is 0; missing/unreached readings have no bar.
4. Where: Material Details > Compare. Action: select two materials with matching tests in A and B.
   Expected: all selected materials share one table despite differing setup notes or n; mean/SD/CV/range stay unchanged.
   Different measurement conditions remain separate rows. No Test setup 1/2 sections appear.
5. Where: Help window. Action: search Material Detail — Analytics reference and Material Detail — Compare reference.
   Expected: Help describes the grouping and statistics just inspected.

Do not click Delete, Restore, Recalculate, Production, FTPS or external AI actions. This review changes no measured data.

Owner accepted the unified Compare table on 2026-09-12 and requested the next step. Analytics review remains separately pending.


## v66.0.8 final research review — read-only

1. Where: Materials. Action: filter to materials with saved Flexible tests, then open Material Details > Video Planner.
   Expected: main candidate list includes Flexible ready rows even with Mechanical data only enabled; Overall remains blank.
2. Where: Navigate > YouTube Research. Action: generate research, inspect titles/thumbnails, Comparison Discovery, Channel Gap,
   Content Calendar and Playlist Discovery. Expected: measured Flexible topics appear; comparisons include only matching pairs.
   Priorities describe editorial planning, never material quality. Calendar reserves every third slot when Flexible is available.
3. Where: YouTube Research > Copy Best Thumbnail. Action: copy and paste into a local text editor.
   Expected: material matches the displayed top candidate; Flexible facts include result, unit, n and condition.
   Do not send this externally or click publishing/Production/FTPS controls.

Remaining earlier checks are listed under v66.0.2, v66.0.6 and Analytics in v66.0.7; do not repeat already accepted Compare layout.

2026-09-12 closure reconciliation: owner confirms Save Flexible idea survives restart and the other requested checks look correct.
This accepts v66.0.2 Recovery hold default, v66.0.6 save/restart workflow and v66.0.7 Analytics. New v66.0.8 review remains pending.


FINAL ACCEPTANCE 2026-09-12: owner accepts v66.0.8 research integration. All recorded v66 runtime/visual reviews are complete.
