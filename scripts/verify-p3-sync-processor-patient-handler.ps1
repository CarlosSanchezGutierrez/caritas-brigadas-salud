$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_PATIENT_HANDLER_BASELINE.md"
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
    "P3 Sync Processor Patient Handler Baseline",
    "EntityType: patient",
    "Operation: create",
    "parse PayloadJson as CreatePatientRequest",
    "conflict duplicate PatientFolio inside the organization",
    "duplicate PatientFolio values inside the same pending batch",
    "set SyncEvent.EntityId to the created Patient.Id",
    "patient update is not implemented in P3-13",
    "Acceptance criteria",
    "P3-14 patient visit handler note",
    "P3-15 vital signs handler note",
    "P3-16 service encounter handler note",
    "P3-17 form response handler note",
    "P3-18 consent document handler note"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor patient handler baseline"
}

$RequiredProcessorTokens = @(
    "HandlePatientEventAsync",
    "syncEvent.EntityType == SyncEntityType.Patient",
    "syncEvent.Operation != SyncOperation.Create",
    "patient_operation_not_implemented",
    "JsonSerializer.Deserialize<CreatePatientRequest>",
    "new Patient(",
    "_dbContext.Patients.Add(patient)",
    "syncEvent.Accept(",
    "patient.Id",
    "patient_folio_already_exists",
    "patient_folio_duplicate_in_pending_batch",
    "acceptedPatientFoliosInBatch",
    "acceptedPatientFoliosInBatch.Contains(normalizedFolio)",
    "!acceptedPatientFoliosInBatch.Add(normalizedFolio)",
    "GenerateSyncPatientFolio",
    "ParseSex"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor patient handler"
}

$ForbiddenProcessorTokens = @(
    "_dbContext.MedicationDeliveries.Add"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor patient handler scope violation: $Token"
    }
}

Write-Host "P3 sync processor patient handler verification passed." -ForegroundColor Green