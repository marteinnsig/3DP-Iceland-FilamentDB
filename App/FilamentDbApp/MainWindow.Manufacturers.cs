using FilamentDbApp.Models;
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
    private ICollectionView? _manufacturerView;
    private bool _loadingManufacturers;

    private void InitializeManufacturerManager()
    {
        _loadingManufacturers = true;
        _manufacturerRows.Clear();
        foreach (var row in _database.LoadManufacturers())
        {
            AttachManufacturerAutoSave(row);
            _manufacturerRows.Add(row);
        }
        _manufacturerView = CollectionViewSource.GetDefaultView(_manufacturerRows);
        _manufacturerView.Filter = ManufacturerProfileFilter;
        ManufacturersGrid.ItemsSource = _manufacturerView;
        _loadingManufacturers = false;
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
        Dispatcher.BeginInvoke(new Action(() =>
        {
            try
            {
                _database.SaveManufacturer(row);
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
        _database.SaveManufacturer(row); AttachManufacturerAutoSave(row); _manufacturerRows.Add(row); _manufacturerView?.Refresh(); ManufacturersGrid.SelectedItem = row; ManufacturersGrid.ScrollIntoView(row);
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
        _database.SaveManufacturer(row); AttachManufacturerAutoSave(row); _manufacturerRows.Add(row); _manufacturerView?.Refresh(); ManufacturersGrid.SelectedItem = row;
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
        Dispatcher.BeginInvoke(new Action(() => _manufacturerView?.Refresh()), DispatcherPriority.Background);
    }

    private void DeleteManufacturer_Click(object sender, RoutedEventArgs e)
    {
        var row = ResolveSelectedManufacturer();
        if (row is null)
        {
            ManufacturerManagerStatusText.Text = "Select a manufacturer row or cell before deleting.";
            return;
        }
        if (MessageBox.Show(this, $"Delete manufacturer '{row.Name}'?", "Delete Manufacturer", MessageBoxButton.YesNo, MessageBoxImage.Warning) != MessageBoxResult.Yes) return;
        _database.DeleteManufacturer(row.ManufacturerId); _manufacturerRows.Remove(row); _manufacturerView?.Refresh(); ManufacturerManagerStatusText.Text = "Manufacturer deleted; automatic database backup created.";
    }

    private void ManufacturerSearchBox_TextChanged(object sender, TextChangedEventArgs e) => _manufacturerView?.Refresh();
    private void ManufacturerFilter_Changed(object sender, RoutedEventArgs e) => _manufacturerView?.Refresh();

    private string CreateUniqueManufacturerName(string seed)
    {
        var name = seed; var index = 2;
        while (_manufacturerRows.Any(row => row.Name.Equals(name, StringComparison.OrdinalIgnoreCase))) name = $"{seed} {index++}";
        return name;
    }

    private Dictionary<string, ManufacturerRecord> LoadManufacturerKnowledgeByName() => _database.LoadManufacturers()
        .Where(row => row.IsActive)
        .GroupBy(row => row.Name, StringComparer.OrdinalIgnoreCase)
        .ToDictionary(group => group.Key, group => group.OrderBy(row => row.SortOrder).First(), StringComparer.OrdinalIgnoreCase);
}
