using FilamentDbApp.Models;

namespace FilamentDbApp.Services.Reporting;

public sealed class ReportGeneratorService
{
    public ReportingReportModel BuildReport(ReportingDataPipelinePayload payload)
    {
        var reports = payload.Rows
            .Where(row => !string.IsNullOrWhiteSpace(row.MaterialId))
            .Select(row => new ReportingMaterialReportModel(
                row.MaterialId,
                row.Label,
                row.Manufacturer,
                row.ProductLine,
                row.BaseMaterial,
                BuildSections(row))
            {
                FlexibleResults = row.FlexibleResults.ToArray()
            })
            .ToList();

        return new ReportingReportModel(reports);
    }

    public ReportingReportGeneratorVerificationResult Verify(ReportingDataPipelinePayload payload)
    {
        var report = BuildReport(payload);
        var materialIds = report.MaterialReports
            .Select(row => row.MaterialId)
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .ToList();

        var result = new ReportingReportGeneratorVerificationResult
        {
            InputRows = payload.Rows.Count,
            GeneratedReports = report.MaterialReports.Count,
            MaterialOverviewSections = report.MaterialReports.Count(row => row.Sections.Any(section => section.SectionType == ReportingSectionType.MaterialOverview)),
            EngineeringSummarySections = report.MaterialReports.Count(row => row.Sections.Any(section => section.SectionType == ReportingSectionType.EngineeringSummary)),
            MetricSections = report.MaterialReports.Sum(row => row.Sections.Count(section => section.SectionType == ReportingSectionType.MechanicalMetric)),
            MaterialIdCoverage = payload.Rows.Count > 0 && materialIds.Count == payload.Rows.Count,
            NoDuplicateMaterialIds = materialIds.Count == materialIds.Distinct(StringComparer.OrdinalIgnoreCase).Count(),
            HasReportSections = report.MaterialReports.All(row => row.Sections.Count >= 2)
        };

        result.Passed = result.InputRows > 0 &&
                        result.GeneratedReports == result.InputRows &&
                        result.MaterialOverviewSections == result.GeneratedReports &&
                        result.EngineeringSummarySections == result.GeneratedReports &&
                        result.MetricSections > 0 &&
                        result.MaterialIdCoverage &&
                        result.NoDuplicateMaterialIds &&
                        result.HasReportSections;

        return result;
    }

    public static bool VerifyFlexibleEvidenceContract(IReadOnlyList<PublicFlexibleMetricGroup> groups)
    {
        if (groups.Count == 0) return false;
        var input = new ReportingMaterialInput("FLEX-REPORT-CONTRACT", new Dictionary<string, object?>(),
            new global::FilamentDbApp.Services.Calculations.MaterialResults("FLEX-REPORT-CONTRACT", null, null, null, DateTime.UnixEpoch))
        {
            FlexibleResults = groups
        };
        var payload = new ReportingDataPipelineService().BuildPayload(new[] { input });
        var report = new ReportGeneratorService().BuildReport(payload);
        var material = report.MaterialReports.Single();
        var engineering = new MaterialEngineeringReportService().Build(report).Outputs.Single();
        var continuationReport = new ReportingReportModel(new[]
        {
            material with
            {
                Sections = material.Sections.Concat(new[]
                {
                    new ReportingReportSection("Continuation fixture", ReportingSectionType.FlexibleMetrics,
                        new Dictionary<string, string?> { ["Results"] = string.Join("\n", Enumerable.Range(0, 80).Select(i => "Condition " + i)) })
                }).ToArray()
            }
        });
        var continuationPdf = new ReportPdfRendererService().Render(continuationReport);
        var certificateService = new ReportCertificateGeneratorService();
        var certificates = certificateService.BuildCertificates(continuationReport, continuationPdf);
        return continuationPdf.PageCount > 1 && continuationPdf.MaterialReportsRendered == 1 &&
            certificates.Certificates.Count == 1 && certificateService.Verify(continuationReport, continuationPdf).Passed &&
            !certificateService.Verify(continuationReport, continuationPdf with { MaterialReportsRendered = 0 }).Passed &&
            !certificateService.Verify(continuationReport, continuationPdf with
            {
                Payload = new ReportingPdfPayload(continuationPdf.Payload.Pages.Select(page => page with { MaterialId = "WRONG-MATERIAL" }).ToArray())
            }).Passed &&
            !payload.Rows.Single().IsComplete &&
            payload.Rows.Single().FlexibleResults.SequenceEqual(groups) &&
            material.FlexibleResults.SequenceEqual(groups) &&
            material.Sections.Single(section => section.SectionType == ReportingSectionType.FlexibleMetrics).Fields["Results"] ==
                FlexibleReportEvidenceService.RenderText(groups) &&
            !engineering.HasMechanicalMetricSection && engineering.FlexibleMetricGroupCount == groups.Count &&
            new ReportTemplateService().BuildTemplates(report).Outputs.All(output => output.ReadyForRender) &&
            ReportPdfRendererService.VerifyContinuationContract();
    }
    private static IReadOnlyList<ReportingReportSection> BuildSections(ReportingMaterialSummaryRow row)
    {
        var sections = new List<ReportingReportSection>
        {
            new("Material Overview", ReportingSectionType.MaterialOverview, new Dictionary<string, string?>
            {
                ["MaterialID"] = row.MaterialId,
                ["Label"] = row.Label,
                ["Manufacturer"] = row.Manufacturer,
                ["ProductLine"] = row.ProductLine,
                ["BaseMaterial"] = row.BaseMaterial,
                ["Reinforcement"] = row.Reinforcement,
                ["Color"] = row.Color
            }),
            new("Engineering Summary", ReportingSectionType.EngineeringSummary, new Dictionary<string, string?>
            {
                ["SummaryStatus"] = row.IsComplete ? "Complete" : "Partial",
                ["DataSource"] = "Verified Material Summary",
                ["CalculationOwnership"] = "Engineering Platform"
            })
        };

        AddMetricSection(sections, "Tensile Flat", "MPa", row.TensileFlatMpa);
        AddMetricSection(sections, "Tensile Upright", "MPa", row.TensileUprightMpa);
        AddMetricSection(sections, "Impact Flat", "kJ/m²", row.ImpactFlatKjM2);
        AddMetricSection(sections, "Impact Upright", "kJ/m²", row.ImpactUprightKjM2);
        AddMetricSection(sections, "Stiffness", "MPa", row.StiffnessMpa);

        if (row.FlexibleResults.Count > 0)
        {
            sections.Add(new ReportingReportSection("Flexible Material Testing", ReportingSectionType.FlexibleMetrics,
                new Dictionary<string, string?>
                {
                    ["Results"] = FlexibleReportEvidenceService.RenderText(row.FlexibleResults)
                }));
        }

        return sections;
    }

    private static void AddMetricSection(List<ReportingReportSection> sections, string metricName, string unit, double? value)
    {
        if (!value.HasValue) return;

        sections.Add(new ReportingReportSection(metricName, ReportingSectionType.MechanicalMetric, new Dictionary<string, string?>
        {
            ["Metric"] = metricName,
            ["Value"] = value.Value.ToString("0.###", System.Globalization.CultureInfo.InvariantCulture),
            ["Unit"] = unit,
            ["Source"] = "Verified Material Summary"
        }));
    }
}

public sealed record ReportingReportModel(
    IReadOnlyList<ReportingMaterialReportModel> MaterialReports);

public sealed record ReportingMaterialReportModel(
    string MaterialId,
    string? Label,
    string? Manufacturer,
    string? ProductLine,
    string? BaseMaterial,
    IReadOnlyList<ReportingReportSection> Sections)
{
    public IReadOnlyList<PublicFlexibleMetricGroup> FlexibleResults { get; init; } = Array.Empty<PublicFlexibleMetricGroup>();
}

public sealed record ReportingReportSection(
    string Title,
    ReportingSectionType SectionType,
    IReadOnlyDictionary<string, string?> Fields);

public enum ReportingSectionType
{
    MaterialOverview,
    EngineeringSummary,
    MechanicalMetric,
    FlexibleMetrics
}

public sealed class ReportingReportGeneratorVerificationResult
{
    public bool Passed { get; set; }
    public int InputRows { get; set; }
    public int GeneratedReports { get; set; }
    public int MaterialOverviewSections { get; set; }
    public int EngineeringSummarySections { get; set; }
    public int MetricSections { get; set; }
    public bool MaterialIdCoverage { get; set; }
    public bool NoDuplicateMaterialIds { get; set; }
    public bool HasReportSections { get; set; }
}
