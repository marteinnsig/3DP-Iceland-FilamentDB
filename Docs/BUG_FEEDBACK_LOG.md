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
