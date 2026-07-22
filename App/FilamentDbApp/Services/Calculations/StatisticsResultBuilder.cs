namespace FilamentDbApp.Services.Calculations;

public sealed class StatisticsResultBuilder
{
    private readonly IStatisticsService _statisticsService;

    public StatisticsResultBuilder(IStatisticsService? statisticsService = null)
    {
        _statisticsService = statisticsService ?? new StatisticsService();
    }

    public StatisticsResult Build(IEnumerable<double?> values)
    {
        var sampleCount = _statisticsService.CountNumeric(values);
        var average = _statisticsService.Average(values);
        var standardDeviation = _statisticsService.StandardDeviationSample(values);
        var coefficientOfVariation = _statisticsService.CoefficientOfVariation(standardDeviation, average);
        var confidence = _statisticsService.ConfidenceFromSampleCount(sampleCount);

        return new StatisticsResult(
            sampleCount,
            average,
            standardDeviation,
            coefficientOfVariation,
            confidence);
    }
}
