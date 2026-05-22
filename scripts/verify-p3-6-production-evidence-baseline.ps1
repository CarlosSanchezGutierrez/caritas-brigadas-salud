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

Assert-Contains $Register "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE" "Production evidence register"
Assert-Contains $Register "Every required evidence item listed in `P3_6_PRODUCTION_EVIDENCE_IMPLEMENTATION.md` must have a corresponding row in this register." "Production evidence register"

$RequiredEvidenceRows = @(
    "P3.6-EV-001 | Deployment | Environment name",
    "P3.6-EV-002 | Deployment | Provider or infrastructure target",
    "P3.6-EV-003 | Deployment | Deployed commit SHA",
    "P3.6-EV-004 | Deployment | Deployment date",
    "P3.6-EV-005 | Deployment | Deployment responsible",
    "P3.6-EV-006 | Deployment | API URL or internal endpoint",
    "P3.6-EV-007 | Deployment | Deployment logs or CI reference",
    "P3.6-EV-008 | Deployment | Rollback reference",
    "P3.6-EV-009 | Configuration | ASPNETCORE_ENVIRONMENT documented",
    "P3.6-EV-010 | Configuration | CORS indexed origins verified",
    "P3.6-EV-011 | Configuration | Forwarded headers indexed known proxies verified",
    "P3.6-EV-012 | Configuration | Forwarded headers indexed known networks verified",
    "P3.6-EV-013 | Configuration | Rate limiting status documented",
    "P3.6-EV-014 | Configuration | Max request body size documented",
    "P3.6-EV-015 | Configuration | Swagger exposure status documented",
    "P3.6-EV-016 | Configuration | Authentication mode documented",
    "P3.6-EV-017 | Configuration | Secrets provider documented without secret values",
    "P3.6-EV-018 | Security | Secrets stored outside repository",
    "P3.6-EV-019 | Security | CodeQL clean",
    "P3.6-EV-020 | Security | Dependency review clean or justified",
    "P3.6-EV-021 | Security | Secret scanning clean",
    "P3.6-EV-022 | Security | Authentication smoke test",
    "P3.6-EV-023 | Security | Authorization smoke test",
    "P3.6-EV-024 | Security | Security headers verification",
    "P3.6-EV-025 | Security | Rate limiting verification",
    "P3.6-EV-026 | Security | Sensitive logs verification",
    "P3.6-EV-027 | Database | SQL Server target documented",
    "P3.6-EV-028 | Database | Database name documented",
    "P3.6-EV-029 | Database | Migration status documented",
    "P3.6-EV-030 | Database | Application user least privilege documented",
    "P3.6-EV-031 | Database | Backup executed",
    "P3.6-EV-032 | Database | Restore tested",
    "P3.6-EV-033 | Database | Recovery time notes documented",
    "P3.6-EV-034 | Database | Data retention notes documented",
    "P3.6-EV-035 | Observability | Health live verified",
    "P3.6-EV-036 | Observability | Health ready verified",
    "P3.6-EV-037 | Observability | Structured logging evidence",
    "P3.6-EV-038 | Observability | Propagated correlation id evidence",
    "P3.6-EV-039 | Observability | 4xx and 5xx traceability evidence",
    "P3.6-EV-040 | Observability | Latency evidence",
    "P3.6-EV-041 | Observability | Startup log evidence",
    "P3.6-EV-042 | Smoke tests | Root endpoint verified",
    "P3.6-EV-043 | Smoke tests | Health live endpoint verified",
    "P3.6-EV-044 | Smoke tests | Health ready endpoint verified",
    "P3.6-EV-045 | Smoke tests | Anonymous request to protected endpoint fails",
    "P3.6-EV-046 | Smoke tests | Authenticated request to protected endpoint succeeds",
    "P3.6-EV-047 | Smoke tests | Representative organization endpoint succeeds",
    "P3.6-EV-048 | Smoke tests | Representative report/export endpoint succeeds when applicable",
    "P3.6-EV-049 | Rollback | Rollback criteria documented",
    "P3.6-EV-050 | Rollback | Rollback command or procedure documented",
    "P3.6-EV-051 | Rollback | Database rollback policy documented",
    "P3.6-EV-052 | Rollback | Restore procedure documented",
    "P3.6-EV-053 | Rollback | Decision owner documented",
    "P3.6-EV-054 | Rollback | Incident record template documented"
)

foreach ($RequiredEvidenceRow in $RequiredEvidenceRows) {
    Assert-Contains $Register $RequiredEvidenceRow "Production evidence register"
}

Assert-Contains $DeploymentRunbook "CodeQL clean" "Production deployment runbook"
Assert-Contains $DeploymentRunbook "SQL Server target available" "Production deployment runbook"
Assert-Contains $DeploymentRunbook "ConnectionStrings__SqlServer" "Production deployment runbook"
Assert-Contains $DeploymentRunbook "Cors__AllowedOrigins__0" "Production deployment runbook"
Assert-Contains $DeploymentRunbook "Cors__AllowedOrigins__1" "Production deployment runbook"
Assert-Contains $DeploymentRunbook "Minimum smoke tests" "Production deployment runbook"

Assert-Contains $RollbackRunbook "Application rollback" "Production rollback runbook"
Assert-Contains $RollbackRunbook "Database rollback" "Production rollback runbook"
Assert-Contains $RollbackRunbook "last known good commit SHA" "Production rollback runbook"

Assert-Contains $DatabaseRunbook "Backup evidence" "Database backup/restore runbook"
Assert-Contains $DatabaseRunbook "Restore evidence" "Database backup/restore runbook"
Assert-Contains $DatabaseRunbook "A backup without a restore test is not sufficient production evidence." "Database backup/restore runbook"

Write-Host "P3.6 production evidence baseline verification passed." -ForegroundColor Green
