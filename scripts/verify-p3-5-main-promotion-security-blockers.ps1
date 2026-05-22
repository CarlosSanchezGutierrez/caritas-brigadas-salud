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
Assert-Contains $Program "ReverseProxy:ForwardedHeaders:KnownIPNetworks" "Program.cs"
Assert-Contains $Program "IPAddress.TryParse" "Program.cs"
Assert-Contains $Program "options.KnownIPNetworks.Add" "Program.cs"
Assert-Contains $Program "new System.Net.IPNetwork" "Program.cs"
Assert-Contains $Program "IsValidKnownNetworkPrefixLength(prefix, prefixLength)" "Program.cs"
Assert-Contains $Program "AddressFamily.InterNetwork" "Program.cs"
Assert-Contains $Program "AddressFamily.InterNetworkV6" "Program.cs"
Assert-Contains $Program "prefixLength is >= 0 and <= 32" "Program.cs"
Assert-Contains $Program "prefixLength is >= 0 and <= 128" "Program.cs"

if ($AuthHeaders -match 'if\s*\(\s*AUTH_MODE\s*===\s*["'']oidc["'']\s*\)\s*\{\s*return\s*\{\s*\}\s*;') {
    throw "auth-headers.ts still returns an unconditional empty header object in OIDC mode."
}

Assert-Contains $AuthHeaders "Authorization" "auth-headers.ts"
Assert-Contains $AuthHeaders "Bearer" "auth-headers.ts"
Assert-Contains $AuthHeaders "readOidcAccessTokenFromBrowserStorage" "auth-headers.ts"
Assert-Contains $AuthHeaders "normalizeBearerToken" "auth-headers.ts"
Assert-Contains $AuthHeaders "readBrowserStorageItem" "auth-headers.ts"
Assert-Contains $AuthHeaders "window[storageName]" "auth-headers.ts"
Assert-Contains $AuthHeaders "return {} satisfies Record<string, string>;" "auth-headers.ts"
Assert-NotContains $AuthHeaders "OIDC access token is required" "auth-headers.ts"
Assert-NotContains $AuthHeaders "throw new Error(" "auth-headers.ts"
Assert-NotContains $AuthHeaders "const storageCandidates = [window.sessionStorage, window.localStorage];" "auth-headers.ts"

Write-Host "P3.5 main promotion security blocker verification passed." -ForegroundColor Green