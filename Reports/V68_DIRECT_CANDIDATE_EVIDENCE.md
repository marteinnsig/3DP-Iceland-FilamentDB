# v68.0.1 Direct Izod and Charpy Entry candidate

## Changes

The owner-requested replacement removes the unaccepted run UI. Each visible material now has a row in each method tab.
Ten instrument readings in kJ/m² feed mean, sample SD, actual CV%, Samples and sample-count Confidence.
The tables reuse the accepted Fast editor, deferred focus, editable-cell navigation and stable row identities.
Settings holds reference geometry (80 x 4 x 10 mm, 2 mm notch, 4 x 8 mm remaining section).
Legacy energy records retain their units and supported recovery/report interpretation. No conversion applies to new readings.
The direct report projection, Help, control coverage and existing authorized disposable CRUD scenario were updated together.
The rejected run selectors, metadata editors and unused refresh adapter were retired; historical storage remains supported.

## Verification

Final Debug and Release builds: zero warnings and zero errors.
Help coverage: 803 discovered controls plus current programmatic ownership PASS. Release documentation and diff audits PASS.
Dependency vulnerability scan: no vulnerable packages reported by configured sources.
Release DLL SHA-256: 419E3961B3DFDFD58E28DF683650C7F1D86B5CEB106D1A2A2FB98778E5AED4D4.
Full Data Verification: 472/472 PASS on final candidate profile 20260921133555-613feb80.
Final disposable CRUD completion and exact business-state recovery: PASS.
Baseline/final business SHA-256: 177F36F2E04451F190D9935B7FEA3A0B28B1BD7D392B9AD4E560FFD386C23D5E.
Prior direct-entry runs 20260921132947-7064cca1 and 20260921133244-b986cbb6 passed CRUD and exact business-state recovery.
Retained evidence location: .private/v68-direct-impact.

Populated Izod and Charpy screenshots were inspected; both show one compact automatic row per material and no run setup UI.
Automated callbacks cover independent method values, blank/NB, invalid-input rollback, summary values and SQLite restart.
Calculation/persistence tests include >100 readings, zero, ten slots, legacy separation and Excel recovery.
Physical single-click immediate typing and keyboard usability remain owner acceptance requirements.

## What to Test

Open the normal Release application and navigate to Measurements & Testing > Izod Measurements, then Charpy Measurements.
On an intended measurement cell, click once, type immediately, and use Tab, Shift+Tab and arrows.
Expect the first character immediately and predictable movement without focus loss; summary columns remain read-only.
Use actual intended measurements, or a disposable profile for the 10/20 example. Do not overwrite permanent data with examples.
Exact read-only and entry/restart instructions: Docs/V68_DIRECT_IMPACT_ACCEPTANCE.md.
Owner runtime acceptance is pending; canonical release remains v67.0.8. No commit/push or live publication occurred.
