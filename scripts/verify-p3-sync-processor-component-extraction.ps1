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
    "No domain handler is extracted in this package",
    "SyncBatchProcessor must sort pending events using SyncProcessingOrder.GetOrder",
"Compatibility P3 processor tests must read SyncProcessingOrder for topological return tokens",
"handler bodies must continue using their received ISet parameters until handlers are extracted",
    "SyncBatchProcessor must instantiate PendingBatchReservationState once per ProcessAsync call",
    "SyncBatchProcessor must not directly instantiate per-handler HashSet reservation variables",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor component extraction baseline"
}

$RequiredOrderTokens = @(
    "internal static class SyncProcessingOrder",
    "public static int GetOrder(SyncEvent syncEvent)",
    "SyncEntityType.Patient",
    "return 0;",
    "SyncEntityType.PatientVisit",
    "return 1;",
    "SyncEntityType.ServiceEncounter",
    "return 2;",
    "SyncEntityType.VitalSigns",
    "return 3;",
    "SyncEntityType.FormResponse",
    "return 4;",
    "SyncEntityType.ConsentDocument",
    "return 5;",
    "SyncEntityType.MedicalReferral",
    "return 6;",
    "SyncEntityType.MedicationDelivery",
    "return 7;",
    "return 8;"
)

foreach ($Token in $RequiredOrderTokens) {
    Assert-Contains $Order $Token "SyncProcessingOrder"
}

$RequiredReservationStateTokens = @(
    "internal sealed class PendingBatchReservationState",
    "AcceptedPatientFoliosInBatch",
    "AcceptedVisitFoliosInBatch",
    "AcceptedVitalSignsIdsInBatch",
    "AcceptedEncounterFoliosInBatch",
    "AcceptedEncounterVisitServiceKeysInBatch",
    "AcceptedFormResponseIdsInBatch",
    "AcceptedFormResponseEncounterTemplateKeysInBatch",
    "AcceptedConsentDocumentIdsInBatch",
    "AcceptedConsentDocumentKeysInBatch",
    "AcceptedMedicalReferralIdsInBatch",
    "AcceptedMedicalReferralFoliosInBatch",
    "AcceptedMedicationDeliveryIdsInBatch"
)

foreach ($Token in $RequiredReservationStateTokens) {
    Assert-Contains $ReservationState $Token "PendingBatchReservationState"
}

$RequiredProcessorTokens = @(
    "var reservationState = new PendingBatchReservationState();",
    ".OrderBy(SyncProcessingOrder.GetOrder)",
    "return SyncProcessingOrder.GetOrder(syncEvent);",
    "reservationState.AcceptedPatientFoliosInBatch",
    "reservationState.AcceptedVisitFoliosInBatch",
    "reservationState.AcceptedVitalSignsIdsInBatch",
    "reservationState.AcceptedEncounterFoliosInBatch",
    "reservationState.AcceptedEncounterVisitServiceKeysInBatch",
    "reservationState.AcceptedFormResponseIdsInBatch",
    "reservationState.AcceptedFormResponseEncounterTemplateKeysInBatch",
    "reservationState.AcceptedConsentDocumentIdsInBatch",
    "reservationState.AcceptedConsentDocumentKeysInBatch",
    "reservationState.AcceptedMedicalReferralIdsInBatch",
    "reservationState.AcceptedMedicalReferralFoliosInBatch",
    "reservationState.AcceptedMedicationDeliveryIdsInBatch"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor component extraction"
}

$ForbiddenProcessorTokens = @(
    "var acceptedPatientFoliosInBatch = new HashSet",
    "var acceptedVisitFoliosInBatch = new HashSet",
    "var acceptedVitalSignsIdsInBatch = new HashSet",
    "var acceptedEncounterFoliosInBatch = new HashSet",
    "var acceptedFormResponseIdsInBatch = new HashSet",
    "var acceptedConsentDocumentIdsInBatch = new HashSet",
    "var acceptedMedicalReferralIdsInBatch = new HashSet",
    "var acceptedMedicationDeliveryIdsInBatch = new HashSet",
    ".OrderBy(GetSyncProcessingOrder)"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains forbidden component extraction token: $Token"
    }
}

$FirstHandlerIndex = $Processor.IndexOf("private async Task HandlePatientEventAsync", [System.StringComparison]::Ordinal)

if ($FirstHandlerIndex -lt 0) {
    throw "SyncBatchProcessor first handler was not found."
}

$HandlerSection = $Processor.Substring($FirstHandlerIndex)

if ($HandlerSection.Contains("reservationState.")) {
    throw "SyncBatchProcessor handlers must not reference reservationState directly before handler extraction."
}

Write-Host "P3 sync processor component extraction verification passed." -ForegroundColor Green
