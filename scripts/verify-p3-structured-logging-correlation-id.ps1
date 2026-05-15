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

function Assert-NotContains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if ($Content.Contains($Token)) {
        throw "$Label contains forbidden token: $Token"
    }
}

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_STRUCTURED_LOGGING_CORRELATION_ID_BASELINE.md"
$ObservabilityPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_OBSERVABILITY_BASELINE.md"
$CorrelationPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Middleware/CorrelationIdMiddleware.cs"
$TelemetryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Middleware/RequestTelemetryMiddleware.cs"
$HttpContextExtensionsPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Extensions/HttpContextExtensions.cs"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 structured logging baseline"
Assert-FileExists $ObservabilityPath "P3 production observability baseline"
Assert-FileExists $CorrelationPath "CorrelationIdMiddleware"
Assert-FileExists $TelemetryPath "RequestTelemetryMiddleware"
Assert-FileExists $HttpContextExtensionsPath "HttpContextExtensions"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Observability = Get-Content $ObservabilityPath -Raw -Encoding UTF8
$Correlation = Get-Content $CorrelationPath -Raw -Encoding UTF8
$Telemetry = Get-Content $TelemetryPath -Raw -Encoding UTF8
$HttpContextExtensions = Get-Content $HttpContextExtensionsPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Structured Logging and Correlation ID Baseline",
    "X-Correlation-Id",
    "MaxCorrelationIdLength",
    "safe ASCII characters",
    "CorrelationId",
    "RequestId",
    "HttpMethod",
    "EndpointRoute",
    "StatusCode",
    "ElapsedMilliseconds",
    "/api/v1/[sensitive-resource]",
    "raw PayloadJson",
    "Information for successful responses below 400",
    "Warning for responses from 400 to 499",
    "Error for responses 500 or greater",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 structured logging baseline"
}

$RequiredCorrelationTokens = @(
    "public const string HeaderName = ""X-Correlation-Id"";",
    "MaxCorrelationIdLength",
    "IsValidCorrelationId",
    "IsAllowedCorrelationIdCharacter",
    "char.IsAsciiLetterOrDigit",
    "value is '-' or '_' or '.' or ':'",
    "context.Items[HeaderName] = correlationId;",
    "context.Response.Headers[HeaderName] = correlationId;",
    "return context.TraceIdentifier;"
)

foreach ($Token in $RequiredCorrelationTokens) {
    Assert-Contains $Correlation $Token "CorrelationIdMiddleware"
}

$RequiredTelemetryTokens = @(
    "using Caritas.Brigadas.Api.Extensions;",
    "context.GetCorrelationId()",
    "SanitizePath(context.Request.Path)",
    "var scopeProperties = new Dictionary<string, object?>",
    "using var scope = _logger.BeginScope(scopeProperties);",
    "await _next(context);",
    "[""CorrelationId""]",
    "[""RequestId""]",
    "[""HttpMethod""]",
    "[""EndpointRoute""]",
    "[""StatusCode""]",
    "[""ElapsedMilliseconds""]",
    "scopeProperties[""StatusCode""] = statusCode;",
    "scopeProperties[""ElapsedMilliseconds""] = elapsedMilliseconds;",
    "StatusCodes.Status500InternalServerError",
    "StatusCodes.Status400BadRequest",
    "LogInformation",
    "LogWarning",
    "LogError",
    "/api/v1/[sensitive-resource]",
    "sync-batches"
)

foreach ($Token in $RequiredTelemetryTokens) {
    Assert-Contains $Telemetry $Token "RequestTelemetryMiddleware"
}

$ForbiddenTelemetryTokens = @(
    "PayloadJson",
    "Request.Body",
    "Request.QueryString",
    "ConnectionStrings",
    "Bearer ",
    "Password",
    "BeginScope(new Dictionary<string, object?>"
)

foreach ($Token in $ForbiddenTelemetryTokens) {
    Assert-NotContains $Telemetry $Token "RequestTelemetryMiddleware"
}

$BeginScopeIndex = $Telemetry.IndexOf("using var scope = _logger.BeginScope(scopeProperties);", [System.StringComparison]::Ordinal)
$NextIndex = $Telemetry.IndexOf("await _next(context);", [System.StringComparison]::Ordinal)

if ($BeginScopeIndex -lt 0 -or $NextIndex -lt 0 -or $BeginScopeIndex -gt $NextIndex) {
    throw "RequestTelemetryMiddleware must start the logging scope before invoking downstream middleware."
}

Assert-Contains $HttpContextExtensions "GetCorrelationId" "HttpContextExtensions"
Assert-Contains $Observability "P3-26F structured logging and correlation id implementation" "P3 production observability baseline"
Assert-Contains $Governance "verify-p3-structured-logging-correlation-id.ps1" "repository governance baseline"

Write-Host "P3 structured logging and correlation id verification passed." -ForegroundColor Green