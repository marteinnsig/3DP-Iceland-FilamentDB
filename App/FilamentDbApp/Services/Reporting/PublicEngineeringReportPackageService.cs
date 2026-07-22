using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Reporting;

public sealed class PublicEngineeringReportPackageService
{
    public const string CatalogFileName = "report-catalog.json";
    public const string ManifestFileName = "manifest.txt";
    public const string PortfolioMarker = "3DP-PUBLIC-REPORT-PORTFOLIO-v42.8";

    public static IReadOnlyList<string> RequiredReportTypes { get; } = new[]
    {
        "material-summary", "material-engineering", "comparison", "manufacturer", "test-session", "printing-recommendation"
    };

    public static IReadOnlyList<string> PublicFieldAllowlist { get; } = new[]
    {
        "PublicMaterials", "Reports", "ReportType", "Title", "ScopeType", "ScopeId", "Html", "Pdf", "Metadata"
    };

    private static readonly string[] Forbidden =
    {
        "SupplierUrl", "PurchasedFrom", "PurchaseId", "InventoryId", "StorageLocation", "BatchNumber",
        "LandedCost", "Credentials", "DevicePath", "\"Notes\"", ".sqlite"
    };

    public PublicEngineeringReportPackageResult Build(
        PublicEngineeringReportPackageModel model,
        DateTime generatedAt,
        string version,
        string releaseTitle)
    {
        if (model.PublicMaterials <= 0 || model.Reports.Count == 0)
            throw new InvalidOperationException("The public report package requires at least one allowlisted MaterialID and generated report artifacts.");

        var missingTypes = RequiredReportTypes.Except(model.Reports.Select(entry => entry.ReportType), StringComparer.Ordinal).ToList();
        if (missingTypes.Count > 0)
            throw new InvalidOperationException("The public report package is missing report types: " + string.Join(", ", missingTypes));

        var catalog = JsonSerializer.Serialize(new
        {
            schema = "3dpiceland.public-engineering-report-package.v1",
            version,
            generatedAt = generatedAt.ToString("O", CultureInfo.InvariantCulture),
            canonicalIndex = "index.html",
            manifest = ManifestFileName,
            publicFieldAllowlist = PublicFieldAllowlist,
            publicData = model
        }, new JsonSerializerOptions { WriteIndented = true });

        return new PublicEngineeringReportPackageResult
        {
            IndexHtml = PublicReportScreenThemeService.Apply(BuildIndex(model, generatedAt, version, releaseTitle)),
            Manifest = BuildManifest(model, generatedAt, version, releaseTitle),
            CatalogJson = catalog
        };
    }

    public PublicComparisonVerificationResult Verify(
        PublicEngineeringReportPackageModel model,
        PublicEngineeringReportPackageResult result)
    {
        var payload = string.Join("\n", result.IndexHtml, result.Manifest, result.CatalogJson);
        var types = model.Reports.Select(entry => entry.ReportType).Distinct(StringComparer.Ordinal).ToList();
        var routeValues = model.Reports.SelectMany(entry => new[] { entry.Html, entry.Pdf, entry.Metadata }).ToList();
        var passed = RequiredReportTypes.All(types.Contains) &&
                     model.Reports.Select(entry => entry.Html).Distinct(StringComparer.OrdinalIgnoreCase).Count() == model.Reports.Count &&
                     routeValues.All(IsSafePublicRoute) &&
                     PublicFieldAllowlist.All(field => result.CatalogJson.Contains($"\"{field}\"", StringComparison.Ordinal)) &&
                     Forbidden.All(field => !payload.Contains(field, StringComparison.OrdinalIgnoreCase)) &&
                     !Regex.IsMatch(payload, @"(?<![A-Za-z0-9])[A-Za-z]:[\\/]") &&
                     result.IndexHtml.Contains(PortfolioMarker, StringComparison.Ordinal) &&
                     result.IndexHtml.Contains("Public Material Summary", StringComparison.Ordinal) &&
                     result.IndexHtml.Contains("Material Engineering Reports", StringComparison.Ordinal) &&
                     result.IndexHtml.Contains("Material Family Comparisons", StringComparison.Ordinal) &&
                     result.IndexHtml.Contains("Manufacturer Reports", StringComparison.Ordinal) &&
                     result.IndexHtml.Contains("Test Session Evidence", StringComparison.Ordinal) &&
                     result.IndexHtml.Contains("Printing Recommendations", StringComparison.Ordinal) &&
                     result.IndexHtml.Contains("class=\"material-id\"", StringComparison.Ordinal) &&
                     model.Reports.Where(entry => entry.ReportType == "material-engineering").All(entry => result.IndexHtml.Contains(MaterialNameFromEngineeringTitle(entry.Title), StringComparison.Ordinal)) &&
                     result.IndexHtml.Contains("assets/3dp-iceland-labs-logo-pdf.jpg", StringComparison.Ordinal) &&
                     result.Manifest.Contains(CatalogFileName, StringComparison.Ordinal);

        return new PublicComparisonVerificationResult
        {
            Passed = passed,
            Detail = passed
                ? $"Public report portfolio contains {model.Reports.Count} verified artifact entries across all six report types"
                : "Public report package type coverage, routes, allowlist, exclusion, index or manifest verification failed"
        };
    }

    private static string BuildIndex(PublicEngineeringReportPackageModel model, DateTime at, string version, string release)
    {
        var counts = RequiredReportTypes.ToDictionary(type => type, type => model.Reports.Count(entry => entry.ReportType == type), StringComparer.Ordinal);
        var summary = model.Reports.Single(entry => entry.ReportType == "material-summary");
        var comparisons = Cards(model.Reports.Where(entry => entry.ReportType == "comparison"));
        var manufacturers = Cards(model.Reports.Where(entry => entry.ReportType == "manufacturer"));
        var materialIds = model.Reports.Where(entry => entry.ScopeType == "MaterialID").Select(entry => entry.ScopeId).Distinct(StringComparer.OrdinalIgnoreCase).OrderBy(id => id, StringComparer.OrdinalIgnoreCase).ToList();
        var materialRows = string.Join("", materialIds.Select(id =>
        {
            var entries = model.Reports.Where(entry => entry.ScopeType == "MaterialID" && string.Equals(entry.ScopeId, id, StringComparison.OrdinalIgnoreCase)).ToDictionary(entry => entry.ReportType, StringComparer.Ordinal);
            var engineering = entries.GetValueOrDefault("material-engineering");
            var materialName = engineering is null ? id : MaterialNameFromEngineeringTitle(engineering.Title);
            return $"<tr><td><strong>{H(materialName)}</strong><small class=\"material-id\">{H(id)}</small></td><td>{Link(engineering, "Engineering report")}</td><td>{Link(entries.GetValueOrDefault("printing-recommendation"), "Printing guidance")}</td><td>{Link(entries.GetValueOrDefault("test-session"), "Test evidence")}</td></tr>";
        }));

        return $"<!doctype html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>3DPIceland Public Engineering Reports</title><style>body{{font-family:Segoe UI,Arial;margin:0;background:#f1f5f9;color:#0f172a}}main{{max-width:1280px;margin:30px auto;background:#fff;padding:30px;border-radius:18px}}header{{display:grid;grid-template-columns:1fr auto;gap:20px;border-bottom:3px solid #0f172a}}header img{{width:180px}}.cards,.directory{{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:14px;margin:20px 0}}.card,.entry,.note{{border:1px solid #cbd5e1;border-radius:12px;padding:15px}}.value{{font-size:25px;font-weight:800}}.entry h3{{margin-top:0}}table{{width:100%;border-collapse:collapse}}th,td{{padding:9px;border-bottom:1px solid #e2e8f0;text-align:left}}.material-id{{display:block;margin-top:3px;color:#64748b;font-size:12px;font-weight:500}}a{{color:#1d4ed8;font-weight:700}}@media(max-width:800px){{header,.cards,.directory{{grid-template-columns:1fr}}}}@media print{{body{{background:#fff}}main{{margin:0;padding:0}}}}</style></head><body><main><!-- {PortfolioMarker} --><header><div><strong>Public engineering report portfolio</strong><h1>3DPIceland Engineering Reports</h1><p>One canonical catalog for the complete public report portfolio.</p></div><div><img src=\"assets/3dp-iceland-labs-logo-pdf.jpg\" alt=\"3DPIceland Labs\"><p>{H(version)} - {H(release)}<br>{at:yyyy-MM-dd HH:mm:ss}</p></div></header><div class=\"note\"><strong>Package contract:</strong> this index links existing allowlisted canonical HTML/PDF artifacts. It does not merge reports or recalculate measurements and scores.</div><div class=\"cards\"><div class=\"card\"><div>Public materials</div><div class=\"value\">{model.PublicMaterials}</div></div><div class=\"card\"><div>Report artifacts</div><div class=\"value\">{model.Reports.Count}</div></div><div class=\"card\"><div>Report types</div><div class=\"value\">{counts.Count}</div></div><div class=\"card\"><div>Material Engineering Reports</div><div class=\"value\">{counts["material-engineering"]}</div></div><div class=\"card\"><div>Test Session Evidence</div><div class=\"value\">{counts["test-session"]}</div></div><div class=\"card\"><div>Printing Recommendations</div><div class=\"value\">{counts["printing-recommendation"]}</div></div></div><h2>Public Material Summary</h2><div class=\"entry\"><h3>{H(summary.Title)}</h3><p>Dataset identity, public test coverage, material/manufacturer distribution and the complete public score ledger.</p>{Link(summary, "Open HTML")} &middot; <a href=\"{H(summary.Pdf)}\">PDF</a></div><h2>Material Family Comparisons</h2><div class=\"directory\">{comparisons}</div><h2>Manufacturer Reports</h2><div class=\"directory\">{manufacturers}</div><h2>Material-level report directory</h2><table><thead><tr><th>Material</th><th>Material Engineering Reports</th><th>Printing Recommendations</th><th>Test Session Evidence</th></tr></thead><tbody>{materialRows}</tbody></table><div class=\"note\">Every PDF is printed from the canonical HTML in its report directory. See <a href=\"{CatalogFileName}\">public report catalog metadata</a> and <a href=\"{ManifestFileName}\">package manifest</a>.</div></main></body></html>";
    }

    private static string BuildManifest(PublicEngineeringReportPackageModel model, DateTime at, string version, string release)
    {
        var sb = new StringBuilder();
        sb.AppendLine("3DPIceland Public Engineering Report Package");
        sb.AppendLine($"Version: {version} - {release}");
        sb.AppendLine($"Generated: {at:O}");
        sb.AppendLine($"Public MaterialIDs: {model.PublicMaterials}");
        sb.AppendLine($"Catalog entries: {model.Reports.Count}");
        sb.AppendLine("Canonical package index: index.html");
        sb.AppendLine("Catalog metadata: " + CatalogFileName);
        sb.AppendLine("Rendering contract: every linked PDF is printed from its canonical HTML; the package performs no engineering calculations.");
        sb.AppendLine();
        foreach (var entry in model.Reports.OrderBy(entry => entry.ReportType).ThenBy(entry => entry.Title))
            sb.AppendLine($"- {entry.ReportType} | {entry.ScopeType}:{entry.ScopeId} | {entry.Html} | {entry.Pdf} | {entry.Metadata}");
        return sb.ToString();
    }

    private static string Cards(IEnumerable<PublicReportCatalogEntryModel> entries) => string.Join("", entries.OrderBy(entry => entry.Title).Select(entry =>
        $"<article class=\"entry\"><h3>{H(entry.Title)}</h3><p>{H(entry.ScopeType)}: {H(entry.ScopeId)}</p>{Link(entry, "Open HTML")} &middot; <a href=\"{H(entry.Pdf)}\">PDF</a></article>"));

    private static string Link(PublicReportCatalogEntryModel? entry, string label) => entry is null ? "n/a" : $"<a href=\"{H(entry.Html)}\">{H(label)}</a>";

    private static string MaterialNameFromEngineeringTitle(string title)
    {
        const string suffix = " Material Engineering Report";
        return title.EndsWith(suffix, StringComparison.Ordinal) ? title[..^suffix.Length] : title;
    }

    private static bool IsSafePublicRoute(string route) =>
        !string.IsNullOrWhiteSpace(route) &&
        route.StartsWith("reports/", StringComparison.Ordinal) &&
        !route.Contains("..", StringComparison.Ordinal) &&
        !route.Contains('\\') &&
        !Regex.IsMatch(route, @"^[A-Za-z]:");

    private static string H(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
