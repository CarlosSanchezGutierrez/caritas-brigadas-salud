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
    "docs/release/P3_43_FINAL_PRODUCTION_GOVERNANCE_EVIDENCE_INDEX.md",
    "docs/web/P3_43_WEB_FINAL_GOVERNANCE_EVIDENCE_INDEX.md",
    "docs/mobile/P3_43_IOS_FINAL_GOVERNANCE_EVIDENCE_INDEX.md",
    "docs/mobile/P3_43_ANDROID_FINAL_GOVERNANCE_EVIDENCE_INDEX.md",
    "docs/operations/P3_43_FINAL_GOVERNANCE_EVIDENCE_INDEX_OWNERSHIP_BOUNDARY.md",
    "docs/security/P3_43_FINAL_SECURITY_PRIVACY_DATA_EVIDENCE_INDEX.md",
    "docs/qa/P3_43_FINAL_GOVERNANCE_EVIDENCE_TRACEABILITY_MATRIX.md",
    "docs/runbooks/P3_43_FINAL_PRODUCTION_GOVERNANCE_EVIDENCE_INDEX_RUNBOOK.md",
    "docs/operations/templates/P3_43_FINAL_PRODUCTION_GOVERNANCE_EVIDENCE_INDEX_TEMPLATE.md",
    "scripts/verify-p3-43-final-production-governance-evidence-index.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) {
    $AllDocumentationContent += "`n--- FILE: $File ---`n"
    $AllDocumentationContent += Read-RepoText -RelativePath $File
}

$ApprovalTokens = @(
    "approved post transition monitoring review reference",
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
    "approved production readiness review entry reference",
    "approved pilot evidence review reference",
    "approved controlled pilot readiness reference",
    "approved release candidate reference",
    "approved API contract freeze reference"
)

$IdentityTokens = @(
    "environment name",
    "deployed commit SHA",
    "artifact reference",
    "API contract version",
    "OpenAPI artifact reference"
)

$IndexTokens = @(
    "final production governance evidence index package evidence",
    "P3.6 production evidence baseline index entry",
    "P3.7 on prem backend closure architecture index entry",
    "P3.8 SQL Server on prem operational evidence index entry",
    "P3.9 auditability longitudinal history index entry",
    "P3.10 operational analytical pipelines index entry",
    "P3.11 KPI dashboard insight direction reporting index entry",
    "P3.12 offline first mobile sync contract index entry",
    "P3.13 API contract freeze index entry",
    "P3.14 OpenAPI client stub evidence baseline index entry",
    "P3.15 client integration readiness matrix index entry",
    "P3.16 client implementation kickoff boundary index entry",
    "P3.17 implementation workstream split index entry",
    "P3.18 shared API client scaffold governance index entry",
    "P3.19 API client model error envelope contracts index entry",
    "P3.20 client API contract test harness index entry",
    "P3.21 client runtime config environment boundary index entry",
    "P3.22 client observability telemetry support boundary index entry",
    "P3.23 client CI CD quality gate boundary index entry",
    "P3.24 client release candidate approval boundary index entry",
    "P3.25 controlled pilot readiness boundary index entry",
    "P3.26 controlled pilot evidence review boundary index entry",
    "P3.27 production readiness review entry boundary index entry",
    "P3.28 production readiness review execution boundary index entry",
    "P3.29 go live planning review boundary index entry",
    "P3.30 final go live authorization review boundary index entry",
    "P3.31 deployment execution planning boundary index entry",
    "P3.32 deployment execution review boundary index entry",
    "P3.33 hypercare monitoring review boundary index entry",
    "P3.34 stabilization review boundary index entry",
    "P3.35 operational handover review boundary index entry",
    "P3.36 steady state readiness review boundary index entry",
    "P3.37 production evidence closure review boundary index entry",
    "P3.38 backend production readiness decision review boundary index entry",
    "P3.39 institutional signoff review boundary index entry",
    "P3.40 readiness status transition review boundary index entry",
    "P3.41 controlled readiness status transition execution review boundary index entry",
    "P3.42 post transition monitoring review boundary index entry",
    "evidence catalog evidence",
    "evidence owner evidence",
    "evidence location evidence",
    "evidence version evidence",
    "evidence freshness evidence",
    "evidence approval evidence",
    "evidence completeness evidence",
    "evidence traceability evidence",
    "evidence sanitization evidence",
    "evidence retention evidence",
    "evidence retrieval evidence",
    "evidence review cadence evidence",
    "evidence gap register",
    "blocker register",
    "exception register",
    "residual risk register",
    "real evidence requirement",
    "institutional owner assignment",
    "technical owner assignment",
    "operations owner assignment",
    "support owner assignment",
    "security owner assignment",
    "privacy owner assignment",
    "data owner assignment",
    "risk owner assignment",
    "compliance owner assignment",
    "P3 closure decision evidence",
    "P4 implementation readiness handoff evidence",
    "P4 real evidence backlog evidence",
    "final governance index review state"
)

$MobileIndexTokens = @(
    "mobile release channel evidence index entry",
    "device fleet evidence index entry",
    "offline sync evidence index entry",
    "conflict resolution evidence index entry"
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
    "Final Production Governance Evidence Index",
    "Web final governance evidence index",
    "iOS final governance evidence index",
    "Android final governance evidence index",
    "Final governance evidence index ownership boundary",
    "Final security privacy data evidence index",
    "Final governance evidence traceability matrix",
    "No secrets in repository",
    "No direct mobile write to SQL Server",
    "No cloud dependency"
) + $ApprovalTokens + $IdentityTokens + $IndexTokens + $MobileIndexTokens + $MetadataTokens + $MobileMetadataTokens

foreach ($Token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllDocumentationContent -Token $Token
}

$P343CommonClientRequiredTokens = $ApprovalTokens + $IdentityTokens + $IndexTokens + $MetadataTokens

$P343ClientFiles = @(
    "docs/web/P3_43_WEB_FINAL_GOVERNANCE_EVIDENCE_INDEX.md",
    "docs/mobile/P3_43_IOS_FINAL_GOVERNANCE_EVIDENCE_INDEX.md",
    "docs/mobile/P3_43_ANDROID_FINAL_GOVERNANCE_EVIDENCE_INDEX.md"
)

foreach ($ClientFile in $P343ClientFiles) {
    $ClientContent = Read-RepoText -RelativePath $ClientFile
    foreach ($Token in $P343CommonClientRequiredTokens) {
        Assert-ContainsToken -Content $ClientContent -Token $Token
    }
}

$P343MobileRequiredTokens = $MobileIndexTokens + $MobileMetadataTokens

foreach ($MobileFile in @("docs/mobile/P3_43_IOS_FINAL_GOVERNANCE_EVIDENCE_INDEX.md", "docs/mobile/P3_43_ANDROID_FINAL_GOVERNANCE_EVIDENCE_INDEX.md")) {
    $MobileContent = Read-RepoText -RelativePath $MobileFile
    foreach ($Token in $P343MobileRequiredTokens) {
        Assert-ContainsToken -Content $MobileContent -Token $Token
    }
}

$P343FileSpecificTokens = @{
    "docs/release/P3_43_FINAL_PRODUCTION_GOVERNANCE_EVIDENCE_INDEX.md" = @(
        "Final Production Governance Evidence Index",
        "approved post transition monitoring review reference",
        "final production governance evidence index package evidence",
        "P3.42 post transition monitoring review boundary index entry",
        "P4 implementation readiness handoff evidence",
        "P4 real evidence backlog evidence",
        "mobile release channel evidence index entry when applicable",
        "final governance index review state"
    );
    "docs/operations/P3_43_FINAL_GOVERNANCE_EVIDENCE_INDEX_OWNERSHIP_BOUNDARY.md" = @(
        "Final governance evidence index ownership boundary",
        "evidence catalog evidence",
        "evidence owner evidence",
        "evidence location evidence",
        "evidence review cadence evidence",
        "institutional owner assignment",
        "technical owner assignment",
        "operations owner assignment",
        "P4 implementation readiness handoff evidence",
        "P4 real evidence backlog evidence",
        "final governance index review state"
    );
    "docs/security/P3_43_FINAL_SECURITY_PRIVACY_DATA_EVIDENCE_INDEX.md" = @(
        "Final security privacy data evidence index",
        "security owner assignment",
        "privacy owner assignment",
        "data owner assignment",
        "evidence completeness evidence",
        "evidence traceability evidence",
        "evidence sanitization evidence",
        "SQL Server operational source of truth confirmation",
        "mobile release channel evidence index entry"
    );
    "docs/qa/P3_43_FINAL_GOVERNANCE_EVIDENCE_TRACEABILITY_MATRIX.md" = @(
        "Final governance evidence traceability matrix",
        "approved post transition monitoring review reference",
        "final production governance evidence index package evidence",
        "P3.6 production evidence baseline index entry",
        "P3.42 post transition monitoring review boundary index entry",
        "P4 real evidence backlog evidence",
        "mobile release channel evidence index entry"
    );
    "docs/runbooks/P3_43_FINAL_PRODUCTION_GOVERNANCE_EVIDENCE_INDEX_RUNBOOK.md" = @(
        "Final Production Governance Evidence Index Runbook",
        "authorization role",
        "endpoint id when applicable",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "final governance index review state",
        "P4 implementation readiness handoff evidence is required",
        "P4 real evidence backlog evidence is required",
        "mobile release channel evidence index entry is required"
    );
    "docs/operations/templates/P3_43_FINAL_PRODUCTION_GOVERNANCE_EVIDENCE_INDEX_TEMPLATE.md" = @(
        "Final Production Governance Evidence Index Template",
        "approved post transition monitoring review reference",
        "approved release candidate reference is required",
        "final production governance evidence index package evidence",
        "P3.6 production evidence baseline index entry",
        "P3.42 post transition monitoring review boundary index entry",
        "P4 implementation readiness handoff evidence",
        "P4 real evidence backlog evidence",
        "mobile release channel evidence index entry is required",
        "authorization role",
        "endpoint id",
        "standard error envelope",
        "support diagnostic evidence",
        "monitoring evidence",
        "alerting evidence",
        "final governance index review state"
    )
}

foreach ($Entry in $P343FileSpecificTokens.GetEnumerator()) {
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
    "final production governance evidence index is backend production readiness approval"
)

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token
}

Write-Host "P3.43 final production governance evidence index verifier passed from repo root: $RepoRoot"