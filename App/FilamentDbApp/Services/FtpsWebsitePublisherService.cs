using FluentFTP;
using FilamentDbApp.Models;
using FilamentDbApp.Services.Website;
using System.Collections.Concurrent;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Security.Authentication;
using System.Text;
using System.Text.Json;

namespace FilamentDbApp.Services;

public sealed class FtpsWebsitePublisherService
{
    public const int DefaultPort = 21;
    public const string BackupRoot = "/backups";
    public const string MainRemotePath = "/index.html";
    public const string ManufacturerRemotePath = "/manufacturers/index.html";
    public const string BackupManifestFileName = "deployment-backup-manifest.json";
    public const string BackupManifestSchema = "3dpiceland.ftps-deployment-backup.v1";
    public const int ParallelTransferWorkers = 1;
    public const int LocalValidationWorkers = 4;
    public const int TransferRetryAttempts = 3;
    public const bool DeltaPublishingEnabled = true;

    public string Host { get; }
    public int Port { get; }
    public string UserName { get; }

    public FtpsWebsitePublisherService(DeploymentSettingsRecord? settings = null)
    {
        settings ??= new DeploymentSettingsRecord();
        Host = settings.FtpsHost.Trim();
        Port = settings.FtpsPort;
        UserName = settings.FtpsUserName.Trim();
        if (string.IsNullOrWhiteSpace(Host)) throw new ArgumentException("FTPS host is required.");
        if (Port is < 1 or > 65535) throw new ArgumentOutOfRangeException(nameof(settings), "FTPS port must be between 1 and 65535.");
        if (string.IsNullOrWhiteSpace(UserName)) throw new ArgumentException("FTPS username is required.");
    }

    public sealed record ConnectionProbe(bool Connected, string CertificateSubject, string Message);
    public sealed record PublishFile(string LocalPath, string RemotePath);
    public sealed record PublishResult(string BackupFolder, IReadOnlyList<string> PublishedFiles);
    public sealed record PublishProgress(string Phase, int Completed, int Total, string RemotePath);
    public sealed record PlanPublishResult(string BackupFolder, string BaselineFolder, int PlannedFiles, int UnchangedFiles, int StagedFiles, int ActivatedFiles, long TotalBytes, int TransferWorkers, double StagingSeconds, double ActivationSeconds, bool DeltaBaselineUsed);
    public sealed record ProductionBackupSummary(string BackupFolder, string CreatedAtUtc, int Files, long PreviousBytes);
    public sealed record RestoreResult(string SourceBackupFolder, string RecoveryBackupFolder, int RestoredFiles, int RemovedFiles, long RestoredBytes);

    public ConnectionProbe TestConnection(string password)
    {
        string certificateSubject = string.Empty;
        using var client = CreateClient(password, subject => certificateSubject = subject);
        try
        {
            ConnectWithRetry(client);
            var connected = client.IsConnected;
            if (connected) DisconnectBestEffort(client);
            return new ConnectionProbe(connected, certificateSubject, connected ? "Explicit FTPS connection succeeded." : "FTPS connection did not complete.");
        }
        catch (Exception ex)
        {
            return new ConnectionProbe(false, certificateSubject, ex.Message);
        }
    }

    public PublishResult Publish(string password, IReadOnlyList<PublishFile> files)
    {
        using var client = CreateClient(password, _ => { });
        client.Connect();
        EnsureDirectory(client, BackupRoot);
        EnsureDirectory(client, "/manufacturers");
        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
        var backupFolder = $"{BackupRoot}/website_{stamp}";
        EnsureDirectory(client, backupFolder);

        var temporaryPaths = new Dictionary<string, string>(StringComparer.Ordinal);
        var backupPaths = new Dictionary<string, string>(StringComparer.Ordinal);
        var replacedPaths = new List<string>();
        try
        {
            foreach (var file in files)
            {
                if (!File.Exists(file.LocalPath)) throw new FileNotFoundException("A production export file is missing.", file.LocalPath);
                if (client.FileExists(file.RemotePath))
                {
                    var backupPath = $"{backupFolder}/{file.RemotePath.Trim('/').Replace('/', '_')}";
                    using var backup = new MemoryStream();
                    if (!client.DownloadStream(backup, file.RemotePath)) throw new IOException($"Could not download remote backup source {file.RemotePath}.");
                    backup.Position = 0;
                    var status = client.UploadStream(backup, backupPath, FtpRemoteExists.NoCheck, createRemoteDir: false);
                    if (status != FtpStatus.Success || client.GetFileSize(backupPath) != backup.Length) throw new IOException($"Remote backup validation failed for {file.RemotePath}.");
                    backupPaths[file.RemotePath] = backupPath;
                }

                var temporaryPath = file.RemotePath + ".uploading-" + Guid.NewGuid().ToString("N");
                var uploadStatus = client.UploadFile(file.LocalPath, temporaryPath, FtpRemoteExists.NoCheck, createRemoteDir: false, FtpVerify.None);
                if (uploadStatus != FtpStatus.Success || client.GetFileSize(temporaryPath) != new FileInfo(file.LocalPath).Length) throw new IOException($"Uploaded size validation failed for {file.RemotePath}.");
                temporaryPaths[file.RemotePath] = temporaryPath;
            }

            foreach (var file in files)
            {
                replacedPaths.Add(file.RemotePath);
                if (!client.MoveFile(temporaryPaths[file.RemotePath], file.RemotePath, FtpRemoteExists.Overwrite)) throw new IOException($"Could not activate {file.RemotePath}.");
                temporaryPaths.Remove(file.RemotePath);
            }

            DisconnectBestEffort(client);
            return new PublishResult(backupFolder, files.Select(file => file.RemotePath).ToList());
        }
        catch
        {
            foreach (var remotePath in replacedPaths.AsEnumerable().Reverse())
            {
                try
                {
                    if (!backupPaths.TryGetValue(remotePath, out var backupPath)) continue;
                    using var original = new MemoryStream();
                    if (!client.DownloadStream(original, backupPath)) continue;
                    original.Position = 0;
                    client.UploadStream(original, remotePath, FtpRemoteExists.Overwrite, createRemoteDir: false);
                }
                catch { }
            }
            throw;
        }
        finally
        {
            foreach (var temporaryPath in temporaryPaths.Values)
            {
                try { if (client.IsConnected && client.FileExists(temporaryPath)) client.DeleteFile(temporaryPath); } catch { }
            }
        }
    }

    public PlanPublishResult PublishPlan(
        string password,
        WebsiteProductionPublishPlan plan,
        IProgress<PublishProgress>? progress = null)
    {
        if (!new WebsiteProductionPublishPlanService().VerifyForPublish(plan))
            throw new InvalidOperationException("The guarded publish plan schema, release identity, allowlist or activation order is invalid.");

        ValidateLocalArtifacts(plan.Files, progress);
        using var client = CreateClient(password, _ => { });
        ConnectWithRetry(client);
        ExecuteControlOperationWithRetry(client, () => EnsureDirectoryTree(client, BackupRoot));
        var deploymentMode = string.Equals(plan.Schema, WebsiteProductionPublishPlanService.TestSchema, StringComparison.Ordinal) ? "Test" : "Production";
        var deltaSelection = ResolveDeltaPublishSelection(client, deploymentMode, plan, progress);
        var publishFiles = deltaSelection.Files;
        if (publishFiles.Count == 0)
        {
            DisconnectBestEffort(client);
            return new PlanPublishResult(string.Empty, deltaSelection.BaselineFolder, plan.Files.Count, plan.Files.Count, 0, 0, 0, 1, 0, 0, true);
        }

        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss_fff");
        var mode = string.Equals(deploymentMode, "Test", StringComparison.Ordinal) ? "website_test" : "website";
        var backupFolder = $"{BackupRoot}/{mode}_{stamp}";
        var originalRoot = backupFolder + "/original";
        var stagedRoot = backupFolder + "/staged";
        ExecuteControlOperationWithRetry(client, () => EnsureDirectoryTree(client, originalRoot));
        ExecuteControlOperationWithRetry(client, () => EnsureDirectoryTree(client, stagedRoot));

        var backupPaths = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        var stagedPaths = new ConcurrentDictionary<string, string>(StringComparer.Ordinal);
        var preparedTransferDirectories = new ConcurrentDictionary<string, byte>(StringComparer.Ordinal);
        var directoryCreationGate = new object();
        var activated = new List<string>();
        var manifestEntrySlots = new DeploymentBackupEntry?[publishFiles.Count];
        var transferWorkers = Math.Min(ParallelTransferWorkers, publishFiles.Count);
        var stagingSeconds = 0.0;
        var activationSeconds = 0.0;
        var publishPhase = "host-compatible backup and staging";
        DeploymentBackupManifest? backupManifest = null;
        try
        {
            var stagingTimer = Stopwatch.StartNew();
            DisconnectForParallelWorkers(client);

            var errors = new ConcurrentQueue<Exception>();
            using var cancellation = new CancellationTokenSource();
            var completed = 0;
            var batches = BuildTransferBatches(publishFiles, transferWorkers);
            Parallel.ForEach(
                batches,
                new ParallelOptions { MaxDegreeOfParallelism = transferWorkers },
                batch =>
                {
                    using var worker = CreateClient(password, _ => { });
                    try
                    {
                        foreach (var work in batch)
                        {
                            if (cancellation.IsCancellationRequested) break;
                            var file = work.File;
                            Exception? transferError = null;
                            for (var attempt = 1; attempt <= TransferRetryAttempts; attempt++)
                            {
                                try
                                {
                                    if (!worker.IsConnected) worker.Connect();
                                    ValidateLocalArtifact(file);

                                    var originalBytes = 0L;
                                    var originalSha256 = string.Empty;
                                    var originalExisted = worker.FileExists(file.RemotePath);
                                    if (originalExisted)
                                    {
                                        var backupPath = originalRoot + file.RemotePath;
                                        using var original = new MemoryStream();
                                        if (!worker.DownloadStream(original, file.RemotePath)) throw new IOException($"Could not download remote backup source {file.RemotePath}.");
                                        original.Position = 0;
                                        EnsureTransferParentDirectory(worker, backupPath, directoryCreationGate, preparedTransferDirectories);
                                        var backupStatus = worker.UploadStream(original, backupPath, FtpRemoteExists.Overwrite, createRemoteDir: false);
                                        if (backupStatus != FtpStatus.Success || worker.GetFileSize(backupPath) != original.Length)
                                            throw new IOException($"Remote backup validation failed for {file.RemotePath}.");
                                        backupPaths[file.RemotePath] = backupPath;
                                        originalBytes = original.Length;
                                        originalSha256 = ComputeSha256(original);
                                    }

                                    var stagedPath = stagedRoot + file.RemotePath;
                                    stagedPaths[file.RemotePath] = stagedPath;
                                    EnsureTransferParentDirectory(worker, stagedPath, directoryCreationGate, preparedTransferDirectories);
                                    var uploadStatus = worker.UploadFile(file.LocalPath, stagedPath, FtpRemoteExists.Overwrite, createRemoteDir: false, FtpVerify.None);
                                    if (uploadStatus != FtpStatus.Success || worker.GetFileSize(stagedPath) != file.Bytes)
                                        throw new IOException($"Staged upload size validation failed for {file.RemotePath}.");
                                    manifestEntrySlots[work.Index] = new DeploymentBackupEntry
                                    {
                                        RemotePath = file.RemotePath,
                                        OriginalExisted = originalExisted,
                                        OriginalBytes = originalBytes,
                                        OriginalSha256 = originalSha256,
                                        PublishedBytes = file.Bytes
                                    };
                                    var completedNow = Interlocked.Increment(ref completed);
                                    progress?.Report(new PublishProgress("Host-compatible backup and staging", completedNow, publishFiles.Count, file.RemotePath));
                                    transferError = null;
                                    break;
                                }
                                catch (Exception ex)
                                {
                                    transferError = ex;
                                    try { if (worker.IsConnected) worker.Disconnect(); } catch { }
                                    if (attempt < TransferRetryAttempts) Thread.Sleep(250 * attempt);
                                }
                            }
                            if (transferError is not null)
                                throw new IOException($"FTPS staging failed after {TransferRetryAttempts} attempts for {file.RemotePath}: {transferError.Message}", transferError);
                        }
                    }
                    catch (Exception ex)
                    {
                        errors.Enqueue(ex);
                        cancellation.Cancel();
                    }
                    finally
                    {
                        try { if (worker.IsConnected) worker.Disconnect(); } catch { }
                    }
                });

            if (!errors.IsEmpty)
                throw new InvalidOperationException("Host-compatible FTPS backup/staging failed: " + string.Join(" | ", errors.Select(error => error.Message)));
            if (manifestEntrySlots.Any(entry => entry is null) || stagedPaths.Count != publishFiles.Count)
                throw new InvalidOperationException("Host-compatible FTPS staging ended before every allowlisted artifact completed.");

            ConnectWithRetry(client);
            stagingTimer.Stop();
            stagingSeconds = stagingTimer.Elapsed.TotalSeconds;
            var manifestEntries = manifestEntrySlots.Select(entry => entry!).ToList();
            backupManifest = new DeploymentBackupManifest
            {
                Schema = BackupManifestSchema,
                Mode = deploymentMode,
                CreatedAtUtc = DateTime.UtcNow.ToString("O"),
                SourcePlanVersion = plan.Version,
                Files = manifestEntries,
                PublishedArtifacts = plan.Files.Select(file => new DeploymentPublishedArtifact
                {
                    RemotePath = file.RemotePath,
                    Bytes = file.Bytes,
                    Sha256 = file.Sha256
                }).ToList()
            };
            publishPhase = "initial backup manifest";
            ExecuteControlOperationWithRetry(client, () => UploadManifest(client, backupFolder + "/" + BackupManifestFileName, backupManifest));

            publishPhase = "sequential activation";
            var activationTimer = Stopwatch.StartNew();
            for (var index = 0; index < publishFiles.Count; index++)
            {
                var file = publishFiles[index];
                progress?.Report(new PublishProgress("Activating", index, publishFiles.Count, file.RemotePath));
                activated.Add(file.RemotePath);
                ActivateStagedFileWithRetry(client, stagedPaths[file.RemotePath], file.RemotePath, file.Bytes);
                stagedPaths.TryRemove(file.RemotePath, out _);
                progress?.Report(new PublishProgress("Activating", index + 1, publishFiles.Count, file.RemotePath));
            }
            activationTimer.Stop();
            activationSeconds = activationTimer.Elapsed.TotalSeconds;

            backupManifest.DeploymentCompleted = true;
            publishPhase = "completed backup manifest";
            ExecuteControlOperationWithRetry(client, () => UploadManifest(client, backupFolder + "/" + BackupManifestFileName, backupManifest));

            publishPhase = "disconnect";
            DisconnectBestEffort(client);
            return new PlanPublishResult(
                backupFolder,
                deltaSelection.BaselineFolder,
                plan.Files.Count,
                plan.Files.Count - publishFiles.Count,
                publishFiles.Count,
                activated.Count,
                publishFiles.Sum(file => file.Bytes),
                transferWorkers,
                stagingSeconds,
                activationSeconds,
                deltaSelection.BaselineUsed);
        }
        catch (Exception publishError)
        {
            var rollbackErrors = new List<string>();
            try
            {
                if (!client.IsConnected) client.Connect();
            }
            catch (Exception ex)
            {
                rollbackErrors.Add("Could not reconnect for rollback: " + ex.Message);
            }

            if (client.IsConnected)
            {
                for (var index = activated.Count - 1; index >= 0; index--)
                {
                    var remotePath = activated[index];
                    progress?.Report(new PublishProgress("Rolling back", activated.Count - 1 - index, activated.Count, remotePath));
                    try
                    {
                        if (backupPaths.TryGetValue(remotePath, out var backupPath))
                        {
                            using var original = new MemoryStream();
                            if (!client.DownloadStream(original, backupPath)) throw new IOException("Could not read remote backup.");
                            original.Position = 0;
                            var restoreStatus = client.UploadStream(original, remotePath, FtpRemoteExists.Overwrite, createRemoteDir: false);
                            if (restoreStatus != FtpStatus.Success || client.GetFileSize(remotePath) != original.Length)
                                throw new IOException("Restored file size validation failed.");
                        }
                        else if (client.FileExists(remotePath))
                        {
                            client.DeleteFile(remotePath);
                        }
                    }
                    catch (Exception ex)
                    {
                        rollbackErrors.Add(remotePath + ": " + ex.Message);
                    }
                }
            }

            if (backupManifest is not null)
            {
                try
                {
                    backupManifest.DeploymentCompleted = false;
                    ExecuteControlOperationWithRetry(client, () => UploadManifest(client, backupFolder + "/" + BackupManifestFileName, backupManifest));
                }
                catch (Exception ex)
                {
                    rollbackErrors.Add("Could not invalidate the failed deployment manifest: " + ex.Message);
                }
            }

            if (rollbackErrors.Count > 0)
                throw new InvalidOperationException(
                    $"FTPS publish failed during {publishPhase} and rollback was incomplete. Remote backup: {backupFolder}. Publish error: {publishError.Message}. Rollback errors: {string.Join(" | ", rollbackErrors)}",
                    publishError);
            throw new InvalidOperationException(
                $"FTPS publish failed during {publishPhase}; all activated files were rolled back. Remote backup: {backupFolder}. {publishError.Message}",
                publishError);
        }
        finally
        {
            if (client.IsConnected)
            {
                foreach (var stagedPath in stagedPaths.Values)
                {
                    try { if (client.FileExists(stagedPath)) client.DeleteFile(stagedPath); } catch { }
                }
            }
        }
    }

    public ProductionBackupSummary GetLatestProductionBackup(string password)
    {
        using var client = CreateClient(password, _ => { });
        client.Connect();
        if (!client.DirectoryExists(BackupRoot))
            throw new InvalidOperationException("No remote website backups are available.");

        var candidates = client.GetListing(BackupRoot)
            .Where(item => item.Type == FtpObjectType.Directory && IsProductionBackupFolderName(item.Name))
            .OrderByDescending(item => item.Name, StringComparer.Ordinal)
            .ToList();
        foreach (var candidate in candidates)
        {
            var folder = BackupRoot + "/" + candidate.Name;
            var manifestPath = folder + "/" + BackupManifestFileName;
            if (!client.FileExists(manifestPath)) continue;
            var manifest = DownloadManifest(client, manifestPath);
            if (!manifest.DeploymentCompleted || !string.Equals(manifest.Mode, "Production", StringComparison.Ordinal)) continue;
            ValidateManifest(manifest, "Production");
            DisconnectBestEffort(client);
            return new ProductionBackupSummary(
                folder,
                manifest.CreatedAtUtc,
                manifest.Files.Count,
                manifest.Files.Where(entry => entry.OriginalExisted).Sum(entry => entry.OriginalBytes));
        }

        throw new InvalidOperationException("No restorable Production backup with a deployment manifest was found. Legacy or Website Test backups are not eligible.");
    }

    public RestoreResult RestoreProductionBackup(
        string password,
        string backupFolder,
        IProgress<PublishProgress>? progress = null)
    {
        if (!IsProductionBackupFolderPath(backupFolder))
            throw new InvalidOperationException("The selected restore source is not an approved Production backup folder.");

        using var client = CreateClient(password, _ => { });
        client.Connect();
        var sourceManifestPath = backupFolder + "/" + BackupManifestFileName;
        if (!client.FileExists(sourceManifestPath)) throw new FileNotFoundException("The selected Production backup manifest is missing.", sourceManifestPath);
        var sourceManifest = DownloadManifest(client, sourceManifestPath);
        ValidateManifest(sourceManifest, "Production");

        var stamp = DateTime.Now.ToString("yyyy-MM-dd_HHmmss_fff");
        var recoveryFolder = $"{BackupRoot}/website_revert_{stamp}";
        var recoveryOriginalRoot = recoveryFolder + "/original";
        var restoreStagedRoot = recoveryFolder + "/staged";
        EnsureDirectoryTree(client, recoveryOriginalRoot);
        EnsureDirectoryTree(client, restoreStagedRoot);

        var recoveryPaths = new Dictionary<string, string>(StringComparer.Ordinal);
        var recoveryHashes = new Dictionary<string, string>(StringComparer.Ordinal);
        var stagedPaths = new Dictionary<string, string>(StringComparer.Ordinal);
        var activated = new List<string>();
        var recoveryEntries = new List<DeploymentBackupEntry>();
        try
        {
            for (var index = 0; index < sourceManifest.Files.Count; index++)
            {
                var entry = sourceManifest.Files[index];
                progress?.Report(new PublishProgress("Preparing restore", index, sourceManifest.Files.Count, entry.RemotePath));

                var currentExisted = client.FileExists(entry.RemotePath);
                var currentBytes = 0L;
                if (currentExisted)
                {
                    var recoveryPath = recoveryOriginalRoot + entry.RemotePath;
                    EnsureParentDirectory(client, recoveryPath);
                    using var current = new MemoryStream();
                    if (!client.DownloadStream(current, entry.RemotePath)) throw new IOException("Could not read the current live file: " + entry.RemotePath);
                    current.Position = 0;
                    UploadStreamChecked(client, current, recoveryPath, "Current-live recovery backup validation failed for " + entry.RemotePath);
                    recoveryPaths[entry.RemotePath] = recoveryPath;
                    currentBytes = current.Length;
                    recoveryHashes[entry.RemotePath] = ComputeSha256(current);
                }

                if (entry.OriginalExisted)
                {
                    var sourcePath = backupFolder + "/original" + entry.RemotePath;
                    if (!client.FileExists(sourcePath) || client.GetFileSize(sourcePath) != entry.OriginalBytes)
                        throw new IOException("The selected backup artifact is missing or changed: " + sourcePath);
                    var stagedPath = restoreStagedRoot + entry.RemotePath;
                    EnsureParentDirectory(client, stagedPath);
                    using var restoreSource = new MemoryStream();
                    if (!client.DownloadStream(restoreSource, sourcePath)) throw new IOException("Could not read selected backup artifact: " + sourcePath);
                    if (!string.Equals(ComputeSha256(restoreSource), entry.OriginalSha256, StringComparison.Ordinal))
                        throw new IOException("Selected backup artifact hash validation failed: " + sourcePath);
                    restoreSource.Position = 0;
                    UploadStreamChecked(client, restoreSource, stagedPath, "Restore staging validation failed for " + entry.RemotePath);
                    stagedPaths[entry.RemotePath] = stagedPath;
                }

                recoveryEntries.Add(new DeploymentBackupEntry
                {
                    RemotePath = entry.RemotePath,
                    OriginalExisted = currentExisted,
                    OriginalBytes = currentBytes,
                    OriginalSha256 = currentExisted ? recoveryHashes[entry.RemotePath] : string.Empty,
                    PublishedBytes = entry.OriginalExisted ? entry.OriginalBytes : 0
                });
                progress?.Report(new PublishProgress("Preparing restore", index + 1, sourceManifest.Files.Count, entry.RemotePath));
            }

            UploadManifest(client, recoveryFolder + "/" + BackupManifestFileName, new DeploymentBackupManifest
            {
                Schema = BackupManifestSchema,
                Mode = "RevertRecovery",
                CreatedAtUtc = DateTime.UtcNow.ToString("O"),
                SourcePlanVersion = sourceManifest.SourcePlanVersion,
                DeploymentCompleted = true,
                Files = recoveryEntries
            });

            var restored = 0;
            var removed = 0;
            for (var index = 0; index < sourceManifest.Files.Count; index++)
            {
                var entry = sourceManifest.Files[index];
                progress?.Report(new PublishProgress("Restoring", index, sourceManifest.Files.Count, entry.RemotePath));
                activated.Add(entry.RemotePath);
                if (entry.OriginalExisted)
                {
                    ActivateStagedFileWithRetry(client, stagedPaths[entry.RemotePath], entry.RemotePath, entry.OriginalBytes);
                    stagedPaths.Remove(entry.RemotePath);
                    restored++;
                }
                else
                {
                    DeleteRemoteFileWithRetry(client, entry.RemotePath);
                    removed++;
                }
                progress?.Report(new PublishProgress("Restoring", index + 1, sourceManifest.Files.Count, entry.RemotePath));
            }

            DisconnectBestEffort(client);
            return new RestoreResult(
                backupFolder,
                recoveryFolder,
                restored,
                removed,
                sourceManifest.Files.Where(entry => entry.OriginalExisted).Sum(entry => entry.OriginalBytes));
        }
        catch (Exception restoreError)
        {
            var rollbackErrors = RollBackActivatedFiles(client, activated, recoveryPaths, recoveryHashes, progress);
            if (rollbackErrors.Count > 0)
                throw new InvalidOperationException(
                    $"Production restore failed and recovery rollback was incomplete. Recovery backup: {recoveryFolder}. Restore error: {restoreError.Message}. Rollback errors: {string.Join(" | ", rollbackErrors)}",
                    restoreError);
            throw new InvalidOperationException(
                $"Production restore failed; all changed live files were recovered. Recovery backup: {recoveryFolder}. {restoreError.Message}",
                restoreError);
        }
        finally
        {
            if (client.IsConnected)
            {
                foreach (var stagedPath in stagedPaths.Values)
                {
                    try { if (client.FileExists(stagedPath)) client.DeleteFile(stagedPath); } catch { }
                }
            }
        }
    }

    private static List<string> RollBackActivatedFiles(
        FtpClient client,
        IReadOnlyList<string> activated,
        IReadOnlyDictionary<string, string> recoveryPaths,
        IReadOnlyDictionary<string, string> recoveryHashes,
        IProgress<PublishProgress>? progress)
    {
        var errors = new List<string>();
        try
        {
            if (!client.IsConnected) client.Connect();
        }
        catch (Exception ex)
        {
            errors.Add("Could not reconnect for recovery rollback: " + ex.Message);
            return errors;
        }

        for (var index = activated.Count - 1; index >= 0; index--)
        {
            var remotePath = activated[index];
            progress?.Report(new PublishProgress("Recovering failed restore", activated.Count - 1 - index, activated.Count, remotePath));
            try
            {
                if (recoveryPaths.TryGetValue(remotePath, out var recoveryPath))
                {
                    using var current = new MemoryStream();
                    if (!client.DownloadStream(current, recoveryPath)) throw new IOException("Could not read recovery backup.");
                    if (!recoveryHashes.TryGetValue(remotePath, out var expectedHash) ||
                        !string.Equals(ComputeSha256(current), expectedHash, StringComparison.Ordinal))
                        throw new IOException("Recovery backup hash validation failed.");
                    current.Position = 0;
                    UploadStreamChecked(client, current, remotePath, "Recovery rollback validation failed for " + remotePath);
                }
                else if (client.FileExists(remotePath))
                {
                    client.DeleteFile(remotePath);
                }
            }
            catch (Exception ex)
            {
                errors.Add(remotePath + ": " + ex.Message);
            }
        }
        return errors;
    }

    private static void UploadManifest(FtpClient client, string path, DeploymentBackupManifest manifest)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(manifest, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
        using var stream = new MemoryStream(bytes, writable: false);
        EnsureParentDirectory(client, path);
        UploadStreamChecked(client, stream, path, "Remote deployment backup manifest validation failed.");
    }

    private static DeploymentBackupManifest DownloadManifest(FtpClient client, string path)
    {
        using var stream = new MemoryStream();
        if (!client.DownloadStream(stream, path)) throw new IOException("Could not download remote deployment backup manifest: " + path);
        var manifest = JsonSerializer.Deserialize<DeploymentBackupManifest>(stream.ToArray(), new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        });
        return manifest ?? throw new InvalidOperationException("The remote deployment backup manifest could not be read: " + path);
    }

    private static void ValidateManifest(DeploymentBackupManifest manifest, string expectedMode)
    {
        if (!string.Equals(manifest.Schema, BackupManifestSchema, StringComparison.Ordinal) ||
            !string.Equals(manifest.Mode, expectedMode, StringComparison.Ordinal) ||
            !manifest.DeploymentCompleted ||
            manifest.Files.Count == 0 ||
            manifest.Files.Select(entry => entry.RemotePath).Distinct(StringComparer.Ordinal).Count() != manifest.Files.Count ||
            (manifest.Files.Any(entry => string.Equals(entry.RemotePath, MainRemotePath, StringComparison.Ordinal)) &&
             !string.Equals(manifest.Files[^1].RemotePath, MainRemotePath, StringComparison.Ordinal)) ||
            manifest.Files.Any(entry => !IsSafeProductionTarget(entry.RemotePath) || entry.PublishedBytes <= 0 || entry.OriginalBytes < 0 ||
                (entry.OriginalExisted && entry.OriginalSha256.Length != 64) ||
                (!entry.OriginalExisted && (entry.OriginalBytes != 0 || entry.OriginalSha256.Length != 0))) ||
            (manifest.PublishedArtifacts.Count > 0 &&
             (manifest.PublishedArtifacts.Select(entry => entry.RemotePath).Distinct(StringComparer.Ordinal).Count() != manifest.PublishedArtifacts.Count ||
              !string.Equals(manifest.PublishedArtifacts[^1].RemotePath, MainRemotePath, StringComparison.Ordinal) ||
              manifest.PublishedArtifacts.Any(entry => !IsSafeProductionTarget(entry.RemotePath) || entry.Bytes <= 0 || entry.Sha256.Length != 64))))
            throw new InvalidOperationException("The remote Production backup manifest failed schema, mode, route, size or activation-order validation.");
    }

    private static void UploadStreamChecked(FtpClient client, MemoryStream stream, string path, string error)
    {
        stream.Position = 0;
        var status = client.UploadStream(stream, path, FtpRemoteExists.Overwrite, createRemoteDir: false);
        if (status != FtpStatus.Success || client.GetFileSize(path) != stream.Length) throw new IOException(error);
    }

    private static string ComputeSha256(MemoryStream stream) =>
        Convert.ToHexString(SHA256.HashData(stream.ToArray())).ToLowerInvariant();

    private static DeltaPublishSelection ResolveDeltaPublishSelection(
        FtpClient client,
        string deploymentMode,
        WebsiteProductionPublishPlan plan,
        IProgress<PublishProgress>? progress)
    {
        var fullPublish = new DeltaPublishSelection(false, string.Empty, plan.Files);
        IReadOnlyList<FtpListItem> listing;
        try
        {
            listing = client.GetListing(BackupRoot);
        }
        catch
        {
            return fullPublish;
        }

        var candidates = listing
            .Where(item => item.Type == FtpObjectType.Directory)
            .Select(item => new { item.Name, Timestamp = GetDeploymentTimestampKey(item.Name, deploymentMode) })
            .Where(item => item.Timestamp is not null)
            .OrderByDescending(item => item.Timestamp, StringComparer.Ordinal)
            .ToList();

        foreach (var candidate in candidates)
        {
            var folder = BackupRoot + "/" + candidate.Name;
            var manifestPath = folder + "/" + BackupManifestFileName;
            try
            {
                if (!client.FileExists(manifestPath)) continue;
                var manifest = DownloadManifest(client, manifestPath);
                if (!manifest.DeploymentCompleted) continue;
                if (string.Equals(deploymentMode, "Production", StringComparison.Ordinal) &&
                    string.Equals(manifest.Mode, "RevertRecovery", StringComparison.Ordinal))
                    return fullPublish;
                if (!string.Equals(manifest.Mode, deploymentMode, StringComparison.Ordinal)) continue;
                if (!PublishedStateMatchesPlan(manifest.PublishedArtifacts, deploymentMode, plan))
                    return fullPublish;

                var baseline = manifest.PublishedArtifacts.ToDictionary(entry => entry.RemotePath, StringComparer.Ordinal);
                var changed = new List<WebsiteProductionPublishFile>();
                for (var index = 0; index < plan.Files.Count; index++)
                {
                    var file = plan.Files[index];
                    var published = baseline[file.RemotePath];
                    var unchanged = published.Bytes == file.Bytes &&
                                    string.Equals(published.Sha256, file.Sha256, StringComparison.Ordinal) &&
                                    RemoteArtifactMatchesExpected(client, file.RemotePath, file.Bytes);
                    if (!unchanged) changed.Add(file);
                    progress?.Report(new PublishProgress("Remote delta comparison", index + 1, plan.Files.Count, file.RemotePath));
                }
                return new DeltaPublishSelection(true, folder, changed);
            }
            catch
            {
                // Never fall back to an older state after the newest completed event is unreadable.
                return fullPublish;
            }
        }

        return fullPublish;
    }

    private static string? GetDeploymentTimestampKey(string name, string deploymentMode)
    {
        if (string.Equals(deploymentMode, "Test", StringComparison.Ordinal))
            return name.StartsWith("website_test_", StringComparison.Ordinal) ? name["website_test_".Length..] : null;
        if (name.StartsWith("website_revert_", StringComparison.Ordinal)) return name["website_revert_".Length..];
        return name.StartsWith("website_", StringComparison.Ordinal) && !name.StartsWith("website_test_", StringComparison.Ordinal)
            ? name["website_".Length..]
            : null;
    }

    private static bool PublishedStateMatchesPlan(
        IReadOnlyList<DeploymentPublishedArtifact> publishedArtifacts,
        string deploymentMode,
        WebsiteProductionPublishPlan plan)
    {
        var expectedEntry = string.Equals(deploymentMode, "Test", StringComparison.Ordinal)
            ? WebsiteProductionPublishPlanService.TestEntryRemotePath
            : MainRemotePath;
        return publishedArtifacts.Count == plan.Files.Count &&
               publishedArtifacts.Count > 0 &&
               string.Equals(publishedArtifacts[^1].RemotePath, expectedEntry, StringComparison.Ordinal) &&
               publishedArtifacts.Select(entry => entry.RemotePath).Distinct(StringComparer.Ordinal).Count() == publishedArtifacts.Count &&
               publishedArtifacts.All(entry => IsSafePublishedTarget(deploymentMode, entry.RemotePath) && entry.Bytes > 0 && entry.Sha256.Length == 64) &&
               publishedArtifacts.Select(entry => entry.RemotePath).ToHashSet(StringComparer.Ordinal)
                   .SetEquals(plan.Files.Select(file => file.RemotePath));
    }

    private static bool IsSafePublishedTarget(string deploymentMode, string path)
    {
        if (string.Equals(deploymentMode, "Production", StringComparison.Ordinal)) return IsSafeProductionTarget(path);
        return (string.Equals(path, WebsiteProductionPublishPlanService.TestEntryRemotePath, StringComparison.Ordinal) ||
                path.StartsWith(WebsiteProductionPublishPlanService.TestRemoteRoot + "/", StringComparison.Ordinal)) &&
               !path.Contains("..", StringComparison.Ordinal) &&
               !path.Contains('\\') &&
               !path.StartsWith(BackupRoot + "/", StringComparison.OrdinalIgnoreCase);
    }

    private static bool RemoteArtifactMatchesExpected(FtpClient client, string remotePath, long expectedBytes)
    {
        var matches = false;
        ExecuteControlOperationWithRetry(client, () =>
        {
            matches = client.FileExists(remotePath) && client.GetFileSize(remotePath) == expectedBytes;
        });
        return matches;
    }

    private static IReadOnlyList<IReadOnlyList<TransferWorkItem>> BuildTransferBatches(
        IReadOnlyList<WebsiteProductionPublishFile> files,
        int workerCount)
    {
        var batches = Enumerable.Range(0, workerCount).Select(_ => new List<TransferWorkItem>()).ToArray();
        var batchBytes = new long[workerCount];
        foreach (var work in files
                     .Select((file, index) => new TransferWorkItem(index, file))
                     .OrderByDescending(item => item.File.Bytes))
        {
            var target = 0;
            for (var index = 1; index < workerCount; index++)
            {
                if (batchBytes[index] < batchBytes[target]) target = index;
            }
            batches[target].Add(work);
            batchBytes[target] += work.File.Bytes;
        }
        return batches.Where(batch => batch.Count > 0).Cast<IReadOnlyList<TransferWorkItem>>().ToList();
    }

    private static void ValidateLocalArtifacts(
        IReadOnlyList<WebsiteProductionPublishFile> files,
        IProgress<PublishProgress>? progress)
    {
        var errors = new ConcurrentQueue<Exception>();
        var completed = 0;
        Parallel.ForEach(
            files,
            new ParallelOptions { MaxDegreeOfParallelism = LocalValidationWorkers },
            file =>
            {
                try
                {
                    ValidateLocalArtifact(file);
                    var completedNow = Interlocked.Increment(ref completed);
                    progress?.Report(new PublishProgress("Parallel local artifact validation", completedNow, files.Count, file.RemotePath));
                }
                catch (Exception ex)
                {
                    errors.Enqueue(ex);
                }
            });
        if (!errors.IsEmpty)
            throw new InvalidOperationException("Parallel local publish-artifact validation failed: " + string.Join(" | ", errors.Select(error => error.Message)));
    }

    private static void ValidateLocalArtifact(WebsiteProductionPublishFile file)
    {
        if (!File.Exists(file.LocalPath)) throw new FileNotFoundException("A publish-plan artifact is missing.", file.LocalPath);
        if (new FileInfo(file.LocalPath).Length != file.Bytes) throw new IOException("A publish-plan artifact changed size: " + file.LocalPath);
        using var stream = File.OpenRead(file.LocalPath);
        var actualHash = Convert.ToHexString(SHA256.HashData(stream)).ToLowerInvariant();
        if (!string.Equals(actualHash, file.Sha256, StringComparison.Ordinal))
            throw new IOException("A publish-plan artifact changed content: " + file.LocalPath);
    }

    private static bool IsProductionBackupFolderName(string name) =>
        name.StartsWith("website_", StringComparison.Ordinal) &&
        !name.StartsWith("website_test_", StringComparison.Ordinal) &&
        !name.StartsWith("website_revert_", StringComparison.Ordinal) &&
        !name.Contains('/') && !name.Contains('\\');

    private static bool IsProductionBackupFolderPath(string path)
    {
        var prefix = BackupRoot + "/";
        return path.StartsWith(prefix, StringComparison.Ordinal) && IsProductionBackupFolderName(path[prefix.Length..]);
    }

    private static bool IsSafeProductionTarget(string path) =>
        path.StartsWith("/", StringComparison.Ordinal) &&
        !path.Contains("..", StringComparison.Ordinal) &&
        !path.Contains('\\') &&
        !path.StartsWith(BackupRoot + "/", StringComparison.OrdinalIgnoreCase) &&
        !path.Contains("index-test.html", StringComparison.OrdinalIgnoreCase) &&
        !path.StartsWith(WebsiteProductionPublishPlanService.TestRemoteRoot + "/", StringComparison.OrdinalIgnoreCase);

    private sealed class DeploymentBackupManifest
    {
        public string Schema { get; init; } = string.Empty;
        public string Mode { get; init; } = string.Empty;
        public string CreatedAtUtc { get; init; } = string.Empty;
        public string SourcePlanVersion { get; init; } = string.Empty;
        public bool DeploymentCompleted { get; set; }
        public List<DeploymentBackupEntry> Files { get; init; } = new();
        public List<DeploymentPublishedArtifact> PublishedArtifacts { get; init; } = new();
    }

    private sealed record DeltaPublishSelection(bool BaselineUsed, string BaselineFolder, IReadOnlyList<WebsiteProductionPublishFile> Files);
    private sealed record TransferWorkItem(int Index, WebsiteProductionPublishFile File);

    private sealed class DeploymentPublishedArtifact
    {
        public string RemotePath { get; init; } = string.Empty;
        public long Bytes { get; init; }
        public string Sha256 { get; init; } = string.Empty;
    }

    private sealed class DeploymentBackupEntry
    {
        public string RemotePath { get; init; } = string.Empty;
        public bool OriginalExisted { get; init; }
        public long OriginalBytes { get; init; }
        public string OriginalSha256 { get; init; } = string.Empty;
        public long PublishedBytes { get; init; }
    }

    private static void ConnectWithRetry(FtpClient client)
    {
        Exception? lastError = null;
        for (var attempt = 1; attempt <= TransferRetryAttempts; attempt++)
        {
            try
            {
                if (!client.IsConnected) client.Connect();
                return;
            }
            catch (Exception ex)
            {
                lastError = ex;
                try { if (client.IsConnected) client.Disconnect(); } catch { }
                if (attempt < TransferRetryAttempts) Thread.Sleep(250 * attempt);
            }
        }
        throw new IOException($"FTPS connection failed after {TransferRetryAttempts} attempts: {lastError?.Message}", lastError);
    }

    private static void ActivateStagedFileWithRetry(FtpClient client, string stagedPath, string remotePath, long expectedBytes)
    {
        Exception? lastError = null;
        for (var attempt = 1; attempt <= TransferRetryAttempts; attempt++)
        {
            try
            {
                ConnectWithRetry(client);
                EnsureParentDirectory(client, remotePath);
                if (client.FileExists(stagedPath) && !client.MoveFile(stagedPath, remotePath, FtpRemoteExists.Overwrite))
                    throw new IOException("The server did not confirm activation.");
                if (!client.FileExists(remotePath) || client.GetFileSize(remotePath) != expectedBytes)
                    throw new IOException("The activated target is missing or has the wrong size.");
                return;
            }
            catch (Exception ex)
            {
                lastError = ex;
                try { if (client.IsConnected) client.Disconnect(); } catch { }
                if (attempt < TransferRetryAttempts) Thread.Sleep(250 * attempt);
            }
        }
        throw new IOException($"Could not activate {remotePath} after {TransferRetryAttempts} attempts: {lastError?.Message}", lastError);
    }

    private static void DeleteRemoteFileWithRetry(FtpClient client, string remotePath)
    {
        Exception? lastError = null;
        for (var attempt = 1; attempt <= TransferRetryAttempts; attempt++)
        {
            try
            {
                ConnectWithRetry(client);
                if (client.FileExists(remotePath)) client.DeleteFile(remotePath);
                if (client.FileExists(remotePath)) throw new IOException("The server still reports the target after deletion.");
                return;
            }
            catch (Exception ex)
            {
                lastError = ex;
                try { if (client.IsConnected) client.Disconnect(); } catch { }
                if (attempt < TransferRetryAttempts) Thread.Sleep(250 * attempt);
            }
        }
        throw new IOException($"Could not remove {remotePath} after {TransferRetryAttempts} attempts: {lastError?.Message}", lastError);
    }

    private static void DisconnectForParallelWorkers(FtpClient client)
    {
        try { if (client.IsConnected) client.Disconnect(); } catch { }
        if (client.IsConnected) throw new IOException("The FTPS control session could not be closed before the host-compatible transfer worker started.");
    }

    private static void DisconnectBestEffort(FtpClient client)
    {
        try { if (client.IsConnected) client.Disconnect(); } catch { }
    }

    private static void ExecuteControlOperationWithRetry(FtpClient client, Action operation)
    {
        Exception? lastError = null;
        for (var attempt = 1; attempt <= TransferRetryAttempts; attempt++)
        {
            try
            {
                ConnectWithRetry(client);
                operation();
                return;
            }
            catch (Exception ex)
            {
                lastError = ex;
                try { if (client.IsConnected) client.Disconnect(); } catch { }
                if (attempt < TransferRetryAttempts) Thread.Sleep(250 * attempt);
            }
        }
        throw new IOException($"FTPS control operation failed after {TransferRetryAttempts} attempts: {lastError?.Message}", lastError);
    }

    private FtpClient CreateClient(string password, Action<string> observeCertificate)
    {
        var client = new FtpClient(Host, UserName, password, Port);
        client.Config.EncryptionMode = FtpEncryptionMode.Explicit;
        client.Config.DataConnectionType = FtpDataConnectionType.AutoPassive;
        client.Config.SslProtocols = SslProtocols.Tls12 | SslProtocols.Tls13;
        client.Config.ConnectTimeout = 20000;
        client.Config.DataConnectionConnectTimeout = 20000;
        client.ValidateCertificate += (_, args) =>
        {
            observeCertificate(args.Certificate?.Subject ?? string.Empty);
            args.Accept = args.PolicyErrors == System.Net.Security.SslPolicyErrors.None;
        };
        return client;
    }

    private static void EnsureDirectory(FtpClient client, string path)
    {
        if (!client.DirectoryExists(path) && !client.CreateDirectory(path)) throw new IOException($"Could not create remote directory {path}.");
    }

    private static void EnsureParentDirectory(FtpClient client, string filePath)
    {
        var separator = filePath.LastIndexOf('/');
        if (separator > 0) EnsureDirectoryTree(client, filePath[..separator]);
    }

    private static void EnsureTransferParentDirectory(
        FtpClient client,
        string filePath,
        object directoryCreationGate,
        ConcurrentDictionary<string, byte> preparedDirectories)
    {
        var separator = filePath.LastIndexOf('/');
        if (separator <= 0) return;
        var parent = filePath[..separator];
        if (preparedDirectories.ContainsKey(parent)) return;

        lock (directoryCreationGate)
        {
            if (preparedDirectories.ContainsKey(parent)) return;
            EnsureDirectoryTree(client, parent);
            preparedDirectories[parent] = 0;
        }
    }

    private static void EnsureDirectoryTree(FtpClient client, string path)
    {
        var current = string.Empty;
        foreach (var part in path.Split('/', StringSplitOptions.RemoveEmptyEntries))
        {
            current += "/" + part;
            EnsureDirectory(client, current);
        }
    }
}
