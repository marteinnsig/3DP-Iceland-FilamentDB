# Help Control and Field Ledger

v70.0.1 material-detail.charts/analytics and measurements.impact own the Legacy Impact visual radar reference.
It averages available corrected Flat/Upright means over the matching rig maximum; 100% means rig ceiling, not material score.
Historical partial break/drag limits are explicit. Missing/calibration-invalid references remain unavailable.
The visual reference cannot affect Overall, modern score coverage, Consistency, recommendations or ranking.
Existing read-only chart owners remain; no new editor/window/schema or owner-data mutation. Owner visual acceptance pending.

v70.0.0 material-detail.charts/analytics owns method-aware scoring and read-only Score coverage.
Analytics has 430 DataGrid columns in the full XAML inventory (one added column). Legacy Impact remains historical;
its score is unavailable. Missing axes render gaps without zero markers or fill. Group coverage states eligible/total members.
Five equal Overall families require both approved method scores; pending fixed references never fall back to cohort maxima.
Raw Izod/Charpy means remain visible while references await owner approval. No new editable field, Settings row or window.
Existing smoke rankings/selectors and Full Verification own policy, missing/reference, grouping and propagation checks.
Owner readability/navigation acceptance is pending; the accepted v69 runtime and recalculation are recorded separately.

v69.0.0 menu.tools-validation owns Tools → Legacy Impact Recalculation... (LegacyImpactRecalculation).
A standard confirmation previews settings; Yes creates backup, recalculates derived results and retains JSON audit.
No cancels; backup/validation failure stops. Raw percentages, notes and dates remain unchanged.
measurements.impact documents explicit J/mm² conversion and independence from direct Izod/Charpy.
No new editable input or runtime window is introduced. Owner readability/repair acceptance remains pending.
The owner explicitly requests a candidate ZIP for this handoff.

v68.0.3 extends existing read-only analysis owners without adding editing or external-send actions.
Rankings adds Izod and Charpy metric choices and score columns (429 DataGrid columns total).
analysis.rankings, analysis.category-rankings and analysis.awards own separate method rankings and awards.
material-detail.charts/analytics and analysis.rankings explain the equal-weight available seven-component Overall:
Tensile, Impact, Izod, Charpy, Stiffness, Consistency and Layer Adhesion. Thermal and Flexible remain independent.
Consistency now includes valid direct-method CV percentages and sample counts, preserving missing and measured-zero semantics.
analysis.dashboard-insights, ai.planning-briefs, ai.collections and youtube.overview/generate own method evidence and
candidate relevance. Same-method leaders, kJ/m² means and normalized scores respect each caller's current material scope.
Existing navigation destinations and read-only boundaries remain stable. Full Verification covers score propagation,
independent method leaders, missing/zero values and suggestions; owner readability/navigation acceptance remains required.

v68.0.2 Material Detail result integration: PendulumDetailSummaryText is a read-only selected-material summary above
every inner tab. DashboardIzodMeanText/StatisticsText and DashboardCharpyMeanText/StatisticsText show canonical
method statistics. ChartIzodScoreText/SourceText and ChartCharpyScoreText/SourceText own separate comparative axes;
Analytics exposes IzodDisplay and CharpyDisplay. Help material-detail.mechanical, material-detail.charts,
material-detail.analytics and measurements.pendulum-impact own units, missing values, cohort normalization and refresh.
Materials Help owns In Izod/In Charpy after In Flexible and six-method Tested Status; Flexible remains independent.
General Test Information groups all coverage flags; the Other presentation is hidden without deleting source fields.
Compare Help specifies only direct method means in kJ/m². Existing IDs and read-only boundaries remain unchanged.
The existing disposable tester and Full Verification need status, method independence, restart and display-owner checks.
Owner display acceptance remains required. v68.0.1 layout/speed/input were accepted on 2026-09-21.
Future owner handoffs use the local runnable application rather than a ZIP.

v68.0.1 runtime.pendulum-impact: IzodImpactViewHost / CharpyImpactViewHost contain the shared Fast views
IzodImpactMeasurementsGrid / CharpyImpactMeasurementsGrid. Every currently visible material receives one row automatically.
Help measurements.pendulum-impact owns Sample1-10 (direct instrument kJ/m²), TestNotes, MeasuredDateText and read-only
Mean, StdDev, Cv, Samples, Confidence and Validation. Blank is unmeasured; NB is nonnumeric; explicit zero remains a sample.
CV % is 100 times sample SD divided by mean; Confidence is numeric sample count capped at 10.
SQLite save follows successful cell commit; the shared editor preserves click/type, Tab/Shift+Tab and arrow behavior.
Current Help retires run selectors, run creation, profile editors, specimen creation and diagram-window destinations.
Saved historical energy graphs remain supported for persistence and reporting, separate from direct readings.
Settings Help owns specimen geometry defaults: 80 x 4 x 10 mm, 2 mm notch and 4 x 8 mm remaining fracture section.
Geometry supplies context only; direct kJ/m² readings are not converted. Full Verification covers direct data and navigation;
owner runtime acceptance covers typing, focus, filtering, save/restart and Help readability/navigation.

v67.0.10 Fast Flexible entry: FastFlexibleSessionsGrid, FastFlexibleSpecimensGrid, FastCompressionPointsGrid,
FastStressRelaxationGrid, FastRecoveryMeasurementsGrid and FastShoreHardnessGrid use the shared Materials renderer.
Help flexible-testing.overview owns inherited columns, units, validation, pending/save status and keyboard behavior.
Existing XAML DataGrids are hidden column/selection bridges, retained until runtime acceptance then retired in v67.0.11.
Full Verification covers host ownership, detached TVL input and keyboard/sort rules. No schema or statistics change.

Version: v66.0.6 Flexible intelligence candidate coverage

Compare / FlexibleComparisonEvidence aligns A-D results by metric/unit/measurement condition in one table.
Video Planner / SelectedFlexibleVideoEvidence and Recommendations / SelectedFlexibleRecommendationEvidence are read-only expanders
for the currently displayed Material Detail identity; their panels carry saved evidence and matching visible-scope peer candidates.
Help material-detail.compare, material-detail.general and material-detail.controls-fields own these destinations. No editable input
or persistence command is added. Existing reset, selection and successful-save refresh update them without changing Overall.

Material Detail / General / Flexible Material Testing is a compact coverage summary (AutomationId MaterialDetailsFlexibleEvidence).
Mechanical / Engineering Dashboard / Flexible Material Testing owns the result tables (MaterialDetailsFlexibleDashboard).
DashboardFlexiblePanel renders one table and collapsed Test setup per method; FlexibleDashboardSetup1..N expose setup expanders.
Help material-detail.mechanical owns these read-only controls. Statistics are aligned once per table; conditions are shared.
Saved refresh and reset update the Mechanical section without touching Flexible measurement editors or legacy Overall.
Help material-detail.general and flexible-testing.overview own scope, units, missing values and save/refresh guidance.
Saved active MaterialID sessions feed detached typed summaries; no editor, new input handler or persistence action is introduced.
Comparable Results adds Recovery and shares the same calculations; it remains selected-session scoped, including history inspection.
Full Verification owns projection/format contracts; owner accepts readability and General/Comparable navigation.

Compression / Force retention % is read-only, owned by FlexibleMaterialTestingService and Help's flexible-testing reference.
It uses saved factual force/hold values in the same specimen, cycle, target and displacement group; the first point stays blank.
Dependent values update after commit through property notification without replacing ItemsSource. Raw readings remain authoritative.
Add Relaxation Point is retired. Saved Stress Relaxation appears only for a specimen with older rows; otherwise the tab is hidden;
its model, storage, export/recovery and existing delete ownership remain supported, not a temporary replacement adapter.
Comparable Results owns read-only retention summaries by method, target, displacement, cycle and actual time interval.
Full Verification covers independent n and statistics; owner acceptance covers focus, readability, visibility and Help navigation.

Recovery / TVL contact offset mm is an alternate editable representation of initial minus recovered height, under flexible-testing.overview.
Zero is initial contact before loading; positive offsets mean downward travel. Decimal comma/dot input retains partial/invalid text.
Invalid TVL blocks saving; blank clears height and zero means full height recovery. Direct height entry is a supported alternative.
Property notifications synchronize the two inputs without rebind. Existing saved heights are authoritative, and historical offsets
are derived equivalents. No schema, input handler registration, scenario authorization or seed changes are needed.

Settings Manager / Flexible Material Testing / Default recovery compressed hold is an editable nonnegative seconds value,
initially 30. Existing Settings editing and Save/Reload/default restoration own persistence; Add Recovery snapshots it into new rows.
Old readings and the separate rest-time default remain unchanged. Help settings.reference and flexible-testing.overview own guidance.

Purpose: authoritative control-level inventory for the mandatory v50.4
exhaustive Help audit. `Docs/HELP_COVERAGE_MATRIX.md` remains the accepted
surface-level v50.2/v50.3 baseline; it is not control-level completion proof.

## Audit rules

Every supported user-facing candidate must receive one stable ledger key and
one classification:

- `action`: button, menu action or other invoked command;
- `input`: text, numeric, date, password or multiline value;
- `choice`: checkbox, radio option, selector or filter;
- `editable-column`: user-editable grid/custom-grid cell type;
- `status`: validation, readiness, progress or evidence needed to use an action;
- `read-only`: output or interpretation that must be understood but cannot be edited;
- `layout`: resize/reset/display-only behavior;
- `unsupported`: dead or retired UI with an explicit removal owner.

Coverage is complete only when the row records:

1. stable key and exact visible path;
2. XAML/runtime/custom owner and canonical service/data owner;
3. exact Help destination or subsection;
4. purpose, prerequisites, allowed values, units and default meaning;
5. validation, save timing and failure behavior;
6. side effects, confirmation and historical-data rules;
7. cross-tab inputs, downstream handoff and external boundaries;
8. deterministic evidence or an explicit manual-only reason.

AutomationId presence, a click-handler count or a tab overview is not coverage.

## Discovery baseline

Source snapshot: canonical runtime-accepted v55.0.6 on 2026-07-28.

| Source | Discovered candidates | Reconciliation requirement |
|---|---:|---|
| Top-level tabs | 24 | Preserve accepted overview and contextual mappings, including Flexible Material Testing |
| Nested tabs | 16 | Preserve accepted Experimental and Material Detail nested-aware mappings |
| XAML buttons | 169 | Map visible action, handler, owner and Help subsection |
| XAML menu items | 60 | Separate headings/separators from invoked commands |
| XAML text boxes | 35 | Determine editable, read-only, multiline and generated output |
| XAML password boxes | 2 | Record secret ownership and non-persistence boundary |
| XAML combo boxes | 49 | Record source, allowed choice, default and save timing |
| XAML checkboxes | 10 | Record true/false effect, persistence and dependent states |
| XAML data grids | 38 | Reconcile grid-level read-only state and runtime columns |
| XAML grid-column declarations | 425 | Resolve binding, grid owner and actual editability |
| Runtime-built windows/dialogs | open | Inventory constructors, generated controls and confirmations |
| Owner-drawn/custom grids | 6 known | Reconcile column builders and supported edit interactions |

The raw XAML column count is deliberately not labelled editable. Column-level
`IsReadOnly` can inherit or be overridden by grid-level state, binding mode,
templates and runtime behavior. Each candidate must be reconciled with its
grid owner and edit handler.

## Exact XAML candidate inventory

`Docs/HelpControlInventory.tsv` is generated from the registered XAML source
only with an explicit `Tools/Test-HelpControlCoverage.ps1 -UpdateInventory`
action. A normal gate run compares all generated rows with the committed file
and fails on drift.

Current exact inventory:

| Owner | Candidates |
|---|---:|
| v50.4.1 | 219 |
| v50.4.2 | 267 |
| v50.4.3 | 156 |
| v52.1 | 7 |
| v52.2 | 3 |
| v52.3.2 | 1 |
| v53.0.2 | 2 |
| v53.0.4.1 | 6 |
| **Total** | **661** |

v52.1 adds seven Settings candidates for provider/model preference, protected
credential ownership and local-only foundation diagnostics. All route to
`settings.controls-fields`; none enables a live OpenAI request.

v52.2 adds three AI Assistant pilot actions for exact payload preview,
consent-gated generation and active-request cancellation. They route to
`ai.controls-fields`; automation covers preview only and never performs live
network I/O.

v52.3.2 adds one secret-safe operational-evidence clipboard action. It remains
disabled until a live attempt and never retains raw payloads or credentials.

v53.0.2 adds the landed-cost currency selector and default-No Draft override.
Both route to `purchase-orders.controls-fields` and remain disabled for legacy,
calculated or non-Draft orders.

v53.0.4.1 adds six hidden landed-cost lifecycle controls. They are authorized
only by an exact disposable manifest and remain unsupported owner-facing UI.

| Initial source classification | Candidates |
|---|---:|
| Actions | 181 |
| Choices | 59 |
| Input candidates | 33 |
| Grid candidates | 11 |
| Editable-column candidates | 318 |
| Read-only fields | 4 |
| Read-only grids | 20 |
| Read-only columns | 38 |

Candidate classification is intentionally conservative. The 318 column and
11 grid candidates are not accepted as editable until binding, grid-level
state, templates and handlers agree. The eight Application-shell actions are
automation-only controls and require an explicit supported/unsupported
disposition rather than user Help prose.

## Known owner-drawn/custom column registries

| Surface | Declared columns | Current reconciliation owner |
|---|---:|---|
| Fast Materials | 52 | v50.4.1 |
| Fast Tensile | 45 | v50.4.2 |
| Fast Impact | 45 | v50.4.2 |
| Fast Stiffness | 18 | v50.4.2 |
| Fast Settings | 6 | v50.4.1 |
| Fast Base Materials | 23 | v50.4.1 |

These 189 declarations are separate from the XAML grid-column declarations.
Each builder passes through `PrototypeColumnKey`, and existing Verification
requires unique keys and exact expected counts for all six registries. The
audit must use each property/layout key and editor kind, not only the header.

## Runtime-generated registry

Eight runtime surface owners declare 42 exact controls in
`Docs/HelpControlCoverageRegistry.json`. The normal gate requires unique
control keys, a live entry point, a live Help destination and a valid status.

The hidden PDF print host is explicitly `unsupported` as a user Help surface.
It remains supported internal report infrastructure. Verification now records
its v51.3 classification owner, while Diagnostics records its v53.0.4.4
aggregate landed-cost reconciliation owner. Other surfaces retain their
bounded v50 ownership.

## Post-v50 reconciliation

The deterministic gate requires explicit coverage ownership for v51.1-v51.4,
v52.1-v52.3.2 and v53.0.2-v53.0.5. It also requires exact Help markers for:

- Owner, Verification/Disposable and Clean/Readiness runtime profiles;
- Mandatory versus CanonicalDataDependent Verification classification;
- Windows Credential Manager and OpenAI payload/evidence boundaries;
- landed-cost defaults, manual fallback, calculation provenance and
  Diagnostics non-mutation.

Inventory provenance retained in SQLite but not exposed as an editable visible
column is explained as read-only historical evidence in Inventory Help. It is
not misclassified as a user-editable XAML field.

## Delivery ownership

| Increment | Required ledger scope | Exit condition |
|---|---|---|
| v50.4.0 | All discovery sources and stable registry/gate contract | Every candidate has a bounded owner |
| v50.4.1 | Data, purchasing, inventory, cost and configuration | No unexplained supported control/field |
| v50.4.2 | Measurements, Experimental Testing and analysis | No unexplained supported control/field |
| v50.4.3 | Output, publishing, assistant, creator, menus and runtime windows | No unexplained supported control/field |
| v50.4.4 | Cross-scope reconciliation, deterministic drift gates and tester | Zero gaps plus owner acceptance |
| v53.0.5 | v51-v53 Help, owner and runtime-surface reconciliation | Zero unexplained post-v50 gaps |
| v54.0.5 | Six Materials facets, persisted scope, exact AI/collection evidence | Owner runtime accepted |
| v54.0.6 | Hidden scalar filter retirement and final Help inventory | Canonical; 661/661 Help audit |
| v55.0.2 | Named default-No Base Material deletion and cancellation safety | Candidate Help and Verification |
| v55.0.5 | Runner-owned reviewed cleanup and diagnostics ownership | Candidate Help and Verification |
| v55.0.5.1 | Shared safe-delete dialog for Material and Base Material | Owner runtime feedback correction |
| v55.0.6 | Operational safety runtime acceptance and closure | Canonical; Verification PASS |
| v61.0.8.2 | Hidden disposable thermal persistence controls | Tester-only; bounded exact-ID authorization |

## Initial findings and risks

- The accepted Help catalog is broad and substantive but is organized mainly
  by workflow subsections, not by a declared registry of every control/field.
- Several named XAML controls already have AutomationIds, but many do not.
  IDs will be added only where stable lookup provides useful evidence.
- Runtime-built Verification and Diagnostics controls are safety-sensitive.
  Help/tester coverage must not authorize recalculation or export mutations.
- Recovery, updater, Production, FTPS and delete controls require exact
  default-No and historical-data wording; read-only tester inspection only.
- Custom-grid columns require units, editor type, validation and save timing
  from their column definitions and handlers, not guesses from their labels.
- Repeated column shapes may share one precise Help table only when every
  covered key is declared; prose implying coverage is insufficient.

## Open implementation choices

1. Prefer a source-controlled registry consumed by Verification and Help.
   This gives one machine-checkable coverage owner.
2. A generated report alone is insufficient because generated output can
   silently normalize away unsupported or runtime-only controls.
3. Per-control Help popups are not required. Exact searchable subsections and
   field/control tables can satisfy coverage with less UI clutter.

The registry format and complete candidate rows are the remaining v50.4.0
implementation work. No exhaustive Help content is accepted until that ledger
can detect missing additions.

Report previews, HTML/TXT/PDF exports and opted-in public report templates now carry safe Flexible aggregates for actual scope.
Existing report controls and help destinations own generation; flexible-testing.overview explains statistics, method groups and privacy.
No new editable controls; fallback PDF continuation preserves all results. Existing scenario authorization and publish opt-ins remain.

v66.0.5: Rankings Dashboard, Category Rankings and Awards & Winners add Flexible read-only evidence sections.
FlexibleRanking/Category/AwardCategoryFilter and ExportFlexibleRanking/Category/AwardCsv belong to analysis.rankings,
analysis.category-rankings and analysis.awards. Each uses its own category; physical/visible scope filters remain shared.
Test setup expanders expose saved internal methods only; CSV/report renderers project safe identifiers and numerical conditions.
No editable grid or measurement field is added. Standard noneditable ComboBoxes retain native keyboard navigation.
Reset restores All Flexible categories; refresh/empty-scope clearing and successful saved-measurement refresh own recalculation.

v66.0.6: FlexibleVideo/Recommendation/ResearchChoice, CopyFlexible*Brief and SaveFlexible*Idea use stable AutomationIds.
Help material-detail.video-planner, material-detail.recommendations and youtube.overview own the dataset opportunity selectors,
single-line material/topic choices, method details in the result panel, clipboard action and explicit saved-idea snapshot action. Existing selected-material panels stay.
Dashboard/AI topics cover visible/processed scope, declared topic limits, safe aggregate preview and unchanged consent boundary.
No new editable grid is introduced. Before saving, the existing idea grid commits cell/row edits through the accepted shared flow.
Detail clearing neither clears nor rebinds the global saved queue; existing editable-grid handlers/persistence remain supported.

## v66.0.7 Flexible analytics
material-detail.analytics owns FlexibleAnalyticsEvidence and the generated native-unit charts, scoped by existing Chart Mode.
material-detail.compare owns additional SD/CV/range in the accepted read-only comparison. No new editing or persistence controls.

v66.0.7 Compare correction: material-detail.compare owns one condition table; FlexibleCompareSetup expanders are retired.
Saved method snapshots remain supported by data entry, Mechanical detail, analytics and report consumers.

v66.0.8: material-detail.video-planner and youtube.overview own Flexible candidates in the existing read-only discovery grids.
No controls/IDs or editing path added. Copy actions use current generated facts; saved records are not rewritten.

v67.0.0: website.preview owns Flexible Testing, synchronized web filters, filament selection and Mean/SD/CV tooltips.
No WPF data-entry control is added; website keyboard/tooltip/selection verification is browser-owned.

### v67.0.1 — Horizontal website charts

website.preview owns horizontal labels, end values and SD whiskers; existing chart identities and filtering remain unchanged.
No editable WPF surface or new AutomationId. Browser visual checks and owner acceptance own label readability.

### v67.0.2 — Flexible whitepaper methods

help.whitepaper owns Chapter 8 Recovery and established Shore A/D procedure: 50 x 50 x 8 mm, 24 h, five locations, 10 s.

Flexible testing Help owns the read-only Shore location statistics column and dashboard rows; website.preview owns its popup.
Each specimen retains valid reading count, mean, sample SD and CV; independent-specimen statistics remain separate.
No editable field or input handler changes. Existing Shore rows retain their saved dwell, thickness and identities.

### v67.0.4 — Flexible list layout and ordering

flexible-testing.overview owns the larger bounded Session and specimen lists, page scrolling and numeric Specimen header sorting.
No new input field or AutomationId. Existing shared click/keyboard handlers and deferred save path remain; sort commits edits first.
Owner visual/input acceptance is required; pure numeric ordering is added to Full Verification without pixel assertions.

### v67.0.5 — Flexible session templates
flexible-testing.overview owns the 10 Compression + 1 Shore template, prepared blank rows and FlexibleCorrectPrintLayers.
settings.controls-fields owns eight new default rows and validation; no new input editor or keyboard contract.
The print-layer action confirms its all-session scope, backs up first and preserves measured values; no automatic startup mutation.

### v67.0.6 — Retired Flexible controls
flexible-testing.overview no longer exposes FlexibleCorrectPrintLayers or the Legacy Experimental Run column.
The previous v67.0.5 correction ownership is historical; no Help action remains. Saved legacy metadata stays database-owned.

### v67.0.7 — Session numeric ordering
flexible-testing.overview owns initial numeric Session order and numeric header toggling; no new control or editor.
The specimen numeric comparer and shared edit-commit helpers are reused. Other headers retain ordinary WPF sorting.

### v67.0.8 — Shore template defaults
flexible-testing.overview covers dedicated Shore defaults and the one-time backup-first repair of unused legacy rows.
Existing editors and Settings handlers remain; no new control or AutomationId. Measured/custom rows are preserved.

### v67.0.9 — Flexible selection isolation
flexible-testing.overview owns synchronized Session/specimen selection, keyboard navigation and commit-before-switch behavior.
Existing six grids, input controls and AutomationIds retained. Reading actions reject stale/out-of-session specimen selections.
