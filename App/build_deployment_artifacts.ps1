param(
    [Parameter(Mandatory = $true)][string]$SignedPackage,
    [string]$OutputFolder = (Join-Path $PSScriptRoot "artifacts\deployment"),
    [string]$InnoCompiler = "",
    [string]$ExpectedVersion = ""
)

$ErrorActionPreference = "Stop"
$repository = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$package = (Resolve-Path -LiteralPath $SignedPackage).Path
if (-not [IO.Path]::IsPathRooted($OutputFolder)) { $OutputFolder = Join-Path (Get-Location).Path $OutputFolder }
$OutputFolder = [IO.Path]::GetFullPath($OutputFolder)
$verifier = Join-Path $repository "Tools\UpdatePackageVerifier\UpdatePackageVerifier.csproj"
$installerScript = Join-Path $repository "Deployment\3DPIcelandInstaller.iss"
if ([string]::IsNullOrWhiteSpace($InnoCompiler)) {
    $candidate = Get-Command ISCC.exe -ErrorAction SilentlyContinue
    if ($null -ne $candidate) { $InnoCompiler = $candidate.Source }
}
if ([string]::IsNullOrWhiteSpace($InnoCompiler) -or -not (Test-Path -LiteralPath $InnoCompiler)) {
    throw "Inno Setup ISCC.exe is required to build the installer. Portable artifact generation was not started."
}

$work = Join-Path ([IO.Path]::GetTempPath()) ("3DPIceland-Deployment-" + [Guid]::NewGuid().ToString("N"))
$source = Join-Path $work "source"
try {
    New-Item -ItemType Directory -Force -Path $source, $OutputFolder | Out-Null
    if ([string]::IsNullOrWhiteSpace($ExpectedVersion)) {
        dotnet run --project $verifier -c Release -- $package
    } else {
        dotnet run --project $verifier -c Release -- $package $ExpectedVersion "REMOTE-SIGNED-UPDATE-DELIVERY"
    }
    if ($LASTEXITCODE -ne 0) { throw "Application verifier rejected the signed source package." }
    Expand-Archive -LiteralPath $package -DestinationPath $source
    $manifestPath = Join-Path $source "3dp-update-manifest.json"
    $manifest = Get-Content -LiteralPath $manifestPath -Raw | ConvertFrom-Json
    Remove-Item -LiteralPath $manifestPath

    $forbiddenPayloadExtensions = @(".sqlite", ".sqlite3", ".db", ".xlsx", ".xls", ".csv", ".tsv", ".bak")
    $forbiddenPayloadFiles = Get-ChildItem -LiteralPath $source -Recurse -File | Where-Object {
        $forbiddenPayloadExtensions -contains $_.Extension.ToLowerInvariant() -or
        $_.Name.Equals("website-template-index.html", [StringComparison]::OrdinalIgnoreCase) -or
        $_.Name.StartsWith("native-", [StringComparison]::OrdinalIgnoreCase)
    }
    if ($forbiddenPayloadFiles.Count -gt 0) {
        throw "Deployment payload contains governed/user data files: $($forbiddenPayloadFiles.FullName -join ', ')"
    }


    $portableName = "3DPIceland-Portable-x64-v$($manifest.releaseVersion).zip"
    $portablePath = Join-Path $OutputFolder $portableName
    if (Test-Path -LiteralPath $portablePath) { throw "Portable output already exists: $portablePath" }
    Compress-Archive -Path (Join-Path $source "*") -DestinationPath $portablePath -CompressionLevel Optimal

    & $InnoCompiler "/DSourceDir=$source" "/DOutputDir=$OutputFolder" "/DAppVersion=$($manifest.releaseVersion)" $installerScript
    if ($LASTEXITCODE -ne 0) { throw "Inno Setup compiler rejected the governed installer definition." }
    $installerPath = Join-Path $OutputFolder "3DPIceland-Setup-x64-v$($manifest.releaseVersion).exe"
    if (-not (Test-Path -LiteralPath $installerPath)) { throw "Expected installer output is missing." }

    $files = @(
        @{ kind="Installer"; localFile=(Split-Path $installerPath -Leaf); stableRemotePath="/downloads/3DPIceland-Setup-x64.exe"; versionedRemotePath="/downloads/$([IO.Path]::GetFileName($installerPath))"; bytes=(Get-Item $installerPath).Length; sha256=(Get-FileHash $installerPath -Algorithm SHA256).Hash },
        @{ kind="Portable"; localFile=$portableName; stableRemotePath="/downloads/3DPIceland-Portable-x64.zip"; versionedRemotePath="/downloads/$portableName"; bytes=(Get-Item $portablePath).Length; sha256=(Get-FileHash $portablePath -Algorithm SHA256).Hash }
    )
    $plan = [ordered]@{ schema="3dpiceland.application-deployment-plan.v1"; releaseVersion=[string]$manifest.releaseVersion; releaseCode=[string]$manifest.releaseCode; generatedAtUtc=[DateTimeOffset]::UtcNow.ToString("O"); sourcePackageSha256=(Get-FileHash $package -Algorithm SHA256).Hash; files=$files }
    $plan | ConvertTo-Json -Depth 5 | Set-Content -LiteralPath (Join-Path $OutputFolder "application-deployment-plan.json") -Encoding UTF8
    Write-Host "Deployment artifacts ready: $OutputFolder"
}
finally {
    if (Test-Path -LiteralPath $work) { Remove-Item -LiteralPath $work -Recurse -Force }
}
