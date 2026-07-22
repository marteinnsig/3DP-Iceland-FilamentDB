﻿using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FilamentDbApp.Models;

public sealed class MaterialExperimentRecord : INotifyPropertyChanged
{
    private bool _isActive = true;
    private bool _publishOnWebsite;
    private string _updatedAtUtc = string.Empty;

    public string MaterialExperimentId { get; set; } = string.Empty;
    public string MaterialID { get; set; } = string.Empty;
    public string ExperimentDefinitionId { get; set; } = string.Empty;
    public string ParameterValue { get; set; } = string.Empty;
    public string ParameterUnit { get; set; } = string.Empty;
    public string BaselineMaterialID { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;

    public bool PublishOnWebsite
    {
        get => _publishOnWebsite;
        set => SetField(ref _publishOnWebsite, value);
    }

    public bool IsActive
    {
        get => _isActive;
        set => SetField(ref _isActive, value);
    }

    public string CreatedAtUtc { get; set; } = string.Empty;

    public string UpdatedAtUtc
    {
        get => _updatedAtUtc;
        set => SetField(ref _updatedAtUtc, value);
    }

    public string MaterialDisplayName { get; set; } = string.Empty;
    public string BaselineMaterialDisplayName { get; set; } = string.Empty;
    public string ExperimentName { get; set; } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;

    private void SetField<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
