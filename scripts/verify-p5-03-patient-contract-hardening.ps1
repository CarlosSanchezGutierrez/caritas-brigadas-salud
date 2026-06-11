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
"services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/CreatePatientRequest.cs",
"services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientSummaryDto.cs",
"services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientContractReadiness.cs",
"docs/implementation/P5_03_PATIENT_CONTRACT_HARDENING.md",
"docs/qa/P5_03_PATIENT_CONTRACT_HARDENING_MATRIX.md",
"docs/runbooks/P5_03_PATIENT_CONTRACT_HARDENING_RUNBOOK.md",
"scripts/verify-p5-03-patient-contract-hardening.ps1"
)

foreach ($File in $RequiredFiles) {
Assert-FileExists -RelativePath $File
}

$CreatePatientRequestContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/CreatePatientRequest.cs"
$PatientSummaryDtoContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientSummaryDto.cs"
$ReadinessContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientContractReadiness.cs"
$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P5_03_PATIENT_CONTRACT_HARDENING.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P5_03_PATIENT_CONTRACT_HARDENING_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P5_03_PATIENT_CONTRACT_HARDENING_RUNBOOK.md"

$AllContent = $CreatePatientRequestContent + "n" + $PatientSummaryDtoContent + "n" + $ReadinessContent + "n" + $ImplementationContent + "n" + $MatrixContent + "`n" + $RunbookContent

$ContractTokens = @(
"SourceBrigadeId",
"LocalPatientId",
"ClientOperationId",
"IdempotencyKey",
"SyncStatus",
"DataCaptureSource"
)

foreach ($Token in $ContractTokens) {
Assert-ContainsToken -Content $CreatePatientRequestContent -Token $Token
Assert-ContainsToken -Content $PatientSummaryDtoContent -Token $Token
}

$ReadinessTokens = @(
"PatientContractReadiness",
"PatientCoreRequiredForFinalSystem",
"OfflineFirstRequiredForFinalSystem",
"LongitudinalHistoryRequiredForFinalSystem",
"DashboardsRequiredForFinalSystem",
"AnalyticsRequiredForFinalSystem",
"RequiredOfflineCreateFields",
"RequiredLongitudinalLinkFields",
"RequiredFlexibleIdentityFields"
)

foreach ($Token in $ReadinessTokens) {
Assert-ContainsToken -Content $ReadinessContent -Token $Token
}

$DocumentationTokens = @(
"P5.3 Patient Contract Hardening",
"Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
"offline-first",
"longitudinal",
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

Write-Host "P5.3 patient contract hardening verifier passed from repo root: $RepoRoot"