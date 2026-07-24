using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private MaterialsRenderingPrototypeView? _embeddedFastImpactView;
    private List<MaterialsPrototypeColumn>? _defaultFastImpactColumns;

    private void ActivateDefaultFastImpactView()
    {
        if (_embeddedFastImpactView is not null) return;
        _embeddedFastImpactView = CreateFastImpactView();
        FastImpactViewHost.Content = _embeddedFastImpactView;
        FastImpactViewHost.Visibility = Visibility.Visible;
    }

    private void ResetFastImpactColumns_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show(
            this,
            "Reset Fast Impact column widths and order to the application defaults?",
            "Reset Fast Impact Columns",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes) return;

        _workflowPreferencesService.SetFastGridLayout("Impact", Array.Empty<Services.WorkflowColumnLayout>());
        _embeddedFastImpactView?.ResetLayout(
            _defaultFastImpactColumns ?? BuildFastImpactColumns());
        ShowTransientStatus("Fast Impact columns reset to defaults.");
    }

    private MaterialsRenderingPrototypeView CreateFastImpactView(bool useSavedLayout = true)
    {
        var columns = BuildFastImpactColumns();
        _defaultFastImpactColumns ??= columns.Select(column => column with { }).ToList();
        if (useSavedLayout)
        {
            columns = ApplyFastMaterialsLayout(
                columns,
                _workflowPreferencesService.GetFastGridLayout("Impact"));
        }

        return new MaterialsRenderingPrototypeView(
            columns,
            BuildFastImpactRows(columns),
            ApplyFastImpactChanges,
            layout => _workflowPreferencesService.SetFastGridLayout("Impact", layout),
            BuildFastImpactRows,
            _ => { },
            directCanonicalEditing: true,
            reloadAfterApply: true);
    }

    private List<MaterialsPrototypeColumn> BuildFastImpactColumns()
    {
        var columns = BuildFastMeasurementIdentityColumns();
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        for (var index = 1; index <= 10; index++)
        {
            columns.Add(FastMeasurementColumn(
                $"Upright % {index}",
                82,
                $"Upright{index}",
                false,
                FastGridCellKind.ImpactSample));
        }
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        for (var index = 1; index <= 10; index++)
        {
            columns.Add(FastMeasurementColumn(
                $"Flat % {index}",
                75,
                $"Flat{index}",
                false,
                FastGridCellKind.ImpactSample));
        }
        columns.Add(FastMeasurementColumn(string.Empty, 50, null, true, FastGridCellKind.Spacer));
        columns.Add(FastMeasurementColumn("Test Notes", 220, "TestNotes", false));
        columns.Add(FastMeasurementColumn("Measured date", 110, "MeasuredDateText", false));
        columns.Add(FastMeasurementColumn("kJ/m² - Upright", 115, "KjUpright", true, FastGridCellKind.Computed));
        columns.Add(FastMeasurementColumn("kJ/m² - Flat", 105, "KjFlat", true, FastGridCellKind.Computed));
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

    private List<MaterialsPrototypeRow> BuildFastImpactRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns)
    {
        var visibleMaterialIds = GetVisibleNativeMaterialIdsFromCurrentFilters();
        return _nativeImpactRows
            .Where(row => visibleMaterialIds.Contains(row.MaterialID))
            .Select(row =>
            {
                var cells = columns.Select(column => PrototypeCellText(row, column.PropertyName)).ToArray();
                return new MaterialsPrototypeRow(
                    row,
                    row.MaterialID,
                    cells,
                    cells.ToArray(),
                    () => row.ValidationSummary is not ("Invalid needle %" or "Needle % outside 0-100"));
            }).ToList();
    }

    private bool ApplyFastImpactChanges(IReadOnlyList<MaterialsPrototypeChange> changes)
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
                    $"'{change.NewValue}' is not a valid impact needle percentage.",
                    "Fast Impact Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
            if (value is < 0 or > 100)
            {
                MessageBox.Show(
                    this,
                    "Impact needle percentage must be between 0 and 100.",
                    "Fast Impact Input",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
                return false;
            }
        }

        var rowsWithoutMeasurements = sampleChanges
            .Select(change => (NativeImpactMeasurementRow)change.Row.Source)
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

        ApplyNativeImpactComputedFields(_nativeImpactRows);
        RefreshNativeMaterialTestStatusFromNativeInputTabs(markDirty: true);
        MarkNativeImpactDirty();
        SaveNativeImpactSilent();
        RefreshNativeImpactSummary();
        return true;
    }
}
