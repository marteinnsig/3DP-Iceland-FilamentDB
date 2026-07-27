using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;
using System.Text.RegularExpressions;
using System.Windows.Threading;

namespace FilamentDbApp;

public partial class HelpWindow : Window
{
    private IReadOnlyList<HelpSection> _visibleSections = HelpContentCatalog.Sections;

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
        SectionList.SelectedItem = section;
        SectionList.ScrollIntoView(section);
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

        SectionList.ItemsSource = _visibleSections;
        ResultStatusText.Text = string.IsNullOrWhiteSpace(query)
            ? $"{_visibleSections.Count} help topics · Press F1 from the main window for contextual help"
            : $"{_visibleSections.Count} topic(s) match “{query}” · first match highlighted";

        if (_visibleSections.Count > 0)
        {
            var firstSection = _visibleSections[0];
            SectionList.SelectedItem = firstSection;
            SectionList.ScrollIntoView(firstSection);
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

    private void SectionList_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (SectionList.SelectedItem is HelpSection section)
        {
            Render(section);
        }
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
