$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_FORM_RESPONSE_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$FormHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/FormResponseSyncEventHandler.cs"

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
Assert-FileExists $FormHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$FormHandler = Get-Content $FormHandlerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Form Response Sync Event Handler Extraction Baseline",
    "FormResponseSyncEventHandler must own form_response/create payload parsing",
    "SyncBatchProcessor must not directly construct FormResponse",
    "SyncBatchProcessor must not directly parse CreateFormResponseRequest",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 form response sync event handler extraction baseline"
}

$RequiredProcessorTokens = @(
    "private readonly FormResponseSyncEventHandler _formResponseSyncEventHandler;",
    "_formResponseSyncEventHandler = new FormResponseSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandleFormResponseEventAsync",
    "await _formResponseSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor form response handler extraction"
}

$ForbiddenProcessorTokens = @(
    "out CreateFormResponseRequest? request",
    "var formResponse = new FormResponse(",
    "_dbContext.FormResponses.Add(formResponse)",
    "form_response_operation_not_implemented",
    "form_response_encounter_not_found",
    "form_response_brigade_mismatch",
    "form_response_template_not_found",
    "form_response_template_inactive",
    "form_response_template_not_yet_effective",
    "form_response_template_expired",
    "form_response_submitted_by_user_not_found",
    "form_response_id_already_exists",
    "form_response_duplicate_in_pending_batch",
    "form_response_duplicate_encounter_template_in_pending_batch",
    "form_response_duplicate_encounter_template",
    "formResponseIdReserved",
    "formResponseEncounterTemplateKeyReserved",
    "acceptedFormResponseIdsInBatch.Remove(formResponseId)"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct form response handler logic: $Token"
    }
}

$RequiredFormHandlerTokens = @(
    "internal sealed class FormResponseSyncEventHandler",
    "public async Task HandleAsync",
    "SyncPayloadReader.TryReadObject",
    "out CreateFormResponseRequest? request",
    "JsonDocument.Parse(request.ResponseJson)",
    "var formResponse = new FormResponse(",
    "_dbContext.FormResponses.Add(formResponse)",
    "syncEvent.Accept(",
    "formResponse.Id",
    "form_response_operation_not_implemented",
    "form_response_encounter_not_found",
    "form_response_brigade_mismatch",
    "form_response_template_not_found",
    "form_response_template_inactive",
    "form_response_template_not_yet_effective",
    "form_response_template_expired",
    "form_response_submitted_by_user_not_found",
    "form_response_id_already_exists",
    "form_response_duplicate_in_pending_batch",
    "form_response_duplicate_encounter_template_in_pending_batch",
    "form_response_duplicate_encounter_template",
    "acceptedFormResponseIdsInBatch",
    "acceptedFormResponseEncounterTemplateKeysInBatch",
    "reserved only after successful FormResponse construction and reserved atomically",
    "formResponseIdReserved",
    "formResponseEncounterTemplateKeyReserved",
    "acceptedFormResponseIdsInBatch.Remove(formResponseId)"
)

foreach ($Token in $RequiredFormHandlerTokens) {
    Assert-Contains $FormHandler $Token "FormResponseSyncEventHandler"
}

Write-Host "P3 form response sync event handler extraction verification passed." -ForegroundColor Green
