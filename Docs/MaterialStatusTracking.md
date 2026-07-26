# Material Status Tracking

## v47.0.3 Candidate

Coverage status now supports stable collection-ID and MaterialID ownership.
Existing collection-title/material-label entries remain supported through exact
fallback and can be bound only through a previewed, default-No, unique exact
workflow. Ambiguous and unmatched entries are never changed automatically.

New Apply Status operations write stable identity while retaining readable
title and label snapshots. Clearing a selected collection removes both its
stable entries and its exact legacy entries.

## v26.6

Material Status Tracking extends the Collection Dashboard beyond simple Open/Published coverage.

## Statuses

Supported workflow statuses:

- Untested
- Tested
- Video Planned
- Filmed
- Edited
- Published

Existing entries with no status are shown as Open.

## Visible App Features

- Material status dropdown.
- Apply Status button.
- Collection Dashboard status breakdown.
- Row-level material status list.

## Workflow

1. Create or select a Material Collection.
2. Choose a status from Material status.
3. Click Apply Status.
4. Click Collection Dashboard.
5. Review the status breakdown and row-level material status list.

## Notes

Status is stored locally in the same coverage tracking storage used by the Collection Dashboard.
