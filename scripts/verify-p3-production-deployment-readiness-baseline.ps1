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

$ProductionReadinessPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"
$SyncReadinessPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_BACKEND_READINESS_CHECKLIST.md"
$DeploymentBaselinePath = Join-Path $RepoRoot "docs/operations/deployment-baseline.md"
$DatabaseDeploymentValidatorPath = Join-Path $RepoRoot "scripts/validate-database-deployment-baseline.ps1"
$GovernanceValidatorPath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"
$WorkflowPath = Join-Path $RepoRoot ".github/workflows/verify.yml"

Assert-FileExists $ProductionReadinessPath "P3 production deployment readiness baseline"
Assert-FileExists $SyncReadinessPath "P3 sync backend readiness checklist"
Assert-FileExists $DeploymentBaselinePath "deployment baseline"
Assert-FileExists $DatabaseDeploymentValidatorPath "database deployment baseline validator"
Assert-FileExists $GovernanceValidatorPath "repository governance baseline validator"
Assert-FileExists $WorkflowPath "verify workflow"

$ProductionReadiness = Get-Content $ProductionReadinessPath -Raw -Encoding UTF8
$SyncReadiness = Get-Content $SyncReadinessPath -Raw -Encoding UTF8
$DeploymentBaseline = Get-Content $DeploymentBaselinePath -Raw -Encoding UTF8
$DatabaseDeploymentValidator = Get-Content $DatabaseDeploymentValidatorPath -Raw -Encoding UTF8
$GovernanceValidator = Get-Content $GovernanceValidatorPath -Raw -Encoding UTF8
$Workflow = Get-Content $WorkflowPath -Raw -Encoding UTF8

$RequiredProductionReadinessTokens = @(
    "P3 Production Deployment Readiness Baseline",
    "Production go-live status: blocked.",
    "P3-26B authentication and authorization hardening",
    "P3-26C SQL Server integration smoke test",
    "no automatic database migrations during API startup",
    "SQL Server migration scripts generated as idempotent SQL",
    "separate runtime and migration database users",
    "minimum privilege for the runtime user",
    "no local development headers in production authentication flows",
    "no development authentication mode in production",
    "no localhost CORS origins in production",
    "secrets stored outside source control",
    "health endpoint available for deployment verification",
    "deployment evidence captured for every production release",
    "rollback and restore plan available before release",
    "Required deployment evidence",
    "Required environment configuration",
    "Required security posture",
    "Required database deployment posture",
    "Required operational posture",
    "Explicit non-goals",
    "Required follow-up workstreams",
    "Acceptance criteria"
)

foreach ($Token in $RequiredProductionReadinessTokens) {
    Assert-Contains $ProductionReadiness $Token "P3 production deployment readiness baseline"
}

$RequiredSyncReadinessTokens = @(
    "P3 Sync Backend Readiness Checklist",
    "Backend sync readiness status: ready for next backend workstream.",
    "Processor-level coverage closed",
    "API-level coverage closed",
    "Privacy coverage closed",
    "Tenant boundary coverage closed",
    "Governance and CI coverage closed"
)

foreach ($Token in $RequiredSyncReadinessTokens) {
    Assert-Contains $SyncReadiness $Token "P3 sync backend readiness checklist"
}

$RequiredDeploymentBaselineTokens = @(
    "Las migraciones no deben ejecutarse automáticamente al iniciar la API"
)

foreach ($Token in $RequiredDeploymentBaselineTokens) {
    Assert-Contains $DeploymentBaseline $Token "deployment baseline"
}

$RequiredDatabaseValidatorTokens = @(
    "validate-database-deployment-baseline.ps1",
    "db-generate-migration-script.ps1",
    "sql-server-migration-deployment-baseline.md",
    "sql-server-rollback-and-recovery.md",
    "sql-server-permissions-baseline.md"
)

foreach ($Token in $RequiredDatabaseValidatorTokens) {
    Assert-Contains $DatabaseDeploymentValidator $Token "database deployment baseline validator"
}

Assert-Contains $GovernanceValidator "verify-p3-production-deployment-readiness-baseline.ps1" "repository governance baseline validator"
Assert-Contains $Workflow "Database deployment baseline metadata gate" "verify workflow"

Write-Host "P3 production deployment readiness baseline verification passed." -ForegroundColor Green