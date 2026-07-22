## v40.3.1 – Experimental Deferred Run Refresh Fix

- Fixed intermittent `InvalidOperationException` at `MaterialExperimentsGrid_PreviewMouseLeftButtonDown`.
- Child-run view refresh is now conditional and deferred until the current WPF input/edit transaction is complete.
- Preserved v40.3.0 Test Series & Runs functionality and schema version 16.

## v40.3.0 – Experimental Test Series & Runs

## v40.3.0 – Experimental Test Series & Runs

This release changes the Experimental Testing Framework from one-record experiments into a parent/child test structure. A MaterialID-linked test series can now contain multiple controlled runs, for example 190, 200, 210, 220 and 230 °C for one temperature study.

## Build Summary

- Added SQLite-backed `ExperimentalRuns` with canonical `RunID` and parent `SeriesID`.
- Existing `MaterialExperiments` records now act as test-series definitions.
- Added a two-level Experimental Testing UI: series grid and runs grid.
- Added Add, Duplicate and Delete workflows for runs.
- Each run stores parameter value, unit, status, baseline flag, active flag and notes.
- Enforced one visible baseline selection per series in the UI.
- Deleting a series removes its child runs from both memory and SQLite.
- Series save preserves child runs across the existing replace-based persistence workflow.
- Added Verification Center gates for series/run integrity and orphan detection.
- Schema version increased to 16.

## What to Test

1. Build and start the application.
2. Open Experimental Testing and select an existing series or create a new one.
3. Set Material, Experiment type, default unit and series notes.
4. Add five runs and enter values such as 190, 200, 210, 220 and 230 with unit °C.
5. Mark one run as Baseline and confirm another baseline in the same series is cleared.
6. Edit Status, Active and Run Notes.
7. Test Duplicate Run and Delete Run.
8. Switch between two series and confirm each displays only its own runs.
9. Restart the application and confirm all series and runs persist.
10. Delete a temporary series and confirm its runs disappear.
11. Run Verification Center and confirm Overall Verification: PASS.
## v43.7.0 - Installer and Portable Deployment

Complete and runtime accepted. Debug/Release, production-signed six-file verification, clean-VM lifecycle, zero-data/privacy boundary, SQLite transfer, credential isolation, embedded branding, Verification Center, live FTPS application-release publishing and stable browser downloads pass.

## v43.6.0 - Update and Deployment Diagnostics

Complete and runtime accepted on 2026-07-22. The existing v1 updater contract now has read-only transaction history, startup detection and default-No external recovery for every incomplete durable phase. Visual Studio Debug confirmed v43.6.0 identity, zero incomplete transactions and Verification PASS 294/294. SQLite, website/report publishing and FTPS boundaries remain unchanged.
## v43.8.0 - Remote Signed Update Delivery

Complete and runtime accepted on 2026-07-22. Governed HTTPS discovery/download, production-signed package verification, Default-No apply and isolated FTPS feed publishing passed end-to-end VM testing. Interrupted snapshot/recovery handling was hardened after an induced Windows file-lock failure. Subsequent v43.8.5 -> v43.8.6 and v43.8.6 -> v43.8.7 updates committed with zero incomplete transactions. SQLite-native restore recovered the 200-Material schema-v29 owner dataset and final Verification Center passed 296/296. Canonical repository release identity is v43.8.0.

## v43.8.8 - Production Consolidation

Production identity above all published VM candidates. It carries the accepted v43.8 runtime behavior plus bounded chunked-download handling and rollback-capable update-feed publishing. Release packaging is pending final clean builds.
