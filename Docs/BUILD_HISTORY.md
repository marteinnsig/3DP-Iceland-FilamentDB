> Canonical role: engineering build, verification and runtime evidence narrative.
> `CHANGELOG.md` owns chronology and `RELEASES.md` owns the curated release
> ledger.

## v44.7.12 - Clean Baseline Retirement

Repository-wide ownership review identified a parallel hand-built PDF layer
that had no caller after canonical HTML/WebView2 PDF became authoritative.
Its model projection still performed report-specific work on every preview,
despite never reaching the exported PDF. The complete dead layer is removed.

The same review retired caller-free workbook-import writes, old
website-template selection/renderer residue, legacy workflow handlers and
dependent helpers. Compile probes protected cross-partial Fast-grid helpers;
one initially over-broad removal was immediately detected and restored before
the cleanup continued.

Tracked asset review retained every runtime/package-owned logo and removed one
1.28 MB source PNG that was copied to build output without a consumer.
Compatibility, recovery, schema, formulas, canonical reports and deployment
behavior remain outside the cleanup boundary.

Initial owner testing passed Verification and normal Engineering Package
export, but a fresh Public Report Package exposed a residual fingerprint query
against retired `TensileResults`. The query set is corrected to the canonical
native measurement tables without recreating any legacy schema.

The corrected Public Report Package completed every report family without
visible errors. Sampled HTML, prior Engineering Package output, branding and
final Full Data Verification passed; v44.7.12 is canonical.

## v44.7.11 - Settings Manager Command Clarity

v44.7.11 separates three existing Settings Manager command boundaries.
Reload Saved Settings reads General and Deployment values from SQLite after a
default-No discard confirmation. Restore Built-in Defaults replaces and saves
only General Settings. Reset Columns remains a machine-local two-layout reset.

Research corrected a real ownership defect: the previous built-in restore
reused an initialization helper that also replaced the in-memory Base Material
Catalog. Deployment Settings, Base Materials, schema, formulas, reports,
website/FTPS and recovery remain separately owned. Runtime acceptance is
complete.

Owner visual review found the shared Fast-grid Materials reload footer repeated
in both Settings views. It refreshed only current in-memory rows, not SQLite,
so both Settings instances are hidden while programmatic refresh remains.

Owner reload, restore, cancellation, restart, layout and visual tests passed.
Full Data Verification passed 336/336; v44.7.11 is canonical.

## v44.7.10 - Canonical MaterialID Default Row Order

v44.7.10 centralizes natural MaterialID comparison in the accepted Fast-grid
presentation layer. All four daily grids build default rows in numeric ID order
while active header sorts are reapplied whenever the visible source set
changes.

No SQLite query/order, canonical collection, schema, formula, filter,
column-layout preference or retired legacy grid is changed. Debug/Release,
static/documentation and read-only NuGet advisory gates pass.

The first owner ordering run succeeded but close/restart exposed
parent-before-child persistence inversion after Add/Duplicate. The candidate
now commits active Fast editors and saves Materials before measurement FK
children, with one default-No warning if parent persistence is blocked.

Owner re-test accepted the close-order fix and identified startup viewport
positioning. Deferred filter refresh re-entered ensure-visible after selection
restore, so startup now resets the Fast Materials viewport to top-left at
ApplicationIdle while retaining the saved selection.

The next owner Add test correctly blocked an invalid parent row and caused
dependent canonical-parity Verification failures. Research traced this to
duplicate generic placeholder display names. Add/Duplicate defaults now include
their generated MaterialID and are unique before the first edit.

Owner Add, Duplicate, active/default sort, close/restart and startup viewport
tests passed. Full Data Verification passed 335/335; v44.7.10 is canonical.

## v44.7.9 - Public Measurement Date Provenance

Candidate adds a shared public measurement-date model to the existing Material
Engineering and Test Session publication pipelines. The projection reads
canonical schema-v31 per-module metadata and exposes only ISO dates or the exact
`Not recorded` fallback through reviewed allowlists.

Internal create/edit timestamps remain forbidden. Public material opt-in,
detail approval, formulas, routes, manifests, canonical HTML-to-PDF generation,
website/FTPS and other report families retain their accepted behavior.

Owner review exposed a GUI naming ambiguity: `Build Selected Public Reports`
did not consume the adjacent template or scope selection. The action is renamed
`Build Public Material Reports`, with explicit workflow guidance. No handler,
route or output behavior changed.

Long string tooltips were also clipped by the shared maximum width. The common
tooltip content template now wraps text while retaining the existing width,
padding, font size and display timing.

The Reports workflow action row now uses a WrapPanel so narrower windows retain
every action without horizontal clipping. The explanatory workflow text stays
below the actions and wraps independently.

Debug/Release builds passed with zero warnings/errors, and
static/documentation gates passed. Owner runtime review accepted HTML/PDF date
provenance, exact `Not recorded` behavior, responsive workflow actions and
wrapped tooltips. Full Data Verification passed 334/334.

## v44.7.8 - Backup Filename Compatibility

Candidate centralizes readable presentation filenames for all newly created
SQLite backups while retaining the established online SQLite backup engine.
New files use purpose-specific `3DPIceland-...-YYYY-MM-DD_HHmmss_fff.bak`
names with a GUID fallback for same-millisecond collisions.

Recovery Center, compatibility verification and direct restore now accept both
`.bak` and every existing `.sqlite` file. Legacy automatic files remain visible
to diagnostics/throttling but are excluded from the new automatic cleanup set,
so the increment does not rename, move or delete existing evidence.

Updater state continues to record the verified backup as an opaque full path.
SQLite restore remains explicit/default-No and interrupted application recovery
continues to restore governed application files only. Isolated Debug/Release
passed with zero warnings/errors, and static/documentation gates passed.
A Windows PowerShell reflection probe could not load the net9 assembly because
its host lacked `System.Runtime 9.0.0.0`; the compiled probe remains covered by
the net9 in-app Verification Center.

The first owner runtime pass confirmed new manual/automatic and SQLite
pre/post-restore names, legacy discovery and Full Data Verification 333/333.
Diagnostics exposed `Automatic backups: 21 / 20` because it combined retained
legacy files with the new rotating set. The corrected candidate reports those
counts separately without changing backup files or cleanup behavior. The
provided Excel workbook was an export, so it correctly created no pre-restore
snapshot. Final explicit Excel restore created a verified pre-restore `.bak`;
corrected diagnostics and Full Data Verification 333/333 passed. v44.7.8 is
runtime accepted and complete.

## v44.7.7 - Legacy Grid Retirement

Stage 1 hides the accepted Fast-workflow legacy/preview switches across
Materials, Tensile, Impact, Stiffness and Settings. Reset Columns and canonical
editing actions remain visible. Legacy DataGrids remain collapsed internal
column/row adapters until explicit Fast contracts replace those dependencies
in later runtime-gated stages.
The first Verification run failed 77 aggregate gates because informational
metadata remained on v44.7.6 while assembly metadata was v44.7.7; the candidate
identity is now aligned.
Owner runtime testing accepted Stage 1 and Full Data Verification passed
319/319. Stage 2 is the measurement-adapter replacement checkpoint.
Stage 2 now gives Tensile, Impact and Stiffness explicit Fast column contracts
and canonical filtered row sources. The three Fast builders no longer consume
legacy DataGrid columns or items; legacy XAML remains collapsed pending runtime
acceptance.
Owner runtime testing accepted Stage 2 and Full Data Verification passed
320/320. Stage 3 moves the Materials Fast view to an explicit schema and
canonical filtered row source.
Stage 3 now provides an explicit 52-column Materials Fast contract with
canonical filtered rows, checkbox/ComboBox ownership and stable layout keys.
The Fast builder no longer consumes legacy Materials columns or items.
Runtime review found edit-time reordering, hidden-grid Duplicate selection,
white tab return and premature new-Material measurement synchronization. The
candidate now preserves row state during same-scope refresh, prefers Fast
selection, redraws on tab load and gates measurement sync on successful
Materials persistence.
Follow-up Verification exposed deferred Delete persistence: UI/measurements
were at 201 while SQLite Materials remained at 203. Delete now persists
measurement removals first and the parent MaterialID removal immediately.
Owner retest confirmed 201/201 UI/SQLite parity and Full Data Verification
passed 321/321. Stage 3 is accepted.
Stage 4 gives General Settings an explicit six-column Value-only Fast contract
and Base Materials an explicit 23-column contract with three governed
ComboBoxes. Both retain canonical row sources and no longer derive Fast schemas
from legacy DataGrid columns.
Owner runtime testing accepted Stage 4 and Full Data Verification passed
322/322.
Stage 5A removes all measurement legacy toggle controls, handlers, fallback
state and reset branches. Fast measurement views are the only activatable
paths; collapsed legacy XAML awaits the Stage 5B deletion checkpoint.
Owner runtime testing accepted Stage 5A and Full Data Verification passed
323/323.
Stage 5B-Tensile removes the complete legacy Tensile DataGrid XAML and all
named grid-specific bind/edit/commit/filter/layout/warm-up/close references.
Fast Tensile remains on the accepted explicit canonical contract.
Owner runtime testing accepted Stage 5B-Tensile and Full Data Verification
passed 324/324.
Stage 5B-Impact removes the complete legacy Impact DataGrid XAML and all named
grid-specific bind/edit/commit/filter/layout/warm-up/close references. Fast
Impact remains on the accepted explicit canonical contract. Stiffness remains
available as the final measurement deletion checkpoint. Impact runtime
testing passed across editing, validation, calculations, navigation, filters,
layout and restart persistence. Full Data Verification passed 325/325;
Stage 5B-Impact is accepted.
Stage 5B-Stiffness removes the final legacy measurement DataGrid XAML and all
named grid-specific bind/edit/commit/filter/layout/close references. The
obsolete legacy measurement DataGrid warm-up is removed with it. Fast
Stiffness remains on the accepted explicit canonical contract; runtime
testing passed across editing, validation, calculations, navigation, filters,
layout and restart persistence. Full Data Verification passed 326/326;
Stage 5B-Stiffness and measurement-grid retirement are accepted.
Stage 5C removes the retired global Tools workflow-column reset command and its
uncalled generic handler family. Materials gains a local Fast reset action;
accepted per-workspace reset behavior and saved layouts remain unchanged.
Owner runtime testing accepted menu removal, local reset behavior, saved
layouts and restart persistence. Full Data Verification passed 326/326;
Stage 5C is accepted.
Stage 5D removes the hidden Settings legacy-grid toggle, handler and fallback
activation state. Fast General Settings and Base Materials become the only
activatable Settings UI; legacy XAML and grid-only lifecycle remain for the
next checkpoint. Owner runtime testing accepted Settings editing, validation,
Base Material CRUD, reset, persistence and cross-tab behavior. Full Data
Verification passed 326/326; Stage 5D is accepted.
Stage 5E removes the complete legacy General Settings and Base Material
DataGrid XAML plus grid-only bind/edit/layout/recovery and selection fallback
callers. Fast canonical selection now solely owns Base Material deletion.
Owner runtime testing accepted Settings editing, validation, exact Fast
selection CRUD, layout and restart persistence. Full Data Verification passed
327/327; Stage 5E is accepted.
Stage 5F removes the hidden Materials preview toggle, fallback handler/state
and legacy-view reactivation method. Fast Materials becomes the only
activatable Materials UI; legacy XAML and grid-only lifecycle remain for the
next checkpoint. Owner runtime testing accepted editing, exact-selection CRUD,
filters, reset, layout, tab return and restart persistence with 201 canonical
rows. Full Data Verification passed 327/327; Stage 5F is accepted.
Stage 5G moves reports and Materials CRUD to the Fast-owned canonical
selection contract. New-row focus, archive/restore refresh and recalculation
no longer drive hidden DataGrid state. Remaining legacy filter/edit/recovery
adapters stay deletion-gated; runtime acceptance is pending.
Owner runtime testing accepted exact Fast selection for reports and all
Materials CRUD across filters, sort, tab changes and restart cleanup to 201
canonical rows. Full Data Verification passed 328/328; Stage 5G is accepted.
Stage 5H moves Materials filter, visible-report/count and governed-column
ownership from hidden DataGrid state to canonical predicates and the explicit
Fast contract. Legacy edit/recovery adapters remain deletion-gated; runtime
testing accepted filters, selection clearing, measurement propagation,
ranking/report scope and restart parity at 201 canonical rows. Full Data
Verification passed 329/329; Stage 5H is accepted.
Stage 5I removes Materials DataGrid commit/edit handlers and their recovery,
update, close, validation, inventory and manual-save callers. Fast canonical
autosave coalescing remains. A recursive unapplied-snapshot prompt found during
runtime testing was corrected by deferring validation refresh until after Fast
snapshot acceptance. Owner runtime acceptance and Full Data Verification
330/330 passed; Stage 5I is accepted.
Stage 5J retires the remaining Materials workflow registry, binding, selection
and refresh callers. Canonical Fast selection now owns startup and Material
Detail, while purchasing, Inventory and measurement sync refresh the Fast view.
Startup selection and tab-return viewport regressions found during owner
testing were corrected with a one-shot post-load selection and measured
visibility refresh. Full Data Verification passed 331/331; Stage 5J is accepted.
The collapsed DataGrid XAML remains deletion-gated.
Stage 5K deletes that final collapsed Materials DataGrid and its duplicate XAML
column definitions. Fast Materials becomes the sole grid host. Owner runtime
acceptance and Full Data Verification 332/332 passed; v44.7.7 is complete.

## v44.7.6 - Fast Workflow Grid - Settings

Candidate migrates both editable Settings Manager tables to the accepted Fast
Workflow Grid. General Settings keeps Value-only editing, manual/close save and
immediate transactional Deployment validation. Base Material Catalog keeps all
text/ComboBox fields, canonical SQLite replacement, downstream recalculation
and add/delete selection. Both views own separate layouts, in-place reset and a
shared visible legacy fallback. Password, SQLite schema, reports, FTPS
publishing, updater and recovery remain unchanged. Debug/Release, Verification
and owner runtime acceptance are required. First runtime materialization hit a
`FormattedText` argument-range crash from transient lazy-tab render geometry;
the shared renderer now normalizes DPI and geometry before text construction.
Follow-up showed blank constructor-created surfaces and misplaced toolbar
actions. Settings now activates Fast views only after tab realization, with
toggle/reset actions owned by the correct toolbar. Cross-tab review also found
duplicate blank spacer keys grouping Impact/Stiffness separators. Stable
occurrence-qualified identities and one-time stale-layout fallback correct
canonical placement and restart persistence. Materials filter propagation now
also reloads all three Fast measurement snapshots after their legacy
collection views receive the shared visible MaterialID set.
Owner runtime retest accepted both Settings views, legacy fallback, separator
persistence and shared filter propagation. Full Data Verification passed;
v44.7.6 is accepted.

## v44.7.5 - Fast Workflow Grid - Stiffness

Candidate applies the accepted viewport-only workflow grid to Stiffness while
retaining canonical rows, 0–10 revolutions, 0–359 degrees, measurement dates,
unchanged deflection/modulus formulas, filters, summaries, test-status refresh
and SQLite auto-save. It starts by default, owns separate keyed layout state
and retains a visible legacy fallback. Rejected input restores once; in-place
cell refresh and reset preserve sorting, selection and scroll. SQLite schema,
reports, FTPS, updater and recovery remain unchanged. Debug/Release,
Verification and owner runtime acceptance are required. First runtime review
found narrow Stiffness content offset from the editor overlay; explicit
left/top surface alignment and WPF coordinate translation correct both the
leading gap and editor placement. Owner runtime retest accepted the complete
Stiffness workflow and Full Data Verification passed 317/317. v44.7.5 is
accepted.

## v44.7.4 - Fast Workflow Grid - Impact

Candidate applies the accepted viewport-only workflow grid to Impact while
retaining the existing canonical row objects, 0–100 needle validation,
measurement-date behavior, formulas, colors, filters, summaries, test-status
refresh and SQLite auto-save. Fast Impact starts by default, owns separate
keyed layout state and retains a visible one-click legacy fallback. In-place
cell refresh preserves sorting and selection after edits. Impact formulas,
SQLite schema, reports, FTPS, updater and recovery remain unchanged.
First runtime review found negative Tensile input, repeated Impact invalid
warnings and reset-time canonical reordering. The correction enforces
non-negative canonical samples, restores rejected Fast cells after one warning
and resets layout in place. Debug/Release and security/static gates passed.
Owner runtime retest accepted all corrections and Full Data Verification passed
316/316. v44.7.4 is accepted.

## v44.7.3 - Fast Workflow Grid - Tensile

Candidate begins the approved migration away from problematic editable WPF
DataGrids. The accepted viewport-only Materials renderer now has reusable row,
column, editing and layout contracts. Tensile is the first migration: it uses
the existing canonical measurement rows, calculations, validation, filters,
test-status refresh and SQLite auto-save. Fast Tensile owns separate keyed
layout state, starts by default and retains a visible one-click legacy-grid
fallback until runtime acceptance. Sample and computed-cell visual meaning is
retained. The first runtime sort/selection reset was corrected with in-place
cell refresh. Impact, Stiffness and Settings remain unchanged pending
sequential acceptance. Debug/Release and security/static gates passed. Owner
runtime testing accepted the full checklist, reported a noticeably snappier
view and Full Data Verification passed 315/315. v44.7.3 is accepted.

## v44.7.2 - Validation Help Clarity

Candidate adds concise Materials help defining row Validation as the presence
of Material ID, Manufacturer, Product Line, Base Material and computed Website
Display Name. `OK` means those five fields are present; it does not claim that
measurements, pricing or other material data are fully verified. Existing
validation calculations, duplicate checks and save boundaries remain
unchanged. Debug/Release passed with zero warnings/errors, the documentation
and vulnerability gates passed, and owner runtime screenshot review plus Full
Data Verification passed. v44.7.2 is accepted.

## v44.7.1 - Category Rankings Scope Controls

Candidate adds bounded 5, 10, 50, 100 and All row scopes to Category Rankings.
The canonical visible MaterialID source, score selectors, ordering, tie-breaks
and CSV-visible-scope behavior remain unchanged. The separate Rankings
Dashboard retains its existing Top 25 reset behavior. Debug/Release passed
with zero warnings/errors. Owner runtime acceptance and Full Data Verification
313/313 passed; v44.7.1 is accepted.

## v44.6.2 - Canonical Measurement Date Foundation

Adds an explicit nullable measured date to each native material/test-type set
and each Experimental run. Storage uses invariant `yyyy-MM-dd`; WPF displays
`dd.MM.yyyy`. First-input auto-assignment never overwrites an
existing date and does not infer dates for historical data. SQLite schema v31
is additive and schema-v30 canonical databases migrate without using the
retired pre-v30 workbook shape. Runtime corrections made blank dates safe,
preserved partial manual input, aligned compact Stiffness editing with
Tensile/Impact and corrected editor activation after column reordering.
Debug/Release passed with zero warnings/errors and runtime Full Data
Verification passed 312/312. v44.6.2 is accepted.

## v44.6.1 - Canonical Release Documentation Audit

The release-documentation audit established explicit ownership for the four
historical release documents and reconciled v44.5.2-v44.6.0 into the build,
release and milestone records that had stopped at v44.5.1. A standalone
read-only gate compares recent canonical headings and bounds every known
historical within-file duplicate through a checked-in baseline. New duplicates
or recent gaps fail; old history is reported but never rewritten. Debug/Release
passed with zero warnings/errors, the audit and vulnerability gate passed, and
runtime Full Data Verification passed 311/311. v44.6.1 is accepted.

## v44.6.0 - Recovery Center Clarity

Removed the persistent verbose updater-evidence box and replaced the
compatibility glossary with one concise Recovery Center sentence. Exact
selected-backup details, guarded Default-No restore and complete updater
evidence in diagnostics remain unchanged. Runtime Full Data Verification passed
311/311 and v44.6.0 is accepted.

## v44.5.9 - Supported Migration Naming

Renamed canonical SQLite projection, supported empty-target JSON migration and
built-in default methods according to current ownership. Runtime testing with
MAT0206 also exposed and corrected whole-revolution stiffness calculation and
close-time active-cell persistence. Full Data Verification passed 310/310.

## v44.5.8 - Retired Transition UI Residue

Removed caller-free load/import-sync handlers, their confirmation helpers and
unused JSON save-state allocations while preserving the supported empty-target
JSON migration readers and governed recovery. Runtime Full Data Verification
passed 309/309.

## v44.5.7 - Legacy Workbook Schema Retirement

Advanced SQLite to schema v30 and transactionally retired the 13 original
workbook/normalized tables after retained verified backup. Canonical
Tensile/Impact/Stiffness consumers and schema-v30 recovery were corrected and
runtime accepted with Full Data Verification 308/308.

## v44.5.6 - Retired Workbook Metadata Readers

Removed original-workbook metadata readers and their Material Detail, Tools and
diagnostics surfaces. Governed Excel disaster recovery and compatibility
inspection remained intact. Runtime Full Data Verification passed 307/307.

## v44.5.5 - Retired Legacy Write Entry Points

Removed caller-free broad workbook/material/cache replacement entry points
while retaining still-required read compatibility and recovery boundaries.
Runtime Full Data Verification passed 306/306.

## v44.5.4 - Measurement Help Clarity

Removed duplicated Tensile, Impact and Stiffness help fragments without
changing calculation or storage behavior. Runtime Full Data Verification passed
305/305.

## v44.5.3 - Canonical Storage Terminology

Replaced stale transition/import wording with canonical SQLite terminology
while retaining supported JSON migration and governed recovery. Runtime Full
Data Verification passed 304/304.

## v44.5.2 - Canonical SQLite UI Boundaries

Removed misleading cache UI, retired `MaterialsImport` backup-first and renamed
the Settings action to its actual built-in-default ownership. Supported JSON
migration, recovery and updater behavior remained unchanged. Runtime Full Data
Verification passed 303/303.

## v44.5.1 - Active SQLite Compatibility Safety

Removed the legacy startup `File.Delete(DatabasePath)` behavior. Active SQLite
compatibility is inspected read-only; unsupported files remain unchanged while
an exact verified evidence copy is retained and startup fails closed. An
initial runtime fixture handle exposed SQLite pooling retention, corrected with
non-pooled inspection/fixture connections. Full Data Verification then passed
302/302; v44.5.1 is accepted.

## v44.5.0 - Retired Excel Import Surface

Removed only the unreachable original-Excel database import handler and its
caller-exclusive importer services. Lower-level SQLite compatibility data,
legacy JSON migration snapshots and governed Excel disaster recovery remain
intact. Runtime Full Data Verification passed 301/301 with zero failures;
v44.5.0 is accepted.

## v44.4.1 - Measured Materials Responsiveness

Measured the wide Materials DataGrid bottleneck, rejected configuration-only
experiments and introduced a bounded viewport-only renderer as the daily-use
default with the native DataGrid retained as fallback. Runtime acceptance
covered cold/repeated scrolling, direct canonical edits, filters, selection,
sorting, clipboard, persisted layout and visual parity. Full Data Verification
passed 300/300. Clean-VM direct install, explicit SQLite restore, fast-view
runtime, native fallback and portable runtime passed; v44.4.1 is canonical.

## v44.3.1 - Backup, Recovery and Update Evidence Clarity

Completed honest empty-profile backup classification and four separate
read-only update-evidence boundaries for transaction state, health
acknowledgement, application rollback snapshot and SQLite backup evidence.
Clean-VM runtime acceptance passed Application Readiness and Overall
Verification 209/209 with 90 N/A. SQLite restore remains explicit/default-No,
application rollback never restores SQLite automatically and evidence is
retained.

## v44.2.0 - Daily-use UI State and MaterialID Clarity

Debug and Release builds completed with zero warnings and zero errors. Runtime
accepted machine-local width/order and MaterialID persistence, one light-blue
selected row, normal text editing and checkbox-only hit bounds. Full Data
Verification passed 298/298. A measured older 15-second first horizontal page
jump is retained for v44.4 after A/B testing ruled out saved column order.

## v44.1.2 - Verification Profiles and Diagnostic Honesty

Debug and isolated Release builds completed with zero warnings and zero errors.
Candidate ECDSA packaging, NuGet vulnerability checks, BOM-less update feed,
exact bytes/SHA-256, governed six-file inventory, schema v29 and
stable-route-last gates passed. Clean VM Application Readiness passed 207/207
with 90 N/A; restored-data Full Data Verification passed 297/297 immediately
after explicit restore and automatic restart.

## v43.7.0 - Installer and Portable Deployment

Built and runtime-accepted a first-install deployment layer over the signed updater package rather than creating a second application inventory. VM probes removed an obsolete 176-row compiled material fallback, a data-bearing website snapshot, owner-specific FTPS defaults, a loose WPF logo dependency and three unused SVG diagrams. The corrected per-user installer and portable ZIP use exactly six governed runtime files and are blocked if data files or private markers appear. Clean VM, SQLite transfer, credential isolation, Verification Center, live publish and browser downloads passed.

## v43.6.0 - Update and Deployment Diagnostics

Extended the accepted v43.5.1 updater without changing its v1 durable schema. Added read-only prior-transaction diagnostics, startup detection and default-No external recovery for all incomplete phases. Isolated tests prove safe Prepared restart and last-known-good restoration from SnapshotReady, Installed, RollingBack and RollbackFailed. SQLite restore remains prohibited, transaction evidence/backups are retained, and website/report/FTPS code is untouched. Debug and Release builds completed with zero warnings and zero errors; Visual Studio Debug runtime acceptance passed Verification 294/294.

## v43.5.1 - Guarded Application Update

Completed the manual guarded updater path with repeated signed extraction verification, pending-edit persistence, verified SQLite backup, external process orchestration, durable last-known-good application snapshots and startup health acknowledgement. Initial portable testing identified and removed an invalid loose single-file WPF icon URI without touching SQLite. The corrected signed 11-file v43.5.1 base applied v43.5.2, restarted successfully, wrote matching transaction/version/schema health, committed durable state and passed Verification 293/293. Automatic rollback remains limited to governed application files; SQLite restoration is always separate and explicit.

## v43.5.0 - Transactional Updater Engine

Added an external updater helper and shared versioned transaction engine with durable phases, contained staging/rollback roots, last-known-good governed-file snapshots and rollback after partial installation or failed health. Disposable contract tests committed a complete staged set, restored all prior files after an injected mid-install failure, restored after failed health and blocked traversal. The helper became the tenth signed package file. Runtime acceptance confirmed the complete signed inventory and Overall Verification PASS while same-version protection remained blocked. Live process orchestration and Apply remain deferred to v43.5.1.

## v43.4.1 - Governed Signed Release Packaging

Established the production signing trust root in the Windows user-scoped CNG key store with a non-exportable ECDSA P-256 private key and fingerprint-pinned public key embedded in the application. Added strict clean-worktree release publishing, an exact nine-file package allowlist, CNG-backed manifest signing and an independent probe that invokes the real application verifier. Production and tampered package tests passed their accept/block boundaries. Runtime acceptance confirmed all integrity, trust and SQLite schema checks; same-version protection correctly remained blocked and Verification was Overall PASS. Application-file apply remains disabled pending the transactional external updater.

## v43.4.0 - Signed Update Readiness Foundation

Introduced a bounded, read-only application-update package verifier before any installer or updater apply path. The governed manifest binds release identity, supported SQLite schema range and every application file to exact path, size and SHA-256 values, then requires an ECDSA P-256/SHA-256 signature. Unsafe ZIP paths, incomplete inventories, tampering, downgrade and incompatible schema states fail closed. Production apply remains disabled until a governed public trust key and external transactional helper are approved. Runtime acceptance confirmed the new Tools entry and Overall Verification PASS; Debug and Release builds completed with zero warnings and zero errors.

## v42.2.1 - Public Engineering Report Content Expansion

Expanded the v42.1 safety-focused mini report toward the accepted internal Material Engineering Report contract without exposing the internal record. The dedicated public model now carries Verified Material Summary averages, standard deviation, CV, samples/confidence and stiffness results; engineering score bars and a six-axis selected/material-average/manufacturer-average radar; all metric ranks/percentiles; decision guidance; stronger alternatives; governed interpretation; strengths, limitations, trade-offs, recommended applications and peer context. The public allowlist grows from 21 to 38 explicit top-level fields; publishing still receives no raw specimens, internal notes, purchasing, operational stock, credentials or device paths and performs no measurement or score calculation. Internal operational shell labels including `Unified HTML report engine` and `Materials in database` are excluded.

Corrected the radar SVG axis and label coordinates to use locale-independent decimal points. Icelandic decimal commas had been interpreted by SVG as coordinate lists, splitting labels into individual letters and extending left-side axes beyond the chart. The SVG viewport now also includes the complete left-side `Consistency` and `Layer adhesion` labels. A dedicated Verification check rejects locale-formatted scalar radar coordinates and requires the expanded viewport.

Added the canonical `3dp-iceland-labs-logo-pdf.jpg` asset to public HTML. Because the PDF is printed from that exact HTML and the package already copies the JPG into its local assets folder, HTML/PDF branding remains one canonical contract. Verification covers substantive content and JPEG branding. Runtime acceptance confirmed the expanded public HTML/PDF report, corrected complete radar labels and all-PASS Verification. Release build completes with zero warnings and zero errors.

## v42.2.0 - Canonical Public Material Selection

Added the publication gate requested for the Materials workspace: each canonical MaterialID now has an explicit `Public reports` checkbox backed by the SQLite `PublishPublicReports` column. Migration is additive and defaults every existing and future row to false, so public reporting always requires deliberate opt-in. The backwards-compatible JSON working copy continues to load material content, but SQLite overrides the publication flag.

The existing website material universe is unchanged. Selected rows receive stable `reports/materials/{MaterialID}/` and companion `report.pdf` fields; unselected rows receive null report fields. The shared Preview/Production HTML transform adds report actions to the existing visible-material links surface without creating a separate renderer. No FTPS file list or upload behavior changed.

Verification covers UI availability, record mapping, default-off selected-versus-unselected routing, renderer integration and the aggregate v42.2 release gate. Debug and Release builds complete with zero warnings and zero errors.

Initial runtime testing exposed a UI-only interaction defect: the shared one-click DataGrid editor marked the mouse event handled before `DataGridCheckBoxColumn` could toggle. Native Materials boolean columns now toggle directly on the first click, update the visible checkbox immediately and use the existing deferred refresh and SQLite auto-save workflow.

A second runtime pass exposed a scope mismatch inherited from v42.1: the preview action still used the current row rather than the new publication selection. It now batches every active opted-in MaterialID, completes safety verification for the whole set before artifact writing, produces one canonical HTML/PDF package per material and writes a combined preview index with links for the full selection.

Runtime acceptance confirmed that checked publication selections persist after restart, two selected materials create two complete report packages, the combined preview index links to both reports and Verification Center is all PASS. v42.2.0 is accepted; FTPS report publication remains intentionally deferred until the full public report portfolio is ready.

## v42.1.0 - Public Report Publishing Foundation

Started the v42 public publishing roadmap with one deliberately bounded artifact: a local static Material Engineering Report preview for the currently selected canonical MaterialID. The package uses the stable relative route `reports/materials/{MaterialID}/` and contains canonical `index.html`, `report.pdf` printed from that HTML, public metadata, a manifest and local assets. The preview root has its own landing page and is not connected to FTPS production publishing.

Public output is not produced by passing the complete internal report or native material record to a sanitizer. A dedicated public model structurally exposes 21 approved identity, public-link, canonical MSRP and governed engineering-result fields. Purchasing, operational stock, credentials, device paths, raw specimen rows and internal notes are absent from the renderer input. Verification covers the allowlist, forbidden-field boundary, stable route, canonical artifact links, methodology/whitepaper context and local-only UI workflow.

Initial runtime testing exposed a verifier-only false positive: the generic drive-path expression matched the trailing `s:/` inside valid `https://` public links. The rule now requires a non-alphanumeric drive-letter boundary, preserving real Windows path detection while allowing HTTPS links. Failure details now report the exact matched token/path condition.

A second verifier-only false positive came from the public safety statement itself: its sentence explaining that credentials are excluded matched the generic word filter. The safety gate now detects actual serialized internal field names, rendered operational table/section signatures and device paths. Explanatory governance text is allowed while real internal payload structure remains blocked.

The first successful public preview run exposed an unrelated timing race in the prior v41.8.2 gate: Verification was opened after Tensile and Impact warm-up but before low-priority Stiffness warm-up received Dispatcher time. Optional preload now passes while it is queued and healthy, reports its `n/3` progress, and still fails on a captured warm-up exception.

Runtime acceptance confirmed a successful selected-material public preview package and an all-PASS Verification Center run once the deferred warm-up received a brief opportunity to complete. Build Solution also completed successfully. v42.1.0 is accepted as a local-only foundation; no FTPS production publishing is connected.

## v41.8.2 - Deferred Measurement Tab Warm-up

Added a post-render warm-up for the three high-use measurement workspaces. WPF normally performs the expensive first layout and DataGrid visual realization only when a TabItem is selected, which made the first visit to Tensile, Impact and Stiffness slower than later visits even though their canonical rows were already loaded.

After the first usable Materials frame, each measurement tab is now selected, laid out and restored inside one low-priority Dispatcher callback. This realizes the tab without presenting an intermediate frame or changing the user's current selection. Each tab is measured independently in Startup Diagnostics. All visual-tree work remains on the WPF UI thread; no SQLite operation, canonical data projection or calculation was moved or duplicated.

Runtime acceptance confirmed that Materials remained visible after about 5 seconds and Verification passed. The accepted trace reached first usable Materials at 4.47 seconds, then warmed Tensile in 0.71 seconds, Impact in 0.77 seconds and Stiffness in 0.32 seconds, completing all measurement workspace warm-up at 8.00 seconds without delaying initial visibility.

## v41.8.1 - Startup Refresh Coalescing

The first v41.8.0 runtime trace showed that MainWindow construction itself completed in about 1.0 seconds, while roughly 17 seconds elapsed before `Show()` began. Code review traced that gap to Native Materials bulk replacement: clearing and adding 200 rows raised collection notifications that each queued the same filters, Inventory and measurement synchronization work on the WPF Dispatcher.

Bulk replacement now suppresses those per-row scheduling requests and queues one consolidated Background-priority refresh after the canonical collection is complete. The refresh still executes the same downstream operations and is included in startup diagnostics. No SQLite reads or writes were parallelized, and canonical MaterialID ownership remains unchanged.

Runtime acceptance confirmed Verification PASS and reduced the measured Debug startup to the visible Materials list from about 19-20 seconds to about 5 seconds. The accepted trace recorded MainWindow construction at 0.73 seconds, the consolidated refresh at 0.08 seconds and first usable Materials rendering at 4.49 seconds from instrumentation start.

## v41.8.0 - Startup Performance Instrumentation

Started the profiling-first Startup Performance & Safe Concurrency extension. A centralized read-only timing service now records process-to-instrumentation context, splash milestones, MainWindow construction, individual workspace initialization, Loaded processing, first usable Materials rendering and deferred engineering intelligence completion.

System Diagnostics exposes the complete ordered timing trace so Debug, cold Release and warm Release runs can be compared without relying on subjective splash timing. Verification checks the core timing markers and owns the v41.8.0 release gate. No startup work has been reordered, deferred or parallelized in this build; canonical SQLite/MaterialID/Verified Material Summary ownership and all WPF thread boundaries remain unchanged.

Debug and Release builds complete with zero warnings and zero errors. Runtime Verification and measured-trace acceptance remain pending.

## v41.7.8 - Combined Engineering Report Package

Implemented the final v41.7 package workflow after all six individual reports passed visual acceptance. `Export Engineering Package` creates one timestamped parent folder with numbered Material Summary, Material Engineering, Comparison, Manufacturer, Test Session and Printing Recommendation subpackages. Each remains an independent canonical HTML/PDF contract and carries its own text, metadata, manifest and assets.

The parent package provides an indexed HTML landing page, direct links to every HTML/PDF/metadata artifact, a package manifest and machine-readable JSON metadata. The current selected-versus-visible scope applies through each report's already accepted behavior; the package layer does not merge results or recalculate data. Repeated exports receive a safe numeric suffix rather than overwriting an existing folder. Verification covers the six-report set and package structure. Debug and Release builds complete with zero warnings and zero errors. User acceptance confirmed a successful end-to-end six-report export, working index links and Verification PASS; v41.7 is complete.

## v41.7.7 - Report Portfolio: Printing Recommendation

Implemented `REPORT-150` as the final distinct individual report in the initial v41.7 portfolio. Selected scope composes existing engineering profiles into recommended applications, measured strengths, limitations, engineering trade-offs, decision guidance, family-aware print-workflow checks and stronger same-family alternatives. All-visible scope provides a compact recommendation ledger over the exact current Materials filter.

The report does not calculate measurements or fabricate exact printer settings. It explicitly requires manufacturer/printer validation for nozzle and bed temperatures, speed, cooling, drying and enclosure requirements. Reinforced, higher-temperature and flexible families receive cautious workflow checks without being assigned invented numeric profiles. MaterialID and canonical MSRP now travel through the shared ranking projection. Verification asserts distinct REPORT-150 content and the absence of Video Planner/YouTube hooks. Debug and Release builds complete with zero warnings and zero errors. User acceptance confirmed the presentation and Verification PASS; REPORT-150 is accepted.

## v41.7.6 - Report Portfolio: Test Session

Implemented `REPORT-140` as a distinct Test Session Report. Selected scope exposes the selected material's native tensile, impact and stiffness record: aggregate result quality, recorded raw inputs, module validation, sample/confidence coverage, test notes and the Settings Manager constants used by the governed ResultsService pipeline. All-visible scope provides a compact test-record ledger over the exact current Materials filter.

The current canonical mechanical tables are linked by MaterialID but do not store a dedicated SessionID, test timestamp, operator, printer/slicer profile or environmental record. REPORT-140 states that boundary prominently and renders those fields as `Not recorded`; it never infers or fabricates missing provenance. Verification asserts the distinct selected-detail and all-visible-ledger contracts. Debug and Release builds complete with zero warnings and zero errors. User acceptance confirmed the presentation and Verification PASS; REPORT-140 is accepted.

## v41.7.5 - Report Portfolio: Manufacturer

Implemented `REPORT-130` as the third distinct report in the v41.7 portfolio. Selected scope deliberately changes from one selected material to the complete active canonical portfolio belonging to that material's manufacturer. All-visible scope remains an exact projection of the current Materials search/filter result and can contain several manufacturers.

The canonical HTML/PDF report now covers manufacturer portfolio breadth, product lines, material types, verified-result coverage, complete engineering profiles, MSRP and video availability, average overall engineering context, strongest material/axis, global manufacturer positioning and category position by base material. The product-level table preserves MaterialID and existing Verified Material Summary outputs, links valid product/video URLs and renders missing values as `n/a`.

No measurements or governed engineering scores are recalculated in the reporting layer. Verification distinguishes `REPORT-130` from the earlier report identities, checks selected-manufacturer portfolio expansion, checks multi-manufacturer all-visible behavior and owns the aggregate v41.7.5 release gate. Debug and Release builds complete with zero warnings and zero errors. User acceptance confirmed the report presentation, selected-source identification, clear engineering-axis terminology and Verification PASS; REPORT-130 is accepted.

## v41.7.4 - Concise Report Package Naming

Simplified the shared current-report export folder contract for every engineering report. Package folders now use the readable report title followed once by the timestamp, for example `comparison-report-20260721-231416`. The previous name repeated the platform prefix, report key, output kind and report title.

Internal package contents remain unchanged: canonical HTML, PDF printed from that HTML, text, metadata, manifest and shared assets. Verification asserts the exact naming contract. Release build completed with zero warnings and zero errors, and the user accepted the concise naming change.

## v41.7.3 - Report Portfolio: Comparison

Implemented the second distinct report contract in the v41.7 portfolio for individual acceptance. Comparison Report no longer falls through to the generic material report and now identifies itself as `REPORT-120`.

Selected scope uses the current material as an explicit highlighted anchor and compares it with up to five peers from the canonical visible Materials set. Same-base-material peers are prioritized, then ordered by closest available overall-score distance. All-visible scope compares the exact current Materials search/filter result and retains missing evidence as `n/a` rather than excluding untested materials.

The canonical HTML contains comparison scope, evidence coverage, engineering-axis leaders, score charts, side-by-side axes, overall delta relative to the selected anchor, MSRP USD/kg when available and methodology/whitepaper links. It explicitly limits interpretation of deltas and does not recalculate governed engineering results.

Verification now distinguishes REPORT-120 from REPORT-110 and checks selected-anchor, peer-selection and all-visible contracts. Debug and Release builds complete with zero warnings and zero errors. User acceptance confirmed that the report looks correct and Verification Center passes; REPORT-120 is accepted.

## v41.7.2 - Canonical Material Projection

Completed the application-wide removal of `_materialsView` as a material source. The native SQLite-backed Materials collection now owns current identity, active/archived totals and the filtered visible set everywhere in the application. DataRow projections remain as short-lived adapters for established services, but every such row is built from a canonical native MaterialID rather than retained from the imported workbook cache.

Analytics, rankings, category rankings, awards, Video Planner, recommendations, Dashboard Insights, YouTube Research, AI session/collection context, reports and website export now share the same active or visible material projection. Secondary filter lists are also rebuilt from native records, preventing old imported row sets from reappearing through dropdowns.

The hidden legacy Materials tab, its search/filter controls, legacy selection fallback and imported-view count logic were removed. Workbook tables are now limited to ingestion and explicit transition synchronization. Verification Center checks active/visible unique MaterialID parity, confirms the visible set is a subset of active records and confirms the legacy tab is absent.

Initial acceptance exposed a startup regression because the expanded canonical consumer refresh ran while the splash screen still owned the UI thread. The refresh is now deferred until the main window has loaded and is coalesced at background dispatcher priority; material filtering remains immediate while dependent rankings, advisor, reporting support and research surfaces refresh together afterward.

Debug and Release builds complete with zero warnings and zero errors. User acceptance confirmed a clean Verification Center run, normal behavior across the reviewed tabs and correct propagation of Materials filters into downstream surfaces. v41.7.2 is accepted.

## v41.7.1 - Report Portfolio: Material Summary

Started the report-by-report v41.7 acceptance workflow with the shared scope/export foundation and Material Summary only. Reports no longer derive `All Visible Materials` from the older 176-row projection: the current native Materials collection view supplies the exact filtered MaterialID set, and the active native collection supplies the canonical total.

Material Summary now has its own `REPORT-110` contract instead of the generic fallback. It reports scope size, manufacturer/material-type coverage, verified evidence coverage, complete five-axis profiles and the high-level score state for every material in scope. Preview remains intentionally short and explicitly identifies its first-10 limit; export contains the full scope.

Acceptance refinement added complete, partial and no-evidence profile counts, preserved the active Materials search/filter values in the report, expanded selected-scope identity fields and linked the public platform, methodology portal and governed whitepaper. Technical `Canonical total` wording and duplicate total cards were removed in favor of one clear active-database total.

Final wording review clarified that a verified result is an accepted tensile, impact, stiffness, consistency or layer-adhesion result, not a video/database/pricing status. Materials with a valid HTTP(S) YouTube review URL now expose a clickable link in both selected identity and the full scoped table.

Excel cross-checking exposed that REPORT-110 was still counting legacy score availability (169/169/168) rather than native test modules (193 tensile, 190 impact and 190 stiffness in the user's current dataset). Coverage and complete/partial states now come directly from native Verified Material Summary presence, while report scores use a new native `MaterialResults` scoring path with the established scoring formula. Verification asserts parity between native Materials flags and summary-module presence.

User acceptance confirmed that the corrected report counts now match the Excel export and Verification Center passes. REPORT-110 Material Summary is accepted; v41.7.2 will remove the legacy `_materialsView` dependency across the whole application before Comparison Report work begins.

The single-report export action now always produces canonical HTML and the matching WebView2-printed PDF together with text, metadata, manifest and assets. The multi-report Engineering Package remains deferred until each of the six engineering reports has been individually implemented and accepted, preventing unfinished generic reports from entering a misleading package.

Release build completed with 0 warnings and 0 errors. In-app Verification Center and user visual review remain the acceptance gate before Comparison Report work begins.

## v41.6.0 - Internal Repeatability Calibration

Unified the previously conflicting website, dashboard, Engineering Advisor and whitepaper repeatability bands behind `ConsistencyCalibrationService`. The historic ranking formula remains unchanged, so existing consistency scores and ranking order are preserved. The new canonical labels are based on the resulting 0–100 score and are explicitly named the 3DPIceland internal comparative repeatability scale.

The user-provided observed anchors are now Verification contracts: a complete set with 7.8% average CV maps to 92.2/100 and Excellent repeatability, while 19.4% maps to 80.6/100 and Good repeatability. Review guidance begins at 30% CV for an individual measurement set; 40% is the explicit internal high-variation boundary.

Documentation now distinguishes repeatability from absolute accuracy. It records impact-pointer sensitivity to pivot tension, fastener adjustment and equipment temperature, the tensile carriage's practical 7–8 N low-force floor and roughly 10–15 degrees of stiffness placement/reading variation. CV cannot correct these systematic limitations, so pre-test checks and cautious interpretation remain required.

Verification Center covers canonical score mappings, app/website labels, review boundaries, methodology/whitepaper content and downstream handoffs. Debug and Release builds completed with 0 warnings and 0 errors and produced file version 41.6.0.0.

During report acceptance, a missing context handoff caused Selected Material Engineering Reports to show a valid consistency score while claiming that Verified Material Summary was unavailable. The focused report now passes the canonical summary map to both the selected material and peer-context rows, and a regression check guards that propagation. Debug and Release builds passed again with 0 warnings and 0 errors.

Review of the exported high- and low-consistency reports found that the low-coverage material displayed both its established profile score and a second summary-only repeatability score. The handoff now uses the existing profile consistency score for rating and display, while Verified Material Summary remains authoritative for CV, sample coverage and variation-review evidence. This preserves historic ranking values and removes the contradictory dual score.

The same manufacturer-facing review identified an obsolete v36 header, an unexplained Video Planner hook, two visually distinct but incompletely labelled radar averages and a misleading AI label. The report now shows the current platform version, omits the editorial video hook, names both material-family and manufacturer average lines, and describes its review as deterministic local analysis without external AI/LLM use. Debug and Release validation builds completed with 0 warnings and 0 errors.

User acceptance confirmed the revised Material Engineering Report and Verification Center completed with all checks passing. Follow-up review found that the other report-selector options still converge on the same generic fallback output; distinct report contracts and regression checks are queued as v41.7 Report Portfolio Differentiation.

## v41.5.0 - Governed Intelligence Handoffs

Introduced one non-calculating handoff service over the already established Engineering Advisor, Consistency, Context, Peer Position and Alternatives outputs. Canonical report HTML now declares the handoff source boundary, and the methodology whitepaper documents how reports and editorial planning reuse verified interpretations.

Recommendation-created Video Planner rows now preserve canonical MaterialID, manufacturer identity and the existing EngineeringScoreProfile axes instead of treating the recommendation placement score as the overall engineering score. The persistent VideoIdeaQueue gains its MaterialId column through the existing additive migration pattern.

Verification Center covers composition, source ownership, Video Planner transfer, whitepaper governance and report-payload readiness. Debug and Release builds completed with 0 warnings and 0 errors and produced file version 41.5.0.0.

During visual acceptance, focused reports were found to prefer a stale selection held by another materials grid. The report workflow now treats the row displayed in Material Detail as canonical, exposes that label beside Selected Material Only and refreshes the focused preview on selection change. Report HTML branding was also switched from the obsolete PNG to the approved PDF/whitepaper JPG asset.

## v41.4.0 - Manufacturer & Category Positioning

Recommendation Detail now explains where a material sits among products from the same manufacturer and among materials in the same category. Each line exposes rank, comparable peer count, selected overall score and the existing-score group average within the active filtered dataset.

`EngineeringPeerPositionService` consumes existing `EngineeringScoreProfile.OverallScore` values keyed by canonical MaterialID. It does not read raw measurements or create another engineering scoring path. Duplicate recommendation rows do not inflate peer counts because the peer set is built once from unique visible materials before recommendation types are expanded.

Missing manufacturer/category classifications and missing overall scores remain explicit unavailable states. Verification Center covers deterministic rank/count behavior, missing-data honesty, UI presence and the aggregate v41.4 gate. Debug and Release builds completed with 0 warnings and 0 errors and produced file version 41.4.0.0.

The first visual review showed that positioning appeared only for the selected global winner. The purple Selected Material Intelligence card now receives the same filtered peer dataset, so manufacturer/category positioning follows the active MaterialID even when that material is not a global leader.

User acceptance confirmed both global-recommendation and active-material positioning, followed by a clean in-app Verification Center run.

## v41.3.0 - Price, Inventory & Manufacturer Context

Engineering recommendations now expose practical context beside the verified score, consistency and alternatives: canonical public MSRP, current stock state, linked spool count, remaining weight, storage locations and the active manufacturer intelligence record.

The new interpretation service consumes existing owners instead of rebuilding their calculations. Pricing remains owned by Materials, inventory quantities and remaining weight remain owned by `InventoryEngineService`, and manufacturer context remains owned by SQLite manufacturer records. Missing values are stated explicitly.

Initial acceptance testing found that the pre-existing recommendation `PricePerKg` resolver still fell back from an empty native MSRP to landed cost or an older DataRow projection. That could display a value as canonical MSRP after the user removed MSRP in Materials. Native Materials now remains authoritative even when its MSRP is empty, and the Verification probe preserves this no-fallback contract.

Verification Center now proves these three source boundaries, deterministic context text, UI availability and the aggregate v41.3 release gate. Debug and Release builds completed with 0 warnings and 0 errors and produced file version 41.3.0.0. User acceptance confirmed the Public MSRP wording, correct missing-price behavior and a clean in-app Verification Center run.

## v41.2.0 - Consistency & Outlier Intelligence

Extended Engineering Advisor with repeatability context sourced only from Verified Material Summary. The new service reads the already-calculated coefficient of variation and sample count for tensile and impact orientations, then reports measurement-set coverage, average/highest CV and whether each set has useful specimen coverage. It does not recalculate raw measurements or alter the established EngineeringScoreProfile.

Variation bands produce review guidance rather than automatic exclusions. A high-CV orientation is identified as a summary-level review signal, but the UI and prompt state that an individual specimen requires a traceable test-specific failure reason before it can be treated as an outlier. Missing or single-specimen sets remain visible as insufficient repeatability evidence instead of receiving inferred confidence.

Recommendation Detail now shows the status, repeatability evidence and outlier-review note beside the existing advisor and alternative intelligence. Verification Center adds deterministic stable, high-variation and limited-evidence probes plus a v41.2 aggregate release gate.

Initial acceptance testing exposed a scope mismatch rather than a data defect: the Recommendations tab sits under Material Detail, but its winner lists are global rankings and did not follow the active material selection. A dedicated Selected Material Intelligence card now binds directly to the active MaterialID and always shows that material's score coverage, verified-summary repeatability and outlier-review context. The unchanged ranking lists are explicitly labelled global.

The first visual acceptance pass then exposed a dual-grid selection precedence defect: the Material Detail header showed the newly selected material while a recommendation refresh reused an older selection retained by the other materials grid. Material Detail now records its displayed DataRow as the canonical UI context, and recommendation refresh uses that row before any grid fallback. A deterministic verification probe preserves this precedence contract.

Debug and Release builds completed with 0 warnings and 0 errors and produced file version 41.2.0.0. The user then confirmed a clean in-app Verification Center run, completing the v41.2 acceptance gate.

## Operational validation - Production FTPS Publishing (2026-07-21)

The explicit-FTPS publishing workflow delivered in v40.18.1 has now completed its previously pending live-server validation. The production connection succeeded with required TLS and a trusted Let's Encrypt certificate for `www.iskort.is`; passive-mode transfers also succeeded after the server data-port range became available.

A complete Website Export publish created the timestamped remote backup, replaced the live website package and preserved the compatibility route from `manufacturers/index.html` to the canonical `index.html#manufacturers` portal. This closes the external operational dependency recorded by v40.20.0. No new application build was created for this documentation-only validation record.

## v41.1.0 - Comparable Alternatives & Hidden Gems

Extended the explainable advisor from describing a selected recommendation to finding actionable alternatives within the same filtered recommendation context. The service consumes already-calculated recommendation scores and engineering axes; it does not introduce another scoring or measurement pipeline.

The closest alternative favors the same material family and similar five-axis profile. Value hidden gems retain near-peer recommendation performance while using the canonical native MSRP USD/kg value. Specialist alternatives surface a material with a meaningful axis advantage and state the corresponding trade-off. Missing price data remains valid and is shown as unavailable rather than inferred.

Recommendation Detail now presents up to three non-duplicate alternatives with score, price, gain and trade-off. Debug and Release build validation is recorded in the current build notes; in-app Verification Center confirmation remains pending.

Initial visual testing exposed an initialization-order defect: cached recommendation rows were created before native pricing records were hydrated, so MSRP appeared only after a filter forced a rebuild. A coalesced post-load refresh now rebuilds every native-material-dependent intelligence surface once pricing is available and repeats after relevant edits or undo operations.

The user confirmed MSRP now appears on the initial Engineering Advisor view and the complete in-app Verification Center run finishes normally.

## v41.0.1 - Advisor Locale Verification Fix

The first v41.0.0 in-app run passed 169 of 171 checks. The direct failure was limited to the advisor comparison probe: the correct Icelandic UI text contained decimal commas (`4,0` and `-12,0`), while the test expected English decimal points. The aggregate v41 gate failed only because it depended on that probe.

The advisor result now exposes structured comparison deltas and axis identities. Verification checks those numeric fields directly, while human-facing text remains correctly localized. This removes culture from the test contract and gives future advisor consumers typed comparison evidence.

The user confirmed the corrected in-app Verification Center run reports 171 / 171 PASS.

## v41.0.0 - Explainable Engineering Advisor

Started the v41 Engineering Intelligence cycle with an additive explanation layer over the existing Recommendation Engine. The advisor consumes the same five-axis engineering score profile already used by recommendations and produces evidence, trade-off, coverage and nearest-alternative context. It does not calculate tensile, impact, stiffness, consistency, layer adhesion or overall scores.

Recommendation Detail now exposes these explanations directly, and its copy/paste ChatGPT prompt includes the deterministic context. API connectivity remains a future optional presentation layer and is not required for this build.

Debug and Release builds completed with 0 warnings and 0 errors and produced file version 41.0.0.0. In-app Verification Center confirmation is pending user testing.

## v40.20.1 - Pricing Filter Synchronization Fix

Corrected the shared website-filter bridge rather than adding a second pricing calculation path. Pricing controls previously emitted `change`, while the canonical Filament Database engine listens for `input`; the visible selection therefore never triggered filtering. Pricing multi-selects also lacked the existing Database click-to-toggle handler and fell back to the browser's single-selection click behavior. Both tabs now use the same input and toggle contract while retaining one canonical filter state.

Debug and Release builds completed with 0 warnings and 0 errors and produced file version 40.20.1.0.

The first in-app v40.20.1 run confirmed both Pricing filter contracts but exposed a v40.20.0-specific identity predicate. The release gate now compares the active BuildInfo version/code with assembly and informational metadata, preserving a meaningful identity check across patch builds.

## v40.20.0 - Platform Integration & Release Readiness

Converted the final v40 integration audit into executable Verification Center contracts. Preview and Production are rendered from the same active SQLite template and compared after removing only their explicit mode header. The release contract also verifies all website portal routes, release identity, manufacturer redirect behavior and the complete HTML/redirect/whitepaper package manifest. A combined local-release gate now fails when any core Engineering, Experimental, Website, Reporting, workspace or identity dependency fails.

The first in-app v40.20 verification run passed 162 of 165 checks. It exposed two stale verification assumptions rather than website-output defects: the Pricing compact row now has a combined class, and the generated Preview/Production header follows the governed terminology marker instead of being the first line. Both contracts were corrected at their source, and the aggregate gate will now follow them. Nullable flow was also tightened across the affected UI, export, report, experimental and AI paths.

Release and Debug builds completed successfully with 0 warnings and 0 errors. Live FTPS transfer remains an external operational validation pending passive-port access and is not misrepresented as a local software PASS.

## v40.19.1 - Pricing & Value Portal Tab

Moved the three existing pricing intelligence surfaces into a dedicated canonical portal route without rewriting their calculation or rendering logic. The portal transform preserves the original pricing element IDs and extracts each section exactly once. A mirror filter surface uses unique IDs and synchronizes category, material, variant, reinforcement, colour, manufacturer, product line, MSRP range, pricing availability and search state with the original Filament Database controls in both directions.

Consolidated the conflicting strategic roadmap lists into `Docs/Roadmaps/MASTER_ROADMAP.md`. The cleanup reconciles the two v39 definitions, records Cost Analytics and website/deployment foundations where they were actually delivered, restores v41 Engineering Intelligence as the next major milestone, and turns the older top-level roadmap into a pointer rather than a competing source.

## v40.19.0 - Experimental Website Analytics

Extended the existing series-level Website publication control into a full Experimental Engineering Lab. A new Website service projects stored calculated result fields and canonical Experimental Analytics rankings into a deterministic payload, validates identity/baseline/value/ranking safety, and renders the same responsive dashboard, five SVG chart types and accessible result table into Preview and Production. The previous MainWindow-owned experimental website table renderer was retired; browser code owns presentation only and contains no tensile, impact, stiffness or scoring formulas.

## v40.18.1 - Explicit FTPS Publishing Fix

Corrected the v40.18.0 transport assumption after confirming the FileZilla profile uses explicit FTP over TLS on port 21 rather than SSH/SFTP. Publishing now requires TLS, validates the server certificate and uses passive data connections while preserving the backup-first staged replacement workflow.

## v40.18.0 - Secure SFTP Website Publishing

Website Export can now publish a validated Production package directly to the configured SFTP root. The workflow verifies the SSH server identity, protects the password with Windows Credential Manager, backs up existing remote files, stages and size-validates every upload, then replaces the main website, manufacturer redirect and methodology whitepaper. A failed replacement attempts rollback from the timestamped remote backup.

## v40.17.4.4 - Website Export Folder Persistence

Extended the existing local workflow-preference architecture with the selected Website Export root folder. Choosing a folder saves it immediately; the next application session restores it when the directory remains available. Invalid or removed directories fall back through the established export-folder resolution without blocking startup or export.

## v40.17.4.3 - Manufacturer Relative Redirect Fix

Corrected the v40.17.4.2 redirect routing so Preview remains genuinely local and Production remains portable. The preview companion now opens `../index-test.html#manufacturers`; the production companion opens `../index.html#manufacturers`. The public iskort.is address remains canonical metadata and is no longer used as the immediate redirect destination.

## v40.17.4.2 - Manufacturer Redirect Export Cleanup

The separate manufacturers-page export controls have been retired. Every canonical website export now owns a small backwards-compatible redirect companion: Preview writes only `manufacturers/index-test.html`, while confirmed Production writes `manufacturers/index.html` and backs up any existing production file. Both point old manufacturer URLs to the Manufacturers section in the main website at `https://iskort.is/3dp/index.html#manufacturers`.

## v40.17.4.1 - Manufacturer Terminology Verification Fix

Corrected the underlying verification predicate rather than changing working website output. The legacy cleanup removes only an element whose exact class is `manufacturer-cta`; the previous check searched for that text as a substring and therefore confused the valid v40.17.4 classes `manufacturer-cta-row` and `manufacturer-cta-primary` with the obsolete block. The gate now verifies exact legacy-class removal and explicitly preserves the current submission CTA.

## v40.17.4 - Manufacturer Material Submission Workflow

Extended the v40.17.3 manufacturer outreach surface into a structured first-stage intake experience. The canonical website renderer now builds a validated manufacturer form, generates an enquiry reference in the visitor's browser and prepares a complete email to `iskort@iskort.is`. A clipboard fallback preserves the same structured payload when `mailto:` is unavailable. No public endpoint, SMTP dependency, web database or direct SQLite write path was introduced; the form delivery layer can later be replaced by a governed server endpoint without changing its field contract or user interface.

## Repository layout and generated-output cleanup

Consolidated repository-control files at the project root and cleared accumulated reproducible Visual Studio and .NET build output. Active IDE tooling may recreate small ignored cache files after cleanup. Root `.gitattributes` now governs line-ending normalization and binary asset handling across the complete repository, while the expanded root `.gitignore` replaces the older nested Visual Studio template. Documentation SVG assets were deliberately retained because they remain candidates for governed whitepaper and export use.

## Repository hygiene - generated-file protection

Added repository-level ignore rules for generated builds and machine-local state. Existing `.vs` cache entries were removed from Git tracking while remaining available locally, closing the gap where a future commit could otherwise publish Visual Studio indexes, user state or local build artifacts.

## Repository licensing update - GPL-3.0-only

The repository's original application source is now distributed under GNU General Public License v3.0 only. The canonical unmodified GPLv3 text is stored at the repository root, .NET project metadata declares `GPL-3.0-only`, and third-party package licenses are recorded separately. The About dialog identifies the license and no-warranty status, while the license and third-party notice files are copied beside the built executable. The v40.17.3 functional behavior and build identity remain unchanged.

## v40.17.3 - Manufacturer Outreach & Submission Portal

The native Manufacturers portal now serves both audiences it was designed for: visitors can explore verified manufacturer intelligence, while filament manufacturers receive a clear explanation of the testing programme, participation value and submission route. The outreach surface uses live database scope where available, links into the governed Methodology Portal and Engineering Whitepaper, and keeps independent, data-driven results separate from paid placement. Coverage tiles and manufacturer tested counts consume Verified Material Summary by canonical MaterialID; website links no longer imply completed testing.

## v40.17.2 - Manufacturer Best Value Display Detail

Improved the Manufacturers website Best Value card so the selected material shows its actual MSRP per kg, engineering score and value score rather than only the derived ratio.

## v40.17.2 - Manufacturer Best Value Pricing Source Fix

- Fixed Manufacturer Engineering Intelligence so **Best Value** reads canonical SQLite `MsrpUsdPerKg` by `MaterialID`.
- Added `LandedCostUsdPerKg` and projected DataRow column aliases only as backwards-compatible fallbacks.
- Aligned the manufacturer value calculation with the public Website Pricing & Value definition: overall engineering score divided by MSRP USD/kg.
- No schema, measurement, report, whitepaper, or material calculation changes.

## v40.17.0 - Manufacturer Engineering Intelligence

- Added verified manufacturer engineering intelligence to the native website Manufacturers tab.
- Each manufacturer now exposes strongest tensile, impact and stiffness materials, best layer adhesion, best overall profile and best value when pricing is available.
- Added complete/partial engineering coverage, average overall score, Top 3 materials and material-family coverage.
- All values are generated from SQLite-backed materials, verified Material Summary results and the canonical EngineeringScoreProfile.
- Added Verification Center coverage for the manufacturer intelligence renderer.

## v40.16.2 - Manufacturer Selection Action Fix

- Duplicate, Archive/Restore and Delete now resolve the active manufacturer from SelectedItem, CurrentItem, CurrentCell or SelectedCells.
- Manufacturer actions now work after selecting any editable cell, matching the grid's CellOrRowHeader selection mode.
- Added clear status feedback when no manufacturer is selected.
- Archive/Restore refresh is deferred until the active DataGrid edit transaction is complete.
- No changes to SQLite schema, website payload, calculations, reports or whitepaper generation.

## v40.16.1 - Manufacturer Website Source Sync Fix

- Manufacturers portal now consumes the native SQLite-backed material collection instead of the potentially stale legacy Materials DataTable.
- Material totals and manufacturer groupings now update immediately after native material additions.
- Active SQLite manufacturer profiles are included even when they do not yet have linked materials.
- Preserved the canonical website DATA pipeline, legacy standalone manufacturers export, and all reporting/whitepaper workflows.

## v40.16.0 - Native Manufacturers Website

- Replaced the placeholder For Manufacturers portal page with a native Manufacturers tab in the canonical single-file website export.
- Generates manufacturer cards from active SQLite manufacturer profiles and canonical material rows.
- Adds live material counts, tested coverage, material-family counts, product-line tags, manufacturer metadata, profile content, website links and review links.
- Preserves the separate legacy manufacturers export for backwards compatibility while making the main website portal the preferred publication surface.
- Added Verification Center gates for the native manufacturers portal and SQLite profile binding.
- No changes to measurement calculations, Material Summary, reporting or database schema.

## v40.15.3 - Manufacturer Edit Transaction Safety

Corrected the manufacturer editor transaction lifecycle. The view is no longer refreshed from `CellEditEnding`; edits commit normally and auto-save remains owned by `ManufacturerRecord.PropertyChanged`.

## v40.15.2 - Manufacturer Grid Editability Fix

Corrected the Manufacturers manager so profile fields can be edited directly. The grid now participates in the platform's established editable workflow while preserving all v40.15.1 build fixes.

## v40.15.1 - Manufacturer Filter Build Fix

- Fixed the MainWindow compile error caused by the new manufacturer view filter method sharing the generated XAML field name `ManufacturerFilter`.
- Renamed the native Manufacturers tab predicate to `ManufacturerProfileFilter`.
- Preserved the legacy hidden Materials import-cache ComboBox named `ManufacturerFilter` and all existing filtering behavior.
- No SQLite schema, manufacturer data model, website payload or export workflow changes.

## v40.15.0 - Manufacturer Knowledge Platform

- Expanded the native SQLite Manufacturers entity into a full knowledge profile.
- Added the Manufacturers WPF manager with CRUD, archive/restore, search, filtering and auto-save.
- Enriched manufacturer website payloads with native profile content while preserving material-derived counts and links.
- Increased SQLite schema governance to version 21.
- Preserved existing website preview/production export compatibility.

## v40.14.5 - Whitepaper Logo Rendering Fix
- Replaced the PDF-specific logo asset with the newly supplied 801 x 482 baseline JPEG.
- Removed the previously generated/corrupted PDF logo image from the whitepaper asset path.
- Preserved the existing `3dp-iceland-labs-logo-pdf.jpg` asset contract so the cover, running header and verification pipeline continue to use the same canonical path.
- Confirmed the replacement JPEG matches the native PDF renderer's fixed 801 x 482 image dimensions and RGB JPEG embedding path.
- No SQLite schema, calculations, Material Summary, Website payload, report model or whitepaper layout changes.
- Updated application identity to v40.14.5 WHITEPAPER-LOGO-RENDERING-FIX.

## v40.14.4 - Engineering Whitepaper Professional Edition
- Converted the whitepaper to a white/light-gray print-ready engineering handbook theme.
- Fixed body text entering the running header by introducing a reserved header zone and tighter pagination threshold.
- Integrated the newly supplied 3DPIceland Labs logo on the cover and all standard pages.
- Added professional cover, contents, typography, spacing, callouts, engineering table, vector architecture diagram and explanatory confidence graph.
- Added reusable SVG source figures under `Assets/Documentation/`.
- Preserved the canonical SQLite -> native calculations -> verified Material Summary -> publication architecture.
- Updated application identity to v40.14.4 ENGINEERING-WHITEPAPER-PROFESSIONAL-EDITION.

## v40.14.3 – Engineering Whitepaper First Edition

Primary objective: turn the Documentation Engine output into a substantial, usable engineering handbook.

Delivered a fully expanded methodology corpus and a pagination-aware native PDF renderer. The website/download filename remains stable at methodology version 1.0, while the platform build advances to v40.14.3.

## v40.14.2 – Manufacturers Export Build Fix

- Corrected the manufacturers export log after the v40.14.0 whitepaper integration.
- Build target: restore successful compilation without changing export behavior.

## v40.14.2 – Documentation Engine Build Fix

- Fixed CS0246 in `DocumentationEngineService.cs`.
- Added the missing `System.IO` namespace required by `Stream` and `MemoryStream`.
- No functional changes to the Documentation Engine or whitepaper output.

﻿
## v40.14.0 – Documentation Engine & Engineering Whitepaper
- Added native documentation model and PDF whitepaper generator.
- Added manual whitepaper export and automatic website-side PDF publication.
- Linked Methodology Portal to the generated methodology v1.0 PDF.
- Added documentation integrity and whitepaper verification gates.
## v40.13.0 – Methodology Portal

- Replaced the Methodology portal placeholder with Level 2 Engineering documentation.
- Added tensile, impact and stiffness procedures, formulas, constants and video links.
- Added printing standard, statistics/confidence, limitations, FAQ and v40.14.0 whitepaper handoff.
- Added embedded methodology resource and Verification Center gates.

## v40.12.2 – Pricing & Value Terminology Cleanup

- Completed website-only tensile terminology mapping in Performance vs Price and Value Rankings.
- Updated metric selectors, chart labels, axis text and tooltips to use Tensile Strength and Layer Adhesion Strength.
- Preserved canonical WPF, SQLite and calculation field names.
- Added Website pricing terminology mapping verification.

## v40.12.1 – Website Terminology & Navigation Cleanup

- Removed the redundant manufacturers CTA from the main database header.
- Added website-only tensile terminology mapping for chart title, legend and tooltips.
- Renamed flat tensile display to Tensile Strength and upright tensile display to Layer Adhesion Strength.
- Updated Layer Adhesion Ranking wording while preserving canonical native field names.
- Added Website terminology mapping verification.

## v40.12.0 – Native Website Navigation Foundation
**Primary objective:** establish the tab-based website portal architecture without redesigning the existing database surface.

The release keeps one canonical SQLite-backed template and one preview/production export pair. A post-render portal transform wraps the existing website, moves Experimental content into its own page and introduces the future Manufacturers and Methodology surfaces.

## v40.9.4 – Experimental CV Pipeline Fix
- Confirmed ResultsService correctly returns coefficient of variation as a ratio (`1.0 = 100%`).
- Fixed Experimental measurement editors to convert that ratio to percentage points before display and downstream aggregation.
- Experimental Measurement Editor, Results Table and Dashboard now share one CV unit: percentage points.
- Example: raw ratio `1.226` now displays as `122.60%`, and correctly triggers the 15% high-variation threshold.
- No tensile, impact, stiffness, average or standard-deviation formulas were changed.

## v40.9.3 – Experimental Dashboard Canonical CV Fix
Dashboard quality status and Highest CV were aligned directly with the canonical Results Table collection.

## v40.9.1 – Experimental Dashboard CV Threshold Fix
- Corrected the Experimental Dashboard CV threshold unit mismatch.
- Source of truth: v40.9.0 Experimental Engineering Dashboard.

## v40.8.0 – EXPERIMENTAL-CHARTS-VISUALIZATION

Primary objective: turn v40.7 Experimental Analytics into live, readable visual comparisons while preserving canonical data and calculation ownership.

## v40.7.1 – EXPERIMENTAL-IMPACT-FLAT-DELTA-FIX

Primary objective: restore the missing Impact Flat Δ Baseline value in Experimental Results.

- Renamed the row display property to `ImpactFlatDeltaDisplay`.
- Updated the WPF column to an explicit one-way binding.
- Verified a 6.0 versus 5.0 baseline probe produces +20%.


## v40.7.0 – EXPERIMENTAL-ANALYTICS-ENGINE
Primary objective: turn native Experimental Results into transparent per-Series analytics and recommendations.
﻿
## v40.6.1 – Experimental Series Context Reset Fix
- Made Experimental Series switching an atomic context transition.
- Clears stale Run selection, measurement editors and Results before loading the selected Series.
- Added a version guard against out-of-order deferred WPF callbacks.
- Empty Series now remain fully empty; returning to populated Series immediately rebinds the first Run.
- No SQLite schema or calculation changes.

## v40.5.9 – Experimental Measurement Verification Gate

Verification foundation completed before the Experimental Results Engine milestone.


## v40.5.8 – Experimental Validation Build Fix

- Fixed CS0019 in `IsValidMeasurementCellText`.
- The DataGrid lookup now uses a shared `DependencyObject` reference instead of applying `??` directly between `DataGridCell` and `TextBox`.
- Experimental input limits from v40.5.7 remain unchanged.

## v40.5.2 – Experimental Input Visibility & Rebind Fix

- Replaced shared filtered measurement CollectionViews with fixed per-RunID editor row lists.
- Tensile and Impact Upright/Flat rows remain visible throughout editing and tab changes.
- Run selection now binds all four editors deterministically to the selected RunID.
- Deferred calculations capture the edited row's RunID and commit the DataGrid edit before calculation and persistence.
- Preserves the v40.5.0 specialized editors and native ResultsService calculation paths.


## v40.5.0 – Experimental Measurement Editor Redesign
- Replaced the generic experimental measurement grid with dedicated Tensile, Impact, Stiffness and Layer Adhesion tabs.
- Clarified Stiffness entry as Revolutions plus Degrees for one specimen.
- Retained 10-sample statistical capacity for tensile, impact and layer adhesion.
- Removed in-transaction DataGrid refresh that caused Impact entry crashes.
- Added Verification Center coverage for the redesign and edit-transaction safety.
## v40.4.1 – Experimental Native Measurement Context

- Replaced generic five-result entry with canonical raw measurement sessions.
- Added 10-sample Tensile Upright/Flat, Impact Upright/Flat and Layer Adhesion entry.
- Added one-specimen Stiffness entry using revolutions and degrees.
- Reused ResultsService and native Settings values for MPa, kJ/m², statistics, CV and confidence.
- Upgraded ExperimentalMeasurements storage to schema version 18.

## v40.4.0 – Experimental Measurement Entry

- Added canonical RunID-linked measurement storage for Tensile, Impact, Stiffness and Layer Adhesion.
- Added five-sample entry, automatic count and average, persistence and verification gates.
- Preserved separation from normal MaterialID measurement rows.

## v40.3.1 – Experimental Deferred Run Refresh Fix

- Fixed intermittent `InvalidOperationException` at `MaterialExperimentsGrid_PreviewMouseLeftButtonDown`.
- Child-run view refresh is now conditional and deferred until the current WPF input/edit transaction is complete.
- Preserved v40.3.0 Test Series & Runs functionality and schema version 16.

## v40.3.0 – Experimental Test Series & Runs

- Added parent/child Experimental Test Series and Run architecture.
- Added SQLite-backed ExperimentalRuns with canonical RunID and SeriesID.
- Added multi-run CRUD UI, baseline selection and verification gates.
- Preserved MaterialID as the canonical material link and isolated experimental runs from native measurement rows.

## v40.2.10 – Experimental Active Notification Fix
- Added property-change notification for Experimental Active and Updated UTC fields.
- Checkbox state now redraws immediately without requiring Find/filter refresh.
- Added Experimental active notification verification gate.

## v40.2.9 – Experimental Row Context Fix

- Removed `MaterialExperimentsGrid` from the legacy workflow-grid click handler.
- Added a dedicated experimental cell handler that remembers the current record without forcing invalid row selection.
- Duplicate and Delete now use the current cell record and a stable last-clicked record fallback.
- Active toggles on the first click and persists directly to SQLite.
- Material, Experiment, Baseline, Value, Unit and Notes remain editable through normal WPF DataGrid editing.

### What to Test
1. Click any cell in an experimental record, then Duplicate.
2. Click any cell in an experimental record, then Delete.
3. Toggle Active on and off with one click.
4. Edit all dropdown and text columns.
5. Test Clear Filters and restart the app to verify persistence.
6. Run Verification Center and confirm Overall PASS.

## v40.2.8 – Experimental Editing Restore

- Restored editable Experimental cells by using `SelectionUnit=CellOrRowHeader`.
- Preserved safe Delete row resolution through current-cell fallback.
- Updated Verification Center expectations for the editable selection model.

## v40.2.7 – Experimental Build Fix

- Fixed CS0165 in Verification Center by declaring the Experimental DataGrid reference before evaluating editability checks.
- No runtime workflow, SQLite schema, or data-model behavior changed.

## v40.2.7 – Experimental Build Fix

- Changed Experimental Testing grid to full-row selection so WPF row selection and editable cells use a compatible selection model.
- Delete now resolves either SelectedItem or CurrentCell.Item and gives clear feedback when no record is selected.
- Clear Filters suppresses duplicate change events and defers collection-view refresh until active DataGrid edits are committed.
- Added Verification Center gate for experimental row-selection and filter safety.
- SQLite schema and canonical MaterialID links are unchanged.

## v40.2.5 – Experimental ComboBox Edit Fix

- Fixed `InvalidOperationException` when selecting Material, Experiment or Baseline in the Experimental Testing grid.
- ComboBox column ItemsSource collections are now initialized once and remain stable during edit transactions.
- `CellEditEnding` no longer calls `RefreshExperimentalChoices()` or refreshes the active `ListCollectionView`.
- Display labels are updated only for the edited row before explicit SQLite persistence.
- Added `Experimental ComboBox edit safety` Verification Center gate.

## v40.2.4 – Experimental Post-Add UI Fix
Experimental records were already being created and saved, but a delayed DataGrid selection/scroll callback could throw InvalidOperationException afterward. The callback was removed and row display is now handled only by ObservableCollection notification.

## v40.2.3 – Experimental Collection Reentrancy Fix

- Removed SQLite save/UI refresh re-entry from `ObservableCollection.CollectionChanged`.
- Add and Duplicate now mutate the collection, persist explicitly, and update the DataGrid in separate dispatcher phases.
- Add no longer calls `RefreshExperimentalChoices()` or `BeginEdit()` on the same stack as the collection mutation.
- Material/experiment display labels are assigned directly to the new record.
- Regression scope: no SQLite schema or canonical MaterialID changes.

### What to test
1. Open Experimental Testing and click Add Experiment repeatedly.
2. Confirm no InvalidOperationException occurs.
3. Select Material and Experiment, enter Value and Notes, then leave the cell.
4. Duplicate and Delete a record.
5. Restart and confirm persisted values.
6. Run Verification Center and confirm Overall PASS.

## v40.2.2 – Experimental Add Crash Fix
Runtime stability fix for creation of experimental records.

## v40.2.2 – Experimental Add Crash Fix

- Fixed blocked editors in the Experimental Material Manager.
- Experimental rows now use direct editable WPF DataGrid behavior instead of the legacy native workflow click handler.
- Added Verification Center coverage for grid editability.
- No schema or canonical MaterialID changes.

## v40.2.0 – Experimental Material Manager
Primary objective completed: user-facing management of MaterialExperiments.

# Build History

## v39.1.3 – MATERIALS-WORKFLOW-COLUMN-LAYOUT

**Primary objective:** Put the most frequently reviewed and edited Material fields near the left side of the grid.

- Website, YouTube, video status, test coverage, and Notes now follow Color.
- Diameter remains in the data model but is no longer shown in the daily Materials grid.
- Existing purchasing and inventory functionality is unchanged.


## v39.1.2 – PURCHASE-TO-MATERIAL-STORAGE-SYNC
Focused bugfix completing Storage Location transfer from Purchase Orders to Materials.

# v39.1.0 – Daily Workflow Live Status

## Primary objective
Remove daily workflow friction caused by stale counts, unclear filtered-result status, cramped startup layout, and low-contrast measurement entry cells.

## Changes
- Material counts now refresh from the current SQLite-backed native collection and active filtered view.
- Materials header shows active, archived, visible, and total counts where relevant.
- Selecting a filter that matches no materials now visibly reports `0 visible` instead of appearing to fall back to all rows.
- Header and footer workflow counts are refreshed together after material filter changes.
- Tensile and Impact sample cells now use dark text so values below 10 remain readable on yellow backgrounds.
- Default window size increased to 1700 × 960 while existing saved window preferences remain respected.
- Verification Center gains live-count and entry-contrast workflow checks.

## What to test
1. Start the application with no existing window preference and confirm the larger default workspace.
2. Confirm the header count matches the SQLite material count.
3. Filter by manufacturer and verify the Materials count shows visible and total rows.
4. Select `Not Tested` when every material is tested and confirm the grid becomes empty and shows `0 visible`.
5. Clear filters and confirm all rows return immediately.
6. Enter a Tensile value below 10 and confirm the value is readable on the yellow background.
7. Repeat the low-value contrast test in Impact.
8. Run Verification Center and confirm the new workflow checks pass.

---

# v38.5.0 – Purchasing Intelligence

## Primary objective
Use the existing SQLite-backed purchasing and inventory data to provide decision-support analysis without adding new persisted fields or changing the purchasing schema.

## Changes
- Added Purchasing Intelligence Report to the existing report selector.
- Added total-spend, average-order, shipping-share, top-supplier and supplier-concentration indicators.
- Added supplier, monthly-spend and category-spend tables.
- Added material-level landed unit price history in ISK.
- Added transparent rule-based recommendations for low stock, missing landed costs, concentrated supplier spend, historical price spread and unlinked inventory.
- Added a dedicated Verification Center check.

## Regression scope
No SQLite schema, Purchase Order editing, landed-cost allocation, inventory persistence, website export, or engineering calculation behavior was intentionally changed.
# v38.4.2.3 – Purchasing Report Executive Dashboards

- Upgraded Inventory, Purchase and Supplier Reports with executive summary dashboards.
- Inventory Report now highlights estimated value, remaining kg, total/opened/unopened spools and supplier count.
- Purchase Report now highlights total spend, shipping, VAT, average order and largest order.
- Supplier Report now highlights top supplier, top-supplier spend, supplier count, total spend and average order.
- Existing detailed tables, canonical HTML, PDF export, currency parsing and SQLite-backed calculations are preserved.

# v38.4.2.2 – Purchasing Report Currency Parsing Fix

- Fixed culture-dependent parsing in purchasing reports.
- Decimal-point exchange rates such as 149.70 are no longer interpreted as 14,970 under Icelandic culture.
- Supplier and Purchase report ISK totals now use normalized decimal values.
- No database schema or allocation-engine changes.

# v38.4.2.1 – Purchasing Reports Build Fix

- Fixed CS9006 in `PurchasingReportService.HtmlDocument`.
- Replaced the problematic interpolated raw string with a standard interpolated verbatim string.
- No purchasing-report behavior or data model changes.

## v38.4.2 – Purchasing Reports

Primary objective: reporting for the Purchasing Platform. Five reports now consume SQLite-backed purchasing and inventory data through the existing report export workflow.

## v38.4.1.7 – LANDED-COST-PERSISTENCE-FIX

- Persisted all calculated purchase-line cost fields in SQLite.
- Restored allocated shipping, tax, customs, fees, landed line/unit/kg values and allocation status after restart.
- SQLite schema advanced to v14 with backward-compatible column migration.
- Successful Calculate Landed Costs results now survive application restart.

## v38.4.1.6 – CURRENCY-RATE-LIVE-SYNC

- Purchase Order exchange rates synchronize immediately when Purchasing currency settings are edited.
- Orders no longer require re-selecting the currency to receive the updated ISK rate.
- Matching Purchase Orders are saved and refreshed after the Settings Manager edit transaction completes.
- Preserves v38.4.1.5 currency dropdown stability and all cost allocation behavior.

## v38.4.1.5 – CURRENCY-DROPDOWN-FIRST-LOAD-FIX

- Fixed an intermittent `InvalidOperationException` when a Purchase Order currency selection triggered a DataGrid refresh before the edit transaction had completed.
- Deferred Purchase Orders grid refresh until WPF reports that neither AddNew nor EditItem is active.
- Preserved automatic ISK exchange-rate fill and all v38.4.1.4 purchasing currency settings behavior.


## v38.4.1.4 – PURCHASE-CURRENCY-AUTOFILL

- Purchase Order currency dropdown sourced from Settings Manager.
- Automatic ISK exchange-rate fill on currency selection.
- Added configurable Purchasing currency rates and backward-compatible settings enrichment.


## v38.4.1.3 – COST-ALLOCATION-VALIDATION-FIX

- Enforced strict validation for manually selected By weight allocation.
- Blocked all cost and landed-cost calculations when an included line has missing weight.
- Added visible Cost Allocation Validation feedback and a Verification Center regression test.
## v38.4.1.1 – STARTUP-PREFERENCES-CRASH-FIX


## v38.4.1.2 – INVENTORY-COST-COLUMN-LOAD-FIX

- Fixed startup failure caused by Inventory spool SQL payload omitting `CustomsAmount`, `OtherFeesAmount`, and `LandedCostAmount` while the record mapper attempted to read them.
- Preserved v38.4.1 cost allocation and v38.4.1.1 preference safety behavior.

- Guarded window preference capture against invalid startup geometry.
- Added non-blocking handling for JSON serialization `ArgumentException`.
- Prevented preference saving from masking the original startup exception.
- Preserved all v38.4.1 Cost Allocation functionality.

## v38.4.0 – PURCHASING-PLATFORM-FOUNDATION

- Established the four-platform architecture: Materials, Purchasing & Inventory, Engineering, and Publishing.
- Added `InventoryCategory` to Purchase Order Lines with Filament, Printer, Equipment, Spare Parts, Consumables and Other.
- Mixed-category supplier orders can now be recorded exactly as purchased.
- Only Filament lines create Material and Inventory Spool records; non-filament lines remain safely recorded for future category modules.
- Added schema v12 foundations for Suppliers and Purchase Documents.
- Added ADR-001 through ADR-007, official domain/data-flow documents, platform roadmaps and a milestone record.
- Added Verification Center coverage for category integrity and invalid non-filament material links.
- Landed-cost calculations remain intentionally deferred.

## v38.3.3 – INVENTORY-DELETE-FIX

- Fixed Inventory delete crash caused by WPF `NewItemPlaceholder` being cast to `InventorySpoolRecord`.
- Inventory deletion now persists immediately and refreshes material quantities and summary data.

## v38.3.2 – RECEIVING-AUTOMATION

- Combined missing-Material creation and received-spool generation into one receiving action.
- Active Purchase Order edits are committed before processing.
- Newly created Materials are linked and persisted before Inventory Spools are generated.
- Storage location is copied to physical Inventory Spools, not the shared Material definition.
- Repeated execution remains duplicate-safe.
- No schema or landed-cost changes.

## v38.3.1 – PURCHASE-WORKFLOW-RECEIVING

- Added purchase lifecycle and receiving reconciliation.
- Purchase items no longer require an existing Material at order-entry time.
- Added Expected/Received quantities, receiving condition, storage location, material creation/linking, and received-spool generation.
- Optional documents are copied outside SQLite into the configured storage folder.
- Schema advanced to v11.

## v38.2.11 – INVENTORY-EDIT-COMMIT-FIX

Bugfix release for the v38.2.10 Inventory workflow polish. Prevents `InvalidOperationException` caused by refreshing the editable spool collection view during a DataGrid edit transaction.


## v38.2.10 – INVENTORY-POLISH-WORKFLOW

- Added Inventory search, filters and clear-filter workflow.
- Added alphabetical material display sorting and visible row count.
- Added status and low-stock color coding.
- Added Average Cost/kg summary and calculated Cost/kg column.
- Added specific missing-field validation and improved Add/Duplicate/Delete focus workflow.
- SQLite schema and multi-spool persistence architecture remain unchanged.

## v38.2.9 – INVENTORY-VERIFICATION-FIX

### Build Summary

- Corrected Verification Center for the multi-spool data model.
- Inventory Engine verification now compares calculated rows with active spool rows, not with every active material.
- Detects orphaned spool rows whose MaterialID no longer belongs to an active material.
- Row-level inventory data warnings remain visible but are non-blocking for the platform build gate.
- No database schema, persistence, UI, or calculation rules changed.

### What to Test

1. Build and confirm the header shows `v38.2.9 INVENTORY-VERIFICATION-FIX`.
2. Run Verification Center with the current inventory data.
3. Confirm Inventory Engine reports the active spool-row count and zero orphaned rows.
4. Confirm Inventory Validation is PASS even when a row has a review warning.
5. Confirm the warning remains visible in the Inventory grid Validation column.
6. Confirm Overall Verification is PASS when all structural tests pass.
7. Recheck Add Spool, persistence, remaining weight, and estimated value calculations.

## v38.2.8 – FIRST-SPOOL-DEFAULTS-FIX

- First spool inherits spool and purchase defaults from its selected material.
- Subsequent Add Spool rows remain blank; Duplicate is unchanged.
- No schema or calculation changes.

## v38.2.7 – INVENTORY-MATERIAL-CHOICE-FIX

- Refreshes Inventory material choices after Material Manager changes.
- Sorts the material dropdown alphabetically.
- Ensures Add Spool creates and reveals a row for the currently selected material.


## v38.2.6 – INVENTORY-PERSISTENCE-FIX
Persistence safety fix for the v38.2 multi-spool inventory model. Existing material rows are updated in place so spool records survive auto-save and application restart.
## v38.2.3 – INVENTORY-QUANTITY-VALUE-FIX

- Corrected Quantity scaling in Estimated Inventory Value.
- Defined Spool Weight and Remaining Weight as per-spool values.
- Quantity now multiplies total capacity, total remaining weight and estimated value.
- Updated Materials grid headers to display `g / spool`.
- Added regression test instructions for one, two and three spool scenarios.

# v38.2.2 – INVENTORY-VALUE-STATE-FIX
Inventory Estimated Value state correction and post-commit manual refresh fix.


Primary objective: make the v38.2 Inventory Engine update live after Material Manager changes without requiring an application restart.

# v38.2.0 – INVENTORY-ENGINE

Primary objective: activate the v38.1 purchasing fields as a verified inventory engine and summary UI.


## v38.1.0 – Purchasing & Inventory Foundation
Primary objective: establish the purchasing data model and UI foundation.

## v37.9.0 – WORKFLOW-COMPLETE Workflow Optimization Completion

- Completed the v37 Workflow Optimization milestone.
- Enabled explicit row and column virtualization with recycling on the four primary editable grids.
- Added pixel scrolling, viewport caching and deferred scrollbar-thumb scrolling for smoother navigation through full material lists.
- Removed remaining visible legacy development-version labels from page descriptions.
- Updated release identity, roadmap and project status; no database, calculation or export logic changed.

## v37.2.4.2 – UI-005B Build Identity Synchronization
- Fixed stale assembly informational metadata that caused the main header and splash screen to show v37.2.2.
- Centralized visible release identity through BuildInfo and synchronized assembly, file, product and documentation versions.
- Preserved all v37.2.4.1 non-blocking success feedback behavior.


## v37.1.5.2 – QOL-001A End-of-Text Caret Fix

- Fixed mouse caret placement in the blank area after text.
- Clicking to the right of the final character now places the caret at the true end of the value.
- Preserved precise in-text mouse editing and keyboard Select All behavior.

## v37.2.2 – WORKFLOW-003B Bulk Update Engine

- Replaced Fill Current Column Down with filter-aware Bulk Update.
- Added Remaining Visible, Current Filtered and All Rows scopes.
- Added affected-row preview and explicit overwrite warning.
- Preserved v37.2.1 single-cell workflow behavior.

## v37.2.1 – WORKFLOW-003A Workflow Restoration + Single Cell Productivity

- Primary objective: restore first-click editing while retaining stable single-cell productivity commands.
- Multi-selection remains removed.

## v37.1.9 – WORKFLOW-002I Reliable Single-Cell Copy/Paste Fix

- Ctrl+C now copies CurrentCell directly.
- Ctrl+V pastes the first clipboard cell into CurrentCell.
- No dependency on multi-cell selection.

## v37.1.8 – WORKFLOW-002G Persistent Multi-Cell Selection Fix

- Preserved native drag/Shift/Ctrl multi-cell selection after mouse-up.
- Delayed first-click editing until a stationary plain mouse-up.
- Prevented edit activation from collapsing a selected range.
- Restored usable SelectedCells state for Ctrl+C, Ctrl+V and Ctrl+D.

## v37.1.6 – WORKFLOW-002F Multi-Cell Selection & Clipboard Command Fix

- Added native DataGrid Copy/Paste command bindings.
- Added Shift-click rectangular selection and Ctrl-click additive selection.
- Made clipboard copy persistent for external applications.
- Preserved navigation and stability fixes.

## v37.1.5 – WORKFLOW-002E Workflow Clipboard Shortcut Routing Fix

- Added routed keyboard handlers with handled-events support for workflow grids.
- Restored Ctrl+C, Ctrl+V and Ctrl+D while cell editors have focus.
- Improved paste anchoring and clipboard error handling.
- Preserved v37.1.4 navigation crash safeguards.

## v37.1.4 – WORKFLOW-002D
- Startup Row/Cell Selection Compatibility Fix.
- Workflow grids now use CellOrRowHeader selection.

## v37.1.2 – WORKFLOW-002B
DataGrid Selection Initialization Crash Fix.

## v37.0.7 – WORKFLOW-001H Native Captured-Popup Outside-Click Fix
- Replaced global `InputManager.PreProcessInput` handling with `PreviewMouseDownOutsideCapturedElement`.
- Outside clicks close an open workflow ComboBox and remain unhandled for normal WPF routing.
- Preserved full MainWindow source and removed no application features.

## v37.0.6.1 – WORKFLOW-001G-A Native Outside-Click Routing Fix

- Removed the synthetic Win32 `SendInput` click replay introduced in v37.0.5.
- An outside click now closes the open workflow ComboBox and releases mouse capture without marking or recreating the click.
- Preserves the user's original WPF input route for grid cells, filters, buttons, tabs, title bar and close button.
- Keeps the v37.0.4 grid/tab workflow behavior while removing the v37.0.5 regression.

## v37.0.5 – WORKFLOW-001F Universal ComboBox Outside-Click Release
- Replaced target-specific ComboBox outside-click fixes with one global input recovery mechanism.
- Filters, buttons, tabs, grids and the window close button now receive the first click after an open workflow dropdown is dismissed.
- Preserved normal dropdown selection and Escape cancellation behavior.

## v37.0.4 – WORKFLOW-001E ComboBox Tab Navigation Release
- Fixed tab headers being blocked while a workflow Currency dropdown was open.
- Clicking a main or nested tab now dismisses the dropdown without changing its value and navigates with the same click.

## v37.0.3 – WORKFLOW-001D ComboBox Outside-Click Release

Completed the final ComboBox popup focus/capture fix for Daily Workflow Foundation.

## v37.0.2.1 – WORKFLOW-001C-A DataGridCellsPresenter Build Fix

- Added the missing `System.Windows.Controls.Primitives` namespace import required by `DataGridCellsPresenter`.
- Resolves the two CS0246 build errors introduced in v37.0.2.
- No runtime workflow behavior changes.

## v37.0.2 – WORKFLOW-001C Cross-Row ComboBox First-Click Fix

- Captures the original target item and column before committing the previous ComboBox editor.
- Handles the mouse event to prevent WPF from consuming the first click only for row selection.
- Reopens editing on the requested target cell through the Dispatcher after the previous edit state has closed.

## v37.0.1 – WORKFLOW-001B ComboBox Edit-State Fix
- Workflow bug fix for reliable one-click movement between dropdown, text, numeric, and different material rows.


## v37.0.0.1 – WORKFLOW-001A WorkflowPreferences Build Fix

- Added the missing `System.IO` namespace import to `WorkflowPreferencesService.cs`.
- Restores compilation for `Path`, `Directory`, `File`, and `IOException`.
- No runtime workflow behavior or database logic changed.

## v37.0.0 – WORKFLOW-001 Daily Workflow Foundation

- Began the v37 Workflow Optimization phase.
- Added persistent window size, position, and maximized state.
- Added persistent native DataGrid column widths.
- Added first-click editing and immediate ComboBox opening in daily native grids.
- No database schema, calculation, website, or report payload changes.

## v36.2.4.2 – STABILITY-004B Website Export Count Synchronization

- Main website export now resolves the exact native export row set once and uses it for the DATA block, validation, summary, manifest, status text, and data-quality counts.
- Fixed stale UI/cache counts (for example 176) being reported while 200 native materials were actually exported.
- UI cache count remains visible only as an informational diagnostic and is clearly labelled.
- No calculation, database schema, scoring, or filtering logic changed.

## v36.2.4.1 – STABILITY-004A SafeFileOperations Build Fix

- Fixed CS0246 in `SafeFileOperations.cs`.
- Added the missing `using System.IO;` directive so `IOException`, `Path`, `Directory`, and `File` resolve explicitly.
- No runtime behaviour, database schema, calculations, or export payloads changed.

## v36.2.4 – STABILITY-004 Platform Hardening
- Added atomic hardened writes for website/manufacturer/report/log/diagnostics exports.
- Added friendly file-lock, OneDrive, unavailable path, and permission diagnostics.
- Expanded System Diagnostics with process memory and automatic website routing paths.
- Completed the planned v36.2 stability milestone hardening pass.

## v36.2.3 – Full Manufacturer List Export

- Removed the legacy `.Take(10)` cap from the Manufacturers Website Export list.
- All active manufacturers are now rendered in descending material-count order.
- Kept existing archive filtering, native counts, responsive layout, and unrelated Top 10 report/dashboard lists unchanged.

## v36.2.1 – STABILITY-003A Manufacturers Visible HTML Refresh Fix

Corrected manufacturer preview/production HTML synchronization so displayed content and embedded export data use the same native dataset.

## v36.0 – STABILITY-001

Phase 1 stability release: live statistics synchronization, filter correctness, and report branding/count corrections.

# v34.5.2 WEB-PRICE-001
- Fixed stale website pricing caused by legacy PRICE_DATA template overlay.
- Native DATA pricing is now authoritative on every export.

## v34.5.1 – PRICE-002A Pricing Verification Severity Fix

- Changed pricing completeness from a blocking requirement to an optional coverage check.
- Added missing-pricing material names/MaterialIDs and coverage percentage to verification details.
- Kept invalid currencies, zero/negative values, missing weight on priced rows, and normalization errors as blocking failures.
- Centralized generated diagnostics version labels through BuildInfo.


## v34.5.0 – PRICE-002 Pricing Validation & Normalization

- Added USD/kg pricing normalization, optional price checked date, SQLite/Excel persistence, website payload pricing fields, and Verification Center pricing gates.
## v34.4.3 – PRICE-001C Centralized UI Version Display

- Added a shared `BuildInfo` source for application release labels.
- Main header and splash screen no longer contain hardcoded version text.
- Numeric UI version now comes from assembly informational metadata.
- Updated project version metadata to v34.4.3.
- No changes to pricing calculations, persistence, website export, or reporting behavior.

## v34.3.0 - BRAND-001 App Identity & Animated Splash Screen

- Adopted the new minimalist 3D printer / hidden “3” symbol as the permanent Windows application icon.
- Added a production multi-resolution ICO for Windows shell, taskbar, shortcuts and EXE branding.
- Added a WPF startup splash screen with animated blue extrusion trace and startup status text.
- Updated the application header to use the compact new symbol.
- Preserved the full 3DPIceland Labs wordmark for reports and document branding.
- No database schema, calculation, website-export or report-rendering logic changed.

## v34.2.6 - RELEASE-003B Header Version Fix

- Fixed visible application header version text.
- Header now matches the current release line instead of showing v34.2.2 RELEASE-002.
- Preserves RELEASE-003/003A website export and verification fixes.
- No calculation, website export, reporting, storage, or workflow changes.


## v34.2.6 - RELEASE-003B Header Version Fix

- Fixed Reporting Platform verification regression after native website export candidate fix.
- Partial/new materials without complete mechanical measurements are accepted as renderable report shells.
- Website export fix for newly added materials preserved.

# v34.0 LTS-001 - Long-Term Stability Release

- Stability baseline for daily use.
- No new feature scope.
- Preserves Engineering, Website and Reporting workflows.
- Adds stability docs, known limitations, usage-mode plan and regression audit.
- Previous verification baseline: v33.5 PASS, 70 / 70.


## v33.4.2 - REPORT-400B Intelligence Output Wiring Fix

- Added Engineering Intelligence section to selected Material Detail and Material Engineering reports.
- Added automatic Engineering Summary, Best/Weakest Feature badges and engineering highlight.
- Added Recommended Applications based on verified engineering score profiles.
- Added Rankings and Percentiles table for overall/tensile/impact/stiffness/consistency/layer adhesion.
- Preserved canonical HTML -> WebView2 PDF workflow so HTML and PDF match.
- No raw measurement consumption or engineering calculation changes.


## v33.3.1 - REPORT-302A Chart Output Wiring Fix

- Fixed the REPORT-302 chart output wiring issue where verification and CSS existed but chart sections were not inserted into the exported canonical HTML body.
- Material Detail Report now writes the engineering chart suite into `report.html`.
- Material Engineering Report now writes the engineering chart suite into `report.html`.
- HTML and PDF continue to share the same canonical HTML through WebView2 PrintToPdf.
- No calculation, storage, import/export, or raw measurement access rules changed.

## v33.3 - REPORT-302 Interactive Engineering Charts

- Added canonical HTML engineering chart visuals for report exports.
- Added SVG radar chart for selected material vs material/manufacturer average context.
- Added material-vs-average and manufacturer-vs-average comparison tables.
- Charts render in the same HTML report package and therefore appear in PDF through WebView2 PrintToPdf.
- Added Verification Center gates for report chart renderer, engineering chart payload, chart asset validation, and interactive report readiness.
- Preserved SQLite/MaterialID/verified Material Summary source-of-truth rules.

## v33.2 - DOCS-002 Strategic Master Roadmap

- Rebuilt `Docs/Roadmaps/MASTER_ROADMAP.md` as a strategic roadmap instead of a changelog/build-history duplicate.
- Added permanent Documentation Governance Rule defining separate roles for `CHANGELOG.md`, `BUILD_HISTORY.md`, `MASTER_ROADMAP.md`, `PROJECT_STATUS.md`, and `ARCHITECTURE.md`.
- Added platform progress, completed strategic milestones, current Reporting Platform focus, next planned releases, long-term platform vision, and architecture evolution timeline.
- Added v33.3-v34.x near-term roadmap for interactive report charts, product image library, report themes, comparison reports, manufacturer reports, test session reports, and batch export.
- Preserved runtime behavior, report package workflow, HTML Print Engine, Engineering Platform rules, and raw measurement blocking.

## v33.1 - REPORT-301 Report Package Polish

- Added polished report package folder workflow on top of the HTML Print Engine.
- Export Report Package now writes a dedicated package folder with `report.html`, `report.txt`, `manifest.txt`, `report-metadata.json`, and `assets/`.
- Export PDF now writes the same package folder plus `report.pdf` printed from canonical HTML.
- Added report metadata JSON for report key, title, material counts, canonical HTML path, and optional PDF path.
- Added Verification Center gates for report package folder standard, report package metadata, and report package naming.
- HTML remains the canonical report layout; PDF visual parity path preserved.
- No engineering calculations or raw measurement access changed.

## v33.0.1 - REPORT-300A HTML Print Engine Build Fix

- Fixed WebView2 print settings API usage for the installed WebView2 SDK.
- Replaced unsupported `PaperWidth` / `PaperHeight` with compatible page size settings.
- Preserved HTML Print Engine architecture: PDF export remains printed from canonical HTML.
- No engineering calculations, storage, import/export data flow, or raw measurement rules changed.

## v33.0 - REPORT-300 HTML Print Engine

- Replaced the user-facing PDF export path with a WebView2 HTML print engine.
- Canonical HTML is now printed directly to PDF instead of manually recreating the layout in a separate PDF renderer.
- Report package continues to include PDF, companion HTML, and manifest.
- No engineering calculations, storage, import/export data workflows, or raw measurement access rules changed.

## v32.1 - REPORT-201 Report Asset Pipeline

- Added `ReportAssetPipelineService` for shared HTML/PDF report asset verification.
- Added Verification Center gates for report asset pipeline, report asset coverage, and report asset manifest readiness.
- Preserved unified report rendering: HTML remains canonical and PDF export is preserved.
- Preserved Engineering Platform calculation rules and raw measurement blocking.

# 3DPIceland FilamentDB - Full Changelog

This changelog is maintained as the project-level build history. It is intentionally broader than the latest release notes and should preserve the major feature history across the application, Engineering Platform, Website Platform, Reporting Platform, and documentation system.

Maintenance rule: every build must add one new top entry here and keep the detailed build note under `Docs/BUILD_NOTES_<version>.md`.

## v32.0.3 - DOCS-001 Full Changelog Reconstruction

- Rebuilt `Docs/CHANGELOG.md` as a full project-history changelog instead of a partial recent-build list.
- Added backfilled historical entries for the major v19-v32 workstreams using the existing documentation set in this package.
- Added clear platform milestones for Reporting Platform, Website Platform, Engineering Platform, native input migration, reporting/PDF, YouTube research, and website export work.
- Added a documentation maintenance rule: every build must update CHANGELOG, BUILD_HISTORY, RELEASES, PROJECT_STATUS, ROADMAP / MASTER_ROADMAP, and the current build notes.
- No calculation, storage, import/export, website generation, reporting pipeline, or native input behavior intentionally changed.

## v32.0.2 - REPORT-200B Roadmap Documentation Sync

- Updated MASTER_ROADMAP and roadmap documents so the project no longer stops conceptually at v29.
- Documented completed v30 Reporting Platform foundation work, v31 production report work, and v32 unified rendering work.
- Preserved runtime behavior and Engineering Platform rules.

## v32.0.1 - REPORT-200A Unified Report Rendering Build Fix

- Fixed C# build error caused by newline inside a string in the unified rendering update.
- Preserved the unified report rendering architecture and canonical HTML report package workflow.

## v32.0 - REPORT-200 Unified Report Rendering Engine

- Added UnifiedReportRenderingService.
- Made HTML report package the canonical/master report layout.
- PDF export now writes a companion HTML source and manifest from the same report foundation.
- Added shared logo/image asset handling for HTML/PDF workflow.

## v31.1.2 - REPORT-101B PDF Layout Integrity Fix

- Fixed PDF overlap/layout corruption in the native PDF renderer.
- Improved page-flow spacing so report body no longer starts inside header/card content.
- Refreshed logo and application icon assets with white background handling.

## v31.1.1 - REPORT-101A PDF Visual Parity + Icon Fix

- Improved PDF output to better match the HTML report style.
- Embedded 3DPIceland Labs branding assets in report outputs.
- Updated application/taskbar icon asset handling.

## v31.1 - REPORT-101 Branded PDF + App Icon Refresh

- Added branded PDF renderer styling using the supplied 3DPIceland Labs logo.
- Refreshed app/taskbar icon from the provided logo.
- Added Verification Center gates for branded PDF renderer, report logo asset, and application icon asset.

## v31.0 - REPORT-100 Material Engineering Report

- Added MaterialEngineeringReportService.
- Delivered the first production report: Material Engineering Report.
- Report output includes material identity, engineering summary, metric sections, and peer context from verified report models.
- Added Verification Center gates for material engineering report payload and render readiness.

## v30.5 - REPORT-006 Report Export UI

- Added ReportExportUiService.
- Made native report templates visible in Reports / PDF Export.
- Added template selection, preview/export readiness, and Verification Center gates.

## v30.4 - REPORT-005 Native Report Templates

- Added ReportTemplateService.
- Added template payloads for Material Summary, Engineering, Comparison, Manufacturer, Test Session, and Printing Recommendation reports.
- Added template payload validation and render readiness gates.

## v30.3 - REPORT-004 Certificate Generator

- Added ReportCertificateGeneratorService downstream of PDF renderer.
- Added certificate payload validation and certificate issue readiness gates.
- Confirmed certificate generation consumes verified report models and PDF payload metadata only.

## v30.2 - REPORT-003 Native PDF Renderer

- Added ReportPdfRendererService downstream of ReportGeneratorService.
- Added PDF-ready document/payload models and PDF renderer verification gates.
- Preserved verified Material Summary as the Reporting Platform engineering data source.

## v30.1 - REPORT-002 Native Report Generator

- Added ReportGeneratorService and native report model generation.
- Added report model and payload verification gates.
- Confirmed Reporting Platform consumes verified Material Summary output only.

## v30.0 - REPORT-001 Native Reporting Data Pipeline

- Started Version 30 Reporting Platform.
- Added ReportingDataPipelineService as the first reporting subsystem.
- Reporting pipeline consumes verified Material Summary outputs only and blocks raw measurement consumption.
- Added Reporting Platform gates to Verification Center.

## v29.5.1 - DOC-002 Project Milestone & Master Roadmap

- Added v29 milestone documentation, MASTER_ROADMAP, and platform evolution notes.
- Documented Website Platform completion and v30 Reporting Platform foundation path.

## v29.5 - DOC-001 Platform Architecture Documentation

- Added dedicated Architecture documentation set.
- Formalized platform ownership, service map, data flow, dependency rules, and downstream consumption rules.

## v29.4 - WEB-005 Website Verification Suite

- Added WebsiteVerificationService and Website publish-readiness gate.
- Verification validates HTML, DATA, JSON serialization, required sections, MaterialID integrity, duplicate checks, and publish readiness.

## v29.3 - WEB-004 Native HTML Renderer

- Added WebsiteHtmlRendererService ownership for template DATA injection and render validation.
- Moved website HTML render validation into the Website Platform.

## v29.2 - WEB-003 Native Radar Generator

- Added WebsiteRadarGeneratorService.
- Added radar payload validation for selected rows, material average groups, reinforcement groups, and normalization readiness.

## v29.1 / v29.1.1 - WEB-002 Native Chart Generator + Verification UI Visibility Fix

- Added WebsiteChartGeneratorService for chart payload generation.
- Added chart payload coverage gates.
- Follow-up hotfix improved Verification Center visibility/readability for the Website Platform gates.

## v29.0 - WEB-001 Native Website Data Pipeline

- Opened Version 29 Website Platform.
- Website DATA generation now consumes verified Material Summary outputs.
- Added Website pipeline source verification gates and preserved existing website export workflow.

## v28.x - Engineering Platform Consolidation and Native Calculation Verification

- Consolidated Native Calculation Engine work from v27 into a platform-level Engineering Platform.
- Strengthened Material Summary Engine as the verified downstream output for website and reporting consumers.
- Added platform verification expectations and standardized diagnostics/reporting workflow.
- Established architecture rule that downstream systems consume verified summary outputs instead of raw tensile/impact/stiffness rows.

## v27.8.3.2.1 - Duplicate Helper Cleanup

- Cleaned duplicate-helper workflow and preserved native MaterialID/data integrity.

## v27.8.3.2 - Native Tensile Calculation Migration

- Migrated tensile calculation ownership into the native ResultsService verification path.

## v27.8.3.1 - Help Diagnostics Center

- Added/expanded Help diagnostics and Verification Center surfaces.

## v27.8.3 - Native Results Engine

- Added native Results Engine foundation for Material Summary and verified calculations.

## v27.8.2.4 - Smart SQLite Backup Triggers

- Added smarter backup trigger behavior around SQLite persistence.

## v27.8.2.3 - Automatic SQLite Backup Rotation

- Added SQLite backup rotation workflow.

## v27.8.2.2 - Autosave Dirty Flag Cleanup

- Cleaned autosave dirty-state behavior.

## v27.8.2.1 - Autosave Status Bar

- Added visible autosave/status feedback.

## v27.8.2 - Native Rating Engine

- Added native rating/scoring engine services.

## v27.8.1.1 - Material Detail Filter Commit Hotfix

- Fixed Material Detail filter commit workflow.

## v27.8.1 - Native Statistics Engine

- Added native statistics service foundation.

## v27.8.0 - Calculation Engine Audit

- Audited calculation ownership before migration to native services.

## v27.7.11 - Architecture Cleanup

- Cleaned architecture boundaries and documentation.

## v27.7.10 - Project Cleanup Validation

- Added cleanup validation and regression checks.

## v27.7.9 - Historical Documentation Audit

- Audited historical documentation and release notes.

## v27.7.6 - Input Tab Workflow Cleanup

- Cleaned native input tab workflow.

## v27.7.4 - Material Manager Toolbar Cleanup

- Simplified material manager toolbar behavior.

## v27.7.3 - Native Excel Export Backup

- Added safer Excel export backup workflow.

## v27.7 - Material Manager Source of Truth

- Documented and enforced Material Manager source-of-truth behavior.

## v27.6.2 - Native Input Undo

- Added undo support for native input workflows.

## v27.6 - Native Stiffness Measurements

- Added native stiffness measurement input/management.

## v27.5.5 - Native Tensile Palette

- Improved native tensile measurement palette/visual handling.

## v27.5.4 - Input Value Limits and Palette

- Added input limits and palette improvements for measurement entry.

## v27.5.1 - Native Impact Excel Import Fix

- Fixed impact import behavior from Excel/native import path.

## v27.4.6 - Native Input Grid Navigation

- Improved native input grid keyboard/navigation workflow.

## v27.4.5 - Native Input MaterialID Sync

- Improved MaterialID synchronization for native inputs.

## v27.4.2 - Native Tensile Measurements

- Added native tensile measurement workflows.

## v27.4 - Native Tensile Measurements

- Started native tensile measurement entry/visibility work.

## v27.3.6 - Native Computed Fields Engine

- Added native computed-field behavior matching Excel-derived computed fields.

## v27.3.5 - Native Material Database Prep

- Prepared native material database structure for deeper migration.

## v27.3.4 - Safe Editing Validation

- Added safe editing validation for native material workflows.

## v27.3.3 - Native Material Manager Archive Export State

- Improved archive/export state handling in native Material Manager.

## v27.3.2 - Native Material Manager YouTube URL

- Added/cleaned YouTube URL handling in Material Manager.

## v27.3.1 - Native Material Manager Sync Validation

- Added sync validation for native Material Manager.

## v27.3 - Native Material Management

- Started native material management platform work.

## v27.2.1 - Settings Value-Only Editing

- Locked settings grid editing to value-only where appropriate.

## v27.2 - Native Settings Grid Editor

- Added native Settings Manager grid editor.

## v25.6.5 - Full Documentation Audit

- Audited accumulated documentation and build notes across the project history.
- Improved release-package documentation consistency.

## v25.6.2 - Playlist Discovery Polish

- Polished playlist discovery workflow.

## v25.6.1 - Playlist Discovery Build Fix

- Fixed playlist discovery build issue.

## v25.6 - Playlist Discovery

- Added playlist discovery support for YouTube/video planning.

## v25.5.1 - YouTube Research Scrolling Fix

- Fixed scrolling/layout issue in YouTube research surfaces.

## v25.5 - Channel Gap Analysis

- Added channel gap analysis workflow.

## v25.4 - Content Calendar Planner

- Added content calendar planning workflow.

## v25.3.3 - Comparison Discovery Layout Cleanup

- Cleaned comparison discovery layout.

## v23.4 - Visual Analytics / Runner-Up Aggregation Fix

- Improved material detail/report analytics and runner-up aggregation behavior.

## v23.1.4 - Selected Material Report Polish

- Polished selected-material report content and layout.

## v22.7 - Report PDF Content

- Expanded native PDF report content.

## v22.5 - Native PDF Export

- Added native PDF export foundation.

## v22.4 - Report Type Layouts

- Separated report type layouts and fixed report layout duplication.

## v22.3 / v22.3.1 - Material Report Layout + Preview Block Cleanup

- Improved material report layout and removed redundant preview block.

## v22.2 - Material Report Generator

- Added material report generator foundation.

## v22.1 - Report Preview Templates

- Added report preview templates.

## v22.0 - Report Export Foundation

- Started reporting/export foundation work.

## v21.5 - Export Validation Logging Layout Fixes

- Improved export validation logging and fixed cramped export-log layout.

## v21.4 - Website Manufacturer Export

- Added/expanded manufacturer website export workflow.

## v21.3 - Website Safe Publish

- Added safe publish workflow for website export.

## v21.2 - Website External Template

- Added external website template support.

## v21.1 - Website Template Export

- Added website template export workflow.

## v21.0 - Website Export Preview

- Added website export preview workflow.

## v20.5 - Ranking Polish

- Polished ranking dashboard output.

## v20.4 - Filtered Rankings

- Added/expanded filtered ranking views.

## v20.3 - Awards Winners

- Added awards/winners ranking views.

## v20.2 - Category Rankings

- Added category ranking views.

## v20.0 - Workspace Layout Redesign

- Redesigned workspace layout so the material list and detail area are more usable by default.

## v19.7 - Storage Queue Persistence

- Improved storage queue persistence.

## v19.3 Step 2 - Recommendation Workflow Step 2

- Expanded recommendation workflow implementation.

## v19.3 - Recommendation Workflow

- Added recommendation workflow foundation.

## v0.x - v18.x - Foundation, Database Viewer, Dashboards, Import/Export, and Early Analytics

- Built the WPF Filament Database application foundation, SQLite-backed material database, Excel import/export path, and early dashboard/reporting surfaces.
- Added material identity cards, engineering dashboard concepts, comparison workbench foundations, recommendation engine, video planner, and website export groundwork.
- These early builds are summarized here from the available package documentation; fine-grained release notes should be reconstructed later from archived ZIPs if needed.

## v33.5 - REPORT-500 AI Engineering Review

- Build focus: AI-style engineering review layer for reports.
- Runtime scope: report output only.
- Calculation scope: unchanged.
- Test focus: export report.html/PDF and confirm AI Engineering Review sections appear.


## v34.1 - USAGE-001 Daily Use Readiness Pack

- Daily-use baseline after v34.0 LTS.
- Added daily workflow checklist.
- Added bug/feedback capture log.
- Added usage baseline documentation.
- Added regression audit for usage-mode readiness.
- No engineering calculations changed.
- No website/report workflow changes intended.

## v34.2 - USAGE-002 Real-World Feedback Loop

- Baseline: v34.1 USAGE-001 PASS 77 / 77.
- Added usage-mode feedback capture and weekly review cadence documentation.
- Verification Center extended with real-world feedback-loop gates.
- No engineering calculation changes.


## v34.2.1 - RELEASE-001 Shared Database Working Copy Fix

- Fixed Release vs Visual Studio edit mismatch caused by executable-folder JSON working copies.
- Native working-copy files now live beside the configured SQLite database in the storage folder.
- Added diagnostics visibility for executable folder and native working-copy paths.
- No calculation or website/report export workflow changes.

## v34.2.3 - RELEASE-002A Path Diagnostics Build Fix

- Added final release acceptance diagnostics for database path, executable path, storage folder and native working-copy paths.
- Added Verification Center checks confirming shared storage-folder working-copy readiness.
- Added Release Acceptance Diagnostics section to exported verification reports.
- Documented single-instance editing rule for daily Release use.
- No calculation, import/export or report workflow changes.

## v34.3.1 – BRAND-001A Icon & Splash Visibility Fix
Corrected startup splash timing and refreshed executable version metadata for the new application icon.

## v34.4 – PRICE-001 Material Pricing & Currency Conversion
Introduced material price tracking with original currency preservation and automatic USD normalization through user-maintained exchange rates.
## v34.4.1 – PRICE-001A Pricing Build Fix
- Fixed compiler error CS0103 caused by an undefined `row` reference in tensile synchronization.
- Removed the redundant pricing-calculation call; pricing remains calculated in the native material computed-fields pipeline.
- Updated assembly version metadata to v34.4.1.

## v34.4.2 – PRICE-001B Currency Dropdowns
- Replaced free-text MSRP Currency editing with an in-cell dropdown.
- Replaced free-text Landed Currency editing with an in-cell dropdown.
- Restricted both lists to USD, ISK, EUR, and GBP.
- Existing stored values and USD conversion behavior remain unchanged.
- Updated assembly version metadata to v34.4.2.

## v34.4.4 – PRICE-001D Excel Pricing Export
- Added all six material pricing fields to the native `00 Materials` Excel backup export.
- Preserved the original amount/currency and calculated USD values in exported workbooks.
- Updated centralized version metadata to v34.4.4.

## v36.0.2 – UI-001 Website Export Hidden Control Cleanup
- Corrected Website Export layout overlap and improved AI collection coverage-status UX.

## v36.1.0 – STABILITY-002 Material Detail & Measurement Synchronization
- Connected native measurement changes to Material Detail and Dashboard Insights refresh paths.

## v36.2.0 – STABILITY-003 Native Export & Long-Session Stability
- Completed native count synchronization for manufacturers export and reports.
- Added queued Material Manager post-edit refresh to reduce duplicate event workload and autosave churn.

## v36.2.2 – STABILITY-003B Manufacturers Overview HTML Repair
Corrected the v36.2.1 overview regression by replacing the full stats HTML block rather than capture-group substitution.

## v36.3.0 – WEB-ROUTING-001
Automatic Website Template Routing. Main and manufacturers exports now resolve their dedicated templates from one website root folder, preventing accidental cross-template selection.


## v37.2.3 – WORKFLOW-003C Bulk Update Safety Refinement
- Removed All materials / All rows from Bulk Update.
- Remaining visible rows below is now the default scope.
- Current filtered materials / rows remains available as the only alternate scope.

## v37.2.4 – WORKFLOW-003D Material Selection Persistence Fix
- Fixed Material Manager toolbar actions reporting that no material was selected after focus left the grid.
- Added a guarded persistent material-selection fallback without changing the restored editing workflow.

## v37.1.1 – DATA-ENTRY-001A Responsive Search & Notes Cleanup
- Removed the redundant standalone Tech Notes editor; Notes remains editable in the Material Manager grid.
- Debounced Material Manager search by 300 ms so typing remains responsive while material and measurement views update after a short pause.
## v37.1.5.1 – QOL-001 Mouse Editing Fix
- Restored precise mouse caret placement in editable DataGrid text fields.
- Preserved Select All for keyboard navigation and deliberate focus actions.
## v37.2.0 – UI-001 Interface Consistency Foundation
- Established common control sizing, spacing, alignment and tooltip behavior.
- Added the reusable UI consistency checklist and static verification report.


## v37.2.3 – UI-004 Dialog and Page Header Consistency
- Cleaned historical version labels from feature pages and removed the technical column count from the main header.
- Standardized high-impact confirmation wording and default behavior.

## v37.2.4 – UI-005 Status Messages & Final Visual Polish
- Standardized core status messages and compact export results.
- Refined the header status strip and footer status labels.
- Completed the v37.2 UI polish milestone without changing data or calculation behavior.


## v37.2.5 – PKG-001 Lean Package and Documentation Consolidation

- Replaced per-version build-note files with one current `BUILD_NOTES.md`.
- Removed historical verification and implementation snapshot files from the regular release package.
- Preserved durable history in CHANGELOG, BUILD_HISTORY, and PROJECT_HISTORY.
- Added a canonical package structure and cleanup policy.
- Removed duplicate App/README.md while preserving all buildable source and required assets.

## v38.2.4 – MULTI-SPOOL-INVENTORY
Primary objective: support multiple inventory spools per material using a dedicated relational SQLite table and inventory editor.

## v38.2.5 – INVENTORY-SPOOL-SYNC-FIX
Bugfix release for multi-spool source-of-truth synchronization and safe blank spool creation.

## v38.3.0 – PURCHASE-ORDER-FOUNDATION
Primary objective: establish Purchase Orders as the source structure for the future Cost Engine.
## v38.3.4 – INVENTORY-EDITABLE-FILTER-FIX
- Fixed Inventory Find and filters so they directly filter Editable spool records.
- Added a dedicated ListCollectionView for the editable spool grid.
- Kept Calculated Inventory aligned with the same filter state.
- Expanded editable-row search coverage and removed the WPF new-item placeholder row.


## v38.4.1 – COST-ALLOCATION-FOUNDATION
- Added mixed-order landed-cost allocation.
- Automatic shipping allocation uses weight only when every included line has known weight; otherwise it falls back to line value.
- Added line-level cost inclusion, manual shipping allocation, calculated landed line/unit/kg values, and inventory spool cost transfer.
- Upgraded SQLite schema to v13.

## v39.1.1 – PURCHASE-TO-MATERIAL-PRICING-SYNC
Focused workflow bugfix connecting Purchase Orders, landed-cost allocation and Materials pricing.

## v40.1.0 – Experimental Definition Foundation
First v40 build; establishes the verified SQLite domain foundation for experimental testing.


## v40.5.3 – Experimental Layer Adhesion Deduplication
- Removed the duplicate Experimental Layer Adhesion editor and row type.
- Defined Tensile Upright as the canonical experimental layer-adhesion measurement.
- Preserved native sample counts, formulas, persistence and canonical analytics.

## v40.5.4 – Experimental Run Selection Rebind Fix
Primary objective: make the active experimental measurement context follow Run selection immediately.

Delivered explicit row activation and deferred editor rebinding for the Experimental Runs DataGrid.

## v40.5.5 – Experimental Keyboard Navigation
Primary objective: restore the canonical arrow-key data-entry workflow in Experimental measurements.

Added Experimental Tensile, Impact and Stiffness grids to the shared workflow keyboard registration, enabling Right/Left/Up/Down, Enter and Tab navigation while skipping read-only calculated columns.

## v40.5.6 – Experimental Single-Click Editor Fix
- Removed the deferred `CommitEdit` calls from Experimental measurement `CellEditEnding`.
- Prevents the calculation callback from closing the next input cell after it has been activated.
- Restores reliable first-click editing for consecutive Tensile, Impact and Stiffness inputs.
- Preserves per-Run data, calculations, auto-save and keyboard navigation.

## v40.5.7 – Experimental Input Limits & Validation
- Extended the shared numeric input filter to Experimental Tensile, Impact and Stiffness grids.
- Added measurement-aware maximum-value validation before UI commit and model persistence.
- No formula or database schema changes.

## v40.6.0 – Experimental Results Engine
Primary objective: convert verified per-Run measurements into a Test Series comparison engine.

Delivered a read-only Results surface that consumes native ResultsService outputs, highlights the baseline Run, calculates Δ%, exposes CV and summarizes high/low Runs.

## v40.8.1 – Experimental Charts Layout Optimization
Layout-only refinement: compact Series/Run grids and expanded Experimental measurement/chart workspace.

## v40.8.2 – Experimental Chart Header Spacing
- Adjusted custom Canvas chart header geometry for clearer title, unit and legend spacing.
- No database, calculation or analytics changes.

## v40.9.0 – Experimental Engineering Dashboard
Native dashboard consuming verified Experimental Results and Analytics outputs.
## v40.9.2 – Experimental Dashboard CV Transparency
- Added visible Highest CV, source Run/metric and 15% threshold to the Dashboard.
- Engineering status now reports the exact highest CV source when High variation is triggered.
- Preserved canonical CV calculations and all existing Experimental functionality.


## v40.9.5 – Bundled Website Master Template
Synchronized the app's bundled website resource with the latest approved production `index.html` and added master-template identity verification before v40.10.

## v40.10.0 – Native Website Template Database
Canonical website master moved into SQLite schema v19 with history, activation and verified export routing.

## v40.11.0 – Experimental Website Data Pipeline
Primary objective: connect verified Experimental Test Series to the canonical Website Platform with reversible per-series publication control.
## v42.13.0 - Material Printing Settings Foundation

Extended each canonical SQLite-backed MaterialID with optional nozzle-temperature, bed-temperature and print-speed min/recommended/max values, cooling and enclosure requirements, drying time in hours, and vendor-neutral printer/slicer profile references. Units are explicit (`°C`, `mm/s`, `hours`); blank remains unknown and is never converted to zero. Schema v23 is additive, the backwards-compatible JSON working copy hydrates governed values from SQLite, and native Excel `00 Materials` import/export round-trips every field.

Material Manager exposes the settings as editable columns and Material Detail groups populated values under Printing Settings. Public report models and allowlists remain unchanged. Verification covers schema identity, numeric/range validity, record round-trip, detail projection, internal-only publication boundaries and the aggregate v42.13 release gate.

The first runtime attempt exposed an empty XAML `sys:String` item that compiled but caused `NullReferenceException` during `InitializeComponent`; removing the empty ComboBox entries restored startup. Runtime data entry then exposed that the complete Materials recompute/filter/module-sync/SQLite-save queue delayed arrow-key navigation. A restartable 450 ms edit debounce now gives navigation priority, collapses rapid edits into one governed save and waits while the next cell remains in edit mode; application close retains synchronous dirty-data persistence.

Debug and Release builds complete with zero warnings and zero errors. Runtime acceptance confirmed normal startup, Verification Center Overall PASS, visible printing-setting columns, persistence across application restarts and fast arrow-key column navigation. v42.13.0 is accepted.
