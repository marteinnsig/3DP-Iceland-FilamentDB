> Canonical role: curated canonical and runtime-accepted release ledger.
> Detailed implementation chronology remains in `CHANGELOG.md`.

## v44.6.1 - Canonical Release Documentation Audit

- Canonical documentation-maintainability release.
- Reconciles v44.5.2-v44.6.0 across the canonical release documents.
- Adds a read-only duplicate, coverage and title-consistency audit.
- Baselines known historical collisions without rewriting release history.
- Debug/Release and documentation/security gates passed; runtime Full Data
  Verification passed 311/311.

## v44.6.0 - Recovery Center Clarity

- Replaced the persistent verbose updater-evidence box with concise Recovery
  Center guidance and selected-backup details.
- Preserved guarded restore and complete evidence in diagnostics.
- Runtime Full Data Verification passed 311/311.

## v44.5.9 - Supported Migration Naming

- Clarified canonical SQLite, supported JSON migration and built-in-default
  method ownership.
- Corrected partial stiffness calculation and close-time measurement persistence
  exposed by runtime testing.
- Runtime Full Data Verification passed 310/310.

## v44.5.8 - Retired Transition UI Residue

- Removed unreachable transition handlers and unused JSON save-state residue.
- Preserved supported migration readers and recovery boundaries.
- Runtime Full Data Verification passed 309/309.

## v44.5.7 - Legacy Workbook Schema Retirement

- Advanced SQLite to schema v30 and retired 13 original-workbook tables after a
  retained verified backup.
- Moved remaining engineering consumers to canonical measurements.
- Runtime Full Data Verification passed 308/308.

## v44.5.6 - Retired Workbook Metadata Readers

- Removed original-workbook metadata readers and display-only UI.
- Preserved governed Excel disaster recovery and compatibility inspection.
- Runtime Full Data Verification passed 307/307.

## v44.5.5 - Retired Legacy Write Entry Points

- Removed caller-free broad legacy replace/clear entry points.
- Preserved supported read compatibility and recovery.
- Runtime Full Data Verification passed 306/306.

## v44.5.4 - Measurement Help Clarity

- Removed duplicated measurement instructions without changing calculations or
  storage.
- Runtime Full Data Verification passed 305/305.

## v44.5.3 - Canonical Storage Terminology

- Aligned visible storage wording with canonical SQLite and retained supported
  JSON migration/recovery boundaries.
- Runtime Full Data Verification passed 304/304.

## v44.5.2 - Canonical SQLite UI Boundaries

- Removed misleading cache actions and retired `MaterialsImport` backup-first.
- Preserved supported migration and recovery paths.
- Runtime Full Data Verification passed 303/303.

## v44.5.1 - Active SQLite Compatibility Safety

- Inspects the active SQLite database read-only before startup migration.
- Retains an exact SHA-256-verified evidence copy instead of deleting an
  unsupported, newer, malformed or unreadable database.
- Stops startup without moving, replacing or restoring the active database.
- Keeps supported migration backups and updater application/SQLite boundaries
  unchanged.
- Runtime Full Data Verification passed 302/302 with zero failures.

## v44.5.0 - Retired Excel Import Surface

- Removes the unreachable original-Excel database import handler and its
  caller-exclusive importer services.
- Preserves governed Excel disaster recovery, lower-level SQLite compatibility
  data and JSON empty-database migration snapshots.
- Replaces stale instructions that pointed to the unavailable import command.
- Adds a Verification ownership gate for the retired/current Excel boundaries.
- Runtime Full Data Verification passed 301/301 with zero failures.

## v44.4.1 - Measured Materials Responsiveness

- Makes the accepted viewport-only Fast Materials view the startup default.
- Keeps all Materials visible in one comparison list while rendering only the
  visible cell viewport.
- Uses the existing canonical validation and SQLite auto-save workflow.
- Retains the native WPF DataGrid behind a checked Tools toggle as fallback.
- Full Data Verification passed 300/300; clean-VM direct install, explicit
  SQLite restore and portable runtime are accepted.
- Authenticode remains deferred; trusted ECDSA package verification is
  mandatory and Windows may display Unknown publisher.

## v44.3.1 - Backup, Recovery and Update Evidence Clarity

- Shows healthy schema-v29 empty-profile backups as explicitly restorable but
  not full-data release evidence.
- Explains every Recovery Center compatibility state.
- Shows the latest transaction, health acknowledgement, application rollback
  snapshot and SQLite backup evidence as separate read-only boundaries.
- Keeps application rollback separate from explicit/default-No SQLite recovery.
- Runtime accepted on a clean VM with Verification PASS 209/209 plus 90 N/A.

## v44.2.0 - Daily-use UI State and MaterialID Clarity

- Restores machine-local window geometry, keyed column widths/order and the last
  visible canonical MaterialID with invalid-state fallback.
- Keeps exactly one selected Materials row visibly light blue while preserving
  the accepted one-click text editor and cell-selection model.
- Checkbox values change only when the rendered box itself is clicked; blank
  checkbox-cell space selects the material without mutation.
- Runtime accepted with Full Data Verification PASS 298/298.
- A measured older first-horizontal-page delay remains scheduled for v44.4;
  startup-regressing and custom-scroll experiments were removed.

## v44.1.2 - Verification Profiles and Diagnostic Honesty

- Separates zero-data Application Readiness from Full Data Verification without
  weakening schema, privacy, installer, updater, recovery or release gates.
- A clean profile passes 207/207 applicable checks and reports 90 known
  data-dependent checks as not applicable; restored owner data passes 297/297.
- Explicit SQLite restore creates and verifies a retained post-restore evidence
  backup before restart. Pre-restore recovery evidence remains retained and
  SQLite is never restored automatically.
- Trusted ECDSA package signing remains canonical. Authenticode remains deferred,
  so Windows may display an Unknown publisher warning.

## v43.8.9 - SQLite Dependency Security

- Updates `Microsoft.Data.Sqlite` within the supported net9 line from 9.0.7 to 9.0.18.
- Explicitly selects `SQLitePCLRaw.bundle_e_sqlite3` 2.1.12 so restore cannot resolve the high-severity affected 2.1.10/2.1.11 native SQLite line.
- No SQLite schema, database ownership, backup/restore or automatic-restore policy change.
- Release promotion requires vulnerability scan, Debug/Release, updater/verifier, Verification Center and VM acceptance.
- Update-feed generation writes BOM-less UTF-8 for v43.8.8 compatibility; the v43.8.9 reader also tolerates one standard UTF-8 BOM defensively.
- Canonical runtime accepted: fresh v43.8.8-to-v43.8.9 guarded VM update committed with zero data leakage, restored schema-v29 owner data remained intact, and final Verification Center passed 296/296.

## v43.8.8 - Production Consolidation

- Canonical production identity above all v43.8 VM candidates.
- Remote signed-update delivery and interrupted-state recovery runtime accepted.
- SQLite-native restore, clean-profile isolation and Verification 296/296 accepted.
- Repository privacy audit completed before v44 open-platform planning.

## v43.7.0 - Installer and Portable Deployment

- Per-user/no-admin Setup EXE and exact governed portable ZIP.
- External SQLite/backups/configuration/evidence preserved across uninstall.
- Default-No application-release publishing to stable and versioned browser download routes.
- Authenticode deferred for private testing; clean-VM, Verification Center, live publish and browser-download runtime acceptance passed.

## v43.6.0 - Update and Deployment Diagnostics

- Read-only System Diagnostics history for durable application-update transactions.
- Default-No startup recovery using the existing external helper and last-known-good snapshot.
- Explicit coverage for Prepared, SnapshotReady, Installed, RollingBack and RollbackFailed.
- No automatic evidence cleanup or SQLite restore; no website/report/FTPS changes.
- Debug/Release builds and Visual Studio Debug runtime Verification passed 294/294.

## v40.5.9

Experimental Measurement Verification Gate.

## v40.5.2 – Experimental Input Visibility & Rebind Fix

- Replaced shared filtered measurement CollectionViews with fixed per-RunID editor row lists.
- Tensile and Impact Upright/Flat rows remain visible throughout editing and tab changes.
- Run selection now binds all four editors deterministically to the selected RunID.
- Deferred calculations capture the edited row's RunID and commit the DataGrid edit before calculation and persistence.
- Preserves the v40.5.0 specialized editors and native ResultsService calculation paths.


## v40.5.0 – Experimental Measurement Editor Redesign
- Replaced the generic experimental measurement grid with dedicated Tensile, Impact, Stiffness and Layer Adhesion tabs.
- Clarified Stiffness entry as Revolutions plus Degrees for one specimen.
- Retained 10-sample statistical capacity for tensile, impact and layer adhesion.
- Removed in-transaction DataGrid refresh that caused Impact entry crashes.
- Added Verification Center coverage for the redesign and edit-transaction safety.
## v40.4.1 – Experimental Native Measurement Context

- Replaced generic five-result entry with canonical raw measurement sessions.
- Added 10-sample Tensile Upright/Flat, Impact Upright/Flat and Layer Adhesion entry.
- Added one-specimen Stiffness entry using revolutions and degrees.
- Reused ResultsService and native Settings values for MPa, kJ/m², statistics, CV and confidence.
- Upgraded ExperimentalMeasurements storage to schema version 18.

## v40.4.0 – Experimental Measurement Entry

- Added canonical RunID-linked measurement storage for Tensile, Impact, Stiffness and Layer Adhesion.
- Added five-sample entry, automatic count and average, persistence and verification gates.
- Preserved separation from normal MaterialID measurement rows.

## v38.3.3 – INVENTORY-DELETE-FIX

- Fixed Inventory delete crash caused by WPF `NewItemPlaceholder` being cast to `InventorySpoolRecord`.
- Inventory deletion now persists immediately and refreshes material quantities and summary data.

## v38.3.2 – RECEIVING-AUTOMATION

- Combined missing-Material creation and received-spool generation into one receiving action.
- Active Purchase Order edits are committed before processing.
- Newly created Materials are linked and persisted before Inventory Spools are generated.
- Storage location is copied to physical Inventory Spools, not the shared Material definition.
- Repeated execution remains duplicate-safe.
- No schema or landed-cost changes.

Current development baseline: **v38.3.1 – PURCHASE-WORKFLOW-RECEIVING**


## v38.1.0
Purchasing & Inventory Foundation. Additive SQLite migration and native UI/persistence foundation; no cost engine calculations.

## v37.2.4.2 – UI-005B Build Identity Synchronization
- Fixed stale assembly informational metadata that caused the main header and splash screen to show v37.2.2.
- Centralized visible release identity through BuildInfo and synchronized assembly, file, product and documentation versions.
- Preserved all v37.2.4.1 non-blocking success feedback behavior.


## v37.1.5.2 – QOL-001A End-of-Text Caret Fix

- Fixed mouse caret placement in the blank area after text.
- Clicking to the right of the final character now places the caret at the true end of the value.
- Preserved precise in-text mouse editing and keyboard Select All behavior.

# v34.0 LTS-001 - Long-Term Stability Release

- Stability baseline for daily use.
- No new feature scope.
- Preserves Engineering, Website and Reporting workflows.
- Adds stability docs, known limitations, usage-mode plan and regression audit.
- Previous verification baseline: v33.5 PASS, 70 / 70.


## v33.5 - REPORT-500 AI Engineering Review

- Added Engineering Intelligence section to selected Material Detail and Material Engineering reports.
- Added automatic Engineering Summary, Best/Weakest Feature badges and engineering highlight.
- Added Recommended Applications based on verified engineering score profiles.
- Added Rankings and Percentiles table for overall/tensile/impact/stiffness/consistency/layer adhesion.
- Preserved canonical HTML -> WebView2 PDF workflow so HTML and PDF match.
- No raw measurement consumption or engineering calculation changes.


## v33.3.1 - REPORT-302A Chart Output Wiring Fix

- Fixed the REPORT-302 chart output wiring issue where verification and CSS existed but chart sections were not inserted into the exported canonical HTML body.
- Material Detail Report now writes the engineering chart suite into `report.html`.
- Material Engineering Report now writes the engineering chart suite into `report.html`.
- HTML and PDF continue to share the same canonical HTML through WebView2 PrintToPdf.
- No calculation, storage, import/export, or raw measurement access rules changed.

## v33.3 - REPORT-302 Interactive Engineering Charts

- Added canonical HTML engineering chart visuals for report exports.
- Added SVG radar chart for selected material vs material/manufacturer average context.
- Added material-vs-average and manufacturer-vs-average comparison tables.
- Charts render in the same HTML report package and therefore appear in PDF through WebView2 PrintToPdf.
- Added Verification Center gates for report chart renderer, engineering chart payload, chart asset validation, and interactive report readiness.
- Preserved SQLite/MaterialID/verified Material Summary source-of-truth rules.

## v33.2 - DOCS-002 Strategic Master Roadmap

- Rebuilt `Docs/Roadmaps/MASTER_ROADMAP.md` as a strategic roadmap instead of a changelog/build-history duplicate.
- Added permanent Documentation Governance Rule defining separate roles for `CHANGELOG.md`, `BUILD_HISTORY.md`, `MASTER_ROADMAP.md`, `PROJECT_STATUS.md`, and `ARCHITECTURE.md`.
- Added platform progress, completed strategic milestones, current Reporting Platform focus, next planned releases, long-term platform vision, and architecture evolution timeline.
- Added v33.3-v34.x near-term roadmap for interactive report charts, product image library, report themes, comparison reports, manufacturer reports, test session reports, and batch export.
- Preserved runtime behavior, report package workflow, HTML Print Engine, Engineering Platform rules, and raw measurement blocking.

## v33.1 - REPORT-301 Report Package Polish

- Added polished report package folder workflow on top of the HTML Print Engine.
- Export Report Package now writes a dedicated package folder with `report.html`, `report.txt`, `manifest.txt`, `report-metadata.json`, and `assets/`.
- Export PDF now writes the same package folder plus `report.pdf` printed from canonical HTML.
- Added report metadata JSON for report key, title, material counts, canonical HTML path, and optional PDF path.
- Added Verification Center gates for report package folder standard, report package metadata, and report package naming.
- HTML remains the canonical report layout; PDF visual parity path preserved.
- No engineering calculations or raw measurement access changed.

## v33.0.1 - REPORT-300A HTML Print Engine Build Fix

- Fixed WebView2 print settings API usage for the installed WebView2 SDK.
- Replaced unsupported `PaperWidth` / `PaperHeight` with compatible page size settings.
- Preserved HTML Print Engine architecture: PDF export remains printed from canonical HTML.
- No engineering calculations, storage, import/export data flow, or raw measurement rules changed.

## v33.0 - REPORT-300 HTML Print Engine

- Replaced the user-facing PDF export path with a WebView2 HTML print engine.
- Canonical HTML is now printed directly to PDF instead of manually recreating the layout in a separate PDF renderer.
- Report package continues to include PDF, companion HTML, and manifest.
- No engineering calculations, storage, import/export data workflows, or raw measurement access rules changed.

## v32.1 - REPORT-201 Report Asset Pipeline

- Added `ReportAssetPipelineService` for shared HTML/PDF report asset verification.
- Added Verification Center gates for report asset pipeline, report asset coverage, and report asset manifest readiness.
- Preserved unified report rendering: HTML remains canonical and PDF export is preserved.
- Preserved Engineering Platform calculation rules and raw measurement blocking.

# v32.0.3 - DOCS-001 Full Changelog Reconstruction

Current version: v41.6.0 INTERNAL-REPEATABILITY-CALIBRATION - Internal Repeatability Calibration

Status: documentation sync build. The active runtime platform remains v32 REPORT-200 unified report rendering.

Completed in this build:
- Rebuilt full project changelog.
- Mirrored changelog into build history.
- Preserved runtime behavior.

Next likely work:
- Continue report asset pipeline / product images / QR code support.
- Continue PDF fidelity improvements from canonical HTML output.


## v32.0.3 - DOCS-001 Full Changelog Reconstruction

- Updated the master roadmap so it no longer stops conceptually at v29.
- Documented completed v30 Reporting Platform foundation work.
- Documented completed v31 production report work.
- Documented completed v32 Unified Report Rendering Engine work.
- Added near-term roadmap for asset pipeline, PDF fidelity, and report package export.
- No calculation/storage/import/export behavior intentionally changed.

## v30.4 - REPORT-005 Native Report Templates

- Adds native report templates on top of verified report models.
- Adds report template Verification Center gates.
- Keeps all engineering calculations inside the Engineering Platform.

## v30.3 - REPORT-004 Certificate Generator

- Added `ReportCertificateGeneratorService` downstream of `ReportPdfRendererService`.
- Added certificate generator, certificate payload validation and certificate issue readiness verification gates.
- Preserved report model generation and PDF rendering behavior.
- Confirmed certificate generation consumes verified report models and PDF payload metadata only.
- No engineering calculations added.

## v30.2 - REPORT-003 Native PDF Renderer

- Added `ReportPdfRendererService` downstream of `ReportGeneratorService`.
- Added PDF renderer, payload validation, and render readiness verification gates.
- Preserved verified Material Summary as the only engineering data source for Reporting Platform.
- Preserved Website Platform behavior and existing report model generation.

# Releases

## v27.7.10 - Project Cleanup Validation

- Pre-v27.8 hygiene build.
- Removed legacy case-conflicting documentation files.
- No application behavior changed.


## v27.3.4 - Native Material Manager Safe Editing / Validation
- Added dirty-state tracking and unsaved-change warnings for Material Manager.
- Save Materials is disabled until edits are present.
- Added row validation column and required-field row highlighting.
- Added duplicate Material ID and duplicate Website Display Name validation.
- Save warns before writing if validation issues remain.

# Releases
Release inventory rebuilt from the 156-build history archive during v25.6.5.

## Classification
- **Feature:** adds user-visible functionality.
- **Build fix:** fixes compile/build errors.
- **Layout/UI fix:** fixes visible layout, sizing or wording problems.
- **Documentation:** updates docs without changing app behavior.
- **Foundation/scaffold:** starts a feature area but may not be the final functional version.

## Full inventory

| # | Build file | Classification |
|---:|---|---|
| 1 | `3DPIceland_FilamentDB_WPF_v01.zip` | Feature |
| 2 | `3DPIceland_FilamentDB_WPF_v01_fixed.zip` | Feature |
| 3 | `3DPIceland_FilamentDB_WPF_v02_all_material_columns.zip` | Feature |
| 4 | `3DPIceland_FilamentDB_WPF_v03_cache_fix_all_headers.zip` | Feature |
| 5 | `3DPIceland_FilamentDB_WPF_v04_variant_cache_refresh.zip` | Feature |
| 6 | `3DPIceland_FilamentDB_WPF_v05_variant_display_fix.zip` | Feature |
| 7 | `3DPIceland_FilamentDB_WPF_v06_material_details.zip` | Feature |
| 8 | `3DPIceland_FilamentDB_WPF_v07_ui_polish.zip` | Layout/UI fix or polish |
| 9 | `3DPIceland_FilamentDB_WPF_v08_professional_ui.zip` | Feature |
| 10 | `3DPIceland_FilamentDB_WPF_v09_code_cleanup.zip` | Layout/UI fix or polish |
| 11 | `3DPIceland_FilamentDB_WPF_v10_database_engine.zip` | Feature |
| 12 | `3DPIceland_FilamentDB_WPF_v10_1_database_engine_cache_fix.zip` | Feature |
| 13 | `3DPIceland_FilamentDB_WPF_v11_mechanical_framework.zip` | Feature |
| 14 | `3DPIceland_FilamentDB_WPF_v12_tensile_import.zip` | Feature |
| 15 | `3DPIceland_FilamentDB_WPF_v12_1_tensile_summary_fix.zip` | Feature |
| 16 | `3DPIceland_FilamentDB_WPF_v12_2_decimal_comma_fix.zip` | Feature |
| 17 | `3DPIceland_FilamentDB_WPF_v13_engineering_dashboard.zip` | Feature |
| 18 | `3DPIceland_FilamentDB_WPF_v14_material_identity_cards.zip` | Feature |
| 19 | `3DPIceland_FilamentDB_WPF_v14_1_dashboard_refinement.zip` | Feature |
| 20 | `3DPIceland_FilamentDB_WPF_v14_2_dashboard_cleanup.zip` | Layout/UI fix or polish |
| 21 | `3DPIceland_FilamentDB_WPF_v14_3_remove_duplicate_confidence.zip` | Feature |
| 22 | `3DPIceland_FilamentDB_WPF_v14_4_reliability_rating.zip` | Feature |
| 23 | `3DPIceland_FilamentDB_WPF_v15_scoring_engine.zip` | Feature |
| 24 | `3DPIceland_FilamentDB_WPF_v15_1_website_radar_alignment.zip` | Feature |
| 25 | `3DPIceland_FilamentDB_WPF_v16_material_comparison_workbench.zip` | Feature |
| 26 | `3DPIceland_FilamentDB_WPF_v16_1_comparison_table_polish.zip` | Layout/UI fix or polish |
| 27 | `3DPIceland_FilamentDB_WPF_v17_analytics_radar_framework.zip` | Feature |
| 28 | `3DPIceland_FilamentDB_WPF_v17_1_branding_logo.zip` | Feature |
| 29 | `3DPIceland_FilamentDB_WPF_v17_1_1_logo_sizing_fix.zip` | Layout/UI fix or polish |
| 30 | `3DPIceland_FilamentDB_WPF_v17_2_analytics_radar_engine.zip` | Feature |
| 31 | `3DPIceland_FilamentDB_WPF_v17_2_1_path_build_fix.zip` | Build fix |
| 32 | `3DPIceland_FilamentDB_WPF_v17_3_interactive_analytics.zip` | Feature |
| 33 | `3DPIceland_FilamentDB_WPF_v18_video_planner.zip` | Feature |
| 34 | `3DPIceland_FilamentDB_WPF_v18_1_data_aware_suggestions.zip` | Feature |
| 35 | `3DPIceland_FilamentDB_WPF_v18_2_automatic_comparison_discovery.zip` | Feature |
| 36 | `3DPIceland_FilamentDB_WPF_v18_2_1_duplicate_average_fix.zip` | Feature |
| 37 | `3DPIceland_FilamentDB_WPF_v18_2_2_video_planner_layout_fix.zip` | Layout/UI fix or polish |
| 38 | `3DPIceland_FilamentDB_WPF_v18_2_3_video_planner_visible_tables_fix.zip` | Layout/UI fix or polish |
| 39 | `3DPIceland_FilamentDB_WPF_v18_2_4_radar_legend_fix.zip` | Layout/UI fix or polish |
| 40 | `3DPIceland_FilamentDB_WPF_v18_3_video_title_angle_generator.zip` | Feature |
| 41 | `3DPIceland_FilamentDB_WPF_v19_0_recommendation_engine.zip` | Feature |
| 42 | `3DPIceland_FilamentDB_WPF_v19_1_context_aware_recommendations.zip` | Feature |
| 43 | `3DPIceland_FilamentDB_WPF_v19_2_recommendation_video_integration.zip` | Feature |
| 44 | `3DPIceland_FilamentDB_WPF_v19_2_recommendation_video_integration_real.zip` | Feature |
| 45 | `3DPIceland_FilamentDB_WPF_v19_2_1_recommendation_list_build_fix.zip` | Build fix |
| 46 | `3DPIceland_FilamentDB_WPF_v19_3_recommendation_prioritization.zip` | Feature |
| 47 | `3DPIceland_FilamentDB_WPF_v19_3_2_recommendation_cards.zip` | Feature |
| 48 | `3DPIceland_FilamentDB_WPF_v19_3_3_recommendation_detail_panel.zip` | Feature |
| 49 | `3DPIceland_FilamentDB_WPF_v19_3_4_title_polish.zip` | Layout/UI fix or polish |
| 50 | `3DPIceland_FilamentDB_WPF_v19_3_5_chatgpt_prompt_copy.zip` | Feature |
| 51 | `3DPIceland_FilamentDB_WPF_v19_4_recommendation_to_video_planner.zip` | Feature |
| 52 | `3DPIceland_FilamentDB_WPF_v19_4_1_video_idea_queue.zip` | Feature |
| 53 | `3DPIceland_FilamentDB_WPF_v19_5_0_video_idea_queue_management.zip` | Feature |
| 54 | `3DPIceland_FilamentDB_WPF_v19_5_1_copy_chatgpt_prompt.zip` | Feature |
| 55 | `3DPIceland_FilamentDB_WPF_v19_6_0_production_dashboard.zip` | Feature |
| 56 | `3DPIceland_FilamentDB_WPF_v19_6_0_production_dashboard_real.zip` | Feature |
| 57 | `3DPIceland_FilamentDB_WPF_v19_7_storage_queue_persistence.zip` | Feature |
| 58 | `3DPIceland_FilamentDB_WPF_v19_8_publish_planning_scaffold.zip` | Foundation/scaffold |
| 59 | `3DPIceland_FilamentDB_WPF_v19_8_0_publish_planning_fields.zip` | Feature |
| 60 | `3DPIceland_FilamentDB_WPF_v20_0_rankings_dashboard.zip` | Feature |
| 61 | `3DPIceland_FilamentDB_WPF_v20_0_workspace_layout_redesign.zip` | Feature |
| 62 | `3DPIceland_FilamentDB_WPF_v20_2_category_rankings.zip` | Feature |
| 63 | `3DPIceland_FilamentDB_WPF_v20_3_awards_winners.zip` | Feature |
| 64 | `3DPIceland_FilamentDB_WPF_v20_4_filtered_rankings.zip` | Feature |
| 65 | `3DPIceland_FilamentDB_WPF_v20_5_ranking_polish_validation.zip` | Layout/UI fix or polish |
| 66 | `3DPIceland_FilamentDB_WPF_v21_0_website_export_preview.zip` | Feature |
| 67 | `3DPIceland_FilamentDB_WPF_v21_0_website_export_preview_buildfix.zip` | Build fix |
| 68 | `3DPIceland_FilamentDB_WPF_v21_0_website_export_preview_tabfix.zip` | Layout/UI fix or polish |
| 69 | `3DPIceland_FilamentDB_WPF_v21_1_website_template_export.zip` | Feature |
| 70 | `3DPIceland_FilamentDB_WPF_v21_2_external_html_template_selection.zip` | Feature |
| 71 | `3DPIceland_FilamentDB_WPF_v21_2_external_html_template_selection_buildfix.zip` | Build fix |
| 72 | `3DPIceland_FilamentDB_WPF_v21_3_safe_publish_workflow.zip` | Feature |
| 73 | `3DPIceland_FilamentDB_WPF_v21_3_safe_publish_workflow_pathfix.zip` | Layout/UI fix or polish |
| 74 | `3DPIceland_FilamentDB_WPF_v21_4_manufacturer_export.zip` | Feature |
| 75 | `3DPIceland_FilamentDB_WPF_v21_4_manufacturer_export_buildfix.zip` | Build fix |
| 76 | `3DPIceland_FilamentDB_WPF_v21_4_manufacturer_export_templatefix.zip` | Layout/UI fix or polish |
| 77 | `3DPIceland_FilamentDB_WPF_v21_5_export_validation_logging.zip` | Feature |
| 78 | `3DPIceland_FilamentDB_WPF_v21_5_export_validation_logging_log_layout_fix2.zip` | Layout/UI fix or polish |
| 79 | `3DPIceland_FilamentDB_WPF_v21_5_export_validation_logging_log_layout_patch.zip` | Layout/UI fix or polish |
| 80 | `3DPIceland_FilamentDB_WPF_v21_5_export_validation_logging_wording_patch.zip` | Layout/UI fix or polish |
| 81 | `3DPIceland_FilamentDB_WPF_v22_0_report_export_foundation.zip` | Foundation/scaffold |
| 82 | `3DPIceland_FilamentDB_WPF_v22_0_report_export_foundation_buildfix.zip` | Build fix |
| 83 | `3DPIceland_FilamentDB_WPF_v22_1_report_preview_templates.zip` | Feature |
| 84 | `3DPIceland_FilamentDB_WPF_v22_1_report_preview_templates_buildfix.zip` | Build fix |
| 85 | `3DPIceland_FilamentDB_WPF_v22_2_material_report_generator.zip` | Feature |
| 86 | `3DPIceland_FilamentDB_WPF_v22_3_material_report_layout.zip` | Feature |
| 87 | `3DPIceland_FilamentDB_WPF_v22_3_1_remove_preview_block.zip` | Feature |
| 88 | `3DPIceland_FilamentDB_WPF_v22_4_report_type_layouts.zip` | Feature |
| 89 | `3DPIceland_FilamentDB_WPF_v22_5_native_pdf_export.zip` | Feature |
| 90 | `3DPIceland_FilamentDB_WPF_v22_6_real_pdf_generation.zip` | Feature |
| 91 | `3DPIceland_FilamentDB_WPF_v22_7_rich_pdf_report_content.zip` | Feature |
| 92 | `3DPIceland_FilamentDB_WPF_v23_0_chart_foundation.zip` | Foundation/scaffold |
| 93 | `3DPIceland_FilamentDB_WPF_v23_0_1_html_chart_export_fix.zip` | Feature |
| 94 | `3DPIceland_FilamentDB_WPF_v23_0_2_pdf_vector_charts.zip` | Feature |
| 95 | `3DPIceland_FilamentDB_WPF_v23_1_material_detail_reports.zip` | Feature |
| 96 | `3DPIceland_FilamentDB_WPF_v23_1_1_selected_material_reports.zip` | Feature |
| 97 | `3DPIceland_FilamentDB_WPF_v23_1_2_material_report_scope_selector_real.zip` | Feature |
| 98 | `3DPIceland_FilamentDB_WPF_v23_1_3_global_report_scope_selector.zip` | Feature |
| 99 | `3DPIceland_FilamentDB_WPF_v23_1_4_selected_material_report_polish.zip` | Layout/UI fix or polish |
| 100 | `3DPIceland_FilamentDB_WPF_v23_1_4_selected_material_report_polish_buildfix.zip` | Build fix |
| 101 | `3DPIceland_FilamentDB_WPF_v23_2_rankings_analytics.zip` | Feature |
| 102 | `3DPIceland_FilamentDB_WPF_v23_2_rankings_analytics_buildfix.zip` | Build fix |
| 103 | `3DPIceland_FilamentDB_WPF_v23_2_rankings_analytics_buildfix2.zip` | Build fix |
| 104 | `3DPIceland_FilamentDB_WPF_v23_3_awards_analytics.zip` | Feature |
| 105 | `3DPIceland_FilamentDB_WPF_v23_3_awards_analytics_buildfix.zip` | Build fix |
| 106 | `3DPIceland_FilamentDB_WPF_v23_3_awards_analytics_buildfix2.zip` | Build fix |
| 107 | `3DPIceland_FilamentDB_WPF_v23_3_1_awards_analytics_functional_fix.zip` | Feature |
| 108 | `3DPIceland_FilamentDB_WPF_v23_3_1_awards_analytics_functional_fix_buildfix.zip` | Build fix |
| 109 | `3DPIceland_FilamentDB_WPF_v23_3_2_real_award_strength_tables.zip` | Feature |
| 110 | `3DPIceland_FilamentDB_WPF_v23_3_3_award_analytics_aggregation.zip` | Feature |
| 111 | `3DPIceland_FilamentDB_WPF_v23_3_4_runner_up_aggregation_fix.zip` | Feature |
| 112 | `3DPIceland_FilamentDB_WPF_v23_4_visual_analytics.zip` | Feature |
| 113 | `3DPIceland_FilamentDB_WPF_v23_4_1_pdf_visual_analytics_fix.zip` | Feature |
| 114 | `3DPIceland_FilamentDB_WPF_v24_0_dashboard_insights_foundation.zip` | Foundation/scaffold |
| 115 | `3DPIceland_FilamentDB_WPF_v24_0_1_dashboard_metrics.zip` | Feature |
| 116 | `3DPIceland_FilamentDB_WPF_v24_0_2_dashboard_layout_leaders.zip` | Feature |
| 117 | `3DPIceland_FilamentDB_WPF_v24_0_3_dashboard_stretch_layout_fix.zip` | Layout/UI fix or polish |
| 118 | `3DPIceland_FilamentDB_WPF_v24_0_4_dashboard_textbox_layout_fix.zip` | Layout/UI fix or polish |
| 119 | `3DPIceland_FilamentDB_WPF_v24_0_5_dashboard_textblock_layout_fix.zip` | Layout/UI fix or polish |
| 120 | `3DPIceland_FilamentDB_WPF_v24_1_hidden_gem_detection.zip` | Feature |
| 121 | `3DPIceland_FilamentDB_WPF_v24_1_hidden_gem_detection_buildfix.zip` | Build fix |
| 122 | `3DPIceland_FilamentDB_WPF_v24_2_opportunity_discovery.zip` | Feature |
| 123 | `3DPIceland_FilamentDB_WPF_v24_2_1_real_opportunity_discovery.zip` | Feature |
| 124 | `3DPIceland_FilamentDB_WPF_v24_2_1_real_opportunity_discovery_buildfix.zip` | Build fix |
| 125 | `3DPIceland_FilamentDB_WPF_v24_2_3_youtube_opportunity_scoring.zip` | Feature |
| 126 | `3DPIceland_FilamentDB_WPF_v24_2_4_code_cleanup_dashboard_optimization.zip` | Layout/UI fix or polish |
| 127 | `3DPIceland_FilamentDB_WPF_v24_2_4_code_cleanup_dashboard_optimization_buildfix.zip` | Build fix |
| 128 | `3DPIceland_FilamentDB_WPF_v24_3_competitive_insights.zip` | Feature |
| 129 | `3DPIceland_FilamentDB_WPF_v24_3_1_real_competitive_insights(1).zip` | Feature |
| 130 | `3DPIceland_FilamentDB_WPF_v24_4_creator_insights.zip` | Feature |
| 131 | `3DPIceland_FilamentDB_WPF_v24_4_1_real_creator_insights.zip` | Feature |
| 132 | `3DPIceland_FilamentDB_WPF_v24_4_2_creator_roadmap.zip` | Feature |
| 133 | `3DPIceland_FilamentDB_WPF_v25_0_youtube_research_engine_foundation.zip` | Foundation/scaffold |
| 134 | `3DPIceland_FilamentDB_WPF_v25_1_advanced_title_engine.zip` | Feature |
| 135 | `3DPIceland_FilamentDB_WPF_v25_1_real_advanced_title_engine.zip` | Feature |
| 136 | `3DPIceland_FilamentDB_WPF_v25_1_real_advanced_title_engine_buildfix.zip` | Build fix |
| 137 | `3DPIceland_FilamentDB_WPF_v25_2_thumbnail_research_engine.zip` | Feature |
| 138 | `3DPIceland_FilamentDB_WPF_v25_2_thumbnail_research_engine_layoutfix.zip` | Layout/UI fix or polish |
| 139 | `3DPIceland_FilamentDB_WPF_v25_2_thumbnail_research_engine_layoutfix2.zip` | Layout/UI fix or polish |
| 140 | `3DPIceland_FilamentDB_WPF_v25_2_thumbnail_research_engine_preview_panel_polish.zip` | Layout/UI fix or polish |
| 141 | `3DPIceland_FilamentDB_WPF_v25_2_thumbnail_research_engine_refined.zip` | Feature |
| 142 | `3DPIceland_FilamentDB_WPF_v25_2_1_thumbnail_research_engine_recommendation_reason.zip` | Feature |
| 143 | `3DPIceland_FilamentDB_WPF_v25_3_comparison_discovery.zip` | Feature |
| 144 | `3DPIceland_FilamentDB_WPF_v25_3_1_comparison_discovery_buildfix.zip` | Build fix |
| 145 | `3DPIceland_FilamentDB_WPF_v25_3_2_comparison_discovery_panel_polish.zip` | Layout/UI fix or polish |
| 146 | `3DPIceland_FilamentDB_WPF_v25_3_3_comparison_discovery_layout_cleanup.zip` | Layout/UI fix or polish |
| 147 | `3DPIceland_FilamentDB_WPF_v25_4_content_calendar_planner.zip` | Feature |
| 148 | `3DPIceland_FilamentDB_WPF_v25_5_channel_gap_analysis.zip` | Feature |
| 149 | `3DPIceland_FilamentDB_WPF_v25_5_1_youtube_research_scrolling_fix.zip` | Feature |
| 150 | `3DPIceland_FilamentDB_WPF_v25_6_playlist_discovery.zip` | Feature |
| 151 | `3DPIceland_FilamentDB_WPF_v25_6_playlist_discovery_actual.zip` | Feature |
| 152 | `3DPIceland_FilamentDB_WPF_v25_6_1_playlist_discovery_buildfix.zip` | Build fix |
| 153 | `3DPIceland_FilamentDB_WPF_v25_6_2_playlist_discovery_polish.zip` | Layout/UI fix or polish |
| 154 | `3DPIceland_FilamentDB_WPF_v25_6_3_documentation_refresh.zip` | Documentation |
| 155 | `3DPIceland_FilamentDB_WPF_v25_6_4_documentation_overhaul.zip` | Documentation |
| 156 | `v25_4_Content_Calendar_Planner_Implementation_Plan.zip` | Feature |
| 159 | `3DPIceland_FilamentDB_WPF_v27_3_3_Native_Material_Manager_Archive_Export_State.zip` | Feature polish |


## v32.0.3 - DOCS-001 Full Changelog Reconstruction
- Fixed native PDF layout overlap by reserving header/card/body zones.
- Rebuilt report logo and application icon with white backgrounds for visibility.
- Preserved verified Material Summary sourcing and blocked raw measurement consumption.
- ZIP packaging standard preserved.


## v34.1 - USAGE-001 Daily Use Readiness Pack

- Daily-use baseline after v34.0 LTS.
- Added daily workflow checklist.
- Added bug/feedback capture log.
- Added usage baseline documentation.
- Added regression audit for usage-mode readiness.
- No engineering calculations changed.
- No website/report workflow changes intended.

## v34.2 - USAGE-002 Real-World Feedback Loop

Usage-mode support release. Adds the feedback loop and review cadence used while the app is operated as the daily production tool.


## v34.2.1 - RELEASE-001 Shared Database Working Copy Fix

- Fixed Release vs Visual Studio edit mismatch caused by executable-folder JSON working copies.
- Native working-copy files now live beside the configured SQLite database in the storage folder.
- Added diagnostics visibility for executable folder and native working-copy paths.
- No calculation or website/report export workflow changes.

## v34.2.3 - RELEASE-002A Path Diagnostics Build Fix

- Added final release acceptance diagnostics for database path, executable path, storage folder and native working-copy paths.
- Added Verification Center checks confirming shared storage-folder working-copy readiness.
- Added Release Acceptance Diagnostics section to exported verification reports.
- Documented single-instance editing rule for daily Release use.
- No calculation, import/export or report workflow changes.


## v34.3.0 - BRAND-001 App Identity & Animated Splash Screen

- Adopted the new minimalist 3D printer / hidden “3” symbol as the permanent Windows application icon.
- Added a production multi-resolution ICO for Windows shell, taskbar, shortcuts and EXE branding.
- Added a WPF startup splash screen with animated blue extrusion trace and startup status text.
- Updated the application header to use the compact new symbol.
- Preserved the full 3DPIceland Labs wordmark for reports and document branding.
- No database schema, calculation, website-export or report-rendering logic changed.


## v37.2.5 - WORKFLOW-004 DataGrid Column Layout Completion
- Completed persistent DataGrid column-width handling for the five daily workflow grids.
- Saved widths are now keyed to stable bound fields instead of fragile column positions.
- Preserved backward compatibility with earlier v37 workflow preference files.
- Retained smart first-run widths and the proven v37.0.7 editing workflow.

## v37.1.2 - DATA-ENTRY-002 Material Detail Workflow Layout
- Reorganized Material Detail General fields into workflow-oriented groups.
- Added predictable field order within each group.
- Kept unknown imported fields visible under Other.
- No database, calculation, report or website export logic changes.

## v37.1.3 - DATA-ENTRY-003 Clear Filter Language
- Replaced generic Material Manager filter defaults with descriptive per-filter text.
- Added consistent labels, widths and tooltips.
- Preserved existing filtering, measurement synchronization and responsive search behavior.

## v37.1.4 - DATA-ENTRY-004 Compact Detail & Test Notes Flow
- Compact workflow-oriented Material Detail presentation.
- Test Notes repositioned before computed measurement results.
## v37.1.5.1 – QOL-001 Mouse Editing Fix
- Restored precise mouse caret placement in editable DataGrid text fields.
- Preserved Select All for keyboard navigation and deliberate focus actions.
## v37.2.0.1 – UI-001A Tab Content Stretch Fix
- Fixed vertically centered main-tab pages introduced by v37.2.0.
- Main workspace content now fills the available page area consistently.


## v37.2.4.1 – UI-005A Non-Blocking Success Feedback
- Replaced frequent success MessageBox dialogs with three-second status feedback.
- Preserved confirmation, warning and error dialogs.
- Removed internal storage terminology from normal user-facing success messages.
