using System.Globalization;
using System.IO;
using System.Text;

namespace FilamentDbApp.Services.Reporting;

public sealed class ReportPdfRendererService
{
    private const string ContentType = "application/pdf";
    private const string LogoFileName = "3dp-iceland-labs-logo-pdf.jpg";

    public ReportingPdfDocument Render(ReportingReportModel reportModel)
    {
        return Render(
            reportModel,
            TryLoadLogo(),
            DocumentBrandIdentityService.DefaultBrandDisplayName);
    }

    public ReportingPdfDocument Render(
        ReportingReportModel reportModel,
        DocumentBrandingRenderAsset branding)
    {
        ArgumentNullException.ThrowIfNull(branding);
        return Render(
            reportModel,
            new PdfLogoAsset(
                branding.JpegBytes,
                branding.PixelWidth,
                branding.PixelHeight),
            branding.BrandDisplayName);
    }

    private static ReportingPdfDocument Render(
        ReportingReportModel reportModel,
        PdfLogoAsset? logo,
        string brandDisplayName)
    {
        var pages = reportModel.MaterialReports
            .Where(report => !string.IsNullOrWhiteSpace(report.MaterialId))
            .Select(report => BuildPage(report, brandDisplayName))
            .ToList();

        var bytes = BuildBrandedPdf(pages, logo, brandDisplayName);
        return new ReportingPdfDocument(
            FileName: "3DPIceland_Material_Engineering_Report.pdf",
            ContentType: ContentType,
            Bytes: bytes,
            PageCount: pages.Count,
            MaterialReportsRendered: pages.Count,
            RenderedAtUtc: DateTime.UtcNow,
            Payload: new ReportingPdfPayload(pages));
    }

    public ReportingPdfRendererVerificationResult Verify(ReportingReportModel reportModel)
    {
        return Verify(reportModel, null);
    }

    public ReportingPdfRendererVerificationResult Verify(
        ReportingReportModel reportModel,
        DocumentBrandingRenderAsset? branding)
    {
        var document = branding is null
            ? Render(reportModel)
            : Render(reportModel, branding);
        var payload = document.Payload;
        var materialIds = payload.Pages
            .Select(page => page.MaterialId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();
        var pdfText = Encoding.ASCII.GetString(document.Bytes.Where(b => b < 128).ToArray());
        var logo = branding is null
            ? TryLoadLogo()
            : new PdfLogoAsset(
                branding.JpegBytes,
                branding.PixelWidth,
                branding.PixelHeight);

        var result = new ReportingPdfRendererVerificationResult
        {
            InputReports = reportModel.MaterialReports.Count,
            RenderedReports = document.MaterialReportsRendered,
            PageCount = document.PageCount,
            PayloadPages = payload.Pages.Count,
            ByteCount = document.Bytes.Length,
            ContentType = document.ContentType,
            HasPdfHeader = document.Bytes.Length > 5 && Encoding.ASCII.GetString(document.Bytes, 0, 5) == "%PDF-",
            HasPdfTrailer = pdfText.Contains("%%EOF", StringComparison.Ordinal),
            MaterialIdCoverage = materialIds.Count == reportModel.MaterialReports.Count && materialIds.Count == materialIds.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            PayloadValidationPassed = payload.Pages.Count == reportModel.MaterialReports.Count && payload.Pages.All(page => page.Lines.Count > 0),
            RenderReady = document.Bytes.Length > 0 && document.PageCount == reportModel.MaterialReports.Count && document.ContentType == ContentType,
            HasBrandedLayout = pdfText.Contains("3DPIceland Branded PDF Renderer", StringComparison.Ordinal) && pdfText.Contains("/ImLogo", StringComparison.Ordinal),
            HasLogoAsset = logo is not null && logo.Bytes.Length > 0,
            LogoAssetName = LogoFileName
        };

        result.Passed = result.InputReports > 0 &&
                        result.RenderedReports == result.InputReports &&
                        result.PageCount == result.InputReports &&
                        result.PayloadPages == result.InputReports &&
                        result.ByteCount > 0 &&
                        result.HasPdfHeader &&
                        result.HasPdfTrailer &&
                        result.MaterialIdCoverage &&
                        result.PayloadValidationPassed &&
                        result.RenderReady &&
                        result.HasBrandedLayout &&
                        result.HasLogoAsset;

        return result;
    }

    private static ReportingPdfPage BuildPage(
        ReportingMaterialReportModel report,
        string brandDisplayName)
    {
        var lines = new List<string>
        {
            brandDisplayName + " Material Engineering Report",
            "MaterialID: " + report.MaterialId,
            "Label: " + Clean(report.Label),
            "Manufacturer: " + Clean(report.Manufacturer),
            "Product Line: " + Clean(report.ProductLine),
            "Base Material: " + Clean(report.BaseMaterial),
            "Data Source: Verified Material Summary",
            "Calculation Owner: Engineering Platform"
        };

        foreach (var section in report.Sections)
        {
            lines.Add(section.Title);
            foreach (var field in section.Fields)
            {
                lines.Add("  " + field.Key + ": " + Clean(field.Value));
            }
        }

        return new ReportingPdfPage(report.MaterialId, lines);
    }

    private static string Clean(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "-" : value.Trim();
    }

    private static PdfLogoAsset? TryLoadLogo()
    {
        var candidatePaths = new[]
        {
            Path.Combine(AppContext.BaseDirectory, "Assets", LogoFileName),
            Path.Combine(AppContext.BaseDirectory, LogoFileName),
            Path.Combine(Directory.GetCurrentDirectory(), "Assets", LogoFileName)
        };

        foreach (var path in candidatePaths)
        {
            if (!File.Exists(path)) continue;
            var bytes = File.ReadAllBytes(path);
            if (bytes.Length > 0)
            {
                return new PdfLogoAsset(bytes, 801, 482);
            }
        }

        return null;
    }

    private static byte[] BuildBrandedPdf(
        IReadOnlyList<ReportingPdfPage> pages,
        PdfLogoAsset? logo,
        string brandDisplayName)
    {
        var objects = new List<byte[]>();
        var pageObjectNumbers = new List<int>();

        objects.Add(Ascii("<< /Type /Catalog /Pages 2 0 R >>"));
        objects.Add(Ascii("__PAGES__"));
        objects.Add(Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica >>"));
        objects.Add(Ascii("<< /Type /Font /Subtype /Type1 /BaseFont /Helvetica-Bold >>"));

        var logoObjectNumber = 0;
        if (logo is not null)
        {
            logoObjectNumber = objects.Count + 1;
            objects.Add(BuildJpegImageObject(logo));
        }

        foreach (var page in pages)
        {
            var pageObjectNumber = objects.Count + 1;
            var contentObjectNumber = pageObjectNumber + 1;
            pageObjectNumbers.Add(pageObjectNumber);
            var xObjectResources = logoObjectNumber > 0 ? $" /XObject << /ImLogo {logoObjectNumber.ToString(CultureInfo.InvariantCulture)} 0 R >>" : string.Empty;
            objects.Add(Ascii($"<< /Type /Page /Parent 2 0 R /MediaBox [0 0 612 792] /Resources << /Font << /F1 3 0 R /F2 4 0 R >>{xObjectResources} >> /Contents {contentObjectNumber.ToString(CultureInfo.InvariantCulture)} 0 R >>"));

            var stream = BuildPageStream(
                page.Lines,
                logoObjectNumber > 0,
                brandDisplayName);
            var streamBytes = Encoding.ASCII.GetBytes(stream);
            objects.Add(Ascii($"<< /Length {streamBytes.Length.ToString(CultureInfo.InvariantCulture)} >>\nstream\n{stream}\nendstream"));
        }

        var kids = string.Join(" ", pageObjectNumbers.Select(number => number.ToString(CultureInfo.InvariantCulture) + " 0 R"));
        objects[1] = Ascii($"<< /Type /Pages /Kids [{kids}] /Count {pageObjectNumbers.Count.ToString(CultureInfo.InvariantCulture)} >>");

        using var output = new MemoryStream();
        var offsets = new List<long> { 0 };
        WriteAscii(output, "%PDF-1.4\n");
        WriteAscii(output, "% 3DPIceland Branded PDF Renderer\n");

        for (var i = 0; i < objects.Count; i++)
        {
            offsets.Add(output.Position);
            WriteAscii(output, (i + 1).ToString(CultureInfo.InvariantCulture) + " 0 obj\n");
            output.Write(objects[i], 0, objects[i].Length);
            WriteAscii(output, "\nendobj\n");
        }

        var xrefOffset = output.Position;
        WriteAscii(output, "xref\n");
        WriteAscii(output, "0 " + (objects.Count + 1).ToString(CultureInfo.InvariantCulture) + "\n");
        WriteAscii(output, "0000000000 65535 f \n");
        foreach (var offset in offsets.Skip(1))
        {
            WriteAscii(output, offset.ToString("0000000000", CultureInfo.InvariantCulture) + " 00000 n \n");
        }
        WriteAscii(output, "trailer\n");
        WriteAscii(output, $"<< /Size {(objects.Count + 1).ToString(CultureInfo.InvariantCulture)} /Root 1 0 R >>\n");
        WriteAscii(output, "startxref\n");
        WriteAscii(output, xrefOffset.ToString(CultureInfo.InvariantCulture) + "\n");
        WriteAscii(output, "%%EOF\n");

        return output.ToArray();
    }

    private static byte[] BuildJpegImageObject(PdfLogoAsset logo)
    {
        using var ms = new MemoryStream();
        WriteAscii(ms, $"<< /Type /XObject /Subtype /Image /Width {logo.Width.ToString(CultureInfo.InvariantCulture)} /Height {logo.Height.ToString(CultureInfo.InvariantCulture)} /ColorSpace /DeviceRGB /BitsPerComponent 8 /Filter /DCTDecode /Length {logo.Bytes.Length.ToString(CultureInfo.InvariantCulture)} >>\nstream\n");
        ms.Write(logo.Bytes, 0, logo.Bytes.Length);
        WriteAscii(ms, "\nendstream");
        return ms.ToArray();
    }

    private static string BuildPageStream(
        IReadOnlyList<string> lines,
        bool includeLogo,
        string brandDisplayName)
    {
        var sb = new StringBuilder();

        // Website-inspired dark header, blue glow/accent bars, rounded-card-like sections.
        sb.AppendLine("0.02 0.06 0.12 rg 0 0 612 792 re f");
        sb.AppendLine("0.00 0.42 0.95 rg 0 642 612 150 re f");
        sb.AppendLine("0.04 0.10 0.18 rg 0 0 612 642 re f");
        sb.AppendLine("0.00 0.55 1.00 rg 0 637 612 5 re f");
        sb.AppendLine("0.08 0.16 0.28 rg 34 502 544 104 re f");
        sb.AppendLine("0.08 0.16 0.28 rg 34 250 544 232 re f");
        sb.AppendLine("0.00 0.48 1.00 rg 34 602 544 2 re f");
        sb.AppendLine("0.00 0.48 1.00 rg 34 478 544 2 re f");
        sb.AppendLine("0.80 0.90 1.00 rg");

        if (includeLogo)
        {
            sb.AppendLine("q 145 0 0 87 420 676 cm /ImLogo Do Q");
        }

        Text(sb, brandDisplayName, 42, 724, 22, true);
        Text(sb, "Material Engineering Report", 42, 698, 15, false);
        Text(sb, "Verified Engineering Platform output - no raw measurement consumption", 42, 676, 9, false);

        var safeLines = lines.Select(line => ToPdfAscii(line)).ToList();
        var title = safeLines.ElementAtOrDefault(2)?.Replace("Label: ", "", StringComparison.Ordinal) ?? "Material";
        var materialId = safeLines.ElementAtOrDefault(1) ?? "MaterialID: -";
        var manufacturer = safeLines.ElementAtOrDefault(3) ?? "Manufacturer: -";
        var baseMaterial = safeLines.ElementAtOrDefault(5) ?? "Base Material: -";

        Text(sb, title, 52, 574, 18, true);
        Text(sb, materialId, 52, 548, 11, false);
        Text(sb, manufacturer, 52, 530, 11, false);
        Text(sb, baseMaterial, 52, 512, 11, false);

        Text(sb, "Engineering Summary", 52, 452, 15, true);
        var y = 428;
        foreach (var line in safeLines.Skip(6).Take(22))
        {
            Text(sb, TrimForPdf(line, 92), 58, y, 9, false);
            y -= 14;
        }

        Text(sb, "Source of truth: SQLite + MaterialID + verified Material Summary", 42, 44, 8, false);
        Text(sb, "Generated with 3DPIceland Engineering Platform", 350, 44, 7, true);

        return sb.ToString();
    }

    private static void Text(StringBuilder sb, string text, int x, int y, int size, bool bold)
    {
        sb.AppendLine("BT");
        sb.AppendLine("0.86 0.93 1.00 rg");
        sb.AppendLine((bold ? "/F2 " : "/F1 ") + size.ToString(CultureInfo.InvariantCulture) + " Tf");
        sb.AppendLine(x.ToString(CultureInfo.InvariantCulture) + " " + y.ToString(CultureInfo.InvariantCulture) + " Td");
        sb.AppendLine("(" + EscapePdfText(text) + ") Tj");
        sb.AppendLine("ET");
    }

    private static string TrimForPdf(string value, int max)
    {
        return value.Length <= max ? value : value[..Math.Max(0, max - 3)] + "...";
    }

    private static string ToPdfAscii(string value)
    {
        var sb = new StringBuilder(value.Length);
        foreach (var ch in value)
        {
            sb.Append(ch <= 126 && ch >= 32 ? ch : '?');
        }
        return sb.ToString();
    }

    private static string EscapePdfText(string value)
    {
        return value.Replace("\\", "\\\\", StringComparison.Ordinal)
            .Replace("(", "\\(", StringComparison.Ordinal)
            .Replace(")", "\\)", StringComparison.Ordinal);
    }

    private static byte[] Ascii(string value) => Encoding.ASCII.GetBytes(value);

    private static void WriteAscii(Stream stream, string value)
    {
        var bytes = Encoding.ASCII.GetBytes(value);
        stream.Write(bytes, 0, bytes.Length);
    }

    private sealed record PdfLogoAsset(byte[] Bytes, int Width, int Height);
}

public sealed record ReportingPdfDocument(
    string FileName,
    string ContentType,
    byte[] Bytes,
    int PageCount,
    int MaterialReportsRendered,
    DateTime RenderedAtUtc,
    ReportingPdfPayload Payload);

public sealed record ReportingPdfPayload(
    IReadOnlyList<ReportingPdfPage> Pages);

public sealed record ReportingPdfPage(
    string MaterialId,
    IReadOnlyList<string> Lines);

public sealed class ReportingPdfRendererVerificationResult
{
    public bool Passed { get; set; }
    public int InputReports { get; set; }
    public int RenderedReports { get; set; }
    public int PageCount { get; set; }
    public int PayloadPages { get; set; }
    public int ByteCount { get; set; }
    public string ContentType { get; set; } = "";
    public bool HasPdfHeader { get; set; }
    public bool HasPdfTrailer { get; set; }
    public bool MaterialIdCoverage { get; set; }
    public bool PayloadValidationPassed { get; set; }
    public bool RenderReady { get; set; }
    public bool HasBrandedLayout { get; set; }
    public bool HasLogoAsset { get; set; }
    public string LogoAssetName { get; set; } = "";
}
