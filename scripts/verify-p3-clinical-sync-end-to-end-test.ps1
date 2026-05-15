$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_CLINICAL_SYNC_END_TO_END_TEST_BASELINE.md"
$TestPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3ClinicalSyncEndToEndIntegrationTests.cs"
$ProjectPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Caritas.Brigadas.Api.Tests.csproj"

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
Assert-FileExists $ProjectPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Test = Get-Content $TestPath -Raw -Encoding UTF8
$Project = Get-Content $ProjectPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Clinical Sync End-to-End Test Baseline",
    "patient;",
    "patient_visit;",
    "service_encounter;",
    "vital_signs;",
    "form_response;",
    "consent_document;",
    "medical_referral;",
    "medication_delivery.",
    "PendingEventsProcessed equals 8",
    "AcceptedCount equals 8",
    "RejectedCount equals 0",
    "ConflictCount equals 0",
    "BrigadeService",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 clinical sync end-to-end test baseline"
}

$RequiredProjectTokens = @(
    'Microsoft.EntityFrameworkCore.InMemory',
    'Caritas.Brigadas.Infrastructure.csproj'
)

foreach ($Token in $RequiredProjectTokens) {
    Assert-Contains $Project $Token "Caritas.Brigadas.Api.Tests.csproj"
}

$RequiredTestTokens = @(
    "P3ClinicalSyncEndToEndIntegrationTests",
    "SyncBatchProcessor_ProcessesCompleteClinicalOfflineBatchEndToEnd",
    "UseInMemoryDatabase",
    "CaritasDbContext",
    "new SyncBatchProcessor(dbContext)",
    "SyncEntityType.Patient",
    "SyncEntityType.PatientVisit",
    "SyncEntityType.ServiceEncounter",
    "SyncEntityType.VitalSigns",
    "SyncEntityType.FormResponse",
    "SyncEntityType.ConsentDocument",
    "SyncEntityType.MedicalReferral",
    "SyncEntityType.MedicationDelivery",
    "CreatePatientRequest",
    "CreatePatientVisitRequest",
    "CreateServiceEncounterRequest",
    "CreateVitalSignsRecordRequest",
    "CreateFormResponseRequest",
    "CreateConsentDocumentRequest",
    "CreateMedicalReferralRequest",
    "CreateMedicationDeliveryRequest",
    "Assert.Equal(8, result.PendingEventsProcessed)",
    "Assert.Equal(8, result.AcceptedCount)",
    "Assert.Equal(0, result.RejectedCount)",
    "Assert.Equal(0, result.ConflictCount)",
    "dbContext.BrigadeServices.Add(new BrigadeService",
    "dbContext.Patients.CountAsync",
    "dbContext.PatientVisits.CountAsync",
    "dbContext.ServiceEncounters.CountAsync",
    "dbContext.VitalSignsRecords.CountAsync",
    "dbContext.FormResponses.CountAsync",
    "dbContext.ConsentDocuments.CountAsync",
    "dbContext.MedicalReferrals.CountAsync",
    "dbContext.MedicationDeliveries.CountAsync",
    "Assert.Equal(SyncEventStatus.Accepted, syncEvent.Status)",
    "Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status)"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 clinical sync end-to-end integration test"
}

Write-Host "P3 clinical sync end-to-end test verification passed." -ForegroundColor Green