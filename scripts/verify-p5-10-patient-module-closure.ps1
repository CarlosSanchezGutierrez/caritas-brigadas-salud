$ErrorActionPreference = "Stop"

$RepoRootText = git rev-parse --show-toplevel

if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($RepoRootText)) {
    throw "Unable to resolve repo root."
}

$RepoRoot = $RepoRootText.Trim()

function Resolve-RepoPath {
    param([string]$RelativePath)

    return Join-Path -Path $RepoRoot -ChildPath $RelativePath
}

function Assert-FileExists {
    param([string]$RelativePath)

    $AbsolutePath = Resolve-RepoPath -RelativePath $RelativePath

    if (-not (Test-Path $AbsolutePath)) {
        throw "Missing required file: $RelativePath resolved to $AbsolutePath"
    }
}

function Assert-AnyFileExists {
    param(
        [string]$DirectoryRelativePath,
        [string[]]$Filters,
        [string]$Label
    )

    $DirectoryPath = Resolve-RepoPath -RelativePath $DirectoryRelativePath

    if (-not (Test-Path $DirectoryPath)) {
        throw "Missing directory for ${Label}: $DirectoryRelativePath"
    }

    foreach ($Filter in $Filters) {
        $Matches = @(Get-ChildItem $DirectoryPath -Filter $Filter -File)

        if ($Matches.Count -gt 0) {
            return
        }
    }

    throw "Missing evidence file for ${Label} in $DirectoryRelativePath"
}

function Read-RepoText {
    param([string]$RelativePath)

    $AbsolutePath = Resolve-RepoPath -RelativePath $RelativePath

    if (-not (Test-Path $AbsolutePath)) {
        throw "Cannot read missing file: $RelativePath resolved to $AbsolutePath"
    }

    return [System.IO.File]::ReadAllText($AbsolutePath)
}

function Assert-ContainsToken {
    param(
        [string]$Content,
        [string]$Token,
        [string]$EvidenceLabel
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "Missing required token in ${EvidenceLabel}: $Token"
    }
}

function Assert-DoesNotContainToken {
    param(
        [string]$Content,
        [string]$Token,
        [string]$EvidenceLabel
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "Forbidden token found in ${EvidenceLabel}: $Token"
    }
}

function Assert-Tokens {
    param(
        [string]$Content,
        [string[]]$Tokens,
        [string]$EvidenceLabel
    )

    foreach ($Token in $Tokens) {
        Assert-ContainsToken -Content $Content -Token $Token -EvidenceLabel $EvidenceLabel
    }
}

$RequiredFiles = @(
    "docs/implementation/P5_10_PATIENT_MODULE_CLOSURE.md",
    "docs/qa/P5_10_PATIENT_MODULE_CLOSURE_MATRIX.md",
    "docs/runbooks/P5_10_PATIENT_MODULE_CLOSURE_RUNBOOK.md",
    "scripts/verify-p5-10-patient-module-closure.ps1",
    "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/CreatePatientRequest.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientSummaryDto.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientClinicalRecordDto.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientWriteRepository.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientReadRepository.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/CaritasDbContext.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Api/Audit/ClinicalWriteAuditActionMapper.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Api/Audit/ClinicalWriteAuditActionFilter.cs"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$PhaseEvidence = @(
    @{ Phase = "P5.3"; Filters = @("P5_03*PATIENT*.md", "P5_3*PATIENT*.md") },
    @{ Phase = "P5.4"; Filters = @("P5_04*PATIENT*.md", "P5_4*PATIENT*.md") },
    @{ Phase = "P5.5"; Filters = @("P5_05*PATIENT*.md", "P5_5*PATIENT*.md") },
    @{ Phase = "P5.6"; Filters = @("P5_06*PATIENT*.md", "P5_6*PATIENT*.md") },
    @{ Phase = "P5.7"; Filters = @("P5_07*PATIENT*.md", "P5_7*PATIENT*.md") },
    @{ Phase = "P5.8"; Filters = @("P5_08*PATIENT*.md", "P5_8*PATIENT*.md") },
    @{ Phase = "P5.9"; Filters = @("P5_09_PATIENT*.md", "P5_9_PATIENT*.md") },
    @{ Phase = "P5.9.1"; Filters = @("P5_09_1*PATIENT*.md", "P5_9_1*PATIENT*.md") },
    @{ Phase = "P5.9.2"; Filters = @("P5_09_2*PATIENT*.md", "P5_9_2*PATIENT*.md") }
)

foreach ($Phase in $PhaseEvidence) {
    Assert-AnyFileExists -DirectoryRelativePath "docs/implementation" -Filters $Phase.Filters -Label ($Phase.Phase + " implementation")
    Assert-AnyFileExists -DirectoryRelativePath "docs/qa" -Filters $Phase.Filters -Label ($Phase.Phase + " QA matrix")
    Assert-AnyFileExists -DirectoryRelativePath "docs/runbooks" -Filters $Phase.Filters -Label ($Phase.Phase + " runbook")
}

$VerifierEvidence = @(
    @{ Label = "P5.9 verifier"; Filters = @("verify-p5-09-patient-create-idempotency.ps1") },
    @{ Label = "P5.9.1 verifier"; Filters = @("verify-p5-09-1-patient-create-atomic-idempotency-backstop.ps1") },
    @{ Label = "P5.9.2 verifier"; Filters = @("verify-p5-09-2-patient-idempotency-violated-index-replay.ps1") }
)

foreach ($Evidence in $VerifierEvidence) {
    Assert-AnyFileExists -DirectoryRelativePath "scripts" -Filters $Evidence.Filters -Label $Evidence.Label
}

$ClosureContent = Read-RepoText -RelativePath "docs/implementation/P5_10_PATIENT_MODULE_CLOSURE.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P5_10_PATIENT_MODULE_CLOSURE_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P5_10_PATIENT_MODULE_CLOSURE_RUNBOOK.md"

$CreateRequestContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/CreatePatientRequest.cs"
$SummaryContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientSummaryDto.cs"
$ClinicalRecordContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientClinicalRecordDto.cs"

$WriteRepositoryContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientWriteRepository.cs"
$ReadRepositoryContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientReadRepository.cs"
$DbContextContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/CaritasDbContext.cs"
$AuditMapperContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Api/Audit/ClinicalWriteAuditActionMapper.cs"
$AuditFilterContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Api/Audit/ClinicalWriteAuditActionFilter.cs"

$MigrationFiles = @(
    Get-ChildItem (Resolve-RepoPath -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Migrations") -Filter "*AddPatientCreateIdempotencyUniqueIndexes.cs" |
        Where-Object { $_.Name -notlike "*.Designer.cs" } |
        Select-Object -ExpandProperty FullName
)

if ($MigrationFiles.Count -eq 0) {
    throw "Missing AddPatientCreateIdempotencyUniqueIndexes migration."
}

$MigrationContent = ""

foreach ($MigrationFile in $MigrationFiles) {
    $MigrationContent += [System.IO.File]::ReadAllText([string]$MigrationFile) + "`n"
}

$DocsContent = $ClosureContent + "`n" + $MatrixContent + "`n" + $RunbookContent
$ContractsContent = $CreateRequestContent + "`n" + $SummaryContent + "`n" + $ClinicalRecordContent
$TimelineImplementationContent = $ClinicalRecordContent + "`n" + $ReadRepositoryContent
$AuditImplementationContent = $AuditMapperContent + "`n" + $AuditFilterContent
$PersistenceImplementationContent = $DbContextContent + "`n" + $MigrationContent
$WriteImplementationContent = $WriteRepositoryContent

Assert-Tokens -Content $DocsContent -EvidenceLabel "P5.10 docs" -Tokens @(
    "P5.10 Patient Module Closure",
    "Patient module backend controlled milestone: CLOSED_PENDING_REAL_ENVIRONMENT_EVIDENCE",
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "P5.3 Patient API contracts",
    "P5.4 Patient offline and source persistence",
    "P5.5 Patient API endpoint hardening",
    "P5.6 Patient validation and organization scoping",
    "P5.7 Patient write audit evidence",
    "P5.8 Patient longitudinal timeline",
    "P5.9 Patient create idempotency",
    "P5.9.1 Patient create atomic idempotency backstop",
    "P5.9.2 Patient idempotency violated-index replay",
    "No backend production readiness approval",
    "No fabricated evidence",
    "No secrets in repository",
    "No committed real patient data",
    "No direct mobile write to SQL Server",
    "No client may bypass the API",
    "No cloud dependency",
    "SQL Server remains the operational source of truth",
    "Offline sync processor",
    "Patient merge or deduplication",
    "Real environment SQL Server migration execution"
)

Assert-Tokens -Content $ContractsContent -EvidenceLabel "patient contract files" -Tokens @(
    "SourceBrigadeId",
    "LocalPatientId",
    "ClientOperationId",
    "IdempotencyKey",
    "SyncStatus",
    "DataCaptureSource",
    "PatientClinicalRecordTimelineEventDto",
    "Timeline"
)

Assert-Tokens -Content $WriteImplementationContent -EvidenceLabel "patient write repository implementation" -Tokens @(
    "FindExistingIdempotentPatientAsync",
    "PatientCreateIdempotencyUniqueIndexNames",
    "IdempotencyKeyUniqueIndexName",
    "ClientOperationIdUniqueIndexName",
    "LocalPatientUniqueIndexName",
    "FindExistingIdempotentPatientForUniqueViolationAsync",
    "GetPatientCreateIdempotencyUniqueIndexName",
    "FindExistingPatientByIdempotencyKeyAsync",
    "FindExistingPatientByClientOperationIdAsync",
    "FindExistingPatientByLocalPatientIdAsync",
    "violatedIndexName switch",
    "catch (DbUpdateException exception) when (IsPatientCreateIdempotencyUniqueViolation(exception))",
    "error.Number is 2601 or 2627",
    "return ToSummary(replayedPatient)"
)

Assert-DoesNotContainToken -Content $WriteImplementationContent -Token "var replayedPatient = await FindExistingIdempotentPatientAsync(" -EvidenceLabel "patient write repository catch replay path"

Assert-Tokens -Content $PersistenceImplementationContent -EvidenceLabel "patient persistence and migration implementation" -Tokens @(
    "IX_patients_OrganizationId_IdempotencyKey_UQ",
    "IX_patients_OrganizationId_ClientOperationId_UQ",
    "IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ",
    "CREATE UNIQUE INDEX [IX_patients_OrganizationId_IdempotencyKey_UQ]",
    "CREATE UNIQUE INDEX [IX_patients_OrganizationId_ClientOperationId_UQ]",
    "CREATE UNIQUE INDEX [IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ",
    "WHERE [IdempotencyKey] IS NOT NULL AND [IsDeleted] = 0",
    "WHERE [ClientOperationId] IS NOT NULL AND [IsDeleted] = 0",
    "WHERE [SourceBrigadeId] IS NOT NULL AND [LocalPatientId] IS NOT NULL AND [IsDeleted] = 0"
)

Assert-Tokens -Content $TimelineImplementationContent -EvidenceLabel "patient timeline implementation" -Tokens @(
    "PatientClinicalRecordTimelineEventDto",
    "Timeline",
    "BuildTimeline"
)

Assert-Tokens -Content $AuditImplementationContent -EvidenceLabel "patient audit implementation" -Tokens @(
    "AuditActionCodes.PatientCreate",
    "CreatedAtActionResult"
)

$ForbiddenTokens = @(
    "Backend production readiness: APPROVED",
    "Backend production readiness: READY",
    "Backend production readiness approved",
    "Backend production readiness is approved",
    "Patient module backend controlled milestone: PRODUCTION_READY",
    "Patient module backend controlled milestone: READY_FOR_PRODUCTION",
    "Production readiness approved",
    "Ready for production approval",
    "mobile clients may write directly to SQL Server",
    "frontend may bypass API",
    "real patient data is committed intentionally",
    "repository intentionally stores secrets",
    "Cloud is required",
    "Azure is required",
    "AWS is required",
    "User ID=sa",
    "Password=",
    "Pwd="
)

$AllContentForForbiddenTokens = $DocsContent + "`n" +
    $ContractsContent + "`n" +
    $WriteImplementationContent + "`n" +
    $PersistenceImplementationContent + "`n" +
    $TimelineImplementationContent + "`n" +
    $AuditImplementationContent

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllContentForForbiddenTokens -Token $Token -EvidenceLabel "P5.10 full evidence corpus"
}

Write-Host "P5.10 patient module closure verifier passed from repo root: $RepoRoot"