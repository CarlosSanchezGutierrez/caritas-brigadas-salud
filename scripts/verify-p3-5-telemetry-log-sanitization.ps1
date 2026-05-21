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

Assert-Contains $Telemetry "AllowedHttpMethodsForLog" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "GetSafeHttpMethodForLog" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "SanitizePath" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "AllowedPathSegmentsForLog" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "GetSafePathSegmentForLog" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "/api/v1/[sensitive-resource]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "[segment]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "[id]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "MaxEndpointRouteLength" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "MaxEndpointSegments" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "builder.Append('_')" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "char.IsLetterOrDigit" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "context.TraceIdentifier" "RequestTelemetryMiddleware.cs"

Assert-NotContains $Telemetry "SanitizeForLog(GetSafeHttpMethodForLog" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "GetSafeHttpMethodForLog" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "var httpMethod = SanitizeForLog" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "value.Where(static character => !char.IsControl(character))" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "return SanitizeForLog(rawPath);" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "using Caritas.Brigadas.Api.Extensions;" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "context.GetCorrelationId()" "RequestTelemetryMiddleware.cs"

Write-Host "P3.5 telemetry log sanitization verification passed." -ForegroundColor Green