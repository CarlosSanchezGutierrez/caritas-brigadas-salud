$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_EVENT_READ_MODEL_BASELINE.md"
$DtoPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/Sync/SyncEventSummaryDto.cs"
$InterfacePath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Application/Sync/ISyncBatchReadRepository.cs"
$RepositoryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchReadRepository.cs"
$ControllerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Controllers/SyncBatchesController.cs"

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
Assert-FileExists $DtoPath
Assert-FileExists $InterfacePath
Assert-FileExists $RepositoryPath
Assert-FileExists $ControllerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Dto = Get-Content $DtoPath -Raw -Encoding UTF8
$Interface = Get-Content $InterfacePath -Raw -Encoding UTF8
$Repository = Get-Content $RepositoryPath -Raw -Encoding UTF8
$Controller = Get-Content $ControllerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Event Read Model Baseline",
    "must not expose PayloadJson",
    "SyncEventSummaryDto must not expose",
    "query SyncEvents by OrganizationId and SyncBatchId",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync event read model baseline"
}

$RequiredDtoTokens = @(
    "SyncEventSummaryDto",
    "LocalEventId",
    "IdempotencyKey",
    "EntityType",
    "Operation",
    "Status",
    "ReceivedAtServer"
)

foreach ($Token in $RequiredDtoTokens) {
    Assert-Contains $Dto $Token "SyncEventSummaryDto"
}

if ($Dto.Contains("PayloadJson")) {
    throw "SyncEventSummaryDto must not expose PayloadJson."
}

Assert-Contains $Interface "ListEventsByBatchAsync" "ISyncBatchReadRepository"

$RequiredRepositoryTokens = @(
    "ListEventsByBatchAsync",
    "_dbContext.SyncEvents",
    "syncEvent.OrganizationId == organizationId",
    "syncEvent.SyncBatchId == syncBatchId",
    "new SyncEventSummaryDto"
)

foreach ($Token in $RequiredRepositoryTokens) {
    Assert-Contains $Repository $Token "SyncBatchReadRepository"
}

if ($Repository.Contains("PayloadJson =")) {
    throw "SyncBatchReadRepository must not project PayloadJson into the read model."
}

if ($Repository.Contains("IsPending = syncEvent.IsPending") -or
    $Repository.Contains("IsAccepted = syncEvent.IsAccepted") -or
    $Repository.Contains("IsRejected = syncEvent.IsRejected") -or
    $Repository.Contains("IsConflict = syncEvent.IsConflict")) {
    throw "SyncBatchReadRepository must not project unmapped SyncEvent status getters because that can materialize PayloadJson."
}

Assert-Contains $Repository "IsPending = syncEvent.Status == SyncEventStatus.Pending" "SyncBatchReadRepository"
Assert-Contains $Repository "IsAccepted = syncEvent.Status == SyncEventStatus.Accepted" "SyncBatchReadRepository"
Assert-Contains $Repository "IsRejected = syncEvent.Status == SyncEventStatus.Rejected" "SyncBatchReadRepository"
Assert-Contains $Repository "IsConflict = syncEvent.Status == SyncEventStatus.Conflict" "SyncBatchReadRepository"

$RequiredControllerTokens = @(
    "ListEventsByBatchAsync",
    "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/events",
    "Authorize(Policy = PermissionCodes.SyncBatchesRead)",
    "batch is null || batch.OrganizationId != organizationId"
)

foreach ($Token in $RequiredControllerTokens) {
    Assert-Contains $Controller $Token "SyncBatchesController"
}

Write-Host "P3 sync event read model verification passed." -ForegroundColor Green