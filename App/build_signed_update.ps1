param(
    [string]$OutputFolder = (Join-Path $PSScriptRoot "artifacts"),
    [switch]$AllowDirty,
    [string]$VersionOverride = "",
    [ValidateSet("Candidate", "Production")]
    [string]$ReleaseState = "Candidate",
    [string]$VerifierArtifactsPath = ""
)

$ErrorActionPreference = "Stop"
$repository = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$gitStatus = @(git -C $repository status --porcelain)
if ($LASTEXITCODE -ne 0) { throw "Could not inspect repository status." }
if ($ReleaseState -eq "Production" -and $AllowDirty) { throw "-AllowDirty is valid only for an explicit Candidate package." }
if ($gitStatus.Count -gt 0 -and ($ReleaseState -eq "Production" -or -not $AllowDirty)) {
    throw "$ReleaseState signed packaging requires a clean Git worktree. Use -ReleaseState Candidate -AllowDirty only for an explicit pre-release verification package."
}
if ($gitStatus.Count -gt 0) { Write-Warning "Creating an explicit Candidate package from a dirty worktree." }
$project = Join-Path $PSScriptRoot "FilamentDbApp\FilamentDbApp.csproj"
$publishFolder = Join-Path $PSScriptRoot "FilamentDbApp\bin\Release\net9.0-windows\win-x64\publish"
$updaterProject = Join-Path $PSScriptRoot "FilamentDbUpdater\FilamentDbUpdater.csproj"
$updaterPublishFolder = Join-Path $PSScriptRoot "FilamentDbUpdater\bin\Release\net9.0\win-x64\publish"
$packager = Join-Path $PSScriptRoot "..\Tools\ReleasePackager\ReleasePackager.csproj"
$verifier = Join-Path $PSScriptRoot "..\Tools\UpdatePackageVerifier\UpdatePackageVerifier.csproj"
$buildInfoPath = Join-Path $PSScriptRoot "FilamentDbApp\BuildInfo.cs"
[xml]$projectXml = Get-Content -LiteralPath $project
$version = [string]$projectXml.Project.PropertyGroup.Version
$informational = [string]$projectXml.Project.PropertyGroup.InformationalVersion
$releaseCode = $informational.Substring($informational.IndexOf("-") + 1)
if (-not [string]::IsNullOrWhiteSpace($VersionOverride)) { $version = $VersionOverride }
$buildInfo = [IO.File]::ReadAllText($buildInfoPath)
$minimumSchemaMatch = [Text.RegularExpressions.Regex]::Match(
    $buildInfo,
    'public const int MinimumUpdateDatabaseSchema = (?<value>\d+);')
$currentSchemaMatch = [Text.RegularExpressions.Regex]::Match(
    $buildInfo,
    'public const int CurrentDatabaseSchema = (?<value>\d+);')
if (-not $minimumSchemaMatch.Success -or -not $currentSchemaMatch.Success) {
    throw "Could not read the governed update schema contract from BuildInfo.cs."
}
$minimumSchema = [int]$minimumSchemaMatch.Groups["value"].Value
$currentSchema = [int]$currentSchemaMatch.Groups["value"].Value
if ($minimumSchema -le 0 -or $currentSchema -lt $minimumSchema) {
    throw "The governed update schema contract is invalid."
}

if (Test-Path -LiteralPath $publishFolder) { Remove-Item -LiteralPath $publishFolder -Recurse -Force }
if (Test-Path -LiteralPath $updaterPublishFolder) { Remove-Item -LiteralPath $updaterPublishFolder -Recurse -Force }
dotnet publish $project -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true /p:Version=$version /p:AssemblyVersion=$version.0 /p:FileVersion=$version.0 /p:InformationalVersion=$version-$releaseCode
if ($LASTEXITCODE -ne 0) { throw "Canonical Release publish failed." }
New-Item -ItemType Directory -Force -Path (Join-Path $publishFolder "Assets") | Out-Null
Copy-Item -LiteralPath (Join-Path $PSScriptRoot "FilamentDbApp\Assets\3dp-iceland-labs-icon.ico") -Destination (Join-Path $publishFolder "Assets\3dp-iceland-labs-icon.ico") -Force
dotnet publish $updaterProject -c Release -r win-x64 --self-contained true /p:PublishSingleFile=true /p:IncludeNativeLibrariesForSelfExtract=true /p:EnableCompressionInSingleFile=true
if ($LASTEXITCODE -ne 0) { throw "Transactional updater publish failed." }
Copy-Item -LiteralPath (Join-Path $updaterPublishFolder "3DPIcelandUpdater.exe") -Destination (Join-Path $publishFolder "3DPIcelandUpdater.exe") -Force

New-Item -ItemType Directory -Force -Path $OutputFolder | Out-Null
$output = Join-Path $OutputFolder ("3DPIceland_Update_v" + $version.Replace(".", "_") + ".zip")
if (Test-Path -LiteralPath $output) { throw "Signed update output already exists: $output" }
dotnet run --project $packager -c Release -- package --input $publishFolder --output $output --version $version --code $releaseCode --min-schema $minimumSchema --max-schema $currentSchema
if ($LASTEXITCODE -ne 0) { throw "Signed update packaging failed." }
if ([string]::IsNullOrWhiteSpace($VerifierArtifactsPath)) {
    dotnet run --project $verifier -c Release -- $output $version $releaseCode
} else {
    dotnet restore $verifier "-p:ArtifactsPath=$VerifierArtifactsPath"
    if ($LASTEXITCODE -ne 0) { throw "Isolated application verifier restore failed." }
    dotnet run --project $verifier -c Release --no-restore "-p:ArtifactsPath=$VerifierArtifactsPath" -- $output $version $releaseCode
}
if ($LASTEXITCODE -ne 0) { throw "Application verifier rejected the newly signed update package." }

Write-Host "$ReleaseState signed update package ready: $output"
