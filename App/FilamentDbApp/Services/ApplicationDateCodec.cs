using System.Globalization;

namespace FilamentDbApp.Services;

public static class ApplicationDateCodec
{
    public const string CanonicalFormat = "yyyy-MM-dd";

    private static readonly string[] LegacyIcelandicFormats = ["d.M.yyyy", "dd.MM.yyyy"];

    public static string FormatForDisplay(string? storedValue, CultureInfo? culture = null)
    {
        if (string.IsNullOrWhiteSpace(storedValue)) return string.Empty;
        return TryParseStored(storedValue, out var date)
            ? date.ToString("d", culture ?? CultureInfo.CurrentCulture)
            : storedValue;
    }

    public static string FormatForDisplay(DateTime? value, CultureInfo? culture = null) =>
        value?.Date.ToString("d", culture ?? CultureInfo.CurrentCulture) ?? string.Empty;

    public static bool TryCanonicalizeUserInput(
        string? input,
        CultureInfo? culture,
        out string canonicalValue)
    {
        canonicalValue = string.Empty;
        if (string.IsNullOrWhiteSpace(input)) return true;

        var trimmed = input.Trim();
        if ((DateTime.TryParseExact(
                trimmed,
                CanonicalFormat,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out var iso) ||
            DateTime.TryParse(
                trimmed,
                culture ?? CultureInfo.CurrentCulture,
                DateTimeStyles.AllowWhiteSpaces,
                out iso)) &&
            iso.Year >= 1900)
        {
            canonicalValue = iso.Date.ToString(CanonicalFormat, CultureInfo.InvariantCulture);
            return true;
        }

        return false;
    }

    public static bool TryParseStored(string? storedValue, out DateTime date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(storedValue)) return false;
        var trimmed = storedValue.Trim();
        return DateTime.TryParseExact(
                   trimmed,
                   CanonicalFormat,
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out date) ||
               DateTime.TryParseExact(
                   trimmed,
                   LegacyIcelandicFormats,
                   CultureInfo.InvariantCulture,
                   DateTimeStyles.None,
                   out date);
    }
}
