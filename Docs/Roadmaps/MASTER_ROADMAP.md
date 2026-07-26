# 3DPIceland Engineering Platform — Master Roadmap

Current canonical release: **v47.0.2 — AI Collection Workflow Clarity**

Last runtime-accepted baseline: **v47.0.2 — AI Collection Workflow Clarity**

Current application candidate: **None — v47.0.2 is canonical**

Current roadmap increment: **v47.0.3 — Stable Coverage Identity**

Current acceptance note: v47.0.2 collection workflow is runtime accepted with Full Data Verification 349/349.

Candidate note: None.

This file is the canonical strategic roadmap. Completed build details belong in
`Docs/CHANGELOG.md`, `Docs/BUILD_HISTORY.md`, `Docs/MILESTONES.md` and
`Reports/VERIFICATION_HISTORY.md`.

## Milestone overview

| Version | Strategic focus | Priority | Status |
|---|---|---:|---|
| v36 | Stability & Data Integrity | ★★★★★ | Complete |
| v37 | Daily Workflow Optimization | ★★★★★ | Complete |
| v38 | Purchasing, Inventory & Cost Foundation | ★★★★★ | Complete |
| v39 | Daily Workflow Optimization from real usage | ★★★★★ | Complete |
| v40 | Experimental Testing & Platform Integration | ★★★★★ | Complete — production FTPS publishing validated |
| v41 | Engineering Intelligence | ★★★★★ | Complete — governed downstream handoffs delivered |
| v42 | Website & Approved Template Architecture | ★★★★☆ | Foundation delivered early |
| v43 | Deployment Platform | ★★★★☆ | Complete — canonical v43.8.9 runtime accepted |
| v44 | Daily Use, Reliability & Maintainability | ★★★★★ | Complete — canonical v44.7.18 runtime accepted |
| v45 | Material Model and Canonical Relationships | ★★★★★ | Complete — canonical v45.2.1 runtime accepted |
| v46 | Application Branding | ★★★☆☆ | Complete — canonical v46.0.0 runtime accepted |
| v47 | AI Assistant Workflow | ★★★★☆ | Current |
| v48 | Pricing and Usage Analytics | ★★★★☆ | Planned |
| v49 | Experimental Workflow Extension | ★★★★☆ | Planned |
| v50 | Comprehensive User Help | ★★★★★ | Planned |
| v51 | Governed Runtime Profiles | ★★★☆☆ | Research only |
| v52 | Optional OpenAI Assistant Integration | ★★★★☆ | Planned |

## Reconciliation of the older plans

Two earlier lists assigned different meanings to v39. The canonical meaning is
now **Daily Workflow Optimization from real usage and BUG_FEEDBACK_LOG**.

The older **v39 Cost Analytics** objective was not abandoned. Its purchasing
cost foundation was delivered during v38, while public pricing and value
analytics were delivered during v40.12 and v40.19. It therefore no longer
needs a separate future milestone.

Several v42 and v43 foundations were also delivered early inside v40.x. The
table records strategic completion state rather than forcing completed work
back into its originally predicted version number.

## Completed foundations

### v36 — Stability & Data Integrity

- SQLite is the Single Source of Truth.
- MaterialID is the canonical identifier.
- Verified Material Summary governs website, reports, whitepaper and documentation outputs.
- Verification Center is the release gate.

### v37 and v39 — Workflow Optimization

- Daily editing, filtering, layout, status and feedback improvements.
- Material Detail and primary engineering workflows optimized around real usage.
- Remaining usability work is handled as focused feedback-driven maintenance,
  not as a second competing roadmap milestone.

### v38 — Purchasing, Inventory & Cost Foundation

- Purchase Orders, receiving and multi-spool inventory.
- Landed-cost allocation, purchasing reports and purchasing intelligence.
- Pricing/value inputs available to downstream engineering intelligence.

### v40 — Experimental Testing & Platform Integration

Delivered:

- Generic experimental definitions, MaterialID-linked series and controlled runs.
- Native experimental measurements, results, analytics and charts.
- Experimental website publishing with canonical payloads and responsive charts.
- SQLite-approved website templates and canonical Preview/Production rendering.
- Methodology, engineering reports, whitepaper and manufacturer intelligence.
- Pricing & Value portal and manufacturer participation/submission workflow.
- Guarded explicit-FTPS publishing with production TLS validation, remote backups and staged replacement.

Completion:

- v40.20 adds one aggregate release gate across Engineering, Experimental,
  Website, Reporting, workspace order and release identity.
- Preview/Production parity and the HTML/redirect/whitepaper package contract
  are verified in the application.
- Live explicit-FTPS connection and passive-mode transfer were validated against
  the production web server on 2026-07-21 using a trusted Let's Encrypt certificate.
- A production publish created the remote backup, replaced the website package
  and confirmed the Manufacturers compatibility redirect to `index.html#manufacturers`.

## Complete — v41 Engineering Intelligence

Build on verified engineering, purchasing and pricing data without introducing
parallel calculations.

Delivered in v41.0–v41.6:

- Deterministic recommendation explanations from existing EngineeringScoreProfile data.
- Evidence coverage, strongest evidence, trade-offs and missing-data disclosure.
- Closest-ranked alternative comparison with score and axis differences.
- Recommendation Detail and reusable prompt integration.
- Dedicated Verification Center advisor release gate.
- Comparable alternatives constrained to the active filtered recommendation context.
- Price-aware hidden-gem discovery from canonical MSRP USD/kg data.
- Specialist alternatives with explicit axis gains and trade-offs.
- Verified-summary repeatability interpretation from CV and sample-count evidence.
- Summary-level variation review flags with explicit specimen-outlier safeguards.
- Cross-linked canonical MSRP, Inventory Engine output and active SQLite manufacturer context in engineering recommendations.
- Manufacturer and category rank, peer-count and group-average positioning from existing score profiles.
- Canonical report, whitepaper and Video Planner handoffs over existing governed Engineering Intelligence outputs.
- Internally calibrated consistency-score interpretation and documented purpose-built equipment limitations across all downstream
  surfaces.

Completion:

- Report and PDF presentation retain canonical HTML ownership.
- Whitepaper documents the interpretation boundary without embedding live rankings.
- Video Planner preserves MaterialID and existing score axes with recommendation-created ideas.
- Verification Center owns the aggregate v41.5 release gate.

### Immediate focused extension - v41.7 Report Portfolio Differentiation

Status: **Complete - all six reports and combined Engineering Report Package accepted**

The report dropdown currently over-promises distinct outputs: Material Summary,
Comparison, Manufacturer, Test Session and Printing Recommendation still share
the same generic fallback report. v41.7 will give each option its own governed
report contract, appropriate selected/visible scope, report code, content and
Verification Center identity/difference checks. Material Engineering Report is
the accepted reference for canonical HTML/PDF quality, not a template to copy
unchanged into every report type.

v41.7.1 establishes canonical native filter/scope parity, makes HTML and PDF part
of every current-report export and delivers the distinct REPORT-110 Material
Summary Report. Comparison, Manufacturer, Test Session and Printing
Recommendation are intentionally handled one at a time with user acceptance
between reports. The combined Engineering Report Package is the final portfolio
step so unfinished fallback reports cannot be included.

v41.7.2 completed the Canonical Material Projection Audit before Comparison
Report work. Native SQLite-backed Materials and MaterialID now own the material
universe, filter scope and current counts throughout the application. The old
`_materialsView` field and hidden legacy tab were removed; imported workbook
tables remain bounded ingestion payloads only. Verification distinguishes the
active material set, visible filtered set and tested-result subset. User
acceptance confirmed Verification PASS and downstream filter parity; Comparison
Report is now implemented as `REPORT-120` with selected-anchor and all-visible
contracts. User acceptance confirmed the presentation and Verification PASS.
Manufacturer Report is now implemented as `REPORT-130`: selected scope expands
from the chosen material to its manufacturer's complete active portfolio, while
all-visible scope preserves the exact current Materials filter and supports
multiple manufacturers. Visual review confirmed the selected-source workflow,
clear engineering-axis coverage and Verification PASS. Manufacturer Report is
accepted. Test Session Report is now implemented as `REPORT-140` with detailed
selected-material native records, an all-visible coverage ledger and explicit
disclosure for session metadata not yet stored in SQLite. Visual review and
Verification PASS are confirmed. Test Session Report is accepted. Printing
Recommendation Report is now implemented as `REPORT-150` with selected-material
guidance, an all-visible recommendation ledger, explicit printer-settings
boundaries and no Video Planner hooks. Visual review and Verification PASS are
confirmed. All six individual reports are accepted; the combined Engineering
Report Package is now implemented with six independent canonical HTML/PDF
subpackages, an indexed landing page, package manifest and JSON metadata.
End-to-end export, working index links and Verification PASS were confirmed on
2026-07-22. v41.7 is closed; Startup Performance & Safe Concurrency is now the
next focused extension.

### Complete - v41.8 Startup Performance & Safe Concurrency

Begin only after the current Report Portfolio has been implemented and accepted
report by report. This is a profiling-first optimization build, not a general
multithreaded rewrite.

v41.8.0 Startup Performance Instrumentation is implemented and awaiting runtime
acceptance. It records splash, MainWindow, canonical Materials, individual
workspace, first-render and deferred-intelligence timings in System Diagnostics
without changing startup order. The next optimization increment must be selected
from those measured cold/warm results.

The first Debug trace was accepted on 2026-07-22. It measured MainWindow
construction at about 1.0 seconds but exposed an uninstrumented delay of roughly
17 seconds before `Show()`. v41.8.1 removes the identified cause: bulk loading 200
Materials no longer queues approximately 201 identical downstream UI refreshes;
one measured coalesced refresh is scheduled instead. Runtime acceptance confirmed
Verification PASS and reduced observed Debug startup from about 19-20 seconds to
about 5 seconds, with first usable Materials rendered at 4.49 seconds in the trace.

The remaining v41.8 lazy-initialization and safe-concurrency candidates should now
be evaluated against this improved baseline. Do not introduce concurrency merely
to optimize sub-100 ms secondary workspace phases.

v41.8.2 adds deferred first-use visual warm-up for Tensile, Impact and Stiffness
after Materials has rendered. It keeps WPF realization on the UI thread, preserves
the selected workspace and records each tab's warm-up time. Runtime acceptance
confirmed Materials visible in about 5 seconds, all three workspaces warmed by
8.00 seconds and Verification PASS.

- Record cold/warm phase timings for SQLite/native storage reads, manager
  initialization, WPF binding and deferred intelligence consumers.
- Remove duplicated reads and repeated MaterialID/Verified Material Summary work
  before introducing concurrency.
- Lazy-initialize secondary tabs and calculations that are not required for the
  first usable Materials view.
- Use asynchronous `Task.WhenAll` loading only for proven-independent read paths,
  with a separate SQLite connection per concurrent operation.
- Keep all WPF control binding and collection mutation on the UI thread.
- Consider multi-core parallel calculation only for measured CPU-bound work;
  do not parallelize the approximately 200-row material projection by default.
- Preserve SQLite write serialization, backwards compatibility, canonical
  MaterialID/Verified Material Summary ownership and all Verification gates.
- Use the 2026-07-21 baseline of about 3 seconds to splash and 19 seconds to a
  visible Materials list for before/after comparison.

## Complete — v42 Website & Approved Template Architecture

Already delivered:

- Approved templates stored and versioned in SQLite.
- Canonical single-file portal navigation.
- Preview and Production use the same renderer.
- Database, Pricing, Experimental, Manufacturers and Methodology website surfaces.

Remaining work belongs to the Publishing roadmap and should be driven by
specific user-facing needs rather than a template rewrite.

### Complete - v42 Public Report Publishing

Evaluate which accepted engineering reports should become public website
artifacts. The platform's purpose is to share useful 3D-printing research, so
engineering measurements, methodology, limitations and deterministic guidance
are public-by-intent rather than treated as proprietary output.

- Material Engineering Report: downloadable/viewable per MaterialID.
- Comparison Report: curated comparison links or filter-based presets.
- Manufacturer Report: linked from each manufacturer profile and participation page.
- Test Session Report: public measurement transparency where test notes have
  passed a publication review.
- Printing Recommendation Report: application guidance from material detail pages.
- Material Summary and combined package: versioned database/research snapshots.
- Choose static generated files and stable URLs before considering server-side
  on-demand report generation.
- Add a public-field allowlist and Verification checks that exclude purchasing,
  inventory, credentials, local paths and unintended internal notes while
  preserving engineering evidence and limitations.
- Preserve Preview/Production parity, canonical HTML-to-PDF rendering,
  MaterialID identity and Verified Material Summary ownership.
- Consider website size, navigation, indexing and update frequency before
  publishing every report for every material automatically.

v42.1.0 Public Report Publishing Foundation is implemented and runtime accepted
as a local-only preview. It publishes one selected Material
Engineering Report through a dedicated 21-field public allowlist to the stable
`reports/materials/{MaterialID}/` route with canonical HTML, PDF, metadata,
manifest and assets. The preview build succeeded, Build Solution completed and
Verification Center reported all checks PASS after deferred warm-up completed.
Production FTPS integration remains intentionally disabled for the future
publication-selection and website-link phase.

v42.2.0 Canonical Public Material Selection is implemented and runtime accepted.
Materials provides an explicit, default-off `Public reports` opt-in
stored in SQLite by MaterialID. The shared Preview/Production renderer exposes
stable Material Engineering HTML/PDF links only for selected materials; the
existing engineering dataset remains unchanged for unselected materials. No
report packages have been added to FTPS publishing, which remains deferred until
the complete public report portfolio is ready. Acceptance confirmed restart
persistence, two-material batch packaging, a combined linked index and all-PASS
Verification Center.

v42.2.1 Public Engineering Report Content Expansion is complete and runtime
accepted. It enriches the allowlisted Material Engineering Report with
existing Verified Material Summary measurement results, score visualization,
material/manufacturer comparison context, full metric positions, decision
guidance, alternatives, governed interpretation, strengths, limitations,
trade-offs, recommended applications and bounded peer context. Internal
engine/database diagnostics are excluded from the public shell.
The canonical report JPG is shared by HTML and the PDF printed from that HTML.
No measurement or score is recalculated and FTPS report publishing remains
deferred. Acceptance confirmed canonical HTML/PDF presentation, complete
locale-safe radar labels and all-PASS Verification Center.

v42.7.0 Public Material Summary is implemented for runtime acceptance. It
projects the accepted REPORT-110 coverage and dataset ledger onto the stable
`reports/material-summary/` route using only active MaterialIDs explicitly
approved for public reports. Native module coverage, score availability,
material/manufacturer distributions and six-axis values remain governed by the
existing Verified Material Summary and score outputs. Canonical HTML/PDF,
typed allowlist and Verification gates are included; FTPS remains deferred.

v42.8.0 Public Engineering Report Package is implemented for runtime
acceptance. It verifies and catalogs the generated artifacts for all six public
report types under one stable `public-report-preview/index.html`, root manifest
and JSON catalog. Expected MaterialID, family and manufacturer HTML/PDF/metadata
files must already exist; missing artifacts block the package. The package is an
orchestration and navigation layer only and performs no engineering
calculations. Website portal integration and FTPS publication remain deferred
to the next accepted milestones.

v42.9.0 Public Website Report Portal is implemented for runtime acceptance. The
shared Preview/Production website renderer adds the reports portal and
contextual MaterialID, comparison and manufacturer links from the accepted
public-only catalog. Website export validates and stages every linked canonical
HTML/PDF/metadata artifact locally under `reports/`; Preview uses
`reports/index-test.html` and Production uses the stable `reports/index.html`.
The existing guarded FTPS file list is intentionally unchanged, so no report
artifact is uploaded by this milestone.

v42.9.1 Automatic Website Report Prerequisites is implemented for runtime
acceptance. Generate Preview, Generate Production and the manual package action
share one async ensure pipeline. It derives the expected public catalog from the
current opt-in selection, rebuilds only missing or presentation-stale report
types, verifies the complete canonical artifact set and refreshes the package
before local website staging. Production confirmation and backup behavior remain
in place; the guarded FTPS report upload scope is still unchanged.

v42.9.2 Public Report Data Freshness is implemented for runtime acceptance. A
deterministic SHA-256 revision covers the current public MaterialID selection,
report-relevant canonical SQLite sources and the exact current Verified Summary
and measurement projection consumed by the renderers. Preview, Production and the manual package action
rebuild the complete six-type public report set when that revision changes or is
missing, then persist and stage only the fingerprint metadata. This ensures a
new measurement also refreshes dependent ranks, family comparisons,
manufacturer portfolios and dataset summaries. FTPS report publication remains
deferred.

v42.10.0 Production Publish Readiness is implemented for runtime acceptance.
Generate Production ensures the data-fresh six-type package, stages stable
Production routes and writes an exact catalog-derived deployment allowlist with
local paths, safe remote paths, byte lengths and SHA-256 values. Preview,
backup and unrelated routes are excluded, and the root `/index.html` is ordered
last for later activation. Public HTML buttons use explicit `/index.html`
targets so the same routes work from local files and the hosted site. The
existing FTPS action remains unchanged until this local production handoff is
accepted.

v42.11.0 Guarded Public Report FTPS Deployment is implemented for runtime
acceptance. Publish Website first regenerates and confirms the complete current
Production package, reloads and SHA-256 verifies its exact catalog allowlist,
then requires a second live-publish confirmation. Existing remote targets are
copied into a timestamped backup tree and every planned artifact is uploaded
and size-verified in private staging before ordered activation. The root
`/index.html` is activated last. Failures restore replaced files and remove new
targets; Verification exercises the contract offline without contacting the
production server. The same milestone adds an isolated server Preview action:
all test content is published below `/preview/`, `/index-test.html` is activated
last as the stable browser shortcut, and the test allowlist cannot target the
Production root index, reports tree or manufacturer redirect.
Completed Production deployment backups now include governed existence, byte,
SHA-256 and activation-order metadata. A guarded restore action selects only the
latest eligible Production backup, snapshots the current live targets into a
separate recovery backup, stages and verifies the prior deployment, restores
the root index last and automatically reapplies the recovery snapshot if the
restore itself fails.

v42.11.1 Host-Compatible FTPS Staging is implemented for runtime acceptance.
Website Test and Production publishing validate exact plan artifacts with four
bounded parallel local workers, then use one runtime-proven retry-capable FTPS
session for remote backup/staging. Five-, three- and two-session live trials all
caused concurrent upload aborts; the two-session failure persisted after the
FileZilla passive range was expanded from 101 to 5,001 ports and server threads
were raised to eight. Every transfer must complete before the
activation barrier opens. Activation, rollback and entry-index-last ordering
remain sequential. Local worker count, remote session count plus staging and
activation timings are reported.
Future speed work should be manifest-driven delta publishing that omits
unchanged remote artifacts instead of adding FTPS sessions.

v42.12 Incremental FTPS Publishing is implemented for runtime acceptance.
Completed Test and Production deployment manifests carry the full published
route, byte and SHA-256 state. Every new plan is still fully validated locally,
but only changed, new or remotely missing artifacts enter backup, staging and
activation. The first post-upgrade run and any legacy, malformed, route-set
changed or Restore-invalidated state use safe fallback behavior. Delta backups
remain restorable because they describe exactly the files changed by that
deployment, while the full published state is stored separately for subsequent
comparisons. A fully unchanged deployment performs no remote mutation and
reports all skipped counts and transfer timings.
Runtime Test acceptance established the first full-state baseline and then
skipped 856 of 862 artifacts on the immediate repeat. Transferred bytes fell
from 37,578,896 to 1,089,493 (97.1% reduction); measured remote backup/staging
plus activation fell from 192.1 seconds to 0.8 seconds, approximately 240x for
those remote phases. Whole-workflow time remains reported separately.
Production acceptance established its isolated 861-artifact baseline and then
skipped 855 artifacts on the immediate repeat. Production bytes fell from
37,578,526 to 1,089,476 (97.1%); measured remote backup/staging plus activation
fell from 98.9 seconds to 0.8 seconds, approximately 124x for those phases. The
live `/3dp/index.html` endpoint returned HTTP 200 with the v42.12 identity,
material content and explicit reports route.
Runtime transport-close testing additionally made post-completion disconnect
best-effort and activation idempotent: a lost FTP response is reconciled against
the staged source and verified target before retry or rollback.
Deep backup/staging parent creation was moved from a long sequential control
preflight into the retry-protected remote file operation.
Failure output now names the exact guarded publish phase.

v42.13 Material Printing Settings Foundation is complete and runtime accepted.
The canonical MaterialID row now has optional nozzle-temperature, bed-temperature
and print-speed min/recommended/max fields with fixed °C and mm/s units, plus
cooling, drying time in hours, enclosure requirement and vendor-neutral printer/
slicer profile references. The schema change is additive, blank means unknown,
legacy JSON and Excel workflows remain compatible, and Verification Center owns
range, round-trip and internal-only allowlist gates. Public report models and
allowlists remain unchanged pending a later explicit public-publication scope.
Runtime acceptance confirmed normal startup, Verification Center Overall PASS,
persistence across restarts and responsive Materials arrow-key navigation.

v42.14 Base Material Printing Profiles is implemented for runtime acceptance.
Schema v25 makes the Base Material Catalog SQLite-canonical and owns one controlled
3DPIceland test/G-code baseline per base-material family. Material Detail resolves
through Base Material; the wide per-MaterialID printing block is removed from
Materials. Test-only values are intentionally not migrated. Public allowlists,
website rendering and FTPS remain unchanged. Remaining canonical JSON working
stores are a separate migration milestone.

v42.15 Deployment Settings Governance is implemented for runtime acceptance.
SQLite schema v26 owns FTPS host, port and username through Settings Manager.
Passwords remain host/user-scoped in Windows Credential Manager. Publisher,
connection test, rollback and restore use the same immutable configuration
snapshot while remote safety paths and the accepted transfer engine remain locked.

v42.16 SQLite Native Measurements is implemented for runtime acceptance. A
required-backup, one-transaction migration moves the conflict-free JSON superset
into normalized SQLite tensile, impact, stiffness and notes tables. Startup and
native saves then use SQLite only; JSON remains an untouched migration snapshot.
Verification owns canonical-marker and UI/SQLite count parity.

v42.17 SQLite Canonical Working Stores is implemented for runtime acceptance.
Materials and general Settings now start, load and save through SQLite only.
Conflict-free legacy JSON can seed an empty database once, then remains a
snapshot. Required schema-upgrade backup and Verification row/key parity protect
the transition; operational report/publishing JSON remains unchanged.

## Complete — v43 Deployment Platform

Delivered early:

- Guarded production website publishing over explicit FTPS.
- Trusted TLS certificate validation, passive transfers, timestamped remote
  backups, staged replacement and rollback protection.

Completed scope:

- Installer and portable deployment modes.
- Safe signed updater, durable rollback and interrupted-state recovery.
- Production deployment diagnostics, remote discovery/feed publishing and VM acceptance.
- Further operational hardening now belongs to feedback-driven maintenance, not an open v43 milestone.

Delivered in v43.1-v43.3.1:

- Database migration and backup compatibility diagnostics.
- Consolidated governed SQLite/Excel backup and recovery center.

### v43.1 — Local SQLite Backup and Restore

- Use the SQLite online-backup API instead of raw live-file copies and verify every retained backup.
- Inspect integrity, schema compatibility and canonical row counts before restore.
- Create a verified pre-restore recovery snapshot, atomically replace the database and roll back on failure.
- Restart without allowing stale UI close-save behavior to overwrite the restored state.
- Keep remote website restore and publishing unchanged.
- Treat Excel as secondary readable recovery data until a separate full round-trip milestone is runtime accepted.

### v43.2 — Excel Disaster Recovery

- Add a versioned manifest and exact canonical SQLite table inventory to the readable native Excel export.
- Preserve NULL, UTF-8, BLOB and long HTML values through typed Base64/chunk encoding.
- Verify sheet identity, row/column counts, chunk completeness and deterministic per-table SHA-256 before restore.
- Create a verified SQLite recovery backup and replace all governed tables in one foreign-key-safe transaction.
- Require SQLite foreign-key, integrity and Materials-count checks before commit, then restart through the established restore path.
- Exclude Credential Manager secrets and external referenced file contents; retain SQLite as the preferred first-line backup.

### v43.3 — Recovery Compatibility Center

- Inventory automatic, manual, pre-restore, pre-Excel-restore and external local SQLite backups in one UI.
- Show schema, integrity and canonical row counts with Ready, Migration required, Legacy/incomplete, newer/incompatible or corrupt
  status.
- Treat schema v27 as the minimum standalone SQLite recovery boundary; older backups require external transition evidence.
- Verify schema v27-v28 migrations on self-deleting temporary copies and require integrity plus canonical count preservation.
- Rerun compatibility verification before guarded restore and preserve the established recovery snapshot, atomic replacement and restart
  contracts.
- Keep remote website Production restore and publishing unchanged.

### v43.3.1 — Backup and Recovery Center UI

- Status: complete and runtime accepted on 2026-07-22.
- Replace scattered File commands with one Backup and Recovery Center entry.
- Place manual SQLite backup, SQLite verify/restore, governed Excel backup/restore and Open Storage Folder in the center; omit the
  retired legacy Excel database import.
- Remove manual-backup and duplicate database/backup-folder buttons from System Diagnostics.
- Retain Choose Storage Folder as configuration and keep diagnostics verification/reporting controls unchanged.

### v43.4 — Safe Application Update and Rollback

Status: first read-only signed-package verification increment runtime accepted on 2026-07-22; application-file staging/apply/rollback
remains scheduled before installer work.

- Keep application binaries, SQLite user data and published website state as three separate update/recovery boundaries. v43.4 changes
  only the application-binary boundary and reuses the accepted SQLite backup/migration checks; FTPS publishing and remote rollback
  remain unchanged.
- Define one versioned release manifest containing release identity, minimum/current supported database schema, complete governed file
  inventory, byte lengths and SHA-256 hashes. Integrity hashes alone are not publisher authentication: production auto-update must
  additionally require Authenticode or a detached signature rooted in an embedded trusted public key.
- Download or select an update into a unique staging folder outside the live application directory. Reject downgrade, same-version,
  newer-unsupported-schema, missing/extra governed files, path traversal, hash mismatch, invalid signature and incomplete package states
  before shutdown.
- Create and verify a manual SQLite recovery backup before any update that may start a newer schema. Never package, copy or overwrite
  the active database, backup history, Credential Manager secrets or configured storage-folder pointer as application files.
- Use a small external updater/helper because the running single-file EXE cannot safely replace itself. The helper must preserve the
  current app package as a versioned rollback snapshot, install by same-volume atomic renames where possible, record a durable
  transaction manifest and restart the staged version.
- Require a startup health acknowledgement from the new version. If process launch, startup/schema compatibility or the bounded
  post-update health contract fails or times out, the helper restores the prior app package and relaunches it; the pre-update SQLite
  recovery backup remains available for guarded data rollback rather than being restored silently.
- Preserve the last known-good application package independently of the rotating SQLite backup set. Cleanup must never remove the active
  version, an incomplete transaction or the only known-good rollback package.
- First implementation increment should provide manifest/package verification and update-readiness diagnostics without applying
  untrusted packages. Apply/update automation follows only after the signing trust root and helper transaction tests are runtime
  approved.
- Add Verification Center gates for manifest parsing, path containment, hash/signature policy, version/schema decisions, update-state
  recovery and strict separation from SQLite/website publishing.
- Installer and portable modes must later consume this same manifest, package layout and rollback contract rather than inventing a
  second deployment path.

#### v43 delivery sequence and status

| Release | Objective | Status |
|---|---|---|
| v43.4.0 | Signed update-package readiness verification | Complete; runtime accepted |
| v43.4.1 | Release signing and governed package creation | Complete; runtime accepted |
| v43.5.0 | Isolated transactional updater engine and external helper | Complete; runtime accepted |
| v43.5.1 | Guarded live process orchestration, health acknowledgement and Apply UI | Complete; runtime accepted |
| v43.6.0 | Update/deployment diagnostics and interrupted-state recovery | Complete; runtime accepted |
| v43.7.0 | Installer and portable deployment modes | Complete; runtime accepted |
| v43.8.0 | Remote signed update discovery, download and publishing | Complete; runtime accepted |
| v43.8.8 | Production consolidation above all VM candidates | Complete; runtime accepted and production published |
| v43.8.9 | SQLite dependency security, BOM-safe feed and repository/documentation closure | Complete; canonical and runtime accepted |

### v43.4.1 — Release Signing and Package Creation

Status: complete and runtime accepted on 2026-07-22. Production CNG trust root, canonical signed package command and authoritative
application-verifier probe passed. Private-key rotation/recovery remains required before unattended updater deployment.

- Generate a dedicated ECDSA P-256 production release keypair. Embed only the public key in the application; keep the private key
  outside the repository, application package, SQLite storage/backup folder and normal build output.
- Add an explicit release-packaging command that consumes the private key, produces the existing versioned manifest, hashes the complete
  governed Release file inventory and writes the detached manifest signature into the update ZIP.
- Refuse dirty, mismatched-version, incomplete or unexpected build output. Include licensing/notices and required runtime assets while
  excluding SQLite data, backups, credentials, storage configuration, reports, publish staging and developer files.
- Prove that the production application accepts a correctly signed newer package and blocks a modified payload, modified manifest, wrong
  key, missing signature, downgrade, same version, path traversal and unsupported SQLite schema.
- Keep package inspection read-only; no live application-file replacement is authorized in v43.4.1.

### v43.5 — Transactional Application Update and Rollback

v43.5.0 status: complete and runtime accepted on 2026-07-22. Isolated transaction engine, external helper, signed ten-file package and
commit/rollback Verification passed. Live application mutation remains intentionally disabled. v43.5.1 will add process wait/launch,
startup health acknowledgement and guarded Apply.

v43.5.1 status: complete and runtime accepted on 2026-07-22. A signed 11-file v43.5.1 portable base applied the signed v43.5.2
candidate, created a verified SQLite backup, committed durable transaction state, restarted and acknowledged exact version/schema
health; Verification passed 293/293. Repository/Visual Studio build outputs remain explicitly excluded as update targets.

- Add a minimal external updater/helper so the running single-file application never attempts to overwrite itself.
- Require successful signed-package readiness and a verified SQLite recovery backup before shutdown or application-file mutation.
- Stage on the same volume, preserve the active version as last-known-good and use durable transaction state plus atomic directory/file
  renames where supported.
- Restart the new version and require a bounded startup health acknowledgement covering release identity, executable inventory and
  SQLite schema/startup compatibility.
- Automatically restore and relaunch the prior application package if staging, replacement, launch or health acknowledgement fails.
  Never silently roll SQLite data back; retain the verified pre-update database backup for guarded recovery.
- Test success, crash/power-loss boundaries and rollback from each durable transaction phase before runtime acceptance.

### v43.6 — Update and Deployment Diagnostics

v43.6.0 status: complete and runtime accepted on 2026-07-22. Visual Studio Debug confirmed canonical identity, read-only history with
one prior Committed transaction, zero incomplete transactions and Verification PASS 294/294. The extension preserves the v1 transaction
schema and v43.5.1 process/package contracts.

- Show active, staged and last-known-good application versions, package/signature identity, SQLite pre-update backup and durable
  transaction status.
- Detect and safely resume or roll back an incomplete update after process termination or machine restart.
- Record concise update/rollback history without credentials or private signing material.
- Add cleanup rules that preserve the active version, incomplete transactions and the only known-good rollback package.
- Extend System Diagnostics and Verification Center without mixing application-update state with remote website FTPS deployment state.

### Later v43 — Installer and Portable Deployment

v43.7.0 status: per-user installer, exact six-file governed portable ZIP and isolated application-release deployment plan are runtime
accepted. Clean VM install/restart, SQLite transfer, credential isolation, branding, Verification Center, live `/downloads` publish and
browser download passed. Remote signed-update discovery/download remains follow-on work.

- Build installer and portable modes only after v43.5-v43.6 runtime acceptance.
- Both modes must consume the same signed package, manifest, governed file inventory, data separation and rollback contract; no second
  updater architecture.
- Preserve the configured SQLite storage folder independently of application install location and uninstall behavior.

## Pre-v44 Repository Privacy, Provenance and Hygiene Audit

Status: **Complete; Audits 1-7 passed. Option A replaced GitHub history with one sanitized root commit, and the repository is approved
for public visibility subject to the retained-copy cautions recorded below.**

Purpose: prove that the application, distributable packages, local workspace and complete Git/GitHub history are safe before any
public-repository or open-platform work begins. Preserve accepted runtime behavior and use small backwards-compatible corrections only.
Each gate must record search scope, exact evidence, risk classification, decision and verification result before the next gate starts.

### Audit 1 — Historical 176-material dataset

Status: complete; **gate failed for repository privacy, while runtime/distributable clean-profile isolation remains proven. Remediation
is blocked on complete-history Audit 6.**

- Search current tracked/untracked source, generated SQL, migrations, scripts, embedded resources, tests, docs and retained artifacts
  for the historical 176-material dataset, representative private material markers and any alternate serialized form.
- Inspect SQLite creation/seed paths separately from user-owned databases; never copy or expose the owner database during this audit.
- Classify every match as executable seed, inactive historical code, documentation, build output or false positive.
- Gate: no distributable/runtime path may create private Materials on a clean profile. Do not delete evidence until Git-history exposure
  is assessed in Audit 6.

Audit 1 evidence (2026-07-22):

- `App/FilamentDbApp/MainWindow.xaml.cs` contains exactly 176 historical `NativeMaterialRow` literals (`[private-material-id-removed]`
  through a non-contiguous final `[private-material-id-removed]`) inside `#if false`. The compiler excludes them and
  `GetDefaultNativeMaterialRows()` returns an empty list, but the private dataset text is tracked and visible in current GitHub
  `master`.
- `App/FilamentDbApp/Assets/website-template-index.html` is a tracked 590,345-byte data-bearing snapshot. Its `const DATA` payload
  contains 200 unique MaterialIDs (`[private-material-id-removed]`-`[private-material-id-removed]`) plus measurement/sample, pricing and
  video-link fields. Packaging excludes this file, but current-tree repository privacy fails.
- Retained local signed packages v43.4.1, v43.5.0 and v43.5.1 contain the same 590,345-byte website snapshot. Their application EXEs did
  not expose the known `[private-material-id-removed]` or private-name markers in UTF-8/UTF-16 scans. The production v43.8.8 signed
  package contains neither known marker and preserves the accepted zero-data clean-install boundary.
- No tracked or current non-build `.sqlite`, `.sqlite3`, `.db`, `.sql`, `.csv`, `.tsv`, `.xlsx` or `.xls` data file was found. SQLite
  inserts in `LocalDatabase` are parameterized persistence/import paths, not hard-coded material seeds.
- Immediate safety decision: keep the GitHub repository private; do not publish or mirror it. Do not remove current files or rewrite
  history until Audit 6 establishes every reachable copy and an approved remediation sequence.

### Audit 2 — Embedded HTML and website snapshots

Status: complete; **gate passed for current installer/portable/update payload isolation, but failed for current-tree repository privacy.
Remediation remains blocked on complete-history Audit 6.**

- Inventory literal HTML, HTML templates, JavaScript payloads, compiled resources and `.html` files in source, build scripts and package
  inputs.
- Distinguish small renderer contract fixtures from historical website snapshots or data-bearing templates.
- Gate: no private website snapshot, embedded dataset or obsolete HTML artifact may enter installer, portable or update packages;
  required renderer fixtures must be minimal and documented.

Audit 2 evidence (2026-07-22):

- Exactly two tracked `.html` files exist. `App/FilamentDbApp/Assets/website-template-index.html` is the 590,345-byte
  obsolete/data-bearing snapshot identified by Audit 1: one `const DATA` payload, 200 unique MaterialIDs and private
  engineering/pricing/link data. It is not referenced by the project or runtime template loader; the canonical active main-site template
  is SQLite-governed. Keeping this snapshot in current Git nevertheless fails repository privacy.
- `App/FilamentDbApp/Assets/Website/MethodologyPortal.html` is a 20,400-byte intentionally embedded static methodology fragment. It
  contains no `const DATA`, MaterialID, price/cost field, FTPS identity or application dataset. Its three YouTube methodology links are
  public content, and `BuildMethodologyPortalHtml()` loads the named embedded resource for the website methodology section and
  Verification checks.
- HTML-like source in `MainWindow.xaml.cs`, website services and reporting services is renderer/validation code, not a stored customer
  dataset. Report builders HTML-encode typed model values; the Verification Center website-link fixture uses only synthetic
  `MAT-PUBLIC-*` identities. The 176 real MaterialID literals counted in `MainWindow.xaml.cs` belong solely to the separately identified
  disabled Audit 1 block, not to an HTML fixture.
- The v43.8.8 signed update ZIP contains only the application EXE, updater EXE, governed icon/logo, license, notices and signed
  manifest. The v43.8.8 portable ZIP contains the same governed application files without the manifest. Neither package contains an
  `.html` file or the obsolete snapshot; the installer is built from that already-verified extracted payload.
- Deployment construction explicitly rejects `website-template-index.html`, SQLite/database/spreadsheet/tabular data files, `native-*`
  files and known private seed markers. This is a useful second boundary, while the signed packager's exact governed inventory is the
  primary package boundary.
- Decision: retain the two HTML files and existing evidence unchanged until Audit 6 maps Git-history exposure. In remediation, remove
  the obsolete data snapshot from the current tree/history as approved, retain the data-free embedded methodology fragment, and preserve
  the current SQLite template and governed packaging contracts.

### Audit 3 — FTPS identities, endpoints and credentials

Status: complete; **clean-profile credential isolation and v43.8.8 package gates pass, but current-tree repository privacy fails because
the private FTPS username remains in tracked historical documentation and a negative deployment marker. Remediation remains blocked on
complete-history Audit 6.**

- Search case-insensitively for `3dpiceland`, `[private-ftps-identity-removed]`, `www.iskort.is`, FTP/FTPS URIs, usernames, passwords,
  tokens and connection strings across tracked/untracked files and package inputs.
- Classify public governed endpoints separately from private account identities and secrets. Confirm clean-profile FTPS host/user remain
  empty and passwords remain only in Windows Credential Manager.
- Gate: public HTTPS product endpoints may be explicit; private usernames, passwords and owner-specific defaults must not exist in
  distributable code or repository content.

Audit 3 evidence (2026-07-22):

- Clean SQLite schema creation inserts `DeploymentSettings` with an empty FTPS host, port 21 and empty username.
  `DeploymentSettingsRecord` has the same empty host/user defaults. Verification explicitly checks that a clean profile excludes the
  private deployment identity; the successful VM clean-install observation agrees with this code path.
- No password column or password configuration value exists. The password box falls back to a host/user-scoped Windows generic
  credential, and `WindowsCredentialService` uses `CredWriteW` only after a successful encrypted connection and `CredReadW` for later
  publishes. Password bytes are cleared after the native write. Source and non-build workspace scans found no non-empty
  password/secret/token/API-key assignment, credential-bearing FTP URI, `.env`, private-key, certificate-bundle or similarly named
  secret artifact.
- Exact private username matches in the current tree are limited to: a negative deployment-package rejection marker, the historical
  v39.2 changelog entry, the historical v40.15 build note and this audit plan/evidence. It is not a model default, SQLite seed,
  connection constructor default or public report value. The build note that says existing installations seed that identity describes
  the earlier compatibility state and is stale for clean-profile behavior after the deployment-isolation correction.
- `www.iskort.is` / `iskort.is` HTTPS references are intentional public product routes: website/report links, installer/support pages,
  downloads and the signed update feed. The update client enforces HTTPS, exact `www.iskort.is` host and `/3dp/updates/` root before
  accepting a package URL. These public browser endpoints are not authentication material.
- The v43.8.8 signed update ZIP, portable ZIP and installer contain no detectable private username marker. Text deployment/feed metadata
  contains no private username; the feed intentionally contains the public HTTPS update route. The package payload contains no FTP/FTPS
  credential URI.
- Repository-level gate result: FAIL until the private username is removed from current tracked prose and the rejection rule is
  converted to a non-identifying policy or hashed/test-safe fixture. Runtime/distributable gate result: PASS. No credential rotation is
  presently indicated because no password, token or private key was found, but Audit 6 must still scan every reachable historical blob
  and ref before that conclusion is final.
- Decision: do not alter FTPS runtime behavior, endpoints, documentation history or rejection markers yet. Retain evidence until Audit 6
  maps GitHub exposure; then apply the smallest approved current-tree and, if necessary, history remediation while preserving empty
  clean-profile defaults and Windows Credential Manager storage.

### Audit 4 — `System.IO`, path aliases and repeat compile failures

Status: complete; **gate passed. The recurring failure is a compile-time WPF namespace ambiguity/editing-convention issue, not a missing
`System.IO` dependency.**

- Collect every compile error/fix pattern involving `Path`, `File`, `Directory`, `System.IO`, conflicting WPF types and the existing
  `IOPath` alias.
- Determine whether failures come from missing namespace imports, ambiguous type names, partial-class conventions or inconsistent
  editing practice.
- Define one repository convention and enforce it with the smallest suitable mechanism: shared/global using, explicit alias,
  analyzer/build check and/or repository guidance.
- Consider a durable Codex memory note only after the convention is proven; memory supplements repository enforcement and must not be
  the sole safeguard.
- Gate: Debug/Release builds and a targeted ambiguity probe pass without one-off namespace repair.

Audit 4 evidence and remediation (2026-07-22):

- `MainWindow.xaml.cs` imports both `System.IO` and `System.Windows.Shapes`. Both namespaces define `Path`; a newly added bare `Path.*`
  expression therefore produces compiler error CS0104. `File` and `Directory` have no current WPF collision, and no missing `System.IO`
  assembly/package was found. This explains why the issue repeatedly appeared immediately after source edits and disappeared after
  qualification.
- Git history confirms the same repair pattern: v34.2.3 `Path Diagnostics Build Fix` changed bare `Path.GetFullPath`/separator calls to
  `System.IO.Path`; v43.5 updater work introduced a file-local `IOPath` alias for new transaction paths. Current `MainWindow.xaml.cs`
  had no remaining bare `Path.*` call before this remediation, but mixed fully-qualified and alias styles made recurrence likely.
- Added project-wide `IOPath`, `IOFile` and `IODirectory` aliases in `GlobalUsings.cs`; removed the redundant file-local `IOPath` alias.
  Added a compile-only `IoNamespaceConventionProbe` that deliberately imports `System.Windows.Shapes`, resolves WPF `Path`, and
  exercises all three IO aliases. Any future removal/breakage of the convention now fails Debug/Release compilation.
- Added root `AGENTS.md` repository guidance requiring the aliases for new filesystem code, explicit qualification for WPF shapes, and
  isolated `ArtifactsPath` builds when the running application locks normal output. This repository-owned guidance is the durable
  project memory; a separate assistant-memory-only rule is intentionally not the sole safeguard.
- Initial normal Debug build encountered MSB3027/MSB3021 because the running `3DPIcelandFilamentDB` process (PID 61340) held
  `bin/Debug/net9.0-windows/UpdateCore.dll`. This was an output-file lock, not a C# or `System.IO` failure. The application was not
  stopped. Release independently passed 0 warnings / 0 errors.
- After restoring to an isolated temporary `ArtifactsPath`, Debug and Release both passed with 0 warnings / 0 errors while the
  application remained running. The targeted ambiguity probe compiled in both configurations. No runtime, SQLite, website, report, FTPS
  or deployment behavior changed, so no new runtime Verification Center check was added for this compile-only convention.

### Audit 5 — Workspace structure and obsolete files

Status: complete; **inventory/decision gate passed. No file was moved, consolidated or deleted; privacy-sensitive cleanup remains
blocked on Audit 6 and approved execution belongs to Audit 7.**

- Inventory the repository root and major subtrees by ownership, tracked status, size, duplication, generated output and last relevant
  use.
- Identify obsolete scripts/docs/assets, duplicate canonical documents, misplaced outputs and candidates for consolidation without
  deleting user data or accepted evidence.
- Gate: present a keep/move/consolidate/delete table first. Destructive cleanup requires explicit target validation and must leave
  build, Verification and packaging contracts intact.

Audit 5 evidence (2026-07-22):

- Git tracks 224 files totaling 6,325,314 bytes; there are no non-ignored untracked files, no case-colliding tracked paths and no exact
  duplicate tracked-file hashes. The Git worktree was clean and `master == origin/master` at audit start.
- The non-Git workspace is approximately 1.31 GB. `App/artifacts` owns 611,261,621 bytes of retained releases, while ignored `bin`,
  `obj`, `.vs` and `App/FilamentDbApp/artifacts/codex-build` outputs account for approximately 697 MB and are reproducible. The running
  application may lock normal Debug output, as confirmed in Audit 4.
- The v43.8.8 `feed` and `signed` update ZIPs are byte-identical (SHA-256
  `670AEA149958B561B061A8F532E477423F32E00EFD6E95306B1FC93058D71A5D`). Their duplication follows publishing-stage folder ownership
  rather than differing content. The older v43.4.1, v43.5.0 and v43.5.1 archives each retain the private website snapshot; current
  v43.8.8 archives do not.
- Release identity is inconsistent only in secondary documentation: `FilamentDbApp.csproj`, `BuildInfo` and current build notes identify
  v43.8.8, but `Docs/VERSION.txt` still says v43.5.1; README development focus still says v40.20.0; `PROJECT_STATUS`, `RELEASES`,
  regression/daily-use checklists and known-limitations introductions retain older release baselines. These files are not package
  inputs, but they are misleading maintenance surfaces.
- Three tracked SVGs under `App/FilamentDbApp/Assets/Documentation` have no filename reference in source, project metadata or
  documentation. They were created for v40.14 whitepaper work but are neither embedded nor copied by the current project. The icon/logo
  assets and embedded methodology fragment all have active owners and must stay.
- `App/build_release.bat` has not changed since v29.2 and performs only a direct main-app publish; it bypasses the updater, exact signed
  inventory, verifier and deployment/feed workflow. It must not be treated as a production release command. `run_from_source.bat`
  remains a valid convenience launcher; the three PowerShell scripts are the current governed signed/update/deployment pipeline even
  when not linked from README.
- `MainWindow.xaml.cs` is approximately 1.68 MB and `LocalDatabase.cs` approximately 198 KB. Their size is a maintainability risk, but
  splitting partial classes/services is a separate code-refactoring project with regression risk—not workspace cleanup—and is
  intentionally outside Audit 5.

Audit 5 decision table:

- **Decision:** Keep
  - **Exact scope:** `App` source/projects, `UpdateCore`, updater, `Tools`, installer definition, root governance/license files
  - **Reason and prerequisite:** Active build, verification, update and deployment ownership.

- **Decision:** Keep
  - **Exact scope:** Active icon/logo files, embedded `MethodologyPortal.html`, `run_from_source.bat`, all three governed PowerShell
    release scripts
  - **Reason and prerequisite:** All have a current runtime, documentation, convenience or release-pipeline owner.

- **Decision:** Keep
  - **Exact scope:** v43.8.8 production installer, portable, signed package, feed and deployment metadata
  - **Reason and prerequisite:** Canonical reproducible release evidence. Retain current folder separation until the publish workflow
    documents whether the identical feed ZIP may be generated on demand.

- **Decision:** Keep for now
  - **Exact scope:** Changelog, build history, milestone history, releases inventory, project history and specialist roadmaps/docs
  - **Reason and prerequisite:** They overlap by design but have non-identical content and historical references. Consolidation requires
    a documentation-governance pass, not blind deduplication.

- **Decision:** Consolidate/update
  - **Exact scope:** `Docs/VERSION.txt`, README current focus, `PROJECT_STATUS.md`, `RELEASES.md`, old regression/daily-use checklist
    headers and `KNOWN_LIMITATIONS.md` introduction
  - **Reason and prerequisite:** Align secondary documentation to canonical v43.8.8 or label it explicitly historical. Perform in Audit
    7 after history/privacy decisions so one release-document pass is sufficient.

- **Decision:** Replace or delete
  - **Exact scope:** `App/build_release.bat`
  - **Reason and prerequisite:** Obsolete unsafe production affordance because it bypasses the governed updater/package verifier. Prefer
    removal or a wrapper that calls the canonical signed workflow; decide in Audit 7 and re-run packaging verification.

- **Decision:** Move or delete
  - **Exact scope:** Three unreferenced `Assets/Documentation/*.svg` files
  - **Reason and prerequisite:** No current project/source/doc owner. If historically useful, move under documentation assets; otherwise
    delete after Audit 6 confirms history/evidence needs.

- **Decision:** Delete after Audit 6
  - **Exact scope:** Tracked `Assets/website-template-index.html` and retained v43.4.1/v43.5.0/v43.5.1 local ZIPs
  - **Reason and prerequisite:** They contain private dataset snapshots. Preserve until the complete Git-history exposure map and
    remediation evidence are complete.

- **Decision:** Delete when app is closed
  - **Exact scope:** Ignored `.vs`, all `bin/obj`, and `App/FilamentDbApp/artifacts/codex-build` outputs
  - **Reason and prerequisite:** Approximately 697 MB of reproducible local output. Validate resolved paths and avoid locked running-app
    files; canonical `App/artifacts/v43_8_8-production` is excluded from this cleanup.

- **Decision:** Ignore/remove locally
  - **Exact scope:** Empty untracked `.agents` directory
  - **Reason and prerequisite:** No files, Git content or runtime owner; immaterial and safe to leave.

- **Decision:** Defer
  - **Exact scope:** Splitting `MainWindow.xaml.cs` and `LocalDatabase.cs`
  - **Reason and prerequisite:** Valuable future maintainability work, but not file hygiene and not safe to combine with privacy/history
    remediation.

- Decision: make no destructive workspace change in Audit 5. Audit 6 must first establish which current and historical blobs are
  GitHub-visible. Audit 7 may then execute explicitly approved, path-validated cleanup, followed by Debug/Release, Verification Center
  and complete signed-package/deployment verification.

### Audit 6 — Complete Git history and GitHub exposure

Status: complete; **gate failed. Private material datasets, a private FTPS identity, personal workstation paths, Visual Studio
user-state and one diagnostics export remain reachable from GitHub `master` history. No literal password, token or private key was
found.**

- Search every reachable commit, branch and tag—not only `master` HEAD—for the 176-material dataset, private website snapshots, FTPS
  usernames, credentials, personal paths, databases, spreadsheets, generated reports and other non-public content.
- Compare local refs with GitHub-visible branches/tags and scan historical blobs by content, filename and size. Treat deletion from the
  current tree as insufficient if a blob remains reachable in history.
- Classify findings as public-safe, privacy-sensitive, secret requiring rotation, or historical data requiring removal.
- Gate: produce a remediation decision before changing history. Any `git filter-repo`/BFG rewrite, force-push, tag deletion,
  collaborator coordination or credential rotation is a separate destructive operation requiring explicit approval, backup and
  post-rewrite verification.

Audit 6 evidence (2026-07-22):

- Live remote refs contain `master` at the audited head plus an unrelated one-commit `main` branch containing only `LICENSE`; no tags
  exist. Remote symbolic `HEAD` points to `master`. The local cached `origin/HEAD` still points to `origin/main` and is stale, but this
  does not change reachability. `master` has 364 commits, `main` has one unrelated root commit, and all sensitive commits are contained
  by `origin/master`.
- All reachable refs comprise 365 commits and 3,277 unique blobs (340,925,830 logical bytes across historical blob versions). The scan
  covered text and binary blobs by content, filename, extension and size rather than checking only current paths.
- Private material markers occur in 222 unique blob versions. v27.3 commit `[historical-commit-removed]` introduced the 176-row
  `GetDefaultNativeMaterialRows()` source dataset as an active populated fallback; it remained executable throughout the older history
  and was only compile-disabled by `[historical-commit-removed]` on 2026-07-22. Current `master` still tracks the text inside `#if
  false`.
- Five unique historical website-snapshot blobs are reachable across the old and current path prefixes. They contain respectively 163,
  176 or 200 unique MaterialIDs and include measurement data; the 200-material variants also include pricing fields. The current tracked
  590,345-byte variant is one of these reachable blobs.
- The private FTPS username occurs in 154 blob versions across historical SFTP/FTPS code/defaults, UI, settings, documentation and the
  later negative deployment marker. It was introduced with the former SFTP workflow and later removed from clean-profile runtime
  defaults, but current prose/marker references and all earlier identity-bearing blobs remain reachable.
- Commit `[historical-commit-removed]` accidentally added Visual Studio `.vs` databases, indexes, caches, layout and user-state. Commit
  `[historical-commit-removed]` removed them from the current tree, but their blobs remain reachable. Three of those opaque/binary blobs
  expose the personal Windows user path and may retain additional IDE-indexed source context that cannot be proven safe by plain-text
  scanning.
- Commit `[historical-commit-removed]` added `3DPIceland_FilamentDB_Diagnostics_20260720_071430.txt`; `[historical-commit-removed]`
  later deleted it. The reachable export contains personal database/executable/storage paths, project row counts, two MaterialID
  references and detailed verification/system state. It contains neither the FTPS username nor a password marker.
- No historical application `.sqlite`, `.sqlite3`, general application `.db`, Excel workbook, CSV/TSV, SQL dump, release ZIP/archive,
  certificate bundle or private-key file was found. The only `.db` paths are historical Visual Studio Copilot index databases. No
  private-key header, GitHub/AWS/Slack token signature, credential-bearing FTP URI, literal password/secret assignment or
  password-bearing connection string was found across reachable blobs. Apparent password hits were variable/member/method expressions in
  the Credential Manager flow, not stored values.
- Current tracked files contain domain-owned `iskort.is` contact addresses intended for product/site contact. Separately, Git commit
  author/committer metadata exposes non-noreply personal/local email domains; changing visible file content will not remove that
  metadata.
- Current-tree deletion alone is insufficient: every prior commit remains downloadable from `origin/master`. Removing only branch `main`
  would not affect the exposure because it contains only the unrelated license root; removing only the affected current files would also
  leave all historical blobs reachable.

Audit 6 remediation decision:

- **Option:** A — New clean root (recommended)
  - **Result:** Build one sanitized root commit from the approved current tree, force-replace `master`, remove the unrelated `main`
    branch, and publish only after remote verification
  - **Trade-off / decision:** Strongest and easiest-to-prove privacy boundary for this young private repository; intentionally discards
    public Git history while a secure local bundle can retain evidence.

- **Option:** B — Targeted `git filter-repo` rewrite
  - **Result:** Remove both website-snapshot path histories, `.vs/**`, the diagnostics export, private FTPS identity text, personal
    paths/metadata and every historical active/disabled material-seed block while preserving other commits
  - **Trade-off / decision:** Preserves more history but requires custom blob transformation for many evolving `MainWindow.xaml.cs`
    versions, author metadata rewriting and a materially harder residual-content proof. Higher risk of missing a variant.

- Recommended sequence, requiring explicit approval before execution: keep the repository private; freeze pushes; create and verify an
  offline sensitive Git bundle with restricted local storage; sanitize the current tree under Audit 7; choose Option A unless historical
  commit continuity is essential; force-push the replacement `master`; delete obsolete remote refs; have any other clones
  discarded/recloned; re-scan every live remote ref/blob; account for GitHub cached objects/forks/pull-request refs before public
  visibility; then run Debug/Release, Verification Center, signed-package verifier and deployment checks.
- Credential decision: no mandatory rotation is indicated by this scan because no password/token/private key was found. The FTPS
  username and public endpoint are exposed identifiers, not authentication secrets. Optional password rotation after the rewrite remains
  a defense-in-depth choice, especially if the same password has ever been shared elsewhere.
- Until that sequence passes, repository-public-readiness is **NO**. No history rewrite, force-push, branch deletion, file deletion,
  credential change or remote visibility change was performed by Audit 6.

### Audit 7 — Remediation and public-readiness closure

Status: complete; the owner approved Option A (new clean root) on 2026-07-22, and every current-tree, build, package, replacement-root
and remote verification gate passed.

- Apply only approved remediations, smallest first; keep website/report/FTPS runtime engines outside scope unless a confirmed exposure
  requires a targeted correction.
- Re-run current-tree scan, full-history scan, secret scan, Debug/Release, updater self-test, signed-package verifier and Verification
  Center.
- Record residual risks, intentionally public endpoints, retained historical evidence and any required key/credential rotation.
- Gate: workspace clean, `master == origin/master`, no unapproved sensitive GitHub content, and a documented public-repository readiness
  decision.

Audit 7 approved execution record (2026-07-22):

- Created a complete offline bundle of all pre-rewrite refs under the owner's private Documents archive, verified it with `git bundle
  verify`, and recorded its size and SHA-256 outside the repository workflow. This bundle is privacy-sensitive evidence: never publish,
  upload or copy it into the repository.
- Moved the retained v43.4.1, v43.5.0 and v43.5.1 signed archives containing the historical website snapshot into the same private
  archive boundary. Source removal and destination SHA-256 equality were verified; canonical v43.8.8 production artifacts remain in
  place.
- Removed the historical compiled-source material literals entirely. `GetDefaultNativeMaterialRows()` now has only the zero-row
  clean-profile implementation; the existing Verification Center gate continues to assert that count is zero.
- Removed the private data-bearing website snapshot, three unreferenced documentation SVGs and the obsolete direct-publish batch file.
  The governed PowerShell signed-package, verifier and deployment workflow remains canonical.
- Removed the obsolete packaging-script marker list that repeated private material identifiers. Added
  `Tools/Sanitize-CurrentTreeForCleanRoot.ps1`, whose sensitive FTPS identity is supplied only as an execution parameter, so future
  reproducible sanitation does not retain the value.
- Replaced private FTPS identity text, private material literals and selected historical identifiers in retained documentation with
  explicit redaction markers. Public product-domain references remain intentional; passwords continue to be Windows Credential
  Manager-owned and are not stored in Git.
- Aligned the primary README, release inventory and version identity with canonical v43.8.8. Older milestone/checklist content remains
  explicitly historical rather than being rewritten as current behavior.
- Debug and Release isolated builds both passed with 0 warnings and 0 errors. The updater self-test passed commit, injected rollback,
  failed-health rollback, Prepared restart, SnapshotReady/Installed/RollingBack/RollbackFailed recovery, read-only history, traversal
  rejection and SQLite-backup-reference preservation. The canonical signed v43.8.8 package passed the production verifier with six
  governed files and supported SQLite schema v29.
- The previously runtime-accepted v43.8.8 Verification Center result remains applicable: the only application-source cleanup removed
  text inside an already compiler-excluded block and preserved the existing zero-row verification assertion. No executable
  website/report/FTPS engine or SQLite behavior changed in Audit 7.
- Created one parentless root commit with non-personal noreply metadata, force-replaced remote `master` using an exact force-with-lease,
  deleted obsolete remote `main`, refreshed remote `HEAD` to `master`, and removed obsolete local references/reflogs after remote
  confirmation.
- A fresh clone of the rewritten GitHub repository exposed exactly one commit, only `origin/master`, no tags, no sensitive
  identity/material/personal-path/secret markers and no old audited commit object. The final amended root was re-pushed and rechecked
  against the same gates.
- Public-readiness decision: **YES** for the current reachable repository. Residual operational caution: GitHub may retain inaccessible
  caches for an unspecified period, and every pre-rewrite clone plus the private archive bundle still contains the old history. Never
  publish those copies; any collaborator must discard the old clone and re-clone. The public product domain is intentionally retained,
  while the private FTPS username and all credentials remain excluded.

## v43.8.9 SQLite Dependency Security

Status: complete and canonical; automated gates, local Verification Center, guarded clean-VM update, restored-data runtime and final VM
Verification Center passed.

- Keep the application on `net9.0-windows` and update `Microsoft.Data.Sqlite` only within the supported 9.0 servicing line.
- Explicitly select a non-affected SQLitePCLRaw native bundle so NuGet minimum-version resolution cannot retain 2.1.10 or 2.1.11.
- Do not change schema v29, SQLite ownership, backup/restore behavior, interrupted-update recovery, website/report/FTPS engines or
  automatic-restore policy.
- Gates: resolved dependency inventory contains no vulnerable package; Debug and Release build with zero warnings/errors; updater
  self-test and package verifier pass; clean-profile/static privacy checks pass; Verification Center and VM database/runtime acceptance
  are required before canonical release, commit and push.
- Local hygiene: remove only reproducible ignored `.vs`, `bin`, `obj` and noncanonical build-output directories after validation.
  Preserve `App/artifacts/v43_8_8-production`, all user SQLite/backups/configuration/evidence and the private pre-clean-root archive.

Automated evidence (2026-07-22):

- NuGet resolved `Microsoft.Data.Sqlite` 9.0.18 and the complete SQLitePCLRaw native/provider graph at 2.1.12. `dotnet list package
  --vulnerable --include-transitive` reports no known vulnerable package from the configured sources; the former high-severity 2.1.10
  native library is absent.
- Isolated Debug and Release builds passed with 0 warnings and 0 errors. The updater self-test passed commit, rollback, every
  interrupted-state recovery phase, traversal rejection and SQLite-backup-reference preservation.
- A pre-release v43.8.9 signed candidate passed the production application verifier with exactly six governed files and schema v29, then
  passed clean-VM update and restored-data runtime acceptance. The byte-identical tested artifact was promoted to
  `App/artifacts/v43_8_9-production`.
- Removed all enumerated reproducible ignored `.vs`, `bin`, `obj`, legacy application-local build artifacts and isolated test output. No
  locked target failed. Preserved canonical `v43_8_8-production` and `v43_8_9-production`; user data and the private Git archive were
  outside every cleanup target.
- Visual Studio subsequently reported NU1105 for `UpdateCore` because the application referenced it while the project was absent from
  the solution membership and the prior IDE cache had hidden that gap. Command-line restore/build proved both projects valid;
  `UpdateCore` was then added explicitly to the solution with Debug/Release configurations to make clean-cache Visual Studio restore
  deterministic.
- Local Release runtime acceptance passed on 2026-07-22: Verification Center reported PASS 296/296 for `v43.8.9
  SQLITE-DEPENDENCY-SECURITY`, assembly 43.8.9.0, informational identity aligned, schema v29 and the owner's 200-material SQLite
  database operating normally. The exported diagnostics contained 296 PASS lines and zero FAIL lines.
- Documentation path hygiene consolidated all 45 lowercase `docs/` files into the canonical `Docs/` tree without filename collisions or
  content deletion. All active path references now use exact GitHub case, and README has one canonical v43.8.9 identity instead of the
  stale v41.6 declaration.
- The first v43.8.8 VM update discovery failed before mutation because generated `latest.json` began with the Windows PowerShell UTF-8
  BOM (`EF BB BF`). The generator now writes BOM-less UTF-8 explicitly, and the v43.8.9 client defensively accepts one standard UTF-8
  BOM. This is a parser-compatibility extension only; package bytes, SHA-256, trusted manifest, schema and default-No apply gates remain
  mandatory.
- Final VM acceptance passed on 2026-07-22. A fresh zero-Materials v43.8.8 installation consumed the corrected feed, updated to v43.8.9
  without an error, recorded the transaction as `Committed` with zero incomplete transactions and preserved the zero-data boundary. A
  verified schema-v29 owner backup restored 200 Materials, 3,728 tensile samples, 3,752 impact samples and 191 stiffness rows; creating
  a canonical post-restore SQLite backup satisfied all recovery evidence gates. Verification Center then reported PASS 296/296 with zero
  FAIL lines. v43.8.9 is the canonical release.

## Planned — v44 Daily Use, Reliability & Maintainability

Purpose: strengthen the Windows desktop product that is actually used. v44 is
not an API, plugin, SaaS or cloud-platform programme. Work is selected from
observed daily use, VM/release evidence and concrete owner feedback. Every
increment must remain small, backwards compatible and independently runtime
accepted before the next increment begins.

Non-goals:

- No public application API without a real identified consumer and a separately approved authentication/hosting/support contract.
- No general plugin or arbitrary scripting host; the compatibility and security surface is disproportionate for the current product.
- No assumed manufacturer API integration. Filament manufacturers generally expose pages, datasheets, email and occasional files rather
  than a stable shared service contract; add only a proven source-specific import when a real source and workflow exist.
- No bidirectional cloud synchronization. SQLite conflict resolution, identity, offline state and multi-instance editing would create
  more risk than value; current local/OneDrive storage plus verified backup/restore remains canonical.
- No broad CSV/JSON platform for its own sake. Add a typed export/import only when a specific user workflow cannot be served safely by
  SQLite backup, governed Excel recovery or an existing report.

### v44.0 — Baseline and release-workflow closure

Status: **Complete — direct v43.8.9 installer/portable runtime accepted and promoted byte-identically through clean-tree Production
gates; stable-route publication remains a separate explicit default-No operational action.**

- Start from canonical v43.8.9, clean `master == origin/master`, Debug/Release 0/0 and Verification 296/296.
- Decision: adopt the direct v43.8.9 installer/portable first-install route built from the byte-identical canonical signed package. Both
  Candidate modes pass fresh-VM runtime acceptance. Production promotion must preserve the exact tested artifact bytes; stable routes
  remain on the accepted v43.8.8 plus guarded-update route until that promotion and publish are separately verified.
- Candidate versus Production state is explicit in packaging parameters, console output, update-feed metadata and deployment-plan
  metadata. Production packaging rejects dirty trees; every signed ZIP, feed, installer, portable ZIP and plan fails if its destination
  already exists.
- Deterministic `App/test_release_gates.ps1` covers NuGet vulnerability results, BOM-less update feed, exact feed/ZIP bytes and SHA-256,
  ECDSA signature, governed inventory, schema and stable-route-last publishing.
- Keep Authenticode deferred while distribution is private; retain the trusted ECDSA package signature and document the Windows Unknown
  Publisher trade-off.

### v44.1 — Verification profiles and diagnostic honesty

Status: **Complete — Application Readiness 207/207 plus 90 N/A and immediate post-restore Full Data Verification 297/297 runtime
accepted on v44.1.2.**

- Separate **Application Readiness** from **Full Data Verification** without weakening either gate.
- A legitimate fresh profile with zero Materials should report application/schema/assets/update/recovery readiness as PASS and
  data-dependent calculation/report checks as `Not applicable — no canonical data`, not as 90 product failures.
- Full release acceptance with restored/owner data remains the complete 296/296 gate (or its explicitly versioned successor).
- Show profile name, applicable/pass/fail/not-applicable counts and exact reasons in Verification exports.
- Keep clean-profile isolation, zero compiled seed rows, empty deployment identity and no automatic SQLite restore as mandatory checks
  in every profile.

### v44.2 — Daily-use UI state and selected MaterialID clarity

Status: **Complete — machine-local layout/MaterialID persistence, selected-row clarity and precise checkbox hit bounds
runtime accepted; Verification PASS 298/298.**

- Persist user-resized column widths, column order, window size and other proven daily layout preferences as machine-local UI state.
- Do not place machine-specific presentation state in the portable SQLite engineering backup unless a later explicit portability choice
  is approved.
- Keep the current Materials selection visually unmistakable after focus moves; Material Detail, Reports and other downstream tabs must
  show the same selected MaterialID.
- Evaluate tab/filter persistence one preference at a time. Never restore a stale selection that is absent from the current
  filtered/canonical dataset.
- Add reset-to-default and invalid-state fallback so old preferences cannot block startup or hide required fields.

### v44.3 — Backup, Recovery and update evidence clarity

Status: **Complete — v44.3.1 clean-VM runtime accepted with separate read-only update-evidence boundaries and
Verification PASS 209/209 plus 90 N/A.**

- Explain `Ready`, `Migration required`, `Legacy / incomplete`, `Newer / incompatible`, `Corrupt` and valid zero-data backup states
  directly in Recovery Center.
- Distinguish a healthy empty clean-profile backup from corruption and explain why full-data release gates require a canonical backup
  containing Materials.
- Surface the most recent update transaction, health acknowledgement, application rollback snapshot and SQLite evidence as separate
  read-only boundaries.
- Preserve all existing contracts: recovery snapshot before restore, atomic/transactional data replacement, no automatic SQLite restore,
  default-No application-file recovery and no automatic evidence deletion.

### v44.4 — Measured responsiveness and presentation polish

Status: **Complete — v44.4.1 viewport-only Fast Materials view is Visual
Studio and clean-VM runtime accepted with Full Data Verification PASS 300/300,
direct install, explicit SQLite restore and portable runtime.**

- Investigate the measured approximately 15-second first horizontal page jump
  in the wide Materials DataGrid. Arrow scrolling is responsive; large native
  thumb/track jumps into unrealized columns are slow. v44.2 A/B testing ruled
  out saved `DisplayIndex`; disabling column virtualization fixed scrolling but
  caused an unacceptable startup stall, and a custom ScrollViewer timer was
  rejected after an unresponsive halfway state.
- Use existing startup phase diagnostics to detect regressions; perform more lazy initialization or concurrency work only when a
  measured bottleneck materially affects first usable Materials time.
- Profile first-open and repeated-open latency for top-level Tools/Help menus before changing command construction or UI-thread work.
- Treat splash logo-line animation as optional presentation polish. It must follow measured splash lifetime, support high DPI and never
  delay startup.
- Record before/after Release timings on the same machine; reject optimization work that adds complexity without a meaningful observed
  gain.
- Accepted implementation draws only visible Materials rows/columns, preserves
  direct canonical SQLite auto-save and daily editing/filter/search behavior,
  and retains the native WPF DataGrid as a session fallback.
- The abandoned v44.4.0 artifacts remain non-canonical; v44.4.1 is the
  runtime-accepted canonical release.

### v44.5 — Legacy compatibility audit and bounded maintainability

Status: **First bounded increment complete and runtime accepted as v44.5.0 —
Retired Excel Import Surface. Second bounded increment complete and runtime
accepted as v44.5.1 — Active SQLite Compatibility Safety. Third bounded
increment complete and runtime accepted as v44.5.2 — Canonical SQLite UI
Boundaries. Fourth bounded increment complete and runtime accepted as v44.5.3
— Canonical Storage Terminology. Fifth bounded increment complete and runtime
accepted as v44.5.4 — Measurement Help Clarity. Sixth bounded increment
complete and runtime accepted as v44.5.5 — Retired Legacy Write Entry Points.
Seventh bounded increment complete and runtime accepted as v44.5.6 — Retired
Workbook Metadata Readers. Eighth bounded increment complete and runtime
accepted as v44.5.7 — Legacy Workbook Schema Retirement.**

Ninth bounded increment complete and runtime accepted as v44.5.8 — Retired
Transition UI Residue.

Tenth bounded increment complete and runtime accepted as v44.5.9 — Supported
Migration Naming.

- The unreachable original-Excel database import handler and its
  caller-exclusive importer services are removed. Lower-level SQLite legacy
  tables/readers remain for existing-data compatibility.
- Governed Excel disaster recovery and JSON empty-database migration snapshots
  remain supported and unchanged.
- Runtime Full Data Verification passed 301/301 with zero failures.
- v44.5.1 replaces the pre-backup active-database deletion path with read-only
  inspection, retained SHA-256-verified evidence and fail-closed startup.
  Supported migration paths remain unchanged; newer, malformed and unsupported
  active databases are never silently deleted, replaced or restored.
- Runtime Full Data Verification passed 302/302 with zero failures after the
  isolated SQLite fixture connections were made non-pooled.
- v44.5.2 removes misleading reload/clear cache UI and dead Excel-default reset
  ownership. With explicit owner approval, `MaterialsImport` and its active
  sync/automatic fallback are retired after a required verified SQLite backup.
  Backups, other legacy tables, JSON migration snapshots and governed recovery
  remain intact.
- Runtime Full Data Verification passed 303/303 with zero failures. v44.5.3
  corrects remaining user-visible `JSON transition`/general Excel-import
  terminology while retaining the supported JSON empty-database migration,
  governed Excel disaster recovery and explicit SQLite restore paths.
- Runtime Full Data Verification passed 304/304 with zero failures. v44.5.4
  removes literal duplicated instruction fragments from the three native
  measurement workspaces without changing calculation or storage behavior.
- Runtime Full Data Verification passed 305/305 with zero failures. v44.5.5
  retires the caller-free workbook, normalized-material and broad cache
  replacement entry points while preserving the still-read legacy tables,
  compatibility inspection and governed recovery paths.
- Runtime Full Data Verification passed 306/306 with zero failures. v44.5.6
  removes original-workbook sheet metadata from Material Detail, Tools and
  diagnostics. Legacy tables remain intact for the separate backup-first schema
  migration increment.
- Runtime Full Data Verification passed 308/308 with zero failures. v44.5.7
  advances SQLite to schema v30 and transactionally drops all 13 original
  workbook/normalized measurement tables after a retained verified backup.
  Canonical Impact/Stiffness values and scores remain visible, and schema v30
  backups are restore-ready.
- Runtime Full Data Verification passed 309/309 with zero failures. v44.5.8
  removes caller-free load/import-sync handlers, their
  caller-exclusive confirmation helpers and unused JSON save-state allocations.
  Supported empty-canonical JSON migration readers and all governed recovery
  paths remain intact.
- v44.5.9 renames internal canonical projection, supported JSON
  migration and built-in-default methods according to their current ownership.
  The first runtime run failed 306/310 after a new material exposed
  whole-revolution stiffness and close-time active-cell persistence gaps. The
  bounded correction treats a missing stiffness component as zero when its
  paired input exists and commits dirty/active measurement edits before close;
  schema and recovery behavior remain unchanged.
- Runtime Full Data Verification passed 310/310 with zero failures. MAT0206
  retained its active-cell edit across restart, whole-revolution stiffness
  reached Material Detail/Charts and report coverage parity returned to PASS.

- Inventory remaining original-Excel database, JSON/default/cache and pre-SQLite compatibility paths by caller and supported-state
  ownership.
- Preserve governed Excel disaster recovery and any migration path still required for a supported schema; remove only proven obsolete
  UI, dead handlers/services and stale documentation in small reviewed increments.
- Reduce `MainWindow.xaml.cs` and `LocalDatabase.cs` only while touching an owned feature: extract cohesive services/partial classes
  with no behavioral rewrite and one Verification/runtime gate per increment.
- Keep the project-wide `IOPath`/`IOFile`/`IODirectory` convention and compile probe; do not reintroduce bare ambiguous `Path.*` calls.

### v44.6 — Feedback-driven feature evaluation

Status: **First bounded clarity increment complete and runtime accepted as
v44.6.0 — Recovery Center Clarity. Second bounded maintainability increment
complete and runtime accepted as v44.6.1 — Canonical Release Documentation
Audit. Third bounded feedback increment complete and runtime accepted as
v44.6.2 — Canonical Measurement Date Foundation.**

- v44.6.0 removes verbose updater evidence from the always-visible Recovery
  Center surface and keeps one concise compatibility summary plus exact details
  for the selected backup. System Diagnostics, Verification, guarded restore,
  evidence retention and Default-No behavior remain unchanged.
- Runtime Full Data Verification passed 311/311 with zero failures; selected
  backup verification and retained diagnostics evidence were runtime accepted.
- v44.6.1 defines separate ownership for CHANGELOG, BUILD_HISTORY, RELEASES and
  MILESTONES, reconciles the accepted v44.5.2-v44.6.0 sequence and introduces a
  read-only baseline-aware release-documentation gate. It does not rewrite
  known historical collisions and does not change runtime/storage behavior.
- Runtime Full Data Verification passed 311/311 with zero failures; schema v30,
  six Ready backups and zero incomplete updater transactions were confirmed.
- v44.6.2 adds nullable canonical measured dates
  to native test sets and Experimental runs. It assigns today only on first
  real measurement input, preserves existing/manual dates, does not backfill
  historical data and advances SQLite additively to schema v31 while retaining
  schema-v30 canonical migration and governed recovery boundaries.
- Runtime Full Data Verification passed 312/312; compact Stiffness editing,
  manual date entry after column reordering and restart persistence were
  accepted.

- Use `Docs/BUG_FEEDBACK_LOG.md`, daily operation and VM/release observations as the intake queue.
- Evaluate per-MaterialID printing profiles only when there is a concrete daily-use/report need beyond the delivered base-material
  profile foundation. Preserve typed units, provenance and `Not recorded` honesty.
- Evaluate bilingual website presentation only when translation ownership and ongoing parity are affordable; reuse the one
  renderer/publisher and never fork a second website engine.
- Evaluate new report types only after their canonical source data exists. No inferred printer profiles, test sessions, durability
  history or manufacturer claims.

### v44 delivery discipline

1. Research and map one issue before code changes.
2. Record owner-visible purpose, non-goals, data/storage boundary and rollback plan.
3. Implement the smallest additive increment.
4. Run Debug/Release, relevant static/security/package gates and Verification Center.
5. Obtain visual/runtime acceptance where UI, restore, installer or updater behavior changes.
6. Commit/push only after the increment passes; do not accumulate a large v44 rewrite.

### Historical disposition archive — non-authoritative

The material below preserves earlier proposal decisions and detailed idea
contracts. It is historical context, not the current execution order. The
authoritative completed-state summary and estimated next-version sequence are
at the end of this document.

#### Disposition of previously open proposals

- **Proposal:** Public API
  - **Disposition:** Retired from v44; demand-only backburner
  - **Reason / trigger:** No identified consumer; would require authentication, hosting, versioning, monitoring and support.

- **Proposal:** Broad CSV/JSON exchange
  - **Disposition:** Demand-only
  - **Reason / trigger:** Existing SQLite/Excel/report paths cover current recovery and sharing; require a concrete typed workflow
    first.

- **Proposal:** Plugin and scripting architecture
  - **Disposition:** Retired from v44
  - **Reason / trigger:** Excessive execution, compatibility and support surface for the current desktop product.

- **Proposal:** External research/manufacturer API integrations
  - **Disposition:** Demand-only
  - **Reason / trigger:** Implement only against a real stable source and explicit rights/provenance; do not assume manufacturers
    provide APIs.

- **Proposal:** Cloud synchronization
  - **Disposition:** Retired from v44
  - **Reason / trigger:** Conflicts with single-writer SQLite and adds identity/conflict/offline complexity; verified local/OneDrive
    backup remains safer.

- **Proposal:** Experimental and Data Quality reports
  - **Disposition:** Backburner
  - **Reason / trigger:** Useful only when requested; canonical sources exist partly, but current six-report portfolio is complete.

- **Proposal:** Batch/history, printer-profile and durability reports
  - **Disposition:** Blocked by missing canonical data
  - **Reason / trigger:** Do not infer missing sessions, profiles or long-term history.

- **Proposal:** Per-MaterialID printing profiles
  - **Disposition:** v44.6 evaluation queue
  - **Reason / trigger:** Base-material profiles are delivered; add variants only for a proven workflow with typed provenance.

- **Proposal:** Persistent Materials selection
  - **Disposition:** v44.2
  - **Reason / trigger:** Concrete daily-use clarity improvement with low architectural scope.

- **Proposal:** Column widths/layout persistence
  - **Disposition:** v44.2
  - **Reason / trigger:** Concrete cross-machine observation; keep machine-local by default.

- **Proposal:** Remaining startup optimization
  - **Disposition:** v44.4 conditional
  - **Reason / trigger:** Existing v41.8 work succeeded; continue only from measured regressions/bottlenecks.

- **Proposal:** GUI menu responsiveness
  - **Disposition:** v44.4
  - **Reason / trigger:** Profile first; presentation-only change.

- **Proposal:** Splash logo animation
  - **Disposition:** Optional v44.4 polish
  - **Reason / trigger:** No functional value; never place ahead of reliability/usability.

- **Proposal:** Manufacturer submission server
  - **Disposition:** Retired until real operational need
  - **Reason / trigger:** Current browser/email handoff is sufficient; no public-to-SQLite write path.

- **Proposal:** Bilingual website
  - **Disposition:** v44.6 evaluation queue
  - **Reason / trigger:** Requires durable translation ownership and parity budget.

- **Proposal:** Website logo/branding
  - **Disposition:** Delivered
  - **Reason / trigger:** Canonical governed logo assets already render in application/reports; verify public placement only as part of
    a requested presentation change.

- **Proposal:** Legacy Excel/compatibility audit
  - **Disposition:** v44.5
  - **Reason / trigger:** Valuable security/maintenance cleanup, but requires caller-by-caller proof before removal.

- **Proposal:** Split large application/database files
  - **Disposition:** Incremental v44.5 only
  - **Reason / trigger:** No standalone rewrite; extract only cohesive touched ownership with regression proof.

- **Proposal:** Fresh-install Verification failures
  - **Disposition:** v44.1
  - **Reason / trigger:** Observed in VM: zero-data readiness and full-data coverage need honest separate profiles.

- **Proposal:** Recovery catalog wording
  - **Disposition:** v44.3
  - **Reason / trigger:** Observed in VM: valid empty backups were easy to mistake for failed update transactions.

- **Proposal:** Direct v43.8.9 installer/portable refresh
  - **Disposition:** v44.0 decision
  - **Reason / trigger:** Current v43.8.8 installer plus guarded update is accepted; rebuild only for clearer first-install experience.

All v44 work must preserve canonical MaterialID identity, SQLite ownership,
Verified Material Summary publication boundaries, signed/default-No update
contracts, privacy-clean distribution and backwards compatibility.

## Idea source archive and demand-only backburner

The detailed notes below preserve the original intent of ideas raised during
development. Their current disposition and estimated priority are in the
canonical delivery-status section at the end of this document; presence here
is not authorization to implement them.

### Future report extensions

Status: demand-only backburner; the accepted six-report portfolio is complete.

- Experimental Research Report for one canonical experimental series, including
  baseline, controlled runs, analytics, charts and conclusion.
- Verification & Data Quality Report for coverage, missing evidence, specimen
  counts, repeatability/CV review, orphan checks and release-gate transparency.

Treat Material Family Benchmarking as a Comparison Report preset. Do not create
batch/history, printer-profile or durability reports until the required
canonical session, profile or long-term test data exists.

### Canonical material printing profiles

Status: per-MaterialID variants moved to the v44.6 evaluation queue. The typed
base-material profile foundation is already delivered and remains canonical.

Schedule an additive SQLite/Material Detail milestone for typed per-MaterialID
printing settings: nozzle and bed temperature ranges, print-speed range,
cooling range, drying temperature/time, enclosure requirement, printer-profile
reference, slicer-profile reference/version, provenance/source URL and checked
date. The first increment should store one canonical manufacturer/baseline
profile per material while leaving a stable path for later printer/nozzle/profile
variants.

REPORT-150 and public printing recommendations may use these settings only after
the fields, units, provenance, validation and Verification round trips exist.
Missing settings must remain `Not recorded`; report code must never infer them.

### Materials selection visibility

Status: promoted to v44.2 as a concrete daily-use improvement.

Make the current Materials-grid selection persistent and visually unmistakable
after the grid loses focus. The selected row/MaterialID should remain clearly
highlighted until another material is selected, and downstream tabs such as
Material Detail and Reports should expose the same selected identity. This is a
focused desktop UX improvement and must not change canonical MaterialID or
filter behavior.

### Startup performance optimization (scheduled after v41.7)

Status: the v41.8 instrumentation/coalescing/warm-up work is complete. Further
work is conditional v44.4 profiling only; the old 19-second baseline is historical.

Baseline measured on the current development machine on 2026-07-21: the splash
screen appears after about 3 seconds and the Materials list becomes available
about 19 seconds after pressing Play in Visual Studio. Functionality is correct;
the detailed work contract is scheduled immediately after the Report Portfolio.

- Add phase timings for SQLite/native storage loading, manager initialization,
  WPF grid binding and deferred intelligence refresh.
- Identify duplicate reads and repeated MaterialID projection/summary work.
- Move nonessential tabs and analytics to safe lazy initialization after first paint.
- Preserve canonical data, filter parity and Verification results while optimizing.
- Compare cold and warm startup runs on the same machine; record a measurable
  target only after profiling identifies the actual bottleneck.

### GUI menu responsiveness

Status: promoted to measured v44.4 investigation.

Profile and improve the delay when opening top-level desktop menus such as
`Tools` and `Help`. Measure first-open and repeated-open latency separately,
identify whether menu construction, command state evaluation or UI-thread
background work owns the delay, and preserve existing commands and keyboard
behavior. This is a presentation-performance task only; it must not change
SQLite data, MaterialID identity, calculations or publishing behavior.

### Splash screen logo line animation

Status: optional v44.4 polish after reliability and daily-use work.

Refine the existing splash-screen logo animation so the complete blue logo line
is progressively revealed during the actual visible lifetime of the splash
screen. Replace the current short secondary blue-line movement with one smooth,
single-pass reveal that begins when the splash is rendered and reaches the end
of the full line immediately before the splash closes.

- Bind animation duration to the measured splash lifetime rather than a fixed
  decorative loop or assumed startup duration.
- Preserve the canonical logo geometry, colors, aspect ratio and current window
  transition into the main application.
- Handle unusually fast or slow startup honestly: complete gracefully on early
  close and avoid restarting, jumping backwards or leaving a duplicate line.
- Keep the animation presentation-only; it must not delay startup, block the UI
  thread or change initialization scheduling.
- Verify visually in both Debug and Release startup runs at normal display
  scaling and high-DPI scaling.

### Manufacturer submission server handoff

Status: retired until a real operational need exists; email/browser handoff is canonical.

The browser-only enquiry form and email handoff remain canonical for now.
A future server endpoint may add validation, spam protection, rate limiting,
delivery acknowledgement and governed intake states. It must never write
directly from the public website into the engineering SQLite database.

### Bilingual public website and branding

Status: localization is a v44.6 evaluation item. Core governed application/report
branding assets are delivered; public placement changes require a specific visual request.

Investigate a future additive website-localization milestone that keeps the
current English site and adds a complete Icelandic presentation with a clear
`EN / IS` language toggle near the top of the site. The investigation must
define governed translation ownership, fallback behavior, canonical URLs and
metadata, report-language scope, Preview/Production parity and Verification
coverage before implementation. It must reuse the canonical website renderer
and publishing pipeline rather than creating parallel site engines.

Any public logo placement change must reuse the approved governed assets with
responsive and accessible rendering. Logo source, variants and placement must
remain consistent across generated pages and dark/light surfaces.

### Legacy Excel and compatibility surface audit

Status: promoted to v44.5 as an isolated, evidence-first cleanup sequence.

Inventory every remaining UI command, startup/load path, service, handler,
documentation reference and Verification check that mentions or can read the
retired original Excel material database, Excel defaults, imported-material
sync, local cache or other pre-SQLite working-store behavior. Also identify
other legacy code paths encountered during the audit.

- Classify each finding as active governed compatibility/migration safety,
  current Excel disaster recovery/export, user-visible obsolete behavior or
  unreachable/dead implementation. The governed 29-sheet Excel disaster
  recovery workflow is current functionality and must not be mistaken for the
  retired legacy Excel database workflow.
- Trace callers and runtime data ownership before removal. SQLite remains the
  canonical source; no compatibility code may be deleted solely because its
  name contains `Excel`, `JSON`, `cache`, `legacy` or `default`.
- Remove obsolete UI, dead handlers/services and stale documentation in small
  reviewable increments. Preserve any seed/migration path still required for a
  supported database state until an explicit compatibility boundary retires it.
- Add Verification checks proving normal startup, canonical SQLite counts,
  backup/restore, governed Excel disaster recovery, reports and publishing are
  unchanged after cleanup.
- Execute it as separate small v44.5 increments with runtime acceptance before
  each commit/push; never combine it with an unrelated updater or schema change.

## Platform roadmaps

- [Materials](MATERIALS_ROADMAP.md)
- [Purchasing & Inventory](PURCHASING_ROADMAP.md)
- [Engineering](ENGINEERING_ROADMAP.md)
- [Publishing](PUBLISHING_ROADMAP.md)

## Roadmap governance

- One primary objective per build.
- Architecture evolves by extension, not rewrites.
- Completed release detail goes into history documents, not this roadmap.
- New roadmap items must identify their canonical data source and verification boundary.
- A major milestone is complete only when its required Verification Center gates pass.

---

## Canonical delivery status and next-version plan

Last reviewed: **2026-07-25**

This is the authoritative execution summary. Earlier proposal tables and idea
descriptions above are retained as context only. Estimated versions below are
planning slots, not implementation promises. Only the first `Current` item may
start without a new scope decision; every later item must still begin with
caller/data-ownership research and may be reordered when daily-use evidence
changes priority.

Major versions own one coherent strategic theme. Close a major version when
its bounded milestone is accepted, and advance the major number for the next
materially different theme instead of accumulating unrelated subversions.
Never renumber completed or runtime-accepted history; this rule applies only
to unstarted authoritative planning slots.

### Completed v44 delivery

- **v44.0 — Baseline and release-workflow closure**
  - Complete.
  - Governed installer/portable and Production gates accepted.
- **v44.1 — Verification profiles and diagnostic honesty**
  - Complete.
  - Clean-profile Application Readiness and restored-data Full Verification separated.
- **v44.2 — Daily-use UI state and MaterialID clarity**
  - Complete.
  - Machine-local geometry/column state and selected MaterialID accepted.
  - Full Data Verification 298/298 PASS.
- **v44.3 — Backup, Recovery and update-evidence clarity**
  - Complete.
  - Guarded recovery boundaries accepted.
  - Application Readiness 209/209 plus 90 N/A PASS.
- **v44.4 — Measured Materials responsiveness**
  - Complete.
  - Fast Materials view, native fallback and clean-VM paths accepted.
  - Full Data Verification 300/300 PASS.
- **v44.5.0–v44.5.9 — Legacy compatibility audit and bounded maintainability**
  - Complete.
  - Obsolete Excel/workbook/UI residue retired while governed recovery and supported migration remain.
  - Final Full Data Verification 310/310 PASS.
- **v44.6.0 — Recovery Center Clarity**
  - Complete.
  - Concise UI with exact selected-backup detail and retained diagnostics evidence.
  - Full Data Verification 311/311 PASS.
- **v44.6.1 — Canonical Release Documentation Audit**
  - Complete.
  - Governed document roles and baseline-aware audit.
  - Full Data Verification 311/311 PASS.
- **v44.6.2 — Canonical Measurement Date Foundation**
  - Complete.
  - Schema v31 dates, Material Detail visibility, restart/reorder editing and compact native grids accepted.
  - Full Data Verification 312/312 PASS.
- **v44.7.0 — Feedback Backlog Governance and Roadmap Reset**
  - Complete as a documentation-only roadmap increment.
  - All 49 retained feedback items have lifecycle metadata.
  - Owner review, 136-column formatting and canonical documentation audit passed.

### Completed v44 bounded sequence

- **v44.7.0 — Feedback Backlog Governance and Roadmap Reset**
  - State: **Complete**.
  - Feedback disposition: Bug/Feedback Log governance.
  - Give every retained feedback item a lifecycle state and resolution/evidence where known.
  - Maintain one counted summary and one authoritative future sequence.
  - Documentation-only; no runtime behavior change.
- **v44.7.1 — Category Rankings scope controls**
  - State: **Complete**.
  - Feedback disposition: Solved.
  - Current grouping and canonical visible MaterialID ownership researched.
  - Added the proven 5/10/50/100/All choice with runtime-accepted `All` performance.
  - Ranking formulas remained unchanged; Full Data Verification 313/313 passed.
- **v44.7.2 — Validation Help Clarity**
  - State: **Complete**.
  - Feedback disposition: Solved.
  - Row `Validation` ownership and dataset-level duplicate checks researched.
  - Added concise contextual UI help without changing validation calculations.
  - Owner runtime screenshot review and Full Data Verification passed.
- **v44.7.3 — Fast Workflow Grid — Tensile**
  - State: **Complete**.
  - Feedback disposition: Partially solved; remaining workflow grids are planned below.
  - Runtime testing showed repeated editable-DataGrid layout fixes were not reliable outside Materials.
  - Generalize the accepted viewport-only Materials renderer and migrate Tensile first.
  - Preserve canonical rows, formulas, validation, filters, auto-save and SQLite; retain a visible legacy fallback.
  - First runtime edit found a sort/selection reset; the accepted in-place refresh preserves both.
  - Owner accepted the full runtime checklist, reported a snappier view and Full Data Verification passed 315/315.
- **v44.7.4 — Fast Workflow Grid — Impact**
  - State: **Complete**.
  - Feedback disposition: Partially solved; Stiffness and Settings remain.
  - Reuse the accepted Fast Workflow Grid core with Impact-specific 0–100 validation, colors and calculations.
  - Preserve filters, measurement dates, SQLite auto-save, selection and a visible legacy fallback.
  - Candidate reuses accepted in-place refresh and separate keyed layout state; formulas remain unchanged.
  - First runtime review found negative input, repeated invalid warnings and reset-time row reordering.
  - Accepted correction rejects negatives, restores rejected cells once and resets layout without rebuilding rows.
  - Owner runtime retest and Full Data Verification 316/316 passed.
- **v44.7.5 — Fast Workflow Grid — Stiffness**
  - State: **Complete**.
  - Feedback disposition: Partially solved; Settings remains.
  - Reuse the accepted core with revolutions/degrees limits, computed outputs and unchanged formulas.
  - Preserve filters, measurement dates, SQLite auto-save, selection and a visible legacy fallback.
  - Candidate enforces 0–10 revolutions and 0–359 degrees at Fast and canonical row boundaries.
  - Rejected input, computed refresh and reset reuse the accepted in-place contracts.
  - First runtime view found narrow-content leading space and offset editors; coordinate alignment was corrected.
  - Owner runtime retest and Full Data Verification 317/317 passed.
- **v44.7.6 — Fast Workflow Grid — Settings**
  - State: **Complete**.
  - Feedback disposition: Solved.
  - Migrate Settings Value editing and Base Material Catalog entry only after mapping their distinct CRUD contracts.
  - Preserve FTPS validation, canonical SQLite settings and a visible legacy fallback during acceptance.
  - Candidate provides separate general/Base Material Fast views and keyed layouts.
  - Preserve Value-only general edits, Base Material ComboBoxes, immediate catalog save and add/delete selection.
  - First runtime open found a lazy-tab text-render argument crash; normalized DPI/geometry now awaits retest.
  - Follow-up blank rendering and misplaced controls are corrected by realized-tab activation and explicit toolbar ownership.
  - Cross-tab duplicate spacer identities now fall back safely and persist independently.
  - Materials filter propagation now refreshes all three Fast measurement snapshots from the shared visible MaterialID set.
  - Owner runtime acceptance and Full Data Verification passed.
- **v44.7.7 — Legacy Grid Retirement**
  - State: **Complete**.
  - Feedback disposition: Solved.
  - Remove accepted Fast-workflow legacy fallback controls first.
  - Keep legacy DataGrids hidden temporarily while they still supply Fast column and row adapters.
  - Stage 1 UI retirement is runtime accepted with Full Data Verification 319/319.
  - Stage 2 measurement contracts are runtime accepted with Full Data Verification 320/320.
  - Stage 3 Materials contract is runtime accepted with Full Data Verification 321/321.
  - First runtime review found reorder, stale Duplicate selection, white tab return and premature measurement sync.
  - Corrections preserve Fast row state/selection and require successful Materials save before measurement sync.
  - Follow-up found deferred Delete left 201 UI rows versus 203 SQLite rows; child-first immediate save is required.
  - Stage 4 Settings contracts are runtime accepted with Full Data Verification 322/322.
  - Stage 5 removes retired legacy DataGrid XAML, toggle handlers and grid-only edit paths.
  - Stage 5A toggle/handler retirement is runtime accepted with Full Data Verification 323/323.
  - Stage 5B removes collapsed measurement XAML and grid-only callers.
  - Stage 5B-Tensile deletion is runtime accepted with Full Data Verification 324/324.
  - Stage 5B-Impact deletion is runtime accepted with Full Data Verification 325/325.
  - Stage 5B-Stiffness deletion is runtime accepted with Full Data Verification 326/326.
  - All three legacy measurement grids and the obsolete deferred DataGrid warm-up are retired.
  - Stage 5C global reset retirement and local Materials reset ownership are accepted with Verification 326/326.
  - Stage 5D Settings fallback activation retirement is accepted with Full Data Verification 326/326.
  - Stage 5E Settings legacy-grid deletion is accepted with Full Data Verification 327/327.
  - Stage 5F Materials fallback activation retirement is accepted with Full Data Verification 327/327.
  - Stage 5G Materials canonical selection and CRUD ownership are accepted with Verification 328/328.
  - Stage 5H Materials filter/report/count ownership is accepted with Full Data Verification 329/329.
  - Stage 5I Materials edit/commit and recovery/save lifecycle retirement is accepted with Full Data Verification 330/330.
  - Stage 5J residual Materials caller retirement is accepted with Full Data Verification 331/331.
  - Stage 5K final Materials XAML deletion is accepted with Full Data Verification 332/332.
  - Replace those adapters with explicit Fast contracts before deleting legacy XAML, handlers and commit paths.
  - Preserve canonical SQLite, formulas, filters, validation, settings CRUD and layout behavior at every stage.
  - Require runtime acceptance after each removal stage.
- **v44.7.8 — Backup Filename Compatibility**
  - State: Complete.
  - Feedback disposition: Solved.
  - Give new automatic/manual/restore-evidence SQLite backups readable purpose-specific `.bak` presentation names.
  - Retain SQLite bytes, dual `.bak`/`.sqlite` discovery, explicit restore and updater/recovery evidence.
  - Never rename, move or add existing `.sqlite` backups to the new automatic cleanup set.
  - Keep the 20-file rotation bounded to new automatic `.bak` files.
  - Owner runtime acceptance and Full Data Verification 333/333 passed.
- **v44.7.9 — Public Measurement Date Provenance**
  - State: Complete.
  - Feedback disposition: Solved.
  - Add canonical ISO Tensile, Impact and Stiffness measured dates to Material Engineering and Test Session reports.
  - Preserve exact `Not recorded` fallback, per-material publication opt-in and explicit public allowlists.
  - Exclude internal edit timestamps; retain the separate raw-input and note approval boundary.
  - Clarify that template/scope control preview-export; public batch buttons each name their report family.
  - Preserve schema, formulas, report routes, other report families, website/FTPS and PDF-from-canonical-HTML behavior.
  - Owner HTML/PDF and responsive-GUI review passed; Full Data Verification passed 334/334.
- **v44.7.10 — Canonical MaterialID Default Row Order**
  - State: Complete and runtime accepted.
  - Feedback disposition: Solved.
  - Research view ownership and saved user-sort behavior across Materials, Tensile, Impact and Stiffness.
  - Default unsorted views to numeric MaterialID ascending: lowest first and newest/highest last.
  - Preserve explicit user sorting, filters, canonical row identity, SQLite order and legacy fallback behavior.
  - Reapply active header sorts after filters, reload, Add and Duplicate while preserving selection.
  - Restore startup selection, then reset the default viewport to the top-left after deferred filter refresh.
  - Commit active editors and save parent Materials before FK-child measurements during close/restart.
  - Keep Add/Duplicate placeholder display names unique and immediately save-safe.
  - Owner Add, Duplicate, sort, close/restart and viewport tests passed; Full Data Verification passed 335/335.
- **v44.7.11 — Settings Manager Command Clarity**
  - State: Complete and runtime accepted.
  - Feedback disposition: Solved.
  - Trace persisted SQLite ownership and callers for `Load Settings` and `Restore Built-in Defaults`.
  - Make reload versus default-No replacement behavior explicit; correct overlap only if runtime evidence proves a defect.
  - Preserve Deployment Settings and Base Material Catalog ownership, cancellation and restart behavior.
  - Rename `Reset Fast Columns` to `Reset Columns` without changing the accepted layout-reset scope.
  - Correct built-in restore so it cannot replace the in-memory Base Material Catalog.
  - Make reload default-No because it discards current unsaved Settings edits.
  - Hide duplicate generic in-memory grid reload footers; retain the explicit SQLite reload owner.
  - Owner reload, restore, cancellation, restart, column-layout and visual tests passed; Full Data Verification passed 336/336.
- **v44.7.12 — Clean Baseline Retirement**
  - State: **Complete and runtime accepted**.
  - Feedback disposition: Maintainability rule baseline.
  - Remove only caller-free code after C#, XAML, project, reflection, serialization, migration, recovery and Verification tracing.
  - Retire the unused hand-built PDF layer; canonical HTML plus WebView2 and the active typed report renderer remain owners.
  - Retire caller-free legacy workbook write helpers while preserving schema migration, Excel disaster recovery and SQLite restore.
  - Remove unowned UI/template residue and obsolete repository assets; retain all runtime, installer and report branding assets.
  - Preserve formulas, measurements, website/report/FTPS behavior, updater evidence and canonical SQLite ownership.
  - Public Report Package fingerprint now reads canonical native measurement tables, not retired workbook tables.
  - Debug/Release, static/security/package gates, owner report review and Full Data Verification passed.
- **v44.7.13 — Public HTML Trust Hardening**
  - State: **Complete and runtime accepted**.
  - Make the imported website-template executable-content trust boundary explicit and default-No.
  - Limit new imports to 5 MiB and require one structurally replaceable `const DATA` object.
  - Encode malicious public text, reject unsafe link schemes and verify those boundaries directly.
  - Block unexpected WebView2 navigation, popups and permissions; retain scripts and local assets only for canonical PDF rendering.
  - Preserve accepted website templates, canonical report layout/PDF output, Preview/Production parity and FTPS behavior.
  - Defer CSP and broad sanitization until compatibility evidence proves they do not break accepted output.
  - Owner template/Preview/report HTML/PDF review and Full Data Verification 339/339 passed.

### Completed v44 automation closure

- **v44.7.14 — Automated Runtime Acceptance Foundation**
  - State: **Complete; runtime accepted.**
  - Feedback disposition: Approved owner-value foundation before post-v44 feature work.
  - Research before code:
  - Research UI Automation visibility, startup/profile ownership, Verification exports and current evidence surfaces.
  - Confirm which controls already expose `AutomationId` and whether virtualized Fast controls are visible to UI Automation.
  - Trace startup database/settings path selection and determine whether a clean disposable-profile contract already exists.
  - Map owned dialogs, success/error status, machine-readable Verification export and PDF/report completion signals.
  - Classify current runtime checks by whether they are genuinely safe in read-only mode.
  - Safety policy:
  - Block Production and FTPS by default in every automated scenario.
  - Permit restore, delete and update only in a proven disposable profile and only with explicit per-scenario authorization.
  - Stop the run on any unexpected dialog; do not infer a response or continue past an unknown state.
  - Never select or mutate the owner database automatically.
  - Validate the resolved database path before every write or restore and retain backup plus before/after hashes.
  - Exclude credentials and secret-bearing controls from screenshots, logs and machine-readable evidence.
  - Confine synthetic mouse/keyboard input to the owned application process and window.
  - Stage 1 — Read-only smoke runner:
  - Add a read-only smoke runner that launches an exact build, verifies release identity and navigates stable UI contracts.
  - Add stable WPF `AutomationId` values only where required; never depend primarily on screen coordinates or display text.
  - Add an isolated disposable runtime profile with visible identity and no canonical owner-database ownership.
  - Define data-driven scenarios for navigation, input, waits, assertions, files, screenshots and controlled shutdown.
  - Record machine-readable JSON/TXT results, step timing, logs, screenshots, artifact hashes and Verification exports.
  - Launch the application, inspect tabs/controls, run Full Data Verification and create evidence without changing data.
  - Stage 2 — Report acceptance:
  - Automate local report HTML/PDF content and artifact checks; retain explicit human visual acceptance.
  - Verify routes, allowlisted text, HTML/PDF existence and hashes; capture screenshots for manual visual review.
  - Stage 3 — Disposable CRUD:
  - Add disposable CRUD/save/restart tests only after read-only smoke and profile isolation are accepted.
  - Create, edit, persist and remove only an explicitly identified disposable test record/profile.
  - Stage 4 — Backup and recovery:
  - Add backup, Recovery Center and Excel-restore scenarios only against disposable data with retained evidence.
  - Verify manual SQLite backup, discovery, inspection and pre/post-restore evidence; never silently restore SQLite.
  - Stage 5 — Guarded updater:
  - Add guarded updater scenarios only in disposable portable environments after recovery boundaries are proven.
  - Verify transaction, snapshot and rollback evidence; never target the installed owner application or canonical database.
  - Keep owner SQLite, Production, FTPS, destructive restore/delete and unexpected dialogs blocked by default.
  - Distinguish `PASS`, `FAIL`, `BLOCKED`, `SKIPPED BY SAFETY POLICY` and `MANUAL REVIEW REQUIRED`.
  - Minimum first delivery:
  - Add one runner project, 10–20 stable `AutomationId` values and three to five read-only smoke/report scenarios.
  - Produce screenshots, JSON/TXT results and a Full Data Verification export with hard Production/FTPS blocking.
  - Do not activate later destructive stages until isolation, path validation and scenario policy pass runtime acceptance.
  - Stage 1 candidate now provides the isolated profile, exact-build runner, stable navigation IDs and Verification evidence.
  - Disposable acceptance passes 340/340 with consistent snapshots and identical before/after logical SQLite hashes.
  - Owner runtime acceptance and Full Data Verification 340/340 pass.
  - Fast-cell, report-generation, CRUD, recovery and updater automation remain outside the completed Stage 1 scope.
  - Complete Stages 2–5 in v44.7.15–v44.7.18 before starting post-v44 feature work.
  - Automation supports but never replaces owner runtime or visual acceptance.
- **v44.7.15 — Automated Report Acceptance**
  - State: **Completed and runtime accepted 2026-07-25.**
  - Feedback disposition: Approved Stage 2 completion before post-v44 feature work.
  - Map report controls, completion signals, routes, allowlisted content and current HTML/PDF verification before code changes.
  - Build local previews only; keep Production and FTPS blocked.
  - Verify expected routes, HTML/PDF existence, non-empty artifacts, hashes and governed text without exposing private data.
  - Retain screenshots and machine-readable evidence; require explicit owner visual review for representative HTML and PDF output.
  - Preserve every accepted report, website, formula, measurement and publication-approval boundary.
  - Candidate uses the canonical aggregate package action, explicit scenario authorization and disposable output containment.
  - Disposable runtime passes Verification 341/341 with 211 catalog entries, 639 verified artifacts and matching logical DB hashes.
  - Rendered PDF review corrected a Material Summary continuation-table clip.
  - Owner-found HTML clipping/over-compression is corrected with readable columns and narrow-window scrolling.
  - Owner accepted the landscape PDF, responsive HTML behavior and Full Data Verification 341/341.
  - Debug/Release, static/security, documentation, roadmap-line and NuGet vulnerability gates pass.
- **v44.7.16 — Disposable CRUD Acceptance**
  - State: **Completed and runtime accepted 2026-07-25.**
  - Feedback disposition: Approved Stage 3 completion before post-v44 feature work.
  - Add scenario-authorized create, edit, save, restart, persistence and delete tests only in a proven disposable profile.
  - Use uniquely identified automation records and prove canonical owner paths are never selected.
  - Retain before/after snapshots, logical hashes and per-action evidence; unexpected dialogs stop the run.
  - Do not automate Fast-grid cells until their stable automation contract is researched and accepted.
  - Candidate uses one exact manifest-authorized disposable MaterialID and the canonical Materials collection-save path.
  - Create/save, restart/edit/save, restart/delete/save and final restart/absence checks pass in disposable runtime.
  - Per-action snapshots retain full logical hashes; the final business-state hash normalizes only `UpdatedAtUtc`.
  - Disposable runtime and Full Data Verification pass 342/342 with equal before/after business-state hashes.
  - Production, FTPS, updates, restore, general deletion and owner database selection remain blocked.
  - Owner accepted normal create/edit/delete persistence and Full Data Verification 342/342.
- **v44.7.17 — Disposable Backup and Recovery Acceptance**
  - State: **Completed and runtime accepted 2026-07-25.**
  - Feedback disposition: Approved Stage 4 completion before post-v44 feature work.
  - Exercise manual SQLite backup, Recovery Center discovery/verify and governed Excel restore only against disposable data.
  - Require explicit scenario authorization, validated paths and retained pre/post-restore evidence and hashes.
  - Prove that SQLite is never silently restored and that supported historical `.sqlite` and `.bak` discovery remains intact.
  - Preserve updater/recovery evidence, diagnostics and explicit restore behavior.
  - Candidate adds explicit `recovery` authorization without releasing general SQLite/Excel restore UI locks.
  - Manual `.bak` and legacy `.sqlite` discovery/verification pass inside the disposable database folder.
  - Governed Excel export, mutation, restore, pre/post SQLite evidence and same-manifest restart pass.
  - SQLite restore remains explicit and is not automated; owner, Production, FTPS and updater paths remain blocked.
  - Final timestamp-normalized business-state hash equals baseline.
  - Disposable Full Data Verification passes 343/343; workbook and recovery artifacts retain bytes and SHA-256 evidence.
  - Owner accepted backup discovery, Excel recovery, pre/post evidence and Full Data Verification 343/343.
- **v44.7.18 — Guarded Updater Acceptance**
  - State: **Completed and runtime accepted 2026-07-25.**
  - Feedback disposition: Approved Stage 5 completion before post-v44 feature work.
  - Test only an isolated disposable portable build; never target the installed owner app or canonical database.
  - Verify transaction states, snapshot, exact-build health acknowledgement, rollback and retained evidence.
  - Keep Production, FTPS and owner paths blocked; every updater mutation requires explicit scenario authorization.
  - Require runtime review for success, failure and rollback boundaries before closing the automation program.
  - Candidate forwards the exact disposable profile through helper health and rollback relaunches.
  - Real helper commit covers 54 governed portable files and exact v44.7.18 health acknowledgement.
  - Forced failed launch reaches `RolledBack`; all pre-update SHA-256 values and database business state are restored.
  - Disposable and owner Full Data Verification pass 344/344; owner accepted normal startup and owner-data behavior.
### Authoritative future major-version sequence

- **v45.0 — Material Model Audit and Relationship Plan**
  - State: **Completed research 2026-07-25.**
  - Feedback disposition: Research complete; implementation moved to v45.1.
  - Trace `Manufacturer SKU`, video-thumbnail-name and related Material field callers before hide/retire decisions.
  - Map Materials, Manufacturers and Base Material catalog identity, rename, archive, delete and compatibility ownership.
  - Decide bounded v45.1/v45.2 implementation contracts before schema or UI changes.
  - Preserve recovery, import/export, reports, website and historical/unmapped values.
  - Audit found Manufacturer/Base Material are independent text identities with no catalog foreign keys.
  - `Manufacturer SKU` retains purchasing, detail, export and recovery callers; it is not caller-free.
  - Thumbnail filename has compatibility/export ownership but no active website/report asset reader.
  - Decision: use backwards-compatible name selection first; do not add or silently populate `ManufacturerID`.
  - Owner review superseded name-only binding for v45.1 because canonical rename must propagate through every consumer.
- **v45.1 — Canonical Manufacturer Selection**
  - State: **Complete — canonical and runtime accepted.**
  - Feedback disposition: Solved.
  - Replace free-text Manufacturer editing only after v45.0 approves the ownership contract.
  - Compare backwards-compatible name binding with a real `ManufacturerID` foreign-key relationship.
  - Source the dropdown from canonical Manufacturers and preserve supported legacy/unmapped values.
  - Define rename, archive, delete, Add Material, import/export, report, website and recovery behavior.
  - Update disposable CRUD automation for select/save/restart persistence.
  - Schema v32 adds nullable `ManufacturerId`; migration leaves every existing Material unlinked.
  - Active catalog names plus exact current legacy/unmapped values remain available in Fast Materials.
  - Explicit selection stores ID and text snapshot; linked rename propagates to all existing text projections.
  - Exact-name legacy binding requires a visible preview and default-No confirmation; ambiguous/unmatched values remain unlinked.
  - Referenced hard delete is blocked; archive and unlinked legacy fallback preserve data.
  - Public allowlists stay unchanged; typed Excel recovery includes the nullable ID column.
  - Disposable CRUD covers legacy fallback, ID persistence, rename propagation, delete guard and lossless cleanup.
  - Owner review found stale Manufacturer and Inventory projections after Material delete; the candidate refreshes both.
  - Initial naming of a new Manufacturer remains editable even when a legacy Material uses the default placeholder text.
  - Owner exact binding reached zero unlinked Materials; recovery controls hide at zero and reappear for supported legacy data.
  - Owner runtime accepted the final workflow and Full Data Verification passed 345/345.
- **v45.2 — Canonical Base Material Selection**
  - Feedback disposition: Solved.
  - v45.2.0 first moves the governed catalog out of Settings into a dedicated Base Materials workspace.
  - v45.2.0 is complete, canonical and runtime accepted.
  - The workspace owns Add, Duplicate, guarded Delete, direct editing and an independent Fast-grid layout.
  - Settings Manager retains only measurement, calculation, deployment and exchange-rate settings.
  - Disposable CRUD must navigate the new tab and prove Base Material add/edit/duplicate/delete persistence and cleanup.
  - Owner accepted immediate startup rendering, full CRUD and Full Data Verification 346/346.
  - v45.2.1 adds canonical `BaseMaterialId`, exact-name binding, rename/delete guards and downstream resolution.
  - Use a backwards-compatible dropdown sourced from the governed catalog.
  - Preserve explicit legacy/unmapped values and never silently remap.
  - Existing schema-v32 Materials migrate with null IDs; only explicit selection or confirmed exact binding creates links.
  - Tester owns linked rename, referenced-delete blocking, restart persistence and complete disposable cleanup.
  - Owner accepted binding, live dropdown refresh, rename/delete guards and Full Data Verification 347/347.
  - v45.2 is complete, canonical and runtime accepted.
- **v46.0 — Application Branding Review**
  - Feedback disposition: Solved.
  - Evaluate a transparent-background icon against Windows visibility, installer, About, reports and governed assets.
  - Approved original assets own separate application-icon and Labs-wordmark roles.
  - v46.0.0 keeps the application icon in Windows/splash and uses the Labs wordmark on a light main-header card.
  - Splash masks the baked filament locally and draws one full vector trace across its guaranteed visible interval.
  - Trace timing starts after blocking MainWindow construction; geometry follows measured source-icon bounds.
  - Public HTML/PDF retains its accepted JPG branding contract; plain MessageBox About intentionally remains unbranded.
  - Deterministic tester expansion is not warranted for visual branding; embedded-resource Verification is extended instead.
  - Owner accepted executable/titlebar, splash and main-header visuals, including the smooth complete splash trace.
  - Disposable Full Data Verification passes 347/347 with exact logical and business-state equality.
  - v46.0 is complete, canonical and runtime accepted.
- **v47.0 — AI Assistant Workflow Clarity**
  - Feedback disposition: Partially solved.
  - v47.0.1 labels the assistant as local rule-based, exposes visible MaterialID scope and separates workflow roles.
  - v47.0.1 preserves existing session, collection and coverage JSON without migration.
  - Owner accepted purpose, layout, filter-aware scope, local brief behavior and Full Data Verification 348/348.
  - v47.0.1 is complete, canonical and runtime accepted.
  - Current increment: v47.0.2 — Collection Workflow Clarity.
  - Add preview before saving visible rows and distinguish collection creation from explicit update.
  - Require confirmation before replacing an existing same-name collection.
  - Preserve all existing collection JSON and MaterialID compatibility.
  - Candidate implements bounded MaterialID preview, explicit action state and default-No create/update confirmation.
  - Owner accepted create/update, preview, default-No cancel honesty and Full Data Verification 349/349.
  - v47.0.2 is complete, canonical and runtime accepted.
  - Current increment: v47.0.3 — Stable Coverage Identity.
  - Research and migrate coverage ownership from collection-title/material-label to stable collection ID and MaterialID.
  - Preserve supported legacy coverage entries; bind only exact identities and never perform fuzzy or silent remapping.
  - Keep the current deterministic assistant local-first and explain its product purpose, inputs and outputs.
  - Prove visible/filtered MaterialID scope and simplify collection creation, inspection, reuse and removal.
  - Show whether each result is a local brief and make canonical-source, evidence and non-mutation boundaries explicit.
  - Research the minimum allowlisted data contract that a future external assistant may consume.
  - Record privacy, credential, retention, failure, cost and human-review requirements for the future integration.
  - Assess deterministic tester, AutomationId, scenario, evidence and Full Data Verification ownership.
  - Completion condition: the local workflow is owner-accepted and the future external boundary is documented.
  - This increment does **not** call an external AI API or store an external API credential.
- **v48.0 — Pricing and Usage Analytics Foundation**
  - Feedback disposition: Partially solved.
  - Define governed USD/kg provenance and one honest price/performance metric.
  - Define ownership for print hours, test hours, material usage and sample-count history.
  - Missing cost and history remain `Not recorded`; do not add schema before a proven capture/report workflow.
- **v48.1 — MaterialID-aware Print Job Pricing and Immutable Quote Snapshots**
  - State: Open design.
  - Requires formula/reuse-rights, unit, currency-provenance and Printer Profile approval before coding.
  - Historical quote snapshots must be immutable.
- **v48.2 — Optional Official Exchange-rate Reference Catalog**
  - State: Deferred/conditional.
  - Requires a stable official endpoint and reuse contract.
  - May prefill only new unsaved purchase data and must never rewrite saved purchases, lots or quotes.
- **v49.0 — Experimental Workflow Extension**
  - Feedback disposition: Partially solved.
  - Evaluate owner-visible gaps on top of canonical Series/Runs.
  - Do not duplicate Materials or create dynamic schema columns.
- **v50.0 — Comprehensive User Help and Workflow Guide**
  - State: Planned dedicated milestone.
  - Feedback disposition: Open.
  - Treat this as a full information-architecture and user-documentation project, not incidental tooltip work.
  - Inventory every supported tab, command, prerequisite, validation state and cross-tab handoff before writing guidance.
  - Document the complete workflow from Purchasing, Inventory and Materials through measurement entry and engineering review.
  - Continue through reports, website Preview, public-selection boundaries and guarded Production/FTPS publishing.
  - Provide a start-to-finish guide, per-tab reference, contextual in-app entry points and troubleshooting/recovery boundaries.
  - Preserve canonical calculations, SQLite ownership, public allowlists, default-No publishing and guarded recovery behavior.
  - Entry condition: approved help structure and verified mapping to current runtime behavior.
- **v51.0 — Governed Development/Verification and Production/Clean Profiles**
  - State: Research only until diagnostics have measured cost.
  - Mandatory crash, recovery, security and support evidence can never be removed.
- **v52.0 — Optional OpenAI Assistant Integration Contract**
  - State: Planned; implementation cannot begin before v47 workflow and data-boundary acceptance.
  - Keep the accepted local deterministic assistant available as the offline and failure-safe path.
  - Add an explicit optional OpenAI provider behind a replaceable provider interface; do not couple canonical data to one API.
  - Store provider, pinned model and non-secret preferences separately from the API credential.
  - Store the API credential only in Windows Credential Manager; never persist it in SQLite, backups, exports or logs.
  - Show an allowlisted payload preview and require an explicit user action before any selected data leaves the computer.
  - Default to selected or visible MaterialIDs and exclude credentials, local paths and unrelated private/business data.
  - Use read-only structured responses with validated evidence MaterialIDs, bounded timeouts, cancellation and safe refusal handling.
  - Never let model output silently mutate canonical engineering, material, measurement, report or website data.
  - Record provider, pinned model, prompt/schema version, input MaterialIDs and timestamp without recording the credential.
  - Evaluate data retention, API billing, usage limits, support ownership and model-change policy before runtime activation.
  - Standard automation uses a deterministic fake provider and no network or real credential.
  - Live API acceptance is manual, opt-in and limited to an approved synthetic or disposable payload.
  - Completion condition: owner accepts security and payload preview, local fallback, structured evidence and Verification PASS.
- **v52.1 — Credential and Provider Foundation**
  - Add masked Save/Replace/Delete/Test controls in Settings while Windows Credential Manager remains the secret owner.
  - Add the provider abstraction, pinned configuration, connection diagnostics and safe local fallback.
  - Prove that credentials cannot enter SQLite, import/export, backup/recovery, diagnostics or retained tester evidence.
- **v52.2 — Read-only OpenAI Responses Pilot**
  - Add explicit `Generate with OpenAI`, exact outbound preview, cancellation and bounded error handling.
  - Use structured output for summary, findings, evidence MaterialIDs, uncertainties and suggested next actions.
  - Reject unknown evidence IDs and retain all generated content as advisory until the user explicitly saves an assistant session.
- **v52.3 — Governed Acceptance and Operational Evidence**
  - Evaluate quality, latency and cost with approved representative scenarios before pinning the accepted model.
  - Extend deterministic automation for payload allowlists, response validation, fallback, cancellation and secret non-persistence.
  - Require Debug/Release, security/static gates, Full Data Verification and owner runtime acceptance before closure.

### Intentionally unscheduled

- The unresolved one-day Variant-edit crash remains an immediate defect
  investigation, not a feature version. If reproduced with diagnostics it takes
  priority as the next bounded patch; it is not marked solved by similarity to
  another crash fix.
- External providers other than the planned optional OpenAI integration remain demand-only.
- Optional destructive clean uninstall remains deferred. Normal uninstall stays
  data-preserving; no evidence, backups, SQLite or credentials may be deleted
  automatically.
- Bilingual website, manufacturer/API integrations, cloud sync, plugins,
  general scripting and broad exchange platforms remain demand-only or retired
  under the dispositions above.

### How the next item is selected

1. Finish and accept the `Current` item.
2. Re-read `Docs/BUG_FEEDBACK_LOG.md`; a new reproducible bug may supersede the
   planned feature order.
3. Promote exactly one `Planned` item to `Current` and record purpose,
   non-goals, canonical source, compatibility boundary and rollback plan.
4. Implement one small increment; do not bundle adjacent future versions.
5. Require Debug/Release, applicable static/security/package gates, Verification
   and runtime/visual acceptance before marking it complete or advancing the
   canonical release.
