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
    "docs/api/P3_18_SHARED_API_CLIENT_SCAFFOLD_GOVERNANCE.md",
    "docs/web/P3_18_WEB_API_CLIENT_SCAFFOLD.md",
    "docs/mobile/P3_18_IOS_API_CLIENT_SCAFFOLD.md",
    "docs/mobile/P3_18_ANDROID_API_CLIENT_SCAFFOLD.md",
    "docs/qa/P3_18_API_CLIENT_CONTRACT_TEST_SCAFFOLD.md",
    "docs/security/P3_18_API_CLIENT_SECURITY_SCAFFOLD.md",
    "docs/runbooks/P3_18_SHARED_API_CLIENT_SCAFFOLD_RUNBOOK.md",
    "docs/operations/templates/P3_18_SHARED_API_CLIENT_SCAFFOLD_TEMPLATE.md",
    "scripts/verify-p3-18-shared-api-client-scaffold-governance.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Shared API Client Scaffold Governance",
    "Web API client scaffold",
    "iOS API client scaffold",
    "Android API client scaffold",
    "API client contract test scaffold",
    "API client security scaffold",
    "configuration boundary",
    "transport boundary",
    "metadata boundary",
    "auth boundary",
    "schema boundary",
    "error boundary",
    "offline boundary",
    "audit boundary",
    "test boundary",
    "API contract version",
    "endpoint id",
    "typed request model",
    "typed response model",
    "standard error envelope",
    "authentication requirement",
    "authorization role",
    "organization id",
    "request id",
    "correlation id",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "audit trail reference",
    "contract test evidence",
    "schema drift evidence",
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
    "mobile clients may write directly to SQL Server",
    "frontend may bypass API",
    "repository intentionally stores secrets",
    "real patient data is committed intentionally",
    "mocked data is production evidence",
    "undocumented endpoints are allowed",
    "conflicts may be silently overwritten",
    "shared API client scaffold is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.18 shared API client scaffold governance verifier passed from repo root: $RepoRoot"
