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
