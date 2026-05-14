$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_DIRECT_HANDLER_DISPATCH_BASELINE.md"
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
    "P3 Sync Processor Direct Handler Dispatch Baseline",
    "SyncBatchProcessor must dispatch patient events directly to PatientSyncEventHandler.HandleAsync",
    "SyncBatchProcessor must not contain temporary Handle*EventAsync wrappers",
    "SyncBatchProcessor must not contain GetSyncProcessingOrder",
    "P3-22O does not change handler behavior",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor direct handler dispatch baseline"
}

$RequiredProcessorTokens = @(
    "await _patientSyncEventHandler.HandleAsync(",
    "await _patientVisitSyncEventHandler.HandleAsync(",
    "await _serviceEncounterSyncEventHandler.HandleAsync(",
    "await _vitalSignsSyncEventHandler.HandleAsync(",
    "await _formResponseSyncEventHandler.HandleAsync(",
    "await _consentDocumentSyncEventHandler.HandleAsync(",
    "await _medicalReferralSyncEventHandler.HandleAsync(",
    "await _medicationDeliverySyncEventHandler.HandleAsync(",
    ".OrderBy(SyncProcessingOrder.GetOrder)",
    "var reservationState = new PendingBatchReservationState();",
    "await ProcessPendingEventAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor direct handler dispatch"
}

$ForbiddenProcessorTokens = @(
    "HandlePatientEventAsync",
    "HandlePatientVisitEventAsync",
    "HandleServiceEncounterEventAsync",
    "HandleVitalSignsEventAsync",
    "HandleFormResponseEventAsync",
    "HandleConsentDocumentEventAsync",
    "HandleMedicalReferralEventAsync",
    "HandleMedicationDeliveryEventAsync",
    "GetSyncProcessingOrder",
    "return SyncProcessingOrder.GetOrder(syncEvent);",
    "private async Task await _",
    "private static int .OrderBy"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains temporary wrapper token: $Token"
    }
}

$ForbiddenPatterns = @(
    '(?m)[ \t]+$',
    '(\r?\n){4,}',
    '(?m)^private async Task Handle[A-Za-z]+EventAsync',
    '(?m)^\s*}\r?\n    private async Task Handle',
    '(?m)^private static int GetSyncProcessingOrder'
)

foreach ($Pattern in $ForbiddenPatterns) {
    if ($Processor -match $Pattern) {
        throw "SyncBatchProcessor direct dispatch hygiene violation: $Pattern"
    }
}

Write-Host "P3 sync processor direct handler dispatch verification passed." -ForegroundColor Green
