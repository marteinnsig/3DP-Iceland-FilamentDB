using System.Windows.Controls;
using FilamentDbApp.Models;

namespace FilamentDbApp;

public partial class MainWindow
{
    private bool VerifyPendulumRankingContract()
    {
        var a = new VideoPlannerRow { MaterialId = "IZOD", Label = "Izod leader", IzodScore = 90, CharpyScore = 20 };
        var b = new VideoPlannerRow { MaterialId = "CHARPY", Label = "Charpy leader", IzodScore = 30, CharpyScore = 80 };
        var izod = BuildRankingRow(a, "Izod");
        var charpy = BuildRankingRow(b, "Charpy");
        var labels = RankingMetricFilter.Items.OfType<ComboBoxItem>().Select(item => item.Content?.ToString()).ToList();
        var recommendationLabels = RecommendationUseCaseFilter.Items.OfType<ComboBoxItem>()
            .Select(item => item.Content?.ToString()).ToList();
        foreach (var method in new[] { "Izod", "Charpy" })
        {
            var type = "Highest " + method;
            if (!recommendationLabels.Contains(type) || !IsPerformanceLeaderRecommendation(type) ||
                RecommendationSortGroup(type) != 2 ||
                !BuildRecommendationSuggestedTitle(type, a, 90).Contains(method, StringComparison.Ordinal) ||
                !BuildRecommendationBestUse(type, a).Contains(method, StringComparison.Ordinal) ||
                !BuildRecommendationThumbnailHook(type, a, 90).Contains(method, StringComparison.OrdinalIgnoreCase) ||
                !BuildRecommendationVideoAngle(type, a, 90).Contains(method, StringComparison.Ordinal)) return false;
        }
        return labels.Contains("Izod") && labels.Contains("Charpy") &&
            izod.RankScore == 90 && charpy.RankScore == 80 &&
            BuildRankingRow(new VideoPlannerRow(), "Izod").RankScore is null &&
            CategoryMetricDefinitions().Single(x => x.Name == "Best Izod").ScoreSelector(izod) == 90 &&
            CategoryMetricDefinitions().Single(x => x.Name == "Best Charpy").ScoreSelector(charpy) == 80 &&
            AwardDefinitions().Single(x => x.Name == "Best Izod Material").ScoreSelector(izod) == 90 &&
            AwardDefinitions().Single(x => x.Name == "Best Charpy Material").ScoreSelector(charpy) == 80 &&
            BestProfileDimension(new EngineeringScoreProfile { CharpyScore = 80 }) == "Charpy impact resistance" &&
            BuildMaterialTalkingPoints(a, HighestAxisLabel(a)).Contains("Izod 90", StringComparison.Ordinal) &&
            FindStrongestOutlier(a, new List<VideoPlannerRow> { b }).Axis == "Izod" &&
            BiggestMetricGap(new VideoPlannerRow { IzodScore = 90 }, new VideoPlannerRow(), out _, out _) == 0;
    }
}
