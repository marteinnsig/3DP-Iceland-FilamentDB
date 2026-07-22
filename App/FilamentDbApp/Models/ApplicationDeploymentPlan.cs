namespace FilamentDbApp.Models;

public sealed class ApplicationDeploymentPlan
{
    public const string CurrentSchema = "3dpiceland.application-deployment-plan.v1";
    public string Schema { get; set; } = CurrentSchema;
    public string ReleaseVersion { get; set; } = string.Empty;
    public string ReleaseCode { get; set; } = string.Empty;
    public string GeneratedAtUtc { get; set; } = string.Empty;
    public string SourcePackageSha256 { get; set; } = string.Empty;
    public List<ApplicationDeploymentFile> Files { get; set; } = new();
}

public sealed class ApplicationDeploymentFile
{
    public string Kind { get; set; } = string.Empty;
    public string LocalFile { get; set; } = string.Empty;
    public string StableRemotePath { get; set; } = string.Empty;
    public string VersionedRemotePath { get; set; } = string.Empty;
    public long Bytes { get; set; }
    public string Sha256 { get; set; } = string.Empty;
}

public sealed record ApplicationDeploymentPublishResult(string BackupFolder, IReadOnlyList<string> PublishedPaths);
public sealed record ApplicationDeploymentVerificationResult(bool Passed, string Detail);
