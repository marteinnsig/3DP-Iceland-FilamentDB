using FilamentDbApp.Models;
using System.Globalization;

namespace FilamentDbApp.Services;

public static class FlexibleSessionTemplateService
{
    public static readonly (string Name, string Value, string Unit)[] Defaults =
    [
        ("Compression specimens per session", "10", "specimens"),
        ("Default print temperature", "230", "°C"),
        ("Default extrusion multiplier", "1.1", "ratio"),
        ("Default perimeters", "2", "walls"),
        ("Default top layers", "5", "layers"),
        ("Default bottom layers", "3", "layers"),
        ("Default Shore reading time", "10", "s"),
        ("Default Shore thickness", "8", "mm")
    ];

    public static bool ValidateSetting(string name, string value)
    {
        var number = FlexibleMaterialTestingService.ParseOptional(value);
        if (number is null) return false;
        return name switch
        {
            "Compression specimens per session" => number >= 1 && number <= 100 && number == Math.Truncate(number.Value),
            "Default perimeters" or "Default top layers" or "Default bottom layers" =>
                number >= 0 && number <= 100 && number == Math.Truncate(number.Value),
            "Default Shore reading time" => number >= 0,
            _ => number > 0
        };
    }

    public static FlexibleTestSpecimenRecord CreateSpecimen(string sessionId, string label, bool shore,
        IReadOnlyDictionary<string, string> settings)
    {
        return new FlexibleTestSpecimenRecord
        {
            SpecimenId = NewId("TPU"), FlexibleTestSessionId = sessionId, SpecimenLabel = label,
            IntendedTest = shore ? "Shore" : "Compression", Shape = shore ? "50 x 50 mm coupon" : "Cylinder",
            DiameterMm = shore ? "" : settings["Default specimen diameter"],
            InitialHeightMm = shore ? settings["Default Shore thickness"] : settings["Default specimen height"],
            ThicknessMm = shore ? settings["Default Shore thickness"] : settings["Default specimen thickness"],
            PrintTemperatureC = settings["Default print temperature"], ExtrusionMultiplier = settings["Default extrusion multiplier"],
            Perimeters = settings["Default perimeters"], TopLayers = settings["Default top layers"],
            BottomLayers = settings["Default bottom layers"],
            MethodVersion = shore ? "SHORE-v1" : FlexibleMaterialTestingService.CompressionMethodVersion,
            MethodNotes = shore ? "Comparative in-house Shore test; 50 x 50 mm coupon, 24 h conditioning; four corners 10 mm from both edges and center. Not ASTM/ISO." : FlexibleMaterialTestingService.CompressionMethodNotes,
            CreatedAtUtc = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture)
        };
    }

    public static ShoreHardnessReadingRecord CreateShore(string specimenId, IReadOnlyDictionary<string, string> settings,
        string location = "") => new()
    {
        ShoreReadingId = NewId("SHR"), SpecimenId = specimenId, ShoreScale = "A",
        ReadingTimeSeconds = settings["Default Shore reading time"],
        SpecimenThicknessMm = settings["Default Shore thickness"], Notes = location
    };

    public static string NewId(string prefix) => prefix + "-" + Guid.NewGuid().ToString("N");

    public sealed record TemplateRows(FlexibleTestSpecimenRecord Specimen, List<CompressionPointRecord> Compression,
        List<RecoveryMeasurementRecord> Recovery, List<ShoreHardnessReadingRecord> Shore);

    public static TemplateRows CreateRows(string sessionId, string label, bool shore, IReadOnlyDictionary<string, string> settings)
    {
        var specimen = CreateSpecimen(sessionId, label, shore, settings);
        var rows = new TemplateRows(specimen, [], [], []);
        if (shore)
            foreach (var location in new[] { "Corner 1", "Corner 2", "Corner 3", "Corner 4", "Center" })
                rows.Shore.Add(CreateShore(specimen.SpecimenId, settings, location));
        else
        {
            foreach (var hold in new[] { "10", "30" })
                rows.Compression.Add(new CompressionPointRecord { CompressionPointId = NewId("CMP"), SpecimenId = specimen.SpecimenId,
                    CycleNumber = 1, TargetStrainPercent = "20", DisplacementMm = settings["Default compression displacement"], HoldTimeSeconds = hold });
            rows.Recovery.Add(new RecoveryMeasurementRecord { RecoveryMeasurementId = NewId("RCV"), SpecimenId = specimen.SpecimenId,
                CycleNumber = 1, InitialHeightMm = specimen.InitialHeightMm, RestTimeSeconds = "60", CompressionPercent = "20",
                CompressionHoldSeconds = settings["Default recovery compressed hold"] });
        }
        return rows;
    }

    public static bool Verify()
    {
        var settings = Defaults.ToDictionary(x => x.Name, x => x.Value);
        settings["Default specimen diameter"] = "9"; settings["Default specimen height"] = "10";
        settings["Default specimen thickness"] = "9"; settings["Default compression displacement"] = "2";
        settings["Default recovery compressed hold"] = "30";
        var rows = Enumerable.Range(1, 10).Select(i => CreateRows("VERIFY", $"Specimen {i}", false, settings)).ToList();
        rows.Add(CreateRows("VERIFY", "Shore Specimen 1", true, settings));
        var specimens = rows.Select(x => x.Specimen).ToList();
        var compression = rows.SelectMany(x => x.Compression).ToList();
        var recovery = rows.SelectMany(x => x.Recovery).ToList();
        var shore = rows.SelectMany(x => x.Shore).ToList();
        if (specimens.Select(x => x.SpecimenId).Distinct().Count() != 11 || compression.Count != 20 || recovery.Count != 10 || shore.Count != 5 ||
            compression.Any(x => !x.TargetReached || x.ForceN != "") || recovery.Any(x => x.HeightAfterRestMm != "") ||
            shore.Any(x => x.HardnessValue != "" || x.ReadingTimeSeconds != "10" || x.SpecimenThicknessMm != "8") ||
            FlexibleMaterialTestingService.Validate(specimens, compression, [], recovery, shore).Count != 0 ||
            FlexibleMaterialTestingService.BuildComparisons(specimens, compression, shore, recovery).Count != 0) return false;
        for (var i = 0; i < shore.Count; i++) shore[i].HardnessValue = (80 + i).ToString(CultureInfo.InvariantCulture);
        var measured = FlexibleMaterialTestingService.BuildComparisons(specimens, compression, shore, recovery);
        settings["Default print temperature"] = "240"; settings["Default Shore thickness"] = "9";
        var changed = CreateRows("VERIFY", "Changed", true, settings);
        return measured.Count == 1 && measured[0].SpecimenCount == 1 &&
            specimens.All(x => x.PrintTemperatureC == "230" && x.TopLayers == "5") &&
            changed.Specimen.PrintTemperatureC == "240" && changed.Shore.All(x => x.SpecimenThicknessMm == "9") &&
            !ValidateSetting("Compression specimens per session", "1.5") && !ValidateSetting("Default top layers", "-1") &&
            !ValidateSetting("Default Shore thickness", "NaN") && ValidateSetting("Default extrusion multiplier", "1,1");
    }
}
