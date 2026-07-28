using FilamentDbApp.Models;
using System.Globalization;
using System.Net;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Reporting;

public sealed class PublicTestSessionReportPublishingService
{
    public string BrandDisplayName { get; set; } =
        DocumentBrandIdentityService.DefaultBrandDisplayName;
    public static IReadOnlyList<string> PublicFieldAllowlist { get; } = new[]
    {
        "MaterialId", "MaterialName", "Manufacturer", "SummaryStatus", "ResultModules",
        "SpecimenResultRecords", "PublicDetailsApproved", "VerifiedMeasurements",
        "MeasurementDates", "Tensile", "Impact", "Stiffness", "QualityRows", "Module",
        "Orientation", "Average", "StandardDeviation", "CoefficientOfVariation", "Samples",
        "Confidence", "Validation", "RawInputs", "InputSet", "RecordedValues",
        "ApprovedNotes", "Note"
    };

    private static readonly string[] Forbidden =
    {
        "\"BatchNumber\"", "\"Purchase", "\"Inventory", "\"StorageLocation\"",
        "\"SupplierUrl\"", "\"UpdatedAtUtc\"", "\"CreatedAtUtc\"", ".sqlite"
    };

    public PublicTestSessionPublicationResult Build(
        PublicTestSessionReportModel model,
        DateTime generatedAt,
        string version,
        string release)
    {
        var segment = PublicReportPublishingService.SafeMaterialIdSegment(model.MaterialId);
        if (string.IsNullOrWhiteSpace(segment))
            throw new InvalidOperationException("MaterialID required");
        if (!model.PublicDetailsApproved && (model.RawInputs.Count > 0 || model.ApprovedNotes.Count > 0))
            throw new InvalidOperationException("Raw inputs and notes require explicit public detail approval.");

        var directory = $"reports/test-sessions/{segment}";
        return new PublicTestSessionPublicationResult
        {
            RelativeDirectory = directory,
            Html = DocumentBrandTextRendererService.ApplyToPublicReportHtml(
                PublicReportScreenThemeService.Apply(
                    Html(model, generatedAt, version, release, directory)),
                BrandDisplayName,
                version,
                release),
            Manifest = $"3DPIceland Public Test Session Report\nVersion: {version}\nGenerated: {generatedAt:O}\nMaterialID: {model.MaterialId}\nStable relative directory: {directory}\nCanonical HTML: index.html\nPDF from canonical HTML: report.pdf\nMetadata: report-metadata.json\nAssets: assets/\n",
            MetadataJson = JsonSerializer.Serialize(new
            {
                schema = "3dpiceland.public-test-session-report.v1",
                version,
                generatedAt = generatedAt.ToString("O", CultureInfo.InvariantCulture),
                reportKey = "test-session",
                stableRelativeDirectory = directory,
                canonicalHtml = "index.html",
                pdf = "report.pdf",
                publicFieldAllowlist = PublicFieldAllowlist,
                publicData = model
            }, new JsonSerializerOptions { WriteIndented = true })
        };
    }

    public PublicComparisonVerificationResult Verify(
        PublicTestSessionReportModel model,
        PublicTestSessionPublicationResult publication)
    {
        var payload = string.Join("\n", publication.Html, publication.Manifest, publication.MetadataJson);
        var detailBoundary = model.PublicDetailsApproved ||
            (model.RawInputs.Count == 0 && model.ApprovedNotes.Count == 0 &&
             !publication.Html.Contains("Recorded native inputs", StringComparison.Ordinal));
        var passed =
            publication.RelativeDirectory ==
                $"reports/test-sessions/{PublicReportPublishingService.SafeMaterialIdSegment(model.MaterialId)}" &&
            PublicFieldAllowlist.All(field => publication.MetadataJson.Contains($"\"{field}\"", StringComparison.Ordinal)) &&
            Forbidden.All(token => !payload.Contains(token, StringComparison.OrdinalIgnoreCase)) &&
            !Regex.IsMatch(payload, @"(?<![A-Za-z0-9])[A-Za-z]:[\\/]") &&
            detailBoundary &&
            publication.Html.Contains("Specimen and result quality", StringComparison.Ordinal) &&
            publication.Html.Contains("Measurement provenance", StringComparison.Ordinal) &&
            publication.Html.Contains(H(model.MeasurementDates.Tensile), StringComparison.Ordinal) &&
            publication.Html.Contains(H(model.MeasurementDates.Impact), StringComparison.Ordinal) &&
            publication.Html.Contains(H(model.MeasurementDates.Stiffness), StringComparison.Ordinal) &&
            publication.Html.Contains("Method and equipment context", StringComparison.Ordinal) &&
            publication.Html.Contains("Not recorded", StringComparison.Ordinal) &&
            publication.Html.Contains("assets/3dp-iceland-labs-logo-pdf.jpg", StringComparison.Ordinal);
        return new PublicComparisonVerificationResult
        {
            Passed = passed,
            Detail = passed
                ? $"Allowlisted test-session report ready at {publication.RelativeDirectory}"
                : "Public test-session routing, content or approval boundary failed"
        };
    }

    public static string BuildPreviewIndex(IEnumerable<PublicTestSessionReportModel> models, DateTime generatedAt) =>
        $"<!doctype html><html><head><meta charset=\"utf-8\"><title>Public Test Session Reports</title><style>body{{font-family:Segoe UI,Arial;margin:32px;background:#f1f5f9}}main{{max-width:900px;margin:auto;background:#fff;padding:28px}}article{{border-top:1px solid #ccc;padding:12px}}a{{color:#1d4ed8;font-weight:700}}</style></head><body><main><h1>Public Test Session Reports</h1><p>{generatedAt:yyyy-MM-dd HH:mm:ss}</p>{string.Join("", models.Select(model => $"<article><h2>{H(model.MaterialName)}</h2><p>{H(model.MaterialId)} &middot; details {(model.PublicDetailsApproved ? "approved" : "aggregate only")}</p><a href=\"reports/test-sessions/{H(PublicReportPublishingService.SafeMaterialIdSegment(model.MaterialId))}/index.html\">Open HTML</a> &middot; <a href=\"reports/test-sessions/{H(PublicReportPublishingService.SafeMaterialIdSegment(model.MaterialId))}/report.pdf\">Open PDF</a></article>"))}<p>Local preview only.</p></main></body></html>";

    private static string Html(
        PublicTestSessionReportModel model,
        DateTime generatedAt,
        string version,
        string release,
        string directory)
    {
        var qualityRows = string.Join("", model.QualityRows.Select(row =>
            $"<tr><td>{H(row.Module)}</td><td>{H(row.Orientation)}</td><td>{H(row.Average)}</td><td>{H(row.StandardDeviation)}</td><td>{H(row.CoefficientOfVariation)}</td><td>{row.Samples}</td><td>{H(row.Confidence)}</td><td>{H(row.Validation)}</td></tr>"));
        var dateRows =
            $"<tr><td>Tensile</td><td>{H(model.MeasurementDates.Tensile)}</td></tr>" +
            $"<tr><td>Impact</td><td>{H(model.MeasurementDates.Impact)}</td></tr>" +
            $"<tr><td>Stiffness</td><td>{H(model.MeasurementDates.Stiffness)}</td></tr>";
        var detail = model.PublicDetailsApproved
            ? $"<h2>Recorded native inputs</h2><table><thead><tr><th>Module</th><th>Input set</th><th>Recorded values</th></tr></thead><tbody>{string.Join("", model.RawInputs.Select(row => $"<tr><td>{H(row.Module)}</td><td>{H(row.InputSet)}</td><td>{H(row.RecordedValues)}</td></tr>"))}</tbody></table><h2>Approved test notes</h2><table><tbody>{string.Join("", model.ApprovedNotes.Select(row => $"<tr><td>{H(row.Module)}</td><td>{H(row.Note)}</td></tr>"))}</tbody></table>"
            : "<div class=\"note\"><strong>Detailed-input boundary:</strong> raw inputs and test notes are not published because Public test details is not approved for this MaterialID.</div>";

        return $"<!doctype html><html><head><meta charset=\"utf-8\"><meta name=\"viewport\" content=\"width=device-width,initial-scale=1\"><title>{H(model.MaterialName)} Test Session Report</title><style>body{{font-family:Segoe UI,Arial;margin:0;background:#f1f5f9;color:#0f172a}}main{{max-width:1100px;margin:30px auto;background:#fff;padding:30px}}header,.cards{{display:grid;grid-template-columns:repeat(2,1fr);gap:16px}}img{{width:180px}}.cards{{grid-template-columns:repeat(4,1fr)}}.card,.note{{border:1px solid #cbd5e1;padding:14px;border-radius:12px}}table{{width:100%;border-collapse:collapse;margin:12px 0 24px}}th,td{{padding:8px;border-bottom:1px solid #ddd;text-align:left}}@media(max-width:700px){{header,.cards{{grid-template-columns:1fr}}}}</style></head><body><main><header><div><strong>Public test session report</strong><h1>{H(model.MaterialName)}</h1><p>MaterialID {H(model.MaterialId)}</p></div><div><img src=\"assets/3dp-iceland-labs-logo-pdf.jpg\"><p>{H(version)} - {H(release)}<br>{generatedAt:yyyy-MM-dd HH:mm:ss}</p></div></header><div class=\"note\"><strong>Traceability boundary:</strong> canonical per-module measured dates are shown below. Session identifier, time of day, operator, printer/slicer profile and environmental conditions are Not recorded and are never inferred.</div><h2>Measurement provenance</h2><p>Canonical SQLite measured dates use ISO format; missing dates remain Not recorded.</p><table><thead><tr><th>Module</th><th>Measured date</th></tr></thead><tbody>{dateRows}</tbody></table><div class=\"cards\"><div class=\"card\">Status<br><strong>{H(model.SummaryStatus)}</strong></div><div class=\"card\">Result modules<br><strong>{model.ResultModules}/3</strong></div><div class=\"card\">Specimen/result records<br><strong>{model.SpecimenResultRecords}</strong></div><div class=\"card\">Public details<br><strong>{(model.PublicDetailsApproved ? "Approved" : "Aggregate only")}</strong></div></div><h2>Specimen and result quality</h2><table><thead><tr><th>Module</th><th>Orientation</th><th>Average</th><th>Std. dev.</th><th>CV</th><th>Specimens</th><th>Confidence</th><th>Validation</th></tr></thead><tbody>{qualityRows}</tbody></table>{detail}<h2>Method and equipment context</h2><div class=\"note\">Purpose-built tensile, pendulum impact and three-point bend apparatus are used with the governed Settings Manager constants consumed by ResultsService. Results are comparative and do not claim accredited-laboratory equipment.</div><p><a href=\"https://iskort.is/3dp/index.html#methodology\">Testing methodology</a> &middot; <a href=\"https://iskort.is/3dp/3DPIceland_Labs_Mechanical_Testing_Methodology_v1.0.pdf\">Engineering methodology whitepaper</a> &middot; <a href=\"report.pdf\">Download PDF</a></p><footer>{H(directory)}/</footer></main></body></html>";
    }

    private static string H(string? value) => WebUtility.HtmlEncode(value ?? "");
}
