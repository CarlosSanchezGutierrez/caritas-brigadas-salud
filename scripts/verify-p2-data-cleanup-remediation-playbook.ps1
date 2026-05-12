$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$PlaybookPath = Join-Path $RepoRoot "docs/backend/P2_DATA_CLEANUP_REMEDIATION_PLAYBOOK.md"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
    }
}

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if (-not $Content.Contains($Token)) {
        throw "$Label does not contain required token: $Token"
    }
}

Assert-FileExists $PlaybookPath

$Playbook = Get-Content $PlaybookPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P2 Data Cleanup Remediation Playbook",
    "Do not apply P2 FK migrations when orphan counts are greater than zero",
    "Do not delete clinical, consent, document, audit, or sync data without explicit authorization",
    "## 4. Remediation decision tree",
    "## 6. Repair script requirements",
    "~~~sql",
    "~~~text",
    "## 7. Required post-remediation validation",
    "## 8. Migration readiness rule",
    "## 9. Evidence template",
    "## 10. Final rule"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $Playbook $Token "P2 data cleanup remediation playbook"
}

$FenceCount = ([regex]::Matches($Playbook, "~~~")).Count

if (($FenceCount % 2) -ne 0) {
    throw "P2 data cleanup remediation playbook has unbalanced Markdown code fences."
}

if ($Playbook -notmatch "(?s)~~~sql.*?~~~\s*---\s*## 7\. Required post-remediation validation") {
    throw "SQL repair-script code fence must close before section 7."
}

if ($Playbook -notmatch "(?s)~~~text.*?~~~\s*---\s*## 10\. Final rule") {
    throw "Evidence-template code fence must close before section 10."
}

Write-Host "P2 data cleanup remediation playbook verification passed." -ForegroundColor Green