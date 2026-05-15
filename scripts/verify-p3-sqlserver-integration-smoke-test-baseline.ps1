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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_SQLSERVER_INTEGRATION_SMOKE_TEST_BASELINE.md"
$SmokeScriptPath = Join-Path $RepoRoot "scripts/run-p3-sqlserver-integration-smoke-test.ps1"
$ProductionReadinessPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"
$DesignTimeFactoryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/DesignTimeCaritasDbContextFactory.cs"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 SQL Server smoke baseline"
Assert-FileExists $SmokeScriptPath "P3 SQL Server smoke script"
Assert-FileExists $ProductionReadinessPath "P3 production deployment readiness baseline"
Assert-FileExists $DesignTimeFactoryPath "DesignTimeCaritasDbContextFactory"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$SmokeScript = Get-Content $SmokeScriptPath -Raw -Encoding UTF8
$ProductionReadiness = Get-Content $ProductionReadinessPath -Raw -Encoding UTF8
$DesignTimeFactory = Get-Content $DesignTimeFactoryPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 SQL Server Integration Smoke Test Baseline",
    "real SQL Server smoke test entry point",
    "CARITAS_SQLSERVER_SMOKE_CONNECTION",
    "CARITAS_SQLSERVER_CONNECTION",
    "dotnet tool restore",
    "dotnet ef migrations list",
    "dotnet ef database update",
    "--project src/Caritas.Brigadas.Infrastructure",
    "--startup-project src/Caritas.Brigadas.Api",
    "--context CaritasDbContext",
    "Production go-live remains blocked",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 SQL Server smoke baseline"
}

$RequiredSmokeScriptTokens = @(
    "CARITAS_SQLSERVER_SMOKE_CONNECTION",
    "CARITAS_SQLSERVER_CONNECTION",
    "Required",
    "SkipDatabaseUpdate",
    "AllowNonSmokeDatabase",
    "Refusing to run SQL Server smoke test against a database without a Smoke/Test/Local/Dev marker.",
    '$ToolManifestCandidates',
    '$ToolManifestPath',
    '(Join-Path $ApiRoot ".config/dotnet-tools.json")',
    '(Join-Path $ApiRoot "dotnet-tools.json")',
    "dotnet tool restore",
    'dotnet tool restore --tool-manifest $ToolManifestPath',
    "dotnet build",
    "dotnet ef --version",
    "dotnet ef migrations list",
    "dotnet ef database update",
    '--project $InfrastructureProject',
    '--startup-project $StartupProject',
    '--context $Context',
    "P3 SQL Server integration smoke test passed."
)

foreach ($Token in $RequiredSmokeScriptTokens) {
    Assert-Contains $SmokeScript $Token "P3 SQL Server smoke script"
}

if ($SmokeScript -match 'Join-Path\s+\$ApiRoot\s+"\.config/dotnet-tools\.json",') {
    throw "P3 SQL Server smoke script still contains invalid comma-based Join-Path invocation."
}

$RestoreIndex = $SmokeScript.IndexOf('dotnet tool restore --tool-manifest $ToolManifestPath', [System.StringComparison]::Ordinal)
$EfVersionIndex = $SmokeScript.IndexOf("dotnet ef --version", [System.StringComparison]::Ordinal)

if ($RestoreIndex -lt 0 -or $EfVersionIndex -lt 0 -or $RestoreIndex -gt $EfVersionIndex) {
    throw "P3 SQL Server smoke script must restore local tools before invoking dotnet ef."
}

$RequiredProductionReadinessTokens = @(
    "P3-26C SQL Server integration smoke test",
    "Production go-live status: blocked."
)

foreach ($Token in $RequiredProductionReadinessTokens) {
    Assert-Contains $ProductionReadiness $Token "P3 production deployment readiness baseline"
}

$RequiredDesignTimeFactoryTokens = @(
    "CARITAS_SQLSERVER_CONNECTION",
    "UseSqlServer",
    "CaritasDbContext"
)

foreach ($Token in $RequiredDesignTimeFactoryTokens) {
    Assert-Contains $DesignTimeFactory $Token "DesignTimeCaritasDbContextFactory"
}

Assert-Contains $Governance "verify-p3-sqlserver-integration-smoke-test-baseline.ps1" "repository governance baseline"

Write-Host "P3 SQL Server integration smoke test baseline verification passed." -ForegroundColor Green