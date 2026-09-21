using FilamentDbApp.Models;
using FilamentDbApp.Services.Calculations;
using System.Globalization;

namespace FilamentDbApp;

public partial class MainWindow
{
    private const string PendulumSettingsSection = "Izod / Charpy";

    private bool ValidatePendulumSettings(IReadOnlyList<MaterialsPrototypeChange>? changes = null)
    {
        double? Read(string parameter)
        {
            var row = _nativeSettingsRows.FirstOrDefault(item => item.Section == PendulumSettingsSection && item.Parameter == parameter);
            var value = changes?.LastOrDefault(change => ReferenceEquals(change.Row.Source, row) && change.Column.PropertyName == nameof(NativeSettingRow.Value))?.NewValue ?? row?.Value;
            return new StatisticsService().ParseNullableDouble(value);
        }
        var length = Read("Specimen length"); var width = Read("Specimen width");
        var thickness = Read("Specimen thickness"); var notch = Read("Notch depth");
        return length is > 0 && width is > 0 && thickness is > 0 && notch is >= 0 && notch < width;
    }

    private PendulumImpactRunRecord CreateDirectPendulumRunProfile(string materialId, string method)
    {
        double Read(string name, double fallback) => new StatisticsService().ParseNullableDouble(
            _nativeSettingsRows.FirstOrDefault(row => row.Section == PendulumSettingsSection && row.Parameter == name)?.Value) ?? fallback;
        var width = Read("Specimen width", 10);
        return new PendulumImpactRunRecord
        {
            MaterialID = materialId, Method = method, InputMode = "DirectStrength", TargetCount = "10",
            NominalLengthMm = Read("Specimen length", 80).ToString(CultureInfo.InvariantCulture),
            NominalWidthMm = width.ToString(CultureInfo.InvariantCulture),
            NominalThicknessMm = Read("Specimen thickness", 4).ToString(CultureInfo.InvariantCulture),
            NominalLigamentMm = (width - Read("Notch depth", 2)).ToString(CultureInfo.InvariantCulture),
            NotchType = "Notched", CreatedAtUtc = DateTime.UtcNow.ToString("O")
        };
    }
}
