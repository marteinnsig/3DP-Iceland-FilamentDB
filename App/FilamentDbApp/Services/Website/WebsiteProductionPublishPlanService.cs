using FilamentDbApp.Services;
using FilamentDbApp.Services.Reporting;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text.Json;

namespace FilamentDbApp.Services.Website;

public sealed class WebsiteProductionPublishPlanService
{
    public const string Schema = "3dpiceland.website-production-publish-plan.v1";
    public const string FileName = "website-publish-plan.json";
    public const string TestSchema = "3dpiceland.website-test-publish-plan.v1";
    public const string TestFileName = "website-test-publish-plan.json";
    public const string TestEntryFileName = "website-test-entry.html";
    public const string TestRemoteRoot = "/preview";
    public const string TestEntryRemotePath = "/index-test.html";

    public WebsiteProductionPublishPlan Build(string websiteRoot, DateTime generatedAt)
    {
        var root = Path.GetFullPath(websiteRoot);
        var reportsRoot = Path.Combine(root, "reports");
        var catalogPath = Path.Combine(reportsRoot, PublicEngineeringReportPackageService.CatalogFileName);
        var required = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Path.Combine(root, "index.html"),
            Path.Combine(root, "manufacturers", "index.html"),
            Path.Combine(root, DocumentationEngineService.WhitepaperFileName),
            IOPath.Combine(root, WebsiteBrandingAssetService.FaviconRelativePath),
            IOPath.Combine(root, WebsiteBrandingAssetService.LogoRelativePath.Replace('/', IOPath.DirectorySeparatorChar)),
            Path.Combine(reportsRoot, "index.html"),
            catalogPath,
            Path.Combine(reportsRoot, PublicEngineeringReportPackageService.ManifestFileName),
            Path.Combine(reportsRoot, PublicReportSourceFingerprintService.FileName)
        };

        using var catalog = JsonDocument.Parse(File.ReadAllText(catalogPath));
        var publicData = catalog.RootElement.GetProperty("publicData");
        var publicMaterials = publicData.GetProperty("PublicMaterials").GetInt32();
        var reports = publicData.GetProperty("Reports");
        foreach (var report in reports.EnumerateArray())
        {
            foreach (var property in new[] { "Html", "Pdf", "Metadata" })
            {
                var route = report.GetProperty(property).GetString() ?? string.Empty;
                var path = ResolveSafeRoute(root, route);
                required.Add(path);
                var directory = Path.GetDirectoryName(path)!;
                if (Directory.Exists(directory))
                {
                    foreach (var file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
                        required.Add(Path.GetFullPath(file));
                }
            }
        }

        var sharedAssets = Path.Combine(reportsRoot, "assets");
        if (Directory.Exists(sharedAssets))
        {
            foreach (var file in Directory.EnumerateFiles(sharedAssets, "*", SearchOption.AllDirectories))
                required.Add(Path.GetFullPath(file));
        }

        var entries = required.Select(path => BuildEntry(root, path)).ToList();
        var mainIndex = entries.Single(entry => string.Equals(entry.RemotePath, "/index.html", StringComparison.Ordinal));
        entries.Remove(mainIndex);
        entries = entries
            .OrderBy(entry => entry.RemotePath.EndsWith("/index.html", StringComparison.Ordinal) ? 1 : 0)
            .ThenBy(entry => entry.RemotePath, StringComparer.Ordinal)
            .Append(mainIndex)
            .ToList();

        return new WebsiteProductionPublishPlan
        {
            Schema = Schema,
            Version = BuildInfo.ShortLabel,
            GeneratedAtUtc = generatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            PublicMaterials = publicMaterials,
            CatalogEntries = reports.GetArrayLength(),
            TotalBytes = entries.Sum(entry => entry.Bytes),
            Files = entries
        };
    }

    public WebsiteProductionPublishPlan BuildTest(string websiteRoot, DateTime generatedAt)
    {
        var root = Path.GetFullPath(websiteRoot);
        var reportsRoot = Path.Combine(root, "reports");
        var catalogPath = Path.Combine(reportsRoot, PublicEngineeringReportPackageService.CatalogFileName);
        var entryPath = Path.Combine(root, TestEntryFileName);
        SafeFileOperations.WriteAllTextAtomic(
            entryPath,
            "<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"robots\" content=\"noindex,nofollow\"><meta http-equiv=\"refresh\" content=\"0;url=preview/index-test.html\"><title>3DPIceland Website Test</title></head><body><p>Opening the guarded website test preview… <a href=\"preview/index-test.html\">Continue</a></p></body></html>",
            System.Text.Encoding.UTF8);

        var required = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
        {
            Path.Combine(root, "index-test.html"),
            Path.Combine(root, "manufacturers", "index-test.html"),
            Path.Combine(root, DocumentationEngineService.WhitepaperFileName),
            IOPath.Combine(root, WebsiteBrandingAssetService.FaviconRelativePath),
            IOPath.Combine(root, WebsiteBrandingAssetService.LogoRelativePath.Replace('/', IOPath.DirectorySeparatorChar)),
            Path.Combine(reportsRoot, "index-test.html"),
            catalogPath,
            Path.Combine(reportsRoot, PublicEngineeringReportPackageService.ManifestFileName),
            Path.Combine(reportsRoot, PublicReportSourceFingerprintService.FileName)
        };

        using var catalog = JsonDocument.Parse(File.ReadAllText(catalogPath));
        var publicData = catalog.RootElement.GetProperty("publicData");
        var publicMaterials = publicData.GetProperty("PublicMaterials").GetInt32();
        var reports = publicData.GetProperty("Reports");
        foreach (var report in reports.EnumerateArray())
        {
            foreach (var property in new[] { "Html", "Pdf", "Metadata" })
            {
                var route = report.GetProperty(property).GetString() ?? string.Empty;
                var path = ResolveSafeRoute(root, route);
                required.Add(path);
                var directory = Path.GetDirectoryName(path)!;
                if (Directory.Exists(directory))
                {
                    foreach (var file in Directory.EnumerateFiles(directory, "*", SearchOption.AllDirectories))
                        required.Add(Path.GetFullPath(file));
                }
            }
        }

        var sharedAssets = Path.Combine(reportsRoot, "assets");
        if (Directory.Exists(sharedAssets))
        {
            foreach (var file in Directory.EnumerateFiles(sharedAssets, "*", SearchOption.AllDirectories))
                required.Add(Path.GetFullPath(file));
        }

        var entries = required
            .Select(path => BuildMappedEntry(path, TestRemoteRoot + "/" + Path.GetRelativePath(root, path).Replace(Path.DirectorySeparatorChar, '/')))
            .OrderBy(entry => entry.RemotePath, StringComparer.Ordinal)
            .ToList();
        entries.Add(BuildMappedEntry(entryPath, TestEntryRemotePath));

        return new WebsiteProductionPublishPlan
        {
            Schema = TestSchema,
            Version = BuildInfo.ShortLabel,
            GeneratedAtUtc = generatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture),
            PublicMaterials = publicMaterials,
            CatalogEntries = reports.GetArrayLength(),
            TotalBytes = entries.Sum(entry => entry.Bytes),
            Files = entries
        };
    }

    public string Serialize(WebsiteProductionPublishPlan plan) =>
        JsonSerializer.Serialize(plan, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

    public WebsiteProductionPublishPlan Load(string path)
    {
        if (!File.Exists(path)) throw new FileNotFoundException("The Production publish plan is missing.", path);
        var plan = JsonSerializer.Deserialize<WebsiteProductionPublishPlan>(
            File.ReadAllText(path),
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
        return plan ?? throw new InvalidOperationException("The Production publish plan could not be read.");
    }

    public bool Verify(WebsiteProductionPublishPlan plan) =>
        string.Equals(plan.Schema, Schema, StringComparison.Ordinal) &&
        string.Equals(plan.Version, BuildInfo.ShortLabel, StringComparison.Ordinal) &&
        plan.PublicMaterials > 0 && plan.CatalogEntries > 0 && plan.Files.Count > 0 &&
        plan.TotalBytes == plan.Files.Sum(entry => entry.Bytes) &&
        plan.Files.Select(entry => entry.RemotePath).Distinct(StringComparer.Ordinal).Count() == plan.Files.Count &&
        plan.Files.All(entry => IsSafeRemotePath(entry.RemotePath) && entry.Bytes > 0 && entry.Sha256.Length == 64) &&
        string.Equals(plan.Files[^1].RemotePath, "/index.html", StringComparison.Ordinal) &&
        plan.Files.All(entry => !entry.RemotePath.Contains("index-test.html", StringComparison.OrdinalIgnoreCase));

    public bool VerifyTest(WebsiteProductionPublishPlan plan) =>
        string.Equals(plan.Schema, TestSchema, StringComparison.Ordinal) &&
        string.Equals(plan.Version, BuildInfo.ShortLabel, StringComparison.Ordinal) &&
        plan.PublicMaterials > 0 && plan.CatalogEntries > 0 && plan.Files.Count > 0 &&
        plan.TotalBytes == plan.Files.Sum(entry => entry.Bytes) &&
        plan.Files.Select(entry => entry.RemotePath).Distinct(StringComparer.Ordinal).Count() == plan.Files.Count &&
        plan.Files.All(entry => IsSafeTestRemotePath(entry.RemotePath) && entry.Bytes > 0 && entry.Sha256.Length == 64) &&
        string.Equals(plan.Files[^1].RemotePath, TestEntryRemotePath, StringComparison.Ordinal) &&
        plan.Files.Count(entry => string.Equals(entry.RemotePath, TestEntryRemotePath, StringComparison.Ordinal)) == 1;

    public bool VerifyForPublish(WebsiteProductionPublishPlan plan) => Verify(plan) || VerifyTest(plan);

    public bool VerifyLocalArtifacts(WebsiteProductionPublishPlan plan, string websiteRoot, out string failure)
    {
        failure = string.Empty;
        if (!VerifyForPublish(plan))
        {
            failure = "The Production publish plan schema, release identity, allowlist or activation order is invalid.";
            return false;
        }

        var root = Path.GetFullPath(websiteRoot);
        var isTest = string.Equals(plan.Schema, TestSchema, StringComparison.Ordinal);
        foreach (var entry in plan.Files)
        {
            var fullPath = Path.GetFullPath(entry.LocalPath);
            if (!fullPath.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            {
                failure = "A publish-plan artifact escaped the selected website root: " + entry.LocalPath;
                return false;
            }
            var relativePath = Path.GetRelativePath(root, fullPath).Replace(Path.DirectorySeparatorChar, '/');
            var expectedRemotePath = isTest
                ? string.Equals(relativePath, TestEntryFileName, StringComparison.OrdinalIgnoreCase)
                    ? TestEntryRemotePath
                    : TestRemoteRoot + "/" + relativePath
                : "/" + relativePath;
            if (!string.Equals(expectedRemotePath, entry.RemotePath, StringComparison.Ordinal))
            {
                failure = "A publish-plan local/remote route mapping is invalid: " + entry.RemotePath;
                return false;
            }
            if (!File.Exists(entry.LocalPath))
            {
                failure = "A publish-plan artifact is missing: " + entry.LocalPath;
                return false;
            }

            var info = new FileInfo(entry.LocalPath);
            if (info.Length != entry.Bytes)
            {
                failure = "A publish-plan artifact changed size after Production generation: " + entry.LocalPath;
                return false;
            }

            using var stream = File.OpenRead(entry.LocalPath);
            var actualHash = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
            if (!string.Equals(actualHash, entry.Sha256, StringComparison.Ordinal))
            {
                failure = "A publish-plan artifact changed content after Production generation: " + entry.LocalPath;
                return false;
            }
        }

        return true;
    }

    private static WebsiteProductionPublishFile BuildEntry(string root, string path)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath)) throw new FileNotFoundException("A production publish artifact is missing.", fullPath);
        if (!fullPath.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("A production publish artifact escaped the website root.");
        var info = new FileInfo(fullPath);
        if (info.Length <= 0) throw new InvalidOperationException("A production publish artifact is empty: " + fullPath);
        return BuildMappedEntry(fullPath, "/" + Path.GetRelativePath(root, fullPath).Replace(Path.DirectorySeparatorChar, '/'));
    }

    private static WebsiteProductionPublishFile BuildMappedEntry(string path, string remotePath)
    {
        var fullPath = Path.GetFullPath(path);
        if (!File.Exists(fullPath)) throw new FileNotFoundException("A publish artifact is missing.", fullPath);
        var info = new FileInfo(fullPath);
        if (info.Length <= 0) throw new InvalidOperationException("A publish artifact is empty: " + fullPath);
        using var stream = File.OpenRead(fullPath);
        return new WebsiteProductionPublishFile
        {
            LocalPath = fullPath,
            RemotePath = remotePath,
            Bytes = info.Length,
            Sha256 = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant()
        };
    }

    private static string ResolveSafeRoute(string root, string route)
    {
        if (string.IsNullOrWhiteSpace(route) || route.Contains("..", StringComparison.Ordinal) || Path.IsPathRooted(route))
            throw new InvalidOperationException("Unsafe public report route in production catalog: " + route);
        var path = Path.GetFullPath(Path.Combine(root, route.Replace('/', Path.DirectorySeparatorChar)));
        if (!path.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Public report route escaped the production website root: " + route);
        return path;
    }

    private static bool IsSafeRemotePath(string path) =>
        path.StartsWith("/", StringComparison.Ordinal) &&
        !path.Contains("..", StringComparison.Ordinal) &&
        !path.Contains('\\') &&
        !path.Contains("index-test.html", StringComparison.OrdinalIgnoreCase) &&
        !path.StartsWith("/backups/", StringComparison.OrdinalIgnoreCase);

    private static bool IsSafeTestRemotePath(string path) =>
        (string.Equals(path, TestEntryRemotePath, StringComparison.Ordinal) || path.StartsWith(TestRemoteRoot + "/", StringComparison.Ordinal)) &&
        !path.Contains("..", StringComparison.Ordinal) &&
        !path.Contains('\\') &&
        !string.Equals(path, "/index.html", StringComparison.OrdinalIgnoreCase) &&
        !path.StartsWith("/reports/", StringComparison.OrdinalIgnoreCase) &&
        !string.Equals(path, "/manufacturers/index.html", StringComparison.OrdinalIgnoreCase) &&
        !path.StartsWith("/backups/", StringComparison.OrdinalIgnoreCase);
}

public sealed class WebsiteProductionPublishPlan
{
    public string Schema { get; init; } = string.Empty;
    public string Version { get; init; } = string.Empty;
    public string GeneratedAtUtc { get; init; } = string.Empty;
    public int PublicMaterials { get; init; }
    public int CatalogEntries { get; init; }
    public long TotalBytes { get; init; }
    public List<WebsiteProductionPublishFile> Files { get; init; } = new();
}

public sealed class WebsiteProductionPublishFile
{
    public string LocalPath { get; init; } = string.Empty;
    public string RemotePath { get; init; } = string.Empty;
    public long Bytes { get; init; }
    public string Sha256 { get; init; } = string.Empty;
}
