# Analytics Radar Engine v17.2

This milestone turns the Analytics tab from a table-only framework into a functional radar preview.

## Radar axes

The app follows the same five radar axes used by the 3DP Iceland Labs website:

- Tensile
- Impact
- Stiffness
- Consistency
- Layer Adhesion

## Chart modes

The Analytics tab supports the website-style chart modes:

- All filament samples
- Average by material type
- Average by manufacturer
- Average by product line
- Average by variant / finish
- Average by reinforcement

## Filters

The main left-side filters and global search box also affect the Analytics tab. The radar preview and score table are generated from the currently visible material rows.

## Radar preview behavior

The radar preview renders the highest-ranked visible rows by Overall Profile score. This keeps the first implementation readable while still proving the complete pipeline:

Excel import -> SQLite cache -> scoring engine -> chart mode grouping -> radar preview.

Future milestones can add selectable overlays, hover tooltips, exportable chart images, and comparison-specific radar overlays.


## v17.3 Interactive Analytics

The Analytics results table is now interactive. Selecting one or more rows controls the radar overlay directly. If nothing is selected, the radar shows the top six visible profiles by overall score. If rows are selected, the radar shows up to the first six selected profiles. Use Ctrl-click in the table to compare multiple groups. The Clear radar selection button returns the radar to the automatic top-six view.
