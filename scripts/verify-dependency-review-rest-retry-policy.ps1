$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ScriptPath = Join-Path $RepoRoot "scripts/dependency-review-rest.ps1"
$DocPath = Join-Path $RepoRoot "docs/backend/REPOSITORY_SECURITY_DEPENDENCY_REVIEW_RETRY_BASELINE.md"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
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

Assert-FileExists $ScriptPath
Assert-FileExists $DocPath

$Script = Get-Content $ScriptPath -Raw -Encoding UTF8
$Doc = Get-Content $DocPath -Raw -Encoding UTF8

$RequiredScriptTokens = @(
    "function Invoke-DependencyReviewApiWithRetry",
    "DEPENDENCY_REVIEW_MAX_ATTEMPTS",
    "DEPENDENCY_REVIEW_INITIAL_DELAY_SECONDS",
    "Start-Sleep -Seconds $DelaySeconds",
    "GitHub Dependency Review API request failed after $MaxAttempts attempts.",
    "Dependency Review REST API check found blocking vulnerabilities.",
    "BlockingFindings.Count -gt 0",
    "ConvertFrom-Json -ErrorAction Stop",
    "Retrying Dependency Review API"
)

foreach ($Token in $RequiredScriptTokens) {
    Assert-Contains $Script $Token "dependency-review-rest.ps1 retry policy"
}

$ForbiddenScriptTokens = @(
    'throw "GitHub Dependency Review API request failed."'
)

foreach ($Token in $ForbiddenScriptTokens) {
    if ($Script.Contains($Token)) {
        throw "dependency-review-rest.ps1 still contains non-retry failure token: $Token"
    }
}

$RequiredDocTokens = @(
    "Repository Security Dependency Review Retry Baseline",
    "retry transient API failures",
    "use exponential backoff between attempts",
    "fail closed after all retry attempts fail",
    "Retry hardening must never downgrade real vulnerability findings",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "repository security dependency review retry baseline"
}

Write-Host "Dependency Review REST retry policy verification passed." -ForegroundColor Green