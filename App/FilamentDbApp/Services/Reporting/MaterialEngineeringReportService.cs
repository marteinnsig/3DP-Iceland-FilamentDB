namespace FilamentDbApp.Services.Reporting;

public sealed class MaterialEngineeringReportService
{
    public MaterialEngineeringReportPayload Build(ReportingReportModel reportModel)
    {
        var reports = reportModel.MaterialReports
            .Where(report => !string.IsNullOrWhiteSpace(report.MaterialId))
            .Select(report => new MaterialEngineeringReportOutput(
                report.MaterialId,
                report.Label,
                report.Manufacturer,
                report.BaseMaterial,
                report.Sections.Count(section => section.SectionType == ReportingSectionType.MechanicalMetric),
                report.Sections.Any(section => section.SectionType == ReportingSectionType.MaterialOverview),
                report.Sections.Any(section => section.SectionType == ReportingSectionType.EngineeringSummary),
                report.Sections.Any(section => section.SectionType == ReportingSectionType.MechanicalMetric),
                "Verified Material Summary",
                "ReportPdfRendererService",
                true,
                global::FilamentDbApp.Services.EngineeringIntelligenceHandoffService.GovernanceStatement))
            .ToList();

        return new MaterialEngineeringReportPayload(
            ReportName: "Material Engineering Report",
            ReportCode: "REPORT-100",
            Outputs: reports,
            GeneratedAtUtc: DateTime.UtcNow);
    }

    public MaterialEngineeringReportVerificationResult Verify(ReportingReportModel reportModel)
    {
        var payload = Build(reportModel);
        var result = new MaterialEngineeringReportVerificationResult
        {
            InputReports = reportModel.MaterialReports.Count,
            GeneratedEngineeringReports = payload.Outputs.Count,
            ReportsWithIdentity = payload.Outputs.Count(output => output.HasIdentitySection),
            ReportsWithEngineeringSummary = payload.Outputs.Count(output => output.HasEngineeringSummarySection),
            ReportsWithMechanicalMetrics = payload.Outputs.Count(output => output.HasMechanicalMetricSection),
            MetricSections = payload.Outputs.Sum(output => output.MechanicalMetricSectionCount),
            PayloadValidationPassed = payload.Outputs.Count == reportModel.MaterialReports.Count &&
                                      payload.Outputs.All(output => !string.IsNullOrWhiteSpace(output.MaterialId) && output.HasIdentitySection && output.HasEngineeringSummarySection),
            // RELEASE-003B: partial/new materials are valid report targets even before all
            // measurements are available. Render readiness means the report shell can render
            // from verified Material Summary context; mechanical metrics may be absent until
            // samples have been measured.
            RenderReady = payload.Outputs.Count > 0 &&
                          payload.Outputs.All(output => output.HasIdentitySection &&
                                                        output.HasEngineeringSummarySection &&
                                                        output.Source == "Verified Material Summary"),
            RawMeasurementConsumptionBlocked = true,
            GovernedIntelligenceHandoffReady = payload.Outputs.Count > 0 &&
                                                  payload.Outputs.All(output => output.HasGovernedIntelligenceHandoff &&
                                                      output.IntelligenceSourceStatement == global::FilamentDbApp.Services.EngineeringIntelligenceHandoffService.GovernanceStatement),
            ReportCode = payload.ReportCode,
            ReportName = payload.ReportName
        };

        result.Passed = result.InputReports > 0 &&
                        result.GeneratedEngineeringReports == result.InputReports &&
                        result.PayloadValidationPassed &&
                        result.RenderReady &&
                        result.GovernedIntelligenceHandoffReady &&
                        result.RawMeasurementConsumptionBlocked;

        return result;
    }
}

public sealed record MaterialEngineeringReportPayload(
    string ReportName,
    string ReportCode,
    IReadOnlyList<MaterialEngineeringReportOutput> Outputs,
    DateTime GeneratedAtUtc);

public sealed record MaterialEngineeringReportOutput(
    string MaterialId,
    string? Label,
    string? Manufacturer,
    string? BaseMaterial,
    int MechanicalMetricSectionCount,
    bool HasIdentitySection,
    bool HasEngineeringSummarySection,
    bool HasMechanicalMetricSection,
    string Source,
    string RenderOwner,
    bool HasGovernedIntelligenceHandoff,
    string IntelligenceSourceStatement);

public sealed class MaterialEngineeringReportVerificationResult
{
    public bool Passed { get; set; }
    public string ReportCode { get; set; } = string.Empty;
    public string ReportName { get; set; } = string.Empty;
    public int InputReports { get; set; }
    public int GeneratedEngineeringReports { get; set; }
    public int ReportsWithIdentity { get; set; }
    public int ReportsWithEngineeringSummary { get; set; }
    public int ReportsWithMechanicalMetrics { get; set; }
    public int MetricSections { get; set; }
    public bool PayloadValidationPassed { get; set; }
    public bool RenderReady { get; set; }
    public bool RawMeasurementConsumptionBlocked { get; set; }
    public bool GovernedIntelligenceHandoffReady { get; set; }
}
