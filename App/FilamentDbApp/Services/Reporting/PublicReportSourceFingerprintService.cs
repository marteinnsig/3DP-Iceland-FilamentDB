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
                   ManufacturerWebsite, YouTubeReviewUrl, TestedStatus, InTensile, InImpact, InStiffness,
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
            string.Equals(query.Name, "BaseMaterialCatalog", StringComparison.Ordinal));

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

        foreach (var query in CanonicalQueries)
        {
            canonical.Append("Table:").Append(query.Name).Append('\n');
            using var command = connection.CreateCommand();
            command.CommandText = query.Sql;
            using var reader = command.ExecuteReader();
            while (reader.Read())
            {
                for (var index = 0; index < reader.FieldCount; index++)
                {
                    AppendValue(canonical, reader.GetName(index), DatabaseValue(reader, index));
                }
                canonical.Append("RowEnd\n");
            }
        }

        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical.ToString()))).ToLowerInvariant();
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
