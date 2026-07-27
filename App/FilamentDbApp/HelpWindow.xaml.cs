using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Threading;

namespace FilamentDbApp;

public partial class HelpWindow : Window
{
    private IReadOnlyList<HelpSection> _visibleSections = HelpContentCatalog.Sections;
    private IReadOnlyList<HelpNavigationNode> _navigationRoots = [];

    public HelpWindow()
    {
        InitializeComponent();
        ShowSection(HelpContentCatalog.StartHereId);
    }

    public void ShowSection(string sectionId)
    {
        SearchBox.Clear();
        ApplyFilter();
        var section = _visibleSections.FirstOrDefault(item =>
            string.Equals(item.Id, sectionId, StringComparison.Ordinal)) ?? _visibleSections[0];
        SelectNavigationSection(section);
        Render(section);
    }

    private void SearchBox_TextChanged(object sender, TextChangedEventArgs e) => ApplyFilter();

    private void ApplyFilter()
    {
        var query = SearchBox.Text.Trim();
        _visibleSections = string.IsNullOrWhiteSpace(query)
            ? HelpContentCatalog.Sections
            : HelpContentCatalog.Sections.Where(section =>
                Contains(section.Title, query) ||
                Contains(section.Category, query) ||
                Contains(section.Summary, query) ||
                Contains(section.Body, query))
                .OrderBy(section => SearchMatchRank(section, query))
                .ToArray();

        _navigationRoots = BuildNavigationTree(_visibleSections, !string.IsNullOrWhiteSpace(query));
        SectionTree.ItemsSource = _navigationRoots;
        ResultStatusText.Text = string.IsNullOrWhiteSpace(query)
            ? $"{_visibleSections.Count} help topics in {_navigationRoots.Count} categories · expand a category or press F1"
            : $"{_visibleSections.Count} topic(s) match “{query}” · matching categories expanded";

        if (_visibleSections.Count > 0)
        {
            var firstSection = _visibleSections[0];
            SelectNavigationSection(firstSection);
            Render(firstSection);
        }
        else
        {
            CategoryText.Text = "Search";
            TitleText.Text = "No matching help topic";
            SummaryText.Text = "Try a broader term such as purchase, measurement, report, publish or recovery.";
            BodyText.Text = string.Empty;
        }
    }

    private void SectionTree_SelectedItemChanged(
        object sender,
        RoutedPropertyChangedEventArgs<object> e)
    {
        if (e.NewValue is HelpNavigationNode { Section: { } section })
        {
            Render(section);
        }
    }

    private static IReadOnlyList<HelpNavigationNode> BuildNavigationTree(
        IReadOnlyList<HelpSection> sections,
        bool expandAll)
    {
        return sections
            .GroupBy(section => section.Category, StringComparer.Ordinal)
            .Select(group =>
            {
                var children = group
                    .Select(HelpNavigationNode.ForSection)
                    .ToArray();
                return HelpNavigationNode.ForCategory(group.Key, children, expandAll);
            })
            .ToArray();
    }

    private void SelectNavigationSection(HelpSection section)
    {
        foreach (var root in _navigationRoots)
        {
            var node = root.Children.FirstOrDefault(candidate =>
                string.Equals(candidate.Section?.Id, section.Id, StringComparison.Ordinal));
            if (node is null)
            {
                continue;
            }

            root.IsExpanded = true;
            node.IsSelected = true;
            Dispatcher.BeginInvoke(
                () =>
                {
                    if (FindContainer(SectionTree, node) is TreeViewItem container)
                    {
                        container.BringIntoView();
                    }
                },
                DispatcherPriority.Loaded);
            return;
        }
    }

    private static TreeViewItem? FindContainer(ItemsControl parent, object item)
    {
        if (parent.ItemContainerGenerator.ContainerFromItem(item) is TreeViewItem direct)
        {
            return direct;
        }

        foreach (var parentItem in parent.Items)
        {
            if (parent.ItemContainerGenerator.ContainerFromItem(parentItem) is not TreeViewItem child)
            {
                continue;
            }

            var nested = FindContainer(child, item);
            if (nested is not null)
            {
                return nested;
            }
        }

        return null;
    }

    private void Render(HelpSection section)
    {
        var query = SearchBox.Text.Trim();
        ContentScrollViewer.ScrollToTop();
        var categoryMatch = RenderHighlightedText(CategoryText, section.Category.ToUpperInvariant(), query);
        var titleMatch = RenderHighlightedText(TitleText, section.Title, query);
        var summaryMatch = RenderHighlightedText(SummaryText, section.Summary, query);
        var bodyMatch = RenderBody(section.Body, query);
        var firstVisibleMatch = categoryMatch ?? titleMatch ?? summaryMatch ?? bodyMatch;
        if (firstVisibleMatch is not null)
        {
            Dispatcher.BeginInvoke(
                () => firstVisibleMatch.BringIntoView(),
                DispatcherPriority.Loaded);
        }
    }

    private static bool Contains(string value, string query) =>
        value.Contains(query, StringComparison.OrdinalIgnoreCase);

    private static int SearchMatchRank(HelpSection section, string query)
    {
        if (Contains(section.Title, query)) return 0;
        if (Contains(section.Summary, query)) return 1;
        if (Contains(section.Category, query)) return 2;
        return 3;
    }

    private Run? RenderBody(string body, string query)
    {
        var normalized = NormalizeBodyText(body);
        return RenderHighlightedText(BodyText, normalized, query);
    }

    private static Run? RenderHighlightedText(TextBlock target, string text, string query)
    {
        target.Inlines.Clear();
        if (string.IsNullOrWhiteSpace(query))
        {
            target.Inlines.Add(new Run(text));
            System.Windows.Automation.AutomationProperties.SetHelpText(target, "No highlighted search");
            return null;
        }

        Run? firstMatch = null;
        var current = 0;
        while (current < text.Length)
        {
            var match = text.IndexOf(query, current, StringComparison.OrdinalIgnoreCase);
            if (match < 0)
            {
                target.Inlines.Add(new Run(text[current..]));
                break;
            }

            if (match > current)
            {
                target.Inlines.Add(new Run(text[current..match]));
            }

            var highlighted = new Run(text.Substring(match, query.Length))
            {
                Background = new SolidColorBrush(Color.FromRgb(254, 240, 138)),
                Foreground = new SolidColorBrush(Color.FromRgb(113, 63, 18)),
                FontWeight = FontWeights.SemiBold
            };
            target.Inlines.Add(highlighted);
            firstMatch ??= highlighted;
            current = match + query.Length;
        }

        if (text.Length == 0)
        {
            target.Inlines.Add(new Run(string.Empty));
        }

        System.Windows.Automation.AutomationProperties.SetHelpText(
            target,
            firstMatch is null ? "No highlighted search" : $"Highlighted search: {query}");
        return firstMatch;
    }

    private static string NormalizeBodyText(string body)
    {
        var normalizedNewlines = body.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Replace('\r', '\n');
        var paragraphs = Regex.Split(normalizedNewlines.Trim(), @"\n\s*\n");
        return string.Join(
            "\n\n",
            paragraphs.Select(paragraph =>
                Regex.Replace(paragraph.Trim(), @"[ \t]*\n[ \t]*", " ")));
    }

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}

internal sealed class HelpNavigationNode : INotifyPropertyChanged
{
    private bool _isExpanded;
    private bool _isSelected;

    private HelpNavigationNode(
        string header,
        string summary,
        string automationId,
        HelpSection? section,
        IReadOnlyList<HelpNavigationNode> children,
        bool isExpanded)
    {
        Header = header;
        Summary = summary;
        AutomationId = automationId;
        Section = section;
        Children = children;
        _isExpanded = isExpanded;
    }

    public string Header { get; }
    public string Summary { get; }
    public string AutomationId { get; }
    public HelpSection? Section { get; }
    public IReadOnlyList<HelpNavigationNode> Children { get; }
    public bool IsCategory => Section is null;

    public bool IsExpanded
    {
        get => _isExpanded;
        set => SetField(ref _isExpanded, value);
    }

    public bool IsSelected
    {
        get => _isSelected;
        set => SetField(ref _isSelected, value);
    }

    public event PropertyChangedEventHandler? PropertyChanged;

    public static HelpNavigationNode ForCategory(
        string category,
        IReadOnlyList<HelpNavigationNode> children,
        bool isExpanded) =>
        new(
            category,
            string.Empty,
            "HelpCategory-" + ToAutomationKey(category),
            null,
            children,
            isExpanded);

    public static HelpNavigationNode ForSection(HelpSection section) =>
        new(
            section.Title,
            section.Summary,
            "HelpTopic-" + section.Id,
            section,
            [],
            false);

    private static string ToAutomationKey(string value) =>
        Regex.Replace(value.Trim(), @"[^A-Za-z0-9]+", "-").Trim('-');

    private void SetField(ref bool field, bool value, [CallerMemberName] string? propertyName = null)
    {
        if (field == value)
        {
            return;
        }

        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
