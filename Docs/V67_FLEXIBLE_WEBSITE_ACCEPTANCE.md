# v67 Flexible Website Explorer acceptance

## v67.0.1 - Horizontal Website Charts

### Flexible chart text-size correction

Owner found Flexible text too small. Labels, axis ticks and means now use the database chart size of18px.
Canvas measurement uses the same font; wider label space and24px line spacing preserve full-name wrapping.
Actual browser computed styles confirm18px for Flexible text and database x-labels; long names visually pass.
Debug/Release pass with zero warnings/errors. Cosmetic-only: no Help, AutomationId or tester contract changes are warranted.
Prior460/460 behavior verification remains applicable; no calculation/filter/tooltip content changes. Owner preview retry pending.


Owner accepted v67.0.0 after the TPU fix, then requested all main website bars horizontal.
Current candidate passes 460/460 in profile `20260912032400-e8357206`; Debug/Release and Help pass.
Repeat owner preview steps below, additionally inspecting Tensile/Impact paired bars and SD whiskers, Stiffness/Thermal,
Overall/Consistency, Flexible and Experimental score bars. Names should be beside bars with readable values.
Expected: filtering/grouping and popup facts are preserved. Existing horizontal, line, scatter and radar charts are unchanged.
Read Help → Website preview for orientation and interaction guidance. No new WPF input or AutomationId was added.
Website publication is now planned as v67.0.2; v67.0.1 visual acceptance remains pending.

## v67.0.0 - Flexible Website Explorer

Correction verification: Debug/Release zero warnings/errors; Help/docs/diff checks pass.
Disposable profile `20260912025445-2a1afd9d`: smoke PASS, Full Verification 459/459 PASS.
Exact database SHA-256: `26A4C891E789EF12F987549A0A205E1EC7109D11DED6E8C882078DAA71111F87`.
Exact business-state hash: `E8BCF04B15DCFC77D4E5228FC96DB76A257887F40C67EA3BB08170F05AE2CC88`.
Normal Release updated after owner closed the app. Owner Generate Preview retry remains pending.


### Owner preview correction — 2026-09-12

Owner Generate Preview failed because the public TPU family comparison artifacts were not generated.
Synthetic reproduction confirmed Path/membership/allowlist/exclusion/artifacts passed but content validation failed.
BuildScoreChart omits charts with no source scores; verification incorrectly required every chart heading anyway.
Validation now requires each chart exactly when its score exists. Null remains absent; measured zero still requires a chart.
The regression covers Flexible-only and partial-score comparisons and rejection of a damaged required chart.
Report rendering, raw measurements, public opt-in and publication guards are unchanged. Help was revalidated: existing public-report
family scope, missing evidence and local Generate Preview behavior already describe the corrected contract; no new control or Help text is needed.
The existing Full Verification smoke gains the regression; no new scenario, seed mutation or broader automation permission.
Initial synthetic reproduction failed before the fix and passed after it. Owner retry remains required for actual preview acceptance.


Candidate implemented on 2026-09-12. Owner preview acceptance remains pending; v67.0.1 owns later website publication.
The accepted application baseline remains v66.0.8. The normal Release directory now contains the v67.0.0 candidate.

### Implementation and automated acceptance

- Flexible Testing follows Pricing & Value. Its filters mirror the canonical database controls; filament selection is explicit.
- Charts align by metric, condition and unit. Each saved summary keeps its own Mean, n, Sample SD and CV%; there is no pooling.
- The website projection excludes compression force N, Min/Max, method/specimen notes and raw measurements.
- Null stays missing; zero stays measured. CV is already a percentage and is never multiplied by 100 again.
- Overall, normalized radar and existing statistical aggregators are unchanged. No editable WPF surface was introduced.
- Full Verification adds the public-projection contract and extends existing portal page/hash checks.
- Existing disposable smoke owns this verification. No new scenario, AutomationId, seed change or broader authorization is needed.
- Help `website.preview`, its registry and inventory are synchronized. Browser visual acceptance is separate from static checks.
- Read-only subagent review found a hidden-selection bug after manufacturer changes; obsolete selected IDs are now cleared.
- Long label height is computed from wrapped lines; single bars have a bounded width.

Debug and Release builds pass with zero warnings/errors. Help coverage passes 801/801.
Disposable profile `20260912023829-85994faa` passes smoke and Full Verification **458/458**.
Baseline/final database SHA-256: `5254D37AB4842AB91012677739058C4253EEFCE2D407D03CEFF67DD0E27C56B1`.
Baseline/final business-state hash: `E8BCF04B15DCFC77D4E5228FC96DB76A257887F40C67EA3BB08170F05AE2CC88`.
Profile evidence contains run-result and verification TXT/JSON, screenshots and before/after database copies.

### Browser evidence

The local synthetic fixture uses the canonical SQLite template, actual FlexibleWebsiteService and native portal styles/script.
It contains three synthetic materials, differing n, different source metadata, a zero Mean and a force result to exclude.
Retained locally under `.private/v67-preview`; no owner database was used or changed by browser testing.
In-app browser checks passed: direct Flexible route, adjacent tab, two charts/six bars, individual selection, canonical filter
sync in both directions, changing manufacturer after selection, empty results, Reset filters, Enter/Escape tooltip behavior,
CV 0.481%, n 9 versus 10, zero Mean, long labels, and responsive two-column filters at 600px. No JavaScript errors were observed.
Screenshots were visually inspected during the run; the retained fixture and browser observations support reproduction.
Actual owner-data export and hover/readability acceptance remain manual checks below.

### Owner preview steps

These steps operate the actual app and generated local website, not the Help window.

1. **Where:** normal application. **Action:** restart into v67.0.0, then open **Navigate → Website Export**.
   **Expected result:** the Website Export tab is visible. **Do not click:** Generate Production or Publish Website Production.
2. **Where:** Website Export. **Action:** click **Generate Preview** and wait for the successful export message.
   **Expected result:** local `index-test.html` is generated; existing opted-in public report files may be refreshed locally.
3. **Where:** Website Export. **Action:** click **Open Folder**, then open `index-test.html` in your browser.
   **Expected result:** **Flexible Testing** appears beside **Pricing & Value**. Open Flexible Testing.
4. **Where:** Flexible Testing. **Action:** choose a manufacturer/base material and one or more **Filaments**.
   **Expected result:** only matching measured filaments appear. **Reset filters** restores all matching results.
5. **Where:** a Flexible chart. **Action:** hover over a bar, then click it to keep the popup open.
   **Expected result:** Mean matches the app; popup shows Sample SD, CV%, n and the recorded condition.
   No compression force N chart or Min/Max statistics should appear. Compare both 10s and 30s stress charts and recovery.
6. **Where:** browser. **Action:** resize the window and inspect the longest material labels; switch to Filament Database and back.
   **Expected result:** readable labels, synchronized filters and working tabs. Confirm that this local preview looks correct.

### Remaining release work

Owner preview and Help readability acceptance are required before v67.0.0 closure under repository acceptance rules.
v67.0.1 then prepares/reviews the actual website package and performs governed backup, publication and HTTPS verification.
No live website, Production files, FTPS credentials or application distribution packages were changed by this increment.

### v67.0.2 whitepaper update

Chapter 8 now documents timed Recovery from the retained TVL contact zero, including the illustrative 0.14 mm offset calculation.
It records the owner's planned Shore A/D procedure: separate scales, printed 30 x 30 x 8 mm coupons and five locations per specimen.
Within-specimen location means remain separate from independent-specimen n, Sample SD and CV%; no results or dwell time are invented.
The test matrix and Help now cover Recovery and planned Shore. PDF formula backgrounds and following paragraph spacing are corrected.
Debug/Release pass with zero warnings/errors. Canonical PDF has 37 pages; final pages were rendered and visually reviewed.
Existing disposable smoke owns a new methodology-content Verification gate; no new scenario, input field or AutomationId is needed.
Owner accepted v67.0.1 horizontal bars and font correction. Whitepaper readability remains pending; publication belongs to v67.0.3.

Review the generated PDF, chapter 8 on pages 19-22, especially Recovery and the planned Shore procedure.
In the actual application, use Help > Export Engineering Whitepaper... to generate the same canonical document.
Expected: separate Shore A/D, five locations per specimen and planned status; no claimed collected Shore results.
This is a local document export. No Production or FTPS action is part of this review.

### Shore method clarification and location statistics

Owner clarified the established procedure: 50 x 50 x 8 mm coupon, 24 hours rest, four locations 10 mm from both corner edges
plus the centre, and a 10-second reading at each of the five positions. Testing additional materials is ongoing.
The corrected whitepaper supersedes the initial planned 30 mm description within this unaccepted increment.
Saved Shore rows now also produce per-specimen valid reading count, mean, sample SD and CV. A/D and dwell/thickness remain separate.
Group statistics still use equally weighted specimen means and independent n; point counts never substitute for specimen counts.
Location summaries appear in Comparable Results, the Mechanical dashboard, website bar popups and private/public reports.
Six public templates share the sanitized projection; public summaries use group-local specimen numbers, without raw IDs or notes.
No schema migration, owner-data rewrite, input handler change, default override, AI payload expansion or Overall change.
Existing smoke gains the pure Shore regression; Help owns the new read-only column and procedure. Coverage is 802/802.
Debug and Release pass with zero warnings/errors. Synthetic arithmetic/public-publisher checks and actual browser popup/report pass.
Owner readability and real-data preview acceptance remain pending; no live website publication.

Owner review (actual app, not Help):
1. Where: Navigate > Flexible Material Testing > select the session and Shore specimen > Shore Hardness.
   Action: inspect the five saved readings with the same scale, 10-second time and 8 mm thickness. Expected: five separate rows.
   Do not click Delete Selected Reading or Delete Specimen.
2. Where: the same tab > Comparable Results. Action: inspect Shore location statistics (scroll right if necessary).
   Expected: each specimen has reading count, mean, location SD and CV; n remains the independent specimen count.
3. Where: Material Details > Mechanical > Engineering Dashboard > Flexible Material Testing.
   Action: inspect Location statistics rows. Expected: the same per-specimen values.
4. Where: Navigate > Website Export. Action: Generate Preview, then Open Folder and open index-test.html > Flexible Testing.
   Expected: clicking a Shore bar shows within-specimen location statistics matching the app.
   Do not click Generate Production or Publish Website Production.
5. Where: Help > Export Engineering Whitepaper... in the actual app. Action: export and review chapter 8, pages 21-22.
   Expected: established 50 mm/24 h/five-position/10 s method with separate location and independent-specimen statistics.

### Shore clarification final verification

Final disposable profile `20260912042153-c9d8b6d4`: smoke PASS and Full Verification 462/462 PASS.
Database baseline/final SHA-256: `0F9A1B51479016DCD6F3F382C55AC8E6D1FF93D9D9AD8A813B66675F0043AC91`.
Business-state baseline/final: `E8BCF04B15DCFC77D4E5228FC96DB76A257887F40C67EA3BB08170F05AE2CC88`.
Debug/Release zero warnings/errors, Help 802/802, documentation audit and diff whitespace checks PASS.
The first 461/462 run exposed a widened AI DTO shape; the AI serializer now retains its established group-only allowlist.
Failed profile `20260912041818-e3aaee74` is retained for diagnosis; final run verifies the correction.
Final whitepaper pages 21-22 and browser Shore popup/report tables were visually inspected; owner readability review pending.
Local evidence: `.private/v67-shore`. No owner-data mutation, live publication, commit or push in this unaccepted increment.

### Owner acceptance and publication handoff

Owner accepted the corrected Shore method and statistics. Local v67.0.2 is ready for guarded website publication.
The owner intends to publish through Website Export. No live transfer was executed or verified in this handoff.
v67.0.3 remains open until successful transfer and independent HTTPS checks are recorded.

## v67.0.3 - Flexible Website Publication and Closure

Documentation-only closure; accepted runnable application remains v67.0.2 and its tested Release bytes are preserved.
Owner published the website and confirmed its live appearance, then requested closure on 2026-09-12.
Independent HTTPS returns 200 for the main site, updated Whitepaper, stable Windows installer and portable ZIP links.
Live HTML includes Flexible Testing and within-specimen Shore statistics; the downloaded PDF confirms the established
50 x 50 x 8 mm, 24-hour, five-position, 10-second Shore method. No Codex FTPS operation or remote-backup claim is made.
All recorded v67 increments are complete. Debug/Release zero warnings/errors; final disposable Verification 462/462 PASS.
Help 802/802, documentation audit and diff checks pass. README and feedback are reconciled; no new milestone is scheduled.
Retained template transforms support existing SQLite website templates and remain actively called; no retired path is unowned.
Application installer/update packages were not rebuilt or promoted in this website release; their separate acceptance still applies.
Evidence: `.private/v67-closure`, `.private/v67-shore`, and `Docs/V67_FLEXIBLE_WEBSITE_ACCEPTANCE.md`.
