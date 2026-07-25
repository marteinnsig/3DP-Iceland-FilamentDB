# Automated Runtime Acceptance

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
