# v70.0.2 verification evidence

Owner confirmed kJ/m² and 80 for both method references, 2026-09-29.
Policy: impact-reference-v2-80-kjm2. Shared production projection applies both references without owner-data mutation.
Changed existing Full Verification tests cover current-policy endpoints, midpoint, real PLA example, capping,
unconfigured compatibility and direct default projection; existing five-family and legacy-isolation checks remain.
No editor, AutomationId or schema change; existing reports scenario and Full Verification own automation acceptance.
Help coverage and release documentation gates PASS. Debug/Release PASS with zero warnings/errors.
Candidate: .private/v70-fixed-references/App; 54-file manifest retained alongside candidate.
DLL SHA-256: 75E2ABD2EAED22B71F8E3C41A2054203408D825CD4FECC5629E8463F8A78E354.
Owner checklist: Docs/V70_FIXED_REFERENCES_ACCEPTANCE.md. Final disposable run pending.

Full Verification 481/481 PASS on disposable reports profile 20260929095532-1aa89104. Exact candidate default-policy -> direct projection -> website payload probe PASS: Izod 40 gives 50, Charpy 80 gives 100; both retain reference 80 and policy version. Source/probe retained in .private/v702-export-probe.

Final reports PASS: 2,103 catalog/root artifacts verified and hashed; baseline/final business-state hash 2B09F8539C2B3481ED1969B28F513D5E245C53B08765BAE5F99D4354F55DE4CF unchanged. Exact 54-file candidate manifest rechecked. Final TXT/JSON/PNG evidence retained under .private/v70-fixed-references/evidence. Owner runtime acceptance pending; no owner-data mutation, normal Release promotion, commit, push or publication.

Owner runtime acceptance 2026-09-29: fixed 80 kJ/m² scores and enlarged radar accepted.
Exact v70.0.3 build promoted to App/FilamentDbApp/bin/Release/net9.0-windows without rebuilding.
All 54 target files match the accepted manifest; prior Release files backed up in .private/v70-large-radar/pre-promotion-release.
DLL SHA-256: 0D5C7B37488BD6124E3D4D31B63191FAD1562CBA2826105500E824D0CFEDA6C8.
Debug/Release and Help/documentation gates PASS. v70.0.2 Full Verification 481/481 and reports PASS remain
calculation evidence; v70.0.3 cosmetic scaling is owner visually accepted, not claimed as a fresh Full Verification run.
No owner-data mutation or external publication. Installer/update packages remain outside this local promotion.
