$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ClinicalRulesPath = Join-Path $RepoRoot "docs/backend/P3_CLINICAL_BUSINESS_RULES_BASELINE.md"

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

Assert-FileExists $ClinicalRulesPath

$ClinicalRules = Get-Content $ClinicalRulesPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P3 Clinical Business Rules Baseline",
    "Clinical domain model baseline",
    "Patient identity rules",
    "Patient recapture and reconfirmation rules",
    "Visit rules",
    "Encounter rules",
    "Vital signs rules",
    "systolic and diastolic blood pressure must be separate fields with mmHg units",
    "Measurement validation rules",
    "SystolicBloodPressureMmHg",
    "DiastolicBloodPressureMmHg",
    "HeartRateBpm",
    "RespiratoryRatePerMinute",
    "TemperatureCelsius",
    "OxygenSaturationPercent",
    "WeightKg",
    "HeightCm",
    "GlucoseMgDl",
    "all persisted measurement fields must use canonical units",
    "Expediente / clinical record rules",
    "Consent and document rules",
    "Forms rules",
    "Referrals and medication delivery rules",
    "Offline and sync clinical rules",
    "Audit rules",
    "Tenant boundary rules",
    "Explicitly out of scope for P3-04",
    "Acceptance criteria",
    "P3-05 can implement VitalSignsRecord"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $ClinicalRules $Token "P3 clinical business rules baseline"
}

Write-Host "P3 clinical business rules baseline verification passed." -ForegroundColor Green
