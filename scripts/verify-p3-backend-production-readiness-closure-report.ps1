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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_BACKEND_PRODUCTION_READINESS_CLOSURE_REPORT_BASELINE.md"
$ReportPath = Join-Path $RepoRoot "docs/operations/P3_BACKEND_PRODUCTION_READINESS_CLOSURE_REPORT.md"
$ProductionReadinessPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"
$DeploymentEvidenceTemplatePath = Join-Path $RepoRoot "docs/operations/templates/DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md"
$FinalBlockerMatrixPath = Join-Path $RepoRoot "docs/operations/templates/PRODUCTION_READINESS_FINAL_BLOCKER_MATRIX_TEMPLATE.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 backend production readiness closure report baseline"
Assert-FileExists $ReportPath "P3 backend production readiness closure report"
Assert-FileExists $ProductionReadinessPath "P3 production deployment readiness baseline"
Assert-FileExists $DeploymentEvidenceTemplatePath "deployment evidence record template"
Assert-FileExists $FinalBlockerMatrixPath "production readiness final blocker matrix template"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Report = Get-Content $ReportPath -Raw -Encoding UTF8
$ProductionReadiness = Get-Content $ProductionReadinessPath -Raw -Encoding UTF8
$DeploymentEvidenceTemplate = Get-Content $DeploymentEvidenceTemplatePath -Raw -Encoding UTF8
$FinalBlockerMatrix = Get-Content $FinalBlockerMatrixPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Backend Production Readiness Closure Report Baseline",
    "NOT_PRODUCTION_READY",
    "CONDITIONALLY_READY_FOR_STAGING",
    "READY_FOR_PRODUCTION_WITH_EVIDENCE",
    "PRODUCTION_READY_APPROVED",
    "Required completed work summary",
    "Required implemented backend capabilities",
    "Required blockers",
    "Required final blocker matrix interpretation",
    "Required next actions",
    "Required executive summary",
    "Required technical summary",
    "P3-26K does not approve production go-live",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 backend production readiness closure report baseline"
}

$RequiredReportTokens = @(
    "P3 Backend Production Readiness Closure Report",
    "Readiness conclusion: NOT_PRODUCTION_READY",
    "Go-live decision: NO-GO",
    "CONDITIONALLY_READY_FOR_STAGING",
    "The backend is not production-ready yet.",
    "Completed P3-26 work summary",
    "Implemented backend capabilities",
    "Evidence currently available in repository",
    "Remaining hard blockers",
    "Final blocker matrix interpretation",
    "Recommended next actions",
    "Technical summary",
    "Executive conclusion",
    "SQL Server smoke evidence",
    "Deployment health smoke evidence",
    "Production JWT configuration",
    "Production CORS configuration",
    "Production AllowedHosts",
    "Production secrets source",
    "Backup and restore validation",
    "Rollback validation",
    "Observability validation",
    "Incident response drill",
    "The remaining work is environment validation, operational evidence, and formal approval."
)

foreach ($Token in $RequiredReportTokens) {
    Assert-Contains $Report $Token "P3 backend production readiness closure report"
}

Assert-Contains $ProductionReadiness "P3-26K backend production readiness closure report" "P3 production deployment readiness baseline"
Assert-Contains $DeploymentEvidenceTemplate "Backend production readiness closure report" "deployment evidence record template"
Assert-Contains $FinalBlockerMatrix "P3J-014 | Request telemetry" "production readiness final blocker matrix template"
Assert-Contains $Governance "verify-p3-backend-production-readiness-closure-report.ps1" "repository governance baseline"

Write-Host "P3 backend production readiness closure report verification passed." -ForegroundColor Green