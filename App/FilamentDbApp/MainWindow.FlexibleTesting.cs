using FilamentDbApp.Models;
using FilamentDbApp.Services;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace FilamentDbApp;

public partial class MainWindow
{
    private const string FlexibleSettingsSection = "Flexible Material Testing";
    private const string FlexibleDefaultDiameterParameter = "Default specimen diameter";
    private const string FlexibleDefaultHeightParameter = "Default specimen height";
    private const string FlexibleDefaultThicknessParameter = "Default specimen thickness";
    private readonly ObservableCollection<FlexibleTestSessionRecord> _flexibleTestSessions = new();
    private readonly ObservableCollection<FlexibleTestSpecimenRecord> _flexibleSpecimens = new();
    private readonly ObservableCollection<CompressionPointRecord> _compressionPoints = new();
    private readonly ObservableCollection<StressRelaxationPointRecord> _stressRelaxationPoints = new();
    private readonly ObservableCollection<RecoveryMeasurementRecord> _recoveryMeasurements = new();
    private readonly ObservableCollection<ShoreHardnessReadingRecord> _shoreHardnessReadings = new();
    private readonly ObservableCollection<FlexibleComparisonRow> _flexibleComparisonRows = new();
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
        BindFlexibleTestingSession(_flexibleTestSessions.FirstOrDefault(x => x.IsActive) ?? _flexibleTestSessions.FirstOrDefault());
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
        var id = specimen?.SpecimenId;
        SetRows("CompressionPointsGrid", _compressionPoints.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ToList());
        SetRows("StressRelaxationGrid", _stressRelaxationPoints.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ThenBy(x => FlexibleMaterialTestingService.ParseOptional(x.ElapsedTimeSeconds)).ToList());
        SetRows("RecoveryMeasurementsGrid", _recoveryMeasurements.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ToList());
        SetRows("ShoreHardnessGrid", _shoreHardnessReadings.Where(x => x.SpecimenId == id).OrderBy(x => x.ShoreScale).ToList());
        void SetRows(string name, System.Collections.IEnumerable rows) { if (FindName(name) is DataGrid grid) grid.ItemsSource = rows; }
    }

    private void FlexibleSessionsGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is DataGrid { SelectedItem: FlexibleTestSessionRecord session }) BindFlexibleTestingSession(session);
    }

    private void FlexibleSpecimensGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid grid) return;
        _selectedFlexibleSpecimen = grid.SelectedItem as FlexibleTestSpecimenRecord;
        BindFlexibleSpecimenRows(_selectedFlexibleSpecimen);
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
        if (!TryGetFlexibleSpecimenDefaults(out var diameter, out var height, out var thickness, out var error))
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
            methodPoints.Add(new CompressionPointRecord { CompressionPointId=Id("CMP"), SpecimenId=row.SpecimenId, CycleNumber=1, TargetStrainPercent="20", HoldTimeSeconds="10" });
            methodPoints.Add(new CompressionPointRecord { CompressionPointId=Id("CMP"), SpecimenId=row.SpecimenId, CycleNumber=1, TargetStrainPercent="20", HoldTimeSeconds="30" });
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

    private void AddCompressionPoint_Click(object sender, RoutedEventArgs e) => AddChild(_compressionPoints, new CompressionPointRecord { CompressionPointId=Id("CMP"),SpecimenId=SelectedSpecimenId(),CycleNumber=1,TargetStrainPercent="20",HoldTimeSeconds="30" });
    private void AddRelaxationPoint_Click(object sender, RoutedEventArgs e) => AddChild(_stressRelaxationPoints, new StressRelaxationPointRecord { RelaxationPointId=Id("REL"),SpecimenId=SelectedSpecimenId(),CycleNumber=1,CompressionPercent="25",ElapsedTimeSeconds="10" });
    private void AddRecoveryMeasurement_Click(object sender, RoutedEventArgs e) => AddChild(_recoveryMeasurements, new RecoveryMeasurementRecord { RecoveryMeasurementId=Id("RCV"),SpecimenId=SelectedSpecimenId(),CycleNumber=1,InitialHeightMm=_selectedFlexibleSpecimen?.InitialHeightMm??string.Empty,RestTimeSeconds="60",CompressionPercent="25" });
    private void AddShoreReading_Click(object sender, RoutedEventArgs e) => AddChild(_shoreHardnessReadings, new ShoreHardnessReadingRecord { ShoreReadingId=Id("SHR"),SpecimenId=SelectedSpecimenId(),ShoreScale="A",SpecimenThicknessMm=_selectedFlexibleSpecimen?.ThicknessMm??string.Empty });

    private void AddChild<T>(ObservableCollection<T> collection, T row)
    {
        if (_selectedFlexibleSpecimen is null) { MessageBox.Show(this, "Select or add a specimen first.", "Flexible Material Testing"); return; }
        collection.Add(row); SaveFlexibleTesting(); BindFlexibleSpecimenRows(_selectedFlexibleSpecimen);
    }

    private void DeleteFlexibleSpecimen_Click(object sender, RoutedEventArgs e)
    {
        if (IsAutomationActionBlocked("Flexible specimen deletion") || _selectedFlexibleSpecimen is null) return;
        if (MessageBox.Show(this, $"Delete {_selectedFlexibleSpecimen.SpecimenLabel} and all of its readings?", "Flexible Material Testing", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        var id = _selectedFlexibleSpecimen.SpecimenId;
        RemoveFlexibleSpecimenRows(id); _flexibleSpecimens.Remove(_selectedFlexibleSpecimen); _selectedFlexibleSpecimen = null;
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
        if (removed) { SaveFlexibleTesting(); BindFlexibleSpecimenRows(_selectedFlexibleSpecimen); }
    }

    private bool RemoveSelected<T>(string gridName, ObservableCollection<T> rows)
    {
        if (FindName(gridName) is not DataGrid grid || grid.SelectedItem is not T row) return false;
        return rows.Remove(row);
    }

    private void FlexibleTestingGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        Dispatcher.BeginInvoke(new Action(() => { SaveFlexibleTesting(); BindFlexibleSpecimenRows(_selectedFlexibleSpecimen); }), DispatcherPriority.ContextIdle);
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

    private string SelectedSpecimenId() => _selectedFlexibleSpecimen?.SpecimenId ?? string.Empty;
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

    private bool TryGetFlexibleSpecimenDefaults(
        out string diameter,
        out string height,
        out string thickness,
        out string error)
    {
        diameter = Setting(FlexibleDefaultDiameterParameter);
        height = Setting(FlexibleDefaultHeightParameter);
        thickness = Setting(FlexibleDefaultThicknessParameter);
        error = string.Empty;
        if (FlexibleMaterialTestingService.ParseOptional(diameter) is not > 0d)
            error = "Default specimen diameter must be a positive number in mm.";
        else if (FlexibleMaterialTestingService.ParseOptional(height) is not > 0d)
            error = "Default specimen height must be a positive number in mm.";
        else if (FlexibleMaterialTestingService.ParseOptional(thickness) is not > 0d)
            error = "Default specimen thickness must be a positive number in mm.";
        return error.Length == 0;

        string Setting(string parameter) => _nativeSettingsRows.FirstOrDefault(x =>
            string.Equals(x.Section, FlexibleSettingsSection, StringComparison.OrdinalIgnoreCase) &&
            string.Equals(x.Parameter, parameter, StringComparison.OrdinalIgnoreCase))?.Value?.Trim() ?? string.Empty;
    }
}
