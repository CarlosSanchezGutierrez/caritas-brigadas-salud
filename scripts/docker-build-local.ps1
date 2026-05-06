$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$BackendRoot = Join-Path $RepoRoot "services\api-dotnet"
$DockerfilePath = Join-Path $BackendRoot "src\Caritas.Brigadas.Api\Dockerfile"
$ImageName = "caritas-brigadas-api:local"

Set-Location $RepoRoot

if (-not (Get-Command docker -ErrorAction SilentlyContinue)) {
    throw "Docker is not installed or not available in PATH."
}

$StdOut = Join-Path $env:TEMP "docker-info.stdout.log"
$StdErr = Join-Path $env:TEMP "docker-info.stderr.log"

$InfoProcess = Start-Process -FilePath "docker" -ArgumentList @("info") -Wait -PassThru -NoNewWindow -RedirectStandardOutput $StdOut -RedirectStandardError $StdErr

if ($InfoProcess.ExitCode -ne 0) {
    throw "Docker is installed, but the Docker daemon is not running. Start Docker Desktop and try again."
}

Write-Host "=== DOCKER BUILD API ===" -ForegroundColor Cyan
docker build -f $DockerfilePath -t $ImageName $BackendRoot

if ($LASTEXITCODE -ne 0) {
    throw "docker build failed."
}

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "DOCKER BUILD PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "Image: $ImageName" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
