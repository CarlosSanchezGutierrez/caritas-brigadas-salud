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
    "docs/clients/P3_17_WEB_IOS_ANDROID_IMPLEMENTATION_WORKSTREAM_SPLIT.md",
    "docs/web/P3_17_WEB_WORKSTREAM_BACKLOG.md",
    "docs/mobile/P3_17_IOS_WORKSTREAM_BACKLOG.md",
    "docs/mobile/P3_17_ANDROID_WORKSTREAM_BACKLOG.md",
    "docs/api/P3_17_SHARED_API_CLIENT_WORKSTREAM.md",
    "docs/qa/P3_17_CROSS_CLIENT_QA_WORKSTREAM.md",
    "docs/security/P3_17_CLIENT_SECURITY_WORKSTREAM.md",
    "docs/runbooks/P3_17_WORKSTREAM_SPLIT_RUNBOOK.md",
    "docs/operations/templates/P3_17_WORKSTREAM_SPLIT_TEMPLATE.md",
    "scripts/verify-p3-17-implementation-workstream-split.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Implementation Workstream Split",
    "Web workstream",
    "iOS workstream",
    "Android workstream",
    "Shared API client workstream",
    "Cross-client QA workstream",
    "Client security workstream",
    "workstream dependency order",
    "endpoint integration status",
    "API contract version",
    "OpenAPI contract evidence",
    "client stub baseline",
    "request schema",
    "response schema",
    "standard error envelope",
    "authentication requirement",
    "authorization role",
    "organization id",
    "request id",
    "correlation id",
    "device id",
    "idempotency key",
    "offline sync",
    "audit trail reference",
    "contract test evidence",
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
    "client workstreams are production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.17 implementation workstream split verifier passed from repo root: $RepoRoot"
