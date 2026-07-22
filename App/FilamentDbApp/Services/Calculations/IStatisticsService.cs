namespace FilamentDbApp.Services.Calculations;

public interface IStatisticsService
{
    double? ParseNullableDouble(string? value);
    IReadOnlyList<double> GetNumericValues(IEnumerable<double?> values);
    int CountNumeric(IEnumerable<double?> values);
    double? Average(IEnumerable<double?> values);
    double? StandardDeviationSample(IEnumerable<double?> values);
    double? CoefficientOfVariation(double? standardDeviation, double? average);
    int? ConfidenceFromSampleCount(int sampleCount, int maximumConfidence = 10);
    double? Percentage(double? numerator, double? denominator);
}
