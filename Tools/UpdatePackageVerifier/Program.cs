using FilamentDbApp;
using FilamentDbApp.Services;

if (args.Length is not (1 or 3))
{
    Console.Error.WriteLine("Usage: UpdatePackageVerifier <signed-update.zip> [expected-version expected-code]");
    return 2;
}

var packagePath = Path.GetFullPath(args[0]);
var packageService = new ApplicationUpdatePackageService();
var result = packageService.Inspect(
    packagePath,
    new Version(0, 0, 0),
    BuildInfo.CurrentDatabaseSchema);
if (!result.Ready)
{
    Console.Error.WriteLine(result.Status + ": " + result.Detail);
    return 1;
}
var manifest = result.Manifest ?? throw new InvalidOperationException("Ready package result did not contain a manifest.");
var expectedVersion = args.Length == 3 ? args[1] : BuildInfo.Version;
var expectedCode = args.Length == 3 ? args[2] : BuildInfo.ReleaseCode;
if (!string.Equals(manifest.ReleaseVersion, expectedVersion, StringComparison.Ordinal) ||
    !string.Equals(manifest.ReleaseCode, expectedCode, StringComparison.Ordinal))
{
    Console.Error.WriteLine($"Package identity v{manifest.ReleaseVersion} {manifest.ReleaseCode} does not match expected v{expectedVersion} {expectedCode}.");
    return 1;
}
if (manifest.MinimumDatabaseSchema != BuildInfo.MinimumUpdateDatabaseSchema ||
    manifest.MaximumDatabaseSchema != BuildInfo.CurrentDatabaseSchema)
{
    Console.Error.WriteLine(
        $"Package schema support v{manifest.MinimumDatabaseSchema}-v{manifest.MaximumDatabaseSchema} does not match " +
        $"the governed release contract v{BuildInfo.MinimumUpdateDatabaseSchema}-v{BuildInfo.CurrentDatabaseSchema}.");
    return 1;
}
var minimumSchemaResult = packageService.Inspect(
    packagePath,
    new Version(0, 0, 0),
    BuildInfo.MinimumUpdateDatabaseSchema);
if (!minimumSchemaResult.Ready)
{
    Console.Error.WriteLine(
        $"Package rejected the governed public schema-v{BuildInfo.MinimumUpdateDatabaseSchema} baseline: " +
        minimumSchemaResult.Status + ": " + minimumSchemaResult.Detail);
    return 1;
}

Console.WriteLine("Application verifier accepted production-signed package.");
Console.WriteLine(
    $"{result.Detail} Governed public baseline schema v{BuildInfo.MinimumUpdateDatabaseSchema} also supported.");
return 0;
