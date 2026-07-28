using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Reporting;

public sealed class PublicComparisonReportPublishingService
{
    public string BrandDisplayName { get; set; } =
        DocumentBrandIdentityService.DefaultBrandDisplayName;
    public static IReadOnlyList<string> PublicFieldAllowlist { get; } = new[]
    {
        "PresetSlug", "Title", "BaseMaterial", "ScopeDescription", "Materials",
        "MaterialId", "MaterialName", "Manufacturer", "ProductLine", "Reinforcement", "TestCoverage",
        "VerifiedEngineeringAxes", "OverallScore", "TensileScore", "ImpactScore",
        "StiffnessScore", "ConsistencyScore", "LayerAdhesionScore", "MsrpUsdPerKg"
    };

    private static readonly string[] ForbiddenTokens =
    {
        "\"Password\"", "\"Credential\"", "\"PurchaseId\"", "\"PurchasedFrom\"",
        "\"InventoryId\"", "\"StorageLocation\"", "\"BatchNumber\"", "\"SupplierUrl\"",
        "\"LandedCost", "\"ShippingAmount\"", "\"VatAmount\"", "\"Notes\"", ".sqlite"
    };

    public PublicComparisonPublicationResult Build(PublicComparisonReportModel model, DateTime generatedAt, string versionLabel, string releaseTitle)
    {
        var slug = SafeSlug(model.PresetSlug);
        if (string.IsNullOrWhiteSpace(slug)) throw new InvalidOperationException("A stable comparison preset slug is required.");
        if (model.Materials.Count < 2) throw new InvalidOperationException("A public comparison requires at least two allowlisted materials.");
        var relativeDirectory = $"reports/comparisons/{slug}";
        return new PublicComparisonPublicationResult
        {
            RelativeDirectory = relativeDirectory,
            Html = DocumentBrandTextRendererService.ApplyToPublicReportHtml(
                BuildHtml(
                    model,
                    generatedAt,
                    versionLabel,
                    releaseTitle,
                    relativeDirectory),
                BrandDisplayName,
                versionLabel,
                releaseTitle),
            Manifest = BuildManifest(model, generatedAt, versionLabel, relativeDirectory),
            MetadataJson = BuildMetadata(model, generatedAt, versionLabel, relativeDirectory)
        };
    }

    public PublicComparisonVerificationResult Verify(PublicComparisonReportModel model, PublicComparisonPublicationResult publication)
    {
        var expected = $"reports/comparisons/{SafeSlug(model.PresetSlug)}";
        var payload = string.Join("\n", publication.Html, publication.Manifest, publication.MetadataJson);
        var ids = model.Materials.Select(item => item.MaterialId).ToList();
        var pathPassed = publication.RelativeDirectory == expected;
        var membershipPassed = ids.Count >= 2 && ids.All(id => !string.IsNullOrWhiteSpace(id)) && ids.Distinct(StringComparer.OrdinalIgnoreCase).Count() == ids.Count;
        var allowlistPassed = PublicFieldAllowlist.All(field => publication.MetadataJson.Contains($"\"{field}\"", StringComparison.Ordinal));
        var exclusionPassed = ForbiddenTokens.All(token => !payload.Contains(token, StringComparison.OrdinalIgnoreCase)) &&
                              !Regex.IsMatch(payload, @"(?<![A-Za-z0-9])[A-Za-z]:[\\/]", RegexOptions.CultureInvariant);
        var artifactPassed = publication.Html.Contains("report.pdf", StringComparison.Ordinal) &&
                             publication.Manifest.Contains("Canonical HTML: index.html", StringComparison.Ordinal) &&
                             publication.Manifest.Contains("PDF from canonical HTML: report.pdf", StringComparison.Ordinal);
        var contentPassed = publication.Html.Contains("Public comparison report", StringComparison.Ordinal) &&
                            publication.Html.Contains("Engineering-axis leaders", StringComparison.Ordinal) &&
                            publication.Html.Contains("Overall comparison", StringComparison.Ordinal) &&
                            publication.Html.Contains("Tensile comparison", StringComparison.Ordinal) &&
                            publication.Html.Contains("Impact comparison", StringComparison.Ordinal) &&
                            publication.Html.Contains("Stiffness comparison", StringComparison.Ordinal) &&
                            publication.Html.Contains("comparison-chart-grid", StringComparison.Ordinal) &&
                            publication.Html.Contains("Materials and evidence context", StringComparison.Ordinal) &&
                            publication.Html.Contains("Overall score available", StringComparison.Ordinal) &&
                            publication.Html.Contains("Side-by-side engineering scores", StringComparison.Ordinal) &&
                            publication.Html.Contains("assets/3dp-iceland-labs-logo-pdf.jpg", StringComparison.Ordinal) &&
                            publication.Html.Contains("#methodology", StringComparison.Ordinal);
        var passed = pathPassed && membershipPassed && allowlistPassed && exclusionPassed && artifactPassed && contentPassed;
        return new PublicComparisonVerificationResult
        {
            Passed = passed,
            Detail = passed ? $"Allowlisted comparison ready at {expected}" : $"Path {pathPassed}; membership {membershipPassed}; allowlist {allowlistPassed}; exclusion {exclusionPassed}; artifacts {artifactPassed}; content {contentPassed}"
        };
    }

    public static string SafeSlug(string? value)
    {
        var safe = Regex.Replace((value ?? string.Empty).Trim().ToLowerInvariant(), "[^a-z0-9_-]", "-", RegexOptions.CultureInvariant);
        while (safe.Contains("--", StringComparison.Ordinal)) safe = safe.Replace("--", "-", StringComparison.Ordinal);
        return safe.Trim('-');
    }

    public static string BuildPreviewIndex(IEnumerable<PublicComparisonReportModel> models, DateTime generatedAt)
    {
        var links = models.OrderBy(model => model.Title, StringComparer.CurrentCultureIgnoreCase).Select(model =>
            $"<article><h2>{H(model.Title)}</h2><p>{model.Materials.Count} allowlisted materials</p><a href=\"reports/comparisons/{H(SafeSlug(model.PresetSlug))}/index.html\">Open HTML</a> &middot; <a href=\"reports/comparisons/{H(SafeSlug(model.PresetSlug))}/report.pdf\">Open PDF</a></article>");
        return $"<!doctype html><html><head><meta charset=\"utf-8\"><title>Public Comparison Preview</title><style>body{{font-family:Segoe UI,Arial;margin:32px;background:#f1f5f9;color:#0f172a}}main{{max-width:900px;margin:auto;background:white;padding:28px;border-radius:16px}}article{{border-top:1px solid #cbd5e1;padding:12px 0}}a{{color:#1d4ed8;font-weight:700}}</style></head><body><main><h1>Public Comparison Report Preview</h1><p>Generated {generatedAt:yyyy-MM-dd HH:mm:ss}</p>{string.Join("", links)}<p>Local preview only. Nothing has been uploaded.</p></main></body></html>";
    }

    private static string BuildHtml(PublicComparisonReportModel model, DateTime generatedAt, string versionLabel, string releaseTitle, string relativeDirectory)
    {
        return PublicReportScreenThemeService.Apply(ApplyCanonicalPrintLayout(BuildHtmlCore(model, generatedAt, versionLabel, releaseTitle, relativeDirectory)));
    }

    private static string BuildHtmlCore(PublicComparisonReportModel model, DateTime generatedAt, string versionLabel, string releaseTitle, string relativeDirectory)
    {
        var axes = new (string Name, Func<PublicComparisonMaterialModel, string> Score)[] { ("Overall", x => x.OverallScore), ("Tensile", x => x.TensileScore), ("Impact", x => x.ImpactScore), ("Stiffness", x => x.StiffnessScore), ("Consistency", x => x.ConsistencyScore), ("Layer adhesion", x => x.LayerAdhesionScore) };
        var leaders = axes.Select(axis => model.Materials.Select(item => (Item: item, Score: Number(axis.Score(item)))).Where(x => x.Score.HasValue).OrderByDescending(x => x.Score).ThenBy(x => x.Item.MaterialName).FirstOrDefault()).Select((leader, index) => leader.Item is null ? $"<tr><td>{H(axes[index].Name)}</td><td>&mdash;</td><td>n/a</td></tr>" : $"<tr><td>{H(axes[index].Name)}</td><td>{H(leader.Item.MaterialName)}</td><td>{leader.Score:0.#}</td></tr>");
        var charts = string.Join("", new[]
        {
            BuildScoreChart("Overall comparison", model.Materials, item => item.OverallScore),
            BuildScoreChart("Tensile comparison", model.Materials, item => item.TensileScore),
            BuildScoreChart("Impact comparison", model.Materials, item => item.ImpactScore),
            BuildScoreChart("Stiffness comparison", model.Materials, item => item.StiffnessScore)
        });
        var evidenceRows = model.Materials.Select(item => $"<tr><td>{H(item.MaterialId)}</td><td>{H(item.MaterialName)}</td><td>{H(item.Manufacturer)}</td><td>{H(item.ProductLine)}</td><td>{H(item.BaseMaterial)}</td><td>{H(item.Reinforcement)}</td><td>{H(item.TestCoverage)}</td><td>{item.VerifiedEngineeringAxes}/5</td></tr>");
        var scoredCount = model.Materials.Count(item => Number(item.OverallScore).HasValue);
        var completeCount = model.Materials.Count(item => item.VerifiedEngineeringAxes == 5);
        var rows = model.Materials.Select(item => $"<tr><td>{H(item.MaterialId)}</td><td>{H(item.MaterialName)}</td><td>{H(item.Manufacturer)}</td><td>{H(item.Reinforcement)}</td><td>{H(item.TestCoverage)}</td><td>{item.VerifiedEngineeringAxes}/5</td><td>{H(item.OverallScore)}</td><td>{H(item.TensileScore)}</td><td>{H(item.ImpactScore)}</td><td>{H(item.StiffnessScore)}</td><td>{H(item.ConsistencyScore)}</td><td>{H(item.LayerAdhesionScore)}</td><td>{(string.IsNullOrWhiteSpace(item.MsrpUsdPerKg) ? "n/a" : "$" + H(item.MsrpUsdPerKg))}</td></tr>");
        return $"<!doctype html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>{H(model.Title)}</title><style>body{{font-family:Segoe UI,Arial;margin:0;background:#f1f5f9;color:#0f172a}}main{{max-width:1180px;margin:30px auto;background:#fff;padding:30px;border-radius:18px}}header{{display:flex;justify-content:space-between;border-bottom:3px solid #0f172a}}img{{width:180px}}table{{width:100%;border-collapse:collapse;margin:12px 0 24px}}th,td{{padding:8px;border-bottom:1px solid #e2e8f0;text-align:left}}.note,.card{{padding:14px;background:#f8fafc;border:1px solid #cbd5e1;border-radius:10px;margin:16px 0}}.cards{{display:grid;grid-template-columns:repeat(3,1fr);gap:14px}}.card-value{{font-size:24px;font-weight:800}}.comparison-chart-grid{{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:18px;margin:26px 0}}.comparison-chart{{border:1px solid #cbd5e1;border-radius:14px;padding:18px;background:#fff;break-inside:avoid}}.chart-title{{font-size:19px;font-weight:800;margin-bottom:14px}}.bar-row{{display:grid;grid-template-columns:minmax(150px,1.1fr) minmax(120px,1fr) 72px;gap:10px;align-items:center;margin:10px 0}}.bar-label{{overflow:hidden;text-overflow:ellipsis;white-space:nowrap}}.bar-track{{height:15px;background:#e2e8f0;border-radius:999px;overflow:hidden}}.bar-fill{{height:100%;background:#0f172a;border-radius:999px}}.bar-value{{text-align:right;font-variant-numeric:tabular-nums}}a{{color:#1d4ed8}}@media(max-width:800px){{.comparison-chart-grid,.cards{{grid-template-columns:1fr}}header{{display:block}}.bar-row{{grid-template-columns:minmax(110px,1fr) minmax(90px,1fr) 64px}}}}@media print{{body{{background:white}}main{{margin:0;padding:0}}.comparison-chart-grid{{gap:10px}}}}</style></head><body><main><header><div><strong>Public comparison report</strong><h1>{H(model.Title)}</h1></div><div><img src=\"assets/3dp-iceland-labs-logo-pdf.jpg\" alt=\"3DPIceland Labs\"><p>{H(versionLabel)} - {H(releaseTitle)}<br>{generatedAt:yyyy-MM-dd HH:mm:ss}</p></div></header><div class=\"note\"><strong>Public-data contract:</strong> only explicitly allowlisted MaterialIDs and existing Verified Material Summary scores are included. No measurements or scores are recalculated.</div><div class=\"note\"><strong>Comparison scope:</strong> {H(model.ScopeDescription)}</div><div class=\"cards\"><div class=\"card\"><div>Materials compared</div><div class=\"card-value\">{model.Materials.Count}</div></div><div class=\"card\"><div>Overall score available</div><div class=\"card-value\">{scoredCount}</div></div><div class=\"card\"><div>All 5 axes available</div><div class=\"card-value\">{completeCount}</div></div></div><h2>Engineering-axis leaders in this comparison</h2><table><thead><tr><th>Axis</th><th>Leading material</th><th>Score</th></tr></thead><tbody>{string.Join("", leaders)}</tbody></table><div class=\"comparison-chart-grid\">{charts}</div><h2>Materials and evidence context</h2><table><thead><tr><th>MaterialID</th><th>Material</th><th>Manufacturer</th><th>Product line</th><th>Type</th><th>Reinforcement</th><th>Test coverage</th><th>Engineering axes</th></tr></thead><tbody>{string.Join("", evidenceRows)}</tbody></table><h2>Side-by-side engineering scores</h2><table><thead><tr><th>MaterialID</th><th>Material</th><th>Manufacturer</th><th>Reinforcement</th><th>Coverage</th><th>Evidence</th><th>Overall</th><th>Tensile</th><th>Impact</th><th>Stiffness</th><th>Consistency</th><th>Layer adhesion</th><th>MSRP USD/kg</th></tr></thead><tbody>{string.Join("", rows)}</tbody></table><div class=\"note\">Results are comparative 3DPIceland measurements and do not replace manufacturer datasheets or accredited laboratory testing.</div><p><a href=\"https://iskort.is/3dp/index.html#methodology\">Testing methodology</a> &middot; <a href=\"https://iskort.is/3dp/3DPIceland_Labs_Mechanical_Testing_Methodology_v1.0.pdf\">Engineering methodology whitepaper</a> &middot; <a href=\"report.pdf\">Download PDF</a></p><footer>Stable public path: {H(relativeDirectory)}/</footer></main></body></html>";
    }

    private static string BuildManifest(PublicComparisonReportModel model, DateTime generatedAt, string versionLabel, string directory) => $"3DPIceland Public Comparison Report\nVersion: {versionLabel}\nGenerated: {generatedAt:O}\nPreset: {model.PresetSlug}\nStable relative directory: {directory}\nCanonical HTML: index.html\nPDF from canonical HTML: report.pdf\nMetadata: report-metadata.json\nAssets: assets/\nPublic field allowlist: {string.Join(", ", PublicFieldAllowlist)}\n";
    private static string BuildMetadata(PublicComparisonReportModel model, DateTime generatedAt, string versionLabel, string directory) => JsonSerializer.Serialize(new { schema = "3dpiceland.public-comparison-report.v1", version = versionLabel, generatedAt = generatedAt.ToString("O", CultureInfo.InvariantCulture), reportKey = "comparison", stableRelativeDirectory = directory, canonicalHtml = "index.html", pdf = "report.pdf", publicFieldAllowlist = PublicFieldAllowlist, publicData = model }, new JsonSerializerOptions { WriteIndented = true });
    private static string ApplyCanonicalPrintLayout(string html)
    {
        const string printCss = "@media print{body{background:#fff}main{max-width:none;margin:0;padding:0}header{display:flex!important}.cards{grid-template-columns:repeat(3,1fr)!important}.comparison-chart-grid{grid-template-columns:repeat(2,minmax(0,1fr))!important;gap:10px}.bar-row{grid-template-columns:minmax(125px,1.1fr) minmax(100px,1fr) 58px!important;gap:6px}.comparison-chart{padding:12px;break-inside:avoid}table{font-size:9px;table-layout:auto}th,td{padding:5px;white-space:normal;overflow-wrap:anywhere}}/* 3DP-PUBLIC-PRINT-LAYOUT-v42.8.3 */";
        return html.Replace("</style>", printCss + "</style>", StringComparison.Ordinal);
    }
    private static string BuildScoreChart(string title, IEnumerable<PublicComparisonMaterialModel> materials, Func<PublicComparisonMaterialModel, string> selector)
    {
        var items = materials.Select(item => (item.MaterialName, Score: Number(selector(item)))).Where(item => item.Score.HasValue).Select(item => (item.MaterialName, Score: item.Score!.Value)).OrderByDescending(item => item.Score).ThenBy(item => item.MaterialName, StringComparer.CurrentCultureIgnoreCase).Take(10).ToList();
        if (items.Count == 0) return string.Empty;
        var max = Math.Max(1, items.Max(item => item.Score));
        var rows = items.Select(item => $"<div class=\"bar-row\"><div class=\"bar-label\" title=\"{H(item.MaterialName)}\">{H(item.MaterialName)}</div><div class=\"bar-track\"><div class=\"bar-fill\" style=\"width:{Math.Max(2, Math.Min(100, item.Score / max * 100)).ToString("0.#", CultureInfo.InvariantCulture)}%\"></div></div><div class=\"bar-value\">{H(item.Score.ToString("0.#", CultureInfo.CurrentCulture) + "/100")}</div></div>");
        return $"<section class=\"comparison-chart\"><div class=\"chart-title\">{H(title)}</div>{string.Join("", rows)}</section>";
    }
    private static double? Number(string? value) { var match = Regex.Match(value ?? "", @"-?\d+(?:[.,]\d+)?"); return match.Success && double.TryParse(match.Value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var number) ? number : null; }
    private static string H(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
