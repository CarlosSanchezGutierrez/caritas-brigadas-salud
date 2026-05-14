$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$OrderPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncProcessingOrder.cs"
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

Assert-FileExists $ProcessorPath
Assert-FileExists $OrderPath
Assert-FileExists $VisitHandlerPath

$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$Order = Get-Content $OrderPath -Raw -Encoding UTF8
$VisitHandler = Get-Content $VisitHandlerPath -Raw -Encoding UTF8

$ProcessorAndOrderAndVisitHandler = $Processor + $Order + $VisitHandler

$RequiredTokens = @(
    ".OrderBy(SyncProcessingOrder.GetOrder)",
    "SyncEntityType.PatientVisit",
    "return 1;",
    "HandlePatientVisitEventAsync",
    "PatientVisitSyncEventHandler",
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
    "syncEvent.Accept("
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $ProcessorAndOrderAndVisitHandler $Token "SyncBatchProcessor patient visit handler"
}

$RequiredProcessorTokens = @(
    "private readonly PatientVisitSyncEventHandler _patientVisitSyncEventHandler;",
    "_patientVisitSyncEventHandler = new PatientVisitSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandlePatientVisitEventAsync",
    "await _patientVisitSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor patient visit handler wrapper"
}

$ForbiddenProcessorTokens = @(
    "out CreatePatientVisitRequest? request",
    "var visit = new PatientVisit(",
    "patient_visit_operation_not_implemented",
    "patient_visit_folio_duplicate_in_pending_batch",
    "patient_visit_folio_already_exists",
    "patient_visit_patient_not_found",
    "patient_visit_brigade_not_found",
    "patient_visit_registered_by_user_not_found",
    "GenerateSyncVisitFolio("
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct patient visit logic: $Token"
    }
}

Write-Host "P3 sync processor patient visit handler verification passed." -ForegroundColor Green
