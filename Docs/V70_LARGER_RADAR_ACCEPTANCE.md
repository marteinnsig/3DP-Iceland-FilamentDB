# v70.0.3 Larger Analytics Radar

Búið / Breytingar: Analytics radar is uniformly scaled from 430 x 430 to 860 x 860 using a WPF Viewbox.
Its labels, markers and grid scale together. Radar column grows from 470 to 900; outer scrolling remains available.
Existing raw values, coordinates, score calculation, selection and AutomationIds are unchanged.
The stale Analytics description now accurately states fixed 80 kJ/m² and five-family Overall.
No new input field, calculation or persistence workflow. No low-value cosmetic automation added.
Debug/Release: PASS, zero warnings/errors. Help coverage/inventory and release documentation: PASS.
Full Verification 481/481 and reports PASS are retained from v70.0.2; not rerun for cosmetic-only scaling.

## What to Test

1. Where: .private/v70-large-radar/App/3DPIcelandFilamentDB.exe. Action: open it.
   Expected: v70.0.3 Larger Analytics Radar. Do not click Recalculate, Restore, Production, Update or FTPS.
2. Where: Materials -> select material -> Material Detail -> Analytics. Action: inspect radar.
   Expected: radar twice as wide and high, including readable labels and markers; same scores as v70.0.2.
3. Where: Radar score groups. Action: select one row, then Ctrl-click another.
   Expected: both profiles display correctly; table remains accessible beside the larger radar.
4. Where: same tab. Action: reduce window size and use scroll bars.
   Expected: full graph, legend and table remain reachable.

Owner visual acceptance pending. Normal Release remains unchanged; no commit/push or promotion yet.

Owner runtime acceptance 2026-09-29: fixed 80 kJ/m² scores and enlarged radar accepted.
Exact v70.0.3 build promoted to App/FilamentDbApp/bin/Release/net9.0-windows without rebuilding.
All 54 target files match the accepted manifest; prior Release files backed up in .private/v70-large-radar/pre-promotion-release.
DLL SHA-256: 0D5C7B37488BD6124E3D4D31B63191FAD1562CBA2826105500E824D0CFEDA6C8.
Debug/Release and Help/documentation gates PASS. v70.0.2 Full Verification 481/481 and reports PASS remain
calculation evidence; v70.0.3 cosmetic scaling is owner visually accepted, not claimed as a fresh Full Verification run.
No owner-data mutation or external publication. Installer/update packages remain outside this local promotion.
