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

$BaselinePath = Join-Path $RepoRoot "docs/product/P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/product/P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md"
$GapAuditPath = Join-Path $RepoRoot "docs/operations/P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 patient intake functional contract baseline"
Assert-FileExists $ContractPath "P3 patient intake functional contract"
Assert-FileExists $GapAuditPath "P3 security and product readiness gap audit"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8
$GapAudit = Get-Content $GapAuditPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Patient Intake Functional Contract Baseline",
    "Patient intake must allow incomplete information.",
    "patientId",
    "organizationId",
    "localPatientKey",
    "firstName",
    "paternalLastName",
    "maternalLastName",
    "displayName",
    "dateOfBirth",
    "approximateAgeYears",
    "phoneNumber",
    "isIdentityIncomplete",
    "identityIncompleteReason",
    "capturedAtUtc",
    "capturedByUserId",
    "Migrant or incomplete patient data handling",
    "Social security / insurance fields are finalized in P3-30C.",
    "Consent and signature evidence are finalized in P3-30B.",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 patient intake functional contract baseline"
}

$RequiredContractTokens = @(
    "P3 Patient Intake Functional Contract",
    "Frontend readiness impact: BLOCKS_FULL_FRONTEND",
    "Minimum valid patient intake",
    "Incomplete identity behavior",
    "Validation rules",
    "Spanish frontend labels",
    "Offline sync contract",
    "Search/display behavior",
    "Privacy and logging requirements",
    "patient_created",
    "patient_updated",
    "patient_identity_label_missing",
    "patient_identity_incomplete_reason_missing",
    "MIGRANT_OR_TRANSIENT",
    "NO_DOCUMENTS_AVAILABLE",
    "Nombre",
    "Apellido paterno",
    "Datos incompletos",
    "PARTIAL_FRONTEND_READY"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3 patient intake functional contract"
}

Assert-Contains $GapAudit "P3-30A patient intake functional contract" "P3 security and product readiness gap audit"
Assert-Contains $Governance "verify-p3-patient-intake-functional-contract.ps1" "repository governance baseline"

Write-Host "P3 patient intake functional contract verification passed." -ForegroundColor Green