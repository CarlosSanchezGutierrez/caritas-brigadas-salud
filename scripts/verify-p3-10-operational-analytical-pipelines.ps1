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
    "docs/data/P3_10_OPERATIONAL_ANALYTICAL_PIPELINES_BASELINE.md",
    "docs/data/P3_10_ANALYTICAL_SNAPSHOT_MODEL.md",
    "docs/reporting/P3_10_REPORT_EXPORT_GOVERNANCE.md",
    "docs/reporting/P3_10_METRIC_LINEAGE_BASELINE.md",
    "docs/security/P3_10_DATA_PIPELINE_SECURITY_PRIVACY_MODEL.md",
    "docs/runbooks/P3_10_PIPELINE_EVIDENCE_RUNBOOK.md",
    "docs/operations/templates/P3_10_PIPELINE_EVIDENCE_TEMPLATE.md",
    "scripts/verify-p3-10-operational-analytical-pipelines.ps1"
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
    "operational reporting pipeline",
    "analytical snapshot pipeline",
    "dashboard dataset pipeline",
    "export pipeline",
    "evidence package pipeline",
    "quality monitoring pipeline",
    "impact measurement pipeline",
    "pipeline id",
    "pipeline version",
    "source system",
    "source time range",
    "source tables or source views",
    "input record count",
    "output record count",
    "rejected records",
    "quarantine count",
    "validation result",
    "organization id",
    "correlation id",
    "request id",
    "audit trail reference",
    "snapshot id",
    "export id",
    "dataset id",
    "metric lineage",
    "No secrets in repository",
    "No cloud dependency",
    "No silent overwrite",
    "read-only reporting user",
    "minimum necessary data"
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
    "production ready"
)

foreach ($token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $token
}

Write-Host "P3.10 operational and analytical pipelines verifier passed from repo root: $RepoRoot"