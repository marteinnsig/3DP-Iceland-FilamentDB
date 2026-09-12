# v66 Flexible Evidence Integration Inventory

Research baseline: 2026-09-11. This is a delivery inventory, not an implementation or acceptance claim.
`Docs/Roadmaps/MASTER_ROADMAP.md` owns the sequence and completion conditions.
Source paths below are relative to `App/FilamentDbApp` unless another root is stated.

## Scope and owner decision

The owner requested Flexible Material Testing results throughout Material Details, reports/PDF, rankings,
categories, awards, YouTube Research and other measurement/analysis consumers.
The recorded owner decision is **separate Flexible comparisons/categories; legacy Overall remains unchanged**.
No flexible metric is silently converted into a rigid engineering score or added to the existing score radar.
Compression force and Shore hardness are not universal better/worse measures. Any directional ranking must
name its purpose, native unit and exact comparable conditions. Leader/award claims need two comparable materials.

| Increment | State | Delivery boundary |
|---|---|---|
| v66.0.3 | Owner accepted | Canonical flexible evidence, Recovery in Comparable Results, read-only Material Details |
| v66.0.4 | Owner accepted | Internal/public-safe reports and HTML/TXT/PDF exports; no live publication |
| v66.0.5 | Owner accepted | Condition-matched Flexible rankings, categories, awards and corresponding exports |
| v66.0.6 | Owner accepted | Research, Video Planner, recommendations, scoped local AI and explicit OpenAI preview |
| v66.0.7 | Owner accepted | Comparison workbench, analytics, dashboard charts and remaining consumer parity |
| v66.0.8 | Owner accepted; complete | Inventory reconciliation, tester/Help, visual acceptance, release documentation and closure |

## v66.0.3: canonical projection and Material Details

- `MainWindow.FlexibleTesting.cs`: `SaveFlexibleTesting`, `RefreshFlexibleComparisons` and calculated-cell refresh.
- `Data/LocalDatabase.FlexibleTesting.cs`: read the persisted graph; retain transactional save ownership.
- `Models/FlexibleMaterialTestRecords.cs` and `Services/FlexibleMaterialTestingService.cs`: raw records,
  recalculation and comparable statistics are the starting contract.
- `MainWindow.xaml.cs`: selected-material detail rendering, `RenderEngineeringDashboard`, `RenderChartScores`.
- `Services/MaterialDetailService.cs`: grouped detail fields and their existing read-only ownership.

Project by exact MaterialID and active session, preserving method, specimen, cycle, dimensions, print conditions,
strain, hold/read/rest times and Shore scale where relevant. Pool only identical comparison keys. Count independent
specimens rather than repeated readings; preserve absent values as absent. Keep the raw graph authoritative.
Recovery was absent from the baseline `BuildComparisons` call and is explicitly included in v66.0.3 scope.
Do not rename residual height loss as standardized Compression Set or claim ASTM/ISO compliance.

At the research baseline, `SaveFlexibleTesting` refreshed only local comparisons, saved-relaxation visibility and
material status. Downstream invalidation must follow a successful save; failed saves must not expose unsaved results.
Use coalesced refresh ownership such as `QueueNativeMaterialDependentIntelligenceRefresh`, plus selected details
and comparison as needed. Refresh must not rebind the active flexible editor or steal keyboard focus.

## v66.0.4: reports and export consumers

Implemented: saved-scope HTML/text augmentation, six closed public DTO/publisher families, typed report pipeline,
source freshness, complete fallback PDF continuation and certificate identity. Owner approved public template inclusion
on 2026-09-12; local builds do not authorize live publication. Synthetic visual review passes; owner review pending.

- `MainWindow.xaml.cs`: `BuildMaterialEngineeringReportHtml`, `BuildSelectedMaterialEngineeringReportHtml`,
  `BuildMaterialSummaryReportHtml`, `BuildMaterialReportHtml`, `BuildSelectedMaterialReportHtml`.
- Comparison/manufacturer/session paths: `BuildComparisonReportHtml`, `BuildComparisonReportMaterials`,
  `BuildManufacturerReportHtml`, `BuildManufacturerEngineeringIntelligence`, `BuildTestSessionReportHtml`.
- Supporting evidence: `BuildEngineeringChartSuiteHtml`, `BuildEngineeringIntelligenceHtml`,
  `BuildEngineeringReportHandoff`, corresponding report-preview text and PDF render paths.
- `Services/Reporting/MaterialEngineeringReportService.cs`, `PublicEngineeringReportPackageService.cs`,
  `PublicComparisonReportPublishingService.cs` and affected sibling publishers.
- Reporting pipeline: `ReportingDataPipelineService.BuildPayload`, `ReportingMaterialSummaryRow`,
  `ReportGeneratorService.BuildSections`, `ReportTemplateService`, `ReportPdfRendererService` and
  `UnifiedReportRenderingService`, including fallback rendering paths.
- `Models/PublicReportPublicationModels.cs` and `Services/Reporting/PublicReportSourceFingerprintService.cs`.
- `Services/Reporting/ReportExportUiService.cs`: preserve the established local export destination workflow.

Audit report variants together so HTML, text, PDF, public manifests and freshness detection agree. Add typed,
public-safe evidence fields rather than serializing the flexible database graph. Public/customer output must exclude
private notes, internal evidence identifiers and printer identity. Keep necessary internal traceability separately.
Reconcile all six public report DTO/publisher families. Preserve narrow `BuildPublicMethodV1Summary` compatibility;
introduce a separate general evidence DTO where needed. Raw `MethodKey` can contain private free-form content and
must not be copied into the public allowlist; project only approved numeric conditions and safe method identity.
Include the flexible source state in source fingerprints so edits/deletes invalidate stale generated output.
Preserve saved artifacts and their accepted bytes; regenerate only through the existing explicit generation path.
Extending an exporter does not authorize website, FTPS or automatic report publication.

## v66.0.5: rankings, categories and awards

- `MainWindow.xaml.cs`: `BuildRankingRow`, `UpdateRankings`, `ExportRankingsCsv_Click`.
- `CategoryMetricDefinitions`, `UpdateCategoryRankings`, `ExportCategoryRankingsCsv_Click`.
- `AwardDefinitions`, `UpdateAwards`, `BuildAwardReason`, `ExportAwardsCsv_Click`.
- `AppendRankingsReportPreview`, `AppendAwardsReportPreview`, `BuildRankingsReportHtml`, `BuildAwardsReportHtml`.
- `Services/EngineeringScoringService.cs`, `EngineeringPeerPositionService.cs` and related value/consistency
  consumers: assess for explicit separation; do not change legacy Overall as an integration shortcut.

Use separate Flexible selections and condition keys, disclose independent specimen count, and exclude invalid or
incompatible evidence. Do not select one arbitrary session/result per material. Cross-material/manufacturer averages
must not mix method conditions or treat additional repeated readings as additional independent specimens.

## v66.0.6: research, recommendations and AI

Implemented: pure opportunity/peer projection, dataset research/planner/guidance panels, explicit copy and snapshot save,
compact dashboard coverage, processed-scope local briefs, YouTube report narratives and safe OpenAI preview fields.
Saved global queue survives detail reset; old snapshots are preserved. 455/455 and synthetic visual review pass; owner review pending.

`MainWindow.xaml.cs` is the principal consumer host:

- `VideoPlannerRow`, `BuildVideoPlannerRow`, `BuildCanonicalVisiblePlannerRows`, `UpdateVideoPlanner`.
- `UpdateYouTubeResearch`, `ApplyDataAwareVideoSuggestions`, `ApplyVideoTitleSuggestions`,
  `BuildMaterialTalkingPoints`, `FindStrongestOutlier`, `BuildAutomaticComparisonSuggestions`.
- `BuildComparisonDiscoveryRows`, `BiggestMetricGap`, `BuildChannelGapRows`, `BuildContentCalendarRows`,
  playlist builders, `ApplyAdvancedTitleEngine`, `ApplyThumbnailResearchEngine` and their copy/export actions.
- `BuildRecommendationRows`, `ApplyEngineeringAdvisor`, context/use-case recommendation rules,
  `UpdateSelectedMaterialIntelligence`, `BuildYouTubeReportHtml` and selected video notes.
- `BuildAiAssistantBrief`, `BuildMaterialIntelligenceBrief`, `BuildAiMaterialInsight`,
  `BuildCollectionDashboardBrief`, `BuildVideoPipelineDashboardBrief`.
- `BuildOpenAiPilotPreview`; `Services/OpenAiAssistantPilotService.cs` owns the typed request allowlist,
  payload version, normalization, request hash, instruction limitations and bounded material scope.
- `Services/EngineeringAdvisorService.cs` and `EngineeringIntelligenceHandoffService.cs` own advisor explanations
  and handoffs. The baseline advisor is explicitly five-axis; flexible facts need distinct evidence handling.

Flexible-only materials currently fail the planner's mechanical-readiness filter. Add explicit measured flexible
readiness and facts instead of fabricating an Overall score. Comparisons, titles and thumbnails must carry the same
condition/limitation rules as the canonical evidence. Bulk-load the projection rather than querying each material.

`Models/VideoIdeaRecord.cs`, `VideoPlannerRowToRecord`, `VideoPlannerRowFromRecord` and saved idea prompts need
compatible evidence round-tripping. Preserve saved titles, notes and generation-time evidence. Saved AI session text
must not change retrospectively. Collections may display current scoped evidence only where already defined as live.

The OpenAI path remains preview-first, explicitly sent and hash-bound. Add only allowed numeric/condition facts;
do not include private notes, credentials, filesystem paths, customer or purchasing data. Keep no-tools and advisory
boundaries. No new automatic network request is authorized. YouTube Research remains local and does not publish.

## v66.0.7: comparison, analytics and remaining parity

- `MainWindow.xaml.cs`: `BuildComparisonSnapshot`, `BuildComparisonMetricRows`, `BuildComparisonScoreRows`,
  `BuildComparisonCell`, `UpdateComparisonWorkbench` and comparison headers.
- `BuildAnalyticsMaterialScore`, `BuildGroupedAnalyticsRows`, `RenderAnalyticsRadar`, `DrawRadarProfile`.
- `UpdateDashboardInsights`, selected-material dashboard/chart rendering and manufacturer analytics/handoffs.
- `Services/Website/WebsiteChartGeneratorService.cs` and downstream chart/radar consumers: assess affected
  generated evidence during report parity; preserve publication boundaries and the existing rigid score contract.

Display native-unit flexible evidence separately. Do not place raw N, Shore or percentage values on existing
normalized engineering radar axes. Adapt missing-data/coverage messages for flexible-only materials and keep
different condition keys visibly separate. Comparison highlighting needs explicit metric direction and eligibility.

## Acceptance, Help and closure

Every increment must assess `HelpContentCatalog`, Help registry/ledger and drift checks for labels, read-only scope,
units, defaults, conditions, save timing and limitations. Existing AutomationId ownership must remain stable;
new important controls need deterministic ownership. Read-only presentation does not justify new editing handlers.

Extend existing disposable CRUD/reports/smoke scenarios where their authorization already owns the behavior.
Test raw-to-projection parity, repeated-reading independence, inactive/invalid exclusion, incompatible condition
separation, missing-versus-zero, successful-save refresh, deletion invalidation and unchanged legacy Overall.
Reports need typed allowlist/fingerprint checks and HTML/PDF parity. Research needs flexible-only candidates,
matched comparisons, saved snapshot round-trip and explicit OpenAI preview/hash tests without live sending.

Keep Production, FTPS, owner-database and owned-process guards. No new schema or seed refresh is implied by
a read-only projection; reassess only if implementation creates a concrete requirement.
Automated checks cannot replace owner layout, wrapping, interpretation or keyboard/focus acceptance.
Build Debug and Release, run applicable Help/docs/security gates and Full Data Verification; retain governed evidence.

v66.0.8 must reconcile every inventory entry as implemented/accepted or explicitly owner-deferred. Update README,
feedback and release evidence without claiming unaccepted work canonical. The separate v66.0.2 Settings runtime
acceptance remains pending in the roadmap. Commit/push and parent closure require the recorded acceptance gates;
this inventory grants no Production promotion or FTPS authorization.

## v66.0.3 implementation evidence

Canonical evidence now lives in `Services/FlexibleMaterialEvidenceService.cs` and typed model records. The shared
`BuildMetricGroups` engine feeds Comparable Results (including Recovery) and Material Detail through
`MainWindow.FlexibleEvidence.cs`. A detached saved-graph cache is refreshed after initialization and successful save.
General displays every method/condition group; a queued refresh does not rebind the active Flexible input grids.
Independent review corrected target-strain collisions in comparison keys. Public/report/ranking/research consumers
remain planned as shown above; the private method record must not be serialized directly to public output.
Debug/Release and Help/docs pass. Profile `20260911223150-70e446de` passes Full Verification 451/451 and exact recovery.
Owner runtime acceptance remains pending; subsequent mapped increments remain planned.

### Owner layout correction

Owner rejected repetitive General method/statistics prose and requested Mechanical dashboard parity.
General now contains coverage and the Mechanical location only. Mechanical / Engineering Dashboard has a Flexible table per method,
with condition, result/unit, mean, n, SD, CV and range. Each saved method appears once in collapsed Test setup; shared conditions
are shown once. The original verbose field builder is replaced, with no unused adapter retained. Saved cache and raw inputs persist.
Selected-material dashboard scope moved forward from v66.0.7; other analytics remain planned. Overall remains unchanged.
Existing Verification checks the revised read-only renderer, setup grouping and empty results; no new editable grid or scenario.
Help, field ownership, Debug/Release and documentation gates pass. Profile `20260911230648-cd4b8e83` passes Full Verification 451/451 and exact database/business-state recovery.
Synthetic WPF table rendering inspected; owner review of the revised Mechanical dashboard remains pending.
## v66.0.3.1 - Flexible Material Detail Handoffs

Owner accepted the compact v66.0.3 layout. Adds saved Flexible information to Material Detail Compare, Video Planner and Recommendations.
Compare uses the exact method/condition key, A-D identities, mean/unit/n and an unmatched-peer marker; no universal winner or score.
Selected-material video briefs use measured topics; guidance relates recorded load/retention/recovery/hardness to comparison choices.
Matching candidates require shared measured keys and distinct MaterialIDs within the current Materials scope. Empty data remains empty.
Both panels identify the displayed material, work without Overall and refresh from saved evidence; saved ideas remain immutable.
Existing Overall and global ranking/planner/research/export contracts remain owned by the recorded later increments.
Help and existing Full Verification cover isolation, matching, missing/zero and measured-topic guidance. No new input or scenario.
Debug/Release, Help and documentation gates pass. Profile `20260911234829-81ae08d6` passes Full Verification 452/452 and exact recovery.
Synthetic WPF renders inspected; owner runtime review pending. Canonical remains v65.0.0; no publication or release closure.

## v66.0.7 - Flexible Comparison and Analytics

Owner requested the next increment on 2026-09-12. Prior outstanding acceptance is retained, not inferred complete.
Analytics now includes visible Flexible-only materials in separate native-unit charts, before the Overall eligibility filter.
Existing Chart Mode partitions by manufacturer/material type/product line/variant/reinforcement while keeping each material's
mean, n, SD, CV and range. No pooling of specimen counts or statistics; method and condition boundaries remain exact.
Compare keeps its accepted A-D scope and adds dispersion/range to native-unit cells. No universal Flexible winner highlighting.
Selected Mechanical dashboard and Dashboard Insights already own detail/coverage. Existing website chart payloads and normalized
radars remain score-only; safe Flexible public evidence stays in the separately implemented report sections, without publication.
Subagent review identified ambiguous radar coverage and missing unreached context; both are corrected. Zero bars have zero length.
Help and existing disposable Full Verification cover scope/condition/zero/missing/sample contracts. No new input workflow or seed.
Candidate only: owner chart/grouping/Compare review required. v66.0.8 must reconcile outstanding earlier acceptance before closure.
## Closure audit — current consumer disposition (v66.0.8)

| Consumer family | Current ownership / disposition |
|---|---|
| Raw measurements and Comparable Results | Canonical saved specimen projection; no raw or method-snapshot mutation |
| General / Mechanical | Accepted compact summary and native-unit dashboard |
| Compare | Accepted unified selected-material table; metric/unit/measurement-point matching, not setup metadata |
| Analytics | Per-material native-unit charts and Chart Mode partitions; owner grouping accepted |
| Reports / PDF / six public families | Accepted safe aggregates, freshness and complete PDF continuation; no publication |
| Rankings / category rankings / awards | Accepted separate exact-method evidence, explicit directional eligibility; Overall unchanged |
| Video / recommendations detail | Accepted selected-material handoffs and dataset opportunity panels; save/restart accepted |
| Main video planner | v66.0.8 measured Flexible rows and readiness filtering; pure candidate projection, no score-engine input |
| Titles / thumbnails / copy | v66.0.8 factual representative topic per material; sorted top/copy identity; all topics in opportunity selector |
| Comparison discovery / planner comparisons | v66.0.8 unordered exact-condition pairs and factual copy blocks; no winner claim |
| Channel gaps / calendar / playlists | v66.0.8 recorded-video coverage and measured topic planning; no pooled material quality |
| Local AI / explicit OpenAI preview | Separate scoped evidence, safe fields/hash contract; no automatic sending |
| Dashboard Insights | Live visible Flexible coverage; full results in dedicated surfaces |
| Collection / production pipeline dashboards | Deliberately retained saved workflow-status scope; no live measurement rewrite |
| Five-axis advisor / normalized radar / website score charts | Deliberately retain score contract; native-unit counterparts above |

Earlier research-baseline lists identify investigated callers, not a requirement to inject Flexible values into every score formula.
The closure audit found and implemented the missing main planner/research connections; saved status-only consumers remain unchanged.
Historical acceptance notes above remain evidence history. Current unresolved manual review: Settings default, research save/restart,
Analytics grouping and this final research integration. README runtime-accepted identity remains v65.0.0 until full closure.
FINAL DISPOSITION 2026-09-12: all listed v66 integrations accepted, including v66.0.8 research lists. Source milestone complete.
The tested runtime retains its candidate code; no v66 distribution package or Production runtime acceptance is claimed.
