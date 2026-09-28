using FilamentDbApp.Services.Calculations;

namespace FilamentDbApp.Models;

public sealed class LegacyImpactRecalculationAudit
{
    public string CreatedAtUtc { get; set; } = DateTime.UtcNow.ToString("O");
    public string OwnerAttestation { get; set; } = "2026-09-28: owner confirmed prior legacy Impact readings used the same method and calibration: 2.743860924 J, 48.1603 mm², 105.411 degrees. This attestation does not authorize a different future calibration.";
    public string FormulaVersion { get; set; } = "legacy-impact-energy-area-v2";
    public string SourceFingerprint { get; set; } = "";
    public LegacyImpactSettings? Settings { get; set; }
    public List<string> CalibrationIssues { get; set; } = [];
    public List<Dictionary<string, string?>> CalibrationRows { get; set; } = [];
    public List<LegacyImpactRecalculationGroup> Groups { get; set; } = [];
    public int MaterialCount => Groups.Select(x => x.MaterialId).Where(x => x.Length > 0).Distinct().Count();
    public int ReadingCount => Groups.Sum(x => x.Readings.Count);
    public int ValidReadingCount => Groups.Sum(x => x.Readings.Count(r => r.Calculation is not null));
    public int BlankReadingCount => Groups.Sum(x => x.Readings.Count(r => string.IsNullOrWhiteSpace(r.Raw)));
    public int InvalidReadingCount => Groups.Sum(x => x.Readings.Count(r => !string.IsNullOrWhiteSpace(r.Raw) && r.Calculation is null));
    public int PendingGroups => Groups.Count(x => !x.Ready);
    public int NativeGroupsRecalculated => Groups.Count(x => x.Kind == "Native" && x.Ready);
    public int UpdatedExperimentalRows { get; set; }
    public string BackupPath { get; set; } = "";
    public string BackupSha256 { get; set; } = "";
    public string AuditPath { get; set; } = "";
    public string ApplyState { get; set; } = "Preview";
}

public sealed class LegacyImpactRecalculationGroup
{
    public string Kind { get; set; } = "";
    public string Id { get; set; } = "";
    public string MaterialId { get; set; } = "";
    public string Orientation { get; set; } = "";
    public List<LegacyImpactRecalculationReading> Readings { get; set; } = [];
    public Dictionary<string, string?> PersistedBefore { get; set; } = [];
    public bool Ready { get; set; }
    public string PendingReason { get; set; } = "";
    public LegacyImpactRecalculationSummary Before { get; set; } = new();
    public LegacyImpactRecalculationSummary After { get; set; } = new();
}

public sealed record LegacyImpactRecalculationSummary(double? Average = null, double? StdDev = null,
    double? CvPercent = null, int Count = 0, int? Confidence = null);

public sealed class LegacyImpactRecalculationReading
{
    public int SampleNumber { get; set; }
    public string? Raw { get; set; }
    public string PendingReason { get; set; } = "";
    public double? OldDisplayedKjM2 { get; set; }
    public LegacyImpactCalculationResult? Calculation { get; set; }
}
