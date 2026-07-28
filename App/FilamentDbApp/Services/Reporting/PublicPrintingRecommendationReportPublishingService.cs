using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;
namespace FilamentDbApp.Services.Reporting;

public sealed class PublicPrintingRecommendationReportPublishingService
{
    public string BrandDisplayName { get; set; } =
        DocumentBrandIdentityService.DefaultBrandDisplayName;
    public static IReadOnlyList<string> PublicFieldAllowlist { get; } = new[] { "MaterialId", "MaterialName", "Manufacturer", "BaseMaterial", "TestCoverage", "EngineeringAxes", "OverallScore", "TensileScore", "ImpactScore", "StiffnessScore", "ConsistencyScore", "LayerAdhesionScore", "OverallRank", "MsrpUsdPerKg", "RecommendedApplications", "Strengths", "Limitations", "Tradeoffs", "WorkflowChecks", "DecisionGuidance", "Alternatives", "ManufacturerWebsite" }; static readonly string[] Forbidden = { "SupplierUrl", "Purchase", "Inventory", "StorageLocation", "BatchNumber", "Notes", ".sqlite" };
    public PublicPrintingRecommendationPublicationResult Build(
        PublicPrintingRecommendationReportModel model,
        DateTime generatedAt,
        string version,
        string release)
    {
        var materialId = PublicReportPublishingService.SafeMaterialIdSegment(
            model.MaterialId);
        if (string.IsNullOrWhiteSpace(materialId))
            throw new InvalidOperationException("MaterialID required");
        var directory = $"reports/printing-recommendations/{materialId}";
        return new PublicPrintingRecommendationPublicationResult
        {
            RelativeDirectory = directory,
            Html = DocumentBrandTextRendererService.ApplyToPublicReportHtml(
                PublicReportScreenThemeService.Apply(
                    Html(model, generatedAt, version, release, directory)),
                BrandDisplayName,
                version,
                release),
            Manifest =
                $"3DPIceland Public Printing Recommendation Report\n" +
                $"Version: {version}\nGenerated: {generatedAt:O}\n" +
                $"MaterialID: {model.MaterialId}\n" +
                $"Stable relative directory: {directory}\n" +
                "Canonical HTML: index.html\nPDF from canonical HTML: report.pdf\n" +
                "Metadata: report-metadata.json\nAssets: assets/\n",
            MetadataJson = JsonSerializer.Serialize(
                new
                {
                    schema = "3dpiceland.public-printing-recommendation-report.v1",
                    version,
                    generatedAt = generatedAt.ToString("O", CultureInfo.InvariantCulture),
                    reportKey = "printing-recommendation",
                    stableRelativeDirectory = directory,
                    canonicalHtml = "index.html",
                    pdf = "report.pdf",
                    publicFieldAllowlist = PublicFieldAllowlist,
                    publicData = model
                },
                new JsonSerializerOptions { WriteIndented = true })
        };
    }
    public PublicComparisonVerificationResult Verify(PublicPrintingRecommendationReportModel m, PublicPrintingRecommendationPublicationResult r) { var p = string.Join("\n", r.Html, r.Manifest, r.MetadataJson); var required = new[] { "Recommended applications", "Measured strengths", "Limitations", "Engineering trade-offs", "Print workflow checks", "Decision guidance", "Stronger same-family alternatives", "Nozzle temperature", "Not recorded", "assets/3dp-iceland-labs-logo-pdf.jpg" }; var ok = r.RelativeDirectory == $"reports/printing-recommendations/{PublicReportPublishingService.SafeMaterialIdSegment(m.MaterialId)}" && PublicFieldAllowlist.All(x => r.MetadataJson.Contains($"\"{x}\"", StringComparison.Ordinal)) && Forbidden.All(x => !p.Contains(x, StringComparison.OrdinalIgnoreCase)) && !Regex.IsMatch(p, @"(?<![A-Za-z0-9])[A-Za-z]:[\\/]") && required.All(x => r.Html.Contains(x, StringComparison.Ordinal)); return new() { Passed = ok, Detail = ok ? $"Allowlisted recommendation ready at {r.RelativeDirectory}" : "Recommendation route, parity, settings honesty or exclusion failed" }; }
    public static string BuildPreviewIndex(IEnumerable<PublicPrintingRecommendationReportModel> ms, DateTime at) => $"<!doctype html><html><head><meta charset=\"utf-8\"><title>Public Printing Recommendations</title><style>body{{font-family:Segoe UI,Arial;margin:32px;background:#f1f5f9}}main{{max-width:900px;margin:auto;background:white;padding:28px}}article{{border-top:1px solid #ccc;padding:12px}}a{{color:#1d4ed8;font-weight:700}}</style></head><body><main><h1>Public Printing Recommendations</h1><p>{at:yyyy-MM-dd HH:mm:ss}</p>{string.Join("", ms.Select(m => $"<article><h2>{H(m.MaterialName)}</h2><p>{H(m.MaterialId)}</p><a href=\"reports/printing-recommendations/{H(PublicReportPublishingService.SafeMaterialIdSegment(m.MaterialId))}/index.html\">Open HTML</a> &middot; <a href=\"reports/printing-recommendations/{H(PublicReportPublishingService.SafeMaterialIdSegment(m.MaterialId))}/report.pdf\">Open PDF</a></article>"))}<p>Local preview only.</p></main></body></html>";
    static string Html(PublicPrintingRecommendationReportModel m, DateTime at, string version, string release, string dir) { string List(IEnumerable<string> x) => "<ul>" + string.Join("", x.Select(v => $"<li>{H(v)}</li>")) + "</ul>"; var scores = new[] { ("Overall", m.OverallScore), ("Tensile", m.TensileScore), ("Impact", m.ImpactScore), ("Stiffness", m.StiffnessScore), ("Consistency", m.ConsistencyScore), ("Layer adhesion", m.LayerAdhesionScore) }; var bars = string.Join("", scores.Select(x => { var n = Number(x.Item2) ?? 0; return $"<div class=\"bar\"><span>{H(x.Item1)}</span><div class=\"track\"><div class=\"fill\" style=\"width:{Math.Clamp(n, 0, 100).ToString("0.#", CultureInfo.InvariantCulture)}%\"></div></div><b>{H(x.Item2)}</b></div>"; })); var alternatives = string.Join("", m.Alternatives.Select(x => $"<tr><td>{H(x.MaterialId)}</td><td>{H(x.MaterialName)}</td><td>{H(x.Manufacturer)}</td><td>{H(x.OverallScore)}</td><td>{H(x.TensileScore)}</td><td>{H(x.ImpactScore)}</td></tr>")); return $"<!doctype html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>{H(m.MaterialName)} Printing Recommendation</title><style>body{{font-family:Segoe UI,Arial;margin:0;background:#f1f5f9;color:#0f172a}}main{{max-width:1100px;margin:30px auto;background:#fff;padding:30px}}header,.cards,.reviews{{display:grid;grid-template-columns:repeat(2,1fr);gap:16px}}img{{width:180px}}.cards{{grid-template-columns:repeat(4,1fr)}}.card,.note,.panel{{border:1px solid #cbd5e1;padding:14px;border-radius:12px}}.bar{{display:grid;grid-template-columns:130px 1fr 80px;gap:10px;margin:10px}}.track{{height:15px;background:#ddd;border-radius:99px;overflow:hidden}}.fill{{height:100%;background:#0f172a}}table{{width:100%;border-collapse:collapse}}th,td{{padding:8px;border-bottom:1px solid #ddd;text-align:left}}@media(max-width:700px){{header,.cards,.reviews{{grid-template-columns:1fr}}}}</style></head><body><main><header><div><strong>Public printing recommendation</strong><h1>{H(m.MaterialName)}</h1><p>MaterialID {H(m.MaterialId)}</p></div><div><img src=\"assets/3dp-iceland-labs-logo-pdf.jpg\"><p>{H(version)} - {H(release)}<br>{at:yyyy-MM-dd HH:mm:ss}</p></div></header><div class=\"note\"><strong>Settings boundary:</strong> exact printing settings are shown as Not recorded until canonical Material Printing Profile fields exist. No setting is inferred or invented.</div><div class=\"cards\"><div class=\"card\">Overall<br><b>{H(m.OverallScore)}</b></div><div class=\"card\">Engineering axes<br><b>{m.EngineeringAxes}/5</b></div><div class=\"card\">Public rank<br><b>{H(m.OverallRank)}</b></div><div class=\"card\">MSRP USD/kg<br><b>{H(m.MsrpUsdPerKg)}</b></div></div><h2>Engineering profile</h2>{bars}<div class=\"reviews\"><section class=\"panel\"><h2>Recommended applications</h2>{List(m.RecommendedApplications)}</section><section class=\"panel\"><h2>Measured strengths</h2>{List(m.Strengths)}</section><section class=\"panel\"><h2>Limitations</h2>{List(m.Limitations)}</section><section class=\"panel\"><h2>Engineering trade-offs</h2>{List(m.Tradeoffs)}</section></div><h2>Canonical printing settings</h2><table><tbody><tr><th>Nozzle temperature</th><td>Not recorded</td></tr><tr><th>Bed temperature</th><td>Not recorded</td></tr><tr><th>Print speed</th><td>Not recorded</td></tr><tr><th>Cooling</th><td>Not recorded</td></tr><tr><th>Drying time</th><td>Not recorded</td></tr><tr><th>Enclosure requirement</th><td>Not recorded</td></tr><tr><th>Printer / slicer profile</th><td>Not recorded</td></tr></tbody></table><h2>Print workflow checks</h2>{List(m.WorkflowChecks)}<h2>Decision guidance</h2>{List(m.DecisionGuidance)}<h2>Stronger same-family alternatives</h2><table><thead><tr><th>MaterialID</th><th>Material</th><th>Manufacturer</th><th>Overall</th><th>Tensile</th><th>Impact</th></tr></thead><tbody>{alternatives}</tbody></table><div class=\"note\">Confirm exact settings against the manufacturer profile, spool label and actual printer/material combination. Results do not replace application-specific validation.</div><p><a href=\"https://iskort.is/3dp/index.html#methodology\">Testing methodology</a> &middot; <a href=\"report.pdf\">Download PDF</a></p><footer>{H(dir)}/</footer></main></body></html>"; }
    static double? Number(string? s) { var m = Regex.Match(s ?? "", @"-?\d+(?:[.,]\d+)?"); return m.Success && double.TryParse(m.Value.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var n) ? n : null; }
    static string H(string? s) => WebUtility.HtmlEncode(s ?? "");
}
