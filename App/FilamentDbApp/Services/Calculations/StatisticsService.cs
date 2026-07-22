using System.Globalization;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Calculations;

public sealed class StatisticsService : IStatisticsService
{
    private static readonly CultureInfo[] ParseCultures =
    [
        CultureInfo.InvariantCulture,
        CultureInfo.CurrentCulture,
        CultureInfo.GetCultureInfo("is-IS"),
        CultureInfo.GetCultureInfo("da-DK"),
        CultureInfo.GetCultureInfo("en-US")
    ];

    public double? ParseNullableDouble(string? value)
    {
        if (string.IsNullOrWhiteSpace(value)) return null;

        var cleaned = CleanNumericText(value);
        if (string.IsNullOrWhiteSpace(cleaned)) return null;

        if (cleaned.Contains(',') && !cleaned.Contains('.'))
        {
            var normalizedCommaDecimal = cleaned.Replace(',', '.');
            if (double.TryParse(normalizedCommaDecimal, NumberStyles.Float, CultureInfo.InvariantCulture, out var commaNumber) && IsUsableNumber(commaNumber))
            {
                return commaNumber;
            }
        }

        foreach (var culture in ParseCultures)
        {
            if (double.TryParse(cleaned, NumberStyles.Float, culture, out var number) && IsUsableNumber(number))
            {
                return number;
            }
        }

        return null;
    }

    public IReadOnlyList<double> GetNumericValues(IEnumerable<double?> values)
    {
        ArgumentNullException.ThrowIfNull(values);

        return values
            .Where(value => value.HasValue && IsUsableNumber(value.Value))
            .Select(value => value!.Value)
            .ToList();
    }

    public int CountNumeric(IEnumerable<double?> values)
    {
        return GetNumericValues(values).Count;
    }

    public double? Average(IEnumerable<double?> values)
    {
        var numericValues = GetNumericValues(values);
        return numericValues.Count == 0 ? null : numericValues.Average();
    }

    public double? StandardDeviationSample(IEnumerable<double?> values)
    {
        var numericValues = GetNumericValues(values);
        if (numericValues.Count < 2) return null;

        var average = numericValues.Average();
        var sumSquaredDifference = numericValues.Sum(value => Math.Pow(value - average, 2));
        return Math.Sqrt(sumSquaredDifference / (numericValues.Count - 1));
    }

    public double? CoefficientOfVariation(double? standardDeviation, double? average)
    {
        if (!standardDeviation.HasValue || !average.HasValue) return null;
        if (!IsUsableNumber(standardDeviation.Value) || !IsUsableNumber(average.Value)) return null;
        if (Math.Abs(average.Value) < double.Epsilon) return null;

        return standardDeviation.Value / average.Value;
    }

    public int? ConfidenceFromSampleCount(int sampleCount, int maximumConfidence = 10)
    {
        if (sampleCount <= 0) return null;
        return Math.Min(sampleCount, Math.Max(1, maximumConfidence));
    }

    public double? Percentage(double? numerator, double? denominator)
    {
        if (!numerator.HasValue || !denominator.HasValue) return null;
        if (!IsUsableNumber(numerator.Value) || !IsUsableNumber(denominator.Value)) return null;
        if (Math.Abs(denominator.Value) < double.Epsilon) return null;

        return numerator.Value / denominator.Value;
    }

    private static bool IsUsableNumber(double value)
    {
        return !double.IsNaN(value) && !double.IsInfinity(value);
    }

    private static string CleanNumericText(string value)
    {
        var cleaned = value.Trim()
            .Replace("MPa", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("kJ/m²", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("kJ/m2", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("mm²", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("mm2", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("m²", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("m2", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("%", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        // Keep only the first numeric token if a UI string contains text around the value.
        var match = Regex.Match(cleaned, @"[-+]?(?:\d+[\.,]?\d*|[\.,]\d+)(?:[eE][-+]?\d+)?");
        return match.Success ? match.Value : cleaned;
    }
}
