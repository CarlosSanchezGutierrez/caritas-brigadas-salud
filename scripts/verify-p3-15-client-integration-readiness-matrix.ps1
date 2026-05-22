$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) { $PSCommandPath } elseif ($MyInvocation.MyCommand.Path) { $MyInvocation.MyCommand.Path } else { throw "Unable to resolve script path." }
$ScriptDirectory = Split-Path -Parent $ScriptPath
$RepoRoot = Resolve-Path (Join-Path $ScriptDirectory "..")

function Resolve-RepoPath { param([string]$RelativePath) return Join-Path -Path $RepoRoot -ChildPath $RelativePath }
function Assert-FileExists { param([string]$RelativePath) $AbsolutePath = Resolve-RepoPath -RelativePath $RelativePath; if (-not (Test-Path $AbsolutePath)) { throw "Missing required file: $RelativePath resolved to $AbsolutePath" } }
function Read-RepoText { param([string]$RelativePath) $AbsolutePath = Resolve-RepoPath -RelativePath $RelativePath; if (-not (Test-Path $AbsolutePath)) { throw "Cannot read missing file: $RelativePath resolved to $AbsolutePath" }; return [System.IO.File]::ReadAllText($AbsolutePath) }
function Assert-ContainsToken { param([string]$Content, [string]$Token) if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) { throw "Missing required token: $Token" } }
function Assert-DoesNotContainToken { param([string]$Content, [string]$Token) if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) { throw "Forbidden token found: $Token" } }

$RequiredFiles = @(
    "docs/clients/P3_15_CLIENT_INTEGRATION_READINESS_MATRIX.md",
    "docs/clients/P3_15_WEB_CLIENT_READINESS_BASELINE.md",
    "docs/clients/P3_15_IOS_CLIENT_READINESS_BASELINE.md",
    "docs/clients/P3_15_ANDROID_CLIENT_READINESS_BASELINE.md",
    "docs/api/P3_15_ENDPOINT_INTEGRATION_STATUS_MATRIX.md",
    "docs/qa/P3_15_CLIENT_INTEGRATION_ACCEPTANCE_CRITERIA.md",
    "docs/runbooks/P3_15_CLIENT_INTEGRATION_READINESS_RUNBOOK.md",
    "docs/operations/templates/P3_15_CLIENT_INTEGRATION_READINESS_TEMPLATE.md",
    "scripts/verify-p3-15-client-integration-readiness-matrix.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }
$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Client integration readiness matrix",
    "Web client",
    "iOS client",
    "Android client",
    "endpoint integration status",
    "API contract version",
    "OpenAPI contract evidence",
    "client stub baseline",
    "readiness status",
    "blocked",
    "allowed",
    "requires evidence",
    "request id",
    "correlation id",
    "organization id",
    "device id",
    "idempotency key",
    "audit trail reference",
    "standard error envelope",
    "offline sync",
    "contract testing",
    "acceptance criteria",
    "No secrets in repository",
    "No direct mobile write to SQL Server",
    "No cloud dependency"
)

foreach ($Token in $RequiredTokens) { Assert-ContainsToken -Content $AllDocumentationContent -Token $Token }

$ForbiddenTokens = @(
    "ConnectionStrings__CaritasDatabase",
    "password=",
    "User ID=sa",
    "Cloud is required",
    "Azure is required",
    "AWS is required",
    "all clients are ready",
    "mobile clients may write directly to SQL Server",
    "frontend may bypass API",
    "client integration production readiness is claimed",
    "skip contract tests",
    "patient-level exports are unrestricted",
    "silent overwrite allowed",
    "repository intentionally stores secrets"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }
Write-Host "P3.15 client integration readiness matrix verifier passed from repo root: $RepoRoot"
