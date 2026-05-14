$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ChecklistPath = Join-Path $RepoRoot "docs/backend/P2_MIGRATION_DRY_RUN_CHECKLIST.md"

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

Assert-FileExists $ChecklistPath

$Checklist = Get-Content $ChecklistPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P2 Migration Dry-Run Checklist",
    "Do not run automatic migrations at API startup",
    "Do not apply migrations with the runtime application user",
    "Do not apply P2 FK migrations when orphan counts are greater than zero",
    "backup evidence",
    "Orphan detection",
    "Generate migration script",
    "Dry-run execution",
    "Post-dry-run validation",
    "Production migration readiness",
    "Rollback expectations",
    "Evidence template",
    "Final rule",
    "scripts/db-generate-migration-script.ps1",
    "p2_detect_fk_orphans.sql",
    "ON DELETE CASCADE",
    "ON DELETE SET NULL",
    "DeviceId strong FKs"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $Checklist $Token "P2 migration dry-run checklist"
}

$FenceCount = ([regex]::Matches($Checklist, "~~~")).Count

if (($FenceCount % 2) -ne 0) {
    throw "P2 migration dry-run checklist has unbalanced Markdown code fences."
}

Write-Host "P2 migration dry-run checklist verification passed." -ForegroundColor Green
