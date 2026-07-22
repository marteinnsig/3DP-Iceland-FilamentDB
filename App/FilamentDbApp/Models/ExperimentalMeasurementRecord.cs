using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FilamentDbApp.Models;

public sealed class ExperimentalMeasurementRecord : INotifyPropertyChanged
{
    private string _sample1="",_sample2="",_sample3="",_sample4="",_sample5="",_sample6="",_sample7="",_sample8="",_sample9="",_sample10="";
    private string _notes="",_updatedAtUtc="",_resultAverage="",_resultStdDev="",_resultCv="",_resultCount="",_resultConfidence="";
    public string ExperimentalMeasurementId { get; set; } = "";
    public string ExperimentalRunId { get; set; } = "";
    public string MeasurementType { get; set; } = "";
    public string Orientation { get; set; } = "";
    public string RawUnit { get; set; } = "";
    public string ResultUnit { get; set; } = "";
    public string Sample1 { get=>_sample1; set=>SetSample(ref _sample1,value); }
    public string Sample2 { get=>_sample2; set=>SetSample(ref _sample2,value); }
    public string Sample3 { get=>_sample3; set=>SetSample(ref _sample3,value); }
    public string Sample4 { get=>_sample4; set=>SetSample(ref _sample4,value); }
    public string Sample5 { get=>_sample5; set=>SetSample(ref _sample5,value); }
    public string Sample6 { get=>_sample6; set=>SetSample(ref _sample6,value); }
    public string Sample7 { get=>_sample7; set=>SetSample(ref _sample7,value); }
    public string Sample8 { get=>_sample8; set=>SetSample(ref _sample8,value); }
    public string Sample9 { get=>_sample9; set=>SetSample(ref _sample9,value); }
    public string Sample10 { get=>_sample10; set=>SetSample(ref _sample10,value); }
    public string Notes { get=>_notes; set=>SetField(ref _notes,value); }
    public string UpdatedAtUtc { get=>_updatedAtUtc; set=>SetField(ref _updatedAtUtc,value); }
    public string ResultAverage { get=>_resultAverage; set=>SetField(ref _resultAverage,value); }
    public string ResultStdDev { get=>_resultStdDev; set=>SetField(ref _resultStdDev,value); }
    public string ResultCv { get=>_resultCv; set=>SetField(ref _resultCv,value); }
    public string ResultCount { get=>_resultCount; set=>SetField(ref _resultCount,value); }
    public string ResultConfidence { get=>_resultConfidence; set=>SetField(ref _resultConfidence,value); }
    public string DisplayName => string.IsNullOrWhiteSpace(Orientation) ? MeasurementType : $"{MeasurementType} – {Orientation}";
    public IEnumerable<string> SampleValues() => new[]{Sample1,Sample2,Sample3,Sample4,Sample5,Sample6,Sample7,Sample8,Sample9,Sample10};
    public event PropertyChangedEventHandler? PropertyChanged;
    public void NotifyCalculated(){ foreach(var n in new[]{nameof(ResultAverage),nameof(ResultStdDev),nameof(ResultCv),nameof(ResultCount),nameof(ResultConfidence),nameof(DisplayName)}) PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(n)); }
    private void SetSample(ref string field,string value,[CallerMemberName]string? name=null)
    {
        var candidate = value ?? "";
        if (TryParse(candidate, out var number))
        {
            var max = MeasurementType switch
            {
                "Tensile" => 505d,
                "Impact" => 100d,
                "Stiffness" when name == nameof(Sample1) => 10d,
                "Stiffness" when name == nameof(Sample2) => 359d,
                _ => double.MaxValue
            };
            if (number > max)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
                return;
            }
        }
        if(field==candidate)return;
        field=candidate;
        PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(name));
    }
    private static bool TryParse(string value, out double number)
    {
        var normalized = (value ?? "").Trim().Replace(',', '.');
        return double.TryParse(normalized, System.Globalization.NumberStyles.Float, System.Globalization.CultureInfo.InvariantCulture, out number);
    }
    private void SetField<T>(ref T field,T value,[CallerMemberName]string? name=null){ if(EqualityComparer<T>.Default.Equals(field,value))return; field=value; PropertyChanged?.Invoke(this,new PropertyChangedEventArgs(name)); }
}
