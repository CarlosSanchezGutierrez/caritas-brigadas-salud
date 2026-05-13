$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ReferralPolicyPath = Join-Path $RepoRoot "docs/backend/P3_EXTERNAL_REFERRAL_PASS_TRACEABILITY_BASELINE.md"

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

Assert-FileExists $ReferralPolicyPath

$ReferralPolicy = Get-Content $ReferralPolicyPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P3 External Referral Pass and Scarce Care Traceability Baseline",
    "surgeries",
    "specialty consultations",
    "external medication support",
    "MedicalReferral is the clinical need",
    "ExternalReferralPass is the issued access/justification document",
    "Supported external care types",
    "specialty_consultation",
    "surgery",
    "medication_support",
    "External provider / destination rules",
    "External referral pass rules",
    "Pass status lifecycle",
    "Follow-up traceability rules",
    "Evidence and document rules",
    "Printable pass / format requirements",
    "Analytics and reporting",
    "Clinical record integration",
    "Security and privacy rules",
    "Offline and sync rules",
    "Future implementation options",
    "add ExternalReferralPass",
    "add ReferralFollowUp",
    "add ExternalCareProvider catalog",
    "Acceptance criteria"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $ReferralPolicy $Token "P3 external referral pass traceability baseline"
}

Write-Host "P3 external referral pass traceability baseline verification passed." -ForegroundColor Green