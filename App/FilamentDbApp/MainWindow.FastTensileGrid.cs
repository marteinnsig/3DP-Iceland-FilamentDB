using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private MaterialsRenderingPrototypeView? _embeddedFastTensileView;
    private List<MaterialsPrototypeColumn>? _defaultFastTensileColumns;
    private bool _fastTensileEnabled = true;

    private void ActivateDefaultFastTensileView()
    {
        if (!_fastTensileEnabled || _embeddedFastTensileView is not null) return;
        _embeddedFastTensileView = CreateFastTensileView();
        FastTensileViewHost.Content = _embeddedFastTensileView;
        FastTensileViewHost.Visibility = Visibility.Visible;
        NativeTensileGrid.Visibility = Visibility.Collapsed;
        FastTensileToggleButton.Content = "Use Legacy Grid";
    }

    private void ToggleFastTensileView_Click(object sender, RoutedEventArgs e)
    {
        if (_fastTensileEnabled)
        {
            _embeddedFastTensileView?.ConfirmCanClose();
            _embeddedFastTensileView = null;
            FastTensileViewHost.Content = null;
            FastTensileViewHost.Visibility = Visibility.Collapsed;
            NativeTensileGrid.Visibility = Visibility.Visible;
            FastTensileToggleButton.Content = "Use Fast Grid";
            _fastTensileEnabled = false;
            return;
        }

        _fastTensileEnabled = true;
        ActivateDefaultFastTensileView();
    }

    private void ResetFastTensileColumns_Click(object sender, RoutedEventArgs e)
    {
        if (!_fastTensileEnabled)
        {
            ResetWorkflowGridColumns_Click(
                new Button { Tag = nameof(NativeTensileGrid) },
                e);
            return;
        }

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

    private List<MaterialsPrototypeColumn> BuildFastTensileColumns() =>
        AssignStablePrototypeLayoutKeys(NativeTensileGrid.Columns
            .OrderBy(column => column.DisplayIndex)
            .Select(column =>
            {
                var propertyName = GetBoundPropertyName(column);
                var header = column.Header?.ToString() ?? string.Empty;
                var cellKind = string.IsNullOrWhiteSpace(propertyName)
                    ? FastGridCellKind.Spacer
                    : System.Text.RegularExpressions.Regex.IsMatch(
                        propertyName,
                        "^(Upright|Flat)(10|[1-9])$",
                        System.Text.RegularExpressions.RegexOptions.CultureInvariant)
                        ? FastGridCellKind.TensileSample
                        : column.IsReadOnly &&
                          propertyName is not ("MaterialID" or "Manufacturer" or "ProductLine" or "MarketingName" or
                              "BaseMaterial" or "MaterialCategory" or "VariantFinish" or "Reinforcement" or "Color" or
                              "ValidationSummary")
                            ? FastGridCellKind.Computed
                            : FastGridCellKind.Standard;
                return new MaterialsPrototypeColumn(
                    header,
                    Math.Clamp(column.Width.DisplayValue, 50, 500),
                    propertyName,
                    column.IsReadOnly,
                    MaterialsPrototypeEditorKind.Text,
                    Array.Empty<string>(),
                    cellKind);
            })
            .ToList());

    private List<MaterialsPrototypeRow> BuildFastTensileRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns) =>
        NativeTensileGrid.Items
            .Cast<object>()
            .OfType<NativeTensileMeasurementRow>()
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
