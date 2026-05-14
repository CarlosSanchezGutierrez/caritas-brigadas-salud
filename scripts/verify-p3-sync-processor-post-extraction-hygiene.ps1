$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_POST_EXTRACTION_HYGIENE_BASELINE.md"
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
    "P3 Sync Processor Post-Extraction Hygiene Baseline",
    "SyncBatchProcessor must not contain stale request contract usings for extracted handlers",
    "SyncBatchProcessor must not contain GenerateSyncPatientFolio",
    "SyncBatchProcessor must not contain ParseSex",
    "P3-22N does not remove temporary compatibility wrappers",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor post-extraction hygiene baseline"
}

$RequiredProcessorTokens = @(
    "private readonly PatientSyncEventHandler _patientSyncEventHandler;",
    "private readonly PatientVisitSyncEventHandler _patientVisitSyncEventHandler;",
    "private readonly ServiceEncounterSyncEventHandler _serviceEncounterSyncEventHandler;",
    "private readonly VitalSignsSyncEventHandler _vitalSignsSyncEventHandler;",
    "private readonly FormResponseSyncEventHandler _formResponseSyncEventHandler;",
    "private readonly ConsentDocumentSyncEventHandler _consentDocumentSyncEventHandler;",
    "private readonly MedicalReferralSyncEventHandler _medicalReferralSyncEventHandler;",
    "private readonly MedicationDeliverySyncEventHandler _medicationDeliverySyncEventHandler;",
    ".OrderBy(SyncProcessingOrder.GetOrder)",
    "var reservationState = new PendingBatchReservationState();",
    "TryValidateEvent(syncEvent, out var rejectionReason)",
    "JsonDocument.Parse(syncEvent.PayloadJson)"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor post-extraction hygiene"
}

$ForbiddenProcessorTokens = @(
    "using Caritas.Brigadas.Contracts.Patients;",
    "using Caritas.Brigadas.Contracts.PatientVisits;",
    "using Caritas.Brigadas.Contracts.ServiceEncounters;",
    "using Caritas.Brigadas.Contracts.FormResponses;",
    "using Caritas.Brigadas.Contracts.VitalSigns;",
    "GenerateSyncPatientFolio",
    "private static Sex ParseSex",
    "return Sex.NotSpecified;",
    '"male" or "masculino" or "m"',
    '"female" or "femenino" or "f"'
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains post-extraction residue: $Token"
    }
}

$ForbiddenPatterns = @(
    '(?m)[ \t]+$',
    '(\r?\n){4,}',
    '(?m)^\s*}\r?\n    private async Task Handle',
    '(?m)^private async Task Handle[A-Za-z]+EventAsync',
    '(?m)^var\s+',
    '(?m)^if\s*\(',
    '(?m)^await\s+',
    '(?m)^return;'
)

foreach ($Pattern in $ForbiddenPatterns) {
    if ($Processor -match $Pattern) {
        throw "SyncBatchProcessor post-extraction hygiene violation: $Pattern"
    }
}

Write-Host "P3 sync processor post-extraction hygiene verification passed." -ForegroundColor Green
