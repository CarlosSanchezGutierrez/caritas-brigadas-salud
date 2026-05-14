$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_COMPATIBILITY_GOVERNANCE_BASELINE.md"

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

Assert-FileExists $DocPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Compatibility Governance Baseline",
    "compatibility governance",
    "not accepted technical debt",
    "Compatibility governance means",
    "Zero technical debt interpretation",
    "Backend closure path",
    "PatientSyncEventHandler",
    "PatientVisitSyncEventHandler",
    "ServiceEncounterSyncEventHandler",
    "MedicationDeliverySyncEventHandler"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync compatibility governance baseline"
}

$SearchRoots = @(
    Join-Path $RepoRoot "docs/backend",
    Join-Path $RepoRoot "scripts",
    Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Security"
)

$ForbiddenMatches = @()

foreach ($Root in $SearchRoots) {
    if (-not (Test-Path $Root)) {
        continue
    }

    $ForbiddenMatches += Get-ChildItem $Root -Recurse -File |
        Where-Object { $_.Extension -in @(".md", ".ps1", ".cs") } |
        Select-String -Pattern "\blegacy\b" -CaseSensitive:$false
}

if ($ForbiddenMatches) {
    $ForbiddenMatches | ForEach-Object {
        Write-Host "$($_.Path):$($_.LineNumber):$($_.Line)" -ForegroundColor Red
    }

    throw "P3 sync governance must use compatibility terminology instead of legacy terminology."
}

Write-Host "P3 sync compatibility governance verification passed." -ForegroundColor Green