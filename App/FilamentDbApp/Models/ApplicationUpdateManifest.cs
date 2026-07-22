namespace FilamentDbApp.Models;

public sealed class ApplicationUpdateManifest
{
    public const string CurrentSchema = "3dpiceland.application-update.v1";
    public string Schema { get; set; } = CurrentSchema;
    public string ReleaseVersion { get; set; } = string.Empty;
    public string ReleaseCode { get; set; } = string.Empty;
    public int MinimumDatabaseSchema { get; set; }
    public int MaximumDatabaseSchema { get; set; }
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string SignatureAlgorithm { get; set; } = "ECDSA-P256-SHA256";
    public List<ApplicationUpdateFile> Files { get; set; } = new();
    public string Signature { get; set; } = string.Empty;
}

public sealed class ApplicationUpdateFile
{
    public string Path { get; set; } = string.Empty;
    public long Length { get; set; }
    public string Sha256 { get; set; } = string.Empty;
}

public sealed record ApplicationUpdateReadinessResult(
    bool PackageReadable,
    bool ManifestValid,
    bool FileInventoryValid,
    bool HashesValid,
    bool SignatureValid,
    bool VersionValid,
    bool DatabaseSchemaValid,
    bool Ready,
    string Status,
    string Detail,
    ApplicationUpdateManifest? Manifest);

public sealed record ApplicationUpdateVerificationResult(bool Passed, string Detail);
