$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DecisionRegisterPath = Join-Path $RepoRoot "docs/backend/P3_ARCHITECTURE_BUSINESS_RULES_DECISION_REGISTER.md"

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

Assert-FileExists $DecisionRegisterPath

$DecisionRegister = Get-Content $DecisionRegisterPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P3 Architecture & Business Rules Decision Register",
    "P3 execution order",
    "database modularity",
    "SQL Server and Azure readiness",
    "patient identity and expediente",
    "patient data recapture",
    "vital signs",
    "systolic and diastolic blood pressure must be stored separately",
    "visits and encounters",
    "offline and sync",
    "manual sync button",
    "automatic sync with conservative backoff",
    "Zero Trust and deny-by-default",
    "Traffic governance",
    "Tenant boundary",
    "public",
    "authenticated tenant-scoped",
    "global-only",
    "API and endpoint decision",
    "Audit and traceability",
    "Explicitly out of scope",
    "P3 acceptance criteria"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $DecisionRegister $Token "P3 architecture/business rules decision register"
}

Write-Host "P3 architecture/business rules decision register verification passed." -ForegroundColor Green