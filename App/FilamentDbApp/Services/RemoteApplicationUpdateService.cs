using FilamentDbApp.Models;
using System.IO;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text.Json;

namespace FilamentDbApp.Services;

public sealed class RemoteApplicationUpdateService
{
    public const string FeedUrl = "https://www.iskort.is/3dp/updates/latest.json";
    private const long MaximumFeedBytes = 1_048_576;
    private const long MaximumPackageBytes = 536_870_912;
    private static readonly HttpClient Client = new() { Timeout = TimeSpan.FromSeconds(30) };
    private static readonly JsonSerializerOptions JsonOptions = new() { PropertyNameCaseInsensitive = true };

    public async Task<RemoteUpdateDiscoveryResult> CheckAsync(Version currentVersion, int schema, CancellationToken cancellationToken = default)
    {
        try
        {
            using var response = await Client.GetAsync(FeedUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (!response.IsSuccessStatusCode) return new(false, "Feed unavailable", $"HTTPS returned {(int)response.StatusCode}.", null);
            if (response.Content.Headers.ContentLength is > MaximumFeedBytes) return new(false, "Feed blocked", "Update feed exceeds 1 MiB.", null);
            await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
            var feedBytes = await ReadBoundedAsync(stream, MaximumFeedBytes, cancellationToken);
            var feed = JsonSerializer.Deserialize<ApplicationUpdateFeed>(feedBytes, JsonOptions);
            var error = ValidateFeed(feed);
            if (error is not null) return new(false, "Feed blocked", error, feed);
            var version = Version.Parse(feed!.Manifest.ReleaseVersion);
            if (version <= currentVersion) return new(false, "Up to date", $"Installed v{currentVersion}; feed v{version}.", feed);
            if (schema < feed.Manifest.MinimumDatabaseSchema || schema > feed.Manifest.MaximumDatabaseSchema)
                return new(false, "Schema blocked", $"SQLite schema v{schema} is outside v{feed.Manifest.MinimumDatabaseSchema}-v{feed.Manifest.MaximumDatabaseSchema}.", feed);
            return new(true, "Update available", $"Signed update v{version} {feed.Manifest.ReleaseCode} is available.", feed);
        }
        catch (Exception ex) { return new(false, "Check failed", ex.Message, null); }
    }

    public async Task<RemoteUpdateDownloadResult> DownloadAsync(ApplicationUpdateFeed feed, Version currentVersion, int schema, CancellationToken cancellationToken = default)
    {
        var error = ValidateFeed(feed); if (error is not null) throw new InvalidOperationException(error);
        var root = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "3DPIcelandLabs", "Updates", "downloads");
        Directory.CreateDirectory(root);
        var path = Path.Combine(root, $"3DPIceland_Update_v{feed.Manifest.ReleaseVersion.Replace('.', '_')}_{Guid.NewGuid():N}.zip");
        try
        {
            using var response = await Client.GetAsync(feed.PackageUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            response.EnsureSuccessStatusCode();
            if (response.Content.Headers.ContentLength is long length && length != feed.PackageBytes) throw new IOException("Remote Content-Length differs from the signed feed.");
            await using var input = await response.Content.ReadAsStreamAsync(cancellationToken);
            long received = 0;
            using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
            await using (var output = new FileStream(path, FileMode.CreateNew, FileAccess.Write, FileShare.None, 81920, true))
            {
                var buffer = new byte[81920];
                while (true)
                {
                    var read = await input.ReadAsync(buffer, cancellationToken);
                    if (read == 0) break;
                    received += read;
                    if (received > feed.PackageBytes || received > MaximumPackageBytes) throw new IOException("Downloaded package exceeded its governed byte limit.");
                    hash.AppendData(buffer, 0, read);
                    await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                }
            }
            if (received != feed.PackageBytes || Convert.ToHexString(hash.GetHashAndReset()) != feed.PackageSha256)
                throw new IOException("Downloaded package bytes or SHA-256 differ from the feed.");
            var readiness = new ApplicationUpdatePackageService().Inspect(path, currentVersion, schema);
            if (!readiness.Ready || readiness.Manifest?.Signature != feed.Manifest.Signature || readiness.Manifest.ReleaseVersion != feed.Manifest.ReleaseVersion)
                throw new InvalidOperationException("Downloaded package did not reproduce the feed's trusted signed manifest.");
            return new(path, readiness);
        }
        catch { try { if (File.Exists(path)) File.Delete(path); } catch { } throw; }
    }

    public static string? ValidateFeed(ApplicationUpdateFeed? feed)
    {
        if (feed is null || feed.Schema != ApplicationUpdateFeed.CurrentSchema) return "Unsupported update-feed schema.";
        if (feed.PackageBytes <= 0 || feed.PackageBytes > MaximumPackageBytes || feed.PackageSha256.Length != 64 || feed.PackageSha256.Any(c => !Uri.IsHexDigit(c))) return "Invalid package bytes or SHA-256.";
        if (!Uri.TryCreate(feed.PackageUrl, UriKind.Absolute, out var uri) || uri.Scheme != Uri.UriSchemeHttps || !uri.Host.Equals("www.iskort.is", StringComparison.OrdinalIgnoreCase) || !uri.AbsolutePath.StartsWith("/3dp/updates/", StringComparison.Ordinal)) return "Package URL is outside the governed HTTPS update root.";
        return ApplicationUpdatePackageService.VerifyTrustedManifest(feed.Manifest) ? null : "Feed manifest signature is not trusted.";
    }

    private static async Task<byte[]> ReadBoundedAsync(Stream input, long maximumBytes, CancellationToken cancellationToken)
    {
        using var output = new MemoryStream();
        var buffer = new byte[81920];
        while (true)
        {
            var read = await input.ReadAsync(buffer, cancellationToken);
            if (read == 0) return output.ToArray();
            if (output.Length + read > maximumBytes) throw new IOException($"HTTP response exceeded the governed {maximumBytes:N0}-byte limit.");
            await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
        }
    }
}
