using System.IO;

namespace FilamentDbApp.Models;

public sealed class DatabaseBackupInfo
{
    public required string FilePath { get; init; }
    public required string IntegrityResult { get; init; }
    public int SchemaVersion { get; init; }
    public long FileSizeBytes { get; init; }
    public int Materials { get; init; }
    public int TensileSamples { get; init; }
    public int ImpactSamples { get; init; }
    public int StiffnessRows { get; init; }
    public int SettingsRows { get; init; }
    public string BackupKind { get; init; } = "SQLite backup";
    public string CompatibilityStatus { get; init; } = "Unverified";
    public string CompatibilityDetail { get; init; } = string.Empty;
    public bool MigrationDryRunPassed { get; init; }
    public bool CanRestore { get; init; }
    public DateTime ModifiedAt { get; init; }
    public string FileName => Path.GetFileName(FilePath);
    public bool IsIntegrityValid => string.Equals(IntegrityResult, "ok", StringComparison.OrdinalIgnoreCase);
}

public sealed class DatabaseRestoreResult
{
    public required string SourceBackupPath { get; init; }
    public required string RecoveryBackupPath { get; init; }
    public required DatabaseBackupInfo RestoredDatabase { get; init; }
}
