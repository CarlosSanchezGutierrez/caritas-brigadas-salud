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
    "docs/release/P3_30_FINAL_GO_LIVE_AUTHORIZATION_REVIEW_BOUNDARY.md",
    "docs/web/P3_30_WEB_FINAL_GO_LIVE_AUTHORIZATION_REVIEW.md",
    "docs/mobile/P3_30_IOS_FINAL_GO_LIVE_AUTHORIZATION_REVIEW.md",
    "docs/mobile/P3_30_ANDROID_FINAL_GO_LIVE_AUTHORIZATION_REVIEW.md",
    "docs/operations/P3_30_FINAL_AUTHORIZATION_OWNERSHIP_AND_APPROVAL_BOUNDARY.md",
    "docs/security/P3_30_FINAL_SECURITY_PRIVACY_DATA_AUTHORIZATION_BOUNDARY.md",
    "docs/qa/P3_30_FINAL_GO_LIVE_AUTHORIZATION_DECISION_MATRIX.md",
    "docs/runbooks/P3_30_FINAL_GO_LIVE_AUTHORIZATION_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_30_FINAL_GO_LIVE_AUTHORIZATION_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-30-final-go-live-authorization-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Final Go Live Authorization Review Boundary",
    "Web final go live authorization review",
    "iOS final go live authorization review",
    "Android final go live authorization review",
    "Final authorization ownership approval boundary",
    "Final security privacy data authorization boundary",
    "Final go live authorization decision matrix",
    "approved go live planning review reference",
    "approved production readiness review execution reference",
    "approved production readiness review entry reference",
    "approved pilot evidence review reference",
    "approved release candidate reference",
    "production readiness decision evidence",
    "final go live decision evidence",
    "deployment authorization decision evidence",
    "artifact reference",
    "deployed commit SHA",
    "environment name",
    "API contract version",
    "OpenAPI artifact reference",
    "final deployment window confirmation",
    "final cutover plan confirmation",
    "final rollback checkpoint confirmation",
    "final backup checkpoint confirmation",
    "incident command readiness confirmation",
    "support staffing confirmation",
    "hypercare readiness confirmation",
    "communication readiness confirmation",
    "stakeholder notification approval evidence",
    "mobile release channel authorization",
    "device rollout authorization",
    "offline queue drain authorization",
    "sync reconciliation authorization",
    "final operational authorization evidence",
    "final security authorization evidence",
    "final privacy authorization evidence",
    "final data owner authorization evidence",
    "final risk acceptance evidence",
    "final blocker review evidence",
    "final go live authorization review state",
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
    "evidence sanitization status",
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
    "final go live authorization review is deployment execution",
    "production deployment is approved"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.30 final go live authorization review boundary verifier passed from repo root: $RepoRoot"
