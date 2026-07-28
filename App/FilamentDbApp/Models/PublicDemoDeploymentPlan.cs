namespace FilamentDbApp.Models;

public sealed class PublicDemoDeploymentPlan
{
    public const string CurrentSchema = "3dpiceland.public-demo-deployment-plan.v1";
    public string Schema { get; set; } = CurrentSchema;
    public string Release { get; set; } = string.Empty;
    public string LocalFile { get; set; } = string.Empty;
    public string VersionedRemotePath { get; set; } = string.Empty;
    public string StableRemotePath { get; set; } = string.Empty;
    public long Bytes { get; set; }
    public string Sha256 { get; set; } = string.Empty;
}

public sealed record PublicDemoDeploymentResult(
    string BackupFolder,
    IReadOnlyList<string> PublishedPaths,
    IReadOnlyList<string> VerifiedUrls);

public sealed record PublicDemoDeploymentVerificationResult(bool Passed, string Detail);
