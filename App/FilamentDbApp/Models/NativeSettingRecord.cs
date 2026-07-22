namespace FilamentDbApp.Models;

public sealed class NativeSettingRecord
{
    public string Section { get; set; } = string.Empty;
    public string Parameter { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Unit { get; set; } = string.Empty;
    public string UsedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;
}
