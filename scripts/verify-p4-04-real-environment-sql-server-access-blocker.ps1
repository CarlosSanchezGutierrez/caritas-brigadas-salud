$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) { $PSCommandPath } elseif ($MyInvocation.MyCommand.Path) { $MyInvocation.MyCommand.Path } else { throw "Unable to resolve script path." }
$ScriptDirectory = Split-Path -Parent $ScriptPath
$RepoRoot = Resolve-Path (Join-Path $ScriptDirectory "..")

function Resolve-RepoPath {
    param([string]$RelativePath)

    return Join-Path -Path $RepoRoot -ChildPath $RelativePath
}

function Assert-FileExists {
    param([string]$RelativePath)

    $AbsolutePath = Resolve-RepoPath -RelativePath $RelativePath

    if (-not (Test-Path $AbsolutePath)) {
        throw "Missing required file: $RelativePath resolved to $AbsolutePath"
    }
}

function Read-RepoText {
    param([string]$RelativePath)

    $AbsolutePath = Resolve-RepoPath -RelativePath $RelativePath

    if (-not (Test-Path $AbsolutePath)) {
        throw "Cannot read missing file: $RelativePath resolved to $AbsolutePath"
    }

    return [System.IO.File]::ReadAllText($AbsolutePath)
}

function Assert-ContainsToken {
    param([string]$Content, [string]$Token)

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "Missing required token: $Token"
    }
}

function Assert-DoesNotContainToken {
    param([string]$Content, [string]$Token)

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "Forbidden token found: $Token"
    }
}

$RequiredFiles = @(
    "docs/implementation/P4_04_REAL_ENVIRONMENT_SQL_SERVER_ACCESS_BLOCKER.md",
    "docs/qa/P4_04_SQL_SERVER_ACCESS_BLOCKER_ACCEPTANCE_MATRIX.md",
    "docs/runbooks/P4_04_SQL_SERVER_ACCESS_REQUEST_RUNBOOK.md",
    "scripts/verify-p4-04-real-environment-sql-server-access-blocker.ps1"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P4_04_REAL_ENVIRONMENT_SQL_SERVER_ACCESS_BLOCKER.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P4_04_SQL_SERVER_ACCESS_BLOCKER_ACCEPTANCE_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P4_04_SQL_SERVER_ACCESS_REQUEST_RUNBOOK.md"

$AllContent = $ImplementationContent + "`n" + $MatrixContent + "`n" + $RunbookContent

$GlobalTokens = @(
    'P4.4 Real Environment SQL Server Access Blocker',
    'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE',
    'P4.3 real evidence package',
    'P0 required blockers: 0',
    'P1 blocker candidates: 1',
    'P2 optional evidence gaps: 2',
    'ConnectionStrings__SqlServer missing.',
    'SQL Server configuration presence evidence',
    'database-config',
    'data owner',
    'configuration or SQL Server remediation',
    'institutional access blocker',
    'Required SQL Server access package',
    'SQL Server host, instance, and network access method',
    'Database name',
    'Authentication mode',
    'Least-privilege application credential',
    'Migration permission boundary',
    'Runtime permission boundary',
    'Backup and restore ownership',
    'TLS or certificate trust decision',
    'Data classification',
    'ConnectionStrings__SqlServer is present',
    'value is not printed',
    'sanitized evidence',
    'P4.2 classifier reports zero P1 database-config blockers',
    'No secrets in repository',
    'No fabricated evidence',
    'No backend production readiness approval',
    'No direct mobile write to SQL Server',
    'No client may bypass the API',
    'No cloud dependency',
    'SQL Server remains the operational source of truth'
)

foreach ($Token in $GlobalTokens) {
    Assert-ContainsToken -Content $AllContent -Token $Token
}

$ImplementationTokens = @(
    'This P1 blocker must not be closed with a decorative local environment variable.',
    'P4.4 does not connect to SQL Server.',
    'P4.4 does not approve production readiness.',
    'P4.4 does not create fake evidence.',
    'P4.4 only records the institutional access blocker'
)

foreach ($Token in $ImplementationTokens) {
    Assert-ContainsToken -Content $ImplementationContent -Token $Token
}

$MatrixTokens = @(
    'P4.4 SQL Server Access Blocker Acceptance Matrix',
    'Reject P4.4 closure',
    'A fake local-only connection string is used as institutional evidence',
    'Runtime and migration permission boundaries are mixed without approval',
    'Closure rule'
)

foreach ($Token in $MatrixTokens) {
    Assert-ContainsToken -Content $MatrixContent -Token $Token
}

$RunbookTokens = @(
    'P4.4 SQL Server Access Request Runbook',
    'Do not request secrets through GitHub',
    'Access package request',
    'Please do not send secrets through GitHub or public channels.',
    '& "scripts/p4/collect-p4-01-real-evidence-baseline.ps1"',
    '& "scripts/p4/classify-p4-01-evidence-package.ps1" -ManifestPath',
    'P1 blocker candidates: 0'
)

foreach ($Token in $RunbookTokens) {
    Assert-ContainsToken -Content $RunbookContent -Token $Token
}

$ForbiddenTokens = @(
    'ConnectionStrings__CaritasDatabase',
    'User ID=sa',
    'Password=',
    'Pwd=',
    'backend is production ready',
    'backend production readiness is approved',
    'mobile clients may write directly to SQL Server',
    'frontend may bypass API',
    'repository intentionally stores secrets',
    'real patient data is committed intentionally',
    'Cloud is required',
    'Azure is required',
    'AWS is required'
)

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllContent -Token $Token
}

Write-Host "P4.4 real environment SQL Server access blocker verifier passed from repo root: $RepoRoot"