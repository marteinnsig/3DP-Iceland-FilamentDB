namespace FilamentDbApp.Models;

public sealed record DocumentationSection(
    string Id,
    string Title,
    string Summary,
    IReadOnlyList<string> Details,
    bool VisibleOnWebsite = true,
    bool VisibleInWhitepaper = true);

public sealed record DocumentationDocument(
    string Title,
    string Version,
    DateTime GeneratedAt,
    IReadOnlyList<DocumentationSection> Sections);
