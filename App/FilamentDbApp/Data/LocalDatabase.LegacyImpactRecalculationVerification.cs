using System.Globalization;
using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Data.Sqlite;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    public static bool RunLegacyImpactRecalculationVerification(out string evidence, string? approvedSeedPath = null)
    {
        var folder = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-LegacyImpact-" + Guid.NewGuid().ToString("N"));
        IODirectory.CreateDirectory(folder);
        var path = IOPath.Combine(folder, "filamentdb.sqlite");
        try
        {
            string? seedHash = null;
            if (approvedSeedPath is not null)
            {
                if (!IOPath.GetFullPath(approvedSeedPath).Equals(@"C:\Seed-Database\filamentdb.sqlite", StringComparison.OrdinalIgnoreCase))
                    throw new InvalidOperationException("Only the governed canonical seed is accepted for this fixture.");
                using (var stream = IOFile.OpenRead(approvedSeedPath)) seedHash = Convert.ToHexString(SHA256.HashData(stream));
                IOFile.Copy(approvedSeedPath, path);
            }
            var database = new LocalDatabase(path);
            using var connection = new SqliteConnection(database.ConnectionString);
            connection.Open();
            void Execute(string sql) { using var command = connection.CreateCommand(); command.CommandText = sql; command.ExecuteNonQuery(); }
            string Scalar(string sql) { using var command = connection.CreateCommand(); command.CommandText = sql; return command.ExecuteScalar()?.ToString() ?? ""; }
            static void Require(bool condition, string reason) { if (!condition) throw new InvalidOperationException(reason); }
            var seedPreview = database.PreviewLegacyImpactRecalculation();
            Execute("""
                INSERT INTO NativeMaterialManagerRows(MaterialId,UpdatedAtUtc) VALUES('IMPACT-AUDIT-FIXTURE','fixture-time');
                INSERT INTO NativeImpactSamples VALUES('IMPACT-AUDIT-FIXTURE','Flat',1,'47,00','preserved-native-time');
                DELETE FROM NativeSettingsRows WHERE Section='Impact';
                INSERT INTO NativeSettingsRows VALUES('Impact','Available Joules','2.743860924','J','Impact','fixture','preserved-setting-time');
                INSERT INTO NativeSettingsRows VALUES('Impact','Net cross-section area','48.1603','mm²','Impact','fixture','preserved-setting-time');
                INSERT INTO NativeSettingsRows VALUES('Impact','Net cross-section area','0.0000481603','m²','Impact','fixture','preserved-setting-time');
                INSERT INTO NativeSettingsRows VALUES('Impact','No-sample rebound angle','105.411','degrees','Impact','fixture','preserved-setting-time');
                INSERT INTO NativeSettingsRows VALUES('Impact','Max possible impact','56.97350149396911','kJ/m²','Impact','fixture','preserved-setting-time');
                INSERT INTO ExperimentDefinitions(ExperimentDefinitionId,Name,ParameterKey) VALUES('IMPACT-AUDIT','Impact audit fixture','probe');
                INSERT INTO MaterialExperiments(MaterialExperimentId,MaterialId,ExperimentDefinitionId,CreatedAtUtc,UpdatedAtUtc)
                  VALUES('IMPACT-AUDIT','IMPACT-AUDIT-FIXTURE','IMPACT-AUDIT','created','series-time');
                INSERT INTO ExperimentalRuns(ExperimentalRunId,MaterialExperimentId,CreatedAtUtc,UpdatedAtUtc)
                  VALUES('IMPACT-AUDIT','IMPACT-AUDIT','created','run-time');
                INSERT INTO ExperimentalRuns(ExperimentalRunId,MaterialExperimentId,CreatedAtUtc,UpdatedAtUtc)
                  VALUES('IMPACT-INVALID','IMPACT-AUDIT','created','invalid-run-time');
                INSERT INTO ExperimentalMeasurements(ExperimentalMeasurementId,ExperimentalRunId,MeasurementType,Orientation,RawUnit,ResultUnit,
                  Sample1,Sample2,ResultAverage,ResultStdDev,ResultCv,ResultCount,ResultConfidence,Notes,UpdatedAtUtc)
                  VALUES('IMPACT-AUDIT','IMPACT-AUDIT','Impact','Flat','%','kJ/m²','47,00','50','999','999','999','9','9','preserved-notes','preserved-exp-time');
                INSERT INTO ExperimentalMeasurements(ExperimentalMeasurementId,ExperimentalRunId,MeasurementType,Orientation,RawUnit,ResultUnit,
                  Sample1,ResultAverage,Notes,UpdatedAtUtc)
                  VALUES('IMPACT-INVALID','IMPACT-INVALID','Impact','Upright','%','kJ/m²','invalid','777','invalid-notes','invalid-time');
                """);
            var preview = database.PreviewLegacyImpactRecalculation();
            var beforeRaw = Scalar("SELECT json_group_array(json_object('id',MaterialId,'o',Orientation,'n',SampleNumber,'r',RawValue,'t',UpdatedAtUtc)) FROM NativeImpactSamples;");
            var beforeExp = Scalar("SELECT json_group_array(json_object('id',ExperimentalMeasurementId,'s1',Sample1,'s2',Sample2,'notes',Notes,'t',UpdatedAtUtc)) FROM ExperimentalMeasurements;");
            Require(preview.Groups.Single(x => x.Id == "IMPACT-AUDIT").Ready, "Valid experimental row was not ready.");
            Require(!preview.Groups.Single(x => x.Id == "IMPACT-INVALID").Ready, "Invalid row must remain pending.");
            var projected = database.LoadExperimentalMeasurements().Single(x => x.ExperimentalMeasurementId == "IMPACT-AUDIT");
            projected.ResultAverage = "123"; projected.Notes = "user-note-edit";
            var protectedRow = database.PrepareExperimentalGraphForSave([projected]).Single();
            Require(protectedRow.ResultAverage == "999" && projected.ResultAverage == "123" && protectedRow.Notes == "user-note-edit",
                "Ordinary graph save must preserve stored historical cache without losing user note edits.");
            var backupCount = IODirectory.GetFiles(folder, "3DPIceland-Manual-*.bak").Length;
            projected.Sample1 = "48";
            Require(database.PrepareExperimentalGraphForSave([projected]).Single().Sample1 == "48" &&
                IODirectory.GetFiles(folder, "3DPIceland-Manual-*.bak").Length > backupCount,
                "Actual Impact raw edit must retain new values and create backup first.");
            Execute("UPDATE NativeImpactSamples SET RawValue='48' WHERE MaterialId='IMPACT-AUDIT-FIXTURE';");
            var rejected = false;
            try { database.ApplyLegacyImpactRecalculation(preview); } catch (InvalidOperationException) { rejected = true; }
            Require(rejected && Scalar("SELECT ResultAverage FROM ExperimentalMeasurements WHERE ExperimentalMeasurementId='IMPACT-AUDIT';") == "999", "Stale preview was not rejected before mutation.");
            Execute("UPDATE NativeImpactSamples SET RawValue='47,00' WHERE MaterialId='IMPACT-AUDIT-FIXTURE';");
            var applied = database.ApplyLegacyImpactRecalculation(database.PreviewLegacyImpactRecalculation());
            Require(applied.UpdatedExperimentalRows >= 1 && IOFile.Exists(applied.BackupPath) && IOFile.Exists(applied.AuditPath), "Backup/apply evidence missing.");
            Require(beforeRaw == Scalar("SELECT json_group_array(json_object('id',MaterialId,'o',Orientation,'n',SampleNumber,'r',RawValue,'t',UpdatedAtUtc)) FROM NativeImpactSamples;"), "Native raw data changed.");
            Require(beforeExp == Scalar("SELECT json_group_array(json_object('id',ExperimentalMeasurementId,'s1',Sample1,'s2',Sample2,'notes',Notes,'t',UpdatedAtUtc)) FROM ExperimentalMeasurements;"), "Experimental raw data/notes/timestamps changed.");
            var expected = applied.Groups.Single(x => x.Id == "IMPACT-AUDIT").After;
            Require(Scalar("SELECT ResultAverage FROM ExperimentalMeasurements WHERE ExperimentalMeasurementId='IMPACT-AUDIT';") == expected.Average!.Value.ToString("0.###", CultureInfo.InvariantCulture), "Derived result not persisted.");
            Require(Scalar("SELECT ResultAverage FROM ExperimentalMeasurements WHERE ExperimentalMeasurementId='IMPACT-INVALID';") == "777", "Invalid group overwritten.");
            var repeated = database.ApplyLegacyImpactRecalculation(database.PreviewLegacyImpactRecalculation());
            Require(repeated.UpdatedExperimentalRows == 0 && beforeRaw == Scalar("SELECT json_group_array(json_object('id',MaterialId,'o',Orientation,'n',SampleNumber,'r',RawValue,'t',UpdatedAtUtc)) FROM NativeImpactSamples;"),
                "Repeated apply was not idempotent.");
            using (var stream = IOFile.OpenRead(applied.BackupPath))
                Require(Convert.ToHexString(SHA256.HashData(stream)) == applied.BackupSha256, "Backup hash differs.");
            Execute("UPDATE NativeImpactSamples SET Orientation='Unknown' WHERE MaterialId='IMPACT-AUDIT-FIXTURE'; UPDATE ExperimentalMeasurements SET Orientation='Unknown' WHERE ExperimentalMeasurementId='IMPACT-AUDIT';");
            var unknownOrientation = database.PreviewLegacyImpactRecalculation();
            Require(unknownOrientation.Groups.Where(x => x.MaterialId == "IMPACT-AUDIT-FIXTURE").All(x => !x.Ready),
                "Unknown orientation was not held pending.");
            var unknownProjection = database.LoadExperimentalMeasurements().Single(x => x.ExperimentalMeasurementId == "IMPACT-AUDIT");
            var storedAverage = unknownProjection.ResultAverage;
            unknownProjection.Sample1 = "49"; unknownProjection.ResultAverage = "9999";
            Require(database.PrepareExperimentalGraphForSave([unknownProjection]).Single().ResultAverage == storedAverage,
                "Unknown-orientation raw edit overwrote derived cache.");
            Execute("UPDATE NativeImpactSamples SET Orientation='Flat' WHERE MaterialId='IMPACT-AUDIT-FIXTURE'; UPDATE ExperimentalMeasurements SET Orientation='Flat',RawUnit='unknown' WHERE ExperimentalMeasurementId='IMPACT-AUDIT';");
            Require(!database.PreviewLegacyImpactRecalculation().Groups.Single(x => x.Id == "IMPACT-AUDIT").Ready,
                "Unknown units were not held pending.");
            unknownProjection = database.LoadExperimentalMeasurements().Single(x => x.ExperimentalMeasurementId == "IMPACT-AUDIT");
            unknownProjection.Sample1 = "49"; unknownProjection.ResultAverage = "9999";
            Require(database.PrepareExperimentalGraphForSave([unknownProjection]).Single().ResultAverage == storedAverage,
                "Unknown-unit raw edit overwrote derived cache.");
            Execute("UPDATE ExperimentalMeasurements SET RawUnit='%' WHERE ExperimentalMeasurementId='IMPACT-AUDIT';");
            Execute("UPDATE NativeSettingsRows SET Value='3' WHERE Section='Impact' AND Parameter='Available Joules';");
            var changedCalibration = database.PreviewLegacyImpactRecalculation();
            Require(changedCalibration.Settings is null && changedCalibration.Groups.All(x => !x.Ready),
                "Future calibration incorrectly reused historical attestation.");
            Execute("UPDATE NativeSettingsRows SET Value='0' WHERE Section='Impact' AND Parameter='Available Joules';");
            var invalid = database.PreviewLegacyImpactRecalculation();
            Require(invalid.CalibrationIssues.Count > 0 && invalid.Groups.All(x => !x.Ready), "Invalid calibration permitted apply.");
            rejected = false;
            try { database.ApplyLegacyImpactRecalculation(invalid); } catch (InvalidOperationException) { rejected = true; }
            Require(rejected, "Invalid calibration was applied.");
            if (approvedSeedPath is not null)
            {
                using var stream = IOFile.OpenRead(approvedSeedPath);
                Require(Convert.ToHexString(SHA256.HashData(stream)) == seedHash, "Canonical seed was modified.");
            }
            evidence = JsonSerializer.Serialize(new { Status = "PASS", Folder = folder, SeedSha256 = seedHash,
                SeedNativeGroups = seedPreview.Groups.Count(x => x.Kind == "Native"), SeedPendingGroups = seedPreview.PendingGroups,
                SeedValidReadings = seedPreview.ValidReadingCount,
                SeedTrace = seedPreview.Groups.FirstOrDefault(x => x.Kind == "Native" && x.MaterialId == "MAT0001" && x.Orientation == "Flat")?
                    .Readings.FirstOrDefault(x => x.SampleNumber == 2), applied.BackupPath, applied.BackupSha256,
                applied.AuditPath, applied.UpdatedExperimentalRows, applied.PendingGroups,
                Checks = "backup/hash, raw/notes/time identity, stale preview rejection, invalid/future calibration rejection, unknown units/orientation pending and cache protection, derived persistence, idempotent repeated apply" }, ImpactAuditJson);
            IOFile.WriteAllText(IOPath.Combine(folder, "verification.json"), evidence);
            return true;
        }
        catch (Exception ex)
        {
            evidence = "FAIL " + folder + ": " + ex;
            IOFile.WriteAllText(IOPath.Combine(folder, "failure.txt"), evidence);
            return false;
        }
    }
}
