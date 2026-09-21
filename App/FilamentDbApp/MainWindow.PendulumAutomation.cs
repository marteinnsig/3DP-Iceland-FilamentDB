using FilamentDbApp.Models;
using FilamentDbApp.Services;
using Microsoft.Data.Sqlite;
using System.Reflection;
using System.Text.Json;

namespace FilamentDbApp;

public partial class MainWindow
{


    private void CreateAuthorizedAutomationPendulumRuns(string materialId)
    {
        AutomationRuntimeProfile.DemandMaterialCrudAuthorized(materialId);
        if (_pendulumRuns.Any(run => run.MaterialID == materialId))
            throw new InvalidOperationException("Pendulum CRUD refuses an existing graph for the disposable MaterialID.");
        RefreshFastPendulumViews();
        foreach (var workspace in _pendulumWorkspaces)
        {
            ApplyAuthorizedPendulumUiFields(materialId, workspace,
                ("TestNotes", "Disposable CRUD fixture; not owner measurements."), ("Sample3", "NB"));
            VerifyAuthorizedPendulumCoverage(materialId, workspace.Method, expected: false);
            ApplyAuthorizedPendulumUiFields(materialId, workspace, ("Sample1", "0"));
            VerifyAuthorizedPendulumCoverage(materialId, workspace.Method, expected: true);
            ApplyAuthorizedPendulumUiFields(materialId, workspace,
                ("Sample1", "10"), ("Sample2", "20"), ("Sample3", "NB"),
                ("TestNotes", "Disposable CRUD fixture; not owner measurements."));
        }
        VerifyAuthorizedPendulumRestartState(materialId, edited: false);
    }

    private void EditAuthorizedAutomationPendulumRuns(string materialId)
    {
        AutomationRuntimeProfile.DemandMaterialCrudAuthorized(materialId);
        VerifyAuthorizedPendulumRestartState(materialId, edited: false);
        RefreshFastPendulumViews();
        foreach (var workspace in _pendulumWorkspaces)
        {
            ApplyAuthorizedPendulumUiFields(materialId, workspace, ("Sample1", ""), ("Sample2", ""));
            VerifyAuthorizedPendulumCoverage(materialId, workspace.Method, expected: false);
            ApplyAuthorizedPendulumUiFields(materialId, workspace, ("Sample1", "15"), ("Sample2", ""));
            VerifyAuthorizedPendulumCoverage(materialId, workspace.Method, expected: true);
        }
        VerifyAuthorizedPendulumRestartState(materialId, edited: true);
    }
    private void DeleteAuthorizedAutomationPendulumRuns(string materialId)
    {
        AutomationRuntimeProfile.DemandMaterialCrudAuthorized(materialId);
        VerifyAuthorizedPendulumRestartState(materialId, edited: true);
        var graph = _database.LoadPendulumImpactGraph();
        var fixtureRuns = graph.Runs.Where(run => run.MaterialID == materialId).ToArray();
        if (fixtureRuns.Length != 2 || fixtureRuns.Any(run => run.InputMode != "DirectStrength" || run.Notes != "Disposable CRUD fixture; not owner measurements.") ||
            fixtureRuns.Select(run => run.Method).Distinct().Count() != 2)
            throw new InvalidOperationException("Pendulum cleanup refuses unexpected run ownership.");
        var ids = fixtureRuns.Select(run => run.RunId).ToHashSet(StringComparer.Ordinal);
        string Unrelated((List<PendulumImpactRunRecord> Runs, List<PendulumImpactSpecimenRecord> Specimens) value) =>
            JsonSerializer.Serialize(new { Runs = value.Runs.Where(run => !ids.Contains(run.RunId)).OrderBy(run => run.RunId),
                Specimens = value.Specimens.Where(specimen => !ids.Contains(specimen.RunId)).OrderBy(specimen => specimen.SpecimenId) });
        var before = Unrelated(graph);
        // Delete exact authorized fixture IDs only; no unrelated graph is submitted to persistence.
        using (var connection = new SqliteConnection(new SqliteConnectionStringBuilder { DataSource = _database.DatabasePath, ForeignKeys = true }.ToString()))
        {
            connection.Open();
            using var transaction = connection.BeginTransaction();
            foreach (var run in fixtureRuns)
            {
                using var command = connection.CreateCommand(); command.Transaction = transaction;
                command.CommandText = "DELETE FROM PendulumImpactRuns WHERE RunId=$run AND MaterialID=$material AND Method=$method;";
                command.Parameters.AddWithValue("$run", run.RunId); command.Parameters.AddWithValue("$material", materialId);
                command.Parameters.AddWithValue("$method", run.Method);
                if (command.ExecuteNonQuery() != 1) throw new InvalidOperationException("Pendulum fixture cleanup ownership mismatch.");
            }
            transaction.Commit();
        }
        var after = _database.LoadPendulumImpactGraph();
        if (after.Runs.Any(run => ids.Contains(run.RunId)) || after.Specimens.Any(specimen => ids.Contains(specimen.RunId)) || before != Unrelated(after))
            throw new InvalidOperationException("Pendulum cleanup failed exact unrelated-graph recovery.");
        _pendulumRuns.RemoveAll(run => ids.Contains(run.RunId));
        _pendulumSpecimens.RemoveAll(specimen => ids.Contains(specimen.RunId));
        RefreshFastPendulumViews();
    }

    private void ApplyAuthorizedPendulumUiFields(string materialId, PendulumWorkspace workspace, params (string Property, string Value)[] fields)
    {
        AutomationRuntimeProfile.DemandMaterialCrudAuthorized(materialId);
        var view = workspace.View ?? throw new InvalidOperationException("Direct pendulum view missing.");
        const BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;
        var rows = (List<MaterialsPrototypeRow>)typeof(MaterialsRenderingPrototypeView).GetField("_rows", flags)!.GetValue(view)!;
        var columns = (List<MaterialsPrototypeColumn>)typeof(MaterialsRenderingPrototypeView).GetField("_columns", flags)!.GetValue(view)!;
        var row = rows.Single(item => item.Source is DirectPendulumRow direct && direct.Material.MaterialID == materialId && direct.Method == workspace.Method);
        var changes = fields.Select(field =>
        {
            var index = columns.FindIndex(column => column.PropertyName == field.Property);
            if (index < 0 || columns[index].IsReadOnly) throw new InvalidOperationException("Requested pendulum automation field is not editable.");
            return new MaterialsPrototypeChange(row, columns[index], index, row.Cells[index], field.Value);
        }).ToList();
        var commit = (Func<IReadOnlyList<MaterialsPrototypeChange>, bool>)typeof(MaterialsRenderingPrototypeView)
            .GetField("_applyChanges", flags)!.GetValue(view)!;
        if (!commit(changes)) throw new InvalidOperationException("Pendulum fast-grid commit rejected: " + workspace.Status.Text);
        RefreshNativeMaterialTestStatusFromNativeInputTabs(markDirty: false);
        var beforeRejected = JsonSerializer.Serialize(_database.LoadPendulumImpactGraph(), new JsonSerializerOptions { IncludeFields = true });
        var canonicalBefore = JsonSerializer.Serialize(new { Runs = _pendulumRuns, Specimens = _pendulumSpecimens });
        var sampleIndex = columns.FindIndex(column => column.PropertyName == "Sample1");
        var invalid = new MaterialsPrototypeChange(row, columns[sampleIndex], sampleIndex, row.Cells[sampleIndex], "-1");
        if (commit([invalid]) || beforeRejected != JsonSerializer.Serialize(_database.LoadPendulumImpactGraph(), new JsonSerializerOptions { IncludeFields = true }) ||
            canonicalBefore != JsonSerializer.Serialize(new { Runs = _pendulumRuns, Specimens = _pendulumSpecimens }))
            throw new InvalidOperationException("Rejected direct measurement mutated canonical or persisted values.");
        workspace.Status.Text = "Saved to SQLite";
        view.SynchronizeFromCanonical("authorized CRUD commit");
    }

    private void VerifyAuthorizedPendulumRestartState(string materialId, bool edited)
    {
        AutomationRuntimeProfile.DemandMaterialCrudAuthorized(materialId);
        var graph = _database.LoadPendulumImpactGraph();
        foreach (var method in new[] { "Izod", "Charpy" })
        {
            VerifyAuthorizedPendulumCoverage(materialId, method, expected: true);
            var run = graph.Runs.Single(row => row.MaterialID == materialId && row.Method == method && row.InputMode == "DirectStrength");
            var specimens = graph.Specimens.Where(row => row.RunId == run.RunId).OrderBy(row => row.Label, StringComparer.Ordinal).ToArray();
            if (specimens.Length != 10 || specimens[0].StrengthKjM2Raw != (edited ? "15" : "10") ||
                specimens[1].StrengthKjM2Raw != (edited ? "" : "20") || specimens[2].StrengthKjM2Raw != "NB" ||
                specimens[2].BreakType != "No break" || specimens.Skip(3).Any(specimen => specimen.StrengthKjM2Raw.Length != 0) ||
                specimens.Any(specimen => specimen.EnergyJ.Length != 0))
                throw new InvalidOperationException("Direct instrument raw values, blanks or no-break did not survive SQLite reload.");
            var summary = PendulumImpactService.Summarize(run, specimens);
            if (summary.ValidCount != (edited ? 1 : 2) || summary.Mean != 15d || summary.NoBreakCount != 1 ||
                summary.TargetCount != 10 || summary.Confidence != (edited ? 1 : 2) ||
                (!edited && (summary.CvPercent is null || Math.Abs(summary.CvPercent.Value - Math.Sqrt(50) / 15d * 100d) > 0.000001)) ||
                (edited ? summary.SampleStdDev is not null : summary.SampleStdDev is null || Math.Abs(summary.SampleStdDev.Value - Math.Sqrt(50)) > 0.000001))
                throw new InvalidOperationException("Direct method-isolated sample summary failed after reload.");
        }
        RenderPendulumDetails(materialId);
        if (!PendulumDetailSummaryText.Text.Contains("Izod: 15", StringComparison.Ordinal) ||
            !PendulumDetailSummaryText.Text.Contains("Charpy: 15", StringComparison.Ordinal) ||
            !DashboardIzodMeanText.Text.Contains("15", StringComparison.Ordinal) ||
            !DashboardCharpyMeanText.Text.Contains("15", StringComparison.Ordinal))
            throw new InvalidOperationException("Shared detail summaries did not project persisted Izod and Charpy means.");
    }

    private void VerifyAuthorizedPendulumCoverage(string materialId, string method, bool expected)
    {
        AutomationRuntimeProfile.DemandMaterialCrudAuthorized(materialId);
        RefreshNativeMaterialTestStatusFromNativeInputTabs(markDirty: false);
        var material = _nativeMaterialRows.Single(row => row.MaterialID == materialId);
        var membership = method == "Izod" ? material.InIzod : material.InCharpy;
        var summary = BuildVerifiedMaterialSummaryMap([material])[materialId];
        var result = method == "Izod" ? summary.Izod : summary.Charpy;
        var profile = _scoringService.BuildProfile(summary, summary.HeatTemperatureC);
        var score = method == "Izod" ? profile.IzodScore : profile.CharpyScore;
        var expectedStatus = summary.ResultModuleCount == 0 ? "Not tested" : summary.ResultModuleCount == 6 ? "Fully tested" : "Partially tested";
        if (membership != (expected ? "Yes" : "No") || (result?.HasResults == true) != expected ||
            (expected ? score != result?.Score : score is not null) ||
            material.TestedStatus != expectedStatus)
            throw new InvalidOperationException("Pendulum membership, shared summary, radar score or six-module status drifted for " + method);
        if (NativeMaterialMatchesTestedFilter(material, method) != expected)
            throw new InvalidOperationException("Pendulum Materials test filter disagrees with canonical membership.");
    }
}
