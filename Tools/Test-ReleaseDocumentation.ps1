param(
    [string]$RepositoryRoot = "",
    [string]$ReportPath = ""
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RepositoryRoot)) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
} else {
    $RepositoryRoot = (Resolve-Path -LiteralPath $RepositoryRoot).Path
}

$baselinePath = Join-Path $RepositoryRoot "Docs\ReleaseDocumentationAuditBaseline.json"
if (-not (Test-Path -LiteralPath $baselinePath)) {
    throw "Release documentation audit baseline is missing: $baselinePath"
}

$baseline = [IO.File]::ReadAllText($baselinePath, [Text.Encoding]::UTF8) | ConvertFrom-Json
$failures = [Collections.Generic.List[string]]::new()
$warnings = [Collections.Generic.List[string]]::new()
$summaries = [Collections.Generic.List[object]]::new()

function Normalize-DocumentPath([string]$Path) {
    return $Path.Replace("\", "/")
}

function Read-VersionHeadings([string]$RelativePath) {
    $fullPath = Join-Path $RepositoryRoot $RelativePath
    if (-not (Test-Path -LiteralPath $fullPath)) {
        $failures.Add("Required release document is missing: $RelativePath")
        return @()
    }

    $lineNumber = 0
    $items = foreach ($line in [IO.File]::ReadAllLines($fullPath, [Text.Encoding]::UTF8)) {
        $lineNumber++
        if ($line -match '^#{1,6}\s+v(?<version>\d+(?:\.\d+){1,3})\s+(?<title>.*)$') {
            $title = ($Matches.title.Trim() -replace '^[^A-Za-z0-9.]+', '').Trim()
            [pscustomobject]@{
                Document = Normalize-DocumentPath $RelativePath
                Version = $Matches.version
                Title = $title
                Line = $lineNumber
            }
        }
    }

    return @($items)
}

$headingsByDocument = @{}
foreach ($document in $baseline.Documents) {
    $relativePath = Normalize-DocumentPath ([string]$document.Path)
    $fullPath = Join-Path $RepositoryRoot $relativePath
    $content = if (Test-Path -LiteralPath $fullPath) {
        [IO.File]::ReadAllText($fullPath, [Text.Encoding]::UTF8)
    } else {
        ""
    }

    if (-not $content.Contains([string]$document.OwnershipMarker)) {
        $failures.Add("$relativePath is missing its canonical ownership marker.")
    }

    $headings = @(Read-VersionHeadings $relativePath)
    $headingsByDocument[$relativePath] = $headings
    $summaries.Add([pscustomobject]@{
        Document = $relativePath
        Role = [string]$document.Role
        Headings = $headings.Count
        UniqueVersions = @($headings.Version | Sort-Object -Unique).Count
    })
}

$knownDuplicateKeys = @{}
foreach ($known in $baseline.KnownHistoricalDuplicates) {
    $document = Normalize-DocumentPath ([string]$known.Document)
    $key = "$document|$($known.Version)"
    $knownDuplicateKeys[$key] = $known
}

foreach ($document in $headingsByDocument.Keys) {
    foreach ($group in @($headingsByDocument[$document] | Group-Object Version | Where-Object Count -gt 1)) {
        $key = "$document|$($group.Name)"
        if (-not $knownDuplicateKeys.ContainsKey($key)) {
            $locations = ($group.Group | ForEach-Object { "$($_.Line): $($_.Title)" }) -join "; "
            $failures.Add("New duplicate release version v$($group.Name) in $document ($locations).")
            continue
        }

        $known = $knownDuplicateKeys[$key]
        if ($group.Count -gt [int]$known.MaxOccurrences) {
            $failures.Add("Known duplicate v$($group.Name) in $document grew from at most $($known.MaxOccurrences) to $($group.Count) occurrences.")
        }

        $allowedTitles = @($known.AllowedTitles | ForEach-Object { [string]$_ })
        foreach ($title in @($group.Group.Title | Sort-Object -Unique)) {
            if ($allowedTitles -notcontains [string]$title) {
                $failures.Add("Known duplicate v$($group.Name) in $document gained an unapproved title: '$title'.")
            }
        }

        $warnings.Add("Known historical duplicate retained: $document v$($group.Name) ($($group.Count) occurrences).")
    }
}

$knownCrossDocumentKeys = @{}
foreach ($known in $baseline.KnownHistoricalCrossDocumentTitles) {
    $knownCrossDocumentKeys[[string]$known.Version] = $known
}

$allHeadings = @($headingsByDocument.Values | ForEach-Object { $_ })
foreach ($group in @($allHeadings | Group-Object Version)) {
    $titles = @($group.Group.Title | Sort-Object -Unique)
    if ($titles.Count -le 1) {
        continue
    }

    if (-not $knownCrossDocumentKeys.ContainsKey($group.Name)) {
        $failures.Add("New cross-document title conflict for v$($group.Name): $($titles -join '; ').")
        continue
    }

    $allowedTitles = @($knownCrossDocumentKeys[$group.Name].AllowedTitles | ForEach-Object { [string]$_ })
    foreach ($title in $titles) {
        if ($allowedTitles -notcontains $title) {
            $failures.Add("Historical cross-document v$($group.Name) gained an unapproved title: '$title'.")
        }
    }
    $warnings.Add("Known historical cross-document title variants retained: v$($group.Name).")
}

foreach ($release in $baseline.CanonicalSeries) {
    foreach ($document in @($release.RequiredDocuments)) {
        $normalizedDocument = Normalize-DocumentPath ([string]$document)
        $releaseVersion = [string]$release.Version
        $matching = @($headingsByDocument[$normalizedDocument] | Where-Object { $_.Version -eq $releaseVersion })
        if ($matching.Count -ne 1) {
            $failures.Add("$normalizedDocument must contain exactly one v$($release.Version) heading; found $($matching.Count).")
            continue
        }

        if ($matching[0].Title -ne [string]$release.Title) {
            $failures.Add("$normalizedDocument v$($release.Version) title '$($matching[0].Title)' does not match canonical title '$($release.Title)'.")
        }
    }
}

$lines = [Collections.Generic.List[string]]::new()
$lines.Add("# Release Documentation Audit")
$lines.Add("")
$lines.Add("Read-only result generated by `Tools/Test-ReleaseDocumentation.ps1`.")
$lines.Add("")
$lines.Add("## Document ownership")
$lines.Add("")
$lines.Add("| Document | Canonical role | Version headings | Unique versions |")
$lines.Add("|---|---|---:|---:|")
foreach ($summary in $summaries) {
    $lines.Add("| ``$($summary.Document)`` | $($summary.Role) | $($summary.Headings) | $($summary.UniqueVersions) |")
}
$lines.Add("")
$lines.Add("## Result")
$lines.Add("")
if ($failures.Count -eq 0) {
    $lines.Add("PASS - no new duplicate identifiers, recent canonical release gaps or title conflicts.")
} else {
    $lines.Add("FAIL - $($failures.Count) blocking issue(s).")
}
$lines.Add("")
$lines.Add("Known historical duplicates are baseline-bounded warnings. The audit never edits, deletes, renumbers or reorders release history.")

if ($warnings.Count -gt 0) {
    $lines.Add("")
    $lines.Add("## Baseline warnings")
    $lines.Add("")
    foreach ($warning in $warnings) {
        $lines.Add("- $warning")
    }
}

if ($failures.Count -gt 0) {
    $lines.Add("")
    $lines.Add("## Blocking issues")
    $lines.Add("")
    foreach ($failure in $failures) {
        $lines.Add("- $failure")
    }
}

if (-not [string]::IsNullOrWhiteSpace($ReportPath)) {
    $resolvedReportPath = if ([IO.Path]::IsPathRooted($ReportPath)) {
        $ReportPath
    } else {
        Join-Path $RepositoryRoot $ReportPath
    }
    [IO.File]::WriteAllLines($resolvedReportPath, $lines, [Text.UTF8Encoding]::new($false))
}

foreach ($line in $lines) {
    Write-Host $line
}

if ($failures.Count -gt 0) {
    exit 1
}
