# v68.0.3 — Izod and Charpy Ranking Integration

Owner runtime accepted 2026-09-21; commit/push authorized. No ZIP or live publication.

Overall is the equal-weight mean of available Tensile, Impact, Izod, Charpy, Stiffness, Consistency and Layer Adhesion
scores. Thermal remains a separate axis. Missing components are omitted, not zero. A measured zero remains evidence.
Izod and Charpy retain independent same-method cohort maxima; kJ/m² raw readings are never pooled or converted.
Consistency now includes their actual CV percentages and numeric sample counts through the existing calibration.
Context-specific recommendations keep their prior impact-family weight, shared across available Impact/Izod/Charpy scores.
App legacy reference scales and website visible-cohort scales retain their existing distinction.

Rankings and CSV include both methods. Category Rankings adds Best Izod and Best Charpy; Awards adds Best Izod Material
and Best Charpy Material. Overall-dependent ordering, value, recommendations and alternatives use the updated score.
Dashboard, AI context and YouTube research include independent evidence and leaders; story axes, outliers, comparison
gaps, titles and thumbnails can use either method. Advisor coverage now has seven mechanical axes.
Saved production-queue descriptions and old score snapshots remain historical; additional method scores are derived
from current MaterialID-linked readings on load. No historical readings or descriptions are invented or rewritten.

## Owner read-only inspection

1. **Where:** restart the normal Release application, then Materials.
   **Action:** select a material with saved Izod or Charpy readings.
   **Expected result:** existing readings and input behavior remain available.
   **Do not click:** Delete, Restore, Production, FTPS or Recalculate.
2. **Where:** Rankings Dashboard > Ranking metric.
   **Action:** select Izod, then Charpy, then Overall.
   **Expected result:** independent method rankings; missing methods do not receive invented scores. Overall includes both.
3. **Where:** Category Rankings > Category, then Awards & Winners.
   **Action:** select Best Izod and Best Charpy; inspect the corresponding method awards under Performance awards.
   **Expected result:** each winner is based on that method, not the other impact test.
4. **Where:** Dashboard Insights.
   **Action:** read IZOD AND CHARPY EVIDENCE and the material opportunities.
   **Expected result:** available counts, independent leaders, raw kJ/m² and normalized scores match saved evidence.
5. **Where:** YouTube Research > Generate YouTube Research.
   **Action:** generate local research and inspect candidates, comparison gaps and thumbnail explanations.
   **Expected result:** measured Izod/Charpy strengths can influence local ranking, evidence and story suggestions.
   **Do not click:** any external send, publication or Production action.
6. **Where:** AI Assistant > Generate Full Assistant Brief.
   **Action:** click Generate Full Assistant Brief for the selected visible scope.
   **Expected result:** Izod/Charpy evidence is included and available Overall affects prioritization.
   **Do not click:** Send/OpenAI provider actions; this checklist does not require an external request.
7. **Where:** Material Detail > Charts and Analytics, then Help (F1).
   **Action:** inspect Overall, separate axes and Help descriptions of the seven-component formula.
   **Expected result:** both methods are represented; missing data is clearly distinguished from zero.

The disposable tester selects both methods and categories, verifies calculations and persistence, and retains evidence.
Automated checks do not replace owner layout/readability acceptance.
