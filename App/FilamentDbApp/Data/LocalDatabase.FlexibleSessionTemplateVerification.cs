using FilamentDbApp.Models;
using FilamentDbApp.Services;
using Microsoft.Data.Sqlite;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    public static bool RunFlexibleSessionTemplatePersistenceVerification()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var schema = connection.CreateCommand();
        schema.CommandText = """
CREATE TABLE NativeMaterialManagerRows(MaterialID TEXT PRIMARY KEY);
INSERT INTO NativeMaterialManagerRows VALUES ('MAT-TEMPLATE');
CREATE TABLE MaterialExperiments(MaterialExperimentId TEXT PRIMARY KEY,MaterialID TEXT NOT NULL);
CREATE TABLE ExperimentalRuns(ExperimentalRunId TEXT PRIMARY KEY,MaterialExperimentId TEXT NOT NULL,MeasuredDate TEXT,IsActive INTEGER,CreatedAtUtc TEXT,UpdatedAtUtc TEXT);
""";
        schema.ExecuteNonQuery();
        var settings = FlexibleSessionTemplateService.Defaults.ToDictionary(x => x.Name, x => x.Value);
        settings["Default specimen diameter"] = "9"; settings["Default specimen height"] = "10";
        settings["Default specimen thickness"] = "9"; settings["Default compression displacement"] = "2";
        settings["Default recovery compressed hold"] = "30";
        var templates = Enumerable.Range(1, 10).Select(i => FlexibleSessionTemplateService.CreateRows("FTS-TEMPLATE", $"Specimen {i}", false, settings)).ToList();
        templates.Add(FlexibleSessionTemplateService.CreateRows("FTS-TEMPLATE", "Shore Specimen 1", true, settings));
        var specimens = templates.Select(x => x.Specimen).ToList();
        var compression = templates.SelectMany(x => x.Compression).ToList();
        var recovery = templates.SelectMany(x => x.Recovery).ToList();
        var shore = templates.SelectMany(x => x.Shore).ToList();
        List<FlexibleTestSessionRecord> sessions = [new() { FlexibleTestSessionId = "FTS-TEMPLATE", MaterialID = "MAT-TEMPLATE", SessionLabel = "Template" }];
        void Save() => SynchronizeFlexibleTestingGraph(connection, sessions, specimens, compression, [], recovery, shore);
        Save();
        if (ReadSpecimens(connection).Count != 11 || ReadCompression(connection).Count != 20 ||
            ReadRecovery(connection).Count != 10 || ReadShore(connection).Count != 5 ||
            FlexibleMaterialTestingService.BuildComparisons(ReadSpecimens(connection), ReadCompression(connection),
                ReadShore(connection), ReadRecovery(connection)).Count != 0) return false;
        compression[0].ForceN = "100"; recovery[0].HeightAfterRestMm = "9.8"; shore[0].HardnessValue = "82";
        specimens[0].PrintTemperatureC = "225"; specimens[0].ExtrusionMultiplier = "0.95"; specimens[0].TopLayers = "3";
        Save();
        using var failure = connection.CreateCommand();
        failure.CommandText = "CREATE TRIGGER fail_template_update BEFORE UPDATE ON CompressionMeasurementPoints WHEN NEW.ForceN='BLOCK' BEGIN SELECT RAISE(ABORT,'injected failure'); END;";
        failure.ExecuteNonQuery();
        foreach (var specimen in specimens) { specimen.Perimeters = "2"; specimen.TopLayers = "5"; specimen.BottomLayers = "3"; }
        compression[0].ForceN = "BLOCK";
        try { Save(); return false; } catch (SqliteException) { }
        if (ReadSpecimens(connection).Single(x => x.SpecimenId == specimens[0].SpecimenId).TopLayers != "3" ||
            ReadCompression(connection).Single(x => x.CompressionPointId == compression[0].CompressionPointId).ForceN != "100") return false;
        compression[0].ForceN = "100"; Save();
        var saved = ReadSpecimens(connection).Single(x => x.SpecimenId == specimens[0].SpecimenId);
        return saved.TopLayers == "5" && saved.Perimeters == "2" && saved.BottomLayers == "3" &&
            saved.PrintTemperatureC == "225" && saved.ExtrusionMultiplier == "0.95" &&
            ReadCompression(connection).Single(x => x.CompressionPointId == compression[0].CompressionPointId).ForceN == "100" &&
            ReadRecovery(connection).Single(x => x.RecoveryMeasurementId == recovery[0].RecoveryMeasurementId).HeightAfterRestMm == "9.8" &&
            ReadShore(connection).Single(x => x.ShoreReadingId == shore[0].ShoreReadingId).HardnessValue == "82";
    }
}
