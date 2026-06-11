$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) { $PSCommandPath } elseif ($MyInvocation.MyCommand.Path) { $MyInvocation.MyCommand.Path } else { throw "Unable to resolve script path." }
$RepoRootText = git -C (Split-Path -Parent $ScriptPath) rev-parse --show-toplevel

if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($RepoRootText)) {
    throw "Unable to resolve repo root through git."
}

$RepoRoot = Resolve-Path $RepoRootText.Trim()

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
    "docs/implementation/P4_05_API_RUNTIME_OPENAPI_EVIDENCE_BOUNDARY.md",
    "docs/qa/P4_05_API_RUNTIME_OPENAPI_ACCEPTANCE_MATRIX.md",
    "docs/runbooks/P4_05_API_RUNTIME_OPENAPI_EVIDENCE_RUNBOOK.md",
    "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1",
    "scripts/verify-p4-05-api-runtime-openapi-evidence-boundary.ps1"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P4_05_API_RUNTIME_OPENAPI_EVIDENCE_BOUNDARY.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P4_05_API_RUNTIME_OPENAPI_ACCEPTANCE_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P4_05_API_RUNTIME_OPENAPI_EVIDENCE_RUNBOOK.md"
$CollectorContent = Read-RepoText -RelativePath "scripts/p4/collect-p4-05-api-runtime-openapi-evidence.ps1"

$AllContent = $ImplementationContent + "`n" + $MatrixContent + "`n" + $RunbookContent + "`n" + $CollectorContent

$RequiredTokens = @(
    "P4.5 API Runtime and OpenAPI Evidence Boundary",
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "API health",
    "OpenAPI artifact evidence",
    "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj",
    "/health/live",
    "/health/ready",
    "/openapi/v1/openapi.json",
    "/swagger",
    "P4.4 Real Environment SQL Server Access Blocker",
    "No secrets in repository",
    "No fabricated evidence",
    "No backend production readiness approval",
    "No direct mobile write to SQL Server",
    "No client may bypass the API",
    "No cloud dependency",
    "SQL Server remains the operational source of truth"
)

foreach ($Token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllContent -Token $Token
}

$CollectorTokens = @(
    "StartLocalApi",
    "ApiBaseUrl",
    "API project path evidence",
    "API startup attempt evidence",
    "API health endpoint evidence",
    "OpenAPI endpoint evidence",
    "OpenAPI artifact scan evidence",
    "Redact-Text",
    "ConnectionStrings__SqlServer",
    "manifest.json",
    "BLOCKED_PENDING_REAL_EVIDENCE",
    "HealthCandidates",
    "OpenApiCandidates",
    "health_endpoint_candidates",
    "openapi_endpoint_candidates",
    "/health/live",
    "/health/ready",
    "/openapi/v1/openapi.json",
    "/swagger"
)

foreach ($Token in $CollectorTokens) {
    Assert-ContainsToken -Content $CollectorContent -Token $Token
}

$ForbiddenTokens = @(
    "ConnectionStrings__CaritasDatabase",
    "User ID=sa",
    "backend is production ready",
    
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

Write-Host "P4.5 API runtime and OpenAPI evidence boundary verifier passed from repo root: $RepoRoot"