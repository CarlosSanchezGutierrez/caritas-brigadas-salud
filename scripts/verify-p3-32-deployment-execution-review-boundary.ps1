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
    "docs/release/P3_32_DEPLOYMENT_EXECUTION_REVIEW_BOUNDARY.md",
    "docs/web/P3_32_WEB_DEPLOYMENT_EXECUTION_REVIEW.md",
    "docs/mobile/P3_32_IOS_DEPLOYMENT_EXECUTION_REVIEW.md",
    "docs/mobile/P3_32_ANDROID_DEPLOYMENT_EXECUTION_REVIEW.md",
    "docs/operations/P3_32_CUTOVER_EXECUTION_AND_ROLLBACK_DECISION_BOUNDARY.md",
    "docs/security/P3_32_DEPLOYMENT_EVIDENCE_SECURITY_PRIVACY_REVIEW.md",
    "docs/qa/P3_32_DEPLOYMENT_EXECUTION_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_32_DEPLOYMENT_EXECUTION_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_32_DEPLOYMENT_EXECUTION_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-32-deployment-execution-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Deployment Execution Review Boundary",
    "Web deployment execution review",
    "iOS deployment execution review",
    "Android deployment execution review",
    "Cutover execution rollback decision boundary",
    "Deployment evidence security privacy review",
    "Deployment execution review decision matrix",
    "approved deployment execution planning reference",
    "approved final go live authorization review reference",
    "approved go live planning review reference",
    "approved production readiness review execution reference",
    "approved release candidate reference",
    "deployment authorization decision evidence",
    "artifact reference",
    "deployed commit SHA",
    "environment name",
    "API contract version",
    "OpenAPI artifact reference",
    "deployment execution evidence",
    "cutover start timestamp",
    "cutover completion timestamp",
    "deployment command log evidence",
    "database backup checkpoint evidence",
    "configuration snapshot evidence",
    "release artifact integrity evidence",
    "mobile release channel execution evidence",
    "device rollout execution evidence",
    "offline queue drain evidence",
    "sync reconciliation evidence",
    "deployment owner assignment",
    "rollback owner assignment",
    "validation owner assignment",
    "support owner assignment",
    "incident commander assignment",
    "cutover command channel",
    "deployment freeze window",
    "rollback trigger criteria",
    "rollback decision evidence",
    "post deployment smoke test evidence",
    "post deployment validation evidence",
    "post deployment monitoring evidence",
    "hypercare activation evidence",
    "incident log evidence",
    "support escalation evidence",
    "go live communications evidence",
    "deployment execution review state",
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
    "deployment execution review is production steady state approval",
    "production deployment is approved"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.32 deployment execution review boundary verifier passed from repo root: $RepoRoot"
