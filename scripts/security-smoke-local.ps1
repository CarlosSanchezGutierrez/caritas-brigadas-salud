$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$BackendRoot = Join-Path $RepoRoot "services\api-dotnet"
$ApiProjectPath = @(Get-ChildItem -Path $BackendRoot -Recurse -File -Filter "*.Api.csproj" | Select-Object -First 1).FullName
$ApiBaseUrl = "http://localhost:5031"
$ApiRootUrl = "$ApiBaseUrl/"
$HealthUrl = "$ApiBaseUrl/api/v1/health"
$OrganizationId = "4df92032-4a1c-4cf2-b48f-15b570cd073a"
$UserId = "76279895-817d-47d2-b5c2-2a1e306db4f9"
$ConnectionString = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"
$ApiProcess = $null

function Get-HttpStatus {
    param(
        [string]$Url,
        [hashtable]$Headers
    )

    try {
        if ($Headers) {
            $Response = Invoke-WebRequest -Uri $Url -Headers $Headers -UseBasicParsing -TimeoutSec 15
        }
        else {
            $Response = Invoke-WebRequest -Uri $Url -UseBasicParsing -TimeoutSec 15
        }

        return [int]$Response.StatusCode
    }
    catch {
        if ($_.Exception.Response -and $_.Exception.Response.StatusCode) {
            return [int]$_.Exception.Response.StatusCode
        }

        throw
    }
}

function Assert-Status {
    param(
        [string]$Name,
        [string]$Url,
        [hashtable]$Headers,
        [int]$ExpectedStatus
    )

    $ActualStatus = Get-HttpStatus -Url $Url -Headers $Headers

    if ($ActualStatus -ne $ExpectedStatus) {
        throw "$Name esperaba HTTP $ExpectedStatus pero devolvio HTTP $ActualStatus."
    }

    Write-Host "OK: $Name devolvio HTTP $ActualStatus" -ForegroundColor Green
}

function Wait-ForApi {
    $MaxAttempts = 45

    for ($Attempt = 1; $Attempt -le $MaxAttempts; $Attempt++) {
        try {
            $Status = Get-HttpStatus -Url $HealthUrl -Headers $null

            if ($Status -eq 200) {
                Write-Host "OK: API lista." -ForegroundColor Green
                return
            }
        }
        catch {
        }

        Start-Sleep -Seconds 2
    }

    throw "La API no respondio health check."
}

function Stop-Api {
    if ($ApiProcess -and -not $ApiProcess.HasExited) {
        Stop-Process -Id $ApiProcess.Id -Force -ErrorAction SilentlyContinue
    }

    Get-Process -Name "Caritas.Brigadas.Api" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
    Start-Sleep -Seconds 2
}

$AllPermissions = @(
    "organizations.read",
    "organizations.write",
    "users.read",
    "users.write",
    "roles.read",
    "roles.assign",
    "services.read",
    "services.seed",
    "communities.read",
    "communities.write",
    "mobile-units.read",
    "mobile-units.write",
    "brigades.read",
    "brigades.write",
    "brigade-services.read",
    "brigade-services.write",
    "patients.read",
    "patients.write",
    "patient-visits.read",
    "patient-visits.write",
    "service-encounters.read",
    "service-encounters.write",
    "form-templates.read",
    "form-templates.seed",
    "form-responses.read",
    "form-responses.write",
    "consent-documents.read",
    "consent-documents.write",
    "reports.read",
    "reports.export",
    "sync-batches.read",
    "sync-batches.write",
    "audit-logs.read"
)

$DevHeaders = @{
    "X-Dev-User-Id" = $UserId
    "X-Dev-Organization-Id" = $OrganizationId
    "X-Dev-Roles" = "SUPER_ADMIN"
    "X-Dev-Permissions" = ($AllPermissions -join ",")
    "X-Dev-Name" = "Carlos Sanchez Gutierrez"
    "X-Dev-Email" = "carlos.test@caritas.local"
}

$ForbiddenHeaders = @{
    "X-Dev-User-Id" = $UserId
    "X-Dev-Organization-Id" = $OrganizationId
    "X-Dev-Roles" = "VIEWER"
    "X-Dev-Permissions" = "organizations.read"
    "X-Dev-Name" = "Viewer Test"
    "X-Dev-Email" = "viewer.test@caritas.local"
}

try {
    Set-Location $RepoRoot
    Stop-Api

    Write-Host "=== SECURITY SMOKE: ENV ===" -ForegroundColor Cyan
    $env:ASPNETCORE_ENVIRONMENT = "Development"
    $env:DOTNET_ENVIRONMENT = "Development"
    $env:Authentication__Mode = "Development"
    $env:ConnectionStrings__SqlServer = $ConnectionString
    $env:Security__RateLimiting__Enabled = "true"
    $env:Security__RateLimiting__PermitLimit = "300"
    $env:Security__RateLimiting__WindowMinutes = "1"
    $env:Security__RateLimiting__QueueLimit = "0"

    Write-Host "=== SECURITY SMOKE: START API ===" -ForegroundColor Cyan
    Set-Location $BackendRoot

    $ApiStdOut = Join-Path $BackendRoot "api-security-smoke.stdout.log"
    $ApiStdErr = Join-Path $BackendRoot "api-security-smoke.stderr.log"

    Remove-Item $ApiStdOut -Force -ErrorAction SilentlyContinue
    Remove-Item $ApiStdErr -Force -ErrorAction SilentlyContinue

    $ApiProcess = Start-Process -FilePath "dotnet" -ArgumentList @(
        "run",
        "--project",
        $ApiProjectPath,
        "--no-launch-profile",
        "--urls",
        "https://localhost:7044;http://localhost:5031"
    ) -WorkingDirectory $BackendRoot -RedirectStandardOutput $ApiStdOut -RedirectStandardError $ApiStdErr -PassThru

    Wait-ForApi

    Write-Host "=== SECURITY SMOKE: PUBLIC HEALTH ===" -ForegroundColor Cyan
    Assert-Status -Name "Health publico" -Url "$ApiBaseUrl/api/v1/health" -Headers $null -ExpectedStatus 200

    Write-Host "=== SECURITY SMOKE: SECURITY HEADERS ===" -ForegroundColor Cyan
    $RootResponse = Invoke-WebRequest -Uri $ApiRootUrl -UseBasicParsing -TimeoutSec 15

    $RequiredHeaders = @(
        "X-Content-Type-Options",
        "X-Frame-Options",
        "Referrer-Policy",
        "Permissions-Policy",
        "Content-Security-Policy",
        "Cache-Control"
    )

    foreach ($HeaderName in $RequiredHeaders) {
        if (-not $RootResponse.Headers.ContainsKey($HeaderName)) {
            throw "Falta header de seguridad: $HeaderName"
        }

        Write-Host "OK: Header presente $HeaderName" -ForegroundColor Green
    }

    Write-Host "=== SECURITY SMOKE: AUTHORIZATION ===" -ForegroundColor Cyan
    Assert-Status -Name "Reports sin auth" -Url "$ApiBaseUrl/api/v1/organizations/$OrganizationId/reports/summary" -Headers $null -ExpectedStatus 401
    Assert-Status -Name "Reports sin permiso" -Url "$ApiBaseUrl/api/v1/organizations/$OrganizationId/reports/summary" -Headers $ForbiddenHeaders -ExpectedStatus 403
    Assert-Status -Name "Reports con permisos" -Url "$ApiBaseUrl/api/v1/organizations/$OrganizationId/reports/summary" -Headers $DevHeaders -ExpectedStatus 200

    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Green
    Write-Host "SECURITY SMOKE LOCAL PASO CORRECTAMENTE" -ForegroundColor Green
    Write-Host "Security headers: OK" -ForegroundColor Green
    Write-Host "Auth 401/403/200: OK" -ForegroundColor Green
    Write-Host "Health: OK" -ForegroundColor Green
    Write-Host "============================================================" -ForegroundColor Green
}
catch {
    Write-Host ""
    Write-Host "============================================================" -ForegroundColor Red
    Write-Host "SECURITY SMOKE LOCAL FALLO" -ForegroundColor Red
    Write-Host $_.Exception.Message -ForegroundColor Red
    Write-Host "============================================================" -ForegroundColor Red

    if (Test-Path (Join-Path $BackendRoot "api-security-smoke.stdout.log")) {
        Write-Host ""
        Write-Host "=== ULTIMAS LINEAS STDOUT API ===" -ForegroundColor Yellow
        Get-Content (Join-Path $BackendRoot "api-security-smoke.stdout.log") -Tail 80
    }

    if (Test-Path (Join-Path $BackendRoot "api-security-smoke.stderr.log")) {
        Write-Host ""
        Write-Host "=== ULTIMAS LINEAS STDERR API ===" -ForegroundColor Yellow
        Get-Content (Join-Path $BackendRoot "api-security-smoke.stderr.log") -Tail 80
    }

    throw
}
finally {
    Stop-Api
    Set-Location $RepoRoot
}
