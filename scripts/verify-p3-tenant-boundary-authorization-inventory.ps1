$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$InventoryPath = Join-Path $RepoRoot "docs/backend/P3_TENANT_BOUNDARY_AUTHORIZATION_INVENTORY.md"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
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

$Inventory = Get-Content $InventoryPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P3 Tenant Boundary & Authorization Hardening Inventory",
    "Access classification model",
    "Public",
    "Authenticated tenant-scoped",
    "Global-only",
    "System/internal only",
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

Write-Host "P3 tenant boundary and authorization inventory verification passed." -ForegroundColor Green