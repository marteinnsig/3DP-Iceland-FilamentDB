using FilamentDbApp.Models;
using FilamentDbApp.Services.Reporting;
using System.IO;
using System.Text;
using System.Text.Json;

namespace FilamentDbApp.Services.Website;

public sealed class PublicReportWebsiteIntegrationService
{
    public const string WebsitePortalMarker = "3DP-PUBLIC-WEBSITE-REPORT-PORTAL-v42.9";
    public const string ProductionPortalRoute = "reports/index.html";
    public const string PreviewPortalRoute = "reports/index-test.html";
    public const string EmbeddedPortalMarker = "3DP-PUBLIC-WEBSITE-REPORT-TAB-v60.0.5";
    private const string EmbeddedPortalStyle = "<style id=\"embeddedPublicReportPortfolioStyles\">#portalPageReports>.public-report-portfolio{max-width:1280px;margin:30px auto;padding:30px;border-radius:18px;background:#111827;color:#e5e7eb;box-shadow:0 20px 55px rgba(0,0,0,.34)}#portalPageReports header{display:grid;grid-template-columns:1fr auto;gap:20px;border-bottom:3px solid #3b82f6}#portalPageReports header img{width:180px}#portalPageReports .cards,#portalPageReports .directory{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:14px;margin:20px 0}#portalPageReports .card,#portalPageReports .entry,#portalPageReports .note{border:1px solid #334155;border-radius:12px;padding:15px;background:#0f172a;color:#e5e7eb}#portalPageReports .value{font-size:25px;font-weight:800}#portalPageReports .entry h3{margin-top:0}#portalPageReports .report-directory-wrap{max-width:100%;overflow-x:auto}#portalPageReports table{width:100%;min-width:720px;border-collapse:collapse;color:#e5e7eb}#portalPageReports th,#portalPageReports td{padding:9px;border-bottom:1px solid #334155;text-align:left}#portalPageReports .material-id{display:block;margin-top:3px;color:#94a3b8;font-size:12px;font-weight:500}#portalPageReports a{color:#60a5fa;font-weight:700}@media(max-width:800px){#portalPageReports>.public-report-portfolio{margin:0;padding:18px;border-radius:0}#portalPageReports header,#portalPageReports .cards,#portalPageReports .directory{grid-template-columns:1fr}}</style>";

    public PublicReportWebsitePackageValidation ValidatePackage(string packageRoot)
    {
        try
        {
            var root = Path.GetFullPath(packageRoot);
            var catalogPath = Path.Combine(root, PublicEngineeringReportPackageService.CatalogFileName);
            var indexPath = Path.Combine(root, "index.html");
            var manifestPath = Path.Combine(root, PublicEngineeringReportPackageService.ManifestFileName);
            var fingerprintPath = Path.Combine(root, PublicReportSourceFingerprintService.FileName);
            if (!NonEmpty(catalogPath) || !NonEmpty(indexPath) || !NonEmpty(manifestPath) || !NonEmpty(fingerprintPath))
                return PublicReportWebsitePackageValidation.Fail("Build Public Report Package first; its index, catalog, manifest or canonical source fingerprint is missing.");
            if (string.IsNullOrWhiteSpace(new PublicReportSourceFingerprintService().ReadFingerprint(fingerprintPath)))
                return PublicReportWebsitePackageValidation.Fail("The public report package canonical source fingerprint is invalid.");
            if (!File.ReadAllText(manifestPath, Encoding.UTF8).Contains(PublicReportSourceFingerprintService.FileName, StringComparison.Ordinal))
                return PublicReportWebsitePackageValidation.Fail("The public report package manifest does not declare its canonical source fingerprint metadata.");

            var envelope = JsonSerializer.Deserialize<CatalogEnvelope>(File.ReadAllText(catalogPath), new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            var model = envelope?.PublicData;
            if (model is null || model.PublicMaterials <= 0 || model.Reports.Count == 0)
                return PublicReportWebsitePackageValidation.Fail("The public report catalog contains no publishable MaterialIDs or report entries.");

            var types = model.Reports.Select(entry => entry.ReportType).Distinct(StringComparer.Ordinal).ToList();
            if (!PublicEngineeringReportPackageService.RequiredReportTypes.All(types.Contains))
                return PublicReportWebsitePackageValidation.Fail("The public report catalog does not contain all six accepted report types.");

            var duplicateRoutes = model.Reports.GroupBy(entry => entry.Html, StringComparer.OrdinalIgnoreCase).Any(group => group.Count() > 1);
            if (duplicateRoutes)
                return PublicReportWebsitePackageValidation.Fail("The public report catalog contains duplicate canonical HTML routes.");

            foreach (var entry in model.Reports)
            {
                foreach (var route in new[] { entry.Html, entry.Pdf, entry.Metadata })
                {
                    if (!TryResolvePublicRoute(root, route, out var sourcePath) || !NonEmpty(sourcePath))
                        return PublicReportWebsitePackageValidation.Fail("A catalog-linked public report artifact is missing or unsafe: " + route);
                }
            }

            return new PublicReportWebsitePackageValidation(true, string.Empty, model.PublicMaterials, model.Reports);
        }
        catch (Exception ex)
        {
            return PublicReportWebsitePackageValidation.Fail("Public report package validation failed: " + ex.Message);
        }
    }

    public PublicReportWebsiteStageResult Stage(string packageRoot, string websiteRoot, bool isProduction)
    {
        var validation = ValidatePackage(packageRoot);
        if (!validation.Passed) throw new InvalidOperationException(validation.Detail);

        var sourceRoot = Path.GetFullPath(packageRoot);
        var destinationRoot = Path.GetFullPath(Path.Combine(websiteRoot, "reports"));
        Directory.CreateDirectory(destinationRoot);
        var copied = 0;

        foreach (var sourceDirectory in validation.Reports
                     .Select(entry => Path.GetDirectoryName(ResolvePublicRoute(sourceRoot, entry.Html))!)
                     .Distinct(StringComparer.OrdinalIgnoreCase))
        {
            var relativeDirectory = Path.GetRelativePath(Path.Combine(sourceRoot, "reports"), sourceDirectory);
            copied += CopyDirectory(sourceDirectory, Path.Combine(destinationRoot, relativeDirectory));
        }

        var sourceAssets = Path.Combine(sourceRoot, "assets");
        if (Directory.Exists(sourceAssets)) copied += CopyDirectory(sourceAssets, Path.Combine(destinationRoot, "assets"));

        File.Copy(Path.Combine(sourceRoot, PublicEngineeringReportPackageService.CatalogFileName), Path.Combine(destinationRoot, PublicEngineeringReportPackageService.CatalogFileName), true);
        File.Copy(Path.Combine(sourceRoot, PublicEngineeringReportPackageService.ManifestFileName), Path.Combine(destinationRoot, PublicEngineeringReportPackageService.ManifestFileName), true);
        File.Copy(Path.Combine(sourceRoot, PublicReportSourceFingerprintService.FileName), Path.Combine(destinationRoot, PublicReportSourceFingerprintService.FileName), true);
        copied += 3;

        var portalFileName = isProduction ? "index.html" : "index-test.html";
        var portalPath = Path.Combine(destinationRoot, portalFileName);
        var portalHtml = File.ReadAllText(Path.Combine(sourceRoot, "index.html"), Encoding.UTF8)
            .Replace("href=\"reports/", "href=\"", StringComparison.Ordinal)
            .Replace("<!-- " + PublicEngineeringReportPackageService.PortfolioMarker + " -->", "<!-- " + PublicEngineeringReportPackageService.PortfolioMarker + " --><!-- " + WebsitePortalMarker + " -->", StringComparison.Ordinal);
        SafeFileOperations.WriteAllTextAtomic(portalPath, portalHtml, Encoding.UTF8);
        copied++;

        foreach (var route in validation.Reports.SelectMany(entry => new[] { entry.Html, entry.Pdf, entry.Metadata }))
        {
            var stagedPath = Path.GetFullPath(Path.Combine(websiteRoot, route.Replace('/', Path.DirectorySeparatorChar)));
            if (!NonEmpty(stagedPath)) throw new InvalidOperationException("Website staging did not create catalog artifact: " + route);
        }
        if (!NonEmpty(portalPath) || !NonEmpty(Path.Combine(destinationRoot, PublicReportSourceFingerprintService.FileName)) || !File.ReadAllText(portalPath, Encoding.UTF8).Contains(WebsitePortalMarker, StringComparison.Ordinal))
            throw new InvalidOperationException("Website report portal marker or index is missing after staging.");

        return new PublicReportWebsiteStageResult(portalPath, validation.PublicMaterials, validation.Reports.Count, copied);
    }

    public string ApplyPortalNavigation(string html, bool isProduction)
    {
        return ApplyPortalNavigation(html, isProduction, string.Empty);
    }

    public string ApplyPortalNavigation(string html, bool isProduction, string embeddedPortalSection)
    {
        if (html.Contains(WebsitePortalMarker, StringComparison.Ordinal)) return html;
        var route = isProduction ? ProductionPortalRoute : PreviewPortalRoute;
        const string methodologyTab = "<button type=\"button\" class=\"portal-tab\" data-portal-target=\"methodology\" aria-controls=\"portalPageMethodology\" aria-selected=\"false\">Methodology</button>";
        const string reportsTab = "    <button type=\"button\" class=\"portal-tab\" data-portal-target=\"reports\" aria-controls=\"portalPageReports\" aria-selected=\"false\">Engineering Reports</button>";
        html = html.Replace(methodologyTab, $"<!-- {WebsitePortalMarker} -->\n{reportsTab}\n{methodologyTab}", StringComparison.Ordinal);
        if (!string.IsNullOrWhiteSpace(embeddedPortalSection))
        {
            const string methodologyPage = "<section id=\"portalPageMethodology\" class=\"portal-page\" data-portal-page=\"methodology\" hidden>";
            html = html.Replace(methodologyPage, embeddedPortalSection + Environment.NewLine + methodologyPage, StringComparison.Ordinal);
        }
        html = html.Replace(
            "<section class=\"card\" id=\"videoReviewCard\">",
            $"<section class=\"card report-portal-cta\"><h2>Public engineering reports</h2><p>Explore the allowlisted MaterialID, comparison, manufacturer, test-session, printing-recommendation and dataset reports.</p><a class=\"product-link\" href=\"#reports\">Open Engineering Reports tab</a> &middot; <a class=\"product-link\" href=\"{route}\">Standalone directory</a></section><section class=\"card\" id=\"videoReviewCard\">",
            StringComparison.Ordinal);
        return html;
    }

    public string BuildEmbeddedPortalSection(string packageRoot, bool isProduction)
    {
        var validation = ValidatePackage(packageRoot);
        if (!validation.Passed) throw new InvalidOperationException(validation.Detail);

        var indexPath = Path.Combine(Path.GetFullPath(packageRoot), "index.html");
        var indexHtml = File.ReadAllText(indexPath, Encoding.UTF8);
        var style = Extract(indexHtml, $"<style id=\"{PublicEngineeringReportPackageService.PortfolioStyleId}\">", "</style>");
        var mainTag = $"<main class=\"{PublicEngineeringReportPackageService.PortfolioMainClass}\">";
        var main = Extract(indexHtml, mainTag, "</main>");
        if (string.IsNullOrWhiteSpace(style) || string.IsNullOrWhiteSpace(main))
            throw new InvalidOperationException("The canonical public report portfolio does not expose its reusable style or content fragment.");

        var content = main[mainTag.Length..(main.Length - "</main>".Length)];
        content = content.Replace("<table>", "<div class=\"report-directory-wrap\" tabindex=\"0\" role=\"region\" aria-label=\"Scrollable material report directory\"><table>", StringComparison.Ordinal)
            .Replace("</table>", "</table></div>", StringComparison.Ordinal)
            .Replace($"<div class=\"note\">Every PDF is printed from the canonical HTML in its report directory. See <a href=\"{PublicEngineeringReportPackageService.CatalogFileName}\">public report catalog metadata</a> and <a href=\"{PublicEngineeringReportPackageService.ManifestFileName}\">package manifest</a>.</div>", string.Empty, StringComparison.Ordinal);
        return $"<!-- {EmbeddedPortalMarker} -->{EmbeddedPortalStyle}<section id=\"portalPageReports\" class=\"portal-page\" data-portal-page=\"reports\" hidden><div class=\"{PublicEngineeringReportPackageService.PortfolioMainClass}\">{content}</div></section>";
    }

    public string BuildPortalContractPlaceholder(bool isProduction)
    {
        var route = isProduction ? ProductionPortalRoute : PreviewPortalRoute;
        return $"<!-- {EmbeddedPortalMarker} -->{EmbeddedPortalStyle}<section id=\"portalPageReports\" class=\"portal-page\" data-portal-page=\"reports\" hidden><div class=\"{PublicEngineeringReportPackageService.PortfolioMainClass}\"><div class=\"note\">Engineering Reports are validated and embedded during website generation. <a href=\"{route}\">Open the standalone directory</a>.</div></div></section>";
    }

    private static int CopyDirectory(string source, string destination)
    {
        Directory.CreateDirectory(destination);
        var copied = 0;
        foreach (var file in Directory.EnumerateFiles(source))
        {
            File.Copy(file, Path.Combine(destination, Path.GetFileName(file)), true);
            copied++;
        }
        foreach (var directory in Directory.EnumerateDirectories(source))
            copied += CopyDirectory(directory, Path.Combine(destination, Path.GetFileName(directory)));
        return copied;
    }

    private static bool NonEmpty(string path) => File.Exists(path) && new FileInfo(path).Length > 0;

    private static string Extract(string html, string openingTag, string closingTag)
    {
        var start = html.IndexOf(openingTag, StringComparison.Ordinal);
        if (start < 0) return string.Empty;
        var contentStart = start + openingTag.Length;
        var end = html.IndexOf(closingTag, contentStart, StringComparison.Ordinal);
        return end < 0 ? string.Empty : openingTag + html[contentStart..end] + closingTag;
    }

    private static bool TryResolvePublicRoute(string root, string route, out string path)
    {
        path = string.Empty;
        if (string.IsNullOrWhiteSpace(route) || !route.StartsWith("reports/", StringComparison.Ordinal) || route.Contains("..", StringComparison.Ordinal) || route.Contains('\\')) return false;
        path = Path.GetFullPath(Path.Combine(root, route.Replace('/', Path.DirectorySeparatorChar)));
        var prefix = root.TrimEnd(Path.DirectorySeparatorChar) + Path.DirectorySeparatorChar;
        return path.StartsWith(prefix, StringComparison.OrdinalIgnoreCase);
    }

    private static string ResolvePublicRoute(string root, string route) =>
        TryResolvePublicRoute(root, route, out var path) ? path : throw new InvalidOperationException("Unsafe public report route: " + route);

    private sealed class CatalogEnvelope
    {
        public PublicEngineeringReportPackageModel? PublicData { get; init; }
    }
}

public sealed record PublicReportWebsitePackageValidation(
    bool Passed,
    string Detail,
    int PublicMaterials,
    IReadOnlyList<PublicReportCatalogEntryModel> Reports)
{
    public static PublicReportWebsitePackageValidation Fail(string detail) => new(false, detail, 0, Array.Empty<PublicReportCatalogEntryModel>());
}

public sealed record PublicReportWebsiteStageResult(string PortalPath, int PublicMaterials, int CatalogEntries, int CopiedFiles);
