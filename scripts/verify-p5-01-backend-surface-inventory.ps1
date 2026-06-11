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
"docs/implementation/P5_01_BACKEND_SURFACE_INVENTORY.md",
"docs/qa/P5_01_BACKEND_SURFACE_INVENTORY_MATRIX.md",
"docs/runbooks/P5_01_BACKEND_SURFACE_INVENTORY_RUNBOOK.md",
"scripts/p5/collect-p5-01-backend-surface-inventory.ps1",
"scripts/verify-p5-01-backend-surface-inventory.ps1"
)

foreach ($File in $RequiredFiles) {
Assert-FileExists -RelativePath $File
}

$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P5_01_BACKEND_SURFACE_INVENTORY.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P5_01_BACKEND_SURFACE_INVENTORY_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P5_01_BACKEND_SURFACE_INVENTORY_RUNBOOK.md"
$CollectorContent = Read-RepoText -RelativePath "scripts/p5/collect-p5-01-backend-surface-inventory.ps1"

$AllContent = $ImplementationContent + "n" + $MatrixContent + "n" + $RunbookContent + "`n" + $CollectorContent

$RequiredTokens = @(
"P5.1 Backend Surface Inventory",
"Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
"patient core",
"brigade",
"clinical encounters",
"consent and privacy",
"longitudinal history",
"offline-first synchronization",
"dashboards",
"analytics",
"reports",
"exports",
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
"collect-p5-01-backend-surface-inventory",
"project-inventory.json",
"source-surface-inventory.json",
"domain-coverage.json",
"backend-surface-summary.md",
"gap-backlog.md",
"manifest.json",
"patient_core",
"brigade_core",
"clinical_encounter",
"consent_privacy",
"longitudinal_history",
"offline_first",
"dashboards",
"analytics",
"reports_exports",
"audit_trail",
"authorization",
"sql_server",
"offline_first_required_for_final_system",
"dashboards_required_for_final_system",
"analytics_required_for_final_system",
"longitudinal_history_required_for_final_system"
)

foreach ($Token in $CollectorTokens) {
Assert-ContainsToken -Content $CollectorContent -Token $Token
}

$ForbiddenTokens = @(
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

Write-Host "P5.1 backend surface inventory verifier passed from repo root: $RepoRoot"