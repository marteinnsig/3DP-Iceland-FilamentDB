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
```

## Open findings

Date: 2026-07-24
Area: Verification Center / System Diagnostics exports
Type: Workflow friction / UI polish
Severity: Minor
What happened: Verification Center and System Diagnostics both export files using the generic `3DPIceland_FilamentDB_Diagnostics_YYYYMMDD_HHMMSS.txt` filename, even though they contain different report types. This makes attached files easy to confuse.
Expected behavior: Use terminology consistently across the window title, export action, document header and filename. Verification Center should export `3DPIceland_FilamentDB_Verification_YYYYMMDD_HHMMSS.txt`; System Diagnostics should export `3DPIceland_FilamentDB_System_Diagnostics_YYYYMMDD_HHMMSS.txt`.
Steps to reproduce: Export one file from Verification Center and one from System Diagnostics, then compare their filenames.
Screenshot / export / report attached: Observed in the v44.5.1 runtime diagnostics/verification handoff.



## Triage categories

- Bugs: incorrect behavior or broken workflow.
- Workflow friction: too many clicks, confusing flow, repeated manual work.
- UI polish: readability, sizing, labels, layout.
- Website ideas: improvements visible to YouTube viewers or public users.
- Reporting ideas: improvements to report clarity or sharing.
- Data issues: inconsistent material metadata, missing links, missing measurements.



improvment:
I had this idea if we could implement a Price field in USD for each material, even though I buy it in different currency's I would calculate the USD price if need, 
then as I note spool waight, we could display price per 1KG for all materials independant on spool sise, and use that thata in the charts/report like 
"Price per Impact strengh" or something like that, we have to work on this idea to make the most of this data point.


Workflow friction:
þegar ég er að setja inn efni, þá er röðin með hvítum texta, ef gildi er minna en 10 t.d í tensile, þá sérst ekki hvítir stafir á gulum bakgrunn. -  þetta þarf að laga

Workflow friction:
fæ ekkert vit í AI assistance, - hafði valið bambu labs framleiðandia í materials tab í gegnum Leitina. og bjó til collection, það innihélt ekki bara bambu labs efni, heldur öll efni 0-176, 
sama ef ég hafði filterað eftir manufactures í materials
fæ í raun ekkert vit í neitt af því sem er að gerast í þessum ai assistance tab, - þetta þarf að þróa áfram ´serstakletga.


improvment:
catagory rankings  - sýnir aðeins top 10, - mundi vilja sjá allan listann eða allavega fleiri options í rows per group, 5 , 10, 50,100,all, ég er kanski að miskilja hverning þetta á að virka.



improvment:
appið opnast alltaf í frekar litlum glugga, - mætti vera 2x á hæð og 1.5 x á breydd, eða eitthvað álíka., 
það væri t.d mjög þægileg að glugginn væri það stór til hliðana að hann sýndir allt svæið sem þarf að vera sýnilegt þegar ég færi inn mælingar í tensile og impact

tech notes sem eru þegar ég er að færa inn mælingar, mættu vera í dálk rétt á eftir innslegnum tölum í flat 10, á eftir bilinu sem er þar á eftir, en ekki lengst til hægri eftir að útreikningar eru komnir.

improvment:
útfærsla á experimental mælingum eins og t.d styrk á prenti eftir mismunadni hitastigi, eða nozzle size sem dæmi, - að þegar ég addas material, þá sé dálkur þar sem ég get hakað í "Experimental", 
og þá sé sú lína búin til í impact, tenile and stiffness, og ég set inn þær mælingar eins og um venjulegt efni er að ræða, - þurfum svo að hafa dálk til að gefa til kynna hvaða experimental mælingar eru í gangi, 
það gæti verið dropdown menu item í material listanum, komandi úr settings manager tab, þar sem ég skilgreini tilraunir t.d "Different Nozzle Size" "Diffrent Tempurature" "Different Extrucion with" "Different outer walls"
og hvað annað sem mér mundi detta í hug síðar að gera samanburðarmælingar á.Svo þarf að vera dálkur þar sem ég set gildið á mismunandi mæligum. t.d með hita, þá sé reitur yfir "temp" , 
og þannig kolll af kolli fyrir þær mælingar sem ég er að gera hverju sinni, - ef ég bæti við tilraun síðar, þá þarf að koma dálkur í materials tabfyrir þá tilraun, svo ég geti slegið inn 
þau gildi sem er verið að eiga við hverju sinni. - Fyrir venjulegt html export, þá eru efni merkt "experimental" ekki sett sjálfkrafa á heimasíðuna, en þar gæti verið filter, sem er default á 
"standard mæling", en hægt að velja í dropdown "Experimental" og þá færi filterað niður á efni sem væru í þeim flokki, þá kæmu eingöngu þau efni fram í súluritum, og það þyrfti að vera sér súlurit 
fyrir hverja tilraun sem er gerð, - svo það þyrfti að byggja upp html'ið með þetta í huga samhliða innleiðingu á þessari breytingu. 

improvment:
 gera icon með transparent bakgrunn

Bug:
efst uppi hægra megin er texti sem segir 176 materials | 26 colums -  þetta uppfærist ekki, 
undir haus í forritinu er líka texti sem segir "loaded 176 materials and 26 clolums from local cache, import from excel to refresh, - þetta endurspeiglar ekki fjölda efna í grunninum núna. -
 verification center er rétt og segir 184 efni í material, tensile, impact og stiffness, svo kerfið 
veit hvað það eru mörg efni í grunninun.


improvement:
Gera installer fyrir forritið, sem setti það inn í rétta möppu undir c:\Program files, og þar væru allar tilheirandi skrár, kæmi upp í install ferli hvar á að installa, og hvar á að geima sqllite grunninn, koma deault upp með documents/3dp Iceland app  
eða eitthvað álíka,  s.s ekki hafa gagnagrunninn í program files, heldur í möppu sem er líkleg til að vera afrituð með onedrive

improvement:
rezize á dálkum, should not be as large as they are, should trim down to the text in it or the label of it., website colum should be shorter by default to just the lable size.

BUG-Chrash!
krass, -  var nýbúinn að setja inn values, forritið var búið að vera í gangi í 1 sólarhring,
fór svo í material, og ætlaði að setja inn variant á efni, og þá krassaði það - gögn úr imact sem ég var að setja inn héldu sér.

improvement:
ai api connection

improvement:
hours of testing, printing, and material useage, sample coubt

improvement:
website export - "Used Bundled" what is that, and how do I update the bundled, it should update with the latest html template file used, so if I don't change the html, I can just press use bundled instead of reading the current html, 
The last uploaded html should be  stored in the sql database, not in the build it self.

improvement:
date of entri of mesuraments

bug:
filter í materials, - if I select "not tested" and all filaments are tested, it shows all filaments, it should show none.

bug:
at the bottom, it shows "Engine: 167 materials, 5 sheets, 880 rows 27884 cells" - this is not updateing automatically, - what is this used for anyway?, as to the right there is a list of materials in each test, that is updated.

improvement:
I don't think I will every use "Manufacturer SKU" ,as I link to the product page, and sku information is not given by resellers in most cases.


improvement:
website html fix, - tensile - flat  is tensile, upright is layer adheesion, I'm always explaining in videos that, maybe they should just say "Tensile" and "Layer Adhesion",

Bug:
In Material Detail, - data is not updated  for tested, in tensile ,in impact, in stiffness



improvment: in Materials, I don't think I will save a name of the thumbnail for the videos
i manage them seperatly in my youtube files folder

BUG: 
Dashborad Insights, - does not update as I add more materials , needs to by dynamic

Status bar at the bottom, still says Engine: 176 materials, does not update, same as the status 
on top under the logo, - maybe this is not needed at both places - but later in the bottom line
it states 197 materials, -

qustion;: what does "validation" mean for each line of material?

Bug: in material report, the reports says on top "Total Materials: 176" not the actual number
on the report, the logo is displayed 2 times on the top

on manufacturers html export, - it does not update "materials in database", 

Add "Purchased from" to the material tab

Cost per KG calculation based on cost and spool weight

selecting from dropdown menu for Currency is to many clicks,  first click select the rows
then nex click shows the dropdown, then I have to click the down arrow, and then select curreny
clicking on a cell, should automatically select the row/cell and alowing me to show the drop
down menu right a way so I can select the currency. This is basically the same function on anyway
cell that has user input, it should allow me to click on a sell, and right away
enter the value or text in it.

landed currecny should default to ISK

í staðinn f. að setja inn handvirkt landed cost, gera þá "hvað kostar evran" í sendingunni, og reikna
út sjálfkrafa landed cost

typing in "filter" is really slow to show resaults

improvement:
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
Use a professional, human-readable backup filename such as
`Product-YYYY-MM-DD_HHmmss.bak` instead of exposing a plain `.sqlite` filename.
The `.bak` extension is a presentation/naming convention only: the file remains
a canonical SQLite backup and must retain integrity/schema verification,
collision-safe timestamps, explicit restore handling and backwards-compatible
discovery/restore of existing `.sqlite` backup files. Do not rename or delete
existing backups automatically.

improvement — user-reorderable workflow columns (v44.2 candidate):
Allow users to drag and reorder columns in the Materials, Tensile, Impact and
Stiffness tabs. Persist column order as machine-local UI state, keyed by stable
bound field identity rather than column index. Preserve required fields,
validated frozen-column behavior and backwards compatibility when columns are
added or removed. Invalid/stale layouts must fall back safely, and each grid
should offer Reset columns to default. Do not store presentation layout in the
canonical SQLite engineering backup.

improvement — optional clean uninstall (v44.3 candidate):
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
