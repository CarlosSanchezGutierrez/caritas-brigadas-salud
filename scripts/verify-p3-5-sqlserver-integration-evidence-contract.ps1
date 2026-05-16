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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_SQLSERVER_INTEGRATION_EVIDENCE_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/operations/P3_5_SQLSERVER_INTEGRATION_EVIDENCE_CONTRACT.md"

Assert-FileExists $BaselinePath "P3.5 SQL Server integration evidence baseline"
Assert-FileExists $ContractPath "P3.5 SQL Server integration evidence contract"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
"SQL Server is not the backend",
"Client -> HTTPS -> API -> SQL Server",
"No connection string, password, token, certificate private key, or secret value may be committed",
"The runtime API login should not own schema migrations by default",
"Migrations must not run automatically at API startup in production",
"SQL Server should not be publicly exposed",
"Encrypt=True",
"Restore test evidence",
"Database connectivity health check",
"Default state is BLOCKED"
)

foreach ($Token in $RequiredBaselineTokens) {
Assert-Contains $Baseline $Token "P3.5 SQL Server integration evidence baseline"
}

$RequiredContractTokens = @(
"Status: BLOCKED",
"SQL Server is the database, not the backend",
"Direct SQL Server access from clients is forbidden",
"No plaintext SQL passwords",
"No secrets in mobile apps",
"No secrets in web frontend bundles",
"The runtime API login must be minimum privilege",
"Production migrations must not run automatically at API startup",
"SQL Server not publicly exposed",
"Secret-backed",
"TrustServerCertificate must be explicitly approved",
"SQL smoke test evidence",
"SQL Server integration state",
"BLOCKED"
)

foreach ($Token in $RequiredContractTokens) {
Assert-Contains $Contract $Token "P3.5 SQL Server integration evidence contract"
}

Write-Host "P3.5 SQL Server integration evidence contract verification passed." -ForegroundColor Green