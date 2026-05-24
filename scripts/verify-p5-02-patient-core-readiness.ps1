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
"docs/implementation/P5_02_PATIENT_CORE_READINESS.md",
"docs/qa/P5_02_PATIENT_CORE_READINESS_MATRIX.md",
"docs/runbooks/P5_02_PATIENT_CORE_READINESS_RUNBOOK.md",
"scripts/p5/collect-p5-02-patient-core-readiness.ps1",
"scripts/verify-p5-02-patient-core-readiness.ps1"
)

foreach ($File in $RequiredFiles) {
Assert-FileExists -RelativePath $File
}

$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P5_02_PATIENT_CORE_READINESS.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P5_02_PATIENT_CORE_READINESS_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P5_02_PATIENT_CORE_READINESS_RUNBOOK.md"
$CollectorContent = Read-RepoText -RelativePath "scripts/p5/collect-p5-02-patient-core-readiness.ps1"

$AllContent = $ImplementationContent + "n" + $MatrixContent + "n" + $RunbookContent + "`n" + $CollectorContent

$RequiredTokens = @(
"P5.2 Patient Core Readiness",
"Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
"patient creation",
"patient lookup",
"flexible identity",
"organization scoped access",
"audit trail",
"consent linkage",
"clinical encounter linkage",
"longitudinal history",
"offline-first",
"idempotency",
"SQL Server remains the operational source of truth",
"No backend production readiness approval",
"No fabricated evidence",
"No secrets in repository",
"No committed real patient data",
"No direct mobile write to SQL Server",
"No client may bypass the API",
"No cloud dependency"
)

foreach ($Token in $RequiredTokens) {
Assert-ContainsToken -Content $AllContent -Token $Token
}

$CollectorTokens = @(
"collect-p5-02-patient-core-readiness",
"patient-core-surface-inventory.json",
"patient-core-readiness-summary.md",
"patient-core-gap-backlog.md",
"manifest.json",
"patient_domain_surface",
"patient_identity_surface",
"patient_endpoint_surface",
"patient_persistence_surface",
"patient_validation_surface",
"patient_authorization_surface",
"patient_audit_surface",
"patient_test_surface",
"offline_patient_surface",
"longitudinal_patient_surface",
"patient_core_required_for_final_system",
"offline_first_required_for_final_system",
"longitudinal_history_required_for_final_system"
)

foreach ($Token in $CollectorTokens) {
Assert-ContainsToken -Content $CollectorContent -Token $Token
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

Write-Host "P5.2 patient core readiness verifier passed from repo root: $RepoRoot"