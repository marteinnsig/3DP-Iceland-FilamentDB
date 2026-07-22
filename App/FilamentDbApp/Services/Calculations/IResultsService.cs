namespace FilamentDbApp.Services.Calculations;

public interface IResultsService
{
    TensileResults CalculateTensile(IEnumerable<string?> uprightSamples, IEnumerable<string?> flatSamples, double crossSectionAreaMm2);
    ImpactResults CalculateImpact(IEnumerable<string?> uprightNeedlePercentSamples, IEnumerable<string?> flatNeedlePercentSamples, double noSampleAngleDegrees, double netCrossSectionAreaM2, double maxPossibleImpact);
    StiffnessResults CalculateStiffness(string? revolutions, string? degrees, double mmPerRevolution, double spanLengthMm, double loadNewton, double secondMomentOfAreaMm4);
    MaterialResults CalculateMaterialResults(string materialId, TensileResults? tensile, ImpactResults? impact, StiffnessResults? stiffness);
}
