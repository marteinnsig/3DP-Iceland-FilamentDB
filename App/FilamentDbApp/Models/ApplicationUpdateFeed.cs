namespace FilamentDbApp.Models;

public sealed class ApplicationUpdateFeed
{
    public const string CurrentSchema = "3dpiceland.application-update-feed.v1";
    public string Schema { get; set; } = CurrentSchema;
    public string PackageUrl { get; set; } = string.Empty;
    public long PackageBytes { get; set; }
    public string PackageSha256 { get; set; } = string.Empty;
    public ApplicationUpdateManifest Manifest { get; set; } = new();
}

public sealed record RemoteUpdateDiscoveryResult(bool Available, string Status, string Detail, ApplicationUpdateFeed? Feed);
public sealed record RemoteUpdateDownloadResult(string PackagePath, ApplicationUpdateReadinessResult Readiness);
