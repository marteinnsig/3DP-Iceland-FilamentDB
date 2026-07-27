# 3DPIceland Engineering Platform

An independent, data-driven platform for testing, comparing and publishing engineering information about FDM/FFF filament materials.

Private Windows deployment supports a per-user Setup EXE and a portable ZIP built from the same production-signed governed package used by the transactional updater. Installer/uninstall never owns SQLite data, backups, configured storage or update evidence. Authenticode signing is deferred while distribution remains private, so Windows may show an Unknown publisher warning.

- **Current runtime-accepted release:** v50.4.1 — Data and Configuration Control Help
- **Live engineering database:** [iskort.is/3dp](https://iskort.is/3dp/)
- **Current candidate:** None — v50.4.2 measurement, Experimental and analysis control/field Help is next
- **Manufacturer enquiries and material submissions:** [iskort@iskort.is](mailto:iskort@iskort.is)

## Download the Windows application

- [Download the latest Windows installer](https://www.iskort.is/3dp/downloads/3DPIceland-Setup-x64.exe)
- [Download the latest portable ZIP](https://www.iskort.is/3dp/downloads/3DPIceland-Portable-x64.zip)
- [Open the downloads page](https://www.iskort.is/3dp/downloads/)

The stable installer and portable links always point to the latest explicitly
published application release. Application publishing remains a separate,
guarded, default-No workflow.

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

Version 50.3.0 is the current canonical runtime-accepted release. It adds
guarded safety, recovery, Verification, Diagnostics, update/publishing and
troubleshooting Help with Full Data Verification 374/374.

The current roadmap increment is v50.4 — Exhaustive UI Control and Field Help
Audit and Final Acceptance. Parent v50 cannot close until every supported
control and editable field has explicit Help ownership and accepted coverage.

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
