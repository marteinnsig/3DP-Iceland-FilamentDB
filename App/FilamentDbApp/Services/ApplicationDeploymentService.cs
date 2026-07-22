using FilamentDbApp.Models;
using FluentFTP;
using System.IO;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text.Json;

namespace FilamentDbApp.Services;

public sealed class ApplicationDeploymentService
{
    public const string PlanFileName = "application-deployment-plan.json";
    public const string DownloadsRoot = "/downloads";
    public const string BackupRoot = "/backups/application_releases";
    private static readonly IReadOnlyDictionary<string, string> StablePaths = new Dictionary<string, string>(StringComparer.Ordinal)
    {
        ["Installer"] = DownloadsRoot + "/3DPIceland-Setup-x64.exe",
        ["Portable"] = DownloadsRoot + "/3DPIceland-Portable-x64.zip"
    };

    private readonly DeploymentSettingsRecord _settings;
    public ApplicationDeploymentService(DeploymentSettingsRecord settings) => _settings = settings;

    public ApplicationDeploymentPlan LoadAndVerify(string planPath)
    {
        var fullPlanPath = Path.GetFullPath(planPath);
        var root = Path.GetDirectoryName(fullPlanPath) ?? throw new InvalidOperationException("Deployment plan folder is unavailable.");
        var plan = JsonSerializer.Deserialize<ApplicationDeploymentPlan>(File.ReadAllText(fullPlanPath), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? throw new InvalidOperationException("Application deployment plan could not be read.");
        if (plan.Schema != ApplicationDeploymentPlan.CurrentSchema || !Version.TryParse(plan.ReleaseVersion, out _) ||
            string.IsNullOrWhiteSpace(plan.ReleaseCode) || plan.SourcePackageSha256.Length != 64 || plan.Files.Count != StablePaths.Count)
            throw new InvalidOperationException("Application deployment plan identity is invalid.");
        if (!StablePaths.Keys.OrderBy(value => value).SequenceEqual(plan.Files.Select(file => file.Kind).OrderBy(value => value), StringComparer.Ordinal))
            throw new InvalidOperationException("Application deployment plan must contain exactly Installer and Portable artifacts.");
        foreach (var file in plan.Files)
        {
            if (!StablePaths.TryGetValue(file.Kind, out var stable) || file.StableRemotePath != stable ||
                !file.VersionedRemotePath.StartsWith(DownloadsRoot + "/", StringComparison.Ordinal) ||
                file.VersionedRemotePath == file.StableRemotePath || file.VersionedRemotePath.Contains("..", StringComparison.Ordinal) ||
                file.LocalFile.Contains('/') || file.LocalFile.Contains('\\') || file.Bytes <= 0 || file.Sha256.Length != 64)
                throw new InvalidOperationException("Unsafe application deployment artifact: " + file.Kind);
            var localPath = Path.GetFullPath(Path.Combine(root, file.LocalFile));
            if (!localPath.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !File.Exists(localPath))
                throw new FileNotFoundException("Deployment artifact is missing or outside the plan folder.", localPath);
            if (new FileInfo(localPath).Length != file.Bytes || Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(localPath))) != file.Sha256)
                throw new IOException("Deployment artifact bytes or SHA-256 changed: " + file.LocalFile);
        }
        return plan;
    }

    public ApplicationDeploymentPublishResult Publish(string password, string planPath)
    {
        var plan = LoadAndVerify(planPath);
        var root = Path.GetDirectoryName(Path.GetFullPath(planPath))!;
        using var client = CreateClient(password);
        client.Connect();
        EnsureDirectoryTree(client, DownloadsRoot);
        var backup = BackupRoot + "/release_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmss_fff");
        EnsureDirectoryTree(client, backup + "/original");
        EnsureDirectoryTree(client, backup + "/staged");
        var targets = plan.Files.SelectMany(file => new[]
        {
            new PublishTarget(Path.Combine(root, file.LocalFile), file.VersionedRemotePath, file.Bytes),
            new PublishTarget(Path.Combine(root, file.LocalFile), file.StableRemotePath, file.Bytes)
        }).OrderBy(target => StablePaths.Values.Contains(target.RemotePath, StringComparer.Ordinal) ? 1 : 0).ToList();
        var originals = new Dictionary<string, string>(StringComparer.Ordinal);
        var activated = new List<string>();
        try
        {
            foreach (var target in targets)
            {
                var key = target.RemotePath.Trim('/').Replace('/', '_');
                if (client.FileExists(target.RemotePath))
                {
                    var original = backup + "/original/" + key;
                    using var stream = new MemoryStream();
                    if (!client.DownloadStream(stream, target.RemotePath)) throw new IOException("Could not retain remote application-release backup: " + target.RemotePath);
                    stream.Position = 0;
                    if (client.UploadStream(stream, original, FtpRemoteExists.Overwrite, false) != FtpStatus.Success) throw new IOException("Could not upload application-release backup.");
                    originals[target.RemotePath] = original;
                }
                var staged = backup + "/staged/" + key;
                if (client.UploadFile(target.LocalPath, staged, FtpRemoteExists.Overwrite, false, FtpVerify.None) != FtpStatus.Success || client.GetFileSize(staged) != target.Bytes)
                    throw new IOException("Application-release staging validation failed: " + target.RemotePath);
                if (!client.MoveFile(staged, target.RemotePath, FtpRemoteExists.Overwrite)) throw new IOException("Application-release activation failed: " + target.RemotePath);
                activated.Add(target.RemotePath);
            }
            client.Disconnect();
            return new(backup, activated);
        }
        catch
        {
            foreach (var remotePath in activated.AsEnumerable().Reverse())
            {
                try
                {
                    if (originals.TryGetValue(remotePath, out var original))
                    {
                        using var stream = new MemoryStream();
                        if (client.DownloadStream(stream, original))
                        {
                            stream.Position = 0;
                            client.UploadStream(stream, remotePath, FtpRemoteExists.Overwrite, false);
                        }
                    }
                    else if (client.FileExists(remotePath)) client.DeleteFile(remotePath);
                }
                catch { }
            }
            throw;
        }
    }

    public ApplicationDeploymentPublishResult PublishUpdate(string password, string feedPath)
    {
        var fullFeed = Path.GetFullPath(feedPath);
        var root = Path.GetDirectoryName(fullFeed)!;
        var feed = JsonSerializer.Deserialize<ApplicationUpdateFeed>(File.ReadAllText(fullFeed), new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? throw new InvalidOperationException("Update feed could not be read.");
        var error = RemoteApplicationUpdateService.ValidateFeed(feed); if (error is not null) throw new InvalidOperationException(error);
        var packageName = Path.GetFileName(new Uri(feed.PackageUrl).AbsolutePath);
        var packagePath = Path.GetFullPath(Path.Combine(root, packageName));
        if (!packagePath.StartsWith(root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase) || !File.Exists(packagePath)) throw new FileNotFoundException("Feed package is missing.");
        if (new FileInfo(packagePath).Length != feed.PackageBytes || Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(packagePath))) != feed.PackageSha256) throw new IOException("Feed package bytes or SHA-256 changed.");
        var readiness = new ApplicationUpdatePackageService().Inspect(packagePath, new Version(0, 0, 0), feed.Manifest.MinimumDatabaseSchema);
        if (!readiness.SignatureValid || readiness.Manifest?.Signature != feed.Manifest.Signature) throw new InvalidOperationException("Feed package does not reproduce the trusted manifest.");
        using var client = CreateClient(password); client.Connect(); EnsureDirectoryTree(client, "/updates");
        var backup = BackupRoot + "/update_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmss_fff");
        EnsureDirectoryTree(client, backup + "/original");
        EnsureDirectoryTree(client, backup + "/staged");
        var targets = new[] { new PublishTarget(packagePath, "/updates/" + packageName, feed.PackageBytes), new PublishTarget(fullFeed, "/updates/latest.json", new FileInfo(fullFeed).Length) };
        var originals = new Dictionary<string, string>(StringComparer.Ordinal);
        var activated = new List<string>();
        try
        {
            foreach (var target in targets)
            {
                var key = target.RemotePath.Trim('/').Replace('/', '_');
                if (client.FileExists(target.RemotePath))
                {
                    var original = backup + "/original/" + key;
                    using var stream = new MemoryStream();
                    if (!client.DownloadStream(stream, target.RemotePath)) throw new IOException("Could not retain remote update backup: " + target.RemotePath);
                    stream.Position = 0;
                    if (client.UploadStream(stream, original, FtpRemoteExists.Overwrite, false) != FtpStatus.Success) throw new IOException("Could not upload remote update backup: " + target.RemotePath);
                    originals[target.RemotePath] = original;
                }
                var staged = backup + "/staged/" + key;
                if (client.UploadFile(target.LocalPath, staged, FtpRemoteExists.Overwrite, false, FtpVerify.None) != FtpStatus.Success || client.GetFileSize(staged) != target.Bytes)
                    throw new IOException("Update staging validation failed: " + target.RemotePath);
                if (!client.MoveFile(staged, target.RemotePath, FtpRemoteExists.Overwrite)) throw new IOException("Update activation failed: " + target.RemotePath);
                activated.Add(target.RemotePath);
            }
            client.Disconnect(); return new(backup, activated);
        }
        catch
        {
            foreach (var remotePath in activated.AsEnumerable().Reverse())
            {
                try
                {
                    if (originals.TryGetValue(remotePath, out var original))
                    {
                        using var stream = new MemoryStream();
                        if (client.DownloadStream(stream, original))
                        {
                            stream.Position = 0;
                            client.UploadStream(stream, remotePath, FtpRemoteExists.Overwrite, false);
                        }
                    }
                    else if (client.FileExists(remotePath)) client.DeleteFile(remotePath);
                }
                catch { }
            }
            throw;
        }
    }

    public static ApplicationDeploymentVerificationResult RunContractVerification()
    {
        var root = Path.Combine(Path.GetTempPath(), "3DPIceland-DeploymentVerify-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var files = new[] { ("Installer", "setup.exe", StablePaths["Installer"]), ("Portable", "portable.zip", StablePaths["Portable"]) };
            var plan = new ApplicationDeploymentPlan { ReleaseVersion = "1.0.0", ReleaseCode = "DEPLOYMENT-TEST", SourcePackageSha256 = new string('A', 64) };
            foreach (var item in files)
            {
                var path = Path.Combine(root, item.Item2); File.WriteAllText(path, item.Item1);
                plan.Files.Add(new ApplicationDeploymentFile { Kind = item.Item1, LocalFile = item.Item2, StableRemotePath = item.Item3,
                    VersionedRemotePath = DownloadsRoot + "/" + item.Item2, Bytes = new FileInfo(path).Length, Sha256 = Convert.ToHexString(SHA256.HashData(File.ReadAllBytes(path))) });
            }
            var planPath = Path.Combine(root, PlanFileName); File.WriteAllText(planPath, JsonSerializer.Serialize(plan));
            _ = new ApplicationDeploymentService(new DeploymentSettingsRecord()).LoadAndVerify(planPath);
            plan.Files[0].StableRemotePath = "/index.html"; File.WriteAllText(planPath, JsonSerializer.Serialize(plan));
            try { _ = new ApplicationDeploymentService(new DeploymentSettingsRecord()).LoadAndVerify(planPath); return new(false, "Unsafe website deployment route was accepted."); }
            catch (InvalidOperationException) { }
            return new(true, "Exact installer/portable allowlist, local bytes/SHA-256, versioned downloads and stable-route-last separation passed; website routes blocked.");
        }
        catch (Exception ex) { return new(false, ex.Message); }
        finally { try { Directory.Delete(root, true); } catch { } }
    }

    private FtpClient CreateClient(string password)
    {
        var client = new FtpClient(_settings.FtpsHost.Trim(), _settings.FtpsUserName.Trim(), password, _settings.FtpsPort);
        client.Config.EncryptionMode = FtpEncryptionMode.Explicit; client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
        client.Config.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13; client.Config.ConnectTimeout = 20000; client.Config.DataConnectionConnectTimeout = 20000;
        client.ValidateCertificate += (_, args) => args.Accept = args.PolicyErrors == System.Net.Security.SslPolicyErrors.None;
        return client;
    }
    private static void EnsureDirectoryTree(FtpClient client, string path)
    {
        var current = string.Empty;
        foreach (var part in path.Split('/', StringSplitOptions.RemoveEmptyEntries)) { current += "/" + part; if (!client.DirectoryExists(current) && !client.CreateDirectory(current)) throw new IOException("Could not create " + current); }
    }
    private sealed record PublishTarget(string LocalPath, string RemotePath, long Bytes);
}
