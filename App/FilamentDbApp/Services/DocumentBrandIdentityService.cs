using FilamentDbApp.Data;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services;

public sealed class DocumentBrandIdentityService
{
    public const string DefaultBrandDisplayName = "3DPIceland Labs";
    public const int MaximumLength = 80;

    private readonly LocalDatabase _database;

    public DocumentBrandIdentityService(LocalDatabase database)
    {
        _database = database ?? throw new ArgumentNullException(nameof(database));
    }

    public string Resolve()
    {
        var stored = _database.LoadDocumentBrandDisplayName();
        return TryNormalize(stored, out var normalized)
            ? normalized
            : DefaultBrandDisplayName;
    }

    public string Save(string value)
    {
        if (!TryNormalize(value, out var normalized))
            throw new ArgumentException(
                $"Brand / Organization Name must contain 1–{MaximumLength} visible characters.",
                nameof(value));
        _database.SaveDocumentBrandDisplayName(normalized);
        return normalized;
    }

    public void RestoreDefault()
    {
        _database.ClearDocumentBrandDisplayName();
    }

    internal static bool TryNormalize(string? value, out string normalized)
    {
        normalized = Regex.Replace(value?.Trim() ?? string.Empty, @"\s+", " ");
        return normalized.Length is > 0 and <= MaximumLength &&
               normalized.All(character => !char.IsControl(character));
    }
}
