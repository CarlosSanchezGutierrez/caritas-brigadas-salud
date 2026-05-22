$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

function Assert-FileExists {
    param(
        [string]$Path,
        [string]$Label
    )

    if (-not (Test-Path $Path)) {
        throw "$Label file not found: $Path"
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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_ADMIN_REPORTING_BACKEND_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/operations/P3_5_ADMIN_REPORTING_BACKEND_CONTRACT.md"

Assert-FileExists $BaselinePath "P3.5 admin reporting backend baseline"
Assert-FileExists $ContractPath "P3.5 admin reporting backend contract"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "Web Admin and reporting users must never connect directly to SQL Server",
    "Web Admin -> HTTPS -> API -> SQL Server",
    "Administrative reporting goal",
    "Daily patient counts",
    "Daily service counts",
    "Export requirements",
    "Privacy requirements",
    "Audit requirements",
    "Data quality requirements",
    "Performance requirements",
    "Web Admin requirements",
    "Default state is BLOCKED"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3.5 admin reporting backend baseline"
}

$RequiredContractTokens = @(
    "Status: BLOCKED",
    "Web Admin and reporting users must never connect directly to SQL Server",
    "Reporting roles and permissions",
    "Dashboard metric evidence",
    "Report endpoint evidence",
    "Export evidence",
    "Privacy controls",
    "Audit logging evidence",
    "Data quality indicators",
    "Reporting API requirements",
    "Performance and scalability evidence",
    "Web Admin dependency evidence",
    "Production reporting readiness",
    "BLOCKED"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3.5 admin reporting backend contract"
}

Write-Host "P3.5 admin reporting backend contract verification passed." -ForegroundColor Green