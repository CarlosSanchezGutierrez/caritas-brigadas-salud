$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

function Assert-FileExists {
    param(
        [string]$Path,
        [string]$Label
    )

    if (-not (Test-Path $Path)) {
        throw "$Label file not found: $Path"
    }
}

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if (-not $Content.Contains($Token)) {
        throw "$Label does not contain required token: $Token"
    }
}

$BaselinePath = Join-Path $RepoRoot "docs/operations/P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT_BASELINE.md"
$AuditPath = Join-Path $RepoRoot "docs/operations/P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md"
$ClosureReportPath = Join-Path $RepoRoot "docs/operations/P3_BACKEND_PRODUCTION_READINESS_CLOSURE_REPORT.md"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 security and product readiness gap audit baseline"
Assert-FileExists $AuditPath "P3 security and product readiness gap audit"
Assert-FileExists $ClosureReportPath "P3 backend production readiness closure report"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Audit = Get-Content $AuditPath -Raw -Encoding UTF8
$ClosureReport = Get-Content $ClosureReportPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Security and Product Readiness Gap Audit Baseline",
    "REQUIRED_BEFORE_FRONTEND",
    "REQUIRED_BEFORE_STAGING",
    "REQUIRED_BEFORE_PRODUCTION",
    "OWNED_BY_INFRASTRUCTURE",
    "rate limiting",
    "dependency scanning",
    "secret scanning",
    "penetration testing",
    "SQL Server VM connectivity",
    "SQL Server least privilege",
    "network ACL and firewall rules",
    "deny-by-default traffic posture",
    "TLS between backend and SQL Server",
    "patient signature",
    "social security / insurance fields",
    "emergency contact fields",
    "OpenAPI/frontend contract readiness",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 security and product readiness gap audit baseline"
}

$RequiredAuditTokens = @(
    "P3 Security and Product Readiness Gap Audit",
    "Backend production readiness conclusion: NOT_PRODUCTION_READY",
    "Frontend readiness conclusion: NOT_READY_FOR_FULL_FRONTEND",
    "Phase plan",
    "Security gap matrix",
    "Product and medical workflow gap matrix",
    "What is unnecessary right now",
    "SQL Server VM interpretation",
    "Frontend readiness decision",
    "Production readiness decision",
    "Recommended immediate next PRs",
    "Rate limiting",
    "Dependency Review",
    "Static analysis",
    "Secret scanning",
    "Penetration testing",
    "SQL least privilege",
    "Network ACL/firewall",
    "Deny-by-default traffic posture",
    "Patient signature",
    "Privacy notice consent",
    "Social security / insurance",
    "Emergency contact",
    "Migrant/incomplete data handling",
    "OpenAPI contract",
    "Grafana dashboards",
    "The SQL Server VM does not create the backend."
)

foreach ($Token in $RequiredAuditTokens) {
    Assert-Contains $Audit $Token "P3 security and product readiness gap audit"
}

Assert-Contains $ClosureReport "NOT_PRODUCTION_READY" "P3 backend production readiness closure report"
Assert-Contains $Governance "verify-p3-security-product-readiness-gap-audit.ps1" "repository governance baseline"

Write-Host "P3 security and product readiness gap audit verification passed." -ForegroundColor Green