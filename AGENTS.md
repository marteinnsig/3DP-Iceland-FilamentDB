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
