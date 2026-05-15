$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_MEDICATION_DELIVERY_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$MedicationHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/MedicationDeliverySyncEventHandler.cs"

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
Assert-FileExists $MedicationHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$MedicationHandler = Get-Content $MedicationHandlerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Medication Delivery Sync Event Handler Extraction Baseline",
    "MedicationDeliverySyncEventHandler must own medication_delivery/create payload parsing",
    "SyncBatchProcessor must not directly construct MedicationDelivery",
    "SyncBatchProcessor must not directly parse CreateMedicationDeliveryRequest",
    "Traceability requirement",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 medication delivery sync event handler extraction baseline"
}

$RequiredProcessorTokens = @(
    "private readonly MedicationDeliverySyncEventHandler _medicationDeliverySyncEventHandler;",
    "_medicationDeliverySyncEventHandler = new MedicationDeliverySyncEventHandler(dbContext, PayloadJsonOptions);",
    "await _medicationDeliverySyncEventHandler.HandleAsync(",
    "await _medicationDeliverySyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor medication delivery handler extraction"
}

$ForbiddenProcessorTokens = @(
    "out CreateMedicationDeliveryRequest? request",
    "new MedicationDelivery(",
    "medicationDelivery.MarkDelivered(",
    "_dbContext.Set<MedicationDelivery>().Add(medicationDelivery)",
    "medication_delivery_operation_not_implemented",
    "medication_delivery_encounter_not_found",
    "medication_delivery_brigade_mismatch",
    "medication_delivery_patient_not_found",
    "medication_delivery_delivered_by_user_not_found",
    "medication_delivery_signature_not_supported_until_document_signature_handler",
    "medication_delivery_id_already_exists",
    "medication_delivery_duplicate_in_pending_batch",
    "request.MarkAsDelivered ? null : request.DeliveredByUserId",
    "request.MarkAsDelivered ? null : request.ReceivedByName"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct medication delivery handler logic: $Token"
    }
}

$RequiredMedicationHandlerTokens = @(
    "internal sealed class MedicationDeliverySyncEventHandler",
    "public async Task HandleAsync",
    "SyncPayloadReader.TryReadObject",
    "out CreateMedicationDeliveryRequest? request",
    "new MedicationDelivery(",
    "medicationDelivery.MarkDelivered(",
    "_dbContext.Set<MedicationDelivery>().Add(medicationDelivery)",
    "syncEvent.Accept(",
    "medicationDelivery.Id",
    "medication_delivery_operation_not_implemented",
    "medication_delivery_encounter_not_found",
    "medication_delivery_brigade_mismatch",
    "medication_delivery_patient_not_found",
    "medication_delivery_delivered_by_user_not_found",
    "medication_delivery_signature_not_supported_until_document_signature_handler",
    "medication_delivery_id_already_exists",
    "medication_delivery_duplicate_in_pending_batch",
    "acceptedMedicationDeliveryIdsInBatch",
    "reserved only after successful MedicationDelivery construction and optional delivered transition",
    "Medication delivery id duplicate checks include globally duplicated ids because primary key uniqueness is not tenant-scoped",
    "Non-delivered medication receipt metadata is preserved through constructor fields instead of silently dropped",
    "request.MarkAsDelivered ? null : request.DeliveredByUserId",
    "request.MarkAsDelivered ? null : request.ReceivedByName"
)

foreach ($Token in $RequiredMedicationHandlerTokens) {
    Assert-Contains $MedicationHandler $Token "MedicationDeliverySyncEventHandler"
}

if ($MedicationHandler -match "delivery\.Id == medicationDeliveryId[\s\S]{0,120}delivery\.OrganizationId == organizationId") {
    throw "MedicationDelivery duplicate id check must not be tenant-scoped because primary key uniqueness is global."
}

Write-Host "P3 medication delivery sync event handler extraction verification passed." -ForegroundColor Green
