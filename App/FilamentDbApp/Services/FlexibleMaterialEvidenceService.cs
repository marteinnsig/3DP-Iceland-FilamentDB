using FilamentDbApp.Models;

namespace FilamentDbApp.Services;

/// <summary>Pure MaterialID projection from active saved method snapshots; never edits raw readings.</summary>
public static class FlexibleMaterialEvidenceService
{
    public static FlexibleMaterialEvidenceSnapshot Build(
        string materialId,
        IReadOnlyCollection<FlexibleTestSessionRecord> sessions,
        IReadOnlyCollection<FlexibleTestSpecimenRecord> specimens,
        IReadOnlyCollection<CompressionPointRecord> compression,
        IReadOnlyCollection<StressRelaxationPointRecord> relaxation,
        IReadOnlyCollection<RecoveryMeasurementRecord> recovery,
        IReadOnlyCollection<ShoreHardnessReadingRecord> shore)
    {
        var id = materialId?.Trim() ?? string.Empty;
        // Ambiguous stable identities fail closed even when another duplicate belongs to a different material.
        var sessionIds = sessions.Where(x => !string.IsNullOrWhiteSpace(x.FlexibleTestSessionId))
            .GroupBy(x => x.FlexibleTestSessionId, StringComparer.OrdinalIgnoreCase)
            .Where(x => x.Count() == 1).Select(x => x.Single())
            .Where(x => id.Length > 0 && x.IsActive && string.Equals(x.MaterialID?.Trim(), id, StringComparison.OrdinalIgnoreCase))
            .Select(x => x.FlexibleTestSessionId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var selected = specimens.Where(x => !string.IsNullOrWhiteSpace(x.SpecimenId))
            .GroupBy(x => x.SpecimenId, StringComparer.OrdinalIgnoreCase).Where(x => x.Count() == 1)
            .Select(x => x.Single()).Where(x => sessionIds.Contains(x.FlexibleTestSessionId)).ToArray();
        var specimenIds = selected.Select(x => x.SpecimenId).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var groups = FlexibleMaterialTestingService.BuildMetricGroups(selected,
            compression.Where(x => specimenIds.Contains(x.SpecimenId)).ToArray(),
            shore.Where(x => specimenIds.Contains(x.SpecimenId)).ToArray(),
            recovery.Where(x => specimenIds.Contains(x.SpecimenId)).ToArray());
        return new(id, groups, sessionIds.Count, selected.Length,
            relaxation.Count(x => specimenIds.Contains(x.SpecimenId)));
    }
}
