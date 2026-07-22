# Coverage Tracking & Collection Dashboard

## v26.5.10

This build fixes collection row identity.

## What Changed

Material Collections now store row-level material labels instead of collapsing similar display names.
Where available, Material ID is included in the saved material label so repeated materials, colors and variants remain separate rows.

Example:

- [private-material-id-removed] — Prusa3D Buddy3D PLA Silk Black
- [private-material-id-removed] — Prusa3D Buddy3D Ice Blue PLA Silk Blue

## Workflow

1. Delete the old affected collection.
2. Filter the Materials tab.
3. Save Visible as Collection again.
4. Load Collection Brief.
5. Click Collection Dashboard.

The Collection Brief and Collection Dashboard material counts should now match.
