using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Windows.Automation;
using Microsoft.Data.Sqlite;

namespace FilamentDbApp.AutomationRunner;

internal static class Program
{
    private const string MarkerFileName = ".3dpiceland-disposable-profile.json";
    private static readonly TimeSpan ElementTimeout = TimeSpan.FromSeconds(45);
    private static readonly TimeSpan ReportTimeout = TimeSpan.FromMinutes(20);
    private static readonly List<StepResult> Steps = new();
    private static string CurrentScenario = "smoke";

    [STAThread]
    private static int Main(string[] args)
    {
        Process? application = null;
        string? root = null;
        string executable = string.Empty;
        string seedDatabase = string.Empty;
        string databasePath = string.Empty;
        string seedDatabaseHash = string.Empty;
        string databaseHashBefore = string.Empty;
        try
        {
            var options = RunnerOptions.Parse(args);
            CurrentScenario = options.Scenario;
            executable = IOPath.GetFullPath(options.ApplicationPath);
            seedDatabase = IOPath.GetFullPath(options.SeedDatabasePath);
            if (!IOFile.Exists(executable)) throw new FileNotFoundException("Application executable not found.", executable);
            if (!IOFile.Exists(seedDatabase)) throw new FileNotFoundException("Explicit seed database not found.", seedDatabase);

            root = CreateDisposableProfile(
                executable,
                seedDatabase,
                string.Equals(options.Scenario, "reports", StringComparison.Ordinal),
                out var markerPath,
                out databasePath);
            seedDatabaseHash = Sha256(seedDatabase);
            application = Process.Start(new ProcessStartInfo(executable)
            {
                UseShellExecute = false,
                WorkingDirectory = IOPath.GetDirectoryName(executable)!
            }.WithArgument("--automation-profile", markerPath))
                ?? throw new InvalidOperationException("Application process did not start.");

            var main = WaitForElement(
                AutomationElement.RootElement,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "MainWindow")),
                "Main window");
            Record("startup", true, $"Main window found for PID {application.Id}");
            AssertNoUnexpectedWindows(application.Id, "MainWindow");
            var baselineSnapshotPath = IOPath.Combine(root, "evidence", "database-before.sqlite");
            CreateConsistentSnapshot(databasePath, baselineSnapshotPath);
            databaseHashBefore = ComputeLogicalDatabaseHash(baselineSnapshotPath);
            Record("database-runtime-baseline", true, databaseHashBefore);

            var identity = FindById(main, "AutomationProfileIdentity");
            var identityName = identity.Current.Name;
            Require(identityName.Contains("AUTOMATION / DISPOSABLE", StringComparison.Ordinal),
                "Disposable profile identity is not visible.");
            Record("profile-identity", true, identityName);
            CaptureWindow(main, IOPath.Combine(root, "evidence", "main-window.png"));

            foreach (var tabId in new[]
                     {
                         "MaterialsTab", "TensileMeasurementsTab", "ImpactMeasurementsTab",
                         "StiffnessMeasurementsTab", "SettingsManagerTab", "ReportsTab"
                     })
            {
                SelectTab(main, tabId, application.Id);
                Record("navigate-" + tabId, true, "Tab selected by AutomationId");
            }

            Expand(FindById(main, "HelpMenu"), application.Id);
            Invoke(FindById(main, "OpenVerificationCenter"), application.Id);
            var verification = WaitForElement(
                AutomationElement.RootElement,
                new AndCondition(
                    new PropertyCondition(AutomationElement.ProcessIdProperty, application.Id),
                    new PropertyCondition(AutomationElement.AutomationIdProperty, "VerificationCenterWindow")),
                "Verification Center");
            AssertNoUnexpectedWindows(application.Id, "MainWindow", "VerificationCenterWindow");
            Record("verification-window", true, "Verification Center opened");
            Invoke(FindById(verification, "ExportAutomationVerificationEvidence"), application.Id);
            WaitForFile(
                IOPath.Combine(root, "evidence", "verification.json"),
                application.Id,
                "MainWindow",
                "VerificationCenterWindow");
            var verificationEvidence = JsonDocument.Parse(
                IOFile.ReadAllText(IOPath.Combine(root, "evidence", "verification.json")));
            Require(
                verificationEvidence.RootElement.TryGetProperty("Passed", out var passedElement) &&
                passedElement.GetBoolean(),
                "Exported Full Data Verification reported FAIL.");
            CaptureWindow(verification, IOPath.Combine(root, "evidence", "verification-center.png"));
            Record("verification-export", true, "TXT/JSON evidence exported");

            CloseWindow(verification, application.Id);
            if (string.Equals(options.Scenario, "reports", StringComparison.Ordinal))
            {
                SelectTab(main, "ReportsTab", application.Id);
                var outputFolder = FindById(main, "ReportOutputFolder");
                Require(
                    string.Equals(
                        outputFolder.GetCurrentPropertyValue(ValuePattern.ValueProperty)?.ToString(),
                        IOPath.Combine(root, "output"),
                        StringComparison.OrdinalIgnoreCase),
                    "Reports output folder is not bound to the disposable automation profile.");
                Invoke(FindById(main, "BuildPublicReportPackage"), application.Id);
                WaitForReportCompletion(main, application.Id);
                var artifactCount = ValidateReportArtifacts(root);
                CaptureWindow(main, IOPath.Combine(root, "evidence", "report-package.png"));
                Record("report-package", true, $"{artifactCount} catalog/root artifacts verified and hashed");
            }
            CloseWindow(main, application.Id);
            if (!application.WaitForExit(15000))
                throw new TimeoutException("Application did not complete controlled shutdown.");

            var finalSnapshotPath = IOPath.Combine(root, "evidence", "database-after.sqlite");
            CreateConsistentSnapshot(databasePath, finalSnapshotPath);
            var databaseHashAfter = ComputeLogicalDatabaseHash(finalSnapshotPath);
            Require(string.Equals(databaseHashBefore, databaseHashAfter, StringComparison.OrdinalIgnoreCase),
                "Disposable database bytes changed during the read-only smoke scenario.");
            Record("database-hash", true, databaseHashAfter);
            WriteResult(
                root,
                "PASS",
                executable,
                seedDatabase,
                seedDatabaseHash,
                databaseHashBefore,
                databaseHashAfter,
                null);
            return 0;
        }
        catch (Exception ex)
        {
            try
            {
                if (application is { HasExited: false })
                {
                    application.Kill(entireProcessTree: true);
                    application.WaitForExit(15000);
                }
            }
            catch
            {
            }
            var databaseHashAfter = TrySha256(databasePath);
            if (root is not null)
                WriteResult(
                    root,
                    "FAIL",
                    executable,
                    seedDatabase,
                    seedDatabaseHash,
                    databaseHashBefore,
                    databaseHashAfter,
                    ex.ToString());
            Console.Error.WriteLine(ex);
            return 1;
        }
    }

    private static string CreateDisposableProfile(
        string executable,
        string seedDatabase,
        bool reportGenerationAuthorized,
        out string markerPath,
        out string databasePath)
    {
        var allowedRoot = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-Automation");
        IODirectory.CreateDirectory(allowedRoot);
        var profileId = DateTime.UtcNow.ToString("yyyyMMddHHmmss") + "-" + Guid.NewGuid().ToString("N")[..8];
        var root = IOPath.Combine(allowedRoot, profileId);
        var databaseFolder = IOPath.Combine(root, "database");
        var preferencesFolder = IOPath.Combine(root, "preferences");
        var outputFolder = IOPath.Combine(root, "output");
        var evidenceFolder = IOPath.Combine(root, "evidence");
        foreach (var folder in new[] { databaseFolder, preferencesFolder, outputFolder, evidenceFolder })
            IODirectory.CreateDirectory(folder);

        databasePath = IOPath.Combine(databaseFolder, "filamentdb.sqlite");
        IOFile.Copy(seedDatabase, databasePath, overwrite: false);
        IOFile.Copy(
            seedDatabase,
            IOPath.Combine(databaseFolder, "3DPIceland-Automation-Seed-Evidence.bak"),
            overwrite: false);
        markerPath = IOPath.Combine(root, MarkerFileName);
        var manifest = new
        {
            profileId,
            rootPath = root,
            databaseFolder,
            preferencesFolder,
            outputFolder,
            evidenceFolder,
            expectedExecutableSha256 = Sha256(executable),
            productionAndFtpsBlocked = true,
            updatesBlocked = true,
            reportGenerationAuthorized
        };
        IOFile.WriteAllText(markerPath, JsonSerializer.Serialize(manifest, new JsonSerializerOptions
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        }), new UTF8Encoding(false));
        return root;
    }

    private static AutomationElement FindById(AutomationElement root, string automationId) =>
        WaitForElement(root, new PropertyCondition(AutomationElement.AutomationIdProperty, automationId), automationId);

    private static AutomationElement WaitForElement(AutomationElement root, Condition condition, string label)
    {
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ElementTimeout)
        {
            var element = root.FindFirst(TreeScope.Descendants | TreeScope.Children, condition);
            if (element is not null) return element;
            Thread.Sleep(150);
        }
        throw new TimeoutException($"Timed out waiting for {label}.");
    }

    private static void SelectTab(AutomationElement main, string automationId, int processId)
    {
        var tab = FindById(main, automationId);
        DemandOwned(tab, processId);
        if (!tab.TryGetCurrentPattern(SelectionItemPattern.Pattern, out var pattern))
            throw new InvalidOperationException($"{automationId} does not expose SelectionItemPattern.");
        ((SelectionItemPattern)pattern).Select();
        Thread.Sleep(100);
        AssertNoUnexpectedWindows(processId, "MainWindow");
    }

    private static void Invoke(AutomationElement element, int processId)
    {
        DemandOwned(element, processId);
        if (!element.TryGetCurrentPattern(InvokePattern.Pattern, out var pattern))
            throw new InvalidOperationException($"{element.Current.AutomationId} does not expose InvokePattern.");
        ((InvokePattern)pattern).Invoke();
    }

    private static void Expand(AutomationElement element, int processId)
    {
        DemandOwned(element, processId);
        if (!element.TryGetCurrentPattern(ExpandCollapsePattern.Pattern, out var pattern))
            throw new InvalidOperationException($"{element.Current.AutomationId} does not expose ExpandCollapsePattern.");
        ((ExpandCollapsePattern)pattern).Expand();
    }

    private static void CloseWindow(AutomationElement window, int processId)
    {
        DemandOwned(window, processId);
        if (!window.TryGetCurrentPattern(WindowPattern.Pattern, out var pattern))
            throw new InvalidOperationException("Owned window does not expose WindowPattern.");
        ((WindowPattern)pattern).Close();
    }

    private static void DemandOwned(AutomationElement element, int processId)
    {
        if (element.Current.ProcessId != processId)
            throw new InvalidOperationException("Automation input target is outside the owned application process.");
    }

    private static void AssertNoUnexpectedWindows(int processId, params string[] allowedIds)
    {
        var allowed = allowedIds.ToHashSet(StringComparer.Ordinal);
        var windows = AutomationElement.RootElement.FindAll(
            TreeScope.Children,
            new PropertyCondition(AutomationElement.ProcessIdProperty, processId));
        foreach (AutomationElement window in windows)
        {
            var id = window.Current.AutomationId;
            if (!allowed.Contains(id))
                throw new InvalidOperationException(
                    $"Unexpected dialog/window blocked the run: AutomationId '{id}', name '{window.Current.Name}'.");
        }
    }

    private static void CaptureWindow(AutomationElement window, string path)
    {
        var bounds = window.Current.BoundingRectangle;
        if (bounds.IsEmpty || bounds.Width < 1 || bounds.Height < 1)
            throw new InvalidOperationException("Owned window has no capturable bounds.");
        using var bitmap = new Bitmap((int)Math.Ceiling(bounds.Width), (int)Math.Ceiling(bounds.Height));
        using var graphics = Graphics.FromImage(bitmap);
        graphics.CopyFromScreen((int)bounds.Left, (int)bounds.Top, 0, 0, bitmap.Size);
        bitmap.Save(path, ImageFormat.Png);
    }

    private static void WaitForFile(string path, int processId, params string[] allowedWindowIds)
    {
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ElementTimeout)
        {
            if (IOFile.Exists(path) && new FileInfo(path).Length > 0) return;
            AssertNoUnexpectedWindows(processId, allowedWindowIds);
            Thread.Sleep(100);
        }
        throw new TimeoutException("Timed out waiting for automation evidence.");
    }

    private static void WaitForReportCompletion(AutomationElement main, int processId)
    {
        var summary = FindById(main, "ReportExportSummary");
        var log = FindById(main, "ReportPreviewLog");
        var timer = Stopwatch.StartNew();
        while (timer.Elapsed < ReportTimeout)
        {
            AssertNoUnexpectedWindows(processId, "MainWindow", "ReportPrintHostWindow");
            var status = summary.Current.Name;
            if (status.StartsWith("Public report package built:", StringComparison.Ordinal))
                return;
            if (status.Contains("failed", StringComparison.OrdinalIgnoreCase))
                throw new InvalidOperationException(
                    $"Public report package failed: {log.GetCurrentPropertyValue(ValuePattern.ValueProperty)}");
            Thread.Sleep(250);
        }
        throw new TimeoutException("Timed out waiting for the public report package.");
    }

    private static int ValidateReportArtifacts(string profileRoot)
    {
        const string previewFolder = "public-report-preview";
        const string screenThemeMarker = "3DP-PUBLIC-REPORT-SCREEN-THEME-v42.14-r3";
        var root = IOPath.GetFullPath(IOPath.Combine(profileRoot, "output", previewFolder));
        var catalogPath = IOPath.Combine(root, "report-catalog.json");
        var catalog = JsonDocument.Parse(IOFile.ReadAllText(catalogPath));
        var reports = catalog.RootElement.GetProperty("publicData").GetProperty("Reports").EnumerateArray().ToList();
        var types = reports.Select(report => report.GetProperty("ReportType").GetString() ?? string.Empty)
            .ToHashSet(StringComparer.Ordinal);
        foreach (var required in new[]
                 {
                     "material-summary", "material-engineering", "comparison",
                     "manufacturer", "test-session", "printing-recommendation"
                 })
            Require(types.Contains(required), $"Report catalog is missing required type {required}.");

        var relativePaths = reports
            .SelectMany(report => new[]
            {
                report.GetProperty("Html").GetString(),
                report.GetProperty("Pdf").GetString(),
                report.GetProperty("Metadata").GetString()
            })
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .Select(path => path!)
            .Concat(new[]
            {
                "index.html", "manifest.txt", "report-catalog.json", "source-fingerprint.json",
                "assets/3dp-iceland-labs-logo-pdf.jpg", "assets/3dp-iceland-labs-icon.ico"
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(path => path, StringComparer.Ordinal)
            .ToList();
        var rootPrefix = root.TrimEnd(IOPath.DirectorySeparatorChar) + IOPath.DirectorySeparatorChar;
        var artifacts = new List<ArtifactEvidence>();
        foreach (var relative in relativePaths)
        {
            Require(
                !IOPath.IsPathRooted(relative) &&
                !relative.Contains('\\') &&
                relative.Split('/').All(segment => segment.Length > 0 && segment is not "." and not ".."),
                $"Unsafe report route: {relative}");
            var fullPath = IOPath.GetFullPath(
                IOPath.Combine(root, relative.Replace('/', IOPath.DirectorySeparatorChar)));
            Require(fullPath.StartsWith(rootPrefix, StringComparison.OrdinalIgnoreCase), $"Report route escaped output root: {relative}");
            Require(IOFile.Exists(fullPath), $"Report artifact is missing: {relative}");
            var info = new FileInfo(fullPath);
            Require(info.Length > 0, $"Report artifact is empty: {relative}");
            if (relative.EndsWith(".html", StringComparison.OrdinalIgnoreCase))
            {
                var html = IOFile.ReadAllText(fullPath);
                Require(
                    relative == "index.html"
                        ? html.Contains("3DP-PUBLIC-REPORT-PORTFOLIO-v42.8", StringComparison.Ordinal)
                        : html.Contains(screenThemeMarker, StringComparison.Ordinal),
                    $"Report HTML marker is missing: {relative}");
            }
            if (relative.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                using var stream = IOFile.OpenRead(fullPath);
                var header = new byte[5];
                Require(stream.Read(header, 0, header.Length) == header.Length &&
                        Encoding.ASCII.GetString(header) == "%PDF-", $"Invalid PDF header: {relative}");
            }
            if (relative.EndsWith(".json", StringComparison.OrdinalIgnoreCase))
                using (JsonDocument.Parse(IOFile.ReadAllText(fullPath))) { }
            artifacts.Add(new ArtifactEvidence(relative, info.Length, Sha256(fullPath)));
        }

        var evidenceFolder = IOPath.Combine(profileRoot, "evidence");
        var representative = reports
            .Where(report =>
                report.GetProperty("ReportType").GetString() is "material-summary" or "material-engineering")
            .Take(2)
            .SelectMany(report => new[]
            {
                report.GetProperty("Html").GetString(),
                report.GetProperty("Pdf").GetString()
            })
            .Where(path => !string.IsNullOrWhiteSpace(path))
            .ToList();
        var result = new
        {
            schema = "3dpiceland-automation-report-evidence-v1",
            status = "PASS",
            manualVisualReview = "REQUIRED",
            previewRoot = root,
            reportTypes = types.OrderBy(type => type).ToList(),
            catalogEntries = reports.Count,
            artifacts,
            representativeReviewPaths = representative
        };
        IOFile.WriteAllText(
            IOPath.Combine(evidenceFolder, "report-artifacts.json"),
            JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true }),
            new UTF8Encoding(false));
        IOFile.WriteAllLines(
            IOPath.Combine(evidenceFolder, "report-artifacts.txt"),
            new[]
            {
                "Status: PASS",
                "Manual visual review: REQUIRED",
                $"Catalog entries: {reports.Count}",
                $"Verified artifacts: {artifacts.Count}"
            }.Concat(artifacts.Select(item => $"{item.Sha256} {item.Bytes} {item.RelativePath}")),
            new UTF8Encoding(false));
        return artifacts.Count;
    }

    private static string Sha256(string path) =>
        Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(path)));

    private static string TrySha256(string path)
    {
        if (!IOFile.Exists(path)) return string.Empty;
        try
        {
            return Sha256(path);
        }
        catch
        {
            return "UNAVAILABLE-FILE-LOCKED";
        }
    }

    private static void CreateConsistentSnapshot(string sourcePath, string destinationPath)
    {
        if (IOFile.Exists(destinationPath)) IOFile.Delete(destinationPath);
        using var source = new SqliteConnection($"Data Source={sourcePath};Mode=ReadOnly;Pooling=False");
        using var destination = new SqliteConnection($"Data Source={destinationPath};Pooling=False");
        source.Open();
        destination.Open();
        source.BackupDatabase(destination);
    }

    private static string ComputeLogicalDatabaseHash(string databasePath)
    {
        using var canonical = new MemoryStream();
        using var writer = new BinaryWriter(canonical, Encoding.UTF8, leaveOpen: true);
        using var connection = new SqliteConnection($"Data Source={databasePath};Mode=ReadOnly;Pooling=False");
        connection.Open();

        var tables = new List<(string Name, string Sql)>();
        using (var schema = connection.CreateCommand())
        {
            schema.CommandText = """
                SELECT name, COALESCE(sql, '')
                FROM sqlite_schema
                WHERE type = 'table' AND (name NOT LIKE 'sqlite_%' OR name = 'sqlite_sequence')
                ORDER BY name COLLATE BINARY;
                """;
            using var reader = schema.ExecuteReader();
            while (reader.Read()) tables.Add((reader.GetString(0), reader.GetString(1)));
        }

        writer.Write(tables.Count);
        foreach (var table in tables)
        {
            writer.Write(table.Name);
            writer.Write(table.Sql);
            var columns = new List<string>();
            using (var info = connection.CreateCommand())
            {
                info.CommandText = $"PRAGMA table_info({QuoteIdentifier(table.Name)});";
                using var reader = info.ExecuteReader();
                while (reader.Read()) columns.Add(reader.GetString(1));
            }

            writer.Write(columns.Count);
            foreach (var column in columns) writer.Write(column);
            using var rows = connection.CreateCommand();
            var columnList = string.Join(", ", columns.Select(QuoteIdentifier));
            rows.CommandText = columns.Count == 0
                ? $"SELECT 1 FROM {QuoteIdentifier(table.Name)};"
                : $"SELECT {columnList} FROM {QuoteIdentifier(table.Name)} ORDER BY {columnList};";
            using var rowReader = rows.ExecuteReader();
            while (rowReader.Read())
            {
                writer.Write((byte)0x52);
                for (var index = 0; index < rowReader.FieldCount; index++)
                    WriteCanonicalValue(writer, rowReader.GetValue(index));
            }
        }

        writer.Flush();
        canonical.Position = 0;
        return Convert.ToHexString(SHA256.HashData(canonical));
    }

    private static void WriteCanonicalValue(BinaryWriter writer, object value)
    {
        switch (value)
        {
            case DBNull:
                writer.Write((byte)0);
                break;
            case long integer:
                writer.Write((byte)1);
                writer.Write(integer);
                break;
            case double real:
                writer.Write((byte)2);
                writer.Write(BitConverter.DoubleToInt64Bits(real));
                break;
            case string text:
                writer.Write((byte)3);
                writer.Write(text);
                break;
            case byte[] blob:
                writer.Write((byte)4);
                writer.Write(blob.Length);
                writer.Write(blob);
                break;
            default:
                throw new InvalidOperationException($"Unsupported SQLite value type: {value.GetType().FullName}");
        }
    }

    private static string QuoteIdentifier(string identifier) =>
        "\"" + identifier.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";

    private static void Require(bool condition, string message)
    {
        if (!condition) throw new InvalidOperationException(message);
    }

    private static void Record(string name, bool passed, string detail) =>
        Steps.Add(new StepResult(name, passed ? "PASS" : "FAIL", detail, DateTimeOffset.UtcNow));

    private static void WriteResult(
        string root,
        string status,
        string executable,
        string seedDatabase,
        string seedDatabaseHash,
        string databaseHashBefore,
        string databaseHashAfter,
        string? error)
    {
        var evidence = IOPath.Combine(root, "evidence");
        IODirectory.CreateDirectory(evidence);
        var result = new
        {
            schema = "3dpiceland-automation-run-v1",
            status,
            scenario = CurrentScenario,
            safetyPolicy = new
            {
                productionBlocked = true,
                ftpsBlocked = true,
                updatesBlocked = true,
                ownerDatabaseAutoSelection = false,
                unexpectedDialogsBlocked = true,
                inputConfinedToOwnedProcess = true,
                reportGenerationAuthorized = string.Equals(CurrentScenario, "reports", StringComparison.Ordinal)
            },
            executable,
            seedDatabase,
            seedDatabaseHash,
            databaseHashBefore,
            databaseHashAfter,
            steps = Steps,
            error,
            completedAtUtc = DateTimeOffset.UtcNow
        };
        var json = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true });
        IOFile.WriteAllText(IOPath.Combine(evidence, "run-result.json"), json, new UTF8Encoding(false));
        IOFile.WriteAllLines(
            IOPath.Combine(evidence, "run-result.txt"),
            new[] { $"Status: {status}" }.Concat(Steps.Select(step => $"{step.Status} {step.Name}: {step.Detail}"))
                .Append(error ?? string.Empty),
            new UTF8Encoding(false));
    }

    private sealed record StepResult(string Name, string Status, string Detail, DateTimeOffset AtUtc);
    private sealed record ArtifactEvidence(string RelativePath, long Bytes, string Sha256);

    private sealed record RunnerOptions(string ApplicationPath, string SeedDatabasePath, string Scenario)
    {
        public static RunnerOptions Parse(string[] args)
        {
            string Required(string name)
            {
                var index = Array.IndexOf(args, name);
                if (index < 0 || index + 1 >= args.Length)
                    throw new ArgumentException($"Required argument missing: {name}");
                return args[index + 1];
            }
            var scenarioIndex = Array.IndexOf(args, "--scenario");
            var scenario = scenarioIndex >= 0 && scenarioIndex + 1 < args.Length
                ? args[scenarioIndex + 1].Trim().ToLowerInvariant()
                : "smoke";
            if (scenario is not ("smoke" or "reports"))
                throw new ArgumentException("--scenario must be smoke or reports.");
            return new RunnerOptions(Required("--app"), Required("--seed-database"), scenario);
        }
    }

    private static ProcessStartInfo WithArgument(this ProcessStartInfo start, string name, string value)
    {
        start.ArgumentList.Add(name);
        start.ArgumentList.Add(value);
        return start;
    }
}
