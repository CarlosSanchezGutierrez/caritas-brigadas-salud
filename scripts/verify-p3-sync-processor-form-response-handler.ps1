$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_FORM_RESPONSE_HANDLER_BASELINE.md"
$RequestPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/FormResponses/CreateFormResponseRequest.cs"
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
    "P3 Sync Processor Form Response Handler Baseline",
    "EntityType: form_response",
    "Operation: create",
    "parse PayloadJson as CreateFormResponseRequest",
    "validate ResponseJson is valid JSON",
    "validate FormTemplateId belongs to the same OrganizationId and to the encounter ServiceId",
    "reserve pending-batch form response id and encounter-template keys only after successful FormResponse construction",
    "processor must process service_encounter create events before form_response create events",
    "processor must not log raw PayloadJson or ResponseJson",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor form response handler baseline"
}

$RequiredRequestTokens = @(
    "CreateFormResponseRequest",
    "EncounterId",
    "FormTemplateId",
    "ResponseJson",
    "SubmittedByUserId",
    "SubmittedAt",
    "CreatedOffline",
    "DeviceId"
)

foreach ($Token in $RequiredRequestTokens) {
    Assert-Contains $Request $Token "CreateFormResponseRequest"
}

$RequiredProcessorTokens = @(
    "HandleFormResponseEventAsync",
    "syncEvent.EntityType == SyncEntityType.FormResponse",
    "syncEvent.Operation != SyncOperation.Create",
    "form_response_operation_not_implemented",
    "JsonSerializer.Deserialize<CreateFormResponseRequest>",
    "new FormResponse(",
    "_dbContext.FormResponses.Add(formResponse)",
    "syncEvent.Accept(",
    "formResponse.Id",
    "form_response_encounter_not_found",
    "form_response_brigade_mismatch",
    "form_response_template_not_found",
    "form_response_template_inactive",
    "form_response_template_not_yet_effective",
    "form_response_template_expired",
    "form_response_submitted_by_user_not_found",
    "form_response_id_already_exists",
    "form_response_duplicate_encounter_template",
    "form_response_duplicate_encounter_template_in_pending_batch",
    "acceptedFormResponseIdsInBatch",
    "acceptedFormResponseEncounterTemplateKeysInBatch",
    "reserved only after successful FormResponse construction",
    "return 4;",
    "return 5;"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor form response handler"
}

$ForbiddenProcessorTokens = @(
    "_dbContext.MedicalReferrals.Add",
    "_dbContext.MedicationDeliveries.Add"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor form response handler scope violation: $Token"
    }
}

Write-Host "P3 sync processor form response handler verification passed." -ForegroundColor Green