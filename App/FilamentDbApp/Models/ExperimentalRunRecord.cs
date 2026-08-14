using System.ComponentModel;
using System.Globalization;
using System.Runtime.CompilerServices;
using FilamentDbApp.Services;

namespace FilamentDbApp.Models;

public sealed class ExperimentalRunRecord : INotifyPropertyChanged
{
    private bool _isBaseline;
    private bool _isActive = true;
    private string _updatedAtUtc = string.Empty;
    private DateTime? _measuredDate;
    private string _measuredDateText = string.Empty;
    public string ExperimentalRunId { get; set; } = string.Empty;
    public string MaterialExperimentId { get; set; } = string.Empty;
    public string ParameterValue { get; set; } = string.Empty;
    public string ParameterUnit { get; set; } = string.Empty;
    public string Status { get; set; } = "Planned";
    public string Notes { get; set; } = string.Empty;
    public DateTime? MeasuredDate
    {
        get => _measuredDate;
        set
        {
            var date = value?.Date;
            var text = ApplicationDateCodec.FormatForDisplay(date);
            if (_measuredDate == date && _measuredDateText == text) return;
            _measuredDate = date;
            _measuredDateText = text;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeasuredDate)));
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeasuredDateText)));
        }
    }
    public string MeasuredDateText
    {
        get => _measuredDateText;
        set
        {
            var candidate = value ?? string.Empty;
            if (_measuredDateText == candidate) return;
            _measuredDateText = candidate;
            if (string.IsNullOrWhiteSpace(candidate))
            {
                _measuredDate = null;
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeasuredDate)));
            }
            else if (ApplicationDateCodec.TryCanonicalizeUserInput(candidate, CultureInfo.CurrentCulture, out var canonical) &&
                     ApplicationDateCodec.TryParseStored(canonical, out var parsed))
            {
                _measuredDate = parsed.Date;
                _measuredDateText = ApplicationDateCodec.FormatForDisplay(_measuredDate);
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeasuredDate)));
            }
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(nameof(MeasuredDateText)));
        }
    }
    public bool IsBaseline { get => _isBaseline; set => SetField(ref _isBaseline, value); }
    public bool IsActive { get => _isActive; set => SetField(ref _isActive, value); }
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string UpdatedAtUtc { get => _updatedAtUtc; set => SetField(ref _updatedAtUtc, value); }
    public event PropertyChangedEventHandler? PropertyChanged;
    private bool SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        return true;
    }
}
