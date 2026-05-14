$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_CONSENT_DOCUMENT_HANDLER_BASELINE.md"
$RequestPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/ConsentDocuments/CreateConsentDocumentRequest.cs"
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
Assert-FileExists $RequestPath
Assert-FileExists $ProcessorPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Request = Get-Content $RequestPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Processor Consent Document Handler Baseline",
    "EntityType: consent_document",
    "Operation: create",
    "parse PayloadJson as CreateConsentDocumentRequest",
    "require SignatureDataUrl",
    "preserve DocumentTextSnapshot as the legal text snapshot",
    "preserve SignatureDataUrl as the captured signature evidence",
    "reserve pending-batch consent document id and patient-visit-type-version keys only after successful ConsentDocument construction",
    "processor response must not expose SignatureDataUrl",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor consent document handler baseline"
}

$RequiredRequestTokens = @(
    "CreateConsentDocumentRequest",
    "PatientId",
    "VisitId",
    "ConsentType",
    "DocumentVersion",
    "DocumentTextSnapshot",
    "SignatureDataUrl",
    "GuardianFullName",
    "GuardianRelationship",
    "SignedByUserId",
    "SignedAt",
    "CreatedOffline",
    "DeviceId"
)

foreach ($Token in $RequiredRequestTokens) {
    Assert-Contains $Request $Token "CreateConsentDocumentRequest"
}

$RequiredProcessorTokens = @(
    "HandleConsentDocumentEventAsync",
    "syncEvent.EntityType == SyncEntityType.ConsentDocument",
    "syncEvent.Operation != SyncOperation.Create",
    "consent_document_operation_not_implemented",
    "JsonSerializer.Deserialize<CreateConsentDocumentRequest>",
    "CreateConsentDocumentForSync",
    "SetConsentPropertyIfExists",
    "_dbContext.Set<ConsentDocument>().Add(consentDocument)",
    "syncEvent.Accept(",
    "consentDocument.Id",
    "consent_document_patient_not_found",
    "consent_document_visit_not_found",
    "consent_document_signed_by_user_not_found",
    "consent_document_id_already_exists",
    "consent_document_duplicate_patient_visit_type_version",
    "consent_document_duplicate_patient_visit_type_version_in_pending_batch",
    "acceptedConsentDocumentIdsInBatch",
    "acceptedConsentDocumentKeysInBatch",
    "reserved only after successful ConsentDocument construction",
    "return 5;",
    "return 6;"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor consent document handler"
}

$ForbiddenProcessorTokens = @(
    "_dbContext.MedicalReferrals.Add",
    "_dbContext.MedicationDeliveries.Add"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor consent document handler scope violation: $Token"
    }
}

Write-Host "P3 sync processor consent document handler verification passed." -ForegroundColor Green