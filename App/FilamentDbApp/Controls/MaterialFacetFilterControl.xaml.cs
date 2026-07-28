using FilamentDbApp.Models;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace FilamentDbApp.Controls;

public partial class MaterialFacetFilterControl : UserControl
{
    private readonly List<MaterialFacetOption> _options = new();
    private readonly ContextMenu _menu = new();
    private bool _suppressSelectionChanged;

    public MaterialFacetFilterControl()
    {
        InitializeComponent();
        _menu.Placement = PlacementMode.Bottom;
        _menu.PlacementTarget = OpenButton;
        Loaded += (_, _) => ApplyAutomationIds();
        UpdateSummary();
    }

    public static readonly DependencyProperty FilterLabelProperty =
        DependencyProperty.Register(
            nameof(FilterLabel),
            typeof(string),
            typeof(MaterialFacetFilterControl),
            new PropertyMetadata("Filter", OnFilterLabelChanged));

    public string FilterLabel
    {
        get => (string)GetValue(FilterLabelProperty);
        set => SetValue(FilterLabelProperty, value);
    }

    public event EventHandler? SelectionChanged;

    public IReadOnlyList<MaterialFacetSelection> SelectedValues =>
        _options
            .Where(option => option.IsSelected)
            .Select(option => new MaterialFacetSelection(option.Key, option.Label))
            .ToList();

    public void SetOptions(
        IEnumerable<MaterialFacetOption> options,
        IEnumerable<MaterialFacetSelection>? selectedValues)
    {
        var selected = MaterialFilterProjectionService
            .NormalizeSelections(selectedValues)
            .ToDictionary(value => value.Key, StringComparer.Ordinal);

        _suppressSelectionChanged = true;
        try
        {
            _options.Clear();
            foreach (var option in options
                         .OrderBy(item => item.Label, StringComparer.OrdinalIgnoreCase)
                         .ThenBy(item => item.Key, StringComparer.Ordinal))
            {
                _options.Add(option with
                {
                    IsSelected = selected.ContainsKey(option.Key)
                });
                selected.Remove(option.Key);
            }

            foreach (var stale in selected.Values)
            {
                _options.Add(new MaterialFacetOption(
                    stale.Key,
                    stale.Label,
                    0,
                    true,
                    true));
            }

            RebuildMenu();
            UpdateSummary();
        }
        finally
        {
            _suppressSelectionChanged = false;
        }
    }

    public void ClearSelection()
    {
        if (_options.All(option => !option.IsSelected)) return;

        for (var index = 0; index < _options.Count; index++)
        {
            _options[index] = _options[index] with { IsSelected = false };
        }

        RebuildMenu();
        UpdateSummary();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private static void OnFilterLabelChanged(
        DependencyObject dependencyObject,
        DependencyPropertyChangedEventArgs args)
    {
        if (dependencyObject is MaterialFacetFilterControl control)
        {
            control.UpdateSummary();
        }
    }

    private void OpenButton_Click(object sender, RoutedEventArgs e)
    {
        _menu.IsOpen = true;
    }

    private void ClearButton_Click(object sender, RoutedEventArgs e)
    {
        ClearSelection();
    }

    private void RebuildMenu()
    {
        _menu.Items.Clear();
        if (_options.Count == 0)
        {
            _menu.Items.Add(new MenuItem
            {
                Header = "No values available",
                IsEnabled = false
            });
            return;
        }

        foreach (var option in _options)
        {
            var item = new MenuItem
            {
                Header = option.IsStale
                    ? $"{option.Label} (unavailable)"
                    : $"{option.Label} ({option.Count})",
                IsCheckable = true,
                IsChecked = option.IsSelected,
                StaysOpenOnClick = true,
                Tag = option.Key
            };
            AutomationProperties.SetAutomationId(
                item,
                $"MaterialFacetOption_{SanitizeAutomationPart(option.Key)}");
            item.Click += Option_Click;
            _menu.Items.Add(item);
        }
    }

    private void Option_Click(object sender, RoutedEventArgs e)
    {
        if (_suppressSelectionChanged ||
            sender is not MenuItem item ||
            item.Tag is not string key)
        {
            return;
        }

        var index = _options.FindIndex(option =>
            string.Equals(option.Key, key, StringComparison.Ordinal));
        if (index < 0) return;

        _options[index] = _options[index] with
        {
            IsSelected = item.IsChecked
        };
        UpdateSummary();
        SelectionChanged?.Invoke(this, EventArgs.Empty);
    }

    private void UpdateSummary()
    {
        var selected = _options.Where(option => option.IsSelected).ToList();
        OpenButton.Content = selected.Count == 0
            ? $"{FilterLabel}: All"
            : $"{FilterLabel}: {selected.Count} selected";
        ClearButton.IsEnabled = selected.Count > 0;
        SelectionText.Text = selected.Count == 0
            ? "No selection"
            : string.Join(", ", selected.Select(option =>
                option.IsStale ? $"{option.Label} (unavailable)" : option.Label));
    }

    private void ApplyAutomationIds()
    {
        var prefix = string.IsNullOrWhiteSpace(Name)
            ? SanitizeAutomationPart(FilterLabel)
            : Name;
        AutomationProperties.SetAutomationId(OpenButton, prefix + "Open");
        AutomationProperties.SetAutomationId(ClearButton, prefix + "Clear");
        AutomationProperties.SetAutomationId(
            SelectionText,
            prefix + "SelectionSummary");
    }

    private static string SanitizeAutomationPart(string value) =>
        new(value.Select(character =>
                char.IsLetterOrDigit(character) ? character : '_')
            .ToArray());
}

public sealed record MaterialFacetOption(
    string Key,
    string Label,
    int Count,
    bool IsSelected,
    bool IsStale = false);
