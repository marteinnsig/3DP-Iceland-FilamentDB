using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using FilamentDbApp.Services;
using FilamentDbApp.UpdateCore;
using System.IO;
using System.Text.Json;

namespace FilamentDbApp;

public partial class App : Application
{
    private static readonly TimeSpan MinimumSplashTime = TimeSpan.FromMilliseconds(2200);
    public static StartupPerformanceService StartupPerformance { get; } = new();
    private static string? _updateHealthAcknowledgementPath;
    private static string? _updateTransactionId;

    protected override async void OnStartup(StartupEventArgs e)
    {
        StartupPerformance.Mark("Application.OnStartup entered");
        base.OnStartup(e);
        AutomationRuntimeProfile.Configure(e.Args);
        ConfigureUpdateHealthAcknowledgement(e.Args);

        var splash = new SplashWindow();
        splash.Show();
        splash.Activate();
        StartupPerformance.Mark("Splash Show invoked");

        // Wait until the splash has actually rendered. MainWindow construction can be
        // expensive and otherwise blocks the UI thread before the splash is visible.
        await splash.WaitUntilShownAsync();
        await Dispatcher.Yield(DispatcherPriority.ApplicationIdle);
        StartupPerformance.Mark("Splash rendered");

        var visibleTimer = Stopwatch.StartNew();
        splash.SetStatus("Loading local database and workspace…");

        MainWindow mainWindow;
        try
        {
            using (StartupPerformance.Measure("MainWindow construction"))
            {
                mainWindow = new MainWindow();
            }
            MainWindow = mainWindow;
            mainWindow.ContentRendered += MainWindow_ContentRendered;
        }
        catch (Exception ex)
        {
            splash.Close();
            var startupError = ex;
            while (startupError.InnerException is not null) startupError = startupError.InnerException;
            MessageBox.Show(
                "The application could not start.\n\n" + ex.Message
                + (ReferenceEquals(startupError, ex) ? string.Empty : "\n\nRoot cause: " + startupError.Message),
                "3DPIceland startup error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(-1);
            return;
        }

        var remaining = MinimumSplashTime - visibleTimer.Elapsed;
        if (remaining > TimeSpan.Zero)
        {
            await Task.Delay(remaining);
        }

        splash.SetStatus("Ready");
        await Task.Delay(220);

        splash.FadeOut(() =>
        {
            splash.Close();
            mainWindow.Show();
            mainWindow.Activate();
            StartupPerformance.Mark("MainWindow Show invoked");
        });
    }

    private static void MainWindow_ContentRendered(object? sender, EventArgs e)
    {
        if (sender is MainWindow mainWindow)
        {
            mainWindow.ContentRendered -= MainWindow_ContentRendered;
        }

        StartupPerformance.Mark("First usable Materials view rendered");
        if (sender is MainWindow renderedWindow)
        {
            TryWriteUpdateHealthAcknowledgement(renderedWindow.CurrentDatabaseSchema);
            if (string.IsNullOrWhiteSpace(_updateTransactionId)) renderedWindow.DetectInterruptedApplicationUpdateAtStartup();
            if (string.IsNullOrWhiteSpace(_updateTransactionId) && !AutomationRuntimeProfile.IsActive)
                renderedWindow.BeginAutomaticUpdateCheck();
        }
    }

    private static void ConfigureUpdateHealthAcknowledgement(string[] args)
    {
        var ackIndex = Array.IndexOf(args, "--update-health-ack");
        var transactionIndex = Array.IndexOf(args, "--update-transaction");
        if (ackIndex < 0 || transactionIndex < 0 || ackIndex + 1 >= args.Length || transactionIndex + 1 >= args.Length) return;
        var allowedRoot = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "3DPIcelandLabs", "Updates", "transactions");
        var fullRoot = Path.GetFullPath(allowedRoot).TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        var candidate = Path.GetFullPath(args[ackIndex + 1]);
        if (!candidate.StartsWith(fullRoot, StringComparison.OrdinalIgnoreCase)) return;
        _updateHealthAcknowledgementPath = candidate;
        _updateTransactionId = args[transactionIndex + 1];
    }

    private static void TryWriteUpdateHealthAcknowledgement(int databaseSchema)
    {
        if (string.IsNullOrWhiteSpace(_updateHealthAcknowledgementPath) || string.IsNullOrWhiteSpace(_updateTransactionId)) return;
        try
        {
            var acknowledgement = new ApplicationUpdateHealthAcknowledgement
            {
                TransactionId = _updateTransactionId, ReleaseVersion = BuildInfo.Version, DatabaseSchema = databaseSchema,
                AcknowledgedAtUtc = DateTimeOffset.UtcNow.ToString("O")
            };
            Directory.CreateDirectory(Path.GetDirectoryName(_updateHealthAcknowledgementPath)!);
            var temp = _updateHealthAcknowledgementPath + ".tmp";
            File.WriteAllText(temp, JsonSerializer.Serialize(acknowledgement, new JsonSerializerOptions { WriteIndented = true }));
            File.Move(temp, _updateHealthAcknowledgementPath, true);
        }
        catch { }
    }
}
