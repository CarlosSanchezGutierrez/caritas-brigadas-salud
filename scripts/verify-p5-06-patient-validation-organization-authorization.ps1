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
    "services/api-dotnet/src/Caritas.Brigadas.Application/Patients/IPatientReadRepository.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientReadRepository.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientWriteRepository.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Api/Controllers/PatientsController.cs",
    "docs/implementation/P5_06_PATIENT_VALIDATION_ORGANIZATION_AUTHORIZATION.md",
    "docs/qa/P5_06_PATIENT_VALIDATION_ORGANIZATION_AUTHORIZATION_MATRIX.md",
    "docs/runbooks/P5_06_PATIENT_VALIDATION_ORGANIZATION_AUTHORIZATION_RUNBOOK.md",
    "scripts/verify-p5-06-patient-validation-organization-authorization.ps1"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$InterfaceContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Application/Patients/IPatientReadRepository.cs"
$ReadRepositoryContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientReadRepository.cs"
$WriteRepositoryContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientWriteRepository.cs"
$ControllerContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Api/Controllers/PatientsController.cs"
$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P5_06_PATIENT_VALIDATION_ORGANIZATION_AUTHORIZATION.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P5_06_PATIENT_VALIDATION_ORGANIZATION_AUTHORIZATION_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P5_06_PATIENT_VALIDATION_ORGANIZATION_AUTHORIZATION_RUNBOOK.md"

$AllContent = $InterfaceContent + "`n" + $ReadRepositoryContent + "`n" + $WriteRepositoryContent + "`n" + $ControllerContent + "`n" + $ImplementationContent + "`n" + $MatrixContent + "`n" + $RunbookContent

$RequiredTokens = @(
    "Task<PatientSummaryDto?> GetByIdAsync(",
    "Guid organizationId,",
    "patient.OrganizationId == organizationId",
    "repository.GetByIdAsync(",
    "ArgumentNullException.ThrowIfNull(request);",
    "ValidateCreateRequest(request);",
    "At least one patient identity field is required.",
    "Partial record reason is required when patient record is marked as partial.",
    "Source brigade id cannot be empty.",
    "_dbContext.Brigades",
    "brigade.OrganizationId == organizationId",
    "Source brigade was not found for the organization.",
    "P5.6 Patient Validation and Organization Authorization",
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "No backend production readiness approval",
    "No fabricated evidence",
    "No secrets in repository",
    "No committed real patient data",
    "No direct mobile write to SQL Server",
    "No client may bypass the API",
    "No cloud dependency",
    "SQL Server remains the operational source of truth"
)

foreach ($Token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllContent -Token $Token
}

$ForbiddenTokens = @(
    "patient.OrganizationId != organizationId",
    "mobile clients may write directly to SQL Server",
    "frontend may bypass API",
    "backend is production ready",
    "backend production readiness is approved",
    "Cloud is required",
    "Azure is required",
    "AWS is required",
    "User ID=sa",
    "Password=",
    "Pwd="
)

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllContent -Token $Token
}

Write-Host "P5.6 patient validation and organization authorization verifier passed from repo root: $RepoRoot"