param(
    [Parameter(Mandatory = $true)][string]$SignedPackage,
    [Parameter(Mandatory = $true)][string]$Feed,
    [string]$DeploymentPlan = "",
    [ValidateSet("Candidate", "Production")]
    [string]$ReleaseState = "Candidate"
)

$ErrorActionPreference = "Stop"
$repository = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$package = (Resolve-Path -LiteralPath $SignedPackage).Path
$feedPath = (Resolve-Path -LiteralPath $Feed).Path
$project = Join-Path $PSScriptRoot "FilamentDbApp\FilamentDbApp.csproj"
$verifier = Join-Path $repository "Tools\UpdatePackageVerifier\UpdatePackageVerifier.csproj"
$deploymentService = Join-Path $PSScriptRoot "FilamentDbApp\Services\ApplicationDeploymentService.cs"

function Assert-ReleaseGate([bool]$Condition, [string]$Failure) {
    if (-not $Condition) { throw $Failure }
}

$gitStatus = @(git -C $repository status --porcelain)
Assert-ReleaseGate ($LASTEXITCODE -eq 0) "Could not inspect repository status."
if ($ReleaseState -eq "Production") {
    Assert-ReleaseGate ($gitStatus.Count -eq 0) "Production release gates require a clean Git worktree."
}

$vulnerabilityJson = dotnet list $project package --vulnerable --include-transitive --format json
Assert-ReleaseGate ($LASTEXITCODE -eq 0) "NuGet vulnerability audit failed."
$null = $vulnerabilityJson | ConvertFrom-Json
$vulnerabilityText = $vulnerabilityJson -join [Environment]::NewLine
Assert-ReleaseGate ($vulnerabilityText -notmatch '"severity"\s*:') "NuGet vulnerability audit found one or more vulnerable packages."

$feedBytes = [IO.File]::ReadAllBytes($feedPath)
Assert-ReleaseGate ($feedBytes.Length -gt 0 -and $feedBytes[0] -eq [byte][char]'{') "latest.json must be non-empty BOM-less JSON beginning with '{'."
$feedDocument = [Text.Encoding]::UTF8.GetString($feedBytes) | ConvertFrom-Json
Assert-ReleaseGate ([string]$feedDocument.releaseState -eq [string]$ReleaseState) "Feed releaseState '$($feedDocument.releaseState)' does not match requested gate state '$ReleaseState'."
$packageName = [IO.Path]::GetFileName($package)
Assert-ReleaseGate ([IO.Path]::GetFileName(([Uri]$feedDocument.packageUrl).AbsolutePath) -eq $packageName) "Feed package URL does not identify the supplied ZIP."
Assert-ReleaseGate ([long]$feedDocument.packageBytes -eq (Get-Item -LiteralPath $package).Length) "Feed packageBytes does not match the supplied ZIP."
Assert-ReleaseGate ([string]$feedDocument.packageSha256 -eq (Get-FileHash -LiteralPath $package -Algorithm SHA256).Hash) "Feed packageSha256 does not match the supplied ZIP."

dotnet run --project $verifier -c Release -- $package ([string]$feedDocument.manifest.releaseVersion) ([string]$feedDocument.manifest.releaseCode)
Assert-ReleaseGate ($LASTEXITCODE -eq 0) "Trusted signature, governed inventory, release identity or SQLite schema gate failed."

if (-not [string]::IsNullOrWhiteSpace($DeploymentPlan)) {
    $planPath = (Resolve-Path -LiteralPath $DeploymentPlan).Path
    $planBytes = [IO.File]::ReadAllBytes($planPath)
    Assert-ReleaseGate ($planBytes.Length -gt 0 -and $planBytes[0] -eq [byte][char]'{') "Deployment plan must be BOM-less JSON."
    $plan = [Text.Encoding]::UTF8.GetString($planBytes) | ConvertFrom-Json
    Assert-ReleaseGate ([string]$plan.releaseState -eq [string]$ReleaseState) "Deployment plan releaseState '$($plan.releaseState)' does not match requested gate state '$ReleaseState'."
    Assert-ReleaseGate ($plan.files.Count -eq 2) "Deployment plan must contain exactly Installer and Portable artifacts."
    foreach ($file in $plan.files) {
        $localPath = Join-Path ([IO.Path]::GetDirectoryName($planPath)) ([string]$file.localFile)
        Assert-ReleaseGate (Test-Path -LiteralPath $localPath) "Deployment artifact is missing: $localPath"
        Assert-ReleaseGate ([long]$file.bytes -eq (Get-Item -LiteralPath $localPath).Length) "Deployment artifact byte count changed: $localPath"
        Assert-ReleaseGate ([string]$file.sha256 -eq (Get-FileHash -LiteralPath $localPath -Algorithm SHA256).Hash) "Deployment artifact SHA-256 changed: $localPath"
    }
    $stablePaths = @($plan.files | ForEach-Object { $_.stableRemotePath })
    Assert-ReleaseGate ($stablePaths -contains "/downloads/3DPIceland-Setup-x64.exe" -and $stablePaths -contains "/downloads/3DPIceland-Portable-x64.zip") "Stable deployment routes are invalid."
}

$deploymentSource = Get-Content -LiteralPath $deploymentService -Raw
Assert-ReleaseGate ($deploymentSource.Contains("OrderBy(target => StablePaths.Values.Contains(target.RemotePath, StringComparer.Ordinal) ? 1 : 0)")) "Application deployment no longer proves versioned-route-first/stable-route-last ordering."
Assert-ReleaseGate ($deploymentSource.Contains('new PublishTarget(packagePath, "/updates/" + packageName, feed.PackageBytes), new PublishTarget(fullFeed, "/updates/latest.json"')) "Update publishing no longer proves package-first/latest.json-last ordering."

Write-Host "$ReleaseState release gates PASS: clean-tree policy, NuGet vulnerabilities, BOM-less feed, bytes, SHA-256, ECDSA signature, governed inventory, SQLite schema, and stable-route-last publishing."
