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

- Give each future major version one coherent strategic theme and a bounded
  completion condition.
- Advance to a new major version when the next material milestone has a
  different theme. Do not accumulate long sequences of unrelated work under
  one major version merely to avoid advancing the version number.
- Use minor or patch increments only for related stages, corrections and
  acceptance work inside the same major milestone.
- Never renumber completed, canonical or runtime-accepted releases. Roadmap
  renumbering applies only to unstarted planning slots.
