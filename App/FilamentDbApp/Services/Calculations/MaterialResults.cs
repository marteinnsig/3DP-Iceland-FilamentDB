namespace FilamentDbApp.Services.Calculations;

public sealed record MaterialResults(
    string MaterialId,
    TensileResults? Tensile,
    ImpactResults? Impact,
    StiffnessResults? Stiffness,
    DateTime CalculatedAtUtc)
{
    public bool HasTensileResults => HasMeasurementSet(Tensile?.Upright) || HasMeasurementSet(Tensile?.Flat);
    public bool HasImpactResults => HasMeasurementSet(Impact?.Upright) || HasMeasurementSet(Impact?.Flat);
    public bool HasStiffnessResults => Stiffness?.ModulusMpa.HasValue == true || Stiffness?.DeflectionMm.HasValue == true;
    public int ResultModuleCount => (HasTensileResults ? 1 : 0) + (HasImpactResults ? 1 : 0) + (HasStiffnessResults ? 1 : 0);
    public bool HasAnyResults => ResultModuleCount > 0;
    public bool IsCompleteEngineeringSummary => HasTensileResults && HasImpactResults && HasStiffnessResults;

    public double? BestTensileMpa => MaxNullable(Tensile?.Upright.Average, Tensile?.Flat.Average);
    public double? BestImpactKjM2 => MaxNullable(Impact?.Upright.Average, Impact?.Flat.Average);
    public double? StiffnessMpa => Stiffness?.ModulusMpa;

    public string SummaryStatus => IsCompleteEngineeringSummary
        ? "Complete"
        : HasAnyResults
            ? "Partial"
            : "No native results";

    private static bool HasMeasurementSet(MeasurementSetResult? result) => result?.SampleCount > 0;

    private static double? MaxNullable(double? first, double? second)
    {
        if (first.HasValue && second.HasValue) return Math.Max(first.Value, second.Value);
        return first ?? second;
    }
}
