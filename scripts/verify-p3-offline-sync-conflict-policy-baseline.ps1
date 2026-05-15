$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$SyncPolicyPath = Join-Path $RepoRoot "docs/backend/P3_OFFLINE_SYNC_CONFLICT_POLICY_BASELINE.md"

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

Assert-FileExists $SyncPolicyPath

$SyncPolicy = Get-Content $SyncPolicyPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P3 Offline Sync and Conflict Policy Baseline",
    "Current sync model",
    "SyncBatch",
    "SyncEvent",
    "Online/offline mode policy",
    "support explicit Online mode",
    "support explicit Offline mode",
    "support Sync now action",
    "Tenant boundary rules",
    "actor OrganizationId is authoritative",
    "Device policy",
    "DeviceId strong FK policy remains deferred",
    "Idempotency policy",
    "LocalEventId must be stable across client retries for the same offline event",
    "LocalEventId idempotency scope must exist outside a single SyncBatch",
    "per-batch idempotency scope is not allowed",
    "OrganizationId + DeviceId + LocalEventId",
    "fallback scope when DeviceId is null",
    "duplicate LocalEventId submissions within the approved idempotency scope must not create duplicate clinical records",
    "Ordering policy",
    "Conflict policy",
    "Accepted, rejected, and conflict semantics",
    "Payload governance",
    "PayloadJson is sensitive and untrusted",
    "EntityType allowlist",
    "Clinical data rules",
    "Office capture and central review workflow",
    "Analytics and data engineering implications",
    "Security policy",
    "Observability policy",
    "Developer and testing policy",
    "Explicitly out of scope for P3-07",
    "Acceptance criteria"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $SyncPolicy $Token "P3 offline sync and conflict policy baseline"
}

Write-Host "P3 offline sync and conflict policy baseline verification passed." -ForegroundColor Green
