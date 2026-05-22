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
    "docs/api/P3_14_OPENAPI_CONTRACT_EVIDENCE_BASELINE.md",
    "docs/api/P3_14_OPENAPI_SPEC_GOVERNANCE.md",
    "docs/api/P3_14_CLIENT_STUB_BASELINE_WEB_IOS_ANDROID.md",
    "docs/api/P3_14_CONTRACT_TESTING_BASELINE.md",
    "docs/security/P3_14_API_CONTRACT_SECURITY_EVIDENCE.md",
    "docs/runbooks/P3_14_OPENAPI_CLIENT_STUB_EVIDENCE_RUNBOOK.md",
    "docs/operations/templates/P3_14_OPENAPI_CLIENT_STUB_EVIDENCE_TEMPLATE.md",
    "scripts/verify-p3-14-openapi-client-stub-evidence-baseline.ps1"
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
    "OpenAPI",
    "API contract evidence",
    "contract version",
    "endpoint id",
    "operation id",
    "request schema",
    "response schema",
    "standard error envelope",
    "client stubs",
    "Web client stub",
    "iOS client stub",
    "Android client stub",
    "generated client boundary",
    "contract testing",
    "schema drift",
    "breaking change",
    "client compatibility matrix",
    "request id",
    "correlation id",
    "organization id",
    "idempotency key",
    "device id",
    "audit trail reference",
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
    "mobile clients may write directly to SQL Server",
    "OpenAPI evidence proves production readiness",
    "generated clients are production ready",
    "contract tests may be skipped",
    "schema drift is acceptable",
    "breaking changes without version bump are allowed",
    "patient-level exports are unrestricted",
    "production ready"
)

foreach ($token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $token
}

Write-Host "P3.14 OpenAPI and client stub evidence baseline verifier passed from repo root: $RepoRoot"