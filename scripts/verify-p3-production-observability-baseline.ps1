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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_OBSERVABILITY_BASELINE.md"
$ProductionReadinessPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"
$SqlSmokePath = Join-Path $RepoRoot "docs/operations/P3_SQLSERVER_INTEGRATION_SMOKE_TEST_BASELINE.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 production observability baseline"
Assert-FileExists $ProductionReadinessPath "P3 production deployment readiness baseline"
Assert-FileExists $SqlSmokePath "P3 SQL Server smoke baseline"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$ProductionReadiness = Get-Content $ProductionReadinessPath -Raw -Encoding UTF8
$SqlSmoke = Get-Content $SqlSmokePath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Production Observability Baseline",
    "Production observability status: blocked.",
    "health endpoint",
    "structured application logs",
    "request correlation identifier",
    "error correlation identifier",
    "database connectivity signal",
    "authentication failure visibility",
    "authorization failure visibility",
    "sync processing failure visibility",
    "critical exception visibility",
    "rate limiting visibility",
    "Required health signals",
    "Required logging posture",
    "Required tracing posture",
    "Required metrics posture",
    "Required alerting posture",
    "Required incident response evidence",
    "Required deployment monitoring checklist",
    "Required follow-up workstreams",
    "P3-26E health endpoint and deployment smoke implementation",
    "P3-26F structured logging and correlation id implementation",
    "raw PayloadJson",
    "patient names",
    "connection strings",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 production observability baseline"
}

$RequiredProductionReadinessTokens = @(
    "P3-26D production observability baseline",
    "Production go-live status: blocked."
)

foreach ($Token in $RequiredProductionReadinessTokens) {
    Assert-Contains $ProductionReadiness $Token "P3 production deployment readiness baseline"
}

$RequiredSqlSmokeTokens = @(
    "P3 SQL Server Integration Smoke Test Baseline",
    "Production go-live remains blocked"
)

foreach ($Token in $RequiredSqlSmokeTokens) {
    Assert-Contains $SqlSmoke $Token "P3 SQL Server smoke baseline"
}

Assert-Contains $Governance "verify-p3-production-observability-baseline.ps1" "repository governance baseline"

Write-Host "P3 production observability baseline verification passed." -ForegroundColor Green