# Video Pipeline Dashboard

## v47.0.3 Candidate

Pipeline status resolution prefers stable collection-ID and MaterialID coverage
identity. Exact legacy title/label fallback remains available so historical
coverage JSON is not invalidated before explicit binding.

## v26.7

The Video Pipeline Dashboard provides a single AI Assistant view of the full YouTube production pipeline across all saved Material Collections.

## Visible App Feature

A new **Video Pipeline Dashboard** button is available in the AI Assistant Material Collections & Coverage Tracking section.

## Dashboard Sections

- Collection count
- Total tracked material rows
- Published / completed rows
- Remaining rows
- Overall coverage percentage
- Pipeline status summary
- Next Best Actions
- Collection progress

## Statuses Used

The dashboard uses the material statuses introduced in v26.6:

- Open
- Untested
- Tested
- Video Planned
- Filmed
- Edited
- Published

## Next Best Actions

Next Best Actions are generated from the highest-impact status in each collection:

- Edited → ready to publish
- Filmed → ready for editing
- Video Planned → ready to film
- Tested → ready for planning
- Untested → needs testing
- Open → needs workflow status

## Workflow

1. Create material collections.
2. Apply material statuses inside each collection.
3. Run Video Pipeline Dashboard.
4. Use Next Best Actions to decide what to work on next.
