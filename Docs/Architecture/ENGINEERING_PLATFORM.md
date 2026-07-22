# Engineering Platform

## Responsibility

The Engineering Platform owns all engineering calculations and produces verified Material Summary outputs.

## Services

- StatisticsService
- RatingService
- ResultsService
- Material Summary calculation through ResultsService.CalculateMaterialResults

## Inputs

- Native material rows
- Native tensile rows
- Native impact rows
- Native stiffness rows
- Calculation settings

## Outputs

- Verified tensile results
- Verified impact results
- Verified stiffness results
- Verified Material Summary rows

## Rules

- Engineering calculations exist in exactly one place.
- Raw measurement rows are calculation inputs only.
- Downstream systems consume Material Summary, not raw measurements.
- Every migrated calculation must be visible in Verification Center before it is considered complete.
