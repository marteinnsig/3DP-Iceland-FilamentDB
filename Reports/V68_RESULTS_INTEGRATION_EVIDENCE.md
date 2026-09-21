# v68.0.2 Results Integration evidence

2026-09-21. Implemented candidate; v68.0.1 owner input acceptance is recorded separately.
Owner result/status/radar and Help acceptance for v68.0.2 remains pending.

## Implementation and boundaries

Derived In Izod/In Charpy flags participate in six-method coverage with Tensile, Impact, Stiffness and Heat.
No methods means Not tested; one through five means Partially tested; all six means Fully tested.
Numeric zero is evidence; blank, NB and notes-only rows are not. Schema remains 45; historical energy records are retained.
Direct raw kJ/m² means, sample SD, actual CV%, Samples and Confidence remain independent by method.
Material Detail shares the selected-material summary across all nine inner tabs, with Mechanical statistics and
Charts/Analytics/Compare evidence. WPF, local reports and generated website views expose eight separate radar axes.
Each method uses its own maximum within the relevant cohort; public references use only the opted-in public cohort.
Missing values remain unavailable; Overall retains its existing formula.
Accepted Fast input lifecycle remains intact; expensive downstream refresh is coalesced when leaving measurement tabs.
Read-only review of data mapping, detail ownership and downstream presentation informed integration and gate corrections.

## Verification

- Debug and Release builds: zero warnings and errors, including the final wording-only rebuild.
- Full Data Verification: 475/475 PASS; disposable CRUD, persistence/restarts and exact business recovery PASS.
- Profile: `20260921142945-cdff34bb`; canonical seed unchanged; no owner database fixture mutation.
- Tester covers independent membership, zero/NB/blank/clear transitions and saved selected-material summaries.
- Smoke/CRUD discovers shared detail and method cards and retains Mechanical/Charts/Analytics screenshots.
- Help coverage: 805 candidates, 427 grid columns and 12 runtime surfaces PASS.
- Generated website browser probe: ten method selectors, eight-axis radar, null preservation and no script errors.
- Dependency vulnerability check: no vulnerable packages reported. Documentation and diff gates PASS.

Baseline/final business SHA-256:
`2B09F8539C2B3481ED1969B28F513D5E245C53B08765BAE5F99D4354F55DE4CF`

Canonical seed SHA-256:
`CEF1F9D5142578BBFE91D37A79BF11A6255CE550B8CA388D8582BF2D7D332D09`

Release DLL exercised by the full functional run:
`227DDDA4CE9D9AFD67C03AB6B42C858B20C4BFC7BAA157733A462C5BAB41C5FD`

Final Release DLL after correcting the Analytics eight-axis subtitle and missing-result explanatory text:
`96BC947FE0CD6CD83D248948EA822DB11F7F666259C510FDE8390F145F892417`

The final wording-only rebuild is not claimed as the exact bytes used by the preceding full CRUD run.
Screenshots were inspected for layout, independent axes and missing-result display; they do not claim populated
owner measurements. Numeric direct-result presentation is covered by deterministic disposable callbacks.
Owner visual and keyboard usability acceptance remains distinct from automated checks.

## Retention and handoff

Non-database run evidence and browser probe sources/results are retained under `.private/v68-results-integration`.
Disposable profiles remain available for investigation. Build-only `artifacts` contents are cleaned after retention.
The normal `App/FilamentDbApp/bin/Release/net9.0-windows` runnable directory is updated and preserved.
No ZIP delivery, commit/push, Production promotion, website mutation or FTPS publication was performed.
Exact owner inspection steps: `Docs/V68_RESULT_INTEGRATION_ACCEPTANCE.md`.

## Owner display correction, 2026-09-21

Within pending v68.0.2: Materials places In Izod then In Charpy directly after In Flexible, including saved layouts.
General > Test Information includes In Izod, In Charpy, In Heat and In Flexible with the existing coverage fields.
Other is excluded from grouped presentation; BuildAllFields retains complete source inspection and no data is deleted.
Compare retains only Izod/Charpy means in kJ/m²; full statistics remain in Mechanical and measurement tabs.
Help and the existing Verification contracts cover field grouping, default/migrated order and comparison row scope.
No new input surface or automation authorization is introduced; existing smoke owns Full Verification and detail discovery.
Debug/Release: zero warnings/errors. Owner visual confirmation remains pending using the revised acceptance checklist.
The first smoke run found an obsolete expected column order and its dependent Settings gate; both were corrected.
Final Release DLL SHA-256: A99BE95C1F707243BE8B7E046D2F51663A021A5FDC2EAA238912C36703EAB5D9.

Final smoke profile 20260921145446-51b35ed6: PASS, 475/475 Full Verification, exact business-state recovery.
Evidence retained in .private/v68-results-integration/display-correction. Help/docs/diff gates PASS.

## Existing saved-layout migration correction, 2026-09-21

Owner screenshot showed Izod/Charpy still appended after Validation. The migration helper had been corrected, but
FastMaterialsLayoutContractVersion remained 2, so existing v2 preferences skipped it. Version 3 activates migration
on the next app start while preserving saved widths and the remaining column order. No manual reset is needed.
The existing integration Verification gate now exercises a complete old layout with both columns appended,
checks the final applied order and width preservation, and checks idempotence and migration version eligibility.
Help remains accurate; no new controls, input handlers, IDs or authorization boundaries change.
Debug and Release builds pass with zero warnings/errors. Owner confirmation of the actual saved layout remains pending.

Smoke profile 20260921145941-3074cf92: PASS, 475/475 Full Verification, unchanged business state.
Release DLL SHA-256: CE107470198434BD94B5DBBFDE9B0B9C129132F923EDCCE20D67EE5212D314EA.
Evidence: .private/v68-results-integration/layout-migration. Help/docs/diff PASS; artifacts remains empty.

Owner closure 2026-09-21: result/display follow-ups accepted with v68.0.3; commit/push authorized.
