namespace FilamentDbApp.Services.Website;

/// <summary>Extends the governed template with direct-method scores while retaining its source data.</summary>
public static class PendulumWebsitePresentationService
{
    private const string Marker = "/* 3DP-DIRECT-PENDULUM-v68.0.3 */";

    public static string Apply(string html)
    {
        if (html.Contains(Marker, StringComparison.Ordinal)) return html;
        var replacements = new (string Old, string New)[]
        {
            ("...combMap.keys(),...cMap.keys()", "...combMap.keys(),...cMap.keys(),...izodMap.keys(),...charpyMap.keys()"),
            ("const ref=t||i||st||co||c||{};", "const ref=t||i||st||co||c||izodMap.get(label)||charpyMap.get(label)||{};"),
            (" const cvs=cvPartsFromRows(r.tensile,r.impact);", " const cvs=cvPartsFromRows(r.tensile,r.impact);\n [r.izod,r.charpy].forEach(m=>{if(m?.samples>0&&m.coefficientOfVariation!=null&&isFinite(m.coefficientOfVariation)&&m.coefficientOfVariation>=0)cvs.push(m.coefficientOfVariation*100);});"),
            ("const samples=samplePartsFromRows(r.tensile,r.impact);", "const samples=samplePartsFromRows(r.tensile,r.impact).filter(v=>v>0);\n [r.izod,r.charpy].forEach(m=>{if(m?.samples>0&&isFinite(m.samples))samples.push(m.samples);});"),
            ("apply(DATA.impact).forEach(r=>{ensure(r).impact=r;});", "apply(DATA.impact).forEach(r=>{ensure(r).impact=r;});\n apply(DATA.izod).forEach(r=>{ensure(r).izod=r;});\n apply(DATA.charpy).forEach(r=>{ensure(r).charpy=r;});"),
            ("layerAdhesionScore:scaleScore(Math.min(r.layerAdhesion||0,100),maxL)}));", "layerAdhesionScore:r.layerAdhesion==null?null:scaleScore(Math.min(r.layerAdhesion,100),maxL)})).map(r=>({...r,overall:safeAvg([r.tensileScore,r.impactScore,r.stiffnessScore,r.consistencyScore,r.layerAdhesionScore,r.izodScore,r.charpyScore])}));"),
            (" const sRows=aggregateSingle(apply(DATA.stiffness));", " const sRows=aggregateSingle(apply(DATA.stiffness));\n const izodMap=new Map(aggregateSingle(apply(DATA.izod)).map(r=>[r.label,r]));\n const charpyMap=new Map(aggregateSingle(apply(DATA.charpy)).map(r=>[r.label,r]));"),
            ("marketingName:ref.marketingName,", "marketingName:ref.marketingName,izodMean:izodMap.get(label)?.value??null,charpyMean:charpyMap.get(label)?.value??null,"),
            ("||r.thermalScore!=null);", "||r.thermalScore!=null||r.izodMean!=null||r.charpyMean!=null);"),
            (" return base.map(r=>({...r,tensileScore:", " const maxIzod=Math.max(0,...base.map(r=>r.izodMean??0)),maxCharpy=Math.max(0,...base.map(r=>r.charpyMean??0));\n return base.map(r=>({...r,izodScore:scaleScore(r.izodMean,maxIzod),charpyScore:scaleScore(r.charpyMean,maxCharpy),tensileScore:"),
            ("   impactScore:avg(group.map(x=>x.impactScore)),", "   impactScore:avg(group.map(x=>x.impactScore)),\n   izodScore:avg(group.map(x=>x.izodScore)),\n   charpyScore:avg(group.map(x=>x.charpyScore)),"),
            ("   ['Impact',metricValue(row.impactScore)],", "   ['Impact',metricValue(row.impactScore)],\n   ['Izod',metricValue(row.izodScore)],\n   ['Charpy',metricValue(row.charpyScore)],"),
            ("function renderAll(){", "function renderAll(){renderPendulumCharts();"),
            (" attachTips('performanceProfileChart');", " target.insertAdjacentHTML('beforeend',`<div class=\"value-note\"><strong>Izod:</strong> ${selected.izodMean==null?'N/A':fmt(selected.izodMean)+' kJ/m² · '+fmt(selected.izodScore)+' / 100'}<br><strong>Charpy:</strong> ${selected.charpyMean==null?'N/A':fmt(selected.charpyMean)+' kJ/m² · '+fmt(selected.charpyScore)+' / 100'}<br>Independent axes compare each method with its maximum measured mean in the comparison cohort. Missing axes have no measured marker; Overall averages available tensile, impact, stiffness, consistency, layer adhesion, Izod and Charpy scores. Thermal remains separate.</div>`);\n attachTips('performanceProfileChart');"),
            ("stiffness:avg(items.map(x=>x.stiffnessScore)),thermal:", "izod:avg(items.map(x=>x.izodScore)),charpy:avg(items.map(x=>x.charpyScore)),stiffness:avg(items.map(x=>x.stiffnessScore)),thermal:"),
            ("<th>Impact</th><th>Stiffness</th>", "<th>Impact</th><th>Izod</th><th>Charpy</th><th>Stiffness</th>"),
            ("['overall','tensile','impact','stiffness','thermal','layer','consistency']", "['overall','tensile','impact','izod','charpy','stiffness','thermal','layer','consistency']"),
            ("const metricValue=v=>(v==null||!isFinite(v))?0:", "const metricValue=v=>(v==null||!isFinite(v))?null:"),
            ("const rr=rMax*(val/100);", "const rr=rMax*((val??0)/100);"),
            ("const circles=metrics.map((m,i)=>{const [px,py]=point(i,m[1]);", "const circles=metrics.map((m,i)=>{if(m[1]==null)return '';const [px,py]=point(i,m[1]);"),
            ("if(metric==='stiffness')return row.stiffness?.value??null;", "if(metric==='izod')return row.izod?.value??null;\n if(metric==='charpy')return row.charpy?.value??null;\n if(metric==='stiffness')return row.stiffness?.value??null;"),
            ("if(metric.startsWith('impact'))return 'kJ/m²';", "if(metric==='izod'||metric==='charpy'||metric.startsWith('impact'))return 'kJ/m²';"),
            ("impactFlat:'Impact flat',impactUpright:'Impact upright',", "impactFlat:'Impact flat',impactUpright:'Impact upright',izod:'Izod',charpy:'Charpy',"),
            ("<option value=\"impactFlat\">Impact flat, kJ/m²</option>", "<option value=\"impactFlat\">Impact flat, kJ/m²</option><option value=\"izod\">Izod, kJ/m²</option><option value=\"charpy\">Charpy, kJ/m²</option>"),
            ("function reinforcementMetricValue(row,metric){", "function reinforcementMetricValue(row,metric){if(metric==='izod')return row.izodMean;if(metric==='charpy')return row.charpyMean;"),
            ("metric.startsWith('impact')?' kJ/m²'", "(metric==='izod'||metric==='charpy'||metric.startsWith('impact'))?' kJ/m²'"),
            ("   impactFlat:{key:'impactFlat',", "   izod:{key:'izodMean',label:'Izod',unit:' kJ/m²',shortLabel:'Izod',color:'var(--single)'},\n   charpy:{key:'charpyMean',label:'Charpy',unit:' kJ/m²',shortLabel:'Charpy',color:'var(--single)'},\n   impactFlat:{key:'impactFlat',")
        };
        foreach (var (oldValue, newValue) in replacements)
        {
            if (!html.Contains(oldValue, StringComparison.Ordinal))
                throw new InvalidOperationException("The active website template does not support the direct Izod / Charpy presentation contract: " + oldValue);
            html = html.Replace(oldValue, newValue, StringComparison.Ordinal);
        }

        // The canonical renderer is already responsible for the filter and group selectors.
        const string script = """
function renderPendulumCharts(){
 const anchor=$('stiffnessChart')?.closest('section');
 if(!anchor)return;
 ['izod','charpy'].forEach(method=>{
   const title=method==='izod'?'Izod':'Charpy',id=method+'Chart';
   if(!$(id)){
     const section=document.createElement('section');section.className='card';
     section.innerHTML=`<h2>${title} impact strength</h2><div class="meta"><span id="${method}Count"></span><span>Direct instrument kJ/m²; radar scores compare method means within the comparison cohort. Missing results remain N/A.</span></div><div class="scroll"><div id="${id}"></div></div>`;
     anchor.after(section);
   }
   const rows=aggregateSingle(apply(DATA[method]||[])).filter(r=>r.value!=null&&isFinite(r.value)).sort((a,b)=>b.value-a.value);
   $(method+'Count').textContent=rows.length+' measured materials / groups';
   drawHorizontalMetrics(id,rows,[{key:'value',unit:'kJ/m²',color:'var(--single)',tip:(r,v)=>`${esc(r.label)}<br>${title}: ${fmt(v)} kJ/m²`}]);
 });
}
""";
        return html.Replace("function renderAll(){", Marker + "\n" + script + "\nfunction renderAll(){", StringComparison.Ordinal);
    }
}
