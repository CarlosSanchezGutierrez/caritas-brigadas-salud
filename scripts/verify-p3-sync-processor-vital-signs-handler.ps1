$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$OrderPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncProcessingOrder.cs"
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

Assert-FileExists $ProcessorPath
Assert-FileExists $OrderPath
Assert-FileExists $VitalHandlerPath

$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$Order = Get-Content $OrderPath -Raw -Encoding UTF8
$VitalHandler = Get-Content $VitalHandlerPath -Raw -Encoding UTF8
$ProcessorAndOrderAndVitalHandler = $Processor + $Order + $VitalHandler

$RequiredTokens = @(
    "HandleVitalSignsEventAsync",
    "VitalSignsSyncEventHandler",
    "SyncEntityType.VitalSigns",
    "return 3;",
    "return 4;",
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
    "acceptedVitalSignsIdsInBatch"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $ProcessorAndOrderAndVitalHandler $Token "SyncBatchProcessor vital signs handler"
}

$RequiredProcessorTokens = @(
    "private readonly VitalSignsSyncEventHandler _vitalSignsSyncEventHandler;",
    "_vitalSignsSyncEventHandler = new VitalSignsSyncEventHandler(dbContext, PayloadJsonOptions);",
    "    private async Task HandleVitalSignsEventAsync",
    "await _vitalSignsSyncEventHandler.HandleAsync("
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor vital signs handler wrapper"
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
        throw "SyncBatchProcessor still contains direct vital signs logic: $Token"
    }
}

Write-Host "P3 sync processor vital signs handler verification passed." -ForegroundColor Green
