# Help Control and Field Ledger

Version: v61.0.8.2 disposable thermal persistence automation

Purpose: authoritative control-level inventory for the mandatory v50.4
exhaustive Help audit. `Docs/HELP_COVERAGE_MATRIX.md` remains the accepted
surface-level v50.2/v50.3 baseline; it is not control-level completion proof.

## Audit rules

Every supported user-facing candidate must receive one stable ledger key and
one classification:

- `action`: button, menu action or other invoked command;
- `input`: text, numeric, date, password or multiline value;
- `choice`: checkbox, radio option, selector or filter;
- `editable-column`: user-editable grid/custom-grid cell type;
- `status`: validation, readiness, progress or evidence needed to use an action;
- `read-only`: output or interpretation that must be understood but cannot be edited;
- `layout`: resize/reset/display-only behavior;
- `unsupported`: dead or retired UI with an explicit removal owner.

Coverage is complete only when the row records:

1. stable key and exact visible path;
2. XAML/runtime/custom owner and canonical service/data owner;
3. exact Help destination or subsection;
4. purpose, prerequisites, allowed values, units and default meaning;
5. validation, save timing and failure behavior;
6. side effects, confirmation and historical-data rules;
7. cross-tab inputs, downstream handoff and external boundaries;
8. deterministic evidence or an explicit manual-only reason.

AutomationId presence, a click-handler count or a tab overview is not coverage.

## Discovery baseline

Source snapshot: canonical runtime-accepted v55.0.6 on 2026-07-28.

| Source | Discovered candidates | Reconciliation requirement |
|---|---:|---|
| Top-level tabs | 22 | Preserve accepted overview and contextual mappings |
| Nested tabs | 16 | Preserve accepted nested-aware mappings |
| XAML buttons | 150 | Map visible action, handler, owner and Help subsection |
| XAML menu items | 31 | Separate headings/separators from invoked commands |
| XAML text boxes | 35 | Determine editable, read-only, multiline and generated output |
| XAML password boxes | 2 | Record secret ownership and non-persistence boundary |
| XAML combo boxes | 49 | Record source, allowed choice, default and save timing |
| XAML checkboxes | 10 | Record true/false effect, persistence and dependent states |
| XAML data grids | 31 | Reconcile grid-level read-only state and runtime columns |
| XAML grid-column declarations | 359 | Resolve binding, grid owner and actual editability |
| Runtime-built windows/dialogs | open | Inventory constructors, generated controls and confirmations |
| Owner-drawn/custom grids | 6 known | Reconcile column builders and supported edit interactions |

The raw XAML column count is deliberately not labelled editable. Column-level
`IsReadOnly` can inherit or be overridden by grid-level state, binding mode,
templates and runtime behavior. Each candidate must be reconciled with its
grid owner and edit handler.

## Exact XAML candidate inventory

`Docs/HelpControlInventory.tsv` is generated from the registered XAML source
only with an explicit `Tools/Test-HelpControlCoverage.ps1 -UpdateInventory`
action. A normal gate run compares all generated rows with the committed file
and fails on drift.

Current exact inventory:

| Owner | Candidates |
|---|---:|
| v50.4.1 | 219 |
| v50.4.2 | 267 |
| v50.4.3 | 156 |
| v52.1 | 7 |
| v52.2 | 3 |
| v52.3.2 | 1 |
| v53.0.2 | 2 |
| v53.0.4.1 | 6 |
| **Total** | **661** |

v52.1 adds seven Settings candidates for provider/model preference, protected
credential ownership and local-only foundation diagnostics. All route to
`settings.controls-fields`; none enables a live OpenAI request.

v52.2 adds three AI Assistant pilot actions for exact payload preview,
consent-gated generation and active-request cancellation. They route to
`ai.controls-fields`; automation covers preview only and never performs live
network I/O.

v52.3.2 adds one secret-safe operational-evidence clipboard action. It remains
disabled until a live attempt and never retains raw payloads or credentials.

v53.0.2 adds the landed-cost currency selector and default-No Draft override.
Both route to `purchase-orders.controls-fields` and remain disabled for legacy,
calculated or non-Draft orders.

v53.0.4.1 adds six hidden landed-cost lifecycle controls. They are authorized
only by an exact disposable manifest and remain unsupported owner-facing UI.

| Initial source classification | Candidates |
|---|---:|
| Actions | 181 |
| Choices | 59 |
| Input candidates | 33 |
| Grid candidates | 11 |
| Editable-column candidates | 318 |
| Read-only fields | 4 |
| Read-only grids | 20 |
| Read-only columns | 38 |

Candidate classification is intentionally conservative. The 318 column and
11 grid candidates are not accepted as editable until binding, grid-level
state, templates and handlers agree. The eight Application-shell actions are
automation-only controls and require an explicit supported/unsupported
disposition rather than user Help prose.

## Known owner-drawn/custom column registries

| Surface | Declared columns | Current reconciliation owner |
|---|---:|---|
| Fast Materials | 52 | v50.4.1 |
| Fast Tensile | 45 | v50.4.2 |
| Fast Impact | 45 | v50.4.2 |
| Fast Stiffness | 18 | v50.4.2 |
| Fast Settings | 6 | v50.4.1 |
| Fast Base Materials | 23 | v50.4.1 |

These 189 declarations are separate from the XAML grid-column declarations.
Each builder passes through `PrototypeColumnKey`, and existing Verification
requires unique keys and exact expected counts for all six registries. The
audit must use each property/layout key and editor kind, not only the header.

## Runtime-generated registry

Eight runtime surface owners declare 42 exact controls in
`Docs/HelpControlCoverageRegistry.json`. The normal gate requires unique
control keys, a live entry point, a live Help destination and a valid status.

The hidden PDF print host is explicitly `unsupported` as a user Help surface.
It remains supported internal report infrastructure. Verification now records
its v51.3 classification owner, while Diagnostics records its v53.0.4.4
aggregate landed-cost reconciliation owner. Other surfaces retain their
bounded v50 ownership.

## Post-v50 reconciliation

The deterministic gate requires explicit coverage ownership for v51.1-v51.4,
v52.1-v52.3.2 and v53.0.2-v53.0.5. It also requires exact Help markers for:

- Owner, Verification/Disposable and Clean/Readiness runtime profiles;
- Mandatory versus CanonicalDataDependent Verification classification;
- Windows Credential Manager and OpenAI payload/evidence boundaries;
- landed-cost defaults, manual fallback, calculation provenance and
  Diagnostics non-mutation.

Inventory provenance retained in SQLite but not exposed as an editable visible
column is explained as read-only historical evidence in Inventory Help. It is
not misclassified as a user-editable XAML field.

## Delivery ownership

| Increment | Required ledger scope | Exit condition |
|---|---|---|
| v50.4.0 | All discovery sources and stable registry/gate contract | Every candidate has a bounded owner |
| v50.4.1 | Data, purchasing, inventory, cost and configuration | No unexplained supported control/field |
| v50.4.2 | Measurements, Experimental Testing and analysis | No unexplained supported control/field |
| v50.4.3 | Output, publishing, assistant, creator, menus and runtime windows | No unexplained supported control/field |
| v50.4.4 | Cross-scope reconciliation, deterministic drift gates and tester | Zero gaps plus owner acceptance |
| v53.0.5 | v51-v53 Help, owner and runtime-surface reconciliation | Zero unexplained post-v50 gaps |
| v54.0.5 | Six Materials facets, persisted scope, exact AI/collection evidence | Owner runtime accepted |
| v54.0.6 | Hidden scalar filter retirement and final Help inventory | Canonical; 661/661 Help audit |
| v55.0.2 | Named default-No Base Material deletion and cancellation safety | Candidate Help and Verification |
| v55.0.5 | Runner-owned reviewed cleanup and diagnostics ownership | Candidate Help and Verification |
| v55.0.5.1 | Shared safe-delete dialog for Material and Base Material | Owner runtime feedback correction |
| v55.0.6 | Operational safety runtime acceptance and closure | Canonical; Verification PASS |
| v61.0.8.2 | Hidden disposable thermal persistence controls | Tester-only; bounded exact-ID authorization |

## Initial findings and risks

- The accepted Help catalog is broad and substantive but is organized mainly
  by workflow subsections, not by a declared registry of every control/field.
- Several named XAML controls already have AutomationIds, but many do not.
  IDs will be added only where stable lookup provides useful evidence.
- Runtime-built Verification and Diagnostics controls are safety-sensitive.
  Help/tester coverage must not authorize recalculation or export mutations.
- Recovery, updater, Production, FTPS and delete controls require exact
  default-No and historical-data wording; read-only tester inspection only.
- Custom-grid columns require units, editor type, validation and save timing
  from their column definitions and handlers, not guesses from their labels.
- Repeated column shapes may share one precise Help table only when every
  covered key is declared; prose implying coverage is insufficient.

## Open implementation choices

1. Prefer a source-controlled registry consumed by Verification and Help.
   This gives one machine-checkable coverage owner.
2. A generated report alone is insufficient because generated output can
   silently normalize away unsupported or runtime-only controls.
3. Per-control Help popups are not required. Exact searchable subsections and
   field/control tables can satisfy coverage with less UI clutter.

The registry format and complete candidate rows are the remaining v50.4.0
implementation work. No exhaustive Help content is accepted until that ledger
can detect missing additions.
