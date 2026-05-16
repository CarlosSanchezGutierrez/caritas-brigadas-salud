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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_OBSERVABILITY_INCIDENT_RESPONSE_EVIDENCE_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/operations/P3_5_OBSERVABILITY_INCIDENT_RESPONSE_EVIDENCE_CONTRACT.md"

Assert-FileExists $BaselinePath "P3.5 observability incident response evidence baseline"
Assert-FileExists $ContractPath "P3.5 observability incident response evidence contract"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "A production system without observability is operationally blind",
    "Liveness health check",
    "Readiness health check",
    "Database connectivity health check",
    "Structured logs",
    "Correlation id",
    "Request telemetry",
    "Dashboard or equivalent operational view",
    "Alert routing",
    "Incident runbook",
    "Post-incident review process",
    "Security incident requirements",
    "Mobile incident requirements",
    "Web admin incident requirements",
    "Default state is BLOCKED"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3.5 observability incident response evidence baseline"
}

$RequiredContractTokens = @(
    "Status: BLOCKED",
    "Production readiness requires observable behavior",
    "Health check evidence",
    "Structured logging evidence",
    "Metrics evidence",
    "Tracing and correlation evidence",
    "Monitoring stack decision",
    "Dashboard evidence",
    "Alerting evidence",
    "Incident response evidence",
    "Security incident paths",
    "Mobile incident paths",
    "Web admin incident paths",
    "Evidence record template",
    "Production observability readiness",
    "BLOCKED"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3.5 observability incident response evidence contract"
}

Write-Host "P3.5 observability incident response evidence contract verification passed." -ForegroundColor Green