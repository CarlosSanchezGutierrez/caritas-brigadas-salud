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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_HEALTH_ENDPOINT_DEPLOYMENT_SMOKE_BASELINE.md"
$ObservabilityPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_OBSERVABILITY_BASELINE.md"
$ProgramPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Program.cs"
$DbHealthPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Health/DatabaseConnectivityHealthCheck.cs"
$HealthWriterPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Health/HealthCheckResponseWriter.cs"
$IntegrationTestPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3HealthEndpointIntegrationTests.cs"
$SmokeScriptPath = Join-Path $RepoRoot "scripts/run-p3-deployment-health-smoke.ps1"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 health endpoint baseline"
Assert-FileExists $ObservabilityPath "P3 production observability baseline"
Assert-FileExists $ProgramPath "Program.cs"
Assert-FileExists $DbHealthPath "DatabaseConnectivityHealthCheck"
Assert-FileExists $HealthWriterPath "HealthCheckResponseWriter"
Assert-FileExists $IntegrationTestPath "P3 health endpoint integration tests"
Assert-FileExists $SmokeScriptPath "P3 deployment health smoke script"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Observability = Get-Content $ObservabilityPath -Raw -Encoding UTF8
$Program = Get-Content $ProgramPath -Raw -Encoding UTF8
$DbHealth = Get-Content $DbHealthPath -Raw -Encoding UTF8
$HealthWriter = Get-Content $HealthWriterPath -Raw -Encoding UTF8
$IntegrationTest = Get-Content $IntegrationTestPath -Raw -Encoding UTF8
$SmokeScript = Get-Content $SmokeScriptPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Health Endpoint and Deployment Smoke Baseline",
    "GET /health/live",
    "GET /health/ready",
    "DatabaseConnectivityHealthCheck",
    "HealthCheckResponseWriter.WriteAsync",
    "CARITAS_DEPLOYMENT_SMOKE_BASE_URL",
    "Production go-live remains blocked",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 health endpoint baseline"
}

$RequiredProgramTokens = @(
    "AddCheck(",
    """api-live""",
    "DatabaseConnectivityHealthCheck",
    "MapHealthChecks(",
    """/health/live""",
    """/health/ready""",
    "Predicate = check => check.Tags.Contains(""live"")",
    "Predicate = check => check.Tags.Contains(""ready"")",
    "ResponseWriter = HealthCheckResponseWriter.WriteAsync"
)

foreach ($Token in $RequiredProgramTokens) {
    Assert-Contains $Program $Token "Program.cs health endpoint implementation"
}

$RequiredDbHealthTokens = @(
    "DatabaseConnectivityHealthCheck",
    "CaritasDbContext",
    "CanConnectAsync",
    "Database connectivity check passed.",
    "Database connectivity check failed.",
    "HealthCheckResult.Healthy",
    "HealthCheckResult.Unhealthy"
)

foreach ($Token in $RequiredDbHealthTokens) {
    Assert-Contains $DbHealth $Token "DatabaseConnectivityHealthCheck"
}

$RequiredHealthWriterTokens = @(
    "HealthCheckResponseWriter",
    "context.GetCorrelationId()",
    "status",
    "timestampUtc",
    "correlationId",
    "totalDurationMilliseconds",
    "checks",
    "durationMilliseconds",
    "JsonSerializer.SerializeAsync"
)

foreach ($Token in $RequiredHealthWriterTokens) {
    Assert-Contains $HealthWriter $Token "HealthCheckResponseWriter"
}

$RequiredIntegrationTokens = @(
    "P3HealthEndpointIntegrationTests",
    "LiveHealthEndpoint_ReturnsJsonWithoutAuthentication",
    "ReadyHealthEndpoint_ReturnsDatabaseConnectivitySignalWithoutSensitiveData",
    "/health/live",
    "/health/ready",
    "X-Correlation-Id",
    "api-live",
    "database",
    "Database connectivity check passed.",
    "Assert.DoesNotContain(""PayloadJson"", responseBody, StringComparison.OrdinalIgnoreCase)"
)

foreach ($Token in $RequiredIntegrationTokens) {
    Assert-Contains $IntegrationTest $Token "P3 health endpoint integration tests"
}

$RequiredSmokeTokens = @(
    "CARITAS_DEPLOYMENT_SMOKE_BASE_URL",
    "health/live",
    "health/ready",
    "caritas-brigadas-api",
    "ConnectionStrings",
    "PayloadJson",
    "P3 deployment health smoke test passed."
)

foreach ($Token in $RequiredSmokeTokens) {
    Assert-Contains $SmokeScript $Token "P3 deployment health smoke script"
}

Assert-Contains $Observability "P3-26E health endpoint and deployment smoke implementation" "P3 production observability baseline"
Assert-Contains $Governance "verify-p3-health-endpoint-deployment-smoke.ps1" "repository governance baseline"

Write-Host "P3 health endpoint and deployment smoke verification passed." -ForegroundColor Green