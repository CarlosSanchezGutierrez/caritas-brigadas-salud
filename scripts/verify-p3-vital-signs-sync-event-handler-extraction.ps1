$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_VITAL_SIGNS_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$VitalHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/VitalSignsSyncEventHandler.cs"

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
Assert-FileExists $VitalHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$VitalHandler = Get-Content $VitalHandlerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Vital Signs Sync Event Handler Extraction Baseline",
    "VitalSignsSyncEventHandler must own vital_signs/create payload parsing",
    "SyncBatchProcessor must not directly construct VitalSignsRecord",
    "SyncBatchProcessor must not directly parse CreateVitalSignsRecordRequest",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 vital signs sync event handler extraction baseline"
}

$RequiredProcessorTokens = @(
    "private readonly VitalSignsSyncEventHandler _vitalSignsSyncEventHandler;",
    "_vitalSignsSyncEventHandler = new VitalSignsSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandleVitalSignsEventAsync",
    "await _vitalSignsSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor vital signs handler extraction"
}

$ForbiddenProcessorTokens = @(
    "out CreateVitalSignsRecordRequest? request",
    "var vitalSignsRecord = new VitalSignsRecord(",
    "_dbContext.VitalSignsRecords.Add(vitalSignsRecord)",
    "vital_signs_operation_not_implemented",
    "vital_signs_patient_not_found",
    "vital_signs_visit_not_found",
    "vital_signs_encounter_not_found",
    "vital_signs_measured_by_user_not_found",
    "vital_signs_id_already_exists",
    "vital_signs_duplicate_in_pending_batch"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains direct vital signs handler logic: $Token"
    }
}

$RequiredVitalHandlerTokens = @(
    "internal sealed class VitalSignsSyncEventHandler",
    "public async Task HandleAsync",
    "SyncPayloadReader.TryReadObject",
    "out CreateVitalSignsRecordRequest? request",
    "var vitalSignsRecord = new VitalSignsRecord(",
    "_dbContext.VitalSignsRecords.Add(vitalSignsRecord)",
    "syncEvent.Accept(",
    "vitalSignsRecord.Id",
    "vital_signs_operation_not_implemented",
    "vital_signs_patient_not_found",
    "vital_signs_visit_not_found",
    "vital_signs_encounter_not_found",
    "vital_signs_measured_by_user_not_found",
    "vital_signs_id_already_exists",
    "vital_signs_duplicate_in_pending_batch",
    "acceptedVitalSignsIdsInBatch",
    "request.SystolicBloodPressureMmHg",
    "request.DiastolicBloodPressureMmHg",
    "request.HeartRateBpm",
    "request.OxygenSaturationPercent",
    "request.GlucoseMgDl"
)

foreach ($Token in $RequiredVitalHandlerTokens) {
    Assert-Contains $VitalHandler $Token "VitalSignsSyncEventHandler"
}

Write-Host "P3 vital signs sync event handler extraction verification passed." -ForegroundColor Green