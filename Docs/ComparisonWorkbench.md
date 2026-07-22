# Material Comparison Workbench

Milestone v16 adds the first desktop comparison workflow.

## Purpose

The Compare tab is intended to support video planning and material analysis by comparing up to four filaments side by side.

## Compared values

Measured properties:

- Tensile Flat
- Tensile Upright
- Impact Flat
- Impact Upright
- Stiffness

Website radar / scoring inputs:

- Tensile Score
- Impact Score
- Stiffness Score
- Consistency Score
- Layer Adhesion Score
- Overall Profile

## Notes

The Best column currently identifies the highest available value in each row. This is appropriate for most mechanical performance rows in this first version, but future versions may add context-aware comparison rules, annotations, radar overlays, and chart visuals.


## v16.1 Comparison Table Polish

- Selected material names are now used as comparison table headers.
- The winning value in each row is highlighted.
- Non-winning values show percentage delta compared with the best available value.
- The Best column remains as a quick text summary, but the highlighted cells are the primary visual guide.
