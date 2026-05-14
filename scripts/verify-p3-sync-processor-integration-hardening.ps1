$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PROCESSOR_INTEGRATION_HARDENING_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$SecurityTestsPath = Join-Path $RepoRoot "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Security"

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
    "P3 Sync Processor Integration Hardening Baseline",
    "Topological order contract",
    "Pending-batch reservation atomicity",
    "ServiceEncounter encounter folio and visit-service keys must be reserved only after successful ServiceEncounter construction and reserved atomically",
    "If ServiceEncounter visit-service key reservation fails, the encounter folio reservation must be rolled back",
    "FormResponse id and encounter-template keys must be reserved only after successful FormResponse construction and reserved atomically",
    "If FormResponse encounter-template key reservation fails, the form response id reservation must be rolled back",
    "Payload privacy",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync processor integration hardening baseline"
}

$RequiredProcessorTokens = @(
    "private static int GetSyncProcessingOrder(SyncEvent syncEvent)",
    "syncEvent.EntityType == SyncEntityType.Patient",
    "return 0;",
    "syncEvent.EntityType == SyncEntityType.PatientVisit",
    "return 1;",
    "syncEvent.EntityType == SyncEntityType.ServiceEncounter",
    "return 2;",
    "syncEvent.EntityType == SyncEntityType.VitalSigns",
    "return 3;",
    "syncEvent.EntityType == SyncEntityType.FormResponse",
    "return 4;",
    "syncEvent.EntityType == SyncEntityType.ConsentDocument",
    "return 5;",
    "syncEvent.EntityType == SyncEntityType.MedicalReferral",
    "return 6;",
    "syncEvent.EntityType == SyncEntityType.MedicationDelivery",
    "return 7;",
    "return 8;",
    "reserved only after successful ServiceEncounter construction and reserved atomically",
    "encounterFolioReserved",
    "encounterVisitServiceKeyReserved",
    "acceptedEncounterFoliosInBatch.Remove(normalizedEncounterFolio)",
    "reserved only after successful FormResponse construction and reserved atomically",
    "formResponseIdReserved",
    "formResponseEncounterTemplateKeyReserved",
    "acceptedFormResponseIdsInBatch.Remove(formResponseId)",
    "reserved only after successful ConsentDocument construction and reserved atomically",
    "consentDocumentIdReserved",
    "consentDocumentKeyReserved",
    "acceptedConsentDocumentIdsInBatch.Remove(consentDocumentId)",
    "reserved only after successful MedicalReferral construction and reserved atomically",
    "medicalReferralIdReserved",
    "medicalReferralFolioReserved",
    "acceptedMedicalReferralIdsInBatch.Remove(medicalReferralId)",
    "Medication delivery id duplicate checks include globally duplicated ids because primary key uniqueness is not tenant-scoped"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $Processor $Token "SyncBatchProcessor integration hardening"
}

$ForbiddenPatterns = @(
    'var\s+\w+\s*=\s*new\[\]\s*\{\s*\};',
    'Console\.WriteLine\(.*PayloadJson',
    'Log.*PayloadJson'
)

foreach ($Pattern in $ForbiddenPatterns) {
    if ($Processor -match $Pattern) {
        throw "SyncBatchProcessor contains forbidden integration hardening pattern: $Pattern"
    }
}

$BadTestArrays = Get-ChildItem $SecurityTestsPath -Filter "P3SyncProcessor*ContractTests.cs" |
    Select-String -Pattern 'var\s+\w+\s*=\s*new\[\]\s*\{\s*\};'

if ($BadTestArrays) {
    $BadTestArrays | ForEach-Object {
        Write-Host "$($_.Path):$($_.LineNumber):$($_.Line)" -ForegroundColor Red
    }

    throw "P3 sync processor contract tests contain implicitly typed empty arrays."
}

Write-Host "P3 sync processor integration hardening verification passed." -ForegroundColor Green