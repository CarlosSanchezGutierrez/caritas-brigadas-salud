$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_MEDICATION_DELIVERY_HANDLER_BASELINE.md"
$RequestPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/MedicationDeliveries/CreateMedicationDeliveryRequest.cs"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$OrderPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncProcessingOrder.cs"

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
Assert-FileExists $RequestPath
Assert-FileExists $ProcessorPath
Assert-FileExists $OrderPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Request = Get-Content $RequestPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$Order = Get-Content $OrderPath -Raw -Encoding UTF8
$ProcessorAndOrder = $Processor + $Order

$RequiredDocTokens = @(
    "P3 Sync Processor Medication Delivery Handler Baseline",
    "EntityType: medication_delivery",
    "Operation: create",
    "parse PayloadJson as CreateMedicationDeliveryRequest",
    "derive PatientId from ServiceEncounter.PatientId, not from payload trust",
    "reject SignatureId until the document_signature handler exists",
    "support optional delivered transition only when MarkAsDelivered is true and DeliveredByUserId is provided",
    "reserve pending-batch medication delivery id only after successful MedicationDelivery construction and optional delivered transition",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor medication delivery handler baseline"
}

$RequiredRequestTokens = @(
    "CreateMedicationDeliveryRequest",
    "EncounterId",
    "MedicationName",
    "Presentation",
    "Quantity",
    "LotNumber",
    "ExpirationDate",
    "Instructions",
    "DeliveredByUserId",
    "ReceivedByName",
    "SignatureId",
    "MarkAsDelivered",
    "CreatedOffline",
    "DeviceId"
)

foreach ($Token in $RequiredRequestTokens) {
    Assert-Contains $Request $Token "CreateMedicationDeliveryRequest"
}

$RequiredProcessorTokens = @(
    "HandleMedicationDeliveryEventAsync",
    "syncEvent.EntityType == SyncEntityType.MedicationDelivery",
    "syncEvent.Operation != SyncOperation.Create",
    "medication_delivery_operation_not_implemented",
    "out CreateMedicationDeliveryRequest? request",
    "new MedicationDelivery(",
    "medicationDelivery.MarkDelivered(",
    "_dbContext.Set<MedicationDelivery>().Add(medicationDelivery)",
    "syncEvent.Accept(",
    "medicationDelivery.Id",
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
    "request.MarkAsDelivered ? null : request.ReceivedByName",
    "return 7;",
    "return 8;"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $ProcessorAndOrder $Token "SyncBatchProcessor medication delivery handler"
}

if ($Processor -match "delivery\.Id == medicationDeliveryId[\s\S]{0,120}delivery\.OrganizationId == organizationId") {
    throw "MedicationDelivery duplicate id check must not be tenant-scoped because primary key uniqueness is global."
}

Write-Host "P3 sync processor medication delivery handler verification passed." -ForegroundColor Green