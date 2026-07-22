using System.Globalization;
using System.IO.Compression;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

const string KeyName = "3DPIceland.ApplicationUpdate.Release.v1";
const string ManifestFileName = "3dp-update-manifest.json";
var allowedFiles = new HashSet<string>(StringComparer.Ordinal)
{
    "3DPIcelandFilamentDB.exe",
    "3DPIcelandUpdater.exe",
    "LICENSE",
    "THIRD-PARTY-NOTICES.md",
    "Assets/3dp-iceland-labs-icon.ico",
    "Assets/3dp-iceland-labs-logo-pdf.jpg",
};
var knownBuildOnlyFiles = new HashSet<string>(StringComparer.Ordinal)
{
    "3DPIcelandFilamentDB.pdb",
    "UpdateCore.pdb",
    "Microsoft.Web.WebView2.Core.xml",
    "Microsoft.Web.WebView2.WinForms.xml",
    "Microsoft.Web.WebView2.Wpf.xml"
};

try
{
    if (args.Length == 0) return Usage();
    return args[0] switch
    {
        "init-key" => InitializeKey(),
        "show-public-key" => ShowPublicKey(),
        "package" => CreatePackage(ParseOptions(args.Skip(1).ToArray())),
        _ => Usage()
    };
}
catch (Exception ex)
{
    Console.Error.WriteLine("Release packager failed: " + ex.Message);
    return 1;
}

int InitializeKey()
{
    if (!OperatingSystem.IsWindows()) throw new PlatformNotSupportedException("The governed release key uses Windows CNG.");
    if (!CngKey.Exists(KeyName, CngProvider.MicrosoftSoftwareKeyStorageProvider))
    {
        var creation = new CngKeyCreationParameters
        {
            Provider = CngProvider.MicrosoftSoftwareKeyStorageProvider,
            KeyCreationOptions = CngKeyCreationOptions.None,
            ExportPolicy = CngExportPolicies.None,
            KeyUsage = CngKeyUsages.Signing
        };
        using var created = CngKey.Create(CngAlgorithm.ECDsaP256, KeyName, creation);
        Console.WriteLine("Created user-scoped non-exportable CNG release key: " + KeyName);
    }
    else Console.WriteLine("Release key already exists; no key was replaced: " + KeyName);
    return ShowPublicKey();
}

int ShowPublicKey()
{
    using var key = OpenReleaseKey();
    using var signer = new ECDsaCng(key);
    var pem = signer.ExportSubjectPublicKeyInfoPem();
    var probe = Encoding.UTF8.GetBytes("3DPIceland release-key protection probe");
    var signature = signer.SignData(probe, HashAlgorithmName.SHA256);
    if (!signer.VerifyData(probe, signature, HashAlgorithmName.SHA256)) throw new CryptographicException("Release key sign/verify probe failed.");
    try
    {
        _ = signer.ExportPkcs8PrivateKey();
        throw new CryptographicException("Release private key is exportable; key governance failed.");
    }
    catch (CryptographicException ex) when (!ex.Message.Contains("governance failed", StringComparison.Ordinal))
    {
        // Expected: the persisted production private key is non-exportable.
    }
    Console.WriteLine(pem);
    Console.WriteLine("SHA256 fingerprint: " + Convert.ToHexString(SHA256.HashData(signer.ExportSubjectPublicKeyInfo())));
    Console.WriteLine("Private key export: BLOCKED; sign/verify probe: PASS");
    return 0;
}

int CreatePackage(Dictionary<string, string> options)
{
    var input = RequirePath(options, "input", mustExist: true);
    var output = RequirePath(options, "output", mustExist: false);
    var versionText = Require(options, "version");
    if (!Version.TryParse(versionText, out _)) throw new InvalidOperationException("--version must be a numeric release version.");
    var code = Require(options, "code");
    var minSchema = ParsePositiveInt(options, "min-schema");
    var maxSchema = ParsePositiveInt(options, "max-schema");
    if (maxSchema < minSchema) throw new InvalidOperationException("--max-schema cannot be less than --min-schema.");
    if (File.Exists(output)) throw new IOException("Output package already exists: " + output);
    Directory.CreateDirectory(Path.GetDirectoryName(output) ?? throw new InvalidOperationException("Output folder is unavailable."));

    var discovered = Directory.GetFiles(input, "*", SearchOption.AllDirectories)
        .Select(path => new SourceFile(path, Path.GetRelativePath(input, path).Replace('\\', '/')))
        .OrderBy(file => file.RelativePath, StringComparer.Ordinal)
        .ToList();
    var unexpected = discovered.Where(file => !allowedFiles.Contains(file.RelativePath) && !knownBuildOnlyFiles.Contains(file.RelativePath)).Select(file => file.RelativePath).ToList();
    if (unexpected.Count > 0) throw new InvalidOperationException("Unexpected publish output: " + string.Join(", ", unexpected));
    var missing = allowedFiles.Except(discovered.Select(file => file.RelativePath), StringComparer.Ordinal).ToList();
    if (missing.Count > 0) throw new InvalidOperationException("Required publish output is missing: " + string.Join(", ", missing));

    var packageFiles = discovered.Where(file => allowedFiles.Contains(file.RelativePath)).ToList();
    var manifest = new UpdateManifest
    {
        ReleaseVersion = versionText,
        ReleaseCode = code,
        MinimumDatabaseSchema = minSchema,
        MaximumDatabaseSchema = maxSchema,
        CreatedAtUtc = DateTimeOffset.UtcNow.ToString("O", CultureInfo.InvariantCulture),
        Files = packageFiles.Select(file => new UpdateFile
        {
            Path = file.RelativePath,
            Length = new FileInfo(file.FullPath).Length,
            Sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(file.FullPath)))
        }).ToList()
    };
    using (var key = OpenReleaseKey())
    using (var signer = new ECDsaCng(key))
        manifest.Signature = Convert.ToBase64String(signer.SignData(BuildSigningPayload(manifest), HashAlgorithmName.SHA256));

    using var archive = ZipFile.Open(output, ZipArchiveMode.Create);
    foreach (var file in packageFiles)
        archive.CreateEntryFromFile(file.FullPath, file.RelativePath, CompressionLevel.Optimal);
    var manifestEntry = archive.CreateEntry(ManifestFileName, CompressionLevel.Optimal);
    using (var stream = manifestEntry.Open())
        JsonSerializer.Serialize(stream, manifest, new JsonSerializerOptions { WriteIndented = true });
    Console.WriteLine("Created signed update package: " + output);
    Console.WriteLine($"Release v{versionText} {code}; {packageFiles.Count:N0} governed files; SQLite schema v{minSchema}-v{maxSchema}.");
    return 0;
}

CngKey OpenReleaseKey()
{
    if (!CngKey.Exists(KeyName, CngProvider.MicrosoftSoftwareKeyStorageProvider))
        throw new InvalidOperationException("Release key is not initialized. Run init-key first.");
    return CngKey.Open(KeyName, CngProvider.MicrosoftSoftwareKeyStorageProvider, CngKeyOpenOptions.None);
}

byte[] BuildSigningPayload(UpdateManifest manifest)
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

Dictionary<string, string> ParseOptions(string[] optionArgs)
{
    if (optionArgs.Length % 2 != 0) throw new ArgumentException("Every option must have a value.");
    var result = new Dictionary<string, string>(StringComparer.Ordinal);
    for (var index = 0; index < optionArgs.Length; index += 2)
    {
        if (!optionArgs[index].StartsWith("--", StringComparison.Ordinal)) throw new ArgumentException("Invalid option: " + optionArgs[index]);
        if (!result.TryAdd(optionArgs[index][2..], optionArgs[index + 1])) throw new ArgumentException("Duplicate option: " + optionArgs[index]);
    }
    return result;
}

string Require(Dictionary<string, string> options, string key) =>
    options.TryGetValue(key, out var value) && !string.IsNullOrWhiteSpace(value) ? value : throw new ArgumentException("Missing --" + key + ".");

string RequirePath(Dictionary<string, string> options, string key, bool mustExist)
{
    var path = Path.GetFullPath(Require(options, key));
    if (mustExist && !Directory.Exists(path)) throw new DirectoryNotFoundException(path);
    return path;
}

int ParsePositiveInt(Dictionary<string, string> options, string key) =>
    int.TryParse(Require(options, key), NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) && value > 0
        ? value : throw new ArgumentException("--" + key + " must be a positive integer.");

int Usage()
{
    Console.Error.WriteLine("Usage:");
    Console.Error.WriteLine("  ReleasePackager init-key");
    Console.Error.WriteLine("  ReleasePackager show-public-key");
    Console.Error.WriteLine("  ReleasePackager package --input <publish> --output <zip> --version <x.y.z> --code <code> --min-schema <n> --max-schema <n>");
    return 2;
}

sealed record SourceFile(string FullPath, string RelativePath);

sealed class UpdateManifest
{
    public string Schema { get; set; } = "3dpiceland.application-update.v1";
    public string ReleaseVersion { get; set; } = string.Empty;
    public string ReleaseCode { get; set; } = string.Empty;
    public int MinimumDatabaseSchema { get; set; }
    public int MaximumDatabaseSchema { get; set; }
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string SignatureAlgorithm { get; set; } = "ECDSA-P256-SHA256";
    public List<UpdateFile> Files { get; set; } = new();
    public string Signature { get; set; } = string.Empty;
}

sealed class UpdateFile
{
    public string Path { get; set; } = string.Empty;
    public long Length { get; set; }
    public string Sha256 { get; set; } = string.Empty;
}
