$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_CLINICAL_SYNC_CONFLICT_REGRESSION_BASELINE.md"
$TestPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3ClinicalSyncEndToEndIntegrationTests.cs"

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

Assert-FileExists $DocPath
Assert-FileExists $TestPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Test = Get-Content $TestPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Clinical Sync Conflict Regression Baseline",
    "duplicate patient folio detection inside the same pending batch",
    "PendingEventsProcessed equals 2",
    "AcceptedCount equals 1",
    "RejectedCount equals 0",
    "ConflictCount equals 1",
    "patient_folio_duplicate_in_pending_batch",
    "Controlled conflicts are expected domain outcomes",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 clinical sync conflict regression baseline"
}

$RequiredTestTokens = @(
    "SyncBatchProcessor_CompletesBatchWhenDuplicatePatientFolioCreatesConflict",
    "new SyncBatchProcessor(dbContext)",
    "eventsCount: 2",
    "SyncEntityType.Patient",
    "PAT-CONFLICT-001",
    "Assert.Equal(2, result.PendingEventsProcessed)",
    "Assert.Equal(1, result.AcceptedCount)",
    "Assert.Equal(0, result.RejectedCount)",
    "Assert.Equal(1, result.ConflictCount)",
    "Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken))",
    "SyncEventStatus.Accepted",
    "SyncEventStatus.Conflict",
    "patient_folio_duplicate_in_pending_batch",
    "Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status)",
    "Assert.Equal(1, completedBatch.AcceptedCount)",
    "Assert.Equal(0, completedBatch.RejectedCount)",
    "Assert.Equal(1, completedBatch.ConflictCount)"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 clinical sync conflict regression test"
}

Write-Host "P3 clinical sync conflict regression verification passed." -ForegroundColor Green