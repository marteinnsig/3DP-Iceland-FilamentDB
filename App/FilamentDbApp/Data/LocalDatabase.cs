using FilamentDbApp.Models;
using FilamentDbApp.Services;
using Microsoft.Data.Sqlite;
using System.Data;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    private const int SchemaVersion = 33;
    private const int MinimumStandaloneBackupSchemaVersion = 27;
    private const int MaxAutomaticBackups = 20;
    private const string AutomaticBackupPrefix = "filamentdb_";
    private const string AutomaticBackupExtension = ".sqlite";
    private const string PresentationBackupExtension = ".bak";
    private const string PresentationAutomaticBackupPrefix = "3DPIceland-Automatic-";
    private const string PresentationManualBackupPrefix = "3DPIceland-Manual-";
    private const string PresentationPreRestoreBackupPrefix = "3DPIceland-Pre-SQLite-Restore-";
    private const string PresentationPostRestoreBackupPrefix = "3DPIceland-Post-SQLite-Restore-";
    private const string PresentationPreExcelRestoreBackupPrefix = "3DPIceland-Pre-Excel-Restore-";
    private const string PresentationPostExcelRestoreBackupPrefix = "3DPIceland-Post-Excel-Restore-";
    private bool _requiredCanonicalMigrationBackupCreated;
    private static readonly string[] ExcelRecoveryTableInsertOrder =
    {
        "Manufacturers", "NativeMaterialManagerRows", "BaseMaterialCatalog", "NativeSettingsRows", "DeploymentSettings", "WebsiteTemplates", "VideoIdeaQueue", "Suppliers",
        "PurchaseOrders", "PurchaseOrderLines", "InventorySpoolItems", "PurchaseDocuments", "ExperimentDefinitions", "MaterialExperiments", "ExperimentalRuns", "ExperimentalMeasurements",
        "NativeTensileSamples", "NativeTensileResults", "NativeImpactSamples", "NativeStiffnessMeasurements", "NativeMeasurementNotes"
    };
    private static readonly string[] LegacyWorkbookTablesDropOrder =
    {
        "ExcelSheetCells", "ExcelSheetRows", "ExcelSheetColumns", "ExcelSheets",
        "TestSummaryValues", "StiffnessMeasurements", "ImpactSamples",
        "TensileSamples", "TensileResults", "MaterialAttributes", "LookupValues",
        "Materials", "Imports"
    };
    public string DatabasePath { get; }

    public LocalDatabase()
    {
        var folder = AutomationRuntimeProfile.Current?.DatabaseFolder ?? GetConfiguredStorageFolder();
        Directory.CreateDirectory(folder);
        DatabasePath = Path.Combine(folder, "filamentdb.sqlite");

        if (!AutomationRuntimeProfile.IsActive)
            CopyLegacyDatabaseToConfiguredFolderIfNeeded(folder);

        new ActiveDatabaseCompatibilityService().EnsureSupportedOrPreserve(DatabasePath, SchemaVersion);

        var legacyMaterialsImportRetirementPending = IOFile.Exists(DatabasePath) && LegacyMaterialsImportRetirementIsPending();
        var legacyWorkbookRetirementPending = IOFile.Exists(DatabasePath) && LegacyWorkbookTablesRetirementIsPending();
        var postRetirementBackupPending = IOFile.Exists(DatabasePath) && LegacyWorkbookPostRetirementBackupIsPending();
        var schemaUpgradePending = IOFile.Exists(DatabasePath) && DatabaseSchemaUpgradeIsPending();
        if (IOFile.Exists(DatabasePath) && (NativeMeasurementMigrationIsPending() || schemaUpgradePending || legacyMaterialsImportRetirementPending || legacyWorkbookRetirementPending))
            CreateRequiredBackupBeforeCanonicalMigration(retainAllEvidence: legacyMaterialsImportRetirementPending || legacyWorkbookRetirementPending);

        Initialize();
        if (schemaUpgradePending)
            CreateConsistentDatabaseBackup(AutomaticBackupPrefix);
        if ((legacyWorkbookRetirementPending || postRetirementBackupPending) && LegacyWorkbookTablesAreRetired())
            CreateAndRecordLegacyWorkbookPostRetirementBackup();
    }

    private LocalDatabase(string databasePath)
    {
        DatabasePath = databasePath;
        Initialize();
    }


    public string DatabaseFolder => Path.GetDirectoryName(DatabasePath) ?? string.Empty;

    public static string DefaultDatabaseFolder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "3DPIceland Labs", "FilamentDB");

    private static string SettingsFolder =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "3DPIcelandLabs", "FilamentDB");

    private static string SettingsPath => Path.Combine(SettingsFolder, "storage-folder.txt");

    private static string LegacyDatabaseFolder => SettingsFolder;

    public static string GetConfiguredStorageFolder()
    {
        try
        {
            if (File.Exists(SettingsPath))
            {
                var configured = File.ReadAllText(SettingsPath).Trim();
                if (!string.IsNullOrWhiteSpace(configured))
                {
                    return configured;
                }
            }
        }
        catch
        {
            // Fall back to Documents if settings cannot be read.
        }

        return DefaultDatabaseFolder;
    }

    public static void SetStorageFolder(string folder)
    {
        if (string.IsNullOrWhiteSpace(folder))
        {
            throw new ArgumentException("Storage folder cannot be empty.", nameof(folder));
        }

        Directory.CreateDirectory(folder);
        Directory.CreateDirectory(SettingsFolder);
        File.WriteAllText(SettingsPath, folder);
    }

    private static void CopyLegacyDatabaseToConfiguredFolderIfNeeded(string configuredFolder)
    {
        try
        {
            var configuredPath = Path.Combine(configuredFolder, "filamentdb.sqlite");
            var legacyPath = Path.Combine(LegacyDatabaseFolder, "filamentdb.sqlite");

            if (!File.Exists(configuredPath) && File.Exists(legacyPath) &&
                !string.Equals(Path.GetFullPath(configuredPath), Path.GetFullPath(legacyPath), StringComparison.OrdinalIgnoreCase))
            {
                Directory.CreateDirectory(configuredFolder);
                File.Copy(legacyPath, configuredPath, overwrite: false);
            }
        }
        catch
        {
            // Migration is best effort. The source remains untouched if the configured copy cannot be created.
        }
    }

    public void MoveDatabaseToFolder(string targetFolder)
    {
        if (string.IsNullOrWhiteSpace(targetFolder))
        {
            throw new ArgumentException("Target folder cannot be empty.", nameof(targetFolder));
        }

        Directory.CreateDirectory(targetFolder);
        var targetPath = Path.Combine(targetFolder, "filamentdb.sqlite");

        if (File.Exists(DatabasePath) && !string.Equals(Path.GetFullPath(DatabasePath), Path.GetFullPath(targetPath), StringComparison.OrdinalIgnoreCase))
        {
            if (File.Exists(targetPath))
            {
                var backupPath = targetPath + ".backup-" + DateTime.Now.ToString("yyyyMMdd-HHmmss", System.Globalization.CultureInfo.InvariantCulture);
                File.Copy(targetPath, backupPath, overwrite: false);
                File.Delete(targetPath);
            }

            File.Copy(DatabasePath, targetPath, overwrite: true);
        }

        SetStorageFolder(targetFolder);
    }


    public string BackupFolder => DatabaseFolder;

    public void CreateAutomaticBackupBeforeMajorChange()
    {
        CreateAutomaticBackupBeforeWrite();
    }

    private FileInfo CreateConsistentDatabaseBackup(string prefix)
    {
        if (!IOFile.Exists(DatabasePath) || new FileInfo(DatabasePath).Length == 0)
            throw new InvalidOperationException("The SQLite database is unavailable for backup.");

        IODirectory.CreateDirectory(BackupFolder);
        var backupPath = CreatePresentationBackupPath(prefix, DateTime.Now);
        var sourceBuilder = new SqliteConnectionStringBuilder(ConnectionString) { Pooling = false };
        using var source = new SqliteConnection(sourceBuilder.ToString());
        using var destination = new SqliteConnection($"Data Source={backupPath};Pooling=False");
        source.Open();
        destination.Open();
        source.BackupDatabase(destination);
        var inspection = InspectDatabaseFile(backupPath);
        if (!inspection.IsIntegrityValid)
        {
            try { IOFile.Delete(backupPath); } catch { }
            throw new InvalidOperationException("The new SQLite backup failed integrity verification: " + inspection.IntegrityResult);
        }
        return new FileInfo(backupPath);
    }

    private string CreatePresentationBackupPath(string legacyPrefix, DateTime createdAt)
    {
        var presentationPrefix = legacyPrefix switch
        {
            "filamentdb_manual_" => PresentationManualBackupPrefix,
            "filamentdb_pre_restore_" => PresentationPreRestoreBackupPrefix,
            "filamentdb_post_restore_" => PresentationPostRestoreBackupPrefix,
            "filamentdb_pre_excel_restore_" => PresentationPreExcelRestoreBackupPrefix,
            "filamentdb_post_excel_restore_" => PresentationPostExcelRestoreBackupPrefix,
            _ => PresentationAutomaticBackupPrefix
        };
        var stamp = createdAt.ToString("yyyy-MM-dd_HHmmss_fff", CultureInfo.InvariantCulture);
        var path = IOPath.Combine(BackupFolder, presentationPrefix + stamp + PresentationBackupExtension);
        return IOFile.Exists(path)
            ? IOPath.Combine(
                BackupFolder,
                presentationPrefix + stamp + "-" + Guid.NewGuid().ToString("N") + PresentationBackupExtension)
            : path;
    }

    private void CreateRequiredBackupBeforeCanonicalMigration(bool retainAllEvidence = false)
    {
        if (_requiredCanonicalMigrationBackupCreated) return;
        if (!File.Exists(DatabasePath) || new FileInfo(DatabasePath).Length == 0)
            throw new InvalidOperationException("The SQLite database is unavailable for the required migration backup.");
        CreateConsistentDatabaseBackup(AutomaticBackupPrefix);
        if (!retainAllEvidence) CleanupAutomaticBackups();
        _requiredCanonicalMigrationBackupCreated = true;
    }

    private bool NativeMeasurementMigrationIsPending()
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString); connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Value FROM AppMeta WHERE Key='NativeMeasurementsCanonicalV1' LIMIT 1;";
            return !string.Equals(command.ExecuteScalar()?.ToString(), "complete", StringComparison.Ordinal);
        }
        catch { return true; }
    }

    private bool DatabaseSchemaUpgradeIsPending()
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString); connection.Open();
            var current = ReadSchemaVersion(connection);
            return !int.TryParse(current, NumberStyles.Integer, CultureInfo.InvariantCulture, out var version) || version < SchemaVersion;
        }
        catch { return true; }
    }

    private bool LegacyMaterialsImportRetirementIsPending()
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name='MaterialsImport';";
            return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
        }
        catch { return true; }
    }

    public bool LegacyMaterialsImportIsRetired() => !LegacyMaterialsImportRetirementIsPending();

    private bool LegacyWorkbookTablesRetirementIsPending()
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = $"SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name IN ({string.Join(",", LegacyWorkbookTablesDropOrder.Select((_, index) => $"$table{index}"))});";
            for (var index = 0; index < LegacyWorkbookTablesDropOrder.Length; index++)
                command.Parameters.AddWithValue($"$table{index}", LegacyWorkbookTablesDropOrder[index]);
            return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture) > 0;
        }
        catch { return true; }
    }

    public bool LegacyWorkbookTablesAreRetired() => !LegacyWorkbookTablesRetirementIsPending();

    private bool LegacyWorkbookPostRetirementBackupIsPending()
    {
        try
        {
            using var connection = new SqliteConnection(ConnectionString);
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "SELECT Value FROM AppMeta WHERE Key='LegacyWorkbookPostRetirementBackupV1' LIMIT 1;";
            return !string.Equals(command.ExecuteScalar()?.ToString(), "complete", StringComparison.Ordinal);
        }
        catch { return false; }
    }

    private void CreateAndRecordLegacyWorkbookPostRetirementBackup()
    {
        var backup = CreateConsistentDatabaseBackup(AutomaticBackupPrefix);
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var marker = connection.CreateCommand();
        marker.CommandText = """
            INSERT OR REPLACE INTO AppMeta(Key, Value) VALUES ('LegacyWorkbookPostRetirementBackupV1', 'complete');
            INSERT OR REPLACE INTO AppMeta(Key, Value) VALUES ('LegacyWorkbookPostRetirementBackupPath', $path);
            """;
        marker.Parameters.AddWithValue("$path", backup.FullName);
        marker.ExecuteNonQuery();
    }

    private void CreateThrottledAutomaticBackupBeforeWrite()
    {
        var latest = GetLatestAutomaticBackup();
        if (latest is not null && DateTime.UtcNow - latest.CreationTimeUtc < TimeSpan.FromMinutes(5)) return;
        CreateAutomaticBackupBeforeWrite();
    }

    private void CreateAutomaticBackupBeforeWrite()
    {
        try
        {
            if (!File.Exists(DatabasePath)) return;

            var info = new FileInfo(DatabasePath);
            if (info.Length == 0) return;

            CreateConsistentDatabaseBackup(AutomaticBackupPrefix);
            CleanupAutomaticBackups();
        }
        catch
        {
            // Automatic backups are a safety feature, but they must not block normal saving.
            // Manual backups and Excel export remain available if a backup copy cannot be created.
        }
    }

    private void CleanupAutomaticBackups()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(BackupFolder) || !IODirectory.Exists(BackupFolder)) return;

            var backups = IODirectory
                .GetFiles(
                    BackupFolder,
                    $"{PresentationAutomaticBackupPrefix}*{PresentationBackupExtension}",
                    SearchOption.TopDirectoryOnly)
                .Select(path => new FileInfo(path))
                .Where(IsPresentationAutomaticBackupFile)
                .OrderByDescending(file => file.CreationTimeUtc)
                .ThenByDescending(file => file.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();

            foreach (var oldBackup in backups.Skip(MaxAutomaticBackups))
            {
                try
                {
                    oldBackup.Delete();
                }
                catch
                {
                    // Best effort cleanup. A locked file can be removed on a later save.
                }
            }
        }
        catch
        {
            // Best effort cleanup only.
        }
    }


    public int CurrentSchemaVersion => SchemaVersion;

    public IReadOnlyList<FileInfo> GetAutomaticBackups()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(BackupFolder) || !IODirectory.Exists(BackupFolder)) return Array.Empty<FileInfo>();

            return EnumerateSupportedBackupPaths(BackupFolder)
                .Select(path => new FileInfo(path))
                .Where(IsPresentationAutomaticBackupFile)
                .OrderByDescending(file => file.CreationTimeUtc)
                .ThenByDescending(file => file.Name, StringComparer.OrdinalIgnoreCase)
                .ToList();
        }
        catch
        {
            return Array.Empty<FileInfo>();
        }
    }

    public int GetRetainedLegacyAutomaticBackupCount()
    {
        try
        {
            if (string.IsNullOrWhiteSpace(BackupFolder) || !IODirectory.Exists(BackupFolder)) return 0;
            return EnumerateSupportedBackupPaths(BackupFolder)
                .Select(path => new FileInfo(path))
                .Count(IsLegacyAutomaticBackupFile);
        }
        catch
        {
            return 0;
        }
    }

    public FileInfo? GetLatestAutomaticBackup() => GetAutomaticBackups().FirstOrDefault();

    private static bool IsAutomaticBackupFile(FileInfo file)
    {
        return IsLegacyAutomaticBackupFile(file) || IsPresentationAutomaticBackupFile(file);
    }

    private static bool IsLegacyAutomaticBackupFile(FileInfo file)
    {
        if (!string.Equals(file.Extension, AutomaticBackupExtension, StringComparison.OrdinalIgnoreCase))
            return false;
        var name = IOPath.GetFileNameWithoutExtension(file.Name);
        if (!name.StartsWith(AutomaticBackupPrefix, StringComparison.OrdinalIgnoreCase))
            return false;
        var legacySuffix = name[AutomaticBackupPrefix.Length..];
        return DateTime.TryParseExact(
            legacySuffix,
            "yyyyMMdd_HHmmss_fff",
            CultureInfo.InvariantCulture,
            DateTimeStyles.None,
            out _);
    }

    private static bool IsPresentationAutomaticBackupFile(FileInfo file)
    {
        if (!string.Equals(file.Extension, PresentationBackupExtension, StringComparison.OrdinalIgnoreCase))
            return false;
        var name = IOPath.GetFileNameWithoutExtension(file.Name);
        if (!name.StartsWith(PresentationAutomaticBackupPrefix, StringComparison.OrdinalIgnoreCase))
            return false;
        var suffix = name[PresentationAutomaticBackupPrefix.Length..];
        return DateTime.TryParseExact(
                   suffix,
                   "yyyy-MM-dd_HHmmss_fff",
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out _) ||
               HasCollisionSafePresentationSuffix(suffix);
    }

    private static bool HasCollisionSafePresentationSuffix(string suffix)
    {
        const int stampLength = 21;
        return suffix.Length == stampLength + 33 &&
               suffix[stampLength] == '-' &&
               DateTime.TryParseExact(
                   suffix[..stampLength],
                   "yyyy-MM-dd_HHmmss_fff",
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out _) &&
               Guid.TryParseExact(suffix[(stampLength + 1)..], "N", out _);
    }

    public FileInfo CreateManualBackupNow()
    {
        return CreateConsistentDatabaseBackup("filamentdb_manual_");
    }

    public DatabaseBackupInfo InspectDatabaseBackup(string backupPath) => InspectDatabaseFile(backupPath);

    private static DatabaseBackupInfo InspectDatabaseFile(string databasePath)
    {
        if (string.IsNullOrWhiteSpace(databasePath) || !File.Exists(databasePath))
            throw new FileNotFoundException("SQLite backup was not found.", databasePath);

        var fullPath = Path.GetFullPath(databasePath);
        using var connection = new SqliteConnection($"Data Source={fullPath};Mode=ReadOnly;Pooling=False");
        connection.Open();
        string ScalarText(string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return command.ExecuteScalar()?.ToString() ?? string.Empty;
        }
        int CountIfPresent(string table)
        {
            using var exists = connection.CreateCommand();
            exists.CommandText = "SELECT COUNT(*) FROM sqlite_master WHERE type='table' AND name=$table;";
            exists.Parameters.AddWithValue("$table", table);
            if (Convert.ToInt32(exists.ExecuteScalar(), CultureInfo.InvariantCulture) == 0) return 0;
            return int.Parse(ScalarText($"SELECT COUNT(*) FROM {Quote(table)};"), CultureInfo.InvariantCulture);
        }
        _ = int.TryParse(ScalarText("SELECT Value FROM AppMeta WHERE Key='SchemaVersion' LIMIT 1;"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var schema);
        return new DatabaseBackupInfo
        {
            FilePath = fullPath,
            IntegrityResult = ScalarText("PRAGMA integrity_check;"),
            SchemaVersion = schema,
            FileSizeBytes = new FileInfo(fullPath).Length,
            Materials = CountIfPresent("NativeMaterialManagerRows"),
            TensileSamples = CountIfPresent("NativeTensileSamples"),
            ImpactSamples = CountIfPresent("NativeImpactSamples"),
            StiffnessRows = CountIfPresent("NativeStiffnessMeasurements"),
            SettingsRows = CountIfPresent("NativeSettingsRows"),
            BackupKind = ClassifyBackupKind(fullPath),
            ModifiedAt = File.GetLastWriteTime(fullPath)
        };
    }

    public IReadOnlyList<DatabaseBackupInfo> GetLocalBackupCatalog()
    {
        if (!IODirectory.Exists(BackupFolder)) return Array.Empty<DatabaseBackupInfo>();
        var activePath = IOPath.GetFullPath(DatabasePath);
        var results = new List<DatabaseBackupInfo>();
        foreach (var path in EnumerateSupportedBackupPaths(BackupFolder)
                     .Where(path => !string.Equals(IOPath.GetFullPath(path), activePath, StringComparison.OrdinalIgnoreCase))
                     .OrderByDescending(IOFile.GetLastWriteTimeUtc))
        {
            try { results.Add(VerifyBackupCompatibility(path, runMigrationDryRun: false)); }
            catch (Exception ex)
            {
                results.Add(new DatabaseBackupInfo
                {
                    FilePath = IOPath.GetFullPath(path), IntegrityResult = "error", FileSizeBytes = new FileInfo(path).Length,
                    BackupKind = ClassifyBackupKind(path), CompatibilityStatus = "Corrupt / unreadable", CompatibilityDetail = ex.Message,
                    ModifiedAt = IOFile.GetLastWriteTime(path), CanRestore = false
                });
            }
        }
        return results;
    }

    private static IEnumerable<string> EnumerateSupportedBackupPaths(string folder) =>
        IODirectory
            .EnumerateFiles(folder, "*", SearchOption.TopDirectoryOnly)
            .Where(IsSupportedBackupPath);

    private static bool IsSupportedBackupPath(string path) =>
        string.Equals(IOPath.GetExtension(path), AutomaticBackupExtension, StringComparison.OrdinalIgnoreCase) ||
        string.Equals(IOPath.GetExtension(path), PresentationBackupExtension, StringComparison.OrdinalIgnoreCase);

    public static bool BackupFilenameCompatibilityContractIsReady()
    {
        var legacyAutomatic = new FileInfo("filamentdb_20260725_123456_789.sqlite");
        var presentationAutomatic = new FileInfo("3DPIceland-Automatic-2026-07-25_123456_789.bak");
        var collisionSafeAutomatic = new FileInfo(
            "3DPIceland-Automatic-2026-07-25_123456_789-0123456789abcdef0123456789abcdef.bak");
        return IsSupportedBackupPath(legacyAutomatic.Name) &&
               IsSupportedBackupPath(presentationAutomatic.Name) &&
               IsAutomaticBackupFile(legacyAutomatic) &&
               IsAutomaticBackupFile(presentationAutomatic) &&
               IsAutomaticBackupFile(collisionSafeAutomatic) &&
               ClassifyBackupKind("filamentdb_manual_20260725_123456_789.sqlite") == "Manual backup" &&
               ClassifyBackupKind("3DPIceland-Manual-2026-07-25_123456_789.bak") == "Manual backup" &&
               ClassifyBackupKind("3DPIceland-Pre-SQLite-Restore-2026-07-25_123456_789.bak") ==
               "Pre-SQLite restore recovery" &&
               ClassifyBackupKind("3DPIceland-Post-SQLite-Restore-2026-07-25_123456_789.bak") ==
               "Post-SQLite restore evidence" &&
               ClassifyBackupKind("3DPIceland-Pre-Excel-Restore-2026-07-25_123456_789.bak") ==
               "Pre-Excel restore recovery" &&
               ClassifyBackupKind("3DPIceland-Post-Excel-Restore-2026-07-25_123456_789.bak") ==
               "Post-Excel restore evidence";
    }

    public DatabaseBackupInfo VerifyBackupCompatibility(string backupPath, bool runMigrationDryRun = true)
    {
        var source = InspectDatabaseFile(backupPath);
        if (!source.IsIntegrityValid) return WithCompatibility(source, "Corrupt / unreadable", "PRAGMA integrity_check returned " + source.IntegrityResult, false, false);
        if (source.SchemaVersion > SchemaVersion) return WithCompatibility(source, "Newer / incompatible", $"Backup schema v{source.SchemaVersion} is newer than application schema v{SchemaVersion}.", false, false);
        if (source.SchemaVersion < MinimumStandaloneBackupSchemaVersion)
            return WithCompatibility(source, "Legacy / incomplete", $"Schema v{source.SchemaVersion} predates SQLite-canonical native measurements; external migration snapshots may be required.", false, false);
        if (source.SchemaVersion == SchemaVersion)
        {
            if (source.Materials <= 0)
                return WithCompatibility(source, "Ready — empty profile", $"Integrity ok; schema v{source.SchemaVersion}; healthy clean-profile backup with no canonical Materials. Full-data release evidence requires a separate Ready backup containing Materials.", false, true);
            return WithCompatibility(source, "Ready", $"Integrity ok; schema v{source.SchemaVersion}; no migration required.", false, true);
        }
        if (source.Materials <= 0)
            return WithCompatibility(source, "Legacy / incomplete", $"Schema v{source.SchemaVersion} contains no canonical Materials and requires migration evidence before it can represent a supported clean profile.", false, false);
        if (!runMigrationDryRun)
            return WithCompatibility(source, "Migration required", $"Schema v{source.SchemaVersion} must pass an isolated migration dry-run to v{SchemaVersion} before restore.", false, false);

        var tempFolder = Path.Combine(Path.GetTempPath(), "3DPIceland-RecoveryVerify-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempFolder);
        var tempDatabase = Path.Combine(tempFolder, "filamentdb.sqlite");
        try
        {
            using (var input = new SqliteConnection($"Data Source={source.FilePath};Mode=ReadOnly"))
            using (var output = new SqliteConnection($"Data Source={tempDatabase}"))
            {
                input.Open(); output.Open(); input.BackupDatabase(output);
            }
            _ = new LocalDatabase(tempDatabase);
            var migrated = InspectDatabaseFile(tempDatabase);
            var countsPreserved = migrated.Materials == source.Materials && migrated.TensileSamples == source.TensileSamples &&
                                  migrated.ImpactSamples == source.ImpactSamples && migrated.StiffnessRows == source.StiffnessRows;
            if (!migrated.IsIntegrityValid || migrated.SchemaVersion != SchemaVersion || !countsPreserved)
                return WithCompatibility(source, "Migration failed", $"Dry-run result: integrity {migrated.IntegrityResult}, schema v{migrated.SchemaVersion}, canonical counts preserved {countsPreserved}.", false, false);
            return WithCompatibility(source, "Ready after migration", $"Isolated v{source.SchemaVersion} → v{SchemaVersion} migration passed; integrity and canonical counts preserved.", true, true);
        }
        catch (Exception ex) { return WithCompatibility(source, "Migration failed", ex.Message, false, false); }
        finally
        {
            SqliteConnection.ClearAllPools();
            try { if (Directory.Exists(tempFolder)) Directory.Delete(tempFolder, recursive: true); } catch { }
        }
    }

    private static DatabaseBackupInfo WithCompatibility(DatabaseBackupInfo source, string status, string detail, bool dryRunPassed, bool canRestore) => new()
    {
        FilePath=source.FilePath, IntegrityResult=source.IntegrityResult, SchemaVersion=source.SchemaVersion, FileSizeBytes=source.FileSizeBytes,
        Materials=source.Materials, TensileSamples=source.TensileSamples, ImpactSamples=source.ImpactSamples, StiffnessRows=source.StiffnessRows, SettingsRows=source.SettingsRows,
        BackupKind=source.BackupKind, ModifiedAt=source.ModifiedAt, CompatibilityStatus=status, CompatibilityDetail=detail, MigrationDryRunPassed=dryRunPassed, CanRestore=canRestore
    };

    private static string ClassifyBackupKind(string path)
    {
        var name = IOPath.GetFileName(path);
        if (name.StartsWith("filamentdb_pre_excel_restore_", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith(PresentationPreExcelRestoreBackupPrefix, StringComparison.OrdinalIgnoreCase))
            return "Pre-Excel restore recovery";
        if (name.StartsWith("filamentdb_post_excel_restore_", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith(PresentationPostExcelRestoreBackupPrefix, StringComparison.OrdinalIgnoreCase))
            return "Post-Excel restore evidence";
        if (name.StartsWith("filamentdb_pre_restore_", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith(PresentationPreRestoreBackupPrefix, StringComparison.OrdinalIgnoreCase))
            return "Pre-SQLite restore recovery";
        if (name.StartsWith("filamentdb_post_restore_", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith(PresentationPostRestoreBackupPrefix, StringComparison.OrdinalIgnoreCase))
            return "Post-SQLite restore evidence";
        if (name.StartsWith("filamentdb_manual_", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith(PresentationManualBackupPrefix, StringComparison.OrdinalIgnoreCase))
            return "Manual backup";
        if (name.StartsWith(AutomaticBackupPrefix, StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith(PresentationAutomaticBackupPrefix, StringComparison.OrdinalIgnoreCase))
            return "Automatic / migration backup";
        return "External SQLite backup";
    }

    public DatabaseRestoreResult RestoreDatabaseBackup(string backupPath)
    {
        var source = VerifyBackupCompatibility(backupPath, runMigrationDryRun: true);
        if (!source.CanRestore) throw new InvalidOperationException("Restore blocked: " + source.CompatibilityStatus + ". " + source.CompatibilityDetail);

        var liveFullPath = Path.GetFullPath(DatabasePath);
        if (string.Equals(liveFullPath, source.FilePath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Restore source cannot be the active SQLite database.");

        var recovery = CreateConsistentDatabaseBackup("filamentdb_pre_restore_");
        var stagedPath = Path.Combine(DatabaseFolder, $"filamentdb_restore_staged_{Guid.NewGuid():N}.sqlite");
        try
        {
            File.Copy(source.FilePath, stagedPath, overwrite: false);
            var staged = InspectDatabaseFile(stagedPath);
            if (!staged.IsIntegrityValid) throw new InvalidOperationException("The staged restore copy failed integrity verification.");

            SqliteConnection.ClearAllPools();
            RestoreDatabaseContents(stagedPath, liveFullPath);
            SqliteConnection.ClearAllPools();
            var restored = InspectDatabaseFile(liveFullPath);
            if (!restored.IsIntegrityValid) throw new InvalidOperationException("The restored database failed integrity verification.");
            var postRestore = CreateConsistentDatabaseBackup("filamentdb_post_restore_");
            var postRestoreInspection = InspectDatabaseFile(postRestore.FullName);
            if (!postRestoreInspection.IsIntegrityValid ||
                postRestoreInspection.SchemaVersion != restored.SchemaVersion ||
                postRestoreInspection.Materials != restored.Materials ||
                postRestoreInspection.TensileSamples != restored.TensileSamples ||
                postRestoreInspection.ImpactSamples != restored.ImpactSamples ||
                postRestoreInspection.StiffnessRows != restored.StiffnessRows)
                throw new InvalidOperationException("The post-restore evidence backup does not reproduce the restored canonical database.");
            return new DatabaseRestoreResult
            {
                SourceBackupPath = source.FilePath,
                RecoveryBackupPath = recovery.FullName,
                PostRestoreBackupPath = postRestore.FullName,
                RestoredDatabase = restored
            };
        }
        catch
        {
            try
            {
                SqliteConnection.ClearAllPools();
                RestoreDatabaseContents(recovery.FullName, liveFullPath);
                SqliteConnection.ClearAllPools();
            }
            catch { }
            throw;
        }
        finally
        {
            try { if (File.Exists(stagedPath)) File.Delete(stagedPath); } catch { }
        }
    }

    private static void RestoreDatabaseContents(string sourcePath, string destinationPath)
    {
        var sourceBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = sourcePath,
            Mode = SqliteOpenMode.ReadOnly,
            Pooling = false
        };
        var destinationBuilder = new SqliteConnectionStringBuilder
        {
            DataSource = destinationPath,
            Mode = SqliteOpenMode.ReadWrite,
            Pooling = false
        };
        using var source = new SqliteConnection(sourceBuilder.ToString());
        using var destination = new SqliteConnection(destinationBuilder.ToString());
        source.Open();
        destination.Open();
        source.BackupDatabase(destination);
    }

    public ExcelRecoverySnapshot CreateExcelRecoverySnapshot()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        var snapshot = new ExcelRecoverySnapshot
        {
            SourceSchemaVersion = SchemaVersion,
            ExportedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)
        };
        for (var index = 0; index < ExcelRecoveryTableInsertOrder.Length; index++)
        {
            var tableName = ExcelRecoveryTableInsertOrder[index];
            var columns = GetTableColumnDefinitions(connection, tableName);
            if (columns.Count == 0) throw new InvalidOperationException("Canonical recovery table is missing: " + tableName);
            var table = new ExcelRecoveryTable
            {
                TableName = tableName,
                SheetName = $"DR{index + 1:00} {tableName}"[..Math.Min(31, $"DR{index + 1:00} {tableName}".Length)],
                Columns = columns.Select(column => column.Name).ToList()
            };
            using var command = connection.CreateCommand();
            var primaryKeys = columns.Where(column => column.PrimaryKeyOrder > 0).OrderBy(column => column.PrimaryKeyOrder).Select(column => Quote(column.Name)).ToList();
            command.CommandText = $"SELECT {string.Join(",", table.Columns.Select(Quote))} FROM {Quote(tableName)}" + (primaryKeys.Count > 0 ? " ORDER BY " + string.Join(",", primaryKeys) : string.Empty) + ";";
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                var row = new List<object?>();
                for (var column = 0; column < reader.FieldCount; column++) row.Add(reader.IsDBNull(column) ? null : reader.GetValue(column));
                table.Rows.Add(row);
            }
            snapshot.Tables.Add(table);
        }
        return snapshot;
    }

    public ExcelRecoveryRestoreResult RestoreExcelRecoverySnapshot(ExcelRecoverySnapshot snapshot)
    {
        if (snapshot.SourceSchemaVersion <= 0 || snapshot.SourceSchemaVersion > SchemaVersion)
            throw new InvalidOperationException($"Excel recovery schema v{snapshot.SourceSchemaVersion} is not compatible with application schema v{SchemaVersion}.");
        var supplied = snapshot.Tables.Select(table => table.TableName).ToList();
        if (supplied.Count != ExcelRecoveryTableInsertOrder.Length || supplied.Distinct(StringComparer.OrdinalIgnoreCase).Count() != supplied.Count ||
            ExcelRecoveryTableInsertOrder.Any(required => !supplied.Contains(required, StringComparer.OrdinalIgnoreCase)))
            throw new InvalidOperationException("Excel recovery package does not contain the exact governed canonical table set.");
        var materials = snapshot.Tables.First(table => string.Equals(table.TableName, "NativeMaterialManagerRows", StringComparison.OrdinalIgnoreCase));
        if (materials.Rows.Count == 0) throw new InvalidOperationException("Excel recovery restore blocked: no canonical Materials rows are present.");

        var recovery = CreateConsistentDatabaseBackup("filamentdb_pre_excel_restore_");
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        var committed = false;
        try
        {
            foreach (var tableName in ExcelRecoveryTableInsertOrder.Reverse())
            {
                using var delete = connection.CreateCommand(); delete.Transaction = transaction;
                delete.CommandText = $"DELETE FROM {Quote(tableName)};"; delete.ExecuteNonQuery();
            }
            long rowsRestored = 0;
            foreach (var tableName in ExcelRecoveryTableInsertOrder)
            {
                var table = snapshot.Tables.Single(item => string.Equals(item.TableName, tableName, StringComparison.OrdinalIgnoreCase));
                var currentColumns = GetTableColumnDefinitions(connection, tableName).Select(column => column.Name).ToHashSet(StringComparer.OrdinalIgnoreCase);
                if (table.Columns.Count == 0 || table.Columns.Any(column => !currentColumns.Contains(column)))
                    throw new InvalidOperationException("Excel recovery table has incompatible columns: " + tableName);
                foreach (var row in table.Rows)
                {
                    if (row.Count != table.Columns.Count) throw new InvalidOperationException("Excel recovery row width mismatch in " + tableName);
                    using var insert = connection.CreateCommand(); insert.Transaction = transaction;
                    insert.CommandText = $"INSERT INTO {Quote(tableName)} ({string.Join(",", table.Columns.Select(Quote))}) VALUES ({string.Join(",", table.Columns.Select((_, i) => "$p" + i))});";
                    for (var index = 0; index < row.Count; index++) insert.Parameters.AddWithValue("$p" + index, row[index] ?? DBNull.Value);
                    insert.ExecuteNonQuery(); rowsRestored++;
                }
            }
            using (var marker = connection.CreateCommand())
            {
                marker.Transaction = transaction;
                marker.CommandText = "INSERT OR REPLACE INTO AppMeta(Key,Value) VALUES('NativeMeasurementsCanonicalV1','complete');";
                marker.ExecuteNonQuery();
            }
            using (var foreignKeys = connection.CreateCommand())
            {
                foreignKeys.Transaction = transaction;
                foreignKeys.CommandText = "PRAGMA foreign_key_check;";
                using var reader = foreignKeys.ExecuteReader();
                if (reader.Read()) throw new InvalidOperationException("Excel recovery data failed SQLite foreign-key verification.");
            }
            using (var integrity = connection.CreateCommand())
            {
                integrity.Transaction = transaction;
                integrity.CommandText = "PRAGMA integrity_check;";
                if (!string.Equals(integrity.ExecuteScalar()?.ToString(), "ok", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Excel recovery data failed SQLite integrity verification.");
            }
            using (var count = connection.CreateCommand())
            {
                count.Transaction = transaction;
                count.CommandText = "SELECT COUNT(*) FROM NativeMaterialManagerRows;";
                if (Convert.ToInt32(count.ExecuteScalar(), CultureInfo.InvariantCulture) != materials.Rows.Count)
                    throw new InvalidOperationException("Excel recovery Materials count verification failed.");
            }
            transaction.Commit();
            committed = true;
            var postRestore = CreateConsistentDatabaseBackup("filamentdb_post_excel_restore_");
            var postRestoreInspection = InspectDatabaseFile(postRestore.FullName);
            if (!postRestoreInspection.IsIntegrityValid ||
                postRestoreInspection.Materials != materials.Rows.Count)
                throw new InvalidOperationException("The post-Excel restore evidence backup failed canonical verification.");
            return new ExcelRecoveryRestoreResult
            {
                RecoveryBackupPath = recovery.FullName,
                PostRestoreBackupPath = postRestore.FullName,
                TablesRestored = snapshot.Tables.Count,
                RowsRestored = rowsRestored,
                MaterialsRestored = materials.Rows.Count
            };
        }
        catch
        {
            try { transaction.Rollback(); } catch { }
            if (committed)
            {
                try
                {
                    connection.Close();
                    SqliteConnection.ClearAllPools();
                    RestoreDatabaseContents(recovery.FullName, DatabasePath);
                    SqliteConnection.ClearAllPools();
                }
                catch
                {
                    // The verified pre-restore backup remains retained for explicit recovery.
                }
            }
            throw;
        }
    }

    private static List<(string Name, int PrimaryKeyOrder)> GetTableColumnDefinitions(SqliteConnection connection, string tableName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({Quote(tableName)});";
        using var reader = command.ExecuteReader();
        var columns = new List<(string Name, int PrimaryKeyOrder)>();
        while (reader.Read()) columns.Add((reader["name"]?.ToString() ?? string.Empty, Convert.ToInt32(reader["pk"], CultureInfo.InvariantCulture)));
        return columns;
    }

    public string RunIntegrityCheck()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA integrity_check;";
        return command.ExecuteScalar()?.ToString() ?? "No result";
    }


    private string ConnectionString => $"Data Source={DatabasePath}";

    private static string? ReadSchemaVersion(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='AppMeta';";
        if (command.ExecuteScalar() is null) return null;

        command.CommandText = "SELECT Value FROM AppMeta WHERE Key='SchemaVersion' LIMIT 1;";
        return command.ExecuteScalar()?.ToString();
    }

    private static bool TableHasColumn(SqliteConnection connection, string tableName, string columnName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({Quote(tableName)});";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase))
            {
                return true;
            }
        }
        return false;
    }

    private static bool TableHasPrimaryKey(SqliteConnection connection, string tableName, string columnName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({Quote(tableName)});";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var name = reader["name"]?.ToString();
            var pkText = reader["pk"]?.ToString();
            _ = int.TryParse(pkText, out var pk);
            if (string.Equals(name, columnName, StringComparison.OrdinalIgnoreCase) && pk > 0)
            {
                return true;
            }
        }
        return false;
    }

    private void Initialize()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"
PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS AppMeta (
    Key TEXT PRIMARY KEY,
    Value TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS Imports (
    ImportId INTEGER PRIMARY KEY AUTOINCREMENT,
    ImportedAtUtc TEXT NOT NULL,
    SourceFileName TEXT NOT NULL,
    SourcePath TEXT NOT NULL,
    SchemaVersion INTEGER NOT NULL
);

CREATE TABLE IF NOT EXISTS Materials (
    MaterialId TEXT PRIMARY KEY,
    Manufacturer TEXT,
    ProductLine TEXT,
    MarketingName TEXT,
    BaseMaterial TEXT,
    MaterialCategory TEXT,
    VariantFinish TEXT,
    Reinforcement TEXT,
    Color TEXT,
    DiameterMm TEXT,
    SpoolWeightG TEXT,
    ManufacturerSku TEXT,
    InventoryId TEXT,
    PurchaseId TEXT,
    PurchasedFrom TEXT,
    SupplierUrl TEXT,
    PurchaseDate TEXT,
    OrderNumber TEXT,
    BatchNumber TEXT,
    StorageLocation TEXT,
    InventoryStatus TEXT,
    Quantity TEXT,
    RemainingWeightG TEXT,
    PurchasePriceAmount TEXT,
    PurchaseCurrency TEXT,
    ShippingAmount TEXT,
    VatAmount TEXT,
    MsrpAmount TEXT,
    MsrpCurrency TEXT,
    MsrpUsd TEXT,
    LandedCostAmount TEXT,
    LandedCostCurrency TEXT,
    LandedCostUsd TEXT,
    MsrpUsdPerKg TEXT,
    LandedCostUsdPerKg TEXT,
    PriceCheckedDate TEXT,
    ManufacturerWebsite TEXT,
    YouTubeReviewUrl TEXT,
    ThumbnailFilename TEXT,
    Video TEXT,
    Notes TEXT,
    TestedStatus TEXT,
    InTensile TEXT,
    InImpact TEXT,
    InStiffness TEXT,
    SortOrder TEXT,
    SourcePriority TEXT,
    WebsiteDisplayName TEXT,
    MaterialKey TEXT
);

CREATE TABLE IF NOT EXISTS MaterialAttributes (
    MaterialId TEXT NOT NULL,
    FieldName TEXT NOT NULL,
    FieldValue TEXT,
    PRIMARY KEY (MaterialId, FieldName),
    FOREIGN KEY (MaterialId) REFERENCES Materials(MaterialId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS Manufacturers (
    ManufacturerId INTEGER PRIMARY KEY AUTOINCREMENT,
    Name TEXT NOT NULL UNIQUE,
    Website TEXT,
    DisplayName TEXT,
    Country TEXT,
    Founded TEXT,
    LogoUrl TEXT,
    Description TEXT,
    EngineeringFocus TEXT,
    MaterialCategories TEXT,
    Strengths TEXT,
    Weaknesses TEXT,
    Sustainability TEXT,
    TypicalApplications TEXT,
    Headquarters TEXT,
    Notes TEXT,
    SortOrder INTEGER NOT NULL DEFAULT 100,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAtUtc TEXT,
    UpdatedAtUtc TEXT
);

CREATE TABLE IF NOT EXISTS LookupValues (
    LookupType TEXT NOT NULL,
    Value TEXT NOT NULL,
    PRIMARY KEY (LookupType, Value)
);

CREATE TABLE IF NOT EXISTS ExcelSheets (
    SheetId INTEGER PRIMARY KEY AUTOINCREMENT,
    ImportId INTEGER NOT NULL,
    SheetName TEXT NOT NULL,
    Purpose TEXT NOT NULL,
    HeaderRow INTEGER NOT NULL,
    RowCount INTEGER NOT NULL,
    ColumnCount INTEGER NOT NULL,
    FOREIGN KEY (ImportId) REFERENCES Imports(ImportId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ExcelSheetColumns (
    ColumnId INTEGER PRIMARY KEY AUTOINCREMENT,
    SheetId INTEGER NOT NULL,
    ColumnIndex INTEGER NOT NULL,
    ColumnName TEXT NOT NULL,
    FOREIGN KEY (SheetId) REFERENCES ExcelSheets(SheetId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ExcelSheetRows (
    RowId INTEGER PRIMARY KEY AUTOINCREMENT,
    SheetId INTEGER NOT NULL,
    RowIndex INTEGER NOT NULL,
    MaterialId TEXT,
    FOREIGN KEY (SheetId) REFERENCES ExcelSheets(SheetId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ExcelSheetCells (
    RowId INTEGER NOT NULL,
    ColumnId INTEGER NOT NULL,
    CellValue TEXT,
    PRIMARY KEY (RowId, ColumnId),
    FOREIGN KEY (RowId) REFERENCES ExcelSheetRows(RowId) ON DELETE CASCADE,
    FOREIGN KEY (ColumnId) REFERENCES ExcelSheetColumns(ColumnId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS TensileResults (
    MaterialId TEXT PRIMARY KEY,
    UprightMpa TEXT,
    FlatMpa TEXT,
    StdDevUpright TEXT,
    StdDevFlat TEXT,
    CvUpright TEXT,
    CvFlat TEXT,
    SamplesUpright TEXT,
    SamplesFlat TEXT,
    ConfidenceUpright TEXT,
    ConfidenceFlat TEXT,
    TestNotes TEXT,
    SourceSheet TEXT NOT NULL,
    FOREIGN KEY (MaterialId) REFERENCES Materials(MaterialId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS TensileSamples (
    MaterialId TEXT NOT NULL,
    Orientation TEXT NOT NULL,
    SampleNumber INTEGER NOT NULL,
    RawValue TEXT,
    SourceSheet TEXT NOT NULL,
    PRIMARY KEY (MaterialId, Orientation, SampleNumber),
    FOREIGN KEY (MaterialId) REFERENCES Materials(MaterialId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS ImpactSamples (
    MaterialId TEXT NOT NULL,
    Orientation TEXT NOT NULL,
    SampleNumber INTEGER NOT NULL,
    RawValue TEXT,
    SourceSheet TEXT NOT NULL,
    PRIMARY KEY (MaterialId, Orientation, SampleNumber),
    FOREIGN KEY (MaterialId) REFERENCES Materials(MaterialId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS StiffnessMeasurements (
    MaterialId TEXT PRIMARY KEY,
    Revolutions TEXT,
    Degrees TEXT,
    TestNotes TEXT,
    SourceSheet TEXT NOT NULL,
    FOREIGN KEY (MaterialId) REFERENCES Materials(MaterialId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS NativeMeasurementNotes (
    MaterialId TEXT NOT NULL,
    TestType TEXT NOT NULL,
    TestNotes TEXT,
    MeasuredDate TEXT,
    UpdatedAtUtc TEXT NOT NULL,
    PRIMARY KEY (MaterialId, TestType)
);

CREATE TABLE IF NOT EXISTS NativeTensileSamples (
    MaterialId TEXT NOT NULL, Orientation TEXT NOT NULL, SampleNumber INTEGER NOT NULL,
    RawValue TEXT, UpdatedAtUtc TEXT NOT NULL,
    PRIMARY KEY (MaterialId, Orientation, SampleNumber),
    FOREIGN KEY (MaterialId) REFERENCES NativeMaterialManagerRows(MaterialId) ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS NativeTensileResults (
    MaterialId TEXT PRIMARY KEY, UprightMpa TEXT, FlatMpa TEXT, StdDevUpright TEXT, StdDevFlat TEXT,
    CvUpright TEXT, CvFlat TEXT, SamplesUpright TEXT, SamplesFlat TEXT,
    ConfidenceUpright TEXT, ConfidenceFlat TEXT, TestNotes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (MaterialId) REFERENCES NativeMaterialManagerRows(MaterialId) ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS NativeImpactSamples (
    MaterialId TEXT NOT NULL, Orientation TEXT NOT NULL, SampleNumber INTEGER NOT NULL,
    RawValue TEXT, UpdatedAtUtc TEXT NOT NULL,
    PRIMARY KEY (MaterialId, Orientation, SampleNumber),
    FOREIGN KEY (MaterialId) REFERENCES NativeMaterialManagerRows(MaterialId) ON DELETE CASCADE
);
CREATE TABLE IF NOT EXISTS NativeStiffnessMeasurements (
    MaterialId TEXT PRIMARY KEY, Revolutions TEXT, Degrees TEXT, TestNotes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (MaterialId) REFERENCES NativeMaterialManagerRows(MaterialId) ON DELETE CASCADE
);

CREATE TABLE IF NOT EXISTS TestSummaryValues (
    MaterialId TEXT NOT NULL,
    TestType TEXT NOT NULL,
    MetricName TEXT NOT NULL,
    MetricValue TEXT,
    Unit TEXT,
    SourceSheet TEXT NOT NULL,
    SourceColumn TEXT NOT NULL,
    PRIMARY KEY (MaterialId, TestType, MetricName),
    FOREIGN KEY (MaterialId) REFERENCES Materials(MaterialId) ON DELETE CASCADE
);


CREATE TABLE IF NOT EXISTS NativeMaterialManagerRows (
    MaterialId TEXT PRIMARY KEY,
    ManufacturerId INTEGER,
    Manufacturer TEXT,
    ProductLine TEXT,
    MarketingName TEXT,
    BaseMaterialId INTEGER,
    BaseMaterial TEXT,
    MaterialCategory TEXT,
    VariantFinish TEXT,
    Reinforcement TEXT,
    Color TEXT,
    DiameterMm TEXT,
    SpoolWeightG TEXT,
    ManufacturerSku TEXT,
    InventoryId TEXT,
    PurchaseId TEXT,
    PurchasedFrom TEXT,
    SupplierUrl TEXT,
    PurchaseDate TEXT,
    OrderNumber TEXT,
    BatchNumber TEXT,
    StorageLocation TEXT,
    InventoryStatus TEXT,
    Quantity TEXT,
    RemainingWeightG TEXT,
    PurchasePriceAmount TEXT,
    PurchaseCurrency TEXT,
    ShippingAmount TEXT,
    VatAmount TEXT,
    MsrpAmount TEXT,
    MsrpCurrency TEXT,
    MsrpUsd TEXT,
    LandedCostAmount TEXT,
    LandedCostCurrency TEXT,
    LandedCostUsd TEXT,
    MsrpUsdPerKg TEXT,
    LandedCostUsdPerKg TEXT,
    PriceCheckedDate TEXT,
    NozzleTemperatureMinC TEXT,
    NozzleTemperatureRecommendedC TEXT,
    NozzleTemperatureMaxC TEXT,
    BedTemperatureMinC TEXT,
    BedTemperatureRecommendedC TEXT,
    BedTemperatureMaxC TEXT,
    PrintSpeedMinMmPerS TEXT,
    PrintSpeedRecommendedMmPerS TEXT,
    PrintSpeedMaxMmPerS TEXT,
    CoolingRequirement TEXT,
    DryingTimeHours TEXT,
    EnclosureRequirement TEXT,
    PrinterProfileReference TEXT,
    SlicerProfileReference TEXT,
    PrintingProfileId TEXT,
    PrintingProfileKind TEXT,
    CoolingMinPercent TEXT,
    CoolingRecommendedPercent TEXT,
    CoolingMaxPercent TEXT,
    DryingTemperatureC TEXT,
    SlicerIdentity TEXT,
    SlicerVersion TEXT,
    PrintingSettingsProvenance TEXT,
    PrintingSettingsSourceUrl TEXT,
    PrintingSettingsCheckedDate TEXT,
    PrintingSettingsValidationNote TEXT,
    ManufacturerWebsite TEXT,
    YouTubeReviewUrl TEXT,
    ThumbnailFilename TEXT,
    Video TEXT,
    Notes TEXT,
    TestedStatus TEXT,
    InTensile TEXT,
    InImpact TEXT,
    InStiffness TEXT,
    SortOrder TEXT,
    SourcePriority TEXT,
    WebsiteDisplayName TEXT,
    MaterialKey TEXT,
    PublishPublicReports INTEGER NOT NULL DEFAULT 0,
    PublishPublicTestDetails INTEGER NOT NULL DEFAULT 0,
    IsArchived INTEGER NOT NULL DEFAULT 0,
    UpdatedAtUtc TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS BaseMaterialCatalog (
    BaseMaterialId INTEGER UNIQUE,
    BaseMaterial TEXT PRIMARY KEY,
    Category TEXT,
    SortOrder TEXT,
    NozzleTemperatureMinC TEXT,
    NozzleTemperatureRecommendedC TEXT,
    NozzleTemperatureMaxC TEXT,
    BedTemperatureMinC TEXT,
    BedTemperatureRecommendedC TEXT,
    BedTemperatureMaxC TEXT,
    PrintSpeedMinMmPerS TEXT,
    PrintSpeedRecommendedMmPerS TEXT,
    PrintSpeedMaxMmPerS TEXT,
    CoolingMinPercent TEXT,
    CoolingRecommendedPercent TEXT,
    CoolingMaxPercent TEXT,
    CoolingGuidance TEXT,
    DryingTemperatureC TEXT,
    DryingTimeHours TEXT,
    EnclosureRequirement TEXT,
    PrinterProfileReference TEXT,
    SlicerProfileReference TEXT,
    ProfileId TEXT,
    ProfileKind TEXT,
    UpdatedAtUtc TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS DeploymentSettings (
    SettingsId INTEGER PRIMARY KEY CHECK (SettingsId = 1),
    FtpsHost TEXT NOT NULL,
    FtpsPort INTEGER NOT NULL,
    FtpsUserName TEXT NOT NULL,
    UpdatedAtUtc TEXT NOT NULL
);

CREATE TABLE IF NOT EXISTS NativeSettingsRows (
    Section TEXT NOT NULL,
    Parameter TEXT NOT NULL,
    Value TEXT,
    Unit TEXT NOT NULL,
    UsedBy TEXT NOT NULL,
    Notes TEXT,
    UpdatedAtUtc TEXT NOT NULL,
    PRIMARY KEY (Section, Parameter, Unit, UsedBy)
);

CREATE TABLE IF NOT EXISTS VideoIdeaQueue (
    IdeaId INTEGER PRIMARY KEY AUTOINCREMENT,
    CreatedAtUtc TEXT NOT NULL,
    MaterialId TEXT,
    ProductionStatus TEXT NOT NULL,
    ProductionPriority TEXT NOT NULL,
    Notes TEXT,
    Label TEXT,
    MaterialType TEXT,
    SuggestionCategory TEXT,
    SuggestedTitle TEXT,
    SuggestedAngle TEXT,
    TalkingPoints TEXT,
    DataReason TEXT,
    Standout TEXT,
    ComparisonIdea TEXT,
    BaseMaterial TEXT,
    Category TEXT,
    Manufacturer TEXT,
    Reinforcement TEXT,
    Variant TEXT,
    ProductLine TEXT,
    OverallScore TEXT,
    TensileScore TEXT,
    ImpactScore TEXT,
    StiffnessScore TEXT,
    ConsistencyScore TEXT,
    LayerAdhesionScore TEXT,
    PublishDate TEXT,
    TargetWeek TEXT,
    Series TEXT,
    EpisodeOrder TEXT,
    Effort TEXT
);

CREATE TABLE IF NOT EXISTS InventorySpoolItems (
    InventoryItemId TEXT PRIMARY KEY,
    MaterialId TEXT NOT NULL,
    Status TEXT, Quantity TEXT, SpoolWeightG TEXT, RemainingWeightG TEXT,
    StorageLocation TEXT, BatchNumber TEXT, PurchaseId TEXT, PurchaseOrderLineId TEXT, PurchasedFrom TEXT,
    PurchaseDate TEXT, OrderNumber TEXT, PurchasePriceAmount TEXT, PurchaseCurrency TEXT,
    ShippingAmount TEXT, VatAmount TEXT, CustomsAmount TEXT, OtherFeesAmount TEXT, LandedCostAmount TEXT, Notes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (MaterialId) REFERENCES NativeMaterialManagerRows(MaterialId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_InventorySpoolItems_MaterialId ON InventorySpoolItems(MaterialId);

CREATE TABLE IF NOT EXISTS PurchaseOrders (
    PurchaseOrderId TEXT PRIMARY KEY, Supplier TEXT, OrderNumber TEXT, PurchaseDate TEXT, Currency TEXT, ExchangeRate TEXT,
    TaxTreatment TEXT, ShippingMethod TEXT, TrackingNumber TEXT, SupplierItemsTotal TEXT, SupplierShipping TEXT, SupplierTax TEXT,
    SupplierInvoiceTotal TEXT, ImportVat TEXT, CustomsDuty TEXT, ClearanceFee TEXT, OtherFees TEXT, ShippingAllocationMethod TEXT, TaxAllocationMethod TEXT, CustomsAllocationMethod TEXT, FeeAllocationMethod TEXT, CostStatus TEXT, LifecycleStatus TEXT, ReceivedDate TEXT, InvoiceFile TEXT, Notes TEXT, UpdatedAtUtc TEXT NOT NULL
);
CREATE TABLE IF NOT EXISTS PurchaseOrderLines (
    PurchaseOrderLineId TEXT PRIMARY KEY, PurchaseOrderId TEXT NOT NULL, MaterialId TEXT, InventoryCategory TEXT, Description TEXT, Sku TEXT, Quantity TEXT, ReceivedQuantity TEXT, ReceivingStatus TEXT, StorageLocation TEXT, UnitPrice TEXT, DiscountAmount TEXT, UnitWeightG TEXT, IncludeInCostAllocation INTEGER, ManualShippingAllocation TEXT, ManualTaxAllocation TEXT, ManualCustomsAllocation TEXT, ManualFeesAllocation TEXT, NetLineCost TEXT, AllocatedShipping TEXT, AllocatedTax TEXT, AllocatedCustoms TEXT, AllocatedFees TEXT, LandedLineCost TEXT, LandedUnitCost TEXT, LandedCostPerKg TEXT, AllocationStatus TEXT, Notes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId) ON DELETE CASCADE,
    FOREIGN KEY (MaterialId) REFERENCES NativeMaterialManagerRows(MaterialId) ON DELETE SET NULL
);
CREATE INDEX IF NOT EXISTS IX_PurchaseOrderLines_OrderId ON PurchaseOrderLines(PurchaseOrderId);

CREATE TABLE IF NOT EXISTS Suppliers (
    SupplierId TEXT PRIMARY KEY, Name TEXT NOT NULL, Country TEXT, DefaultCurrency TEXT, WebsiteUrl TEXT, Notes TEXT, UpdatedAtUtc TEXT NOT NULL
);
CREATE UNIQUE INDEX IF NOT EXISTS IX_Suppliers_Name ON Suppliers(Name COLLATE NOCASE);

CREATE TABLE IF NOT EXISTS PurchaseDocuments (
    PurchaseDocumentId TEXT PRIMARY KEY, PurchaseOrderId TEXT NOT NULL, DocumentType TEXT, RelativePath TEXT NOT NULL, OriginalFileName TEXT, Notes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (PurchaseOrderId) REFERENCES PurchaseOrders(PurchaseOrderId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_PurchaseDocuments_OrderId ON PurchaseDocuments(PurchaseOrderId);

CREATE TABLE IF NOT EXISTS ExperimentDefinitions (
    ExperimentDefinitionId TEXT PRIMARY KEY,
    Name TEXT NOT NULL UNIQUE,
    ParameterKey TEXT NOT NULL,
    DefaultUnit TEXT,
    Description TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    SortOrder INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS MaterialExperiments (
    MaterialExperimentId TEXT PRIMARY KEY,
    MaterialId TEXT NOT NULL,
    ExperimentDefinitionId TEXT NOT NULL,
    ParameterValue TEXT,
    ParameterUnit TEXT,
    BaselineMaterialId TEXT,
    Notes TEXT,
    PublishOnWebsite INTEGER NOT NULL DEFAULT 0,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAtUtc TEXT NOT NULL,
    UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (MaterialId) REFERENCES NativeMaterialManagerRows(MaterialId) ON DELETE CASCADE,
    FOREIGN KEY (BaselineMaterialId) REFERENCES NativeMaterialManagerRows(MaterialId) ON DELETE SET NULL,
    FOREIGN KEY (ExperimentDefinitionId) REFERENCES ExperimentDefinitions(ExperimentDefinitionId)
);

CREATE INDEX IF NOT EXISTS IX_MaterialExperiments_MaterialId ON MaterialExperiments(MaterialId);
CREATE INDEX IF NOT EXISTS IX_MaterialExperiments_DefinitionId ON MaterialExperiments(ExperimentDefinitionId);

CREATE TABLE IF NOT EXISTS ExperimentalRuns (
    ExperimentalRunId TEXT PRIMARY KEY,
    MaterialExperimentId TEXT NOT NULL,
    ParameterValue TEXT,
    ParameterUnit TEXT,
    Status TEXT NOT NULL DEFAULT 'Planned',
    IsBaseline INTEGER NOT NULL DEFAULT 0,
    IsActive INTEGER NOT NULL DEFAULT 1,
    Notes TEXT,
    MeasuredDate TEXT,
    CreatedAtUtc TEXT NOT NULL,
    UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (MaterialExperimentId) REFERENCES MaterialExperiments(MaterialExperimentId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_ExperimentalRuns_SeriesId ON ExperimentalRuns(MaterialExperimentId);

CREATE TABLE IF NOT EXISTS ExperimentalMeasurements (
    ExperimentalMeasurementId TEXT PRIMARY KEY,
    ExperimentalRunId TEXT NOT NULL,
    MeasurementType TEXT NOT NULL,
    Unit TEXT,
    Orientation TEXT, RawUnit TEXT, ResultUnit TEXT,
    Sample1 TEXT, Sample2 TEXT, Sample3 TEXT, Sample4 TEXT, Sample5 TEXT,
    Sample6 TEXT, Sample7 TEXT, Sample8 TEXT, Sample9 TEXT, Sample10 TEXT,
    ResultAverage TEXT, ResultStdDev TEXT, ResultCv TEXT, ResultCount TEXT, ResultConfidence TEXT,
    Notes TEXT,
    UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (ExperimentalRunId) REFERENCES ExperimentalRuns(ExperimentalRunId) ON DELETE CASCADE,
    UNIQUE (ExperimentalRunId, MeasurementType, Orientation)
);
CREATE INDEX IF NOT EXISTS IX_ExperimentalMeasurements_RunId ON ExperimentalMeasurements(ExperimentalRunId);

CREATE TABLE IF NOT EXISTS WebsiteTemplates (
    WebsiteTemplateId TEXT PRIMARY KEY,
    TemplateName TEXT NOT NULL,
    TemplateVersion TEXT NOT NULL,
    HtmlContent TEXT NOT NULL,
    ContentHash TEXT NOT NULL,
    IsActive INTEGER NOT NULL DEFAULT 0,
    CreatedAtUtc TEXT NOT NULL,
    UpdatedAtUtc TEXT NOT NULL,
    SourceFileName TEXT,
    Notes TEXT
);
CREATE UNIQUE INDEX IF NOT EXISTS IX_WebsiteTemplates_ContentHash ON WebsiteTemplates(ContentHash);
CREATE INDEX IF NOT EXISTS IX_WebsiteTemplates_Active ON WebsiteTemplates(IsActive, UpdatedAtUtc DESC);

INSERT OR IGNORE INTO ExperimentDefinitions (ExperimentDefinitionId, Name, ParameterKey, DefaultUnit, Description, IsActive, SortOrder) VALUES
('EXP-TEMP', 'Different Temperature', 'temperature', '°C', 'Compare print temperature while keeping the material identity linked by MaterialID.', 1, 10),
('EXP-NOZZLE', 'Different Nozzle Size', 'nozzle_size', 'mm', 'Compare nozzle diameter.', 1, 20),
('EXP-LAYER', 'Different Layer Height', 'layer_height', 'mm', 'Compare layer height.', 1, 30),
('EXP-WIDTH', 'Different Extrusion Width', 'extrusion_width', 'mm', 'Compare extrusion width.', 1, 40),
('EXP-COOLING', 'Different Cooling', 'cooling', '%', 'Compare part cooling.', 1, 50),
('EXP-WALLS', 'Different Outer Walls', 'outer_walls', 'walls', 'Compare wall count.', 1, 60),
('EXP-SPEED', 'Different Print Speed', 'print_speed', 'mm/s', 'Compare print speed.', 1, 70),
('EXP-DRYING', 'Different Drying', 'drying', 'hours', 'Compare drying conditions.', 1, 80),
('EXP-ANNEAL', 'Annealing', 'annealing', '°C / min', 'Compare annealing conditions.', 1, 90),
('EXP-CUSTOM', 'Custom Experiment', 'custom', '', 'User-defined experimental parameter.', 1, 100);

INSERT OR IGNORE INTO DeploymentSettings (SettingsId, FtpsHost, FtpsPort, FtpsUserName, UpdatedAtUtc)
VALUES (1, '', 21, '', CURRENT_TIMESTAMP);

DROP TABLE IF EXISTS MaterialsImport;";
        command.ExecuteNonQuery();
        EnsureNativeSettingsRowsKeySchema(connection);
        EnsureColumn(connection, "Manufacturers", "DisplayName", "TEXT");
        EnsureColumn(connection, "Manufacturers", "Country", "TEXT");
        EnsureColumn(connection, "Manufacturers", "Founded", "TEXT");
        EnsureColumn(connection, "Manufacturers", "LogoUrl", "TEXT");
        EnsureColumn(connection, "Manufacturers", "Description", "TEXT");
        EnsureColumn(connection, "Manufacturers", "EngineeringFocus", "TEXT");
        EnsureColumn(connection, "Manufacturers", "MaterialCategories", "TEXT");
        EnsureColumn(connection, "Manufacturers", "Strengths", "TEXT");
        EnsureColumn(connection, "Manufacturers", "Weaknesses", "TEXT");
        EnsureColumn(connection, "Manufacturers", "Sustainability", "TEXT");
        EnsureColumn(connection, "Manufacturers", "TypicalApplications", "TEXT");
        EnsureColumn(connection, "Manufacturers", "Headquarters", "TEXT");
        EnsureColumn(connection, "Manufacturers", "Notes", "TEXT");
        EnsureColumn(connection, "Manufacturers", "SortOrder", "INTEGER NOT NULL DEFAULT 100");
        EnsureColumn(connection, "Manufacturers", "IsActive", "INTEGER NOT NULL DEFAULT 1");
        EnsureColumn(connection, "Manufacturers", "CreatedAtUtc", "TEXT");
        EnsureColumn(connection, "Manufacturers", "UpdatedAtUtc", "TEXT");
        EnsureWebsiteTemplateSeed(connection);

        EnsureColumn(connection, "MaterialExperiments", "PublishOnWebsite", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "ExperimentalRuns", "MeasuredDate", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "Orientation", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "RawUnit", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "ResultUnit", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "Sample6", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "Sample7", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "Sample8", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "Sample9", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "Sample10", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "ResultAverage", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "ResultStdDev", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "ResultCv", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "ResultCount", "TEXT");
        EnsureColumn(connection, "ExperimentalMeasurements", "ResultConfidence", "TEXT");
        EnsureColumn(connection, "NativeMeasurementNotes", "MeasuredDate", "TEXT");
        EnsureColumn(connection, "VideoIdeaQueue", "PublishDate", "TEXT");
        EnsureColumn(connection, "VideoIdeaQueue", "TargetWeek", "TEXT");
        EnsureColumn(connection, "VideoIdeaQueue", "Series", "TEXT");
        EnsureColumn(connection, "VideoIdeaQueue", "EpisodeOrder", "TEXT");
        EnsureColumn(connection, "VideoIdeaQueue", "Effort", "TEXT");
        EnsureColumn(connection, "VideoIdeaQueue", "MaterialId", "TEXT");
        EnsureColumn(connection, "PurchaseOrders", "LifecycleStatus", "TEXT");
        EnsureColumn(connection, "PurchaseOrders", "ReceivedDate", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "ReceivedQuantity", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "ReceivingStatus", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "StorageLocation", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "InventoryCategory", "TEXT");
        EnsureColumn(connection, "PurchaseOrders", "ShippingAllocationMethod", "TEXT");
        EnsureColumn(connection, "PurchaseOrders", "TaxAllocationMethod", "TEXT");
        EnsureColumn(connection, "PurchaseOrders", "CustomsAllocationMethod", "TEXT");
        EnsureColumn(connection, "PurchaseOrders", "FeeAllocationMethod", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "IncludeInCostAllocation", "INTEGER");
        EnsureColumn(connection, "PurchaseOrderLines", "ManualShippingAllocation", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "ManualTaxAllocation", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "ManualCustomsAllocation", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "ManualFeesAllocation", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "NetLineCost", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "AllocatedShipping", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "AllocatedTax", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "AllocatedCustoms", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "AllocatedFees", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "LandedLineCost", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "LandedUnitCost", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "LandedCostPerKg", "TEXT");
        EnsureColumn(connection, "PurchaseOrderLines", "AllocationStatus", "TEXT");
        EnsureColumn(connection, "InventorySpoolItems", "CustomsAmount", "TEXT");
        EnsureColumn(connection, "InventorySpoolItems", "OtherFeesAmount", "TEXT");
        EnsureColumn(connection, "InventorySpoolItems", "LandedCostAmount", "TEXT");

        EnsureColumn(connection, "NativeMaterialManagerRows", "ManufacturerSku", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "InventoryId", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PurchaseId", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PurchasedFrom", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "SupplierUrl", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "OrderNumber", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "BatchNumber", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "StorageLocation", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "InventoryStatus", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "Quantity", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "RemainingWeightG", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PurchasePriceAmount", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PurchaseCurrency", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "ShippingAmount", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "VatAmount", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PurchaseDate", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "MsrpAmount", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "MsrpCurrency", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "MsrpUsd", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "LandedCostAmount", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "LandedCostCurrency", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "LandedCostUsd", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "MsrpUsdPerKg", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "LandedCostUsdPerKg", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PriceCheckedDate", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "NozzleTemperatureMinC", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "NozzleTemperatureRecommendedC", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "NozzleTemperatureMaxC", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "BedTemperatureMinC", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "BedTemperatureRecommendedC", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "BedTemperatureMaxC", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintSpeedMinMmPerS", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintSpeedRecommendedMmPerS", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintSpeedMaxMmPerS", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "CoolingRequirement", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "DryingTimeHours", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "EnclosureRequirement", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrinterProfileReference", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "SlicerProfileReference", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintingProfileId", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintingProfileKind", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "CoolingMinPercent", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "CoolingRecommendedPercent", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "CoolingMaxPercent", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "DryingTemperatureC", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "SlicerIdentity", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "SlicerVersion", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintingSettingsProvenance", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintingSettingsSourceUrl", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintingSettingsCheckedDate", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PrintingSettingsValidationNote", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "ManufacturerWebsite", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "YouTubeReviewUrl", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "ThumbnailFilename", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "Notes", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "SourcePriority", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "MaterialKey", "TEXT");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PublishPublicReports", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "NativeMaterialManagerRows", "PublishPublicTestDetails", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "NativeMaterialManagerRows", "IsArchived", "INTEGER NOT NULL DEFAULT 0");
        EnsureColumn(connection, "NativeMaterialManagerRows", "UpdatedAtUtc", "TEXT NOT NULL DEFAULT ''");
        EnsureColumn(connection, "NativeMaterialManagerRows", "ManufacturerId", "INTEGER");
        EnsureColumn(connection, "BaseMaterialCatalog", "BaseMaterialId", "INTEGER");
        EnsureColumn(connection, "NativeMaterialManagerRows", "BaseMaterialId", "INTEGER");
        using (var baseMaterialIdentity = connection.CreateCommand())
        {
            baseMaterialIdentity.CommandText = """
                UPDATE BaseMaterialCatalog
                SET BaseMaterialId = rowid
                WHERE BaseMaterialId IS NULL OR BaseMaterialId <= 0;
                CREATE UNIQUE INDEX IF NOT EXISTS UX_BaseMaterialCatalog_BaseMaterialId
                ON BaseMaterialCatalog(BaseMaterialId);
                CREATE TRIGGER IF NOT EXISTS NativeMaterials_BaseMaterialId_InsertGuard
                BEFORE INSERT ON NativeMaterialManagerRows
                WHEN NEW.BaseMaterialId IS NOT NULL
                     AND NOT EXISTS (
                         SELECT 1 FROM BaseMaterialCatalog
                         WHERE BaseMaterialId = NEW.BaseMaterialId
                     )
                BEGIN
                    SELECT RAISE(ABORT, 'Native Material BaseMaterialId does not exist');
                END;
                CREATE TRIGGER IF NOT EXISTS NativeMaterials_BaseMaterialId_UpdateGuard
                BEFORE UPDATE OF BaseMaterialId ON NativeMaterialManagerRows
                WHEN NEW.BaseMaterialId IS NOT NULL
                     AND NOT EXISTS (
                         SELECT 1 FROM BaseMaterialCatalog
                         WHERE BaseMaterialId = NEW.BaseMaterialId
                     )
                BEGIN
                    SELECT RAISE(ABORT, 'Native Material BaseMaterialId does not exist');
                END;
                CREATE TRIGGER IF NOT EXISTS BaseMaterialCatalog_ReferencedDeleteGuard
                BEFORE DELETE ON BaseMaterialCatalog
                WHEN EXISTS (
                    SELECT 1 FROM NativeMaterialManagerRows
                    WHERE BaseMaterialId = OLD.BaseMaterialId
                )
                BEGIN
                    SELECT RAISE(ABORT, 'Base Material is referenced by canonical Materials');
                END;
                """;
            baseMaterialIdentity.ExecuteNonQuery();
        }
        using (var manufacturerRelationship = connection.CreateCommand())
        {
            manufacturerRelationship.CommandText = """
                CREATE TRIGGER IF NOT EXISTS NativeMaterials_ManufacturerId_InsertGuard
                BEFORE INSERT ON NativeMaterialManagerRows
                WHEN NEW.ManufacturerId IS NOT NULL
                     AND NOT EXISTS (SELECT 1 FROM Manufacturers WHERE ManufacturerId = NEW.ManufacturerId)
                BEGIN
                    SELECT RAISE(ABORT, 'Native Material ManufacturerId does not exist');
                END;
                CREATE TRIGGER IF NOT EXISTS NativeMaterials_ManufacturerId_UpdateGuard
                BEFORE UPDATE OF ManufacturerId ON NativeMaterialManagerRows
                WHEN NEW.ManufacturerId IS NOT NULL
                     AND NOT EXISTS (SELECT 1 FROM Manufacturers WHERE ManufacturerId = NEW.ManufacturerId)
                BEGIN
                    SELECT RAISE(ABORT, 'Native Material ManufacturerId does not exist');
                END;
                CREATE TRIGGER IF NOT EXISTS Manufacturers_ReferencedDeleteGuard
                BEFORE DELETE ON Manufacturers
                WHEN EXISTS (
                    SELECT 1 FROM NativeMaterialManagerRows
                    WHERE ManufacturerId = OLD.ManufacturerId
                )
                BEGIN
                    SELECT RAISE(ABORT, 'Manufacturer is referenced by canonical Materials');
                END;
                """;
            manufacturerRelationship.ExecuteNonQuery();
        }
        EnsureColumn(connection, "InventorySpoolItems", "PurchaseOrderLineId", "TEXT");
        RetireLegacyWorkbookTablesIfCanonicalReady(connection);
    }

    private static void RetireLegacyWorkbookTablesIfCanonicalReady(SqliteConnection connection)
    {
        var canonicalReady = false;
        using (var canonical = connection.CreateCommand())
        {
            canonical.CommandText = "SELECT Value FROM AppMeta WHERE Key='NativeMeasurementsCanonicalV1' LIMIT 1;";
            var marker = canonical.ExecuteScalar()?.ToString();
            canonicalReady = string.Equals(marker, "complete", StringComparison.Ordinal);
        }
        if (!canonicalReady)
        {
            using var legacyRows = connection.CreateCommand();
            legacyRows.CommandText = """
                SELECT
                    (SELECT COUNT(*) FROM Materials) +
                    (SELECT COUNT(*) FROM TensileSamples) +
                    (SELECT COUNT(*) FROM ImpactSamples) +
                    (SELECT COUNT(*) FROM StiffnessMeasurements);
                """;
            if (Convert.ToInt64(legacyRows.ExecuteScalar(), CultureInfo.InvariantCulture) > 0) return;
        }

        using var transaction = connection.BeginTransaction();
        foreach (var table in LegacyWorkbookTablesDropOrder)
        {
            using var drop = connection.CreateCommand();
            drop.Transaction = transaction;
            drop.CommandText = $"DROP TABLE IF EXISTS {Quote(table)};";
            drop.ExecuteNonQuery();
        }
        using var update = connection.CreateCommand();
        update.Transaction = transaction;
        update.CommandText = """
            INSERT OR REPLACE INTO AppMeta(Key, Value) VALUES ('LegacyWorkbookTablesRetiredV1', 'complete');
            INSERT OR REPLACE INTO AppMeta(Key, Value) VALUES ('SchemaVersion', '33');
            """;
        update.ExecuteNonQuery();
        transaction.Commit();
    }



    public WebsiteTemplateRecord GetActiveWebsiteTemplate()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT WebsiteTemplateId, TemplateName, TemplateVersion, HtmlContent, ContentHash,
IsActive, CreatedAtUtc, UpdatedAtUtc, SourceFileName, Notes
FROM WebsiteTemplates WHERE IsActive=1 ORDER BY UpdatedAtUtc DESC LIMIT 1;";
        using var reader = command.ExecuteReader();
        if (!reader.Read()) throw new InvalidOperationException("No active website template exists in SQLite.");
        return ReadWebsiteTemplate(reader);
    }

    public IReadOnlyList<WebsiteTemplateRecord> GetWebsiteTemplates()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT WebsiteTemplateId, TemplateName, TemplateVersion, HtmlContent, ContentHash,
IsActive, CreatedAtUtc, UpdatedAtUtc, SourceFileName, Notes
FROM WebsiteTemplates ORDER BY IsActive DESC, UpdatedAtUtc DESC;";
        using var reader = command.ExecuteReader();
        var rows = new List<WebsiteTemplateRecord>();
        while (reader.Read()) rows.Add(ReadWebsiteTemplate(reader));
        return rows;
    }

    public WebsiteTemplateRecord ImportWebsiteTemplate(string html, string sourceFileName, string? notes = null)
    {
        if (string.IsNullOrWhiteSpace(html)) throw new ArgumentException("HTML template cannot be empty.", nameof(html));
        if (!html.Contains("const DATA=", StringComparison.Ordinal) && !html.Contains("const DATA =", StringComparison.Ordinal))
            throw new InvalidOperationException("The HTML file does not contain the required const DATA block.");

        CreateAutomaticBackupBeforeWrite();
        var hash = ComputeSha256(html);
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();

        using (var existing = connection.CreateCommand())
        {
            existing.Transaction = transaction;
            existing.CommandText = "SELECT WebsiteTemplateId FROM WebsiteTemplates WHERE ContentHash=$hash LIMIT 1;";
            existing.Parameters.AddWithValue("$hash", hash);
            var existingId = existing.ExecuteScalar()?.ToString();
            if (!string.IsNullOrWhiteSpace(existingId))
            {
                ActivateWebsiteTemplate(connection, transaction, existingId);
                transaction.Commit();
                return GetWebsiteTemplates().First(x => x.WebsiteTemplateId == existingId);
            }
        }

        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        var id = "WEB-" + DateTime.UtcNow.ToString("yyyyMMddHHmmssfff", CultureInfo.InvariantCulture);
        var version = "DB-" + DateTime.UtcNow.ToString("yyyy.MM.dd-HHmmss", CultureInfo.InvariantCulture);
        using (var deactivate = connection.CreateCommand())
        {
            deactivate.Transaction = transaction;
            deactivate.CommandText = "UPDATE WebsiteTemplates SET IsActive=0, UpdatedAtUtc=$now WHERE IsActive=1;";
            deactivate.Parameters.AddWithValue("$now", now);
            deactivate.ExecuteNonQuery();
        }
        using (var insert = connection.CreateCommand())
        {
            insert.Transaction = transaction;
            insert.CommandText = @"INSERT INTO WebsiteTemplates
(WebsiteTemplateId, TemplateName, TemplateVersion, HtmlContent, ContentHash, IsActive, CreatedAtUtc, UpdatedAtUtc, SourceFileName, Notes)
VALUES ($id, 'Main Website', $version, $html, $hash, 1, $now, $now, $source, $notes);";
            insert.Parameters.AddWithValue("$id", id);
            insert.Parameters.AddWithValue("$version", version);
            insert.Parameters.AddWithValue("$html", html);
            insert.Parameters.AddWithValue("$hash", hash);
            insert.Parameters.AddWithValue("$now", now);
            insert.Parameters.AddWithValue("$source", sourceFileName ?? string.Empty);
            insert.Parameters.AddWithValue("$notes", notes ?? string.Empty);
            insert.ExecuteNonQuery();
        }
        transaction.Commit();
        return GetActiveWebsiteTemplate();
    }

    public void ActivateWebsiteTemplate(string websiteTemplateId)
    {
        CreateAutomaticBackupBeforeWrite();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        ActivateWebsiteTemplate(connection, transaction, websiteTemplateId);
        transaction.Commit();
    }

    private static void ActivateWebsiteTemplate(SqliteConnection connection, SqliteTransaction transaction, string websiteTemplateId)
    {
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        using var verify = connection.CreateCommand();
        verify.Transaction = transaction;
        verify.CommandText = "SELECT COUNT(*) FROM WebsiteTemplates WHERE WebsiteTemplateId=$id;";
        verify.Parameters.AddWithValue("$id", websiteTemplateId);
        if (Convert.ToInt32(verify.ExecuteScalar(), CultureInfo.InvariantCulture) != 1)
            throw new InvalidOperationException("Website template version was not found.");

        using var deactivate = connection.CreateCommand();
        deactivate.Transaction = transaction;
        deactivate.CommandText = "UPDATE WebsiteTemplates SET IsActive=0;";
        deactivate.ExecuteNonQuery();
        using var activate = connection.CreateCommand();
        activate.Transaction = transaction;
        activate.CommandText = "UPDATE WebsiteTemplates SET IsActive=1, UpdatedAtUtc=$now WHERE WebsiteTemplateId=$id;";
        activate.Parameters.AddWithValue("$now", now);
        activate.Parameters.AddWithValue("$id", websiteTemplateId);
        activate.ExecuteNonQuery();
    }

    private void EnsureWebsiteTemplateSeed(SqliteConnection connection)
    {
        using var count = connection.CreateCommand();
        count.CommandText = "SELECT COUNT(*) FROM WebsiteTemplates;";
        if (Convert.ToInt32(count.ExecuteScalar(), CultureInfo.InvariantCulture) > 0) return;

        // Distributable builds must not seed an owner's website/data snapshot.
        // Website templates remain SQLite-governed and are imported explicitly.
        return;
    }

    private static WebsiteTemplateRecord ReadWebsiteTemplate(SqliteDataReader reader) => new()
    {
        WebsiteTemplateId = reader.GetString(0), TemplateName = reader.GetString(1), TemplateVersion = reader.GetString(2),
        HtmlContent = reader.GetString(3), ContentHash = reader.GetString(4), IsActive = reader.GetInt32(5) != 0,
        CreatedAtUtc = reader.GetString(6), UpdatedAtUtc = reader.GetString(7),
        SourceFileName = reader.IsDBNull(8) ? null : reader.GetString(8), Notes = reader.IsDBNull(9) ? null : reader.GetString(9)
    };

    private static string ComputeSha256(string value) => Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(value))).ToLowerInvariant();

    private static void EnsureColumn(SqliteConnection connection, string tableName, string columnName, string columnType)
    {
        if (TableHasColumn(connection, tableName, columnName)) return;

        using var command = connection.CreateCommand();
        command.CommandText = $"ALTER TABLE {Quote(tableName)} ADD COLUMN {Quote(columnName)} {columnType};";
        command.ExecuteNonQuery();
    }

    private static void EnsureNativeSettingsRowsKeySchema(SqliteConnection connection)
    {
        using var info = connection.CreateCommand();
        info.CommandText = "PRAGMA table_info(NativeSettingsRows);";
        using var reader = info.ExecuteReader();
        var primaryKeyColumns = new List<(int Order, string Name)>();
        while (reader.Read())
        {
            var order = Convert.ToInt32(reader["pk"], CultureInfo.InvariantCulture);
            if (order > 0) primaryKeyColumns.Add((order, reader["name"]?.ToString() ?? string.Empty));
        }
        reader.Close();
        var expected = new[] { "Section", "Parameter", "Unit", "UsedBy" };
        if (primaryKeyColumns.OrderBy(item => item.Order).Select(item => item.Name).SequenceEqual(expected, StringComparer.OrdinalIgnoreCase)) return;

        using var transaction = connection.BeginTransaction();
        using (var rename = connection.CreateCommand()) { rename.Transaction=transaction; rename.CommandText="ALTER TABLE NativeSettingsRows RENAME TO NativeSettingsRows_PreV29;"; rename.ExecuteNonQuery(); }
        using (var create = connection.CreateCommand()) { create.Transaction=transaction; create.CommandText=@"CREATE TABLE NativeSettingsRows (
Section TEXT NOT NULL, Parameter TEXT NOT NULL, Value TEXT, Unit TEXT NOT NULL, UsedBy TEXT NOT NULL, Notes TEXT, UpdatedAtUtc TEXT NOT NULL,
PRIMARY KEY (Section, Parameter, Unit, UsedBy));"; create.ExecuteNonQuery(); }
        using (var copy = connection.CreateCommand()) { copy.Transaction=transaction; copy.CommandText=@"INSERT OR REPLACE INTO NativeSettingsRows
(Section,Parameter,Value,Unit,UsedBy,Notes,UpdatedAtUtc)
SELECT Section,Parameter,Value,COALESCE(Unit,''),UsedBy,Notes,UpdatedAtUtc FROM NativeSettingsRows_PreV29;"; copy.ExecuteNonQuery(); }
        using (var drop = connection.CreateCommand()) { drop.Transaction=transaction; drop.CommandText="DROP TABLE NativeSettingsRows_PreV29;"; drop.ExecuteNonQuery(); }
        transaction.Commit();
    }

    public DeploymentSettingsRecord LoadDeploymentSettings()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT FtpsHost, FtpsPort, FtpsUserName, UpdatedAtUtc FROM DeploymentSettings WHERE SettingsId=1;";
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return new DeploymentSettingsRecord();
        return new DeploymentSettingsRecord
        {
            FtpsHost = reader.GetString(0), FtpsPort = reader.GetInt32(1), FtpsUserName = reader.GetString(2),
            UpdatedAtUtc = reader.IsDBNull(3) ? string.Empty : reader.GetString(3)
        };
    }

    public List<NativeSettingRecord> LoadNativeSettingsRows()
    {
        using var connection = new SqliteConnection(ConnectionString); connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Section,Parameter,Value,Unit,UsedBy,Notes,UpdatedAtUtc FROM NativeSettingsRows ORDER BY Section,Parameter,UsedBy;";
        using var reader = command.ExecuteReader(); var rows = new List<NativeSettingRecord>();
        string Text(int i) => reader.IsDBNull(i) ? string.Empty : reader.GetString(i);
        while (reader.Read()) rows.Add(new NativeSettingRecord { Section=Text(0), Parameter=Text(1), Value=Text(2), Unit=Text(3), UsedBy=Text(4), Notes=Text(5), UpdatedAtUtc=Text(6) });
        return rows;
    }

    public void ReplaceNativeSettingsRows(IEnumerable<NativeSettingRecord> settings)
    {
        CreateThrottledAutomaticBackupBeforeWrite();
        using var connection = new SqliteConnection(ConnectionString); connection.Open();
        using var transaction = connection.BeginTransaction();
        using (var clear = connection.CreateCommand()) { clear.Transaction=transaction; clear.CommandText="DELETE FROM NativeSettingsRows;"; clear.ExecuteNonQuery(); }
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        foreach (var row in settings.Where(x => !string.IsNullOrWhiteSpace(x.Section) && !string.IsNullOrWhiteSpace(x.Parameter)))
        {
            using var insert=connection.CreateCommand(); insert.Transaction=transaction;
            insert.CommandText="INSERT INTO NativeSettingsRows VALUES ($section,$parameter,$value,$unit,$usedBy,$notes,$updated);";
            insert.Parameters.AddWithValue("$section",row.Section.Trim()); insert.Parameters.AddWithValue("$parameter",row.Parameter.Trim()); insert.Parameters.AddWithValue("$value",row.Value??string.Empty); insert.Parameters.AddWithValue("$unit",row.Unit??string.Empty); insert.Parameters.AddWithValue("$usedBy",row.UsedBy??string.Empty); insert.Parameters.AddWithValue("$notes",row.Notes??string.Empty); insert.Parameters.AddWithValue("$updated",now); insert.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    public void SaveDeploymentSettings(DeploymentSettingsRecord settings)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"INSERT INTO DeploymentSettings (SettingsId, FtpsHost, FtpsPort, FtpsUserName, UpdatedAtUtc)
VALUES (1, $host, $port, $user, $updated)
ON CONFLICT(SettingsId) DO UPDATE SET FtpsHost=excluded.FtpsHost, FtpsPort=excluded.FtpsPort,
FtpsUserName=excluded.FtpsUserName, UpdatedAtUtc=excluded.UpdatedAtUtc;";
        command.Parameters.AddWithValue("$host", settings.FtpsHost.Trim());
        command.Parameters.AddWithValue("$port", settings.FtpsPort);
        command.Parameters.AddWithValue("$user", settings.FtpsUserName.Trim());
        command.Parameters.AddWithValue("$updated", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
        command.ExecuteNonQuery();
    }

    public List<BaseMaterialCatalogRecord> LoadBaseMaterialCatalog()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT BaseMaterialId, BaseMaterial, Category, SortOrder,
NozzleTemperatureMinC, NozzleTemperatureRecommendedC, NozzleTemperatureMaxC,
BedTemperatureMinC, BedTemperatureRecommendedC, BedTemperatureMaxC,
PrintSpeedMinMmPerS, PrintSpeedRecommendedMmPerS, PrintSpeedMaxMmPerS,
CoolingMinPercent, CoolingRecommendedPercent, CoolingMaxPercent, CoolingGuidance,
DryingTemperatureC, DryingTimeHours, EnclosureRequirement,
PrinterProfileReference, SlicerProfileReference, ProfileId, ProfileKind, UpdatedAtUtc
FROM BaseMaterialCatalog ORDER BY CAST(SortOrder AS INTEGER), BaseMaterial;";
        using var reader = command.ExecuteReader();
        var rows = new List<BaseMaterialCatalogRecord>();
        string Text(int index) => reader.IsDBNull(index) ? string.Empty : reader.GetString(index);
        while (reader.Read())
        {
            rows.Add(new BaseMaterialCatalogRecord
            {
                BaseMaterialId = reader.GetInt64(0),
                BaseMaterial = Text(1), Category = Text(2), SortOrder = Text(3),
                NozzleTemperatureMinC = Text(4), NozzleTemperatureRecommendedC = Text(5), NozzleTemperatureMaxC = Text(6),
                BedTemperatureMinC = Text(7), BedTemperatureRecommendedC = Text(8), BedTemperatureMaxC = Text(9),
                PrintSpeedMinMmPerS = Text(10), PrintSpeedRecommendedMmPerS = Text(11), PrintSpeedMaxMmPerS = Text(12),
                CoolingMinPercent = Text(13), CoolingRecommendedPercent = Text(14), CoolingMaxPercent = Text(15), CoolingGuidance = Text(16),
                DryingTemperatureC = Text(17), DryingTimeHours = Text(18), EnclosureRequirement = Text(19),
                PrinterProfileReference = Text(20), SlicerProfileReference = Text(21), ProfileId = Text(22), ProfileKind = Text(23), UpdatedAtUtc = Text(24)
            });
        }
        return rows;
    }

    public void ReplaceBaseMaterialCatalog(IEnumerable<BaseMaterialCatalogRecord> records)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        var retainedIds = new HashSet<long>();
        foreach (var row in records.Where(x => x.BaseMaterialId > 0 && !string.IsNullOrWhiteSpace(x.BaseMaterial)))
        {
            retainedIds.Add(row.BaseMaterialId);
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText = @"INSERT INTO BaseMaterialCatalog (
BaseMaterialId,BaseMaterial,Category,SortOrder,NozzleTemperatureMinC,
NozzleTemperatureRecommendedC,NozzleTemperatureMaxC,BedTemperatureMinC,
BedTemperatureRecommendedC,BedTemperatureMaxC,PrintSpeedMinMmPerS,
PrintSpeedRecommendedMmPerS,PrintSpeedMaxMmPerS,CoolingMinPercent,
CoolingRecommendedPercent,CoolingMaxPercent,CoolingGuidance,DryingTemperatureC,
DryingTimeHours,EnclosureRequirement,PrinterProfileReference,
SlicerProfileReference,ProfileId,ProfileKind,UpdatedAtUtc) VALUES (
$id,$base,$category,$sort,$nmin,$nrec,$nmax,$bmin,$brec,$bmax,$smin,$srec,$smax,
$cmin,$crec,$cmax,$cooling,$dtemp,$dhours,$enclosure,$printer,$slicer,$profileId,$profileKind,$updated)
ON CONFLICT(BaseMaterialId) DO UPDATE SET
BaseMaterial=excluded.BaseMaterial,Category=excluded.Category,SortOrder=excluded.SortOrder,
NozzleTemperatureMinC=excluded.NozzleTemperatureMinC,
NozzleTemperatureRecommendedC=excluded.NozzleTemperatureRecommendedC,
NozzleTemperatureMaxC=excluded.NozzleTemperatureMaxC,
BedTemperatureMinC=excluded.BedTemperatureMinC,
BedTemperatureRecommendedC=excluded.BedTemperatureRecommendedC,
BedTemperatureMaxC=excluded.BedTemperatureMaxC,
PrintSpeedMinMmPerS=excluded.PrintSpeedMinMmPerS,
PrintSpeedRecommendedMmPerS=excluded.PrintSpeedRecommendedMmPerS,
PrintSpeedMaxMmPerS=excluded.PrintSpeedMaxMmPerS,
CoolingMinPercent=excluded.CoolingMinPercent,
CoolingRecommendedPercent=excluded.CoolingRecommendedPercent,
CoolingMaxPercent=excluded.CoolingMaxPercent,CoolingGuidance=excluded.CoolingGuidance,
DryingTemperatureC=excluded.DryingTemperatureC,DryingTimeHours=excluded.DryingTimeHours,
EnclosureRequirement=excluded.EnclosureRequirement,
PrinterProfileReference=excluded.PrinterProfileReference,
SlicerProfileReference=excluded.SlicerProfileReference,ProfileId=excluded.ProfileId,
ProfileKind=excluded.ProfileKind,UpdatedAtUtc=excluded.UpdatedAtUtc;";
            var values = new Dictionary<string, string>
            {
                ["$base"] = row.BaseMaterial.Trim(), ["$category"] = row.Category, ["$sort"] = row.SortOrder,
                ["$nmin"] = row.NozzleTemperatureMinC, ["$nrec"] = row.NozzleTemperatureRecommendedC, ["$nmax"] = row.NozzleTemperatureMaxC,
                ["$bmin"] = row.BedTemperatureMinC, ["$brec"] = row.BedTemperatureRecommendedC, ["$bmax"] = row.BedTemperatureMaxC,
                ["$smin"] = row.PrintSpeedMinMmPerS, ["$srec"] = row.PrintSpeedRecommendedMmPerS, ["$smax"] = row.PrintSpeedMaxMmPerS,
                ["$cmin"] = row.CoolingMinPercent, ["$crec"] = row.CoolingRecommendedPercent, ["$cmax"] = row.CoolingMaxPercent,
                ["$cooling"] = row.CoolingGuidance, ["$dtemp"] = row.DryingTemperatureC, ["$dhours"] = row.DryingTimeHours,
                ["$enclosure"] = row.EnclosureRequirement, ["$printer"] = row.PrinterProfileReference, ["$slicer"] = row.SlicerProfileReference,
                ["$profileId"] = row.ProfileId, ["$profileKind"] = row.ProfileKind,
                ["$updated"] = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)
            };
            insert.Parameters.AddWithValue("$id", row.BaseMaterialId);
            foreach (var value in values) insert.Parameters.AddWithValue(value.Key, value.Value ?? string.Empty);
            insert.ExecuteNonQuery();
        }
        using (var existing = connection.CreateCommand())
        {
            existing.Transaction = transaction;
            existing.CommandText = "SELECT BaseMaterialId FROM BaseMaterialCatalog;";
            var deleteIds = new List<long>();
            using (var reader = existing.ExecuteReader())
                while (reader.Read())
                    if (!retainedIds.Contains(reader.GetInt64(0))) deleteIds.Add(reader.GetInt64(0));
            foreach (var id in deleteIds)
            {
                using var delete = connection.CreateCommand();
                delete.Transaction = transaction;
                delete.CommandText = "DELETE FROM BaseMaterialCatalog WHERE BaseMaterialId=$id;";
                delete.Parameters.AddWithValue("$id", id);
                delete.ExecuteNonQuery();
            }
        }
        transaction.Commit();
    }

    public void ReplaceNativeMaterialManagerRows(IEnumerable<NativeMaterialRecord> materials)
    {
        CreateThrottledAutomaticBackupBeforeWrite();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();
        using var transaction = connection.BeginTransaction();

        using var insert = connection.CreateCommand();
        insert.Transaction = transaction;
        insert.CommandText = @"
INSERT INTO NativeMaterialManagerRows (
    MaterialId, ManufacturerId, Manufacturer, ProductLine, MarketingName, BaseMaterialId, BaseMaterial, MaterialCategory, VariantFinish,
    Reinforcement, Color, DiameterMm, SpoolWeightG, ManufacturerSku, InventoryId, PurchaseId, PurchasedFrom, SupplierUrl, PurchaseDate, OrderNumber, BatchNumber, StorageLocation, InventoryStatus, Quantity, RemainingWeightG, PurchasePriceAmount, PurchaseCurrency, ShippingAmount, VatAmount, MsrpAmount, MsrpCurrency, MsrpUsd, LandedCostAmount, LandedCostCurrency, LandedCostUsd, MsrpUsdPerKg, LandedCostUsdPerKg, PriceCheckedDate, NozzleTemperatureMinC, NozzleTemperatureRecommendedC, NozzleTemperatureMaxC, BedTemperatureMinC, BedTemperatureRecommendedC, BedTemperatureMaxC, PrintSpeedMinMmPerS, PrintSpeedRecommendedMmPerS, PrintSpeedMaxMmPerS, CoolingRequirement, DryingTimeHours, EnclosureRequirement, PrinterProfileReference, SlicerProfileReference, PrintingProfileId, PrintingProfileKind, CoolingMinPercent, CoolingRecommendedPercent, CoolingMaxPercent, DryingTemperatureC, SlicerIdentity, SlicerVersion, PrintingSettingsProvenance, PrintingSettingsSourceUrl, PrintingSettingsCheckedDate, PrintingSettingsValidationNote, ManufacturerWebsite, YouTubeReviewUrl, ThumbnailFilename,
    Video, Notes, TestedStatus, InTensile, InImpact, InStiffness, SortOrder, SourcePriority, WebsiteDisplayName, MaterialKey, PublishPublicReports, PublishPublicTestDetails, IsArchived, UpdatedAtUtc
) VALUES (
    $MaterialId, $ManufacturerId, $Manufacturer, $ProductLine, $MarketingName, $BaseMaterialId, $BaseMaterial, $MaterialCategory, $VariantFinish,
    $Reinforcement, $Color, $DiameterMm, $SpoolWeightG, $ManufacturerSku, $InventoryId, $PurchaseId, $PurchasedFrom, $SupplierUrl, $PurchaseDate, $OrderNumber, $BatchNumber, $StorageLocation, $InventoryStatus, $Quantity, $RemainingWeightG, $PurchasePriceAmount, $PurchaseCurrency, $ShippingAmount, $VatAmount, $MsrpAmount, $MsrpCurrency, $MsrpUsd, $LandedCostAmount, $LandedCostCurrency, $LandedCostUsd, $MsrpUsdPerKg, $LandedCostUsdPerKg, $PriceCheckedDate, $NozzleTemperatureMinC, $NozzleTemperatureRecommendedC, $NozzleTemperatureMaxC, $BedTemperatureMinC, $BedTemperatureRecommendedC, $BedTemperatureMaxC, $PrintSpeedMinMmPerS, $PrintSpeedRecommendedMmPerS, $PrintSpeedMaxMmPerS, $CoolingRequirement, $DryingTimeHours, $EnclosureRequirement, $PrinterProfileReference, $SlicerProfileReference, $PrintingProfileId, $PrintingProfileKind, $CoolingMinPercent, $CoolingRecommendedPercent, $CoolingMaxPercent, $DryingTemperatureC, $SlicerIdentity, $SlicerVersion, $PrintingSettingsProvenance, $PrintingSettingsSourceUrl, $PrintingSettingsCheckedDate, $PrintingSettingsValidationNote, $ManufacturerWebsite, $YouTubeReviewUrl, $ThumbnailFilename,
    $Video, $Notes, $TestedStatus, $InTensile, $InImpact, $InStiffness, $SortOrder, $SourcePriority, $WebsiteDisplayName, $MaterialKey, $PublishPublicReports, $PublishPublicTestDetails, $IsArchived, $UpdatedAtUtc
)
ON CONFLICT(MaterialId) DO UPDATE SET
    ManufacturerId=excluded.ManufacturerId,
    Manufacturer=excluded.Manufacturer,
    ProductLine=excluded.ProductLine,
    MarketingName=excluded.MarketingName,
    BaseMaterialId=excluded.BaseMaterialId,
    BaseMaterial=excluded.BaseMaterial,
    MaterialCategory=excluded.MaterialCategory,
    VariantFinish=excluded.VariantFinish,
    Reinforcement=excluded.Reinforcement,
    Color=excluded.Color,
    DiameterMm=excluded.DiameterMm,
    SpoolWeightG=excluded.SpoolWeightG,
    ManufacturerSku=excluded.ManufacturerSku,
    InventoryId=excluded.InventoryId,
    PurchaseId=excluded.PurchaseId,
    PurchasedFrom=excluded.PurchasedFrom,
    SupplierUrl=excluded.SupplierUrl,
    PurchaseDate=excluded.PurchaseDate,
    OrderNumber=excluded.OrderNumber,
    BatchNumber=excluded.BatchNumber,
    StorageLocation=excluded.StorageLocation,
    InventoryStatus=excluded.InventoryStatus,
    Quantity=excluded.Quantity,
    RemainingWeightG=excluded.RemainingWeightG,
    PurchasePriceAmount=excluded.PurchasePriceAmount,
    PurchaseCurrency=excluded.PurchaseCurrency,
    ShippingAmount=excluded.ShippingAmount,
    VatAmount=excluded.VatAmount,
    MsrpAmount=excluded.MsrpAmount,
    MsrpCurrency=excluded.MsrpCurrency,
    MsrpUsd=excluded.MsrpUsd,
    LandedCostAmount=excluded.LandedCostAmount,
    LandedCostCurrency=excluded.LandedCostCurrency,
    LandedCostUsd=excluded.LandedCostUsd,
    MsrpUsdPerKg=excluded.MsrpUsdPerKg,
    LandedCostUsdPerKg=excluded.LandedCostUsdPerKg,
    PriceCheckedDate=excluded.PriceCheckedDate,
    NozzleTemperatureMinC=excluded.NozzleTemperatureMinC,
    NozzleTemperatureRecommendedC=excluded.NozzleTemperatureRecommendedC,
    NozzleTemperatureMaxC=excluded.NozzleTemperatureMaxC,
    BedTemperatureMinC=excluded.BedTemperatureMinC,
    BedTemperatureRecommendedC=excluded.BedTemperatureRecommendedC,
    BedTemperatureMaxC=excluded.BedTemperatureMaxC,
    PrintSpeedMinMmPerS=excluded.PrintSpeedMinMmPerS,
    PrintSpeedRecommendedMmPerS=excluded.PrintSpeedRecommendedMmPerS,
    PrintSpeedMaxMmPerS=excluded.PrintSpeedMaxMmPerS,
    CoolingRequirement=excluded.CoolingRequirement,
    DryingTimeHours=excluded.DryingTimeHours,
    EnclosureRequirement=excluded.EnclosureRequirement,
    PrinterProfileReference=excluded.PrinterProfileReference,
    SlicerProfileReference=excluded.SlicerProfileReference,
    PrintingProfileId=excluded.PrintingProfileId,
    PrintingProfileKind=excluded.PrintingProfileKind,
    CoolingMinPercent=excluded.CoolingMinPercent,
    CoolingRecommendedPercent=excluded.CoolingRecommendedPercent,
    CoolingMaxPercent=excluded.CoolingMaxPercent,
    DryingTemperatureC=excluded.DryingTemperatureC,
    SlicerIdentity=excluded.SlicerIdentity,
    SlicerVersion=excluded.SlicerVersion,
    PrintingSettingsProvenance=excluded.PrintingSettingsProvenance,
    PrintingSettingsSourceUrl=excluded.PrintingSettingsSourceUrl,
    PrintingSettingsCheckedDate=excluded.PrintingSettingsCheckedDate,
    PrintingSettingsValidationNote=excluded.PrintingSettingsValidationNote,
    ManufacturerWebsite=excluded.ManufacturerWebsite,
    YouTubeReviewUrl=excluded.YouTubeReviewUrl,
    ThumbnailFilename=excluded.ThumbnailFilename,
    Video=excluded.Video,
    Notes=excluded.Notes,
    TestedStatus=excluded.TestedStatus,
    InTensile=excluded.InTensile,
    InImpact=excluded.InImpact,
    InStiffness=excluded.InStiffness,
    SortOrder=excluded.SortOrder,
    SourcePriority=excluded.SourcePriority,
    WebsiteDisplayName=excluded.WebsiteDisplayName,
    MaterialKey=excluded.MaterialKey,
    PublishPublicReports=excluded.PublishPublicReports,
    PublishPublicTestDetails=excluded.PublishPublicTestDetails,
    IsArchived=excluded.IsArchived,
    UpdatedAtUtc=excluded.UpdatedAtUtc;";

        var pMaterialId = insert.Parameters.Add("$MaterialId", SqliteType.Text);
        var pManufacturerId = insert.Parameters.Add("$ManufacturerId", SqliteType.Integer);
        var pManufacturer = insert.Parameters.Add("$Manufacturer", SqliteType.Text);
        var pProductLine = insert.Parameters.Add("$ProductLine", SqliteType.Text);
        var pMarketingName = insert.Parameters.Add("$MarketingName", SqliteType.Text);
        var pBaseMaterialId = insert.Parameters.Add("$BaseMaterialId", SqliteType.Integer);
        var pBaseMaterial = insert.Parameters.Add("$BaseMaterial", SqliteType.Text);
        var pMaterialCategory = insert.Parameters.Add("$MaterialCategory", SqliteType.Text);
        var pVariantFinish = insert.Parameters.Add("$VariantFinish", SqliteType.Text);
        var pReinforcement = insert.Parameters.Add("$Reinforcement", SqliteType.Text);
        var pColor = insert.Parameters.Add("$Color", SqliteType.Text);
        var pDiameterMm = insert.Parameters.Add("$DiameterMm", SqliteType.Text);
        var pSpoolWeightG = insert.Parameters.Add("$SpoolWeightG", SqliteType.Text);
        var pManufacturerSku = insert.Parameters.Add("$ManufacturerSku", SqliteType.Text);
        var pInventoryId = insert.Parameters.Add("$InventoryId", SqliteType.Text);
        var pPurchaseId = insert.Parameters.Add("$PurchaseId", SqliteType.Text);
        var pPurchasedFrom = insert.Parameters.Add("$PurchasedFrom", SqliteType.Text);
        var pSupplierUrl = insert.Parameters.Add("$SupplierUrl", SqliteType.Text);
        var pPurchaseDate = insert.Parameters.Add("$PurchaseDate", SqliteType.Text);
        var pOrderNumber = insert.Parameters.Add("$OrderNumber", SqliteType.Text);
        var pBatchNumber = insert.Parameters.Add("$BatchNumber", SqliteType.Text);
        var pStorageLocation = insert.Parameters.Add("$StorageLocation", SqliteType.Text);
        var pInventoryStatus = insert.Parameters.Add("$InventoryStatus", SqliteType.Text);
        var pQuantity = insert.Parameters.Add("$Quantity", SqliteType.Text);
        var pRemainingWeightG = insert.Parameters.Add("$RemainingWeightG", SqliteType.Text);
        var pPurchasePriceAmount = insert.Parameters.Add("$PurchasePriceAmount", SqliteType.Text);
        var pPurchaseCurrency = insert.Parameters.Add("$PurchaseCurrency", SqliteType.Text);
        var pShippingAmount = insert.Parameters.Add("$ShippingAmount", SqliteType.Text);
        var pVatAmount = insert.Parameters.Add("$VatAmount", SqliteType.Text);
        var pMsrpAmount = insert.Parameters.Add("$MsrpAmount", SqliteType.Text);
        var pMsrpCurrency = insert.Parameters.Add("$MsrpCurrency", SqliteType.Text);
        var pMsrpUsd = insert.Parameters.Add("$MsrpUsd", SqliteType.Text);
        var pLandedCostAmount = insert.Parameters.Add("$LandedCostAmount", SqliteType.Text);
        var pLandedCostCurrency = insert.Parameters.Add("$LandedCostCurrency", SqliteType.Text);
        var pLandedCostUsd = insert.Parameters.Add("$LandedCostUsd", SqliteType.Text);
        var pMsrpUsdPerKg = insert.Parameters.Add("$MsrpUsdPerKg", SqliteType.Text);
        var pLandedCostUsdPerKg = insert.Parameters.Add("$LandedCostUsdPerKg", SqliteType.Text);
        var pPriceCheckedDate = insert.Parameters.Add("$PriceCheckedDate", SqliteType.Text);
        var pNozzleTemperatureMinC = insert.Parameters.Add("$NozzleTemperatureMinC", SqliteType.Text);
        var pNozzleTemperatureRecommendedC = insert.Parameters.Add("$NozzleTemperatureRecommendedC", SqliteType.Text);
        var pNozzleTemperatureMaxC = insert.Parameters.Add("$NozzleTemperatureMaxC", SqliteType.Text);
        var pBedTemperatureMinC = insert.Parameters.Add("$BedTemperatureMinC", SqliteType.Text);
        var pBedTemperatureRecommendedC = insert.Parameters.Add("$BedTemperatureRecommendedC", SqliteType.Text);
        var pBedTemperatureMaxC = insert.Parameters.Add("$BedTemperatureMaxC", SqliteType.Text);
        var pPrintSpeedMinMmPerS = insert.Parameters.Add("$PrintSpeedMinMmPerS", SqliteType.Text);
        var pPrintSpeedRecommendedMmPerS = insert.Parameters.Add("$PrintSpeedRecommendedMmPerS", SqliteType.Text);
        var pPrintSpeedMaxMmPerS = insert.Parameters.Add("$PrintSpeedMaxMmPerS", SqliteType.Text);
        var pCoolingRequirement = insert.Parameters.Add("$CoolingRequirement", SqliteType.Text);
        var pDryingTimeHours = insert.Parameters.Add("$DryingTimeHours", SqliteType.Text);
        var pEnclosureRequirement = insert.Parameters.Add("$EnclosureRequirement", SqliteType.Text);
        var pPrinterProfileReference = insert.Parameters.Add("$PrinterProfileReference", SqliteType.Text);
        var pSlicerProfileReference = insert.Parameters.Add("$SlicerProfileReference", SqliteType.Text);
        var pPrintingProfileId = insert.Parameters.Add("$PrintingProfileId", SqliteType.Text);
        var pPrintingProfileKind = insert.Parameters.Add("$PrintingProfileKind", SqliteType.Text);
        var pCoolingMinPercent = insert.Parameters.Add("$CoolingMinPercent", SqliteType.Text);
        var pCoolingRecommendedPercent = insert.Parameters.Add("$CoolingRecommendedPercent", SqliteType.Text);
        var pCoolingMaxPercent = insert.Parameters.Add("$CoolingMaxPercent", SqliteType.Text);
        var pDryingTemperatureC = insert.Parameters.Add("$DryingTemperatureC", SqliteType.Text);
        var pSlicerIdentity = insert.Parameters.Add("$SlicerIdentity", SqliteType.Text);
        var pSlicerVersion = insert.Parameters.Add("$SlicerVersion", SqliteType.Text);
        var pPrintingSettingsProvenance = insert.Parameters.Add("$PrintingSettingsProvenance", SqliteType.Text);
        var pPrintingSettingsSourceUrl = insert.Parameters.Add("$PrintingSettingsSourceUrl", SqliteType.Text);
        var pPrintingSettingsCheckedDate = insert.Parameters.Add("$PrintingSettingsCheckedDate", SqliteType.Text);
        var pPrintingSettingsValidationNote = insert.Parameters.Add("$PrintingSettingsValidationNote", SqliteType.Text);
        var pManufacturerWebsite = insert.Parameters.Add("$ManufacturerWebsite", SqliteType.Text);
        var pYouTubeReviewUrl = insert.Parameters.Add("$YouTubeReviewUrl", SqliteType.Text);
        var pThumbnailFilename = insert.Parameters.Add("$ThumbnailFilename", SqliteType.Text);
        var pVideo = insert.Parameters.Add("$Video", SqliteType.Text);
        var pNotes = insert.Parameters.Add("$Notes", SqliteType.Text);
        var pTestedStatus = insert.Parameters.Add("$TestedStatus", SqliteType.Text);
        var pInTensile = insert.Parameters.Add("$InTensile", SqliteType.Text);
        var pInImpact = insert.Parameters.Add("$InImpact", SqliteType.Text);
        var pInStiffness = insert.Parameters.Add("$InStiffness", SqliteType.Text);
        var pSortOrder = insert.Parameters.Add("$SortOrder", SqliteType.Text);
        var pSourcePriority = insert.Parameters.Add("$SourcePriority", SqliteType.Text);
        var pWebsiteDisplayName = insert.Parameters.Add("$WebsiteDisplayName", SqliteType.Text);
        var pMaterialKey = insert.Parameters.Add("$MaterialKey", SqliteType.Text);
        var pPublishPublicReports = insert.Parameters.Add("$PublishPublicReports", SqliteType.Integer);
        var pPublishPublicTestDetails = insert.Parameters.Add("$PublishPublicTestDetails", SqliteType.Integer);
        var pIsArchived = insert.Parameters.Add("$IsArchived", SqliteType.Integer);
        var pUpdatedAtUtc = insert.Parameters.Add("$UpdatedAtUtc", SqliteType.Text);

        var materialList = materials
            .Where(x => !string.IsNullOrWhiteSpace(x.MaterialID))
            .ToList();
        var currentMaterialIds = materialList
            .Select(x => x.MaterialID.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var updatedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        foreach (var material in materialList)
        {
            if (string.IsNullOrWhiteSpace(material.MaterialID)) continue;

            pMaterialId.Value = material.MaterialID.Trim();
            pManufacturerId.Value = material.ManufacturerId.HasValue
                ? material.ManufacturerId.Value
                : DBNull.Value;
            pManufacturer.Value = material.Manufacturer ?? string.Empty;
            pProductLine.Value = material.ProductLine ?? string.Empty;
            pMarketingName.Value = material.MarketingName ?? string.Empty;
            pBaseMaterialId.Value = material.BaseMaterialId.HasValue
                ? material.BaseMaterialId.Value
                : DBNull.Value;
            pBaseMaterial.Value = material.BaseMaterial ?? string.Empty;
            pMaterialCategory.Value = material.MaterialCategory ?? string.Empty;
            pVariantFinish.Value = material.VariantFinish ?? string.Empty;
            pReinforcement.Value = material.Reinforcement ?? string.Empty;
            pColor.Value = material.Color ?? string.Empty;
            pDiameterMm.Value = material.DiameterMm ?? string.Empty;
            pSpoolWeightG.Value = material.SpoolWeightG ?? string.Empty;
            pManufacturerSku.Value = material.ManufacturerSku ?? string.Empty;
            pInventoryId.Value = material.InventoryId ?? string.Empty;
            pPurchaseId.Value = material.PurchaseId ?? string.Empty;
            pPurchasedFrom.Value = material.PurchasedFrom ?? string.Empty;
            pSupplierUrl.Value = material.SupplierUrl ?? string.Empty;
            pPurchaseDate.Value = material.PurchaseDate ?? string.Empty;
            pOrderNumber.Value = material.OrderNumber ?? string.Empty;
            pBatchNumber.Value = material.BatchNumber ?? string.Empty;
            pStorageLocation.Value = material.StorageLocation ?? string.Empty;
            pInventoryStatus.Value = material.InventoryStatus ?? "Unopened";
            pQuantity.Value = material.Quantity ?? "1";
            pRemainingWeightG.Value = material.RemainingWeightG ?? string.Empty;
            pPurchasePriceAmount.Value = material.PurchasePriceAmount ?? string.Empty;
            pPurchaseCurrency.Value = material.PurchaseCurrency ?? "ISK";
            pShippingAmount.Value = material.ShippingAmount ?? string.Empty;
            pVatAmount.Value = material.VatAmount ?? string.Empty;
            pMsrpAmount.Value = material.MsrpAmount ?? string.Empty;
            pMsrpCurrency.Value = material.MsrpCurrency ?? "ISK";
            pMsrpUsd.Value = material.MsrpUsd ?? string.Empty;
            pLandedCostAmount.Value = material.LandedCostAmount ?? string.Empty;
            pLandedCostCurrency.Value = material.LandedCostCurrency ?? "ISK";
            pLandedCostUsd.Value = material.LandedCostUsd ?? string.Empty;
            pMsrpUsdPerKg.Value = material.MsrpUsdPerKg ?? string.Empty;
            pLandedCostUsdPerKg.Value = material.LandedCostUsdPerKg ?? string.Empty;
            pPriceCheckedDate.Value = material.PriceCheckedDate ?? string.Empty;
            pNozzleTemperatureMinC.Value = material.NozzleTemperatureMinC ?? string.Empty;
            pNozzleTemperatureRecommendedC.Value = material.NozzleTemperatureRecommendedC ?? string.Empty;
            pNozzleTemperatureMaxC.Value = material.NozzleTemperatureMaxC ?? string.Empty;
            pBedTemperatureMinC.Value = material.BedTemperatureMinC ?? string.Empty;
            pBedTemperatureRecommendedC.Value = material.BedTemperatureRecommendedC ?? string.Empty;
            pBedTemperatureMaxC.Value = material.BedTemperatureMaxC ?? string.Empty;
            pPrintSpeedMinMmPerS.Value = material.PrintSpeedMinMmPerS ?? string.Empty;
            pPrintSpeedRecommendedMmPerS.Value = material.PrintSpeedRecommendedMmPerS ?? string.Empty;
            pPrintSpeedMaxMmPerS.Value = material.PrintSpeedMaxMmPerS ?? string.Empty;
            pCoolingRequirement.Value = material.CoolingRequirement ?? string.Empty;
            pDryingTimeHours.Value = material.DryingTimeHours ?? string.Empty;
            pEnclosureRequirement.Value = material.EnclosureRequirement ?? string.Empty;
            pPrinterProfileReference.Value = material.PrinterProfileReference ?? string.Empty;
            pSlicerProfileReference.Value = material.SlicerProfileReference ?? string.Empty;
            pPrintingProfileId.Value = material.PrintingProfileId ?? string.Empty;
            pPrintingProfileKind.Value = material.PrintingProfileKind ?? string.Empty;
            pCoolingMinPercent.Value = material.CoolingMinPercent ?? string.Empty;
            pCoolingRecommendedPercent.Value = material.CoolingRecommendedPercent ?? string.Empty;
            pCoolingMaxPercent.Value = material.CoolingMaxPercent ?? string.Empty;
            pDryingTemperatureC.Value = material.DryingTemperatureC ?? string.Empty;
            pSlicerIdentity.Value = material.SlicerIdentity ?? string.Empty;
            pSlicerVersion.Value = material.SlicerVersion ?? string.Empty;
            pPrintingSettingsProvenance.Value = material.PrintingSettingsProvenance ?? string.Empty;
            pPrintingSettingsSourceUrl.Value = material.PrintingSettingsSourceUrl ?? string.Empty;
            pPrintingSettingsCheckedDate.Value = material.PrintingSettingsCheckedDate ?? string.Empty;
            pPrintingSettingsValidationNote.Value = material.PrintingSettingsValidationNote ?? string.Empty;
            pManufacturerWebsite.Value = material.ManufacturerWebsite ?? string.Empty;
            pYouTubeReviewUrl.Value = material.YouTubeReviewUrl ?? string.Empty;
            pThumbnailFilename.Value = material.ThumbnailFilename ?? string.Empty;
            pVideo.Value = material.Video ?? string.Empty;
            pNotes.Value = material.Notes ?? string.Empty;
            pTestedStatus.Value = material.TestedStatus ?? string.Empty;
            pInTensile.Value = material.InTensile ?? string.Empty;
            pInImpact.Value = material.InImpact ?? string.Empty;
            pInStiffness.Value = material.InStiffness ?? string.Empty;
            pSortOrder.Value = material.SortOrder ?? string.Empty;
            pSourcePriority.Value = material.SourcePriority ?? string.Empty;
            pWebsiteDisplayName.Value = material.WebsiteDisplayName ?? string.Empty;
            pMaterialKey.Value = material.MaterialKey ?? string.Empty;
            pPublishPublicReports.Value = material.PublishPublicReports ? 1 : 0;
            pPublishPublicTestDetails.Value = material.PublishPublicTestDetails ? 1 : 0;
            pIsArchived.Value = material.IsArchived ? 1 : 0;
            pUpdatedAtUtc.Value = updatedAtUtc;
            insert.ExecuteNonQuery();
        }

        // Delete only materials that were genuinely removed. Existing rows are
        // updated in place so ON DELETE CASCADE cannot erase their spool items.
        using (var existingCommand = connection.CreateCommand())
        {
            existingCommand.Transaction = transaction;
            existingCommand.CommandText = "SELECT MaterialId FROM NativeMaterialManagerRows;";
            var existingIds = new List<string>();
            using (var reader = existingCommand.ExecuteReader())
            {
                while (reader.Read()) existingIds.Add(reader.GetString(0));
            }

            foreach (var existingId in existingIds.Where(id => !currentMaterialIds.Contains(id)))
            {
                using var delete = connection.CreateCommand();
                delete.Transaction = transaction;
                delete.CommandText = "DELETE FROM NativeMaterialManagerRows WHERE MaterialId = $id;";
                delete.Parameters.AddWithValue("$id", existingId);
                delete.ExecuteNonQuery();
            }
        }

        transaction.Commit();
    }

    public List<NativeMaterialRecord> LoadNativeMaterialManagerRows()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();

        using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='NativeMaterialManagerRows';";
        if (existsCommand.ExecuteScalar() is null) return new List<NativeMaterialRecord>();

        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT
    MaterialId, ManufacturerId, Manufacturer, ProductLine, MarketingName, BaseMaterialId, BaseMaterial, MaterialCategory, VariantFinish,
    Reinforcement, Color, DiameterMm, SpoolWeightG, ManufacturerSku, InventoryId, PurchaseId, PurchasedFrom, SupplierUrl, PurchaseDate, OrderNumber, BatchNumber, StorageLocation, InventoryStatus, Quantity, RemainingWeightG, PurchasePriceAmount, PurchaseCurrency, ShippingAmount, VatAmount, MsrpAmount, MsrpCurrency, MsrpUsd, LandedCostAmount, LandedCostCurrency, LandedCostUsd, MsrpUsdPerKg, LandedCostUsdPerKg, PriceCheckedDate, NozzleTemperatureMinC, NozzleTemperatureRecommendedC, NozzleTemperatureMaxC, BedTemperatureMinC, BedTemperatureRecommendedC, BedTemperatureMaxC, PrintSpeedMinMmPerS, PrintSpeedRecommendedMmPerS, PrintSpeedMaxMmPerS, CoolingRequirement, DryingTimeHours, EnclosureRequirement, PrinterProfileReference, SlicerProfileReference, PrintingProfileId, PrintingProfileKind, CoolingMinPercent, CoolingRecommendedPercent, CoolingMaxPercent, DryingTemperatureC, SlicerIdentity, SlicerVersion, PrintingSettingsProvenance, PrintingSettingsSourceUrl, PrintingSettingsCheckedDate, PrintingSettingsValidationNote, ManufacturerWebsite, YouTubeReviewUrl, ThumbnailFilename,
    Video, Notes, TestedStatus, InTensile, InImpact, InStiffness, SortOrder, SourcePriority, WebsiteDisplayName, MaterialKey, PublishPublicReports, PublishPublicTestDetails, IsArchived
FROM NativeMaterialManagerRows
ORDER BY
    CASE WHEN SortOrder IS NULL OR SortOrder = '' THEN 999999 ELSE CAST(SortOrder AS REAL) END,
    MaterialId;";

        var rows = new List<NativeMaterialRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new NativeMaterialRecord
            {
                MaterialID = ReadString(reader, "MaterialId"),
                ManufacturerId = reader["ManufacturerId"] is DBNull
                    ? null
                    : Convert.ToInt64(reader["ManufacturerId"], CultureInfo.InvariantCulture),
                Manufacturer = ReadString(reader, "Manufacturer"),
                ProductLine = ReadString(reader, "ProductLine"),
                MarketingName = ReadString(reader, "MarketingName"),
                BaseMaterialId = reader["BaseMaterialId"] is DBNull
                    ? null
                    : Convert.ToInt64(reader["BaseMaterialId"], CultureInfo.InvariantCulture),
                BaseMaterial = ReadString(reader, "BaseMaterial"),
                MaterialCategory = ReadString(reader, "MaterialCategory"),
                VariantFinish = ReadString(reader, "VariantFinish"),
                Reinforcement = ReadString(reader, "Reinforcement"),
                Color = ReadString(reader, "Color"),
                DiameterMm = ReadString(reader, "DiameterMm"),
                SpoolWeightG = ReadString(reader, "SpoolWeightG"),
                ManufacturerSku = ReadString(reader, "ManufacturerSku"),
                InventoryId = ReadString(reader, "InventoryId"),
                PurchaseId = ReadString(reader, "PurchaseId"),
                PurchasedFrom = ReadString(reader, "PurchasedFrom"),
                SupplierUrl = ReadString(reader, "SupplierUrl"),
                PurchaseDate = ReadString(reader, "PurchaseDate"),
                OrderNumber = ReadString(reader, "OrderNumber"),
                BatchNumber = ReadString(reader, "BatchNumber"),
                StorageLocation = ReadString(reader, "StorageLocation"),
                InventoryStatus = ReadString(reader, "InventoryStatus"),
                Quantity = ReadString(reader, "Quantity"),
                RemainingWeightG = ReadString(reader, "RemainingWeightG"),
                PurchasePriceAmount = ReadString(reader, "PurchasePriceAmount"),
                PurchaseCurrency = ReadString(reader, "PurchaseCurrency"),
                ShippingAmount = ReadString(reader, "ShippingAmount"),
                VatAmount = ReadString(reader, "VatAmount"),
                MsrpAmount = ReadString(reader, "MsrpAmount"),
                MsrpCurrency = ReadString(reader, "MsrpCurrency"),
                MsrpUsd = ReadString(reader, "MsrpUsd"),
                LandedCostAmount = ReadString(reader, "LandedCostAmount"),
                LandedCostCurrency = ReadString(reader, "LandedCostCurrency"),
                LandedCostUsd = ReadString(reader, "LandedCostUsd"),
                MsrpUsdPerKg = ReadString(reader, "MsrpUsdPerKg"),
                LandedCostUsdPerKg = ReadString(reader, "LandedCostUsdPerKg"),
                PriceCheckedDate = ReadString(reader, "PriceCheckedDate"),
                NozzleTemperatureMinC = ReadString(reader, "NozzleTemperatureMinC"),
                NozzleTemperatureRecommendedC = ReadString(reader, "NozzleTemperatureRecommendedC"),
                NozzleTemperatureMaxC = ReadString(reader, "NozzleTemperatureMaxC"),
                BedTemperatureMinC = ReadString(reader, "BedTemperatureMinC"),
                BedTemperatureRecommendedC = ReadString(reader, "BedTemperatureRecommendedC"),
                BedTemperatureMaxC = ReadString(reader, "BedTemperatureMaxC"),
                PrintSpeedMinMmPerS = ReadString(reader, "PrintSpeedMinMmPerS"),
                PrintSpeedRecommendedMmPerS = ReadString(reader, "PrintSpeedRecommendedMmPerS"),
                PrintSpeedMaxMmPerS = ReadString(reader, "PrintSpeedMaxMmPerS"),
                CoolingRequirement = ReadString(reader, "CoolingRequirement"),
                DryingTimeHours = ReadString(reader, "DryingTimeHours"),
                EnclosureRequirement = ReadString(reader, "EnclosureRequirement"),
                PrinterProfileReference = ReadString(reader, "PrinterProfileReference"),
                SlicerProfileReference = ReadString(reader, "SlicerProfileReference"),
                PrintingProfileId = ReadString(reader, "PrintingProfileId"),
                PrintingProfileKind = ReadString(reader, "PrintingProfileKind"),
                CoolingMinPercent = ReadString(reader, "CoolingMinPercent"),
                CoolingRecommendedPercent = ReadString(reader, "CoolingRecommendedPercent"),
                CoolingMaxPercent = ReadString(reader, "CoolingMaxPercent"),
                DryingTemperatureC = ReadString(reader, "DryingTemperatureC"),
                SlicerIdentity = ReadString(reader, "SlicerIdentity"),
                SlicerVersion = ReadString(reader, "SlicerVersion"),
                PrintingSettingsProvenance = ReadString(reader, "PrintingSettingsProvenance"),
                PrintingSettingsSourceUrl = ReadString(reader, "PrintingSettingsSourceUrl"),
                PrintingSettingsCheckedDate = ReadString(reader, "PrintingSettingsCheckedDate"),
                PrintingSettingsValidationNote = ReadString(reader, "PrintingSettingsValidationNote"),
                ManufacturerWebsite = ReadString(reader, "ManufacturerWebsite"),
                YouTubeReviewUrl = ReadString(reader, "YouTubeReviewUrl"),
                ThumbnailFilename = ReadString(reader, "ThumbnailFilename"),
                Video = ReadString(reader, "Video"),
                Notes = ReadString(reader, "Notes"),
                TestedStatus = ReadString(reader, "TestedStatus"),
                InTensile = ReadString(reader, "InTensile"),
                InImpact = ReadString(reader, "InImpact"),
                InStiffness = ReadString(reader, "InStiffness"),
                SortOrder = ReadString(reader, "SortOrder"),
                SourcePriority = ReadString(reader, "SourcePriority"),
                WebsiteDisplayName = ReadString(reader, "WebsiteDisplayName"),
                MaterialKey = ReadString(reader, "MaterialKey"),
                PublishPublicReports = ReadString(reader, "PublishPublicReports") == "1",
                PublishPublicTestDetails = ReadString(reader, "PublishPublicTestDetails") == "1",
                IsArchived = ReadString(reader, "IsArchived") == "1"
            });
        }

        return rows;
    }


    public List<InventorySpoolRecord> LoadInventorySpoolItems()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();
        using var command = connection.CreateCommand();
        command.CommandText = @"SELECT InventoryItemId, MaterialId, Status, Quantity, SpoolWeightG, RemainingWeightG, StorageLocation, BatchNumber, PurchaseId, PurchaseOrderLineId, PurchasedFrom, PurchaseDate, OrderNumber, PurchasePriceAmount, PurchaseCurrency, ShippingAmount, VatAmount, CustomsAmount, OtherFeesAmount, LandedCostAmount, Notes FROM InventorySpoolItems ORDER BY MaterialId, InventoryItemId;";
        var rows = new List<InventorySpoolRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new InventorySpoolRecord
        {
            InventoryItemId = ReadString(reader, "InventoryItemId"), MaterialId = ReadString(reader, "MaterialId"), Status = ReadString(reader, "Status"), Quantity = ReadString(reader, "Quantity"),
            SpoolWeightG = ReadString(reader, "SpoolWeightG"), RemainingWeightG = ReadString(reader, "RemainingWeightG"), StorageLocation = ReadString(reader, "StorageLocation"), BatchNumber = ReadString(reader, "BatchNumber"),
            PurchaseId = ReadString(reader, "PurchaseId"), PurchaseOrderLineId = ReadString(reader, "PurchaseOrderLineId"), PurchasedFrom = ReadString(reader, "PurchasedFrom"), PurchaseDate = ReadString(reader, "PurchaseDate"), OrderNumber = ReadString(reader, "OrderNumber"),
            PurchasePriceAmount = ReadString(reader, "PurchasePriceAmount"), PurchaseCurrency = ReadString(reader, "PurchaseCurrency"), ShippingAmount = ReadString(reader, "ShippingAmount"), VatAmount = ReadString(reader, "VatAmount"), CustomsAmount = ReadString(reader, "CustomsAmount"), OtherFeesAmount = ReadString(reader, "OtherFeesAmount"), LandedCostAmount = ReadString(reader, "LandedCostAmount"), Notes = ReadString(reader, "Notes")
        });
        return rows;
    }

    public void ReplaceInventorySpoolItems(IEnumerable<InventorySpoolRecord> items)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using (var clear = connection.CreateCommand()) { clear.Transaction = transaction; clear.CommandText = "DELETE FROM InventorySpoolItems;"; clear.ExecuteNonQuery(); }
        using var insert = connection.CreateCommand(); insert.Transaction = transaction;
        insert.CommandText = @"INSERT INTO InventorySpoolItems (InventoryItemId,MaterialId,Status,Quantity,SpoolWeightG,RemainingWeightG,StorageLocation,BatchNumber,PurchaseId,PurchaseOrderLineId,PurchasedFrom,PurchaseDate,OrderNumber,PurchasePriceAmount,PurchaseCurrency,ShippingAmount,VatAmount,CustomsAmount,OtherFeesAmount,LandedCostAmount,Notes,UpdatedAtUtc) VALUES ($id,$material,$status,$qty,$weight,$remaining,$storage,$batch,$purchase,$line,$supplier,$date,$order,$price,$currency,$shipping,$vat,$customs,$fees,$landed,$notes,$updated);";
        foreach (var x in items)
        {
            insert.Parameters.Clear(); insert.Parameters.AddWithValue("$id", x.InventoryItemId); insert.Parameters.AddWithValue("$material", x.MaterialId); insert.Parameters.AddWithValue("$status", x.Status); insert.Parameters.AddWithValue("$qty", x.Quantity); insert.Parameters.AddWithValue("$weight", x.SpoolWeightG); insert.Parameters.AddWithValue("$remaining", x.RemainingWeightG); insert.Parameters.AddWithValue("$storage", x.StorageLocation); insert.Parameters.AddWithValue("$batch", x.BatchNumber); insert.Parameters.AddWithValue("$purchase", x.PurchaseId); insert.Parameters.AddWithValue("$line", x.PurchaseOrderLineId); insert.Parameters.AddWithValue("$supplier", x.PurchasedFrom); insert.Parameters.AddWithValue("$date", x.PurchaseDate); insert.Parameters.AddWithValue("$order", x.OrderNumber); insert.Parameters.AddWithValue("$price", x.PurchasePriceAmount); insert.Parameters.AddWithValue("$currency", x.PurchaseCurrency); insert.Parameters.AddWithValue("$shipping", x.ShippingAmount); insert.Parameters.AddWithValue("$vat", x.VatAmount); insert.Parameters.AddWithValue("$customs", x.CustomsAmount); insert.Parameters.AddWithValue("$fees", x.OtherFeesAmount); insert.Parameters.AddWithValue("$landed", x.LandedCostAmount); insert.Parameters.AddWithValue("$notes", x.Notes); insert.Parameters.AddWithValue("$updated", DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)); insert.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    public List<PurchaseOrderRecord> LoadPurchaseOrders()
    {
        using var connection = new SqliteConnection(ConnectionString); connection.Open(); Initialize();
        using var command = connection.CreateCommand(); command.CommandText = "SELECT * FROM PurchaseOrders ORDER BY PurchaseDate DESC, PurchaseOrderId DESC";
        var rows = new List<PurchaseOrderRecord>(); using var reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new PurchaseOrderRecord { PurchaseOrderId=ReadString(reader,"PurchaseOrderId"), Supplier=ReadString(reader,"Supplier"), OrderNumber=ReadString(reader,"OrderNumber"), PurchaseDate=ReadString(reader,"PurchaseDate"), Currency=ReadString(reader,"Currency"), ExchangeRate=ReadString(reader,"ExchangeRate"), TaxTreatment=ReadString(reader,"TaxTreatment"), ShippingMethod=ReadString(reader,"ShippingMethod"), TrackingNumber=ReadString(reader,"TrackingNumber"), SupplierItemsTotal=ReadString(reader,"SupplierItemsTotal"), SupplierShipping=ReadString(reader,"SupplierShipping"), SupplierTax=ReadString(reader,"SupplierTax"), SupplierInvoiceTotal=ReadString(reader,"SupplierInvoiceTotal"), ImportVat=ReadString(reader,"ImportVat"), CustomsDuty=ReadString(reader,"CustomsDuty"), ClearanceFee=ReadString(reader,"ClearanceFee"), OtherFees=ReadString(reader,"OtherFees"), ShippingAllocationMethod=ReadString(reader,"ShippingAllocationMethod"), TaxAllocationMethod=ReadString(reader,"TaxAllocationMethod"), CustomsAllocationMethod=ReadString(reader,"CustomsAllocationMethod"), FeeAllocationMethod=ReadString(reader,"FeeAllocationMethod"), CostStatus=ReadString(reader,"CostStatus"), LifecycleStatus=ReadString(reader,"LifecycleStatus"), ReceivedDate=ReadString(reader,"ReceivedDate"), InvoiceFile=ReadString(reader,"InvoiceFile"), Notes=ReadString(reader,"Notes") });
        return rows;
    }

    public List<PurchaseOrderLineRecord> LoadPurchaseOrderLines()
    {
        using var connection = new SqliteConnection(ConnectionString); connection.Open(); Initialize();
        using var command = connection.CreateCommand(); command.CommandText = "SELECT * FROM PurchaseOrderLines ORDER BY PurchaseOrderId, PurchaseOrderLineId";
        var rows = new List<PurchaseOrderLineRecord>(); using var reader=command.ExecuteReader();
        while(reader.Read()) rows.Add(new PurchaseOrderLineRecord { PurchaseOrderLineId=ReadString(reader,"PurchaseOrderLineId"), PurchaseOrderId=ReadString(reader,"PurchaseOrderId"), MaterialId=ReadString(reader,"MaterialId"), InventoryCategory=string.IsNullOrWhiteSpace(ReadString(reader,"InventoryCategory")) ? "Filament" : ReadString(reader,"InventoryCategory"), Description=ReadString(reader,"Description"), Sku=ReadString(reader,"Sku"), Quantity=ReadString(reader,"Quantity"), ReceivedQuantity=ReadString(reader,"ReceivedQuantity"), ReceivingStatus=ReadString(reader,"ReceivingStatus"), StorageLocation=ReadString(reader,"StorageLocation"), UnitPrice=ReadString(reader,"UnitPrice"), DiscountAmount=ReadString(reader,"DiscountAmount"), UnitWeightG=ReadString(reader,"UnitWeightG"), IncludeInCostAllocation=ReadString(reader,"IncludeInCostAllocation") != "0", ManualShippingAllocation=ReadString(reader,"ManualShippingAllocation"), ManualTaxAllocation=ReadString(reader,"ManualTaxAllocation"), ManualCustomsAllocation=ReadString(reader,"ManualCustomsAllocation"), ManualFeesAllocation=ReadString(reader,"ManualFeesAllocation"), NetLineCost=ReadString(reader,"NetLineCost"), AllocatedShipping=ReadString(reader,"AllocatedShipping"), AllocatedTax=ReadString(reader,"AllocatedTax"), AllocatedCustoms=ReadString(reader,"AllocatedCustoms"), AllocatedFees=ReadString(reader,"AllocatedFees"), LandedLineCost=ReadString(reader,"LandedLineCost"), LandedUnitCost=ReadString(reader,"LandedUnitCost"), LandedCostPerKg=ReadString(reader,"LandedCostPerKg"), AllocationStatus=ReadString(reader,"AllocationStatus"), Notes=ReadString(reader,"Notes") });
        return rows;
    }

    public void ReplacePurchaseOrders(IEnumerable<PurchaseOrderRecord> orders, IEnumerable<PurchaseOrderLineRecord> lines)
    {
        using var connection=new SqliteConnection(ConnectionString); connection.Open(); using var tx=connection.BeginTransaction();
        using(var clear=connection.CreateCommand()){clear.Transaction=tx; clear.CommandText="DELETE FROM PurchaseOrderLines; DELETE FROM PurchaseOrders;"; clear.ExecuteNonQuery();}
        using var io=connection.CreateCommand(); io.Transaction=tx; io.CommandText=@"INSERT INTO PurchaseOrders (PurchaseOrderId,Supplier,OrderNumber,PurchaseDate,Currency,ExchangeRate,TaxTreatment,ShippingMethod,TrackingNumber,SupplierItemsTotal,SupplierShipping,SupplierTax,SupplierInvoiceTotal,ImportVat,CustomsDuty,ClearanceFee,OtherFees,ShippingAllocationMethod,TaxAllocationMethod,CustomsAllocationMethod,FeeAllocationMethod,CostStatus,LifecycleStatus,ReceivedDate,InvoiceFile,Notes,UpdatedAtUtc) VALUES ($id,$supplier,$number,$date,$currency,$rate,$tax,$shipmethod,$tracking,$items,$shipping,$suppliertax,$total,$importvat,$customs,$clearance,$other,$shipalloc,$taxalloc,$customsalloc,$feealloc,$status,$lifecycle,$receiveddate,$invoice,$notes,$updated)";
        foreach(var x in orders){io.Parameters.Clear(); io.Parameters.AddWithValue("$id",x.PurchaseOrderId);io.Parameters.AddWithValue("$supplier",x.Supplier);io.Parameters.AddWithValue("$number",x.OrderNumber);io.Parameters.AddWithValue("$date",x.PurchaseDate);io.Parameters.AddWithValue("$currency",x.Currency);io.Parameters.AddWithValue("$rate",x.ExchangeRate);io.Parameters.AddWithValue("$tax",x.TaxTreatment);io.Parameters.AddWithValue("$shipmethod",x.ShippingMethod);io.Parameters.AddWithValue("$tracking",x.TrackingNumber);io.Parameters.AddWithValue("$items",x.SupplierItemsTotal);io.Parameters.AddWithValue("$shipping",x.SupplierShipping);io.Parameters.AddWithValue("$suppliertax",x.SupplierTax);io.Parameters.AddWithValue("$total",x.SupplierInvoiceTotal);io.Parameters.AddWithValue("$importvat",x.ImportVat);io.Parameters.AddWithValue("$customs",x.CustomsDuty);io.Parameters.AddWithValue("$clearance",x.ClearanceFee);io.Parameters.AddWithValue("$other",x.OtherFees);io.Parameters.AddWithValue("$shipalloc",x.ShippingAllocationMethod);io.Parameters.AddWithValue("$taxalloc",x.TaxAllocationMethod);io.Parameters.AddWithValue("$customsalloc",x.CustomsAllocationMethod);io.Parameters.AddWithValue("$feealloc",x.FeeAllocationMethod);io.Parameters.AddWithValue("$status",x.CostStatus);io.Parameters.AddWithValue("$lifecycle",x.LifecycleStatus);io.Parameters.AddWithValue("$receiveddate",x.ReceivedDate);io.Parameters.AddWithValue("$invoice",x.InvoiceFile);io.Parameters.AddWithValue("$notes",x.Notes);io.Parameters.AddWithValue("$updated",DateTime.UtcNow.ToString("O",CultureInfo.InvariantCulture));io.ExecuteNonQuery();}
        using var il=connection.CreateCommand(); il.Transaction=tx; il.CommandText=@"INSERT INTO PurchaseOrderLines (PurchaseOrderLineId,PurchaseOrderId,MaterialId,InventoryCategory,Description,Sku,Quantity,ReceivedQuantity,ReceivingStatus,StorageLocation,UnitPrice,DiscountAmount,UnitWeightG,IncludeInCostAllocation,ManualShippingAllocation,ManualTaxAllocation,ManualCustomsAllocation,ManualFeesAllocation,NetLineCost,AllocatedShipping,AllocatedTax,AllocatedCustoms,AllocatedFees,LandedLineCost,LandedUnitCost,LandedCostPerKg,AllocationStatus,Notes,UpdatedAtUtc) VALUES ($id,$order,$material,$category,$description,$sku,$qty,$receivedqty,$receivingstatus,$storage,$price,$discount,$weight,$include,$manualshipping,$manualtax,$manualcustoms,$manualfees,$netline,$allocatedshipping,$allocatedtax,$allocatedcustoms,$allocatedfees,$landedline,$landedunit,$landedkg,$allocationstatus,$notes,$updated)";
        foreach(var x in lines.Where(x=>orders.Any(o=>o.PurchaseOrderId==x.PurchaseOrderId))){il.Parameters.Clear();il.Parameters.AddWithValue("$id",x.PurchaseOrderLineId);il.Parameters.AddWithValue("$order",x.PurchaseOrderId);il.Parameters.AddWithValue("$material",string.IsNullOrWhiteSpace(x.MaterialId)?DBNull.Value:x.MaterialId);il.Parameters.AddWithValue("$category",string.IsNullOrWhiteSpace(x.InventoryCategory)?"Filament":x.InventoryCategory);il.Parameters.AddWithValue("$description",x.Description);il.Parameters.AddWithValue("$sku",x.Sku);il.Parameters.AddWithValue("$qty",x.Quantity);il.Parameters.AddWithValue("$receivedqty",x.ReceivedQuantity);il.Parameters.AddWithValue("$receivingstatus",x.ReceivingStatus);il.Parameters.AddWithValue("$storage",x.StorageLocation);il.Parameters.AddWithValue("$price",x.UnitPrice);il.Parameters.AddWithValue("$discount",x.DiscountAmount);il.Parameters.AddWithValue("$weight",x.UnitWeightG);il.Parameters.AddWithValue("$include",x.IncludeInCostAllocation?1:0);il.Parameters.AddWithValue("$manualshipping",x.ManualShippingAllocation);il.Parameters.AddWithValue("$manualtax",x.ManualTaxAllocation);il.Parameters.AddWithValue("$manualcustoms",x.ManualCustomsAllocation);il.Parameters.AddWithValue("$manualfees",x.ManualFeesAllocation);il.Parameters.AddWithValue("$netline",x.NetLineCost);il.Parameters.AddWithValue("$allocatedshipping",x.AllocatedShipping);il.Parameters.AddWithValue("$allocatedtax",x.AllocatedTax);il.Parameters.AddWithValue("$allocatedcustoms",x.AllocatedCustoms);il.Parameters.AddWithValue("$allocatedfees",x.AllocatedFees);il.Parameters.AddWithValue("$landedline",x.LandedLineCost);il.Parameters.AddWithValue("$landedunit",x.LandedUnitCost);il.Parameters.AddWithValue("$landedkg",x.LandedCostPerKg);il.Parameters.AddWithValue("$allocationstatus",x.AllocationStatus);il.Parameters.AddWithValue("$notes",x.Notes);il.Parameters.AddWithValue("$updated",DateTime.UtcNow.ToString("O",CultureInfo.InvariantCulture));il.ExecuteNonQuery();}
        tx.Commit();
    }

    public List<VideoIdeaRecord> LoadVideoIdeas()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();

        var ideas = new List<VideoIdeaRecord>();

        using var existsCommand = connection.CreateCommand();
        existsCommand.CommandText = "SELECT name FROM sqlite_master WHERE type='table' AND name='VideoIdeaQueue';";
        if (existsCommand.ExecuteScalar() is null) return ideas;

        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT
    MaterialId,
    ProductionStatus,
    ProductionPriority,
    Notes,
    Label,
    MaterialType,
    SuggestionCategory,
    SuggestedTitle,
    SuggestedAngle,
    TalkingPoints,
    DataReason,
    Standout,
    ComparisonIdea,
    BaseMaterial,
    Category,
    Manufacturer,
    Reinforcement,
    Variant,
    ProductLine,
    OverallScore,
    TensileScore,
    ImpactScore,
    StiffnessScore,
    ConsistencyScore,
    LayerAdhesionScore,
    PublishDate,
    TargetWeek,
    Series,
    EpisodeOrder,
    Effort
FROM VideoIdeaQueue
ORDER BY IdeaId DESC;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            ideas.Add(new VideoIdeaRecord
            {
                MaterialId = ReadString(reader, "MaterialId"),
                ProductionStatus = ReadString(reader, "ProductionStatus"),
                ProductionPriority = ReadString(reader, "ProductionPriority"),
                Notes = ReadString(reader, "Notes"),
                Label = ReadString(reader, "Label"),
                MaterialType = ReadString(reader, "MaterialType"),
                SuggestionCategory = ReadString(reader, "SuggestionCategory"),
                SuggestedTitle = ReadString(reader, "SuggestedTitle"),
                SuggestedAngle = ReadString(reader, "SuggestedAngle"),
                TalkingPoints = ReadString(reader, "TalkingPoints"),
                DataReason = ReadString(reader, "DataReason"),
                Standout = ReadString(reader, "Standout"),
                ComparisonIdea = ReadString(reader, "ComparisonIdea"),
                BaseMaterial = ReadString(reader, "BaseMaterial"),
                Category = ReadString(reader, "Category"),
                Manufacturer = ReadString(reader, "Manufacturer"),
                Reinforcement = ReadString(reader, "Reinforcement"),
                Variant = ReadString(reader, "Variant"),
                ProductLine = ReadString(reader, "ProductLine"),
                OverallScore = ReadString(reader, "OverallScore"),
                TensileScore = ReadString(reader, "TensileScore"),
                ImpactScore = ReadString(reader, "ImpactScore"),
                StiffnessScore = ReadString(reader, "StiffnessScore"),
                ConsistencyScore = ReadString(reader, "ConsistencyScore"),
                LayerAdhesionScore = ReadString(reader, "LayerAdhesionScore"),
                PublishDate = ReadString(reader, "PublishDate"),
                TargetWeek = ReadString(reader, "TargetWeek"),
                Series = ReadString(reader, "Series"),
                EpisodeOrder = ReadString(reader, "EpisodeOrder"),
                Effort = ReadString(reader, "Effort")
            });
        }

        return ideas;
    }

    public void SaveVideoIdeas(IEnumerable<VideoIdeaRecord> ideas)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();

        using var transaction = connection.BeginTransaction();

        using (var clear = connection.CreateCommand())
        {
            clear.Transaction = transaction;
            clear.CommandText = "DELETE FROM VideoIdeaQueue;";
            clear.ExecuteNonQuery();
        }

        foreach (var idea in ideas)
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText = @"
INSERT INTO VideoIdeaQueue (
    CreatedAtUtc,
    MaterialId,
    ProductionStatus,
    ProductionPriority,
    Notes,
    Label,
    MaterialType,
    SuggestionCategory,
    SuggestedTitle,
    SuggestedAngle,
    TalkingPoints,
    DataReason,
    Standout,
    ComparisonIdea,
    BaseMaterial,
    Category,
    Manufacturer,
    Reinforcement,
    Variant,
    ProductLine,
    OverallScore,
    TensileScore,
    ImpactScore,
    StiffnessScore,
    ConsistencyScore,
    LayerAdhesionScore,
    PublishDate,
    TargetWeek,
    Series,
    EpisodeOrder,
    Effort
) VALUES (
    $createdAtUtc,
    $materialId,
    $productionStatus,
    $productionPriority,
    $notes,
    $label,
    $materialType,
    $suggestionCategory,
    $suggestedTitle,
    $suggestedAngle,
    $talkingPoints,
    $dataReason,
    $standout,
    $comparisonIdea,
    $baseMaterial,
    $category,
    $manufacturer,
    $reinforcement,
    $variant,
    $productLine,
    $overallScore,
    $tensileScore,
    $impactScore,
    $stiffnessScore,
    $consistencyScore,
    $layerAdhesionScore,
    $publishDate,
    $targetWeek,
    $series,
    $episodeOrder,
    $effort
);";

            insert.Parameters.AddWithValue("$createdAtUtc", DateTime.UtcNow.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
            insert.Parameters.AddWithValue("$materialId", idea.MaterialId ?? string.Empty);
            insert.Parameters.AddWithValue("$productionStatus", idea.ProductionStatus ?? "Idea");
            insert.Parameters.AddWithValue("$productionPriority", idea.ProductionPriority ?? "Medium");
            insert.Parameters.AddWithValue("$notes", idea.Notes ?? string.Empty);
            insert.Parameters.AddWithValue("$label", idea.Label ?? string.Empty);
            insert.Parameters.AddWithValue("$materialType", idea.MaterialType ?? string.Empty);
            insert.Parameters.AddWithValue("$suggestionCategory", idea.SuggestionCategory ?? string.Empty);
            insert.Parameters.AddWithValue("$suggestedTitle", idea.SuggestedTitle ?? string.Empty);
            insert.Parameters.AddWithValue("$suggestedAngle", idea.SuggestedAngle ?? string.Empty);
            insert.Parameters.AddWithValue("$talkingPoints", idea.TalkingPoints ?? string.Empty);
            insert.Parameters.AddWithValue("$dataReason", idea.DataReason ?? string.Empty);
            insert.Parameters.AddWithValue("$standout", idea.Standout ?? string.Empty);
            insert.Parameters.AddWithValue("$comparisonIdea", idea.ComparisonIdea ?? string.Empty);
            insert.Parameters.AddWithValue("$baseMaterial", idea.BaseMaterial ?? string.Empty);
            insert.Parameters.AddWithValue("$category", idea.Category ?? string.Empty);
            insert.Parameters.AddWithValue("$manufacturer", idea.Manufacturer ?? string.Empty);
            insert.Parameters.AddWithValue("$reinforcement", idea.Reinforcement ?? string.Empty);
            insert.Parameters.AddWithValue("$variant", idea.Variant ?? string.Empty);
            insert.Parameters.AddWithValue("$productLine", idea.ProductLine ?? string.Empty);
            insert.Parameters.AddWithValue("$overallScore", idea.OverallScore ?? string.Empty);
            insert.Parameters.AddWithValue("$tensileScore", idea.TensileScore ?? string.Empty);
            insert.Parameters.AddWithValue("$impactScore", idea.ImpactScore ?? string.Empty);
            insert.Parameters.AddWithValue("$stiffnessScore", idea.StiffnessScore ?? string.Empty);
            insert.Parameters.AddWithValue("$consistencyScore", idea.ConsistencyScore ?? string.Empty);
            insert.Parameters.AddWithValue("$layerAdhesionScore", idea.LayerAdhesionScore ?? string.Empty);
            insert.Parameters.AddWithValue("$publishDate", idea.PublishDate ?? string.Empty);
            insert.Parameters.AddWithValue("$targetWeek", idea.TargetWeek ?? string.Empty);
            insert.Parameters.AddWithValue("$series", idea.Series ?? string.Empty);
            insert.Parameters.AddWithValue("$episodeOrder", idea.EpisodeOrder ?? string.Empty);
            insert.Parameters.AddWithValue("$effort", idea.Effort ?? string.Empty);

            insert.ExecuteNonQuery();
        }

        transaction.Commit();
    }

    private static string ReadString(SqliteDataReader reader, string columnName)
    {
        var value = reader[columnName];
        return value == DBNull.Value ? string.Empty : value?.ToString() ?? string.Empty;
    }

    public TensileTestResult? GetTensileTestResult(string materialId)
    {
        if (string.IsNullOrWhiteSpace(materialId)) return null;

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT MaterialId, UprightMpa, FlatMpa, StdDevUpright, StdDevFlat, CvUpright, CvFlat,
       SamplesUpright, SamplesFlat, ConfidenceUpright, ConfidenceFlat, TestNotes
FROM NativeTensileResults
WHERE MaterialId = $materialId
LIMIT 1;";
        AddParameter(command, "$materialId", materialId);

        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;

        return new TensileTestResult
        {
            MaterialId = reader["MaterialId"]?.ToString() ?? string.Empty,
            UprightMpa = reader["UprightMpa"]?.ToString(),
            FlatMpa = reader["FlatMpa"]?.ToString(),
            StdDevUpright = reader["StdDevUpright"]?.ToString(),
            StdDevFlat = reader["StdDevFlat"]?.ToString(),
            CvUpright = reader["CvUpright"]?.ToString(),
            CvFlat = reader["CvFlat"]?.ToString(),
            SamplesUpright = reader["SamplesUpright"]?.ToString(),
            SamplesFlat = reader["SamplesFlat"]?.ToString(),
            ConfidenceUpright = reader["ConfidenceUpright"]?.ToString(),
            ConfidenceFlat = reader["ConfidenceFlat"]?.ToString(),
            TestNotes = reader["TestNotes"]?.ToString()
        };
    }


    public bool NativeMeasurementsAreCanonical()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return NativeMeasurementsAreCanonical(connection);
    }

    public (int TensileSamples, int ImpactSamples, int StiffnessRows, int Notes) GetNativeMeasurementCanonicalCounts()
    {
        using var connection = new SqliteConnection(ConnectionString); connection.Open();
        int Count(string table) { using var command = connection.CreateCommand(); command.CommandText = $"SELECT COUNT(*) FROM {table};"; return Convert.ToInt32(command.ExecuteScalar(), CultureInfo.InvariantCulture); }
        return (Count("NativeTensileSamples"), Count("NativeImpactSamples"), Count("NativeStiffnessMeasurements"), Count("NativeMeasurementNotes"));
    }

    private static bool NativeMeasurementsAreCanonical(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Value FROM AppMeta WHERE Key='NativeMeasurementsCanonicalV1' LIMIT 1;";
        return string.Equals(command.ExecuteScalar()?.ToString(), "complete", StringComparison.Ordinal);
    }

    public void MigrateNativeMeasurementsToSqlite(
        IReadOnlyList<NativeTensilePersistenceRecord> tensile,
        IReadOnlyList<NativeImpactPersistenceRecord> impact,
        IReadOnlyList<NativeStiffnessPersistenceRecord> stiffness,
        bool replaceCanonical = false)
    {
        var alreadyCanonical = NativeMeasurementsAreCanonical();
        if (alreadyCanonical && !replaceCanonical) return;
        if (alreadyCanonical) CreateThrottledAutomaticBackupBeforeWrite(); else CreateRequiredBackupBeforeCanonicalMigration();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        foreach (var table in new[] { "NativeTensileSamples", "NativeTensileResults", "NativeImpactSamples", "NativeStiffnessMeasurements", "NativeMeasurementNotes" })
        {
            using var clear = connection.CreateCommand(); clear.Transaction = transaction; clear.CommandText = $"DELETE FROM {table};"; clear.ExecuteNonQuery();
        }
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        foreach (var row in tensile.Where(x => !string.IsNullOrWhiteSpace(x.MaterialId)))
        {
            InsertNativeSamples(connection, transaction, "NativeTensileSamples", row.MaterialId, row.UprightSamples, row.FlatSamples, now);
            using var result = connection.CreateCommand(); result.Transaction = transaction;
            result.CommandText = @"INSERT INTO NativeTensileResults VALUES ($id,$um,$fm,$su,$sf,$cu,$cf,$nu,$nf,$fu,$ff,$notes,$now);";
            var values = new[] { row.MaterialId, row.UprightMpa, row.FlatMpa, row.StdDevUpright, row.StdDevFlat, row.CvUpright, row.CvFlat, row.SamplesUpright, row.SamplesFlat, row.ConfidenceUpright, row.ConfidenceFlat, row.TestNotes, now };
            var names = new[] { "$id", "$um", "$fm", "$su", "$sf", "$cu", "$cf", "$nu", "$nf", "$fu", "$ff", "$notes", "$now" };
            for (var x = 0; x < names.Length; x++) result.Parameters.AddWithValue(names[x], values[x] ?? string.Empty);
            result.ExecuteNonQuery();
            var hasSamples = row.UprightSamples.Concat(row.FlatSamples).Any(value => !string.IsNullOrWhiteSpace(value));
            InsertNativeMetadata(connection, transaction, row.MaterialId, "Tensile", row.TestNotes ?? string.Empty, row.MeasuredDate, hasSamples, now);
        }
        foreach (var row in impact.Where(x => !string.IsNullOrWhiteSpace(x.MaterialId)))
        {
            InsertNativeSamples(connection, transaction, "NativeImpactSamples", row.MaterialId, row.UprightSamples, row.FlatSamples, now);
            var hasSamples = row.UprightSamples.Concat(row.FlatSamples).Any(value => !string.IsNullOrWhiteSpace(value));
            InsertNativeMetadata(connection, transaction, row.MaterialId, "Impact", row.TestNotes, row.MeasuredDate, hasSamples, now);
        }
        foreach (var row in stiffness.Where(x => !string.IsNullOrWhiteSpace(x.MaterialId)))
        {
            if (string.IsNullOrWhiteSpace(row.Revolutions) && string.IsNullOrWhiteSpace(row.Degrees) && string.IsNullOrWhiteSpace(row.TestNotes) && string.IsNullOrWhiteSpace(row.MeasuredDate)) continue;
            using var insert = connection.CreateCommand(); insert.Transaction = transaction;
            insert.CommandText = "INSERT INTO NativeStiffnessMeasurements VALUES ($id,$rev,$deg,$notes,$now);";
            insert.Parameters.AddWithValue("$id", row.MaterialId); insert.Parameters.AddWithValue("$rev", row.Revolutions ?? string.Empty); insert.Parameters.AddWithValue("$deg", row.Degrees ?? string.Empty); insert.Parameters.AddWithValue("$notes", row.TestNotes ?? string.Empty); insert.Parameters.AddWithValue("$now", now); insert.ExecuteNonQuery();
            InsertNativeMetadata(connection, transaction, row.MaterialId, "Stiffness", row.TestNotes ?? string.Empty, row.MeasuredDate, true, now);
        }
        static int ExpectedSamples(IEnumerable<IReadOnlyList<string>> sets) => sets.Sum(set => set.Count(value => !string.IsNullOrWhiteSpace(value)));
        var expectedTensile = ExpectedSamples(tensile.SelectMany(row => new[] { (IReadOnlyList<string>)row.UprightSamples, row.FlatSamples }));
        var expectedImpact = ExpectedSamples(impact.SelectMany(row => new[] { (IReadOnlyList<string>)row.UprightSamples, row.FlatSamples }));
        var expectedStiffness = stiffness.Count(row => !string.IsNullOrWhiteSpace(row.MaterialId) && (!string.IsNullOrWhiteSpace(row.Revolutions) || !string.IsNullOrWhiteSpace(row.Degrees) || !string.IsNullOrWhiteSpace(row.TestNotes) || !string.IsNullOrWhiteSpace(row.MeasuredDate)));
        int Count(string table) { using var count = connection.CreateCommand(); count.Transaction = transaction; count.CommandText = $"SELECT COUNT(*) FROM {table};"; return Convert.ToInt32(count.ExecuteScalar(), CultureInfo.InvariantCulture); }
        if (Count("NativeTensileSamples") != expectedTensile || Count("NativeImpactSamples") != expectedImpact || Count("NativeStiffnessMeasurements") != expectedStiffness)
            throw new InvalidOperationException("Native measurement SQLite reconciliation failed before commit; the transaction was rolled back.");
        using (var marker = connection.CreateCommand()) { marker.Transaction = transaction; marker.CommandText = "INSERT OR REPLACE INTO AppMeta (Key,Value) VALUES ('NativeMeasurementsCanonicalV1','complete');"; marker.ExecuteNonQuery(); }
        transaction.Commit();
    }

    private static void InsertNativeSamples(SqliteConnection connection, SqliteTransaction transaction, string table, string materialId, IReadOnlyList<string> upright, IReadOnlyList<string> flat, string now)
    {
        foreach (var pair in new[] { ("Upright", upright), ("Flat", flat) })
            for (var i = 0; i < pair.Item2.Count; i++)
            {
                var value = pair.Item2[i]?.Trim() ?? string.Empty; if (value.Length == 0) continue;
                using var insert = connection.CreateCommand(); insert.Transaction = transaction;
                insert.CommandText = $"INSERT INTO {table} VALUES ($id,$orientation,$number,$value,$now);";
                insert.Parameters.AddWithValue("$id", materialId); insert.Parameters.AddWithValue("$orientation", pair.Item1); insert.Parameters.AddWithValue("$number", i + 1); insert.Parameters.AddWithValue("$value", value); insert.Parameters.AddWithValue("$now", now); insert.ExecuteNonQuery();
            }
    }

    private static void InsertNativeMetadata(SqliteConnection connection, SqliteTransaction transaction, string materialId, string testType, string notes, string measuredDate, bool hasMeasurement, string now)
    {
        if (!hasMeasurement && string.IsNullOrWhiteSpace(notes) && string.IsNullOrWhiteSpace(measuredDate)) return;
        using var insert = connection.CreateCommand(); insert.Transaction = transaction;
        insert.CommandText = "INSERT INTO NativeMeasurementNotes (MaterialId,TestType,TestNotes,MeasuredDate,UpdatedAtUtc) VALUES ($id,$type,$notes,$date,$now);";
        insert.Parameters.AddWithValue("$id", materialId); insert.Parameters.AddWithValue("$type", testType); insert.Parameters.AddWithValue("$notes", notes); insert.Parameters.AddWithValue("$date", measuredDate ?? string.Empty); insert.Parameters.AddWithValue("$now", now); insert.ExecuteNonQuery();
    }

    public IReadOnlyList<(string MaterialId, string Orientation, int SampleNumber, string RawValue)> GetTensileSamples()
    {
        var samples = new List<(string MaterialId, string Orientation, int SampleNumber, string RawValue)>();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT MaterialId, Orientation, SampleNumber, RawValue
FROM NativeTensileSamples
ORDER BY MaterialId, Orientation, SampleNumber;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            samples.Add((
                reader["MaterialId"]?.ToString() ?? string.Empty,
                reader["Orientation"]?.ToString() ?? string.Empty,
                Convert.ToInt32(reader["SampleNumber"], CultureInfo.InvariantCulture),
                reader["RawValue"]?.ToString() ?? string.Empty));
        }

        return samples;
    }


    public IReadOnlyList<(string MaterialId, string Orientation, int SampleNumber, string RawValue)> GetImpactSamples()
    {
        var samples = new List<(string MaterialId, string Orientation, int SampleNumber, string RawValue)>();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT MaterialId, Orientation, SampleNumber, RawValue
FROM NativeImpactSamples
ORDER BY MaterialId, Orientation, SampleNumber;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            samples.Add((
                reader["MaterialId"]?.ToString() ?? string.Empty,
                reader["Orientation"]?.ToString() ?? string.Empty,
                Convert.ToInt32(reader["SampleNumber"], CultureInfo.InvariantCulture),
                reader["RawValue"]?.ToString() ?? string.Empty));
        }

        return samples;
    }

    public IReadOnlyList<(string MaterialId, string Revolutions, string Degrees, string TestNotes)> GetStiffnessMeasurements()
    {
        var rows = new List<(string MaterialId, string Revolutions, string Degrees, string TestNotes)>();
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();

        using var command = connection.CreateCommand();
        command.CommandText = @"
SELECT MaterialId, Revolutions, Degrees, TestNotes
FROM NativeStiffnessMeasurements
ORDER BY MaterialId;";

        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add((
                reader["MaterialId"]?.ToString() ?? string.Empty,
                reader["Revolutions"]?.ToString() ?? string.Empty,
                reader["Degrees"]?.ToString() ?? string.Empty,
                reader["TestNotes"]?.ToString() ?? string.Empty));
        }

        return rows;
    }

    public string GetNativeMeasurementNote(string materialId, string testType)
    {
        using var connection = new SqliteConnection(ConnectionString); connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT TestNotes FROM NativeMeasurementNotes WHERE MaterialId=$id AND TestType=$type LIMIT 1;";
        command.Parameters.AddWithValue("$id", materialId); command.Parameters.AddWithValue("$type", testType);
        return command.ExecuteScalar()?.ToString() ?? string.Empty;
    }

    private static DateTime? ParseIsoDate(string value) =>
        DateTime.TryParseExact(value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsed)
            ? parsed.Date
            : null;

    private static string ToIsoDate(DateTime? value) =>
        value?.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture) ?? string.Empty;

    public string GetNativeMeasurementDate(string materialId, string testType)
    {
        using var connection = new SqliteConnection(ConnectionString); connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT MeasuredDate FROM NativeMeasurementNotes WHERE MaterialId=$id AND TestType=$type LIMIT 1;";
        command.Parameters.AddWithValue("$id", materialId);
        command.Parameters.AddWithValue("$type", testType);
        return command.ExecuteScalar()?.ToString() ?? string.Empty;
    }


    public List<ExperimentDefinitionRecord> LoadExperimentDefinitions(bool activeOnly = false)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();
        using var command = connection.CreateCommand();
        command.CommandText = activeOnly
            ? "SELECT * FROM ExperimentDefinitions WHERE IsActive = 1 ORDER BY SortOrder, Name;"
            : "SELECT * FROM ExperimentDefinitions ORDER BY SortOrder, Name;";
        var rows = new List<ExperimentDefinitionRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new ExperimentDefinitionRecord
        {
            ExperimentDefinitionId = ReadString(reader, "ExperimentDefinitionId"),
            Name = ReadString(reader, "Name"),
            ParameterKey = ReadString(reader, "ParameterKey"),
            DefaultUnit = ReadString(reader, "DefaultUnit"),
            Description = ReadString(reader, "Description"),
            IsActive = ReadString(reader, "IsActive") != "0",
            SortOrder = int.TryParse(ReadString(reader, "SortOrder"), out var sortOrder) ? sortOrder : 0
        });
        return rows;
    }

    public List<MaterialExperimentRecord> LoadMaterialExperiments()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM MaterialExperiments ORDER BY UpdatedAtUtc DESC, MaterialExperimentId;";
        var rows = new List<MaterialExperimentRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new MaterialExperimentRecord
        {
            MaterialExperimentId = ReadString(reader, "MaterialExperimentId"),
            MaterialID = ReadString(reader, "MaterialId"),
            ExperimentDefinitionId = ReadString(reader, "ExperimentDefinitionId"),
            ParameterValue = ReadString(reader, "ParameterValue"),
            ParameterUnit = ReadString(reader, "ParameterUnit"),
            BaselineMaterialID = ReadString(reader, "BaselineMaterialId"),
            Notes = ReadString(reader, "Notes"),
            PublishOnWebsite = ReadString(reader, "PublishOnWebsite") == "1",
            IsActive = ReadString(reader, "IsActive") != "0",
            CreatedAtUtc = ReadString(reader, "CreatedAtUtc"),
            UpdatedAtUtc = ReadString(reader, "UpdatedAtUtc")
        });
        return rows;
    }

    public void ReplaceMaterialExperiments(IEnumerable<MaterialExperimentRecord> experiments)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using (var clear = connection.CreateCommand())
        {
            clear.Transaction = transaction;
            clear.CommandText = "DELETE FROM MaterialExperiments;";
            clear.ExecuteNonQuery();
        }
        using var insert = connection.CreateCommand();
        insert.Transaction = transaction;
        insert.CommandText = @"INSERT INTO MaterialExperiments
(MaterialExperimentId, MaterialId, ExperimentDefinitionId, ParameterValue, ParameterUnit, BaselineMaterialId, Notes, PublishOnWebsite, IsActive, CreatedAtUtc, UpdatedAtUtc)
VALUES ($id,$material,$definition,$value,$unit,$baseline,$notes,$publish,$active,$created,$updated);";
        foreach (var x in experiments)
        {
            var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(x.CreatedAtUtc)) x.CreatedAtUtc = now;
            x.UpdatedAtUtc = now;
            insert.Parameters.Clear();
            insert.Parameters.AddWithValue("$id", x.MaterialExperimentId);
            insert.Parameters.AddWithValue("$material", x.MaterialID);
            insert.Parameters.AddWithValue("$definition", x.ExperimentDefinitionId);
            insert.Parameters.AddWithValue("$value", x.ParameterValue ?? string.Empty);
            insert.Parameters.AddWithValue("$unit", x.ParameterUnit ?? string.Empty);
            insert.Parameters.AddWithValue("$baseline", string.IsNullOrWhiteSpace(x.BaselineMaterialID) ? DBNull.Value : x.BaselineMaterialID);
            insert.Parameters.AddWithValue("$notes", x.Notes ?? string.Empty);
            insert.Parameters.AddWithValue("$publish", x.PublishOnWebsite ? 1 : 0);
            insert.Parameters.AddWithValue("$active", x.IsActive ? 1 : 0);
            insert.Parameters.AddWithValue("$created", x.CreatedAtUtc);
            insert.Parameters.AddWithValue("$updated", x.UpdatedAtUtc);
            insert.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    public List<ExperimentalRunRecord> LoadExperimentalRuns()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM ExperimentalRuns ORDER BY MaterialExperimentId, CreatedAtUtc, ExperimentalRunId;";
        var rows = new List<ExperimentalRunRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) rows.Add(new ExperimentalRunRecord
        {
            ExperimentalRunId = ReadString(reader, "ExperimentalRunId"),
            MaterialExperimentId = ReadString(reader, "MaterialExperimentId"),
            ParameterValue = ReadString(reader, "ParameterValue"),
            ParameterUnit = ReadString(reader, "ParameterUnit"),
            Status = ReadString(reader, "Status"),
            IsBaseline = ReadString(reader, "IsBaseline") != "0",
            IsActive = ReadString(reader, "IsActive") != "0",
            Notes = ReadString(reader, "Notes"),
            MeasuredDate = ParseIsoDate(ReadString(reader, "MeasuredDate")),
            CreatedAtUtc = ReadString(reader, "CreatedAtUtc"),
            UpdatedAtUtc = ReadString(reader, "UpdatedAtUtc")
        });
        return rows;
    }

    public void ReplaceExperimentalRuns(IEnumerable<ExperimentalRunRecord> runs)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using (var clear = connection.CreateCommand())
        {
            clear.Transaction = transaction;
            clear.CommandText = "DELETE FROM ExperimentalRuns;";
            clear.ExecuteNonQuery();
        }
        using var insert = connection.CreateCommand();
        insert.Transaction = transaction;
        insert.CommandText = @"INSERT INTO ExperimentalRuns
(ExperimentalRunId, MaterialExperimentId, ParameterValue, ParameterUnit, Status, IsBaseline, IsActive, Notes, MeasuredDate, CreatedAtUtc, UpdatedAtUtc)
VALUES ($id,$series,$value,$unit,$status,$baseline,$active,$notes,$measured,$created,$updated);";
        foreach (var x in runs)
        {
            var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
            if (string.IsNullOrWhiteSpace(x.CreatedAtUtc)) x.CreatedAtUtc = now;
            x.UpdatedAtUtc = now;
            insert.Parameters.Clear();
            insert.Parameters.AddWithValue("$id", x.ExperimentalRunId);
            insert.Parameters.AddWithValue("$series", x.MaterialExperimentId);
            insert.Parameters.AddWithValue("$value", x.ParameterValue ?? string.Empty);
            insert.Parameters.AddWithValue("$unit", x.ParameterUnit ?? string.Empty);
            insert.Parameters.AddWithValue("$status", string.IsNullOrWhiteSpace(x.Status) ? "Planned" : x.Status);
            insert.Parameters.AddWithValue("$baseline", x.IsBaseline ? 1 : 0);
            insert.Parameters.AddWithValue("$active", x.IsActive ? 1 : 0);
            insert.Parameters.AddWithValue("$notes", x.Notes ?? string.Empty);
            insert.Parameters.AddWithValue("$measured", ToIsoDate(x.MeasuredDate));
            insert.Parameters.AddWithValue("$created", x.CreatedAtUtc);
            insert.Parameters.AddWithValue("$updated", x.UpdatedAtUtc);
            insert.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    public List<ExperimentalMeasurementRecord> LoadExperimentalMeasurements()
    {
        using var connection = new SqliteConnection(ConnectionString); connection.Open(); Initialize();
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT * FROM ExperimentalMeasurements ORDER BY ExperimentalRunId, MeasurementType;";
        var rows = new List<ExperimentalMeasurementRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            var storedType = ReadString(reader,"MeasurementType");
            var separator = storedType.IndexOf('|');
            var parsedType = separator >= 0 ? storedType[..separator] : storedType;
            var parsedOrientation = separator >= 0 ? storedType[(separator + 1)..] : ReadString(reader,"Orientation");
            rows.Add(new ExperimentalMeasurementRecord
            {
                ExperimentalMeasurementId=ReadString(reader,"ExperimentalMeasurementId"), ExperimentalRunId=ReadString(reader,"ExperimentalRunId"),
                MeasurementType=parsedType, Orientation=parsedOrientation, RawUnit=ReadString(reader,"RawUnit"), ResultUnit=ReadString(reader,"ResultUnit"), Sample1=ReadString(reader,"Sample1"),
                Sample2=ReadString(reader,"Sample2"), Sample3=ReadString(reader,"Sample3"), Sample4=ReadString(reader,"Sample4"), Sample5=ReadString(reader,"Sample5"), Sample6=ReadString(reader,"Sample6"), Sample7=ReadString(reader,"Sample7"), Sample8=ReadString(reader,"Sample8"), Sample9=ReadString(reader,"Sample9"), Sample10=ReadString(reader,"Sample10"),
                ResultAverage=ReadString(reader,"ResultAverage"), ResultStdDev=ReadString(reader,"ResultStdDev"), ResultCv=ReadString(reader,"ResultCv"), ResultCount=ReadString(reader,"ResultCount"), ResultConfidence=ReadString(reader,"ResultConfidence"), Notes=ReadString(reader,"Notes"), UpdatedAtUtc=ReadString(reader,"UpdatedAtUtc")
            });
        }
        return rows;
    }

    public void ReplaceExperimentalMeasurements(IEnumerable<ExperimentalMeasurementRecord> measurements)
    {
        using var connection = new SqliteConnection(ConnectionString); connection.Open(); using var transaction=connection.BeginTransaction();
        using (var clear=connection.CreateCommand()) { clear.Transaction=transaction; clear.CommandText="DELETE FROM ExperimentalMeasurements;"; clear.ExecuteNonQuery(); }
        using var insert=connection.CreateCommand(); insert.Transaction=transaction;
        insert.CommandText=@"INSERT INTO ExperimentalMeasurements (ExperimentalMeasurementId,ExperimentalRunId,MeasurementType,Unit,Orientation,RawUnit,ResultUnit,Sample1,Sample2,Sample3,Sample4,Sample5,Sample6,Sample7,Sample8,Sample9,Sample10,ResultAverage,ResultStdDev,ResultCv,ResultCount,ResultConfidence,Notes,UpdatedAtUtc) VALUES ($id,$run,$type,$unit,$orientation,$rawUnit,$resultUnit,$s1,$s2,$s3,$s4,$s5,$s6,$s7,$s8,$s9,$s10,$average,$stddev,$cv,$count,$confidence,$notes,$updated);";
        foreach (var x in measurements)
        {
            x.UpdatedAtUtc=DateTime.UtcNow.ToString("O",CultureInfo.InvariantCulture); insert.Parameters.Clear();
            insert.Parameters.AddWithValue("$id",x.ExperimentalMeasurementId); insert.Parameters.AddWithValue("$run",x.ExperimentalRunId); insert.Parameters.AddWithValue("$type", string.IsNullOrWhiteSpace(x.Orientation) ? x.MeasurementType : $"{x.MeasurementType}|{x.Orientation}"); insert.Parameters.AddWithValue("$unit",x.ResultUnit??string.Empty); insert.Parameters.AddWithValue("$orientation",x.Orientation??string.Empty); insert.Parameters.AddWithValue("$rawUnit",x.RawUnit??string.Empty); insert.Parameters.AddWithValue("$resultUnit",x.ResultUnit??string.Empty);
            insert.Parameters.AddWithValue("$s1",x.Sample1??string.Empty); insert.Parameters.AddWithValue("$s2",x.Sample2??string.Empty); insert.Parameters.AddWithValue("$s3",x.Sample3??string.Empty); insert.Parameters.AddWithValue("$s4",x.Sample4??string.Empty); insert.Parameters.AddWithValue("$s5",x.Sample5??string.Empty); insert.Parameters.AddWithValue("$s6",x.Sample6??string.Empty); insert.Parameters.AddWithValue("$s7",x.Sample7??string.Empty); insert.Parameters.AddWithValue("$s8",x.Sample8??string.Empty); insert.Parameters.AddWithValue("$s9",x.Sample9??string.Empty); insert.Parameters.AddWithValue("$s10",x.Sample10??string.Empty); insert.Parameters.AddWithValue("$average",x.ResultAverage??string.Empty); insert.Parameters.AddWithValue("$stddev",x.ResultStdDev??string.Empty); insert.Parameters.AddWithValue("$cv",x.ResultCv??string.Empty); insert.Parameters.AddWithValue("$count",x.ResultCount??string.Empty); insert.Parameters.AddWithValue("$confidence",x.ResultConfidence??string.Empty);
            insert.Parameters.AddWithValue("$notes",x.Notes??string.Empty); insert.Parameters.AddWithValue("$updated",x.UpdatedAtUtc); insert.ExecuteNonQuery();
        }
        transaction.Commit();
    }

    public (int Measurements, int OrphanedRuns, int RunsWithMeasurements) GetExperimentalMeasurementStats()
    {
        using var connection=new SqliteConnection(ConnectionString); connection.Open();
        int Scalar(string sql) { using var c=connection.CreateCommand(); c.CommandText=sql; return Convert.ToInt32(c.ExecuteScalar()??0); }
        return (Scalar("SELECT COUNT(*) FROM ExperimentalMeasurements;"), Scalar("SELECT COUNT(*) FROM ExperimentalMeasurements m LEFT JOIN ExperimentalRuns r ON r.ExperimentalRunId=m.ExperimentalRunId WHERE r.ExperimentalRunId IS NULL;"), Scalar("SELECT COUNT(DISTINCT ExperimentalRunId) FROM ExperimentalMeasurements;"));
    }

    public (int Runs, int OrphanedSeries, int BaselineRuns, int SeriesWithMultipleRuns) GetExperimentalRunStats()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        int Scalar(string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return Convert.ToInt32(command.ExecuteScalar() ?? 0);
        }
        return (
            Scalar("SELECT COUNT(*) FROM ExperimentalRuns;"),
            Scalar("SELECT COUNT(*) FROM ExperimentalRuns r LEFT JOIN MaterialExperiments s ON s.MaterialExperimentId=r.MaterialExperimentId WHERE s.MaterialExperimentId IS NULL;"),
            Scalar("SELECT COUNT(*) FROM ExperimentalRuns WHERE IsBaseline=1;"),
            Scalar("SELECT COUNT(*) FROM (SELECT MaterialExperimentId FROM ExperimentalRuns GROUP BY MaterialExperimentId HAVING COUNT(*) > 1);")
        );
    }

    public (int Definitions, int ActiveDefinitions, int MaterialExperiments, int OrphanedMaterials, int InvalidDefinitions) GetExperimentalFrameworkStats()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        int Scalar(string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return Convert.ToInt32(command.ExecuteScalar() ?? 0);
        }

        return (
            Scalar("SELECT COUNT(*) FROM ExperimentDefinitions;"),
            Scalar("SELECT COUNT(*) FROM ExperimentDefinitions WHERE IsActive = 1;"),
            Scalar("SELECT COUNT(*) FROM MaterialExperiments;"),
            Scalar("SELECT COUNT(*) FROM MaterialExperiments me LEFT JOIN NativeMaterialManagerRows m ON m.MaterialId = me.MaterialId WHERE m.MaterialId IS NULL;"),
            Scalar("SELECT COUNT(*) FROM MaterialExperiments me LEFT JOIN ExperimentDefinitions d ON d.ExperimentDefinitionId = me.ExperimentDefinitionId WHERE d.ExperimentDefinitionId IS NULL;")
        );
    }

    private static string GetValue(DataRow row, string columnName)
    {
        return row.Table.Columns.Contains(columnName) ? row[columnName]?.ToString()?.Trim() ?? string.Empty : string.Empty;
    }

    private static void AddParameter(SqliteCommand command, string name, string? value)
    {
        command.Parameters.AddWithValue(name, string.IsNullOrWhiteSpace(value) ? DBNull.Value : value);
    }

    private static string Quote(string identifier) => "\"" + identifier.Replace("\"", "\"\"") + "\"";
}
