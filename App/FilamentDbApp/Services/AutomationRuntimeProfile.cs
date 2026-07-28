using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace FilamentDbApp.Services;

public sealed class AutomationRuntimeProfile
{
    public const string ArgumentName = "--automation-profile";
    public const string MarkerFileName = ".3dpiceland-disposable-profile.json";
    public const string VerificationPurpose = "verification";
    public const string CleanReadinessPurpose = "clean-readiness";

    public string ProfileId { get; init; } = string.Empty;
    public string Purpose { get; init; } = VerificationPurpose;
    public string RootPath { get; init; } = string.Empty;
    public string DatabaseFolder { get; init; } = string.Empty;
    public string PreferencesFolder { get; init; } = string.Empty;
    public string OutputFolder { get; init; } = string.Empty;
    public string EvidenceFolder { get; init; } = string.Empty;
    public string ExpectedExecutableSha256 { get; init; } = string.Empty;
    public bool ProductionAndFtpsBlocked { get; init; }
    public bool UpdatesBlocked { get; init; }
    public bool PublicDemoDataset { get; init; }
    public bool ReportGenerationAuthorized { get; init; }
    public bool MaterialCrudAuthorized { get; init; }
    public string MaterialCrudId { get; init; } = string.Empty;
    public bool LandedCostWorkflowAuthorized { get; init; }
    public string LandedCostPurchaseOrderId { get; init; } = string.Empty;
    public string LandedCostMaterialId { get; init; } = string.Empty;
    public string LandedCostInventoryItemId { get; init; } = string.Empty;
    public bool RecoveryAuthorized { get; init; }
    public bool UpdaterAuthorized { get; init; }

    public static AutomationRuntimeProfile? Current { get; private set; }
    public static bool IsActive => Current is not null;

    public static void Configure(string[] args)
    {
        var index = Array.IndexOf(args, ArgumentName);
        if (index < 0) return;
        if (index + 1 >= args.Length)
            throw new InvalidOperationException($"{ArgumentName} requires an absolute manifest path.");

        var manifestPath = IOPath.GetFullPath(args[index + 1]);
        var markerPath = IOPath.Combine(IOPath.GetDirectoryName(manifestPath) ?? string.Empty, MarkerFileName);
        if (!string.Equals(manifestPath, markerPath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"Automation profile must use the exact marker name {MarkerFileName}.");
        if (!IOFile.Exists(manifestPath))
            throw new FileNotFoundException("Disposable automation profile marker is missing.", manifestPath);

        var profile = JsonSerializer.Deserialize<AutomationRuntimeProfile>(
                          IOFile.ReadAllText(manifestPath),
                          new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                      ?? throw new InvalidOperationException("Automation profile JSON is invalid.");
        profile.Validate(manifestPath);
        Current = profile;
    }

    public static void DemandReportGenerationAuthorized()
    {
        if (IsActive && Current?.ReportGenerationAuthorized != true)
            throw new InvalidOperationException(
                "Report generation requires explicit authorization in the disposable automation scenario.");
    }

    public static void DemandMaterialCrudAuthorized(string materialId)
    {
        if (!IsActive ||
            Current?.MaterialCrudAuthorized != true ||
            string.IsNullOrWhiteSpace(Current.MaterialCrudId) ||
            !string.Equals(Current.MaterialCrudId, materialId, StringComparison.Ordinal))
            throw new InvalidOperationException(
                "Material CRUD requires explicit authorization for the exact disposable MaterialID.");
    }

    public static void DemandRecoveryAuthorized()
    {
        if (!IsActive || Current?.RecoveryAuthorized != true)
            throw new InvalidOperationException(
                "Backup and recovery automation requires explicit disposable scenario authorization.");
    }

    public static void DemandLandedCostWorkflowAuthorized(
        string purchaseOrderId,
        string materialId,
        string inventoryItemId)
    {
        if (Current?.MatchesLandedCostWorkflowAuthorization(
                purchaseOrderId,
                materialId,
                inventoryItemId) != true)
            throw new InvalidOperationException(
                "Landed-cost automation requires explicit authorization for the exact disposable identities.");
    }

    internal static bool IsSafeAutomationIdentifier(string? value) =>
        !string.IsNullOrWhiteSpace(value) &&
        value.All(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_');

    internal bool HasValidLandedCostWorkflowAuthorization() =>
        !LandedCostWorkflowAuthorized ||
        (IsSafeAutomationIdentifier(LandedCostPurchaseOrderId) &&
         IsSafeAutomationIdentifier(LandedCostMaterialId) &&
         IsSafeAutomationIdentifier(LandedCostInventoryItemId) &&
         !string.Equals(LandedCostPurchaseOrderId, LandedCostMaterialId, StringComparison.Ordinal) &&
         !string.Equals(LandedCostPurchaseOrderId, LandedCostInventoryItemId, StringComparison.Ordinal) &&
         !string.Equals(LandedCostMaterialId, LandedCostInventoryItemId, StringComparison.Ordinal));

    internal bool MatchesLandedCostWorkflowAuthorization(
        string purchaseOrderId,
        string materialId,
        string inventoryItemId) =>
        LandedCostWorkflowAuthorized &&
        HasValidLandedCostWorkflowAuthorization() &&
        string.Equals(LandedCostPurchaseOrderId, purchaseOrderId, StringComparison.Ordinal) &&
        string.Equals(LandedCostMaterialId, materialId, StringComparison.Ordinal) &&
        string.Equals(LandedCostInventoryItemId, inventoryItemId, StringComparison.Ordinal);

    private void Validate(string manifestPath)
    {
        if (string.IsNullOrWhiteSpace(ProfileId) ||
            !ProfileId.All(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_'))
            throw new InvalidOperationException("Automation ProfileId must be a non-empty safe identifier.");
        if (!ProductionAndFtpsBlocked || !UpdatesBlocked)
            throw new InvalidOperationException("Automation profiles must hard-block Production, FTPS and updates.");
        if (Purpose is not (VerificationPurpose or CleanReadinessPurpose))
            throw new InvalidOperationException("Automation profile purpose must be verification or clean-readiness.");
        if (Purpose == CleanReadinessPurpose &&
            (ReportGenerationAuthorized || MaterialCrudAuthorized || LandedCostWorkflowAuthorized ||
             RecoveryAuthorized || UpdaterAuthorized))
            throw new InvalidOperationException("Clean Readiness profiles cannot authorize mutating automation scenarios.");
        if (PublicDemoDataset &&
            (Purpose != VerificationPurpose || MaterialCrudAuthorized ||
             LandedCostWorkflowAuthorized || RecoveryAuthorized || UpdaterAuthorized))
            throw new InvalidOperationException(
                "Public demo profiles permit report output only; CRUD, landed-cost, recovery and updater authorization remain blocked.");
        if (MaterialCrudAuthorized &&
            (string.IsNullOrWhiteSpace(MaterialCrudId) ||
             !MaterialCrudId.All(ch => char.IsLetterOrDigit(ch) || ch is '-' or '_')))
            throw new InvalidOperationException("Authorized automation MaterialID must be a non-empty safe identifier.");
        if (!HasValidLandedCostWorkflowAuthorization())
            throw new InvalidOperationException(
                "Landed-cost automation requires three distinct non-empty safe disposable identities.");

        var allowedRoot = IOPath.GetFullPath(IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-Automation"))
            .TrimEnd(IOPath.DirectorySeparatorChar) + IOPath.DirectorySeparatorChar;
        var root = IOPath.GetFullPath(RootPath).TrimEnd(IOPath.DirectorySeparatorChar);
        if (!root.StartsWith(allowedRoot, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Automation profile root must stay below the dedicated temporary automation root.");
        if (!string.Equals(IOPath.GetDirectoryName(manifestPath), root, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Automation profile marker must be located directly in its declared root.");

        foreach (var path in new[] { DatabaseFolder, PreferencesFolder, OutputFolder, EvidenceFolder })
        {
            var fullPath = IOPath.GetFullPath(path);
            if (!fullPath.StartsWith(root + IOPath.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException("Every automation profile path must stay below its declared disposable root.");
        }

        var ownerDatabaseFolder = IOPath.GetFullPath(Data.LocalDatabase.GetConfiguredStorageFolder())
            .TrimEnd(IOPath.DirectorySeparatorChar);
        if (string.Equals(IOPath.GetFullPath(DatabaseFolder).TrimEnd(IOPath.DirectorySeparatorChar),
                ownerDatabaseFolder, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Automation database folder resolves to the canonical owner database folder.");

        var executable = Environment.ProcessPath
                         ?? throw new InvalidOperationException("Automation executable path is unavailable.");
        var executableHash = Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(executable)));
        if (!string.Equals(executableHash, ExpectedExecutableSha256, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Automation executable SHA-256 does not match the approved profile.");
    }
}
