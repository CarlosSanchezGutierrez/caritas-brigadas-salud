$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) {
    $PSCommandPath
}
elseif ($MyInvocation.MyCommand.Path) {
    $MyInvocation.MyCommand.Path
}
else {
    throw "Unable to resolve script path."
}

$ScriptDirectory = Split-Path -Parent $ScriptPath
$RepoRoot = Resolve-Path (Join-Path $ScriptDirectory "..")

function Resolve-RepoPath {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    return Join-Path -Path $RepoRoot -ChildPath $RelativePath
}

function Assert-FileExists {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    $absolutePath = Resolve-RepoPath -RelativePath $RelativePath

    if (-not (Test-Path $absolutePath)) {
        throw "Missing required file: $RelativePath resolved to $absolutePath"
    }
}

function Read-RepoText {
    param([Parameter(Mandatory = $true)][string]$RelativePath)

    $absolutePath = Resolve-RepoPath -RelativePath $RelativePath

    if (-not (Test-Path $absolutePath)) {
        throw "Cannot read missing file: $RelativePath resolved to $absolutePath"
    }

    return [System.IO.File]::ReadAllText($absolutePath)
}

function Assert-ContainsToken {
    param(
        [Parameter(Mandatory = $true)][string]$Content,
        [Parameter(Mandatory = $true)][string]$Token
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "Missing required token: $Token"
    }
}

function Assert-DoesNotContainToken {
    param(
        [Parameter(Mandatory = $true)][string]$Content,
        [Parameter(Mandatory = $true)][string]$Token
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "Forbidden token found: $Token"
    }
}

$RequiredFiles = @(
    "docs/reporting/P3_11_KPI_DASHBOARD_INSIGHT_CATALOG.md",
    "docs/reporting/P3_11_KPI_DEFINITION_CATALOG.md",
    "docs/reporting/P3_11_DASHBOARD_CATALOG.md",
    "docs/reporting/P3_11_INSIGHT_AND_DIRECTION_REPORTING_GOVERNANCE.md",
    "docs/data/P3_11_KPI_DATA_QUALITY_AND_LINEAGE.md",
    "docs/runbooks/P3_11_REPORTING_CATALOG_EVIDENCE_RUNBOOK.md",
    "docs/operations/templates/P3_11_REPORTING_CATALOG_EVIDENCE_TEMPLATE.md",
    "scripts/verify-p3-11-kpi-dashboard-insight-direction-reporting.ps1"
)

foreach ($file in $RequiredFiles) {
    Assert-FileExists -RelativePath $file
}

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""

foreach ($file in $DocumentationFiles) {
    $AllDocumentationContent += "`n--- FILE: $file ---`n"
    $AllDocumentationContent += Read-RepoText -RelativePath $file
}

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "KPI catalog",
    "dashboard catalog",
    "insight catalog",
    "direction reporting",
    "metric id",
    "metric owner",
    "metric definition",
    "numerator",
    "denominator",
    "aggregation logic",
    "filter logic",
    "source snapshot id",
    "pipeline id",
    "pipeline version",
    "data quality",
    "audit trail reference",
    "organization id",
    "dashboard id",
    "dashboard owner",
    "refresh cadence",
    "decision owner",
    "action recommendation",
    "evidence package",
    "CSV/XLSX export",
    "direction report",
    "executive summary",
    "operational report",
    "tactical report",
    "strategic report",
    "No secrets in repository",
    "No cloud dependency",
    "No silent overwrite"
)

foreach ($token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllDocumentationContent -Token $token
}

$ForbiddenTokens = @(
    "ConnectionStrings__CaritasDatabase",
    "password=",
    "User ID=sa",
    "Cloud is required",
    "Azure is required",
    "AWS is required",
    "dashboard writes into operational clinical tables are allowed",
    "audit bypass allowed",
    "unrestricted patient-level exports are allowed",
    "insight without evidence is allowed",
    "KPI without owner is allowed",
    "production ready"
)

foreach ($token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $token
}

Write-Host "P3.11 KPI, dashboard, insight, and direction reporting verifier passed from repo root: $RepoRoot"