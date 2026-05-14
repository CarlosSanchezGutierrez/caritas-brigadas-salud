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

# Scope deliberately limited:
# Do not scan all docs/backend or all security tests because legitimate concepts such as
# LegacyRole, historical imports, and old external referral formats are allowed outside
# active P3 sync handler-extraction governance.
$TerminologyTargets = @(
    "docs/backend/P3_SYNC_PROCESSOR_COMPONENT_EXTRACTION_BASELINE.md",
    "docs/backend/P3_SYNC_PENDING_EVENT_DISPATCH_EXTRACTION_BASELINE.md",
    "docs/backend/P3_PATIENT_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md",
    "docs/backend/P3_PATIENT_VISIT_SYNC_EVENT_HANDLER_EXTRACTION_BASELINE.md",
    "docs/backend/P3_ZERO_TECHNICAL_DEBT_SYNC_PROCESSOR_BASELINE.md",
    "scripts/verify-p3-sync-processor-component-extraction.ps1",
    "scripts/verify-p3-sync-pending-event-dispatch-extraction.ps1",
    "scripts/verify-p3-patient-sync-event-handler-extraction.ps1",
    "scripts/verify-p3-patient-visit-sync-event-handler-extraction.ps1"
)

$ForbiddenMatches = @()

foreach ($RelativePath in $TerminologyTargets) {
    $Path = Join-Path $RepoRoot $RelativePath

    if (-not (Test-Path $Path)) {
        continue
    }

    $ForbiddenMatches += Select-String `
        -Path $Path `
        -Pattern "\blegacy\b" `
        -CaseSensitive:$false
}

if ($ForbiddenMatches) {
    $ForbiddenMatches | ForEach-Object {
        Write-Host "$($_.Path):$($_.LineNumber):$($_.Line)" -ForegroundColor Red
    }

    throw "Active P3 sync handler-extraction governance must use compatibility terminology instead of legacy terminology."
}

Write-Host "P3 sync compatibility governance verification passed." -ForegroundColor Green