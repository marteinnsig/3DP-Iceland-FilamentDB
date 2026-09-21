using FilamentDbApp.Models;
using FilamentDbApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace FilamentDbApp;

public partial class MainWindow
{
    private const string FlexibleSettingsSection = "Flexible Material Testing";
    private const string FlexibleDefaultDiameterParameter = "Default specimen diameter";
    private const string FlexibleDefaultHeightParameter = "Default specimen height";
    private const string FlexibleDefaultThicknessParameter = "Default specimen thickness";
    private const string FlexibleDefaultDisplacementParameter = "Default compression displacement";
    private const string FlexibleRecoveryHoldParameter = "Default recovery compressed hold";
    private static readonly string[] FlexibleEditableGridNames =
    {
        "FlexibleSessionsGrid",
        "FlexibleSpecimensGrid",
        "CompressionPointsGrid",
        "StressRelaxationGrid",
        "RecoveryMeasurementsGrid",
        "ShoreHardnessGrid"
    };
    private readonly ObservableCollection<FlexibleTestSessionRecord> _flexibleTestSessions = new();
    private readonly ObservableCollection<FlexibleTestSpecimenRecord> _flexibleSpecimens = new();
    private readonly ObservableCollection<CompressionPointRecord> _compressionPoints = new();
    private readonly ObservableCollection<StressRelaxationPointRecord> _stressRelaxationPoints = new();
    private readonly ObservableCollection<RecoveryMeasurementRecord> _recoveryMeasurements = new();
    private readonly ObservableCollection<ShoreHardnessReadingRecord> _shoreHardnessReadings = new();
    private readonly ObservableCollection<FlexibleComparisonRow> _flexibleComparisonRows = new();
    private readonly Dictionary<string, object> _lastSelectedFlexibleReadingByGrid = new(StringComparer.Ordinal);
    private FlexibleTestSessionRecord? _selectedFlexibleSession;
    private FlexibleTestSpecimenRecord? _selectedFlexibleSpecimen;
    private bool _bindingFlexibleSelection;

    private void InitializeFlexibleMaterialTesting()
    {
        string? shoreRepairError = null;
        try { _database.RepairUnusedShoreTemplateDefaults(); }
        catch (Exception ex) { shoreRepairError = $"Shore defaults could not be repaired: {ex.Message}. Existing rows are retained; retry on next startup."; }
        var graph = _database.LoadFlexibleTestingGraph();
        foreach (var row in graph.Sessions) { row.MaterialDisplayName = FlexibleMaterialDisplayName(row.MaterialID); _flexibleTestSessions.Add(row); }
        foreach (var row in graph.Specimens) _flexibleSpecimens.Add(row);
        foreach (var row in graph.Compression) _compressionPoints.Add(row);
        foreach (var row in graph.Relaxation) _stressRelaxationPoints.Add(row);
        foreach (var row in graph.Recovery) _recoveryMeasurements.Add(row);
        foreach (var row in graph.Shore) _shoreHardnessReadings.Add(row);
        FlexibleMaterialTestingService.Recalculate(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements);
        RefreshSavedFlexibleMaterialEvidence();
        FlexibleSessionsGrid.ItemsSource = _flexibleTestSessions;
        if (CollectionViewSource.GetDefaultView(_flexibleTestSessions) is ListCollectionView sessionView)
        {
            sessionView.CustomSort = FlexibleSpecimenLabelComparer.Ascending;
            FlexibleSessionsGrid.Columns[0].SortDirection = ListSortDirection.Ascending;
        }
        FlexibleSessionMaterialColumn.ItemsSource = _nativeMaterialRows
            .Where(x => !x.IsArchived && !string.IsNullOrWhiteSpace(x.MaterialID))
            .Select(x => new FlexibleMaterialChoice(
                x.MaterialID,
                $"{x.MaterialID} — {(string.IsNullOrWhiteSpace(x.WebsiteDisplayName) ? BuildNativeMaterialDisplayName(x) : x.WebsiteDisplayName.Trim())}"))
            .GroupBy(x => x.MaterialID, StringComparer.OrdinalIgnoreCase)
            .Select(x => x.First())
            .OrderBy(x => x.DisplayName, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
        FlexibleComparisonGrid.ItemsSource = _flexibleComparisonRows;
        RegisterFlexibleFirstClickEditing();
        RefreshNativeMaterialTestStatusFromNativeInputTabs(markDirty: false);
        BindFlexibleTestingSession(_flexibleTestSessions.FirstOrDefault(x => x.IsActive) ?? _flexibleTestSessions.FirstOrDefault());
        ActivateFastFlexibleViews();
        if (shoreRepairError is not null) SetFlexibleStatus(shoreRepairError, true);
    }

    private void RegisterFlexibleFirstClickEditing()
    {
        foreach (var gridName in FlexibleEditableGridNames)
        {
            if (FindName(gridName) is not DataGrid grid) continue;
            grid.PreviewMouseLeftButtonDown -= FlexibleParentGrid_PreviewMouseLeftButtonDown;
            grid.PreviewMouseLeftButtonDown += FlexibleParentGrid_PreviewMouseLeftButtonDown;
            grid.CurrentCellChanged -= FlexibleReadingGrid_CurrentCellChanged;
            grid.CurrentCellChanged += FlexibleReadingGrid_CurrentCellChanged;
            grid.SelectedCellsChanged -= FlexibleReadingGrid_SelectedCellsChanged;
            grid.SelectedCellsChanged += FlexibleReadingGrid_SelectedCellsChanged;
            grid.RemoveHandler(Keyboard.PreviewKeyDownEvent, new KeyEventHandler(InputDataGrid_PreviewKeyDown));
            grid.AddHandler(Keyboard.PreviewKeyDownEvent, new KeyEventHandler(InputDataGrid_PreviewKeyDown), true);
            grid.PreviewMouseLeftButtonDown -= WorkflowGrid_PreviewMouseLeftButtonDown;
            grid.PreviewMouseLeftButtonDown += WorkflowGrid_PreviewMouseLeftButtonDown;
        }
    }

    private void FlexibleReadingGrid_CurrentCellChanged(object? sender, EventArgs e)
    {
        if (_bindingFlexibleSelection) return;
        if (sender is DataGrid parent && parent.Name == "FlexibleSessionsGrid" &&
            parent.CurrentCell.Item is FlexibleTestSessionRecord session && parent.Items.Contains(session))
        {
            if (!ReferenceEquals(session, _selectedFlexibleSession)) BindFlexibleTestingSession(session);
            return;
        }
        if (sender is DataGrid specimens && specimens.Name == "FlexibleSpecimensGrid" &&
            specimens.CurrentCell.Item is FlexibleTestSpecimenRecord specimen && specimens.Items.Contains(specimen))
        {
            if (!ReferenceEquals(specimen, _selectedFlexibleSpecimen)) SelectFlexibleSpecimen(specimen);
            return;
        }
        if (sender is DataGrid grid && IsFlexibleReadingGrid(grid.Name) && grid.CurrentCell.Item is { } item)
            _lastSelectedFlexibleReadingByGrid[grid.Name] = item;
    }

    private void FlexibleReadingGrid_SelectedCellsChanged(object? sender, SelectedCellsChangedEventArgs e)
    {
        if (sender is DataGrid grid && IsFlexibleReadingGrid(grid.Name) &&
            e.AddedCells.LastOrDefault().Item is { } item)
            _lastSelectedFlexibleReadingByGrid[grid.Name] = item;
    }

    private static bool IsFlexibleReadingGrid(string gridName) => gridName is
        "CompressionPointsGrid" or "StressRelaxationGrid" or "RecoveryMeasurementsGrid" or "ShoreHardnessGrid";

    private void FlexibleParentGrid_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (_bindingFlexibleSelection) return;
        if (sender is not DataGrid grid) return;
        var cell = FindVisualParent<DataGridCell>(e.OriginalSource as DependencyObject);
        if (cell?.DataContext is FlexibleTestSessionRecord session && grid.Name == "FlexibleSessionsGrid")
        {
            if (!ReferenceEquals(_selectedFlexibleSession, session) && !BindFlexibleTestingSession(session)) e.Handled = true;
            return;
        }
        if (cell?.DataContext is FlexibleTestSpecimenRecord specimen && grid.Name == "FlexibleSpecimensGrid")
        {
            if (!ReferenceEquals(_selectedFlexibleSpecimen, specimen))
                if (!SelectFlexibleSpecimen(specimen)) e.Handled = true;
            return;
        }
        if (cell?.DataContext is not null && IsFlexibleReadingGrid(grid.Name))
            _lastSelectedFlexibleReadingByGrid[grid.Name] = cell.DataContext;
    }

    private string FlexibleMaterialDisplayName(string materialId)
    {
        var row = _nativeMaterialRows.FirstOrDefault(x => string.Equals(x.MaterialID, materialId, StringComparison.OrdinalIgnoreCase));
        return row is null ? materialId : BuildNativeMaterialDisplayName(row);
    }

    private bool BindFlexibleTestingSession(FlexibleTestSessionRecord? session)
    {
        if (_bindingFlexibleSelection) return false;
        if (!TryPrepareFlexibleParentSwitch())
        { RestoreFlexibleParentSelection(); return false; }
        var specimens = session is null ? [] : _flexibleSpecimens
            .Where(x => string.Equals(x.FlexibleTestSessionId, session.FlexibleTestSessionId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.SpecimenLabel, FlexibleSpecimenLabelComparer.Ascending).ToList();
        var selected = specimens.FirstOrDefault();
        var oldSpecimens = FlexibleSpecimensGrid.ItemsSource;
        var applied = false;
        _bindingFlexibleSelection = true;
        try
        {
            FlexibleSpecimensGrid.ItemsSource = specimens;
            if (!BindFlexibleSpecimenRows(selected))
            { FlexibleSpecimensGrid.ItemsSource = oldSpecimens; return false; }
            _selectedFlexibleSession = session;
            _selectedFlexibleSpecimen = selected;
            SetFlexibleParentSelection(FlexibleSessionsGrid, session);
            SetFlexibleParentSelection(FlexibleSpecimensGrid, selected);
            foreach (var column in FlexibleSpecimensGrid.Columns) column.SortDirection = null;
            RefreshFlexibleComparisons(session);
            applied = true;
            return true;
        }
        finally
        {
            _bindingFlexibleSelection = false;
            if (!applied) RestoreFlexibleParentSelection();
            SyncFastFlexibleViews();
        }
    }

    private bool BindFlexibleSpecimenRows(FlexibleTestSpecimenRecord? specimen)
    {
        if (!TryCloseFlexibleReadingEditsForRebind())
        {
            SetFlexibleStatus("Finish or correct the active reading before changing specimen or session.", true);
            return false;
        }
        _lastSelectedFlexibleReadingByGrid.Clear();
        var id = specimen?.SpecimenId;
        var previous = FlexibleEditableGridNames.Where(IsFlexibleReadingGrid)
            .Select(name => FindName(name)).OfType<DataGrid>().ToDictionary(grid => grid, grid => grid.ItemsSource);
        try
        {
            SetRows("CompressionPointsGrid", _compressionPoints.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ToList());
            SetRows("StressRelaxationGrid", _stressRelaxationPoints.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ThenBy(x => FlexibleMaterialTestingService.ParseOptional(x.ElapsedTimeSeconds)).ToList());
            SetRows("RecoveryMeasurementsGrid", _recoveryMeasurements.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ToList());
            SetRows("ShoreHardnessGrid", _shoreHardnessReadings.Where(x => x.SpecimenId == id).OrderBy(x => x.ShoreScale).ToList());
            RefreshSavedRelaxationTabVisibility(specimen);
            SyncFastFlexibleViews();
            return true;
        }
        catch (InvalidOperationException)
        {
            foreach (var entry in previous) entry.Key.ItemsSource = entry.Value;
            SetFlexibleStatus("Finish or correct the active reading before changing specimen or session.", true);
            return false;
        }
        void SetRows(string name, System.Collections.IEnumerable rows) { if (FindName(name) is DataGrid grid) grid.ItemsSource = rows; }
    }

    private bool SelectFlexibleSpecimen(FlexibleTestSpecimenRecord specimen)
    {
        if (_bindingFlexibleSelection || !IsCurrentFlexibleSpecimen(specimen)) return false;
        if (!TryPrepareFlexibleParentSwitch()) { RestoreFlexibleParentSelection(); return false; }
        var applied = false;
        _bindingFlexibleSelection = true;
        try
        {
            if (!BindFlexibleSpecimenRows(specimen)) return false;
            _selectedFlexibleSpecimen = specimen;
            SetFlexibleParentSelection(FlexibleSpecimensGrid, specimen);
            applied = true;
            return true;
        }
        finally
        {
            _bindingFlexibleSelection = false;
            if (!applied) RestoreFlexibleParentSelection();
            SyncFastFlexibleViews();
        }
    }

    private bool IsCurrentFlexibleSpecimen(FlexibleTestSpecimenRecord specimen) =>
        BelongsToFlexibleSession(specimen, _selectedFlexibleSession) &&
        _flexibleSpecimens.Contains(specimen) && FlexibleSpecimensGrid.Items.Contains(specimen);

    private bool TryPrepareFlexibleParentSwitch()
    {
        if (!FlushFastFlexibleChanges()) return false;
        _bindingFlexibleSelection = true;
        try
        {
            if (!TryCloseFlexibleReadingEditsForRebind() || !TryCommitFlexibleGridEdits(FlexibleSpecimensGrid) ||
                !TryCommitFlexibleGridEdits(FlexibleSessionsGrid)) return false;
            if (_selectedFlexibleSpecimen is not { } current || !_flexibleSpecimens.Contains(current)) return true;
            var id = current.SpecimenId;
            return FlexibleMaterialTestingService.Validate([current], _compressionPoints.Where(x => x.SpecimenId == id),
                _stressRelaxationPoints.Where(x => x.SpecimenId == id), _recoveryMeasurements.Where(x => x.SpecimenId == id),
                _shoreHardnessReadings.Where(x => x.SpecimenId == id)).Count == 0;
        }
        finally { _bindingFlexibleSelection = false; }
    }

    private static bool BelongsToFlexibleSession(FlexibleTestSpecimenRecord specimen, FlexibleTestSessionRecord? session) =>
        session is not null && string.Equals(specimen.FlexibleTestSessionId, session.FlexibleTestSessionId, StringComparison.OrdinalIgnoreCase);

    private static T? ResolveFlexibleParentSelection<T>(DataGrid grid, SelectionChangedEventArgs e) where T : class =>
        e.AddedItems.OfType<T>().FirstOrDefault() ?? grid.SelectedItem as T;

    private static void SetFlexibleParentSelection(DataGrid grid, object? row)
    {
        grid.SelectedItem = row;
        grid.CurrentCell = row is null ? default : new DataGridCellInfo(row,
            grid.CurrentCell.Column is { } column && grid.Columns.Contains(column) ? column : grid.Columns[0]);
    }

    private void RestoreFlexibleParentSelection()
    {
        _bindingFlexibleSelection = true;
        try
        {
            SetFlexibleParentSelection(FlexibleSessionsGrid, _selectedFlexibleSession);
            SetFlexibleParentSelection(FlexibleSpecimensGrid, _selectedFlexibleSpecimen);
            SetFlexibleStatus("Finish or correct the active reading before changing specimen or session.", true);
        }
        finally { _bindingFlexibleSelection = false; SyncFastFlexibleViews(); }
    }

    private void RefreshSavedRelaxationTabVisibility(FlexibleTestSpecimenRecord? specimen)
    {
        var hasSavedReadings = specimen is not null &&
            _stressRelaxationPoints.Any(row => row.SpecimenId == specimen.SpecimenId);
        if (!hasSavedReadings && FlexibleRelaxationTab.IsSelected)
            FlexibleTestingTabs.SelectedIndex = 0;
        FlexibleRelaxationTab.Visibility = hasSavedReadings ? Visibility.Visible : Visibility.Collapsed;
    }

    private bool TryCloseFlexibleReadingEditsForRebind()
    {
        if (!CommitFastFlexibleEditors()) return false;
        foreach (var gridName in FlexibleEditableGridNames.Where(IsFlexibleReadingGrid))
        {
            if (FindName(gridName) is not DataGrid grid) continue;
            if (!TryCommitFlexibleGridEdits(grid)) return false;
        }
        return true;
    }

    private static bool TryCommitFlexibleGridEdits(DataGrid grid)
    {
        try
        {
            if (!grid.CommitEdit(DataGridEditingUnit.Cell, true) || !grid.CommitEdit(DataGridEditingUnit.Row, true)) return false;
            if (grid.ItemsSource is { } source && CollectionViewSource.GetDefaultView(source) is IEditableCollectionView editable)
            {
                if (editable.IsAddingNew) editable.CommitNew();
                if (editable.IsEditingItem) editable.CommitEdit();
            }
            return true;
        }
        catch (InvalidOperationException) { return false; }
    }

    private void FlexibleSpecimensGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
        if (sender is not DataGrid grid || e.Column.SortMemberPath != nameof(FlexibleTestSpecimenRecord.SpecimenLabel)) return;
        e.Handled = true;
        if (!TryCommitFlexibleGridEdits(grid))
        {
            SetFlexibleStatus("Finish or correct the specimen edit before sorting.", true);
            return;
        }
        if (CollectionViewSource.GetDefaultView(grid.ItemsSource) is not ListCollectionView view) return;
        var descending = e.Column.SortDirection == ListSortDirection.Ascending;
        view.CustomSort = new FlexibleSpecimenLabelComparer(descending);
        foreach (var column in grid.Columns) column.SortDirection = null;
        e.Column.SortDirection = descending ? ListSortDirection.Descending : ListSortDirection.Ascending;
    }

    private void FlexibleSessionsGrid_Sorting(object sender, DataGridSortingEventArgs e)
    {
        if (sender is not DataGrid grid || e.Column.SortMemberPath != nameof(FlexibleTestSessionRecord.SessionLabel)) return;
        e.Handled = true;
        foreach (var name in FlexibleEditableGridNames)
            if (FindName(name) is DataGrid editable && !TryCommitFlexibleGridEdits(editable))
            { SetFlexibleStatus("Finish or correct the edit before sorting sessions.", true); return; }
        if (CollectionViewSource.GetDefaultView(grid.ItemsSource) is not ListCollectionView view) return;
        var descending = e.Column.SortDirection == ListSortDirection.Ascending;
        view.CustomSort = new FlexibleSpecimenLabelComparer(descending);
        foreach (var column in grid.Columns) column.SortDirection = null;
        e.Column.SortDirection = descending ? ListSortDirection.Descending : ListSortDirection.Ascending;
    }

    private void FlexibleSessionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_bindingFlexibleSelection || sender is not DataGrid grid) return;
        var session = ResolveFlexibleParentSelection<FlexibleTestSessionRecord>(grid, e);
        if (session is not null && !ReferenceEquals(_selectedFlexibleSession, session)) BindFlexibleTestingSession(session);
    }

    private void FlexibleSpecimensGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_bindingFlexibleSelection || sender is not DataGrid grid) return;
        var specimen = ResolveFlexibleParentSelection<FlexibleTestSpecimenRecord>(grid, e);
        if (specimen is null || ReferenceEquals(_selectedFlexibleSpecimen, specimen)) return;
        SelectFlexibleSpecimen(specimen);
    }

    private void AddFlexibleSession_Click(object sender, RoutedEventArgs e)
    {
        if (!FlushFastFlexibleChanges()) return;
        if (!TryGetFlexibleTemplateSettings(out var settings, out var error))
        { SetFlexibleStatus(error, true); return; }
        foreach (var name in FlexibleEditableGridNames)
            if (FindName(name) is DataGrid grid && !TryCommitFlexibleGridEdits(grid)) return;
        var material = _lastSelectedNativeMaterial is { IsArchived: false } selected ? selected
            : _nativeMaterialRows.FirstOrDefault(x => !x.IsArchived && !string.IsNullOrWhiteSpace(x.MaterialID));
        if (material is null) { MessageBox.Show(this, "Create or select an active MaterialID first.", "Flexible Material Testing"); return; }
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);
        var row = new FlexibleTestSessionRecord
        {
            FlexibleTestSessionId = Id("FTS"), MaterialID = material.MaterialID, MaterialDisplayName = BuildNativeMaterialDisplayName(material),
            SessionLabel = $"Test Session {_flexibleTestSessions.Count + 1}", MeasuredDate = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture),
            CreatedAtUtc = now, UpdatedAtUtc = now
        };
        _flexibleTestSessions.Add(row);
        var count = (int)FlexibleMaterialTestingService.ParseOptional(settings["Compression specimens per session"])!.Value;
        for (var i = 1; i <= count; i++)
            AddFlexibleTemplateRows(row.FlexibleTestSessionId, $"Specimen {i}", false, settings);
        AddFlexibleTemplateRows(row.FlexibleTestSessionId, "Shore Specimen 1", true, settings);
        if (!SaveFlexibleTesting())
        {
            foreach (var specimen in _flexibleSpecimens.Where(x => x.FlexibleTestSessionId == row.FlexibleTestSessionId).ToList())
            { RemoveFlexibleSpecimenRows(specimen.SpecimenId); _flexibleSpecimens.Remove(specimen); }
            _flexibleTestSessions.Remove(row); return;
        }
        BindFlexibleTestingSession(row); FlexibleSessionsGrid.ScrollIntoView(row);
    }

    private void DeleteFlexibleSession_Click(object sender, RoutedEventArgs e)
    {
        if (!FlushFastFlexibleChanges()) return;
        if (IsAutomationActionBlocked("Flexible test session deletion") || _selectedFlexibleSession is null) return;
        if (MessageBox.Show(this, $"Delete test session '{_selectedFlexibleSession.SessionLabel}' and all specimens and readings?", "Flexible Material Testing", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        foreach (var specimen in _flexibleSpecimens.Where(x => x.FlexibleTestSessionId == _selectedFlexibleSession.FlexibleTestSessionId).ToList())
        {
            RemoveFlexibleSpecimenRows(specimen.SpecimenId); _flexibleSpecimens.Remove(specimen);
        }
        _flexibleTestSessions.Remove(_selectedFlexibleSession); _selectedFlexibleSession = null;
        SaveFlexibleTesting(); BindFlexibleTestingSession(_flexibleTestSessions.FirstOrDefault());
    }

    private void AddFlexibleCompressionSpecimen_Click(object sender, RoutedEventArgs e) => AddFlexibleSpecimen("Compression");
    private void AddFlexibleShoreSpecimen_Click(object sender, RoutedEventArgs e) => AddFlexibleSpecimen("Shore");

    private void AddFlexibleSpecimen(string intendedTest)
    {
        if (!FlushFastFlexibleChanges()) return;
        if (_selectedFlexibleSession is null) { MessageBox.Show(this, "Select or add a test session first.", "Flexible Material Testing"); return; }
        if (!TryGetFlexibleTemplateSettings(out var settings, out var error))
        {
            MessageBox.Show(this, error, "Flexible Material Testing Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        foreach (var name in FlexibleEditableGridNames)
            if (FindName(name) is DataGrid grid && !TryCommitFlexibleGridEdits(grid)) return;
        var shore = intendedTest == "Shore";
        var prefix = shore ? "Shore Specimen " : "Specimen ";
        var number = 1;
        while (_flexibleSpecimens.Any(x => x.FlexibleTestSessionId == _selectedFlexibleSession.FlexibleTestSessionId &&
                   x.SpecimenLabel == prefix + number)) number++;
        var row = AddFlexibleTemplateRows(_selectedFlexibleSession.FlexibleTestSessionId, prefix + number, shore, settings);
        if (!SaveFlexibleTesting())
        {
            RemoveFlexibleSpecimenRows(row.SpecimenId);
            _flexibleSpecimens.Remove(row); _selectedFlexibleSpecimen = null; BindFlexibleTestingSession(_selectedFlexibleSession); return;
        }
        BindFlexibleTestingSession(_selectedFlexibleSession);
        FlexibleSpecimensGrid.SelectedItem = row; FlexibleSpecimensGrid.ScrollIntoView(row);
    }

    private void AddCompressionPoint_Click(object sender, RoutedEventArgs e)
    {
        if (!FlushFastFlexibleChanges()) return;
        if (!TryGetFlexibleDefaults(out _, out _, out _, out var displacement, out var error))
        {
            MessageBox.Show(this, error, "Flexible Material Testing Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        AddChild(_compressionPoints, CreateCompressionPoint(SelectedSpecimenId(), displacement, "30"));
    }

    private static CompressionPointRecord CreateCompressionPoint(string specimenId, string displacement, string holdSeconds) => new()
    {
        CompressionPointId = Id("CMP"),
        SpecimenId = specimenId,
        CycleNumber = 1,
        TargetStrainPercent = "20",
        DisplacementMm = displacement,
        HoldTimeSeconds = holdSeconds
    };
    private void AddRecoveryMeasurement_Click(object sender, RoutedEventArgs e)
    {
        if (!FlushFastFlexibleChanges()) return;
        if (!TryGetRecoveryHoldDefault(out var holdSeconds, out var error))
        {
            SetFlexibleStatus(error, true);
            return;
        }
        AddChild(_recoveryMeasurements, CreateRecoveryMeasurement(
            Id("RCV"), SelectedSpecimenId(), _selectedFlexibleSpecimen?.InitialHeightMm ?? string.Empty, holdSeconds));
    }

    private static RecoveryMeasurementRecord CreateRecoveryMeasurement(string id, string specimenId, string initialHeight, string hold) => new()
    {
        RecoveryMeasurementId = id, SpecimenId = specimenId, CycleNumber = 1,
        InitialHeightMm = initialHeight, RestTimeSeconds = "60", CompressionPercent = "20", CompressionHoldSeconds = hold
    };

    private static bool IsValidRecoveryHoldSeconds(string value) => FlexibleMaterialTestingService.ParseOptional(value) is >= 0d;

    private bool TryGetRecoveryHoldDefault(out string holdSeconds, out string error)
    {
        holdSeconds = _nativeSettingsRows.FirstOrDefault(row =>
            string.Equals(row.Section, FlexibleSettingsSection, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(row.Parameter, FlexibleRecoveryHoldParameter, StringComparison.OrdinalIgnoreCase))?.Value?.Trim() ?? string.Empty;
        error = IsValidRecoveryHoldSeconds(holdSeconds)
            ? string.Empty : "Default recovery compressed hold must be a nonnegative number in seconds.";
        return error.Length == 0;
    }
    private void AddShoreReading_Click(object sender, RoutedEventArgs e)
    {
        if (!FlushFastFlexibleChanges()) return;
        if (!TryGetFlexibleTemplateSettings(out var settings, out var error)) { SetFlexibleStatus(error, true); return; }
        var specimen = ResolveSelectedFlexibleSpecimen();
        var reading = FlexibleSessionTemplateService.CreateShore(specimen?.SpecimenId ?? string.Empty, settings);
        AddChild(_shoreHardnessReadings, reading);
    }

    private FlexibleTestSpecimenRecord AddFlexibleTemplateRows(string sessionId, string label, bool shore,
        IReadOnlyDictionary<string, string> settings)
    {
        var rows = FlexibleSessionTemplateService.CreateRows(sessionId, label, shore, settings);
        _flexibleSpecimens.Add(rows.Specimen);
        foreach (var row in rows.Compression) _compressionPoints.Add(row);
        foreach (var row in rows.Recovery) _recoveryMeasurements.Add(row);
        foreach (var row in rows.Shore) _shoreHardnessReadings.Add(row);
        return rows.Specimen;
    }

    private bool TryGetFlexibleTemplateSettings(out Dictionary<string, string> settings, out string error)
    {
        settings = _nativeSettingsRows.Where(x => x.Section == FlexibleSettingsSection)
            .GroupBy(x => x.Parameter).ToDictionary(x => x.Key, x => x.First().Value.Trim());
        if (!TryGetFlexibleDefaults(out _, out _, out _, out _, out error) || !TryGetRecoveryHoldDefault(out _, out error)) return false;
        foreach (var entry in FlexibleSessionTemplateService.Defaults)
            if (!settings.TryGetValue(entry.Name, out var value) || !FlexibleSessionTemplateService.ValidateSetting(entry.Name, value))
            { error = $"{entry.Name} has an invalid value in Settings Manager."; return false; }
        return true;
    }

    private void AddChild<T>(ObservableCollection<T> collection, T row)
    {
        var specimen = ResolveSelectedFlexibleSpecimen();
        if (specimen is null) { MessageBox.Show(this, "Select or add a specimen first.", "Flexible Material Testing"); return; }
        collection.Add(row); SaveFlexibleTesting(); BindFlexibleSpecimenRows(_selectedFlexibleSpecimen);
    }

    private void DeleteFlexibleSpecimen_Click(object sender, RoutedEventArgs e)
    {
        if (!FlushFastFlexibleChanges()) return;
        if (IsAutomationActionBlocked("Flexible specimen deletion")) return;
        var specimen = ResolveSelectedFlexibleSpecimen();
        if (specimen is null) return;
        if (MessageBox.Show(this, $"Delete {specimen.SpecimenLabel} and all of its readings?", "Flexible Material Testing", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        var id = specimen.SpecimenId;
        RemoveFlexibleSpecimenRows(id); _flexibleSpecimens.Remove(specimen); _selectedFlexibleSpecimen = null;
        SaveFlexibleTesting(); BindFlexibleTestingSession(_selectedFlexibleSession);
    }

    private void DeleteFlexibleReading_Click(object sender, RoutedEventArgs e)
    {
        if (!FlushFastFlexibleChanges()) return;
        if (IsAutomationActionBlocked("Flexible reading deletion")) return;
        var removed = FlexibleTestingTabs.SelectedIndex switch
        {
            0 => RemoveSelected("CompressionPointsGrid", _compressionPoints), 1 => RemoveSelected("StressRelaxationGrid", _stressRelaxationPoints),
            2 => RemoveSelected("RecoveryMeasurementsGrid", _recoveryMeasurements), 3 => RemoveSelected("ShoreHardnessGrid", _shoreHardnessReadings), _ => false
        };
        if (removed) { SaveFlexibleTesting(); SyncFastFlexibleViews(); }
        else SetFlexibleStatus("Select a reading cell in the current measurement tab before deleting.", true);
    }

    private bool RemoveSelected<T>(string gridName, ObservableCollection<T> rows) where T : class
    {
        if (FindName(gridName) is not DataGrid grid) return false;
        _lastSelectedFlexibleReadingByGrid.TryGetValue(gridName, out var rememberedItem);
        var selectedCellItem = grid.SelectedCells.FirstOrDefault().Item;
        var row = ResolveFlexibleReading<T>(grid.CurrentCell.Item, selectedCellItem, grid.SelectedItem, rememberedItem);
        if (row is null) return false;
        var visibleRows = grid.ItemsSource as System.Collections.IList;
        var deletedIndex = visibleRows?.IndexOf(row) ?? -1;
        var removed = RemoveFlexibleReadingFromCollections(row, rows, visibleRows);
        if (removed)
        {
            grid.Items.Refresh();
            SelectAdjacentFlexibleReading(grid, gridName, visibleRows, deletedIndex);
        }
        return removed;
    }

    private void SelectAdjacentFlexibleReading(
        DataGrid grid,
        string gridName,
        System.Collections.IList? visibleRows,
        int deletedIndex)
    {
        if (visibleRows is null || visibleRows.Count == 0)
        {
            _lastSelectedFlexibleReadingByGrid.Remove(gridName);
            return;
        }

        var nextIndex = Math.Clamp(deletedIndex < 0 ? 0 : deletedIndex, 0, visibleRows.Count - 1);
        var nextRow = visibleRows[nextIndex];
        if (nextRow is null)
        {
            _lastSelectedFlexibleReadingByGrid.Remove(gridName);
            return;
        }
        var nextColumn = grid.Columns.FirstOrDefault();
        _lastSelectedFlexibleReadingByGrid[gridName] = nextRow;
        grid.SelectedItem = nextRow;
        if (nextColumn is not null) grid.CurrentCell = new DataGridCellInfo(nextRow, nextColumn);
        grid.ScrollIntoView(nextRow);
    }

    private static T? ResolveFlexibleReading<T>(
        object? currentCellItem,
        object? selectedCellItem,
        object? selectedItem,
        object? rememberedItem) where T : class =>
        currentCellItem as T ?? selectedCellItem as T ?? selectedItem as T ?? rememberedItem as T;

    private static bool RemoveFlexibleReadingFromCollections<T>(
        T row,
        ICollection<T> canonicalRows,
        System.Collections.IList? visibleRows) where T : class
    {
        if (!canonicalRows.Remove(row)) return false;
        if (visibleRows is not null && !ReferenceEquals(visibleRows, canonicalRows) && visibleRows.Contains(row))
            visibleRows.Remove(row);
        return true;
    }

    private void FlexibleTestingGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        if (sender is not DataGrid grid || e.Row.Item is not { } editedRow) return;
        Dispatcher.BeginInvoke(new Action(() =>
        {
            if (SaveFlexibleTesting())
            {
                if (editedRow is RecoveryMeasurementRecord recovery && e.EditAction == DataGridEditAction.Commit &&
                    GetBoundPropertyName(e.Column) == nameof(RecoveryMeasurementRecord.InitialHeightMm) &&
                    e.Column.GetCellContent(e.Row) is not TextBox)
                    recovery.AcceptInputCommit();
                RefreshFlexibleCalculatedCells(grid, editedRow);
            }
        }), DispatcherPriority.ContextIdle);
    }

    private static void RefreshFlexibleCalculatedCells(DataGrid grid, object rowItem)
    {
        foreach (var column in grid.Columns.Where(column => column.IsReadOnly))
        {
            var cell = GetWorkflowGridCell(grid, rowItem, column);
            var text = cell is null ? null : FindVisualChild<TextBlock>(cell);
            text?.GetBindingExpression(TextBlock.TextProperty)?.UpdateTarget();
        }
    }

    private bool SaveFlexibleTesting()
    {
        foreach (var session in _flexibleTestSessions) session.MaterialDisplayName = FlexibleMaterialDisplayName(session.MaterialID);
        if (_flexibleTestSessions.Any(x => string.IsNullOrWhiteSpace(x.MaterialID) || !_nativeMaterialRows.Any(m => !m.IsArchived && string.Equals(m.MaterialID, x.MaterialID, StringComparison.OrdinalIgnoreCase))))
        {
            SetFlexibleStatus("Each test session must reference an active MaterialID.", true); return false;
        }
        FlexibleMaterialTestingService.Recalculate(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements);
        var errors = FlexibleMaterialTestingService.Validate(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements, _shoreHardnessReadings);
        if (errors.Count > 0) { SetFlexibleStatus(errors[0], true); return false; }
        try
        {
            _database.SynchronizeFlexibleTestingGraph(_flexibleTestSessions, _flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements, _shoreHardnessReadings);
        }
        catch (Exception ex)
        {
            SetFlexibleStatus($"Save failed: {ex.Message}", true);
            return false;
        }
        _flexibleEditDirty = false;
        _flexibleSaveTimer?.Stop();
        foreach (var recovery in _recoveryMeasurements) recovery.AcceptInputCommit();
        RefreshSavedFlexibleMaterialEvidence();
        RefreshFlexibleComparisons(_selectedFlexibleSession);
        RefreshSavedRelaxationTabVisibility(_selectedFlexibleSpecimen);
        QueueNativeMaterialDependentIntelligenceRefresh();
        RefreshNativeMaterialTestStatusFromNativeInputTabs(markDirty: false);
        SetFlexibleStatus($"Saved {_flexibleTestSessions.Count} session(s) and {_flexibleSpecimens.Count} specimen(s); raw readings and method snapshots preserved.", false);
        return true;
    }

    private void RefreshFlexibleComparisons(FlexibleTestSessionRecord? session)
    {
        _flexibleComparisonRows.Clear();
        if (session is null) return;
        var specimens = _flexibleSpecimens.Where(x => x.FlexibleTestSessionId == session.FlexibleTestSessionId).ToList();
        var ids = specimens.Select(x => x.SpecimenId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var row in FlexibleMaterialTestingService.BuildComparisons(specimens, _compressionPoints.Where(x => ids.Contains(x.SpecimenId)).ToList(), _shoreHardnessReadings.Where(x => ids.Contains(x.SpecimenId)).ToList(), _recoveryMeasurements.Where(x => ids.Contains(x.SpecimenId)).ToList())) _flexibleComparisonRows.Add(row);
    }

    private void RemoveFlexibleSpecimenRows(string id)
    {
        foreach (var row in _compressionPoints.Where(x => x.SpecimenId == id).ToList()) _compressionPoints.Remove(row);
        foreach (var row in _stressRelaxationPoints.Where(x => x.SpecimenId == id).ToList()) _stressRelaxationPoints.Remove(row);
        foreach (var row in _recoveryMeasurements.Where(x => x.SpecimenId == id).ToList()) _recoveryMeasurements.Remove(row);
        foreach (var row in _shoreHardnessReadings.Where(x => x.SpecimenId == id).ToList()) _shoreHardnessReadings.Remove(row);
    }

    private FlexibleTestSpecimenRecord? ResolveSelectedFlexibleSpecimen()
    {
        return _selectedFlexibleSpecimen is { } specimen && IsCurrentFlexibleSpecimen(specimen) ? specimen : null;
    }

    private string SelectedSpecimenId() => ResolveSelectedFlexibleSpecimen()?.SpecimenId ?? string.Empty;
    private static string Id(string prefix) => prefix + "-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    private void SetFlexibleStatus(string text, bool error) { FlexibleTestingStatusText.Text=text; FlexibleTestingStatusText.Foreground=error?System.Windows.Media.Brushes.Firebrick:System.Windows.Media.Brushes.DarkSlateGray; }
    private sealed record FlexibleMaterialChoice(string MaterialID, string DisplayName);

    private void EnsureFlexibleTestingDefaultSettings()
    {
        var added = false;
        var oldThickness = _nativeSettingsRows.FirstOrDefault(x => x.Section == FlexibleSettingsSection &&
            x.Parameter == FlexibleDefaultThicknessParameter);
        if (oldThickness is not null && oldThickness.Notes.Contains("used by Shore readings", StringComparison.Ordinal))
        {
            oldThickness.Notes = "New compression specimen metadata only; Shore uses Default Shore thickness";
            added = true;
        }
        foreach (var defaultRow in GetDefaultNativeSettingsRows().Where(x =>
                     string.Equals(x.Section, FlexibleSettingsSection, StringComparison.OrdinalIgnoreCase)))
        {
            if (_nativeSettingsRows.Any(x =>
                    string.Equals(x.Section, defaultRow.Section, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(x.Parameter, defaultRow.Parameter, StringComparison.OrdinalIgnoreCase)))
                continue;
            _nativeSettingsRows.Add(defaultRow);
            added = true;
        }
        if (added) SaveCanonicalNativeSettings();
    }

    private bool TryGetFlexibleDefaults(
        out string diameter,
        out string height,
        out string thickness,
        out string displacement,
        out string error)
    {
        diameter = Setting(FlexibleDefaultDiameterParameter);
        height = Setting(FlexibleDefaultHeightParameter);
        thickness = Setting(FlexibleDefaultThicknessParameter);
        displacement = Setting(FlexibleDefaultDisplacementParameter);
        error = string.Empty;
        if (FlexibleMaterialTestingService.ParseOptional(diameter) is not > 0d)
            error = "Default specimen diameter must be a positive number in mm.";
        else if (FlexibleMaterialTestingService.ParseOptional(height) is not > 0d)
            error = "Default specimen height must be a positive number in mm.";
        else if (FlexibleMaterialTestingService.ParseOptional(thickness) is not > 0d)
            error = "Default specimen thickness must be a positive number in mm.";
        else if (FlexibleMaterialTestingService.ParseOptional(displacement) is not > 0d)
            error = "Default compression displacement must be a positive number in mm.";
        return error.Length == 0;

        string Setting(string parameter) => _nativeSettingsRows.FirstOrDefault(x =>
            string.Equals(x.Section, FlexibleSettingsSection, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.Parameter, parameter, StringComparison.OrdinalIgnoreCase))?.Value?.Trim() ?? string.Empty;
    }
}
