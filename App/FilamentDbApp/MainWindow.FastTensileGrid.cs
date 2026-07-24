using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private MaterialsRenderingPrototypeView? _embeddedFastTensileView;
    private List<MaterialsPrototypeColumn>? _defaultFastTensileColumns;

    private void ActivateDefaultFastTensileView()
    {
        if (_embeddedFastTensileView is not null) return;
        _embeddedFastTensileView = CreateFastTensileView();
        FastTensileViewHost.Content = _embeddedFastTensileView;
        FastTensileViewHost.Visibility = Visibility.Visible;
    }

    private void ResetFastTensileColumns_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show(
            this,
            "Reset Fast Tensile column widths and order to the application defaults?",
            "Reset Fast Tensile Columns",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes) return;

        _workflowPreferencesService.SetFastGridLayout("Tensile", Array.Empty<Services.WorkflowColumnLayout>());
        _embeddedFastTensileView?.ResetLayout(
            _defaultFastTensileColumns ?? BuildFastTensileColumns());
        ShowTransientStatus("Fast Tensile columns reset to defaults.");
    }

    private MaterialsRenderingPrototypeView CreateFastTensileView(bool useSavedLayout = true)
    {
        var columns = BuildFastTensileColumns();
        _defaultFastTensileColumns ??= columns.Select(column => column with { }).ToList();
        if (useSavedLayout)
        {
            columns = ApplyFastMaterialsLayout(
                columns,
                _workflowPreferencesService.GetFastGridLayout("Tensile"));
        }

        return new MaterialsRenderingPrototypeView(
            columns,
            BuildFastTensileRows(columns),
            ApplyFastTensileChanges,
            layout => _workflowPreferencesService.SetFastGridLayout("Tensile", layout),
            BuildFastTensileRows,
            _ => { },
            directCanonicalEditing: true,
            reloadAfterApply: true);
    }

    private List<MaterialsPrototypeColumn> BuildFastTensileColumns()
    {
        var columns = BuildFastMeasurementIdentityColumns();
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        for (var index = 1; index <= 10; index++)
        {
            columns.Add(FastMeasurementColumn(
                $"Upright {index}",
                index == 10 ? 80 : 75,
                $"Upright{index}",
                false,
                FastGridCellKind.TensileSample));
        }
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        for (var index = 1; index <= 10; index++)
        {
            columns.Add(FastMeasurementColumn(
                $"Flat {index}",
                index == 10 ? 75 : 70,
                $"Flat{index}",
                false,
                FastGridCellKind.TensileSample));
        }
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        columns.Add(FastMeasurementColumn("Test Notes", 220, "TestNotes", false));
        columns.Add(FastMeasurementColumn("Measured date", 110, "MeasuredDateText", false));
        columns.Add(FastMeasurementColumn("MPa - Upright", 100, "MpaUpright", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("MPa - Flat", 90, "MpaFlat", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Std Dev - Upright", 120, "StdDevUpright", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Std Dev - Flat", 110, "StdDevFlat", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("CV % - Upright", 110, "CvUpright", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("CV % - Flat", 100, "CvFlat", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Samples - Upright", 115, "SamplesUpright", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Samples - Flat", 105, "SamplesFlat", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Confidence - Upright", 130, "ConfidenceUpright", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Confidence - Flat", 120, "ConfidenceFlat", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Validation", 150, "ValidationSummary", true));
        return AssignStablePrototypeLayoutKeys(columns);
    }

    private List<MaterialsPrototypeRow> BuildFastTensileRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns)
    {
        var visibleMaterialIds = GetVisibleNativeMaterialIdsFromCurrentFilters();
        return _nativeTensileRows
            .Where(row => visibleMaterialIds.Contains(row.MaterialID))
            .Select(row =>
        {
            var cells = columns.Select(column => PrototypeCellText(row, column.PropertyName)).ToArray();
            return new MaterialsPrototypeRow(
                row,
                row.MaterialID,
                cells,
                cells.ToArray(),
                () => !string.Equals(row.ValidationSummary, "Invalid sample value", StringComparison.Ordinal));
        }).ToList();
    }

    private bool ApplyFastTensileChanges(IReadOnlyList<MaterialsPrototypeChange> changes)
    {
        var sampleChanges = changes.Where(change =>
            change.Column.PropertyName is not null &&
            System.Text.RegularExpressions.Regex.IsMatch(
                change.Column.PropertyName,
                "^(Upright|Flat)(10|[1-9])$",
                System.Text.RegularExpressions.RegexOptions.CultureInvariant)).ToList();

        foreach (var change in sampleChanges)
        {
            if (string.IsNullOrWhiteSpace(change.NewValue)) continue;
            if (!TryParseMeasurement(change.NewValue, out var value))
            {
                MessageBox.Show(
                    this,
                    $"'{change.NewValue}' is not a valid tensile sample number.",
                    "Fast Tensile Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            if (value is < 0 or >= 505)
            {
                MessageBox.Show(
                    this,
                    "Tensile sample value must be between 0 and less than 505.",
                    "Fast Tensile Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
        }

        var rowsWithoutMeasurements = sampleChanges
            .Select(change => (NativeTensileMeasurementRow)change.Row.Source)
            .Distinct()
            .Where(row => row.MeasuredDate is null &&
                          !row.SampleValues(true).Concat(row.SampleValues(false)).Any(value => !string.IsNullOrWhiteSpace(value)))
            .ToHashSet();

        foreach (var change in changes)
        {
            SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.NewValue);
        }
        foreach (var row in rowsWithoutMeasurements)
        {
            row.MeasuredDate = DateTime.Today;
        }

        ApplyNativeTensileComputedFields(_nativeTensileRows);
        RefreshNativeMaterialTestStatusFromNativeInputTabs(markDirty: true);
        MarkNativeTensileDirty();
        SaveNativeTensileSilent();
        RefreshNativeTensileSummary();
        return true;
    }
}
