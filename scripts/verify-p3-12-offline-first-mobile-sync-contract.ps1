$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) {
    $PSCommandPath
}
elseif ($MyInvocation.MyCommand.Path) {
    $MyInvocation.MyCommand.Path
}
else {
    throw "Unable to resolve script path."
}

$ScriptDirectory = Split-Path -Parent $ScriptPath
$RepoRoot = Resolve-Path (Join-Path $ScriptDirectory "..")

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    return Join-Path -Path $RepoRoot -ChildPath $RelativePath
}

function Assert-FileExists {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    $absolutePath = Resolve-RepoPath -RelativePath $RelativePath

    if (-not (Test-Path $absolutePath)) {
        throw "Missing required file: $RelativePath resolved to $absolutePath"
    }
}

function Read-RepoText {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    $absolutePath = Resolve-RepoPath -RelativePath $RelativePath

    if (-not (Test-Path $absolutePath)) {
        throw "Cannot read missing file: $RelativePath resolved to $absolutePath"
    }

    return [System.IO.File]::ReadAllText($absolutePath)
}

function Assert-ContainsToken {
    param(
        [Parameter(Mandatory = $true)][string]$Content,
        [Parameter(Mandatory = $true)][string]$Token
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "Missing required token: $Token"
    }
}

function Assert-DoesNotContainToken {
    param(
        [Parameter(Mandatory = $true)][string]$Content,
        [Parameter(Mandatory = $true)][string]$Token
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "Forbidden token found: $Token"
    }
}

$RequiredFiles = @(
    "docs/mobile/P3_12_OFFLINE_FIRST_MOBILE_SYNC_OPERATIONAL_CONTRACT.md",
    "docs/mobile/P3_12_OFFLINE_QUEUE_AND_OUTBOX_MODEL.md",
    "docs/mobile/P3_12_SYNC_CONFLICT_RESOLUTION_MODEL.md",
    "docs/security/P3_12_MOBILE_SYNC_SECURITY_PRIVACY_MODEL.md",
    "docs/runbooks/P3_12_OFFLINE_SYNC_EVIDENCE_RUNBOOK.md",
    "docs/operations/templates/P3_12_OFFLINE_SYNC_EVIDENCE_TEMPLATE.md",
    "scripts/verify-p3-12-offline-first-mobile-sync-contract.ps1"
)

foreach ($file in $RequiredFiles) {
    Assert-FileExists -RelativePath $file
}

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""

foreach ($file in $DocumentationFiles) {
    $AllDocumentationContent += "`n--- FILE: $file ---`n"
    $AllDocumentationContent += Read-RepoText -RelativePath $file
}

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "offline-first",
    "offline capture",
    "local draft",
    "local outbox",
    "retry queue",
    "idempotency key",
    "sync attempt",
    "sync status",
    "conflict detection",
    "conflict resolution",
    "server acknowledgment",
    "device id",
    "organization id",
    "user role",
    "correlation id",
    "request id",
    "audit trail reference",
    "server validation",
    "No silent overwrite",
    "correction event",
    "rejected records",
    "quarantine",
    "No secrets in repository",
    "No cloud dependency",
    "No direct mobile write to SQL Server"
)

foreach ($token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllDocumentationContent -Token $token
}

$ForbiddenTokens = @(
    "ConnectionStrings__CaritasDatabase",
    "password=",
    "User ID=sa",
    "Cloud is required",
    "Azure is required",
    "AWS is required",
    "direct mobile write to SQL Server is allowed",
    "offline sync can bypass audit",
    "silent overwrite allowed",
    "patient data on-chain",
    "AI clinical automation enabled",
    "production ready"
)

foreach ($token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $token
}

Write-Host "P3.12 offline-first mobile sync operational contract verifier passed from repo root: $RepoRoot"