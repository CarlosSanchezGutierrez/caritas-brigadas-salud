$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

function Assert-FileExists {
    param(
        [string]$Path,
        [string]$Label
    )

    if (-not (Test-Path $Path)) {
        throw "$Label file not found: $Path"
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

$ChecklistPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_BACKEND_READINESS_CHECKLIST.md"

Assert-FileExists $ChecklistPath "P3 sync backend readiness checklist"

$Checklist = Get-Content $ChecklistPath -Raw -Encoding UTF8

$RequiredChecklistTokens = @(
    "P3 Sync Backend Readiness Checklist",
    "Backend sync readiness status: ready for next backend workstream.",
    "Processor-level coverage closed",
    "API-level coverage closed",
    "Privacy coverage closed",
    "Tenant boundary coverage closed",
    "Governance and CI coverage closed",
    "Required evidence files",
    "Explicit non-goals",
    "Next backend workstreams",
    "Acceptance criteria"
)

foreach ($Token in $RequiredChecklistTokens) {
    Assert-Contains $Checklist $Token "P3 sync backend readiness checklist"
}

$RequiredFiles = @(
    @{
        Label = "clinical sync end-to-end integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3ClinicalSyncEndToEndIntegrationTests.cs"
    },
    @{
        Label = "sync process endpoint integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncProcessEndpointIntegrationTests.cs"
    },
    @{
        Label = "sync create batch endpoint integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncCreateBatchEndpointIntegrationTests.cs"
    },
    @{
        Label = "sync list events endpoint integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncListEventsEndpointIntegrationTests.cs"
    },
    @{
        Label = "sync tenant boundary endpoint integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncTenantBoundaryEndpointIntegrationTests.cs"
    },
    @{
        Label = "sync process endpoint API baseline"
        Path = "docs/backend/P3_SYNC_PROCESS_ENDPOINT_API_REGRESSION_BASELINE.md"
    },
    @{
        Label = "sync create batch endpoint API baseline"
        Path = "docs/backend/P3_SYNC_CREATE_BATCH_ENDPOINT_API_REGRESSION_BASELINE.md"
    },
    @{
        Label = "sync list events endpoint API baseline"
        Path = "docs/backend/P3_SYNC_LIST_EVENTS_ENDPOINT_API_REGRESSION_BASELINE.md"
    },
    @{
        Label = "sync tenant boundary endpoint API baseline"
        Path = "docs/backend/P3_SYNC_TENANT_BOUNDARY_ENDPOINT_API_REGRESSION_BASELINE.md"
    },
    @{
        Label = "zero technical debt sync processor baseline"
        Path = "docs/backend/P3_ZERO_TECHNICAL_DEBT_SYNC_PROCESSOR_BASELINE.md"
    },
    @{
        Label = "sync compatibility governance verifier"
        Path = "scripts/verify-p3-sync-compatibility-governance.ps1"
    },
    @{
        Label = "zero technical debt sync processor verifier"
        Path = "scripts/verify-p3-zero-technical-debt-sync-processor.ps1"
    },
    @{
        Label = "sync process endpoint API verifier"
        Path = "scripts/verify-p3-sync-process-endpoint-api-regression.ps1"
    },
    @{
        Label = "sync create batch endpoint API verifier"
        Path = "scripts/verify-p3-sync-create-batch-endpoint-api-regression.ps1"
    },
    @{
        Label = "sync list events endpoint API verifier"
        Path = "scripts/verify-p3-sync-list-events-endpoint-api-regression.ps1"
    },
    @{
        Label = "sync tenant boundary endpoint API verifier"
        Path = "scripts/verify-p3-sync-tenant-boundary-endpoint-api-regression.ps1"
    }
)

foreach ($Item in $RequiredFiles) {
    Assert-FileExists (Join-Path $RepoRoot $Item.Path) $Item.Label
}

$EvidenceChecks = @(
    @{
        Label = "clinical sync end-to-end test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3ClinicalSyncEndToEndIntegrationTests.cs"
        Tokens = @(
            "SyncBatchProcessor_ProcessesCompleteClinicalOfflineBatchEndToEnd",
            "SyncBatchProcessor_ProcessesOutOfOrderClinicalOfflineBatchEndToEnd",
            "SyncBatchProcessor_CompletesBatchWhenDuplicatePatientFolioCreatesConflict",
            "SyncBatchProcessor_CompletesBatchWhenInvalidPayloadIsRejected",
            "SyncBatchProcessor_ReturnsAlreadyCompletedWithoutDuplicatingClinicalRows",
            "SyncBatchProcessor_ThrowsWhenFailedBatchIsProcessed"
        )
    },
    @{
        Label = "sync process endpoint integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncProcessEndpointIntegrationTests.cs"
        Tokens = @(
            "ProcessEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
            "ProcessEndpoint_WhenAuthenticatedWithSyncWritePermission_ProcessesPendingBatch"
        )
    },
    @{
        Label = "sync create batch endpoint integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncCreateBatchEndpointIntegrationTests.cs"
        Tokens = @(
            "CreateEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
            "CreateEndpoint_WhenAuthenticatedWithSyncWritePermission_CreatesBatchAndEvents"
        )
    },
    @{
        Label = "sync list events endpoint integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncListEventsEndpointIntegrationTests.cs"
        Tokens = @(
            "ListEventsEndpoint_WhenNoAuthenticationHeaders_ReturnsUnauthorized",
            "ListEventsEndpoint_WhenAuthenticatedWithSyncReadPermission_ReturnsEventsWithoutPayloadJson",
            "ListEventsEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFound"
        )
    },
    @{
        Label = "sync tenant boundary endpoint integration test"
        Path = "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Integration/P3SyncTenantBoundaryEndpointIntegrationTests.cs"
        Tokens = @(
            "GetByIdEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFoundWithoutLeakingPayload",
            "ProcessEndpoint_WhenBatchBelongsToAnotherOrganization_ReturnsNotFoundAndDoesNotProcess"
        )
    }
)

foreach ($Check in $EvidenceChecks) {
    $Path = Join-Path $RepoRoot $Check.Path
    Assert-FileExists $Path $Check.Label

    $Content = Get-Content $Path -Raw -Encoding UTF8

    foreach ($Token in $Check.Tokens) {
        Assert-Contains $Content $Token $Check.Label
    }
}

$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"
Assert-FileExists $GovernancePath "repository governance baseline"

$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8
Assert-Contains $Governance "verify-p3-sync-backend-readiness-checklist.ps1" "repository governance baseline"

Write-Host "P3 sync backend readiness checklist verification passed." -ForegroundColor Green