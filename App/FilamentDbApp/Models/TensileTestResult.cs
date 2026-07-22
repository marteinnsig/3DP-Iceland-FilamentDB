using System.Globalization;

namespace FilamentDbApp.Models;

public sealed class TensileTestResult
{
    public string MaterialId { get; init; } = string.Empty;
    public string? UprightMpa { get; init; }
    public string? FlatMpa { get; init; }
    public string? StdDevUpright { get; init; }
    public string? StdDevFlat { get; init; }
    public string? CvUpright { get; init; }
    public string? CvFlat { get; init; }
    public string? SamplesUpright { get; init; }
    public string? SamplesFlat { get; init; }
    public string? ConfidenceUpright { get; init; }
    public string? ConfidenceFlat { get; init; }
    public string? TestNotes { get; init; }

    public string UprightDisplay => FormatMetric(UprightMpa, "MPa");
    public string FlatDisplay => FormatMetric(FlatMpa, "MPa");
    public string UprightSamplesDisplay => FormatSamples(SamplesUpright);
    public string FlatSamplesDisplay => FormatSamples(SamplesFlat);
    public string UprightCvDisplay => FormatMetric(CvUpright, "%");
    public string FlatCvDisplay => FormatMetric(CvFlat, "%");
    public string UprightStdDevDisplay => FormatMetric(StdDevUpright, "MPa");
    public string FlatStdDevDisplay => FormatMetric(StdDevFlat, "MPa");
    public string UprightConfidenceDisplay => FormatSamples(ConfidenceUpright);
    public string FlatConfidenceDisplay => FormatSamples(ConfidenceFlat);

    public bool HasAnyValue => !string.IsNullOrWhiteSpace(UprightMpa) || !string.IsNullOrWhiteSpace(FlatMpa) || !string.IsNullOrWhiteSpace(SamplesUpright) || !string.IsNullOrWhiteSpace(SamplesFlat);

    private static string FormatSamples(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? "—" : value;
    }

    private static string FormatMetric(string? value, string unit)
    {
        if (string.IsNullOrWhiteSpace(value)) return "—";

        // Excel data in the 3DP Iceland Labs workbook uses Icelandic/European decimal commas
        // in several calculated fields, for example "29,87" MPa.
        // Parsing that value with InvariantCulture treats the comma as a thousands separator
        // and turns it into 2987. Always try comma-decimal cultures and explicit comma
        // normalization before falling back to invariant parsing.
        if (unit == "%" && value.Contains('%')) return value;
        if (TryParseDecimalAware(value, out var number))
        {
            var formatted = unit == "%"
                ? (number * 100.0).ToString("0.0", CultureInfo.CurrentCulture)
                : number.ToString("0.##", CultureInfo.CurrentCulture);
            return string.IsNullOrWhiteSpace(unit) ? formatted : $"{formatted} {unit}";
        }

        return string.IsNullOrWhiteSpace(unit) ? value : $"{value} {unit}";
    }

    private static bool TryParseDecimalAware(string value, out double number)
    {
        number = 0;
        var cleaned = value
            .Trim()
            .Replace("MPa", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("kJ/m²", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("kJ/m2", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Replace("%", string.Empty, StringComparison.OrdinalIgnoreCase)
            .Trim();

        var cultures = new[]
        {
            CultureInfo.CurrentCulture,
            CultureInfo.GetCultureInfo("is-IS"),
            CultureInfo.GetCultureInfo("da-DK"),
            CultureInfo.GetCultureInfo("en-US")
        };

        foreach (var culture in cultures)
        {
            if (double.TryParse(cleaned, NumberStyles.Float, culture, out number))
            {
                return true;
            }
        }

        if (cleaned.Contains(',') && !cleaned.Contains('.'))
        {
            var normalized = cleaned.Replace(',', '.');
            if (double.TryParse(normalized, NumberStyles.Float, CultureInfo.InvariantCulture, out number))
            {
                return true;
            }
        }

        return double.TryParse(cleaned, NumberStyles.Float, CultureInfo.InvariantCulture, out number);
    }
}
