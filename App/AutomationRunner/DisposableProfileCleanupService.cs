using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.IO;

namespace FilamentDbApp.AutomationRunner;

internal static class DisposableProfileCleanupService
{
    private const string MarkerFileName = ".3dpiceland-disposable-profile.json";
    private const string PlanSchema = "3dpiceland-automation-cleanup-plan-v1";
    private static readonly HashSet<string> Scenarios =
    [
        "smoke", "reports", "crud", "landed-cost", "migration", "recovery", "updater", "clean"
    ];

    public static int CreateDryRun(IReadOnlySet<string> pinnedProfileIds)
    {
        var root = ResolveAutomationRoot();
        var entries = Inventory(root, pinnedProfileIds);
        var plan = CreatePlan(root, entries);
        var evidenceRoot = IOPath.Combine(root, "cleanup-evidence");
        IODirectory.CreateDirectory(evidenceRoot);
        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var path = IOPath.Combine(evidenceRoot, $"cleanup-plan-{stamp}-{plan.PlanSha256[..12]}.json");
        WritePlan(path, plan);
        WriteTextSummary(IOPath.ChangeExtension(path, ".txt"), plan, applied: false, []);
        Console.WriteLine($"Dry-run cleanup plan: {path}");
        Console.WriteLine($"Plan SHA-256: {plan.PlanSha256}");
        var removeCount = entries.Count(item => item.Action == "REMOVE");
        var retainCount = entries.Count(item => item.Action == "RETAIN");
        Console.WriteLine($"Remove: {removeCount}; retain: {retainCount}");
        return 0;
    }

    public static int Apply(string planPath, string expectedPlanSha256)
    {
        var root = ResolveAutomationRoot();
        var fullPlanPath = IOPath.GetFullPath(planPath);
        var evidenceRoot = IOPath.GetFullPath(IOPath.Combine(root, "cleanup-evidence"));
        RequireStrictDescendant(fullPlanPath, evidenceRoot, "Cleanup plan");
        if (!IOFile.Exists(fullPlanPath))
            throw new FileNotFoundException("Cleanup plan not found.", fullPlanPath);
        var plan = JsonSerializer.Deserialize<CleanupPlan>(
            IOFile.ReadAllText(fullPlanPath),
            JsonOptions()) ?? throw new InvalidDataException("Cleanup plan is empty.");
        if (!string.Equals(plan.Schema, PlanSchema, StringComparison.Ordinal) ||
            !string.Equals(plan.Mode, "DRY-RUN", StringComparison.Ordinal) ||
            !string.Equals(IOPath.GetFullPath(plan.RootPath), root, StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("Cleanup plan schema or root does not match the governed automation root.");
        var computedHash = ComputePlanHash(plan with { PlanSha256 = string.Empty });
        if (!FixedEquals(computedHash, plan.PlanSha256) ||
            !FixedEquals(computedHash, expectedPlanSha256))
            throw new InvalidDataException("Cleanup plan hash is missing, invalid or was not explicitly reviewed.");
        ValidateEntirePlan(root, plan);

        var removed = new List<string>();
        var errors = new List<string>();
        foreach (var entry in plan.Entries.Where(item => item.Action == "REMOVE"))
        {
            try
            {
                ValidateRemovalCandidate(root, entry);
                var quarantine = IOPath.Combine(
                    root,
                    $".cleanup-quarantine-{entry.ProfileId}-{Guid.NewGuid():N}");
                if (IODirectory.Exists(quarantine) || IOFile.Exists(quarantine))
                    throw new InvalidOperationException("Cleanup quarantine path already exists.");
                IODirectory.Move(entry.ProfilePath, quarantine);
                RequireStrictDescendant(quarantine, root, "Cleanup quarantine");
                if (HasReparsePoint(quarantine) ||
                    !FixedEquals(HashTree(quarantine), entry.TreeSha256))
                    throw new InvalidOperationException(
                        "Quarantined profile changed before removal; manual review is required.");
                IODirectory.Delete(quarantine, recursive: true);
                removed.Add(entry.ProfileId);
            }
            catch (Exception ex)
            {
                errors.Add($"{entry.ProfileId}: {ex.Message}");
                break;
            }
        }

        var stamp = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
        var resultPath = IOPath.Combine(evidenceRoot, $"cleanup-apply-{stamp}-{computedHash[..12]}.txt");
        WriteTextSummary(resultPath, plan, applied: true, removed, errors);
        Console.WriteLine($"Cleanup apply evidence: {resultPath}");
        if (errors.Count > 0)
        {
            Console.Error.WriteLine(errors[0]);
            return 1;
        }
        return 0;
    }

    public static int RunSyntheticSelfTest()
    {
        var testParent = IOPath.GetFullPath(
            IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-Automation-CleanupTests"));
        IODirectory.CreateDirectory(testParent);
        var root = IOPath.Combine(testParent, Guid.NewGuid().ToString("N"));
        IODirectory.CreateDirectory(root);
        try
        {
            CreateSyntheticProfile(root, "20260101010101-aaaaaaaa", "smoke", "PASS",
                new DateTimeOffset(2026, 1, 1, 1, 1, 1, TimeSpan.Zero));
            CreateSyntheticProfile(root, "20260102020202-bbbbbbbb", "smoke", "PASS",
                new DateTimeOffset(2026, 1, 2, 2, 2, 2, TimeSpan.Zero));
            CreateSyntheticProfile(root, "20260103030303-cccccccc", "reports", "FAIL",
                new DateTimeOffset(2026, 1, 3, 3, 3, 3, TimeSpan.Zero));
            CreateSyntheticProfile(root, "20260104040404-dddddddd", "crud", null, null);
            var before = HashTree(root);
            var entries = Inventory(root, new HashSet<string>(StringComparer.Ordinal));
            if (HashTree(root) != before)
                throw new InvalidOperationException("Cleanup dry-run changed synthetic profile bytes.");
            RequireSynthetic(entries, "20260101010101-aaaaaaaa", "REMOVE");
            RequireSynthetic(entries, "20260102020202-bbbbbbbb", "RETAIN");
            RequireSynthetic(entries, "20260103030303-cccccccc", "RETAIN");
            RequireSynthetic(entries, "20260104040404-dddddddd", "RETAIN");
            var plan = CreatePlan(root, entries);
            var computed = ComputePlanHash(plan with { PlanSha256 = string.Empty });
            if (!FixedEquals(computed, plan.PlanSha256))
                throw new InvalidOperationException("Synthetic cleanup plan hash is invalid.");
            var removal = entries.Single(item => item.Action == "REMOVE");
            ValidateRemovalCandidate(root, removal);
            IODirectory.Delete(removal.ProfilePath, recursive: true);
            if (IODirectory.Exists(removal.ProfilePath) ||
                entries.Where(item => item.Action == "RETAIN")
                    .Any(item => !IODirectory.Exists(item.ProfilePath)) ||
                !IODirectory.Exists(root))
                throw new InvalidOperationException("Synthetic apply crossed its reviewed removal boundary.");
            Console.WriteLine("PASS cleanup synthetic classifier, dry-run, plan hash and bounded apply");
            return 0;
        }
        finally
        {
            var fullRoot = IOPath.GetFullPath(root);
            RequireStrictDescendant(fullRoot, testParent, "Synthetic cleanup root");
            if (IODirectory.Exists(fullRoot) && !HasReparsePoint(fullRoot))
                IODirectory.Delete(fullRoot, recursive: true);
        }
    }

    private static List<CleanupEntry> Inventory(string root, IReadOnlySet<string> pinnedProfileIds)
    {
        var candidates = new List<CleanupEntry>();
        foreach (var path in IODirectory.GetDirectories(root))
        {
            var fullPath = IOPath.GetFullPath(path);
            if (string.Equals(IOPath.GetFileName(fullPath), "cleanup-evidence", StringComparison.OrdinalIgnoreCase))
                continue;
            RequireStrictDescendant(fullPath, root, "Profile");
            try
            {
                candidates.Add(InspectProfile(fullPath, pinnedProfileIds));
            }
            catch
            {
                candidates.Add(new CleanupEntry(
                    IOPath.GetFileName(fullPath),
                    fullPath,
                    string.Empty,
                    "UNSAFE",
                    null,
                    "RETAIN",
                    "profile-unreadable-or-locked",
                    string.Empty,
                    string.Empty,
                    string.Empty));
            }
        }

        var newestPassTimes = candidates
            .Where(item => item.Classification == "PASS")
            .GroupBy(item => item.Scenario, StringComparer.Ordinal)
            .ToDictionary(
                group => group.Key,
                group => group.Max(item => item.CompletedAtUtc),
                StringComparer.Ordinal);
        return candidates
            .Select(item =>
            {
                if (item.Action == "RETAIN") return item;
                if (newestPassTimes.TryGetValue(item.Scenario, out var newest) &&
                    item.CompletedAtUtc == newest)
                    return item with { Action = "RETAIN", Reason = "latest-valid-pass-for-scenario" };
                return item with { Action = "REMOVE", Reason = "obsolete-valid-unpinned-pass" };
            })
            .OrderBy(item => item.ProfileId, StringComparer.Ordinal)
            .ToList();
    }

    private static CleanupEntry InspectProfile(string profilePath, IReadOnlySet<string> pinnedProfileIds)
    {
        var profileId = IOPath.GetFileName(profilePath);
        var markerPath = IOPath.Combine(profilePath, MarkerFileName);
        var resultPath = IOPath.Combine(profilePath, "evidence", "run-result.json");
        var markerHash = IOFile.Exists(markerPath) ? Sha256(markerPath) : string.Empty;
        var resultHash = IOFile.Exists(resultPath) ? Sha256(resultPath) : string.Empty;
        var treeHash = HasReparsePoint(profilePath) ? string.Empty : HashTree(profilePath);
        CleanupEntry Retain(string classification, string reason) =>
            new(profileId, profilePath, string.Empty, classification, null, "RETAIN", reason,
                markerHash, resultHash, treeHash);

        if (HasReparsePoint(profilePath)) return Retain("UNSAFE", "reparse-point-present");
        if (!IsSafeProfileId(profileId)) return Retain("MALFORMED", "invalid-profile-id");
        if (pinnedProfileIds.Contains(profileId))
            return Retain("PINNED", "explicit-acceptance-dependency");
        if (!IOFile.Exists(markerPath)) return Retain("ABORTED", "manifest-missing");
        try
        {
            using var marker = JsonDocument.Parse(IOFile.ReadAllText(markerPath));
            if (!marker.RootElement.TryGetProperty("profileId", out var markerId) ||
                !string.Equals(markerId.GetString(), profileId, StringComparison.Ordinal) ||
                !marker.RootElement.TryGetProperty("rootPath", out var markerRoot) ||
                !string.Equals(
                    IOPath.GetFullPath(markerRoot.GetString() ?? string.Empty),
                    profilePath,
                    StringComparison.OrdinalIgnoreCase))
                return Retain("MALFORMED", "manifest-identity-mismatch");
        }
        catch
        {
            return Retain("MALFORMED", "manifest-unreadable");
        }
        if (!IOFile.Exists(resultPath)) return Retain("ABORTED", "run-result-missing");
        try
        {
            using var result = JsonDocument.Parse(IOFile.ReadAllText(resultPath));
            var root = result.RootElement;
            var schema = root.GetProperty("schema").GetString();
            var status = root.GetProperty("status").GetString();
            var scenario = root.GetProperty("scenario").GetString() ?? string.Empty;
            var completedAt = root.GetProperty("completedAtUtc").GetDateTimeOffset();
            var profileCreatedAt = DateTimeOffset.ParseExact(
                profileId[..14],
                "yyyyMMddHHmmss",
                System.Globalization.CultureInfo.InvariantCulture,
                System.Globalization.DateTimeStyles.AssumeUniversal);
            if (completedAt < profileCreatedAt ||
                completedAt > DateTimeOffset.UtcNow.AddMinutes(5))
                return Retain("MALFORMED", "result-timestamp-implausible");
            var safety = root.GetProperty("safetyPolicy");
            var safetyReady =
                safety.GetProperty("productionBlocked").GetBoolean() &&
                safety.GetProperty("ftpsBlocked").GetBoolean() &&
                safety.GetProperty("updatesBlocked").GetBoolean() &&
                !safety.GetProperty("ownerDatabaseAutoSelection").GetBoolean();
            if (!string.Equals(schema, "3dpiceland-automation-run-v1", StringComparison.Ordinal) ||
                !Scenarios.Contains(scenario) || !safetyReady)
                return Retain("MALFORMED", "result-contract-invalid");
            if (string.Equals(status, "FAIL", StringComparison.Ordinal))
                return new(profileId, profilePath, scenario, "FAIL", completedAt, "RETAIN",
                    "failed-run", markerHash, resultHash, treeHash);
            if (!string.Equals(status, "PASS", StringComparison.Ordinal))
                return Retain("MALFORMED", "unknown-result-status");
            return new(profileId, profilePath, scenario, "PASS", completedAt, "CANDIDATE",
                "valid-pass-awaiting-retention-classification", markerHash, resultHash, treeHash);
        }
        catch
        {
            return Retain("MALFORMED", "run-result-unreadable");
        }
    }

    private static CleanupPlan CreatePlan(string root, IReadOnlyList<CleanupEntry> entries)
    {
        var plan = new CleanupPlan(
            PlanSchema,
            root,
            DateTimeOffset.UtcNow,
            "DRY-RUN",
            entries,
            string.Empty);
        return plan with { PlanSha256 = ComputePlanHash(plan) };
    }

    private static void ValidateRemovalCandidate(string root, CleanupEntry entry)
    {
        if (entry.Action != "REMOVE" || entry.Classification != "PASS")
            throw new InvalidOperationException("Only reviewed obsolete PASS entries may be removed.");
        var fullPath = IOPath.GetFullPath(entry.ProfilePath);
        RequireStrictDescendant(fullPath, root, "Removal candidate");
        if (!string.Equals(IOPath.GetFileName(fullPath), entry.ProfileId, StringComparison.Ordinal) ||
            !IODirectory.Exists(fullPath))
            throw new InvalidOperationException("Removal candidate identity changed.");
        if (HasReparsePoint(fullPath))
            throw new InvalidOperationException("Removal candidate contains a reparse point.");
        var markerPath = IOPath.Combine(fullPath, MarkerFileName);
        var resultPath = IOPath.Combine(fullPath, "evidence", "run-result.json");
        if (!IOFile.Exists(markerPath) || !IOFile.Exists(resultPath) ||
            !FixedEquals(Sha256(markerPath), entry.MarkerSha256) ||
            !FixedEquals(Sha256(resultPath), entry.ResultSha256) ||
            !FixedEquals(HashTree(fullPath), entry.TreeSha256))
            throw new InvalidOperationException("Removal candidate drifted after dry-run review.");
    }

    private static void ValidateEntirePlan(string root, CleanupPlan plan)
    {
        if (plan.Entries.Count == 0) return;
        var ids = new HashSet<string>(StringComparer.Ordinal);
        var paths = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var entry in plan.Entries)
        {
            if (!ids.Add(entry.ProfileId) || !paths.Add(IOPath.GetFullPath(entry.ProfilePath)))
                throw new InvalidDataException("Cleanup plan contains duplicate profile identities or paths.");
            RequireStrictDescendant(IOPath.GetFullPath(entry.ProfilePath), root, "Planned profile");
            if (!string.Equals(IOPath.GetFileName(entry.ProfilePath), entry.ProfileId, StringComparison.Ordinal) ||
                entry.Action is not ("REMOVE" or "RETAIN"))
                throw new InvalidDataException("Cleanup plan contains an invalid profile identity or action.");
            if (entry.Action == "REMOVE" &&
                (entry.Classification != "PASS" ||
                 entry.Reason != "obsolete-valid-unpinned-pass" ||
                 string.IsNullOrWhiteSpace(entry.TreeSha256)))
                throw new InvalidDataException("Cleanup plan contains an unauthorized removal entry.");
        }
        foreach (var entry in plan.Entries.Where(item => item.Action == "REMOVE"))
            ValidateRemovalCandidate(root, entry);
    }

    private static bool HasReparsePoint(string root)
    {
        var pending = new Stack<string>();
        pending.Push(root);
        while (pending.Count > 0)
        {
            var current = pending.Pop();
            var attributes = IOFile.GetAttributes(current);
            if ((attributes & FileAttributes.ReparsePoint) != 0) return true;
            foreach (var entry in IODirectory.EnumerateFileSystemEntries(current))
            {
                var entryAttributes = IOFile.GetAttributes(entry);
                if ((entryAttributes & FileAttributes.ReparsePoint) != 0) return true;
                if ((entryAttributes & FileAttributes.Directory) != 0) pending.Push(entry);
            }
        }
        return false;
    }

    private static void CreateSyntheticProfile(
        string root,
        string profileId,
        string scenario,
        string? status,
        DateTimeOffset? completedAtUtc)
    {
        var profilePath = IOPath.Combine(root, profileId);
        var evidencePath = IOPath.Combine(profilePath, "evidence");
        IODirectory.CreateDirectory(evidencePath);
        IOFile.WriteAllText(
            IOPath.Combine(profilePath, MarkerFileName),
            JsonSerializer.Serialize(new { profileId, rootPath = profilePath }, JsonOptions()),
            new UTF8Encoding(false));
        if (status is null) return;
        var result = new
        {
            schema = "3dpiceland-automation-run-v1",
            status,
            scenario,
            safetyPolicy = new
            {
                productionBlocked = true,
                ftpsBlocked = true,
                updatesBlocked = true,
                ownerDatabaseAutoSelection = false
            },
            completedAtUtc
        };
        IOFile.WriteAllText(
            IOPath.Combine(evidencePath, "run-result.json"),
            JsonSerializer.Serialize(result, JsonOptions()),
            new UTF8Encoding(false));
    }

    private static void RequireSynthetic(
        IReadOnlyCollection<CleanupEntry> entries,
        string profileId,
        string action)
    {
        var entry = entries.Single(item => item.ProfileId == profileId);
        if (!string.Equals(entry.Action, action, StringComparison.Ordinal))
            throw new InvalidOperationException(
                $"Synthetic profile {profileId} expected {action}, got {entry.Action}: {entry.Reason}.");
    }

    private static string HashTree(string root)
    {
        using var hash = IncrementalHash.CreateHash(HashAlgorithmName.SHA256);
        foreach (var path in IODirectory.EnumerateFileSystemEntries(root, "*", SearchOption.AllDirectories)
                     .OrderBy(path => path, StringComparer.Ordinal))
        {
            var relative = IOPath.GetRelativePath(root, path);
            var attributes = IOFile.GetAttributes(path);
            if ((attributes & FileAttributes.ReparsePoint) != 0)
                throw new InvalidOperationException("Profile tree contains a reparse point.");
            var isDirectory = (attributes & FileAttributes.Directory) != 0;
            hash.AppendData(Encoding.UTF8.GetBytes((isDirectory ? "D:" : "F:") + relative));
            if (!isDirectory)
            {
                var info = new FileInfo(path);
                hash.AppendData(BitConverter.GetBytes(info.Length));
                hash.AppendData(IOFile.ReadAllBytes(path));
            }
        }
        return Convert.ToHexString(hash.GetHashAndReset());
    }

    private static string ResolveAutomationRoot()
    {
        var root = IOPath.GetFullPath(IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-Automation"))
            .TrimEnd(IOPath.DirectorySeparatorChar);
        IODirectory.CreateDirectory(root);
        if ((IOFile.GetAttributes(root) & FileAttributes.ReparsePoint) != 0)
            throw new InvalidOperationException("Automation root must not be a reparse point.");
        return root;
    }

    private static void RequireStrictDescendant(string path, string root, string label)
    {
        var prefix = root.TrimEnd(IOPath.DirectorySeparatorChar) + IOPath.DirectorySeparatorChar;
        if (!path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(path.TrimEnd(IOPath.DirectorySeparatorChar),
                root.TrimEnd(IOPath.DirectorySeparatorChar),
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException($"{label} is outside the governed root.");
    }

    private static bool IsSafeProfileId(string value) =>
        value.Length == 23 &&
        value[14] == '-' &&
        value[..14].All(char.IsDigit) &&
        value[15..].All(character => char.IsAsciiHexDigit(character));

    private static string ComputePlanHash(CleanupPlan plan)
    {
        var canonical = JsonSerializer.Serialize(plan with { PlanSha256 = string.Empty }, JsonOptions());
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    private static string Sha256(string path)
    {
        using var stream = IOFile.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static bool FixedEquals(string left, string right)
    {
        try
        {
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(left),
                Convert.FromHexString(right));
        }
        catch
        {
            return false;
        }
    }

    private static void WritePlan(string path, CleanupPlan plan) =>
        IOFile.WriteAllText(
            path,
            JsonSerializer.Serialize(plan, JsonOptions()),
            new UTF8Encoding(false));

    private static void WriteTextSummary(
        string path,
        CleanupPlan plan,
        bool applied,
        IReadOnlyCollection<string> removed,
        IReadOnlyCollection<string>? errors = null)
    {
        var lines = new List<string>
        {
            $"Schema: {plan.Schema}",
            $"Mode: {(applied ? "APPLY" : "DRY-RUN")}",
            $"Root: {plan.RootPath}",
            $"Plan SHA-256: {plan.PlanSha256}",
            $"Removed: {removed.Count}",
            $"Errors: {errors?.Count ?? 0}"
        };
        lines.AddRange(plan.Entries.Select(item =>
            $"{item.Action} {item.ProfileId} {item.Classification} {item.Scenario} {item.Reason}"));
        if (errors is not null) lines.AddRange(errors.Select(error => "ERROR " + error));
        IOFile.WriteAllLines(path, lines, new UTF8Encoding(false));
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private sealed record CleanupPlan(
        string Schema,
        string RootPath,
        DateTimeOffset GeneratedAtUtc,
        string Mode,
        IReadOnlyList<CleanupEntry> Entries,
        string PlanSha256);

    private sealed record CleanupEntry(
        string ProfileId,
        string ProfilePath,
        string Scenario,
        string Classification,
        DateTimeOffset? CompletedAtUtc,
        string Action,
        string Reason,
        string MarkerSha256,
        string ResultSha256,
        string TreeSha256);
}
