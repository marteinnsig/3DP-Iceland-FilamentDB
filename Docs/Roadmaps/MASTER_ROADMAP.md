# 3DPIceland Engineering Platform — Master Roadmap

Current canonical release: **v43.8.8 — Remote Signed Update Delivery Production Consolidation**

Last runtime-accepted baseline: **v43.8.8 — Remote Signed Update Delivery Production Consolidation**

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
| v43 | Deployment Platform | ★★★★☆ | In progress — installer/portable deployment runtime accepted |
| v44 | Open Engineering Platform | ★★★☆☆ | Future |

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
- Internally calibrated consistency-score interpretation and documented purpose-built equipment limitations across all downstream surfaces.

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

### In progress - v41.8 Startup Performance & Safe Concurrency

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

## Foundation delivered early — v42 Website & Approved Template Architecture

Already delivered:

- Approved templates stored and versioned in SQLite.
- Canonical single-file portal navigation.
- Preview and Production use the same renderer.
- Database, Pricing, Experimental, Manufacturers and Methodology website surfaces.

Remaining work belongs to the Publishing roadmap and should be driven by
specific user-facing needs rather than a template rewrite.

### v42 candidate - Public Report Publishing

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

## Planned — v43 Deployment Platform

Delivered early:

- Guarded production website publishing over explicit FTPS.
- Trusted TLS certificate validation, passive transfers, timestamped remote
  backups, staged replacement and rollback protection.

Remaining:

- Installer and portable deployment modes.
- Safe updater and rollback workflow.
- Production deployment diagnostics.
- Further deployment diagnostics and operational hardening where real usage
  identifies a concrete need.

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
- Show schema, integrity and canonical row counts with Ready, Migration required, Legacy/incomplete, newer/incompatible or corrupt status.
- Treat schema v27 as the minimum standalone SQLite recovery boundary; older backups require external transition evidence.
- Verify schema v27-v28 migrations on self-deleting temporary copies and require integrity plus canonical count preservation.
- Rerun compatibility verification before guarded restore and preserve the established recovery snapshot, atomic replacement and restart contracts.
- Keep remote website Production restore and publishing unchanged.

### v43.3.1 — Backup and Recovery Center UI

- Status: complete and runtime accepted on 2026-07-22.
- Replace scattered File commands with one Backup and Recovery Center entry.
- Place manual SQLite backup, SQLite verify/restore, governed Excel backup/restore and Open Storage Folder in the center; omit the retired legacy Excel database import.
- Remove manual-backup and duplicate database/backup-folder buttons from System Diagnostics.
- Retain Choose Storage Folder as configuration and keep diagnostics verification/reporting controls unchanged.

### v43.4 — Safe Application Update and Rollback

Status: first read-only signed-package verification increment runtime accepted on 2026-07-22; application-file staging/apply/rollback remains scheduled before installer work.

- Keep application binaries, SQLite user data and published website state as three separate update/recovery boundaries. v43.4 changes only the application-binary boundary and reuses the accepted SQLite backup/migration checks; FTPS publishing and remote rollback remain unchanged.
- Define one versioned release manifest containing release identity, minimum/current supported database schema, complete governed file inventory, byte lengths and SHA-256 hashes. Integrity hashes alone are not publisher authentication: production auto-update must additionally require Authenticode or a detached signature rooted in an embedded trusted public key.
- Download or select an update into a unique staging folder outside the live application directory. Reject downgrade, same-version, newer-unsupported-schema, missing/extra governed files, path traversal, hash mismatch, invalid signature and incomplete package states before shutdown.
- Create and verify a manual SQLite recovery backup before any update that may start a newer schema. Never package, copy or overwrite the active database, backup history, Credential Manager secrets or configured storage-folder pointer as application files.
- Use a small external updater/helper because the running single-file EXE cannot safely replace itself. The helper must preserve the current app package as a versioned rollback snapshot, install by same-volume atomic renames where possible, record a durable transaction manifest and restart the staged version.
- Require a startup health acknowledgement from the new version. If process launch, startup/schema compatibility or the bounded post-update health contract fails or times out, the helper restores the prior app package and relaunches it; the pre-update SQLite recovery backup remains available for guarded data rollback rather than being restored silently.
- Preserve the last known-good application package independently of the rotating SQLite backup set. Cleanup must never remove the active version, an incomplete transaction or the only known-good rollback package.
- First implementation increment should provide manifest/package verification and update-readiness diagnostics without applying untrusted packages. Apply/update automation follows only after the signing trust root and helper transaction tests are runtime approved.
- Add Verification Center gates for manifest parsing, path containment, hash/signature policy, version/schema decisions, update-state recovery and strict separation from SQLite/website publishing.
- Installer and portable modes must later consume this same manifest, package layout and rollback contract rather than inventing a second deployment path.

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

### v43.4.1 — Release Signing and Package Creation

Status: complete and runtime accepted on 2026-07-22. Production CNG trust root, canonical signed package command and authoritative application-verifier probe passed. Private-key rotation/recovery remains required before unattended updater deployment.

- Generate a dedicated ECDSA P-256 production release keypair. Embed only the public key in the application; keep the private key outside the repository, application package, SQLite storage/backup folder and normal build output.
- Add an explicit release-packaging command that consumes the private key, produces the existing versioned manifest, hashes the complete governed Release file inventory and writes the detached manifest signature into the update ZIP.
- Refuse dirty, mismatched-version, incomplete or unexpected build output. Include licensing/notices and required runtime assets while excluding SQLite data, backups, credentials, storage configuration, reports, publish staging and developer files.
- Prove that the production application accepts a correctly signed newer package and blocks a modified payload, modified manifest, wrong key, missing signature, downgrade, same version, path traversal and unsupported SQLite schema.
- Keep package inspection read-only; no live application-file replacement is authorized in v43.4.1.

### v43.5 — Transactional Application Update and Rollback

v43.5.0 status: complete and runtime accepted on 2026-07-22. Isolated transaction engine, external helper, signed ten-file package and commit/rollback Verification passed. Live application mutation remains intentionally disabled. v43.5.1 will add process wait/launch, startup health acknowledgement and guarded Apply.

v43.5.1 status: complete and runtime accepted on 2026-07-22. A signed 11-file v43.5.1 portable base applied the signed v43.5.2 candidate, created a verified SQLite backup, committed durable transaction state, restarted and acknowledged exact version/schema health; Verification passed 293/293. Repository/Visual Studio build outputs remain explicitly excluded as update targets.

- Add a minimal external updater/helper so the running single-file application never attempts to overwrite itself.
- Require successful signed-package readiness and a verified SQLite recovery backup before shutdown or application-file mutation.
- Stage on the same volume, preserve the active version as last-known-good and use durable transaction state plus atomic directory/file renames where supported.
- Restart the new version and require a bounded startup health acknowledgement covering release identity, executable inventory and SQLite schema/startup compatibility.
- Automatically restore and relaunch the prior application package if staging, replacement, launch or health acknowledgement fails. Never silently roll SQLite data back; retain the verified pre-update database backup for guarded recovery.
- Test success, crash/power-loss boundaries and rollback from each durable transaction phase before runtime acceptance.

### v43.6 — Update and Deployment Diagnostics

v43.6.0 status: complete and runtime accepted on 2026-07-22. Visual Studio Debug confirmed canonical identity, read-only history with one prior Committed transaction, zero incomplete transactions and Verification PASS 294/294. The extension preserves the v1 transaction schema and v43.5.1 process/package contracts.

- Show active, staged and last-known-good application versions, package/signature identity, SQLite pre-update backup and durable transaction status.
- Detect and safely resume or roll back an incomplete update after process termination or machine restart.
- Record concise update/rollback history without credentials or private signing material.
- Add cleanup rules that preserve the active version, incomplete transactions and the only known-good rollback package.
- Extend System Diagnostics and Verification Center without mixing application-update state with remote website FTPS deployment state.

### Later v43 — Installer and Portable Deployment

v43.7.0 status: per-user installer, exact six-file governed portable ZIP and isolated application-release deployment plan are runtime accepted. Clean VM install/restart, SQLite transfer, credential isolation, branding, Verification Center, live `/downloads` publish and browser download passed. Remote signed-update discovery/download remains follow-on work.

- Build installer and portable modes only after v43.5-v43.6 runtime acceptance.
- Both modes must consume the same signed package, manifest, governed file inventory, data separation and rollback contract; no second updater architecture.
- Preserve the configured SQLite storage folder independently of application install location and uninstall behavior.

## Pre-v44 Repository Privacy, Provenance and Hygiene Audit

Status: **Complete; Audits 1-7 passed. Option A replaced GitHub history with one sanitized root commit, and the repository is approved for public visibility subject to the retained-copy cautions recorded below.**

Purpose: prove that the application, distributable packages, local workspace and complete Git/GitHub history are safe before any public-repository or open-platform work begins. Preserve accepted runtime behavior and use small backwards-compatible corrections only. Each gate must record search scope, exact evidence, risk classification, decision and verification result before the next gate starts.

### Audit 1 — Historical 176-material dataset

Status: complete; **gate failed for repository privacy, while runtime/distributable clean-profile isolation remains proven. Remediation is blocked on complete-history Audit 6.**

- Search current tracked/untracked source, generated SQL, migrations, scripts, embedded resources, tests, docs and retained artifacts for the historical 176-material dataset, representative private material markers and any alternate serialized form.
- Inspect SQLite creation/seed paths separately from user-owned databases; never copy or expose the owner database during this audit.
- Classify every match as executable seed, inactive historical code, documentation, build output or false positive.
- Gate: no distributable/runtime path may create private Materials on a clean profile. Do not delete evidence until Git-history exposure is assessed in Audit 6.

Audit 1 evidence (2026-07-22):

- `App/FilamentDbApp/MainWindow.xaml.cs` contains exactly 176 historical `NativeMaterialRow` literals (`[private-material-id-removed]` through a non-contiguous final `[private-material-id-removed]`) inside `#if false`. The compiler excludes them and `GetDefaultNativeMaterialRows()` returns an empty list, but the private dataset text is tracked and visible in current GitHub `master`.
- `App/FilamentDbApp/Assets/website-template-index.html` is a tracked 590,345-byte data-bearing snapshot. Its `const DATA` payload contains 200 unique MaterialIDs (`[private-material-id-removed]`-`[private-material-id-removed]`) plus measurement/sample, pricing and video-link fields. Packaging excludes this file, but current-tree repository privacy fails.
- Retained local signed packages v43.4.1, v43.5.0 and v43.5.1 contain the same 590,345-byte website snapshot. Their application EXEs did not expose the known `[private-material-id-removed]` or private-name markers in UTF-8/UTF-16 scans. The production v43.8.8 signed package contains neither known marker and preserves the accepted zero-data clean-install boundary.
- No tracked or current non-build `.sqlite`, `.sqlite3`, `.db`, `.sql`, `.csv`, `.tsv`, `.xlsx` or `.xls` data file was found. SQLite inserts in `LocalDatabase` are parameterized persistence/import paths, not hard-coded material seeds.
- Immediate safety decision: keep the GitHub repository private; do not publish or mirror it. Do not remove current files or rewrite history until Audit 6 establishes every reachable copy and an approved remediation sequence.

### Audit 2 — Embedded HTML and website snapshots

Status: complete; **gate passed for current installer/portable/update payload isolation, but failed for current-tree repository privacy. Remediation remains blocked on complete-history Audit 6.**

- Inventory literal HTML, HTML templates, JavaScript payloads, compiled resources and `.html` files in source, build scripts and package inputs.
- Distinguish small renderer contract fixtures from historical website snapshots or data-bearing templates.
- Gate: no private website snapshot, embedded dataset or obsolete HTML artifact may enter installer, portable or update packages; required renderer fixtures must be minimal and documented.

Audit 2 evidence (2026-07-22):

- Exactly two tracked `.html` files exist. `App/FilamentDbApp/Assets/website-template-index.html` is the 590,345-byte obsolete/data-bearing snapshot identified by Audit 1: one `const DATA` payload, 200 unique MaterialIDs and private engineering/pricing/link data. It is not referenced by the project or runtime template loader; the canonical active main-site template is SQLite-governed. Keeping this snapshot in current Git nevertheless fails repository privacy.
- `App/FilamentDbApp/Assets/Website/MethodologyPortal.html` is a 20,400-byte intentionally embedded static methodology fragment. It contains no `const DATA`, MaterialID, price/cost field, FTPS identity or application dataset. Its three YouTube methodology links are public content, and `BuildMethodologyPortalHtml()` loads the named embedded resource for the website methodology section and Verification checks.
- HTML-like source in `MainWindow.xaml.cs`, website services and reporting services is renderer/validation code, not a stored customer dataset. Report builders HTML-encode typed model values; the Verification Center website-link fixture uses only synthetic `MAT-PUBLIC-*` identities. The 176 real MaterialID literals counted in `MainWindow.xaml.cs` belong solely to the separately identified disabled Audit 1 block, not to an HTML fixture.
- The v43.8.8 signed update ZIP contains only the application EXE, updater EXE, governed icon/logo, license, notices and signed manifest. The v43.8.8 portable ZIP contains the same governed application files without the manifest. Neither package contains an `.html` file or the obsolete snapshot; the installer is built from that already-verified extracted payload.
- Deployment construction explicitly rejects `website-template-index.html`, SQLite/database/spreadsheet/tabular data files, `native-*` files and known private seed markers. This is a useful second boundary, while the signed packager's exact governed inventory is the primary package boundary.
- Decision: retain the two HTML files and existing evidence unchanged until Audit 6 maps Git-history exposure. In remediation, remove the obsolete data snapshot from the current tree/history as approved, retain the data-free embedded methodology fragment, and preserve the current SQLite template and governed packaging contracts.

### Audit 3 — FTPS identities, endpoints and credentials

Status: complete; **clean-profile credential isolation and v43.8.8 package gates pass, but current-tree repository privacy fails because the private FTPS username remains in tracked historical documentation and a negative deployment marker. Remediation remains blocked on complete-history Audit 6.**

- Search case-insensitively for `3dpiceland`, `[private-ftps-identity-removed]`, `www.iskort.is`, FTP/FTPS URIs, usernames, passwords, tokens and connection strings across tracked/untracked files and package inputs.
- Classify public governed endpoints separately from private account identities and secrets. Confirm clean-profile FTPS host/user remain empty and passwords remain only in Windows Credential Manager.
- Gate: public HTTPS product endpoints may be explicit; private usernames, passwords and owner-specific defaults must not exist in distributable code or repository content.

Audit 3 evidence (2026-07-22):

- Clean SQLite schema creation inserts `DeploymentSettings` with an empty FTPS host, port 21 and empty username. `DeploymentSettingsRecord` has the same empty host/user defaults. Verification explicitly checks that a clean profile excludes the private deployment identity; the successful VM clean-install observation agrees with this code path.
- No password column or password configuration value exists. The password box falls back to a host/user-scoped Windows generic credential, and `WindowsCredentialService` uses `CredWriteW` only after a successful encrypted connection and `CredReadW` for later publishes. Password bytes are cleared after the native write. Source and non-build workspace scans found no non-empty password/secret/token/API-key assignment, credential-bearing FTP URI, `.env`, private-key, certificate-bundle or similarly named secret artifact.
- Exact private username matches in the current tree are limited to: a negative deployment-package rejection marker, the historical v39.2 changelog entry, the historical v40.15 build note and this audit plan/evidence. It is not a model default, SQLite seed, connection constructor default or public report value. The build note that says existing installations seed that identity describes the earlier compatibility state and is stale for clean-profile behavior after the deployment-isolation correction.
- `www.iskort.is` / `iskort.is` HTTPS references are intentional public product routes: website/report links, installer/support pages, downloads and the signed update feed. The update client enforces HTTPS, exact `www.iskort.is` host and `/3dp/updates/` root before accepting a package URL. These public browser endpoints are not authentication material.
- The v43.8.8 signed update ZIP, portable ZIP and installer contain no detectable private username marker. Text deployment/feed metadata contains no private username; the feed intentionally contains the public HTTPS update route. The package payload contains no FTP/FTPS credential URI.
- Repository-level gate result: FAIL until the private username is removed from current tracked prose and the rejection rule is converted to a non-identifying policy or hashed/test-safe fixture. Runtime/distributable gate result: PASS. No credential rotation is presently indicated because no password, token or private key was found, but Audit 6 must still scan every reachable historical blob and ref before that conclusion is final.
- Decision: do not alter FTPS runtime behavior, endpoints, documentation history or rejection markers yet. Retain evidence until Audit 6 maps GitHub exposure; then apply the smallest approved current-tree and, if necessary, history remediation while preserving empty clean-profile defaults and Windows Credential Manager storage.

### Audit 4 — `System.IO`, path aliases and repeat compile failures

Status: complete; **gate passed. The recurring failure is a compile-time WPF namespace ambiguity/editing-convention issue, not a missing `System.IO` dependency.**

- Collect every compile error/fix pattern involving `Path`, `File`, `Directory`, `System.IO`, conflicting WPF types and the existing `IOPath` alias.
- Determine whether failures come from missing namespace imports, ambiguous type names, partial-class conventions or inconsistent editing practice.
- Define one repository convention and enforce it with the smallest suitable mechanism: shared/global using, explicit alias, analyzer/build check and/or repository guidance.
- Consider a durable Codex memory note only after the convention is proven; memory supplements repository enforcement and must not be the sole safeguard.
- Gate: Debug/Release builds and a targeted ambiguity probe pass without one-off namespace repair.

Audit 4 evidence and remediation (2026-07-22):

- `MainWindow.xaml.cs` imports both `System.IO` and `System.Windows.Shapes`. Both namespaces define `Path`; a newly added bare `Path.*` expression therefore produces compiler error CS0104. `File` and `Directory` have no current WPF collision, and no missing `System.IO` assembly/package was found. This explains why the issue repeatedly appeared immediately after source edits and disappeared after qualification.
- Git history confirms the same repair pattern: v34.2.3 `Path Diagnostics Build Fix` changed bare `Path.GetFullPath`/separator calls to `System.IO.Path`; v43.5 updater work introduced a file-local `IOPath` alias for new transaction paths. Current `MainWindow.xaml.cs` had no remaining bare `Path.*` call before this remediation, but mixed fully-qualified and alias styles made recurrence likely.
- Added project-wide `IOPath`, `IOFile` and `IODirectory` aliases in `GlobalUsings.cs`; removed the redundant file-local `IOPath` alias. Added a compile-only `IoNamespaceConventionProbe` that deliberately imports `System.Windows.Shapes`, resolves WPF `Path`, and exercises all three IO aliases. Any future removal/breakage of the convention now fails Debug/Release compilation.
- Added root `AGENTS.md` repository guidance requiring the aliases for new filesystem code, explicit qualification for WPF shapes, and isolated `ArtifactsPath` builds when the running application locks normal output. This repository-owned guidance is the durable project memory; a separate assistant-memory-only rule is intentionally not the sole safeguard.
- Initial normal Debug build encountered MSB3027/MSB3021 because the running `3DPIcelandFilamentDB` process (PID 61340) held `bin/Debug/net9.0-windows/UpdateCore.dll`. This was an output-file lock, not a C# or `System.IO` failure. The application was not stopped. Release independently passed 0 warnings / 0 errors.
- After restoring to an isolated temporary `ArtifactsPath`, Debug and Release both passed with 0 warnings / 0 errors while the application remained running. The targeted ambiguity probe compiled in both configurations. No runtime, SQLite, website, report, FTPS or deployment behavior changed, so no new runtime Verification Center check was added for this compile-only convention.

### Audit 5 — Workspace structure and obsolete files

Status: complete; **inventory/decision gate passed. No file was moved, consolidated or deleted; privacy-sensitive cleanup remains blocked on Audit 6 and approved execution belongs to Audit 7.**

- Inventory the repository root and major subtrees by ownership, tracked status, size, duplication, generated output and last relevant use.
- Identify obsolete scripts/docs/assets, duplicate canonical documents, misplaced outputs and candidates for consolidation without deleting user data or accepted evidence.
- Gate: present a keep/move/consolidate/delete table first. Destructive cleanup requires explicit target validation and must leave build, Verification and packaging contracts intact.

Audit 5 evidence (2026-07-22):

- Git tracks 224 files totaling 6,325,314 bytes; there are no non-ignored untracked files, no case-colliding tracked paths and no exact duplicate tracked-file hashes. The Git worktree was clean and `master == origin/master` at audit start.
- The non-Git workspace is approximately 1.31 GB. `App/artifacts` owns 611,261,621 bytes of retained releases, while ignored `bin`, `obj`, `.vs` and `App/FilamentDbApp/artifacts/codex-build` outputs account for approximately 697 MB and are reproducible. The running application may lock normal Debug output, as confirmed in Audit 4.
- The v43.8.8 `feed` and `signed` update ZIPs are byte-identical (SHA-256 `670AEA149958B561B061A8F532E477423F32E00EFD6E95306B1FC93058D71A5D`). Their duplication follows publishing-stage folder ownership rather than differing content. The older v43.4.1, v43.5.0 and v43.5.1 archives each retain the private website snapshot; current v43.8.8 archives do not.
- Release identity is inconsistent only in secondary documentation: `FilamentDbApp.csproj`, `BuildInfo` and current build notes identify v43.8.8, but `Docs/VERSION.txt` still says v43.5.1; README development focus still says v40.20.0; `PROJECT_STATUS`, `RELEASES`, regression/daily-use checklists and known-limitations introductions retain older release baselines. These files are not package inputs, but they are misleading maintenance surfaces.
- Three tracked SVGs under `App/FilamentDbApp/Assets/Documentation` have no filename reference in source, project metadata or documentation. They were created for v40.14 whitepaper work but are neither embedded nor copied by the current project. The icon/logo assets and embedded methodology fragment all have active owners and must stay.
- `App/build_release.bat` has not changed since v29.2 and performs only a direct main-app publish; it bypasses the updater, exact signed inventory, verifier and deployment/feed workflow. It must not be treated as a production release command. `run_from_source.bat` remains a valid convenience launcher; the three PowerShell scripts are the current governed signed/update/deployment pipeline even when not linked from README.
- `MainWindow.xaml.cs` is approximately 1.68 MB and `LocalDatabase.cs` approximately 198 KB. Their size is a maintainability risk, but splitting partial classes/services is a separate code-refactoring project with regression risk—not workspace cleanup—and is intentionally outside Audit 5.

Audit 5 decision table:

| Decision | Exact scope | Reason and prerequisite |
| --- | --- | --- |
| Keep | `App` source/projects, `UpdateCore`, updater, `Tools`, installer definition, root governance/license files | Active build, verification, update and deployment ownership. |
| Keep | Active icon/logo files, embedded `MethodologyPortal.html`, `run_from_source.bat`, all three governed PowerShell release scripts | All have a current runtime, documentation, convenience or release-pipeline owner. |
| Keep | v43.8.8 production installer, portable, signed package, feed and deployment metadata | Canonical reproducible release evidence. Retain current folder separation until the publish workflow documents whether the identical feed ZIP may be generated on demand. |
| Keep for now | Changelog, build history, milestone history, releases inventory, project history and specialist roadmaps/docs | They overlap by design but have non-identical content and historical references. Consolidation requires a documentation-governance pass, not blind deduplication. |
| Consolidate/update | `Docs/VERSION.txt`, README current focus, `PROJECT_STATUS.md`, `RELEASES.md`, old regression/daily-use checklist headers and `KNOWN_LIMITATIONS.md` introduction | Align secondary documentation to canonical v43.8.8 or label it explicitly historical. Perform in Audit 7 after history/privacy decisions so one release-document pass is sufficient. |
| Replace or delete | `App/build_release.bat` | Obsolete unsafe production affordance because it bypasses the governed updater/package verifier. Prefer removal or a wrapper that calls the canonical signed workflow; decide in Audit 7 and re-run packaging verification. |
| Move or delete | Three unreferenced `Assets/Documentation/*.svg` files | No current project/source/doc owner. If historically useful, move under documentation assets; otherwise delete after Audit 6 confirms history/evidence needs. |
| Delete after Audit 6 | Tracked `Assets/website-template-index.html` and retained v43.4.1/v43.5.0/v43.5.1 local ZIPs | They contain private dataset snapshots. Preserve until the complete Git-history exposure map and remediation evidence are complete. |
| Delete when app is closed | Ignored `.vs`, all `bin/obj`, and `App/FilamentDbApp/artifacts/codex-build` outputs | Approximately 697 MB of reproducible local output. Validate resolved paths and avoid locked running-app files; canonical `App/artifacts/v43_8_8-production` is excluded from this cleanup. |
| Ignore/remove locally | Empty untracked `.agents` directory | No files, Git content or runtime owner; immaterial and safe to leave. |
| Defer | Splitting `MainWindow.xaml.cs` and `LocalDatabase.cs` | Valuable future maintainability work, but not file hygiene and not safe to combine with privacy/history remediation. |

- Decision: make no destructive workspace change in Audit 5. Audit 6 must first establish which current and historical blobs are GitHub-visible. Audit 7 may then execute explicitly approved, path-validated cleanup, followed by Debug/Release, Verification Center and complete signed-package/deployment verification.

### Audit 6 — Complete Git history and GitHub exposure

Status: complete; **gate failed. Private material datasets, a private FTPS identity, personal workstation paths, Visual Studio user-state and one diagnostics export remain reachable from GitHub `master` history. No literal password, token or private key was found.**

- Search every reachable commit, branch and tag—not only `master` HEAD—for the 176-material dataset, private website snapshots, FTPS usernames, credentials, personal paths, databases, spreadsheets, generated reports and other non-public content.
- Compare local refs with GitHub-visible branches/tags and scan historical blobs by content, filename and size. Treat deletion from the current tree as insufficient if a blob remains reachable in history.
- Classify findings as public-safe, privacy-sensitive, secret requiring rotation, or historical data requiring removal.
- Gate: produce a remediation decision before changing history. Any `git filter-repo`/BFG rewrite, force-push, tag deletion, collaborator coordination or credential rotation is a separate destructive operation requiring explicit approval, backup and post-rewrite verification.

Audit 6 evidence (2026-07-22):

- Live remote refs contain `master` at the audited head plus an unrelated one-commit `main` branch containing only `LICENSE`; no tags exist. Remote symbolic `HEAD` points to `master`. The local cached `origin/HEAD` still points to `origin/main` and is stale, but this does not change reachability. `master` has 364 commits, `main` has one unrelated root commit, and all sensitive commits are contained by `origin/master`.
- All reachable refs comprise 365 commits and 3,277 unique blobs (340,925,830 logical bytes across historical blob versions). The scan covered text and binary blobs by content, filename, extension and size rather than checking only current paths.
- Private material markers occur in 222 unique blob versions. v27.3 commit `[historical-commit-removed]` introduced the 176-row `GetDefaultNativeMaterialRows()` source dataset as an active populated fallback; it remained executable throughout the older history and was only compile-disabled by `[historical-commit-removed]` on 2026-07-22. Current `master` still tracks the text inside `#if false`.
- Five unique historical website-snapshot blobs are reachable across the old and current path prefixes. They contain respectively 163, 176 or 200 unique MaterialIDs and include measurement data; the 200-material variants also include pricing fields. The current tracked 590,345-byte variant is one of these reachable blobs.
- The private FTPS username occurs in 154 blob versions across historical SFTP/FTPS code/defaults, UI, settings, documentation and the later negative deployment marker. It was introduced with the former SFTP workflow and later removed from clean-profile runtime defaults, but current prose/marker references and all earlier identity-bearing blobs remain reachable.
- Commit `[historical-commit-removed]` accidentally added Visual Studio `.vs` databases, indexes, caches, layout and user-state. Commit `[historical-commit-removed]` removed them from the current tree, but their blobs remain reachable. Three of those opaque/binary blobs expose the personal Windows user path and may retain additional IDE-indexed source context that cannot be proven safe by plain-text scanning.
- Commit `[historical-commit-removed]` added `3DPIceland_FilamentDB_Diagnostics_20260720_071430.txt`; `[historical-commit-removed]` later deleted it. The reachable export contains personal database/executable/storage paths, project row counts, two MaterialID references and detailed verification/system state. It contains neither the FTPS username nor a password marker.
- No historical application `.sqlite`, `.sqlite3`, general application `.db`, Excel workbook, CSV/TSV, SQL dump, release ZIP/archive, certificate bundle or private-key file was found. The only `.db` paths are historical Visual Studio Copilot index databases. No private-key header, GitHub/AWS/Slack token signature, credential-bearing FTP URI, literal password/secret assignment or password-bearing connection string was found across reachable blobs. Apparent password hits were variable/member/method expressions in the Credential Manager flow, not stored values.
- Current tracked files contain domain-owned `iskort.is` contact addresses intended for product/site contact. Separately, Git commit author/committer metadata exposes non-noreply personal/local email domains; changing visible file content will not remove that metadata.
- Current-tree deletion alone is insufficient: every prior commit remains downloadable from `origin/master`. Removing only branch `main` would not affect the exposure because it contains only the unrelated license root; removing only the affected current files would also leave all historical blobs reachable.

Audit 6 remediation decision:

| Option | Result | Trade-off / decision |
| --- | --- | --- |
| A — New clean root (recommended) | Build one sanitized root commit from the approved current tree, force-replace `master`, remove the unrelated `main` branch, and publish only after remote verification | Strongest and easiest-to-prove privacy boundary for this young private repository; intentionally discards public Git history while a secure local bundle can retain evidence. |
| B — Targeted `git filter-repo` rewrite | Remove both website-snapshot path histories, `.vs/**`, the diagnostics export, private FTPS identity text, personal paths/metadata and every historical active/disabled material-seed block while preserving other commits | Preserves more history but requires custom blob transformation for many evolving `MainWindow.xaml.cs` versions, author metadata rewriting and a materially harder residual-content proof. Higher risk of missing a variant. |

- Recommended sequence, requiring explicit approval before execution: keep the repository private; freeze pushes; create and verify an offline sensitive Git bundle with restricted local storage; sanitize the current tree under Audit 7; choose Option A unless historical commit continuity is essential; force-push the replacement `master`; delete obsolete remote refs; have any other clones discarded/recloned; re-scan every live remote ref/blob; account for GitHub cached objects/forks/pull-request refs before public visibility; then run Debug/Release, Verification Center, signed-package verifier and deployment checks.
- Credential decision: no mandatory rotation is indicated by this scan because no password/token/private key was found. The FTPS username and public endpoint are exposed identifiers, not authentication secrets. Optional password rotation after the rewrite remains a defense-in-depth choice, especially if the same password has ever been shared elsewhere.
- Until that sequence passes, repository-public-readiness is **NO**. No history rewrite, force-push, branch deletion, file deletion, credential change or remote visibility change was performed by Audit 6.

### Audit 7 — Remediation and public-readiness closure

Status: complete; the owner approved Option A (new clean root) on 2026-07-22, and every current-tree, build, package, replacement-root and remote verification gate passed.

- Apply only approved remediations, smallest first; keep website/report/FTPS runtime engines outside scope unless a confirmed exposure requires a targeted correction.
- Re-run current-tree scan, full-history scan, secret scan, Debug/Release, updater self-test, signed-package verifier and Verification Center.
- Record residual risks, intentionally public endpoints, retained historical evidence and any required key/credential rotation.
- Gate: workspace clean, `master == origin/master`, no unapproved sensitive GitHub content, and a documented public-repository readiness decision.

Audit 7 approved execution record (2026-07-22):

- Created a complete offline bundle of all pre-rewrite refs under the owner's private Documents archive, verified it with `git bundle verify`, and recorded its size and SHA-256 outside the repository workflow. This bundle is privacy-sensitive evidence: never publish, upload or copy it into the repository.
- Moved the retained v43.4.1, v43.5.0 and v43.5.1 signed archives containing the historical website snapshot into the same private archive boundary. Source removal and destination SHA-256 equality were verified; canonical v43.8.8 production artifacts remain in place.
- Removed the historical compiled-source material literals entirely. `GetDefaultNativeMaterialRows()` now has only the zero-row clean-profile implementation; the existing Verification Center gate continues to assert that count is zero.
- Removed the private data-bearing website snapshot, three unreferenced documentation SVGs and the obsolete direct-publish batch file. The governed PowerShell signed-package, verifier and deployment workflow remains canonical.
- Removed the obsolete packaging-script marker list that repeated private material identifiers. Added `Tools/Sanitize-CurrentTreeForCleanRoot.ps1`, whose sensitive FTPS identity is supplied only as an execution parameter, so future reproducible sanitation does not retain the value.
- Replaced private FTPS identity text, private material literals and selected historical identifiers in retained documentation with explicit redaction markers. Public product-domain references remain intentional; passwords continue to be Windows Credential Manager-owned and are not stored in Git.
- Aligned the primary README, release inventory and version identity with canonical v43.8.8. Older milestone/checklist content remains explicitly historical rather than being rewritten as current behavior.
- Debug and Release isolated builds both passed with 0 warnings and 0 errors. The updater self-test passed commit, injected rollback, failed-health rollback, Prepared restart, SnapshotReady/Installed/RollingBack/RollbackFailed recovery, read-only history, traversal rejection and SQLite-backup-reference preservation. The canonical signed v43.8.8 package passed the production verifier with six governed files and supported SQLite schema v29.
- The previously runtime-accepted v43.8.8 Verification Center result remains applicable: the only application-source cleanup removed text inside an already compiler-excluded block and preserved the existing zero-row verification assertion. No executable website/report/FTPS engine or SQLite behavior changed in Audit 7.
- Created one parentless root commit with non-personal noreply metadata, force-replaced remote `master` using an exact force-with-lease, deleted obsolete remote `main`, refreshed remote `HEAD` to `master`, and removed obsolete local references/reflogs after remote confirmation.
- A fresh clone of the rewritten GitHub repository exposed exactly one commit, only `origin/master`, no tags, no sensitive identity/material/personal-path/secret markers and no old audited commit object. The final amended root was re-pushed and rechecked against the same gates.
- Public-readiness decision: **YES** for the current reachable repository. Residual operational caution: GitHub may retain inaccessible caches for an unspecified period, and every pre-rewrite clone plus the private archive bundle still contains the old history. Never publish those copies; any collaborator must discard the old clone and re-clone. The public product domain is intentionally retained, while the private FTPS username and all credentials remain excluded.

## v43.8.9 SQLite Dependency Security

Status: complete and canonical; automated gates, local Verification Center, guarded clean-VM update, restored-data runtime and final VM Verification Center passed.

- Keep the application on `net9.0-windows` and update `Microsoft.Data.Sqlite` only within the supported 9.0 servicing line.
- Explicitly select a non-affected SQLitePCLRaw native bundle so NuGet minimum-version resolution cannot retain 2.1.10 or 2.1.11.
- Do not change schema v29, SQLite ownership, backup/restore behavior, interrupted-update recovery, website/report/FTPS engines or automatic-restore policy.
- Gates: resolved dependency inventory contains no vulnerable package; Debug and Release build with zero warnings/errors; updater self-test and package verifier pass; clean-profile/static privacy checks pass; Verification Center and VM database/runtime acceptance are required before canonical release, commit and push.
- Local hygiene: remove only reproducible ignored `.vs`, `bin`, `obj` and noncanonical build-output directories after validation. Preserve `App/artifacts/v43_8_8-production`, all user SQLite/backups/configuration/evidence and the private pre-clean-root archive.

Automated evidence (2026-07-22):

- NuGet resolved `Microsoft.Data.Sqlite` 9.0.18 and the complete SQLitePCLRaw native/provider graph at 2.1.12. `dotnet list package --vulnerable --include-transitive` reports no known vulnerable package from the configured sources; the former high-severity 2.1.10 native library is absent.
- Isolated Debug and Release builds passed with 0 warnings and 0 errors. The updater self-test passed commit, rollback, every interrupted-state recovery phase, traversal rejection and SQLite-backup-reference preservation.
- A dirty-tree pre-release v43.8.9 signed candidate passed the production application verifier with exactly six governed files and schema v29. It is retained under `App/artifacts/v43_8_9-security-candidate`; it is not canonical or publish-approved until runtime acceptance.
- Removed all enumerated reproducible ignored `.vs`, `bin`, `obj`, legacy application-local build artifacts and isolated test output. No locked target failed. Preserved only canonical `v43_8_8-production` and the explicit `v43_8_9-security-candidate` beneath `App/artifacts`; user data and the private Git archive were outside every cleanup target.
- Visual Studio subsequently reported NU1105 for `UpdateCore` because the application referenced it while the project was absent from the solution membership and the prior IDE cache had hidden that gap. Command-line restore/build proved both projects valid; `UpdateCore` was then added explicitly to the solution with Debug/Release configurations to make clean-cache Visual Studio restore deterministic.
- Local Release runtime acceptance passed on 2026-07-22: Verification Center reported PASS 296/296 for `v43.8.9 SQLITE-DEPENDENCY-SECURITY`, assembly 43.8.9.0, informational identity aligned, schema v29 and the owner's 200-material SQLite database operating normally. The exported diagnostics contained 296 PASS lines and zero FAIL lines. Commit/push of the completed local security gate is approved; VM update/runtime acceptance remains the final candidate-promotion gate.
- Documentation path hygiene consolidated all 45 lowercase `docs/` files into the canonical `Docs/` tree without filename collisions or content deletion. All active path references now use exact GitHub case, and README has one current v43.8.9 candidate identity instead of the stale v41.6 declaration.
- The first v43.8.8 VM update discovery failed before mutation because generated `latest.json` began with the Windows PowerShell UTF-8 BOM (`EF BB BF`). The generator now writes BOM-less UTF-8 explicitly, and the v43.8.9 client defensively accepts one standard UTF-8 BOM. This is a parser-compatibility extension only; package bytes, SHA-256, trusted manifest, schema and default-No apply gates remain mandatory.
- Final VM acceptance passed on 2026-07-22. A fresh zero-Materials v43.8.8 installation consumed the corrected feed, updated to v43.8.9 without an error, recorded the transaction as `Committed` with zero incomplete transactions and preserved the zero-data boundary. A verified schema-v29 owner backup restored 200 Materials, 3,728 tensile samples, 3,752 impact samples and 191 stiffness rows; creating a canonical post-restore SQLite backup satisfied all recovery evidence gates. Verification Center then reported PASS 296/296 with zero FAIL lines. v43.8.9 is the canonical release.

## Future — v44 Open Engineering Platform

- Governed public API.
- Broader CSV/JSON exchange.
- Plugin and scripting architecture.
- External research and manufacturer integrations.
- Optional cloud synchronization.

All open-platform work must preserve canonical MaterialID identity, SQLite
ownership, Verified Material Summary boundaries and backwards compatibility.

## Backburner

### Future report extensions

- Experimental Research Report for one canonical experimental series, including
  baseline, controlled runs, analytics, charts and conclusion.
- Verification & Data Quality Report for coverage, missing evidence, specimen
  counts, repeatability/CV review, orphan checks and release-gate transparency.

Treat Material Family Benchmarking as a Comparison Report preset. Do not create
batch/history, printer-profile or durability reports until the required
canonical session, profile or long-term test data exists.

### Canonical material printing profiles

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

Make the current Materials-grid selection persistent and visually unmistakable
after the grid loses focus. The selected row/MaterialID should remain clearly
highlighted until another material is selected, and downstream tabs such as
Material Detail and Reports should expose the same selected identity. This is a
focused desktop UX improvement and must not change canonical MaterialID or
filter behavior.

### Startup performance optimization (scheduled after v41.7)

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

Profile and improve the delay when opening top-level desktop menus such as
`Tools` and `Help`. Measure first-open and repeated-open latency separately,
identify whether menu construction, command state evaluation or UI-thread
background work owns the delay, and preserve existing commands and keyboard
behavior. This is a presentation-performance task only; it must not change
SQLite data, MaterialID identity, calculations or publishing behavior.

### Splash screen logo line animation

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

The browser-only enquiry form and email handoff remain canonical for now.
A future server endpoint may add validation, spam protection, rate limiting,
delivery acknowledgement and governed intake states. It must never write
directly from the public website into the engineering SQLite database.

### Bilingual public website and branding

Investigate a future additive website-localization milestone that keeps the
current English site and adds a complete Icelandic presentation with a clear
`EN / IS` language toggle near the top of the site. The investigation must
define governed translation ownership, fallback behavior, canonical URLs and
metadata, report-language scope, Preview/Production parity and Verification
coverage before implementation. It must reuse the canonical website renderer
and publishing pipeline rather than creating parallel site engines.

Add an approved 3DPIceland logo/brand asset to the public website as part of the
same presentation milestone, with responsive and accessible rendering. Logo
source, variants and placement must be governed so generated pages, dark/light
surfaces and published assets remain consistent. This is roadmap-only for now;
no website, report or publishing behavior changes in v43.3.1.

### Legacy Excel and compatibility surface audit

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
- Do not begin this audit inside the active update/rollback milestone; schedule
  it as a separate release with runtime acceptance before commit/push.

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
