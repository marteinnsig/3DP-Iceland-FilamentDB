namespace FilamentDbApp.Services;

public enum RuntimeProfileKind
{
    OwnerProduction,
    DisposableVerification,
    CleanReadiness
}

public sealed record RuntimeProfileDescriptor(
    RuntimeProfileKind Kind,
    string ProfileId,
    string VisibleIdentity,
    bool IsDisposable,
    bool OwnerDatabaseAllowed,
    bool ProductionAndFtpsAllowed,
    bool UpdatesAllowed,
    string DatabaseOwnership,
    string PreferencesOwnership,
    string OutputOwnership,
    string CredentialOwnership,
    string UpdateTransactionOwnership,
    string EvidenceOwnership,
    string CleanupOwnership)
{
    public string CapabilitySummary =>
        $"Owner database: {Allowed(OwnerDatabaseAllowed)}; " +
        $"Production/FTPS: {Allowed(ProductionAndFtpsAllowed)}; " +
        $"updates: {Allowed(UpdatesAllowed)}";

    private static string Allowed(bool value) => value ? "ALLOWED" : "BLOCKED";
}

public static class RuntimeProfileContext
{
    public static RuntimeProfileDescriptor Current
    {
        get
        {
            var automation = AutomationRuntimeProfile.Current;
            return automation is null
                ? DescribeOwnerProduction()
                : automation.Purpose == AutomationRuntimeProfile.CleanReadinessPurpose
                    ? DescribeCleanReadiness(automation)
                    : DescribeDisposableVerification(automation);
        }
    }

    public static RuntimeProfileDescriptor DescribeOwnerProduction() =>
        new(
                RuntimeProfileKind.OwnerProduction,
                "owner-production",
                "OWNER / PRODUCTION",
                IsDisposable: false,
                OwnerDatabaseAllowed: true,
                ProductionAndFtpsAllowed: true,
                UpdatesAllowed: true,
                DatabaseOwnership: "Configured owner storage folder",
                PreferencesOwnership: "Owner LocalApplicationData preferences",
                OutputOwnership: "Owner-selected governed output paths",
                CredentialOwnership: "Owner Windows Credential Manager",
                UpdateTransactionOwnership: "Owner LocalApplicationData update transaction history",
                EvidenceOwnership: "Owner-selected governed diagnostic and release evidence",
                CleanupOwnership: "Owner retention and explicit governed cleanup");

    public static RuntimeProfileDescriptor DescribeDisposableVerification(
        AutomationRuntimeProfile automation) =>
        new(
            RuntimeProfileKind.DisposableVerification,
            automation.ProfileId,
            $"VERIFICATION / DISPOSABLE — {automation.ProfileId}",
            IsDisposable: true,
            OwnerDatabaseAllowed: false,
            ProductionAndFtpsAllowed: false,
            UpdatesAllowed: false,
            DatabaseOwnership: "Disposable manifest database folder",
            PreferencesOwnership: "Disposable manifest preferences folder",
            OutputOwnership: "Disposable manifest output folder",
            CredentialOwnership: "Owner credentials inaccessible",
            UpdateTransactionOwnership: "Disposable runner/profile transaction evidence only",
            EvidenceOwnership: "Disposable manifest evidence folder",
            CleanupOwnership: "Runner-owned dry-run plan; hash-reviewed apply removes only obsolete unpinned PASS profiles");

    public static RuntimeProfileDescriptor DescribeCleanReadiness(
        AutomationRuntimeProfile automation) =>
        new(
            RuntimeProfileKind.CleanReadiness,
            automation.ProfileId,
            $"CLEAN / READINESS — {automation.ProfileId}",
            IsDisposable: true,
            OwnerDatabaseAllowed: false,
            ProductionAndFtpsAllowed: false,
            UpdatesAllowed: false,
            DatabaseOwnership: "Seedless disposable manifest database folder",
            PreferencesOwnership: "Seedless disposable manifest preferences folder",
            OutputOwnership: "Seedless disposable manifest output folder",
            CredentialOwnership: "Owner credentials inaccessible",
            UpdateTransactionOwnership: "No owner history; disposable runner/profile evidence only",
            EvidenceOwnership: "Seedless disposable manifest evidence folder",
            CleanupOwnership: "Runner-owned dry-run plan; hash-reviewed apply removes only obsolete unpinned PASS profiles");
}
