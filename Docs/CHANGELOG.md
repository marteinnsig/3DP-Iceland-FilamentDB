> Canonical role: chronological implementation and acceptance record.
> `RELEASES.md` is the curated release ledger; this file retains detailed
> implementation history.

## v49.0.0 - Experimental Workflow Integrity

- Aligns signed-package generation, independent verification and application
  startup on the governed schema-v29 through schema-v37 update range so the
  accepted v49 release can follow the guarded Production publish workflow.
- Publishes the runtime-accepted installer, portable ZIP and signed update feed
  in governed order; independent remote downloads match Production SHA-256.
- Makes active Runs the default canonical Results, Analytics, Dashboard and
  Charts comparison scope, matching governed website publication.
- Adds explicit `Include inactive history` review without deleting or changing
  retained Runs.
- Resets duplicated Runs to Planned, non-baseline and without a measured date.
- Replaces free-text Run status editing with governed lifecycle choices while
  preserving existing historical values.
- Adds publication readiness for active-run count, completeness, baseline,
  high CV and contradictory Completed status.
- Requires explicit owner confirmation before an incomplete Series remains
  selected for website output.
- Replaces three destructive Experimental save transactions with one atomic,
  targeted UPSERT/snapshot synchronization and guarded UsageEvent references.
- Adds baseline ownership, workflow-integrity and release Verification gates.
- Extends diagnostics and smoke navigation with Experimental counts, readiness
  and inactive-history controls.
- Corrects owner-runtime findings: Series `Active only` now starts checked,
  Results history scope is labeled explicitly and Website readiness appears on
  the first checkbox click instead of after leaving the cell.
- Adds a deterministic default-No publication-decision gate.
- Debug/Release, documentation/security gates and disposable smoke pass Full
  Data Verification 368/368 with exact logical/business-state preservation.
- Owner Verification
  `3DPIceland_FilamentDB_Verification_20260726_220656.txt` passes 368/368 and
  accepts the corrected filter, Results scope and immediate Website prompt.
- Removes the retired v48 Experimental delete/reinsert persistence methods
  after the atomic replacement received runtime acceptance.
- Schema remains v37. Production, FTPS and owner-database automation remain
  blocked.

## v48.2.0 - Optional Official Exchange-rate Reference Catalog

- Selects the documented ECB SDMX HTTPS API and derives ISK cross-rates through
  the shared EUR reference observations.
- Adds schema v37 Purchase Order rate source, observation date and fetch-time
  provenance while preserving schema-v36 Excel recovery compatibility.
- Restricts ECB prefill to Purchase Orders created in the current session.
- Removes the legacy Settings live-sync that rewrote persisted Purchase Order
  exchange rates.
- Keeps ECB refresh out of Materials, Inventory, Printers, saved purchases and
  immutable quote snapshots.
- Uses a validated per-profile cache with bounded timeout, response-size limit,
  no redirects and manual governed Settings fallback.
- Adds offline-safe diagnostics, deterministic parser/immutability Verification
  and Purchase Orders smoke navigation without live-network automation.
- Automated gates and Full Data Verification 364/364 pass. Owner accepted live
  ECB retrieval, new-order prefill, offline behavior and historical-data
  immutability.
- Canonical tester seed is schema v37; the prior schema-v36 seed is retained as
  an explicit migration fixture.

## v48.1.2 - Print Job Quote Workflow

- Adds schema v36 with append-only immutable PrintJobQuotes snapshots.
- Supports exact MaterialID landed USD/kg or explicit manual cost/currency.
- Applies grams per part × quantity once and snapshots Printer, Settings, FX,
  component totals, margin, provenance and calculation version.
- Adds saved quote history, explicit owner deletion and customer-facing PDF
  export without internal JSON evidence.
- Separates print/post-processing, consulting and design/change labor minutes.
- Corrects comma-decimal parsing so a Printer buffer of `1,3` remains 1.3.
- Expands governed Excel recovery to 24 tables with exact v35 compatibility.
- Extends disposable CRUD automation with quote persistence, identity
  immutability and exact cleanup.
- Debug/Release and disposable CRUD/recovery automation pass; candidate Full
  Data Verification is 363/363.
- Owner accepted the final quote workflow and customer-safe Labs-branded PDF;
  Full Data Verification passes 363/363.

## v48.1.1 - Printer and Pricing Settings Foundation

- Adds schema v35 with canonical PrinterProfiles and exact governed recovery.
- Adds a Printers tab with stable IDs, CRUD/archive and editable cost inputs.
- Restricts printer currency selection to a governed Settings-backed dropdown.
- Adds seven SQLite-canonical Print Job Pricing defaults under Settings.
- Calculates deterministic capital, electricity and buffered ISK/hour rates.
- Extends disposable CRUD automation across restart and exact cleanup.
- Debug/Release and disposable CRUD/recovery automation pass; Verification is
  362/362 in the candidate profile.
- Quote creation and immutable snapshots remain v48.1.2.
- Owner accepted Printer CRUD, restart persistence, governed currency
  selection and rate refresh; Full Data Verification passes 362/362.
- Refreshes the canonical tester seed to validated schema v35 and retains the
  prior schema-v33 seed as an explicit migration fixture.

## v48.0.7 - Internal Usage Analytics

- Adds private selected-Material summary cards for effective events, immutable
  ledger rows, net grams, durations, counts and evidence coverage.
- Corrects projection semantics so reversed originals and reversal rows do not
  inflate effective-event or evidence counts.
- Keeps quantitative totals as append-only net projections and missing evidence
  as `Not recorded`.
- Extends disposable CRUD automation to prove 100 g/60 minutes before
  correction and 80 g/55 minutes after correction. Public outputs are unchanged.
- Owner accepted private analytics and visually confirmed no Usage fields in
  public report/website previews; Verification passes 361/361.

## v48.0.6 - Bounded Usage UI and Automation

- Adds a private Usage tab with exact canonical MaterialID selection and
  optional same-Material Inventory spool linkage.
- Records immutable originals and exposes explicit correction as appended
  reversal plus replacement; normal edit/delete actions remain absent.
- Presents a read-only ledger and stores UI minutes as canonical whole seconds.
- Extends disposable CRUD automation to verify stable Usage controls and ledger
  state across restart. Public reports and website output remain unchanged.
- Owner accepted empty-Inventory behavior, optional no-spool usage, linked
  Inventory reconciliation and correction; Verification passes 360/360.

## v48.0.5 - Canonical Usage Persistence and Recovery

- Adds schema v34 with private append-only `UsageEvents`, exact canonical
  relationships and one-reversal-per-original enforcement.
- Commits linked Inventory weight deltas atomically with original, reversal
  and replacement events.
- Expands governed Excel recovery to 22 tables while accepting exact schema-v33
  packages as an empty UsageEvents migration.
- Extends disposable CRUD automation with restart, correction, Inventory and
  cleanup evidence. No normal Usage UI or public output is added.
- Owner runtime accepted normal behavior and Full Data Verification 359/359;
  v48.0.5 is canonical.

## v48.0.4 - Disposable Usage Domain Prototype

- Adds pure immutable Usage Event domain records and a non-persistent service.
- Validates canonical MaterialID, exact compatible InventoryItemID, UTC
  evidence, non-negative originals and measured/slicer grams provenance.
- Builds exact append-only reversal and replacement corrections while blocking
  duplicate reversal and cross-Material correction.
- Projects nullable private totals without turning missing evidence into zero.
- Produces equal/opposite inventory deltas for original/reversal events without
  writing Inventory.
- Adds deterministic Verification; no schema, UI, seed, report, website or
  owner-data behavior changes.
- Owner runtime accepted normal startup and all four usage-domain contracts;
  Full Data Verification passes 358/358.

## v48.0.3 - Usage Event Contract

- Audits native measurement, Experimental Run, Inventory, Purchasing, report,
  recovery, diagnostics and automation ownership before schema/UI work.
- Finds no canonical general Print Job or Test Session identity.
- Keeps specimen counts as measurement evidence rather than inferred usage.
- Recommends one append-only event ledger with exact relationships and explicit
  reversal/replacement correction.
- Blocks schema and UI until inventory atomicity, units, provenance and
  private/public boundaries receive owner approval.
- Requires future persistence to extend the strict governed Excel recovery and
  disposable automation contracts in the same increment.
- Owner approved append-only ownership, reversal/replacement, atomic inventory
  updates, explicit grams provenance, seconds-based duration and private
  default scope. v48.0.3 is complete without a runtime release change.

## v48.0.2 - Governed Value Index

- Adds one non-persisted Overall engineering score/MSRP USD/kg service.
- Shows both inputs, comparison scope and missing-data reason in Recommendation Detail.
- Synchronizes a selected Material's exact canonical Base Material into the
  Recommendation scope so recommendations, alternatives, hidden gems, MSRP and
  value index cannot remain on a previously selected family.
- Labels Recommendation MSRP with the exact recommended material name and
  MaterialID, distinguishing it from the Materials row used to choose scope.
- Uses the same formula for hidden-gem ordering and existing Manufacturer intelligence.
- Labels the index as comparative rather than a physical property.
- Adds stable AutomationId and deterministic Verification coverage.
- Does not change schema, scores, MSRP, public report allowlists or canonical data.
- Owner runtime accepted exact PLA/ASA scope, recommendation-price identity and
  alternatives; Full Data Verification passes 354/354.

## v48.0.1 - Canonical Pricing Provenance

- Centralizes canonical MSRP resolution and material USD conversion.
- Removes landed-cost substitution from manufacturer value intelligence.
- Makes missing rates and unsupported currencies `Not recorded`, never 1:1 USD.
- Preserves source amounts, source currency, purchase, inventory and landed-cost evidence.
- Adds deterministic pricing-provenance Verification without a schema change.
- Owner runtime accepted Materials, Advisor and Manufacturer/website behavior;
  Full Data Verification passes 351/351.

## v47.0.3 - Stable Coverage Identity

- Extends coverage JSON with optional stable collection-ID and MaterialID fields.
- Preserves legacy collection-title and material-label snapshots and exact fallback.
- Adds an explicit unique-exact binding preview with default-No confirmation.
- Leaves unmatched and ambiguous legacy entries unchanged without fuzzy remapping.
- Writes new coverage statuses with stable identity.
- Makes coverage clearing and collection deletion identity-aware.
- Adds AutomationIds, isolated tester evidence and a deterministic Verification probe.
- Leaves SQLite schema, external AI access and canonical material data unchanged.
- Owner runtime accepted identity visibility, zero-legacy behavior, status
  workflows, dashboard/pipeline propagation and Full Data Verification 350/350.

## v47.0.2 - AI Collection Workflow Clarity

- Shows whether the current collection title will create a new collection or
  update an existing one.
- Adds a read-only visible-collection preview with exact counts and bounded
  MaterialID evidence.
- Repeats the bounded MaterialID preview in the final save confirmation.
- Makes both create and update confirmations default-No.
- Warns that update replaces saved membership while retaining existing local
  pipeline-status metadata.
- Loads a selected existing collection title into the editor so update intent
  is explicit.
- Adds stable AutomationIds, deterministic read-only tester coverage and an
  extended Verification contract.
- Preserves all existing collection, session and coverage JSON formats.
- Corrects a candidate-only Verification lookup mismatch by adding stable WPF
  names alongside the AutomationIds.
- Routes AI session/collection storage to the disposable PreferencesFolder
  during automation, preventing personal AppData reads or evidence leakage.
- Corrects cancelled-update output so it shows unchanged persisted membership
  instead of presenting the discarded current-filter proposal as the collection.
- Debug/Release, documentation audit and NuGet vulnerability scan pass.
- Final profile `20260726135702-ceb78987` passes preview automation, cancel-state
  honesty, explicit disposable AI-storage isolation, Full Data Verification
  349/349 and exact business-state equality.
- Owner runtime accepted create/update state, exact preview, default-No cancel
  behavior and unchanged persisted membership after cancellation.
- Owner Full Data Verification passes 349/349; v47.0.2 is canonical.

## v47.0.1 - AI Assistant Scope Clarity

- Labels the workspace as local rule-based functionality with no external AI.
- Explains that generated output is advisory and cannot change canonical data.
- Shows current visible-row and unique-MaterialID counts before generation.
- Adds a bounded visible MaterialID preview and explicit scope refresh action.
- Clarifies that the editable planning note is included for reference rather
  than interpreted by an external AI model.
- Separates local brief generation from MaterialID collections and pipeline
  tracking.
- Adds stable AutomationIds and deterministic runner coverage for tab
  navigation, scope evidence and full-brief output.
- Adds a Full Data Verification contract for local identity and visible-scope
  clarity.
- Preserves all existing session, collection and coverage JSON formats.
- Isolated Debug/Release builds and the NuGet vulnerability scan pass.
- Disposable smoke profile `20260726122624-99485959` passes the new scope
  automation contract, Full Data Verification 348/348 and exact business-state
  equality.
- Owner runtime accepted layout, local-only purpose, filter-aware MaterialID
  scope, planning-note honesty and saved workflow compatibility.
- Owner Full Data Verification passes 348/348; v47.0.1 is canonical.

## v46.0.0 - Application Branding

- Restored the supplied transparent application-icon source and rebuilt the
  governed multi-size Windows ICO from it.
- Retained the application icon in the splash and introduced the supplied
  3DPIceland Labs wordmark as a distinct main-header resource.
- Expanded the main-header light card for readable dark wordmark presentation.
- Replaced the ineffective short splash dash with one full-length vector draw:
  the baked filament is masked only on splash and the blue trace renders from
  the nozzle across the complete 2.0-second visible interval.
- Starts the trace only after UI-thread-heavy MainWindow construction, avoiding
  a frozen first segment followed by a near-instant completion.
- Fits the trace to the measured blue-pixel bounds of the supplied icon and
  restores the original thin stroke and open upper bend.
- Preserved the accepted public HTML/PDF JPG branding contract unchanged.
- Extended Verification to require both embedded splash and header resources.
- Deterministic tester automation is unchanged because final branding,
  clipping and Windows icon acceptance are visual/manual contracts.
- Disposable profile `20260726115305-e5fa34a1` passes Full Data Verification
  347/347 and exact logical/business-state equality.
- Owner runtime accepted Windows/titlebar transparency, the separate readable
  header wordmark and the final smooth complete splash trace.
- Full Data Verification passes 347/347; v46.0.0 is canonical.

## v45.2.1 - Canonical Base Material Identity

- Adds nullable `BaseMaterialId` identity while preserving every legacy text value.
- Provides explicit exact-name binding, unlinked review, ID-owned rename propagation and referenced-delete guards.
- Makes Base Material selection canonical across Materials, reports, website, recovery and printing-profile consumers.
- Refreshes renamed/new Base Material dropdown choices immediately without restart.
- Owner runtime accepted zero unlinked Materials and Full Data Verification 347/347.

## v45.2.0 - Base Materials Workspace

- Moves the canonical Base Material Catalog from Settings Manager into a
  dedicated top-level tab.
- Adds Add, Duplicate, guarded Delete, editable Fast-grid and independent
  column-layout controls.
- Extends disposable CRUD across Base Material create/edit/duplicate/delete
  persistence while leaving schema and `BaseMaterialId` for v45.2.1.
- Owner accepted immediate startup rendering, catalog CRUD and Full Data
  Verification 346/346.

## v44.7.18 - Guarded Updater Acceptance

- Added explicit `updater` scenario authorization without releasing general
  update, Production or FTPS locks.
- Forwarded the exact disposable profile to post-update health and rollback
  relaunch processes.
- Restricted automation profiles and health evidence to the dedicated
  temporary automation root.
- Exercised the real updater helper against a copied portable build only.
- Retained transaction state, health acknowledgement, rollback snapshots and
  SHA-256 evidence for 54 governed build files.
- Candidate runtime reaches `Committed`, then forces a failed health launch
  and reaches `RolledBack` with exact file hashes restored.
- Disposable and owner Full Data Verification pass 344/344; owner accepted
  normal startup and owner-data behavior.

## v44.7.17 - Disposable Backup and Recovery Acceptance

- Added explicit disposable recovery scenario authorization.
- Verified manual `.bak` and retained legacy `.sqlite` discovery, integrity,
  compatibility and hashes.
- Automated governed Excel export, disposable mutation, Excel restore and
  controlled same-manifest restart without automating SQLite restore.
- Added post-Excel-restore SQLite evidence and rollback from verified
  pre-restore evidence if post-restore verification fails.
- Disposable Stage 4 and Full Data Verification pass 343/343 with equal
  baseline/final business-state hashes.
- Owner accepted backup discovery, governed Excel recovery, pre/post evidence
  and Full Data Verification 343/343.

## v44.7.16 - Disposable CRUD Acceptance

- Added an explicit disposable `crud` scenario authorized for one exact
  generated MaterialID.
- Added canonical create/save, restart/edit/save, restart/delete/save and final
  restart/absence checks without automating Fast-grid cells.
- Retained per-action SQLite snapshots, full logical hashes and a final
  business-state hash that normalizes only `UpdatedAtUtc`.
- Kept general deletion, owner paths, Production, FTPS, updates and restore
  blocked.
- Disposable Stage 3 and Full Data Verification pass 342/342 with equal
  before/after business-state hashes.
- Owner accepted normal create/edit/delete persistence and cleanup; owner Full
  Data Verification passes 342/342.

## v44.7.15 - Automated Report Acceptance

- Added an explicit disposable `reports` scenario over the canonical public
  report package workflow.
- Bound automated report output to the disposable profile and rejected report
  writes without per-scenario authorization.
- Added stable report action, output, completion and log Automation IDs.
- Added catalog-driven containment, type coverage, HTML marker, PDF header,
  JSON, byte-count and SHA-256 evidence checks.
- Corrected a rendered Material Summary continuation-table clip with fixed
  columns and deterministic 20-row presentation tables.
- Corrected owner-found Material Summary screen-HTML right-edge clipping with
  readable column widths, normal word wrapping and horizontal narrow-window
  scrolling without changing the PDF contract.
- Disposable acceptance passes Verification 341/341 with 211 catalog entries,
  639 verified artifacts and identical logical database hashes.
- Kept Production, FTPS, updates, restore, delete, formulas, report models,
  routes and publication approvals unchanged.
- Owner visual review accepted the representative landscape PDF and responsive
  HTML behavior; owner Full Data Verification passed 341/341.

## v44.7.14 - Automated Runtime Acceptance Foundation

- Added a disposable automation-profile manifest bound to the exact application
  executable SHA-256 and confined below a dedicated temporary root.
- Isolated database, preferences, output and evidence paths without owner
  database discovery or legacy copy behavior.
- Hard-blocked Production, FTPS, update, restore and Material deletion while
  the automation profile is active.
- Added stable Stage 1 Automation IDs and machine-readable Verification JSON/TXT
  evidence.
- Added a native Windows UI Automation runner with exact-process input,
  unexpected-dialog blocking, owned-window screenshots and before/after
  consistent SQLite snapshots plus canonical logical database hashes.
- Prevented SQLite WAL checkpoint/header normalization from producing a false
  read-only mutation failure, and made failure evidence resilient to file locks.
- Disposable runner and isolated Full Data Verification pass 340/340 with
  matching logical hashes; owner runtime acceptance and Full Data Verification
  also pass 340/340.
- Kept Fast-cell editing, report generation, CRUD, recovery and updater
  automation outside the Stage 1 candidate.

## v44.7.13 - Public HTML Trust Hardening

- Added a 5 MiB limit and structural `const DATA` object validation for new
  website-template imports.
- Added an explicit default-No executable-content trust warning before an
  imported template is stored and immediately activated.
- Preserved all existing stored templates and accepted website generation,
  Preview/Production and FTPS ownership.
- Hardened the hidden WebView2 PDF host against unexpected navigation, popups
  and permissions while retaining scripts and local assets during printing.
- Added Verification probes for malicious public text/link encoding, template
  import validation and the WebView2 host policy.
- Deliberately deferred CSP and broad HTML sanitization until compatibility and
  visual evidence can prove they do not break accepted outputs.
- Owner runtime testing accepted the import trust workflow, website Preview,
  complete Public Report Package and sampled HTML/PDF output.
- Full Data Verification passed 339/339; v44.7.13 is canonical.

## v44.7.12 - Clean Baseline Retirement

- Removed the caller-free hand-built MainWindow PDF renderer, chart/text
  projection helpers and unused `ReportFoundationResult.PdfLines`.
- Preserved canonical HTML-to-WebView2 PDF and the separately owned typed report
  renderer/certificate pipeline.
- Removed retired workbook-import write helpers and `WorkbookImportData` while
  preserving compatibility, migration and recovery owners.
- Removed retired website-template file UI/handlers, standalone manufacturer
  template helpers and residual legacy measurement handlers.
- Removed the unowned `3dp-iceland-app-icon-source.png` project/output asset;
  active application, installer and report branding remain.
- Added a v44.7.12 Verification gate for absence of retired entry points and
  continued canonical reporting ownership.
- Corrected the Public Report Package source fingerprint to read canonical
  native measurement tables instead of retired workbook-era tables.
- Owner runtime review accepted normal Engineering Package output, branding
  and all public report families; sampled HTML and final Verification passed.

## v44.7.11 - Settings Manager Command Clarity

- Renamed `Load Settings` to `Reload Saved Settings` and added a default-No
  confirmation before current unsaved Settings edits are discarded.
- Clarified that reload reads General and Deployment values from SQLite and
  leaves Base Material Catalog unchanged.
- Corrected built-in restore to replace and save only General Settings instead
  of also replacing the in-memory Base Material Catalog.
- Preserve Deployment Settings and canonical Base Materials through built-in
  restore.
- Refresh existing measurement, currency, purchase-rate and Material
  validation consumers after reload or restore.
- Renamed `Reset Fast Columns` to `Reset Columns`; retained its two-layout,
  machine-local scope and default-No confirmation.
- Hid the duplicated generic `Reload current Materials filters/data` footer
  from both Settings views; it did not reload SQLite and the explicit toolbar
  reload owns that workflow.
- Owner reload, restore, cancellation, restart, layout and visual tests passed;
  Full Data Verification passed 336/336.

## v44.7.10 - Canonical MaterialID Default Row Order

- Added one shared natural numeric MaterialID comparer to the Fast Materials,
  Tensile, Impact and Stiffness presentation row builders.
- Default unsorted views now place the lowest numeric MaterialID first and the
  newest/highest last without changing canonical collection or SQLite order.
- Reapply active header sorting when filters, reload, Add or Duplicate change
  the visible source set; preserve selected source identity.
- Kept sort session-owned and column layout preferences unchanged.
- Corrected close-time persistence ordering discovered during runtime testing:
  active Fast editors commit first, followed by parent Materials, FK-child
  measurements and derived Material test-status.
- Block measurement auto-save when its parent Material save failed, avoiding a
  secondary foreign-key error and duplicate close prompts.
- Restore the last selected MaterialID, then reset the startup viewport to the
  top-left after deferred filter refresh; Add/Duplicate keeps its new selected
  row visible.
- Make newly added and duplicated rows immediately save-safe by including the
  generated MaterialID in their editable Product Line/copy presentation text,
  avoiding duplicate computed Website Display Names.
- Debug/Release, static/documentation and read-only NuGet advisory gates passed.
- Owner Add, Duplicate, sorting, close/restart and startup viewport tests
  passed; Full Data Verification passed 335/335.

## v44.7.9 - Public Measurement Date Provenance

- Added typed, allowlisted canonical Tensile, Impact and Stiffness measured
  dates to Material Engineering and Test Session public reports.
- Render dates in ISO `yyyy-MM-dd`; missing/invalid dates remain exactly
  `Not recorded` and are never inferred from edit timestamps.
- Preserved per-material publication opt-in and the separate approval boundary
  for raw inputs and notes.
- Left schema, measurements, formulas, report routes, PDF-from-HTML,
  website/FTPS and other report families unchanged.
- Renamed the misleading `Build Selected Public Reports` action to
  `Build Public Material Reports` and clarified that Report template/scope
  govern preview/export rather than the report-family public batch buttons.
- Added wrapping to the existing shared tooltip template so longer button
  guidance remains readable inside its established maximum width.
- Made the Reports workflow button group wrap onto additional rows in narrower
  windows instead of clipping later report-family actions.
- Debug/Release and static/documentation gates passed. Owner accepted HTML/PDF
  date provenance, missing-data honesty, responsive actions and wrapped
  tooltips; Full Data Verification passed 334/334.

## v44.7.8 - Backup Filename Compatibility

- Added readable purpose-specific `.bak` filenames for newly created automatic,
  manual, SQLite-restore and Excel-restore SQLite backups.
- Kept `.bak` as a presentation convention only; online SQLite backup bytes,
  integrity/schema inspection and canonical database ownership are unchanged.
- Extended Recovery Center catalog and direct restore selection to both new
  `.bak` and every existing `.sqlite` backup.
- Preserved all legacy files without rename/move and limited the continuing
  20-file rotation to new automatic `.bak` backups.
- Preserved explicit/default-No restore, verified pre/post-restore evidence,
  updater backup-path evidence and the prohibition on automatic SQLite restore.
- Added a v44.7.8 Verification gate for naming, dual-format compatibility,
  Recovery Center, updater and interrupted-recovery boundaries.
- Aligned the accepted v44.7.3-v44.7.5 CHANGELOG/BUILD_HISTORY headings with
  their canonical final titles so the release-documentation audit passes.
- Isolated Debug/Release builds passed with zero warnings/errors; aliases,
  136-column, diff and canonical release-documentation gates passed.
- First owner runtime pass verified manual/automatic and SQLite pre/post-restore
  `.bak` names, legacy `.sqlite` discovery and Full Data Verification 333/333.
- Corrected diagnostics that combined retained legacy automatic `.sqlite`
  evidence with the new 20-file rotating `.bak` count; both are now explicit.
- Excel export correctly creates no pre-restore snapshot; the separate explicit
  Excel restore path creates its snapshot only after explicit restore approval.
- Final owner runtime testing confirmed the Excel pre-restore `.bak`, corrected
  diagnostics and Full Data Verification 333/333. v44.7.8 is complete.

## v44.7.7 - Legacy Grid Retirement

- Hid Materials preview switching and all `Use Legacy Grid(s)` buttons.
- Retained Reset Columns and every canonical edit/save action.
- Retained collapsed legacy DataGrids temporarily as internal Fast column and
  row adapters.
- Deferred legacy XAML/handler deletion until explicit Fast contracts replace
  those dependencies and Stage 1 passes runtime acceptance.
- Aligned informational and assembly release identity after the first
  Verification run exposed one cascading metadata mismatch.
- Owner runtime testing accepted the Fast-only UI and Full Data Verification
  passed 319/319; Stage 1 is complete.
- Replaced Tensile, Impact and Stiffness DataGrid-derived columns with explicit
  Fast schemas preserving accepted headers, widths, editability and layout
  keys.
- Replaced measurement DataGrid item sources with canonical measurement
  collections scoped by the established visible Materials MaterialID set.
- Added a Verification gate for exact schema counts, unique stable keys,
  filtered row parity and canonical source-object ownership.
- Owner runtime testing accepted all three measurement contracts and Full Data
  Verification passed 320/320; Stage 2 is complete.
- Replaced the Materials DataGrid-derived schema with an explicit 52-column
  Fast contract preserving read-only, checkbox and ComboBox boundaries.
- Replaced `NativeMaterialsGrid.Items` with the canonical Materials collection
  scoped by the established filter/search MaterialID set.
- Added Verification coverage for Materials schema count, editor kinds, stable
  keys, filtered row parity and canonical source ownership.
- Preserved current Materials row order and selection when edits do not change
  the active filter scope.
- Made Duplicate/Archive/Unarchive/Delete prefer the owner-visible Fast
  selection over the stale hidden DataGrid selection.
- Forced Fast surface redraw after tab reload and preserved selection across
  canonical scope synchronization.
- Delayed new MaterialID measurement synchronization until Materials SQLite
  persistence succeeds, preventing close-time foreign-key failures.
- Kept blocked Materials saves dirty so the close guard remains honest.
- Made Delete persist measurement-child removal before immediate parent
  MaterialID removal, preserving SQLite foreign-key and Verification parity.
- Routed Archive and Unarchive into the normal Materials auto-save queue.
- Owner runtime retest confirmed correct editing, selection, CRUD, tab redraw,
  201/201 SQLite parity and Full Data Verification 321/321; Stage 3 is complete.
- Replaced General Settings and Base Material Catalog DataGrid-derived columns
  with explicit six- and 23-column Fast contracts.
- Preserved Value-only General Settings editing and the three governed Base
  Material ComboBox choice sets.
- Added Verification coverage for Settings schema counts, unique keys,
  Value-only editability, ComboBox ownership and canonical row parity.
- Owner runtime testing accepted Settings validation/save/rollback,
  Base Material editing/CRUD, layouts and tab redraw; Full Data Verification
  passed 322/322 and Stage 4 is complete.
- Removed Tensile, Impact and Stiffness legacy toggle controls, click handlers,
  fallback state and reset branches.
- Added Verification coverage requiring the three measurement fallback
  controls and handlers to be absent.
- Owner runtime testing accepted Fast-only measurement workflows and Full Data
  Verification passed 323/323; Stage 5A is complete.
- Removed the complete legacy Tensile DataGrid XAML and its grid-specific
  bind/edit/commit/filter/layout/warm-up/close lifecycle.
- Added Verification coverage requiring the Tensile legacy XAML and named
  lifecycle methods to be absent.
- Owner runtime testing accepted Tensile after complete legacy deletion and
  Full Data Verification passed 324/324.
- Removed the complete legacy Impact DataGrid XAML and its grid-specific
  bind/edit/commit/filter/layout/warm-up/close lifecycle.
- Added Verification coverage requiring the Impact legacy XAML and named
  lifecycle methods to be absent.
- Retained the accepted Fast Impact schema, canonical filtered rows,
  validation, calculations and SQLite persistence.
- Owner runtime testing accepted Impact after complete legacy deletion and
  Full Data Verification passed 325/325.
- Removed the complete legacy Stiffness DataGrid XAML and its grid-specific
  bind/edit/commit/filter/layout/close lifecycle.
- Removed the obsolete deferred legacy measurement DataGrid warm-up after the
  final measurement fallback grid was deleted.
- Added Verification coverage requiring the Stiffness legacy XAML, named
  lifecycle and obsolete warm-up methods to be absent.
- Retained the accepted Fast Stiffness schema, canonical filtered rows,
  validation, calculations and SQLite persistence.
- Owner runtime testing accepted Stiffness after complete legacy deletion and
  Full Data Verification passed 326/326.
- Completed retirement of all three legacy measurement DataGrids and their
  obsolete deferred visual-tree warm-up.
- Removed the retired global Tools workflow-column reset command and its
  uncalled generic reset handler family.
- Added a local Materials `Reset Columns` action to retain the accepted reset
  capability at its owning Fast workspace.
- Retained each accepted Fast workspace's local Reset Columns action,
  confirmation and saved-layout ownership.
- Owner runtime testing accepted global-menu retirement, local resets and
  restart persistence; Full Data Verification passed 326/326.
- Removed the hidden Settings legacy-grid toggle, handler and fallback
  activation state.
- Made the accepted Fast General Settings and Base Material views the only
  activatable Settings UI; legacy XAML deletion remains runtime-gated.
- Owner runtime testing accepted Fast-only Settings behavior and Full Data
  Verification passed 326/326.
- Removed both legacy Settings DataGrid XAML blocks and their grid-specific
  bind, edit, undo, layout, recovery-commit and selection fallback callers.
- Made Fast canonical selection the sole owner of Base Material deletion while
  retaining Settings collections, SQLite save, validation and recalculation.
- Owner runtime testing accepted Settings after complete legacy-grid deletion
  and Full Data Verification passed 327/327.
- Removed the hidden Materials preview toggle, fallback handler/state and
  legacy-view reactivation method.
- Made the accepted Fast Materials view the only activatable Materials UI;
  legacy XAML deletion remains runtime-gated.
- Owner runtime testing accepted Fast-only Materials behavior, 201-row
  canonical parity and Full Data Verification 327/327.
- Made Fast-owned canonical Materials selection the sole selection source for
  reports and Duplicate/Archive/Unarchive/Delete.
- Removed hidden-DataGrid focus and refresh ownership from new-row focus,
  archive/restore and recalculation paths.
- Owner runtime testing accepted exact Fast selection for reports and
  Materials CRUD; Full Data Verification passed 328/328.
- Moved Materials filters, visible report/count scope and governed column
  Verification from hidden DataGrid state to canonical Fast contracts.
- Routed Enter-key search focus to the Fast Materials view.
- Owner runtime testing accepted canonical filter, ranking/report scope and
  restart parity; Full Data Verification passed 329/329.
- Removed hidden Materials DataGrid commit/edit handlers and their
  recovery/update/close/validation/inventory/manual-save callers.
- Retained canonical Fast autosave coalescing and routed validation refresh
  directly to the Fast view.
- Preserved SQLite, formulas, filters, validation, Settings CRUD, reports,
  FTPS, updater and recovery behavior.

## v44.7.6 - Fast Workflow Grid - Settings

- Added startup-default Fast views for general Settings and Base Material
  Catalog with a shared visible legacy fallback.
- Kept only general `Value` editable and preserved manual/close-time canonical
  save.
- Preserved immediate Deployment validation/SQLite save with transactional
  rollback of rejected host, port or username edits.
- Preserved all Base Material text/ComboBox fields, immediate SQLite save,
  Materials recalculation and Fast-aware add/delete selection.
- Added separate keyed layouts and Default-No in-place reset for both views.
- Prevented a first-open Settings crash by normalizing transient DPI, width,
  height and coordinates before shared WPF `FormattedText` construction.
- Deferred Fast Settings construction until the Settings tab is realized,
  correcting blank first-render surfaces.
- Moved Fast Settings toggle/reset actions from an accidentally matched
  Materials toolbar into the Settings Manager toolbar.
- Assigned stable occurrence-qualified layout identities to duplicate unbound
  columns such as measurement spacers.
- Added one-time canonical fallback for legacy ambiguous spacer layouts so
  Impact/Stiffness separators restore and subsequently persist independently.
- Propagated the existing Materials visible-MaterialID filter result into
  already-created Fast Tensile, Impact and Stiffness snapshots.
- Preserved Credential Manager password ownership, SQLite schema, reports,
  publishing, updater and recovery.
- Debug/Release, static/security gates, Full Data Verification and owner
  runtime acceptance passed.

## v44.7.5 - Fast Workflow Grid - Stiffness

- Added Fast Stiffness as the startup-default Stiffness view with a visible
  one-click legacy-grid fallback.
- Reused accepted Fast rendering, editing, navigation, rejected-cell rollback,
  in-place refresh/reset and immediate layout-persistence contracts.
- Preserved canonical Stiffness rows, formulas, filters, summaries,
  measurement dates, test-status refresh and SQLite auto-save.
- Enforced revolutions 0–10 and degrees 0–359 at Fast and canonical row
  boundaries.
- Added separate keyed Fast Stiffness width/order state and Default-No reset.
- Fixed narrow-grid rendering that left a leading blank region and positioned
  Revolutions/Degrees editors one column left. The surface is explicitly
  left/top aligned and editors use WPF surface-to-overlay coordinates.
- Preserved SQLite schema, reports, publishing, updater and recovery behavior.
- Debug/Release and static/security gates passed.
- Owner runtime retest accepted bounds, editing, calculations, layout,
  fallback and corrected editor alignment.
- Full Data Verification passed 317/317; v44.7.5 is accepted.

## v44.7.4 - Fast Workflow Grid - Impact

- Added Fast Impact as the startup-default Impact view with a visible
  one-click legacy-grid fallback.
- Reused the accepted Fast Workflow Grid rendering, editing, navigation,
  in-place refresh and immediate layout-persistence contracts.
- Preserved canonical Impact rows, 0–100 validation, measurement dates,
  formulas, filters, summaries, colors, test-status refresh and SQLite
  auto-save.
- Added separate keyed Fast Impact column width/order state and Default-No
  reset.
- Rejected negative samples at both Tensile and Impact canonical row
  boundaries.
- Restored rejected Fast input to its previous cell value after one warning,
  preventing repeated warnings from an unapplied snapshot.
- Changed Fast Tensile/Impact reset to apply startup-default layout in place,
  preserving current row order, selection and scroll.
- Preserved SQLite schema, reports, publishing, updater and recovery behavior.
- Debug/Release and static/security gates passed.
- Owner runtime retest accepted all input, navigation, calculation, layout and
  fallback corrections.
- Full Data Verification passed 316/316; v44.7.4 is accepted.

## v44.7.3 - Fast Workflow Grid - Tensile

- Generalized the accepted Fast Materials renderer so canonical workflow rows
  can reuse its viewport-only rendering and editing contracts.
- Added Fast Tensile as the startup-default Tensile view with a visible
  one-click legacy-grid fallback.
- Reused canonical tensile row objects, formulas, filters, validation,
  measurement-date assignment, summaries, test-status refresh and SQLite
  auto-save.
- Retained tensile sample color bands, computed-cell distinction, keyboard
  navigation and single-cell copy/paste.
- Fixed the first runtime finding where committing MAT0206 rebuilt the visible
  list, restored canonical order and moved selection to MAT0102. Computed
  values now refresh in place without changing row order or selection.
- Added separate keyed Fast Tensile column width/order persistence and a
  Default-No reset.
- Retained immediate native DataGrid layout persistence as fallback safety.
- Left Impact, Stiffness and Settings on their accepted paths until sequential
  runtime migration.
- Preserved SQLite, reports, publishing, updater and recovery behavior.
- Debug and Release passed with zero warnings/errors; documentation/static and
  vulnerability gates passed.
- Owner runtime testing accepted the complete checklist and reported that Fast
  Tensile is noticeably snappier than the legacy grid.
- Full Data Verification passed 315/315; v44.7.3 is accepted.

## v44.7.2 - Validation Help Clarity

- Added concise Materials help defining the five required row-identity fields.
- Clarified that `OK` means those fields are present, not that measurements,
  pricing or every other material field have been verified.
- Expanded the invalid-row tooltip with the same exact required-field scope.
- Preserved ValidationSummary calculations, dataset duplicate checks,
  auto-save blocking and manual-save warning behavior.
- Added a Verification gate for the help contract and established OK/missing
  results.
- Debug/Release, documentation and vulnerability gates passed. Owner runtime
  screenshot review and Full Data Verification PASS completed the release.

## v44.7.1 - Category Rankings Scope Controls

- Added 5, 10, 50, 100 and All choices to Category Rankings rows per group.
- Kept 10 as the bounded default and Reset Filters value.
- Preserved canonical visible MaterialID scope, score formulas, category
  selectors, grouping, tie-break ordering and rank assignment.
- Kept the separate Rankings Dashboard and its Top 25 reset behavior unchanged.
- `All` removes only the final per-group row limit after canonical ordering;
  DataGrid row virtualization remains enabled.
- Added a Verification gate for every supported scope and the safe fallback.
- Debug/Release passed with zero warnings/errors. Owner runtime acceptance and
  Full Data Verification 313/313 PASS completed the release.

## v44.6.2 - Canonical Measurement Date Foundation

- Added nullable canonical measurement dates for native Tensile, Impact and
  Stiffness test sets and for Experimental runs.
- A date is assigned from the local calendar only when the first actual
  measurement input is entered. Existing dates are preserved; legacy rows are
  not backfilled and dates remain manually editable.
- Advanced SQLite to schema v31 with additive columns in
  `NativeMeasurementNotes` and `ExperimentalRuns`.
- Preserved schema-v30 canonical migration, governed Excel disaster recovery,
  explicit SQLite restore, updater, website/report and FTPS behavior.
- Runtime review exposed and corrected premature native date assignment on
  cell activation and per-keystroke year normalization during manual editing.
- Replaced direct nullable-`DateTime` grid binding with a blank-safe
  `dd.MM.yyyy` text projection so clearing a date persists `null` without
  locking the DataGrid row in WPF validation.
- Added read-only Tensile, Impact and Stiffness measured dates to Material
  Detail > General > Test Information, with `Not recorded` for missing dates.
- Synchronized blank-safe date text during editing so Stiffness auto-save
  cannot run before a completed manual date or deliberate clearing reaches the
  canonical model.
- Rejected the Stiffness template-editor experiment after runtime keyboard and
  row-height failures. Restored the standard compact DataGridTextColumn and
  explicitly commits its blank-safe text binding before auto-save.
- Retained partial date text during editing so a complete manual Stiffness date
  can be formed before canonical parsing; only valid full dates or blank input
  change stored metadata.
- Rejected the visually inconsistent DatePicker experiment. Removed the
  Stiffness-only synchronous navigation save that blocked first-click editing,
  restoring the same compact DataGridTextColumn and edit-ending save pattern
  used by Tensile and Impact.
- Attach shared first-click and keyboard workflow handlers when the lazy
  Stiffness grid is initialized, so one click enters editing before typing.
- Corrected shared cell lookup after user column reordering so logical and
  visual column positions cannot activate different editors.
- Runtime accepted with compact Stiffness rows matching Tensile/Impact,
  editable dates before and after reordering, restart persistence and Full
  Data Verification 312/312 PASS.

## v44.6.1 - Canonical Release Documentation Audit

- Defined distinct canonical ownership for CHANGELOG, BUILD_HISTORY, RELEASES
  and MILESTONES.
- Reconciled the accepted v44.5.2-v44.6.0 sequence into the three release
  documents that had stopped at v44.5.1.
- Added a read-only repository audit and explicit bounded baseline for known
  historical duplicate version identifiers.
- New duplicate identifiers, missing recent canonical entries and recent title
  conflicts now fail the documentation gate; historical entries are never
  edited, deleted, renumbered or reordered automatically.
- Connected the audit to Candidate/Production release gates without changing
  runtime, SQLite, recovery, updater, website/report or FTPS behavior.
- Runtime accepted with Full Data Verification 311/311, schema v30, six Ready
  backups, zero incomplete updater transactions and unchanged guarded Recovery
  Center behavior.

## v44.6.0 - Recovery Center Clarity

- Removed the always-visible verbose updater transaction/evidence box from
  Backup and Recovery Center.
- Replaced the persistent compatibility glossary with one concise sentence.
- Preserved exact selected-backup details, compatibility classification,
  guarded Default-No restore and all updater evidence in System Diagnostics and
  Verification.
- No schema, backup, evidence, restore, updater, website/report or FTPS behavior
  changed.
- Runtime accepted with Full Data Verification 311/311, concise Recovery Center
  presentation, selected-backup verification and updater evidence retained in
  System Diagnostics.

## v44.5.9 - Supported Migration Naming

- Renamed internal loaders so canonical SQLite projection, supported
  empty-target JSON migration and built-in defaults have distinct ownership.
- Removed misleading `TransitionStorage`, `ImportedNative` and `ExcelDefaults`
  method names without changing callers, conditions or execution order.
- Corrected the remaining user-visible `SQLite transition storage` validation
  phrase to `canonical SQLite storage`.
- Preserved all five JSON migration snapshots/readers, schema v30, governed
  Excel disaster recovery, explicit SQLite restore, updater, website/report and
  FTPS behavior.
- Added a Verification gate proving the old internal names are absent and every
  renamed supported boundary remains present.
- Corrected the first runtime candidate after MAT0206 exposed whole-revolution
  stiffness being marked covered but not calculated.
- Added bounded close-time commit/save for active Tensile, Impact and Stiffness
  edits with a Default-No close-anyway failure prompt.
- Runtime accepted with Full Data Verification 310/310, MAT0206 active-cell
  persistence across restart, visible whole-revolution Stiffness output and
  report coverage parity restored.

## v44.5.8 - Retired Transition UI Residue

- Removed nine private load/import-sync click handlers with no XAML or code
  callers.
- Removed their four caller-exclusive discard-confirm helpers.
- Removed six unused JSON state allocations from measurement save paths;
  canonical SQLite saving is unchanged.
- Preserved all five supported empty-canonical JSON migration snapshot readers,
  built-in Settings defaults, governed Excel disaster recovery, explicit SQLite
  restore, updater, website/report and FTPS behavior.
- Added a Verification gate proving the obsolete handlers remain absent while
  required migration readers remain present.
- Runtime accepted with Full Data Verification 309/309, schema v30, successful
  measurement auto-save/restart evidence and current backups Ready.

## v44.5.7 - Legacy Workbook Schema Retirement

- Advanced canonical SQLite to schema v30.
- Added retained verified backup-first, transactional removal of all 13
  original-workbook/normalized legacy tables.
- Moved engineering metric consumers from `TestSummaryValues` to canonical
  in-memory measurement rows and made database measurement readers canonical.
- Updated active-database inspection for canonical v30 and supported pre-v30
  migration shapes.
- Preserved governed Excel disaster recovery, explicit SQLite restore and JSON
  migration snapshots.
- Corrected the first runtime candidate after it exposed schema-v29 acceptance
  assumptions, a missing post-migration v30 restore-ready backup and blank
  Impact/Stiffness metric adapters.
- Added direct canonical SQLite fallback for Impact/Stiffness dashboard metrics
  and aligned the local restore release gate with restore-ready schema v30
  backups after the second runtime run reached 300/308.
- Runtime accepted with Full Data Verification 308/308, canonical
  Impact/Stiffness values and scores visible, and schema v30 backups Ready.

## v44.5.6 - Retired Workbook Metadata Readers

- Removed imported-workbook sheet status from Material Detail.
- Removed the legacy Database Engine Stats UI and original import metadata from
  System Diagnostics.
- Removed the two caller-exclusive database readers and display models.
- Preserved legacy tables for a separate backup-first schema migration.
- Preserved compatibility inspection, governed Excel disaster recovery,
  explicit SQLite restore and JSON migration snapshots.
- Runtime Full Data Verification passed 307/307 with zero failures; Material
  Detail, diagnostics and governed Excel disaster-recovery export were accepted.

## v44.5.5 - Retired Legacy Write Entry Points

- Removed caller-free `ReplaceWorkbook`, `ReplaceMaterials` and `ClearCache`
  public database write entry points.
- Preserved imported-workbook tables and active readers required by Material
  Detail, diagnostics and supported-schema compatibility inspection.
- Preserved governed Excel disaster recovery, explicit SQLite restore and JSON
  migration snapshots.
- Added a Verification gate for the retired write/read-compatibility boundary.
- Runtime Full Data Verification passed 306/306 with zero failures; Material
  Detail, diagnostics and recovery surfaces were accepted.

## v44.5.4 - Measurement Help Clarity

- Removed duplicated instruction fragments from Tensile, Impact and Stiffness.
- Added stable names and Verification coverage for all three help surfaces.
- Preserved measurement calculations, storage and recovery behavior.
- Runtime Full Data Verification passed 305/305 with zero failures; all three
  corrected measurement instructions were visually accepted.

## v44.5.3 - Canonical Storage Terminology

- Replaced remaining user-visible `JSON transition`, mixed-storage and general
  Excel-import wording with canonical SQLite terminology.
- Preserved all four supported JSON empty-database migration readers.
- Preserved governed Excel disaster recovery and explicit SQLite restore.
- Added a Verification gate for terminology and compatibility-path retention.
- Runtime Full Data Verification passed 304/304 with zero failures; About and
  all three measurement storage summaries were visually accepted.

## v44.5.2 - Canonical SQLite UI Boundaries

- Removed misleading Reload Local Cache and Clear Local Cache menu actions.
- Retired the owner-approved `MaterialsImport` table after the required verified
  SQLite migration backup; retained backups remain untouched evidence.
- Removed its sync command, reader/writer and automatic empty-database fallback.
- Preserved other legacy tables for separate caller-by-caller audit.
- Renamed Settings restoration to built-in defaults, matching its actual
  code-owned source.
- Removed a dead unbound Excel-material reset handler/helper and added a
  Verification ownership/table-retirement gate.
- Runtime Full Data Verification passed 303/303 with zero failures; distinct
  Verification and System Diagnostics export filenames were runtime accepted.

## v44.5.1 - Active SQLite Compatibility Safety

- Replaced automatic deletion of an incompatible active SQLite file with
  read-only inspection, exact evidence-copy retention and fail-closed startup.
- Blocks newer schemas before an older application can initialize or rewrite
  their schema marker.
- Added isolated supported, canonical-only, newer and unreadable fixture
  verification with active-file and evidence SHA-256 parity.
- Disabled pooling on compatibility-inspection and fixture connections after
  runtime Verification exposed a retained Windows handle before evidence copy.
- Preserved supported migration backups, explicit restore, updater boundaries
  and the no-automatic-SQLite-restore contract.
- Runtime Full Data Verification passed 302/302 with zero failures after the
  pooling-handle correction.

## v44.5.0 - Retired Excel Import Surface

- Removed the unreachable original-Excel database import handler and its
  caller-exclusive workbook/material importer services.
- Preserved lower-level SQLite compatibility tables and fallback readers,
  governed Excel disaster recovery and legacy JSON migration snapshots.
- Replaced stale empty-data instructions that pointed to the unavailable
  original-Excel import command.
- Added a Verification ownership gate proving that the retired import surface
  is absent while governed Excel backup, verification and guarded restore
  remain available.
- Runtime Full Data Verification passed 301/301 with zero failures.

## v44.4.1 - Measured Materials Responsiveness

- Added a viewport-only Fast Materials view for the measured 54-column
  DataGrid cold-jump bottleneck.
- Preserved canonical SQLite auto-save, filtering, selection, editing,
  keyboard, clipboard and separately persisted fast-view column layout.
- Kept the native WPF DataGrid as a Tools-toggle fallback.
- Added the fast-view/default/fallback Verification contract; Full Data
  Verification passed 300/300 in Visual Studio runtime testing.
- Retained alternating row colors and aligned the owner-drawn headers with the
  existing WPF grids.
- Clean-VM direct install, explicit SQLite restore, portable runtime and Full
  Data Verification passed.

## v44.3.1 - Backup, Recovery and Update Evidence Clarity

- Classified integrity-valid schema-current zero-Material backups as explicit
  restore-ready healthy empty profiles, distinct from full-data evidence.
- Added a Recovery Center compatibility glossary and explicit empty-profile
  restore warning.
- Added separate read-only transaction, health acknowledgement, application
  rollback snapshot and SQLite backup evidence boundaries.
- Preserved default-No recovery, no automatic SQLite restore and evidence
  retention.

## v44.2.0 - Daily-use UI State and MaterialID Clarity

- Added machine-local last-MaterialID and safe keyed column-order persistence.
- Added one presentation-only selected-row flag and explicit MaterialID labels
  across Materials, Material Detail and Reports.
- Limited checkbox mutation to the rendered checkbox bounds without changing
  text editing or the canonical Materials data model.
- Preserved native virtualization/startup after rejecting two performance
  experiments; recorded the older horizontal first-page delay for v44.4.

## v44.1.2 - Verification Profiles and Diagnostic Honesty

- Added explicit Application Readiness and Full Data Verification profiles with
  per-check PASS, FAIL and NOT APPLICABLE status and exact profile counts.
- Kept unexpected clean-profile failures fail-closed while mapping only known
  zero-data dependencies to not applicable.
- Added a verified post-restore SQLite evidence backup so recovery gates pass
  immediately after explicit restore and restart without a manual backup step.
- Retained pre-restore backups, rollback, default-No file mutation and the
  prohibition on automatic SQLite restore.

## v43.8.8 - Remote Update Production Consolidation

- Consolidates the VM-accepted v43.8 updater, recovery and SQLite restore fixes under the first production version above all test candidates.
- Enforces streaming byte limits and SHA-256 for chunked update downloads.
- Adds remote backup, staging and rollback around update ZIP/feed activation while keeping `latest.json` last.

## v43.7.0 - Installer and Portable Deployment

- Added per-user/no-admin Setup EXE and portable ZIP generation from the existing production-signed governed package.
- Added exact deployment-plan bytes/SHA-256 verification and stable/versioned `/downloads` routing.
- Added default-No Application Release FTPS publishing with isolated backup/staging and stable download activation last.
- Kept SQLite, backups, credentials, update evidence and website deployment state outside installer, uninstall and application-release publishing.
- Removed an obsolete 176-row production-material fallback from clean-profile startup and excluded the historical data-bearing website template from distributable output.
- Added deployment-time rejection of database/spreadsheet/legacy JSON payloads, website snapshots and private material seed markers; corrected governed inventory is 10 files.
- Changed new-database FTPS host/user defaults to empty, removed the owner-specific legacy credential fallback and added the private FTPS username to deployment payload rejection.
- Embedded the splash/header PNG as a WPF pack resource so installed single-file builds render branding independently of working directory; governed inventory is now nine files.
- Removed three unreferenced SVG development diagrams from distributable output; final governed inventory is six files.
- Deferred Authenticode for private testing; clean-VM, Verification Center, live publish and stable browser-download acceptance passed.

## v43.6.0 - Update and Deployment Diagnostics

- Added read-only durable transaction history to System Diagnostics using the existing v1 request/state contract.
- Added startup detection for `Prepared`, `SnapshotReady`, `Installed`, `RollingBack` and `RollbackFailed` with default-No application-file recovery.
- Extended the external helper to restart a `Prepared` update or restore the last-known-good governed-file snapshot for later incomplete phases.
- Retained all transaction evidence and backups; SQLite is never automatically restored and website/report/FTPS engines are unchanged.
- Added isolated interrupted-phase recovery tests and a Verification Center release gate; Visual Studio Debug runtime acceptance passed 294/294 with read-only history correctly reporting one Committed and zero incomplete transactions.

## v43.5.1 - Guarded Application Update

- Added manual default-No Apply after complete signed-package readiness approval.
- Reverified and extracted only governed files into a contained same-volume transaction, saving Materials and creating a verified SQLite backup before shutdown.
- Added external old-PID wait, last-known-good snapshot, candidate launch, exact version/schema startup health acknowledgement and automatic application-file rollback/relaunch on failure.
- Removed a loose WPF runtime icon URI found during portable testing; the executable icon remains embedded and the ICO is the eleventh governed package file.
- Runtime accepted a signed v43.5.1 to v43.5.2 portable update: durable state `Committed`, health reported schema v29 and Verification passed 293/293.

## v43.5.0 - Transactional Updater Engine

- Added a separate external updater executable and shared durable transaction-state engine.
- Added contained staging/rollback boundaries, last-known-good governed-file snapshots and atomic per-file replacement.
- Added complete rollback after injected partial installation or failed health acknowledgement; traversal is blocked before mutation.
- Added the updater as the tenth governed file in production-signed packages and made publish assets deterministic across repeated publish runs.
- Kept live Apply/process orchestration disabled; SQLite is never silently replaced by the application-file engine.
- Runtime accepted the signed ten-file package and Overall Verification PASS; same-version v43.5.0 was correctly blocked.

## v43.4.1 - Governed Signed Release Packaging

- Added a user-scoped non-exportable Windows CNG ECDSA P-256 production release key and embedded only its fingerprint-pinned public trust root.
- Added an exact nine-file signed update packager and clean-worktree canonical Release command.
- Added an authoritative console probe using the real application verifier; production-signed packages pass while modified payloads and wrong keys fail closed.
- Removed single-file `Assembly.Location` fallbacks so canonical publish completes without IL3000 warnings.
- Runtime accepted manifest, inventory, hashes, trusted signature and SQLite schema checks; same-version v43.4.1 was correctly blocked and Verification was Overall PASS.

## v43.4.0 - Signed Update Readiness Foundation

- Added read-only inspection for versioned application update ZIP packages.
- Added strict ZIP path, complete inventory, byte-length, SHA-256, ECDSA signature, upgrade-version and SQLite-schema policy checks.
- Added fail-closed production trust behavior: packages remain blocked until a governed release public key is embedded.
- Added isolated Verification fixtures that accept a signed package and reject tampering, downgrade, traversal and missing production trust.
- Removed the obsolete Restore Excel Defaults entry from the Tools menu; SQLite recovery remains in Backup and Recovery Center.
- Runtime accepted startup, Update Readiness visibility and Overall Verification PASS on 2026-07-22.

## v42.12.0 - Incremental FTPS Publishing

- Added full published route, byte and SHA-256 state to completed Test and Production deployment manifests.
- Added Restore-aware delta baseline discovery isolated by Test/Production mode.
- Kept complete local allowlist validation while skipping remote backup, staging and activation for unchanged live-size-matched artifacts.
- Added safe full-publish fallback for legacy/malformed state, route-set changes and newer Production Restore recovery events.
- Added mandatory failed-manifest invalidation after rollback so a lost final FTP response cannot create a false delta baseline.
- Added successful no-change publishing with no remote mutation.
- Added planned, unchanged, changed, baseline and transferred-byte diagnostics to Test and Production results.
- Updated the live Production confirmation to distinguish complete package size from delta transfer size and disclose first-run full fallback before the default-No approval.
- Preserved backwards-compatible restore for legacy full backups and new changed-file-only delta backups.
- Runtime accepted the Test delta path: 856/862 artifacts skipped, transferred bytes reduced by 97.1%, and measured remote staging plus activation reduced from 192.1s to 0.8s (about 240x for those phases).
- Runtime accepted the Production delta path: 855/861 artifacts skipped, bytes reduced by 97.1%, and measured remote staging plus activation reduced from 98.9s to 0.8s (about 124x); the live Production index returned HTTP 200 with v42.12 and report-route markers.

## v42.11.1 - Host-Compatible FTPS Staging

- Added four-worker bounded parallel local validation of every exact publish-plan artifact before live FTPS mutation.
- Accepted one runtime-proven remote FTPS backup/staging session after five-, three- and two-session trials all produced concurrent upload aborts; the two-session failure persisted after expanding the FileZilla Server passive range from 101 to 5,001 ports and raising server threads to eight.
- Added three reconnect-and-retry attempts per staged artifact and retry-capable control-manifest operations for transient transport closure.
- Kept a complete-transfer barrier that prevents activation after any staging failure.
- Kept activation and rollback sequential, deterministic and entry-index-last.
- Added local worker count, remote session count, staging-time and sequential activation-time diagnostics to publish results.
- Made post-completion FTPS disconnect best-effort so a closed transport cannot falsely trigger rollback after successful deployment.
- Added reconnect-safe idempotent activation and deletion retries that recognize operations completed before a response was lost.
- Moved deep remote-directory creation out of the long sequential control preflight and into each retry-protected worker transfer.
- Kept remote parent-directory creation inside each retry-protected single-session transfer.
- Added publish-phase diagnostics to distinguish staging, manifest and activation transport failures while preserving rollback reporting.

## v42.11.0 - Guarded Public Report FTPS Deployment

- Connected `Publish Website` to the confirmed Generate Production prerequisite so the complete public package is rebuilt whenever canonical inputs or selection changed.
- Reloaded and rehashed the exact catalog-derived publish plan immediately before FTPS publication.
- Added timestamped remote original backups and complete private staging with per-file size verification before activation.
- Activated the full public website/report allowlist in deterministic order with root `/index.html` last.
- Added rollback that restores replaced files and removes newly introduced files, with explicit incomplete-rollback diagnostics.
- Added two-stage operator confirmation, live phase progress and an offline Verification gate that never contacts Production.
- Added `Publish Website Test`, which automatically rebuilds Preview and publishes it below isolated `/preview/` routes with `/index-test.html` activated last.
- Added a dedicated test-plan allowlist that rejects every Production entry route while reusing the same backup, staging, verification and rollback engine.
- Renamed the live action to `Publish Website Production` so the two deployment targets are explicit in the UI.
- Added a completion-marked deployment manifest to every new guarded backup with ordered routes, prior existence, byte length and SHA-256 integrity.
- Added `Restore Last Production Backup`, excluding Website Test and legacy/incomplete backups from selection.
- Added a pre-restore live recovery snapshot, verified restore staging, removal of deployment-added targets, root-index-last restoration and automatic recovery rollback.

## v42.10.0 - Production Publish Readiness

- Extended Generate Production to create an exact catalog-driven `website-publish-plan.json` after the complete data-fresh public report package is staged.
- Added byte length and SHA-256 integrity metadata for every planned production artifact.
- Excluded Preview, backup and unrelated routes while retaining catalog-owned report manifests/assets and shared branding.
- Ordered the root production `/index.html` last so a future FTPS activation cannot expose an incomplete report tree.
- Standardized all public website HTML buttons on explicit `/index.html` targets for local-file and hosted-route parity.
- Added Production publish-plan diagnostics and Verification without changing or invoking the existing FTPS uploader.

## v42.9.2 - Public Report Data Freshness

- Added a deterministic SHA-256 fingerprint over the public MaterialID selection, report-relevant canonical SQLite sources and the exact Verified Material Summary/report projection consumed by the renderers.
- Persisted the accepted revision as `source-fingerprint.json` without exposing raw measurement or operational values.
- Made Preview, Production and Build Public Report Package automatically rebuild all six report types when canonical inputs change or the prior fingerprint is missing.
- Preserved the fast validation-only path when the canonical data revision remains unchanged.
- Included the fingerprint in local website staging, package diagnostics and Verification while keeping FTPS report upload deferred.

## v42.9.1 - Automatic Website Report Prerequisites

- Made Generate Preview and Generate Production automatically ensure the complete public report package before website staging.
- Reused the manual package action's missing-or-presentation-stale detection, canonical report builders, reusable WebView2 batch printer and bounded artifact validation.
- Preserved fast validation-only behavior when every required report artifact already exists.
- Added website progress and export-log diagnostics for automatically rebuilt report types and catalog counts.
- Added Verification coverage for the shared async prerequisite flow while keeping FTPS report upload deferred.

## v42.9.0 - Public Website Report Portal

- Added catalog-driven local staging of all accepted public report HTML/PDF/metadata/assets into website exports.
- Added Preview `reports/index-test.html` and stable Production `reports/index.html` portal routes from one shared renderer.
- Added contextual opted-in MaterialID, comparison and manufacturer report links while keeping private MaterialIDs link-free.
- Replaced MaterialID-only public directory labels with full material names while retaining MaterialID as secondary traceability metadata.
- Added one shared `@media screen` dark theme across all six public report types and the portal while preserving light canonical PDF output.
- Increased Material Engineering radar grid, axis, label and comparison-line contrast in the dark screen theme without changing the light PDF theme.
- Made the one-click package automatically rebuild report types whose existing HTML predates the current shared presentation contract.
- Added complete artifact/dead-link validation, website manifest diagnostics and Preview/Production parity Verification.
- Kept the explicit FTPS publication file list unchanged; report upload remains deferred.

## v42.8.3 - Public Report PDF Layout Parity

- Added explicit print-only desktop grid overrides to Comparison, Manufacturer and Material Summary canonical HTML.
- Printed those wide portfolio reports in A4 landscape while retaining portrait output for per-MaterialID reports.
- Prevented responsive mobile rules from stacking PDF cards and charts or clipping wide engineering tables.
- Added Verification coverage for canonical print markers and report-specific page orientation.

## v42.8.2 - Public Report Bounded Multitasking

- Added four-worker bounded parallel validation across the public HTML/PDF/metadata artifact catalog.
- Validated both existence and non-zero file length using immutable report routes without concurrent SQLite, model or WPF access.
- Replaced the fixed per-PDF 350 ms delay with bounded document/font/image readiness polling and a short layout settle.
- Kept canonical WebView2 PDF printing sequential and STA-safe while making genuine multitasking visible in the package log.

## v42.8.1 - Public Report Batch Performance

- Reused one hidden WebView2 print host across the complete one-click missing-report PDF sequence.
- Removed repeated WebView2/window initialization while preserving sequential STA-safe canonical HTML-to-PDF printing.
- Added total package and per-report-type elapsed-time diagnostics to the Reports log.
- Preserved isolated printing for individual report buttons and made no report-data, route, website or FTPS changes.

## v42.8.0 - Public Engineering Report Package

- Added a canonical public portfolio index, manifest and JSON report catalog over all six accepted public report types.
- Added strict on-disk HTML/PDF/metadata completeness checks for every public MaterialID, comparison family and manufacturer route.
- Made the package action automatically invoke only the missing canonical report-type builders, including from an empty preview folder.
- Added dataset, comparison, manufacturer and MaterialID report directories without copying or recalculating individual reports.
- Preserved the portfolio root when Material Engineering previews are rebuilt by moving their standalone batch index to `materials.html`.
- Added typed catalog allowlist, safe-relative-route checks and an aggregate v42.8 Verification gate.
- Kept FTPS publication and website navigation integration deferred.

## v42.7.0 - Public Material Summary Report

- Added the stable `reports/material-summary/` canonical HTML/PDF preview route over public-approved MaterialIDs.
- Preserved accepted REPORT-110 coverage cards, native module coverage, material/manufacturer distributions and the full six-score material ledger.
- Added cross-links to existing public Material Engineering, Printing Recommendation and Test Session routes.
- Added a dedicated typed allowlist plus route, membership, parity and exclusion Verification gates.
- Kept FTPS report publication deferred.

## v42.6.0 - Public Printing Recommendation Report

- Added full per-MaterialID public REPORT-150 recommendation HTML/PDF previews.
- Preserved applications, strengths, limitations, trade-offs, workflow checks, guidance and six-axis profiles.
- Restricted alternatives to opted-in public MaterialIDs and kept exact printing settings explicitly `Not recorded`.
- Added Verification parity/settings-honesty gates without FTPS integration.

## v42.5.0 - Public Test Session Report

- Added stable per-MaterialID public Test Session HTML/PDF preview routes.
- Preserved REPORT-140 result-quality, validation, method/equipment and missing-provenance depth.
- Added a default-off SQLite `Public test details` approval for raw inputs and reviewed test notes.
- Excluded batch/lot, operational fields and unapproved details from the public projection.
- Added aggregate-only and approved-detail Verification Center probes without FTPS integration.
- Updated application identity to v42.5.0 PUBLIC-TEST-SESSION-REPORT.

## v42.4.0 - Public Manufacturer Report

- Established internal-to-public content parity as the standing report contract: preserve engineering depth and remove only explicitly non-public fields.
- Audited Material Engineering, Comparison and Manufacturer parity; restored Comparison coverage cards and materials/evidence context and added a combined Verification gate.
- Added stable public manufacturer portfolio routes and a combined local preview index.
- Restricted every portfolio, aggregate, chart and category position to opted-in MaterialIDs.
- Added a dedicated public manufacturer allowlist and safe-link contract without Supplier URL fallback.
- Added overall/consistency charts, category position and product-level engineering context.
- Expanded the public presentation to retain REPORT-130 coverage cards, public global rank, MSRP/video availability and full six-axis product profiles.
- Added Verification Center checks and kept FTPS report upload disabled.
- Updated application identity to v42.4.0 PUBLIC-MANUFACTURER-REPORT.

## v42.3.0 - Public Comparison Report

- Added stable public Material Family comparison preset routes under `reports/comparisons/`.
- Required explicit `Public reports` opt-in for every MaterialID included in a public comparison.
- Added a dedicated 18-field comparison allowlist and sensitive-field verification.
- Added local canonical HTML/PDF comparison preview generation without FTPS integration.
- Preserved the accepted REPORT-120 Overall, Tensile, Impact and Stiffness comparison chart suite in the public renderer.
- Preserved the accepted internal REPORT-120 engine and existing public Material Engineering Report.
- Updated application identity to v42.3.0 PUBLIC-COMPARISON-REPORT.

## v42.2.1 - Public Engineering Report Content Expansion

- Runtime acceptance confirmed the expanded canonical HTML/PDF report, complete radar labels and all-PASS Verification Center.

- Expanded the public Material Engineering Report from a compact safety foundation to substantive governed engineering content.
- Added Verified Material Summary averages, variation/CV, sample confidence and stiffness results plus score bars, six-axis selected/material/manufacturer radar, full metric positions, decision guidance, stronger alternatives, deterministic review, strengths, limitations, trade-offs, applications and peer context.
- Expanded the structural public allowlist from 21 to 38 top-level fields without exposing raw specimen rows or operational/internal data; internal engine/database diagnostics are omitted.
- Added the canonical `3dp-iceland-labs-logo-pdf.jpg` asset to public HTML and its print-matched PDF.
- Added Verification checks for rich content, JPEG branding and the v42.2.1 release gate.
- Fixed public radar labels and axis lines under Icelandic locale by emitting invariant SVG coordinates, and expanded the viewport so left-side labels remain complete, with a dedicated Verification guard.
- Updated application identity to v42.2.1 PUBLIC-REPORT-CONTENT.

## v42.2.0 - Canonical Public Material Selection

- Added a default-off `Public reports` checkbox to Materials for explicit per-MaterialID publication selection.
- Added additive SQLite `PublishPublicReports` storage while retaining backwards-compatible JSON loading; SQLite owns publication intent.
- Added stable Material Engineering HTML/PDF fields only for opted-in materials.
- Added public report actions to the shared Preview/Production website links renderer without changing the existing engineering chart population.
- Kept FTPS publication unchanged and excluded report packages until the complete public report portfolio is accepted.
- Added Verification checks for UI/mapping persistence, selected-versus-unselected link behavior, renderer integration and the v42.2 release gate.
- Corrected the shared Materials first-click handler so native checkbox columns toggle immediately and auto-save normally.
- Changed public preview generation from the current row to every active opted-in MaterialID, with one combined multi-material preview index.
- Updated application identity to v42.2.0 PUBLIC-MATERIAL-SELECTION.

## v42.1.0 - Public Report Publishing Foundation

- Added a local-only public Material Engineering Report preview workflow for the selected canonical MaterialID.
- Added stable static routing at `public-report-preview/reports/materials/{MaterialID}/` with canonical `index.html`, PDF printed from that HTML, public metadata, manifest and assets.
- Added a structural 21-field public allowlist. The public renderer never receives purchasing, operational stock, credentials, device paths, raw specimen rows or internal notes.
- Added a local preview index and an explicit Reports-tab `Build Public Report Preview` action; this build does not upload or mutate the production website.
- Added public methodology/whitepaper links, home-built equipment limitations, canonical MSRP and existing Verified Material Summary/governed score outputs.
- Added Verification checks for stable MaterialID routing, artifact links, allowlist enforcement, sensitive-field exclusion, methodology context, UI availability and the aggregate v42.1 release gate.
- Corrected public-link safety validation so `https://` URLs are not misclassified as Windows drive paths; failed checks now identify the exact forbidden token or device-path condition.
- Replaced generic sensitive-word matching with checks for actual serialized internal field names, operational HTML sections and device paths, allowing the report to explain that sensitive data is excluded without failing its own safety gate.
- Corrected the earlier measurement-tab release gate so a healthy low-priority warm-up still in progress is reported as PASS; an actual warm-up exception remains a Verification failure.
- Updated application identity to v42.1.0 PUBLIC-REPORT-PUBLISHING.

## v41.8.2 - Deferred Measurement Tab Warm-up

- Added low-priority first-use warm-up for the Tensile, Impact and Stiffness workspace visual trees after the Materials view has rendered.
- Realizes each measurement DataGrid separately on the WPF UI thread while restoring the user's current workspace selection before rendering can present an intermediate tab.
- Preserved the improved initial Materials startup path; measurement warm-up runs only after higher-priority startup and UI work.
- Added per-tab startup timings, Verification coverage and an aggregate v41.8.2 release gate.
- Updated application identity to v41.8.2 MEASUREMENT-TAB-WARMUP.

## v41.8.1 - Startup Refresh Coalescing

- Fixed the measured startup bottleneck where bulk loading 200 canonical Materials queued approximately 201 identical downstream UI refresh callbacks.
- Suppressed per-row collection refresh scheduling during bulk replacement and replaced it with one coalesced Background-priority refresh.
- Preserved Materials filters, Inventory choices/summary, measurement identity synchronization and deferred engineering intelligence through the same existing refresh operations.
- Added timing and Verification coverage for the consolidated native Material collection refresh.
- Updated application identity to v41.8.1 STARTUP-REFRESH-COALESCING.

## v41.8.0 - Startup Performance Instrumentation

- Added timestamped startup phase measurements from application startup through splash rendering, MainWindow construction, first usable Materials rendering and deferred engineering intelligence.
- Added individual timings for canonical Materials, measurement workspaces and secondary Manufacturers, Inventory, Experimental, Purchasing, Website, AI and Video modules.
- Added a Startup Performance section to System Diagnostics, including process/instrumentation timestamps and guidance for separate Debug, cold Release and warm Release comparisons.
- Added Verification Center coverage and an aggregate v41.8.0 release gate for the instrumentation contract.
- Preserved the existing startup order, SQLite ownership, UI-thread binding and all data workflows; this measurement build does not introduce concurrency or lazy initialization yet.
- Updated application identity to v41.8.0 STARTUP-INSTRUMENTATION.

## v41.7.8 - Combined Engineering Report Package

- Added `Export Engineering Package` alongside the single-report export action.
- The combined export writes all six accepted engineering reports into one timestamped package without changing the currently selected report in the UI.
- Each numbered report subfolder contains its own canonical `report.html`, PDF printed from that HTML, text preview, metadata, manifest and shared assets.
- Added a package-level `index.html` with direct HTML/PDF/metadata links, plus a package manifest and JSON metadata file.
- Preserved each report's accepted `Selected Material Only` or `All Visible Materials` scope contract instead of merging their data or calculations.
- Added unique package-folder collision handling so repeated exports never overwrite an earlier package.
- Added Verification Center checks for the six-report definition set, index links, manifest, metadata, export control and the aggregate v41.7.8 release gate.
- Updated application identity to v41.7.8 ENGINEERING-REPORT-PACKAGE.

## v41.7.7 - Report Portfolio: Printing Recommendation

- Added a distinct `REPORT-150` Printing Recommendation Report instead of the generic material-report fallback.
- `Selected Material Only` now produces governed application guidance, engineering strengths, limitations, trade-offs, decision guidance, print-workflow checks and stronger same-family alternatives.
- `All Visible Materials` produces a recommendation ledger over the exact current Materials-tab search/filter scope.
- Preserved canonical MaterialID and MSRP through the shared ranking projection so report rows and alternatives do not rely on display-name matching.
- Added explicit settings honesty: the report never invents nozzle/bed temperature, speed, cooling, drying or enclosure values and directs users to manufacturer/printer validation.
- Kept Video Planner, YouTube and content-planning hooks out of the report contract.
- Added `REPORT-150` Verification Center identity/scope checks and the aggregate v41.7.7 release gate.
- Updated application identity to v41.7.7 REPORT-PORTFOLIO-PRINTING-RECOMMENDATION.

## v41.7.6 - Report Portfolio: Test Session

- Added a distinct `REPORT-140` Test Session Report instead of the generic material-report fallback.
- `Selected Material Only` now produces detailed MaterialID-linked tensile, impact and stiffness traceability with specimen counts, averages, standard deviations, CV, confidence/completeness, validation state, raw native inputs and test notes.
- `All Visible Materials` produces a test-record coverage ledger over the exact current Materials-tab search/filter scope.
- Added governed method/equipment context from native Settings Manager constants without recalculating results in the report layer.
- Added an explicit traceability boundary: dedicated SessionID, test timestamp, operator, printer/slicer profile and environmental fields are marked `Not recorded` because they do not yet exist in the canonical SQLite test schema.
- Added `REPORT-140` Verification Center identity/scope checks and the aggregate v41.7.6 release gate.
- Updated application identity to v41.7.6 REPORT-PORTFOLIO-TEST-SESSION.

## v41.7.5 - Report Portfolio: Manufacturer

- Added a distinct `REPORT-130` Manufacturer Report instead of the generic material-report fallback.
- `Selected Material Only` now uses the selected material to identify its manufacturer, then expands to that manufacturer's complete active canonical portfolio.
- `All Visible Materials` reports the exact current Materials-tab filtered scope and supports one or multiple manufacturers without replacing the filter set.
- Added manufacturer portfolio coverage, product-line and material-type breadth, verified-result and complete-profile counts, MSRP/video availability, average engineering profile and strongest material/axis context.
- Added global manufacturer positioning and category position by base material from existing governed engineering scores, without recalculating measurements in the report layer.
- Added product-level engineering context with canonical MaterialID, test coverage, evidence coverage, MSRP, product links, video links and honest `n/a` values.
- Selected-scope reports now identify the source material explicitly and highlight it in the expanded manufacturer product table.
- Corrected the selected-source MaterialID lookup so the selected product marker and row highlight are emitted in exported HTML/PDF.
- Renamed the ambiguous Manufacturer Report `Evidence` column to `Engineering axes`; values such as 5/5 describe available tensile, impact, stiffness, consistency and layer-adhesion axes rather than measurement counts.
- Added `REPORT-130` Verification Center identity/scope checks and the aggregate v41.7.5 release gate.
- Updated application identity to v41.7.5 REPORT-PORTFOLIO-MANUFACTURER.

## v41.7.4 - Concise Report Package Naming

- Simplified exported report folder names to `report-name-yyyyMMdd-HHmmss`.
- Removed duplicated `3dpiceland`, report key, `pdf` and repeated report-title segments.
- Example: `comparison-report-20260721-231416` instead of `3dpiceland-comparison-pdf-20260721-231416-comparison-report`.
- Added a Verification Center naming-contract check and aggregate v41.7.4 release gate.
- Updated application identity to v41.7.4 REPORT-PACKAGE-NAMING.

## v41.7.3 - Report Portfolio: Comparison

- Added distinct `REPORT-120` Comparison Report content instead of the generic material-report fallback.
- `Selected Material Only` now treats the chosen material as a highlighted comparison anchor and selects up to five visible peers, preferring the same base material before closest overall-score distance.
- `All Visible Materials` compares the exact canonical Materials-tab filtered scope and preserves unscored materials with honest `n/a` values.
- Added engineering-axis leaders, overall/tensile/impact/stiffness charts, test-coverage and evidence-axis context, canonical MSRP USD/kg and a full side-by-side comparison table.
- Added explicit scope/methodology language stating that score deltas are comparative context, not statistical confidence or proof of fitness.
- Added Verification Center checks for selected-anchor behavior, peer selection, all-visible behavior, distinct REPORT-120 identity and the aggregate v41.7.3 release gate.
- Updated application identity to v41.7.3 REPORT-PORTFOLIO-COMPARISON.

## v41.7.2 - Canonical Material Projection

- Removed the application-wide `_materialsView` field and the hidden legacy Materials import-cache tab.
- Made the native SQLite-backed Materials collection and canonical MaterialID set the only runtime material universe.
- Migrated analytics, rankings, category rankings, awards, Video Planner, recommendations, Dashboard Insights, YouTube Research, AI collections/sessions, report scope and website export to the canonical visible/active material projections.
- Removed legacy fallback counts and selection paths; active, archived and visible totals now come from the native Materials view.
- Rebuilt all secondary material filter lists from active native records after startup and material collection changes.
- Deferred secondary filters and intelligence consumers until after the main window is visible, preventing canonical refresh work from extending the splash-screen startup phase.
- Kept imported workbook tables bounded to ingestion and transition synchronization; they no longer own runtime scope, identity or counts.
- Added Verification Center invariants for unique active/visible MaterialIDs, visible-subset parity and removal of the legacy tab, plus the aggregate v41.7.2 release gate.
- Updated application identity to v41.7.2 CANONICAL-MATERIAL-PROJECTION.

## v41.7.1 - Report Portfolio: Material Summary

- Switched report scope from the stale legacy material projection to the filtered native Materials view backed by canonical SQLite MaterialIDs.
- `All Visible Materials` now follows the search and filters currently applied on the Materials tab, while the report total records all active native materials.
- Added distinct `REPORT-110` Material Summary content for identity, test coverage, engineering-axis coverage and high-level verified results.
- Added complete, partial and no-verified-evidence coverage states, a recorded Materials-tab filter/search scope, selected-material identity details and methodology/whitepaper source links.
- Replaced internal `verified evidence` wording with plain verified-test-result language and added safe clickable video-review links to selected identity and the scoped material table.
- Corrected REPORT-110 coverage so fully/partially tested and tensile/impact/stiffness counts come from native Verified Material Summary modules rather than legacy score availability.
- Added a native `MaterialResults` scoring overload so report scores and coverage share the same native measurement summary, plus Verification parity checks against native Materials test flags.
- Clarified that the text preview shows only the first 10 materials while the exported report includes the full report scope.
- Replaced the ambiguous split package/PDF actions with `Export Current Report`, which always writes canonical HTML, PDF, text, metadata, manifest and shared assets.
- Added Verification Center checks for canonical native scope, Material Summary identity and the aggregate v41.7.1 release gate.
- Updated application identity to v41.7.1 REPORT-PORTFOLIO-SUMMARY.

## v41.6.0 - Internal Repeatability Calibration

- Added one canonical `ConsistencyCalibrationService` for the 3DPIceland internal comparative repeatability scale.
- Preserved the established consistency formula: `100 - average tensile/impact CV% - incomplete-sample penalty`.
- Standardized score labels: 90 excellent, 85 very good, 80 good, 70 moderate, 60 low and below 60 very low repeatability.
- Moved summary-level CV review to 30% and the explicit high-variation boundary to 40%.
- Replaced the conflicting 4/8/13/20 whitepaper graph and advisor thresholds with the internal score scale.
- Updated app reliability cards, Engineering Advisor, report handoffs, website tooltips, Methodology portal and whitepaper to share the same interpretation.
- Fixed Selected Material Engineering Reports so the chosen material's matching Verified Material Summary reaches repeatability and outlier analysis instead of reporting it as unavailable.
- Reconciled report repeatability labels with the existing canonical consistency profile so incomplete-sample penalties and ranking scores cannot produce a second conflicting score in the same report.
- Replaced the obsolete v36 report header with the current platform version for traceability.
- Removed Video Planner hook text from manufacturer-facing Material Engineering Reports because recommended applications and decision guidance already provide the governed usage interpretation.
- Split the Engineering radar legend into selected material, named material-family average and named manufacturer average with distinct line styles.
- Renamed the misleading `AI Engineering Review` heading to `Data-driven Engineering Review` and disclosed that the prose is generated locally by deterministic rules without an external AI or LLM.
- Documented impact-pointer temperature/fastener sensitivity, the tensile 7–8 N low-force floor and stiffness 10–15 degree repeat variation.
- Added pre-test equipment checks and explicit wording that the scale is not an industry standard or accredited accuracy statement.
- Added Verification Center probes for the observed 7.8% -> 92.2 and 19.4% -> 80.6 mappings plus an aggregate v41.6 release gate.
- Updated application identity to v41.6.0 INTERNAL-REPEATABILITY-CALIBRATION.

## v41.5.0 - Governed Intelligence Handoffs

- Added `EngineeringIntelligenceHandoffService` to compose existing advisor, repeatability, context, peer-position and alternative outputs without recalculation.
- Added the governed source statement to canonical Material Engineering Report HTML; PDF continues to print from that same HTML.
- Added a whitepaper section documenting report and Video Planner calculation boundaries.
- Recommendation-created Video Planner ideas now carry canonical MaterialID, the existing engineering score axes and the governed context into the persistent queue.
- Added backwards-compatible `MaterialId` storage to `VideoIdeaQueue`.
- Extended Material Engineering Report payload verification and Verification Center with v41.5 handoff gates.
- Fixed Selected Material Only reports to follow the material currently displayed in Material Detail instead of a stale selection retained by another materials grid.
- Added the selected material name beside the report scope selector and refresh the selected-material preview when the displayed material changes.
- Replaced the obsolete PNG report branding with the approved `3dp-iceland-labs-logo-pdf.jpg` asset in canonical HTML and PDF output.
- Updated application identity to v41.5.0 GOVERNED-INTELLIGENCE-HANDOFFS.

## v41.4.0 - Manufacturer & Category Positioning

- Added manufacturer and category peer positioning to Recommendation Detail.
- Added the same peer positioning to the purple Selected Material Intelligence card so it follows the active MaterialID independently of the global leader selection.
- Added rank, comparable material count and group-average comparison for both contexts.
- Scoped positioning to unique MaterialIDs in the active filtered recommendation dataset.
- Reused existing `EngineeringScoreProfile.OverallScore` values without recalculating engineering measurements or score axes.
- Added explicit unavailable states for missing manufacturer, category or overall-score evidence.
- Added the same governed positioning context to the reusable ChatGPT prompt.
- Added Verification Center probes and an aggregate v41.4 release gate.
- Updated application identity to v41.4.0 MANUFACTURER-CATEGORY-POSITIONING.

## v41.3.0 - Price, Inventory & Manufacturer Context

- Added a cross-context intelligence section to Recommendation Detail.
- Added canonical MSRP availability sourced from the existing Materials pricing field.
- Added stock status, linked spool count, remaining weight and storage context sourced from `InventoryEngineService` results.
- Added manufacturer country, engineering focus and strengths sourced from active SQLite manufacturer records.
- Added the same governed context to the reusable ChatGPT prompt.
- Added Verification Center source-boundary, deterministic interpretation, UI and aggregate v41.3 release gates.
- Fixed canonical MSRP resolution so an empty native Materials MSRP remains unavailable instead of falling back to landed cost or a stale legacy projection.
- Replaced the internal-facing `Canonical MSRP` UI wording with the clearer `Public MSRP reference` label.
- Updated application identity to v41.3.0 ENGINEERING-CONTEXT-INTELLIGENCE.

## v41.2.0 - Consistency & Outlier Intelligence

- Added `EngineeringConsistencyService` as a deterministic interpretation layer over Verified Material Summary CV and sample-count outputs.
- Added repeatability status, average/highest CV, measurement-set coverage and adequate-sample coverage to Recommendation Detail.
- Added a dedicated Selected Material Intelligence card that follows the active MaterialID even when that material is not present in a global Top 3 recommendation list.
- Fixed refresh precedence so the currently displayed Material Detail remains canonical and cannot be overwritten by a stale selection retained in another materials grid.
- Relabelled performance and application lists as global rankings so their scope is no longer confused with the selected Material Detail record.
- Added summary-level variation review flags for tensile and impact orientations using the documented CV interpretation bands.
- Kept specimen-level outlier decisions explicit and honest: high CV triggers review but never removes a value or claims an individual outlier without traceable raw-sample and failure-note evidence.
- Added consistency/outlier context to the reusable ChatGPT prompt while preserving existing recommendation scores and native engineering calculations.
- Added Verification Center contracts for the Verified Material Summary boundary, deterministic CV interpretation, high-variation review, limited evidence, selected-material binding, UI coverage and the aggregate v41.2 release gate.
- Updated application identity to v41.2.0 CONSISTENCY-OUTLIER-INTELLIGENCE.

## Operational validation - Production FTPS Publishing (2026-07-21)

- Completed the previously pending live operational validation of explicit FTPS publishing against the production web server.
- Confirmed TLS certificate validation with a production Let's Encrypt certificate for `www.iskort.is` and encrypted explicit FTPS on port 21.
- Confirmed passive-mode transfer, timestamped remote backup creation and successful replacement of the live website package.
- Confirmed the deployed `manufacturers/index.html` compatibility redirect opens the canonical `index.html#manufacturers` portal route.
- No application code, release identity, database schema or engineering calculations changed; this entry records production validation of the existing v40.18.1 publishing workflow.

## v41.1.0 - Comparable Alternatives & Hidden Gems

- Added comparable-alternative discovery inside each active filtered recommendation group using the existing recommendation scores and EngineeringScoreProfile axes.
- Added canonical MSRP USD/kg context from native material records, with the established landed-cost compatibility fallback.
- Added value hidden gems when an alternative retains comparable recommendation performance at a materially lower price per kilogram.
- Added specialist alternatives that disclose the strongest axis gain and the clearest trade-off against the selected material.
- Extended Recommendation Detail with an alternatives table containing type, material, score, MSRP, reason, gain and trade-off.
- Removed the obsolete hard-coded Yasin Playlist Discovery prototype from Recommendation Detail; the live data-driven Playlist Discovery under YouTube Research remains canonical.
- Corrected startup hydration so price-aware Recommendation, Video Planner and YouTube Research surfaces refresh after native SQLite material pricing has loaded, without requiring a filter change.
- Added the same coalesced refresh after native material pricing edits and undo operations.
- Added the same deterministic alternative context to the reusable ChatGPT prompt.
- Added Verification Center contracts for context isolation, value discovery, specialist trade-offs, UI availability, startup pricing hydration and the aggregate v41.1 gate.
- Updated application identity to v41.1.0 COMPARABLE-ALTERNATIVES-HIDDEN-GEMS.

## v41.0.1 - Advisor Locale Verification Fix

- Corrected the advisor comparison verification to validate structured numeric deltas instead of culture-formatted display text.
- Exposed comparison score delta and clearest axis lead/trade-off as deterministic advisor output fields for verification and future consumers.
- Preserved localized UI formatting, including Icelandic decimal commas, without weakening the numeric contract.
- Updated application identity to v41.0.1 ADVISOR-LOCALE-VERIFICATION-FIX.

## v41.0.0 - Explainable Engineering Advisor

- Added a deterministic Engineering Advisor service that consumes existing engineering score profiles without recalculating native measurements or normalized score axes.
- Added evidence summaries, strongest and weakest available axes, explicit missing-evidence caveats and an evidence-coverage indicator that is not presented as statistical confidence.
- Added comparison explanations against the closest ranked alternative, including recommendation-score delta and the clearest axis lead or trade-off.
- Extended Recommendation Detail and the reusable ChatGPT prompt with the governed advisor context while keeping any future API integration optional and non-canonical.
- Added Verification Center checks for deterministic evidence, comparison behavior, missing-data honesty, UI availability and a v41 advisor release gate.
- Updated application identity to v41.0.0 EXPLAINABLE-ENGINEERING-ADVISOR.

## v40.20.1 - Pricing Filter Synchronization Fix

- Corrected the Pricing & Value mirror controls to publish through the canonical `input` event used by the Filament Database filter engine.
- Added the same click-to-toggle interaction to Pricing multi-select controls, allowing several values to remain selected without Ctrl/Cmd.
- Preserved bidirectional state synchronization so filters selected on either portal tab remain visible and active on the other.
- Added Verification Center coverage for canonical input propagation and multi-select toggle behavior.
- Replaced the v40.20.0-specific release-identity predicate with metadata alignment across BuildInfo, assembly version and informational version, so governed patch releases validate without weakening identity checks.
- Updated application identity to v40.20.1 PRICING-FILTER-SYNC-FIX.

## v40.20.0 - Platform Integration & Release Readiness

- Added deterministic Website Preview/Production renderer-parity verification using the same canonical SQLite template, data path and portal transform.
- Added release-contract validation for all five website routes, build identity, mode-aware manufacturer redirects and Preview/Production safety markers.
- Added a complete export-package contract covering the main HTML, manufacturer redirect and methodology whitepaper.
- Added the methodology whitepaper path to Preview and Production export manifests.
- Added a single v40 local integration release gate spanning Engineering, Experimental, Website, Reporting, workspace order and release identity.
- Removed obsolete always-PASS verification statements and stale v34.3.0 release wording.
- Corrected the Pricing visual-parity contract after the compact lower filter row gained its combined CSS class.
- Corrected Preview/Production parity normalization to remove the explicit generated header wherever it appears after governed website markers, instead of assuming it is the first line.
- Added actionable parity diagnostics with the first differing character and output lengths if the renderer contract fails again.
- Resolved all 12 nullable-analysis sites reported twice by the WPF build pipeline; Debug and Release now complete with 0 warnings and 0 errors.
- Kept live FTPS deployment validation explicitly outside the local PASS gate while passive ports remain an external network dependency.
- Updated application identity to v40.20.0 PLATFORM-RELEASE-READINESS.

## v40.19.1 - Pricing & Value Portal Tab

- Consolidated the conflicting v36–v44 roadmap lists into one strategic Master Roadmap, reconciled the two v39 definitions, recorded features delivered ahead of their original milestone and retired the duplicate roadmap narrative.
- Added a dedicated `#pricing` website tab beside Filament Database.
- Moved Pricing & Value Explorer, Performance vs Price and Value Rankings out of the bottom of the database page and into the focused pricing surface.
- Added matching pricing-tab filters synchronized bidirectionally with the canonical Filament Database filter state.
- Reused the Filament Database filter card, grid, controls, helper text and reset styling for visual continuity between tabs.
- Aligned both filter cards to the same eight-column desktop layout: Chart mode through Product line on top, Sort/Search at lower left, and MSRP range with Pricing availability bottom-aligned at lower right; Pricing now mirrors Chart mode and Sort mode as well.
- Replaced verbose per-filter explanations with one concise multi-select instruction and removed redundant MSRP and availability helper text.
- Displayed the multi-select instruction once beneath Base material and increased all multi-select heights by approximately 20 percent.
- Reorganized the compact lower filter row with Reset above Search, Sort between Search and MSRP, and narrower proportional controls.
- Increased the multi-select filter height by a further 25 percent for easier scanning and selection.
- Reordered the desktop workspace for daily use: Material Detail now stays beside Materials, while Manufacturers, Purchase Orders, Inventory and Experimental Testing follow Website Export.
- Removed the obsolete "About the methodology" summary from the bottom of Filament Database now that the governed Methodology tab is canonical.
- Preserved existing pricing data, element identities, calculations, interactive charts and Preview/Production rendering path.
- Added Verification Center coverage for pricing-section placement, hash navigation and synchronized filter controls.
- Updated application identity to v40.19.1 PRICING-VALUE-PORTAL-TAB.

## v40.19.0 - Experimental Website Analytics

- Replaced the basic Experimental Lab website table renderer with a governed Experimental Website payload and renderer service.
- Added responsive charts for parameter vs tensile/layer adhesion, impact, stiffness, combined performance score and baseline-normalized comparison.
- Added series selection, dashboard completion/baseline/quality summaries, CV context and accessible result tables.
- Preserved native calculation ownership: browser JavaScript visualizes persisted Experimental Results and ExperimentalAnalyticsService output without engineering formulas.
- Added Verification Center gates for canonical identities, baseline safety, finite values, ranking parity, chart payload coverage and a deterministic two-run chart contract probe.
- Updated application identity to v40.19.0 EXPERIMENTAL-WEBSITE-ANALYTICS.

## v40.18.1 - Explicit FTPS Publishing Fix

- Corrected the deployment protocol from SFTP/SSH to FTP with required explicit TLS on port 21.
- Uses passive data connections and refuses to fall back to unencrypted FTP.
- Validates the server TLS certificate through the Windows trust chain.
- Replaced SSH.NET with .NET 9-compatible FluentFTP 54.2.0 and updated third-party notices.
- Preserved Windows Credential Manager password protection, production-file checks, timestamped remote backups, staged uploads, size validation and rollback handling.
- Updated application identity to v40.18.1 EXPLICIT-FTPS-PUBLISHING-FIX.

## v40.18.0 - Secure SFTP Website Publishing

- Added Test Connection and Publish Website actions to Website Export for `[private-ftps-identity-removed]@www.iskort.is:22`.
- Added first-use SSH host-key fingerprint approval and strict fingerprint matching on later connections.
- Stores the SFTP password in Windows Credential Manager only after a successful verified connection; credentials never enter SQLite, Git or workflow-preferences JSON.
- Publishing requires the generated Production `index.html`, `manufacturers/index.html` and methodology whitepaper PDF.
- Copies existing remote files into a timestamped `/backups/website_*` folder before replacement.
- Uploads and size-validates all files under unique temporary names before changing live files.
- Attempts automatic rollback from the remote backup if live replacement fails.
- Added SSH.NET 2025.1.0 with third-party MIT license documentation.
- Updated application identity to v40.18.0 SFTP-WEBSITE-PUBLISHING.

## v40.17.4.4 - Website Export Folder Persistence

- Website Export now saves the selected website root folder in the existing local workflow preferences.
- The last valid folder is restored automatically when the application starts again.
- Folder selection is persisted immediately instead of relying only on a clean application shutdown.
- Missing or unavailable saved folders are ignored safely, preserving the existing database-folder fallback.
- Updated application identity to v40.17.4.4 WEBSITE-EXPORT-FOLDER-PERSISTENCE.

## v40.17.4.3 - Manufacturer Relative Redirect Fix

- Changed the Manufacturers Preview redirect to `../index-test.html#manufacturers`, so local preview opens the newly generated main preview rather than the currently deployed website.
- Changed the Manufacturers Production redirect to `../index.html#manufacturers`, keeping deployment independent of domain and installation path.
- Retained `https://iskort.is/3dp/index.html#manufacturers` as canonical metadata only.
- Extended export validation to verify both mode-specific redirect targets and the canonical URL.
- Updated application identity to v40.17.4.3 MANUFACTURER-RELATIVE-REDIRECT-FIX.

## v40.17.4.2 - Manufacturer Redirect Export Cleanup

- Removed the separate Manufacturers Preview and Manufacturers Production actions from Website Export.
- Retired the separate manufacturers-page export workflow; the canonical Manufacturers section remains inside the main website.
- Main Preview now creates `manufacturers/index-test.html` as a redirect to `https://iskort.is/3dp/index.html#manufacturers`.
- Main Production now creates `manufacturers/index.html` with the same redirect and backs up an existing file before replacement.
- Added meta-refresh, JavaScript redirect, canonical-link and clickable fallback coverage.
- Added export validation, manifest, diagnostics and logging visibility for the redirect companion file.
- Updated application identity to v40.17.4.2 MANUFACTURER-REDIRECT-EXPORT-CLEANUP.

## v40.17.4.1 - Manufacturer Terminology Verification Fix

- Fixed a false Verification Center failure introduced when the v40.17.4 form added `manufacturer-cta-row` and `manufacturer-cta-primary` class names.
- Changed the terminology gate to detect only the exact obsolete `manufacturer-cta` class token.
- Added a positive assertion that the current manufacturer submission CTA remains present after terminology cleanup.
- Preserved the v40.17.4 form, email handoff, SQLite data, calculations and website output unchanged.
- Updated application identity to v40.17.4.1 MANUFACTURER-TERMINOLOGY-VERIFICATION-FIX.

## v40.17.4 - Manufacturer Material Submission Workflow

- Added a structured material-testing submission form to the canonical Manufacturers portal.
- Captures company, contact, material, product-line, colour, spool, quantity, product-page, datasheet and testing-goal details.
- Requires explicit acknowledgement that testing and publication are independent and data-driven.
- Generates a browser-side `3DPI-YYYYMMDD-XXXXXXXX` reference ID for each enquiry.
- Prepares an encoded email to `iskort@iskort.is` without uploading or storing form data on the server.
- Added Copy Submission Details as a fallback when the visitor has no configured local email application.
- Added responsive layout, native browser validation, accessible status feedback and clear privacy/delivery guidance.
- Added Verification Center gates for form fields, email handoff, copy fallback, absence of a server API and Preview/Production renderer parity.
- Preserved SQLite as Single Source of Truth and made no changes to material results, calculations, reports or whitepaper generation.
- Updated application identity to v40.17.4 MANUFACTURER-MATERIAL-SUBMISSION-WORKFLOW.

## Repository layout and generated-output cleanup

- Consolidated the nested `App/.gitignore` rules into the canonical repository-root `.gitignore`.
- Moved line-ending and binary-file attributes from `App/.gitattributes` to a repository-root `.gitattributes` so they apply consistently to source and documentation.
- Cleared accumulated local output from `App/.vs`, `App/FilamentDbApp/bin` and `App/FilamentDbApp/obj`; active Visual Studio tooling may immediately recreate small ignored cache files.
- Retained the documentation SVG assets pending a future whitepaper/export asset review.
- No application source, SQLite schema, calculations or publication behavior changed.

## Repository hygiene - generated-file protection

- Added a root `.gitignore` for Visual Studio state, .NET build/test/publish output, local SQLite data, environment overrides, logs, backups and local release archives.
- Removed the already tracked `.vs` workspace cache from version control without deleting the local developer copy.
- Prevents local executable builds and machine-specific files from being included in future commits by default.
- No application source, SQLite schema, calculations or publication behavior changed.

## Repository licensing update - GPL-3.0-only

- Relicensed the original 3DPIceland Engineering Platform source code from MIT to GNU General Public License v3.0 only (`GPL-3.0-only`).
- Moved the canonical full license text to the repository-root `LICENSE` file so GitHub and source distributions can identify it consistently.
- Added explicit project license metadata to the .NET project file.
- Added the GPL identity, no-warranty notice and license-file location to the application's About dialog.
- Included `LICENSE` and `THIRD-PARTY-NOTICES.md` in build output for redistributable packages.
- Added a third-party dependency license inventory; bundled and restored dependencies retain their own licenses and notices.
- Updated the README and package-structure documentation to distinguish the project license from third-party component licenses.
- No application behavior, SQLite schema, calculation, report, whitepaper or website-output changes.

## v40.17.3 - Manufacturer Outreach & Submission Portal

- Aligned the participating-manufacturers heading with the full Manufacturers portal width.
- Replaced descriptive capability tiles with live Verified Material Summary coverage for tensile, impact, stiffness and layer adhesion, plus canonical platform visibility counts.
- Corrected manufacturer `Tested` counts so product/review URLs can no longer make an untested material appear tested.
- Reframed the top of the Manufacturers website around independent testing and manufacturer participation.
- Added a four-step material submission workflow and a clear email CTA for manufacturer enquiries.
- Added methodology and Engineering Whitepaper links directly beside the submission action.
- Added live material and manufacturer scope, plus the verified 150+ platform-check baseline.
- Added platform capability cards for tensile, impact, stiffness, layer adhesion, manufacturer comparisons, reports, website visibility and relevant YouTube exposure.
- Added a dedicated participation-benefits section, including independent exposure, cross-brand comparability, rankings, long-term discoverability and no paid-placement requirement.
- Preserved the SQLite-backed manufacturer directory and existing engineering intelligence below the new outreach content.
- Updated application identity to v40.17.3 MANUFACTURER-OUTREACH-SUBMISSION-PORTAL.

## v40.17.2 - Manufacturer Best Value Display Detail

- Replaced the opaque Best Value ratio-only line with user-facing pricing context.
- Best Value now displays MSRP USD/kg, Engineering Score and Value Score together.
- The ranking calculation and canonical SQLite MSRP source remain unchanged.
- No changes to SQLite schema, Material Summary, Reports or Whitepaper.

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

- Removed the Manufacturers grid `CellEditEnding` refresh that attempted to refresh the active `ICollectionView` while the DataGrid was still committing its edit transaction.
- Prevents `InvalidOperationException` when moving directly from one editable manufacturer cell to another.
- Preserves direct cell editing, PropertyChanged auto-save, search filtering, archive/restore, duplicate and delete workflows.
- No SQLite schema or website payload changes.

## v40.15.2 - Manufacturer Grid Editability Fix

- Explicitly enables editing on the Manufacturers DataGrid.
- Uses CellOrRowHeader selection and the proven first-click editor workflow.
- Adds BeginningEdit and PreparingCellForEdit integration for consistent direct cell editing.
- Preserves SQLite auto-save, CRUD and website export behavior.

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

- Replaced the short proof-of-concept whitepaper content with a full engineering-methodology first edition.
- Added 16 controlled chapters covering scope, philosophy, printing, specimens, tensile/layer adhesion, impact, stiffness, statistics, confidence, data architecture, interpretation, limitations, QA, future work, references and revision history.
- Added detailed equations and all current native constants.
- Added explicit equipment procedures, failure-mode guidance, outlier rules and operational checklists.
- Rebuilt the native PDF layout engine with paragraph wrapping, automatic continuation pages, chapter styling, formula/note callouts, page totals and revision footers.
- Updated application identity to v40.14.3 ENGINEERING-WHITEPAPER-FIRST-EDITION.

## v40.14.2 – Manufacturers Export Build Fix

- Fixed CS0103 in `MainWindow.xaml.cs` by removing an invalid `whitepaperPath` reference from the manufacturers-only export completion log.
- The main website export still generates and lists the Engineering Whitepaper normally.
- No functional methodology or documentation changes.

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
- Added one-file portal navigation around the canonical website export.
- Preserved the full existing Filament Database as the default page.
- Moved published experimental output into an Experimental Lab page.
- Added Manufacturers and Methodology foundation pages.
- Added responsive sticky navigation, direct hash links and browser history behavior.
- Added three portal verification gates.

## v40.9.4 – Experimental CV Pipeline Fix
- Confirmed ResultsService correctly returns coefficient of variation as a ratio (`1.0 = 100%`).
- Fixed Experimental measurement editors to convert that ratio to percentage points before display and downstream aggregation.
- Experimental Measurement Editor, Results Table and Dashboard now share one CV unit: percentage points.
- Example: raw ratio `1.226` now displays as `122.60%`, and correctly triggers the 15% high-variation threshold.
- No tensile, impact, stiffness, average or standard-deviation formulas were changed.

## v40.9.3 – Experimental Dashboard Canonical CV Fix
- Dashboard CV quality checks now consume the same `ExperimentalSeriesResultRow` values displayed by the Results Table.
- Corrected CV unit interpretation to percentage points, restoring the 15.00% threshold.
- Removed raw measurement CV interpretation from the Dashboard quality path.

## v40.9.1 – Experimental Dashboard CV Threshold Fix
- Fixed dashboard high-variation detection by comparing canonical CV ratios against `0.15` (15%) instead of `15`.
- Preserved native CV calculation and all existing Experimental Dashboard behavior.

## v40.8.0 – Experimental Charts & Visualization

- Added five live Experimental chart surfaces for tensile, impact, stiffness, combined score and baseline comparison.
- Added Results Table/Charts subviews without replacing the existing table.
- Reused native ResultsService outputs and ExperimentalAnalyticsService scores; no formulas or storage duplicated.
- Added baseline and best-result highlighting plus Verification Center coverage.

## v40.7.1 – Experimental Impact Flat Delta Fix

- Fixed the Results grid binding for Impact Flat Δ Baseline.
- Added a dedicated `ImpactFlatDeltaDisplay` property to remove the ambiguous/stale binding path.
- Added Verification Center coverage for the deterministic Impact Flat baseline delta display.
- No storage, native calculation, measurement-entry, or analytics-weight changes.


## v40.7.0 – Experimental Analytics Engine
- Added canonical ExperimentalAnalyticsService.
- Added best-result detection, weighted ranking and Recommended Run.
- Added analytics summary cards plus Rank and Overall Score Results columns.
- Added analytics verification gates.
﻿
## v40.6.1 – Experimental Series Context Reset Fix
- Made Experimental Series switching an atomic context transition.
- Clears stale Run selection, measurement editors and Results before loading the selected Series.
- Added a version guard against out-of-order deferred WPF callbacks.
- Empty Series now remain fully empty; returning to populated Series immediately rebinds the first Run.
- No SQLite schema or calculation changes.

## v40.5.9 – Experimental Measurement Verification Gate

Added exact row-shape, uniqueness, RunID isolation, input-limit and native calculation-alignment verification for Experimental measurements.


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
- Fixed the remaining Add Experiment crash caused by delayed DataGrid SelectedItem/ScrollIntoView operations.
- Removed automatic post-add focus behavior from Add and Duplicate.
- Added a dedicated Verification Center gate for the safe post-add UI path.

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
- Fixed `InvalidOperationException` when clicking Add Experiment.
- Added safe editable-cell selection before automatic edit mode.
- Added Experimental add workflow safety verification.

## v40.2.2 – Experimental Add Crash Fix

- Fixed blocked editors in the Experimental Material Manager.
- Experimental rows now use direct editable WPF DataGrid behavior instead of the legacy native workflow click handler.
- Added Verification Center coverage for grid editability.
- No schema or canonical MaterialID changes.

## v40.2.0 – Experimental Material Manager
- Added SQLite-backed experimental CRUD UI with MaterialID and baseline linking, filters, duplication, deletion, timestamps and Verification Center gates.

# Changelog

## v39.1.3 – Materials Workflow Column Layout

- Reordered Materials for the daily website/video/testing workflow.
- Hid Diameter from the visible grid while preserving the stored field.
- Added workflow layout verification coverage.


## v39.1.2 – Purchase to Material Storage Sync
- Added Storage Location to purchase-line-to-material synchronization.
- Preserved existing Material location when the source purchase line is blank.
- Extended Verification Center coverage.

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

# v38.4.2 – Purchasing Reports

- Added Inventory Report.
- Added Purchase Report.
- Added Supplier Report.
- Added Low Stock Report.
- Added Inventory Verification Report.
- Integrated all reports with canonical HTML package and PDF export.
- Added five Verification Center checks.

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

- Fixed Inventory DataGrid crash when committing Purchase Price or other spool edits.
- Deferred filter/sort refresh until the WPF edit transaction completes.
- Added guarded collection-view refresh for active AddNew/EditItem states.
- Preserved all v38.2.10 Inventory Polish features.


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
- Fixed destructive material-table replacement that activated `ON DELETE CASCADE` and removed multi-spool inventory rows.
- Native material saves now use SQLite UPSERT and preserve child spool records.
- Fixed live Materials inventory quantity refresh by targeting the correct `NativeMaterialsGrid`.
## v38.2.3 – INVENTORY-QUANTITY-VALUE-FIX

- Corrected Quantity scaling in Estimated Inventory Value.
- Defined Spool Weight and Remaining Weight as per-spool values.
- Quantity now multiplies total capacity, total remaining weight and estimated value.
- Updated Materials grid headers to display `g / spool`.
- Added regression test instructions for one, two and three spool scenarios.

## v38.2.2 – INVENTORY-VALUE-STATE-FIX
- Fixed stale/proportional Estimated Value remaining after switching a spool to Unopened.
- Unopened now restores full-capacity remaining weight and full purchase value by rule.
- Opened with full remaining capacity correctly produces full estimated value.
- Refresh Inventory now recalculates after edit bindings fully commit.


## v38.2.1 – INVENTORY-LIVE-REFRESH
- Fixed Inventory summary data only refreshing after application restart.
- Added post-commit Inventory recalculation to the coalesced Material Manager edit/auto-save flow.
- Added Inventory refresh after material collection changes and manual saves.
- Preserved SQLite schema, inventory formulas and all downstream platform behavior.

# v38.2.0 – INVENTORY-ENGINE

- Added native InventoryEngineService.
- Added Inventory Summary workspace with live inventory totals.
- Added spool status, remaining weight, percentage and estimated-value calculations.
- Added row-level inventory validation and Verification Center coverage.
- Preserved purchasing schema and deferred Cost Engine calculations to v38.3.


## v38.1.0 – PURCHASING-FOUNDATION
- Added SQLite schema v8 purchasing and inventory fields.
- Added editable Purchasing & Inventory columns to Material Manager.
- Added supplier, order, batch, storage, status, quantity and remaining-weight foundation.
- Added purchase price, currency, shipping and VAT inputs; calculations intentionally deferred.
- Extended native persistence, detail views and Excel import aliases.

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

- Restored the proven v37.0.7 first-click TextBox and ComboBox editing path.
- Kept the v37.2.0 single-cell clipboard and context-menu productivity tools.
- Kept multi-selection removed.
- No data, calculation, report, or export changes.

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

## v37.1.4 – WORKFLOW-002D Startup Row/Cell Selection Compatibility Fix

- Replaced Cell-only selection with CellOrRowHeader on workflow DataGrids.
- Preserved multi-cell workflow tools while restoring compatibility with existing row-selection logic.
- Fixed startup and rapid-click SelectionUnit crashes.

## v37.1.2 – WORKFLOW-002B DataGrid Selection Initialization Crash Fix

- Defined cell-selection mode in XAML before grid population and user interaction.
- Removed runtime `SelectionUnit` mutation that could throw `CannotSelectRowWhenCells` during rapid clicks.
- Preserved all v37.1 workflow features.


## v37.1.1 – WORKFLOW-002A DataGrid Safe Cell Activation Fix
- Prevented `InvalidOperationException` during rapid editable-cell activation.
- Added target validation, deferred activation and one safe retry after transient WPF edit-state conflicts.
- Preserved all v37.1.0 navigation, clipboard, fill-down and smart-width behavior.

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

- Added click-through recovery when a workflow ComboBox closes without a selection.
- Outside clicks now activate the underlying editable DataGrid cell instead of being consumed by the WPF popup.
- Preserved normal selection commit and Escape cancellation behavior.

## v37.0.2.1 – WORKFLOW-001C-A DataGridCellsPresenter Build Fix

- Added the missing `System.Windows.Controls.Primitives` namespace import required by `DataGridCellsPresenter`.
- Resolves the two CS0246 build errors introduced in v37.0.2.
- No runtime workflow behavior changes.

## v37.0.2 – WORKFLOW-001C Cross-Row ComboBox First-Click Fix

- Captures the original target item and column before committing the previous ComboBox editor.
- Handles the mouse event to prevent WPF from consuming the first click only for row selection.
- Reopens editing on the requested target cell through the Dispatcher after the previous edit state has closed.

## v37.0.1 – WORKFLOW-001B ComboBox Edit-State Fix
- Fixed a stale DataGrid ComboBox edit state that could swallow clicks after editing Currency.
- The previous cell and row are now committed before another cell is activated.
- Open workflow-grid dropdowns are closed cleanly before focus moves.


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

## v36.2.1 – STABILITY-003A

- Fixed manufacturers HTML where embedded data refreshed but visible totals and manufacturer list stayed hard-coded.
- Visible summary now rebuilds from active native rows.

## v36.0 – STABILITY-001 – Stability & Data Integrity

- Centralized live material-count refreshes.
- Fixed stale header/footer/dashboard/report totals.
- Fixed `Not Tested` zero-result behavior.
- Removed duplicate report logo.

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

- Added AI-style Engineering Review output to canonical HTML/PDF reports.
- Added Executive Summary, Strengths, Weaknesses, Engineering trade-offs, Decision guidance, and Better alternatives.
- Preserved verified Material Summary consumption and blocked raw measurement access.


## v34.1 - USAGE-001 Daily Use Readiness Pack

- Daily-use baseline after v34.0 LTS.
- Added daily workflow checklist.
- Added bug/feedback capture log.
- Added usage baseline documentation.
- Added regression audit for usage-mode readiness.
- No engineering calculations changed.
- No website/report workflow changes intended.

## v34.2 - USAGE-002 Real-World Feedback Loop

- Added real-world feedback-loop documentation for the usage-mode period.
- Added usage review cadence.
- Added verification gates for feedback loop and review cadence capture.
- Preserved v34 LTS feature freeze and daily-use baseline.
- No calculation or workflow changes intended.


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

- Fixed splash visibility timing so startup work cannot consume the display interval before the window renders.
- Set a minimum 2.2-second visible splash duration plus Ready/fade transition.
- Updated executable version metadata to 34.3.1 to help Windows refresh the application resource.

## v34.4 – PRICE-001 Material Pricing & Currency Conversion
- Added MSRP and Landed Cost source amount/currency fields.
- Added automatic USD normalization for USD, ISK, EUR, and GBP.
- Added editable USD-baseline exchange rates to Settings Manager.
- Added SQLite migration and persistence for pricing data.
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
- Added MSRP Amount, MSRP Currency, MSRP USD, Landed Cost Amount, Landed Cost Currency, and Landed Cost USD to `00 Materials` in native Excel exports.
- No changes to pricing calculation or persistence behavior.


## v36.0.1 – STABILITY-001A
- Fixed Dashboard Insights inventory and coverage statistics remaining tied to stale imported workbook rows.

## v36.0.2 – UI-001
- Removed misplaced AI collection buttons from Website Export and clarified the selected-collection status reset command.

## v36.1.0 – STABILITY-002
- Fixed Material Detail tested/mechanical status synchronization with native measurement tabs.
- Refreshed selected material details and Dashboard Insights after Tensile, Impact, and Stiffness edits.

## v36.2.0 – STABILITY-003
- Migrated Manufacturers Website Export totals/data to active native Material Manager rows.
- Excluded archived materials from active report and manufacturer counts.
- Coalesced duplicate Material Manager edit refresh/save events to improve long-session stability.

## v36.2.2 – STABILITY-003B
- Fixed malformed Manufacturers overview stats caused by ambiguous regex replacement strings.
- Export now rebuilds the complete stats block and repairs previously corrupted templates.

## v36.3.0 – WEB-ROUTING-001 Automatic Website Template Routing
- Removed ambiguous shared manual HTML template selection from the visible Website Export workflow.
- Main exports now automatically use the root `index.html`.
- Manufacturers exports now automatically use `manufacturers/index.html`.
- Added bundled main-template fallback and clearer Website root folder UI wording.

## v37.1.0 – WORKFLOW-002 Data Entry Navigation & Smart Column Widths
- Added Enter, Tab, and Shift+Tab navigation across editable workflow cells.
- Added automatic ComboBox opening and TextBox selection during keyboard navigation.
- Added rectangular Ctrl+C/Ctrl+V clipboard operations compatible with Excel.
- Added Ctrl+D fill-down for selected cell ranges.
- Added bounded first-run smart column widths while preserving saved user widths.

## v37.2.0 – WORKFLOW-003 Single-Cell Productivity Tools
- Removed experimental multi-cell selection and range clipboard behavior.
- Kept reliable current-cell Ctrl+C/Ctrl+V.
- Added double-click copy and right-click Copy Value.
- Added Paste Special → Value Only.
- Added automatic copying for Material Manager identity fields.
- Added confirmed Fill Current Column Down without multi-selection.


## v37.2.3 – WORKFLOW-003C Bulk Update Safety Refinement
- Removed All materials / All rows from Bulk Update.
- Remaining visible rows below is now the default scope.
- Current filtered materials / rows remains available as the only alternate scope.

## v37.2.4 – WORKFLOW-003D Material Selection Persistence Fix
- Preserved the last valid Material Manager selection when focus moves from the grid to action buttons.
- Duplicate, Delete, Archive and Unarchive now resolve the material from selected row, current cell, or the last valid selection.
- Updated the remembered selection after duplication and cleared it safely after deletion.

## v37.1.1 – DATA-ENTRY-001A
- Removed the standalone Tech Notes panel.
- Improved Material Manager search responsiveness with deferred filtering after typing pauses.

## v37.1.5 – DATA-ENTRY-005
- Completed the data-entry quality-of-life keyboard and focus pass.
## v37.1.5.1 – QOL-001 Mouse Editing Fix
- Restored precise mouse caret placement in editable DataGrid text fields.
- Preserved Select All for keyboard navigation and deliberate focus actions.
## v37.2.0 – UI-001 Interface Consistency Foundation
- Added a shared visual baseline for buttons, inputs, tabs, DataGrid headers, GroupBox panels and status-bar items.
- Standardized tooltip presentation and timing.
- Added a permanent UI consistency checklist for future interface builds.
- Refreshed outdated Materials-page guidance and initial status wording.


## v37.2.2 – UI-003 Tooltip Consistency Pass
- Removed redundant tooltips from self-explanatory controls.
- Standardized concise tooltips for destructive, state-changing and technically ambiguous actions.
- Corrected the Manual Backup tooltip to describe the actual SQLite backup operation.

## v37.2.3 – UI-004 Dialog and Page Header Consistency
- Removed obsolete feature-version labels from individual page headers.
- Centralized visible build identity in the main application header.
- Removed the internal column count from header summaries.
- Standardized selected destructive and replacement confirmation dialogs with clearer consequences and safe No defaults.

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
- Added relational `InventorySpoolItems` SQLite table (schema v9).
- Added editable spool-record grid with Add, Duplicate and Delete actions.
- Added many-spools-to-one-material relationship.
- Inventory engine now calculates from independent spool records.
- Added automatic migration of existing material-level inventory values.
- Preserved existing material, engineering, website and reporting pipelines.

## v38.2.5 – INVENTORY-SPOOL-SYNC-FIX
- Corrected assembly/header version after v38.2.4 packaging mismatch.
- Made Materials Inventory Qty a read-only aggregate from InventorySpoolItems.
- Added immediate quantity synchronization after Add, Duplicate, Delete and spool-grid edits.
- Changed Add Spool to create a blank independent record instead of reusing material-level legacy values.
- Preserved Duplicate as the only explicit spool-copy action.

## v38.3.0 – Purchase Order Foundation

- Added relational PurchaseOrders and PurchaseOrderLines persistence.
- Added Purchase Orders UI and invoice attachment path.
- Added domestic VAT, deferred import VAT and tax-exempt workflows.
- Added controlled generation of linked inventory spools from purchase-line quantity.
- Upgraded SQLite schema to v10.
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
- Synced purchase-order pricing and metadata into linked material records.
- Defined MSRP Amount as the same invoice unit price as Purchase Price for the established database workflow.
- Added automatic resync after landed-cost calculation.
- Added verification coverage.

## v40.1.0 – Experimental Definition Foundation
- Added canonical ExperimentDefinitions and MaterialExperiments tables.
- Seeded ten generic experiment types.
- Added Verification Center checks for catalog readiness and MaterialID integrity.


## v40.5.3 – Experimental Layer Adhesion Deduplication
- Removed the duplicate Experimental Layer Adhesion editor and row type.
- Defined Tensile Upright as the canonical experimental layer-adhesion measurement.
- Preserved native sample counts, formulas, persistence and canonical analytics.

## v40.5.4 – Experimental Run Selection Rebind Fix
- Fixed stale Experimental Measurement Editor contents after selecting a different Run.
- Run-row clicks now activate the new RunID and rebind the current editor immediately.
- Removed the need to change measurement tabs to refresh inputs.
- No SQLite schema or calculation changes.

## v40.5.5 – Experimental Keyboard Navigation
- Added Experimental Tensile, Impact and Stiffness grids to the shared measurement keyboard-navigation workflow.
- Right/Left arrows now commit and move between editable input fields.
- Enter, Tab and Shift+Tab now work consistently with the normal measurement grids.
- Preserved per-Run data separation, calculations and SQLite schema.

## v40.5.6 – Experimental Single-Click Editor Fix
- Removed the deferred `CommitEdit` calls from Experimental measurement `CellEditEnding`.
- Prevents the calculation callback from closing the next input cell after it has been activated.
- Restores reliable first-click editing for consecutive Tensile, Impact and Stiffness inputs.
- Preserves per-Run data, calculations, auto-save and keyboard navigation.

## v40.5.7 – Experimental Input Limits & Validation
- Added native-parity range guards to Experimental measurement editors.
- Tensile is limited to 0–505 N; Impact to 0–100%; Stiffness to 0–10 revolutions and 0–359 degrees.
- Applied validation to typed input, pasted input and the persisted measurement model.
- Preserved per-Run calculations, editor navigation and SQLite schema.

## v40.6.0 – Experimental Results Engine
- Added a native Results tab for comparing all Runs in a selected Test Series.
- Added averages, CV values, baseline markers and Δ% for Tensile Upright/Flat, Impact Upright/Flat and Stiffness.
- Added numeric Run ordering and high/low result summaries.
- Results refresh after measurement edits, baseline changes, Run CRUD and Series selection.
- Added Verification Center coverage without changing SQLite schema or native formulas.

## v40.8.1 – Experimental Charts Layout Optimization
- Capped Experimental Series and Runs grids at 210 px.
- Added explicit internal vertical scrolling to both grids.
- Changed the measurement/results row to consume remaining window height with a 320 px minimum.
- Preserved all v40.8.0 charts and v40.7.1 analytics behavior.

## v40.8.2 – Experimental Chart Header Spacing
- Separated Experimental chart titles, Y-axis units and legends.
- Added a dedicated legend row below line-chart titles.
- Increased chart plot top margins to prevent header overlap.
- Preserved all chart data, analytics and live-refresh behavior.

## v40.9.0
- Added Experimental Engineering Dashboard.
- Added run completeness, missing-result, baseline and CV quality indicators.
- Added best tensile, impact, stiffness and overall recommendation cards.
- Added Verification Center dashboard check.
## v40.9.2 – Experimental Dashboard CV Transparency
- Added visible Highest CV, source Run/metric and 15% threshold to the Dashboard.
- Engineering status now reports the exact highest CV source when High variation is triggered.
- Preserved canonical CV calculations and all existing Experimental functionality.


## v40.9.5 – Bundled Website Master Template
- Replaced the bundled website template with the latest approved user-supplied production HTML.
- Made **Use Bundled** point to the synchronized master template instead of an older fallback.
- Added Verification Center coverage for the v36 Pricing & Value master-template identity.
- Preserved all current website charts, filters, pricing and value functionality.

## v40.10.0 – Native Website Template Database
- Added versioned SQLite website-template storage and active-template export routing.
- Added HTML import, activation/rollback and active-template export controls.
- Added database-template verification, hashing and first-run seed migration.

## v40.11.0 – Experimental Website Data Pipeline
- Added persistent per-series website publication control in SQLite schema v20.
- Added native experimental results and analytics rendering to main Website Preview and Production exports.
- Website visibility can be removed without deleting Test Series, Runs or Measurements.
- Added publication, payload-integrity and HTML-renderer verification gates.
## v42.13.0 - Material Printing Settings Foundation

- Added additive SQLite-backed nozzle, bed, speed, cooling, drying, enclosure and printer/slicer profile fields per canonical MaterialID.
- Added Materials editing, Material Detail grouping and Excel import/export round-trip while keeping public report allowlists unchanged.
- Added schema/range/round-trip/internal-only Verification Center gates.
- Fixed the initial XAML startup crash caused by empty `sys:String` ComboBox entries.
- Debounced heavy Materials refresh/autosave work so arrow and Tab navigation activate the next editor immediately.
- Runtime accepted with Verification PASS, restart persistence and responsive column navigation.
## v43.8.0 - Remote Signed Update Delivery

- Added governed HTTPS update feed discovery manually and one minute after usable startup.
- Authenticates the embedded production-signed manifest before bounded download, then re-verifies exact ZIP bytes, SHA-256, inventory, signature, version and schema.
- Added Default-No guarded apply and isolated FTPS update ZIP/feed publishing with versioned package activation before `latest.json`.
- Fixed clean-profile Verification Center assumptions for unconfigured FTPS and absent optional website templates.
- Made SQLite restore use SQLite-native backup copying so an in-use database is never replaced through Windows file operations.
- Hardened interrupted updates with separate pre-install snapshot failure handling, bounded file-lock retry, byte-identical recovery skips, durable path errors and execution of the verified staged updater helper.
- Runtime accepted after committed VM updates, successful restored-data restart and Verification PASS 296/296.
- Fixed a v44.7.7 Stage 5I candidate regression where editing a Fast Materials checkbox or text cell could repeatedly show the
  `Reload Materials Prototype` unapplied-change prompt.
- Owner runtime testing accepted the corrected Materials edit, persistence, validation, recalculation and recovery-export lifecycle;
  Full Data Verification passed 330/330.
- Added the v44.7.7 Stage 5J candidate, retiring residual Materials workflow-grid binding, selection, copy and refresh callers while
  retaining the collapsed legacy XAML only for the final deletion checkpoint.
- Fixed Stage 5J startup selection so the saved canonical MaterialID is visibly selected after the Fast Materials surface is created.
- Corrected that fix to defer a selection-only handoff until Fast Materials is loaded, avoiding a blank pre-layout surface that required
  `Reload current Materials filters/data`.
- Fixed blank Fast Materials tab return by making saved selection one-shot and refreshing the measured viewport at Render priority.
- Owner runtime testing accepted Stage 5J startup selection, tab return and canonical sync; Full Data Verification passed 331/331.
- Stage 5K deleted the final collapsed Materials DataGrid XAML and requires Fast Materials as the sole
  grid host.
- Owner runtime testing accepted final Materials legacy XAML deletion and all retained Fast workflows; Full Data Verification passed
  332/332 and v44.7.7 is complete.
## v45.1 - Canonical Manufacturer Selection

- Replaced free-text Fast Materials Manufacturer editing with a canonical
  dropdown sourced from active SQLite Manufacturer profiles.
- Preserved exact current legacy/unmapped Manufacturer values without silent
  remapping.
- Added nullable schema-v32 `ManufacturerId`; existing rows remain unlinked
  until an explicit catalog selection is made.
- Added previewed, default-No exact-name binding for unique catalog matches;
  ambiguous and unmatched legacy values remain unlinked.
- Added a counted `Unlinked manufacturers (n)` Materials filter for explicit
  one-by-one assignment; the count refreshes without fuzzy remapping.
- Canonical Manufacturer selection now commits `ManufacturerId` when the chosen
  catalog name equals the existing legacy text.
- Hides the unlinked filter and exact-binding action when the count reaches zero;
  both return if a supported migration/import/recovery path introduces one.
- Linked catalog rename updates Material snapshots and all existing downstream
  projections; referenced hard delete is blocked and archive remains safe.
- Governed typed Excel recovery includes the nullable ID while public allowlists
  and output shapes remain unchanged.
- Blocked catalog rename and hard delete while a Manufacturer name is referenced
  by canonical Materials; archive remains non-destructive.
- Extended disposable CRUD acceptance across unmapped and canonical
  Manufacturer save/restart persistence.
- Passed disposable CRUD and Full Data Verification 345/345 with equal
  baseline/final business-state hashes and an unchanged source seed.
- Kept Manufacturer SKU, thumbnail metadata, reports, website, public allowlists,
  Excel and governed recovery contracts unchanged.
- Owner runtime accepted zero unlinked Materials, conditional recovery controls
  and Full Data Verification 345/345.
