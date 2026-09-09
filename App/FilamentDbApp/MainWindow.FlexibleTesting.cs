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

    private void InitializeFlexibleMaterialTesting()
    {
        var graph = _database.LoadFlexibleTestingGraph();
        foreach (var row in graph.Sessions) { row.MaterialDisplayName = FlexibleMaterialDisplayName(row.MaterialID); _flexibleTestSessions.Add(row); }
        foreach (var row in graph.Specimens) _flexibleSpecimens.Add(row);
        foreach (var row in graph.Compression) _compressionPoints.Add(row);
        foreach (var row in graph.Relaxation) _stressRelaxationPoints.Add(row);
        foreach (var row in graph.Recovery) _recoveryMeasurements.Add(row);
        foreach (var row in graph.Shore) _shoreHardnessReadings.Add(row);
        FlexibleMaterialTestingService.Recalculate(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements);
        FlexibleSessionsGrid.ItemsSource = _flexibleTestSessions;
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
        if (sender is not DataGrid grid) return;
        var cell = FindVisualParent<DataGridCell>(e.OriginalSource as DependencyObject);
        if (cell?.DataContext is FlexibleTestSessionRecord session && grid.Name == "FlexibleSessionsGrid")
        {
            if (!ReferenceEquals(_selectedFlexibleSession, session)) BindFlexibleTestingSession(session);
            return;
        }
        if (cell?.DataContext is FlexibleTestSpecimenRecord specimen && grid.Name == "FlexibleSpecimensGrid")
        {
            if (!ReferenceEquals(_selectedFlexibleSpecimen, specimen))
            {
                _selectedFlexibleSpecimen = specimen;
                BindFlexibleSpecimenRows(specimen);
            }
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

    private void BindFlexibleTestingSession(FlexibleTestSessionRecord? session)
    {
        _selectedFlexibleSession = session;
        if (FlexibleSessionsGrid.SelectedItem != session) FlexibleSessionsGrid.SelectedItem = session;
        var specimens = session is null ? [] : _flexibleSpecimens
            .Where(x => string.Equals(x.FlexibleTestSessionId, session.FlexibleTestSessionId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.SpecimenLabel, StringComparer.CurrentCultureIgnoreCase).ToList();
        FlexibleSpecimensGrid.ItemsSource = specimens;
        _selectedFlexibleSpecimen = specimens.FirstOrDefault();
        FlexibleSpecimensGrid.SelectedItem = _selectedFlexibleSpecimen;
        BindFlexibleSpecimenRows(_selectedFlexibleSpecimen);
        RefreshFlexibleComparisons(session);
    }

    private void BindFlexibleSpecimenRows(FlexibleTestSpecimenRecord? specimen)
    {
        if (!TryCloseFlexibleReadingEditsForRebind())
        {
            SetFlexibleStatus("Finish or correct the active reading before changing specimen or session.", true);
            return;
        }
        _lastSelectedFlexibleReadingByGrid.Clear();
        var id = specimen?.SpecimenId;
        try
        {
            SetRows("CompressionPointsGrid", _compressionPoints.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ToList());
            SetRows("StressRelaxationGrid", _stressRelaxationPoints.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ThenBy(x => FlexibleMaterialTestingService.ParseOptional(x.ElapsedTimeSeconds)).ToList());
            SetRows("RecoveryMeasurementsGrid", _recoveryMeasurements.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ToList());
            SetRows("ShoreHardnessGrid", _shoreHardnessReadings.Where(x => x.SpecimenId == id).OrderBy(x => x.ShoreScale).ToList());
        }
        catch (InvalidOperationException)
        {
            SetFlexibleStatus("Finish or correct the active reading before changing specimen or session.", true);
        }
        void SetRows(string name, System.Collections.IEnumerable rows) { if (FindName(name) is DataGrid grid) grid.ItemsSource = rows; }
    }

    private bool TryCloseFlexibleReadingEditsForRebind()
    {
        foreach (var gridName in FlexibleEditableGridNames.Where(IsFlexibleReadingGrid))
        {
            if (FindName(gridName) is not DataGrid grid) continue;
            try
            {
                if (!grid.CommitEdit(DataGridEditingUnit.Cell, true)) return false;
                if (!grid.CommitEdit(DataGridEditingUnit.Row, true)) return false;

                if (grid.ItemsSource is { } source &&
                    CollectionViewSource.GetDefaultView(source) is IEditableCollectionView editableView)
                {
                    if (editableView.IsAddingNew) editableView.CommitNew();
                    if (editableView.IsEditingItem) editableView.CommitEdit();
                }
            }
            catch (InvalidOperationException)
            {
                return false;
            }
        }
        return true;
    }

    private void FlexibleSessionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid grid) return;
        var session = grid.CurrentCell.Item as FlexibleTestSessionRecord ??
                      grid.SelectedItem as FlexibleTestSessionRecord ??
                      e.AddedItems.OfType<FlexibleTestSessionRecord>().FirstOrDefault();
        if (session is not null && !ReferenceEquals(_selectedFlexibleSession, session)) BindFlexibleTestingSession(session);
    }

    private void FlexibleSpecimensGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid grid) return;
        var specimen = grid.CurrentCell.Item as FlexibleTestSpecimenRecord ??
                       grid.SelectedItem as FlexibleTestSpecimenRecord ??
                       e.AddedItems.OfType<FlexibleTestSpecimenRecord>().FirstOrDefault();
        if (specimen is null || ReferenceEquals(_selectedFlexibleSpecimen, specimen)) return;
        _selectedFlexibleSpecimen = specimen;
        BindFlexibleSpecimenRows(specimen);
    }

    private void AddFlexibleSession_Click(object sender, RoutedEventArgs e)
    {
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
        if (!SaveFlexibleTesting()) { _flexibleTestSessions.Remove(row); return; }
        BindFlexibleTestingSession(row); FlexibleSessionsGrid.ScrollIntoView(row);
    }

    private void DeleteFlexibleSession_Click(object sender, RoutedEventArgs e)
    {
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
        if (_selectedFlexibleSession is null) { MessageBox.Show(this, "Select or add a test session first.", "Flexible Material Testing"); return; }
        if (!TryGetFlexibleDefaults(out var diameter, out var height, out var thickness, out var displacement, out var error))
        {
            MessageBox.Show(this, error, "Flexible Material Testing Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        var number = _flexibleSpecimens.Count(x => x.FlexibleTestSessionId == _selectedFlexibleSession.FlexibleTestSessionId) + 1;
        var row = new FlexibleTestSpecimenRecord
        {
            SpecimenId = Id("TPU"), FlexibleTestSessionId = _selectedFlexibleSession.FlexibleTestSessionId,
            SpecimenLabel = $"Specimen {number}", IntendedTest = intendedTest,
            DiameterMm = diameter, InitialHeightMm = height, ThicknessMm = thickness,
            MethodVersion = intendedTest == "Shore" ? "SHORE-v1" : FlexibleMaterialTestingService.CompressionMethodVersion,
            MethodNotes = intendedTest == "Shore" ? "Comparative in-house test; not ASTM/ISO." : FlexibleMaterialTestingService.CompressionMethodNotes,
            CreatedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)
        };
        _flexibleSpecimens.Add(row); _selectedFlexibleSpecimen = row;
        var methodPoints = new List<CompressionPointRecord>();
        if (string.Equals(intendedTest, "Compression", StringComparison.Ordinal))
        {
            methodPoints.Add(CreateCompressionPoint(row.SpecimenId, displacement, "10"));
            methodPoints.Add(CreateCompressionPoint(row.SpecimenId, displacement, "30"));
            foreach (var point in methodPoints) _compressionPoints.Add(point);
        }
        if (!SaveFlexibleTesting())
        {
            foreach (var point in methodPoints) _compressionPoints.Remove(point);
            _flexibleSpecimens.Remove(row); _selectedFlexibleSpecimen = null; BindFlexibleTestingSession(_selectedFlexibleSession); return;
        }
        BindFlexibleTestingSession(_selectedFlexibleSession);
        FlexibleSpecimensGrid.SelectedItem = row; FlexibleSpecimensGrid.ScrollIntoView(row);
    }

    private void AddCompressionPoint_Click(object sender, RoutedEventArgs e)
    {
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
    private void AddRelaxationPoint_Click(object sender, RoutedEventArgs e) => AddChild(_stressRelaxationPoints, new StressRelaxationPointRecord { RelaxationPointId=Id("REL"),SpecimenId=SelectedSpecimenId(),CycleNumber=1,CompressionPercent="25",ElapsedTimeSeconds="10" });
    private void AddRecoveryMeasurement_Click(object sender, RoutedEventArgs e) => AddChild(_recoveryMeasurements, new RecoveryMeasurementRecord { RecoveryMeasurementId=Id("RCV"),SpecimenId=SelectedSpecimenId(),CycleNumber=1,InitialHeightMm=_selectedFlexibleSpecimen?.InitialHeightMm??string.Empty,RestTimeSeconds="60",CompressionPercent="25" });
    private void AddShoreReading_Click(object sender, RoutedEventArgs e) => AddChild(_shoreHardnessReadings, new ShoreHardnessReadingRecord { ShoreReadingId=Id("SHR"),SpecimenId=SelectedSpecimenId(),ShoreScale="A",SpecimenThicknessMm=_selectedFlexibleSpecimen?.ThicknessMm??string.Empty });

    private void AddChild<T>(ObservableCollection<T> collection, T row)
    {
        var specimen = ResolveSelectedFlexibleSpecimen();
        if (specimen is null) { MessageBox.Show(this, "Select or add a specimen first.", "Flexible Material Testing"); return; }
        collection.Add(row); SaveFlexibleTesting(); BindFlexibleSpecimenRows(_selectedFlexibleSpecimen);
    }

    private void DeleteFlexibleSpecimen_Click(object sender, RoutedEventArgs e)
    {
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
        if (IsAutomationActionBlocked("Flexible reading deletion")) return;
        var removed = FlexibleTestingTabs.SelectedIndex switch
        {
            0 => RemoveSelected("CompressionPointsGrid", _compressionPoints), 1 => RemoveSelected("StressRelaxationGrid", _stressRelaxationPoints),
            2 => RemoveSelected("RecoveryMeasurementsGrid", _recoveryMeasurements), 3 => RemoveSelected("ShoreHardnessGrid", _shoreHardnessReadings), _ => false
        };
        if (removed) SaveFlexibleTesting();
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
            if (SaveFlexibleTesting()) RefreshFlexibleCalculatedCells(grid, editedRow);
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
        RefreshFlexibleComparisons(_selectedFlexibleSession);
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
        foreach (var row in FlexibleMaterialTestingService.BuildComparisons(specimens, _compressionPoints.Where(x => ids.Contains(x.SpecimenId)).ToList(), _shoreHardnessReadings.Where(x => ids.Contains(x.SpecimenId)).ToList())) _flexibleComparisonRows.Add(row);
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
        var specimen = FlexibleSpecimensGrid.CurrentCell.Item as FlexibleTestSpecimenRecord ??
                       FlexibleSpecimensGrid.SelectedItem as FlexibleTestSpecimenRecord ??
                       _selectedFlexibleSpecimen;
        if (specimen is not null) _selectedFlexibleSpecimen = specimen;
        return specimen;
    }

    private string SelectedSpecimenId() => ResolveSelectedFlexibleSpecimen()?.SpecimenId ?? string.Empty;
    private static string Id(string prefix) => prefix + "-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant();
    private void SetFlexibleStatus(string text, bool error) { FlexibleTestingStatusText.Text=text; FlexibleTestingStatusText.Foreground=error?System.Windows.Media.Brushes.Firebrick:System.Windows.Media.Brushes.DarkSlateGray; }
    private sealed record FlexibleMaterialChoice(string MaterialID, string DisplayName);

    private void EnsureFlexibleTestingDefaultSettings()
    {
        var added = false;
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
