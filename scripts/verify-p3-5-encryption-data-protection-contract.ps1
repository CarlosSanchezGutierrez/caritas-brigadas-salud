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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_ENCRYPTION_DATA_PROTECTION_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/operations/P3_5_ENCRYPTION_DATA_PROTECTION_CONTRACT.md"

Assert-FileExists $BaselinePath "P3.5 encryption and data protection baseline"
Assert-FileExists $ContractPath "P3.5 encryption and data protection contract"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "The project must not claim full end-to-end encryption",
    "Full end-to-end encryption is not the default architecture",
    "Encryption in transit",
    "Encryption at rest",
    "Backup encryption",
    "Mobile local storage encryption",
    "Field-level encryption decision",
    "Never log",
    "Exports must define",
    "Key management requirements",
    "Analytics must not bypass privacy",
    "No PHI on-chain",
    "AI Gateway must remain disabled",
    "Default state is BLOCKED"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3.5 encryption and data protection baseline"
}

$RequiredContractTokens = @(
    "Status: BLOCKED",
    "Do not claim full end-to-end encryption",
    "Encryption in transit",
    "Encryption at rest",
    "Mobile local storage encryption",
    "Data classification matrix",
    "Field-level protection decision",
    "Never log",
    "Raw clinical request bodies",
    "Export audit logging",
    "Backup encryption",
    "Key management",
    "De-identification decision",
    "No PHI on-chain",
    "AI Gateway must remain disabled",
    "Production data protection readiness",
    "BLOCKED"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3.5 encryption and data protection contract"
}

Write-Host "P3.5 encryption and data protection contract verification passed." -ForegroundColor Green