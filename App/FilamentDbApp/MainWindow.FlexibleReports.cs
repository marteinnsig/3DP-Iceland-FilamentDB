using FilamentDbApp.Models;
using FilamentDbApp.Services.Reporting;
using System.Data;
using System.Text;

namespace FilamentDbApp;

public partial class MainWindow
{
    private IReadOnlyList<PublicFlexibleMetricGroup> GetFlexibleReportResults(string materialId)
    {
        _flexibleMaterialEvidence.TryGetValue(materialId.Trim(), out var evidence);
        return FlexibleReportEvidenceService.Build(evidence);
    }

    private string BuildFlexibleReportHtml(IReadOnlyList<DataRow> rows)
    {
        var content = new StringBuilder();
        foreach (var row in rows.GroupBy(row => GetCell(row, "Material ID", "MaterialID").Trim(), StringComparer.OrdinalIgnoreCase).Select(group => group.First()))
        {
            var groups = GetFlexibleReportResults(GetCell(row, "Material ID", "MaterialID"));
            if (groups.Count == 0) continue;
            content.Append("<section class=\"flexible-material-report\"><h3>").Append(Html(_detailService.BuildTitle(row))).Append("</h3>")
                .Append(FlexibleReportEvidenceService.RenderHtml(groups)).Append("</section>");
        }
        return content.ToString();
    }

    private void AppendFlexibleReportText(StringBuilder text, IReadOnlyList<DataRow> rows)
    {
        foreach (var row in rows.GroupBy(row => GetCell(row, "Material ID", "MaterialID").Trim(), StringComparer.OrdinalIgnoreCase).Select(group => group.First()))
        {
            var groups = GetFlexibleReportResults(GetCell(row, "Material ID", "MaterialID"));
            if (groups.Count == 0) continue;
            text.AppendLine().AppendLine(_detailService.BuildTitle(row)).AppendLine(FlexibleReportEvidenceService.RenderText(groups));
        }
    }
}
