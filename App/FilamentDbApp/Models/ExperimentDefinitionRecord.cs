namespace FilamentDbApp.Models;

public sealed class ExperimentDefinitionRecord
{
    public string ExperimentDefinitionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string ParameterKey { get; set; } = string.Empty;
    public string DefaultUnit { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public int SortOrder { get; set; }
}
