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
    "docs/api/P3_13_API_CONTRACT_FREEZE_BASELINE.md",
    "docs/api/P3_13_ENDPOINT_CATALOG.md",
    "docs/api/P3_13_REQUEST_RESPONSE_ERROR_CONVENTIONS.md",
    "docs/api/P3_13_CLIENT_COMPATIBILITY_CONTRACT_WEB_IOS_ANDROID.md",
    "docs/api/P3_13_SYNC_AND_IDEMPOTENCY_API_CONTRACT.md",
    "docs/security/P3_13_API_SECURITY_AUTHORIZATION_CONTRACT.md",
    "docs/runbooks/P3_13_API_CONTRACT_FREEZE_EVIDENCE_RUNBOOK.md",
    "docs/operations/templates/P3_13_API_CONTRACT_FREEZE_EVIDENCE_TEMPLATE.md",
    "scripts/verify-p3-13-api-contract-freeze.ps1"
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
    "API contract freeze",
    "Web/iOS/Android",
    "endpoint catalog",
    "request schema",
    "response schema",
    "standard error envelope",
    "authentication requirement",
    "authorization role",
    "organization id requirement",
    "API version",
    "idempotency key",
    "offline sync",
    "device id",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "audit trail reference",
    "correlation id",
    "request id",
    "pagination convention",
    "filtering convention",
    "sorting convention",
    "breaking change policy",
    "Web compatibility",
    "iOS compatibility",
    "Android compatibility",
    "No secrets in repository",
    "No cloud dependency",
    "No direct mobile write to SQL Server",
    "No silent overwrite"
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
    "unauthenticated API access is allowed",
    "audit bypass allowed",
    "silent overwrite allowed",
    "unrestricted patient-level exports are allowed",
    "offline sync can bypass audit is allowed",
    "production ready"
)

foreach ($token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $token
}

Write-Host "P3.13 API contract freeze verifier passed from repo root: $RepoRoot"