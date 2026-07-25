using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private MaterialsRenderingPrototypeView? _embeddedFastStiffnessView;
    private List<MaterialsPrototypeColumn>? _defaultFastStiffnessColumns;

    private void ActivateDefaultFastStiffnessView()
    {
        if (_embeddedFastStiffnessView is not null) return;
        _embeddedFastStiffnessView = CreateFastStiffnessView();
        FastStiffnessViewHost.Content = _embeddedFastStiffnessView;
        FastStiffnessViewHost.Visibility = Visibility.Visible;
    }

    private void ResetFastStiffnessColumns_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show(
            this,
            "Reset Fast Stiffness column widths and order to the application defaults?",
            "Reset Fast Stiffness Columns",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes) return;

        _workflowPreferencesService.SetFastGridLayout("Stiffness", Array.Empty<Services.WorkflowColumnLayout>());
        _embeddedFastStiffnessView?.ResetLayout(
            _defaultFastStiffnessColumns ?? BuildFastStiffnessColumns());
        ShowTransientStatus("Fast Stiffness columns reset to defaults.");
    }

    private MaterialsRenderingPrototypeView CreateFastStiffnessView(bool useSavedLayout = true)
    {
        var columns = BuildFastStiffnessColumns();
        _defaultFastStiffnessColumns ??= columns.Select(column => column with { }).ToList();
        if (useSavedLayout)
        {
            columns = ApplyFastMaterialsLayout(
                columns,
                _workflowPreferencesService.GetFastGridLayout("Stiffness"));
        }

        return new MaterialsRenderingPrototypeView(
            columns,
            BuildFastStiffnessRows(columns),
            ApplyFastStiffnessChanges,
            layout => _workflowPreferencesService.SetFastGridLayout("Stiffness", layout),
            BuildFastStiffnessRows,
            _ => { },
            directCanonicalEditing: true,
            reloadAfterApply: true);
    }

    private List<MaterialsPrototypeColumn> BuildFastStiffnessColumns()
    {
        var columns = BuildFastMeasurementIdentityColumns();
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        columns.Add(FastMeasurementColumn("Revolutions", 95, "Revolutions", false));
        columns.Add(FastMeasurementColumn("Degrees", 85, "Degrees", false));
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        columns.Add(FastMeasurementColumn("Test Notes", 220, "TestNotes", false));
        columns.Add(FastMeasurementColumn("Measured date", 110, "MeasuredDateText", false));
        columns.Add(FastMeasurementColumn("Deflection mm", 110, "DeflectionMm", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Modulus MPa", 110, "ModulusMpa", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("Validation", 150, "ValidationSummary", true));
        return AssignStablePrototypeLayoutKeys(columns);
    }

    private List<MaterialsPrototypeRow> BuildFastStiffnessRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns)
    {
        var visibleMaterialIds = GetVisibleNativeMaterialIdsFromCurrentFilters();
        return _nativeStiffnessRows
            .Where(row => visibleMaterialIds.Contains(row.MaterialID))
            .OrderBy(row => row.MaterialID, CanonicalMaterialIdComparer)
            .Select(row =>
            {
                var cells = columns.Select(column => PrototypeCellText(row, column.PropertyName)).ToArray();
                return new MaterialsPrototypeRow(
                    row,
                    row.MaterialID,
                    cells,
                    cells.ToArray(),
                    () => row.ValidationSummary == "OK");
            }).ToList();
    }

    private bool ApplyFastStiffnessChanges(IReadOnlyList<MaterialsPrototypeChange> changes)
    {
        var inputChanges = changes
            .Where(change => change.Column.PropertyName is "Revolutions" or "Degrees")
            .ToList();

        foreach (var change in inputChanges)
        {
            if (string.IsNullOrWhiteSpace(change.NewValue)) continue;
            if (!TryParseMeasurement(change.NewValue, out var value))
            {
                MessageBox.Show(
                    this,
                    $"'{change.NewValue}' is not a valid stiffness number.",
                    "Fast Stiffness Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }

            var limit = change.Column.PropertyName == "Revolutions" ? 10d : 359d;
            if (value < 0 || value > limit)
            {
                MessageBox.Show(
                    this,
                    $"{change.Column.Header} must be between 0 and {limit:0}.",
                    "Fast Stiffness Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
        }

        var rowsWithoutMeasurements = inputChanges
            .Select(change => (NativeStiffnessMeasurementRow)change.Row.Source)
            .Distinct()
            .Where(row => row.MeasuredDate is null &&
                          string.IsNullOrWhiteSpace(row.Revolutions) &&
                          string.IsNullOrWhiteSpace(row.Degrees))
            .ToHashSet();

        foreach (var change in changes)
        {
            SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.NewValue);
        }
        foreach (var row in rowsWithoutMeasurements)
        {
            row.MeasuredDate = DateTime.Today;
        }

        ApplyNativeStiffnessComputedFields(_nativeStiffnessRows);
        RefreshNativeMaterialTestStatusFromNativeInputTabs(markDirty: true);
        MarkNativeStiffnessDirty();
        SaveNativeStiffnessSilent();
        RefreshNativeStiffnessSummary();
        return true;
    }
}
