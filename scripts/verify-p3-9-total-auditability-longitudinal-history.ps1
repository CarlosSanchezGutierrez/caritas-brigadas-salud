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
    "docs/audit/P3_9_TOTAL_AUDITABILITY_BASELINE.md",
    "docs/audit/P3_9_AUDITABLE_ACTION_MATRIX.md",
    "docs/data/P3_9_LONGITUDINAL_HISTORY_BASELINE.md",
    "docs/security/P3_9_AUDIT_EVENT_SECURITY_MODEL.md",
    "docs/runbooks/P3_9_AUDIT_LONGITUDINAL_EVIDENCE_RUNBOOK.md",
    "docs/operations/templates/P3_9_AUDIT_LONGITUDINAL_EVIDENCE_TEMPLATE.md",
    "scripts/verify-p3-9-total-auditability-longitudinal-history.ps1"
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
    "total auditability",
    "audit trail",
    "actor",
    "action",
    "entity",
    "timestamp",
    "correlation id",
    "request id",
    "source ip",
    "device id",
    "organization id",
    "user role",
    "result",
    "reason",
    "before snapshot reference",
    "after snapshot reference",
    "No silent overwrite",
    "correction event",
    "patient timeline",
    "consent timeline",
    "encounter timeline",
    "clinical timeline",
    "partial identity",
    "merge and deduplication",
    "controlled data injection",
    "rejected records",
    "quarantine",
    "No secrets in repository"
)

foreach ($token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllDocumentationContent -Token $token
}

$ForbiddenTokens = @(
    "ConnectionStrings__CaritasDatabase",
    "password=",
    "User ID=sa",
    "audit bypass allowed",
    "silent overwrite allowed",
    "patient data on-chain",
    "AI clinical automation enabled",
    "Cloud is required"
)

foreach ($token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $token
}

Write-Host "P3.9 total auditability and longitudinal history verifier passed from repo root: $RepoRoot"