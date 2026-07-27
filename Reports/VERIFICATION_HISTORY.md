# v53.0.4.4 - Diagnostics, Help and Final Acceptance

Date: 2026-07-27

- Debug/Release application, AutomationRunner and updater builds pass with zero warnings/errors.
- Final read-only smoke `20260727232314-5c3961bc` passes 392/392.
- Landed-cost, migration, recovery, Clean, Reports, corrected CRUD and corrected updater scenarios pass.
- Every applicable scenario returns exact normalized business state or its governed clean baseline.
- Diagnostics contains only aggregate landed-cost snapshot counts and never executes migration/recovery.
- Production, FTPS, owner database and live ECB remain blocked.
- Owner diagnostics confirms 200 Materials, two uncalculated Purchase Orders,
  two legacy Inventory snapshots and no disposable authorization.
- Owner accepts Help/readability and Full Data Verification 392/392.
- Schema remains v38; v53.0.4.4 is canonical and v53.0.5 is current.

# v53.0.4.2 - Disposable Landed-cost Lifecycle

Date: 2026-07-27

- Debug/Release application and AutomationRunner builds pass with zero warnings/errors.
- Documentation/static and NuGet vulnerability gates pass.
- Lifecycle profile `20260727222431-7383588f` passes 390/390.
- Read-only smoke `20260727222716-fdeff6d3` passes 390/390.
- Default, cancellation, override, calculation, downstream, cleanup and absence checkpoints pass across restarts.
- Database and normalized business-state hashes match exactly before/after.
- Owner runtime confirms 200 intentional `MAT` identities, zero automation residue and Full Data Verification PASS.
- Production, FTPS, updater, live ECB and owner-data mutation were excluded.
- Schema remains v38; v53.0.4.3 is canonical and v53.0.4.4 is the current authoritative increment.

# v53.0.4.1 - Landed-cost Acceptance Contract and Automation Surface

Date: 2026-07-27

- Debug/Release application and AutomationRunner builds pass with zero warnings/errors.
- Documentation/static gates and the NuGet vulnerability scan pass.
- Disposable Release smoke `20260727215805-25c18520` passes 389/389.
- The v53.0.4.1 exact authorization and control-surface gate passes.
- Database and normalized business-state hashes match exactly before/after.
- Owner runtime accepts Purchase Orders and diagnostics evidence
  `3DPIceland_FilamentDB_System_Diagnostics_20260727_220416.txt`.
- Production, FTPS, updater, live ECB and owner-data mutation were excluded.
- Schema remains v38; v53.0.4.2 is the next authoritative increment.

# v52.3.3 - Bounded Owner Live Evaluation and Closure

Date: 2026-07-27

- Five owner-approved live calls complete; four score 12/12.
- One comparison call scores 10/12 and fails mandatory full grounding and
  no-invention because a seven-ID cohort was called eight records.
- Exact-request repeat stability passes with the same request SHA-256.
- Current project Usage reconciles at seven requests, 13,461 tokens and
  USD 0.37; the owner accepts the model as Provisional, not pinned.
- Debug and isolated Release builds pass with zero warnings/errors.
- NuGet vulnerability scan reports no vulnerable packages.
- Release documentation and Help control coverage audits pass.
- Disposable Release smoke `20260727182043-ab84a0eb` passes 385/385 with
  zero fail/N/A and exact logical/business-state equality.
- Production, FTPS, owner credentials and live OpenAI Generate remain blocked.
- Runtime remains v52.3.2; schema remains v37; parent v52 is complete.

# v52.3.2 - OpenAI Operational Evidence and Failure Harness

Date: 2026-07-27

- Adds a deterministic scripted transport and safe-failure Verification gate.
- Proves success usage/request IDs, safe 401 handling, timeout classification,
  refusal/incomplete/malformed/no-output rejection and forty-row governance.
- Proves operational evidence excludes secrets, raw payload/response, planning
  notes and material identity.
- Proves exact payload preview output cannot be saved as an AI session.
- Release smoke `20260727165007-99840438` passes 385/385 with exact logical and
  business-state equality.
- Clean Readiness `20260727165051-713aecf4` passes 285/285 applicable plus
  100 N/A, controlled restart and exact state equality.
- Post-correction smoke `20260727170133-15d13fb7` passes 385/385 and exact
  business-state equality.
- Reasoning-budget and safe-diagnostic correction smoke
  `20260727170841-4473c154` passes 385/385 with exact state.
- Canonical Release rerun `20260727171213-558ca0ca` passes 385/385 with exact
  state.
- Owner live request completes at the 40-material maximum with secret-safe
  timing, token and request-ID evidence.
- State-sensitive Copy Operational Evidence correction smoke
  `20260727172300-de072a18` passes 385/385 with exact state.
- Canonical Release confirmation `20260727172502-e8736f1b` repeats 385/385
  with exact state.
- Owner validated-advisory Save Session and final Full Data Verification
  385/385 pass on 2026-07-27.
- Schema remains v37; v52.3.2 is canonical and runtime accepted.

# v52.2.0 - Read-only OpenAI Responses Pilot

Date: 2026-07-27

- Exact request preview is deterministic and performs no network request.
- Payload uses `store=false`, no tools/files/web search and an eight-field
  material allowlist capped at forty canonical visible MaterialIDs.
- Strict response parsing rejects unknown evidence IDs.
- Disposable automation may preview but cannot read credentials or generate.
- Release smoke `20260727162340-f07d3caa` passes 384/384 with exact
  business-state equality.
- Clean Readiness `20260727162422-0de18a69` passes 284/284 applicable plus
  100 N/A with exact state equality.
- Owner preview, consent, live advisory output and Full Data Verification
  acceptance pass on 2026-07-27.
- Schema remains v37.

# v52.1.0 - Optional AI Provider Foundation

Date: 2026-07-27

- Release smoke profile `20260727153808-6a1788dc` passes 383/383.
- Seedless Clean profile `20260727153913-5a0120d2` passes 283/283 applicable
  plus 100 canonical-data N/A and a controlled restart.
- Both resolve the deterministic fake provider, report `network used: no` and
  retain inaccessible owner credentials.
- Smoke retains exact logical and business-state equality and creates no AI
  session or collection residue.
- Debug/Release, Help/release audits and NuGet vulnerability scan pass.
- An owner-reported Settings Grid overlap was corrected before acceptance.
- Post-correction Release smoke `20260727154904-5ef08d0a` passes 383/383 with
  exact business-state equality.
- Owner layout, provider persistence, diagnostics and Full Data Verification
  acceptance pass on 2026-07-27.
- Schema remains v37.

# v50.3.0 - Safety and Recovery Help

Date: 2026-07-27

- Eighteen safety/recovery/troubleshooting Help destinations are present.
- Recovery, Diagnostics and Verification expose stable read-only lookup IDs.
- Smoke invokes no restore, recalculation, update, Production or FTPS action.
- Initial smoke found Diagnostics ID on the Verification window; corrected.
- Profile `20260727120029-14c0556f` passes Full Data Verification 374/374 and
  exact logical/business-state equality.
- Owner runtime/safety acceptance passes on 2026-07-27.
- Schema remains v37; parent v50 continues at mandatory v50.4.

# v50.2.4 - Contextual Help Coverage

Date: 2026-07-27

- All 22 top-level and 16 nested tabs expose unique stable AutomationIds.
- F1 and Help for Current View resolve through one central nested-aware Help
  path; Documentation remains the whole-system overview.
- Four planned Experimental nested destinations plus Tools validation and
  Help-menu references complete the v50.2.4 catalog scope.
- Disabled Website Export Preview is retired in favor of supported tab-only
  navigation.
- Smoke profile `20260727113500-3bc427ca` visits 22/22 and 16/16 surfaces,
  passes representative contextual links and Full Data Verification 373/373.
- Logical and business-state hashes match exactly.
- Owner runtime/UI acceptance passes on 2026-07-27; parent v50.2 is complete.

# v50.2.3 - Output and Tool Reference

Date: 2026-07-27

- Thirty-four stable destinations cover the recorded v50.2.3 matrix scope.
- Owner review rejected the initial short leaf texts as placeholder-like.
- All destinations now document controls, scope, writes, failure behavior,
  evidence and handoff boundaries in substantive multi-paragraph references.
- Verification now owns IDs/mappings, minimum content depth and representative
  control, persistence and output safety markers.
- Smoke owns local Production/FTPS and creator-calendar boundary searches.
- Initial smoke exposed an outdated overview-specific wrapping assertion after
  title-first search selected the new Report scope leaf; the probe was moved.
- Profile `20260727110155-90afdea3` passes Full Data Verification 372/372 and
  exact logical/business-state equality.
- That profile predates the expanded content-depth contract and is superseded
  pending a new disposable run.
- The first expanded-content smoke failed the calendar Help search after an
  accidental `do not` to `does not` wording change; the stable phrase is kept.
- The next run exposed CRLF-only paragraph detection against LF source. The
  content-depth gate now normalizes line endings before checking paragraphs.
- Profile `20260727111924-0056eda1` passes the detailed contract, Full Data
  Verification 372/372 and exact logical/business-state equality.
- Debug/Release, documentation, roadmap and NuGet vulnerability gates pass.
- Owner runtime/content acceptance passes on 2026-07-27.
- Schema remains v37; parent v50 continues at v50.2.4.

# v50.2.2 - Testing and Analysis Reference

Date: 2026-07-27

- Twenty-two stable Help destinations cover the recorded v50.2.2 matrix scope.
- Verification owns exact IDs/mappings and representative save/scope markers.
- Smoke owns Rankings Top 25 and corrected Material Detail Notes searches.
- Debug/Release, documentation, roadmap and vulnerability gates pass.
- Profile `20260727103712-93b32f7b` passes Full Data Verification 371/371 and
  exact logical/business-state equality.
- Owner-search correction makes Experimental Dashboard/Table/Charts directly
  discoverable and adds an `Experimental Table` smoke query.
- Profile `20260727104534-dc519c51` proves title-first search relevance and
  passes Full Data Verification 371/371 with exact state equality.
- Usage Help terminology now says accepted events plus append-only corrections
  instead of immutable events; runtime/data behavior is unchanged.
- Help > Documentation now has a deterministic Start-to-finish overview
  contract, while F1 remains contextual.
- Profile `20260727105308-cc3ce0b1` passes Full Data Verification 371/371 and
  exact logical/business-state equality.
- Owner accepted reference content, terminology, direct Experimental discovery,
  title-first search and Help-menu/F1 entry behavior. v50.2.2 is canonical.
- Schema remains v37; owner runtime/visual acceptance is pending.

# v50.2.1 - Data, Cost and Configuration Reference

Date: 2026-07-27

- Nine stable searchable Help sections cover the v50.2.1 matrix scope.
- Full Data Verification requires representative content and exact tab mappings.
- Smoke adds read-only Materials and Printer/quote reference searches.
- Initial profile `20260727100746-ddc0c805` passed Help but failed 369/370
  because a marker crossed a source-only line break; no data contract failed.
- Profile `20260727101000-dddd3159` passes Full Data Verification 370/370 and
  exact logical/business-state equality.
- Owner review corrected current Quote Help from `immutable` terminology to
  saved read-only history with supported explicit deletion.
- Correction profile `20260727101828-1f53f4e1` passes the revised Help search,
  Full Data Verification 370/370 and exact logical/business-state equality.
- Highlight profile `20260727102551-3b35ebbc` proves visible category, title,
  summary and body matching, including `lifecycle` in summary, and passes Full
  Data Verification 370/370 with exact state equality.
- Debug/Release builds, documentation audit, roadmap and vulnerability gates
  pass. Owner accepted reference content, corrected terminology and header/body
  highlighting. Schema remains v37; v50.2.1 is canonical.

# v50.1.0 - Start-to-finish Workflow Guide

Date: 2026-07-27

- Three independent read-only code traces mapped purchasing/receiving,
  measurement/analysis and reports/website/publishing handoffs.
- Debug and Release solution builds pass with zero warnings and zero errors.
- Documentation audit and roadmap 136-column gate pass.
- Read-only NuGet scan reports no vulnerable packages.
- Initial disposable Verification failed 369/370 on a non-canonical exact text
  marker; no product/data contract failed and the mismatch was corrected.
- Disposable smoke `20260727092658-c116167b` passes the central Help workflow
  and Full Data Verification 370/370.
- Owner-review correction smoke `20260727094039-c70f0cd7` passes in-body search,
  normalized wrapping and Full Data Verification 370/370.
- Refresh-race correction smoke `20260727094810-fa07643d` passes immediate
  Search highlight, Clear removal, wrapping and Full Data Verification 370/370.
- Baseline/final logical and business-state hashes are exactly equal.
- Production, FTPS, updates and owner database selection remain blocked.
- Schema remains v37.
- Owner accepted the complete workflow, natural wrapping, highlighted
  search/jump and immediate Search/Clear refresh. v50.1.0 is canonical.

# v50.0.0 - Comprehensive User Help

Date: 2026-07-27

- One reusable non-modal Help window owns offline content, search and F1
  contextual navigation across all top-level tab families.
- Debug and Release solution builds pass with zero warnings and zero errors.
- Canonical release-documentation audit and roadmap line-length gate pass.
- Read-only NuGet vulnerability scan reports no vulnerable packages.
- Disposable smoke `20260727090306-8a8e6a21` passes the new `central-help`
  contract and Full Data Verification 370/370.
- Baseline/final logical and business-state hashes are exactly equal.
- Production, FTPS, updates and owner database selection remain blocked.
- Owner accepted workflow, search, contextual navigation and the final
  contents-list wrapping correction.
- Schema remains v37; v50.0.0 is canonical and runtime accepted.

# v49.0.0 - Application Release Packaging Candidate

Date: 2026-07-26

- Shared release compatibility contract supports schema v29 through v37.
- Debug and Release solution builds pass with zero warnings and zero errors.
- Candidate release gates pass documentation, NuGet vulnerability, BOM-less
  feed, exact bytes/SHA-256, ECDSA, inventory and activation-order checks.
- Independent package verification accepts both schema-v29 and schema-v37
  endpoints for the exact signed v49.0.0 ZIP.
- Portable smoke `20260726224510-b4ab7d08` passes Full Data Verification
  369/369 with exact baseline/final business-state equality.
- Updater profile `20260726224940-fcb3321a` commits six exact Candidate files,
  receives v49.0.0/schema-v37 health and proves injected-failure rollback with
  exact governed-file hash restoration.
- Owner workstation and clean-VM installer runtime acceptance pass.
- Production, FTPS, owner application and owner database remained blocked
  throughout automated acceptance.
- Exact Candidate bytes are approved for Production promotion; publish remains
  separately guarded.
- Owner live publish completed installer/portable before update-feed activation.
- Read-only remote verification confirms HTTP 200, Production v49.0.0,
  schema v29-v37 and promotion commit `5073da4`.
- Fresh remote installer, portable and signed-update downloads match all three
  accepted Production SHA-256 values exactly.

# v49.0.0 - Experimental Workflow Integrity candidate

Date: 2026-07-26

- Schema remains v37; canonical owner database is not used by automation.
- Candidate gates include active-only comparison, explicit inactive-history
  review, duplicate lifecycle reset, readiness and atomic graph persistence.
- Debug/Release app and AutomationRunner builds pass with zero warnings/errors.
- Release-documentation audit and roadmap line-length gate pass.
- Read-only NuGet vulnerability scan reports no vulnerable packages.
- Initial smoke `20260726212955-219a159d` exposed missing readiness
  initialization before a Series was selected; initialization was corrected.
- Independent agent review found that the first atomic implementation updated
  unrelated Experimental timestamps. Conditional UPSERT now preserves
  byte-identical `UpdatedAtUtc` values for unchanged graph rows.
- Final disposable smoke `20260726213611-698a3177` passes Full Data Verification
  367/367 and exact logical/business-state preservation.
- Owner Verification
  `3DPIceland_FilamentDB_Verification_20260726_215702.txt` passes 367/367.
- Owner runtime found Series Active-only defaulted off, Results-only history
  scope was unclear and Website readiness appeared after leaving the cell.
- The correction sets a checked transient Active-only startup default, labels
  Results/Dashboard/Charts scope and handles Website readiness on first click.
- Final correction smoke `20260726220205-e1dea7a8` passes 368/368 with exact
  logical/business-state preservation.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260726_220656.txt`
  passes 368/368 and accepts all corrected runtime behavior.
- Retired v48 delete/reinsert persistence methods are removed after acceptance.
- Closure smoke `20260726221001-c7dcae2e` passes 368/368 with exact logical and
  business-state preservation after the removal.
- v49.0.0 is complete, canonical and runtime accepted.
- Production and FTPS remain blocked.

# v48.2.0 - Optional Official Exchange-rate Reference Catalog

Date: 2026-07-26

- ECB SDMX parsing and EUR-to-ISK cross-rate derivation are deterministic.
- Automation is offline and must never call the live ECB endpoint.
- Schema v37 owns Purchase Order rate provenance; schema-v36 recovery remains
  compatible.
- Loaded purchases, Inventory, Materials and quotes are excluded from refresh.
- Debug/Release app and AutomationRunner builds pass with zero warnings/errors.
- Documentation and roadmap-line gates pass; the read-only NuGet vulnerability
  scan reports no vulnerable direct or transitive packages.
- Initial smoke `20260726203553-6fe655e4` found 13 recovery gates still bounded
  to schema v36; the bounded schema-v37 compatibility correction removed them.
- Disposable smoke `20260726203714-dcbdce20` passes Full Data Verification
  364/364 and exact logical/business-state preservation.
- Disposable recovery `20260726203749-a8eac754` passes schema-v36 migration,
  24-table restore, restart and exact final business-state recovery.
- Owner Verification
  `3DPIceland_FilamentDB_Verification_20260726_210009.txt` passes 364/364.
- Owner diagnostics confirm schema v37, live ECB observation `2026-07-24` and
  the new-order-only ownership boundary.
- Canonical schema-v37 seed SHA-256 is
  `43503623D8D19A1F38B1505F456BDAA34E9B33AFF4609AD2F79CC2321B7150AE`.
- Preserved schema-v36 migration fixture SHA-256 is
  `6E276E26FB218BC588CC70DA57E10FE2E812A1CA5420494315BE2B22C205EAB0`.
- Final refreshed-seed smoke `20260726210132-f34523ba` passes 364/364 with
  exact logical/business-state preservation.
- v48.2.0 is complete, canonical and runtime accepted.
- Production, FTPS and owner database remain blocked.

# v48.0.7 - Internal Usage Analytics candidate

Date: 2026-07-26

- Debug/Release app and AutomationRunner builds pass with zero warnings/errors.
- NuGet vulnerability audit reports no vulnerable direct or transitive
  packages.
- Release documentation audit passes; Master Roadmap has zero lines over 136
  characters.
- Disposable CRUD profile `20260726175433-70e8dba1`: PASS 361/361.
- Visible analytics prove 1 effective/1 ledger row, 100 g and 1 hour before
  correction.
- After correction they prove 1 effective/3 ledger rows, 80 g and 55 minutes.
- Final cleanup restores exact baseline business state.
- Production, FTPS, owner database and public publishing remained blocked.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260726_181431.txt`:
  PASS 361/361.
- Owner accepted private analytics and visually confirmed public report/website
  previews contain no Usage fields; v48.0.7 is canonical.

# v48.0.6 - Bounded Usage UI and Automation

Date: 2026-07-26

- Debug/Release app and AutomationRunner builds pass with zero warnings/errors.
- NuGet vulnerability audit reports no vulnerable direct or transitive
  packages.
- Release documentation audit passes; Master Roadmap has zero lines over 136
  characters.
- Disposable CRUD profile `20260726173640-4d663edc`: PASS 360/360.
- Visible Usage workspace proves one original event/900 g and three
  original/reversal/replacement rows/920 g across restart.
- Exact MaterialID selection survives tab refresh; final cleanup restores the
  baseline business state.
- Production, FTPS, owner database and public publishing remained blocked.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260726_174800.txt`:
  PASS 360/360.
- Owner accepted empty-Inventory/no-spool behavior and the linked-spool
  Inventory/correction workflow; v48.0.6 is canonical.

# v48.0.5 - Canonical Usage Persistence and Recovery

Date: 2026-07-26

- Debug/Release app and AutomationRunner builds pass with zero warnings/errors.
- NuGet vulnerability audit reports no vulnerable direct or transitive
  packages.
- Release documentation audit passes; Master Roadmap has zero lines over 136
  characters.
- Disposable CRUD profile `20260726171743-3d1e2a09`: PASS 359/359.
- CRUD evidence: original event/900 g, reversal plus replacement/920 g,
  restart persistence and exact cleanup.
- Disposable recovery profile `20260726172034-4e048744`: PASS 359/359.
- Governed 22-table Excel restore and seven SQLite backup artifacts pass.
- Both profiles preserve baseline/final business-state hash
  `E26A54CFF9E6DE6566D3683EE4EACEAC210B225DD4F20925B39DCACCA01E7B09`.
- Production, FTPS and owner database remained blocked.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260726_172529.txt`:
  PASS 359/359.
- Owner accepted normal runtime behavior; v48.0.5 is canonical.

# v48.0.4 - Disposable Usage Domain Prototype

Date: 2026-07-26

- Adds synthetic pure-domain checks for validation, exact reversal/replacement,
  nullable projections and equal/opposite linked-inventory deltas.
- No SQLite, UI, seed, report, website or owner-data change.
- Debug/Release pass with zero warnings/errors.
- Disposable profile `20260726165857-04d43636`: PASS 358/358.
- Database hash remains
  `F0EDCC3295A114C935668D2B92D7A1AEB1C67C4D1630EFC89F11B7FCDC556E5F`.
- Business-state hash remains
  `4FBCF6A2656678875A6692C0A7AA30CD0CDC3F4AAB83003B3BB2C77081B1C87D`.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260726_170130.txt`:
  PASS 358/358.
- Owner runtime accepted normal startup and all four usage-domain gates.
  v48.0.4 is canonical.

# v48.0.2 - Governed Value Index

Date: 2026-07-26

- Adds deterministic formula, input, scope, disclosure, missing-data and UI
  AutomationId Verification.
- Profile `20260726161514-840e50cc`: PASS 352/352.
- Selection-scope correction profile `20260726162917-7e102563`: PASS 353/353.
- MSRP identity profile `20260726163905-dfb93502`: PASS 354/354.
- Recommendation MSRP identity clarity gate: PASS.
- Exact selected-Material Base Material scope now governs Recommendation,
  alternatives, hidden-gem pricing and value-index calculations.
- Database byte and business-state hashes are exactly equal before and after.
- Schema remains v33; Production, FTPS and owner database remained blocked.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260726_164434.txt`:
  PASS 354/354.
- Owner runtime accepted exact PLA/ASA scope, alternatives and recommendation
  price identity. v48.0.2 is canonical.

# v48.0.1 - Canonical Pricing Provenance

Date: 2026-07-26

- Adds deterministic explicit-rate, missing-rate, unsupported-currency and
  canonical-MSRP precedence verification.
- Profile `20260726150144-83168b2e`: PASS 351/351.
- Database byte and business-state hashes are exactly equal before and after.
- Schema remains v33; Production, FTPS and owner database remained blocked.
- Owner runtime accepted Materials, Advisor and Manufacturer/website pricing
  behavior with Full Data Verification PASS.
- Canonical state: v48.0.1.

# v47.0.3 - Stable Coverage Identity

Date: 2026-07-26

- Adds deterministic stable-first coverage identity, exact legacy-binding and
  unmatched-preservation verification.
- Adds read-only AutomationId inspection of isolated disposable coverage state.
- Profile `20260726141853-e0ce0c53`: PASS 350/350.
- Database byte and business-state hashes are exactly equal before and after.
- Production, FTPS and owner database remained blocked.
- Owner runtime accepted coverage identity visibility, zero legacy entries,
  remaining workflow behavior and Full Data Verification PASS.
- Canonical state: v47.0.3.

# v45.2.0 candidate - Base Materials workspace

Date: 2026-07-26

- Verification adds a dedicated-tab, CRUD-control, 23-column Fast contract and
  independent layout-ownership gate.
- Disposable CRUD now includes Base Material create/edit/duplicate/delete
  persistence and exact business-state cleanup.
- Profile `20260726094502-312196e2`: PASS 346/346 with Base Materials tab
  navigation and equal baseline/final business-state hash.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260726_095539.txt` passes
  346/346; immediate-restart visual review found a blank-host activation race
  that is corrected pending owner retest.
- Startup-fix profile `20260726095812-c88314b0` passes immediate
  BaseMaterialsTab activation, CRUD, 346/346 and exact business-state cleanup.
- Final owner evidence `3DPIceland_FilamentDB_Verification_20260726_100133.txt`
  passes 346/346 and accepts immediate first-open rendering. v45.2.0 is canonical.

# v45.1 candidate - Owner relationship acceptance

Date: 2026-07-25

- Exact-name confirmation linked 182 Materials and preserved 20 unmatched legacy values.
- `3DPIceland_FilamentDB_Verification_20260725_224919.txt`: PASS 345/345.
- The v45.1 contract now requires the counted unlinked filter; final UI acceptance remains pending.
- Disposable CRUD profile `20260725225702-22872c00` passes 345/345 with equal
  baseline/final business-state hash
  `9BA19BE80FF49D7BBA1EDEF27B115177D3315AE8422FA0CFBFF17FE826BFD516`.
- Same-name identity correction profile `20260725230522-1ea4b751` also passes
  345/345 and returns the same exact baseline business-state hash.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260725_231008.txt` passes
  345/345 with zero unlinked Materials after staged exact-name binding.
- Disposable profile `20260725231345-2dfcad7a` passes 345/345 and verifies that
  the recovery controls remain available when unlinked legacy rows exist.
- Final owner evidence `3DPIceland_FilamentDB_Verification_20260725_231731.txt`
  passes 345/345 with zero unlinked Materials and accepts the conditional
  recovery UI. v45.1 is canonical.
- Schema-v32 seed acceptance profile `20260725233352-929a4f1a` passes CRUD and
  Full Data Verification 345/345 with baseline/final business-state hash
  `5922D4E7B8AF2FB045C07F1F5F813E1C050C5F17B562003504FCBC15EDBDBDF0`.

# v44.7.11 - Settings Manager Command Clarity

Date: 2026-07-25

- PASS: Full Data Verification reported 336/336 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.7.11, assembly 44.7.11.0 and
  `SETTINGS-MANAGER-COMMAND-CLARITY`.
- PASS: Reload Saved Settings used default-No and restored saved
  General/Deployment SQLite values while preserving Base Material Catalog.
- PASS: built-in restore replaced only General Settings and preserved
  Deployment Settings plus canonical Base Materials through restart.
- PASS: Reset Columns retained its two-layout machine-local scope.
- PASS: misleading duplicated in-memory Settings reload footers were absent.
- ACCEPTED: v44.7.11 is runtime accepted and canonical.

# v44.7.10 - Canonical MaterialID Default Row Order

Date: 2026-07-25

- PASS: Full Data Verification reported 335/335 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.7.10, assembly 44.7.10.0 and
  `CANONICAL-MATERIALID-DEFAULT-ROW-ORDER`.
- PASS: natural numeric default MaterialID order covered Fast Materials,
  Tensile, Impact and Stiffness without changing SQLite order.
- PASS: explicit sorting, Add, Duplicate, retained selection and top-left
  startup viewport passed owner runtime review.
- PASS: close/restart persisted parent Materials before FK-child measurements.
- PASS: Add/Duplicate unique editable defaults remained save-safe.
- ACCEPTED: v44.7.10 is runtime accepted and canonical.

# v44.7.9 - Public Measurement Date Provenance

Date: 2026-07-25

- PASS: Full Data Verification reported 334/334 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.7.9, assembly 44.7.9.0 and
  `PUBLIC-MEASUREMENT-DATE-PROVENANCE`.
- PASS: owner HTML/PDF review confirmed canonical ISO Tensile, Impact and
  Stiffness dates plus exact `Not recorded` fallback.
- PASS: internal edit timestamps stayed excluded and raw inputs/notes retained
  the separate public-detail approval boundary.
- PASS: Reports build-action naming, wrapped tooltips and narrow-window button
  wrapping passed owner runtime review.
- ACCEPTED: v44.7.9 is runtime accepted and canonical.

# v44.7.8 - Backup Filename Compatibility

Date: 2026-07-25

- PASS: Full Data Verification reported 333/333 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.7.8, assembly 44.7.8.0 and
  `BACKUP-FILENAME-COMPATIBILITY`.
- PASS: new automatic, manual, SQLite pre/post-restore and Excel pre-restore
  backups used readable purpose-specific `.bak` names.
- PASS: legacy `.sqlite` discovery, integrity/schema inspection and explicit
  restore remained available.
- PASS: diagnostics separated the new 20-file rotating automatic set from
  retained legacy automatic evidence.
- PASS: updater evidence and the prohibition on automatic SQLite restore
  remained intact.
- ACCEPTED: v44.7.8 is runtime accepted and canonical.

# v44.7.7 - Legacy Grid Retirement - Stage 1

Date: 2026-07-24

- PASS: Full Data Verification reported 319/319 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.7.7, assembly 44.7.7.0 and
  `LEGACY-GRID-RETIREMENT`.
- PASS: Materials preview and all measurement/Settings legacy switches were
  absent from the owner-visible UI.
- PASS: Fast editing, filters, Settings behavior and Reset Columns remained
  operational with temporary collapsed adapters.
- ACCEPTED: v44.7.7 Stage 1 is runtime accepted.

# v44.7.7 - Legacy Grid Retirement - Stage 2

Date: 2026-07-24

- PASS: Full Data Verification reported 320/320 applicable checks, zero FAIL
  and zero N/A.
- PASS: Tensile, Impact and Stiffness used explicit Fast column contracts and
  canonical filtered measurement collections.
- PASS: editing, navigation, calculations, reset, layout persistence and
  Materials-filter propagation passed owner runtime testing.
- ACCEPTED: v44.7.7 Stage 2 is runtime accepted.

# v44.7.7 - Legacy Grid Retirement - Stage 3

Date: 2026-07-24

- PASS: Full Data Verification reported 321/321 applicable checks, zero FAIL
  and zero N/A.
- PASS: Materials used an explicit 52-column Fast contract and canonical
  filtered row source.
- PASS: editing, selection, filters, layouts, CRUD and tab redraw passed owner
  runtime testing.
- PASS: child-first Delete persistence restored UI/SQLite parity at 201
  canonical Materials.
- ACCEPTED: v44.7.7 Stage 3 is runtime accepted.

# v44.7.7 - Legacy Grid Retirement - Stage 4

Date: 2026-07-24

- PASS: Full Data Verification reported 322/322 applicable checks, zero FAIL
  and zero N/A.
- PASS: General Settings and Base Materials used explicit Fast contracts and
  canonical row collections.
- PASS: validation/save/rollback, ComboBoxes, CRUD, layouts and tab redraw
  passed owner runtime testing.
- ACCEPTED: v44.7.7 Stage 4 is runtime accepted.

# v44.7.7 - Legacy Grid Retirement - Stage 5A

Date: 2026-07-24

- PASS: Full Data Verification reported 323/323 applicable checks, zero FAIL
  and zero N/A.
- PASS: measurement legacy toggle controls, handlers, fallback state and reset
  branches were absent.
- PASS: editing, navigation, reset, filters, layouts and close-time save passed
  owner runtime testing.
- ACCEPTED: v44.7.7 Stage 5A is runtime accepted.

# v44.7.7 - Legacy Grid Retirement - Stage 5B Tensile

Date: 2026-07-24

- PASS: Full Data Verification reported 324/324 applicable checks, zero FAIL
  and zero N/A.
- PASS: legacy Tensile DataGrid XAML and named grid lifecycle were absent.
- PASS: Fast Tensile editing, navigation, reset, filters, layouts and restart
  persistence passed owner runtime testing.
- ACCEPTED: v44.7.7 Stage 5B Tensile is runtime accepted.

# v44.7.6 - Fast Workflow Grid - Settings

Date: 2026-07-24

- PASS: Full Data Verification completed with all applicable checks passing.
- PASS: release identity aligned at v44.7.6, assembly 44.7.6.0 and
  `FAST-WORKFLOW-SETTINGS`.
- PASS: Fast general Settings and Base Material Catalog retained their
  canonical validation, save and CRUD contracts with visible legacy fallback.
- PASS: Impact/Stiffness separator persistence and Materials-filter
  propagation to Fast Tensile, Impact and Stiffness passed owner runtime tests.
- PASS: Debug/Release, static checks and NuGet vulnerability scan passed.
- ACCEPTED: v44.7.6 is runtime accepted.

# v44.7.5 - Fast Workflow Grid - Stiffness

Date: 2026-07-24

- PASS: Full Data Verification reported 317/317 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.7.5, assembly 44.7.5.0 and
  `FAST-WORKFLOW-STIFFNESS`.
- PASS: 201 canonical Stiffness rows remained aligned with 201 Materials.
- PASS: Fast Stiffness retained bounded non-negative input, canonical
  calculations, dates, filters, SQLite auto-save and legacy fallback.
- PASS: narrow-grid alignment and Revolutions/Degrees editor placement passed
  owner runtime retest.
- ACCEPTED: v44.7.5 is runtime accepted.

# v44.7.4 - Fast Workflow Grid - Impact

Date: 2026-07-24

- PASS: Full Data Verification reported 316/316 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.7.4, assembly 44.7.4.0 and
  `FAST-WORKFLOW-IMPACT`.
- PASS: 201 canonical Impact rows remained aligned with 201 Materials.
- PASS: Fast Impact retained bounded non-negative input, canonical
  calculations, colors, dates, filters, SQLite auto-save and legacy fallback.
- PASS: rejected values restored once and reset preserved sort, selection and
  scroll during owner runtime testing.
- ACCEPTED: v44.7.4 is runtime accepted.

# v44.7.3 - Fast Workflow Grid - Tensile

Date: 2026-07-24

- PASS: Full Data Verification reported 315/315 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.7.3, assembly 44.7.3.0 and
  `FAST-WORKFLOW-TENSILE`.
- PASS: all 201 canonical Materials retained aligned Tensile, Impact and
  Stiffness rows; schema remained v31.
- PASS: Fast Tensile retained canonical calculations, validation, filters,
  measurement dates, SQLite auto-save and visible legacy fallback.
- PASS: owner runtime testing accepted editing, navigation, colors,
  calculations, copy/paste, reset, restart persistence and fallback.
- ACCEPTED: the owner reported Fast Tensile noticeably snappier than the
  legacy DataGrid; v44.7.3 is runtime accepted.

# v44.6.1 - Canonical Release Documentation Audit

Date: 2026-07-24

- PASS: Full Data Verification reported 311/311 applicable checks, zero FAIL
  and zero N/A.
- PASS: release identity aligned at v44.6.1, assembly 44.6.1.0 and
  `CANONICAL-RELEASE-DOCUMENTATION-AUDIT`.
- PASS: all 201 canonical Materials retained aligned Tensile, Impact and
  Stiffness rows; schema remained v30.
- PASS: standalone release-documentation audit found no new duplicate
  identifiers, recent canonical gaps or unapproved title conflicts.
- PASS: Recovery Center retained its concise summary, exact selected-backup
  Ready detail and guarded restore behavior.
- PASS: System Diagnostics reported six Ready backups, snapshot-folder
  governance PASS and zero incomplete updater transactions.
- ACCEPTED: v44.6.1 is runtime accepted.

# Verification History

## v44.3.1 Recovery and Update Evidence

Status: PASS; RUNTIME ACCEPTED

- Backup and Recovery Center presents transaction state, health
  acknowledgement, application rollback snapshot and SQLite backup evidence as
  four separate read-only boundaries.
- Missing, present, invalid/unreadable and not-recorded evidence states remain
  explicit. A clean profile still shows every boundary.
- No update/recovery engine, durable schema, SQLite restore, evidence retention,
  website/report or FTPS behavior changed.
- Debug and Release builds: 0 warnings, 0 errors.
- NuGet vulnerability audit: PASS.
- Clean-VM Recovery Center visually confirmed all four boundaries as
  `None recorded`, retained two healthy empty-profile backups and explained
  that application rollback never restores SQLite automatically.
- PASS: Application Readiness and Overall Verification 209/209 applicable
  checks; 90 data-dependent checks explicitly not applicable. Build identity:
  `v44.3.1 RECOVERY-UPDATE-EVIDENCE`.

## v44.3.0 Backup, Recovery and Update Evidence Clarity Candidate

Status: PASS; FIRST INCREMENT RUNTIME ACCEPTED; v44.3 REMAINS IN PROGRESS

- Schema-v29 integrity-valid backups with zero Materials are classified
  `Ready — empty profile`, explicitly restorable and distinct from corruption.
- Recovery Center visibly explains every compatibility state and why an empty
  profile is not full-data release evidence.
- Empty-profile restore remains explicit/default-No and warns that current
  SQLite data will be replaced with a zero-data profile.
- No automatic SQLite restore/deletion, schema, transaction, update, website,
  report or FTPS behavior changed.
- Clean-VM Recovery Center displayed two integrity-valid schema-v29 zero-data
  automatic backups as `Ready — empty profile`.
- The initial Candidate correctly exposed a Verification-only dependency error:
  v44.3 reused the v43.1 full-data-backup predicate and failed 208/209. The
  bounded correction tests the guarded restore API/UI contract separately while
  retaining v43.1 as not applicable without canonical Materials.
- PASS: rebuilt clean-VM Candidate, Application Readiness and Overall
  Verification 209/209 applicable checks passed; 90 data-dependent checks were
  explicitly not applicable. Build identity:
  `v44.3.0 RECOVERY-EVIDENCE-CLARITY`.

## v44.2.0 Daily-use UI State and MaterialID Clarity

Status: PASS; RUNTIME ACCEPTED

- PASS: existing machine-local window geometry and keyed column-width storage
  was preserved; captured column display order is now restored with validation
  and invalid-state fallback.
- PASS: selected canonical MaterialID is stored only in machine-local workflow
  preferences and is restored only when present in the current visible dataset.
- PASS: Materials, Material Detail and Reports expose the same explicit
  MaterialID. Exactly one presentation-only row flag keeps the selected row
  light blue without changing the accepted cell-selection/editing model.
- PASS: checkbox mutation occurs only inside the rendered checkbox bounds;
  blank checkbox-cell space selects the material without changing its value.
- RUNTIME REJECTED / REMOVED: a persistent full-row highlight conflicted with
  the existing one-click editor, produced multiple apparent selections and
  blocked text/checkbox edits. The `FullRow` selection-mode change was removed.
- RUNTIME FINDING: a first native horizontal page jump into unrealized Materials
  columns measured about 15 seconds. Disabling column virtualization fixed
  scrolling but caused unacceptable startup delay; a custom bounded timer later
  produced an unresponsive halfway state. Both experiments were fully removed.
  A/B testing ruled out saved `DisplayIndex`; the older performance issue is
  explicitly scheduled for measured v44.4 investigation.
- PASS: no SQLite schema/backup, calculation, report/website publishing,
  installer, updater or recovery behavior changed.
- PASS: Debug and Release builds completed with zero warnings and zero errors.
- PASS: desktop runtime accepted row-selection clarity, checkbox hit bounds,
  text editing, restart selection and keyed column-width persistence.
- PASS: Full Data Verification reported 298/298 applicable PASS, zero FAIL and
  zero N/A with aligned v44.2.0 `DAILY-UI-STATE` identity.

## v43.8.0 Remote Signed Update Delivery

Status: PASS; RUNTIME ACCEPTED

- Debug and Release builds pass with zero warnings and zero errors.
- Production-signed runtime candidates passed the verifier with six governed files and schema v29.
- Manual/one-minute discovery, bounded HTTPS download, full re-verification, Default-No Apply and isolated update publishing passed clean-VM runtime testing.
- Interrupted snapshot testing exposed and fixed transient Windows file-lock handling, incomplete-snapshot phase classification, idempotent rollback and staged-helper bootstrapping; transaction evidence and SQLite backups remained preserved.
- VM v43.8.5 -> v43.8.6 and v43.8.6 -> v43.8.7 committed with zero incomplete transactions.
- SQLite-native restore recovered 200 Materials and canonical measurement data, created a verified recovery backup and restarted successfully; SQLite was never automatically restored by update/recovery.
- Final restored-data Verification Center result: PASS 296/296 on v43.8.7. Canonical repository release: v43.8.0.

---

## v43.7.0 Installer and Portable Deployment

Status: PASS; RUNTIME ACCEPTED

- Corrected production-signed v43.7.0 source package passed the real verifier with six governed files and SQLite schema v29.
- Inno Setup produced a 68,032,212-byte per-user installer; the 95,403,440-byte portable ZIP contains exactly the same six governed runtime files.
- Deployment plan binds installer and portable artifacts to exact bytes/SHA-256 and `/downloads` stable/versioned routes.
- The original VM candidate was invalidated after clean startup exposed 176 compiled historical material rows. Audit also found and removed the bundled website data snapshot from governed output.
- A subsequent clean VM probe found owner-specific FTPS host/user defaults; these and the legacy username-specific credential fallback were removed. Existing owner SQLite settings remain unchanged.
- A subsequent VM probe found the splash/header PNG missing under installed single-file startup. The PNG is now an embedded WPF pack resource and no longer a loose governed file; ICO and report JPEG remain external.
- Three SVG development diagrams were confirmed unreferenced by runtime/report/documentation code and removed from publish/governed inventory.
- Corrected signed package, installer and portable ZIP contain no `[private-material-id-removed]`, known private seed marker, private FTPS username, SQLite/spreadsheet/legacy JSON or website snapshot payload.
- Website routes are rejected; application-release backup/staging is isolated. Live FTPS publish was not run.
- Authenticode is intentionally deferred for private VM testing.
- Clean VM install and restart passed with zero materials, empty FTPS host/user defaults, rendered splash/header branding and only ICO/JPEG assets installed.
- Explicit SQLite transfer restored owner data while the Credential Manager password remained machine-local and prompted on first FTPS use.
- Verification Center passed on the owner workstation. Default-No live application-release publish and both stable browser downloads passed.

---

## v43.6.0 Update and Deployment Diagnostics

Status: PASS

- Debug/Release application and updater builds completed with 0 warnings and 0 errors.
- Updater self-tests safely restarted Prepared and restored last-known-good governed files from SnapshotReady, Installed, RollingBack and RollbackFailed.
- Read-only history classification passed and prior commit, partial-install rollback, failed-health rollback and traversal protections remain intact.
- Visual Studio Debug runtime confirmed v43.6.0 identity, read-only history with one prior Committed transaction, zero incomplete transactions and Verification PASS 294/294.
- SQLite was not restored; transaction evidence/backups remain retained; website/report/FTPS engines were not changed.

---

## v43.5.1 Guarded Application Update

Status: PASS

- Signed v43.5.2 readiness passed manifest, exact 11-file inventory, byte length, SHA-256, trusted signature, version policy and SQLite schema v29.
- Guarded Apply created verified backup `filamentdb_manual_20260722_153448_945.sqlite` before application-file mutation.
- External helper committed transaction `20260722153447164-66831a0866194e268f3a8260570bdc81` from v43.5.1 to v43.5.2.
- First usable startup wrote matching transaction identity, release v43.5.2 and database schema v29 health acknowledgement.
- Verification Center passed 293/293 after restart; SQLite was never silently restored.

---

## v43.5.0 Transactional Updater Engine

Status: PASS

- Isolated complete transaction committed the full staged governed-file set.
- Injected partial installation and failed health acknowledgement both restored the complete last-known-good set.
- Traversal and directory escape were blocked before application-file mutation.
- Packaged external updater self-test passed; signed package contained 10 governed files.
- Runtime Update Readiness passed package, manifest, inventory, SHA-256, trusted signature and SQLite schema v29; same-version v43.5.0 was correctly blocked.
- Verification Center was Overall PASS on 2026-07-22; live Apply and SQLite mutation remain disabled.

---

## v43.4.1 Governed Signed Release Packaging

Status: PASS

- Production CNG release key sign/verify passed and private-key export was blocked.
- Canonical single-file publish and exact nine-file signed package creation completed with 0 warnings and 0 errors.
- The real application verifier accepted the production-signed package and blocked a modified LICENSE payload at the length/hash boundary.
- Runtime Update Readiness reported PASS for package, manifest, inventory, SHA-256, trusted signature and SQLite schema v29.
- Same-version v43.4.1 was correctly blocked by version policy; inspection changed no application or SQLite files.
- Verification Center was Overall PASS on 2026-07-22.

---

## v43.4.0 Signed Update Readiness Foundation

Status: PASS

- Versioned update manifest, safe relative paths, exact inventory, sizes, SHA-256 and ECDSA P-256/SHA-256 policy are enforced.
- Isolated signed fixture passes; tampered content, downgrade, path traversal and missing production trust are blocked.
- Inspection is read-only and does not extract, stage, replace or launch application files or alter SQLite.
- Existing SQLite backup/recovery and website FTPS publishing/rollback boundaries remain unchanged.
- Release and Debug builds complete with 0 warnings and 0 errors.
- Runtime acceptance on 2026-07-22 confirmed startup, Update Readiness menu visibility and Overall Verification PASS.

---

## v42.13.0 Material Printing Settings Foundation

Status: PASS

- SQLite schema v23 adds optional printing settings without replacing canonical MaterialID ownership.
- Nozzle temperature, bed temperature and print speed preserve min/recommended/max values with explicit °C and mm/s units.
- Drying time uses hours; blank values remain unknown rather than zero.
- Cooling and enclosure choices remain vendor-neutral; printer/slicer profiles remain governed free-form references.
- Native record mapping, Material Detail projection and Excel import/export cover every new field.
- Public report models and allowlists exclude the new settings pending a later explicit publication decision.
- The aggregate v42.13 gate requires the printing-settings contract, internal-only boundary, v42.12 publishing gate and aligned release identity.
- Debug and Release builds complete with 0 warnings and 0 errors and produce file version 42.13.0.0.
- Runtime acceptance on 2026-07-22 confirmed startup, Overall PASS, persistence across restarts and responsive arrow-key navigation after Materials edit debouncing.

---

## v41.4.0 Manufacturer & Category Positioning

Status: PASS

- Debug and Release builds complete with 0 warnings and 0 errors and produce file version 41.4.0.0.
- Manufacturer positioning must use unique MaterialIDs and existing overall engineering scores.
- Category positioning must expose deterministic rank, peer count and group-average context.
- Missing manufacturer, category or score evidence must remain unavailable rather than inferred.
- Recommendation Detail and the reusable prompt must expose the governed positioning context.
- The aggregate v41.4 gate requires all prior v41 engineering-intelligence gates and aligned release identity.
- User acceptance confirmed correct positioning for both global recommendations and the active MaterialID in Selected Material Intelligence, followed by a clean Verification Center run on 2026-07-21.

---

## v41.3.0 Price, Inventory & Manufacturer Context

Status: PASS

- Debug and Release builds complete with 0 warnings and 0 errors and produce file version 41.3.0.0.
- Canonical pricing boundary must preserve the Materials MSRP USD/kg value.
- Empty canonical MSRP must remain unavailable even when landed cost or a stale legacy projection contains a value.
- Inventory context must consume `InventoryEngineService` output without recalculating spool state or remaining weight.
- Manufacturer context must consume active SQLite manufacturer records.
- Recommendation Detail must expose price, inventory status/detail and manufacturer context.
- The aggregate v41.3 gate requires all prior v41 engineering-intelligence gates and aligned release identity.
- User acceptance confirmed the clearer Public MSRP wording, the missing-MSRP behavior and a clean in-app Verification Center run on 2026-07-21.

---

## v41.2.0 Consistency & Outlier Intelligence

Status: PASS

- Debug and Release builds complete with 0 warnings and 0 errors and produce file version 41.2.0.0.
- Verified Material Summary probe covers four tensile/impact orientation sets with deterministic CV and sample-count inputs.
- Strong-repeatability probe validates average CV, measurement-set coverage and adequate specimen coverage.
- High-variation probe validates the highest-CV orientation and summary-level review flag without enabling specimen-level outlier claims.
- Limited-evidence probe preserves single-specimen data as insufficient repeatability evidence.
- Recommendation Detail must expose status, repeatability summary and outlier-review guidance.
- Selected Material Intelligence must follow the active MaterialID independently of the global Top 3 ranking lists, with the displayed Material Detail taking precedence over stale selections in another grid.
- The aggregate v41.2 gate requires consistency, alternatives, advisor and release identity contracts.
- User acceptance confirmed a clean in-app Verification Center run on 2026-07-21.

---

## v41.1.0 Comparable Alternatives & Hidden Gems

Status: PASS

- Closest-alternative probe excludes the selected material and candidates from another recommendation context.
- Value hidden-gem probe validates a 40% canonical MSRP USD/kg reduction while preserving near-peer recommendation performance.
- Specialist probe validates a meaningful stiffness gain plus an explicit trade-off.
- Recommendation Detail must expose the alternatives table and status surface.
- Startup pricing hydration must complete after canonical native materials load, without relying on a filter interaction.
- The aggregate v41.1 gate requires alternatives, explainable advisor and release identity contracts.
- User confirmed MSRP is hydrated on initial Recommendation Engine display and the complete Verification Center run finishes normally without filter interaction.

---

## v41.0.1 Advisor Locale Verification Fix

Status: PASS

- The v41.0.0 user run reported 169 / 171 PASS.
- The direct advisor comparison failure was caused by culture-formatted decimal commas in correct Icelandic output; the aggregate v41 gate was the only dependent failure.
- v41.0.1 validates typed score delta, lead-axis delta and trade-off-axis delta instead of matching localized display text.
- User confirmed the corrected Verification Center run reports 171 / 171 PASS.

---

## v41.0.0 Explainable Engineering Advisor

Status: PENDING IN-APP CONFIRMATION

- Debug and Release builds complete with 0 warnings and 0 errors.
- Deterministic probes cover complete and partial five-axis EngineeringScoreProfile inputs.
- Advisor checks validate strongest evidence, lowest-axis trade-off, missing-data disclosure and evidence-coverage wording.
- Alternative comparison checks validate recommendation-score delta plus clearest axis lead/trade-off.
- Recommendation Detail checks require the evidence, coverage and comparison UI surfaces.
- The aggregate v41 advisor gate also requires release identity alignment.

---

## v40.20.1 Pricing Filter Synchronization Fix

Status: PASS

- Root cause: Pricing mirrors emitted `change`, but the canonical website filter engine listens for `input`.
- Root cause: Pricing multi-selects did not inherit the Database `mousedown` toggle behavior and therefore used the browser's default single-selection click behavior.
- Pricing now transfers selection into the canonical controls and dispatches the canonical `input` event.
- Pricing multi-select options now toggle independently without requiring Ctrl/Cmd.
- Verification Center includes an explicit interaction-parity contract in addition to the existing element and visual-parity checks.
- User confirmed website filtering works correctly in both directions between Filament Database and Pricing & Value.
- Initial v40.20.1 run passed 164 / 166 checks: Pricing interaction and visual parity both passed; the only direct failure was the identity predicate still requiring the exact v40.20.0 label, with the aggregate gate failing as its dependent result.
- Release identity now validates BuildInfo against assembly and informational metadata instead of hard-coding one patch version.
- User confirmed the final Verification Center run reports 166 / 166 PASS.

---

## v40.20.0 Platform Integration & Release Readiness

Status: PASS

- Initial user run: 162 / 165 PASS. The website rendered correctly; failures were limited to Pricing visual parity, Preview/Production renderer parity and the aggregate gate that depends on renderer parity.
- Corrected the Pricing predicate to match the combined `search-reset compact-filter-row` class emitted by the renderer.
- Corrected renderer parity normalization to remove the exact generated mode header after governed prefix markers; it no longer assumes that header is the first HTML line.
- Parity failures now report the first differing character and both canonical output lengths instead of returning an empty detail message.
- Resolved all 12 unique nullable-analysis sites (previously reported twice by the WPF temporary and final project passes).
- Debug and Release builds completed with 0 warnings and 0 errors.
- User confirmed the corrected v40.20 Verification Center run reports all checks PASS.
- Preview and Production renderer parity is now verified after removing only the explicit output-mode header.
- Release contract covers build identity, all five website portal routes and mode-aware manufacturer redirects.
- Export package contract covers main HTML, redirect companion and methodology whitepaper in both manifests.
- Aggregate local v40 gate requires Engineering, Experimental, Website, Reporting, workspace order and release identity to pass together.
- Obsolete v34.3.0 and unconditional documentation/workflow PASS statements were removed.
- Live FTPS is explicitly deferred until the external passive port range is available.

---

## v40.19.1 Pricing & Value Portal Tab

Status: RELEASE BUILD AND USER VERIFICATION PENDING

- Dedicated `#pricing` route and portal page added.
- Deterministic verification confirms all three pricing sections move out of Database and appear exactly in Pricing & Value.
- Filter synchronization coverage includes category, base material, variant, reinforcement, colour, manufacturer, product line, MSRP range, pricing availability and search.
- Visual-parity coverage confirms the Pricing tab reuses the canonical Database filter card, hints and search/reset layout.
- Filter guidance coverage confirms verbose independent-option descriptions are replaced by one concise multi-select instruction.
- Multi-select sizing coverage confirms the selection surfaces use the expanded 198-pixel height.
- Compact-row coverage confirms Reset sits above Search and Sort occupies the space between Search and MSRP.
- Workspace tab-order coverage confirms Material Detail follows Materials and the four secondary workflow tabs follow Website Export.
- Shared-grid coverage confirms both tabs show Chart mode through Product line on top, Sort/Search at lower left and MSRP range with Pricing availability bottom-aligned at lower right.
- Portal verification confirms the legacy Database methodology summary is absent while the dedicated Methodology page remains present.
- Embedded portal JavaScript passed an independent syntax check with the bundled Node.js runtime.
- Existing pricing calculation functions and original interactive element IDs remain unchanged.

---

## v40.19.0 Experimental Website Analytics

Status: PASS

- Experimental website payload and renderer moved behind `ExperimentalWebsiteService`.
- Release build completed with 0 errors and produced file version 40.19.0.0; 24 pre-existing nullable-analysis warnings remain.
- Embedded chart JavaScript passed an independent syntax check with the bundled Node.js runtime.
- Verification Center now checks live published identities, baseline/value safety, ranking parity, chart coverage and the browser calculation boundary.
- A deterministic two-run contract probe covers serialization, five chart types and baseline-normalized indexes even when no real series is selected for publication.
- User confirmed the in-app Verification Center reports all checks PASS.
- User confirmed Website Preview renders the Experimental Lab correctly and that responsive resizing works as intended.

---

## v40.18.1 Explicit FTPS Publishing Fix

Status: BUILD PASS / USER CONNECTION VERIFICATION PENDING

- Mandatory explicit TLS on port 21; no plaintext FTP fallback.
- Passive data connections match the confirmed FileZilla profile.
- Server certificate must pass Windows trust validation.
- Password remains isolated in Windows Credential Manager.
- Existing backup, staging, size-validation and rollback workflow is preserved.
- Verification Center checks the approved FTPS deployment contract.
- Dependency audit confirms FluentFTP 54.2.0 with no additional transitive packages and no remaining SSH.NET dependency.
- Release build completed with 0 errors and produced file version 40.18.1.0; 24 existing nullable-analysis warnings remain outside this transport correction.
- Live TLS negotiation, login, passive transfer and remote backup creation remain for the user to test with the password inside the application.

---

## v40.18.0 Secure SFTP Website Publishing

Status: SUPERSEDED BY v40.18.1 BEFORE SUCCESSFUL LIVE CONNECTION

- SSH host-key trust is required and persisted separately from the password.
- Password storage uses Windows Credential Manager and never enters project or database files.
- Production files are required before publishing.
- Existing remote files are copied to a timestamped backup folder.
- All uploads use unique temporary names and remote-size validation before live replacement.
- Replacement failure triggers best-effort rollback from the remote backup.
- Verification Center includes an approved SFTP host/account/path/backup-root contract gate.
- Release build completed with 0 errors and produced file version 40.18.0.0; 24 existing nullable-analysis warnings remain outside this publishing milestone.
- Live host-key approval, login, remote backup creation and publishing remain for the user to test with the SFTP password inside the application.

---

## v40.17.4.4 Website Export Folder Persistence

Status: PASS

- Folder selection writes through the existing workflow-preference service immediately.
- Startup restores only a saved directory that still exists.
- Missing directories preserve the established default-folder fallback.
- Existing preference JSON remains backwards-compatible because the new field has a safe empty default.
- Static checks confirmed startup restore, immediate Choose Folder persistence and clean-shutdown capture use the shared preference service.
- Release build completed with 0 errors and produced file version 40.17.4.4; 24 existing nullable-analysis warnings remain outside this preference change.

---

## v40.17.4.3 Manufacturer Relative Redirect Fix

Status: PASS

- Preview redirect target: `../index-test.html#manufacturers`.
- Production redirect target: `../index.html#manufacturers`.
- Canonical metadata remains `https://iskort.is/3dp/index.html#manufacturers`.
- Both variants are checked by the website export validation gate.
- Static regression checks confirmed the mode-specific targets and found no obsolete parameterless redirect builder call.
- Release build completed with 0 errors and produced file version 40.17.4.3; 24 existing nullable-analysis warnings remain outside this routing fix.

---

## v40.17.4.2 Manufacturer Redirect Export Cleanup

Status: PASS

- Separate manufacturer export controls and command handlers were removed.
- Main Preview/Production export owns the redirect companion file.
- Redirect structure is checked during export validation.
- Production backup behavior covers the existing manufacturers redirect.
- Static regression search found no remaining manufacturer export buttons, event handlers or separate export command.
- Redirect target, meta refresh, JavaScript replacement, canonical link and clickable fallback are emitted by one deterministic builder.
- Release build completed with 0 errors and produced file version 40.17.4.2; 24 existing nullable-analysis warnings remain outside this cleanup scope.

---

Consolidated static-audit and verification record. New release verification is added at the top of this file; do not create per-build report files.

---

## v40.17.4.1 Manufacturer Terminology Verification Fix

Status: PASS

- Diagnostics from 2026-07-20 showed 156/157 checks passing; all v40.17.4 manufacturer submission checks passed.
- Root cause: the terminology gate used a substring search for `manufacturer-cta`, which also matched valid `manufacturer-cta-row` and `manufacturer-cta-primary` classes.
- The production cleanup regex already targeted only the exact legacy class and did not remove the new form.
- The corrected gate checks exact legacy-class absence and current submission CTA presence.
- Regression probe confirmed the exact legacy class is removed while `manufacturer-cta-row` and `manufacturer-cta-primary` remain.
- Release build completed with 0 errors and produced file version 40.17.4.1.

---

## v40.17.4 Manufacturer Material Submission Workflow Verification

Status: PASS

- Structured manufacturer form fields are emitted by the canonical portal renderer.
- Required-field and independent-testing acknowledgement constraints are present.
- Browser-side reference generation uses the `3DPI-YYYYMMDD-XXXXXXXX` format.
- Email subject and body are encoded before the `mailto:` handoff to `iskort@iskort.is`.
- Copy Submission Details uses the same payload with a clipboard fallback.
- No `fetch` call, backend endpoint, web database or direct SQLite write path is present in the submission surface.
- Preview and Production use the same deterministic manufacturer portal renderer.
- Embedded portal JavaScript passed Node.js syntax parsing.
- Release build completed with 0 errors; existing nullable-analysis warnings remain outside this milestone scope.

---

## Repository layout cleanup verification

Status: PASS

- Root `.gitignore` contains the relevant Visual Studio, .NET, test, coverage, publish and packaging exclusions formerly provided by `App/.gitignore`.
- Root `.gitattributes` applies text normalization and binary handling across the full repository.
- Nested `App/.gitignore` and `App/.gitattributes` were removed after consolidation.
- Cleanup targets were resolved and verified inside `C:\3DPIceland-App-Codex\App` before clearing accumulated `.vs`, `bin` and `obj` contents.
- Active IDE indexing recreated a small set of locked/ignored `.vs` and `obj` cache files; generated directories remain reproducible and ignored by Git.
- No tracked source or canonical data was removed.
- Documentation SVG assets were retained for a separate usage decision.

---

## Generated-file and local-data protection verification

Status: PASS

- Root `.gitignore` covers Visual Studio state, `bin`, `obj`, test results, publish output and package artifacts.
- Local SQLite databases and their WAL/SHM companion files are ignored.
- Environment overrides, common secrets files, logs, backups, temporary files and local release archives are ignored.
- Previously tracked `.vs` files are removed from Git tracking without deleting the local directory.
- Representative paths were checked with Git's ignore matcher before commit.
- No application behavior or canonical data path changed.

---

## GPL-3.0-only Repository Licensing Verification

Status: PASS

- Canonical repository-root `LICENSE` content matches the official GNU GPLv3 text downloaded from `gnu.org` (ignoring trailing blank lines only).
- README, .NET project metadata, About dialog, package structure, changelog and build history consistently identify the project as `GPL-3.0-only`.
- The obsolete `App/LICENSE` MIT file was removed, and no active project-level MIT license reference remains.
- `LICENSE` and `THIRD-PARTY-NOTICES.md` are copied into the Release output beside the application binaries.
- Current direct and transitive NuGet dependencies were inventoried as MIT, Apache-2.0 or the permissive Microsoft WebView2 SDK license; their independent licenses are preserved in `THIRD-PARTY-NOTICES.md`.
- Release build completed successfully with 0 errors. Existing nullable-analysis warnings remain outside the licensing change scope.
- No SQLite schema, canonical identifier, measurement, calculation, report, whitepaper or website-export behavior changed.

---

## Source: Reports/v40.17.3_MANUFACTURER_OUTREACH_SUBMISSION_PORTAL_STATIC_AUDIT.md

# v40.17.3 Manufacturer Outreach & Submission Portal – Static Audit

## Result

PASS – Release build completed with 0 errors.

## Verified scope

- Manufacturer outreach hero and independent, data-driven positioning.
- Live manufacturer/material coverage and 150+ Verification Center baseline.
- Four-step material submission workflow.
- `Submit Materials for Testing` email CTA.
- Methodology Portal and Engineering Whitepaper links.
- Participation benefits, platform capability cards and no-paid-placement statement.
- Existing SQLite-backed manufacturer directory and engineering intelligence preserved.
- Dedicated Verification Center gate added for the outreach content.

## Release boundary

No SQLite schema, engineering calculation, ranking, report or whitepaper-generation changes.

## Build

`dotnet build App\FilamentDbApp.sln -c Release --no-restore`

Result: 0 errors. Existing nullable warnings remain outside this milestone's scope.

---

## Source: Reports/v40.17.2_MANUFACTURER_BEST_VALUE_DISPLAY_DETAIL_STATIC_AUDIT.md

# v40.17.2 Static Audit

- Best Value selection still uses Overall Score divided by canonical MSRP USD/kg.
- Display now includes MSRP, Engineering Score and Value Score.
- Generic engineering leader cards remain unchanged.
- No SQLite schema or calculation changes.

---

## Source: Reports/v40.17.1_MANUFACTURER_BEST_VALUE_PRICING_SOURCE_FIX_STATIC_AUDIT.md

# v40.17.2 Static Audit

- Canonical pricing source: SQLite NativeMaterialManagerRows via MaterialID.
- Preferred denominator: MSRP USD/kg.
- Compatibility fallback: landed USD/kg / projected export columns.
- Existing Manufacturer Engineering Intelligence rendering preserved.

---

## Source: Reports/v40.17.0_MANUFACTURER_ENGINEERING_INTELLIGENCE_STATIC_AUDIT.md

# v40.17.0 Static Audit

- Source remains native SQLite material rows.
- Mechanical leaders consume freshly built verified Material Summary values.
- Layer adhesion, overall and value intelligence consume canonical EngineeringScoreProfile results.
- No result values are hard-coded in the website template.
- Existing manufacturer profile editing and website export compatibility are preserved.
- A Verification Center gate checks that the intelligence surface is rendered.

---

## Source: Reports/v40.16.2_MANUFACTURER_SELECTION_ACTION_STATIC_AUDIT.md

# v40.16.2 Static Audit

- Manufacturer action handlers no longer depend only on `DataGrid.SelectedItem`.
- Cell selection resolves through CurrentItem, CurrentCell and SelectedCells fallbacks.
- Archive/Restore view refresh is deferred to avoid edit-transaction conflicts.
- No storage schema or downstream publication changes.

---

## Source: Reports/v40.16.1_MANUFACTURER_WEBSITE_SOURCE_SYNC_STATIC_AUDIT.md

# v40.16.1 Static Audit

- Main website Manufacturers portal receives `BuildNativeMaterialDataRows(includeArchived: false)`.
- Portal manufacturer names are the union of native material manufacturers and active SQLite manufacturer profiles.
- Material totals are calculated from native active rows.
- No SQLite schema or calculation engine changes.

---

## Source: Reports/v40.16.0_NATIVE_MANUFACTURERS_WEBSITE_STATIC_AUDIT.md

# v40.16.0 Static Audit

- Main portal placeholder removed.
- Native manufacturer renderer receives canonical material rows.
- Active SQLite profiles enrich website cards.
- HTML output uses encoded content and external links use noopener/noreferrer.
- Legacy standalone manufacturers export preserved.
- Build identity and governance documents updated.

---

## Source: Reports/v40.15.3_MANUFACTURER_EDIT_TRANSACTION_SAFETY_STATIC_AUDIT.md

# v40.15.3 Static Audit – Manufacturer Edit Transaction Safety

- PASS: Manufacturers grid remains editable.
- PASS: Unsafe CellEditEnding ICollectionView refresh removed.
- PASS: Manufacturer PropertyChanged auto-save remains active.
- PASS: Search and archived filters retain explicit refresh handlers.
- PASS: Add, Duplicate, Archive/Restore and Delete retain controlled refresh calls outside edit transactions.
- PASS: No SQLite schema or website export contract changes.

---

## Source: Reports/v40.15.2_MANUFACTURER_GRID_EDITABILITY_FIX_STATIC_AUDIT.md

# v40.15.2 Static Audit – Manufacturer Grid Editability Fix

- ManufacturersGrid `IsReadOnly=False`: PASS
- Cell-level selection enabled: PASS
- First-click workflow registration: PASS
- Two-way bindings retained: PASS
- SQLite PropertyChanged auto-save retained: PASS
- Website manufacturer payload path unchanged: PASS

---

## Source: Reports/v40.15.1_MANUFACTURER_FILTER_BUILD_FIX_STATIC_AUDIT.md

# v40.15.1 Static Audit – Manufacturer Filter Build Fix

- Confirmed `MainWindow.Manufacturers.cs` no longer declares a method named `ManufacturerFilter`.
- Confirmed the existing XAML element `x:Name="ManufacturerFilter"` remains available to the legacy import-cache filter workflow.
- Confirmed the manufacturer collection view now references `ManufacturerProfileFilter`.
- No database schema or website payload changes were introduced.
- Runtime build and Verification Center execution remain part of local acceptance testing.

---

## Source: Reports/v40.15.0_MANUFACTURER_KNOWLEDGE_PLATFORM_STATIC_AUDIT.md

# v40.15.0 Static Audit – Manufacturer Knowledge Platform

## Result
STATIC IMPLEMENTATION REVIEW: PASS

## Reviewed
- SQLite schema expansion is additive and preserves the existing Manufacturers primary key and Name uniqueness.
- Schema migration uses idempotent `EnsureColumn` operations for existing databases.
- Manufacturer CRUD uses parameterized SQLite commands.
- Delete creates an automatic database backup before the destructive operation.
- WPF manager binds directly to native ManufacturerRecord rows and auto-saves property changes.
- Website payload retains canonical material-derived metrics and adds active SQLite profile fields.
- Existing external manufacturers template contract and preview/production file paths are preserved.
- No raw measurement rows are introduced into website generation.

## Runtime acceptance required
Clean/Rebuild in Visual Studio, exercise manufacturer CRUD, generate manufacturers preview, and run Verification Center.

---

## Source: Reports/v40.14.5_WHITEPAPER_LOGO_RENDERING_FIX_STATIC_AUDIT.md

# v40.14.5 Whitepaper Logo Rendering Fix - Static Audit

## Scope reviewed
- Supplied JPG logo file.
- PDF-specific logo asset replacement.
- Native PDF renderer image contract.
- Output-copy and Report Asset Pipeline configuration.
- Release identity and documentation.

## Static verification results
- PASS: supplied asset is a baseline JFIF JPEG.
- PASS: supplied asset dimensions are 801 x 482 pixels.
- PASS: supplied asset contains three RGB components and no alpha channel.
- PASS: `Assets/3dp-iceland-labs-logo-pdf.jpg` contains the supplied JPEG bytes.
- PASS: DocumentationEngineService and ReportPdfRendererService continue to use the canonical PDF JPG asset.
- PASS: project copies the PDF JPG asset to output.
- PASS: Report Asset Pipeline requires and verifies the PDF JPG asset.
- PASS: project, assembly, file and informational versions are 40.14.5.
- PASS: release identity is WHITEPAPER-LOGO-RENDERING-FIX.
- PASS: no SQLite schema, calculation service, Material Summary, Website payload, report model or whitepaper layout changes.

## Local release verification
Run a clean Visual Studio build, export the whitepaper, inspect the cover and running header, then run Verification Center.

---

## Source: Reports/v40.14.4_ENGINEERING_WHITEPAPER_STATIC_AUDIT.md

# v40.14.4 Engineering Whitepaper Professional Edition - Static Audit

## Scope reviewed
- DocumentationEngineService PDF object generation and page layout.
- New logo asset and output-copy configuration.
- New SVG source figures.
- Project/build identity and release documentation.

## Static verification results
- PASS: project, assembly, file and informational versions are 40.14.4.
- PASS: application release identity is ENGINEERING-WHITEPAPER-PROFESSIONAL-EDITION.
- PASS: dark full-page PDF background commands were removed.
- PASS: content begins below a fixed 64-point running header and pagination uses a reduced content-height threshold.
- PASS: logo JPEG is embedded as a PDF image XObject and copied to application output.
- PASS: cover, contents, engineering matrix, architecture diagram and confidence graph are generated natively.
- PASS: reusable SVG source figures are packaged under Assets/Documentation.
- PASS: no SQLite schema, calculation service, Material Summary, Website payload or Report model changes.

## Local release verification
Run a clean Visual Studio build, export the whitepaper, inspect A4 print preview and run Verification Center.

---

## Source: Reports/v40.14.3_ENGINEERING_WHITEPAPER_STATIC_AUDIT.md

# v40.14.3 Static Audit

- Documentation service contains 16 ordered sections.
- PDF rendering uses automatic line wrapping and continuation pagination.
- Page footer includes methodology version and current/total page number.
- Current tensile, impact and stiffness constants are documented.
- All three procedure video URLs are retained.
- Project, assembly, file and informational versions updated to 40.14.3.
- Build execution was not available in the packaging environment; local Visual Studio rebuild is required.

---

## Source: Reports/v40.14.0_STATIC_VERIFICATION.md

# v40.14.0 Static Verification

- Package structure: PASS
- Version metadata: PASS
- Documentation model/service present: PASS
- Whitepaper PDF renderer present: PASS
- Manual export command wired: PASS
- Main website export writes whitepaper: PASS
- Methodology Portal PDF link: PASS
- Verification Center gates added: PASS
- Source compilation: NOT EXECUTED (runtime does not contain the .NET SDK)

---

## Source: Reports/v40.13.0_METHODOLOGY_PORTAL_VERIFICATION.md

# v40.13.0 Methodology Portal Verification

## Expected gates

- PASS — Methodology portal content
- PASS — Methodology procedure videos
- PASS — Methodology technical constants
- PASS — Methodology whitepaper handoff

## Regression scope

This release changes the public portal documentation surface only. Native material storage, measurement entry, calculations, experimental data, reporting and website chart payload ownership remain unchanged.

---

## Source: Reports/v40.12.2_PRICING_VALUE_TERMINOLOGY_VERIFICATION.md

# v40.12.2 Pricing & Value Terminology Verification

- PASS: Performance vs Price selector terminology mapped.
- PASS: Value Rankings selector terminology mapped.
- PASS: Shared chart labels and tooltip labels mapped.
- PASS: Canonical native field keys remain `tensileFlat` and `tensileUpright`.
- PASS: WPF and SQLite schema remain unchanged.

---

## Source: Reports/v40.12.0_NATIVE_WEBSITE_NAVIGATION_FOUNDATION_VERIFICATION.md

# v40.12.0 Static Verification

## Scope

Native single-file website navigation foundation.

## Static checks

- Build identity updated to v40.12.0.
- Portal transform is applied after canonical website and experimental rendering.
- Existing `<main>` content is retained inside the Filament Database page.
- Experimental marker block is moved into the Experimental Lab page.
- Four portal page IDs and four hash routes are present.
- Portal CSS and JavaScript are injected idempotently.
- Verification Center includes portal navigation, page foundation and hash-routing gates.
- XAML remains unchanged.
- ZIP integrity checked after packaging.

## Runtime acceptance required

Build locally, run Verification Center, export preview and production HTML, and test existing database interactions plus tab/hash navigation in the browser.

---

## Source: Reports/v40.11.0_EXPERIMENTAL_WEBSITE_DATA_PIPELINE_VERIFICATION.md

# v40.11.0 Static Verification

## Scope
- SQLite schema v20 adds `MaterialExperiments.PublishOnWebsite` with a safe default of false.
- Existing series remain hidden until explicitly selected for publication.
- Main Website Preview and Production share the same experimental renderer.
- Renderer consumes active series, active runs, native result metrics and canonical analytics ranking.
- Hidden/inactive experimental data remains persisted.

## Static checks
- MainWindow.xaml parses as valid XML.
- Website publication column is bound two-way to `PublishOnWebsite`.
- Load and replace persistence paths include the new field.
- Canonical HTML injection has idempotent start/end markers.
- Build identity and documentation updated to v40.11.0.

Runtime acceptance is performed by the app Verification Center and local Visual Studio build.

---

## Source: Reports/v40.10.0_NATIVE_WEBSITE_TEMPLATE_DATABASE_VERIFICATION.md

# Verification Scope

- `WebsiteTemplates` table exists in schema v19.
- Exactly one active template is selected by activation operations.
- SHA-256 is stored for each distinct HTML body.
- Duplicate imports reactivate the existing version instead of duplicating content.
- Preview and production main exports read the active SQLite HTML.
- Output `index.html` is not read back as a template.
- Active template must contain `const DATA` and approved v36 master-surface markers.

---

## Source: Reports/v40.9.5_BUNDLED_WEBSITE_MASTER_TEMPLATE_VERIFICATION.md

# v40.9.5 Verification

## Static checks
- PASS: user-supplied HTML copied to `App/FilamentDbApp/Assets/website-template-index.html`.
- PASS: template contains a `const DATA=` block required by the renderer.
- PASS: template contains Tensile, Impact, Stiffness and Performance Profile chart surfaces.
- PASS: template contains v36 Pricing & Value marker, Pricing Explorer, Performance vs Price and Value Rankings surfaces.
- PASS: project copies the bundled template to the output directory.
- PASS: **Use Bundled** resolves through the existing bundled-template loader.
- PASS: Verification Center now validates master-template identity.

## Regression scope
No database schema, measurements, Results, Analytics, Experimental Dashboard or website payload calculations were changed.

---

## Source: Reports/v40.9.4_EXPERIMENTAL_CV_PIPELINE_VERIFICATION.md

# v40.9.4 Experimental CV Pipeline Verification

## Static verification
PASS – Experimental calculation converts `CoefficientOfVariation` ratio to percentage points exactly once.

PASS – Results rows append `%` without an additional conversion.

PASS – Dashboard reads canonical Results Table CV strings and compares percentage points to threshold 15.

PASS – Average and standard-deviation calculations remain unchanged.

## Expected probe
Input: 10, 500, 500, 500, 500, 10, 10, 11, 12, 9.
Expected CV: approximately 122.6%, therefore High variation.

---

## Source: Reports/v40.9.3_EXPERIMENTAL_DASHBOARD_CANONICAL_CV_VERIFICATION.md

# v40.9.3 Static Verification

- PASS: Dashboard CV readings originate from `_experimentalSeriesResultRows`, the Results Table source.
- PASS: Raw `GetExperimentalRunMetrics(...Cv)` values are no longer used by Dashboard quality evaluation.
- PASS: Threshold is 15.00 percentage points.
- PASS: Highest CV display uses the same parsed Table field.
- PASS: No database schema, measurement formula, analytics score or chart behavior changed.

---

## Source: Reports/v40.9.2_EXPERIMENTAL_DASHBOARD_CV_TRANSPARENCY_VERIFICATION.md

# v40.9.2 Verification Report

## Static verification
- MainWindow.xaml parsed successfully as XML.
- Dashboard contains named fields for Quality, Highest CV and CV source.
- CV threshold remains canonical ratio 0.15 (15%).
- Highest CV is selected across Tensile Upright, Tensile Flat, Impact Upright and Impact Flat for every Run.
- Dashboard percentage output uses the current culture and percentage formatting.
- No database schema or calculation service changes were made.

## Expected behavior
A Results-table maximum of 0.64% displays as Highest CV: 0.64% and does not trigger High variation. A maximum above 15% displays its Run and metric and triggers High variation.

---

## Source: Reports/Verification_v40.9.1.txt

~~~text
3DPIceland Engineering Platform v40.9.1
Experimental Dashboard CV Threshold Fix

Static verification:
PASS - StatisticsService returns coefficient of variation as standard deviation / average.
PASS - Dashboard high-variation threshold changed from > 15d to > 0.15d.
PASS - Warning text remains expressed as CV above 15%.
PASS - No database schema, measurement formula, ranking or chart behavior changed.

Example:
CV 1.175 = 117.5% and now triggers High variation.
CV 0.150 = 15.0% and does not trigger because the rule is above 15%.
~~~

---

## Source: Reports/VERIFICATION_v40.9.0.md

# Verification – v40.9.0

- Experimental dashboard controls added.
- Dashboard reads canonical ExperimentalAnalyticsService outputs.
- Completeness and quality indicators derive from native calculated measurement rows.
- Existing Table and Charts views retained.
- No database schema or formula changes.

---

## Source: Reports/v40.8.2_EXPERIMENTAL_CHART_HEADER_SPACING_VERIFICATION.md

# v40.8.2 Experimental Chart Header Spacing – Static Verification

- PASS: Line-chart plot top margin increased to 72 px.
- PASS: Legend lines/text moved to Y=46/Y=37 below the title.
- PASS: Y-axis unit is positioned relative to the plot top rather than title row.
- PASS: Bar-chart top margin increased to 62 px.
- PASS: No schema, Results Engine, Analytics Service or measurement persistence changes.

---

## Source: Reports/v40.8.1_EXPERIMENTAL-CHART-LAYOUT_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform
v40.8.1 EXPERIMENTAL-CHART-LAYOUT

STATIC AUDIT
PASS - Experimental root grid uses content-sized Series and Runs rows.
PASS - Series grid MinHeight=105, MaxHeight=210, VerticalScrollBarVisibility=Auto.
PASS - Runs grid MinHeight=105, MaxHeight=210, VerticalScrollBarVisibility=Auto.
PASS - Measurement/Results row uses remaining height and MinHeight=320.
PASS - No database, result calculation, analytics, measurement editor, or chart rendering code changed.
~~~

---

## Source: Reports/v40.8.0_EXPERIMENTAL-CHARTS-VISUALIZATION_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.8.0
Experimental Charts & Visualization – Static Audit

PASS  Package structure contains README.md, App/, Docs/, Reports/.
PASS  MainWindow XAML parses as valid XML.
PASS  Existing Experimental Results DataGrid remains present.
PASS  Five chart canvases are present: Tensile, Impact, Stiffness, Score, Baseline.
PASS  Charts consume GetExperimentalRunMetrics and ExperimentalAnalyticsService output.
PASS  Measurement edit completion calls RefreshExperimentalSeriesResults, which refreshes charts.
PASS  Baseline and Series selection paths refresh Experimental Results and charts.
PASS  No database schema or native mechanical formula changes were introduced.
PASS  v40.7.1 Impact Flat delta display property and binding are preserved.
PASS  Verification Center includes chart-surface and analytics-alignment checks.

Build note: static validation completed in the sandbox. Local Visual Studio build and runtime UI verification remain part of What to Test.
~~~

---

## Source: Reports/v40.7.1_EXPERIMENTAL-IMPACT-FLAT-DELTA-FIX_Static_Audit.txt

~~~text
3DPIceland Engineering Platform
Version: v40.7.1 EXPERIMENTAL-IMPACT-FLAT-DELTA-FIX

STATIC AUDIT
PASS  ExperimentalSeriesResultRow exposes ImpactFlatDeltaDisplay
PASS  Experimental Results XAML binds Impact Flat Δ Baseline to ImpactFlatDeltaDisplay
PASS  Binding is explicit Mode=OneWay
PASS  RefreshExperimentalSeriesResults populates ImpactFlatDeltaDisplay
PASS  Verification Center contains deterministic Impact Flat delta display check
PASS  Version metadata updated to 40.7.1
PASS  No database schema or canonical ResultsService formulas changed

Runtime acceptance target:
- Baseline row shows 0.00% when Impact Flat has a result.
- Non-baseline rows show the calculated percentage relative to baseline.
~~~

---

## Source: Reports/v40.7.0_EXPERIMENTAL-ANALYTICS-ENGINE_Static_Audit.txt

~~~text
3DPIceland Engineering Platform
v40.7.0 EXPERIMENTAL-ANALYTICS-ENGINE

STATIC AUDIT
============
PASS  Package structure preserved: README.md, App/, Docs/, Reports/
PASS  MainWindow.xaml parses as valid XML
PASS  FilamentDbApp.csproj parses as valid XML
PASS  ExperimentalAnalyticsService consumes calculated Run metrics only
PASS  Best-result selection covers five canonical Experimental metrics
PASS  Overall score weights are Tensile 40%, Impact 40%, Stiffness 20%
PASS  Partial metric availability is reweighted rather than treated as zero
PASS  Results table exposes Rank and Overall Score
PASS  Summary UI exposes Baseline, best metrics and Recommended Run
PASS  Verification Center includes deterministic analytics checks
PASS  Version metadata updated to 40.7.0

Build execution was not performed in this environment because the .NET SDK is unavailable.
~~~

---

## Source: Reports/STATIC_AUDIT_v40.6.1.txt

~~~text
3DPIceland Engineering Platform - Static Audit
Version: v40.6.1 EXPERIMENTAL-SERIES-CONTEXT-RESET-FIX

PASS  Atomic Series context workflow added
PASS  Previous Run selection cleared before child view refresh
PASS  Measurement editors cleared for Series without Runs
PASS  Results collection cleared and rebuilt for selected Series
PASS  Deferred callback protected by context-version token
PASS  Populated Series reselects and activates a valid Run
PASS  SQLite schema unchanged
PASS  Native calculation engine unchanged

Local .NET compilation was not available in the packaging environment.
~~~

---

## Source: Reports/v40.6.0_EXPERIMENTAL_RESULTS_ENGINE_VERIFICATION.md

# v40.6.0 Experimental Results Engine – Static Verification

- Package source: v40.5.9 verified baseline.
- Input verification supplied by user: 126 / 126 PASS.
- Added Results DataGrid for selected Test Series.
- Added baseline highlight and percentage-delta calculation.
- Added average/CV aggregation from existing ExperimentalMeasurementRecord values.
- Added refresh hooks for edits, baseline changes, Run CRUD and Series changes.
- Added Verification Center check: Experimental results engine.
- SQLite schema unchanged.
- MainWindow.xaml and project XML parse successfully.
- Local .NET compilation was not available in the packaging environment; user build is the executable acceptance gate.

---

## Source: Reports/v40.5.9_EXPERIMENTAL-MEASUREMENT-VERIFICATION-GATE_Static_Audit.txt

~~~text
3DPIceland Engineering Platform v40.5.9
Experimental Measurement Verification Gate – Static Audit

Implemented checks:
PASS – exact five-row canonical shape definition
PASS – duplicate MeasurementID detection
PASS – duplicate RunID/type/orientation detection
PASS – RunID ownership and orphan detection
PASS – native input-limit validation
PASS – Stiffness one-specimen input shape
PASS – ResultsService average/sample-count comparison logic

Runtime verification must be confirmed against the user SQLite database in Verification Center.
~~~

---

## Source: Reports/v40.5.8_EXPERIMENTAL_VALIDATION_BUILD_FIX_VERIFICATION.md

# v40.5.8 Static Verification

- Corrected incompatible null-coalescing operands (`DataGridCell` and `TextBox`).
- Parent DataGrid lookup receives a `DependencyObject`.
- No SQLite schema change.
- Input limits remain identical to v40.5.7.

---

## Source: Reports/v40.5.7_EXPERIMENTAL_INPUT_LIMITS_VALIDATION_VERIFICATION.md

# v40.5.7 Static Verification

## Scope
Experimental measurement input limits and validation.

## Verified in source
- Experimental grids are connected to `MeasurementGrid_PreparingCellForEdit`.
- Experimental sample columns are recognized as numeric measurement inputs.
- Tensile maximum is 505.
- Impact maximum is 100.
- Stiffness maximums are 10 revolutions and 359 degrees.
- Typing and paste paths use measurement-aware range validation.
- `ExperimentalMeasurementRecord` includes a persistence-level range guard.
- No SQLite schema changes were introduced.

## Local runtime test required
Build and run the application, test each boundary and run Verification Center.

---

## Source: Reports/v40.5.6_EXPERIMENTAL_SINGLE_CLICK_EDITOR_FIX_VERIFICATION.md

# v40.5.6 Static Verification

- Experimental measurement `CellEditEnding` no longer commits the DataGrid from its deferred callback.
- The callback only recalculates, persists and refreshes status at ContextIdle.
- This prevents a stale callback from closing a newly activated input editor.
- SQLite schema and calculation services are unchanged.

Runtime acceptance: confirm consecutive input cells accept typing after a single click and Verification Center reports Overall PASS.

---

## Source: Reports/v40.5.5_EXPERIMENTAL_KEYBOARD_NAVIGATION_VERIFICATION.md

# v40.5.5 Static Verification

## Primary objective
Enable canonical keyboard cell navigation in all Experimental measurement editors.

## Static checks
- PASS: Experimental Tensile grid is registered in `WorkflowGridNames`.
- PASS: Experimental Impact grid is registered in `WorkflowGridNames`.
- PASS: Experimental Stiffness grid is registered in `WorkflowGridNames`.
- PASS: Registered grids receive `InputDataGrid_PreviewKeyDown` with handled events enabled.
- PASS: Right/Left/Up/Down navigation uses the existing proven workflow handler.
- PASS: Enter and Tab commit and move through editable columns.
- PASS: Read-only result columns are excluded by `GetEditableWorkflowColumns`.
- PASS: No SQLite schema or calculation-formula changes.

## Runtime regression test
Use Right Arrow after entering each Tensile and Impact sample. Confirm values commit, calculations update, and focus advances without a mouse click.

---

## Source: Reports/v40.5.4_EXPERIMENTAL_RUN_SELECTION_REBIND_FIX_VERIFICATION.md

# v40.5.4 Static Verification

## Scope
Experimental Run selection and measurement-editor context rebinding.

## Verified in source
- Run row clicks explicitly set `SelectedItem` and `CurrentCell`.
- `ActivateExperimentalRun` is deferred until the DataGrid current-cell transition completes.
- The helper ensures rows and binds Tensile, Impact and Stiffness editors for the selected RunID.
- Existing measurement persistence and calculation paths remain unchanged.
- SQLite schema remains unchanged.

## Runtime verification required
Use the README `What to test now` checklist and run Verification Center locally.

---

## Source: Reports/v40.5.3_EXPERIMENTAL_LAYER_ADHESION_DEDUPLICATION_VERIFICATION.md

# v40.5.3 Static Verification

- Separate Experimental Layer Adhesion tab removed.
- Experimental editor binding references only Tensile, Impact and Stiffness grids.
- Per-run initialization creates Tensile Upright/Flat, Impact Upright/Flat and one Stiffness row.
- Obsolete dedicated Layer Adhesion rows are removed when runs are initialized.
- Tensile Upright remains available as the layer-adhesion measurement.
- SQLite schema unchanged.

---

## Source: Reports/v40.5.2_VERIFICATION_NOTES.txt

~~~text
3DPIceland Engineering Platform v40.5.2
Experimental Input Visibility & Rebind Fix

Static package checks:
PASS - MainWindow.xaml is valid XML.
PASS - Four experimental editor DataGrids retain explicit editable sample columns.
PASS - Editor area and grids have explicit minimum height and scrollbars.
PASS - Selected run is rebound after series view refresh.
PASS - Measurement tab change rebinds the current run.
PASS - SQLite schema unchanged.

Local runtime verification required through Visual Studio and Verification Center.
~~~

---

## Source: Reports/v40.5.1_EXPERIMENTAL_EDITOR_ROW_STABILITY_VERIFICATION.md

# v40.5.1 Static Verification

## Confirmed in source
- Experimental editor ItemsSource values are fixed lists scoped to the selected RunID.
- No experimental measurement editor uses a filtering CollectionView.
- Row creation occurs before editor binding.
- CellEditEnding captures the edited row RunID.
- DataGrid edit is committed before ResultsService calculation and SQLite persistence.
- Existing specialized Tensile, Impact, Stiffness and Layer Adhesion editors remain present.

## Local runtime verification required
1. Select one run and confirm all expected rows remain visible.
2. Enter Tensile values in both Upright and Flat.
3. Enter Impact values in both Upright and Flat.
4. Confirm calculated results update.
5. Change tabs repeatedly; rows must remain visible.
6. Select another run and confirm separate values.
7. Return to the first run and confirm persistence.
8. Run Verification Center.

---

## Source: Reports/v40.5.0_EXPERIMENTAL_MEASUREMENT_EDITOR_REDESIGN_VERIFICATION.md

# v40.5.0 Experimental Measurement Editor Redesign – Static Verification

- Version metadata updated to 40.5.0.
- Generic `ExperimentalMeasurementsGrid` removed from XAML.
- Dedicated editor grids present for Tensile, Impact, Stiffness and Layer Adhesion.
- All editor grids use a shared deferred `ExperimentalMeasurementEditor_CellEditEnding` handler.
- `RecalculateExperimentalMeasurements` no longer calls `Items.Refresh()`.
- Stiffness columns are explicitly labelled Revolutions and Degrees.
- Existing RunID-owned SQLite measurement model remains unchanged.
- Verification Center includes editor-redesign and edit-transaction-safety checks.

Runtime compilation and Verification Center execution should be performed in Visual Studio on the target Windows machine.

---

## Source: Reports/v40.4.1_IMPLEMENTATION_VERIFICATION.txt

~~~text
v40.4.1 Experimental Native Measurement Context

Implementation review:
PASS - Experimental rows are owned by RunID.
PASS - Tensile Upright and Flat expose 10 raw N inputs.
PASS - Impact Upright and Flat expose 10 raw percent inputs.
PASS - Stiffness uses one revolutions/degrees specimen.
PASS - Layer Adhesion exposes 10 raw N inputs.
PASS - ResultsService is reused for calculations and statistics.
PASS - Existing native Settings values are reused.
PASS - SQLite schema version advanced to 18.

Local Visual Studio build and runtime regression testing required.
~~~

---

## Source: Reports/v40.4.0_IMPLEMENTATION_VERIFICATION.txt

~~~text
v40.4.0 Experimental Measurement Entry

Implemented:
- ExperimentalMeasurements SQLite table
- Canonical ExperimentalMeasurementId and RunID foreign key
- Four measurement types per selected run
- Five samples, automatic count and average
- Persistence restoration across existing replace workflows
- Run/series cascade cleanup
- Verification Center gates

Local Visual Studio build and runtime verification required.
~~~

---

## Source: Reports/v40.3.1_STATIC_VERIFICATION.txt

~~~text
3DPIceland Engineering Platform v40.3.1
EXPERIMENTAL-DEFERRED-RUN-REFRESH-FIX

STATIC VERIFICATION
PASS Immediate _experimentalRunView.Refresh removed from PreviewMouseLeftButtonDown
PASS Refresh runs only when selected series changes
PASS Refresh deferred through Dispatcher at ContextIdle
PASS Active toggle and persistence flow preserved
PASS Experimental Test Series & Runs schema remains version 16

Runtime acceptance requires local build plus Verification Center PASS.
~~~

---

## Source: Reports/Verification_v40.3.0.txt

~~~text
3DPIceland Engineering Platform - v40.3.0 Static Build Verification

PASS  Package structure                    README.md, App/, Docs/, Reports/
PASS  Experimental run model               ExperimentalRunRecord with canonical RunID and SeriesID
PASS  SQLite schema foundation             ExperimentalRuns table and SeriesID index; schema version 16
PASS  Test series ownership                Existing MaterialExperiments rows retain canonical MaterialID
PASS  Multi-run persistence                Run rows load and save through LocalDatabase
PASS  Parent-child deletion                Series deletion removes its in-memory child runs; SQLite CASCADE remains enabled
PASS  Series save preservation             Child runs are restored after replace-based series persistence
PASS  Experimental run CRUD UI             Add, Duplicate and Delete Run controls available
PASS  Baseline workflow                    One run per series can be marked as baseline through the UI
PASS  Verification Center integration      Run counts, orphan checks and CRUD integrity gates added

Runtime acceptance requires local Visual Studio build and Verification Center execution.
~~~

---

## Source: Reports/v40.2.10_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.10
EXPERIMENTAL-ACTIVE-NOTIFICATION-FIX

Static audit: PASS
- Model notification implemented
- SQLite schema unchanged
- MaterialID architecture unchanged
- Existing CRUD handlers preserved
~~~

---

## Source: Reports/v40.2.10_EXPERIMENTAL_ACTIVE_NOTIFICATION_FIX_VERIFICATION.md

# v40.2.10 Verification Scope

## Experimental Active Notification

- `MaterialExperimentRecord` implements `INotifyPropertyChanged`.
- `IsActive` raises `PropertyChanged` immediately.
- `UpdatedAtUtc` raises `PropertyChanged` immediately.
- Active changes continue to persist through `SaveMaterialExperiments()`.
- No collection-view or filter refresh is used to make the checkbox repaint.

Expected gate: `PASS Experimental active notification`.

---

## Source: Reports/v40.2.9_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.9
EXPERIMENTAL-ROW-CONTEXT-FIX
Static source audit completed.
~~~

---

## Source: Reports/v40.2.9_EXPERIMENTAL_ROW_CONTEXT_FIX_VERIFICATION.md

# v40.2.9 Verification Scope

- Experimental grid is excluded from legacy WorkflowGridNames.
- Dedicated click handling tracks the active MaterialExperimentRecord without row selection.
- Duplicate/Delete have current-cell and last-clicked record resolution.
- Active checkbox uses direct first-click persistence.
- SQLite schema and MaterialID links are unchanged.

---

## Source: Reports/v40.2.8_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.8
EXPERIMENTAL-EDITING-RESTORE

PASS XAML experimental grid uses IsReadOnly=False
PASS XAML experimental grid uses SelectionUnit=CellOrRowHeader
PASS ComboBox and text bindings remain TwoWay
PASS Delete retains current-cell row fallback
PASS SQLite schema unchanged
~~~

---

## Source: Reports/v40.2.8_EXPERIMENTAL_EDITING_RESTORE_VERIFICATION.md

# v40.2.8 Verification Scope

- Experimental grid is writable.
- SelectionUnit is CellOrRowHeader, allowing cell editing and row-header selection.
- Delete can resolve a record from SelectedItem or CurrentCell.Item.
- No SQLite schema changes.
- MaterialID remains canonical.

---

## Source: Reports/v40.2.7_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.7
EXPERIMENTAL-BUILD-FIX

PASS Experimental grid full-row selection configured
PASS Delete resolves selected/current experiment record
PASS Clear Filters uses deferred refresh after edit commit
PASS SQLite schema unchanged
PASS MaterialID remains canonical
~~~

---

## Source: Reports/v40.2.7_EXPERIMENTAL_BUILD_FIX_VERIFICATION.md

# v40.2.7 Verification Scope

- Experimental grid uses `SelectionUnit=FullRow` with single selection.
- Delete supports both `SelectedItem` and `CurrentCell.Item`.
- Clear Filters suppresses duplicate UI change events.
- Collection-view refresh is dispatched after DataGrid edit commit.
- No SQLite schema or MaterialID changes.

---

## Source: Reports/v40.2.6_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.6
EXPERIMENTAL-SELECTION-FILTER-FIX

PASS Experimental grid full-row selection configured
PASS Delete resolves selected/current experiment record
PASS Clear Filters uses deferred refresh after edit commit
PASS SQLite schema unchanged
PASS MaterialID remains canonical
~~~

---

## Source: Reports/v40.2.6_EXPERIMENTAL_SELECTION_FILTER_FIX_VERIFICATION.md

# v40.2.6 Verification Scope

- Experimental grid uses `SelectionUnit=FullRow` with single selection.
- Delete supports both `SelectedItem` and `CurrentCell.Item`.
- Clear Filters suppresses duplicate UI change events.
- Collection-view refresh is dispatched after DataGrid edit commit.
- No SQLite schema or MaterialID changes.

---

## Source: Reports/v40.2.5_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.5
EXPERIMENTAL-COMBOBOX-EDIT-FIX

PASS - CellEditEnding does not call RefreshExperimentalChoices
PASS - ComboBox column ItemsSource is initialized outside edit transaction
PASS - Edited row labels update independently
PASS - Explicit SQLite persistence retained
PASS - MaterialID remains canonical
~~~

---

## Source: Reports/v40.2.5_EXPERIMENTAL_COMBOBOX_EDIT_FIX_VERIFICATION.md

# v40.2.5 Verification Scope

- Stable Material/Experiment/Baseline ComboBox ItemsSource collections.
- No `RefreshExperimentalChoices()` call from `MaterialExperimentsGrid_CellEditEnding`.
- No active `ListCollectionView.Refresh()` during ComboBox commit.
- Edited row display labels update without rebuilding DataGrid columns.
- SQLite persistence and MaterialID integrity remain unchanged.
- Verification Center exposes `Experimental ComboBox edit safety`.

---

## Source: Reports/v40.2.4_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.4
EXPERIMENTAL-POST-ADD-UI-FIX

PASS  Add Experiment creates and persists a row without forced DataGrid focus.
PASS  Duplicate creates and persists a row without forced selection or scrolling.
PASS  Existing SQLite experimental rows are preserved.
PASS  Verification Center includes post-add UI safety coverage.
~~~

---

## Source: Reports/v40.2.4_EXPERIMENTAL_POST_ADD_UI_FIX_VERIFICATION.md

# v40.2.4 Verification Scope

## Primary objective
Prevent Experimental Add and Duplicate from crashing after a row has already been created and persisted.

## Verified architecture
- SQLite remains Source of Truth.
- MaterialID remains canonical.
- ObservableCollection notification displays newly created rows.
- Add/Duplicate do not force DataGrid SelectedItem, ScrollIntoView, BeginEdit, or immediate CollectionView refresh.
- Verification Center exposes Experimental post-add UI safety.

## Regression scope
No schema, calculation, website, reporting, purchasing, inventory, or native measurement workflow changes.

---

## Source: Reports/v40.2.3_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.3
EXPERIMENTAL-COLLECTION-REENTRANCY-FIX

PASS CollectionChanged persistence removed
PASS Add workflow phases separated
PASS Duplicate workflow phases separated
PASS MaterialID architecture unchanged
PASS SQLite schema unchanged
~~~

---

## Source: Reports/v40.2.3_EXPERIMENTAL_COLLECTION_REENTRANCY_FIX_VERIFICATION.md

# v40.2.3 Verification Scope

- Add Experiment does not persist or refresh from CollectionChanged.
- Collection mutation, SQLite persistence, and DataGrid refresh run as separate phases.
- Duplicate follows the same safe workflow.
- SQLite schema and MaterialID integrity remain unchanged.

---

## Source: Reports/v40.2.2_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.2
EXPERIMENTAL-ADD-CRASH-FIX

STATIC AUDIT
PASS Add Experiment does not commit the unrelated Native Materials grid
PASS CurrentCell is assigned before BeginEdit
PASS BeginEdit is deferred until the DataGrid has selected and scrolled the new row
PASS transient InvalidOperationException from WPF focus state is contained
PASS MaterialID and experiment definition defaults remain SQLite-backed
PASS package structure contains App, Docs, Reports and root README.md
~~~

---

## Source: Reports/v40.2.2_EXPERIMENTAL_ADD_CRASH_FIX_VERIFICATION.md

# v40.2.2 Verification Scope

- Add Experiment no longer invokes the Native Materials commit workflow.
- A current editable cell is assigned before automatic `BeginEdit`.
- Deferred WPF focus transitions cannot crash the application.
- Verification Center exposes `Experimental add workflow safety`.
- Existing experimental persistence and MaterialID integrity gates remain active.
- SQLite schema and canonical identifiers are unchanged.

---

## Source: Reports/v40.2.1_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.1
PASS Experimental grid explicitly IsReadOnly=False.
PASS Cell-based editing enabled.
PASS Value, Unit and Notes use TwoWay bindings.
PASS Material, Experiment and Baseline combo columns retain TwoWay SelectedValue bindings.
PASS Experimental grid removed from incompatible workflow click routing.
PASS Verification Center editability gate added.
PASS Package folders App, Docs, Reports and root README present.
~~~

---

## Source: Reports/v40.2.1_EXPERIMENTAL_EDITING_FIX_VERIFICATION.md

# v40.2.1 Verification Scope

- Experimental grid is explicitly writable.
- Editable text columns use two-way bindings.
- MaterialExperimentsGrid is excluded from the legacy native workflow click handler.
- Verification Center exposes `Experimental grid editability`.
- SQLite schema and MaterialID integrity remain unchanged.

---

## Source: Reports/v40.2.0_STATIC_AUDIT.txt

~~~text
3DPIceland Engineering Platform v40.2.0
Experimental Material Manager - Static Audit

PASS Package root contains README.md, App/, Docs/ and Reports/.
PASS MainWindow XAML parses successfully.
PASS Project XML parses successfully.
PASS MaterialExperiments CRUD persistence methods are present.
PASS Experimental Testing UI contains Add, Duplicate, Delete, search and Active filter controls.
PASS Material and baseline selectors store canonical MaterialID values.
PASS Experiment selector consumes SQLite ExperimentDefinitions.
PASS Verification Center contains UI, persistence alignment and CRUD integrity gates.
PASS Release identity and documentation updated to v40.2.0.

Runtime acceptance target: local Visual Studio build and Verification Center Overall PASS.
~~~

---

## Source: Reports/v40.2.0_EXPERIMENTAL_MATERIAL_MANAGER_VERIFICATION.md

# v40.2.0 Verification Scope

Static implementation checks:
- Experimental Testing tab and CRUD handlers are present.
- MaterialExperiments load/replace persistence is implemented in LocalDatabase.
- Material and baseline values remain canonical MaterialID references.
- Definition choices are loaded from ExperimentDefinitions in SQLite.
- Verification Center includes UI, persistence alignment and CRUD integrity gates.

Runtime acceptance requires the user-generated Verification Center diagnostic after local build.

---

## Source: Reports/v40.1.0_EXPERIMENTAL_DEFINITION_FOUNDATION_VERIFICATION.md

# v40.1.0 Static Verification

- PASS: ZIP structure preserved.
- PASS: SQLite schema version advanced to 15.
- PASS: ExperimentDefinitions and MaterialExperiments added with foreign keys and indexes.
- PASS: Ten generic experiment definitions seeded idempotently.
- PASS: MaterialID remains the canonical material link.
- PASS: Verification Center includes definition availability and referential-integrity gates.
- PASS: Existing calculation, website, reporting, purchasing, and inventory pipelines were not intentionally changed.

---

## Source: Reports/v39.1.3_MATERIALS_WORKFLOW_COLUMN_LAYOUT_VERIFICATION.md

# v39.1.3 Materials Workflow Column Layout – Verification Notes

## Scope
Focused Materials-grid layout change only.

## Expected layout
After Color:
- Manufacturer Website
- YouTube Review URL
- Video
- Tested Status
- In Tensile
- In Impact
- In Stiffness
- Notes

## Data safety
- DiameterMm remains part of NativeMaterialRow.
- Diameter remains available to persistence, import/export, website, and reporting pipelines.
- No database schema or calculation changes were made.

## Regression target
- Material editing and auto-save remain unchanged.
- Purchase-to-material pricing and storage sync remain unchanged.
- Website, reporting, measurement, and inventory pipelines remain unchanged.

---

## Source: Reports/v39.1.2_PURCHASE_TO_MATERIAL_STORAGE_SYNC_VERIFICATION.md

# v39.1.2 Purchase-to-Material Storage Sync Verification

## Static verification scope
- `SyncMaterialPricingFromPurchaseLine` copies non-empty `PurchaseOrderLineRecord.StorageLocation` to `NativeMaterialRow.StorageLocation`.
- `CreateMaterialFromPurchaseLine` initializes Storage Location from the selected purchase line.
- Blank source locations preserve the existing Material location.
- Verification Center test data includes `Shelf A-2` and requires the destination Material to match.

## Runtime acceptance
Run the Verification Center after building locally. The purchase-to-material pricing sync check must PASS and all existing gates must remain PASS.

---

## Source: Reports/v39.1.1_PURCHASE_TO_MATERIAL_PRICING_SYNC_VERIFICATION.md

# v39.1.1 Purchase to Material Pricing Sync – Verification

## Acceptance criteria
- Invoice unit price populates both Purchase Price and MSRP Amount.
- Purchase order currency populates Purchase Currency, MSRP Currency and Landed Cost Currency.
- Calculated landed unit cost populates Material Landed Cost.
- Allocated shipping and tax populate per-unit Material values.
- Recalculation updates existing linked materials.
- New material creation uses the same synchronization path.
- Verification Center includes `Purchase-to-material pricing sync`.

---

## Source: Reports/v39.1.0_DAILY_WORKFLOW_LIVE_STATUS_VERIFICATION.md

# v39.1.0 Daily Workflow Live Status – Static Verification

## Release objective
Make daily material-entry status accurate, immediate, and readable without changing SQLite storage or calculation behavior.

## Static checks
- PASS – Application release identity updated to v39.1.0.
- PASS – Native material count derives from `_nativeMaterialRows`.
- PASS – Filtered visible count derives from the active native collection view.
- PASS – Zero-result filters remain zero and are surfaced as `0 visible`.
- PASS – Counts refresh after native material filter changes.
- PASS – Tensile sample cells use explicit dark foreground text.
- PASS – Impact sample cells use explicit dark foreground text.
- PASS – Default window dimensions increased; saved workflow preferences remain authoritative.
- PASS – No intentional SQLite schema, import/export, measurement calculation, website, report, purchasing, or inventory changes.

---

## Source: Reports/v38.5.0_PURCHASING_INTELLIGENCE_VERIFICATION.md

# v38.5.0 Purchasing Intelligence – Static Verification

## Implemented
- Purchasing Intelligence report key is recognized by the canonical report workflow.
- Report selector exposes Purchasing Intelligence Report.
- Report generation consumes Purchase Orders, Purchase Order Lines and Inventory rows already loaded from SQLite.
- Currency conversion reuses the hardened purchasing numeric parser and persisted ExchangeRate.
- Material price history uses persisted LandedUnitCost values and converts them to ISK.
- Recommendations are deterministic and explain their triggering condition.
- Verification Center requires all six purchasing reports and includes a dedicated Purchasing Intelligence check.

## Safety
- No database schema migration.
- No edits to purchasing persistence.
- No changes to landed-cost allocation.
- No changes to inventory calculations.
- No changes to website or engineering report pipelines.

## Local verification required
- Visual Studio build
- Report preview
- HTML/package export
- PDF export
- Verification Center PASS

---

## Source: Reports/v38.4.2.3_EXECUTIVE_DASHBOARD_VERIFICATION.md

# v38.4.2.3 Executive Dashboard Verification

## Static scope

- Inventory Report: six-card overview generated from inventory rows.
- Purchase Report: six-card overview generated from persisted landed-cost values.
- Supplier Report: six-card overview generated from supplier groups.
- Detailed report tables remain present below each dashboard.
- Existing tolerant currency parsing remains active.
- No database schema or persistence changes.

## User verification

Run the application locally, compare dashboard values with Inventory and Purchase Orders, export HTML/PDF, and run Verification Center.

---

## Source: Reports/v38.4.2.2_CURRENCY_PARSE_VERIFICATION.md

# v38.4.2.2 Currency Parse Verification

## Regression target

Purchasing report conversion must interpret stored decimal rates consistently across Icelandic and invariant number formats.

## Required checks

- `149.70` parses as 149.70, not 14970.
- `149,70` parses as 149.70.
- Supplier Report and Purchase Report use the same normalized values.
- Existing landed-cost persistence and allocation behavior remain unchanged.

---

## Source: Reports/v38.4.2.1_BUILD_FIX_VERIFICATION.md

# v38.4.2.1 Build Fix Verification

- Corrected compiler error CS9006 in `PurchasingReportService.cs`.
- HTML/CSS output remains functionally unchanged.
- Purchasing report models, report selection, package export, and PDF workflow are unchanged.
- Local Visual Studio build and runtime verification required.

---

## Source: Reports/v38.4.2_RELEASE_VERIFICATION.md

# v38.4.2 Static Release Verification

- PurchasingReportService added.
- Five purchasing report types added to Reports / PDF Export.
- Canonical HTML and text payload generation implemented.
- Existing WebView2 HTML-to-PDF workflow reused.
- Inventory integrity report checks negative remaining weight, missing links/currency, orphan links, missing calculated landed cost and duplicate batch numbers.
- Five Verification Center checks added.
- Source-level inspection completed.
- Local compilation was not available in this environment; build and runtime validation are included in the user test checklist.

---

## Source: Reports/v38.4.1.7_RELEASE_VERIFICATION.md

# v38.4.1.7 Release Verification

## Scope
Landed-cost result persistence only.

## Static verification
- `PurchaseOrderLines` schema contains all calculated fields.
- Existing databases receive the fields through backward-compatible migration.
- `ReplacePurchaseOrders` writes all calculated fields.
- `LoadPurchaseOrderLines` restores all calculated fields.
- Cost calculation still saves immediately through the existing Purchase Order save path.

## Required local verification
1. Calculate landed costs successfully.
2. Restart the application.
3. Confirm net, allocated and landed values remain unchanged.
4. Recalculate after changing an input and repeat the restart test.

## Regression scope
Currency dropdown, exchange-rate live sync, receiving, inventory linkage and strict weight validation are intentionally unchanged.

---

## Source: Reports/v38.4.1.6_RELEASE_VERIFICATION.md

# v38.4.1.6 Release Verification

## Primary objective

Synchronize Purchase Order exchange rates immediately after Purchasing currency settings are edited.

## Verification scope

- Settings Manager currency edit commits before synchronization.
- Matching Purchase Orders receive the new ISK rate without currency re-selection.
- Purchase Orders are saved after synchronization.
- Purchase Order grid refresh waits for active DataGrid edit transactions to finish.
- Currency dropdown first-load stability remains preserved.
- Cost allocation and strict weight validation remain unchanged.

---

## Source: Reports/v38.4.1.5_RELEASE_VERIFICATION.md

# v38.4.1.5 Release Verification

## Defect
Selecting a Purchase Order currency could call `DataGrid.Items.Refresh()` from the currency-rate autofill path while WPF still held an AddNew/EditItem transaction, producing an intermittent `InvalidOperationException`.

## Fix
- Removed the immediate grid refresh from the currency-rate application path.
- Added an ApplicationIdle refresh scheduler.
- The scheduler checks `IEditableCollectionView.IsAddingNew` and `IsEditingItem` and retries only after both states are clear.
- Currency normalization and Settings Manager rate lookup are unchanged.

## Regression scope
- Purchase Order persistence unchanged.
- Cost allocation unchanged.
- Currency list and configurable rates unchanged.
- No database schema change.

---

## Source: Reports/v38.4.1.3_RELEASE_VERIFICATION.md

# v38.4.1.3 Release Verification

Primary objective: prevent silent fallback when a user explicitly selects By weight.

Static checks completed:
- strict pre-allocation validation added
- all calculated allocation fields remain cleared on validation failure
- validation dialog and Purchase Orders validation panel added
- Automatic fallback behavior preserved
- Verification Center synthetic regression check added

Local Visual Studio build and runtime verification are included in the user test checklist.

---

## Source: Reports/v38.4.1_IMPLEMENTATION_VERIFICATION.md

# v38.4.1 Implementation Verification

- Package structure verified: README.md root; App/, Docs/, Reports/.
- XAML and project XML parsed successfully.
- SQLite schema migration updated from v12 to v13.
- Purchasing cost allocation service added.
- Mixed-order automatic shipping fallback implemented.
- Purchase Order and Purchase Line persistence updated for allocation settings.
- Inventory spool persistence updated for customs, fees and landed cost.
- Local compile and runtime verification remain part of the What to Test checklist.

---

## Source: Reports/v38.4.0_VERIFICATION_SCOPE.txt

~~~text
3DPIceland FilamentDB v38.4.0 PURCHASING-PLATFORM-FOUNDATION

Static release verification performed in the packaging environment:
- Version metadata aligned to 38.4.0.
- SQLite schema constant and AppMeta schema aligned to v12.
- PurchaseOrderLineRecord includes InventoryCategory with Filament default.
- Database load/save includes InventoryCategory.
- Purchase Order UI exposes six supported categories.
- Receiving routes only Filament lines into Material/Inventory Spool creation.
- Non-filament categories remain recorded without downstream conversion.
- ADR-001 through ADR-007 and split platform roadmaps included.

Local Visual Studio build and runtime Verification Center remain part of the user acceptance checklist.
~~~

---

## Source: Reports/STATIC_AUDIT_v38.3.3.txt

~~~text
v38.3.3 INVENTORY-DELETE-FIX – Static Audit

PASS: DeleteInventorySpool_Click uses OfType<InventorySpoolRecord>().
PASS: WPF NewItemPlaceholder cannot be cast during next-row selection.
PASS: Inventory deletion persists immediately.
PASS: Material quantities and Inventory summary refresh after deletion.
PASS: Release identity updated to 38.3.3.
~~~

---

## Source: Reports/STATIC_AUDIT_v38.3.2.txt

~~~text
v38.3.2 RECEIVING-AUTOMATION – Static Audit

PASS: Release identity updated to 38.3.2.
PASS: Purchase Order and Purchase Order Line edits are committed before receiving automation.
PASS: Unlinked received lines require a description and explicit confirmation before Material creation.
PASS: Created MaterialID is assigned to the Purchase Order Line before inventory generation.
PASS: Native Materials and Purchase Orders are persisted before Inventory Spools are generated.
PASS: Existing PurchaseOrderLineId spool count prevents duplicate spool creation.
PASS: StorageLocation is copied from Purchase Order Line to each InventorySpoolRecord.
PASS: Material-level StorageLocation is not overwritten by receiving automation.
PASS: No SQLite schema change.
PASS: Documentation and test checklist updated.
~~~

---

## Source: Reports/STATIC_AUDIT_v38.3.1.txt

~~~text
v38.3.1 PURCHASE-WORKFLOW-RECEIVING – STATIC AUDIT

PASS: package layout contains README.md, App, Docs, Reports.
PASS: BuildInfo and project version updated to v38.3.1.
PASS: XAML parses as well-formed XML.
PASS: Purchase order schema includes lifecycle and received date.
PASS: Purchase line schema includes received quantity, receiving status, and storage location.
PASS: migrated databases use explicit-column INSERT statements, avoiding column-order dependency.
PASS: inventory generation is based on ReceivedQuantity and blocks unlinked received lines.
PASS: attachment data is stored as a relative path; file content is outside SQLite.
PASS: landed-cost calculations were not added, preserving the single-objective release scope.

BUILD STATUS: .NET SDK is not installed in this execution environment; compile/runtime validation must be performed in Visual Studio 2022 using the included What to Test checklist.
~~~

---

## Source: Reports/STATIC_AUDIT_v38.3.0.txt

~~~text
v38.3.0 static audit

PASS Package root contains README.md only plus App/ Docs/ Reports/ directories.
PASS BuildInfo and project metadata identify v38.3.0.
PASS MainWindow.xaml is well-formed XML.
PASS Purchase Orders and Purchase Order Lines models exist.
PASS SQLite schema version advanced from 9 to 10.
PASS PurchaseOrders and PurchaseOrderLines tables include relational keys.
PASS InventorySpoolItems includes PurchaseOrderLineId linkage.
PASS Purchase Orders tab contains order grid, line grid and inventory creation workflow.
PASS Inventory generation checks existing linked spools before creating missing records.
PASS Landed-cost allocation is not prematurely implemented in this foundation release.
NOTE Runtime compilation must be completed in the user's Visual Studio environment.
~~~

---

## Source: Reports/v38.2.11_STATIC_VERIFICATION.txt

~~~text
3DPIceland FilamentDB Engineering Platform
v38.2.11 INVENTORY-EDIT-COMMIT-FIX

Static Verification
-------------------
PASS  Release identity updated to 38.2.11
PASS  CellEditEnding post-commit work uses DispatcherPriority.ContextIdle
PASS  Inventory spool collection refresh is guarded during AddNew/EditItem
PASS  v38.2.10 search/filter/sort features preserved
PASS  SQLite schema unchanged
PASS  Inventory persistence methods unchanged
PASS  Documentation and test checklist updated
~~~

---

## Source: Reports/v38.2.10_STATIC_VERIFICATION.txt

~~~text
3DPIceland FilamentDB Engineering Platform
v38.2.10 INVENTORY-POLISH-WORKFLOW
Static Verification

PASS  Package structure preserved: README.md, App/, Docs/, Reports/
PASS  Release identity updated to 38.2.10
PASS  SQLite schema unchanged
PASS  Inventory spool persistence path preserved
PASS  Materials aggregate Inventory Qty sync preserved
PASS  Inventory search and filter controls wired
PASS  Alphabetical material-display sorting added
PASS  Status and remaining-level visual triggers added
PASS  Average Cost/kg and row Cost/kg calculations added
PASS  Row-level missing-field validation messages added
PASS  Add/Duplicate/Delete selection workflow improved
PASS  MainWindow.xaml is well-formed XML
NOTE  Runtime build and UI verification must be completed in Visual Studio on Windows.
~~~

---

## Source: Reports/v38.2.9_STATIC_VERIFICATION.txt

~~~text
3DPIceland FilamentDB v38.2.9 INVENTORY-VERIFICATION-FIX

STATIC AUDIT: PASS
- Version identity updated consistently.
- Multi-spool verification compares engine output to active spool records.
- Orphaned spool records remain a blocking structural error.
- User data warnings are reported as non-blocking review items.
- Inventory calculation and SQLite persistence code unchanged.
- Local .NET compile required.
~~~

---

## Source: Reports/v38.2.8_STATIC_VERIFICATION.txt

~~~text
3DPIceland FilamentDB v38.2.8 FIRST-SPOOL-DEFAULTS-FIX

Static verification:
PASS Version metadata aligned to 38.2.8
PASS BuildInfo release code aligned
PASS First spool detection is scoped by MaterialID
PASS First spool maps material spool and purchase defaults
PASS Subsequent Add Spool records remain blank
PASS Duplicate behavior remains explicit and unchanged
PASS No SQLite schema changes
PASS Standard package structure preserved

Local Visual Studio build and runtime verification required.
~~~

---

## Source: Reports/v38.2.6_INVENTORY-PERSISTENCE-FIX_Static_Audit.txt

~~~text
3DPIceland FilamentDB v38.2.6 Static Audit

PASS Build identity updated to v38.2.6 INVENTORY-PERSISTENCE-FIX
PASS Native material persistence changed from destructive DELETE/INSERT to UPSERT
PASS Existing material IDs remain in place and no longer cascade-delete spool rows
PASS Only genuinely removed material IDs are deleted
PASS Materials inventory quantity refresh targets NativeMaterialsGrid
PASS InventorySpoolItems remains the multi-spool source of truth
PASS Package structure contains README, App, Docs and Reports

Compile note: dotnet SDK is not installed in the packaging environment; local Visual Studio build is required.
~~~

---

## Source: Reports/v38.2.4_MULTI-SPOOL-INVENTORY_Static_Audit.txt

~~~text
3DPIceland FilamentDB v38.2.4 MULTI-SPOOL-INVENTORY
Static audit

PASS  Package contains README, App, Docs and Reports
PASS  SQLite schema incremented from 8 to 9
PASS  InventorySpoolItems table has InventoryItemId primary key
PASS  InventorySpoolItems references MaterialId
PASS  Multiple spool rows can reference the same material
PASS  Existing material-level inventory values have first-run migration path
PASS  Inventory editor exposes Add, Duplicate and Delete
PASS  Inventory calculations consume spool records
PASS  Estimated value remains per-spool price × quantity × remaining ratio
PASS  Material catalogue and engineering pipeline remain separate
NOTE  dotnet SDK is not installed in the packaging environment; compile and runtime validation are included in What to Test.
~~~

---

## Source: Reports/v38.2.3_INVENTORY-QUANTITY-VALUE-FIX_Static_Audit.txt

~~~text
3DPIceland FilamentDB v38.2.3 INVENTORY-QUANTITY-VALUE-FIX
Static audit

PASS InventoryEngineService treats Spool Weight as per-spool capacity.
PASS InventoryEngineService treats Remaining Weight as per-spool remaining weight.
PASS Quantity multiplies total capacity.
PASS Quantity multiplies total remaining weight.
PASS Purchase Price is treated as price per spool.
PASS Estimated Value multiplies price per spool by Quantity and remaining ratio.
PASS Unopened validation compares entered remaining against per-spool capacity.
PASS Materials grid labels clarify g / spool semantics.
PASS No SQLite schema migration required.
PASS Package structure contains README.md, App, Docs and Reports.

Runtime build and Verification Center execution must be completed in the user's Visual Studio environment.
~~~

---

## Source: Reports/v38.2.2_INVENTORY-VALUE-STATE-FIX_Static_Audit.txt

~~~text
3DPIceland FilamentDB v38.2.2 INVENTORY-VALUE-STATE-FIX
Static source audit

PASS InventoryEngineService treats Unopened as full capacity
PASS Opened uses explicit remaining-weight ratio
PASS Estimated Value uses effective remaining capacity
PASS Refresh Inventory runs after DataGrid binding commit
PASS SQLite schema unchanged
PASS Standard package folders present
~~~

---

## Source: Reports/v38.2.1_INVENTORY-LIVE-REFRESH_Static_Audit.txt

~~~text
3DPIceland FilamentDB v38.2.1 INVENTORY-LIVE-REFRESH

Static audit
------------
PASS  Assembly and documentation version updated to 38.2.1
PASS  Material Manager post-commit dispatcher refreshes Inventory summary
PASS  Material collection changes refresh Inventory summary
PASS  Manual Material save refreshes Inventory summary
PASS  Existing InventoryEngineService calculation rules unchanged
PASS  SQLite schema unchanged
PASS  Standard package structure preserved
~~~

---

## Source: Reports/v38.2.0_INVENTORY-ENGINE_Static_Audit.txt

~~~text
3DPIceland FilamentDB v38.2.0 INVENTORY-ENGINE
Static package audit

PASS  Standard package structure present: README.md, App/, Docs/, Reports/
PASS  Release identity updated to v38.2.0 INVENTORY-ENGINE
PASS  InventoryEngineService added as calculation owner
PASS  Inventory Summary UI added as an extension of the existing workspace
PASS  Existing SQLite purchasing schema reused; no destructive migration required
PASS  Inventory verification checks added to Verification Center
PASS  Inventory diagnostics section added
PASS  XAML parsed successfully
PASS  Project XML parsed successfully
PASS  Landed-cost and exchange-rate calculations remain outside this release
NOTE  Local .NET compilation remains part of the user's Visual Studio test workflow.
~~~

---

## Source: Reports/v38.1.0_PURCHASING-FOUNDATION_Static_Audit.txt

~~~text
v38.1.0 PURCHASING-FOUNDATION – Static Package Audit

PASS: Standard package structure present: README.md, App/, Docs/, Reports/.
PASS: Project Version, AssemblyVersion, FileVersion and InformationalVersion updated to 38.1.0.
PASS: BuildInfo release code/title updated.
PASS: Docs/VERSION.txt updated.
PASS: SQLite schema version increased from 7 to 8.
PASS: Additive EnsureColumn migration exists for all v38.1 native purchasing/inventory fields.
PASS: NativeMaterialRecord and NativeMaterialRow contain all v38.1 fields.
PASS: SQLite INSERT, parameter binding, SELECT and reader mapping cover all v38.1 fields.
PASS: Material Manager XAML exposes all v38.1 fields.
PASS: Inventory Status and Currency use constrained dropdown values.
PASS: JSON transition storage automatically serializes the new row fields.
PASS: Material Detail data rows and grouping include purchasing/inventory fields.
PASS: Native Excel export includes purchasing/inventory fields.
PASS: MainWindow.xaml and App.xaml are well-formed XML.
PASS: v38.1 milestone, changelog, build history, build notes and test checklist updated.

Environment note: dotnet SDK is not installed in this packaging environment, so local Visual Studio compilation remains part of the What to Test checklist.
~~~

---

## Source: Reports/v37.9.0_WORKFLOW-COMPLETE_Static_Audit.txt

~~~text
v37.9.0 WORKFLOW-COMPLETE – Static Package Audit

PASS: MainWindow.xaml parses as XML.
PASS: HighPerformanceDataGrid style exists.
PASS: Materials, Tensile, Impact and Stiffness grids use the performance style.
PASS: Row virtualization enabled.
PASS: Column virtualization enabled.
PASS: Recycling virtualization mode enabled.
PASS: BuildInfo, project metadata and Docs/VERSION.txt agree on v37.9.0.
PASS: Package root remains README.md, App/, Docs/ and Reports/.

Runtime scroll performance and build compilation must be verified in Visual Studio on the user's Windows system.
~~~

---

## Source: Reports/PKG-001_Static_Package_Audit.txt

~~~text
3DPIceland FilamentDB v37.2.5 – PKG-001 Static Package Audit

PASS: Root contains README.md plus App, Docs, and Reports folders.
PASS: App/README.md duplicate removed.
PASS: No BUILD_NOTES_v* files remain.
PASS: Current build notes stored in Docs/BUILD_NOTES.md.
PASS: Historical release information remains in CHANGELOG.md and BUILD_HISTORY.md.
PASS: Reports folder contains only current package records.
PASS: No bin, obj, .vs, publish, database, or user settings files detected.

Docs files: 223 -> 65
Reports files: 60 -> 2
App source/assets files retained: 68
~~~

# v44.1.2 Verification Profiles and Diagnostic Honesty — 2026-07-23

Status: PASS; RUNTIME ACCEPTED

- RUNTIME FINDING / v44.1.1 BUILD FIX: fresh-VM Application Readiness selected
  correctly and reported 207 PASS, 88 NOT APPLICABLE and two FAIL. The remaining
  `Website portal release contract` and `Website export package contract` are
  known downstream zero-data/template dependencies whose generic detail text
  lacked the classifier marker. v44.1.1 explicitly classifies only those two
  named contracts as not applicable on zero-Materials profiles; both remain
  mandatory in Full Data Verification.
- PASS: fresh-VM v44.1.1 Application Readiness runtime acceptance. The profile
  reported 207/207 applicable PASS, 0 FAIL and 90 NOT APPLICABLE across 297
  checks. Release identity aligned at v44.1.1/44.1.1.0/
  VERIFICATION-PROFILES; schema-v29 zero Materials/native measurements,
  intentionally empty deployment identity, trusted signed packaging,
  transactional/default-No update and recovery, installer/deployment and remote
  feed gates all passed.
- PASS: restored schema-v29 owner data selected Full Data Verification with 200
  active Materials and zero not-applicable checks. The first run passed 293/297;
  only the four local recovery-evidence gates failed because no canonical local
  backup of the restored state existed. Creating a manual SQLite backup produced
  297/297 PASS, 0 FAIL and 0 NOT APPLICABLE.
- v44.1.2 BUILD FIX: successful explicit SQLite restore now atomically creates
  and verifies a retained post-restore evidence backup whose schema, Materials,
  tensile, impact and stiffness counts reproduce the restored database. The
  pre-restore recovery backup remains retained, rollback remains fail-closed and
  no automatic restore was introduced.

- PASS: verification checks now retain their existing truth result while an
  additive profile layer reports PASS, FAIL or NOT APPLICABLE.
- PASS: zero active canonical Materials selects Application Readiness; profiles
  containing Materials select Full Data Verification.
- PASS: only known zero-data dependencies with explicit reasons can become
  `NOT APPLICABLE — No canonical data`. Unexpected release identity,
  installer/deployment, schema/assets/privacy/update/recovery failures remain
  applicable FAIL results.
- PASS: exports include profile name, selection reason, applicable/pass/fail/
  not-applicable counts and exact per-check reasons.
- PASS: Full Data Verification keeps every check applicable. A new v44.1
  contract gate preserves zero compiled seed Materials, empty default deployment
  identity and the prior no-automatic-SQLite-restore boundary.
- PASS: Debug and Release builds completed with 0 warnings and 0 errors.
- PASS: trusted ECDSA Candidate package contains exactly six governed files and
  supports SQLite schema v29. NuGet vulnerability, BOM-less feed, exact bytes/
  SHA-256, signature, inventory and stable-route-last gates passed.
- Candidate installer: 68,151,058 bytes; SHA-256
  `E27D7F268211DDEF0923715A9776120D808AF88B7C7E7409E593CD35046BF818`.
- Candidate portable ZIP: 95,528,026 bytes; SHA-256
  `3795690466CDF02487B0977EDFF3EEE06B908CB7E4CF93401514D25B171BE024`.
- Candidate signed update ZIP: 95,899,889 bytes; SHA-256
  `456E1CEAD96CA8F0A7A92BACCF2D2D0C43C6C9D48C428163C51AA7A724F6F777`.
- PASS: transactional updater self-test preserved commit/rollback/interrupted
  recovery, traversal rejection and SQLite-backup-reference boundaries.
- PASS: final clean-VM Application Readiness passed 207/207 applicable checks
  with 90 explicit N/A results. Final restored-data Full Data Verification
  passed 297/297 immediately after explicit restore and automatic restart,
  without requiring a manual backup.
- PASS: Recovery Center showed the retained post-restore evidence backup as
  Ready at schema v29 with 200 Materials, 3,728 tensile rows, 3,752 impact rows,
  191 stiffness rows and 50 settings. Pre-restore and automatic/migration
  backups remained retained and were not silently deleted or restored.

# v44.0 Baseline and Release-Workflow Closure Candidate — 2026-07-23

- PASS: baseline began on clean `master` at `105d454`, aligned 0/0 with
  `origin/master`; canonical runtime identity remains v43.8.9.
- DECISION: the only supported first-install route remains the runtime-accepted
  v43.8.8 installer/portable package followed by the manual default-No guarded
  update to canonical v43.8.9. No new installer was built or promoted.
- PASS: Candidate feed regenerated from the byte-identical canonical v43.8.9
  signed ZIP; NuGet transitive vulnerability result, BOM-less JSON, 95,897,210
  package bytes, SHA-256, trusted ECDSA signature, exact six-file governed
  inventory, SQLite schema v29 and stable-route-last contracts passed.
- PASS: dirty-tree Production and existing-feed overwrite probes failed closed.
- PASS: Inno Setup 7.0.2 built the direct v43.8.9 Candidate installer
  (68,149,629 bytes; SHA-256
  `99623DDD0C34F15E50FA5583FE56CD88B5AF283085627DD4AB071F2F3529E2FA`)
  and portable ZIP (95,525,502 bytes; SHA-256
  `DDE4C025C18BAA859A61666B1F066136B48FEF1E0EF6FB852F769528DC61C4F8`)
  from the canonical signed package. The portable inventory is exactly the six
  governed files; forbidden data/backup/native payload filenames were absent.
- PASS: fresh-VM direct-installer runtime acceptance. The application ran from
  `C:\Users\Maddi\AppData\Local\Programs\3DPIceland Engineering Platform`,
  reported aligned v43.8.9/43.8.9.0/SQLITE-DEPENDENCY-SECURITY identity, created
  a schema-v29 clean profile with zero Materials/native measurements, retained
  empty deployment identity and passed the signed-package, transactional
  updater, guarded-update, diagnostics, installer/deployment and remote-feed
  release gates. The VM clock was incorrect, so the exported filename/generated
  timestamp reads 2026-07-22 even though this evidence belongs to the 2026-07-23
  Candidate test.
- EXPECTED CLEAN-PROFILE LIMITATION: Verification reported 223 PASS and 91 FAIL.
  The failures are data-dependent full-verification checks cascading from zero
  Materials, no active website template and no canonical recovery backup; no
  installer/runtime identity, clean-profile isolation, updater or deployment
  contract failed. This is the accepted evidence feeding v44.1 profile work,
  not a 296/296 full-data release acceptance.
- PASS: fresh-VM direct-portable runtime acceptance. The application ran from
  `C:\Users\Maddi\Documents\3DPIceland-Portable-x64-v43.8.9`, reported the same
  aligned v43.8.9 identity and schema-v29 privacy-clean zero-data profile, and
  passed the signed-package, transactional updater, guarded-update, diagnostics,
  installer/portable deployment and remote-feed gates. First usable Materials
  rendered at approximately 3.64 seconds. Its 206 PASS/90 FAIL result contains
  the same data-dependent failure labels as the installer clean profile; no
  portable-specific failure appeared.
- PASS: Debug and Release solution builds completed with 0 warnings and 0
  errors. Transactional updater self-test passed commit, rollback, interrupted
  recovery, traversal rejection and SQLite-backup-reference preservation.
- RUNTIME DECISION: direct v43.8.9 installer and portable first-install
  Candidates are accepted. Production promotion must preserve these exact tested
  artifact bytes, run only from a clean tree, remain fail-if-exists and keep
  stable routes untouched until the separately verified publish step.
- PASS: clean-tree Production promotion copied the runtime-tested installer,
  portable ZIP and signed update ZIP without changing their bytes. Production
  metadata is BOM-less, records promotion commit `dd4eaf4`, and the Production
  gates passed NuGet vulnerabilities, feed/package bytes and SHA-256, trusted
  ECDSA signature, six-file governed inventory, schema v29 and stable-route-last
  contracts.
- COMPLETE: v44.0 local release-workflow closure. Stable FTPS routes were not
  changed; their publication remains a separate explicit default-No operational
  action. No application UI, updater runtime, SQLite, website/report or FTPS
  behavior changed in source.

# v43.8.9 SQLite Dependency Security — 2026-07-22

- PASS: BOM-less `latest.json` updated a fresh v43.8.8 VM install to v43.8.9; durable transaction phase `Committed`, zero incomplete transactions and no automatic SQLite restore.
- PASS: Fresh-install zero-Materials boundary remained intact after update.
- PASS: Verified schema-v29 restore recovered 200 Materials, 3,728 tensile samples, 3,752 impact samples and 191 stiffness rows; post-restore canonical backup passed integrity and recovery compatibility.
- PASS: Final VM Verification Center 296/296 with zero failures; v43.8.9 promoted to canonical.
- PASS: Release runtime identity `v43.8.9 SQLITE-DEPENDENCY-SECURITY`, assembly 43.8.9.0 and informational version aligned.
- PASS: Verification Center 296/296; diagnostics export contained 296 PASS lines and zero FAIL lines.
- PASS: SQLite schema v29 and the existing 200-material owner database loaded and verified normally.
- PASS: NuGet vulnerability scan resolved `Microsoft.Data.Sqlite` 9.0.18 and SQLitePCLRaw 2.1.12 with no known vulnerable package from configured sources.
- PASS: Debug/Release 0 warnings and 0 errors; updater self-test and six-file signed-package verifier passed.
## v45.1 Canonical Manufacturer Selection candidate

- PASS: isolated Debug compile probe and final Debug/Release solution builds
  completed with zero warnings/errors.
- PASS: schema v32 stores nullable `ManufacturerId`; migration leaves historical
  rows unlinked and preserves exact legacy/unmapped Manufacturer text.
- PASS: explicit selection persists ID, canonical rename propagates to linked
  Material snapshots and referenced hard delete is blocked.
- PASS: website/report/public allowlists remain unchanged; typed Excel recovery
  carries the nullable relationship column.
- PASS: release-documentation audit, 136-character roadmap-line gate, diff
  checks and read-only NuGet vulnerability scan.
- PASS: disposable CRUD against `C:\Seed-Database\filamentdb.sqlite` reported
  Full Data Verification 345/345 with zero failures.

- PASS: generated `AUT5abe4fcf` preserved unmapped Manufacturer create/restart,
  canonical Manufacturer edit/restart and final delete/restart absence.
- PASS: baseline/final business-state hashes match. The source seed remained
  4,825,088 bytes with unchanged timestamp and SHA-256
  `7851D26BA82E345E2C4B156996B68F0360B7F74A48B777752A609CB368EFD6D4`.
- Owner follow-up evidence `3DPIceland_FilamentDB_Verification_20260725_214218.txt`
  reported 344/345: Inventory engine alone failed with one stale in-memory spool
  row after its MaterialID was deleted in the same session. The accompanying
  screenshot also showed the deleted Material's unique Manufacturer until a
  later grid rebuild.
- The candidate now refreshes Manufacturer choices on Material collection
  changes and reloads Inventory from canonical SQLite after successful Material
  deletion. Disposable CRUD run `20260725214900-5abe4fcf` re-passed Full Data
  Verification 345/345; Inventory engine reported zero orphan rows and the seed
  hash remained unchanged. Owner runtime retest remains required.
- Owner review then found initial Name typing blocked for a new catalog row when
  its placeholder matched a Material value. Draft rows now permit their initial
  character-by-character rename and mirror Display Name; established in-use
  records remain guarded. Debug/Release and disposable CRUD run
  `20260725220220-dc872e8c` pass 345/345 with unchanged seed and business state.
  Direct Name-cell typing remains owner-manual.
- The approved ManufacturerId pivot passed disposable CRUD run
  `20260725222727-42605144`: schema 31 migrated to 32 with zero automatically
  linked historical rows, explicit ID survived restart, canonical rename
  propagated, referenced delete was blocked and cleanup returned the exact
  business-state hash to
  `9BA19BE80FF49D7BBA1EDEF27B115177D3315AE8422FA0CFBFF17FE826BFD516`.
- Final delivery build run `20260725223305-cc3cce96` repeated the same 345/345
  PASS and equal business-state hash after the default-No exact-binding UI and
  Verification contract were added.
- Owner exact binding linked 182 Materials and left 20 unmatched by design.
  Owner evidence `3DPIceland_FilamentDB_Verification_20260725_224310.txt`
  reported 341/345: four chained v44.7.7 gates failed because one archived
  MaterialID remained in the raw All-filter ID set while canonical report rows
  correctly contained only 201 active Materials. The gate now compares active
  scope on both sides; owner retest remains pending.
- Disposable owner-fix run `20260725224654-16f3c455` passed 345/345; all four
  v44.7.7 gates passed and the final business-state hash matched baseline.
- PENDING: normal owner runtime/visual review.

# v47.0.1 - AI Assistant Scope Clarity Candidate

- Date: 2026-07-26
- Disposable profile: `20260726122624-99485959`
- Scenario: smoke
- Result: PASS
- Full Data Verification: 348/348
- AI Assistant visible scope: 201 active rows and 201 unique MaterialIDs
- Production, FTPS, updates and owner-database selection: blocked
- Logical database hash before/after:
  `F0EDCC3295A114C935668D2B92D7A1AEB1C67C4D1630EFC89F11B7FCDC556E5F`
- Normalized business-state hash before/after:
  `4FBCF6A2656678875A6692C0A7AA30CD0CDC3F4AAB83003B3BB2C77081B1C87D`
- Owner runtime and visual acceptance: PASS
- Owner Full Data Verification: PASS 348/348
- Canonical state: v47.0.1

# v47.0.2 - AI Collection Workflow Clarity Candidate

- Date: 2026-07-26
- Initial disposable profile: `20260726133919-b630ebf4`
- Initial result: FAIL — Verification WPF-name lookup mismatch; canonical and
  owner data were not used.
- Corrected disposable profile: `20260726134033-9c21103d`
- Corrected result: PASS, but legacy AI AppData resolution exposed an owner
  collection title in evidence without writing data.
- Owner runtime finding: cancelled update output looked updated because it
  displayed the discarded current-filter preview; persistence was not executed.
- Final hardened disposable profile: `20260726135702-ceb78987`
- Scenario: smoke
- Result: PASS
- Full Data Verification: 349/349
- Read-only collection preview and explicit Create/Update state: PASS
- Cancel-state persisted/proposed membership honesty: PASS
- Disposable AI preferences isolation and personal AppData exclusion: PASS
- Production, FTPS, updates and owner-database selection: blocked
- Logical database hash before/after:
  `F0EDCC3295A114C935668D2B92D7A1AEB1C67C4D1630EFC89F11B7FCDC556E5F`
- Normalized business-state hash before/after:
  `4FBCF6A2656678875A6692C0A7AA30CD0CDC3F4AAB83003B3BB2C77081B1C87D`
- Owner runtime acceptance: PASS
- Owner Full Data Verification: PASS 349/349
- Canonical state: v47.0.2
