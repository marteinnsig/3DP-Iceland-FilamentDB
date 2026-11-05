// Deterministic browser-logic checks; visual acceptance remains an owner test.
const fs = require('node:fs');
const path = require('node:path');
const vm = require('node:vm');
const assert = require('node:assert/strict');
const source = fs.readFileSync(path.join(__dirname, '../App/FilamentDbApp/Services/Website/PendulumWebsitePresentationService.cs'), 'utf8');
const script = source.match(/const string script = """\r?\n([\s\S]*?)\r?\n""";/)?.[1];
assert.ok(script, 'Versioned website script boundary must exist');
let mode = 'samples', unlocked = 0, draws = [];
const elements = {
  chartMode: { get value() { return mode; } },
  impactChart: { closest: () => ({}) }, izodChart: {}, charpyChart: {}, izodCount: {}, charpyCount: {}
};
const a = { materialId: 'A', label: '<Yellow & PLA>', manufacturer: 'Maker', value: 0,
  measurementEvidence: 'Izod Impact Strength · 2026-11-05\nMean 0 kJ/m²\n<script>private-looking text</script>' };
const b = { ...a, materialId: 'B', value: 2, measurementEvidence: 'Independent B statistics' };
const other = { ...a, materialId: 'C', manufacturer: 'Other', measurementEvidence: 'OTHER MATERIAL' };
const context = {
  DATA: { izod: [a, b, other, { ...a, materialId: 'MISSING', value: null }], charpy: [] },
  $: id => elements[id], apply: rows => rows.filter(r => r.materialId !== 'C'),
  aggregateSingle: rows => mode === 'samples' ? rows : [{ label: 'Maker average', value: 1 }],
  fmt: value => String(value),
  esc: value => String(value).replace(/&/g, '&amp;').replace(/</g, '&lt;').replace(/>/g, '&gt;'),
  lockedTooltipElement: { closest: selector => selector === '#izodChart,#charpyChart' },
  unlockTooltip: () => { unlocked++; },
  drawHorizontalMetrics: (id, rows, series) => draws.push({ id, rows, tip: series[0].tip })
};
vm.createContext(context);
vm.runInContext(script, context);
let tip = context.pendulumMeasurementTip(a, 0, 'Izod', [a, b, other]);
assert.match(tip, /Izod: 0 kJ\/m²/);
assert.match(tip, /&lt;Yellow &amp; PLA&gt; · A/);
assert.match(tip, /2026-11-05<br>Mean 0/);
assert.doesNotMatch(tip, /<script>|Independent B/);
mode = 'manufacturer';
tip = context.pendulumMeasurementTip({ label: 'Maker average' }, 1, 'Izod', [a, b, other]);
assert.match(tip, /not pooled specimen statistics/);
assert.match(tip, / · A/);
assert.match(tip, / · B/);
assert.doesNotMatch(tip, /OTHER MATERIAL/);
mode = 'samples';
context.renderPendulumCharts();
assert.equal(unlocked, 1, 'Filter redraw must release a detached pinned pendulum bar');
assert.equal(draws[0].rows.length, 2, 'Filtered/missing materials must not become measured bars');
assert.equal(draws[0].rows[1].value, 0, 'A measured zero remains available');
assert.doesNotMatch(draws[0].tip(draws[0].rows[1], 0), /Independent B/);
const fallback = context.pendulumMeasurementTip({ label: 'No detail' }, 1, 'Charpy', []);
assert.match(fallback, /Saved measurement details unavailable/);
assert.match(fallback, /standard conformity unverified/);
console.log('PASS: method tooltip escaping, MaterialID identity, separate group members, filters, null/zero and stale-pin dismissal');
