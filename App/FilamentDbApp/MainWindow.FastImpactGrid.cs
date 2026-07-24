using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private MaterialsRenderingPrototypeView? _embeddedFastImpactView;
    private List<MaterialsPrototypeColumn>? _defaultFastImpactColumns;
    private bool _fastImpactEnabled = true;

    private void ActivateDefaultFastImpactView()
    {
        if (!_fastImpactEnabled || _embeddedFastImpactView is not null) return;
        _embeddedFastImpactView = CreateFastImpactView();
        FastImpactViewHost.Content = _embeddedFastImpactView;
        FastImpactViewHost.Visibility = Visibility.Visible;
        NativeImpactGrid.Visibility = Visibility.Collapsed;
        FastImpactToggleButton.Content = "Use Legacy Grid";
    }

    private void ToggleFastImpactView_Click(object sender, RoutedEventArgs e)
    {
        if (_fastImpactEnabled)
        {
            _embeddedFastImpactView?.ConfirmCanClose();
            _embeddedFastImpactView = null;
            FastImpactViewHost.Content = null;
            FastImpactViewHost.Visibility = Visibility.Collapsed;
            NativeImpactGrid.Visibility = Visibility.Visible;
            FastImpactToggleButton.Content = "Use Fast Grid";
            _fastImpactEnabled = false;
            return;
        }

        _fastImpactEnabled = true;
        ActivateDefaultFastImpactView();
    }

    private void ResetFastImpactColumns_Click(object sender, RoutedEventArgs e)
    {
        if (!_fastImpactEnabled)
        {
            ResetWorkflowGridColumns_Click(
                new Button { Tag = nameof(NativeImpactGrid) },
                e);
            return;
        }

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

    private List<MaterialsPrototypeColumn> BuildFastImpactColumns() =>
        AssignStablePrototypeLayoutKeys(NativeImpactGrid.Columns
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
                        ? FastGridCellKind.ImpactSample
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

    private List<MaterialsPrototypeRow> BuildFastImpactRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns) =>
        NativeImpactGrid.Items
            .Cast<object>()
            .OfType<NativeImpactMeasurementRow>()
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
