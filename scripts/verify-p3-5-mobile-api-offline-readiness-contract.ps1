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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_MOBILE_API_OFFLINE_READINESS_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/operations/P3_5_MOBILE_API_OFFLINE_READINESS_CONTRACT.md"

Assert-FileExists $BaselinePath "P3.5 mobile API offline readiness baseline"
Assert-FileExists $ContractPath "P3.5 mobile API offline readiness contract"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
"Mobile and web clients must never connect directly to SQL Server",
"iOS -> HTTPS -> API -> SQL Server",
"Android -> HTTPS -> API -> SQL Server",
"Web Admin -> HTTPS -> API -> SQL Server",
"Mobile-first production goal",
"Stable API contract",
"Offline queue",
"Idempotent sync",
"Retry-safe sync",
"Conflict-aware sync",
"Mobile local storage requirements",
"App Store and Play Store readiness requirements",
"Default state is BLOCKED"
)

foreach ($Token in $RequiredBaselineTokens) {
Assert-Contains $Baseline $Token "P3.5 mobile API offline readiness baseline"
}

$RequiredContractTokens = @(
"Status: BLOCKED",
"Clients must never connect directly to SQL Server",
"Mobile-first goal",
"API readiness evidence",
"Offline sync evidence",
"Conflict handling evidence",
"Retry and idempotency evidence",
"Mobile local storage evidence",
"Mobile release configuration evidence",
"API compatibility evidence",
"Web admin API readiness evidence",
"App Store and Play Store readiness evidence",
"Production mobile/API readiness",
"BLOCKED"
)

foreach ($Token in $RequiredContractTokens) {
Assert-Contains $Contract $Token "P3.5 mobile API offline readiness contract"
}

Write-Host "P3.5 mobile API offline readiness contract verification passed." -ForegroundColor Green