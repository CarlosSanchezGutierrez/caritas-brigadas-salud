$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_TENANT_BOUNDARY_ENDPOINT_API_REGRESSION_BASELINE.md"
$TestPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncTenantBoundaryEndpointIntegrationTests.cs"
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
    "P3 Sync Tenant Boundary Endpoint API Regression Baseline",
    "GET /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}",
    "POST /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/process",
    "PermissionCodes.SyncBatchesRead",
    "PermissionCodes.SyncBatchesWrite",
    "Tenant boundary rule",
    "404 NotFound",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync tenant boundary endpoint API regression baseline"
}

$RequiredTestTokens = @(
    "P3SyncTenantBoundaryEndpointIntegrationTests",
    "WebApplicationFactory<Program>",
    "GetByIdEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFoundWithoutLeakingPayload",
    "ProcessEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFoundAndDoesNotProcess",
    "/api/v1/organizations/{otherOrganizationId}/sync-batches/{syncBatchId}",
    "/api/v1/organizations/{otherOrganizationId}/sync-batches/{syncBatchId}/process",
    "Authentication:Mode",
    "Development",
    "X-Dev-Organization-Id",
    "sync-batches.read",
    "sync-batches.write",
    "UseInMemoryDatabase",
    "services.RemoveAll<DbContextOptions<CaritasDbContext>>();",
    "services.RemoveAll<DbContextOptions>();",
    "services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();",
    "services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();",
    "services.AddScoped<ISyncBatchProcessor, SyncBatchProcessor>();",
    "HttpStatusCode.NotFound",
    "Sync batch was not found.",
    "TenantBoundarySensitiveNameShouldNotLeak",
    "TenantBoundarySensitivePhoneShouldNotLeak",
    "Assert.DoesNotContain(""payloadJson"", responseBody, StringComparison.OrdinalIgnoreCase)",
    "Assert.Equal(0, await dbContext.Patients.CountAsync(cancellationToken))",
    "Assert.Equal(SyncBatchStatus.Received, batch.Status)",
    "Assert.Equal(0, batch.AcceptedCount)",
    "Assert.Equal(0, batch.RejectedCount)",
    "Assert.Equal(0, batch.ConflictCount)",
    "Assert.Equal(SyncEventStatus.Pending, syncEvent.Status)"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 sync tenant boundary endpoint API regression test"
}

$RequiredControllerTokens = @(
    "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}",
    "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/process",
    "Authorize(Policy = PermissionCodes.SyncBatchesRead)",
    "Authorize(Policy = PermissionCodes.SyncBatchesWrite)",
    "Sync batch was not found."
)

foreach ($Token in $RequiredControllerTokens) {
    Assert-Contains $Controller $Token "SyncBatchesController tenant boundary endpoints"
}

Write-Host "P3 sync tenant boundary endpoint API regression verification passed." -ForegroundColor Green