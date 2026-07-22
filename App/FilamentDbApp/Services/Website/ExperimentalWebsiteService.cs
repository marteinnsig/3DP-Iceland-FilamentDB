using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using FilamentDbApp.Models;

namespace FilamentDbApp.Services.Website;

/// <summary>
/// Projects persisted Experimental Results and ExperimentalAnalyticsService output into
/// a deterministic public payload. The browser renderer visualizes these values only.
/// </summary>
public sealed class ExperimentalWebsiteService
{
    private const double HighVariationThresholdPercent = 15d;
    private readonly ExperimentalAnalyticsService _analytics = new();

    public ExperimentalWebsitePayload BuildPayload(IReadOnlyList<ExperimentalWebsiteSeriesInput> inputs)
    {
        var series = inputs.Select(BuildSeries).ToList();
        return new ExperimentalWebsitePayload { Series = series };
    }

    public ExperimentalWebsiteVerificationResult Verify(ExperimentalWebsitePayload payload)
    {
        var errors = new List<string>();
        var ids = payload.Series.Select(x => x.SeriesId).ToList();
        var identityValid = ids.All(x => !string.IsNullOrWhiteSpace(x)) &&
                            ids.Distinct(StringComparer.OrdinalIgnoreCase).Count() == ids.Count &&
                            payload.Series.All(x => !string.IsNullOrWhiteSpace(x.MaterialId) &&
                                x.Runs.All(r => !string.IsNullOrWhiteSpace(r.RunId)) &&
                                x.Runs.Select(r => r.RunId).Distinct(StringComparer.OrdinalIgnoreCase).Count() == x.Runs.Count);
        if (!identityValid) errors.Add("Experimental website identity coverage is incomplete or duplicated.");

        var baselinesValid = payload.Series.All(x => x.Runs.Count(r => r.IsBaseline) <= 1);
        if (!baselinesValid) errors.Add("An experimental website series contains more than one baseline.");

        var valuesFinite = payload.Series.SelectMany(x => x.Runs).All(AllFinite);
        if (!valuesFinite) errors.Add("Experimental website payload contains a non-finite value.");

        var rankingsAligned = payload.Series.All(x =>
        {
            var ranked = x.Runs.Where(r => r.OverallScore.HasValue).OrderBy(r => r.Rank).ToList();
            return ranked.Select(r => r.Rank).SequenceEqual(Enumerable.Range(1, ranked.Count)) &&
                   ranked.Zip(ranked.Skip(1), (a, b) => a.OverallScore >= b.OverallScore).All(ok => ok);
        });
        if (!rankingsAligned) errors.Add("Experimental website rankings do not align with analytics scores.");

        var chartPayloadComplete = payload.Series.All(x => x.Runs.All(r => r.BaselineIndex.Count == 5));
        if (!chartPayloadComplete) errors.Add("Experimental website chart payload is incomplete.");

        return new ExperimentalWebsiteVerificationResult
        {
            Passed = errors.Count == 0,
            IdentityValid = identityValid,
            BaselinesValid = baselinesValid,
            ValuesFinite = valuesFinite,
            RankingsAligned = rankingsAligned,
            ChartPayloadComplete = chartPayloadComplete,
            SeriesCount = payload.Series.Count,
            RunCount = payload.Series.Sum(x => x.Runs.Count),
            Errors = errors
        };
    }

    public string RenderSection(ExperimentalWebsitePayload payload, string markerStart, string markerEnd)
    {
        if (payload.Series.Count == 0) return markerStart + Environment.NewLine + markerEnd;
        var json = JsonSerializer.Serialize(payload, new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase });
        var sb = new StringBuilder();
        sb.AppendLine(markerStart);
        sb.AppendLine("<section id=\"experimentalResultsSection\" class=\"experimental-lab\" aria-labelledby=\"experimentalLabHeading\">");
        sb.AppendLine("<style id=\"experimentalWebsiteStyles\">");
        sb.AppendLine(".experimental-lab{max-width:1400px;margin:28px auto;padding:0 20px 36px}.experimental-lab *{box-sizing:border-box}.experimental-lab-hero{padding:28px;border:1px solid rgba(148,163,184,.25);border-radius:22px;background:linear-gradient(145deg,rgba(30,41,59,.97),rgba(15,23,42,.94));color:#fff}.experimental-lab-hero h1{margin:4px 0 9px;font-size:clamp(30px,4vw,48px)}.experimental-lab-hero p{max-width:920px;color:#cbd5e1;line-height:1.65}.experimental-series-tabs{display:flex;gap:8px;flex-wrap:wrap;margin:18px 0}.experimental-series-tab{border:1px solid #475569;border-radius:999px;background:#172033;color:#dbeafe;padding:9px 14px;cursor:pointer;font-weight:700}.experimental-series-tab[aria-selected=true]{background:#2563eb;border-color:#60a5fa;color:#fff}.experimental-dashboard{display:grid;grid-template-columns:repeat(5,minmax(0,1fr));gap:10px;margin:14px 0}.experimental-stat{padding:13px;border-radius:14px;background:#172033;border:1px solid #334155;color:#fff}.experimental-stat span{display:block;color:#94a3b8;font-size:11px;text-transform:uppercase;letter-spacing:.06em}.experimental-stat strong{display:block;margin-top:5px;font-size:18px}.experimental-quality{grid-column:span 2}.experimental-quality small{display:block;color:#cbd5e1;margin-top:5px;line-height:1.4}.experimental-chart-grid{display:grid;grid-template-columns:repeat(2,minmax(0,1fr));gap:14px}.experimental-chart-card{min-width:0;padding:15px;border-radius:17px;background:#fff;border:1px solid #d8dee9;color:#172033}.experimental-chart-card.wide{grid-column:1/-1}.experimental-chart-card h3{margin:0 0 3px;font-size:17px}.experimental-chart-card p{margin:0 0 9px;color:#64748b;font-size:12px}.experimental-chart{width:100%;height:300px;display:block}.experimental-chart text{font-family:system-ui,-apple-system,Segoe UI,sans-serif}.experimental-table-wrap{overflow:auto;margin-top:16px;border-radius:16px;border:1px solid #334155}.experimental-table{width:100%;border-collapse:collapse;min-width:850px;background:#111827;color:#e5e7eb;font-size:12px}.experimental-table th,.experimental-table td{padding:9px;border-bottom:1px solid #293548;text-align:center}.experimental-table th:first-child,.experimental-table td:first-child{text-align:left}.experimental-table th{color:#93c5fd}.experimental-baseline{color:#60a5fa}.experimental-empty{color:#94a3b8}.experimental-series-panel[hidden]{display:none!important}@media(max-width:950px){.experimental-dashboard{grid-template-columns:repeat(2,minmax(0,1fr))}.experimental-quality{grid-column:1/-1}.experimental-chart-grid{grid-template-columns:1fr}.experimental-chart-card.wide{grid-column:auto}}@media(max-width:560px){.experimental-lab{padding:0 10px 24px}.experimental-lab-hero{padding:20px}.experimental-dashboard{grid-template-columns:1fr}.experimental-quality{grid-column:auto}.experimental-chart{height:260px}}");
        sb.AppendLine("</style>");
        sb.AppendLine("<div class=\"experimental-lab-hero\"><span class=\"portal-eyebrow\">Controlled parameter studies</span><h1 id=\"experimentalLabHeading\">Experimental Engineering Lab</h1><p>Compare controlled print and material parameters using the same persisted native results and analytics reviewed in the desktop Engineering Platform. Charts visualize exported values; no engineering result is recalculated in the browser.</p></div>");
        sb.AppendLine("<div class=\"experimental-series-tabs\" role=\"tablist\" aria-label=\"Published experimental series\">");
        for (var i = 0; i < payload.Series.Count; i++)
        {
            var item = payload.Series[i];
            sb.Append("<button type=\"button\" class=\"experimental-series-tab\" role=\"tab\" data-experimental-series=\"").Append(i).Append("\" aria-selected=\"").Append(i == 0 ? "true" : "false").Append("\">").Append(E(item.MaterialName)).Append(" · ").Append(E(item.ExperimentName)).AppendLine("</button>");
        }
        sb.AppendLine("</div>");
        for (var i = 0; i < payload.Series.Count; i++) AppendSeriesPanel(sb, payload.Series[i], i);
        sb.Append("<script id=\"experimentalWebsiteData\" type=\"application/json\">").Append(json).AppendLine("</script>");
        sb.AppendLine(ChartScript);
        sb.AppendLine("</section>");
        sb.AppendLine(markerEnd);
        return sb.ToString();
    }

    private ExperimentalWebsiteSeriesPayload BuildSeries(ExperimentalWebsiteSeriesInput input)
    {
        var analyticsInputs = input.Runs.Select(r => new ExperimentalAnalyticsInput(r.ExperimentalRunId, r.Label, r.IsBaseline,
            r.LayerAdhesionStrength, r.TensileStrength, r.ImpactUpright, r.ImpactFlat, r.Stiffness)).ToList();
        var analytics = _analytics.Analyze(analyticsInputs);
        var ranks = analytics.RankedRuns.ToDictionary(x => x.ExperimentalRunId, StringComparer.OrdinalIgnoreCase);
        var baseline = input.Runs.FirstOrDefault(x => x.IsBaseline);
        var runs = input.Runs.Select(r =>
        {
            ranks.TryGetValue(r.ExperimentalRunId, out var rank);
            return new ExperimentalWebsiteRunPayload
            {
                RunId = r.ExperimentalRunId, Label = r.Label, Status = r.Status, IsBaseline = r.IsBaseline, ParameterValue = r.ParameterValue,
                TensileStrength = r.TensileStrength, TensileStrengthCv = r.TensileStrengthCv,
                LayerAdhesionStrength = r.LayerAdhesionStrength, LayerAdhesionStrengthCv = r.LayerAdhesionStrengthCv,
                ImpactFlat = r.ImpactFlat, ImpactFlatCv = r.ImpactFlatCv, ImpactUpright = r.ImpactUpright, ImpactUprightCv = r.ImpactUprightCv,
                Stiffness = r.Stiffness, Rank = rank?.Rank ?? 0, OverallScore = rank?.OverallScore,
                BaselineIndex = new Dictionary<string, double?>
                {
                    ["tensileStrength"] = Ratio(r.TensileStrength, baseline?.TensileStrength),
                    ["layerAdhesionStrength"] = Ratio(r.LayerAdhesionStrength, baseline?.LayerAdhesionStrength),
                    ["impactFlat"] = Ratio(r.ImpactFlat, baseline?.ImpactFlat),
                    ["impactUpright"] = Ratio(r.ImpactUpright, baseline?.ImpactUpright),
                    ["stiffness"] = Ratio(r.Stiffness, baseline?.Stiffness)
                }
            };
        }).ToList();
        var missing = runs.Sum(r => Metrics(r).Count(v => !v.HasValue));
        var complete = runs.Count(r => Metrics(r).All(v => v.HasValue));
        var cvs = input.Runs.SelectMany(r => new[] { r.TensileStrengthCv, r.LayerAdhesionStrengthCv, r.ImpactFlatCv, r.ImpactUprightCv }).Where(v => v.HasValue).Select(v => v!.Value).ToList();
        var highestCv = cvs.Count == 0 ? (double?)null : cvs.Max();
        var quality = runs.Count == 0 ? "No runs" : missing > 0 ? "Needs measurements" : highestCv > HighVariationThresholdPercent ? "High variation" : "Complete";
        var detail = highestCv.HasValue ? $"Highest CV {highestCv:0.00}% · warning threshold {HighVariationThresholdPercent:0}%" : "No CV values available";
        return new ExperimentalWebsiteSeriesPayload
        {
            SeriesId = input.MaterialExperimentId, MaterialId = input.MaterialId, MaterialName = input.MaterialName,
            ExperimentName = input.ExperimentName, ParameterUnit = input.ParameterUnit, Notes = input.Notes,
            BaselineLabel = analytics.BaselineLabel, Quality = quality, QualityDetail = detail,
            CompletedRuns = complete, MissingResults = missing, Runs = runs,
            BestTensile = Best(analytics.BestTensileFlat ?? analytics.BestTensileUpright),
            BestImpact = Best(analytics.BestImpactFlat ?? analytics.BestImpactUpright),
            BestStiffness = Best(analytics.BestStiffness),
            Recommended = analytics.RecommendedRun is null ? null : new ExperimentalWebsiteBestPayload(analytics.RecommendedRun.RunLabel, analytics.RecommendedRun.OverallScore)
        };
    }

    private static void AppendSeriesPanel(StringBuilder sb, ExperimentalWebsiteSeriesPayload item, int index)
    {
        sb.Append("<div class=\"experimental-series-panel\" data-experimental-panel=\"").Append(index).Append("\""); if (index > 0) sb.Append(" hidden"); sb.AppendLine(">");
        sb.Append("<div class=\"experimental-dashboard\"><div class=\"experimental-stat\"><span>Runs</span><strong>").Append(item.Runs.Count).Append("</strong></div><div class=\"experimental-stat\"><span>Completed</span><strong>").Append(item.CompletedRuns).Append("</strong></div><div class=\"experimental-stat\"><span>Baseline</span><strong>").Append(E(string.IsNullOrWhiteSpace(item.BaselineLabel) ? "Not selected" : item.BaselineLabel)).Append("</strong></div><div class=\"experimental-stat\"><span>Recommended</span><strong>").Append(E(item.Recommended?.Label ?? "—")).Append("</strong></div><div class=\"experimental-stat experimental-quality\"><span>Quality</span><strong>").Append(E(item.Quality)).Append("</strong><small>").Append(E(item.QualityDetail)).AppendLine("</small></div></div>");
        sb.AppendLine("<div class=\"experimental-chart-grid\">");
        AppendChart(sb, index, "tensile", "Parameter vs Tensile and Layer Adhesion", "MPa", false);
        AppendChart(sb, index, "impact", "Parameter vs Impact", "kJ/m²", false);
        AppendChart(sb, index, "stiffness", "Parameter vs Stiffness", "MPa", false);
        AppendChart(sb, index, "score", "Combined Performance Score", "%", false);
        AppendChart(sb, index, "baseline", "Baseline vs Run Comparison", "Index (baseline = 100)", true);
        sb.AppendLine("</div>");
        sb.AppendLine("<div class=\"experimental-table-wrap\"><table class=\"experimental-table\"><thead><tr><th>Run</th><th>Status</th><th>Rank</th><th>Tensile</th><th>Layer adhesion</th><th>Impact F/U</th><th>Stiffness</th><th>Score</th></tr></thead><tbody>");
        foreach (var run in item.Runs)
        {
            sb.Append("<tr><td>").Append(E(run.Label)); if (run.IsBaseline) sb.Append(" <span class=\"experimental-baseline\" title=\"Baseline\">★</span>");
            sb.Append("</td><td>").Append(E(run.Status)).Append("</td><td>").Append(run.Rank > 0 ? "#" + run.Rank : "—").Append("</td><td>").Append(N(run.TensileStrength)).Append("</td><td>").Append(N(run.LayerAdhesionStrength)).Append("</td><td>").Append(N(run.ImpactFlat)).Append(" / ").Append(N(run.ImpactUpright)).Append("</td><td>").Append(N(run.Stiffness)).Append("</td><td>").Append(N(run.OverallScore)).AppendLine("</td></tr>");
        }
        sb.AppendLine("</tbody></table></div></div>");
    }

    private static void AppendChart(StringBuilder sb, int index, string kind, string title, string unit, bool wide) =>
        sb.Append("<article class=\"experimental-chart-card").Append(wide ? " wide" : string.Empty).Append("\"><h3>").Append(E(title)).Append("</h3><p>").Append(E(unit)).Append(" · baseline ring · best value in gold</p><svg class=\"experimental-chart\" data-experimental-chart=\"").Append(kind).Append("\" data-series-index=\"").Append(index).Append("\" role=\"img\" aria-label=\"").Append(E(title)).AppendLine("\"></svg></article>");

    private static ExperimentalWebsiteBestPayload? Best(ExperimentalBestResult? result) => result is null ? null : new(result.RunLabel, result.Value);
    private static double? Ratio(double? value, double? baseline) => value.HasValue && baseline.HasValue && Math.Abs(baseline.Value) > 0.000001 ? value.Value / baseline.Value * 100d : null;
    private static double?[] Metrics(ExperimentalWebsiteRunPayload r) => [r.TensileStrength, r.LayerAdhesionStrength, r.ImpactFlat, r.ImpactUpright, r.Stiffness];
    private static bool AllFinite(ExperimentalWebsiteRunPayload r) => Metrics(r).Concat([r.ParameterValue, r.OverallScore]).Concat(r.BaselineIndex.Values).Where(v => v.HasValue).All(v => double.IsFinite(v!.Value));
    private static string E(string? value) => WebUtility.HtmlEncode(value ?? string.Empty);
    private static string N(double? value) => value.HasValue && double.IsFinite(value.Value) ? value.Value.ToString("0.##", CultureInfo.InvariantCulture) : "—";

    private const string ChartScript = """
<script id="experimentalWebsiteCharts">
(()=>{
  const root=document.getElementById('experimentalResultsSection');
  const dataNode=document.getElementById('experimentalWebsiteData');
  if(!root||!dataNode)return;
  const payload=JSON.parse(dataNode.textContent||'{"series":[]}');
  const NS='http://www.w3.org/2000/svg';
  const colors={blue:'#2563eb',orange:'#ea580c',green:'#16a34a',pink:'#db2777',violet:'#7c3aed',gold:'#d97706',grid:'#cbd5e1',text:'#475569'};
  const configs={
    tensile:{unit:'MPa',series:[['Tensile strength','tensileStrength',colors.orange],['Layer adhesion','layerAdhesionStrength',colors.blue]]},
    impact:{unit:'kJ/m²',series:[['Flat','impactFlat',colors.green],['Upright','impactUpright',colors.pink]]},
    stiffness:{unit:'MPa',series:[['Stiffness','stiffness',colors.violet]]},
    score:{unit:'%',bar:true,series:[['Score','overallScore',colors.blue]]},
    baseline:{unit:'Index',series:[['Tensile','tensileStrength',colors.orange],['Layer adhesion','layerAdhesionStrength',colors.blue],['Impact flat','impactFlat',colors.green],['Impact upright','impactUpright',colors.pink],['Stiffness','stiffness',colors.violet]],baseline:true}
  };
  const el=(name,attrs={})=>{const node=document.createElementNS(NS,name);Object.entries(attrs).forEach(([k,v])=>node.setAttribute(k,String(v)));return node};
  function draw(svg,series,kind){
    const cfg=configs[kind]; if(!cfg)return; svg.replaceChildren();
    const w=Math.max(480,svg.clientWidth||700),h=Math.max(250,svg.clientHeight||300),m={l:54,r:18,t:48,b:55}; svg.setAttribute('viewBox',`0 0 ${w} ${h}`);
    const values=cfg.series.flatMap(([,key])=>series.runs.map(run=>cfg.baseline?run.baselineIndex[key]:run[key])).filter(Number.isFinite);
    if(!values.length){const t=el('text',{x:25,y:55,fill:colors.text});t.textContent=cfg.baseline?'Select a baseline with calculated results.':'No calculated results available.';svg.append(t);return;}
    const max=Math.max(cfg.bar?100:0,...values)*1.08||1,min=Math.min(0,...values),range=max-min||1;
    for(let i=0;i<=4;i++){const y=m.t+i*(h-m.t-m.b)/4;svg.append(el('line',{x1:m.l,y1:y,x2:w-m.r,y2:y,stroke:colors.grid,'stroke-width':1}));const t=el('text',{x:m.l-8,y:y+4,'text-anchor':'end',fill:colors.text,'font-size':10});t.textContent=(max-i*range/4).toFixed(max<20?1:0);svg.append(t)}
    const numeric=series.runs.length>1&&series.runs.every(r=>Number.isFinite(r.parameterValue));const xs=series.runs.map((r,i)=>numeric?r.parameterValue:i),xmin=Math.min(...xs),xmax=Math.max(...xs),xrange=xmax-xmin||1;
    const px=(run,i)=>m.l+((numeric?run.parameterValue:i)-xmin)/xrange*(w-m.l-m.r),py=v=>m.t+(max-v)/range*(h-m.t-m.b);
    cfg.series.forEach(([label,key,color],si)=>{
      const pts=series.runs.map((run,i)=>({run,i,v:cfg.baseline?run.baselineIndex[key]:run[key]})).filter(p=>Number.isFinite(p.v));
      if(cfg.bar){const slot=(w-m.l-m.r)/Math.max(1,series.runs.length),bw=slot*.58,best=Math.max(...pts.map(p=>p.v));pts.forEach(p=>{const x=m.l+p.i*slot+slot*.21,y=py(p.v);svg.append(el('rect',{x,y,width:bw,height:h-m.b-y,rx:4,fill:p.v===best?colors.gold:color,stroke:p.run.isBaseline?colors.blue:'none','stroke-width':p.run.isBaseline?3:0}));});}
      else {const path=pts.map((p,i)=>(i?'L':'M')+px(p.run,p.i)+' '+py(p.v)).join(' ');if(path)svg.append(el('path',{d:path,fill:'none',stroke:color,'stroke-width':2.5}));const best=Math.max(...pts.map(p=>p.v));pts.forEach(p=>{const x=px(p.run,p.i),y=py(p.v);if(p.run.isBaseline)svg.append(el('circle',{cx:x,cy:y,r:8,fill:'none',stroke:colors.blue,'stroke-width':2}));const dot=el('circle',{cx:x,cy:y,r:p.v===best?5:3.5,fill:p.v===best?colors.gold:color,tabindex:0});const title=el('title');title.textContent=`${p.run.label} · ${label}: ${p.v.toFixed(2)} ${cfg.unit}`;dot.append(title);svg.append(dot);});}
      const lx=m.l+si*125;svg.append(el('line',{x1:lx,y1:22,x2:lx+18,y2:22,stroke:color,'stroke-width':3}));const lt=el('text',{x:lx+23,y:26,fill:colors.text,'font-size':11});lt.textContent=label;svg.append(lt);
    });
    series.runs.forEach((run,i)=>{const t=el('text',{x:px(run,i),y:h-m.b+18,'text-anchor':'middle',fill:colors.text,'font-size':10});t.textContent=run.label;svg.append(t)});
  }
  function renderPanel(index){root.querySelectorAll('[data-experimental-chart]').forEach(svg=>{if(Number(svg.dataset.seriesIndex)===index)draw(svg,payload.series[index],svg.dataset.experimentalChart)});}
  root.querySelectorAll('[data-experimental-series]').forEach(button=>button.addEventListener('click',()=>{const index=Number(button.dataset.experimentalSeries);root.querySelectorAll('[data-experimental-series]').forEach(x=>x.setAttribute('aria-selected',String(x===button)));root.querySelectorAll('[data-experimental-panel]').forEach(x=>x.hidden=Number(x.dataset.experimentalPanel)!==index);requestAnimationFrame(()=>renderPanel(index));}));
  let timer;window.addEventListener('resize',()=>{clearTimeout(timer);timer=setTimeout(()=>{const active=[...root.querySelectorAll('[data-experimental-series]')].findIndex(x=>x.getAttribute('aria-selected')==='true');renderPanel(Math.max(0,active));},120)});
  renderPanel(0);
})();
</script>
""";
}
