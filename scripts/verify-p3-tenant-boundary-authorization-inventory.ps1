$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$InventoryPath = Join-Path $RepoRoot "docs/backend/P3_TENANT_BOUNDARY_AUTHORIZATION_INVENTORY.md"
$ControllersPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Controllers"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
    }
}

function Assert-DirectoryExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required directory not found: $Path"
    }
}

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if (-not $Content.Contains($Token)) {
        throw "$Label does not contain required token: $Token"
    }
}

Assert-FileExists $InventoryPath
Assert-DirectoryExists $ControllersPath

$Inventory = Get-Content $InventoryPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P3 Tenant Boundary & Authorization Hardening Inventory",
    "Access classification model",
    "Public",
    "Authenticated global",
    "Authenticated tenant-scoped",
    "Authenticated self-scoped",
    "Global-only",
    "System/internal only",
    "No composite classifications are allowed",
    "Tenant boundary principles",
    "Claims and principal inventory",
    "RoleCode",
    "LegacyRole",
    "Permission and role inventory",
    "Endpoint classification inventory",
    "Data domain tenant scope inventory",
    "Authorization hardening risks",
    "Controller-only checks",
    "Missing OrganizationId claim",
    "Global-only drift",
    "Legacy claim drift",
    "List endpoint leakage",
    "Required P3-02 endpoint authorization tests",
    "Required P3-03 tenant scope tests",
    "Tenant boundary for expediente and clinical record",
    "Tenant boundary for vital signs",
    "Tenant boundary for offline/sync",
    "Zero Trust and traffic governance implications",
    "P3-01 output"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $Inventory $Token "P3 tenant boundary and authorization inventory"
}

$ControllerNames = Get-ChildItem $ControllersPath -File -Filter "*Controller.cs" |
    Select-Object -ExpandProperty BaseName |
    Sort-Object

foreach ($ControllerName in $ControllerNames) {
    Assert-Contains $Inventory $ControllerName "P3 tenant boundary and authorization inventory"
}

$AllowedClassifications = @(
    "Public",
    "Authenticated global",
    "Authenticated tenant-scoped",
    "Authenticated self-scoped",
    "Global-only",
    "System/internal only"
)

$ForbiddenCompositeClassifications = @(
    "Public or system-safe",
    "Tenant-scoped with global guardrails",
    "Tenant-scoped/system constrained",
    "Authenticated tenant-scoped with global guardrails",
    "System constrained",
    "System-safe"
)

$EndpointSectionMatch = [regex]::Match(
    $Inventory,
    "(?s)## 6\. Endpoint classification inventory.*?(?=## 7\. Data domain tenant scope inventory)"
)

if (-not $EndpointSectionMatch.Success) {
    throw "Endpoint classification inventory section was not found."
}

$EndpointSection = $EndpointSectionMatch.Value
$EndpointRows = $EndpointSection -split "`r?`n" |
    Where-Object {
        $_.Trim().StartsWith("|") -and
        $_ -notmatch "\|---" -and
        $_ -notmatch "Controller / area"
    }

if ($EndpointRows.Count -eq 0) {
    throw "Endpoint classification inventory has no endpoint rows."
}

foreach ($Row in $EndpointRows) {
    $Columns = $Row.Trim().Trim("|").Split("|") | ForEach-Object { $_.Trim() }

    if ($Columns.Count -lt 4) {
        throw "Endpoint classification row must have at least 4 columns: $Row"
    }

    $Classification = $Columns[2]

    if ($AllowedClassifications -notcontains $Classification) {
        throw "Endpoint row uses non-canonical classification '$Classification'. Row: $Row"
    }

    if ($ForbiddenCompositeClassifications -contains $Classification) {
        throw "Endpoint row uses forbidden composite classification '$Classification'. Row: $Row"
    }
}

Write-Host "P3 tenant boundary and authorization inventory verification passed." -ForegroundColor Green
