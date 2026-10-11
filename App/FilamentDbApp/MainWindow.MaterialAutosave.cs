using System.Windows.Threading;
using FilamentDbApp.Services;

namespace FilamentDbApp;

public partial class MainWindow
{
    private readonly DispatcherTimer _materialAutosaveRetryTimer = new()
    {
        Interval = TimeSpan.FromMinutes(5)
    };
    private string? _materialAutosaveFailure;
    private readonly Dictionary<string, string> _otherAutosaveFailures = new(StringComparer.Ordinal);

    private void InitializeMaterialAutosaveRetry()
    {
        _materialAutosaveRetryTimer.Tick += MaterialAutosaveRetry_Tick;
        Closed += (_, _) => _materialAutosaveRetryTimer.Stop();
        _materialAutosaveRetryTimer.Start();
    }

    private void MaterialAutosaveRetry_Tick(object? sender, EventArgs e)
    {
        // Save only canonical, committed Material rows. Do not commit an active editor,
        // rebind grids, refresh filters or synchronize child measurements on a timer.
        if (_databaseRestoreShutdown || !_nativeMaterialDirty || _nativeMaterialEditDebounceTimer.IsEnabled)
            return;
        SaveNativeMaterialsSilent();
    }

    private string MaterialAutosaveStatus(string fallback)
    {
        if (_otherAutosaveFailures.Count > 0)
        {
            var failures = string.Join("; ", _otherAutosaveFailures.Select(item => $"{item.Key}: {item.Value}"));
            if (_materialAutosaveFailure is not null) failures += $"; Materials: {_materialAutosaveFailure}";
            return "Auto-save blocked — " + failures;
        }
        if (_materialAutosaveFailure is not null)
            return $"Materials not saved: {_materialAutosaveFailure} (retry every 5 min)";
        return _nativeMaterialDirty
            ? "Materials changes pending — auto-save retry every 5 min"
            : fallback;
    }

    private void VerifyAuthorizedMaterialAutosaveRetry(string materialId)
    {
        AutomationRuntimeProfile.DemandMaterialCrudAuthorized(materialId);
        var row = _nativeMaterialRows.Single(item => item.MaterialID == materialId);
        var originalNotes = row.Notes;
        var originalProductLine = row.ProductLine;
        if (_materialAutosaveRetryTimer.Interval != TimeSpan.FromMinutes(5) || !_materialAutosaveRetryTimer.IsEnabled)
            throw new InvalidOperationException("Materials periodic autosave is not running at five-minute intervals.");

        // Exercise the actual timer callback without a five-minute wall-clock delay.
        row.Notes = "AUTOMATION-PERIODIC-AUTOSAVE";
        MarkNativeMaterialsDirty();
        MaterialAutosaveRetry_Tick(null, EventArgs.Empty);
        if (_nativeMaterialDirty || _database.LoadNativeMaterialManagerRows().Single(item => item.MaterialID == materialId).Notes != row.Notes)
            throw new InvalidOperationException("Periodic autosave did not persist pending Material data.");

        row.ProductLine = "";
        MarkNativeMaterialsDirty();
        ShowNativeSaveBlockedStatus("Tensile", "AUTOMATION-SAVE-ERROR");
        RefreshApplicationStatistics();
        if (!AutoSaveStatusText.Text.Contains("Tensile: AUTOMATION-SAVE-ERROR", StringComparison.Ordinal))
            throw new InvalidOperationException("Pending Materials hid a measurement save failure.");
        ShowNativeSavedStatus("Tensile");
        MaterialAutosaveRetry_Tick(null, EventArgs.Empty);
        RefreshApplicationStatistics();
        ShowNativeSavedStatus("Measurements");
        if (!_nativeMaterialDirty || !AutoSaveStatusText.Text.StartsWith("Materials not saved:", StringComparison.Ordinal) ||
            _database.LoadNativeMaterialManagerRows().Single(item => item.MaterialID == materialId).ProductLine != originalProductLine)
            throw new InvalidOperationException("Failed Material save was hidden or invalid data reached SQLite.");

        row.ProductLine = originalProductLine;
        row.Notes = originalNotes;
        MaterialAutosaveRetry_Tick(null, EventArgs.Empty);
        RefreshApplicationStatistics();
        if (_nativeMaterialDirty || _materialAutosaveFailure is not null ||
            _database.LoadNativeMaterialManagerRows().Single(item => item.MaterialID == materialId).Notes != originalNotes)
            throw new InvalidOperationException("Periodic autosave failed to recover after validation correction.");
    }
}
