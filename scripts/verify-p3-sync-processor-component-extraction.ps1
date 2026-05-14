$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_COMPONENT_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$OrderPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncProcessingOrder.cs"
$ReservationStatePath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/PendingBatchReservationState.cs"

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
Assert-FileExists $OrderPath
Assert-FileExists $ReservationStatePath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$Order = Get-Content $OrderPath -Raw -Encoding UTF8
$ReservationState = Get-Content $ReservationStatePath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Processor Component Extraction Baseline",
    "SyncProcessingOrder",
    "PendingBatchReservationState",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor component extraction baseline"
}

$RequiredProcessorTokens = @(
    ".OrderBy(SyncProcessingOrder.GetOrder)",
    "var reservationState = new PendingBatchReservationState();",
    "await _patientSyncEventHandler.HandleAsync(",
    "await _patientVisitSyncEventHandler.HandleAsync(",
    "await _serviceEncounterSyncEventHandler.HandleAsync(",
    "await _vitalSignsSyncEventHandler.HandleAsync(",
    "await _formResponseSyncEventHandler.HandleAsync(",
    "await _consentDocumentSyncEventHandler.HandleAsync(",
    "await _medicalReferralSyncEventHandler.HandleAsync(",
    "await _medicationDeliverySyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor component extraction"
}

$RequiredOrderTokens = @(
    "internal static class SyncProcessingOrder",
    "public static int GetOrder(SyncEvent syncEvent)",
    "SyncEntityType.Patient",
    "SyncEntityType.PatientVisit",
    "SyncEntityType.ServiceEncounter",
    "SyncEntityType.VitalSigns",
    "SyncEntityType.FormResponse",
    "SyncEntityType.ConsentDocument",
    "SyncEntityType.MedicalReferral",
    "SyncEntityType.MedicationDelivery"
)

foreach ($Token in $RequiredOrderTokens) {
    Assert-Contains $Order $Token "SyncProcessingOrder"
}

$RequiredReservationTokens = @(
    "internal sealed class PendingBatchReservationState",
    "AcceptedPatientFoliosInBatch",
    "AcceptedVisitFoliosInBatch",
    "AcceptedEncounterFoliosInBatch",
    "AcceptedEncounterVisitServiceKeysInBatch",
    "AcceptedVitalSignsIdsInBatch",
    "AcceptedFormResponseIdsInBatch",
    "AcceptedFormResponseEncounterTemplateKeysInBatch",
    "AcceptedConsentDocumentIdsInBatch",
    "AcceptedConsentDocumentKeysInBatch",
    "AcceptedMedicalReferralIdsInBatch",
    "AcceptedMedicalReferralFoliosInBatch",
    "AcceptedMedicationDeliveryIdsInBatch"
)

foreach ($Token in $RequiredReservationTokens) {
    Assert-Contains $ReservationState $Token "PendingBatchReservationState"
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
    "private static int GetSyncProcessingOrder",
    "return SyncProcessingOrder.GetOrder(syncEvent);"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains removed wrapper/component residue: $Token"
    }
}

Write-Host "P3 sync processor component extraction verification passed." -ForegroundColor Green
