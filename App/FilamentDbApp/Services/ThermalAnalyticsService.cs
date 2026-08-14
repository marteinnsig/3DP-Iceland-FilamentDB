namespace FilamentDbApp.Services;

public sealed record ThermalAnalyticsProjection(
    double ResultTemperatureC,
    double Score,
    string ContractVersion,
    double ReferenceTemperatureC);

public static class ThermalAnalyticsService
{
    public const string ContractVersion = "3dp-thermal-analytics-fixture-v1";
    public const double ReferenceTemperatureC = 200.0;
    public const int MinimumRankedPeerCount = 2;

    public static ThermalAnalyticsProjection? Project(double? resultTemperatureC)
    {
        if (!resultTemperatureC.HasValue || !double.IsFinite(resultTemperatureC.Value)) return null;

        return new ThermalAnalyticsProjection(
            resultTemperatureC.Value,
            Math.Clamp(resultTemperatureC.Value / ReferenceTemperatureC * 100.0, 0.0, 100.0),
            ContractVersion,
            ReferenceTemperatureC);
    }

    public static bool VerifyContract()
    {
        var missing = Project(null);
        var low = Project(44.0);
        var acceptedMaximum = Project(171.0);
        var reference = Project(200.0);
        var aboveReference = Project(240.0);

        return missing is null &&
               low is { Score: 22.0 } &&
               acceptedMaximum is { Score: 85.5 } &&
               reference is { Score: 100.0 } &&
               aboveReference is { Score: 100.0 } &&
               MinimumRankedPeerCount == 2;
    }
}
