$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_CONSENT_DOCUMENT_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$ConsentHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/ConsentDocumentSyncEventHandler.cs"

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
Assert-FileExists $ConsentHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$ConsentHandler = Get-Content $ConsentHandlerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Consent Document Sync Event Handler Extraction Baseline",
    "ConsentDocumentSyncEventHandler must own consent_document/create payload parsing",
    "SyncBatchProcessor must not directly create ConsentDocument",
    "SyncBatchProcessor must not directly parse CreateConsentDocumentRequest",
    "SyncBatchProcessor must not contain CreateConsentDocumentForSync",
    "SyncBatchProcessor must not contain SetConsentPropertyIfExists",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 consent document sync event handler extraction baseline"
}

$RequiredProcessorTokens = @(
    "private readonly ConsentDocumentSyncEventHandler _consentDocumentSyncEventHandler;",
    "_consentDocumentSyncEventHandler = new ConsentDocumentSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandleConsentDocumentEventAsync",
    "await _consentDocumentSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor consent document handler extraction"
}

$ForbiddenProcessorTokens = @(
    "out CreateConsentDocumentRequest? request",
    "CreateConsentDocumentForSync",
    "SetConsentPropertyIfExists",
    "_dbContext.Set<ConsentDocument>().Add(consentDocument)",
    "consent_document_operation_not_implemented",
    "consent_document_patient_not_found",
    "consent_document_visit_not_found",
    "consent_document_signed_by_user_not_found",
    "consent_document_id_already_exists",
    "consent_document_duplicate_patient_visit_type_version",
    "consent_document_duplicate_patient_visit_type_version_in_pending_batch",
    "consentDocumentIdReserved",
    "consentDocumentKeyReserved",
    "acceptedConsentDocumentIdsInBatch.Remove(consentDocumentId)"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct consent document handler logic: $Token"
    }
}

$RequiredConsentHandlerTokens = @(
    "internal sealed class ConsentDocumentSyncEventHandler",
    "public async Task HandleAsync",
    "SyncPayloadReader.TryReadObject",
    "out CreateConsentDocumentRequest? request",
    "CreateConsentDocumentForSync",
    "SetConsentPropertyIfExists",
    "_dbContext.Set<ConsentDocument>().Add(consentDocument)",
    "syncEvent.Accept(",
    "consentDocument.Id",
    "consent_document_operation_not_implemented",
    "consent_document_patient_not_found",
    "consent_document_visit_not_found",
    "consent_document_signed_by_user_not_found",
    "consent_document_id_already_exists",
    "consent_document_duplicate_patient_visit_type_version",
    "consent_document_duplicate_patient_visit_type_version_in_pending_batch",
    "acceptedConsentDocumentIdsInBatch",
    "acceptedConsentDocumentKeysInBatch",
    "reserved only after successful ConsentDocument construction",
    "reserved atomically",
    "consentDocumentIdReserved",
    "consentDocumentKeyReserved",
    "acceptedConsentDocumentIdsInBatch.Remove(consentDocumentId)",
    "SignatureDataUrl",
    "DocumentTextSnapshot",
    "GuardianFullName",
    "GuardianRelationship",
    "BindingFlags.Instance",
    "property.SetValue(instance, value)"
)

foreach ($Token in $RequiredConsentHandlerTokens) {
    Assert-Contains $ConsentHandler $Token "ConsentDocumentSyncEventHandler"
}

Write-Host "P3 consent document sync event handler extraction verification passed." -ForegroundColor Green