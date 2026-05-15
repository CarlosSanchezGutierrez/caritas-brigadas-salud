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

$BaselinePath = Join-Path $RepoRoot "docs/product/P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/product/P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT.md"
$PatientIntakePath = Join-Path $RepoRoot "docs/product/P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md"
$ConsentPath = Join-Path $RepoRoot "docs/product/P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md"
$GapAuditPath = Join-Path $RepoRoot "docs/operations/P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 emergency contact and insurance baseline"
Assert-FileExists $ContractPath "P3 emergency contact and insurance contract"
Assert-FileExists $PatientIntakePath "P3 patient intake functional contract"
Assert-FileExists $ConsentPath "P3 consent signature evidence contract"
Assert-FileExists $GapAuditPath "P3 security and product readiness gap audit"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8
$PatientIntake = Get-Content $PatientIntakePath -Raw -Encoding UTF8
$Consent = Get-Content $ConsentPath -Raw -Encoding UTF8
$GapAudit = Get-Content $GapAuditPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Emergency Contact and Insurance Fields Contract Baseline",
    "hasEmergencyContact",
    "emergencyContactFullName",
    "emergencyContactPhoneNumber",
    "emergencyContactRelationship",
    "emergencyContactIsUnavailable",
    "emergencyContactUnavailableReason",
    "hasSocialSecurity",
    "socialSecurityProvider",
    "socialSecurityProviderOther",
    "hasPrivateInsurance",
    "privateInsuranceProvider",
    "insuranceInformationUnavailable",
    "insuranceInformationUnavailableReason",
    "IMSS",
    "ISSSTE",
    "OTHER",
    "emergency_contact_name_missing",
    "insurance_unavailable_reason_missing",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 emergency contact and insurance baseline"
}

$RequiredContractTokens = @(
    "P3 Emergency Contact and Insurance Fields Contract",
    "Frontend readiness impact: BLOCKS_FULL_FRONTEND",
    "Emergency contact fields",
    "Insurance and social security fields",
    "MVP decision",
    "Do not collect national social security numbers or policy numbers",
    "Emergency contact behavior",
    "Insurance/social security behavior",
    "Required enum values",
    "Validation rules",
    "Offline sync contract",
    "Spanish frontend labels",
    "Privacy and logging requirements",
    "Never log",
    "PARTIAL_PATIENT_DETAILS_FRONTEND_READY",
    "hasEmergencyContact",
    "emergencyContactFullName",
    "emergencyContactRelationship",
    "emergencyContactUnavailableReason",
    "hasSocialSecurity",
    "socialSecurityProvider",
    "socialSecurityProviderOther",
    "hasPrivateInsurance",
    "insuranceInformationUnavailableReason",
    "emergency_contact_name_missing",
    "social_security_provider_other_missing",
    "insurance_unavailable_reason_missing",
    "PayloadJson"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3 emergency contact and insurance contract"
}

Assert-Contains $PatientIntake "P3-30C" "P3 patient intake functional contract"
Assert-Contains $Consent "P3-30C" "P3 consent signature evidence contract"
Assert-Contains $GapAudit "P3-30C emergency contact and insurance fields" "P3 security and product readiness gap audit"
Assert-Contains $Governance "verify-p3-emergency-contact-insurance-fields-contract.ps1" "repository governance baseline"

Write-Host "P3 emergency contact and insurance fields contract verification passed." -ForegroundColor Green