$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PENDING_EVENT_DISPATCH_EXTRACTION_BASELINE.md"
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
    "P3 Sync Pending Event Dispatch Extraction Baseline",
    "ProcessAsync must call ProcessPendingEventAsync for each pending event",
    "ProcessAsync must not directly branch on SyncEntityType for handler dispatch",
    "ProcessPendingEventAsync must dispatch to the existing patient handler",
    "ProcessPendingEventAsync must dispatch to the existing medication delivery handler",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync pending event dispatch extraction baseline"
}

$RequiredProcessorTokens = @(
    "private async Task ProcessPendingEventAsync",
    "await ProcessPendingEventAsync(",
    "PendingBatchReservationState reservationState",
    "syncEvent.MarkProcessing();",
    "TryValidateEvent(syncEvent, out var rejectionReason)",
    "HandlePatientEventAsync",
    "HandlePatientVisitEventAsync",
    "HandleServiceEncounterEventAsync",
    "HandleVitalSignsEventAsync",
    "HandleFormResponseEventAsync",
    "HandleConsentDocumentEventAsync",
    "HandleMedicalReferralEventAsync",
    "HandleMedicationDeliveryEventAsync",
    "SkeletonConflictReason"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor pending event dispatch extraction"
}

$ProcessAsyncStart = $Processor.IndexOf("public async Task<ProcessSyncBatchResultDto> ProcessAsync", [System.StringComparison]::Ordinal)
$DispatchStart = $Processor.IndexOf("private async Task ProcessPendingEventAsync", [System.StringComparison]::Ordinal)

if ($ProcessAsyncStart -lt 0 -or $DispatchStart -lt 0) {
    throw "Could not locate ProcessAsync or ProcessPendingEventAsync."
}

$ProcessAsyncSection = $Processor.Substring($ProcessAsyncStart, $DispatchStart - $ProcessAsyncStart)

$ForbiddenProcessAsyncTokens = @(
    "syncEvent.EntityType == SyncEntityType.Patient",
    "syncEvent.EntityType == SyncEntityType.PatientVisit",
    "syncEvent.EntityType == SyncEntityType.ServiceEncounter",
    "syncEvent.EntityType == SyncEntityType.VitalSigns",
    "syncEvent.EntityType == SyncEntityType.FormResponse",
    "syncEvent.EntityType == SyncEntityType.ConsentDocument",
    "syncEvent.EntityType == SyncEntityType.MedicalReferral",
    "syncEvent.EntityType == SyncEntityType.MedicationDelivery",
    "SkeletonConflictReason);"
)

foreach ($Token in $ForbiddenProcessAsyncTokens) {
    if ($ProcessAsyncSection.Contains($Token)) {
        throw "ProcessAsync still contains direct dispatch token: $Token"
    }
}

Write-Host "P3 sync pending event dispatch extraction verification passed." -ForegroundColor Green