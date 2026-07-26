# Automated Runtime Acceptance

## v48.0.3 - Usage event contract

- Documentation/ownership contract only; no runtime behavior or schema changed.
- No new UI action, AutomationId, scenario authorization or seed is required.
- Existing Production, FTPS, owner-database and unexpected-dialog guards remain
  unchanged.
- v48.0.4 pure domain work must remain non-persistent; persistence later
  requires bounded disposable CRUD/reversal/recovery evidence.

## v48.0.2 - Governed value index

- No new write workflow exists; smoke automation does not select mutable
  Recommendation state.
- Verification proves Overall-score/MSRP calculation, both inputs, comparison
  scope, disclosure and missing-score/price behavior.
- Stable `RecommendationDetailValueIndex` AutomationId is present for future
  bounded UI automation.
- Profile `20260726161514-840e50cc` passes 352/352 with exact database byte and
  business-state equality.
- Selection-scope correction profile `20260726162917-7e102563` passes 353/353;
  exact Base Material scope and database/business-state equality pass.
- MSRP identity profile `20260726163905-dfb93502` passes 354/354 and proves the
  exact recommended MaterialID is shown beside its canonical MSRP.
- Owner runtime accepted PLA/ASA scope refresh, alternatives and price identity;
  owner Full Data Verification passes 354/354.
- Production, FTPS, owner-database and unexpected-dialog guards are unchanged.

## v48.0.1 candidate - Canonical pricing provenance

- No new UI action or write workflow is introduced, so no tester scenario is
  added.
- Verification deterministically proves valid configured-rate conversion.
- Missing rates, unsupported currencies and missing canonical MSRP remain
  unavailable without 1:1, landed-cost or legacy substitution.
- Profile `20260726150144-83168b2e` passes 351/351 with exact database byte and
  business-state equality.
- Production, FTPS, owner-database and unexpected-dialog guards are unchanged.
- Owner runtime accepted the pricing surfaces and Full Data Verification PASS.

## v47.0.3 candidate - Stable coverage identity

- Smoke automation reads the coverage identity summary from disposable storage.
- A new disposable profile must begin with zero stable and zero legacy entries.
- Automation never invokes the explicit binding action.
- Verification proves stable-first lookup, one unique exact binding candidate
  and preservation of unmatched legacy entries.
- Stable AutomationIds cover the binding action and identity summary.
- Production, FTPS, owner-database and unexpected-dialog guards are unchanged.
- Profile `20260726141853-e0ce0c53` passes 350/350 with zero stable and zero
  legacy entries in isolated storage and exact pre/post database-state equality.
- Owner runtime accepted visible identity state, zero-legacy behavior, coverage
  workflows and Full Data Verification PASS.

## v47.0.2 candidate - AI collection workflow clarity

- Every scenario reads the stable collection Create/Update action state.
- The runner invokes only the read-only collection preview.
- Preview must state that no data was written and report unique MaterialIDs.
- Standard automation never invokes collection create/update or touches
  personal AppData collection storage.
- Existing Production, FTPS, owner-database and unexpected-dialog boundaries
  remain unchanged.
- Initial profile `20260726133919-b630ebf4` failed only the in-process
  Verification control-name lookup; no canonical or owner data was involved.
- Corrected profile `20260726134033-9c21103d` passed but exposed an owner
  collection title because legacy AI storage still resolved to personal
  AppData. It did not write collection data.
- AI session/collection storage now resolves to the disposable PreferencesFolder
  under automation and retains normal AppData ownership outside automation.
- Owner review found misleading cancelled-update output; persisted data was not
  written, but proposed filter scope looked saved.
- Verification now proves that cancel output retains persisted MaterialIDs and
  excludes proposed IDs.
- Final profile `20260726135702-ceb78987` passes read-only preview, cancel-state
  honesty, explicit absence of disposable AI session/collection files, Full
  Data Verification 349/349 and exact logical/business-state equality without
  personal AppData access.
- Owner runtime accepted the workflow and Full Data Verification 349/349.
  v47.0.2 is canonical.

## v47.0.1 candidate - Local AI Assistant scope clarity

- Every scenario navigates `AiAssistantTab` by stable AutomationId.
- The runner explicitly refreshes and reads the visible MaterialID scope.
- The scope must report unique MaterialIDs and expose a bounded MaterialID
  preview before generation.
- The runner generates a local full brief and requires visible-scope evidence
  in the read-only output.
- No external network, credential, destructive action or scenario
  authorization is added.
- The smoke scenario remains read-only and must preserve exact baseline/final
  canonical business state.
- Profile `20260726122624-99485959` passes with 201 visible rows, 201 unique
  MaterialIDs and Full Data Verification 348/348.
- Logical hash before/after:
  `F0EDCC3295A114C935668D2B92D7A1AEB1C67C4D1630EFC89F11B7FCDC556E5F`.
- Normalized business-state hash before/after:
  `4FBCF6A2656678875A6692C0A7AA30CD0CDC3F4AAB83003B3BB2C77081B1C87D`.
- Owner runtime accepted the visible-scope workflow and Full Data Verification
  348/348. v47.0.1 is canonical.

## Approved local disposable seed

The owner-approved local automation seed is:

`C:\Seed-Database\filamentdb.sqlite`

This path must still be passed explicitly with `--seed-database`. The runner
copies it below the disposable `%TEMP%\3DPIceland-Automation\<ProfileId>` root
and never mutates the source seed. It must not be generalized into owner-data
discovery or fallback selection.

## v44.7.14 Stage 1 candidate

The Stage 1 runner is a read-only smoke and evidence tool. It does not replace
owner runtime or visual acceptance.

Safety boundaries:

- The runner never discovers or selects the canonical owner database.
- `--seed-database` is mandatory and must identify an explicitly chosen copy.
- The seed is copied below `%TEMP%\3DPIceland-Automation\<ProfileId>`.
- The application accepts an automation profile only through the exact
  `.3dpiceland-disposable-profile.json` marker below that temporary root.
- The marker binds the exact application executable SHA-256.
- Database, preferences, output and evidence paths must remain below the same
  disposable root.
- Production, FTPS and application updates are blocked while the profile is
  active.
- SQLite/Excel restore and Material deletion are blocked in Stage 1.
- An unexpected owned-process dialog/window fails the run.
- UI input is sent only through Windows UI Automation to the launched process.
- Screenshots contain only the owned application or Verification window.
- Consistent before/after SQLite snapshots are retained as evidence.
- A canonical logical hash over schema, tables, columns and values must remain
  unchanged after controlled shutdown; WAL/header byte normalization is not
  treated as a logical data change.

Build:

```powershell
dotnet build App/FilamentDbApp.sln -c Debug
```

Run with an explicit non-live database copy:

```powershell
App\AutomationRunner\bin\Debug\net9.0-windows\3DPIcelandAutomationRunner.exe `
  --app App\FilamentDbApp\bin\Debug\net9.0-windows\3DPIcelandFilamentDB.exe `
  --seed-database C:\path\to\explicit\filamentdb-test-copy.sqlite
```

Evidence is retained below the disposable profile:

- `evidence\main-window.png`
- `evidence\verification-center.png`
- `evidence\verification.txt`
- `evidence\verification.json`
- `evidence\run-result.txt`
- `evidence\run-result.json`
- `evidence\database-before.sqlite`
- `evidence\database-after.sqlite`

The first scenarios verify exact-process startup, visible disposable identity,
stable tab/control navigation, Verification export and controlled shutdown.
Fast-grid cells remain outside Stage 1 because the custom drawn surface does
not yet expose row/cell automation peers.

## v44.7.15 Stage 2 accepted

The report scenario requires explicit `--scenario reports`. Its manifest
authorizes local report generation only, binds the Reports output box to the
disposable profile output folder and keeps Production, FTPS, updates, restore
and delete actions blocked.

```powershell
App\AutomationRunner\bin\Debug\net9.0-windows\3DPIcelandAutomationRunner.exe `
  --app App\FilamentDbApp\bin\Debug\net9.0-windows\3DPIcelandFilamentDB.exe `
  --seed-database C:\path\to\explicit\filamentdb-test-copy.sqlite `
  --scenario reports
```

Stage 2 invokes the canonical `Build Public Report Package` action and waits
for its existing six-report workflow. It validates catalog-owned safe routes,
non-empty HTML/PDF/JSON artifacts, canonical HTML markers, PDF headers and
SHA-256 values. Evidence adds:

- `evidence\report-package.png`
- `evidence\report-artifacts.json`
- `evidence\report-artifacts.txt`

Representative HTML/PDF paths are recorded as `MANUAL REVIEW REQUIRED`.
Automation never declares visual layout acceptance.

Disposable acceptance on 2026-07-25 passed Full Data Verification 341/341,
validated 211 catalog entries and retained hashes for 639 catalog/root
artifacts. Before/after logical SQLite hashes matched. PDF rendering exposed a
Material Summary final-page table clip; deterministic 20-row presentation
tables corrected it while retaining the same allowlisted rows, order, routes
and report ownership. Owner runtime accepted Full Data Verification 341/341,
the landscape PDF and responsive HTML behavior.

## v44.7.16 Stage 3 accepted

The CRUD scenario requires explicit `--scenario crud`. Its manifest authorizes
one exact generated disposable MaterialID; it does not release the general
Material deletion lock or permit owner-path selection.

```powershell
App\AutomationRunner\bin\Debug\net9.0-windows\3DPIcelandAutomationRunner.exe `
  --app App\FilamentDbApp\bin\Debug\net9.0-windows\3DPIcelandFilamentDB.exe `
  --seed-database C:\path\to\explicit\filamentdb-test-copy.sqlite `
  --scenario crud
```

Stage 3 uses a narrow in-process Automation contract because Fast-grid cells do
not yet expose stable automation peers. It exercises the canonical Materials
row model, computed fields and SQLite collection save:

- create and save one valid uniquely identified disposable record;
- restart, prove create persistence, edit and save;
- restart, prove edit persistence, delete only the exact authorized record;
- restart and prove the record is absent.

Consistent SQLite snapshots and full logical hashes are retained after create,
edit and delete. The final business-state hash excludes only columns named
exactly `UpdatedAtUtc`; full hashes remain visible. This records autosave
timestamp movement while requiring every row count and canonical business
value to return to its baseline. Unexpected dialogs, Production, FTPS, updates,
restore and owner paths remain blocked.

Disposable acceptance on 2026-07-25 passes Full Data Verification
342/342. The exact generated record `AUTce389f57` persisted through create and
edit restarts, was deleted through the authorized contract and was absent
after the final restart. Three per-action snapshots were retained and the
before/after business-state hashes match.
Owner runtime then accepted normal create/edit/delete persistence and cleanup;
owner Full Data Verification passes 342/342.

## v44.7.17 Stage 4 accepted

The backup/recovery scenario requires explicit `--scenario recovery`. It
creates and verifies a manual `.bak`, retains a verified legacy `.sqlite`
compatibility copy, exports a governed Excel recovery package, applies one
disposable mutation and restores the package. SQLite restore remains blocked
and is never automated.

The Excel restore now retains both pre-restore and post-restore SQLite evidence.
If post-restore evidence fails after commit, the verified pre-restore backup is
used for immediate rollback. The runner controls restart under the same exact
manifest and requires the final business-state hash to equal baseline.

Disposable acceptance passes Full Data Verification 343/343. The
governed workbook, both filename generations and pre/post restore artifacts
retain bytes and SHA-256 evidence; baseline/final business-state hashes match.
Owner runtime accepted backup discovery, Excel recovery, pre/post evidence and
Full Data Verification 343/343.

## v44.7.18 Stage 5 accepted

The guarded updater scenario requires explicit `--scenario updater` plus an
explicit `--updater` helper path. Its manifest keeps general updates,
Production and FTPS blocked while authorizing only the isolated Stage 5
transaction.

```powershell
App\AutomationRunner\bin\Debug\net9.0-windows\3DPIcelandAutomationRunner.exe `
  --app App\FilamentDbApp\bin\Debug\net9.0-windows\3DPIcelandFilamentDB.exe `
  --seed-database C:\Seed-Database\filamentdb.sqlite `
  --scenario updater `
  --updater App\FilamentDbUpdater\bin\Debug\net9.0\3DPIcelandUpdater.exe
```

The runner copies only build artifacts into a disposable portable directory.
It applies an identical staged build through the real updater helper, requires
an exact v44.7.18 health acknowledgement, then stages an intentionally invalid
disposable executable and requires complete rollback. The updater forwards the
same validated disposable profile both after install and after rollback.

Disposable evidence passes Full Data Verification 344/344. The success
transaction commits 54 governed files, the failure transaction reaches
`RolledBack`, all pre-update SHA-256 values are restored and the canonical
business-state hash remains equal to baseline. Owner runtime then accepted
normal startup, owner-data behavior and Full Data Verification 344/344.
# v45.1 disposable CRUD extension

The final unlinked-review addition is covered by Full Data Verification rather
than fragile grid input automation. Profile `20260725225702-22872c00` passes
345/345 and verifies that the counted Manufacturer filter represents null
`ManufacturerId` rows. The CRUD scenario also returns the database business
state to its exact baseline; owner, Production and FTPS paths remain blocked.
Owner runtime accepted the same-name selection and zero-count collapsed UI;
final owner Full Data Verification passes 345/345.

The accepted canonical tester seed is now schema v32 at
`C:\Seed-Database\filamentdb.sqlite`, SHA-256
`65BD03F668768F0AAEBF937BAFC628559A168EA1A07E586CECADC7431AF7BB84`.
The prior schema-v31 fixture remains at
`C:\Seed-Database\filamentdb-schema31-migration.sqlite`, SHA-256
`7851D26BA82E345E2C4B156996B68F0360B7F74A48B777752A609CB368EFD6D4`.
Profile `20260725233352-929a4f1a` passes CRUD, Full Data Verification 345/345
and exact baseline/final business-state hash equality on the schema-v32 seed.

## v45.2.0 Base Materials workspace extension

Every scenario now navigates `BaseMaterialsTab` by stable AutomationId.
Disposable CRUD additionally creates one uniquely named Base Material, restarts
and verifies it, edits it, creates a duplicate, restarts and verifies both,
then removes both and proves absence. This reuses the exact disposable profile
and business-state rollback boundary; owner paths, Production and FTPS remain
blocked. Fast-grid visual editing remains owner-manual because the owner-drawn
cells do not expose stable per-cell automation peers.

Profile `20260726094502-312196e2` passes the extended CRUD lifecycle and Full
Data Verification 346/346. The final business-state hash equals baseline
`5922D4E7B8AF2FB045C07F1F5F813E1C050C5F17B562003504FCBC15EDBDBDF0`,
and the canonical schema-v32 seed remains byte-identical.
Startup-fix profile `20260726095812-c88314b0` also passes immediate tab
navigation, CRUD, 346/346 and exact business-state cleanup.

The `crud` scenario remains confined to its exact generated disposable
MaterialID and a disposable Manufacturer catalog record. Create stores a
deliberate legacy/unmapped Manufacturer with null ID and restart preserves it
exactly. Edit creates and selects the catalog identity; restart proves
`ManufacturerId` plus canonical name persistence. The final stage proves
canonical rename propagation, referenced-delete blocking, rename restoration,
Material/catalog deletion and lossless sequence cleanup. The runner still does
not automate Fast-grid cells, owner data, Production or FTPS.

Disposable ManufacturerId acceptance run `20260725222727-42605144` passed Full
Data Verification 345/345. It preserved the deliberate unmapped Manufacturer
through create/restart, persisted the explicit catalog ID through edit/restart,
propagated a catalog rename, blocked referenced deletion and removed all
generated records. Final business-state hash matched baseline. The approved
source seed retained its original
4,825,088 bytes, timestamp and SHA-256
`7851D26BA82E345E2C4B156996B68F0360B7F74A48B777752A609CB368EFD6D4`.

## v45.2.1 canonical Base Material identity extension

The existing disposable CRUD scenario now starts with a deliberate unlinked
Base Material text value, creates the matching catalog row, explicitly stores
its `BaseMaterialId`, and proves the relationship after restart. It also proves
referenced-delete blocking, ID-owned rename propagation, rename restoration,
catalog cleanup and final business-state hash equality. Fast-grid visual input
remains owner-manual; Production, FTPS and owner paths remain blocked.

Candidate profile `20260726101917-5bc56749` passes Full Data Verification
347/347 and the complete disposable CRUD lifecycle. The final business-state
hash is `A199A91611C51D6074D147B16AB538B9A45485B1D4CB65E58C9EA5ECF54DC8CC`.

Dropdown-refresh profile `20260726104609-8be75c9a` re-passes 347/347 and exact
business-state cleanup while the CRUD rename uses the production edit handler
and proves the old choice disappears and the new choice is immediately present.

Owner final evidence `3DPIceland_FilamentDB_Verification_20260726_105027.txt`
passes 347/347 with zero unlinked Materials. The normalized schema-v33 seed at
`C:\Seed-Database\filamentdb.sqlite` has SHA-256
`50782D4E2DBE8F773E0A915E9E2460525B43FB68611E19DD6EB12F47B131AB31`.
Profile `20260726105424-55caa15d` passes CRUD, 347/347 and exact business-state
equality on that state. Schema-v32 is preserved as
`filamentdb-schema32-migration.sqlite`, SHA-256
`65BD03F668768F0AAEBF937BAFC628559A168EA1A07E586CECADC7431AF7BB84`.
Final canonical-path profile `20260726105756-ce0811c4` repeats the same PASS and
business-state hash `4FBCF6A2656678875A6692C0A7AA30CD0CDC3F4AAB83003B3BB2C77081B1C87D`.
