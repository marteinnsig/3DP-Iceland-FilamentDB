using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Data.Sqlite;
using IODirectory = System.IO.Directory;
using IOFile = System.IO.File;
using IOFileInfo = System.IO.FileInfo;
using IOFileStream = System.IO.FileStream;
using IOPath = System.IO.Path;

namespace FilamentDbApp.DemoDatasetTool;

internal static class DemoDatasetInspector
{
    private const string ManifestSchema = "3dpiceland-public-demo-inspection-v1";
    private const string ContractVersion = "v56.0.2";
    private const int RequiredSchemaVersion = 38;
    private const int RequiredAllowlistCount = 36;
    private const string RequiredAllowlistSha256 =
        "A26A8406F2219DE035AEE6F24ECE3676037FD7880C01266009CACBF027BA9A7B";
    private const string RequiredSchemaObjectSha256 =
        "036C390C4D7FC6239776F14BF03ACD16E59ADECF78DCBE011B7A7E821BEA41B9";

    private static readonly IReadOnlyDictionary<string, string> TableModes =
        new SortedDictionary<string, string>(StringComparer.Ordinal)
        {
            ["AppMeta"] = "TRANSFORM",
            ["BaseMaterialCatalog"] = "TRANSFORM",
            ["DeploymentSettings"] = "EMPTY",
            ["ExperimentDefinitions"] = "EMPTY",
            ["ExperimentalMeasurements"] = "EMPTY",
            ["ExperimentalRuns"] = "EMPTY",
            ["InventorySpoolItems"] = "EMPTY",
            ["Manufacturers"] = "TRANSFORM",
            ["MaterialExperiments"] = "EMPTY",
            ["NativeImpactSamples"] = "RETAIN",
            ["NativeMaterialManagerRows"] = "TRANSFORM",
            ["NativeMeasurementNotes"] = "EMPTY",
            ["NativeSettingsRows"] = "TRANSFORM",
            ["NativeStiffnessMeasurements"] = "RETAIN",
            ["NativeTensileResults"] = "RECOMPUTE",
            ["NativeTensileSamples"] = "RETAIN",
            ["PrintJobQuotes"] = "EMPTY",
            ["PrinterProfiles"] = "EMPTY",
            ["PurchaseDocuments"] = "EMPTY",
            ["PurchaseOrderLines"] = "EMPTY",
            ["PurchaseOrders"] = "EMPTY",
            ["Suppliers"] = "EMPTY",
            ["UsageEvents"] = "EMPTY",
            ["VideoIdeaQueue"] = "EMPTY",
            ["WebsiteTemplates"] = "EMPTY"
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> TransformColumns =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            ["AppMeta"] = Set("Key", "Value"),
            ["Manufacturers"] = Set("ManufacturerId", "Name"),
            ["BaseMaterialCatalog"] = Set(
                "BaseMaterialId", "BaseMaterial", "Category", "SortOrder"),
            ["NativeMaterialManagerRows"] = Set(
                "MaterialId", "Manufacturer", "ProductLine", "MarketingName",
                "BaseMaterial", "MaterialCategory", "VariantFinish",
                "Reinforcement", "Color", "DiameterMm", "TestedStatus",
                "InTensile", "InImpact", "InStiffness", "InHeat", "SortOrder",
                "WebsiteDisplayName", "IsArchived", "UpdatedAtUtc",
                "PublishPublicReports", "PublishPublicTestDetails",
                "NozzleTemperatureMinC", "NozzleTemperatureRecommendedC",
                "NozzleTemperatureMaxC", "BedTemperatureMinC",
                "BedTemperatureRecommendedC", "BedTemperatureMaxC",
                "PrintSpeedMinMmPerS", "PrintSpeedRecommendedMmPerS",
                "PrintSpeedMaxMmPerS", "CoolingRequirement",
                "DryingTimeHours", "EnclosureRequirement",
                "CoolingMinPercent", "CoolingRecommendedPercent",
                "CoolingMaxPercent", "DryingTemperatureC",
                "PrintingSettingsProvenance", "PrintingSettingsCheckedDate",
                "ManufacturerId", "BaseMaterialId"),
            ["NativeTensileSamples"] = Set("UpdatedAtUtc"),
            ["NativeTensileResults"] = Set("MaterialId", "UpdatedAtUtc"),
            ["NativeImpactSamples"] = Set("UpdatedAtUtc"),
            ["NativeStiffnessMeasurements"] = Set("UpdatedAtUtc")
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> RetainColumns =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            ["NativeTensileSamples"] =
                Set("MaterialId", "Orientation", "SampleNumber", "RawValue"),
            ["NativeImpactSamples"] =
                Set("MaterialId", "Orientation", "SampleNumber", "RawValue"),
            ["NativeStiffnessMeasurements"] =
                Set("MaterialId", "Revolutions", "Degrees")
        };

    private static readonly IReadOnlyDictionary<string, IReadOnlySet<string>> ExcludeColumns =
        new Dictionary<string, IReadOnlySet<string>>(StringComparer.Ordinal)
        {
            ["NativeTensileResults"] = Set("TestNotes")
        };

    public static int Run(string[] args)
    {
        if (args.Length == 1 && string.Equals(args[0], "self-test", StringComparison.Ordinal))
            return RunSelfTest();

        var options = Parse(args);
        var result = Inspect(options);
        WriteManifest(options.OutputPath, result);
        Console.WriteLine($"Inspection result: {result.OverallResult}");
        Console.WriteLine($"Manifest SHA-256: {result.ManifestSha256}");
        Console.WriteLine("Manifest written inside the governed inspection root.");
        return string.Equals(result.OverallResult, "PASS", StringComparison.Ordinal) ? 0 : 2;
    }

    private static InspectionOptions Parse(string[] args)
    {
        if (args.Length != 9 || !string.Equals(args[0], "inspect", StringComparison.Ordinal))
            throw new ArgumentException("ARGS");

        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 1; index < args.Length; index += 2)
        {
            if (!values.TryAdd(args[index], args[index + 1]))
                throw new ArgumentException("ARGS");
        }

        if (!values.TryGetValue("--source", out var source) ||
            !values.TryGetValue("--allowlist", out var allowlist) ||
            !values.TryGetValue("--output", out var output) ||
            !values.TryGetValue("--inspection-root", out var inspectionRoot))
            throw new ArgumentException("ARGS");

        var sourcePath = IOPath.GetFullPath(source);
        var allowlistPath = IOPath.GetFullPath(allowlist);
        var outputPath = IOPath.GetFullPath(output);
        var rootPath = IOPath.GetFullPath(inspectionRoot);
        if (string.Equals(sourcePath, allowlistPath, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(sourcePath, outputPath, StringComparison.OrdinalIgnoreCase) ||
            string.Equals(allowlistPath, outputPath, StringComparison.OrdinalIgnoreCase))
            throw new InvalidOperationException("PATH_ALIAS");
        if (!IOFile.Exists(sourcePath) || !IOFile.Exists(allowlistPath))
            throw new FileNotFoundException("INPUT_MISSING");
        if (IOFile.Exists(outputPath))
            throw new IOException("OUTPUT_EXISTS");
        if (!string.Equals(IOPath.GetExtension(sourcePath), ".bak", StringComparison.OrdinalIgnoreCase) &&
            !string.Equals(IOPath.GetExtension(sourcePath), ".sqlite", StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("SOURCE_EXTENSION");
        if ((IOFile.GetAttributes(sourcePath) & FileAttributes.ReparsePoint) != 0 ||
            (IOFile.GetAttributes(allowlistPath) & FileAttributes.ReparsePoint) != 0)
            throw new InvalidDataException("INPUT_REPARSE");
        if (!IODirectory.Exists(rootPath) ||
            (IOFile.GetAttributes(rootPath) & FileAttributes.ReparsePoint) != 0 ||
            !string.Equals(IOPath.GetFileName(rootPath), "v56-source-inspection",
                StringComparison.Ordinal) ||
            !string.Equals(IOPath.GetFileName(IOPath.GetDirectoryName(rootPath)),
                "artifacts", StringComparison.Ordinal) ||
            !string.Equals(IOPath.GetDirectoryName(sourcePath), rootPath,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(IOPath.GetDirectoryName(allowlistPath), rootPath,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(IOPath.GetDirectoryName(outputPath), rootPath,
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("INSPECTION_CONTAINMENT");

        return new InspectionOptions(sourcePath, allowlistPath, outputPath, rootPath);
    }

    private static InspectionManifest Inspect(InspectionOptions options)
    {
        var failures = new SortedSet<string>(StringComparer.Ordinal);
        var sourceBefore = FileSha256(options.SourcePath);
        var sourceBytes = new IOFileInfo(options.SourcePath).Length;
        var allowlistBytes = IOFile.ReadAllBytes(options.AllowlistPath);
        var allowlistHash = Convert.ToHexString(SHA256.HashData(allowlistBytes));
        var allowlist = ParseAllowlist(allowlistBytes, failures);
        if (!FixedEquals(allowlistHash, RequiredAllowlistSha256))
            failures.Add("ALLOWLIST_REGISTRY_HASH");
        if (!FixedEquals(sourceBefore, allowlist.SourceSha256))
            failures.Add("ALLOWLIST_SOURCE_HASH");
        if (allowlist.SourceSchemaVersion != RequiredSchemaVersion)
            failures.Add("ALLOWLIST_SOURCE_SCHEMA");

        var objectInventory = new List<SchemaObject>();
        var tableResults = new List<TableInspection>();
        var relationship = new RelationshipInspection(0, 0, 0, 0, 0, 0, 0);
        var integrityResult = "NOT_RUN";
        var foreignKeyViolations = -1;
        var schemaVersion = -1;
        var queryOnly = false;

        using (var connection = OpenImmutable(options.SourcePath))
        {
            queryOnly = ScalarLong(connection, "PRAGMA query_only;") == 1;
            if (!queryOnly) failures.Add("SQLITE_NOT_QUERY_ONLY");

            integrityResult = ScalarText(connection, "PRAGMA integrity_check;");
            if (!string.Equals(integrityResult, "ok", StringComparison.Ordinal))
                failures.Add("SQLITE_INTEGRITY");
            foreignKeyViolations = CountRows(connection, "PRAGMA foreign_key_check;");
            if (foreignKeyViolations != 0) failures.Add("SQLITE_FOREIGN_KEY");

            schemaVersion = ReadSchemaVersion(connection);
            if (schemaVersion != RequiredSchemaVersion) failures.Add("SCHEMA_VERSION");

            objectInventory.AddRange(ReadSchemaObjects(connection));
            var actualTables = objectInventory
                .Where(item => item.Type == "table")
                .Select(item => item.Name)
                .ToHashSet(StringComparer.Ordinal);
            if (!actualTables.SetEquals(TableModes.Keys))
                failures.Add("SCHEMA_TABLE_SET");
            if (objectInventory.Any(item => item.Type is not ("table" or "index" or "trigger")))
                failures.Add("SCHEMA_OBJECT_TYPE");

            foreach (var policy in TableModes)
            {
                if (!actualTables.Contains(policy.Key)) continue;
                var sourceCount = ScalarLong(
                    connection,
                    $"SELECT COUNT(*) FROM {QuoteIdentifier(policy.Key)};");
                var candidateCount = CandidateCount(
                    connection,
                    policy.Key,
                    policy.Value,
                    allowlist.SourceIds);
                var columnCounts = ClassifyColumns(
                    policy.Key,
                    policy.Value,
                    ReadColumnNames(connection, policy.Key));
                tableResults.Add(new TableInspection(
                    policy.Key,
                    policy.Value,
                    sourceCount,
                    candidateCount,
                    policy.Value == "EMPTY" ? sourceCount : sourceCount - candidateCount,
                    columnCounts));
            }

            relationship = InspectRelationships(connection, allowlist.SourceIds, failures);
            ValidateExpectedCoverage(connection, allowlist, relationship, failures);
            if (CountInvalidRetainedMeasurements(
                    connection,
                    allowlist.SourceIds) != 0)
                failures.Add("PROJECTED_CONTENT");
        }

        var sourceAfter = FileSha256(options.SourcePath);
        if (!FixedEquals(sourceBefore, sourceAfter)) failures.Add("SOURCE_CHANGED");
        var schemaHash = ComputeSchemaHash(objectInventory);
        if (!FixedEquals(schemaHash, RequiredSchemaObjectSha256))
            failures.Add("SCHEMA_OBJECT_HASH");
        var manifest = new InspectionManifest(
            ManifestSchema,
            ContractVersion,
            "DRY-RUN",
            failures.Count == 0 ? "PASS" : "FAIL",
            sourceBytes,
            sourceBefore,
            sourceAfter,
            schemaVersion,
            schemaHash,
            queryOnly,
            integrityResult,
            foreignKeyViolations,
            allowlist.Version,
            allowlistHash,
            allowlist.SourceIds.Count,
            objectInventory.Count(item => item.Type == "table"),
            objectInventory.Count(item => item.Type == "index"),
            objectInventory.Count(item => item.Type == "trigger"),
            tableResults.OrderBy(item => item.Table, StringComparer.Ordinal).ToArray(),
            relationship,
            failures.ToArray(),
            string.Empty);
        return manifest with { ManifestSha256 = ComputeManifestHash(manifest) };
    }

    private static PrivateAllowlist ParseAllowlist(byte[] bytes, ISet<string> failures)
    {
        PrivateAllowlistDocument? value;
        try
        {
            value = JsonSerializer.Deserialize<PrivateAllowlistDocument>(bytes, JsonOptions());
        }
        catch (JsonException)
        {
            throw new InvalidDataException("ALLOWLIST_JSON");
        }
        if (value is null ||
            !string.Equals(value.ContractVersion, "v56.0.1", StringComparison.Ordinal) ||
            !value.OwnerApproval.ExactAllowlistApproved ||
            !value.OwnerApproval.RealMeasurementReidentificationRiskAccepted)
            throw new InvalidDataException("ALLOWLIST_CONTRACT");

        var sourceIds = value.Entries.Select(item => item.SourceMaterialId).ToArray();
        var demoIds = value.Entries.Select(item => item.DemoMaterialId).ToArray();
        if (sourceIds.Length != RequiredAllowlistCount) failures.Add("ALLOWLIST_COUNT");
        if (sourceIds.Distinct(StringComparer.Ordinal).Count() != sourceIds.Length ||
            sourceIds.Distinct(StringComparer.OrdinalIgnoreCase).Count() != sourceIds.Length)
            failures.Add("ALLOWLIST_SOURCE_DUPLICATE");
        if (demoIds.Distinct(StringComparer.Ordinal).Count() != demoIds.Length ||
            demoIds.Distinct(StringComparer.OrdinalIgnoreCase).Count() != demoIds.Length)
            failures.Add("ALLOWLIST_DEMO_DUPLICATE");
        for (var index = 0; index < value.Entries.Count; index++)
        {
            var entry = value.Entries[index];
            if (!string.Equals(
                    entry.DemoMaterialId,
                    $"DEMO-MAT-{index + 1:000}",
                    StringComparison.Ordinal))
                failures.Add("ALLOWLIST_DEMO_SEQUENCE");
            if (string.IsNullOrWhiteSpace(entry.SourceMaterialId) ||
                !string.Equals(
                    entry.SourceMaterialId,
                    entry.SourceMaterialId.Trim(),
                    StringComparison.Ordinal))
                failures.Add("ALLOWLIST_SOURCE_FORMAT");
            if (!entry.Approved ||
                entry.ArchivedApproved ||
                string.IsNullOrWhiteSpace(entry.ManufacturerGroup) ||
                string.IsNullOrWhiteSpace(entry.ProductFamilyGroup) ||
                string.IsNullOrWhiteSpace(entry.BaseMaterial) ||
                string.IsNullOrWhiteSpace(entry.RiskReason) ||
                !entry.ApprovedDomains.OrderBy(item => item, StringComparer.Ordinal)
                    .SequenceEqual(
                        new[] { "IMPACT", "STIFFNESS", "TENSILE" },
                        StringComparer.Ordinal))
                failures.Add("ALLOWLIST_ENTRY_CONTRACT");
        }
        if (value.Entries.Select(item => item.ManufacturerGroup)
                .Distinct(StringComparer.Ordinal).Count() !=
            value.Expected.ManufacturerGroupCount ||
            value.Entries.Select(item => item.BaseMaterial)
                .Distinct(StringComparer.Ordinal).Count() !=
            value.Expected.BaseMaterialCount)
            failures.Add("ALLOWLIST_GROUP_CONTRACT");
        return new PrivateAllowlist(
            value.AllowlistVersion,
            value.Source.Sha256,
            value.Source.SchemaVersion,
            value.Expected,
            sourceIds.ToHashSet(StringComparer.Ordinal));
    }

    private static SqliteConnection OpenImmutable(string path)
    {
        var uriPath = new Uri(path).AbsoluteUri;
        var connection = new SqliteConnection(
            $"Data Source={uriPath}?immutable=1;Mode=ReadOnly;Pooling=False");
        connection.Open();
        using var command = connection.CreateCommand();
        command.CommandText = "PRAGMA query_only=ON;";
        command.ExecuteNonQuery();
        return connection;
    }

    private static IReadOnlyList<SchemaObject> ReadSchemaObjects(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            """
            SELECT type, name, tbl_name, COALESCE(sql, '')
            FROM sqlite_master
            WHERE name NOT LIKE 'sqlite_%'
            ORDER BY type, name;
            """;
        using var reader = command.ExecuteReader();
        var values = new List<SchemaObject>();
        while (reader.Read())
        {
            values.Add(new SchemaObject(
                reader.GetString(0),
                reader.GetString(1),
                reader.GetString(2),
                NormalizeSql(reader.GetString(3))));
        }
        return values;
    }

    private static IReadOnlyList<string> ReadColumnNames(
        SqliteConnection connection,
        string table)
    {
        using var command = connection.CreateCommand();
        command.CommandText = $"PRAGMA table_info({QuoteIdentifier(table)});";
        using var reader = command.ExecuteReader();
        var values = new List<string>();
        while (reader.Read()) values.Add(reader.GetString(1));
        return values;
    }

    private static ColumnClassification ClassifyColumns(
        string table,
        string mode,
        IReadOnlyCollection<string> columns)
    {
        var retained = RetainColumns.TryGetValue(table, out var retain)
            ? retain
            : Set();
        var transformed = TransformColumns.TryGetValue(table, out var transform)
            ? transform
            : Set();
        var explicitlyExcluded = ExcludeColumns.TryGetValue(table, out var exclude)
            ? exclude
            : Set();
        var retainCount = columns.Count(retained.Contains);
        var transformCount = columns.Count(transformed.Contains);
        var recomputeCount = mode == "RECOMPUTE"
            ? columns.Count - transformCount - columns.Count(explicitlyExcluded.Contains)
            : 0;
        var excludedCount =
            columns.Count - retainCount - transformCount - recomputeCount;
        return new ColumnClassification(
            columns.Count,
            retainCount,
            transformCount,
            recomputeCount,
            excludedCount);
    }

    private static long CandidateCount(
        SqliteConnection connection,
        string table,
        string mode,
        IReadOnlySet<string> sourceIds)
    {
        if (mode == "EMPTY") return 0;
        // SchemaVersion alone would let first startup mutate the demo.
        // The builder owns four readiness markers plus one explicit public-demo marker.
        if (table is "AppMeta") return 5;
        if (table is "Manufacturers")
            return CountDistinctParents(connection, sourceIds, "ManufacturerId");
        if (table is "BaseMaterialCatalog")
            return CountDistinctParents(connection, sourceIds, "BaseMaterialId");
        if (table is "NativeMaterialManagerRows" or
            "NativeTensileSamples" or
            "NativeTensileResults" or
            "NativeImpactSamples" or
            "NativeStiffnessMeasurements")
            return CountForMaterials(connection, table, sourceIds);
        if (table is "NativeSettingsRows") return 0;
        return 0;
    }

    private static RelationshipInspection InspectRelationships(
        SqliteConnection connection,
        IReadOnlySet<string> sourceIds,
        ISet<string> failures)
    {
        var materialCount = CountForMaterials(connection, "NativeMaterialManagerRows", sourceIds);
        var manufacturerCount = CountDistinctParents(connection, sourceIds, "ManufacturerId");
        var baseMaterialCount = CountDistinctParents(connection, sourceIds, "BaseMaterialId");
        var tensileCount = CountForMaterials(connection, "NativeTensileSamples", sourceIds);
        var tensileResultCount = CountForMaterials(connection, "NativeTensileResults", sourceIds);
        var impactCount = CountForMaterials(connection, "NativeImpactSamples", sourceIds);
        var stiffnessCount = CountForMaterials(connection, "NativeStiffnessMeasurements", sourceIds);

        if (materialCount != sourceIds.Count) failures.Add("CLOSURE_MATERIAL");
        if (manufacturerCount == 0) failures.Add("CLOSURE_MANUFACTURER");
        if (baseMaterialCount == 0) failures.Add("CLOSURE_BASE_MATERIAL");
        if (tensileResultCount != sourceIds.Count) failures.Add("CLOSURE_TENSILE_RESULT");
        if (CountSelectedWithoutChildren(connection, sourceIds, "NativeTensileSamples") != 0)
            failures.Add("CLOSURE_TENSILE_SAMPLE");
        if (CountSelectedWithoutChildren(connection, sourceIds, "NativeImpactSamples") != 0)
            failures.Add("CLOSURE_IMPACT_SAMPLE");
        if (CountSelectedWithoutChildren(connection, sourceIds, "NativeStiffnessMeasurements") != 0)
            failures.Add("CLOSURE_STIFFNESS");
        if (CountMissingParentLinks(connection, sourceIds) != 0)
            failures.Add("CLOSURE_PARENT_LINK");

        return new RelationshipInspection(
            materialCount,
            manufacturerCount,
            baseMaterialCount,
            tensileCount,
            tensileResultCount,
            impactCount,
            stiffnessCount);
    }

    private static void ValidateExpectedCoverage(
        SqliteConnection connection,
        PrivateAllowlist allowlist,
        RelationshipInspection actual,
        ISet<string> failures)
    {
        var expected = allowlist.Expected;
        if (actual.MaterialCount != RequiredAllowlistCount ||
            actual.ManufacturerCount != expected.ManufacturerGroupCount ||
            actual.BaseMaterialCount != expected.BaseMaterialCount ||
            actual.TensileSampleCount != expected.TensileSampleCount ||
            actual.TensileResultCount != expected.TensileResultCount ||
            actual.ImpactSampleCount != expected.ImpactSampleCount ||
            actual.StiffnessRowCount != expected.StiffnessRowCount)
            failures.Add("CLOSURE_EXPECTED_COUNTS");

        using var command = connection.CreateCommand();
        var parameters = AddMaterialParameters(command, allowlist.SourceIds);
        command.CommandText =
            $"SELECT COUNT(*) FROM NativeMaterialManagerRows " +
            $"WHERE MaterialId IN ({parameters}) AND IsArchived <> 0;";
        var archived = Convert.ToInt64(
            command.ExecuteScalar(),
            CultureInfo.InvariantCulture);
        if (archived != expected.ArchivedMaterialCount)
            failures.Add("CLOSURE_ARCHIVED");
    }

    private static int CountInvalidRetainedMeasurements(
        SqliteConnection connection,
        IReadOnlySet<string> sourceIds)
    {
        var invalid = 0;
        invalid += CountInvalidNumericColumn(
            connection, sourceIds, "NativeTensileSamples", "RawValue");
        invalid += CountInvalidNumericColumn(
            connection, sourceIds, "NativeImpactSamples", "RawValue");
        invalid += CountInvalidNumericColumn(
            connection, sourceIds, "NativeStiffnessMeasurements", "Revolutions");
        invalid += CountInvalidNumericColumn(
            connection, sourceIds, "NativeStiffnessMeasurements", "Degrees");
        return invalid;
    }

    private static int CountInvalidNumericColumn(
        SqliteConnection connection,
        IReadOnlySet<string> sourceIds,
        string table,
        string column)
    {
        using var command = connection.CreateCommand();
        var parameters = AddMaterialParameters(command, sourceIds);
        command.CommandText =
            $"SELECT {QuoteIdentifier(column)} FROM {QuoteIdentifier(table)} " +
            $"WHERE MaterialId IN ({parameters});";
        using var reader = command.ExecuteReader();
        var invalid = 0;
        while (reader.Read())
        {
            var text = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
            if (!decimal.TryParse(
                    text,
                    NumberStyles.Number,
                    CultureInfo.InvariantCulture,
                    out _))
                invalid++;
        }
        return invalid;
    }

    private static long CountForMaterials(
        SqliteConnection connection,
        string table,
        IReadOnlySet<string> sourceIds)
    {
        using var command = connection.CreateCommand();
        var parameters = AddMaterialParameters(command, sourceIds);
        command.CommandText =
            $"SELECT COUNT(*) FROM {QuoteIdentifier(table)} WHERE MaterialId IN ({parameters});";
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static long CountDistinctParents(
        SqliteConnection connection,
        IReadOnlySet<string> sourceIds,
        string column)
    {
        using var command = connection.CreateCommand();
        var parameters = AddMaterialParameters(command, sourceIds);
        command.CommandText =
            $"SELECT COUNT(DISTINCT {QuoteIdentifier(column)}) " +
            $"FROM NativeMaterialManagerRows WHERE MaterialId IN ({parameters});";
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static long CountSelectedWithoutChildren(
        SqliteConnection connection,
        IReadOnlySet<string> sourceIds,
        string childTable)
    {
        using var command = connection.CreateCommand();
        var parameters = AddMaterialParameters(command, sourceIds);
        command.CommandText =
            $"""
             SELECT COUNT(*)
             FROM NativeMaterialManagerRows material
             WHERE material.MaterialId IN ({parameters})
               AND NOT EXISTS (
                   SELECT 1 FROM {QuoteIdentifier(childTable)} child
                   WHERE child.MaterialId = material.MaterialId);
             """;
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static long CountMissingParentLinks(
        SqliteConnection connection,
        IReadOnlySet<string> sourceIds)
    {
        using var command = connection.CreateCommand();
        var parameters = AddMaterialParameters(command, sourceIds);
        command.CommandText =
            $"""
             SELECT COUNT(*)
             FROM NativeMaterialManagerRows material
             LEFT JOIN Manufacturers manufacturer
               ON manufacturer.ManufacturerId = material.ManufacturerId
             LEFT JOIN BaseMaterialCatalog baseMaterial
               ON baseMaterial.BaseMaterialId = material.BaseMaterialId
             WHERE material.MaterialId IN ({parameters})
               AND (manufacturer.ManufacturerId IS NULL OR baseMaterial.BaseMaterialId IS NULL);
             """;
        return Convert.ToInt64(command.ExecuteScalar(), CultureInfo.InvariantCulture);
    }

    private static string AddMaterialParameters(
        SqliteCommand command,
        IReadOnlySet<string> sourceIds)
    {
        var names = new List<string>();
        var index = 0;
        foreach (var sourceId in sourceIds.OrderBy(value => value, StringComparer.Ordinal))
        {
            var name = $"$material{index++}";
            command.Parameters.AddWithValue(name, sourceId);
            names.Add(name);
        }
        return string.Join(",", names);
    }

    private static int ReadSchemaVersion(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText = "SELECT Value FROM AppMeta WHERE Key='SchemaVersion';";
        return int.TryParse(
            command.ExecuteScalar()?.ToString(),
            NumberStyles.None,
            CultureInfo.InvariantCulture,
            out var value)
            ? value
            : -1;
    }

    private static string ComputeSchemaHash(IEnumerable<SchemaObject> objects)
    {
        var builder = new StringBuilder();
        foreach (var item in objects.OrderBy(value => value.Type, StringComparer.Ordinal)
                     .ThenBy(value => value.Name, StringComparer.Ordinal))
        {
            Append(builder, item.Type);
            Append(builder, item.Name);
            Append(builder, item.Table);
            Append(builder, item.Sql);
        }
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(builder.ToString())));
    }

    private static string ComputeManifestHash(InspectionManifest manifest)
    {
        var canonical = JsonSerializer.Serialize(
            manifest with { ManifestSha256 = string.Empty },
            JsonOptions());
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonical)));
    }

    private static void Append(StringBuilder builder, string value) =>
        builder.Append(value.Length.ToString(CultureInfo.InvariantCulture))
            .Append(':')
            .Append(value);

    private static string NormalizeSql(string value) =>
        string.Join(' ', value.Split((char[]?)null, StringSplitOptions.RemoveEmptyEntries));

    private static long ScalarLong(SqliteConnection connection, string sql) =>
        Convert.ToInt64(Scalar(connection, sql), CultureInfo.InvariantCulture);

    private static string ScalarText(SqliteConnection connection, string sql) =>
        Convert.ToString(Scalar(connection, sql), CultureInfo.InvariantCulture) ?? string.Empty;

    private static object? Scalar(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        return command.ExecuteScalar();
    }

    private static int CountRows(SqliteConnection connection, string sql)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        using var reader = command.ExecuteReader();
        var count = 0;
        while (reader.Read()) count++;
        return count;
    }

    private static string QuoteIdentifier(string value) =>
        "\"" + value.Replace("\"", "\"\"", StringComparison.Ordinal) + "\"";

    private static IReadOnlySet<string> Set(params string[] values) =>
        values.ToHashSet(StringComparer.Ordinal);

    private static string FileSha256(string path)
    {
        using var stream = IOFile.OpenRead(path);
        return Convert.ToHexString(SHA256.HashData(stream));
    }

    private static bool FixedEquals(string left, string right)
    {
        try
        {
            return CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(left),
                Convert.FromHexString(right));
        }
        catch
        {
            return false;
        }
    }

    private static void WriteManifest(string path, InspectionManifest manifest)
    {
        var parent = IOPath.GetDirectoryName(path);
        if (string.IsNullOrWhiteSpace(parent) || !IODirectory.Exists(parent))
            throw new DirectoryNotFoundException("OUTPUT_PARENT");
        var bytes = JsonSerializer.SerializeToUtf8Bytes(manifest, JsonOptions());
        using var stream = new IOFileStream(
            path,
            FileMode.CreateNew,
            FileAccess.Write,
            FileShare.None);
        stream.Write(bytes);
    }

    private static JsonSerializerOptions JsonOptions() => new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow
    };

    private static int RunSelfTest()
    {
        var failures = new List<string>();
        var options = JsonOptions();
        var duplicateJson =
            """
            {"contractVersion":"v56.0.1","allowlistVersion":"test","ownerApproval":{"exactAllowlistApproved":true,
            "realMeasurementReidentificationRiskAccepted":true},"entries":[],"unexpected":true}
            """;
        try
        {
            JsonSerializer.Deserialize<PrivateAllowlistDocument>(duplicateJson, options);
            failures.Add("unknown-json-property");
        }
        catch (JsonException)
        {
        }

        var sample = new InspectionManifest(
            ManifestSchema, ContractVersion, "DRY-RUN", "PASS", 1, "AA", "AA",
            38, "BB", true, "ok", 0, "test", "CC", 36, 25, 17, 6, [],
            new RelationshipInspection(36, 10, 11, 712, 36, 718, 36), [], string.Empty);
        var first = ComputeManifestHash(sample);
        var second = ComputeManifestHash(sample);
        if (!FixedEquals(first, second)) failures.Add("manifest-hash");

        Console.WriteLine(failures.Count == 0
            ? "Demo dataset inspector self-test: PASS"
            : "Demo dataset inspector self-test: FAIL " + string.Join(",", failures));
        return failures.Count == 0 ? 0 : 1;
    }

    private sealed record InspectionOptions(
        string SourcePath,
        string AllowlistPath,
        string OutputPath,
        string InspectionRoot);

    private sealed record PrivateAllowlist(
        string Version,
        string SourceSha256,
        int SourceSchemaVersion,
        ExpectedDocument Expected,
        IReadOnlySet<string> SourceIds);

    private sealed record SchemaObject(string Type, string Name, string Table, string Sql);

    private sealed record TableInspection(
        string Table,
        string Classification,
        long SourceRowCount,
        long CandidateRowCount,
        long ExcludedRowCount,
        ColumnClassification Columns);

    private sealed record ColumnClassification(
        int Total,
        int Retain,
        int Transform,
        int Recompute,
        int Exclude);

    private sealed record RelationshipInspection(
        long MaterialCount,
        long ManufacturerCount,
        long BaseMaterialCount,
        long TensileSampleCount,
        long TensileResultCount,
        long ImpactSampleCount,
        long StiffnessRowCount);

    private sealed record InspectionManifest(
        string Schema,
        string ContractVersion,
        string Mode,
        string OverallResult,
        long SourceByteLength,
        string SourceSha256Before,
        string SourceSha256After,
        int SourceSchemaVersion,
        string SchemaObjectSha256,
        bool QueryOnly,
        string IntegrityResult,
        int ForeignKeyViolationCount,
        string AllowlistVersion,
        string AllowlistSha256,
        int AllowlistCount,
        int TableCount,
        int IndexCount,
        int TriggerCount,
        IReadOnlyList<TableInspection> Tables,
        RelationshipInspection Relationships,
        IReadOnlyList<string> FailureCodes,
        string ManifestSha256);

    private sealed class PrivateAllowlistDocument
    {
        public string ContractVersion { get; init; } = string.Empty;
        public string AllowlistVersion { get; init; } = string.Empty;
        public DateTimeOffset ApprovedAtUtc { get; init; }
        public SourceDocument Source { get; init; } = new();
        public OwnerApprovalDocument OwnerApproval { get; init; } = new();
        public ExpectedDocument Expected { get; init; } = new();
        public List<AllowlistEntryDocument> Entries { get; init; } = [];
    }

    private sealed class SourceDocument
    {
        public string Path { get; init; } = string.Empty;
        public string Sha256 { get; init; } = string.Empty;
        public int SchemaVersion { get; init; }
    }

    private sealed class OwnerApprovalDocument
    {
        public bool ExactAllowlistApproved { get; init; }
        public bool RealMeasurementReidentificationRiskAccepted { get; init; }
    }

    private sealed class AllowlistEntryDocument
    {
        public string DemoMaterialId { get; init; } = string.Empty;
        public string SourceMaterialId { get; init; } = string.Empty;
        public string ManufacturerGroup { get; init; } = string.Empty;
        public string ProductFamilyGroup { get; init; } = string.Empty;
        public string BaseMaterial { get; init; } = string.Empty;
        public List<string> ApprovedDomains { get; init; } = [];
        public bool ArchivedApproved { get; init; }
        public bool Approved { get; init; }
        public string RiskReason { get; init; } = string.Empty;
    }

    private sealed class ExpectedDocument
    {
        public int ManufacturerGroupCount { get; init; }
        public int BaseMaterialCount { get; init; }
        public int TensileSampleCount { get; init; }
        public int TensileResultCount { get; init; }
        public int ImpactSampleCount { get; init; }
        public int StiffnessRowCount { get; init; }
        public int ArchivedMaterialCount { get; init; }
    }
}

internal static class SafeError
{
    public static string Code(Exception exception) => exception switch
    {
        ArgumentException => "ARGS",
        FileNotFoundException => "INPUT_MISSING",
        DirectoryNotFoundException => "OUTPUT_PARENT",
        UnauthorizedAccessException => "ACCESS_DENIED",
        JsonException => "ALLOWLIST_JSON",
        InvalidDataException data => SafeToken(data.Message, "INVALID_DATA"),
        InvalidOperationException operation => SafeToken(operation.Message, "INVALID_OPERATION"),
        IOException io => SafeToken(io.Message, "IO_ERROR"),
        SqliteException => "SQLITE_ERROR",
        _ => "UNEXPECTED"
    };

    private static string SafeToken(string value, string fallback) =>
        value.Length is > 0 and <= 64 &&
        value.All(character => char.IsAsciiLetterOrDigit(character) || character == '_')
            ? value
            : fallback;
}
