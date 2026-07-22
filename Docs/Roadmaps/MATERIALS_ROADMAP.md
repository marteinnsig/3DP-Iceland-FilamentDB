# Materials Roadmap

Maintain canonical MaterialID definitions, controlled settings, classification, archive behavior and clean handoff into Engineering and Publishing.

## v43.7.0 — Installer and Portable Deployment

Status: complete and runtime accepted; clean VM starts with zero materials and owner data moves only through explicit SQLite transfer/import.

- Produce per-user Setup EXE and portable ZIP from the same signed 11-file update package.
- Preserve SQLite storage, backups, credentials and transaction evidence independently of install/uninstall.
- Publish exact installer/portable artifacts through an isolated default-No application-release FTPS plan.
- Keep website deployment manifests/routes and update apply contracts unchanged.

## v43.6.0 — Update and Deployment Diagnostics

Status: complete and runtime accepted on 2026-07-22; Verification PASS 294/294.

- List prior durable update transactions read-only in System Diagnostics.
- Detect Prepared, SnapshotReady, Installed, RollingBack and RollbackFailed at normal startup.
- Require a default-No confirmation before the external helper changes governed application files.
- Restart Prepared safely; restore last-known-good application files for later incomplete phases.
- Retain evidence and backups; never automatically restore SQLite or touch website/report/FTPS engines.

## v43.5.1 — Guarded Application Update

Status: complete and isolated portable runtime accepted on 2026-07-22; unattended updating remains disabled.

- Offer default-No Apply only for a newer fully production-signed package, then reverify during contained extraction.
- Commit pending Materials edits, require SQLite save and create a verified manual database backup before shutdown.
- Wait for the old PID, snapshot/install governed app files and require matching release/schema health acknowledgement after usable startup.
- Kill and roll back/relaunch on launch, timeout, version, schema or health failure; never silently restore SQLite.
- Test v43.5.1→signed v43.5.2 only in a disposable portable install folder, not repository or Visual Studio output.
- Runtime result: transaction committed, health acknowledgement reported v43.5.2/schema v29, verified pre-update backup was retained and Verification passed 293/293.

## v43.5.0 — Transactional Updater Engine

Status: complete and runtime accepted on 2026-07-22; no live Apply path is enabled.

- Add an external updater helper and shared durable transaction-state contract.
- Snapshot only governed application files, install from contained staging and restore the complete last-known-good set on partial-install or health failure.
- Preserve the verified SQLite backup path as evidence while never silently replacing SQLite data.
- Prove commit, injected partial failure, failed health and traversal boundaries in disposable isolated app trees.
- Include the helper as the tenth governed file in signed packages; defer live process orchestration and Apply UI to v43.5.1.

## v43.4.1 — Governed Signed Release Packaging

Status: complete and runtime accepted on 2026-07-22; application update/apply remains disabled.

- Keep the production ECDSA private key non-exportable in the Windows user-scoped CNG key store; embed only the fingerprint-pinned public key.
- Build one signed package from an exact nine-file self-contained Release inventory and block any unexpected publish output.
- Verify the newly created package through the real application verifier before declaring the artifact ready.
- Exclude SQLite, backups, credentials, configured storage, reports, website staging and developer output.
- Record the single-machine non-exportable key continuity risk; design rotation/recovery before unattended updating.

## Future — Legacy Excel and compatibility surface audit

- Locate all remaining readers, commands, handlers and documentation tied to the retired original Excel Materials database and other pre-SQLite working stores.
- Distinguish obsolete legacy import/default/cache behavior from the current governed Excel disaster-recovery package and supported migration safety.
- Remove only call-traced obsolete or dead surfaces, in small increments with SQLite ownership and existing recovery/report/publishing gates preserved.
- Require Debug/Release builds, a dedicated Verification gate and runtime acceptance before each cleanup batch is committed.

## v43.4 — Signed Update Readiness Foundation

Status: runtime accepted on 2026-07-22; update application remains intentionally disabled until the production trust key and external transactional updater are governed.

- Inspect a selected update ZIP without extracting or changing application/SQLite files.
- Require a versioned manifest, safe relative paths, exact inventory, byte lengths, SHA-256 and ECDSA P-256 signature policy.
- Block tampering, traversal, downgrade, unsupported SQLite schema and missing production trust; never fall back to hash-only acceptance.
- Keep SQLite backup/restore, configured storage, credentials and website FTPS deployment outside the application-file update boundary.

## v43.3.1 — Backup and Recovery Center UI

Status: complete and runtime accepted on 2026-07-22.

- Consolidate SQLite backup/verify/restore and governed Excel backup/restore in one center; omit the retired legacy Excel database import.
- Provide one Open Storage Folder action because database and backup folders share the governed path.
- Remove backup creation and duplicate folder-opening actions from System Diagnostics.
- Preserve storage-folder configuration, compatibility rules, transactional recovery and all publishing boundaries.

## v43.3 — Recovery Compatibility Center

Status: runtime accepted; schema v26/v27/v28/v29 classification, isolated migration verification, manual backup identity, guarded same-state restore, restart and Verification PASS.

- List every local SQLite backup with type, schema, integrity, core counts and compatibility status.
- Block schema v26 and older as non-standalone, block newer/corrupt files, and accept current schema directly.
- Require a self-deleting isolated v27/v28→v29 migration dry-run with canonical count preservation before restore eligibility.
- Route both Recovery Center and direct SQLite restore through the same compatibility policy.
- Preserve recovery snapshots separately from the rotating automatic-backup set.
- Leave remote website restore, Excel disaster recovery, reports and publishing unchanged.

## v43.2 — Excel Disaster Recovery

Status: runtime accepted on 2026-07-22; 29-sheet export, 21-table/8,188-row restore, verified SQLite recovery snapshot, restart and pre/post-restore Verification PASS.

- Package all canonical SQLite user-data tables behind a versioned Excel manifest while retaining readable engineering sheets.
- Encode NULL, Unicode text, BLOB and long values losslessly and hash every ordered logical table.
- Reject missing tables, incompatible columns, incomplete chunks, changed hashes and newer schemas before mutation.
- Take a verified SQLite recovery backup and restore the exact governed table set in one transaction.
- Verify foreign keys, SQLite integrity and canonical Materials count before commit and controlled restart.
- Keep passwords and external referenced file contents outside Excel; SQLite backup remains preferred.

## v43.1 — Local SQLite Backup and Restore

Status: runtime accepted on 2026-07-22 with verified backup inspection, recovery snapshot, automatic restart and 286/286 Verification PASS.

- Create consistent SQLite online backups and verify integrity before retention.
- Inspect schema and canonical Materials/measurement counts before restore.
- Preserve the current database in a separate recovery snapshot before atomic replacement.
- Suppress stale close-save and restart through the normal migration/startup path after restore.
- Keep Excel export explicitly secondary until complete governed recovery round-trip support is delivered.
- Do not change remote Production restore, report rendering, website generation or FTPS publishing.

## v42.17 — SQLite Canonical Working Stores

Status: implemented for runtime acceptance.

- Reconcile all 200 Materials rows and 78 persisted fields between JSON and SQLite before changing ownership.
- Make `NativeMaterialManagerRows` the direct startup/load/save source and retain JSON only as an empty-database seed snapshot.
- Store general calculation, equipment and currency Settings in normalized SQLite rows with unique section/parameter/consumer keys.
- Keep Base Material Catalog and Deployment Settings in their dedicated SQLite tables.
- Require a successful SQLite backup before schema upgrade and verify Materials/Settings parity at runtime.

## v42.16 — SQLite Native Measurements

Status: complete and runtime accepted on 2026-07-22.

- Reconcile production JSON and SQLite without writes before migration design.
- Require a successful SQLite backup, then migrate tensile, impact, stiffness and notes in one transaction.
- Preserve every JSON-only sample/note and reject conflicting/count-mismatched migration before commit.
- Read and save native measurement grids through SQLite only after the migration marker is committed.
- Retain JSON unchanged as a migration snapshot, not an active source.
- Throttle automatic editing backups while retaining the established 20-backup policy.

## v42.15 — Deployment Settings Governance

Status: implemented for runtime acceptance.

- Store FTPS host, port and username canonically in SQLite and edit them through governed Settings Manager rows.
- Keep passwords exclusively in Windows Credential Manager with host/user-scoped credential identity.
- Preserve backwards-compatible defaults and legacy credential read fallback.
- Keep remote routes, backup root, TLS/passive behavior, concurrency, retry, incremental publishing, rollback and restore algorithms unchanged.

## v42.13 — Material Printing Settings Foundation

Status: complete and runtime accepted on 2026-07-22.

- Extend each canonical MaterialID with optional nozzle-temperature, bed-temperature and print-speed min/recommended/max values.
- Fix units at °C, mm/s and hours; blank values mean unknown or not recorded, never zero.
- Keep cooling and enclosure requirements vendor-neutral and governed by UI choices.
- Keep printer and slicer profiles as optional references without committing the schema to one vendor or file format.
- Preserve additive SQLite migration, JSON transition compatibility and Excel import/export round-tripping.
- Keep every new field internal until schema, UI, persistence and Verification Center pass runtime acceptance and a later release explicitly changes public allowlists.

## v42.14 — Base Material Printing Profiles

Status: implemented for runtime acceptance.

- Move the controlled 3DPIceland test/G-code baseline from each MaterialID to the SQLite Base Material Catalog.
- Resolve Material Detail printing settings through the material's Base Material.
- Keep temperature, speed and cooling min/recommended/max values, plus drying, enclosure and vendor-neutral printer/slicer references.
- Remove printing columns from the already-wide Materials grid; test-only per-material values are intentionally discarded.
- Treat profile ID/kind as trailing governance metadata in the catalog, not routine Material fields.
- Keep public report allowlists unchanged and defer destructive Excel-import testing until backup/restore safety work.
- Audit remaining JSON working stores and migrate canonical measurement/settings data to SQLite in a separate bounded milestone.
