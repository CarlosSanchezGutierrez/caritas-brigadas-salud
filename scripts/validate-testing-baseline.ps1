$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$WorkflowPath = Join-Path $RepoRoot ".github\workflows\verify.yml"
$PlaywrightConfigPath = Join-Path $RepoRoot "apps\web-next\playwright.config.ts"
$E2eSpecPath = Join-Path $RepoRoot "apps\web-next\e2e\app-shell.spec.ts"
$K6ScriptPath = Join-Path $RepoRoot "tests\load\api-smoke-load-test.js"
$E2eDocsPath = Join-Path $RepoRoot "docs\testing\e2e-testing-baseline.md"
$LoadDocsPath = Join-Path $RepoRoot "docs\testing\load-testing-baseline.md"
$PerformanceDocsPath = Join-Path $RepoRoot "docs\testing\performance-thresholds-baseline.md"
$RequiredChecksDocsPath = Join-Path $RepoRoot "docs\governance\required-checks-baseline.md"

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
Assert-FileExists $PlaywrightConfigPath
Assert-FileExists $E2eSpecPath
Assert-FileExists $K6ScriptPath
Assert-FileExists $E2eDocsPath
Assert-FileExists $LoadDocsPath
Assert-FileExists $PerformanceDocsPath
Assert-FileExists $RequiredChecksDocsPath

Assert-Contains $WorkflowPath "Testing baseline metadata gate"
Assert-Contains $WorkflowPath "pwsh scripts/validate-testing-baseline.ps1"

Assert-Contains $PlaywrightConfigPath "defineConfig"
Assert-Contains $PlaywrightConfigPath "webServer"
Assert-Contains $PlaywrightConfigPath "trace"

Assert-Contains $E2eSpecPath "mockCaritasApi"
Assert-Contains $E2eSpecPath "Dashboard institucional"
Assert-Contains $E2eSpecPath "Exportar CSV"

Assert-Contains $K6ScriptPath "http_req_failed"
Assert-Contains $K6ScriptPath "http_req_duration"
Assert-Contains $K6ScriptPath "BASE_URL"

Assert-Contains $E2eDocsPath "Playwright"
Assert-Contains $E2eDocsPath "mocked API"
Assert-Contains $E2eDocsPath "no PHI"

Assert-Contains $LoadDocsPath "k6"
Assert-Contains $LoadDocsPath "manual"
Assert-Contains $LoadDocsPath "thresholds"

Assert-Contains $PerformanceDocsPath "p95"
Assert-Contains $PerformanceDocsPath "error rate"

Assert-Contains $RequiredChecksDocsPath "Testing baseline metadata gate"

Write-Host ""
Write-Host "============================================================" -ForegroundColor Green
Write-Host "TESTING BASELINE VALIDATION PASO CORRECTAMENTE" -ForegroundColor Green
Write-Host "============================================================" -ForegroundColor Green
