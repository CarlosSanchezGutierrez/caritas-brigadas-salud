$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

$ProgramPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Program.cs"
$AuthHeadersPath = Join-Path $RepoRoot "apps/web-next/src/lib/auth-headers.ts"
$TelemetryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Middleware/RequestTelemetryMiddleware.cs"

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

Assert-FileExists $ProgramPath "Program.cs"
Assert-FileExists $AuthHeadersPath "auth-headers.ts"
Assert-FileExists $TelemetryPath "RequestTelemetryMiddleware.cs"

$Program = Get-Content $ProgramPath -Raw -Encoding UTF8
$AuthHeaders = Get-Content $AuthHeadersPath -Raw -Encoding UTF8
$Telemetry = Get-Content $TelemetryPath -Raw -Encoding UTF8

Assert-NotContains $Program "options.KnownIPNetworks.Clear();" "Program.cs"
Assert-NotContains $Program "options.KnownProxies.Clear();" "Program.cs"
Assert-NotContains $Program "Microsoft.AspNetCore.HttpOverrides.IPNetwork" "Program.cs"
Assert-NotContains $Program "options.KnownNetworks" "Program.cs"
Assert-Contains $Program "ReverseProxy:ForwardedHeaders:KnownProxies" "Program.cs"
Assert-Contains $Program "ReverseProxy:ForwardedHeaders:KnownIPNetworks" "Program.cs"
Assert-Contains $Program "prefixLength is >= 0 and <= 128" "Program.cs"
Assert-Contains $Program "prefixLength is >= 0 and <= 32" "Program.cs"
Assert-Contains $Program "AddressFamily.InterNetworkV6" "Program.cs"
Assert-Contains $Program "AddressFamily.InterNetwork" "Program.cs"
Assert-Contains $Program "IsValidKnownNetworkPrefixLength(prefix, prefixLength)" "Program.cs"
Assert-Contains $Program "new System.Net.IPNetwork" "Program.cs"
Assert-Contains $Program "new System.Net.IPNetwork(prefix, prefixLength)" "Program.cs"

if ($AuthHeaders -match 'if\s*\(\s*AUTH_MODE\s*===\s*["'']oidc["'']\s*\)\s*\{\s*return\s*\{\s*\}\s*;') {
    throw "auth-headers.ts still returns unconditional empty headers in OIDC mode."
}

Assert-Contains $AuthHeaders "Authorization" "auth-headers.ts"
Assert-Contains $AuthHeaders "Bearer" "auth-headers.ts"
Assert-Contains $AuthHeaders "readBrowserStorageItem" "auth-headers.ts"
Assert-Contains $AuthHeaders "return {} satisfies Record<string, string>;" "auth-headers.ts"
Assert-NotContains $AuthHeaders "OIDC access token is required" "auth-headers.ts"
Assert-NotContains $AuthHeaders "throw new Error(" "auth-headers.ts"
Assert-NotContains $AuthHeaders "const storageCandidates = [window.sessionStorage, window.localStorage];" "auth-headers.ts"

Assert-Contains $Telemetry "using Caritas.Brigadas.Api.Extensions;" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "using Microsoft.AspNetCore.Mvc.Controllers;" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "context.GetCorrelationId()" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "GetSafeEndpointRouteForLog(context.GetEndpoint())" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "ControllerActionDescriptor" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "ClassifyEndpointTemplateForLog" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "HttpMethods.IsGet(method)" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "/api/v1/[sensitive-resource]" "RequestTelemetryMiddleware.cs"
Assert-Contains $Telemetry "SanitizeForLog(context.GetCorrelationId())" "RequestTelemetryMiddleware.cs"

Assert-NotContains $Telemetry "SanitizePath(context.Request.Path)" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "context.TraceIdentifier);" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "NormalizeHttpMethodForLog" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "rawPath.Split" "RequestTelemetryMiddleware.cs"
Assert-NotContains $Telemetry "if (MaxEndpointSegments <= 0)" "RequestTelemetryMiddleware.cs"

Write-Host "P3 pre-main security review findings verification passed." -ForegroundColor Green