using FilamentDbApp.Models;
using FilamentDbApp.Services;
using Microsoft.Data.Sqlite;
using System.IO;
using System.Security.Cryptography;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    public string? LoadDocumentBrandDisplayName()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT BrandDisplayName
            FROM DocumentBrandIdentitySettings
            WHERE SettingsId = 1;
            """;
        return command.ExecuteScalar()?.ToString();
    }

    public void SaveDocumentBrandDisplayName(string value)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            INSERT INTO DocumentBrandIdentitySettings
                (SettingsId, BrandDisplayName, UpdatedAtUtc)
            VALUES (1, $name, $updated)
            ON CONFLICT(SettingsId) DO UPDATE SET
                BrandDisplayName=excluded.BrandDisplayName,
                UpdatedAtUtc=excluded.UpdatedAtUtc;
            """;
        command.Parameters.AddWithValue("$name", value);
        command.Parameters.AddWithValue(
            "$updated",
            DateTime.UtcNow.ToString("O", System.Globalization.CultureInfo.InvariantCulture));
        command.ExecuteNonQuery();
    }

    public void ClearDocumentBrandDisplayName()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText =
            "DELETE FROM DocumentBrandIdentitySettings WHERE SettingsId = 1;";
        command.ExecuteNonQuery();
    }

    public DocumentBrandingRecord? LoadDocumentBranding()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
            SELECT CustomLogoEnabled, NormalizedPng, Sha256, PixelWidth, PixelHeight,
                   ByteLength, HasTransparency, UpdatedAtUtc
            FROM DocumentBrandingSettings
            WHERE SettingsId = 1;
            """;
        using var reader = command.ExecuteReader();
        if (!reader.Read()) return null;
        return new DocumentBrandingRecord(
            reader.GetInt32(0) != 0,
            reader.IsDBNull(1) ? Array.Empty<byte>() : (byte[])reader[1],
            reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
            reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
            reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
            reader.IsDBNull(5) ? 0 : reader.GetInt64(5),
            !reader.IsDBNull(6) && reader.GetInt32(6) != 0,
            reader.IsDBNull(7) ? string.Empty : reader.GetString(7));
    }

    public void SaveDocumentBranding(DocumentBrandingRecord record)
    {
        ArgumentNullException.ThrowIfNull(record);
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
            INSERT INTO DocumentBrandingSettings
                (SettingsId, CustomLogoEnabled, NormalizedPng, Sha256, PixelWidth,
                 PixelHeight, ByteLength, HasTransparency, UpdatedAtUtc)
            VALUES
                (1, $enabled, $png, $sha256, $width, $height, $bytes, $alpha, $updated)
            ON CONFLICT(SettingsId) DO UPDATE SET
                CustomLogoEnabled=excluded.CustomLogoEnabled,
                NormalizedPng=excluded.NormalizedPng,
                Sha256=excluded.Sha256,
                PixelWidth=excluded.PixelWidth,
                PixelHeight=excluded.PixelHeight,
                ByteLength=excluded.ByteLength,
                HasTransparency=excluded.HasTransparency,
                UpdatedAtUtc=excluded.UpdatedAtUtc;
            """;
        command.Parameters.AddWithValue("$enabled", record.CustomLogoEnabled ? 1 : 0);
        command.Parameters.Add("$png", SqliteType.Blob).Value = record.NormalizedPng;
        command.Parameters.AddWithValue("$sha256", record.Sha256);
        command.Parameters.AddWithValue("$width", record.PixelWidth);
        command.Parameters.AddWithValue("$height", record.PixelHeight);
        command.Parameters.AddWithValue("$bytes", record.ByteLength);
        command.Parameters.AddWithValue("$alpha", record.HasTransparency ? 1 : 0);
        command.Parameters.AddWithValue("$updated", record.UpdatedAtUtc);
        command.ExecuteNonQuery();
        transaction.Commit();
    }

    public void ClearDocumentBranding()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM DocumentBrandingSettings WHERE SettingsId = 1;";
        command.ExecuteNonQuery();
    }

    public DocumentBrandingFoundationContractVerification RunDocumentBrandingFoundationContractVerification()
    {
        var root = IOPath.Combine(
            IOPath.GetTempPath(),
            "3DPIceland-DocumentBranding-" + Guid.NewGuid().ToString("N"));
        IODirectory.CreateDirectory(root);
        try
        {
            var databasePath = IOPath.Combine(root, "filamentdb.sqlite");
            var sourcePath = IOPath.Combine(root, "source-logo.png");
            var database = new LocalDatabase(databasePath);
            var pixels = new byte[16 * 16 * 4];
            for (var index = 0; index < pixels.Length; index += 4)
            {
                pixels[index] = 0x40;
                pixels[index + 1] = 0x80;
                pixels[index + 2] = 0xc0;
                pixels[index + 3] = (byte)(index == 0 ? 128 : 255);
            }
            var bitmap = BitmapSource.Create(
                16, 16, 96, 96, PixelFormats.Bgra32, null, pixels, 16 * 4);
            var encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));
            using (var output = IOFile.Create(sourcePath)) encoder.Save(output);
            var sourceBefore = Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(sourcePath)));

            var service = new DocumentBrandingService(database);
            var saved = service.ImportCustomLogo(sourcePath);
            var resolved = new DocumentBrandingService(new LocalDatabase(databasePath)).ResolveCustomOrFallback();
            var identityService = new DocumentBrandIdentityService(database);
            var savedBrand = identityService.Save("  Test   Brand  ");
            var restartedBrand = new DocumentBrandIdentityService(
                new LocalDatabase(databasePath)).Resolve();
            var backup = database.CreateManualBackupNow();
            var backupDatabase = new LocalDatabase(backup.FullName);
            var backupLogo = new DocumentBrandingService(backupDatabase).ResolveCustomOrFallback();
            var backupBrand = new DocumentBrandIdentityService(backupDatabase).Resolve();
            var sourceAfter = Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(sourcePath)));
            var cacheHash = IOFile.Exists(service.CachePath)
                ? Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(service.CachePath)))
                : string.Empty;

            var wrongSignatureRejected = false;
            try
            {
                _ = DocumentBrandingService.NormalizePng([1, 2, 3, 4, 5, 6, 7, 8]);
            }
            catch (InvalidDataException)
            {
                wrongSignatureRejected = true;
            }

            var oversizedDimensionsRejected = false;
            try
            {
                var oversizedPixels = new byte[4097 * 16 * 4];
                var oversizedBitmap = BitmapSource.Create(
                    4097, 16, 96, 96, PixelFormats.Bgra32, null, oversizedPixels, 4097 * 4);
                var oversizedEncoder = new PngBitmapEncoder();
                oversizedEncoder.Frames.Add(BitmapFrame.Create(oversizedBitmap));
                using var oversizedOutput = new MemoryStream();
                oversizedEncoder.Save(oversizedOutput);
                _ = DocumentBrandingService.NormalizePng(oversizedOutput.ToArray());
            }
            catch (InvalidDataException)
            {
                oversizedDimensionsRejected = true;
            }

            var invalidBrandsRejected = false;
            try
            {
                identityService.Save(new string('x', DocumentBrandIdentityService.MaximumLength + 1));
            }
            catch (ArgumentException)
            {
                try
                {
                    identityService.Save("Control\u0001Character");
                }
                catch (ArgumentException)
                {
                    invalidBrandsRejected = true;
                }
            }

            using (var connection = new SqliteConnection($"Data Source={databasePath};Pooling=False"))
            {
                connection.Open();
                using var corrupt = connection.CreateCommand();
                corrupt.CommandText = """
                    UPDATE DocumentBrandingSettings
                    SET NormalizedPng = X'89504E470D0A1A0A', ByteLength = 8
                    WHERE SettingsId = 1;
                    """;
                corrupt.ExecuteNonQuery();
            }
            var fallback = new DocumentBrandingService(new LocalDatabase(databasePath)).ResolveCustomOrFallback();
            service.RestoreDefault();
            var defaultSnapshot = new DocumentBrandingService(new LocalDatabase(databasePath)).ResolveCustomOrFallback();
            identityService.RestoreDefault();
            var defaultBrand = new DocumentBrandIdentityService(
                new LocalDatabase(databasePath)).Resolve();

            var passed =
                saved.PixelWidth == 16 &&
                saved.PixelHeight == 16 &&
                saved.HasTransparency &&
                saved.NormalizedPng.Length > 8 &&
                saved.Sha256 == cacheHash &&
                sourceBefore == sourceAfter &&
                resolved.Provenance == DocumentBrandingProvenance.Custom &&
                resolved.Sha256 == saved.Sha256 &&
                backupLogo.Provenance == DocumentBrandingProvenance.Custom &&
                backupLogo.Sha256 == saved.Sha256 &&
                backupBrand == "Test Brand" &&
                wrongSignatureRejected &&
                oversizedDimensionsRejected &&
                invalidBrandsRejected &&
                fallback.Provenance == DocumentBrandingProvenance.Fallback &&
                defaultSnapshot.Provenance == DocumentBrandingProvenance.Default &&
                savedBrand == "Test Brand" &&
                restartedBrand == "Test Brand" &&
                defaultBrand == DocumentBrandIdentityService.DefaultBrandDisplayName &&
                database.CurrentSchemaVersion == BuildInfo.CurrentDatabaseSchema;
            return new DocumentBrandingFoundationContractVerification(
                passed,
                passed
                    ? $"Schema v{BuildInfo.CurrentDatabaseSchema}; PNG limits/source/cache, brand validation, backup/restart and fallback/default pass."
                    : "Document branding limits, persistence, backup, restart or fallback/default contract failed.");
        }
        catch (Exception ex)
        {
            return new DocumentBrandingFoundationContractVerification(false, ex.Message);
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            try { IODirectory.Delete(root, true); } catch { }
        }
    }
}

public sealed record DocumentBrandingFoundationContractVerification(
    bool Passed,
    string Detail);
