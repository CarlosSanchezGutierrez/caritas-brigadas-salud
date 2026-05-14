$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_PATIENT_VISIT_HANDLER_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"

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
Assert-FileExists $ProcessorPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Processor Patient Visit Handler Baseline",
    "EntityType: patient_visit",
    "Operation: create",
    "parse PayloadJson as CreatePatientVisitRequest",
    "validate PatientId belongs to the same OrganizationId",
    "validate PatientId can be found either in persisted Patients or in Patients staged in the same DbContext",
    "process patient create events before patient_visit create events",
    "conflict duplicate VisitFolio values inside the same pending batch",
    "set SyncEvent.EntityId to the created PatientVisit.Id",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor patient visit handler baseline"
}

$RequiredProcessorTokens = @(
    "HandlePatientVisitEventAsync",
    "syncEvent.EntityType == SyncEntityType.PatientVisit",
    "syncEvent.Operation != SyncOperation.Create",
    "patient_visit_operation_not_implemented",
    "JsonSerializer.Deserialize<CreatePatientVisitRequest>",
    "new PatientVisit(",
    "_dbContext.PatientVisits.Add(visit)",
    "syncEvent.Accept(",
    "visit.Id",
    "patient_visit_patient_not_found",
    "patient_visit_brigade_not_found",
    "patient_visit_brigade_mismatch",
    "patient_visit_registered_by_user_not_found",
    "patient_visit_folio_already_exists",
    "patient_visit_folio_duplicate_in_pending_batch",
    "acceptedVisitFoliosInBatch",
    "acceptedVisitFoliosInBatch.Contains(normalizedVisitFolio)",
    "!acceptedVisitFoliosInBatch.Add(normalizedVisitFolio)",
    "GenerateSyncVisitFolio",
    "GetSyncProcessingOrder",
    ".OrderBy(GetSyncProcessingOrder)",
    "pendingEvents = pendingEvents",
    "return 0;",
    "return 1;",
    "return 2;"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor patient visit handler"
}

$ForbiddenProcessorTokens = @(
    "_dbContext.ConsentDocuments.Add",
    "_dbContext.MedicalReferrals.Add",
    "_dbContext.MedicationDeliveries.Add"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor patient visit handler scope violation: $Token"
    }
}

Write-Host "P3 sync processor patient visit handler verification passed." -ForegroundColor Green