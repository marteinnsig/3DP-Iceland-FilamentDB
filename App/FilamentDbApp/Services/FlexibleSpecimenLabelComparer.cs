using FilamentDbApp.Models;
using System.Collections;
using System.Globalization;
using System.Text.RegularExpressions;

namespace FilamentDbApp.Services;

/// <summary>Natural label order for the specimen view only; saved labels and identities remain unchanged.</summary>
public sealed class FlexibleSpecimenLabelComparer : IComparer<string>, IComparer
{
    public static readonly FlexibleSpecimenLabelComparer Ascending = new();
    private readonly bool _descending;
    public FlexibleSpecimenLabelComparer(bool descending = false) => _descending = descending;

    public int Compare(string? x, string? y)
    {
        var left = Regex.Split(x ?? string.Empty, "([0-9]+)");
        var right = Regex.Split(y ?? string.Empty, "([0-9]+)");
        for (var i = 0; i < Math.Min(left.Length, right.Length); i++)
        {
            int result;
            if (i % 2 == 1)
            {
                // Compare digit lengths, then digits: no integer overflow for long user-entered labels.
                var a = left[i].TrimStart('0');
                var b = right[i].TrimStart('0');
                result = a.Length.CompareTo(b.Length);
                if (result == 0) result = string.CompareOrdinal(a, b);
            }
            else result = CultureInfo.CurrentCulture.CompareInfo.Compare(left[i], right[i], CompareOptions.IgnoreCase);
            if (result != 0) return _descending ? -Math.Sign(result) : Math.Sign(result);
        }
        var remaining = left.Length.CompareTo(right.Length);
        return _descending ? -remaining : remaining;
    }

    int IComparer.Compare(object? x, object? y) => Compare(
        (x as FlexibleTestSpecimenRecord)?.SpecimenLabel, (y as FlexibleTestSpecimenRecord)?.SpecimenLabel);

    public static bool Verify()
    {
        var labels = new[] { "Specimen 10", "Specimen 2", "Specimen 15", "Specimen 1", "Specimen 9" };
        var ordered = labels.OrderBy(x => x, Ascending).ToArray();
        return ordered.SequenceEqual(new[] { "Specimen 1", "Specimen 2", "Specimen 9", "Specimen 10", "Specimen 15" }) &&
            labels.OrderBy(x => x, new FlexibleSpecimenLabelComparer(true)).SequenceEqual(ordered.Reverse()) &&
            Ascending.Compare("Sample 2 point 9", "Sample 2 point 10") < 0 &&
            Ascending.Compare("Specimen 99999999999999999999", "Specimen 100000000000000000000") < 0 &&
            Ascending.Compare("Specimen 02", "Specimen 2") == 0;
    }
}
