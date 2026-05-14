$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$OrderPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncProcessingOrder.cs"
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

Assert-FileExists $ProcessorPath
Assert-FileExists $OrderPath
Assert-FileExists $FormHandlerPath

$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$Order = Get-Content $OrderPath -Raw -Encoding UTF8
$FormHandler = Get-Content $FormHandlerPath -Raw -Encoding UTF8
$ProcessorAndOrderAndFormHandler = $Processor + $Order + $FormHandler

$RequiredTokens = @(
    "HandleFormResponseEventAsync",
    "FormResponseSyncEventHandler",
    "SyncEntityType.FormResponse",
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

foreach ($Token in $RequiredTokens) {
    Assert-Contains $ProcessorAndOrderAndFormHandler $Token "SyncBatchProcessor form response handler"
}

$RequiredProcessorTokens = @(
    "private readonly FormResponseSyncEventHandler _formResponseSyncEventHandler;",
    "_formResponseSyncEventHandler = new FormResponseSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandleFormResponseEventAsync",
    "await _formResponseSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor form response handler wrapper"
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
        throw "SyncBatchProcessor still contains direct form response logic: $Token"
    }
}

Write-Host "P3 sync processor form response handler verification passed." -ForegroundColor Green
