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
    "docs/release/P3_40_READINESS_STATUS_TRANSITION_REVIEW_BOUNDARY.md",
    "docs/web/P3_40_WEB_READINESS_STATUS_TRANSITION_INPUT_REVIEW.md",
    "docs/mobile/P3_40_IOS_READINESS_STATUS_TRANSITION_INPUT_REVIEW.md",
    "docs/mobile/P3_40_ANDROID_READINESS_STATUS_TRANSITION_INPUT_REVIEW.md",
    "docs/operations/P3_40_READINESS_STATUS_TRANSITION_OPERATIONS_CONTROL_BOUNDARY.md",
    "docs/security/P3_40_READINESS_STATUS_TRANSITION_SECURITY_PRIVACY_DATA_CONTROL_BOUNDARY.md",
    "docs/qa/P3_40_READINESS_STATUS_TRANSITION_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_40_READINESS_STATUS_TRANSITION_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_40_READINESS_STATUS_TRANSITION_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-40-readiness-status-transition-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Readiness Status Transition Review Boundary",
    "Web readiness status transition input review",
    "iOS readiness status transition input review",
    "Android readiness status transition input review",
    "Readiness status transition operations control boundary",
    "Readiness status transition security privacy data control boundary",
    "Readiness status transition review decision matrix",
    "approved institutional signoff review reference",
    "approved backend production readiness decision review reference",
    "approved production evidence closure review reference",
    "approved steady state readiness review reference",
    "approved operational handover review reference",
    "approved stabilization review reference",
    "approved hypercare monitoring review reference",
    "approved deployment execution review reference",
    "approved deployment execution planning reference",
    "approved final go live authorization review reference",
    "approved go live planning review reference",
    "approved production readiness review execution reference",
    "approved release candidate reference",
    "environment name",
    "deployed commit SHA",
    "artifact reference",
    "API contract version",
    "OpenAPI artifact reference",
    "readiness status transition package evidence",
    "current readiness status evidence",
    "target readiness status evidence",
    "readiness status transition authority evidence",
    "readiness status transition criteria evidence",
    "readiness status transition record evidence",
    "readiness status transition state",
    "status transition owner assignment",
    "executive sponsor transition authorization evidence",
    "technical owner transition authorization evidence",
    "operations owner transition authorization evidence",
    "support owner transition authorization evidence",
    "security owner transition authorization evidence",
    "privacy owner transition authorization evidence",
    "data owner transition authorization evidence",
    "risk owner transition authorization evidence",
    "compliance owner transition authorization evidence",
    "institutional acceptance decision evidence",
    "final risk acceptance evidence",
    "final blocker disposition evidence",
    "exception register acceptance evidence",
    "transition rollback criteria evidence",
    "transition rollback owner evidence",
    "transition communication evidence",
    "transition audit trail evidence",
    "transition monitoring evidence",
    "post transition validation plan evidence",
    "production monitoring acceptance evidence",
    "production support acceptance evidence",
    "API operational acceptance evidence",
    "OpenAPI contract acceptance evidence",
    "SQL Server operational acceptance evidence",
    "database operational acceptance evidence",
    "backup recovery acceptance evidence",
    "incident response acceptance evidence",
    "change management acceptance evidence",
    "release management acceptance evidence",
    "access control acceptance evidence",
    "audit trail acceptance evidence",
    "data governance acceptance evidence",
    "security acceptance evidence",
    "privacy acceptance evidence",
    "residual risk acceptance evidence",
    "evidence inventory evidence",
    "evidence completeness evidence",
    "evidence traceability evidence",
    "evidence sanitization evidence",
    "mobile release channel transition evidence",
    "device fleet transition evidence",
    "offline sync transition evidence",
    "conflict resolution transition evidence",
    "readiness status transition blockers",
    "request id",
    "correlation id",
    "organization id",
    "authorization role",
    "endpoint id",
    "standard error envelope",
    "audit trail reference",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id",
    "support diagnostic evidence",
    "monitoring evidence",
    "alerting evidence",
    "evidence sanitization status",
    "No secrets in repository",
    "No direct mobile write to SQL Server",
    "No cloud dependency"
)

foreach ($Token in $RequiredTokens) { Assert-ContainsToken -Content $AllDocumentationContent -Token $Token }

$P340CommonClientRequiredTokens = @(
    "approved institutional signoff review reference",
    "approved backend production readiness decision review reference",
    "approved production evidence closure review reference",
    "approved steady state readiness review reference",
    "approved operational handover review reference",
    "approved stabilization review reference",
    "approved hypercare monitoring review reference",
    "approved deployment execution review reference",
    "approved deployment execution planning reference",
    "approved final go live authorization review reference",
    "approved go live planning review reference",
    "approved production readiness review execution reference",
    "approved release candidate reference",
    "environment name",
    "deployed commit SHA",
    "artifact reference",
    "API contract version",
    "OpenAPI artifact reference",
    "readiness status transition package evidence",
    "current readiness status evidence",
    "target readiness status evidence",
    "readiness status transition authority evidence",
    "readiness status transition criteria evidence",
    "readiness status transition record evidence",
    "readiness status transition state",
    "status transition owner assignment",
    "executive sponsor transition authorization evidence",
    "technical owner transition authorization evidence",
    "operations owner transition authorization evidence",
    "support owner transition authorization evidence",
    "security owner transition authorization evidence",
    "privacy owner transition authorization evidence",
    "data owner transition authorization evidence",
    "risk owner transition authorization evidence",
    "compliance owner transition authorization evidence",
    "institutional acceptance decision evidence",
    "final risk acceptance evidence",
    "final blocker disposition evidence",
    "exception register acceptance evidence",
    "transition rollback criteria evidence",
    "transition rollback owner evidence",
    "transition communication evidence",
    "transition audit trail evidence",
    "transition monitoring evidence",
    "post transition validation plan evidence",
    "production monitoring acceptance evidence",
    "production support acceptance evidence",
    "API operational acceptance evidence",
    "OpenAPI contract acceptance evidence",
    "SQL Server operational acceptance evidence",
    "database operational acceptance evidence",
    "backup recovery acceptance evidence",
    "incident response acceptance evidence",
    "change management acceptance evidence",
    "release management acceptance evidence",
    "access control acceptance evidence",
    "audit trail acceptance evidence",
    "data governance acceptance evidence",
    "security acceptance evidence",
    "privacy acceptance evidence",
    "residual risk acceptance evidence",
    "evidence inventory evidence",
    "evidence completeness evidence",
    "evidence traceability evidence",
    "evidence sanitization evidence",
    "readiness status transition blockers",
    "request id",
    "correlation id",
    "organization id",
    "authorization role",
    "endpoint id",
    "standard error envelope",
    "audit trail reference",
    "support diagnostic evidence",
    "monitoring evidence",
    "alerting evidence",
    "evidence sanitization status"
)

$P340ClientFiles = @(
    "docs/web/P3_40_WEB_READINESS_STATUS_TRANSITION_INPUT_REVIEW.md",
    "docs/mobile/P3_40_IOS_READINESS_STATUS_TRANSITION_INPUT_REVIEW.md",
    "docs/mobile/P3_40_ANDROID_READINESS_STATUS_TRANSITION_INPUT_REVIEW.md"
)

foreach ($ClientFile in $P340ClientFiles) {
    $ClientContent = Read-RepoText -RelativePath $ClientFile
    foreach ($Token in $P340CommonClientRequiredTokens) {
        Assert-ContainsToken -Content $ClientContent -Token $Token
    }
}

$P340MobileRequiredTokens = @(
    "mobile release channel transition evidence",
    "device fleet transition evidence",
    "offline sync transition evidence",
    "conflict resolution transition evidence",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id"
)

foreach ($MobileFile in @("docs/mobile/P3_40_IOS_READINESS_STATUS_TRANSITION_INPUT_REVIEW.md", "docs/mobile/P3_40_ANDROID_READINESS_STATUS_TRANSITION_INPUT_REVIEW.md")) {
    $MobileContent = Read-RepoText -RelativePath $MobileFile
    foreach ($Token in $P340MobileRequiredTokens) {
        Assert-ContainsToken -Content $MobileContent -Token $Token
    }
}

$P340FileSpecificTokens = @{
    "docs/release/P3_40_READINESS_STATUS_TRANSITION_REVIEW_BOUNDARY.md" = @(
        "Readiness Status Transition Review Boundary",
        "approved institutional signoff review reference",
        "current readiness status evidence",
        "target readiness status evidence",
        "readiness status transition authority evidence",
        "transition rollback criteria evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel transition evidence when applicable",
        "readiness status transition state"
    );
    "docs/operations/P3_40_READINESS_STATUS_TRANSITION_OPERATIONS_CONTROL_BOUNDARY.md" = @(
        "Readiness status transition operations control boundary",
        "current readiness status evidence",
        "target readiness status evidence",
        "readiness status transition authority evidence",
        "transition rollback criteria evidence",
        "post transition validation plan evidence",
        "change management acceptance evidence",
        "release management acceptance evidence",
        "SQL Server operational acceptance evidence",
        "readiness status transition state"
    );
    "docs/security/P3_40_READINESS_STATUS_TRANSITION_SECURITY_PRIVACY_DATA_CONTROL_BOUNDARY.md" = @(
        "Readiness status transition security privacy data control boundary",
        "security owner transition authorization evidence",
        "privacy owner transition authorization evidence",
        "data owner transition authorization evidence",
        "compliance owner transition authorization evidence",
        "transition audit trail evidence",
        "SQL Server operational source of truth confirmation",
        "mobile release channel transition evidence"
    );
    "docs/qa/P3_40_READINESS_STATUS_TRANSITION_REVIEW_DECISION_MATRIX.md" = @(
        "Readiness status transition review decision matrix",
        "approved institutional signoff review reference",
        "current readiness status evidence",
        "target readiness status evidence",
        "readiness status transition authority evidence",
        "transition rollback criteria evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel transition evidence",
        "readiness status transition blockers"
    );
    "docs/runbooks/P3_40_READINESS_STATUS_TRANSITION_REVIEW_RUNBOOK.md" = @(
        "Readiness Status Transition Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "readiness status transition state",
        "change management acceptance evidence is required",
        "release management acceptance evidence is required"
    );
    "docs/operations/templates/P3_40_READINESS_STATUS_TRANSITION_REVIEW_TEMPLATE.md" = @(
        "Readiness Status Transition Review Template",
        "Approved institutional signoff review reference",
        "approved release candidate reference is required",
        "readiness status transition package evidence",
        "current readiness status evidence",
        "target readiness status evidence",
        "readiness status transition authority evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel transition evidence is required for mobile",
        "change management acceptance evidence is required",
        "release management acceptance evidence is required",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "readiness status transition state"
    )
}

foreach ($Entry in $P340FileSpecificTokens.GetEnumerator()) {
    $FileContent = Read-RepoText -RelativePath $Entry.Key
    foreach ($Token in $Entry.Value) {
        Assert-ContainsToken -Content $FileContent -Token $Token
    }
}

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
    "backend production readiness is approved",
    "backend is production ready",
    "status transition review is status update execution"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.40 readiness status transition review boundary verifier passed from repo root: $RepoRoot"
