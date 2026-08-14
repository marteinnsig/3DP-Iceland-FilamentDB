namespace FilamentDbApp.Models;

public sealed class EngineeringScoreProfile
{
    // Mirrors the existing website radar profile axes:
    // Tensile, Impact, Stiffness, Consistency, Layer Adhesion. Thermal is an
    // independent sixth decision axis and intentionally does not alter Overall.
    public double? TensileScore { get; init; }
    public double? ImpactScore { get; init; }
    public double? StiffnessScore { get; init; }
    public double? ConsistencyScore { get; init; }
    public double? LayerAdhesionScore { get; init; }
    public double? ThermalScore { get; init; }
    public double? ThermalResultTemperatureC { get; init; }
    public double? OverallScore { get; init; }

    public string TensileSource { get; init; } = "Average of flat/upright tensile MPa";
    public string ImpactSource { get; init; } = "Average of flat/upright impact kJ/m²";
    public string StiffnessSource { get; init; } = "Stiffness modulus MPa";
    public string ConsistencySource { get; init; } = "3DPIceland internal repeatability scale: 100 - average CV% - sample-count penalty";
    public string LayerAdhesionSource { get; init; } = "Upright tensile / flat tensile ratio";
    public string ThermalSource { get; init; } = "Fixture-specific probe temperature / fixed 200 °C reference";
}
