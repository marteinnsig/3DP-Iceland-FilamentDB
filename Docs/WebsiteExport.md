# Website Export

Version: v29.4 WEB-005 - Website Verification Suite

The Website Export tab remains the user workflow for producing website HTML.

The internal data flow is:

SQLite -> Calculation Engine -> Material Summary Engine -> Website Data Pipeline -> Chart/Radar/HTML/Verification services -> index-test.html or index.html

Website Export must not consume raw tensile, impact, or stiffness measurements directly for engineering values. It consumes verified Material Summary output.

Izod/Charpy chart bars use the same hover and click-to-pin interaction as Legacy Impact. Saved public-safe same-method evidence is attached by MaterialID to each DATA row; it includes dates, statistics and comparative-testing limitations previously shown in the static bottom list. The list is removed. Engineering chart values still come from verified summaries. The evidence renderer retains separate runs/conditions/batches and never exports private setup notes.

Filters apply before tooltip membership is resolved. Group views identify constituent material results separately, without claiming pooled specimen statistics. Tab then Enter/Space pins a bar's details; Escape closes them. Pinned long details can scroll. HTML/PDF report evidence remains supported independently.
