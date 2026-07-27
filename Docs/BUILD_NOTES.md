# Current Build Notes - v51.3.0

## Verification Classification

Verification explicitly separates runtime profile identity/capabilities from
the Full Data Verification or Application Readiness data profile. Every check
is exported with Mandatory or CanonicalDataDependent applicability; designated
runtime, path, recovery, updater, deployment and diagnostic/support checks
also carry a mandatory-evidence flag.

N/A now requires an explicitly governed check name/family. Free-form failure
detail text alone cannot convert a mandatory failure to N/A. Clean Debug
profile `20260727145307-ec78b262` passes 281/281 applicable with 100
CanonicalDataDependent N/A, zero mandatory N/A and all 16 mandatory-evidence
checks passing. Release and owner acceptance remain pending; schema stays v37.
Clean Release profile `20260727145433-df2701f4` repeats 281/281 plus 100
classified N/A with exact state. Populated Release profile
`20260727145601-56734ac4` passes 381/381 with zero N/A and exact state.
Owner Production and Clean runtime acceptance pass on 2026-07-27; schema
remains v37 and v51.4 is current.

## Clean Readiness Profile

The new `CLEAN / READINESS` runtime is a manifest-governed, seedless
first-run profile. It creates a new schema-v37 SQLite database only inside its
disposable root and blocks owner database/preferences, credentials,
Production, FTPS, updates and mutating tester authorizations.

Application Readiness now classifies ten exact recovery/reporting composite
gates as zero-data dependent while retaining unexpected release, identity,
schema, executable, path and security failures as mandatory FAIL.
Profile `20260727143116-3138cb93` passes 280 applicable checks with zero FAIL
and 100 explicit N/A checks. Controlled restart preserves the clean identity,
zero Materials and exact post-initialization state. Release clean profile
`20260727143346-142a5972` repeats 280/280 applicable and 100 N/A; populated
Release smoke `20260727143423-7961372f` passes Full Data Verification 380/380
with exact state recovery. Debug/Release app and tester builds, Help/document
audits and the NuGet vulnerability scan pass. Owner runtime acceptance passes
on 2026-07-27; schema remains v37 and v51.3 is current.

An owner-run Clean test found a transient anonymous WPF Help-menu popup race in
the tester's unexpected-window enumeration. A bounded one-second retry applies
only while both AutomationId and name are empty; identified or persistent
windows remain blocked. Two consecutive Clean Release profiles
`20260727144106-e4feeaf6` and `20260727144139-15e29155` pass, followed by
populated Release profile `20260727144213-6e88b752` at 380/380 with exact state.
The owner rerun completes without unexpected dialogs or PowerShell errors.

## Runtime Profile Foundation

The main shell now always identifies the active ownership contract. Normal
runtime shows `OWNER / PRODUCTION`; disposable tester runtime shows
`VERIFICATION / DISPOSABLE — {ProfileId}`. System Diagnostics and Verification
report the same profile kind, ID, database/preferences/output ownership and
effective owner-database, Production/FTPS and update capabilities.

The existing automation manifest, containment, executable-hash and scenario
authorization implementation remains the safety owner. The new descriptor does
not grant actions or skip crash, recovery, security, updater transaction or
support evidence. Help and zero-gap inventory are aligned.

Debug/Release app and tester builds pass with zero warnings/errors. Disposable
profile `20260727141117-d8438ac6` passes Full Data Verification 379/379 with
exact logical/business-state equality. Owner Verification and Diagnostics
confirm `OWNER / PRODUCTION`, the canonical database path and governed
capabilities. Owner runtime acceptance passes on 2026-07-27; schema remains
v37 and v51.2 is current.

## Help Zero-gap Final Acceptance

The left Help navigation is now an expandable category/topic tree. Ordinary
navigation shows all categories, opens the selected topic's category and keeps
the remaining hierarchy compact. Search shows matching categories only,
expands them and selects the highest-ranked result without changing accepted
highlight and jump behavior.

Stable category/topic AutomationIds let smoke prove the hierarchical UI without
authorizing data mutation. The source-derived inventory contains zero planned
rows. Debug/Release app and tester builds pass with zero warnings/errors.
Disposable profile `20260727134442-edf82227` passes Full Data Verification
378/378 with exact logical/business-state equality. Final owner runtime/UI
acceptance passes on 2026-07-27; v50.4.4 and parent v50 are canonical, schema
remains v37 and v51.0 research is current.

## Output and Runtime Control Help

Five detailed Help topics cover Reports, Website Export, AI Assistant, YouTube
Research, application menus and the supported runtime windows. They name the
individual controls and fields and explain editability, local/external effects,
credentials, confirmations, failure handling and evidence.

The registry routes all 156 v50.4.3 XAML candidates and the eight runtime
surfaces to these destinations or an explicit unsupported-user boundary.
Verification and smoke enforce topic depth, unique search routing and the
Production/FTPS/restore/update/recalculation safety separation.

Debug/Release app and tester builds pass with zero warnings/errors. Disposable
profile `20260727132731-ea4e1d20` passes Full Data Verification 377/377 with
exact logical/business-state equality. Owner runtime/readability acceptance
passes on 2026-07-27; schema remains v37 and v50.4.4 is current.

## Engineering Control and Field Help

Four new detailed Help topics cover native Measurements, Experimental Testing,
interactive Material Detail and Rankings/Category/Awards/Insights. They name
the individual controls and column groups and explain ranges, units,
editability, validation, persistence, baseline/history scope and read-only
projection behavior.

The registry routes 267 XAML candidates and 108 custom native measurement
columns to these leaf topics. Verification and smoke enforce topic depth,
ownership markers and unique Help search routing.

Debug/Release app and tester builds pass with zero warnings/errors. Disposable
profile `20260727131457-dee03dd4` passes Full Data Verification 376/376 with
exact logical/business-state equality. Owner runtime/readability acceptance
passes on 2026-07-27; schema remains v37 and v50.4.3 is current.

## Data and Configuration Control Help

Nine new detailed Help topics cover every registered control/field group in
Materials, Manufacturers, Purchase Orders, Inventory, Usage, Printers, Print
Job Quotes, Base Materials and Settings. They distinguish editable inputs from
read-only/calculated output and explain units, selectors, validation, save
timing, destructive actions and historical-data boundaries.

The Help registry routes 222 XAML candidates plus 81 custom-grid columns and
the Bulk Update runtime dialog to these leaf topics. Verification and smoke
enforce topic depth, safety markers and representative unique search routing.

Debug/Release app and tester builds pass with zero warnings/errors. Disposable
profile `20260727125607-36733e81` passes Full Data Verification 375/375 with
exact logical/business-state equality. Owner runtime/readability acceptance
passes on 2026-07-27; schema remains v37 and v50.4.2 is current.

## Safety and Recovery Help

Eighteen searchable destinations now cover File/Recovery, storage ownership,
signed update discovery/apply/recovery, application release/update publishing,
Website safety, Recovery Center, Verification, Diagnostics and five
symptom-led troubleshooting paths.

The content explicitly separates read-only Refresh, integrity and evidence
actions from mutating recalculation, restore, storage move, guarded update and
live FTPS. Application publishing remains distinct from Website publishing;
SQLite backup is retained update evidence and is never restored automatically.

Runtime-built Recovery and Diagnostics surfaces plus their safety-relevant
actions have stable AutomationIds. Smoke opens and inspects them without
invoking any mutating action, then exports Verification evidence.

The first smoke timed out because `SystemDiagnosticsWindow` was accidentally
assigned to the Verification window initializer. The ID was moved to the
correct runtime-built Diagnostics window. Profile
`20260727120029-14c0556f` passes 374/374 and exact state equality. Owner
runtime/safety acceptance passes on 2026-07-27; parent v50 continues at v50.4.

## Contextual Help Coverage

All 22 top-level tabs and the 16 nested Experimental/Material Detail tabs now
have unique AutomationIds. `Help for Current View` and F1 share one resolver:
ordinary top-level tabs open their overview, Experimental editors/results open
their exact nested topic, and Material Detail opens its selected nested topic.
Documentation continues to open the Start-to-finish whole-system overview.

Four planned Experimental destinations complete the nested registry:
Tensile, Impact, Stiffness and the Results overview. Searchable Tools validation
and Help-menu references document mutating versus read-only boundaries.

The unsupported disabled Website Export Preview menu item is retired. Its
replacement only selects the supported Website Export tab and never generates
or publishes output.

Smoke visits exactly 22/22 top-level and 16/16 nested tabs, asserts no
unexpected window, exercises representative contextual destinations and
retains scenario separation from CRUD, Reports, Recovery, Updater, Production
and FTPS. Profile `20260727113500-3bc427ca` passes Full Data Verification
373/373 and exact logical/business-state equality. Owner runtime/UI acceptance
passes on 2026-07-27; parent v50.2 is complete and v50 continues at v50.3.

## Output and Tool Reference

The v50.2.0 matrix defines four overview and 30 leaf/support destinations for
Reports/PDF, Website Export, AI Assistant, YouTube Research, Whitepaper,
Changelog and About. All 34 now exist in the central searchable catalog.

Owner review correctly found the first leaf-topic draft too close to
placeholders. Every v50.2.3 destination now has substantive multi-paragraph
guidance covering the visible controls, input scope, explicit write boundary,
validation/failure behavior, evidence and safe handoff.

Reports now names all twelve templates and each package/public family. Website
separates root/template ownership, Preview, local Production, FTPS Test/live
Production, restore and evidence. AI covers every brief action, sessions,
collections, workflow status/legacy binding and output. YouTube covers all
seven clipboard actions and every displayed planning projection.

Reports placeholder text no longer requests import or describes Material
Engineering Report as the first production report. No schema, report model,
publishing, credential or creator-tool behavior changes.

Verification requires every stable ID, four top-level mappings, at least 300
characters and multiple paragraphs per destination, plus representative
control/write/safety markers. Smoke searches the local Production-versus-FTPS
and creator-calendar boundaries.

The first smoke reached the new title-ranked Report scope leaf but the older
wrapping assertion still expected overview content. The normalized
`scope and output folder` probe now belongs to that leaf. Profile
`20260727110155-90afdea3` passes Full Data Verification 372/372 and exact
logical/business-state equality.

The expanded-content rerun first preserved the stable calendar search phrase,
then exposed and corrected a CRLF-only paragraph-depth assumption. Disposable
profile `20260727111924-0056eda1` now passes the detailed contract, Full Data
Verification 372/372 and exact logical/business-state equality. Debug and
Release build with zero warnings; documentation and vulnerability gates pass.
Owner runtime/content acceptance passes on 2026-07-27. v50.2.3 is canonical;
the parent v50 milestone continues at authoritative increment v50.2.4.

## Testing and Analysis Reference

The v50.2.0 matrix defines 22 stable destinations across native Tensile,
Impact and Stiffness; Experimental Series, Runs, editors and Results;
Material Detail nested views; and Rankings, Category Rankings, Awards and
Dashboard Insights. All are now searchable in the central Help catalog.

The references distinguish editable raw inputs from ResultsService outputs,
auto-save from read-only analysis, current visible Materials scope from
selected-Series Experimental scope, and active history from optional inactive
comparison. Rankings default Top 25 and Category Rankings default 10 rows per
group remain separate contracts.

Four stale UI messages are corrected: Material Detail Notes no longer calls the
whole app read-only; Rankings/Category/Awards no longer request import; Awards
no longer claims future website ownership. No schema, calculation or data
persistence behavior changes.

Full Data Verification requires all 22 unique IDs, nine top-level mappings and
representative save/scope/lifecycle markers. Smoke searches the Top 25 and
Notes corrections. Debug compile passes; remaining gates and owner acceptance
are pending.

Debug/Release, documentation, roadmap and vulnerability gates pass. Disposable
profile `20260727103712-93b32f7b` passes the expanded Help searches and Full
Data Verification 371/371 with exact logical/business-state equality.

Owner review found that `Experimental Results Dashboard/Table` did not match
natural searches for `Experimental Dashboard/Table`. The visible titles now
use those direct names, and Series Help lists Dashboard, Table and Charts.

The first retest showed that the Series overview then won the catalog-order
search before the exact Table title. Search relevance now ranks title, summary,
category and body in that order. The accepted Purchase Order smoke assertion
was aligned with its now-higher-ranked direct reference. Profile
`20260727104534-dc519c51` passes 371/371 with exact state equality.

Owner review found that Help > Documentation inherited the active Materials
context and opened Materials reference. The menu action now opens the canonical
Start-to-finish whole-system overview; F1 remains contextual.
Profile `20260727105308-cc3ce0b1` passes the overview contract, Full Data
Verification 371/371 and exact state equality.

Owner runtime/visual acceptance passes all testing/analysis references,
terminology corrections, Experimental discovery, search relevance and the
menu-versus-F1 entry behavior. v50.2.2 is canonical; parent v50 continues at
v50.2.3.

## Data, Cost and Configuration Reference

The authoritative v50.2.0 coverage matrix identifies nine data, purchasing,
cost and configuration surfaces for this increment. Each now has one stable
searchable Help section that records purpose, commands, fields, filters,
validation, save behavior, destructive boundaries and downstream ownership.

Materials and catalog references distinguish identity relationships, archive
history, exact-name binding, auto-save and publication choices. Purchasing,
Inventory and Usage references distinguish landed-cost persistence, receiving,
spool creation, calculated refresh, explicit event commits and append-only
corrections. Printer, Quote and Settings references distinguish prospective
inputs from saved quote history and protected purchasing evidence. Quotes can
be explicitly deleted and are not described as immutable.

Full Data Verification requires all nine stable IDs, representative contract
phrases and exact top-level tab mappings. The existing safe smoke scenario adds
read-only searches for the Materials save boundary and Printer-to-quote
saved-history handoff. No schema, data, calculation or Production/FTPS behavior
changes. Debug compile succeeds with zero warnings; remaining gates and owner
runtime/visual acceptance were pending at implementation time.

The first disposable run `20260727100746-ddc0c805` correctly failed 369/370
because a Verification marker crossed an intentional source line break; Help
search/navigation itself passed and no data contract failed. Markers now bind
to stable content tokens rather than source formatting. Disposable smoke
`20260727101000-dddd3159` passes the expanded Help contract, Full Data
Verification 370/370 and exact logical/business-state equality.

Owner review then removed the misleading `immutable` Quote label. The current
reference describes read-only saved history, no automatic recalculation and
supported explicit deletion. Correction profile
`20260727101828-1f53f4e1` passes 370/370 with exact state equality.

Owner follow-up found that `lifecycle` matched visible topic summaries while
the renderer highlighted body text only. Category, title, summary and body now
share one case-insensitive highlighter. Hidden keyword-only matches were
removed so every returned result has a visible match. Start-to-finish step 6
now says `retained purchase evidence`, not `immutable purchase snapshot`.
Profile `20260727102551-3b35ebbc` passes body/summary highlighting, Full Data
Verification 370/370 and exact state equality.

Owner runtime/visual acceptance passes all nine references, corrected Quote and
purchase wording, wrapping and highlight behavior in both headers and body.
v50.2.1 is canonical; parent v50 continues at v50.2.2.

## Prior v50.1.0 Start-to-finish Workflow Guide

## Start-to-finish Workflow Guide

Three independent read-only code traces mapped Purchasing/Inventory/Materials,
native and Experimental measurements/analysis, and Reports/Website/Verification
publishing. They found that the v50.0 overview was directionally safe but
compressed several distinct mutation and scope boundaries.

The v50.1.0 candidate records the exact ordered actions and corrects misleading
guidance: Purchase grids auto-save; landed-cost calculation persists; receiving
does not create Inventory; native measurements calculate and auto-save; main
Website DATA and public-report permissions are distinct; Production generation
and live FTPS are separately confirmed.

Owner review found that search filtered topics without locating the match and
that source-code line breaks caused visually clipped phrases. Search now
highlights every body match and scrolls the first match into view. Rendering
collapses only single source line breaks inside a paragraph while preserving
intentional blank-line paragraph boundaries. Automation requires the normalized
`scope and output folder` phrase.

The existing smoke scenario now searches for `landed costs` and requires the
received-spool and `READY FOR PUBLISH` handoffs in the rendered Help body. Full
Data Verification owns six required action/save/scope markers. No schema or
canonical calculation contract changes.

An initial disposable run correctly failed 369/370 because the Verification
marker expected wording different from the canonical second live-FTPS
confirmation text. The contract was aligned to the rendered guide. Disposable
smoke `20260727092658-c116167b` then passes Full Data Verification 370/370 with
exact logical and business-state equality. Owner content/visual acceptance is
pending.

After the search/highlight/wrapping correction, disposable profile
`20260727094039-c70f0cd7` passes the central Help workflow, normalized wrapping,
Full Data Verification 370/370 and exact logical/business-state equality.

Owner follow-up exposed a WPF refresh race when filtering retained the same
first topic and when Search was cleared. Rendering is now explicit on every
Search/Clear change. Disposable profile `20260727094810-fa07643d` verifies
immediate highlighted state, cleared state and normalized wrapping, then passes
Full Data Verification 370/370 with exact state equality.

Owner accepted the complete ordered workflow, Website scope correction,
natural wrapping, highlighted search/jump and immediate Search/Clear refresh.
v50.1.0 is canonical and runtime accepted. Parent v50 remains open at v50.2.

## Prior v50.0.0 Central User Help

## Central User Help

The v50.0.0 candidate introduces one canonical, reusable Help window. Help >
Documentation and F1 open the same non-modal surface; F1 selects the topic for
the active top-level tab. Topic search is local and offline, and the help
catalog is compiled into the application so the installer/update governed-file
allowlist does not expand.

The catalog covers the end-to-end owner workflow and top-level tab families.
It preserves canonical data and safety boundaries, including immutable saved
purchase/inventory/quote values, ECB reference-only prefill for new unsaved
purchases, explicit public selection, Production/FTPS default-No, Verification
and guarded recovery.

The existing smoke scenario now opens Help, verifies a selected topic and
searches for recovery. Full Data Verification checks unique non-empty topic
IDs, representative contextual mappings and the canonical MainWindow entry.
Disposable smoke passes with exact baseline/final business-state recovery and
Full Data Verification 370/370. Debug and Release build with zero warnings,
the documentation audit passes and the read-only NuGet vulnerability scan
reports no vulnerable packages. Visual readability and workflow wording
require owner runtime acceptance. Schema remains v37.

Owner runtime accepted menu and F1 navigation, search, topic content and
single-window reuse. A first visual review found horizontal clipping in the
contents list; the accepted correction stretches items, disables horizontal
scrolling and wraps category, title and summary text. The final visual retest
passes. v50.0.0 is canonical and runtime accepted.

## Prior v49.0.0 Application Release Packaging and Publish Readiness

The v49.0.0 runtime remains unchanged and schema remains v37. Release packaging
now reads the governed compatibility range from `BuildInfo`: the public
schema-v29 deployment baseline through current schema v37. `LocalDatabase`, the
signed-package builder and the independent production verifier use this shared
contract, preventing the old hard-coded v29-v29 package range from blocking
current owner profiles.

Full Data Verification now contains a deterministic application-release
compatibility gate that binds the governed schema-v29 public baseline to the
runtime's current schema v37. The existing smoke scenario owns this safe,
offline contract; no Production or FTPS automation is added.

Production and FTPS remain blocked while a new Candidate is built and verified.
The required order is application installer/portable publish first, followed by
the signed update package and `/updates/latest.json` activation last. Candidate
runtime acceptance must prove fresh install, guarded update, SQLite preservation,
recovery and Full Data Verification before clean-tree Production promotion.

The signed v49.0.0 Candidate contains exactly six governed files and declares
continuous schema support v29-v37. The independent production verifier accepts
the package at both endpoints. Candidate release gates pass NuGet vulnerability,
documentation, BOM-less feed, exact bytes/SHA-256, ECDSA signature, inventory,
schema and stable-route-last contracts.

Exact portable Candidate smoke profile `20260726224510-b4ab7d08` passes Full
Data Verification 369/369 and exact baseline/final business-state equality.
Updater profile `20260726224940-fcb3321a` commits six exact staged files with
v49.0.0 health acknowledgement, then rolls back an injected invalid executable
and restores the exact governed file hashes. Earlier updater attempts exposed
and corrected a stale hard-coded v44 tester identity; owner paths, Production
and FTPS remained blocked.

Owner runtime acceptance completed on 2026-07-26. The Candidate installer ran
normally on the owner workstation and on a clean VM; the installed application
opened normally, retained the expected data boundary and passed Verification
369/369. The exact accepted Candidate bytes are approved for clean-tree,
byte-preserving Production promotion. Live FTPS publish remains a separate
explicit action.

The owner completed the guarded live publish on 2026-07-26: installer and
portable stable routes were activated before the signed update ZIP and
`/updates/latest.json`. Read-only post-publish verification reports HTTP 200,
Production v49.0.0, schema v29-v37 and promotion commit `5073da4`. Fresh remote
downloads match the accepted Production SHA-256 values exactly:

- Installer:
  `C641C064310510DED32209D591E24FAAF2668AF3862CB8BAE924B5723148B829`
- Portable:
  `9FFB9BBEFAB31D56EB3FA80EBB6F0606036C72576F65988569547029C232200E`
- Signed update:
  `9C74369C3A20B8F4165AF98E4265D49B5DFA527A63211998624CDAF2C6802CE0`

The v49.0.0 application release and update feed are Production published and
independently byte-verified.

## Optional Official Exchange-rate Reference Catalog

The candidate uses the documented ECB SDMX HTTPS API. It retrieves the latest
EUR reference observations for ISK and the governed Purchasing currencies,
then derives `ISK per 1 currency` as `ISK per EUR / currency per EUR`.
Provenance labels the value as ECB-derived rather than a direct Central Bank
of Iceland observation.

Schema v37 adds source, observation date and fetch UTC to each Purchase Order.
Only an order created in the current application session is eligible for ECB
prefill when its currency is selected. Orders loaded from SQLite retain their
stored rate and provenance. The legacy Settings reload/reset live-sync that
rewrote persisted Purchase Orders has been removed.

ECB refresh never edits governed Settings, Materials price fields, Inventory
lots, Printers or saved quote snapshots. The optional cache is per profile and
re-downloadable; it is not a canonical recovery table. Invalid, unavailable or
offline responses fall back to the existing manual governed Settings workflow.
Automation performs no live network call.

The provider uses a fixed HTTPS host, no redirects, a 12-second timeout, a 1 MB
response limit, strict CSV/date/positive-rate validation and strict cache
identity validation. System Diagnostics exposes endpoint, cache path, fetch
time, observation date and the new-order-only ownership boundary.

Debug/Release app and AutomationRunner builds pass with zero warnings/errors.
Documentation and 136-column roadmap gates pass. The read-only NuGet
vulnerability scan reports no vulnerable direct or transitive packages.

The first disposable smoke profile `20260726203553-6fe655e4` exposed 13
recovery-gate failures because current-schema compatibility comparisons still
used schema v36. No ECB, purchase or business-state gate failed. Updating those
bounded comparisons to schema v37 removed the cascade.

Disposable smoke profile `20260726203714-dcbdce20` passes Full Data
Verification 364/364, offline ECB status, all tab navigation and exact
logical/business-state preservation. Recovery profile
`20260726203749-a8eac754` passes 24-table schema-v36 migration, mutation,
transactional restore, restart and exact final business-state recovery.
Production and FTPS remained blocked. Owner runtime evidence
`3DPIceland_FilamentDB_Verification_20260726_210009.txt` passes 364/364.
Diagnostics `3DPIceland_FilamentDB_System_Diagnostics_20260726_210017.txt`
confirm schema v37, live ECB fetch UTC `2026-07-26T20:59:36.1893007Z`, latest
observation `2026-07-24` and new-order-only ownership. Owner accepted live
retrieval, prefill, offline behavior and historical-data immutability.

Canonical tester seed `C:\Seed-Database\filamentdb.sqlite` is refreshed to
schema v37 from the accepted disposable derivative. SHA-256 is
`43503623D8D19A1F38B1505F456BDAA34E9B33AFF4609AD2F79CC2321B7150AE`.
The prior schema-v36 seed is retained as
`filamentdb-schema36-migration.sqlite`, SHA-256
`6E276E26FB218BC588CC70DA57E10FE2E812A1CA5420494315BE2B22C205EAB0`.
Both pass SQLite integrity and foreign-key checks. The v37 seed has 201
Materials and zero Printer, Quote, Usage or automation residue. Final smoke
profile `20260726210132-f34523ba` passes Full Data Verification 364/364 and
exact logical/business-state preservation. v48.2.0 is complete, canonical and
runtime accepted.

## Print Job Quote Workflow

Candidate schema v36 adds append-only `PrintJobQuotes` as immutable calculation
snapshots. A new Print Job Quotes workspace selects an active canonical
Printer and either exact MaterialID `LandedCostUsdPerKg` or explicit manual
cost/kg with governed source currency. Manual evidence never receives a
MaterialID automatically.

The v1 calculation applies grams per part × quantity exactly once, then the
governed material-efficiency factor. It snapshots material and quote FX rates,
all Printer inputs and derived hourly rate, Pricing Settings, time/labor,
additional cost, margin, component totals and final ISK/quote-currency values.
Saved quotes do not recalculate from later catalog/Settings changes. The owner
may explicitly delete obsolete or test quotes from Saved Quote History.

Customer PDF export reads the saved snapshot and never current catalog/Settings
values. It uses an A4 quote layout with the Labs logo, customer, description,
customer-safe Material name, quantity, unit price, total, final price and
estimate terms. MaterialID, Printer identity and internal JSON evidence are not
shown to the customer.
Governed Excel recovery expands to 24 tables. Exact schema-v35 packages migrate
with an empty PrintJobQuotes table; earlier supported packages retain their
already-governed empty-table migrations.

Disposable CRUD automation creates a quote tied to its disposable Material and
Printer, verifies persistence and duplicate-ID rejection across restart, then
uses an explicitly restricted automation-only cleanup adapter. Normal snapshot
updates/recalculation remain absent; owner-confirmed deletion is available.
The quote form owns separate minutes for print/post-processing labor, customer
consulting and parts design/changes, all using the governed labor hourly rate.
Locale-safe parsing treats `1,3` as 1.3, not 13, for Printer buffer and quote
inputs.

Debug and Release app/tester builds pass with zero warnings/errors. Disposable
CRUD profile `20260726200110-78b75cc7` passes Full Data Verification 363/363,
proves one immutable quote after create and restart/edit, zero after authorized
cleanup, and exact baseline/final business-state equality. Recovery profile
`20260726191402-0994d878` passes 24-table export, mutation, transactional
restore, restart and exact business-state recovery. Owner runtime acceptance
and owner runtime PDF review are complete. A representative A4 PDF was
rendered from the customer template and visually inspected: header, customer
metadata, customer-safe Material context, quote table, final total and terms
fit one page without clipping or internal JSON exposure.
The final customer revision embeds the canonical Labs logo, removes MaterialID,
Printer identity and external methodology credit, and retains those internal
identities only in the app snapshot.
Owner accepted the complete workflow, deletion behavior and final customer PDF
layout. Owner evidence
`3DPIceland_FilamentDB_Verification_20260726_195537.txt` passes 363/363;
v48.1.2 is complete, canonical and runtime accepted.

Canonical tester seed `C:\Seed-Database\filamentdb.sqlite` is refreshed from
the accepted disposable schema-v36 database. SHA-256 is
`6E276E26FB218BC588CC70DA57E10FE2E812A1CA5420494315BE2B22C205EAB0`.
The prior schema-v35 seed is retained as
`filamentdb-schema35-migration.sqlite`, SHA-256
`2F231B51A9728384979363CAD38AC101D9312AC00D71FC70019185BC99F84A78`.
Both pass SQLite integrity and foreign-key checks; the v36 seed has zero
Printer, Quote, Usage and automation residue. Disposable CRUD profile
`20260726201020-61ee8843` passes Full Data Verification 363/363, quote
create/restart/delete cleanup and exact final business-state recovery from the
refreshed canonical seed.

## Printer and Pricing Settings Foundation

Candidate schema v35 adds canonical `PrinterProfiles` and a separate Printers
workspace with add, duplicate, archive/restore, delete and editable inputs.
Stable PrinterID is read-only. Uptime is explicitly 0–100 percent. Printer
currency is selected from a dropdown populated only by valid governed
`ISK per 1 ...` Purchasing currency rows in Settings.

Seven global Print Job Pricing defaults are SQLite-canonical Settings rows.
The deterministic rate converts capital to ISK, adds electricity and applies a
printer override or global buffer. It does not yet create quotes.

Excel recovery owns 23 exact tables. Exact v34 packages migrate with an empty
PrinterProfiles table; pre-v34 packages may also synthesize the supported empty
UsageEvents table. Ambiguous sets remain blocked. Disposable CRUD automation
creates, edits across restart and removes a PrinterProfile with exact cleanup.

Debug and Release app/tester builds pass with zero warnings/errors. Disposable
profile `20260726184326-9610f189` passes Full Data Verification 362/362 and
PrinterProfile CRUD across restarts with exact baseline/final business-state
equality. Recovery profile `20260726184448-e9176bec` passes 23-table export,
mutation, transactional restore, restart and exact final business-state
recovery.
Owner accepted Printer CRUD, restart persistence, governed currency dropdown
and hourly-rate refresh. Owner evidence
`3DPIceland_FilamentDB_Verification_20260726_185305.txt` passes 362/362;
v48.1.1 is canonical and runtime accepted.

Canonical tester seed `C:\Seed-Database\filamentdb.sqlite` was refreshed from
the accepted disposable schema-v35 database. SHA-256 is
`2F231B51A9728384979363CAD38AC101D9312AC00D71FC70019185BC99F84A78`.
The prior schema-v33 seed is retained as
`filamentdb-schema33-migration.sqlite`, SHA-256
`50782D4E2DBE8F773E0A915E9E2460525B43FB68611E19DD6EB12F47B131AB31`.
Both pass SQLite integrity; the v35 seed has zero automation Printer residue.

## Internal Usage Analytics

The candidate adds private analytics to the selected MaterialID in the Usage
workspace. Summary cards show immutable ledger rows separately from effective
events, then net filament, print/hands-on time, produced/accepted/rejected
counts and evidence coverage.

The pure projection now excludes reversal rows and reversed originals from
effective-event and coverage counts. Quantitative totals still net the complete
append-only ledger, so a 100 g original corrected to 80 g shows three audit
rows, one effective event and 80 g. Missing active evidence remains
`Not recorded`.

No schema, public report, website, price or cost analytics change is included.
The disposable CRUD scenario now reads the visible analytics through stable
AutomationIds before and after correction.

Candidate profile `20260726175433-70e8dba1` passes Full Data Verification
361/361. Visible analytics prove 1 effective/1 ledger row, 100 g and 1 hour
before correction; after correction they prove 1 effective/3 ledger rows,
80 g and 55 minutes. Cleanup restores exact baseline business state.

Owner accepted the summary layout, effective/ledger semantics and MaterialID
refresh. HTML/PDF/website previews remain free of Usage fields. Owner evidence
`3DPIceland_FilamentDB_Verification_20260726_181431.txt` passes 361/361;
v48.0.7 is canonical.

## Bounded Usage UI and Automation

The candidate adds one private Usage workspace on top of accepted schema-v34
persistence. Users select an exact MaterialID, optionally select only an
Inventory spool linked to that MaterialID, and record observed grams, explicit
provenance, duration, counts, source and note. UI minutes convert to canonical
whole seconds.

The ledger is read-only. Correct Selected prepares a replacement draft; saving
appends an exact reversal plus replacement and atomically reconciles linked
Inventory. No Print Job/Test Session selector, normal delete/edit action,
public report field or website output is added.

The existing disposable CRUD scenario remains the bounded write authority and
now verifies the visible Usage controls and ledger state after create and
correction restarts.

Candidate profile `20260726173640-4d663edc` passes Full Data Verification
360/360. It proves one visible event at 900 g, three visible
original/reversal/replacement rows at 920 g, selection preservation across tab
refresh, exact cleanup and baseline/final business-state equality.

Owner added canonical Inventory spools for the linked-weight test and accepted
the full workflow. An empty Inventory correctly leaves the optional spool
selector empty while no-spool usage remains valid. Owner evidence
`3DPIceland_FilamentDB_Verification_20260726_174800.txt` passes 360/360;
v48.0.6 is canonical.

## Canonical Usage Persistence and Recovery

Schema v34 persists private immutable Usage Events with exact Material,
Inventory and Experimental Run relationships. Event insertion and linked
Inventory weight adjustment share one SQLite transaction. Corrections append
an equal/opposite reversal plus replacement; accepted rows are never edited.

Governed Excel recovery now owns 22 tables. Exact schema-v33 packages remain
supported and restore with an empty UsageEvents table; incomplete or ambiguous
table sets remain blocked. The existing disposable CRUD scenario proves
original persistence, restart, correction, Inventory reconciliation and exact
cleanup. Normal Usage UI and public report/website exposure remain out of
scope pending later increments.

Candidate automation profiles `20260726171743-3d1e2a09` (CRUD) and
`20260726172034-4e048744` (recovery) pass Full Data Verification 359/359.
CRUD proves 1/0/0 events at 900 g, then 3/1/1 at 920 g and exact cleanup.
Recovery proves the 22-table workbook, pre/post backups and equal baseline/final
business-state hash. Owner runtime accepted normal behavior and Full Data
Verification 359/359 from
`3DPIceland_FilamentDB_Verification_20260726_172529.txt`; v48.0.5 is canonical.

## Disposable Usage Domain Prototype

Pure immutable records and `UsageEventDomainService` implement the accepted
contract without persistence or UI. The service validates canonical identity,
UTC evidence, exact inventory/material compatibility, quantity provenance and
entry-kind rules. It builds exact reversal/replacement corrections, nullable
MaterialID projections and equal/opposite inventory delta plans.

The prototype does not read or write SQLite, Inventory collections, owner data,
reports, website output, preferences or the tester seed. Verification uses
fully synthetic records. AutomationRunner receives no new action because no
workflow or write authorization exists; its smoke scenario will execute the
new Full Data Verification contracts.

Owner runtime accepted normal startup, unchanged Materials/Inventory and all
four synthetic usage-domain gates. Full Data Verification passes 358/358;
v48.0.4 is canonical.

## Usage Event Contract Audit

The ownership audit finds no canonical general Print Job or Test Session
entity. Native sample counts remain derived measurement evidence; Experimental
Runs own only Experimental Testing; Inventory owns current spool state rather
than immutable consumption history.

The recommended future owner is one append-only Usage Event ledger. Events use
required MaterialID, exact optional inventory/experimental relationships,
nullable observed quantities and explicit reversal/replacement corrections.
Material totals remain projections. Job pricing stays separately owned by
v48.1, and no usage data enters public outputs without a later allowlist
decision.

No runtime, schema, UI, seed or calculation change is included. Therefore no
build, tester or Verification count change is claimed. The owner approved event
vocabulary, reversal/replacement, atomic inventory updates, measured/slicer
grams provenance, seconds-based duration and private-by-default scope.
v48.0.3 is complete; v48.0.4 owns the pure disposable domain prototype.

## Governed Value Index

One service calculates Overall engineering score points per canonical MSRP
USD/kg. It stores nothing and never substitutes landed cost or inferred price.
Recommendation Detail exposes both inputs, the current filtered family/dataset
scope, a comparative-not-physical-property disclosure and precise missing-data
reason.

Existing hidden-gem ordering and Manufacturer intelligence now reuse the same
formula. No public report allowlist or schema changes. Verification owns valid
calculation, missing score/price, disclosure, scope and UI AutomationId.
Selecting a Material now moves the Recommendation Base Material filter to that
exact canonical family before rebuilding recommendation, alternative,
hidden-gem, MSRP and value-index rows. Unsupported legacy names are retained
without fuzzy scope remapping. The smoke tester does not select mutable
recommendation state, so a new tester step would be low-value; deterministic
Verification owns the calculation and selection-scope contract.

Recommendation MSRP belongs to the selected recommendation result, which can
be a different product from the Materials row used to choose the family scope.
The price line now names that recommendation product and MaterialID explicitly.

Owner runtime accepted the value index, exact PLA/ASA recommendation scope,
alternative refresh and recommendation MSRP identity. Full Data Verification
passes 354/354; v48.0.2 is canonical.

## Canonical Pricing Provenance

One service now owns canonical MSRP selection and material-price USD conversion.
Canonical Materials MSRP wins even when blank; landed cost and legacy
projections cannot substitute for it. Missing/invalid configured rates and
unsupported currencies produce `Not recorded` calculated fields instead of a
silent 1:1 conversion.

Purchase-order, inventory and material source fields remain unchanged. No
schema, import/export, recovery or public allowlist expansion is included.
Verification receives deterministic valid-rate, missing-rate,
unsupported-currency and no-landed-fallback probes. No tester scenario change
is warranted because no new UI action or safely isolated write workflow exists.

Owner runtime accepted Materials pricing, separated MSRP/landed values,
Advisor context, Manufacturer/website behavior and Full Data Verification
351/351. v48.0.1 is canonical.

## Stable Coverage Identity

Coverage storage is extended backwards-compatibly with optional `CollectionId`
and `MaterialKey` properties. Existing title/label snapshots are retained and
remain readable through exact fallback.

An explicit binding action previews unique exact candidates and defaults to No.
It never performs fuzzy matching and never changes unmatched or ambiguous
legacy entries. Newly applied statuses are stable immediately.

Collection Dashboard and Video Pipeline status lookup prefer stable identity.
Clear Status and collection deletion recognize stable and exact legacy
ownership. Deletion explicitly confirms related coverage removal.

The tester only reads disposable identity state and does not invoke migration.
Verification deterministically proves stable-first lookup, exact binding and
unmatched preservation. No SQLite schema or external-AI boundary changes.

Debug and Release builds pass with zero warnings/errors. Documentation and
NuGet vulnerability gates pass. Disposable smoke profile
`20260726141853-e0ce0c53` passes Full Data Verification 350/350 and exact
pre/post database byte and business-state equality. Owner runtime subsequently
accepted identity visibility, zero-legacy behavior, remaining coverage
workflows and Verification PASS. v47.0.3 is canonical.

## AI Collection Workflow Clarity

Collection saving now has an inspectable read-only preview and explicit action
state. A unique title is a create action; an existing exact title is an update
action with its saved MaterialID count.

Preview writes nothing and lists visible-row count, unique MaterialID count,
existing saved membership and bounded exact MaterialIDs. The final default-No
confirmation repeats the MaterialID preview. Cancelling writes nothing.

An accepted update preserves collection identity and replaces only its saved
MaterialID/label snapshot. Existing pipeline status metadata is deliberately
retained for backwards compatibility; stable identity migration remains owned
by the later v47.0.3 increment.

Standard automation exercises preview only and cannot write personal AppData
collections. No schema or JSON format changes in this increment.

An initial disposable run exposed a Verification-only control-name mismatch;
the buttons had stable AutomationIds but no WPF names for in-process lookup.
After adding those names, the next pass exposed that legacy AppData ownership
allowed automation to read an owner collection title. No write occurred.
Automation now binds AI storage to its disposable PreferencesFolder while
normal owner AppData remains unchanged.

Owner runtime review found that cancelling an update rebuilt the output from
the proposed filter and therefore looked like a successful membership update.
The cancel path now reports the old persisted MaterialIDs as unchanged and the
current-filter count as a discarded proposal.

Final profile `20260726135702-ceb78987` passes preview automation, cancel-state
honesty, explicit empty disposable AI-storage evidence, Full Data Verification
349/349 and exact logical/business-state equality without personal AI storage access.
Debug/Release and the NuGet vulnerability scan pass. Owner runtime acceptance
passes for create/update, preview and corrected cancel behavior. Owner Full
Data Verification passes 349/349; v47.0.2 is canonical.

## AI Assistant Scope Clarity

The AI Assistant remains deterministic, local and backwards compatible. Its
header now states that no external AI service is called and explains that the
workspace creates planning briefs without changing canonical application data.

Opening the tab refreshes an exact visible-scope summary sourced from canonical
active MaterialIDs. A bounded MaterialID preview makes Materials-filter scope
inspectable before generation. The planning-note field is explicitly described
as reference text rather than a free-form external AI prompt.

Stable AutomationIds cover the tab, scope summary, MaterialID preview, refresh,
full-brief generation and output. Every disposable scenario verifies this
read-only contract without enabling network access or destructive actions.

No schema, session JSON, collection JSON or coverage JSON format changes in
this increment.

Isolated Debug and Release solution builds pass with zero warnings and errors.
The read-only NuGet vulnerability scan reports no vulnerable packages.
Disposable smoke profile `20260726122624-99485959` confirms 201 visible rows,
201 unique MaterialIDs, Full Data Verification 348/348 and exact logical and
normalized business-state equality. Owner runtime accepted layout, scope,
local brief behavior and compatibility; owner Verification passes 348/348.

## Application Branding

The supplied 1254 x 1254 transparent application-icon PNG is now the canonical
splash source. A governed multi-size ICO derived from that exact source owns
the executable, Windows shell, installer, shortcut and signed-package role.

The supplied 801 x 482 transparent 3DPIceland Labs wordmark is a separate
embedded resource. The main application header displays it on a 180 x 96 white
rounded card, while the splash keeps the application icon and existing
extrusion animation. The accepted public-report JPG is unchanged.

The supplied icon already contains a completed blue filament path. Splash masks
only that interior path and draws a matching WPF vector from the nozzle over
2.0 seconds. This avoids the former `1,220` dash contract, whose values were
scaled by stroke thickness and exposed only a short heavy segment near exit.
The canonical PNG, ICO and non-splash callers remain unchanged.

Runtime review showed that MainWindow construction blocked the UI dispatcher,
so the animation clock advanced while no frames could render. The trace now
starts only after construction and is awaited before splash exit. Its geometry
uses the measured original blue bounds (approximately x 76-137, y 108-159 in
the 210-pixel splash card) and a matching 5-pixel stroke.

Verification requires both WPF resources to load. No tester scenario was added:
small-icon rendering, alpha edges, wordmark readability, clipping and card
balance require manual Windows/runtime inspection.

Debug and Release isolated builds pass with zero warnings and errors. The
read-only NuGet vulnerability scan reports no vulnerable direct or transitive
packages. Disposable smoke profile `20260726115305-e5fa34a1` passes Full Data
Verification 347/347. Logical database hash
`F0EDCC3295A114C935668D2B92D7A1AEB1C67C4D1630EFC89F11B7FCDC556E5F`
and normalized business-state hash
`4FBCF6A2656678875A6692C0A7AA30CD0CDC3F4AAB83003B3BB2C77081B1C87D`
are identical before and after. Owner runtime accepted transparent Windows and
titlebar presentation, the readable unclipped header wordmark and the final
smooth complete splash trace. Minor vector/icon geometry variation is accepted
as an intentional splash presentation difference. v46.0.0 is canonical.

## Canonical Base Material Identity

Schema v33 adds nullable `BaseMaterialId` to canonical Materials and a stable
identity to Base Material Catalog rows. Migration assigns catalog identities
but deliberately leaves every existing Material unlinked. Legacy/unmapped text
remains supported and is included in the Materials dropdown.

New and duplicated Materials retain an explicit catalog identity. Existing
rows can be linked by selecting the dropdown value, including committing the
same visible name, or through the default-No `Bind Exact Base Material Names`
action. Exact binding is case-insensitive and unique-only; it never performs
fuzzy or silent remapping.

Linked rows resolve the current catalog name by ID. Catalog rename updates the
Material compatibility snapshot, so filters, reports, website/public outputs,
measurement projections and printing-profile callers receive the corrected
name. Referenced catalog deletion is blocked in UI and SQLite. Recovery exports
retain the governed schema/table identity columns.

The disposable CRUD contract now links its generated Material to its generated
Base Material, verifies restart persistence, blocks referenced deletion,
proves rename propagation and restores complete baseline business state.
Production, FTPS and owner-database paths remain blocked.

Disposable profile `20260726101917-5bc56749` passes the complete relationship
lifecycle, Full Data Verification 347/347 and exact final business-state
cleanup. Owner runtime acceptance remains pending.

Owner runtime confirmed the relationship workflow but Verification exposed a
prior-stage false FAIL when Base Materials had not yet been visited. The v45.2
workspace gate now validates stable controls and handlers independently of lazy
view activation. Materials also shows an explicit unlinked Base Material count
beside the filter; the dropdown remains available for viewing those exact rows.

Renaming a newly added Base Material now refreshes the Materials dropdown in
the same successful edit transaction. The accepted old choice is removed, the
new canonical name is available immediately and the Fast Materials viewport is
synchronized without requiring restart or tab cycling. Disposable CRUD uses
the same edit handler and verifies both sides of this choice refresh.

Owner final runtime acceptance confirms exact binding, zero unlinked Materials,
live dropdown refresh and Full Data Verification 347/347 in
`3DPIceland_FilamentDB_Verification_20260726_105027.txt`. v45.2.1 is closed,
canonical and runtime accepted.

The canonical tester seed is now normalized schema v33 at
`C:\Seed-Database\filamentdb.sqlite`, SHA-256
`50782D4E2DBE8F773E0A915E9E2460525B43FB68611E19DD6EB12F47B131AB31`.
The prior schema-v32 seed is preserved as
`C:\Seed-Database\filamentdb-schema32-migration.sqlite`, SHA-256
`65BD03F668768F0AAEBF937BAFC628559A168EA1A07E586CECADC7431AF7BB84`.

## Base Materials Workspace

The SQLite-canonical Base Material Catalog now has a dedicated top-level
workspace instead of sharing Settings Manager. It retains the accepted
23-column Fast editor and adds explicit Add, Duplicate, guarded Delete and
independent column-reset controls. Settings Manager now owns only actual
measurement, calculation, deployment and exchange-rate settings.

Text-key compatibility remains unchanged in this UI increment. Rename
propagates exact matching Material text so existing reports, website,
filters and profile lookup callers continue to receive the current name.
Delete is blocked while a Material references the catalog name.

Disposable CRUD now navigates `BaseMaterialsTab` and creates, restarts, edits,
duplicates, restarts, deletes and verifies absence of disposable Base Material
records alongside the existing Material/Manufacturer lifecycle. Schema and
`BaseMaterialId` are deliberately deferred until this workspace passes owner
runtime acceptance.

Debug and Release pass with zero warnings/errors. Disposable profile
`20260726094502-312196e2` passes Base Materials tab navigation, catalog
create/edit/duplicate/delete persistence, Full Data Verification 346/346 and
exact baseline/final business-state recovery. Owner UI/runtime acceptance
remains pending.

Owner testing passed every CRUD workflow and Full Data Verification 346/346,
then found that immediate navigation after restart could show an empty Base
Materials host until another tab transition. The selection handler only
recognized `Settings Manager` and deferred view creation. It now recognizes
both tabs and completes lightweight Fast-view creation synchronously before the
selected workspace is presented.

Startup-fix disposable profile `20260726095812-c88314b0` re-passes immediate
BaseMaterialsTab navigation, catalog CRUD, Full Data Verification 346/346 and
exact baseline/final business-state recovery.

Owner retest `3DPIceland_FilamentDB_Verification_20260726_100133.txt` passes
346/346 and accepts immediate first-open rendering after restart. v45.2.0 is
closed; v45.2.1 owns canonical Base Material identity.

v45.1 now implements a nullable canonical `ManufacturerId` relationship in
schema v32. Fast Materials stores the selected catalog identity while retaining
the exact historical Manufacturer text as a compatibility snapshot and fallback.
Existing rows remain unlinked after migration; no value is silently matched,
cleared, corrected or remapped.

Manufacturers provides a `Bind Exact Material Names` action with a preview and
default-No confirmation. It links only unique case-insensitive exact catalog
names; ambiguous and unmatched values remain unchanged.

A linked Material resolves the current catalog name at runtime. Manufacturer
rename updates linked Material snapshots and therefore every existing filter,
report, website and public-output projection. Archive is non-destructive and
hard delete is blocked while an ID is referenced. Legacy/unmapped rows continue
to use their exact text. Governed Excel recovery carries the nullable column
through its typed table snapshot without changing public allowlists.

Disposable CRUD proves an unmapped Manufacturer survives create/restart, then
creates and selects a disposable catalog identity, proves ID persistence,
canonical rename propagation and referenced-delete blocking, and removes both
generated records. Production, FTPS and owner paths remain
blocked. Debug and Release solution builds pass with zero warnings/errors;
documentation, roadmap-line and NuGet vulnerability gates pass. Disposable CRUD
passes against the explicit approved seed with Full Data Verification 345/345,
equal baseline/final business-state hashes and an unchanged source-seed hash.
Owner runtime acceptance is complete.

Owner review then exposed one shared stale-projection defect after deleting a
test Material: its unique Manufacturer remained in the dropdown and the
in-memory Inventory collection retained a row already removed by SQLite
cascade, causing Inventory engine to fail at 344/345. The candidate now
refreshes Manufacturer choices on Material collection changes and reloads
Inventory from canonical SQLite after a successful Material delete save.
Disposable CRUD re-passed 345/345 with zero inventory orphans; owner retest is
still required.

Owner review also found that a new catalog row could not be named when its
`New Manufacturer` placeholder matched a Material value: the in-use rename
guard rewrote the active editor after each character. Name now remains in the
cell edit buffer until focus leaves; the guard evaluates the complete proposed
name once. New and recognizable placeholder rows are draft-owned for their
initial edit, mirror Name into Display Name and become normally guarded after
restart. Placeholder generation avoids both catalog and Material names.

Owner exact binding linked 182 Materials and deliberately left 20 unmatched.
The next Verification run exposed a scope-only defect: one archived MaterialID
was present in the raw All-filter ID set but absent from canonical active report
rows, cascading four v44.7.7 failures. The ownership check now compares active
scope on both sides; no Material or report behavior changed.

The owner retest passed Full Data Verification 345/345. Materials now adds a
counted `Unlinked manufacturers (n)` filter so each remaining legacy value can
be reviewed and explicitly assigned through the canonical dropdown. The count
refreshes after each assignment; no fuzzy or silent mapping is introduced.
Disposable CRUD profile `20260725225702-22872c00` passes the extended
Verification contract 345/345 with equal baseline/final business-state hashes
and an unchanged schema-v31 migration seed.

Owner review exposed a same-text identity edge case: choosing the canonical
Manufacturer whose name already matched the legacy text created no cell-text
delta, so the Fast grid skipped the binding call. Manufacturer dropdown
confirmation now commits the identity explicitly even when display text is
unchanged; other columns retain normal change-only behavior.
Disposable CRUD profile `20260725230522-1ea4b751` passes Full Data Verification
345/345 after this correction and returns the business-state hash to baseline.

Owner exact binding then reached zero unlinked Materials and Verification passed
345/345 in `3DPIceland_FilamentDB_Verification_20260725_231008.txt`. The
unlinked filter and exact-binding button are collapsed at zero. They remain a
supported recovery surface and reappear only if migration, import or restore
introduces an unlinked legacy value.
Disposable recovery-visibility profile `20260725231345-2dfcad7a` passes
345/345 on the schema-v31 seed and returns the business-state hash to baseline.
Owner evidence `3DPIceland_FilamentDB_Verification_20260725_231731.txt`
subsequently accepted the zero-count UI and passed 345/345. v45.1 is closed.

Post-release seed maintenance preserved the schema-v31 migration fixture and
promoted a normalized schema-v32 acceptance seed. The first two manual-backup
copies exposed canonical `SortOrder` normalization on disposable startup; no
owner data was changed. The normalized candidate contains 203 Materials, zero
unlinked/invalid Manufacturer identities and no automation residue. Disposable
profile `20260725233352-929a4f1a` passes 345/345 and exact business-state
rollback.

# Previous Build Notes - v44.7.18

## Guarded Updater Acceptance

v44.7.18 completes Stage 5. It adds explicit disposable updater scenario
authorization while keeping general updates, Production, FTPS and owner paths
blocked. The real updater helper forwards the exact validated disposable
profile to both the updated application and the rollback relaunch.

The runner copies a portable build below its temporary profile, verifies
snapshot and `Committed` state with an exact-build health acknowledgement,
then stages an invalid disposable executable and requires `RolledBack`.
Retained evidence includes request/state/health files, rollback snapshots and
SHA-256 for all 54 governed files.

Disposable runtime and Full Data Verification pass 344/344. All pre-update
portable hashes and the database business-state hash return to baseline.
Debug/Release, static/security, release-documentation and read-only NuGet
advisory gates pass. Owner runtime and Full Data Verification 344/344 passed.
v44.7.18 is closed.

## Retained v44.7.17 evidence

## Disposable Backup and Recovery Acceptance

v44.7.17 adds explicit `recovery` scenario authorization. It creates and
verifies manual `.bak` plus legacy `.sqlite` evidence, exports and verifies the
canonical governed Excel package, applies a disposable mutation, restores the
package and restarts under the same manifest.

Excel restore now retains verified pre/post SQLite backups. A post-evidence
failure triggers rollback from the pre-restore backup. General SQLite/Excel
restore UI, owner paths, Production, FTPS and updates remain blocked. SQLite
restore is not automated. Disposable and owner Full Data Verification pass
343/343 with equal baseline/final business-state hashes. Owner accepted backup
discovery, governed Excel recovery and pre/post evidence. v44.7.17 is closed.

## Retained v44.7.16 evidence

## Disposable CRUD Acceptance

v44.7.16 adds an explicit `crud` scenario over the accepted disposable runtime
foundation. The manifest authorizes one exact generated disposable MaterialID.
The app exposes a narrow profile-only Automation contract because Fast-grid
cells do not yet provide stable row/cell automation peers.

The runner creates and saves a valid record, restarts, proves persistence,
edits and saves, restarts, proves the edit, deletes only the authorized record,
then restarts and proves absence. Per-action consistent SQLite snapshots retain
full logical and business-state hashes. The final business-state gate excludes
only `UpdatedAtUtc`; full hashes preserve evidence of canonical autosave
timestamp movement.

Disposable Stage 3 runtime and Full Data Verification pass 342/342 with equal
before/after business-state hashes. Production, FTPS, updates, restore, general
delete and owner database paths remain blocked. Debug/Release and
static/security/documentation gates pass. Owner runtime and Full Data
Verification acceptance pass 342/342. v44.7.16 is closed.

## Retained v44.7.15 evidence

## Automated Report Acceptance

v44.7.15 is the runtime-accepted Stage 2 increment over the accepted v44.7.14 disposable
runtime foundation. A new explicit `reports` scenario invokes the existing
canonical `Build Public Report Package` workflow without changing its six
report builders, report models, formulas, routes or publication approvals.

Automation report writes require scenario authorization and are confined to
the disposable profile output folder. Production, FTPS, updates, restore and
delete actions remain blocked. The runner waits for the stable report
completion contract, validates catalog-owned safe routes, HTML markers, PDF
headers and JSON, then records exact bytes and SHA-256 values.

Disposable Stage 2 runtime passes with Full Data Verification 341/341, 211
catalog entries, 639 catalog/root artifacts and identical before/after logical
SQLite hashes. PDF rendering found and corrected a Material Summary continuation
table clip by retaining fixed columns in deterministic 20-row presentation
tables. Representative Summary and Material Engineering PDFs now render without
clipping or overlap. Owner review then found right-edge clipping in the screen
HTML; Material Summary screen tables now use fixed 100% layout and wrapped
cells. Narrow-window review then showed over-compression, so the final screen
contract uses readable column widths, normal word wrapping and horizontal
scrolling below the table minimum width. The accepted landscape PDF contract
is retained. Owner accepted the landscape PDF and responsive HTML behavior;
owner Full Data Verification passes 341/341.

Debug and Release solution builds pass with zero warnings/errors. Diff,
documentation, 136-column roadmap, security/static and NuGet vulnerability
gates pass; all three solution projects report no known vulnerable packages.
Repository-wide `dotnet format --verify-no-changes` remains unsuitable as an
increment gate because the accepted baseline contains thousands of pre-existing
whitespace findings across unchanged files; no bulk formatting was performed.
All v44.7.15 release gates pass.

## Retained v44.7.14 evidence

## Automated Runtime Acceptance Foundation

v44.7.14 is the runtime-accepted Stage 1 foundation. A new framework-only
Windows UI Automation runner launches an exact SHA-256-bound application build
only after it creates an isolated profile below the dedicated temporary
automation root and copies an explicitly supplied non-live SQLite seed.

The app validates every profile path, visibly labels the disposable session,
separates database/preferences/output/evidence folders and skips owner-profile
legacy migration. Production, FTPS, update, restore and Material-delete entry
points are blocked while automation mode is active.

Stable Automation IDs cover the main window, workspace tabs, Fast hosts,
Verification Center and evidence command. Verification exports TXT and JSON
without a file dialog. The runner retains owned-window screenshots and result
evidence, rejects unexpected dialogs and compares canonical logical database
hashes from consistent before/after SQLite snapshots. This avoids treating
normal WAL checkpoint/header normalization as a data mutation.

Fast-grid cell automation, report generation, CRUD, recovery and updater
scenarios remain outside Stage 1. The disposable runner passes with isolated
Full Data Verification 340/340 and matching logical hashes. Owner runtime
acceptance and Full Data Verification pass 340/340.

## Retained v44.7.13 evidence

## Public HTML Trust Hardening

v44.7.13 is runtime accepted. New website-template imports are
limited to 5 MiB, must contain one structurally replaceable `const DATA`
object and require an explicit default-No trust confirmation before SQLite
storage and immediate activation. Existing stored templates are unchanged.

The hidden WebView2 PDF host retains JavaScript and local report assets while
printing, but blocks unexpected top-level navigation, popups and permission
requests. Script execution is disabled again between batch documents.

Verification now exercises encoded malicious public text, rejection of
`javascript:` links, bounded website-template parsing and presence of the
WebView2 hardening policy. CSP and arbitrary-HTML sanitization remain outside
this increment pending compatibility proof.

Owner runtime testing accepted template-import cancellation/confirmation,
website Preview, the complete Public Report Package and sampled HTML/PDF
output. Full Data Verification passed 339/339. v44.7.13 is canonical.

## Retained v44.7.12 evidence

## Clean Baseline Retirement

v44.7.12 is runtime accepted. Repository-wide
ownership tracing covered C#, XAML, project resources, partial classes,
reflection-based Verification, serialization, migration, recovery, updater,
diagnostics, export and report/deployment paths.

The candidate removes the unused hand-built MainWindow PDF renderer and its
parallel `PdfLines` model projection. User-facing PDF remains exclusively
printed from canonical HTML with WebView2. The separate typed
`ReportPdfRendererService`, report certificates and documentation PDFs remain
active and unchanged.

Caller-free legacy workbook-import write helpers and their model are removed.
Supported database compatibility, migration inspection, governed Excel
disaster recovery and explicit SQLite backup/restore remain intact.

Retired website-template file-selection residue, standalone manufacturer
template rendering, old workflow handlers and dependent helpers are removed.
The SQLite-approved website template, canonical website renderer and current
public report families remain unchanged.

Asset review found four tracked image assets. The runtime WPF PNG, installer
ICO and report JPG all have active owners. The 1.28 MB icon-source PNG had only
a self-copy project item and is removed from the project and repository.

Debug/Release and updater Release compile probes pass with zero
warnings/errors. Static, security, package and release-documentation gates pass.

Initial owner testing passed Full Data Verification, normal Engineering Package
export, branding and the remaining runtime checklist. A fresh Public Report
Package failed while calculating its source fingerprint because that service
still queried retired workbook-era `TensileResults` and related tables. The
fingerprint now reads only the canonical native measurement tables. Legacy
tables were not recreated. The corrected package rebuilt every public report
family without visible errors, sampled HTML passed visual review and final Full
Data Verification passed. v44.7.12 is canonical.

## Retained v44.7.11 evidence

## Settings Manager Command Clarity

v44.7.11 is runtime accepted. `Load Settings` is now `Reload
Saved Settings` and explains that it reloads General and Deployment settings
from canonical SQLite, discards current unsaved Settings edits only after a
default-No confirmation, and leaves Base Material Catalog unchanged.

Research found an ownership defect behind `Restore Built-in Defaults`.
`LoadBuiltInNativeSettingsDefaults` seeds both General Settings and Base
Materials during initialization, but the restore command reused it despite
promising a General-only replacement. That temporarily replaced the in-memory
Base Material Catalog and a later Save Settings could persist the unintended
catalog defaults. The restore command now uses a General-only replacement,
preserves Deployment rows and Base Materials, saves the new General values and
refreshes their existing calculation/currency consumers.

`Reset Fast Columns` is renamed `Reset Columns`. It still resets only the two
machine-local Settings and Base Material layout preferences after default-No
confirmation; canonical values are unchanged.

Owner visual review exposed a generic Fast-grid footer command labelled
`Reload current Materials filters/data` in both Settings views. It only rebuilt
each view from its current in-memory collection and did not reload SQLite.
Both redundant Settings instances are now hidden; programmatic refresh and the
explicit toolbar `Reload Saved Settings` command remain intact.

Schema, formulas, SQLite ownership, Deployment credentials, reports,
website/FTPS, recovery and measurement behavior are otherwise unchanged.
Debug/Release and static/security gates passed. Owner reload, restore,
cancellation, restart, layout and visual tests passed; Full Data Verification
passed 336/336.

## Retained v44.7.10 evidence

## Canonical MaterialID Default Row Order

v44.7.10 is runtime accepted. The shared Fast-grid presentation
layer now gives unsorted Materials, Tensile, Impact and Stiffness views a
natural numeric MaterialID order. `MAT2` precedes `MAT10`; arbitrarily long
numeric suffixes remain deterministic without integer conversion.

The comparer is presentation-only. Canonical observable collections, SQLite
queries/bytes, schema, `SortOrder`, formulas, filters, selection identity,
column-layout preferences, reports, website/FTPS and recovery are unchanged.
Header-click sorting remains session-owned and is reapplied after filter,
reload, Add or Duplicate changes the visible source set.

The candidate also resolves the related Add/Duplicate sort-retention finding:
a new row follows the active ascending, descending or other-column header sort
instead of being appended outside that order. Legacy workflow grids remain
retired.

Initial runtime testing passed the ordering scenarios but exposed a close-time
FK failure after Add/Duplicate: measurement children were auto-saved before the
new parent MaterialID reached SQLite. Closing now commits active Fast editors,
saves dirty parent Materials first, then measurement children, then any derived
Material test-status update. If the parent save is blocked, measurement save is
not attempted and one default-No warning owns the decision.

The close-order re-test passed and exposed a first-load presentation detail:
the deferred filter/collection refresh re-selected the saved MaterialID through
the normal ensure-visible path, moving both viewport axes. Startup now resets
the Fast Materials viewport to `(0,0)` after deferred refresh while retaining
the saved selection. Add/Duplicate still ensure the new row is visible.

A later Add test exposed a pre-existing validation conflict: repeated generic
`New manufacturer / New product line / PLA` placeholders produced duplicate
computed Website Display Names, blocking parent SQLite auto-save and causing
dependent Verification parity failures. Add now includes its generated
MaterialID in the placeholder Product Line; Duplicate appends a MaterialID-
specific copy marker to Marketing Name. Both remain visibly editable while
being unique and save-safe immediately.

Debug and Release builds pass with zero warnings/errors. Diff, 136-column
roadmap, release-documentation and read-only direct/transitive NuGet advisory
gates pass. Owner Add, Duplicate, sort, close/restart and viewport testing
passed; Full Data Verification passed 335/335.

## Retained v44.7.9 evidence

## Public Measurement Date Provenance

v44.7.9 adds a shared typed public
measurement-date projection to the Material Engineering and Test Session
reports. Tensile, Impact and Stiffness dates come from canonical schema-v31
SQLite metadata and are rendered as ISO `yyyy-MM-dd`; absent or invalid values
remain exactly `Not recorded`.

Both report families use explicit allowlists and exclude internal create/edit
timestamps. The existing per-material `PublishPublicReports` boundary remains
the publication owner, while raw inputs and notes still require the separate
`PublishPublicTestDetails` approval. Comparison, manufacturer, material
summary, printing recommendation, routes, manifests, PDF-from-HTML behavior,
website/FTPS, measurements and formulas are unchanged.

Runtime review found that `Build Selected Public Reports` sounded governed by
the adjacent Report template and Report scope controls even though its accepted
handler always builds the Material Engineering public batch. It is now named
`Build Public Material Reports`; its tooltip and workflow guidance explicitly
separate template/scope preview-export actions from the report-family-specific
public build buttons. Handlers, routes and output behavior are unchanged.

The same runtime review showed long button tooltips being clipped at the shared
360-pixel boundary. The existing window-wide tooltip style now wraps its
string content inside that maximum width. Tooltip text, timing and button
behavior are unchanged.

Narrow-window review then showed the Reports workflow buttons being clipped by
their horizontal StackPanel. The buttons now use a WrapPanel with consistent
row spacing, while the workflow guidance remains below them. Button identity,
order and handlers are unchanged.

Debug and Release builds pass with zero warnings/errors. The changed-code bare
`Path.*`/credential-token probes, diff check, 136-column roadmap check and
release-documentation audit pass. Owner runtime review accepted the report
dates, exact missing-data fallback, responsive button layout and wrapped
tooltips. Full Data Verification passed 334/334. v44.7.9 is complete.

## Retained v44.7.8 evidence

## Backup Filename Compatibility

v44.7.8 gives every newly created SQLite backup a readable,
purpose-specific `3DPIceland-...-YYYY-MM-DD_HHmmss_fff.bak` presentation name.
The `.bak` extension changes only the filename: backup creation, integrity and
schema inspection, canonical SQLite contents and restore ownership are
unchanged.

Recovery Center and direct SQLite restore discover both new `.bak` files and
all existing `.sqlite` backups. Legacy files are not renamed, moved or added
to the new cleanup policy. The existing 20-file rotation now applies only to
new `3DPIceland-Automatic-*.bak` files; legacy `.sqlite` evidence is retained.

Manual, pre-SQLite-restore, post-SQLite-restore and pre-Excel-restore backups
use distinct readable names. Guarded updater state continues to retain the
opaque verified backup path, and interrupted recovery still never restores
SQLite automatically.

The candidate adds a Verification gate for legacy/new naming, collision-safe
automatic names, dual-format restore discovery, Recovery Center ownership and
updater/recovery boundaries. Isolated Debug/Release builds passed with zero
warnings/errors; alias, 136-column, diff and release-documentation gates passed.
Owner runtime acceptance and Full Data Verification 333/333 passed.

A direct reflection attempt from Windows PowerShell could not load the net9
candidate assembly because that host lacked `System.Runtime, Version=9.0.0.0`.
This was a test-host mismatch, not a build failure. The same static naming
probe is compiled into and will run inside the net9 in-app Verification Center.

First runtime diagnostics passed 333/333 but exposed a presentation-only count:
`Automatic backups: 21 / 20` combined retained legacy `.sqlite` files with the
new rotating `.bak` set. Diagnostics now report rotating and retained legacy
automatic counts separately. No backup file or cleanup behavior changed.

Final runtime testing confirmed the corrected diagnostics plus manual,
automatic, SQLite pre/post-restore and Excel pre-restore `.bak` names. Legacy
`.sqlite` files remained discoverable and restore-ready. v44.7.8 is complete.

## Retained v44.7.7 evidence

## Legacy Grid Retirement

v44.7.7 is a staged retirement of the accepted Fast-workflow legacy DataGrid
fallbacks. Stage 1 removes every visible legacy/preview switch from Materials,
Tensile, Impact, Stiffness and Settings while retaining Reset Columns and all
canonical editing actions.

The legacy DataGrids remain collapsed internal adapters during this stage
because Fast views still derive column definitions, ComboBox choices and some
visible row projections from them. Removing those grids before introducing
explicit Fast contracts would break the accepted views. Later stages will
replace those dependencies before deleting legacy XAML, event handlers and
commit paths.

SQLite, formulas, validation, filters, Settings CRUD, reports, FTPS, updater
and recovery behavior remain unchanged. Runtime acceptance is required before
the first adapter is removed.

The first Verification run exposed a release-metadata mismatch rather than a
workflow regression: assembly/version fields were 44.7.7 while informational
metadata still identified v44.7.6. That single identity failure cascaded
through 77 aggregate release gates. Informational metadata is now aligned to
`44.7.7-LEGACY-GRID-RETIREMENT`.

Owner runtime testing accepted the Fast-only UI presentation and retained
editing/filter behavior. After the identity correction, Full Data Verification
passed 319/319. Stage 1 is accepted; Stage 2 will replace the three measurement
DataGrid adapters before removing their legacy XAML and event paths.

Stage 2 replaces Tensile, Impact and Stiffness column derivation with explicit
Fast contracts matching the accepted headers, widths, editability, cell kinds
and stable layout keys. Visible rows now come directly from the three canonical
measurement collections intersected with the established Materials filter
MaterialID set. No Fast measurement builder reads legacy DataGrid columns or
items.

Legacy measurement XAML and event paths remain collapsed for this checkpoint;
they are not removed until the explicit contracts pass runtime editing,
layout, filter and Verification tests.

Owner runtime testing accepted measurement columns, editing, navigation,
calculations, layout reset/persistence and Materials-filter propagation. Full
Data Verification passed 320/320. Stage 2 is accepted; Stage 3 will replace the
Materials DataGrid column/item adapter.

Stage 3 replaces Materials column derivation with an explicit 52-column Fast
contract, including the accepted read-only boundaries, three checkboxes and
four governed ComboBox choice sets. Visible rows now come directly from the
canonical Materials collection intersected with the established filter/search
MaterialID set.

The Fast Materials builder no longer reads `NativeMaterialsGrid.Columns` or
`.Items`. Legacy Materials XAML and event paths remain collapsed pending this
checkpoint's runtime editing, filtering, selection, layout and Verification
acceptance.

Initial Stage 3 runtime testing found four ownership regressions. Fast edits
and checkbox changes triggered a full canonical-order reload, Duplicate read
the hidden DataGrid selection instead of the Fast selection, returning from a
measurement tab could leave the rendering surface white until manual reload,
and Add/Duplicate synchronized new measurement rows before the new MaterialID
existed in SQLite. The latter could cause a close-time measurement foreign-key
failure and SQLite/UI parity Verification failures.

Same-scope refresh now updates cells in place and preserves current row order
and selection. Scope changes retain surviving order and selection. CRUD
selection prefers the Fast canonical selection, tab reload invalidates the
surface, and Materials must save successfully before new measurement rows are
synchronized. Blocked Materials saves remain dirty instead of being reported
as saved.

The runtime corrections passed, but Verification still reported 309/321
because the UI had returned to 201 Materials while SQLite retained 203. Delete
had persisted removed measurement children but deferred the parent Materials
save. Delete now completes the required child-first measurement save followed
by immediate Materials persistence. Archive/Unarchive also enter the normal
auto-save queue.

Owner retest confirmed the two test Materials were no longer present, UI and
SQLite returned to 201 canonical Materials and all Stage 3 behavior remained
correct. Full Data Verification passed 321/321. Stage 3 is accepted; Stage 4
will replace the Settings and Base Material DataGrid adapters.

Stage 4 replaces General Settings and Base Material Catalog column derivation
with explicit Fast contracts. General Settings owns six columns with only
`Value` editable. Base Materials owns 23 editable columns and the three
accepted Cooling, Enclosure and Profile-kind ComboBox choice sets. Both views
continue to read their canonical row collections directly.

No Fast Settings builder reads `NativeSettingsGrid.Columns` or
`BaseMaterialsGrid.Columns`. Legacy Settings XAML and edit handlers remain
collapsed pending runtime validation/save/CRUD and Verification acceptance.

Owner runtime testing accepted General Settings validation/save, Deployment
rollback, Base Material editing/ComboBoxes/CRUD, layouts and tab redraw. Full
Data Verification passed 322/322. Stage 4 is accepted; Stage 5 can remove the
retired legacy DataGrid XAML, toggles and grid-only event paths.

Stage 5A removes the Tensile, Impact and Stiffness legacy toggle controls,
toggle handlers, fallback state and legacy reset branches. The three accepted
Fast views are now the only activatable measurement UI paths.

The collapsed measurement DataGrid XAML and grid-only startup/edit/close
callers remain for Stage 5B. Removing them is deliberately gated behind one
runtime pass proving that eliminating the fallback activation paths did not
affect Fast editing, reset, filters, persistence or close-time save.

Owner runtime testing accepted measurement editing, navigation, reset,
filters, layout persistence and close-time save with Fast as the only
activatable path. Full Data Verification passed 323/323. Stage 5A is accepted;
Stage 5B can remove collapsed measurement XAML and grid-only lifecycle code.

Stage 5B begins with Tensile. The complete `NativeTensileGrid` XAML, its
edit/current-cell handlers, commit path, ItemsSource/refresh/filter calls,
workflow-layout ownership, warm-up target and close-time edit checks are
removed. Fast Tensile and the canonical collection/save/calculation paths are
unchanged.

Impact and Stiffness legacy XAML remain until the Tensile deletion passes
runtime editing, reset, filters, persistence, close-time save and Verification.

Owner runtime testing accepted Tensile editing, navigation, reset, filters,
layout persistence and restart/close-time save after complete legacy deletion.
Impact and Stiffness remained normal and Full Data Verification passed 324/324.
Stage 5B-Tensile is accepted.

Stage 5B-Impact removes the complete `NativeImpactGrid` XAML, its edit and
current-cell handlers, commit path, ItemsSource/refresh/filter calls,
workflow-layout ownership, warm-up target and close-time edit checks. Fast
Impact keeps its accepted explicit 45-column contract, canonical filtered row
source, validation, calculations and SQLite save path.

Stiffness legacy XAML remains in place. The Impact deletion is a UI/runtime
candidate until Impact editing, navigation, reset, filtering, persistence,
close/restart behavior and Full Data Verification pass owner testing.

Owner runtime testing accepted the complete Impact legacy-grid deletion,
including editing, validation, calculations, navigation, filters, column
reset, persistence and restart behavior. Tensile and Stiffness remained normal
and Full Data Verification passed 325/325. Stage 5B-Impact is accepted;
Stiffness is the final measurement deletion checkpoint.

Stage 5B-Stiffness removes the complete `NativeStiffnessGrid` XAML, its edit
and commit paths, ItemsSource/refresh/filter calls, workflow-layout ownership
and close-time edit checks. Because no legacy measurement DataGrid remains,
the obsolete deferred DataGrid visual-tree warm-up is also removed. Fast
Stiffness keeps its accepted explicit 18-column contract, canonical filtered
row source, validation, calculations and SQLite save path.

This final measurement deletion remains a UI/runtime candidate until Stiffness
editing, navigation, reset, filtering, persistence, close/restart behavior and
Full Data Verification pass owner testing.

Owner runtime testing accepted Stiffness editing, validation, calculations,
navigation, filters, column reset, persistence and restart behavior. Tensile
and Impact remained normal and Full Data Verification passed 326/326.
Stage 5B-Stiffness is accepted; all three legacy measurement DataGrids and
their obsolete warm-up lifecycle are retired.

Stage 5C removes the retired Tools-menu `Reset Current Workflow Columns to
Default...` command and its now-uncalled generic reset handler family.
Materials receives a local Fast `Reset Columns` action so the accepted reset
capability remains available at its owning workspace. Existing Fast reset
buttons, saved Fast layouts and local default-No confirmation behavior remain
unchanged. Runtime menu, reset and restart acceptance is pending.

Owner runtime testing accepted Tools-menu removal, Materials and measurement
reset actions, default-No cancellation, saved-layout persistence and restart
behavior. Full Data Verification passed 326/326. Stage 5C is accepted.

Stage 5D removes the hidden Settings `Use Legacy Grids` control, toggle handler
and fallback activation state. Fast General Settings and Base Material views
are now the only activatable Settings UI. Their legacy XAML, bind/edit callers
and Base Material selection fallback remain temporarily for the next
runtime-gated deletion stage. Runtime acceptance is pending.

Owner runtime testing accepted both Fast Settings views, tab return, general
and Deployment validation, Base Material editing/CRUD, column reset,
persistence and cross-tab behavior. Full Data Verification passed 326/326.
Stage 5D is accepted.

Stage 5E removes both legacy Settings DataGrid XAML blocks and their
grid-specific bind, edit, undo, layout, recovery-commit and selection fallback
callers. Base Material deletion now uses only the Fast view's canonical
selection. General Settings and Base Material collections, validation, SQLite
save, downstream recalculation and Fast refresh paths remain unchanged.
Runtime acceptance is pending.

Owner runtime testing accepted General and Deployment Settings editing,
validation, Base Material text/ComboBox editing, exact Fast selection CRUD,
column reset, persistence, tab return and restart behavior. Full Data
Verification passed 327/327. Stage 5E is accepted.

Stage 5F removes the hidden Materials Fast-preview toggle, fallback handler,
default-enable flag and legacy-view reactivation method. Fast Materials is now
the only activatable Materials UI. The legacy Materials XAML and grid-specific
selection/edit/CRUD callers remain temporarily for the next runtime-gated
deletion stage. Runtime acceptance is pending.

Owner runtime testing accepted Fast-only Materials startup, editing,
checkboxes, exact-selection CRUD, filters and measurement propagation, local
reset, layout persistence, tab return, close/restart save and 201-row
canonical parity. Full Data Verification passed 327/327. Stage 5F is accepted.

Stage 5G makes `_lastSelectedNativeMaterial`, owned by Fast row selection, the
sole Materials selection contract for reports and Duplicate, Archive,
Unarchive and Delete. New-row focus, archive refresh, restore refresh and
recalculation no longer drive the hidden DataGrid. Legacy XAML and remaining
filter/edit/recovery adapters stay in place for later checkpoints. Runtime
exact-selection CRUD and Verification acceptance are pending.

Owner runtime testing accepted exact Fast selection across Material Detail,
selected-material reports, Duplicate, Archive, Unarchive and Delete, including
filters, sort, tab changes, recalculation and restart cleanup to 201 canonical
rows. Full Data Verification passed 328/328. Stage 5G is accepted.

Stage 5H moves Materials filtering, visible report scope, visible counts and
governed column Verification from the hidden DataGrid view/columns to the
canonical filter predicate and explicit Fast Materials contract. Enter-key
search focus also targets the Fast view. Hidden XAML and edit/recovery adapters
remain for later checkpoints. Runtime filter, report-scope and Verification
acceptance are pending.

Owner runtime testing accepted search and combined filters, honest selection
clearing, Fast focus, measurement propagation, ranking/report visible scope,
archive/unarchive scope and restart parity at 201 canonical rows. Full Data
Verification passed 329/329. Stage 5H is accepted.

Stage 5I removes the hidden Materials DataGrid commit method, cell/current
handlers and edit-transaction guards from recovery export/restore, updater,
validation, close, inventory refresh and manual save. The existing debounce
remains as a Fast canonical autosave coalescer. Validation refresh now
synchronizes the Fast view directly. Owner runtime testing accepted checkbox
and text editing, tab/restart persistence, validation, computed-field rebuild
and recovery export. Full Data Verification passed 330/330; Stage 5I is
accepted.

Stage 5J removes Materials from the legacy workflow-grid registry and retires
its hidden binding, selection, checkbox/copy and refresh callers. Startup
selection and Material Detail now use the canonical Fast selection contract;
Inventory, purchasing and measurement updates refresh the Fast view directly.
The collapsed DataGrid XAML remains only for the final deletion checkpoint.
Owner runtime testing accepted selection, sync and Verification behavior.
The first runtime review found canonical startup selection restored without a
visible Fast-row highlight. Fast activation now explicitly hands the saved
canonical row to the rendering surface after the control is loaded. An initial
pre-layout full synchronization produced a blank surface until manual reload;
the corrected handoff selects only the saved row after layout and never rebuilds
the startup snapshot.
Tab return then exposed a stale rendering viewport: the reusable Loaded handler
reset offsets and the saved-selection callback ran again. Selection is now
one-shot, and Loaded/visibility return resend the measured current offsets at
Render priority, matching the scrollbar refresh path without reloading rows.
The corrected startup selection and tab-return rendering passed owner runtime
re-test. Full Data Verification passed 331/331; Stage 5J is accepted.

Stage 5K deletes the final collapsed `NativeMaterialsGrid` XAML, including its
duplicate columns, row styling and bindings. Fast Materials is now the sole
Materials grid host. Verification requires the retired name to be absent;
canonical SQLite, selection, filters, editing, reports and sync paths are
unchanged. Owner runtime testing accepted startup, selection, tab return,
editing, filters, CRUD, measurement/Inventory sync and column reset. Full Data
Verification passed 332/332. Stage 5K and v44.7.7 are complete.

## Fast Workflow Grid - Settings

v44.7.6 completes the planned input-workspace migration with two Fast views on
Settings Manager. General Settings exposes only `Value` for editing and
preserves manual/close-time canonical save. Deployment host, port and username
retain immediate validation and SQLite save; rejected edits restore both the
row object and Fast cell. Password ownership remains Windows Credential
Manager.

Fast Base Material Catalog exposes all existing text and governed ComboBox
fields. Edits retain immediate canonical SQLite replacement and downstream
Materials recalculation. Existing add/delete buttons use Fast row selection
and reload both views safely. Each view owns separate keyed layout state,
Default-No reset and a shared visible legacy fallback. SQLite schema, reports,
FTPS publishing, updater and recovery remain unchanged. Debug/Release,
static/security gates, Full Data Verification and owner runtime acceptance are
required.

The first Settings-tab runtime materialization failed in WPF `FormattedText`
with `ArgumentOutOfRangeException`. Unlike previously visible Fast grids, the
two Settings surfaces are created while their tab is unrealized and can
receive a transient non-positive DPI/geometry value during the first render.
The shared text renderer now normalizes DPI, width, height and coordinates
before constructing `FormattedText`, preventing lazy-tab render state from
terminating the application.

The crash guard exposed that constructor-created Settings surfaces could still
render blank because they had no realized tab presentation context; the Fast
toolbar buttons were also inserted into the Materials toolbar by an overly
broad XAML patch. Fast Settings activation now occurs only after the Settings
tab is selected at Loaded dispatcher priority. Legacy grids remain available
until activation, and toggle/reset ownership is explicitly inside the Settings
toolbar.

Cross-tab review then exposed legacy Fast layout ambiguity: every unbound blank
spacer used the same `header:` key, so Impact/Stiffness restore grouped all
spacers before editable inputs and could not persist a moved spacer. Duplicate
columns now receive stable occurrence-qualified identities from canonical
column order. A legacy ambiguous layout falls back once to canonical order;
the next preference write uses the unique identities.

Runtime cross-tab testing also found that Materials filters refreshed the
legacy measurement collection views but not the already-created Fast
measurement snapshots. `ApplyNativeMeasurementFilters` now reloads Fast
Tensile, Impact and Stiffness after applying their shared visible MaterialID
set, preserving the established filter owner and canonical results.

Owner runtime retesting accepted both Settings views, validation/save
behavior, legacy fallback, separator persistence and shared Materials filter
propagation. Full Data Verification passed. Debug/Release and static/security
gates passed; v44.7.6 is accepted.

## Fast Workflow Grid - Stiffness Candidate

v44.7.5 applies the accepted Fast Workflow Grid core to Stiffness. Fast
Stiffness starts by default and retains a visible `Use Legacy Grid` fallback.
It edits the existing canonical Stiffness rows and reuses unchanged deflection
and modulus formulas, filters, summaries, measurement dates, test-status
refresh and SQLite auto-save.

Revolutions are bounded to 0–10 and Degrees to 0–359 at both Fast validation
and canonical row boundaries. Rejected values restore the prior cell once.
Computed cells refresh in place, and layout reset preserves row order,
selection and scroll. Stiffness owns separate keyed layout state with immediate
resize/reorder persistence. Debug/Release, static/security gates, Full Data
Verification and owner runtime acceptance are required.

The first Stiffness runtime view exposed a narrow-content coordinate issue:
the render surface was offset inside a viewport wider than its columns while
the overlay editor used a manual scroll-origin calculation. Editors therefore
appeared one column left and an empty leading region was visible. The surface
is now left/top aligned and editor placement uses WPF coordinate translation
from the rendered cell to the overlay, including scroll and DPI offsets.

Owner runtime retest accepted Stiffness bounds, editing, navigation,
calculations, dates, persistence, in-place reset, fallback and the corrected
narrow-grid/editor alignment. Full Data Verification passed 317/317 with 201
canonical Stiffness rows. v44.7.5 is accepted.

## Fast Workflow Grid - Impact Candidate

v44.7.4 applies the runtime-accepted Fast Workflow Grid core to Impact.
Fast Impact starts by default and keeps a visible `Use Legacy Grid` fallback.
It edits the existing canonical Impact rows and reuses the unchanged 0–100
needle-percentage validation, measurement-date assignment, calculations,
summary, filters, test-status refresh and SQLite auto-save paths.

Impact sample colors and computed-cell distinction are retained. Keyed column
width/order state is separate from Materials, Tensile and the legacy grid and
persists immediately after resize/reorder. Computed cells refresh in place so
the accepted Tensile sort, selection and navigation behavior carries forward.
Debug/Release, static/security gates, Full Data Verification and owner runtime
acceptance are required.

The first runtime review found three acceptance issues. Tensile still accepted
negative samples. Fast Impact left a rejected snapshot value in the cell,
causing repeated warnings, and column reset rebuilt canonical row order.
Tensile and Impact now reject negative values at the canonical row boundary;
failed Fast commits restore the previous cell after one warning. Fast reset
now applies captured startup defaults in place, preserving row order,
selection and scroll.

Owner runtime retest accepted Fast Impact editing, bounds, one-time rejection,
navigation, calculations, colors, layout persistence, in-place reset and
legacy fallback. Full Data Verification passed 316/316 with 201 canonical
Impact rows. v44.7.4 is accepted.

## Fast Workflow Grid - Tensile Candidate

v44.7.3 begins the approved phased migration from editable WPF DataGrids to
the accepted viewport-only Fast Materials rendering approach. Fast Tensile is
the first candidate and starts by default while retaining a visible `Use
Legacy Grid` fallback.

The candidate edits the existing canonical tensile row objects and calls the
unchanged calculation, validation, summary, test-status and SQLite auto-save
paths. It retains measurement filters, sample color bands, computed-cell
distinction, keyboard navigation, copy/paste and first-measurement date
assignment. Fast Tensile owns separate keyed column width/order state and
persists resize/reorder immediately; reset is Default-No.

Impact, Stiffness and Settings are deliberately unchanged pending sequential
migration. Debug and Release pass with zero warnings/errors, the
documentation/static checks pass and NuGet reports no vulnerable packages.
Owner runtime testing accepted the full checklist, described the Fast Tensile
view as noticeably snappier than the legacy grid and Full Data Verification
passed 315/315. v44.7.3 is accepted.

The first runtime edit exposed that refreshing computed output rebuilt the
visible row list, discarded its current sort and moved selection from MAT0206
to the first canonical row. The accepted correction refreshes cell snapshots
in place, preserving visible order, selected row and selected cell after
commit.

## Canonical Measurement Date Foundation

v44.6.2 adds one nullable measured date per native MaterialID/TestType and one
per Experimental run. SQLite stores invariant `yyyy-MM-dd` values in additive
schema-v31 columns; the UI displays them consistently as `dd.MM.yyyy`.

Today is assigned only after the first non-empty measurement input when no date
already exists. Historical rows remain blank, later measurement edits preserve
the recorded date, and users may enter or correct a date manually. Clearing
measurement values does not silently erase the date.

Schema-v30 canonical databases remain supported migration inputs. Governed
Excel disaster recovery dynamically includes the new columns; no automatic
SQLite restore, evidence deletion, updater, website/report or FTPS behavior is
changed.

The first runtime review reached Full Data Verification 312/312 and proved
schema-v31 backup readiness and persistence, but exposed two UI acceptance
issues. Merely entering an existing native measurement cell could assign today,
and per-keystroke `DateTime` conversion normalized a partially edited year.
The bounded correction now assigns today only when the first previously empty
numeric measurement is committed on an otherwise empty native test row, and
date text is converted on focus loss so a complete historic date can be entered
before parsing. Experimental first-input behavior remains unchanged.

A second runtime review exposed WPF's nullable-`DateTime` validation lock when
the date cell was cleared. The UI now binds to a nullable-safe text projection:
blank text explicitly stores no measured date, complete `d.M.yyyy` input is
normalized to `dd.MM.yyyy`, and invalid partial text cannot trap the DataGrid
row or block editing in measurement and notes cells.

Stiffness runtime review then showed that focus-loss source timing could let
auto-save run before a manually entered date reached the model. The blank-safe
text projection now updates the model during editing for all four date
surfaces; incomplete text remains harmless, while a completed or cleared value
is available before the save callback.

Template-based Stiffness editing was rejected after runtime showed that the
shared DataGrid workflow could focus its embedded TextBox without delivering
normal text input and also changed row height. Stiffness is again a standard
compact DataGridTextColumn like Tensile and Impact. Its `CellEditEnding`
explicitly commits the text binding before auto-save, preserving reliable
manual dates without a separate grid system.

The standard editor then proved that edit-mode was active, but the calculated
text property discarded every partial date before a complete value could be
formed. Measurement rows now retain the in-progress display text independently
while typing. Canonical `DateTime?` changes only when a complete `d.M.yyyy`
value is valid or the editor is deliberately cleared.

The DatePicker experiment was rejected because it did not match the compact
Tensile/Impact presentation. Final comparison found a Stiffness-only
`CurrentCellChanged` save firing synchronously during selection, before the
shared first-click workflow could open the editor. Stiffness now uses the same
compact DataGridTextColumn as Tensile/Impact and saves completed edits through
`CellEditEnding`; navigation alone no longer triggers a Stiffness write.

Runtime then confirmed typing worked but the first click only selected the cell.
The Stiffness measurement tab can be materialized after `MainWindow.Loaded`, so
its shared first-click/keyboard handlers are now attached during Stiffness grid
initialization as well as the normal loaded-window configuration.

Final runtime review exposed one shared DataGrid lookup defect after a user
reordered columns: the editor used visual `DisplayIndex` against containers
generated in logical column order, so a different read-only cell could receive
edit mode while Measured date appeared selected. Cell lookup now uses the
logical column index. Runtime acceptance confirmed manual Stiffness date entry
before and after column reordering, compact row height matching Tensile/Impact,
restart persistence and Full Data Verification 312/312 PASS.

Material Detail now consumes the same canonical metadata read-only under
General > Test Information. Separate Tensile, Impact and Stiffness measured
dates are displayed as `dd.MM.yyyy`; missing historical dates remain honestly
`Not recorded`. This does not add dates to public report allowlists.

## Canonical Release Documentation Audit

v44.6.1 defines separate canonical roles for CHANGELOG, BUILD_HISTORY, RELEASES
and MILESTONES and reconciles the accepted v44.5.2-v44.6.0 sequence into the
three documents that had stopped at v44.5.1.

`Tools/Test-ReleaseDocumentation.ps1` is a read-only repository audit. It
requires one consistently titled recent release entry in every governed
document and blocks new within-file duplicate identifiers. Existing historical
duplicates are listed explicitly in
`Docs/ReleaseDocumentationAuditBaseline.json`; they remain warnings bounded by
their current occurrence count and approved titles. The audit never edits,
deletes, renumbers or reorders history and is now required by Candidate and
Production release gates.

No runtime data, SQLite/schema, JSON migration, Excel disaster recovery,
backup/restore, updater, website/report or FTPS behavior changes. Debug/Release,
the standalone documentation audit, static/security gates and Full Data
Verification are required.

Runtime acceptance passed Full Data Verification 311/311 with zero failures on
2026-07-24. Release identity aligned at assembly 44.6.1.0, all 201 canonical
Materials produced aligned measurement rows and summaries, schema v30 remained
active, Recovery Center retained six Ready backups and diagnostics reported
zero incomplete updater transactions. The concise Recovery Center presentation
and exact selected-backup Ready detail were visually accepted. v44.6.1 is
runtime accepted.

## Recovery Center Clarity

v44.6.0 removes the 145-pixel verbose application-update evidence box from the
Backup and Recovery Center and replaces the persistent multi-status glossary
with one concise compatibility sentence. Exact file path and compatibility
detail continue to appear only after a backup is selected.

The updater transaction, health acknowledgement, application rollback snapshot
and SQLite backup evidence builders remain unchanged and available through
System Diagnostics and Verification. Restore eligibility, isolated migration
verification, explicit Default-No confirmation, pre-restore recovery backup,
post-restore evidence, Excel disaster recovery and the rule prohibiting
automatic SQLite restore are unchanged.

No database/schema, backup, evidence, updater, website/report or FTPS behavior
changes. Debug/Release, static/security checks, Full Data Verification and
visual Recovery Center acceptance are required.

Runtime acceptance passed Full Data Verification 311/311 with zero failures on
2026-07-24. Recovery Center displayed the concise compatibility summary,
expanded catalog and exact selected-backup Ready detail; Verify Selected passed.
System Diagnostics retained application update transaction and SQLite backup
evidence, confirmed schema v30, six Ready backups, snapshot-folder governance
PASS and zero incomplete transactions. v44.6.0 is runtime accepted.

## Supported Migration Naming

v44.5.9 is a rename-only ownership clarification. Materials startup now calls
`LoadNativeMaterialsFromCanonicalOrMigrationSnapshot`; measurement bootstrap
uses the three `LoadNative*RowsForCanonicalMigration` methods; normal
measurement initialization uses `BuildNative*RowsFromCanonicalStorage`; and
Settings uses `LoadBuiltInNativeSettingsDefaults`.

The underlying bodies, conditions and call order are unchanged. Canonical
SQLite remains first for Materials, JSON snapshots remain bounded to supported
empty-target migration, measurement bootstrap remains guarded by the canonical
migration marker, and built-in Settings defaults remain an explicit Default-No
action. The remaining user-visible `SQLite transition storage` validation
phrase now says `canonical SQLite storage`.

No database/schema, snapshot, backup, recovery, updater, website/report or FTPS
behavior changes. Debug/Release, static/security checks, Full Data Verification
and runtime startup/edit/save acceptance are required.

The first runtime run exposed two pre-existing measurement-boundary defects
while testing a newly created MAT0206 row. Full Data Verification failed
306/310 because a stiffness row with whole Revolutions and blank Degrees was
counted as covered but the calculator required both fields, leaving Material
Detail/Charts and report parity incomplete. The active SQLite row proved the
input was retained. `ResultsService` now treats a missing component as zero
when the other component exists, matching the revolutions-plus-degrees
measurement model; both fields empty still produce no result.

The same run also exposed a close-time race: the window committed only the
Materials grid before shutdown, so an active measurement cell could close
before its deferred auto-save callback. Closing now commits all three
measurement grids and persists them when dirty or actively editing. Failure
remains Default-No through an explicit close-anyway prompt. No automatic
restore or evidence cleanup is introduced. Runtime re-acceptance is required.

Final runtime acceptance passed Full Data Verification 310/310 with zero
failures on 2026-07-24. MAT0206 retained its active-cell measurement edit across
restart, whole-revolution stiffness produced a visible Material Detail result
and 100/100 chart score, and report coverage parity returned to PASS. System
Diagnostics confirmed schema v30, 201 canonical rows in each measurement
workspace, successful Stiffness auto-save, six Ready backups, snapshot-folder
governance PASS and zero incomplete update transactions. v44.5.9 is runtime
accepted.

## Retired Transition UI Residue

The JSON/default/cache caller audit distinguishes five retained
`native-*.json` empty-canonical migration snapshots from active current JSON
owners such as workflow preferences, AI collections, updater transactions and
report/publish manifests. Built-in Settings defaults remain a governed
Default-No SQLite reset/seed path and are not part of the retired original
Excel import.

v44.5.8 removes nine private load/import-sync click handlers that have no XAML
or code caller, plus their four caller-exclusive unsaved-change confirmation
helpers. It also removes six unused measurement JSON state allocations; the
save methods already write only to canonical SQLite. The five migration
snapshot readers remain available for empty canonical targets, and no snapshot,
backup or evidence file is read, changed or deleted by this increment.

Schema v30, governed Excel disaster recovery, explicit SQLite restore, updater,
website/report and FTPS behavior are unchanged. Debug/Release, static/security
checks, Full Data Verification and runtime measurement edit/save/restart
acceptance are required.

Runtime acceptance passed Full Data Verification 309/309 with zero failures on
2026-07-24. System Diagnostics confirmed schema v30, 200 canonical Materials,
four Ready backups, zero incomplete update transactions and successful
Stiffness auto-save after restart. About displayed the exact v44.5.8 release
identity and Recovery Center retained the supported migration evidence while
classifying current schema v30 backups Ready. v44.5.8 is runtime accepted.

## Legacy Workbook Schema Retirement

v44.5.7 advances the canonical database to schema v30. On an existing v29
database it first creates the established integrity-verified SQLite backup and
retains all evidence, then transactionally drops the 13 original workbook,
normalized Materials and pre-canonical measurement/summary tables.

Engineering dashboard, Charts, Compare and report fallback metrics now consume
the live canonical Tensile, Impact and Stiffness rows rather than
`TestSummaryValues`. Tensile/sample/stiffness database readers are canonical
only. Schema v29 and older supported databases must still present their legacy
migration shape during read-only compatibility inspection; malformed v30,
newer and unreadable databases remain blocked unchanged with evidence copies.
If an older database has not completed canonical measurement migration, table
retirement is deferred rather than deleting its only source.

Governed Excel disaster-recovery export and explicit restore remain supported;
they contain only the allowlisted canonical tables and are independent of the
retired workbook schema. Explicit SQLite restore, JSON migration snapshots,
updater, reports, website and FTPS behavior remain unchanged. Runtime Full Data
Verification, recovery export and visual Engineering/Charts/Compare acceptance
are required.

The first runtime run correctly reached schema v30 and removed the legacy
tables, but Full Data Verification failed 292/308. Acceptance checks and
Recovery Center classification still hard-coded schema v29, no post-migration
v30 restore-ready backup had been created, and the first canonical metric
adapter left Impact/Stiffness blank in Mechanical and Charts. The correction
updates all gates/policy text to v30, retains both the pre-migration evidence
backup and a separately verified post-migration v30 backup, and recalculates
canonical Tensile/Impact/Stiffness fields before building the shared metric
contract. Runtime re-acceptance is required.

The second runtime run proved schema v30 backup classification in Recovery
Center and improved Full Data Verification to 300/308. It also proved that
MAT0102 still had 16 canonical Impact samples and one canonical Stiffness input
row even though Mechanical and Charts displayed blanks. The follow-up reads
those canonical rows directly when the initialized UI collections have no
usable measurement row, and the local restore release gate now accepts the
current schema v30 backup that Recovery Center already classifies as Ready.
Debug and Release compile with zero warnings and errors.

Final runtime acceptance passed Full Data Verification 308/308 with zero
failures on 2026-07-24. Mechanical displayed canonical Impact and Stiffness
values for MAT0102, Charts displayed both scores, and Recovery Center displayed
schema v30 backups as Ready. System Diagnostics confirmed schema v30, 200
canonical Materials, three Ready backups and no incomplete update transaction.
v44.5.7 is runtime accepted.

## Retired Workbook Metadata Readers

v44.5.6 removes the original-workbook `Imported test sheets` list from Material
Detail, the legacy Database Engine Stats tool and original import counts/source
metadata from System Diagnostics. Their two public database readers and
display-only models are removed.

The legacy workbook tables are deliberately not dropped in this increment.
Active-database compatibility inspection, remaining supported-schema fallback
readers, governed Excel disaster recovery, explicit SQLite restore and JSON
migration snapshots remain unchanged. The separate schema-retirement increment
must create and verify a retained SQLite backup before dropping tables.

A Verification gate proves the workbook metadata readers and UI surfaces are
absent while compatibility and recovery boundaries remain available. Runtime
Full Data Verification passed 307/307 with zero failures on 2026-07-24;
Material Detail, diagnostics and governed Excel disaster-recovery export were
accepted and v44.5.6 is runtime accepted.

## Retired Legacy Write Entry Points

v44.5.5 removes the caller-free `ReplaceWorkbook`, `ReplaceMaterials` and
`ClearCache` public write entry points from `LocalDatabase`. These broad
operations belonged to the retired original-workbook/cache workflow and could
replace or clear normalized Materials, Manufacturers and imported workbook
metadata if accidentally reconnected.

The `Imports`, `ExcelSheets`, `ExcelSheetRows` and related tables are retained:
Material Detail and diagnostics still read them, and active-database
compatibility inspection still requires the supported legacy schema shape.
Their read paths are unchanged. Governed Excel disaster recovery, explicit
SQLite restore, JSON migration snapshots, updater, reports, website and FTPS
behavior are unchanged. A Verification gate proves the write entry points are
absent while the compatibility and recovery boundaries remain available.
Runtime Full Data Verification passed 306/306 with zero failures on 2026-07-24;
v44.5.5 is runtime accepted.

## Measurement Help Clarity

v44.5.4 removes literal duplicated sentence fragments from the Tensile,
Impact and Stiffness measurement workspace instructions. The three help
surfaces now have stable names and a Verification gate checks their complete
accepted wording so the duplication cannot silently return.

Only XAML help text, release identity, Verification coverage and documentation
are changed. Measurement inputs, calculations, SQLite/JSON compatibility,
Excel disaster recovery, restore, updater, reports, website and FTPS behavior
are unchanged. Runtime Full Data Verification passed 305/305 with zero failures
on 2026-07-24; all three help surfaces were visually accepted and v44.5.4 is
runtime accepted.

## Canonical Storage Terminology

v44.5.3 removes stale user-visible `JSON transition`, mixed-storage and
general Excel-import wording from About and the native measurement status
summaries. The replacement wording identifies SQLite as the canonical store
while explicitly retaining legacy JSON snapshots for supported empty-database
migration.

The underlying JSON migration readers are unchanged. Governed Excel
disaster-recovery export/verification/explicit restore, SQLite backup and
explicit restore, updater, reports, website and FTPS behavior are unchanged.
A new Verification gate requires canonical SQLite terminology together with
all four JSON migration readers and both recovery boundaries. Runtime Full
Data Verification passed 304/304 with zero failures on 2026-07-24; v44.5.3 is
runtime accepted.

## Canonical SQLite UI Boundaries

v44.5.2 removes the misleading Reload Local Cache and Clear Local Cache menu
surfaces. Reload only inspected the retired `MaterialsImport` projection and
did not reload canonical Materials. Clear was a broad legacy-engine operation
that also cleared the currently governed `Manufacturers` table, so it was not a
safe general cache command.

The obsolete `MaterialsImport` table, its active sync command, automatic
empty-database fallback, reader and writer are retired with explicit owner
approval. Startup detects the table, creates and verifies the established
required SQLite migration backup, then drops only `MaterialsImport`. Existing
backups retain the retired rows as evidence and are never deleted
automatically. Settings now accurately offers `Restore Built-in Defaults`; it
never read Excel. Open Storage Folder uses matching handler/error terminology.
A dead unbound Excel-material reset handler and helper are removed.

JSON migration snapshots, governed Excel disaster recovery, SQLite restore,
updater, reports, website and FTPS behavior are unchanged. A new Verification
gate proves that misleading cache UI/dead reset handlers and the
`MaterialsImport` runtime surface are absent after backup-first retirement.
Other legacy tables remain separate audit items. Runtime Full Data Verification
passed 303/303 with zero failures on 2026-07-24; v44.5.2 is runtime accepted.

## Active SQLite Compatibility Safety

v44.5.1 replaces the early startup `File.Delete(DatabasePath)` compatibility
reset with a cohesive read-only inspection and evidence-preservation service.
Supported existing schemas keep the established required-backup and migration
path. A newer, malformed, unreadable or structurally unsupported active
database is left byte-for-byte unchanged, copied to a deterministic
`filamentdb_startup_blocked_*.sqlite` evidence file with SHA-256 verification,
and startup stops with the exact retained path.

The contract verification uses isolated supported, canonical-only, newer and
unreadable fixtures. It requires supported startup to remain available and
every unsupported fixture to be blocked with both its active bytes and evidence
copy unchanged. No automatic SQLite restore, deletion or replacement is added.
Updater rollback remains application-only because the active SQLite file is
not moved or overwritten before startup health acknowledgement.

The first compile probe exposed two missing explicit `System.IO` type
qualifications in the new service; those were corrected. Isolated Debug and
Release builds then passed with zero warnings and zero errors. The first
runtime Verification run exposed a Windows SQLite pooling handle that remained
on the isolated canonical-only fixture after read-only inspection. Service
inspection and fixture connections now disable pooling so every handle is
closed before evidence-copy/hash validation. Runtime Full Data Verification and
explicit startup/recovery acceptance then passed 302/302 with zero failures on
2026-07-24; v44.5.1 is runtime accepted.

## Retired Excel Import Surface

v44.5.0 is the first bounded Legacy Compatibility Audit increment. It removes
the unreachable original-Excel database import click handler and its two
caller-exclusive importer services. The lower-level SQLite compatibility
tables, models, readers and writers remain intact so existing migrated data and
pre-canonical fallback behavior are not retired by inference.

Governed Excel disaster-recovery export, manifest/hash verification,
transactional restore and required SQLite recovery backup are unchanged. JSON
migration snapshots, Recovery Center, SQLite restore, updater, reports,
website and FTPS behavior are also unchanged. Stale empty-data messages now
refer to loading canonical Materials rather than an unavailable original-Excel
import command.

A new Verification check requires the retired handler to remain absent while
the governed Excel backup, verification and guarded-restore entry points remain
available. Isolated Debug and Release builds pass with zero warnings and zero
errors. Runtime Full Data Verification passed 301/301 with zero failures on
2026-07-24; v44.5.0 is runtime accepted.

## Measured Materials Responsiveness

v44.4.1 replaces only the wide daily-use Materials DataGrid presentation with
a viewport-only owner-drawn view after repeated runtime measurements showed
approximately 10–15 second cold horizontal jumps in the 54-column WPF
DataGrid. The accepted view draws only visible rows and columns and keeps the
full canonical Materials list, direct text/ComboBox/checkbox editing, precise
checkbox hit bounds, selection, sorting, keyboard navigation, clipboard,
filters, search and machine-local keyed width/order persistence.

Edits enter the existing canonical validation and SQLite auto-save workflow;
there is no second data model or automatic restore path. The previous native
DataGrid remains available through the checked Tools toggle as an immediate
session fallback. The standalone rendering prototype retains explicit
snapshot Apply/Discard/Cancel behavior and is not the daily-use path.

Visual Studio runtime acceptance confirmed sub-second cold jumps and drags,
normal vertical scrolling, editing/persistence, automatic filter/search
synchronization, fallback switching and WPF-style headers. Full Data
Verification passed 300/300. Debug/Release, NuGet vulnerability and Candidate
release gates pass. The direct-canonical first-install package contains a
68,169,842-byte installer
(`9DACC8B5B2E1DD2CE300AEFDC538C3FCC8327410E4BD89E7301D518C707F6A44`)
and a 95,547,867-byte portable ZIP
(`3AB0CA35C049DD121CDA46EA3D58748622B41C01603598762279BAC49541C16D`).
The 95,918,523-byte trusted ECDSA package SHA-256 is
`25F9BC6E1737EE6A1A51199279D55738AB1842AA6E40053D305D9F06DFE3D489`.
Authenticode remains deferred, so Windows may show Unknown publisher.
Clean-VM runtime acceptance completed on 2026-07-24: direct install, explicit
SQLite restore, startup-default Fast Materials editing/filtering/scrolling,
native DataGrid fallback, portable runtime and Full Data Verification all
passed. v44.4.1 is the canonical runtime-accepted release.

## Backup, Recovery and Update Evidence Clarity candidate

The first v44.3 increment corrects a presentation/policy mismatch observed on
clean VM backups. A schema-v29 SQLite backup with `PRAGMA integrity_check = ok`
and zero canonical Materials is a healthy clean-profile snapshot, not corrupt
or structurally incomplete. It is now classified `Ready — empty profile` and
remains an explicit, confirmation-gated restore source.

Recovery Center explains Ready, healthy empty profile, Migration required,
Legacy/incomplete, Newer/incompatible and Corrupt/unreadable states in one
visible glossary. Empty-profile restore confirmation states that it will
replace the current database with an empty profile. Full-data release evidence
continues to require a separate Ready backup containing canonical Materials.
Pre-restore recovery, post-restore evidence, rollback, restart, default-No
confirmation, no automatic SQLite restore and evidence retention are unchanged.

Clean-VM runtime acceptance completed on 2026-07-23. Recovery Center showed two
integrity-valid schema-v29 zero-Material automatic backups as
`Ready — empty profile`, including one with 50 Settings and one with zero
Settings. The first Verification run exposed a gate-only defect: v44.3 reused
the v43.1 full-data-backup predicate and therefore reported 208/209 despite the
correct UI classification. The correction separates the guarded restore API/UI
contract from v43.1 full-data release evidence; v43.1 remains honestly not
applicable on a clean profile. The rebuilt Candidate then passed Application
Readiness and Overall Verification with 209/209 applicable checks and 90 not
applicable.

v44.3.1 adds the second bounded increment without changing the durable updater
schema or recovery engine. Backup and Recovery Center now shows the latest
application-update evidence in four separate read-only boundaries:
transaction state, health acknowledgement, application rollback snapshot and
SQLite backup evidence. Every boundary explicitly reports present, missing,
invalid/unreadable or not recorded state. Refresh rereads the evidence but does
not create, modify, recover or delete anything.

Application rollback and SQLite recovery remain explicitly separate. The panel
states that application rollback never restores SQLite automatically; SQLite
recovery remains a separate default-No operation and all evidence/backups are
retained. A clean profile with no transaction history still displays all four
boundaries as `None recorded`, rather than hiding the distinction.

Clean-VM runtime acceptance completed on 2026-07-23. The Recovery Center showed
all four boundaries as `None recorded`, retained both healthy empty-profile
backups and displayed the explicit application-rollback/SQLite separation.
Application Readiness and Overall Verification passed 209/209 applicable checks
with 90 not applicable. v44.3.1 is the canonical runtime-accepted release.
The tested distribution artifacts remain Candidate; Production promotion and
stable-route publication remain separate clean-tree/default-No operations.

## Daily-use UI State and MaterialID Clarity candidate

Research confirmed that window geometry and keyed DataGrid widths were already
stored machine-locally in `%LocalAppData%\3DPIceland\FilamentDbApp\
workflow-preferences.json`. Column display order was captured but never restored,
and the selected Material was retained only as an in-memory row reference.

v44.2.0 restores keyed column widths and valid, non-conflicting saved
`DisplayIndex` order. A/B runtime testing temporarily disabled order restoration,
but the same roughly 15-second first horizontal page jump remained. Order
restoration was therefore exonerated and restored; invalid, duplicate or
out-of-range positions still fall back safely.
The last canonical MaterialID is stored in the same machine-local preference
file and restored only when that MaterialID exists in the current visible
dataset; an absent or filtered identity is never forced back as stale selection.

Materials, Material Detail and Reports each show the same explicit MaterialID.
An attempted `FullRow` selection-mode change was rejected during runtime testing
because it conflicted with the accepted one-click editor. The accepted correction
keeps `CellOrRowHeader` and uses one presentation-only row flag, with selection
brushes that do not alter editing. Checkbox mutation is limited to the rendered
checkbox bounds; blank cell space selects the material only.

Runtime investigation measured about 15 seconds for a first large horizontal
Materials-grid jump into previously unrealized columns. Arrow scrolling remained
responsive, proving the bottleneck was a large pixel-offset/column-virtualization
layout rather than SQLite auto-save. Disabling column virtualization made all
scroll paths responsive but caused an unacceptable startup stall and was
removed. A later bounded ScrollViewer timer experiment was also rejected after
a large track jump stalled halfway and left the app unresponsive at 0% CPU.
The custom timer/ScrollChanged state machine was removed completely. Materials
keeps native live scrolling plus row/column virtualization; the measured
first-large-jump delay remains an open performance finding rather than risking
startup regression or a frozen UI.

No SQLite schema, engineering backup, portable package data, calculation,
website/report publishing or recovery contract changes.

## Verification Profiles and Diagnostic Honesty candidate

v44.1.0 adds an explicit verification profile layer without changing any
calculation, SQLite, update, recovery, website, report or publishing contract.
A profile with zero active canonical Materials is `Application Readiness`;
profiles containing Materials remain `Full Data Verification`.

Passing checks remain PASS in both profiles. On a legitimate zero-data profile,
only known data-dependent failures with explicit zero-data reasons become
`NOT APPLICABLE — No canonical data`. Unexpected schema, asset, privacy,
release-identity, installer, update or recovery failures remain FAIL. Exports
show the profile name, selection reason and applicable/pass/fail/not-applicable
counts. Full Data Verification retains every check as applicable and mandatory.

The v44.1 contract gate also proves that compiled production seed Materials and
default deployment identity remain empty and that unexpected readiness failures
cannot be reclassified as not applicable.

Fresh-VM v44.1.0 diagnostics proved 207 applicable PASS, 88 not applicable and
two remaining FAIL results. `Website portal release contract` and `Website
export package contract` are known downstream zero-data/template dependencies,
but their generic detail text did not carry the zero-data marker used by the
fail-closed classifier. v44.1.1 explicitly adds only those two named contracts
to the clean-profile allowlist. They remain mandatory in Full Data Verification.

Fresh-VM v44.1.1 runtime acceptance then passed Application Readiness with
207/207 applicable checks, zero failures and 90 explicitly not-applicable
data-dependent checks. Release identity, schema-v29 clean-profile isolation,
empty deployment identity and signed/default-No update/recovery/deployment gates
all passed. Restored owner-data Full Data Verification remains pending.

Restored owner data selected Full Data Verification correctly and passed 293/297
before the v44.1.2 correction, then 297/297 after Create SQLite Backup. The four
intermediate failures were recovery-evidence gates: the explicit restore retained
a pre-restore backup of the empty profile but did not create a canonical local
backup of the successfully restored state. v44.1.2 makes successful SQLite
restore atomically create and verify a `Post-SQLite restore evidence` backup with
matching schema, Materials, tensile, impact and stiffness counts. Explicit
default-No restore, rollback on failure, evidence retention and no automatic
SQLite restore remain unchanged.

Final clean-VM runtime acceptance proves both profiles. Application Readiness
passes 207/207 applicable checks with 90 data-dependent checks reported as
not applicable. After explicit restore and automatic restart, Full Data
Verification passes 297/297 immediately without a manual backup step. The
Recovery Center shows the verified schema-v29 post-restore evidence backup as
Ready with 200 Materials, 3,728 tensile rows, 3,752 impact rows, 191 stiffness
rows and 50 settings; pre-restore and automatic/migration backups remain
retained and honestly classified.

## v44.0 release-workflow closure candidate

Canonical runtime identity remains v43.8.9. A direct v43.8.9 Setup/portable
Candidate has now been built from the byte-identical canonical signed package
with Inno Setup 7.0.2. It passes local release gates. Fresh-VM direct-installer
runtime acceptance also passed with aligned identity and a privacy-clean
schema-v29 zero-Materials profile; the exported diagnostics had 223 PASS and 91
expected data-dependent FAIL results because the full-data profile requires
canonical Materials, website template and recovery backup. Fresh-VM portable
runtime acceptance also passed from an isolated Documents folder with the same
clean-profile boundary and no portable-specific failure. The direct v43.8.9
first-install route is accepted for promotion, but the currently published
stable route remains v43.8.8 plus guarded update until a clean-tree,
byte-preserving Production promotion and separate publish verification complete.

The clean-tree Production promotion subsequently passed. Installer and portable
bytes are identical to the fresh-VM runtime-accepted Candidates, Production
metadata is BOM-less and binds promotion commit `dd4eaf4`, and the full
Production release-gate suite passed. Stable-route FTPS publication remains a
separate explicit default-No operational action and was not performed as part of
local v44.0 closure.

Packaging now declares Candidate or Production state, refuses silent artifact
replacement, and requires a clean Git tree for Production. Feed and deployment
metadata are BOM-less. A single release-gate script verifies NuGet vulnerability
results, exact bytes/SHA-256, ECDSA signature, governed inventory, schema and
stable-route-last publishing. Authenticode remains deferred; Windows may show
Unknown publisher even though the application update package has its separate
trusted ECDSA signature.

## SQLite Dependency Security

v43.8.9 is a narrow security-maintenance candidate. It updates `Microsoft.Data.Sqlite` from 9.0.7 to the current net9 servicing release 9.0.18 and directly selects `SQLitePCLRaw.bundle_e_sqlite3` 2.1.12. This prevents NuGet's minimum-version resolution from retaining the high-severity affected `SQLitePCLRaw.lib.e_sqlite3` 2.1.10/2.1.11 line (CVE-2025-6965). No schema, SQLite ownership, backup/restore, update transaction, website, report or FTPS behavior changes.

`UpdateCore` is now explicitly included in `FilamentDbApp.sln`. The application already used a project reference, but Visual Studio could report NU1105 after `.vs`/`obj` cleanup because the referenced project was not a solution member; command-line restore masked that solution-state gap.

Local Release runtime Verification Center passed 296/296 on 2026-07-22 with schema v29 and the existing 200-material owner database. The v43.8.9 signed candidate remains pending a guarded v43.8.8-to-v43.8.9 VM update/runtime acceptance before canonical promotion.

The first v43.8.8 VM discovery attempt exposed a Windows PowerShell encoding boundary: `Set-Content -Encoding UTF8` emitted an `EF BB BF` BOM, while the byte-oriented JSON reader requires `{` as the first token. Feed generation now writes explicit BOM-less UTF-8. The v43.8.9 reader also strips one standard UTF-8 BOM defensively for backwards compatibility; feed size, hash, signature and governed-root validation remain unchanged.

Canonical runtime acceptance completed in a snapshot-isolated VM. The BOM-less feed updated a fresh v43.8.8 install to v43.8.9 with a `Committed` transaction and zero incomplete transactions while preserving the intentional zero-Materials profile. A schema-v29 owner backup then restored 200 Materials, 3,728 tensile samples, 3,752 impact samples and 191 stiffness rows; a new canonical 200-Material SQLite backup made every recovery gate ready. Final Verification Center result was PASS 296/296 with zero failures.

# Previous Build Notes - v43.8.8

## Production Consolidation

v43.8.8 consolidates the runtime-accepted remote update delivery, SQLite-native restore, clean-profile Verification fixes and interrupted snapshot/recovery hardening proven through the v43.8.1-v43.8.7 VM candidates. It also bounds chunked HTTP responses during streaming download and gives update-feed publishing remote staging, backup and rollback parity. This is the first production identity above every published VM candidate.

# Previous Build Notes - v43.8.0

## Remote Signed Update Delivery

v43.8.0 adds manual and delayed one-minute HTTPS discovery at the governed `/updates/latest.json` route. The feed embeds the production-signed package manifest, which is authenticated before download; package URL is confined to the same HTTPS update root and downloads are bounded to 512 MiB. Downloaded bytes and SHA-256 must match the feed, then the complete ZIP must reproduce the trusted manifest and pass the existing inventory, signature, version and schema verifier before Default-No Apply is offered.

`Publish Application Update...` separately verifies and publishes the versioned signed ZIP before activating `latest.json`; it does not use website deployment state or change SQLite. Runtime acceptance completed through clean installer, remote update and restored-data VM scenarios. The accepted transaction engine keeps incomplete snapshot preparation in `Prepared`, retries transient Windows file locks, skips byte-identical rollback files, uses the verified staged helper, and preserves durable error/path evidence. SQLite restore now uses the native SQLite backup API instead of replacing an open Windows file; no update or recovery path automatically restores SQLite.

VM v43.8.5 -> v43.8.6 and v43.8.6 -> v43.8.7 transactions committed with zero incomplete transactions. A schema-v29 backup restored 200 Materials, 3,728 tensile samples, 3,752 impact samples and 191 stiffness rows with a verified pre-restore recovery backup. Final Verification Center result was PASS 296/296 on v43.8.7. Canonical repository release identity remains v43.8.0.

# Previous Build Notes - v43.7.0

## Installer and Portable Deployment

v43.7.0 consumes the production-signed six-file application package and produces two first-install artifacts: a per-user/no-admin Inno Setup EXE and a portable ZIP containing the exact governed runtime inventory. The installer targets `%LocalAppData%\Programs\3DPIceland Engineering Platform`, creates Start Menu and optional Desktop shortcuts, and uninstalls only application files. SQLite, backups, configured storage, Credential Manager secrets and update transaction evidence remain external and are never installer payloads or uninstall targets.

`App/build_deployment_artifacts.ps1` first invokes the real application package verifier, extracts only the accepted signed payload into a disposable source tree, rejects database/spreadsheet/legacy JSON files, the historical website snapshot and private material or FTPS identity markers, builds the portable ZIP and compiles `Deployment/3DPIcelandInstaller.iss`. It writes a versioned deployment plan binding both artifacts to byte length, SHA-256, versioned FTPS paths and stable download paths. The corrected v43.7.0 VM candidate contains a 68,032,212-byte installer and 95,403,440-byte portable ZIP; the portable archive contains exactly six governed runtime files.

Tools now includes explicit default-No `Publish Application Release...`. It accepts only the exact Installer/Portable deployment plan, rechecks local bytes and SHA-256 before connection, uses the existing SQLite-governed explicit-FTPS endpoint and host/user-scoped Credential Manager password, stages versioned files first and activates stable `/downloads` routes last. Remote backup/staging is isolated under `/backups/application_releases`; website deployment manifests and routes are blocked. Public browser routes are `https://www.iskort.is/3dp/downloads/3DPIceland-Setup-x64.exe` and `.../3DPIceland-Portable-x64.zip` because the FTPS account root already maps to `/3dp`.

The first VM candidate exposed 176 historical production material rows because an obsolete clean-profile fallback was compiled into the executable. A second audit found that `website-template-index.html` also carried a publication snapshot outside SQLite. The next VM probe then found an owner-specific FTPS host/user seed. Clean startup now uses only SQLite or an explicit legacy/import source for materials, website templates require explicit owner import, and new deployment settings start with empty host/user values. A later VM probe found that the loose PNG URI did not render from the installed single-file application; splash/header branding now uses an embedded WPF pack resource while the ICO and report JPEG retain their governed external roles. Three unused SVG development diagrams were removed from publish/governed inventory. Earlier candidates were deleted and invalidated.

Authenticode is intentionally deferred while distribution remains private; Windows may show Unknown publisher. The existing ECDSA signed-package verification remains mandatory. Debug/Release builds and corrected artifact compilation pass with no private marker in the signed package, installer or portable ZIP. Verification Center passed; corrected clean-VM install/restart, zero-data boundary, explicit SQLite transfer, credential isolation, embedded branding, live application-release publish and browser download were runtime accepted.

# Previous Build Notes - v43.6.0

## Update and Deployment Diagnostics

v43.6.0 adds a backwards-compatible read-only catalog over the existing v1 durable transaction state and request files. System Diagnostics lists prior transaction identity, versions, phase, timestamp, recommended recovery and the retained SQLite backup reference without changing application files, transaction evidence, backups or SQLite.

Normal startup detects `Prepared`, `SnapshotReady`, `Installed`, `RollingBack` and `RollbackFailed` transactions, except the transaction currently completing its guarded health acknowledgement. Any application-file recovery is explicit and default-No. The existing external helper waits for the running PID before recovery. `Prepared` safely restarts the guarded transaction; the other incomplete phases restore the complete last-known-good governed-file snapshot and relaunch the prior application. Request/state identity and live-directory containment must match before mutation.

No transaction evidence or backup is automatically deleted. SQLite is never automatically restored. Website/report generation and FTPS publishing are unchanged. Isolated updater tests cover all five interrupted phases, read-only history classification, prior commit/rollback behavior and path traversal. Debug and Release builds complete with zero warnings and zero errors. Visual Studio Debug runtime acceptance on 2026-07-22 confirmed v43.6.0 identity, read-only history with one prior Committed transaction, zero incomplete transactions and Verification PASS 294/294.

# Previous Build Notes - v43.5.1

## Guarded Application Update

Update Readiness now offers a separate default-No Apply confirmation only after a package is newer and every manifest, exact inventory, byte length, SHA-256, production signature and SQLite schema check passes. Apply repeats full verification during extraction into a unique LocalAppData transaction staging folder, commits active Materials edits, requires SQLite save completion and creates a verified manual SQLite backup before shutdown. The package must govern the running executable and an installed external helper; transaction and live application folders must share a volume but may not contain one another.

The app copies the installed helper to the transaction folder, writes a durable request and launches that copy before shutting down with stale close-save suppressed. The helper waits for the exact old PID to exit, snapshots all governed live files, installs the staged set and launches the new executable with a contained health-ack path and transaction identity. The new app writes acknowledgement only after MainWindow construction, SQLite startup/schema checks and first usable content rendering. Helper acceptance requires matching transaction ID, exact new release version and supported SQLite schema.

Launch failure, early process exit, timeout, invalid acknowledgement or unsupported schema kills the candidate process, restores the last-known-good application files and relaunches the prior app. SQLite is never automatically rolled back; the verified pre-update backup remains recorded for guarded recovery. Apply remains manual; key loss/rotation and unattended updating are still out of scope.

For isolated runtime acceptance, production-signed v43.5.1 base and v43.5.2 candidate packages are created from the same governed code with assembly/informational version override. v43.5.1 is extracted into a disposable portable app directory so the repository/Visual Studio outputs are never live-update targets.

Runtime acceptance completed on 2026-07-22. The signed v43.5.2 package passed all readiness checks with 11 governed files and SQLite schema v29, the external helper committed the v43.5.1 to v43.5.2 transaction, and the restarted app acknowledged the exact transaction ID, release v43.5.2 and schema v29 after first usable rendering. Verification Center passed 293/293 after restart. Durable state recorded `Committed`; a verified manual SQLite backup was created before mutation and SQLite was not restored.

The first portable launch exposed that `MainWindow.xaml` loaded a loose ICO through a BAML type converter. Shipping the file alone was insufficient for single-file WPF and startup failed before updater or SQLite mutation. The runtime `Icon` URI was removed because `ApplicationIcon` already embeds the same icon in the executable; the ICO remains explicitly governed as the eleventh signed package file. Startup diagnostics now also display the deepest inner exception.

## Transactional Updater Engine

The first v43.5 increment adds a separate `3DPIcelandUpdater.exe` and a shared transaction engine, but deliberately does not expose live application-file Apply. A versioned durable transaction state records Prepared, SnapshotReady, Installed, Committed, RollingBack, RolledBack or RollbackFailed with previous/new version identity and the verified pre-update SQLite backup reference. The engine accepts only unique safe relative governed paths, requires every staged file, confines staging/rollback below the transaction folder and keeps that folder separate from the live application directory.

Before mutation, every existing governed live file is copied atomically into a last-known-good rollback tree and previously missing paths are recorded. Staged files then replace only the governed application paths through per-file temporary copy/rename. Failed installation or failed health acknowledgement restores the complete snapshot and removes files that did not exist before the transaction. SQLite is never copied or restored by this engine; its verified backup path is evidence for separately guarded database recovery.

Isolated self-tests use disposable fake application trees. A complete update committed, an injected failure after the first installed file restored all old files, failed health acknowledgement rolled back, and traversal was blocked before mutation. The external helper runs the same contract self-test. It is added as the tenth governed file in the signed update package. Live process waiting, launch, health acknowledgement and the guarded Apply UI remain disabled until this engine is runtime accepted.

Runtime acceptance on 2026-07-22 confirmed Verification Overall PASS and selected the production-signed v43.5.0 package in Update Readiness. Package, manifest, ten-file inventory, SHA-256, trusted signature and SQLite schema v29 all passed; version policy alone correctly blocked the same-version v43.5.0 package. No live application or SQLite files were changed.

## Governed Signed Release Packaging

A dedicated Windows-only `Tools/ReleasePackager` now creates the application update signing identity and package. The production ECDSA P-256 private key is persisted in the current Windows user's Microsoft Software Key Storage Provider under `3DPIceland.ApplicationUpdate.Release.v1`, is marked non-exportable and passed both sign/verify and blocked-private-export probes. Only the public SubjectPublicKeyInfo and its pinned SHA-256 fingerprint `87D407FEA230D484D8F436A4BA4958BF7F70336B968FFEC7F3966C15DFDFF1EA` are embedded in the application.

`App/build_signed_update.ps1` requires a clean Git worktree for normal production packaging, performs the canonical self-contained single-file Release publish, reads version identity from the project, requires exactly nine governed runtime files and invokes the CNG-backed packager. An explicit `-AllowDirty` switch exists only for pre-release verification before runtime acceptance. PDB and the three known WebView2 XML documentation files are explicitly excluded; any other unexpected publish output blocks packaging. SQLite, backups, credentials, storage configuration, reports, website staging and developer files are never package candidates. An independent console probe references the real application verifier and rejects a newly created package unless its production signature, identity, file inventory, hashes and SQLite schema policy pass.

The canonical v43.4.1 pre-release packaging run created `3DPIceland_Update_v43_4_1.zip` with nine governed files and SQLite schema v29-v29. The application verifier accepted the production-signed package, then blocked a separately modified production package at the LICENSE length/hash boundary. Single-file `Assembly.Location` fallbacks were removed so canonical publish completes without IL3000 warnings. The update remains read-only: no helper, staging, extraction or application-file replacement exists yet.

Operational risk: the current production private key is intentionally non-exportable and therefore tied to this Windows user profile/machine. Loss of that key requires a manually installed future build with a new trust root; automated key rotation/recovery must be designed before unattended updater deployment.

Runtime acceptance on 2026-07-22 selected the production-signed v43.4.1 package in Tools > Update Readiness. Package readability, manifest, nine-file inventory, SHA-256, trusted production signature and SQLite schema v29 policy all passed. Version policy alone was correctly blocked because package and installed application were both v43.4.1. Verification Center was Overall PASS.

## Signed Update Readiness Foundation

Tools > Update Readiness now performs a read-only inspection of a selected ZIP update package. The versioned `3dp-update-manifest.json` contract governs release identity, supported SQLite schema range and a complete application-file inventory with exact lengths and SHA-256 hashes. ZIP traversal, rooted/backslash paths, duplicate/reserved paths, missing or extra files, malformed hashes, unsupported algorithms and invalid manifest fields are blocked before any trust or version decision.

The manifest requires an ECDSA P-256/SHA-256 signature over a deterministic payload. Production deliberately has no fallback to hash-only trust: until the governed release public key is provisioned, an otherwise intact package reports `Signing trust root not provisioned` and remains blocked. This foundation does not download, extract, stage, replace or launch application files and never changes SQLite data. An external transactional updater and protected private-key release workflow remain follow-on work.

Verification creates isolated temporary packages and requires a valid signed fixture to pass while tampered content, downgrade, path traversal and absent production trust are blocked. Temporary fixtures are deleted after the check. Existing SQLite backup/recovery and website FTPS publishing/rollback boundaries remain unchanged.

The obsolete `Restore Excel Defaults` entry was removed from Tools because canonical Materials are SQLite-owned and governed recovery now belongs in Backup and Recovery Center. The underlying backwards-compatibility code is not rewritten by this UI cleanup.

Runtime acceptance on 2026-07-22 confirmed v43.4 startup, the new Tools > Update Readiness entry and Overall Verification PASS. Release and Debug builds completed with zero warnings and zero errors.

## Backup and Recovery Center UI

The File menu now exposes one `Backup and Recovery Center` entry instead of separate Excel import/export, Excel restore, SQLite restore and Recovery Center commands. The center retains the compatibility catalog and adds manual SQLite backup, browse/restore an external SQLite backup, governed Excel backup/restore and one Open Storage Folder action. The retired legacy Excel database import is intentionally omitted. Its wrapping toolbar remains usable when the window is narrowed.

System Diagnostics is again diagnostics-only: Open Database Folder, Open Backup Folder and Create Manual Backup were removed. Database and backup folders are the same governed storage folder, so one center action replaces both folder buttons. Refresh, integrity check, recalculation and diagnostics export remain in System Diagnostics. Choose Storage Folder remains in File because it changes configuration rather than performing backup or recovery.

Runtime acceptance on 2026-07-22 confirmed the consolidated center, its eight final actions and correct button layout. `Create SQLite Backup` continues to create/list the established `Manual backup` file identity; only its visible button label was simplified. Verification was PASS after the accepted UI run.

## Recovery Compatibility Center

File > Recovery Center inventories every local `.sqlite` backup except the active database and shows file, type, timestamp, schema, integrity, canonical Materials/measurement/Settings counts and compatibility status. Backup types distinguish rotating automatic/migration copies, newly named manual backups, pre-SQLite-restore recovery, pre-Excel-restore recovery and external SQLite files. Recovery snapshots are not consumed by the 20-file automatic rotation.

The standalone recovery boundary is explicit: schema v27 is the first SQLite backup containing canonical native measurements. Schema v26 and older are classified `Legacy / incomplete` because external JSON migration snapshots may be required; newer-than-application and corrupt files are blocked. Current schema v29 is `Ready`. Schema v27-v28 starts as `Migration required` and becomes restore-eligible only after Verify Selected copies the source read-only through SQLite online backup into a unique temp folder, applies current migrations there, verifies integrity/schema and preserves Materials plus native tensile/impact/stiffness counts, then clears pools and deletes the temp tree.

Restore Selected reruns compatibility verification, presents a default-No confirmation and then uses the established verified pre-restore recovery snapshot, atomic replace and controlled restart. The original File > Restore SQLite Backup path now uses the same compatibility policy. Manual backups receive a separate `filamentdb_manual_*` identity and are preserved independently of automatic rotation. Remote Production restore remains unchanged.

Isolated production-backup testing classified schema v26 as blocked, migrated v27→v29 and v28→v29 with integrity and canonical counts preserved, and accepted v29 without migration. The read-only catalog contained 22 local backups. Runtime acceptance then confirmed Recovery Center classification, Verify Selected, separately identified manual backup, guarded same-state restore, automatic restart and Verification PASS.

## Excel Disaster Recovery

`Export Native Excel Recovery Data` now adds a versioned `DR Manifest` and 21 canonical SQLite table payloads to the six readable engineering sheets. Every logical value has an explicit NULL, UTF-8 text or BLOB identity; UTF-8 and binary data use Base64 and long values are split into bounded chunks so full website-template HTML is not truncated by Excel's per-cell limit. Each logical table has a deterministic SHA-256 covering table identity, ordered columns and ordered typed values.

`Restore Excel Disaster Recovery` accepts only the exact governed table set and verifies package identity, format version, schema compatibility, sheet identity, logical row/column counts, chunk completeness and every table hash before showing a default-No confirmation. Restore creates a verified `filamentdb_pre_excel_restore_*` SQLite recovery backup, deletes/inserts all governed rows in foreign-key-safe order inside one transaction, restores the native-measurement canonical marker, and requires foreign-key, integrity and Materials-count checks before commit. It then performs the same controlled restart used by local SQLite restore.

The governed set covers Materials, native measurement inputs/results/notes, general/base-material/deployment settings, manufacturers, website templates, Video Ideas, inventory, purchasing/suppliers/document metadata and the complete experimental hierarchy. Windows Credential Manager passwords and external file contents referenced by path, such as purchase documents and image assets, are intentionally not embedded. SHA-256 detects accidental corruption or editing; it is not a digital signature proving workbook authorship. SQLite remains the preferred first-line backup.

An isolated production-data round-trip exported 21 tables and 8,188 rows, including 200 Materials and long website-template content, into a 1,476,887-byte workbook. After deliberately deleting canonical rows from the isolated database, transactional restore created its SQLite recovery backup and reproduced every table with zero SHA-256 mismatches.

Runtime acceptance on 2026-07-22 produced the expected 29-sheet workbook. Pre-restore verification reported format v1, source schema v29, 21 governed tables, 8,188 rows and 200 Materials. Transactional restore created `filamentdb_pre_excel_restore_20260722_125835_333.sqlite`, passed foreign-key, integrity and Materials-count checks, restarted successfully, and Verification was PASS both before and after restore.

## Local SQLite Backup and Restore

Local SQLite backups now use the SQLite online-backup API and are integrity-checked before they are retained. File > Restore SQLite Backup inspects the selected database read-only, requires a compatible schema and canonical Material rows, and presents schema, size and measurement counts before a default-No confirmation.

Restore creates and verifies a separate `filamentdb_pre_restore_*` recovery database, stages and verifies the selected source, atomically replaces the active database, then verifies the restored file again. Any failure attempts to restore the recovery copy. Successful restore suppresses stale Material close-save behavior and restarts the application so normal schema migrations and startup loading run against the restored state. Remote website Production restore and FTPS behavior are unchanged.

Runtime acceptance on 2026-07-22 restored the newly created `filamentdb_20260722_123610_165.sqlite` backup. Pre-restore inspection reported schema v29, integrity `ok`, 200 Materials, 3,728 tensile samples, 3,752 impact samples and 191 stiffness rows. Restore created `filamentdb_pre_restore_20260722_123743_325.sqlite`, restarted successfully and completed 286/286 Verification PASS with unchanged canonical counts and 5 measurement notes.

The Excel command is renamed `Export Native Excel Recovery Data` and its README now explicitly states that it is a readable secondary export of core engineering data, not a complete SQLite backup. Governed Excel disaster-recovery round-trip remains a separate follow-on milestone and is not claimed by this release.

## SQLite Canonical Working Stores

Schema v29 retires active Materials and general Settings JSON working stores. Production reconciliation found all 200 MaterialIDs and all 78 persisted Material fields exactly equal between `native-materials-manager.json` and `NativeMaterialManagerRows`, with no conflicts or one-sided rows. No `native-settings-manager.json` existed, so the governed built-in defaults safely seed the new normalized `NativeSettingsRows` table. The Settings identity includes Unit because the Impact net cross-section exists legitimately in both mm² and m²; the rejected pre-release three-part key is upgraded in place without discarding rows.

Pre-release runtime testing exposed that the original three-part Settings key caused startup to fail with `UNIQUE constraint failed` immediately after the splash screen. The failed run left an intact schema-v28 database and an empty Settings table. Schema v29 repairs that exact state in place, preserves any rows if present by normalizing null units to blank, and then permits both governed unit variants.

Materials startup/load/save now uses SQLite directly. Legacy Materials JSON is read only as a one-time seed if SQLite is empty; silent validation failure no longer writes a divergent JSON copy. General Settings startup/load/save/reset uses SQLite, while Deployment Settings and Base Material Catalog retain their dedicated SQLite tables. Schema upgrades require a successful pre-upgrade SQLite backup.

Diagnostics now label JSON files as legacy snapshots rather than working copies. Verification requires SQLite Materials row parity, general Settings row/key parity and schema v29. Report metadata, website/publish manifests and other operational JSON artifacts remain intentionally unchanged.

## SQLite Native Measurements

Schema v27 makes native Tensile, Impact and Stiffness input rows canonical in normalized SQLite tables linked to `NativeMaterialManagerRows`. A read-only production reconciliation found JSON to be a conflict-free superset: 3,256 matching plus 472 new tensile samples, 3,332 matching plus 420 new impact samples, and 168 matching plus 23 new stiffness rows. Four tensile notes and the stiffness `[private-material-id-removed]` DNF note are explicitly preserved.

First startup performs one atomic migration only when the canonical marker is absent. A required timestamped SQLite backup must succeed before the transaction begins; sample counts are reconciled inside the transaction before commit. JSON files remain untouched as migration snapshots but are no longer active startup or save sources after migration.

Native grid edits now persist to SQLite. Automatic backups are throttled to one per five-minute editing window so rapid entry does not evict all 20 retained backups. Verification Center compares live UI sample/row counts with SQLite and requires the canonical migration marker.

Runtime acceptance on 2026-07-22 confirmed two consecutive 284/284 Verification PASS runs. The active schema-v27 database passed `PRAGMA integrity_check` with 3,728 tensile samples, 3,752 impact samples, 191 stiffness rows and 5 notes. The required `filamentdb_20260722_114619_301.sqlite` pre-migration backup also passed integrity check at schema v26 with no migration marker. All three measurement JSON timestamps remained unchanged. The second startup retained identical counts without rerunning migration.

## Deployment Settings Governance

SQLite schema v26 owns the FTPS host, port and username shown as governed `Deployment` rows in Settings Manager. Existing installations seed the previously approved `www.iskort.is:21` and `[private-ftps-identity-removed]` values, so publishing remains backwards compatible without silently changing the live endpoint.

Every connection test, Website Test publish, Production publish, rollback and restore receives one immutable endpoint snapshot. The password remains outside SQLite and JSON in Windows Credential Manager; its new credential target is scoped to normalized host plus username, with read-only fallback to the legacy fixed target until the next successful connection test stores it under the governed identity.

Remote production/test activation routes, `/backups`, explicit TLS, passive mode, one remote session, three retries, four local validators and incremental manifest behavior remain locked and unchanged. Verification Center validates SQLite endpoint ownership, endpoint syntax, credential scoping and the existing publishing safety gates.

## Base Material Printing Profiles

Schema v25 moves the working printing baseline to a dedicated SQLite `BaseMaterialCatalog`. One governed 3DPIceland test/G-code baseline can now serve every MaterialID with the same Base Material. It is explicitly not a manufacturer recommendation. Temperature, speed and cooling retain min/recommended/max values; blank remains unknown.

Material Detail now has a focused Printing Profile tab that lists the complete canonical baseline and renders missing evidence as `Not recorded`. Materials remains the editing surface, including controlled profile-kind and provenance choices. Native SQLite persistence, backwards-compatible JSON hydration and Excel `00 Materials` import/export include every field.

Verification checks 0–100 cooling ranges, numeric ordering, drying temperature, absolute source URLs, checked dates, profile round-trip/projection and the unchanged public-report allowlist boundary. Public reports, website rendering and FTPS publishing remain unchanged. Runtime startup, edit/restart persistence, Excel round-trip, Printing Profile review and Verification Center Overall PASS remain required.

The per-MaterialID printing columns were removed from the Materials grid after runtime review showed that the controlled test method reuses one G-code/settings baseline per base-material family. The few entered values were test-only and are intentionally not migrated. Material Detail resolves the baseline by Base Material. Public reports and allowlists remain unchanged. Excel import remains intentionally deferred until the planned backup/restore safety milestone.

The Base Material Catalog is now SQLite-canonical. `native-settings-manager.json` remains only for the older general measurement-settings area and as a one-time backwards-compatible Base Material seed; it no longer overwrites the catalog. A separate roadmap item will migrate remaining canonical JSON working data (general settings and native tensile/impact/stiffness stores) without mixing that broader migration into this release.

Runtime review restored controlled dropdowns for Cooling Guidance, Enclosure and Profile Kind in the Base Material Catalog. The obsolete per-MaterialID Printing Settings card is removed from Material Detail General; the dedicated Printing Profile tab is the sole resolved presentation.
Profile Kind records profile origin with the vendor-neutral choices `Slicer provided`, `Manufacturer provided` and `User provided`; validation status is not conflated with profile origin.

The same review exposed light `.review` cards under dark-screen text in the public Material Engineering Report. The shared screen-only theme now includes `.review` in its dark background/border contract and uses a new marker revision. Light base CSS and PDF/print rendering remain unchanged, and Verification explicitly checks the review-card contrast selector.

## Material Printing Settings Foundation

Canonical MaterialID rows now carry optional nozzle-temperature, bed-temperature and print-speed min/recommended/max values. Units are part of the governed contract: °C, mm/s and hours. Cooling and enclosure use vendor-neutral controlled UI values, while printer and slicer profiles remain free-form references so this foundation is not tied to one manufacturer or profile format.

SQLite schema v23 adds columns without rewriting existing rows. Blank values remain unknown/unrecorded and are not converted to zero. The backwards-compatible JSON working copy is hydrated from SQLite for these canonical fields, and the native Excel `00 Materials` import/export path round-trips them. Material Detail groups them under Printing Settings.

Verification checks numeric parsing, non-negative values, min/recommended/max ordering, model round-trip, Material Detail projection and the unchanged public-report allowlist boundary. Public report models, rendering, website generation and FTPS publishing are unchanged. Runtime acceptance confirmed Material editing, persistence across restart, normal column display and Verification Center Overall PASS.

The first Debug startup attempt failed inside `MainWindow.InitializeComponent` with `NullReferenceException`. The cause was an empty `sys:String` element in each new XAML ComboBox `x:Array`; WPF compiled the BAML but failed while constructing it at runtime. The empty array entries were removed. Runtime re-test confirmed normal startup.

Runtime Materials entry then exposed a navigation delay not present in Impact: `CellEditEnding` queued the complete material recompute, filter rebuild, three measurement-module synchronizations and SQLite autosave at the same dispatcher priority as arrow-key selection, but earlier in the queue. Materials edits now use a 450 ms restartable debounce. Arrow/Tab navigation can activate the next editor immediately, while rapid sequential edits collapse into one governed refresh/save; application close still commits and saves dirty material data synchronously.

Runtime acceptance confirmed the delay is removed, arrow-key column movement is fast and edited values remain present across application restarts.

## Incremental FTPS Publishing

Website Test and Production publishing still construct and locally SHA-256 verify the complete catalog-derived allowlist. A completed deployment manifest now also records the full published route, byte and SHA-256 state. On the next matching publish, the application compares that governed state with the new canonical plan and checks that each candidate remote artifact still exists with the expected byte length. Only new, changed or missing artifacts proceed to remote backup, staging and sequential activation; unchanged artifacts are counted and skipped.

The first publish after this upgrade intentionally remains a full publish because legacy manifests do not contain full published state. A route-set change, unreadable or malformed newest completed state, missing remote artifact, byte mismatch, or a newer Production Restore recovery event also forces a safe full or changed-file path rather than trusting stale state. Test and Production histories remain isolated. Delta backups contain only files changed by that deployment, while `publishedArtifacts` retains the full resulting state for the next comparison. Restore remains backwards compatible with legacy full backups and new delta backups.

Any failed publish invalidates its remote deployment manifest after rollback. If that invalidation cannot be confirmed, the operation is reported as an incomplete rollback rather than allowing a possibly completed-looking manifest to become a later delta baseline.

If every artifact matches, publishing performs no backup, upload or activation and reports a successful no-change deployment. Results show the baseline folder, complete allowlist count, unchanged files skipped, files staged/activated, transferred bytes and timings. The single runtime-proven FTPS transfer session, three retries, complete-transfer barrier, rollback and entry-index-last contracts remain unchanged.

The Production button runs Generate Production first, reloads and rehashes the exact generated plan, tests the encrypted connection and presents a final default-No confirmation. That confirmation distinguishes complete package size from actual post-comparison transfer bytes, explains that only changed/new/missing targets are backed up and staged, and warns that the first v42.12 Production run may be a full fallback because Test and Production baselines are isolated.

Runtime acceptance on the isolated Website Test route used 862 allowlisted files. The first post-upgrade fallback published all 862 files (37,578,896 bytes), with 134.2 seconds of backup/staging and 57.9 seconds of activation. The immediately repeated delta publish used `/backups/website_test_2026-07-22_091758_872` as its baseline, skipped 856 files (99.3%), and transferred/activated only 6 files totaling 1,089,493 bytes. Backup/staging took 0.7 seconds and activation 0.1 seconds. This is a 97.1% byte reduction and approximately 240x improvement for the measured remote staging-plus-activation phase; it is not stated as a whole-workflow speedup because local validation, export generation and remote state comparison are separate phases.

Production runtime acceptance used 861 allowlisted files. The first Production fallback published all 861 files (37,578,526 bytes) into `/backups/website_2026-07-22_093445_629`, with 41.9 seconds of backup/staging and 57.0 seconds of activation. The immediate repeat used that completed Production manifest as its isolated baseline, skipped 855 files (99.3%), and transferred/activated 6 files totaling 1,089,476 bytes into `/backups/website_2026-07-22_093754_428`. Backup/staging took 0.7 seconds and activation 0.1 seconds: a 97.1% byte reduction and approximately 124x improvement for the measured Production remote staging-plus-activation phase. A read-only HTTPS check of `https://www.iskort.is/3dp/index.html` returned HTTP 200, 826,646 bytes, the v42.12 release identity, material content and the explicit reports route.

## Host-Compatible FTPS Staging

Guarded Website Test and Production publishing validate all exact publish-plan artifacts with four bounded parallel local workers before any live FTPS mutation. Remote backup and staging use one runtime-proven explicit-FTPS session, with up to three reconnect-and-retry attempts per artifact. Five- and three-session trials failed on the original 101-port passive range; after the FileZilla Server range was expanded to 5,001 ports and its thread count raised to eight, a two-session trial still produced concurrent upload aborts. One remote session is therefore the accepted safety bound for this FTPS/TLS/network path.

The local validation stage checks existence, byte length and SHA-256 concurrently. The host-compatible remote worker then creates required parent directories inside the retry-protected file operation, backs up existing targets and uploads the isolated staged copy. Only after every allowlisted artifact is represented in the ordered deployment manifest does the control connection resume sequential activation. `/index.html` or `/index-test.html` remains last, and the existing rollback/recovery contracts are unchanged.

Publish results report four local validation workers, the single remote transfer session, backup/staging time and sequential activation time. The application retains real bounded local multitasking without parallelizing the unreliable remote path, activation or rollback. Future transfer-speed work should use manifest-driven delta publishing rather than additional FTPS sessions.

Runtime testing also exposed a transport-close exception after a long successful transfer sequence. FTPS disconnect is now best-effort after a completed and manifest-confirmed deployment, so an already closed socket cannot falsely convert success into rollback. Sequential activation and restore are idempotent and retry-capable: if a remote move succeeds but its response is lost, the next attempt recognizes the missing staged source plus the correctly sized target as success. Remote deletions use the same reconnect-and-verify pattern.

Failure messages identify the guarded publish phase (`host-compatible backup and staging`, manifest creation, sequential activation or manifest completion), while still reporting whether rollback completed. This keeps a transient host-side connection closure diagnosable without weakening the rollback gate.

## Guarded Public Report FTPS Deployment

`Publish Website` now starts by running the same confirmed Generate Production workflow, including automatic data-fresh rebuilding of all six public report types and creation of the exact catalog-derived `website-publish-plan.json`. The plan is then loaded back from disk and every allowlisted artifact is checked for the current release identity, byte length and SHA-256 before an FTPS connection is made. A second explicit confirmation shows the public MaterialID count, catalog entries, exact file count and total bytes before any live change.

The complete allowlist is transferred over explicit TLS into a timestamped `/backups/website_*` tree. Existing targets are preserved under `original/`; all new artifacts are uploaded and size-verified under `staged/` before activation begins. Activation follows the plan order with the root `/index.html` last. Every activated target is size-verified. If activation fails, replaced targets are restored from their remote backups and newly introduced targets are removed; incomplete rollback is reported distinctly with the retained backup path. The older three-file publisher remains available internally for backwards compatibility, while the website button uses only the new guarded plan flow.

Verification remains offline and never connects to or mutates the production server. Runtime acceptance requires the operator to run Verification Center and then deliberately confirm the live publish from the application.

`Publish Website Test` provides the same automatic Preview generation, source-fresh report rebuilding, SHA-256 allowlist validation, remote backup, complete staging, size checks and rollback protection without touching Production routes. All test content is isolated under `/preview/`; only the browser entry `/index-test.html` sits at the FTPS root and it is activated last. The test contract explicitly rejects `/index.html`, `/reports/` and `/manufacturers/index.html`. This gives the operator a stable `https://iskort.is/3dp/index-test.html` browser shortcut for server-level validation before choosing `Publish Website Production`.

Every newly completed guarded deployment backup now contains `deployment-backup-manifest.json`. It records mode, completion state, ordered target routes, whether each target existed before deployment, original byte length and original SHA-256. Incomplete or legacy manifest-free folders and Website Test backups are never offered by `Restore Last Production Backup`.

Production restore discovers the newest eligible completed backup and presents its timestamp, target count and previous byte total before confirmation. It first captures the current live state into a new `/backups/website_revert_*` recovery folder, copies and verifies every selected original into restore staging, removes targets that did not exist before the reverted deployment and restores `/index.html` last. If restore fails, the recovery snapshot is applied in reverse order; incomplete recovery is reported separately with the retained recovery path. Source backups are copied, never consumed.

## Production Publish Readiness

Generate Production now completes the full local handoff required before public-report FTPS integration. After ensuring the data-fresh six-type report package and staging stable Production routes, it builds `website-publish-plan.json` from the canonical staged report catalog. Only the root website, manufacturer redirect, methodology whitepaper, production report portal, catalog/manifest/fingerprint, catalog-owned report directories and shared report assets enter the plan.

Every planned file must exist, be non-empty and remain inside the selected website root. The plan records its exact local path, safe remote route, byte length and SHA-256. Preview routes, backups and unrelated folders are excluded, duplicate remote routes fail verification, and `/index.html` is deliberately ordered last for activation.

All public website HTML buttons now target explicit `/index.html` files instead of relying on web-server directory-index resolution. This keeps the same stable report directories on Production while making Preview and Production links work when opened directly from the local filesystem without a development web server. PDF buttons remain explicit `report.pdf` links.

## Public Report Data Freshness

The public package ensure pipeline now computes a deterministic SHA-256 source fingerprint from the current public MaterialID selection, the report-relevant canonical SQLite rows and the exact current Verified Material Summary/report projection consumed by the renderers. This includes the active tensile, impact and stiffness inputs, so adding another measurement changes the fingerprint even before any unrelated package file changes. The package stores only the hash, schema, public material count and generation timestamp in `source-fingerprint.json`; raw measurement, projection or operational values are not copied into that file.

Preview, Production and Build Public Report Package compare the current fingerprint with the accepted package revision. A missing or changed fingerprint marks the report set data-stale and rebuilds all six report types before the package is refreshed and staged. This deliberately favors correctness across dataset-level ranks, averages, comparisons, manufacturer portfolios and the Material Summary; a measurement on one material can affect context shown by several reports. When the fingerprint still matches, the existing fast validation-only path remains active.

## Automatic Website Report Prerequisites

Generate Preview and Generate Production no longer require the operator to build the public report package first. Both actions call the same async package prerequisite used by the manual Build Public Report Package action. The shared flow derives the expected six-type catalog from the current public MaterialID selection, rebuilds only missing or presentation-stale report types, verifies every HTML/PDF/metadata artifact, refreshes the package index/manifest/catalog and then stages the website portal.

Production retains its explicit confirmation and local backup behavior. Preview remains local-only. FTPS publication and its explicit file allowlist are unchanged; this milestone does not upload public reports.

## Public Website Report Portal

Website Preview and Production now consume the accepted public report catalog through one shared renderer. The local website export validates the complete six-type package, stages its canonical HTML/PDF/metadata/assets under the website `reports/` tree, and writes `reports/index-test.html` for Preview or the stable `reports/index.html` route for Production. Every catalog-linked artifact must exist and be non-empty before website export can proceed.

The main portal adds an Engineering Reports navigation action and dataset-level entry card. Opted-in MaterialIDs receive contextual Material Engineering, PDF, Printing Recommendation, Test Session, Material Family Comparison and Manufacturer Report actions; private MaterialIDs receive none. Manufacturer cards link their public report when at least one included material is opted in. The website export manifest records the staged portal and catalog counts. FTPS remains deliberately unchanged: the staged report tree is not part of the guarded Publish Website upload list.

The material-level directory presents the full public material name as its primary label and retains MaterialID as smaller traceability metadata. Stable routes and catalog identity continue to use canonical MaterialID.

All six public report types and the report portal now share one screen-only dark theme aligned with the main website. The existing light report CSS remains the canonical base and the dark overrides are restricted to `@media screen`, so WebView2 PDF output and paper printing remain light without a second renderer or duplicated report content.

The Material Engineering radar has dedicated dark-screen contrast for its grid, axes, labels and three comparison profiles. These overrides are presentation-only; the light PDF radar remains unchanged.

The one-click package validator treats existing report HTML without the current shared screen-theme marker as presentation-stale. It automatically rebuilds the affected report types and PDFs, so website staging cannot silently retain an older light-only HTML package.

## Public Report PDF Layout Parity

Comparison, Manufacturer and Material Summary reports now have explicit canonical print CSS that preserves their desktop header, summary-card and chart-grid relationships. This prevents the A4 print viewport from activating the narrow-screen layout that previously stacked cards and charts in PDF while HTML remained wide.

Those three wide portfolio report types print in A4 landscape so their engineering tables remain visible instead of clipping right-hand columns. Per-MaterialID Material Engineering, Test Session and Printing Recommendation reports remain portrait. HTML is still canonical, PDF is still printed from that same HTML, and no report data, allowlist, route or FTPS behavior changed.

## Public Report Bounded Multitasking

The public package pipeline now performs real bounded multitasking: up to four thread-pool workers validate the existence and non-zero length of the complete HTML/PDF/metadata artifact catalog concurrently. Work is limited to immutable relative routes and read-only filesystem checks, so SQLite ownership, report models and WPF/WebView2 thread affinity remain unchanged.

PDF rendering still uses the reusable single STA-safe WebView2 host, but the fixed 350 ms delay after every navigation is replaced by bounded polling of document, font and image readiness plus a short layout-settle delay. The package log explicitly reports the four-worker artifact validation contract. This is a measured, limited parallel extension rather than an unsafe parallel rewrite of the print engine.

## Public Report Batch Performance

The clean-folder one-click package workflow now opens one hidden WebView2 print host for the entire missing-report sequence and reuses it for every canonical HTML-to-PDF operation. Individual report buttons retain the existing isolated one-PDF host behavior. The batch remains sequential at the WebView2/STA print boundary, preserving WPF thread affinity and deterministic PDF parity while removing repeated browser/window initialization for every artifact.

The package log records total elapsed time and per-report-type timings. This establishes evidence for any later bounded two-worker experiment without introducing unmeasured concurrent WebView2 instances. HTML, metadata, allowlists, routes and report calculations are unchanged.

## Public Engineering Report Package

`Build Public Report Package` is the one-action public portfolio workflow. It detects missing artifacts by report type, invokes only the required existing canonical report builders, waits for them to finish, and then verifies the complete portfolio before writing `public-report-preview/index.html`, root `manifest.txt`, `report-catalog.json` and shared JPG branding. A clean output folder is therefore supported. The package requires all six accepted public report types: Material Summary, Material Engineering, Comparison, Manufacturer, Test Session and Printing Recommendation.

The package does not copy, merge or recalculate report data. It orchestrates the accepted builders and catalogs their stable HTML/PDF/metadata artifacts. If an invoked builder still cannot produce an expected artifact, the package fails with the remaining exact paths. The root index presents portfolio counts, dataset summary, comparison and manufacturer directories, and MaterialID-level Engineering/Recommendation/Test links. Rebuilding Material Engineering previews writes their batch index to `materials.html` and preserves an existing v42.8 portfolio root. FTPS and website navigation remain unchanged.

## Public Material Summary Report

Public Material Summary publishes the accepted REPORT-110 dataset view at the stable local-preview route `reports/material-summary/`. Its scope is every active MaterialID explicitly checked `Public reports`. The public report preserves the internal coverage cards, native tensile/impact/stiffness and score-availability table, material-type and manufacturer distributions, and the full MaterialID-level six-score ledger.

The dedicated typed allowlist excludes purchasing, inventory, supplier, path, credential, raw specimen and internal-note fields. Each ledger row links to the existing public Material Engineering, Printing Recommendation and Test Session routes. HTML is canonical, PDF is printed from that HTML, and FTPS remains deferred.

## Public Printing Recommendation Report

Public Printing Recommendation Report creates stable per-MaterialID local-preview routes with full REPORT-150 applications, strengths, limitations, trade-offs, six-axis profile, workflow checks, decision guidance and stronger public same-family alternatives. Exact printer settings remain `Not recorded` until canonical Material Printing Profiles exist; the renderer never infers them. FTPS remains deferred.

## Public Test Session Report

Public Test Session Report creates stable `reports/test-sessions/{MaterialID}/` local-preview routes for every active MaterialID checked `Public reports`. The public-safe baseline retains REPORT-140 result-quality depth: module/specimen coverage, averages, standard deviation, CV, confidence, validation, method/equipment context and honest missing-provenance disclosure. Batch/lot and operational fields are excluded.

An additive SQLite-backed `Public test details` checkbox defaults false. Only explicitly approved MaterialIDs expose recorded raw tensile/impact/stiffness inputs and reviewed module test notes; otherwise those collections never enter the public DTO and the report states the detail boundary. Verification probes both aggregate-only and approved-detail contracts. FTPS remains deferred.

## Public Manufacturer Report

Public-report parity is now a standing platform rule: an accepted internal engineering report is the canonical content baseline for its public counterpart. Public output retains the available engineering data, comparisons, charts, rankings, coverage and limitations, while explicit internal, operational and sensitive fields are removed through typed allowlists. Public reports are not intentionally reduced to summary-only editions.

A parity audit covered all three current public reports. Material Engineering already retained the governed measurements, score/radar context, interpretation, rankings, alternatives and peers. Manufacturer parity was expanded to the full public portfolio/global/category and six-axis product context. Comparison parity now also retains the internal coverage summary cards and separate materials/evidence context table in addition to leaders, four charts and side-by-side scores. Verification Center has a combined internal-content parity gate across all three publishers.

Public Manufacturer Report batch preview creates stable `reports/manufacturers/{slug}/` routes for every manufacturer with at least one active MaterialID explicitly checked `Public reports`. Portfolio counts, averages, leaders, category positions and charts use only that public projection. The dedicated allowlist excludes purchasing, inventory, supplier URLs, local paths, raw specimen rows and internal notes.

Each manufacturer receives canonical HTML, PDF printed from that HTML, metadata, manifest and canonical JPG branding. Public presentation intentionally retains the accepted REPORT-130 depth: full coverage cards, public global position, MSRP/video availability, category positions, overall/consistency charts and a six-axis product-level engineering table. Public safety removes sensitive fields rather than reducing engineering usefulness. Product links use only approved manufacturer/product fields; `Supplier URL` is never a public fallback. FTPS integration remains deferred.

## Public Comparison Report

The accepted internal REPORT-120 contract now has a separate public publishing projection. Local preview builds stable `reports/comparisons/material-family-{slug}/` routes for base-material families containing at least two active MaterialIDs with explicit `Public reports` opt-in. The public renderer receives only its dedicated 18-field allowlist and existing Verified Material Summary score outputs; it does not receive internal report rows or calculate measurements and scores.

Each preset contains canonical `index.html`, PDF printed from that HTML, metadata, manifest and the canonical JPG logo. The accepted REPORT-120 visual comparison is preserved through leaders plus responsive Overall, Tensile, Impact and Stiffness bar-chart panels sourced from the same governed scores. Verification checks the stable route, unique MaterialID membership, allowlist, sensitive-field exclusion, all four charts, report content and local preview action. FTPS integration remains intentionally deferred.

## Public Engineering Report Content Expansion

The public Material Engineering Report now carries the substantive governed content already available to the accepted internal report: Verified Material Summary measurement averages, standard deviation, CV, sample count and confidence; stiffness modulus/deflection; engineering score bars and six-axis radar comparing the selected material with material-family and manufacturer averages; per-metric ranks/percentiles; decision guidance; stronger same-family alternatives; engineering interpretation; strengths, limitations, trade-offs, recommended applications and peer context. These values are prepared by the existing calculation, Engineering Intelligence and ranking layers and passed through an expanded 38-field public allowlist; the public renderer only converts existing scores into SVG coordinates and performs no measurement or score calculation. Internal shell diagnostics such as `Unified HTML report engine` and `Materials in database` are intentionally absent.

Canonical public HTML now displays `assets/3dp-iceland-labs-logo-pdf.jpg`, which is already copied into every public report package and therefore also appears in the PDF printed from that HTML. Radar axis and label coordinates use invariant SVG number formatting so Icelandic decimal commas cannot split labels or extend axis lines, and the viewport includes enough left margin for the full `Consistency` and `Layer adhesion` labels; Verification has a dedicated radar-layout check. Verification also adds rich-content and JPEG-branding checks plus the v42.2.1 release gate. Runtime acceptance confirmed the expanded public report, canonical HTML/PDF presentation, complete radar labels and all-PASS Verification. Release build completes with zero warnings and zero errors.

## Canonical Public Material Selection

Materials now exposes an explicit `Public reports` checkbox for each canonical MaterialID. The opt-in is stored as `PublishPublicReports` in SQLite with a backwards-compatible default of false; existing databases migrate in place and no material is selected automatically. The legacy JSON working copy remains readable, but SQLite overrides publication intent during load.

The shared Preview/Production website renderer emits stable `reports/materials/{MaterialID}/` HTML and PDF links only for selected materials. Unselected materials remain available in the existing engineering charts and product/video link surfaces but receive no public report links. The selection does not upload anything and the guarded FTPS workflow is unchanged.

Verification covers checkbox/UI availability, canonical record round-trip, selected-versus-unselected URL behavior, shared website rendering and the v42.2 release gate. Debug and Release builds complete with zero warnings and zero errors.

The first UI run showed that the existing shared first-click DataGrid editor consumed checkbox clicks before WPF could toggle them. Native Materials boolean columns now toggle directly on the first click, update the visible checkbox immediately and enter the normal coalesced auto-save path.

The next runtime pass showed that the original v42.1 button still built only the currently selected row even when several materials were opted in. `Build Selected Public Reports` now resolves every active checked MaterialID before writing, verifies every allowlisted model, creates one independent canonical HTML/PDF package per material and writes a combined preview index containing all selected reports.

Runtime acceptance confirmed that publication selections persist across restart, two opted-in materials produce two independent report packages, the combined `index.html` links to both, and Verification Center reports all checks PASS. v42.2.0 is accepted; public report FTPS publishing remains deferred.

## Public Report Publishing Foundation

The Reports tab now provides `Build Public Report Preview` for the selected material. It writes a local static website package under `public-report-preview/reports/materials/{MaterialID}/`, including canonical HTML, a PDF printed from that HTML, public JSON metadata, a manifest and assets. The preview is never uploaded automatically.

A dedicated 21-field public model is the security boundary. Only approved material identity, public links, canonical MSRP and existing Verified Material Summary/governed score outputs reach the public renderer. Purchasing, operational stock, credentials, device filesystem locations, raw specimen rows and internal notes are not supplied. Verification checks routing, allowlist enforcement, sensitive-field exclusion, methodology links, artifact parity and the v42.1 release gate.

Runtime acceptance confirmed that the selected-material public preview builds successfully with the canonical HTML/PDF package and that Verification Center reports all checks PASS after the deferred measurement-tab warm-up receives time to complete. Build Solution also completed successfully. v42.1.0 is accepted as a local-only publishing foundation; production FTPS publishing remains intentionally disabled.

## Deferred Measurement Tab Warm-up

Tensile, Impact and Stiffness now perform their first WPF visual-tree/DataGrid layout automatically after the Materials view is visible and higher-priority startup work has finished. The selected workspace is restored inside the same Dispatcher callback, preventing visible tab switching.

Startup Diagnostics records an independent first-use warm-up time for each measurement tab. Verification requires all three warm-up phases and the prior v41.8 startup gates. No measurement data, calculations or SQLite ownership changed.

Runtime acceptance confirmed Materials remained visible after about 5 seconds with Verification PASS. The accepted Debug trace rendered Materials at 4.47 seconds and completed Tensile, Impact and Stiffness warm-up at 8.00 seconds.

## Startup Refresh Coalescing

The first measured Debug trace found that MainWindow construction took about 1.0 seconds but the app then waited roughly 17 seconds before showing the window. Bulk loading 200 Materials had queued approximately 201 copies of the same downstream UI refresh.

Bulk replacement now schedules one measured consolidated refresh. Materials filters, Inventory choices and summary, measurement identity synchronization and deferred engineering intelligence still use their existing implementations. No concurrency or data-source change is included.

User runtime acceptance confirmed Verification PASS and reduced the observed Visual Studio startup to the visible Materials list from about 19-20 seconds to about 5 seconds. The accepted diagnostic trace reached first usable Materials rendering at 4.49 seconds from instrumentation start.

## Startup Performance Instrumentation

This profiling-only build records the full startup path without changing its behavior. System Diagnostics now shows ordered timestamps and phase durations for splash rendering, MainWindow/XAML construction, canonical Materials loading, every major secondary workspace, the Loaded workflow, first usable Materials rendering and deferred engineering intelligence.

Use the trace to compare Visual Studio Debug startup separately from cold and warm Release EXE runs. Verification checks that the core markers are present and that the v41.8.0 release identity is aligned. Concurrency, lazy tabs and startup reordering remain intentionally deferred until the measurements identify the actual bottlenecks.

Debug and Release builds complete with zero warnings and zero errors. v41.8.0 instrumentation and the v41.8.1 coalescing result are runtime accepted with Verification PASS.

## Combined Engineering Report Package

The Reports tab now exposes `Export Engineering Package`. It exports the six accepted engineering reports into one timestamped parent folder with stable numbered subfolders:

1. Material Summary
2. Material Engineering
3. Comparison
4. Manufacturer
5. Test Session
6. Printing Recommendation

Every subfolder contains canonical HTML, a PDF printed from that exact HTML, text, metadata, manifest and assets. The parent folder contains `index.html`, `manifest.txt` and `package-metadata.json`. Existing folders are never overwritten; a numeric suffix is added if a same-second package already exists.

Selected and all-visible scope behavior remains owned by each individual accepted report contract. The package layer only orchestrates exports and does not merge or recalculate report data.

Verification checks the exact six-report set, stable folders, index links, manifest, metadata, export button and the aggregate v41.7.8 release gate. Debug and Release builds complete with zero warnings and zero errors. User acceptance confirmed normal package export and Verification PASS; v41.7 is closed.

## Report Portfolio - Printing Recommendation

`REPORT-150` is a distinct Printing Recommendation Report over existing governed engineering profiles, Verified Material Summary coverage and canonical MSRP. No measurement or score is recalculated.

Selected scope provides application guidance, measured strengths, limitations, engineering trade-offs, decision guidance, print-workflow checks and stronger same-family alternatives. All-visible scope provides a recommendation ledger following the exact current Materials search/filter result.

The report deliberately contains no Video Planner or YouTube content. It does not invent nozzle temperature, bed temperature, speed, cooling, drying or enclosure values; exact settings remain a manufacturer/printer validation step. MaterialID and MSRP now remain present through the shared ranking projection.

Verification checks REPORT-150 identity, selected guidance, all-visible ledger behavior, settings honesty, exclusion of content-planning hooks and the aggregate v41.7.7 release gate. Debug and Release builds complete with zero warnings and zero errors. User visual review and Verification PASS are confirmed; REPORT-150 is closed.

## Report Portfolio - Test Session

`REPORT-140` is a distinct Test Session Report over native MaterialID-linked tensile, impact and stiffness records plus existing Verified Material Summary results. The report does not recalculate measurements.

Selected scope provides detailed traceability: specimen/result counts, average, standard deviation, CV, confidence/completeness, validation status, recorded native inputs and module notes. All-visible scope provides a compact ledger for the exact current Materials search/filter result.

Method/equipment context uses the same native Settings Manager constants consumed by ResultsService. The report also declares the current provenance limitation: SessionID, test timestamp, operator, printer/slicer profile and environmental conditions are not stored in the canonical test schema and are therefore shown as `Not recorded`, never inferred.

Verification checks REPORT-140 identity, selected detail, all-visible ledger behavior, missing-metadata honesty and the aggregate v41.7.6 release gate. Debug and Release builds complete with zero warnings and zero errors. User visual review and Verification PASS are confirmed; REPORT-140 is closed.

## Report Portfolio - Manufacturer

`REPORT-130` is a distinct Manufacturer Report over canonical active/visible Materials projections and existing Verified Material Summary outputs. The report layer does not calculate measurements or introduce a parallel score source.

With `Selected Material Only`, the selected MaterialID identifies a manufacturer and the report expands to that manufacturer's complete active portfolio, even when the Materials view is currently narrowed to one product. With `All Visible Materials`, the exact current Materials search/filter scope is preserved and grouped into one or multiple manufacturer sections.

The selected-scope report names the source material and highlights its row in the expanded portfolio. Selected and all-visible reports can legitimately contain the same materials when the current Materials filter already selects exactly that manufacturer; the scope explanation distinguishes those paths.

The HTML/PDF report includes portfolio/test/profile coverage, product-line and material-type breadth, MSRP/video availability, existing average score and strongest-axis context, global manufacturer positioning, category position by base material, safe product/video links and a full product-level engineering table. Missing evidence remains `n/a`.

The product table labels its five-axis availability as `Engineering axes`, avoiding the ambiguous impression that a value such as 5/5 represents five specimens or measurements.

Verification checks REPORT-130 identity, selected-manufacturer expansion, exclusion of unrelated manufacturers, all-visible multi-manufacturer behavior and the aggregate v41.7.5 release gate. Debug and Release builds complete with zero warnings and zero errors. User visual review and Verification PASS are confirmed; REPORT-130 is closed.

## Concise Report Package Naming

Export Current Report now creates folders using `report-name-yyyyMMdd-HHmmss`. Comparison Report therefore exports to a name such as `comparison-report-20260721-231416`, without repeated platform, key, PDF and title segments. The HTML/PDF/text/metadata/manifest/assets package contract is unchanged.

Verification checks the exact folder-name contract. Release build completes with zero warnings and zero errors, and the concise naming change is accepted.

## Report Portfolio - Comparison

`REPORT-120` is a distinct Comparison Report over the canonical native Materials projection and existing Verified Material Summary scores. It never calculates measurements in the report layer.

With `Selected Material Only`, the selected material is the highlighted anchor and up to five peers are taken from the current visible Materials scope. Same-base-material peers come first, followed by closest available overall-score context. With `All Visible Materials`, every canonical visible material is included and missing values remain `n/a`.

The HTML/PDF report includes scope explanation, comparison-set coverage, leaders by engineering axis, overall/tensile/impact/stiffness charts, a side-by-side score/evidence/MSRP table and methodology links. Overall deltas are explicitly comparative context rather than statistical confidence or application certification.

Verification checks REPORT-120 identity and both report scopes. User visual review confirmed the report presentation and Verification Center passes; v41.7.3 is closed.

## Canonical Material Projection

The legacy `_materialsView` material universe has been removed. Native SQLite-backed Materials records and MaterialID now govern active, archived, visible and selected material scope throughout the application.

All user-facing engineering consumers now receive either the current filtered native Materials projection or the full active native projection. This includes analytics, rankings, category rankings, awards, Video Planner, recommendations, dashboards, YouTube Research, AI collections/sessions, reports and website export. Existing DataRow-based services receive transient adapters built from native rows, preserving compatibility without retaining a second material list.

The hidden legacy import-cache tab and its filters, search, grid selection and fallback counts no longer exist. Imported workbook material tables are bounded to ingestion/transition workflows and cannot become the runtime material scope.

Canonical secondary-filter and intelligence refreshes are deferred until after the main window is visible. This keeps the splash-screen phase limited to core data/workspace initialization and coalesces downstream refreshes in the background.

Verification Center now checks unique MaterialID parity for active and visible projections, validates that every visible row belongs to the active native set and confirms that the legacy workspace tab is absent. Debug and Release builds complete with zero warnings and zero errors. User acceptance confirmed Verification PASS, normal reviewed-tab behavior and correct Materials-filter propagation; v41.7.2 is closed.

## Report Portfolio - Material Summary

The first v41.7 report increment establishes the canonical report scope and completes Material Summary as a distinct report. `All Visible Materials` now consumes the same filtered native Materials view shown to the user, rather than the older imported projection. With no active filters it therefore uses all 200 active materials in the current database; active search/filter choices reduce that scope predictably.

`REPORT-110` presents material identity, manufacturer and material-type coverage, verified engineering-axis coverage, complete-profile count and high-level score values without recalculating measurements. Missing verified evidence remains `n/a`.

The accepted refinement separates complete, partial and no-verified-evidence profiles, records active Materials-tab search/filter values, expands selected-material identity and links the public methodology and whitepaper. Duplicate scope totals and internal `Canonical total` wording were removed.

Verified-result wording now explains the accepted engineering-result boundary in plain language. Valid video-review URLs are rendered as safe clickable HTTP(S) links in selected and multi-material summaries.

REPORT-110 test coverage is now counted from native Verified Material Summary modules, not from whether the legacy score projection happened to contain an axis. Fully tested means tensile, impact and stiffness are all present; partial means one or two modules are present. Native summaries also feed the existing score formula for the report table, and Verification checks exact parity with native Materials test flags.

User acceptance confirmed parity with the Excel export and a clean Verification Center run. REPORT-110 is closed. The next build is the v41.7.2 application-wide removal of `_materialsView` as a material source.

`Refresh Preview` shows at most ten material lines and states that limitation. `Export Current Report` always creates canonical HTML plus its print-matched PDF, text summary, metadata, manifest and assets. The future Engineering Report Package will be enabled after all six reports above the selector separator have passed individual visual acceptance.

Release build completes with 0 warnings and 0 errors. Verification Center and visual acceptance are pending before work starts on Comparison Report.

## Internal Repeatability Calibration

One canonical service now owns the 3DPIceland internal repeatability score and its user-facing labels across the app, Engineering Advisor, reports, website and whitepaper. The established `100 - average CV% - sample penalty` calculation is unchanged, preserving existing values and rankings.

Score labels are 90–100 Excellent, 85–89.9 Very good, 80–84.9 Good, 70–79.9 Moderate, 60–69.9 Low and below 60 Very low repeatability. Individual measurement sets reach the internal review boundary at 30% CV and the high-variation boundary at 40% CV.

These are internal comparative bands, not an industry standard or accredited measurement uncertainty. The methodology now records the known impact-pointer, tensile low-force and stiffness-angle limitations and adds matching pre-session checks.

Acceptance testing found that Selected Material Engineering Reports retained the numerical consistency score but did not pass the matching Verified Material Summary into the governed intelligence handoff. The report path now builds the canonical summary map once, reuses it for the selected material and its context rows, and Verification Center checks that the summary survives into repeatability analysis.

Comparison of high- and low-consistency exports then exposed two scores in the low-coverage report: the canonical profile included its established incomplete-sample penalty while repeatability context recalculated only the available sets. Engineering Consistency now accepts the existing profile score as canonical for its label and displayed score while continuing to source CV, sample coverage and outlier review from Verified Material Summary. Existing website rankings therefore remain unchanged.

Manufacturer-facing report review also removed obsolete and editorial-only content. The report header now records the current platform version instead of the v36 milestone name; the final Video Planner hook has been removed; radar lines explicitly identify material-family and manufacturer averages; and the former AI heading now states that the review is generated locally by deterministic rules with no external AI or LLM.

Debug and Release builds complete with 0 warnings and 0 errors. User acceptance confirmed the updated Material Engineering Report presentation and a clean Verification Center run. Differentiating the remaining legacy report choices is queued as v41.7.

## Governed Intelligence Handoffs

Material Engineering Reports, the methodology whitepaper and recommendation-created Video Planner ideas now consume one shared Engineering Intelligence handoff. It composes the advisor, repeatability, price/inventory/manufacturer context, peer position and alternatives that their existing services have already produced; it does not calculate measurements or score axes.

The canonical HTML report contains the governance statement, so PDF receives exactly the same content through the established HTML print path. The whitepaper documents the source boundary without embedding changing per-material rankings.

Video Planner ideas retain canonical MaterialID and the existing EngineeringScoreProfile axes. The queue migration is additive and preserves older rows, whose MaterialID remains empty until they are recreated or relinked.

Verification Center covers the shared composition, report payload, whitepaper section, persistent video-planning handoff and aggregate v41.5 release gate.

Acceptance review found that Selected Material Only still prioritized a retained Native Materials grid selection over the material currently displayed in Material Detail. Report selection now uses the displayed row as canonical, shows its name beside the scope selector and refreshes the selected-material preview when that row changes. Canonical report HTML also embeds the approved JPG logo used by the PDF/whitepaper path instead of the obsolete PNG asset.

Debug and Release builds complete with 0 warnings and 0 errors. The in-app Verification Center run and visual review remain the user acceptance gate.

## Manufacturer & Category Positioning

Recommendation Detail now compares the selected recommendation with same-manufacturer and same-category peers in the active dataset. It shows rank, peer count, overall score and the group average while preserving the current recommendation filters as the comparison boundary.

The purple Selected Material Intelligence card exposes the same positioning for the active MaterialID independently of the global winner lists.

The service consumes existing EngineeringScoreProfile values and canonical MaterialIDs only. It does not calculate raw test values or create a parallel score model. Missing classifications and score evidence are shown explicitly.

Verification Center covers manufacturer/category rank, peer counts, missing-data honesty, UI availability and the aggregate v41.4 release gate. Debug and Release builds complete with 0 warnings and 0 errors; user acceptance is confirmed.

## Price, Inventory & Manufacturer Context

Recommendation Detail now shows canonical MSRP, current inventory availability and active manufacturer context beside the Engineering Advisor output. Inventory values come from `InventoryEngineService`; the context layer does not recalculate spool quantities, remaining weight or cost.

The reusable ChatGPT prompt carries the same governed context. Missing price, inventory links or manufacturer records remain explicit rather than inferred.

Canonical MSRP never falls back to landed cost or a stale legacy projection when a native Materials record exists. Clearing MSRP in Materials therefore produces an explicit unavailable state throughout recommendation context.

The UI calls this value `Public MSRP reference`; `canonical` remains an internal architecture term rather than user-facing language.

Verification Center covers source ownership, deterministic interpretation, UI presence and the aggregate v41.3 release gate. Debug and Release builds complete with 0 warnings and 0 errors; user acceptance is confirmed.

## Consistency & Outlier Intelligence

Recommendation Detail now interprets repeatability directly from Verified Material Summary CV and sample-count outputs. It reports how many tensile/impact orientation sets contain CV evidence, average and highest CV, and how many sets have at least five valid specimens.

Selected Material Intelligence now follows the active MaterialID above the recommendation rankings. This exposes the chosen material even when it is not a global winner, while the existing performance and application lists remain separate and are labelled as global rankings.

The displayed Material Detail row is the canonical selected-material UI context. A stale selection retained by another materials grid can no longer overwrite the card during recommendation refresh.

Documented CV bands are used only to identify variation that deserves review. The platform does not infer or remove an individual specimen outlier from aggregate statistics; raw samples, failure notes and a test-specific reason remain authoritative. Missing repeatability evidence is shown explicitly rather than converted into confidence.

The same governed context is included in the reusable ChatGPT prompt. Verification Center covers strong repeatability, high-variation review, insufficient evidence, selected-material binding, the Verified Material Summary calculation boundary, UI availability and the aggregate v41.2 gate.

Debug and Release builds complete with 0 warnings and 0 errors. The user confirmed a clean in-app Verification Center run on 2026-07-21.

## Comparable Alternatives & Hidden Gems

The Recommendation Engine now turns each selected result into a small decision set. It finds the closest comparable profile, a price-aware hidden gem when one exists, and a specialist alternative with a clear engineering-axis advantage.

All candidates come from the active filtered recommendation group. Existing recommendation scores remain canonical, while MSRP USD/kg is read from the same native material pricing record used by Pricing & Value. The alternatives service compares these governed outputs and does not calculate raw measurements or create a parallel scoring model.

Recommendation Detail shows type, material, score, MSRP, reason, gain and trade-off. Missing pricing is displayed honestly as unavailable. Five new Verification Center checks cover selection exclusion, recommendation-context isolation, price/value behavior, specialist trade-offs, UI and the aggregate v41.1 gate.

The obsolete fixed Yasin Playlist Discovery prototype has been removed from Recommendation Detail. The separate Playlist Discovery surface under YouTube Research remains the only canonical implementation and continues to derive its candidates from live material and video-coverage data.

Cached material views load before the native material manager during startup. A coalesced post-load refresh now rebuilds Recommendation, Video Planner and YouTube Research after canonical pricing is hydrated, so MSRP is visible immediately without touching a filter. The same refresh follows native pricing edits and undo operations.

## Advisor Locale Verification Fix

The v41.0.0 advisor output was correct, but its comparison verification inspected English-formatted text. On an Icelandic system the same values are displayed with decimal commas. v41.0.1 verifies typed comparison deltas and axis identities instead, preserving localized UI text and a culture-independent release gate.

## Explainable Engineering Advisor

v41 begins with a deterministic advisor integrated into the existing Recommendation Engine. It reads the established EngineeringScoreProfile only; native test calculations and normalized axes remain owned by their existing services.

For every recommendation, the detail panel now identifies the strongest available evidence, lowest available axis, missing axes, evidence coverage and consistency context. It also compares the row with the nearest ranked alternative and reports both recommendation-score difference and the clearest axis lead or trade-off.

Evidence coverage is deliberately labelled as an advisor indicator rather than statistical confidence. Partial profiles remain usable but explicitly disclose missing evidence. The copy/paste ChatGPT prompt carries the same deterministic context, while direct API integration remains optional and outside the canonical decision path.

Verification Center adds four advisor contracts plus a v41 release gate. Debug and Release builds complete with 0 warnings and 0 errors; the in-app Verification Center run remains the user acceptance gate.

## Pricing Filter Synchronization Fix

Pricing & Value remains a mirror of the canonical Filament Database filter state. The bridge now emits the `input` event consumed by the established filter engine, so selections made from Pricing immediately update the shared dataset and charts. Its multi-select controls also use the same click-to-toggle interaction as Database, preserving several selected values without Ctrl/Cmd and allowing a selected option to be deselected with another click.

## Platform Integration & Release Readiness

v40.20.0 is the local completion build for the v40 platform cycle. Verification Center now owns an aggregate release gate instead of relying on stale informational PASS entries. The gate combines Engineering, Experimental, Website, Reporting, workspace order and release identity.

Website Preview and Production render through the same active SQLite template and canonical portal path. Their complete HTML output must match after removing only the generated mode header. The export contract also verifies the five portal routes, mode-aware manufacturer redirects and the main HTML/redirect/whitepaper package manifests.

The production manifest now records the methodology whitepaper path. Live explicit-FTPS transfer is still pending the external passive-port opening and is not included as a local software PASS.

The first user Verification Center run passed 162 of 165 checks while the exported website remained visually correct. The two direct failures were stale verification assumptions introduced by later portal extensions: the Pricing compact row carries two CSS classes, and a governed terminology marker precedes the generated Preview/Production header. The checks now match those exact contracts, and parity failures include a useful first-difference location if another renderer divergence is introduced.

Nullable flow has been made explicit in DataGrid lookup, website export without a legacy DataView, selected-material reporting, Experimental verification and AI material scope. Both Debug and Release builds now complete with 0 warnings and 0 errors; warning analysis remains enabled.

## Roadmap consolidation

`Docs/Roadmaps/MASTER_ROADMAP.md` is again a strategic forward-looking document rather than a release log. The conflicting v39 Cost Analytics and Daily Workflow definitions are reconciled, early delivery of v42/v43 foundations is recorded, and `Docs/Roadmap.md` now points to the canonical roadmap instead of maintaining a competing plan.

## Pricing & Value Portal Tab

The main application workspace now prioritizes daily engineering work. Materials and Material Detail remain adjacent at the left, while Manufacturers, Purchase Orders, Inventory and Experimental Testing are grouped immediately after Website Export.

The website portal now includes `#pricing`. Pricing & Value Explorer, Performance vs Price and Value Rankings are extracted from the canonical template into that page exactly once. Matching pricing filters use unique HTML identities and synchronize with the original Filament Database controls in both directions, so both tabs keep one canonical visible dataset.

The mirrored controls deliberately reuse the original `.filters`, `.price-filter`, `.filter-hint` and `.search-reset` presentation classes so the two tabs have the same spacing, card surface, field sizes, multi-select height, helper text and responsive breakpoints.

The concise instruction “Click to select/deselect — multiple selections allowed.” appears once beneath Base material instead of repeating under every field. Multi-select surfaces are now 198 pixels high after a further 25 percent increase, and redundant descriptions beneath MSRP range and Pricing availability are omitted.

On wide screens both filter cards use the same eight-column order from Chart mode through Product line. Sort/Search occupy the lower-left side, while MSRP range and Pricing availability are bottom-aligned across columns six through eight on the lower-right side. Pricing mirrors both Chart mode and Sort mode as part of the shared canonical state. Explicit placement is released at narrower five-, three- and one-column breakpoints.

Within the lower row, Reset filters sits directly above Search, Sort tensile / impact occupies the next compact column, and MSRP begins immediately to its right.

The legacy "About the methodology" summary is removed during the portal transform. The approved SQLite template remains untouched, while the dedicated Methodology tab and whitepaper handoff remain canonical.

## Experimental Website Analytics

Series marked Website in Experimental Testing now publish a responsive engineering dashboard, series selector, five SVG chart types and an accessible result table. The payload is created and verified by `ExperimentalWebsiteService` from stored native result fields and `ExperimentalAnalyticsService` output. Browser JavaScript draws exported values only and does not own engineering calculations.

Verification Center includes live publication checks plus a deterministic two-run contract probe for identities, rankings, baselines, finite values, payload serialization and chart coverage.

## Explicit FTPS publishing fix

Website publishing now matches the confirmed FileZilla profile: explicit FTP
over TLS on port 21 with passive data connections. TLS is mandatory and the
server certificate must pass Windows trust validation.

## Superseded SFTP assumption

The initial v40.18.0 SSH/SFTP transport was replaced before a successful live
connection. Its backup, staging and rollback design is retained by v40.18.1.

## Website export folder persistence

The last folder selected with Choose Folder is stored in the existing local
workflow preferences and restored automatically on the next application start.
Unavailable folders are ignored safely.

## Relative manufacturer redirect fix

Preview redirects to the matching local `../index-test.html#manufacturers` file,
while Production redirects to `../index.html#manufacturers`. The full iskort.is
address remains canonical metadata only.

## Manufacturer redirect export cleanup

Website Export now has one canonical Preview/Production workflow. Each export
also writes a small redirect companion under `manufacturers/` pointing to
`https://iskort.is/3dp/index.html#manufacturers`. Preview remains isolated in
`index-test.html`; Production backs up the previous redirect before replacing it.

## Verification hotfix

The terminology verification gate now distinguishes the exact obsolete
`manufacturer-cta` class from the current `manufacturer-cta-row`,
`manufacturer-cta-primary` and `manufacturer-cta-secondary` classes. The
v40.17.4 form passed its own verification checks; this patch corrects the
false failure without changing the rendered form or website terminology.

## Manufacturer Material Submission Workflow

The Manufacturers portal now includes a structured browser-only form for
material testing enquiries. It prepares an addressed email to
`iskort@iskort.is`; no form data is uploaded to or stored by the website.

## Included

- Company, contact name and contact email.
- Material name, product line, material type and colour.
- Spool size and proposed sample quantity.
- Product-page and technical-datasheet URLs.
- Testing goals and additional notes.
- Required acknowledgement of independent, data-driven testing.
- Per-enquiry `3DPI-YYYYMMDD-XXXXXXXX` reference ID.
- `mailto:` submission with encoded subject and message body.
- Copy Submission Details fallback when a local email application is unavailable.
- Responsive, keyboard-accessible form styling and inline validation/status feedback.

## Architecture boundary

- No backend, SMTP integration, web database or direct SQLite access was added.
- The public form is an email preparation surface only.
- Canonical materials, results and manufacturer intelligence remain SQLite-backed.
- Website Preview and Production use the same manufacturer portal renderer.
- A server endpoint remains a future extension and can replace the delivery layer without redesigning the form.

## What to test

1. Open Website Preview and select Manufacturers.
2. Press Submit Materials for Testing and confirm the form scrolls into view.
3. Leave required fields empty and confirm browser validation prevents submission.
4. Enter company, contact, material and sample details; accept the acknowledgement.
5. Press Prepare Email Submission and confirm the email client opens an addressed message to `iskort@iskort.is` with a reference ID and all entered details.
6. Press Copy Submission Details and confirm the same structured content is copied.
7. Check desktop and narrow/mobile layouts.
8. Run Verification Center and confirm all manufacturer submission checks pass.
- Fixed the v44.7.7 Stage 5I candidate Fast Materials apply/reload loop: canonical validation refresh now waits for the existing
  edit debounce after the Fast view has accepted its snapshot change, so checkbox and text edits no longer reopen the unapplied-change
  prompt recursively.
