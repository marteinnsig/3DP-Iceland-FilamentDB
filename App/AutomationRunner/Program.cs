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
        Process? application = null;
        string? root = null;
        string executable = string.Empty;
        string seedDatabase = string.Empty;
        string databasePath = string.Empty;
        string seedDatabaseHash = string.Empty;
        string databaseHashBefore = string.Empty;
        string databaseBusinessHashBefore = string.Empty;
        try
        {
            var options = RunnerOptions.Parse(args);
            CurrentScenario = options.Scenario;
            executable = IOPath.GetFullPath(options.ApplicationPath);
            seedDatabase = IOPath.GetFullPath(options.SeedDatabasePath);
            if (!IOFile.Exists(executable)) throw new FileNotFoundException("Application executable not found.", executable);
            if (!IOFile.Exists(seedDatabase)) throw new FileNotFoundException("Explicit seed database not found.", seedDatabase);

            root = CreateDisposableProfile(
                executable,
                seedDatabase,
                string.Equals(options.Scenario, "reports", StringComparison.Ordinal),
                string.Equals(options.Scenario, "crud", StringComparison.Ordinal),
                string.Equals(options.Scenario, "recovery", StringComparison.Ordinal),
                string.Equals(options.Scenario, "updater", StringComparison.Ordinal),
                out var markerPath,
                out databasePath,
                out var materialCrudId);
            seedDatabaseHash = Sha256(seedDatabase);
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
            Require(identityName.Contains("VERIFICATION / DISPOSABLE", StringComparison.Ordinal),
                "Disposable Verification runtime profile identity is not visible.");
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
                diagnosticsReport.Contains("Identity: VERIFICATION / DISPOSABLE", StringComparison.Ordinal) &&
                diagnosticsReport.Contains(
                    "Capabilities: Owner database: BLOCKED; Production/FTPS: BLOCKED; updates: BLOCKED",
                    StringComparison.Ordinal),
                "System Diagnostics did not expose runtime identity/capabilities, read-only evidence and the distinct mutating recalculation control.");
            Record("system-diagnostics-read-only-inspection", true,
                "Verified Disposable profile ownership plus refresh, integrity, recalculation and export boundaries without invoking them");
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
                assistantOutput.Contains("AI ASSISTANT", StringComparison.Ordinal) &&
                assistantOutput.Contains("Visible materials used:", StringComparison.Ordinal),
                "AI Assistant local full brief did not retain visible-scope evidence.");
            Record("ai-assistant-local-scope", true, assistantScope + " " + assistantMaterialIds);
            var collectionAction = FindById(main, "AiCollectionActionState").Current.Name;
            Require(
                collectionAction.StartsWith("Action: Create a new collection", StringComparison.Ordinal),
                "AI collection workflow read non-disposable collection state.");
            Invoke(FindById(main, "PreviewAiMaterialCollection"), application.Id);
            var collectionPreview = FindById(main, "AiAssistantOutput")
                .GetCurrentPropertyValue(ValuePattern.ValueProperty)?.ToString() ?? string.Empty;
            Require(
                collectionPreview.Contains("COLLECTION SAVE PREVIEW", StringComparison.Ordinal) &&
                collectionPreview.Contains("No data has been written.", StringComparison.Ordinal) &&
                collectionPreview.Contains("Unique MaterialIDs to save:", StringComparison.Ordinal),
                "AI collection preview did not expose its read-only exact MaterialID contract.");
            Record("ai-collection-preview", true, collectionAction);
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
            ((ValuePattern)helpValuePattern).SetValue("sends no payload to OpenAI");
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
            Require(
                verificationEvidence.RootElement.TryGetProperty("Passed", out var passedElement) &&
                passedElement.GetBoolean(),
                "Exported Full Data Verification reported FAIL.");
            CaptureWindow(verification, IOPath.Combine(root, "evidence", "verification-center.png"));
            Record("verification-export", true, "TXT/JSON evidence exported");

            CloseWindow(verification, application.Id);
            if (string.Equals(options.Scenario, "reports", StringComparison.Ordinal))
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
            else if (string.Equals(options.Scenario, "recovery", StringComparison.Ordinal))
            {
                RunRecoveryAction(main, application.Id, "AutomationRecoveryBackup", "BACKUPS-VERIFIED");
                var backupArtifacts = ValidateRecoveryBackupArtifacts(root, databasePath);
                Record("recovery-backup-catalog", true, $"{backupArtifacts} verified .bak/.sqlite artifacts");
                RunRecoveryAction(main, application.Id, "AutomationRecoveryExportExcel", "EXCEL-EXPORTED");
                var workbook = IOPath.Combine(root, "output", "3DPIceland-Automation-DisasterRecovery.xlsx");
                Require(IOFile.Exists(workbook) && new FileInfo(workbook).Length > 0,
                    "Governed Excel recovery package is missing.");
                Record("recovery-excel-package", true,
                    $"{new FileInfo(workbook).Length} bytes; sha256={Sha256(workbook)}");
                RunRecoveryAction(main, application.Id, "AutomationRecoveryMutate", "MUTATED");
                RecordDatabaseEvidence(root, databasePath, "recovery-after-mutation");
                RunRecoveryAction(main, application.Id, "AutomationRecoveryRestoreExcel", "EXCEL-RESTORED");
                RecordDatabaseEvidence(root, databasePath, "recovery-after-excel-restore");
                var restoreArtifacts = ValidateRecoveryBackupArtifacts(root, databasePath);
                Require(restoreArtifacts >= backupArtifacts + 2,
                    "Excel restore did not add both pre/post SQLite evidence backups.");
                Record("recovery-pre-post-evidence", true, $"{restoreArtifacts} verified backup artifacts");
                (application, main) = RestartApplication(application, executable, markerPath);
                CaptureWindow(main, IOPath.Combine(root, "evidence", "recovery-complete.png"));
                Record("recovery-restart", true, "Restored disposable profile restarted under the same manifest");
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
        bool reportGenerationAuthorized,
        bool materialCrudAuthorized,
        bool recoveryAuthorized,
        bool updaterAuthorized,
        out string markerPath,
        out string databasePath,
        out string materialCrudId)
    {
        var allowedRoot = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-Automation");
        IODirectory.CreateDirectory(allowedRoot);
        var profileId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N")[..8];
        materialCrudId = "AUT" + profileId[^8..];
        var root = IOPath.Combine(allowedRoot, profileId);
        var databaseFolder = IOPath.Combine(root, "database");
        var preferencesFolder = IOPath.Combine(root, "preferences");
        var outputFolder = IOPath.Combine(root, "output");
        var evidenceFolder = IOPath.Combine(root, "evidence");
        foreach (var folder in new[] { databaseFolder, preferencesFolder, outputFolder, evidenceFolder })
            IODirectory.CreateDirectory(folder);

        databasePath = IOPath.Combine(databaseFolder, "filamentdb.sqlite");
        IOFile.Copy(seedDatabase, databasePath, overwrite: false);
        IOFile.Copy(
            seedDatabase,
            IOPath.Combine(databaseFolder, "3DPIceland-Automation-Seed-Evidence.bak"),
            overwrite: false);
        markerPath = IOPath.Combine(root, MarkerFileName);
        var manifest = new
        {
            profileId,
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
        var candidateReleaseVersion = candidateFileVersion!.ToString(3);
        var previousReleaseVersion = candidateFileVersion.Build > 0
            ? new Version(candidateFileVersion.Major, candidateFileVersion.Minor, candidateFileVersion.Build - 1).ToString(3)
            : candidateFileVersion.Minor > 0
                ? new Version(candidateFileVersion.Major, candidateFileVersion.Minor - 1, 0).ToString(3)
                : new Version(candidateFileVersion.Major - 1, 0, 0).ToString(3);
        var nextReleaseVersion = new Version(
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
        var windows = AutomationElement.RootElement.FindAll(
            TreeScope.Children,
            new PropertyCondition(AutomationElement.ProcessIdProperty, processId));
        foreach (AutomationElement window in windows)
        {
            var id = window.Current.AutomationId;
            if (!allowed.Contains(id))
                throw new InvalidOperationException(
                    $"Unexpected dialog/window blocked the run: AutomationId '{id}', name '{window.Current.Name}'.");
        }
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
            if (scenario is not ("smoke" or "reports" or "crud" or "recovery" or "updater"))
                throw new ArgumentException("--scenario must be smoke, reports, crud, recovery or updater.");
            var updater = scenario == "updater" ? Required("--updater") : string.Empty;
            return new RunnerOptions(Required("--app"), Required("--seed-database"), scenario, updater);
        }
    }

    private static ProcessStartInfo WithArgument(this ProcessStartInfo start, string name, string value)
    {
        start.ArgumentList.Add(name);
        start.ArgumentList.Add(value);
        return start;
    }
}
