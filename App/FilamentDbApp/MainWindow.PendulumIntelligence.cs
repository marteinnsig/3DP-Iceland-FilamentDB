using System.Data;
using System.Globalization;

namespace FilamentDbApp;

public partial class MainWindow
{
    // Callers supply their already-filtered cohort. Reference scores remain the
    // canonical active-material scores; no method is substituted for another.
    private string BuildPendulumIntelligenceEvidence(IReadOnlyList<VideoPlannerRow> rows) =>
        BuildPendulumIntelligenceEvidenceCore(rows, row =>
        {
            var projection = GetPendulumProjection(row.MaterialId);
            return (projection?.Izod?.MeanKjM2, projection?.Charpy?.MeanKjM2);
        });

    private static string BuildPendulumIntelligenceEvidenceCore(
        IReadOnlyList<VideoPlannerRow> rows,
        Func<VideoPlannerRow, (double? Izod, double? Charpy)>? rawMeans = null)
    {
        static string Number(double value) => value.ToString("0.###", CultureInfo.CurrentCulture);
        static bool Present(double? value) => value.HasValue && double.IsFinite(value.Value) && value.Value >= 0;
        var evidence = rows.Select(row => (Row: row, Raw: rawMeans?.Invoke(row) ?? (null, null))).ToList();
        var lines = new List<string> { "IZOD AND CHARPY EVIDENCE" };
        foreach (var method in new[] { "Izod", "Charpy" })
        {
            var values = evidence.Select(item => (item.Row,
                    Score: method == "Izod" ? item.Row.IzodScore : item.Row.CharpyScore,
                    Mean: method == "Izod" ? item.Raw.Item1 : item.Raw.Item2))
                .Where(item => Present(item.Score) || Present(item.Mean))
                .OrderByDescending(item => Present(item.Score) ? item.Score : null)
                .ThenByDescending(item => Present(item.Mean) ? item.Mean : null)
                .ThenBy(item => item.Row.Label, StringComparer.CurrentCultureIgnoreCase)
                .ToList();
            if (values.Count == 0)
            {
                lines.Add($"{method}: no measured result in this scope.");
                continue;
            }

            var leader = values[0];
            var rawText = Present(leader.Mean) ? $"mean {Number(leader.Mean!.Value)} kJ/m²" : "raw mean unavailable";
            var scoreText = Present(leader.Score) ? $"score {Number(leader.Score!.Value)}/100" : "score unavailable";
            lines.Add($"{method}: {values.Count}/{rows.Count} materials with evidence; leader {leader.Row.Label}: {rawText}; {scoreText}.");
        }
        lines.Add("Compare Izod with Izod and Charpy with Charpy. Missing results are not zero; zero is a measured result.");
        return string.Join(Environment.NewLine, lines);
    }

    private void AppendAiPendulumContext(List<string> lines, IReadOnlyList<DataRow> rows)
    {
        var candidates = rows.Select(row =>
        {
            var id = GetCell(row, "Material ID", "MaterialID").Trim();
            var projection = GetPendulumProjection(id);
            return new VideoPlannerRow
            {
                MaterialId = id,
                Label = _detailService.BuildTitle(row),
                IzodScore = projection?.Izod?.Score,
                CharpyScore = projection?.Charpy?.Score
            };
        }).ToList();
        lines.Add(string.Empty);
        lines.Add(BuildPendulumIntelligenceEvidence(candidates));
    }

    private void AppendAiCollectionPendulumContext(List<string> lines, IReadOnlySet<string> materialIds) =>
        AppendAiPendulumContext(lines, GetCanonicalActiveMaterialRows()
            .Where(row => materialIds.Contains(GetCell(row, "Material ID", "MaterialID").Trim())).ToList());

    private static bool VerifyPendulumIntelligenceContract()
    {
        var izod = new VideoPlannerRow { MaterialId = "I", Label = "Izod specialist", IzodScore = 90, CharpyScore = 10 };
        var charpy = new VideoPlannerRow { MaterialId = "C", Label = "Charpy specialist", IzodScore = 20, CharpyScore = 95 };
        var zero = new VideoPlannerRow { MaterialId = "Z", Label = "Measured zero", IzodScore = 0 };
        var missing = new VideoPlannerRow { MaterialId = "M", Label = "Missing" };
        var report = BuildPendulumIntelligenceEvidenceCore(new[] { izod, charpy, zero, missing }, row => row.MaterialId switch
        {
            "I" => (18d, 2d),
            "C" => (4d, 19d),
            "Z" => (0d, null),
            _ => (null, null)
        });
        var empty = BuildPendulumIntelligenceEvidenceCore(new[] { missing });
        var zeroOnly = BuildPendulumIntelligenceEvidenceCore(new[] { zero }, _ => (0d, null));
        var scoped = BuildPendulumIntelligenceEvidenceCore(new[] { charpy });
        return report.Contains("Izod: 3/4 materials with evidence; leader Izod specialist", StringComparison.Ordinal)
            && report.Contains("Charpy: 2/4 materials with evidence; leader Charpy specialist", StringComparison.Ordinal)
            && report.Contains("mean 18 kJ/m²", StringComparison.Ordinal)
            && report.Contains("mean 19 kJ/m²", StringComparison.Ordinal)
            && empty.Contains("Izod: no measured result", StringComparison.Ordinal)
            && empty.Contains("Charpy: no measured result", StringComparison.Ordinal)
            && zeroOnly.Contains("mean 0 kJ/m²; score 0/100", StringComparison.Ordinal)
            && !scoped.Contains("Izod specialist", StringComparison.Ordinal)
            && scoped.Contains("Izod: 1/1", StringComparison.Ordinal)
            && scoped.Contains("Charpy: 1/1", StringComparison.Ordinal);
    }
}
