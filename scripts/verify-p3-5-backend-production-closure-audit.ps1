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

$AuditPath = Join-Path $RepoRoot "docs/operations/P3_5_BACKEND_PRODUCTION_CLOSURE_AUDIT.md"

Assert-FileExists $AuditPath "P3.5 backend production closure audit"

$Audit = Get-Content $AuditPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "Status: BLOCKED FOR PRODUCTION",
    "Documentation alone is not production evidence",
    "P3.5 contract inventory",
    "P3.5-01",
    "P3.5-02",
    "P3.5-03",
    "P3.5-04",
    "P3.5-05",
    "P3.5-06",
    "P3.5-07",
    "P3.5-08",
    "P3.5-09",
    "P3.5-10",
    "AI Gateway / crypto audit lab",
    "Required evidence before staging",
    "Required evidence before pilot",
    "Required evidence before production",
    "Overengineering control",
    "App Store and Play Store implications",
    "P3.5 documentation closure",
    "Backend production readiness: BLOCKED",
    "P3.6-01 Staging environment evidence",
    "P4 Frontend/Web Admin/iOS/Android implementation"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $Audit $Token "P3.5 backend production closure audit"
}

$RequiredFiles = @(
    "docs/operations/P3_5_PRODUCTION_ENVIRONMENT_CONTRACT.md",
    "docs/operations/P3_5_SQLSERVER_INTEGRATION_EVIDENCE_CONTRACT.md",
    "docs/operations/P3_5_PRODUCTION_SECRETS_AUTH_HARDENING_CONTRACT.md",
    "docs/operations/P3_5_ENCRYPTION_DATA_PROTECTION_CONTRACT.md",
    "docs/operations/P3_5_BACKUP_RESTORE_ROLLBACK_EVIDENCE_CONTRACT.md",
    "docs/operations/P3_5_OBSERVABILITY_INCIDENT_RESPONSE_EVIDENCE_CONTRACT.md",
    "docs/operations/P3_5_SECURITY_TESTING_VULNERABILITY_MANAGEMENT_CONTRACT.md",
    "docs/operations/P3_5_MOBILE_API_OFFLINE_READINESS_CONTRACT.md",
    "docs/operations/P3_5_ADMIN_REPORTING_BACKEND_CONTRACT.md",
    "docs/architecture/ADR_P3_5_10_AI_GATEWAY_CRYPTO_AUDIT_LAB.md"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists (Join-Path $RepoRoot $File) "Required P3.5 source artifact"
}

Write-Host "P3.5 backend production closure audit verification passed." -ForegroundColor Green