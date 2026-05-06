$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$BackendRoot = Join-Path $RepoRoot "services\api-dotnet"
$InfrastructureProject = Join-Path $BackendRoot "src\Caritas.Brigadas.Infrastructure\Caritas.Brigadas.Infrastructure.csproj"
$ApiProject = Join-Path $BackendRoot "src\Caritas.Brigadas.Api\Caritas.Brigadas.Api.csproj"
$OutputRoot = Join-Path $RepoRoot "artifacts\db"
$OutputPath = Join-Path $OutputRoot "caritas-brigadas-idempotent-migrations.sql"

Set-Location $RepoRoot

New-Item -ItemType Directory -Path $OutputRoot -Force | Out-Null

if (-not (Test-Path $InfrastructureProject)) {
    throw "Infrastructure project not found: $InfrastructureProject"
}

if (-not (Test-Path $ApiProject)) {
    throw "API startup project not found: $ApiProject"
}

Write-Host "=== RESTORE DOTNET TOOLS ===" -ForegroundColor Cyan
Set-Location $BackendRoot
dotnet tool restore

if ($LASTEXITCODE -ne 0) {
    throw "dotnet tool restore failed."
}

Write-Host "=== GENERATING IDEMPOTENT SQL SERVER MIGRATION SCRIPT ===" -ForegroundColor Cyan
dotnet ef migrations script --idempotent --project $InfrastructureProject --startup-project $ApiProject --output $OutputPath

if ($LASTEXITCODE -ne 0) {
    throw "dotnet ef migrations script failed."
}

if (-not (Test-Path $OutputPath)) {
    throw "Migration script was not generated: $OutputPath"
}

$ScriptContent = Get-Content $OutputPath -Raw

if ([string]::IsNullOrWhiteSpace($ScriptContent)) {
    throw "Migration script is empty."
}

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "SQL SERVER IDEMPOTENT MIGRATION SCRIPT GENERATED" -ForegroundColor Green
Write-Host $OutputPath -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green

Set-Location $RepoRoot
