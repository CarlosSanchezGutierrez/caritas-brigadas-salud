param(
    [string]$BaseUrl = $env:CARITAS_DEPLOYMENT_SMOKE_BASE_URL,
    [switch]$Required,
    [int]$TimeoutSeconds = 15
)

$ErrorActionPreference = "Stop"

function Assert-SmokeResponse {
    param(
        [string]$Name,
        [string]$Url,
        [int]$TimeoutSeconds
    )

    Write-Host "Checking $Name at $Url" -ForegroundColor Cyan

    $response = Invoke-WebRequest `
        -Uri $Url `
        -Method Get `
        -TimeoutSec $TimeoutSeconds `
        -UseBasicParsing

    if ($response.StatusCode -ne 200) {
        throw "$Name returned HTTP $($response.StatusCode)."
    }

    $body = [string]$response.Content

    if ([string]::IsNullOrWhiteSpace($body)) {
        throw "$Name returned an empty body."
    }

    $forbiddenTokens = @(
        "ConnectionStrings",
        "Server=",
        "Password",
        "Bearer ",
        "PayloadJson",
        "TrustServerCertificate"
    )

    foreach ($token in $forbiddenTokens) {
        if ($body.Contains($token, [System.StringComparison]::OrdinalIgnoreCase)) {
            throw "$Name response leaked forbidden token: $token"
        }
    }

    return $body
}

if ([string]::IsNullOrWhiteSpace($BaseUrl)) {
    if ($Required) {
        throw "CARITAS_DEPLOYMENT_SMOKE_BASE_URL or -BaseUrl is required for deployment health smoke testing."
    }

    Write-Host "Skipping deployment health smoke because no base URL was provided." -ForegroundColor Yellow
    Write-Host "Set CARITAS_DEPLOYMENT_SMOKE_BASE_URL or pass -BaseUrl to execute it." -ForegroundColor Yellow
    exit 0
}

$BaseUrl = $BaseUrl.TrimEnd("/")

$LiveBody = Assert-SmokeResponse `
    -Name "live health endpoint" `
    -Url "$BaseUrl/health/live" `
    -TimeoutSeconds $TimeoutSeconds

if ($LiveBody -notmatch '"status"\s*:\s*"healthy"') {
    throw "Live health endpoint did not report healthy status."
}

$ReadyBody = Assert-SmokeResponse `
    -Name "ready health endpoint" `
    -Url "$BaseUrl/health/ready" `
    -TimeoutSeconds $TimeoutSeconds

if ($ReadyBody -notmatch '"status"\s*:\s*"healthy"') {
    throw "Ready health endpoint did not report healthy status."
}

$RootBody = Assert-SmokeResponse `
    -Name "root endpoint" `
    -Url "$BaseUrl/" `
    -TimeoutSeconds $TimeoutSeconds

if ($RootBody -notmatch "caritas-brigadas-api") {
    throw "Root endpoint did not return service identity."
}

Write-Host "P3 deployment health smoke test passed." -ForegroundColor Green