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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_DEPLOYMENT_EVIDENCE_RELEASE_CHECKLIST_BASELINE.md"
$TemplatePath = Join-Path $RepoRoot "docs/operations/templates/DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md"
$ProductionReadinessPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"
$ObservabilityPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_OBSERVABILITY_BASELINE.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 deployment evidence baseline"
Assert-FileExists $TemplatePath "deployment evidence record template"
Assert-FileExists $ProductionReadinessPath "P3 production deployment readiness baseline"
Assert-FileExists $ObservabilityPath "P3 production observability baseline"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Template = Get-Content $TemplatePath -Raw -Encoding UTF8
$ProductionReadiness = Get-Content $ProductionReadinessPath -Raw -Encoding UTF8
$Observability = Get-Content $ObservabilityPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Deployment Evidence and Release Checklist Baseline",
    "Production go-live remains blocked",
    "Required release identity evidence",
    "Required pre-deployment evidence",
    "Required database deployment evidence",
    "Required smoke evidence",
    "Required security evidence",
    "Required observability evidence",
    "Required rollback evidence",
    "Required approval evidence",
    "git commit SHA",
    "migration script checksum",
    "SQL Server smoke command",
    "deployment health smoke command",
    "explicit go/no-go decision",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 deployment evidence baseline"
}

$RequiredTemplateTokens = @(
    "Deployment Evidence Record",
    "Release identity",
    "Pre-deployment verification",
    "Database deployment evidence",
    "Smoke test evidence",
    "Security evidence",
    "Observability evidence",
    "Rollback evidence",
    "Approval evidence",
    "Final release decision",
    "Git commit SHA",
    "Migration script checksum",
    "SQL Server smoke command",
    "Deployment health smoke command",
    "/health/live status",
    "/health/ready status",
    "No X-Dev-* authentication in production",
    "Explicit HTTPS CORS origins",
    "Security:RateLimiting:Enabled",
    "Explicit go/no-go decision",
    "GO",
    "NO-GO",
    "ROLLBACK"
)

foreach ($Token in $RequiredTemplateTokens) {
    Assert-Contains $Template $Token "deployment evidence record template"
}

Assert-Contains $ProductionReadiness "P3-26H deployment evidence template and release checklist" "P3 production deployment readiness baseline"
Assert-Contains $Observability "deployment evidence record" "P3 production observability baseline"
Assert-Contains $Governance "verify-p3-deployment-evidence-release-checklist.ps1" "repository governance baseline"

Write-Host "P3 deployment evidence and release checklist verification passed." -ForegroundColor Green