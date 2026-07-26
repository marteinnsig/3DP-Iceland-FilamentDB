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

Canonical implementation uses one non-persisted service for Recommendation
Detail, hidden-gem ordering and the existing Manufacturer intelligence caller.
Recommendation Detail shows the index, Overall Score, canonical MSRP USD/kg,
comparison scope and missing-data reason. The value is explicitly comparative,
not a physical property. No public report allowlist is expanded.

Owner runtime accepted exact PLA/ASA scope refresh, alternatives and explicit
recommendation MaterialID/MSRP identity. Full Data Verification passes 354/354;
v48.0.2 is canonical.

### v48.0.3 Usage Event Contract

- prototype the capture and correction workflow without touching owner data;
- decide whether events belong to a print job, test session or a general usage
  ledger;
- approve printer/profile, inventory-lot and specimen-count relationships;
- only then propose additive SQLite tables, Excel recovery and diagnostics.

#### Ownership audit findings

The current runtime has no canonical general Print Job or Test Session entity.
Native tensile, impact and stiffness storage is MaterialID-oriented and owns
raw inputs, measured dates, notes and derived sample counts. Those counts prove
measurement coverage only; they do not prove how many specimens were printed,
accepted, rejected or how much filament/time was consumed.

Experimental Runs are canonical only inside Experimental Testing. They may be
an optional usage-event reference when an event genuinely belongs to an
experimental run, but they cannot own normal native testing or production
printing.

`InventorySpoolItems` owns current physical spool state and optional purchase
provenance. `RemainingWeightG` is an editable state snapshot, not an immutable
consumption ledger. Purchase lines prove acquisition and landed cost; they do
not prove usage.

No current report, website payload or public allowlist owns usage history.
REPORT-140 Test Session means measurement traceability and must not be treated
as a persisted session/job identity. Existing backup/restore and governed Excel
recovery cover 21 exact canonical tables; a future usage table must be added to
both sides of that strict contract.

#### Recommended canonical boundary

Use one general append-only Usage Event ledger as the ownership root. Do not
make native measurement rows, Experimental Runs, Inventory or Materials own
editable lifetime totals.

Each original event requires:

- stable `UsageEventId` and required canonical `MaterialId`;
- event type from a governed catalog: test preparation, test print, production
  print or inventory adjustment;
- occurred-at UTC timestamp plus created-at UTC evidence;
- optional `InventoryItemId`, `ExperimentalRunId`, future `PrintJobId` and
  future `TestSessionId`;
- optional filament-used grams, print-duration minutes and hands-on/test
  duration minutes;
- optional produced, accepted and rejected part/specimen counts;
- source/provenance, note and creator/origin evidence;
- all missing numeric values stored as null/`Not recorded`, never zero.

Material, inventory, job and test totals are read-only projections over events.
The event stores observed facts, not calculated cost. Job pricing and immutable
quote snapshots remain separately owned by v48.1.

#### Correction and deletion contract

Accepted historical events are never overwritten or deleted through normal
workflow. A correction writes:

1. one reversal event referencing the exact original `UsageEventId`; and
2. when appropriate, one replacement event with the corrected observed facts.

The reversal must negate the original quantitative contribution exactly and
retain the same Material/inventory relationship. A replacement is independent
and must not silently inherit missing values. Duplicate reversals, reversal of
a reversal and cross-Material correction are invalid.

Hard deletion is reserved for disposable automation cleanup or failed,
uncommitted creation inside one database transaction. Archive is not a
substitute for reversal because hidden history would make totals dishonest.

#### Inventory relationship

An event may reference one canonical inventory spool. It must never silently
move consumption to another spool or fuzzy-match by Material name. When an
inventory link is present, its MaterialID must match the event MaterialID.

Whether saving a usage event also decrements `RemainingWeightG` must be decided
before schema work. The recommended implementation is one atomic service-owned
transaction that writes the event and updates the spool projection together,
with reversal applying the exact opposite delta. Direct editing of remaining
weight must remain an explicit inventory adjustment event once this path
becomes canonical. Until that replacement is runtime accepted, the current
inventory edit path remains supported.

#### Capture workflow prototype

Prototype in a disposable profile before adding schema:

1. Start from a selected canonical MaterialID.
2. Choose event type and occurred-at time.
3. Optionally choose an exact compatible inventory spool.
4. Record only observed grams, print minutes, hands-on/test minutes and counts.
5. Preview the exact event and any inventory delta.
6. Save once; later changes use explicit Reverse or Correct actions.
7. Show a read-only MaterialID ledger and totals with `Not recorded` coverage.

Do not expose future Print Job/Test Session selectors until those identities
exist. An optional Experimental Run selector is valid only inside Experimental
Testing context.

#### Additive implementation plan

1. **v48.0.3 — Contract acceptance:** approve event vocabulary, null/zero
   semantics, reversal rules, inventory atomicity and private/public boundary.
   No schema, UI, seed or runtime release change.
2. **v48.0.4 — Disposable domain prototype:** add pure models/services and
   deterministic projections/correction validation without owner-data writes.
3. **v48.0.5 — Canonical persistence and recovery:** add schema only after
   prototype acceptance; include foreign keys, indexes, diagnostics, SQLite
   backup/restore and governed Excel recovery.
4. **v48.0.6 — Bounded UI and automation:** add MaterialID-led capture, ledger,
   reverse/correct workflow and a disposable tester scenario with exact
   baseline/final business-state recovery.
5. **v48.0.7 — Internal analytics:** add private usage projections only after
   CRUD/recovery acceptance. Public reports/website remain unchanged unless a
   later explicit allowlist increment is approved.

#### Owner decision — 2026-07-26

v48.0.3 is approved and complete as a documentation/contract increment.
Approved decisions:

- one append-only general Usage Event ledger;
- explicit reversal plus optional replacement correction;
- atomic event/inventory update when an InventoryItemID is linked;
- grams provenance distinguishes measured actual from slicer estimate;
- duration is stored in seconds and may be presented as minutes/hours;
- current direct inventory-weight editing remains supported until the event
  replacement path is runtime accepted;
- usage remains private by default;
- v48.0.4 may implement a pure disposable domain prototype only, without
  schema, UI, seed or owner-data writes.

#### v48.0.4 canonical implementation

The candidate adds immutable Usage Event records plus one pure domain service.
Synthetic Verification proves:

- canonical MaterialID and exact inventory/material identity validation;
- required UTC timestamps and seconds-based duration;
- measured/slicer grams provenance with nullable missing values;
- exact reversal plus replacement correction and duplicate-reversal blocking;
- read-only totals where missing evidence remains null/`Not recorded`;
- equal/opposite inventory delta plans without mutating Inventory.

No database, UI, seed, public output or owner-data caller is added.

Owner runtime accepted normal startup and all four usage-domain Verification
contracts. Full Data Verification passes 358/358; v48.0.4 is canonical.

#### v48.0.5 canonical implementation

Schema v34 adds the private append-only UsageEvents ledger. Original events,
reversals and replacements use the accepted domain validation, while linked
Inventory deltas commit in the same SQLite transaction. Governed Excel
recovery expands from 21 to 22 tables and exact schema-v33 packages migrate
with no inferred usage rows.

The disposable CRUD scenario owns deterministic create, restart, correction,
Inventory reconciliation and cleanup evidence. No normal Usage UI, report or
website allowlist is added in this increment.

Owner runtime accepted normal behavior and Full Data Verification 359/359.
v48.0.5 is canonical; v48.0.6 owns the bounded Usage UI and automation.

#### v48.0.6 candidate implementation

One private Usage tab now owns exact MaterialID-led capture, optional
same-Material Inventory selection and a read-only immutable ledger. Record
Usage appends an original event. Correct Selected prepares a replacement and
Save Correction appends reversal plus replacement; accepted rows have no
normal edit/delete path.

UI minutes are converted to canonical whole seconds. Print Job, Test Session,
general Experimental Run, public reports and website output remain outside the
surface. Existing disposable CRUD authorization now verifies the visible tab,
stable AutomationIds and ledger state across restart.

Owner accepted optional no-spool capture when Inventory is empty, then added
canonical spools and accepted atomic linked-weight updates and correction.
Full Data Verification passes 360/360; v48.0.6 is canonical.

#### v48.0.7 candidate implementation

Private selected-Material analytics now distinguish immutable ledger-row count
from effective events. Reversal rows and reversed originals do not inflate
effective-event or evidence coverage, while quantities continue to net the
complete append-only ledger.

The Usage workspace presents net grams, print/hands-on duration, part counts
and effective evidence coverage. No schema, price/cost calculation, public
report or website payload is changed.

Owner accepted correction netting, evidence coverage and summary presentation,
and visually confirmed Usage remains absent from report and website previews.
Full Data Verification passes 361/361; v48.0.7 is canonical.

#### Risks and open decisions

1. Decide whether inventory decrement is mandatory whenever
   `InventoryItemId` is present. Optional decrement would create two truths.
2. Define whether grams are measured actual usage or slicer-estimated usage;
   provenance must distinguish them.
3. Decide whether duration accepts decimal minutes or whole seconds internally;
   UI units must not control storage precision.
4. Define timezone capture and display while retaining canonical UTC.
5. Define creator/origin identity without introducing unsupported user-account
   claims.
6. Decide how existing direct `RemainingWeightG` edits transition after the
   event path is accepted; no caller may be retired early.
7. Keep usage private by default. Supplier, inventory-lot, internal notes and
   time history must not enter public payloads implicitly.

### v48.1 Job Pricing

Keep job quotations separate from analytics. A quote must snapshot MaterialID,
effective cost provenance, printer profile, every formula input, currency/rate,
calculation version and timestamp so later Settings changes cannot rewrite it.

#### v48.1 approved pricing and ownership contract — 2026-07-26

The standalone HTML calculator was reviewed as business-rule reference only.
Its Advanced Inputs mix global defaults with printer-specific ownership and
must not become one editable quote form.

Approved quantity and currency decisions:

- filament is **grams per part × quoted quantity**, exactly once;
- ISK is the default calculation and quote currency;
- another supported quote currency requires an explicit
  `ISK per 1 {currency}` rate;
- every quote snapshots the exact rate, source label and calculation time;
- printer uptime is entered and stored as an unambiguous 0–100 percent.

Global Pricing Settings own material efficiency, labor hourly rate,
electricity cost per kWh, default printer buffer, target margin, quote currency
and company/prepared-by defaults.

A separate Printer catalog owns stable `PrinterID`, name/model, purchase cost,
additional upfront cost, annual maintenance, estimated life, productive uptime
percent, average power, optional buffer override, active/archive state, notes
and provenance.

Material cost selection is explicit:

1. a canonical MaterialID uses governed `LandedCostUsdPerKg`, converted through
   snapshotted currency rates;
2. when no Material exists or landed cost is missing, manual cost per kg plus
   explicit source currency is allowed;
3. manual evidence remains manual and is never rebound silently to a later
   MaterialID.

Every saved quote snapshots PrinterID and printer inputs, global settings,
MaterialID or manual material evidence, grams per part, quantity, time inputs,
additional lines, currency rates, component costs, margin and final price.
Later catalog or Settings changes never recalculate historical quotes.

Bounded delivery:

1. **v48.1.0 — Pricing and ownership contract:** formula decisions,
   provenance, snapshot boundary and reuse attribution; no schema/UI.
2. **v48.1.1 — Printer and pricing-settings foundation:** canonical Printer
   catalog, global settings, recovery, diagnostics and disposable CRUD.
3. **v48.1.2 — Immutable quote workflow:** Material/manual cost selection,
   exact calculation service, snapshots, quote UI and governed export.

The Print Farm Academy attribution in the reference HTML remains a
reuse-review requirement. The app will encode approved business rules
independently rather than copy the external presentation or JavaScript.

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
v48.0.2 formula, missing-input, scope and price-identity probes pass.

The accepted v48.0.3 contract audit changes no deterministic runtime behavior. Therefore
AutomationRunner, AutomationIds, scenarios, seed database and Full Data
Verification remain unchanged. A future usage schema requires a new bounded
scenario or explicit CRUD extension with event/reversal integrity, inventory
atomicity, recovery round-trip, exact cleanup and business-state equality.
