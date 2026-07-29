# 3DPIceland Engineering Platform

An independent, data-driven platform for testing, comparing and publishing engineering information about FDM/FFF filament materials.

Private Windows deployment supports a per-user Setup EXE and a portable ZIP built from the same production-signed governed package used by the transactional updater. Installer/uninstall never owns SQLite data, backups, configured storage or update evidence. Authenticode signing is deferred while distribution remains private, so Windows may show an Unknown publisher warning.

- **Current runtime-accepted release:** v59.0.7 — Canonical Navigation and Materials Layout
- **Live engineering database:** [iskort.is/3dp](https://iskort.is/3dp/)
- **Current development focus:** v59.0.8 — Documentation-only parent closure
- **Manufacturer enquiries and material submissions:** [iskort@iskort.is](mailto:iskort@iskort.is)

## Your data stays yours

> **3DPIceland does not collect telemetry, user accounts or the information you
> enter into the application. Your SQLite database, measurements, purchases,
> inventory, notes, settings and backups remain under your control and are not
> automatically uploaded or sent to 3DPIceland.**

Normal application use is local. Backups, reports, website previews and exports
stay on your computer or in a storage location you choose until you explicitly
move or publish them. Uninstalling the application does not claim or delete
your database.

Data can leave your computer only through an action you deliberately authorize.
The optional OpenAI pilot shows the exact allowlisted payload and requires
one-time consent before each live request. Guarded Production/FTPS publishing
also requires an explicit publishing workflow. Neither action runs silently,
and neither sends your private database to 3DPIceland.

## Download the Windows application

- [Download the latest Windows installer](https://www.iskort.is/3dp/downloads/3DPIceland-Setup-x64.exe)
- [Download the latest portable ZIP](https://www.iskort.is/3dp/downloads/3DPIceland-Portable-x64.zip)
- [Download the governed public demo database](https://www.iskort.is/3dp/downloads/3DPIceland-Public-Demo.zip)

The stable installer and portable links always point to the latest explicitly
published application release. Application publishing remains a separate,
guarded, default-No workflow.

## Try the public demo database

The optional demo contains 36 fictional material identities with real,
owner-approved comparative measurements. Pseudonymization is not anonymization:
distinctive measurement patterns may still support re-identification. The demo
is for evaluation and workflow exploration, not a replacement for manufacturer
datasheets or accredited testing.

To install it:

1. Download the [public demo ZIP](https://www.iskort.is/3dp/downloads/3DPIceland-Public-Demo.zip)
   and extract `3DPIceland-Public-Demo-v59.0.7.sqlite`.
2. In the application, open **File → Backup and Recovery Center…**.
3. Choose **Restore SQLite Backup** and select the extracted `.sqlite` file.
4. Confirm the exact file and the default-No restore prompt. The application
   verifies the database, saves the current profile as a pre-restore recovery
   backup, restores the demo and restarts.

To remove the demo and return to your own data:

1. Open **File → Backup and Recovery Center…** and choose **Refresh**.
2. Select the **Pre-SQLite restore recovery** row created when the demo was
   installed. Match its timestamp, full path and row counts carefully.
3. Choose **Verify Selected**, then **Restore Selected**.
4. Confirm the default-No prompt only for that exact backup. The application
   restores your previous data—or the original healthy empty profile for a new
   installation—and restarts.

Never overwrite or delete the active `filamentdb.sqlite` manually. **Choose
Storage Folder** moves the current canonical database; it is not a demo-data
switch.

## What this project is

3DPIceland began as a filament database and has grown into a Windows engineering platform for managing the complete path from material intake to verified public output.

The application brings material records, mechanical measurements, pricing, inventory, purchasing, experimental research, manufacturer intelligence and publishing workflows into one governed system. Its public website presents comparative results generated from the same verified data used by the desktop application.

The platform is intended for:

- engineers comparing real-world printed-material behaviour;
- makers choosing materials using measured evidence;
- manufacturers seeking independent technical exposure;
- researchers running controlled parameter studies.

## Platform capabilities

- Native material database and canonical `MaterialID` lifecycle
- Tensile strength and layer-adhesion analysis
- Impact resistance measurements
- Stiffness and deflection calculations
- Statistical consistency and confidence indicators
- Engineering scoring, rankings and recommendations
- Pricing and value comparison
- Purchasing and physical inventory workflows
- Experimental test series, runs and analytics
- Manufacturer profiles and engineering intelligence
- Canonical website preview and production export
- Engineering reports generated from canonical HTML
- Engineering Methodology Whitepaper and documentation portal
- Verification Center release gates

## Public website

The generated public engineering database is available at:

### [Open the 3DPIceland Engineering Platform](https://iskort.is/3dp/)

The website includes the filament database, interactive comparisons, experimental results, manufacturer intelligence and testing methodology. Website Preview is the canonical publication output; Production Export uses the same rendering and verified data path.

## Data and architecture principles

The project follows a few non-negotiable rules:

1. **SQLite is the single source of truth.**
2. **`MaterialID` is the canonical material identifier.**
3. **Website, reports, whitepapers and documentation do not independently recalculate engineering results.** They consume the Verified Material Summary.
4. **Verified Material Summary is the publication boundary** between native calculations and public/reporting outputs.
5. **Architecture grows by extension, not rewrites.** Existing workflows and backwards compatibility are preserved.
6. **Verification Center is the release gate.** A build is not considered ready solely because it compiles.

```text
SQLite material and measurement data
                │
                ▼
Native calculation services
                │
                ▼
Verified Material Summary
       ┌────────┼─────────┬────────────┐
       ▼        ▼         ▼            ▼
    Website   Reports  Whitepaper  Manufacturer intelligence
```

## Technology

- C# and WPF
- .NET 9 for Windows
- SQLite through `Microsoft.Data.Sqlite`
- ClosedXML for spreadsheet workflows
- WebView2 for canonical HTML preview and print-to-PDF workflows

## Build from source

Requirements:

- Windows
- [.NET 9 SDK](https://dotnet.microsoft.com/download/dotnet/9.0)
- Visual Studio with .NET desktop development support, or the .NET CLI

Open the solution:

[`App/FilamentDbApp.sln`](App/FilamentDbApp.sln)

Or build from the repository root:

```powershell
dotnet restore App/FilamentDbApp.sln
dotnet build App/FilamentDbApp.sln --configuration Release
```

The desktop application is Windows-specific because it targets `net9.0-windows` and uses WPF.

## Repository structure

| Path | Purpose |
| --- | --- |
| [`App/`](App/) | Visual Studio solution, application source, services, models and assets |
| [`Docs/`](Docs/) | Architecture, workflows, roadmap and canonical project documentation |
| [`Docs/CHANGELOG.md`](Docs/CHANGELOG.md) | Concise chronological release changes |
| [`Docs/BUILD_HISTORY.md`](Docs/BUILD_HISTORY.md) | Detailed build-by-build engineering history |
| [`Docs/MILESTONES.md`](Docs/MILESTONES.md) | Consolidated milestone history |
| [`Docs/Roadmaps/MASTER_ROADMAP.md`](Docs/Roadmaps/MASTER_ROADMAP.md) | Current and planned platform roadmap |
| [`Reports/VERIFICATION_HISTORY.md`](Reports/VERIFICATION_HISTORY.md) | Consolidated release verification and static-audit history |

## Current development focus

Version 58.0.6 is the current runtime-accepted application release. Full Data
Verification passes 414/414 with governed document logo/name, renderer,
migration, recovery and release-identity contracts aligned.

The v56.0.6.1 governed public-demo dataset is complete and owner accepted. Its
36 fictional identities retain approved real comparative measurements; the
local publish-ready package remains separate from the owner database and the
canonical tester seed. Application and corrected-demo publication remain
separate guarded default-No actions.

The coordinated public website is live with the independent Printing Price
Calculator, canonical Labs wordmark and favicon. `/3dp/price/` redirects to
`index.html#calculator`. Governed custom document logo and Brand / Organization
Name output is runtime accepted across reports, website documents and customer
quotes. The current roadmap increment is research-first v59.0 — Application
Navigation Finalization.

The clean-install profile contains no material dataset or private FTPS
identity. SQLite is never restored automatically by application update
recovery. Website and application publishing remain explicit,
credential-scoped and default-No.

## Measurement scope

Published values are comparative measurements from the documented 3DPIceland printed-specimen workflow. They are designed for consistent comparison inside this database and do not replace manufacturer datasheets, accredited laboratory testing or certified ISO/ASTM results. Methodology, sample count, orientation, spread and limitations should always be considered together with a published value.

## Contributing and contact

Issues, engineering feedback, manufacturer enquiries and material-submission questions are welcome at [iskort@iskort.is](mailto:iskort@iskort.is).

When changing calculations or publication logic, preserve the SQLite → native calculation services → Verified Material Summary boundary and add appropriate Verification Center coverage.

## License

The original 3DPIceland Engineering Platform source code is licensed under the
[GNU General Public License v3.0 only](LICENSE) (`GPL-3.0-only`). You may use,
study, modify and redistribute it under those terms. Distributed modified
versions must remain under GPLv3 and include the corresponding source code.

Third-party packages and components retain their own licenses. See
[THIRD-PARTY-NOTICES.md](THIRD-PARTY-NOTICES.md) for the dependency license
inventory used by the current application build.

Copyright © 2026 Marteinn Sigurðsson / 3DPIceland Labs.
