using Microsoft.Data.Sqlite;

namespace FilamentDbApp.Data;

public sealed partial class LocalDatabase
{
    private const string ShoreTemplateRepairKey = "flexible-unused-shore-defaults-v67.0.8";
    private const string UnusedShoreTemplatePredicate = """
trim(COALESCE(HardnessValue,''))='' AND trim(COALESCE(ReadingTimeSeconds,''))=''
AND (trim(COALESCE(SpecimenThicknessMm,''))='' OR trim(SpecimenThicknessMm) IN ('9','9.0','9,0','10','10.0','10,0'))
""";

    public void RepairUnusedShoreTemplateDefaults()
    {
        using var connection = new SqliteConnection(ConnectionString);
        connection.Open();
        if (ShoreTemplateRepairDone(connection)) return;
        using var count = connection.CreateCommand();
        count.CommandText = "SELECT COUNT(*) FROM ShoreHardnessReadings WHERE " + UnusedShoreTemplatePredicate;
        if (Convert.ToInt64(count.ExecuteScalar()) > 0) CreateManualBackupNow();
        ApplyUnusedShoreTemplateRepair(connection);
    }

    private static bool ShoreTemplateRepairDone(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT COUNT(*) FROM AppMeta WHERE Key=$key";
        command.Parameters.AddWithValue("$key", ShoreTemplateRepairKey);
        return Convert.ToInt64(command.ExecuteScalar()) > 0;
    }

    private static void ApplyUnusedShoreTemplateRepair(SqliteConnection connection)
    {
        if (ShoreTemplateRepairDone(connection)) return;
        using var transaction = connection.BeginTransaction();
        using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = "UPDATE ShoreHardnessReadings SET ReadingTimeSeconds='10',SpecimenThicknessMm='8' WHERE " +
            UnusedShoreTemplatePredicate + "; INSERT INTO AppMeta(Key,Value) VALUES($key,'complete');";
        command.Parameters.AddWithValue("$key", ShoreTemplateRepairKey);
        command.ExecuteNonQuery();
        transaction.Commit();
    }

    public static bool VerifyUnusedShoreTemplateRepair()
    {
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = """
CREATE TABLE AppMeta(Key TEXT PRIMARY KEY,Value TEXT NOT NULL);
CREATE TABLE ShoreHardnessReadings(Id TEXT PRIMARY KEY,HardnessValue TEXT,ReadingTimeSeconds TEXT,SpecimenThicknessMm TEXT);
INSERT INTO ShoreHardnessReadings VALUES ('old','','','10'),('old9','','','9'),('blank','','',''),
 ('measured','82','','10'),('custom','','30','10'),('customThickness','','','7');
""";
        command.ExecuteNonQuery();
        command.CommandText = "CREATE TRIGGER fail_repair BEFORE UPDATE ON ShoreHardnessReadings WHEN OLD.Id='old9' BEGIN SELECT RAISE(ABORT,'injected failure'); END;";
        command.ExecuteNonQuery();
        try { ApplyUnusedShoreTemplateRepair(connection); return false; } catch (SqliteException) { }
        if (ShoreTemplateRepairDone(connection)) return false;
        command.CommandText = "SELECT COUNT(*) FROM ShoreHardnessReadings WHERE ReadingTimeSeconds='10'";
        if (Convert.ToInt32(command.ExecuteScalar()) != 0) return false;
        command.CommandText = "DROP TRIGGER fail_repair";
        command.ExecuteNonQuery();
        ApplyUnusedShoreTemplateRepair(connection);
        command.CommandText = "SELECT COUNT(*) FROM ShoreHardnessReadings WHERE ReadingTimeSeconds='10' AND SpecimenThicknessMm='8'";
        if (Convert.ToInt32(command.ExecuteScalar()) != 3) return false;
        command.CommandText = """
SELECT COUNT(*) FROM ShoreHardnessReadings WHERE
(Id='measured' AND HardnessValue='82' AND ReadingTimeSeconds='' AND SpecimenThicknessMm='10') OR
(Id='custom' AND ReadingTimeSeconds='30' AND SpecimenThicknessMm='10') OR
(Id='customThickness' AND ReadingTimeSeconds='' AND SpecimenThicknessMm='7');
""";
        if (Convert.ToInt32(command.ExecuteScalar()) != 3) return false;
        command.CommandText = "UPDATE ShoreHardnessReadings SET ReadingTimeSeconds='',SpecimenThicknessMm='10' WHERE Id='old'";
        command.ExecuteNonQuery();
        ApplyUnusedShoreTemplateRepair(connection);
        command.CommandText = "SELECT ReadingTimeSeconds || '|' || SpecimenThicknessMm FROM ShoreHardnessReadings WHERE Id='old'";
        return command.ExecuteScalar()?.ToString() == "|10";
    }
}
