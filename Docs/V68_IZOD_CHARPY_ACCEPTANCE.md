# v68.0.0 — Izod and Charpy Measurement Foundation

Status: Implementation candidate; owner runtime and visual acceptance pending.
Accepted baseline remains v67.0.8. Existing v67 input work and owner deferrals are preserved.

## Búið / Breytingar

Two separate workspaces use the existing Fast Materials/Tensile renderer directly.
SQLite owns independent MaterialID-linked runs and specimens, planned target 10 for each method,
raw energy J, method-specific hammer capacity, measured section geometry, fracture/validity and print batch.
Run settings preserve nominal geometry, explicit orientation, notch preparation, printing, drying and conditioning.
Unknown dimensions and environment values remain blank. Run metadata and specimen overrides are retained separately.
No legacy Impact reinterpretation, pooled Izod/Charpy average, automatic outlier removal or overall score change.

The owner plans long-side printing with 100% infill. The exact bed-contact face and layer/notch directions still
require confirmation from the specimen files. Similar orientation to My Tech Fun does not establish equivalent methods.
Ten independent specimens improve estimation of spread but cannot remove systematic setup differences.

## Calculation and method evidence

ISO's definitions describe impact strength as absorbed energy divided by original cross-sectional area;
for notched specimens that area is the remaining section at the notch. The implemented unit conversion is
`strength kJ/m² = 1000 × absorbed energy J / section mm²`.
Unnotched rectangular section uses measured width × thickness; notched section uses remaining ligament × thickness.
There is no assumed 4 × 8 mm section and nominal profile dimensions are never substituted for measurements.
The formula is a unit conversion over the recorded section, not evidence of full standards conformity.

Primary method references inspected 2026-09-21:

- [ISO 180:2023](https://www.iso.org/standard/84394.html): Izod method and configurable specimen/test conditions.
- [ISO 180 preview](https://cdn.standards.iteh.ai/samples/84394/5153dee23ba64b048fe7ce68e1fc36a1/ISO-180-2023.pdf):
  ISO-authored definitions of notched and unnotched impact strength.
- [ISO 179-1 definition](https://www.iso.org/obp/ui#iso:std:iso:179:-1:ed-4:v1:en): Charpy section and kJ/m² definitions.

Standard editions in the actual procedure remain owner-confirmed profile metadata. No certification or full conformity claim.
Complete-break eligible results own the main numerical summary. No-break, partial/hinge, invalid and excluded records remain
separate evidence. Sample SD uses n-1 and needs n >= 2; CV requires an available SD and nonzero mean.
Measured count, eligible numerical count and planned target are distinct. Batch and incompatible override conditions stay separate.

## Automated acceptance assessment

Extend the existing smoke navigation contract with both method tabs and register calculation, storage and input contracts
in Full Data Verification. Synthetic/disposable fixtures cover raw-value roundtrip, legacy preservation, no-break,
exclusions, missing dimensions, unit conversion, sample statistics, partial runs and method/condition isolation.
No new Production, FTPS, restore, owner-database or destructive scenario authorization is introduced.
The existing canonical tester seed remains a supported migration input; no refresh is needed merely for additive schema 45.
The existing authorized CRUD scenario creates two method runs on its disposable material, exercises the real commit callback,
checks persistence after restart, edits/clears values, then removes only its fixture before exact business-state recovery.
First-run selector initialization is tested explicitly; direct Items-to-ItemsSource mixing is prohibited by the fixture.
Material deletion checks saved pendulum dependencies before any legacy measurement mutation; Archive retains linked history.
Help owns all programmatic controls/metadata fields through measurements.pendulum-impact and the coverage ledger.
Owner typing and visual layout acceptance remain mandatory; reflection and successful builds do not establish usability.

## What to Test — actual application

Use an intended test material and real readings for permanent data; use the disposable tester profile for illustrative values.
The following actions are in the application, not the Help window, unless the step says otherwise.

1. **Where:** Navigate > Measurements & Testing > Izod Measurements.
   **Action:** select the intended Material, then New run. **Expected result:** an Izod run with ten blank specimens and target 10;
   no numerical strength from blank rows. **Do not click:** any Production, FTPS, Restore or Delete action elsewhere in the app.
2. **Where:** the new run's settings table and group selector.
   **Action:** record date, nominal profile, actual orientation, notch state, print settings and known conditioning values.
   **Expected result:** unknown fields can remain blank; one click starts editing, immediate typing is retained, Tab/Shift+Tab
   and arrows behave predictably without jumping to another run or losing the next cell's editor.
3. **Where:** specimen table in the same Izod run.
   **Action:** enter actual hammer J, measured width/thickness/remaining ligament where applicable, absorbed J and break type.
   Commit with Tab. **Expected result:** eligible complete breaks display calculated kJ/m² and summary n; target remains visible.
   Move forward/backward through editable cells and between rows; calculated output cells must be skipped.
4. **Where:** the same specimen table, preferably in the disposable profile.
   **Action:** record a no-break outcome and then an excluded reading with its reason entered before exclusion status.
   **Expected result:** raw values remain visible; no-break is not zero and neither row joins the complete-break average.
   Leave another row unmeasured. Incomplete runs must still save without invented zeros.
5. **Where:** Navigate > Measurements & Testing > Charpy Measurements.
   **Action:** create a separate run for the same material and enter its actual setup/readings.
   **Expected result:** independent ten-specimen run, Charpy hammer choices and separate summaries; Izod and legacy Impact unchanged.
6. **Where:** either new tab > specimen Print batch and batch selector.
   **Action:** assign actual batch labels, select each batch, then all batches.
   **Expected result:** batch-scoped counts/statistics and separate incompatible conditions; no automatic outlier removal.
7. **Where:** both new tabs, then close and reopen the application.
   **Action:** commit a last-cell edit, switch run/method, return, close and reopen.
   **Expected result:** exact run/specimen IDs, raw readings, metadata, diagram and exclusions persist under the correct material.
8. **Where:** Help window, opened with F1 while on either new tab.
   **Action:** read Izod and Charpy Measurements. **Expected result:** current field labels, units, save timing and method limits.
9. **Where:** Navigate > Materials & Setup > Materials, then Navigate > Publishing > Reports / PDF Export.
   **Action:** select the intended material in Materials first. In Reports choose Material Engineering Report,
   Report scope = Selected Material Only, then Refresh Preview. Inspect the separate Izod/Charpy result blocks.
   **Expected result:** readable wrapping, units, n/target, sample SD/CV and notch/condition context; no conformity claim.
   **Do not click:** live Publish/FTPS/Production controls; this step authorizes local inspection only.
10. **Where:** Tools > Verification Center (a separate window).
    **Action:** wait for the Full Data Verification result. **Expected result:** all mandatory checks PASS.
    Record the displayed result with input acceptance. **Do not click:** Restore, Production or FTPS controls elsewhere.

## Build, package and verification evidence

2026-09-21 candidate verification:

- Normal Debug and Release builds: zero warnings and errors. Owner runnable Release directory updated intentionally.
- Full Data Verification: 472/472 PASS, zero failed or not-applicable checks.
- Exact Release CRUD profile: `20260921130621-53b8281e`; create/edit/two restarts/delete/recovery PASS.
- Seed SHA-256: `CEF1F9D5142578BBFE91D37A79BF11A6255CE550B8CA388D8582BF2D7D332D09`.
- Baseline/final business hash: `177F36F2E04451F190D9935B7FEA3A0B28B1BD7D392B9AD4E560FFD386C23D5E`.
- Release DLL SHA-256: `E5F8C185C2B252F5F3FEC4079ABB8B8DF17234462957B41F84539F7FAC04532A`.
- Help: 803/803 XAML candidates plus registered runtime controls; documentation and diff gates PASS.
- Dependency vulnerability audit: no vulnerable packages reported by configured sources.
- README stable installer and portable HTTPS routes: both HEAD 200.
- Both populated-tab screenshots inspected. Synthetic long-description HTML and three-page PDF inspected;
  no horizontal HTML overflow, truncation or orphaned pendulum group headings.
- Evidence retained under `.private/v68-izod-charpy/accepted-candidate` and `visual`.
  The directory name denotes automated candidate acceptance, not owner runtime acceptance.
- Initial failed fixture `20260921125403-19217842` retained under `failed-initial`; stale table/allowlist gates
  and older-schema recovery fallback were corrected before final passing verification.

The subagent review found first-run selector binding, reason-entry order and pre-delete preservation issues;
all were corrected and the real CRUD path rerun successfully. Existing shared deferred focus behavior is preserved.
Automated callbacks do not establish immediate physical keyboard timing: the owner input checklist remains mandatory.
No existing v67 latency issue is declared solved. No owner database/seed mutation, commit/push or live publication occurred.
Candidate ZIP contains App/, Docs/ and Reports/ only; excludes SQLite, credentials and Production packages.
Do not close the milestone or replace accepted release identity before owner runtime acceptance.
