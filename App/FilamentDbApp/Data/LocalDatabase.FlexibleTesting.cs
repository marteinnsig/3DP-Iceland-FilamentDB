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
        return Math.Abs(strain.GetValueOrDefault() - 10d) < 0.000001d &&
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
               new FlexibleTestSpecimenRecord().InitialHeightMm == "9" &&
               new FlexibleTestSpecimenRecord().ThicknessMm == "9" &&
               Services.FlexibleMaterialTestingService.CalculateStrainPercent("1", "") is null &&
               Services.FlexibleMaterialTestingService.CalculateApparentStressMpa("100", "0") is null;
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
 (SELECT CASE WHEN ExperimentalRunId IS NULL THEN 'NULL-RUN' ELSE 'BAD-RUN' END FROM FlexibleTestSpecimens WHERE SpecimenId='SPEC-STANDALONE');
""";
            return string.Equals(verify.ExecuteScalar()?.ToString(), "Gyroid|225|0.95;2|25|0|10;2;MAT-VERIFY|RUN-VERIFY;FTS-LEGACY-RUN-VERIFY|RUN-VERIFY;NULL-RUN", StringComparison.Ordinal);
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
