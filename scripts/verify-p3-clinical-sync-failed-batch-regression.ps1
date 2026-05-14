$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_CLINICAL_SYNC_FAILED_BATCH_REGRESSION_BASELINE.md"
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
    "P3 Clinical Sync Failed Batch Regression Baseline",
    "failed sync batches cannot be processed",
    "mark the SyncBatch as failed using SyncBatch.Fail",
    "InvalidOperationException",
    "Failed sync batch cannot be processed.",
    "Failed batches are terminal",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 clinical sync failed batch regression baseline"
}

$RequiredTestTokens = @(
    "SyncBatchProcessor_ThrowsWhenFailedBatchIsProcessed",
    "syncBatch.Fail(",
    "new SyncBatchProcessor(dbContext)",
    "Assert.ThrowsAsync<InvalidOperationException>",
    'Assert.Equal("Failed sync batch cannot be processed.", exception.Message)',
    "Assert.Equal(0, await dbContext.Patients.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.PatientVisits.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.ServiceEncounters.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.VitalSignsRecords.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.FormResponses.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.ConsentDocuments.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.MedicalReferrals.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.MedicationDeliveries.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.SyncEvents.CountAsync(cancellationToken))",
    "Assert.Equal(SyncBatchStatus.Failed, failedBatch.Status)",
    "Assert.Equal(0, failedBatch.AcceptedCount)",
    "Assert.Equal(0, failedBatch.RejectedCount)",
    "Assert.Equal(0, failedBatch.ConflictCount)"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 clinical sync failed batch regression test"
}

Write-Host "P3 clinical sync failed batch regression verification passed." -ForegroundColor Green