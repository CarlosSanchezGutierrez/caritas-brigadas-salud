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
    "docs/release/P3_39_INSTITUTIONAL_SIGNOFF_REVIEW_BOUNDARY.md",
    "docs/web/P3_39_WEB_INSTITUTIONAL_SIGNOFF_INPUT_REVIEW.md",
    "docs/mobile/P3_39_IOS_INSTITUTIONAL_SIGNOFF_INPUT_REVIEW.md",
    "docs/mobile/P3_39_ANDROID_INSTITUTIONAL_SIGNOFF_INPUT_REVIEW.md",
    "docs/operations/P3_39_INSTITUTIONAL_OPERATIONS_SIGNOFF_BOUNDARY.md",
    "docs/security/P3_39_INSTITUTIONAL_SECURITY_PRIVACY_DATA_SIGNOFF_BOUNDARY.md",
    "docs/qa/P3_39_INSTITUTIONAL_SIGNOFF_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_39_INSTITUTIONAL_SIGNOFF_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_39_INSTITUTIONAL_SIGNOFF_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-39-institutional-signoff-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Institutional Signoff Review Boundary",
    "Web institutional signoff input review",
    "iOS institutional signoff input review",
    "Android institutional signoff input review",
    "Institutional operations signoff boundary",
    "Institutional security privacy data signoff boundary",
    "Institutional signoff review decision matrix",
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
    "institutional signoff package evidence",
    "institutional signoff authority evidence",
    "institutional signoff criteria evidence",
    "institutional signoff record evidence",
    "institutional signoff state",
    "executive sponsor signoff evidence",
    "technical owner signoff evidence",
    "operations owner signoff evidence",
    "support owner signoff evidence",
    "security owner signoff evidence",
    "privacy owner signoff evidence",
    "data owner signoff evidence",
    "risk owner signoff evidence",
    "compliance owner signoff evidence",
    "final risk acceptance evidence",
    "final blocker disposition evidence",
    "readiness decision record acceptance evidence",
    "exception register acceptance evidence",
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
    "institutional acceptance decision evidence",
    "mobile release channel signoff evidence",
    "device fleet signoff evidence",
    "offline sync signoff evidence",
    "conflict resolution signoff evidence",
    "institutional signoff blockers",
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

$P339CommonClientRequiredTokens = @(
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
    "institutional signoff package evidence",
    "institutional signoff authority evidence",
    "institutional signoff criteria evidence",
    "institutional signoff record evidence",
    "institutional signoff state",
    "executive sponsor signoff evidence",
    "technical owner signoff evidence",
    "operations owner signoff evidence",
    "support owner signoff evidence",
    "security owner signoff evidence",
    "privacy owner signoff evidence",
    "data owner signoff evidence",
    "risk owner signoff evidence",
    "compliance owner signoff evidence",
    "final risk acceptance evidence",
    "final blocker disposition evidence",
    "readiness decision record acceptance evidence",
    "exception register acceptance evidence",
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
    "institutional acceptance decision evidence",
    "institutional signoff blockers",
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

$P339ClientFiles = @(
    "docs/web/P3_39_WEB_INSTITUTIONAL_SIGNOFF_INPUT_REVIEW.md",
    "docs/mobile/P3_39_IOS_INSTITUTIONAL_SIGNOFF_INPUT_REVIEW.md",
    "docs/mobile/P3_39_ANDROID_INSTITUTIONAL_SIGNOFF_INPUT_REVIEW.md"
)

foreach ($ClientFile in $P339ClientFiles) {
    $ClientContent = Read-RepoText -RelativePath $ClientFile
    foreach ($Token in $P339CommonClientRequiredTokens) {
        Assert-ContainsToken -Content $ClientContent -Token $Token
    }
}

$P339MobileRequiredTokens = @(
    "mobile release channel signoff evidence",
    "device fleet signoff evidence",
    "offline sync signoff evidence",
    "conflict resolution signoff evidence",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id"
)

foreach ($MobileFile in @("docs/mobile/P3_39_IOS_INSTITUTIONAL_SIGNOFF_INPUT_REVIEW.md", "docs/mobile/P3_39_ANDROID_INSTITUTIONAL_SIGNOFF_INPUT_REVIEW.md")) {
    $MobileContent = Read-RepoText -RelativePath $MobileFile
    foreach ($Token in $P339MobileRequiredTokens) {
        Assert-ContainsToken -Content $MobileContent -Token $Token
    }
}

$P339FileSpecificTokens = @{
    "docs/release/P3_39_INSTITUTIONAL_SIGNOFF_REVIEW_BOUNDARY.md" = @(
        "Institutional Signoff Review Boundary",
        "approved backend production readiness decision review reference",
        "institutional signoff package evidence",
        "institutional signoff authority evidence",
        "executive sponsor signoff evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel signoff evidence when applicable",
        "institutional signoff state"
    );
    "docs/operations/P3_39_INSTITUTIONAL_OPERATIONS_SIGNOFF_BOUNDARY.md" = @(
        "Institutional operations signoff boundary",
        "institutional signoff authority evidence",
        "executive sponsor signoff evidence",
        "operations owner signoff evidence",
        "support owner signoff evidence",
        "change management acceptance evidence",
        "release management acceptance evidence",
        "SQL Server operational acceptance evidence",
        "institutional signoff state"
    );
    "docs/security/P3_39_INSTITUTIONAL_SECURITY_PRIVACY_DATA_SIGNOFF_BOUNDARY.md" = @(
        "Institutional security privacy data signoff boundary",
        "security owner signoff evidence",
        "privacy owner signoff evidence",
        "data owner signoff evidence",
        "compliance owner signoff evidence",
        "SQL Server operational source of truth confirmation",
        "mobile release channel signoff evidence"
    );
    "docs/qa/P3_39_INSTITUTIONAL_SIGNOFF_REVIEW_DECISION_MATRIX.md" = @(
        "Institutional signoff review decision matrix",
        "approved backend production readiness decision review reference",
        "institutional signoff package evidence",
        "executive sponsor signoff evidence",
        "compliance owner signoff evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel signoff evidence",
        "institutional signoff blockers"
    );
    "docs/runbooks/P3_39_INSTITUTIONAL_SIGNOFF_REVIEW_RUNBOOK.md" = @(
        "Institutional Signoff Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "institutional signoff state",
        "change management acceptance evidence is required",
        "release management acceptance evidence is required"
    );
    "docs/operations/templates/P3_39_INSTITUTIONAL_SIGNOFF_REVIEW_TEMPLATE.md" = @(
        "Institutional Signoff Review Template",
        "Approved backend production readiness decision review reference",
        "approved release candidate reference is required",
        "institutional signoff package evidence",
        "institutional signoff authority evidence",
        "executive sponsor signoff evidence",
        "SQL Server operational acceptance evidence",
        "mobile release channel signoff evidence is required for mobile",
        "change management acceptance evidence is required",
        "release management acceptance evidence is required",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "institutional signoff state"
    )
}

foreach ($Entry in $P339FileSpecificTokens.GetEnumerator()) {
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
    "automatic backend readiness status transition is allowed"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.39 institutional signoff review boundary verifier passed from repo root: $RepoRoot"
