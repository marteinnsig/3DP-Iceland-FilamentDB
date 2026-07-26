# Pricing and Usage Analytics

## v48.0 Material, Purchase and Usage Ownership Audit

Status: Research candidate. No schema, UI or calculation behavior changed.

## Existing price owners

### Public material price

`NativeMaterialManagerRows` owns the optional material reference fields:

- MSRP amount and source currency
- calculated MSRP USD
- calculated MSRP USD/kg
- price checked date
- spool net weight used by the per-kg calculation

This is the accepted public comparison price. Recommendation Detail,
Engineering Advisor, public reports and most ranking callers use
`MsrpUsdPerKg`. Missing MSRP remains unavailable and must never fall back to a
purchase or inventory cost.

The USD conversion is based on user-maintained Currency settings. The current
material row stores calculated USD values but does not snapshot the exact
exchange-rate value or effective date used by that calculation. Price Checked
Date describes the material price check; it is not exchange-rate provenance.

The current conversion helper returns a `1` rate when a supported configured
rate is missing/invalid and normalizes an unsupported currency code to USD.
That can create a plausible but false USD value. v48 must make missing or
unsupported conversion explicit instead of silently applying 1:1.

### Purchase transaction cost

`PurchaseOrders` owns transaction currency and its explicit ISK exchange rate.
`PurchaseOrderLines` owns unit price, discount, unit weight, allocated
shipping/tax/customs/fees and calculated landed line/unit/kg cost.

This is transaction evidence. It must remain attached to the order and line
that produced it. A later rate or a later purchase of the same MaterialID must
never rewrite an accepted historical transaction.

### Physical inventory cost

`InventorySpoolItems` owns physical spool identity, remaining weight, purchase
link and its transferred purchase/landed-cost snapshot. Inventory summaries
group values by currency and do not silently combine currencies.

The current Inventory Engine `Cost/kg` uses the spool's purchase-price field and
weight. It is inventory valuation context, not public MSRP and not necessarily
the fully allocated landed cost.

### Compatibility material landed cost

Materials also retain material-level landed-cost fields. Purchase receiving can
synchronize the latest accepted purchase line into these fields. They are a
convenience/compatibility snapshot, not immutable price history and not a safe
substitute for public MSRP.

## Current downstream consumers

- Fast Materials displays both MSRP USD/kg and landed USD/kg.
- Native Excel export/recovery round-trips both material-level sets.
- Purchase and inventory tables, reports, backup/restore and diagnostics retain
  transaction and spool cost evidence.
- Engineering Advisor and Recommendation Detail use canonical MSRP USD/kg.
- Public Material, Comparison, Manufacturer and Printing Recommendation reports
  allowlist MSRP USD/kg and explicitly exclude landed/purchase/internal fields.
- Website pricing payload carries both values internally, but public report
  allowlists remain MSRP-specific.
- Manufacturer engineering intelligence still falls back from missing MSRP to
  landed cost and then a legacy projection. This conflicts with the accepted
  no-fallback public-MSRP contract and is the first bounded v48 correction.

## Honest price/performance metric

The minimum defensible metric is:

`Overall engineering score points / MSRP USD per kg`

Required presentation:

- label it as a comparative 3DPIceland value index, not a physical property;
- show the two inputs beside the result;
- calculate only when canonical MSRP USD/kg is positive and Overall Score is
  available;
- keep missing input as `Not recorded`;
- never substitute landed cost, purchase price or an inferred exchange rate;
- compare primarily within a relevant material family and disclose when a
  broader ranking is shown;
- preserve the existing engineering score and price independently so the ratio
  cannot hide a weak absolute result.

No stored metric column is required. The index should be calculated from the
governed inputs by one service and reused by UI/report callers.

## Usage and history ownership

Current measurement samples provide specimen counts, but they do not prove
print time, operator/test time or filament consumption. Sample count is derived
measurement evidence and must not be repurposed as a usage log.

A future usage record requires an append-only event identity with:

- stable event ID and MaterialID;
- optional InventoryItemID and printer/profile identity;
- event type: test preparation, test print, production print or adjustment;
- occurred-at timestamp;
- filament used in grams when measured;
- print duration and hands-on/test duration as separate optional values;
- produced and accepted specimen/part counts as separate optional values;
- source/provenance, note and created/updated evidence;
- explicit reversal/correction rather than silent historical overwrite.

Usage must be recorded at the event/job level. Material totals are projections,
not editable Material fields. Missing measurements remain `Not recorded`, never
zero. No usage schema should be added until the owner-approved capture workflow
shows where these values are entered and corrected.

## Risks

1. Public MSRP and internal landed cost can be mislabeled if compatibility
   fallback remains in a public/value caller.
2. Material-level landed cost represents only a latest snapshot and cannot
   provide immutable purchase history.
3. Material currency settings lack rate/date snapshots, so historical USD
   recalculation is not yet auditable.
4. Missing/unsupported material conversion can currently become a silent 1:1
   USD calculation.
5. Inventory `Cost/kg` and purchase-line `LandedCostPerKg` have different
   semantics despite similar labels.
6. A single editable total for hours or grams would destroy job/event history.
7. Price/score ratios can reward low price while hiding weak absolute
   performance unless both inputs and scope are visible.
8. Public outputs must never expose supplier, order, lot, storage or internal
   landed-cost evidence without a new explicit allowlist decision.

## Recommended implementation sequence

### v48.0.1 Canonical Pricing Provenance

Candidate implementation:

- centralize MSRP resolution in one reusable service;
- remove the manufacturer-intelligence landed-cost fallback;
- reject missing/unsupported currency conversion as `Not recorded`, never 1:1;
- define rate value/effective-date provenance before historical USD snapshots;
- add deterministic tests proving missing MSRP remains unavailable;
- inventory every website/report/value caller against the same source boundary;
- make labels distinguish public MSRP, purchase landed cost and inventory cost.

No schema change should be needed.

The candidate implements the shared resolver and fail-closed conversion
boundary without changing schema. Existing stored source amounts/currencies
remain untouched; calculated USD fields become blank when their required rate
is missing or the currency is unsupported. Verification covers valid explicit
conversion, missing rate, unsupported currency and canonical MSRP precedence.

Owner runtime acceptance confirms Materials pricing, separated MSRP/landed
values, Advisor context, Manufacturer/website behavior and Full Data
Verification 351/351. v48.0.1 is canonical.

### v48.0.2 Governed Value Index

- implement the documented Overall-score/MSRP index in one service;
- expose inputs, missing-data reason and comparison scope;
- reuse it in selected internal surfaces before any public allowlist expansion;
- add public output only after manual HTML/PDF review and explicit approval.

No stored metric column should be needed.

### v48.0.3 Usage Event Contract

- prototype the capture and correction workflow without touching owner data;
- decide whether events belong to a print job, test session or a general usage
  ledger;
- approve printer/profile, inventory-lot and specimen-count relationships;
- only then propose additive SQLite tables, Excel recovery and diagnostics.

### v48.1 Job Pricing

Keep job quotations separate from analytics. A quote must snapshot MaterialID,
effective cost provenance, printer profile, every formula input, currency/rate,
calculation version and timestamp so later Settings changes cannot rewrite it.

### v48.2 Exchange-rate catalog

Remain deferred until an official endpoint and reuse contract are approved.
Downloaded reference rates may prefill only new unsaved transactions and must
never alter saved purchases, inventory lots or quotes.

## Automation and Verification assessment

This research increment changes no deterministic runtime contract, so the
tester, AutomationIds, scenarios, seed database and Full Data Verification do
not need modification.

v48.0.1 must extend Verification for strict MSRP provenance and should extend an
existing safe deterministic scenario only if a visible workflow changes.
v48.0.2 requires formula/missing-input probes. A future usage schema requires
disposable CRUD, recovery round-trip, exact cleanup and business-state equality.
