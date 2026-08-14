using System.Globalization;
using System.Windows.Data;
using FilamentDbApp.Services;

namespace FilamentDbApp;

public sealed class ApplicationDateConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture) =>
        ApplicationDateCodec.FormatForDisplay(value?.ToString(), CultureInfo.CurrentCulture);

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (ApplicationDateCodec.TryCanonicalizeUserInput(
                value?.ToString(),
                CultureInfo.CurrentCulture,
                out var canonicalValue))
        {
            return canonicalValue;
        }

        throw new FormatException(
            $"Enter a valid date using the Windows short-date format ({CultureInfo.CurrentCulture.DateTimeFormat.ShortDatePattern}) or yyyy-MM-dd.");
    }
}
