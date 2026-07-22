using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FilamentDbApp.Models;

public sealed class ManufacturerRecord : INotifyPropertyChanged
{
    private string _name = string.Empty;
    private string _displayName = string.Empty;
    private string _country = string.Empty;
    private string _founded = string.Empty;
    private string _website = string.Empty;
    private string _logoUrl = string.Empty;
    private string _description = string.Empty;
    private string _engineeringFocus = string.Empty;
    private string _materialCategories = string.Empty;
    private string _strengths = string.Empty;
    private string _weaknesses = string.Empty;
    private string _sustainability = string.Empty;
    private string _typicalApplications = string.Empty;
    private string _headquarters = string.Empty;
    private string _notes = string.Empty;
    private int _sortOrder = 100;
    private bool _isActive = true;

    public long ManufacturerId { get; set; }
    public string Name { get => _name; set => Set(ref _name, value); }
    public string DisplayName { get => _displayName; set => Set(ref _displayName, value); }
    public string Country { get => _country; set => Set(ref _country, value); }
    public string Founded { get => _founded; set => Set(ref _founded, value); }
    public string Website { get => _website; set => Set(ref _website, value); }
    public string LogoUrl { get => _logoUrl; set => Set(ref _logoUrl, value); }
    public string Description { get => _description; set => Set(ref _description, value); }
    public string EngineeringFocus { get => _engineeringFocus; set => Set(ref _engineeringFocus, value); }
    public string MaterialCategories { get => _materialCategories; set => Set(ref _materialCategories, value); }
    public string Strengths { get => _strengths; set => Set(ref _strengths, value); }
    public string Weaknesses { get => _weaknesses; set => Set(ref _weaknesses, value); }
    public string Sustainability { get => _sustainability; set => Set(ref _sustainability, value); }
    public string TypicalApplications { get => _typicalApplications; set => Set(ref _typicalApplications, value); }
    public string Headquarters { get => _headquarters; set => Set(ref _headquarters, value); }
    public string Notes { get => _notes; set => Set(ref _notes, value); }
    public int SortOrder { get => _sortOrder; set => Set(ref _sortOrder, value); }
    public bool IsActive { get => _isActive; set => Set(ref _isActive, value); }
    public string CreatedAtUtc { get; set; } = string.Empty;
    public string UpdatedAtUtc { get; set; } = string.Empty;

    public event PropertyChangedEventHandler? PropertyChanged;
    private void Set<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value)) return;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
