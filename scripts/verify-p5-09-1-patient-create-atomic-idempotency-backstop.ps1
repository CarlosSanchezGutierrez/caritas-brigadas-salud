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
        [string]$Token
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "Missing required token: $Token"
    }
}

$DbContext = Read-RepoText "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/CaritasDbContext.cs"
$Repository = Read-RepoText "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientWriteRepository.cs"
$Docs = Read-RepoText "docs/implementation/P5_09_1_PATIENT_CREATE_ATOMIC_IDEMPOTENCY_BACKSTOP.md"

$MigrationFiles = @(
    Get-ChildItem (Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Migrations") -Filter "*AddPatientCreateIdempotencyUniqueIndexes.cs" |
        Where-Object { $_.Name -notlike "*.Designer.cs" } |
        Select-Object -ExpandProperty FullName
)

if ($MigrationFiles.Count -eq 0) {
    throw "Missing AddPatientCreateIdempotencyUniqueIndexes migration."
}

$Migration = [System.IO.File]::ReadAllText([string]$MigrationFiles[0])

$All = $DbContext + "`n" + $Repository + "`n" + $Migration + "`n" + $Docs

$RequiredTokens = @(
    "PatientCreateIdempotencyUniqueIndexNames",
    "IX_patients_OrganizationId_IdempotencyKey_UQ",
    "IX_patients_OrganizationId_ClientOperationId_UQ",
    "IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ",
    "CREATE UNIQUE INDEX [IX_patients_OrganizationId_IdempotencyKey_UQ]",
    "CREATE UNIQUE INDEX [IX_patients_OrganizationId_ClientOperationId_UQ]",
    "CREATE UNIQUE INDEX [IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ]",
    "WHERE [IdempotencyKey] IS NOT NULL AND [IsDeleted] = 0",
    "WHERE [ClientOperationId] IS NOT NULL AND [IsDeleted] = 0",
    "WHERE [SourceBrigadeId] IS NOT NULL AND [LocalPatientId] IS NOT NULL AND [IsDeleted] = 0",
    "catch (DbUpdateException exception) when (IsPatientCreateIdempotencyUniqueViolation(exception))",
    "error.Number is 2601 or 2627",
    "return ToSummary(replayedPatient)",
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

foreach ($Token in $RequiredTokens) {
    Assert-Contains -Content $All -Token $Token
}

Write-Host "P5.9.1 patient create atomic idempotency backstop verifier passed from repo root: $RepoRoot"