$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_CLINICAL_SYNC_IDEMPOTENCY_REGRESSION_BASELINE.md"
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
    "P3 Clinical Sync Idempotency Regression Baseline",
    "processing an already completed sync batch is idempotent",
    "process the same SyncBatch a second time",
    "PendingEventsProcessed equals 0",
    "AcceptedCount remains 8",
    "Sync batch was already completed.",
    "Already completed batches are immutable",
    "safe no-op",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 clinical sync idempotency regression baseline"
}

$RequiredTestTokens = @(
    "SyncBatchProcessor_ReturnsAlreadyCompletedWithoutDuplicatingClinicalRows",
    "SeedCompleteClinicalBatchAsync",
    "var firstResult = await processor.ProcessAsync",
    "var secondResult = await processor.ProcessAsync",
    "AssertCompletedClinicalBatchAsync",
    "Assert.Equal(0, secondResult.PendingEventsProcessed)",
    "Assert.Equal(8, secondResult.AcceptedCount)",
    "Assert.Equal(0, secondResult.RejectedCount)",
    "Assert.Equal(0, secondResult.ConflictCount)",
    'Assert.Equal("Sync batch was already completed.", secondResult.Message)',
    "Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken))",
    "Assert.Equal(1, await dbContext.PatientVisits.CountAsync(cancellationToken))",
    "Assert.Equal(1, await dbContext.ServiceEncounters.CountAsync(cancellationToken))",
    "Assert.Equal(1, await dbContext.VitalSignsRecords.CountAsync(cancellationToken))",
    "Assert.Equal(1, await dbContext.FormResponses.CountAsync(cancellationToken))",
    "Assert.Equal(1, await dbContext.ConsentDocuments.CountAsync(cancellationToken))",
    "Assert.Equal(1, await dbContext.MedicalReferrals.CountAsync(cancellationToken))",
    "Assert.Equal(1, await dbContext.MedicationDeliveries.CountAsync(cancellationToken))",
    "Assert.Equal(8, syncEvents.Length)",
    "Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status)"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 clinical sync idempotency regression test"
}

Write-Host "P3 clinical sync idempotency regression verification passed." -ForegroundColor Green