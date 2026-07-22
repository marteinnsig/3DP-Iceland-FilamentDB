# Website Export

Version: v29.4 WEB-005 - Website Verification Suite

The Website Export tab remains the user workflow for producing website HTML.

The internal data flow is:

SQLite -> Calculation Engine -> Material Summary Engine -> Website Data Pipeline -> Chart/Radar/HTML/Verification services -> index-test.html or index.html

Website Export must not consume raw tensile, impact, or stiffness measurements directly for engineering values. It consumes verified Material Summary output.
