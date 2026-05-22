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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_PRODUCTION_SECRETS_AUTH_HARDENING_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/operations/P3_5_PRODUCTION_SECRETS_AUTH_HARDENING_CONTRACT.md"

Assert-FileExists $BaselinePath "P3.5 production secrets and auth hardening baseline"
Assert-FileExists $ContractPath "P3.5 production secrets and auth hardening contract"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "Production must not depend on development-only authentication",
    "Development authentication headers",
    "Hardcoded secrets",
    "SQL credentials in mobile apps",
    "Secret provider",
    "Azure Key Vault",
    "AWS Secrets Manager",
    "HashiCorp Vault",
    "Production authentication must use a real token-based provider",
    "Microsoft Entra ID / Azure AD",
    "Auth0",
    "Keycloak",
    "Authorization must remain server-enforced",
    "Production must define how the first administrator is created",
    "Break-glass access must be controlled",
    "iOS and Android must not contain production secrets",
    "Never log",
    "Default state is BLOCKED"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3.5 production secrets and auth hardening baseline"
}

$RequiredContractTokens = @(
    "Status: BLOCKED",
    "Development authentication headers",
    "Static admin tokens",
    "Hardcoded secrets",
    "SQL credentials in clients",
    "Secret provider selected",
    "SQL runtime connection string",
    "OIDC authority",
    "Authentication provider decision",
    "Backend validates tokens",
    "Bootstrap admin process",
    "Break-glass process",
    "App Store release config separation",
    "Play Store release config separation",
    "No secrets in frontend bundle",
    "Never log",
    "Secret scanning enabled",
    "AI Gateway keys must not exist in production",
    "Crypto audit work may be researched only as",
    "Secrets readiness",
    "Auth readiness",
    "BLOCKED"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3.5 production secrets and auth hardening contract"
}

Write-Host "P3.5 production secrets and auth hardening contract verification passed." -ForegroundColor Green