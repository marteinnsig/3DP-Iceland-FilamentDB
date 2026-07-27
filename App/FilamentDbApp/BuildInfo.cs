using System.Reflection;

namespace FilamentDbApp;

/// <summary>
/// Central source for release identity shown by the application UI.
/// The numeric version is read from the project assembly metadata; release code
/// and title are defined once here for both the main window and splash screen.
/// </summary>
public static class BuildInfo
{
    public const string ReleaseCode = "AI-PROVIDER-FOUNDATION";
    public const string ReleaseTitle = "Optional AI Provider Foundation";
    public const int MinimumUpdateDatabaseSchema = 29;
    public const int CurrentDatabaseSchema = 37;

    public static string Version
    {
        get
        {
            var informationalVersion = Assembly
                .GetExecutingAssembly()
                .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?
                .InformationalVersion;

            if (!string.IsNullOrWhiteSpace(informationalVersion))
            {
                var versionPart = informationalVersion.Split('+')[0];
                var suffixIndex = versionPart.IndexOf('-');
                return suffixIndex >= 0 ? versionPart[..suffixIndex] : versionPart;
            }

            return Assembly.GetExecutingAssembly().GetName().Version?.ToString() ?? "0.0.0";
        }
    }

    public static string ShortLabel => $"v{Version} {ReleaseCode}";

    public static string MainHeaderText =>
        $"Engineering Platform • {ShortLabel} {ReleaseTitle}";
}
