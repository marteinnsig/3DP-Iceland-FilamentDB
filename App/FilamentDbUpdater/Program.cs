using FilamentDbApp.UpdateCore;
using System.Diagnostics;
using System.Text.Json;

if (args.Length == 1 && args[0] == "--self-test")
{
    var verification = ApplicationUpdateTransactionEngine.RunContractVerification();
    Console.WriteLine(verification.Detail);
    return verification.Passed ? 0 : 1;
}
if (args.Length == 2 && args[0] == "--apply")
{
    var requestPath = Path.GetFullPath(args[1]);
    var request = JsonSerializer.Deserialize<ApplicationUpdateTransactionRequest>(File.ReadAllText(requestPath))
        ?? throw new InvalidOperationException("Updater request could not be read.");
    WaitForApplicationExit(request.WaitForProcessId, TimeSpan.FromSeconds(60));
    Process? updatedProcess = null;
    var result = new ApplicationUpdateTransactionEngine().Execute(request, () => LaunchAndAwaitHealth(request, out updatedProcess));
    Console.WriteLine(result.Detail);
    if (!result.Succeeded)
    {
        TryStop(updatedProcess);
        var restoredExecutable = Path.Combine(request.LiveDirectory, request.ApplicationRelativePath.Replace('/', Path.DirectorySeparatorChar));
        Process.Start(CreateApplicationStartInfo(restoredExecutable, request));
    }
    return result.Succeeded ? 0 : 1;
}
if (args.Length == 2 && args[0] == "--recover")
{
    var requestPath = Path.GetFullPath(args[1]);
    var request = JsonSerializer.Deserialize<ApplicationUpdateTransactionRequest>(File.ReadAllText(requestPath))
        ?? throw new InvalidOperationException("Updater recovery request could not be read.");
    WaitForApplicationExit(request.WaitForProcessId, TimeSpan.FromSeconds(60));
    Process? updatedProcess = null;
    var result = new ApplicationUpdateTransactionEngine().Recover(request,
        () => LaunchAndAwaitHealth(request, out updatedProcess));
    Console.WriteLine(result.Detail);
    if (!result.Succeeded)
    {
        TryStop(updatedProcess);
        var restoredExecutable = Path.Combine(request.LiveDirectory, request.ApplicationRelativePath.Replace('/', Path.DirectorySeparatorChar));
        Process.Start(CreateApplicationStartInfo(restoredExecutable, request));
    }
    return result.Succeeded || result.RolledBack ? 0 : 1;
}

Console.Error.WriteLine("3DPIceland Updater v1: use --self-test, --apply <request.json> or --recover <request.json>.");
return 2;

static void WaitForApplicationExit(int processId, TimeSpan timeout)
{
    if (processId <= 0) throw new InvalidOperationException("A valid application process ID is required.");
    try
    {
        using var process = Process.GetProcessById(processId);
        if (!process.WaitForExit((int)timeout.TotalMilliseconds)) throw new TimeoutException("The running application did not exit before the update timeout.");
    }
    catch (ArgumentException) { }
}

static bool LaunchAndAwaitHealth(ApplicationUpdateTransactionRequest request, out Process? process)
{
    process = null;
    var healthy = false;
    try
    {
        if (File.Exists(request.HealthAcknowledgementPath)) File.Delete(request.HealthAcknowledgementPath);
        var executable = Path.Combine(request.LiveDirectory, request.ApplicationRelativePath.Replace('/', Path.DirectorySeparatorChar));
        var start = new ProcessStartInfo(executable) { UseShellExecute = false, WorkingDirectory = request.LiveDirectory };
        start.ArgumentList.Add("--update-health-ack"); start.ArgumentList.Add(request.HealthAcknowledgementPath);
        start.ArgumentList.Add("--update-transaction"); start.ArgumentList.Add(request.TransactionId);
        AddAutomationProfileArgument(start, request);
        process = Process.Start(start) ?? throw new InvalidOperationException("Updated application process could not be started.");
        var deadline = DateTime.UtcNow.AddSeconds(Math.Clamp(request.HealthTimeoutSeconds, 10, 300));
        while (DateTime.UtcNow < deadline)
        {
            if (File.Exists(request.HealthAcknowledgementPath))
            {
                var acknowledgement = JsonSerializer.Deserialize<ApplicationUpdateHealthAcknowledgement>(File.ReadAllText(request.HealthAcknowledgementPath));
                healthy = acknowledgement is not null && acknowledgement.HealthSchema == ApplicationUpdateHealthAcknowledgement.Schema &&
                          acknowledgement.TransactionId == request.TransactionId && acknowledgement.ReleaseVersion == request.NewVersion &&
                          acknowledgement.DatabaseSchema >= request.MinimumDatabaseSchema && acknowledgement.DatabaseSchema <= request.MaximumDatabaseSchema;
                return healthy;
            }
            if (process.HasExited) return false;
            Thread.Sleep(250);
        }
        return false;
    }
    catch { return false; }
    finally { if (!healthy) TryStop(process); }
}

static ProcessStartInfo CreateApplicationStartInfo(
    string executable,
    ApplicationUpdateTransactionRequest request)
{
    var start = new ProcessStartInfo(executable)
    {
        UseShellExecute = false,
        WorkingDirectory = request.LiveDirectory
    };
    AddAutomationProfileArgument(start, request);
    return start;
}

static void AddAutomationProfileArgument(
    ProcessStartInfo start,
    ApplicationUpdateTransactionRequest request)
{
    if (string.IsNullOrWhiteSpace(request.AutomationProfilePath)) return;
    start.ArgumentList.Add("--automation-profile");
    start.ArgumentList.Add(request.AutomationProfilePath);
}

static void TryStop(Process? process)
{
    try { if (process is not null && !process.HasExited) { process.Kill(entireProcessTree: true); process.WaitForExit(5000); } } catch { }
}
