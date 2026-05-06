$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$WorkflowPath = Join-Path $RepoRoot ".github\workflows\verify.yml"
$ContainerScanningDocsPath = Join-Path $RepoRoot "docs\security\container-image-scanning-and-sbom-baseline.md"
$SupplyChainDocsPath = Join-Path $RepoRoot "docs\security\software-supply-chain-security-baseline.md"
$ImageReleaseDocsPath = Join-Path $RepoRoot "docs\operations\container-image-release-strategy.md"

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
Assert-FileExists $ContainerScanningDocsPath
Assert-FileExists $SupplyChainDocsPath
Assert-FileExists $ImageReleaseDocsPath

Assert-Contains $WorkflowPath "Container image vulnerability scan"
Assert-Contains $WorkflowPath "aquasecurity/trivy-action@0.35.0"
Assert-Contains $WorkflowPath "severity: CRITICAL,HIGH"
Assert-Contains $WorkflowPath "exit-code: 1"
Assert-Contains $WorkflowPath "Create SBOM artifact directory"
Assert-Contains $WorkflowPath "mkdir -p artifacts/sbom"
Assert-Contains $WorkflowPath "Generate container SBOM"
Assert-Contains $WorkflowPath "anchore/sbom-action@v0"
Assert-Contains $WorkflowPath "actions/upload-artifact@v7"
Assert-Contains $WorkflowPath "caritas-brigadas-api:ci"

Assert-Contains $ContainerScanningDocsPath "Trivy"
Assert-Contains $ContainerScanningDocsPath "SBOM"
Assert-Contains $ContainerScanningDocsPath "CRITICAL,HIGH"
Assert-Contains $ContainerScanningDocsPath "no debe desplegarse"

Assert-Contains $SupplyChainDocsPath "dependency pinning"
Assert-Contains $SupplyChainDocsPath "SBOM"
Assert-Contains $SupplyChainDocsPath "container image"

Assert-Contains $ImageReleaseDocsPath "latest"
Assert-Contains $ImageReleaseDocsPath "commit SHA"
Assert-Contains $ImageReleaseDocsPath "rollback"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "SUPPLY CHAIN BASELINE VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
