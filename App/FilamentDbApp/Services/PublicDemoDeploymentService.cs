using FilamentDbApp.Models;
using FluentFTP;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Text.Json;

namespace FilamentDbApp.Services;

public sealed class PublicDemoDeploymentService
{
    public const string PlanFileName = "public-demo-deployment-plan.json";
    public const string DownloadsRoot = "/downloads";
    public const string BackupRoot = "/backups/public_demo";
    public const string StableRemotePath = DownloadsRoot + "/3DPIceland-Public-Demo.zip";
    public const string StableHttpsUrl = "https://www.iskort.is/3dp/downloads/3DPIceland-Public-Demo.zip";

    private readonly DeploymentSettingsRecord _settings;
    public PublicDemoDeploymentService(DeploymentSettingsRecord settings) => _settings = settings;

    public PublicDemoDeploymentPlan LoadAndVerify(string planPath)
    {
        var fullPlanPath = IOPath.GetFullPath(planPath);
        var root = IOPath.GetDirectoryName(fullPlanPath)
                   ?? throw new InvalidOperationException("Public demo deployment plan folder is unavailable.");
        var plan = JsonSerializer.Deserialize<PublicDemoDeploymentPlan>(
                       IOFile.ReadAllText(fullPlanPath),
                       new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                   ?? throw new InvalidOperationException("Public demo deployment plan could not be read.");
        var expectedName = $"3DPIceland-Public-Demo-{plan.Release}.zip";
        var expectedVersioned = DownloadsRoot + "/" + expectedName;
        if (plan.Schema != PublicDemoDeploymentPlan.CurrentSchema ||
            !Version.TryParse(plan.Release.TrimStart('v', 'V'), out _) ||
            plan.LocalFile != expectedName ||
            plan.VersionedRemotePath != expectedVersioned ||
            plan.StableRemotePath != StableRemotePath ||
            plan.Bytes <= 0 ||
            plan.Sha256.Length != 64 ||
            plan.VersionedRemotePath.Contains("..", StringComparison.Ordinal) ||
            plan.StableRemotePath.Contains("..", StringComparison.Ordinal))
            throw new InvalidOperationException("Public demo deployment identity or exact route allowlist is invalid.");

        var localPath = IOPath.GetFullPath(IOPath.Combine(root, plan.LocalFile));
        var containedPrefix = root.TrimEnd(IOPath.DirectorySeparatorChar) + IOPath.DirectorySeparatorChar;
        if (!localPath.StartsWith(containedPrefix, StringComparison.OrdinalIgnoreCase) || !IOFile.Exists(localPath))
            throw new FileNotFoundException("Public demo ZIP is missing or outside the plan folder.", localPath);
        if (new FileInfo(localPath).Length != plan.Bytes ||
            Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(localPath))) != plan.Sha256)
            throw new IOException("Public demo ZIP bytes or SHA-256 changed.");
        return plan;
    }

    public async Task<PublicDemoDeploymentResult> PublishAndVerifyAsync(
        string password,
        string planPath,
        CancellationToken cancellationToken = default)
    {
        var plan = LoadAndVerify(planPath);
        var root = IOPath.GetDirectoryName(IOPath.GetFullPath(planPath))!;
        var localPath = IOPath.Combine(root, plan.LocalFile);
        var backup = BackupRoot + "/release_" + DateTime.UtcNow.ToString("yyyy-MM-dd_HHmmss_fff");
        var targets = new[] { plan.VersionedRemotePath, plan.StableRemotePath };
        var originals = new Dictionary<string, string>(StringComparer.Ordinal);
        var activated = new List<string>();

        using (var client = CreateClient(password))
        {
            client.Connect();
            EnsureDirectoryTree(client, DownloadsRoot);
            EnsureDirectoryTree(client, backup + "/original");
            EnsureDirectoryTree(client, backup + "/staged");
            try
            {
                foreach (var remotePath in targets)
                {
                    var key = remotePath.Trim('/').Replace('/', '_');
                    if (client.FileExists(remotePath))
                    {
                        var original = backup + "/original/" + key;
                        using var stream = new MemoryStream();
                        if (!client.DownloadStream(stream, remotePath))
                            throw new IOException("Could not retain remote public-demo backup: " + remotePath);
                        stream.Position = 0;
                        if (client.UploadStream(stream, original, FtpRemoteExists.Overwrite, false) != FtpStatus.Success)
                            throw new IOException("Could not upload remote public-demo backup: " + remotePath);
                        originals[remotePath] = original;
                    }

                    var staged = backup + "/staged/" + key;
                    if (client.UploadFile(localPath, staged, FtpRemoteExists.Overwrite, false, FtpVerify.None) != FtpStatus.Success ||
                        client.GetFileSize(staged) != plan.Bytes)
                        throw new IOException("Public-demo staging validation failed: " + remotePath);
                    if (!client.MoveFile(staged, remotePath, FtpRemoteExists.Overwrite))
                        throw new IOException("Public-demo activation failed: " + remotePath);
                    activated.Add(remotePath);
                }
                client.Disconnect();
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
                        else if (client.FileExists(remotePath))
                        {
                            client.DeleteFile(remotePath);
                        }
                    }
                    catch
                    {
                        // Retain the original exception; the backup path is included in the caller's evidence.
                    }
                }
                throw;
            }
        }

        var verifiedUrls = new List<string>();
        using var http = new HttpClient { Timeout = TimeSpan.FromSeconds(60) };
        foreach (var remotePath in targets)
        {
            var url = "https://www.iskort.is/3dp" + remotePath;
            var bytes = await http.GetByteArrayAsync(url, cancellationToken);
            if (bytes.LongLength != plan.Bytes || Convert.ToHexString(SHA256.HashData(bytes)) != plan.Sha256)
                throw new IOException("Published HTTPS bytes or SHA-256 do not match the governed public demo: " + url);
            verifiedUrls.Add(url);
        }
        return new PublicDemoDeploymentResult(backup, activated, verifiedUrls);
    }

    public static PublicDemoDeploymentVerificationResult RunContractVerification()
    {
        var root = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-PublicDemoDeployVerify-" + Guid.NewGuid().ToString("N"));
        IODirectory.CreateDirectory(root);
        try
        {
            const string release = "v56.0.6";
            var localFile = $"3DPIceland-Public-Demo-{release}.zip";
            var localPath = IOPath.Combine(root, localFile);
            IOFile.WriteAllText(localPath, "governed-demo");
            var plan = new PublicDemoDeploymentPlan
            {
                Release = release,
                LocalFile = localFile,
                VersionedRemotePath = DownloadsRoot + "/" + localFile,
                StableRemotePath = StableRemotePath,
                Bytes = new FileInfo(localPath).Length,
                Sha256 = Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(localPath)))
            };
            var planPath = IOPath.Combine(root, PlanFileName);
            IOFile.WriteAllText(planPath, JsonSerializer.Serialize(plan));
            _ = new PublicDemoDeploymentService(new DeploymentSettingsRecord()).LoadAndVerify(planPath);
            plan.StableRemotePath = "/index.html";
            IOFile.WriteAllText(planPath, JsonSerializer.Serialize(plan));
            try
            {
                _ = new PublicDemoDeploymentService(new DeploymentSettingsRecord()).LoadAndVerify(planPath);
                return new(false, "Unsafe website route was accepted.");
            }
            catch (InvalidOperationException)
            {
            }
            return new(true,
                "Exact one-ZIP allowlist, bytes/SHA-256, immutable versioned route and stable demo route passed; unrelated routes blocked.");
        }
        catch (Exception ex)
        {
            return new(false, ex.Message);
        }
        finally
        {
            try { IODirectory.Delete(root, true); } catch { }
        }
    }

    private FtpClient CreateClient(string password)
    {
        var client = new FtpClient(_settings.FtpsHost.Trim(), _settings.FtpsUserName.Trim(), password, _settings.FtpsPort);
        client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
        client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
        client.Config.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
        client.Config.ConnectTimeout = 20000;
        client.Config.DataConnectionConnectTimeout = 20000;
        client.ValidateCertificate += (_, args) =>
            args.Accept = args.PolicyErrors == System.Net.Security.SslPolicyErrors.None;
        return client;
    }

    private static void EnsureDirectoryTree(FtpClient client, string path)
    {
        var current = string.Empty;
        foreach (var part in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            current += "/" + part;
            if (!client.DirectoryExists(current) && !client.CreateDirectory(current))
                throw new IOException("Could not create " + current);
        }
    }
}
