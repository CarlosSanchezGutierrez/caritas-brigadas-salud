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

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_5_BACKUP_RESTORE_ROLLBACK_EVIDENCE_CONTRACT_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/operations/P3_5_BACKUP_RESTORE_ROLLBACK_EVIDENCE_CONTRACT.md"

Assert-FileExists $BaselinePath "P3.5 backup restore rollback evidence baseline"
Assert-FileExists $ContractPath "P3.5 backup restore rollback evidence contract"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "A system is not production-ready until recovery has been tested",
    "Documentation without a restore test is not recovery evidence",
    "Backups are encrypted",
    "Restore is tested",
    "RTO is defined",
    "RPO is defined",
    "Migrations must not run automatically at API startup",
    "SQL Server disaster recovery requirements",
    "Sync and offline recovery requirements",
    "Mobile recovery requirements",
    "Evidence package requirements",
    "Default state is BLOCKED"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3.5 backup restore rollback evidence baseline"
}

$RequiredContractTokens = @(
    "Status: BLOCKED",
    "A backup that has not been restored is only an assumption",
    "Production readiness requires restore evidence",
    "Backup evidence",
    "Restore evidence",
    "Recovery Time Objective",
    "Recovery Point Objective",
    "Deployment rollback",
    "Migration rollback",
    "Production migrations must not run automatically at API startup",
    "SQL Server disaster recovery",
    "Offline sync recovery",
    "Mobile recovery",
    "Web admin recovery",
    "Evidence record template",
    "Production recovery readiness",
    "BLOCKED"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3.5 backup restore rollback evidence contract"
}

Write-Host "P3.5 backup restore rollback evidence contract verification passed." -ForegroundColor Green