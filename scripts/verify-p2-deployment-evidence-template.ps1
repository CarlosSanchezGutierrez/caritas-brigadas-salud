$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$TemplatePath = Join-Path $RepoRoot "docs/backend/P2_DEPLOYMENT_EVIDENCE_TEMPLATE.md"

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

Assert-FileExists $TemplatePath

$Template = Get-Content $TemplatePath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P2 Deployment Evidence Template",
    "Do not store secrets",
    "Deployment identification",
    "Repository validation evidence",
    "Backup evidence",
    "Orphan detection evidence",
    "Migration script evidence",
    "Dry-run evidence",
    "Production deployment evidence",
    "Post-deployment validation",
    "Rollback evidence",
    "Final decision",
    "Final rule",
    "scripts/db-generate-migration-script.ps1",
    "p2_detect_fk_orphans.sql",
    "total_orphans",
    "required_fk_orphans",
    "optional_fk_orphans",
    "Contains ON DELETE CASCADE",
    "Contains ON DELETE SET NULL",
    "Contains strong DeviceId FKs",
    "Runtime user used for migration"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $Template $Token "P2 deployment evidence template"
}

$FenceCount = ([regex]::Matches($Template, "~~~")).Count

if (($FenceCount % 2) -ne 0) {
    throw "P2 deployment evidence template has unbalanced Markdown code fences."
}

Write-Host "P2 deployment evidence template verification passed." -ForegroundColor Green