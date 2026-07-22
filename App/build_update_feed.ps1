param(
    [Parameter(Mandatory = $true)][string]$SignedPackage,
    [string]$OutputFolder = (Join-Path $PSScriptRoot "artifacts\update-feed")
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
    Copy-Item -LiteralPath $package -Destination (Join-Path $OutputFolder $packageName)
    $feed = [ordered]@{
        schema = "3dpiceland.application-update-feed.v1"
        packageUrl = "https://www.iskort.is/3dp/updates/$packageName"
        packageBytes = (Get-Item -LiteralPath $package).Length
        packageSha256 = (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash
        manifest = $manifest
    }
    $feed | ConvertTo-Json -Depth 8 | Set-Content -LiteralPath (Join-Path $OutputFolder "latest.json") -Encoding UTF8
    Write-Host "Update feed ready: $OutputFolder"
}
finally { if (Test-Path -LiteralPath $work) { Remove-Item -LiteralPath $work -Recurse -Force } }
