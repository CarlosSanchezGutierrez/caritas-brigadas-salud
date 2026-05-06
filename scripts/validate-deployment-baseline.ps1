$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DockerfilePath = Join-Path $RepoRoot "services\api-dotnet\src\Caritas.Brigadas.Api\Dockerfile"
$DockerIgnorePath = Join-Path $RepoRoot "services\api-dotnet\.dockerignore"
$ComposePath = Join-Path $RepoRoot "docker-compose.local.yml"
$WorkflowPath = Join-Path $RepoRoot ".github\workflows\verify.yml"

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

Assert-FileExists $DockerfilePath
Assert-FileExists $DockerIgnorePath
Assert-FileExists $ComposePath
Assert-FileExists $WorkflowPath

Assert-Contains $DockerfilePath "USER $APP_UID"
Assert-Contains $DockerfilePath "DOTNET_EnableDiagnostics=0"
Assert-Contains $DockerfilePath "HEALTHCHECK"
Assert-Contains $DockerfilePath "/health/live"
Assert-Contains $DockerfilePath "EXPOSE 8080"

Assert-Contains $DockerIgnorePath "appsettings.Local.json"
Assert-Contains $DockerIgnorePath ".env.*"
Assert-Contains $DockerIgnorePath "**/bin/"
Assert-Contains $DockerIgnorePath "**/obj/"

Assert-Contains $ComposePath "mcr.microsoft.com/mssql/server:2022-latest"
Assert-Contains $ComposePath "MSSQL_SA_PASSWORD"
Assert-Contains $ComposePath "condition: service_healthy"
Assert-Contains $ComposePath "/health/live"

Assert-Contains $WorkflowPath "Docker image build gate"
Assert-Contains $WorkflowPath "Deployment baseline metadata gate"
Assert-Contains $WorkflowPath "docker build -f services/api-dotnet/src/Caritas.Brigadas.Api/Dockerfile -t caritas-brigadas-api:ci services/api-dotnet"
Assert-Contains $WorkflowPath "pwsh scripts/validate-deployment-baseline.ps1"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "DEPLOYMENT BASELINE METADATA VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
