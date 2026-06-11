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

function Assert-DoesNotMatchRegex {
    param(
        [string]$Content,
        [string]$Pattern,
        [string]$Description
    )

    if ([System.Text.RegularExpressions.Regex]::IsMatch($Content, $Pattern, [System.Text.RegularExpressions.RegexOptions]::Multiline)) {
        throw "Forbidden pattern found: $Description"
    }
}

$RequiredFiles = @(
    "docs/implementation/P4_01_REAL_EVIDENCE_EXECUTION_BASELINE.md",
    "docs/implementation/P4_01_IMPLEMENTATION_READINESS_HANDOFF.md",
    "docs/qa/P4_01_REAL_EVIDENCE_ACCEPTANCE_MATRIX.md",
    "docs/runbooks/P4_01_REAL_EVIDENCE_CAPTURE_RUNBOOK.md",
    "scripts/p4/collect-p4-01-real-evidence-baseline.ps1",
    "scripts/verify-p4-01-real-evidence-execution-baseline.ps1"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P4_01_REAL_EVIDENCE_EXECUTION_BASELINE.md"
$HandoffContent = Read-RepoText -RelativePath "docs/implementation/P4_01_IMPLEMENTATION_READINESS_HANDOFF.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P4_01_REAL_EVIDENCE_ACCEPTANCE_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P4_01_REAL_EVIDENCE_CAPTURE_RUNBOOK.md"
$CollectorContent = Read-RepoText -RelativePath "scripts/p4/collect-p4-01-real-evidence-baseline.ps1"

$AllDocumentationContent = $ImplementationContent + "`n" + $HandoffContent + "`n" + $MatrixContent + "`n" + $RunbookContent

$GlobalRequiredTokens = @(
    'P4.1 Real Evidence Execution Baseline',
    'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE',
    'SQL Server is the operational source of truth',
    'ConnectionStrings__SqlServer',
    'P3.43 final production governance evidence index reference',
    'P4 implementation readiness handoff evidence',
    'P4 real evidence backlog evidence',
    'real evidence only',
    'sanitized evidence only',
    'evidence output root',
    'artifacts/p4/p4-01-real-evidence-baseline',
    'manifest.json',
    'command exit code',
    'git commit SHA evidence',
    'repository clean state evidence',
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
    'evidence rejection criteria',
    'real environment blocker register',
    'technical owner assignment',
    'operations owner assignment',
    'support owner assignment',
    'security owner assignment',
    'privacy owner assignment',
    'data owner assignment',
    'risk owner assignment',
    'compliance owner assignment',
    'mobile release channel evidence',
    'device fleet evidence',
    'offline sync evidence',
    'conflict resolution evidence',
    'No secrets in repository',
    'No direct mobile write to SQL Server',
    'No cloud dependency'
)

foreach ($Token in $GlobalRequiredTokens) {
    Assert-ContainsToken -Content $AllDocumentationContent -Token $Token
}

$FileSpecificTokens = @{
    "docs/implementation/P4_01_REAL_EVIDENCE_EXECUTION_BASELINE.md" = @(
        'P4.1 Real Evidence Execution Baseline',
        'This phase does not claim backend production readiness.',
        'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE',
        'SQL Server is the operational source of truth',
        'ConnectionStrings__SqlServer',
        'artifacts/p4/p4-01-real-evidence-baseline',
        'manifest.json',
        'dotnet build evidence',
        'dotnet test evidence',
        'P3 governance verifier evidence',
        'P4 verifier evidence',
        'SQL Server configuration presence evidence',
        'real environment blocker register'
    );
    "docs/implementation/P4_01_IMPLEMENTATION_READINESS_HANDOFF.md" = @(
        'P4.1 Implementation Readiness Handoff',
        'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE',
        'P3.43 final production governance evidence index reference',
        'P4 implementation readiness handoff evidence',
        'P4 real evidence backlog evidence',
        'technical owner assignment',
        'operations owner assignment',
        'support owner assignment',
        'security owner assignment',
        'privacy owner assignment',
        'data owner assignment',
        'risk owner assignment',
        'compliance owner assignment',
        'SQL Server configuration presence evidence',
        'API health check evidence',
        'OpenAPI artifact evidence',
        'monitoring evidence',
        'alerting evidence',
        'evidence sanitization status'
    );
    "docs/qa/P4_01_REAL_EVIDENCE_ACCEPTANCE_MATRIX.md" = @(
        'P4.1 Real Evidence Acceptance Matrix',
        'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE',
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
        'Reject P4.1 evidence'
    );
    "docs/runbooks/P4_01_REAL_EVIDENCE_CAPTURE_RUNBOOK.md" = @(
        'P4.1 Real Evidence Capture Runbook',
        'Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE',
        'scripts/p4/collect-p4-01-real-evidence-baseline.ps1',
        'artifacts/p4/p4-01-real-evidence-baseline',
        'manifest.json',
        '## Standard command',
        '```powershell',
        '& "scripts/p4/collect-p4-01-real-evidence-baseline.ps1"',
        '## Optional API health command',
        '& "scripts/p4/collect-p4-01-real-evidence-baseline.ps1" -ApiBaseUrl "https://localhost:7044"',
        'current PowerShell host',
        'ConnectionStrings__SqlServer',
        'command exit code',
        'evidence sanitization status',
        'P4.1 real evidence collector'
    )
}

foreach ($Entry in $FileSpecificTokens.GetEnumerator()) {
    $FileContent = Read-RepoText -RelativePath $Entry.Key

    foreach ($Token in $Entry.Value) {
        Assert-ContainsToken -Content $FileContent -Token $Token
    }
}

$CollectorRequiredTokens = @(
    'P4.1 Real Evidence Collector',
    'artifacts/p4/p4-01-real-evidence-baseline',
    'manifest.json',
    'ConnectionStrings__SqlServer',
    'dotnet restore',
    'dotnet build',
    'dotnet test',
    'api/v1/health',
    'Redact-Text',
    'command exit code',
    'BLOCKED_PENDING_REAL_EVIDENCE',
    'SQL Server is the operational source of truth',
    'sanitized evidence only',
    'real evidence only',
    'Join-Path $RepoRoot "scripts/verify-p3-43-final-production-governance-evidence-index.ps1"',
    'Join-Path $RepoRoot "scripts/verify-p4-01-real-evidence-execution-baseline.ps1"'
)

foreach ($Token in $CollectorRequiredTokens) {
    Assert-ContainsToken -Content $CollectorContent -Token $Token
}

$ForbiddenDocumentationTokens = @(
    'ConnectionStrings__CaritasDatabase',
    'User ID=sa',
    'Cloud is required',
    'Azure is required',
    'AWS is required',
    'mobile clients may write directly to SQL Server',
    'frontend may bypass API',
    'repository intentionally stores secrets',
    'real patient data is committed intentionally',
    'undocumented endpoints are allowed',
    'conflicts may be silently overwritten',
    'backend production readiness is approved',
    'backend is production ready'
)

foreach ($Token in $ForbiddenDocumentationTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token
}

$ForbiddenCollectorTokens = @(
    'powershell -ExecutionPolicy Bypass -File "scripts/verify-p3-43-final-production-governance-evidence-index.ps1"',
    'powershell -ExecutionPolicy Bypass -File "scripts/verify-p4-01-real-evidence-execution-baseline.ps1"'
)

foreach ($Token in $ForbiddenCollectorTokens) {
    Assert-DoesNotContainToken -Content $CollectorContent -Token $Token
}

Assert-DoesNotMatchRegex -Content $RunbookContent -Pattern '(?m)^`powershell$' -Description 'single-backtick malformed powershell fence'
Assert-DoesNotMatchRegex -Content $RunbookContent -Pattern '(?m)^`",$' -Description 'serialized malformed closing fence artifact'
Assert-DoesNotMatchRegex -Content $RunbookContent -Pattern '(?m)^\s*",\s*$' -Description 'standalone serialized comma artifact'
Assert-DoesNotContainToken -Content $RunbookContent -Token 'powershell -ExecutionPolicy Bypass -File scripts/p4/collect-p4-01-real-evidence-baseline.ps1'
Assert-DoesNotContainToken -Content $RunbookContent -Token 'powershell -ExecutionPolicy Bypass -File scripts/p4/collect-p4-01-real-evidence-baseline.ps1 -ApiBaseUrl'

Write-Host "P4.1 real evidence execution baseline verifier passed from repo root: $RepoRoot"