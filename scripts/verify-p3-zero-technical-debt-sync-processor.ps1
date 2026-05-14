$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_ZERO_TECHNICAL_DEBT_SYNC_PROCESSOR_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$ServiceHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/ServiceEncounterSyncEventHandler.cs"
$FormHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/FormResponseSyncEventHandler.cs"
$ConsentHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/ConsentDocumentSyncEventHandler.cs"
$ReferralHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/MedicalReferralSyncEventHandler.cs"

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
Assert-FileExists $ProcessorPath
Assert-FileExists $ServiceHandlerPath
Assert-FileExists $FormHandlerPath
Assert-FileExists $ConsentHandlerPath
Assert-FileExists $ReferralHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$ServiceHandler = Get-Content $ServiceHandlerPath -Raw -Encoding UTF8
$FormHandler = Get-Content $FormHandlerPath -Raw -Encoding UTF8
$ConsentHandler = Get-Content $ConsentHandlerPath -Raw -Encoding UTF8
$ReferralHandler = Get-Content $ReferralHandlerPath -Raw -Encoding UTF8
$ProcessorAndServiceHandler = $Processor + $ServiceHandler + $FormHandler + $ConsentHandler + $ReferralHandler

$RequiredDocTokens = @(
    "P3 Zero Technical Debt Sync Processor Baseline",
    "This baseline does not permit technical debt",
    "no new sync entity handlers may be added directly to SyncBatchProcessor before decomposition",
    "SyncBatchProcessor must keep no more than the current eight domain event handlers",
    "extract sync processing order into a dedicated internal component",
    "extract pending-batch reservation state into a dedicated internal component",
    "extract payload parsing/validation into a dedicated internal component",
    "extract each domain handler into a dedicated internal handler class",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 zero technical debt sync processor baseline"
}

$RequiredProcessorTokens = @(
    "private static int GetSyncProcessingOrder(SyncEvent syncEvent)",
    "HandlePatientEventAsync",
    "HandlePatientVisitEventAsync",
    "HandleServiceEncounterEventAsync",
    "HandleVitalSignsEventAsync",
    "HandleFormResponseEventAsync",
    "HandleConsentDocumentEventAsync",
    "HandleMedicalReferralEventAsync",
    "HandleMedicationDeliveryEventAsync",
    "reserved only after successful ServiceEncounter construction and reserved atomically",
    "reserved only after successful FormResponse construction and reserved atomically",
    "reserved only after successful ConsentDocument construction and reserved atomically",
    "reserved only after successful MedicalReferral construction and reserved atomically",
    "Medication delivery id duplicate checks include globally duplicated ids because primary key uniqueness is not tenant-scoped"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $ProcessorAndServiceHandler $Token "SyncBatchProcessor zero technical debt guard"
}

$ForbiddenProcessorPatterns = @(
    '(?m)[ \t]+$',
    '(?m)^ {20,}// Pending-batch',
    'TODO',
    'HACK',
    'quick fix',
    'temporary workaround',
    'technical debt accepted'
)

foreach ($Pattern in $ForbiddenProcessorPatterns) {
    if ($Processor -match $Pattern) {
        throw "SyncBatchProcessor contains forbidden zero-debt pattern: $Pattern"
    }
}

$HandlerCount = ([regex]::Matches($Processor, 'private async Task Handle[A-Za-z]+EventAsync')).Count

if ($HandlerCount -gt 8) {
    throw "SyncBatchProcessor has $HandlerCount handlers. No new handlers are allowed before decomposition."
}

Write-Host "P3 zero technical debt sync processor verification passed." -ForegroundColor Green