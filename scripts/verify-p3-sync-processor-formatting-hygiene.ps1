$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_FORMATTING_HYGIENE_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"

$HandlerPaths = @(
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/PatientSyncEventHandler.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/PatientVisitSyncEventHandler.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/ServiceEncounterSyncEventHandler.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/VitalSignsSyncEventHandler.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/FormResponseSyncEventHandler.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/ConsentDocumentSyncEventHandler.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/MedicalReferralSyncEventHandler.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/MedicationDeliverySyncEventHandler.cs"
)

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
$Handlers = ""

foreach ($RelativePath in $HandlerPaths) {
    $Path = Join-Path $RepoRoot $RelativePath
    Assert-FileExists $Path
    $Handlers += Get-Content $Path -Raw -Encoding UTF8
}

$ProcessorAndHandlers = $Processor + $Handlers

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
    ".OrderBy(SyncProcessingOrder.GetOrder)",
    "await _patientSyncEventHandler.HandleAsync(",
    "await _patientVisitSyncEventHandler.HandleAsync(",
    "await _serviceEncounterSyncEventHandler.HandleAsync(",
    "await _vitalSignsSyncEventHandler.HandleAsync(",
    "await _formResponseSyncEventHandler.HandleAsync(",
    "await _consentDocumentSyncEventHandler.HandleAsync(",
    "await _medicalReferralSyncEventHandler.HandleAsync(",
    "await _medicationDeliverySyncEventHandler.HandleAsync(",
    "var reservationState = new PendingBatchReservationState();"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor formatting hygiene"
}

$RequiredHandlerTokens = @(
    "SyncPayloadReader.TryReadObject",
    "PatientSyncEventHandler",
    "PatientVisitSyncEventHandler",
    "ServiceEncounterSyncEventHandler",
    "VitalSignsSyncEventHandler",
    "FormResponseSyncEventHandler",
    "ConsentDocumentSyncEventHandler",
    "MedicalReferralSyncEventHandler",
    "MedicationDeliverySyncEventHandler"
)

foreach ($Token in $RequiredHandlerTokens) {
    Assert-Contains $ProcessorAndHandlers $Token "P3 sync handler formatting hygiene"
}

Write-Host "P3 sync processor formatting hygiene verification passed." -ForegroundColor Green
