using System.Globalization;

namespace FilamentDbApp.Services;

public sealed class EngineeringValueIndexService
{
    public EngineeringValueIndexResult Calculate(
        double? overallEngineeringScore,
        double? canonicalMsrpUsdPerKg,
        string? comparisonScope)
    {
        var scope = string.IsNullOrWhiteSpace(comparisonScope)
            ? "current filtered material dataset"
            : comparisonScope.Trim();

        if (!overallEngineeringScore.HasValue ||
            !double.IsFinite(overallEngineeringScore.Value) ||
            overallEngineeringScore.Value < 0)
            return EngineeringValueIndexResult.NotRecorded(
                scope,
                "Overall engineering score is not recorded.");

        if (!canonicalMsrpUsdPerKg.HasValue ||
            !double.IsFinite(canonicalMsrpUsdPerKg.Value) ||
            canonicalMsrpUsdPerKg.Value <= 0)
            return EngineeringValueIndexResult.NotRecorded(
                scope,
                "Canonical MSRP USD/kg is not recorded.");

        return EngineeringValueIndexResult.Available(
            overallEngineeringScore.Value,
            canonicalMsrpUsdPerKg.Value,
            scope);
    }
}

public sealed record EngineeringValueIndexResult(
    bool HasValue,
    double? Value,
    double? OverallEngineeringScore,
    double? CanonicalMsrpUsdPerKg,
    string ComparisonScope,
    string MissingReason)
{
    public string Summary => HasValue
        ? "3DPIceland value index: " +
          Value!.Value.ToString("0.00", CultureInfo.InvariantCulture) +
          " overall-score points per USD/kg. Inputs: Overall " +
          OverallEngineeringScore!.Value.ToString("0.0", CultureInfo.InvariantCulture) +
          "/100; canonical MSRP $" +
          CanonicalMsrpUsdPerKg!.Value.ToString("0.00", CultureInfo.InvariantCulture) +
          " USD/kg. Scope: " + ComparisonScope +
          ". Comparative index, not a physical property."
        : "3DPIceland value index: Not recorded. " + MissingReason +
          " Scope: " + ComparisonScope +
          ". No landed-cost or inferred-price substitution.";

    public static EngineeringValueIndexResult Available(
        double overallEngineeringScore,
        double canonicalMsrpUsdPerKg,
        string comparisonScope) =>
        new(
            true,
            overallEngineeringScore / canonicalMsrpUsdPerKg,
            overallEngineeringScore,
            canonicalMsrpUsdPerKg,
            comparisonScope,
            string.Empty);

    public static EngineeringValueIndexResult NotRecorded(
        string comparisonScope,
        string missingReason) =>
        new(false, null, null, null, comparisonScope, missingReason);
}
