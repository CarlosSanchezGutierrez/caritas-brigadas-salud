$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_FORMATTING_HYGIENE_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"

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

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8

$RequiredDocTokens = @(
    "P3 Sync Processor Formatting Hygiene Baseline",
    "SyncBatchProcessor must not contain trailing whitespace",
    "SyncBatchProcessor handler methods must not start at column 1",
    "SyncBatchProcessor must not contain unindented local var declarations at column 1",
    "SyncBatchProcessor must not contain unindented if statements at column 1",
    "SyncBatchProcessor must not contain method declarations glued directly after a closing brace",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor formatting hygiene baseline"
}

$ForbiddenPatterns = @(
    '(?m)[ \t]+$',
    '(?m)^private async Task Handle[A-Za-z]+EventAsync',
    '(?m)^var\s+',
    '(?m)^if\s*\(',
    '(?m)^\s*}\r?\nprivate async Task',
    '(?m)^await\s+',
    '(?m)^return;'
)

foreach ($Pattern in $ForbiddenPatterns) {
    if ($Processor -match $Pattern) {
        throw "SyncBatchProcessor formatting hygiene violation: $Pattern"
    }
}

$RequiredProcessorTokens = @(
    "    private async Task HandlePatientEventAsync",
    "    private async Task HandlePatientVisitEventAsync",
    "    private async Task HandleServiceEncounterEventAsync",
    "    private async Task HandleVitalSignsEventAsync",
    "    private async Task HandleFormResponseEventAsync",
    "    private async Task HandleConsentDocumentEventAsync",
    "    private async Task HandleMedicalReferralEventAsync",
    "    private async Task HandleMedicationDeliveryEventAsync",
    "SyncPayloadReader.TryReadObject",
    ".OrderBy(SyncProcessingOrder.GetOrder)",
    "var reservationState = new PendingBatchReservationState();"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor formatting hygiene"
}

Write-Host "P3 sync processor formatting hygiene verification passed." -ForegroundColor Green