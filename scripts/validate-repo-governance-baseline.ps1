$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$WorkflowPath = Join-Path $RepoRoot ".github\workflows\verify.yml"
$PrTemplatePath = Join-Path $RepoRoot ".github\pull_request_template.md"
$CodeownersPath = Join-Path $RepoRoot ".github\CODEOWNERS"
$RepoGovernanceDocsPath = Join-Path $RepoRoot "docs\governance\repository-governance-baseline.md"
$BranchProtectionDocsPath = Join-Path $RepoRoot "docs\governance\branch-protection-baseline.md"
$RequiredChecksDocsPath = Join-Path $RepoRoot "docs\governance\required-checks-baseline.md"
$ReleaseGovernanceDocsPath = Join-Path $RepoRoot "docs\governance\release-governance-baseline.md"
$ReleaseChecklistDocsPath = Join-Path $RepoRoot "docs\operations\release-checklist.md"
$P3DecisionRegisterScriptPath = Join-Path $RepoRoot "scripts\verify-p3-architecture-business-rules-decision-register.ps1"
$P3TenantBoundaryInventoryScriptPath = Join-Path $RepoRoot "scripts\verify-p3-tenant-boundary-authorization-inventory.ps1"
$P3ClinicalBusinessRulesScriptPath = Join-Path $RepoRoot "scripts\verify-p3-clinical-business-rules-baseline.ps1"
$P3ClinicalDataGovernancePrivacyAnalyticsScriptPath = Join-Path $RepoRoot "scripts\verify-p3-clinical-data-governance-privacy-analytics-baseline.ps1"
$P3OperationalRolesPanelsAnalyticsAccessMatrixScriptPath = Join-Path $RepoRoot "scripts\verify-p3-operational-roles-panels-analytics-access-matrix.ps1"
$P3OfflineSyncConflictPolicyScriptPath = Join-Path $RepoRoot "scripts\verify-p3-offline-sync-conflict-policy-baseline.ps1"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
    }
}

function Assert-Contains {
    param(
        [string]$Path,
        [string]$Token
    )

    $Content = Get-Content $Path -Raw

    if ($Content.IndexOf($Token, [StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "$Path does not contain required token: $Token"
    }
}

Assert-FileExists $WorkflowPath
Assert-FileExists $PrTemplatePath
Assert-FileExists $CodeownersPath
Assert-FileExists $RepoGovernanceDocsPath
Assert-FileExists $BranchProtectionDocsPath
Assert-FileExists $RequiredChecksDocsPath
Assert-FileExists $ReleaseGovernanceDocsPath
Assert-FileExists $ReleaseChecklistDocsPath
Assert-FileExists $P3DecisionRegisterScriptPath
Assert-FileExists $P3TenantBoundaryInventoryScriptPath
Assert-FileExists $P3ClinicalBusinessRulesScriptPath
Assert-FileExists $P3ClinicalDataGovernancePrivacyAnalyticsScriptPath
Assert-FileExists $P3OperationalRolesPanelsAnalyticsAccessMatrixScriptPath
Assert-FileExists $P3OfflineSyncConflictPolicyScriptPath

Assert-Contains $WorkflowPath "Repository governance metadata gate"
Assert-Contains $WorkflowPath "pwsh scripts/validate-repo-governance-baseline.ps1"

Assert-Contains $PrTemplatePath "Security checklist"
Assert-Contains $PrTemplatePath "Database checklist"
Assert-Contains $PrTemplatePath "GitHub Actions Verify passed"

Assert-Contains $CodeownersPath "@CarlosSanchezGutierrez"

Assert-Contains $RepoGovernanceDocsPath "pull request"
Assert-Contains $RepoGovernanceDocsPath "CODEOWNERS"
Assert-Contains $RepoGovernanceDocsPath "no direct pushes"

Assert-Contains $BranchProtectionDocsPath "develop"
Assert-Contains $BranchProtectionDocsPath "main"
Assert-Contains $BranchProtectionDocsPath "required checks"

Assert-Contains $RequiredChecksDocsPath "Backend security and quality gate"
Assert-Contains $RequiredChecksDocsPath "Frontend security and quality gate"
Assert-Contains $RequiredChecksDocsPath "Docker image build gate"

Assert-Contains $ReleaseGovernanceDocsPath "semantic versioning"
Assert-Contains $ReleaseGovernanceDocsPath "release notes"
Assert-Contains $ReleaseGovernanceDocsPath "rollback"

Assert-Contains $ReleaseChecklistDocsPath "pre-release"
Assert-Contains $ReleaseChecklistDocsPath "post-release"
Assert-Contains $ReleaseChecklistDocsPath "production approval"

& $P3DecisionRegisterScriptPath
& $P3TenantBoundaryInventoryScriptPath
& $P3ClinicalBusinessRulesScriptPath
& $P3ClinicalDataGovernancePrivacyAnalyticsScriptPath
& $P3OperationalRolesPanelsAnalyticsAccessMatrixScriptPath
& $P3OfflineSyncConflictPolicyScriptPath

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "REPOSITORY GOVERNANCE BASELINE VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
