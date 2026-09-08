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
    private readonly ObservableCollection<FlexibleTestSpecimenRecord> _flexibleSpecimens = new();
    private readonly ObservableCollection<CompressionPointRecord> _compressionPoints = new();
    private readonly ObservableCollection<StressRelaxationPointRecord> _stressRelaxationPoints = new();
    private readonly ObservableCollection<RecoveryMeasurementRecord> _recoveryMeasurements = new();
    private readonly ObservableCollection<ShoreHardnessReadingRecord> _shoreHardnessReadings = new();
    private readonly ObservableCollection<FlexibleComparisonRow> _flexibleComparisonRows = new();
    private FlexibleTestSpecimenRecord? _selectedFlexibleSpecimen;

    private void InitializeFlexibleMaterialTesting()
    {
        var graph = _database.LoadFlexibleTestingGraph();
        foreach (var row in graph.Specimens) _flexibleSpecimens.Add(row);
        foreach (var row in graph.Compression) _compressionPoints.Add(row);
        foreach (var row in graph.Relaxation) _stressRelaxationPoints.Add(row);
        foreach (var row in graph.Recovery) _recoveryMeasurements.Add(row);
        foreach (var row in graph.Shore) _shoreHardnessReadings.Add(row);
        FlexibleMaterialTestingService.Recalculate(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements);
        if (FindName("FlexibleComparisonGrid") is DataGrid comparisonGrid) comparisonGrid.ItemsSource = _flexibleComparisonRows;
        BindFlexibleTestingRun(ResolveExperimentalRun());
    }

    private void BindFlexibleTestingRun(ExperimentalRunRecord? run)
    {
        var specimens = run is null ? new List<FlexibleTestSpecimenRecord>() : _flexibleSpecimens
            .Where(x => string.Equals(x.ExperimentalRunId, run.ExperimentalRunId, StringComparison.OrdinalIgnoreCase))
            .OrderBy(x => x.SpecimenLabel, StringComparer.CurrentCultureIgnoreCase).ToList();
        if (FindName("FlexibleSpecimensGrid") is DataGrid grid)
        {
            grid.ItemsSource = specimens;
            _selectedFlexibleSpecimen = specimens.FirstOrDefault();
            grid.SelectedItem = _selectedFlexibleSpecimen;
        }
        BindFlexibleSpecimenRows(_selectedFlexibleSpecimen);
        RefreshFlexibleComparisons(run);
    }

    private void BindFlexibleSpecimenRows(FlexibleTestSpecimenRecord? specimen)
    {
        var id = specimen?.SpecimenId;
        SetRows("CompressionPointsGrid", _compressionPoints.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ToList());
        SetRows("StressRelaxationGrid", _stressRelaxationPoints.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ThenBy(x => FlexibleMaterialTestingService.ParseOptional(x.ElapsedTimeSeconds)).ToList());
        SetRows("RecoveryMeasurementsGrid", _recoveryMeasurements.Where(x => x.SpecimenId == id).OrderBy(x => x.CycleNumber).ToList());
        SetRows("ShoreHardnessGrid", _shoreHardnessReadings.Where(x => x.SpecimenId == id).OrderBy(x => x.ShoreScale).ToList());
        void SetRows(string name, System.Collections.IEnumerable rows) { if (FindName(name) is DataGrid childGrid) childGrid.ItemsSource = rows; }
    }

    private void FlexibleSpecimensGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid grid) return;
        _selectedFlexibleSpecimen = grid.SelectedItem as FlexibleTestSpecimenRecord;
        BindFlexibleSpecimenRows(_selectedFlexibleSpecimen);
    }

    private void AddFlexibleCompressionSpecimen_Click(object sender, RoutedEventArgs e) => AddFlexibleSpecimen("Compression");
    private void AddFlexibleShoreSpecimen_Click(object sender, RoutedEventArgs e) => AddFlexibleSpecimen("Shore");

    private void AddFlexibleSpecimen(string intendedTest)
    {
        var run = ResolveExperimentalRun();
        if (run is null) { MessageBox.Show(this, "Select an Experimental Run first.", "Flexible Materials"); return; }
        var number = _flexibleSpecimens.Count(x => x.ExperimentalRunId == run.ExperimentalRunId) + 1;
        var row = new FlexibleTestSpecimenRecord
        {
            SpecimenId = Id("TPU"), ExperimentalRunId = run.ExperimentalRunId,
            SpecimenLabel = $"Specimen {number}", IntendedTest = intendedTest,
            ThicknessMm = intendedTest == "Shore" ? "10" : string.Empty,
            MethodVersion = intendedTest == "Shore" ? "SHORE-v1" : "TPU-COMP-v1",
            CreatedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)
        };
        _flexibleSpecimens.Add(row); _selectedFlexibleSpecimen = row;
        SaveFlexibleTesting(); BindFlexibleTestingRun(run);
        if (FindName("FlexibleSpecimensGrid") is DataGrid grid) { grid.SelectedItem = row; grid.ScrollIntoView(row); }
    }

    private void AddCompressionPoint_Click(object sender, RoutedEventArgs e) => AddChild(_compressionPoints, new CompressionPointRecord { CompressionPointId=Id("CMP"),SpecimenId=SelectedSpecimenId(),CycleNumber=1,TargetStrainPercent="10",HoldTimeSeconds="10" });
    private void AddRelaxationPoint_Click(object sender, RoutedEventArgs e) => AddChild(_stressRelaxationPoints, new StressRelaxationPointRecord { RelaxationPointId=Id("REL"),SpecimenId=SelectedSpecimenId(),CycleNumber=1,CompressionPercent="25",ElapsedTimeSeconds="10" });
    private void AddRecoveryMeasurement_Click(object sender, RoutedEventArgs e) => AddChild(_recoveryMeasurements, new RecoveryMeasurementRecord { RecoveryMeasurementId=Id("RCV"),SpecimenId=SelectedSpecimenId(),CycleNumber=1,InitialHeightMm=_selectedFlexibleSpecimen?.InitialHeightMm??string.Empty,RestTimeSeconds="60",CompressionPercent="25" });
    private void AddShoreReading_Click(object sender, RoutedEventArgs e) => AddChild(_shoreHardnessReadings, new ShoreHardnessReadingRecord { ShoreReadingId=Id("SHR"),SpecimenId=SelectedSpecimenId(),ShoreScale="A",SpecimenThicknessMm=_selectedFlexibleSpecimen?.ThicknessMm??string.Empty });

    private void AddChild<T>(ObservableCollection<T> collection, T row)
    {
        if (_selectedFlexibleSpecimen is null) { MessageBox.Show(this, "Select or add a specimen first.", "Flexible Materials"); return; }
        collection.Add(row); SaveFlexibleTesting(); BindFlexibleSpecimenRows(_selectedFlexibleSpecimen);
    }

    private void DeleteFlexibleSpecimen_Click(object sender, RoutedEventArgs e)
    {
        if (IsAutomationActionBlocked("Flexible specimen deletion") || _selectedFlexibleSpecimen is null) return;
        if (MessageBox.Show(this, $"Delete {_selectedFlexibleSpecimen.SpecimenLabel} and all of its readings?", "Flexible Materials", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        var id = _selectedFlexibleSpecimen.SpecimenId;
        RemoveFlexibleSpecimenRows(id); _flexibleSpecimens.Remove(_selectedFlexibleSpecimen); _selectedFlexibleSpecimen = null;
        SaveFlexibleTesting(); BindFlexibleTestingRun(ResolveExperimentalRun());
    }

    private void DeleteFlexibleReading_Click(object sender, RoutedEventArgs e)
    {
        if (IsAutomationActionBlocked("Flexible reading deletion") || FindName("FlexibleTestingTabs") is not TabControl tabs) return;
        var removed = tabs.SelectedIndex switch
        {
            0 => RemoveSelected("CompressionPointsGrid", _compressionPoints),
            1 => RemoveSelected("StressRelaxationGrid", _stressRelaxationPoints),
            2 => RemoveSelected("RecoveryMeasurementsGrid", _recoveryMeasurements),
            3 => RemoveSelected("ShoreHardnessGrid", _shoreHardnessReadings),
            _ => false
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
        FlexibleMaterialTestingService.Recalculate(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements);
        var errors = FlexibleMaterialTestingService.Validate(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements, _shoreHardnessReadings);
        if (errors.Count > 0) { SetFlexibleStatus(errors[0], true); return false; }
        _database.SynchronizeFlexibleTestingGraph(_flexibleSpecimens, _compressionPoints, _stressRelaxationPoints, _recoveryMeasurements, _shoreHardnessReadings);
        RefreshFlexibleComparisons(ResolveExperimentalRun());
        SetFlexibleStatus($"Saved {_flexibleSpecimens.Count} specimen(s); raw readings and method snapshots preserved.", false);
        return true;
    }

    private void RefreshFlexibleComparisons(ExperimentalRunRecord? run)
    {
        _flexibleComparisonRows.Clear();
        if (run is null) return;
        var specimens = _flexibleSpecimens.Where(x => x.ExperimentalRunId == run.ExperimentalRunId).ToList();
        var ids = specimens.Select(x => x.SpecimenId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        foreach (var row in FlexibleMaterialTestingService.BuildComparisons(specimens, _compressionPoints.Where(x => ids.Contains(x.SpecimenId)).ToList(), _shoreHardnessReadings.Where(x => ids.Contains(x.SpecimenId)).ToList())) _flexibleComparisonRows.Add(row);
    }

    private void RemoveFlexibleTestingRun(string runId)
    {
        foreach (var specimen in _flexibleSpecimens.Where(x => x.ExperimentalRunId == runId).ToList()) { RemoveFlexibleSpecimenRows(specimen.SpecimenId); _flexibleSpecimens.Remove(specimen); }
        SaveFlexibleTesting();
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
    private void SetFlexibleStatus(string text, bool error) { if (FindName("FlexibleTestingStatusText") is TextBlock status) { status.Text=text; status.Foreground=error?System.Windows.Media.Brushes.Firebrick:System.Windows.Media.Brushes.DarkSlateGray; } }
}
