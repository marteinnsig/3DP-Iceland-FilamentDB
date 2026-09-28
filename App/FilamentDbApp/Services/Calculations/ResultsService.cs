namespace FilamentDbApp.Services.Calculations;

public sealed class ResultsService : IResultsService
{
    private readonly IStatisticsService _statisticsService;
    private readonly IRatingService _ratingService;

    public ResultsService(IStatisticsService? statisticsService = null, IRatingService? ratingService = null)
    {
        _statisticsService = statisticsService ?? new StatisticsService();
        _ratingService = ratingService ?? new RatingService(_statisticsService);
    }

    public TensileResults CalculateTensile(IEnumerable<string?> uprightSamples, IEnumerable<string?> flatSamples, double crossSectionAreaMm2)
    {
        if (crossSectionAreaMm2 <= 0) crossSectionAreaMm2 = 0;

        var uprightRaw = ParseSamples(uprightSamples);
        var flatRaw = ParseSamples(flatSamples);
        var uprightMpa = crossSectionAreaMm2 > 0 ? uprightRaw.Select(v => v / crossSectionAreaMm2) : Enumerable.Empty<double>();
        var flatMpa = crossSectionAreaMm2 > 0 ? flatRaw.Select(v => v / crossSectionAreaMm2) : Enumerable.Empty<double>();

        return new TensileResults(
            BuildMeasurementSet(uprightMpa),
            BuildMeasurementSet(flatMpa),
            crossSectionAreaMm2,
            DateTime.UtcNow);
    }

    public ImpactResults CalculateImpact(IEnumerable<string?> uprightNeedlePercentSamples, IEnumerable<string?> flatNeedlePercentSamples, double angleDegrees, double netAreaMm2, double availableEnergyJ)
    {
        var settings = new LegacyImpactSettings(availableEnergyJ, netAreaMm2, angleDegrees);
        var uprightKj = ParseSamples(uprightNeedlePercentSamples)
            .Select(percent => LegacyImpactCalculationService.TryCalculate(percent, settings)?.ImpactKjM2)
            .Where(value => value.HasValue)
            .Select(value => value!.Value);

        var flatKj = ParseSamples(flatNeedlePercentSamples)
            .Select(percent => LegacyImpactCalculationService.TryCalculate(percent, settings)?.ImpactKjM2)
            .Where(value => value.HasValue)
            .Select(value => value!.Value);

        return new ImpactResults(
            BuildMeasurementSet(uprightKj),
            BuildMeasurementSet(flatKj),
            double.IsFinite(angleDegrees) && angleDegrees > 0 ? angleDegrees : null,
            double.IsFinite(netAreaMm2) && netAreaMm2 > 0 ? netAreaMm2 / 1_000_000d : null,
            LegacyImpactCalculationService.TryCalculate(100d, settings)?.ImpactKjM2,
            DateTime.UtcNow)
        {
            AvailableEnergyJ = double.IsFinite(availableEnergyJ) && availableEnergyJ > 0 ? availableEnergyJ : null
        };
    }

    public StiffnessResults CalculateStiffness(string? revolutions, string? degrees, double mmPerRevolution, double spanLengthMm, double loadNewton, double secondMomentOfAreaMm4)
    {
        var parsedRevolutions = _statisticsService.ParseNullableDouble(revolutions);
        var parsedDegrees = _statisticsService.ParseNullableDouble(degrees);

        double? deflection = null;
        double? modulus = null;
        var sampleCount = 0;

        if (parsedRevolutions.HasValue || parsedDegrees.HasValue) sampleCount = 1;

        if ((parsedRevolutions.HasValue || parsedDegrees.HasValue) && mmPerRevolution > 0)
        {
            deflection = (parsedRevolutions.GetValueOrDefault() + parsedDegrees.GetValueOrDefault() / 360d) * mmPerRevolution;
            if (deflection <= 0) deflection = null;
        }

        if (deflection.HasValue && spanLengthMm > 0 && loadNewton > 0 && secondMomentOfAreaMm4 > 0)
        {
            modulus = (loadNewton * Math.Pow(spanLengthMm, 3)) / (48d * deflection.Value * secondMomentOfAreaMm4);
        }

        return new StiffnessResults(
            deflection,
            modulus,
            _ratingService.BuildCompletenessRating(sampleCount, 1),
            DateTime.UtcNow);
    }

    public MaterialResults CalculateMaterialResults(string materialId, TensileResults? tensile, ImpactResults? impact, StiffnessResults? stiffness)
    {
        return new MaterialResults(materialId, tensile, impact, stiffness, DateTime.UtcNow);
    }

    private MeasurementSetResult BuildMeasurementSet(IEnumerable<double> values)
    {
        var numericValues = values.Where(IsUsableNumber).ToList();
        var nullableValues = numericValues.Select(value => (double?)value).ToList();
        var sampleCount = numericValues.Count;
        var average = _statisticsService.Average(nullableValues);
        var standardDeviation = _statisticsService.StandardDeviationSample(nullableValues);
        var coefficientOfVariation = _statisticsService.CoefficientOfVariation(standardDeviation, average);
        var confidence = _statisticsService.ConfidenceFromSampleCount(sampleCount);

        return new MeasurementSetResult(
            average,
            standardDeviation,
            coefficientOfVariation,
            sampleCount,
            confidence,
            _ratingService.BuildCompletenessRating(sampleCount));
    }

    private IReadOnlyList<double> ParseSamples(IEnumerable<string?> values)
    {
        return values
            .Select(value => _statisticsService.ParseNullableDouble(value))
            .Where(value => value.HasValue)
            .Select(value => value!.Value)
            .Where(IsUsableNumber)
            .ToList();
    }

    private static bool IsUsableNumber(double value) => !double.IsNaN(value) && !double.IsInfinity(value);
}
