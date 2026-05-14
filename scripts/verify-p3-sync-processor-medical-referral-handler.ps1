$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_MEDICAL_REFERRAL_HANDLER_BASELINE.md"
$RequestPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/MedicalReferrals/CreateMedicalReferralRequest.cs"
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
    "P3 Sync Processor Medical Referral Handler Baseline",
    "EntityType: medical_referral",
    "Operation: create",
    "parse PayloadJson as CreateMedicalReferralRequest",
    "derive PatientId from ServiceEncounter.PatientId, not from payload trust",
    "reject ProviderSignatureId until the document_signature handler exists",
    "ReferralFolio is the stable traceability key for printed/PDF passes",
    "reserve pending-batch medical referral id and referral folio atomically",
    "rollback the medical referral id reservation when referral folio reservation fails",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor medical referral handler baseline"
}

$RequiredRequestTokens = @(
    "CreateMedicalReferralRequest",
    "EncounterId",
    "ReferralFolio",
    "DestinationInstitution",
    "ReferralReason",
    "Priority",
    "ReferredByUserId",
    "ProviderSignatureId",
    "CreatedOffline",
    "DeviceId"
)

foreach ($Token in $RequiredRequestTokens) {
    Assert-Contains $Request $Token "CreateMedicalReferralRequest"
}

$RequiredProcessorTokens = @(
    "HandleMedicalReferralEventAsync",
    "syncEvent.EntityType == SyncEntityType.MedicalReferral",
    "syncEvent.Operation != SyncOperation.Create",
    "medical_referral_operation_not_implemented",
    "out CreateMedicalReferralRequest? request",
    "new MedicalReferral(",
    "_dbContext.Set<MedicalReferral>().Add(medicalReferral)",
    "syncEvent.Accept(",
    "medicalReferral.Id",
    "medical_referral_encounter_not_found",
    "medical_referral_brigade_mismatch",
    "medical_referral_patient_not_found",
    "medical_referral_referred_by_user_not_found",
    "medical_referral_provider_signature_not_supported_until_document_signature_handler",
    "medical_referral_id_already_exists",
    "medical_referral_folio_already_exists",
    "medical_referral_folio_duplicate_in_pending_batch",
    "acceptedMedicalReferralIdsInBatch",
    "acceptedMedicalReferralFoliosInBatch",
    "GenerateSyncMedicalReferralFolio",
    "medicalReferralIdReserved",
    "medicalReferralFolioReserved",
    "acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId)",
    "reserved only after successful MedicalReferral construction and reserved atomically",
    "Medical referral id duplicate checks include soft-deleted rows because primary key uniqueness is not filtered by IsDeleted",
    "Medical referral folio duplicate checks include soft-deleted rows because database unique index is not filtered by IsDeleted",
    "return 6;",
    "return 7;"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $ProcessorAndOrder $Token "SyncBatchProcessor medical referral handler"
}

$ForbiddenProcessorTokens = @(
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor medical referral handler scope violation: $Token"
    }
}

if ($Processor -match "referral\.Id == medicalReferralId[\s\S]{0,180}!referral\.IsDeleted") {
    throw "MedicalReferral id duplicate check must include soft-deleted rows."
}

if ($Processor -match "referral\.ReferralFolio == normalizedReferralFolio[\s\S]{0,180}!referral\.IsDeleted") {
    throw "MedicalReferral folio duplicate check must include soft-deleted rows because the unique index is not soft-delete filtered."
}

Write-Host "P3 sync processor medical referral handler verification passed." -ForegroundColor Green