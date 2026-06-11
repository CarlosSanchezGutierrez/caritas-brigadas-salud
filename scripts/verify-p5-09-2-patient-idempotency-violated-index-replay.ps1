$ErrorActionPreference = "Stop"

$RepoRootText = git rev-parse --show-toplevel

if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($RepoRootText)) {
    throw "Unable to resolve repo root."
}

$RepoRoot = $RepoRootText.Trim()

function Read-RepoText {
    param([string]$RelativePath)

    $Path = Join-Path $RepoRoot $RelativePath

    if (-not (Test-Path $Path)) {
        throw "Missing file: $RelativePath"
    }

    return [System.IO.File]::ReadAllText($Path)
}

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "Missing required token in ${Label}: $Token"
    }
}

function Assert-DoesNotContain {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "Forbidden token found in ${Label}: $Token"
    }
}

$Repository = Read-RepoText "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientWriteRepository.cs"
$Docs = Read-RepoText "docs/implementation/P5_09_2_PATIENT_IDEMPOTENCY_VIOLATED_INDEX_REPLAY.md"

$RequiredRepositoryTokens = @(
    "IdempotencyKeyUniqueIndexName",
    "ClientOperationIdUniqueIndexName",
    "LocalPatientUniqueIndexName",
    "FindExistingIdempotentPatientForUniqueViolationAsync",
    "GetPatientCreateIdempotencyUniqueIndexName",
    "FindExistingPatientByIdempotencyKeyAsync",
    "FindExistingPatientByClientOperationIdAsync",
    "FindExistingPatientByLocalPatientIdAsync",
    "violatedIndexName switch",
    "IdempotencyKeyUniqueIndexName => await FindExistingPatientByIdempotencyKeyAsync",
    "ClientOperationIdUniqueIndexName => await FindExistingPatientByClientOperationIdAsync",
    "LocalPatientUniqueIndexName => await FindExistingPatientByLocalPatientIdAsync",
    "catch (DbUpdateException exception) when (IsPatientCreateIdempotencyUniqueViolation(exception))",
    "error.Number is 2601 or 2627",
    "return ToSummary(replayedPatient)"
)

foreach ($Token in $RequiredRepositoryTokens) {
    Assert-Contains -Content $Repository -Token $Token -Label "PatientWriteRepository"
}

Assert-DoesNotContain -Content $Repository -Token "var replayedPatient = await FindExistingIdempotentPatientAsync(" -Label "PatientWriteRepository catch replay path"

$RequiredDocTokens = @(
    "P5.9.2 Patient Idempotency Violated Index Replay",
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "No backend production readiness approval",
    "No fabricated evidence",
    "No secrets in repository",
    "No committed real patient data",
    "No direct mobile write to SQL Server",
    "No client may bypass the API",
    "No cloud dependency",
    "SQL Server remains the operational source of truth"
)

foreach ($Token in $RequiredDocTokens) {
    Assert-Contains -Content $Docs -Token $Token -Label "P5.9.2 docs"
}

Write-Host "P5.9.2 patient idempotency violated-index replay verifier passed from repo root: $RepoRoot"