using Microsoft.Data.Sqlite;
using System.Globalization;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace FilamentDbApp.Services.Reporting;

public sealed class PublicReportSourceFingerprintService
{
    public const string Schema = "3dpiceland.public-report-source-fingerprint.v1";
    public const string FileName = "source-fingerprint.json";

    private static readonly (string Name, string Sql)[] CanonicalQueries =
    {
        ("NativeMaterialManagerRows", """
            SELECT MaterialId, Manufacturer, ProductLine, MarketingName, BaseMaterialId, BaseMaterial, MaterialCategory,
                   VariantFinish, Reinforcement, Color, DiameterMm, SpoolWeightG, MsrpUsdPerKg,
                   ManufacturerWebsite, YouTubeReviewUrl, TestedStatus, InTensile, InImpact, InStiffness, InHeat,
                   SortOrder, WebsiteDisplayName, MaterialKey, PublishPublicReports,
                   PublishPublicTestDetails, IsArchived
            FROM NativeMaterialManagerRows
            ORDER BY MaterialId COLLATE NOCASE
            """),
        ("BaseMaterialCatalog", """
            SELECT BaseMaterialId, NozzleTemperatureMinC, NozzleTemperatureRecommendedC,
                   NozzleTemperatureMaxC, BedTemperatureMinC, BedTemperatureRecommendedC,
                   BedTemperatureMaxC, PrintSpeedMinMmPerS, PrintSpeedRecommendedMmPerS,
                   PrintSpeedMaxMmPerS, CoolingMinPercent, CoolingRecommendedPercent,
                   CoolingMaxPercent, CoolingGuidance, DryingTemperatureC, DryingTimeHours,
                   EnclosureRequirement, PrinterProfileReference, SlicerProfileReference
            FROM BaseMaterialCatalog
            ORDER BY BaseMaterialId
            """),
        ("FlexibleTestSessions", "SELECT * FROM FlexibleTestSessions ORDER BY FlexibleTestSessionId"),
        ("FlexibleTestSpecimens", "SELECT * FROM FlexibleTestSpecimens ORDER BY SpecimenId"),
        ("CompressionMeasurementPoints", "SELECT * FROM CompressionMeasurementPoints ORDER BY CompressionPointId"),
        ("StressRelaxationPoints", "SELECT * FROM StressRelaxationPoints ORDER BY RelaxationPointId"),
        ("RecoveryMeasurements", "SELECT * FROM RecoveryMeasurements ORDER BY RecoveryMeasurementId"),
        ("ShoreHardnessReadings", "SELECT * FROM ShoreHardnessReadings ORDER BY ShoreReadingId"),
        ("NativeTensileResults", "SELECT * FROM NativeTensileResults ORDER BY MaterialId COLLATE NOCASE"),
        ("NativeTensileSamples", """
            SELECT * FROM NativeTensileSamples
            ORDER BY MaterialId COLLATE NOCASE, Orientation COLLATE NOCASE, SampleNumber
            """),
        ("NativeImpactSamples", """
            SELECT * FROM NativeImpactSamples
            ORDER BY MaterialId COLLATE NOCASE, Orientation COLLATE NOCASE, SampleNumber
            """),
        ("NativeStiffnessMeasurements", """
            SELECT * FROM NativeStiffnessMeasurements
            ORDER BY MaterialId COLLATE NOCASE
            """),
        ("NativeMeasurementNotes", """
            SELECT * FROM NativeMeasurementNotes
            ORDER BY MaterialId COLLATE NOCASE, TestType COLLATE NOCASE
            """)
    };

    public static bool CanonicalQueriesUseNativeTables() =>
        CanonicalQueries.All(query =>
            query.Name.StartsWith("Native", StringComparison.Ordinal) ||
            string.Equals(query.Name, "BaseMaterialCatalog", StringComparison.Ordinal) ||
            query.Name is "FlexibleTestSessions" or "FlexibleTestSpecimens" or "CompressionMeasurementPoints" or
                "StressRelaxationPoints" or "RecoveryMeasurements" or "ShoreHardnessReadings");

    public string Compute(string databasePath, IEnumerable<string> publicMaterialIds, string canonicalReportProjection)
    {
        if (string.IsNullOrWhiteSpace(databasePath) || !File.Exists(databasePath))
            throw new InvalidOperationException("The canonical SQLite database is unavailable for public report source fingerprinting.");

        var canonical = new StringBuilder(32768);
        canonical.Append(Schema).Append('\n');
        foreach (var materialId in publicMaterialIds
                     .Where(value => !string.IsNullOrWhiteSpace(value))
                     .Select(value => value.Trim())
                     .Distinct(StringComparer.OrdinalIgnoreCase)
                     .OrderBy(value => value, StringComparer.OrdinalIgnoreCase))
        {
            AppendValue(canonical, "PublicMaterialID", materialId);
        }
        AppendValue(canonical, "CanonicalReportProjection", canonicalReportProjection ?? string.Empty);

        var connectionString = new SqliteConnectionStringBuilder
        {
            DataSource = databasePath,
            Mode = SqliteOpenMode.ReadOnly,
            Cache = SqliteCacheMode.Shared
        }.ToString();
        using var connection = new SqliteConnection(connectionString);
        connection.Open();

        AppendQueryRows(canonical, connection, CanonicalQueries);
        return Digest(canonical);
    }

    private static string Digest(StringBuilder canonical) =>
        Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString()))).ToLowerInvariant();

    private static void AppendQueryRows(StringBuilder canonical, SqliteConnection connection,
        IEnumerable<(string Name, string Sql)> queries)
    {
        foreach (var query in queries)
        {
            canonical.Append("Table:").Append(query.Name).Append('\n');
            using var command = connection.CreateCommand();
            command.CommandText = query.Sql;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                for (var index = 0; index < reader.FieldCount; index++)
                    AppendValue(canonical, reader.GetName(index), DatabaseValue(reader, index));
                canonical.Append("RowEnd\n");
            }
        }
    }

    /// <summary>Exercises actual Flexible SELECT queries and canonical hashing with synthetic in-memory SQLite only.</summary>
    public static bool VerifyFlexibleSourceFreshness()
    {
        (string Name, string Id)[] tables =
        [
            ("FlexibleTestSessions", "FlexibleTestSessionId"), ("FlexibleTestSpecimens", "SpecimenId"),
            ("CompressionMeasurementPoints", "CompressionPointId"), ("StressRelaxationPoints", "RelaxationPointId"),
            ("RecoveryMeasurements", "RecoveryMeasurementId"), ("ShoreHardnessReadings", "ShoreReadingId")
        ];
        var queries = CanonicalQueries.Where(query => tables.Any(table => table.Name == query.Name)).ToArray();
        if (queries.Length != tables.Length) return false;
        using var connection = new SqliteConnection("Data Source=:memory:");
        connection.Open();
        void Execute(string sql)
        {
            using var command = connection.CreateCommand();
            command.CommandText = sql;
            command.ExecuteNonQuery();
        }
        string Fingerprint()
        {
            var canonical = new StringBuilder();
            AppendQueryRows(canonical, connection, queries);
            return Digest(canonical);
        }
        foreach (var table in tables)
        {
            Execute($"CREATE TABLE {table.Name} ({table.Id} TEXT PRIMARY KEY, RawValue TEXT, IsActive INTEGER);");
            Execute($"INSERT INTO {table.Name} VALUES ('synthetic', '10', 1);");
        }
        var baseline = Fingerprint();
        foreach (var table in tables)
        {
            Execute($"UPDATE {table.Name} SET RawValue = '11';");
            if (Fingerprint() == baseline) return false;
            Execute($"UPDATE {table.Name} SET RawValue = '10';");
            if (Fingerprint() != baseline) return false;
        }
        Execute("UPDATE FlexibleTestSessions SET IsActive = 0;");
        if (Fingerprint() == baseline) return false;
        Execute("UPDATE FlexibleTestSessions SET IsActive = 1;");
        Execute("DELETE FROM RecoveryMeasurements;");
        if (Fingerprint() == baseline) return false;
        Execute("INSERT INTO RecoveryMeasurements VALUES ('synthetic', '10', 1);");
        Execute("UPDATE ShoreHardnessReadings SET RawValue = NULL;");
        if (Fingerprint() == baseline) return false;
        Execute("UPDATE ShoreHardnessReadings SET RawValue = '10';");
        return Fingerprint() == baseline;
    }
    public string BuildMetadataJson(string fingerprint, int publicMaterials, DateTime generatedAt) =>
        JsonSerializer.Serialize(new PublicReportSourceFingerprintRecord
        {
            Schema = Schema,
            SourceFingerprint = fingerprint,
            PublicMaterials = publicMaterials,
            GeneratedAtUtc = generatedAt.ToUniversalTime().ToString("O", CultureInfo.InvariantCulture)
        }, new JsonSerializerOptions { WriteIndented = true, PropertyNamingPolicy = JsonNamingPolicy.CamelCase });

    public string ReadFingerprint(string path)
    {
        try
        {
            if (!File.Exists(path)) return string.Empty;
            return ReadFingerprintJson(File.ReadAllText(path));
        }
        catch (IOException)
        {
            return string.Empty;
        }
    }

    public string ReadFingerprintJson(string json)
    {
        try
        {
            var record = JsonSerializer.Deserialize<PublicReportSourceFingerprintRecord>(json, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });
            return record is not null && string.Equals(record.Schema, Schema, StringComparison.Ordinal)
                ? record.SourceFingerprint
                : string.Empty;
        }
        catch (JsonException)
        {
            return string.Empty;
        }
    }

    public static bool Matches(string currentFingerprint, string storedFingerprint) =>
        !string.IsNullOrWhiteSpace(currentFingerprint) &&
        string.Equals(currentFingerprint, storedFingerprint, StringComparison.Ordinal);

    private static void AppendValue(StringBuilder builder, string name, string value) =>
        builder.Append(name.Length).Append(':').Append(name).Append('=')
            .Append(value.Length).Append(':').Append(value).Append('\n');

    private static string DatabaseValue(SqliteDataReader reader, int index)
    {
        if (reader.IsDBNull(index)) return "<null>";
        var value = reader.GetValue(index);
        return value switch
        {
            byte[] bytes => Convert.ToHexString(bytes),
            IFormattable formattable => formattable.ToString(null, CultureInfo.InvariantCulture) ?? string.Empty,
            _ => value.ToString() ?? string.Empty
        };
    }
}

public sealed class PublicReportSourceFingerprintRecord
{
    public string Schema { get; init; } = string.Empty;
    public string SourceFingerprint { get; init; } = string.Empty;
    public int PublicMaterials { get; init; }
    public string GeneratedAtUtc { get; init; } = string.Empty;
}
