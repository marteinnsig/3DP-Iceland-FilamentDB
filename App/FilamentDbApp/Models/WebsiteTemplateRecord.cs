namespace FilamentDbApp.Models;

public sealed class WebsiteTemplateRecord
{
    public string WebsiteTemplateId { get; set; } = string.Empty;
    public string TemplateName { get; set; } = string.Empty;
    public string TemplateVersion { get; set; } = string.Empty;
    public string HtmlContent { get; set; } = string.Empty;
    public string ContentHash { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
    public string? SourceFileName { get; set; }
    public string? Notes { get; set; }
    public string DisplayLabel => $"{TemplateVersion} — {SourceFileName ?? TemplateName}" + (IsActive ? " (Active)" : string.Empty);
}
