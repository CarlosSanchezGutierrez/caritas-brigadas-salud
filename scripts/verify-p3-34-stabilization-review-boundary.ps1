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
    "docs/release/P3_34_STABILIZATION_REVIEW_BOUNDARY.md",
    "docs/web/P3_34_WEB_STABILIZATION_REVIEW.md",
    "docs/mobile/P3_34_IOS_STABILIZATION_REVIEW.md",
    "docs/mobile/P3_34_ANDROID_STABILIZATION_REVIEW.md",
    "docs/operations/P3_34_STABILIZATION_OPERATIONAL_HANDOVER_READINESS_BOUNDARY.md",
    "docs/security/P3_34_STABILIZATION_SECURITY_PRIVACY_CLOSURE_REVIEW_BOUNDARY.md",
    "docs/qa/P3_34_STABILIZATION_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_34_STABILIZATION_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_34_STABILIZATION_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-34-stabilization-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Stabilization Review Boundary",
    "Web stabilization review",
    "iOS stabilization review",
    "Android stabilization review",
    "Stabilization operational handover readiness boundary",
    "Stabilization security privacy closure review boundary",
    "Stabilization review decision matrix",
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
    "stabilization monitoring window",
    "steady state readiness evidence",
    "operational handoff evidence",
    "support handoff evidence",
    "runbook handoff evidence",
    "knowledge transfer evidence",
    "service level baseline evidence",
    "open incident review evidence",
    "open defect review evidence",
    "known limitation review evidence",
    "residual risk acceptance evidence",
    "security closure evidence",
    "privacy closure evidence",
    "data governance closure evidence",
    "availability evidence",
    "latency evidence",
    "API error rate evidence",
    "database health evidence",
    "SQL Server connectivity evidence",
    "audit trail health evidence",
    "privacy-safe telemetry evidence",
    "user feedback evidence",
    "mobile release channel stability evidence",
    "device rollout stability evidence",
    "sync health evidence",
    "offline queue health evidence",
    "conflict resolution evidence",
    "stabilization action register",
    "operational handover readiness blockers",
    "stabilization review state",
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

$P334FileSpecificTokens = @{
    "docs/release/P3_34_STABILIZATION_REVIEW_BOUNDARY.md" = @(
        "Stabilization Review Boundary",
        "approved hypercare monitoring review reference",
        "steady state readiness evidence",
        "operational handoff evidence",
        "support handoff evidence",
        "security closure evidence",
        "privacy closure evidence",
        "data governance closure evidence",
        "operational handover readiness blockers",
        "stabilization review state"
    );
    "docs/web/P3_34_WEB_STABILIZATION_REVIEW.md" = @(
        "Web stabilization review",
        "approved hypercare monitoring review reference",
        "steady state readiness evidence",
        "service level baseline evidence",
        "open incident review evidence",
        "open defect review evidence",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "stabilization review state"
    );
    "docs/mobile/P3_34_IOS_STABILIZATION_REVIEW.md" = @(
        "iOS stabilization review",
        "mobile release channel stability evidence",
        "device rollout stability evidence",
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
        "stabilization review state"
    );
    "docs/mobile/P3_34_ANDROID_STABILIZATION_REVIEW.md" = @(
        "Android stabilization review",
        "mobile release channel stability evidence",
        "device rollout stability evidence",
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
        "stabilization review state"
    );
    "docs/operations/P3_34_STABILIZATION_OPERATIONAL_HANDOVER_READINESS_BOUNDARY.md" = @(
        "Stabilization operational handover readiness boundary",
        "operational handoff evidence",
        "support handoff evidence",
        "runbook handoff evidence",
        "knowledge transfer evidence",
        "service level baseline evidence",
        "SQL Server connectivity evidence",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "stabilization review state"
    );
    "docs/security/P3_34_STABILIZATION_SECURITY_PRIVACY_CLOSURE_REVIEW_BOUNDARY.md" = @(
        "Stabilization security privacy closure review boundary",
        "security closure evidence",
        "privacy closure evidence",
        "data governance closure evidence",
        "privacy-safe telemetry evidence",
        "evidence sanitization status",
        "SQL Server operational source of truth confirmation",
        "sync health evidence",
        "offline queue health evidence",
        "conflict resolution evidence"
    );
    "docs/qa/P3_34_STABILIZATION_REVIEW_DECISION_MATRIX.md" = @(
        "Stabilization review decision matrix",
        "approved hypercare monitoring review reference",
        "steady state readiness evidence",
        "operational handoff evidence",
        "security closure evidence",
        "privacy closure evidence",
        "data governance closure evidence",
        "sync health evidence",
        "offline queue health evidence",
        "conflict resolution evidence",
        "stabilization review state"
    );
    "docs/runbooks/P3_34_STABILIZATION_REVIEW_RUNBOOK.md" = @(
        "Stabilization Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "stabilization review state"
    );
    "docs/operations/templates/P3_34_STABILIZATION_REVIEW_TEMPLATE.md" = @(
        "Stabilization Review Template",
        "Approved hypercare monitoring review reference",
        "approved release candidate reference is required",
        "steady state readiness evidence",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "stabilization review state"
    )
}

foreach ($Entry in $P334FileSpecificTokens.GetEnumerator()) {
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
    "stabilization review is final production acceptance",
    "production steady state is approved",
    "backend is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.34 stabilization review boundary verifier passed from repo root: $RepoRoot"
