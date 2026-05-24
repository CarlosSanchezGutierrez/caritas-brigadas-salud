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
    "docs/implementation/P4_06_API_ROUTE_EVIDENCE_ALIGNMENT.md",
    "docs/qa/P4_06_API_ROUTE_EVIDENCE_ALIGNMENT_MATRIX.md",
    "docs/runbooks/P4_06_API_ROUTE_EVIDENCE_ALIGNMENT_RUNBOOK.md",
    "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1",
    "scripts/verify-p4-06-api-route-evidence-alignment.ps1"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P4_06_API_ROUTE_EVIDENCE_ALIGNMENT.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P4_06_API_ROUTE_EVIDENCE_ALIGNMENT_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P4_06_API_ROUTE_EVIDENCE_ALIGNMENT_RUNBOOK.md"
$CollectorContent = Read-RepoText -RelativePath "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1"
$P45ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P4_05_API_RUNTIME_OPENAPI_EVIDENCE_BOUNDARY.md"
$P45RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P4_05_API_RUNTIME_OPENAPI_EVIDENCE_RUNBOOK.md"

$AllContent = $ImplementationContent + "`n" + $MatrixContent + "`n" + $RunbookContent + "`n" + $CollectorContent + "`n" + $P45ImplementationContent + "`n" + $P45RunbookContent

$RequiredTokens = @(
    "P4.6 API Route Evidence Alignment",
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "/health/live",
    "/health/ready",
    "/openapi/v1/openapi.json",
    "/swagger",
    "P4.4 Real Environment SQL Server Access Blocker",
    "SQL Server remains the operational source of truth",
    "No fabricated evidence",
    "No backend production readiness approval",
    "No client may bypass the API",
    "No cloud dependency"
)

foreach ($Token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllContent -Token $Token
}

$CollectorTokens = @(
    '$HealthCandidates',
    '$OpenApiCandidates',
    'health_endpoint_candidates',
    'openapi_endpoint_candidates',
    'P4.6 API Route Evidence Alignment Applied',
    '/health/live',
    '/health/ready',
    '/openapi/v1/openapi.json',
    '/swagger'
)

foreach ($Token in $CollectorTokens) {
    Assert-ContainsToken -Content $CollectorContent -Token $Token
}

$ForbiddenTokens = @(
    "/api/v1/health",
    "/swagger/v1/swagger.json",
    "backend is production ready",
    "backend production readiness is approved",
    "mobile clients may write directly to SQL Server",
    "frontend may bypass API",
    "repository intentionally stores secrets",
    "real patient data is committed intentionally",
    "Cloud is required",
    "Azure is required",
    "AWS is required"
)

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllContent -Token $Token
}

Write-Host "P4.6 API route evidence alignment verifier passed from repo root: $RepoRoot"