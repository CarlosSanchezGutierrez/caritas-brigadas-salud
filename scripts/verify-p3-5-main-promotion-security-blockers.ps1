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

$ProgramPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Program.cs"
$AuthHeadersPath = Join-Path $RepoRoot "apps/web-next/src/lib/auth-headers.ts"

Assert-FileExists $ProgramPath "Program.cs"
Assert-FileExists $AuthHeadersPath "auth-headers.ts"

$Program = Get-Content $ProgramPath -Raw -Encoding UTF8
$AuthHeaders = Get-Content $AuthHeadersPath -Raw -Encoding UTF8

Assert-NotContains $Program "options.KnownIPNetworks.Clear();" "Program.cs"
Assert-NotContains $Program "options.KnownProxies.Clear();" "Program.cs"
Assert-Contains $Program "ReverseProxy:ForwardedHeaders:KnownProxies" "Program.cs"
Assert-Contains $Program "ReverseProxy:ForwardedHeaders:KnownNetworks" "Program.cs"
Assert-Contains $Program "IPAddress.TryParse" "Program.cs"
Assert-Contains $Program "new IPNetwork" "Program.cs"

if ($AuthHeaders -match 'if\s*\(\s*AUTH_MODE\s*===\s*["'']oidc["'']\s*\)\s*\{\s*return\s*\{\s*\}\s*;') {
    throw "auth-headers.ts still returns an empty header object in OIDC mode."
}

Assert-Contains $AuthHeaders "Authorization" "auth-headers.ts"
Assert-Contains $AuthHeaders "Bearer" "auth-headers.ts"
Assert-Contains $AuthHeaders "OIDC access token is required" "auth-headers.ts"
Assert-Contains $AuthHeaders "readOidcAccessTokenFromBrowserStorage" "auth-headers.ts"
Assert-Contains $AuthHeaders "normalizeBearerToken" "auth-headers.ts"

Write-Host "P3.5 main promotion security blocker verification passed." -ForegroundColor Green