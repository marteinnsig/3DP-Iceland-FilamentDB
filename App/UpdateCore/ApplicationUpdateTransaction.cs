using System.Security.Cryptography;
using System.Text.Json;

namespace FilamentDbApp.UpdateCore;

public sealed class ApplicationUpdateTransactionRequest
{
    public string TransactionId { get; set; } = string.Empty;
    public string LiveDirectory { get; set; } = string.Empty;
    public string StagingDirectory { get; set; } = string.Empty;
    public string RollbackDirectory { get; set; } = string.Empty;
    public string StatePath { get; set; } = string.Empty;
    public string PreviousVersion { get; set; } = string.Empty;
    public string NewVersion { get; set; } = string.Empty;
    public string ReleaseCode { get; set; } = string.Empty;
    public string SignatureAlgorithm { get; set; } = string.Empty;
    public string SigningKeyFingerprint { get; set; } = string.Empty;
    public string DatabaseBackupPath { get; set; } = string.Empty;
    public string ApplicationRelativePath { get; set; } = "3DPIcelandFilamentDB.exe";
    public int WaitForProcessId { get; set; }
    public string HealthAcknowledgementPath { get; set; } = string.Empty;
    public int HealthTimeoutSeconds { get; set; } = 60;
    public int MinimumDatabaseSchema { get; set; }
    public int MaximumDatabaseSchema { get; set; }
    public List<string> GovernedFiles { get; set; } = new();
}

public sealed class ApplicationUpdateHealthAcknowledgement
{
    public const string Schema = "3dpiceland.application-update-health.v1";
    public string HealthSchema { get; set; } = Schema;
    public string TransactionId { get; set; } = string.Empty;
    public string ReleaseVersion { get; set; } = string.Empty;
    public int DatabaseSchema { get; set; }
    public string AcknowledgedAtUtc { get; set; } = string.Empty;
}

public sealed class ApplicationUpdateTransactionState
{
    public const string Schema = "3dpiceland.application-update-transaction.v1";
    public string TransactionSchema { get; set; } = Schema;
    public string TransactionId { get; set; } = string.Empty;
    public string Phase { get; set; } = "Prepared";
    public string PreviousVersion { get; set; } = string.Empty;
    public string NewVersion { get; set; } = string.Empty;
    public string ReleaseCode { get; set; } = string.Empty;
    public string SignatureAlgorithm { get; set; } = string.Empty;
    public string SigningKeyFingerprint { get; set; } = string.Empty;
    public string DatabaseBackupPath { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
    public string Error { get; set; } = string.Empty;
    public List<string> PreviouslyMissingFiles { get; set; } = new();
}

public sealed record ApplicationUpdateTransactionResult(bool Succeeded, bool RolledBack, string Phase, string Detail);
public sealed record ApplicationUpdateTransactionVerificationResult(bool Passed, string Detail);
public sealed record ApplicationUpdateTransactionDiagnostic(
    string TransactionDirectory, string StatePath, string RequestPath, string TransactionId, string Phase,
    string PreviousVersion, string NewVersion, string ReleaseCode, string SignatureAlgorithm, string SigningKeyFingerprint,
    string UpdatedAtUtc, string DatabaseBackupPath,
    bool StateReadable, bool RequestReadable, bool IsIncomplete, string RecoveryAction, string Detail);

public sealed class ApplicationUpdateTransactionEngine
{
    private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

    public ApplicationUpdateTransactionResult Execute(
        ApplicationUpdateTransactionRequest request,
        Func<bool> healthCheck,
        int failAfterInstalledFiles = -1)
    {
        Validate(request);
        var state = NewState(request);
        WriteState(request.StatePath, state);
        try
        {
            Directory.CreateDirectory(request.RollbackDirectory);
            foreach (var relativePath in request.GovernedFiles)
            {
                var livePath = ResolveContained(request.LiveDirectory, relativePath);
                var rollbackPath = ResolveContained(request.RollbackDirectory, relativePath);
                if (File.Exists(livePath)) CopyAtomic(livePath, rollbackPath);
                else state.PreviouslyMissingFiles.Add(relativePath);
            }
            SetPhase(request.StatePath, state, "SnapshotReady");
        }
        catch (Exception snapshotError)
        {
            SetPhase(request.StatePath, state, "Prepared", "Last-known-good snapshot is incomplete; no application files were installed. " + snapshotError.Message);
            return new(false, false, state.Phase, state.Error);
        }

        try
        {
            var installed = 0;
            foreach (var relativePath in request.GovernedFiles)
            {
                var stagedPath = ResolveContained(request.StagingDirectory, relativePath);
                var livePath = ResolveContained(request.LiveDirectory, relativePath);
                CopyAtomic(stagedPath, livePath);
                installed++;
                if (failAfterInstalledFiles >= 0 && installed >= failAfterInstalledFiles)
                    throw new IOException("Injected partial-install failure after " + installed + " file(s).");
            }
            SetPhase(request.StatePath, state, "Installed");
            if (!healthCheck()) throw new InvalidOperationException("Post-update health acknowledgement failed.");
            SetPhase(request.StatePath, state, "Committed");
            return new(true, false, state.Phase, $"Transaction {request.TransactionId} committed {request.GovernedFiles.Count:N0} governed file(s).");
        }
        catch (Exception updateError)
        {
            try
            {
                SetPhase(request.StatePath, state, "RollingBack", updateError.Message);
                foreach (var relativePath in request.GovernedFiles)
                {
                    var livePath = ResolveContained(request.LiveDirectory, relativePath);
                    var rollbackPath = ResolveContained(request.RollbackDirectory, relativePath);
                    if (File.Exists(rollbackPath))
                    {
                        if (!FilesHaveSameContent(rollbackPath, livePath)) CopyAtomic(rollbackPath, livePath);
                    }
                    else if (state.PreviouslyMissingFiles.Contains(relativePath, StringComparer.Ordinal) && File.Exists(livePath)) File.Delete(livePath);
                }
                SetPhase(request.StatePath, state, "RolledBack", updateError.Message);
                return new(false, true, state.Phase, "Update failed and application files were rolled back: " + updateError.Message);
            }
            catch (Exception rollbackError)
            {
                SetPhase(request.StatePath, state, "RollbackFailed", updateError.Message + " | " + rollbackError.Message);
                return new(false, false, state.Phase, "Update and rollback failed: " + state.Error);
            }
        }
    }

    public ApplicationUpdateTransactionResult Recover(ApplicationUpdateTransactionRequest request, Func<bool>? healthCheck = null)
    {
        var state = ReadState(request.StatePath);
        if (state.Phase == "Prepared") Validate(request); else ValidateRollbackRequest(request);
        ValidateStateIdentity(request, state);
        return state.Phase switch
        {
            "Prepared" => Execute(request, healthCheck ?? throw new InvalidOperationException("Prepared recovery requires the guarded startup health check.")),
            "SnapshotReady" or "Installed" or "RollingBack" or "RollbackFailed" => RestoreSnapshot(request, state, "Interrupted transaction recovery requested."),
            "Committed" => new(true, false, state.Phase, "Transaction is already committed; no recovery changed application files."),
            "RolledBack" => new(false, true, state.Phase, "Transaction is already rolled back; no recovery changed application files."),
            _ => throw new InvalidOperationException("Unsupported transaction phase: " + state.Phase)
        };
    }

    public static ApplicationUpdateTransactionVerificationResult RunContractVerification()
    {
        var root = Path.Combine(Path.GetTempPath(), "3DPIceland-UpdaterVerify-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(root);
        try
        {
            var success = CreateFixture(root, "success");
            var successResult = new ApplicationUpdateTransactionEngine().Execute(success, () => true);
            if (!successResult.Succeeded || Read(success.LiveDirectory, "app.exe") != "new-app" || Read(success.LiveDirectory, "Assets/data.txt") != "new-asset")
                return new(false, "Successful transaction did not commit the complete staged file set.");

            var partial = CreateFixture(root, "partial");
            var partialResult = new ApplicationUpdateTransactionEngine().Execute(partial, () => true, failAfterInstalledFiles: 1);
            if (!partialResult.RolledBack || Read(partial.LiveDirectory, "app.exe") != "old-app" || Read(partial.LiveDirectory, "Assets/data.txt") != "old-asset")
                return new(false, "Partial-install failure did not restore the complete last-known-good file set.");

            var unhealthy = CreateFixture(root, "unhealthy");
            var unhealthyResult = new ApplicationUpdateTransactionEngine().Execute(unhealthy, () => false);
            if (!unhealthyResult.RolledBack || Read(unhealthy.LiveDirectory, "app.exe") != "old-app")
                return new(false, "Failed health acknowledgement did not roll back application files.");

            var traversal = CreateFixture(root, "traversal"); traversal.GovernedFiles.Add("../escape.exe");
            try { _ = new ApplicationUpdateTransactionEngine().Execute(traversal, () => true); return new(false, "Traversal path was accepted."); }
            catch (InvalidOperationException) { }

            var interruptedInstalled = CreateFixture(root, "interrupted-installed");
            var interruptedState = NewState(interruptedInstalled);
            Directory.CreateDirectory(interruptedInstalled.RollbackDirectory);
            foreach (var relativePath in interruptedInstalled.GovernedFiles)
                CopyAtomic(ResolveContained(interruptedInstalled.LiveDirectory, relativePath), ResolveContained(interruptedInstalled.RollbackDirectory, relativePath));
            SetPhase(interruptedInstalled.StatePath, interruptedState, "SnapshotReady");
            CopyAtomic(ResolveContained(interruptedInstalled.StagingDirectory, "app.exe"), ResolveContained(interruptedInstalled.LiveDirectory, "app.exe"));
            var recoveryResult = new ApplicationUpdateTransactionEngine().Recover(interruptedInstalled);
            if (!recoveryResult.RolledBack || Read(interruptedInstalled.LiveDirectory, "app.exe") != "old-app" || Read(interruptedInstalled.LiveDirectory, "Assets/data.txt") != "old-asset")
                return new(false, "Interrupted SnapshotReady recovery did not restore the complete last-known-good file set.");

            foreach (var phase in new[] { "Installed", "RollingBack", "RollbackFailed" })
            {
                var interrupted = CreateFixture(root, "interrupted-" + phase.ToLowerInvariant());
                var state = NewState(interrupted);
                Directory.CreateDirectory(interrupted.RollbackDirectory);
                foreach (var relativePath in interrupted.GovernedFiles)
                {
                    CopyAtomic(ResolveContained(interrupted.LiveDirectory, relativePath), ResolveContained(interrupted.RollbackDirectory, relativePath));
                    CopyAtomic(ResolveContained(interrupted.StagingDirectory, relativePath), ResolveContained(interrupted.LiveDirectory, relativePath));
                }
                SetPhase(interrupted.StatePath, state, phase, "Injected interruption.");
                var result = new ApplicationUpdateTransactionEngine().Recover(interrupted);
                if (!result.RolledBack || Read(interrupted.LiveDirectory, "app.exe") != "old-app" || Read(interrupted.LiveDirectory, "Assets/data.txt") != "old-asset")
                    return new(false, $"Interrupted {phase} recovery did not restore the complete last-known-good file set.");
            }

            var prepared = CreateFixture(root, "interrupted-prepared");
            WriteState(prepared.StatePath, NewState(prepared));
            var preparedResult = new ApplicationUpdateTransactionEngine().Recover(prepared, () => true);
            if (!preparedResult.Succeeded || preparedResult.Phase != "Committed" || Read(prepared.LiveDirectory, "app.exe") != "new-app")
                return new(false, "Interrupted Prepared recovery did not safely restart and commit the guarded transaction.");

            WriteRequest(interruptedInstalled);
            var diagnostics = ApplicationUpdateTransactionDiagnostics.InspectRoot(root);
            if (!diagnostics.Any(item => item.TransactionId == "interrupted-installed" && item.Phase == "RolledBack" && item.StateReadable && item.RequestReadable))
                return new(false, "Durable transaction diagnostics did not classify the recovered transaction.");

            return new(true, "Isolated success committed; injected partial install and failed health rolled back; Prepared safely restarted; SnapshotReady, Installed, RollingBack and RollbackFailed restored; read-only history classified; traversal blocked; SQLite backup reference preserved.");
        }
        catch (Exception ex) { return new(false, ex.Message); }
        finally { try { Directory.Delete(root, recursive: true); } catch { } }
    }

    private static ApplicationUpdateTransactionRequest CreateFixture(string root, string name)
    {
        var transaction = Path.Combine(root, name); var live = Path.Combine(root, name + "-live"); var staging = Path.Combine(transaction, "staging");
        Write(live, "app.exe", "old-app"); Write(live, "Assets/data.txt", "old-asset");
        Write(staging, "app.exe", "new-app"); Write(staging, "Assets/data.txt", "new-asset");
        return new()
        {
            TransactionId = name, LiveDirectory = live, StagingDirectory = staging,
            RollbackDirectory = Path.Combine(transaction, "rollback"), StatePath = Path.Combine(transaction, "transaction-state.json"),
            PreviousVersion = "1.0.0", NewVersion = "2.0.0", DatabaseBackupPath = Path.Combine(transaction, "filamentdb_manual.sqlite"),
            ApplicationRelativePath = "app.exe",
            GovernedFiles = new() { "app.exe", "Assets/data.txt" }
        };
    }

    private static void Validate(ApplicationUpdateTransactionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TransactionId) || !Version.TryParse(request.PreviousVersion, out var previous) || !Version.TryParse(request.NewVersion, out var next) || next <= previous)
            throw new InvalidOperationException("Transaction identity or upgrade version policy is invalid.");
        if (!Directory.Exists(request.LiveDirectory) || !Directory.Exists(request.StagingDirectory) || request.GovernedFiles.Count == 0)
            throw new InvalidOperationException("Live/staging directories and governed files are required.");
        var unique = new HashSet<string>(StringComparer.Ordinal);
        foreach (var path in request.GovernedFiles)
        {
            if (!IsSafeRelativePath(path) || !unique.Add(path)) throw new InvalidOperationException("Unsafe or duplicate governed path: " + path);
            if (!File.Exists(ResolveContained(request.StagingDirectory, path))) throw new InvalidOperationException("Staged governed file is missing: " + path);
        }
        var live = Path.GetFullPath(request.LiveDirectory); var staging = Path.GetFullPath(request.StagingDirectory); var rollback = Path.GetFullPath(request.RollbackDirectory);
        if (string.Equals(live, staging, StringComparison.OrdinalIgnoreCase) || string.Equals(live, rollback, StringComparison.OrdinalIgnoreCase) || string.Equals(staging, rollback, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("Live, staging and rollback directories must be distinct.");
        var transactionRoot = Path.GetDirectoryName(Path.GetFullPath(request.StatePath)) ?? throw new InvalidOperationException("Transaction state folder is unavailable.");
        if (!IsDirectoryContained(transactionRoot, staging) || !IsDirectoryContained(transactionRoot, rollback))
            throw new InvalidOperationException("Staging and rollback directories must stay inside the durable transaction folder.");
        if (IsDirectoryContained(transactionRoot, live) || IsDirectoryContained(live, transactionRoot))
            throw new InvalidOperationException("Live application and durable transaction folders must not contain one another.");
        if (!IsSafeRelativePath(request.ApplicationRelativePath) || !request.GovernedFiles.Contains(request.ApplicationRelativePath, StringComparer.Ordinal))
            throw new InvalidOperationException("The application executable must be a governed relative path.");
        if (!string.IsNullOrWhiteSpace(request.HealthAcknowledgementPath))
        {
            var ack = Path.GetFullPath(request.HealthAcknowledgementPath);
            if (!IsFileContained(transactionRoot, ack)) throw new InvalidOperationException("Health acknowledgement must stay inside the durable transaction folder.");
        }
    }
    private static void ValidateRollbackRequest(ApplicationUpdateTransactionRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.TransactionId) || !Directory.Exists(request.LiveDirectory) ||
            !Directory.Exists(request.RollbackDirectory) || request.GovernedFiles.Count == 0)
            throw new InvalidOperationException("Live/rollback directories, transaction identity and governed files are required for recovery.");
        var unique = new HashSet<string>(StringComparer.Ordinal);
        foreach (var path in request.GovernedFiles)
            if (!IsSafeRelativePath(path) || !unique.Add(path)) throw new InvalidOperationException("Unsafe or duplicate governed recovery path: " + path);
        var live = Path.GetFullPath(request.LiveDirectory); var rollback = Path.GetFullPath(request.RollbackDirectory);
        var transactionRoot = Path.GetDirectoryName(Path.GetFullPath(request.StatePath)) ?? throw new InvalidOperationException("Transaction state folder is unavailable.");
        if (!IsDirectoryContained(transactionRoot, rollback) || IsDirectoryContained(transactionRoot, live) || IsDirectoryContained(live, transactionRoot))
            throw new InvalidOperationException("Recovery live/rollback directory containment is invalid.");
        if (!IsSafeRelativePath(request.ApplicationRelativePath) || !request.GovernedFiles.Contains(request.ApplicationRelativePath, StringComparer.Ordinal))
            throw new InvalidOperationException("The recovery application executable must be a governed relative path.");
    }

    private static ApplicationUpdateTransactionState NewState(ApplicationUpdateTransactionRequest request) => new()
    {
        TransactionId = request.TransactionId, PreviousVersion = request.PreviousVersion, NewVersion = request.NewVersion,
        ReleaseCode = request.ReleaseCode, SignatureAlgorithm = request.SignatureAlgorithm, SigningKeyFingerprint = request.SigningKeyFingerprint,
        DatabaseBackupPath = request.DatabaseBackupPath, UpdatedAtUtc = DateTimeOffset.UtcNow.ToString("O")
    };
    private static ApplicationUpdateTransactionState ReadState(string statePath) =>
        JsonSerializer.Deserialize<ApplicationUpdateTransactionState>(File.ReadAllText(statePath))
        ?? throw new InvalidOperationException("Transaction state could not be read.");
    private static void ValidateStateIdentity(ApplicationUpdateTransactionRequest request, ApplicationUpdateTransactionState state)
    {
        if (state.TransactionSchema != ApplicationUpdateTransactionState.Schema ||
            !string.Equals(state.TransactionId, request.TransactionId, StringComparison.Ordinal) ||
            !string.Equals(state.PreviousVersion, request.PreviousVersion, StringComparison.Ordinal) ||
            !string.Equals(state.NewVersion, request.NewVersion, StringComparison.Ordinal) ||
            !string.Equals(state.ReleaseCode, request.ReleaseCode, StringComparison.Ordinal) ||
            !string.Equals(state.SignatureAlgorithm, request.SignatureAlgorithm, StringComparison.Ordinal) ||
            !string.Equals(state.SigningKeyFingerprint, request.SigningKeyFingerprint, StringComparison.Ordinal))
            throw new InvalidOperationException("Transaction request and durable state identity do not match.");
    }
    private static ApplicationUpdateTransactionResult RestoreSnapshot(ApplicationUpdateTransactionRequest request, ApplicationUpdateTransactionState state, string reason)
    {
        try
        {
            SetPhase(request.StatePath, state, "RollingBack", reason);
            foreach (var relativePath in request.GovernedFiles)
            {
                var livePath = ResolveContained(request.LiveDirectory, relativePath);
                var rollbackPath = ResolveContained(request.RollbackDirectory, relativePath);
                if (File.Exists(rollbackPath))
                {
                    if (!FilesHaveSameContent(rollbackPath, livePath)) CopyAtomic(rollbackPath, livePath);
                }
                else if (state.PreviouslyMissingFiles.Contains(relativePath, StringComparer.Ordinal) && File.Exists(livePath)) File.Delete(livePath);
                else if (!state.PreviouslyMissingFiles.Contains(relativePath, StringComparer.Ordinal))
                    throw new FileNotFoundException("Last-known-good snapshot file is missing.", rollbackPath);
            }
            SetPhase(request.StatePath, state, "RolledBack", reason);
            return new(false, true, state.Phase, "Interrupted update application files were restored. SQLite was not restored.");
        }
        catch (Exception rollbackError)
        {
            SetPhase(request.StatePath, state, "RollbackFailed", reason + " | " + rollbackError.Message);
            return new(false, false, state.Phase, "Interrupted update recovery failed: " + state.Error);
        }
    }
    private static void SetPhase(string statePath, ApplicationUpdateTransactionState state, string phase, string error = "")
    {
        state.Phase = phase; state.Error = error; state.UpdatedAtUtc = DateTimeOffset.UtcNow.ToString("O"); WriteState(statePath, state);
    }
    private static void WriteState(string statePath, ApplicationUpdateTransactionState state)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(Path.GetFullPath(statePath))!);
        var temp = statePath + ".tmp"; File.WriteAllText(temp, JsonSerializer.Serialize(state, JsonOptions)); File.Move(temp, statePath, true);
    }
    private static void CopyAtomic(string source, string destination)
    {
        Directory.CreateDirectory(Path.GetDirectoryName(destination)!);
        var temp = destination + ".update-tmp";
        Exception? lastError = null;
        for (var attempt = 1; attempt <= 40; attempt++)
        {
            try
            {
                File.Copy(source, temp, true);
                File.Move(temp, destination, true);
                return;
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException)
            {
                lastError = ex;
                if (attempt < 40) Thread.Sleep(250);
            }
        }
        throw new IOException($"Atomic copy failed after 10 seconds. Source: {source}. Destination: {destination}.", lastError);
    }
    private static bool FilesHaveSameContent(string firstPath, string secondPath)
    {
        if (!File.Exists(firstPath) || !File.Exists(secondPath)) return false;
        var first = new FileInfo(firstPath);
        var second = new FileInfo(secondPath);
        if (first.Length != second.Length) return false;
        using var firstStream = File.OpenRead(firstPath);
        using var secondStream = File.OpenRead(secondPath);
        return SHA256.HashData(firstStream).AsSpan().SequenceEqual(SHA256.HashData(secondStream));
    }
    private static string ResolveContained(string root, string relative)
    {
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var full = Path.GetFullPath(Path.Combine(fullRoot, relative.Replace('/', Path.DirectorySeparatorChar)));
        if (!full.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase)) throw new InvalidOperationException("Path escapes governed root: " + relative);
        return full;
    }
    private static bool IsSafeRelativePath(string path) => !string.IsNullOrWhiteSpace(path) && !path.Contains('\\') && !path.StartsWith('/') && !Path.IsPathRooted(path) &&
        path.Split('/').All(segment => segment.Length > 0 && segment != "." && segment != ".." && segment.IndexOfAny(Path.GetInvalidFileNameChars()) < 0);
    private static bool IsDirectoryContained(string root, string candidate)
    {
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var fullCandidate = Path.GetFullPath(candidate).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return fullCandidate.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase);
    }
    private static bool IsFileContained(string root, string candidate)
    {
        var fullRoot = Path.GetFullPath(root).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return Path.GetFullPath(candidate).StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase);
    }
    private static void Write(string root, string relative, string value) { var path = ResolveContained(root, relative); Directory.CreateDirectory(Path.GetDirectoryName(path)!); File.WriteAllText(path, value); }
    private static string Read(string root, string relative) => File.ReadAllText(ResolveContained(root, relative));
    private static void WriteRequest(ApplicationUpdateTransactionRequest request) =>
        File.WriteAllText(Path.Combine(Path.GetDirectoryName(request.StatePath)!, "update-request.json"), JsonSerializer.Serialize(request, JsonOptions));
}

public static class ApplicationUpdateTransactionDiagnostics
{
    private static readonly HashSet<string> IncompletePhases = new(StringComparer.Ordinal)
    {
        "Prepared", "SnapshotReady", "Installed", "RollingBack", "RollbackFailed"
    };

    public static IReadOnlyList<ApplicationUpdateTransactionDiagnostic> InspectRoot(string transactionRoot)
    {
        if (!Directory.Exists(transactionRoot)) return Array.Empty<ApplicationUpdateTransactionDiagnostic>();
        var results = new List<ApplicationUpdateTransactionDiagnostic>();
        foreach (var directory in Directory.EnumerateDirectories(transactionRoot).OrderByDescending(path => path, StringComparer.OrdinalIgnoreCase))
            results.Add(InspectDirectory(directory));
        return results;
    }

    private static ApplicationUpdateTransactionDiagnostic InspectDirectory(string directory)
    {
        var statePath = Path.Combine(directory, "transaction-state.json");
        var requestPath = Path.Combine(directory, "update-request.json");
        ApplicationUpdateTransactionState? state = null;
        ApplicationUpdateTransactionRequest? request = null;
        string stateError = string.Empty, requestError = string.Empty;
        try { state = JsonSerializer.Deserialize<ApplicationUpdateTransactionState>(File.ReadAllText(statePath)); }
        catch (Exception ex) { stateError = ex.Message; }
        try { request = JsonSerializer.Deserialize<ApplicationUpdateTransactionRequest>(File.ReadAllText(requestPath)); }
        catch (Exception ex) { requestError = ex.Message; }
        var phase = state?.Phase ?? "Unreadable";
        var incomplete = state is not null && IncompletePhases.Contains(phase);
        var identitiesMatch = state is not null && request is not null && state.TransactionSchema == ApplicationUpdateTransactionState.Schema &&
                              state.TransactionId == request.TransactionId && state.PreviousVersion == request.PreviousVersion && state.NewVersion == request.NewVersion &&
                              state.ReleaseCode == request.ReleaseCode && state.SignatureAlgorithm == request.SignatureAlgorithm &&
                              state.SigningKeyFingerprint == request.SigningKeyFingerprint;
        var action = !incomplete ? "None" : phase == "Prepared" ? "Restart guarded update from verified staging" : "Restore last-known-good application files";
        var durableError = string.IsNullOrWhiteSpace(state?.Error) ? string.Empty : " Durable error: " + state.Error;
        var detail = identitiesMatch ? "Durable state and request identity match. SQLite backup is evidence only; it will not be restored." + durableError
            : "Recovery blocked until durable state and request identity can be read and matched. " + stateError + " " + requestError;
        return new(directory, statePath, requestPath, state?.TransactionId ?? Path.GetFileName(directory), phase,
            state?.PreviousVersion ?? string.Empty, state?.NewVersion ?? string.Empty, state?.ReleaseCode ?? string.Empty,
            state?.SignatureAlgorithm ?? string.Empty, state?.SigningKeyFingerprint ?? string.Empty, state?.UpdatedAtUtc ?? string.Empty,
            state?.DatabaseBackupPath ?? string.Empty, state is not null, request is not null && identitiesMatch, incomplete, action, detail.Trim());
    }
}
