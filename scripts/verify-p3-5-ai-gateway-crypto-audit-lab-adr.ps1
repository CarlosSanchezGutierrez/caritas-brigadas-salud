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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_AI_GATEWAY_CRYPTO_AUDIT_LAB_ADR_BASELINE.md"
$AdrPath = Join-Path $RepoRoot "docs/architecture/ADR_P3_5_10_AI_GATEWAY_CRYPTO_AUDIT_LAB.md"

Assert-FileExists $BaselinePath "P3.5 AI Gateway crypto audit lab ADR baseline"
Assert-FileExists $AdrPath "P3.5 AI Gateway crypto audit lab ADR"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Adr = Get-Content $AdrPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "AI Gateway and crypto audit work must be disabled by default",
    "The AI Gateway is deferred",
    "No PHI processing",
    "No patient data prompts",
    "No autonomous medical advice",
    "Feature flag",
    "Kill switch",
    "Crypto audit and blockchain work is deferred",
    "No PHI on-chain",
    "No public blockchain dependency",
    "Neither AI Gateway nor blockchain is required for production MVP",
    "Default state is DEFERRED"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3.5 AI Gateway crypto audit lab ADR baseline"
}

$RequiredAdrTokens = @(
    "Status",
    "DEFERRED",
    "AI Gateway and crypto audit lab work are deferred and disabled by default",
    "Neither AI Gateway nor blockchain is required for production MVP",
    "Current AI Gateway state: DISABLED",
    "PHI sent to LLM providers",
    "AI Gateway future approval checklist",
    "Kill switch",
    "Current crypto audit lab state: DISABLED FOR PRODUCTION CLINICAL WORKFLOW",
    "Blockchain is not required for production MVP",
    "Patient PHI on-chain",
    "Crypto audit future approval checklist",
    "No irreversible sensitive-data disclosure",
    "Production MVP dependency",
    "NOT REQUIRED"
)

foreach ($Token in $RequiredAdrTokens) {
    Assert-Contains $Adr $Token "P3.5 AI Gateway crypto audit lab ADR"
}

Write-Host "P3.5 AI Gateway crypto audit lab ADR verification passed." -ForegroundColor Green