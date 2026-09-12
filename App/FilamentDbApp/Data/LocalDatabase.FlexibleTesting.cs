using FilamentDbApp.Models;
using Microsoft.Data.Sqlite;
using System.Globalization;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    private static void EnsureFlexibleTestingSchema(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = """
CREATE TABLE IF NOT EXISTS FlexibleTestSpecimens (
    SpecimenId TEXT PRIMARY KEY,
    ExperimentalRunId TEXT NOT NULL,
    SpecimenLabel TEXT NOT NULL,
    IntendedTest TEXT NOT NULL,
    Shape TEXT NOT NULL,
    DiameterMm TEXT, InitialHeightMm TEXT, ThicknessMm TEXT, MassG TEXT,
    InfillPercent TEXT, InfillPattern TEXT, NozzleDiameterMm TEXT, LayerHeightMm TEXT,
    Perimeters TEXT, TopLayers TEXT, BottomLayers TEXT, PrintTemperatureC TEXT,
    ExtrusionMultiplier TEXT, PrintSettings TEXT, MethodVersion TEXT, MethodNotes TEXT,
    CreatedAtUtc TEXT NOT NULL, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (ExperimentalRunId) REFERENCES ExperimentalRuns(ExperimentalRunId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_FlexibleTestSpecimens_RunId ON FlexibleTestSpecimens(ExperimentalRunId);

CREATE TABLE IF NOT EXISTS CompressionMeasurementPoints (
    CompressionPointId TEXT PRIMARY KEY,
    SpecimenId TEXT NOT NULL, CycleNumber INTEGER NOT NULL,
    TargetStrainPercent TEXT, TargetReached INTEGER NOT NULL DEFAULT 1,
    DisplacementMm TEXT, ForceN TEXT, HoldTimeSeconds TEXT, Notes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (SpecimenId) REFERENCES FlexibleTestSpecimens(SpecimenId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_CompressionPoints_SpecimenCycle ON CompressionMeasurementPoints(SpecimenId, CycleNumber);

CREATE TABLE IF NOT EXISTS StressRelaxationPoints (
    RelaxationPointId TEXT PRIMARY KEY,
    SpecimenId TEXT NOT NULL, CycleNumber INTEGER NOT NULL,
    CompressionPercent TEXT, ElapsedTimeSeconds TEXT, ForceN TEXT, Notes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (SpecimenId) REFERENCES FlexibleTestSpecimens(SpecimenId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_RelaxationPoints_SpecimenCycle ON StressRelaxationPoints(SpecimenId, CycleNumber);

CREATE TABLE IF NOT EXISTS RecoveryMeasurements (
    RecoveryMeasurementId TEXT PRIMARY KEY,
    SpecimenId TEXT NOT NULL, CycleNumber INTEGER NOT NULL,
    InitialHeightMm TEXT, HeightAfterRestMm TEXT, RestTimeSeconds TEXT,
    CompressionPercent TEXT, CompressionHoldSeconds TEXT, Notes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (SpecimenId) REFERENCES FlexibleTestSpecimens(SpecimenId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_RecoveryMeasurements_Specimen ON RecoveryMeasurements(SpecimenId);

CREATE TABLE IF NOT EXISTS ShoreHardnessReadings (
    ShoreReadingId TEXT PRIMARY KEY,
    SpecimenId TEXT NOT NULL, ShoreScale TEXT NOT NULL,
    HardnessValue TEXT, ReadingTimeSeconds TEXT, SpecimenThicknessMm TEXT,
    Notes TEXT, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (SpecimenId) REFERENCES FlexibleTestSpecimens(SpecimenId) ON DELETE CASCADE
);
CREATE INDEX IF NOT EXISTS IX_ShoreReadings_SpecimenScale ON ShoreHardnessReadings(SpecimenId, ShoreScale);
""";
        command.ExecuteNonQuery();
        EnsureStandaloneFlexibleTestingSessions(connection);
    }

    private static void EnsureStandaloneFlexibleTestingSessions(SqliteConnection connection)
    {
        using (var create = connection.CreateCommand())
        {
            create.CommandText = """
CREATE TABLE IF NOT EXISTS FlexibleTestSessions (
    FlexibleTestSessionId TEXT PRIMARY KEY,
    MaterialID TEXT NOT NULL,
    SessionLabel TEXT NOT NULL,
    MeasuredDate TEXT, Notes TEXT, LegacyExperimentalRunId TEXT,
    IsActive INTEGER NOT NULL DEFAULT 1,
    CreatedAtUtc TEXT NOT NULL, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (MaterialID) REFERENCES NativeMaterialManagerRows(MaterialID) ON DELETE RESTRICT
);
CREATE INDEX IF NOT EXISTS IX_FlexibleTestSessions_MaterialId ON FlexibleTestSessions(MaterialID);
""";
            create.ExecuteNonQuery();
        }
        if (!HasColumn(connection, "FlexibleTestSpecimens", "FlexibleTestSessionId"))
        {
            using (var foreignKeys = connection.CreateCommand()) { foreignKeys.CommandText = "PRAGMA foreign_keys=OFF;"; foreignKeys.ExecuteNonQuery(); }
            using var migrate = connection.CreateCommand();
            migrate.CommandText = """
INSERT OR IGNORE INTO FlexibleTestSessions
(FlexibleTestSessionId,MaterialID,SessionLabel,MeasuredDate,Notes,LegacyExperimentalRunId,IsActive,CreatedAtUtc,UpdatedAtUtc)
SELECT 'FTS-LEGACY-' || r.ExperimentalRunId, s.MaterialID,
       'Migrated Experimental Run ' || r.ExperimentalRunId, COALESCE(r.MeasuredDate,''),
       'Migrated losslessly from the v64.0.0 Experimental Testing host.', r.ExperimentalRunId, r.IsActive,
       COALESCE(r.CreatedAtUtc,CURRENT_TIMESTAMP), COALESCE(r.UpdatedAtUtc,CURRENT_TIMESTAMP)
FROM FlexibleTestSpecimens f
JOIN ExperimentalRuns r ON r.ExperimentalRunId=f.ExperimentalRunId
JOIN MaterialExperiments s ON s.MaterialExperimentId=r.MaterialExperimentId
GROUP BY r.ExperimentalRunId;
CREATE TABLE FlexibleTestSpecimens_v44 (
    SpecimenId TEXT PRIMARY KEY, FlexibleTestSessionId TEXT, ExperimentalRunId TEXT,
    SpecimenLabel TEXT NOT NULL, IntendedTest TEXT NOT NULL, Shape TEXT NOT NULL,
    DiameterMm TEXT, InitialHeightMm TEXT, ThicknessMm TEXT, MassG TEXT,
    InfillPercent TEXT, InfillPattern TEXT, NozzleDiameterMm TEXT, LayerHeightMm TEXT,
    Perimeters TEXT, TopLayers TEXT, BottomLayers TEXT, PrintTemperatureC TEXT,
    ExtrusionMultiplier TEXT, PrintSettings TEXT, MethodVersion TEXT, MethodNotes TEXT,
    CreatedAtUtc TEXT NOT NULL, UpdatedAtUtc TEXT NOT NULL,
    FOREIGN KEY (FlexibleTestSessionId) REFERENCES FlexibleTestSessions(FlexibleTestSessionId) ON DELETE CASCADE,
    FOREIGN KEY (ExperimentalRunId) REFERENCES ExperimentalRuns(ExperimentalRunId) ON DELETE SET NULL
);
INSERT INTO FlexibleTestSpecimens_v44
SELECT SpecimenId,'FTS-LEGACY-' || ExperimentalRunId,ExperimentalRunId,SpecimenLabel,IntendedTest,Shape,
       DiameterMm,InitialHeightMm,ThicknessMm,MassG,InfillPercent,InfillPattern,NozzleDiameterMm,LayerHeightMm,
       Perimeters,TopLayers,BottomLayers,PrintTemperatureC,ExtrusionMultiplier,PrintSettings,MethodVersion,MethodNotes,
       CreatedAtUtc,UpdatedAtUtc
FROM FlexibleTestSpecimens;
DROP TABLE FlexibleTestSpecimens;
ALTER TABLE FlexibleTestSpecimens_v44 RENAME TO FlexibleTestSpecimens;
CREATE INDEX IX_FlexibleTestSpecimens_SessionId ON FlexibleTestSpecimens(FlexibleTestSessionId);
CREATE INDEX IX_FlexibleTestSpecimens_RunId ON FlexibleTestSpecimens(ExperimentalRunId);
""";
            migrate.ExecuteNonQuery();
            using var enable = connection.CreateCommand(); enable.CommandText = "PRAGMA foreign_keys=ON;"; enable.ExecuteNonQuery();
        }
        using var backfill = connection.CreateCommand();
        backfill.CommandText = """
INSERT OR IGNORE INTO FlexibleTestSessions
(FlexibleTestSessionId,MaterialID,SessionLabel,MeasuredDate,Notes,LegacyExperimentalRunId,IsActive,CreatedAtUtc,UpdatedAtUtc)
SELECT 'FTS-LEGACY-' || r.ExperimentalRunId,s.MaterialID,'Migrated Experimental Run ' || r.ExperimentalRunId,
       COALESCE(r.MeasuredDate,''),'Migrated losslessly from the v64.0.0 Experimental Testing host.',r.ExperimentalRunId,r.IsActive,
       COALESCE(r.CreatedAtUtc,CURRENT_TIMESTAMP),COALESCE(r.UpdatedAtUtc,CURRENT_TIMESTAMP)
FROM FlexibleTestSpecimens f JOIN ExperimentalRuns r ON r.ExperimentalRunId=f.ExperimentalRunId
JOIN MaterialExperiments s ON s.MaterialExperimentId=r.MaterialExperimentId
WHERE COALESCE(f.FlexibleTestSessionId,'')='' GROUP BY r.ExperimentalRunId;
UPDATE FlexibleTestSpecimens SET FlexibleTestSessionId='FTS-LEGACY-' || ExperimentalRunId
WHERE COALESCE(FlexibleTestSessionId,'')='' AND COALESCE(ExperimentalRunId,'')<>'';
""";
        backfill.ExecuteNonQuery();
    }

    private static bool HasColumn(SqliteConnection connection, string table, string column)
    {
        using var command = connection.CreateCommand(); command.CommandText = $"PRAGMA table_info({table});";
        using var reader = command.ExecuteReader();
        while (reader.Read()) if (string.Equals(reader.GetString(1), column, StringComparison.OrdinalIgnoreCase)) return true;
        return false;
    }

    public (List<FlexibleTestSessionRecord> Sessions, List<FlexibleTestSpecimenRecord> Specimens, List<CompressionPointRecord> Compression,
        List<StressRelaxationPointRecord> Relaxation, List<RecoveryMeasurementRecord> Recovery,
        List<ShoreHardnessReadingRecord> Shore) LoadFlexibleTestingGraph()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        Initialize();
        EnsureFlexibleTestingSchema(connection);
        var sessions = ReadSessions(connection);
        var specimens = ReadSpecimens(connection);
        var compression = ReadCompression(connection);
        var relaxation = ReadRelaxation(connection);
        var recovery = ReadRecovery(connection);
        var shore = ReadShore(connection);
        return (sessions, specimens, compression, relaxation, recovery, shore);
    }

    public void SynchronizeFlexibleTestingGraph(
        IReadOnlyCollection<FlexibleTestSessionRecord> sessions,
        IReadOnlyCollection<FlexibleTestSpecimenRecord> specimens,
        IReadOnlyCollection<CompressionPointRecord> compression,
        IReadOnlyCollection<StressRelaxationPointRecord> relaxation,
        IReadOnlyCollection<RecoveryMeasurementRecord> recovery,
        IReadOnlyCollection<ShoreHardnessReadingRecord> shore)
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        EnsureFlexibleTestingSchema(connection);
        using var transaction = connection.BeginTransaction();
        var now = DateTime.UtcNow.ToString("O", CultureInfo.InvariantCulture);

        foreach (var row in sessions)
        {
            if (string.IsNullOrWhiteSpace(row.CreatedAtUtc)) row.CreatedAtUtc = now;
            row.UpdatedAtUtc = now;
            Upsert(connection, transaction, """
INSERT INTO FlexibleTestSessions
(FlexibleTestSessionId,MaterialID,SessionLabel,MeasuredDate,Notes,LegacyExperimentalRunId,IsActive,CreatedAtUtc,UpdatedAtUtc)
VALUES ($id,$material,$label,$date,$notes,$legacy,$active,$created,$updated)
ON CONFLICT(FlexibleTestSessionId) DO UPDATE SET MaterialID=excluded.MaterialID,SessionLabel=excluded.SessionLabel,
MeasuredDate=excluded.MeasuredDate,Notes=excluded.Notes,LegacyExperimentalRunId=excluded.LegacyExperimentalRunId,
IsActive=excluded.IsActive,UpdatedAtUtc=excluded.UpdatedAtUtc;
""",("$id",row.FlexibleTestSessionId),("$material",row.MaterialID),("$label",row.SessionLabel),("$date",row.MeasuredDate),
                ("$notes",row.Notes),("$legacy",row.LegacyExperimentalRunId),("$active",row.IsActive?1:0),("$created",row.CreatedAtUtc),("$updated",now));
        }

        foreach (var row in specimens)
        {
            if (string.IsNullOrWhiteSpace(row.CreatedAtUtc)) row.CreatedAtUtc = now;
            row.UpdatedAtUtc = now;
            Upsert(connection, transaction, """
INSERT INTO FlexibleTestSpecimens
(SpecimenId,FlexibleTestSessionId,ExperimentalRunId,SpecimenLabel,IntendedTest,Shape,DiameterMm,InitialHeightMm,ThicknessMm,MassG,InfillPercent,InfillPattern,NozzleDiameterMm,LayerHeightMm,Perimeters,TopLayers,BottomLayers,PrintTemperatureC,ExtrusionMultiplier,PrintSettings,MethodVersion,MethodNotes,CreatedAtUtc,UpdatedAtUtc)
VALUES ($id,$session,$run,$label,$test,$shape,$diameter,$height,$thickness,$mass,$infill,$pattern,$nozzle,$layer,$walls,$top,$bottom,$temperature,$multiplier,$settings,$version,$notes,$created,$updated)
ON CONFLICT(SpecimenId) DO UPDATE SET FlexibleTestSessionId=excluded.FlexibleTestSessionId,ExperimentalRunId=excluded.ExperimentalRunId,SpecimenLabel=excluded.SpecimenLabel,IntendedTest=excluded.IntendedTest,Shape=excluded.Shape,DiameterMm=excluded.DiameterMm,InitialHeightMm=excluded.InitialHeightMm,ThicknessMm=excluded.ThicknessMm,MassG=excluded.MassG,InfillPercent=excluded.InfillPercent,InfillPattern=excluded.InfillPattern,NozzleDiameterMm=excluded.NozzleDiameterMm,LayerHeightMm=excluded.LayerHeightMm,Perimeters=excluded.Perimeters,TopLayers=excluded.TopLayers,BottomLayers=excluded.BottomLayers,PrintTemperatureC=excluded.PrintTemperatureC,ExtrusionMultiplier=excluded.ExtrusionMultiplier,PrintSettings=excluded.PrintSettings,MethodVersion=excluded.MethodVersion,MethodNotes=excluded.MethodNotes,UpdatedAtUtc=excluded.UpdatedAtUtc;
""", ("$id",row.SpecimenId),("$session",row.FlexibleTestSessionId),("$run",OptionalForeignKey(row.ExperimentalRunId)),("$label",row.SpecimenLabel),("$test",row.IntendedTest),("$shape",row.Shape),("$diameter",row.DiameterMm),("$height",row.InitialHeightMm),("$thickness",row.ThicknessMm),("$mass",row.MassG),("$infill",row.InfillPercent),("$pattern",row.InfillPattern),("$nozzle",row.NozzleDiameterMm),("$layer",row.LayerHeightMm),("$walls",row.Perimeters),("$top",row.TopLayers),("$bottom",row.BottomLayers),("$temperature",row.PrintTemperatureC),("$multiplier",row.ExtrusionMultiplier),("$settings",row.PrintSettings),("$version",row.MethodVersion),("$notes",row.MethodNotes),("$created",row.CreatedAtUtc),("$updated",now));
        }
        foreach (var row in compression)
        {
            row.UpdatedAtUtc = now;
            Upsert(connection, transaction, "INSERT INTO CompressionMeasurementPoints VALUES ($id,$specimen,$cycle,$target,$reached,$displacement,$force,$hold,$notes,$updated) ON CONFLICT(CompressionPointId) DO UPDATE SET SpecimenId=excluded.SpecimenId,CycleNumber=excluded.CycleNumber,TargetStrainPercent=excluded.TargetStrainPercent,TargetReached=excluded.TargetReached,DisplacementMm=excluded.DisplacementMm,ForceN=excluded.ForceN,HoldTimeSeconds=excluded.HoldTimeSeconds,Notes=excluded.Notes,UpdatedAtUtc=excluded.UpdatedAtUtc;", ("$id",row.CompressionPointId),("$specimen",row.SpecimenId),("$cycle",row.CycleNumber),("$target",row.TargetStrainPercent),("$reached",row.TargetReached?1:0),("$displacement",row.DisplacementMm),("$force",row.ForceN),("$hold",row.HoldTimeSeconds),("$notes",row.Notes),("$updated",now));
        }
        foreach (var row in relaxation)
        {
            row.UpdatedAtUtc = now;
            Upsert(connection, transaction, "INSERT INTO StressRelaxationPoints VALUES ($id,$specimen,$cycle,$compression,$time,$force,$notes,$updated) ON CONFLICT(RelaxationPointId) DO UPDATE SET SpecimenId=excluded.SpecimenId,CycleNumber=excluded.CycleNumber,CompressionPercent=excluded.CompressionPercent,ElapsedTimeSeconds=excluded.ElapsedTimeSeconds,ForceN=excluded.ForceN,Notes=excluded.Notes,UpdatedAtUtc=excluded.UpdatedAtUtc;", ("$id",row.RelaxationPointId),("$specimen",row.SpecimenId),("$cycle",row.CycleNumber),("$compression",row.CompressionPercent),("$time",row.ElapsedTimeSeconds),("$force",row.ForceN),("$notes",row.Notes),("$updated",now));
        }
        foreach (var row in recovery)
        {
            row.UpdatedAtUtc = now;
            Upsert(connection, transaction, "INSERT INTO RecoveryMeasurements VALUES ($id,$specimen,$cycle,$initial,$after,$rest,$compression,$hold,$notes,$updated) ON CONFLICT(RecoveryMeasurementId) DO UPDATE SET SpecimenId=excluded.SpecimenId,CycleNumber=excluded.CycleNumber,InitialHeightMm=excluded.InitialHeightMm,HeightAfterRestMm=excluded.HeightAfterRestMm,RestTimeSeconds=excluded.RestTimeSeconds,CompressionPercent=excluded.CompressionPercent,CompressionHoldSeconds=excluded.CompressionHoldSeconds,Notes=excluded.Notes,UpdatedAtUtc=excluded.UpdatedAtUtc;", ("$id",row.RecoveryMeasurementId),("$specimen",row.SpecimenId),("$cycle",row.CycleNumber),("$initial",row.InitialHeightMm),("$after",row.HeightAfterRestMm),("$rest",row.RestTimeSeconds),("$compression",row.CompressionPercent),("$hold",row.CompressionHoldSeconds),("$notes",row.Notes),("$updated",now));
        }
        foreach (var row in shore)
        {
            row.UpdatedAtUtc = now;
            Upsert(connection, transaction, "INSERT INTO ShoreHardnessReadings VALUES ($id,$specimen,$scale,$value,$time,$thickness,$notes,$updated) ON CONFLICT(ShoreReadingId) DO UPDATE SET SpecimenId=excluded.SpecimenId,ShoreScale=excluded.ShoreScale,HardnessValue=excluded.HardnessValue,ReadingTimeSeconds=excluded.ReadingTimeSeconds,SpecimenThicknessMm=excluded.SpecimenThicknessMm,Notes=excluded.Notes,UpdatedAtUtc=excluded.UpdatedAtUtc;", ("$id",row.ShoreReadingId),("$specimen",row.SpecimenId),("$scale",row.ShoreScale),("$value",row.HardnessValue),("$time",row.ReadingTimeSeconds),("$thickness",row.SpecimenThicknessMm),("$notes",row.Notes),("$updated",now));
        }

        DeleteMissing(connection, transaction, "CompressionMeasurementPoints", "CompressionPointId", compression.Select(x => x.CompressionPointId));
        DeleteMissing(connection, transaction, "StressRelaxationPoints", "RelaxationPointId", relaxation.Select(x => x.RelaxationPointId));
        DeleteMissing(connection, transaction, "RecoveryMeasurements", "RecoveryMeasurementId", recovery.Select(x => x.RecoveryMeasurementId));
        DeleteMissing(connection, transaction, "ShoreHardnessReadings", "ShoreReadingId", shore.Select(x => x.ShoreReadingId));
        DeleteMissing(connection, transaction, "FlexibleTestSpecimens", "SpecimenId", specimens.Select(x => x.SpecimenId));
        DeleteMissing(connection, transaction, "FlexibleTestSessions", "FlexibleTestSessionId", sessions.Select(x => x.FlexibleTestSessionId));
        using var foreignKeys = connection.CreateCommand();
        foreignKeys.Transaction = transaction;
        foreignKeys.CommandText = "PRAGMA foreign_key_check;";
        using var reader = foreignKeys.ExecuteReader();
        if (reader.Read()) throw new InvalidOperationException("Flexible-material test save failed foreign-key validation.");
        transaction.Commit();
    }

    public static bool RunFlexibleTestingCalculationContractVerification()
    {
        var strain = Services.FlexibleMaterialTestingService.CalculateStrainPercent("1", "10");
        var stress = Services.FlexibleMaterialTestingService.CalculateApparentStressMpa("100", "20");
        var retention = Services.FlexibleMaterialTestingService.CalculateForceRetentionPercent("60", "100");
        var recovery = Services.FlexibleMaterialTestingService.CalculateResidualHeightLossPercent("10", "9.5");
        var specimens = new[]
        {
            new FlexibleTestSpecimenRecord { SpecimenId="S1",FlexibleTestSessionId="FTS-1",Shape="Cylinder",DiameterMm="20",InitialHeightMm="10",InfillPercent="30",InfillPattern="Gyroid" },
            new FlexibleTestSpecimenRecord { SpecimenId="S2",FlexibleTestSessionId="FTS-1",Shape="Cylinder",DiameterMm="20,0",InitialHeightMm="10,0",InfillPercent="30,0",InfillPattern="Gyroid" },
            new FlexibleTestSpecimenRecord { SpecimenId="S3",FlexibleTestSessionId="FTS-1",Shape="Cylinder",DiameterMm="20",InitialHeightMm="10",InfillPercent="100",InfillPattern="Rectilinear" }
        };
        var points = new[]
        {
            new CompressionPointRecord { SpecimenId="S1",CycleNumber=1,TargetStrainPercent="25",TargetReached=true,ForceN="100",HoldTimeSeconds="10",ApparentStressMpa="0.318309886" },
            new CompressionPointRecord { SpecimenId="S1",CycleNumber=1,TargetStrainPercent="25",TargetReached=true,ForceN="102",HoldTimeSeconds="10",ApparentStressMpa="0.324676" },
            new CompressionPointRecord { SpecimenId="S2",CycleNumber=1,TargetStrainPercent="25,0",TargetReached=true,ForceN="110",HoldTimeSeconds="10,0",ApparentStressMpa="0.350141" },
            new CompressionPointRecord { SpecimenId="S3",CycleNumber=1,TargetStrainPercent="25",TargetReached=false,HoldTimeSeconds="10" }
        };
        var comparisons = Services.FlexibleMaterialTestingService.BuildComparisons(specimens, points,
        [
            new ShoreHardnessReadingRecord { SpecimenId="S1",ShoreScale="A",HardnessValue="90",ReadingTimeSeconds="1",SpecimenThicknessMm="10" },
            new ShoreHardnessReadingRecord { SpecimenId="S1",ShoreScale="D",HardnessValue="35",ReadingTimeSeconds="1",SpecimenThicknessMm="10" }
        ]);
        var forceComparison = comparisons.Single(x => x.Metric == "Compression Force at 25% Strain" && x.SpecimenCount == 2);
        var compressionRetentionReady = VerifyCompressionRetention();
        return RunFlexibleMaterialEvidenceContractVerification() && compressionRetentionReady && VerifyCompressionRetentionComparisons() && VerifyRecoveryTvlInput() &&
               Math.Abs(strain.GetValueOrDefault() - 10d) < 0.000001d &&
               Math.Abs(stress.GetValueOrDefault() - 0.318309886d) < 0.000001d &&
               Math.Abs(retention.GetValueOrDefault() - 60d) < 0.000001d &&
               Math.Abs(recovery.GetValueOrDefault() - 5d) < 0.000001d &&
               string.Equals(forceComparison.Mean, 105.5d.ToString("0.###", CultureInfo.CurrentCulture), StringComparison.Ordinal) &&
               comparisons.Count(x => x.Metric.StartsWith("Shore ", StringComparison.Ordinal)) == 2 &&
               comparisons.All(x => x.SpecimenCount <= 2) &&
               Services.FlexibleMaterialTestingService.Validate(
                   [new FlexibleTestSpecimenRecord { SpecimenId="STANDALONE",FlexibleTestSessionId="FTS-STANDALONE" }],
                   [], [], [], []).Count == 0 &&
               OptionalForeignKey(string.Empty) is DBNull &&
               string.Equals(OptionalForeignKey(" RUN-1 ").ToString(), "RUN-1", StringComparison.Ordinal) &&
               new FlexibleTestSpecimenRecord().DiameterMm == "9" &&
               new FlexibleTestSpecimenRecord().InitialHeightMm == "10" &&
               new FlexibleTestSpecimenRecord().ThicknessMm == "9" &&
               Services.FlexibleMaterialTestingService.CalculateStrainPercent("1", "") is null &&
               Services.FlexibleMaterialTestingService.CalculateApparentStressMpa("100", "0") is null;

        bool VerifyCompressionRetentionComparisons()
        {
            var comparisonSpecimens = Enumerable.Range(1, 6).Select(index => new FlexibleTestSpecimenRecord
            {
                SpecimenId = $"RET-{index}", FlexibleTestSessionId = "RET-SESSION",
                MethodVersion = index == 4 ? "different-method" : "retention-verification"
            }).ToArray();
            CompressionPointRecord Reading(int specimen, string force, string time, string displacement = "2",
                int cycle = 1, string target = "20") => new()
            {
                SpecimenId = $"RET-{specimen}", CycleNumber = cycle, TargetStrainPercent = target,
                TargetReached = true, DisplacementMm = displacement, HoldTimeSeconds = time, ForceN = force,
                // Comparable Results must derive from raw readings, even before Recalculate runs.
                ForceRetentionPercent = "stale"
            };
            static bool Close(string actual, double expected) =>
                Services.FlexibleMaterialTestingService.ParseOptional(actual) is double value &&
                Math.Abs(value - expected) <= 0.001d;
            var readings = new List<CompressionPointRecord>
            {
                Reading(1, "100", "10"), Reading(1, "80", "30"),
                Reading(2, "200", "10,0"), Reading(2, "180", "30,0", "2,0")
            };
            var baseline = Services.FlexibleMaterialTestingService.BuildComparisons(comparisonSpecimens, readings, [])
                .Single(row => row.Metric == "Force retention at 20% Strain");
            if (baseline.Condition != "10–30 s · 2 mm displacement · cycle 1" || baseline.Unit != "%" ||
                baseline.SpecimenCount != 2 || !Close(baseline.Mean, 85) ||
                !Close(baseline.StandardDeviation, 7.071067812) || !Close(baseline.CoefficientOfVariation, 8.318903308) ||
                !Close(baseline.Minimum, 80) || !Close(baseline.Maximum, 90)) return false;

            // Repeated later readings contribute one specimen mean, not extra independent samples.
            readings.Add(Reading(1, "60", "30"));
            var repeated = Services.FlexibleMaterialTestingService.BuildComparisons(comparisonSpecimens, readings, [])
                .Single(row => row.Metric == "Force retention at 20% Strain");
            if (repeated.SpecimenCount != 2 || !Close(repeated.Mean, 80) ||
                !Close(repeated.StandardDeviation, 14.142135624) || !Close(repeated.Minimum, 70) ||
                !Close(repeated.Maximum, 90)) return false;

            readings.AddRange(
            [
                Reading(3, "100", "5"), Reading(3, "50", "30"),
                Reading(1, "40", "60"),
                Reading(1, "100", "10", "3"), Reading(1, "60", "30", "3"),
                Reading(1, "100", "10", cycle: 2), Reading(1, "65", "30", cycle: 2),
                Reading(4, "100", "10"), Reading(4, "55", "30"),
                Reading(1, "100", "10", target: "30"), Reading(1, "45", "30", target: "30"),
                // A zero reference and duplicate earliest times must never enter a summary.
                Reading(5, "0", "10"), Reading(5, "50", "30"),
                Reading(6, "100", "10"), Reading(6, "100", "10,0"), Reading(6, "50", "30")
            ]);
            var summaries = Services.FlexibleMaterialTestingService.BuildComparisons(comparisonSpecimens, readings, [])
                .Where(row => row.Metric.StartsWith("Force retention at ", StringComparison.Ordinal)).ToList();
            bool HasSingle(string metric, string condition, double expected) => summaries.Any(row =>
                row.Metric == metric && row.Condition == condition && row.SpecimenCount == 1 && Close(row.Mean, expected));
            return summaries.Count == 7 && summaries.Sum(row => row.SpecimenCount) == 8 &&
                summaries.Count(row => row.Condition == baseline.Condition && row.Metric == baseline.Metric) == 2 &&
                summaries.Where(row => row.Condition == baseline.Condition && row.Metric == baseline.Metric)
                    .Select(row => row.MethodGroup).Distinct(StringComparer.Ordinal).Count() == 2 &&
                summaries.Any(row => row.Metric == baseline.Metric && row.Condition == baseline.Condition &&
                    row.SpecimenCount == 2 && Close(row.Mean, 80)) &&
                HasSingle(baseline.Metric, baseline.Condition, 55) &&
                HasSingle(baseline.Metric, "5–30 s · 2 mm displacement · cycle 1", 50) &&
                HasSingle(baseline.Metric, "10–60 s · 2 mm displacement · cycle 1", 40) &&
                HasSingle(baseline.Metric, "10–30 s · 3 mm displacement · cycle 1", 60) &&
                HasSingle(baseline.Metric, "10–30 s · 2 mm displacement · cycle 2", 65) &&
                HasSingle("Force retention at 30% Strain", baseline.Condition, 45) &&
                readings.All(point => point.ForceRetentionPercent == "stale");
        }

        bool VerifyCompressionRetention()
        {
            CompressionPointRecord Point(string force, string time) => new()
            {
                SpecimenId = "S1", CycleNumber = 1, TargetStrainPercent = "20",
                TargetReached = true, DisplacementMm = "2", ForceN = force, HoldTimeSeconds = time
            };
            void Calculate(params CompressionPointRecord[] readings) =>
                Services.FlexibleMaterialTestingService.Recalculate(specimens, readings, [], []);
            bool IsRetention(CompressionPointRecord point, double expected) =>
                Services.FlexibleMaterialTestingService.ParseOptional(point.ForceRetentionPercent) is { } actual &&
                Math.Abs(actual - expected) < 0.000001d;

            var first = Point("100", "10");
            var later = Point("85", "30,0");
            later.TargetStrainPercent = "20,0";
            later.DisplacementMm = "2.0";
            Calculate(later, first);
            if (first.ForceRetentionPercent.Length != 0 || !IsRetention(later, 85)) return false;

            var notified = false;
            later.PropertyChanged += (_, args) => notified |= args.PropertyName == nameof(CompressionPointRecord.ForceRetentionPercent);
            first.ForceN = "200";
            Calculate(first, later);
            if (!notified || !IsRetention(later, 42.5)) return false;
            first.ForceN = "100";

            // Unrelated conditions must never borrow this specimen's reference force.
            var otherCycle = Point("90", "30"); otherCycle.CycleNumber = 2;
            var otherSpecimen = Point("90", "30"); otherSpecimen.SpecimenId = "S2";
            var otherDisplacement = Point("90", "30"); otherDisplacement.DisplacementMm = "3";
            var otherTarget = Point("90", "30"); otherTarget.TargetStrainPercent = "30";
            Calculate(first, later, otherCycle, otherSpecimen, otherDisplacement, otherTarget);
            if (!IsRetention(later, 85) || new[] { otherCycle, otherSpecimen, otherDisplacement, otherTarget }
                .Any(point => point.ForceRetentionPercent.Length != 0)) return false;

            foreach (var invalidate in new Action<CompressionPointRecord>[]
            {
                point => point.TargetReached = false,
                point => point.CycleNumber = 0,
                point => point.TargetStrainPercent = "",
                point => point.TargetStrainPercent = "0",
                point => point.TargetStrainPercent = "101",
                point => point.DisplacementMm = "",
                point => point.DisplacementMm = "0",
                point => point.DisplacementMm = "-1",
                point => point.HoldTimeSeconds = "",
                point => point.HoldTimeSeconds = "-1",
                point => point.ForceN = "",
                point => point.ForceN = "-1",
                point => point.ForceN = "NaN"
            })
            {
                var invalid = Point("50", "5");
                invalidate(invalid);
                invalid.ForceRetentionPercent = "stale";
                Calculate(invalid, first, later);
                if (invalid.ForceRetentionPercent.Length != 0 || !IsRetention(later, 85)) return false;
            }

            Calculate(first, Point("100", "10,0"), later);
            if (first.ForceRetentionPercent.Length != 0 || later.ForceRetentionPercent.Length != 0) return false;
            first.ForceN = "0";
            Calculate(first, later);
            if (later.ForceRetentionPercent.Length != 0) return false;
            first.ForceN = "100";
            first.HoldTimeSeconds = "0";
            later.ForceN = "0";
            Calculate(first, later);
            if (!IsRetention(later, 0)) return false;

            var legacy = new[]
            {
                new StressRelaxationPointRecord { SpecimenId = "S1", CompressionPercent = "20", ElapsedTimeSeconds = "10", ForceN = "100" },
                new StressRelaxationPointRecord { SpecimenId = "S1", CompressionPercent = "20", ElapsedTimeSeconds = "30", ForceN = "85" }
            };
            Services.FlexibleMaterialTestingService.Recalculate(specimens, [first, later], legacy, []);
            return legacy[0].ForceRetentionPercent.Length == 0 &&
                Services.FlexibleMaterialTestingService.ParseOptional(legacy[1].ForceRetentionPercent) == 85d &&
                legacy[0].ForceN == "100" && legacy[1].ForceN == "85";
        }
    }

    private static bool VerifyRecoveryTvlInput()
    {
        var specimen = new FlexibleTestSpecimenRecord { SpecimenId = "TVL", FlexibleTestSessionId = "TVL-SESSION" };
        var row = new RecoveryMeasurementRecord { SpecimenId = specimen.SpecimenId, InitialHeightMm = "10" };
        var notifications = new HashSet<string>();
        row.PropertyChanged += (_, change) => notifications.Add(change.PropertyName ?? string.Empty);
        bool Number(string value, double expected) =>
            Services.FlexibleMaterialTestingService.ParseOptional(value) is double actual && Math.Abs(actual - expected) < 0.000001d;
        bool Valid() => Services.FlexibleMaterialTestingService.Validate([specimen], [], [], [row], []).Count == 0;
        void Calculate() => Services.FlexibleMaterialTestingService.Recalculate([specimen], [], [], [row]);

        row.TvlContactOffsetMm = "0.14";
        Calculate();
        if (!Valid() || !Number(row.HeightAfterRestMm, 9.86) || !Number(row.ResidualHeightLossPercent, 1.4) ||
            !notifications.Contains(nameof(row.HeightAfterRestMm)) ||
            !notifications.Contains(nameof(row.TvlContactOffsetMm)) ||
            !notifications.Contains(nameof(row.ResidualHeightLossPercent))) return false;
        row.TvlContactOffsetMm = "0,14";
        if (row.TvlContactOffsetMm != "0,14" || !Number(row.HeightAfterRestMm, 9.86)) return false;
        row.InitialHeightMm = "9";
        if (!Number(row.HeightAfterRestMm, 9.86) || !Number(row.TvlContactOffsetMm, -0.86)) return false;
        row.TvlContactOffsetMm = "0,14";
        Calculate();
        if (!Number(row.HeightAfterRestMm, 8.86) || !Number(row.ResidualHeightLossPercent, 1.56)) return false;
        row.TvlContactOffsetMm = "0";
        if (!Valid() || !Number(row.HeightAfterRestMm, 9)) return false;
        row.TvlContactOffsetMm = "-0.14";
        if (!Valid() || !Number(row.HeightAfterRestMm, 9.14)) return false;
        var savedHeight = row.HeightAfterRestMm;
        foreach (var invalid in new[] { "NaN", "Infinity", "bad", "10", "-" })
        {
            row.TvlContactOffsetMm = invalid;
            if (Valid() || row.TvlContactOffsetError.Length == 0 || row.TvlContactOffsetMm != invalid ||
                row.HeightAfterRestMm != savedHeight) return false;
        }
        row.InitialHeightMm = "8";
        if (row.TvlContactOffsetMm != "-" || Valid() || row.HeightAfterRestMm != savedHeight) return false;
        row.HeightAfterRestMm = "7.50";
        if (!Valid() || !Number(row.TvlContactOffsetMm, 0.5) || row.HeightAfterRestMm != "7.50") return false;
        row.TvlContactOffsetMm = "";
        if (!Valid() || row.HeightAfterRestMm.Length != 0 || row.TvlContactOffsetMm.Length != 0) return false;
        row.InitialHeightMm = "";
        row.TvlContactOffsetMm = "0.14";
        if (Valid() || row.HeightAfterRestMm.Length != 0) return false;
        row.InitialHeightMm = "1";
        if (!Valid() || !Number(row.HeightAfterRestMm, 0.86)) return false;
        row.InitialHeightMm = "10";
        if (!Valid() || !Number(row.HeightAfterRestMm, 9.86)) return false;
        row.AcceptInputCommit();
        row.InitialHeightMm = "11";
        if (!Number(row.HeightAfterRestMm, 9.86) || !Number(row.TvlContactOffsetMm, 1.14)) return false;
        row.TvlContactOffsetMm = "12";
        if (Valid()) return false;
        row.AcceptInputCommit(); // Invalid edits cannot release pending offset ownership.
        row.InitialHeightMm = "2";
        if (Valid() || !Number(row.HeightAfterRestMm, 9.86)) return false;
        row.InitialHeightMm = "20";
        if (!Valid() || !Number(row.HeightAfterRestMm, 8)) return false;
        row.InitialHeightMm = "200";
        if (!Valid() || !Number(row.HeightAfterRestMm, 188) || row.TvlContactOffsetMm != "12") return false;
        row.AcceptInputCommit();
        row.InitialHeightMm = "201";
        if (!Number(row.HeightAfterRestMm, 188) || !Number(row.TvlContactOffsetMm, 13)) return false;
        var legacy = new RecoveryMeasurementRecord { InitialHeightMm = "10", HeightAfterRestMm = "9.5" };
        return Number(legacy.TvlContactOffsetMm, 0.5) && legacy.InitialHeightMm == "10" && legacy.HeightAfterRestMm == "9.5";
    }

    public static bool RunFlexibleMaterialEvidenceContractVerification()
    {
        var sessions = new[]
        {
            new FlexibleTestSessionRecord { FlexibleTestSessionId = "A", MaterialID = "MAT-A" },
            new FlexibleTestSessionRecord { FlexibleTestSessionId = "B", MaterialID = "MAT-B" },
            new FlexibleTestSessionRecord { FlexibleTestSessionId = "OFF", MaterialID = "MAT-A", IsActive = false }
        };
        var specimens = new[]
        {
            new FlexibleTestSpecimenRecord { SpecimenId = "A1", FlexibleTestSessionId = "A" },
            new FlexibleTestSpecimenRecord { SpecimenId = "A2", FlexibleTestSessionId = "A", InitialHeightMm = "10,0" },
            new FlexibleTestSpecimenRecord { SpecimenId = "B1", FlexibleTestSessionId = "B" },
            new FlexibleTestSpecimenRecord { SpecimenId = "OFF1", FlexibleTestSessionId = "OFF" },
            new FlexibleTestSpecimenRecord { SpecimenId = "ORPHAN", FlexibleTestSessionId = "MISSING" }
        };
        RecoveryMeasurementRecord Recovery(string specimen, string height) => new()
        {
            SpecimenId = specimen, InitialHeightMm = "10", HeightAfterRestMm = height,
            CompressionPercent = "20", CompressionHoldSeconds = "30", RestTimeSeconds = "60"
        };
        var recoveries = new[] { Recovery("A1", "9.8"), Recovery("A1", "9.8"), Recovery("A2", "9.6"),
            Recovery("B1", "5"), Recovery("OFF1", "5"), Recovery("ORPHAN", "5"), Recovery("MISSING", "5") };
        var points = new[]
        {
            new CompressionPointRecord { SpecimenId = "A1", TargetStrainPercent = "20", DisplacementMm = "2",
                HoldTimeSeconds = "10", ForceN = "100", ApparentStressMpa = "stale" },
            new CompressionPointRecord { SpecimenId = "A1", TargetStrainPercent = "20", DisplacementMm = "2",
                HoldTimeSeconds = "30", ForceN = "80", ForceRetentionPercent = "stale" }
        };
        var legacy = new[] { new StressRelaxationPointRecord { SpecimenId = "A1" }, new StressRelaxationPointRecord { SpecimenId = "B1" } };
        FlexibleMaterialEvidenceSnapshot Build() => Services.FlexibleMaterialEvidenceService.Build("mat-a", sessions,
            specimens, points, legacy, recoveries, []);
        static bool Near(double? actual, double expected) => actual.HasValue && Math.Abs(actual.Value - expected) < 0.000001d;
        var snapshot = Build();
        var recovery = snapshot.Groups.Single(x => x.MetricKind == FlexibleMetricKind.ResidualHeightLoss);
        if (snapshot.SessionCount != 1 || snapshot.SpecimenCount != 2 || !snapshot.HasResults || snapshot.LegacyRelaxationPointCount != 1 ||
            recovery.SpecimenCount != 2 || !Near(recovery.Mean, 3d) || !Near(recovery.StandardDeviation, Math.Sqrt(2d)) ||
            !Near(recovery.Minimum, 2d) || !Near(recovery.Maximum, 4d) ||
            !snapshot.Groups.Any(x => x.MetricKind == FlexibleMetricKind.ApparentCompressiveStress && Near(x.Mean, 100d / (Math.PI * 81d / 4d))) ||
            points[0].ApparentStressMpa != "stale" || points[1].ForceRetentionPercent != "stale" ||
            recoveries[0].ResidualHeightLossPercent.Length != 0) return false;
        var originalCulture = CultureInfo.CurrentCulture;
        try
        {
            CultureInfo.CurrentCulture = CultureInfo.GetCultureInfo("is-IS");
            if (!snapshot.Groups.Select(x => x.ComparisonKey).Order().SequenceEqual(Build().Groups.Select(x => x.ComparisonKey).Order())) return false;
        }
        finally { CultureInfo.CurrentCulture = originalCulture; }
        var alternateTargetPoints = points.Select(x => new CompressionPointRecord
        {
            SpecimenId = x.SpecimenId, TargetStrainPercent = "25", DisplacementMm = x.DisplacementMm,
            HoldTimeSeconds = x.HoldTimeSeconds, ForceN = x.ForceN, CycleNumber = x.CycleNumber
        }).ToArray();
        var alternateTarget = Services.FlexibleMaterialEvidenceService.Build("MAT-A", sessions, specimens, alternateTargetPoints, [], [], []);
        foreach (var kind in new[] { FlexibleMetricKind.CompressionForce, FlexibleMetricKind.ApparentCompressiveStress,
            FlexibleMetricKind.ForceRetention, FlexibleMetricKind.ForceReduction })
        {
            var originalKeys = snapshot.Groups.Where(x => x.MetricKind == kind).Select(x => x.ComparisonKey).ToHashSet(StringComparer.Ordinal);
            var alternateKeys = alternateTarget.Groups.Where(x => x.MetricKind == kind).Select(x => x.ComparisonKey).ToArray();
            if (originalKeys.Count == 0 || alternateKeys.Length != originalKeys.Count || alternateKeys.Any(originalKeys.Contains)) return false;
        }
        var displaced = points.Concat([new CompressionPointRecord { SpecimenId = "A2", TargetStrainPercent = "20",
            DisplacementMm = "1.9", HoldTimeSeconds = "10", ForceN = "300" }]).ToArray();
        var displacementGroups = Services.FlexibleMaterialEvidenceService.Build("MAT-A", sessions, specimens, displaced, [], [], []).Groups;
        if (displacementGroups.Count(x => x.MetricKind == FlexibleMetricKind.CompressionForce) != 3 ||
            displacementGroups.Count(x => x.MetricKind == FlexibleMetricKind.ApparentCompressiveStress) != 3 ||
            displacementGroups.Where(x => x.MetricKind == FlexibleMetricKind.CompressionForce).Any(x => x.SpecimenCount != 1) ||
            displacementGroups.Any(x => x.MethodGroup.Contains("0.20000000000000001", StringComparison.Ordinal))) return false;
        recoveries[2].CompressionHoldSeconds = "60";
        if (Build().Groups.Count(x => x.MetricKind == FlexibleMetricKind.ResidualHeightLoss) != 2) return false;
        recoveries[2].CompressionHoldSeconds = "30";
        recoveries[2].HeightAfterRestMm = string.Empty;
        var missingHeight = Build().Groups.Single(x => x.MetricKind == FlexibleMetricKind.ResidualHeightLoss);
        if (missingHeight.SpecimenCount != 1 || missingHeight.StandardDeviation is not null || !Near(missingHeight.Mean, 2d)) return false;
        recoveries[2].HeightAfterRestMm = "9.6";
        points[1].DisplacementMm = "3";
        if (Build().Groups.Any(x => x.MetricKind is FlexibleMetricKind.ForceRetention or FlexibleMetricKind.ForceReduction)) return false;
        points[1].DisplacementMm = "2";
        var duplicatePoints = points.Concat([points[0]]).ToArray();
        if (Services.FlexibleMaterialEvidenceService.Build("MAT-A", sessions, specimens, duplicatePoints, [], [], []).Groups
            .Any(x => x.MetricKind is FlexibleMetricKind.ForceRetention or FlexibleMetricKind.ForceReduction)) return false;
        var ambiguous = specimens.Concat([new FlexibleTestSpecimenRecord { SpecimenId = "A1", FlexibleTestSessionId = "B" }]).ToArray();
        var isolated = Services.FlexibleMaterialEvidenceService.Build("MAT-A", sessions, ambiguous, points, legacy, recoveries, []);
        var ambiguousSessions = sessions.Concat([new FlexibleTestSessionRecord { FlexibleTestSessionId = "A", MaterialID = "MAT-B" }]).ToArray();
        return isolated.SpecimenCount == 1 && isolated.LegacyRelaxationPointCount == 0 &&
            isolated.Groups.Count == 1 && Near(isolated.Groups[0].Mean, 4d) &&
            !Services.FlexibleMaterialEvidenceService.Build("MAT-A", ambiguousSessions, specimens, points, legacy, recoveries, []).HasResults &&
            !Services.FlexibleMaterialEvidenceService.Build("UNKNOWN", sessions, specimens, points, legacy, recoveries, []).HasResults &&
            !Services.FlexibleMaterialEvidenceService.Build("MAT-A", sessions, specimens, [], [], [], []).HasResults;
    }

    public static bool RunTpuCompressionMethodV1ContractVerification()
    {
        var forces10 = new[] { 450d, 430d, 435d, 410d, 450d, 445d, 426d, 445d, 420d, 419d };
        var forces30 = new[] { 408d, 405d, 402d, 391d, 418d, 410d, 394d, 410d, 387d, 380d };
        var specimens = Enumerable.Range(1, 10).Select(index => new FlexibleTestSpecimenRecord
        {
            SpecimenId = $"V1-{index}", FlexibleTestSessionId = "UNLINKED-VERIFICATION-EVIDENCE",
            DiameterMm = "9", InitialHeightMm = "10", InfillPercent = "100", InfillPattern = "Rectilinear",
            MethodVersion = Services.FlexibleMaterialTestingService.CompressionMethodVersion
        }).ToArray();
        var points = specimens.SelectMany((specimen, index) => new[]
        {
            new CompressionPointRecord { SpecimenId=specimen.SpecimenId,CycleNumber=1,TargetStrainPercent="20",TargetReached=true,DisplacementMm="2",ForceN=forces10[index].ToString(CultureInfo.InvariantCulture),HoldTimeSeconds="10" },
            new CompressionPointRecord { SpecimenId=specimen.SpecimenId,CycleNumber=1,TargetStrainPercent="20",TargetReached=true,DisplacementMm="2",ForceN=forces30[index].ToString(CultureInfo.InvariantCulture),HoldTimeSeconds="30" }
        }).ToArray();
        var comparisons = Services.FlexibleMaterialTestingService.BuildComparisons(specimens, points, []);
        var publicSummary = Services.FlexibleMaterialTestingService.BuildPublicMethodV1Summary(
            "MAT-VALIDATION",
            [new FlexibleTestSessionRecord { FlexibleTestSessionId="UNLINKED-VERIFICATION-EVIDENCE",MaterialID="MAT-VALIDATION",IsActive=true }],
            specimens,
            points);
        var at10 = comparisons.Single(row => row.Metric == "Compression Force at 20% Strain" && row.Condition.StartsWith("10 s hold", StringComparison.Ordinal));
        var at30 = comparisons.Single(row => row.Metric == "Compression Force at 20% Strain" && row.Condition.StartsWith("30 s hold", StringComparison.Ordinal));
        var reduction = comparisons.Single(row => row.Metric == "Force reduction from 10 s to 30 s at 20% Strain");
        static bool Close(string actual, double expected, double tolerance) =>
            Services.FlexibleMaterialTestingService.ParseOptional(actual) is double value && Math.Abs(value - expected) <= tolerance;
        return at10.SpecimenCount == 10 && Close(at10.Mean, 433.0, 0.001) && Close(at10.StandardDeviation, 14.228, 0.001) &&
               Close(at10.CoefficientOfVariation, 3.286, 0.001) && at10.Minimum == "410" && at10.Maximum == "450" &&
               at30.SpecimenCount == 10 && Close(at30.Mean, 400.5, 0.001) && Close(at30.StandardDeviation, 12.021, 0.001) &&
               Close(at30.CoefficientOfVariation, 3.002, 0.001) && at30.Minimum == "380" && at30.Maximum == "418" &&
               forces30[1] == 405d && reduction.SpecimenCount == 10 &&
               publicSummary is { SpecimenCount30Seconds: 10, SpecimenCount10Seconds: 10 } &&
               Math.Abs(publicSummary.Mean30SecondsN - 400.5d) < 0.001d &&
               Services.FlexibleMaterialTestingService.BuildPublicMethodV1Summary("UNKNOWN", [], specimens, points) is null &&
               Services.FlexibleMaterialTestingService.CalculateForceReductionPercent("450", "408") is double firstReduction &&
               Math.Abs(firstReduction - 9.333333333d) < 0.000001d;
    }

    public static bool RunPendingCompressionRowValidationContractVerification()
    {
        var specimen = new FlexibleTestSpecimenRecord { SpecimenId="PENDING",FlexibleTestSessionId="FTS-PENDING" };
        IReadOnlyList<string> Validate(CompressionPointRecord point) =>
            Services.FlexibleMaterialTestingService.Validate([specimen], [point], [], [], []);
        var pending10 = new CompressionPointRecord { SpecimenId=specimen.SpecimenId,CycleNumber=1,TargetStrainPercent="20",TargetReached=true,HoldTimeSeconds="10" };
        var pending30 = new CompressionPointRecord { SpecimenId=specimen.SpecimenId,CycleNumber=1,TargetStrainPercent="20",TargetReached=true,HoldTimeSeconds="30" };
        var invalidMeasured = new CompressionPointRecord { SpecimenId=specimen.SpecimenId,CycleNumber=1,TargetStrainPercent="20",TargetReached=true,HoldTimeSeconds="30",ForceN="400" };
        var forceLimited = new CompressionPointRecord { SpecimenId=specimen.SpecimenId,CycleNumber=1,TargetStrainPercent="20",TargetReached=false,HoldTimeSeconds="30",ForceN="499" };
        var completed = new CompressionPointRecord { SpecimenId=specimen.SpecimenId,CycleNumber=1,TargetStrainPercent="20",TargetReached=true,HoldTimeSeconds="30",ForceN="400",DisplacementMm="2" };
        return Validate(pending10).Count == 0 && Validate(pending30).Count == 0 &&
               Validate(invalidMeasured).Any(error => error.Contains("with force requires a displacement", StringComparison.Ordinal)) &&
               Validate(forceLimited).Count == 0 && Validate(completed).Count == 0;
    }

    public static bool RunFlexibleTestingPersistenceContractVerification()
    {
        var folder = IOPath.Combine(IOPath.GetTempPath(), "3DPIceland-FlexibleContract-" + Guid.NewGuid().ToString("N"));
        var path = IOPath.Combine(folder, "contract.sqlite");
        try
        {
            IODirectory.CreateDirectory(folder);
            using (var connection = new SqliteConnection($"Data Source={path};Pooling=False"))
            {
                connection.Open();
                using var parent = connection.CreateCommand();
                parent.CommandText = """
CREATE TABLE NativeMaterialManagerRows(MaterialID TEXT PRIMARY KEY);
INSERT INTO NativeMaterialManagerRows VALUES ('MAT-VERIFY');
CREATE TABLE MaterialExperiments(MaterialExperimentId TEXT PRIMARY KEY,MaterialID TEXT NOT NULL);
INSERT INTO MaterialExperiments VALUES ('SERIES-VERIFY','MAT-VERIFY');
CREATE TABLE ExperimentalRuns(ExperimentalRunId TEXT PRIMARY KEY,MaterialExperimentId TEXT NOT NULL,MeasuredDate TEXT,IsActive INTEGER,CreatedAtUtc TEXT,UpdatedAtUtc TEXT);
INSERT INTO ExperimentalRuns VALUES ('RUN-VERIFY','SERIES-VERIFY','2026-01-01',1,'2026-01-01','2026-01-01');
CREATE TABLE FlexibleTestSpecimens (
 SpecimenId TEXT PRIMARY KEY,ExperimentalRunId TEXT NOT NULL,SpecimenLabel TEXT NOT NULL,IntendedTest TEXT NOT NULL,Shape TEXT NOT NULL,
 DiameterMm TEXT,InitialHeightMm TEXT,ThicknessMm TEXT,MassG TEXT,InfillPercent TEXT,InfillPattern TEXT,NozzleDiameterMm TEXT,LayerHeightMm TEXT,
 Perimeters TEXT,TopLayers TEXT,BottomLayers TEXT,PrintTemperatureC TEXT,ExtrusionMultiplier TEXT,PrintSettings TEXT,MethodVersion TEXT,MethodNotes TEXT,
 CreatedAtUtc TEXT NOT NULL,UpdatedAtUtc TEXT NOT NULL);
INSERT INTO FlexibleTestSpecimens VALUES ('SPEC-1','RUN-VERIFY','S1','Compression','Cylinder','20','10','','','30','Gyroid','0.4','0.20','2','3','3','225','0.95','Z compression','TPU-COMP-v1','in-house','2026-01-01','2026-01-01');
""";
                parent.ExecuteNonQuery();
                EnsureFlexibleTestingSchema(connection);
                using var insert = connection.CreateCommand();
                insert.CommandText = """
INSERT INTO CompressionMeasurementPoints VALUES ('CMP-1','SPEC-1',2,'25',0,'2.1','500','10','force limit','2026-01-01');
INSERT INTO StressRelaxationPoints VALUES ('REL-1','SPEC-1',2,'25','60','300','actual time','2026-01-01');
INSERT INTO RecoveryMeasurements VALUES ('RCV-1','SPEC-1',2,'10','9.5','3600','25','60','not compression set','2026-01-01');
INSERT INTO ShoreHardnessReadings VALUES ('SHR-A','SPEC-1','A','92','1','10','measured','2026-01-01');
INSERT INTO ShoreHardnessReadings VALUES ('SHR-D','SPEC-1','D','38','1','10','separate scale','2026-01-01');
INSERT INTO FlexibleTestSessions VALUES ('FTS-STANDALONE','MAT-VERIFY','Standalone','','','',1,'2026-01-01','2026-01-01');
INSERT INTO FlexibleTestSpecimens VALUES ('SPEC-STANDALONE','FTS-STANDALONE',NULL,'Standalone','Compression','Cylinder','20','10','','','100','Rectilinear','0.4','0.20','2','3','3','','','','TPU-COMP-v1','in-house','2026-01-01','2026-01-01');
""";
                insert.ExecuteNonQuery();
                var enteredRecovery = new RecoveryMeasurementRecord
                {
                    InitialHeightMm = "10", TvlContactOffsetMm = "0,14"
                };
                using var recoveryTransaction = connection.BeginTransaction();
                Upsert(connection, recoveryTransaction,
                    "INSERT INTO RecoveryMeasurements VALUES ('RCV-TVL','SPEC-STANDALONE',1,$initial,$after,'60','20','30','TVL roundtrip','2026-01-01');",
                    ("$initial", enteredRecovery.InitialHeightMm), ("$after", enteredRecovery.HeightAfterRestMm));
                recoveryTransaction.Commit();
            }
            using var reopened = new SqliteConnection($"Data Source={path};Mode=ReadOnly;Pooling=False");
            reopened.Open();
            using var verify = reopened.CreateCommand();
            verify.CommandText = """
SELECT
 (SELECT InfillPattern || '|' || PrintTemperatureC || '|' || ExtrusionMultiplier FROM FlexibleTestSpecimens WHERE SpecimenId='SPEC-1') || ';' ||
 (SELECT CycleNumber || '|' || TargetStrainPercent || '|' || TargetReached || '|' || HoldTimeSeconds FROM CompressionMeasurementPoints WHERE CompressionPointId='CMP-1') || ';' ||
 (SELECT COUNT(DISTINCT ShoreScale) FROM ShoreHardnessReadings WHERE SpecimenId='SPEC-1') || ';' ||
 (SELECT MaterialID || '|' || LegacyExperimentalRunId FROM FlexibleTestSessions WHERE FlexibleTestSessionId='FTS-LEGACY-RUN-VERIFY') || ';' ||
 (SELECT FlexibleTestSessionId || '|' || ExperimentalRunId FROM FlexibleTestSpecimens WHERE SpecimenId='SPEC-1') || ';' ||
 (SELECT CASE WHEN ExperimentalRunId IS NULL THEN 'NULL-RUN' ELSE 'BAD-RUN' END FROM FlexibleTestSpecimens WHERE SpecimenId='SPEC-STANDALONE') || ';' ||
 (SELECT InitialHeightMm || '|' || HeightAfterRestMm || '|' || RestTimeSeconds || '|' || CompressionPercent || '|' || CompressionHoldSeconds FROM RecoveryMeasurements WHERE RecoveryMeasurementId='RCV-1');
""";
            if (!string.Equals(verify.ExecuteScalar()?.ToString(),
                    "Gyroid|225|0.95;2|25|0|10;2;MAT-VERIFY|RUN-VERIFY;FTS-LEGACY-RUN-VERIFY|RUN-VERIFY;NULL-RUN;10|9.5|3600|25|60",
                    StringComparison.Ordinal)) return false;
            var recovered = ReadRecovery(reopened);
            var legacyRecovery = recovered.Single(row => row.RecoveryMeasurementId == "RCV-1");
            var tvlRecovery = recovered.Single(row => row.RecoveryMeasurementId == "RCV-TVL");
            return legacyRecovery.InitialHeightMm == "10" && legacyRecovery.HeightAfterRestMm == "9.5" &&
                Services.FlexibleMaterialTestingService.ParseOptional(legacyRecovery.TvlContactOffsetMm) == 0.5d &&
                tvlRecovery.InitialHeightMm == "10" &&
                Services.FlexibleMaterialTestingService.ParseOptional(tvlRecovery.HeightAfterRestMm) == 9.86d &&
                Services.FlexibleMaterialTestingService.ParseOptional(tvlRecovery.TvlContactOffsetMm) == 0.14d;
        }
        catch { return false; }
        finally
        {
            SqliteConnection.ClearAllPools();
            try { if (IODirectory.Exists(folder)) IODirectory.Delete(folder, true); } catch { }
        }
    }

    private static void Upsert(SqliteConnection connection, SqliteTransaction transaction, string sql, params (string Name, object? Value)[] values)
    {
        using var command = connection.CreateCommand(); command.Transaction = transaction; command.CommandText = sql;
        foreach (var value in values) command.Parameters.AddWithValue(value.Name, value.Value ?? string.Empty);
        command.ExecuteNonQuery();
    }

    private static object OptionalForeignKey(string value) =>
        string.IsNullOrWhiteSpace(value) ? DBNull.Value : value.Trim();

    private static void DeleteMissing(SqliteConnection connection, SqliteTransaction transaction, string table, string idColumn, IEnumerable<string> retained)
    {
        var ids = retained.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        using var command = connection.CreateCommand(); command.Transaction = transaction;
        if (ids.Count == 0) command.CommandText = $"DELETE FROM {table};";
        else
        {
            var names = ids.Select((_, i) => $"$id{i}").ToList();
            command.CommandText = $"DELETE FROM {table} WHERE {idColumn} NOT IN ({string.Join(',', names)});";
            for (var i = 0; i < ids.Count; i++) command.Parameters.AddWithValue(names[i], ids[i]);
        }
        command.ExecuteNonQuery();
    }

    private static List<FlexibleTestSessionRecord> ReadSessions(SqliteConnection c) => Read(c, "SELECT * FROM FlexibleTestSessions ORDER BY MeasuredDate DESC,SessionLabel;", r => new FlexibleTestSessionRecord { FlexibleTestSessionId=S(r,"FlexibleTestSessionId"),MaterialID=S(r,"MaterialID"),SessionLabel=S(r,"SessionLabel"),MeasuredDate=S(r,"MeasuredDate"),Notes=S(r,"Notes"),LegacyExperimentalRunId=S(r,"LegacyExperimentalRunId"),IsActive=I(r,"IsActive")!=0,CreatedAtUtc=S(r,"CreatedAtUtc"),UpdatedAtUtc=S(r,"UpdatedAtUtc") });
    private static List<FlexibleTestSpecimenRecord> ReadSpecimens(SqliteConnection c) => Read(c, "SELECT * FROM FlexibleTestSpecimens ORDER BY FlexibleTestSessionId,SpecimenLabel,SpecimenId;", r => new FlexibleTestSpecimenRecord { SpecimenId=S(r,"SpecimenId"),FlexibleTestSessionId=S(r,"FlexibleTestSessionId"),ExperimentalRunId=S(r,"ExperimentalRunId"),SpecimenLabel=S(r,"SpecimenLabel"),IntendedTest=S(r,"IntendedTest"),Shape=S(r,"Shape"),DiameterMm=S(r,"DiameterMm"),InitialHeightMm=S(r,"InitialHeightMm"),ThicknessMm=S(r,"ThicknessMm"),MassG=S(r,"MassG"),InfillPercent=S(r,"InfillPercent"),InfillPattern=S(r,"InfillPattern"),NozzleDiameterMm=S(r,"NozzleDiameterMm"),LayerHeightMm=S(r,"LayerHeightMm"),Perimeters=S(r,"Perimeters"),TopLayers=S(r,"TopLayers"),BottomLayers=S(r,"BottomLayers"),PrintTemperatureC=S(r,"PrintTemperatureC"),ExtrusionMultiplier=S(r,"ExtrusionMultiplier"),PrintSettings=S(r,"PrintSettings"),MethodVersion=S(r,"MethodVersion"),MethodNotes=S(r,"MethodNotes"),CreatedAtUtc=S(r,"CreatedAtUtc"),UpdatedAtUtc=S(r,"UpdatedAtUtc") });
    private static List<CompressionPointRecord> ReadCompression(SqliteConnection c) => Read(c, "SELECT * FROM CompressionMeasurementPoints ORDER BY SpecimenId,CycleNumber,rowid;", r => new CompressionPointRecord { CompressionPointId=S(r,"CompressionPointId"),SpecimenId=S(r,"SpecimenId"),CycleNumber=I(r,"CycleNumber"),TargetStrainPercent=S(r,"TargetStrainPercent"),TargetReached=I(r,"TargetReached")!=0,DisplacementMm=S(r,"DisplacementMm"),ForceN=S(r,"ForceN"),HoldTimeSeconds=S(r,"HoldTimeSeconds"),Notes=S(r,"Notes"),UpdatedAtUtc=S(r,"UpdatedAtUtc") });
    private static List<StressRelaxationPointRecord> ReadRelaxation(SqliteConnection c) => Read(c, "SELECT * FROM StressRelaxationPoints ORDER BY SpecimenId,CycleNumber,rowid;", r => new StressRelaxationPointRecord { RelaxationPointId=S(r,"RelaxationPointId"),SpecimenId=S(r,"SpecimenId"),CycleNumber=I(r,"CycleNumber"),CompressionPercent=S(r,"CompressionPercent"),ElapsedTimeSeconds=S(r,"ElapsedTimeSeconds"),ForceN=S(r,"ForceN"),Notes=S(r,"Notes"),UpdatedAtUtc=S(r,"UpdatedAtUtc") });
    private static List<RecoveryMeasurementRecord> ReadRecovery(SqliteConnection c) => Read(c, "SELECT * FROM RecoveryMeasurements ORDER BY SpecimenId,CycleNumber,rowid;", r => new RecoveryMeasurementRecord { RecoveryMeasurementId=S(r,"RecoveryMeasurementId"),SpecimenId=S(r,"SpecimenId"),CycleNumber=I(r,"CycleNumber"),InitialHeightMm=S(r,"InitialHeightMm"),HeightAfterRestMm=S(r,"HeightAfterRestMm"),RestTimeSeconds=S(r,"RestTimeSeconds"),CompressionPercent=S(r,"CompressionPercent"),CompressionHoldSeconds=S(r,"CompressionHoldSeconds"),Notes=S(r,"Notes"),UpdatedAtUtc=S(r,"UpdatedAtUtc") });
    private static List<ShoreHardnessReadingRecord> ReadShore(SqliteConnection c) => Read(c, "SELECT * FROM ShoreHardnessReadings ORDER BY SpecimenId,ShoreScale,rowid;", r => new ShoreHardnessReadingRecord { ShoreReadingId=S(r,"ShoreReadingId"),SpecimenId=S(r,"SpecimenId"),ShoreScale=S(r,"ShoreScale"),HardnessValue=S(r,"HardnessValue"),ReadingTimeSeconds=S(r,"ReadingTimeSeconds"),SpecimenThicknessMm=S(r,"SpecimenThicknessMm"),Notes=S(r,"Notes"),UpdatedAtUtc=S(r,"UpdatedAtUtc") });
    private static List<T> Read<T>(SqliteConnection c, string sql, Func<SqliteDataReader,T> map) { using var command=c.CreateCommand();command.CommandText=sql;using var reader=command.ExecuteReader();var rows=new List<T>();while(reader.Read())rows.Add(map(reader));return rows; }
    private static string S(SqliteDataReader r,string name)=>r[name] is DBNull?string.Empty:Convert.ToString(r[name],CultureInfo.InvariantCulture)??string.Empty;
    private static int I(SqliteDataReader r,string name)=>Convert.ToInt32(r[name],CultureInfo.InvariantCulture);
}
