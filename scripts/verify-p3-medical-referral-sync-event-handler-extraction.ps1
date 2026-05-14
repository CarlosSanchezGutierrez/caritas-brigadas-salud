$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_MEDICAL_REFERRAL_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$ReferralHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/MedicalReferralSyncEventHandler.cs"

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
Assert-FileExists $ReferralHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$ReferralHandler = Get-Content $ReferralHandlerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Medical Referral Sync Event Handler Extraction Baseline",
    "MedicalReferralSyncEventHandler must own medical_referral/create payload parsing",
    "SyncBatchProcessor must not directly construct MedicalReferral",
    "SyncBatchProcessor must not directly parse CreateMedicalReferralRequest",
    "SyncBatchProcessor must not contain GenerateSyncMedicalReferralFolio",
    "Traceability requirement",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 medical referral sync event handler extraction baseline"
}

$RequiredProcessorTokens = @(
    "private readonly MedicalReferralSyncEventHandler _medicalReferralSyncEventHandler;",
    "_medicalReferralSyncEventHandler = new MedicalReferralSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandleMedicalReferralEventAsync",
    "await _medicalReferralSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor medical referral handler extraction"
}

$ForbiddenProcessorTokens = @(
    "out CreateMedicalReferralRequest? request",
    "new MedicalReferral(",
    "_dbContext.Set<MedicalReferral>().Add(medicalReferral)",
    "medical_referral_operation_not_implemented",
    "medical_referral_encounter_not_found",
    "medical_referral_brigade_mismatch",
    "medical_referral_patient_not_found",
    "medical_referral_referred_by_user_not_found",
    "medical_referral_provider_signature_not_supported_until_document_signature_handler",
    "medical_referral_id_already_exists",
    "medical_referral_folio_already_exists",
    "medical_referral_folio_duplicate_in_pending_batch",
    "GenerateSyncMedicalReferralFolio",
    "medicalReferralIdReserved",
    "medicalReferralFolioReserved",
    "acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId)"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct medical referral handler logic: $Token"
    }
}

$RequiredReferralHandlerTokens = @(
    "internal sealed class MedicalReferralSyncEventHandler",
    "public async Task HandleAsync",
    "SyncPayloadReader.TryReadObject",
    "out CreateMedicalReferralRequest? request",
    "new MedicalReferral(",
    "_dbContext.Set<MedicalReferral>().Add(medicalReferral)",
    "syncEvent.Accept(",
    "medicalReferral.Id",
    "medical_referral_operation_not_implemented",
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
    "private static string GenerateSyncMedicalReferralFolio",
    "medicalReferralIdReserved",
    "medicalReferralFolioReserved",
    "acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId)",
    "reserved only after successful MedicalReferral construction and reserved atomically",
    "Medical referral id duplicate checks include soft-deleted rows because primary key uniqueness is not filtered by IsDeleted",
    "Medical referral folio duplicate checks include soft-deleted rows because database unique index is not filtered by IsDeleted"
)

foreach ($Token in $RequiredReferralHandlerTokens) {
    Assert-Contains $ReferralHandler $Token "MedicalReferralSyncEventHandler"
}

if ($ReferralHandler -match "referral\.Id == medicalReferralId[\s\S]{0,180}!referral\.IsDeleted") {
    throw "MedicalReferral id duplicate check must include soft-deleted rows."
}

if ($ReferralHandler -match "referral\.ReferralFolio == normalizedReferralFolio[\s\S]{0,180}!referral\.IsDeleted") {
    throw "MedicalReferral folio duplicate check must include soft-deleted rows because the unique index is not soft-delete filtered."
}

Write-Host "P3 medical referral sync event handler extraction verification passed." -ForegroundColor Green
