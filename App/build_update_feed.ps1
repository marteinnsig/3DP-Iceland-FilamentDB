param(
    [Parameter(Mandatory = $true)][string]$SignedPackage,
    [string]$OutputFolder = (Join-Path $PSScriptRoot "artifacts\update-feed"),
    [ValidateSet("Candidate", "Production")]
    [string]$ReleaseState = "Candidate"
)
$ErrorActionPreference = "Stop"
$package = (Resolve-Path -LiteralPath $SignedPackage).Path
if (-not [IO.Path]::IsPathRooted($OutputFolder)) { $OutputFolder = Join-Path (Get-Location).Path $OutputFolder }
$OutputFolder = [IO.Path]::GetFullPath($OutputFolder)
$work = Join-Path ([IO.Path]::GetTempPath()) ("3DPIceland-Feed-" + [Guid]::NewGuid().ToString("N"))
try {
    New-Item -ItemType Directory -Force -Path $work, $OutputFolder | Out-Null
    Expand-Archive -LiteralPath $package -DestinationPath $work
    $manifest = Get-Content -LiteralPath (Join-Path $work "3dp-update-manifest.json") -Raw | ConvertFrom-Json
    $packageName = [IO.Path]::GetFileName($package)
    $feedPackagePath = Join-Path $OutputFolder $packageName
    $latestPath = Join-Path $OutputFolder "latest.json"
    if (Test-Path -LiteralPath $feedPackagePath) { throw "Update-feed package already exists: $feedPackagePath" }
    if (Test-Path -LiteralPath $latestPath) { throw "Update-feed metadata already exists: $latestPath" }
    Copy-Item -LiteralPath $package -Destination $feedPackagePath
    $feed = [ordered]@{
        schema = "3dpiceland.application-update-feed.v1"
        releaseState = $ReleaseState
        packageUrl = "https://www.iskort.is/3dp/updates/$packageName"
        packageBytes = (Get-Item -LiteralPath $package).Length
        packageSha256 = (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash
        manifest = $manifest
    }
    $feedJson = $feed | ConvertTo-Json -Depth 8
    [IO.File]::WriteAllText(
        $latestPath,
        $feedJson,
        [Text.UTF8Encoding]::new($false))
    Write-Host "$ReleaseState update feed ready: $OutputFolder"
}
finally { if (Test-Path -LiteralPath $work) { Remove-Item -LiteralPath $work -Recurse -Force } }
