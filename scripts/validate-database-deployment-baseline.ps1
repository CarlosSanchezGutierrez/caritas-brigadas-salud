$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$GenerateScriptPath = Join-Path $RepoRoot "scripts\db-generate-migration-script.ps1"
$MigrationDocsPath = Join-Path $RepoRoot "docs\database\sql-server-migration-deployment-baseline.md"
$RollbackDocsPath = Join-Path $RepoRoot "docs\database\sql-server-rollback-and-recovery.md"
$PermissionsDocsPath = Join-Path $RepoRoot "docs\database\sql-server-permissions-baseline.md"
$WorkflowPath = Join-Path $RepoRoot ".github\workflows\verify.yml"
$DeploymentDocsPath = Join-Path $RepoRoot "docs\operations\deployment-baseline.md"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
    }
}

function Assert-Contains {
    param(
        [string]$Path,
        [string]$Token
    )

    $Content = Get-Content $Path -Raw

    if (-not $Content.Contains($Token)) {
        throw "$Path does not contain required token: $Token"
    }
}

Assert-FileExists $GenerateScriptPath
Assert-FileExists $MigrationDocsPath
Assert-FileExists $RollbackDocsPath
Assert-FileExists $PermissionsDocsPath
Assert-FileExists $WorkflowPath
Assert-FileExists $DeploymentDocsPath

Assert-Contains $GenerateScriptPath "dotnet ef migrations script --idempotent"
Assert-Contains $GenerateScriptPath "Caritas.Brigadas.Infrastructure.csproj"
Assert-Contains $GenerateScriptPath "Caritas.Brigadas.Api.csproj"

Assert-Contains $MigrationDocsPath "No ejecutar migraciones automáticamente al arrancar la API"
Assert-Contains $MigrationDocsPath "script SQL idempotente"
Assert-Contains $MigrationDocsPath "usuario de migraciones"
Assert-Contains $MigrationDocsPath "usuario runtime"

Assert-Contains $RollbackDocsPath "RPO"
Assert-Contains $RollbackDocsPath "RTO"
Assert-Contains $RollbackDocsPath "restore probado"

Assert-Contains $PermissionsDocsPath "mínimo privilegio"
Assert-Contains $PermissionsDocsPath "db_owner"
Assert-Contains $PermissionsDocsPath "usuario de aplicación"
Assert-Contains $PermissionsDocsPath "usuario de migraciones"

Assert-Contains $WorkflowPath "Database deployment baseline metadata gate"
Assert-Contains $WorkflowPath "pwsh scripts/validate-database-deployment-baseline.ps1"

Assert-Contains $DeploymentDocsPath "Las migraciones no deben ejecutarse automáticamente al iniciar la API"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "DATABASE DEPLOYMENT BASELINE VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
