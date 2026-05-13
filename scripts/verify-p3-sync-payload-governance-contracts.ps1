$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PAYLOAD_GOVERNANCE_PROCESSOR_CONTRACT_BASELINE.md"
$SyncEventPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Domain/Entities/SyncEvent.cs"

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

Assert-FileExists $DocPath
Assert-FileExists $SyncEventPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$SyncEvent = Get-Content $SyncEventPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Payload Governance and Processor Contract Baseline",
    "PayloadJson is sensitive and untrusted",
    "EntityType must come from an explicit allowlist",
    "Operation must come from an explicit allowlist",
    "unknown EntityType must be rejected",
    "unknown Operation must be rejected",
    "raw PayloadJson must not be logged",
    "Allowed EntityType values",
    "Allowed Operation values",
    "Processor contract expectations",
    "Payload validation expectations",
    "Safe diagnostics",
    "Forbidden diagnostics by default",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync payload governance baseline"
}

$RequiredCodeTokens = @(
    "public static class SyncEntityType",
    "public static class SyncOperation",
    "SyncEntityType.IsAllowed",
    "SyncOperation.IsAllowed",
    "patient_visit",
    "service_encounter",
    "vital_signs",
    "form_response",
    "consent_document",
    "document_signature",
    "medical_referral",
    "medication_delivery"
)

foreach ($Token in $RequiredCodeTokens) {
    Assert-Contains $SyncEvent $Token "SyncEvent payload governance"
}

Write-Host "P3 sync payload governance contracts verification passed." -ForegroundColor Green