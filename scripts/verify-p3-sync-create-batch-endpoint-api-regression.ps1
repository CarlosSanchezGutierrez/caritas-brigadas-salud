$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_CREATE_BATCH_ENDPOINT_API_REGRESSION_BASELINE.md"
$TestPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncCreateBatchEndpointIntegrationTests.cs"
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
    "P3 Sync Create Batch Endpoint API Regression Baseline",
    "POST /api/v1/organizations/{organizationId}/sync-batches",
    "PermissionCodes.SyncBatchesWrite",
    "401 Unauthorized",
    "HTTP 201 Created",
    "Sync batch received successfully.",
    "Create sync batch endpoint is intake only",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync create batch endpoint API regression baseline"
}

$RequiredTestTokens = @(
    "P3SyncCreateBatchEndpointIntegrationTests",
    "WebApplicationFactory<Program>",
    "CreateEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
    "HttpStatusCode.Unauthorized",
    "CreateEndpoint_WhenAuthenticatedWithSyncWritePermission_CreatesBatchAndEvents",
    "/api/v1/organizations/{organizationId}/sync-batches",
    "Authentication:Mode",
    "Development",
    "X-Dev-User-Id",
    "X-Dev-Organization-Id",
    "X-Dev-Roles",
    "X-Dev-Permissions",
    "sync-batches.write",
    "UseInMemoryDatabase",
    "services.RemoveAll<DbContextOptions<CaritasDbContext>>();",
    "services.RemoveAll<DbContextOptions>();",
    "services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();",
    "services.AddScoped<ISyncBatchWriteRepository, SyncBatchWriteRepository>();",
    "CreateSyncBatchRequest",
    "PayloadJson",
    "SyncEntityType.Patient",
    "SyncOperation.Create",
    "Assert.Equal(HttpStatusCode.Created, response.StatusCode)",
    "Assert.NotNull(response.Headers.Location)",
    "Sync batch received successfully.",
    "eventsCount",
    "received",
    "isCompleted",
    "Assert.Equal(1, await dbContext.SyncBatches.CountAsync(cancellationToken))",
    "Assert.Equal(1, await dbContext.SyncEvents.CountAsync(cancellationToken))",
    "Assert.Equal(0, await dbContext.Patients.CountAsync(cancellationToken))",
    "Assert.Equal(SyncBatchStatus.Received, batch.Status)",
    "Assert.Equal(SyncEventStatus.Pending, syncEvent.Status)",
    "syncEvent.IdempotencyKey"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 sync create batch endpoint API regression test"
}

$RequiredControllerTokens = @(
    "api/v1/organizations/{organizationId:guid}/sync-batches",
    "Authorize(Policy = PermissionCodes.SyncBatchesWrite)",
    "ISyncBatchWriteRepository",
    "repository.CreateAsync(",
    "Sync batch received successfully."
)

foreach ($Token in $RequiredControllerTokens) {
    Assert-Contains $Controller $Token "SyncBatchesController create endpoint"
}

Write-Host "P3 sync create batch endpoint API regression verification passed." -ForegroundColor Green