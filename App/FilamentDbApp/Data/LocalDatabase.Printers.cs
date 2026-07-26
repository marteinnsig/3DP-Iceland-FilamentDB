using FilamentDbApp.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    public List<PrinterProfileRecord> LoadPrinterProfiles()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
                              SELECT PrinterId,Name,Manufacturer,Model,
                                     CostCurrency,PurchaseCostAmount,
                                     AdditionalUpfrontCostAmount,
                                     AnnualMaintenanceAmount,EstimatedLifeYears,
                                     UptimePercent,AveragePowerWatts,
                                     BufferOverride,IsActive,Notes,Provenance,
                                     UpdatedAtUtc
                              FROM PrinterProfiles
                              ORDER BY IsActive DESC,Name,PrinterId;
                              """;
        var rows = new List<PrinterProfileRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new PrinterProfileRecord
            {
                PrinterId = ReadString(reader, "PrinterId"),
                Name = ReadString(reader, "Name"),
                Manufacturer = ReadString(reader, "Manufacturer"),
                Model = ReadString(reader, "Model"),
                CostCurrency = ReadString(reader, "CostCurrency"),
                PurchaseCostAmount = ReadString(reader, "PurchaseCostAmount"),
                AdditionalUpfrontCostAmount =
                    ReadString(reader, "AdditionalUpfrontCostAmount"),
                AnnualMaintenanceAmount =
                    ReadString(reader, "AnnualMaintenanceAmount"),
                EstimatedLifeYears = ReadString(reader, "EstimatedLifeYears"),
                UptimePercent = ReadString(reader, "UptimePercent"),
                AveragePowerWatts = ReadString(reader, "AveragePowerWatts"),
                BufferOverride = ReadString(reader, "BufferOverride"),
                IsActive = reader.GetInt32(
                    reader.GetOrdinal("IsActive")) != 0,
                Notes = ReadString(reader, "Notes"),
                Provenance = ReadString(reader, "Provenance"),
                UpdatedAtUtc = ReadString(reader, "UpdatedAtUtc")
            });
        }
        return rows;
    }

    public void ReplacePrinterProfiles(IEnumerable<PrinterProfileRecord> profiles)
    {
        var rows = profiles.ToList();
        if (rows.Any(row => string.IsNullOrWhiteSpace(row.PrinterId)) ||
            rows.Select(row => row.PrinterId.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase).Count() != rows.Count)
            throw new InvalidOperationException(
                "PrinterID must be present and unique.");

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        foreach (var row in rows)
        {
            using var insert = connection.CreateCommand();
            insert.Transaction = transaction;
            insert.CommandText = """
                                 INSERT INTO PrinterProfiles (
                                     PrinterId,Name,Manufacturer,Model,
                                     CostCurrency,PurchaseCostAmount,
                                     AdditionalUpfrontCostAmount,
                                     AnnualMaintenanceAmount,EstimatedLifeYears,
                                     UptimePercent,AveragePowerWatts,
                                     BufferOverride,IsActive,Notes,Provenance,
                                     UpdatedAtUtc
                                 ) VALUES (
                                     $id,$name,$manufacturer,$model,$currency,
                                     $purchase,$upfront,$maintenance,$life,
                                     $uptime,$power,$buffer,$active,$notes,
                                     $provenance,$updated)
                                 ON CONFLICT(PrinterId) DO UPDATE SET
                                     Name=excluded.Name,
                                     Manufacturer=excluded.Manufacturer,
                                     Model=excluded.Model,
                                     CostCurrency=excluded.CostCurrency,
                                     PurchaseCostAmount=excluded.PurchaseCostAmount,
                                     AdditionalUpfrontCostAmount=excluded.AdditionalUpfrontCostAmount,
                                     AnnualMaintenanceAmount=excluded.AnnualMaintenanceAmount,
                                     EstimatedLifeYears=excluded.EstimatedLifeYears,
                                     UptimePercent=excluded.UptimePercent,
                                     AveragePowerWatts=excluded.AveragePowerWatts,
                                     BufferOverride=excluded.BufferOverride,
                                     IsActive=excluded.IsActive,
                                     Notes=excluded.Notes,
                                     Provenance=excluded.Provenance,
                                     UpdatedAtUtc=excluded.UpdatedAtUtc;
                                 """;
            insert.Parameters.AddWithValue("$id", row.PrinterId.Trim());
            insert.Parameters.AddWithValue("$name", row.Name.Trim());
            insert.Parameters.AddWithValue("$manufacturer", row.Manufacturer.Trim());
            insert.Parameters.AddWithValue("$model", row.Model.Trim());
            insert.Parameters.AddWithValue(
                "$currency",
                string.IsNullOrWhiteSpace(row.CostCurrency)
                    ? "ISK"
                    : row.CostCurrency.Trim().ToUpperInvariant());
            insert.Parameters.AddWithValue("$purchase", row.PurchaseCostAmount.Trim());
            insert.Parameters.AddWithValue(
                "$upfront",
                row.AdditionalUpfrontCostAmount.Trim());
            insert.Parameters.AddWithValue(
                "$maintenance",
                row.AnnualMaintenanceAmount.Trim());
            insert.Parameters.AddWithValue("$life", row.EstimatedLifeYears.Trim());
            insert.Parameters.AddWithValue("$uptime", row.UptimePercent.Trim());
            insert.Parameters.AddWithValue("$power", row.AveragePowerWatts.Trim());
            insert.Parameters.AddWithValue("$buffer", row.BufferOverride.Trim());
            insert.Parameters.AddWithValue("$active", row.IsActive ? 1 : 0);
            insert.Parameters.AddWithValue("$notes", row.Notes.Trim());
            insert.Parameters.AddWithValue("$provenance", row.Provenance.Trim());
            insert.Parameters.AddWithValue(
                "$updated",
                DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
            insert.ExecuteNonQuery();
        }
        var retainedIds = rows.Select(row => row.PrinterId.Trim()).ToList();
        using (var deleteMissing = connection.CreateCommand())
        {
            deleteMissing.Transaction = transaction;
            if (retainedIds.Count == 0)
            {
                deleteMissing.CommandText = "DELETE FROM PrinterProfiles;";
            }
            else
            {
                var names = retainedIds.Select((_, index) => "$keep" + index).ToList();
                deleteMissing.CommandText =
                    $"DELETE FROM PrinterProfiles WHERE PrinterId NOT IN ({string.Join(",", names)});";
                for (var index = 0; index < retainedIds.Count; index++)
                    deleteMissing.Parameters.AddWithValue("$keep" + index, retainedIds[index]);
            }
            deleteMissing.ExecuteNonQuery();
        }
        transaction.Commit();
    }
}
