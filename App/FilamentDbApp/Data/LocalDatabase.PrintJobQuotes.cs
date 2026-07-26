using FilamentDbApp.Models;
using Microsoft.Data.Sqlite;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    public List<PrintJobQuoteRecord> LoadPrintJobQuotes()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT QuoteId,QuoteNumber,CreatedAtUtc,PreparedBy,
                                     CustomerName,Description,MaterialId,
                                     MaterialLabelSnapshot,MaterialCostProvenance,
                                     PrinterId,PrinterLabelSnapshot,QuoteCurrency,
                                     FinalPriceQuoteCurrency,FinalPriceIsk,
                                     CalculationVersion,SnapshotJson
                              FROM PrintJobQuotes
                              ORDER BY CreatedAtUtc DESC,QuoteId DESC;
                              """;
        var rows = new List<PrintJobQuoteRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new PrintJobQuoteRecord
            {
                QuoteId = ReadString(reader, "QuoteId"),
                QuoteNumber = ReadString(reader, "QuoteNumber"),
                CreatedAtUtc = ReadString(reader, "CreatedAtUtc"),
                PreparedBy = ReadString(reader, "PreparedBy"),
                CustomerName = ReadString(reader, "CustomerName"),
                Description = ReadString(reader, "Description"),
                MaterialId = ReadString(reader, "MaterialId"),
                MaterialLabelSnapshot = ReadString(reader, "MaterialLabelSnapshot"),
                MaterialCostProvenance = ReadString(reader, "MaterialCostProvenance"),
                PrinterId = ReadString(reader, "PrinterId"),
                PrinterLabelSnapshot = ReadString(reader, "PrinterLabelSnapshot"),
                QuoteCurrency = ReadString(reader, "QuoteCurrency"),
                FinalPriceQuoteCurrency = ReadString(reader, "FinalPriceQuoteCurrency"),
                FinalPriceIsk = ReadString(reader, "FinalPriceIsk"),
                CalculationVersion = ReadString(reader, "CalculationVersion"),
                SnapshotJson = ReadString(reader, "SnapshotJson")
            });
        }
        return rows;
    }

    public void InsertPrintJobQuote(PrintJobQuoteRecord quote)
    {
        if (string.IsNullOrWhiteSpace(quote.QuoteId) ||
            string.IsNullOrWhiteSpace(quote.SnapshotJson))
            throw new InvalidOperationException("Immutable quote identity and snapshot are required.");
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
                              INSERT INTO PrintJobQuotes (
                                  QuoteId,QuoteNumber,CreatedAtUtc,PreparedBy,
                                  CustomerName,Description,MaterialId,
                                  MaterialLabelSnapshot,MaterialCostProvenance,
                                  PrinterId,PrinterLabelSnapshot,QuoteCurrency,
                                  FinalPriceQuoteCurrency,FinalPriceIsk,
                                  CalculationVersion,SnapshotJson
                              ) VALUES (
                                  $id,$number,$created,$prepared,$customer,
                                  $description,$materialId,$materialLabel,
                                  $provenance,$printerId,$printerLabel,$currency,
                                  $finalCurrency,$finalIsk,$version,$snapshot);
                              """;
        command.Parameters.AddWithValue("$id", quote.QuoteId);
        command.Parameters.AddWithValue("$number", quote.QuoteNumber);
        command.Parameters.AddWithValue("$created", quote.CreatedAtUtc);
        command.Parameters.AddWithValue("$prepared", quote.PreparedBy);
        command.Parameters.AddWithValue("$customer", quote.CustomerName);
        command.Parameters.AddWithValue("$description", quote.Description);
        command.Parameters.AddWithValue("$materialId", quote.MaterialId);
        command.Parameters.AddWithValue("$materialLabel", quote.MaterialLabelSnapshot);
        command.Parameters.AddWithValue("$provenance", quote.MaterialCostProvenance);
        command.Parameters.AddWithValue("$printerId", quote.PrinterId);
        command.Parameters.AddWithValue("$printerLabel", quote.PrinterLabelSnapshot);
        command.Parameters.AddWithValue("$currency", quote.QuoteCurrency);
        command.Parameters.AddWithValue("$finalCurrency", quote.FinalPriceQuoteCurrency);
        command.Parameters.AddWithValue("$finalIsk", quote.FinalPriceIsk);
        command.Parameters.AddWithValue("$version", quote.CalculationVersion);
        command.Parameters.AddWithValue("$snapshot", quote.SnapshotJson);
        command.ExecuteNonQuery();
    }

    public void DeletePrintJobQuoteForAuthorizedAutomation(string quoteId)
    {
        if (!quoteId.StartsWith("AUT-Q-", StringComparison.Ordinal))
            throw new InvalidOperationException(
                "Only an explicitly disposable automation quote may be removed.");
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM PrintJobQuotes WHERE QuoteId=$id;";
        command.Parameters.AddWithValue("$id", quoteId);
        if (command.ExecuteNonQuery() != 1)
            throw new InvalidOperationException(
                "Authorized disposable quote was not found.");
    }

    public void DeletePrintJobQuote(string quoteId)
    {
        if (string.IsNullOrWhiteSpace(quoteId))
            throw new InvalidOperationException("QuoteID is required.");
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "DELETE FROM PrintJobQuotes WHERE QuoteId=$id;";
        command.Parameters.AddWithValue("$id", quoteId);
        if (command.ExecuteNonQuery() != 1)
            throw new InvalidOperationException("Saved quote was not found.");
    }
}
