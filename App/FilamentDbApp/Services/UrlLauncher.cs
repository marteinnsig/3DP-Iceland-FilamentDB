using System.Diagnostics;

namespace FilamentDbApp.Services;

public static class UrlLauncher
{
    public static void Open(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
