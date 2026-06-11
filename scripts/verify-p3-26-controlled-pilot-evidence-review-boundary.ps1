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
    "docs/pilot/P3_26_CONTROLLED_PILOT_EVIDENCE_COLLECTION_REVIEW_BOUNDARY.md",
    "docs/web/P3_26_WEB_PILOT_EVIDENCE_COLLECTION.md",
    "docs/mobile/P3_26_IOS_PILOT_EVIDENCE_COLLECTION.md",
    "docs/mobile/P3_26_ANDROID_PILOT_EVIDENCE_COLLECTION.md",
    "docs/operations/P3_26_PILOT_FEEDBACK_TRIAGE_AND_SUPPORT_REVIEW.md",
    "docs/security/P3_26_PILOT_PRIVACY_INCIDENT_REVIEW_BOUNDARY.md",
    "docs/qa/P3_26_PILOT_EVIDENCE_REVIEW_MATRIX.md",
    "docs/runbooks/P3_26_CONTROLLED_PILOT_EVIDENCE_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_26_CONTROLLED_PILOT_EVIDENCE_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-26-controlled-pilot-evidence-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Controlled Pilot Evidence Collection Review Boundary",
    "Web pilot evidence collection",
    "iOS pilot evidence collection",
    "Android pilot evidence collection",
    "Pilot feedback triage support review",
    "Pilot privacy incident review boundary",
    "Pilot evidence review matrix",
    "approved pilot readiness reference",
    "approved release candidate reference",
    "artifact reference",
    "deployed commit SHA",
    "environment name",
    "build profile",
    "release channel",
    "API contract version",
    "OpenAPI artifact reference",
    "pilot site or brigade scope",
    "pilot participant scope",
    "pilot device inventory",
    "UAT execution evidence",
    "workflow completion evidence",
    "field feedback evidence",
    "support ticket evidence",
    "incident evidence",
    "defect triage evidence",
    "consent workflow evidence",
    "privacy review evidence",
    "observability evidence",
    "privacy-safe telemetry evidence",
    "offline field workflow evidence",
    "sync dry run evidence",
    "sync reconciliation evidence",
    "rollback decision evidence",
    "evidence review state",
    "evidence sanitization status",
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
    "pilot evidence review is production approval",
    "controlled pilot evidence is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.26 controlled pilot evidence review boundary verifier passed from repo root: $RepoRoot"
