# Repository coding rules

## WPF and System.IO type names

- In C# changes, use the project-wide `IOPath`, `IOFile`, and `IODirectory`
  aliases for new filesystem code.
- Do not add bare `Path.*` calls to WPF files. `System.Windows.Shapes.Path`
  and `System.IO.Path` make the bare name ambiguous.
- Use an explicitly qualified WPF type when constructing a shape in C#.
- Do not repair this class of error by removing required WPF namespaces.
- Before completing a code change, build both Debug and Release. If a running
  app locks the normal output, use an isolated MSBuild `ArtifactsPath` instead
  of stopping the user's application.

## Feature retirement and cleanup

- When functionality, a UI action, a workflow, or a larger system surface is
  removed or replaced, research every related caller, handler, state field,
  adapter, XAML element, service, test, Verification gate, and document.
- Do not delete the accepted path until its replacement has passed appropriate
  runtime acceptance.
- After the replacement is runtime accepted, remove the retired code in the
  same increment or make the immediately following authoritative roadmap
  increment a bounded cleanup/retirement increment. Do not move on to unrelated
  feature work while known retired implementation remains unowned.
- Do not retain unused code "just in case." Retained compatibility, migration,
  updater, evidence, diagnostics, export, or recovery code must have a real
  supported caller and a documented ownership reason.
- Give every temporary adapter, fallback, compatibility bridge, or hidden
  legacy surface an explicit removal condition. Re-check that condition after
  the replacement receives runtime acceptance.
- Cleanup is complete only after related callers, handlers, state, adapters,
  XAML, services, tests, Verification gates, and documentation have been
  reviewed and retired or deliberately retained.
- Preserve supported historical data and artifact compatibility. Code required
  to discover, inspect, migrate, restore, or explain supported older data is
  not dead code merely because the normal UI no longer creates that format.
- Do not rely on static unreferenced-code detection alone. Trace reflection,
  XAML event bindings, serialization, migration, recovery, diagnostics, updater,
  export, and Verification entry points before deletion.
- Run Debug and Release builds, relevant static/security gates, and Full Data
  Verification after a material cleanup. Require runtime acceptance before
  closing and committing a cleanup increment.

## Automated acceptance maintenance

- For every new feature, changed workflow, retired action, or material behavior
  change, explicitly assess whether the automated acceptance runner,
  `AutomationId` coverage, scenario authorization, retained evidence, or Full
  Data Verification contracts must change.
- Update the tester in the same increment whenever the change creates or alters
  an important runtime contract that can be tested safely and deterministically.
- Prefer extending the existing smoke, reports, CRUD, recovery, or updater
  scenario when its safety boundary already owns the behavior. Add a new
  bounded scenario only when the workflow needs distinct authorization,
  isolation, evidence, or destructive-action guards.
- Keep Production, FTPS, owner-database, unexpected-dialog, owned-process input,
  and disposable-profile policies intact when extending automation.
- Do not add low-value automation merely to mirror cosmetic text or layout.
  Keep visual HTML, PDF, wrapping, clipping, branding, and usability acceptance
  manual when deterministic automation would not provide reliable evidence.
- Document why tester changes are required or why no tester change is warranted
  in the increment evidence and release notes.

## Proactive subagent delegation

- Codex is authorized to use subagents proactively when a task contains
  concrete, independent workstreams that benefit from parallel research,
  codebase mapping, test analysis, or independent review.
- The lead agent remains responsible for synthesis, implementation ownership,
  repository integrity, final verification, runtime handoff, and release
  closure.
- Prefer read-only subagent work for exploration, triage, log analysis, and
  review. Give each subagent a bounded scope and an explicit deliverable.
- Avoid parallel writes to overlapping files, schemas, databases,
  documentation, or other shared mutable state.
- Do not use subagents when work is small, sequential, tightly coupled, or
  unlikely to benefit after coordination overhead.
- Tell the user when subagents are used and summarize how their findings
  influenced the final implementation or decision.

## Canonical tester seed maintenance

- Codex is authorized to refresh `C:\Seed-Database\filamentdb.sqlite` when a
  schema change, accepted data contract, migration requirement, tester need, or
  other concrete acceptance reason makes the existing canonical seed stale.
- Treat this as standing project authorization; a separate user confirmation is
  not required for each justified seed refresh. Filesystem sandbox or escalation
  approval may still be required by the execution environment.
- Never copy or mutate the active owner database directly. Use an explicit
  owner-approved Manual Backup or a validated disposable derivative as source.
- Before replacement, validate exact paths, SQLite integrity, schema version,
  relationship integrity, automation residue and relevant row counts.
- Preserve the prior seed under an explicit schema/migration fixture name when
  it remains required for supported migration coverage. Never silently discard
  the last required legacy fixture.
- After replacement, require source/target SHA-256 evidence, disposable tester
  acceptance, Full Data Verification and exact baseline/final business-state
  recovery where the scenario supports it.
- If the candidate fails any gate, restore the last accepted canonical seed,
  retain failure evidence and report the reason.
- Record accepted seed paths, schema versions, hashes and tester evidence in
  the current release or automated-acceptance documentation.

## Build artifact cleanup

- After each completed release, build, publish, or push workflow, clean the
  contents of `C:\3DPIceland-App-Codex\artifacts`.
- Perform cleanup only after every build, tester, packaging, verification, or
  deployment consumer for that workflow has finished.
- Treat `artifacts` as disposable build output. Move any evidence or release
  artifact that must be retained to its governed long-term location before
  cleanup.
- Resolve and verify the exact absolute target before recursive cleanup. Never
  apply this rule to `.artifacts`, the repository root, source directories,
  canonical seed fixtures, or external release/evidence locations.
- Leave the empty `artifacts` directory available for later isolated builds.

## Automated tester profile cleanup

- After each completed and accepted commit or push workflow, clean disposable
  tester profiles below
  `C:\Users\maddi\AppData\Local\Temp\3DPIceland-Automation`.
- Perform cleanup only after every tester, Verification, recovery, diagnostics,
  evidence, and runtime-acceptance consumer for that workflow has finished.
- Preserve the latest accepted profile temporarily when it is still needed for
  evidence, review, diagnostics, or screen recording.
- Preserve failed profiles until the failure has been investigated and all
  evidence required for correction or release documentation has been retained.
- Remove older successful disposable profiles after Verification and exact
  business-state recovery have passed and required evidence has been moved to
  its governed long-term location.
- Resolve and verify the exact absolute profile paths before recursive cleanup.
  Never apply this rule to the temp root, owner AppData, canonical seed fixtures,
  the active owner database, or any path outside the tester profile root.

## Roadmap major-version governance

- When a milestone is divided into planned increments, record every agreed
  increment in `Docs/Roadmaps/MASTER_ROADMAP.md` immediately, before
  implementation starts. Give each increment an explicit scope, state and
  completion condition.
- Treat those recorded increments as the authoritative delivery sequence.
  Do not close the parent milestone or advance to the next major version until
  every recorded increment is complete, deliberately deferred or formally
  removed by an owner-approved roadmap decision.
- If implementation reveals a new required increment, add it to the current
  roadmap milestone before starting that work. Never rely on a chat-only plan
  as the sole record of remaining release scope.
- Give each future major version one coherent strategic theme and a bounded
  completion condition.
- Advance to a new major version when the next material milestone has a
  different theme. Do not accumulate long sequences of unrelated work under
  one major version merely to avoid advancing the version number.
- Use minor or patch increments only for related stages, corrections and
  acceptance work inside the same major milestone.
- Never renumber completed, canonical or runtime-accepted releases. Roadmap
  renumbering applies only to unstarted planning slots.

## GitHub README release hygiene

- Before closing each major-version milestone, review and update the repository
  root `README.md` in the same accepted release workflow.
- Align its current runtime-accepted release and development-focus text with
  `Docs/Roadmaps/MASTER_ROADMAP.md`; never advertise an unaccepted candidate as
  canonical.
- Keep stable public links for the latest Windows installer and latest portable
  ZIP visible near the top of the README.
- Include a downloads-page link only while its public route is independently
  verified accessible; a route returning 403 must not be advertised.
- Use the governed stable routes rather than a version-specific artifact URL:
  `https://www.iskort.is/3dp/downloads/3DPIceland-Setup-x64.exe` and
  `https://www.iskort.is/3dp/downloads/3DPIceland-Portable-x64.zip`.
- Verify README links and release identity during major-version documentation
  closure. README drift blocks closing the parent major milestone.

## Major-release standing FTPS authorization

- The owner grants standing authorization to complete the guarded application
  FTPS publication after each fully accepted major version. A separate repeated
  FTPS confirmation is not required when every condition below is satisfied.
- This authorization activates only after every recorded increment in the major
  is complete, owner runtime acceptance is recorded, Full Data Verification is
  PASS and all required build, security, documentation and package gates pass.
- Require a clean committed release state and exact accepted Production bytes.
  Never rebuild or substitute artifacts between runtime acceptance and publish.
- The standing scope is limited to the approved Windows installer, portable
  ZIP, signed application update package and update feed for that major release.
- At final v59 closure, the standing scope also includes the accepted
  `v59.0.1` governed public-demo ZIP containing the SQLite demo database plus
  its governed public manifest/checksum artifacts. Publish only the exact
  rebuilt and runtime-accepted demo bytes.
- Publish immutable versioned installer/portable/update routes first. Activate
  stable installer/portable routes and `/updates/latest.json` only after their
  exact predecessor uploads verify.
- Take a remote backup before stable-route mutation, retain rollback evidence
  and independently download every stable HTTPS artifact after activation.
  Require exact bytes, size, release identity and SHA-256 parity.
- Stop fail-closed on local drift, remote inventory drift, backup failure,
  upload failure, hash mismatch or HTTPS verification failure. Preserve the
  prior stable routes or execute the governed rollback; never continue partly.
- Except for the exact accepted v59.0.1 public-demo package above, this standing
  authorization does not include website content, public reports, raw SQLite
  files, owner data, FTPS credential changes or artifacts outside the governed
  release plans. Those remain separately authorized.
- Keep credentials and private remote details out of logs and documentation.
  Record secret-safe plans, hashes, backups, transfers, remote verification and
  final route identity in the accepted major-release evidence.
- If major-version completion or owner runtime acceptance is ambiguous, do not
  infer authorization. Stop before Production promotion or FTPS mutation.

## In-application Help maintenance

- For every user-visible feature, workflow, field, control, menu item, runtime
  window, validation rule or safety-boundary change, assess and update the
  in-application Help in the same increment.
- Keep Help synchronized with actual labels, navigation paths, units, defaults,
  save timing, persistence ownership, offline behavior, failure handling and
  read-only versus mutating boundaries.
- Update `HelpContentCatalog`, the Help coverage registry/ledger and relevant
  deterministic Verification or tester contracts whenever supported UI
  ownership or behavior changes.
- When functionality is retired or replaced, remove or redirect stale Help
  destinations in the same accepted retirement workflow. Preserve historical
  release documentation, but never leave current Help describing a retired
  action.
- New controls and fields must have a stable Help destination before their
  increment can close. Existing controls whose behavior changes require their
  current Help text to be revalidated even when their AutomationId is unchanged.
- Cosmetic-only changes that do not alter labels, discoverability, behavior or
  user decisions do not require low-value Help text changes; record that
  assessment in increment evidence.
- Run the Help coverage/drift gates and require owner readability/navigation
  acceptance for material Help changes. Stale or missing Help blocks release
  closure.

## Owner runtime test handoff clarity

- Write owner test checklists as exact click-by-click instructions suitable for
  a user who does not know whether a step belongs in Help, a tab, a menu or a
  separate runtime window.
- For every step state: **Where**, **Action**, **Expected result**, and
  **Do not click** when adjacent destructive or live controls exist.
- Say explicitly when the user should search inside the Help window versus
  navigate to and operate the actual application surface.
- Name the full menu/tab path and the exact visible control label.
- Keep read-only inspection steps separate from mutating runtime acceptance.
- Never ask the owner to infer whether Restore, Recalculate, Update, Production,
  FTPS, Delete or another guarded action is authorized.

## Cross-chat continuity and autonomous increment flow

- Treat owner changes from another Codex chat, side chat or manual edit as
  legitimate workspace changes. Preserve and reconcile them, especially in
  `README.md`, roadmap, feedback and release documentation; never remove or
  overwrite them merely because they were not created in the current chat.
- If an external/side-chat change conflicts with canonical data, accepted
  release identity or another owner decision, show the exact conflict and
  resolve it with the owner instead of silently choosing one version.
- After the owner approves a recorded sequence of related sub-increments, Codex
  may continue from one sub-increment to the next without requesting repeated
  approval when no new owner decision, runtime/visual test, destructive action,
  external publication or expanded authority is required.
- Pause autonomous progression whenever runtime acceptance is required, a new
  choice would materially change scope, or Production, FTPS, owner data,
  credentials, deletion or another guarded boundary needs fresh authority.
- During autonomous multi-increment work, provide concise progress updates that
  state what is complete, what is being worked on and what comes next. Do not
  leave the owner to infer whether work stopped or timed out.
- When an implementation batch is complete, all required gates pass, Full Data
  Verification passes where applicable and owner runtime acceptance has been
  received, Codex is authorized to commit and push at the next sensible clean
  completion point. Verify branch, worktree and origin alignment first and
  report the resulting commit/push status.
- Documentation-only owner-approved changes may be committed and pushed after
  their applicable documentation/static gates pass and branch/worktree/origin
  state has been verified; do not require unrelated runtime testing.
- Preserve
  `C:\3DPIceland-App-Codex\App\FilamentDbApp\bin\Release\net9.0-windows`
  as the owner's continuing runnable application directory. An intentional
  accepted Release build may update it, but cleanup must never delete or empty
  it.
