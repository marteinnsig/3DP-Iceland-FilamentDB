# Scoring Engine / Website Radar Alignment

Version: v15.1

The application scoring engine now mirrors the radar profile used on the website.

## Radar axes

1. Tensile
2. Impact
3. Stiffness
4. Consistency
5. Layer Adhesion

## Current app-side formulas

These are first-pass chart inputs. They are not intended to replace the raw measured values.

- Tensile Score: average of available flat/upright tensile MPa, normalized to a temporary app reference.
- Impact Score: average of available flat/upright impact kJ/m², normalized to a temporary app reference.
- Stiffness Score: stiffness modulus MPa, normalized to a temporary app reference.
- Consistency Score: same concept as the website consistency chart: average CV% with a small sample-count penalty.
- Layer Adhesion Score: upright tensile / flat tensile ratio, clamped to 0–100 until full dataset normalization is available.

## Important note

The website radar normalizes against the currently visible dataset. The Windows app currently calculates the selected-material radar inputs independently. A later milestone should import or calculate the full comparison dataset so the radar chart can use the same dynamic normalization behavior as the website.
