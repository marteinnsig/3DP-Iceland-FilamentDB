using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Automation;
using Microsoft.Data.Sqlite;
using System.Globalization;
using FilamentDbApp.UpdateCore;

namespace FilamentDbApp.AutomationRunner;

internal static class Program
{
    private const string MarkerFileName = ".3dpiceland-disposable-profile.json";
    private static readonly TimeSpan ElementTimeout = TimeSpan.FromSeconds(45);
    private static readonly TimeSpan ReportTimeout = TimeSpan.FromMinutes(20);
    private static readonly List<StepResult> Steps = new();
    private static string CurrentScenario = "smoke";

    [STAThread]
    private static int Main(string[] args)
    {
        if (args.Contains("--cleanup-profiles", StringComparer.Ordinal))
        {
            var pins = ReadRepeatedArguments(args, "--pin-profile");
            return DisposableProfileCleanupService.CreateDryRun(
                pins.ToHashSet(StringComparer.Ordinal));
        }
        if (args.Contains("--cleanup-self-test", StringComparer.Ordinal))
            return DisposableProfileCleanupService.RunSyntheticSelfTest();
        if (args.Contains("--apply-cleanup-plan", StringComparer.Ordinal))
        {
            return DisposableProfileCleanupService.Apply(
                ReadRequiredArgument(args, "--apply-cleanup-plan"),
                ReadRequiredArgument(args, "--plan-sha256"));
        }

        Process? application = null;
        string? root = null;
        string executable = string.Empty;
        string seedDatabase = string.Empty;
        string databasePath = string.Empty;
        string seedDatabaseHash = string.Empty;
        string databaseHashBefore = string.Empty;
        string databaseBusinessHashBefore = string.Empty;
        LandedCostEvidenceCapture? landedCostCapture = null;
        try
        {
            var options = RunnerOptions.Parse(args);
            CurrentScenario = options.Scenario;
            var cleanReadiness = string.Equals(options.Scenario, "clean", StringComparison.Ordinal);
            executable = IOPath.GetFullPath(options.ApplicationPath);
            seedDatabase = string.IsNullOrWhiteSpace(options.SeedDatabasePath)
                ? string.Empty
                : IOPath.GetFullPath(options.SeedDatabasePath);
            if (!IOFile.Exists(executable)) throw new FileNotFoundException("Application executable not found.", executable);
            if (!cleanReadiness && !IOFile.Exists(seedDatabase))
                throw new FileNotFoundException("Explicit seed database not found.", seedDatabase);

            root = CreateDisposableProfile(
                executable,
                seedDatabase,
                cleanReadiness,
                string.Equals(options.Scenario, "reports", StringComparison.Ordinal),
                string.Equals(options.Scenario, "crud", StringComparison.Ordinal),
                string.Equals(options.Scenario, "landed-cost", StringComparison.Ordinal) ||
                string.Equals(options.Scenario, "recovery", StringComparison.Ordinal),
                string.Equals(options.Scenario, "recovery", StringComparison.Ordinal),
                string.Equals(options.Scenario, "updater", StringComparison.Ordinal),
                out var markerPath,
                out databasePath,
                out var materialCrudId,
                out var landedCostPurchaseOrderId,
                out var landedCostMaterialId,
                out var landedCostInventoryItemId);
            seedDatabaseHash = cleanReadiness ? string.Empty : Sha256(seedDatabase);
            Require(
                !cleanReadiness || !IOFile.Exists(databasePath),
                "Clean Readiness database unexpectedly existed before first launch.");
            application = Process.Start(new ProcessStartInfo(executable)
            {
                UseShellExecute = false,
                WorkingDirectory = IOPath.GetDirectoryName(executable)!
            }.WithArgument("--automation-profile", markerPath))
                ?? throw new InvalidOperationException("Application process did not start.");

            var main = WaitForElement(
                AutomationElement.RootElement,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "MainWindow")),
                "Main window");
            Record("startup", true, $"Main window found for PID {application.Id}");
            AssertNoUnexpectedWindows(application.Id, "MainWindow");
            var baselineSnapshotPath = IOPath.Combine(root, "evidence", "database-before.sqlite");
            CreateConsistentSnapshot(databasePath, baselineSnapshotPath);
            databaseHashBefore = ComputeLogicalDatabaseHash(baselineSnapshotPath);
            databaseBusinessHashBefore = ComputeLogicalDatabaseHash(
                baselineSnapshotPath,
                excludeVolatileTimestamps: true);
            Record("database-runtime-baseline", true, databaseHashBefore);

            var identity = FindById(main, "RuntimeProfileIdentity");
            var identityName = identity.Current.Name;
            var expectedIdentity = cleanReadiness ? "CLEAN / READINESS" : "VERIFICATION / DISPOSABLE";
            Require(identityName.Contains(expectedIdentity, StringComparison.Ordinal),
                $"{expectedIdentity} runtime profile identity is not visible.");
            Require(
                identity.Current.HelpText.Contains("Owner database: BLOCKED", StringComparison.Ordinal) &&
                identity.Current.HelpText.Contains("Production/FTPS: BLOCKED", StringComparison.Ordinal) &&
                identity.Current.HelpText.Contains("updates: BLOCKED", StringComparison.Ordinal),
                "Disposable runtime profile capability summary is incomplete.");
            Record("profile-identity", true, identityName);
            CaptureWindow(main, IOPath.Combine(root, "evidence", "main-window.png"));

            var topLevelTabIds = new[]
            {
                "MaterialsTab", "ManufacturersTab", "PurchaseOrdersTab", "PrintersTab",
                "PrintJobQuotesTab", "InventoryTab", "UsageTab", "ExperimentalTestingTab",
                "MaterialDetailTab", "TensileMeasurementsTab", "ImpactMeasurementsTab",
                "StiffnessMeasurementsTab", "WebsiteExportTab", "BaseMaterialsTab",
                "SettingsManagerTab", "AiAssistantTab", "ReportsTab", "RankingsDashboardTab",
                "CategoryRankingsTab", "AwardsWinnersTab", "DashboardInsightsTab",
                "YouTubeResearchTab"
            };
            foreach (var tabId in topLevelTabIds)
            {
                SelectTab(main, tabId, application.Id);
            }
            Require(topLevelTabIds.Distinct(StringComparer.Ordinal).Count() == 22,
                "Top-level tab registry is not exactly 22 unique AutomationIds.");
            Record("top-level-tab-navigation", true,
                $"Visited {topLevelTabIds.Length}/22 unique top-level tabs by AutomationId");

            SelectTab(main, "MaterialsTab", application.Id);
            var materialFacetIds = new[]
            {
                "NativeMaterialManufacturerMultiFilter",
                "NativeMaterialBaseMaterialMultiFilter",
                "NativeMaterialVariantFinishMultiFilter",
                "NativeMaterialReinforcementMultiFilter",
                "NativeMaterialColorMultiFilter",
                "NativeMaterialProductLineMultiFilter"
            };
            foreach (var facetId in materialFacetIds)
            {
                FindById(main, facetId);
                FindById(main, facetId + "Open");
                FindById(main, facetId + "Clear");
                var summary = FindById(main, facetId + "SelectionSummary");
                Require(
                    summary.Current.Name.Contains(
                        "selection",
                        StringComparison.OrdinalIgnoreCase),
                    $"{facetId} did not expose its visible selection summary.");
            }
            FindById(main, "NativeMaterialSearch");
            FindById(main, "ClearNativeMaterialFilters");
            Record(
                "materials-multi-select-discovery",
                true,
                "Six no-modifier facets expose open, selection summary and per-filter Clear controls; global Clear is stable");

            SelectTab(main, "ExperimentalTestingTab", application.Id);
            var experimentalNestedTabIds = new[]
            {
                "ExperimentalTensileTab", "ExperimentalImpactTab", "ExperimentalStiffnessTab",
                "ExperimentalResultsTab"
            };
            foreach (var tabId in experimentalNestedTabIds)
                SelectTab(main, tabId, application.Id);
            SelectTab(main, "ExperimentalResultsTab", application.Id);
            var experimentalResultViewIds = new[]
            {
                "ExperimentalResultsDashboardTab", "ExperimentalResultsTableTab",
                "ExperimentalResultsChartsTab"
            };
            foreach (var tabId in experimentalResultViewIds)
                SelectTab(main, tabId, application.Id);

            SelectTab(main, "MaterialDetailTab", application.Id);
            var materialDetailNestedTabIds = new[]
            {
                "MaterialDetailGeneralTab", "MaterialDetailPrintingProfileTab",
                "MaterialDetailMechanicalTab", "MaterialDetailChartsTab",
                "MaterialDetailAnalyticsTab", "MaterialDetailCompareTab",
                "MaterialDetailVideoPlannerTab", "MaterialDetailRecommendationsTab",
                "MaterialDetailNotesTab"
            };
            foreach (var tabId in materialDetailNestedTabIds)
                SelectTab(main, tabId, application.Id);
            var nestedTabIds = experimentalNestedTabIds
                .Concat(experimentalResultViewIds)
                .Concat(materialDetailNestedTabIds)
                .ToArray();
            Require(nestedTabIds.Length == 16 &&
                    nestedTabIds.Distinct(StringComparer.Ordinal).Count() == 16,
                "Nested tab registry is not exactly 16 unique AutomationIds.");
            Record("nested-tab-navigation", true,
                $"Visited {nestedTabIds.Length}/16 unique nested tabs by AutomationId");

            InvokeWebsiteMenuNavigation(main, application.Id);
            Record("website-menu-navigation", true,
                "Supported Website menu action selected Website Export; disabled dead-end is retired");

            SelectTab(main, "RankingsDashboardTab", application.Id);
            OpenContextHelpAndRequireTitle(main, application.Id, "Rankings Dashboard reference");
            SelectTab(main, "ExperimentalTestingTab", application.Id);
            SelectTab(main, "ExperimentalResultsTab", application.Id);
            SelectTab(main, "ExperimentalResultsTableTab", application.Id);
            OpenContextHelpAndRequireTitle(main, application.Id, "Experimental Table reference");
            SelectTab(main, "MaterialDetailTab", application.Id);
            SelectTab(main, "MaterialDetailNotesTab", application.Id);
            OpenContextHelpAndRequireTitle(main, application.Id, "Material Detail — Notes reference");
            Record("contextual-help-navigation", true,
                "Current-view menu resolved representative top-level and nested tabs in the central Help window");

            Expand(FindById(main, "FileMenu"), application.Id);
            Invoke(FindById(main, "OpenRecoveryCenter"), application.Id);
            var recoveryCenter = WaitForElement(
                AutomationElement.RootElement,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "RecoveryCenterWindow")),
                "Recovery Center");
            AssertNoUnexpectedWindows(application.Id, "MainWindow", "RecoveryCenterWindow");
            Require(
                FindById(recoveryCenter, "RecoveryBackupCatalog").Current.ControlType == ControlType.DataGrid &&
                FindById(recoveryCenter, "RefreshRecoveryCatalog").Current.ControlType == ControlType.Button &&
                FindById(recoveryCenter, "VerifySelectedRecoveryBackup").Current.ControlType == ControlType.Button &&
                FindById(recoveryCenter, "RestoreSelectedRecoveryBackup").Current.ControlType == ControlType.Button &&
                FindById(recoveryCenter, "CreateRecoverySqliteBackup").Current.ControlType == ControlType.Button &&
                FindById(recoveryCenter, "RestoreRecoveryExcelBackup").Current.ControlType == ControlType.Button,
                "Recovery Center did not expose the governed catalog/read-only lookup and guarded action boundaries.");
            Record("recovery-center-read-only-inspection", true,
                "Opened and inspected catalog, verify, backup and restore controls without invoking a mutating action");
            CloseWindow(recoveryCenter, application.Id);

            Expand(FindById(main, "HelpMenu"), application.Id);
            Invoke(FindById(main, "OpenSystemDiagnostics"), application.Id);
            var diagnostics = WaitForElement(
                AutomationElement.RootElement,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "SystemDiagnosticsWindow")),
                "System Diagnostics");
            AssertNoUnexpectedWindows(application.Id, "MainWindow", "SystemDiagnosticsWindow");
            var diagnosticsReport = FindById(diagnostics, "SystemDiagnosticsReportText")
                .GetCurrentPropertyValue(ValuePattern.ValueProperty)?.ToString() ?? string.Empty;
            Require(
                FindById(diagnostics, "RefreshSystemDiagnostics").Current.ControlType == ControlType.Button &&
                FindById(diagnostics, "RunSystemIntegrityCheck").Current.ControlType == ControlType.Button &&
                FindById(diagnostics, "RecalculateAllMaterials").Current.ControlType == ControlType.Button &&
                FindById(diagnostics, "ExportSystemDiagnostics").Current.ControlType == ControlType.Button &&
                diagnosticsReport.Contains(
                    "Diagnostics do not modify application files",
                    StringComparison.Ordinal) &&
                diagnosticsReport.Contains("Identity: " + expectedIdentity, StringComparison.Ordinal) &&
                diagnosticsReport.Contains(
                    "Capabilities: Owner database: BLOCKED; Production/FTPS: BLOCKED; updates: BLOCKED",
                    StringComparison.Ordinal) &&
                diagnosticsReport.Contains("Credential ownership: Owner credentials inaccessible", StringComparison.Ordinal) &&
                diagnosticsReport.Contains("Update transaction ownership: ", StringComparison.Ordinal) &&
                diagnosticsReport.Contains("Evidence ownership: ", StringComparison.Ordinal) &&
                diagnosticsReport.Contains("Cleanup ownership: ", StringComparison.Ordinal) &&
                diagnosticsReport.Contains("Transactions found: 0", StringComparison.Ordinal) &&
                diagnosticsReport.Contains(
                    "Purchase Order landed-cost snapshots: current ",
                    StringComparison.Ordinal) &&
                diagnosticsReport.Contains(
                    "Inventory landed-cost snapshots: current ",
                    StringComparison.Ordinal) &&
                diagnosticsReport.Contains(
                    "opening or refreshing Diagnostics never executes either workflow",
                    StringComparison.Ordinal),
                "System Diagnostics did not expose runtime identity/capabilities, read-only evidence and the distinct mutating recalculation control.");
            Record("system-diagnostics-read-only-inspection", true,
                "Verified disposable database/preferences/output/credential/update/evidence/cleanup ownership plus read-only controls");
            CloseWindow(diagnostics, application.Id);

            SelectTab(main, "ExperimentalTestingTab", application.Id);
            var experimentalReadiness = FindById(main, "ExperimentalPublicationReadiness").Current.Name;
            Require(
                experimentalReadiness.StartsWith("Publication readiness:", StringComparison.Ordinal),
                "Experimental Testing did not expose publication readiness.");
            var experimentalActiveOnly = FindById(main, "ExperimentalActiveOnly");
            Require(
                experimentalActiveOnly.GetCurrentPattern(TogglePattern.Pattern) is TogglePattern activeOnlyToggle &&
                activeOnlyToggle.Current.ToggleState == ToggleState.On,
                "Experimental Series Active only filter did not start in its safe checked state.");
            Require(
                FindById(main, "ExperimentalIncludeInactiveHistory").Current.ControlType ==
                ControlType.CheckBox,
                "Experimental Testing did not expose the governed inactive-history comparison toggle.");
            Record("experimental-workflow-integrity", true, experimentalReadiness);

            SelectTab(main, "PurchaseOrdersTab", application.Id);
            var ecbStatus = FindById(main, "EcbExchangeRateStatus").Current.Name;
            Require(
                ecbStatus.Contains("Automation is offline", StringComparison.Ordinal) ||
                ecbStatus.Contains("Manual governed Settings", StringComparison.Ordinal),
                "Purchase Orders did not retain the offline-safe ECB reference status.");
            Record("ecb-reference-offline-boundary", true, ecbStatus);
            Require(
                FindById(main, "NewPurchaseOrderButton").Current.ControlType == ControlType.Button &&
                FindById(main, "PurchaseOrdersGrid").Current.ControlType == ControlType.DataGrid &&
                FindById(main, "DeletePurchaseOrderButton").Current.ControlType == ControlType.Button &&
                FindById(main, "AddPurchaseOrderLineButton").Current.ControlType == ControlType.Button &&
                FindById(main, "DeletePurchaseOrderLineButton").Current.ControlType == ControlType.Button &&
                FindById(main, "CreateMaterialFromPurchaseLineButton").Current.ControlType == ControlType.Button &&
                FindById(main, "ReceivePurchaseOrderButton").Current.ControlType == ControlType.Button &&
                FindById(main, "CreateInventoryFromPurchaseOrderButton").Current.ControlType == ControlType.Button &&
                FindById(main, "CalculatePurchaseCostsButton").Current.ControlType == ControlType.Button &&
                FindById(main, "PurchaseOrderLinesGrid").Current.ControlType == ControlType.DataGrid &&
                FindById(main, "LandedCostCurrencySelector").Current.ControlType == ControlType.ComboBox &&
                FindById(main, "ApplyLandedCostCurrencyOverride").Current.ControlType == ControlType.Button,
                "Purchase Orders did not expose the governed landed-cost Draft controls.");
            var landedCostStatus = FindById(main, "LandedCostRateStatus").Current.Name;
            Require(
                landedCostStatus.Contains("Select a Purchase Order", StringComparison.Ordinal) ||
                landedCostStatus.Contains("Draft override is available", StringComparison.Ordinal) ||
                landedCostStatus.Contains("Snapshot is locked", StringComparison.Ordinal),
                "Purchase Orders did not expose an honest landed-cost Draft status.");
            Require(
                FindById(main, "LandedCostConversionRate").Current.ControlType == ControlType.Text &&
                FindById(main, "PurchaseCostValidation").Current.ControlType == ControlType.Text,
                "Purchase Orders did not expose the landed-cost conversion-rate evidence.");
            Record("landed-cost-draft-ui-contract", true, landedCostStatus);

            SelectTab(main, "SettingsManagerTab", application.Id);
            Invoke(FindById(main, "TestAiProviderFoundation"), application.Id);
            var aiProviderStatus = FindById(main, "AiProviderStatus").Current.Name;
            Require(
                aiProviderStatus.Contains("deterministic fake provider", StringComparison.OrdinalIgnoreCase) &&
                aiProviderStatus.Contains("network used: no", StringComparison.OrdinalIgnoreCase),
                "Disposable Settings did not resolve the deterministic no-network AI provider.");
            var aiProviderApiKey = FindById(main, "AiProviderApiKey");
            Require(
                aiProviderApiKey.Current.ControlType == ControlType.Edit &&
                (bool)aiProviderApiKey.GetCurrentPropertyValue(AutomationElement.IsPasswordProperty),
                "Disposable AI provider credential field was not exposed as a protected password control.");
            Record("ai-provider-foundation-isolation", true, aiProviderStatus);

            SelectTab(main, "AiAssistantTab", application.Id);
            Invoke(FindById(main, "RefreshAiAssistantScope"), application.Id);
            var assistantScope = FindById(main, "AiAssistantScopeSummary").Current.Name;
            var assistantMaterialIds = FindById(main, "AiAssistantScopeMaterialIds").Current.Name;
            Require(
                assistantScope.Contains("unique MaterialID(s)", StringComparison.Ordinal) &&
                assistantMaterialIds.StartsWith("MaterialID preview:", StringComparison.Ordinal),
                "AI Assistant did not expose its deterministic visible MaterialID scope.");
            Invoke(FindById(main, "GenerateAiFullBrief"), application.Id);
            var assistantOutput = FindById(main, "AiAssistantOutput")
                .GetCurrentPropertyValue(ValuePattern.ValueProperty)?.ToString() ?? string.Empty;
            Require(
                cleanReadiness
                    ? assistantOutput.Contains("No visible materials are loaded", StringComparison.Ordinal)
                    : assistantOutput.Contains("AI ASSISTANT", StringComparison.Ordinal) &&
                      assistantOutput.Contains("Visible source materials:", StringComparison.Ordinal) &&
                      assistantOutput.Contains("Materials processed:", StringComparison.Ordinal) &&
                      assistantOutput.Contains("Materials omitted by the 60-material local brief limit:", StringComparison.Ordinal),
                "AI Assistant local full brief did not retain visible-scope evidence.");
            Record("ai-assistant-local-scope", true, assistantScope + " " + assistantMaterialIds);
            if (cleanReadiness)
            {
                Record("openai-pilot-zero-data-boundary", true,
                    "Payload preview was not invoked because zero visible Materials cannot produce an outbound request");
            }
            else
            {
                Invoke(FindById(main, "PreviewOpenAiPayload"), application.Id);
                var openAiPreview = FindById(main, "AiAssistantOutput")
                    .GetCurrentPropertyValue(ValuePattern.ValueProperty)?.ToString() ?? string.Empty;
                var openAiStatus = FindById(main, "OpenAiPilotStatus").Current.Name;
                var requestStart = openAiPreview.IndexOf("{\r\n  \"model\"", StringComparison.Ordinal);
                if (requestStart < 0)
                {
                    requestStart = openAiPreview.IndexOf("{\n  \"model\"", StringComparison.Ordinal);
                }
                Require(requestStart >= 0, "OpenAI preview did not expose its exact JSON request body.");
                using var requestDocument = JsonDocument.Parse(openAiPreview[requestStart..]);
                var requestRoot = requestDocument.RootElement;
                var previewHasStoreFalse =
                    requestRoot.TryGetProperty("store", out var store) &&
                    store.ValueKind == JsonValueKind.False;
                var previewHasNoTools =
                    requestRoot.TryGetProperty("tools", out var tools) &&
                    tools.ValueKind == JsonValueKind.Array &&
                    tools.GetArrayLength() == 0;
                var inputJson = requestRoot.GetProperty("input").GetString() ?? string.Empty;
                using var inputDocument = JsonDocument.Parse(inputJson);
                var inputRoot = inputDocument.RootElement;
                var firstMaterial = inputRoot.GetProperty("materials").EnumerateArray().First();
                var previewHasMaterialId =
                    firstMaterial.TryGetProperty("materialID", out var materialId) &&
                    !string.IsNullOrWhiteSpace(materialId.GetString());
                var previewHasForbiddenFields = new[]
                    {
                        "purchaseId", "inventoryId", "landedCostAmount", "notes",
                        "supplierUrl", "storageLocation", "purchasePriceAmount"
                    }
                    .Any(name => firstMaterial.TryGetProperty(name, out _));
                var previewUsedNoNetwork =
                    openAiStatus.Contains("no network used", StringComparison.OrdinalIgnoreCase);
                Require(
                    openAiPreview.Contains("OPENAI EXACT OUTBOUND PAYLOAD PREVIEW", StringComparison.Ordinal) &&
                    openAiPreview.Contains("Visible source MaterialIDs:", StringComparison.Ordinal) &&
                    openAiPreview.Contains("Omitted by governed 40-material limit:", StringComparison.Ordinal) &&
                    previewHasStoreFalse &&
                    previewHasNoTools &&
                    previewHasMaterialId &&
                    !previewHasForbiddenFields &&
                    previewUsedNoNetwork,
                    "OpenAI pilot preview did not retain its exact allowlist and no-network boundary. " +
                    $"length={openAiPreview.Length}; storeFalse={previewHasStoreFalse}; noTools={previewHasNoTools}; " +
                    $"materialId={previewHasMaterialId}; forbidden={previewHasForbiddenFields}; " +
                    $"noNetwork={previewUsedNoNetwork}; status={openAiStatus}");
                Require(
                    !FindById(main, "CancelOpenAiRequest").Current.IsEnabled,
                    "OpenAI cancel control was enabled without an active live request.");
                Require(
                    !FindById(main, "CopyOpenAiOperationalEvidence").Current.IsEnabled,
                    "OpenAI operational evidence was enabled without a live request attempt.");
                Invoke(FindById(main, "SaveAiSession"), application.Id);
                var previewSaveDialog = WaitForElement(
                    AutomationElement.RootElement,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                        new PropertyCondition(AutomationElement.NameProperty, "AI Assistant Session")),
                    "OpenAI preview session-save guard");
                var previewSaveText = previewSaveDialog.FindFirst(
                    TreeScope.Descendants,
                    new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Text));
                Require(
                    previewSaveText is not null &&
                    previewSaveText.Current.Name.Contains(
                        "exact outbound OpenAI payload preview cannot be saved",
                        StringComparison.Ordinal),
                    "OpenAI preview session-save guard did not explain the raw-payload persistence boundary.");
                var previewSaveOk = previewSaveDialog.FindFirst(
                    TreeScope.Descendants,
                    new AndCondition(
                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Button),
                        new PropertyCondition(AutomationElement.NameProperty, "OK")));
                Require(previewSaveOk is not null, "OpenAI preview session-save guard has no OK action.");
                Invoke(previewSaveOk!, application.Id);
                AssertNoUnexpectedWindows(application.Id, "MainWindow");
                Record("openai-pilot-payload-isolation", true, openAiStatus);
                Record(
                    "openai-pilot-preview-persistence-guard",
                    true,
                    "Exact payload preview was rejected by Save Session; operational evidence remained unavailable without live network");
            }
            var collectionAction = FindById(main, "AiCollectionActionState").Current.Name;
            Require(
                collectionAction.StartsWith("Action: Create a new collection", StringComparison.Ordinal),
                "AI collection workflow read non-disposable collection state.");
            if (cleanReadiness)
            {
                Record("ai-collection-zero-data-boundary", true,
                    "Preview was not invoked because zero visible Materials has no valid collection payload");
            }
            else
            {
                Invoke(FindById(main, "PreviewAiMaterialCollection"), application.Id);
                var collectionPreview = FindById(main, "AiAssistantOutput")
                    .GetCurrentPropertyValue(ValuePattern.ValueProperty)?.ToString() ?? string.Empty;
                Require(
                    collectionPreview.Contains("COLLECTION SAVE PREVIEW", StringComparison.Ordinal) &&
                    collectionPreview.Contains("No data has been written.", StringComparison.Ordinal) &&
                    collectionPreview.Contains("Unique MaterialIDs to save:", StringComparison.Ordinal) &&
                    collectionPreview.Contains("Exact MaterialID set SHA-256:", StringComparison.Ordinal),
                    "AI collection preview did not expose its read-only exact MaterialID contract.");
                Record("ai-collection-preview", true, collectionAction);
            }
            var coverageIdentity = FindById(main, "AiCoverageIdentityStatus").Current.Name;
            Require(
                coverageIdentity.Contains("0 stable CollectionID/MaterialID, 0 legacy", StringComparison.Ordinal),
                "Disposable AI coverage did not start with an isolated empty identity state.");
            Record("ai-coverage-identity", true, coverageIdentity);

            Expand(FindById(main, "HelpMenu"), application.Id);
            Invoke(FindById(main, "OpenHelp"), application.Id);
            var help = WaitForElement(
                AutomationElement.RootElement,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "HelpWindow")),
                "Help");
            AssertNoUnexpectedWindows(application.Id, "MainWindow", "HelpWindow");
            var helpTitle = FindById(help, "HelpSectionTitle").Current.Name;
            Require(
                string.Equals(helpTitle, "Start-to-finish workflow", StringComparison.Ordinal),
                "Help > Documentation did not open the canonical whole-system overview.");
            var helpContents = FindById(help, "HelpSections");
            Require(
                helpContents.Current.ControlType == ControlType.Tree,
                "Central Help contents did not expose the hierarchical tree contract.");
            Require(
                FindById(helpContents, "HelpCategory-Start-here").Current.ControlType == ControlType.TreeItem &&
                FindById(helpContents, "HelpTopic-start-here").Current.ControlType == ControlType.TreeItem,
                "Central Help did not expose the Start here category and selected topic tree nodes.");
            var helpSearch = FindById(help, "HelpSearch");
            Require(
                helpSearch.TryGetCurrentPattern(ValuePattern.Pattern, out var helpValuePattern),
                "Central Help search does not expose the UI Automation value contract.");
            ((ValuePattern)helpValuePattern).SetValue("landed costs");
            var helpStatus = WaitForElement(
                help,
                new PropertyCondition(AutomationElement.AutomationIdProperty, "HelpResultStatus"),
                "Help search status");
            Require(
                helpStatus.Current.Name.Contains("topic", StringComparison.OrdinalIgnoreCase) &&
                !helpStatus.Current.Name.StartsWith("0 ", StringComparison.Ordinal),
                "Central Help search did not return the deterministic start-to-finish workflow topic.");
            var helpBody = FindById(help, "HelpSectionBody").Current.Name;
            Require(
                helpBody.Contains("Create Materials + Received Spools", StringComparison.Ordinal) &&
                helpBody.Contains("Calculate Landed Costs", StringComparison.Ordinal),
                "Central Help did not expose the required Purchase Order costing and receiving handoffs.");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionBody").Current.HelpText,
                    "Highlighted search: landed costs",
                    StringComparison.Ordinal),
                "Central Help did not refresh the first filtered topic highlight immediately.");
            ((ValuePattern)helpValuePattern).SetValue(string.Empty);
            Require(
                string.Equals(
                    FindById(help, "HelpSectionBody").Current.HelpText,
                    "No highlighted search",
                    StringComparison.Ordinal),
                "Central Help retained a stale highlight after clearing Search.");
            ((ValuePattern)helpValuePattern).SetValue("scope and output");
            var normalizedHelpBody = FindById(help, "HelpSectionBody").Current.Name;
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Report scope and output reference",
                    StringComparison.Ordinal) &&
                normalizedHelpBody.Contains("scope and output folder", StringComparison.Ordinal),
                "Central Help retained source-only line breaks instead of wrapping text to the visible width.");
            ((ValuePattern)helpValuePattern).SetValue("manual backup creates evidence");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Materials reference",
                    StringComparison.Ordinal) &&
                FindById(help, "HelpSectionBody").Current.Name.Contains(
                    "it is not a Save button",
                    StringComparison.Ordinal),
                "Central Help did not expose the Materials command/save-boundary reference.");
            ((ValuePattern)helpValuePattern).SetValue("saved quote references");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Printers reference",
                    StringComparison.Ordinal) &&
                FindById(help, "HelpSectionBody").Current.Name.Contains(
                    "can be explicitly deleted from history",
                    StringComparison.Ordinal),
                "Central Help did not expose the Printer validation and saved-quote lifecycle reference.");
            ((ValuePattern)helpValuePattern).SetValue("lifecycle");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionSummary").Current.HelpText,
                    "Highlighted search: lifecycle",
                    StringComparison.Ordinal),
                "Central Help did not highlight a search match found in topic summary metadata.");
            ((ValuePattern)helpValuePattern).SetValue("Rows defaults to Top 25");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Rankings, Category Rankings, Awards and Insights controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the Rankings Top 25 default-scope reference.");
            ((ValuePattern)helpValuePattern).SetValue("application is not read-only");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Material Detail — Notes reference",
                    StringComparison.Ordinal),
                "Central Help retained the stale Material Detail Notes application-wide read-only guidance.");
            ((ValuePattern)helpValuePattern).SetValue("Experimental Table");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Experimental Table reference",
                    StringComparison.Ordinal),
                "Central Help did not expose the owner-searchable Experimental Table reference.");
            ((ValuePattern)helpValuePattern).SetValue("does not perform FTPS");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Website Export controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the local Production versus FTPS boundary.");
            ((ValuePattern)helpValuePattern).SetValue("do not create external calendar events");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "YouTube calendar reference",
                    StringComparison.Ordinal),
                "Central Help did not expose the local-only creator calendar boundary.");
            ((ValuePattern)helpValuePattern).SetValue("never automatically restore SQLite");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Guarded update apply and recovery reference",
                    StringComparison.Ordinal),
                "Central Help did not expose the no-auto-SQLite-restore update boundary.");
            ((ValuePattern)helpValuePattern).SetValue("Recalculate Native Results is mutating");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Verification Center reference",
                    StringComparison.Ordinal),
                "Central Help did not expose the Verification refresh versus recalculation boundary.");
            ((ValuePattern)helpValuePattern).SetValue("Never include FTPS passwords");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Support evidence collection reference",
                    StringComparison.Ordinal),
                "Central Help did not expose the secret-safe support evidence boundary.");
            ((ValuePattern)helpValuePattern).SetValue("Current filtered rows");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Materials controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the Materials control/field reference.");
            ((ValuePattern)helpValuePattern).SetValue(
                "An empty result clears the current Materials selection/details");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Materials controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the v54 no-modifier OR/AND multi-select contract.");
            ((ValuePattern)helpValuePattern).SetValue("never automatically repriced");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Purchase Orders controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the Purchase Order historical-rate boundary.");
            ((ValuePattern)helpValuePattern).SetValue("Save Settings does not publish");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Settings Manager controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the Settings column/save-boundary reference.");
            ((ValuePattern)helpValuePattern).SetValue("Revolutions accepts 0 through 10");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Native Measurements controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the native measurement control/field reference.");
            ((ValuePattern)helpValuePattern).SetValue("Results always remain selected-Series scoped");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Experimental Testing controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the Experimental read-only Results boundary.");
            ((ValuePattern)helpValuePattern).SetValue("ChatGPT Prompt box is read-only local text");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Material Detail interactive controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the Material Detail interactive control/field reference.");
            ((ValuePattern)helpValuePattern).SetValue("users cannot type or appoint a winner");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Rankings, Category Rankings, Awards and Insights controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the global Analysis read-only boundary.");
            ((ValuePattern)helpValuePattern).SetValue("None of these actions perform FTPS");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Reports controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the Reports local-only action boundary.");
            ((ValuePattern)helpValuePattern).SetValue("four separate contracts");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Website Export controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the four separate Website action contracts.");
            ((ValuePattern)helpValuePattern).SetValue(
                "Preview OpenAI Payload builds and displays the exact request body");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "AI Assistant controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the AI Assistant local/network boundary.");
            ((ValuePattern)helpValuePattern).SetValue("do not open a browser");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "YouTube Research controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the YouTube clipboard-only boundary.");
            ((ValuePattern)helpValuePattern).SetValue("Eight hidden CRUD/recovery buttons");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Application menu and runtime-window controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the automation-only shell boundary.");
            ((ValuePattern)helpValuePattern).SetValue("OWNER / PRODUCTION uses");
            Require(
                string.Equals(
                    FindById(help, "HelpSectionTitle").Current.Name,
                    "Application menu and runtime-window controls and fields",
                    StringComparison.Ordinal),
                "Central Help did not expose the v51.1 owner/disposable runtime identity boundary.");
            Record(
                "central-help",
                true,
                $"Opened hierarchical overview '{helpTitle}', verified highlighting plus v50.2.1-v51.1 reference searches");
            CloseWindow(help, application.Id);

            Expand(FindById(main, "HelpMenu"), application.Id);
            Invoke(FindById(main, "OpenVerificationCenter"), application.Id);
            var verification = WaitForElement(
                AutomationElement.RootElement,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "VerificationCenterWindow")),
                "Verification Center");
            AssertNoUnexpectedWindows(application.Id, "MainWindow", "VerificationCenterWindow");
            Record("verification-window", true, "Verification Center opened");
            Require(
                FindById(verification, "RefreshVerification").Current.ControlType == ControlType.Button &&
                FindById(verification, "RecalculateNativeResults").Current.ControlType == ControlType.Button &&
                FindById(verification, "ExportVerificationReport").Current.ControlType == ControlType.Button,
                "Verification Center did not expose refresh, mutating recalculation and export as distinct controls.");
            Invoke(FindById(verification, "ExportAutomationVerificationEvidence"), application.Id);
            WaitForFile(
                IOPath.Combine(root, "evidence", "verification.json"),
                application.Id,
                "MainWindow",
                "VerificationCenterWindow");
            var verificationEvidence = JsonDocument.Parse(
                IOFile.ReadAllText(IOPath.Combine(root, "evidence", "verification.json")));
            var verificationText = IOFile.ReadAllText(IOPath.Combine(root, "evidence", "verification.txt"));
            Require(
                verificationText.Contains("Runtime profile: " + expectedIdentity, StringComparison.Ordinal) &&
                verificationText.Contains("Data profile: ", StringComparison.Ordinal) &&
                verificationText.Contains("Classification: mandatory ", StringComparison.Ordinal),
                "Verification summary does not explicitly separate runtime identity, data profile and classification.");
            Require(
                verificationEvidence.RootElement.TryGetProperty("Passed", out var passedElement) &&
                passedElement.GetBoolean(),
                "Exported Verification reported FAIL.");
            var verificationRoot = verificationEvidence.RootElement;
            Require(
                verificationRoot.GetProperty("MandatoryNotApplicableCount").GetInt32() == 0 &&
                verificationRoot.GetProperty("MandatoryEvidenceFailedCount").GetInt32() == 0,
                "Verification converted mandatory evidence to N/A or reported a mandatory evidence failure.");
            var classifiedChecks = verificationRoot.GetProperty("checks").EnumerateArray().ToList();
            Require(
                classifiedChecks.Count > 0 &&
                classifiedChecks.All(check =>
                    check.GetProperty("status").GetString() != "NotApplicable" ||
                    check.GetProperty("applicability").GetString() == "CanonicalDataDependent"),
                "Verification evidence contains an N/A check outside CanonicalDataDependent classification.");
            if (cleanReadiness)
            {
                var rootElement = verificationRoot;
                Require(
                    rootElement.GetProperty("profile").GetString() == "Application Readiness" &&
                    rootElement.GetProperty("NotApplicableCount").GetInt32() > 0 &&
                    rootElement.GetProperty("CanonicalDataNotApplicableCount").GetInt32() ==
                    rootElement.GetProperty("NotApplicableCount").GetInt32() &&
                    rootElement.GetProperty("runtimeProfileKind").GetString() == "CleanReadiness",
                    "Clean Readiness evidence did not report Application Readiness, explicit N/A checks and CleanReadiness identity.");
                Require(CountRows(databasePath, "NativeMaterialManagerRows") == 0,
                    "Clean Readiness unexpectedly contains canonical Materials.");
                Require(
                    !IOFile.Exists(IOPath.Combine(root, "database", "3DPIceland-Automation-Seed-Evidence.bak")),
                    "Clean Readiness unexpectedly retained seed evidence.");
                Record("clean-zero-data-classification", true,
                    $"{rootElement.GetProperty("PassedCount").GetInt32()} PASS; " +
                    $"{rootElement.GetProperty("NotApplicableCount").GetInt32()} N/A; 0 Materials");
            }
            else
            {
                Require(
                    verificationRoot.GetProperty("profile").GetString() == "Full Data Verification" &&
                    verificationRoot.GetProperty("NotApplicableCount").GetInt32() == 0,
                    "Populated Verification did not retain Full Data Verification with zero N/A checks.");
            }
            CaptureWindow(verification, IOPath.Combine(root, "evidence", "verification-center.png"));
            Record("verification-export", true, "TXT/JSON evidence exported");

            CloseWindow(verification, application.Id);
            if (cleanReadiness)
            {
                (application, main) = RestartApplication(application, executable, markerPath);
                var restartedIdentity = FindById(main, "RuntimeProfileIdentity").Current.Name;
                Require(restartedIdentity.Contains("CLEAN / READINESS", StringComparison.Ordinal),
                    "Clean Readiness identity was not preserved across restart.");
                Require(CountRows(databasePath, "NativeMaterialManagerRows") == 0,
                    "Clean Readiness restart unexpectedly introduced canonical Materials.");
                var restartBaselinePath = IOPath.Combine(root, "evidence", "clean-restart-baseline.sqlite");
                CreateConsistentSnapshot(databasePath, restartBaselinePath);
                databaseHashBefore = ComputeLogicalDatabaseHash(restartBaselinePath);
                databaseBusinessHashBefore = ComputeLogicalDatabaseHash(
                    restartBaselinePath,
                    excludeVolatileTimestamps: true);
                Record("clean-restart", true,
                    "Same manifest restarted with CLEAN / READINESS identity, zero canonical Materials and a stable post-initialization baseline");
                CaptureWindow(main, IOPath.Combine(root, "evidence", "clean-restart.png"));
            }
            else if (string.Equals(options.Scenario, "reports", StringComparison.Ordinal))
            {
                SelectTab(main, "ReportsTab", application.Id);
                var outputFolder = FindById(main, "ReportOutputFolder");
                Require(
                    string.Equals(
                        outputFolder.GetCurrentPropertyValue(ValuePattern.ValueProperty)?.ToString(),
                        IOPath.Combine(root, "output"),
                        StringComparison.OrdinalIgnoreCase),
                    "Reports output folder is not bound to the disposable automation profile.");
                Invoke(FindById(main, "BuildPublicReportPackage"), application.Id);
                WaitForReportCompletion(main, application.Id);
                var artifactCount = ValidateReportArtifacts(root);
                CaptureWindow(main, IOPath.Combine(root, "evidence", "report-package.png"));
                Record("report-package", true, $"{artifactCount} catalog/root artifacts verified and hashed");
            }
            else if (string.Equals(options.Scenario, "crud", StringComparison.Ordinal))
            {
                RunCrudAction(main, application.Id, "AutomationCrudCreate", "CREATED");
                RecordDatabaseEvidence(root, databasePath, "crud-after-create");
                ValidateQuotePersistence(databasePath, materialCrudId, 1);
                ValidateUsagePersistence(databasePath, materialCrudId, 1, 0, 0, "900");
                ValidateUsageWorkspace(main, application.Id, materialCrudId, 1);
                ValidateUsageAnalytics(main, materialCrudId, 1, 1, "100.00 g", "1 h");
                Record("crud-create-save", true, materialCrudId);
                (application, main) = RestartApplication(application, executable, markerPath);
                RunCrudAction(main, application.Id, "AutomationCrudEdit", "EDITED");
                RecordDatabaseEvidence(root, databasePath, "crud-after-edit");
                ValidateQuotePersistence(databasePath, materialCrudId, 1);
                ValidateUsagePersistence(databasePath, materialCrudId, 3, 1, 1, "920");
                ValidateUsageWorkspace(main, application.Id, materialCrudId, 3);
                ValidateUsageAnalytics(main, materialCrudId, 1, 3, "80.00 g", "55 min");
                Record("crud-restart-edit-save", true, materialCrudId);
                (application, main) = RestartApplication(application, executable, markerPath);
                RunCrudAction(main, application.Id, "AutomationCrudDelete", "DELETED");
                RecordDatabaseEvidence(root, databasePath, "crud-after-delete");
                ValidateQuotePersistence(databasePath, materialCrudId, 0);
                Record("crud-restart-delete-save", true, materialCrudId);
                (application, main) = RestartApplication(application, executable, markerPath);
                RunCrudAction(main, application.Id, "AutomationCrudVerifyAbsent", "ABSENT");
                ValidateUsagePersistence(databasePath, materialCrudId, 0, 0, 0, null);
                Record("crud-restart-verify-absent", true, materialCrudId);
                CaptureWindow(main, IOPath.Combine(root, "evidence", "crud-complete.png"));
            }
            else if (string.Equals(options.Scenario, "landed-cost", StringComparison.Ordinal))
            {
                var lineId = landedCostPurchaseOrderId + "-LINE";
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostPrepare", "DEFAULTED");
                RequireExactRowCount(databasePath, "PurchaseOrders", "PurchaseOrderId",
                    landedCostPurchaseOrderId, 1);
                RequireExactRowCount(databasePath, "PurchaseOrderLines", "PurchaseOrderLineId",
                    lineId, 1);
                Record("landed-cost-default", true,
                    $"{landedCostPurchaseOrderId}: governed default snapshot persisted");

                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostOverride", "OVERRIDDEN");
                var overrideRow = ReadExactRow(
                    databasePath, "PurchaseOrders", "PurchaseOrderId", landedCostPurchaseOrderId);
                Require(
                    !string.IsNullOrWhiteSpace(overrideRow["LandedCostCurrency"]) &&
                    !string.IsNullOrWhiteSpace(overrideRow["LandedCostConversionRate"]) &&
                    !string.IsNullOrWhiteSpace(overrideRow["LandedCostRateSource"]),
                    "Disposable landed-cost override did not retain governed provenance.");
                Record("landed-cost-override", true,
                    $"cancel preserved default; applied {overrideRow["LandedCostCurrency"]} at " +
                    overrideRow["LandedCostConversionRate"]);

                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostCalculate", "CALCULATED");
                var calculatedOrder = ReadExactRow(
                    databasePath, "PurchaseOrders", "PurchaseOrderId", landedCostPurchaseOrderId);
                var calculatedLine = ReadExactRow(
                    databasePath, "PurchaseOrderLines", "PurchaseOrderLineId", lineId);
                Require(
                    calculatedOrder["LandedCostCalculationVersion"] == "landed-currency-v2" &&
                    !string.IsNullOrWhiteSpace(calculatedOrder["LandedCostCalculatedAtUtc"]) &&
                    !string.IsNullOrWhiteSpace(calculatedLine["LandedLineCost"]) &&
                    !string.IsNullOrWhiteSpace(calculatedLine["LandedUnitCost"]) &&
                    !string.IsNullOrWhiteSpace(calculatedLine["LandedCostPerKg"]),
                    "Disposable landed-cost calculation snapshot is incomplete.");
                Record("landed-cost-calculation-lock", true,
                    "calculation version, UTC stamp and line/unit/kg results persisted");

                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostDownstream", "DOWNSTREAM");
                var materialRow = ReadExactRow(
                    databasePath, "NativeMaterialManagerRows", "MaterialID", landedCostMaterialId);
                var inventoryRow = ReadExactRow(
                    databasePath, "InventorySpoolItems", "InventoryItemId", landedCostInventoryItemId);
                Require(
                    materialRow["PurchaseId"] == landedCostPurchaseOrderId &&
                    materialRow["LandedCostCurrency"] == calculatedOrder["LandedCostCurrency"] &&
                    inventoryRow["PurchaseId"] == landedCostPurchaseOrderId &&
                    inventoryRow["MaterialId"] == landedCostMaterialId &&
                    inventoryRow["LandedCostCurrency"] == calculatedOrder["LandedCostCurrency"] &&
                    inventoryRow["LandedCostCalculationVersion"] == "landed-currency-v2",
                    "Disposable downstream Material/Inventory snapshots do not match the saved order.");
                landedCostCapture = new LandedCostEvidenceCapture(
                    landedCostPurchaseOrderId,
                    landedCostMaterialId,
                    landedCostInventoryItemId,
                    calculatedOrder["Currency"],
                    calculatedOrder["LandedCostCurrency"],
                    calculatedOrder["LandedCostConversionRate"],
                    calculatedOrder["LandedCostRateSource"],
                    calculatedOrder["LandedCostRateObservationDate"],
                    calculatedOrder["LandedCostRateFetchedAtUtc"],
                    calculatedOrder["LandedCostCalculatedAtUtc"],
                    calculatedOrder["LandedCostCalculationVersion"],
                    [
                        "DEFAULTED-RESTART",
                        "OVERRIDDEN-RESTART",
                        "CALCULATED-RESTART",
                        "DOWNSTREAM-RESTART",
                        "CLEANED-RESTART",
                        "ABSENT"
                    ],
                    HashExactRow(databasePath, "PurchaseOrders", "PurchaseOrderId",
                        landedCostPurchaseOrderId),
                    HashExactRow(databasePath, "InventorySpoolItems", "InventoryItemId",
                        landedCostInventoryItemId),
                    HashExactRow(databasePath, "NativeMaterialManagerRows", "MaterialID",
                        landedCostMaterialId),
                    HashTable(databasePath, "UsageEvents"),
                    HashTable(databasePath, "PrintJobQuotes"));
                Record("landed-cost-downstream", true,
                    $"{landedCostMaterialId}/{landedCostInventoryItemId}: exact saved provenance");

                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostCleanup", "CLEANED");
                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostVerifyAbsent", "ABSENT");
                RequireExactRowCount(databasePath, "PurchaseOrders", "PurchaseOrderId",
                    landedCostPurchaseOrderId, 0);
                RequireExactRowCount(databasePath, "PurchaseOrderLines", "PurchaseOrderLineId",
                    lineId, 0);
                RequireExactRowCount(databasePath, "NativeMaterialManagerRows", "MaterialID",
                    landedCostMaterialId, 0);
                RequireExactRowCount(databasePath, "InventorySpoolItems", "InventoryItemId",
                    landedCostInventoryItemId, 0);
                CaptureWindow(main, IOPath.Combine(root, "evidence", "landed-cost-complete.png"));
                Record("landed-cost-cleanup", true,
                    "exact disposable PO/line/Material/Inventory identities absent after restart");
            }
            else if (string.Equals(options.Scenario, "recovery", StringComparison.Ordinal))
            {
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostPrepare", "DEFAULTED");
                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostOverride", "OVERRIDDEN");
                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostCalculate", "CALCULATED");
                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostDownstream", "DOWNSTREAM");
                RunRecoveryAction(main, application.Id, "AutomationRecoveryBackup", "BACKUPS-VERIFIED");
                var backupArtifacts = ValidateRecoveryBackupArtifacts(root, databasePath);
                Record("recovery-backup-catalog", true, $"{backupArtifacts} verified .bak/.sqlite artifacts");
                RunRecoveryAction(main, application.Id, "AutomationRecoveryExportExcel", "EXCEL-EXPORTED");
                var workbook = IOPath.Combine(root, "output", "3DPIceland-Automation-DisasterRecovery.xlsx");
                Require(IOFile.Exists(workbook) && new FileInfo(workbook).Length > 0,
                    "Governed Excel recovery package is missing.");
                Record("recovery-excel-package", true,
                    $"{new FileInfo(workbook).Length} bytes; sha256={Sha256(workbook)}");
                var v53RecoveryHashes = RecoveryHistoricalHashes(databasePath);
                RunRecoveryAction(main, application.Id, "AutomationRecoveryMutate", "MUTATED");
                RecordDatabaseEvidence(root, databasePath, "recovery-after-mutation");
                var mutatedRecoveryHashes = RecoveryHistoricalHashes(databasePath);
                Require(
                    mutatedRecoveryHashes.PurchaseOrders != v53RecoveryHashes.PurchaseOrders &&
                    mutatedRecoveryHashes.Inventory != v53RecoveryHashes.Inventory,
                    "Recovery mutation did not change both exact v53 Purchase Order and Inventory fields.");
                RunRecoveryAction(main, application.Id, "AutomationRecoveryRestoreExcel", "EXCEL-RESTORED");
                RecordDatabaseEvidence(root, databasePath, "recovery-after-excel-restore");
                var restoredRecoveryHashes = RecoveryHistoricalHashes(databasePath);
                Require(
                    restoredRecoveryHashes == v53RecoveryHashes,
                    "Excel recovery did not restore exact v53 and historical table state.");
                var restoreArtifacts = ValidateRecoveryBackupArtifacts(root, databasePath);
                Require(restoreArtifacts >= backupArtifacts + 2,
                    "Excel restore did not add both pre/post SQLite evidence backups.");
                Record("recovery-v53-field-equality", true,
                    "Purchase Orders, Inventory, Materials, Usage and Quotes restored exactly");
                Record("recovery-pre-post-evidence", true,
                    $"{restoreArtifacts} verified backup artifacts");
                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostCleanup", "CLEANED");
                (application, main) = RestartApplication(application, executable, markerPath);
                RunLandedCostAction(
                    main, application.Id, "AutomationLandedCostVerifyAbsent", "ABSENT");
                CaptureWindow(main, IOPath.Combine(root, "evidence", "recovery-complete.png"));
                Record("recovery-restart", true,
                    "Restored disposable profile restarted, cleaned exact v53 records and verified absence");
            }
            else if (string.Equals(options.Scenario, "migration", StringComparison.Ordinal))
            {
                ValidateSchema37StartupMigration(seedDatabase, databasePath);
                Record("schema37-startup-migration", true,
                    "v37 fixture migrated to v38; all shared historical columns matched exactly");
            }
            CloseWindow(main, application.Id);
            if (!application.WaitForExit(15000))
                throw new TimeoutException("Application did not complete controlled shutdown.");
            Require(
                !IOFile.Exists(IOPath.Combine(root, "preferences", "ai-assistant-sessions.json")) &&
                !IOFile.Exists(IOPath.Combine(root, "preferences", "ai-material-collections.json")),
                "Read-only automation unexpectedly created AI session or collection storage.");
            Record("ai-storage-isolation", true,
                "Disposable preferences remained free of AI session/collection files");
            if (string.Equals(options.Scenario, "updater", StringComparison.Ordinal))
            {
                RunUpdaterAcceptance(
                    root,
                    executable,
                    IOPath.GetFullPath(options.UpdaterPath),
                    markerPath,
                    application.Id);
            }

            var finalSnapshotPath = IOPath.Combine(root, "evidence", "database-after.sqlite");
            CreateConsistentSnapshot(databasePath, finalSnapshotPath);
            var databaseHashAfter = ComputeLogicalDatabaseHash(finalSnapshotPath);
            var databaseBusinessHashAfter = ComputeLogicalDatabaseHash(
                finalSnapshotPath,
                excludeVolatileTimestamps: true);
            Require(
                string.Equals(databaseBusinessHashBefore, databaseBusinessHashAfter, StringComparison.OrdinalIgnoreCase),
                "Disposable canonical business state did not return to its baseline after the scenario.");
            Require(
                cleanReadiness ||
                string.Equals(Sha256(seedDatabase), seedDatabaseHash, StringComparison.OrdinalIgnoreCase),
                "Explicit source seed changed during disposable automation.");
            if (landedCostCapture is not null)
            {
                var evidence = new LandedCostAutomationEvidence(
                    "3dpiceland.landed-cost-automation-evidence.v1",
                    IOPath.GetFileName(root),
                    landedCostCapture.PurchaseOrderId,
                    landedCostCapture.MaterialId,
                    landedCostCapture.InventoryItemId,
                    landedCostCapture.InvoiceCurrency,
                    landedCostCapture.LandedCurrency,
                    landedCostCapture.ConversionRate,
                    landedCostCapture.RateSource,
                    landedCostCapture.ObservationDate,
                    landedCostCapture.FetchedAtUtc,
                    landedCostCapture.CalculatedAtUtc,
                    landedCostCapture.CalculationVersion,
                    landedCostCapture.RestartCheckpoints,
                    landedCostCapture.PurchaseOrderStateSha256,
                    landedCostCapture.InventoryStateSha256,
                    landedCostCapture.MaterialStateSha256,
                    landedCostCapture.UsageStateSha256,
                    landedCostCapture.QuoteStateSha256,
                    databaseBusinessHashBefore,
                    databaseBusinessHashAfter,
                    true);
                IOFile.WriteAllText(
                    IOPath.Combine(root, "evidence", "landed-cost-evidence.json"),
                    JsonSerializer.Serialize(evidence, new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                    }),
                    new UTF8Encoding(false));
            }
            Record("database-hash", true, databaseHashAfter);
            Record("database-business-state-hash", true, databaseBusinessHashAfter);
            WriteResult(
                root,
                "PASS",
                executable,
                seedDatabase,
                seedDatabaseHash,
                databaseHashBefore,
                databaseHashAfter,
                databaseBusinessHashBefore,
                databaseBusinessHashAfter,
                null);
            return 0;
        }
        catch (Exception ex)
        {
            try
            {
                if (application is { HasExited: false })
                {
                    application.Kill(entireProcessTree: true);
                    application.WaitForExit(15000);
                }
            }
            catch
            {
            }
            var databaseHashAfter = TrySha256(databasePath);
            if (root is not null)
                WriteResult(
                    root,
                    "FAIL",
                    executable,
                    seedDatabase,
                    seedDatabaseHash,
                    databaseHashBefore,
                    databaseHashAfter,
                    databaseBusinessHashBefore,
                    string.Empty,
                    ex.ToString());
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static string CreateDisposableProfile(
        string executable,
        string seedDatabase,
        bool cleanReadiness,
        bool reportGenerationAuthorized,
        bool materialCrudAuthorized,
        bool landedCostWorkflowAuthorized,
        bool recoveryAuthorized,
        bool updaterAuthorized,
        out string markerPath,
        out string databasePath,
        out string materialCrudId,
        out string landedCostPurchaseOrderId,
        out string landedCostMaterialId,
        out string landedCostInventoryItemId)
    {
        var allowedRoot = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-Automation");
        IODirectory.CreateDirectory(allowedRoot);
        var profileId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N")[..8];
        materialCrudId = "AUT" + profileId[^8..];
        landedCostPurchaseOrderId = "AUT-PO-" + profileId[^8..];
        landedCostMaterialId = "AUT-MAT-" + profileId[^8..];
        landedCostInventoryItemId = "AUT-INV-" + profileId[^8..];
        var root = IOPath.Combine(allowedRoot, profileId);
        var databaseFolder = IOPath.Combine(root, "database");
        var preferencesFolder = IOPath.Combine(root, "preferences");
        var outputFolder = IOPath.Combine(root, "output");
        var evidenceFolder = IOPath.Combine(root, "evidence");
        foreach (var folder in new[] { databaseFolder, preferencesFolder, outputFolder, evidenceFolder })
            IODirectory.CreateDirectory(folder);

        databasePath = IOPath.Combine(databaseFolder, "filamentdb.sqlite");
        if (!cleanReadiness)
        {
            IOFile.Copy(seedDatabase, databasePath, overwrite: false);
            IOFile.Copy(
                seedDatabase,
                IOPath.Combine(databaseFolder, "3DPIceland-Automation-Seed-Evidence.bak"),
                overwrite: false);
        }
        markerPath = IOPath.Combine(root, MarkerFileName);
        var manifest = new
        {
            profileId,
            purpose = cleanReadiness ? "clean-readiness" : "verification",
            rootPath = root,
            databaseFolder,
            preferencesFolder,
            outputFolder,
            evidenceFolder,
            expectedExecutableSha256 = Sha256(executable),
            productionAndFtpsBlocked = true,
            updatesBlocked = true,
            reportGenerationAuthorized,
            materialCrudAuthorized,
            materialCrudId,
            landedCostWorkflowAuthorized,
            landedCostPurchaseOrderId,
            landedCostMaterialId,
            landedCostInventoryItemId,
            recoveryAuthorized,
            updaterAuthorized
        };
        IOFile.WriteAllText(markerPath, JsonSerializer.Serialize(manifest, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }), new UTF8Encoding(false));
        return root;
    }

    private static void RunUpdaterAcceptance(
        string root,
        string sourceExecutable,
        string updaterExecutable,
        string markerPath,
        int exitedApplicationProcessId)
    {
        Require(IOFile.Exists(updaterExecutable), "Updater executable not found.");
        var sourceDirectory = IOPath.GetDirectoryName(sourceExecutable)
                              ?? throw new InvalidOperationException("Application source directory is unavailable.");
        var portableDirectory = IOPath.Combine(root, "portable-app");
        CopyDirectory(sourceDirectory, portableDirectory);
        var applicationRelativePath = IOPath.GetFileName(sourceExecutable);
        var portableExecutable = IOPath.Combine(portableDirectory, applicationRelativePath);
        Require(IOFile.Exists(portableExecutable), "Disposable portable application executable is missing.");
        var candidateFileVersionText = FileVersionInfo.GetVersionInfo(portableExecutable).FileVersion;
        Require(
            Version.TryParse(candidateFileVersionText, out var candidateFileVersion),
            "Candidate Windows file version is unavailable.");
        var candidateReleaseVersion = candidateFileVersion!.Revision >= 0
            ? candidateFileVersion.ToString(4)
            : candidateFileVersion.ToString(3);
        var previousReleaseVersion = candidateFileVersion.Revision > 0
            ? new Version(
                candidateFileVersion.Major,
                candidateFileVersion.Minor,
                candidateFileVersion.Build,
                candidateFileVersion.Revision - 1).ToString(4)
            : candidateFileVersion.Build > 0
                ? new Version(
                    candidateFileVersion.Major,
                    candidateFileVersion.Minor,
                    candidateFileVersion.Build - 1).ToString(3)
                : candidateFileVersion.Minor > 0
                    ? new Version(
                        candidateFileVersion.Major,
                        candidateFileVersion.Minor - 1,
                        0).ToString(3)
                    : new Version(
                        candidateFileVersion.Major - 1,
                        0,
                        0).ToString(3);
        var nextReleaseVersion = candidateFileVersion.Revision >= 0
            ? new Version(
                candidateFileVersion.Major,
                candidateFileVersion.Minor,
                candidateFileVersion.Build,
                candidateFileVersion.Revision + 1).ToString(4)
            : new Version(
                candidateFileVersion.Major,
                candidateFileVersion.Minor,
                candidateFileVersion.Build + 1).ToString(3);
        var governedFiles = IODirectory.EnumerateFiles(
                portableDirectory,
                "*",
                SearchOption.AllDirectories)
            .Select(path => IOPath.GetRelativePath(portableDirectory, path).Replace('\\', '/'))
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
        Require(governedFiles.Contains(applicationRelativePath, StringComparer.Ordinal),
            "Portable application executable is not governed.");
        var baselineHashes = HashGovernedFiles(portableDirectory, governedFiles);

        var successRoot = IOPath.Combine(root, "transactions", "success");
        var successStaging = IOPath.Combine(successRoot, "staging");
        CopyDirectory(portableDirectory, successStaging);
        var success = CreateUpdaterRequest(
            "automation-success-" + Guid.NewGuid().ToString("N"),
            portableDirectory,
            successStaging,
            successRoot,
            markerPath,
            applicationRelativePath,
            governedFiles,
            previousReleaseVersion,
            candidateReleaseVersion,
            exitedApplicationProcessId);
        var successExit = RunUpdater(updaterExecutable, successRoot, success);
        Require(successExit == 0, $"Disposable updater success returned exit code {successExit}.");
        var successState = ReadTransactionState(success.StatePath);
        Require(successState.Phase == "Committed", "Disposable updater success did not reach Committed.");
        var acknowledgement = JsonSerializer.Deserialize<ApplicationUpdateHealthAcknowledgement>(
            IOFile.ReadAllText(success.HealthAcknowledgementPath));
        Require(
            acknowledgement is not null &&
            acknowledgement.TransactionId == success.TransactionId &&
            acknowledgement.ReleaseVersion == success.NewVersion,
            "Exact-build health acknowledgement is missing or mismatched.");
        ClosePortableApplication(portableExecutable);
        var committedHashes = HashGovernedFiles(portableDirectory, governedFiles);
        Require(HashMapsEqual(baselineHashes, committedHashes),
            "Success scenario changed bytes despite an identical staged build.");

        var failureRoot = IOPath.Combine(root, "transactions", "rollback");
        var failureStaging = IOPath.Combine(failureRoot, "staging");
        CopyDirectory(portableDirectory, failureStaging);
        IOFile.WriteAllText(
            IOPath.Combine(failureStaging, applicationRelativePath),
            "INTENTIONALLY INVALID DISPOSABLE AUTOMATION EXECUTABLE",
            new UTF8Encoding(false));
        var beforeRollbackHashes = HashGovernedFiles(portableDirectory, governedFiles);
        var failure = CreateUpdaterRequest(
            "automation-rollback-" + Guid.NewGuid().ToString("N"),
            portableDirectory,
            failureStaging,
            failureRoot,
            markerPath,
            applicationRelativePath,
            governedFiles,
            candidateReleaseVersion,
            nextReleaseVersion,
            exitedApplicationProcessId);
        var failureExit = RunUpdater(updaterExecutable, failureRoot, failure);
        Require(failureExit == 1, $"Disposable updater failure returned unexpected exit code {failureExit}.");
        var failureState = ReadTransactionState(failure.StatePath);
        Require(failureState.Phase == "RolledBack", "Failed disposable update did not reach RolledBack.");
        ClosePortableApplication(portableExecutable);
        var afterRollbackHashes = HashGovernedFiles(portableDirectory, governedFiles);
        Require(HashMapsEqual(beforeRollbackHashes, afterRollbackHashes),
            "Rollback did not restore the exact pre-update portable file set.");

        var evidence = new
        {
            schema = "3dpiceland-automation-updater-evidence-v1",
            status = "PASS",
            liveDirectory = portableDirectory,
            ownerApplicationTargeted = false,
            ownerDatabaseTargeted = false,
            productionAndFtpsBlocked = true,
            governedFileCount = governedFiles.Count,
            success = new
            {
                success.TransactionId,
                successState.Phase,
                success.StatePath,
                success.HealthAcknowledgementPath,
                rollbackDirectory = success.RollbackDirectory,
                exactBuildRelease = acknowledgement!.ReleaseVersion
            },
            rollback = new
            {
                failure.TransactionId,
                failureState.Phase,
                failure.StatePath,
                rollbackDirectory = failure.RollbackDirectory,
                exactFileSetRestored = true
            },
            hashes = afterRollbackHashes
        };
        var evidencePath = IOPath.Combine(root, "evidence", "updater-evidence.json");
        IOFile.WriteAllText(
            evidencePath,
            JsonSerializer.Serialize(evidence, new JsonSerializerOptions { WriteIndented = true }),
            new UTF8Encoding(false));
        Record(
            "updater-commit-health",
            true,
            $"Committed {governedFiles.Count} files; exact release {acknowledgement.ReleaseVersion}");
        Record(
            "updater-failure-rollback",
            true,
            $"RolledBack; {governedFiles.Count} pre-update SHA-256 values restored");
    }

    private static ApplicationUpdateTransactionRequest CreateUpdaterRequest(
        string transactionId,
        string liveDirectory,
        string stagingDirectory,
        string transactionRoot,
        string markerPath,
        string applicationRelativePath,
        List<string> governedFiles,
        string previousVersion,
        string newVersion,
        int exitedApplicationProcessId)
    {
        IODirectory.CreateDirectory(transactionRoot);
        var request = new ApplicationUpdateTransactionRequest
        {
            TransactionId = transactionId,
            LiveDirectory = liveDirectory,
            StagingDirectory = stagingDirectory,
            RollbackDirectory = IOPath.Combine(transactionRoot, "rollback"),
            StatePath = IOPath.Combine(transactionRoot, "transaction-state.json"),
            PreviousVersion = previousVersion,
            NewVersion = newVersion,
            ReleaseCode = "GUARDED-UPDATER-ACCEPTANCE",
            SignatureAlgorithm = "AUTOMATION-DISPOSABLE-ONLY",
            SigningKeyFingerprint = "NO-PRODUCTION-SIGNING-KEY",
            DatabaseBackupPath = IOPath.Combine(
                IOPath.GetDirectoryName(markerPath)!,
                "database",
                "3DPIceland-Automation-Seed-Evidence.bak"),
            ApplicationRelativePath = applicationRelativePath,
            WaitForProcessId = exitedApplicationProcessId,
            HealthAcknowledgementPath = IOPath.Combine(transactionRoot, "health-ack.json"),
            AutomationProfilePath = markerPath,
            HealthTimeoutSeconds = 30,
            MinimumDatabaseSchema = 0,
            MaximumDatabaseSchema = int.MaxValue,
            GovernedFiles = governedFiles
        };
        IOFile.WriteAllText(
            IOPath.Combine(transactionRoot, "request.json"),
            JsonSerializer.Serialize(request, new JsonSerializerOptions { WriteIndented = true }),
            new UTF8Encoding(false));
        return request;
    }

    private static int RunUpdater(
        string updaterExecutable,
        string transactionRoot,
        ApplicationUpdateTransactionRequest request)
    {
        var requestPath = IOPath.Combine(transactionRoot, "request.json");
        using var updater = Process.Start(new ProcessStartInfo(updaterExecutable)
        {
            UseShellExecute = false,
            WorkingDirectory = transactionRoot
        }.WithArgument("--apply", requestPath))
            ?? throw new InvalidOperationException("Updater helper did not start.");
        if (!updater.WaitForExit((int)TimeSpan.FromMinutes(3).TotalMilliseconds))
        {
            updater.Kill(entireProcessTree: true);
            throw new TimeoutException("Updater helper did not finish.");
        }
        return updater.ExitCode;
    }

    private static ApplicationUpdateTransactionState ReadTransactionState(string path) =>
        JsonSerializer.Deserialize<ApplicationUpdateTransactionState>(IOFile.ReadAllText(path))
        ?? throw new InvalidOperationException("Updater transaction state is unreadable.");

    private static void CopyDirectory(string source, string destination)
    {
        IODirectory.CreateDirectory(destination);
        foreach (var directory in IODirectory.EnumerateDirectories(source, "*", SearchOption.AllDirectories)
                     .Where(path => !IsGeneratedRuntimePath(source, path)))
            IODirectory.CreateDirectory(IOPath.Combine(
                destination,
                IOPath.GetRelativePath(source, directory)));
        foreach (var file in IODirectory.EnumerateFiles(source, "*", SearchOption.AllDirectories)
                     .Where(path => !IsGeneratedRuntimePath(source, path)))
        {
            var target = IOPath.Combine(destination, IOPath.GetRelativePath(source, file));
            IODirectory.CreateDirectory(IOPath.GetDirectoryName(target)!);
            IOFile.Copy(file, target, overwrite: true);
        }
    }

    private static bool IsGeneratedRuntimePath(string root, string path) =>
        IOPath.GetRelativePath(root, path)
            .Split(IOPath.DirectorySeparatorChar, IOPath.AltDirectorySeparatorChar)
            .Any(segment => segment.EndsWith(".exe.WebView2", StringComparison.OrdinalIgnoreCase));

    private static SortedDictionary<string, string> HashGovernedFiles(
        string root,
        IEnumerable<string> governedFiles)
    {
        var result = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var relativePath in governedFiles)
            result[relativePath] = Sha256(IOPath.Combine(
                root,
                relativePath.Replace('/', IOPath.DirectorySeparatorChar)));
        return result;
    }

    private static bool HashMapsEqual(
        IReadOnlyDictionary<string, string> first,
        IReadOnlyDictionary<string, string> second) =>
        first.Count == second.Count &&
        first.All(item =>
            second.TryGetValue(item.Key, out var value) &&
            string.Equals(item.Value, value, StringComparison.OrdinalIgnoreCase));

    private static void ClosePortableApplication(string executable)
    {
        var fullExecutable = IOPath.GetFullPath(executable);
        var deadline = DateTime.UtcNow.AddSeconds(45);
        while (DateTime.UtcNow < deadline)
        {
            var matches = Process.GetProcesses()
                .Where(process =>
                {
                    try
                    {
                        return string.Equals(
                            IOPath.GetFullPath(process.MainModule?.FileName ?? string.Empty),
                            fullExecutable,
                            StringComparison.OrdinalIgnoreCase);
                    }
                    catch
                    {
                        return false;
                    }
                })
                .ToList();
            if (matches.Count == 0)
            {
                Thread.Sleep(200);
                continue;
            }
            foreach (var process in matches)
            {
                try
                {
                    var window = WaitForElement(
                        AutomationElement.RootElement,
                        new AndCondition(
                            new PropertyCondition(AutomationElement.ProcessIdProperty, process.Id),
                            new PropertyCondition(AutomationElement.AutomationIdProperty, "MainWindow")),
                        "disposable updated application");
                    CloseWindow(window, process.Id);
                    if (!process.WaitForExit(15000))
                        throw new TimeoutException("Disposable updated application did not close.");
                }
                finally
                {
                    process.Dispose();
                }
            }
            return;
        }
        throw new TimeoutException("Disposable updated application process was not found.");
    }

    private static AutomationElement FindById(AutomationElement root, string automationId) =>
        WaitForElement(root, new PropertyCondition(AutomationElement.AutomationIdProperty, automationId), automationId);

    private static void RunCrudAction(
        AutomationElement main,
        int processId,
        string automationId,
        string expectedStatus)
    {
        Invoke(FindById(main, automationId), processId);
        var status = FindById(main, "AutomationCrudStatus");
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ElementTimeout)
        {
            AssertNoUnexpectedWindows(processId, "MainWindow");
            if (string.Equals(status.Current.Name, expectedStatus, StringComparison.Ordinal)) return;
            Thread.Sleep(150);
        }
        throw new TimeoutException($"Timed out waiting for CRUD status {expectedStatus}.");
    }

    private static void RunRecoveryAction(
        AutomationElement main,
        int processId,
        string automationId,
        string expectedStatus)
    {
        Invoke(FindById(main, automationId), processId);
        var status = FindById(main, "AutomationRecoveryStatus");
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ReportTimeout)
        {
            AssertNoUnexpectedWindows(processId, "MainWindow");
            if (string.Equals(status.Current.Name, expectedStatus, StringComparison.Ordinal)) return;
            Thread.Sleep(150);
        }
        throw new TimeoutException($"Timed out waiting for recovery status {expectedStatus}.");
    }

    private static void RunLandedCostAction(
        AutomationElement main,
        int processId,
        string automationId,
        string expectedStatus)
    {
        Invoke(FindById(main, automationId), processId);
        var status = FindById(main, "AutomationLandedCostStatus");
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ElementTimeout)
        {
            AssertNoUnexpectedWindows(processId, "MainWindow");
            if (string.Equals(status.Current.Name, expectedStatus, StringComparison.Ordinal)) return;
            Thread.Sleep(150);
        }
        throw new TimeoutException(
            $"Timed out waiting for landed-cost status {expectedStatus}.");
    }

    private static int ValidateRecoveryBackupArtifacts(string root, string databasePath)
    {
        var databaseFolder = IOPath.Combine(root, "database");
        var paths = IODirectory.EnumerateFiles(databaseFolder, "*", SearchOption.TopDirectoryOnly)
            .Where(path => !string.Equals(IOPath.GetFullPath(path), IOPath.GetFullPath(databasePath),
                StringComparison.OrdinalIgnoreCase))
            .Where(path => string.Equals(IOPath.GetExtension(path), ".bak", StringComparison.OrdinalIgnoreCase) ||
                           string.Equals(IOPath.GetExtension(path), ".sqlite", StringComparison.OrdinalIgnoreCase))
            .ToList();
        Require(paths.Any(path => string.Equals(IOPath.GetExtension(path), ".bak", StringComparison.OrdinalIgnoreCase)),
            "Recovery evidence has no presentation .bak backup.");
        Require(paths.Any(path => string.Equals(IOPath.GetExtension(path), ".sqlite", StringComparison.OrdinalIgnoreCase)),
            "Recovery evidence has no legacy .sqlite compatibility backup.");
        foreach (var path in paths)
        {
            using var connection = new SqliteConnection($"Data Source={path};Mode=ReadOnly;Pooling=False");
            connection.Open();
            using var integrity = connection.CreateCommand();
            integrity.CommandText = "PRAGMA integrity_check;";
            Require(string.Equals(integrity.ExecuteScalar()?.ToString(), "ok", StringComparison.OrdinalIgnoreCase),
                $"Recovery backup failed integrity verification: {IOPath.GetFileName(path)}");
            Record("recovery-artifact", true,
                $"{IOPath.GetFileName(path)}; {new FileInfo(path).Length} bytes; sha256={Sha256(path)}");
        }
        return paths.Count;
    }

    private static (Process Process, AutomationElement Main) RestartApplication(
        Process application,
        string executable,
        string markerPath)
    {
        var currentMain = WaitForElement(
            AutomationElement.RootElement,
            new AndCondition(
                new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                new PropertyCondition(AutomationElement.AutomationIdProperty, "MainWindow")),
            "Main window before restart");
        CloseWindow(currentMain, application.Id);
        if (!application.WaitForExit(15000))
            throw new TimeoutException("Application did not complete controlled CRUD restart.");
        var restarted = Process.Start(new ProcessStartInfo(executable)
        {
            UseShellExecute = false,
            WorkingDirectory = IOPath.GetDirectoryName(executable)!
        }.WithArgument("--automation-profile", markerPath))
            ?? throw new InvalidOperationException("Application process did not restart.");
        var main = WaitForElement(
            AutomationElement.RootElement,
            new AndCondition(
                new PropertyCondition(AutomationElement.ProcessIdProperty, restarted.Id),
                new PropertyCondition(AutomationElement.AutomationIdProperty, "MainWindow")),
            "Main window after restart");
        AssertNoUnexpectedWindows(restarted.Id, "MainWindow");
        return (restarted, main);
    }

    private static AutomationElement WaitForElement(AutomationElement root, Condition condition, string label)
    {
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ElementTimeout)
        {
            var element = root.FindFirst(TreeScope.Descendants | TreeScope.Children, condition);
            if (element is not null) return element;
            Thread.Sleep(150);
        }
        throw new TimeoutException($"Timed out waiting for {label}.");
    }

    private static void SelectTab(AutomationElement main, string automationId, int processId)
    {
        var tab = FindById(main, automationId);
        DemandOwned(tab, processId);
        if (!tab.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var pattern))
            throw new InvalidOperationException($"{automationId} does not expose SelectionItemPattern.");
        ((SelectionItemPattern)pattern).Select();
        Thread.Sleep(100);
        AssertNoUnexpectedWindows(processId, "MainWindow");
    }

    private static void InvokeWebsiteMenuNavigation(AutomationElement main, int processId)
    {
        Expand(FindById(main, "WebsiteMenu"), processId);
        Invoke(FindById(main, "OpenWebsiteExportTab"), processId);
        Thread.Sleep(100);
        var websiteTab = FindById(main, "WebsiteExportTab");
        Require(
            websiteTab.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var selectionPattern) &&
            ((SelectionItemPattern)selectionPattern).Current.IsSelected,
            "Website menu action did not select the supported Website Export tab.");
        AssertNoUnexpectedWindows(processId, "MainWindow");
    }

    private static void OpenContextHelpAndRequireTitle(
        AutomationElement main,
        int processId,
        string expectedTitle)
    {
        Expand(FindById(main, "HelpMenu"), processId);
        Invoke(FindById(main, "OpenContextHelp"), processId);
        var help = WaitForElement(
            AutomationElement.RootElement,
            new AndCondition(
                new PropertyCondition(AutomationElement.ProcessIdProperty, processId),
                new PropertyCondition(AutomationElement.AutomationIdProperty, "HelpWindow")),
            "contextual Help");
        AssertNoUnexpectedWindows(processId, "MainWindow", "HelpWindow");
        Require(
            string.Equals(
                FindById(help, "HelpSectionTitle").Current.Name,
                expectedTitle,
                StringComparison.Ordinal),
            $"Contextual Help did not open '{expectedTitle}'.");
        CloseWindow(help, processId);
    }

    private static void Invoke(AutomationElement element, int processId)
    {
        DemandOwned(element, processId);
        if (!element.TryGetCurrentPattern(InvokePattern.Pattern, out var pattern))
            throw new InvalidOperationException($"{element.Current.AutomationId} does not expose InvokePattern.");
        ((InvokePattern)pattern).Invoke();
    }

    private static void Expand(AutomationElement element, int processId)
    {
        DemandOwned(element, processId);
        if (!element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var pattern))
            throw new InvalidOperationException($"{element.Current.AutomationId} does not expose ExpandCollapsePattern.");
        ((ExpandCollapsePattern)pattern).Expand();
    }

    private static void CloseWindow(AutomationElement window, int processId)
    {
        DemandOwned(window, processId);
        if (!window.TryGetCurrentPattern(WindowPattern.Pattern, out var pattern))
            throw new InvalidOperationException("Owned window does not expose WindowPattern.");
        ((WindowPattern)pattern).Close();
    }

    private static void DemandOwned(AutomationElement element, int processId)
    {
        if (element.Current.ProcessId != processId)
            throw new InvalidOperationException("Automation input target is outside the owned application process.");
    }

    private static void AssertNoUnexpectedWindows(int processId, params string[] allowedIds)
    {
        var allowed = allowedIds.ToHashSet(StringComparer.Ordinal);
        for (var attempt = 0; attempt < 11; attempt++)
        {
            var unexpected = FindUnexpectedOwnedWindow(processId, allowed);
            if (unexpected is null) return;
            var (id, name) = unexpected.Value;
            var anonymousTransient = string.IsNullOrWhiteSpace(id) && string.IsNullOrWhiteSpace(name);
            if (!anonymousTransient || attempt == 10)
                throw new InvalidOperationException(
                    $"Unexpected dialog/window blocked the run: AutomationId '{id}', name '{name}'.");
            Thread.Sleep(100);
        }
    }

    private static (string Id, string Name)? FindUnexpectedOwnedWindow(
        int processId,
        IReadOnlySet<string> allowedIds)
    {
        var windows = AutomationElement.RootElement.FindAll(
            TreeScope.Children,
            new PropertyCondition(AutomationElement.ProcessIdProperty, processId));
        foreach (AutomationElement window in windows)
        {
            try
            {
                var id = window.Current.AutomationId;
                if (!allowedIds.Contains(id))
                    return (id, window.Current.Name);
            }
            catch (ElementNotAvailableException)
            {
                // A UIA popup disappeared between enumeration and inspection.
            }
        }
        return null;
    }

    private static void CaptureWindow(AutomationElement window, string path)
    {
        var bounds = window.Current.BoundingRectangle;
        if (bounds.IsEmpty || bounds.Width < 1 || bounds.Height < 1)
            throw new InvalidOperationException("Owned window has no capturable bounds.");
        using var bitmap = new Bitmap((int)Math.Ceiling(bounds.Width), (int)Math.Ceiling(bounds.Height));
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen((int)bounds.Left, (int)bounds.Top, 0, 0, bitmap.Size);
        bitmap.Save(path, ImageFormat.Png);
    }

    private static void WaitForFile(string path, int processId, params string[] allowedWindowIds)
    {
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ElementTimeout)
        {
            if (IOFile.Exists(path) && new FileInfo(path).Length > 0) return;
            AssertNoUnexpectedWindows(processId, allowedWindowIds);
            Thread.Sleep(100);
        }
        throw new TimeoutException("Timed out waiting for automation evidence.");
    }

    private static void WaitForReportCompletion(AutomationElement main, int processId)
    {
        var summary = FindById(main, "ReportExportSummary");
        var log = FindById(main, "ReportPreviewLog");
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ReportTimeout)
        {
            AssertNoUnexpectedWindows(processId, "MainWindow", "ReportPrintHostWindow");
            var status = summary.Current.Name;
            if (status.StartsWith("Public report package built:", StringComparison.Ordinal))
                return;
            if (status.Contains("failed", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Public report package failed: {log.GetCurrentPropertyValue(ValuePattern.ValueProperty)}");
            Thread.Sleep(250);
        }
        throw new TimeoutException("Timed out waiting for the public report package.");
    }

    private static int ValidateReportArtifacts(string profileRoot)
    {
        const string previewFolder = "public-report-preview";
        const string screenThemeMarker = "3DP-PUBLIC-REPORT-SCREEN-THEME-v42.14-r3";
        var root = IOPath.GetFullPath(IOPath.Combine(profileRoot, "output", previewFolder));
        var catalogPath = IOPath.Combine(root, "report-catalog.json");
        var catalog = JsonDocument.Parse(IOFile.ReadAllText(catalogPath));
        var reports = catalog.RootElement.GetProperty("publicData").GetProperty("Reports").EnumerateArray().ToList();
        var types = reports.Select(report => report.GetProperty("ReportType").GetString() ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var required in new[]
                 {
                     "material-summary", "material-engineering", "comparison",
                     "manufacturer", "test-session", "printing-recommendation"
                 })
            Require(types.Contains(required), $"Report catalog is missing required type {required}.");

        var relativePaths = reports
            .SelectMany(report => new[]
            {
                report.GetProperty("Html").GetString(),
                report.GetProperty("Pdf").GetString(),
                report.GetProperty("Metadata").GetString()
            })
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => path!)
            .Concat(new[]
            {
                "index.html", "manifest.txt", "report-catalog.json", "source-fingerprint.json",
                "assets/3dp-iceland-labs-logo-pdf.jpg", "assets/3dp-iceland-labs-icon.ico"
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
        var rootPrefix = root.TrimEnd(IOPath.DirectorySeparatorChar) + IOPath.DirectorySeparatorChar;
        var artifacts = new List<ArtifactEvidence>();
        foreach (var relative in relativePaths)
        {
            Require(
                !IOPath.IsPathRooted(relative) &&
                !relative.Contains('\\') &&
                relative.Split('/').All(segment => segment.Length > 0 && segment is not "." and not ".."),
                $"Unsafe report route: {relative}");
            var fullPath = IOPath.GetFullPath(
                IOPath.Combine(root, relative.Replace('/', IOPath.DirectorySeparatorChar)));
            Require(fullPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase), $"Report route escaped output root: {relative}");
            Require(IOFile.Exists(fullPath), $"Report artifact is missing: {relative}");
            var info = new FileInfo(fullPath);
            Require(info.Length > 0, $"Report artifact is empty: {relative}");
            if (relative.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                var html = IOFile.ReadAllText(fullPath);
                Require(
                    relative == "index.html"
                        ? html.Contains("3DP-PUBLIC-REPORT-PORTFOLIO-v42.8", StringComparison.Ordinal)
                        : html.Contains(screenThemeMarker, StringComparison.Ordinal),
                    $"Report HTML marker is missing: {relative}");
            }
            if (relative.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = IOFile.OpenRead(fullPath);
                var header = new byte[5];
                Require(stream.Read(header, 0, header.Length) == header.Length &&
                        Encoding.ASCII.GetString(header) == "%PDF-", $"Invalid PDF header: {relative}");
            }
            if (relative.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                using (JsonDocument.Parse(IOFile.ReadAllText(fullPath))) { }
            artifacts.Add(new ArtifactEvidence(relative, info.Length, Sha256(fullPath)));
        }

        var evidenceFolder = IOPath.Combine(profileRoot, "evidence");
        var representative = reports
            .Where(report =>
                report.GetProperty("ReportType").GetString() is "material-summary" or "material-engineering")
            .Take(2)
            .SelectMany(report => new[]
            {
                report.GetProperty("Html").GetString(),
                report.GetProperty("Pdf").GetString()
            })
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToList();
        var result = new
        {
            schema = "3dpiceland-automation-report-evidence-v1",
            status = "PASS",
            manualVisualReview = "REQUIRED",
            previewRoot = root,
            reportTypes = types.OrderBy(type => type).ToList(),
            catalogEntries = reports.Count,
            artifacts,
            representativeReviewPaths = representative
        };
        IOFile.WriteAllText(
            IOPath.Combine(evidenceFolder, "report-artifacts.json"),
            JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }),
            new UTF8Encoding(false));
        IOFile.WriteAllLines(
            IOPath.Combine(evidenceFolder, "report-artifacts.txt"),
            new[]
            {
                "Status: PASS",
                "Manual visual review: REQUIRED",
                $"Catalog entries: {reports.Count}",
                $"Verified artifacts: {artifacts.Count}"
            }.Concat(artifacts.Select(item => $"{item.Sha256} {item.Bytes} {item.RelativePath}")),
            new UTF8Encoding(false));
        return artifacts.Count;
    }

    private static string Sha256(string path) =>
        Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(path)));

    private static string TrySha256(string path)
    {
        if (!IOFile.Exists(path)) return string.Empty;
        try
        {
            return Sha256(path);
        }
        catch
        {
            return "UNAVAILABLE-FILE-LOCKED";
        }
    }

    private static void CreateConsistentSnapshot(string sourcePath, string destinationPath)
    {
        if (IOFile.Exists(destinationPath)) IOFile.Delete(destinationPath);
        using var source = new SqliteConnection($"Data Source={sourcePath};Mode=ReadOnly;Pooling=False");
        using var destination = new SqliteConnection($"Data Source={destinationPath};Pooling=False");
        source.Open();
        destination.Open();
        source.BackupDatabase(destination);
    }

    private static void RecordDatabaseEvidence(string root, string databasePath, string label)
    {
        var snapshot = IOPath.Combine(root, "evidence", label + ".sqlite");
        CreateConsistentSnapshot(databasePath, snapshot);
        Record(
            label,
            true,
            $"logical={ComputeLogicalDatabaseHash(snapshot)}; " +
            $"business={ComputeLogicalDatabaseHash(snapshot, excludeVolatileTimestamps: true)}");
    }

    private static void ValidateUsagePersistence(
        string databasePath,
        string materialId,
        int expectedEvents,
        int expectedReversals,
        int expectedReplacements,
        string? expectedRemainingWeight)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();

        int Count(string predicate)
        {
            using var command = connection.CreateCommand();
            command.CommandText =
                $"SELECT COUNT(*) FROM UsageEvents WHERE MaterialId=$material {predicate};";
            command.Parameters.AddWithValue("$material", materialId);
            return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
        }

        var events = Count(string.Empty);
        var reversals = Count("AND EntryKind='Reversal'");
        var replacements = Count("AND EntryKind='Replacement'");
        Require(
            events == expectedEvents &&
            reversals == expectedReversals &&
            replacements == expectedReplacements,
            $"Usage persistence mismatch for {materialId}: " +
            $"events {events}/{expectedEvents}, reversals {reversals}/{expectedReversals}, " +
            $"replacements {replacements}/{expectedReplacements}.");

        string remainingDetail;
        using (var inventory = connection.CreateCommand())
        {
            inventory.CommandText = """
                                    SELECT RemainingWeightG
                                    FROM InventorySpoolItems
                                    WHERE MaterialId=$material;
                                    """;
            inventory.Parameters.AddWithValue("$material", materialId);
            var remaining = inventory.ExecuteScalar()?.ToString();
            Require(
                string.Equals(remaining, expectedRemainingWeight, StringComparison.Ordinal),
                $"Usage inventory mismatch for {materialId}: " +
                $"{remaining ?? "<absent>"}/{expectedRemainingWeight ?? "<absent>"}.");
            remainingDetail = remaining ?? "absent";
        }

        Record(
            "usage-persistence-" + expectedEvents.ToString(CultureInfo.InvariantCulture),
            true,
            $"{materialId}: events={events}; reversals={reversals}; " +
            $"replacements={replacements}; remaining={remainingDetail}");
    }

    private static void ValidateQuotePersistence(
        string databasePath,
        string materialId,
        int expectedQuotes)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT COUNT(*)
                              FROM PrintJobQuotes
                              WHERE MaterialId=$material
                                AND QuoteId=$quote
                                AND CalculationVersion='v1'
                                AND SnapshotJson LIKE '%3dpiceland-print-job-quote-v1%';
                              """;
        command.Parameters.AddWithValue("$material", materialId);
        command.Parameters.AddWithValue("$quote", "AUT-Q-" + materialId);
        var actual = Convert.ToInt32(
            command.ExecuteScalar(), CultureInfo.InvariantCulture);
        Require(actual == expectedQuotes,
            $"Immutable Quote persistence mismatch for {materialId}: " +
            $"{actual}/{expectedQuotes}.");
        Record("quote-persistence-" + expectedQuotes, true,
            $"{materialId}: immutable v1 snapshot count={actual}");
    }

    private static void ValidateUsageWorkspace(
        AutomationElement main,
        int processId,
        string materialId,
        int expectedEvents)
    {
        SelectTab(main, "UsageTab", processId);
        _ = FindById(main, "UsageMaterialSelector");
        _ = FindById(main, "UsageInventorySelector");
        _ = FindById(main, "SaveUsageEvent");
        _ = FindById(main, "BeginUsageCorrection");
        _ = FindById(main, "UsageLedgerGrid");
        var status = FindById(main, "UsageStatus")
            .GetCurrentPropertyValue(AutomationElement.NameProperty)?
            .ToString() ?? string.Empty;
        Require(
            status.Contains(
                $"{expectedEvents} immutable event row(s)",
                StringComparison.Ordinal) &&
            status.Contains("private", StringComparison.OrdinalIgnoreCase),
            $"Usage UI did not expose the expected private ledger state for {materialId}: {status}");
        Record(
            "usage-workspace-" + expectedEvents.ToString(CultureInfo.InvariantCulture),
            true,
            $"{materialId}: bounded controls present; {status}");
    }

    private static void ValidateUsageAnalytics(
        AutomationElement main,
        string materialId,
        int expectedEffectiveEvents,
        int expectedLedgerRows,
        string expectedFilament,
        string expectedPrintTime)
    {
        string Name(string automationId) =>
            FindById(main, automationId)
                .GetCurrentPropertyValue(AutomationElement.NameProperty)?
                .ToString() ?? string.Empty;
        var effective = Name("UsageEffectiveEvents");
        var ledger = Name("UsageLedgerRows");
        var filament = Name("UsageFilamentTotal");
        var printTime = Name("UsagePrintTimeTotal");
        var coverage = Name("UsageCoverage");
        Require(
            effective == expectedEffectiveEvents.ToString(CultureInfo.CurrentCulture) &&
            ledger == expectedLedgerRows.ToString(CultureInfo.CurrentCulture) &&
            filament.Replace(',', '.') == expectedFilament &&
            printTime == expectedPrintTime &&
            coverage.Contains(
                $"Grams 1/{expectedEffectiveEvents}",
                StringComparison.Ordinal),
            $"Usage analytics mismatch for {materialId}: effective={effective}; " +
            $"ledger={ledger}; filament={filament}; print={printTime}; coverage={coverage}");
        Record(
            "usage-analytics-" + expectedLedgerRows.ToString(CultureInfo.InvariantCulture),
            true,
            $"{materialId}: effective={effective}; ledger={ledger}; " +
            $"filament={filament}; print={printTime}; coverage={coverage}");
    }

    private static string ComputeLogicalDatabaseHash(
        string databasePath,
        bool excludeVolatileTimestamps = false)
    {
        using var canonical = new MemoryStream();
        using var writer = new BinaryWriter(canonical, Encoding.UTF8, leaveOpen: true);
        using var connection = new SqliteConnection($"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();

        var tables = new List<(string Name, string Sql)>();
        using (var schema = connection.CreateCommand())
        {
            schema.CommandText = """
                SELECT name, COALESCE(sql, '')
                FROM sqlite_schema
                WHERE type = 'table' AND (name NOT LIKE 'sqlite_%' OR name = 'sqlite_sequence')
                ORDER BY name COLLATE BINARY;
                """;
            using var reader = schema.ExecuteReader();
            while (reader.Read()) tables.Add((reader.GetString(0), reader.GetString(1)));
        }

        writer.Write(tables.Count);
        foreach (var table in tables)
        {
            writer.Write(table.Name);
            writer.Write(table.Sql);
            var columns = new List<string>();
            using (var info = connection.CreateCommand())
            {
                info.CommandText = $"PRAGMA table_info({QuoteIdentifier(table.Name)});";
                using var reader = info.ExecuteReader();
                while (reader.Read())
                {
                    var column = reader.GetString(1);
                    if (excludeVolatileTimestamps &&
                        string.Equals(column, "UpdatedAtUtc", StringComparison.Ordinal))
                        continue;
                    columns.Add(column);
                }
            }

            writer.Write(columns.Count);
            foreach (var column in columns) writer.Write(column);
            using var rows = connection.CreateCommand();
            var columnList = string.Join(", ", columns.Select(QuoteIdentifier));
            rows.CommandText = columns.Count == 0
                ? $"SELECT 1 FROM {QuoteIdentifier(table.Name)};"
                : $"SELECT {columnList} FROM {QuoteIdentifier(table.Name)} ORDER BY {columnList};";
            using var rowReader = rows.ExecuteReader();
            while (rowReader.Read())
            {
                writer.Write((byte)0x52);
                for (var index = 0; index < rowReader.FieldCount; index++)
                    WriteCanonicalValue(writer, rowReader.GetValue(index));
            }
        }

        writer.Flush();
        canonical.Position = 0;
        return Convert.ToHexString(SHA256.HashData(canonical));
    }

    private static void WriteCanonicalValue(BinaryWriter writer, object value)
    {
        switch (value)
        {
            case DBNull:
                writer.Write((byte)0);
                break;
            case long integer:
                writer.Write((byte)1);
                writer.Write(integer);
                break;
            case double real:
                writer.Write((byte)2);
                writer.Write(BitConverter.DoubleToInt64Bits(real));
                break;
            case string text:
                writer.Write((byte)3);
                writer.Write(text);
                break;
            case byte[] blob:
                writer.Write((byte)4);
                writer.Write(blob.Length);
                writer.Write(blob);
                break;
            default:
                throw new InvalidOperationException($"Unsupported SQLite value type: {value.GetType().FullName}");
        }
    }

    private static string QuoteIdentifier(string identifier) =>
        "\"" + identifier.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";

    private static long CountRows(string databasePath, string tableName)
    {
        using var connection = new SqliteConnection($"Data Source={databasePath};Mode=ReadOnly");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"SELECT COUNT(*) FROM {QuoteIdentifier(tableName)};";
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static void RequireExactRowCount(
        string databasePath,
        string tableName,
        string keyColumn,
        string keyValue,
        int expected)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            $"SELECT COUNT(*) FROM {QuoteIdentifier(tableName)} " +
            $"WHERE {QuoteIdentifier(keyColumn)} = $value;";
        command.Parameters.AddWithValue("$value", keyValue);
        var actual = Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture);
        Require(actual == expected,
            $"{tableName}.{keyColumn} expected {expected} exact row(s), found {actual}.");
    }

    private static Dictionary<string, string> ReadExactRow(
        string databasePath,
        string tableName,
        string keyColumn,
        string keyValue)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            $"SELECT * FROM {QuoteIdentifier(tableName)} " +
            $"WHERE {QuoteIdentifier(keyColumn)} = $value;";
        command.Parameters.AddWithValue("$value", keyValue);
        using var reader = command.ExecuteReader();
        Require(reader.Read(), $"{tableName}.{keyColumn} exact row is missing.");
        var result = Enumerable.Range(0, reader.FieldCount).ToDictionary(
            reader.GetName,
            index => reader.IsDBNull(index)
                ? string.Empty
                : Convert.ToString(reader.GetValue(index), CultureInfo.InvariantCulture) ?? string.Empty,
            StringComparer.Ordinal);
        Require(!reader.Read(), $"{tableName}.{keyColumn} exact identity is not unique.");
        return result;
    }

    private static string HashExactRow(
        string databasePath,
        string tableName,
        string keyColumn,
        string keyValue) =>
        HashRows(databasePath, tableName,
            $"{QuoteIdentifier(keyColumn)} = $value", keyValue);

    private static string HashTable(string databasePath, string tableName) =>
        HashRows(databasePath, tableName, null, null);

    private static RecoveryTableHashes RecoveryHistoricalHashes(string databasePath) =>
        new(
            HashTable(databasePath, "PurchaseOrders"),
            HashTable(databasePath, "PurchaseOrderLines"),
            HashTable(databasePath, "InventorySpoolItems"),
            HashTable(databasePath, "NativeMaterialManagerRows"),
            HashTable(databasePath, "UsageEvents"),
            HashTable(databasePath, "PrintJobQuotes"));

    private static void ValidateSchema37StartupMigration(
        string sourcePath,
        string migratedPath)
    {
        Require(ReadSchemaVersion(sourcePath) == 37,
            "Migration source fixture is not schema v37.");
        Require(ReadSchemaVersion(migratedPath) == 38,
            "Disposable startup did not migrate the fixture to schema v38.");
        foreach (var table in new[]
                 {
                     "PurchaseOrders",
                     "PurchaseOrderLines",
                     "InventorySpoolItems",
                     "NativeMaterialManagerRows",
                     "UsageEvents",
                     "PrintJobQuotes"
                 })
        {
            var sourceColumns = ReadTableColumns(sourcePath, table);
            var migratedColumns = ReadTableColumns(migratedPath, table);
            var sharedColumns = sourceColumns
                .Where(column => migratedColumns.Contains(column, StringComparer.Ordinal))
                .ToArray();
            Require(sharedColumns.Length == sourceColumns.Count,
                $"{table} lost a schema-v37 column during startup migration.");
            Require(
                HashSelectedColumns(sourcePath, table, sharedColumns) ==
                HashSelectedColumns(migratedPath, table, sharedColumns),
                $"{table} shared schema-v37 values changed during startup migration.");
        }

        var purchaseColumns = ReadTableColumns(migratedPath, "PurchaseOrders");
        var inventoryColumns = ReadTableColumns(migratedPath, "InventorySpoolItems");
        foreach (var column in LandedCostMigrationColumns)
        {
            Require(purchaseColumns.Contains(column, StringComparer.Ordinal),
                $"PurchaseOrders.{column} was not added by the v38 migration.");
            Require(inventoryColumns.Contains(column, StringComparer.Ordinal),
                $"InventorySpoolItems.{column} was not added by the v38 migration.");
        }

        using var connection = new SqliteConnection(
            $"Data Source={migratedPath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT COUNT(*)
            FROM InventorySpoolItems
            WHERE LandedCostCurrency = PurchaseCurrency
              AND LandedCostConversionRate = '1'
              AND LandedCostRateSource = 'Legacy transaction-currency landed cost'
              AND LandedCostCalculationVersion = 'legacy-v1';
            """;
        var compatibleRows = Convert.ToInt32(
            command.ExecuteScalar(), CultureInfo.InvariantCulture);
        using var countCommand = connection.CreateCommand();
        countCommand.CommandText = "SELECT COUNT(*) FROM InventorySpoolItems;";
        var totalRows = Convert.ToInt32(
            countCommand.ExecuteScalar(), CultureInfo.InvariantCulture);
        Require(compatibleRows == totalRows,
            "Schema-v37 Inventory landed-cost compatibility backfill is incomplete.");
        Record("schema37-shared-column-stability", true,
            "Purchase Orders, lines, Inventory, Materials, Usage and Quotes match the v37 source");
        Record("schema38-landed-cost-backfill", true,
            $"{compatibleRows}/{totalRows} legacy Inventory row(s) received compatibility provenance");
    }

    private static int ReadSchemaVersion(string databasePath)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT Value FROM AppMeta WHERE Key = 'SchemaVersion' LIMIT 1;";
        return int.Parse(
            Convert.ToString(command.ExecuteScalar(), CultureInfo.InvariantCulture)
            ?? throw new InvalidOperationException("SchemaVersion is missing."),
            CultureInfo.InvariantCulture);
    }

    private static List<string> ReadTableColumns(
        string databasePath,
        string tableName)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({QuoteIdentifier(tableName)});";
        using var reader = command.ExecuteReader();
        var columns = new List<string>();
        while (reader.Read())
            columns.Add(reader.GetString(1));
        Require(columns.Count > 0, $"{tableName} is missing.");
        return columns;
    }

    private static string HashSelectedColumns(
        string databasePath,
        string tableName,
        IReadOnlyCollection<string> columns)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        var columnList = string.Join(", ", columns.Select(QuoteIdentifier));
        using var command = connection.CreateCommand();
        command.CommandText =
            $"SELECT {columnList} FROM {QuoteIdentifier(tableName)} ORDER BY {columnList};";
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        using var rows = command.ExecuteReader();
        while (rows.Read())
        {
            for (var index = 0; index < rows.FieldCount; index++)
                WriteCanonicalValue(writer, rows.GetValue(index));
        }
        writer.Flush();
        return Convert.ToHexString(SHA256.HashData(stream.ToArray()));
    }

    private static string HashRows(
        string databasePath,
        string tableName,
        string? whereClause,
        string? parameterValue)
    {
        using var connection = new SqliteConnection(
            $"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        var columns = new List<string>();
        using (var info = connection.CreateCommand())
        {
            info.CommandText = $"PRAGMA table_info({QuoteIdentifier(tableName)});";
            using var reader = info.ExecuteReader();
            while (reader.Read())
            {
                var column = reader.GetString(1);
                if (!string.Equals(column, "UpdatedAtUtc", StringComparison.Ordinal))
                    columns.Add(column);
            }
        }
        using var command = connection.CreateCommand();
        var columnList = string.Join(", ", columns.Select(QuoteIdentifier));
        command.CommandText =
            $"SELECT {columnList} FROM {QuoteIdentifier(tableName)}" +
            (whereClause is null ? string.Empty : " WHERE " + whereClause) +
            $" ORDER BY {columnList};";
        if (parameterValue is not null)
            command.Parameters.AddWithValue("$value", parameterValue);
        using var stream = new MemoryStream();
        using var writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true);
        using var rows = command.ExecuteReader();
        while (rows.Read())
        {
            for (var index = 0; index < rows.FieldCount; index++)
                WriteCanonicalValue(writer, rows.GetValue(index));
        }
        writer.Flush();
        return Convert.ToHexString(SHA256.HashData(stream.ToArray()));
    }

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Record(string name, bool passed, string detail) =>
        Steps.Add(new StepResult(name, passed ? "PASS" : "FAIL", detail, DateTimeOffset.UtcNow));

    private static void WriteResult(
        string root,
        string status,
        string executable,
        string seedDatabase,
        string seedDatabaseHash,
        string databaseHashBefore,
        string databaseHashAfter,
        string databaseBusinessHashBefore,
        string databaseBusinessHashAfter,
        string? error)
    {
        var evidence = IOPath.Combine(root, "evidence");
        IODirectory.CreateDirectory(evidence);
        var result = new
        {
            schema = "3dpiceland-automation-run-v1",
            status,
            scenario = CurrentScenario,
            safetyPolicy = new
            {
                productionBlocked = true,
                ftpsBlocked = true,
                updatesBlocked = true,
                ownerDatabaseAutoSelection = false,
                unexpectedDialogsBlocked = true,
                inputConfinedToOwnedProcess = true,
                reportGenerationAuthorized = string.Equals(CurrentScenario, "reports", StringComparison.Ordinal)
                ,materialCrudAuthorized = string.Equals(CurrentScenario, "crud", StringComparison.Ordinal)
                ,landedCostWorkflowAuthorized =
                    string.Equals(CurrentScenario, "landed-cost", StringComparison.Ordinal) ||
                    string.Equals(CurrentScenario, "recovery", StringComparison.Ordinal)
                ,recoveryAuthorized = string.Equals(CurrentScenario, "recovery", StringComparison.Ordinal)
                ,updaterAuthorized = string.Equals(CurrentScenario, "updater", StringComparison.Ordinal)
            },
            executable,
            seedDatabase,
            seedDatabaseHash,
            databaseHashBefore,
            databaseHashAfter,
            databaseBusinessHashBefore,
            databaseBusinessHashAfter,
            steps = Steps,
            error,
            completedAtUtc = DateTimeOffset.UtcNow
        };
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        IOFile.WriteAllText(IOPath.Combine(evidence, "run-result.json"), json, new UTF8Encoding(false));
        IOFile.WriteAllLines(
            IOPath.Combine(evidence, "run-result.txt"),
            new[] { $"Status: {status}" }.Concat(Steps.Select(step => $"{step.Status} {step.Name}: {step.Detail}"))
                .Append(error ?? string.Empty),
            new UTF8Encoding(false));
    }

    private sealed record StepResult(string Name, string Status, string Detail, DateTimeOffset AtUtc);
    private sealed record ArtifactEvidence(string RelativePath, long Bytes, string Sha256);
    private sealed record LandedCostEvidenceCapture(
        string PurchaseOrderId,
        string MaterialId,
        string InventoryItemId,
        string InvoiceCurrency,
        string LandedCurrency,
        string ConversionRate,
        string RateSource,
        string ObservationDate,
        string FetchedAtUtc,
        string CalculatedAtUtc,
        string CalculationVersion,
        IReadOnlyList<string> RestartCheckpoints,
        string PurchaseOrderStateSha256,
        string InventoryStateSha256,
        string MaterialStateSha256,
        string UsageStateSha256,
        string QuoteStateSha256);
    private sealed record LandedCostAutomationEvidence(
        string Schema,
        string ProfileId,
        string PurchaseOrderId,
        string MaterialId,
        string InventoryItemId,
        string InvoiceCurrency,
        string LandedCurrency,
        string ConversionRate,
        string RateSource,
        string ObservationDate,
        string FetchedAtUtc,
        string CalculatedAtUtc,
        string CalculationVersion,
        IReadOnlyList<string> RestartCheckpoints,
        string PurchaseOrderStateSha256,
        string InventoryStateSha256,
        string MaterialStateSha256,
        string UsageStateSha256,
        string QuoteStateSha256,
        string BaselineBusinessStateSha256,
        string FinalBusinessStateSha256,
        bool CleanupMatchedBaseline);
    private sealed record RecoveryTableHashes(
        string PurchaseOrders,
        string PurchaseOrderLines,
        string Inventory,
        string Materials,
        string Usage,
        string Quotes);
    private static readonly string[] LandedCostMigrationColumns =
    [
        "LandedCostCurrency",
        "LandedCostConversionRate",
        "LandedCostRateSource",
        "LandedCostRateObservationDate",
        "LandedCostRateFetchedAtUtc",
        "LandedCostCalculatedAtUtc",
        "LandedCostCalculationVersion"
    ];

    private sealed record RunnerOptions(
        string ApplicationPath,
        string SeedDatabasePath,
        string Scenario,
        string UpdaterPath)
    {
        public static RunnerOptions Parse(string[] args)
        {
            string Required(string name)
            {
                var index = Array.IndexOf(args, name);
                if (index < 0 || index + 1 >= args.Length)
                    throw new ArgumentException($"Required argument missing: {name}");
                return args[index + 1];
            }
            var scenarioIndex = Array.IndexOf(args, "--scenario");
            var scenario = scenarioIndex >= 0 && scenarioIndex + 1 < args.Length
                ? args[scenarioIndex + 1].Trim().ToLowerInvariant()
                : "smoke";
            if (scenario is not ("smoke" or "reports" or "crud" or "landed-cost" or "migration" or "recovery" or "updater" or "clean"))
                throw new ArgumentException(
                    "--scenario must be smoke, reports, crud, landed-cost, migration, recovery, updater or clean.");
            var updater = scenario == "updater" ? Required("--updater") : string.Empty;
            var seed = scenario == "clean" ? Optional("--seed-database") : Required("--seed-database");
            return new RunnerOptions(Required("--app"), seed, scenario, updater);

            string Optional(string name)
            {
                var index = Array.IndexOf(args, name);
                return index >= 0 && index + 1 < args.Length ? args[index + 1] : string.Empty;
            }
        }
    }

    private static ProcessStartInfo WithArgument(this ProcessStartInfo start, string name, string value)
    {
        start.ArgumentList.Add(name);
        start.ArgumentList.Add(value);
        return start;
    }

    private static string ReadRequiredArgument(string[] args, string name)
    {
        var index = Array.IndexOf(args, name);
        if (index < 0 || index + 1 >= args.Length ||
            args[index + 1].StartsWith("--", StringComparison.Ordinal))
            throw new ArgumentException($"Required argument missing: {name}");
        var value = args[index + 1];
        if (name == "--plan-sha256" &&
            (value.Length != 64 || !value.All(char.IsAsciiHexDigit)))
            throw new ArgumentException("--plan-sha256 must be exactly 64 hexadecimal characters.");
        return value;
    }

    private static IEnumerable<string> ReadRepeatedArguments(string[] args, string name)
    {
        for (var index = 0; index < args.Length - 1; index++)
            if (string.Equals(args[index], name, StringComparison.Ordinal))
            {
                if (args[index + 1].StartsWith("--", StringComparison.Ordinal))
                    throw new ArgumentException($"Argument value missing: {name}");
                yield return args[index + 1];
            }
    }
}
