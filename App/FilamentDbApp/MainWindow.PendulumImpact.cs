using FilamentDbApp.Models;
using FilamentDbApp.Services;
using System.Globalization;
using System.Text.Json;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;

namespace FilamentDbApp;

public partial class MainWindow
{
    private readonly List<PendulumImpactRunRecord> _pendulumRuns = new();
    private readonly List<PendulumImpactSpecimenRecord> _pendulumSpecimens = new();
    private readonly List<PendulumWorkspace> _pendulumWorkspaces = new();
    private bool _switchingPendulumTab;

    private sealed record DirectPendulumRow(NativeMaterialRow Material, string Method);
    private sealed class PendulumWorkspace
    {
        public required string Method { get; init; }
        public Dictionary<string, DirectPendulumRow> Rows { get; } = new(StringComparer.OrdinalIgnoreCase);
        public MaterialsRenderingPrototypeView? View { get; set; }
        public TextBlock Status { get; } = new() { Margin = new Thickness(4), TextWrapping = TextWrapping.Wrap };
    }

    private void InitializePendulumImpact()
    {
        if (_pendulumWorkspaces.Count > 0) return;
        var graph = _database.LoadPendulumImpactGraph();
        _pendulumRuns.AddRange(graph.Runs);
        _pendulumSpecimens.AddRange(graph.Specimens);
        foreach (var method in new[] { "Izod", "Charpy" })
        {
            var workspace = new PendulumWorkspace { Method = method };
            _pendulumWorkspaces.Add(workspace);
            var root = new DockPanel { Margin = new Thickness(8) };
            var description = new TextBlock { Text = "Enter the instrument reading directly in kJ/m². Ten samples per material; summary values are read-only. Specimen dimensions are in Settings Manager.", Margin = new Thickness(4), TextWrapping = TextWrapping.Wrap };
            DockPanel.SetDock(description, Dock.Top); root.Children.Add(description);
            DockPanel.SetDock(workspace.Status, Dock.Top); root.Children.Add(workspace.Status);
            var columns = ApplyFastMaterialsLayout(BuildFastPendulumColumns(), _workflowPreferencesService.GetFastGridLayout(method + "DirectImpact"));
            workspace.View = new MaterialsRenderingPrototypeView(columns, BuildFastPendulumRows(workspace, columns),
                changes => ApplyPendulumChanges(workspace, changes),
                layout => _workflowPreferencesService.SetFastGridLayout(method + "DirectImpact", layout),
                cols => BuildFastPendulumRows(workspace, cols), _ => { },
                directCanonicalEditing: true, reloadAfterApply: false, flexibleNavigation: true,
                cellText: (source, property) => DirectPendulumCellText((DirectPendulumRow)source, property));
            AutomationProperties.SetAutomationId(workspace.View, method + "ImpactMeasurementsGrid");
            root.Children.Add(workspace.View);
            ((ContentControl)FindName(method + "ImpactViewHost")).Content = root;
        }
        WorkspaceTabs.SelectionChanged += (_, args) =>
        {
            if (_switchingPendulumTab || !ReferenceEquals(args.Source, WorkspaceTabs)) return;
            if (!FlushPendulumImpactEditors())
            {
                _switchingPendulumTab = true;
                try { if (args.RemovedItems.Count > 0) WorkspaceTabs.SelectedItem = args.RemovedItems[0]; }
                finally { _switchingPendulumTab = false; }
                return;
            }
            RefreshFastPendulumViews();
            FlushPendulumIntelligenceRefresh();
        };
    }

    private static List<MaterialsPrototypeColumn> BuildFastPendulumColumns()
    {
        var columns = BuildFastMeasurementIdentityColumns();
        columns.Add(FastMeasurementColumn("", 35, null, true, FastGridCellKind.Spacer));
        for (var index = 1; index <= 10; index++)
            columns.Add(FastMeasurementColumn($"Sample {index}", 70, $"Sample{index}", false));
        columns.Add(FastMeasurementColumn("", 35, null, true, FastGridCellKind.Spacer));
        columns.Add(FastMeasurementColumn("Test Notes", 220, "TestNotes", false));
        columns.Add(FastMeasurementColumn("Measured date", 110, "MeasuredDateText", false));
        foreach (var (header, property) in new[] { ("Mean (kJ/m²)", "Mean"), ("Std Dev", "StdDev"), ("CV %", "Cv"), ("Samples", "Samples"), ("Confidence", "Confidence"), ("Validation", "Validation") })
            columns.Add(FastMeasurementColumn(header, 110, property, true, FastGridCellKind.Computed));
        return AssignStablePrototypeLayoutKeys(columns);
    }

    private List<MaterialsPrototypeRow> BuildFastPendulumRows(PendulumWorkspace workspace, IReadOnlyList<MaterialsPrototypeColumn> columns)
    {
        var visible = GetVisibleNativeMaterialIdsFromCurrentFilters();
        return _nativeMaterialRows.Where(material => visible.Contains(material.MaterialID))
            .OrderBy(material => material.MaterialID, CanonicalMaterialIdComparer).Select(material =>
            {
                if (!workspace.Rows.TryGetValue(material.MaterialID, out var row) || !ReferenceEquals(row.Material, material))
                    workspace.Rows[material.MaterialID] = row = new DirectPendulumRow(material, workspace.Method);
                var cells = columns.Select(column => DirectPendulumCellText(row, column.PropertyName)).ToArray();
                return new MaterialsPrototypeRow(row, material.MaterialID, cells, cells.ToArray(), () => true);
            }).ToList();
    }

    private PendulumImpactRunRecord? FindDirectPendulumRun(DirectPendulumRow row) =>
        _pendulumRuns.SingleOrDefault(run => run.MaterialID == row.Material.MaterialID && run.Method == row.Method && run.InputMode == "DirectStrength");

    private string DirectPendulumCellText(DirectPendulumRow row, string? property)
    {
        if (property is null) return "";
        var run = FindDirectPendulumRun(row);
        var specimens = run is null ? new List<PendulumImpactSpecimenRecord>() : _pendulumSpecimens.Where(item => item.RunId == run.RunId).OrderBy(item => item.Label, StringComparer.Ordinal).ToList();
        if (property.StartsWith("Sample", StringComparison.Ordinal) && int.TryParse(property[6..], out var index))
            return specimens.ElementAtOrDefault(index - 1)?.StrengthKjM2Raw ?? "";
        if (property == "TestNotes") return run?.Notes ?? "";
        if (property == "MeasuredDateText") return DateTime.TryParseExact(run?.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var measured) ? measured.ToString("d", CultureInfo.CurrentCulture) : "";
        if (property == "Validation") return "OK";
        if (property is "Mean" or "StdDev" or "Cv" or "Samples" or "Confidence")
        {
            if (run is null) return "";
            var summary = PendulumImpactService.Summarize(run, specimens);
            return property switch
            {
                "Mean" => PendulumNumber(summary.Mean), "StdDev" => PendulumNumber(summary.SampleStdDev),
                "Cv" => PendulumNumber(summary.CvPercent), "Samples" => summary.ValidCount.ToString(CultureInfo.CurrentCulture),
                "Confidence" => summary.Confidence?.ToString(CultureInfo.CurrentCulture) ?? "", _ => ""
            };
        }
        return PrototypeCellText(row.Material, property);
    }

    private bool ApplyPendulumChanges(PendulumWorkspace workspace, IReadOnlyList<MaterialsPrototypeChange> changes)
    {
        var runs = ClonePendulum(_pendulumRuns);
        var specimens = ClonePendulum(_pendulumSpecimens);
        foreach (var change in changes)
        {
            if (change.Column.IsReadOnly || change.Row.Source is not DirectPendulumRow row || row.Method != workspace.Method) return false;
            var run = runs.SingleOrDefault(item => item.MaterialID == row.Material.MaterialID && item.Method == row.Method && item.InputMode == "DirectStrength");
            if (run is null)
            {
                run = CreateDirectPendulumRunProfile(row.Material.MaterialID, row.Method);
                run.RunId = Guid.NewGuid().ToString("N"); run.InputMode = "DirectStrength";
                runs.Add(run);
                for (var index = 1; index <= 10; index++)
                    specimens.Add(new PendulumImpactSpecimenRecord { SpecimenId = Guid.NewGuid().ToString("N"), RunId = run.RunId, Label = index.ToString("00"), CreatedAtUtc = DateTime.UtcNow.ToString("O") });
            }
            var property = change.Column.PropertyName ?? "";
            if (property.StartsWith("Sample", StringComparison.Ordinal) && int.TryParse(property[6..], out var sampleIndex) && sampleIndex is >= 1 and <= 10)
            {
                var specimen = specimens.Where(item => item.RunId == run.RunId).OrderBy(item => item.Label, StringComparer.Ordinal).ElementAt(sampleIndex - 1);
                specimen.StrengthKjM2Raw = change.NewValue.Trim();
                specimen.BreakType = string.IsNullOrWhiteSpace(specimen.StrengthKjM2Raw) ? "Unmeasured" :
                    specimen.StrengthKjM2Raw.Equals("NB", StringComparison.OrdinalIgnoreCase) || specimen.StrengthKjM2Raw.Equals("No break", StringComparison.OrdinalIgnoreCase) ? "No break" : "Complete";
                specimen.UpdatedAtUtc = DateTime.UtcNow.ToString("O");
                if (string.IsNullOrEmpty(run.Date) && !string.IsNullOrWhiteSpace(specimen.StrengthKjM2Raw)) run.Date = DateTime.Today.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else if (property == "TestNotes") run.Notes = change.NewValue;
            else if (property == "MeasuredDateText")
            {
                if (!string.IsNullOrWhiteSpace(change.NewValue) && !DateTime.TryParse(change.NewValue, CultureInfo.CurrentCulture, DateTimeStyles.None, out _))
                { workspace.Status.Text = "Enter a valid measured date or leave it blank."; return false; }
                run.Date = string.IsNullOrWhiteSpace(change.NewValue) ? "" : DateTime.Parse(change.NewValue, CultureInfo.CurrentCulture).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }
            else return false;
            run.UpdatedAtUtc = DateTime.UtcNow.ToString("O");
        }
        try
        {
            var errors = PendulumImpactService.Validate(runs, specimens);
            if (errors.Count > 0) { workspace.Status.Text = errors[0] + " Correct the input or press Escape."; return false; }
            _database.SynchronizePendulumImpactGraph(ClonePendulum(runs), ClonePendulum(specimens));
            // SQLite accepts the detached graph before canonical values change. Fast cell refresh retains source/editor identity.
            foreach (var run in runs)
            {
                var existing = _pendulumRuns.SingleOrDefault(item => item.RunId == run.RunId);
                if (existing is null) _pendulumRuns.Add(run); else CopyPendulum(run, existing);
            }
            foreach (var specimen in specimens)
            {
                var existing = _pendulumSpecimens.SingleOrDefault(item => item.SpecimenId == specimen.SpecimenId);
                if (existing is null) _pendulumSpecimens.Add(specimen); else CopyPendulum(specimen, existing);
            }
            workspace.Status.Text = "Saved to SQLite";
            QueuePendulumConsumerRefresh();
            return true;
        }
        catch (Exception ex) { workspace.Status.Text = "Save failed; original data retained: " + ex.Message; return false; }
    }

    private void RefreshFastPendulumViews()
    {
        foreach (var workspace in _pendulumWorkspaces) workspace.View?.SynchronizeFromCanonical("material filters", preserveActiveEditor: true);
    }
    private bool FlushPendulumImpactEditors() => _pendulumWorkspaces.All(workspace => workspace.View?.TryCommitActiveEditor() ?? true);
    private static T ClonePendulum<T>(T value) => JsonSerializer.Deserialize<T>(JsonSerializer.Serialize(value))!;
    private static void CopyPendulum(object source, object target)
    {
        foreach (var property in source.GetType().GetProperties().Where(property => property.CanRead && property.CanWrite)) property.SetValue(target, property.GetValue(source));
        if (target is PendulumImpactRecord record) record.Refresh();
    }
    private static string PendulumNumber(double? value) => value?.ToString("0.###", CultureInfo.CurrentCulture) ?? "";

    private bool VerifyPendulumInputContract()
    {
        var columns = BuildFastPendulumColumns();
        return columns.Count(column => column.PropertyName?.StartsWith("Sample", StringComparison.Ordinal) == true && !column.IsReadOnly) == 10 &&
            columns.Where(column => column.PropertyName is "Mean" or "StdDev" or "Cv" or "Samples" or "Confidence").All(column => column.IsReadOnly) &&
            MaterialsRenderingPrototypeView.RunEditorFocusContractVerification() && MaterialsRenderingPrototypeView.RunFlexibleNavigationContractVerification() &&
            _pendulumWorkspaces.Count == 2 && _pendulumWorkspaces.All(workspace => workspace.View is not null &&
                AutomationProperties.GetAutomationId(workspace.View) == workspace.Method + "ImpactMeasurementsGrid");
    }
}
