using System.Data;
using FilamentDbApp.Services.Website;

namespace FilamentDbApp;

public partial class MainWindow
{
    private string BuildFlexibleWebsitePage(IReadOnlyList<DataRow> rows) => new FlexibleWebsiteService().BuildPage(
        GetNativeWebsitePipelineRows(rows).Select(material =>
        {
            var id = material.MaterialID?.Trim() ?? string.Empty;
            _flexibleMaterialEvidence.TryGetValue(id, out var evidence);
            return new FlexibleWebsiteMaterialInput(id, BuildWebsiteTemplateCommonFields(material), evidence);
        }));
}
