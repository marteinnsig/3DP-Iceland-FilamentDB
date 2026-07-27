using System.Windows;
using System.Windows.Controls;

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
            : $"{_visibleSections.Count} topic(s) match “{query}”";

        if (_visibleSections.Count > 0)
        {
            SectionList.SelectedIndex = 0;
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
        BodyText.Text = section.Body;
    }

    private static bool Contains(string value, string query) =>
        value.Contains(query, StringComparison.OrdinalIgnoreCase);

    private void Close_Click(object sender, RoutedEventArgs e) => Close();
}
