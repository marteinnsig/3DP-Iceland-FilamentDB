using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private MaterialsRenderingPrototypeView? _embeddedFastStiffnessView;
    private List<MaterialsPrototypeColumn>? _defaultFastStiffnessColumns;
    private bool _fastStiffnessEnabled = true;

    private void ActivateDefaultFastStiffnessView()
    {
        if (!_fastStiffnessEnabled || _embeddedFastStiffnessView is not null) return;
        _embeddedFastStiffnessView = CreateFastStiffnessView();
        FastStiffnessViewHost.Content = _embeddedFastStiffnessView;
        FastStiffnessViewHost.Visibility = Visibility.Visible;
        NativeStiffnessGrid.Visibility = Visibility.Collapsed;
        FastStiffnessToggleButton.Content = "Use Legacy Grid";
    }

    private void ToggleFastStiffnessView_Click(object sender, RoutedEventArgs e)
    {
        if (_fastStiffnessEnabled)
        {
            _embeddedFastStiffnessView?.ConfirmCanClose();
            _embeddedFastStiffnessView = null;
            FastStiffnessViewHost.Content = null;
            FastStiffnessViewHost.Visibility = Visibility.Collapsed;
            NativeStiffnessGrid.Visibility = Visibility.Visible;
            FastStiffnessToggleButton.Content = "Use Fast Grid";
            _fastStiffnessEnabled = false;
            return;
        }

        _fastStiffnessEnabled = true;
        ActivateDefaultFastStiffnessView();
    }

    private void ResetFastStiffnessColumns_Click(object sender, RoutedEventArgs e)
    {
        if (!_fastStiffnessEnabled)
        {
            ResetWorkflowGridColumns_Click(
                new Button { Tag = nameof(NativeStiffnessGrid) },
                e);
            return;
        }

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

    private List<MaterialsPrototypeColumn> BuildFastStiffnessColumns() =>
        AssignStablePrototypeLayoutKeys(NativeStiffnessGrid.Columns
            .OrderBy(column => column.DisplayIndex)
            .Select(column =>
            {
                var propertyName = GetBoundPropertyName(column);
                var cellKind = string.IsNullOrWhiteSpace(propertyName)
                    ? FastGridCellKind.Spacer
                    : propertyName is "DeflectionMm" or "ModulusMpa"
                        ? FastGridCellKind.Computed
                        : FastGridCellKind.Standard;
                return new MaterialsPrototypeColumn(
                    column.Header?.ToString() ?? string.Empty,
                    Math.Clamp(column.Width.DisplayValue, 50, 500),
                    propertyName,
                    column.IsReadOnly,
                    MaterialsPrototypeEditorKind.Text,
                    Array.Empty<string>(),
                    cellKind);
            })
            .ToList());

    private List<MaterialsPrototypeRow> BuildFastStiffnessRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns) =>
        NativeStiffnessGrid.Items
            .Cast<object>()
            .OfType<NativeStiffnessMeasurementRow>()
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
