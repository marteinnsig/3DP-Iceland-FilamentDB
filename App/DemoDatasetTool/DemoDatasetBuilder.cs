using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using Microsoft.Data.Sqlite;
using IOFile = System.IO.File;
using IOFileStream = System.IO.FileStream;
using IODirectory = System.IO.Directory;
using IOPath = System.IO.Path;

namespace FilamentDbApp.DemoDatasetTool;

internal static class DemoDatasetBuilder
{
    private const string FixedUtc = "2026-01-01T00:00:00.0000000Z";
    private const string AllowlistHash =
        "A26A8406F2219DE035AEE6F24ECE3676037FD7880C01266009CACBF027BA9A7B";
    private const string TransformationHash =
        "1AED8DE62B74CFDFE34AFD8992BDBD4D1ED557EFA8515971EFDCCCBB3017017C";
    private const string SourceHash =
        "74943C492AE0FD06DABD7485D648222F87EE059343C2E3F6EAC298116F6D14F8";
    private const string SchemaHash =
        "036C390C4D7FC6239776F14BF03ACD16E59ADECF78DCBE011B7A7E821BEA41B9";

    private static readonly IReadOnlyDictionary<string, (string Category, int Sort)> Bases =
        new Dictionary<string, (string, int)>(StringComparer.Ordinal)
        {
            ["ABS"] = ("Engineering", 30), ["ASA"] = ("Engineering", 35),
            ["PA12"] = ("Engineering", 62), ["PA6"] = ("Engineering", 60),
            ["PC"] = ("Engineering", 40), ["PC/PBT"] = ("Engineering", 42),
            ["PCTG"] = ("Standard", 25), ["PET"] = ("Engineering", 27),
            ["PETG"] = ("Standard", 20), ["PLA"] = ("Standard", 10),
            ["PP"] = ("Engineering", 50)
        };

    private static readonly string[] EmptyTables =
    [
        "DeploymentSettings", "ExperimentDefinitions", "ExperimentalMeasurements",
        "ExperimentalRuns", "InventorySpoolItems", "MaterialExperiments",
        "NativeMeasurementNotes", "NativeSettingsRows", "PrintJobQuotes",
        "PrinterProfiles", "PurchaseDocuments", "PurchaseOrderLines",
        "PurchaseOrders", "Suppliers", "UsageEvents", "VideoIdeaQueue",
        "WebsiteTemplates"
    ];

    public static int Run(string[] args)
    {
        var options = Parse(args);
        var sourceBefore = HashFile(options.Source);
        try
        {
            RequireHash(sourceBefore, SourceHash, "SOURCE_HASH");
            RequireHash(HashFile(options.Allowlist), AllowlistHash, "ALLOWLIST_HASH");
            RequireHash(HashFile(options.Transformation), TransformationHash, "TRANSFORMATION_HASH");

            var contractA = LoadContract(options.Allowlist, options.Transformation);
            var payloadA = ReadPayload(options.Source, contractA);
            BuildOne(options.Source, options.OutputA, contractA, payloadA);
            var contractB = LoadContract(options.Allowlist, options.Transformation);
            var payloadB = ReadPayload(options.Source, contractB);
            BuildOne(options.Source, options.OutputB, contractB, payloadB);
            SqliteConnection.ClearAllPools();

            var hashA = HashFile(options.OutputA);
            var hashB = HashFile(options.OutputB);
            RequireHash(hashB, hashA, "BYTE_DRIFT");
            var inspectionA = InspectOutput(options.OutputA);
            var inspectionB = InspectOutput(options.OutputB);
            RequireHash(inspectionB.LogicalHash, inspectionA.LogicalHash, "LOGICAL_DRIFT");
            if (inspectionA.SchemaHash != SchemaHash || inspectionB.SchemaHash != SchemaHash)
                throw new InvalidDataException("OUTPUT_SCHEMA_HASH");
            if (inspectionA.PageSize != 4096 ||
                inspectionA.Encoding != "UTF-8" ||
                inspectionA.AutoVacuum != 0 ||
                !inspectionA.JournalMode.Equals("delete",
                    StringComparison.OrdinalIgnoreCase) ||
                inspectionB.PageSize != inspectionA.PageSize ||
                inspectionB.Encoding != inspectionA.Encoding ||
                inspectionB.AutoVacuum != inspectionA.AutoVacuum ||
                inspectionB.JournalMode != inspectionA.JournalMode ||
                inspectionB.SqliteVersion != inspectionA.SqliteVersion)
                throw new InvalidDataException("SQLITE_RUNTIME");
            RequireNoSidecars(options.Source, options.OutputA, options.OutputB);

            var manifest = new BuildManifest(
                "3dpiceland-public-demo-build-v1", "v56.0.4", "PASS",
                sourceBefore, AllowlistHash, TransformationHash, SchemaHash,
                hashA, hashB, inspectionA.LogicalHash, inspectionB.LogicalHash,
                inspectionA.TableRowCounts, inspectionA.TableSha256,
                inspectionA.FileBytes, inspectionA.Materials, inspectionA.Manufacturers,
                inspectionA.BaseMaterials, inspectionA.TensileSamples,
                inspectionA.TensileResults, inspectionA.ImpactSamples,
                inspectionA.StiffnessRows, inspectionA.EmptyDomainRows,
                inspectionA.Integrity, inspectionA.ForeignKeyViolations,
                $"Microsoft.Data.Sqlite 9.0.18 / SQLitePCLRaw 2.1.12 / SQLite {inspectionA.SqliteVersion}",
                inspectionA.PageSize, inspectionA.Encoding,
                inspectionA.AutoVacuum, inspectionA.JournalMode, string.Empty);
            manifest = manifest with { ManifestSha256 = HashManifest(manifest) };
            WriteNew(options.Manifest, JsonSerializer.SerializeToUtf8Bytes(
                manifest, JsonOptions()));
            Console.WriteLine("Demo dataset build: PASS");
            Console.WriteLine($"SQLite SHA-256: {hashA}");
            Console.WriteLine($"Logical SHA-256: {inspectionA.LogicalHash}");
            Console.WriteLine($"Manifest SHA-256: {manifest.ManifestSha256}");
            return 0;
        }
        finally
        {
            SqliteConnection.ClearAllPools();
            RequireHash(HashFile(options.Source), sourceBefore, "SOURCE_MUTATED");
        }
    }

    private static BuildOptions Parse(string[] args)
    {
        if (args.Length != 15 || args[0] != "build")
            throw new ArgumentException("ARGS");
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 1; index < args.Length; index += 2)
            if (!values.TryAdd(args[index], args[index + 1]))
                throw new ArgumentException("ARGS");
        string Get(string key) => values.TryGetValue(key, out var value)
            ? IOPath.GetFullPath(value) : throw new ArgumentException("ARGS");
        var root = Get("--inspection-root");
        var result = new BuildOptions(
            root, Get("--source"), Get("--allowlist"), Get("--transformation"),
            Get("--output-a"), Get("--output-b"), Get("--manifest"));
        if (!IODirectory.Exists(root) ||
            IOPath.GetFileName(root) != "v56-source-inspection" ||
            IOPath.GetFileName(IOPath.GetDirectoryName(root)) != "artifacts" ||
            (IOFile.GetAttributes(root) & FileAttributes.ReparsePoint) != 0)
            throw new InvalidDataException("BUILD_CONTAINMENT");
        var expectedRoot = IOPath.GetFullPath(IOPath.Combine(
            Environment.CurrentDirectory, "artifacts", "v56-source-inspection"));
        if (!string.Equals(root, expectedRoot, StringComparison.OrdinalIgnoreCase) ||
            HasReparseAncestor(root, Environment.CurrentDirectory))
            throw new InvalidDataException("BUILD_ROOT");
        foreach (var input in new[] { result.Source, result.Allowlist, result.Transformation })
            if (!IOFile.Exists(input) ||
                (IOFile.GetAttributes(input) & FileAttributes.ReparsePoint) != 0)
                throw new InvalidDataException("INPUT_INVALID");
        if (!string.Equals(IOPath.GetDirectoryName(result.Source), root,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(IOPath.GetDirectoryName(result.Allowlist), root,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(result.Transformation, IOPath.Combine(
                    Environment.CurrentDirectory, "App", "DemoDatasetTool",
                    "Contracts", "public-demo-transformation-v1.json"),
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("INPUT_LOCATION");
        if (!string.Equals(IOPath.GetFileName(result.Source),
                "source-manual-backup.bak", StringComparison.Ordinal) ||
            !string.Equals(IOPath.GetFileName(result.Allowlist),
                "private-allowlist.json", StringComparison.Ordinal))
            throw new InvalidDataException("INPUT_NAME");
        foreach (var output in new[] { result.OutputA, result.OutputB, result.Manifest })
            if (IOFile.Exists(output) ||
                !string.Equals(IOPath.GetDirectoryName(output), root,
                    StringComparison.OrdinalIgnoreCase) ||
                IOPath.GetFileName(output).Contains(':'))
                throw new InvalidDataException("OUTPUT_INVALID");
        if (new[] { result.OutputA, result.OutputB, result.Manifest }
            .Distinct(StringComparer.OrdinalIgnoreCase).Count() != 3)
            throw new InvalidDataException("OUTPUT_ALIAS");
        return result;
    }

    private static Contract LoadContract(string allowlistPath, string transformPath)
    {
        using var allow = JsonDocument.Parse(IOFile.ReadAllBytes(allowlistPath));
        using var transform = JsonDocument.Parse(IOFile.ReadAllBytes(transformPath));
        var allowRoot = allow.RootElement;
        var approval = allowRoot.GetProperty("ownerApproval");
        var source = allowRoot.GetProperty("source");
        var expected = allowRoot.GetProperty("expected");
        if (allowRoot.GetProperty("contractVersion").GetString() != "v56.0.1" ||
            !approval.GetProperty("exactAllowlistApproved").GetBoolean() ||
            !approval.GetProperty("realMeasurementReidentificationRiskAccepted").GetBoolean() ||
            source.GetProperty("sha256").GetString() != SourceHash ||
            source.GetProperty("schemaVersion").GetInt32() != 38 ||
            expected.GetProperty("tensileSampleCount").GetInt32() != 712 ||
            expected.GetProperty("tensileResultCount").GetInt32() != 36 ||
            expected.GetProperty("impactSampleCount").GetInt32() != 718 ||
            expected.GetProperty("stiffnessRowCount").GetInt32() != 36 ||
            expected.GetProperty("archivedMaterialCount").GetInt32() != 0)
            throw new InvalidDataException("ALLOWLIST_APPROVAL");
        var allowEntries = allow.RootElement.GetProperty("entries")
            .EnumerateArray().ToDictionary(
                item => item.GetProperty("demoMaterialId").GetString()!,
                item => new AllowEntry(
                    item.GetProperty("sourceMaterialId").GetString()!,
                    item.GetProperty("manufacturerGroup").GetString()!,
                    item.GetProperty("productFamilyGroup").GetString()!,
                    item.GetProperty("baseMaterial").GetString()!),
                StringComparer.Ordinal);
        var entries = allowRoot.GetProperty("entries").EnumerateArray().ToArray();
        if (entries.Select(item => item.GetProperty("sourceMaterialId").GetString()!)
                .Distinct(StringComparer.OrdinalIgnoreCase).Count() != 36 ||
            entries.Select(item => item.GetProperty("demoMaterialId").GetString()!)
                .Distinct(StringComparer.OrdinalIgnoreCase).Count() != 36 ||
            entries.Any(item =>
                !item.GetProperty("approved").GetBoolean() ||
                item.GetProperty("archivedApproved").GetBoolean() ||
                !item.GetProperty("approvedDomains").EnumerateArray()
                    .Select(value => value.GetString()!)
                    .OrderBy(value => value, StringComparer.Ordinal)
                    .SequenceEqual(new[] { "IMPACT", "STIFFNESS", "TENSILE" },
                        StringComparer.Ordinal)))
            throw new InvalidDataException("ALLOWLIST_ENTRIES");
        var materials = new List<MaterialIdentity>();
        var index = 0;
        foreach (var item in transform.RootElement.GetProperty("materials").EnumerateArray())
        {
            index++;
            var id = item.GetProperty("materialId").GetString()!;
            if (id != $"DEMO-MAT-{index:000}" || !Canonical(id))
                throw new InvalidDataException("MATERIAL_SEQUENCE");
            if (!allowEntries.TryGetValue(id, out var approved))
                throw new InvalidDataException("ALLOWLIST_CLOSURE");
            var manufacturerGroup = item.GetProperty("manufacturerGroup").GetString()!;
            var familyGroup = item.GetProperty("productFamilyGroup").GetString()!;
            var baseMaterial = item.GetProperty("baseMaterial").GetString()!;
            if (manufacturerGroup != approved.ManufacturerGroup ||
                familyGroup != approved.ProductFamilyGroup ||
                baseMaterial != approved.BaseMaterial)
                throw new InvalidDataException("PRIVATE_GROUP_PARITY");
            var mfrOrdinal = ParseOrdinal(manufacturerGroup, "DEMO-MFR-");
            var familyOrdinal = ParseOrdinal(familyGroup, "DEMO-FAM-");
            var manufacturer = $"Fictional Manufacturer {mfrOrdinal:00}";
            var line = $"Demo Line {familyOrdinal:00}";
            var marketing = $"Engineering Sample {index:000}";
            var variant = item.GetProperty("variantFinish").GetString()!;
            var reinforcement = item.GetProperty("reinforcement").GetString()!;
            var color = item.GetProperty("color").GetString()!;
            if (new[]
                {
                    manufacturerGroup, familyGroup, baseMaterial, variant,
                    reinforcement, color
                }.Any(value => !Canonical(value)))
                throw new InvalidDataException("TRANSFORM_CANONICAL");
            var display = Join(manufacturer, line, marketing, baseMaterial,
                variant, reinforcement, color);
            var key = string.Join("|", baseMaterial, variant, reinforcement,
                color, manufacturer, line, marketing);
            materials.Add(new MaterialIdentity(
                id, approved.SourceId, 560000 + mfrOrdinal, manufacturer,
                line, marketing, 0, baseMaterial, variant, reinforcement,
                color, display, key, index - 1));
        }
        var baseOrder = transform.RootElement.GetProperty("baseMaterials")
            .EnumerateArray().Select(item => item.GetString()!).ToArray();
        materials = materials.Select(item => item with
        {
            BaseMaterialId = 561001 + Array.IndexOf(baseOrder, item.BaseMaterial)
        }).ToList();
        var stableRowIndexes = materials
            .OrderBy(item => Bases[item.BaseMaterial].Sort)
            .ThenBy(item => item.MaterialId, StringComparer.Ordinal)
            .Select((item, rowIndex) => (item.MaterialId, rowIndex))
            .ToDictionary(item => item.MaterialId, item => item.rowIndex,
                StringComparer.Ordinal);
        materials = materials.Select(item => item with
        {
            RowIndex = stableRowIndexes[item.MaterialId]
        }).ToList();
        if (materials.Count != 36 || allowEntries.Count != 36)
            throw new InvalidDataException("MATERIAL_COUNT");
        if (materials.GroupBy(item => item.ProductLine, StringComparer.Ordinal)
            .Any(group => group.Select(item => item.ManufacturerId)
                .Distinct().Count() != 1))
            throw new InvalidDataException("FAMILY_PARENT");
        return new Contract(materials);
    }

    private static Payload ReadPayload(string sourcePath, Contract contract)
    {
        using var connection = OpenImmutable(sourcePath);
        var sourceMap = contract.Materials.ToDictionary(
            item => item.SourceId, StringComparer.Ordinal);
        var tensile = ReadSamples(connection, "NativeTensileSamples", sourceMap);
        var impact = ReadSamples(connection, "NativeImpactSamples", sourceMap);
        var stiffness = ReadStiffness(connection, sourceMap);
        var expectedTensile = ReadExpectedTensile(connection, sourceMap);
        if (tensile.Count != 712 || impact.Count != 718 || stiffness.Count != 36)
            throw new InvalidDataException("SOURCE_COUNTS");
        if (sourceMap.Keys.Any(id => !tensile.Any(row => row.MaterialId == sourceMap[id].MaterialId) ||
                                     !impact.Any(row => row.MaterialId == sourceMap[id].MaterialId) ||
                                     !stiffness.Any(row => row.MaterialId == sourceMap[id].MaterialId)))
            throw new InvalidDataException("SOURCE_CLOSURE");
        VerifyTensileParity(contract, tensile, expectedTensile);
        return new Payload(tensile, impact, stiffness, expectedTensile);
    }

    private static List<ExpectedTensile> ReadExpectedTensile(
        SqliteConnection connection,
        IReadOnlyDictionary<string, MaterialIdentity> sourceMap)
    {
        using var command = connection.CreateCommand();
        var parameters = AddIds(command, sourceMap.Keys);
        command.CommandText = """
            SELECT MaterialId,UprightMpa,FlatMpa,StdDevUpright,StdDevFlat,
                   CvUpright,CvFlat,SamplesUpright,SamplesFlat,
                   ConfidenceUpright,ConfidenceFlat
            FROM NativeTensileResults
            WHERE MaterialId IN (
            """ + parameters + ") ORDER BY MaterialId COLLATE BINARY;";
        using var reader = command.ExecuteReader();
        var rows = new List<ExpectedTensile>();
        while (reader.Read())
        {
            var sourceId = reader.GetString(0);
            rows.Add(new ExpectedTensile(
                sourceMap[sourceId].MaterialId,
                Enumerable.Range(1, 10).Select(index =>
                    reader.IsDBNull(index) ? "" : reader.GetString(index)).ToArray()));
        }
        return rows;
    }

    private static void VerifyTensileParity(
        Contract contract, IReadOnlyList<Sample> samples,
        IReadOnlyList<ExpectedTensile> expectedRows)
    {
        if (expectedRows.Count != 36)
            throw new InvalidDataException("TENSILE_RESULT_COUNT");
        var expected = expectedRows.ToDictionary(
            item => item.MaterialId, StringComparer.Ordinal);
        foreach (var material in contract.Materials)
        {
            var rows = samples.Where(item => item.MaterialId == material.MaterialId)
                .ToList();
            var upright = CalculateSet(rows.Where(item =>
                item.Orientation.Equals("Upright", StringComparison.OrdinalIgnoreCase)));
            var flat = CalculateSet(rows.Where(item =>
                item.Orientation.Equals("Flat", StringComparison.OrdinalIgnoreCase)));
            var actual = new[]
            {
                upright.Average, flat.Average, upright.StdDev, flat.StdDev,
                upright.Cv, flat.Cv, upright.Count, flat.Count,
                upright.Confidence, flat.Confidence
            };
            if (!expected.TryGetValue(material.MaterialId, out var source) ||
                actual.Where((value, index) =>
                        !EquivalentResult(value, source.Values[index], index >= 6))
                    .Any())
                throw new InvalidDataException("TENSILE_APP_PARITY");
        }
    }

    private static bool EquivalentResult(
        string actual, string expected, bool wholeNumber)
    {
        if (actual.Length == 0 || expected.Trim().Length == 0)
            return actual.Length == 0 && expected.Trim().Length == 0;
        if (wholeNumber)
            return int.TryParse(expected.Trim(), NumberStyles.Integer,
                       CultureInfo.InvariantCulture, out var integer) &&
                   actual == integer.ToString(CultureInfo.InvariantCulture);
        return TryParseFlexibleDouble(expected, out var number) &&
               actual == FormatResult(number);
    }

    private static bool TryParseFlexibleDouble(string value, out double number)
    {
        var text = value.Trim();
        if (text.Contains(',') && !text.Contains('.'))
            text = text.Replace(',', '.');
        return double.TryParse(text, NumberStyles.Float,
            CultureInfo.InvariantCulture, out number) &&
            !double.IsNaN(number) && !double.IsInfinity(number);
    }

    private static List<Sample> ReadSamples(
        SqliteConnection connection, string table,
        IReadOnlyDictionary<string, MaterialIdentity> sourceMap)
    {
        using var command = connection.CreateCommand();
        var parameters = AddIds(command, sourceMap.Keys);
        command.CommandText =
            $"SELECT MaterialId,Orientation,SampleNumber,RawValue FROM {table} " +
            $"WHERE MaterialId IN ({parameters}) ORDER BY MaterialId COLLATE BINARY," +
            "Orientation COLLATE BINARY,SampleNumber;";
        using var reader = command.ExecuteReader();
        var rows = new List<Sample>();
        while (reader.Read())
        {
            var sourceId = reader.GetString(0);
            rows.Add(new Sample(sourceMap[sourceId].MaterialId,
                reader.GetString(1), reader.GetInt32(2),
                CanonicalDecimal(reader.GetString(3))));
        }
        return rows;
    }

    private static List<Stiffness> ReadStiffness(
        SqliteConnection connection,
        IReadOnlyDictionary<string, MaterialIdentity> sourceMap)
    {
        using var command = connection.CreateCommand();
        var parameters = AddIds(command, sourceMap.Keys);
        command.CommandText =
            "SELECT MaterialId,Revolutions,Degrees FROM NativeStiffnessMeasurements " +
            $"WHERE MaterialId IN ({parameters}) ORDER BY MaterialId COLLATE BINARY;";
        using var reader = command.ExecuteReader();
        var rows = new List<Stiffness>();
        while (reader.Read())
        {
            var sourceId = reader.GetString(0);
            rows.Add(new Stiffness(sourceMap[sourceId].MaterialId,
                CanonicalDecimal(reader.GetString(1)),
                CanonicalDecimal(reader.GetString(2))));
        }
        return rows;
    }

    private static void BuildOne(
        string sourcePath, string outputPath, Contract contract, Payload payload)
    {
        using (new IOFileStream(outputPath, FileMode.CreateNew,
                   FileAccess.Write, FileShare.None)) { }
        var cs = new SqliteConnectionStringBuilder
        {
            DataSource = outputPath, Mode = SqliteOpenMode.ReadWrite,
            Cache = SqliteCacheMode.Private, Pooling = false
        }.ToString();
        using var output = new SqliteConnection(cs);
        output.Open();
        Execute(output, "PRAGMA page_size=4096; PRAGMA encoding='UTF-8'; " +
            "PRAGMA auto_vacuum=NONE; PRAGMA journal_mode=DELETE; PRAGMA foreign_keys=ON;");
        CreateSchemaFromPinnedSource(sourcePath, output);
        using (var transaction = output.BeginTransaction())
        {
            InsertMeta(output, transaction);
            InsertManufacturers(output, transaction, contract);
            InsertBases(output, transaction, contract);
            InsertMaterials(output, transaction, contract, payload);
            InsertSamples(output, transaction, "NativeTensileSamples", payload.Tensile);
            InsertTensileResults(output, transaction, contract, payload.Tensile);
            InsertSamples(output, transaction, "NativeImpactSamples", payload.Impact);
            InsertStiffness(output, transaction, payload.Stiffness);
            Execute(output, transaction, "DELETE FROM sqlite_sequence;");
            transaction.Commit();
        }
        ValidateDatabase(output);
        Execute(output, "VACUUM;");
        ValidateDatabase(output);
    }

    private static void CreateSchemaFromPinnedSource(
        string sourcePath, SqliteConnection output)
    {
        using var source = OpenImmutable(sourcePath);
        var objects = ReadSchema(source);
        if (ComputeSchemaHash(objects) != SchemaHash)
            throw new InvalidDataException("SOURCE_SCHEMA_HASH");
        foreach (var item in objects.Where(item => item.Type == "table")
                     .OrderBy(item => item.Name, StringComparer.Ordinal))
            Execute(output, item.Sql);
        foreach (var item in objects.Where(item => item.Type == "index")
                     .OrderBy(item => item.Name, StringComparer.Ordinal))
            Execute(output, item.Sql);
        foreach (var item in objects.Where(item => item.Type == "trigger")
                     .OrderBy(item => item.Name, StringComparer.Ordinal))
            Execute(output, item.Sql);
    }

    private static List<SchemaItem> ReadSchema(SqliteConnection connection)
    {
        using var command = connection.CreateCommand();
        command.CommandText =
            "SELECT type,name,tbl_name,sql FROM sqlite_schema " +
            "WHERE sql IS NOT NULL AND name NOT LIKE 'sqlite_%' " +
            "ORDER BY type,name;";
        using var reader = command.ExecuteReader();
        var rows = new List<SchemaItem>();
        while (reader.Read())
            rows.Add(new SchemaItem(reader.GetString(0), reader.GetString(1),
                reader.GetString(2), reader.GetString(3)));
        return rows;
    }

    private static void InsertMeta(SqliteConnection db, SqliteTransaction tx)
    {
        foreach (var row in new[]
        {
            ("SchemaVersion", "38"),
            ("NativeMeasurementsCanonicalV1", "complete"),
            ("LegacyWorkbookTablesRetiredV1", "complete"),
            ("LegacyWorkbookPostRetirementBackupV1", "complete")
        })
            Insert(db, tx, "INSERT INTO AppMeta(Key,Value) VALUES($a,$b);",
                row.Item1, row.Item2);
    }

    private static void InsertManufacturers(
        SqliteConnection db, SqliteTransaction tx, Contract contract)
    {
        foreach (var row in contract.Materials.GroupBy(item => item.ManufacturerId)
                     .Select(group => group.First()).OrderBy(item => item.ManufacturerId))
            Insert(db, tx, """
                INSERT INTO Manufacturers(
                  ManufacturerId,Name,Website,DisplayName,Country,Founded,LogoUrl,
                  Description,EngineeringFocus,MaterialCategories,Strengths,
                  Weaknesses,Sustainability,TypicalApplications,Headquarters,
                  Notes,SortOrder,IsActive,CreatedAtUtc,UpdatedAtUtc)
                VALUES($a,$b,'',$b,'','','','','','','','','','','','',$c,1,$d,$d);
                """, row.ManufacturerId, row.Manufacturer,
                row.ManufacturerId - 560000, FixedUtc);
    }

    private static void InsertBases(
        SqliteConnection db, SqliteTransaction tx, Contract contract)
    {
        foreach (var row in contract.Materials.GroupBy(item => item.BaseMaterialId)
                     .Select(group => group.First()).OrderBy(item => item.BaseMaterialId))
        {
            var definition = Bases[row.BaseMaterial];
            Insert(db, tx, """
                INSERT INTO BaseMaterialCatalog(
                  BaseMaterial,Category,SortOrder,UpdatedAtUtc,BaseMaterialId)
                VALUES($a,$b,$c,$d,$e);
                """, row.BaseMaterial, definition.Category,
                definition.Sort.ToString(CultureInfo.InvariantCulture),
                FixedUtc, row.BaseMaterialId);
        }
    }

    private static void InsertMaterials(
        SqliteConnection db, SqliteTransaction tx, Contract contract, Payload payload)
    {
        foreach (var row in contract.Materials.OrderBy(item => item.MaterialId,
                     StringComparer.Ordinal))
        {
            var hasTensile = payload.Tensile.Any(item => item.MaterialId == row.MaterialId);
            var hasImpact = payload.Impact.Any(item => item.MaterialId == row.MaterialId);
            var hasStiffness = payload.Stiffness.Any(item => item.MaterialId == row.MaterialId);
            var testedCount = new[] { hasTensile, hasImpact, hasStiffness }
                .Count(value => value);
            var tested = testedCount switch
            {
                0 => "Not tested",
                3 => "Fully tested",
                _ => "Partially tested"
            };
            var definition = Bases[row.BaseMaterial];
            var sort = (definition.Sort + ((row.RowIndex + 3) / 1000d))
                .ToString("0.###", CultureInfo.InvariantCulture);
            Insert(db, tx, """
                INSERT INTO NativeMaterialManagerRows(
                  MaterialId,ManufacturerId,Manufacturer,ProductLine,MarketingName,
                  BaseMaterialId,BaseMaterial,MaterialCategory,VariantFinish,
                  Reinforcement,Color,DiameterMm,Video,TestedStatus,InTensile,
                  InImpact,InStiffness,SortOrder,SourcePriority,WebsiteDisplayName,
                  MaterialKey,PublishPublicReports,PublishPublicTestDetails,
                  IsArchived,UpdatedAtUtc)
                VALUES($a,$b,$c,$d,$e,$f,$g,$h,$i,$j,$k,'','No',$l,$m,$n,$o,
                  $p,'Materials master',$q,$r,0,0,0,$s);
                """, row.MaterialId, row.ManufacturerId, row.Manufacturer,
                row.ProductLine, row.MarketingName, row.BaseMaterialId,
                row.BaseMaterial, definition.Category, row.VariantFinish,
                row.Reinforcement, row.Color, tested,
                hasTensile ? "Yes" : "No", hasImpact ? "Yes" : "No",
                hasStiffness ? "Yes" : "No", sort, row.DisplayName,
                row.MaterialKey, FixedUtc);
        }
    }

    private static void InsertSamples(
        SqliteConnection db, SqliteTransaction tx, string table,
        IReadOnlyList<Sample> rows)
    {
        foreach (var row in rows)
            Insert(db, tx,
                $"INSERT INTO {table}(MaterialId,Orientation,SampleNumber,RawValue,UpdatedAtUtc) " +
                "VALUES($a,$b,$c,$d,$e);",
                row.MaterialId, row.Orientation, row.Number, row.RawValue, FixedUtc);
    }

    private static void InsertTensileResults(
        SqliteConnection db, SqliteTransaction tx, Contract contract,
        IReadOnlyList<Sample> samples)
    {
        foreach (var material in contract.Materials.OrderBy(item => item.MaterialId,
                     StringComparer.Ordinal))
        {
            var rows = samples.Where(item => item.MaterialId == material.MaterialId).ToList();
            var upright = CalculateSet(rows.Where(item =>
                item.Orientation.Equals("Upright", StringComparison.OrdinalIgnoreCase)));
            var flat = CalculateSet(rows.Where(item =>
                item.Orientation.Equals("Flat", StringComparison.OrdinalIgnoreCase)));
            Insert(db, tx, """
                INSERT INTO NativeTensileResults(
                  MaterialId,UprightMpa,FlatMpa,StdDevUpright,StdDevFlat,
                  CvUpright,CvFlat,SamplesUpright,SamplesFlat,
                  ConfidenceUpright,ConfidenceFlat,TestNotes,UpdatedAtUtc)
                VALUES($a,$b,$c,$d,$e,$f,$g,$h,$i,$j,$k,'',$l);
                """, material.MaterialId, upright.Average, flat.Average,
                upright.StdDev, flat.StdDev, upright.Cv, flat.Cv,
                upright.Count, flat.Count, upright.Confidence, flat.Confidence,
                FixedUtc);
        }
    }

    private static MeasurementSet CalculateSet(IEnumerable<Sample> rows)
    {
        var values = rows.Select(item =>
                double.Parse(item.RawValue, CultureInfo.InvariantCulture) / 6.5016d)
            .ToList();
        if (values.Count == 0) return new("", "", "", "", "");
        var average = values.Average();
        double? standardDeviation = values.Count < 2 ? null :
            Math.Sqrt(values.Sum(value => Math.Pow(value - average, 2)) /
                      (values.Count - 1));
        var cv = standardDeviation / average;
        return new MeasurementSet(
            FormatResult(average), FormatResult(standardDeviation),
            FormatResult(cv), values.Count.ToString(CultureInfo.InvariantCulture),
            Math.Min(values.Count, 10).ToString(CultureInfo.InvariantCulture));
    }

    private static void InsertStiffness(
        SqliteConnection db, SqliteTransaction tx, IReadOnlyList<Stiffness> rows)
    {
        foreach (var row in rows)
            Insert(db, tx, """
                INSERT INTO NativeStiffnessMeasurements(
                  MaterialId,Revolutions,Degrees,TestNotes,UpdatedAtUtc)
                VALUES($a,$b,$c,'',$d);
                """, row.MaterialId, row.Revolutions, row.Degrees, FixedUtc);
    }

    private static void ValidateDatabase(SqliteConnection db)
    {
        if (Scalar(db, "PRAGMA integrity_check;") != "ok")
            throw new InvalidDataException("INTEGRITY");
        if (Convert.ToInt32(Scalar(db,
                "SELECT COUNT(*) FROM pragma_foreign_key_check;"),
                CultureInfo.InvariantCulture) != 0)
            throw new InvalidDataException("FOREIGN_KEY");
        foreach (var table in EmptyTables)
            if (Convert.ToInt64(Scalar(db, $"SELECT COUNT(*) FROM {table};"),
                    CultureInfo.InvariantCulture) != 0)
                throw new InvalidDataException("EMPTY_DOMAIN");
        if (Scalar(db, "SELECT COUNT(*) FROM AppMeta;") != "4" ||
            Scalar(db, "SELECT COUNT(*) FROM NativeMaterialManagerRows;") != "36" ||
            Scalar(db, "SELECT COUNT(*) FROM Manufacturers;") != "10" ||
            Scalar(db, "SELECT COUNT(*) FROM BaseMaterialCatalog;") != "11" ||
            Scalar(db, "SELECT COUNT(*) FROM NativeTensileSamples;") != "712" ||
            Scalar(db, "SELECT COUNT(*) FROM NativeTensileResults;") != "36" ||
            Scalar(db, "SELECT COUNT(*) FROM NativeImpactSamples;") != "718" ||
            Scalar(db, "SELECT COUNT(*) FROM NativeStiffnessMeasurements;") != "36")
            throw new InvalidDataException("OUTPUT_COUNTS");
        if (Convert.ToInt32(Scalar(db, """
                SELECT COUNT(*) FROM AppMeta
                WHERE (Key='SchemaVersion' AND Value='38')
                   OR (Key='NativeMeasurementsCanonicalV1' AND Value='complete')
                   OR (Key='LegacyWorkbookTablesRetiredV1' AND Value='complete')
                   OR (Key='LegacyWorkbookPostRetirementBackupV1' AND Value='complete');
                """), CultureInfo.InvariantCulture) != 4)
            throw new InvalidDataException("APPMETA_CONTRACT");
        if (Convert.ToInt64(Scalar(db, """
                SELECT COUNT(*) FROM NativeMaterialManagerRows m
                LEFT JOIN Manufacturers f ON f.ManufacturerId=m.ManufacturerId
                LEFT JOIN BaseMaterialCatalog b ON b.BaseMaterialId=m.BaseMaterialId
                WHERE f.Name<>m.Manufacturer OR b.BaseMaterial<>m.BaseMaterial
                   OR m.PublishPublicReports<>0 OR m.PublishPublicTestDetails<>0
                   OR m.IsArchived<>0;
                """), CultureInfo.InvariantCulture) != 0)
            throw new InvalidDataException("OUTPUT_PARITY");
        if (Convert.ToInt64(Scalar(db, $"""
                SELECT COUNT(*) FROM NativeMaterialManagerRows
                WHERE MaterialId NOT GLOB 'DEMO-MAT-[0-9][0-9][0-9]'
                   OR ManufacturerId NOT BETWEEN 560001 AND 560010
                   OR Manufacturer <> printf('Fictional Manufacturer %02d',
                                              ManufacturerId-560000)
                   OR ProductLine NOT GLOB 'Demo Line [0-9][0-9]'
                   OR MarketingName <> printf('Engineering Sample %03d',
                                              CAST(substr(MaterialId,10,3) AS INTEGER))
                   OR BaseMaterialId NOT BETWEEN 561001 AND 561011
                   OR Color NOT GLOB 'Demo Color [0-9][0-9]'
                   OR CAST(substr(Color,12,2) AS INTEGER) NOT BETWEEN 1 AND 10
                   OR (COALESCE(VariantFinish,'')<>''
                       AND (VariantFinish NOT GLOB 'Demo Variant [0-9][0-9]'
                            OR CAST(substr(VariantFinish,14,2) AS INTEGER)
                               NOT BETWEEN 1 AND 4))
                   OR COALESCE(Reinforcement,'') NOT IN ('','CF','GF')
                   OR SourcePriority <> 'Materials master'
                   OR TestedStatus <> 'Fully tested'
                   OR InTensile <> 'Yes' OR InImpact <> 'Yes'
                   OR InStiffness <> 'Yes' OR Video <> 'No'
                   OR UpdatedAtUtc <> '{FixedUtc}'
                   OR WebsiteDisplayName <>
                      trim(Manufacturer||' '||ProductLine||' '||MarketingName||' '||
                           BaseMaterial||
                           CASE WHEN COALESCE(VariantFinish,'')='' THEN '' ELSE ' '||VariantFinish END||
                           CASE WHEN COALESCE(Reinforcement,'')='' THEN '' ELSE ' '||Reinforcement END||
                           ' '||Color)
                   OR MaterialKey <> BaseMaterial||'|'||COALESCE(VariantFinish,'')||
                      '|'||COALESCE(Reinforcement,'')||'|'||Color||'|'||
                      Manufacturer||'|'||ProductLine||'|'||MarketingName;
                """), CultureInfo.InvariantCulture) != 0)
            throw new InvalidDataException("MATERIAL_DERIVATION");
        if (Convert.ToInt64(Scalar(db, """
                WITH ordered AS (
                  SELECT m.MaterialId, CAST(m.SortOrder AS REAL) AS Actual,
                         CAST(b.SortOrder AS REAL) +
                         ((ROW_NUMBER() OVER (
                           ORDER BY CAST(b.SortOrder AS REAL),m.MaterialId
                         ) + 2) / 1000.0) AS Expected
                  FROM NativeMaterialManagerRows m
                  JOIN BaseMaterialCatalog b
                    ON b.BaseMaterialId=m.BaseMaterialId
                )
                SELECT COUNT(*) FROM ordered WHERE ABS(Actual-Expected)>0.0000001;
                """), CultureInfo.InvariantCulture) != 0)
            throw new InvalidDataException("SORT_PARITY");
        if (Convert.ToInt64(Scalar(db, $"""
                SELECT
                  (SELECT COUNT(*) FROM NativeTensileSamples
                   WHERE MaterialId NOT GLOB 'DEMO-MAT-[0-9][0-9][0-9]'
                      OR Orientation NOT IN ('Flat','Upright')
                      OR SampleNumber NOT BETWEEN 1 AND 10
                      OR COALESCE(RawValue,'')=''
                      OR UpdatedAtUtc<>'{FixedUtc}')
                + (SELECT COUNT(*) FROM NativeImpactSamples
                   WHERE MaterialId NOT GLOB 'DEMO-MAT-[0-9][0-9][0-9]'
                      OR Orientation NOT IN ('Flat','Upright')
                      OR SampleNumber NOT BETWEEN 1 AND 10
                      OR COALESCE(RawValue,'')=''
                      OR UpdatedAtUtc<>'{FixedUtc}')
                + (SELECT COUNT(*) FROM NativeTensileResults
                   WHERE MaterialId NOT GLOB 'DEMO-MAT-[0-9][0-9][0-9]'
                      OR COALESCE(TestNotes,'')<>''
                      OR UpdatedAtUtc<>'{FixedUtc}')
                + (SELECT COUNT(*) FROM NativeStiffnessMeasurements
                   WHERE MaterialId NOT GLOB 'DEMO-MAT-[0-9][0-9][0-9]'
                      OR COALESCE(Revolutions,'')=''
                      OR COALESCE(Degrees,'')=''
                      OR COALESCE(TestNotes,'')<>''
                      OR UpdatedAtUtc<>'{FixedUtc}');
                """), CultureInfo.InvariantCulture) != 0)
            throw new InvalidDataException("MEASUREMENT_BOUNDARY");
        if (Convert.ToInt64(Scalar(db, """
                SELECT COUNT(*) FROM NativeMaterialManagerRows
                WHERE COALESCE(ManufacturerWebsite,'')<>'' OR
                      COALESCE(YouTubeReviewUrl,'')<>'' OR
                      COALESCE(Notes,'')<>'' OR COALESCE(ManufacturerSku,'')<>'' OR
                      COALESCE(PurchasePriceAmount,'')<>'' OR
                      COALESCE(PrintingSettingsSourceUrl,'')<>'';
                """), CultureInfo.InvariantCulture) != 0)
            throw new InvalidDataException("PRIVACY_VALUES");
        if (Count(db, "sqlite_sequence") != 0)
            throw new InvalidDataException("SQLITE_SEQUENCE");
        var objects = ReadSchema(db);
        if (objects.Count(item => item.Type == "table") != 25 ||
            objects.Count(item => item.Type == "index") != 17 ||
            objects.Count(item => item.Type == "trigger") != 6)
            throw new InvalidDataException("SCHEMA_OBJECT_COUNTS");
        ScanAllTextValues(db);
        RequireOnlyColumns(db, "NativeMaterialManagerRows", new[]
        {
            "MaterialId", "ManufacturerId", "Manufacturer", "ProductLine",
            "MarketingName", "BaseMaterialId", "BaseMaterial",
            "MaterialCategory", "VariantFinish", "Reinforcement", "Color",
            "Video", "TestedStatus", "InTensile", "InImpact", "InStiffness",
            "SortOrder", "SourcePriority", "WebsiteDisplayName", "MaterialKey",
            "PublishPublicReports", "PublishPublicTestDetails", "IsArchived",
            "UpdatedAtUtc"
        });
        RequireOnlyColumns(db, "Manufacturers", new[]
        {
            "ManufacturerId", "Name", "DisplayName", "SortOrder", "IsActive",
            "CreatedAtUtc", "UpdatedAtUtc"
        });
        RequireOnlyColumns(db, "BaseMaterialCatalog", new[]
        {
            "BaseMaterialId", "BaseMaterial", "Category", "SortOrder",
            "UpdatedAtUtc"
        });
        ValidateCatalogRows(db);
    }

    private static void ValidateCatalogRows(SqliteConnection db)
    {
        using var command = db.CreateCommand();
        command.CommandText =
            "SELECT BaseMaterialId,BaseMaterial,Category,SortOrder,UpdatedAtUtc " +
            "FROM BaseMaterialCatalog ORDER BY BaseMaterialId;";
        using var reader = command.ExecuteReader();
        var count = 0;
        while (reader.Read())
        {
            var id = reader.GetInt32(0);
            var name = reader.GetString(1);
            if (!Bases.TryGetValue(name, out var expected) ||
                id != 561001 + Array.IndexOf(Bases.Keys.ToArray(), name) ||
                reader.GetString(2) != expected.Category ||
                reader.GetString(3) != expected.Sort.ToString(
                    CultureInfo.InvariantCulture) ||
                reader.GetString(4) != FixedUtc)
                throw new InvalidDataException("BASE_CATALOG_PARITY");
            count++;
        }
        if (count != 11)
            throw new InvalidDataException("BASE_CATALOG_COUNT");
    }

    private static void RequireOnlyColumns(
        SqliteConnection db, string table, IEnumerable<string> allowedColumns)
    {
        var allowed = allowedColumns.ToHashSet(StringComparer.Ordinal);
        using var columns = db.CreateCommand();
        columns.CommandText = $"PRAGMA table_info(\"{table}\");";
        using var reader = columns.ExecuteReader();
        var excluded = new List<string>();
        while (reader.Read())
            if (!allowed.Contains(reader.GetString(1)))
                excluded.Add(reader.GetString(1));
        reader.Close();
        foreach (var column in excluded)
        {
            var quoted = column.Replace("\"", "\"\"");
            if (Convert.ToInt64(Scalar(db,
                    $"SELECT COUNT(*) FROM \"{table}\" WHERE " +
                    $"COALESCE(CAST(\"{quoted}\" AS TEXT),'')<>'';"),
                    CultureInfo.InvariantCulture) != 0)
                throw new InvalidDataException("COLUMN_BOUNDARY");
        }
    }

    private static void ScanAllTextValues(SqliteConnection db)
    {
        foreach (var table in ReadSchema(db)
                     .Where(item => item.Type == "table")
                     .Select(item => item.Name))
        {
            using var columns = db.CreateCommand();
            columns.CommandText = $"PRAGMA table_info({table});";
            using var columnReader = columns.ExecuteReader();
            var textColumns = new List<string>();
            while (columnReader.Read())
                if (columnReader.GetString(2).Contains(
                        "TEXT", StringComparison.OrdinalIgnoreCase))
                    textColumns.Add(columnReader.GetString(1));
            foreach (var column in textColumns)
            {
                using var command = db.CreateCommand();
                command.CommandText =
                    $"SELECT \"{column.Replace("\"", "\"\"")}\" FROM \"{table}\" " +
                    $"WHERE COALESCE(\"{column.Replace("\"", "\"\"")}\",'')<>'';";
                using var reader = command.ExecuteReader();
                while (reader.Read())
                {
                    var value = reader.GetString(0);
                    if (value.Contains("://", StringComparison.Ordinal) ||
                        value.Contains('\\') ||
                        Regex.IsMatch(value, @"(?i)\b[A-Z]:[/\\]|\S+@\S+|\bMAT\d{4}\b"))
                        throw new InvalidDataException("PRIVACY_TEXT");
                }
            }
        }
    }

    private static OutputInspection InspectOutput(string path)
    {
        var cs = new SqliteConnectionStringBuilder
        {
            DataSource = path, Mode = SqliteOpenMode.ReadOnly,
            Cache = SqliteCacheMode.Private, Pooling = false
        }.ToString();
        using var db = new SqliteConnection(cs);
        db.Open();
        ValidateDatabase(db);
        var tables = ReadSchema(db).Where(item => item.Type == "table")
            .Select(item => item.Name).OrderBy(item => item, StringComparer.Ordinal)
            .ToArray();
        var tableRows = new SortedDictionary<string, int>(StringComparer.Ordinal);
        var tableHashes = new SortedDictionary<string, string>(StringComparer.Ordinal);
        foreach (var table in tables)
        {
            var logical = new StringBuilder();
            using var command = db.CreateCommand();
            var orderBy = ReadPrimaryKeyOrder(db, table);
            command.CommandText = $"SELECT * FROM \"{table}\" ORDER BY {orderBy};";
            using var reader = command.ExecuteReader();
            for (var index = 0; index < reader.FieldCount; index++)
            {
                AppendHashValue(logical, "C");
                AppendHashValue(logical, reader.GetName(index));
            }
            var rowCount = 0;
            while (reader.Read())
            {
                rowCount++;
                AppendHashValue(logical, "R");
                for (var index = 0; index < reader.FieldCount; index++)
                {
                    if (reader.IsDBNull(index))
                    {
                        AppendHashValue(logical, "N");
                        continue;
                    }
                    switch (reader.GetValue(index))
                    {
                        case long integer:
                            AppendHashValue(logical, "I");
                            AppendHashValue(logical, integer.ToString(
                                CultureInfo.InvariantCulture));
                            break;
                        case double real:
                            AppendHashValue(logical, "R");
                            AppendHashValue(logical, real.ToString(
                                "R", CultureInfo.InvariantCulture));
                            break;
                        case byte[] blob:
                            AppendHashValue(logical, "B");
                            AppendHashValue(logical, Convert.ToBase64String(blob));
                            break;
                        default:
                            AppendHashValue(logical, "T");
                            AppendHashValue(logical, reader.GetString(index));
                            break;
                    }
                }
            }
            tableRows[table] = rowCount;
            tableHashes[table] = Sha256(Encoding.UTF8.GetBytes(logical.ToString()));
        }
        var aggregate = new StringBuilder();
        foreach (var item in tableHashes)
        {
            AppendHashValue(aggregate, item.Key);
            AppendHashValue(aggregate, tableRows[item.Key]
                .ToString(CultureInfo.InvariantCulture));
            AppendHashValue(aggregate, item.Value);
        }
        var emptyRows = EmptyTables.Sum(table => Convert.ToInt32(
            Scalar(db, $"SELECT COUNT(*) FROM {table};"), CultureInfo.InvariantCulture));
        return new OutputInspection(
            new FileInfo(path).Length, ComputeSchemaHash(ReadSchema(db)),
            Sha256(Encoding.UTF8.GetBytes(aggregate.ToString())),
            tableRows, tableHashes,
            Count(db, "NativeMaterialManagerRows"), Count(db, "Manufacturers"),
            Count(db, "BaseMaterialCatalog"), Count(db, "NativeTensileSamples"),
            Count(db, "NativeTensileResults"), Count(db, "NativeImpactSamples"),
            Count(db, "NativeStiffnessMeasurements"), emptyRows,
            Scalar(db, "PRAGMA integrity_check;"),
            Count(db, "pragma_foreign_key_check"),
            Scalar(db, "SELECT sqlite_version();"),
            Convert.ToInt32(Scalar(db, "PRAGMA page_size;"),
                CultureInfo.InvariantCulture),
            Scalar(db, "PRAGMA encoding;"),
            Convert.ToInt32(Scalar(db, "PRAGMA auto_vacuum;"),
                CultureInfo.InvariantCulture),
            Scalar(db, "PRAGMA journal_mode;"));
    }

    private static void AppendHashValue(StringBuilder builder, string value) =>
        builder.Append(value.Length.ToString(CultureInfo.InvariantCulture))
            .Append(':').Append(value);

    private static string ReadPrimaryKeyOrder(SqliteConnection db, string table)
    {
        using var command = db.CreateCommand();
        command.CommandText = $"PRAGMA table_info(\"{table}\");";
        using var reader = command.ExecuteReader();
        var keys = new List<(int Order, string Name)>();
        while (reader.Read())
        {
            var order = reader.GetInt32(5);
            if (order > 0) keys.Add((order, reader.GetString(1)));
        }
        return keys.Count == 0
            ? "rowid"
            : string.Join(",", keys.OrderBy(item => item.Order)
                .Select(item => $"\"{item.Name.Replace("\"", "\"\"")}\""));
    }

    private static string ComputeSchemaHash(IEnumerable<SchemaItem> objects)
    {
        var builder = new StringBuilder();
        foreach (var item in objects.OrderBy(value => value.Type, StringComparer.Ordinal)
                     .ThenBy(value => value.Name, StringComparer.Ordinal))
            foreach (var value in new[]
                     {
                         item.Type, item.Name, item.Table, NormalizeSql(item.Sql)
                     })
                builder.Append(value.Length.ToString(CultureInfo.InvariantCulture))
                    .Append(':').Append(value);
        return Sha256(Encoding.UTF8.GetBytes(builder.ToString()));
    }

    private static string NormalizeSql(string value) =>
        string.Join(' ', value.Split((char[]?)null,
            StringSplitOptions.RemoveEmptyEntries));

    private static string AddIds(SqliteCommand command, IEnumerable<string> ids)
    {
        var names = new List<string>();
        var index = 0;
        foreach (var id in ids.OrderBy(value => value, StringComparer.Ordinal))
        {
            var name = $"$id{index++}";
            command.Parameters.AddWithValue(name, id);
            names.Add(name);
        }
        return string.Join(",", names);
    }

    private static SqliteConnection OpenImmutable(string path)
    {
        var uriPath = new Uri(path).AbsoluteUri;
        var connection = new SqliteConnection(
            $"Data Source={uriPath}?immutable=1;Mode=ReadOnly;Pooling=False");
        connection.Open();
        Execute(connection, "PRAGMA query_only=ON;");
        return connection;
    }

    private static void Insert(
        SqliteConnection db, SqliteTransaction tx, string sql, params object[] values)
    {
        using var command = db.CreateCommand();
        command.Transaction = tx;
        command.CommandText = sql;
        for (var index = 0; index < values.Length; index++)
            command.Parameters.AddWithValue($"${(char)('a' + index)}",
                values[index] ?? DBNull.Value);
        if (command.ExecuteNonQuery() != 1)
            throw new InvalidDataException("INSERT_COUNT");
    }

    private static void Execute(SqliteConnection db, string sql)
    {
        using var command = db.CreateCommand();
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static void Execute(
        SqliteConnection db, SqliteTransaction tx, string sql)
    {
        using var command = db.CreateCommand();
        command.Transaction = tx;
        command.CommandText = sql;
        command.ExecuteNonQuery();
    }

    private static string Scalar(SqliteConnection db, string sql)
    {
        using var command = db.CreateCommand();
        command.CommandText = sql;
        return Convert.ToString(command.ExecuteScalar(),
            CultureInfo.InvariantCulture) ?? "";
    }

    private static int Count(SqliteConnection db, string table) =>
        Convert.ToInt32(Scalar(db, $"SELECT COUNT(*) FROM {table};"),
            CultureInfo.InvariantCulture);

    private static string CanonicalDecimal(string value)
    {
        if (!decimal.TryParse(value, NumberStyles.Number,
                CultureInfo.InvariantCulture, out var number))
            throw new InvalidDataException("NUMERIC_VALUE");
        return number.ToString("0.############################",
            CultureInfo.InvariantCulture);
    }

    private static string FormatResult(double? value) =>
        value.HasValue ? value.Value.ToString("0.###",
            CultureInfo.InvariantCulture) : "";

    private static int ParseOrdinal(string value, string prefix) =>
        value.StartsWith(prefix, StringComparison.Ordinal) &&
        int.TryParse(value.AsSpan(prefix.Length), NumberStyles.None,
            CultureInfo.InvariantCulture, out var ordinal)
            ? ordinal : throw new InvalidDataException("GROUP_FORMAT");

    private static string Join(params string[] values) =>
        string.Join(" ", values.Where(value => value.Length > 0));

    private static bool Canonical(string value) =>
        string.Equals(value, value.Trim(), StringComparison.Ordinal) &&
        string.Equals(value, value.Normalize(NormalizationForm.FormC),
            StringComparison.Ordinal);

    private static bool HasReparseAncestor(string path, string stopPath)
    {
        var stop = IOPath.GetFullPath(stopPath);
        var current = IOPath.GetFullPath(path);
        while (true)
        {
            if ((IOFile.GetAttributes(current) & FileAttributes.ReparsePoint) != 0)
                return true;
            if (string.Equals(current, stop, StringComparison.OrdinalIgnoreCase))
                return false;
            var parent = IOPath.GetDirectoryName(current);
            if (string.IsNullOrWhiteSpace(parent) ||
                string.Equals(parent, current, StringComparison.OrdinalIgnoreCase))
                return true;
            current = parent;
        }
    }

    private static void RequireNoSidecars(params string[] paths)
    {
        foreach (var path in paths)
            foreach (var suffix in new[] { "-wal", "-shm", "-journal" })
                if (IOFile.Exists(path + suffix))
                    throw new InvalidDataException("SQLITE_SIDECAR");
    }

    private static void RequireHash(string actual, string expected, string error)
    {
        if (!CryptographicOperations.FixedTimeEquals(
                Convert.FromHexString(actual), Convert.FromHexString(expected)))
            throw new InvalidDataException(error);
    }

    private static string HashFile(string path) =>
        Convert.ToHexString(SHA256.HashData(IOFile.ReadAllBytes(path)));
    private static string Sha256(byte[] bytes) =>
        Convert.ToHexString(SHA256.HashData(bytes));
    private static string HashManifest(BuildManifest manifest) =>
        Sha256(JsonSerializer.SerializeToUtf8Bytes(
            manifest with { ManifestSha256 = "" }, JsonOptions()));
    private static JsonSerializerOptions JsonOptions() => new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };
    private static void WriteNew(string path, byte[] bytes)
    {
        using var stream = new IOFileStream(path, FileMode.CreateNew,
            FileAccess.Write, FileShare.None);
        stream.Write(bytes);
    }

    private sealed record BuildOptions(
        string Root, string Source, string Allowlist, string Transformation,
        string OutputA, string OutputB, string Manifest);
    private sealed record AllowEntry(
        string SourceId, string ManufacturerGroup,
        string ProductFamilyGroup, string BaseMaterial);
    private sealed record MaterialIdentity(
        string MaterialId, string SourceId, int ManufacturerId,
        string Manufacturer, string ProductLine, string MarketingName,
        int BaseMaterialId, string BaseMaterial, string VariantFinish,
        string Reinforcement, string Color, string DisplayName,
        string MaterialKey, int RowIndex);
    private sealed record Contract(IReadOnlyList<MaterialIdentity> Materials);
    private sealed record Sample(
        string MaterialId, string Orientation, int Number, string RawValue);
    private sealed record Stiffness(
        string MaterialId, string Revolutions, string Degrees);
    private sealed record Payload(
        IReadOnlyList<Sample> Tensile, IReadOnlyList<Sample> Impact,
        IReadOnlyList<Stiffness> Stiffness,
        IReadOnlyList<ExpectedTensile> ExpectedTensile);
    private sealed record ExpectedTensile(
        string MaterialId, IReadOnlyList<string> Values);
    private sealed record MeasurementSet(
        string Average, string StdDev, string Cv, string Count, string Confidence);
    private sealed record SchemaItem(
        string Type, string Name, string Table, string Sql);
    private sealed record OutputInspection(
        long FileBytes, string SchemaHash, string LogicalHash,
        IReadOnlyDictionary<string, int> TableRowCounts,
        IReadOnlyDictionary<string, string> TableSha256, int Materials,
        int Manufacturers, int BaseMaterials, int TensileSamples,
        int TensileResults, int ImpactSamples, int StiffnessRows,
        int EmptyDomainRows, string Integrity, int ForeignKeyViolations,
        string SqliteVersion, int PageSize, string Encoding, int AutoVacuum,
        string JournalMode);
    private sealed record BuildManifest(
        string Schema, string ContractVersion, string OverallResult,
        string SourceSha256, string AllowlistSha256,
        string TransformationSha256, string SchemaSha256,
        string OutputASha256, string OutputBSha256,
        string LogicalASha256, string LogicalBSha256,
        IReadOnlyDictionary<string, int> TableRowCounts,
        IReadOnlyDictionary<string, string> TableSha256, long OutputBytes,
        int MaterialCount, int ManufacturerCount, int BaseMaterialCount,
        int TensileSampleCount, int TensileResultCount, int ImpactSampleCount,
        int StiffnessRowCount, int EmptyDomainRowCount, string IntegrityResult,
        int ForeignKeyViolationCount, string RuntimeVersions,
        int PageSize, string Encoding, int AutoVacuum, string JournalMode,
        string ManifestSha256);
}
