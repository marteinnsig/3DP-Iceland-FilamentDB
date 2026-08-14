using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Reporting;

/// <summary>
/// Creates a structurally allowlisted public projection of an accepted Material
/// Engineering Report. It never receives purchasing, inventory, credentials,
/// local paths, raw measurements or internal notes.
/// </summary>
public sealed class PublicReportPublishingService
{
    public string BrandDisplayName { get; set; } =
        DocumentBrandIdentityService.DefaultBrandDisplayName;
    public const string PreviewRootFolderName = "public-report-preview";

    public static IReadOnlyList<string> PublicFieldAllowlist { get; } = new[]
    {
        "MaterialID", "MaterialName", "Manufacturer", "ProductLine", "BaseMaterial",
        "MaterialCategory", "VariantFinish", "Reinforcement", "Color", "TestCoverage",
        "OverallScore", "TensileScore", "ImpactScore", "StiffnessScore", "ConsistencyScore",
        "LayerAdhesionScore", "ThermalScore", "ThermalResultTemperatureC", "ThermalMethodVersion",
        "ThermalLimitation", "BestAxis", "MsrpUsdPerKg", "ManufacturerWebsite",
        "VideoReviewUrl", "VerifiedEngineeringAxes", "EngineeringSummary", "ExecutiveReview",
        "BestFeature", "WeakestFeature", "OverallRank", "OverallPercentile",
        "RecommendedApplications", "Strengths", "Limitations", "Tradeoffs", "PeerContext",
        "MaterialAverage", "ManufacturerAverage", "VerifiedMeasurements", "MeasurementDates",
        "Tensile", "Impact", "Stiffness", "MetricPositions",
        "DecisionGuidance", "BetterAlternatives"
    };

    private static readonly string[] ForbiddenPublicFieldTokens =
    {
        "\"Password\"", "\"Credential\"", "\"PrivateKey\"", "\"PurchaseId\"",
        "\"PurchasedFrom\"", "\"InventoryId\"", "\"StorageLocation\"", "\"BatchNumber\"",
        "\"SupplierUrl\"", "\"LandedCostAmount\"", "\"LandedCostUsdPerKg\"",
        "\"ShippingAmount\"", "\"VatAmount\"", "\"Notes\"", "\"UpdatedAtUtc\"", "\"CreatedAtUtc\""
    };

    private static readonly string[] ForbiddenRenderedFragments =
    {
        "<th>Purchase order", "<th>Purchased from", "<th>Inventory", "<th>Storage location",
        "<th>Batch number", "<th>Landed cost", "<h2>Inventory", "spools linked",
        ".sqlite", "Database path:"
    };

    public PublicReportPublicationResult Build(
        PublicMaterialEngineeringReportModel model,
        DateTime generatedAt,
        string versionLabel,
        string releaseTitle)
    {
        var materialSegment = SafeMaterialIdSegment(model.MaterialId);
        if (string.IsNullOrWhiteSpace(materialSegment))
            throw new InvalidOperationException("A canonical MaterialID is required for public report publishing.");

        var relativeDirectory = $"reports/materials/{materialSegment}";
        var html = DocumentBrandTextRendererService.ApplyToPublicReportHtml(
            PublicReportScreenThemeService.Apply(
                BuildHtml(model, generatedAt, versionLabel, releaseTitle)),
            BrandDisplayName,
            versionLabel,
            releaseTitle);
        var manifest = BuildManifest(model, generatedAt, versionLabel, relativeDirectory);
        var metadataJson = BuildMetadata(model, generatedAt, versionLabel, relativeDirectory);
        var previewIndexHtml = BuildPreviewIndex(new[] { model }, generatedAt);

        return new PublicReportPublicationResult
        {
            RelativeDirectory = relativeDirectory,
            Html = html,
            Manifest = manifest,
            MetadataJson = metadataJson,
            PreviewIndexHtml = previewIndexHtml
        };
    }

    public PublicReportPublicationVerificationResult Verify(
        PublicMaterialEngineeringReportModel model,
        PublicReportPublicationResult publication)
    {
        var expectedPath = $"reports/materials/{SafeMaterialIdSegment(model.MaterialId)}";
        var materialIdPathPassed = string.Equals(publication.RelativeDirectory, expectedPath, StringComparison.Ordinal) &&
                                   publication.Html.Contains(WebUtility.HtmlEncode(model.MaterialId), StringComparison.Ordinal);
        var publicFieldAllowlistPassed = PublicFieldAllowlist.Count == 46 &&
                                         publication.MetadataJson.Contains("publicFieldAllowlist", StringComparison.Ordinal) &&
                                         PublicFieldAllowlist.All(field => publication.MetadataJson.Contains($"\"{field}\"", StringComparison.Ordinal));
        var publicPayload = string.Join("\n", publication.Html, publication.Manifest, publication.MetadataJson, publication.PreviewIndexHtml);
        // Validate actual serialized field names and rendered operational sections. Plain
        // explanatory prose may legitimately state that credentials or stock data are excluded.
        var matchedForbiddenTokens = ForbiddenPublicFieldTokens
            .Concat(ForbiddenRenderedFragments)
            .Where(token => publicPayload.Contains(token, StringComparison.OrdinalIgnoreCase))
            .ToList();
        // A drive path must start at a non-alphanumeric boundary. Without this boundary,
        // the trailing "s:/" inside an ordinary https:// public link is a false match.
        var containsDevicePath = Regex.IsMatch(
            publicPayload,
            @"(?<![A-Za-z0-9])[A-Za-z]:[\\/]",
            RegexOptions.CultureInvariant);
        var sensitiveFieldExclusionPassed = matchedForbiddenTokens.Count == 0 && !containsDevicePath;
        var stableArtifactLinksPassed = publication.Html.Contains("report.pdf", StringComparison.Ordinal) &&
                                        publication.PreviewIndexHtml.Contains($"{expectedPath}/index.html", StringComparison.Ordinal) &&
                                        publication.PreviewIndexHtml.Contains($"{expectedPath}/report.pdf", StringComparison.Ordinal) &&
                                        publication.Manifest.Contains("Canonical HTML: index.html", StringComparison.Ordinal) &&
                                        publication.Manifest.Contains("PDF from canonical HTML: report.pdf", StringComparison.Ordinal);
        var methodologyLinksPassed = publication.Html.Contains("https://iskort.is/3dp/index.html#methodology", StringComparison.Ordinal) &&
                                     publication.Html.Contains("3DPIceland_Labs_Mechanical_Testing_Methodology_v1.0.pdf", StringComparison.Ordinal);
        var richContentPassed = publication.Html.Contains("Engineering interpretation", StringComparison.Ordinal) &&
                                publication.Html.Contains("Strengths", StringComparison.Ordinal) &&
                                publication.Html.Contains("Limitations", StringComparison.Ordinal) &&
                                publication.Html.Contains("Recommended applications", StringComparison.Ordinal) &&
                                publication.Html.Contains("Peer context", StringComparison.Ordinal) &&
                                publication.Html.Contains("Engineering score profile", StringComparison.Ordinal) &&
                                publication.Html.Contains("Engineering radar", StringComparison.Ordinal) &&
                                publication.Html.Contains("radar-poly-selected", StringComparison.Ordinal) &&
                                publication.Html.Contains("Material and manufacturer context", StringComparison.Ordinal) &&
                                publication.Html.Contains("Verified measurement results", StringComparison.Ordinal) &&
                                publication.Html.Contains("Measurement provenance", StringComparison.Ordinal) &&
                                publication.Html.Contains("Fixture thermal result", StringComparison.Ordinal) &&
                                publication.Html.Contains("BlueDOT probe-indicated fixture temperature", StringComparison.Ordinal) &&
                                publication.Html.Contains(H(model.MeasurementDates.Tensile), StringComparison.Ordinal) &&
                                publication.Html.Contains(H(model.MeasurementDates.Impact), StringComparison.Ordinal) &&
                                publication.Html.Contains(H(model.MeasurementDates.Stiffness), StringComparison.Ordinal) &&
                                publication.Html.Contains("Metric rankings and percentiles", StringComparison.Ordinal) &&
                                publication.Html.Contains("Decision guidance", StringComparison.Ordinal) &&
                                publication.Html.Contains("Better alternatives", StringComparison.Ordinal) &&
                                 !publication.Html.Contains("Unified HTML report engine", StringComparison.OrdinalIgnoreCase) &&
                                 !publication.Html.Contains("Materials in database", StringComparison.OrdinalIgnoreCase);
        var radarLayoutPassed = publication.Html.Contains("radar-label", StringComparison.Ordinal) &&
                                publication.Html.Contains(">Thermal</text>", StringComparison.Ordinal) &&
                                publication.Html.Contains("viewBox=\"-70 0 490 410\"", StringComparison.Ordinal) &&
                                !Regex.IsMatch(
                                    publication.Html,
                                    @"\b(?:x|y|x1|y1|x2|y2)=""-?\d+,\d+""",
                                    RegexOptions.CultureInvariant);
        var brandingPassed = publication.Html.Contains("assets/3dp-iceland-labs-logo-pdf.jpg", StringComparison.Ordinal);
        var passed = materialIdPathPassed && publicFieldAllowlistPassed && sensitiveFieldExclusionPassed && stableArtifactLinksPassed && methodologyLinksPassed && richContentPassed && radarLayoutPassed && brandingPassed;

        return new PublicReportPublicationVerificationResult
        {
            Passed = passed,
            MaterialIdPathPassed = materialIdPathPassed,
            PublicFieldAllowlistPassed = publicFieldAllowlistPassed,
            SensitiveFieldExclusionPassed = sensitiveFieldExclusionPassed,
            StableArtifactLinksPassed = stableArtifactLinksPassed,
            MethodologyLinksPassed = methodologyLinksPassed,
            RichContentPassed = richContentPassed,
            RadarLayoutPassed = radarLayoutPassed,
            BrandingPassed = brandingPassed,
            Detail = passed
                ? $"Allowlisted Material Engineering publication ready at {expectedPath}"
                : $"Path {materialIdPathPassed}; allowlist {publicFieldAllowlistPassed}; sensitive exclusion {sensitiveFieldExclusionPassed}" +
                  $" (forbidden tokens: {(matchedForbiddenTokens.Count == 0 ? "none" : string.Join(", ", matchedForbiddenTokens))}; device path: {containsDevicePath});" +
                  $" links {stableArtifactLinksPassed}; methodology {methodologyLinksPassed}; rich content {richContentPassed}; radar layout {radarLayoutPassed}; branding {brandingPassed}"
        };
    }

    public static string SafeMaterialIdSegment(string? materialId)
    {
        if (string.IsNullOrWhiteSpace(materialId)) return string.Empty;
        var safe = Regex.Replace(materialId.Trim(), "[^A-Za-z0-9_-]", "-", RegexOptions.CultureInvariant);
        while (safe.Contains("--", StringComparison.Ordinal)) safe = safe.Replace("--", "-", StringComparison.Ordinal);
        return safe.Trim('-');
    }

    private static string BuildHtml(PublicMaterialEngineeringReportModel model, DateTime generatedAt, string versionLabel, string releaseTitle)
    {
        var identityRows = new[]
        {
            ("MaterialID", model.MaterialId), ("Material", model.MaterialName), ("Manufacturer", model.Manufacturer),
            ("Product line", model.ProductLine), ("Base material", model.BaseMaterial), ("Category", model.MaterialCategory),
            ("Variant / finish", model.VariantFinish), ("Reinforcement", model.Reinforcement), ("Color", model.Color)
        };
        var scoreRows = new[]
        {
            ("Overall", model.OverallScore), ("Tensile", model.TensileScore), ("Impact", model.ImpactScore),
            ("Stiffness", model.StiffnessScore), ("Consistency", model.ConsistencyScore),
            ("Layer adhesion", model.LayerAdhesionScore), ("Thermal", model.ThermalScore),
            ("Best available axis", model.BestAxis)
        };
        var identityHtml = string.Join("", identityRows.Select(row => $"<tr><th>{H(row.Item1)}</th><td>{Value(row.Item2)}</td></tr>"));
        var scoresHtml = string.Join("", scoreRows.Select(row => $"<tr><th>{H(row.Item1)}</th><td>{Value(row.Item2)}</td></tr>"));
        var manufacturerLink = PublicLink(model.ManufacturerWebsite, "Manufacturer website");
        var videoLink = PublicLink(model.VideoReviewUrl, "Video review");
        var msrp = string.IsNullOrWhiteSpace(model.MsrpUsdPerKg) ? "n/a" : $"${H(model.MsrpUsdPerKg)} USD/kg";
        var strengths = ListHtml(model.Strengths, "No distinct strength is available from the governed score context.");
        var limitations = ListHtml(model.Limitations, "No distinct limitation is available from the governed score context.");
        var tradeoffs = ListHtml(model.Tradeoffs, "No additional engineering trade-off is available.");
        var applications = ListHtml(model.RecommendedApplications, "Review manually against the intended application.");
        var peerRows = model.PeerContext.Count == 0
            ? "<tr><td colspan=\"6\">No same-material peers with governed score context are available.</td></tr>"
            : string.Join("", model.PeerContext.Select(peer => $"<tr><td>{H(peer.MaterialId)}</td><td>{H(peer.MaterialName)}</td><td>{H(peer.Manufacturer)}</td><td>{H(peer.OverallScore)}</td><td>{H(peer.TensileScore)}</td><td>{H(peer.ImpactScore)}</td></tr>"));
        var scoreProfile = ScoreProfileHtml(model);
        var radar = RadarHtml(model);
        var comparisonRows = ComparisonRowsHtml(model);
        var measurementRows = MeasurementRowsHtml(model.VerifiedMeasurements);
        var measurementDateRows = MeasurementDateRowsHtml(model.MeasurementDates);
        var positionRows = model.MetricPositions.Count == 0
            ? "<tr><td colspan=\"4\">No governed ranking positions are available.</td></tr>"
            : string.Join("", model.MetricPositions.Select(item => $"<tr><td>{H(item.Metric)}</td><td>{Value(item.Score)}</td><td>{Value(item.Rank)}</td><td>{Value(item.Percentile)}</td></tr>"));
        var decisionGuidance = ListHtml(model.DecisionGuidance, "Review the governed score profile against the intended application.");
        var alternativeRows = model.BetterAlternatives.Count == 0
            ? "<tr><td colspan=\"6\">No stronger same-base-material alternatives were identified in the current governed dataset.</td></tr>"
            : string.Join("", model.BetterAlternatives.Select(item => $"<tr><td>{H(item.MaterialId)}</td><td>{H(item.MaterialName)}</td><td>{H(item.Manufacturer)}</td><td>{Value(item.OverallScore)}</td><td>{Value(item.TensileScore)}</td><td>{Value(item.ImpactScore)}</td></tr>"));

        return "<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\">" +
               $"<title>{H(model.MaterialName)} - Material Engineering Report</title>" +
               "<style>body{font-family:Segoe UI,Arial,sans-serif;margin:0;background:#f1f5f9;color:#0f172a}.shell{max-width:1040px;margin:30px auto;background:#fff;border:1px solid #cbd5e1;border-radius:18px;padding:30px;box-shadow:0 14px 40px rgba(15,23,42,.08)}header{display:flex;justify-content:space-between;gap:24px;border-bottom:3px solid #0f172a;padding-bottom:18px}.logo{width:180px;height:auto;display:block;margin:0 0 12px auto}h1{margin:0 0 8px}.meta,.muted{color:#64748b;line-height:1.5}.badge{display:inline-block;padding:6px 10px;border-radius:999px;background:#eff6ff;color:#1e40af;font-weight:700}.cards,.review-grid{display:grid;grid-template-columns:repeat(3,minmax(0,1fr));gap:12px;margin:20px 0}.review-grid{grid-template-columns:repeat(2,minmax(0,1fr))}.card,.review,.chart{border:1px solid #cbd5e1;border-radius:12px;padding:14px;background:#f8fafc}.label{text-transform:uppercase;font-size:11px;letter-spacing:.06em;color:#64748b}.value{font-size:20px;font-weight:800;margin-top:5px}table{width:100%;border-collapse:collapse;margin:12px 0 24px}th,td{text-align:left;border-bottom:1px solid #e2e8f0;padding:9px}th{color:#334155}.note{border:1px solid #cbd5e1;border-radius:12px;background:#f8fafc;padding:15px;line-height:1.5;margin:18px 0}.bar-row{display:grid;grid-template-columns:140px 1fr 72px;gap:10px;align-items:center;margin:9px 0}.bar-track{height:14px;background:#e2e8f0;border-radius:999px;overflow:hidden}.bar-fill{height:100%;background:#0f172a;border-radius:999px}.bar-value{text-align:right;font-variant-numeric:tabular-nums}.radar-wrap{display:grid;grid-template-columns:minmax(0,520px) 1fr;gap:20px;align-items:center}.radar-svg{width:100%;height:auto}.radar-grid{fill:none;stroke:#cbd5e1;stroke-width:1}.radar-axis{stroke:#cbd5e1;stroke-width:1}.radar-poly-selected{fill:rgba(15,23,42,.20);stroke:#0f172a;stroke-width:3}.radar-poly-material{fill:rgba(37,99,235,.08);stroke:#2563eb;stroke-width:2;stroke-dasharray:7 4}.radar-poly-manufacturer{fill:rgba(14,165,233,.06);stroke:#0ea5e9;stroke-width:2;stroke-dasharray:2 4}.radar-label{font-size:12px;fill:#334155;font-weight:650}.legend-item{display:flex;align-items:center;gap:8px;margin:10px 0}.legend-line{width:32px;height:4px;border-radius:99px}li{margin:7px 0;line-height:1.45}a{color:#1d4ed8;font-weight:650}.links{display:flex;gap:16px;flex-wrap:wrap}.footer{margin-top:28px;padding-top:14px;border-top:1px solid #e2e8f0;color:#64748b;font-size:12px}@media(max-width:700px){.shell{margin:0;border-radius:0;padding:20px}.cards,.review-grid,.radar-wrap{grid-template-columns:1fr}header{display:block}.logo{margin:16px 0 0}.bar-row{grid-template-columns:105px 1fr 64px}}@media print{body{background:#fff}.shell{margin:0;border:0;box-shadow:none;padding:0}.card,.note,.review,.chart{break-inside:avoid}}</style></head><body><main class=\"shell\">" +
               $"<header><div><div class=\"badge\">Public engineering report</div><h1>{H(model.MaterialName)}</h1><div class=\"meta\">Material Engineering Report<br>MaterialID {H(model.MaterialId)}</div></div><div><img class=\"logo\" src=\"assets/3dp-iceland-labs-logo-pdf.jpg\" alt=\"3DPIceland Labs\"><div class=\"meta\">3DPIceland Engineering Platform<br>{H(versionLabel)} - {H(releaseTitle)}<br>Generated {generatedAt:yyyy-MM-dd HH:mm:ss}</div></div></header>" +
               "<div class=\"note\"><strong>Public-data contract:</strong> This static report contains only allowlisted material identity, public links, canonical MSRP and existing Verified Material Summary / governed score outputs. It excludes raw specimen rows and non-public operational data.</div>" +
               $"<div class=\"cards\"><div class=\"card\"><div class=\"label\">Test coverage</div><div class=\"value\">{Value(model.TestCoverage)}</div></div><div class=\"card\"><div class=\"label\">Engineering axes</div><div class=\"value\">{model.VerifiedEngineeringAxes}/6</div></div><div class=\"card\"><div class=\"label\">Public MSRP</div><div class=\"value\">{msrp}</div></div></div>" +
               "<h2>Material identity</h2><table><tbody>" + identityHtml + "</tbody></table>" +
               "<h2>Governed engineering profile</h2><table><tbody>" + scoresHtml + "</tbody></table>" +
               "<h2>Engineering score profile</h2><div class=\"chart\">" + scoreProfile + "</div>" +
               "<h2>Engineering radar</h2><div class=\"chart radar-wrap\">" + radar + "</div>" +
               $"<h2>Fixture thermal result</h2><div class=\"note\"><strong>{Number(model.ThermalResultTemperatureC, "°C")}</strong> &middot; score {Value(model.ThermalScore)} &middot; method {Value(model.ThermalMethodVersion)}<br>{Value(model.ThermalLimitation)}</div>" +
               "<h2>Material and manufacturer context</h2><table><thead><tr><th>Metric</th><th>Selected material</th><th>Material average</th><th>Manufacturer average</th></tr></thead><tbody>" + comparisonRows + "</tbody></table>" +
               "<h2>Verified measurement results</h2><p class=\"muted\">Existing Verified Material Summary outputs. The public renderer does not read raw specimen rows or recalculate these values.</p><table><thead><tr><th>Measurement</th><th>Average</th><th>Std. deviation</th><th>CV</th><th>Samples</th><th>Confidence</th></tr></thead><tbody>" + measurementRows + "</tbody></table>" +
               "<h2>Measurement provenance</h2><p class=\"muted\">Canonical per-module measured dates from SQLite, shown in unambiguous ISO format. Missing dates are not inferred.</p><table><thead><tr><th>Module</th><th>Measured date</th></tr></thead><tbody>" + measurementDateRows + "</tbody></table>" +
               $"<h2>Engineering interpretation</h2><div class=\"note\"><strong>Summary:</strong> {Value(model.EngineeringSummary)}</div><div class=\"note\"><strong>Data-driven review:</strong> {Value(model.ExecutiveReview)}</div>" +
               $"<div class=\"cards\"><div class=\"card\"><div class=\"label\">Best feature</div><div class=\"value\">{Value(model.BestFeature)}</div></div><div class=\"card\"><div class=\"label\">Main limitation</div><div class=\"value\">{Value(model.WeakestFeature)}</div></div><div class=\"card\"><div class=\"label\">Dataset position</div><div class=\"value\">{Value(model.OverallPercentile)}</div><div class=\"muted\">{Value(model.OverallRank)}</div></div></div>" +
               $"<div class=\"review-grid\"><section class=\"review\"><h2>Strengths</h2>{strengths}</section><section class=\"review\"><h2>Limitations</h2>{limitations}</section><section class=\"review\"><h2>Engineering trade-offs</h2>{tradeoffs}</section><section class=\"review\"><h2>Recommended applications</h2>{applications}</section></div>" +
               "<h2>Metric rankings and percentiles</h2><table><thead><tr><th>Metric</th><th>Score</th><th>Rank</th><th>Percentile</th></tr></thead><tbody>" + positionRows + "</tbody></table>" +
               "<h2>Decision guidance</h2><div class=\"note\">" + decisionGuidance + "</div>" +
               "<h2>Better alternatives</h2><table><thead><tr><th>MaterialID</th><th>Material</th><th>Manufacturer</th><th>Overall</th><th>Tensile</th><th>Impact</th></tr></thead><tbody>" + alternativeRows + "</tbody></table>" +
               "<h2>Peer context</h2><p class=\"muted\">Highest-scoring same-base-material peers from the current governed dataset. This context is descriptive and is not recalculated by the public renderer.</p><table><thead><tr><th>MaterialID</th><th>Material</th><th>Manufacturer</th><th>Overall</th><th>Tensile</th><th>Impact</th></tr></thead><tbody>" + peerRows + "</tbody></table>" +
               $"<h2>Public links</h2><div class=\"links\">{manufacturerLink}{videoLink}<a href=\"report.pdf\">Download PDF</a></div>" +
               "<h2>Methodology and limitations</h2><div class=\"note\">Results are comparative 3DPIceland measurements from home-built test equipment. Thermal is a nearby BlueDOT probe-indicated fixture temperature, not ASTM D648, ISO 75 or specimen temperature. Missing evidence remains n/a. Scores and test results are not recalculated by this publishing layer and do not replace certified manufacturer datasheets or accredited laboratory testing.</div>" +
               "<ul><li><a href=\"https://iskort.is/3dp/index.html#methodology\">Testing methodology</a></li><li><a href=\"https://iskort.is/3dp/3DPIceland_Labs_Mechanical_Testing_Methodology_v1.0.pdf\">Engineering methodology whitepaper</a></li></ul>" +
               $"<div class=\"footer\">Stable public path: reports/materials/{H(SafeMaterialIdSegment(model.MaterialId))}/ &middot; Canonical HTML: index.html &middot; PDF printed from this HTML.</div>" +
               "</main></body></html>";
    }

    private static string BuildManifest(PublicMaterialEngineeringReportModel model, DateTime generatedAt, string versionLabel, string relativeDirectory)
    {
        return "3DPIceland Public Material Engineering Report\n" +
               $"Version: {versionLabel}\nGenerated: {generatedAt:yyyy-MM-dd HH:mm:ss}\nMaterialID: {model.MaterialId}\n" +
               $"Stable relative directory: {relativeDirectory}\nCanonical HTML: index.html\nPDF from canonical HTML: report.pdf\nMetadata: report-metadata.json\nAssets: assets/\n" +
               "Public field allowlist: " + string.Join(", ", PublicFieldAllowlist) + "\n" +
               "Excluded by contract: purchasing, operational stock data, credentials, device filesystem locations, raw specimen rows and internal notes.\n";
    }

    private static string BuildMetadata(PublicMaterialEngineeringReportModel model, DateTime generatedAt, string versionLabel, string relativeDirectory)
    {
        var payload = new
        {
            schema = "3dpiceland.public-material-engineering-report.v1",
            version = versionLabel,
            generatedAt = generatedAt.ToString("O", CultureInfo.InvariantCulture),
            reportKey = "material-engineering",
            materialId = model.MaterialId,
            materialName = model.MaterialName,
            stableRelativeDirectory = relativeDirectory,
            canonicalHtml = "index.html",
            pdf = "report.pdf",
            publicFieldAllowlist = PublicFieldAllowlist,
            publicData = model
        };
        return JsonSerializer.Serialize(payload, new JsonSerializerOptions { WriteIndented = true });
    }

    public static string BuildPreviewIndex(IEnumerable<PublicMaterialEngineeringReportModel> models, DateTime generatedAt)
    {
        var entries = models
            .Where(model => !string.IsNullOrWhiteSpace(model.MaterialId))
            .OrderBy(model => model.MaterialName, StringComparer.CurrentCultureIgnoreCase)
            .ThenBy(model => model.MaterialId, StringComparer.OrdinalIgnoreCase)
            .Select(model =>
            {
                var relativeDirectory = $"reports/materials/{SafeMaterialIdSegment(model.MaterialId)}";
                return $"<article><h2>{H(model.MaterialName)}</h2><p>MaterialID {H(model.MaterialId)}</p>" +
                       $"<p><a href=\"{H(relativeDirectory)}/index.html\">Open canonical HTML report</a> &middot; " +
                       $"<a href=\"{H(relativeDirectory)}/report.pdf\">Open PDF</a></p></article>";
            });

        return "<!doctype html><html lang=\"en\"><head><meta charset=\"utf-8\"><title>3DPIceland Public Report Preview</title>" +
               "<style>body{font-family:Segoe UI,Arial,sans-serif;margin:32px;background:#f1f5f9;color:#0f172a}.card{max-width:920px;margin:auto;background:#fff;border:1px solid #cbd5e1;border-radius:16px;padding:26px}article{border-top:1px solid #e2e8f0;padding:14px 0}a{color:#1d4ed8;font-weight:700}</style></head><body><div class=\"card\">" +
               $"<h1>Public Report Preview</h1><p>Generated {generatedAt:yyyy-MM-dd HH:mm:ss}</p>{string.Join(string.Empty, entries)}" +
               "<p>This is a local preview package. It has not been uploaded to the production website.</p></div></body></html>";
    }

    private static string PublicLink(string? value, string label)
    {
        if (!Uri.TryCreate(value?.Trim(), UriKind.Absolute, out var uri) ||
            (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)) return string.Empty;
        return $"<a href=\"{H(uri.AbsoluteUri)}\" target=\"_blank\" rel=\"noopener noreferrer\">{H(label)}</a>";
    }

    private static string Value(string? value) => string.IsNullOrWhiteSpace(value) ? "&mdash;" : H(value);
    private static string ListHtml(IEnumerable<string> values, string emptyText)
    {
        var items = values.Where(value => !string.IsNullOrWhiteSpace(value)).Select(value => $"<li>{H(value)}</li>").ToList();
        return items.Count == 0 ? $"<p class=\"muted\">{H(emptyText)}</p>" : "<ul>" + string.Join("", items) + "</ul>";
    }
    private static string ScoreProfileHtml(PublicMaterialEngineeringReportModel model)
    {
        var rows = new[]
        {
            ("Overall", model.OverallScore), ("Tensile", model.TensileScore), ("Impact", model.ImpactScore),
            ("Stiffness", model.StiffnessScore), ("Consistency", model.ConsistencyScore),
            ("Layer adhesion", model.LayerAdhesionScore), ("Thermal", model.ThermalScore)
        };
        return string.Join("", rows.Select(row =>
        {
            var score = ScoreValue(row.Item2);
            var width = score.HasValue ? Math.Clamp(score.Value, 0, 100).ToString("0.#", CultureInfo.InvariantCulture) : "0";
            return $"<div class=\"bar-row\"><div>{H(row.Item1)}</div><div class=\"bar-track\"><div class=\"bar-fill\" style=\"width:{width}%\"></div></div><div class=\"bar-value\">{Value(row.Item2)}</div></div>";
        }));
    }
    private static string ComparisonRowsHtml(PublicMaterialEngineeringReportModel model)
    {
        var rows = new[]
        {
            ("Overall", model.OverallScore, model.MaterialAverage.OverallScore, model.ManufacturerAverage.OverallScore),
            ("Tensile", model.TensileScore, model.MaterialAverage.TensileScore, model.ManufacturerAverage.TensileScore),
            ("Impact", model.ImpactScore, model.MaterialAverage.ImpactScore, model.ManufacturerAverage.ImpactScore),
            ("Stiffness", model.StiffnessScore, model.MaterialAverage.StiffnessScore, model.ManufacturerAverage.StiffnessScore),
            ("Consistency", model.ConsistencyScore, model.MaterialAverage.ConsistencyScore, model.ManufacturerAverage.ConsistencyScore),
            ("Layer adhesion", model.LayerAdhesionScore, model.MaterialAverage.LayerAdhesionScore, model.ManufacturerAverage.LayerAdhesionScore),
            ("Thermal", model.ThermalScore, model.MaterialAverage.ThermalScore, model.ManufacturerAverage.ThermalScore)
        };
        return string.Join("", rows.Select(row => $"<tr><td>{H(row.Item1)}</td><td>{Value(row.Item2)}</td><td>{Value(row.Item3)}</td><td>{Value(row.Item4)}</td></tr>"));
    }
    private static string RadarHtml(PublicMaterialEngineeringReportModel model)
    {
        var labels = new[] { "Tensile", "Impact", "Stiffness", "Thermal", "Layer adhesion", "Consistency" };
        var selected = new[] { model.TensileScore, model.ImpactScore, model.StiffnessScore, model.ThermalScore, model.LayerAdhesionScore, model.ConsistencyScore };
        var material = ProfileScores(model.MaterialAverage);
        var manufacturer = ProfileScores(model.ManufacturerAverage);
        var grid = string.Join("", new[] { 25d, 50d, 75d, 100d }.Select(level => $"<polygon class=\"radar-grid\" points=\"{RadarPoints(Enumerable.Repeat(level, 6))}\"/>"));
        var axes = string.Join("", Enumerable.Range(0, 6).Select(index =>
        {
            var point = RadarPoint(index, 100);
            var labelPoint = RadarPoint(index, 119);
            var anchor = labelPoint.X < 196 ? "end" : labelPoint.X > 224 ? "start" : "middle";
            return $"<line class=\"radar-axis\" x1=\"210\" y1=\"200\" x2=\"{SvgNumber(point.X)}\" y2=\"{SvgNumber(point.Y)}\"/><text class=\"radar-label\" x=\"{SvgNumber(labelPoint.X)}\" y=\"{SvgNumber(labelPoint.Y)}\" text-anchor=\"{anchor}\">{H(labels[index])}</text>";
        }));
        var svg = $"<svg class=\"radar-svg\" viewBox=\"-70 0 490 410\" role=\"img\" aria-label=\"Engineering radar comparing selected material with material and manufacturer averages\">{grid}{axes}<polygon class=\"radar-poly-manufacturer\" points=\"{RadarPoints(manufacturer)}\"/><polygon class=\"radar-poly-material\" points=\"{RadarPoints(material)}\"/><polygon class=\"radar-poly-selected\" points=\"{RadarPoints(selected.Select(value => ScoreValue(value) ?? 0))}\"/></svg>";
        var legend = "<div><div class=\"legend-item\"><span class=\"legend-line\" style=\"background:#0f172a\"></span><strong>Selected material</strong></div>" +
                     $"<div class=\"legend-item\"><span class=\"legend-line\" style=\"background:#2563eb\"></span><span>{Value(model.MaterialAverage.Label)}</span></div>" +
                     $"<div class=\"legend-item\"><span class=\"legend-line\" style=\"background:#0ea5e9\"></span><span>{Value(model.ManufacturerAverage.Label)}</span></div>" +
                     "<p class=\"muted\">All axes use existing governed 0–100 engineering scores. Missing axes are shown at zero and remain listed as n/a elsewhere in the report.</p></div>";
        return svg + legend;
    }
    private static IEnumerable<double> ProfileScores(PublicEngineeringScoreProfile profile) => new[]
    {
        profile.TensileScore, profile.ImpactScore, profile.StiffnessScore, profile.ThermalScore, profile.LayerAdhesionScore, profile.ConsistencyScore
    }.Select(value => ScoreValue(value) ?? 0);
    private static string RadarPoints(IEnumerable<double> values) => string.Join(" ", values.Select((value, index) =>
    {
        var point = RadarPoint(index, Math.Clamp(value, 0, 100));
        return point.X.ToString("0.#", CultureInfo.InvariantCulture) + "," + point.Y.ToString("0.#", CultureInfo.InvariantCulture);
    }));
    private static string SvgNumber(double value) => value.ToString("0.#", CultureInfo.InvariantCulture);
    private static (double X, double Y) RadarPoint(int index, double score)
    {
        var angle = (-90 + index * 60) * Math.PI / 180.0;
        var radius = 146 * score / 100.0;
        return (210 + Math.Cos(angle) * radius, 200 + Math.Sin(angle) * radius);
    }
    private static string MeasurementRowsHtml(PublicVerifiedMeasurementsModel measurements)
    {
        var rows = new[]
        {
            MeasurementRow("Tensile upright", measurements.TensileUpright, "MPa"),
            MeasurementRow("Tensile flat", measurements.TensileFlat, "MPa"),
            MeasurementRow("Impact upright", measurements.ImpactUpright, "kJ/m²"),
            MeasurementRow("Impact flat", measurements.ImpactFlat, "kJ/m²")
        };
        var stiffness = $"<tr><td>Stiffness modulus</td><td>{Number(measurements.StiffnessModulusMpa, "MPa")}</td><td>&mdash;</td><td>&mdash;</td><td>&mdash;</td><td>&mdash;</td></tr>" +
                        $"<tr><td>Stiffness deflection</td><td>{Number(measurements.StiffnessDeflectionMm, "mm")}</td><td>&mdash;</td><td>&mdash;</td><td>&mdash;</td><td>&mdash;</td></tr>";
        return string.Join("", rows) + stiffness;
    }
    private static string MeasurementDateRowsHtml(PublicMeasurementDateProvenanceModel dates) =>
        $"<tr><td>Tensile</td><td>{H(dates.Tensile)}</td></tr>" +
        $"<tr><td>Impact</td><td>{H(dates.Impact)}</td></tr>" +
        $"<tr><td>Stiffness</td><td>{H(dates.Stiffness)}</td></tr>" +
        $"<tr><td>Thermal</td><td>{H(dates.Thermal)}</td></tr>";
    private static string MeasurementRow(string label, PublicMeasurementSetModel result, string unit) =>
        $"<tr><td>{H(label)}</td><td>{Number(result.Average, unit)}</td><td>{Number(result.StandardDeviation, unit)}</td><td>{Percent(result.CoefficientOfVariation)}</td><td>{(result.SampleCount > 0 ? result.SampleCount.ToString(CultureInfo.InvariantCulture) : "&mdash;")}</td><td>{(result.Confidence.HasValue ? H(result.Confidence.Value.ToString(CultureInfo.InvariantCulture) + "/10") : "&mdash;")}</td></tr>";
    private static string Number(double? value, string unit) => value.HasValue && double.IsFinite(value.Value) ? H(value.Value.ToString("0.###", CultureInfo.InvariantCulture) + " " + unit) : "&mdash;";
    private static string Percent(double? value) => value.HasValue && double.IsFinite(value.Value) ? H((value.Value * 100).ToString("0.#", CultureInfo.InvariantCulture) + "%") : "&mdash;";
    private static double? ScoreValue(string? value)
    {
        var match = Regex.Match(value ?? string.Empty, @"-?\d+(?:[.,]\d+)?", RegexOptions.CultureInvariant);
        if (!match.Success) return null;
        var normalized = match.Value.Replace(',', '.');
        return double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out var parsed) ? parsed : null;
    }
    private static string H(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
}
