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
    "docs/release/P3_41_CONTROLLED_READINESS_STATUS_TRANSITION_EXECUTION_REVIEW_BOUNDARY.md",
    "docs/web/P3_41_WEB_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW.md",
    "docs/mobile/P3_41_IOS_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW.md",
    "docs/mobile/P3_41_ANDROID_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW.md",
    "docs/operations/P3_41_STATUS_TRANSITION_EXECUTION_OPERATIONS_CONTROL_BOUNDARY.md",
    "docs/security/P3_41_STATUS_TRANSITION_EXECUTION_SECURITY_PRIVACY_DATA_CONTROL_BOUNDARY.md",
    "docs/qa/P3_41_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_41_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_41_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-41-controlled-readiness-status-transition-execution-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Controlled Readiness Status Transition Execution Review Boundary",
    "Web controlled status transition execution review",
    "iOS controlled status transition execution review",
    "Android controlled status transition execution review",
    "Status transition execution operations control boundary",
    "Status transition execution security privacy data control boundary",
    "Controlled status transition execution review decision matrix",
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
    "approved release candidate reference",
    "environment name",
    "deployed commit SHA",
    "artifact reference",
    "API contract version",
    "OpenAPI artifact reference",
    "controlled transition execution package evidence",
    "pre transition readiness status evidence",
    "target readiness status evidence",
    "observed readiness status evidence",
    "status transition execution authority evidence",
    "status transition execution criteria evidence",
    "status transition execution record evidence",
    "status transition execution state",
    "status transition owner assignment",
    "transition execution start timestamp",
    "transition execution completion timestamp",
    "transition execution command evidence",
    "transition execution audit trail evidence",
    "transition execution monitoring evidence",
    "post transition validation evidence",
    "post transition smoke test evidence",
    "rollback criteria evaluation evidence",
    "post transition rollback decision evidence",
    "rollback execution readiness evidence",
    "rollback owner evidence",
    "transition communication execution evidence",
    "stakeholder notification evidence",
    "support readiness confirmation evidence",
    "incident command readiness evidence",
    "hypercare continuation evidence",
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
    "mobile release channel execution evidence",
    "device fleet execution evidence",
    "offline sync execution evidence",
    "conflict resolution execution evidence",
    "controlled transition execution blockers",
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

$P341CommonClientRequiredTokens = @(
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
    "approved release candidate reference",
    "environment name",
    "deployed commit SHA",
    "artifact reference",
    "API contract version",
    "OpenAPI artifact reference",
    "controlled transition execution package evidence",
    "pre transition readiness status evidence",
    "target readiness status evidence",
    "observed readiness status evidence",
    "status transition execution authority evidence",
    "status transition execution criteria evidence",
    "status transition execution record evidence",
    "status transition execution state",
    "status transition owner assignment",
    "transition execution start timestamp",
    "transition execution completion timestamp",
    "transition execution command evidence",
    "transition execution audit trail evidence",
    "transition execution monitoring evidence",
    "post transition validation evidence",
    "post transition smoke test evidence",
    "rollback criteria evaluation evidence",
    "post transition rollback decision evidence",
    "rollback execution readiness evidence",
    "rollback owner evidence",
    "transition communication execution evidence",
    "stakeholder notification evidence",
    "support readiness confirmation evidence",
    "incident command readiness evidence",
    "hypercare continuation evidence",
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
    "controlled transition execution blockers",
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

$P341ClientFiles = @(
    "docs/web/P3_41_WEB_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW.md",
    "docs/mobile/P3_41_IOS_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW.md",
    "docs/mobile/P3_41_ANDROID_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW.md"
)

foreach ($ClientFile in $P341ClientFiles) {
    $ClientContent = Read-RepoText -RelativePath $ClientFile
    foreach ($Token in $P341CommonClientRequiredTokens) {
        Assert-ContainsToken -Content $ClientContent -Token $Token
    }
}

$P341MobileRequiredTokens = @(
    "mobile release channel execution evidence",
    "device fleet execution evidence",
    "offline sync execution evidence",
    "conflict resolution execution evidence",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id"
)

foreach ($MobileFile in @("docs/mobile/P3_41_IOS_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW.md", "docs/mobile/P3_41_ANDROID_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW.md")) {
    $MobileContent = Read-RepoText -RelativePath $MobileFile
    foreach ($Token in $P341MobileRequiredTokens) {
        Assert-ContainsToken -Content $MobileContent -Token $Token
    }
}

$P341FileSpecificTokens = @{
    "docs/release/P3_41_CONTROLLED_READINESS_STATUS_TRANSITION_EXECUTION_REVIEW_BOUNDARY.md" = @(
        "Controlled Readiness Status Transition Execution Review Boundary",
        "approved readiness status transition review reference",
        "controlled transition execution package evidence",
        "pre transition readiness status evidence",
        "observed readiness status evidence",
        "transition execution audit trail evidence",
        "post transition rollback decision evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel execution evidence when applicable",
        "status transition execution state"
    );
    "docs/operations/P3_41_STATUS_TRANSITION_EXECUTION_OPERATIONS_CONTROL_BOUNDARY.md" = @(
        "Status transition execution operations control boundary",
        "transition execution start timestamp",
        "transition execution completion timestamp",
        "transition execution command evidence",
        "transition execution audit trail evidence",
        "post transition validation evidence",
        "post transition smoke test evidence",
        "change management acceptance evidence",
        "release management acceptance evidence",
        "SQL Server operational acceptance evidence",
        "status transition execution state"
    );
    "docs/security/P3_41_STATUS_TRANSITION_EXECUTION_SECURITY_PRIVACY_DATA_CONTROL_BOUNDARY.md" = @(
        "Status transition execution security privacy data control boundary",
        "security acceptance evidence",
        "privacy acceptance evidence",
        "data governance acceptance evidence",
        "transition execution audit trail evidence",
        "rollback criteria evaluation evidence",
        "SQL Server operational source of truth confirmation",
        "mobile release channel execution evidence"
    );
    "docs/qa/P3_41_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW_DECISION_MATRIX.md" = @(
        "Controlled status transition execution review decision matrix",
        "approved readiness status transition review reference",
        "controlled transition execution package evidence",
        "transition execution start timestamp",
        "transition execution completion timestamp",
        "post transition validation evidence",
        "post transition rollback decision evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel execution evidence",
        "controlled transition execution blockers"
    );
    "docs/runbooks/P3_41_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW_RUNBOOK.md" = @(
        "Controlled Status Transition Execution Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "status transition execution state",
        "change management acceptance evidence is required",
        "release management acceptance evidence is required"
    );
    "docs/operations/templates/P3_41_CONTROLLED_STATUS_TRANSITION_EXECUTION_REVIEW_TEMPLATE.md" = @(
        "Controlled Status Transition Execution Review Template",
        "Approved readiness status transition review reference",
        "approved release candidate reference is required",
        "controlled transition execution package evidence",
        "pre transition readiness status evidence",
        "observed readiness status evidence",
        "transition execution audit trail evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel execution evidence is required for mobile",
        "change management acceptance evidence is required",
        "release management acceptance evidence is required",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "status transition execution state"
    )
}

foreach ($Entry in $P341FileSpecificTokens.GetEnumerator()) {
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
    "controlled transition execution review is final readiness status closure"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.41 controlled readiness status transition execution review boundary verifier passed from repo root: $RepoRoot"
