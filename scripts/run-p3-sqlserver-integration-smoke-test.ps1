param(
    [string]$ConnectionString = $env:CARITAS_SQLSERVER_SMOKE_CONNECTION,
    [switch]$Required,
    [switch]$SkipDatabaseUpdate,
    [switch]$AllowNonSmokeDatabase
)

$ErrorActionPreference = "Stop"

function Assert-ExitCode {
    param(
        [int]$Code,
        [string]$Message
    )

    if ($Code -ne 0) {
        throw $Message
    }
}

function Get-DatabaseNameFromConnectionString {
    param([string]$Value)

    $match = [regex]::Match(
        $Value,
        '(?i)(Database|Initial Catalog)\s*=\s*([^;]+)')

    if (-not $match.Success) {
        return $null
    }

    return $match.Groups[2].Value.Trim()
}

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ApiRoot = Join-Path $RepoRoot "services/api-dotnet"
$InfrastructureProject = "src/Caritas.Brigadas.Infrastructure"
$StartupProject = "src/Caritas.Brigadas.Api"
$Context = "CaritasDbContext"

if ([string]::IsNullOrWhiteSpace($ConnectionString)) {
    if ($Required) {
        throw "CARITAS_SQLSERVER_SMOKE_CONNECTION or -ConnectionString is required for SQL Server smoke testing."
    }

    Write-Host "Skipping SQL Server smoke test because no smoke connection string was provided." -ForegroundColor Yellow
    Write-Host "Set CARITAS_SQLSERVER_SMOKE_CONNECTION or pass -ConnectionString to execute it." -ForegroundColor Yellow
    exit 0
}

$DatabaseName = Get-DatabaseNameFromConnectionString $ConnectionString

if (-not $AllowNonSmokeDatabase) {
    $SafetyTarget = if ([string]::IsNullOrWhiteSpace($DatabaseName)) {
        $ConnectionString
    }
    else {
        $DatabaseName
    }

    if ($SafetyTarget -notmatch '(?i)(Smoke|Test|Local|Dev)') {
        throw "Refusing to run SQL Server smoke test against a database without a Smoke/Test/Local/Dev marker. Pass -AllowNonSmokeDatabase only for a controlled non-production target."
    }
}

if (-not (Test-Path $ApiRoot)) {
    throw "API root not found: $ApiRoot"
}

$env:CARITAS_SQLSERVER_CONNECTION = $ConnectionString

Write-Host "Starting P3 SQL Server integration smoke test." -ForegroundColor Cyan
Write-Host "Repository: $RepoRoot" -ForegroundColor Cyan
Write-Host "API root: $ApiRoot" -ForegroundColor Cyan

if (-not [string]::IsNullOrWhiteSpace($DatabaseName)) {
    Write-Host "Database target: $DatabaseName" -ForegroundColor Cyan
}
else {
    Write-Host "Database target: not parsed from connection string" -ForegroundColor Yellow
}

Push-Location $ApiRoot

try {
    dotnet build "Caritas.Brigadas.sln" -warnaserror
    Assert-ExitCode $LASTEXITCODE "dotnet build failed during SQL Server smoke test."

    dotnet ef --version
    Assert-ExitCode $LASTEXITCODE "dotnet ef is not available. Install dotnet-ef before running SQL Server smoke test."

    dotnet ef migrations list `
        --project $InfrastructureProject `
        --startup-project $StartupProject `
        --context $Context
    Assert-ExitCode $LASTEXITCODE "dotnet ef migrations list failed during SQL Server smoke test."

    if ($SkipDatabaseUpdate) {
        Write-Host "Skipping dotnet ef database update because -SkipDatabaseUpdate was provided." -ForegroundColor Yellow
    }
    else {
        dotnet ef database update `
            --project $InfrastructureProject `
            --startup-project $StartupProject `
            --context $Context
        Assert-ExitCode $LASTEXITCODE "dotnet ef database update failed during SQL Server smoke test."
    }

    Write-Host "P3 SQL Server integration smoke test passed." -ForegroundColor Green
}
finally {
    Pop-Location
}