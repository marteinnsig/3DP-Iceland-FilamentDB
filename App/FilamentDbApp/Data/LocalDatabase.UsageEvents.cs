using FilamentDbApp.Models;
using FilamentDbApp.Services;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    public List<UsageEventRecord> LoadUsageEvents()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        return LoadUsageEvents(connection, null);
    }

    public void AppendUsageEventAtomic(UsageEventRecord usageEvent)
    {
        ArgumentNullException.ThrowIfNull(usageEvent);
        PersistUsageEventsAtomic([usageEvent]);
    }

    public UsageEventCorrection AppendUsageCorrectionAtomic(
        string originalUsageEventId,
        UsageEventRecord replacement,
        string reversalUsageEventId,
        DateTimeOffset correctedAtUtc,
        string source,
        string note)
    {
        var accepted = LoadUsageEvents();
        var original = accepted.SingleOrDefault(item => item.UsageEventId.Equals(
                           originalUsageEventId,
                           StringComparison.OrdinalIgnoreCase))
                       ?? throw new InvalidOperationException(
                           "The original UsageEventId was not found.");
        var correction = new UsageEventDomainService().CreateCorrection(
            original,
            replacement,
            reversalUsageEventId,
            correctedAtUtc,
            source,
            note,
            accepted);
        PersistUsageEventsAtomic([correction.Reversal, correction.Replacement]);
        return correction;
    }

    public (int Events, int Reversals, int Replacements, int OrphanedMaterials,
        int OrphanedInventory, int DuplicateReversals) GetUsageEventStats()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        int Scalar(string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            return Convert.ToInt32(command.ExecuteScalar() ?? 0, CultureInfo.InvariantCulture);
        }

        return (
            Scalar("SELECT COUNT(*) FROM UsageEvents;"),
            Scalar("SELECT COUNT(*) FROM UsageEvents WHERE EntryKind='Reversal';"),
            Scalar("SELECT COUNT(*) FROM UsageEvents WHERE EntryKind='Replacement';"),
            Scalar("""
                   SELECT COUNT(*) FROM UsageEvents u
                   LEFT JOIN NativeMaterialManagerRows m ON m.MaterialId=u.MaterialId
                   WHERE m.MaterialId IS NULL;
                   """),
            Scalar("""
                   SELECT COUNT(*) FROM UsageEvents u
                   LEFT JOIN InventorySpoolItems i ON i.InventoryItemId=u.InventoryItemId
                   WHERE u.InventoryItemId IS NOT NULL AND i.InventoryItemId IS NULL;
                   """),
            Scalar("""
                   SELECT COUNT(*) FROM (
                       SELECT ReversesUsageEventId FROM UsageEvents
                       WHERE ReversesUsageEventId IS NOT NULL
                       GROUP BY ReversesUsageEventId HAVING COUNT(*) > 1
                   );
                   """));
    }

    public void DeleteUsageEventsForAuthorizedAutomation(string materialId)
    {
        AutomationRuntimeProfile.DemandMaterialCrudAuthorized(materialId);
        DeleteUsageEventsForMaterialMaintenance(materialId);
    }

    public int DeleteUsageEventsForMaterialMaintenance(string materialId)
    {
        var normalizedMaterialId = materialId?.Trim();
        if (string.IsNullOrWhiteSpace(normalizedMaterialId))
            throw new ArgumentException("MaterialID is required.", nameof(materialId));

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        int Count(string sql)
        {
            using var command = connection.CreateCommand();
            command.Transaction = transaction;
            command.CommandText = sql;
            command.Parameters.AddWithValue("$material", normalizedMaterialId);
            return Convert.ToInt32(command.ExecuteScalar() ?? 0, CultureInfo.InvariantCulture);
        }

        var eventCount = Count(
            "SELECT COUNT(*) FROM UsageEvents WHERE MaterialId=$material;");
        if (eventCount == 0)
        {
            transaction.Commit();
            return 0;
        }

        var externalReferenceCount = Count("""
            SELECT COUNT(*)
            FROM UsageEvents AS child
            WHERE child.MaterialId <> $material
              AND (
                    child.ReversesUsageEventId IN (
                        SELECT UsageEventId FROM UsageEvents WHERE MaterialId=$material)
                 OR child.CorrectsUsageEventId IN (
                        SELECT UsageEventId FROM UsageEvents WHERE MaterialId=$material)
              );
            """);
        if (externalReferenceCount != 0)
            throw new InvalidOperationException(
                "Usage cleanup was blocked because another MaterialID references this ledger chain.");

        using (var corrections = connection.CreateCommand())
        {
            corrections.Transaction = transaction;
            corrections.CommandText = """
                                      DELETE FROM UsageEvents
                                      WHERE MaterialId=$material
                                        AND (ReversesUsageEventId IS NOT NULL
                                             OR CorrectsUsageEventId IS NOT NULL);
                                      """;
            corrections.Parameters.AddWithValue("$material", normalizedMaterialId);
            corrections.ExecuteNonQuery();
        }
        using (var originals = connection.CreateCommand())
        {
            originals.Transaction = transaction;
            originals.CommandText =
                "DELETE FROM UsageEvents WHERE MaterialId=$material;";
            originals.Parameters.AddWithValue("$material", normalizedMaterialId);
            originals.ExecuteNonQuery();
        }

        if (Count("SELECT COUNT(*) FROM UsageEvents WHERE MaterialId=$material;") != 0)
            throw new InvalidOperationException(
                "Usage cleanup did not remove the exact selected MaterialID scope.");
        transaction.Commit();
        return eventCount;
    }

    public static bool RunUsageHistoryMaintenanceDeletionContractVerification()
    {
        var root = IOPath.Combine(
            IOPath.GetTempPath(),
            "3DPIceland-UsageCleanup-" + Guid.NewGuid().ToString("N"));
        IODirectory.CreateDirectory(root);
        try
        {
            var database = new LocalDatabase(IOPath.Combine(root, "usage-cleanup.sqlite"));
            using (var connection = new SqliteConnection(database.ConnectionString))
            {
                connection.Open();
                using var command = connection.CreateCommand();
                command.CommandText = """
                    INSERT INTO NativeMaterialManagerRows(MaterialId, UpdatedAtUtc)
                    VALUES
                        ('VERIFY-USAGE-CLEANUP', '2026-07-29T00:00:00Z'),
                        ('VERIFY-USAGE-KEEP', '2026-07-29T00:00:00Z');
                    INSERT INTO InventorySpoolItems
                        (InventoryItemId, MaterialId, Status, Quantity, RemainingWeightG, UpdatedAtUtc)
                    VALUES
                        ('VERIFY-USAGE-SPOOL', 'VERIFY-USAGE-CLEANUP', 'Opened', '1', '900',
                         '2026-07-29T00:00:00Z');
                    INSERT INTO UsageEvents
                        (UsageEventId, MaterialId, EventType, EntryKind, OccurredAtUtc, CreatedAtUtc,
                         InventoryItemId, FilamentUsedGrams, FilamentProvenance,
                         ReversesUsageEventId, CorrectsUsageEventId)
                    VALUES
                        ('VERIFY-USAGE-ORIGINAL', 'VERIFY-USAGE-CLEANUP', 'Print', 'Original',
                         '2026-07-29T00:00:00Z', '2026-07-29T00:00:00Z',
                         'VERIFY-USAGE-SPOOL', '100', 'Measured', NULL, NULL),
                        ('VERIFY-USAGE-REVERSAL', 'VERIFY-USAGE-CLEANUP', 'Print', 'Reversal',
                         '2026-07-29T00:01:00Z', '2026-07-29T00:01:00Z',
                         'VERIFY-USAGE-SPOOL', '-100', 'Measured', 'VERIFY-USAGE-ORIGINAL', NULL),
                        ('VERIFY-USAGE-REPLACEMENT', 'VERIFY-USAGE-CLEANUP', 'Print', 'Replacement',
                         '2026-07-29T00:02:00Z', '2026-07-29T00:02:00Z',
                         'VERIFY-USAGE-SPOOL', '80', 'Measured', NULL, 'VERIFY-USAGE-ORIGINAL'),
                        ('VERIFY-USAGE-OTHER', 'VERIFY-USAGE-KEEP', 'Print', 'Original',
                         '2026-07-29T00:03:00Z', '2026-07-29T00:03:00Z',
                         NULL, NULL, 'NotRecorded', NULL, NULL);
                    """;
                command.ExecuteNonQuery();
            }

            if (database.DeleteUsageEventsForMaterialMaintenance(
                    "VERIFY-USAGE-CLEANUP") != 3)
                return false;

            using var verifyConnection = new SqliteConnection(database.ConnectionString);
            verifyConnection.Open();
            using var verify = verifyConnection.CreateCommand();
            verify.CommandText = """
                SELECT
                    (SELECT COUNT(*) FROM UsageEvents
                     WHERE MaterialId='VERIFY-USAGE-CLEANUP') || '|' ||
                    (SELECT COUNT(*) FROM UsageEvents
                     WHERE MaterialId='VERIFY-USAGE-KEEP') || '|' ||
                    (SELECT RemainingWeightG FROM InventorySpoolItems
                     WHERE InventoryItemId='VERIFY-USAGE-SPOOL');
                """;
            return string.Equals(
                verify.ExecuteScalar()?.ToString(),
                "0|1|900",
                StringComparison.Ordinal);
        }
        catch
        {
            return false;
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            try { IODirectory.Delete(root, true); } catch { }
        }
    }

    private void PersistUsageEventsAtomic(IReadOnlyList<UsageEventRecord> newEvents)
    {
        if (newEvents.Count == 0) return;

        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        using var transaction = connection.BeginTransaction();
        var accepted = LoadUsageEvents(connection, transaction);
        var materials = LoadIdentityValues(
            connection,
            transaction,
            "SELECT MaterialId FROM NativeMaterialManagerRows;");
        var inventory = LoadUsageInventoryIdentities(connection, transaction);
        var service = new UsageEventDomainService();
        var inventoryDeltas = new Dictionary<string, decimal>(StringComparer.OrdinalIgnoreCase);

        foreach (var usageEvent in newEvents)
        {
            var validation = service.Validate(usageEvent, materials, inventory, accepted);
            if (!validation.IsValid)
                throw new InvalidOperationException(
                    "Usage event validation failed: " +
                    string.Join(" ", validation.Errors));

            var delta = service.BuildInventoryDelta(usageEvent);
            if (delta is not null)
                inventoryDeltas[delta.InventoryItemId] =
                    inventoryDeltas.GetValueOrDefault(delta.InventoryItemId) +
                    delta.RemainingWeightDeltaGrams;

            InsertUsageEvent(connection, transaction, usageEvent);
            accepted.Add(usageEvent);
        }

        foreach (var delta in inventoryDeltas)
            ApplyInventoryDelta(connection, transaction, delta.Key, delta.Value);

        transaction.Commit();
    }

    private static List<UsageEventRecord> LoadUsageEvents(
        SqliteConnection connection,
        SqliteTransaction? transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
                              SELECT UsageEventId,MaterialId,EventType,EntryKind,
                                     OccurredAtUtc,CreatedAtUtc,InventoryItemId,
                                     ExperimentalRunId,FuturePrintJobId,
                                     FutureTestSessionId,FilamentUsedGrams,
                                     FilamentProvenance,PrintDurationSeconds,
                                     HandsOnDurationSeconds,ProducedCount,
                                     AcceptedCount,RejectedCount,Source,Note,
                                     Origin,ReversesUsageEventId,CorrectsUsageEventId
                              FROM UsageEvents
                              ORDER BY CreatedAtUtc,UsageEventId;
                              """;
        var rows = new List<UsageEventRecord>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
        {
            rows.Add(new UsageEventRecord
            {
                UsageEventId = ReadString(reader, "UsageEventId"),
                MaterialId = ReadString(reader, "MaterialId"),
                EventType = ParseEnum<UsageEventType>(ReadString(reader, "EventType")),
                EntryKind = ParseEnum<UsageEventEntryKind>(ReadString(reader, "EntryKind")),
                OccurredAtUtc = ParseUtc(ReadString(reader, "OccurredAtUtc")),
                CreatedAtUtc = ParseUtc(ReadString(reader, "CreatedAtUtc")),
                InventoryItemId = ReadNullableString(reader, "InventoryItemId"),
                ExperimentalRunId = ReadNullableString(reader, "ExperimentalRunId"),
                FuturePrintJobId = ReadNullableString(reader, "FuturePrintJobId"),
                FutureTestSessionId = ReadNullableString(reader, "FutureTestSessionId"),
                FilamentUsedGrams = ReadNullableDecimal(reader, "FilamentUsedGrams"),
                FilamentProvenance = ParseEnum<UsageQuantityProvenance>(
                    ReadString(reader, "FilamentProvenance")),
                PrintDurationSeconds = ReadNullableInt64(reader, "PrintDurationSeconds"),
                HandsOnDurationSeconds = ReadNullableInt64(reader, "HandsOnDurationSeconds"),
                ProducedCount = ReadNullableInt32(reader, "ProducedCount"),
                AcceptedCount = ReadNullableInt32(reader, "AcceptedCount"),
                RejectedCount = ReadNullableInt32(reader, "RejectedCount"),
                Source = ReadString(reader, "Source"),
                Note = ReadString(reader, "Note"),
                Origin = ReadString(reader, "Origin"),
                ReversesUsageEventId = ReadNullableString(reader, "ReversesUsageEventId"),
                CorrectsUsageEventId = ReadNullableString(reader, "CorrectsUsageEventId")
            });
        }

        return rows;
    }

    private static void InsertUsageEvent(
        SqliteConnection connection,
        SqliteTransaction transaction,
        UsageEventRecord usageEvent)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = """
                              INSERT INTO UsageEvents (
                                  UsageEventId,MaterialId,EventType,EntryKind,
                                  OccurredAtUtc,CreatedAtUtc,InventoryItemId,
                                  ExperimentalRunId,FuturePrintJobId,
                                  FutureTestSessionId,FilamentUsedGrams,
                                  FilamentProvenance,PrintDurationSeconds,
                                  HandsOnDurationSeconds,ProducedCount,
                                  AcceptedCount,RejectedCount,Source,Note,
                                  Origin,ReversesUsageEventId,CorrectsUsageEventId
                              ) VALUES (
                                  $id,$material,$type,$kind,$occurred,$created,
                                  $inventory,$run,$job,$session,$grams,
                                  $provenance,$printSeconds,$handsOnSeconds,
                                  $produced,$accepted,$rejected,$source,$note,
                                  $origin,$reverses,$corrects
                              );
                              """;
        command.Parameters.AddWithValue("$id", usageEvent.UsageEventId);
        command.Parameters.AddWithValue("$material", usageEvent.MaterialId);
        command.Parameters.AddWithValue("$type", usageEvent.EventType.ToString());
        command.Parameters.AddWithValue("$kind", usageEvent.EntryKind.ToString());
        command.Parameters.AddWithValue(
            "$occurred",
            usageEvent.OccurredAtUtc.ToString("O", CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue(
            "$created",
            usageEvent.CreatedAtUtc.ToString("O", CultureInfo.InvariantCulture));
        AddNullable(command, "$inventory", usageEvent.InventoryItemId);
        AddNullable(command, "$run", usageEvent.ExperimentalRunId);
        AddNullable(command, "$job", usageEvent.FuturePrintJobId);
        AddNullable(command, "$session", usageEvent.FutureTestSessionId);
        AddNullable(
            command,
            "$grams",
            usageEvent.FilamentUsedGrams?.ToString(CultureInfo.InvariantCulture));
        command.Parameters.AddWithValue("$provenance", usageEvent.FilamentProvenance.ToString());
        AddNullable(command, "$printSeconds", usageEvent.PrintDurationSeconds);
        AddNullable(command, "$handsOnSeconds", usageEvent.HandsOnDurationSeconds);
        AddNullable(command, "$produced", usageEvent.ProducedCount);
        AddNullable(command, "$accepted", usageEvent.AcceptedCount);
        AddNullable(command, "$rejected", usageEvent.RejectedCount);
        AddNullable(command, "$source", usageEvent.Source);
        AddNullable(command, "$note", usageEvent.Note);
        AddNullable(command, "$origin", usageEvent.Origin);
        AddNullable(command, "$reverses", usageEvent.ReversesUsageEventId);
        AddNullable(command, "$corrects", usageEvent.CorrectsUsageEventId);
        command.ExecuteNonQuery();
    }

    private static void ApplyInventoryDelta(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string inventoryItemId,
        decimal delta)
    {
        using var select = connection.CreateCommand();
        select.Transaction = transaction;
        select.CommandText = """
                             SELECT RemainingWeightG FROM InventorySpoolItems
                             WHERE InventoryItemId=$id;
                             """;
        select.Parameters.AddWithValue("$id", inventoryItemId);
        var text = select.ExecuteScalar()?.ToString();
        if (!decimal.TryParse(
                (text ?? string.Empty).Replace(',', '.'),
                NumberStyles.Number,
                CultureInfo.InvariantCulture,
                out var remaining))
            throw new InvalidOperationException(
                "Linked inventory RemainingWeightG is missing or invalid.");
        var updated = remaining + delta;
        if (updated < 0)
            throw new InvalidOperationException(
                "Usage event would reduce RemainingWeightG below zero.");

        using var update = connection.CreateCommand();
        update.Transaction = transaction;
        update.CommandText = """
                             UPDATE InventorySpoolItems
                             SET RemainingWeightG=$remaining,UpdatedAtUtc=$updated
                             WHERE InventoryItemId=$id;
                             """;
        update.Parameters.AddWithValue(
            "$remaining",
            updated.ToString("0.############################", CultureInfo.InvariantCulture));
        update.Parameters.AddWithValue(
            "$updated",
            DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture));
        update.Parameters.AddWithValue("$id", inventoryItemId);
        if (update.ExecuteNonQuery() != 1)
            throw new InvalidOperationException("Linked inventory spool was not updated exactly once.");
    }

    private static List<string> LoadIdentityValues(
        SqliteConnection connection,
        SqliteTransaction transaction,
        string sql)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        var values = new List<string>();
        using var reader = command.ExecuteReader();
        while (reader.Read()) values.Add(reader.GetString(0));
        return values;
    }

    private static List<UsageInventoryIdentity> LoadUsageInventoryIdentities(
        SqliteConnection connection,
        SqliteTransaction transaction)
    {
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "SELECT InventoryItemId,MaterialId FROM InventorySpoolItems;";
        var values = new List<UsageInventoryIdentity>();
        using var reader = command.ExecuteReader();
        while (reader.Read())
            values.Add(new UsageInventoryIdentity(reader.GetString(0), reader.GetString(1)));
        return values;
    }

    private static string? ReadNullableString(SqliteDataReader reader, string name) =>
        reader.IsDBNull(reader.GetOrdinal(name)) ? null : reader.GetString(reader.GetOrdinal(name));

    private static decimal? ReadNullableDecimal(SqliteDataReader reader, string name)
    {
        var value = ReadNullableString(reader, name);
        return value is null
            ? null
            : decimal.Parse(value, NumberStyles.Number, CultureInfo.InvariantCulture);
    }

    private static long? ReadNullableInt64(SqliteDataReader reader, string name)
    {
        var ordinal = reader.GetOrdinal(name);
        return reader.IsDBNull(ordinal) ? null : reader.GetInt64(ordinal);
    }

    private static int? ReadNullableInt32(SqliteDataReader reader, string name)
    {
        var value = ReadNullableInt64(reader, name);
        return value.HasValue ? checked((int)value.Value) : null;
    }

    private static T ParseEnum<T>(string value) where T : struct, Enum =>
        Enum.TryParse<T>(value, false, out var parsed)
            ? parsed
            : throw new InvalidOperationException($"Unsupported {typeof(T).Name}: {value}");

    private static DateTimeOffset ParseUtc(string value)
    {
        var parsed = DateTimeOffset.Parse(value, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);
        if (parsed.Offset != TimeSpan.Zero)
            throw new InvalidOperationException("Usage event timestamp is not UTC.");
        return parsed;
    }

    private static void AddNullable(SqliteCommand command, string name, object? value) =>
        command.Parameters.AddWithValue(name, value ?? DBNull.Value);
}
