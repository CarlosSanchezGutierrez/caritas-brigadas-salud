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
    "docs/implementation/P4_02_REAL_EVIDENCE_PACKAGE_CLASSIFICATION.md",
    "docs/qa/P4_02_REAL_EVIDENCE_BLOCKER_CLASSIFICATION_MATRIX.md",
    "docs/runbooks/P4_02_REAL_EVIDENCE_BLOCKER_TRIAGE_RUNBOOK.md",
    "scripts/p4/classify-p4-01-evidence-package.ps1",
    "scripts/verify-p4-02-real-evidence-package-classification.ps1"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P4_02_REAL_EVIDENCE_PACKAGE_CLASSIFICATION.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P4_02_REAL_EVIDENCE_BLOCKER_CLASSIFICATION_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P4_02_REAL_EVIDENCE_BLOCKER_TRIAGE_RUNBOOK.md"
$ClassifierContent = Read-RepoText -RelativePath "scripts/p4/classify-p4-01-evidence-package.ps1"

$AllContent = $ImplementationContent + "`n" + $MatrixContent + "`n" + $RunbookContent + "`n" + $ClassifierContent

$GlobalRequiredTokens = @(
    'P4.2 Real Evidence Package Classification',
    'P4.1 Real Evidence Execution Baseline',
    'manifest.json',
    'artifacts/p4/p4-01-real-evidence-baseline',
    'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE',
    'SQL Server is the operational source of truth',
    'ConnectionStrings__SqlServer',
    'P4.2 real evidence classification report',
    'P4.2 blocker backlog JSON',
    'P4.2 blocker backlog Markdown',
    'blocker severity',
    'blocker category',
    'blocker owner group',
    'remediation type',
    'evidence source',
    'required blocker flag',
    'optional evidence gap flag',
    'pass classification',
    'skipped classification',
    'failed classification',
    'unknown classification',
    'P0 required blocker',
    'P1 blocker candidate',
    'P2 optional evidence gap',
    'PASS accepted evidence',
    'real evidence only',
    'sanitized evidence only',
    'No secrets in repository',
    'No cloud dependency',
    'No fabricated evidence',
    'No backend production readiness approval',
    'No direct mobile write to SQL Server',
    'No client may bypass the API',
    'No undocumented endpoints',
    'No silent overwrite'
)

foreach ($Token in $GlobalRequiredTokens) {
    Assert-ContainsToken -Content $AllContent -Token $Token
}

$ImplementationTokens = @(
    'P4.2 does not approve backend production readiness.',
    'P4.2 consumes the P4.1 manifest.json',
    'Classification categories',
    'Severity rules',
    'P0 required blocker',
    'P1 blocker candidate',
    'P2 optional evidence gap',
    'PASS accepted evidence',
    'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE'
)

foreach ($Token in $ImplementationTokens) {
    Assert-ContainsToken -Content $ImplementationContent -Token $Token
}

$MatrixTokens = @(
    'P4.2 Real Evidence Blocker Classification Matrix',
    'repository clean state evidence',
    'git commit SHA evidence',
    'dotnet restore evidence',
    'dotnet build evidence',
    'dotnet test evidence',
    'P3 governance verifier evidence',
    'P4 verifier evidence',
    'SQL Server configuration presence evidence',
    'API health check evidence',
    'OpenAPI artifact evidence',
    'endpoint contract evidence',
    'audit trail evidence',
    'support diagnostic evidence',
    'monitoring evidence',
    'alerting evidence',
    'evidence sanitization status',
    'real environment blocker register',
    'Reject P4.2 classification',
    'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE'
)

foreach ($Token in $MatrixTokens) {
    Assert-ContainsToken -Content $MatrixContent -Token $Token
}

$RunbookTokens = @(
    'P4.2 Real Evidence Blocker Triage Runbook',
    '& "scripts/p4/collect-p4-01-real-evidence-baseline.ps1"',
    '& "scripts/p4/classify-p4-01-evidence-package.ps1" -ManifestPath',
    'p4-02-classification.json',
    'p4-02-blocker-backlog.md',
    'Triage order',
    'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE'
)

foreach ($Token in $RunbookTokens) {
    Assert-ContainsToken -Content $RunbookContent -Token $Token
}

$ClassifierTokens = @(
    'param(',
    'ManifestPath',
    'P4.1 Real Evidence Execution Baseline',
    'BLOCKED_PENDING_REAL_EVIDENCE',
    'Manifest results array is empty.',
    '$ManifestResults.Count -eq 0',
    'foreach ($Result in $ManifestResults)',
    'Get-Category',
    'Get-Severity',
    'Get-OwnerGroup',
    'Get-RemediationType',
    'p4-02-classification.json',
    'p4-02-blocker-backlog.md',
    'Convert-ToSafeText',
    'ConnectionStrings__SqlServer',
    'P0',
    'P1',
    'P2',
    'PASS',
    'UNKNOWN'
)

foreach ($Token in $ClassifierTokens) {
    Assert-ContainsToken -Content $ClassifierContent -Token $Token
}

$ForbiddenTokens = @(
    'ConnectionStrings__CaritasDatabase',
    'User ID=sa',
    'Cloud is required',
    'Azure is required',
    'AWS is required',
    'mobile clients may write directly to SQL Server',
    'frontend may bypass API',
    'repository intentionally stores secrets',
    'real patient data is committed intentionally',
    'backend production readiness is approved',
    'backend is production ready',
    'P4.2 approves backend production readiness'
)

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllContent -Token $Token
}

Write-Host "P4.2 real evidence package classification verifier passed from repo root: $RepoRoot"