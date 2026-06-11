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
    "services/api-dotnet/src/Caritas.Brigadas.Api/Controllers/PatientsController.cs",
    "docs/implementation/P5_05_PATIENT_API_ENDPOINT_HARDENING.md",
    "docs/qa/P5_05_PATIENT_API_ENDPOINT_HARDENING_MATRIX.md",
    "docs/runbooks/P5_05_PATIENT_API_ENDPOINT_HARDENING_RUNBOOK.md",
    "scripts/verify-p5-05-patient-api-endpoint-hardening.ps1"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$ControllerContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Api/Controllers/PatientsController.cs"
$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P5_05_PATIENT_API_ENDPOINT_HARDENING.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P5_05_PATIENT_API_ENDPOINT_HARDENING_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P5_05_PATIENT_API_ENDPOINT_HARDENING_RUNBOOK.md"

$AllContent = $ControllerContent + "`n" + $ImplementationContent + "`n" + $MatrixContent + "`n" + $RunbookContent

$ControllerTokens = @(
    "[ApiController]",
    "[Produces(""application/json"")]",
    "[HttpGet(""api/v1/organizations/{organizationId:guid}/patients"")]",
    "[HttpGet(""api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}"")]",
    "[HttpGet(""api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}/clinical-record"")]",
    "[HttpPost(""api/v1/organizations/{organizationId:guid}/patients"")]",
    "PermissionCodes.PatientsRead",
    "PermissionCodes.PatientsWrite",
    "StatusCodes.Status200OK",
    "StatusCodes.Status201Created",
    "StatusCodes.Status400BadRequest",
    "StatusCodes.Status404NotFound",
    "StatusCodes.Status409Conflict",
    "StatusCodes.Status503ServiceUnavailable",
    "ApiResponse<",
    "ApiErrorResponse",
    "DatabaseNotConfigured",
    "database_not_configured",
    "CreatedAtAction(",
    "nameof(GetByIdAsync)",
    "BadRequest(error)",
    "NotFound(error)",
    "Conflict(error)"
)

foreach ($Token in $ControllerTokens) {
    Assert-ContainsToken -Content $ControllerContent -Token $Token
}

Assert-DoesNotContainToken -Content $ControllerContent -Token 'return Created($"/api/v1/organizations/{organizationId}/patients/{patient.Id}", response);'

$DocumentationTokens = @(
    "P5.5 Patient API Endpoint Hardening",
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "GET /api/v1/organizations/{organizationId:guid}/patients",
    "GET /api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}",
    "GET /api/v1/organizations/{organizationId:guid}/patients/{patientId:guid}/clinical-record",
    "POST /api/v1/organizations/{organizationId:guid}/patients",
    "PatientsRead",
    "PatientsWrite",
    "201 Created",
    "400 Bad Request",
    "404 Not Found",
    "409 Conflict",
    "503 Service Unavailable",
    "No backend production readiness approval",
    "No fabricated evidence",
    "No secrets in repository",
    "No committed real patient data",
    "No direct mobile write to SQL Server",
    "No client may bypass the API",
    "No cloud dependency",
    "SQL Server remains the operational source of truth"
)

foreach ($Token in $DocumentationTokens) {
    Assert-ContainsToken -Content $AllContent -Token $Token
}

$ForbiddenTokens = @(
    "User ID=sa",
    "Password=",
    "Pwd=",
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

Write-Host "P5.5 patient API endpoint hardening verifier passed from repo root: $RepoRoot"