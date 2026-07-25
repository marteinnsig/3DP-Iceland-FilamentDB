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
