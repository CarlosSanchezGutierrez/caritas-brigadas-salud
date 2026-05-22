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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_PRODUCTION_ENVIRONMENT_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/operations/P3_5_PRODUCTION_ENVIRONMENT_CONTRACT.md"

Assert-FileExists $BaselinePath "P3.5 production environment contract baseline"
Assert-FileExists $ContractPath "P3.5 production environment contract"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
"Clients must never connect directly to SQL Server",
"Migrations must not run automatically at API startup",
"Production must not use development-only headers or bypasses",
"No SQL credentials",
"No embedded production secrets",
"CORS allowed origins",
"AllowedHosts",
"Rate limiting",
"SBOM",
"OWASP baseline test plan",
"The AI Gateway must remain disabled until a dedicated ADR exists",
"Blockchain must not be required for production MVP",
"Default state is BLOCKED"
)

foreach ($Token in $RequiredBaselineTokens) {
Assert-Contains $Baseline $Token "P3.5 production environment contract baseline"
}

$RequiredContractTokens = @(
"Status: BLOCKED",
"Direct client-to-database access is forbidden",
"Application user has minimum privileges",
"Connection string is stored as a secret",
"SQL Server is not exposed to mobile or web clients",
"Production must use real token-based authentication",
"Development authentication headers",
"Encrypted local storage for sensitive records",
"Export audit logs",
"Field-level data classification",
"Database connectivity health check",
"CodeQL clean or reviewed",
"Dependency review clean or reviewed",
"Secret scanning clean",
"AI Gateway is deferred",
"Blockchain is deferred",
"READY FOR STAGING",
"READY FOR PILOT",
"READY FOR PRODUCTION"
)

foreach ($Token in $RequiredContractTokens) {
Assert-Contains $Contract $Token "P3.5 production environment contract"
}

Write-Host "P3.5 production environment contract verification passed." -ForegroundColor Green