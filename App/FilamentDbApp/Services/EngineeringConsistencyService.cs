using FilamentDbApp.Models;
using FilamentDbApp.Services.Calculations;

namespace FilamentDbApp.Services;

/// <summary>
/// Interprets repeatability from an existing Verified Material Summary.
/// It consumes calculated CV and sample-count values and never recalculates raw
/// measurements or removes specimen-level outliers.
/// </summary>
public sealed class EngineeringConsistencyService
{
    private const int AdequateSampleCount = 5;

    public EngineeringConsistencyInsight Analyze(MaterialResults? summary, double? canonicalConsistencyScore = null)
    {
        if (summary is null)
        {
            return Unavailable("Verified Material Summary is unavailable; repeatability and outlier review cannot be assessed.");
        }

        var sets = GetMeasurementSets(summary)
            .Where(item => item.Result.SampleCount > 0)
            .ToList();
        if (sets.Count == 0)
        {
            return Unavailable("Verified Material Summary contains no repeated tensile or impact measurement sets.", usesVerifiedSummary: true);
        }

        var cvSets = sets
            .Where(item => item.Result.CoefficientOfVariation is >= 0)
            .Select(item => new
            {
                item.Name,
                item.Result.SampleCount,
                CvPercent = item.Result.CoefficientOfVariation!.Value * 100.0
            })
            .Where(item => double.IsFinite(item.CvPercent))
            .ToList();
        var adequateSets = sets.Count(item => item.Result.SampleCount >= AdequateSampleCount);

        if (cvSets.Count == 0)
        {
            return new EngineeringConsistencyInsight
            {
                StatusLabel = "Insufficient repeatability evidence",
                RepeatabilitySummary = $"{sets.Count} verified measurement set{Plural(sets.Count)} available, but CV requires at least two valid specimens per set. {adequateSets}/{sets.Count} sets contain at least {AdequateSampleCount} specimens.",
                OutlierReviewSummary = "Specimen-level outliers cannot be assessed from the available aggregate summary; inspect traceable raw samples and failure notes before excluding any value.",
                MeasurementSetCount = sets.Count,
                AdequateSampleSetCount = adequateSets,
                UsesVerifiedMaterialSummary = true,
                DirectSpecimenOutlierDetectionAvailable = false
            };
        }

        var averageCv = cvSets.Average(item => item.CvPercent);
        var highest = cvSets.OrderByDescending(item => item.CvPercent).First();
        var summaryConsistencyScore = ConsistencyCalibrationService.CalculateScore(
            cvSets.Select(item => (double?)item.CvPercent),
            sets.Select(item => (double?)item.Result.SampleCount));
        var consistencyScore = canonicalConsistencyScore.HasValue && double.IsFinite(canonicalConsistencyScore.Value)
            ? Math.Clamp(canonicalConsistencyScore.Value, 0.0, 100.0)
            : summaryConsistencyScore;
        var calibratedRating = ConsistencyCalibrationService.InterpretScore(consistencyScore);
        var reviewSets = cvSets.Where(item => item.CvPercent >= ConsistencyCalibrationService.ReviewCvPercent).ToList();
        var highVariationSets = cvSets.Where(item => item.CvPercent >= ConsistencyCalibrationService.HighVariationCvPercent).ToList();

        var repeatability = $"{cvSets.Count}/{sets.Count} verified measurement sets provide CV; average CV {averageCv:0.0}% and highest {highest.Name} {highest.CvPercent:0.0}%. {adequateSets}/{sets.Count} sets contain at least {AdequateSampleCount} specimens.";
        var outlierReview = reviewSets.Count == 0
            ? "No 3DPIceland internal variation-review flag is present. This does not prove that individual outliers are absent; raw specimens and failure notes remain authoritative."
            : $"Review {string.Join(", ", reviewSets.Select(item => $"{item.Name} ({item.CvPercent:0.0}% CV)"))}. " +
              (highVariationSets.Count > 0 ? "At least one set exceeds the internal 40% high-variation boundary. " : string.Empty) +
              "High CV is a review signal, not proof that any specimen is an outlier; values require a test-specific failure reason before exclusion.";

        return new EngineeringConsistencyInsight
        {
            StatusLabel = calibratedRating.Label,
            RepeatabilitySummary = $"Consistency score {calibratedRating.Score:0.0}/100 on the {ConsistencyCalibrationService.ScaleName}. {repeatability}",
            OutlierReviewSummary = outlierReview,
            AverageCvPercent = averageCv,
            HighestCvPercent = highest.CvPercent,
            HighestVariationSet = highest.Name,
            MeasurementSetCount = sets.Count,
            CvSetCount = cvSets.Count,
            AdequateSampleSetCount = adequateSets,
            ReviewFlagCount = reviewSets.Count,
            ConsistencyScore = consistencyScore,
            UsesVerifiedMaterialSummary = true,
            DirectSpecimenOutlierDetectionAvailable = false
        };
    }

    private static EngineeringConsistencyInsight Unavailable(string message, bool usesVerifiedSummary = false) => new()
    {
        StatusLabel = "Repeatability unavailable",
        RepeatabilitySummary = message,
        OutlierReviewSummary = "No outlier claim is made without verified repeatability evidence and traceable specimen-level context.",
        UsesVerifiedMaterialSummary = usesVerifiedSummary,
        DirectSpecimenOutlierDetectionAvailable = false
    };

    private static IReadOnlyList<(string Name, MeasurementSetResult Result)> GetMeasurementSets(MaterialResults summary)
    {
        var sets = new List<(string Name, MeasurementSetResult Result)>();
        if (summary.Tensile is not null)
        {
            sets.Add(("Tensile upright", summary.Tensile.Upright));
            sets.Add(("Tensile flat", summary.Tensile.Flat));
        }
        if (summary.Impact is not null)
        {
            sets.Add(("Impact upright", summary.Impact.Upright));
            sets.Add(("Impact flat", summary.Impact.Flat));
        }
        return sets;
    }

    private static string Plural(int count) => count == 1 ? string.Empty : "s";
}
