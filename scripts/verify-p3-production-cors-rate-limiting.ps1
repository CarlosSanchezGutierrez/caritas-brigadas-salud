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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_CORS_RATE_LIMITING_BASELINE.md"
$ObservabilityPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_OBSERVABILITY_BASELINE.md"
$ProgramPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Program.cs"
$ProductionValidationPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Extensions/ProductionConfigurationValidationExtensions.cs"
$ProductionTestsPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Security/ProductionConfigurationValidationTests.cs"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 production CORS and rate limiting baseline"
Assert-FileExists $ObservabilityPath "P3 production observability baseline"
Assert-FileExists $ProgramPath "Program.cs"
Assert-FileExists $ProductionValidationPath "ProductionConfigurationValidationExtensions"
Assert-FileExists $ProductionTestsPath "ProductionConfigurationValidationTests"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Observability = Get-Content $ObservabilityPath -Raw -Encoding UTF8
$Program = Get-Content $ProgramPath -Raw -Encoding UTF8
$ProductionValidation = Get-Content $ProductionValidationPath -Raw -Encoding UTF8
$ProductionTests = Get-Content $ProductionTestsPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Production CORS and Rate Limiting Baseline",
    "Production requires explicit Cors:AllowedOrigins.",
    "Production requires Security:RateLimiting:Enabled to be true.",
    "Security:RateLimiting:PermitLimit greater than zero",
    "Security:RateLimiting:WindowMinutes greater than zero",
    "Security:RateLimiting:QueueLimit zero or greater",
    "Runtime evidence",
    "Production validation evidence",
    "Production go-live remains blocked",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 production CORS and rate limiting baseline"
}

$RequiredProgramTokens = @(
    "const string CorsPolicyName = ""ConfiguredOrigins"";",
    "builder.Services.AddCors",
    "Cors:AllowedOrigins",
    "WithOrigins(allowedOrigins)",
    "app.UseCors(CorsPolicyName);",
    "Security:RateLimiting:Enabled",
    "Security:RateLimiting:PermitLimit",
    "Security:RateLimiting:WindowMinutes",
    "Security:RateLimiting:QueueLimit",
    "builder.Services.AddRateLimiter",
    "options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;",
    "PartitionedRateLimiter.Create<HttpContext, string>",
    "RateLimitPartition.GetFixedWindowLimiter",
    "FixedWindowRateLimiterOptions",
    "app.UseRateLimiter();"
)

foreach ($Token in $RequiredProgramTokens) {
    Assert-Contains $Program $Token "Program.cs CORS/rate limiting runtime"
}

$RequiredProductionValidationTokens = @(
    "ValidateProductionCors(configuration);",
    "ValidateProductionRateLimiting(configuration);",
    "private static void ValidateProductionCors",
    "private static void ValidateProductionRateLimiting",
    "private static bool IsUnsafeCorsOrigin",
    "var allowedOrigins = configuredOrigins.Where(origin => !string.IsNullOrWhiteSpace(origin)).ToArray();",
    "uri.IsLoopback",
    "[::1]",
    "Security:RateLimiting:Enabled",
    "Security:RateLimiting:PermitLimit",
    "Security:RateLimiting:WindowMinutes",
    "Security:RateLimiting:QueueLimit",
    "Production requires at least one explicit Cors:AllowedOrigins entry.",
    "Production CORS origins must be explicit HTTPS origins and cannot use localhost, loopback addresses, or wildcards.",
    "Production requires Security:RateLimiting:Enabled to be true.",
    "Production requires Security:RateLimiting:PermitLimit to be greater than zero.",
    "Production requires Security:RateLimiting:WindowMinutes to be greater than zero.",
    "Production requires Security:RateLimiting:QueueLimit to be zero or greater."
)

foreach ($Token in $RequiredProductionValidationTokens) {
    Assert-Contains $ProductionValidation $Token "ProductionConfigurationValidationExtensions"
}

$RequiredProductionTestTokens = @(
    "ValidateProductionConfiguration_Throws_WhenProductionHasNoCorsOrigins",
    "ValidateProductionConfiguration_Throws_WhenProductionCorsOriginIsUnsafe",
    "ValidateProductionConfiguration_Throws_WhenProductionDisablesRateLimiting",
    "ValidateProductionConfiguration_Throws_WhenProductionRateLimitingValuesAreInvalid",
    "Security:RateLimiting:Enabled",
    "Security:RateLimiting:PermitLimit",
    "Security:RateLimiting:WindowMinutes",
    "Security:RateLimiting:QueueLimit",
    "https://localhost:3000",
    "http://brigadas.caritas.example.org",
    "https://[::1]",
    "*",
    "not-a-uri"
)

foreach ($Token in $RequiredProductionTestTokens) {
    Assert-Contains $ProductionTests $Token "ProductionConfigurationValidationTests"
}

Assert-Contains $Observability "P3-26G production CORS and rate limiting validation" "P3 production observability baseline"
Assert-Contains $Governance "verify-p3-production-cors-rate-limiting.ps1" "repository governance baseline"

Write-Host "P3 production CORS and rate limiting verification passed." -ForegroundColor Green