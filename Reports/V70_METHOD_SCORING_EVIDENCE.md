# v70.0.0 Method-aware Engineering Scoring evidence

Owner authorizes the scoring revision and explicitly defers numeric Izod/Charpy references until more materials
have been measured. The unconfigured versioned reference policy is therefore intentional. No measured values,
raw samples, database schema or owner database are modified by this increment.

## Scoring contract

- Five equal Overall families: Tensile, Stiffness, Consistency, Layer Adhesion and combined Izod/Charpy.
- Both independent method scores must exist for their combined family; all five families must exist for Overall.
- Legacy Impact has no modern score and cannot affect Overall through mean, CV or sample count.
- Instrument kJ/m² and statistics remain visible. Missing calibration/measurements are unavailable, never zero.
- Fixed references never depend on filters, group size or maximum observed material. No singleton receives an automatic 100.
- App, website and report projections share canonical scores, policy identity and coverage.
- Radar gaps distinguish absent evidence from measured zero. Incomplete profiles receive no invented Overall ranking.
- Current advice uses the modern impact family and does not interpret missing data as weak performance.
- Historical saved planning snapshots and raw legacy charts retain their supported compatibility ownership.

## Verified implementation

- Final application Debug and Release builds: PASS, zero warnings/errors.
- Candidate: `.private/v70-impact-scoring/App`, 54 files; inventory `.private/v70-impact-scoring/app-files.json`.
- Final application DLL SHA-256: `5FE101419DE3946437339EA791280710422373249ED3D8CFB2645979057A7107`.
- Core fixed-reference/Overall and advisor linked-source harnesses: PASS.
- JavaScript syntax, canonical score/null/group projection, label parity and radar missing/zero probes: PASS.
- Disposable smoke profile `20260928161601-285e91b5`: Full Verification 480/480 PASS.
- Help coverage/drift: PASS, 809 controls, 430 columns; release documentation and dependency vulnerability gates PASS.

## Investigated intermediate failures

Initial smoke `20260928161248-14b85f46` failed 13 checks. Root causes were stale seven-axis advisor assertions
and an outdated required website-transform marker; downstream release gates cascaded. Updated assertions preserve
meaningful modern-axis evidence and strengthen canonical website score/coverage requirements. Rerun passed 480/480.

Reports profile `20260928161932-4269fffc` passed 480/480 Verification but stopped on an unidentified owned window
during export. It remains FAIL and its profile/evidence are retained. No unexpected-window policy was weakened.
AutomationRunner now includes bounded owned-window handle/class/type/ID/text diagnostics in retained failure evidence;
password text is redacted and no extra input or allowlist entry was added. Runner Debug/Release pass without warnings.

The first reports build DLL was `E21B201D642876B08E0BAF727619FDB524B6045FE93E6C14120175DDCA27141E`.
The final app additionally retains genuine zero contextual recommendations and tests zero versus missing through the caller.
Reports rerun `20260928162838-5372c4d9`: PASS, 480/480 Full Verification and 2,103 catalog/root artifacts verified/hashed.
Its application DLL was `10C0CAD8D795616E2BA86B9E7D85F1BD2FE219C38B9971D1036084AE94835FD9`.
The unidentified window did not recur; its original run remains recorded as a failure, not retroactively passed.
Final application adds only explicit pending-reference/ranking status messages after this export; score/export logic is unchanged.
Final exact-byte smoke `20260928163536-620b51f2`: PASS, 480/480 Full Verification and exact business-state recovery.

Exported MAT0001 keeps legacy Flat 39.80106468968572 and Upright 6.4941805847971255 kJ/m², identical to v69.
Its modern Overall/legacy scores are unavailable, with explicit 4/5 families and 0/2 measured/scored method coverage.
Reports baseline/final business-state SHA-256 matches:
`2B09F8539C2B3481ED1969B28F513D5E245C53B08765BAE5F99D4354F55DE4CF`.
Canonical seed SHA-256 remains `CEF1F9D5142578BBFE91D37A79BF11A6255CE550B8CA388D8582BF2D7D332D09`.

## Delivery boundary

v69.0.0 remains the accepted normal Release application. v70 is a runnable candidate awaiting owner runtime/readability
acceptance. No ZIP, Production promotion, FTPS, website publication, commit or push is included in this handoff.
Owner checklist: `Docs/V70_METHOD_SCORING_ACCEPTANCE.md`.
Owner acceptance 2026-09-28: approved the radar and requested promotion to the accepted application. Exact tested v70.0.1 bytes (54 manifest files) are now in App/FilamentDbApp/bin/Release/net9.0-windows; DLL SHA-256 F34FB9B2C5B56A1959CD877DDDF3914ECCEFB0F10F3AE723F0C227279EEFE80F. Full Verification 481/481 and reports 2,103 artifacts PASS; no rebuild or data mutation. Numeric method references remain deliberately deferred. Application acceptance does not publish installer/update packages; those packages have not received exact-byte acceptance.

