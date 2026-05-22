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
    "docs/release/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_REVIEW_BOUNDARY.md",
    "docs/web/P3_38_WEB_BACKEND_READINESS_DECISION_INPUT_REVIEW.md",
    "docs/mobile/P3_38_IOS_BACKEND_READINESS_DECISION_INPUT_REVIEW.md",
    "docs/mobile/P3_38_ANDROID_BACKEND_READINESS_DECISION_INPUT_REVIEW.md",
    "docs/operations/P3_38_BACKEND_READINESS_OPERATIONS_DECISION_BOUNDARY.md",
    "docs/security/P3_38_BACKEND_READINESS_SECURITY_PRIVACY_DATA_DECISION_BOUNDARY.md",
    "docs/qa/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_MATRIX.md",
    "docs/runbooks/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-38-backend-production-readiness-decision-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Backend Production Readiness Decision Review Boundary",
    "Web backend readiness decision input review",
    "iOS backend readiness decision input review",
    "Android backend readiness decision input review",
    "Backend readiness operations decision boundary",
    "Backend readiness security privacy data decision boundary",
    "Backend production readiness decision matrix",
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
    "production evidence closure package evidence",
    "backend production readiness decision input evidence",
    "backend readiness decision authority evidence",
    "backend readiness decision criteria evidence",
    "backend readiness decision record evidence",
    "backend readiness decision state",
    "decision owner assignment",
    "technical owner signoff evidence",
    "operations owner signoff evidence",
    "support owner signoff evidence",
    "security owner signoff evidence",
    "privacy owner signoff evidence",
    "data owner signoff evidence",
    "risk owner signoff evidence",
    "final risk acceptance evidence",
    "final blocker disposition evidence",
    "production readiness exception register",
    "production readiness rejection criteria",
    "production readiness rollback posture evidence",
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
    "mobile release channel decision input evidence",
    "device fleet decision input evidence",
    "offline sync decision input evidence",
    "conflict resolution decision input evidence",
    "backend production readiness decision blockers",
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

$P338CommonClientRequiredTokens = @(
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
    "production evidence closure package evidence",
    "backend production readiness decision input evidence",
    "backend readiness decision authority evidence",
    "backend readiness decision criteria evidence",
    "backend readiness decision record evidence",
    "backend readiness decision state",
    "decision owner assignment",
    "technical owner signoff evidence",
    "operations owner signoff evidence",
    "support owner signoff evidence",
    "security owner signoff evidence",
    "privacy owner signoff evidence",
    "data owner signoff evidence",
    "risk owner signoff evidence",
    "final risk acceptance evidence",
    "final blocker disposition evidence",
    "production readiness exception register",
    "production readiness rejection criteria",
    "production readiness rollback posture evidence",
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
    "backend production readiness decision blockers",
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

$P338ClientFiles = @(
    "docs/web/P3_38_WEB_BACKEND_READINESS_DECISION_INPUT_REVIEW.md",
    "docs/mobile/P3_38_IOS_BACKEND_READINESS_DECISION_INPUT_REVIEW.md",
    "docs/mobile/P3_38_ANDROID_BACKEND_READINESS_DECISION_INPUT_REVIEW.md"
)

foreach ($ClientFile in $P338ClientFiles) {
    $ClientContent = Read-RepoText -RelativePath $ClientFile
    foreach ($Token in $P338CommonClientRequiredTokens) {
        Assert-ContainsToken -Content $ClientContent -Token $Token
    }
}

$P338MobileRequiredTokens = @(
    "mobile release channel decision input evidence",
    "device fleet decision input evidence",
    "offline sync decision input evidence",
    "conflict resolution decision input evidence",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id"
)

foreach ($MobileFile in @("docs/mobile/P3_38_IOS_BACKEND_READINESS_DECISION_INPUT_REVIEW.md", "docs/mobile/P3_38_ANDROID_BACKEND_READINESS_DECISION_INPUT_REVIEW.md")) {
    $MobileContent = Read-RepoText -RelativePath $MobileFile
    foreach ($Token in $P338MobileRequiredTokens) {
        Assert-ContainsToken -Content $MobileContent -Token $Token
    }
}

$P338FileSpecificTokens = @{
    "docs/release/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_REVIEW_BOUNDARY.md" = @(
        "Backend Production Readiness Decision Review Boundary",
        "approved production evidence closure review reference",
        "backend production readiness decision input evidence",
        "backend readiness decision authority evidence",
        "backend readiness decision criteria evidence",
        "backend readiness decision record evidence",
        "SQL Server operational acceptance evidence",
        "backend readiness decision state"
    );
    "docs/operations/P3_38_BACKEND_READINESS_OPERATIONS_DECISION_BOUNDARY.md" = @(
        "Backend readiness operations decision boundary",
        "backend readiness decision authority evidence",
        "technical owner signoff evidence",
        "operations owner signoff evidence",
        "API operational acceptance evidence",
        "SQL Server operational acceptance evidence",
        "database operational acceptance evidence",
        "backup recovery acceptance evidence",
        "backend readiness decision state"
    );
    "docs/security/P3_38_BACKEND_READINESS_SECURITY_PRIVACY_DATA_DECISION_BOUNDARY.md" = @(
        "Backend readiness security privacy data decision boundary",
        "security owner signoff evidence",
        "privacy owner signoff evidence",
        "data owner signoff evidence",
        "access control acceptance evidence",
        "security acceptance evidence",
        "privacy acceptance evidence",
        "SQL Server operational source of truth confirmation"
    );
    "docs/qa/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_MATRIX.md" = @(
        "Backend production readiness decision matrix",
        "approved production evidence closure review reference",
        "backend production readiness decision input evidence",
        "backend readiness decision authority evidence",
        "backend readiness decision criteria evidence",
        "backend readiness decision record evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel decision input evidence",
        "backend production readiness decision blockers"
    );
    "docs/runbooks/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_REVIEW_RUNBOOK.md" = @(
        "Backend Production Readiness Decision Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "backend readiness decision state"
    );
    "docs/operations/templates/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_REVIEW_TEMPLATE.md" = @(
        "Backend Production Readiness Decision Review Template",
        "Approved production evidence closure review reference",
        "approved release candidate reference is required",
        "backend production readiness decision input evidence",
        "backend readiness decision authority evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel decision input evidence is required for mobile",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "backend readiness decision state"
    )
}

foreach ($Entry in $P338FileSpecificTokens.GetEnumerator()) {
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
    "automatic readiness status change is allowed"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }


# P3.38 Codex change release management runbook template regression checks
$P338RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_REVIEW_RUNBOOK.md"
$P338TemplateContent = Read-RepoText -RelativePath "docs/operations/templates/P3_38_BACKEND_PRODUCTION_READINESS_DECISION_REVIEW_TEMPLATE.md"

Assert-ContainsToken -Content $P338RunbookContent -Token "change management acceptance evidence"
Assert-ContainsToken -Content $P338RunbookContent -Token "release management acceptance evidence"
Assert-ContainsToken -Content $P338RunbookContent -Token "change management acceptance evidence is required"
Assert-ContainsToken -Content $P338RunbookContent -Token "release management acceptance evidence is required"

Assert-ContainsToken -Content $P338TemplateContent -Token "change management acceptance evidence"
Assert-ContainsToken -Content $P338TemplateContent -Token "release management acceptance evidence"
Assert-ContainsToken -Content $P338TemplateContent -Token "change management acceptance evidence is required"
Assert-ContainsToken -Content $P338TemplateContent -Token "release management acceptance evidence is required"
Write-Host "P3.38 backend production readiness decision review boundary verifier passed from repo root: $RepoRoot"
