using FilamentDbApp.Models;
using FilamentDbApp.Services.Reporting;
using FilamentDbApp.Services.Website;
using System.Text.Json;

namespace FilamentDbApp.Services;

/// <summary>Pure synthetic regression for location statistics and independent-specimen publication.</summary>
public static class ShoreStatisticsVerification
{
    public static bool Verify()
    {
        const string materialId = "SHORE-VERIFICATION";
        var session = new FlexibleTestSessionRecord { FlexibleTestSessionId = "SESSION", MaterialID = materialId };
        FlexibleTestSpecimenRecord Specimen(string id) => new()
        {
            SpecimenId = id, FlexibleTestSessionId = session.FlexibleTestSessionId,
            SpecimenLabel = "PRIVATE-SHORE-LABEL-" + id, IntendedTest = "Shore",
            Shape = "Square", DiameterMm = "", InitialHeightMm = "8", ThicknessMm = "8",
            MethodVersion = "SHORE-v1", MethodNotes = "PRIVATE-SHORE-METHOD"
        };
        var specimens = new[] { Specimen("PRIVATE-SHORE-ONE"), Specimen("PRIVATE-SHORE-TWO") };
        var readings = new List<ShoreHardnessReadingRecord>();
        void Add(string specimen, string value, string scale = "A", string time = "10") => readings.Add(new()
        {
            ShoreReadingId = "PRIVATE-SHORE-READING-" + readings.Count, SpecimenId = specimen,
            ShoreScale = scale, HardnessValue = value, ReadingTimeSeconds = time,
            SpecimenThicknessMm = "8", Notes = "PRIVATE-SHORE-POINT-NOTE"
        });
        FlexibleMaterialEvidenceSnapshot Evidence() => FlexibleMaterialEvidenceService.Build(
            materialId, [session], specimens, [], [], [], readings);
        static bool Near(double? actual, double expected) => actual is double value && Math.Abs(value - expected) < 1e-9;
        foreach (var value in new[] { "70", "72", "74", "76", "78", "", "invalid", "101" })
            Add(specimens[0].SpecimenId, value);
        var first = Evidence().Groups.Single();
        var locations = first.ShoreSpecimens.Single();
        if (first.SpecimenCount != 1 || !Near(first.Mean, 74) || first.StandardDeviation is not null ||
            first.CoefficientOfVariation is not null || locations.ReadingCount != 5 ||
            !Near(locations.Mean, 74) || !Near(locations.StandardDeviation, Math.Sqrt(10)) ||
            !Near(locations.CoefficientOfVariation, Math.Sqrt(10) / 74 * 100)) return false;

        Add(specimens[1].SpecimenId, "80");
        Add(specimens[1].SpecimenId, "84");
        Add(specimens[0].SpecimenId, "40", "D");
        Add(specimens[0].SpecimenId, "60", time: "30");
        Add(specimens[0].SpecimenId, "0", time: "20");
        Add(specimens[0].SpecimenId, "0", time: "20");
        var evidence = Evidence();
        if (evidence.Groups.Count != 4) return false;
        var combined = evidence.Groups.Single(g => g.MetricKind == FlexibleMetricKind.ShoreA &&
            g.Condition.StartsWith("10 s reading", StringComparison.Ordinal));
        var zero = evidence.Groups.Single(g => g.Condition.StartsWith("20 s reading", StringComparison.Ordinal));
        var one = evidence.Groups.Single(g => g.MetricKind == FlexibleMetricKind.ShoreD);
        if (combined.SpecimenCount != 2 || !Near(combined.Mean, 78) ||
            !Near(combined.StandardDeviation, Math.Sqrt(32)) ||
            !Near(combined.CoefficientOfVariation, Math.Sqrt(32) / 78 * 100) ||
            combined.ShoreSpecimens.Count != 2 || !Near(combined.ShoreSpecimens[1].Mean, 82) ||
            combined.ShoreSpecimens[1].ReadingCount != 2 ||
            !Near(combined.ShoreSpecimens[1].StandardDeviation, Math.Sqrt(8)) ||
            !Near(zero.ShoreSpecimens.Single().StandardDeviation, 0) ||
            zero.ShoreSpecimens.Single().CoefficientOfVariation is not null ||
            one.ShoreSpecimens.Single().StandardDeviation is not null ||
            one.ShoreSpecimens.Single().CoefficientOfVariation is not null) return false;

        var published = FlexibleReportEvidenceService.Build(evidence);
        var publicCombined = published.Single(g => g.Unit == "Shore A" &&
            g.Condition.StartsWith("10 s reading", StringComparison.Ordinal));
        var website = new FlexibleWebsiteService().BuildRows([new(materialId,
            new Dictionary<string, object?> { ["label"] = "Synthetic Shore material" }, evidence)]);
        var websiteCombined = website.Single(g => g.GroupId == publicCombined.GroupId);
        var html = FlexibleReportEvidenceService.RenderHtml(published);
        var text = FlexibleReportEvidenceService.RenderText(published);
        var publicJson = JsonSerializer.Serialize(published);
        var websiteJson = JsonSerializer.Serialize(website);
        using var shape = JsonDocument.Parse(JsonSerializer.Serialize(publicCombined.ShoreSpecimens[0]));
        var expected = new[] { "SpecimenNumber", "ReadingCount", "Mean", "StandardDeviation", "CoefficientOfVariation" };
        return publicCombined.ShoreSpecimens.Count == 2 && publicCombined.ShoreSpecimens[0].SpecimenNumber == 1 &&
            publicCombined.ShoreSpecimens[0].ReadingCount == 5 &&
            Near(publicCombined.ShoreSpecimens[0].StandardDeviation, Math.Sqrt(10)) &&
            websiteCombined.ShoreSpecimens.SequenceEqual(publicCombined.ShoreSpecimens) &&
            shape.RootElement.EnumerateObject().Select(p => p.Name).OrderBy(n => n, StringComparer.Ordinal)
                .SequenceEqual(expected.OrderBy(n => n, StringComparer.Ordinal)) &&
            new[] { html, text }.All(output => output.Contains("Location SD", StringComparison.Ordinal) &&
                output.Contains("Location CV", StringComparison.Ordinal)) &&
            new[] { html, text, publicJson, websiteJson }.All(output =>
                !output.Contains("PRIVATE-SHORE", StringComparison.Ordinal));
    }
}
