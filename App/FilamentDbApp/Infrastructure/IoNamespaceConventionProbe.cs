using System.Windows.Shapes;

namespace FilamentDbApp.Infrastructure;

// Compile-only guard: this file deliberately imports the WPF namespace that
// also defines Path. A missing project-wide IO alias therefore breaks every
// Debug/Release build before ambiguous path code can reach runtime.
internal static class IoNamespaceConventionProbe
{
    internal static string Normalize(string path) => IOPath.GetFullPath(path);

    internal static bool FileExists(string path) => IOFile.Exists(path);

    internal static bool DirectoryExists(string path) => IODirectory.Exists(path);

    internal static Type WpfPathType => typeof(Path);
}
