# v20.0 Rankings Dashboard

v20.0 introduces a new **Rankings Dashboard** workspace tab.

## Included in v20.0

- Ranking table for mechanically scored materials.
- Ranking metric selector:
  - Overall
  - Tensile
  - Impact
  - Stiffness
  - Consistency
  - Layer Adhesion
- Base material filter.
- Manufacturer filter.
- Result limits:
  - Top 10
  - Top 25
  - Top 50
  - Top 100
  - All ranked
- CSV export for the currently displayed ranking rows.
- Rankings respect the active search/filter state from the Materials tab.

## Scoring source

The dashboard reuses the current EngineeringScoringService profile used by the Analytics, Video Planner, and Recommendations areas:

- Overall Score
- Tensile Score
- Impact Score
- Stiffness Score
- Consistency Score
- Layer Adhesion Score

This keeps v20.0 aligned with the existing radar/recommendation foundation and gives a clean base for future v20.x award/badge logic.


v20.2 - Category Rankings
- Added Category Rankings workspace.
- Added winner-focused lists by category, base material, and manufacturer.
- Added Category Rankings CSV export.
