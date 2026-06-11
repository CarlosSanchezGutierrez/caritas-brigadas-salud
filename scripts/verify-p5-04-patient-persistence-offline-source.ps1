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
"services/api-dotnet/src/Caritas.Brigadas.Domain/Entities/Patient.cs",
"services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/CaritasDbContext.cs",
"services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientWriteRepository.cs",
"services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientReadRepository.cs",
"docs/implementation/P5_04_PATIENT_PERSISTENCE_OFFLINE_SOURCE.md",
"docs/qa/P5_04_PATIENT_PERSISTENCE_OFFLINE_SOURCE_MATRIX.md",
"docs/runbooks/P5_04_PATIENT_PERSISTENCE_OFFLINE_SOURCE_RUNBOOK.md",
"scripts/verify-p5-04-patient-persistence-offline-source.ps1"
)

foreach ($File in $RequiredFiles) {
Assert-FileExists -RelativePath $File
}

$PatientContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Domain/Entities/Patient.cs"
$DbContextContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/CaritasDbContext.cs"
$WriteRepositoryContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientWriteRepository.cs"
$ReadRepositoryContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientReadRepository.cs"
$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P5_04_PATIENT_PERSISTENCE_OFFLINE_SOURCE.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P5_04_PATIENT_PERSISTENCE_OFFLINE_SOURCE_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P5_04_PATIENT_PERSISTENCE_OFFLINE_SOURCE_RUNBOOK.md"

$AllContent = $PatientContent + "n" + $DbContextContent + "n" + $WriteRepositoryContent + "n" + $ReadRepositoryContent + "n" + $ImplementationContent + "n" + $MatrixContent + "n" + $RunbookContent

$PatientFieldTokens = @(
"SourceBrigadeId",
"LocalPatientId",
"ClientOperationId",
"IdempotencyKey",
"SyncStatus",
"DataCaptureSource"
)

foreach ($Token in $PatientFieldTokens) {
Assert-ContainsToken -Content $PatientContent -Token $Token
Assert-ContainsToken -Content $DbContextContent -Token $Token
Assert-ContainsToken -Content $WriteRepositoryContent -Token $Token
Assert-ContainsToken -Content $ReadRepositoryContent -Token $Token
}

Assert-ContainsToken -Content $PatientContent -Token "UpdateOfflineSourceMetadata"
Assert-ContainsToken -Content $WriteRepositoryContent -Token "UpdateOfflineSourceMetadata"
Assert-ContainsToken -Content $DbContextContent -Token "x.OrganizationId, x.SourceBrigadeId"
Assert-ContainsToken -Content $DbContextContent -Token "x.OrganizationId, x.ClientOperationId"
Assert-ContainsToken -Content $DbContextContent -Token "x.OrganizationId, x.IdempotencyKey"

$MigrationDirectory = Resolve-RepoPath -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Migrations"
$MigrationFiles = @(Get-ChildItem -Path $MigrationDirectory -Filter "*AddPatientOfflineSourceFields.cs" -File -ErrorAction SilentlyContinue)

if ($MigrationFiles.Count -eq 0) {
throw "Missing migration file for AddPatientOfflineSourceFields."
}

$MigrationContent = [System.IO.File]::ReadAllText($MigrationFiles[0].FullName)

foreach ($Token in $PatientFieldTokens) {
Assert-ContainsToken -Content $MigrationContent -Token $Token
}

$DocumentationTokens = @(
"P5.4 Patient Persistence Offline Source",
"Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
"SQL Server remains the operational source of truth",
"No backend production readiness approval",
"No fabricated evidence",
"No secrets in repository",
"No committed real patient data",
"No direct mobile write to SQL Server",
"No client may bypass the API",
"No cloud dependency"
)

foreach ($Token in $DocumentationTokens) {
Assert-ContainsToken -Content $AllContent -Token $Token
}

$ForbiddenTokens = @(
"User ID=sa",
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

Write-Host "P5.4 patient persistence offline source verifier passed from repo root: $RepoRoot"