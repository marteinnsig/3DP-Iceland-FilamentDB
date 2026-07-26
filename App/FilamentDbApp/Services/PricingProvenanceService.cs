using System.Globalization;

namespace FilamentDbApp.Services;

public sealed class PricingProvenanceService
{
    private static readonly HashSet<string> SupportedCurrencies =
        new(StringComparer.OrdinalIgnoreCase) { "USD", "ISK", "EUR", "GBP" };

    public double? ResolveCanonicalMsrpUsdPerKg(
        bool canonicalMaterialExists,
        string? canonicalMsrpUsdPerKg,
        string? legacyMsrpUsdPerKg)
    {
        if (canonicalMaterialExists)
            return ParsePositiveDouble(canonicalMsrpUsdPerKg);

        return ParsePositiveDouble(legacyMsrpUsdPerKg);
    }

    public PricingConversionResult ConvertToUsd(
        string? amountText,
        string? currencyText,
        IReadOnlyDictionary<string, decimal> currencyUnitsPerUsd)
    {
        if (!TryParseNonNegativeDecimal(amountText, out var amount))
            return PricingConversionResult.NotRecorded("Amount is missing or invalid.");

        var currency = NormalizeCurrency(currencyText);
        if (currency is null)
            return PricingConversionResult.NotRecorded("Currency is missing or unsupported.");

        if (currency == "USD")
            return PricingConversionResult.Available(amount, currency, 1m);

        if (!currencyUnitsPerUsd.TryGetValue(currency, out var rate) || rate <= 0m)
            return PricingConversionResult.NotRecorded($"No valid {currency} per 1 USD rate is configured.");

        return PricingConversionResult.Available(amount / rate, currency, rate);
    }

    public static string NormalizeCurrencyForStorage(string? currencyText) =>
        (currencyText ?? string.Empty).Trim().ToUpperInvariant();

    public static bool IsSupportedCurrency(string? currencyText) =>
        NormalizeCurrency(currencyText) is not null;

    private static string? NormalizeCurrency(string? currencyText)
    {
        var currency = NormalizeCurrencyForStorage(currencyText);
        return SupportedCurrencies.Contains(currency) ? currency : null;
    }

    private static bool TryParseNonNegativeDecimal(string? text, out decimal value) =>
        decimal.TryParse(
            (text ?? string.Empty).Replace(',', '.'),
            NumberStyles.Number,
            CultureInfo.InvariantCulture,
            out value) &&
        value >= 0m;

    private static double? ParsePositiveDouble(string? text)
    {
        if (!double.TryParse(
                (text ?? string.Empty).Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var value) ||
            !double.IsFinite(value) ||
            value <= 0)
            return null;

        return value;
    }
}

public sealed record PricingConversionResult(
    bool HasValue,
    string UsdAmount,
    string SourceCurrency,
    decimal? CurrencyUnitsPerUsd,
    string MissingReason)
{
    public static PricingConversionResult Available(decimal usdAmount, string sourceCurrency, decimal rate) =>
        new(
            true,
            usdAmount.ToString("0.00", CultureInfo.InvariantCulture),
            sourceCurrency,
            rate,
            string.Empty);

    public static PricingConversionResult NotRecorded(string reason) =>
        new(false, string.Empty, string.Empty, null, reason);
}
