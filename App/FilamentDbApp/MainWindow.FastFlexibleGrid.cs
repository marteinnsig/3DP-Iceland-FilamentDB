using FilamentDbApp.Models;
using FilamentDbApp.Services;
using System.Collections;
using System.Globalization;
using System.Reflection;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace FilamentDbApp;

public partial class MainWindow
{
    // Temporary column/selection bridges are retired in v67.0.11 after runtime acceptance.
    private readonly Dictionary<DataGrid, MaterialsRenderingPrototypeView> _fastFlexibleViews = new();
    private DispatcherTimer? _flexibleSaveTimer;
    private bool _flexibleEditDirty;
    private bool _committingFastFlexible;
    private bool _syncingFastFlexible;

    private void ActivateFastFlexibleViews()
    {
        foreach (var name in FlexibleEditableGridNames)
        {
            if (FindName(name) is not DataGrid grid || _fastFlexibleViews.ContainsKey(grid)) continue;
            var columns = grid.Columns.Select(column =>
            {
                var binding = column switch
                {
                    DataGridBoundColumn bound => bound.Binding as Binding,
                    DataGridComboBoxColumn combo => (combo.SelectedValueBinding ?? combo.SelectedItemBinding) as Binding,
                    _ => null
                };
                var property = binding?.Path?.Path;
                var choices = column is DataGridComboBoxColumn c
                    ? c.ItemsSource.Cast<object>().Select(item => item is FlexibleMaterialChoice material ? material.DisplayName : item.ToString() ?? "").ToList()
                    : new List<string>();
                return new MaterialsPrototypeColumn(column.Header?.ToString() ?? "", column.Width.IsAbsolute ? column.Width.Value : 260,
                    property, column.IsReadOnly, column is DataGridCheckBoxColumn ? MaterialsPrototypeEditorKind.CheckBox :
                    column is DataGridComboBoxColumn ? MaterialsPrototypeEditorKind.ComboBox : MaterialsPrototypeEditorKind.Text,
                    choices, column.IsReadOnly ? FastGridCellKind.Computed : FastGridCellKind.Standard);
            }).ToList();
            List<MaterialsPrototypeRow> Rows(IReadOnlyList<MaterialsPrototypeColumn> currentColumns) => grid.Items.Cast<object>()
                .Where(row => row != CollectionView.NewItemPlaceholder).Select(row =>
                {
                    var cells = currentColumns.Select(column => FastFlexibleCellText(row, column.PropertyName)).ToArray();
                    return new MaterialsPrototypeRow(row, FlexibleRowIdentity(row), cells, cells.ToArray(), () => true);
                }).ToList();
            var view = new MaterialsRenderingPrototypeView(columns, Rows(columns), ApplyFastFlexibleChanges,
                _ => { }, Rows, row => SelectFastFlexibleRow(grid, row), directCanonicalEditing: true,
                reloadAfterApply: true, showReloadButton: false, flexibleNavigation: true,
                cellText: FastFlexibleCellText, showStatus: false);
            AutomationProperties.SetAutomationId(view, "Fast" + name);
            var host = new Grid();
            Grid.SetRow(host, Grid.GetRow(grid));
            Grid.SetColumn(host, Grid.GetColumn(grid));
            if (grid.Parent is Panel panel)
            {
                var index = panel.Children.IndexOf(grid);
                panel.Children.Remove(grid);
                panel.Children.Insert(index, host);
            }
            else if (grid.Parent is ContentControl content) content.Content = host;
            else throw new InvalidOperationException($"Unsupported Flexible host: {name}.");
            grid.Visibility = Visibility.Collapsed;
            host.Children.Add(grid);
            host.Children.Add(view);
            if (name == "FlexibleSpecimensGrid") host.Height = 292;
            _fastFlexibleViews.Add(grid, view);
        }
        _flexibleSaveTimer = new DispatcherTimer(DispatcherPriority.ApplicationIdle, Dispatcher)
        { Interval = TimeSpan.FromMilliseconds(800) };
        _flexibleSaveTimer.Tick += (_, _) =>
        {
            // Never occupy the UI thread with graph/evidence work while a text editor is active.
            if (_fastFlexibleViews.Values.Any(view => view.HasActiveEditor)) return;
            _flexibleSaveTimer.Stop();
            if (_flexibleEditDirty) SaveFlexibleTesting();
        };
        SyncFastFlexibleViews();
    }

    private static string FlexibleRowIdentity(object row) => row switch
    {
        FlexibleTestSessionRecord r => r.FlexibleTestSessionId,
        FlexibleTestSpecimenRecord r => r.SpecimenId,
        CompressionPointRecord r => r.CompressionPointId,
        StressRelaxationPointRecord r => r.RelaxationPointId,
        RecoveryMeasurementRecord r => r.RecoveryMeasurementId,
        ShoreHardnessReadingRecord r => r.ShoreReadingId,
        _ => throw new InvalidOperationException("Unsupported Flexible row.")
    };

    private string FastFlexibleCellText(object row, string? property)
    {
        if (row is FlexibleTestSessionRecord session && property == nameof(session.MaterialID))
            return FlexibleSessionMaterialColumn.ItemsSource.Cast<FlexibleMaterialChoice>()
                .FirstOrDefault(choice => choice.MaterialID == session.MaterialID)?.DisplayName ?? session.MaterialID;
        return PrototypeCellText(row, property);
    }

    private void SelectFastFlexibleRow(DataGrid grid, object? row)
    {
        if (_syncingFastFlexible || _bindingFlexibleSelection) return;
        if (grid == FlexibleSessionsGrid)
        {
            if (!ReferenceEquals(row, _selectedFlexibleSession)) BindFlexibleTestingSession(row as FlexibleTestSessionRecord);
        }
        else if (grid == FlexibleSpecimensGrid && row is FlexibleTestSpecimenRecord specimen)
        {
            if (!ReferenceEquals(specimen, _selectedFlexibleSpecimen)) SelectFlexibleSpecimen(specimen);
        }
        else
        {
            SetFlexibleParentSelection(grid, row);
            if (row is null) _lastSelectedFlexibleReadingByGrid.Remove(grid.Name);
            else _lastSelectedFlexibleReadingByGrid[grid.Name] = row;
        }
    }

    private void SyncFastFlexibleViews()
    {
        if (_syncingFastFlexible) return;
        _syncingFastFlexible = true;
        try
        {
            foreach (var (grid, view) in _fastFlexibleViews)
            {
                view.SynchronizeFromCanonical("Flexible selection/commit", grid.SelectedItem, preserveActiveEditor: true);
                view.SelectCanonicalSource(grid.SelectedItem);
                if (grid == FlexibleSessionsGrid && view.Parent is Grid host)
                    host.Height = Math.Clamp(36 + grid.Items.Count * 26, 125, 625);
            }
        }
        finally { _syncingFastFlexible = false; }
    }

    private bool CommitFastFlexibleEditors()
    {
        if (_committingFastFlexible) return true;
        _committingFastFlexible = true;
        try { return _fastFlexibleViews.Values.All(view => view.TryCommitActiveEditor()); }
        finally { _committingFastFlexible = false; }
    }

    private bool FlushFastFlexibleChanges() => CommitFastFlexibleEditors() && (!_flexibleEditDirty || SaveFlexibleTesting());

    private bool ApplyFastFlexibleChanges(IReadOnlyList<MaterialsPrototypeChange> changes)
    {
        // Validate detached copies first: rejected input must not partially mutate a row or its coupled TVL heights.
        var copies = changes.Select(change => change.Row.Source).Distinct()
            .ToDictionary(row => row, CloneFlexibleInputRow);
        var assignments = new List<(object Source, PropertyInfo Property, object Value)>();
        try
        {
            foreach (var change in changes)
            {
                if (change.Column.IsReadOnly || change.Column.PropertyName is not { } name) return false;
                var source = change.Row.Source;
                if (!IsCanonicalFlexibleInputRow(source)) throw new FormatException("The selected reading is no longer available.");
                var property = source.GetType().GetProperty(name)!;
                object value = change.NewValue;
                if (source is FlexibleTestSessionRecord && name == "MaterialID")
                    value = FlexibleSessionMaterialColumn.ItemsSource.Cast<FlexibleMaterialChoice>()
                        .FirstOrDefault(choice => choice.DisplayName == change.NewValue || choice.MaterialID == change.NewValue)?.MaterialID
                        ?? throw new FormatException("Choose an active material from the list.");
                else if (property.PropertyType == typeof(bool)) value = change.NewValue is "✓" or "True" or "true";
                else if (property.PropertyType == typeof(int)) value = int.Parse(change.NewValue, CultureInfo.CurrentCulture);
                property.SetValue(copies[source], value);
                assignments.Add((source, property, value));
            }
            IEnumerable<T> Candidate<T>(IEnumerable<T> rows) where T : class => rows.Select(row => copies.TryGetValue(row, out var copy) ? (T)copy : row);
            var errors = FlexibleMaterialTestingService.Validate(Candidate(_flexibleSpecimens), Candidate(_compressionPoints),
                Candidate(_stressRelaxationPoints), Candidate(_recoveryMeasurements), Candidate(_shoreHardnessReadings));
            if (errors.Count > 0) throw new FormatException(errors[0]);
            foreach (var session in Candidate(_flexibleTestSessions))
                if (!_nativeMaterialRows.Any(material => !material.IsArchived && material.MaterialID == session.MaterialID))
                    throw new FormatException("Each test session must reference an active material.");
        }
        catch (Exception ex) when (ex is FormatException or OverflowException or ArgumentException or TargetInvocationException)
        {
            SetFlexibleStatus(ex.InnerException?.Message ?? ex.Message, true);
            return false;
        }
        foreach (var (source, property, value) in assignments) property.SetValue(source, value);
        FlexibleMaterialTestingService.Recalculate(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements);
        _flexibleEditDirty = true;
        _flexibleSaveTimer?.Stop();
        _flexibleSaveTimer?.Start();
        SetFlexibleStatus("Changes pending — saved after leaving the editor, switching session or closing.", false);
        return true;
    }

    private bool IsCanonicalFlexibleInputRow(object row) => row switch
    {
        FlexibleTestSessionRecord r => _flexibleTestSessions.Contains(r),
        FlexibleTestSpecimenRecord r => _flexibleSpecimens.Contains(r),
        CompressionPointRecord r => _compressionPoints.Contains(r),
        StressRelaxationPointRecord r => _stressRelaxationPoints.Contains(r),
        RecoveryMeasurementRecord r => _recoveryMeasurements.Contains(r),
        ShoreHardnessReadingRecord r => _shoreHardnessReadings.Contains(r),
        _ => false
    };

    private static object CloneFlexibleInputRow(object row)
    {
        var copy = typeof(object).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(row, null)!;
        // Copy private TVL input state, but never notify live bindings while validating a detached candidate.
        foreach (var field in row.GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic))
            if (typeof(Delegate).IsAssignableFrom(field.FieldType)) field.SetValue(copy, null);
        return copy;
    }

    private bool VerifyFastFlexibleInputContract()
    {
        var original = new RecoveryMeasurementRecord { InitialHeightMm = "10", HeightAfterRestMm = "9.8" };
        var notifications = 0;
        original.PropertyChanged += (_, _) => notifications++;
        var candidate = (RecoveryMeasurementRecord)CloneFlexibleInputRow(original);
        candidate.TvlContactOffsetMm = "20";
        var rejected = candidate.TvlContactOffsetError.Length > 0 && original.HeightAfterRestMm == "9.8";
        candidate.TvlContactOffsetMm = "0.14";
        return rejected && notifications == 0 && original.HeightAfterRestMm == "9.8" &&
            FlexibleMaterialTestingService.ParseOptional(candidate.HeightAfterRestMm) == 9.86 &&
            _fastFlexibleViews.Count == FlexibleEditableGridNames.Length &&
            _fastFlexibleViews.All(pair => pair.Key.Visibility == Visibility.Collapsed &&
                AutomationProperties.GetAutomationId(pair.Value) == "Fast" + pair.Key.Name) &&
            MaterialsRenderingPrototypeView.RunFlexibleNavigationContractVerification();
    }
}
