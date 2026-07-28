using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using IOFile = System.IO.File;
using IOFileStream = System.IO.FileStream;
using IODirectory = System.IO.Directory;
using IOPath = System.IO.Path;

namespace FilamentDbApp.DemoDatasetTool;

internal static class TransformationContractValidator
{
    private const string ManifestSchema =
        "3dpiceland-public-demo-transformation-validation-v1";
    private const string TransformationSchema =
        "3dpiceland-public-demo-transformation-v1";
    private const string ContractVersion = "v56.0.3";
    private const string RequiredAllowlistSha256 =
        "A26A8406F2219DE035AEE6F24ECE3676037FD7880C01266009CACBF027BA9A7B";
    private const string RequiredTransformationSha256 =
        "1AED8DE62B74CFDFE34AFD8992BDBD4D1ED557EFA8515971EFDCCCBB3017017C";
    private const string RequiredSourceSha256 =
        "74943C492AE0FD06DABD7485D648222F87EE059343C2E3F6EAC298116F6D14F8";
    private const string FixedUtc = "2026-01-01T00:00:00.0000000Z";

    public static int Run(string[] args)
    {
        var options = Parse(args);
        var manifest = Validate(options);
        WriteManifest(options.OutputPath, manifest);
        Console.WriteLine($"Transformation validation: {manifest.OverallResult}");
        Console.WriteLine($"Manifest SHA-256: {manifest.ManifestSha256}");
        Console.WriteLine("Manifest written inside the governed inspection root.");
        return manifest.OverallResult == "PASS" ? 0 : 2;
    }

    private static ValidationOptions Parse(string[] args)
    {
        if (args.Length != 9 ||
            !string.Equals(args[0], "validate-transform", StringComparison.Ordinal))
            throw new ArgumentException("ARGS");
        var values = new Dictionary<string, string>(StringComparer.Ordinal);
        for (var index = 1; index < args.Length; index += 2)
        {
            if (!values.TryAdd(args[index], args[index + 1]))
                throw new ArgumentException("ARGS");
        }
        if (!values.TryGetValue("--inspection-root", out var root) ||
            !values.TryGetValue("--allowlist", out var allowlist) ||
            !values.TryGetValue("--transformation", out var transformation) ||
            !values.TryGetValue("--output", out var output))
            throw new ArgumentException("ARGS");

        var rootPath = IOPath.GetFullPath(root);
        var allowlistPath = IOPath.GetFullPath(allowlist);
        var transformationPath = IOPath.GetFullPath(transformation);
        var outputPath = IOPath.GetFullPath(output);
        if (!IODirectory.Exists(rootPath) ||
            (IOFile.GetAttributes(rootPath) & FileAttributes.ReparsePoint) != 0 ||
            !string.Equals(IOPath.GetFileName(rootPath), "v56-source-inspection",
                StringComparison.Ordinal) ||
            !string.Equals(IOPath.GetFileName(IOPath.GetDirectoryName(rootPath)),
                "artifacts", StringComparison.Ordinal) ||
            !string.Equals(IOPath.GetDirectoryName(allowlistPath), rootPath,
                StringComparison.OrdinalIgnoreCase) ||
            !string.Equals(IOPath.GetDirectoryName(outputPath), rootPath,
                StringComparison.OrdinalIgnoreCase))
            throw new InvalidDataException("INSPECTION_CONTAINMENT");
        if (!IOFile.Exists(allowlistPath) || !IOFile.Exists(transformationPath))
            throw new FileNotFoundException("INPUT_MISSING");
        if (IOFile.Exists(outputPath))
            throw new IOException("OUTPUT_EXISTS");
        if ((IOFile.GetAttributes(allowlistPath) & FileAttributes.ReparsePoint) != 0 ||
            (IOFile.GetAttributes(transformationPath) & FileAttributes.ReparsePoint) != 0)
            throw new InvalidDataException("INPUT_REPARSE");
        var expectedTransformationPath = IOPath.GetFullPath(IOPath.Combine(
            Environment.CurrentDirectory,
            "App",
            "DemoDatasetTool",
            "Contracts",
            "public-demo-transformation-v1.json"));
        if (!string.Equals(
                transformationPath,
                expectedTransformationPath,
                StringComparison.OrdinalIgnoreCase) ||
            HasReparseAncestor(
                IOPath.GetDirectoryName(transformationPath),
                Environment.CurrentDirectory))
            throw new InvalidDataException("TRANSFORMATION_LOCATION");
        return new ValidationOptions(
            rootPath,
            allowlistPath,
            transformationPath,
            outputPath);
    }

    private static ValidationManifest Validate(ValidationOptions options)
    {
        var failures = new SortedSet<string>(StringComparer.Ordinal);
        var allowlistBytes = IOFile.ReadAllBytes(options.AllowlistPath);
        var transformationBytes = IOFile.ReadAllBytes(options.TransformationPath);
        var allowlistSha256 = Sha256(allowlistBytes);
        var transformationSha256 = Sha256(transformationBytes);
        if (!FixedEquals(allowlistSha256, RequiredAllowlistSha256))
            failures.Add("ALLOWLIST_REGISTRY_HASH");
        if (!FixedEquals(transformationSha256, RequiredTransformationSha256))
            failures.Add("TRANSFORMATION_SPEC_HASH");

        var allowlist = Deserialize<AllowlistDocument>(
            allowlistBytes,
            "ALLOWLIST_JSON");
        var transformation = Deserialize<TransformationDocument>(
            transformationBytes,
            "TRANSFORMATION_JSON");
        if (!string.Equals(transformation.Schema, TransformationSchema,
                StringComparison.Ordinal) ||
            !string.Equals(transformation.ContractVersion, ContractVersion,
                StringComparison.Ordinal) ||
            !string.Equals(transformation.FixedUtc, FixedUtc,
                StringComparison.Ordinal) ||
            transformation.ManufacturerIdStart != 560001 ||
            transformation.BaseMaterialIdStart != 561001)
            failures.Add("TRANSFORMATION_CONTRACT");
        if (!string.Equals(allowlist.ContractVersion, "v56.0.1",
                StringComparison.Ordinal) ||
            allowlist.ApprovedAtUtc == default ||
            !allowlist.OwnerApproval.ExactAllowlistApproved ||
            !allowlist.OwnerApproval.RealMeasurementReidentificationRiskAccepted ||
            allowlist.Source.SchemaVersion != 38 ||
            !FixedEquals(allowlist.Source.Sha256, RequiredSourceSha256))
            failures.Add("ALLOWLIST_APPROVAL_CONTRACT");
        if (allowlist.Expected.ManufacturerGroupCount != 10 ||
            allowlist.Expected.BaseMaterialCount != 11 ||
            allowlist.Expected.TensileSampleCount != 712 ||
            allowlist.Expected.TensileResultCount != 36 ||
            allowlist.Expected.ImpactSampleCount != 718 ||
            allowlist.Expected.StiffnessRowCount != 36 ||
            allowlist.Expected.ArchivedMaterialCount != 0)
            failures.Add("ALLOWLIST_EXPECTED_CONTRACT");

        var expectedBases = new[]
        {
            "ABS", "ASA", "PA12", "PA6", "PC", "PC/PBT",
            "PCTG", "PET", "PETG", "PLA", "PP"
        };
        if (!transformation.BaseMaterials.SequenceEqual(
                expectedBases,
                StringComparer.Ordinal))
            failures.Add("BASE_TAXONOMY");
        if (transformation.Materials.Count != 36 ||
            allowlist.Entries.Count != 36)
            failures.Add("MATERIAL_COUNT");

        var sourceIds = allowlist.Entries.Select(item => item.SourceMaterialId).ToArray();
        var demoIds = allowlist.Entries.Select(item => item.DemoMaterialId).ToArray();
        if (sourceIds.Any(value => !Canonical(value)) ||
            demoIds.Any(value => !Canonical(value)) ||
            sourceIds.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 36 ||
            demoIds.Distinct(StringComparer.OrdinalIgnoreCase).Count() != 36 ||
            allowlist.Entries.Any(item =>
                !item.Approved ||
                item.ArchivedApproved ||
                !Canonical(item.ManufacturerGroup) ||
                !Canonical(item.ProductFamilyGroup) ||
                !Canonical(item.BaseMaterial) ||
                !Canonical(item.RiskReason) ||
                !item.ApprovedDomains.OrderBy(value => value, StringComparer.Ordinal)
                    .SequenceEqual(
                        new[] { "IMPACT", "STIFFNESS", "TENSILE" },
                        StringComparer.Ordinal)))
            failures.Add("ALLOWLIST_BIJECTION_CONTRACT");
        var allowlistByDemo = allowlist.Entries
            .GroupBy(item => item.DemoMaterialId, StringComparer.Ordinal)
            .ToDictionary(group => group.Key, group => group.First(),
                StringComparer.Ordinal);
        var derivedRows = new List<DerivedIdentity>();
        for (var index = 0; index < transformation.Materials.Count; index++)
        {
            var item = transformation.Materials[index];
            if (!Canonical(item.MaterialId) ||
                !Canonical(item.ManufacturerGroup) ||
                !Canonical(item.ProductFamilyGroup) ||
                !Canonical(item.BaseMaterial) ||
                !Canonical(item.VariantFinish) ||
                !Canonical(item.Reinforcement) ||
                !Canonical(item.Color))
                failures.Add("ATOMIC_CANONICALIZATION");
            var expectedMaterialId = $"DEMO-MAT-{index + 1:000}";
            if (!string.Equals(item.MaterialId, expectedMaterialId,
                    StringComparison.Ordinal))
                failures.Add("MATERIAL_SEQUENCE");
            if (!allowlistByDemo.TryGetValue(item.MaterialId, out var approved))
            {
                failures.Add("ALLOWLIST_CLOSURE");
                continue;
            }
            if (!string.Equals(item.ManufacturerGroup, approved.ManufacturerGroup,
                    StringComparison.Ordinal) ||
                !string.Equals(item.ProductFamilyGroup, approved.ProductFamilyGroup,
                    StringComparison.Ordinal) ||
                !string.Equals(item.BaseMaterial, approved.BaseMaterial,
                    StringComparison.Ordinal))
                failures.Add("PRIVATE_GROUP_PARITY");
            if (!TryOrdinal(item.ManufacturerGroup, "DEMO-MFR-", 10,
                    out var manufacturerOrdinal) ||
                !TryOrdinal(item.ProductFamilyGroup, "DEMO-FAM-", 14,
                    out var familyOrdinal))
            {
                failures.Add("GROUP_FORMAT");
                continue;
            }
            var baseIndex = Array.IndexOf(expectedBases, item.BaseMaterial);
            if (baseIndex < 0 ||
                !ValidOptionalToken(item.VariantFinish, "Demo Variant ", 4) ||
                !ValidRequiredToken(item.Color, "Demo Color ", 10) ||
                item.Reinforcement is not ("" or "CF" or "GF"))
            {
                failures.Add("ATOMIC_VALUE_CONTRACT");
                continue;
            }
            var manufacturerName =
                $"Fictional Manufacturer {manufacturerOrdinal:00}";
            var productLine = $"Demo Line {familyOrdinal:00}";
            var marketingName = $"Engineering Sample {index + 1:000}";
            var websiteDisplayName = JoinNonEmpty(
                manufacturerName,
                productLine,
                marketingName,
                item.BaseMaterial,
                item.VariantFinish,
                item.Reinforcement,
                item.Color);
            var materialKey = string.Join(
                "|",
                item.BaseMaterial,
                item.VariantFinish,
                item.Reinforcement,
                item.Color,
                manufacturerName,
                productLine,
                marketingName);
            if (websiteDisplayName.Contains('|') ||
                materialKey.Split('|').Length != 7)
                failures.Add("DERIVED_DELIMITER");
            derivedRows.Add(new DerivedIdentity(
                item.MaterialId,
                560000 + manufacturerOrdinal,
                manufacturerName,
                productLine,
                marketingName,
                561001 + baseIndex,
                item.BaseMaterial,
                item.VariantFinish,
                item.Reinforcement,
                item.Color,
                websiteDisplayName,
                materialKey,
                FixedUtc));
        }

        if (derivedRows.Select(item => item.MaterialId)
                .Distinct(StringComparer.OrdinalIgnoreCase).Count() != 36 ||
            derivedRows.Select(item => item.WebsiteDisplayName)
                .Distinct(StringComparer.OrdinalIgnoreCase).Count() != 36 ||
            derivedRows.Select(item => item.MaterialKey)
                .Distinct(StringComparer.OrdinalIgnoreCase).Count() != 36)
            failures.Add("DERIVED_UNIQUENESS");
        if (derivedRows.Select(item => item.ManufacturerId).Distinct().Count() != 10 ||
            derivedRows.Select(item => item.ProductLine)
                .Distinct(StringComparer.Ordinal).Count() != 14 ||
            derivedRows.Select(item => item.BaseMaterialId).Distinct().Count() != 11 ||
            derivedRows.Select(item => item.Color)
                .Distinct(StringComparer.Ordinal).Count() != 10 ||
            derivedRows.Where(item => item.VariantFinish.Length > 0)
                .Select(item => item.VariantFinish)
                .Distinct(StringComparer.Ordinal).Count() != 4)
            failures.Add("DERIVED_GROUP_COUNTS");
        if (derivedRows
                .GroupBy(item => item.ProductLine, StringComparer.Ordinal)
                .Any(group => group.Select(item => item.ManufacturerId)
                    .Distinct().Count() != 1))
            failures.Add("PRODUCT_FAMILY_PARENT");

        var materialIdSetSha256 = HashStrings(
            derivedRows.Select(item => item.MaterialId));
        var identityGraphSha256 = HashStrings(
            derivedRows.SelectMany(item => new[]
            {
                item.MaterialId,
                item.ManufacturerId.ToString(CultureInfo.InvariantCulture),
                item.Manufacturer,
                item.ProductLine,
                item.MarketingName,
                item.BaseMaterialId.ToString(CultureInfo.InvariantCulture),
                item.BaseMaterial,
                item.VariantFinish,
                item.Reinforcement,
                item.Color,
                item.WebsiteDisplayName,
                item.MaterialKey,
                item.UpdatedAtUtc
            }));
        var manifest = new ValidationManifest(
            ManifestSchema,
            ContractVersion,
            "DRY-RUN",
            failures.Count == 0 ? "PASS" : "FAIL",
            allowlistSha256,
            transformationSha256,
            derivedRows.Count,
            derivedRows.Select(item => item.ManufacturerId).Distinct().Count(),
            derivedRows.Select(item => item.ProductLine)
                .Distinct(StringComparer.Ordinal).Count(),
            derivedRows.Select(item => item.BaseMaterialId).Distinct().Count(),
            derivedRows.Select(item => item.Color)
                .Distinct(StringComparer.Ordinal).Count(),
            derivedRows.Where(item => item.VariantFinish.Length > 0)
                .Select(item => item.VariantFinish)
                .Distinct(StringComparer.Ordinal).Count(),
            derivedRows.Count(item => item.Reinforcement == "CF"),
            derivedRows.Count(item => item.Reinforcement == "GF"),
            materialIdSetSha256,
            identityGraphSha256,
            failures.ToArray(),
            string.Empty);
        return manifest with { ManifestSha256 = ManifestHash(manifest) };
    }

    private static bool ValidOptionalToken(string value, string prefix, int max) =>
        value.Length == 0 || ValidRequiredToken(value, prefix, max);

    private static bool ValidRequiredToken(string value, string prefix, int max)
    {
        if (!value.StartsWith(prefix, StringComparison.Ordinal) ||
            value.Length != prefix.Length + 2 ||
            !int.TryParse(value.AsSpan(prefix.Length), NumberStyles.None,
                CultureInfo.InvariantCulture, out var ordinal))
            return false;
        return ordinal >= 1 && ordinal <= max;
    }

    private static bool TryOrdinal(
        string value,
        string prefix,
        int max,
        out int ordinal)
    {
        ordinal = 0;
        return value.StartsWith(prefix, StringComparison.Ordinal) &&
            value.Length == prefix.Length + 3 &&
            int.TryParse(value.AsSpan(prefix.Length), NumberStyles.None,
                CultureInfo.InvariantCulture, out ordinal) &&
            ordinal >= 1 &&
            ordinal <= max;
    }

    private static string JoinNonEmpty(params string[] values) =>
        string.Join(" ", values.Where(value => value.Length > 0));

    private static bool Canonical(string value) =>
        string.Equals(value, value.Trim(), StringComparison.Ordinal) &&
        string.Equals(value, value.Normalize(NormalizationForm.FormC),
            StringComparison.Ordinal);

    private static bool HasReparseAncestor(string? path, string stopPath)
    {
        var stop = IOPath.GetFullPath(stopPath);
        var current = string.IsNullOrWhiteSpace(path)
            ? string.Empty
            : IOPath.GetFullPath(path);
        while (current.Length > 0)
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
        return true;
    }

    private static T Deserialize<T>(byte[] bytes, string failure)
    {
        try
        {
            return JsonSerializer.Deserialize<T>(bytes, JsonOptions()) ??
                throw new InvalidDataException(failure);
        }
        catch (JsonException)
        {
            throw new InvalidDataException(failure);
        }
    }

    private static string HashStrings(IEnumerable<string> values)
    {
        var builder = new StringBuilder();
        foreach (var value in values)
        {
            builder.Append(value.Length.ToString(CultureInfo.InvariantCulture))
                .Append(':')
                .Append(value);
        }
        return Sha256(Encoding.UTF8.GetBytes(builder.ToString()));
    }

    private static string ManifestHash(ValidationManifest manifest) =>
        Sha256(JsonSerializer.SerializeToUtf8Bytes(
            manifest with { ManifestSha256 = string.Empty },
            JsonOptions()));

    private static string Sha256(byte[] bytes) =>
        Convert.ToHexString(SHA256.HashData(bytes));

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

    private static void WriteManifest(string path, ValidationManifest manifest)
    {
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

    private sealed record ValidationOptions(
        string InspectionRoot,
        string AllowlistPath,
        string TransformationPath,
        string OutputPath);

    private sealed record DerivedIdentity(
        string MaterialId,
        int ManufacturerId,
        string Manufacturer,
        string ProductLine,
        string MarketingName,
        int BaseMaterialId,
        string BaseMaterial,
        string VariantFinish,
        string Reinforcement,
        string Color,
        string WebsiteDisplayName,
        string MaterialKey,
        string UpdatedAtUtc);

    private sealed record ValidationManifest(
        string Schema,
        string ContractVersion,
        string Mode,
        string OverallResult,
        string AllowlistSha256,
        string TransformationSha256,
        int MaterialCount,
        int ManufacturerCount,
        int ProductLineCount,
        int BaseMaterialCount,
        int ColorCount,
        int VariantCount,
        int CarbonFiberCount,
        int GlassFiberCount,
        string MaterialIdSetSha256,
        string IdentityGraphSha256,
        IReadOnlyList<string> FailureCodes,
        string ManifestSha256);

    private sealed class TransformationDocument
    {
        public string Schema { get; init; } = string.Empty;
        public string ContractVersion { get; init; } = string.Empty;
        public string FixedUtc { get; init; } = string.Empty;
        public int ManufacturerIdStart { get; init; }
        public int BaseMaterialIdStart { get; init; }
        public List<string> BaseMaterials { get; init; } = [];
        public List<TransformationMaterial> Materials { get; init; } = [];
    }

    private sealed class TransformationMaterial
    {
        public string MaterialId { get; init; } = string.Empty;
        public string ManufacturerGroup { get; init; } = string.Empty;
        public string ProductFamilyGroup { get; init; } = string.Empty;
        public string BaseMaterial { get; init; } = string.Empty;
        public string VariantFinish { get; init; } = string.Empty;
        public string Reinforcement { get; init; } = string.Empty;
        public string Color { get; init; } = string.Empty;
    }

    private sealed class AllowlistDocument
    {
        public string ContractVersion { get; init; } = string.Empty;
        public string AllowlistVersion { get; init; } = string.Empty;
        public DateTimeOffset ApprovedAtUtc { get; init; }
        public SourceDocument Source { get; init; } = new();
        public OwnerApprovalDocument OwnerApproval { get; init; } = new();
        public ExpectedDocument Expected { get; init; } = new();
        public List<AllowlistEntry> Entries { get; init; } = [];
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

    private sealed class AllowlistEntry
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
}
