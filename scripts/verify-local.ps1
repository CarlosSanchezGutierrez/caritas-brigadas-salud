# ============================================================
# CÃƒÂ¡ritas Brigadas de Salud
# Local verification gate
# ============================================================

param(
    [switch]$SkipGitClean,
    [switch]$SkipSmoke
)

$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$WebAppRoot = Join-Path $RepoRoot "apps\web-next"
$BackendRoot = Join-Path $RepoRoot "services\api-dotnet"
$SolutionPath = Join-Path $BackendRoot "Caritas.Brigadas.sln"
$ApiProject = Join-Path $BackendRoot "src\Caritas.Brigadas.Api\Caritas.Brigadas.Api.csproj"
$SmokeTestPath = Join-Path $BackendRoot "scripts\smoke-test-local.ps1"
$ConnectionString = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"
$HealthUrl = "http://localhost:5031/api/v1/health"

$ApiProcess = $null

function Assert-ExitCode {
    param([string]$StepName)

    if ($LASTEXITCODE -ne 0) {
        Write-Host "ERROR: Fallo en paso: $StepName" -ForegroundColor Red
        Set-Location $RepoRoot
        git status
        throw "$StepName failed."
    }
}

function Assert-GitClean {
    $GitStatus = git status --porcelain

    if ($GitStatus) {
        Write-Host "ERROR: Working tree no esta limpio." -ForegroundColor Red
        git status
        throw "Working tree is not clean."
    }
}

function Stop-LocalProcesses {
    Get-Process -Name "Caritas.Brigadas.Api" -ErrorAction SilentlyContinue |
        Stop-Process -Force -ErrorAction SilentlyContinue

    Start-Sleep -Seconds 2
}

function Wait-ForApi {
    $MaxAttempts = 45

    for ($Attempt = 1; $Attempt -le $MaxAttempts; $Attempt++) {
        try {
            $Response = Invoke-WebRequest -Uri $HealthUrl -UseBasicParsing -TimeoutSec 5

            if ($Response.StatusCode -eq 200) {
                Write-Host "OK: API respondio health check." -ForegroundColor Green
                return
            }
        }
        catch {
        }

        Start-Sleep -Seconds 2
    }

    throw "La API no respondio health check."
}

function Remove-LocalArtifacts {
    $Paths = @(
        (Join-Path $RepoRoot "report-summary.csv"),
        (Join-Path $RepoRoot "report-summary-verification.csv"),
        (Join-Path $RepoRoot "sync-batch-create.json"),
        (Join-Path $BackendRoot "report-summary.csv"),
        (Join-Path $BackendRoot "sync-batch-create.json"),
        (Join-Path $BackendRoot "api-verification.stdout.log"),
        (Join-Path $BackendRoot "api-verification.stderr.log")
    )

    foreach ($Path in $Paths) {
        Remove-Item $Path -Force -ErrorAction SilentlyContinue
    }
}

try {
    Write-Host "=== VERIFY LOCAL: REPO ===" -ForegroundColor Cyan
    Set-Location $RepoRoot

    if (-not $SkipGitClean) {
        Assert-GitClean
    }

    Remove-LocalArtifacts
    Stop-LocalProcesses

    Write-Host "=== VERIFY LOCAL: ENV ===" -ForegroundColor Cyan
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:DOTNET_ENVIRONMENT = "Development"
    $env:Authentication__Mode = "Development"
    $env:ConnectionStrings__SqlServer = $ConnectionString
    $env:Security__RateLimiting__Enabled = "false"
    $env:Security__RateLimiting__PermitLimit = "1000"
    $env:Security__RateLimiting__WindowMinutes = "1"
    $env:Security__RateLimiting__QueueLimit = "0"

    Write-Host "=== VERIFY LOCAL: SQL LOCALDB + MIGRATIONS ===" -ForegroundColor Cyan
    Set-Location $BackendRoot

    sqllocaldb start MSSQLLocalDB | Out-Null

    dotnet tool restore
    Assert-ExitCode "dotnet tool restore"

    dotnet tool run dotnet-ef database update `
        --context CaritasDbContext `
        --project src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj `
        --startup-project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
        --connection $ConnectionString
    Assert-ExitCode "dotnet ef database update"

    Write-Host "=== VERIFY LOCAL: BACKEND BUILD ===" -ForegroundColor Cyan
    dotnet build $SolutionPath
    Assert-ExitCode "dotnet build"

    Write-Host "=== VERIFY LOCAL: BACKEND TESTS ===" -ForegroundColor Cyan
    dotnet test $SolutionPath
    Assert-ExitCode "dotnet test"

    Write-Host "=== VERIFY LOCAL: FRONTEND INSTALL ===" -ForegroundColor Cyan
    Set-Location $WebAppRoot

    npm ci
    Assert-ExitCode "npm ci"

    Write-Host "=== VERIFY LOCAL: FRONTEND TYPECHECK ===" -ForegroundColor Cyan
    npm run typecheck
    Assert-ExitCode "npm run typecheck"

    Write-Host "=== VERIFY LOCAL: FRONTEND BUILD ===" -ForegroundColor Cyan
    npm run build
    Assert-ExitCode "npm run build"

    Write-Host "=== VERIFY LOCAL: FRONTEND NPM AUDIT MODERATE ===" -ForegroundColor Cyan
    npm audit --audit-level=moderate
    Assert-ExitCode "npm audit --audit-level=moderate"

    if (-not $SkipSmoke) {
        Write-Host "=== VERIFY LOCAL: START API FOR SMOKE TEST ===" -ForegroundColor Cyan
        Set-Location $BackendRoot

        $ApiStdOut = Join-Path $BackendRoot "api-verification.stdout.log"
        $ApiStdErr = Join-Path $BackendRoot "api-verification.stderr.log"

        Remove-Item $ApiStdOut -Force -ErrorAction SilentlyContinue
        Remove-Item $ApiStdErr -Force -ErrorAction SilentlyContinue

        $ApiArgs = @(
            "run",
            "--project",
            $ApiProject,
            "--no-launch-profile",
            "--urls",
            "https://localhost:7044;http://localhost:5031"
        )

        $ApiProcess = Start-Process `
            -FilePath "dotnet" `
            -ArgumentList $ApiArgs `
            -WorkingDirectory $BackendRoot `
            -RedirectStandardOutput $ApiStdOut `
            -RedirectStandardError $ApiStdErr `
            -PassThru

        Wait-ForApi

        Write-Host "=== VERIFY LOCAL: SMOKE TEST ===" -ForegroundColor Cyan
        Set-Location $RepoRoot

        powershell -ExecutionPolicy Bypass -File $SmokeTestPath
        Assert-ExitCode "smoke-test-local.ps1"
    }

    Set-Location $RepoRoot
    Remove-LocalArtifacts

    if (-not $SkipGitClean) {
        Assert-GitClean
    }

    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Green
    Write-Host "VERIFY LOCAL PASO CORRECTAMENTE" -ForegroundColor Green
    Write-Host "Backend build/test: OK" -ForegroundColor Green
    Write-Host "Frontend ci/typecheck/build: OK" -ForegroundColor Green
    Write-Host "npm audit moderate: OK" -ForegroundColor Green
    Write-Host "Smoke test: OK" -ForegroundColor Green
    Write-Host "============================================================" -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Red
    Write-Host "VERIFY LOCAL FALLO" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "============================================================" -ForegroundColor Red

    if ($BackendRoot -and (Test-Path (Join-Path $BackendRoot "api-verification.stdout.log"))) {
        Write-Host ""
        Write-Host "=== ULTIMAS LINEAS STDOUT API ===" -ForegroundColor Yellow
        Get-Content (Join-Path $BackendRoot "api-verification.stdout.log") -Tail 80
    }

    if ($BackendRoot -and (Test-Path (Join-Path $BackendRoot "api-verification.stderr.log"))) {
        Write-Host ""
        Write-Host "=== ULTIMAS LINEAS STDERR API ===" -ForegroundColor Yellow
        Get-Content (Join-Path $BackendRoot "api-verification.stderr.log") -Tail 80
    }

    Set-Location $RepoRoot
    git status

    throw
}
finally {
    if ($ApiProcess -and -not $ApiProcess.HasExited) {
        Stop-Process -Id $ApiProcess.Id -Force -ErrorAction SilentlyContinue
    }

    Stop-LocalProcesses
    Set-Location $RepoRoot
}