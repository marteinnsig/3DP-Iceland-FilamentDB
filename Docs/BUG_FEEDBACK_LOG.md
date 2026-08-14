# Bug / Feedback Log

Use this during the usage-mode period.

## Status review — 2026-08-02

This review preserves every original description and lifecycle status.
`Solved` means the change is implemented, relevant gates pass and owner runtime
testing has confirmed the result. Runtime and Verification evidence remains
recorded inside each finding rather than creating a second solved category.
`Deferred` records a deliberate boundary rather than silently discarding the
idea.

| Status | Items |
|---|---:|
| Open | 3 |
| In progress | 0 |
| Partially solved | 0 |
| Solved | 105 |
| Deferred | 3 |
| Duplicate | 2 |
| Not planned | 1 |
| **Total tracked findings** | **114** |

## Triage categories

- Bugs: incorrect behavior or broken workflow.
- Workflow friction: too many clicks, confusing flow, repeated manual work.
- UI polish: readability, sizing, labels, layout.
- Website ideas: improvements visible to YouTube viewers or public users.
- Reporting ideas: improvements to report clarity or sharing.
- Data issues: inconsistent material metadata, missing links, missing measurements.

## How to log an item

Copy this block for each finding:

```text
Date:
Area:
Type: Bug / Workflow friction / UI polish / Report idea / Website idea / Data issue
Severity: Blocker / Important / Minor / Idea
What happened:
Expected behavior:
Steps to reproduce:
Screenshot / export / report attached:
Status: Open / In progress / Partially solved / Solved / Deferred / Duplicate / Not planned
Resolution:
Verification evidence:
```

# 2026-07-28 - v59.0 navigation inventory and tab-order ownership

- **v59.0.2 implementation:** A grouped Navigate menu now exposes six ordered
  workflow groups and all 22 top-level destinations through stable menu
  AutomationIds.
- **Boundary:** Commands only select, bring into view and focus their target
  tab. They do not save, recalculate, restore, export, publish or change
  filters; normal tab selection lazy-load/read behavior remains unchanged.
- **Transition:** The accepted Website menu remains callable until Navigate >
  Publishing > Website Export passes owner runtime acceptance. The prototype
  command also remains pending its bounded v59.0.3 retirement.
- **v59.0.2 evidence:** Debug/Release and disposable smoke
  `20260729004027-cb36e901` pass. Automation invokes all 22 Navigate commands,
  verifies their selected stable tab destinations, passes Verification 416/416
  and restores exact logical/business state.
- **Owner acceptance:** All six groups, mouse and keyboard navigation and the
  Website replacement work correctly; Full Data Verification passes.
- **Status:** v59.0.2 complete and runtime accepted. The v59.0.3 retirement
  condition is met.
- **v59.0.3 implementation:** The one-command Website menu and its handler are
  removed after replacement acceptance. The snapshot-only prototype menu,
  window handler, tester step, runtime registry and stale Help ownership are
  also removed.
- **Preserved rendering:** The shared owner-drawn renderer and its direct
  canonical callers in Materials, measurement, Settings and Base Materials
  remain. Navigate > Publishing > Website Export owns supported menu access.
- **v59.0.3 evidence:** Debug/Release and smoke
  `20260729005051-359d2f59` pass 417/417. The retirement contract, all 22
  Navigate destinations, embedded Fast rendering contracts and exact
  logical/business-state recovery pass.
- **Owner acceptance:** Retired menu entries are absent; Materials,
  measurement, Settings and Base Materials rendering remains normal; Full Data
  Verification passes.
- **Status:** v59.0.3 complete and runtime accepted.
- **v59.0.4 reconciliation:** Help inventory, coverage registry, matrix,
  AutomationRunner, Verification, release chronology and feedback records agree
  on 22 top-level, 16 nested and 22 grouped Navigate destinations.
- **Automation assessment:** No new scenario is warranted. Existing smoke
  already invokes every Navigate leaf and proves retired surfaces absent while
  preserving exact state.
- **Status:** v59.0.4 complete; no runtime behavior changed after the accepted
  417/417 v59.0.3 evidence.
- **Runtime defect found 2026-07-29:** Leaving an edited Inventory cell could
  terminate the process without an app dialog.
- **Evidence:** Windows .NET Runtime event 1026 records SQLite error 19 at
  `ReplaceInventorySpoolItems` during deferred `InventorySpoolGrid_CellEditEnding`.
- **Root cause:** Inventory save deleted every spool before reinserting the
  snapshot. Canonical Usage rows use `ON DELETE RESTRICT`, so even an ordinary
  edit failed when any saved spool was referenced by Usage history.
- **Correction:** Save now upserts stable spool identities and deletes only
  records absent from the snapshot. A genuinely referenced deletion rolls back,
  shows `Inventory Save Blocked` and reloads canonical Inventory state.
- **Regression contract:** A disposable SQLite fixture must prove that an
  Usage-referenced spool can be edited, its deletion is rejected and both
  spool/Usage rows survive the rejected transaction.
- **Correction evidence:** Debug/Release pass. Disposable smoke
  `20260729002614-0f7d20d1` passes 415/415; both the canonical tab-order and
  referenced-spool save/restricted-delete checks pass with exact logical and
  business-state recovery.
- **Finding:** XAML source order is not the visible runtime order.
  `ApplyWorkspaceTabPriorityOrder` moves Material Detail after Materials, then
  moves Manufacturers, Purchase Orders, Inventory and Experimental Testing
  after Website Export during startup.
- **Risk:** Reordering XAML alone would be partially overridden and could make
  tests, Help ownership and future maintenance disagree with the visible UI.
- **Decision:** The owner approved the complete 22-tab order, a grouped
  Navigate menu and retirement of the redundant Website menu plus snapshot-only
  prototype command after replacement runtime acceptance.
- **Follow-up:** Review Materials default column order later in v59 through a
  separate research and owner-decision increment before any implementation.
- **v59.0.5 finding:** Materials owns 52 visible, unfrozen columns. Saved
  width/order preferences override defaults and remain isolated from canonical
  data, reports, PDF and website exports.
- **v59.0.5 defect:** Reset Columns clears only the legacy compatibility list;
  the current `FastGridLayouts["Materials"]` entry can restore the old layout
  after restart.
- **v59.0.5 decision:** The owner approved an exact 52-column default order.
  Existing saved layouts remain untouched; clean profiles and explicit reset
  receive the new order. All columns remain visible and unfrozen.
- **v59.0.6 correction scope:** Reorder only the default builder, clear both
  Materials layout stores during reset and add deterministic order/restart
  contracts without changing exports or canonical data.
- **v59.0.6 implementation evidence:** Debug/Release pass with zero warnings;
  Help and release-documentation gates pass. Disposable Release smoke
  `20260729011614-ee93c65e` passes 417/417 with exact logical/business state.
- **Runtime acceptance remaining:** Confirm saved-layout preservation, explicit
  reset order and reset persistence after restart in the owner application.
- **Owner runtime finding:** The first v59.0.6 candidate passed Verification and
  layout persistence, but Reset Columns did not map window close or Escape to
  No. Purchase Currency also belonged directly after Purchase Price.
- **Correction:** Reset Columns now uses the accepted named default-No dialog
  shared with safe Material lifecycle warnings. No, Escape and window close
  preserve layout; the exact default order places Currency after Purchase Price.
- **Correction evidence:** Debug/Release and Help/docs gates pass. Disposable
  smoke `20260729012749-9dc37e1a` passes 418/418 with exact logical/business
  state; focused owner dialog/order retest remains.
- **Owner acceptance:** Exact default order, Purchase Price/Currency adjacency,
  saved/reset restart behavior and Reset Columns No/Escape/window-close safety
  all pass. Owner Full Data Verification reports PASS.
- **Status:** v59.0.6 complete and runtime accepted.
- **Preserved:** All 22 top-level and 16 nested AutomationIds, F1 Help mapping,
  selection-triggered refresh/lazy loading and guarded actions remain unchanged
  during research.
- **Implementation:** The complete owner-approved sequence is canonical in XAML;
  the startup reorder helper is retired and the tester registry matches the
  same stable AutomationId order.
- **Evidence:** Debug and Release pass. Disposable smoke
  `20260729001518-51116653` visits 22/22 top-level and 16/16 nested tabs,
  passes Full Data Verification 414/414 and restores exact logical/business
  state.
- **Owner acceptance:** Inventory remains stable after repeated tab navigation
  and editing; the owner completes the earlier full-tab checklist and confirms
  Full Data Verification PASS.
- **Status:** Resolved and runtime accepted in v59.0.1.

## Open findings

Date: 2026-08-02
Area: Native measurements / fixture-specific heat deflection
Type: Workflow friction / Data issue
Severity: Important
Status: Solved
What happened: The owner has built a thermal bend fixture but the application has no dedicated place to record the result for each
MaterialID. A 127 x 12.7 x 3.2 mm specimen is printed flat and supported on two end pillars with a 110 mm clear span. A 54 g M20 nut
applies the centered load, and the recorded endpoint is the temperature at which mid-span deflection reaches 2 mm.
Expected behavior: Add a dedicated `Heat Deflection` measurement tab using the accepted Stiffness-style synchronized material list.
Each material has one editable result cell, `Deflection temperature °C`, with material identity read-only and canonical auto-save.
Define it as the nearby BlueDOT probe-indicated temperature at the 2 mm endpoint, not internal specimen temperature.
Identify the value as a 3DPIceland fixture-specific result rather than standardized ASTM D648 or ISO 75 HDT unless the complete
standard method is actually followed. Preserve the exact method version behind every saved result.
Steps to reproduce: Open the application measurement tabs; Tensile, Impact and Stiffness exist, but there is no MaterialID-linked
surface for entering the temperature observed at 2 mm deflection in the owner's 110 mm-span/M20-nut fixture.
Screenshot / export / report attached: None; owner fixture and workflow description on 2026-08-02. The specimen is heated in an
oven. A digital grill-temperature probe sits immediately below the specimen beside the central metal bolt/nut assembly, close to
the specimen. One test and one recorded result are used per material.
Resolution: Planned under v61.0.0 as a distinct Thermal Deflection Measurement Foundation after the current v60 sequence. Reuse
stable MaterialID synchronization, Fast-grid editing, canonical SQLite ownership, locale-aware numeric input, auto-save, filtering,
column reset, measured-date/notes evidence, Excel import/export, Help and Verification patterns from native Stiffness without
coupling the new result to stiffness formulas or existing Engineering Scores. The approved fixture baseline is a 54 g nominal
moving load, approximately 0.530 N under standard gravity; the central bolt adds no specimen load; 110 mm span; 2.00 mm endpoint;
127 x 12.7 x 3.2 mm flat-printed specimen; oven heating; and one test per material. The nearby under-specimen sensor is a BlueDOT
from thermapen.co.uk, FCC ID `2A167 BlueDot`, with no user calibration option. From a 25 °C ambient start, the observed checkpoints
are 50 °C at 1:50, 100 °C at 3:26, 150 °C at 4:35, 200 °C at 6:53 and 250 °C at 10:30. Treat this as a non-linear observed oven
ramp profile rather than one constant heating rate, and disclose the uncalibrated-probe limitation. Keep the grid to one editable
temperature result per material; centrally govern and automatically snapshot the method metadata.
Workbook research on 2026-08-14 finds 221 unique MaterialIDs, 191 numeric results and 30 blanks with no duplicate IDs or formulas.
The owner confirms `MAT0107 = 21` was an entry error and corrects it to 51 °C; no below-ambient workbook value remains.
The authoritative staged delivery sequence is recorded under v61 in `Docs/Roadmaps/MASTER_ROADMAP.md`.
v61.0.1 candidate adds schema v41, immutable method-v1 storage and a preview-first ClosedXML importer. Exact MaterialID binding,
25-300 °C validation, non-destructive blanks, duplicate/unknown/non-numeric rejection, source SHA-256 and idempotent apply are
deterministically verified. No user-facing import control, Help destination, public output or Engineering Score changes yet.
Verification evidence: schema-v40 migration and refreshed schema-v41 seed profiles pass 426/426 with exact business-state recovery.
The corrected-workbook preview against the configured 221-Material database resolves all IDs: 191 inserts, 30 blanks and zero
issues; no measurement was written. The earlier 20-unknown result used a stale 201-Material tester seed and is superseded. Future
acceptance must prove
add/edit/clear/restart, invalid/range and locale handling,
MaterialID identity synchronization, delete/archive behavior, method-snapshot immutability, schema migration, Excel round trip,
Help/navigation, disposable automation and Full Data Verification with exact business-state recovery. Public reports, website
publication and Engineering Score use remain out of scope until separately designed and approved.
Owner accepted the corrected 221-Material preview and v61.0.1 schema/import foundation on 2026-08-14.
v61.0.2 adds the native Fast workspace, validation, date/notes, immutable method display, auto-save, blank-clear, navigation and
central Help. Profile `20260814174328-29081146` passes 427/427 and exact recovery. Owner runtime/visual acceptance passes on
2026-08-14 for entry, invalid-value rejection, restart persistence, explicit clear, layout and Help readability.
v61.0.3 uses a guarded one-time transition to populate 191 results and preserve 30 blanks from owner-confirmed source SHA-256
`5CC2742C...58CE`. Profile `20260814181952-48fb589d` passes 428/428 with exact state recovery. Owner accepts preview/apply,
restart, export readability and Verification, then requests retirement because future measurements are entered directly in the app.
The buttons, handlers, importer, verifier, Help and automation discovery are removed.
Post-retirement profile `20260814184446-a76801d2` passes 428/428 with exact recovery; Help passes 696/696. Owner accepts the
final post-retirement UI and Verification PASS on 2026-08-14; the feedback is resolved.

Date: 2026-07-30
Area: Application date entry and display / Purchase Orders / Materials
Type: Workflow friction / Data issue
Severity: Important
Status: Resolved
What happened: Dates entered manually are understood in Icelandic day-month-year order, while a Purchase Order created by the
application receives `yyyy-MM-dd`. That value is copied unchanged into Materials `Purchase Date` and `Price Checked`, so equivalent
user-facing date fields show different formats depending on their source.
Expected behavior: User-editable date fields should parse and display according to the active Windows system locale; for example,
an Icelandic locale should use its configured day-month-year short-date format consistently. Persist an unambiguous canonical
`yyyy-MM-dd` date in SQLite and localize only the UI boundary, rather than storing locale-dependent text that becomes ambiguous when
the database moves between computers or the system locale changes. Keep UTC timestamps, manifests, machine evidence and other
deterministic interchange values in their governed ISO formats, and do not silently rewrite immutable historical snapshots.
Steps to reproduce: Create a Purchase Order and inspect its `Purchase Date`; create Materials from its Filament lines and compare
the resulting Materials `Purchase Date` and `Price Checked` values with a date entered manually under an Icelandic Windows locale.
Screenshot / export / report attached: None; owner workflow observation on 2026-07-30.
Resolution: Scheduled as v60.0.6. Inventory every editable/displayed application date and classify it as a calendar date, UTC
timestamp, immutable snapshot or interchange/export value before implementation. Define one locale-aware parse/display boundary
with canonical ISO persistence and explicit legacy-input compatibility; do not apply locale formatting blindly to public reports,
file names, IDs, logs or signed/deterministic evidence.
Research evidence: editable calendar surfaces are Purchase Order Order/Received date, Inventory and Materials Purchase date,
Materials Price/Printing Settings Checked date, Video Publish date, and native/Experimental Measured date. Rate dates are
read-only snapshot provenance. The grids currently mix raw ISO strings with the native measurement path's fixed Icelandic display;
PO creation stores ISO and copies that raw text into Materials. The bounded design uses one shared active-Windows-culture display
and parse codec, canonicalizes successful edits/PO propagation to ISO before save, rejects invalid or ambiguous non-locale input,
and never bulk-rewrites load-time legacy data or immutable snapshots. Excel/JSON, public reports, UTC evidence, file names, IDs,
manifests and logs retain their governed formats. Owner approved this bounded design before implementation.
Implementation evidence: the approved shared codec now localizes PO Order/Received/Rate, Inventory/Materials Purchase, Materials
Price/Settings Checked, Video Publish and native/Experimental Measured dates at the UI boundary. Valid edits and PO downstream
propagation use ISO; invalid grid edits do not persist. Internal purchasing reports localize only their calendar values while
generated timestamps stay deterministic. Help and the existing measurement gate are updated. No schema, seed, immutable snapshot,
public-report, Production or FTPS contract changed. Existing reports automation remains the correct bounded owner; no new mutating
scenario or AutomationId is warranted. Disposable profile `20260814153636-06cea92b` passes Full Data Verification 425/425, the
dedicated `is-IS`/`en-US`/ISO/leap-day/invalid/legacy gate, 627 artifact checks and exact logical/business-state recovery. Owner
accepts the Icelandic runtime presentation and confirms Full Data Verification PASS.

Date: 2026-07-30
Area: Fast Materials / editable-cell keyboard navigation and focus
Type: Bug / Workflow friction
Severity: Important
Status: Solved
What happened: After entering a value such as `GF` in Reinforcement and pressing an arrow key, focus initially moves toward the
adjacent cell but then jumps back or closes. Clicking another editable cell can also lose focus or activate a nearby cell without an
obvious pattern. The behavior occurs across editable Materials fields, not only Reinforcement.
Expected behavior: Arrow keys commit the current value and move exactly one row/column to the requested editable cell, which remains
active for continued typing. A mouse-selected cell must retain its editor until the user commits, cancels or deliberately navigates;
background auto-save/refresh must never steal or redirect focus.
Steps to reproduce: Edit Reinforcement, enter `GF`, press Down to edit the row below, and begin typing. Repeat with Left/Right/Up or
click another editable cell while the prior edit's deferred auto-save is pending.
Screenshot / export / report attached: None; owner runtime reproduction on 2026-07-30.
Resolution: Scheduled as v60.0.2. Read-only diagnosis confirms a focus race. Arrow navigation closes the current editor and
schedules the adjacent editor at Input priority. The Materials auto-save timer then rebuilds filters and invokes canonical
synchronization more than once; each synchronization calls `CloseEditor(commit: true)` and can close the newly activated cell.
Preserve auto-save, filters, viewport and
canonical refresh while preventing background synchronization from closing or relocating an intentional active editor.
Verification evidence: Not yet implemented. Future deterministic coverage must prove all four arrow directions, Tab/Shift+Tab,
mouse activation, rapid repeated edits, one-field auto-save and unchanged row/column/viewport selection after deferred refresh.
Candidate evidence: v60.0.2 ignores delayed focus-close events unless their source is still the active editor and defers only
background canonical synchronization while an intentional editor remains active. Explicit reload/close/reset behavior is unchanged;
auto-save, validation, filters, selection and viewport continue through the accepted paths. Debug/Release and Help pass. Disposable
smoke `20260730145422-31f69821` passes Full Data Verification 422/422 with exact logical/business-state recovery. Deterministic
coverage proves stale/current event ownership plus active/inactive background-refresh decisions. Owner runtime acceptance remains.
Final resolution: Solved in v60.0.2 — 2026-07-30. Owner accepts four-direction arrows, Tab/Shift+Tab, mouse transfer, rapid edits,
restart persistence and Full Data Verification PASS.

Date: 2026-07-30
Area: Purchase Orders / bulk Material creation
Type: Workflow friction
Severity: Important
Status: Duplicate
What happened: `Create Material from Selected Item` correctly creates only the selected line's Material, but an order containing
several Filament lines requires repeating the action line by line. The existing order-level bulk path is coupled to positively
received quantities and Inventory-spool creation.
Expected behavior: Add an explicit selected-order action that creates and links one draft Material for every unlinked line whose
Inventory Category is `Filament`, without creating Inventory spools. Skip already linked and non-Filament lines; block missing
descriptions; preview exact create/skip/error counts in a default-No confirmation.
Steps to reproduce: Select a Purchase Order containing several described, unlinked Filament lines and compare
`Create Material from Selected Item` with the desired order-level material-only workflow.
Screenshot / export / report attached: None; owner workflow request on 2026-07-30.
Resolution: Owner withdrew the feature request after confirming that `Create Materials + Received Spools` already creates Materials
and spools in bulk for the eligible Filament lines in the selected order. Preserve the singular selected-item action and the existing
received-order bulk workflow; no separate material-only action is planned.
Verification evidence: Owner runtime observation on 2026-07-30 confirmed that the existing order-level action created all expected
Materials and spools. This disposition does not resolve the separately tracked foreign-key crash in the shared creation path.

Date: 2026-07-30
Area: Purchase Orders / Create Material from Selected Item
Type: Bug / Data issue
Severity: Blocker
Status: Solved
What happened: After creating a Purchase Order with several lines and calculating landed cost, clicking
`Create Material from Selected Item` terminated the application.
Expected behavior: Validate and persist the new parent Material before assigning its MaterialID to the Purchase Order line. If the
Material save or link save fails, roll back both in-memory changes, keep the app running, preserve the order/landed-cost snapshot and
show one actionable error without creating an orphan, partial link or duplicate retry.
Steps to reproduce: Create a Purchase Order, add several item lines, calculate landed cost, select an unlinked Filament line and click
`Create Material from Selected Item`.
Screenshot / export / report attached: Windows Application events at 2026-07-30 13:50:44; .NET Runtime event 1026, Application Error
1000 and WER report `4d4e4004-1f39-47a8-9dad-a1562ee87812`.
Resolution: Scheduled first as v60.0.1. Read-only diagnosis confirms unhandled SQLite error 19 in `ReplacePurchaseOrders`, called by
`CreateMaterialFromPurchaseLine_Click` line 28744. The handler assigns the generated MaterialID to the line, ignores the Boolean
result of `SaveNativeMaterialsSilent()` and then saves the foreign-key child link. The bulk inventory/material path has the same
unchecked parent-save sequence. Correct both paths through one fail-closed parent/link workflow; do not weaken the foreign key.
Verification evidence: Not yet implemented. Future disposable coverage must exercise selected and multi-line creation, parent
validation/save rejection, landed-cost retention, exact rollback/retry behavior, no orphan/duplicate rows, continued app operation
and exact final business-state recovery.
Candidate evidence: v60.0.1 now persists canonical Materials and Purchase Order links in one SQLite transaction shared by the
selected-item and received-order bulk paths. Validation or persistence failure removes the candidate rows/links from memory, keeps
the app running and shows `Material Creation Blocked`. Debug/Release and Help/docs gates pass. Disposable Release smoke
`20260730143243-0f8ff11b` passes Full Data Verification 421/421 with exact logical/business-state recovery. The synthetic contract
proves single- and multi-line commit, forced foreign-key rollback, landed-cost retention and duplicate-free retry. Owner runtime
acceptance confirms selected-item creation, bulk Materials/spools and duplicate-free retry. The first owner Verification export
passed v60.0.1 but exposed a false v53.0.1 failure: 14 valid versioned cross-currency Inventory rows were incorrectly required to
have a legacy 1:1 rate. Verification now accepts positive versioned rates with calculation provenance while retaining exact 1:1
requirements for `legacy-v1`. Corrected smoke `20260730144248-13602dab` passes 421/421; owner Verification rerun remains required.
Final resolution: Solved in v60.0.1 — 2026-07-30. Owner runtime accepts selected-item creation, bulk Materials/spools,
duplicate-free retry and corrected Full Data Verification PASS.

Date: 2026-07-30
Area: Materials / Tensile / Impact / Stiffness identity synchronization
Type: Bug / Data issue
Severity: Important
Status: Solved
What happened: For `MAT0207`, a Color-only correction did not reliably update the visible Color in Tensile, Impact and Stiffness.
Changing Color to `Yellow` propagated, but changing it back to `Black` did not. Changing Marketing Name from `Black` to `test1`
then caused all three measurement views to display the already-saved `Black` Color.
Expected behavior: Every committed Materials identity-field change, including Color alone and values equal to Marketing Name, must
refresh Tensile, Impact and Stiffness by stable MaterialID. Synchronization must compare each named field independently and must not
depend on a combined display label changing.
Steps to reproduce: Create or select `MAT0207`; set Marketing Name and Color to `Black`; change Color to `Yellow` and confirm all
measurement views; change Color back to `Black`. If they remain Yellow, change only Marketing Name to `test1` and observe Color
refresh to Black in all three views.
Screenshot / export / report attached: None; owner runtime reproduction on 2026-07-30.
Resolution: Scheduled as v60.0.3. Read-only source review confirms canonical Materials persistence and all three MaterialID-based
copy methods include Color. Investigate the Fast measurement snapshot/invalidation trigger and any display-identity equality
shortcut; do not rewrite measurement samples or make identity fields measurement-owned.
Verification evidence: Not yet implemented. Future deterministic coverage must exercise Color-only changes in both directions when
Marketing Name equals Color, prove immediate three-view refresh and restart persistence, and preserve samples, dates and notes.
Candidate evidence: v60.0.3 now compares every named identity field independently, copies identity by stable MaterialID into Tensile,
Impact and Stiffness, then reloads the three Fast views once after all copies complete. Lifecycle callers use the same three-view path;
samples, dates and notes remain measurement-owned. Debug/Release and Help pass. Disposable smoke
`20260730151123-a5098658` passes Full Data Verification 423/423 with exact logical/business-state recovery. Deterministic coverage
proves Black→Yellow→Black while Marketing Name is Black, a subsequent Marketing Name-only change and preserved measurement evidence.
Owner retest on 2026-07-30 failed: Black did not replace Yellow in the three measurement views, close showed `Auto-Save Blocked`,
and restart restored Materials Color Yellow. Root cause: duplicate Website Display Name was incorrectly a silent-save blocker even
though MaterialID owns relationships, and close stopped the edit debounce before synchronizing a final committed identity edit.
Follow-up candidate keeps duplicate display names as validation warnings, persists synchronized measurement identity after normal
Materials auto-save, and synchronizes after the parent save before guarded measurement persistence during close. Debug/Release pass
with zero warnings/errors. Corrected Release smoke `20260730162359-1eaaead1` passes 423/423 with exact logical/business-state
recovery. Solved in v60.0.3 on 2026-07-30: owner confirms the corrected
Black/Yellow/Black flow updates Materials, Tensile, Impact and Stiffness,
closes without `Auto-Save Blocked`, persists after restart, preserves
measurement evidence and passes Full Data Verification.

Date: 2026-07-29
Area: Public Manufacturer Report / product-level engineering table
Type: Bug / UI polish
Severity: Important
Status: Solved
What happened: The wide `Product-level engineering context` table extends beyond the right edge of the report frame. Its final
columns and row content render outside the governed page surface instead of remaining contained and readable.
Expected behavior: Keep the complete table inside the visual report contract through a responsive wrapper, readable column sizing,
natural wrapping and narrow-window horizontal scrolling where necessary. Preserve a separate print/PDF layout that fits every column
without clipping, overlap or lost content.
Steps to reproduce: Open a public Manufacturer Report containing product-level rows and scroll to
`Product-level engineering context`; inspect the rightmost MSRP, Product and Video columns.
Screenshot / export / report attached:
`codex-clipboard-a7f4753b-15c8-439a-8c16-d36110c9b8ef.png`; owner website review on 2026-07-29.
Resolution: Scheduled as v60.0.4. Initial source review finds that the 17-column table is emitted directly inside `main` without a
screen overflow wrapper; the existing compact wrapping rule applies only to print media. Research the smallest shared HTML/CSS
correction before implementation and visually inspect representative short/long manufacturer and material names.
Verification evidence: v60.0.4 candidate adds a keyboard-focusable responsive screen wrapper, visible focus, natural wrapping and
a separate compact print override without changing cells, links or allowlists. Debug/Release pass with zero warnings/errors.
Disposable reports profile `20260814134142-7857ce51` passes Full Data Verification 423/423, 627 artifact checks and exact recovery.
The four-page Prusa3D PDF retains all 17 columns/links without clipping or overlap. Owner accepted the contained HTML table,
horizontal access to the rightmost columns, Verification PASS and the unclipped PDF. Solved in version v60.0.4 — 2026-08-14.

Date: 2026-07-29
Area: Public Printing Guidance / Base Material Catalog settings
Type: Report idea / Data issue
Severity: Important
Status: Solved in v59.0.11
What happened: Public Printing Guidance shows `Not recorded` for every canonical printing setting even when the MaterialID is linked
to a Base Material Catalog row containing nozzle, bed, speed, cooling, drying, enclosure and printer/slicer guidance.
Expected behavior: Resolve each eligible MaterialID through its canonical `BaseMaterialId` and show the matching Base Material Catalog
values as an explicitly labelled base-material baseline. Preserve min/recommended/max values and units; show `Not recorded` for each
empty field or when no valid catalog link exists. Do not present generic material-type guidance as manufacturer- or spool-specific
validated settings.
Steps to reproduce: Open a public Printing Guidance report for a MaterialID whose linked Base Material Catalog row has printing
settings; inspect the `Canonical printing settings` table.
Screenshot / export / report attached:
`codex-clipboard-9ffab59a-a298-4a0c-915d-61b04934ff69.png`; owner report review on 2026-07-29.
Resolution: Research and approve a bounded public allowlist and presentation contract before implementation. Prefer stable
`BaseMaterialId` matching over text inference; retain the catalog as the single source instead of copying values into every Material.
Keep a later path for genuinely per-MaterialID or printer/nozzle-specific profiles and provenance.
Verification evidence: Candidate profile `20260729173741-50c1c3bb` passes Full Verification 420/420 and validates/hashes 627 report
artifacts with exact database/business-state recovery. Deterministic coverage proves linked/unlinked/orphan/partial behavior, units,
metadata/manifest allowlisting, escaping and catalog-aware stale rebuild. Owner accepted the full-data HTML/PDF presentation and
confirmed Full Verification PASS; the unavailable partial/unlinked cases remain covered deterministically without altering owner data.

Date: 2026-07-29
Area: Public website / Engineering Reports navigation
Type: Website idea / Workflow friction
Severity: Important
Status: Solved
What happened: `Engineering Reports` appears alongside the main website tabs, but it is an ordinary link that opens the separate
`reports/index.html` portal instead of behaving as an in-page tab like the other main website destinations.
Expected behavior: Integrate Engineering Reports as a consistent main-site tab and in-page portal section. Preserve stable report
routes, direct report links, public allowlists, responsive behavior and Preview/Production parity; do not duplicate report rendering
or weaken per-material publication controls.
Steps to reproduce: Open the main public website and select `Engineering Reports`; compare its full-page navigation with Database,
Manufacturers, Experimental, Printing Price Calculator and Methodology tabs.
Screenshot / export / report attached: None; owner workflow observation on 2026-07-29.
Resolution: Scheduled as v60.0.5. Research the current report-portal renderer, back-navigation, deep links, browser history and
mobile tab behavior before choosing an embedded-section or shared-shell design. Require owner approval of the bounded design before
implementation.
Research evidence: current tabs already use hash routing, `pushState`, `hashchange`, horizontal mobile navigation and hidden
`aria-controls` pages. The report package is one typed catalog-driven renderer, while staging preserves separate Preview/Production
portal files and every deep artifact route. Proposed design reuses one server-rendered portfolio fragment in both shells, adds a true
`#reports` tab and keeps all standalone routes; no iframe, client fetch or copied renderer. Owner approved the bounded design.
Future acceptance should cover direct/deep report URLs, tab switching, Back/Forward, responsive layout, accessibility,
Preview/Production HTML parity and unchanged report publication scope.
Implementation evidence: v60.0.5 candidate identifies the canonical package style/content, embeds that exact server-rendered content
inside `portalPageReports`, converts Engineering Reports to a real hash-routed button and preserves the standalone directory link.
Website generation fails closed if the validated fragment is absent; no iframe, browser fetch, allowlist or publication change exists.
Debug/Release app and AutomationRunner builds pass with zero warnings/errors. Disposable reports profile
`20260814142236-007cbc3a` passes Full Data Verification 424/424, 627 artifact checks and exact state recovery. The dedicated
v60.0.5 shared-renderer, `#reports`, stable-route and no-iframe/client-fetch gate passes.
Owner follow-up accepts the overall website and Verification PASS and requests removal of only the redundant standalone-directory
link plus the technical catalog/manifest note at the bottom of the embedded page; stable standalone and deep routes remain
supported. Owner confirms the final generated Preview looks correct and authorizes commit/push; Production and FTPS are untouched.
Solved in version v60.0.5 — 2026-08-14.

Date: 2026-07-29
Area: Inventory / Usage history / blocked spool deletion recovery
Type: Bug
Severity: Blocker
Status: Solved
What happened: Deleting an Inventory spool already referenced by canonical Usage history correctly showed `Inventory Save Blocked`.
After the owner clicked OK, the application terminated without another application error dialog.
Expected behavior: The restricted deletion must roll back, close the warning normally, reload the last saved Inventory state and keep
the application running. The referenced spool and Usage history must remain unchanged.
Steps to reproduce: Open Inventory, select a spool referenced by Usage history, click Delete, confirm Yes and click OK in
`Inventory Save Blocked`.
Screenshot / export / report attached: Owner screenshot
`codex-clipboard-8e00dc8f-fba2-4d4a-a3d2-8c47bcd82b57.png`; Windows .NET Runtime event 1026 at 2026-07-29 14:45:49.
Resolution: v59.0.9 candidate. Read-only diagnosis confirms that the SQLite constraint and transaction rollback work as intended. The
collection-change save handler calls `ReloadInventorySpoolsFromCanonicalDatabase`, which clears `_inventorySpoolRows` while its
`CollectionChanged` event is still active. `ObservableCollection.CheckReentrancy` then throws an unhandled
`InvalidOperationException`. The correction gives explicit deletion one save path, prevents reload during active collection
notification and retains the Usage-history restriction. The warning identifies Spool ID and MaterialID and explains the Usage filter.
Verification evidence: Event 1026 terminates in `ObservableCollection.ClearItems` from
`ReloadInventorySpoolsFromCanonicalDatabase` line 30218, called by `SaveInventorySpools` line 30208 and the collection handler at
line 30161 after `DeleteInventorySpool_Click` removes the row at line 30323. Corrected Debug/Release builds and Help/docs/NuGet gates
pass. Disposable smoke `20260729150254-6fd9ba36` passes 419/419 with exact state recovery.
Owner acceptance: The restricted deletion no longer crashes and owner Full Data Verification passes.

Date: 2026-07-29
Area: Usage / explicit development-test data cleanup
Type: Workflow friction / Data issue
Severity: Important
Status: Solved
What happened: Usage rows created while developing the workspace do not represent real owner usage. Immutable correction keeps those
rows and their spool foreign-key relationships, so it cannot return the owner database to a clean real-usage baseline.
Expected behavior: Provide an explicit backup-first maintenance exception that permanently deletes only the selected MaterialID ledger
chain after an exact default-No confirmation, without deleting its Material/spools or inferring Inventory weight restoration.
Steps to reproduce: Select a Material with development Usage rows; observe that correction/reversal cannot remove the historical rows
or release the linked spool relationship.
Screenshot / export / report attached: Owner runtime finding and accepted v59.0.9 blocked-delete evidence on 2026-07-29.
Resolution: v59.0.10 candidate adds `Delete Material Usage History...`, selected-Material scoping, row/spool counts, backup-first
execution, correction-first transactional deletion, cross-Material reference rejection and unchanged Inventory remaining weight.
Verification evidence: Debug/Release and Help/docs/NuGet gates pass. Smoke `20260729152013-913df565` passes 420/420; CRUD
`20260729152131-1cc71adc` proves cleanup is blocked by automation and exact business state recovers.
Owner acceptance: Cancel safety, selected test-data cleanup, subsequent Material deletion and Full Data Verification all pass.

## Deferred findings

Date: 2026-07-27
Area: Purchase Orders exchange-rate refresh
Type: Workflow friction
Severity: Minor
What happened: Currency required leaving the cell before rate/source refreshed. A live ECB cache still used manual EUR fallback
because the ECB base EUR leg was absent from the parsed catalog, and existing cache was not opportunistically refreshed daily.
Expected behavior: Dropdown close immediately commits currency; official EUR/ISK provenance is available; a new-order action
refreshes missing or older-than-24-hour cache while offline/manual operation and saved history remain unchanged.
Steps to reproduce: Create a new order, select EUR and inspect Rate source, observation date and ECB cache status.
Screenshot / export / report attached: Owner screenshots `codex-clipboard-31b7e94c-b01d-48f0-9afe-0d4dd73bbe0e.png` and
`codex-clipboard-7fb3c07a-35d4-49c3-8746-501d54789b4c.png`.
Status: Deferred
Resolution: Owner accepted the standard DataGrid commit behavior as a minor detail: rate/source refresh after Enter or focus change.
Unsuccessful immediate-event adapters were removed rather than retaining unnecessary UI complexity. The material defects are fixed:
the parser adds the official EUR base leg, old cache without EUR is rejected, and New Order refreshes missing/older-than-24-hour
cache with cache/manual fallback on failure and no saved-order rewrite.
Verification evidence: Owner confirms correct post-commit rate behavior and Full Data Verification PASS. Final regression gates pass;
the immediate-without-cell-commit enhancement is deliberately deferred.

Date: Historical feedback (date not recorded)
Area: AI integration
Type: Feature idea
Severity: Normal
Status: Deferred
What happened: External AI API connection
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: External AI API connectivity requires an approved concrete consumer, privacy/credential boundary and support contract. Do not implement speculatively.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Installer / uninstall
Type: Destructive workflow proposal
Severity: Important
Status: Deferred
What happened: Optional explicit clean uninstall
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Normal uninstall remains deliberately data-preserving. A destructive profile-reset option is deferred until its exact ownership, credential separation and recoverability contract justify the risk.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

## Other findings

Date: Historical feedback (date not recorded)
Area: Exchange rates / Settings
Type: Integration proposal
Severity: Important
Status: Solved
What happened: Delayed optional official exchange-rate refresh
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved through v48.2.0 and retained by later v53/daily ECB corrections. The accepted ECB SDMX catalog derives ISK through
EUR, preserves manual/offline fallback and immutable saved history, and refreshes missing or older-than-24-hour cache for new-order
work without silently rewriting accepted records.
Verification evidence: v48.2.0 is canonical and runtime accepted. Later accepted evidence confirms the official EUR base leg,
daily/missing-cache refresh, offline fallback and Full Data Verification PASS.

Date: 2026-07-28
Area: Clean installer / restored public demo Verification
Type: Bug
Severity: Important
Status: Solved
What happened: Candidate 1 installed and restored schema-v38 demo data, but
Full Data Verification failed 12 Website checks.
Expected behavior: A governed restored demo must declare the intentional
absence of owner Website publication configuration without weakening any
mandatory safety or evidence check.
Steps to reproduce: Clean-install Candidate 1, restore demo v56.0.6 and run
Verification from an owner runtime.
Screenshot / export / report attached:
`3DPIceland_FilamentDB_Verification_20260728_190337.txt`.
Resolution: v57.0.2.1 adds one exact public AppMeta marker in deterministic A/B
output and recognizes only that marker for the existing closed 12-check
Website-publication N/A allowlist. Candidate 1 was rejected.
Verification evidence: Candidate 2 clean-VM install, corrected v56.0.6.1
restore and restart pass 393/393 applicable checks, 12 canonical-data N/A,
zero mandatory N/A and zero FAIL in
`3DPIceland_FilamentDB_Verification_20260728_191916.txt`.
Stable/versioned installer, portable and demo HTTPS downloads independently
match the accepted Production bytes and SHA-256 values; remote backups are
retained under the governed application-release and public-demo backup roots.

Date: 2026-07-28
Area: Distribution / public demo download
Type: Workflow friction / Website idea
Severity: Important
Status: Solved
What happened: The accepted public demo package had no public stable download
route or exact user workflow for installing and later removing the demo.
Expected behavior: Publish the governed ZIP under versioned and stable download
routes and document guarded install/removal without manual SQLite replacement.
Steps to reproduce: Open the root README and public downloads area; no governed
demo link or lifecycle instructions are available.
Screenshot / export / report attached: Package manifest, checksums, deployment
plan and retained guarded transfer evidence.
Resolution: v57.0.1 adds an exact-one-ZIP guarded publisher, stable-last
activation, remote backup/rollback, HTTPS hash verification and README/Help
guidance using Restore SQLite Backup and matching pre-restore recovery.
Verification evidence: Debug/Release builds pass with zero warnings/errors.
Both 45,098-byte HTTPS routes independently reproduce SHA-256
`FA36B9D668292FC9567EB58AA27D7760321BC4325BD3DD43F2D6AD702738E640`;
remote backup is `/backups/public_demo/release_2026-07-28_183853_587`.
Owner Full Data Verification PASS completes runtime acceptance.

Date: 2026-07-28
Area: Materials / Base Materials delete confirmation
Type: Bug / UI consistency
Severity: Important
What happened: Standard Material `MessageBox YesNo` ignored Escape and disabled
the window close button. The first Base Material custom dialog fixed those
actions but looked materially different from the earlier Windows dialog.
Expected behavior: Material and Base Material use one consistent Windows-like
named warning. No is focused/default; Enter, Escape and window close cancel
without mutation; only explicit Yes deletes.
Steps to reproduce: Select a Material or unreferenced Base Material, click
Delete and compare Enter, Escape, close and appearance.
Screenshot / export / report attached: Owner runtime feedback on 2026-07-28.
Status: Solved
Resolution: v55.0.5.1 introduces one shared DPI-safe WPF safe-delete window for
both workflows with consistent sizing, vector warning icon and button layout.
Verification evidence: Owner accepts consistent appearance, Enter/No, Escape,
close and explicit Yes for both workflows. Final profile
`20260728135719-4ac326ab` passes 403/403 with exact state.

Date: 2026-07-28
Area: Verification Center / disposable Base Material delete probe
Type: Bug
Severity: Important
What happened: Owner Debug runtime showed `Verification Center Error` because
the temporary `probe.sqlite` remained locked while the in-process Verification
gate tried to recursively delete its probe folder.
Expected behavior: Verification Center opens without temporary-file cleanup
errors and never changes application files or canonical SQLite.
Steps to reproduce: Run the stale v55.0.5 Debug build, open Help and select
Verification Center.
Screenshot / export / report attached: Owner screenshot
`codex-clipboard-2637604c-7fe3-4a05-a96a-96300e2c47ea.png`.
Status: Solved
Resolution: Removed the disposable schema probe from the UI Verification path.
The actual bounded SQLite delete, referenced block and missing-ID no-op now run
inside the authorized disposable CRUD scenario. Debug was rebuilt after the
correction.
Verification evidence: Corrected Debug smoke `20260728132704-f9f0b973` opens
Verification Center, passes 402/402 and preserves exact database/business state.

Date: 2026-07-27
Area: AutomationRunner / Historical Verification and cleanup
Type: Bug
Severity: Important
What happened: The first v53.0.4.4 smoke failed because the accepted
v53.0.4.3 migration/recovery gate required the obsolete exact release version.
The first CRUD sweep also found one legacy Inventory row changing optional
landed-cost timestamps from SQL NULL to empty strings during broad persistence.
Expected behavior: Historical gates survive later versions and exact cleanup
returns every historical business value, including NULL/empty distinctions.
Steps to reproduce: Run v53.0.4.4 smoke and CRUD against the canonical seed.
Screenshot / export / report attached: Failed disposable profiles
`20260727230318-c0e9fb84` and `20260727231303-3e2eebb9`.
Status: Solved
Resolution: The historical gate now checks its retained contract while the
current gate owns exact release identity. Authorized CRUD cleanup validates
baseline Inventory identities and restores only three optional provenance
fields from the retained disposable baseline.
Verification evidence: Corrected CRUD `20260727231704-d31b92bc` and final smoke
`20260727232314-5c3961bc` pass exact state; smoke passes 392/392.

Date: 2026-07-27
Area: AutomationRunner / Guarded updater version fixture
Type: Bug
Severity: Important
What happened: The disposable updater fixture truncated v53.0.4.4 to v53.0.4,
so the helper correctly rejected the otherwise healthy acknowledgement.
Expected behavior: Synthetic previous/candidate/next versions preserve the
application's governed three- or four-part release identity.
Steps to reproduce: Run the updater scenario with a four-part FileVersion.
Screenshot / export / report attached: Failed disposable profile
`20260727231856-b4616b17`.
Status: Solved
Resolution: The runner now preserves four-part versions and increments the
revision component for synthetic adjacent releases.
Verification evidence: Corrected updater profile `20260727232117-b8100815`
commits 486 governed files and proves negative-health rollback.

Date: 2026-07-27
Area: Purchase Orders landed-cost snapshot lock
Type: Bug
Severity: Blocker
What happened: After calculation, Unit price still entered edit mode. The property changed before CellEditEnding cancelled the
commit, and leaving the cell could crash the WPF DataGrid edit transaction.
Expected behavior: Financial inputs on a calculated snapshot never enter edit mode and saved results remain unchanged.
Steps to reproduce: Calculate landed costs, type a new Unit price, then click outside the cell.
Screenshot / export / report attached: Owner screenshot `codex-clipboard-a7f75fec-e65a-46ce-9a4f-08d3e2200879.png`.
Status: Solved
Resolution: The candidate now blocks protected cells in DataGrid BeginningEdit, before PropertyChanged can mutate the row.
CellEditEnding remains a defensive fallback. Verification checks that Unit price is locked while Received remains editable.
Verification evidence: Debug/Release pass with 0 warnings/errors. Disposable Release smoke
`20260727214133-34260e52` passes 388/388 with exact database and business-state equality. Owner confirms Unit price remains locked,
the app does not crash and Full Data Verification passes.

Date: 2026-07-27
Area: v53.0.4 deterministic landed-cost acceptance
Type: Workflow improvement / Data integrity
Severity: Important
What happened: v53.0.3 deterministic Verification covers formulas and snapshots, but AutomationRunner smoke only inspects the
read-only Purchase Orders surface and does not execute the complete disposable default/override/restart/downstream lifecycle.
Expected behavior: Add bounded, exact-ID automation with safe evidence, restart checkpoints, migration/recovery comparison and exact
final state recovery while keeping smoke, owner data, Production, FTPS, updater and live ECB access isolated.
Steps to reproduce: Compare the v53.0.4 roadmap completion condition with current smoke/recovery scenario ownership.
Screenshot / export / report attached: None.
Status: Solved
Resolution: Recorded v53.0.4.1-v53.0.4.4 before code changes. v53.0.4.1 candidate adds exact default-off authorization,
Clean Readiness exclusion, secret-safe evidence shape, required real-control AutomationIds and read-only smoke discovery.
Verification evidence: Debug/Release app and runner builds pass with 0 warnings/errors; documentation/static and NuGet gates pass.
Disposable Release smoke `20260727215805-25c18520` passes 389/389 with exact database and business-state equality. Owner runtime
accepted the unchanged Purchase Orders surface and secret-safe diagnostics report
`3DPIceland_FilamentDB_System_Diagnostics_20260727_220416.txt`. v53.0.4.2 now adds the exact-ID disposable lifecycle with
restart, calculation, downstream and cleanup checkpoints. Initial final-state failures exposed broad legacy Inventory normalization
and historical Material derived-field refresh; exact-row persistence and baseline restoration corrected both. Release profile
`20260727222431-7383588f` passes 390/390 with exact normalized state. Owner runtime acceptance passes with the intentional
200-Material owner dataset, all identities beginning `MAT`, zero automation
residue and Full Data Verification PASS. v53.0.4.3 adds a read-only schema-v37
startup migration scenario and extends governed Excel recovery with exact
authorized v53 PO/Inventory mutation and whole-table preservation hashes.
Migration `20260727224150-47f14b19` and recovery
`20260727224236-eaa2bdb1` pass 391/391, source/final state and cleanup gates.
Owner runtime accepts 200 intentional Materials, unchanged Purchase
Orders/Inventory, zero automation residue and Full Data Verification 391/391.

Date: 2026-07-27
Area: Full Data Verification OpenAI timeout probe
Type: Bug
Severity: Blocker
What happened: Help -> Verify stopped under the Visual Studio debugger on the intentionally cancelled fake timeout request.
Expected behavior: Verification deterministically validates timeout/caller-cancellation classification without surfacing a
first-chance or user-unhandled TaskCanceledException.
Steps to reproduce: Run the Debug application under Visual Studio, open Help -> Verify and start Full Data Verification.
Screenshot / export / report attached: Owner screenshot `codex-clipboard-3e920095-2f8e-466b-994d-3b96f657dd2d.png`.
Status: Solved
Resolution: The production catch path and deterministic gate now share a pure cancellation classifier. Verification exercises
both classifications directly and no longer creates an intentionally cancelled task.
Verification evidence: Debug/Release pass with 0 warnings/errors. Disposable Release smoke
`20260727214133-34260e52` passes 388/388 with exact database and business-state equality. Owner confirms Help -> Verify completes
without debugger interruption and Full Data Verification passes.

Date: 2026-07-27
Area: Optional OpenAI Assistant integration
Type: Workflow improvement
Severity: Important
What happened: v52 needed a provider, payload, credential, retention and failure contract before external AI could be enabled.
Expected behavior: Local deterministic behavior remains available; secrets and canonical data stay governed and no data leaves
the computer without an exact preview and explicit action.
Steps to reproduce: Open Settings Manager and inspect Optional AI Assistant Provider.
Screenshot / export / report attached: None.
Status: Solved
Resolution: v52.0 research was owner accepted. v52.1 adds provider/model preferences, Windows Credential Manager ownership,
local-only diagnostics and a deterministic fake provider. An owner-reported Settings overlap was corrected by assigning the
provider panel its own Auto-sized Grid row and retaining the settings table in the sole expanding row. Live generation remains
excluded until v52.2. The v52.2 candidate adds exact preview, one-time consent, bounded cancellation and strict advisory output.
v52.3 research defines v52.3.1 acceptance/evidence, v52.3.2 deterministic failure/operational evidence and v52.3.3 bounded
owner evaluation/model decision. It identifies accidental raw preview persistence through Save Session as a boundary to govern.
The v52.3.2 candidate blocks that preview persistence, adds in-memory secret-safe operational evidence and deterministic
no-network success, failure, validation, timeout and evidence-allowlist coverage. Live Generate remains automation-blocked.
Verification evidence: v52.2 Release smoke `20260727162340-f07d3caa` passes 384/384 and Clean
`20260727162422-0de18a69` passes 284/284 plus 100 N/A. Both retain exact business-state equality. Owner preview, consent,
live advisory output, cancellation boundary and Full Data Verification acceptance pass on 2026-07-27. v52.3 retains
representative quality, latency, cost and operational-evidence evaluation.
The five-call evaluation completes with four PASS results and one strict cohort-count failure. Current project Usage reports
seven requests, 13,461 tokens and USD 0.37. The owner accepts `gpt-5.6-sol` as Provisional, not pinned; the read-only pilot and
local fallback remain available. Disposable smoke `20260727182043-ab84a0eb` passes 385/385 with exact state. Parent v52 is
complete.

## Tracked findings

Date: 2026-07-27
Area: In-application Help / Post-v50 maintenance
Type: Workflow friction / Documentation
Severity: Important
Status: Solved
What happened: The exhaustive v50 Help system is accepted, but later user-visible v51-v53 changes need one explicit reconciliation
pass, and repository governance did not yet require Help maintenance in every future increment.
Expected behavior: Keep Help, stable destinations, coverage registry/ledger and deterministic gates synchronized whenever supported
features, controls, fields, workflows, labels, validation, persistence or safety boundaries change.
Steps to reproduce: Compare accepted v51-v53 release changes with current Help topics, control/field ledger and coverage registry.
Screenshot / export / report attached: Owner governance review on 2026-07-27.
Resolution: v53.0.5 reconciles v51 runtime-profile ownership, v52 OpenAI boundaries, v53 landed-cost controls, Inventory provenance,
runtime surfaces and the exact 664-row XAML inventory. `AGENTS.md` requires same-increment Help maintenance for future user-visible
changes; cosmetic-only changes may record that no Help update is warranted.
Verification evidence: Debug/Release, Help, documentation and NuGet gates pass. Disposable smoke `20260727234421-49e325d1` passes
393/393 with exact database/business-state equality. Owner accepts search/navigation/readability and Full Data Verification PASS.

Date: 2026-07-27
Area: Runtime profiles / ownership and acceptance isolation
Type: Workflow friction / Data integrity
Severity: Important
Status: Solved
What happened: Normal owner runtime was implicit while disposable automation exposed a separate identity and isolated paths. There was
no common contract describing database/preferences/output ownership or effective Production, FTPS and update capabilities.
Expected behavior: Show an unambiguous governed runtime identity, preserve owner/disposable isolation and never weaken mandatory crash,
recovery, security, updater transaction, Verification or support evidence.
Steps to reproduce: Compare normal startup with a manifest-governed disposable tester startup and inspect System Diagnostics.
Screenshot / export / report attached: v51.0 research and disposable profile `20260727141117-d8438ac6`.
Resolution: v51 is split into v51.0 research plus v51.1-v51.4 identity, Clean Readiness, classification and final reconciliation.
v51.1.0 implements the shared Owner Production/Disposable Verification descriptor and visible diagnostics. Owner acceptance passes.
The v51.2 candidate adds seedless Clean Readiness, honest zero-data N/A classification and deterministic restart evidence;
owner acceptance remains pending, while mandatory classification and final reconciliation remain owned by v51.3-v51.4.
Verification evidence: Disposable smoke passes Full Data Verification 379/379 with exact state equality and hard
owner-database/Production/FTPS/update blocks. Owner evidence
`3DPIceland_FilamentDB_Verification_20260727_141617.txt` passes 379/379; Diagnostics
`3DPIceland_FilamentDB_System_Diagnostics_20260727_141552.txt` confirms the canonical database path and governed capabilities.
Clean profile `20260727143116-3138cb93` passes Application Readiness 280/280 applicable, zero FAIL and 100 N/A with exact
post-initialization state equality. Release profiles `20260727143346-142a5972` and `20260727143423-7961372f` pass clean
280/280 applicable plus 100 N/A and populated Full Data Verification 380/380 respectively, both with exact state equality.
Owner execution then exposed an anonymous transient WPF Help-menu popup race. The tester now retries only a fully anonymous
owned window for at most one second; identified or persistent dialogs remain blocked. Clean profiles
`20260727144106-e4feeaf6` and `20260727144139-15e29155` and populated profile `20260727144213-6e88b752` all pass.
Owner rerun completes without an unexpected dialog or PowerShell error; v51.2.0 is accepted and v51.3-v51.4 retain ownership
of the remaining classification and final reconciliation work.
The v51.3 candidate now exports explicit Mandatory/CanonicalDataDependent classification, keeps 16 evidence checks mandatory
and prevents free-form detail text from granting N/A. Clean profile `20260727145307-ec78b262` passes 281/281 applicable,
100 canonical-data N/A, zero mandatory N/A and zero mandatory-evidence failures; Release and owner acceptance remain pending.
Release profiles `20260727145433-df2701f4` and `20260727145601-56734ac4` pass Clean 281/281 plus 100 classified N/A and
populated 381/381 with zero N/A respectively; both retain exact business-state equality.
Owner confirms Owner Production and Clean runtime complete without visible errors; v51.3.0 is accepted and v51.4 owns final
profile reconciliation.
The v51.4 candidate closes a disposable read-isolation gap for owner update history, exposes credential/update/evidence/cleanup
ownership and retires two caller-free adapters. Clean profile `20260727150741-e127db3c` passes 282/282 plus 100 N/A with all
17 mandatory-evidence checks; Release scenario matrix and owner acceptance remain pending.
The final Release matrix passes Clean 282/282 plus 100 N/A and populated Smoke, Reports, CRUD, Recovery and Updater 382/382
with zero N/A and exact state; Updater proves both committed success and injected health-failure rollback.
Owner confirms Owner Production, Diagnostics, Verification 382/382 and Clean runtime without visible errors. v51.4.0 and
parent v51 are canonical and closed on 2026-07-27.

Date: 2026-07-27
Area: Help / Exhaustive control and editable-field coverage
Type: Workflow friction / Documentation
Severity: Important
Status: Solved
What happened: The accepted Help system explains every tab and major workflow, but it does not yet prove that every button, selector,
editable field and editable grid cell has precise guidance. The initial v50.4 audit found 135 XAML buttons, 31 menu items, 47 combo
boxes, 34 text boxes, 10 checkboxes, 31 grids and 356 XAML grid-column declarations, plus runtime-built and owner-drawn controls.
Expected behavior: Maintain a machine-checkable ledger that assigns every supported control/field a stable key, owner, exact Help
destination, behavior contract and evidence route. Complete with zero unexplained supported controls or editable fields.
Steps to reproduce: Compare `MainWindow.xaml`, runtime-created controls and custom-grid column builders with the current Help catalog.
Screenshot / export / report attached: `Docs/HELP_CONTROL_FIELD_LEDGER.md`.
Resolution: v50.4 is split into v50.4.0-v50.4.4 for ledger/gates, three bounded coverage groups and final zero-gap acceptance.
Verification evidence: The source-derived registry now tracks 645 exact XAML candidates: 222 in v50.4.1, 267 in v50.4.2 and 156
in v50.4.3. Six custom registries add 189 stable-key columns and eight runtime surfaces add 42 controls, for 876 candidates total.
The read-only drift gate passes; v50.4.0 is complete. The v50.4.1 candidate adds nine detailed topics, and disposable profile
`20260727125607-36733e81` passes Verification 375/375 with exact state equality. Owner runtime/readability acceptance passes;
v50.4.2 now adds four detailed engineering/analysis topics. Disposable profile `20260727131457-dee03dd4` passes Verification
376/376 with exact state equality. Owner v50.4.2 runtime/readability acceptance passes. The v50.4.3 candidate adds five output,
creator and runtime control topics; disposable profile `20260727132731-ea4e1d20` passes Verification 377/377 with exact state
equality. Owner v50.4.3 runtime/readability acceptance passes. The v50.4.4 candidate replaces the flat Help contents list with an
expandable category/topic tree; inventory has zero planned rows and disposable profile `20260727134442-edf82227` passes
Verification 378/378 with exact state equality. Final owner runtime/UI acceptance passes; v50.4.4 and parent v50 are complete.

Date: 2026-07-27
Area: Application menu / Navigation and command discovery
Type: Workflow friction / UI polish
Severity: Idea
Status: Solved
What happened: The application menu has not received a complete usefulness review against the current tab structure and workflows.
Useful navigation shortcuts may be missing, while retained items may now be redundant, stale or lower value.
Expected behavior: Inventory every menu item and destination, propose high-value commands that select and focus the correct tab, and
identify possible retirement candidates. Preserve guarded actions and do not remove accepted paths before impact and ownership review.
Steps to reproduce: Compare the current application menu with all supported top-level and nested tabs and common user workflows.
Screenshot / export / report attached: Owner workflow idea recorded on 2026-07-27.
Resolution: Planned with the final tab-order review in v59.0 — Application Navigation Finalization.
Verification evidence: Not implemented; roadmap scope, safety boundaries and completion condition are recorded.

Date: 2026-07-27
Area: GitHub README / Release and download discovery
Type: Workflow friction / Documentation
Severity: Important
Status: Solved
What happened: The repository README still advertises older v44.6.2/v43.8.9 release states and does not provide prominent stable
installer or portable download links.
Expected behavior: At each major-version closure, align README release/development text with the canonical Master Roadmap and retain
stable links for the downloads page, latest Windows installer and portable ZIP. README drift must block major-milestone closure.
Steps to reproduce: Open the GitHub repository README and compare its release identity with the current canonical roadmap.
Screenshot / export / report attached: Owner GitHub review on 2026-07-27.
Resolution: Solved as repository release governance on 2026-07-27. Root README now exposes stable installer, portable ZIP and
downloads-page routes, while AGENTS requires README release alignment before every major-version closure.
Verification evidence: The README links use the governed stable routes; release-documentation audit and diff checks pass.

Date: 2026-07-27
Area: Purchase Orders / Landed-cost currency
Type: Workflow improvement / Data integrity
Severity: Important
Status: Solved
What happened: Purchase Orders calculate landed cost in the transaction currency, but an owner may need governed landed-cost results
in a different operational currency such as ISK while retaining the original invoice currency and values.
Expected behavior: Add a Settings-owned Default Landed Cost Currency for new Purchase Orders plus a reviewed per-order override.
Snapshot the accepted conversion rate, date, source and provenance; never recalculate saved Purchase Orders, Inventory, Material cost
evidence, Usage history or saved Quotes. Preserve manual/offline operation and keep ECB optional.
Steps to reproduce: Create a Purchase Order with an EUR or USD invoice while operational landed-cost review is required in ISK.
Screenshot / export / report attached: Owner workflow idea recorded on 2026-07-27.
Resolution: Research accepted. Invoice inputs remain in invoice currency; landed results use a separate snapshotted currency and an
explicit `1 invoice-currency unit = X landed-cost-currency units` rate. v53.0.1-v53.0.4 are authoritative in the Master Roadmap.
Verification evidence: Disposable Release profile `20260727190229-ad32a552` passes 386/386 with exact database and business-state
hash equality. The v37 fixture migrates to v38 with exact monetary values and explicit 1:1 legacy metadata.
Owner acceptance: PASS on 2026-07-27. The owner profile contained two empty test orders and no populated historical order lines, so
visual monetary migration coverage was limited; the deterministic exact-value fixture owns populated monetary migration evidence.
v53.0.2 adds the new-Draft-only Settings default and confirmed override without monetary calculation. Disposable Release smoke
`20260727194840-c7ca50f8` passes 387/387 with exact state equality; owner runtime evidence remains pending.
Owner runtime review then found two candidate defects: the Settings value used a free-text editor, and startup migration restamped a
new v53 Draft as legacy. The correction uses a governed currency dropdown and limits legacy backfill to actual pre-v38 migration,
with a narrow repair for affected uncalculated Drafts. Replacement smoke `20260727200102-a16f943f` passes 387/387 with exact state;
owner runtime re-acceptance confirms dropdown, override, restart persistence and Full Data Verification PASS.
v53.0.3 candidate now converts only final landed outputs, stamps one calculation snapshot, separates downstream invoice/landed
evidence and updates saved-snapshot reports. Disposable smoke `20260727213614-403d89af` passes 388/388 with exact state after
the calculated-input, debugger-safe Verification and daily ECB/EUR fixes; owner runtime acceptance passes. v53.0.4.1-v53.0.4.3
then add exact authorization, lifecycle, migration and recovery evidence through 391/391. v53.0.4.4 reconciles secret-safe
Diagnostics, current Help and final acceptance. Final smoke and all disposable scenarios pass; owner diagnostics confirms 200
Materials, two uncalculated Purchase Orders, two legacy Inventory snapshots and no disposable authorization. Full Data Verification
392/392 and Help/readability are accepted. Mandatory v53.0.5 remains a separate parent-closure Help audit.

Date: 2026-07-27
Area: Automated acceptance / Application-wide navigation
Type: Workflow improvement / Test coverage
Severity: Important
Status: Solved
What happened: The disposable tester visibly visits several tabs, but it does not yet provide exact evidence that every supported
top-level tab and eligible nested tab was selected and rendered.
Expected behavior: Add an all-top-level-tab navigation sweep with exact visited/expected evidence and nested-tab checks where stable
AutomationIds exist. Require no crash, unexpected dialog or business-state change. Keep mutating workflows in separately authorized
CRUD, Reports, Recovery and Updater scenarios; Production and FTPS remain blocked.
Steps to reproduce: Run the smoke scenario and compare its navigation evidence with the authoritative tab inventory.
Screenshot / export / report attached: Owner tester observation recorded on 2026-07-27.
Resolution: Solved through v50.2.4 and retained by later disposable smoke coverage. The runner selects all 22 unique top-level tabs
by AutomationId plus Experimental nested/result views while preserving separate authorization for mutating scenarios.
Verification evidence: `AutomationRunner` records exact `Visited 22/22 unique top-level tabs by AutomationId`; accepted v50-v52
disposable runs retain exact business-state equality with Production and FTPS blocked.

Date: 2026-07-26
Area: Application release / signed update packaging
Type: Bug / Workflow friction
Severity: Blocker
Status: Solved
What happened: The v49 application uses schema v37, but signed-package generation and the independent package verifier remained
hard-coded to the historical schema-v29-only release contract.
Expected behavior: Build one governed v49 package that supports the public schema-v29 baseline through current schema v37, verifies
that exact range independently and publishes installer/portable before activating the update feed.
Steps to reproduce: Run the v49 signed packaging script and inspect the generated manifest or verify it against a schema-v37 profile.
Screenshot / export / report attached: Candidate package/feed/deployment plan and retained smoke/updater evidence profiles.
Resolution: A shared `BuildInfo` compatibility contract now owns minimum update schema v29 and current schema v37. `LocalDatabase`,
packaging, the independent verifier and a deterministic Full Data Verification gate consume the same values. The existing smoke
scenario owns runtime acceptance without authorizing Production or FTPS.
Verification evidence: Debug/Release and Candidate release gates pass. The independent verifier accepts the signed package at schema
v29 and v37. Portable smoke `20260726224510-b4ab7d08` passes 369/369 with exact business-state equality. Updater profile
`20260726224940-fcb3321a` commits six files with v49.0.0 health and proves exact rollback. Owner and clean-VM installer runtime
acceptance pass. Owner live publish completed in the governed order. Remote installer, portable and update ZIP downloads return HTTP
200 and match the accepted Production SHA-256 values; `latest.json` reports Production v49.0.0, schema v29-v37 and commit `5073da4`.

Date: 2026-07-26
Area: Purchase Orders / Exchange-rate provenance
Type: Workflow improvement / Data integrity
Severity: Important
Status: Solved
What happened: Purchasing used manual Settings rates and a legacy live-sync could rewrite persisted Purchase Order exchange rates.
Expected behavior: An optional official reference may prefill only a new Purchase Order. Saved purchases, Inventory lots, Materials
prices, Printers and quotes must retain their original values.
Steps to reproduce: Change a Purchasing currency rate in Settings, reload/reset Settings and inspect existing Purchase Orders.
Screenshot / export / report attached: Capture new-order ECB provenance, offline fallback and a retained older order during runtime.
Resolution: v48.2.0 candidate uses ECB SDMX observations, derives ISK through EUR, snapshots source/date/fetch provenance and removes
the saved-order Settings live-sync. A validated per-profile cache and manual governed fallback preserve offline operation.
Verification evidence: Debug/Release and documentation/roadmap gates pass. NuGet vulnerability scan is clean. Disposable smoke
`20260726203714-dcbdce20`, recovery `20260726203749-a8eac754` and refreshed-seed smoke `20260726210132-f34523ba` pass Full Data
Verification 364/364. Owner evidence `3DPIceland_FilamentDB_Verification_20260726_210009.txt` accepts live ECB, prefill and immutable
historical values.

Date: 2026-07-26
Area: Public website / Navigation / Printing Price Calculator
Type: Website idea / Workflow improvement
Severity: Important
Status: Solved
What happened: The standalone Printing Price Calculator at `https://iskort.is/3dp/price` is separate from the main
`https://iskort.is/3dp/` website experience.
Expected behavior: Add the existing price-calculator HTML to the main 3DPIceland website as a dedicated tab. Preserve the calculator
as-is, including its current fields, formulas, wording and client-side behavior. Do not populate it from the desktop Materials list,
SQLite, MaterialID, reports, website export data or any other application data. The change is website navigation/presentation only.
Steps to reproduce: Open the main public website and confirm that no dedicated tab currently exposes the standalone price calculator.
Screenshot / export / report attached: Capture desktop and narrow/mobile tab navigation during implementation.
Resolution: v57.0.3 candidate adds a scoped Printing Price Calculator portal tab without iframe or external runtime dependency.
The accepted fields, defaults, formulas, reset and quote-print behavior are retained. After owner approval, `/3dp/price` becomes a
small noindex redirect to `../index.html#calculator`, avoiding two calculator implementations that could drift apart.
Verification evidence: Debug/Release builds, documentation audit, NuGet scan and deterministic website contracts pass. Owner accepted
the calculator, totals, quote print and Verification; coordinated main/redirect/logo/favicon HTTPS publication now passes.

Date: 2026-07-28
Area: Public website / Printing Price Calculator / Materials and quote print
Type: Calculation and print correction
Severity: Critical
Status: Solved
What happened: Total Filament Required already represented the full job grams, but the generated first Materials row defaulted to Qty 4.
At 1,000 g, 5,000 ISK/kg and efficiency 1.1 this incorrectly produced 22,000 ISK instead of 5,500 ISK. Quote print also hid the
calculator page parent containing the quote document, so browser print preview was nearly empty.
Expected behavior: The calculated filament line defaults to Qty 1 because grams already represent the job total. Quote print shows the
prepared quote, line items and final price.
Steps to reproduce: Enter 1,000 g, 5,000 ISK/kg and efficiency 1.1, then inspect Materials and export the official quote.
Screenshot / export / report attached: Owner screenshot of an empty one-page browser print preview, 2026-07-28.
Resolution: v57.0.3 candidate defaults the calculated filament line to Qty 1 and uses visibility-scoped print CSS that preserves the
nested quote document while hiding the surrounding website.
Verification evidence: Deterministic Verification requires Qty 1 plus the nested quote visibility contract. Debug/Release and owner
runtime acceptance pass. The authorized live `/price` route now redirects to the accepted main-site calculator.

Date: 2026-07-28
Area: Public website / Canonical branding / Responsive navigation
Type: Visual correction
Severity: Important
Status: Solved
What happened: The first v57.0.4 Preview placed the Labs wordmark as a separate item inside the portal tabs. It consumed navigation
width, produced a horizontal scrollbar below the tabs and left the duplicate `3DPIceland Labs` text in the large page heading.
Expected behavior: Replace only the `3DPIceland Labs` portion of the top header with the canonical wordmark, retain
`Filament Testing Database` as the large adjacent title and let navigation tabs wrap without a horizontal scrollbar.
Steps to reproduce: Generate Preview, inspect the top header and narrow the browser while watching the portal navigation.
Screenshot / export / report attached: Owner Preview screenshot, 2026-07-28.
Resolution: v57.0.4 moves the wordmark into a responsive top-header grid, retains the large database title and changes portal
navigation from horizontal overflow to wrapping rows.
Verification evidence: Deterministic branding contract requires the header classes, relative logo/favicon routes and wrapping CSS.
Debug/Release, regenerated Preview, owner desktop/narrow visual acceptance and final Verification 407/407 pass.

Date: 2026-07-28
Area: Verification Center / Canonical website branding
Type: False-negative Verification correction
Severity: Blocking
Status: Solved
What happened: Owner accepted the regenerated logo/favicon Preview, but Full Data Verification passed 406/407 because the branding
probe applied the renderer to synthetic portal HTML that contained no top-level website header. It then required header classes that
could only be produced when the actual canonical header exists.
Expected behavior: The deterministic probe supplies the accepted SQLite-template header shape and validates replacement, relative
assets, favicon and wrapping navigation independently from owner data.
Steps to reproduce: Run Full Data Verification on the accepted v57.0.4 Preview build.
Screenshot / export / report attached: `3DPIceland_FilamentDB_Verification_20260728_202853.txt`; branding only FAIL.
Resolution: The probe now includes `3DPIceland Labs – Filament Testing Database` in a bounded synthetic header before applying the
branding renderer. Production output and accepted visual layout are unchanged.
Verification evidence: Calculator contract, Debug/Release and owner Full Data Verification 407/407 pass.

Date: 2026-07-28
Area: Public website / Printing Price Calculator / Uptime percentage
Type: Calculation correction
Severity: Critical
Status: Solved
What happened: Estimated Uptime was labelled `%` but defaulted to 0.5 and was consumed as a fraction. Entering 100 for 100% made
available hours one hundred times too large, nearly removing printer capital and maintenance from Machine Time.
Expected behavior: Enter percentages as 0-100; 50 means 50% and 100 means 100%. Machine Time includes amortized printer/upfront/lifetime
maintenance, electricity and the configured buffer. Warn when requested print hours exceed available lifetime uptime.
Steps to reproduce: Use printer cost 300,000, maintenance 10,000/year, life 2 years, uptime 100%, 17,088 print hours and buffer 1.3.
Screenshot / export / report attached: Owner runtime calculation report, 2026-07-28.
Resolution: v57.0.3 calculator contract now normalizes uptime by 100, defaults to 50, bounds the input to 0-100 and exposes a capacity
warning. No Materials, SQLite or report data is consumed.
Verification evidence: Deterministic Verification requires percent normalization, available lifetime hours, default 50 and warning.
Debug/Release, owner calculation acceptance and Full Data Verification 407/407 pass.

Date: 2026-07-28
Area: Verification Center / v57 release identity
Type: False-negative Verification correction
Severity: Blocking
Status: Solved
What happened: Final v57.0.5 Verification passed release identity, calculator and branding checks but failed 406/407 because an older
release gate still required the exact v57.0.2 version, code and title.
Expected behavior: The current release gate requires v57.0.5 identity while retaining default-No delete safety, owner/disposable cleanup
ownership and generic assembly alignment.
Steps to reproduce: Run Full Data Verification under v57.0.5 WEBSITE-CALCULATOR-BRANDING.
Screenshot / export / report attached: `3DPIceland_FilamentDB_Verification_20260728_204413.txt`; stale identity gate only FAIL.
Resolution: Rename and align the gate with v57.0.5; no delete, cleanup, installer/demo compatibility or assembly safety condition is
weakened.
Verification evidence: Final report `3DPIceland_FilamentDB_Verification_20260728_204858.txt` confirms 407/407 PASS.

Date: 2026-07-26
Area: Recommendation Detail / MSRP context
Type: UI polish
Severity: Important
Status: Solved
What happened: A selected $11.86/kg Elegoo PLA scoped the Recommendation Engine
to PLA, whose selected winner was a $91.74/kg ProtoPasta PLA. The generic
`Public MSRP reference` label made the valid winner price appear to be an
incorrect conversion of the selected Materials row.
Expected behavior: Recommendation MSRP must identify the exact recommended
product and MaterialID while preserving the selected Materials row as scope.
Steps to reproduce: Select an Elegoo PLA Material, then inspect the first
Recommendation Detail MSRP when a ProtoPasta PLA recommendation is selected.
Screenshot / export / report attached: Owner runtime report, 2026-07-26.
Resolution: Solved in v48.0.2 — 2026-07-26. MSRP now prefixes the recommendation name
and MaterialID. No price, score, scope or canonical data is changed.
Verification evidence: Seed evidence confirms Elegoo MAT0190-MAT0193 at
$11.86/kg and ProtoPasta MAT0065-MAT0069 at $91.74/kg. Owner runtime acceptance
and Full Data Verification 354/354 PASS.

Date: 2026-07-26
Area: Recommendation Engine / pricing and alternatives
Type: Bug
Severity: Blocker
Status: Solved
What happened: Selecting an ASA Material updated Selected Material Intelligence,
but Recommendation, alternatives and hidden gems retained the previous PLA
Base Material filter and therefore showed PLA MSRP/value context.
Expected behavior: Selecting a Material must rebuild all recommendation
surfaces from that Material's exact canonical Base Material family.
Steps to reproduce: Leave Recommendation Base Material on PLA, select an ASA
row in Materials and inspect Recommendation Detail and alternatives.
Screenshot / export / report attached: `codex-clipboard-12ff8fe9-9f51-4c82-b20c-984d6d39663d.png`.
Resolution: Solved in v48.0.2 — 2026-07-26. Exact canonical Base Material scope synchronizes
before rebuilding Recommendation, alternatives, hidden gems, MSRP and value
index. Unsupported legacy names are not fuzzy remapped.
Verification evidence: Deterministic selected-material scope gate added;
owner runtime acceptance and Full Data Verification 354/354 PASS.

Date: 2026-07-26
Area: Materials filters / AI Assistant collections / Video planning
Type: Workflow improvement
Severity: Important
Status: Solved
What happened: Materials filters allow only one selected value per category. Building a comparison collection therefore requires
repeated filtering or manual collection work, while the website already supports multi-select Base Material, Variant / Finish,
Reinforcement, Color, Manufacturer and Product Line filters.
Expected behavior: Materials should support visible multi-select values for those six filter categories. Values inside one category
use OR behavior; categories combine with AND behavior. For example, three selected Manufacturers plus PLA plus Blue and Yellow
should show PLA records from any selected Manufacturer where Color is Blue or Yellow. AI Assistant scope and saved collections must
consume the exact visible canonical MaterialID set. Filtering must never change Materials, relationships, measurements or reports.
The UI must show active selections, resulting MaterialID count, per-filter/global Clear actions and require no Ctrl/Shift modifiers.
Steps to reproduce: Open Materials and attempt to select three Manufacturers, PLA and two Colors for one comparison collection.
Only one value can currently be selected in each desktop filter.
Screenshot / export / report attached: Capture current Materials single-select and website multi-select behavior during implementation.
Resolution: Planned in v54.0 — Materials Scope and Collection Workflow. Preserve the accepted single-select path until replacement
runtime acceptance. Saved collections remain explicit MaterialID snapshots and must not change silently when filters change.
Candidate v54.0.5 implements six no-modifier facets, OR-within/AND-between semantics, profile-local restart persistence, selected
value/count/Clear presentation, exact visible-scope SHA-256 evidence, immutable collection membership diagnostics and honest AI
processing limits. Website/publication scope remains independent. Owner runtime acceptance passed and v54.0.6 retired the hidden
scalar controls and obsolete matcher.
Verification evidence: Required future coverage includes OR within each filter, AND across filters, Clear restoring accepted scope,
exact visible MaterialID count and AI preview/save parity. Deterministic scope tests belong in the existing safe acceptance boundary;
multi-select dropdown layout, wrapping and usability remain owner-manual acceptance.
Candidate evidence: Debug/Release and static/security/documentation gates pass. Disposable Release smoke
`20260728124401-5f1d1f99` passes Full Data Verification 400/400 with exact database/business-state equality. Owner runtime
interaction, wrapping, restart and local preview acceptance passed; v54.0.6 is canonical and parent v54 is complete.

Date: 2026-07-26
Area: Base Materials / Settings / Materials
Type: Workflow improvement
Severity: Important
Status: Solved
Resolution: v45.2.0 moves the 23-field canonical Base Material Catalog into a dedicated tab with Add, Duplicate, guarded Delete,
direct Fast-grid editing and independent layout reset. Exact-name rename propagation preserves existing text consumers. Schema and
canonical `BaseMaterialId` mapping remain gated behind workspace runtime acceptance in v45.2.1.
Verification evidence: Debug/Release pass with zero warnings/errors. Disposable profile `20260726094502-312196e2` passes tab
navigation, Base Material create/edit/duplicate/delete persistence, Full Data Verification 346/346 and exact business-state cleanup.
Owner runtime accepted CRUD and Verification 346/346 but exposed an immediate-restart blank-tab race. Tab selection now synchronously
creates both Settings/Base Materials Fast views before returning. Owner retest
`3DPIceland_FilamentDB_Verification_20260726_100133.txt` passes 346/346 and accepts immediate first-open rendering.
What happened: Canonical Manufacturers had a dedicated workspace while canonical Base Materials were mixed into Settings Manager.
Expected behavior: Domain catalogs should have consistent discoverable ownership, complete CRUD and tester coverage before identity
relationships are introduced.
Steps to reproduce: Open Settings Manager and locate Base Material Catalog beneath measurement/calculation settings.
Screenshot / export / report attached: Not required.
`codex-clipboard-478305e4-d56a-4fa3-816a-9f7da612508e.png` and
`3DPIceland_FilamentDB_Verification_20260726_095539.txt`.

Follow-up: v45.2.1 candidate adds schema v33 `BaseMaterialId` identity while retaining the exact text snapshot for compatibility.
Existing rows remain unlinked until explicit dropdown selection or default-No exact-name binding. Linked rename propagates by ID;
referenced catalog delete is blocked; reports, website, filters and printing-profile resolution consume the current canonical name.
Disposable profile `20260726101917-5bc56749` passes the linked lifecycle and Full Data Verification 347/347.
Follow-up acceptance: Owner final evidence `3DPIceland_FilamentDB_Verification_20260726_105027.txt` passes 347/347 with zero
unlinked Materials and accepts immediate dropdown refresh.

Owner Verification `3DPIceland_FilamentDB_Verification_20260726_102301.txt` reports 203 explicit unlinked Base Material rows and
passes the v45.2.1 identity contract. The single FAIL is the older v45.2 workspace gate depending on prior lazy tab activation.
The gate now verifies the stable workspace contract without requiring a prior visit, and Materials shows an always-visible
`203 unlinked` / `All linked` status beside the Base Material filter.

Owner runtime then found that renaming a newly added Base Material persisted correctly but left the Materials dropdown on its
original `New material` snapshot until restart. The catalog edit path now refreshes the shared observable choices immediately and
synchronizes the Fast Materials view. Disposable CRUD exercises the same rename handler and rejects stale old/new choices.

Date: 2026-07-25
Area: Verification / Materials filters
Type: Bug
Severity: Important
Status: Solved
Resolution: The v44.7.7 ownership gate now compares canonical active rows with active filtered MaterialIDs. A filter reset to All no
longer lets an archived MaterialID create a false 201-versus-202 mismatch and cascade four unrelated retirement-gate failures.
Verification evidence: Owner evidence reported 341/345 with exactly four chained v44.7.7 failures. Disposable owner-fix run
`20260725224654-16f3c455` and owner evidence `3DPIceland_FilamentDB_Verification_20260725_224919.txt` pass 345/345.
What happened: Exact Manufacturer binding rebuilt the filter list and selected All, exposing one archived ID only in the raw ID set.
Expected behavior: Canonical active report/filter ownership must compare the same active scope on both sides.
Steps to reproduce: Keep one archived Material, reset Manufacturer filter to All, then run Full Data Verification.
Screenshot / export / report attached: `3DPIceland_FilamentDB_Verification_20260725_224310.txt`.

Date: 2026-07-25
Area: Manufacturers
Type: Bug
Severity: Important
Status: Solved
Resolution: Manufacturer Name uses a cell edit buffer and updates on focus loss. The name-based rename guard was superseded by
nullable Material `ManufacturerId`: canonical rename is allowed and propagates to linked Materials, while hard delete is blocked.
New or recognizable `New Manufacturer` rows remain explicit drafts and placeholders avoid catalog and Material name collisions.
Verification evidence: Debug/Release pass with zero warnings/errors. Disposable CRUD and Full Data Verification pass 345/345 with
unchanged seed and business-state hashes. Owner accepted direct naming, later rename propagation and Full Data Verification 345/345.
What happened: Immediate per-character binding let the in-use rename guard write the stored text back into the active editor.
Expected behavior: A newly added Manufacturer must accept its initial proper name while established in-use names remain protected.
Steps to reproduce: Ensure a Material uses `New Manufacturer`, add a Manufacturer, then type in its Name cell.
Screenshot / export / report attached: `codex-clipboard-e6ad4625-65e3-4880-a8ad-d6f9cf772122.png`.

Date: 2026-07-25
Area: Distribution / public demo dataset
Type: Workflow improvement / Data issue
Severity: Idea
Status: Solved
Resolution: Solved in v56.0.6 — Governed Public Demo Dataset. Built a versioned, distributable SQLite demo backup from an allowlist of
roughly 30–40 owner-measured materials, with stable pseudonymous Manufacturer, Product Line, Marketing Name and demo MaterialID
identities. Preserve real measurement values and useful same-manufacturer/base-material relationships, but remove or replace private
purchasing, inventory, notes, URLs, local paths, credentials, deployment and production identity. Keep this public demo seed separate
from the internal canonical acceptance seed. Require deterministic regeneration, provenance, privacy/static scans, SQLite integrity,
Full Data Verification, runtime/report review, package SHA-256 and explicit guarded publication under
`https://www.iskort.is/3dp/downloads/`. Production and FTPS remain default-No until separately authorized.
Verification evidence: v56 research and the fictional-identity/real-measurement strategy are owner-approved.
`Docs/PUBLIC_DEMO_DATASET_CONTRACT.md` records the candidate threat model, source boundary, allowlist, denylist, acceptance and
publication contract. v56.0.1 is complete: the owner accepted the exact private 36-Material allowlist and re-identification risk.
Schema v38 integrity, relationships, uniqueness and measurement coverage pass; the gitignored registry is hash-bound.
v56.0.2 is complete: the owner accepted the isolated read-only inspector, deterministic privacy-safe manifest and fail-closed
source, private-registry and schema-drift evidence. v56.0.3 through v56.0.6 are complete.
v56.0.3 fictionalizes Manufacturer, Product Line, Marketing Name, Color and Variant while retaining approved generic Base
Material and CF/GF taxonomy. Public spec/private parity, identity uniqueness and deterministic graph hashes pass; no SQLite is built.
Two final Release validations are byte-identical with logical manifest SHA-256
`82C62984153513CD78797FDBE71839BE4B4F4E805F009234105CF5A3EE30F621`; Debug/Release and governed static gates pass.
The owner accepted the fictional identity/transformation contract.
v56.0.4 fresh-builds two byte-identical schema-v38 SQLite files.
SQLite SHA-256 is
`8D68E8B8E03F5714565D1258EB9D79D9BBDD851D4E8A5FB8B4A10ED024C082D9`;
all 25 table ledgers, exact counts, integrity/FK/privacy and 17 empty-domain
gates pass. Runtime and Full Data Verification remain owned by v56.0.5.
v56.0.4.1 corrects runtime-found coverage drift: 9 records are Fully tested
and 27 are Partially tested under current valid-result rules. Corrected A/B
SQLite SHA-256 is
`1BA01065925F2BD8FC8ABF62B5A9C5D710D7403E9EDFC0DD5744BBC604EF61E6`.
v56.0.5 disposable profile `20260728164408-ba9afb2f` passes 391/391
applicable Public Demo Verification checks with 12 explicit owner
website-publication N/A checks, zero mandatory N/A, exact restart/filter/AI/
collection scope, local Material Summary export and business-state equality.
Independent PDF review then found customer-facing MaterialID disclosure and
split rows. v56.0.5.1 owns removal from visible HTML/PDF, retained internal
metadata traceability, row pagination guards and renewed disposable/PDF
acceptance before v56.0.6 owner review resumes.
Correction profile `20260728173706-49ad4986` passes 391/391 applicable checks.
The seven-page PDF uses compact customer identity, four-row CSS-grid ledgers
and repeating `@page` margins; extracted text and every rendered page pass.
Owner review then exposed 34 red/invalid Impact rows. All 567 samples above
100 follow one deterministic legacy decimal pattern: 557 end in zero and 10
end in five. v56.0.5.2 owns bounded normalization of only those invalid values,
fresh deterministic A/B generation and renewed runtime/Verification acceptance.
Candidate profile `20260728180126-bb11afd8` passes 391/391 applicable checks,
contains 718/718 Impact samples inside 0-100 and reports 36 Fully tested rows.
The owner accepted all corrected Impact rows and confirmed the rest of the
demo remained normal. The governed local four-file ZIP is byte-deterministic;
ZIP SHA-256 is
`B38F3173CBAE4E95D4EF589D460BC820AEB9040FF9CC59EB0E7CA4077EE191F4`.
Production, FTPS and stable-route activation remain separately unauthorized.
What happened: Trial users or evaluators currently lack a safe populated database that demonstrates Materials, measurements, filters,
rankings, reports and same-family/manufacturer comparisons without exposing the owner database or commercial product identities.
Expected behavior: Offer an optional downloadable demo SQLite backup with real 3DPIceland measurement evidence under fictional,
clearly disclosed identities, enough grouped coverage to demonstrate the application while preventing owner/private-data disclosure.
Steps to reproduce: Install the application on a clean profile and look for a governed populated demo dataset beside the installer
downloads.
Screenshot / export / report attached: Not required.

Date: 2026-07-25
Area: Disposable automation / evidence retention
Type: Workflow friction
Severity: Minor
Status: Solved
Resolution: v55.0.6 adds runner-owned dry-run inventory and a separately hash-reviewed apply path. It preserves the latest
valid PASS per scenario plus every FAIL, aborted, malformed or pinned dependency. Full-tree hashes, preflight validation, reparse
rejection and same-root quarantine protect the exact reviewed removals. Real apply remains owner-acceptance-gated.
Verification evidence: Synthetic classifier, dry-run byte preservation, plan SHA-256 and bounded apply pass. Final pinned dry-run
`0EC0B8565A655B116F8B50F295975B6E13B64E0035B93150DF39A1FE4F04748B` retains four required profiles and removes zero.
What happened: Completed disposable tester profiles can remain below the temporary automation root after their results have been
reviewed and are no longer needed, causing old databases, reports, screenshots and transaction evidence to accumulate.
Expected behavior: Codex should perform a deliberate post-review cleanup when obsolete disposable evidence is no longer needed,
while preserving the latest accepted scenario evidence and all material failure or current-increment evidence.
Steps to reproduce: Run disposable tester scenarios repeatedly and inspect `%TEMP%\3DPIceland-Automation` after Codex has reviewed
the retained results.
Screenshot / export / report attached: Not required.

Date: 2026-07-25
Area: Materials / Inventory / Manufacturers
Type: Bug
Severity: Important
Status: Solved
Resolution: Material Add/Duplicate/Delete now refreshes the live Manufacturer choices immediately. After a successful Material
delete save, Inventory is reloaded from canonical SQLite so cascaded spool deletion is reflected before Verification runs. No value
is remapped and no owner database is touched by automation.
Verification evidence: The reported owner run failed only Inventory engine at 344/345 because one deleted MaterialID still existed
in the in-memory spool collection. The dropdown screenshot showed the same stale-projection class. Debug/Release pass; disposable
CRUD and Full Data Verification pass 345/345, including dropdown removal checks and zero inventory orphans.
Owner runtime accepted the refreshed projections and Full Data Verification 345/345.
What happened: A deleted Material's unique Manufacturer remained in the open dropdown until a later grid rebuild. Verification in
the same session also counted an Inventory row already removed from SQLite by the Material delete cascade.
Expected behavior: All live projections must refresh immediately after canonical Material changes.
Steps to reproduce: Add a Material with a unique Manufacturer, delete it, inspect the dropdown, then run Full Data Verification
without restarting the app.
Screenshot / export / report attached: `codex-clipboard-067a4d7d-1faa-4b07-948b-7088409a9c29.png` and
`3DPIceland_FilamentDB_Verification_20260725_214218.txt`.

Date: 2026-07-25
Area: Materials / Manufacturers
Type: Workflow improvement
Severity: Idea
Status: Solved
Resolution: Owner review showed name binding could not propagate later spelling corrections. The v45.1 candidate now stores nullable
`ManufacturerId`, resolves linked catalog names, retains exact unlinked legacy text and blocks deletion of referenced catalog rows.
Migration never auto-links existing Materials. A previewed default-No action binds only explicit unique exact-name matches.
Materials also provides a counted `Unlinked manufacturers (n)` filter for explicit one-by-one assignment of the remaining values.
Owner runtime then found that selecting the same visible name produced no text delta, so the Fast grid skipped the save. Manufacturer
dropdown confirmation now explicitly commits canonical identity even when the legacy text already equals the selected catalog name.
After owner binding reached zero unlinked rows, the review filter and exact-binding button became daily-workflow noise. Both are now
hidden at zero and return only if migration, import or recovery legitimately reintroduces a supported unlinked value.
Verification evidence: Disposable CRUD passes legacy fallback, ID persistence, rename propagation, delete guard and cleanup;
Full Data Verification passes 345/345 with equal baseline/final business-state hashes. Owner exact binding linked 182 rows, retained
20 unmatched rows without remapping, then linked the final rows after catalog creation. Owner evidence
`3DPIceland_FilamentDB_Verification_20260725_231731.txt` passes 345/345 with zero unlinked Materials.
What happened: Materials currently accepts free-text Manufacturer values even though Manufacturers has a governed canonical catalog.
Expected behavior: Materials should offer a dropdown sourced from canonical Manufacturers while preserving supported legacy/unmapped values.
Steps to reproduce: Open Materials and edit Manufacturer on an existing or new row.
Screenshot / export / report attached: Not required.

Date: 2026-07-25
Area: Guarded updater acceptance
Type: Workflow improvement
Severity: Important
Status: Solved
Resolution: Solved in version v44.7.18 — 2026-07-25. Explicit disposable Stage 5 authorization adds real helper
commit/health and forced rollback evidence. General updates, Production, FTPS, owner application and owner database remain blocked.
Verification evidence: Disposable Stage 5 and Full Data Verification pass 344/344. Exact v44.7.18 health acknowledgement,
`Committed`/`RolledBack` states, 54 restored SHA-256 values and equal baseline/final database business-state hashes are retained.
Owner accepted normal startup, owner-data behavior and Full Data Verification 344/344.
What happened: Existing updater contract probes were isolated in-process fixtures and did not exercise real helper/app health startup.
Expected behavior: Stage 5 should prove commit and rollback through the real helper without reaching owner or production paths.
Steps to reproduce: Run the explicit disposable `updater` scenario against an approved seed copy and Debug helper.
Screenshot / export / report attached: Evidence below `%TEMP%\3DPIceland-Automation\<ProfileId>\evidence`.

Date: 2026-07-25
Area: Disposable backup and recovery acceptance
Type: Workflow improvement
Severity: Important
Status: Solved
Resolution: Solved in version v44.7.17 — 2026-07-25. Explicit disposable recovery authorization covers verified `.bak`/legacy `.sqlite` discovery,
governed Excel export/restore and pre/post SQLite restore evidence. General restore UI remains blocked in automation.
Verification evidence: Disposable Stage 4 and Full Data Verification pass 343/343 with equal baseline/final business-state hashes.
Workbook and backup artifacts retain bytes and SHA-256 evidence. Owner accepted normal runtime recovery and Verification 343/343.
What happened: Backup and restore paths were runtime-proven manually but lacked repeatable isolated end-to-end evidence.
Expected behavior: Stage 4 should verify backups and governed Excel recovery without owner paths or silent SQLite restore.
Steps to reproduce: Run the explicit disposable `recovery` scenario against an approved seed copy.
Screenshot / export / report attached: Evidence below `%TEMP%\3DPIceland-Automation\<ProfileId>\evidence`.

Date: 2026-07-25
Area: Disposable CRUD acceptance
Type: Workflow improvement
Severity: Important
Status: Solved
Resolution: Solved in version v44.7.16 — 2026-07-25. Exact-scenario authorization covers one generated disposable MaterialID and exercises canonical
create/save, restart/edit/save, restart/delete/save and final restart/absence checks without automating Fast-grid cells.
Verification evidence: Disposable CRUD runtime PASS. Per-action SQLite snapshots retain full logical and timestamp-normalized
business-state hashes. Disposable and owner Full Data Verification pass 342/342; owner accepted normal CRUD persistence and cleanup.
What happened: Stage 1 and Stage 2 could verify read-only behavior and local reports, but persistence and cleanup were still manual.
Expected behavior: A disposable scenario should prove canonical Materials CRUD persistence without owner paths or broad deletion access.
Steps to reproduce: Run the explicit disposable `crud` scenario against an approved seed copy.
Screenshot / export / report attached: Evidence below `%TEMP%\3DPIceland-Automation\<ProfileId>\evidence`.

Date: 2026-07-25
Area: Automated report acceptance
Type: Workflow improvement
Severity: Important
Status: Solved
Resolution: Solved in version v44.7.15 — 2026-07-25. Stage 2 adds an explicitly authorized disposable report scenario over the canonical six-family public
report package workflow. Automated output is confined below the disposable profile; catalog routes, HTML/PDF/JSON artifacts, bytes and
SHA-256 values are retained as machine-readable evidence. Visual HTML/PDF review remains manual.
Verification evidence: Disposable report runtime passes Verification 341/341 with 211 catalog entries, 639 verified artifacts and
matching logical SQLite hashes. Debug/Release, static/security, documentation, roadmap-line and NuGet vulnerability gates pass.
Owner Full Data Verification passed 341/341.
What happened: The accepted report workflow had strong in-app verification but no safe runner completion contract or retained
catalog-driven artifact inventory.
Expected behavior: Repeatable local report acceptance should prove report completion and artifact integrity without owner paths,
Production/FTPS access, private-field expansion or false claims of visual acceptance.
Steps to reproduce: Run the explicit disposable `reports` scenario against an approved seed copy.
Screenshot / export / report attached: Disposable Stage 2 PASS on 2026-07-25 with Verification 341/341, 211 catalog entries,
639 catalog/root artifacts and identical before/after logical SQLite hashes. Representative Material Summary and Material
Engineering PDFs were rendered and visually reviewed; a discovered final-page summary-table clip was corrected with deterministic
20-row presentation tables. Owner review then found screen-HTML right-edge clipping and excessive narrow-window wrapping; the screen
table now preserves readable columns with normal word wrapping and horizontal narrow-window scrolling while retaining the PDF contract.
Owner accepted the landscape PDF and responsive HTML behavior; owner Full Data Verification passes 341/341.

Date: 2026-07-25
Area: Runtime acceptance automation
Type: Workflow friction
Severity: Important
Status: Solved
Resolution: Solved in version v44.7.14 — 2026-07-25. Stage 1 adds an exact-build Windows UI Automation runner only after implementing a visibly disposable startup
profile. The profile is confined below a dedicated temporary root, separates all writable paths, requires an explicit seed copy and
blocks Production, FTPS, updates, restore and Material deletion. Stable navigation/Verification IDs and TXT/JSON evidence are added.
Verification evidence: Disposable Stage 1 runner PASS; isolated and owner-profile Full Data Verification 340/340 PASS; canonical
logical SQLite hash matched before/after; Debug and Release solution builds pass with zero warnings/errors; static, security,
documentation and NuGet vulnerability gates PASS.
What happened: Runtime acceptance depended on repeated manual navigation and evidence capture, while the app had no Automation IDs,
disposable startup profile or machine-readable Verification export.
Expected behavior: Safe repeatable smoke evidence should reduce manual repetition without selecting owner data, crossing application
process boundaries or replacing owner runtime/visual acceptance.
Steps to reproduce: Inspect current startup path ownership, UI Automation tree and Verification export workflow.
Screenshot / export / report attached: Disposable runner JSON/TXT, Verification JSON/TXT, owned-window screenshots and consistent
before/after SQLite snapshots retained below the dedicated temporary automation root.

Date: 2026-07-25
Area: Website template import / public HTML / PDF host
Type: Bug / Website idea
Severity: Important
Status: Solved
Resolution: v44.7.13 makes new executable-template import and immediate activation an explicit default-No trust decision. Imports are
limited to 5 MiB and require a structurally replaceable `const DATA` object. Existing stored templates remain unchanged. The hidden
WebView2 PDF host blocks unexpected navigation, popups and permissions while retaining required local scripts/assets during printing.
Verification evidence: Debug/Release, analyzers, documentation, roadmap and NuGet advisory gates passed. Owner runtime testing
accepted import cancellation/confirmation, website Preview, the complete Public Report Package and sampled HTML/PDF output. Full Data
Verification passed 339/339 with malicious-input encoding and hardened WebView2 host checks both PASS.
What happened: Arbitrary imported HTML was stored and immediately activated after only a loose DATA text check, and the hidden
WebView2 PDF host had no explicit navigation, popup or permission policy.
Expected behavior: Executable template trust must be explicit and default-No, with bounded structural validation and a local PDF host
that cannot unexpectedly leave its canonical document or grant browser capabilities.
Steps to reproduce: Import an HTML template from Website Export, or inspect the hidden WebView2 host used by report PDF generation.
Screenshot / export / report attached: `3DPIceland_FilamentDB_Verification_20260725_175112.txt` and owner runtime review.

Date: 2026-07-25
Area: Repository-wide maintainability and retired artifacts
Type: Workflow friction
Severity: Important
Status: Solved
Resolution: v44.7.12 removes only caller-free code and files after tracing C#, XAML, project resources, reflection, serialization,
migration, recovery, updater, diagnostics, export and Verification ownership. The candidate retires the unused hand-built PDF layer,
legacy workbook write helpers, obsolete website-template/UI residue and one unowned icon-source PNG. Canonical HTML/WebView2 PDF,
typed report/certificate rendering, Excel disaster recovery, SQLite restore/migration and active branding assets remain.
Verification evidence: Debug/Release compile probes passed after removing 2,200+ lines and the 1.28 MB unused source asset. Initial owner
testing passed Verification and normal Engineering Package export, then exposed a Public Report Package fingerprint query against
retired `TensileResults`. The corrected package rebuilt every public report family without visible errors, sampled HTML passed visual
review and final Full Data Verification passed.
What happened: Feature retirement had left caller-free renderer, import, handler, compatibility-UI and asset residue in the current
tree, increasing maintenance cost and making future cleanup harder to reason about.
Expected behavior: Each accepted retirement should leave only active runtime or documented compatibility owners, with obsolete code
and files removed in the same or immediately following bounded increment.
Steps to reproduce: Trace the retired PDF, legacy workbook-write, template-file and workflow-handler symbols across the complete
solution and compare project resource ownership for all tracked image assets.
Screenshot / export / report attached: Repository ownership audit and compiler/analyzer evidence for v44.7.12.

Date: 2026-07-25
Area: Materials / manual backup and save ownership
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: Retained `Manual Backup` as an explicit SQLite evidence action, not a Save command. Normal Fast Materials edits auto-save;
tooltip and exhaustive Help state the distinction. Backup and Recovery Center owns the wider catalog and restore workflow.
Verification evidence: v44.7.17 accepted manual-backup discovery and guarded recovery; v50.4 accepted exhaustive Materials and
runtime-window Help, and disposable Verification retained exact business-state recovery.
What happened: The Materials toolbar labels the action `Manual Backup`, but it validates and saves Materials and creates a database
backup before replacement; a separate canonical SQLite backup action already exists in Backup and Recovery Center.
Expected behavior: The label and placement should state the action's real ownership without duplicating backup UI or weakening the
existing save-time protection.
Steps to reproduce: Edit a Material, inspect the enabled `Manual Backup` action and compare it with File > Backup and Recovery Center.
Screenshot / export / report attached: User feedback and source-path review on 2026-07-25.

Date: 2026-07-24
Area: Materials / Add Material and Duplicate
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: v44.7.10 reapplies the active Fast-grid header sort when
Add, Duplicate, filters or reload change the visible source set. Selection,
column layout and canonical SQLite order remain separate.
Verification evidence: Debug/Release, static/documentation and read-only NuGet
advisory gates passed. Owner Add, Duplicate, sorting, close/restart and
save-safe placeholder tests passed. Full Data Verification passed 335/335.
What happened: With Material ID sorted descending, a newly created next MaterialID does not appear at the expected top position, so
the user must apply the sort again to find it.
Expected behavior: If `MAT0206` is the current highest MaterialID and Add or Duplicate creates `MAT0207`, the new selected row should
appear first under descending Material ID sorting while the existing sort order remains active.
Steps to reproduce: Sort Materials by Material ID descending, then use Add Material or Duplicate and inspect the new row position.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Settings Manager / Base Material Catalog
Type: Workflow friction / Data issue
Severity: Important
Status: Solved
Resolution: v55.0.6 adds a warning naming the selected unreferenced Base Material, Yes/No buttons and explicit default No
before any mutation. Every non-Yes response preserves SQLite, selection and calculations. Explicit Yes uses one transactional
BaseMaterialID delete; referenced rows remain blocked before confirmation. Owner runtime acceptance passed.
Verification evidence: Deterministic prompt/default/cancellation contract and disposable SQLite unreferenced/referenced/missing-ID
probe pass. Owner Enter/Escape/close/Yes/restart acceptance and final Full Data Verification pass.
What happened: `Delete Selected Base Material` can delete the selected catalog entry without a warning or default-No confirmation.
Expected behavior: Show a clear warning naming the selected Base Material and perform no mutation unless the user explicitly selects
`Yes`; pressing Enter, Escape or closing the dialog must follow the safe default-No path.
Steps to reproduce: Open Settings Manager, select a Base Material and click `Delete Selected Base Material`.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Tools menu / workflow column reset
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: Removed the retired `Reset Current Workflow Columns to Default...` command and its uncalled handler family.
Materials receives a local `Reset Columns` action; existing Fast reset buttons, saved layouts and default-No confirmation remain.
Verification evidence: Owner runtime testing accepted Tools-menu removal, local Fast resets, saved layouts and restart behavior.
Full Data Verification passed 326/326.
What happened: The Tools menu still exposes a legacy workflow-column reset command after the measurement legacy grids were retired.
Expected behavior: Column reset should be owned only by each accepted Fast workflow through its local `Reset Columns` action.
Steps to reproduce: Open the Tools menu and observe `Reset Current Workflow Columns to Default...`.
Screenshot / export / report attached: `codex-clipboard-e19b2469-4bcd-4ab8-b6b3-a4c7c3762791.png`.

Date: 2026-07-24
Area: Settings Manager command clarity and column reset naming
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: v44.7.11 renames Load to `Reload Saved Settings`, adds a
default-No discard confirmation, and renames `Reset Fast Columns` to `Reset
Columns`. Research found that built-in restore also replaced the in-memory Base
Material Catalog despite its stated General-only scope; the candidate now
replaces and saves only General Settings while preserving Deployment Settings
and the canonical Base Material Catalog. Duplicate generic Fast-grid footer
reloads are hidden because they refreshed only in-memory rows; the explicit
toolbar command owns SQLite reload.
Verification evidence: Owner persisted-value reload, default-No cancellation,
built-in restore, Base Material/Deployment preservation, restart, column reset
and visual tests passed. Full Data Verification passed 336/336.
What happened: `Load Settings` and `Restore Built-in Defaults` appear to perform the same action by replacing entered values with
defaults. The `Reset Fast Columns` label also exposes an implementation detail rather than the user action.
Expected behavior: Loading should clearly reload the intended saved canonical values, restoring defaults should be a distinct
default-No destructive replacement, and the layout action should be labeled `Reset Columns`.
Steps to reproduce: Enter non-default Settings values, use each command separately and compare the resulting values; inspect the
Settings Manager toolbar label for the column reset action.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Materials / Tensile / Impact / Stiffness default row ordering
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: v44.7.10 adds one presentation-only natural MaterialID
comparer to all four accepted Fast grids. It does not renumber or reorder
canonical SQLite rows.
Verification evidence: Debug/Release, static/documentation and read-only NuGet
advisory gates passed. Owner default/explicit sorting, retained selection,
top-left startup viewport and restart tests passed. Full Data Verification
passed 335/335.
What happened: Default views in Materials, Tensile, Impact and Stiffness are not consistently presented in ascending MaterialID order.
Expected behavior: On an unsorted/default view, show the lowest numeric MaterialID first and the newest/highest MaterialID last in all
four workflows.
Steps to reproduce: Open each of Materials, Tensile, Impact and Stiffness without applying a manual sort and compare the row order.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Application-wide user help and workflow guidance
Type: Workflow friction / UI polish
Severity: Idea
Status: Solved
Resolution: v50.0.0 accepted the information architecture and one reusable non-modal Help window with offline search and F1
contextual navigation. The parent milestone continues through recorded v50.1 start-to-finish guidance, v50.2 per-tab reference and
contextual links, and v50.3 troubleshooting/recovery/publishing safety plus final acceptance.
v50.1.0 is now the implementation candidate for the complete owner workflow from Purchasing, Materials and Inventory through
measurement entry, validation, analysis, reports, Website Preview and guarded Production publishing. Code-traced guidance must not
redefine calculations, data ownership, public allowlists, FTPS confirmation or recovery boundaries.
Verification evidence: Static inventory and implementation complete. Deterministic smoke covers open/search/navigation and passes
with exact business-state recovery. Full Data Verification 370/370 owns catalog uniqueness/content/mapping; owner visual/runtime
acceptance confirmed the workflow on 2026-07-27. Owner screenshot identified horizontal clipping in the contents list; the candidate
now stretches item content, disables horizontal scrolling and wraps category, title and summary text. Owner accepted the final visual
retest on 2026-07-27; v50.0.0 is canonical foundation. v50.1 requires new deterministic and owner workflow acceptance.
The first v50.1 owner review found no in-body search highlight/jump and unnatural wrapping caused by source line breaks. The candidate
now highlights all matches, scrolls the first match into view and normalizes only source-only line breaks within paragraphs.
A follow-up found that WPF did not always raise `SelectionChanged` when filtering retained the same first topic, leaving body highlight
stale until a manual topic switch. Search and Clear now render the selected first result directly, independent of that event.
Owner accepted the complete v50.1 workflow, natural wrapping, search highlight/jump and immediate Search/Clear refresh on 2026-07-27.
The parent item remains In progress for recorded v50.2 per-tab reference/context links and v50.3 safety/final acceptance.
Before v50.2 implementation, the exhaustive work is recorded as v50.2.0 inventory, v50.2.1 materials/purchasing/configuration,
v50.2.2 testing/analysis, v50.2.3 outputs/tools and v50.2.4 contextual-link/coverage acceptance.
v50.2.0 inventory is complete in `Docs/HELP_COVERAGE_MATRIX.md`: 22 top-level tabs, 16 nested tabs and all identified menu/runtime
surfaces have owners, stable Help destinations, save-mode contracts, Automation gaps and repair increments. v50.2.1 is current.
The v50.2.1 candidate implements nine stable searchable references for Materials, Manufacturers, Purchase Orders, Inventory, Usage,
Printers, Print Job Quotes, Base Materials and Settings Manager. It documents commands, fields, filters, validation, persistence,
destructive boundaries and handoffs while preserving prospective inputs and governed saved history. Owner acceptance is pending.
Disposable smoke `20260727101000-dddd3159` passes the expanded Help searches, Full Data Verification 370/370 and exact
logical/business-state equality. Production, FTPS, updates and owner database selection remained blocked.
Owner review corrected Quote terminology: saved rows are read-only in this view and do not auto-recalculate, but explicit deletion is
supported, so current Help no longer labels quote history immutable. Historical release records remain unchanged.
Correction profile `20260727101828-1f53f4e1` passes revised Help search, Full Data Verification 370/370 and exact state equality.
Owner follow-up found that summary/category/title and hidden-keyword matches could filter topics while only body text was highlighted.
The renderer now highlights every visible searchable field and excludes hidden keyword-only hits. Start-to-finish step 6 uses
`retained purchase evidence` instead of `immutable purchase snapshot`. Profile `20260727102551-3b35ebbc` passes 370/370.
Owner accepted all v50.2.1 reference content, terminology, wrapping and header/body highlighting on 2026-07-27. v50.2.1 is canonical;
the parent item remains In progress for recorded v50.2.2-v50.2.4 and v50.3 work.
The v50.2.2 candidate adds 22 stable references for native measurements, Experimental Testing, Material Detail and global analysis.
It corrects stale Notes/import/future-export UI guidance and adds deterministic scope/save/lifecycle coverage. Owner acceptance is
pending.
Disposable profile `20260727103712-93b32f7b` passes the new Help searches, Full Data Verification 371/371 and exact state equality.
Owner review found that the visible `Experimental Results Dashboard/Table` titles did not match natural searches without `Results`.
The titles and Series overview now expose direct Experimental Dashboard, Table and Charts names. Title matches rank before overview
body matches. Profile `20260727104534-dc519c51` passes Full Data Verification 371/371 and exact state equality.
Owner review also removed `immutable` from current Usage Help. Accepted events remain preserved and corrections still append exact
reversal/replacement rows; only the user-facing terminology changed.
Owner review found Help > Documentation opening Materials reference because it reused F1 context. The menu now opens the canonical
whole-system Start-to-finish overview; F1 remains contextual. Profile `20260727105308-cc3ce0b1` passes 371/371 and exact state equality.
Owner accepted the complete v50.2.2 content, terminology, search/discovery and Help entry behavior on 2026-07-27. v50.2.2 is canonical;
the parent item remains In progress for v50.2.3-v50.2.4 and v50.3.
The v50.2.3 candidate adds 34 stable Reports, Website, AI, YouTube and packaged Help/support destinations. It corrects stale Reports
UI guidance and adds deterministic local-versus-publish/clipboard/calendar safety contracts. Owner acceptance is pending.
Profile `20260727110155-90afdea3` passes the expanded Help searches, Full Data Verification 372/372 and exact state equality.
Owner review found that the initial v50.2.3 leaf topics were structural placeholders rather than the promised detailed reference.
Every destination is now expanded with exact controls, scope, writes, failure behavior, evidence and handoff boundaries. Verification
now enforces substantive multi-paragraph content, so the earlier 372/372 evidence is superseded pending a new disposable run.
The first expanded-content smoke then failed because calendar wording changed the accepted visible search phrase from `do not create`
to `does not create`. The text now retains the stable owner-searchable phrase; this was a Help search-contract correction only.
The next Full Verification run exposed a line-ending assumption in the paragraph-depth gate: LF source strings were checked with the
Windows CRLF separator. The gate now normalizes CRLF to LF before requiring a blank-line paragraph boundary.
Disposable profile `20260727111924-0056eda1` then passes the detailed Help contract, Full Data Verification 372/372 and exact
logical/business-state equality. Owner runtime/content acceptance was the remaining gate.
Owner accepted the complete v50.2.3 output/tool Help content on 2026-07-27. v50.2.3 is canonical; the parent Help milestone remains
In progress at v50.2.4 for contextual entry points and full coverage acceptance.
The v50.2.4 candidate implements the expanded authoritative roadmap scope: 22 top-level and 16 nested stable tab IDs, nested-aware F1
and Help for Current View, four missing Experimental destinations, Tools/Help menu references and retirement of the disabled Website
Preview dead-end. Profile `20260727113500-3bc427ca` passes 373/373 and exact logical/business-state equality; owner acceptance was the
remaining gate.
Owner accepted v50.2.4 contextual navigation, nested Help behavior and Website menu replacement on 2026-07-27. v50.2.4 and the parent
v50.2 milestone are canonical and complete; the v50 parent remains In progress at authoritative increment v50.3.
Owner review clarified that the accepted tab-level Help is not yet exhaustive at individual button, input and editable-grid-cell
level. Authoritative v50.4 now requires a complete UI/control/field ledger, exact Help ownership, drift detection and zero unexplained
supported controls or editable fields. v50.3 remains next; parent v50 cannot close until v50.4 receives final owner acceptance.
The v50.3.0 candidate adds 18 safety/recovery/troubleshooting references and read-only runtime inspection for Recovery, Diagnostics and
Verification. Initial smoke found the Diagnostics window ID on Verification; corrected profile `20260727120029-14c0556f` passes
374/374 and exact logical/business-state equality. Owner safety/recovery acceptance was the remaining gate.
Owner accepted v50.3.0 safety/recovery Help and runtime inspection on 2026-07-27. Owner also requested future runtime checklists to
state exact location, action, expected result and forbidden adjacent actions instead of assuming whether a step belongs in Help or the
actual workflow. This handoff format is now a repository rule. v50.4 becomes authoritative; parent v50 remains In progress.
What happened: The application has many connected workflows, but no single structured user guide explains what every tab does or how
data should move through the platform from purchase entry to measurements, reports and website publication.
Expected behavior: Provide a well-organized user-help system with a start-to-finish workflow, per-tab reference, field-entry guidance,
prerequisites, validation meanings, safe publishing steps, recovery boundaries and links from relevant UI contexts.
Steps to reproduce: Follow a new user from recording a purchase through creating/linking a material, entering measurements, reviewing
results, generating reports and publishing the website; guidance is currently distributed or implicit.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Tensile / Impact / Stiffness Measurements
Type: UI polish
Severity: Minor
Status: Solved
Resolution: Solved in version v44.5.4 — 2026-07-24.
Verification evidence: Runtime screenshots and Full Data Verification PASS.
What happened: The instructional sentence at the top of each native measurement workspace is duplicated inside the XAML text itself.
Expected behavior: Each Tensile, Impact and Stiffness instruction appears once, followed by its calculation/read-only field guidance.
Steps to reproduce: Open each of the three measurement tabs and read the instruction directly below the page heading.
Screenshot / export / report attached: v44.5.3 runtime screenshots `codex-clipboard-ca877ee8-7358-4c0d-85d4-28a91426487f.png`, `codex-clipboard-967228f4-533e-4771-a97e-ae142b6fb715.png` and `codex-clipboard-050971f8-3092-468c-8e14-4f1fafee3f63.png`.

Date: 2026-07-24
Area: Verification Center / System Diagnostics exports
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: Solved in version v44.5.2 — 2026-07-24.
Verification evidence: Distinct Verification and System Diagnostics filenames were runtime accepted; Full Data Verification PASS.
What happened: Verification Center and System Diagnostics both export files using the generic `3DPIceland_FilamentDB_Diagnostics_YYYYMMDD_HHMMSS.txt` filename, even though they contain different report types. This makes attached files easy to confuse.
Expected behavior: Use terminology consistently across the window title, export action, document header and filename. Verification Center should export `3DPIceland_FilamentDB_Verification_YYYYMMDD_HHMMSS.txt`; System Diagnostics should export `3DPIceland_FilamentDB_System_Diagnostics_YYYYMMDD_HHMMSS.txt`.
Steps to reproduce: Export one file from Verification Center and one from System Diagnostics, then compare their filenames.
Screenshot / export / report attached: Observed in the v44.5.1 runtime diagnostics/verification handoff.

Date: 2026-07-24
Area: Build history / release documentation governance
Type: Workflow friction / Data issue
Severity: Minor
Status: Solved
Resolution: Solved in version v44.6.1 — 2026-07-24.
Verification evidence: Canonical release-documentation audit PASS and Full Data Verification 311/311 PASS.
What happened: There is no canonical total for completed builds or documented releases. `BUILD_HISTORY.md`, `CHANGELOG.md`, `RELEASES.md` and `MILESTONES.md` contain different numbers of version headings, duplicate version identifiers and different historical coverage, so the total depends on which document is counted.
Expected behavior: Define exactly what counts as a build, candidate, runtime-accepted release and canonical release. Generate one canonical version/release index from governed metadata, show totals by status, and validate the documentation files against it. The check should flag duplicate version headings, missing entries, conflicting titles/statuses and version-order anomalies without rewriting historical records silently.
Steps to reproduce: Count unique version headings and duplicates independently in `BUILD_HISTORY.md`, `CHANGELOG.md`, `RELEASES.md` and `MILESTONES.md`; the totals do not match.
Screenshot / export / report attached: Repository audit on 2026-07-24 found 289 unique versions in Build History, 317 in Changelog, 47 in Releases and 57 in Milestones.

Date: 2026-07-24
Area: Tensile / Impact / Stiffness / Experimental measurements and Material Details
Type: Workflow friction / Data issue
Severity: Idea
Status: Solved
Resolution: Solved in version v44.6.2 — 2026-07-24.
Verification evidence: Schema v31, native/Experimental date persistence, Material Detail dates, column-reorder editing and Full Data Verification 312/312 PASS.
What happened: Measurement-entry rows do not record when each test was performed, so the application has little historical context for measurement age, test sequence or retesting.
Expected behavior: Store a canonical measured date per test type and per experimental run rather than one shared Material date. Auto-fill the date when the first measurement value is entered, but do not silently replace it during later corrections or delete it when values are cleared. Allow intentional manual editing for measurements entered after the fact. Keep `Measured date` explicitly separate from record `Last edited` metadata, display a clear local date while storing an unambiguous ISO date in SQLite, and preserve it through governed Excel disaster-recovery export/restore.
Steps to reproduce: Enter Tensile, Impact or Stiffness measurements for a MaterialID and inspect the measurement rows and Material Details; no canonical measured date or test history is available.
Screenshot / export / report attached: User feedback on 2026-07-24.
Potential downstream use: Material Details test timeline; first/last measured dates and testing span; measurement freshness; oldest/newest filters; retest queues; report provenance; comparisons over time. Public reports must expose dates only through an explicit reviewed allowlist.

Date: 2026-07-24
Area: Public website / Material engineering reports
Type: Website idea / Data issue
Severity: Idea
Status: Solved
Resolution: Solved in version v44.7.9 — 2026-07-25. Typed, allowlisted Material
Engineering and Test Session reports publish canonical ISO dates or exact
`Not recorded`; internal edit timestamps remain excluded.
Verification evidence: Owner HTML/PDF review confirmed recorded/missing date
behavior; responsive Reports actions and wrapped tooltips passed runtime
review; Full Data Verification 334/334 PASS.
What happened: Canonical measured dates are now available in SQLite and Material Details, but public website users cannot yet see when the displayed engineering measurements were performed.
Expected behavior: Public material pages and applicable reports show clear per-test measured dates sourced from the same canonical SQLite metadata, with an unambiguous display format and honest missing-data behavior.
Steps to reproduce: Open a published material page after v44.6.2 and compare its engineering information with Material Detail > General > Test Information in the desktop application.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Bug / Feedback Log governance
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: Solved as documentation-only roadmap increment v44.7.0 — 2026-07-24. Every retained item has lifecycle metadata, and the authoritative future sequence is maintained in Master Roadmap.
Verification evidence: Owner review accepted; status coverage 49/49 PASS, 136-column roadmap formatting PASS and canonical release-documentation audit PASS.
What happened: Feedback items do not have a consistent status or resolution record, making it difficult to distinguish open work from completed work or identify which accepted release solved an item.
Expected behavior: Preserve every original feedback description and append structured lifecycle metadata. New items should use `Status: Open`; supported states should include `Open`, `In progress`, `Partially solved`, `Solved`, `Deferred`, `Duplicate` and `Not planned`. After runtime acceptance, a solved item should receive a simple canonical resolution such as `Solved in version v44.5.1 — 2026-07-24`, optionally followed by `Verification evidence: Full Data Verification 302/302 PASS`. Use ISO dates and only canonical runtime-accepted versions; never mark a failed or unaccepted candidate as solved. Partially completed items remain open or explicitly `Partially solved`.
Steps to reproduce: Review older entries in `BUG_FEEDBACK_LOG.md`; completed and outstanding items cannot be distinguished consistently from the entry itself.
Screenshot / export / report attached: User feedback on 2026-07-24.



Date: Historical feedback (date not recorded)
Area: Pricing / analytics
Type: Feature improvement
Severity: Important
Status: Solved
What happened: USD/kg and price-per-engineering-result analytics
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in v48.0.2. The non-persisted governed value index shows Overall engineering score per canonical MSRP USD/kg with
both inputs, exact comparison scope and honest missing-data behavior; landed cost is never substituted.
Verification evidence: Owner accepted exact scope, recommendation-price identity and alternatives; Full Data Verification 354/354.

Date: Historical feedback (date not recorded)
Area: Tensile / Impact data entry
Type: Bug / accessibility
Severity: Important
Status: Solved
What happened: Unreadable values below 10 on yellow measurement cells
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v39.1.2 — native Tensile and Impact sample cells use dark text on yellow backgrounds.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: AI Assistant / Materials scope
Type: Workflow issue
Severity: Important
Status: Solved
What happened: AI collection scope and workflow clarity
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved through v47 and v52. Collections use exact visible/active MaterialID snapshots; local deterministic behavior
remains available, and optional OpenAI generation requires exact preview, one-time consent and read-only advisory output.
Verification evidence: v47 passes Full Data Verification 350/350. Parent v52 is owner accepted with Provisional model disposition;
disposable smoke passes 385/385 with exact state equality.

Date: Historical feedback (date not recorded)
Area: Category Rankings
Type: Workflow improvement
Severity: Normal
Status: Solved
What happened: Configurable rows per ranking group
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v44.7.1 — Category Rankings provides safe 5/10/50/100/All scope controls.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Application window / measurement entry
Type: Usability improvement
Severity: Normal
Status: Solved
What happened: Larger useful default application window
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v39.1.2 — default window size increased to 1700 × 960 while saved user geometry remains authoritative.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Tensile / Impact data entry
Type: Workflow improvement
Severity: Normal
Status: Solved
What happened: Test Notes placement beside entered samples
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v37.1.4 — Test Notes were repositioned before computed measurement results.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Experimental Testing
Type: Feature improvement
Severity: Important
Status: Solved
What happened: Governed experimental measurement comparisons
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: The requested duplicate-material/dynamic-column concept was superseded by canonical MaterialID-linked Experimental Series and Runs with dedicated measurement editors and governed website/report integration. Additional experimental presentation ideas remain open only when backed by a concrete consumer.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Follow-up: v49.0.0 candidate addresses concrete owner-visible integrity gaps on
top of the accepted Series/Run model. Active Runs define the normal desktop and
website comparison; inactive history remains explicitly reviewable. Duplicate
Run resets lifecycle evidence, publication readiness is visible and persistence
is atomic and guarded. No Materials are duplicated and schema remains v37.
Final Verification and owner runtime acceptance remain pending.

Owner follow-up: Initial v49.0 runtime passed Verification 367/367 but showed
that Series Active-only started unchecked, the Results-only inactive-history
scope looked like a Runs-grid filter and Website readiness appeared only after
leaving the checkbox cell. The corrected candidate starts Active-only checked,
labels the Results/Dashboard/Charts boundary and prompts immediately with
default No. Correction smoke and owner retest pass 368/368. v49.0.0 is
canonical and runtime accepted.

Date: Historical feedback (date not recorded)
Area: Branding / application icon / splash
Type: Visual improvement
Severity: Normal
Status: Solved
What happened: Transparent application icon and branded header/splash
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v46.0.0 — 2026-07-26. The supplied transparent original is used as the Windows and
splash icon source, and separates the supplied Labs wordmark into the main
header on a white rounded card. Reports retain their accepted JPG contract.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Header / Materials counts
Type: Bug
Severity: Important
Status: Solved
What happened: Stale compiled material and column counts
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: The obsolete 176-row compiled fallback and stale cache terminology were retired; live SQLite counts are canonical.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Installer / data storage
Type: Deployment improvement
Severity: Important
Status: Solved
What happened: Installer with governed user-owned SQLite location
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v43.7.0 — governed per-user installer and portable deployment keep SQLite/backups/configuration outside application files and support a user-owned storage location.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Workflow grids
Type: Usability improvement
Severity: Normal
Status: Solved
What happened: Smarter default column widths and persisted sizing
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved through v37.1.0 smart first-run widths plus v37.0.0 machine-local width persistence.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Materials editing / stability
Type: Bug / crash
Severity: Critical
Status: Not planned
What happened: Historical long-running-session crash while editing Variant
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Closed as stale and non-reproducible after many subsequent accepted builds without recurrence. Do not spend additional
time attempting to reconstruct this historical one-day Variant-edit crash. If the behavior returns, create a new finding with current
build identity, exact steps, stack trace and runtime diagnostics.
Verification evidence: No recurrence has been observed across the subsequent accepted build and Verification history. This is an
explicit owner disposition, not evidence that one historical fix was identified.

Date: Historical feedback (date not recorded)
Area: Testing / printing / material usage
Type: Data-model idea
Severity: Normal
Status: Solved
What happened: Track testing hours, print hours, material usage and sample count
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved through v48.0.3-v48.0.7. Private append-only Usage Events record explicit grams and duration, optionally reconcile
same-Material Inventory, and correct history through reversal/replacement; specimen counts remain measurement evidence.
Verification evidence: Owner accepted Usage capture, correction and private analytics; Full Data Verification passes 361/361.

Date: Historical feedback (date not recorded)
Area: Website template / SQLite
Type: Workflow improvement
Severity: Important
Status: Solved
What happened: Persist latest accepted website template in SQLite
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v40.10.0 — the active validated website template is stored and versioned in SQLite and reused by Preview/Production export.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Measurements / dates
Type: Duplicate
Severity: Normal
Status: Duplicate
What happened: Record measurement entry dates
Expected behavior: Keep one canonical owner for the requirement and avoid parallel implementations.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Duplicate of the structured canonical measurement-date item above; solved in v44.6.2.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Materials filters
Type: Bug
Severity: Important
Status: Solved
What happened: Not Tested filter returned all materials when none matched
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v36.0 — selecting `Not Tested` with no matching material returns an empty result.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Status bar / diagnostics
Type: Bug
Severity: Normal
Status: Solved
What happened: Stale Engine material and cell counters
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Obsolete engine/cache counters were removed or replaced by canonical SQLite live-data status and current per-module counts.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Materials / Manufacturer SKU
Type: Field-retirement review
Severity: Normal
Status: Solved
What happened: Audit possible retirement of Manufacturer SKU
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved as a retain decision in v45.0. The audit found active purchasing, Material Detail, export and recovery ownership,
so Manufacturer SKU remains supported and is not eligible for retirement.
Verification evidence: The v45.0 caller audit and later exhaustive v50 Help/control ledger retain the field and its supported owners.

Date: Historical feedback (date not recorded)
Area: Website / terminology
Type: Content improvement
Severity: Important
Status: Solved
What happened: Use Tensile and Layer Adhesion terminology
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v40.12.2 — public/chart terminology uses Tensile Strength for flat orientation and Layer Adhesion Strength for upright orientation.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Material Detail
Type: Bug
Severity: Important
Status: Solved
What happened: Tested-state indicators did not refresh
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v36.1.0 — selected Material Detail and Dashboard Insights refresh after Tensile, Impact and Stiffness edits.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Materials / video metadata
Type: Field-retirement review
Severity: Normal
Status: Solved
What happened: Audit possible retirement of video thumbnail name
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved as a compatibility-retain decision in v45.0. Thumbnail Filename retains export/recovery compatibility ownership;
no active website/report asset reader was found, so it remains supported without inventing a new runtime consumer.
Verification evidence: The v45.0 caller audit and exhaustive v50 Help/control ledger document the retained compatibility boundary.

Date: Historical feedback (date not recorded)
Area: Dashboard Insights
Type: Bug
Severity: Important
Status: Solved
What happened: Dashboard did not refresh as materials were added
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved through v36.0.1/v36.1.0 and later canonical projection work; Dashboard Insights now uses current canonical material and measurement projections.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Status bar / diagnostics
Type: Bug
Severity: Normal
Status: Solved
What happened: Stale bottom status-bar material count
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Duplicate stale engine/cache status was retired in favor of canonical SQLite live-data counts.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Materials validation
Type: Documentation / workflow issue
Severity: Normal
Status: Solved
What happened: Explain Materials row validation status
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v44.7.2 — Materials explains the five required row-identity fields and the exact meaning of OK.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Material report
Type: Bug
Severity: Important
Status: Solved
What happened: Stale material total and duplicate report logo
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v36.0 and subsequent canonical report projections; stale total and duplicate logo were removed.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Manufacturer HTML export
Type: Bug
Severity: Important
Status: Solved
What happened: Stale Materials in database count
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Manufacturer/public report surfaces now consume canonical active/material visibility projections rather than the historical 176-row dataset.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Materials purchasing
Type: Feature improvement
Severity: Normal
Status: Solved
What happened: Purchased From field in Materials workflow
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Purchased From is present in canonical Material Detail/Material workflow data.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Inventory / pricing
Type: Feature improvement
Severity: Important
Status: Solved
What happened: Cost per kg from cost and spool weight
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v38.2.10 for inventory calculations; later landed-cost releases add governed purchasing context.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Purchasing / grid editing
Type: Workflow issue
Severity: Normal
Status: Solved
What happened: First-click currency and cell editing
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved by the shared first-click editor workflow and immediate ComboBox opening in v37.0.x.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Purchasing / currency
Type: Workflow improvement
Severity: Normal
Status: Solved
What happened: Default landed-cost currency to ISK
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Purchasing currency settings and automatic ISK exchange-rate fill were delivered in v38.4.1.4–v38.4.1.6.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Purchasing / landed cost
Type: Feature improvement
Severity: Important
Status: Solved
What happened: Calculate landed cost from transaction exchange rate
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Governed Purchase Order exchange rates and landed-cost allocation replaced the manual-only workflow.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Materials filters / performance
Type: Performance bug
Severity: Important
Status: Solved
What happened: Slow filter response
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v44.4.1 — measured viewport-only Materials rendering is the accepted daily-use default with native fallback retained.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Startup / performance
Type: Performance improvement
Severity: Important
Status: Solved
What happened: Profile and optimize application startup
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved through v41.8.0 instrumentation, v41.8.1 callback coalescing and v44.4.1 measured Materials responsiveness.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Backup / recovery
Type: Naming improvement
Severity: Normal
Status: Solved
What happened: Human-readable purpose-specific backup filenames
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in version v44.7.8 — readable purpose-specific `.bak` names retain canonical SQLite contents, dual
`.bak`/`.sqlite` discovery, explicit restore and updater/recovery evidence. Existing `.sqlite` files are not renamed, moved or added
to the new automatic cleanup set.
Verification evidence: See the cited accepted release and canonical build/Verification documentation.

Date: Historical feedback (date not recorded)
Area: Fast workflow grids / layout
Type: Workflow improvement
Severity: Important
Status: Solved
What happened: User-reorderable persisted workflow columns
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in v44.7.7. The accepted Fast workflow grids provide stable
field-identity layout persistence, user column reordering and Reset Columns,
and the legacy Materials, Tensile, Impact, Stiffness and Settings grids were
retired after staged runtime acceptance.
Verification evidence: Staged owner runtime acceptance completed with Full Data Verification 332/332 PASS.

Date: Historical feedback (date not recorded)
Area: Base Materials / Materials relationship
Type: Data-model / workflow improvement
Severity: Important
Status: Solved
What happened: Canonical BaseMaterialID selection and legacy preservation
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in v45.2.1. Schema v33 adds canonical `BaseMaterialId`,
exact-name binding, rename/delete guards, live dropdown refresh and downstream
canonical resolution while retaining the text snapshot for compatibility.
Verification evidence: Owner runtime acceptance and Full Data Verification 347/347 PASS with zero unlinked Materials.

Date: Historical feedback (date not recorded)
Area: Pricing / job quotation
Type: Feature proposal
Severity: Important
Status: Solved
What happened: MaterialID-aware print-job price calculator
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in v48.1.2. Print Job Quotes calculate quantity, Material/manual cost, Printer and governed Settings/FX inputs,
retain accepted snapshots, support explicit deletion and export a customer-safe PDF without silently recalculating saved quotes.
Verification evidence: Owner accepted the quote workflow and customer PDF; Full Data Verification passes 363/363.

Date: Historical feedback (date not recorded)
Area: Pricing settings / printer profiles
Type: Data-model proposal
Severity: Important
Status: Solved
What happened: Governed calculator settings, printer profiles and quote snapshots
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved through v48.1.1-v48.1.2. Schema v35 owns canonical PrinterProfiles and seven pricing defaults; schema v36 owns
saved quote snapshots and the dedicated Print Job Quotes workflow.
Verification evidence: Owner accepted Printer CRUD/currency behavior and quote workflow; Full Data Verification passes 363/363.

Date: Historical feedback (date not recorded)
Area: Purchasing / inventory lots
Type: Data-ownership rule
Severity: Important
Status: Solved
What happened: Purchase- and lot-scoped exchange-rate snapshots
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved through v48.2.0 and retained by v53. Purchase Orders snapshot accepted exchange-rate source/date/provenance;
Inventory and downstream saved history retain their accepted values and never become MaterialID-wide mutable rates.
Verification evidence: Owner accepted ECB new-order prefill and unchanged historical values; v48.2 Verification passes 364/364.

Date: Historical feedback (date not recorded)
Area: Build / diagnostics / packaging
Type: Build-governance proposal
Severity: Important
Status: Solved
What happened: Governed Production and Development build profiles
Expected behavior: Resolve the bounded requirement described in Resolution after ownership and compatibility review.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved by the v51 governed runtime-profile milestone. Owner Production, Disposable Verification and Clean Readiness
profiles share explicit identity/capability ownership while mandatory recovery, security, updater and support evidence remains active.
Verification evidence: v51.4 owner acceptance and Release profile matrix pass; populated scenarios pass 382/382 with exact state.

Date: 2026-07-25
Area: Fast Materials / validation refresh
Type: Bug
Severity: Important
Status: Solved
What happened: Recursive Reload Materials Prototype prompt after an accepted Fast edit
Expected behavior: Preserve the accepted behavior and compatibility contract described in Resolution.
Steps to reproduce: Historical feedback; no additional reproduction steps were retained.
Screenshot / export / report attached: None retained with the original note.
Resolution: Solved in v44.7.7 Stage 5I. Validation now runs after snapshot acceptance through the existing edit debounce.
Verification evidence: Owner runtime re-test and Full Data Verification 330/330 PASS.
2026-07-26 v48.1.1 implementation: schema v35 now owns canonical
PrinterProfiles, seven governed Print Job Pricing defaults and a separate
Printers CRUD/archive workspace. Deterministic hourly cost treats uptime as an
explicit 0–100 percent. Quotes remain deliberately out of scope until v48.1.2.

Date: 2026-07-26
Area: Public website / Canonical branding / Favicon
Type: Website idea / UI polish
Severity: Normal
Status: Solved
What happened: The main public 3DPIceland website does not consistently expose the canonical 3DPIceland logo in its page header or
the small application-icon artwork as the browser favicon.
Expected behavior: Add the canonical 3DPIceland logo to the main public website and use the approved small 3DPIceland icon artwork
for favicon output. Preserve the accepted website layout, responsive behavior and report branding contracts.
Steps to reproduce: Open the main website, inspect the visible header branding, browser tab icon and generated page metadata.
Screenshot / export / report attached: Capture desktop, narrow/mobile and browser-tab presentation during implementation.
Resolution: v57.0.3 stages the approved canonical website logo and favicon through exact Preview and Production publish-plan
allowlists. The main header, favicon metadata, calculator portal and standalone redirect share governed relative asset routes without
changing desktop application or custom document-logo ownership.
Verification evidence: Debug/Release, deterministic website contracts and owner visual review pass. Coordinated publication completed
on 2026-07-28; the main page, calculator redirect, PNG logo and favicon independently return HTTPS 200 with the expected markers.

Date: 2026-07-26
Area: Export branding / Reports / Website documents / Print-job quotes
Type: Feature idea / Branding workflow
Severity: Normal
Status: Solved
What happened: Exported reports and future print-job quotes currently use only the built-in 3DPIceland branding contract.
Expected behavior: Allow users to configure their own PNG logo for generated HTML/PDF website documents, reports and print-job
quotes. Custom branding must be confined to generated document output. It must never replace or alter the Windows application icon,
the logo/header inside the desktop application, the splash screen or other application chrome.
Steps to reproduce: Generate a report or quote and confirm there is no governed option to select a user-owned document logo.
Screenshot / export / report attached: Future acceptance should compare default-branded and custom-branded HTML/PDF/quote output.
Resolution: Planned in v58.0 — Governed Custom Document Branding. Research one governed document-branding owner and every
HTML/PDF/report/quote renderer before coding.
Accept PNG only after validating signature, decoded dimensions, size limits and transparency behavior; never trust extension alone.
Store or copy the accepted asset into a controlled per-user location without modifying the source file. Provide preview, explicit
Remove/Restore default branding, honest missing/corrupt fallback and consistent aspect-ratio sizing without stretching or clipping.
Preserve the built-in 3DPIceland logo as the default. Do not embed local source paths, owner data or unintended metadata in public
output. Existing report routes and standalone generated artifacts must remain compatible.
Research result: one immutable `DocumentBrandingSnapshot` will own normalized bytes, hash and fallback provenance. A dedicated SQLite
singleton retains backup/restore consistency; renderer cache files are governed materializations only. Public-site/app branding stays
separate, and stable report asset names remain compatibility aliases. v58.0.1-v58.0.6 now define the authoritative delivery order.
v58.0.2 candidate adds schema-v39 transactional normalized PNG storage, full decode/dimension/pixel/size validation, metadata-free
re-encoding, SHA-256 verification, contained atomic cache materialization and explicit Default/Custom/Fallback provenance. No visible
control or renderer consumes it yet, so Help and AutomationRunner UI changes correctly remain owned by v58.0.3-v58.0.5.
Verification evidence: v58.0.2 disposable profiles `20260728215136-14b9769c` and `20260728215405-332dd301` pass 409/409 with exact
state recovery. Schema v38 is retained as a migration fixture and the validated v39 seed is canonical. Renderer selection remains
owned by v58.0.4; visual scale, transparency, clipping and HTML/PDF/quote presentation remain owner-manual acceptance.
Final resolution: Solved in v58.0.6 — 2026-07-28. Schema v40 owns the governed
custom PNG and Brand / Organization Name. Reports, website documents and
print-job quotes render accepted custom/default branding while application
chrome and canonical public-site branding remain unchanged.
Final verification evidence: Owner accepts default, wide, tall, square and
transparent logo output, combined Restore Default and brand identity across
HTML/PDF/quote paths. Full Data Verification passes 414/414.

Date: 2026-07-26
Area: Main application navigation / Tab ordering
Type: UI polish / Final workflow review
Severity: Normal
Status: Solved
What happened: New workspaces and tabs have been added over multiple releases, so the overall tab order may no longer reflect the
best daily-use workflow or the clearest grouping of related areas.
Expected behavior: Review and, where justified, reorder all main application tabs as one coherent final navigation pass. Perform this
only after every authoritative Master Roadmap item is complete and the Bug / Feedback Log has no unresolved implementation items.
Steps to reproduce: After roadmap and feedback completion, traverse every tab from left to right and compare the order with common
Materials, measurement, reporting, website, purchasing, inventory, research and administration workflows.
Screenshot / export / report attached: Capture the complete tab order before and after any accepted reordering.
Resolution: Planned last in v59.0 — Application Navigation Finalization. Do not reorder tabs incrementally while new workspaces are
still being
introduced. Preserve tab content, AutomationIds, keyboard navigation, lazy-load behavior and saved layout/state ownership. Assess the
tester and Verification navigation contracts in the same final increment, while keeping visual ordering acceptance owner-manual.
Verification evidence: None; blocked until the authoritative Master Roadmap is complete and all earlier open, partially solved and
deferred feedback has been resolved, retired or explicitly dispositioned.
Final resolution: Solved in v59.0.8 — 2026-07-29. The owner-approved 22-tab
order is canonical, six grouped Navigate menus cover every top-level
destination and replaced Website/prototype entrypoints are retired. Materials
also uses the accepted exact 52-column default order with preserved user
layouts and safe persistent reset behavior.
Final verification evidence: Owner accepts every tab, menu, Materials order,
restart/reset and retirement behavior. Disposable Verification passes 418/418,
clean-VM/demo compatibility passes and parent v59 is closed and published at
canonical application release v59.0.7.

2026-07-26 v48.1.1 runtime feedback: owner accepted the initial Printer
workspace and Verification 362/362, but requested governed currency selection.
Printer currency is now a dropdown backed only by valid Purchasing currency
rates from Settings; unsupported free text is blocked.

2026-07-26 v48.1.1 acceptance: owner confirmed Printer CRUD, restart
persistence, Settings-backed currency dropdown and hourly-rate refresh.
Full Data Verification evidence passes 362/362. Increment complete; immutable
quote workflow continues in v48.1.2.

2026-07-26 v48.1.2 candidate: schema v36 adds immutable Print Job Quote
snapshots, canonical Material landed-cost or explicit manual-cost provenance,
Printer/Settings/FX evidence, deterministic component totals and snapshot-based
HTML export. Normal update/delete/recalculate actions are deliberately absent.

2026-07-26 v48.1.2 runtime bug: Save Immutable Quote could appear to do
nothing when draft validation blocked persistence because the reason was shown
only in the calculation summary. Save now always reports validating, explicit
not-saved reason, database failure or successful immutable Quote number.

2026-07-26 v48.1.2 runtime bugs: Material selection did not prioritize the
canonical Website Display Name, making Base Material/variant identity unclear.
Quote validation also rejected an empty optional Printer upfront cost although
the Printers workspace correctly treated it as zero. Dropdown labels now use
Website Display Name first and empty optional numeric values normalize to zero.

2026-07-26 v48.1.2 runtime scope revision: owner requested separate
print/post-processing, customer consulting and parts design/change labor,
customer-facing PDF output and deletion of obsolete/test quotes. The workflow
now snapshots three labor-minute inputs, renders an A4 customer PDF without raw
JSON, and supports confirmed owner deletion while preventing silent
recalculation of retained saved quotes. Locale parsing also corrects `1,3`
Printer buffer from the erroneous 13 to 1.3.

2026-07-26 v48.1.2 customer PDF feedback: owner approved quote generation but
classified MaterialID and Printer identity as internal-only, requested the
3DP Iceland Labs logo and removed the external methodology credit from the
customer document. PDF now shows the customer Material name without MaterialID,
omits Printer entirely, embeds the canonical Labs logo and contains no
Print Farm Academy reference. Internal saved snapshots retain exact IDs.

2026-07-26 v48.1.2 acceptance: owner approved separate labor inputs, quote
deletion, corrected comma-decimal pricing and the final customer PDF with the
Labs logo and no internal MaterialID, Printer identity or methodology text.
Full Data Verification passes 363/363. The increment is complete, canonical
and runtime accepted.

# 2026-07-28 - v58.0.3 document branding Settings workflow

- **Observed:** The accepted schema-v39 document-logo foundation had no
  user-visible selection, preview, status or removal workflow.
- **Correction:** Added a dedicated Settings Manager group with stable
  AutomationIds, governed Select PNG, read-only Preview, default-No Remove
  Custom Logo and default-No Restore Default. Status and preview distinguish
  Default, Custom and honest Fallback without changing the source file.
- **Boundary:** The workflow saves immediately and remains separate from generic
  Settings save/reset. Renderer output stays on built-in branding until v58.0.4;
  application and canonical public-site branding are out of scope.
- **Status:** Implementation and Help complete. Debug/Release, Help,
  documentation and read-only vulnerability gates pass. Disposable smoke
  `20260728221033-cb9d2635` passes 410/410 with exact state. Owner runtime
  acceptance remains pending.

# 2026-07-28 - v58.0.3 preview and safe-cancel runtime correction

- **Observed:** Preview only refreshed the inline image and gave no visible
  response. Standard MessageBox confirmation did not map Escape or window close
  to No for Remove Custom Logo or Restore Default.
- **Correction:** Preview now opens a large, read-only, owner-modal window with
  Close/Escape/X support. Both mutating actions use the shared accepted
  `SafeDeleteConfirmationWindow`, where No is focused/default and Escape/X
  preserve the custom logo.
- **Preserved:** Custom selection survived restart, Restore Default Yes returned
  to built-in branding and owner Verification passed.
- **Status:** Corrected Debug/Release, Help/documentation and disposable smoke
  `20260728221852-cdfa377b` pass 410/410 with exact state. Owner retest pending.

# 2026-07-28 - v58.0.3 redundant action and Settings layout correction

- **Observed:** Remove Custom Logo and Restore Default produced the same
  persisted default state. The Settings grid declared only four rows while the
  measurement host used row 4, so it overlaid the AI provider row.
- **Correction:** Owner approved retaining only Restore Default. Remove control,
  handler, Help, inventory and Verification ownership are retired. The missing
  Auto row is restored before the final measurement star row.
- **Status:** Debug/Release, Help/documentation and disposable smoke
  `20260728222506-c4fc44fe` pass 410/410 with exact state. Owner visual retest
  pending.

# 2026-07-28 - v58.0.3 Settings row correction initially targeted wrong grid

- **Observed:** Owner retest showed the AI/measurement overlap remained.
- **Finding:** The first generic XAML patch inserted extra rows into Base
  Materials instead of `SettingsManagerTab`; Settings still had four rows while
  its measurement host used row 4.
- **Correction:** Removed the unintended Base Materials rows, added the fifth
  Settings row in an explicitly anchored patch and named the Settings layout,
  AI group and measurement host. Verification now requires exactly five rows
  with the two groups in rows 3 and 4.
- **Status:** Debug/Release, Help/documentation and disposable smoke
  `20260728223043-346ac3db` pass 410/410 with the explicit five-row layout
  contract. Owner confirms the layout is normal and Verification passes.

# 2026-07-28 - v58.0.4 renderer branding ownership

- **Observed:** Accepted v58.0.3 selection and preview did not yet affect
  generated report, report website-document or customer quote output.
- **Correction:** One immutable renderer asset now supplies normalized PNG data
  URIs and a matching white-background JPEG derivative. Internal HTML and quote
  export use the PNG; public report packages and native PDF use the derivative
  under the existing stable JPEG route.
- **Preserved:** Main-site/app branding, public routes and allowlists, WebView2
  PDF path, manifests, canonical data and saved quote calculations are
  unchanged. Existing generated files are not rewritten.
- **Status:** Debug/Release, Help/documentation/security and disposable smoke
  `20260728224453-a8dd5135` pass 411/411 with exact state. Owner custom/default
  output review and Full Data Verification pass.

# 2026-07-28 - v58.0.4.1 document brand identity

- **Observed:** A selected custom document logo could appear beside prominent
  hard-coded 3DPIceland document headings, producing mixed branding.
- **Decision:** Add one optional Brand / Organization Name with
  `3DPIceland Labs` as default/fallback. Apply it only to generated report,
  website-document and quote-facing text.
- **Preserved:** A smaller Generated with 3DPIceland Engineering Platform
  version line retains platform provenance. Application identity, canonical
  public main-site branding, stable routes and saved snapshots are unchanged.
- **Status:** Debug/Release, Help/documentation/security and disposable smoke
  `20260728230807-481ac60c` pass 412/412 with exact logical state after
  schema-v39-to-v40 migration. Owner visual, restart and Restore Default
  acceptance remain required before closure.

# 2026-07-28 - v58.0.4.1 duplicate platform provenance

- **Observed:** A custom-brand website document displayed two Generated with
  3DPIceland Engineering Platform lines, and the first incorrectly ended with
  the custom brand name.
- **Finding:** The write path reapplied the brand transform to HTML whose
  platform provenance had already been rendered.
- **Correction:** The transform now recognizes an existing exact platform
  identity and is idempotent. Deterministic checks cover both repeated
  transformation and already-formatted provenance.
- **Status:** Solved and owner runtime accepted. Debug/Release,
  Help/documentation and disposable smoke `20260728231627-626aa475` pass
  412/412 with exact logical state. Custom/default identity, Restore Default
  and exactly one platform-provenance line are confirmed.

# 2026-07-28 - v58.0.5 branding migration and recovery acceptance

- **Risk:** Schema-v40 brand identity could leave the canonical tester seed
  stale or allow logo/name state to diverge across restart and SQLite backup.
- **Correction:** Synthetic Verification now covers PNG limits, source/cache
  integrity, brand validation, backup parity, corrupt fallback and combined
  Restore Default. Existing disposable smoke owns bounded runtime execution.
- **Evidence:** Schema-v39 migration smoke `20260728232355-2dd2b477` and
  promoted schema-v40 smoke `20260728232520-3f97f349` pass 413/413 with exact
  logical and business state.
- **Status:** Complete. Schema-v39 fixture is retained; validated schema-v40
  seed is canonical. Owner database, Production and FTPS were not touched.

# 2026-07-28 - v58.0.6 document branding parent-closure candidate

- **Scope:** Review default plus wide, tall, square and transparent custom
  logos across internal report, website-document and customer quote output.
- **Preserved:** Schema v40, canonical data, app chrome, public main-site
  branding, stable routes and saved snapshots remain unchanged.
- **Evidence:** Debug/Release and disposable smoke
  `20260728233100-98643431` pass 414/414 with exact logical state.
- **Status:** Complete and owner runtime accepted. Default, wide, tall, square
  and transparent outputs are normal; Full Verification passes 414/414.

# 2026-07-27 - v52.3.2 live response validation lacked safe failure detail

- **Observed:** After owner consent, the first live Responses result reached the
  safe validation-failure dialog instead of a usable advisory.
- **Finding:** The first hypothesis was an unconstrained evidence ID. Adding an
  exact enum did not resolve the live failure, proving that hypothesis was not
  sufficient. The retained evidence safely reported only generic validation
  and zero usage, which prevented definitive offline diagnosis.
- **Correction:** Exact evidence-ID enum remains as defense in depth. The
  request now sets low reasoning effort and a 4,000-token output budget.
  Validation evidence safely classifies incomplete/token-limit, refusal,
  missing/malformed output and captures returned usage without raw content.
- **Status:** Solved and owner runtime accepted. Debug and isolated Release builds pass with zero warnings/errors;
  smoke `20260727170841-4473c154` passes 385/385 with exact state. Owner live
  rerun was pending. Canonical Release smoke `20260727171213-558ca0ca`
  repeats 385/385 with exact state after the locked owner app was closed.
  Owner live rerun then completes successfully: 40 governed Materials,
  3,246 input and 1,655 output tokens in 34.7 seconds. Copied evidence exposes
  no credential, material identity, planning note or raw provider content.
  Save Session succeeds, but owner Verification reports 384/385 because its
  deterministic contract required Copy Operational Evidence to remain disabled
  even after live evidence existed. The assertion now requires button state to
  match in-memory evidence presence; automation still proves the disabled
  no-live state. Isolated smoke `20260727172300-de072a18` passes 385/385 with
  exact business-state equality. Canonical Release smoke
  `20260727172502-e8736f1b` repeats 385/385 with exact state.
  Final owner Full Data Verification passes 385/385.
- **v59.0.7 sequencing decision:** The owner approved v59.0.7 as the final
  canonical application-byte identity. v59.0.8 is docs-only parent closure and
  must not change accepted app, installer, portable or demo bytes.
- **Pinned demo inputs:** Source, private allowlist and public transformation
  SHA-256 values match the accepted v56 contracts; the fast compatibility path
  is authorized without reopening privacy selection.
- **v59.0.7 local evidence:** Debug/Release and vulnerability/Help/docs gates
  pass. Disposable application smoke passes 418/418 with exact state.
- **Demo evidence:** A/B SQLite SHA-256 is
  `874027BCAEEFE8B32A8BADCC9F5159A4C54A5D317C3029D84260107B8AE634C4`;
  A/B ZIP SHA-256 is
  `85DC95E96DF6F43903A0A9E108F555B1CDA3912E7A407B156B54AE2736D81500`.
- **Runtime evidence:** Disposable demo scenario
  `20260729014916-de8d1b9b` passes 406/406 applicable checks with exactly 12
  governed Website N/A checks, restart, 36-ID scope, local reports and exact
  state recovery.
- **Remaining:** Clean-VM installer/restore/uninstall compatibility and owner
  acceptance precede final Production artifact freeze and guarded FTPS.
- **Clean-VM acceptance:** v59.0.7 installs cleanly; schema-v38 demo restore and
  restart pass, Full Verification passes and uninstall/reinstall preserves the
  restored user database.
- **Remaining boundary:** Freeze clean-worktree Production bytes locally.
  Production/FTPS publication still requires separate explicit live authority.
- **Publication accepted:** Guarded application release, public demo and signed
  updater workflows completed. Application backup:
  `/backups/application_releases/release_2026-07-29_122745_402`; demo backup:
  `/backups/public_demo/release_2026-07-29_122815_902`.
- **Independent HTTPS evidence:** Stable and versioned installer, portable and
  demo hashes reproduce the frozen local files. Signed update hash matches and
  live `latest.json` identifies v59.0.7 `NAVIGATION-LAYOUT-CLOSURE`.
- **Status:** v59.0.7 and docs-only v59.0.8 parent closure complete.
