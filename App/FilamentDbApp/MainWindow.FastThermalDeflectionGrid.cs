using FilamentDbApp.Data;
using FilamentDbApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private readonly ObservableCollection<NativeThermalDeflectionRow> _nativeThermalDeflectionRows = new();
    private MaterialsRenderingPrototypeView? _embeddedFastThermalDeflectionView;
    private List<MaterialsPrototypeColumn>? _defaultFastThermalDeflectionColumns;

    private void InitializeNativeThermalDeflectionMeasurements()
    {
        ReloadNativeThermalDeflectionRows();
        RefreshNativeThermalDeflectionSummary();
    }

    private void ActivateDefaultFastThermalDeflectionView()
    {
        if (_embeddedFastThermalDeflectionView is not null) return;
        _embeddedFastThermalDeflectionView = CreateFastThermalDeflectionView();
        FastThermalDeflectionViewHost.Content = _embeddedFastThermalDeflectionView;
        FastThermalDeflectionViewHost.Visibility = Visibility.Visible;
    }

    private void ResetFastThermalDeflectionColumns_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show(this,
            "Reset Heat Deflection column widths and order to the application defaults?",
            "Reset Heat Deflection Columns", MessageBoxButton.YesNo,
            MessageBoxImage.Question, MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes) return;
        _workflowPreferencesService.SetFastGridLayout("ThermalDeflection", []);
        _embeddedFastThermalDeflectionView?.ResetLayout(
            _defaultFastThermalDeflectionColumns ?? BuildFastThermalDeflectionColumns());
        ShowTransientStatus("Heat Deflection columns reset to defaults.");
    }

    private MaterialsRenderingPrototypeView CreateFastThermalDeflectionView(bool useSavedLayout = true)
    {
        var columns = BuildFastThermalDeflectionColumns();
        _defaultFastThermalDeflectionColumns ??= columns.Select(column => column with { }).ToList();
        if (useSavedLayout)
            columns = ApplyFastMaterialsLayout(columns,
                _workflowPreferencesService.GetFastGridLayout("ThermalDeflection"));
        return new(columns, BuildFastThermalDeflectionRows(columns),
            ApplyFastThermalDeflectionChanges,
            layout => _workflowPreferencesService.SetFastGridLayout("ThermalDeflection", layout),
            BuildFastThermalDeflectionRows, _ => { }, directCanonicalEditing: true,
            reloadAfterApply: true);
    }

    private List<MaterialsPrototypeColumn> BuildFastThermalDeflectionColumns()
    {
        var columns = BuildFastMeasurementIdentityColumns();
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        columns.Add(FastMeasurementColumn("Deflection temperature °C", 175, "ResultTemperatureC", false));
        columns.Add(FastMeasurementColumn("Measured date", 110, "MeasuredDateText", false));
        columns.Add(FastMeasurementColumn("Test Notes", 240, "TestNotes", false));
        columns.Add(FastMeasurementColumn("Method version", 210, "MethodVersion", true));
        columns.Add(FastMeasurementColumn("Validation", 150, "ValidationSummary", true));
        return AssignStablePrototypeLayoutKeys(columns);
    }

    private List<MaterialsPrototypeRow> BuildFastThermalDeflectionRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns)
    {
        var visibleMaterialIds = GetVisibleNativeMaterialIdsFromCurrentFilters();
        return _nativeThermalDeflectionRows
            .Where(row => visibleMaterialIds.Contains(row.MaterialID))
            .OrderBy(row => row.MaterialID, CanonicalMaterialIdComparer)
            .Select(row =>
            {
                var cells = columns.Select(column => PrototypeCellText(row, column.PropertyName)).ToArray();
                return new MaterialsPrototypeRow(row, row.MaterialID, cells, cells.ToArray(),
                    () => row.ValidationSummary == "OK");
            }).ToList();
    }

    private bool ApplyFastThermalDeflectionChanges(IReadOnlyList<MaterialsPrototypeChange> changes)
    {
        foreach (var change in changes.Where(change => change.Column.PropertyName == "ResultTemperatureC"))
        {
            if (string.IsNullOrWhiteSpace(change.NewValue)) continue;
            if (!TryParseMeasurement(change.NewValue, out var value) || value < 25 || value > 300)
            {
                MessageBox.Show(this, "Deflection temperature must be blank or between 25 and 300 °C.",
                    "Heat Deflection Input", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }
        }

        foreach (var change in changes)
            SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.NewValue);

        foreach (var row in changes.Select(change => (NativeThermalDeflectionRow)change.Row.Source).Distinct())
        {
            if (!string.IsNullOrWhiteSpace(row.MeasuredDateText) && row.MeasuredDate is null)
            {
                MessageBox.Show(this, "Measured date is incomplete or invalid.", "Heat Deflection Input",
                    MessageBoxButton.OK, MessageBoxImage.Warning);
                ReloadNativeThermalDeflectionRows();
                return false;
            }
            var hasResult = TryParseMeasurement(row.ResultTemperatureC, out var value);
            if (hasResult && row.MeasuredDate is null) row.MeasuredDate = DateTime.Today;
            _database.SaveManualThermalDeflectionMeasurement(row.MaterialID,
                hasResult ? value : null,
                row.MeasuredDate?.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), row.TestNotes);
            if (!hasResult) { row.MeasuredDate = null; row.TestNotes = ""; }
            row.MethodVersion = hasResult ? ThermalDeflectionMethodContract.Version : "";
        }
        RefreshNativeThermalDeflectionSummary();
        return true;
    }

    private void ReloadNativeThermalDeflectionRows()
    {
        var saved = _database.GetThermalDeflectionMeasurements()
            .ToDictionary(row => row.MaterialId, StringComparer.OrdinalIgnoreCase);
        _nativeThermalDeflectionRows.Clear();
        foreach (var material in _nativeMaterialRows.Where(row => !string.IsNullOrWhiteSpace(row.MaterialID)))
        {
            saved.TryGetValue(material.MaterialID, out var measurement);
            _nativeThermalDeflectionRows.Add(NativeThermalDeflectionRow.From(material, measurement));
        }
        _embeddedFastThermalDeflectionView?.ReloadFromCanonical("canonical Heat Deflection storage");
        RefreshNativeThermalDeflectionSummary();
    }

    private void RefreshNativeThermalDeflectionSummary()
    {
        if (FindName("NativeThermalDeflectionSummaryText") is not TextBlock text) return;
        var measured = _nativeThermalDeflectionRows.Count(row => row.HasMeasurementData());
        var invalid = _nativeThermalDeflectionRows.Count(row => row.ValidationSummary != "OK");
        text.Text = $"Rows: {_nativeThermalDeflectionRows.Count} | With result: {measured} | Invalid: {invalid} | Storage: {CanonicalStorageStatusText} | Auto-save ready";
    }

    private sealed class NativeThermalDeflectionRow : INotifyPropertyChanged
    {
        public string MaterialID { get; set; } = "";
        public string Manufacturer { get; set; } = "";
        public string ProductLine { get; set; } = "";
        public string MarketingName { get; set; } = "";
        public string BaseMaterial { get; set; } = "";
        public string MaterialCategory { get; set; } = "";
        public string VariantFinish { get; set; } = "";
        public string Reinforcement { get; set; } = "";
        public string Color { get; set; } = "";
        public string ResultTemperatureC { get; set; } = "";
        public string TestNotes { get; set; } = "";
        public string MethodVersion { get; set; } = "";
        private DateTime? _measuredDate;
        private string _measuredDateText = "";
        public DateTime? MeasuredDate { get => _measuredDate; set { _measuredDate = value?.Date; _measuredDateText = FormatDisplayedMeasuredDate(_measuredDate); OnPropertyChanged(nameof(MeasuredDate)); OnPropertyChanged(nameof(MeasuredDateText)); } }
        [JsonIgnore]
        public string MeasuredDateText { get => _measuredDateText; set { _measuredDateText = value ?? ""; if (string.IsNullOrWhiteSpace(_measuredDateText)) _measuredDate = null; else if (TryParseDisplayedMeasuredDate(_measuredDateText, out var parsed)) { _measuredDate = parsed.Date; _measuredDateText = FormatDisplayedMeasuredDate(_measuredDate); } OnPropertyChanged(nameof(MeasuredDateText)); } }
        [JsonIgnore]
        public string ValidationSummary => string.IsNullOrWhiteSpace(ResultTemperatureC) ||
            TryParseMeasurement(ResultTemperatureC, out var value) && value is >= 25 and <= 300 ? "OK" : "Invalid temperature";
        public bool HasMeasurementData() => TryParseMeasurement(ResultTemperatureC, out _);
        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new(propertyName));

        public static NativeThermalDeflectionRow From(NativeMaterialRow material,
            LocalDatabase.ThermalDeflectionMeasurementRecord? measurement) => new()
        {
            MaterialID = material.MaterialID, Manufacturer = material.Manufacturer,
            ProductLine = material.ProductLine, MarketingName = material.MarketingName,
            BaseMaterial = material.BaseMaterial, MaterialCategory = material.MaterialCategory,
            VariantFinish = material.VariantFinish, Reinforcement = material.Reinforcement,
            Color = material.Color,
            ResultTemperatureC = measurement is null ? "" : measurement.ResultTemperatureC.ToString("0.####", CultureInfo.InvariantCulture),
            MeasuredDate = ParseIsoMeasuredDate(measurement?.MeasuredDate ?? ""),
            TestNotes = measurement?.TestNotes ?? "", MethodVersion = measurement?.MethodVersion ?? ""
        };
    }
}
