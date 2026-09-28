namespace FilamentDbApp.Models;

public sealed class EngineeringScoreProfile
{
    // Shared app and website profile. ImpactScore is retained for historical compatibility.
    // Overall uses five families; the separate Izod/Charpy axes share one impact family.
    // Thermal remains an independent decision axis outside Overall.
    public double? TensileScore { get; init; }
    public double? ImpactScore { get; init; }
    public double? LegacyImpactRadarPercent { get; init; }
    public string LegacyImpactRadarSource => Services.Calculations.LegacyImpactRadarService.Description;
    public double? IzodScore { get; init; }
    public double? CharpyScore { get; init; }
    public double? StiffnessScore { get; init; }
    public double? ConsistencyScore { get; init; }
    public double? LayerAdhesionScore { get; init; }
    public double? ThermalScore { get; init; }
    public double? ThermalResultTemperatureC { get; init; }
    public double? OverallScore { get; init; }
    public double? ImpactFamilyScore { get; init; }
    public int MeasuredImpactMethodCount { get; init; }
    public int ScoredImpactMethodCount => (IzodScore.HasValue ? 1 : 0) + (CharpyScore.HasValue ? 1 : 0);
    public int ScoredComponentCount => new[] { TensileScore, StiffnessScore, ConsistencyScore, LayerAdhesionScore, ImpactFamilyScore }.Count(value => value.HasValue);
    public bool IsOverallComparable => ScoredComponentCount == 5 && ScoredImpactMethodCount == 2 && OverallScore.HasValue;
    public string ScorePolicyVersion => Services.EngineeringScoringService.PolicyVersion;
    public string CoverageSummary => $"{ScoredComponentCount}/5 score families; Izod/Charpy {MeasuredImpactMethodCount}/2 measured, {ScoredImpactMethodCount}/2 scored; " +
        (IsOverallComparable ? "Overall eligible" : "Overall unavailable: missing measurements or references");

    public string TensileSource { get; init; } = "Average of flat/upright tensile MPa";
    public string IzodSource { get; init; } = "Direct Izod kJ/m² / fixed versioned method reference; unavailable until configured";
    public string CharpySource { get; init; } = "Direct Charpy kJ/m² / fixed versioned method reference; unavailable until configured";
    public string ImpactSource { get; init; } = "Legacy Impact: historical raw comparison only; excluded from modern scores";
    public string StiffnessSource { get; init; } = "Stiffness modulus MPa";
    public string ConsistencySource { get; init; } = "3DPIceland internal repeatability scale: 100 - average CV% - sample-count penalty";
    public string LayerAdhesionSource { get; init; } = "Upright tensile / flat tensile ratio";
    public string ThermalSource { get; init; } = "Fixture-specific probe temperature / fixed 200 °C reference";
}
