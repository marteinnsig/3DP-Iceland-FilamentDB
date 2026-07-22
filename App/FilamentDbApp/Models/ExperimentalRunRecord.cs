using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FilamentDbApp.Models;

public sealed class ExperimentalRunRecord : INotifyPropertyChanged
{
    private bool _isBaseline;
    private bool _isActive = true;
    private string _updatedAtUtc = string.Empty;
    public string ExperimentalRunId { get; set; } = string.Empty;
    public string MaterialExperimentId { get; set; } = string.Empty;
    public string ParameterValue { get; set; } = string.Empty;
    public string ParameterUnit { get; set; } = string.Empty;
    public string Status { get; set; } = "Planned";
    public string Notes { get; set; } = string.Empty;
    public bool IsBaseline { get => _isBaseline; set => SetField(ref _isBaseline, value); }
    public bool IsActive { get => _isActive; set => SetField(ref _isActive, value); }
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string UpdatedAtUtc { get => _updatedAtUtc; set => SetField(ref _updatedAtUtc, value); }
    public event PropertyChangedEventHandler? PropertyChanged;
    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
