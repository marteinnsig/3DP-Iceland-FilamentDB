using FilamentDbApp.Models;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FilamentDbApp.Services;

public sealed class ApplicationUpdatePackageService
{
    public const string ManifestFileName = "3dp-update-manifest.json";
    public const string ProductionTrustedPublicKeyPem = """
-----BEGIN PUBLIC KEY-----
MFkwEwYHKoZIzj0CAQYIKoZIzj0DAQcDQgAEwFHCyaBjc47nq40WuCmmp+2p43nm
+LDj+oN74hkS/VfunrYSzzZZE1+k70VPJOAuzX7SvCrn+bzx3kG4UQ0sFg==
-----END PUBLIC KEY-----
""";
    public const string ProductionTrustedPublicKeyFingerprint = "87D407FEA230D484D8F436A4BA4958BF7F70336B968FFEC7F3966C15DFDFF1EA";
    private const int MaximumPackageFiles = 512;
    private const long MaximumManifestBytes = 1_048_576;
    private const long MaximumUncompressedBytes = 2_147_483_648;
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };
    private readonly string _trustedPublicKeyPem;

    public ApplicationUpdatePackageService(string? trustedPublicKeyPem = null) =>
        _trustedPublicKeyPem = trustedPublicKeyPem?.Trim() ?? ProductionTrustedPublicKeyPem;

    public ApplicationUpdateReadinessResult Inspect(
        string packagePath,
        Version currentVersion,
        int currentDatabaseSchema)
    {
        ApplicationUpdateManifest? manifest = null;
        try
        {
            if (string.IsNullOrWhiteSpace(packagePath) || !File.Exists(packagePath))
                return Blocked("Package unavailable", "The selected update package does not exist.", manifest);

            using var archive = ZipFile.OpenRead(packagePath);
            if (archive.Entries.Count > MaximumPackageFiles)
                return Blocked("Package limits exceeded", $"The package contains more than {MaximumPackageFiles:N0} ZIP entries.", manifest, packageReadable: true);
            long totalUncompressedBytes = 0;
            foreach (var entry in archive.Entries)
            {
                if (!IsCanonicalArchivePath(entry.FullName))
                    return Blocked("Unsafe ZIP path", "Unsafe or non-canonical ZIP entry: " + entry.FullName, manifest, packageReadable: true);
                if (entry.Length < 0 || totalUncompressedBytes > MaximumUncompressedBytes - entry.Length)
                    return Blocked("Package limits exceeded", "The uncompressed package exceeds the 2 GiB inspection limit.", manifest, packageReadable: true);
                totalUncompressedBytes += entry.Length;
            }
            var manifestEntries = archive.Entries.Where(entry => string.Equals(NormalizeEntryPath(entry.FullName), ManifestFileName, StringComparison.Ordinal)).ToList();
            if (manifestEntries.Count != 1)
                return Blocked("Invalid manifest", "The package must contain exactly one root update manifest.", manifest, packageReadable: true);
            if (manifestEntries[0].Length > MaximumManifestBytes)
                return Blocked("Invalid manifest", "The update manifest exceeds the 1 MiB limit.", manifest, packageReadable: true);

            using (var stream = manifestEntries[0].Open())
                manifest = JsonSerializer.Deserialize<ApplicationUpdateManifest>(stream, JsonOptions);
            if (manifest is null)
                return Blocked("Invalid manifest", "The update manifest could not be read.", null, packageReadable: true);

            var manifestError = ValidateManifest(manifest);
            if (manifestError is not null)
                return Blocked("Invalid manifest", manifestError, manifest, packageReadable: true);

            var archiveFiles = archive.Entries
                .Where(entry => !string.IsNullOrEmpty(entry.Name))
                .Select(entry => (Entry: entry, Path: NormalizeEntryPath(entry.FullName)))
                .ToList();
            var duplicates = archiveFiles.GroupBy(item => item.Path, StringComparer.Ordinal).Where(group => group.Count() > 1).Select(group => group.Key).ToList();
            if (duplicates.Count > 0)
                return Blocked("Invalid file inventory", "Duplicate ZIP path: " + duplicates[0], manifest, true, true);

            var expected = manifest.Files.ToDictionary(file => file.Path, StringComparer.Ordinal);
            var actualPayload = archiveFiles.Where(item => item.Path != ManifestFileName).ToDictionary(item => item.Path, item => item.Entry, StringComparer.Ordinal);
            var missing = expected.Keys.Except(actualPayload.Keys, StringComparer.Ordinal).FirstOrDefault();
            var extra = actualPayload.Keys.Except(expected.Keys, StringComparer.Ordinal).FirstOrDefault();
            if (missing is not null || extra is not null)
                return Blocked("Invalid file inventory", missing is not null ? "Missing governed file: " + missing : "Unexpected file: " + extra, manifest, true, true);

            foreach (var file in manifest.Files)
            {
                var entry = actualPayload[file.Path];
                if (entry.Length != file.Length)
                    return Blocked("Hash verification failed", $"Length mismatch for {file.Path}.", manifest, true, true, true);
                using var stream = entry.Open();
                var hash = Convert.ToHexString(SHA256.HashData(stream));
                if (!string.Equals(hash, file.Sha256, StringComparison.Ordinal))
                    return Blocked("Hash verification failed", $"SHA-256 mismatch for {file.Path}.", manifest, true, true, true);
            }

            if (string.IsNullOrWhiteSpace(_trustedPublicKeyPem))
                return Blocked("Signing trust root not provisioned", "All package paths and hashes passed, but no production ECDSA public key is embedded. Update application remains disabled.", manifest, true, true, true, true);
            if (!VerifySignature(manifest, _trustedPublicKeyPem))
                return Blocked("Signature verification failed", "The manifest was not signed by the trusted 3DPIceland release key.", manifest, true, true, true, true);

            var versionValid = Version.TryParse(manifest.ReleaseVersion, out var packageVersion) && packageVersion > currentVersion;
            var databaseSchemaValid = currentDatabaseSchema >= manifest.MinimumDatabaseSchema && currentDatabaseSchema <= manifest.MaximumDatabaseSchema;
            if (!versionValid)
                return Blocked("Version blocked", $"Package v{manifest.ReleaseVersion} must be newer than installed v{currentVersion}.", manifest, true, true, true, true, true, false, databaseSchemaValid);
            if (!databaseSchemaValid)
                return Blocked("Database schema blocked", $"Current SQLite schema v{currentDatabaseSchema} is outside package support v{manifest.MinimumDatabaseSchema}-v{manifest.MaximumDatabaseSchema}.", manifest, true, true, true, true, true, true);

            return new ApplicationUpdateReadinessResult(true, true, true, true, true, true, true, true,
                "Ready for a future guarded updater", $"Signed package v{manifest.ReleaseVersion}; {manifest.Files.Count:N0} governed file(s); SQLite schema v{currentDatabaseSchema} supported.", manifest);
        }
        catch (InvalidDataException ex) { return Blocked("Unreadable ZIP package", ex.Message, manifest); }
        catch (Exception ex) { return Blocked("Package inspection failed", ex.Message, manifest); }
    }

    public ApplicationUpdateManifest ExtractVerifiedPackage(string packagePath, Version currentVersion, int currentDatabaseSchema, string stagingDirectory)
    {
        var readiness = Inspect(packagePath, currentVersion, currentDatabaseSchema);
        if (!readiness.Ready || readiness.Manifest is null) throw new InvalidOperationException(readiness.Status + ": " + readiness.Detail);
        var stagingRoot = Path.GetFullPath(stagingDirectory);
        if (Directory.Exists(stagingRoot) && Directory.EnumerateFileSystemEntries(stagingRoot).Any())
            throw new InvalidOperationException("Update staging directory must be new and empty.");
        Directory.CreateDirectory(stagingRoot);
        try
        {
            using var archive = ZipFile.OpenRead(packagePath);
            foreach (var file in readiness.Manifest.Files)
            {
                var entry = archive.Entries.Single(item => string.Equals(NormalizeEntryPath(item.FullName), file.Path, StringComparison.Ordinal));
                var destination = ResolveContained(stagingRoot, file.Path);
                Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
                using (var input = entry.Open()) using (var output = File.Create(destination)) input.CopyTo(output);
                using var staged = File.OpenRead(destination);
                if (staged.Length != file.Length || !string.Equals(Convert.ToHexString(SHA256.HashData(staged)), file.Sha256, StringComparison.Ordinal))
                    throw new IOException("Extracted staged file failed verification: " + file.Path);
            }
            return readiness.Manifest;
        }
        catch
        {
            try { Directory.Delete(stagingRoot, recursive: true); } catch { }
            throw;
        }
    }

    public static byte[] BuildSigningPayload(ApplicationUpdateManifest manifest)
    {
        var builder = new StringBuilder();
        builder.AppendLine(manifest.Schema);
        builder.AppendLine(manifest.ReleaseVersion);
        builder.AppendLine(manifest.ReleaseCode);
        builder.AppendLine(manifest.MinimumDatabaseSchema.ToString(CultureInfo.InvariantCulture));
        builder.AppendLine(manifest.MaximumDatabaseSchema.ToString(CultureInfo.InvariantCulture));
        builder.AppendLine(manifest.CreatedAtUtc);
        builder.AppendLine(manifest.SignatureAlgorithm);
        foreach (var file in manifest.Files.OrderBy(file => file.Path, StringComparer.Ordinal))
            builder.Append(file.Path).Append('\t').Append(file.Length.ToString(CultureInfo.InvariantCulture)).Append('\t').AppendLine(file.Sha256);
        return Encoding.UTF8.GetBytes(builder.ToString());
    }

    public static bool ProductionTrustRootIsValid()
    {
        try
        {
            using var verifier = ECDsa.Create();
            verifier.ImportFromPem(ProductionTrustedPublicKeyPem);
            return string.Equals(Convert.ToHexString(SHA256.HashData(verifier.ExportSubjectPublicKeyInfo())), ProductionTrustedPublicKeyFingerprint, StringComparison.Ordinal);
        }
        catch { return false; }
    }

    public static bool VerifyTrustedManifest(ApplicationUpdateManifest manifest) =>
        ValidateManifest(manifest) is null && VerifySignature(manifest, ProductionTrustedPublicKeyPem);

    public static ApplicationUpdateVerificationResult RunContractVerification()
    {
        var root = Path.Combine(Path.GetTempPath(), "3DPIceland-UpdateVerify-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            using var signer = ECDsa.Create(ECCurve.NamedCurves.nistP256);
            var publicKey = signer.ExportSubjectPublicKeyInfoPem();
            var validPackage = Path.Combine(root, "valid.zip");
            CreateFixture(validPackage, signer, "99.0.0", 29, 29, "fixture/app.exe", Encoding.UTF8.GetBytes("verified application payload"));
            var service = new ApplicationUpdatePackageService(publicKey);
            var valid = service.Inspect(validPackage, new Version(43, 4, 0), 29);
            if (!valid.Ready) return new(false, "Valid signed fixture was rejected: " + valid.Detail);

            var tampered = Path.Combine(root, "tampered.zip");
            File.Copy(validPackage, tampered);
            using (var archive = ZipFile.Open(tampered, ZipArchiveMode.Update))
            {
                var entry = archive.GetEntry("fixture/app.exe")!; entry.Delete();
                using var stream = archive.CreateEntry("fixture/app.exe").Open();
                stream.Write(Encoding.UTF8.GetBytes("tampered payload"));
            }
            var tamperedResult = service.Inspect(tampered, new Version(43, 4, 0), 29);
            if (tamperedResult.Ready || tamperedResult.HashesValid) return new(false, "Tampered payload was not blocked by hash verification.");

            var downgrade = Path.Combine(root, "downgrade.zip");
            CreateFixture(downgrade, signer, "43.3.1", 29, 29, "fixture/app.exe", Encoding.UTF8.GetBytes("older application payload"));
            var downgradeResult = service.Inspect(downgrade, new Version(43, 4, 0), 29);
            if (downgradeResult.Ready || downgradeResult.VersionValid) return new(false, "Downgrade package was not blocked.");

            var traversal = Path.Combine(root, "traversal.zip");
            CreateFixture(traversal, signer, "99.0.0", 29, 29, "../escape.exe", Encoding.UTF8.GetBytes("escape"));
            var traversalResult = service.Inspect(traversal, new Version(43, 4, 0), 29);
            if (traversalResult.ManifestValid) return new(false, "Path traversal package was not blocked.");

            var wrongProductionKey = new ApplicationUpdatePackageService().Inspect(validPackage, new Version(43, 4, 0), 29);
            if (wrongProductionKey.Ready || wrongProductionKey.SignatureValid) return new(false, "Fixture signed by an untrusted key was accepted by the production trust root.");
            var explicitlyMissingTrust = new ApplicationUpdatePackageService(string.Empty).Inspect(validPackage, new Version(43, 4, 0), 29);
            if (explicitlyMissingTrust.Ready || explicitlyMissingTrust.SignatureValid) return new(false, "Package was accepted without a trust root.");
            return new(true, "Signed fixture accepted; tampered payload, downgrade, path traversal, wrong production key and missing trust root blocked.");
        }
        catch (Exception ex) { return new(false, ex.Message); }
        finally { try { Directory.Delete(root, recursive: true); } catch { } }
    }

    private static void CreateFixture(string path, ECDsa signer, string version, int minSchema, int maxSchema, string filePath, byte[] bytes)
    {
        var manifest = new ApplicationUpdateManifest
        {
            ReleaseVersion = version, ReleaseCode = "VERIFICATION-FIXTURE", MinimumDatabaseSchema = minSchema, MaximumDatabaseSchema = maxSchema,
            CreatedAtUtc = "2026-07-22T00:00:00Z",
            Files = new() { new() { Path = filePath, Length = bytes.LongLength, Sha256 = Convert.ToHexString(SHA256.HashData(bytes)) } }
        };
        manifest.Signature = Convert.ToBase64String(signer.SignData(BuildSigningPayload(manifest), HashAlgorithmName.SHA256));
        using var archive = ZipFile.Open(path, ZipArchiveMode.Create);
        var payload = archive.CreateEntry(filePath);
        using (var stream = payload.Open()) stream.Write(bytes);
        var manifestEntry = archive.CreateEntry(ManifestFileName);
        using var manifestStream = manifestEntry.Open();
        JsonSerializer.Serialize(manifestStream, manifest, new JsonSerializerOptions { WriteIndented = true });
    }

    private static string? ValidateManifest(ApplicationUpdateManifest manifest)
    {
        if (manifest.Schema != ApplicationUpdateManifest.CurrentSchema) return "Unsupported update manifest schema.";
        if (!Version.TryParse(manifest.ReleaseVersion, out _)) return "ReleaseVersion is invalid.";
        if (string.IsNullOrWhiteSpace(manifest.ReleaseCode)) return "ReleaseCode is required.";
        if (manifest.MinimumDatabaseSchema <= 0 || manifest.MaximumDatabaseSchema < manifest.MinimumDatabaseSchema) return "Database schema range is invalid.";
        if (!DateTimeOffset.TryParse(manifest.CreatedAtUtc, CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out _)) return "CreatedAtUtc is invalid.";
        if (manifest.SignatureAlgorithm != "ECDSA-P256-SHA256") return "Unsupported signature algorithm.";
        if (manifest.Files.Count == 0) return "At least one governed application file is required.";
        var paths = new HashSet<string>(StringComparer.Ordinal);
        foreach (var file in manifest.Files)
        {
            if (!IsSafeRelativePath(file.Path) || file.Path == ManifestFileName) return "Unsafe or reserved governed path: " + file.Path;
            if (!paths.Add(file.Path)) return "Duplicate governed path: " + file.Path;
            if (file.Length < 0 || file.Sha256.Length != 64 || file.Sha256.Any(character => !Uri.IsHexDigit(character))) return "Invalid length or SHA-256 for " + file.Path;
            if (!string.Equals(file.Sha256, file.Sha256.ToUpperInvariant(), StringComparison.Ordinal)) return "SHA-256 must use uppercase hexadecimal for " + file.Path;
        }
        if (string.IsNullOrWhiteSpace(manifest.Signature)) return "Manifest signature is required.";
        return null;
    }

    private static bool VerifySignature(ApplicationUpdateManifest manifest, string publicKeyPem)
    {
        try
        {
            using var verifier = ECDsa.Create(); verifier.ImportFromPem(publicKeyPem);
            return verifier.VerifyData(BuildSigningPayload(manifest), Convert.FromBase64String(manifest.Signature), HashAlgorithmName.SHA256);
        }
        catch { return false; }
    }

    private static bool IsSafeRelativePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path.Contains('\\') || path.StartsWith('/') || Path.IsPathRooted(path)) return false;
        var segments = path.Split('/');
        return segments.All(segment => segment.Length > 0 && segment != "." && segment != ".." && segment.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
    }

    private static bool IsCanonicalArchivePath(string path)
    {
        if (string.IsNullOrWhiteSpace(path) || path.Contains('\\') || path.StartsWith('/') || Path.IsPathRooted(path)) return false;
        var candidate = path.EndsWith('/') ? path[..^1] : path;
        return IsSafeRelativePath(candidate);
    }

    private static string NormalizeEntryPath(string path) => path.Replace('\\', '/');

    private static string ResolveContained(string root, string relativePath)
    {
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(Path.Combine(fullRoot, relativePath.Replace('/', Path.DirectorySeparatorChar)));
        if (!full.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Package path escapes staging root: " + relativePath);
        return full;
    }

    private static ApplicationUpdateReadinessResult Blocked(string status, string detail, ApplicationUpdateManifest? manifest,
        bool packageReadable = false, bool manifestValid = false, bool inventoryValid = false, bool hashesValid = false,
        bool signatureValid = false, bool versionValid = false, bool schemaValid = false) =>
        new(packageReadable, manifestValid, inventoryValid, hashesValid, signatureValid, versionValid, schemaValid, false, status, detail, manifest);
}
