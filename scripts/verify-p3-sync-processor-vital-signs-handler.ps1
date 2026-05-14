$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_VITAL_SIGNS_HANDLER_BASELINE.md"
$RequestPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/VitalSigns/CreateVitalSignsRecordRequest.cs"
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
    "P3 Sync Processor Vital Signs Handler Baseline",
    "EntityType: vital_signs",
    "Operation: create",
    "parse PayloadJson as CreateVitalSignsRecordRequest",
    "validate VisitId belongs to the same OrganizationId, PatientId, and parent SyncBatch.BrigadeId",
    "use canonical TemperatureCelsius",
    "processor must process patient_visit create events before vital_signs create events",
    "vital signs must remain historical records, not overwritten fields on Patient",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor vital signs handler baseline"
}

$RequiredRequestTokens = @(
    "CreateVitalSignsRecordRequest",
    "PatientId",
    "VisitId",
    "EncounterId",
    "MeasuredByUserId",
    "MeasuredAt",
    "SystolicBloodPressureMmHg",
    "DiastolicBloodPressureMmHg",
    "HeartRateBpm",
    "RespiratoryRatePerMinute",
    "TemperatureCelsius",
    "OxygenSaturationPercent",
    "WeightKg",
    "HeightCm",
    "GlucoseMgDl",
    "DeviceId"
)

foreach ($Token in $RequiredRequestTokens) {
    Assert-Contains $Request $Token "CreateVitalSignsRecordRequest"
}

$RequiredProcessorTokens = @(
    "HandleVitalSignsEventAsync",
    "syncEvent.EntityType == SyncEntityType.VitalSigns",
    "syncEvent.Operation != SyncOperation.Create",
    "vital_signs_operation_not_implemented",
    "JsonSerializer.Deserialize<CreateVitalSignsRecordRequest>",
    "new VitalSignsRecord(",
    "_dbContext.VitalSignsRecords.Add(vitalSignsRecord)",
    "syncEvent.Accept(",
    "vitalSignsRecord.Id",
    "vital_signs_patient_not_found",
    "vital_signs_visit_not_found",
    "vital_signs_encounter_not_found",
    "vital_signs_measured_by_user_not_found",
    "vital_signs_id_already_exists",
    "acceptedVitalSignsIdsInBatch",
    "acceptedVitalSignsIdsInBatch.Contains(vitalSignsRecordId)",
    "acceptedVitalSignsIdsInBatch.Add(vitalSignsRecordId)",
    "return 2;",
    "return 3;"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor vital signs handler"
}

$ForbiddenProcessorTokens = @(
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor vital signs handler scope violation: $Token"
    }
}

Write-Host "P3 sync processor vital signs handler verification passed." -ForegroundColor Green