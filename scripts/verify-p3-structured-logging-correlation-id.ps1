$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$TelemetryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Middleware/RequestTelemetryMiddleware.cs"
$CorrelationPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Middleware/CorrelationIdMiddleware.cs"
$ProgramPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Program.cs"

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

Assert-FileExists $TelemetryPath "RequestTelemetryMiddleware"
Assert-FileExists $CorrelationPath "CorrelationIdMiddleware"
Assert-FileExists $ProgramPath "Program.cs"

$Telemetry = Get-Content $TelemetryPath -Raw -Encoding UTF8
$Correlation = Get-Content $CorrelationPath -Raw -Encoding UTF8
$Program = Get-Content $ProgramPath -Raw -Encoding UTF8

Assert-Contains $Program "app.UseMiddleware<CorrelationIdMiddleware>();" "Program.cs"
Assert-Contains $Program "app.UseMiddleware<RequestTelemetryMiddleware>();" "Program.cs"

$correlationIndex = $Program.IndexOf("app.UseMiddleware<CorrelationIdMiddleware>();", [StringComparison]::Ordinal)
$telemetryIndex = $Program.IndexOf("app.UseMiddleware<RequestTelemetryMiddleware>();", [StringComparison]::Ordinal)

if ($correlationIndex -lt 0 -or $telemetryIndex -lt 0 -or $correlationIndex -gt $telemetryIndex) {
    throw "CorrelationIdMiddleware must run before RequestTelemetryMiddleware."
}

Assert-Contains $Correlation "public const string HeaderName = `"X-Correlation-Id`";" "CorrelationIdMiddleware"
Assert-Contains $Correlation "context.Items[HeaderName] = correlationId;" "CorrelationIdMiddleware"
Assert-Contains $Correlation "context.Response.Headers[HeaderName] = correlationId;" "CorrelationIdMiddleware"
Assert-Contains $Correlation "IsValidCorrelationId" "CorrelationIdMiddleware"
Assert-Contains $Correlation "MaxCorrelationIdLength" "CorrelationIdMiddleware"

Assert-Contains $Telemetry "using Caritas.Brigadas.Api.Extensions;" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "using Microsoft.AspNetCore.Mvc.Controllers;" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "context.GetCorrelationId()" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "SanitizeForLog(context.GetCorrelationId())" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "GetSafeEndpointRouteForLog(context.GetEndpoint())" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "ControllerActionDescriptor" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "ClassifyEndpointTemplateForLog" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "SensitiveEndpointTokens" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "/api/v1/[sensitive-resource]" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "CorrelationId" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "RequestId" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "HttpMethod" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "EndpointRoute" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "StatusCode" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "ElapsedMilliseconds" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "_logger.BeginScope(scopeProperties)" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "_logger.LogError" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "_logger.LogWarning" "RequestTelemetryMiddleware"
Assert-Contains $Telemetry "_logger.LogInformation" "RequestTelemetryMiddleware"

Assert-NotContains $Telemetry "SanitizePath(context.Request.Path)" "RequestTelemetryMiddleware"
Assert-NotContains $Telemetry "SensitivePathSegments" "RequestTelemetryMiddleware"
Assert-NotContains $Telemetry "context.TraceIdentifier);" "RequestTelemetryMiddleware"
Assert-NotContains $Telemetry "NormalizeHttpMethodForLog" "RequestTelemetryMiddleware"
Assert-NotContains $Telemetry "rawPath.Split" "RequestTelemetryMiddleware"
Assert-NotContains $Telemetry "return SanitizeForLog(rawPath);" "RequestTelemetryMiddleware"

Write-Host "P3 structured logging and correlation-id verification passed." -ForegroundColor Green