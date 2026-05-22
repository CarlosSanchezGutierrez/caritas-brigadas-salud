$ErrorActionPreference = "Stop"

function Assert-FileExists {
    param([Parameter(Mandatory = $true)][string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Missing required file: $Path"
    }
}

function Read-Text {
    param([Parameter(Mandatory = $true)][string]$Path)

    return [System.IO.File]::ReadAllText((Resolve-Path $Path))
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
    Assert-FileExists $file
}

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""

foreach ($file in $DocumentationFiles) {
    $AllDocumentationContent += "`n--- FILE: $file ---`n"
    $AllDocumentationContent += Read-Text $file
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

Write-Host "P3.8 SQL Server on-prem operational evidence baseline verifier passed."