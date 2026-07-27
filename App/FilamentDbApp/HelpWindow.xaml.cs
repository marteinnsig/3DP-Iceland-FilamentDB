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
                Contains(section.Body, query) ||
                section.Keywords.Any(keyword => Contains(keyword, query))).ToArray();

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
        CategoryText.Text = section.Category.ToUpperInvariant();
        TitleText.Text = section.Title;
        SummaryText.Text = section.Summary;
        RenderBody(section.Body, SearchBox.Text.Trim());
    }

    private static bool Contains(string value, string query) =>
        value.Contains(query, StringComparison.OrdinalIgnoreCase);

    private void RenderBody(string body, string query)
    {
        var normalized = NormalizeBodyText(body);
        BodyText.Inlines.Clear();
        System.Windows.Automation.AutomationProperties.SetHelpText(
            BodyText,
            string.IsNullOrWhiteSpace(query) ? "No highlighted search" : $"Highlighted search: {query}");

        if (string.IsNullOrWhiteSpace(query))
        {
            BodyText.Inlines.Add(new Run(normalized));
            ContentScrollViewer.ScrollToTop();
            return;
        }

        Run? firstMatch = null;
        var current = 0;
        while (current < normalized.Length)
        {
            var match = normalized.IndexOf(query, current, StringComparison.OrdinalIgnoreCase);
            if (match < 0)
            {
                BodyText.Inlines.Add(new Run(normalized[current..]));
                break;
            }

            if (match > current)
            {
                BodyText.Inlines.Add(new Run(normalized[current..match]));
            }

            var highlighted = new Run(normalized.Substring(match, query.Length))
            {
                Background = new SolidColorBrush(Color.FromRgb(254, 240, 138)),
                Foreground = new SolidColorBrush(Color.FromRgb(113, 63, 18)),
                FontWeight = FontWeights.SemiBold
            };
            BodyText.Inlines.Add(highlighted);
            firstMatch ??= highlighted;
            current = match + query.Length;
        }

        if (normalized.Length == 0)
        {
            BodyText.Inlines.Add(new Run(string.Empty));
        }

        if (firstMatch is not null)
        {
            Dispatcher.BeginInvoke(
                () => firstMatch.BringIntoView(),
                DispatcherPriority.Loaded);
        }
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
