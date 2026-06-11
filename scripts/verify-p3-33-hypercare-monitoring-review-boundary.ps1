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
    "docs/release/P3_33_HYPERCARE_MONITORING_REVIEW_BOUNDARY.md",
    "docs/web/P3_33_WEB_HYPERCARE_MONITORING_REVIEW.md",
    "docs/mobile/P3_33_IOS_HYPERCARE_MONITORING_REVIEW.md",
    "docs/mobile/P3_33_ANDROID_HYPERCARE_MONITORING_REVIEW.md",
    "docs/operations/P3_33_HYPERCARE_SUPPORT_INCIDENT_AND_ESCALATION_BOUNDARY.md",
    "docs/security/P3_33_HYPERCARE_SECURITY_PRIVACY_MONITORING_BOUNDARY.md",
    "docs/qa/P3_33_HYPERCARE_MONITORING_DECISION_MATRIX.md",
    "docs/runbooks/P3_33_HYPERCARE_MONITORING_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_33_HYPERCARE_MONITORING_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-33-hypercare-monitoring-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Hypercare Monitoring Review Boundary",
    "Web hypercare monitoring review",
    "iOS hypercare monitoring review",
    "Android hypercare monitoring review",
    "Hypercare support incident escalation boundary",
    "Hypercare security privacy monitoring boundary",
    "Hypercare monitoring decision matrix",
    "approved deployment execution review reference",
    "approved deployment execution planning reference",
    "approved final go live authorization review reference",
    "approved go live planning review reference",
    "approved production readiness review execution reference",
    "approved release candidate reference",
    "deployment execution evidence",
    "rollback decision evidence",
    "post deployment smoke test evidence",
    "post deployment validation evidence",
    "post deployment monitoring evidence",
    "hypercare activation evidence",
    "environment name",
    "deployed commit SHA",
    "artifact reference",
    "API contract version",
    "OpenAPI artifact reference",
    "hypercare monitoring window",
    "hypercare owner assignment",
    "support owner assignment",
    "incident commander assignment",
    "escalation owner assignment",
    "security owner assignment",
    "privacy owner assignment",
    "data owner assignment",
    "support ticket evidence",
    "incident log evidence",
    "error budget evidence",
    "availability evidence",
    "latency evidence",
    "API error rate evidence",
    "database health evidence",
    "SQL Server connectivity evidence",
    "audit trail health evidence",
    "privacy-safe telemetry evidence",
    "user feedback evidence",
    "mobile release channel monitoring evidence",
    "device rollout monitoring evidence",
    "sync health evidence",
    "offline queue health evidence",
    "conflict resolution evidence",
    "post deployment defect triage evidence",
    "hypercare action register",
    "stabilization readiness blockers",
    "hypercare monitoring review state",
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

$P333FileSpecificTokens = @{
    "docs/release/P3_33_HYPERCARE_MONITORING_REVIEW_BOUNDARY.md" = @(
        "Hypercare Monitoring Review Boundary",
        "approved deployment execution review reference",
        "hypercare monitoring window",
        "support ticket evidence",
        "incident log evidence",
        "database health evidence",
        "sync health evidence",
        "stabilization readiness blockers",
        "hypercare monitoring review state"
    );
    "docs/web/P3_33_WEB_HYPERCARE_MONITORING_REVIEW.md" = @(
        "Web hypercare monitoring review",
        "support ticket evidence",
        "incident log evidence",
        "availability evidence",
        "latency evidence",
        "API error rate evidence",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "hypercare monitoring review state"
    );
    "docs/mobile/P3_33_IOS_HYPERCARE_MONITORING_REVIEW.md" = @(
        "iOS hypercare monitoring review",
        "mobile release channel monitoring evidence",
        "device rollout monitoring evidence",
        "sync health evidence",
        "offline queue health evidence",
        "conflict resolution evidence",
        "device id",
        "idempotency key",
        "client operation id",
        "sync status",
        "server acknowledgment",
        "conflict id",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "hypercare monitoring review state"
    );
    "docs/mobile/P3_33_ANDROID_HYPERCARE_MONITORING_REVIEW.md" = @(
        "Android hypercare monitoring review",
        "mobile release channel monitoring evidence",
        "device rollout monitoring evidence",
        "sync health evidence",
        "offline queue health evidence",
        "conflict resolution evidence",
        "device id",
        "idempotency key",
        "client operation id",
        "sync status",
        "server acknowledgment",
        "conflict id",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "hypercare monitoring review state"
    );
    "docs/operations/P3_33_HYPERCARE_SUPPORT_INCIDENT_AND_ESCALATION_BOUNDARY.md" = @(
        "Hypercare support incident escalation boundary",
        "support ticket evidence",
        "incident log evidence",
        "support escalation evidence",
        "SQL Server connectivity evidence",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "hypercare monitoring review state"
    );
    "docs/security/P3_33_HYPERCARE_SECURITY_PRIVACY_MONITORING_BOUNDARY.md" = @(
        "Hypercare security privacy monitoring boundary",
        "privacy-safe telemetry evidence",
        "evidence sanitization status",
        "SQL Server operational source of truth confirmation",
        "audit trail health evidence",
        "sync health evidence",
        "offline queue health evidence",
        "conflict resolution evidence"
    );
    "docs/qa/P3_33_HYPERCARE_MONITORING_DECISION_MATRIX.md" = @(
        "Hypercare monitoring decision matrix",
        "approved deployment execution review reference",
        "support ticket evidence",
        "incident log evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "privacy-safe telemetry evidence",
        "sync health evidence",
        "offline queue health evidence",
        "conflict resolution evidence",
        "hypercare monitoring review state"
    );
    "docs/runbooks/P3_33_HYPERCARE_MONITORING_REVIEW_RUNBOOK.md" = @(
        "Hypercare Monitoring Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "hypercare monitoring review state"
    );
    "docs/operations/templates/P3_33_HYPERCARE_MONITORING_REVIEW_TEMPLATE.md" = @(
        "Hypercare Monitoring Review Template",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "hypercare monitoring review state"
    )
}

foreach ($Entry in $P333FileSpecificTokens.GetEnumerator()) {
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
    "hypercare monitoring review is steady state approval",
    "production steady state is approved",
    "backend is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.33 hypercare monitoring review boundary verifier passed from repo root: $RepoRoot"
