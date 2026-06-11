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
    "docs/release/P3_36_STEADY_STATE_READINESS_REVIEW_BOUNDARY.md",
    "docs/web/P3_36_WEB_STEADY_STATE_READINESS_REVIEW.md",
    "docs/mobile/P3_36_IOS_STEADY_STATE_READINESS_REVIEW.md",
    "docs/mobile/P3_36_ANDROID_STEADY_STATE_READINESS_REVIEW.md",
    "docs/operations/P3_36_STEADY_STATE_OPERATIONS_AND_SUPPORT_BOUNDARY.md",
    "docs/security/P3_36_STEADY_STATE_SECURITY_PRIVACY_DATA_GOVERNANCE_BOUNDARY.md",
    "docs/qa/P3_36_STEADY_STATE_READINESS_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_36_STEADY_STATE_READINESS_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_36_STEADY_STATE_READINESS_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-36-steady-state-readiness-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Steady State Readiness Review Boundary",
    "Web steady state readiness review",
    "iOS steady state readiness review",
    "Android steady state readiness review",
    "Steady state operations and support boundary",
    "Steady state security privacy data governance boundary",
    "Steady state readiness review decision matrix",
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
    "steady state readiness evidence",
    "steady state monitoring window",
    "operational ownership confirmation evidence",
    "support model acceptance evidence",
    "support roster acceptance evidence",
    "escalation path acceptance evidence",
    "runbook operational acceptance evidence",
    "knowledge transfer closure evidence",
    "service level objective evidence",
    "service level indicator evidence",
    "availability evidence",
    "latency evidence",
    "API error rate evidence",
    "database health evidence",
    "SQL Server connectivity evidence",
    "backup recovery readiness evidence",
    "incident response readiness evidence",
    "change management readiness evidence",
    "release management readiness evidence",
    "access control readiness evidence",
    "audit trail health evidence",
    "data governance readiness evidence",
    "security readiness evidence",
    "privacy readiness evidence",
    "residual risk acceptance evidence",
    "open incident closure evidence",
    "open defect closure evidence",
    "known limitation acceptance evidence",
    "mobile release channel steady state evidence",
    "device fleet steady state evidence",
    "offline sync steady state evidence",
    "conflict resolution steady state evidence",
    "steady state acceptance decision evidence",
    "steady state readiness blockers",
    "steady state readiness review state",
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

$P336FileSpecificTokens = @{
    "docs/release/P3_36_STEADY_STATE_READINESS_REVIEW_BOUNDARY.md" = @(
        "Steady State Readiness Review Boundary",
        "approved operational handover review reference",
        "steady state readiness evidence",
        "service level objective evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "mobile release channel steady state evidence when applicable",
        "offline sync steady state evidence when applicable",
        "steady state readiness review state"
    );
    "docs/web/P3_36_WEB_STEADY_STATE_READINESS_REVIEW.md" = @(
        "Web steady state readiness review",
        "approved operational handover review reference",
        "approved stabilization review reference",
        "approved deployment execution planning reference",
        "approved go live planning review reference",
        "approved production readiness review execution reference",
        "approved release candidate reference",
        "steady state readiness evidence",
        "support model acceptance evidence",
        "service level objective evidence",
        "service level indicator evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "backup recovery readiness evidence",
        "security readiness evidence",
        "privacy readiness evidence",
        "data governance readiness evidence",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "steady state readiness review state"
    );
    "docs/mobile/P3_36_IOS_STEADY_STATE_READINESS_REVIEW.md" = @(
        "iOS steady state readiness review",
        "approved operational handover review reference",
        "approved stabilization review reference",
        "approved deployment execution planning reference",
        "approved go live planning review reference",
        "approved production readiness review execution reference",
        "approved release candidate reference",
        "steady state readiness evidence",
        "support model acceptance evidence",
        "service level objective evidence",
        "service level indicator evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "backup recovery readiness evidence",
        "security readiness evidence",
        "privacy readiness evidence",
        "data governance readiness evidence",
        "mobile release channel steady state evidence",
        "device fleet steady state evidence",
        "offline sync steady state evidence",
        "conflict resolution steady state evidence",
        "device id",
        "idempotency key",
        "client operation id",
        "sync status",
        "server acknowledgment",
        "conflict id",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "steady state readiness review state"
    );
    "docs/mobile/P3_36_ANDROID_STEADY_STATE_READINESS_REVIEW.md" = @(
        "Android steady state readiness review",
        "approved operational handover review reference",
        "approved stabilization review reference",
        "approved deployment execution planning reference",
        "approved go live planning review reference",
        "approved production readiness review execution reference",
        "approved release candidate reference",
        "steady state readiness evidence",
        "support model acceptance evidence",
        "service level objective evidence",
        "service level indicator evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "backup recovery readiness evidence",
        "security readiness evidence",
        "privacy readiness evidence",
        "data governance readiness evidence",
        "mobile release channel steady state evidence",
        "device fleet steady state evidence",
        "offline sync steady state evidence",
        "conflict resolution steady state evidence",
        "device id",
        "idempotency key",
        "client operation id",
        "sync status",
        "server acknowledgment",
        "conflict id",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "steady state readiness review state"
    );
    "docs/operations/P3_36_STEADY_STATE_OPERATIONS_AND_SUPPORT_BOUNDARY.md" = @(
        "Steady state operations and support boundary",
        "operational ownership confirmation evidence",
        "support model acceptance evidence",
        "service level objective evidence",
        "service level indicator evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "backup recovery readiness evidence",
        "incident response readiness evidence",
        "change management readiness evidence",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "steady state readiness review state"
    );
    "docs/security/P3_36_STEADY_STATE_SECURITY_PRIVACY_DATA_GOVERNANCE_BOUNDARY.md" = @(
        "Steady state security privacy data governance boundary",
        "access control readiness evidence",
        "data governance readiness evidence",
        "security readiness evidence",
        "privacy readiness evidence",
        "residual risk acceptance evidence",
        "privacy-safe telemetry evidence",
        "SQL Server operational source of truth confirmation",
        "mobile release channel steady state evidence",
        "offline sync steady state evidence"
    );
    "docs/qa/P3_36_STEADY_STATE_READINESS_REVIEW_DECISION_MATRIX.md" = @(
        "Steady state readiness review decision matrix",
        "approved operational handover review reference",
        "steady state readiness evidence",
        "service level objective evidence",
        "service level indicator evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "mobile release channel steady state evidence",
        "offline sync steady state evidence",
        "steady state readiness review state"
    );
    "docs/runbooks/P3_36_STEADY_STATE_READINESS_REVIEW_RUNBOOK.md" = @(
        "Steady State Readiness Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "steady state readiness review state"
    );
    "docs/operations/templates/P3_36_STEADY_STATE_READINESS_REVIEW_TEMPLATE.md" = @(
        "Steady State Readiness Review Template",
        "Approved operational handover review reference",
        "approved release candidate reference is required",
        "steady state readiness evidence",
        "service level objective evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "mobile release channel steady state evidence is required for mobile",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "steady state readiness review state"
    )
}

foreach ($Entry in $P336FileSpecificTokens.GetEnumerator()) {
    $FileContent = Read-RepoText -RelativePath $Entry.Key
    foreach ($Token in $Entry.Value) {
        Assert-ContainsToken -Content $FileContent -Token $Token
    }
}

$P336CommonClientRequiredTokens = @(
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
    "steady state readiness evidence",
    "steady state monitoring window",
    "operational ownership confirmation evidence",
    "support model acceptance evidence",
    "support roster acceptance evidence",
    "escalation path acceptance evidence",
    "runbook operational acceptance evidence",
    "knowledge transfer closure evidence",
    "service level objective evidence",
    "service level indicator evidence",
    "availability evidence",
    "latency evidence",
    "API error rate evidence",
    "database health evidence",
    "SQL Server connectivity evidence",
    "backup recovery readiness evidence",
    "incident response readiness evidence",
    "change management readiness evidence",
    "release management readiness evidence",
    "access control readiness evidence",
    "audit trail health evidence",
    "data governance readiness evidence",
    "security readiness evidence",
    "privacy readiness evidence",
    "residual risk acceptance evidence",
    "open incident closure evidence",
    "open defect closure evidence",
    "known limitation acceptance evidence",
    "steady state acceptance decision evidence",
    "steady state readiness blockers",
    "steady state readiness review state",
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

$P336ClientFiles = @(
    "docs/web/P3_36_WEB_STEADY_STATE_READINESS_REVIEW.md",
    "docs/mobile/P3_36_IOS_STEADY_STATE_READINESS_REVIEW.md",
    "docs/mobile/P3_36_ANDROID_STEADY_STATE_READINESS_REVIEW.md"
)

foreach ($ClientFile in $P336ClientFiles) {
    $ClientContent = Read-RepoText -RelativePath $ClientFile
    foreach ($Token in $P336CommonClientRequiredTokens) {
        Assert-ContainsToken -Content $ClientContent -Token $Token
    }
}

$P336MobileRequiredTokens = @(
    "mobile release channel steady state evidence",
    "device fleet steady state evidence",
    "offline sync steady state evidence",
    "conflict resolution steady state evidence",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id"
)

foreach ($MobileFile in @("docs/mobile/P3_36_IOS_STEADY_STATE_READINESS_REVIEW.md", "docs/mobile/P3_36_ANDROID_STEADY_STATE_READINESS_REVIEW.md")) {
    $MobileContent = Read-RepoText -RelativePath $MobileFile
    foreach ($Token in $P336MobileRequiredTokens) {
        Assert-ContainsToken -Content $MobileContent -Token $Token
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
    "steady state readiness review is production evidence closure",
    "production evidence closure is approved",
    "backend is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.36 steady state readiness review boundary verifier passed from repo root: $RepoRoot"
