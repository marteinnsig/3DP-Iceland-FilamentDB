param(
    [Parameter(Mandatory = $true)]
    [string]$PrivateFtpsIdentity
)

$ErrorActionPreference = "Stop"
$repository = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path

function Rewrite-Utf8File([string]$relativePath, [scriptblock]$transform) {
    $path = Join-Path $repository $relativePath
    $original = [IO.File]::ReadAllText($path)
    $updated = & $transform $original
    if ($updated -ne $original) {
        [IO.File]::WriteAllText($path, $updated, [Text.UTF8Encoding]::new($false))
    }
}

Rewrite-Utf8File "App\FilamentDbApp\MainWindow.xaml.cs" {
    param($text)
    $pattern = '(?s)(private static List<NativeMaterialRow> GetDefaultNativeMaterialRows\(\)\s*\{)\s*#if false.*?#else\s*return new List<NativeMaterialRow>\(\);\s*#endif\s*(\})'
    $matches = [regex]::Matches($text, $pattern)
    if ($matches.Count -eq 1) {
        return [regex]::Replace($text, $pattern, '$1' + [Environment]::NewLine + '        return new List<NativeMaterialRow>();' + [Environment]::NewLine + '    }')
    }
    if ($text -notmatch '(?s)private static List<NativeMaterialRow> GetDefaultNativeMaterialRows\(\)\s*\{\s*return new List<NativeMaterialRow>\(\);\s*\}') {
        throw "Expected one disabled or already-sanitized native-material seed method; found neither."
    }
    return $text
}

Rewrite-Utf8File "App\build_deployment_artifacts.ps1" {
    param($text)
    $pattern = '(?s)\r?\n    \$privateSeedMarkers = @\(.*?\r?\n    \}\r?\n\r?\n    \$portableName'
    $matches = [regex]::Matches($text, $pattern)
    if ($matches.Count -eq 1) {
        return [regex]::Replace($text, $pattern, [Environment]::NewLine + [Environment]::NewLine + '    $portableName')
    }
    if ($text -notmatch '\$portableName' -or $text -match '\$privateSeedMarkers') {
        throw "Expected one obsolete or already-removed private-marker scan block."
    }
    return $text
}

$textExtensions = @('.cs', '.xaml', '.csproj', '.ps1', '.bat', '.iss', '.md', '.txt', '.json', '.xml', '.props', '.targets', '.sln')
$trackedFiles = @(git -C $repository ls-files)
if ($LASTEXITCODE -ne 0) { throw "Could not enumerate tracked files." }

foreach ($relativePath in $trackedFiles) {
    $extension = [IO.Path]::GetExtension($relativePath).ToLowerInvariant()
    if ($textExtensions -notcontains $extension) { continue }
    $path = Join-Path $repository $relativePath
    if (-not (Test-Path -LiteralPath $path -PathType Leaf)) { continue }
    $text = [IO.File]::ReadAllText($path)
    $updated = [regex]::Replace($text, [regex]::Escape($PrivateFtpsIdentity), '[private-ftps-identity-removed]', [Text.RegularExpressions.RegexOptions]::IgnoreCase)
    $updated = [regex]::Replace($updated, 'MAT\d{4}', '[private-material-id-removed]')
    foreach ($commitId in @('46b33e8', 'a72c25d', '29de20a', '13ca8d7', 'd2ef3d5', '1f617e4')) {
        $updated = [regex]::Replace($updated, [regex]::Escape($commitId), '[historical-commit-removed]', [Text.RegularExpressions.RegexOptions]::IgnoreCase)
    }
    if ($updated -ne $text) {
        [IO.File]::WriteAllText($path, $updated, [Text.UTF8Encoding]::new($false))
    }
}

Write-Host "Current-tree sensitive literals removed."
