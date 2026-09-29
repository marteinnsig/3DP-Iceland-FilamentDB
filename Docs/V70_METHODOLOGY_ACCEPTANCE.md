# v70.0.4 Izod and Charpy Methodology acceptance

## Búið / Breytingar

Both website Methodology and Engineering Whitepaper now cover Izod and Charpy separately:
flat 80 x 10 x 4 mm specimens, 2 mm notch, geometric 32 mm² remaining section, direct kJ/m² input,
operator-reported nominal 2.75 J / 2 J hammers, ten reading positions, NB, statistics and fixed 80-point-reference policy.
Method families ISO 180 / ISO 179 are identified without asserting an unverified edition or certification.
Internal construction, notch radius and calibration are not invented. Legacy Impact remains separate historical evidence.
The website source and native Documentation Engine are separate; both updated and protected by a shared coverage gate.
Whitepaper contents, matrix, revision history and consistency text updated. Stable PDF route/filename retained.
No measurement or score calculation changed; no database migration or editing surface added.

## What to Test

1. **Where:** `.private/v70-methodology/preview/methodology-preview.html` in your browser.
   **Action:** use the Izod and Charpy links and read their sections.
   **Expected:** separate descriptions, correct geometry/units and 80 kJ/m² score reference; no legacy conversion applied.
   This is an isolated local rendering of the exact embedded portal, not a live website publication.
2. **Where:** companion `3DPIceland_Labs_Mechanical_Testing_Methodology_v1.0.pdf` in the same preview folder.
   **Action:** read pages 2–3 and 17–19.
   **Expected:** contents/matrix include both methods; 6a Izod, 6b Charpy and 6c statistics/scoring match the portal.
3. **Where:** candidate `.private/v70-methodology/App/3DPIcelandFilamentDB.exe` → Help → Export Engineering Whitepaper…
   **Action:** export to a local review folder.
   **Expected:** the same updated 40-page whitepaper, stamped with v70.0.4.
   **Do not click:** Production, FTPS, Restore, Update or Recalculate; review needs none of these actions.
4. **Where:** candidate → Website Export → Generate Preview (when the normal preview prerequisites are satisfied).
   **Action:** generate a local preview and open Methodology.
   **Expected:** updated sections and companion PDF generated together. Publishing the live website is a separate step.

Normal Release stays v70.0.3 pending owner acceptance. No live website publication, commit or push yet.

Owner acceptance 2026-09-29: approved the updated methodology and whitepaper and requested continuation.
Exact v70.0.4 tested bytes promoted to App/FilamentDbApp/bin/Release/net9.0-windows without rebuilding.
All 54 files match the accepted manifest; previous Release retained at .private/v70-methodology/pre-promotion-release.
DLL SHA-256 B7B813A25CC8AC29B3D27040F5E486F98FE0DD6D45FDC7DEEF0BCD47581E44CA.
Debug/Release, 482/482 Full Verification, final reports (2,103 artifacts), Help/docs and PDF/portal visual checks PASS.
Final reports profile 20260929130815-41c0b3f6 preserves exact business state; prior failed run remains retained.
Normal website export now regenerates the accepted portal and companion PDF together. Live website, installer/update
packages and FTPS remain unchanged; this local application acceptance is not evidence of external publication.
