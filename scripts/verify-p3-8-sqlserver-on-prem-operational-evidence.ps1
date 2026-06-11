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
    "docs/operations/P3_8_SQLSERVER_ON_PREM_OPERATIONAL_EVIDENCE.md",
    "docs/database/P3_8_SQLSERVER_ON_PREM_DEPLOYMENT_BASELINE.md",
    "docs/database/P3_8_SQLSERVER_LEAST_PRIVILEGE_MATRIX.md",
    "docs/runbooks/P3_8_SQLSERVER_BACKUP_RESTORE_EVIDENCE_RUNBOOK.md",
    "docs/runbooks/P3_8_SQLSERVER_MIGRATION_EXECUTION_RUNBOOK.md",
    "docs/data/P3_8_CONTROLLED_DATA_INJECTION_BASELINE.md",
    "docs/operations/templates/P3_8_SQLSERVER_OPERATIONAL_EVIDENCE_TEMPLATE.md",
    "scripts/verify-p3-8-sqlserver-on-prem-operational-evidence.ps1"
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
    "SQL Server on-premise",
    "SQL Server is the operational source of truth",
    "ConnectionStrings__SqlServer",
    "No secrets in repository",
    "least privilege",
    "app runtime user",
    "migration user",
    "read-only reporting user",
    "backup and restore",
    "migration execution",
    "controlled data injection",
    "idempotency key",
    "rejected records",
    "accepted records",
    "quarantine",
    "audit trail",
    "health endpoint",
    "smoke test",
    "RPO",
    "RTO",
    "restore validation",
    "no sysadmin for runtime",
    "no db_owner for runtime"
)

foreach ($token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllDocumentationContent -Token $token
}

$ForbiddenTokens = @(
    "ConnectionStrings__CaritasDatabase",
    "password=",
    "User ID=sa",
    "sysadmin for app runtime",
    "db_owner for app runtime",
    "Azure is required",
    "AWS is required",
    "Cloud is required",
    "production ready"
)

foreach ($token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $token
}

Write-Host "P3.8 SQL Server on-prem operational evidence baseline verifier passed from repo root: $RepoRoot"