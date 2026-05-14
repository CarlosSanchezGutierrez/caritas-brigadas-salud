$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_SERVICE_ENCOUNTER_HANDLER_BASELINE.md"
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
    "P3 Sync Processor Service Encounter Handler Baseline",
    "EntityType: service_encounter",
    "Operation: create",
    "parse PayloadJson as CreateServiceEncounterRequest",
    "validate service is available for the visit brigade through BrigadeServices",
    "conflict duplicate VisitId plus ServiceId values inside the same pending batch",
    "processor must process service_encounter create events before vital_signs create events",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor service encounter handler baseline"
}

$RequiredProcessorTokens = @(
    "HandleServiceEncounterEventAsync",
    "syncEvent.EntityType == SyncEntityType.ServiceEncounter",
    "syncEvent.Operation != SyncOperation.Create",
    "service_encounter_operation_not_implemented",
    "JsonSerializer.Deserialize<CreateServiceEncounterRequest>",
    "new ServiceEncounter(",
    "_dbContext.ServiceEncounters.Add(encounter)",
    "syncEvent.Accept(",
    "encounter.Id",
    "service_encounter_visit_not_found",
    "service_encounter_brigade_mismatch",
    "service_encounter_service_not_found",
    "service_encounter_service_inactive",
    "service_encounter_service_not_available_for_brigade",
    "service_encounter_provider_user_not_found",
    "service_encounter_folio_already_exists",
    "service_encounter_folio_duplicate_in_pending_batch",
    "service_encounter_duplicate_visit_service",
    "service_encounter_duplicate_visit_service_in_pending_batch",
    "acceptedEncounterFoliosInBatch",
    "acceptedEncounterVisitServiceKeysInBatch",
    "GenerateSyncEncounterFolio",
    "private static string GenerateSyncEncounterFolio",
    "reserved only after successful ServiceEncounter construction",
    "return 2;",
    "return 3;",
    "return 4;"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor service encounter handler"
}

$ForbiddenProcessorTokens = @(
    "_dbContext.ConsentDocuments.Add",
    "_dbContext.MedicalReferrals.Add",
    "_dbContext.MedicationDeliveries.Add"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor service encounter handler scope violation: $Token"
    }
}

Write-Host "P3 sync processor service encounter handler verification passed." -ForegroundColor Green