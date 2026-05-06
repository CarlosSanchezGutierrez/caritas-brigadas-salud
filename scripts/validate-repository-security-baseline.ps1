$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DependencyReviewWorkflowPath = Join-Path $RepoRoot ".github\workflows\dependency-review.yml"
$DependencyReviewRestScriptPath = Join-Path $RepoRoot "scripts\dependency-review-rest.ps1"
$DependencyReviewDocsPath = Join-Path $RepoRoot "docs\security\dependency-review-baseline.md"
$SecretScanningDocsPath = Join-Path $RepoRoot "docs\security\secret-scanning-and-push-protection-baseline.md"
$RequiredChecksDocsPath = Join-Path $RepoRoot "docs\governance\required-checks-baseline.md"
$BranchProtectionDocsPath = Join-Path $RepoRoot "docs\governance\branch-protection-baseline.md"

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

Assert-FileExists $DependencyReviewWorkflowPath
Assert-FileExists $DependencyReviewRestScriptPath
Assert-FileExists $DependencyReviewDocsPath
Assert-FileExists $SecretScanningDocsPath
Assert-FileExists $RequiredChecksDocsPath
Assert-FileExists $BranchProtectionDocsPath

Assert-Contains $DependencyReviewWorkflowPath "Repository Security"
Assert-Contains $DependencyReviewWorkflowPath "Repository security metadata gate"
Assert-Contains $DependencyReviewWorkflowPath "Dependency Review"
Assert-Contains $DependencyReviewWorkflowPath "scripts/dependency-review-rest.ps1"
Assert-Contains $DependencyReviewWorkflowPath "FAIL_ON_SEVERITY: high"
Assert-Contains $DependencyReviewWorkflowPath "pull_request"

Assert-Contains $DependencyReviewRestScriptPath "dependency-graph/compare"
Assert-Contains $DependencyReviewRestScriptPath "vulnerabilities"
Assert-Contains $DependencyReviewRestScriptPath "FAIL_ON_SEVERITY"
Assert-Contains $DependencyReviewRestScriptPath "Blocking vulnerabilities"

Assert-Contains $DependencyReviewDocsPath "Dependency Review REST API"
Assert-Contains $DependencyReviewDocsPath "high y critical"
Assert-Contains $DependencyReviewDocsPath "No ejecuta OpenSSF Scorecard"

Assert-Contains $SecretScanningDocsPath "Secret scanning"
Assert-Contains $SecretScanningDocsPath "Push protection"
Assert-Contains $SecretScanningDocsPath "no secrets"
Assert-Contains $SecretScanningDocsPath "GitHub Settings"

Assert-Contains $RequiredChecksDocsPath "Repository security metadata gate"
Assert-Contains $RequiredChecksDocsPath "Dependency Review"

Assert-Contains $BranchProtectionDocsPath "Repository security metadata gate"
Assert-Contains $BranchProtectionDocsPath "Dependency Review"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "REPOSITORY SECURITY BASELINE VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
