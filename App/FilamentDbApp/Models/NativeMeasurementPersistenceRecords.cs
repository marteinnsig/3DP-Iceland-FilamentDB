namespace FilamentDbApp.Models;

public sealed class NativeTensilePersistenceRecord
{
    public string MaterialId { get; set; } = string.Empty;
    public List<string> UprightSamples { get; set; } = new();
    public List<string> FlatSamples { get; set; } = new();
    public string UprightMpa { get; set; } = string.Empty;
    public string FlatMpa { get; set; } = string.Empty;
    public string StdDevUpright { get; set; } = string.Empty;
    public string StdDevFlat { get; set; } = string.Empty;
    public string CvUpright { get; set; } = string.Empty;
    public string CvFlat { get; set; } = string.Empty;
    public string SamplesUpright { get; set; } = string.Empty;
    public string SamplesFlat { get; set; } = string.Empty;
    public string ConfidenceUpright { get; set; } = string.Empty;
    public string ConfidenceFlat { get; set; } = string.Empty;
    public string TestNotes { get; set; } = string.Empty;
}

public sealed class NativeImpactPersistenceRecord
{
    public string MaterialId { get; set; } = string.Empty;
    public List<string> UprightSamples { get; set; } = new();
    public List<string> FlatSamples { get; set; } = new();
    public string TestNotes { get; set; } = string.Empty;
}

public sealed class NativeStiffnessPersistenceRecord
{
    public string MaterialId { get; set; } = string.Empty;
    public string Revolutions { get; set; } = string.Empty;
    public string Degrees { get; set; } = string.Empty;
    public string TestNotes { get; set; } = string.Empty;
}
