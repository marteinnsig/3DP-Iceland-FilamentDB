# Recommendation Engine

## v19.1 Context-Aware Recommendations

The recommendation engine is local and rule-based. It does not call an AI service.

It uses the imported radar-score dimensions:

- Tensile
- Impact
- Stiffness
- Consistency
- Layer adhesion
- Overall profile

v19.0 introduced direct rankings such as strongest, highest impact and most consistent.

v19.1 adds context-aware use cases with weighted scoring:

- Best outdoor material
- Best functional part material
- Best beginner engineering material
- Best impact resistant material
- Best layer adhesion material
- Best stiffness-focused material
- Best consistency-focused material

These are intended as starting points for analysis and video research, not absolute engineering certification.

Outdoor recommendations use material-family suitability plus mechanical scores. They should still be verified against manufacturer datasheets, print settings and real-world environment requirements.
