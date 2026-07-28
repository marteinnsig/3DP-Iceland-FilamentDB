using FilamentDbApp.Models;
using FilamentDbApp.Services;
using Microsoft.Win32;
using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private sealed record QuoteMaterialOption(NativeMaterialRow Row, string Label);

    private sealed record QuoteDraft(
        PrintJobQuoteInput Input,
        PrintJobQuoteCalculation Calculation,
        string MaterialId,
        string MaterialLabel,
        string MaterialProvenance,
        string MaterialSourceCurrency,
        decimal MaterialSourceRate,
        PrinterProfileRecord Printer,
        PrinterRateInput PrinterRateInput,
        PrinterRateResult PrinterRate,
        string QuoteCurrency,
        decimal QuoteCurrencyRate);

    private void InitializePrintJobQuoteWorkspace()
    {
        QuotePreparedByBox.Text = PricingSetting("Default prepared by", "3DP Iceland");
        QuoteGramsPerPartBox.Text = "100";
        QuoteQuantityBox.Text = "1";
        QuotePrintHoursBox.Text = "1";
        QuotePrintLaborMinutesBox.Text = "15";
        QuoteConsultingMinutesBox.Text = "10";
        QuoteDesignMinutesBox.Text = "0";
        QuoteAdditionalCostBox.Text = "0";
        QuoteMarginBox.Text = PricingSetting("Default target margin", "60");
        var currencies = GetGovernedPrinterCurrencies();
        QuoteCurrencySelector.ItemsSource = currencies;
        QuoteManualCurrencySelector.ItemsSource = currencies;
        QuoteCurrencySelector.SelectedItem =
            currencies.Contains(PricingSetting("Default quote currency", "ISK"),
                StringComparer.OrdinalIgnoreCase)
                ? PricingSetting("Default quote currency", "ISK").ToUpperInvariant()
                : "ISK";
        QuoteManualCurrencySelector.SelectedItem = "ISK";
        QuoteMaterialSelector.ItemsSource = _nativeMaterialRows
            .Where(row => !row.IsArchived)
            .OrderBy(row => QuoteMaterialLabel(row), StringComparer.OrdinalIgnoreCase)
            .Select(row => new QuoteMaterialOption(row, QuoteMaterialLabel(row)))
            .ToList();
        QuotePrinterSelector.ItemsSource = _printerRows.Where(row => row.IsActive).ToList();
        _printJobQuoteRows.Clear();
        foreach (var quote in _database.LoadPrintJobQuotes()) _printJobQuoteRows.Add(quote);
        PrintJobQuoteHistoryGrid.ItemsSource = _printJobQuoteRows;
        UpdateManualMaterialControls();
        RefreshPrintJobQuotePreview();
    }

    private static string QuoteMaterialLabel(NativeMaterialRow row)
    {
        var name = row.WebsiteDisplayName?.Trim() ?? string.Empty;
        if (string.IsNullOrWhiteSpace(name))
            name = string.Join(" ", new[]
            {
                row.Manufacturer, row.ProductLine, row.MarketingName,
                row.BaseMaterial, row.Color
            }.Where(value => !string.IsNullOrWhiteSpace(value)));
        if (string.IsNullOrWhiteSpace(name)) name = row.MaterialID;
        return $"{name} ({row.MaterialID})";
    }

    private void QuoteManualMaterialCheck_Changed(object sender, RoutedEventArgs e)
    {
        UpdateManualMaterialControls();
        RefreshPrintJobQuotePreview();
    }

    private void UpdateManualMaterialControls()
    {
        var manual = QuoteManualMaterialCheck.IsChecked == true;
        QuoteMaterialSelector.IsEnabled = !manual;
        QuoteManualMaterialCostBox.IsEnabled = manual;
        QuoteManualCurrencySelector.IsEnabled = manual;
    }

    private void QuoteInput_Changed(object sender, EventArgs e) =>
        RefreshPrintJobQuotePreview();

    private void RefreshPrintJobQuotePreview()
    {
        if (!IsLoaded && QuoteCalculationSummaryText is null) return;
        var draft = BuildQuoteDraft();
        if (draft is null) return;
        QuoteCalculationSummaryText.Text =
            $"{draft.Calculation.RequiredGrams:N2} g total · material " +
            $"{draft.Calculation.MaterialCostIsk:N2} ISK · printer " +
            $"{draft.Calculation.PrinterCostIsk:N2} ISK · labor " +
            $"{draft.Calculation.TotalLaborCostIsk:N2} ISK · landed " +
            $"{draft.Calculation.LandedCostIsk:N2} ISK · final " +
            $"{draft.Calculation.FinalPriceQuoteCurrency:N2} {draft.QuoteCurrency}";
    }

    private QuoteDraft? BuildQuoteDraft()
    {
        QuoteStatusText.Text = string.Empty;
        if (QuotePrinterSelector.SelectedItem is not PrinterProfileRecord printer)
        {
            QuoteCalculationSummaryText.Text = "Select an active canonical Printer.";
            return null;
        }
        if (!TryQuoteDecimal(QuoteGramsPerPartBox.Text, out var grams) ||
            !TryQuoteDecimal(QuoteQuantityBox.Text, out var quantity) ||
            !TryQuoteDecimal(QuotePrintHoursBox.Text, out var printHours) ||
            !TryQuoteDecimal(QuotePrintLaborMinutesBox.Text, out var printLaborMinutes) ||
            !TryQuoteDecimal(QuoteConsultingMinutesBox.Text, out var consultingMinutes) ||
            !TryQuoteDecimal(QuoteDesignMinutesBox.Text, out var designMinutes) ||
            !TryQuoteDecimal(QuoteAdditionalCostBox.Text, out var additional) ||
            !TryQuoteDecimal(QuoteMarginBox.Text, out var margin) ||
            !TryQuoteDecimal(PricingSetting("Material efficiency factor", "1.10"),
                out var efficiency) ||
            !TryQuoteDecimal(PricingSetting("Labor hourly rate", "7500"),
                out var laborRate))
        {
            QuoteCalculationSummaryText.Text = "Enter valid numeric quote inputs.";
            return null;
        }

        string materialId;
        string materialLabel;
        string provenance;
        string materialCurrency;
        decimal materialCostPerKg;
        if (QuoteManualMaterialCheck.IsChecked == true)
        {
            materialId = string.Empty;
            materialLabel = "Manual material evidence";
            provenance = "Manual cost per kg";
            materialCurrency = QuoteManualCurrencySelector.SelectedItem?.ToString() ?? "";
            if (!TryQuoteDecimal(QuoteManualMaterialCostBox.Text, out materialCostPerKg))
            {
                QuoteCalculationSummaryText.Text = "Enter a valid manual material cost per kg.";
                return null;
            }
        }
        else
        {
            if (QuoteMaterialSelector.SelectedItem is not QuoteMaterialOption option ||
                !TryQuoteDecimal(option.Row.LandedCostUsdPerKg, out materialCostPerKg) ||
                materialCostPerKg <= 0)
            {
                QuoteCalculationSummaryText.Text =
                    "Select a Material with valid canonical Landed Cost USD/kg, or use explicit manual cost.";
                return null;
            }
            materialId = option.Row.MaterialID;
            materialLabel = option.Label;
            provenance = "Canonical MaterialID LandedCostUsdPerKg";
            materialCurrency = "USD";
        }
        if (!TryQuoteDecimal(GetIskRateForPurchaseCurrency(materialCurrency),
                out var materialRate) || materialRate <= 0)
        {
            QuoteCalculationSummaryText.Text =
                $"No valid governed ISK rate exists for {materialCurrency}.";
            return null;
        }
        var quoteCurrency = QuoteCurrencySelector.SelectedItem?.ToString() ?? "";
        if (!TryQuoteDecimal(GetIskRateForPurchaseCurrency(quoteCurrency),
                out var quoteRate) || quoteRate <= 0)
        {
            QuoteCalculationSummaryText.Text =
                $"No valid governed quote rate exists for {quoteCurrency}.";
            return null;
        }
        if (!TryBuildPrinterRate(printer, out var printerInput, out var printerRate))
        {
            QuoteCalculationSummaryText.Text =
                "Selected Printer does not have valid governed rate inputs.";
            return null;
        }
        var input = new PrintJobQuoteInput(
            grams, quantity, efficiency, materialCostPerKg, materialRate,
            printHours, printerRate.TotalPrinterCostIskPerHour!.Value,
            printLaborMinutes, consultingMinutes, designMinutes, laborRate,
            additional, margin, quoteRate);
        var calculation = _printJobQuoteService.Calculate(input);
        if (!calculation.IsValid)
        {
            QuoteCalculationSummaryText.Text = string.Join(" ", calculation.Errors);
            return null;
        }
        QuoteMaterialEvidenceText.Text =
            $"{provenance}; {materialCostPerKg:N2} {materialCurrency}/kg; " +
            $"snapshotted rate {materialRate:N4} ISK per {materialCurrency}.";
        return new QuoteDraft(
            input, calculation, materialId, materialLabel, provenance,
            materialCurrency, materialRate, printer, printerInput, printerRate,
            quoteCurrency, quoteRate);
    }

    private bool TryBuildPrinterRate(
        PrinterProfileRecord printer,
        out PrinterRateInput input,
        out PrinterRateResult result)
    {
        input = default!;
        result = default!;
        if (!TryQuoteDecimal(printer.PurchaseCostAmount, out var purchase) ||
            !TryQuoteDecimal(printer.AdditionalUpfrontCostAmount, out var upfront) ||
            !TryQuoteDecimal(printer.AnnualMaintenanceAmount, out var maintenance) ||
            !TryQuoteDecimal(printer.EstimatedLifeYears, out var life) ||
            !TryQuoteDecimal(printer.UptimePercent, out var uptime) ||
            !TryQuoteDecimal(printer.AveragePowerWatts, out var power) ||
            !TryQuoteDecimal(GetIskRateForPurchaseCurrency(printer.CostCurrency),
                out var printerCurrencyRate) ||
            !TryQuoteDecimal(PricingSetting("Electricity cost per kWh", "13"),
                out var electricity) ||
            !TryQuoteDecimal(string.IsNullOrWhiteSpace(printer.BufferOverride)
                    ? PricingSetting("Default printer buffer factor", "1.30")
                    : printer.BufferOverride,
                out var buffer))
            return false;
        input = new PrinterRateInput(
            purchase, upfront, maintenance, life, uptime, power,
            printerCurrencyRate, electricity, buffer);
        result = _printerRateService.Calculate(input);
        return result.IsValid;
    }

    private void SavePrintJobQuote_Click(object sender, RoutedEventArgs e)
    {
        QuoteStatusText.Text = "Validating quote...";
        try
        {
            var draft = BuildQuoteDraft();
            if (draft is null)
            {
                QuoteStatusText.Text =
                    "Quote not saved: " + QuoteCalculationSummaryText.Text;
                return;
            }
            var created = DateTimeOffset.UtcNow;
            var id = "Q-" + Guid.NewGuid().ToString("N").ToUpperInvariant();
            var number = created.ToString("yyyyMMdd-HHmmss", CultureInfo.InvariantCulture) +
                         "-" + id[^4..];
            var snapshot = JsonSerializer.Serialize(new
            {
                schema = "3dpiceland-print-job-quote-v1",
                createdAtUtc = created,
                preparedBy = QuotePreparedByBox.Text.Trim(),
                customer = QuoteCustomerBox.Text.Trim(),
                description = QuoteDescriptionBox.Text.Trim(),
                draft.MaterialId,
                draft.MaterialLabel,
                draft.MaterialProvenance,
                draft.MaterialSourceCurrency,
                draft.MaterialSourceRate,
                draft.Printer,
                draft.PrinterRateInput,
                draft.PrinterRate,
                pricingSettings = new
                {
                    materialEfficiencyFactor = PricingSetting("Material efficiency factor", ""),
                    laborHourlyRate = PricingSetting("Labor hourly rate", ""),
                    electricityCostPerKwh = PricingSetting("Electricity cost per kWh", ""),
                    defaultPrinterBuffer = PricingSetting("Default printer buffer factor", ""),
                    targetMargin = QuoteMarginBox.Text.Trim()
                },
                draft.QuoteCurrency,
                draft.QuoteCurrencyRate,
                draft.Input,
                draft.Calculation,
                calculationVersion = "v1"
            });
            var quote = new PrintJobQuoteRecord
            {
                QuoteId = id,
                QuoteNumber = number,
                CreatedAtUtc = created.ToString("O", CultureInfo.InvariantCulture),
                PreparedBy = QuotePreparedByBox.Text.Trim(),
                CustomerName = QuoteCustomerBox.Text.Trim(),
                Description = QuoteDescriptionBox.Text.Trim(),
                MaterialId = draft.MaterialId,
                MaterialLabelSnapshot = draft.MaterialLabel,
                MaterialCostProvenance = draft.MaterialProvenance,
                PrinterId = draft.Printer.PrinterId,
                PrinterLabelSnapshot = draft.Printer.DisplayLabel,
                QuoteCurrency = draft.QuoteCurrency,
                FinalPriceQuoteCurrency =
                    draft.Calculation.FinalPriceQuoteCurrency!.Value.ToString(
                        "0.00", CultureInfo.InvariantCulture),
                FinalPriceIsk = draft.Calculation.FinalPriceIsk!.Value.ToString(
                        "0.00", CultureInfo.InvariantCulture),
                CalculationVersion = "v1",
                SnapshotJson = snapshot
            };
            _database.InsertPrintJobQuote(quote);
            _printJobQuoteRows.Insert(0, quote);
            PrintJobQuoteHistoryGrid.SelectedItem = quote;
            PrintJobQuoteHistoryGrid.ScrollIntoView(quote);
            QuoteStatusText.Text =
                $"Saved quote {quote.QuoteNumber}. Its calculation snapshot remains stable until you delete the quote.";
        }
        catch (Exception ex)
        {
            QuoteStatusText.Text = "Quote not saved: " + ex.Message;
        }
    }

    private async void ExportPrintJobQuote_Click(object sender, RoutedEventArgs e)
    {
        if (PrintJobQuoteHistoryGrid.SelectedItem is not PrintJobQuoteRecord quote)
        {
            QuoteStatusText.Text = "Select a saved quote to export.";
            return;
        }
        var dialog = new SaveFileDialog
        {
            Filter = "PDF document (*.pdf)|*.pdf",
            FileName = $"3DPIceland-Quote-{quote.QuoteNumber}.pdf",
            AddExtension = true,
            DefaultExt = ".pdf"
        };
        if (dialog.ShowDialog(this) != true) return;
        var tempHtml = IOPath.Combine(
            IOPath.GetTempPath(),
            "3DPIceland-Quote-" + Guid.NewGuid().ToString("N") + ".html");
        try
        {
            IOFile.WriteAllText(
                tempHtml, BuildCustomerQuoteHtml(quote), new UTF8Encoding(false));
            QuoteStatusText.Text = "Rendering customer PDF...";
            await WriteReportPdfFromCanonicalHtmlAsync(dialog.FileName, tempHtml);
            QuoteStatusText.Text = "Exported customer quote PDF: " + dialog.FileName;
        }
        catch (Exception ex)
        {
            QuoteStatusText.Text = "PDF export failed: " + ex.Message;
        }
        finally
        {
            if (IOFile.Exists(tempHtml)) IOFile.Delete(tempHtml);
        }
    }

    private string BuildCustomerQuoteHtml(PrintJobQuoteRecord quote)
    {
        static string H(string value) => WebUtility.HtmlEncode(value);
        var branding = new DocumentBrandingRendererService(_database).Resolve();
        var logoDataUri = branding.PngDataUri;
        var brandDisplayName = H(branding.BrandDisplayName);
        var customerMaterial = Regex.Replace(
            quote.MaterialLabelSnapshot,
            @"\s+\(MAT[^)]*\)\s*$",
            string.Empty,
            RegexOptions.IgnoreCase).Trim();
        using var snapshot = JsonDocument.Parse(quote.SnapshotJson);
        var root = snapshot.RootElement;
        var input = root.GetProperty("Input");
        var calculation = root.GetProperty("Calculation");
        var quantity = input.GetProperty("Quantity").GetDecimal();
        var finalPrice = calculation.GetProperty("FinalPriceQuoteCurrency").GetDecimal();
        var unitPrice = quantity > 0 ? finalPrice / quantity : finalPrice;
        var created = DateTimeOffset.TryParse(
            quote.CreatedAtUtc, CultureInfo.InvariantCulture,
            DateTimeStyles.RoundtripKind, out var createdAt)
            ? createdAt.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture)
            : quote.CreatedAtUtc;
        string Money(decimal value) =>
            value.ToString("#,##0", CultureInfo.InvariantCulture) +
            " " + H(quote.QuoteCurrency);
        return $$"""
                 <!doctype html><html lang="en"><head><meta charset="utf-8">
                 <title>3DPIceland Price Quote {{H(quote.QuoteNumber)}}</title>
                 <style>
                 @page{size:A4;margin:18mm 16mm}
                 *{box-sizing:border-box}body{font-family:Segoe UI,Arial,sans-serif;color:#0f172a;margin:0;font-size:14px}
                 .header{display:flex;justify-content:space-between;gap:28px;border-bottom:4px solid #0f172a;padding-bottom:20px;margin-bottom:28px}
                 .brand img{display:block;width:205px;max-height:92px;object-fit:contain;border:1px solid #dbe3ef;border-radius:12px;background:#fff;margin-bottom:12px}
                 .brand h1,.title h2{font-size:34px;margin:0 0 8px}.brand p,.title p{margin:5px 0;color:#475569}
                 .title{text-align:right}.title h2{font-size:36px}.meta{font-weight:700;color:#0f172a!important}
                 .customer{display:grid;grid-template-columns:1fr 1fr;gap:14px 28px;border:1px solid #cbd5e1;border-radius:12px;padding:18px;margin-bottom:22px}
                 .label{text-transform:uppercase;letter-spacing:.05em;font-size:11px;color:#64748b;margin-bottom:4px}
                 .value{font-size:15px;font-weight:700}.box{border:1px solid #cbd5e1;border-radius:12px;padding:18px}
                 table{width:100%;border-collapse:collapse;margin-top:10px}th{background:#e2e8f0;text-align:left}
                 th,td{border:1px solid #cbd5e1;padding:11px}th:nth-child(n+2),td:nth-child(n+2){text-align:right}
                 .grand{width:55%;margin:18px 0 0 auto}.grand td{font-size:18px;border-width:0 0 2px}
                 .grand td:last-child{font-size:22px;font-weight:800}.note{margin-top:28px;border-top:1px solid #cbd5e1;padding-top:16px;color:#475569;line-height:1.55}
                 .footer{margin-top:28px;text-align:center;font-size:11px;color:#64748b}
                 </style></head><body>
                 <div class="header"><div class="brand"><img src="{{logoDataUri}}" alt="{{brandDisplayName}}">
                 <h1>{{brandDisplayName}}</h1>
                 <p><strong>3D Printing Price Quote</strong></p>
                 <p class="meta">Quote #: {{H(quote.QuoteNumber)}}</p>
                 <p class="meta">Date: {{H(created)}}</p></div>
                 <div class="title"><h2>PRICE QUOTE</h2>
                 <p>Prepared by {{H(quote.PreparedBy)}}</p></div></div>
                 <div class="customer">
                 <div><div class="label">Customer</div><div class="value">{{H(quote.CustomerName)}}</div></div>
                 <div><div class="label">Description</div><div class="value">{{H(quote.Description)}}</div></div>
                 <div><div class="label">Material</div><div class="value">{{H(customerMaterial)}}</div></div>
                 </div>
                 <div class="box"><div class="label">Quote Summary</div>
                 <table><thead><tr><th>Description</th><th>Qty</th><th>Unit Price</th><th>Total</th></tr></thead>
                 <tbody><tr><td>{{H(string.IsNullOrWhiteSpace(quote.Description) ? "Custom 3D Prints" : quote.Description)}}</td>
                 <td>{{quantity.ToString("0.##", CultureInfo.InvariantCulture)}}</td>
                 <td>{{Money(unitPrice)}}</td><td>{{Money(finalPrice)}}</td></tr></tbody></table>
                 <table class="grand"><tr><td>Final quote price</td><td>{{Money(finalPrice)}}</td></tr></table></div>
                 <div class="note">This quote is an estimate based on the supplied project information.
                 Final pricing may change if the model, material, quantity, design requirements,
                 shipping, or delivery requirements change.</div>
                 <div class="footer">Generated with 3DPIceland Engineering Platform v{{H(BuildInfo.Version)}}</div>
                 </body></html>
                 """;
    }

    private string QuoteLogoDataUri()
    {
        return new DocumentBrandingRendererService(_database).Resolve().PngDataUri;
    }

    private void DeletePrintJobQuote_Click(object sender, RoutedEventArgs e)
    {
        if (PrintJobQuoteHistoryGrid.SelectedItem is not PrintJobQuoteRecord quote)
        {
            QuoteStatusText.Text = "Select a saved quote to delete.";
            return;
        }
        var answer = MessageBox.Show(this,
            $"Delete saved quote {quote.QuoteNumber}?\n\nThis removes the quote and its calculation snapshot permanently.",
            "Delete Saved Quote?", MessageBoxButton.YesNo,
            MessageBoxImage.Warning, MessageBoxResult.No);
        if (answer != MessageBoxResult.Yes) return;
        _database.DeletePrintJobQuote(quote.QuoteId);
        _printJobQuoteRows.Remove(quote);
        QuoteStatusText.Text = "Deleted saved quote " + quote.QuoteNumber + ".";
    }

    private static bool TryQuoteDecimal(string? text, out decimal value)
    {
        if (string.IsNullOrWhiteSpace(text))
        {
            value = 0m;
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
