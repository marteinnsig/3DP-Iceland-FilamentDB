# Engineering Roadmap

Preserve native tensile, impact and stiffness ownership, Material Summary verification, and report/website gates. New test systems must be independently verifiable before downstream use.

## v41 Engineering Intelligence

Delivered in v41.0–v41.6:

- Explainable Engineering Advisor over existing score profiles.
- Evidence coverage and missing-data honesty.
- Strongest/weakest axis explanations and closest-alternative comparison.
- Recommendation Detail and Verification Center integration.
- Comparable alternatives and price-aware hidden-gem discovery.
- Specialist alternatives with explicit axis gains and trade-offs.
- Consistency and outlier-review intelligence sourced from Verified Material Summary CV and sample-count outputs.
- Explicit safeguards against claiming or removing specimen outliers from aggregate evidence alone.
- Canonical price, Inventory Engine and SQLite manufacturer context in Recommendation Detail and reusable prompts.
- Manufacturer and category rank, peer-count and group-average positioning over existing score profiles.
- Governed handoffs that carry existing Engineering Intelligence into canonical reports, methodology whitepaper context and Video Planner without recalculation.
- One internally calibrated repeatability scale shared by app, website, reports and whitepaper, with documented in-house equipment limitations.

Next:

### v41.7 - Report Portfolio Differentiation (Complete)

The report selector currently exposes several report names, but all legacy choices except Material Engineering Report fall back to substantially the same generic material report. Replace that fallback with explicit report contracts:

- Material Summary Report: dataset coverage, identity, test completeness and high-level verified results.
- Comparison Report: selected/visible material comparison with clearly declared comparison scope.
- Manufacturer Report: manufacturer portfolio, coverage, category position and product-level engineering context.
- Test Session Report: traceable test-session, specimen, method, equipment and result-quality context.
- Printing Recommendation Report: governed application, trade-off and print-use guidance without Video Planner hooks.
- Give every report type its own report code, headings, tables/charts and selected-versus-visible scope behavior.
- Add Verification Center checks that assert report-type identity and meaningful content differences so no option can silently fall back to the same HTML again.

Delivered for individual acceptance in v41.7.1:

- Canonical native Materials scope and filter parity.
- One current-report export containing both HTML and PDF.
- Distinct `REPORT-110` Material Summary Report and Verification identity checks.

Implemented for individual acceptance in v41.7.3:

- Distinct `REPORT-120` Comparison Report.
- Selected material as a highlighted anchor with up to five canonical visible peers.
- All-visible comparison respecting the current Materials search/filter scope.
- Engineering-axis leaders, score charts, side-by-side evidence/score/MSRP context and Verification identity checks.

Accepted on 2026-07-21 with visual review and Verification PASS.

Implemented for individual acceptance in v41.7.5:

- Distinct `REPORT-130` Manufacturer Report.
- Selected material expands to its manufacturer's complete active canonical portfolio.
- All-visible scope preserves the exact current Materials filter and supports multiple manufacturers.
- Portfolio coverage, global/category positioning, product-level engineering context, MSRP and product/video links.
- Verification identity, scope-expansion and report-difference checks.

Accepted on 2026-07-21 after visual review, selected-source clarification and
Verification PASS. Next: Test Session Report.

Implemented for individual acceptance in v41.7.6:

- Distinct `REPORT-140` Test Session Report.
- Selected-material native specimen/result-quality detail and raw recorded inputs.
- All-visible test-record ledger following the exact Materials filter.
- Settings Manager method/equipment constants and explicit missing-session-metadata disclosure.
- Verification identity, scope and traceability-honesty checks.

Accepted on 2026-07-22 after visual review and Verification PASS. Next:
Printing Recommendation Report.

Implemented for individual acceptance in v41.7.7:

- Distinct `REPORT-150` Printing Recommendation Report.
- Selected-material applications, strengths, limitations, trade-offs, workflow checks and alternatives.
- All-visible recommendation ledger following the exact Materials filter.
- Explicit manufacturer/printer settings boundary and no Video Planner/YouTube hooks.
- Verification identity, scope, settings-honesty and report-difference checks.

Accepted on 2026-07-22 after visual review and Verification PASS. All six
individual engineering reports are now accepted. Next: combined Engineering
Report Package.

Implemented for final portfolio acceptance in v41.7.8:

- One `Export Engineering Package` action for all six accepted reports.
- Independent canonical HTML/PDF/text/metadata/manifest/assets subpackages.
- Indexed package landing page, package manifest and JSON metadata.
- Existing selected/all-visible report contracts preserved without recalculation.
- Safe non-overwriting timestamped package folders and Verification structure checks.

Accepted on 2026-07-22 after a successful end-to-end six-report export, working
package index and Verification PASS. v41.7 Report Portfolio Differentiation is
complete.

### Future report extensions

After the combined package and startup-performance work, consider these as
separate evidence-driven additions rather than expanding the core six-report
portfolio:

- Experimental Research Report: one experimental series with baseline, runs,
  controlled variables, canonical analytics, charts and research conclusion.
- Verification & Data Quality Report: dataset coverage, missing evidence,
  specimen/sample coverage, repeatability/CV review, orphan checks and release
  Verification status for internal audit and methodology transparency.

A Material Family Benchmark should be implemented as a Comparison Report preset
rather than another report type. Batch/history, printer-profile and durability
reports remain blocked until their required canonical source data exists.

### Canonical Material Printing Profiles

Add a structured, SQLite-owned printing-profile section to Material Detail for
every canonical MaterialID. This work must land before REPORT-150 or public
recommendation outputs begin displaying exact printing settings; until then the
existing no-inference boundary remains mandatory.

Initial per-material fields:

- Nozzle temperature minimum and maximum (`°C`).
- Bed temperature minimum and maximum (`°C`).
- Print-speed minimum and maximum (`mm/s`).
- Cooling-fan minimum and maximum (`%`).
- Drying temperature (`°C`) and drying time (`hours`).
- Enclosure requirement using a controlled value such as `Required`,
  `Recommended`, `Optional`, `Not recommended`, or `Unknown`.
- Printer-profile reference/name.
- Slicer-profile reference/name and slicer identity/version where known.
- Source/provenance, source URL, checked date, and a concise validation note.

Implementation contract:

- MaterialID is the foreign key and SQLite remains the single source of truth.
- Use nullable typed numeric fields and explicit units; never encode ranges only
  as free text or silently replace missing values with zero.
- Add the fields as a focused Material Detail panel with validation, clear
  `Not recorded` states, and small additive schema migration/backwards
  compatibility.
- Preserve the distinction between a general manufacturer baseline and future
  printer/nozzle/material-specific profile variants. Start with one canonical
  baseline per MaterialID, but design identifiers so later profile variants do
  not require rewriting the base material table or report engine.
- Record whether values are manufacturer-published, internally validated, or
  provisional. Reports and website output must label provenance and must not
  present unvalidated values as universal settings.
- Extend exports/imports, diagnostics, backup/restore and Verification Center
  round-trip checks with the same fields.
- Once canonical data exists, REPORT-150, its public counterpart and Material
  Detail may consume the same typed profile. They must display `Not recorded`
  for missing settings and may not calculate or invent replacements.

### v41.7.2 - Canonical Material Projection Audit (Complete)

Removed stale material identity, scope and count dependencies before building
the remaining reports. The legacy `_materialsView` field and hidden import-cache
tab are gone; imported tables are bounded ingestion payloads and do not own the
current material universe.

- Inventory every `_materialsView` consumer and classify identity/scope usage separately from measurement/result usage.
- Route total, active, archived and visible material counts through the native SQLite-backed Materials collection.
- Route filters and material sets through canonical MaterialID joins so newly added and untested materials remain visible with honest missing-result states.
- Correct Rankings, Awards, Video Planner, analytics, AI/session context, dashboards, diagnostics and report helpers where they still treat the legacy projection as the material universe.
- Remove hard-coded/stale dataset-count wording from active UI and generated artifacts; historical documentation remains unchanged.
- Add Verification Center invariants for canonical total, filtered visible count, tested-result subset and MaterialID set parity across dependent surfaces.

Accepted on 2026-07-21 with Verification PASS, reviewed-tab stability and
confirmed Materials-filter propagation. Next: Comparison Report.

After user acceptance of this audit: Comparison Report. The combined Engineering Report Package follows only after all six reports have been accepted individually.

With the complete v41.7 Report Portfolio accepted, return to the Master Roadmap
for the dedicated Startup Performance & Safe Concurrency build. Profile
first, then apply lazy initialization, independent asynchronous reads and
measured CPU parallelism without moving WPF mutations off the UI thread or
weakening SQLite/MaterialID/Verified Material Summary ownership.

Continue engineering-assistance depth only where new verified evidence or an approved downstream workflow requires it.
