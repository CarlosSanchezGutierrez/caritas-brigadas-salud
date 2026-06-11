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
    "docs/release/P3_28_PRODUCTION_READINESS_REVIEW_EXECUTION_BOUNDARY.md",
    "docs/web/P3_28_WEB_PRODUCTION_READINESS_REVIEW_EXECUTION.md",
    "docs/mobile/P3_28_IOS_PRODUCTION_READINESS_REVIEW_EXECUTION.md",
    "docs/mobile/P3_28_ANDROID_PRODUCTION_READINESS_REVIEW_EXECUTION.md",
    "docs/operations/P3_28_OPERATIONAL_REVIEW_EXECUTION_AND_RISK_DECISION.md",
    "docs/security/P3_28_SECURITY_PRIVACY_DATA_REVIEW_EXECUTION.md",
    "docs/qa/P3_28_PRODUCTION_READINESS_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_28_PRODUCTION_READINESS_REVIEW_EXECUTION_RUNBOOK.md",
    "docs/operations/templates/P3_28_PRODUCTION_READINESS_REVIEW_EXECUTION_TEMPLATE.md",
    "scripts/verify-p3-28-production-readiness-review-execution-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Production Readiness Review Execution Boundary",
    "Web production readiness review execution",
    "iOS production readiness review execution",
    "Android production readiness review execution",
    "Operational review execution and risk decision",
    "Security privacy data review execution",
    "Production readiness review decision matrix",
    "approved production readiness review entry reference",
    "approved pilot evidence review reference",
    "approved release candidate reference",
    "artifact reference",
    "deployed commit SHA",
    "environment name",
    "API contract version",
    "OpenAPI artifact reference",
    "operational review evidence",
    "support review evidence",
    "security review evidence",
    "privacy review evidence",
    "data governance review evidence",
    "backup and recovery review evidence",
    "rollback rehearsal evidence",
    "incident response rehearsal evidence",
    "monitoring review evidence",
    "alerting review evidence",
    "defect closure evidence",
    "known limitations review",
    "risk acceptance evidence",
    "go live readiness blockers",
    "production readiness decision evidence",
    "production readiness review execution state",
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
    "production readiness review execution is production approval",
    "production deployment is approved"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.28 production readiness review execution boundary verifier passed from repo root: $RepoRoot"
