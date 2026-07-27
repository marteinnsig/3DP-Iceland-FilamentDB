# Help Coverage Matrix

Version: v52.2 contextual coverage maintained

Purpose: authoritative inventory for the v50.2 per-tab reference milestone.
This matrix records every supported UI surface, its owner, required reference
coverage, stable Help destination and delivery increment. A row is not complete
merely because a coarse overview mentions the tab.

## Structural baseline

- 22 top-level tabs.
- 16 nested tabs.
- 38 total tab surfaces.
- 142 declared WPF buttons.
- 30 menu items.
- 100 existing WPF AutomationIds.
- 136 click handlers across the MainWindow implementation.
- Additional runtime-built Recovery, Verification, Diagnostics and document
  windows.

Every reference section must state:

1. Purpose and prerequisites.
2. Commands and their side effects.
3. Editable versus read-only fields.
4. Auto-save, immediate-save or explicit-save behavior.
5. Filters, selection and comparison scope.
6. Validation/status meanings.
7. Destructive confirmation and history rules.
8. Cross-tab inputs and downstream handoffs.

## Exact tab registry

Top-level tabs (22):

| Tab | Planned overview Help ID |
|---|---|
| Materials | `materials.overview` |
| Manufacturers | `manufacturers.overview` |
| Purchase Orders | `purchase-orders.overview` |
| Printers | `printers.overview` |
| Print Job Quotes | `print-job-quotes.overview` |
| Inventory | `inventory.overview` |
| Usage | `usage.overview` |
| Experimental Testing | `experimental.series` |
| Material Detail | `material-detail.general` |
| Tensile Measurements | `measurements.tensile` |
| Impact Measurements | `measurements.impact` |
| Stiffness Measurements | `measurements.stiffness` |
| Website Export | `website.overview` |
| Base Materials | `base-materials.overview` |
| Settings Manager | `settings.overview` |
| AI Assistant | `ai.overview` |
| Reports / PDF Export | `reports.overview` |
| Rankings Dashboard | `analysis.rankings` |
| Category Rankings | `analysis.category-rankings` |
| Awards & Winners | `analysis.awards` |
| Dashboard Insights | `analysis.dashboard-insights` |
| YouTube Research | `youtube.overview` |

Nested tabs (16):

| Parent | Nested tab | Planned Help ID |
|---|---|---|
| Experimental Testing | Tensile | `experimental.measurements.tensile` |
| Experimental Testing | Impact | `experimental.measurements.impact` |
| Experimental Testing | Stiffness | `experimental.measurements.stiffness` |
| Experimental Testing | Results | `experimental.results` |
| Experimental Results | Dashboard | `experimental.results.dashboard` |
| Experimental Results | Table | `experimental.results.table` |
| Experimental Results | Charts | `experimental.results.charts` |
| Material Detail | General | `material-detail.general` |
| Material Detail | Printing Profile | `material-detail.printing-profile` |
| Material Detail | Mechanical | `material-detail.mechanical` |
| Material Detail | Charts | `material-detail.charts` |
| Material Detail | Analytics | `material-detail.analytics` |
| Material Detail | Compare | `material-detail.compare` |
| Material Detail | Video Planner | `material-detail.video-planner` |
| Material Detail | Recommendations | `material-detail.recommendations` |
| Material Detail | Notes | `material-detail.notes` |

## v50.2.1 — Materials, purchasing, inventory, cost and configuration

| Surface | Stable Help ID | Owner / source of truth | Required reference coverage | Automation / current gap |
|---|---|---|---|---|
| Materials | `materials.overview` | SQLite Materials + owner-drawn grid | Identity, all field groups, filters, Manual Backup, Add, Duplicate, Archive, Unarchive, Delete, Reset Columns, validation, publication choices and auto-save | Only tab/filter/host fragments have IDs; lifecycle/filter commands need coverage |
| Manufacturers | `manufacturers.overview` | SQLite Manufacturers + Material relationship | Fields, search, archived scope, Add, Duplicate, exact-name binding, Archive/Restore, Delete, rename propagation and auto-save | Tab, grid, filters and most actions lack IDs |
| Purchase Orders | `purchase-orders.overview` | SQLite PO/lines + purchasing services | Header/line fields, lifecycle, ECB, landed costs, receiving, Material/Inventory creation, attachments and deletion | Tab/ECB only; both grids and almost every action lack IDs |
| Inventory | `inventory.overview` | SQLite Inventory + `InventoryEngineService` | Spool fields, filters, summaries, Add, Duplicate, Delete, Refresh, validation, auto-save and Usage handoff | Entire tab currently lacks IDs |
| Usage | `usage.overview` | Accepted Usage ledger + atomic Inventory transaction | Event inputs, Record, Correct, Cancel, fixed units, spool decrement, totals, ledger and append-only history | Good core IDs; several inputs, Cancel and totals missing |
| Printers | `printers.overview` | SQLite Printers + `PrinterRateService` | Rate fields, Add, Duplicate, Archive/Restore, Delete, Save, auto-save and quote handoff | Strong ID coverage |
| Print Job Quotes | `print-job-quotes.overview` | SQLite saved quote snapshots | Customer/currency, Material evidence, Printer/time/labor inputs, calculation, Save, history, PDF and explicit Delete | Key paths covered; most numeric inputs/evidence/status missing |
| Base Materials | `base-materials.overview` | SQLite Base Material Catalog | Profile fields, Add, Duplicate, Delete, Reset Columns, exact-name binding, rename propagation and relationship blocks | Tab/key actions covered; grid, Reset and status missing |
| Settings Manager | `settings.overview` | SQLite General/Deployment settings | Sections/fields, prospective boundary, Save, Reload, Restore Defaults, Reset Columns and separate Base Material ownership | Tab only; commands/grid/status missing |

Required leaf destinations:

- `materials.records-and-fields`
- `materials.filters`
- `materials.lifecycle`
- `materials.validation-and-save`
- `materials.publication-boundaries`
- `manufacturers.fields-and-autosave`
- `manufacturers.binding-and-renames`
- `manufacturers.archive-delete`
- `purchase-orders.order-header`
- `purchase-orders.ordered-items`
- `purchase-orders.exchange-rates`
- `purchase-orders.landed-costs`
- `purchase-orders.receiving`
- `purchase-orders.material-inventory-handoff`
- `purchase-orders.attachments-and-delete`
- `inventory.spool-records`
- `inventory.filters-and-summary`
- `inventory.edit-save-validation`
- `inventory.material-usage-handoff`
- `usage.record-event`
- `usage.inventory-linked-consumption`
- `usage.ledger-and-totals`
- `usage.corrections`
- `printers.fields-and-rates`
- `printers.save-archive-delete`
- `printers.quote-handoff`
- `print-job-quotes.customer-and-currency`
- `print-job-quotes.material-cost`
- `print-job-quotes.printer-time-labor`
- `print-job-quotes.calculate-and-save`
- `print-job-quotes.history-pdf-delete`
- `print-job-quotes.immutability`
- `base-materials.profile-fields`
- `base-materials.catalog-editing`
- `base-materials.exact-name-binding`
- `base-materials.relationship-delete-rules`
- `settings.general-measurement-calculation`
- `settings.currency-and-purchasing`
- `settings.deployment`
- `settings.save-reload-defaults`
- `settings.prospective-change-boundary`

## v50.2.2 — Measurements, Experimental Testing and engineering analysis

| Surface | Stable Help ID | Owner / source of truth | Required reference coverage | Automation / current gap |
|---|---|---|---|---|
| Tensile Measurements | `measurements.tensile` | SQLite raw samples + `ResultsService` | Visible Material scope, Upright/Flat inputs and ranges, computed fields, date, notes, validation, auto-save and Reset Columns | Tab/host covered; Reset and field surface not covered |
| Impact Measurements | `measurements.impact` | SQLite raw samples + `ResultsService` | Visible Material scope, percentage inputs/ranges, computed fields, governed Settings, validation, auto-save and Reset | Tab/host covered; Reset and field surface not covered |
| Stiffness Measurements | `measurements.stiffness` | SQLite raw inputs + `ResultsService` | Revolutions/degrees ranges, computed deflection/modulus, Settings, validation, auto-save and Reset | Tab/host covered; Reset and field surface not covered |
| Experimental Series | `experimental.series` | SQLite Series graph | Add, Duplicate, Delete, Find, Active only, Clear, Material/experiment/unit/baseline/Website/Active/Notes and readiness | Core IDs good; Clear and nested tabs missing |
| Experimental Runs | `experimental.runs` | SQLite Runs graph | Add, Duplicate, Delete, lifecycle, measured date, baseline uniqueness, Active/history and persistence | Core action/grid IDs present |
| Experimental editors | `experimental.measurements` | Run measurement rows + `ResultsService` | Tensile/Impact/Stiffness raw and computed fields, dates, notes, validation and auto-save | All four nested tabs have IDs; grids remain outside v50.2.4 navigation |
| Experimental Results Dashboard | `experimental.results.dashboard` | Selected Series comparison | Scope label, baseline/best/recommended cards, readiness and active/history effect | Nested tab ID covered |
| Experimental Results Table | `experimental.results.table` | Selected Series comparison | Rank, metrics, delta-to-baseline, CV and baseline highlighting | Nested tab ID covered |
| Experimental Results Charts | `experimental.results.charts` | Selected Series comparison | Metric charts, baseline-normalized chart and baseline prerequisite | Nested tab ID covered |
| Material Detail General | `material-detail.general` | Selected Material + `MaterialDetailService` | Identity and dynamically grouped read-only fields | Parent/nested tab IDs covered |
| Material Detail Printing Profile | `material-detail.printing-profile` | Base Material Catalog | Controlled test/G-code baseline, Not recorded meaning and non-manufacturer boundary | Nested tab ID covered |
| Material Detail Mechanical | `material-detail.mechanical` | Canonical calculated results | Test/publication status, tensile/impact/stiffness, reliability and expanded data | Nested tab ID covered |
| Material Detail Charts | `material-detail.charts` | Canonical normalized metrics | Five-axis profile, score meaning and scientific-rating limitation | Nested tab ID covered |
| Material Detail Analytics | `material-detail.analytics` | Visible canonical results | Chart mode, multi-select results, radar selection and Clear | Nested tab ID covered; action controls remain outside navigation sweep |
| Material Detail Compare | `material-detail.compare` | Selected comparison Materials | A-D selectors, Use Selected, winners and deltas | Nested tab ID covered; action controls remain outside navigation sweep |
| Material Detail Video Planner | `material-detail.video-planner` | Local creator-planning records | Filters, Refresh, ideas lifecycle, dashboard, Clear, prompt copy and read-only candidates | Nested tab ID covered; action controls retain separate contracts |
| Material Detail Recommendations | `material-detail.recommendations` | Recommendation services | Filters, Refresh, details/evidence, alternatives, cautions, prompt and Video Planner handoff | Nested tab ID covered; actions retain separate contracts |
| Material Detail Notes | `material-detail.notes` | Currently unavailable placeholder | State exact current availability; never claim the whole app is read-only | Parent/nested tab IDs and corrected guidance covered |
| Rankings Dashboard | `analysis.rankings` | Current visible Materials projection | Metrics, filters, Top 10/25/50/100/All, Reset, Refresh, CSV and missing-score omission | Top-tab ID covered; action controls remain outside navigation sweep |
| Category Rankings | `analysis.category-rankings` | Current visible grouped projection | View mode, filters, rows/group 5/10/50/100/All, Reset, Refresh and CSV | Top-tab ID covered; action controls remain outside navigation sweep |
| Awards & Winners | `analysis.awards` | Current visible award projection | Award set, filters, Reset, Refresh, CSV, winner/runner-up and reasoning | Top-tab ID covered; action controls remain outside navigation sweep |
| Dashboard Insights | `analysis.dashboard-insights` | Current database projection | Counts, highest metrics, narrative insights and refresh ownership | Top-tab ID covered |

Important scope contract:

- Rankings default row count is Top 25.
- Category Rankings default rows per group is 10.
- Materials search/filter scope affects measurement visibility and all global
  analysis surfaces.
- Experimental Results remain selected-Series scoped.

v50.2.2 accepted status: all 22 destinations above exist as searchable Help
sections. Deterministic ID, mapping, scope and stale-guidance gates plus owner
runtime/visual acceptance pass.

## v50.2.3 — Reports, Website, Assistant and creator/output tools

| Surface | Stable Help ID | Owner / source of truth | Required reference coverage | Automation / current gap |
|---|---|---|---|---|
| Reports / PDF Export | `reports.overview` | Reporting pipeline/services | 12 templates, folder, Selected/Visible scope, preview/export, engineering package, public builds, logs/evidence | Only tab, folder/status/log and public package have IDs |
| Website Export | `website.overview` | SQLite template + website/report/publish services | Folder, templates, Preview, Production generation, FTPS Test/Production, restore, credential and evidence boundaries | Only top tab has ID |
| AI Assistant | `ai.overview` | Local deterministic scoped analysis | Visible MaterialID scope, briefs, sessions, collections, coverage status, dashboards and read-only output | Partial automation coverage |
| YouTube Research | `youtube.overview` | Local generated creator research | Generate, seven clipboard actions, thumbnails, comparisons, gaps, calendar, playlists and candidates | Entire tab/actions lack IDs |

Required leaf destinations:

- `reports.current-report`
- `reports.engineering-package`
- `reports.public-builds`
- `reports.scope-and-output`
- `reports.preview-evidence`
- `website.folder`
- `website.templates`
- `website.preview`
- `website.production-generate`
- `website.ftps-test`
- `website.ftps-production`
- `website.restore`
- `website.logs-evidence`
- `ai.visible-scope`
- `ai.planning-briefs`
- `ai.sessions`
- `ai.collections`
- `ai.coverage-status`
- `ai.output`
- `youtube.generate`
- `youtube.copy-actions`
- `youtube.thumbnail`
- `youtube.comparisons`
- `youtube.gaps`
- `youtube.calendar`
- `youtube.playlists`
- `youtube.candidates`

## Menus and runtime support surfaces

| Surface | Stable Help ID | Required coverage | Delivery |
|---|---|---|---|
| File menu / Storage | `menu.file-recovery`, `menu.storage` | Recovery Center, Choose Storage Folder, Exit and storage mutation boundary | v50.3 |
| Materials menu | `menu.materials` | Lifecycle commands plus Clear Search/Filters; same owners as Materials tab | v50.2.1 |
| Tools validation | `menu.tools-validation` | Validate Materials, Rebuild Computed Fields and rendering prototype | v50.2.4 candidate implemented |
| Tools updates/releases | `menu.updates`, `menu.release-publishing` | Update readiness/check/apply and application release/update publishing, distinct from Website FTPS | v50.3 |
| Help menu | `menu.help` | Documentation/F1, whitepaper, Changelog, Verification, Diagnostics and About | v50.2.4 candidate implemented |
| Recovery Center | `recovery.overview` | Catalog, verify, guarded restore, SQLite/Excel backup/restore and storage | v50.3 |
| Verification Center | `verification.overview` | Refresh, mutating Recalculate, export, PASS/FAIL and READY FOR PUBLISH | v50.3 |
| System Diagnostics | `diagnostics.overview` | Refresh, integrity, mutating recalculation, export and evidence sections | v50.3 |
| Whitepaper | `help.whitepaper` | Governed PDF export | v50.2.3 |
| Changelog | `help.changelog` | Packaged release history viewer | v50.2.3 |
| About | `help.about` | Version, storage model, license and notices | v50.2.3 |

### v50.3 planned safety and recovery destinations

- `menu.file-recovery`
- `menu.storage`
- `menu.updates`
- `menu.release-publishing`
- `recovery.overview`
- `recovery.catalog-and-verification`
- `recovery.sqlite-backup-and-restore`
- `recovery.excel-disaster-recovery`
- `verification.overview`
- `diagnostics.overview`
- `updates.guarded-apply-and-recovery`
- `publishing.application-release-and-update`
- `publishing.website-safety`
- `troubleshooting.verification-fail`
- `troubleshooting.backup-restore-blocked`
- `troubleshooting.interrupted-update`
- `troubleshooting.publish-failure`
- `troubleshooting.support-evidence`

v50.3 automation may open, inspect and close Recovery, Verification and
Diagnostics in a disposable profile. It must not invoke restore, recalculation,
update apply/recovery, Production, FTPS or owner-database/storage mutation.

v50.3.0 accepted status: all 18 planned destinations exist. Disposable
profile `20260727120029-14c0556f` passes read-only Recovery/Diagnostics/
Verification inspection, Full Verification 374/374 and exact state equality.
Owner runtime/safety acceptance passes on 2026-07-27.

v50.2.3 accepted status: all 34 overview, leaf and packaged Help destinations
contain substantive multi-paragraph control, scope, write, failure, evidence
and handoff guidance. The initial short leaf draft was rejected during owner
review. Profile `20260727111924-0056eda1` passes the refreshed deterministic
gates and owner runtime/content acceptance passes on 2026-07-27.

## v50.2.4 contextual entry points and coverage accepted

- All 22 top-level and 16 nested tabs have unique stable AutomationIds.
- F1 and Help for Current View share one nested-aware central Help resolver.
- Documentation remains the whole-system Start-to-finish entry point.
- Experimental Tensile/Impact/Stiffness editors and Results overview now have
  their planned stable nested destinations.
- Tools validation and Help-menu references are searchable.
- The disabled Website Export Preview dead-end is retired; Open Website Export
  selects the supported tab without generating or publishing.
- Disposable profile `20260727113500-3bc427ca` visits 22/22 and 16/16 surfaces,
  passes representative contextual links and Full Verification 373/373 with
  exact logical/business-state equality. Owner runtime/UI acceptance passes on
  2026-07-27; parent v50.2 is complete.

## Confirmed stale or misleading UI/documentation

| Location | Finding | Planned owner |
|---|---|---|
| Material Detail > Notes | Says the app is still read-only | v50.2.2 |
| Rankings/Category/Awards placeholders | Say Import data in a SQLite-native workflow | v50.2.2 |
| Awards explanatory text | Refers to future website exports; current publication ownership must be stated accurately | v50.2.2 |
| Reports side panel | Historical “first production report” implementation language | v50.2.3 |
| Reports preview placeholder | Says Import data | v50.2.3 |
| Settings header | Describes only USD exchange-rate settings despite multi-currency purchasing references | v50.2.1 |
| Printer delete dialog | Says quote snapshots will be introduced in the future although they already exist | v50.2.1 |
| Website menu | Disabled Website Export Preview retired; supported Open Website Export navigation added | v50.2.4 accepted |
| Regression checklist | Remains v33.5 with obsolete REPORT-500 assumptions | v50.3 |

## Save-mode contract

| Mode | Surfaces |
|---|---|
| Auto-save after edit | Materials, Manufacturers, Purchase grids/lines, Inventory, Printers, native measurements, Experimental graph |
| Immediate command persistence | Most Add, Duplicate, Archive/Restore and governed binding actions |
| Explicit save/record | Settings Save, Save Quote, Record Usage, report/export/build actions |
| Append-only accepted history | Usage ledger and corrections |
| Saved calculation snapshot with explicit deletion | Print Job Quotes |
| Retained purchase/inventory provenance | Received purchase and Inventory history |
| Read-only interpretation | Material Detail result surfaces, Rankings, Category Rankings, Awards and Dashboard Insights |
| Evidence only | Materials Manual Backup, Verification/Diagnostics exports and generated logs/manifests |

## v50.2.0 completion assessment

Inventory is structurally complete when:

- All 38 tab surfaces above remain represented.
- Every top-level tab has a stable overview Help ID.
- Every high-risk nested workflow has a stable leaf ID.
- Every menu/runtime surface has an assigned v50.2 or v50.3 delivery owner.
- Confirmed stale guidance has a recorded repair increment.
- v50.2.1-v50.2.4 implementation uses this matrix as its coverage contract.

## Mandatory v50.4 exhaustive control and editable-field audit

v50.2 proves stable Help destinations and navigation for the 38 recorded tab
surfaces. It does not claim that every individual control or editable grid
field has its own complete explanation. v50.4 must rebuild the structural
baseline and close that deeper gap after v50.3.

The v50.4 ledger must include every supported:

- top-level and nested tab, menu, runtime-built window and dialog;
- button and menu action;
- checkbox, radio option, selector and filter;
- text, numeric, date, password and multiline user field;
- editable grid and every user-editable column/cell type;
- owner-drawn/custom input surface and its supported edit interactions;
- visible status, validation, readiness and disabled/hidden state needed to
  understand an action.

Each ledger row must record:

1. Stable control key and AutomationId where technically appropriate.
2. Visible label/location and canonical UI/service/data owner.
3. Exact Help section or subsection that explains the control.
4. Purpose, prerequisites, allowed values, units and default meaning.
5. Validation, auto/immediate/explicit save timing and failure behavior.
6. Side effects, destructive confirmation and historical-data rules.
7. Cross-tab inputs, downstream handoff and publication/external boundaries.
8. Deterministic coverage evidence or an explicit manual-acceptance reason.

A tab overview, handler count or AutomationId alone is not proof that its
buttons and editable fields are documented. Completion requires zero supported
controls or editable fields with missing Help coverage, deterministic drift
detection for future additions and owner runtime/readability acceptance.
