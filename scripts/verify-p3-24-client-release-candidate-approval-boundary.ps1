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
    "docs/release/P3_24_CLIENT_RELEASE_CANDIDATE_APPROVAL_BOUNDARY.md",
    "docs/web/P3_24_WEB_RELEASE_CANDIDATE_BOUNDARY.md",
    "docs/mobile/P3_24_IOS_RELEASE_CANDIDATE_BOUNDARY.md",
    "docs/mobile/P3_24_ANDROID_RELEASE_CANDIDATE_BOUNDARY.md",
    "docs/security/P3_24_RELEASE_SECURITY_PRIVACY_APPROVAL_BOUNDARY.md",
    "docs/qa/P3_24_RELEASE_CANDIDATE_ACCEPTANCE_MATRIX.md",
    "docs/runbooks/P3_24_RELEASE_CANDIDATE_APPROVAL_RUNBOOK.md",
    "docs/operations/templates/P3_24_RELEASE_CANDIDATE_APPROVAL_TEMPLATE.md",
    "scripts/verify-p3-24-client-release-candidate-approval-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Client Release Candidate Approval Boundary",
    "Web release candidate boundary",
    "iOS release candidate boundary",
    "Android release candidate boundary",
    "Release security privacy approval boundary",
    "Release candidate acceptance matrix",
    "artifact reference",
    "deployed commit SHA",
    "environment name",
    "build profile",
    "release channel",
    "API contract version",
    "OpenAPI artifact reference",
    "dependency review evidence",
    "secret scan evidence",
    "static analysis evidence",
    "build reproducibility evidence",
    "unit test evidence",
    "contract test evidence",
    "runtime configuration test evidence",
    "observability test evidence",
    "privacy-safe telemetry test evidence",
    "schema drift evidence",
    "breaking change evidence",
    "artifact retention evidence",
    "signing boundary evidence",
    "release notes evidence",
    "rollback plan",
    "support diagnostic evidence",
    "approval state",
    "request id",
    "correlation id",
    "organization id",
    "audit trail reference",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id",
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
    "contract tests may be skipped",
    "release candidate is production approval",
    "client release is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.24 client release candidate approval boundary verifier passed from repo root: $RepoRoot"
