using System.Text.RegularExpressions;

namespace FilamentDbApp.Services.Website;

/// <summary>Extends the governed template with direct-method scores while retaining its source data.</summary>
public static class PendulumWebsitePresentationService
{
    private const string Marker = "/* 3DP-DIRECT-PENDULUM-v70.0.1 */";

    public static string Apply(string html)
    {
        if (html.Contains(Marker, StringComparison.Ordinal)) return html;
        var replacements = new (string Old, string New)[]
        {
            ("...combMap.keys(),...cMap.keys()", "...combMap.keys(),...cMap.keys(),...izodMap.keys(),...charpyMap.keys()"),
            ("const ref=t||i||st||co||c||{};", "const ref=t||i||st||co||c||izodMap.get(label)||charpyMap.get(label)||{};"),
            (" const cvs=cvPartsFromRows(r.tensile,r.impact);", " const cvs=cvPartsFromRows(r.tensile,null);\n [r.izod,r.charpy].forEach(m=>{if(m?.samples>0&&m.coefficientOfVariation!=null&&isFinite(m.coefficientOfVariation)&&m.coefficientOfVariation>=0)cvs.push(m.coefficientOfVariation*100);});"),
            ("const samples=samplePartsFromRows(r.tensile,r.impact);", "const samples=samplePartsFromRows(r.tensile,null).filter(v=>v>0);\n [r.izod,r.charpy].forEach(m=>{if(m?.samples>0&&isFinite(m.samples))samples.push(m.samples);});"),
            ("apply(DATA.impact).forEach(r=>{ensure(r).impact=r;});", "apply(DATA.impact).forEach(r=>{ensure(r).impact=r;});\n apply(DATA.izod).forEach(r=>{ensure(r).izod=r;});\n apply(DATA.charpy).forEach(r=>{ensure(r).charpy=r;});"),
            (" const sRows=aggregateSingle(apply(DATA.stiffness));", " const sRows=aggregateSingle(apply(DATA.stiffness));\n const canonicalScores=new Map(canonicalScoreRows().map(r=>[r.label,r]));\n const izodMap=new Map(aggregateSingle(apply(DATA.izod)).map(r=>[r.label,r]));\n const charpyMap=new Map(aggregateSingle(apply(DATA.charpy)).map(r=>[r.label,r]));"),
            ("marketingName:ref.marketingName,", "marketingName:ref.marketingName,legacyImpactRadarPercent:canonicalScores.get(label)?.legacyImpactRadarPercent??null,izodMean:izodMap.get(label)?.value??null,charpyMean:charpyMap.get(label)?.value??null,"),
            ("||r.thermalScore!=null);", "||r.thermalScore!=null||r.izodMean!=null||r.charpyMean!=null||r.legacyImpactRadarPercent!=null);"),
            ("   impactScore:avg(group.map(x=>x.impactScore)),", "   impactScore:avg(group.map(x=>x.impactScore)),\n   legacyImpactRadarPercent:avg(group.map(x=>x.legacyImpactRadarPercent)),\n   izodScore:avg(group.map(x=>x.izodScore)),\n   charpyScore:avg(group.map(x=>x.charpyScore)),"),
            ("   ['Impact',metricValue(row.impactScore)],", "   ['Legacy Impact %',metricValue(row.legacyImpactRadarPercent)],\n   ['Izod',metricValue(row.izodScore)],\n   ['Charpy',metricValue(row.charpyScore)],"),
            ("function renderAll(){", "function renderAll(){renderPendulumCharts();"),
            (" attachTips('performanceProfileChart');", " target.insertAdjacentHTML('beforeend',`<div class=\"value-note\"><strong>Izod:</strong> ${selected.izodMean==null?'N/A':fmt(selected.izodMean)+' kJ/m² · '+fmt(selected.izodScore)+' / 100'}<br><strong>Charpy:</strong> ${selected.charpyMean==null?'N/A':fmt(selected.charpyMean)+' kJ/m² · '+fmt(selected.charpyScore)+' / 100'}<br>${esc(selected.scoreCoverage||'Score coverage unavailable')}<br>Policy: ${esc(selected.scorePolicyVersion||'unavailable')}. Fixed method references; unconfigured references leave scores N/A. Izod and Charpy form one equally weighted impact family. Overall requires all five families and both methods. Legacy Impact % is the mean corrected Flat/Upright result divided by the matching rig capacity; visual historical reference only, not a modern score. Legacy Impact and thermal remain outside Overall.</div>`);\n attachTips('performanceProfileChart');"),
            ("stiffness:avg(items.map(x=>x.stiffnessScore)),thermal:", "izod:avg(items.map(x=>x.izodScore)),charpy:avg(items.map(x=>x.charpyScore)),stiffness:avg(items.map(x=>x.stiffnessScore)),thermal:"),
            ("<th>Impact</th><th>Stiffness</th>", "<th>Impact</th><th>Izod</th><th>Charpy</th><th>Stiffness</th>"),
            ("['overall','tensile','impact','stiffness','thermal','layer','consistency']", "['overall','tensile','impact','izod','charpy','stiffness','thermal','layer','consistency']"),
            ("const metricValue=v=>(v==null||!isFinite(v))?0:", "const metricValue=v=>(v==null||!isFinite(v))?null:"),
            ("const rr=rMax*(val/100);", "if(val==null)return null; const rr=rMax*(val/100);"),
            ("const path='M '+pathPts.map(p=>p.join(' ')).join(' L ')+' Z';", "const completePath=pathPts.every(p=>p!==null);\n const path=completePath?'M '+pathPts.map(p=>p.join(' ')).join(' L ')+' Z':pathPts.map((p,i)=>{const next=pathPts[(i+1)%pathPts.length];return p&&next?'M '+p.join(' ')+' L '+next.join(' '):'';}).filter(Boolean).join(' ');"),
            ("<path d=\"${path}\" fill=\"var(--combined)\"", "<path d=\"${path}\" fill=\"${completePath?'var(--combined)':'none'}\""),
            ("const circles=metrics.map((m,i)=>{const [px,py]=point(i,m[1]);", "const circles=metrics.map((m,i)=>{if(m[1]==null)return '';const [px,py]=point(i,m[1]);"),
            ("${esc(m[0])}: ${fmt(m[1])} / 100", "${esc(m[0])}: ${fmt(m[1])}${m[0]==='Legacy Impact %'?'% of legacy rig capacity':' / 100'}"),
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

        // Scores come from the canonical profile, never a new browser-side cohort calibration.
        const string normalization = " const maxT=Math.max(1,...base.map(r=>r.tensileMean||0)),maxI=Math.max(1,...base.map(r=>r.impactMean||0)),maxS=Math.max(1,...base.map(r=>r.stiffness||0)),maxL=Math.max(1,...base.map(r=>Math.min(r.layerAdhesion||0,100)));";
        const string scoreReturn = " return base.map(r=>({...r,tensileScore:scaleScore(r.tensileMean,maxT),impactScore:scaleScore(r.impactMean,maxI),stiffnessScore:scaleScore(r.stiffness,maxS),layerAdhesionScore:scaleScore(Math.min(r.layerAdhesion||0,100),maxL)}));";
        if (!html.Contains(normalization, StringComparison.Ordinal) || !html.Contains(scoreReturn, StringComparison.Ordinal))
            throw new InvalidOperationException("The website template lacks the canonical score projection boundary.");
        html = html.Replace(normalization, string.Empty, StringComparison.Ordinal)
            .Replace(scoreReturn, " return base.map(r=>({...r,...canonicalScores.get(r.label)}));", StringComparison.Ordinal);
        const string combinedPattern = @"function combinedRows\(\)\{[\s\S]*?(?=function renderCombined\()";
        if (Regex.Matches(html, combinedPattern).Count != 1)
            throw new InvalidOperationException("The website template lacks the canonical Overall chart boundary.");
        html = Regex.Replace(html, combinedPattern,
            "function combinedRows(){return canonicalScoreRows().filter(r=>r.isOverallComparable&&r.overall!=null).map(r=>({...r,value:r.overall})).sort((a,b)=>b.value-a.value);}\n");
        html = html.Replace("<h2>Impact resistance</h2>", "<h2>Legacy Impact — historical comparison</h2>", StringComparison.Ordinal);

        // The canonical renderer is already responsible for the filter and group selectors.
        const string script = """
function canonicalScoreRows(){
 const scoreKeys=['tensileScore','izodScore','charpyScore','impactFamilyScore','stiffnessScore','consistencyScore','layerAdhesionScore','legacyImpactRadarPercent'];
 const source=apply(DATA.tensile||[]).map(r=>({...r,impactScore:null,overall:r.isOverallComparable===true?r.overallScore??null:null}));
 const mode=$('chartMode')?.value||'samples';
 if(mode==='samples')return source;
 const groups=new Map();source.forEach(r=>{const key=r[mode]||'(blank)';if(!groups.has(key))groups.set(key,[]);groups.get(key).push(r);});
 return [...groups.entries()].map(([label,items])=>{
   const complete=items.every(r=>r.isOverallComparable===true&&r.overall!=null);
   const result={label:label+' average',impactScore:null,sampleCount:items.length,isOverallComparable:complete,
     overall:complete?safeAvg(items.map(r=>r.overall)):null,
     scoreCoverage:items.filter(r=>r.isOverallComparable).length+'/'+items.length+' complete profiles; Overall requires every member to be complete',
     scorePolicyVersion:[...new Set(items.map(r=>r.scorePolicyVersion||'unavailable'))].join(', ')};
   scoreKeys.forEach(key=>{result[key]=safeAvg(items.map(r=>r[key]));});return result;
 });
}
function renderPendulumCharts(){
 const anchor=$('impactChart')?.closest('section')||$('stiffnessChart')?.closest('section');
 if(!anchor)return;
 ['izod','charpy'].forEach(method=>{
   const title=method==='izod'?'Izod':'Charpy',id=method+'Chart';
   if(!$(id)){
     const section=document.createElement('section');section.className='card';
     section.innerHTML=`<h2>${title} impact strength</h2><div class="meta"><span id="${method}Count"></span><span>Direct instrument kJ/m²; scores use versioned method references. Unconfigured references and missing results remain N/A.</span></div><div class="scroll"><div id="${id}"></div></div>`;
     anchor.before(section);
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
