using System.Net;

namespace FilamentDbApp.Services;

public static class DocumentBrandTextRendererService
{
    public static string ApplyToPublicReportHtml(
        string html,
        string brandDisplayName,
        string versionLabel,
        string releaseTitle)
    {
        var brand = WebUtility.HtmlEncode(brandDisplayName);
        var platformIdentity =
            WebUtility.HtmlEncode(versionLabel + " - " + releaseTitle);
        var provenance =
            $"{brand}<br><small>Generated with 3DPIceland Engineering Platform " +
            $"{platformIdentity}</small>";
        var transformed = html
            .Replace(
                "<h1>3DPIceland engineering dataset</h1>",
                $"<h1>{brand} engineering dataset</h1>",
                StringComparison.Ordinal)
            .Replace(
                "<h1>3DPIceland Engineering Reports</h1>",
                $"<h1>{brand} Engineering Reports</h1>",
                StringComparison.Ordinal)
            .Replace(
                "alt=\"3DPIceland Labs\"",
                $"alt=\"{brand}\"",
                StringComparison.Ordinal);
        var existingProvenance =
            "Generated with 3DPIceland Engineering Platform " + platformIdentity;
        return transformed.Contains(existingProvenance, StringComparison.Ordinal)
            ? transformed
            : transformed.Replace(platformIdentity, provenance, StringComparison.Ordinal);
    }
}
