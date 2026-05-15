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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_READINESS_FINAL_BLOCKER_MATRIX_BASELINE.md"
$TemplatePath = Join-Path $RepoRoot "docs/operations/templates/PRODUCTION_READINESS_FINAL_BLOCKER_MATRIX_TEMPLATE.md"
$ProductionReadinessPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"
$DeploymentEvidenceTemplatePath = Join-Path $RepoRoot "docs/operations/templates/DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md"
$IncidentTemplatePath = Join-Path $RepoRoot "docs/operations/templates/INCIDENT_RESPONSE_RECORD_TEMPLATE.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 production readiness final blocker matrix baseline"
Assert-FileExists $TemplatePath "production readiness final blocker matrix template"
Assert-FileExists $ProductionReadinessPath "P3 production deployment readiness baseline"
Assert-FileExists $DeploymentEvidenceTemplatePath "deployment evidence record template"
Assert-FileExists $IncidentTemplatePath "incident response record template"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Template = Get-Content $TemplatePath -Raw -Encoding UTF8
$ProductionReadiness = Get-Content $ProductionReadinessPath -Raw -Encoding UTF8
$DeploymentEvidenceTemplate = Get-Content $DeploymentEvidenceTemplatePath -Raw -Encoding UTF8
$IncidentTemplate = Get-Content $IncidentTemplatePath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Production Readiness Final Blocker Matrix Baseline",
    "Production readiness status: blocked.",
    "READY",
    "BLOCKED",
    "CONDITIONAL",
    "WAIVED_WITH_APPROVAL",
    "NOT_APPLICABLE",
    "Required blocker categories",
    "Required matrix fields",
    "Required final decision",
    "Hard blockers",
    "SQL Server smoke test",
    "deployment health smoke",
    "request telemetry",
    "incident response runbook",
    "privacy/data handling evidence",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 production readiness final blocker matrix baseline"
}

$RequiredTemplateTokens = @(
    "Production Readiness Final Blocker Matrix",
    "Production readiness status: BLOCKED",
    "Final go/no-go decision: PENDING",
    "Blocker ID",
    "Category",
    "Blocker description",
    "Required evidence",
    "Current status",
    "Owner",
    "Approver",
    "Evidence link",
    "Exit criterion",
    "Risk if unresolved",
    "Target resolution date",
    "Final decision",
    "P3J-001",
    "Repository governance",
    "P3J-006",
    "SQL Server smoke test",
    "P3J-011",
    "Deployment health smoke",
    "P3J-014",
    "Request telemetry",
    "request telemetry evidence",
    "Request telemetry fields are present and sanitized",
    "P3J-015",
    "Production observability",
    "P3J-017",
    "Incident response runbook",
    "P3J-020",
    "Privacy/data handling evidence",
    "GO",
    "NO-GO",
    "CONDITIONAL-GO",
    "ROLLBACK"
)

foreach ($Token in $RequiredTemplateTokens) {
    Assert-Contains $Template $Token "production readiness final blocker matrix template"
}

Assert-Contains $ProductionReadiness "P3-26J production readiness final blocker matrix" "P3 production deployment readiness baseline"
Assert-Contains $DeploymentEvidenceTemplate "Production readiness final blocker matrix" "deployment evidence record template"
Assert-Contains $IncidentTemplate "Deployment commit SHA" "incident response record template"
Assert-Contains $Governance "verify-p3-production-readiness-final-blocker-matrix.ps1" "repository governance baseline"

Write-Host "P3 production readiness final blocker matrix verification passed." -ForegroundColor Green