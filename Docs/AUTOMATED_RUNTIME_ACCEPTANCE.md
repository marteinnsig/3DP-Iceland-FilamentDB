# Automated Runtime Acceptance

## v61.0.5 - App-wide Thermal Decision Surfaces Accepted

- Existing smoke remains the safe read-only owner for Material Detail, Rankings,
  Category Rankings, Awards, Insights and YouTube Research navigation.
- No new mutating scenario or authorization is warranted.
- Full Verification owns fixed 200 °C projection, clamping, missing-value,
  minimum-peer and legacy Overall-invariance contracts plus required UI surfaces.
- Release profile `20260814193047-3e01de23` passes 429/429, 23/23 tab/menu
  navigation and exact logical/business-state recovery.
- Owner review finds two visibility gaps after the first PASS: Compare lacked raw
  °C and YouTube Research exposed thermal only conditionally. Verification now
  requires the corrected Measured Properties row and explicit candidate column.
- Replacement profile `20260814195816-dc8f3960` passes the hardened contract,
  Full Verification 429/429 and exact logical/business-state recovery.
- Owner accepts corrected Compare, YouTube evidence and all named thermal
  decision surfaces on 2026-08-14.

## v61.0.3 - Thermal Population and Transition Retirement Accepted

- Pre-retirement profile `20260814181952-48fb589d` passes 428/428 and exact recovery.
- After owner acceptance, smoke no longer discovers retired workbook controls.
- Full Verification owns immutable-method/manual persistence, accepted 191-row
  provenance and exact absence of the transition UI/service.
- Post-retirement profile `20260814184446-a76801d2` passes 428/428 with 23/23 tabs,
  23/23 Navigate actions and exact logical/business-state recovery.
- Seed hash is `28741D5F...8E25`; Production, FTPS, updates and owner database
  remain blocked. Owner accepts the final UI and Verification PASS on 2026-08-14.

## v60.0.6 - Locale-aware Application Date Presentation

- The existing explicit `reports` scenario remains the bounded owner because it
  visits the affected workspaces, exports Full Verification, builds reports and
  proves exact logical/business-state recovery.
- No new mutating scenario or AutomationId is warranted. Deterministic
  Verification directly exercises `is-IS`, `en-US`, ISO persistence and
  propagation, leap-day, invalid/implausible input and legacy Icelandic reading.
- Disposable profile `20260814153636-06cea92b` passes 425/425 and hashes 627
  report artifacts with exact state recovery. Production and FTPS stay blocked.

## v60.0.3 - Material Identity Refresh Across Measurement Views

- Existing read-only smoke remains the bounded scenario owner; owner Materials
  and measurement evidence are not mutated.
- Full Verification creates isolated Material/Tensile/Impact/Stiffness models
  and proves Black/Yellow/Black Color-only propagation while Marketing Name is
  Black, then a Marketing Name-only update.
- The same contract proves tensile/impact samples, stiffness inputs and all
  three notes remain unchanged during identity copying.
- The contract also proves duplicate Website Display Name values are advisory:
  stable MaterialID remains the relational key and silent save is not blocked.
- Owner retest exposed the close/debounce persistence boundary; the candidate
  now synchronizes child identities after the parent save and before the
  existing guarded measurement transaction.
- Release smoke `20260730151123-a5098658` passes 423/423 with exact logical and
  business-state recovery.
- Corrected Release smoke `20260730162359-1eaaead1` repeats 423/423 with exact
  logical and business-state recovery after the save/close fix.
- Owner accepts Black/Yellow/Black propagation, clean close, restart
  persistence and Full Data Verification PASS.

## v60.0.2 - Fast Materials Editor Focus and Keyboard Navigation

- Existing read-only smoke remains the bounded scenario owner; it does not
  mutate owner Materials.
- Full Verification proves stale editor events cannot close the current editor
  and background canonical refresh defers only while an editor is active.
- Arrow/Tab/mouse usability remains owner-manual because reliable UI Automation
  typing would add low-value timing sensitivity.
- Release smoke `20260730145422-31f69821` passes 422/422 with exact logical and
  business-state recovery.
- Owner accepts four-direction arrows, Tab/Shift+Tab, mouse transfer, rapid
  edits, restart persistence and Full Data Verification PASS.

## v60.0.1 - Purchase-order Material Creation Integrity

- Existing read-only smoke remains the bounded scenario owner; no Purchase
  Order UI mutation was added to the general smoke.
- Full Verification adds a disposable SQLite contract for the shared selected
  and bulk parent/link transaction.
- The contract proves single- and multi-line success, forced foreign-key
  failure, complete rollback, immutable landed-cost retention and
  duplicate-free retry.
- Final corrected Release smoke `20260730144248-13602dab` passes 421/421 with exact
  logical and business-state recovery.
- Owner runtime passes selected, bulk and duplicate-free retry behavior. The
  first owner Verification passed v60.0.1 but exposed a false v53.0.1 failure
  for valid versioned cross-currency Inventory rates. Deterministic v53 probes
  now accept positive versioned provenance, reject zero and retain exact
  `legacy-v1` 1:1 ownership.
- Owner rerun confirms corrected Full Data Verification PASS; v60.0.1 runtime
  acceptance is complete.
- Failed diagnostic profile `20260730142418-86598ce2` is retained pending
  evidence review; it exposed and bounded the corrected transaction-placement
  regression before acceptance.

## v59.0.11 - Public Base-material guidance accepted

- The existing `reports` scenario remains the bounded owner; no new mutation,
  Production, FTPS, update or owner-database authorization was added.
- It now requires the base-material label/boundary, explicit units, metadata
  guidance allowlist, at least one linked recorded value and per-field
  `Not recorded` evidence.
- Full Verification proves linked stale-name, null-link, orphan-link, partial
  value, escaping, manifest and metadata contracts deterministically.
- Source fingerprinting includes `NativeMaterialManagerRows.BaseMaterialId` and
  every consumed `BaseMaterialCatalog` guidance field for stale rebuilds.
- Final profile `20260729173741-50c1c3bb` passes 420/420, validates/hashes 627
  artifacts and retains exact database/business-state equality.
- Owner full-data HTML/PDF review and Full Verification PASS are accepted.
  Partial/unlinked coverage remains deterministic without owner-data mutation.

## v58.0.5 - Document branding migration and recovery accepted

- Existing smoke remains the bounded runtime owner; no new file-dialog or
  branding mutation authorization is added.
- Full Verification synthetically covers valid/invalid/oversize PNG input,
  source/cache integrity, brand validation, restart, SQLite backup parity,
  corrupt fallback and combined Restore Default.
- Schema-v39 migration smoke `20260728232355-2dd2b477` passes 413/413 with
  exact logical database and business-state equality.
- The old seed remains
  `C:\Seed-Database\filamentdb-schema39-migration.sqlite`, SHA-256
  `C56963156E2CC660E088AF1F07D54B9ACA5C439A0532B7BBA0FFFBC45B25E5E6`.
- The schema-v40 canonical seed SHA-256 is
  `A0C1E72984ED747C8205D8C847C1620C5C22AC6C587C86FDC7A1B6998A600870`;
  current-schema smoke `20260728232520-3f97f349` repeats 413/413 exact state.
- Owner database, Production, FTPS, updater and live network remain blocked.

## v56.0.6 - Owner acceptance and guarded artifact closure

- Owner accepts the full demo plus all 36 corrected Impact rows.
- Final profile `20260728180126-bb11afd8` passes 391/391 applicable checks,
  12 canonical-data N/A, zero mandatory N/A and exact state recovery.
- The four-file local package is ZIP-built twice with fixed timestamps and
  byte-identical SHA-256.
- Expanded allowlist, checksums, manifest, SQLite integrity, foreign keys,
  exact counts and zero invalid Impact samples pass.
- Production, FTPS and stable-route activation remain unauthorized.

## v56.0.5.2 - Demo Impact percentage correction

- Builder normalization is limited to the exact 567 invalid legacy encodings:
  557 integer values ending in zero and 10 ending in five.
- All 151 already-valid samples remain unchanged; all 718 output samples must
  be numeric and within 0-100.
- Scenario `demo` requires 36 `InImpact=Yes` and Fully tested rows in addition
  to the accepted identity and measurement counts.
- Profile `20260728180126-bb11afd8` passes 391/391 applicable checks, 12
  canonical-data N/A, zero mandatory N/A and exact state recovery.
- Seven rendered report pages and extracted text pass renewed privacy/layout
  review; Production, FTPS, updates and owner data remain blocked.

## v56.0.5.1 - Demo report privacy and pagination correction

- Material Summary customer-facing HTML/PDF omits the MaterialID header and
  every fictional `DEMO-MAT-` value.
- Governed report metadata and manifest retain exact internal scope
  traceability.
- Compact customer identity, four-row CSS-grid ledger groups and repeating
  `@page` margins prevent row fragmentation and continuation-page clipping.
- Verification and scenario `demo` own deterministic HTML checks; corrected
  PDF text and rendered pages require independent review.
- Final profile `20260728173706-49ad4986` passes 391/391 applicable checks,
  12 canonical-data N/A, zero mandatory N/A and exact state recovery.
- Seven rendered pages and extracted PDF text pass; PDF SHA-256 is
  `DDD48F096E2624133F07F059F2901A95EB29E9C6EC21370B56FAE7417A884F23`.

## v56.0.5 - Public demo runtime accepted

- Scenario `demo` uses only a disposable copy of the governed public artifact.
- Search, restart, exact AI/collection MaterialID scope and global Clear pass.
- Public Demo Verification passes 391/391 applicable checks; 12 owner
  website-publication checks are explicitly N/A and no mandatory check is N/A.
- Material Summary / All Visible exports five local artifacts with fictional
  identity, 36/36 scope and no customer-facing MaterialID.
- Source seed remains byte-identical and disposable business state returns
  exactly to SHA-256
  `78D52C17B6ADA7BC264A4E2F079F5AAE67928D25D3BDAD3F59D9E4901CA49E18`.

## v56.0.4.1 - Coverage parity correction accepted

- Corrected A/B artifacts are byte-identical and retain all privacy/integrity
  gates.
- Valid-result coverage produces 9 Fully tested and 27 Partially tested rows;
  REPORT-110 parity and blank optional-pricing validation pass.

## v56.0.4 - Deterministic builder accepted

- No AutomationRunner scenario is added because the builder is an operator-only
  CLI requiring the private allowlist and approved source copy; it has no app
  profile, UI, Production, FTPS or canonical-seed authority.
- Release generation creates two fresh databases and requires exact file,
  logical and per-table equality before emitting a manifest.
- Integrity, foreign keys, 25/17/6 schema objects, exact counts, identity
  parity, 17 empty domains, all-TEXT privacy scan, empty `sqlite_sequence` and
  unchanged source bytes fail closed.
- Output-alias and transformation-location probes fail before creating output.
- v56.0.5 owns disposable app runtime, Help, AutomationRunner and Full Data
  Verification.
- Deterministic generation and privacy acceptance are complete; v56.0.5 is
  current.

## v56.0.3 - Fictional transformation accepted

- No AutomationRunner scenario is added because this increment generates no
  SQLite and exposes no application runtime surface.
- `validate-transform` checks pinned private/public hashes, owner approval,
  source/demo bijection, exact identity partitions and derived key uniqueness.
- Repeated validation must be byte-identical. Changed public spec or private
  partition parity fails closed with codes only.
- Two final Release validations pass byte-identically with logical manifest
  SHA-256
  `82C62984153513CD78797FDBE71839BE4B4F4E805F009234105CF5A3EE30F621`.
- v56.0.5 remains the owner of disposable app runtime, restart, filters,
  rankings, collections, reports, Help and Full Data Verification.
- Owner accepted the fictional identity and transformation contract; v56.0.3
  is complete and v56.0.4 is current.

## v56.0.2 - Read-only inspector accepted

- No AutomationRunner scenario is added. The inspector accepts private
  operator-supplied input and must not inherit canonical-seed/profile authority.
- `DemoDatasetTool self-test` covers deterministic serialization and strict
  private-contract JSON parsing without opening a database.
- Disposable positive and schema/source-drift probes verify immutable,
  read-only, query-only operation, source-byte preservation, no SQLite sidecars
  and fail-closed hash binding.
- The dry-run manifest exposes only classifications, counts, hashes and ordered
  failure codes. It contains no paths, source IDs or private names.
- v56.0.5 remains the owner of bounded disposable demo runtime, reports,
  restart, Help and Full Data Verification.
- Owner accepts deterministic dry-run, privacy-safe output, exact closure and
  fail-closed source, registry and schema drift evidence.

## v55.0.6 - Operational safety accepted

- Normal Base Material deletion remains automation-blocked; deterministic
  Verification owns prompt/default/cancellation semantics and a disposable
  SQLite delete-by-ID probe.
- Existing CRUD authorization retains generated-record persistence and exact
  business-state cleanup without broadening to arbitrary catalog IDs.
- `--cleanup-profiles` is dry-run only and emits JSON/TXT plan evidence.
- `--apply-cleanup-plan` requires the exact reviewed `--plan-sha256`.
- `--cleanup-self-test` creates only synthetic profiles and proves classifier,
  dry-run byte preservation, retention, plan hash and bounded apply.
- Real cleanup apply remains acceptance-gated. Owner database, canonical seed,
  Production and FTPS stay blocked.
- Release smoke `20260728132054-713375ea` passes 402/402 with exact state.
- CRUD `20260728132200-3f006c6b` passes bounded delete, restart and exact
  business-state recovery.
- Reviewed real dry-run plan SHA-256
  `0EC0B8565A655B116F8B50F295975B6E13B64E0035B93150DF39A1FE4F04748B`
  retains four required profiles and removes zero.
- Owner runtime accepts both shared delete dialogs and Full Data Verification
  PASS. v55.0.6 is canonical and parent v55 is complete.
- Final profile `20260728135719-4ac326ab` passes 403/403 with exact
  database/business-state equality.

## v54.0.6 - Materials scope accepted

- Owner runtime acceptance passes for the complete v54 workflow.
- The accepted six-facet controls replace and retire hidden scalar filters.
- Final profile `20260728124401-5f1d1f99` passes 400/400 with exact database
  and normalized business-state equality. Schema remains v38.

## v54.0.5 - Materials multi-select candidate

- Smoke discovers six stable no-modifier facet surfaces, visible selection
  summaries, per-filter Clear, global Clear and Materials Search.
- Full Data Verification owns deterministic OR-within/AND-between, archived,
  unlinked, Clear, persistence, exact scope-hash and publication-isolation
  contracts.
- AI smoke requires honest local processed/omitted counts, OpenAI
  source/allowlisted/omitted counts and exact collection snapshot SHA-256.
- The scenario remains disposable and read-only; collection persistence,
  Production, FTPS, owner data and live OpenAI remain blocked.
- Release profile `20260728122340-f5ff3f77` passes 399/399, exact database and
  normalized business-state equality and isolated AI storage.
- Superseded by accepted v54.0.6 closure evidence above.

## v53.0.5 - Post-v50 Help reconciliation accepted

- No new mutating tester scenario is warranted because runtime business
  behavior, schema and visible controls are unchanged.
- The read-only Help audit requires exact v51 profile, v52 OpenAI and v53
  landed-cost markers plus explicit coverage ownership.
- Disposable smoke `20260727234421-49e325d1` passes 393/393.
- It visits all 22 top-level and 16 nested tabs, validates contextual Help and
  restores database and normalized business-state hashes exactly.
- Production, FTPS, updates and owner-database selection remain blocked.
- Owner accepts Help search/navigation/readability and Full Data Verification
  PASS; v53.0.5 requires no additional AutomationRunner scenario.

## v53.0.4.4 - Diagnostics and Help accepted

- Existing smoke opens System Diagnostics read-only.
- It now requires aggregate Purchase Order and Inventory landed-cost snapshot
  counts plus the explicit migration/recovery non-execution boundary.
- Full Verification requires matching Purchase Orders and Diagnostics Help
  markers and aligned v53.0.4.4 release identity.
- No new scenario or authorization is added; Production, FTPS, owner database,
  updater mutation and live ECB remain blocked.
- Final smoke `20260727232314-5c3961bc` passes 392/392 with exact state.
- Landed-cost `20260727230608-4b755706`, migration
  `20260727230757-30f56347`, recovery `20260727230846-e78c5a9c`, Clean
  `20260727231038-b0a96336`, Reports `20260727231114-c2f912da`, corrected CRUD
  `20260727231704-d31b92bc` and corrected updater
  `20260727232117-b8100815` pass.
- Failed smoke, CRUD and updater profiles remain retained through owner review.
- Owner runtime accepts aggregate Diagnostics, current Help, unchanged
  canonical data and Full Data Verification 392/392.

## v53.0.4.3 - Migration and recovery accepted

- `--scenario migration` opens only a disposable copy of the preserved
  schema-v37 fixture and requires startup migration to schema v38.
- Every shared legacy column in Purchase Orders, lines, Inventory, Materials,
  Usage and Quotes must match the source fixture exactly.
- The source fixture SHA-256 must remain unchanged.
- `--scenario recovery` also authorizes the exact landed-cost triplet and
  creates a calculated PO/Material/Inventory snapshot before Excel export.
- Mutation is confined to the exact authorized PO/Inventory provenance and the
  existing disposable Material recovery probe.
- Excel restore requires whole-table equality for Purchase Orders, lines,
  Inventory, Materials, Usage and Quotes.
- Cleanup requires exact record absence and baseline normalized state.
- Migration profile `20260727224150-47f14b19` passes 391/391 and migration,
  shared-column, backfill, source-hash and final-state gates.
- Recovery profile `20260727224236-eaa2bdb1` passes 391/391 and exact v53,
  historical-table, backup-evidence, cleanup and final-state gates.
- Final read-only smoke `20260727224717-d81709b9` passes 391/391 with exact
  normalized business-state equality and no mutating authorization.
- Owner database, Production, FTPS, updater and live ECB remain blocked.
- Owner runtime accepts unchanged canonical Purchase Orders/Inventory, 200
  intentional Materials, zero automation residue and Verification 391/391.

## v53.0.4.2 - Disposable landed-cost lifecycle candidate

- Scenario: `--scenario landed-cost`.
- Authorization is default-off elsewhere and binds three exact safe IDs.
- Checkpoints: `DEFAULTED`, `OVERRIDDEN`, `CALCULATED`, `DOWNSTREAM`,
  `CLEANED` and `ABSENT`, with restart boundaries between material stages.
- Cancellation must preserve the default snapshot before the confirmed
  override is applied.
- Calculation must persist version/UTC and landed line/unit/kg outputs.
- Material and Inventory must copy the saved order currency provenance.
- Cleanup must remove only the exact disposable PO/line/Material/Inventory and
  return the normalized business-state hash to baseline.
- Smoke, Production, FTPS, updater, owner database and live ECB remain blocked.
- Failed candidates exposed and rejected global Inventory normalization plus
  historical Material-derived-field refresh.
- Corrected profile `20260727222431-7383588f` passes six lifecycle checkpoints,
  Verification 390/390 and exact normalized business-state equality.
- Default-off regression smoke `20260727222716-fdeff6d3` passes 390/390 with
  exact normalized state and invokes no landed-cost mutation.
- Owner runtime acceptance passes with 200 intentional `MAT` identities, zero
  automation residue and Full Data Verification PASS.

## v53.0.4.1 - Landed-cost acceptance contract accepted

- `LandedCostWorkflowAuthorized` is default-off and requires exact safe,
  distinct PurchaseOrderID, MaterialID and InventoryItemID values.
- Clean Readiness cannot authorize the capability; current scenarios emit
  `landedCostWorkflowAuthorized: false`.
- The retained evidence shape contains only bounded IDs, currencies, rates,
  provenance, calculation version, restart checkpoints and state hashes.
- Read-only smoke discovers every required real Purchase Order lifecycle
  control, lines grid and validation status without invoking them.
- Full Verification owns safe/unsafe/exact-triplet matching and visible-control
  presence. The mutating lifecycle remains v53.0.4.2.
- Disposable Release smoke `20260727215805-25c18520` passes 389/389, including
  the v53.0.4.1 gate, with exact database and business-state equality.
- Debug/Release, documentation/static and NuGet vulnerability gates pass.
  Owner runtime accepts the unchanged Purchase Orders surface and diagnostics
  evidence on 2026-07-27.

## v53.0.3 - Cross-currency calculation accepted

- Full Verification covers invoice allocation, final-output conversion,
  deterministic rounding, non-destructive failure and one-time stamping.
- Material and Inventory probes require separate invoice/landed currencies and
  exact downstream provenance.
- Report verification derives ISK per landed currency from saved order rates.
- Existing smoke remains read-only; broader mutation/restart automation remains
  authoritative in v53.0.4.
- Production, FTPS, owner database and live-network actions remain blocked.
- Release smoke `20260727214133-34260e52` passes 388/388 with exact database and
  business-state equality after the calculated-input, debugger-safe
  Verification and daily ECB/EUR reference fixes.
- Owner runtime accepts calculation, locking, ECB provenance and standard
  DataGrid currency commit behavior; Full Data Verification passes 388/388.

## v53.0.2 - Landed-cost Currency Draft workflow accepted

- Full Verification checks the governed Settings default, cross-rate direction,
  persisted Draft eligibility, legacy lock and required UI controls.
- Smoke remains read-only and checks all new AutomationIds plus honest
  select/override/locked status.
- Mutation is retained for owner runtime acceptance because New Order saves
  immediately; Production, FTPS, owner data and live ECB access remain blocked.
- No monetary calculation or historical rewrite is authorized in this increment.
- Verification also proves the Settings editor uses the complete governed
  currency set and startup distinguishes real legacy rows from v53 Drafts.
- Replacement Release smoke `20260727200102-a16f943f` passes 387/387 with exact
  database and business-state equality.
- Owner runtime re-acceptance confirms the governed dropdown, Draft override,
  restart persistence and Full Data Verification 387/387.

## v53.0.1 - Governed Landed-cost Currency schema accepted

- Full Verification owns the new deterministic contract because this increment
  adds no user-facing workflow or safe UI action for AutomationRunner.
- An isolated schema-v37 fixture migrates to v38 and proves exact preservation
  of Purchase Order and Inventory monetary strings.
- Purchase Order and Inventory Excel recovery tables include all landed-cost
  currency, rate, provenance and calculation metadata columns.
- Legacy records receive only explicit same-currency, 1:1 provenance metadata.
- Release smoke `20260727190229-ad32a552` passes 386/386 with exact database and
  business-state hash equality.
- Production, FTPS, owner database and live network actions remain blocked.
- Owner runtime acceptance passes; the deterministic fixture owns populated
  monetary coverage because the two owner test orders contained no items.
- Canonical seed is schema v38; schema v37 is retained as the migration fixture.
- Canonical-seed Release smoke `20260727192919-e558dc25` repeats 386/386 with
  exact database and business-state equality.

## v52.3.3 - Bounded Owner Live Evaluation and Closure accepted

- Automation remains no-network and never invokes Generate.
- Subjective rubric scoring and project Usage reconciliation are manual owner
  evidence and are not duplicated by low-value deterministic automation.
- Existing payload, validation, safe-failure, credential, Production, FTPS and
  exact-state contracts remain unchanged.
- No AutomationRunner or Verification contract change is warranted. One
  compiler-only nullability annotation preserves the existing required guard.
- The owner accepts the Provisional, not-pinned model disposition.
- Disposable Release smoke `20260727182043-ab84a0eb` passes 385/385 with exact
  logical and business-state equality and no live network request.

## v52.3.2 - OpenAI Operational Evidence and Failure Harness accepted

- Automation invokes Preview OpenAI Payload but never Generate or Copy
  Operational Evidence.
- Save Session is invoked only against the preview and must show the bounded
  rejection; no AI session file may be created.
- Full Verification uses only scripted `HttpMessageHandler` responses and no
  real credential, network socket or provider endpoint.
- Evidence requires safe success/error/timeout classification, parser failure
  rejection, the forty-row limit and secret/raw-payload exclusion.
- Exact business-state equality and existing Production/FTPS/update locks
  remain mandatory.
- Release smoke `20260727165007-99840438` passes 385/385 with exact logical and
  business-state equality.
- Clean Readiness `20260727165051-713aecf4` passes 285/285 applicable plus
  100 N/A, controlled restart and exact state equality.
- Post-correction smoke `20260727170133-15d13fb7` passes 385/385 and proves the
  exact evidence-ID schema enum with unchanged business state.
- Reasoning-budget and safe-diagnostic correction smoke
  `20260727170841-4473c154` passes 385/385 with exact state.
- Canonical Release rerun `20260727171213-558ca0ca` passes 385/385 with exact
  state after the owner application lock was released.
- Post-owner Verification-state correction smoke `20260727172300-de072a18`
  passes 385/385 with exact business-state equality.
- Canonical Release confirmation `20260727172502-e8736f1b` repeats 385/385
  with exact state.
- Owner live generation, secret-safe evidence, validated advisory Save Session
  and Full Data Verification 385/385 pass on 2026-07-27.

## v52.2.0 - Read-only OpenAI Responses Pilot accepted

- Populated disposable scenarios invoke Preview OpenAI Payload only.
- Preview evidence requires `store=false`, no tools, an allowlisted MaterialID
  payload, forbidden-field absence and `no network used`.
- Clean Readiness records the zero-data boundary without building a payload.
- Generate with OpenAI remains blocked by automation safety policy.
- Verification uses deterministic structured response probes and requires
  unknown evidence MaterialIDs to be rejected.
- Real credentials, API traffic and consent dialogs remain manual owner-only
  acceptance.
- Release smoke `20260727162340-f07d3caa` passes 384/384 with exact logical
  business-state equality.
- Clean Readiness `20260727162422-0de18a69` passes 284/284 applicable plus
  100 N/A with exact state equality.
- Owner live workflow and Full Data Verification acceptance pass on 2026-07-27.

## v52.1.0 - Optional AI Provider Foundation accepted

- Standard disposable scenarios resolve the deterministic fake provider,
  regardless of owner provider preferences.
- The tester invokes only `Test Provider Foundation`, which is a local
  configuration check and never a live OpenAI request.
- The protected API-key control must expose password semantics while owner
  credentials remain inaccessible.
- Evidence requires fake-provider identity and `network used: no`.
- No scenario saves/deletes credentials or sends a material payload.
- Full Data Verification adds the provider foundation and isolation contract.
- Release smoke profile `20260727153808-6a1788dc` passes 383/383 with exact
  business state and no AI session/collection residue.
- Clean profile `20260727153913-5a0120d2` passes 283/283 applicable plus
  100 N/A, zero Materials and a controlled restart.
- After the Settings Grid correction, Release smoke
  `20260727154904-5ef08d0a` passes 383/383 with exact business-state equality.
- Owner visual, provider-state, Diagnostics and Verification acceptance passes
  on 2026-07-27.

## v51.4.0 - Profile Reconciliation candidate

Disposable startup and Diagnostics no longer inspect owner update-transaction
history. The shared descriptor now owns database, preferences, output,
credentials, update transactions, evidence and cleanup. Caller-free legacy
identity and generic network-block adapters are retired; scenario-specific
report, CRUD, recovery and updater authorization remains.

Clean Debug profile `20260727150741-e127db3c` passes 282/282 applicable with
100 canonical-data N/A, zero mandatory N/A and all 17 mandatory-evidence
checks. Diagnostics prove owner credentials inaccessible and zero owner update
transactions.

The final Release matrix passes with exact business-state equality:
Clean `20260727150915-c3619774` at 282/282 plus 100 N/A; populated smoke
`20260727150947-13052381`, Reports `20260727151027-2bcf3bb2`, CRUD
`20260727151210-ced8e3ea`, Recovery `20260727151328-6e7fd208` and real-helper
Updater `20260727151426-82bbb19b` each at 382/382 with zero N/A. Updater proves
both committed success and injected health-failure rollback.

## v51.3.0 - Verification Classification candidate

Verification now exports every check with `Mandatory` or
`CanonicalDataDependent` applicability plus a dedicated mandatory-evidence
flag. Runtime identity/capabilities and the Full Data/Application Readiness
data profile are separate fields. Free-form detail text cannot grant N/A.

Clean Debug profile `20260727145307-ec78b262` passes 281/281 applicable with
100 CanonicalDataDependent N/A, zero mandatory N/A and zero failures across 16
mandatory-evidence checks. The total matrix is 381 checks.

Clean Release profile `20260727145433-df2701f4` repeats 281/281 plus 100
classified N/A with exact state. Populated Release profile
`20260727145601-56734ac4` passes all 381 checks with zero N/A and exact
business-state recovery.

## v51.2.0 - Clean Readiness candidate

The `clean` scenario creates a manifest-contained profile without accepting or
copying a seed database. First launch creates schema v37 only below the
disposable database root. The header, Diagnostics and Verification identify
`CLEAN / READINESS`; owner database, preferences, credentials, Production,
FTPS, updates and all mutating scenario authorizations remain inaccessible.

Profile `20260727143116-3138cb93` passes Application Readiness with 280
mandatory PASS, zero FAIL and 100 explicit zero-data N/A checks. It contains
zero canonical Materials and no seed-evidence file. A controlled restart
preserves identity and zero-data state; the post-initialization logical and
business-state baseline remains exact through final shutdown.

Release profile `20260727143346-142a5972` repeats the same 280/280 applicable,
100 N/A and exact-state result. Populated Release smoke profile
`20260727143423-7961372f` passes Full Data Verification 380/380 with exact
business-state recovery.

Owner execution exposed a transient anonymous WPF Help-menu popup that could
survive the immediate UIA window enumeration and falsely trigger the
unexpected-dialog guard. The runner now retries only an entirely anonymous
owned window for at most one second; any identified or persistent window still
fails immediately or at the bounded deadline. Clean profiles
`20260727144106-e4feeaf6` and `20260727144139-15e29155` pass consecutively.
Populated profile `20260727144213-6e88b752` passes 380/380 with exact state.

## v50.3.0 - Safety and recovery Help accepted

Smoke opens Recovery Center, System Diagnostics and Verification Center in the
disposable profile. It verifies the presence of read-only inspection/export
and distinct guarded/mutating controls but invokes no restore, backup,
recalculation, update, recovery, Production or FTPS action.

Central Help searches prove the no-auto-SQLite-restore update boundary,
mutating Verification recalculation and secret-safe support evidence.
Profile `20260727120029-14c0556f` passes Full Data Verification 374/374 and
exact logical/business-state equality.

The initial run timed out before Diagnostics inspection because the new window
ID was bound to Verification. Correct window ownership is now deterministic.
Owner runtime/safety acceptance passes on 2026-07-27.

## v50.2.4 - Contextual Help coverage accepted

Smoke owns a read-only navigation registry of exactly 22 unique top-level and
16 unique nested AutomationIds. Every surface is selected without authorizing
its actions; an unexpected owned-process window fails the run.

Representative central-Help checks cover Rankings Dashboard, Experimental
Table and Material Detail Notes through Help for Current View. The Website menu
check proves that the supported action selects Website Export while performing
no generation or publication.

Profile `20260727113500-3bc427ca` passes the 22/22 and 16/16 sweeps, contextual
checks, Full Data Verification 373/373 and exact logical/business-state
equality. Production, FTPS, updates, clipboard writes, CRUD, report generation,
recovery and owner database selection remain blocked.
Owner runtime/UI acceptance passes on 2026-07-27.

## v50.2.3 - Output and tool reference accepted

Full Data Verification requires 34 stable Reports, Website, AI, YouTube and
packaged Help destinations, four exact top-level mappings, substantive
multi-paragraph content in every destination and representative local-output,
FTPS, persistence, status and external-calendar boundaries.

The content-depth rule was added after owner review rejected the initially
short structural leaf texts as placeholder-like. The earlier 372/372 result is
therefore superseded and the expanded contract must be rerun.

The read-only Help smoke searches `does not perform FTPS` and
`do not create external calendar events`. Production, FTPS, updates, clipboard
writes and owner database selection remain blocked. Profile
`20260727111924-0056eda1` passes the expanded Help contract, Full Data
Verification 372/372 and exact logical/business-state equality.
Owner runtime/content acceptance passes on 2026-07-27.

## v50.2.2 - Testing and analysis reference accepted

Full Data Verification requires 22 stable Help destinations, exact top-level
mapping and representative native auto-save, Experimental baseline, Rankings
Top 25, Category 10-row and Material Detail Notes markers.

The read-only central Help smoke searches the Rankings default-scope phrase and
the corrected Notes application boundary. Production, FTPS, updates and owner
database selection remain blocked. Profile `20260727103712-93b32f7b` passes
the expanded Help contract, Full Data Verification 371/371 and exact
logical/business-state equality. Owner acceptance remains pending.

Owner-search correction adds a smoke query for `Experimental Table` and
requires the direct visible `Experimental Table reference` title.
Profile `20260727104534-dc519c51` passes title-first relevance, the revised
Purchase Order direct-reference assertion, Full Data Verification 371/371 and
exact logical/business-state equality.

The smoke scenario now requires Help > Documentation to open
`Start-to-finish workflow`; contextual F1 ownership remains separate.
Profile `20260727105308-cc3ce0b1` passes 371/371 with exact state equality.
Owner runtime and visual acceptance pass.

## v50.2.1 - Data, cost and configuration reference accepted

Full Data Verification requires unique, non-empty Help content plus nine stable
section IDs, representative persistence/relationship/immutability markers and
exact top-level tab mappings for the v50.2.1 scope.

The existing smoke scenario remains read-only for Help. In addition to the
accepted workflow search/highlight/wrapping contract, it searches for the
Materials Manual Backup/save boundary and the Printer/saved-quote boundary and
requires the intended reference topic and body text. Production, FTPS, updates
and owner database selection remain blocked. Disposable evidence is pending.

Owner review removed the misleading `immutable` label from current Quote Help.
The smoke contract now requires the supported saved-quote lifecycle wording:
later inputs do not auto-update a saved row and explicit history deletion exists.
Correction profile `20260727101828-1f53f4e1` passes the revised search, Full
Data Verification 370/370 and exact logical/business-state equality.

The smoke scenario now searches `lifecycle` and requires the visible summary
to expose `Highlighted search: lifecycle` through UI Automation. Profile
`20260727102551-3b35ebbc` passes body and summary highlighting, Full Data
Verification 370/370 and exact logical/business-state equality.
Owner runtime and visual acceptance pass.

Initial profile `20260727100746-ddc0c805` passed the Help searches but failed
369/370 because one Verification phrase crossed a source-only line break. The
marker was narrowed to semantic content and no runtime/data behavior changed.
Profile `20260727101000-dddd3159` passes the expanded `central-help` contract,
Full Data Verification 370/370 and exact logical/business-state equality.
Owner runtime and visual acceptance remain pending.

## v50.1.0 - Start-to-finish Help accepted

The smoke scenario searches central Help for `landed costs`, requires a
non-empty result and verifies that the rendered workflow contains both
`Create Materials + Received Spools` and `READY FOR PUBLISH`. Full Data
Verification additionally requires ordered guide markers for landed-cost
persistence, measurement auto-save, public report package handoff and the two
guarded Production confirmations.

The scenario remains read-only for Help, keeps Production/FTPS/update/owner
database actions blocked and requires exact baseline/final logical and
business-state equality. The initial exact-text contract mismatch failed
369/370 and was corrected to the canonical second live-FTPS confirmation.
Disposable profile `20260727092658-c116167b` passes the Help workflow step,
Full Data Verification 370/370 and exact state equality. Owner runtime
acceptance remains pending.

Owner-review correction profile `20260727094039-c70f0cd7` additionally requires
the normalized `scope and output folder` accessibility text after a second Help
search. It passes with Full Data Verification 370/370 and exact state equality.

The follow-up refresh contract exposes only highlight state, never body data,
through `HelpSectionBody` UI Automation HelpText. Smoke requires immediate
`Highlighted search: landed costs`, clears Search and requires
`No highlighted search` before the next query.

Disposable profile `20260727094810-fa07643d` passes that Search/Clear refresh
contract, normalized wrapping, Full Data Verification 370/370 and exact
logical/business-state equality. Owner runtime and visual acceptance pass.

## v50.0.0 - Central user Help accepted

The existing smoke scenario opens the single canonical Help window from the
Help menu, requires a non-empty contextual topic, searches for `recovery` and
requires at least one result. Automation IDs cover the window, search, topic
list, selected content, result status and Close action. Production, FTPS,
updates, owner database selection and unexpected dialogs remain blocked.

Disposable acceptance passes `central-help`, exact baseline/final logical and
business-state hashes, and Full Data Verification 370/370. Visual layout,
readability and workflow wording received explicit owner-runtime acceptance
after the contents-list wrapping correction.

## v49.0.0 - Experimental workflow integrity

- Smoke navigation now selects the Experimental Testing tab by stable
  AutomationId.
- The tester requires owner-visible publication readiness and the explicit
  inactive-history comparison toggle.
- Full Data Verification deterministically proves duplicate lifecycle reset,
  baseline ownership, active-only default scope controls and graph-persistence
  wiring.
- The smoke scenario is read-only for Experimental data. No broad Experimental
  write authorization is introduced in this increment.
- The tester requires the Series Active-only filter to start checked.
- Verification proves incomplete publication defaults to No, explicit
  confirmation enables it, ready publication proceeds and hiding needs no
  confirmation.
- Profile `20260726220205-e1dea7a8` passes Full Data Verification 368/368 and
  exact logical/business-state preservation.
- Owner evidence `3DPIceland_FilamentDB_Verification_20260726_220656.txt`
  passes 368/368 and accepts the corrected runtime behavior.
- Closure smoke `20260726221001-c7dcae2e` passes 368/368 with exact state
  preservation after retired persistence removal.
- Production, FTPS, owner database and unexpected-dialog guards are unchanged.
- HTML/website presentation and partial-publication confirmation remained
  owner-manual and are runtime accepted.

## v48.0.7 - Internal usage analytics

- Reuses the existing disposable `crud` scenario and write authorization.
- Stable AutomationIds expose effective events, ledger rows, net filament,
  print duration and evidence coverage.
- Before correction the tester requires one effective/one ledger row, 100 g
  and 60 minutes.
- After correction it requires one effective/three ledger rows, 80 g and
  55 minutes.
- Exact cleanup and baseline/final business-state equality remain mandatory.
- Production, FTPS, owner database and public publishing remain blocked.
- Profile `20260726175433-70e8dba1` passes Verification 361/361 and proves
  both visible analytics states plus exact final cleanup.
- Owner accepted the visible analytics, manually confirmed public-output
  isolation and passed Full Data Verification 361/361.

## v48.0.6 - Bounded usage UI and automation

- Reuses only the existing disposable `crud` authorization.
- Stable AutomationIds cover the Usage tab, MaterialID selector, Inventory
  selector, ledger, record action, correction action and status evidence.
- After create/restart, the tester requires one visible private ledger row and
  exact SQLite/Inventory evidence.
- After correction/restart, it requires three visible rows plus exact
  original/reversal/replacement and 920 g Inventory evidence.
- Final cleanup and baseline/final business-state equality remain mandatory.
- Production, FTPS, owner database and public publishing stay blocked.
- Profile `20260726173640-4d663edc` passes Verification 360/360 and records
  visible ledger states 1 and 3 with exact Inventory evidence and final cleanup.
- Owner runtime accepted the empty-Inventory boundary, linked-spool flow and
  Full Data Verification 360/360.

## v48.0.5 - Canonical usage persistence and recovery

- Reuses the authorized disposable `crud` scenario; no new broad write
  authorization is introduced.
- After create, SQLite evidence requires one original event and linked
  Inventory remaining weight of 900 g.
- After restart/edit, evidence requires original plus reversal and replacement,
  with linked Inventory reconciled to 920 g.
- After delete/restart, UsageEvents, disposable Inventory and Material rows
  must all be absent and baseline/final business state must match.
- The recovery scenario must prove the governed 22-table workbook and
  transactional restore. Exact schema-v33 workbooks remain supported.
- Production, FTPS, owner database and unexpected-dialog guards are unchanged.
- CRUD profile `20260726171743-3d1e2a09` passes Verification 359/359 and
  restores the exact baseline business-state hash.
- Recovery profile `20260726172034-4e048744` passes Verification 359/359,
  verifies seven backup artifacts and restores the exact baseline
  business-state hash.
- Owner runtime accepted normal behavior with Full Data Verification 359/359.

## v48.0.4 - Disposable usage domain prototype

- No runtime write workflow or UI action exists, so no new scenario
  authorization or AutomationId is added.
- Smoke Full Data Verification executes synthetic identity, correction,
  projection and inventory-delta contracts.
- SQLite, seed, Inventory state and baseline/final business state must remain
  unchanged.
- Production, FTPS, owner-database and unexpected-dialog guards are unchanged.
- Disposable profile `20260726165857-04d43636` passes Full Data Verification
  358/358 with exact database and business-state hash equality.
- Owner runtime accepted normal startup and Full Data Verification 358/358.

## v48.0.3 - Usage event contract

- Documentation/ownership contract only; no runtime behavior or schema changed.
- No new UI action, AutomationId, scenario authorization or seed is required.
- Existing Production, FTPS, owner-database and unexpected-dialog guards remain
  unchanged.
- v48.0.4 pure domain work must remain non-persistent; persistence later
  requires bounded disposable CRUD/reversal/recovery evidence.

## v48.0.2 - Governed value index

- No new write workflow exists; smoke automation does not select mutable
  Recommendation state.
- Verification proves Overall-score/MSRP calculation, both inputs, comparison
  scope, disclosure and missing-score/price behavior.
- Stable `RecommendationDetailValueIndex` AutomationId is present for future
  bounded UI automation.
- Profile `20260726161514-840e50cc` passes 352/352 with exact database byte and
  business-state equality.
- Selection-scope correction profile `20260726162917-7e102563` passes 353/353;
  exact Base Material scope and database/business-state equality pass.
- MSRP identity profile `20260726163905-dfb93502` passes 354/354 and proves the
  exact recommended MaterialID is shown beside its canonical MSRP.
- Owner runtime accepted PLA/ASA scope refresh, alternatives and price identity;
  owner Full Data Verification passes 354/354.
- Production, FTPS, owner-database and unexpected-dialog guards are unchanged.

## v48.0.1 candidate - Canonical pricing provenance

- No new UI action or write workflow is introduced, so no tester scenario is
  added.
- Verification deterministically proves valid configured-rate conversion.
- Missing rates, unsupported currencies and missing canonical MSRP remain
  unavailable without 1:1, landed-cost or legacy substitution.
- Profile `20260726150144-83168b2e` passes 351/351 with exact database byte and
  business-state equality.
- Production, FTPS, owner-database and unexpected-dialog guards are unchanged.
- Owner runtime accepted the pricing surfaces and Full Data Verification PASS.

## v47.0.3 candidate - Stable coverage identity

- Smoke automation reads the coverage identity summary from disposable storage.
- A new disposable profile must begin with zero stable and zero legacy entries.
- Automation never invokes the explicit binding action.
- Verification proves stable-first lookup, one unique exact binding candidate
  and preservation of unmatched legacy entries.
- Stable AutomationIds cover the binding action and identity summary.
- Production, FTPS, owner-database and unexpected-dialog guards are unchanged.
- Profile `20260726141853-e0ce0c53` passes 350/350 with zero stable and zero
  legacy entries in isolated storage and exact pre/post database-state equality.
- Owner runtime accepted visible identity state, zero-legacy behavior, coverage
  workflows and Full Data Verification PASS.

## v47.0.2 candidate - AI collection workflow clarity

- Every scenario reads the stable collection Create/Update action state.
- The runner invokes only the read-only collection preview.
- Preview must state that no data was written and report unique MaterialIDs.
- Standard automation never invokes collection create/update or touches
  personal AppData collection storage.
- Existing Production, FTPS, owner-database and unexpected-dialog boundaries
  remain unchanged.
- Initial profile `20260726133919-b630ebf4` failed only the in-process
  Verification control-name lookup; no canonical or owner data was involved.
- Corrected profile `20260726134033-9c21103d` passed but exposed an owner
  collection title because legacy AI storage still resolved to personal
  AppData. It did not write collection data.
- AI session/collection storage now resolves to the disposable PreferencesFolder
  under automation and retains normal AppData ownership outside automation.
- Owner review found misleading cancelled-update output; persisted data was not
  written, but proposed filter scope looked saved.
- Verification now proves that cancel output retains persisted MaterialIDs and
  excludes proposed IDs.
- Final profile `20260726135702-ceb78987` passes read-only preview, cancel-state
  honesty, explicit absence of disposable AI session/collection files, Full
  Data Verification 349/349 and exact logical/business-state equality without
  personal AppData access.
- Owner runtime accepted the workflow and Full Data Verification 349/349.
  v47.0.2 is canonical.

## v47.0.1 candidate - Local AI Assistant scope clarity

- Every scenario navigates `AiAssistantTab` by stable AutomationId.
- The runner explicitly refreshes and reads the visible MaterialID scope.
- The scope must report unique MaterialIDs and expose a bounded MaterialID
  preview before generation.
- The runner generates a local full brief and requires visible-scope evidence
  in the read-only output.
- No external network, credential, destructive action or scenario
  authorization is added.
- The smoke scenario remains read-only and must preserve exact baseline/final
  canonical business state.
- Profile `20260726122624-99485959` passes with 201 visible rows, 201 unique
  MaterialIDs and Full Data Verification 348/348.
- Logical hash before/after:
  `F0EDCC3295A114C935668D2B92D7A1AEB1C67C4D1630EFC89F11B7FCDC556E5F`.
- Normalized business-state hash before/after:
  `4FBCF6A2656678875A6692C0A7AA30CD0CDC3F4AAB83003B3BB2C77081B1C87D`.
- Owner runtime accepted the visible-scope workflow and Full Data Verification
  348/348. v47.0.1 is canonical.

## Approved local disposable seed

The owner-approved local automation seed is:

`C:\Seed-Database\filamentdb.sqlite`

This path must still be passed explicitly with `--seed-database`. The runner
copies it below the disposable `%TEMP%\3DPIceland-Automation\<ProfileId>` root
and never mutates the source seed. It must not be generalized into owner-data
discovery or fallback selection.

## v44.7.14 Stage 1 candidate

The Stage 1 runner is a read-only smoke and evidence tool. It does not replace
owner runtime or visual acceptance.

Safety boundaries:

- The runner never discovers or selects the canonical owner database.
- `--seed-database` is mandatory and must identify an explicitly chosen copy.
- The seed is copied below `%TEMP%\3DPIceland-Automation\<ProfileId>`.
- The application accepts an automation profile only through the exact
  `.3dpiceland-disposable-profile.json` marker below that temporary root.
- The marker binds the exact application executable SHA-256.
- Database, preferences, output and evidence paths must remain below the same
  disposable root.
- Production, FTPS and application updates are blocked while the profile is
  active.
- SQLite/Excel restore and Material deletion are blocked in Stage 1.
- An unexpected owned-process dialog/window fails the run.
- UI input is sent only through Windows UI Automation to the launched process.
- Screenshots contain only the owned application or Verification window.
- Consistent before/after SQLite snapshots are retained as evidence.
- A canonical logical hash over schema, tables, columns and values must remain
  unchanged after controlled shutdown; WAL/header byte normalization is not
  treated as a logical data change.

Build:

```powershell
dotnet build App/FilamentDbApp.sln -c Debug
```

Run with an explicit non-live database copy:

```powershell
App\AutomationRunner\bin\Debug\net9.0-windows\3DPIcelandAutomationRunner.exe `
  --app App\FilamentDbApp\bin\Debug\net9.0-windows\3DPIcelandFilamentDB.exe `
  --seed-database C:\path\to\explicit\filamentdb-test-copy.sqlite
```

Evidence is retained below the disposable profile:

- `evidence\main-window.png`
- `evidence\verification-center.png`
- `evidence\verification.txt`
- `evidence\verification.json`
- `evidence\run-result.txt`
- `evidence\run-result.json`
- `evidence\database-before.sqlite`
- `evidence\database-after.sqlite`

The first scenarios verify exact-process startup, visible disposable identity,
stable tab/control navigation, Verification export and controlled shutdown.
Fast-grid cells remain outside Stage 1 because the custom drawn surface does
not yet expose row/cell automation peers.

## v44.7.15 Stage 2 accepted

The report scenario requires explicit `--scenario reports`. Its manifest
authorizes local report generation only, binds the Reports output box to the
disposable profile output folder and keeps Production, FTPS, updates, restore
and delete actions blocked.

```powershell
App\AutomationRunner\bin\Debug\net9.0-windows\3DPIcelandAutomationRunner.exe `
  --app App\FilamentDbApp\bin\Debug\net9.0-windows\3DPIcelandFilamentDB.exe `
  --seed-database C:\path\to\explicit\filamentdb-test-copy.sqlite `
  --scenario reports
```

Stage 2 invokes the canonical `Build Public Report Package` action and waits
for its existing six-report workflow. It validates catalog-owned safe routes,
non-empty HTML/PDF/JSON artifacts, canonical HTML markers, PDF headers and
SHA-256 values. Evidence adds:

- `evidence\report-package.png`
- `evidence\report-artifacts.json`
- `evidence\report-artifacts.txt`

Representative HTML/PDF paths are recorded as `MANUAL REVIEW REQUIRED`.
Automation never declares visual layout acceptance.

Disposable acceptance on 2026-07-25 passed Full Data Verification 341/341,
validated 211 catalog entries and retained hashes for 639 catalog/root
artifacts. Before/after logical SQLite hashes matched. PDF rendering exposed a
Material Summary final-page table clip; deterministic 20-row presentation
tables corrected it while retaining the same allowlisted rows, order, routes
and report ownership. Owner runtime accepted Full Data Verification 341/341,
the landscape PDF and responsive HTML behavior.

## v44.7.16 Stage 3 accepted

The CRUD scenario requires explicit `--scenario crud`. Its manifest authorizes
one exact generated disposable MaterialID; it does not release the general
Material deletion lock or permit owner-path selection.

```powershell
App\AutomationRunner\bin\Debug\net9.0-windows\3DPIcelandAutomationRunner.exe `
  --app App\FilamentDbApp\bin\Debug\net9.0-windows\3DPIcelandFilamentDB.exe `
  --seed-database C:\path\to\explicit\filamentdb-test-copy.sqlite `
  --scenario crud
```

Stage 3 uses a narrow in-process Automation contract because Fast-grid cells do
not yet expose stable automation peers. It exercises the canonical Materials
row model, computed fields and SQLite collection save:

- create and save one valid uniquely identified disposable record;
- restart, prove create persistence, edit and save;
- restart, prove edit persistence, delete only the exact authorized record;
- restart and prove the record is absent.

Consistent SQLite snapshots and full logical hashes are retained after create,
edit and delete. The final business-state hash excludes only columns named
exactly `UpdatedAtUtc`; full hashes remain visible. This records autosave
timestamp movement while requiring every row count and canonical business
value to return to its baseline. Unexpected dialogs, Production, FTPS, updates,
restore and owner paths remain blocked.

Disposable acceptance on 2026-07-25 passes Full Data Verification
342/342. The exact generated record `AUTce389f57` persisted through create and
edit restarts, was deleted through the authorized contract and was absent
after the final restart. Three per-action snapshots were retained and the
before/after business-state hashes match.
Owner runtime then accepted normal create/edit/delete persistence and cleanup;
owner Full Data Verification passes 342/342.

## v44.7.17 Stage 4 accepted

The backup/recovery scenario requires explicit `--scenario recovery`. It
creates and verifies a manual `.bak`, retains a verified legacy `.sqlite`
compatibility copy, exports a governed Excel recovery package, applies one
disposable mutation and restores the package. SQLite restore remains blocked
and is never automated.

The Excel restore now retains both pre-restore and post-restore SQLite evidence.
If post-restore evidence fails after commit, the verified pre-restore backup is
used for immediate rollback. The runner controls restart under the same exact
manifest and requires the final business-state hash to equal baseline.

Disposable acceptance passes Full Data Verification 343/343. The
governed workbook, both filename generations and pre/post restore artifacts
retain bytes and SHA-256 evidence; baseline/final business-state hashes match.
Owner runtime accepted backup discovery, Excel recovery, pre/post evidence and
Full Data Verification 343/343.

## v44.7.18 Stage 5 accepted

The guarded updater scenario requires explicit `--scenario updater` plus an
explicit `--updater` helper path. Its manifest keeps general updates,
Production and FTPS blocked while authorizing only the isolated Stage 5
transaction.

```powershell
App\AutomationRunner\bin\Debug\net9.0-windows\3DPIcelandAutomationRunner.exe `
  --app App\FilamentDbApp\bin\Debug\net9.0-windows\3DPIcelandFilamentDB.exe `
  --seed-database C:\Seed-Database\filamentdb.sqlite `
  --scenario updater `
  --updater App\FilamentDbUpdater\bin\Debug\net9.0\3DPIcelandUpdater.exe
```

The runner copies only build artifacts into a disposable portable directory.
It applies an identical staged build through the real updater helper, requires
an exact v44.7.18 health acknowledgement, then stages an intentionally invalid
disposable executable and requires complete rollback. The updater forwards the
same validated disposable profile both after install and after rollback.

Disposable evidence passes Full Data Verification 344/344. The success
transaction commits 54 governed files, the failure transaction reaches
`RolledBack`, all pre-update SHA-256 values are restored and the canonical
business-state hash remains equal to baseline. Owner runtime then accepted
normal startup, owner-data behavior and Full Data Verification 344/344.
# v45.1 disposable CRUD extension

The final unlinked-review addition is covered by Full Data Verification rather
than fragile grid input automation. Profile `20260725225702-22872c00` passes
345/345 and verifies that the counted Manufacturer filter represents null
`ManufacturerId` rows. The CRUD scenario also returns the database business
state to its exact baseline; owner, Production and FTPS paths remain blocked.
Owner runtime accepted the same-name selection and zero-count collapsed UI;
final owner Full Data Verification passes 345/345.

The accepted canonical tester seed is now schema v32 at
`C:\Seed-Database\filamentdb.sqlite`, SHA-256
`65BD03F668768F0AAEBF937BAFC628559A168EA1A07E586CECADC7431AF7BB84`.
The prior schema-v31 fixture remains at
`C:\Seed-Database\filamentdb-schema31-migration.sqlite`, SHA-256
`7851D26BA82E345E2C4B156996B68F0360B7F74A48B777752A609CB368EFD6D4`.
Profile `20260725233352-929a4f1a` passes CRUD, Full Data Verification 345/345
and exact baseline/final business-state hash equality on the schema-v32 seed.

## v45.2.0 Base Materials workspace extension

Every scenario now navigates `BaseMaterialsTab` by stable AutomationId.
Disposable CRUD additionally creates one uniquely named Base Material, restarts
and verifies it, edits it, creates a duplicate, restarts and verifies both,
then removes both and proves absence. This reuses the exact disposable profile
and business-state rollback boundary; owner paths, Production and FTPS remain
blocked. Fast-grid visual editing remains owner-manual because the owner-drawn
cells do not expose stable per-cell automation peers.

Profile `20260726094502-312196e2` passes the extended CRUD lifecycle and Full
Data Verification 346/346. The final business-state hash equals baseline
`5922D4E7B8AF2FB045C07F1F5F813E1C050C5F17B562003504FCBC15EDBDBDF0`,
and the canonical schema-v32 seed remains byte-identical.
Startup-fix profile `20260726095812-c88314b0` also passes immediate tab
navigation, CRUD, 346/346 and exact business-state cleanup.

The `crud` scenario remains confined to its exact generated disposable
MaterialID and a disposable Manufacturer catalog record. Create stores a
deliberate legacy/unmapped Manufacturer with null ID and restart preserves it
exactly. Edit creates and selects the catalog identity; restart proves
`ManufacturerId` plus canonical name persistence. The final stage proves
canonical rename propagation, referenced-delete blocking, rename restoration,
Material/catalog deletion and lossless sequence cleanup. The runner still does
not automate Fast-grid cells, owner data, Production or FTPS.

Disposable ManufacturerId acceptance run `20260725222727-42605144` passed Full
Data Verification 345/345. It preserved the deliberate unmapped Manufacturer
through create/restart, persisted the explicit catalog ID through edit/restart,
propagated a catalog rename, blocked referenced deletion and removed all
generated records. Final business-state hash matched baseline. The approved
source seed retained its original
4,825,088 bytes, timestamp and SHA-256
`7851D26BA82E345E2C4B156996B68F0360B7F74A48B777752A609CB368EFD6D4`.

## v45.2.1 canonical Base Material identity extension

The existing disposable CRUD scenario now starts with a deliberate unlinked
Base Material text value, creates the matching catalog row, explicitly stores
its `BaseMaterialId`, and proves the relationship after restart. It also proves
referenced-delete blocking, ID-owned rename propagation, rename restoration,
catalog cleanup and final business-state hash equality. Fast-grid visual input
remains owner-manual; Production, FTPS and owner paths remain blocked.

Candidate profile `20260726101917-5bc56749` passes Full Data Verification
347/347 and the complete disposable CRUD lifecycle. The final business-state
hash is `A199A91611C51D6074D147B16AB538B9A45485B1D4CB65E58C9EA5ECF54DC8CC`.

Dropdown-refresh profile `20260726104609-8be75c9a` re-passes 347/347 and exact
business-state cleanup while the CRUD rename uses the production edit handler
and proves the old choice disappears and the new choice is immediately present.

Owner final evidence `3DPIceland_FilamentDB_Verification_20260726_105027.txt`
passes 347/347 with zero unlinked Materials. The normalized schema-v33 seed at
`C:\Seed-Database\filamentdb.sqlite` has SHA-256
`50782D4E2DBE8F773E0A915E9E2460525B43FB68611E19DD6EB12F47B131AB31`.
Profile `20260726105424-55caa15d` passes CRUD, 347/347 and exact business-state
equality on that state. Schema-v32 is preserved as
`filamentdb-schema32-migration.sqlite`, SHA-256
`65BD03F668768F0AAEBF937BAFC628559A168EA1A07E586CECADC7431AF7BB84`.
Final canonical-path profile `20260726105756-ce0811c4` repeats the same PASS and
business-state hash `4FBCF6A2656678875A6692C0A7AA30CD0CDC3F4AAB83003B3BB2C77081B1C87D`.
## v61.0.2 Heat Deflection workspace candidate

- Read-only smoke traverses 23/23 top-level tabs and 23/23 grouped Navigate
  destinations, including stable Heat Deflection IDs.
- Full Verification uses disposable temporary SQLite to prove manual insert,
  update, date/notes, method binding, blank-clear and exact final state without
  authorizing owner-data mutation.
- Profile `20260814174328-29081146` passes 427/427 and exact business-state
  recovery. Visual grid use remains owner-manual.

## v61.0.1 thermal schema and workbook import

- The existing read-only smoke remains the bounded automation owner; no import
  apply authorization or user-facing control is added before v61.0.2.
- Full Data Verification deterministically covers schema v41, immutable method
  v1, exact MaterialID binding, blanks, locale parsing, rejection, transactional
  apply and idempotence without reading owner files or writing owner data.
- Schema-v40 migration profile `20260814165236-dfcee36b` and refreshed
  221-Material schema-v41 seed profile `20260814171134-29850755` both pass 426/426 with exact
  business-state recovery and Production, FTPS, updates and owner data blocked.
- The real corrected workbook is inspected only through the read-only verifier;
  all 221 IDs resolve, with 191 inserts, 30 blanks, zero issues and no write.

## v59.0.6 Materials column order

- The existing read-only smoke remains the bounded automation owner; no new
  scenario or mutation authorization is added for a local layout preference.
- Full Data Verification requires the exact approved 52-column default order
  and a synthetic reset contract that clears both Materials preference stores
  while preserving an unrelated grid layout.
- Corrected disposable Release smoke `20260729012749-9dc37e1a` passes 418/418
  with exact logical and business-state recovery, including the new default-No
  Materials reset contract.
- Visual order, manual drag persistence and Reset Columns across application
  restart remain owner-manual because UI Automation would add low-value
  preference mutation beyond the established read-only smoke boundary.
- Owner runtime acceptance confirms exact order, saved/reset restart behavior,
  Purchase Price/Currency adjacency and No/Escape/window-close cancellation.
  Owner Full Data Verification passes.
