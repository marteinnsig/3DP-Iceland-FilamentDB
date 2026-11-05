using FilamentDbApp.Models;
using FilamentDbApp.Services.Reporting;
using System.Data;
using FilamentDbApp.Services.Website;
using System.Text;

namespace FilamentDbApp;

public partial class MainWindow
{
    private IReadOnlyList<PublicPendulumImpactGroup> GetPendulumReportResults(string materialId)
    {
        // Read persisted SQLite inputs; pending editors are never report sources.
        var graph = _database.LoadPendulumImpactGraph();
        return PendulumImpactReportService.Build(materialId, graph.Runs, graph.Specimens);
    }

    private string BuildPendulumReportHtml(IReadOnlyList<DataRow> rows)
    {
        var content = new StringBuilder();
        foreach (var row in rows.GroupBy(r => GetCell(r, "Material ID", "MaterialID").Trim(), StringComparer.OrdinalIgnoreCase).Select(g => g.First()))
        {
            var groups = GetPendulumReportResults(GetCell(row, "Material ID", "MaterialID"));
            if (groups.Count > 0) content.Append("<h3>").Append(Html(_detailService.BuildTitle(row))).Append("</h3>")
                .Append(PendulumImpactReportService.RenderHtml(groups));
        }
        return content.ToString();
    }

    private void AppendPendulumReportText(StringBuilder text, IReadOnlyList<DataRow> rows)
    {
        foreach (var row in rows.GroupBy(r => GetCell(r, "Material ID", "MaterialID").Trim(), StringComparer.OrdinalIgnoreCase).Select(g => g.First()))
        {
            var groups = GetPendulumReportResults(GetCell(row, "Material ID", "MaterialID"));
            if (groups.Count > 0) text.AppendLine().AppendLine(_detailService.BuildTitle(row)).AppendLine(PendulumImpactReportService.RenderText(groups));
        }
    }

    private MaterialDetailGroup BuildPendulumMaterialDetailGroup(string materialId) => new("Izod / Charpy Impact Strength",
        [new MaterialDetailField("Separate saved measurement summaries", GetPendulumReportResults(materialId) is { Count: > 0 } groups
            ? PendulumImpactReportService.RenderText(groups) : "No saved Izod or Charpy results.")]);

    private IReadOnlyList<WebsiteChartMaterialInput> BuildPendulumWebsiteChartInputs(
        IReadOnlyList<NativeMaterialRow> rows, IReadOnlyDictionary<string, FilamentDbApp.Services.Calculations.MaterialResults> summaries)
    {
        var graph = _database.LoadPendulumImpactGraph();
        return rows.Where(row => summaries.ContainsKey(row.MaterialID.Trim()))
            .Select(row => new WebsiteChartMaterialInput(BuildWebsiteTemplateCommonFields(row), summaries[row.MaterialID.Trim()])
            {
                PendulumResults = PendulumImpactReportService.Build(row.MaterialID, graph.Runs, graph.Specimens)
            }).ToArray();
    }
}
