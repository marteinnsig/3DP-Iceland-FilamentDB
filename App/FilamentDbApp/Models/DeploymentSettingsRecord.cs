namespace FilamentDbApp.Models;

public sealed class DeploymentSettingsRecord
{
    public string FtpsHost { get; set; } = string.Empty;
    public int FtpsPort { get; set; } = 21;
    public string FtpsUserName { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
}
