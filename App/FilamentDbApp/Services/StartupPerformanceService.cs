using System.Diagnostics;
using System.Globalization;
using System.Text;

namespace FilamentDbApp.Services;

/// <summary>
/// Records read-only startup phase timings. The service deliberately does not
/// schedule work or change startup ordering; it supplies evidence for later
/// optimization builds.
/// </summary>
public sealed class StartupPerformanceService
{
    private readonly Stopwatch _startupClock = Stopwatch.StartNew();
    private readonly object _syncRoot = new();
    private readonly List<StartupPerformanceEntry> _entries = new();

    public StartupPerformanceService()
    {
        try
        {
            ProcessStartedAtLocal = Process.GetCurrentProcess().StartTime;
        }
        catch
        {
            ProcessStartedAtLocal = DateTime.Now;
        }

        InstrumentationStartedAtLocal = DateTime.Now;
        Mark("Startup instrumentation active");
    }

    public DateTime ProcessStartedAtLocal { get; }

    public DateTime InstrumentationStartedAtLocal { get; }

    public IDisposable Measure(string phaseName) => new StartupPhaseScope(this, phaseName);

    public void Mark(string phaseName)
    {
        AddEntry(phaseName, null, _startupClock.Elapsed);
    }

    public IReadOnlyList<StartupPerformanceEntry> Snapshot()
    {
        lock (_syncRoot)
        {
            return _entries.ToList();
        }
    }

    public bool HasEntry(string phaseName)
    {
        lock (_syncRoot)
        {
            return _entries.Any(entry => string.Equals(entry.PhaseName, phaseName, StringComparison.Ordinal));
        }
    }

    public string BuildDiagnosticsReport()
    {
        var entries = Snapshot();
        var sb = new StringBuilder();
        sb.AppendLine("Startup Performance");
        sb.AppendLine("-------------------");
        sb.AppendLine("Process started: " + ProcessStartedAtLocal.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
        sb.AppendLine("Instrumentation started: " + InstrumentationStartedAtLocal.ToString("yyyy-MM-dd HH:mm:ss.fff", CultureInfo.InvariantCulture));
        sb.AppendLine("Process to instrumentation: " + Math.Max(0, (InstrumentationStartedAtLocal - ProcessStartedAtLocal).TotalMilliseconds).ToString("0.0", CultureInfo.InvariantCulture) + " ms");
        sb.AppendLine("Recorded phases: " + entries.Count.ToString(CultureInfo.InvariantCulture));

        foreach (var entry in entries)
        {
            var duration = entry.Duration.HasValue
                ? entry.Duration.Value.TotalMilliseconds.ToString("0.0", CultureInfo.InvariantCulture) + " ms"
                : "marker";
            sb.AppendLine($"{entry.ElapsedFromInstrumentationStart.TotalMilliseconds,9:0.0} ms  {duration,12}  {entry.PhaseName}");
        }

        sb.AppendLine("Note: Visual Studio Debug startup includes debugger/JIT overhead; compare repeated cold and warm Release EXE runs separately.");
        return sb.ToString();
    }

    private void AddEntry(string phaseName, TimeSpan? duration, TimeSpan elapsed)
    {
        lock (_syncRoot)
        {
            _entries.Add(new StartupPerformanceEntry(phaseName, duration, elapsed));
        }
    }

    private sealed class StartupPhaseScope : IDisposable
    {
        private readonly StartupPerformanceService _owner;
        private readonly string _phaseName;
        private readonly Stopwatch _phaseClock = Stopwatch.StartNew();
        private bool _disposed;

        public StartupPhaseScope(StartupPerformanceService owner, string phaseName)
        {
            _owner = owner;
            _phaseName = phaseName;
        }

        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _phaseClock.Stop();
            _owner.AddEntry(_phaseName, _phaseClock.Elapsed, _owner._startupClock.Elapsed);
        }
    }
}

public sealed record StartupPerformanceEntry(
    string PhaseName,
    TimeSpan? Duration,
    TimeSpan ElapsedFromInstrumentationStart);
