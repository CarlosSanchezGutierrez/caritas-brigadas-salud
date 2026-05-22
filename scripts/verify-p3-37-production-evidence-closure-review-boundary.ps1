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
    "docs/release/P3_37_PRODUCTION_EVIDENCE_CLOSURE_REVIEW_BOUNDARY.md",
    "docs/web/P3_37_WEB_PRODUCTION_EVIDENCE_CLOSURE_REVIEW.md",
    "docs/mobile/P3_37_IOS_PRODUCTION_EVIDENCE_CLOSURE_REVIEW.md",
    "docs/mobile/P3_37_ANDROID_PRODUCTION_EVIDENCE_CLOSURE_REVIEW.md",
    "docs/operations/P3_37_PRODUCTION_EVIDENCE_OPERATIONS_CLOSURE_BOUNDARY.md",
    "docs/security/P3_37_PRODUCTION_EVIDENCE_SECURITY_PRIVACY_DATA_CLOSURE_BOUNDARY.md",
    "docs/qa/P3_37_PRODUCTION_EVIDENCE_CLOSURE_REVIEW_DECISION_MATRIX.md",
    "docs/runbooks/P3_37_PRODUCTION_EVIDENCE_CLOSURE_REVIEW_RUNBOOK.md",
    "docs/operations/templates/P3_37_PRODUCTION_EVIDENCE_CLOSURE_REVIEW_TEMPLATE.md",
    "scripts/verify-p3-37-production-evidence-closure-review-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Production Evidence Closure Review Boundary",
    "Web production evidence closure review",
    "iOS production evidence closure review",
    "Android production evidence closure review",
    "Production evidence operations closure boundary",
    "Production evidence security privacy data closure boundary",
    "Production evidence closure review decision matrix",
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
    "steady state readiness evidence",
    "operational ownership confirmation evidence",
    "support model acceptance evidence",
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
    "evidence inventory evidence",
    "evidence completeness evidence",
    "evidence traceability evidence",
    "evidence sanitization evidence",
    "final blocker review evidence",
    "backend production readiness decision input evidence",
    "mobile release channel closure evidence",
    "device fleet closure evidence",
    "offline sync closure evidence",
    "conflict resolution closure evidence",
    "production evidence closure decision evidence",
    "production evidence closure readiness blockers",
    "production evidence closure review state",
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

$P337CommonClientRequiredTokens = @(
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
    "steady state readiness evidence",
    "operational ownership confirmation evidence",
    "support model acceptance evidence",
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
    "evidence inventory evidence",
    "evidence completeness evidence",
    "evidence traceability evidence",
    "evidence sanitization evidence",
    "final blocker review evidence",
    "backend production readiness decision input evidence",
    "production evidence closure decision evidence",
    "production evidence closure readiness blockers",
    "production evidence closure review state",
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

$P337ClientFiles = @(
    "docs/web/P3_37_WEB_PRODUCTION_EVIDENCE_CLOSURE_REVIEW.md",
    "docs/mobile/P3_37_IOS_PRODUCTION_EVIDENCE_CLOSURE_REVIEW.md",
    "docs/mobile/P3_37_ANDROID_PRODUCTION_EVIDENCE_CLOSURE_REVIEW.md"
)

foreach ($ClientFile in $P337ClientFiles) {
    $ClientContent = Read-RepoText -RelativePath $ClientFile
    foreach ($Token in $P337CommonClientRequiredTokens) {
        Assert-ContainsToken -Content $ClientContent -Token $Token
    }
}

$P337MobileRequiredTokens = @(
    "mobile release channel closure evidence",
    "device fleet closure evidence",
    "offline sync closure evidence",
    "conflict resolution closure evidence",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id"
)

foreach ($MobileFile in @("docs/mobile/P3_37_IOS_PRODUCTION_EVIDENCE_CLOSURE_REVIEW.md", "docs/mobile/P3_37_ANDROID_PRODUCTION_EVIDENCE_CLOSURE_REVIEW.md")) {
    $MobileContent = Read-RepoText -RelativePath $MobileFile
    foreach ($Token in $P337MobileRequiredTokens) {
        Assert-ContainsToken -Content $MobileContent -Token $Token
    }
}

$P337FileSpecificTokens = @{
    "docs/release/P3_37_PRODUCTION_EVIDENCE_CLOSURE_REVIEW_BOUNDARY.md" = @(
        "Production Evidence Closure Review Boundary",
        "approved steady state readiness review reference",
        "production evidence closure package evidence",
        "evidence inventory evidence",
        "evidence completeness evidence",
        "evidence traceability evidence",
        "backend production readiness decision input evidence",
        "mobile release channel closure evidence when applicable",
        "production evidence closure review state"
    );
    "docs/operations/P3_37_PRODUCTION_EVIDENCE_OPERATIONS_CLOSURE_BOUNDARY.md" = @(
        "Production evidence operations closure boundary",
        "production evidence closure package evidence",
        "database health evidence",
        "SQL Server connectivity evidence",
        "backup recovery readiness evidence",
        "incident response readiness evidence",
        "backend production readiness decision input evidence",
        "production evidence closure review state"
    );
    "docs/security/P3_37_PRODUCTION_EVIDENCE_SECURITY_PRIVACY_DATA_CLOSURE_BOUNDARY.md" = @(
        "Production evidence security privacy data closure boundary",
        "access control readiness evidence",
        "data governance readiness evidence",
        "security readiness evidence",
        "privacy readiness evidence",
        "evidence completeness evidence",
        "evidence traceability evidence",
        "SQL Server operational source of truth confirmation"
    );
    "docs/qa/P3_37_PRODUCTION_EVIDENCE_CLOSURE_REVIEW_DECISION_MATRIX.md" = @(
        "Production evidence closure review decision matrix",
        "approved steady state readiness review reference",
        "production evidence closure package evidence",
        "evidence inventory evidence",
        "evidence completeness evidence",
        "backend production readiness decision input evidence",
        "mobile release channel closure evidence",
        "production evidence closure review state"
    );
    "docs/runbooks/P3_37_PRODUCTION_EVIDENCE_CLOSURE_REVIEW_RUNBOOK.md" = @(
        "Production Evidence Closure Review Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "production evidence closure review state"
    );
    "docs/operations/templates/P3_37_PRODUCTION_EVIDENCE_CLOSURE_REVIEW_TEMPLATE.md" = @(
        "Production Evidence Closure Review Template",
        "Approved steady state readiness review reference",
        "approved release candidate reference is required",
        "production evidence closure package evidence",
        "evidence inventory evidence",
        "evidence completeness evidence",
        "backend production readiness decision input evidence",
        "mobile release channel closure evidence is required for mobile",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "production evidence closure review state"
    )
}

foreach ($Entry in $P337FileSpecificTokens.GetEnumerator()) {
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
    "production evidence closure review is backend production readiness approval",
    "backend production readiness is approved",
    "backend is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.37 production evidence closure review boundary verifier passed from repo root: $RepoRoot"
