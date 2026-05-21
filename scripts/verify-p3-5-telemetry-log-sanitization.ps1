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
Assert-Contains $Telemetry "context.GetCorrelationId()" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "SanitizePath(context.Request.Path)" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "GetSafeHttpMethodForLog(context.Request.Method)" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "AllowedHttpMethodsForLog" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "AllowedPathSegmentsForLog" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "/api/v1/[sensitive-resource]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "[segment]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "[id]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "MaxEndpointRouteLength" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "MaxEndpointSegments" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "builder.Append('_')" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "char.IsLetterOrDigit" "RequestTelemetryMiddleware.cs"

Assert-NotContains $Telemetry "SanitizeForLog(NormalizeHttpMethodForLog(context.Request.Method))" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "NormalizeHttpMethodForLog" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry 'normalizedMethod is "GET"' "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "var httpMethod = SanitizeForLog" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "value.Where(static character => !char.IsControl(character))" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "return SanitizeForLog(rawPath);" "RequestTelemetryMiddleware.cs"

Write-Host "P3.5 telemetry log sanitization verification passed." -ForegroundColor Green