$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_BATCH_EVENT_INTAKE_BASELINE.md"
$RequestPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/Sync/CreateSyncBatchRequest.cs"
$RepositoryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchWriteRepository.cs"

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
Assert-FileExists $RepositoryPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Request = Get-Content $RequestPath -Raw -Encoding UTF8
$Repository = Get-Content $RepositoryPath -Raw -Encoding UTF8

$Fence = "$([char]96)$([char]96)$([char]96)"

if ($Doc.Contains($Fence)) {
    throw "P3 sync batch event intake baseline must not contain Markdown code fences."
}

$RequiredDocTokens = @(
    "P3 Sync Batch Event Intake Baseline",
    "Payload envelope",
    "Idempotency behavior",
    "Safe staging",
    "P3-10 only stages events",
    "Future processor handoff",
    "Acceptance criteria",
    "SyncBatchWriteRepository does not apply clinical domain writes"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync batch event intake baseline"
}

$RequiredRequestTokens = @(
    "ClientInstanceId",
    "[MaxLength(150)]",
    "PayloadJson"
)

foreach ($Token in $RequiredRequestTokens) {
    Assert-Contains $Request $Token "CreateSyncBatchRequest"
}

$RequiredRepositoryTokens = @(
    "ExtractSyncPayloadEvents",
    "ParseSyncPayloadEvent",
    "BuildIdempotencyKey",
    "Client instance id is required when device id is not provided.",
    "_dbContext.SyncBatches.Add(batch)",
    "_dbContext.SyncEvents.AddRange(events)",
    "existingEvents",
    "return ToSummaryDto(existingBatch);",
    "Payload contains sync events that were already submitted in a different batch.",
    "existingKeySet",
    "Payload contains duplicate sync event idempotency keys.",
    "Events count does not match payload event count.",
    "eventsCount: syncPayloadEvents.Count",
    "new SyncEvent("
)

foreach ($Token in $RequiredRepositoryTokens) {
    Assert-Contains $Repository $Token "SyncBatchWriteRepository"
}

Write-Host "P3 sync batch event intake verification passed." -ForegroundColor Green
