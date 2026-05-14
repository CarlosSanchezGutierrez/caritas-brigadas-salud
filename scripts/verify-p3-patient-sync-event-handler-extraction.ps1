$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_PATIENT_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$PatientHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/PatientSyncEventHandler.cs"

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
Assert-FileExists $PatientHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$PatientHandler = Get-Content $PatientHandlerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Patient Sync Event Handler Extraction Baseline",
    "PatientSyncEventHandler must own patient/create payload parsing",
    "SyncBatchProcessor must not directly construct Patient",
    "SyncBatchProcessor must not directly parse CreatePatientRequest",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 patient sync event handler extraction baseline"
}

$RequiredProcessorTokens = @(
    "private readonly PatientSyncEventHandler _patientSyncEventHandler;",
    "_patientSyncEventHandler = new PatientSyncEventHandler(dbContext, PayloadJsonOptions);",
    "private async Task HandlePatientEventAsync",
    "await _patientSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor patient handler extraction"
}

$ForbiddenProcessorTokens = @(
    "var patient = new Patient(",
    "patient.UpdateSensitiveIdentifiers(",
    "patient.UpdateLocation(",
    "patient.MarkAsMigrant();",
    "patient.MarkAsPartialRecord(",
    "patient.UpdateAdminNotes(",
    "out CreatePatientRequest? request"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct patient handler logic: $Token"
    }
}

$RequiredPatientHandlerTokens = @(
    "internal sealed class PatientSyncEventHandler",
    "public async Task HandleAsync",
    "SyncPayloadReader.TryReadObject",
    "out CreatePatientRequest? request",
    "var patient = new Patient(",
    "patient.UpdateSensitiveIdentifiers(",
    "patient.UpdateLocation(",
    "patient.MarkAsMigrant();",
    "patient.MarkAsPartialRecord(",
    "patient.UpdateAdminNotes(",
    "patient_folio_duplicate_in_pending_batch",
    "patient_folio_already_exists",
    "syncEvent.Accept("
)

foreach ($Token in $RequiredPatientHandlerTokens) {
    Assert-Contains $PatientHandler $Token "PatientSyncEventHandler"
}

Write-Host "P3 patient sync event handler extraction verification passed." -ForegroundColor Green
