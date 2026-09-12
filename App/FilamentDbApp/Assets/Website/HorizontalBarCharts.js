// Shared geometry only: callers retain their existing selection, aggregation, sorting and tooltip contracts.
function drawHorizontalMetrics(target,rows,series,limit){
 const wrapLabel=value=>{const lines=[];let line='';for(const word of String(value||'').split(/\s+/)){if((line+' '+word).trim().length>44&&line){lines.push(line);line=word;}else line=(line+' '+word).trim();}if(line)lines.push(line);return lines;};
 const labels=rows.map(r=>wrapLabel(r.label));
 const left=400,right=110,top=24,bottom=46,width=Math.max(900,($(target).clientWidth||1280)-2);
 const heights=labels.map(lines=>Math.max(series.length*25+12,lines.length*16+12));
 const height=Math.max(180,top+heights.reduce((a,b)=>a+b,0)+bottom);
 const values=rows.flatMap(r=>series.map(s=>r[s.key]).filter(v=>v!=null&&isFinite(v)));
 const low=Math.min(0,...values),high=limit||Math.max(1,...rows.flatMap(r=>series.map(s=>r[s.key]==null?0:Number(r[s.key])+(Number(r[s.err])||0))).filter(Number.isFinite));
 const x=v=>left+(v-low)/(high-low||1)*(width-left-right),base=x(0);
 let grid='';for(let i=0;i<=5;i++){const value=low+(high-low)*i/5,xx=x(value);grid+=`<g class="grid"><line x1="${xx}" x2="${xx}" y1="${top}" y2="${height-bottom}"/></g><text class="x-label" x="${xx}" y="${height-18}" text-anchor="middle">${fmt(value)}</text>`;}
 let body='',offset=top;
 rows.forEach((r,index)=>{
   const rowHeight=heights[index],cy=offset+rowHeight/2;
   body+=`<text class="x-label" x="${left-14}" y="${cy-(labels[index].length-1)*8+4}" text-anchor="end">${labels[index].map((line,i)=>`<tspan x="${left-14}" dy="${i?16:0}">${esc(line)}</tspan>`).join('')}<title>${esc(r.label)}</title></text>`;
   series.forEach((s,j)=>{const value=r[s.key];if(value==null||!isFinite(value))return;
     const y=cy-series.length*25/2+j*25+3,bh=19,end=x(value),bw=Math.abs(end-base),err=Number(r[s.err])||0;
     const tip=s.tip(r,value,index),rank=s.rank?s.rank(r,index):index+1;
     body+=`<rect class="bar" tabindex="0" aria-label="${esc(r.label+' '+fmt(value)+' '+(s.unit||''))}" data-tip="${esc(tip)}" x="${Math.min(base,end)}" y="${y}" width="${Math.max(2,bw)}" height="${bh}" fill="${s.color}" rx="3"/>`;
     if(bw>36)body+=`<text class="rank-label" x="${base+(value<0?-18:18)}" y="${y+14}" text-anchor="middle">${rank||''}</text>`;
     if(err){const a=x(Math.max(low,value-err)),b=x(value+err),ey=y+bh/2;body+=`<line class="err" x1="${a}" x2="${b}" y1="${ey}" y2="${ey}"/><line class="err" x1="${a}" x2="${a}" y1="${ey-5}" y2="${ey+5}"/><line class="err" x1="${b}" x2="${b}" y1="${ey-5}" y2="${ey+5}"/>`;}
     body+=`<text class="x-label" x="${x(value+(value<0?-err:err))+(value<0?-7:7)}" y="${y+14}" text-anchor="${value<0?'end':'start'}">${fmt(value)}</text>`;
   });offset+=rowHeight;
 });
 $(target).innerHTML=`<svg data-horizontal-bars="v67.0.1" width="${width}" height="${height}" viewBox="0 0 ${width} ${height}" role="img" aria-label="Horizontal material comparison"><rect width="100%" height="100%" fill="#0b1220"/>${grid}${body}</svg>`;
 attachTips(target);
 $(target).querySelectorAll('rect.bar').forEach(bar=>bar.addEventListener('keydown',event=>{if(event.key==='Enter'||event.key===' '){event.preventDefault();const box=bar.getBoundingClientRect();bar.dispatchEvent(new MouseEvent('click',{bubbles:true,clientX:box.x+box.width/2,clientY:box.y+box.height/2}));}else if(event.key==='Escape'&&typeof unlockTooltip==='function')unlockTooltip();}));
}
// BEGIN renderGrouped
function renderGrouped(target,countId,rawRows,unit){
 const rows=aggregateGrouped(apply(rawRows)).sort((a,b)=>sortValue(b)-sortValue(a));
 const urls={tensileCount:'https://youtu.be/kax8Ha_AGcQ?si=dXtpEqpu-w8VvFb6',impactCount:'https://youtu.be/ibjS_tWL6sg?si=2-qAKmr6I3IHwx-m'};
 updateMethodLink(countId,rows.length,rawRows.length,urls[countId]||'#');
 const uprightRanks=rankMap(rows,'upright'),flatRanks=rankMap(rows,'flat');
 const topFlat=Math.max(...rows.map(r=>r.flat).filter(v=>v!=null&&isFinite(v))),topUpright=Math.max(...rows.map(r=>r.upright).filter(v=>v!=null&&isFinite(v)));
 drawHorizontalMetrics(target,rows,[['upright','uprightErr','var(--upright)'],['flat','flatErr','var(--flat)']].map(([key,err,color])=>({key,err,color,unit,
   rank:r=>(key==='flat'?flatRanks:uprightRanks).get(r.label),
   tip:(r,val)=>{const marketing=r.marketingName?`<br>Marketing Name: ${esc(r.marketingName)}`:'';return `${esc(r.label)}<br>${key==='flat'?'Flat':'Upright'}: ${fmt(val)} ${unit}${marketing}${pctFromTop(val,key==='flat'?topFlat:topUpright,unit)}${qualityTip(r,key)}`;}
 })));
}
// END renderGrouped
// BEGIN renderSingle
function renderSingle(target,countId,rawRows,unit,metricName='Stiffness'){
 const rows=aggregateSingle(apply(rawRows)).sort((a,b)=>(b.value??0)-(a.value??0));
 updateMethodLink(countId,rows.length,rawRows.length,'https://youtu.be/nv9PexjvFRw?si=LypqEKGuhwySrWDq');
 drawHorizontalMetrics(target,rows,[{key:'value',unit,color:'var(--single)',tip:(r,val)=>{
   const marketing=r.marketingName?`<br>Marketing Name: ${esc(r.marketingName)}`:'',review=(r.youtubeUrl?'<br>YouTube review available':'')+(r.productUrl?'<br>Product page available':''),tipCount=r.sampleCount?`<br>Samples averaged: ${r.sampleCount}`:'';
   return `${esc(r.label)}<br>${esc(metricName)}: ${fmt(val)} ${unit}${marketing}${review}${tipCount}`;
 }}]);
}
// END renderSingle
// BEGIN renderCombined
function renderCombined(){
 const rows=combinedRows(),el=$('combinedCount');if(el)el.textContent=`${rows.length} shown of ${Math.min(DATA.tensile.length,DATA.impact.length)} possible matched samples`;
 drawHorizontalMetrics('combinedChart',rows,[{key:'value',unit:'/ 100',color:'var(--combined)',tip:(r,val)=>`${esc(r.label)}<br>Overall score: ${fmt(val)} / 100${r.marketingName?`<br>Marketing Name: ${esc(r.marketingName)}`:''}<br>Tensile score: ${fmt(r.tensileScore)} / 100<br>Impact score: ${fmt(r.impactScore)} / 100`}],100);
}
// END renderCombined
// BEGIN renderConsistency
function renderConsistency(){
 const rows=consistencyRows(),el=$('consistencyCount');if(el)el.textContent=`${rows.length} shown with CV% data`;
 drawHorizontalMetrics('consistencyChart',rows,[{key:'consistencyScore',unit:'/ 100',color:'var(--single)',tip:(r,val)=>{
   const marketing=r.marketingName?`<br>Marketing Name: ${esc(r.marketingName)}`:'',tipCount=r.sampleCount?`<br>Filaments averaged: ${r.sampleCount}`:'';
   return `${esc(r.label)}<br>Consistency score: ${fmt(val)} / 100${marketing}<br>Average CV%: ${fmt(r.avgCv)}%<br>Average samples: ${r.avgSamples==null?'N/A':fmt(r.avgSamples)+' / 10'}<br>CV measurements used: ${r.measurementCount}${tipCount}`;
 }}],100);
}
// END renderConsistency
