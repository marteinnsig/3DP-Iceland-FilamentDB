using FilamentDbApp.Models;
using FilamentDbApp.Services;
using FilamentDbApp.Services.Calculations;
using System.Globalization;
using System.Windows.Threading;

namespace FilamentDbApp;

public partial class MainWindow
{
    private IReadOnlyDictionary<string, PendulumMaterialProjection>? _pendulumProjectionCache;
    private string _pendulumProjectionScope = "";
    private bool _pendulumConsumersQueued;
    private bool _pendulumIntelligencePending;

    private IReadOnlyDictionary<string, PendulumMaterialProjection> BuildPendulumProjections(IEnumerable<NativeMaterialRow> materials)
    {
        var graph = _database.LoadPendulumImpactGraph();
        return PendulumImpactProjectionService.Build(graph.Runs, graph.Specimens, materials.Select(row => row.MaterialID));
    }

    private PendulumMaterialProjection? GetPendulumProjection(string materialId)
    {
        var active = _nativeMaterialRows.Where(row => !row.IsArchived).ToArray();
        var scope = string.Join("|", active.Select(row => row.MaterialID));
        if (_pendulumProjectionCache is null || scope != _pendulumProjectionScope)
        {
            _pendulumProjectionCache = BuildPendulumProjections(active);
            _pendulumProjectionScope = scope;
        }
        return _pendulumProjectionCache.GetValueOrDefault(materialId);
    }

    private void AppendPendulumMetrics(string materialId, List<TestSummaryMetric> metrics)
    {
        var projection = GetPendulumProjection(materialId);
        foreach (var (method, result) in new[] { ("Izod", projection?.Izod), ("Charpy", projection?.Charpy) })
        {
            if (result is null) continue;
            void Add(string name, double? value, string unit)
            {
                if (value is null) return;
                metrics.Add(new TestSummaryMetric { TestType = method, MetricName = name,
                    MetricValue = value.Value.ToString("G17", CultureInfo.InvariantCulture), Unit = unit,
                    SourceSheet = "Canonical SQLite", SourceColumn = name });
            }
            Add("Mean kJ/m²", result.MeanKjM2, "kJ/m²");
            Add("Std Dev", result.Statistics.SampleStdDev, "kJ/m²");
            Add("CV", result.Statistics.CvPercent, "%");
            Add("Samples", result.Statistics.ValidCount, "");
            Add("Confidence", result.Statistics.Confidence, "");
            Add("Score", result.Score, "/100");
            Add("Reference maximum", result.ReferenceMaximumKjM2, "kJ/m²");
        }
    }

    private static string PendulumDetailText(string method, PendulumMethodResults? result) => result?.HasResults == true
        ? $"{method}: {PendulumNumber(result.MeanKjM2)} kJ/m² | Std Dev {PendulumNumber(result.Statistics.SampleStdDev)} | " +
          $"CV {PendulumNumber(result.Statistics.CvPercent)}% | Samples {result.Statistics.ValidCount} | Confidence {result.Statistics.Confidence} | {result.Date}"
        : $"{method}: not measured";

    private void RenderPendulumDetails(string materialId)
    {
        var p = GetPendulumProjection(materialId);
        PendulumDetailSummaryText.Text = PendulumDetailText("Izod", p?.Izod) + "\n" + PendulumDetailText("Charpy", p?.Charpy);
        DashboardIzodMeanText.Text = p?.Izod?.HasResults == true ? PendulumNumber(p.Izod.MeanKjM2) + " kJ/m²" : "—";
        DashboardCharpyMeanText.Text = p?.Charpy?.HasResults == true ? PendulumNumber(p.Charpy.MeanKjM2) + " kJ/m²" : "—";
        DashboardIzodStatisticsText.Text = PendulumDetailText("Izod", p?.Izod);
        DashboardCharpyStatisticsText.Text = PendulumDetailText("Charpy", p?.Charpy);
        ChartIzodScoreText.Text = FormatScore(p?.Izod?.Score);
        ChartCharpyScoreText.Text = FormatScore(p?.Charpy?.Score);
        ChartIzodSourceText.Text = p?.Izod?.HasResults == true
            ? $"Mean {PendulumNumber(p.Izod.MeanKjM2)} kJ/m² / active-material Izod maximum {PendulumNumber(p.Izod.ReferenceMaximumKjM2)} kJ/m²"
            : "No numeric Izod result; comparison score unavailable.";
        ChartCharpySourceText.Text = p?.Charpy?.HasResults == true
            ? $"Mean {PendulumNumber(p.Charpy.MeanKjM2)} kJ/m² / active-material Charpy maximum {PendulumNumber(p.Charpy.ReferenceMaximumKjM2)} kJ/m²"
            : "No numeric Charpy result; comparison score unavailable.";
    }

    private void QueuePendulumConsumerRefresh()
    {
        _pendulumProjectionCache = null;
        _pendulumIntelligencePending = true;
        if (_pendulumConsumersQueued) return;
        _pendulumConsumersQueued = true;
        Dispatcher.BeginInvoke(new Action(() =>
        {
            _pendulumConsumersQueued = false;
            RefreshNativeMaterialTestStatusFromNativeInputTabs(markDirty: true, refreshDetails: false);
            FlushPendulumIntelligenceRefresh();
        }), DispatcherPriority.Background);
    }

    private void FlushPendulumIntelligenceRefresh()
    {
        if (!_pendulumIntelligencePending || WorkspaceTabs.SelectedItem is System.Windows.Controls.TabItem selected &&
            selected.Header?.ToString() is "Izod Measurements" or "Charpy Measurements") return;
        _pendulumIntelligencePending = false;
        RefreshSelectedNativeMaterialDetails();
        QueueNativeMaterialDependentIntelligenceRefresh();
    }

    private static PublicMeasurementSetModel PublicPendulumMeasurementSet(PendulumMethodResults? result) => new()
    {
        Average = result?.MeanKjM2, StandardDeviation = result?.Statistics.SampleStdDev,
        CoefficientOfVariation = result?.Statistics.CvPercent / 100d,
        SampleCount = result?.Statistics.ValidCount ?? 0, Confidence = result?.Statistics.Confidence
    };

    private bool VerifyPendulumIntegrationContract()
    {
        var columns = BuildFastMaterialsColumns().Select(column => column.PropertyName).ToList();
        // Exercise the actual upgrade from a complete v2 layout, not just the new default order.
        var defaultColumns = BuildFastMaterialsColumns();
        var oldOrder = defaultColumns.Where(column => column.PropertyName is not ("InIzod" or "InCharpy"))
            .Concat(defaultColumns.Where(column => column.PropertyName is "InIzod" or "InCharpy"))
            .Select((column, index) => new WorkflowColumnLayout(PrototypeColumnKey(column), column.Width + 7, index))
            .ToList();
        var migrated = MigrateFastMaterialsCoverageColumnLayout(defaultColumns, oldOrder);
        var applied = ApplyFastMaterialsLayout(defaultColumns, migrated);
        var appliedNames = applied.Select(column => column.PropertyName).ToList();
        if (FastMaterialsLayoutContractVersion <= 2 ||
            appliedNames.IndexOf("InIzod") != appliedNames.IndexOf("InFlexible") + 1 ||
            appliedNames.IndexOf("InCharpy") != appliedNames.IndexOf("InIzod") + 1 ||
            applied.Any(column => column.Width != oldOrder.Single(item => item.Key == PrototypeColumnKey(column)).Width) ||
            !migrated.SequenceEqual(MigrateFastMaterialsCoverageColumnLayout(defaultColumns, migrated))) return false;
        if (columns.IndexOf("InIzod") != columns.IndexOf("InFlexible") + 1 ||
            columns.IndexOf("InCharpy") != columns.IndexOf("InIzod") + 1) return false;
        var table = new System.Data.DataTable();
        foreach (var name in new[] { "In Izod", "In Charpy", "In Heat", "In Flexible", "Internal field" })
            table.Columns.Add(name);
        table.Rows.Add("Yes", "No", "Yes", "Yes", "retained");
        var groups = _detailService.BuildGroupedFields(table.Rows[0]);
        if (groups.Any(group => group.Name == "Other") ||
            groups.Single(group => group.Name == "Test Information").Fields.Count != 4 ||
            _detailService.BuildAllFields(table.Rows[0]).Count != 5 ||
            BuildComparisonMetricRows(Array.Empty<ComparisonSnapshot?>()).Count != 8) return false;
        var row = new NativeMaterialRow();
        if (BuildNativeMaterialTestedStatus(row) != "Not tested") return false;
        row.InTensile = row.InImpact = row.InStiffness = row.InHeat = "Yes";
        if (BuildNativeMaterialTestedStatus(row) != "Partially tested") return false;
        row.InIzod = "Yes";
        if (BuildNativeMaterialTestedStatus(row) != "Partially tested") return false;
        row.InCharpy = "Yes";
        if (BuildNativeMaterialTestedStatus(row) != "Fully tested") return false;
        row.InIzod = "No";
        return BuildNativeMaterialTestedStatus(row) == "Partially tested" &&
            BuildFastMaterialsColumns().Count(column => column.PropertyName is "InIzod" or "InCharpy" && column.IsReadOnly) == 2 &&
            FormatSummaryMetric(new TestSummaryMetric { TestType = "Izod", MetricName = "CV", MetricValue = "0.5", Unit = "%", SourceSheet = "Verification", SourceColumn = "CV" }) ==
                0.5.ToString("0.##", CultureInfo.CurrentCulture) + " %" &&
            IntegratedRadarAxes.Length == 8 &&
            IntegratedRadarValues(new RankingRow { Izod = 25, Charpy = 75 }).SequenceEqual(new double?[] { null, null, 25, 75, null, null, null, null });
    }

    private static readonly string[] IntegratedRadarAxes = ["Tensile", "Impact", "Izod", "Charpy", "Stiffness", "Thermal", "Layer", "Consistency"];
    private static double?[] IntegratedRadarValues(RankingRow row) =>
        [row.Tensile, row.Impact, row.Izod, row.Charpy, row.Stiffness, row.Thermal, row.LayerAdhesion, row.Consistency];

    private string BuildIntegratedRadarHtml(string title, RankingRow selected, RankingRow? materialAverage, RankingRow? manufacturerAverage)
    {
        string Point(int i, double score, double radius = 146)
        {
            var angle = (-90 + i * 360d / IntegratedRadarAxes.Length) * Math.PI / 180;
            return (210 + Math.Cos(angle) * radius * score / 100).ToString("0.#", CultureInfo.InvariantCulture) + "," +
                (200 + Math.Sin(angle) * radius * score / 100).ToString("0.#", CultureInfo.InvariantCulture);
        }
        var sb = new System.Text.StringBuilder($"<div class=\"radar-card\"><div class=\"chart-title\">{Html(title)}</div><svg class=\"radar-svg\" viewBox=\"0 0 420 400\" role=\"img\" aria-label=\"Eight-axis engineering radar\">");
        foreach (var level in new[] { 20, 40, 60, 80, 100 })
            sb.Append("<polygon class=\"radar-grid\" points=\"").Append(string.Join(" ", Enumerable.Range(0, 8).Select(i => Point(i, level)))).Append("\"/>");
        for (var i = 0; i < IntegratedRadarAxes.Length; i++)
        {
            var end = Point(i, 100).Split(','); var label = Point(i, 100, 180).Split(',');
            sb.Append($"<line class=\"radar-axis\" x1=\"210\" y1=\"200\" x2=\"{end[0]}\" y2=\"{end[1]}\"/>");
            sb.Append($"<text class=\"radar-label\" x=\"{label[0]}\" y=\"{label[1]}\" text-anchor=\"middle\">{IntegratedRadarAxes[i]}</text>");
        }
        foreach (var (row, style) in new[] { (materialAverage, "radar-poly-material-average"), (manufacturerAverage, "radar-poly-manufacturer-average"), (selected, "radar-poly-selected") })
            if (row is not null) sb.Append($"<polygon class=\"{style}\" points=\"{BuildRadarPolygonPoints(row)}\"/>");
        sb.Append("</svg><div class=\"legend\">Selected material");
        if (materialAverage is not null) sb.Append("; ").Append(Html(materialAverage.Label));
        if (manufacturerAverage is not null) sb.Append("; ").Append(Html(manufacturerAverage.Label));
        sb.Append("</div>");
        sb.Append("<p>Izod and Charpy use separate same-method cohort maxima. Missing results are n/a; central placeholder points are not measured zero.</p>");
        sb.Append("<p>").Append(string.Join("; ", IntegratedRadarAxes.Zip(IntegratedRadarValues(selected), (axis, value) => Html(axis + ": " + FormatScore(value))))).Append("</p></div>");
        return sb.ToString();
    }
}
