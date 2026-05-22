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
    "docs/release/P3_35_OPERATIONAL_HANDOVER_REVIEW_BOUNDARY.md",
    "docs/web/P3_35_WEB_OPERATIONAL_HANDOVER_REVIEW.md",
    "docs/mobile/P3_35_IOS_OPERATIONAL_HANDOVER_REVIEW.md",
    "docs/mobile/P3_35_ANDROID_OPERATIONAL_HANDOVER_REVIEW.md",
    "docs/operations/P3_35_OPERATIONAL_HANDOVER_SUPPORT_MODEL_BOUNDARY.md",
    "docs/security/P3_35_OPERATIONAL_HANDOVER_SECURITY_PRIVACY_DATA_BOUNDARY.md",
    "docs/qa/P3_35_OPERATIONAL_HANDOVER_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_35_OPERATIONAL_HANDOVER_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_35_OPERATIONAL_HANDOVER_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-35-operational-handover-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Operational Handover Review Boundary",
    "Web operational handover review",
    "iOS operational handover review",
    "Android operational handover review",
    "Operational handover support model boundary",
    "Operational handover security privacy data boundary",
    "Operational handover review decision matrix",
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
    "operational handover package evidence",
    "ownership transfer evidence",
    "support model evidence",
    "support roster evidence",
    "escalation path evidence",
    "runbook acceptance evidence",
    "knowledge transfer completion evidence",
    "service level baseline evidence",
    "monitoring ownership evidence",
    "alert response ownership evidence",
    "incident management handover evidence",
    "change management handover evidence",
    "release management handover evidence",
    "backup ownership evidence",
    "recovery ownership evidence",
    "access control handover evidence",
    "audit trail ownership evidence",
    "mobile release channel ownership evidence",
    "device fleet ownership evidence",
    "offline sync ownership evidence",
    "conflict resolution ownership evidence",
    "data governance handover evidence",
    "security ownership handover evidence",
    "privacy ownership handover evidence",
    "residual risk ownership evidence",
    "open incident acceptance evidence",
    "open defect acceptance evidence",
    "known limitation acceptance evidence",
    "operational acceptance decision evidence",
    "operational handover readiness blockers",
    "operational handover review state",
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

$P335FileSpecificTokens = @{
    "docs/release/P3_35_OPERATIONAL_HANDOVER_REVIEW_BOUNDARY.md" = @(
        "Operational Handover Review Boundary",
        "approved stabilization review reference",
        "operational handover package evidence",
        "ownership transfer evidence",
        "support model evidence",
        "runbook acceptance evidence",
        "monitoring ownership evidence",
        "security ownership handover evidence",
        "privacy ownership handover evidence",
        "operational handover review state"
    );
    "docs/web/P3_35_WEB_OPERATIONAL_HANDOVER_REVIEW.md" = @(
        "Web operational handover review",
        "approved stabilization review reference",
        "approved release candidate reference",
        "operational handover package evidence",
        "support model evidence",
        "runbook acceptance evidence",
        "access control handover evidence",
        "audit trail ownership evidence",
        "security ownership handover evidence",
        "privacy ownership handover evidence",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "operational handover review state"
    );
    "docs/mobile/P3_35_IOS_OPERATIONAL_HANDOVER_REVIEW.md" = @(
        "iOS operational handover review",
        "mobile release channel ownership evidence",
        "device fleet ownership evidence",
        "offline sync ownership evidence",
        "conflict resolution ownership evidence",
        "device id",
        "idempotency key",
        "client operation id",
        "sync status",
        "server acknowledgment",
        "conflict id",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "operational handover review state"
    );
    "docs/mobile/P3_35_ANDROID_OPERATIONAL_HANDOVER_REVIEW.md" = @(
        "Android operational handover review",
        "mobile release channel ownership evidence",
        "device fleet ownership evidence",
        "offline sync ownership evidence",
        "conflict resolution ownership evidence",
        "device id",
        "idempotency key",
        "client operation id",
        "sync status",
        "server acknowledgment",
        "conflict id",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "operational handover review state"
    );
    "docs/operations/P3_35_OPERATIONAL_HANDOVER_SUPPORT_MODEL_BOUNDARY.md" = @(
        "Operational handover support model boundary",
        "support model evidence",
        "support roster evidence",
        "escalation path evidence",
        "knowledge transfer completion evidence",
        "monitoring owner assignment",
        "backup owner assignment",
        "access control owner assignment",
        "operational handover review state"
    );
    "docs/security/P3_35_OPERATIONAL_HANDOVER_SECURITY_PRIVACY_DATA_BOUNDARY.md" = @(
        "Operational handover security privacy data boundary",
        "security ownership handover evidence",
        "privacy ownership handover evidence",
        "data governance handover evidence",
        "access control handover evidence",
        "residual risk ownership evidence",
        "privacy-safe telemetry evidence",
        "SQL Server operational source of truth confirmation",
        "evidence sanitization status"
    );
    "docs/qa/P3_35_OPERATIONAL_HANDOVER_REVIEW_DECISION_MATRIX.md" = @(
        "Operational handover review decision matrix",
        "approved stabilization review reference",
        "operational handover package evidence",
        "support model evidence",
        "runbook acceptance evidence",
        "access control handover evidence",
        "security ownership handover evidence",
        "privacy ownership handover evidence",
        "mobile release channel ownership evidence",
        "operational handover review state"
    );
    "docs/runbooks/P3_35_OPERATIONAL_HANDOVER_REVIEW_RUNBOOK.md" = @(
        "Operational Handover Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "operational handover review state"
    );
    "docs/operations/templates/P3_35_OPERATIONAL_HANDOVER_REVIEW_TEMPLATE.md" = @(
        "Operational Handover Review Template",
        "Approved stabilization review reference",
        "approved release candidate reference is required",
        "operational handover package evidence",
        "ownership transfer evidence",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "operational handover review state"
    )
}

foreach ($Entry in $P335FileSpecificTokens.GetEnumerator()) {
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
    "operational handover review is production acceptance",
    "production steady state is approved",
    "backend is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.35 operational handover review boundary verifier passed from repo root: $RepoRoot"
