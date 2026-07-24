# Bug / Feedback Log

Use this during the usage-mode period.

## How to log an item

Copy this block for each finding:

```text
Date:
Area:
Type: Bug / Workflow friction / UI polish / Report idea / Website idea / Data issue
Severity: Blocker / Important / Minor / Idea
What happened:
Expected behavior:
Steps to reproduce:
Screenshot / export / report attached:
Status: Open / In progress / Partially solved / Solved / Deferred / Duplicate / Not planned
Resolution:
Verification evidence:
```

## Status review — 2026-07-24

This review preserves every original description and adds lifecycle metadata.
`Solved` is used only where an accepted release or direct runtime evidence can
be identified. `Partially solved` keeps the undelivered remainder visible.
`Deferred` records a deliberate boundary rather than silently discarding the
idea. Historical free-form entries remain in their original language and order.

| Status | Items |
|---|---:|
| Open | 15 |
| In progress | 1 |
| Partially solved | 3 |
| Solved | 29 |
| Deferred | 3 |
| Duplicate | 1 |
| Not planned | 0 |
| **Total tracked findings** | **52** |

## Tracked findings

Date: 2026-07-24
Area: Settings Manager command clarity and column reset naming
Type: Workflow friction / UI polish
Severity: Minor
Status: Open
Resolution: Research the exact persisted-data ownership and mutation paths behind `Load Settings` and `Restore Built-in Defaults`.
Make their distinct purpose, confirmation and result explicit; if they are unintentionally equivalent, correct the caller behavior
without changing canonical SQLite settings or deployment/Base Material ownership. Rename `Reset Fast Columns` to `Reset Columns` while
preserving the accepted settings-column layout reset scope.
Verification evidence: Requires persisted custom-value, load, built-in restore, cancellation, restart and column-layout runtime tests.
What happened: `Load Settings` and `Restore Built-in Defaults` appear to perform the same action by replacing entered values with
defaults. The `Reset Fast Columns` label also exposes an implementation detail rather than the user action.
Expected behavior: Loading should clearly reload the intended saved canonical values, restoring defaults should be a distinct
default-No destructive replacement, and the layout action should be labeled `Reset Columns`.
Steps to reproduce: Enter non-default Settings values, use each command separately and compare the resulting values; inspect the
Settings Manager toolbar label for the column reset action.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Materials / Tensile / Impact / Stiffness default row ordering
Type: Workflow friction / UI polish
Severity: Minor
Status: Open
Resolution: Add a researched default sort that orders canonical MaterialID values numerically ascending across the four daily grids.
Preserve explicit user sorting and saved layout behavior; do not use lexical ordering that places `MAT10` before `MAT2`, and do not
renumber, rewrite or move canonical rows in SQLite.
Verification evidence: Requires caller/view ownership research plus first-load, reset, filter, edit, restart and fallback-grid runtime
acceptance with the lowest MaterialID at the top and newest/highest MaterialID at the bottom.
What happened: Default views in Materials, Tensile, Impact and Stiffness are not consistently presented in ascending MaterialID order.
Expected behavior: On an unsorted/default view, show the lowest numeric MaterialID first and the newest/highest MaterialID last in all
four workflows.
Steps to reproduce: Open each of Materials, Tensile, Impact and Stiffness without applying a manual sort and compare the row order.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Application-wide user help and workflow guidance
Type: Workflow friction / UI polish
Severity: Idea
Status: Open
Resolution: Planned as the dedicated v46.0 user-help milestone. Cover every supported tab and the complete owner workflow
from Purchasing, Materials and Inventory through measurement entry, validation, analysis, reports, website Preview and guarded
Production publishing. Research current UI ownership and accepted behavior before writing; do not let help text redefine calculations,
data ownership, public allowlists, FTPS confirmation or recovery boundaries.
Verification evidence: Requires a complete tab/action inventory, owner review, in-app navigation testing, documentation link validation
and visual/runtime acceptance.
What happened: The application has many connected workflows, but no single structured user guide explains what every tab does or how
data should move through the platform from purchase entry to measurements, reports and website publication.
Expected behavior: Provide a well-organized user-help system with a start-to-finish workflow, per-tab reference, field-entry guidance,
prerequisites, validation meanings, safe publishing steps, recovery boundaries and links from relevant UI contexts.
Steps to reproduce: Follow a new user from recording a purchase through creating/linking a material, entering measurements, reviewing
results, generating reports and publishing the website; guidance is currently distributed or implicit.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Tensile / Impact / Stiffness Measurements
Type: UI polish
Severity: Minor
Status: Solved
Resolution: Solved in version v44.5.4 — 2026-07-24.
Verification evidence: Runtime screenshots and Full Data Verification PASS.
What happened: The instructional sentence at the top of each native measurement workspace is duplicated inside the XAML text itself.
Expected behavior: Each Tensile, Impact and Stiffness instruction appears once, followed by its calculation/read-only field guidance.
Steps to reproduce: Open each of the three measurement tabs and read the instruction directly below the page heading.
Screenshot / export / report attached: v44.5.3 runtime screenshots `codex-clipboard-ca877ee8-7358-4c0d-85d4-28a91426487f.png`, `codex-clipboard-967228f4-533e-4771-a97e-ae142b6fb715.png` and `codex-clipboard-050971f8-3092-468c-8e14-4f1fafee3f63.png`.

Date: 2026-07-24
Area: Verification Center / System Diagnostics exports
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: Solved in version v44.5.2 — 2026-07-24.
Verification evidence: Distinct Verification and System Diagnostics filenames were runtime accepted; Full Data Verification PASS.
What happened: Verification Center and System Diagnostics both export files using the generic `3DPIceland_FilamentDB_Diagnostics_YYYYMMDD_HHMMSS.txt` filename, even though they contain different report types. This makes attached files easy to confuse.
Expected behavior: Use terminology consistently across the window title, export action, document header and filename. Verification Center should export `3DPIceland_FilamentDB_Verification_YYYYMMDD_HHMMSS.txt`; System Diagnostics should export `3DPIceland_FilamentDB_System_Diagnostics_YYYYMMDD_HHMMSS.txt`.
Steps to reproduce: Export one file from Verification Center and one from System Diagnostics, then compare their filenames.
Screenshot / export / report attached: Observed in the v44.5.1 runtime diagnostics/verification handoff.

Date: 2026-07-24
Area: Build history / release documentation governance
Type: Workflow friction / Data issue
Severity: Minor
Status: Solved
Resolution: Solved in version v44.6.1 — 2026-07-24.
Verification evidence: Canonical release-documentation audit PASS and Full Data Verification 311/311 PASS.
What happened: There is no canonical total for completed builds or documented releases. `BUILD_HISTORY.md`, `CHANGELOG.md`, `RELEASES.md` and `MILESTONES.md` contain different numbers of version headings, duplicate version identifiers and different historical coverage, so the total depends on which document is counted.
Expected behavior: Define exactly what counts as a build, candidate, runtime-accepted release and canonical release. Generate one canonical version/release index from governed metadata, show totals by status, and validate the documentation files against it. The check should flag duplicate version headings, missing entries, conflicting titles/statuses and version-order anomalies without rewriting historical records silently.
Steps to reproduce: Count unique version headings and duplicates independently in `BUILD_HISTORY.md`, `CHANGELOG.md`, `RELEASES.md` and `MILESTONES.md`; the totals do not match.
Screenshot / export / report attached: Repository audit on 2026-07-24 found 289 unique versions in Build History, 317 in Changelog, 47 in Releases and 57 in Milestones.

Date: 2026-07-24
Area: Tensile / Impact / Stiffness / Experimental measurements and Material Details
Type: Workflow friction / Data issue
Severity: Idea
Status: Solved
Resolution: Solved in version v44.6.2 — 2026-07-24.
Verification evidence: Schema v31, native/Experimental date persistence, Material Detail dates, column-reorder editing and Full Data Verification 312/312 PASS.
What happened: Measurement-entry rows do not record when each test was performed, so the application has little historical context for measurement age, test sequence or retesting.
Expected behavior: Store a canonical measured date per test type and per experimental run rather than one shared Material date. Auto-fill the date when the first measurement value is entered, but do not silently replace it during later corrections or delete it when values are cleared. Allow intentional manual editing for measurements entered after the fact. Keep `Measured date` explicitly separate from record `Last edited` metadata, display a clear local date while storing an unambiguous ISO date in SQLite, and preserve it through governed Excel disaster-recovery export/restore.
Steps to reproduce: Enter Tensile, Impact or Stiffness measurements for a MaterialID and inspect the measurement rows and Material Details; no canonical measured date or test history is available.
Screenshot / export / report attached: User feedback on 2026-07-24.
Potential downstream use: Material Details test timeline; first/last measured dates and testing span; measurement freshness; oldest/newest filters; retest queues; report provenance; comparisons over time. Public reports must expose dates only through an explicit reviewed allowlist.

Date: 2026-07-24
Area: Public website / Material engineering reports
Type: Website idea / Data issue
Severity: Idea
Status: Open
Resolution: Add the canonical Tensile, Impact and Stiffness measured dates introduced in v44.6.2 to governed public material data so users can see when each published measurement was performed. Publish only explicitly reviewed fields for materials already allowed on the public website; missing dates must display `Not recorded`, and no internal edit timestamps, notes or private history may leak.
Verification evidence: Requires public-model allowlist review, local HTML/PDF visual verification, Preview/Production renderer parity, manifest checks and explicit runtime acceptance before publication.
What happened: Canonical measured dates are now available in SQLite and Material Details, but public website users cannot yet see when the displayed engineering measurements were performed.
Expected behavior: Public material pages and applicable reports show clear per-test measured dates sourced from the same canonical SQLite metadata, with an unambiguous display format and honest missing-data behavior.
Steps to reproduce: Open a published material page after v44.6.2 and compare its engineering information with Material Detail > General > Test Information in the desktop application.
Screenshot / export / report attached: User feedback on 2026-07-24.

Date: 2026-07-24
Area: Bug / Feedback Log governance
Type: Workflow friction / UI polish
Severity: Minor
Status: Solved
Resolution: Solved as documentation-only roadmap increment v44.7.0 — 2026-07-24. Every retained item has lifecycle metadata, and the authoritative future sequence is maintained in Master Roadmap.
Verification evidence: Owner review accepted; status coverage 49/49 PASS, 136-column roadmap formatting PASS and canonical release-documentation audit PASS.
What happened: Feedback items do not have a consistent status or resolution record, making it difficult to distinguish open work from completed work or identify which accepted release solved an item.
Expected behavior: Preserve every original feedback description and append structured lifecycle metadata. New items should use `Status: Open`; supported states should include `Open`, `In progress`, `Partially solved`, `Solved`, `Deferred`, `Duplicate` and `Not planned`. After runtime acceptance, a solved item should receive a simple canonical resolution such as `Solved in version v44.5.1 — 2026-07-24`, optionally followed by `Verification evidence: Full Data Verification 302/302 PASS`. Use ISO dates and only canonical runtime-accepted versions; never mark a failed or unaccepted candidate as solved. Partially completed items remain open or explicitly `Partially solved`.
Steps to reproduce: Review older entries in `BUG_FEEDBACK_LOG.md`; completed and outstanding items cannot be distinguished consistently from the entry itself.
Screenshot / export / report attached: User feedback on 2026-07-24.



## Triage categories

- Bugs: incorrect behavior or broken workflow.
- Workflow friction: too many clicks, confusing flow, repeated manual work.
- UI polish: readability, sizing, labels, layout.
- Website ideas: improvements visible to YouTube viewers or public users.
- Reporting ideas: improvements to report clarity or sharing.
- Data issues: inconsistent material metadata, missing links, missing measurements.



improvment:
Status: Partially solved
Resolution: Material-level USD/kg and price/value context exist, but the requested governed cross-metric `price per engineering result` design is not fully delivered. Retain as a bounded analytics idea.
Verification evidence: v41.3.0 pricing context and v40.12.2 terminology/value work are accepted; remaining metric contract is open.
I had this idea if we could implement a Price field in USD for each material, even though I buy it in different currency's I would calculate the USD price if need, 
then as I note spool waight, we could display price per 1KG for all materials independant on spool sise, and use that thata in the charts/report like 
"Price per Impact strengh" or something like that, we have to work on this idea to make the most of this data point.


Workflow friction:
Status: Solved
Resolution: Solved in version v39.1.2 — native Tensile and Impact sample cells use dark text on yellow backgrounds.
Verification evidence: Runtime acceptance checklist explicitly covered values below 10.
þegar ég er að setja inn efni, þá er röðin með hvítum texta, ef gildi er minna en 10 t.d í tensile, þá sérst ekki hvítir stafir á gulum bakgrunn. -  þetta þarf að laga

Workflow friction:
Status: Partially solved
Resolution: The incorrect all-material collection scope was solved by canonical visible/active MaterialID projection. Broader AI Assistant usability and product purpose remain open for separate research.
Verification evidence: Solved scope defect in v41.7.2; no claim that the complete AI Assistant redesign is finished.
fæ ekkert vit í AI assistance, - hafði valið bambu labs framleiðandia í materials tab í gegnum Leitina. og bjó til collection, það innihélt ekki bara bambu labs efni, heldur öll efni 0-176, 
sama ef ég hafði filterað eftir manufactures í materials
fæ í raun ekkert vit í neitt af því sem er að gerast í þessum ai assistance tab, - þetta þarf að þróa áfram ´serstakletga.


improvment:
Status: Solved
Resolution: Solved in version v44.7.1 — Category Rankings provides safe 5/10/50/100/All scope controls.
Verification evidence: Owner runtime acceptance and Full Data Verification 313/313 PASS.
catagory rankings  - sýnir aðeins top 10, - mundi vilja sjá allan listann eða allavega fleiri options í rows per group, 5 , 10, 50,100,all, ég er kanski að miskilja hverning þetta á að virka.



improvment:
Status: Solved
Resolution: Solved in version v39.1.2 — default window size increased to 1700 × 960 while saved user geometry remains authoritative.
Verification evidence: Runtime-accepted window sizing behavior.
appið opnast alltaf í frekar litlum glugga, - mætti vera 2x á hæð og 1.5 x á breydd, eða eitthvað álíka., 
það væri t.d mjög þægileg að glugginn væri það stór til hliðana að hann sýndir allt svæið sem þarf að vera sýnilegt þegar ég færi inn mælingar í tensile og impact

tech notes sem eru þegar ég er að færa inn mælingar, mættu vera í dálk rétt á eftir innslegnum tölum í flat 10, á eftir bilinu sem er þar á eftir, en ekki lengst til hægri eftir að útreikningar eru komnir.
Status: Solved
Resolution: Solved in version v37.1.4 — Test Notes were repositioned before computed measurement results.
Verification evidence: DATA-ENTRY-004 runtime-accepted compact detail and notes flow.

improvment:
Status: Partially solved
Resolution: The requested duplicate-material/dynamic-column concept was superseded by canonical MaterialID-linked Experimental Series and Runs with dedicated measurement editors and governed website/report integration. Additional experimental presentation ideas remain open only when backed by a concrete consumer.
Verification evidence: v40.3.0 Experimental Series/Runs foundation and later v40 Experimental analytics/publishing releases are accepted.
útfærsla á experimental mælingum eins og t.d styrk á prenti eftir mismunadni hitastigi, eða nozzle size sem dæmi, - að þegar ég addas material, þá sé dálkur þar sem ég get hakað í "Experimental", 
og þá sé sú lína búin til í impact, tenile and stiffness, og ég set inn þær mælingar eins og um venjulegt efni er að ræða, - þurfum svo að hafa dálk til að gefa til kynna hvaða experimental mælingar eru í gangi, 
það gæti verið dropdown menu item í material listanum, komandi úr settings manager tab, þar sem ég skilgreini tilraunir t.d "Different Nozzle Size" "Diffrent Tempurature" "Different Extrucion with" "Different outer walls"
og hvað annað sem mér mundi detta í hug síðar að gera samanburðarmælingar á.Svo þarf að vera dálkur þar sem ég set gildið á mismunandi mæligum. t.d með hita, þá sé reitur yfir "temp" , 
og þannig kolll af kolli fyrir þær mælingar sem ég er að gera hverju sinni, - ef ég bæti við tilraun síðar, þá þarf að koma dálkur í materials tabfyrir þá tilraun, svo ég geti slegið inn 
þau gildi sem er verið að eiga við hverju sinni. - Fyrir venjulegt html export, þá eru efni merkt "experimental" ekki sett sjálfkrafa á heimasíðuna, en þar gæti verið filter, sem er default á 
"standard mæling", en hægt að velja í dropdown "Experimental" og þá færi filterað niður á efni sem væru í þeim flokki, þá kæmu eingöngu þau efni fram í súluritum, og það þyrfti að vera sér súlurit 
fyrir hverja tilraun sem er gerð, - svo það þyrfti að byggja upp html'ið með þetta í huga samhliða innleiðingu á þessari breytingu. 

improvment:
Status: Open
Resolution: No accepted release evidence proves that the requested transparent-background icon was adopted. Existing canonical application/report branding must be reviewed before changing it.
Verification evidence: None.
 gera icon með transparent bakgrunn

Bug:
Status: Solved
Resolution: The obsolete 176-row compiled fallback and stale cache terminology were retired; live SQLite counts are canonical.
Verification evidence: v43.7.0 removed the compiled 176-row fallback; v44.5.x runtime accepted canonical SQLite status/count surfaces.
efst uppi hægra megin er texti sem segir 176 materials | 26 colums -  þetta uppfærist ekki, 
undir haus í forritinu er líka texti sem segir "loaded 176 materials and 26 clolums from local cache, import from excel to refresh, - þetta endurspeiglar ekki fjölda efna í grunninum núna. -
 verification center er rétt og segir 184 efni í material, tensile, impact og stiffness, svo kerfið 
veit hvað það eru mörg efni í grunninun.


improvement:
Status: Solved
Resolution: Solved in version v43.7.0 — governed per-user installer and portable deployment keep SQLite/backups/configuration outside application files and support a user-owned storage location.
Verification evidence: Clean-VM installer, explicit SQLite transfer/restore, portable and governed package gates PASS.
Gera installer fyrir forritið, sem setti það inn í rétta möppu undir c:\Program files, og þar væru allar tilheirandi skrár, kæmi upp í install ferli hvar á að installa, og hvar á að geima sqllite grunninn, koma deault upp með documents/3dp Iceland app  
eða eitthvað álíka,  s.s ekki hafa gagnagrunninn í program files, heldur í möppu sem er líkleg til að vera afrituð með onedrive

improvement:
Status: Solved
Resolution: Solved through v37.1.0 smart first-run widths plus v37.0.0 machine-local width persistence.
Verification evidence: Runtime-accepted workflow grid sizing; user widths remain preserved.
rezize á dálkum, should not be as large as they are, should trim down to the text in it or the label of it., website colum should be shorter by default to just the lable size.

BUG-Chrash!
Status: Open
Resolution: Retained as an unresolved historical crash report. The log has no stack trace or exact accepted fix that can safely be tied to this one-day runtime/Variant edit crash.
Verification evidence: Needs reproduction evidence or diagnostics before closure.
krass, -  var nýbúinn að setja inn values, forritið var búið að vera í gangi í 1 sólarhring,
fór svo í material, og ætlaði að setja inn variant á efni, og þá krassaði það - gögn úr imact sem ég var að setja inn héldu sér.

improvement:
Status: Deferred
Resolution: External AI API connectivity requires an approved concrete consumer, privacy/credential boundary and support contract. Do not implement speculatively.
Verification evidence: Deferred by bounded v44 roadmap discipline.
ai api connection

improvement:
Status: Open
Resolution: The idea needs a defined owner model for test hours, print hours, material usage and sample counts before schema/UI work.
Verification evidence: None.
hours of testing, printing, and material useage, sample coubt

improvement:
Status: Solved
Resolution: Solved in version v40.10.0 — the active validated website template is stored and versioned in SQLite and reused by Preview/Production export.
Verification evidence: Native Website Template Database and renderer-parity gates PASS.
website export - "Used Bundled" what is that, and how do I update the bundled, it should update with the latest html template file used, so if I don't change the html, I can just press use bundled instead of reading the current html, 
The last uploaded html should be  stored in the sql database, not in the build it self.

improvement:
Status: Duplicate
Resolution: Duplicate of the structured canonical measurement-date item above; solved in v44.6.2.
Verification evidence: Full Data Verification 312/312 PASS.
date of entri of mesuraments

bug:
Status: Solved
Resolution: Solved in version v36.0 — selecting `Not Tested` with no matching material returns an empty result.
Verification evidence: Runtime checklist required `0 visible`.
filter í materials, - if I select "not tested" and all filaments are tested, it shows all filaments, it should show none.

bug:
Status: Solved
Resolution: Obsolete engine/cache counters were removed or replaced by canonical SQLite live-data status and current per-module counts.
Verification evidence: v44.5.x canonical storage terminology/count runtime acceptance.
at the bottom, it shows "Engine: 167 materials, 5 sheets, 880 rows 27884 cells" - this is not updateing automatically, - what is this used for anyway?, as to the right there is a list of materials in each test, that is updated.

improvement:
Status: Open
Resolution: Product decision remains open. The field must not be removed until callers, Excel recovery and report compatibility are audited.
Verification evidence: None.
I don't think I will every use "Manufacturer SKU" ,as I link to the product page, and sku information is not given by resellers in most cases.


improvement:
Status: Solved
Resolution: Solved in version v40.12.2 — public/chart terminology uses Tensile Strength for flat orientation and Layer Adhesion Strength for upright orientation.
Verification evidence: Metric selectors, chart labels, axes and tooltips were updated and accepted.
website html fix, - tensile - flat  is tensile, upright is layer adheesion, I'm always explaining in videos that, maybe they should just say "Tensile" and "Layer Adhesion",

Bug:
Status: Solved
Resolution: Solved in version v36.1.0 — selected Material Detail and Dashboard Insights refresh after Tensile, Impact and Stiffness edits.
Verification evidence: STABILITY-002 accepted refresh path.
In Material Detail, - data is not updated  for tested, in tensile ,in impact, in stiffness



improvment: in Materials, I don't think I will save a name of the thumbnail for the videos
Status: Open
Resolution: Product decision remains open. Audit YouTube/video-planner callers before hiding or retiring any thumbnail-name field.
Verification evidence: None.
i manage them seperatly in my youtube files folder

BUG: 
Status: Solved
Resolution: Solved through v36.0.1/v36.1.0 and later canonical projection work; Dashboard Insights now uses current canonical material and measurement projections.
Verification evidence: Accepted dynamic coverage/inventory refresh and v41.7.2 canonical MaterialID projection.
Dashborad Insights, - does not update as I add more materials , needs to by dynamic

Status bar at the bottom, still says Engine: 176 materials, does not update, same as the status 
Status: Solved
Resolution: Duplicate stale engine/cache status was retired in favor of canonical SQLite live-data counts.
Verification evidence: v44.5.x runtime screenshots showed current 200/201-material SQLite status.
on top under the logo, - maybe this is not needed at both places - but later in the bottom line
it states 197 materials, -

qustion;: what does "validation" mean for each line of material?
Status: Solved
Resolution: Solved in version v44.7.2 — Materials explains the five required row-identity fields and the exact meaning of OK.
Verification evidence: Owner runtime screenshot acceptance and Full Data Verification PASS.

Bug: in material report, the reports says on top "Total Materials: 176" not the actual number
Status: Solved
Resolution: Solved in version v36.0 and subsequent canonical report projections; stale total and duplicate logo were removed.
Verification evidence: Duplicate report logo fix and canonical live material/report scope accepted.
on the report, the logo is displayed 2 times on the top

on manufacturers html export, - it does not update "materials in database", 
Status: Solved
Resolution: Manufacturer/public report surfaces now consume canonical active/material visibility projections rather than the historical 176-row dataset.
Verification evidence: v41.7.2 canonical projection and later public manufacturer report verification.

Add "Purchased from" to the material tab
Status: Solved
Resolution: Purchased From is present in canonical Material Detail/Material workflow data.
Verification evidence: Runtime-accepted Material Detail Supplier/Purchase presentation.

Cost per KG calculation based on cost and spool weight
Status: Solved
Resolution: Solved in version v38.2.10 for inventory calculations; later landed-cost releases add governed purchasing context.
Verification evidence: Accepted calculated Cost/kg column and Average Cost/kg summary.

selecting from dropdown menu for Currency is to many clicks,  first click select the rows
Status: Solved
Resolution: Solved by the shared first-click editor workflow and immediate ComboBox opening in v37.0.x.
Verification evidence: Runtime-accepted first-click TextBox/ComboBox workflow retained in current native grids.
then nex click shows the dropdown, then I have to click the down arrow, and then select curreny
clicking on a cell, should automatically select the row/cell and alowing me to show the drop
down menu right a way so I can select the currency. This is basically the same function on anyway
cell that has user input, it should allow me to click on a sell, and right away
enter the value or text in it.

landed currecny should default to ISK
Status: Solved
Resolution: Purchasing currency settings and automatic ISK exchange-rate fill were delivered in v38.4.1.4–v38.4.1.6.
Verification evidence: Currency dropdown, autofill and live-sync runtime fixes accepted.

í staðinn f. að setja inn handvirkt landed cost, gera þá "hvað kostar evran" í sendingunni, og reikna
Status: Solved
Resolution: Governed Purchase Order exchange rates and landed-cost allocation replaced the manual-only workflow.
Verification evidence: v38.4 purchasing currency/autofill/live-sync and landed-cost persistence releases accepted.
út sjálfkrafa landed cost

typing in "filter" is really slow to show resaults
Status: Solved
Resolution: Solved in version v44.4.1 — measured viewport-only Materials rendering is the accepted daily-use default with native fallback retained.
Verification evidence: Full Data Verification 300/300 and cold/repeated search/filter/scroll runtime acceptance.

improvement:
Status: Solved
Resolution: Solved through v41.8.0 instrumentation, v41.8.1 callback coalescing and v44.4.1 measured Materials responsiveness.
Verification evidence: Observed Debug startup reduced from about 19–20 seconds to about 5 seconds; later canonical fast-view acceptance PASS.
startup performance optimization. Baseline on 2026-07-21 from pressing Play in
Visual Studio: splash appears after about 3 seconds and Materials is visible at
about 19 seconds. Profile SQLite/native loading, manager initialization, grid
binding and deferred intelligence work before changing architecture. Preserve
canonical MaterialID/Verified Material Summary behavior and Verification parity.
Scheduled immediately after the v41.7 Report Portfolio. Prefer lazy initialization
and independent asynchronous reads; use extra CPU cores only where profiling shows
CPU-bound work, with WPF updates kept on the UI thread and separate SQLite read
connections for concurrent operations.

v41.8.0 instrumentation implemented on 2026-07-22. System Diagnostics now records
the ordered startup phases needed to choose the first optimization from measured
Debug, cold Release and warm Release results. No startup work has been reordered yet.

First trace: MainWindow construction about 1.0 seconds, followed by an approximately
17-second delay before Show. Root cause found in bulk Native Materials replacement:
about 201 identical Dispatcher refresh callbacks were queued for 200 rows. v41.8.1
coalesces them into one downstream refresh. Runtime acceptance confirmed Verification
PASS and reduced observed Debug startup from about 19-20 seconds to about 5 seconds;
the accepted trace rendered the first usable Materials view at 4.49 seconds.

improvement — backup filename convention:
Status: Open
Resolution: Current Recovery Center intentionally retains discoverable `.sqlite` backups. The proposed presentation-only `.bak` convention has not been accepted and must remain backwards compatible without renaming/deleting evidence.
Verification evidence: Current v44.6.2 runtime still displays canonical `.sqlite` backup names.
Use a professional, human-readable backup filename such as
`Product-YYYY-MM-DD_HHmmss.bak` instead of exposing a plain `.sqlite` filename.
The `.bak` extension is a presentation/naming convention only: the file remains
a canonical SQLite backup and must retain integrity/schema verification,
collision-safe timestamps, explicit restore handling and backwards-compatible
discovery/restore of existing `.sqlite` backup files. Do not rename or delete
existing backups automatically.

improvement — user-reorderable workflow columns (v44.2 candidate):
Status: In progress
Resolution: v44.7.3 supersedes repeated editable-DataGrid reset fixes with an
approved phased Fast Workflow Grid migration. Tensile is the first candidate;
the legacy Tensile grid remains a visible fallback until runtime acceptance.
Verification evidence: Debug/Release pass. Runtime editing, calculation,
autosave, layout restart and fallback were accepted with Full Data Verification
315/315. First runtime editing found commit-time row rebuild returned MAT0206
selection to MAT0102; the accepted correction refreshes computed cells in
place. Owner reported Fast Tensile noticeably snappier than the legacy grid.
v44.7.4 applies the accepted core to Impact with unchanged bounded input,
formulas, colors and canonical persistence; runtime acceptance remains
required. First runtime review found negative Tensile input, repeated Impact
invalid-value warnings and reset-time canonical reordering. The candidate now
enforces non-negative canonical samples, restores rejected cells after one
warning and resets Fast layouts in place. Owner runtime retest and Full Data
Verification 316/316 accepted the complete Fast Impact increment.
v44.7.5 applies the accepted core to Stiffness with canonical 0–10
revolutions, 0–359 degrees, unchanged formulas and separate layout state;
runtime acceptance remains required. First Stiffness runtime review found a
leading blank region and editors one column left; the shared surface now uses
explicit left/top alignment and WPF coordinate translation. Owner runtime
retest and Full Data Verification 317/317 accepted Fast Stiffness.
v44.7.6 applies the accepted core to general Settings Value editing and the
Base Material Catalog while preserving distinct save, FTPS validation and CRUD
contracts. First Settings open raised an
unhandled `FormattedText` argument-range exception; shared Fast rendering now
normalizes transient lazy-tab DPI and geometry before drawing text. Follow-up
showed blank Fast content and missing Settings controls: activation is now
deferred until tab realization and controls have explicit Settings ownership.
Cross-tab review found duplicate blank spacer keys grouping Impact/Stiffness
separators; stable unique identities and stale-layout fallback now preserve
their canonical positions and later user layout. Materials filters refreshed
only legacy measurement views; Fast Tensile, Impact and Stiffness now reload
from the same established visible MaterialID set. Owner runtime retesting
accepted Settings editing/fallback, separator persistence and shared filter
scope; Full Data Verification passed. v44.7.6 is complete.
v44.7.7 retires the now-accepted legacy fallback in runtime-gated stages.
Stage 1 hides all legacy/preview switches while retaining collapsed DataGrids
as temporary Fast schema/row adapters. Later stages must introduce explicit
Fast contracts before removing legacy XAML and handlers.
The first v44.7.7 Verification run reported 77 cascading FAIL results from one
release-identity mismatch: informational metadata remained v44.7.6 while the
assembly was v44.7.7. The metadata is aligned; no canonical data or workflow
failure was reported.
Owner runtime acceptance and Full Data Verification 319/319 completed Stage 1.
Stage 2 will replace measurement DataGrid schema/row adapters.
Stage 2 candidate now uses explicit Fast measurement schemas and canonical
filtered measurement collections. Legacy measurement XAML remains collapsed
until this ownership passes runtime acceptance.
Owner runtime acceptance and Full Data Verification 320/320 completed Stage 2.
Stage 3 will replace the Materials DataGrid schema/row adapter.
Stage 3 candidate now uses an explicit Materials Fast schema and canonical
filtered row source. Legacy Materials XAML remains collapsed until runtime
acceptance.
Initial runtime testing found edit-time reordering, stale hidden-grid Duplicate
selection, a white Materials surface after tab return and a close-time SQLite
foreign-key failure after unsaved Add/Duplicate measurement synchronization.
The candidate now preserves same-scope row state, uses Fast selection, redraws
on load and synchronizes measurements only after successful Materials save.
Runtime behavior then passed, but Verification found 201 UI Materials versus
203 SQLite Materials because Delete deferred parent persistence after removing
measurement children. Delete now saves child removal first and parent removal
immediately; Archive/Unarchive queue normal auto-save.
Owner retest confirmed the test Materials were absent, UI/SQLite parity was
restored at 201 and Full Data Verification passed 321/321. Stage 3 is complete.
Stage 4 candidate now uses explicit General Settings and Base Material Fast
schemas with canonical row collections. Legacy Settings XAML remains collapsed
until runtime acceptance.
Owner runtime acceptance and Full Data Verification 322/322 completed Stage 4.
Stage 5 can remove legacy DataGrid XAML and grid-only event paths.
Stage 5A candidate removes measurement legacy toggle controls, handlers and
fallback state. Collapsed measurement XAML remains until this activation-path
retirement passes runtime acceptance.
Owner runtime acceptance and Full Data Verification 323/323 completed Stage
5A. Stage 5B can remove collapsed measurement XAML and grid-only lifecycle
code.
Stage 5B-Tensile candidate removes the complete legacy Tensile DataGrid and
named lifecycle references while retaining the accepted Fast/canonical paths.
Owner runtime acceptance and Full Data Verification 324/324 completed the
Tensile deletion checkpoint. Impact is next.
Stage 5B-Impact candidate removes the complete legacy Impact DataGrid and
named lifecycle references while retaining the accepted Fast/canonical paths.
Owner runtime acceptance and Full Data Verification 325/325 completed the
Impact deletion checkpoint. Stiffness remains unchanged and is next.
Stage 5B-Stiffness candidate removes the final legacy measurement DataGrid,
named grid lifecycle and now-obsolete deferred DataGrid warm-up while
retaining the accepted Fast/canonical paths. Owner runtime acceptance and Full
Data Verification 326/326 completed Stiffness and measurement-grid retirement.
Allow users to drag and reorder columns in the Materials, Tensile, Impact and
Stiffness tabs. Persist column order as machine-local UI state, keyed by stable
bound field identity rather than column index. Preserve required fields,
validated frozen-column behavior and backwards compatibility when columns are
added or removed. Invalid/stale layouts must fall back safely, and each grid
should offer Reset columns to default. Do not store presentation layout in the
canonical SQLite engineering backup.

improvement — optional clean uninstall (v44.3 candidate):
Status: Deferred
Resolution: Normal uninstall remains deliberately data-preserving. A destructive profile-reset option is deferred until its exact ownership, credential separation and recoverability contract justify the risk.
Verification evidence: v43.7.0 installer and all later updater/recovery releases preserve SQLite, backups, credentials and evidence by design.
Keep normal uninstall data-preserving. Add a clearly separate, default-unchecked
`Delete all 3DPIceland user data and reset this Windows profile` option with a
second Default-No confirmation that lists every exact target. If explicitly
accepted, remove only validated 3DPIceland-owned current-user paths for the
SQLite database, backups/recovery evidence, local UI preferences, updater
transactions/rollback/health evidence and supported legacy app-data locations.
Never delete a parent folder, unrelated files or an unresolved path. A custom or
OneDrive storage location requires an additional explicit warning and must be
proven to be the configured 3DPIceland storage folder. Treat stored Windows/FTPS
credentials as a separate default-unchecked choice rather than deleting them
silently. Record which targets were removed, retained, missing or locked, and
state clearly that accepted data deletion is permanent and cannot be undone.

improvement — canonical Base Material selection:
Status: Open
Resolution: The governed Base Material Catalog exists, but no accepted release evidence proves this exact backwards-compatible Materials dropdown/unmapped-value workflow.
Verification evidence: Requires caller and migration audit before implementation.
Replace free-text Base Material editing in Materials with a dropdown sourced
from the SQLite-governed Base Material Catalog maintained in Settings. Store the
canonical catalog value rather than display position so sorting or catalog edits
cannot change existing records. This should prevent spelling, casing and
near-duplicate classifications while preserving keyboard-friendly selection.
Existing/imported Base Material values that are absent from the current catalog
must remain visible and must not be silently cleared or remapped; show them as
an explicit legacy/unmapped value and require an intentional catalog addition or
user-selected replacement. Refresh dropdown choices after an accepted Settings
change and preserve MaterialID, import/export and report compatibility.

improvement — MaterialID-aware print-job price calculator:
Status: Open
Resolution: Approved design concept, not an accepted implementation. Retain as a bounded future workflow; formula rights, units, currency provenance, profiles and immutable quote snapshots must be researched first.
Verification evidence: None.
Evaluate bringing the existing standalone Printing Price Calculator from
`price/index.html` into the desktop application as a bounded job-quotation
workflow. Replace its free-text `Material Used` input with a canonical MaterialID
selector from the active Materials list. When a material is selected, populate
`Filament Cost per kg` from the material's governed landed-cost data, with the
material name/manufacturer and cost provenance visible beside the value.

Research and define the unit/currency contract before implementation. If the
stored landed cost is a whole-spool value, calculate cost per kg only from the
recorded net spool weight; never assume a 1 kg spool. Convert currency only from
an explicit governed exchange-rate source and show the source currency, rate and
checked date. Missing landed cost, weight or conversion data must remain
`Not recorded` and require manual user input rather than a silent MSRP/default
fallback. Selecting a material may prefill the calculator but must not overwrite
the canonical Material record.

Preserve the calculator's existing inputs and formulas for filament grams,
material-efficiency factor, labor, machine time, packaging, landed job cost,
target margin and quote output unless formula-by-formula verification approves a
change. Record the selected MaterialID and effective cost-per-kg provenance in
the exported quote. Review the existing Print Farm Academy attribution and reuse
rights before moving or adapting its credited methodology into the application.

Approved calculator settings/profile design:
Status: Open
Resolution: Design extension owned by the open MaterialID-aware print-job price calculator item; no schema/UI implementation is accepted.
Verification evidence: None.
Move reusable formula inputs into governed Settings rather than requiring
re-entry for every job. Keep business-wide values such as Material Efficiency
Factor, Labor Hourly Rate, Electricity Cost per kWh and default currency in a
global pricing-settings group. Add a typed Printer Profiles catalog with stable
PrinterID, printer name, purchase cost, additional upfront cost, annual
maintenance/repair, estimated life, estimated uptime, average power consumption
and printer-cost buffer factor.

Allow multiple printer profiles and one explicit default printer. The calculator
must offer a printer dropdown populated from this catalog while still allowing a
different printer to be selected for each job. Archive rather than hard-delete a
profile that has historical quote references. Validate ranges/units and show
missing values honestly; do not silently borrow values from another printer.

Each generated quote must store an immutable calculation snapshot so later
Settings changes cannot alter historical results. The snapshot/export should
include MaterialID and landed-cost provenance, PrinterID and printer name, every
effective calculation input, calculation/formula version, currency, any governed
exchange-rate evidence and calculation timestamp. Printer profiles and global
pricing settings are configuration data; they must not recalculate or overwrite
canonical Material purchasing records.

improvement — delayed official exchange-rate refresh:
Status: Deferred
Resolution: Deferred until an official stable endpoint, reuse contract and concrete purchase/quotation consumer are approved. Existing manual/governed purchasing rates remain authoritative.
Verification evidence: Current accepted releases do not perform an official background-rate download.
Add an optional, non-blocking exchange-rate refresh after the application is
fully usable (initial candidate delay: about 60 seconds). This must never delay
startup, require credentials or fail application readiness when offline. Prefer
an official subscription-free source; the European Central Bank publishes
working-day EUR reference rates, including ISK, with downloadable XML. Evaluate
an official Central Bank of Iceland feed as the preferred ISK-native source if a
stable documented endpoint and reuse contract are confirmed before coding.

Store exchange rates as governed Settings/reference data with source name,
source URL, base currency, effective date, fetched-at UTC, exact rate and status
(`Current`, `Weekend / no new rate`, `Stale`, `Unavailable`, `Manual`). Refresh
at most once per effective day unless the user presses `Refresh rates`; use a
short timeout, bounded retry and the last verified cached rates when the network
or parser fails. Show last successful update, age and source in Settings, and
allow automatic refresh to be disabled. Validate TLS, content type, supported
currency codes, positive finite values, duplicate entries and plausible payload
date before replacing the cached set.

Automatic refresh updates only the reference-rate catalog. It must never
silently rewrite historical purchasing records, landed costs or prior quotes.
Every calculation/quote that converts currency must snapshot the exact source
rate and effective date it used. Missing or stale conversion must be visible and
must require an explicit user decision rather than silently using 1:1 or another
currency. Document that official reference rates are informational and may not
match the card/bank rate actually charged; an entered transaction-specific rate
remains authoritative for landed-cost evidence.

Approved purchase/lot exchange-rate ownership:
Status: Open
Resolution: Approved ownership rule for any future implementation; purchase/lot snapshots must never become MaterialID-wide mutable rates. No claim of delivery.
Verification evidence: None.
Bind the effective exchange-rate snapshot to each purchase line, receipt lot or
spool acquisition record, not to MaterialID. The same MaterialID may therefore
have multiple historical purchases with different original prices, currencies
and exchange rates. A newly downloaded rate may only prefill a new, unsaved
purchase transaction; it must never recalculate or overwrite a saved purchase,
received spool/lot, historical landed cost, inventory valuation evidence or
quote.

Each purchase snapshot must preserve original amount/currency, exact rate,
source, effective date, fetched-or-entered timestamp, converted ISK amount,
shipping/tax/fee inputs and resulting landed cost. For job pricing, prefer the
landed cost of the explicitly selected spool/lot. When no lot is selected, use
only a separately approved and visibly identified inventory costing method such
as weighted average; expose the contributing scope and provenance. Never apply
the latest exchange rate to an old foreign-currency purchase price or silently
fall back to 1:1, MSRP or the most recent purchase.

improvement — governed Production and Development build profiles:
Status: Open
Resolution: Research item. Measure actual diagnostic cost before introducing build-profile behavior; mandatory recovery/security/support diagnostics must remain available.
Verification evidence: No deterministic Production/Clean profile release has been accepted.
Research and classify all accumulated diagnostics, verification probes, startup
measurements, debug-only commands and owner/developer menus by measured runtime
cost and operational ownership. Introduce deterministic build profiles rather
than deleting instrumentation code:

- `Development / Verification`: includes the complete diagnostic, profiling and
  verification surface needed during an active development or runtime-acceptance
  session.
- `Production / Clean`: excludes or disables proven expensive development-only
  probes and hides developer-only commands/menus unless they are explicitly
  enabled by an approved build option.

Keep the instrumentation definitions, expected outputs and enablement contract
in source control so a later development session can reproduce the required
measurement surface without reimplementing it. Prefer an explicit MSBuild
property/compile constant and visible build identity over manual commenting,
source deletion or dynamically downloaded diagnostic code. A Production artifact
must state which profile it uses, and packaging/Verification must reject an
unknown or contradictory profile.

Do not assume diagnostics are expensive: measure startup, steady-state CPU,
memory, SQLite reads and menu/open costs first. Production/Clean must retain
mandatory crash/error logging, release identity, privacy/schema/integrity checks,
update/recovery transaction evidence, backup/restore safety, security/package
verification and the minimum support diagnostics needed to investigate a user
failure. It must not weaken release acceptance or make a failure untraceable.

For public/user-facing distributions, keep normal workflow menus focused and
hide diagnostic/developer actions by default. If an owner explicitly requests a
diagnostic-enabled Production package, expose that choice in build metadata and
the About/Diagnostics surface; do not silently change the executable's support
or security contract. Clean-profile distribution must continue to exclude owner
data, credentials and deployment identity in every build profile.
