param(
    [Parameter(Mandatory = $true)][string]$CandidateRoot,
    [Parameter(Mandatory = $true)][string]$ProductionRoot
)

$ErrorActionPreference = "Stop"
$repository = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
$candidate = (Resolve-Path -LiteralPath $CandidateRoot).Path
if (-not [IO.Path]::IsPathRooted($ProductionRoot)) { $ProductionRoot = Join-Path (Get-Location).Path $ProductionRoot }
$production = [IO.Path]::GetFullPath($ProductionRoot)

$gitStatus = @(git -C $repository status --porcelain)
if ($LASTEXITCODE -ne 0) { throw "Could not inspect repository status." }
if ($gitStatus.Count -gt 0) { throw "Production promotion requires a clean Git worktree." }
if (Test-Path -LiteralPath $production) { throw "Production promotion target already exists: $production" }

$candidatePlanPath = Join-Path $candidate "deployment\application-deployment-plan.json"
$candidateFeedPath = Join-Path $candidate "feed\latest.json"
$plan = Get-Content -LiteralPath $candidatePlanPath -Raw | ConvertFrom-Json
$feed = Get-Content -LiteralPath $candidateFeedPath -Raw | ConvertFrom-Json
if ($plan.releaseState -ne "Candidate" -or $feed.releaseState -ne "Candidate") {
    throw "Promotion source must contain Candidate deployment and feed metadata."
}
if ($plan.firstInstallRoute -ne "DirectCanonicalPackage" -or $plan.files.Count -ne 2) {
    throw "Promotion source is not the governed direct installer/portable Candidate."
}

foreach ($file in $plan.files) {
    $path = Join-Path (Join-Path $candidate "deployment") ([string]$file.localFile)
    if (-not (Test-Path -LiteralPath $path)) { throw "Candidate deployment artifact is missing: $path" }
    if ((Get-Item -LiteralPath $path).Length -ne [long]$file.bytes) { throw "Candidate artifact bytes changed: $path" }
    if ((Get-FileHash -LiteralPath $path -Algorithm SHA256).Hash -ne [string]$file.sha256) { throw "Candidate artifact SHA-256 changed: $path" }
}

$packageName = [IO.Path]::GetFileName(([Uri]$feed.packageUrl).AbsolutePath)
$candidatePackagePath = Join-Path (Join-Path $candidate "feed") $packageName
if (-not (Test-Path -LiteralPath $candidatePackagePath)) { throw "Candidate feed package is missing: $candidatePackagePath" }
if ((Get-Item -LiteralPath $candidatePackagePath).Length -ne [long]$feed.packageBytes) { throw "Candidate feed package bytes changed." }
if ((Get-FileHash -LiteralPath $candidatePackagePath -Algorithm SHA256).Hash -ne [string]$feed.packageSha256) { throw "Candidate feed package SHA-256 changed." }

$deploymentOutput = Join-Path $production "deployment"
$feedOutput = Join-Path $production "feed"
New-Item -ItemType Directory -Path $deploymentOutput, $feedOutput | Out-Null
foreach ($file in $plan.files) {
    Copy-Item -LiteralPath (Join-Path (Join-Path $candidate "deployment") ([string]$file.localFile)) -Destination $deploymentOutput
}
Copy-Item -LiteralPath $candidatePackagePath -Destination $feedOutput

$head = git -C $repository rev-parse HEAD
if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($head)) { throw "Could not resolve promotion commit." }
$promotedAt = [DateTimeOffset]::UtcNow.ToString("O")
$plan.releaseState = "Production"
$plan | Add-Member -NotePropertyName promotedAtUtc -NotePropertyValue $promotedAt
$plan | Add-Member -NotePropertyName promotionCommit -NotePropertyValue $head.Trim()
$feed.releaseState = "Production"
$feed | Add-Member -NotePropertyName promotedAtUtc -NotePropertyValue $promotedAt
$feed | Add-Member -NotePropertyName promotionCommit -NotePropertyValue $head.Trim()
[IO.File]::WriteAllText(
    (Join-Path $deploymentOutput "application-deployment-plan.json"),
    ($plan | ConvertTo-Json -Depth 8),
    [Text.UTF8Encoding]::new($false))
[IO.File]::WriteAllText(
    (Join-Path $feedOutput "latest.json"),
    ($feed | ConvertTo-Json -Depth 8),
    [Text.UTF8Encoding]::new($false))

Write-Host "Production promotion ready: $production"
Write-Host "Installer/portable/update ZIP bytes were copied unchanged from the runtime-accepted Candidate."
