$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_PATIENT_VISIT_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$VisitHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/PatientVisitSyncEventHandler.cs"

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
Assert-FileExists $VisitHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$VisitHandler = Get-Content $VisitHandlerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Patient Visit Sync Event Handler Extraction Baseline",
    "PatientVisitSyncEventHandler must own patient_visit/create payload parsing",
    "SyncBatchProcessor must not directly construct PatientVisit",
    "SyncBatchProcessor must not directly parse CreatePatientVisitRequest",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 patient visit sync event handler extraction baseline"
}

$RequiredProcessorTokens = @(
    "private readonly PatientVisitSyncEventHandler _patientVisitSyncEventHandler;",
    "_patientVisitSyncEventHandler = new PatientVisitSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandlePatientVisitEventAsync",
    "await _patientVisitSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor patient visit handler extraction"
}

$ForbiddenProcessorTokens = @(
    "out CreatePatientVisitRequest? request",
    "var visit = new PatientVisit(",
    "patient_visit_folio_duplicate_in_pending_batch",
    "patient_visit_folio_already_exists",
    "patient_visit_patient_not_found",
    "patient_visit_brigade_not_found",
    "patient_visit_registered_by_user_not_found",
    "GenerateSyncVisitFolio("
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct patient visit handler logic: $Token"
    }
}

$RequiredVisitHandlerTokens = @(
    "internal sealed class PatientVisitSyncEventHandler",
    "public async Task HandleAsync",
    "SyncPayloadReader.TryReadObject",
    "out CreatePatientVisitRequest? request",
    "var visit = new PatientVisit(",
    "patient_visit_operation_not_implemented",
    "patient_visit_brigade_mismatch",
    "patient_visit_patient_not_found",
    "patient_visit_brigade_not_found",
    "patient_visit_registered_by_user_not_found",
    "patient_visit_id_already_exists",
    "patient_visit_folio_duplicate_in_pending_batch",
    "patient_visit_folio_already_exists",
    "GenerateSyncVisitFolio",
    "syncEvent.Accept("
)

foreach ($Token in $RequiredVisitHandlerTokens) {
    Assert-Contains $VisitHandler $Token "PatientVisitSyncEventHandler"
}

Write-Host "P3 patient visit sync event handler extraction verification passed." -ForegroundColor Green