using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Reporting;

public sealed class PublicMaterialSummaryReportPublishingService
{
    public const string StableDirectory = "reports/material-summary";

    public static IReadOnlyList<string> PublicFieldAllowlist { get; } = new[]
    {
        "PublicMaterials", "FullyTested", "PartiallyTested", "NoTestResults", "Manufacturers",
        "MaterialTypes", "MaterialsWithResults", "Coverage", "MaterialTypeDistribution",
        "ManufacturerDistribution", "Materials", "Module", "Label", "MaterialId", "MaterialName",
        "Manufacturer", "BaseMaterial", "TestCoverage", "EngineeringAxes", "OverallScore",
        "TensileScore", "ImpactScore", "StiffnessScore", "ConsistencyScore", "LayerAdhesionScore"
    };

    private static readonly string[] Forbidden =
    {
        "SupplierUrl", "PurchasedFrom", "PurchaseId", "InventoryId", "StorageLocation",
        "BatchNumber", "LandedCost", "Credentials", "DevicePath", "\"Notes\"", ".sqlite"
    };

    public PublicMaterialSummaryPublicationResult Build(
        PublicMaterialSummaryReportModel model,
        DateTime generatedAt,
        string version,
        string releaseTitle)
    {
        if (model.Materials.Count == 0)
            throw new InvalidOperationException("A public Material Summary requires at least one allowlisted MaterialID.");

        return new PublicMaterialSummaryPublicationResult
        {
            RelativeDirectory = StableDirectory,
            Html = BuildHtml(model, generatedAt, version, releaseTitle),
            Manifest = $"3DPIceland Public Material Summary Report\nVersion: {version}\nGenerated: {generatedAt:O}\nMaterials: {model.Materials.Count}\nStable relative directory: {StableDirectory}\nCanonical HTML: index.html\nPDF from canonical HTML: report.pdf\nMetadata: report-metadata.json\nAssets: assets/\nPublic field allowlist: {string.Join(", ", PublicFieldAllowlist)}\n",
            MetadataJson = JsonSerializer.Serialize(new
            {
                schema = "3dpiceland.public-material-summary-report.v1",
                version,
                generatedAt = generatedAt.ToString("O", CultureInfo.InvariantCulture),
                reportKey = "material-summary",
                stableRelativeDirectory = StableDirectory,
                canonicalHtml = "index.html",
                pdf = "report.pdf",
                publicFieldAllowlist = PublicFieldAllowlist,
                publicData = model
            }, new JsonSerializerOptions { WriteIndented = true })
        };
    }

    public PublicComparisonVerificationResult Verify(
        PublicMaterialSummaryReportModel model,
        PublicMaterialSummaryPublicationResult result)
    {
        var payload = string.Join("\n", result.Html, result.Manifest, result.MetadataJson);
        var ids = model.Materials.Select(row => row.MaterialId).ToList();
        var passed = result.RelativeDirectory == StableDirectory &&
                     ids.Count > 0 &&
                     ids.Distinct(StringComparer.OrdinalIgnoreCase).Count() == ids.Count &&
                     model.PublicMaterials == ids.Count &&
                     model.FullyTested + model.PartiallyTested + model.NoTestResults == model.PublicMaterials &&
                     PublicFieldAllowlist.All(field => result.MetadataJson.Contains($"\"{field}\"", StringComparison.Ordinal)) &&
                     Forbidden.All(field => !payload.Contains(field, StringComparison.OrdinalIgnoreCase)) &&
                     !Regex.IsMatch(payload, @"(?<![A-Za-z0-9])[A-Za-z]:[\\/]") &&
                     result.Html.Contains("Native test-result coverage", StringComparison.Ordinal) &&
                     result.Html.Contains("Material type distribution", StringComparison.Ordinal) &&
                     result.Html.Contains("Manufacturer distribution", StringComparison.Ordinal) &&
                     result.Html.Contains("Materials in public report scope", StringComparison.Ordinal) &&
                     result.Html.Contains("Tensile", StringComparison.Ordinal) &&
                     result.Html.Contains("Layer adhesion", StringComparison.Ordinal) &&
                     result.Html.Contains("assets/3dp-iceland-labs-logo-pdf.jpg", StringComparison.Ordinal);

        return new PublicComparisonVerificationResult
        {
            Passed = passed,
            Detail = passed
                ? $"Allowlisted Material Summary ready at {StableDirectory}"
                : "Public Material Summary route, membership, parity, allowlist or exclusion verification failed"
        };
    }

    private static string BuildHtml(PublicMaterialSummaryReportModel model, DateTime at, string version, string release)
    {
        return PublicReportScreenThemeService.Apply(ApplyCanonicalPrintLayout(BuildHtmlCore(model, at, version, release)));
    }

    private static string BuildHtmlCore(PublicMaterialSummaryReportModel model, DateTime at, string version, string release)
    {
        var coverageRows = string.Join("", model.Coverage.Select(row =>
            $"<tr><td>{H(row.Module)}</td><td>{row.Materials}</td><td>{H(row.Coverage)}</td></tr>"));
        var materialRows = string.Join("", model.Materials.Select(row =>
            $"<tr><td>{H(row.MaterialId)}</td><td>{H(row.MaterialName)}</td><td>{H(row.Manufacturer)}</td><td>{H(row.BaseMaterial)}</td><td>{H(row.TestCoverage)}</td><td>{row.EngineeringAxes}/5</td><td>{H(row.OverallScore)}</td><td>{H(row.TensileScore)}</td><td>{H(row.ImpactScore)}</td><td>{H(row.StiffnessScore)}</td><td>{H(row.ConsistencyScore)}</td><td>{H(row.LayerAdhesionScore)}</td><td><a href=\"../materials/{H(PublicReportPublishingService.SafeMaterialIdSegment(row.MaterialId))}/index.html\">Engineering</a> &middot; <a href=\"../printing-recommendations/{H(PublicReportPublishingService.SafeMaterialIdSegment(row.MaterialId))}/index.html\">Recommendation</a> &middot; <a href=\"../test-sessions/{H(PublicReportPublishingService.SafeMaterialIdSegment(row.MaterialId))}/index.html\">Tests</a></td></tr>"));

        return $"<!doctype html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>3DPIceland Public Material Summary</title><style>body{{font-family:Segoe UI,Arial;margin:0;background:#f1f5f9;color:#0f172a}}main{{max-width:1380px;margin:30px auto;background:#fff;padding:30px;border-radius:18px}}header{{display:grid;grid-template-columns:1fr auto;gap:20px;border-bottom:3px solid #0f172a}}header img{{width:180px}}.cards{{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:14px;margin:20px 0}}.card,.note,.chart{{border:1px solid #cbd5e1;border-radius:12px;padding:14px}}.value{{font-size:25px;font-weight:800}}.charts{{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:18px}}.bar{{display:grid;grid-template-columns:minmax(130px,1fr) minmax(130px,2fr) 52px;gap:10px;align-items:center;margin:9px 0}}.track{{height:14px;background:#e2e8f0;border-radius:99px;overflow:hidden}}.fill{{height:100%;background:#0f172a}}.table-wrap{{overflow-x:auto}}table{{width:100%;border-collapse:collapse;margin:12px 0 24px}}th,td{{padding:8px;border-bottom:1px solid #e2e8f0;text-align:left;white-space:nowrap}}a{{color:#1d4ed8}}@media(max-width:800px){{header,.cards,.charts{{grid-template-columns:1fr}}}}@media print{{body{{background:#fff}}main{{margin:0;padding:0}}.chart{{break-inside:avoid}}}}</style></head><body><main><header><div><strong>Public Material Summary</strong><h1>3DPIceland engineering dataset</h1><p>Canonical identity, public test coverage and high-level verified engineering results.</p></div><div><img src=\"assets/3dp-iceland-labs-logo-pdf.jpg\" alt=\"3DPIceland Labs\"><p>{H(version)} - {H(release)}<br>{at:yyyy-MM-dd HH:mm:ss}</p></div></header><div class=\"note\"><strong>Public-data contract:</strong> this snapshot contains only MaterialIDs explicitly approved for public reports and existing Verified Material Summary / governed score outputs. Missing results remain n/a; no measurement or score is recalculated.</div><div class=\"cards\"><div class=\"card\"><div>Public materials</div><div class=\"value\">{model.PublicMaterials}</div></div><div class=\"card\"><div>Fully tested (3 modules)</div><div class=\"value\">{model.FullyTested}</div></div><div class=\"card\"><div>Partially tested</div><div class=\"value\">{model.PartiallyTested}</div></div><div class=\"card\"><div>No test results</div><div class=\"value\">{model.NoTestResults}</div></div><div class=\"card\"><div>Manufacturers</div><div class=\"value\">{model.Manufacturers}</div></div><div class=\"card\"><div>Material types</div><div class=\"value\">{model.MaterialTypes}</div></div></div><h2>Native test-result coverage</h2><table><thead><tr><th>Engineering module / score</th><th>Materials</th><th>Coverage</th></tr></thead><tbody>{coverageRows}</tbody></table><div class=\"charts\">{Distribution("Material type distribution", model.MaterialTypeDistribution, model.PublicMaterials)}{Distribution("Manufacturer distribution", model.ManufacturerDistribution, model.PublicMaterials)}</div><h2>Materials in public report scope</h2><div class=\"table-wrap\"><table><thead><tr><th>MaterialID</th><th>Material</th><th>Manufacturer</th><th>Type</th><th>Test coverage</th><th>Engineering axes</th><th>Overall</th><th>Tensile</th><th>Impact</th><th>Stiffness</th><th>Consistency</th><th>Layer adhesion</th><th>Reports</th></tr></thead><tbody>{materialRows}</tbody></table></div><h2>Methodology and source</h2><div class=\"note\">Verified means an accepted result is available under the platform validation rules. Results are comparative 3DPIceland measurements and do not replace certified manufacturer datasheets or accredited laboratory testing.</div><p><a href=\"https://iskort.is/3dp/index.html#methodology\">Testing methodology</a> &middot; <a href=\"https://iskort.is/3dp/3DPIceland_Labs_Mechanical_Testing_Methodology_v1.0.pdf\">Engineering methodology whitepaper</a> &middot; <a href=\"report.pdf\">Download PDF</a></p><footer>Stable public path: {StableDirectory}/</footer></main></body></html>";
    }

    private static string ApplyCanonicalPrintLayout(string html)
    {
        const string printCss = "@media print{body{background:#fff}main{max-width:none;margin:0;padding:0}header{grid-template-columns:1fr auto!important}.cards{grid-template-columns:repeat(3,minmax(0,1fr))!important}.charts{grid-template-columns:repeat(2,minmax(0,1fr))!important}.chart{break-inside:avoid}.table-wrap{overflow:visible}table{font-size:8px;table-layout:auto}th,td{padding:4px;white-space:normal;overflow-wrap:anywhere}}/* 3DP-PUBLIC-PRINT-LAYOUT-v42.8.3 */";
        return html.Replace("</style>", printCss + "</style>", StringComparison.Ordinal);
    }

    private static string Distribution(string title, IReadOnlyList<PublicSummaryDistributionModel> rows, int total)
    {
        var max = Math.Max(1, rows.Select(row => row.Materials).DefaultIfEmpty().Max());
        var bars = string.Join("", rows.Select(row =>
            $"<div class=\"bar\"><div>{H(row.Label)}</div><div class=\"track\"><div class=\"fill\" style=\"width:{((double)row.Materials / max * 100).ToString("0.#", CultureInfo.InvariantCulture)}%\"></div></div><div>{row.Materials}</div></div>"));
        return $"<section class=\"chart\"><h2>{H(title)}</h2>{bars}<p>{rows.Count} groups across {total} public materials.</p></section>";
    }

    private static string H(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
