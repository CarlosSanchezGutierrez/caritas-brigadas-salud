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
    "docs/release/P3_42_POST_TRANSITION_MONITORING_REVIEW_BOUNDARY.md",
    "docs/web/P3_42_WEB_POST_TRANSITION_MONITORING_REVIEW.md",
    "docs/mobile/P3_42_IOS_POST_TRANSITION_MONITORING_REVIEW.md",
    "docs/mobile/P3_42_ANDROID_POST_TRANSITION_MONITORING_REVIEW.md",
    "docs/operations/P3_42_POST_TRANSITION_MONITORING_OPERATIONS_BOUNDARY.md",
    "docs/security/P3_42_POST_TRANSITION_SECURITY_PRIVACY_DATA_MONITORING_BOUNDARY.md",
    "docs/qa/P3_42_POST_TRANSITION_MONITORING_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_42_POST_TRANSITION_MONITORING_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_42_POST_TRANSITION_MONITORING_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-42-post-transition-monitoring-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) {
    $AllDocumentationContent += "`n--- FILE: $File ---`n"
    $AllDocumentationContent += Read-RepoText -RelativePath $File
}

$ApprovalTokens = @(
    "approved controlled readiness status transition execution review reference",
    "approved readiness status transition review reference",
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
    "approved release candidate reference"
)

$IdentityTokens = @(
    "environment name",
    "deployed commit SHA",
    "artifact reference",
    "API contract version",
    "OpenAPI artifact reference"
)

$CommonPostTransitionTokens = @(
    "post transition monitoring package evidence",
    "post transition monitoring window",
    "pre transition readiness status evidence",
    "target readiness status evidence",
    "observed readiness status evidence",
    "post transition availability evidence",
    "post transition latency evidence",
    "post transition API error rate evidence",
    "post transition database health evidence",
    "post transition SQL Server connectivity evidence",
    "post transition audit trail health evidence",
    "post transition security monitoring evidence",
    "post transition privacy monitoring evidence",
    "post transition data governance monitoring evidence",
    "post transition incident review evidence",
    "post transition defect review evidence",
    "post transition support review evidence",
    "post transition rollback posture evidence",
    "post transition rollback decision evidence",
    "post transition stakeholder communication evidence",
    "post transition hypercare continuation evidence",
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
    "post transition monitoring decision evidence",
    "post transition monitoring blockers",
    "post transition monitoring review state"
)

$MobilePostTransitionTokens = @(
    "mobile release channel post transition monitoring evidence",
    "device fleet post transition monitoring evidence",
    "offline sync post transition monitoring evidence",
    "conflict resolution post transition monitoring evidence"
)

$MetadataTokens = @(
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

$MobileMetadataTokens = @(
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id"
)

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Post Transition Monitoring Review Boundary",
    "Web post transition monitoring review",
    "iOS post transition monitoring review",
    "Android post transition monitoring review",
    "Post transition monitoring operations boundary",
    "Post transition security privacy data monitoring boundary",
    "Post transition monitoring review decision matrix",
    "No secrets in repository",
    "No direct mobile write to SQL Server",
    "No cloud dependency"
) + $ApprovalTokens + $IdentityTokens + $CommonPostTransitionTokens + $MobilePostTransitionTokens + $MetadataTokens + $MobileMetadataTokens

foreach ($Token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllDocumentationContent -Token $Token
}

$P342CommonClientRequiredTokens = $ApprovalTokens + $IdentityTokens + $CommonPostTransitionTokens + $MetadataTokens

$P342ClientFiles = @(
    "docs/web/P3_42_WEB_POST_TRANSITION_MONITORING_REVIEW.md",
    "docs/mobile/P3_42_IOS_POST_TRANSITION_MONITORING_REVIEW.md",
    "docs/mobile/P3_42_ANDROID_POST_TRANSITION_MONITORING_REVIEW.md"
)

foreach ($ClientFile in $P342ClientFiles) {
    $ClientContent = Read-RepoText -RelativePath $ClientFile
    foreach ($Token in $P342CommonClientRequiredTokens) {
        Assert-ContainsToken -Content $ClientContent -Token $Token
    }
}

$P342MobileRequiredTokens = $MobilePostTransitionTokens + $MobileMetadataTokens

foreach ($MobileFile in @("docs/mobile/P3_42_IOS_POST_TRANSITION_MONITORING_REVIEW.md", "docs/mobile/P3_42_ANDROID_POST_TRANSITION_MONITORING_REVIEW.md")) {
    $MobileContent = Read-RepoText -RelativePath $MobileFile
    foreach ($Token in $P342MobileRequiredTokens) {
        Assert-ContainsToken -Content $MobileContent -Token $Token
    }
}

$P342FileSpecificTokens = @{
    "docs/release/P3_42_POST_TRANSITION_MONITORING_REVIEW_BOUNDARY.md" = @(
        "Post Transition Monitoring Review Boundary",
        "approved controlled readiness status transition execution review reference",
        "post transition monitoring package evidence",
        "post transition monitoring window",
        "observed readiness status evidence",
        "post transition SQL Server connectivity evidence",
        "post transition rollback decision evidence",
        "mobile release channel post transition monitoring evidence when applicable",
        "post transition monitoring review state"
    );
    "docs/operations/P3_42_POST_TRANSITION_MONITORING_OPERATIONS_BOUNDARY.md" = @(
        "Post transition monitoring operations boundary",
        "post transition monitoring package evidence",
        "post transition availability evidence",
        "post transition latency evidence",
        "post transition API error rate evidence",
        "post transition database health evidence",
        "post transition SQL Server connectivity evidence",
        "post transition rollback decision evidence",
        "change management acceptance evidence",
        "release management acceptance evidence",
        "post transition monitoring review state"
    );
    "docs/security/P3_42_POST_TRANSITION_SECURITY_PRIVACY_DATA_MONITORING_BOUNDARY.md" = @(
        "Post transition security privacy data monitoring boundary",
        "post transition audit trail health evidence",
        "post transition security monitoring evidence",
        "post transition privacy monitoring evidence",
        "post transition data governance monitoring evidence",
        "evidence completeness evidence",
        "evidence traceability evidence",
        "SQL Server operational source of truth confirmation",
        "mobile release channel post transition monitoring evidence"
    );
    "docs/qa/P3_42_POST_TRANSITION_MONITORING_REVIEW_DECISION_MATRIX.md" = @(
        "Post transition monitoring review decision matrix",
        "approved controlled readiness status transition execution review reference",
        "post transition monitoring package evidence",
        "post transition monitoring window",
        "post transition SQL Server connectivity evidence",
        "mobile release channel post transition monitoring evidence",
        "post transition monitoring blockers"
    );
    "docs/runbooks/P3_42_POST_TRANSITION_MONITORING_REVIEW_RUNBOOK.md" = @(
        "Post Transition Monitoring Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "post transition monitoring review state",
        "change management acceptance evidence is required",
        "release management acceptance evidence is required",
        "mobile release channel post transition monitoring evidence is required"
    );
    "docs/operations/templates/P3_42_POST_TRANSITION_MONITORING_REVIEW_TEMPLATE.md" = @(
        "Post Transition Monitoring Review Template",
        "approved controlled readiness status transition execution review reference",
        "approved release candidate reference is required",
        "post transition monitoring package evidence",
        "post transition monitoring window",
        "post transition SQL Server connectivity evidence",
        "mobile release channel post transition monitoring evidence is required",
        "change management acceptance evidence is required",
        "release management acceptance evidence is required",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "post transition monitoring review state"
    )
}

foreach ($Entry in $P342FileSpecificTokens.GetEnumerator()) {
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
    "post transition monitoring review is final backend production readiness closure"
)

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token
}

Write-Host "P3.42 post transition monitoring review boundary verifier passed from repo root: $RepoRoot"