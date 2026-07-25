using FilamentDbApp.Models;
using FilamentDbApp.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;

namespace FilamentDbApp;

public partial class MainWindow
{
    private readonly ObservableCollection<ManufacturerRecord> _manufacturerRows = new();
    private readonly Dictionary<long, string> _persistedManufacturerNames = new();
    private readonly HashSet<long> _manufacturerRenameDraftIds = new();
    private ICollectionView? _manufacturerView;
    private bool _loadingManufacturers;

    private void InitializeManufacturerManager()
    {
        _loadingManufacturers = true;
        _manufacturerRows.Clear();
        _persistedManufacturerNames.Clear();
        _manufacturerRenameDraftIds.Clear();
        foreach (var row in _database.LoadManufacturers())
        {
            AttachManufacturerAutoSave(row);
            _manufacturerRows.Add(row);
            _persistedManufacturerNames[row.ManufacturerId] = row.Name;
            if (IsManufacturerDraft(row)) _manufacturerRenameDraftIds.Add(row.ManufacturerId);
        }
        _manufacturerView = CollectionViewSource.GetDefaultView(_manufacturerRows);
        _manufacturerView.Filter = ManufacturerProfileFilter;
        ManufacturersGrid.ItemsSource = _manufacturerView;
        _loadingManufacturers = false;
        ResolveLinkedManufacturerNames();
        RefreshFastManufacturerChoices();
        ManufacturerManagerStatusText.Text = $"{_manufacturerRows.Count} SQLite-backed manufacturer profile(s).";
    }

    private void AttachManufacturerAutoSave(ManufacturerRecord row)
    {
        row.PropertyChanged -= ManufacturerRecord_PropertyChanged;
        row.PropertyChanged += ManufacturerRecord_PropertyChanged;
    }

    private void ManufacturerRecord_PropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (_loadingManufacturers || sender is not ManufacturerRecord row) return;
        var isDraftNameEdit =
            string.Equals(e.PropertyName, nameof(ManufacturerRecord.Name), StringComparison.Ordinal) &&
            _manufacturerRenameDraftIds.Contains(row.ManufacturerId);
        if (isDraftNameEdit)
        {
            _loadingManufacturers = true;
            row.DisplayName = row.Name ?? string.Empty;
            _loadingManufacturers = false;
        }
        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                _database.SaveManufacturer(row);
                _persistedManufacturerNames[row.ManufacturerId] = row.Name ?? string.Empty;
                if (string.Equals(e.PropertyName, nameof(ManufacturerRecord.Name), StringComparison.Ordinal))
                {
                    PropagateCanonicalManufacturerName(row);
                }
                RefreshFastManufacturerChoices();
                ManufacturerManagerStatusText.Text = $"Auto-saved {row.Name} at {DateTime.Now:HH:mm:ss}.";
            }
            catch (Exception ex)
            {
                ManufacturerManagerStatusText.Text = "Save failed: " + ex.Message;
            }
        }), DispatcherPriority.Background);
    }

    private bool ManufacturerProfileFilter(object item)
    {
        if (item is not ManufacturerRecord row) return false;
        if (ShowArchivedManufacturersCheckBox?.IsChecked != true && !row.IsActive) return false;
        var search = ManufacturerSearchBox?.Text?.Trim();
        if (string.IsNullOrWhiteSpace(search)) return true;
        return new[] { row.Name, row.DisplayName, row.Country, row.EngineeringFocus, row.MaterialCategories, row.Description }
            .Any(value => value?.Contains(search, StringComparison.OrdinalIgnoreCase) == true);
    }

    private void AddManufacturer_Click(object sender, RoutedEventArgs e)
    {
        var row = new ManufacturerRecord { Name = CreateUniqueManufacturerName("New Manufacturer"), DisplayName = "New Manufacturer", SortOrder = (_manufacturerRows.Count + 1) * 10 };
        _database.SaveManufacturer(row); AttachManufacturerAutoSave(row); _manufacturerRows.Add(row); _persistedManufacturerNames[row.ManufacturerId] = row.Name; _manufacturerRenameDraftIds.Add(row.ManufacturerId); _manufacturerView?.Refresh(); RefreshFastManufacturerChoices(); ManufacturersGrid.SelectedItem = row; ManufacturersGrid.ScrollIntoView(row);
    }

    private ManufacturerRecord? ResolveSelectedManufacturer()
    {
        if (ManufacturersGrid.SelectedItem is ManufacturerRecord selected) return selected;
        if (ManufacturersGrid.CurrentItem is ManufacturerRecord current) return current;
        if (ManufacturersGrid.CurrentCell.Item is ManufacturerRecord currentCell) return currentCell;
        return ManufacturersGrid.SelectedCells
            .Select(cell => cell.Item)
            .OfType<ManufacturerRecord>()
            .FirstOrDefault();
    }

    private void DuplicateManufacturer_Click(object sender, RoutedEventArgs e)
    {
        var source = ResolveSelectedManufacturer();
        if (source is null)
        {
            ManufacturerManagerStatusText.Text = "Select a manufacturer row or cell before duplicating.";
            return;
        }
        var row = new ManufacturerRecord { Name = CreateUniqueManufacturerName(source.Name + " Copy"), DisplayName = source.DisplayName, Country = source.Country, Founded = source.Founded, Website = source.Website, LogoUrl = source.LogoUrl, Description = source.Description, EngineeringFocus = source.EngineeringFocus, MaterialCategories = source.MaterialCategories, Strengths = source.Strengths, Weaknesses = source.Weaknesses, Sustainability = source.Sustainability, TypicalApplications = source.TypicalApplications, Headquarters = source.Headquarters, Notes = source.Notes, SortOrder = source.SortOrder + 1, IsActive = source.IsActive };
        _database.SaveManufacturer(row); AttachManufacturerAutoSave(row); _manufacturerRows.Add(row); _persistedManufacturerNames[row.ManufacturerId] = row.Name; _manufacturerRenameDraftIds.Add(row.ManufacturerId); _manufacturerView?.Refresh(); RefreshFastManufacturerChoices(); ManufacturersGrid.SelectedItem = row;
    }

    private sealed record ExactManufacturerBinding(
        NativeMaterialRow Material,
        ManufacturerRecord Manufacturer);

    private List<ExactManufacturerBinding> BuildExactManufacturerBindingPlan()
    {
        var uniqueCatalogNames = _manufacturerRows
            .Where(row => !string.IsNullOrWhiteSpace(row.Name))
            .GroupBy(row => row.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .Where(group => group.Count() == 1)
            .ToDictionary(
                group => group.Key,
                group => group.Single(),
                StringComparer.OrdinalIgnoreCase);
        return _nativeMaterialRows
            .Where(material =>
                !material.ManufacturerId.HasValue &&
                !string.IsNullOrWhiteSpace(material.Manufacturer) &&
                uniqueCatalogNames.ContainsKey(material.Manufacturer.Trim()))
            .Select(material => new ExactManufacturerBinding(
                material,
                uniqueCatalogNames[material.Manufacturer.Trim()]))
            .ToList();
    }

    private void BindExactMaterialManufacturerNames_Click(object sender, RoutedEventArgs e)
    {
        if (IsAutomationActionBlocked("Manufacturer exact-name binding")) return;
        var plan = BuildExactManufacturerBindingPlan();
        if (plan.Count == 0)
        {
            ManufacturerManagerStatusText.Text =
                "No unlinked Materials have an exact, unique Manufacturer catalog match.";
            return;
        }

        var unmatched = _nativeMaterialRows.Count(material =>
            !material.ManufacturerId.HasValue &&
            !string.IsNullOrWhiteSpace(material.Manufacturer)) - plan.Count;
        var preview = string.Join(
            "\n",
            plan.Take(12).Select(item =>
                $"{item.Material.MaterialID}: {item.Material.Manufacturer} → ID {item.Manufacturer.ManufacturerId}"));
        var result = MessageBox.Show(
            this,
            $"Bind {plan.Count} unlinked Material(s) to exact, unique Manufacturer names?\n\n" +
            $"{preview}" +
            (plan.Count > 12 ? $"\n… and {plan.Count - 12} more" : string.Empty) +
            $"\n\n{unmatched} other unlinked value(s) will remain unchanged. Matching is exact ignoring case; no fuzzy remapping is performed.",
            "Bind Exact Manufacturer Names?",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question,
            MessageBoxResult.No);
        if (result != MessageBoxResult.Yes) return;

        CreateDatabaseBackupBeforeMajorMaterialChange("binding exact Manufacturer identities");
        var originalNames = plan.ToDictionary(
            item => item.Material,
            item => item.Material.Manufacturer);
        foreach (var item in plan)
        {
            item.Material.ManufacturerId = item.Manufacturer.ManufacturerId;
            item.Material.Manufacturer = item.Manufacturer.Name;
        }
        ApplyNativeMaterialComputedFieldsToAllRows();
        RefreshNativeMaterialGridValidation();
        if (!SaveNativeMaterialsSilent())
        {
            foreach (var item in plan)
            {
                item.Material.ManufacturerId = null;
                item.Material.Manufacturer = originalNames[item.Material];
            }
            ManufacturerManagerStatusText.Text =
                "Exact Manufacturer binding could not be saved; the pre-change backup was retained.";
            return;
        }
        _embeddedMaterialsPrototypeView?.SynchronizeFromCanonical(
            "explicit exact Manufacturer identity binding");
        RefreshFastManufacturerChoices();
        QueueNativeMaterialCollectionRefresh();
        QueueNativeMaterialDependentIntelligenceRefresh();
        ManufacturerManagerStatusText.Text =
            $"Bound {plan.Count} Material(s) by explicit exact-name confirmation; {unmatched} remained unlinked.";
    }

    private void ToggleManufacturerActive_Click(object sender, RoutedEventArgs e)
    {
        var row = ResolveSelectedManufacturer();
        if (row is null)
        {
            ManufacturerManagerStatusText.Text = "Select a manufacturer row or cell before archiving or restoring.";
            return;
        }

        row.IsActive = !row.IsActive;
        RefreshFastManufacturerChoices();
        Dispatcher.BeginInvoke(new Action(() => _manufacturerView?.Refresh()), DispatcherPriority.Background);
    }

    private void DeleteManufacturer_Click(object sender, RoutedEventArgs e)
    {
        if (IsAutomationActionBlocked("Manufacturer deletion")) return;
        var row = ResolveSelectedManufacturer();
        if (row is null)
        {
            ManufacturerManagerStatusText.Text = "Select a manufacturer row or cell before deleting.";
            return;
        }
        if (IsManufacturerReferenced(row.ManufacturerId))
        {
            ManufacturerManagerStatusText.Text =
                $"Delete blocked: '{row.Name}' is linked by canonical Materials. Archive it or select replacements explicitly.";
            return;
        }
        if (MessageBox.Show(this, $"Delete manufacturer '{row.Name}'?", "Delete Manufacturer", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        _database.DeleteManufacturer(row.ManufacturerId); _manufacturerRows.Remove(row); _persistedManufacturerNames.Remove(row.ManufacturerId); _manufacturerRenameDraftIds.Remove(row.ManufacturerId); _manufacturerView?.Refresh(); RefreshFastManufacturerChoices(); ManufacturerManagerStatusText.Text = "Manufacturer deleted; automatic database backup created.";
    }

    private void ManufacturerSearchBox_TextChanged(object sender, TextChangedEventArgs e) => _manufacturerView?.Refresh();
    private void ManufacturerFilter_Changed(object sender, RoutedEventArgs e) => _manufacturerView?.Refresh();

    private string CreateUniqueManufacturerName(string seed)
    {
        var name = seed; var index = 2;
        while (_manufacturerRows.Any(row => row.Name.Equals(name, StringComparison.OrdinalIgnoreCase)) ||
               IsManufacturerNameInUse(name))
            name = $"{seed} {index++}";
        return name;
    }

    private static bool IsManufacturerDraft(ManufacturerRecord row) =>
        row.Name.StartsWith("New Manufacturer", StringComparison.OrdinalIgnoreCase) &&
        row.DisplayName.StartsWith("New Manufacturer", StringComparison.OrdinalIgnoreCase);

    private bool IsManufacturerNameInUse(string name) =>
        !string.IsNullOrWhiteSpace(name) &&
        _nativeMaterialRows.Any(row =>
            string.Equals(row.Manufacturer?.Trim(), name.Trim(), StringComparison.OrdinalIgnoreCase));

    private bool IsManufacturerReferenced(long manufacturerId) =>
        manufacturerId > 0 && _nativeMaterialRows.Any(row => row.ManufacturerId == manufacturerId);

    private void ResolveLinkedManufacturerNames()
    {
        var names = _manufacturerRows.ToDictionary(
            row => row.ManufacturerId,
            row => row.Name);
        foreach (var material in _nativeMaterialRows)
        {
            if (material.ManufacturerId is { } manufacturerId &&
                names.TryGetValue(manufacturerId, out var canonicalName))
            {
                material.Manufacturer = canonicalName;
            }
        }
        _embeddedMaterialsPrototypeView?.SynchronizeFromCanonical(
            "canonical Manufacturer identity resolution");
    }

    private void PropagateCanonicalManufacturerName(ManufacturerRecord manufacturer)
    {
        var linked = _nativeMaterialRows
            .Where(row => row.ManufacturerId == manufacturer.ManufacturerId)
            .ToList();
        if (linked.Count == 0) return;

        foreach (var material in linked) material.Manufacturer = manufacturer.Name;
        ApplyNativeMaterialComputedFieldsToAllRows();
        RefreshNativeMaterialGridValidation();
        if (!SaveNativeMaterialsSilent())
            throw new InvalidOperationException("Canonical Manufacturer rename could not update linked Material snapshots.");
        _embeddedMaterialsPrototypeView?.SynchronizeFromCanonical(
            "canonical Manufacturer rename");
        QueueNativeMaterialCollectionRefresh();
        QueueNativeMaterialDependentIntelligenceRefresh();
    }

    private void RenameManufacturerForAuthorizedAutomation(ManufacturerRecord manufacturer, string newName)
    {
        if (AutomationRuntimeProfile.Current?.MaterialCrudAuthorized != true)
            throw new InvalidOperationException("Manufacturer rename automation is not authorized.");
        _loadingManufacturers = true;
        try
        {
            manufacturer.Name = newName;
        }
        finally
        {
            _loadingManufacturers = false;
        }
        _database.SaveManufacturer(manufacturer);
        _persistedManufacturerNames[manufacturer.ManufacturerId] = manufacturer.Name;
        PropagateCanonicalManufacturerName(manufacturer);
        RefreshFastManufacturerChoices();
    }

    private Dictionary<string, ManufacturerRecord> LoadManufacturerKnowledgeByName() => _database.LoadManufacturers()
        .Where(row => row.IsActive)
        .GroupBy(row => row.Name, StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.OrderBy(row => row.SortOrder).First(), StringComparer.OrdinalIgnoreCase);
}
