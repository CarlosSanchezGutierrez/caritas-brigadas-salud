$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$DocPath = Join-Path $RepoRoot "docs/backend/P3_SYNC_PAYLOAD_READER_EXTRACTION_BASELINE.md"
$ProcessorPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncBatchProcessor.cs"
$ReaderPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/SyncPayloadReader.cs"
$PatientHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/PatientSyncEventHandler.cs"
$VisitHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/PatientVisitSyncEventHandler.cs"
$ServiceHandlerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Sync/ServiceEncounterSyncEventHandler.cs"

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
Assert-FileExists $ReaderPath
Assert-FileExists $PatientHandlerPath
Assert-FileExists $VisitHandlerPath
Assert-FileExists $ServiceHandlerPath

$Doc = Get-Content $DocPath -Raw -Encoding UTF8
$Processor = Get-Content $ProcessorPath -Raw -Encoding UTF8
$Reader = Get-Content $ReaderPath -Raw -Encoding UTF8
$PatientHandler = Get-Content $PatientHandlerPath -Raw -Encoding UTF8
$VisitHandler = Get-Content $VisitHandlerPath -Raw -Encoding UTF8
$ServiceHandler = Get-Content $ServiceHandlerPath -Raw -Encoding UTF8
$ProcessorAndPatientHandler = $Processor + $PatientHandler + $VisitHandler + $ServiceHandler

$RequiredDocTokens = @(
    "P3 Sync Payload Reader Extraction Baseline",
    "SyncPayloadReader",
    "parse PayloadJson",
    "require JSON object root",
    "deserialize the request DTO",
    "SyncBatchProcessor must use SyncPayloadReader.TryReadObject for all current create request DTOs",
    "SyncBatchProcessor must use explicit typed out variables for current create request DTOs",
    "Acceptance criteria"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains $Doc $Token "P3 sync payload reader extraction baseline"
}

$RequiredReaderTokens = @(
    "internal static class SyncPayloadReader",
    "public static bool TryReadObject<TRequest>",
    "[NotNullWhen(true)] out TRequest? request",
    "where TRequest : class",
    "JsonDocument.Parse(payloadJson)",
    "document.RootElement.ValueKind != JsonValueKind.Object",
    "document.RootElement.Deserialize<TRequest>(serializerOptions)",
    'payload must be a JSON object.',
    'payload JSON is invalid.',
    'payload is required.'
)

foreach ($Token in $RequiredReaderTokens) {
    Assert-Contains $Reader $Token "SyncPayloadReader"
}

$RequiredProcessorTokens = @(
    "SyncPayloadReader.TryReadObject",
    "out CreatePatientRequest? request",
    "out CreatePatientVisitRequest? request",
    "out CreateServiceEncounterRequest? request",
    "out CreateVitalSignsRecordRequest? request",
    "out CreateFormResponseRequest? request",
    "out CreateConsentDocumentRequest? request",
    "out CreateMedicalReferralRequest? request",
    "out CreateMedicationDeliveryRequest? request",
    "payloadRejectionReason"
)

foreach ($Token in $RequiredProcessorTokens) {
    Assert-Contains $ProcessorAndPatientHandler $Token "SyncBatchProcessor payload reader extraction"
}

$ForbiddenProcessorTokens = @(
    "JsonSerializer.Deserialize<CreatePatientRequest>",
    "JsonSerializer.Deserialize<CreatePatientVisitRequest>",
    "JsonSerializer.Deserialize<CreateServiceEncounterRequest>",
    "JsonSerializer.Deserialize<CreateVitalSignsRecordRequest>",
    "JsonSerializer.Deserialize<CreateFormResponseRequest>",
    "JsonSerializer.Deserialize<CreateConsentDocumentRequest>",
    "JsonSerializer.Deserialize<CreateMedicalReferralRequest>",
    "JsonSerializer.Deserialize<CreateMedicationDeliveryRequest>",
    "out  request"
)

foreach ($Token in $ForbiddenProcessorTokens) {
    if ($Processor.Contains($Token)) {
        throw "SyncBatchProcessor still contains forbidden payload reader extraction token: $Token"
    }
}

Write-Host "P3 sync payload reader extraction verification passed." -ForegroundColor Green