using System.Security.Cryptography;

namespace FilamentDbApp.Services;

public static class ThermalDeflectionMethodContract
{
    public const string Version = "3dp-thermal-deflection-fixture-v1";
    public const string SnapshotJson = """
        {"methodVersion":"3dp-thermal-deflection-fixture-v1","result":"nearby BlueDOT probe-indicated temperature at 2.00 mm mid-span deflection","standardClaim":"3DPIceland fixture-specific; not ASTM D648 or ISO 75 HDT","specimen":{"lengthMm":127.0,"widthMm":12.7,"thicknessMm":3.2,"orientation":"flat"},"fixture":{"clearSpanMm":110.0,"movingLoadG":54.0,"nominalLoadN":0.530,"load":"centered M20 nut","centralBoltAddsSpecimenLoad":false},"sensor":{"name":"BlueDOT","vendor":"thermapen.co.uk","fccId":"2A167 BlueDot","location":"nearby under specimen beside central assembly","userCalibration":false},"heating":{"environment":"oven","ambientStartC":25.0,"ramp":"non-linear observed","checkpoints":[{"temperatureC":50,"elapsed":"00:01:50"},{"temperatureC":100,"elapsed":"00:03:26"},{"temperatureC":150,"elapsed":"00:04:35"},{"temperatureC":200,"elapsed":"00:06:53"},{"temperatureC":250,"elapsed":"00:10:30"}]},"testsPerMaterial":1,"unit":"degC"}
        """;

    public static string SnapshotSha256 =>
        Convert.ToHexString(SHA256.HashData(System.Text.Encoding.UTF8.GetBytes(SnapshotJson)));
}

public sealed record ThermalDeflectionExportRow(
    string MaterialId,
    double? TemperatureC,
    string? MeasuredDate,
    string? TestNotes,
    string? MethodVersion,
    string? SourceFileName,
    string? SourceSha256,
    string? ImportedAtUtc,
    string? UpdatedAtUtc);
