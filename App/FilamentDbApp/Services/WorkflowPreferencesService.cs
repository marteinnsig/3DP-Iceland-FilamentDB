using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace FilamentDbApp.Services;

public sealed class WorkflowPreferencesService
{
    private readonly string _settingsPath;
    private WorkflowPreferences _preferences = new();

    public WorkflowPreferencesService()
    {
        var folder = IOPath.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "3DPIceland",
            "FilamentDbApp");
        IODirectory.CreateDirectory(folder);
        _settingsPath = IOPath.Combine(folder, "workflow-preferences.json");
        Load();
    }

    public void RestoreWindow(Window window)
    {
        var saved = _preferences.Window;
        if (saved is null)
        {
            window.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            window.WindowState = WindowState.Maximized;
            return;
        }

        if (saved.Width >= window.MinWidth && saved.Height >= window.MinHeight)
        {
            window.Width = saved.Width;
            window.Height = saved.Height;
        }

        if (IsReasonableCoordinate(saved.Left) && IsReasonableCoordinate(saved.Top))
        {
            window.Left = saved.Left;
            window.Top = saved.Top;
            window.WindowStartupLocation = WindowStartupLocation.Manual;
        }

        window.WindowState = saved.IsMaximized ? WindowState.Maximized : WindowState.Normal;
    }

    public void CaptureWindow(Window window)
    {
        var bounds = window.RestoreBounds;

        // RestoreBounds may contain NaN/Infinity when startup fails before the main
        // window is fully shown. Never persist invalid JSON numbers or allow UI
        // preference saving to mask the original startup exception.
        if (!IsReasonableCoordinate(bounds.Left) ||
            !IsReasonableCoordinate(bounds.Top) ||
            !double.IsFinite(bounds.Width) || bounds.Width < window.MinWidth ||
            !double.IsFinite(bounds.Height) || bounds.Height < window.MinHeight)
        {
            return;
        }

        _preferences.Window = new WindowPreference
        {
            Left = bounds.Left,
            Top = bounds.Top,
            Width = bounds.Width,
            Height = bounds.Height,
            IsMaximized = window.WindowState == WindowState.Maximized
        };
    }

    public string GetWebsiteExportFolder() => _preferences.WebsiteExportFolder?.Trim() ?? string.Empty;

    public void SetWebsiteExportFolder(string? folder)
    {
        _preferences.WebsiteExportFolder = folder?.Trim() ?? string.Empty;
    }

    public string GetLastSelectedMaterialId() => _preferences.LastSelectedMaterialId?.Trim() ?? string.Empty;

    public void SetLastSelectedMaterialId(string? materialId)
    {
        _preferences.LastSelectedMaterialId = materialId?.Trim() ?? string.Empty;
    }

    public IReadOnlyList<WorkflowColumnLayout> GetFastMaterialsGridLayout() =>
        GetFastGridLayout("Materials");

    public IReadOnlyList<WorkflowColumnLayout> GetFastGridLayout(string workspace)
    {
        var layout = string.Equals(workspace, "Materials", StringComparison.Ordinal) &&
                     !_preferences.FastGridLayouts.ContainsKey(workspace)
            ? _preferences.FastMaterialsGridLayout
            : _preferences.FastGridLayouts.GetValueOrDefault(workspace) ?? new List<WorkflowColumnLayout>();
        return layout
            .Where(item =>
                !string.IsNullOrWhiteSpace(item.Key) &&
                double.IsFinite(item.Width) &&
                item.Width >= 50 &&
                item.Width <= 500 &&
                item.DisplayIndex >= 0)
            .GroupBy(item => item.Key, StringComparer.Ordinal)
            .Select(group => group.First())
            .OrderBy(item => item.DisplayIndex)
            .ToList();
    }

    public void SetFastMaterialsGridLayout(IEnumerable<WorkflowColumnLayout> layout) =>
        SetFastGridLayout("Materials", layout);

    public void SetFastGridLayout(string workspace, IEnumerable<WorkflowColumnLayout> layout)
    {
        var normalized = layout
            .Where(item =>
                !string.IsNullOrWhiteSpace(item.Key) &&
                double.IsFinite(item.Width) &&
                item.Width >= 50 &&
                item.Width <= 500 &&
                item.DisplayIndex >= 0)
            .GroupBy(item => item.Key, StringComparer.Ordinal)
            .Select(group => group.First())
            .OrderBy(item => item.DisplayIndex)
            .ToList();
        _preferences.FastGridLayouts[workspace] = normalized;
        if (string.Equals(workspace, "Materials", StringComparison.Ordinal))
        {
            _preferences.FastMaterialsGridLayout = normalized;
        }
        Save();
    }

    public void ClearFastMaterialsGridLayout()
    {
        _preferences.FastMaterialsGridLayout.Clear();
        Save();
    }

    public bool HasSavedGridWidths(DataGrid grid) =>
        !string.IsNullOrWhiteSpace(grid.Name) &&
        ((_preferences.GridColumnLayouts.TryGetValue(grid.Name, out var layouts) && layouts.Count > 0) ||
         (_preferences.GridColumnWidths.TryGetValue(grid.Name, out var widths) && widths.Count > 0));

    public void RestoreGrid(DataGrid grid)
    {
        if (string.IsNullOrWhiteSpace(grid.Name)) return;

        // v37.2.5+: restore by a stable column identity rather than by collection index.
        // This keeps user widths attached to the correct field if XAML column order changes.
        if (_preferences.GridColumnLayouts.TryGetValue(grid.Name, out var layouts) && layouts.Count > 0)
        {
            var savedByKey = layouts
                .Where(layout => !string.IsNullOrWhiteSpace(layout.Key))
                .GroupBy(layout => layout.Key, StringComparer.Ordinal)
                .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

            foreach (var column in grid.Columns)
            {
                var key = GetColumnKey(column);
                if (!savedByKey.TryGetValue(key, out var saved)) continue;

                if (double.IsFinite(saved.Width) && saved.Width >= 20 && saved.Width <= 1200)
                {
                    column.Width = new DataGridLength(saved.Width);
                }
            }

            var orderedColumns = new DataGridColumn?[grid.Columns.Count];
            foreach (var column in grid.Columns)
            {
                var key = GetColumnKey(column);
                if (!savedByKey.TryGetValue(key, out var saved) ||
                    saved.DisplayIndex < 0 ||
                    saved.DisplayIndex >= orderedColumns.Length ||
                    orderedColumns[saved.DisplayIndex] is not null)
                {
                    continue;
                }

                orderedColumns[saved.DisplayIndex] = column;
            }

            var remainingColumns = new Queue<DataGridColumn>(
                grid.Columns
                    .Where(column => !orderedColumns.Contains(column))
                    .OrderBy(column => column.DisplayIndex));
            for (var index = 0; index < orderedColumns.Length; index++)
            {
                orderedColumns[index] ??= remainingColumns.Dequeue();
            }
            for (var index = 0; index < orderedColumns.Length; index++)
            {
                orderedColumns[index]!.DisplayIndex = index;
            }

            return;
        }

        // Backward-compatible fallback for workflow-preferences.json files written by
        // earlier v37 builds. The next clean shutdown migrates these values to keyed layouts.
        if (!_preferences.GridColumnWidths.TryGetValue(grid.Name, out var widths)) return;

        for (var index = 0; index < grid.Columns.Count && index < widths.Count; index++)
        {
            var width = widths[index];
            if (double.IsFinite(width) && width >= 20 && width <= 1200)
            {
                grid.Columns[index].Width = new DataGridLength(width);
            }
        }
    }

    public void CaptureGrid(DataGrid grid)
    {
        if (string.IsNullOrWhiteSpace(grid.Name)) return;

        _preferences.GridColumnLayouts[grid.Name] = grid.Columns
            .Select(column => new GridColumnPreference
            {
                Key = GetColumnKey(column),
                Width = ResolveColumnWidth(column),
                DisplayIndex = column.DisplayIndex
            })
            .Where(layout => double.IsFinite(layout.Width))
            .ToList();

        // Keep the legacy list populated for downgrade compatibility.
        _preferences.GridColumnWidths[grid.Name] = grid.Columns
            .OrderBy(column => column.DisplayIndex)
            .Select(ResolveColumnWidth)
            .Where(double.IsFinite)
            .ToList();
    }

    public static IReadOnlyList<WorkflowColumnLayout> CaptureGridLayout(DataGrid grid) =>
        grid.Columns
            .Select(column => new WorkflowColumnLayout(
                GetColumnKey(column),
                ResolveColumnWidth(column),
                column.DisplayIndex))
            .Where(layout =>
                !string.IsNullOrWhiteSpace(layout.Key) &&
                double.IsFinite(layout.Width) &&
                layout.Width >= 20 &&
                layout.Width <= 1200 &&
                layout.DisplayIndex >= 0)
            .ToList();

    public void ResetGrid(DataGrid grid, IReadOnlyList<WorkflowColumnLayout> defaultLayout)
    {
        if (string.IsNullOrWhiteSpace(grid.Name) || defaultLayout.Count == 0) return;

        _preferences.GridColumnLayouts.Remove(grid.Name);
        _preferences.GridColumnWidths.Remove(grid.Name);
        ApplyLayout(grid, defaultLayout);
        CaptureGrid(grid);
        Save();
    }

    private static void ApplyLayout(DataGrid grid, IReadOnlyList<WorkflowColumnLayout> layout)
    {
        var savedByKey = layout
            .Where(item => !string.IsNullOrWhiteSpace(item.Key))
            .GroupBy(item => item.Key, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(), StringComparer.Ordinal);

        foreach (var column in grid.Columns)
        {
            if (!savedByKey.TryGetValue(GetColumnKey(column), out var saved)) continue;
            if (double.IsFinite(saved.Width) && saved.Width >= 20 && saved.Width <= 1200)
            {
                column.Width = new DataGridLength(saved.Width);
            }
        }

        var ordered = grid.Columns
            .Select((column, originalIndex) =>
            {
                var savedIndex = savedByKey.TryGetValue(GetColumnKey(column), out var saved)
                    ? saved.DisplayIndex
                    : int.MaxValue;
                return (Column: column, OriginalIndex: originalIndex, SavedIndex: savedIndex);
            })
            .OrderBy(item => item.SavedIndex)
            .ThenBy(item => item.OriginalIndex)
            .Select(item => item.Column)
            .ToList();
        for (var index = 0; index < ordered.Count; index++)
        {
            ordered[index].DisplayIndex = index;
        }
    }

    private static double ResolveColumnWidth(DataGridColumn column)
    {
        if (column.Width.UnitType == DataGridLengthUnitType.Pixel &&
            double.IsFinite(column.Width.Value) &&
            column.Width.Value >= 20)
        {
            return column.Width.Value;
        }

        return column.ActualWidth;
    }

    private static string GetColumnKey(DataGridColumn column)
    {
        static string? BindingPath(BindingBase? binding) =>
            binding is Binding concrete ? concrete.Path?.Path : null;

        var path = column switch
        {
            DataGridBoundColumn bound => BindingPath(bound.Binding),
            DataGridComboBoxColumn combo =>
                BindingPath(combo.SelectedValueBinding) ??
                BindingPath(combo.SelectedItemBinding) ??
                BindingPath(combo.TextBinding),
            _ => null
        };

        if (!string.IsNullOrWhiteSpace(path)) return $"binding:{path}";

        var header = Convert.ToString(column.Header)?.Trim();
        return !string.IsNullOrWhiteSpace(header)
            ? $"header:{header}"
            : $"display:{column.DisplayIndex}";
    }

    public void Save()
    {
        try
        {
            var json = JsonSerializer.Serialize(_preferences, new JsonSerializerOptions { WriteIndented = true });
            SafeFileOperations.WriteAllTextAtomic(_settingsPath, json, new UTF8Encoding(false));
        }
        catch (IOException)
        {
            // Workflow preferences must never block normal application shutdown.
        }
        catch (UnauthorizedAccessException)
        {
            // The app remains fully usable even if local UI preferences cannot be saved.
        }
        catch (ArgumentException)
        {
            // Invalid UI geometry (for example NaN/Infinity during failed startup)
            // must never crash or hide the real application exception.
        }
    }

    private void Load()
    {
        try
        {
            if (!IOFile.Exists(_settingsPath)) return;
            _preferences = JsonSerializer.Deserialize<WorkflowPreferences>(IOFile.ReadAllText(_settingsPath)) ?? new WorkflowPreferences();
        }
        catch (JsonException)
        {
            _preferences = new WorkflowPreferences();
        }
        catch (IOException)
        {
            _preferences = new WorkflowPreferences();
        }
        catch (UnauthorizedAccessException)
        {
            _preferences = new WorkflowPreferences();
        }
    }

    private static bool IsReasonableCoordinate(double value) => double.IsFinite(value) && value > -10000 && value < 10000;

    private sealed class WorkflowPreferences
    {
        public WindowPreference? Window { get; set; }
        public string WebsiteExportFolder { get; set; } = string.Empty;
        public string LastSelectedMaterialId { get; set; } = string.Empty;
        public List<WorkflowColumnLayout> FastMaterialsGridLayout { get; set; } = new();
        public Dictionary<string, List<WorkflowColumnLayout>> FastGridLayouts { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, List<double>> GridColumnWidths { get; set; } = new(StringComparer.Ordinal);
        public Dictionary<string, List<GridColumnPreference>> GridColumnLayouts { get; set; } = new(StringComparer.Ordinal);
    }

    private sealed class GridColumnPreference
    {
        public string Key { get; set; } = string.Empty;
        public double Width { get; set; }
        public int DisplayIndex { get; set; }
    }

    private sealed class WindowPreference
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Width { get; set; }
        public double Height { get; set; }
        public bool IsMaximized { get; set; }
    }
}

public sealed record WorkflowColumnLayout(string Key, double Width, int DisplayIndex);
