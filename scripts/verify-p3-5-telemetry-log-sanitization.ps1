$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$TelemetryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Middleware/RequestTelemetryMiddleware.cs"

if (-not (Test-Path $TelemetryPath)) {
    throw "RequestTelemetryMiddleware.cs not found."
}

$Telemetry = Get-Content $TelemetryPath -Raw -Encoding UTF8

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

Assert-Contains $Telemetry "using Caritas.Brigadas.Api.Extensions;" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "using Microsoft.AspNetCore.Mvc.Controllers;" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "context.GetCorrelationId()" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "GetSafeEndpointRouteForLog(context.GetEndpoint())" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "ControllerActionDescriptor" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "ClassifyEndpointTemplateForLog" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "HttpMethods.IsGet(method)" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "/api/v1/[sensitive-resource]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "/api/v1/organizations/[id]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "/api/v1/organizations/[id]/reports/[segment]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "SanitizeForLog(context.GetCorrelationId())" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "builder.Append('_')" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "char.IsLetterOrDigit" "RequestTelemetryMiddleware.cs"

Assert-NotContains $Telemetry "SanitizePath(context.Request.Path)" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "context.TraceIdentifier);" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "NormalizeHttpMethodForLog" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "rawPath.Split" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "return SanitizeForLog(rawPath);" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "if (MaxEndpointSegments <= 0)" "RequestTelemetryMiddleware.cs"

Write-Host "P3.5 telemetry log sanitization verification passed." -ForegroundColor Green