$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$GovernancePath = Join-Path $RepoRoot "docs/backend/P3_CLINICAL_DATA_GOVERNANCE_PRIVACY_ANALYTICS_BASELINE.md"

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

Assert-FileExists $GovernancePath

$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P3 Clinical Data Governance, Privacy, and Analytics Baseline",
    "web data capturers",
    "developers",
    "data analysts",
    "data engineers",
    "data scientists",
    "Non-negotiable principles",
    "Data classification",
    "Direct identifier",
    "Sensitive clinical data",
    "Analytics-ready data",
    "Identified operational data",
    "Web capturer workflow rules",
    "Developer data rules",
    "Analytics, data engineering, and data science rules",
    "Pseudonymized analytics",
    "De-identified analytics",
    "Aggregated analytics",
    "Anonymization, pseudonymization, and de-identification",
    "hashing PatientId alone is not anonymization",
    "Minimum analytics export controls",
    "Recommended data views",
    "Data quality rules",
    "TemperatureCelsius",
    "SystolicBloodPressureMmHg",
    "DiastolicBloodPressureMmHg",
    "HeartRateBpm",
    "Retention and deletion baseline",
    "Security and access baseline",
    "Offline and sync privacy rules",
    "AI and advanced analytics baseline",
    "Evidence and audit requirements",
    "Acceptance criteria",
    "P3-05 can implement VitalSignsRecord"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $Governance $Token "P3 clinical data governance privacy analytics baseline"
}

Write-Host "P3 clinical data governance privacy analytics baseline verification passed." -ForegroundColor Green