using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private MaterialsRenderingPrototypeView? _embeddedFastNativeSettingsView;
    private MaterialsRenderingPrototypeView? _embeddedFastBaseMaterialsView;
    private List<MaterialsPrototypeColumn>? _defaultFastNativeSettingsColumns;
    private List<MaterialsPrototypeColumn>? _defaultFastBaseMaterialsColumns;
    private NativeBaseMaterialRow? _selectedFastBaseMaterialRow;
    private bool _fastSettingsEnabled = true;

    private void ActivateDefaultFastSettingsViews()
    {
        if (!_fastSettingsEnabled || _embeddedFastNativeSettingsView is not null) return;

        _embeddedFastNativeSettingsView = CreateFastNativeSettingsView();
        _embeddedFastBaseMaterialsView = CreateFastBaseMaterialsView();
        FastNativeSettingsViewHost.Content = _embeddedFastNativeSettingsView;
        FastBaseMaterialsViewHost.Content = _embeddedFastBaseMaterialsView;
        FastNativeSettingsViewHost.Visibility = Visibility.Visible;
        FastBaseMaterialsViewHost.Visibility = Visibility.Visible;
        NativeSettingsGrid.Visibility = Visibility.Collapsed;
        BaseMaterialsGrid.Visibility = Visibility.Collapsed;
        FastSettingsToggleButton.Content = "Use Legacy Grids";
    }

    private void ToggleFastSettingsViews_Click(object sender, RoutedEventArgs e)
    {
        if (_fastSettingsEnabled)
        {
            _embeddedFastNativeSettingsView?.ConfirmCanClose();
            _embeddedFastBaseMaterialsView?.ConfirmCanClose();
            _embeddedFastNativeSettingsView = null;
            _embeddedFastBaseMaterialsView = null;
            FastNativeSettingsViewHost.Content = null;
            FastBaseMaterialsViewHost.Content = null;
            FastNativeSettingsViewHost.Visibility = Visibility.Collapsed;
            FastBaseMaterialsViewHost.Visibility = Visibility.Collapsed;
            NativeSettingsGrid.Visibility = Visibility.Visible;
            BaseMaterialsGrid.Visibility = Visibility.Visible;
            FastSettingsToggleButton.Content = "Use Fast Grids";
            _fastSettingsEnabled = false;
            return;
        }

        _fastSettingsEnabled = true;
        ActivateDefaultFastSettingsViews();
    }

    private void ResetFastSettingsColumns_Click(object sender, RoutedEventArgs e)
    {
        if (!_fastSettingsEnabled)
        {
            MessageBox.Show(
                this,
                "Switch to Fast Grids before resetting Fast Settings columns.",
                "Fast Settings Columns",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
            return;
        }

        var confirmation = MessageBox.Show(
            this,
            "Reset both Fast Settings column layouts to application defaults?",
            "Reset Fast Settings Columns",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes) return;

        _workflowPreferencesService.SetFastGridLayout("Settings", Array.Empty<Services.WorkflowColumnLayout>());
        _workflowPreferencesService.SetFastGridLayout("BaseMaterials", Array.Empty<Services.WorkflowColumnLayout>());
        _embeddedFastNativeSettingsView?.ResetLayout(
            _defaultFastNativeSettingsColumns ?? BuildFastNativeSettingsColumns());
        _embeddedFastBaseMaterialsView?.ResetLayout(
            _defaultFastBaseMaterialsColumns ?? BuildFastBaseMaterialColumns());
        ShowTransientStatus("Fast Settings columns reset to defaults.");
    }

    private MaterialsRenderingPrototypeView CreateFastNativeSettingsView()
    {
        var columns = BuildFastNativeSettingsColumns();
        _defaultFastNativeSettingsColumns ??= columns.Select(column => column with { }).ToList();
        columns = ApplyFastMaterialsLayout(columns, _workflowPreferencesService.GetFastGridLayout("Settings"));
        return new MaterialsRenderingPrototypeView(
            columns,
            BuildFastNativeSettingsRows(columns),
            ApplyFastNativeSettingsChanges,
            layout => _workflowPreferencesService.SetFastGridLayout("Settings", layout),
            BuildFastNativeSettingsRows,
            _ => { },
            directCanonicalEditing: true,
            reloadAfterApply: true);
    }

    private MaterialsRenderingPrototypeView CreateFastBaseMaterialsView()
    {
        var columns = BuildFastBaseMaterialColumns();
        _defaultFastBaseMaterialsColumns ??= columns.Select(column => column with { }).ToList();
        columns = ApplyFastMaterialsLayout(columns, _workflowPreferencesService.GetFastGridLayout("BaseMaterials"));
        return new MaterialsRenderingPrototypeView(
            columns,
            BuildFastBaseMaterialRows(columns),
            ApplyFastBaseMaterialChanges,
            layout => _workflowPreferencesService.SetFastGridLayout("BaseMaterials", layout),
            BuildFastBaseMaterialRows,
            source => _selectedFastBaseMaterialRow = source as NativeBaseMaterialRow,
            directCanonicalEditing: true,
            reloadAfterApply: true);
    }

    private static List<MaterialsPrototypeColumn> BuildFastNativeSettingsColumns() =>
        AssignStablePrototypeLayoutKeys(
        [
            FastSettingsColumn("Section", 140, "Section", true),
            FastSettingsColumn("Parameter", 220, "Parameter", true),
            FastSettingsColumn("Value", 130, "Value", false),
            FastSettingsColumn("Unit", 90, "Unit", true),
            FastSettingsColumn("Used By", 160, "UsedBy", true),
            FastSettingsColumn("Notes", 500, "Notes", true)
        ]);

    private static List<MaterialsPrototypeColumn> BuildFastBaseMaterialColumns() =>
        AssignStablePrototypeLayoutKeys(
        [
            FastSettingsColumn("Base Material", 200, "BaseMaterial", false),
            FastSettingsColumn("Category", 220, "Category", false),
            FastSettingsColumn("Sort Order", 120, "SortOrder", false),
            FastSettingsColumn("Nozzle min °C", 105, "NozzleTemperatureMinC", false),
            FastSettingsColumn("Nozzle rec. °C", 110, "NozzleTemperatureRecommendedC", false),
            FastSettingsColumn("Nozzle max °C", 105, "NozzleTemperatureMaxC", false),
            FastSettingsColumn("Bed min °C", 95, "BedTemperatureMinC", false),
            FastSettingsColumn("Bed rec. °C", 100, "BedTemperatureRecommendedC", false),
            FastSettingsColumn("Bed max °C", 95, "BedTemperatureMaxC", false),
            FastSettingsColumn("Speed min mm/s", 115, "PrintSpeedMinMmPerS", false),
            FastSettingsColumn("Speed rec. mm/s", 120, "PrintSpeedRecommendedMmPerS", false),
            FastSettingsColumn("Speed max mm/s", 115, "PrintSpeedMaxMmPerS", false),
            FastSettingsColumn("Cooling min %", 105, "CoolingMinPercent", false),
            FastSettingsColumn("Cooling rec. %", 110, "CoolingRecommendedPercent", false),
            FastSettingsColumn("Cooling max %", 105, "CoolingMaxPercent", false),
            FastSettingsColumn("Cooling guidance", 145, "CoolingGuidance", false,
                ["Off", "Low", "Moderate", "High", "Required"]),
            FastSettingsColumn("Drying °C", 95, "DryingTemperatureC", false),
            FastSettingsColumn("Drying hours", 100, "DryingTimeHours", false),
            FastSettingsColumn("Enclosure", 175, "EnclosureRequirement", false,
                ["Not required", "Recommended", "Required", "Heated chamber recommended"]),
            FastSettingsColumn("Printer / G-code reference", 190, "PrinterProfileReference", false),
            FastSettingsColumn("Slicer profile reference", 180, "SlicerProfileReference", false),
            FastSettingsColumn("Profile ID", 145, "ProfileId", false),
            FastSettingsColumn("Profile kind", 155, "ProfileKind", false,
                ["Slicer provided", "Manufacturer provided", "User provided"])
        ]);

    private static MaterialsPrototypeColumn FastSettingsColumn(
        string header,
        double width,
        string propertyName,
        bool isReadOnly,
        IReadOnlyList<string>? choices = null) =>
        new(
            header,
            width,
            propertyName,
            isReadOnly,
            choices is null ? MaterialsPrototypeEditorKind.Text : MaterialsPrototypeEditorKind.ComboBox,
            choices ?? Array.Empty<string>());

    private List<MaterialsPrototypeRow> BuildFastNativeSettingsRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns) =>
        _nativeSettingsRows.Select(row =>
        {
            var cells = columns.Select(column => PrototypeCellText(row, column.PropertyName)).ToArray();
            return new MaterialsPrototypeRow(row, row.Parameter, cells, cells.ToArray(), () => true);
        }).ToList();

    private List<MaterialsPrototypeRow> BuildFastBaseMaterialRows(
        IReadOnlyList<MaterialsPrototypeColumn> columns) =>
        _nativeBaseMaterialRows.Select(row =>
        {
            var cells = columns.Select(column => PrototypeCellText(row, column.PropertyName)).ToArray();
            return new MaterialsPrototypeRow(row, row.BaseMaterial, cells, cells.ToArray(), () => true);
        }).ToList();

    private bool ApplyFastNativeSettingsChanges(IReadOnlyList<MaterialsPrototypeChange> changes)
    {
        foreach (var change in changes)
        {
            SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.NewValue);
        }

        try
        {
            if (changes.Any(change =>
                    change.Row.Source is NativeSettingRow row &&
                    string.Equals(row.Section, "Deployment", StringComparison.OrdinalIgnoreCase)))
            {
                SaveDeploymentSettingsFromManager();
            }
            RefreshNativeInputModulesFromMaterialManager(markDirty: false);
            RefreshPurchaseCurrencyChoices();
            SyncPurchaseOrderCurrencyRatesFromSettings();
            return true;
        }
        catch (Exception ex)
        {
            foreach (var change in changes)
            {
                SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.OldValue);
            }
            MessageBox.Show(this, ex.Message, "Deployment Settings", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }

    private bool ApplyFastBaseMaterialChanges(IReadOnlyList<MaterialsPrototypeChange> changes)
    {
        foreach (var change in changes)
        {
            SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.NewValue);
        }

        try
        {
            SaveBaseMaterialCatalogToDatabase();
            ApplyNativeMaterialComputedFieldsToAllRows();
            RefreshNativeMaterialGridValidation();
            RefreshNativeInputModulesFromMaterialManager(markDirty: false);
            return true;
        }
        catch (Exception ex)
        {
            foreach (var change in changes)
            {
                SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.OldValue);
            }
            MessageBox.Show(this, ex.Message, "Base Material Catalog", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }

    private void RefreshFastSettingsViews()
    {
        if (!_fastSettingsEnabled) return;
        _embeddedFastNativeSettingsView?.ReloadFromCanonical("current canonical Settings");
        _embeddedFastBaseMaterialsView?.ReloadFromCanonical("current canonical Base Material Catalog");
    }
}
