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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_OPERATIONAL_INCIDENT_RESPONSE_RUNBOOK_BASELINE.md"
$TemplatePath = Join-Path $RepoRoot "docs/operations/templates/INCIDENT_RESPONSE_RECORD_TEMPLATE.md"
$ProductionReadinessPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"
$ObservabilityPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_OBSERVABILITY_BASELINE.md"
$DeploymentEvidenceTemplatePath = Join-Path $RepoRoot "docs/operations/templates/DEPLOYMENT_EVIDENCE_RECORD_TEMPLATE.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 operational incident response baseline"
Assert-FileExists $TemplatePath "incident response record template"
Assert-FileExists $ProductionReadinessPath "P3 production deployment readiness baseline"
Assert-FileExists $ObservabilityPath "P3 production observability baseline"
Assert-FileExists $DeploymentEvidenceTemplatePath "deployment evidence record template"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Template = Get-Content $TemplatePath -Raw -Encoding UTF8
$ProductionReadiness = Get-Content $ProductionReadinessPath -Raw -Encoding UTF8
$Observability = Get-Content $ObservabilityPath -Raw -Encoding UTF8
$DeploymentEvidenceTemplate = Get-Content $DeploymentEvidenceTemplatePath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Operational Incident Response Runbook Baseline",
    "Production go-live remains blocked",
    "SEV-1 Critical",
    "SEV-2 High",
    "SEV-3 Medium",
    "SEV-4 Low",
    "incident commander",
    "technical owner",
    "communications owner",
    "database owner",
    "security/privacy owner",
    "business owner",
    "detection timestamp UTC",
    "acknowledgement timestamp UTC",
    "correlation ids",
    "request ids",
    "rollback decision",
    "privacy/legal escalation status",
    "Postmortem is required",
    "follow-up PR or issue reference",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 operational incident response baseline"
}

$RequiredTemplateTokens = @(
    "Incident Response Record",
    "Incident identity",
    "Severity classification",
    "SEV-1 Critical",
    "SEV-2 High",
    "SEV-3 Medium",
    "SEV-4 Low",
    "Incident commander",
    "Technical owner",
    "Communications owner",
    "Database owner",
    "Security/privacy owner",
    "Business owner",
    "Detection timestamp UTC",
    "Acknowledgement timestamp UTC",
    "Correlation ids",
    "Request ids",
    "Health endpoint status",
    "Database connectivity status",
    "Authentication failure rate",
    "Authorization failure rate",
    "Sync rejection rate",
    "Rollback decision",
    "Privacy/legal escalation status",
    "Postmortem required",
    "Follow-up PR or issue reference",
    "OPEN",
    "MITIGATED",
    "RESOLVED",
    "POSTMORTEM_REQUIRED",
    "CLOSED"
)

foreach ($Token in $RequiredTemplateTokens) {
    Assert-Contains $Template $Token "incident response record template"
}

Assert-Contains $ProductionReadiness "P3-26I operational incident response runbook" "P3 production deployment readiness baseline"
Assert-Contains $Observability "operational incident response" "P3 production observability baseline"
Assert-Contains $DeploymentEvidenceTemplate "Escalation contact" "deployment evidence record template"
Assert-Contains $DeploymentEvidenceTemplate "Incident owner" "deployment evidence record template"
Assert-Contains $Governance "verify-p3-operational-incident-response-runbook.ps1" "repository governance baseline"

Write-Host "P3 operational incident response runbook verification passed." -ForegroundColor Green