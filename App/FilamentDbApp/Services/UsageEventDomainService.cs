using FilamentDbApp.Models;
using System.Numerics;

namespace FilamentDbApp.Services;

public sealed class UsageEventDomainService
{
    public UsageEventValidationResult Validate(
        UsageEventRecord usageEvent,
        IEnumerable<string> canonicalMaterialIds,
        IEnumerable<UsageInventoryIdentity> inventory,
        IEnumerable<UsageEventRecord>? acceptedEvents = null)
    {
        ArgumentNullException.ThrowIfNull(usageEvent);
        ArgumentNullException.ThrowIfNull(canonicalMaterialIds);
        ArgumentNullException.ThrowIfNull(inventory);

        var errors = new List<string>();
        var materials = canonicalMaterialIds
            .Where(value => !string.IsNullOrWhiteSpace(value))
            .Select(value => value.Trim())
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var accepted = (acceptedEvents ?? Array.Empty<UsageEventRecord>()).ToList();

        if (string.IsNullOrWhiteSpace(usageEvent.UsageEventId))
            errors.Add("UsageEventId is required.");
        else if (accepted.Any(item => item.UsageEventId.Equals(
                     usageEvent.UsageEventId.Trim(),
                     StringComparison.OrdinalIgnoreCase)))
            errors.Add("UsageEventId already exists.");

        if (string.IsNullOrWhiteSpace(usageEvent.MaterialId) ||
            !materials.Contains(usageEvent.MaterialId.Trim()))
            errors.Add("MaterialId must identify one canonical Material.");

        if (usageEvent.OccurredAtUtc == default || usageEvent.OccurredAtUtc.Offset != TimeSpan.Zero)
            errors.Add("OccurredAtUtc must be recorded in UTC.");
        if (usageEvent.CreatedAtUtc == default || usageEvent.CreatedAtUtc.Offset != TimeSpan.Zero)
            errors.Add("CreatedAtUtc must be recorded in UTC.");

        ValidateInventoryRelationship(usageEvent, inventory, errors);
        ValidateQuantities(usageEvent, errors);
        ValidateEntryKind(usageEvent, accepted, errors);

        return new UsageEventValidationResult(errors.Count == 0, errors);
    }

    public UsageEventRecord CreateReversal(
        UsageEventRecord original,
        string reversalEventId,
        DateTimeOffset occurredAtUtc,
        DateTimeOffset createdAtUtc,
        string source,
        string note,
        IEnumerable<UsageEventRecord>? acceptedEvents = null)
    {
        ArgumentNullException.ThrowIfNull(original);
        if (original.EntryKind == UsageEventEntryKind.Reversal)
            throw new InvalidOperationException("A reversal cannot be reversed.");
        if ((acceptedEvents ?? Array.Empty<UsageEventRecord>()).Any(item =>
                string.Equals(
                    item.ReversesUsageEventId,
                    original.UsageEventId,
                    StringComparison.OrdinalIgnoreCase)))
            throw new InvalidOperationException("The original event already has a reversal.");

        return original with
        {
            UsageEventId = RequireIdentity(reversalEventId, nameof(reversalEventId)),
            EntryKind = UsageEventEntryKind.Reversal,
            OccurredAtUtc = occurredAtUtc,
            CreatedAtUtc = createdAtUtc,
            FilamentUsedGrams = Negate(original.FilamentUsedGrams),
            PrintDurationSeconds = Negate(original.PrintDurationSeconds),
            HandsOnDurationSeconds = Negate(original.HandsOnDurationSeconds),
            ProducedCount = Negate(original.ProducedCount),
            AcceptedCount = Negate(original.AcceptedCount),
            RejectedCount = Negate(original.RejectedCount),
            Source = source?.Trim() ?? string.Empty,
            Note = note?.Trim() ?? string.Empty,
            ReversesUsageEventId = original.UsageEventId,
            CorrectsUsageEventId = null
        };
    }

    public UsageEventCorrection CreateCorrection(
        UsageEventRecord original,
        UsageEventRecord replacement,
        string reversalEventId,
        DateTimeOffset correctedAtUtc,
        string correctionSource,
        string correctionNote,
        IEnumerable<UsageEventRecord>? acceptedEvents = null)
    {
        ArgumentNullException.ThrowIfNull(original);
        ArgumentNullException.ThrowIfNull(replacement);
        if (!string.Equals(
                original.MaterialId,
                replacement.MaterialId,
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("A correction cannot change MaterialId.");
        if (replacement.EntryKind == UsageEventEntryKind.Reversal)
            throw new InvalidOperationException("A replacement cannot be a reversal.");

        var reversal = CreateReversal(
            original,
            reversalEventId,
            correctedAtUtc,
            correctedAtUtc,
            correctionSource,
            correctionNote,
            acceptedEvents);
        return new UsageEventCorrection(
            reversal,
            replacement with
            {
                EntryKind = UsageEventEntryKind.Replacement,
                CorrectsUsageEventId = original.UsageEventId,
                ReversesUsageEventId = null
            });
    }

    public UsageInventoryDelta? BuildInventoryDelta(UsageEventRecord usageEvent)
    {
        ArgumentNullException.ThrowIfNull(usageEvent);
        if (string.IsNullOrWhiteSpace(usageEvent.InventoryItemId) ||
            !usageEvent.FilamentUsedGrams.HasValue)
            return null;

        return new UsageInventoryDelta(
            usageEvent.InventoryItemId.Trim(),
            usageEvent.MaterialId.Trim(),
            -usageEvent.FilamentUsedGrams.Value,
            usageEvent.UsageEventId.Trim());
    }

    public UsageEventProjection ProjectMaterial(
        string materialId,
        IEnumerable<UsageEventRecord> acceptedEvents)
    {
        ArgumentNullException.ThrowIfNull(acceptedEvents);
        var normalizedMaterialId = RequireIdentity(materialId, nameof(materialId));
        var events = acceptedEvents
            .Where(item => item.MaterialId.Equals(
                normalizedMaterialId,
                StringComparison.OrdinalIgnoreCase))
            .ToList();
        var reversedIds = events
            .Where(item =>
                item.EntryKind == UsageEventEntryKind.Reversal &&
                !string.IsNullOrWhiteSpace(item.ReversesUsageEventId))
            .Select(item => item.ReversesUsageEventId!)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);
        var effectiveEvents = events
            .Where(item =>
                item.EntryKind != UsageEventEntryKind.Reversal &&
                !reversedIds.Contains(item.UsageEventId))
            .ToList();

        return new UsageEventProjection(
            normalizedMaterialId,
            SumNullable(events.Select(item => item.FilamentUsedGrams)),
            SumNullable(events.Select(item => item.PrintDurationSeconds)),
            SumNullable(events.Select(item => item.HandsOnDurationSeconds)),
            SumNullable(events.Select(item => item.ProducedCount)),
            SumNullable(events.Select(item => item.AcceptedCount)),
            SumNullable(events.Select(item => item.RejectedCount)),
            events.Count,
            effectiveEvents.Count,
            effectiveEvents.Count(item => item.FilamentUsedGrams.HasValue),
            effectiveEvents.Count(item => item.PrintDurationSeconds.HasValue),
            effectiveEvents.Count(item => item.HandsOnDurationSeconds.HasValue));
    }

    private static void ValidateInventoryRelationship(
        UsageEventRecord usageEvent,
        IEnumerable<UsageInventoryIdentity> inventory,
        ICollection<string> errors)
    {
        if (string.IsNullOrWhiteSpace(usageEvent.InventoryItemId)) return;
        var matches = inventory
            .Where(item => item.InventoryItemId.Equals(
                usageEvent.InventoryItemId.Trim(),
                StringComparison.OrdinalIgnoreCase))
            .ToList();
        if (matches.Count != 1)
        {
            errors.Add("InventoryItemId must identify one exact inventory spool.");
            return;
        }

        if (!matches[0].MaterialId.Equals(
                usageEvent.MaterialId.Trim(),
                StringComparison.OrdinalIgnoreCase))
            errors.Add("InventoryItemId MaterialId must match the usage event MaterialId.");
    }

    private static void ValidateQuantities(
        UsageEventRecord usageEvent,
        ICollection<string> errors)
    {
        var isReversal = usageEvent.EntryKind == UsageEventEntryKind.Reversal;
        ValidateSigned(usageEvent.FilamentUsedGrams, isReversal, "FilamentUsedGrams", errors);
        ValidateSigned(usageEvent.PrintDurationSeconds, isReversal, "PrintDurationSeconds", errors);
        ValidateSigned(usageEvent.HandsOnDurationSeconds, isReversal, "HandsOnDurationSeconds", errors);
        ValidateSigned(usageEvent.ProducedCount, isReversal, "ProducedCount", errors);
        ValidateSigned(usageEvent.AcceptedCount, isReversal, "AcceptedCount", errors);
        ValidateSigned(usageEvent.RejectedCount, isReversal, "RejectedCount", errors);

        if (usageEvent.FilamentUsedGrams.HasValue &&
            usageEvent.FilamentProvenance == UsageQuantityProvenance.NotRecorded)
            errors.Add("Filament provenance is required when grams are recorded.");
        if (!usageEvent.FilamentUsedGrams.HasValue &&
            usageEvent.FilamentProvenance != UsageQuantityProvenance.NotRecorded)
            errors.Add("Filament provenance must be NotRecorded when grams are missing.");
        if (!isReversal &&
            usageEvent.AcceptedCount.HasValue &&
            usageEvent.ProducedCount.HasValue &&
            usageEvent.AcceptedCount > usageEvent.ProducedCount)
            errors.Add("AcceptedCount cannot exceed ProducedCount.");
        if (!isReversal &&
            usageEvent.RejectedCount.HasValue &&
            usageEvent.ProducedCount.HasValue &&
            usageEvent.RejectedCount > usageEvent.ProducedCount)
            errors.Add("RejectedCount cannot exceed ProducedCount.");
    }

    private static void ValidateEntryKind(
        UsageEventRecord usageEvent,
        IReadOnlyCollection<UsageEventRecord> accepted,
        ICollection<string> errors)
    {
        if (usageEvent.EntryKind == UsageEventEntryKind.Reversal)
        {
            if (string.IsNullOrWhiteSpace(usageEvent.ReversesUsageEventId))
            {
                errors.Add("A reversal must reference the original UsageEventId.");
                return;
            }

            var original = accepted.SingleOrDefault(item => item.UsageEventId.Equals(
                usageEvent.ReversesUsageEventId,
                StringComparison.OrdinalIgnoreCase));
            if (original is null)
                errors.Add("The reversal original was not found.");
            else if (!IsExactReversal(original, usageEvent))
                errors.Add("A reversal must negate the original quantities and retain its relationships exactly.");
            if (accepted.Any(item => item.ReversesUsageEventId?.Equals(
                    usageEvent.ReversesUsageEventId,
                    StringComparison.OrdinalIgnoreCase) == true))
                errors.Add("The original event already has a reversal.");
        }
        else if (!string.IsNullOrWhiteSpace(usageEvent.ReversesUsageEventId))
        {
            errors.Add("Only a reversal may set ReversesUsageEventId.");
        }

        if (usageEvent.EntryKind == UsageEventEntryKind.Replacement)
        {
            if (string.IsNullOrWhiteSpace(usageEvent.CorrectsUsageEventId))
            {
                errors.Add("A replacement must reference the corrected UsageEventId.");
                return;
            }

            var original = accepted.SingleOrDefault(item => item.UsageEventId.Equals(
                usageEvent.CorrectsUsageEventId,
                StringComparison.OrdinalIgnoreCase));
            if (original is null)
                errors.Add("The corrected original was not found.");
            else if (original.EntryKind == UsageEventEntryKind.Reversal ||
                     !original.MaterialId.Equals(
                         usageEvent.MaterialId,
                         StringComparison.OrdinalIgnoreCase))
                errors.Add("A replacement must retain the original MaterialId.");
        }
        else if (!string.IsNullOrWhiteSpace(usageEvent.CorrectsUsageEventId))
        {
            errors.Add("Only a replacement may set CorrectsUsageEventId.");
        }
    }

    private static bool IsExactReversal(
        UsageEventRecord original,
        UsageEventRecord reversal) =>
        original.EntryKind != UsageEventEntryKind.Reversal &&
        original.MaterialId.Equals(reversal.MaterialId, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(original.InventoryItemId, reversal.InventoryItemId, StringComparison.OrdinalIgnoreCase) &&
        string.Equals(original.ExperimentalRunId, reversal.ExperimentalRunId, StringComparison.OrdinalIgnoreCase) &&
        original.EventType == reversal.EventType &&
        reversal.FilamentUsedGrams == Negate(original.FilamentUsedGrams) &&
        reversal.PrintDurationSeconds == Negate(original.PrintDurationSeconds) &&
        reversal.HandsOnDurationSeconds == Negate(original.HandsOnDurationSeconds) &&
        reversal.ProducedCount == Negate(original.ProducedCount) &&
        reversal.AcceptedCount == Negate(original.AcceptedCount) &&
        reversal.RejectedCount == Negate(original.RejectedCount);

    private static void ValidateSigned<T>(
        T? value,
        bool isReversal,
        string name,
        ICollection<string> errors)
        where T : struct, INumber<T>
    {
        if (!value.HasValue) return;
        if (!isReversal && value.Value < T.Zero)
            errors.Add($"{name} cannot be negative.");
        if (isReversal && value.Value > T.Zero)
            errors.Add($"{name} must negate the original value.");
    }

    private static string RequireIdentity(string? value, string parameterName)
    {
        if (string.IsNullOrWhiteSpace(value))
            throw new ArgumentException("A stable identity is required.", parameterName);
        return value.Trim();
    }

    private static decimal? Negate(decimal? value) => value.HasValue ? -value.Value : null;
    private static long? Negate(long? value) => value.HasValue ? -value.Value : null;
    private static int? Negate(int? value) => value.HasValue ? -value.Value : null;

    private static decimal? SumNullable(IEnumerable<decimal?> values)
    {
        var recorded = values.Where(value => value.HasValue).Select(value => value!.Value).ToList();
        return recorded.Count == 0 ? null : recorded.Sum();
    }

    private static long? SumNullable(IEnumerable<long?> values)
    {
        var recorded = values.Where(value => value.HasValue).Select(value => value!.Value).ToList();
        return recorded.Count == 0 ? null : recorded.Sum();
    }

    private static int? SumNullable(IEnumerable<int?> values)
    {
        var recorded = values.Where(value => value.HasValue).Select(value => value!.Value).ToList();
        return recorded.Count == 0 ? null : recorded.Sum();
    }
}
