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

$BaselinePath = Join-Path $RepoRoot "docs/product/P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/product/P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md"
$PatientIntakePath = Join-Path $RepoRoot "docs/product/P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md"
$GapAuditPath = Join-Path $RepoRoot "docs/operations/P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 consent signature evidence contract baseline"
Assert-FileExists $ContractPath "P3 consent signature evidence contract"
Assert-FileExists $PatientIntakePath "P3 patient intake functional contract"
Assert-FileExists $GapAuditPath "P3 security and product readiness gap audit"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8
$PatientIntake = Get-Content $PatientIntakePath -Raw -Encoding UTF8
$GapAudit = Get-Content $GapAuditPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Consent and Signature Evidence Contract Baseline",
    "privacy notice presentation",
    "patient or guardian signature",
    "consentDocumentId",
    "privacyNoticeVersion",
    "consentStatus",
    "signatureMethod",
    "signerType",
    "refusalReason",
    "unableToSignReason",
    "voidReason",
    "signatureSha256",
    "consentTextSnapshotHash",
    "ACCEPTED",
    "REFUSED",
    "UNABLE_TO_SIGN",
    "GUARDIAN_ACCEPTED",
    "DRAWN_SIGNATURE",
    "GUARDIAN_SIGNATURE",
    "signature evidence",
    "consent_document_created",
    "Consent and signature data must not be logged as raw request body.",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 consent signature evidence contract baseline"
}

$RequiredContractTokens = @(
    "P3 Consent and Signature Evidence Contract",
    "Frontend readiness impact: BLOCKS_FULL_FRONTEND",
    "Core consent workflow",
    "Required fields",
    "Consent status behavior",
    "Signature method behavior",
    "Storage contract",
    "Offline sync contract",
    "Validation rules",
    "Spanish frontend labels",
    "PARTIAL_CONSENT_FRONTEND_READY",
    "consent_document_created",
    "consent_signature_missing",
    "consent_guardian_relationship_missing",
    "consent_unable_to_sign_reason_missing",
    "refusalReason",
    "unableToSignReason",
    "voidReason",
    "REFUSED without refusalReason",
    "UNABLE_TO_SIGN without unableToSignReason",
    "VOIDED without voidReason",
    "drawnSignature",
    "guardian",
    "pendingSync",
    "Never log",
    "base64 signature",
    "PayloadJson"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3 consent signature evidence contract"
}

Assert-Contains $PatientIntake "P3-30B" "P3 patient intake functional contract"
Assert-Contains $GapAudit "P3-30B consent and signature evidence contract" "P3 security and product readiness gap audit"
Assert-Contains $Governance "verify-p3-consent-signature-evidence-contract.ps1" "repository governance baseline"

Write-Host "P3 consent and signature evidence contract verification passed." -ForegroundColor Green