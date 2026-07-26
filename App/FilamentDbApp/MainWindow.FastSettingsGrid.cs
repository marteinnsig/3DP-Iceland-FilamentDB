using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private sealed record ExactBaseMaterialBinding(
        NativeMaterialRow Material,
        NativeBaseMaterialRow BaseMaterial);
    private MaterialsRenderingPrototypeView? _embeddedFastNativeSettingsView;
    private MaterialsRenderingPrototypeView? _embeddedFastBaseMaterialsView;
    private List<MaterialsPrototypeColumn>? _defaultFastNativeSettingsColumns;
    private List<MaterialsPrototypeColumn>? _defaultFastBaseMaterialsColumns;
    private NativeBaseMaterialRow? _selectedFastBaseMaterialRow;

    private void ActivateDefaultFastSettingsViews()
    {
        if (_embeddedFastNativeSettingsView is not null) return;

        _embeddedFastNativeSettingsView = CreateFastNativeSettingsView();
        _embeddedFastBaseMaterialsView = CreateFastBaseMaterialsView();
        FastNativeSettingsViewHost.Content = _embeddedFastNativeSettingsView;
        FastBaseMaterialsViewHost.Content = _embeddedFastBaseMaterialsView;
        FastNativeSettingsViewHost.Visibility = Visibility.Visible;
        FastBaseMaterialsViewHost.Visibility = Visibility.Visible;
    }

    private void ResetFastSettingsColumns_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show(
            this,
            "Reset the Settings Manager column layout to the application default?\n\nSaved Settings are unchanged.",
            "Reset Settings Columns",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes) return;

        _workflowPreferencesService.SetFastGridLayout("Settings", Array.Empty<Services.WorkflowColumnLayout>());
        _embeddedFastNativeSettingsView?.ResetLayout(
            _defaultFastNativeSettingsColumns ?? BuildFastNativeSettingsColumns());
        ShowTransientStatus("Settings Manager columns reset to defaults; saved data was unchanged.");
    }

    private void ResetFastBaseMaterialColumns_Click(object sender, RoutedEventArgs e)
    {
        var confirmation = MessageBox.Show(
            this,
            "Reset the Base Materials column layout to the application default?\n\nSaved catalog data is unchanged.",
            "Reset Base Material Columns",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (confirmation != MessageBoxResult.Yes) return;

        _workflowPreferencesService.SetFastGridLayout("BaseMaterials", Array.Empty<Services.WorkflowColumnLayout>());
        _embeddedFastBaseMaterialsView?.ResetLayout(
            _defaultFastBaseMaterialsColumns ?? BuildFastBaseMaterialColumns());
        ShowTransientStatus("Base Materials columns reset to defaults; saved catalog data was unchanged.");
    }

    private List<ExactBaseMaterialBinding> BuildExactBaseMaterialBindingPlan()
    {
        var uniqueNames = _nativeBaseMaterialRows
            .Where(row => !string.IsNullOrWhiteSpace(row.BaseMaterial))
            .GroupBy(row => row.BaseMaterial.Trim(), StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() == 1)
            .ToDictionary(group => group.Key, group => group.Single(), StringComparer.OrdinalIgnoreCase);
        return _nativeMaterialRows
            .Where(material =>
                !material.BaseMaterialId.HasValue &&
                !string.IsNullOrWhiteSpace(material.BaseMaterial) &&
                uniqueNames.ContainsKey(material.BaseMaterial.Trim()))
            .Select(material => new ExactBaseMaterialBinding(
                material,
                uniqueNames[material.BaseMaterial.Trim()]))
            .ToList();
    }

    private void BindExactBaseMaterialNames_Click(object sender, RoutedEventArgs e)
    {
        if (IsAutomationActionBlocked("Base Material exact-name binding")) return;
        var plan = BuildExactBaseMaterialBindingPlan();
        if (plan.Count == 0)
        {
            ShowTransientStatus("No unlinked Materials have an exact, unique Base Material catalog match.");
            return;
        }
        var unmatched = _nativeMaterialRows.Count(material =>
            !material.BaseMaterialId.HasValue &&
            !string.IsNullOrWhiteSpace(material.BaseMaterial)) - plan.Count;
        var preview = string.Join(
            "\n",
            plan.Take(12).Select(item =>
                $"{item.Material.MaterialID}: {item.Material.BaseMaterial} → ID {item.BaseMaterial.BaseMaterialId}"));
        var result = MessageBox.Show(
            this,
            $"Bind {plan.Count} unlinked Material(s) to exact, unique Base Material names?\n\n" +
            preview +
            (plan.Count > 12 ? $"\n… and {plan.Count - 12} more" : string.Empty) +
            $"\n\n{unmatched} other unlinked value(s) will remain unchanged. " +
            "Matching is exact ignoring case; no fuzzy remapping is performed.",
            "Bind Exact Base Material Names?",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (result != MessageBoxResult.Yes) return;

        CreateDatabaseBackupBeforeMajorMaterialChange("binding exact Base Material identities");
        var originals = plan.Select(item =>
            (item.Material, item.Material.BaseMaterialId, item.Material.BaseMaterial)).ToList();
        foreach (var item in plan)
        {
            item.Material.BaseMaterialId = item.BaseMaterial.BaseMaterialId;
            item.Material.BaseMaterial = item.BaseMaterial.BaseMaterial;
        }
        ApplyNativeMaterialComputedFieldsToAllRows();
        if (!SaveNativeMaterialsSilent())
        {
            foreach (var original in originals)
            {
                original.Material.BaseMaterialId = original.BaseMaterialId;
                original.Material.BaseMaterial = original.BaseMaterial;
            }
            ShowTransientStatus("Exact Base Material binding failed; the pre-change backup was retained.");
            return;
        }
        PopulateNativeMaterialFilters();
        RefreshFastBaseMaterialChoices();
        _embeddedMaterialsPrototypeView?.SynchronizeFromCanonical(
            "explicit exact Base Material identity binding");
        RefreshNativeInputModulesFromMaterialManager(markDirty: false);
        RefreshCanonicalMaterialConsumerFilters();
        ShowTransientStatus(
            $"Bound {plan.Count} Material(s) by exact Base Material name; {unmatched} remain unlinked.");
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
            reloadAfterApply: true,
            showReloadButton: false);
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
            reloadAfterApply: true,
            showReloadButton: false);
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
        var renamedMaterials = new List<(NativeMaterialRow Material, string OldName)>();
        foreach (var change in changes)
        {
            SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.NewValue);
            if (change.Row.Source is NativeBaseMaterialRow catalogRow &&
                string.Equals(
                    change.Column.PropertyName,
                    nameof(NativeBaseMaterialRow.BaseMaterial),
                    StringComparison.Ordinal))
            {
                if (string.IsNullOrWhiteSpace(change.NewValue) ||
                    _nativeBaseMaterialRows.Count(row =>
                        string.Equals(
                            row.BaseMaterial.Trim(),
                            change.NewValue.Trim(),
                            StringComparison.OrdinalIgnoreCase)) > 1)
                {
                    SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.OldValue);
                    MessageBox.Show(
                        this,
                        "Base Material names must be non-empty and unique.",
                        "Base Material Catalog",
                        MessageBoxButton.OK,
                        MessageBoxImage.Warning);
                    return false;
                }

                foreach (var material in _nativeMaterialRows.Where(material =>
                             material.BaseMaterialId == catalogRow.BaseMaterialId))
                {
                    renamedMaterials.Add((material, material.BaseMaterial));
                    material.BaseMaterial = change.NewValue.Trim();
                }
            }
        }

        try
        {
            SaveBaseMaterialCatalogToDatabase();
            ApplyNativeMaterialComputedFieldsToAllRows();
            RefreshNativeMaterialGridValidation();
            if (renamedMaterials.Count > 0 && !SaveNativeMaterialsSilent())
                throw new InvalidOperationException("Linked Material Base Material names could not be saved.");
            RefreshFastBaseMaterialChoices();
            _embeddedMaterialsPrototypeView?.SynchronizeFromCanonical(
                "current canonical Base Material Catalog choices");
            RefreshNativeInputModulesFromMaterialManager(markDirty: false);
            RefreshCanonicalMaterialConsumerFilters();
            return true;
        }
        catch (Exception ex)
        {
            foreach (var change in changes)
            {
                SetPropertyValue(change.Row.Source, change.Column.PropertyName!, change.OldValue);
            }
            foreach (var renamed in renamedMaterials)
                renamed.Material.BaseMaterial = renamed.OldName;
            RefreshFastBaseMaterialChoices();
            try
            {
                SaveBaseMaterialCatalogToDatabase();
                if (renamedMaterials.Count > 0) SaveNativeMaterialsSilent();
            }
            catch
            {
                // The original failure remains the actionable error. The automatic
                // database backup and normal recovery surfaces retain rollback evidence.
            }
            MessageBox.Show(this, ex.Message, "Base Material Catalog", MessageBoxButton.OK, MessageBoxImage.Warning);
            return false;
        }
    }

    private void RefreshFastSettingsViews()
    {
        _embeddedFastNativeSettingsView?.ReloadFromCanonical("current canonical Settings");
        _embeddedFastBaseMaterialsView?.ReloadFromCanonical("current canonical Base Material Catalog");
        RefreshFastBaseMaterialChoices();
    }
}
