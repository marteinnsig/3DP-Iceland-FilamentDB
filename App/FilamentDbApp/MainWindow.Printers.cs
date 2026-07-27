using FilamentDbApp.Models;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private bool _isLoadingPrinters;

    private void EnsurePricingSettings()
    {
        var added = false;
        foreach (var row in GetDefaultNativeSettingsRows().Where(row =>
                     string.Equals(row.Section, "Pricing", StringComparison.OrdinalIgnoreCase)))
        {
            if (_nativeSettingsRows.Any(existing =>
                    string.Equals(existing.Section, row.Section, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(existing.Parameter, row.Parameter, StringComparison.OrdinalIgnoreCase) &&
                    string.Equals(existing.UsedBy, row.UsedBy, StringComparison.OrdinalIgnoreCase)))
                continue;
            _nativeSettingsRows.Add(row);
            added = true;
        }
        if (added) SaveCanonicalNativeSettings();
    }

    private void InitializePrinterManager()
    {
        _isLoadingPrinters = true;
        _printerRows.Clear();
        foreach (var row in _database.LoadPrinterProfiles()) _printerRows.Add(row);
        PrinterCurrencyColumn.ItemsSource = GetGovernedPrinterCurrencies();
        PrinterGrid.ItemsSource = _printerRows;
        _isLoadingPrinters = false;
        RefreshPrinterRateSummary();
    }

    private IReadOnlyList<string> GetGovernedPrinterCurrencies()
    {
        const string prefix = "ISK per 1 ";
        return _nativeSettingsRows
            .Where(row =>
                string.Equals(row.Section, "Currency", StringComparison.OrdinalIgnoreCase) &&
                string.Equals(row.UsedBy, "Purchasing", StringComparison.OrdinalIgnoreCase) &&
                row.Parameter.StartsWith(prefix, StringComparison.OrdinalIgnoreCase) &&
                TryPrinterDecimal(row.Value, out var rate) &&
                rate > 0m)
            .Select(row => row.Parameter[prefix.Length..].Trim().ToUpperInvariant())
            .Where(currency => currency.Length > 0)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(currency => currency, StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private void AddPrinter_Click(object sender, RoutedEventArgs e)
    {
        var row = new PrinterProfileRecord
        {
            PrinterId = "PRN-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
            Name = "New printer"
        };
        _printerRows.Add(row);
        PrinterGrid.SelectedItem = row;
        PrinterGrid.ScrollIntoView(row);
        SavePrinters();
    }

    private void DuplicatePrinter_Click(object sender, RoutedEventArgs e)
    {
        if (PrinterGrid.SelectedItem is not PrinterProfileRecord source) return;
        var row = new PrinterProfileRecord
        {
            PrinterId = "PRN-" + Guid.NewGuid().ToString("N")[..8].ToUpperInvariant(),
            Name = source.Name + " copy",
            Manufacturer = source.Manufacturer,
            Model = source.Model,
            CostCurrency = source.CostCurrency,
            PurchaseCostAmount = source.PurchaseCostAmount,
            AdditionalUpfrontCostAmount = source.AdditionalUpfrontCostAmount,
            AnnualMaintenanceAmount = source.AnnualMaintenanceAmount,
            EstimatedLifeYears = source.EstimatedLifeYears,
            UptimePercent = source.UptimePercent,
            AveragePowerWatts = source.AveragePowerWatts,
            BufferOverride = source.BufferOverride,
            Notes = source.Notes,
            Provenance = source.Provenance
        };
        _printerRows.Add(row);
        PrinterGrid.SelectedItem = row;
        SavePrinters();
    }

    private void ArchivePrinter_Click(object sender, RoutedEventArgs e)
    {
        if (PrinterGrid.SelectedItem is not PrinterProfileRecord row) return;
        row.IsActive = !row.IsActive;
        PrinterGrid.Items.Refresh();
        SavePrinters();
    }

    private void DeletePrinter_Click(object sender, RoutedEventArgs e)
    {
        if (PrinterGrid.SelectedItem is not PrinterProfileRecord row) return;
        var answer = MessageBox.Show(this,
            $"Delete printer '{row.Name}'?\n\nExisting saved quote references block deletion so their printer snapshots remain traceable.",
            "Delete Printer?", MessageBoxButton.YesNo, MessageBoxImage.Warning,
            MessageBoxResult.No);
        if (answer != MessageBoxResult.Yes) return;
        _printerRows.Remove(row);
        try
        {
            SavePrinters();
        }
        catch (Microsoft.Data.Sqlite.SqliteException)
        {
            _printerRows.Add(row);
            PrinterGrid.SelectedItem = row;
            PrinterRateStatusText.Text =
                "Delete blocked: an immutable saved quote references this PrinterID. Archive it instead.";
        }
    }

    private void SavePrinters_Click(object sender, RoutedEventArgs e) => SavePrinters();

    private void PrinterGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
    {
        Dispatcher.BeginInvoke(() =>
        {
            SavePrinters();
            RefreshPrinterRateSummary();
        });
    }

    private void PrinterGrid_SelectionChanged(object sender, SelectionChangedEventArgs e) =>
        RefreshPrinterRateSummary();

    private void SavePrinters()
    {
        if (_isLoadingPrinters) return;
        if (_printerRows.Any(row => string.IsNullOrWhiteSpace(row.Name)))
        {
            PrinterRateStatusText.Text = "Name is required before printers can be saved.";
            return;
        }
        if (_printerRows.GroupBy(row => row.Name.Trim(), StringComparer.OrdinalIgnoreCase)
            .Any(group => group.Count() > 1))
        {
            PrinterRateStatusText.Text = "Printer names must be unique.";
            return;
        }
        var governedCurrencies = GetGovernedPrinterCurrencies();
        var unsupportedCurrency = _printerRows.FirstOrDefault(row =>
            !governedCurrencies.Contains(row.CostCurrency.Trim(),
                StringComparer.OrdinalIgnoreCase));
        if (unsupportedCurrency is not null)
        {
            PrinterRateStatusText.Text =
                $"Printer '{unsupportedCurrency.Name}' must use a governed Settings currency.";
            return;
        }
        _database.ReplacePrinterProfiles(_printerRows);
        PrinterRateStatusText.Text = $"Saved {_printerRows.Count:N0} printer profile(s).";
    }

    private void RefreshPrinterRateSummary()
    {
        if (PrinterGrid.SelectedItem is not PrinterProfileRecord row)
        {
            PrinterRateStatusText.Text = _printerRows.Count == 0
                ? "Add a printer to calculate its governed hourly rate."
                : "Select a printer to inspect its governed hourly rate.";
            return;
        }
        if (!TryPrinterDecimal(row.PurchaseCostAmount, out var purchase) ||
            !TryPrinterDecimal(row.AdditionalUpfrontCostAmount, out var upfront) ||
            !TryPrinterDecimal(row.AnnualMaintenanceAmount, out var maintenance) ||
            !TryPrinterDecimal(row.EstimatedLifeYears, out var life) ||
            !TryPrinterDecimal(row.UptimePercent, out var uptime) ||
            !TryPrinterDecimal(row.AveragePowerWatts, out var power) ||
            !TryPrinterDecimal(GetIskRateForPurchaseCurrency(row.CostCurrency), out var rate) ||
            !TryPrinterDecimal(PricingSetting("Electricity cost per kWh", "13"), out var electricity) ||
            !TryPrinterDecimal(string.IsNullOrWhiteSpace(row.BufferOverride)
                ? PricingSetting("Default printer buffer factor", "1.30")
                : row.BufferOverride, out var buffer))
        {
            PrinterRateStatusText.Text = "Enter valid numeric inputs and a governed currency.";
            return;
        }
        var result = _printerRateService.Calculate(new PrinterRateInput(
            purchase, upfront, maintenance, life, uptime, power, rate, electricity, buffer));
        PrinterRateStatusText.Text = result.IsValid
            ? $"Capital {result.CapitalCostIskPerHour:N2} + electricity " +
              $"{result.ElectricityCostIskPerHour:N2}; governed printer rate " +
              $"{result.TotalPrinterCostIskPerHour:N2} ISK/hour."
            : string.Join(" ", result.Errors);
    }

    private string PricingSetting(string parameter, string fallback) =>
        _nativeSettingsRows.FirstOrDefault(row =>
            string.Equals(row.Section, "Pricing", StringComparison.OrdinalIgnoreCase) &&
            string.Equals(row.Parameter, parameter, StringComparison.OrdinalIgnoreCase))
            ?.Value?.Trim() ?? fallback;

    private static bool TryPrinterDecimal(string? text, out decimal value)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            value = 0;
            return true;
        }
        var normalized = text.Trim().Replace(" ", "");
        if (normalized.Contains(',') && !normalized.Contains('.'))
            normalized = normalized.Replace(',', '.');
        else if (normalized.Contains(',') && normalized.Contains('.'))
        {
            if (normalized.LastIndexOf(',') > normalized.LastIndexOf('.'))
                normalized = normalized.Replace(".", "").Replace(',', '.');
            else
                normalized = normalized.Replace(",", "");
        }
        return decimal.TryParse(normalized, NumberStyles.AllowLeadingSign |
            NumberStyles.AllowDecimalPoint, CultureInfo.InvariantCulture, out value);
    }
}
