$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_SKELETON_BASELINE.md"
$InterfacePath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Application/Sync/ISyncBatchProcessor.cs"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$ControllerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Controllers/SyncBatchesController.cs"
$ResultDtoPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/Sync/ProcessSyncBatchResultDto.cs"
$DependencyInjectionPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/DependencyInjection.cs"

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
Assert-FileExists $InterfacePath
Assert-FileExists $ProcessorPath
Assert-FileExists $ControllerPath
Assert-FileExists $ResultDtoPath
Assert-FileExists $DependencyInjectionPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Interface = Get-Content $InterfacePath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$Controller = Get-Content $ControllerPath -Raw -Encoding UTF8
$ResultDto = Get-Content $ResultDtoPath -Raw -Encoding UTF8
$DependencyInjection = Get-Content $DependencyInjectionPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Processor Skeleton Baseline",
    "must not expose PayloadJson in the response",
    "mark valid pending events as conflict because domain handlers are not implemented yet",
    "processor must not complete against client-supplied event totals",
    "P3-13 supersedes the skeleton for patient create",
    "P3-14 supersedes the skeleton for patient_visit create",
    "P3-15 supersedes the skeleton for vital_signs create",
    "P3-16 supersedes the skeleton for service_encounter create",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor skeleton baseline"
}

Assert-Contains $Interface "ISyncBatchProcessor" "ISyncBatchProcessor"
Assert-Contains $Interface "Task<ProcessSyncBatchResultDto> ProcessAsync" "ISyncBatchProcessor"

$RequiredProcessorTokens = @(
    "SyncBatchProcessor",
    "batch.MarkProcessing()",
    "syncEvent.Status == SyncEventStatus.Pending",
    "syncEvent.MarkProcessing()",
    "TryValidateEvent",
    "SyncEntityType.IsAllowed",
    "SyncOperation.IsAllowed",
    "JsonDocument.Parse(syncEvent.PayloadJson)",
    "syncEvent.Reject(",
    "syncEvent.MarkConflict(",
    "batch.Complete(",
    "allEvents.Count(syncEvent => syncEvent.Status == SyncEventStatus.Accepted)",
    "allEvents.Count(syncEvent => syncEvent.Status == SyncEventStatus.Rejected)",
    "allEvents.Count(syncEvent => syncEvent.Status == SyncEventStatus.Conflict)"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor"
}

$ForbiddenProcessorTokens = @(
    "_dbContext.FormResponses.Add",
    "_dbContext.ConsentDocuments.Add",
    "_dbContext.MedicalReferrals.Add",
    "_dbContext.MedicationDeliveries.Add"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor contains forbidden token: $Token"
    }
}

Assert-Contains $Controller "GetService<ISyncBatchProcessor>" "SyncBatchesController"
Assert-Contains $Controller "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/process" "SyncBatchesController"
Assert-Contains $Controller "Authorize(Policy = PermissionCodes.SyncBatchesWrite)" "SyncBatchesController"

Assert-Contains $ResultDto "ProcessSyncBatchResultDto" "ProcessSyncBatchResultDto"

if ($ResultDto.Contains("PayloadJson")) {
    throw "ProcessSyncBatchResultDto must not expose PayloadJson."
}

Assert-Contains $DependencyInjection "AddScoped<ISyncBatchProcessor, SyncBatchProcessor>" "DependencyInjection"

Write-Host "P3 sync processor skeleton verification passed." -ForegroundColor Green