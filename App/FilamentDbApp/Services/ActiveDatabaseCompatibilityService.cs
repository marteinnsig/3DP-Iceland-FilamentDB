using Microsoft.Data.Sqlite;
using System.Globalization;
using System.Security.Cryptography;

namespace FilamentDbApp.Services;

public sealed class ActiveDatabaseCompatibilityService
{
    public ActiveDatabaseCompatibilityInspection Inspect(string databasePath, int supportedSchemaVersion)
    {
        if (string.IsNullOrWhiteSpace(databasePath) || !IOFile.Exists(databasePath))
            return new ActiveDatabaseCompatibilityInspection(true, 0, "No existing active SQLite database requires compatibility inspection.");

        try
        {
            using var connection = new SqliteConnection($"Data Source={IOPath.GetFullPath(databasePath)};Mode=ReadOnly;Pooling=False");
            connection.Open();

            var schemaText = ReadSchemaVersion(connection);
            if (!int.TryParse(schemaText, NumberStyles.Integer, CultureInfo.InvariantCulture, out var schemaVersion) || schemaVersion <= 0)
                return new ActiveDatabaseCompatibilityInspection(false, 0, "The active SQLite database has no valid AppMeta schema version.");
            if (schemaVersion > supportedSchemaVersion)
                return new ActiveDatabaseCompatibilityInspection(false, schemaVersion, $"The active SQLite database schema v{schemaVersion} is newer than supported schema v{supportedSchemaVersion}.");

            var materialsHasPrimaryKey = TableHasPrimaryKey(connection, "Materials", "MaterialId");
            var importsHasExpectedColumn = TableHasColumn(connection, "Imports", "SchemaVersion");
            if (!materialsHasPrimaryKey || !importsHasExpectedColumn)
                return new ActiveDatabaseCompatibilityInspection(false, schemaVersion, "The active SQLite database does not have the required legacy compatibility table shape.");

            return new ActiveDatabaseCompatibilityInspection(true, schemaVersion, $"Schema v{schemaVersion} has the required startup compatibility shape.");
        }
        catch (Exception ex)
        {
            return new ActiveDatabaseCompatibilityInspection(false, 0, "The active SQLite database could not be inspected read-only: " + ex.Message);
        }
    }

    public void EnsureSupportedOrPreserve(string databasePath, int supportedSchemaVersion)
    {
        var inspection = Inspect(databasePath, supportedSchemaVersion);
        if (inspection.IsSupported) return;

        var evidencePath = PreserveEvidenceCopy(databasePath);
        throw new InvalidOperationException(
            inspection.Detail
            + "\n\nStartup stopped before changing the active database. An exact evidence copy was retained at:\n"
            + evidencePath
            + "\n\nUse Backup and Recovery Center or an explicitly supported migration path before replacing the active database.");
    }

    public ActiveDatabaseCompatibilityContractVerification RunContractVerification(int supportedSchemaVersion)
    {
        var root = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-ActiveDatabaseCompatibility-" + Guid.NewGuid().ToString("N"));
        IODirectory.CreateDirectory(root);
        try
        {
            var supportedPath = IOPath.Combine(root, "supported.sqlite");
            CreateFixture(supportedPath, supportedSchemaVersion, includeRequiredLegacyShape: true);
            var supported = Inspect(supportedPath, supportedSchemaVersion);

            var canonicalOnlyPath = IOPath.Combine(root, "canonical-only.sqlite");
            CreateFixture(canonicalOnlyPath, supportedSchemaVersion, includeRequiredLegacyShape: false);
            var canonicalOnlyHash = ComputeSha256(canonicalOnlyPath);
            var canonicalOnlyBlocked = BlocksAndPreserves(canonicalOnlyPath, supportedSchemaVersion, canonicalOnlyHash);

            var newerPath = IOPath.Combine(root, "newer.sqlite");
            CreateFixture(newerPath, supportedSchemaVersion + 1, includeRequiredLegacyShape: true);
            var newerHash = ComputeSha256(newerPath);
            var newerBlocked = BlocksAndPreserves(newerPath, supportedSchemaVersion, newerHash);

            var unreadablePath = IOPath.Combine(root, "unreadable.sqlite");
            IOFile.WriteAllBytes(unreadablePath, "not a SQLite database"u8.ToArray());
            var unreadableHash = ComputeSha256(unreadablePath);
            var unreadableBlocked = BlocksAndPreserves(unreadablePath, supportedSchemaVersion, unreadableHash);

            var passed = supported.IsSupported && supported.SchemaVersion == supportedSchemaVersion &&
                         canonicalOnlyBlocked && newerBlocked && unreadableBlocked;
            return new ActiveDatabaseCompatibilityContractVerification(
                passed,
                passed
                    ? "Supported schema remains available; canonical-only, newer and unreadable active databases are blocked unchanged with retained evidence copies."
                    : "Active-database compatibility preservation contract failed.");
        }
        catch (Exception ex)
        {
            return new ActiveDatabaseCompatibilityContractVerification(false, ex.Message);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            try { if (IODirectory.Exists(root)) IODirectory.Delete(root, recursive: true); } catch { }
        }
    }

    private bool BlocksAndPreserves(string databasePath, int supportedSchemaVersion, string originalHash)
    {
        try
        {
            EnsureSupportedOrPreserve(databasePath, supportedSchemaVersion);
            return false;
        }
        catch (InvalidOperationException ex)
        {
            var evidencePath = ex.Message
                .Split(["\r\n", "\n"], StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault(line => line.EndsWith(".sqlite", StringComparison.OrdinalIgnoreCase));
            return IOFile.Exists(databasePath) &&
                   ComputeSha256(databasePath) == originalHash &&
                   !string.IsNullOrWhiteSpace(evidencePath) &&
                   IOFile.Exists(evidencePath) &&
                   ComputeSha256(evidencePath) == originalHash;
        }
    }

    private static void CreateFixture(string path, int schemaVersion, bool includeRequiredLegacyShape)
    {
        using var connection = new SqliteConnection($"Data Source={path};Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = $"""
            CREATE TABLE AppMeta (Key TEXT PRIMARY KEY, Value TEXT NOT NULL);
            INSERT INTO AppMeta(Key, Value) VALUES ('SchemaVersion', '{schemaVersion.ToString(CultureInfo.InvariantCulture)}');
            CREATE TABLE NativeMaterialManagerRows (MaterialId TEXT PRIMARY KEY);
            {(includeRequiredLegacyShape ? "CREATE TABLE Materials (MaterialId TEXT PRIMARY KEY); CREATE TABLE Imports (ImportId INTEGER PRIMARY KEY, SchemaVersion INTEGER NOT NULL);" : string.Empty)}
            """;
        command.ExecuteNonQuery();
    }

    private static string PreserveEvidenceCopy(string databasePath)
    {
        var fullPath = IOPath.GetFullPath(databasePath);
        var info = new System.IO.FileInfo(fullPath);
        var stamp = info.LastWriteTimeUtc.ToString("yyyyMMdd_HHmmss_fff", CultureInfo.InvariantCulture);
        var folder = IOPath.GetDirectoryName(fullPath) ?? throw new InvalidOperationException("The active SQLite database folder is unavailable.");
        var evidencePath = IOPath.Combine(folder, $"filamentdb_startup_blocked_{stamp}_{info.Length}.sqlite");

        if (IOFile.Exists(evidencePath))
        {
            if (ComputeSha256(evidencePath) == ComputeSha256(fullPath)) return evidencePath;
            evidencePath = IOPath.Combine(folder, $"filamentdb_startup_blocked_{stamp}_{info.Length}_{Guid.NewGuid():N}.sqlite");
        }

        IOFile.Copy(fullPath, evidencePath, overwrite: false);
        if (ComputeSha256(evidencePath) != ComputeSha256(fullPath))
        {
            try { IOFile.Delete(evidencePath); } catch { }
            throw new System.IO.IOException("The preserved SQLite evidence copy failed SHA-256 verification.");
        }
        return evidencePath;
    }

    private static string ComputeSha256(string path)
    {
        using var stream = IOFile.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static string? ReadSchemaVersion(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Value FROM AppMeta WHERE Key='SchemaVersion' LIMIT 1;";
        return command.ExecuteScalar()?.ToString();
    }

    private static bool TableHasColumn(SqliteConnection connection, string tableName, string columnName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info(\"{tableName}\");";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            if (string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }

    private static bool TableHasPrimaryKey(SqliteConnection connection, string tableName, string columnName)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info(\"{tableName}\");";
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            _ = int.TryParse(reader["pk"]?.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var primaryKeyOrder);
            if (primaryKeyOrder > 0 && string.Equals(reader["name"]?.ToString(), columnName, StringComparison.OrdinalIgnoreCase)) return true;
        }
        return false;
    }
}

public sealed record ActiveDatabaseCompatibilityInspection(bool IsSupported, int SchemaVersion, string Detail);

public sealed record ActiveDatabaseCompatibilityContractVerification(bool Passed, string Detail);
