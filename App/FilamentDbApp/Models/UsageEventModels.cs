namespace FilamentDbApp.Models;

public enum UsageEventType
{
    TestPreparation,
    TestPrint,
    ProductionPrint,
    InventoryAdjustment
}

public enum UsageQuantityProvenance
{
    NotRecorded,
    MeasuredActual,
    SlicerEstimate
}

public enum UsageEventEntryKind
{
    Original,
    Reversal,
    Replacement
}

public sealed record UsageEventRecord
{
    public required string UsageEventId { get; init; }
    public required string MaterialId { get; init; }
    public UsageEventType EventType { get; init; }
    public UsageEventEntryKind EntryKind { get; init; } = UsageEventEntryKind.Original;
    public DateTimeOffset OccurredAtUtc { get; init; }
    public DateTimeOffset CreatedAtUtc { get; init; }
    public string? InventoryItemId { get; init; }
    public string? ExperimentalRunId { get; init; }
    public string? FuturePrintJobId { get; init; }
    public string? FutureTestSessionId { get; init; }
    public decimal? FilamentUsedGrams { get; init; }
    public UsageQuantityProvenance FilamentProvenance { get; init; }
    public long? PrintDurationSeconds { get; init; }
    public long? HandsOnDurationSeconds { get; init; }
    public int? ProducedCount { get; init; }
    public int? AcceptedCount { get; init; }
    public int? RejectedCount { get; init; }
    public string Source { get; init; } = string.Empty;
    public string Note { get; init; } = string.Empty;
    public string Origin { get; init; } = string.Empty;
    public string? ReversesUsageEventId { get; init; }
    public string? CorrectsUsageEventId { get; init; }
}

public sealed record UsageInventoryIdentity(string InventoryItemId, string MaterialId);

public sealed record UsageEventValidationResult(bool IsValid, IReadOnlyList<string> Errors);

public sealed record UsageEventCorrection(UsageEventRecord Reversal, UsageEventRecord Replacement);

public sealed record UsageInventoryDelta(
    string InventoryItemId,
    string MaterialId,
    decimal RemainingWeightDeltaGrams,
    string UsageEventId);

public sealed record UsageEventProjection(
    string MaterialId,
    decimal? FilamentUsedGrams,
    long? PrintDurationSeconds,
    long? HandsOnDurationSeconds,
    int? ProducedCount,
    int? AcceptedCount,
    int? RejectedCount,
    int LedgerRowCount,
    int EffectiveEventCount,
    int FilamentEvidenceEventCount,
    int PrintDurationEvidenceEventCount,
    int HandsOnDurationEvidenceEventCount);
