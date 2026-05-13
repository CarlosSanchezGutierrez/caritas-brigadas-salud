$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$SqlPath = Join-Path $RepoRoot "database/diagnostics/sqlserver/p2_detect_fk_orphans.sql"
$PlaybookPath = Join-Path $RepoRoot "docs/backend/P2_ORPHAN_DETECTION_PLAYBOOK.md"

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

function Assert-NotContains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if ($Content.Contains($Token)) {
        throw "$Label contains forbidden token: $Token"
    }
}

Assert-FileExists $SqlPath
Assert-FileExists $PlaybookPath

$Sql = Get-Content $SqlPath -Raw -Encoding UTF8
$Playbook = Get-Content $PlaybookPath -Raw -Encoding UTF8

$RequiredSqlTokens = @(
    "CREATE TABLE #p2_fk_orphan_results",
    "P2_FK_ORPHAN_SUMMARY",
    "P2_FK_ORPHAN_DETAIL",
    "P2_FK_ORPHAN_ALL_CHECKS",
    "DEFERRED_DEVICE_REFERENCE",
    "Role.OrganizationId -> Organization.Id",
    "UserRole.UserId -> User.Id",
    "PatientVisit.BrigadeId -> Brigade.Id",
    "ServiceEncounter.VisitId -> PatientVisit.Id",
    "VitalSignsRecord.VisitId -> PatientVisit.Id",
    "VitalSignsRecord.MeasuredByUserId -> User.Id",
    "DocumentSignature.DocumentTemplateId -> DocumentTemplate.Id",
    "SyncEvent.SyncBatchId -> SyncBatch.Id"
)

foreach ($Token in $RequiredSqlTokens) {
    Assert-Contains $Sql $Token "P2 orphan detection SQL"
}

$RequiredPlaybookTokens = @(
    "P2 Orphan Detection Playbook",
    "The script is read-only",
    "Deferred references",
    "Blocking rule",
    "total_orphans"
)

foreach ($Token in $RequiredPlaybookTokens) {
    Assert-Contains $Playbook $Token "P2 orphan detection playbook"
}

$ForbiddenSqlTokens = @(
    "DELETE FROM",
    "UPDATE ",
    "ALTER TABLE",
    "DROP TABLE [",
    "TRUNCATE TABLE"
)

foreach ($Token in $ForbiddenSqlTokens) {
    Assert-NotContains $Sql $Token "P2 orphan detection SQL"
}

Write-Host "P2 orphan detection SQL verification passed." -ForegroundColor Green