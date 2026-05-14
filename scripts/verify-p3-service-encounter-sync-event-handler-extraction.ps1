$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SERVICE_ENCOUNTER_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$ServiceHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/ServiceEncounterSyncEventHandler.cs"

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
Assert-FileExists $ServiceHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$ServiceHandler = Get-Content $ServiceHandlerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Service Encounter Sync Event Handler Extraction Baseline",
    "ServiceEncounterSyncEventHandler must own service_encounter/create payload parsing",
    "SyncBatchProcessor must not directly construct ServiceEncounter",
    "SyncBatchProcessor must not directly parse CreateServiceEncounterRequest",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 service encounter sync event handler extraction baseline"
}

$RequiredProcessorTokens = @(
    "private readonly ServiceEncounterSyncEventHandler _serviceEncounterSyncEventHandler;",
    "_serviceEncounterSyncEventHandler = new ServiceEncounterSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandleServiceEncounterEventAsync",
    "await _serviceEncounterSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor service encounter handler extraction"
}

$ForbiddenProcessorTokens = @(
    "out CreateServiceEncounterRequest? request",
    "var encounter = new ServiceEncounter(",
    "_dbContext.ServiceEncounters.Add(encounter)",
    "service_encounter_operation_not_implemented",
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
    "GenerateSyncEncounterFolio("
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct service encounter handler logic: $Token"
    }
}

$RequiredServiceHandlerTokens = @(
    "internal sealed class ServiceEncounterSyncEventHandler",
    "public async Task HandleAsync",
    "SyncPayloadReader.TryReadObject",
    "out CreateServiceEncounterRequest? request",
    "var encounter = new ServiceEncounter(",
    "_dbContext.ServiceEncounters.Add(encounter)",
    "syncEvent.Accept(",
    "encounter.Id",
    "service_encounter_operation_not_implemented",
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
    "reserved only after successful ServiceEncounter construction and reserved atomically",
    "encounterFolioReserved",
    "encounterVisitServiceKeyReserved",
    "acceptedEncounterFoliosInBatch.Remove(normalizedEncounterFolio)"
)

foreach ($Token in $RequiredServiceHandlerTokens) {
    Assert-Contains $ServiceHandler $Token "ServiceEncounterSyncEventHandler"
}

Write-Host "P3 service encounter sync event handler extraction verification passed." -ForegroundColor Green
