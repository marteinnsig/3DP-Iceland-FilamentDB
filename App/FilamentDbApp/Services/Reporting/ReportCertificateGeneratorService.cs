namespace FilamentDbApp.Services.Reporting;

public sealed class ReportCertificateGeneratorService
{
    public ReportingCertificateBatch BuildCertificates(ReportingReportModel reportModel, ReportingPdfDocument pdfDocument)
    {
        var generatedAtUtc = DateTime.UtcNow;
        var pdfReady = pdfDocument.ContentType == "application/pdf" &&
                       pdfDocument.Bytes.Length > 0 &&
                       pdfDocument.PageCount == reportModel.MaterialReports.Count;

        var certificates = reportModel.MaterialReports
            .Where(report => !string.IsNullOrWhiteSpace(report.MaterialId))
            .Select(report => BuildCertificate(report, pdfDocument, pdfReady, generatedAtUtc))
            .ToList();

        return new ReportingCertificateBatch(
            Certificates: certificates,
            GeneratedAtUtc: generatedAtUtc,
            SourceReportCount: reportModel.MaterialReports.Count,
            SourcePdfPageCount: pdfDocument.PageCount,
            SourcePdfByteCount: pdfDocument.Bytes.Length,
            SourcePdfContentType: pdfDocument.ContentType);
    }

    public ReportingCertificateGeneratorVerificationResult Verify(ReportingReportModel reportModel, ReportingPdfDocument pdfDocument)
    {
        var batch = BuildCertificates(reportModel, pdfDocument);
        var materialIds = batch.Certificates
            .Select(certificate => certificate.MaterialId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        var result = new ReportingCertificateGeneratorVerificationResult
        {
            InputReports = reportModel.MaterialReports.Count,
            GeneratedCertificates = batch.Certificates.Count,
            SourcePdfPages = batch.SourcePdfPageCount,
            SourcePdfBytes = batch.SourcePdfByteCount,
            MaterialIdCoverage = materialIds.Count == reportModel.MaterialReports.Count && materialIds.Count == materialIds.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            PayloadValidationPassed = batch.Certificates.Count == reportModel.MaterialReports.Count &&
                                      batch.Certificates.All(certificate => certificate.Fields.Count >= 6 && certificate.SourcePdfReady),
            CertificateReady = batch.SourcePdfContentType == "application/pdf" &&
                               batch.SourcePdfByteCount > 0 &&
                               batch.SourcePdfPageCount == reportModel.MaterialReports.Count &&
                               batch.Certificates.All(certificate => certificate.ReadyForIssue)
        };

        result.Passed = result.InputReports > 0 &&
                        result.GeneratedCertificates == result.InputReports &&
                        result.SourcePdfPages == result.InputReports &&
                        result.SourcePdfBytes > 0 &&
                        result.MaterialIdCoverage &&
                        result.PayloadValidationPassed &&
                        result.CertificateReady;

        return result;
    }

    private static ReportingCertificateModel BuildCertificate(ReportingMaterialReportModel report, ReportingPdfDocument pdfDocument, bool pdfReady, DateTime generatedAtUtc)
    {
        var overview = report.Sections.FirstOrDefault(section => section.SectionType == ReportingSectionType.MaterialOverview);
        var engineering = report.Sections.FirstOrDefault(section => section.SectionType == ReportingSectionType.EngineeringSummary);

        var fields = new Dictionary<string, string?>
        {
            ["MaterialID"] = report.MaterialId,
            ["Label"] = report.Label,
            ["Manufacturer"] = report.Manufacturer,
            ["ProductLine"] = report.ProductLine,
            ["BaseMaterial"] = report.BaseMaterial,
            ["Source"] = "Verified Material Summary",
            ["CalculationOwner"] = "Engineering Platform",
            ["ReportModelOwner"] = "ReportGeneratorService",
            ["PdfRendererOwner"] = "ReportPdfRendererService",
            ["CertificateOwner"] = "ReportCertificateGeneratorService",
            ["PdfFileName"] = pdfDocument.FileName,
            ["PdfContentType"] = pdfDocument.ContentType,
            ["PdfPages"] = pdfDocument.PageCount.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["PdfBytes"] = pdfDocument.Bytes.Length.ToString(System.Globalization.CultureInfo.InvariantCulture),
            ["SummaryStatus"] = engineering?.Fields.TryGetValue("SummaryStatus", out var status) == true ? status : "Unknown",
            ["GeneratedAtUtc"] = generatedAtUtc.ToString("O", System.Globalization.CultureInfo.InvariantCulture)
        };

        if (overview is not null)
        {
            foreach (var field in overview.Fields)
            {
                fields.TryAdd("Overview." + field.Key, field.Value);
            }
        }

        return new ReportingCertificateModel(
            MaterialId: report.MaterialId,
            Title: "3DPIceland Labs Material Verification Certificate",
            Fields: fields,
            SourcePdfReady: pdfReady,
            ReadyForIssue: pdfReady && report.Sections.Any(section => section.SectionType == ReportingSectionType.EngineeringSummary));
    }
}

public sealed record ReportingCertificateBatch(
    IReadOnlyList<ReportingCertificateModel> Certificates,
    DateTime GeneratedAtUtc,
    int SourceReportCount,
    int SourcePdfPageCount,
    int SourcePdfByteCount,
    string SourcePdfContentType);

public sealed record ReportingCertificateModel(
    string MaterialId,
    string Title,
    IReadOnlyDictionary<string, string?> Fields,
    bool SourcePdfReady,
    bool ReadyForIssue);

public sealed class ReportingCertificateGeneratorVerificationResult
{
    public bool Passed { get; set; }
    public int InputReports { get; set; }
    public int GeneratedCertificates { get; set; }
    public int SourcePdfPages { get; set; }
    public int SourcePdfBytes { get; set; }
    public bool MaterialIdCoverage { get; set; }
    public bool PayloadValidationPassed { get; set; }
    public bool CertificateReady { get; set; }
}
