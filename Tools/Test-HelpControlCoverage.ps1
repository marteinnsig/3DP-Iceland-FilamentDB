param(
    [string]$RepositoryRoot = "",
    [switch]$UpdateInventory
)

$ErrorActionPreference = "Stop"

if ([string]::IsNullOrWhiteSpace($RepositoryRoot)) {
    $RepositoryRoot = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path
} else {
    $RepositoryRoot = (Resolve-Path -LiteralPath $RepositoryRoot).Path
}

$registryPath = Join-Path $RepositoryRoot "Docs\HelpControlCoverageRegistry.json"
$ledgerPath = Join-Path $RepositoryRoot "Docs\HELP_CONTROL_FIELD_LEDGER.md"
$helpCatalogPath = Join-Path $RepositoryRoot "App\FilamentDbApp\HelpContentCatalog.cs"
$mainWindowCodePath = Join-Path $RepositoryRoot "App\FilamentDbApp\MainWindow.xaml.cs"
$inventoryPath = Join-Path $RepositoryRoot "Docs\HelpControlInventory.tsv"

foreach ($requiredPath in @($registryPath, $ledgerPath, $helpCatalogPath, $mainWindowCodePath)) {
    if (-not (Test-Path -LiteralPath $requiredPath)) {
        throw "Required Help coverage source is missing: $requiredPath"
    }
}

$registry = [IO.File]::ReadAllText($registryPath, [Text.Encoding]::UTF8) |
    ConvertFrom-Json
$xamlPath = Join-Path $RepositoryRoot ([string]$registry.XamlSource)
if (-not (Test-Path -LiteralPath $xamlPath)) {
    throw "Registered XAML source is missing: $xamlPath"
}

$xaml = [xml]([IO.File]::ReadAllText($xamlPath, [Text.Encoding]::UTF8))
$namespaces = [Xml.XmlNamespaceManager]::new($xaml.NameTable)
$namespaces.AddNamespace("w", "http://schemas.microsoft.com/winfx/2006/xaml/presentation")
$namespaces.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml")

$failures = [Collections.Generic.List[string]]::new()
$summaries = [Collections.Generic.List[string]]::new()
$controlTypes = @(
    "Button",
    "MenuItem",
    "TextBox",
    "PasswordBox",
    "ComboBox",
    "CheckBox",
    "RadioButton",
    "DatePicker",
    "DataGrid"
)

function ConvertTo-KeyPart([string]$Value) {
    $part = $Value.Trim().ToLowerInvariant()
    $part = [Text.RegularExpressions.Regex]::Replace($part, "[^a-z0-9]+", "-")
    $part = $part.Trim("-")
    if ([string]::IsNullOrWhiteSpace($part)) {
        return "anonymous"
    }
    return $part
}

function Get-XamlName([Xml.XmlElement]$Element) {
    $xamlName = $Element.GetAttribute(
        "Name",
        "http://schemas.microsoft.com/winfx/2006/xaml")
    if (-not [string]::IsNullOrWhiteSpace($xamlName)) {
        return $xamlName
    }
    return $Element.GetAttribute("Name")
}

function Get-SurfacePath([Xml.XmlElement]$Element) {
    $headers = [Collections.Generic.List[string]]::new()
    $current = $Element.ParentNode
    while ($null -ne $current) {
        if ($current -is [Xml.XmlElement] -and $current.LocalName -eq "TabItem") {
            $header = $current.GetAttribute("Header")
            if (-not [string]::IsNullOrWhiteSpace($header)) {
                $headers.Insert(0, $header)
            }
        }
        $current = $current.ParentNode
    }
    if ($headers.Count -gt 0) {
        return $headers -join " > "
    }
    if ($Element.LocalName -eq "MenuItem") {
        return "Application menu"
    }
    return "Application shell"
}

function Get-TopSurface([string]$SurfacePath) {
    if ($SurfacePath.Contains(" > ")) {
        return $SurfacePath.Split(@(" > "), [StringSplitOptions]::None)[0]
    }
    return $SurfacePath
}

function Get-OwnerIncrement([string]$SurfacePath, [string]$Identity) {
    if ($Identity -match "^AutomationLandedCost") {
        return "v53.0.4.1"
    }
    if ($Identity -match "^(LandedCostCurrencySelector|ApplyLandedCostCurrencyOverride)$") {
        return "v53.0.2"
    }
    if ($Identity -match "CopyOpenAiOperationalEvidence") {
        return "v52.3.2"
    }
    if ($Identity -match "OpenAi|OpenAI") {
        return "v52.2"
    }
    if ($Identity -match "AiProvider") {
        return "v52.1"
    }
    $topSurface = Get-TopSurface $SurfacePath
    if (@(
            "Materials", "Manufacturers", "Purchase Orders", "Inventory",
            "Usage", "Printers", "Print Job Quotes", "Base Materials",
            "Settings Manager"
        ) -contains $topSurface) {
        return "v50.4.1"
    }
    if (@(
            "Experimental Testing", "Material Detail", "Tensile Measurements",
            "Impact Measurements", "Stiffness Measurements",
            "Rankings Dashboard", "Category Rankings", "Awards & Winners",
            "Dashboard Insights"
        ) -contains $topSurface) {
        return "v50.4.2"
    }
    return "v50.4.3"
}

function Get-HelpDestination([string]$SurfacePath) {
    $topSurface = Get-TopSurface $SurfacePath
    $destinations = @{
        "Materials" = "materials.controls-fields"
        "Manufacturers" = "manufacturers.controls-fields"
        "Purchase Orders" = "purchase-orders.controls-fields"
        "Inventory" = "inventory.controls-fields"
        "Usage" = "usage.controls-fields"
        "Printers" = "printers.controls-fields"
        "Print Job Quotes" = "print-job-quotes.controls-fields"
        "Base Materials" = "base-materials.controls-fields"
        "Settings Manager" = "settings.controls-fields"
        "Experimental Testing" = "experimental.controls-fields"
        "Material Detail" = "material-detail.controls-fields"
        "Tensile Measurements" = "measurements.controls-fields"
        "Impact Measurements" = "measurements.controls-fields"
        "Stiffness Measurements" = "measurements.controls-fields"
        "Rankings Dashboard" = "analysis.controls-fields"
        "Category Rankings" = "analysis.controls-fields"
        "Awards & Winners" = "analysis.controls-fields"
        "Dashboard Insights" = "analysis.controls-fields"
        "Reports / PDF Export" = "reports.controls-fields"
        "Website Export" = "website.controls-fields"
        "AI Assistant" = "ai.controls-fields"
        "YouTube Research" = "youtube.controls-fields"
        "Application menu" = "menu-runtime.controls-fields"
        "Application shell" = "menu-runtime.controls-fields"
    }
    if ($destinations.ContainsKey($topSurface)) {
        return $destinations[$topSurface]
    }
    return "start.workflow"
}

function Get-ControlClassification([string]$TypeName, [Xml.XmlElement]$Element) {
    if ($TypeName -in @("Button", "MenuItem")) {
        return "action"
    }
    if ($TypeName -in @("ComboBox", "CheckBox", "RadioButton")) {
        return "choice"
    }
    if ($TypeName -in @("TextBox", "PasswordBox", "DatePicker")) {
        if ($Element.GetAttribute("IsReadOnly") -eq "True") {
            return "read-only"
        }
        return "input-candidate"
    }
    if ($TypeName -eq "DataGrid") {
        if ($Element.GetAttribute("IsReadOnly") -eq "True") {
            return "read-only-grid"
        }
        return "grid-candidate"
    }
    if ($TypeName.EndsWith("Column", [StringComparison]::Ordinal)) {
        if ($Element.GetAttribute("IsReadOnly") -eq "True") {
            return "read-only-column"
        }
        return "editable-column-candidate"
    }
    return "unclassified"
}

function Get-ControlIdentity([string]$TypeName, [Xml.XmlElement]$Element) {
    $values = @(
        $Element.GetAttribute("AutomationProperties.AutomationId"),
        (Get-XamlName $Element),
        $Element.GetAttribute("Click"),
        $Element.GetAttribute("SelectionChanged"),
        $Element.GetAttribute("Checked"),
        $Element.GetAttribute("Header"),
        $Element.GetAttribute("Content"),
        $Element.GetAttribute("Binding"),
        $Element.GetAttribute("SelectedItemBinding"),
        $Element.GetAttribute("SelectedValueBinding")
    )
    foreach ($value in $values) {
        if (-not [string]::IsNullOrWhiteSpace($value)) {
            return $value
        }
    }
    return $TypeName
}

function ConvertTo-TsvValue([string]$Value) {
    return $Value.Replace("`t", " ").Replace("`r", " ").Replace("`n", " ")
}

function New-InventoryRows {
    $rows = [Collections.Generic.List[object]]::new()
    $keyCounts = @{}
    $inventoryTypes = @($controlTypes) + @(
        "DataGridTextColumn",
        "DataGridCheckBoxColumn",
        "DataGridComboBoxColumn",
        "DataGridTemplateColumn"
    )
    $coveredOwnerIncrements = @(
        $registry.CoveredOwnerIncrements |
            ForEach-Object { [string]$_ }
    )
    foreach ($typeName in $inventoryTypes) {
        foreach ($element in @($xaml.SelectNodes("//w:$typeName", $namespaces))) {
            $surface = Get-SurfacePath $element
            $identity = Get-ControlIdentity $typeName $element
            $baseKey = "xaml.$(ConvertTo-KeyPart $surface).$(ConvertTo-KeyPart $typeName).$(ConvertTo-KeyPart $identity)"
            if (-not $keyCounts.ContainsKey($baseKey)) {
                $keyCounts[$baseKey] = 0
            }
            $keyCounts[$baseKey]++
            $key = if ($keyCounts[$baseKey] -eq 1) {
                $baseKey
            } else {
                "$baseKey-$($keyCounts[$baseKey])"
            }
            $ownerIncrement = Get-OwnerIncrement $surface $identity
            $rows.Add([pscustomobject]@{
                Key = $key
                Surface = $surface
                Type = $typeName
                Identity = $identity
                Classification = Get-ControlClassification $typeName $element
                OwnerIncrement = $ownerIncrement
                HelpDestination = Get-HelpDestination $surface
                Status = if ($coveredOwnerIncrements -contains $ownerIncrement) {
                    "covered"
                } else {
                    "planned"
                }
            })
        }
    }
    return @($rows | Sort-Object Key)
}

function Convert-InventoryToLines([object[]]$Rows) {
    $lines = [Collections.Generic.List[string]]::new()
    $lines.Add("Key`tSurface`tType`tIdentity`tClassification`tOwnerIncrement`tHelpDestination`tStatus")
    foreach ($row in $Rows) {
        $values = @(
            $row.Key,
            $row.Surface,
            $row.Type,
            $row.Identity,
            $row.Classification,
            $row.OwnerIncrement,
            $row.HelpDestination,
            $row.Status
        ) | ForEach-Object { ConvertTo-TsvValue ([string]$_) }
        $lines.Add($values -join "`t")
    }
    return @($lines)
}

if ([string]$registry.Schema -ne "3dpiceland.help-control-coverage.v1") {
    $failures.Add("Unknown Help control registry schema '$($registry.Schema)'.")
}

foreach ($controlType in $controlTypes) {
    $actual = @($xaml.SelectNodes("//w:$controlType", $namespaces)).Count
    $expected = [int]$registry.XamlCounts.$controlType
    $summaries.Add("$controlType $actual/$expected")
    if ($actual -ne $expected) {
        $failures.Add("$controlType discovery drifted: expected $expected, found $actual.")
    }
}

$columnExpression = @(
    "//w:DataGridTextColumn",
    "//w:DataGridCheckBoxColumn",
    "//w:DataGridComboBoxColumn",
    "//w:DataGridTemplateColumn"
) -join "|"
$actualColumns = @($xaml.SelectNodes($columnExpression, $namespaces)).Count
$expectedColumns = [int]$registry.XamlCounts.DataGridColumn
$summaries.Add("DataGridColumn $actualColumns/$expectedColumns")
if ($actualColumns -ne $expectedColumns) {
    $failures.Add("DataGridColumn discovery drifted: expected $expectedColumns, found $actualColumns.")
}

$topLevelTabs = @($xaml.SelectNodes(
    "//w:TabControl[@x:Name='WorkspaceTabs']/w:TabItem",
    $namespaces))
$nestedTabControls = @(
    "ExperimentalMeasurementEditors",
    "ExperimentalResultsViewTabs",
    "MaterialDetailTabs"
)
$nestedTabs = foreach ($tabControlName in $nestedTabControls) {
    $xaml.SelectNodes(
        "//w:TabControl[@x:Name='$tabControlName' or @Name='$tabControlName']/w:TabItem",
        $namespaces)
}

if ($topLevelTabs.Count -ne [int]$registry.SurfaceCounts.TopLevelTab) {
    $failures.Add(
        "Top-level tab discovery drifted: expected $($registry.SurfaceCounts.TopLevelTab), found $($topLevelTabs.Count).")
}
if (@($nestedTabs).Count -ne [int]$registry.SurfaceCounts.NestedTab) {
    $failures.Add(
        "Nested-tab discovery drifted: expected $($registry.SurfaceCounts.NestedTab), found $(@($nestedTabs).Count).")
}

$mainWindowCodeFiles = @(
    [IO.Directory]::GetFiles(
        (Split-Path -Parent $mainWindowCodePath),
        "MainWindow*.cs",
        [IO.SearchOption]::TopDirectoryOnly) |
        Sort-Object
)
$mainWindowCodeParts = @(
    $mainWindowCodeFiles |
        ForEach-Object { [IO.File]::ReadAllText($_, [Text.Encoding]::UTF8) }
)
$mainWindowCode = [string]::Join("`n", $mainWindowCodeParts)
$allCode = [IO.File]::ReadAllText($helpCatalogPath, [Text.Encoding]::UTF8) + "`n" + $mainWindowCode
$allowedStatuses = @($registry.AllowedStatuses | ForEach-Object { [string]$_ })

foreach ($customRegistry in $registry.CustomColumnRegistries) {
    if ($mainWindowCode.IndexOf(
            [string]$customRegistry.Builder,
            [StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Custom column builder is missing: $($customRegistry.Builder).")
    }
    if ($allCode.IndexOf(
            [string]$customRegistry.HelpDestination,
            [StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Custom registry Help destination is missing: $($customRegistry.HelpDestination).")
    }
    if ($mainWindowCode.IndexOf(
            [string]$customRegistry.KeyContract,
            [StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Custom registry key contract is missing: $($customRegistry.KeyContract).")
    }
    if ($allowedStatuses -notcontains [string]$customRegistry.Status) {
        $failures.Add("Custom registry '$($customRegistry.Key)' has invalid status '$($customRegistry.Status)'.")
    }
}

foreach ($runtimeSurface in $registry.RuntimeSurfaces) {
    if ($mainWindowCode.IndexOf(
            [string]$runtimeSurface.EntryPoint,
            [StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Runtime entry point is missing: $($runtimeSurface.EntryPoint).")
    }
    if ($allCode.IndexOf(
            [string]$runtimeSurface.HelpDestination,
            [StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Runtime Help destination is missing: $($runtimeSurface.HelpDestination).")
    }
    if ($allowedStatuses -notcontains [string]$runtimeSurface.Status) {
        $failures.Add("Runtime surface '$($runtimeSurface.Key)' has invalid status '$($runtimeSurface.Status)'.")
    }
    $runtimeControls = @($runtimeSurface.Controls | ForEach-Object { [string]$_ })
    if ($runtimeControls.Count -eq 0) {
        $failures.Add("Runtime surface '$($runtimeSurface.Key)' has no declared controls.")
    }
    $duplicateRuntimeControls = @(
        $runtimeControls |
            Group-Object |
            Where-Object Count -gt 1
    )
    if ($duplicateRuntimeControls.Count -gt 0) {
        $failures.Add(
            "Runtime surface '$($runtimeSurface.Key)' has duplicate controls: " +
            "$($duplicateRuntimeControls.Name -join ', ').")
    }
}

$postV50RequiredIncrements = @(
    "v51.1", "v51.2", "v51.3", "v51.4",
    "v52.1", "v52.2", "v52.3.2",
    "v53.0.2", "v53.0.3", "v53.0.4.1", "v53.0.4.2",
    "v53.0.4.3", "v53.0.4.4", "v53.0.5", "v54.0.5", "v54.0.6", "v55.0.2", "v55.0.5", "v55.0.5.1", "v55.0.6"
)
$coveredOwnerIncrements = @(
    $registry.CoveredOwnerIncrements |
        ForEach-Object { [string]$_ }
)
foreach ($requiredIncrement in $postV50RequiredIncrements) {
    if ($coveredOwnerIncrements -notcontains $requiredIncrement) {
        $failures.Add("Post-v50 Help owner increment is missing: $requiredIncrement.")
    }
}

$postV50Markers = @(
    "OWNER / PRODUCTION",
    "CLEAN / READINESS",
    "CanonicalDataDependent",
    "Windows Credential Manager",
    "Preview OpenAI Payload",
    "Copy Operational Evidence",
    "Default Landed Cost Currency",
    "Manual Governed Settings",
    "calculation UTC and calculation version",
    "opening or refreshing Diagnostics never runs either one"
)
foreach ($marker in $postV50Markers) {
    if ($allCode.IndexOf($marker, [StringComparison]::Ordinal) -lt 0) {
        $failures.Add("Post-v50 Help contract marker is missing: $marker.")
    }
}

$duplicateKeys = @(
    @($registry.CustomColumnRegistries.Key) + @($registry.RuntimeSurfaces.Key) |
        Group-Object |
        Where-Object Count -gt 1
)
if ($duplicateKeys.Count -gt 0) {
    $failures.Add("Duplicate registry keys: $($duplicateKeys.Name -join ', ').")
}

$inventoryRows = @(New-InventoryRows)
$inventoryLines = @(Convert-InventoryToLines $inventoryRows)
if ($UpdateInventory) {
    [IO.File]::WriteAllLines(
        $inventoryPath,
        $inventoryLines,
        [Text.UTF8Encoding]::new($false))
} elseif (-not (Test-Path -LiteralPath $inventoryPath)) {
    $failures.Add("Committed per-control inventory is missing: $inventoryPath")
} else {
    $committedInventoryLines = [IO.File]::ReadAllLines(
        $inventoryPath,
        [Text.Encoding]::UTF8)
    $generatedInventoryText = [string]::Join("`n", $inventoryLines)
    $committedInventoryText = [string]::Join("`n", $committedInventoryLines)
    if (-not [string]::Equals(
            $generatedInventoryText,
            $committedInventoryText,
            [StringComparison]::Ordinal)) {
        $failures.Add(
            "Committed per-control inventory drifted. Review source changes, then run with -UpdateInventory.")
    }
}

Write-Host "# Help Control Coverage Audit"
Write-Host ""
Write-Host "Registry: $($registry.Release) / $($registry.Schema)"
Write-Host "XAML: $($summaries -join '; ')"
Write-Host "Tabs: $($topLevelTabs.Count) top-level; $(@($nestedTabs).Count) nested"
Write-Host "Custom registries: $(@($registry.CustomColumnRegistries).Count)"
Write-Host "Runtime surfaces: $(@($registry.RuntimeSurfaces).Count)"
Write-Host "Individual XAML candidates: $($inventoryRows.Count)"
if ($UpdateInventory) {
    Write-Host "Inventory refreshed: $inventoryPath"
}
Write-Host ""

if ($failures.Count -gt 0) {
    Write-Host "FAIL - $($failures.Count) blocking issue(s)."
    foreach ($failure in $failures) {
        Write-Host "- $failure"
    }
    exit 1
}

Write-Host "PASS - discovery counts, owners, destinations, statuses and keys match the post-v50 reconciled registry."
Write-Host "NOTE - planned rows remain implementation gaps until their status becomes covered or manual-only."
