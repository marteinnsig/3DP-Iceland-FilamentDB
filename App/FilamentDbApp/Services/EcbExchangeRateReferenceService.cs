using System.Globalization;
using System.Net.Http;
using System.Text.Json;
namespace FilamentDbApp.Services;

public sealed record ExchangeRateReference(
    string Currency,
    decimal IskPerCurrencyUnit,
    string ObservationDate,
    string Source,
    string SourceUrl,
    string FetchedAtUtc);

public sealed class ExchangeRateReferenceCatalog
{
    public string Provider { get; init; } = "European Central Bank";
    public string SourceUrl { get; init; } = string.Empty;
    public string FetchedAtUtc { get; init; } = string.Empty;
    public List<ExchangeRateReference> Rates { get; init; } = new();
}

public sealed class EcbExchangeRateReferenceService
{
    public const string ProviderName = "ECB reference rates, derived through EUR";
    public const string ApiUrl =
        "https://data-api.ecb.europa.eu/service/data/EXR/" +
        "D.USD+GBP+DKK+SEK+NOK+CHF+CAD+AUD+CNY+JPY+ISK.EUR.SP00.A" +
        "?lastNObservations=1&format=csvdata";

    private static readonly HttpClient Client = new(new HttpClientHandler
    {
        AllowAutoRedirect = false
    })
    {
        Timeout = TimeSpan.FromSeconds(12)
    };

    public async Task<ExchangeRateReferenceCatalog> FetchAsync(
        string cachePath,
        CancellationToken cancellationToken = default)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, ApiUrl);
        request.Headers.Accept.ParseAdd("text/csv");
        using var response = await Client.SendAsync(
            request,
            HttpCompletionOption.ResponseHeadersRead,
            cancellationToken);
        response.EnsureSuccessStatusCode();
        if (response.RequestMessage?.RequestUri is not { Scheme: "https", Host: "data-api.ecb.europa.eu" })
            throw new InvalidOperationException("ECB response identity did not match the governed HTTPS endpoint.");
        var csv = await response.Content.ReadAsStringAsync(cancellationToken);
        if (csv.Length > 1_000_000)
            throw new InvalidOperationException("ECB response exceeded the 1 MB safety limit.");

        var catalog = ParseCsv(csv, DateTime.UtcNow);
        var json = JsonSerializer.Serialize(catalog, new JsonSerializerOptions
        {
            WriteIndented = true
        });
        SafeFileOperations.WriteAllTextAtomic(cachePath, json);
        return catalog;
    }

    public static ExchangeRateReferenceCatalog? LoadCache(string cachePath)
    {
        if (!IOFile.Exists(cachePath)) return null;
        try
        {
            var catalog = JsonSerializer.Deserialize<ExchangeRateReferenceCatalog>(
                IOFile.ReadAllText(cachePath));
            return IsValidCache(catalog) ? catalog : null;
        }
        catch
        {
            return null;
        }
    }

    private static bool IsValidCache(ExchangeRateReferenceCatalog? catalog)
    {
        if (catalog is null ||
            !string.Equals(catalog.Provider, "European Central Bank", StringComparison.Ordinal) ||
            !string.Equals(catalog.SourceUrl, ApiUrl, StringComparison.Ordinal) ||
            !DateTime.TryParse(
                catalog.FetchedAtUtc,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind,
                out _) ||
            catalog.Rates.Count == 0 ||
            catalog.Rates.Select(rate => rate.Currency)
                .Distinct(StringComparer.OrdinalIgnoreCase).Count() != catalog.Rates.Count)
            return false;
        return catalog.Rates.All(rate =>
            rate.Currency.Length == 3 &&
            rate.Currency.All(character => character is >= 'A' and <= 'Z') &&
            rate.IskPerCurrencyUnit > 0m &&
            DateOnly.TryParseExact(
                rate.ObservationDate,
                "yyyy-MM-dd",
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out _) &&
            string.Equals(rate.Source, ProviderName, StringComparison.Ordinal) &&
            string.Equals(rate.SourceUrl, ApiUrl, StringComparison.Ordinal));
    }

    public static ExchangeRateReferenceCatalog ParseCsv(string csv, DateTime fetchedAtUtc)
    {
        var lines = csv.Replace("\r\n", "\n", StringComparison.Ordinal)
            .Split('\n', StringSplitOptions.RemoveEmptyEntries);
        if (lines.Length < 2)
            throw new InvalidOperationException("ECB response contained no observations.");

        var headers = ParseCsvLine(lines[0]);
        var currencyIndex = HeaderIndex(headers, "CURRENCY");
        var dateIndex = HeaderIndex(headers, "TIME_PERIOD");
        var valueIndex = HeaderIndex(headers, "OBS_VALUE");
        var observations = new Dictionary<string, (decimal Value, string Date)>(
            StringComparer.OrdinalIgnoreCase);

        foreach (var line in lines.Skip(1))
        {
            var fields = ParseCsvLine(line);
            if (fields.Count <= Math.Max(currencyIndex, Math.Max(dateIndex, valueIndex)))
                continue;
            var currency = fields[currencyIndex].Trim().ToUpperInvariant();
            var date = fields[dateIndex].Trim();
            if (currency.Length != 3 ||
                !DateOnly.TryParseExact(
                    date,
                    "yyyy-MM-dd",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out _) ||
                !decimal.TryParse(
                    fields[valueIndex],
                    NumberStyles.Float,
                    CultureInfo.InvariantCulture,
                    out var value) ||
                value <= 0m)
                continue;
            observations[currency] = (value, date);
        }

        if (!observations.TryGetValue("ISK", out var isk))
            throw new InvalidOperationException("ECB response did not contain a valid ISK/EUR observation.");

        var fetched = fetchedAtUtc.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture);
        var rates = new List<ExchangeRateReference>
        {
            new("ISK", 1m, isk.Date, ProviderName, ApiUrl, fetched)
        };
        foreach (var (currency, observation) in observations
                     .Where(item => !string.Equals(item.Key, "ISK", StringComparison.OrdinalIgnoreCase))
                     .OrderBy(item => item.Key, StringComparer.Ordinal))
        {
            rates.Add(new ExchangeRateReference(
                currency,
                decimal.Round(isk.Value / observation.Value, 6, MidpointRounding.AwayFromZero),
                string.CompareOrdinal(isk.Date, observation.Date) <= 0 ? isk.Date : observation.Date,
                ProviderName,
                ApiUrl,
                fetched));
        }

        if (rates.Count < 2)
            throw new InvalidOperationException("ECB response contained no usable cross-rates.");
        return new ExchangeRateReferenceCatalog
        {
            SourceUrl = ApiUrl,
            FetchedAtUtc = fetched,
            Rates = rates
        };
    }

    private static int HeaderIndex(IReadOnlyList<string> headers, string name)
    {
        for (var index = 0; index < headers.Count; index++)
            if (string.Equals(headers[index].Trim(), name, StringComparison.OrdinalIgnoreCase))
                return index;
        throw new InvalidOperationException("ECB response is missing the " + name + " column.");
    }

    private static List<string> ParseCsvLine(string line)
    {
        var fields = new List<string>();
        var current = new System.Text.StringBuilder();
        var quoted = false;
        for (var index = 0; index < line.Length; index++)
        {
            var character = line[index];
            if (character == '"')
            {
                if (quoted && index + 1 < line.Length && line[index + 1] == '"')
                {
                    current.Append('"');
                    index++;
                }
                else
                {
                    quoted = !quoted;
                }
            }
            else if (character == ',' && !quoted)
            {
                fields.Add(current.ToString());
                current.Clear();
            }
            else
            {
                current.Append(character);
            }
        }
        fields.Add(current.ToString());
        return fields;
    }
}
