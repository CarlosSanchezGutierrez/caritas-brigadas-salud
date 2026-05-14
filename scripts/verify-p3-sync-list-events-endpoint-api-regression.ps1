$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_LIST_EVENTS_ENDPOINT_API_REGRESSION_BASELINE.md"
$TestPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncListEventsEndpointIntegrationTests.cs"
$ControllerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Controllers/SyncBatchesController.cs"
$DtoPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Contracts/Sync/SyncEventSummaryDto.cs"
$RepositoryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchReadRepository.cs"

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
Assert-FileExists $DtoPath
Assert-FileExists $RepositoryPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Test = Get-Content $TestPath -Raw -Encoding UTF8
$Controller = Get-Content $ControllerPath -Raw -Encoding UTF8
$Dto = Get-Content $DtoPath -Raw -Encoding UTF8
$Repository = Get-Content $RepositoryPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync List Events Endpoint API Regression Baseline",
    "GET /api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/events",
    "PermissionCodes.SyncBatchesRead",
    "401 Unauthorized",
    "payloadJson",
    "sensitive payload values",
    "Tenant boundary rule",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync list events endpoint API regression baseline"
}

$RequiredTestTokens = @(
    "P3SyncListEventsEndpointIntegrationTests",
    "WebApplicationFactory<Program>",
    "ListEventsEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
    "HttpStatusCode.Unauthorized",
    "ListEventsEndpoint_WhenAuthenticatedWithSyncReadPermission_ReturnsEventsWithoutPayloadJson",
    "ListEventsEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFound",
    "/api/v1/organizations/{organizationId}/sync-batches/{syncBatchId}/events",
    "Authentication:Mode",
    "Development",
    "X-Dev-User-Id",
    "X-Dev-Organization-Id",
    "X-Dev-Roles",
    "X-Dev-Permissions",
    "sync-batches.read",
    "UseInMemoryDatabase",
    "services.RemoveAll<DbContextOptions<CaritasDbContext>>();",
    "services.RemoveAll<DbContextOptions>();",
    "services.RemoveAll<IDbContextOptionsConfiguration<CaritasDbContext>>();",
    "services.AddScoped<ISyncBatchReadRepository, SyncBatchReadRepository>();",
    "SensitiveFirstNameShouldNotLeak",
    "Assert.DoesNotContain(""payloadJson"", responseBody, StringComparison.OrdinalIgnoreCase)",
    "Assert.False(item.TryGetProperty(""payloadJson"", out _))",
    "Assert.False(item.TryGetProperty(""payload"", out _))",
    "Assert.Equal(HttpStatusCode.OK, response.StatusCode)",
    "Assert.Equal(HttpStatusCode.NotFound, response.StatusCode)",
    "Sync batch was not found.",
    "Assert.Equal(SyncEventStatus.Pending, item.GetProperty(""status"").GetString())",
    "Assert.Equal(SyncEventStatus.Pending, syncEvent.Status)"
)

foreach ($Token in $RequiredTestTokens) {
    Assert-Contains $Test $Token "P3 sync list events endpoint API regression test"
}

$RequiredControllerTokens = @(
    "api/v1/organizations/{organizationId:guid}/sync-batches/{syncBatchId:guid}/events",
    "Authorize(Policy = PermissionCodes.SyncBatchesRead)",
    "ISyncBatchReadRepository",
    "repository.ListEventsByBatchAsync(",
    "Sync batch was not found."
)

foreach ($Token in $RequiredControllerTokens) {
    Assert-Contains $Controller $Token "SyncBatchesController list events endpoint"
}

if ($Dto.Contains("PayloadJson")) {
    throw "SyncEventSummaryDto must not expose PayloadJson."
}

if ($Repository -match "new SyncEventSummaryDto[\s\S]*PayloadJson") {
    throw "SyncBatchReadRepository projects PayloadJson into SyncEventSummaryDto."
}

Write-Host "P3 sync list events endpoint API regression verification passed." -ForegroundColor Green