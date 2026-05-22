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

$PlanPath = Join-Path $RepoRoot "docs/production-evidence/P3_6_PRODUCTION_EVIDENCE_IMPLEMENTATION.md"
$RegisterPath = Join-Path $RepoRoot "docs/production-evidence/evidence-register.md"
$DeploymentRunbookPath = Join-Path $RepoRoot "docs/runbooks/production-deployment-runbook.md"
$RollbackRunbookPath = Join-Path $RepoRoot "docs/runbooks/production-rollback-runbook.md"
$DatabaseRunbookPath = Join-Path $RepoRoot "docs/runbooks/database-backup-restore-runbook.md"

Assert-FileExists $PlanPath "P3.6 plan"
Assert-FileExists $RegisterPath "Production evidence register"
Assert-FileExists $DeploymentRunbookPath "Production deployment runbook"
Assert-FileExists $RollbackRunbookPath "Production rollback runbook"
Assert-FileExists $DatabaseRunbookPath "Database backup/restore runbook"

$Plan = Get-Content $PlanPath -Raw -Encoding UTF8
$Register = Get-Content $RegisterPath -Raw -Encoding UTF8
$DeploymentRunbook = Get-Content $DeploymentRunbookPath -Raw -Encoding UTF8
$RollbackRunbook = Get-Content $RollbackRunbookPath -Raw -Encoding UTF8
$DatabaseRunbook = Get-Content $DatabaseRunbookPath -Raw -Encoding UTF8

Assert-Contains $Plan "BLOCKED_PENDING_REAL_EVIDENCE" "P3.6 plan"
Assert-Contains $Plan "Deployment evidence" "P3.6 plan"
Assert-Contains $Plan "Configuration evidence" "P3.6 plan"
Assert-Contains $Plan "Database evidence" "P3.6 plan"
Assert-Contains $Plan "Security evidence" "P3.6 plan"
Assert-Contains $Plan "Observability evidence" "P3.6 plan"
Assert-Contains $Plan "Smoke test evidence" "P3.6 plan"
Assert-Contains $Plan "Rollback evidence" "P3.6 plan"

Assert-Contains $Register "P3.6-EV-001" "Production evidence register"
Assert-Contains $Register "P3.6-EV-026" "Production evidence register"
Assert-Contains $Register "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE" "Production evidence register"

Assert-Contains $DeploymentRunbook "CodeQL clean" "Production deployment runbook"
Assert-Contains $DeploymentRunbook "SQL Server target available" "Production deployment runbook"
Assert-Contains $DeploymentRunbook "Minimum smoke tests" "Production deployment runbook"

Assert-Contains $RollbackRunbook "Application rollback" "Production rollback runbook"
Assert-Contains $RollbackRunbook "Database rollback" "Production rollback runbook"
Assert-Contains $RollbackRunbook "last known good commit SHA" "Production rollback runbook"

Assert-Contains $DatabaseRunbook "Backup evidence" "Database backup/restore runbook"
Assert-Contains $DatabaseRunbook "Restore evidence" "Database backup/restore runbook"
Assert-Contains $DatabaseRunbook "A backup without a restore test is not sufficient production evidence." "Database backup/restore runbook"

Write-Host "P3.6 production evidence baseline verification passed." -ForegroundColor Green