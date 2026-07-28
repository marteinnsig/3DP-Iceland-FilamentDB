namespace FilamentDbApp.Services.Website;

public sealed class PrintingPriceCalculatorPortalService
{
    public const string PortalPageId = "portalPageCalculator";
    public const string PortalMarker = "3DP-PRINTING-PRICE-CALCULATOR-v57.0.3";
    public const string StandaloneRoute = "price/";

    public string BuildHtml() => """
        <!-- 3DP-PRINTING-PRICE-CALCULATOR-v57.0.3 -->
        <div class="price-calculator">
          <header class="price-calculator-hero">
            <div class="price-calculator-local-note"><strong>Iceland (ISK) Default Pricing</strong><br>
              Default pricing values in this calculator are configured for Iceland and use Icelandic Króna (ISK). Adjust labor rates,
              electricity costs, printer operating costs, and other values as needed for your region or business.
            </div>
            <h1>3D Printing Price Calculator</h1>
            <p>Standalone calculator based on the Print Farm Academy Excel sheet. Fill in the blue input fields to estimate material
              cost, labor, machine time, packaging, landed cost, and suggested customer pricing.</p>
          </header>
          <main class="price-calculator-grid">
            <section class="price-calculator-card half"><h2>Job Inputs <span>Customer quote</span></h2><div class="fields">
              <label>Part / Project Name<input data-pc="partName" value="Custom 3D Prints"></label>
              <label>Revision No.<input data-pc="revision" value="V1"></label>
              <label>Date<input data-pc="date" type="date"></label>
              <label>Company / Prepared By<input data-pc="preparedBy" value="3DP Iceland"></label>
              <label>Material Used<input data-pc="material" value="PLA"></label>
              <label>Filament Cost per kg<input data-pc="filamentCost" type="number" step="0.01" value="5000"></label>
              <label>Total Filament Required (g)<input data-pc="filamentGrams" type="number" step="0.01" value="10"></label>
              <label>Total Printing Time (hr)<input data-pc="printHours" type="number" step="0.01" value="0.4"></label>
              <label>Print/Post-processing Labor (min)<input data-pc="laborMinutes" type="number" step="1" value="15"></label>
              <label>Customer Consulting Time (min)<input data-pc="commMinutes" type="number" step="1" value="10"></label>
              <label>Parts Design / Changes Time (min)<input data-pc="designMinutes" type="number" step="1" value="0"></label>
              <label>Target Margin (%)<input data-pc="targetMargin" type="number" step="1" min="0" max="95" value="60"></label>
            </div><p class="hint">Labor cost uses print/post-processing labor, customer consulting time, and parts design / changes time.</p></section>
            <section class="price-calculator-card half advanced"><h2>Advanced Inputs <small>(Default monetary values are in ISK)</small>
              <span>Calculation values</span></h2><div class="fields">
              <label>Material Efficiency Factor<input data-pc="efficiency" type="number" step="0.01" value="1.1"></label>
              <label>Labor Hourly Rate<input data-pc="laborRate" type="number" step="0.01" value="7500"></label>
              <label>Printer Cost<input data-pc="printerCost" type="number" step="0.01" value="300000"></label>
              <label>Additional Upfront Cost<input data-pc="upfrontCost" type="number" step="0.01" value="0"></label>
              <label>Annual Maintenance &amp; Repair<input data-pc="maintenance" type="number" step="0.01" value="10000"></label>
              <label>Estimated Life (yrs)<input data-pc="lifeYears" type="number" step="0.1" value="2"></label>
              <label>Estimated Uptime (%)<input data-pc="uptime" type="number" step="1" min="0" max="100" value="50"></label>
              <label>Average Power Consumption (W)<input data-pc="powerW" type="number" step="1" value="150"></label>
              <label>Electricity Cost per kWh<input data-pc="electricity" type="number" step="0.01" value="13"></label>
              <label>Printer Cost Buffer Factor<input data-pc="buffer" type="number" step="0.01" value="1.3"></label>
            </div><p class="hint">Estimated Uptime uses a percentage from 0 to 100; enter 50 for 50%.</p>
              <p class="capacity-warning" data-pc="capacityWarning" hidden></p></section>
            <section class="price-calculator-card wide"><h2>Materials Input <span>Printed &amp; purchased</span></h2>
              <div class="table-scroll"><table data-pc="materialsTable"><thead><tr><th>Item</th><th>Name</th><th>Qty</th>
                <th>Unit Cost</th><th>Total</th></tr></thead><tbody></tbody></table></div></section>
            <section class="price-calculator-card narrow"><h2>Summary <span>Live</span></h2><div class="results">
              <div>Materials<strong data-pc="materialsCost">0</strong></div><div>Labor<strong data-pc="laborCost">0</strong></div>
              <div>Machine Time<strong data-pc="machineCost">0</strong></div>
              <div>Total Landed Cost<strong data-pc="landedCost">0</strong></div></div></section>
            <section class="price-calculator-card wide"><h2>Packaging &amp; Shipping Input <span>Optional</span></h2>
              <div class="table-scroll"><table data-pc="packagingTable"><thead><tr><th>Item</th><th>Name</th><th>Qty</th>
                <th>Unit Cost</th><th>Total</th></tr></thead><tbody></tbody></table></div></section>
            <section class="price-calculator-card narrow"><h2>Printer Rate Results <span>From adv. inputs</span></h2><div class="results">
              <div>Capital cost / hr<strong data-pc="capitalHr">0</strong></div>
              <div>Electrical cost / hr<strong data-pc="electricHr">0</strong></div>
              <div>Total printer cost / hr<strong data-pc="printerHr">0</strong></div></div></section>
            <section class="price-calculator-card"><h2>Final Pricing Based on Target Margin <span>Quote output</span></h2>
              <div class="final-price">Final Quote Price<strong data-pc="finalPrice">0</strong></div>
              <p class="hint">Final price is calculated from the landed cost and the user-defined target margin.</p>
              <div class="actions"><button type="button" data-pc-action="print">Export Official Price Quote PDF</button>
                <button type="button" class="secondary" data-pc-action="reset">Reset Defaults</button></div></section>
          </main>
        </div>
        <div class="price-calculator-quote" data-pc="quote"><header><div><h1 data-pc="qCompany">3DP Iceland</h1>
          <p>3D Printing Price Quote</p><p><strong>Quote #:</strong> <span data-pc="qRevision"></span></p>
          <p><strong>Date:</strong> <span data-pc="qDate"></span></p></div><div><h2>PRICE QUOTE</h2>
          <p>Prepared using a calculator based on the Print Farm Academy pricing sheet.</p></div></header>
          <table><thead><tr><th>Description</th><th>Qty</th><th>Unit Price</th><th>Total</th></tr></thead>
            <tbody data-pc="qRows"></tbody></table><div class="quote-total">Final quote price <strong data-pc="qTotal"></strong></div>
          <p>This quote is an estimate based on the supplied project information. Shipping is shown separately when entered in the
            calculator. Final pricing may change if the model, material, quantity, design requirements, shipping, or delivery
            requirements change.</p><footer>Pricing calculator methodology credited to Print Farm Academy.</footer></div>
        """;

    public string BuildCss() => """
        <style id="printingPriceCalculatorStyles">
        #portalPageCalculator{--pc-line:#334155;--pc-muted:#94a3b8;--pc-text:#e5e7eb;--pc-input:#dbeafe}
        #portalPageCalculator *{box-sizing:border-box}
        #portalPageCalculator .price-calculator{max-width:1200px;margin:auto;padding:24px;color:var(--pc-text)}
        #portalPageCalculator .price-calculator-hero{background:linear-gradient(135deg,#075985,#0f172a);border:1px solid #155e75;
          border-radius:22px;padding:24px;margin-bottom:18px;box-shadow:0 18px 50px #0006}
        #portalPageCalculator .price-calculator-local-note{display:inline-block;margin-bottom:14px;padding:14px 18px;border-radius:10px;
          background:#e8f4ff;border:1px solid #b7d8f5;color:#123}
        #portalPageCalculator .price-calculator-hero h1{margin:0 0 8px;font-size:clamp(28px,4vw,44px)}
        #portalPageCalculator .price-calculator-hero p{color:#cbd5e1;max-width:850px;line-height:1.5}
        #portalPageCalculator .price-calculator-grid{display:grid;grid-template-columns:repeat(12,1fr);gap:16px}
        #portalPageCalculator .price-calculator-card{grid-column:span 12;background:#111827e6;border:1px solid var(--pc-line);
          border-radius:18px;padding:18px;box-shadow:0 12px 35px #0004}
        #portalPageCalculator .half{grid-column:span 6}#portalPageCalculator .wide{grid-column:span 8}
        #portalPageCalculator .narrow{grid-column:span 4}
        #portalPageCalculator .price-calculator-card h2{display:flex;justify-content:space-between;gap:12px;align-items:baseline;
          font-size:20px;margin:0 0 12px;padding-bottom:9px;border-bottom:1px solid var(--pc-line)}
        #portalPageCalculator .price-calculator-card h2 span{font-size:12px;color:#082f49;background:#bae6fd;border-radius:999px;
          padding:5px 9px;white-space:nowrap}#portalPageCalculator .price-calculator-card h2 small{font-size:.8em;font-weight:400}
        #portalPageCalculator .fields{display:grid;grid-template-columns:1fr 1fr;gap:12px}
        #portalPageCalculator label{display:block;font-size:13px;color:#cbd5e1}
        #portalPageCalculator input{display:block;width:100%;margin-top:5px;padding:10px 11px;border-radius:10px;border:1px solid #60a5fa;
          background:var(--pc-input);color:#0f172a;font-size:15px}
        #portalPageCalculator input[readonly]{background:#e5e7eb;border-color:#94a3b8}
        #portalPageCalculator table{width:100%;border-collapse:separate;border-spacing:0 8px}
        #portalPageCalculator th{font-size:12px;text-align:left;color:#cbd5e1}#portalPageCalculator td{padding:0 4px}
        #portalPageCalculator td:first-child{font-size:12px;color:var(--pc-muted)}
        #portalPageCalculator td input{margin:0;padding:8px}#portalPageCalculator td .total{text-align:right}
        #portalPageCalculator .results{display:grid;gap:12px}#portalPageCalculator .results div{background:#0b1220;
          border:1px solid var(--pc-line);border-radius:16px;padding:14px;color:var(--pc-muted);font-size:12px}
        #portalPageCalculator .results strong{display:block;font-size:24px;color:#86efac;margin-top:4px}
        #portalPageCalculator .final-price{background:#08111f;border:1px solid #1e40af;border-radius:16px;padding:16px;text-align:center;
          color:#bfdbfe;font-weight:700}#portalPageCalculator .final-price strong{display:block;color:#fff;font-size:30px;margin-top:8px}
        #portalPageCalculator .hint{font-size:12px;color:var(--pc-muted);line-height:1.45}
        #portalPageCalculator .capacity-warning{padding:10px 12px;border-radius:10px;background:#451a03;border:1px solid #f59e0b;
          color:#fde68a;font-size:13px;line-height:1.45}
        #portalPageCalculator .actions{display:flex;gap:10px;flex-wrap:wrap}
        #portalPageCalculator .actions button{border:0;border-radius:12px;padding:11px 14px;font-weight:800;cursor:pointer;
          background:#38bdf8;color:#062235}#portalPageCalculator .actions .secondary{background:#334155;color:#e5e7eb}
        .price-calculator-quote{display:none}
        @media(max-width:900px){#portalPageCalculator .half,#portalPageCalculator .wide,#portalPageCalculator .narrow{grid-column:span 12}}
        @media(max-width:600px){#portalPageCalculator .price-calculator{padding:14px}#portalPageCalculator .fields{grid-template-columns:1fr}
          #portalPageCalculator .table-scroll{overflow-x:auto}#portalPageCalculator table{min-width:680px}
          #portalPageCalculator .price-calculator-card h2{align-items:flex-start;flex-direction:column}}
        @media print{body.price-calculator-print *{visibility:hidden!important}
          body.price-calculator-print .price-calculator-quote,body.price-calculator-print .price-calculator-quote *{visibility:visible!important}
          body.price-calculator-print .price-calculator-quote{display:block!important;position:absolute;inset:0 auto auto 0;
            width:100%;max-width:820px;margin:0;padding:28px;color:#111;background:#fff}
          body.price-calculator-print .price-calculator-quote header{display:flex;justify-content:space-between;gap:24px;
            border-bottom:3px solid #0f172a;margin-bottom:22px}body.price-calculator-print .price-calculator-quote table{width:100%;border-collapse:collapse}
          body.price-calculator-print .price-calculator-quote th,body.price-calculator-print .price-calculator-quote td{
            padding:9px;border:1px solid #cbd5e1}body.price-calculator-print .quote-total{margin:14px 0 0 auto;padding:12px;
            width:360px;background:#e2e8f0;font-size:20px;display:flex;justify-content:space-between}
          body.price-calculator-print footer{text-align:center;color:#64748b;margin-top:24px;font-size:11px}@page{size:A4;margin:14mm}}
        </style>
        """;

    public string BuildScript() => """
        <script id="printingPriceCalculatorScript">
        (function(){
          const root=document.getElementById('portalPageCalculator');if(!root||root.dataset.pcReady)return;root.dataset.pcReady='true';
          const get=id=>root.querySelector('[data-pc="'+id+'"]'),money=n=>Number(n||0).toLocaleString('en-US',{maximumFractionDigits:0});
          const html=value=>String(value??'').replace(/[&<>"']/g,char=>({'&':'&amp;','<':'&lt;','>':'&gt;','"':'&quot;',"'":'&#39;'}[char]));
          const num=id=>parseFloat(get(id).value)||0,today=new Date().toISOString().slice(0,10);get('date').value=today;
          const rowNames={materials:['Printed Part','Hardware 1','Hardware 2','Hardware 3','Hardware 4','Hardware 5','Hardware 6','Hardware 7'],
            packaging:['Packaging 1','Packaging 2','Packaging 3','Packaging 4','Packaging 5','Packaging 6','Packaging 7','Postage']};
          function makeRows(id,names){const body=get(id).querySelector('tbody');body.innerHTML='';names.forEach((item,index)=>{
            const row=document.createElement('tr'),name=index===0&&id==='materialsTable'?get('partName').value:
              (item==='Postage'?'Average Shipping Cost':'(Insert name here)'),qty=index===0&&id==='materialsTable'?1:0;
            row.innerHTML='<td>'+item+'</td><td><input class="name" value="'+name+'"></td><td><input class="qty" type="number" step="0.01" value="'+qty+
              '"></td><td><input class="unit" type="number" step="0.01" value="0"></td><td><input class="total" readonly value="0"></td>';
            body.appendChild(row);});}
          function sumTable(id){let sum=0;Array.from(get(id).querySelectorAll('tbody tr')).forEach((row,index)=>{
            const qty=parseFloat(row.querySelector('.qty').value)||0;let unit=parseFloat(row.querySelector('.unit').value)||0;
            if(id==='materialsTable'&&index===0){unit=(num('filamentGrams')/1000)*num('filamentCost')*num('efficiency');
              row.querySelector('.unit').value=unit.toFixed(2);row.querySelector('.name').value=get('partName').value;}
            const total=qty*unit;sum+=total;row.querySelector('.total').value=money(total);});return sum;}
          function calculate(){const investment=num('printerCost')+num('upfrontCost');
            const lifetime=investment+num('maintenance')*num('lifeYears');
            const uptimeFraction=Math.min(Math.max(num('uptime'),0),100)/100;
            const availableLifetimeHours=8760*uptimeFraction*num('lifeYears');
            const capital=availableLifetimeHours?lifetime/availableLifetimeHours:0;
            const electric=(num('powerW')/1000)*num('electricity'),printer=(capital+electric)*num('buffer');
            const materials=sumTable('materialsTable'),packaging=sumTable('packagingTable');
            const labor=((num('laborMinutes')+num('commMinutes')+num('designMinutes'))/60)*num('laborRate');
            const machine=num('printHours')*printer,landed=materials+labor+machine+packaging;
            [['materialsCost',materials],['laborCost',labor],['machineCost',machine],['landedCost',landed],['capitalHr',capital],
             ['electricHr',electric],['printerHr',printer]].forEach(x=>get(x[0]).textContent=money(x[1]));
            const capacityWarning=get('capacityWarning'),exceeds=num('printHours')>availableLifetimeHours&&availableLifetimeHours>0;
            capacityWarning.hidden=!exceeds;capacityWarning.textContent=exceeds?
              'Printing time exceeds the '+money(availableLifetimeHours)+' available uptime hours over the estimated printer life.':'';
            const margin=Math.min(Math.max(num('targetMargin'),0),95)/100;get('finalPrice').textContent=money(margin<1?landed/(1-margin):landed);}
          function lineItems(id){return Array.from(get(id).querySelectorAll('tbody tr')).map(row=>{const name=row.querySelector('.name').value.trim();
            const qty=parseFloat(row.querySelector('.qty').value)||0,unit=parseFloat(row.querySelector('.unit').value)||0;
            return{name:name,qty:qty,unit:unit,total:qty*unit};}).filter(x=>x.qty>0&&x.total>0&&x.name&&x.name!=='(Insert name here)');}
          function quote(){calculate();get('qCompany').textContent=get('preparedBy').value||'3D Printing Service';
            get('qDate').textContent=get('date').value||today;get('qRevision').textContent=get('revision').value||'V1';
            const landed=Number(get('landedCost').textContent.replace(/,/g,''))||0,margin=Math.min(Math.max(num('targetMargin'),0),95)/100;
            const suggested=margin<1?landed/(1-margin):landed,first=get('materialsTable').querySelector('tbody tr');
            const qty=Math.max(parseFloat(first.querySelector('.qty').value)||1,1),shipping=lineItems('packagingTable');
            const shippingTotal=shipping.reduce((sum,x)=>sum+x.total,0),partsTotal=Math.max(suggested-shippingTotal,0);
            const lines=[{name:(get('partName').value||'Custom 3D Prints').trim(),qty:qty,unit:partsTotal/qty,total:partsTotal}].concat(shipping);
            get('qRows').innerHTML=lines.filter(x=>x.total>0).map(x=>'<tr><td>'+html(x.name)+'</td><td>'+money(x.qty)+'</td><td>'+
              money(x.unit)+' ISK</td><td>'+money(x.total)+' ISK</td></tr>').join('');get('qTotal').textContent=money(suggested)+' ISK';}
          function reset(){get('partName').value='Custom 3D Prints';get('revision').value='V1';get('preparedBy').value='3DP Iceland';
            get('material').value='PLA';get('date').value=today;Object.entries({filamentCost:5000,filamentGrams:10,printHours:.4,
              laborMinutes:15,commMinutes:10,designMinutes:0,targetMargin:60,efficiency:1.1,laborRate:7500,printerCost:300000,
              upfrontCost:0,maintenance:10000,lifeYears:2,uptime:50,powerW:150,electricity:13,buffer:1.3})
              .forEach(x=>get(x[0]).value=x[1]);makeRows('materialsTable',rowNames.materials);makeRows('packagingTable',rowNames.packaging);calculate();}
          makeRows('materialsTable',rowNames.materials);makeRows('packagingTable',rowNames.packaging);root.addEventListener('input',calculate);
          root.querySelector('[data-pc-action="reset"]').addEventListener('click',reset);
          root.querySelector('[data-pc-action="print"]').addEventListener('click',()=>{quote();document.body.classList.add('price-calculator-print');
            setTimeout(()=>window.print(),100);});window.addEventListener('afterprint',()=>document.body.classList.remove('price-calculator-print'));calculate();
        })();
        </script>
        """;
}
