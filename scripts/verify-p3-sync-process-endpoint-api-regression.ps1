$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESS_ENDPOINT_API_REGRESSION_BASELINE.md"
$TestPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncProcessEndpointIntegrationTests.cs"
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
Assert-FileExists $TestPath
Assert-FileExists $ControllerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Test = Get-Content $TestPath -Raw -Encoding UTF8
$Controller = Get-Content $ControllerPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Process Endpoint API Regression Baseline",
    "POST /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process",
    "PermissionCodes.SyncBatchesWrite",
    "401 Unauthorized",
    "Sync batch processed successfully.",
    "stale skeleton wording is removed",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync process endpoint API regression baseline"
}

$RequiredTestTokens = @(
    "P3SyncProcessEndpointIntegrationTests",
    "WebApplicationFactory<Program>",
    "Authentication:Mode",
    "Development",
    "X-Dev-User-Id",
    "X-Dev-Organization-Id",
    "X-Dev-Roles",
    "X-Dev-Permissions",
    "sync-batches.write",
    "ProcessEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
    "HttpStatusCode.Unauthorized",
    "ProcessEndpoint_WhenAuthenticatedWithSyncWritePermission_ProcessesPendingBatch",
    "/api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process",
    "UseInMemoryDatabase",
    "SyncEntityType.Patient",
    "SyncOperation.Create",
    "Assert.Equal(HttpStatusCode.OK, response.StatusCode)",
    "Sync batch processed successfully.",
    "pendingEventsProcessed",
    "acceptedCount",
    "rejectedCount",
    "conflictCount",
    "Assert.Equal(1, await dbContext.Patients.CountAsync(cancellationToken))",
    "Assert.Equal(SyncEventStatus.Accepted, syncEvent.Status)",
    "Assert.Equal(SyncBatchStatus.Completed, completedBatch.Status)"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 sync process endpoint API regression test"
}

$RequiredControllerTokens = @(
    "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/process",
    "Authorize(Policy = PermissionCodes.SyncBatchesWrite)",
    "ISyncBatchProcessor",
    "processor.ProcessAsync(",
    "Sync batch processed successfully."
)

foreach ($Token in $RequiredControllerTokens) {
    Assert-Contains $Controller $Token "SyncBatchesController process endpoint"
}

$ForbiddenControllerTokens = @(
    "skeleton processor",
    "sin aplicar todavía escrituras clínicas reales"
)

foreach ($Token in $ForbiddenControllerTokens) {
    if ($Controller.Contains($Token)) {
        throw "SyncBatchesController still contains stale token: $Token"
    }
}

Write-Host "P3 sync process endpoint API regression verification passed." -ForegroundColor Green