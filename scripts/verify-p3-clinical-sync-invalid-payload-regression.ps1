$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_CLINICAL_SYNC_INVALID_PAYLOAD_REGRESSION_BASELINE.md"
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
    "P3 Clinical Sync Invalid Payload Regression Baseline",
    "malformed sync payload JSON rejects only the invalid event",
    "PendingEventsProcessed equals 2",
    "AcceptedCount equals 1",
    "RejectedCount equals 1",
    "ConflictCount equals 0",
    "rejectedEvent.ErrorMessage",
    "Sync event payload JSON is invalid.",
    "Malformed payload JSON is a controlled rejected input",
    "completed_with_errors",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 clinical sync invalid payload regression baseline"
}

$RequiredTestTokens = @(
    "SyncBatchProcessor_CompletesBatchWhenInvalidPayloadIsRejected",
    "new SyncBatchProcessor(dbContext)",
    "eventsCount: 2",
    "SyncEntityType.Patient",
    "PAT-REJECTED-001",
    "002-patient-invalid-json",
    "p3-rejected-invalid-json",
    "Assert.Equal(2, result.PendingEventsProcessed)",
    "Assert.Equal(1, result.AcceptedCount)",
    "Assert.Equal(1, result.RejectedCount)",
    "Assert.Equal(0, result.ConflictCount)",
    "Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken))",
    "SyncEventStatus.Accepted",
    "SyncEventStatus.Rejected",
    "rejectedEvent.ErrorMessage",
    "Sync event payload JSON is invalid.",
    "Assert.Equal(SyncBatchStatus.CompletedWithErrors, completedBatch.Status)",
    "Assert.Equal(1, completedBatch.AcceptedCount)",
    "Assert.Equal(1, completedBatch.RejectedCount)",
    "Assert.Equal(0, completedBatch.ConflictCount)"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 clinical sync invalid payload regression test"
}

Write-Host "P3 clinical sync invalid payload regression verification passed." -ForegroundColor Green