$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_CLINICAL_SYNC_ORDERING_REGRESSION_BASELINE.md"
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
    "P3 Clinical Sync Ordering Regression Baseline",
    "SyncProcessingOrder.GetOrder",
    "medication_delivery;",
    "medical_referral;",
    "consent_document;",
    "form_response;",
    "vital_signs;",
    "service_encounter;",
    "patient_visit;",
    "patient.",
    "reverseEventInsertionOrder: true",
    "events.Reverse()",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 clinical sync ordering regression baseline"
}

$RequiredTestTokens = @(
    "SyncBatchProcessor_ProcessesCompleteClinicalOfflineBatchEndToEnd",
    "SyncBatchProcessor_ProcessesOutOfOrderClinicalOfflineBatchUsingSyncProcessingOrder",
    "SeedCompleteClinicalBatchAsync",
    "AssertCompletedClinicalBatchAsync",
    "reverseEventInsertionOrder: false",
    "reverseEventInsertionOrder: true",
    "if (reverseEventInsertionOrder)",
    "events.Reverse();",
    "dbContext.BrigadeServices.Add(new BrigadeService",
    "SyncEntityType.Patient",
    "SyncEntityType.PatientVisit",
    "SyncEntityType.ServiceEncounter",
    "SyncEntityType.VitalSigns",
    "SyncEntityType.FormResponse",
    "SyncEntityType.ConsentDocument",
    "SyncEntityType.MedicalReferral",
    "SyncEntityType.MedicationDelivery",
    "Assert.Equal(8, result.PendingEventsProcessed)",
    "Assert.Equal(8, result.AcceptedCount)",
    "Assert.Equal(0, result.RejectedCount)",
    "Assert.Equal(0, result.ConflictCount)",
    "ClinicalSyncSeed"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 clinical sync ordering regression test"
}

Write-Host "P3 clinical sync ordering regression verification passed." -ForegroundColor Green