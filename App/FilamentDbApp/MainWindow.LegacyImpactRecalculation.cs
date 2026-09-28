using System.Globalization;
using System.Windows;
using FilamentDbApp.Services.Calculations;

namespace FilamentDbApp;

public partial class MainWindow
{
    // Read-only projection: the persistence adapter preserves historical stored results
    // when raw readings are unchanged. Only the explicit audited action replaces them.
    private void RefreshLegacyExperimentalImpactProjection()
    {
        foreach (var row in _experimentalMeasurementRows.Where(x => x.MeasurementType == "Impact"))
        {
            var settings = new LegacyImpactSettings(GetNativeImpactAvailableEnergyJ(), GetNativeImpactNetAreaMm2(), GetNativeImpactNoSampleAngle());
            var valid = row.RawUnit == "%" && row.ResultUnit == "kJ/m²" &&
                row.Orientation is "Flat" or "Upright" &&
                row.SampleValues().All(raw => string.IsNullOrWhiteSpace(raw) ||
                    (TryParseMeasurement(raw, out var percent) && LegacyImpactCalculationService.TryCalculate(percent, settings) is not null));
            var result = _resultsService.CalculateImpact(valid ? row.SampleValues() : Array.Empty<string>(),
                Array.Empty<string>(), settings.NoSampleAngleDegrees, settings.NetAreaMm2, settings.AvailableEnergyJ).Upright;
            row.ResultAverage = FormatNativeResult(result.Average);
            row.ResultStdDev = FormatNativeResult(result.StandardDeviation);
            row.ResultCv = FormatNativeResult(result.CoefficientOfVariation * 100d);
            row.ResultCount = result.SampleCount == 0 ? "" : result.SampleCount.ToString(CultureInfo.CurrentCulture);
            row.ResultConfidence = result.Confidence?.ToString(CultureInfo.CurrentCulture) ?? "";
            row.NotifyCalculated();
        }
    }

    private void LegacyImpactRecalculation_Click(object sender, RoutedEventArgs e)
    {
        try
        {
            var preview = _database.PreviewLegacyImpactRecalculation();
            var calibration = preview.Settings;
            var example = preview.Groups.FirstOrDefault(g => g.Ready);
            var message = "Read-only preview from saved SQLite readings and calibration.\n\n" +
                $"Valid readings: {preview.ValidReadingCount}; invalid: {preview.InvalidReadingCount}\n" +
                $"Native orientation groups: {preview.NativeGroupsRecalculated}; pending groups: {preview.PendingGroups}\n" +
                $"Experimental rows eligible: {preview.Groups.Count(g => g.Kind == "Experimental" && g.Ready)}\n\n" +
                $"Available energy: {calibration?.AvailableEnergyJ:G12} J\nNet area: {calibration?.NetAreaMm2:G12} mm²\n" +
                $"No-sample angle: {calibration?.NoSampleAngleDegrees:G12} degrees\n\n" +
                (example is null ? "No eligible group.\n" : $"Example {example.MaterialId} {example.Orientation} mean: {example.Before.Average:0.######} → {example.After.Average:0.######} kJ/m²\n") +
                string.Join("\n", preview.CalibrationIssues) + "\n\n" +
                "Yes: create a verified database backup and detailed JSON audit, then replace eligible Experimental derived results. " +
                "Native results are calculated from raw readings at runtime. Raw readings remain unchanged. " +
                "Pending groups remain untouched. No: close without changes.\n\nBack up and recalculate?";
            if (MessageBox.Show(this, message, "Legacy Impact Recalculation", MessageBoxButton.YesNo,
                    MessageBoxImage.Question, MessageBoxResult.No) != MessageBoxResult.Yes) return;
            var audit = _database.ApplyLegacyImpactRecalculation(preview);
            ApplyNativeImpactComputedFields(_nativeImpactRows);
            RefreshLegacyExperimentalImpactProjection();
            RefreshExperimentalSeriesResults();
            RefreshNativeImpactSummary();
            MessageBox.Show(this, $"Updated Experimental rows: {audit.UpdatedExperimentalRows}\n" +
                $"Native groups recalculated: {audit.NativeGroupsRecalculated}\nPending groups: {audit.PendingGroups}\n\n" +
                $"Backup: {audit.BackupPath}\nAudit: {audit.AuditPath}\n\n{audit.ApplyState}",
                "Legacy Impact Recalculation", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Legacy Impact Recalculation", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }
}
