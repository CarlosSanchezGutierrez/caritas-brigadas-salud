$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) {
    $PSCommandPath
}
elseif ($MyInvocation.MyCommand.Path) {
    $MyInvocation.MyCommand.Path
}
else {
    throw "Unable to resolve script path."
}

$RepoRootText = git -C (Split-Path -Parent $ScriptPath) rev-parse --show-toplevel

if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($RepoRootText)) {
    throw "Unable to resolve repo root through git."
}

$RepoRoot = Resolve-Path $RepoRootText.Trim()

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
        [string]$Filter,
        [string]$Label
    )

    $DirectoryPath = Resolve-RepoPath -RelativePath $DirectoryRelativePath

    if (-not (Test-Path $DirectoryPath)) {
        throw "Missing directory for $Label: $DirectoryRelativePath"
    }

    $Matches = @(
        Get-ChildItem $DirectoryPath -Filter $Filter -File
    )

    if ($Matches.Count -eq 0) {
        throw "Missing evidence file for $Label using filter $Filter in $DirectoryRelativePath"
    }
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
        [string]$Token
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) {
        throw "Missing required token: $Token"
    }
}

function Assert-DoesNotContainToken {
    param(
        [string]$Content,
        [string]$Token
    )

    if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
        throw "Forbidden token found: $Token"
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

$PhaseFilters = @(
    @{ Phase = "P5.3"; Filter = "P5_03*PATIENT*" },
    @{ Phase = "P5.4"; Filter = "P5_04*PATIENT*" },
    @{ Phase = "P5.5"; Filter = "P5_05*PATIENT*" },
    @{ Phase = "P5.6"; Filter = "P5_06*PATIENT*" },
    @{ Phase = "P5.7"; Filter = "P5_07*PATIENT*" },
    @{ Phase = "P5.8"; Filter = "P5_08*PATIENT*" },
    @{ Phase = "P5.9"; Filter = "P5_09*PATIENT*" }
)

foreach ($Phase in $PhaseFilters) {
    Assert-AnyFileExists -DirectoryRelativePath "docs/implementation" -Filter ($Phase.Filter + ".md") -Label ($Phase.Phase + " implementation")
    Assert-AnyFileExists -DirectoryRelativePath "docs/qa" -Filter ($Phase.Filter + "*.md") -Label ($Phase.Phase + " QA matrix")
    Assert-AnyFileExists -DirectoryRelativePath "docs/runbooks" -Filter ($Phase.Filter + "*.md") -Label ($Phase.Phase + " runbook")
}

$VerifierFilters = @(
    "verify-p5-03*.ps1",
    "verify-p5-04*.ps1",
    "verify-p5-05*.ps1",
    "verify-p5-06*.ps1",
    "verify-p5-07*.ps1",
    "verify-p5-08*.ps1",
    "verify-p5-09*.ps1"
)

foreach ($Filter in $VerifierFilters) {
    Assert-AnyFileExists -DirectoryRelativePath "scripts" -Filter $Filter -Label "phase verifier $Filter"
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

$AllContent = $ClosureContent + "`n" +
    $MatrixContent + "`n" +
    $RunbookContent + "`n" +
    $CreateRequestContent + "`n" +
    $SummaryContent + "`n" +
    $ClinicalRecordContent + "`n" +
    $WriteRepositoryContent + "`n" +
    $ReadRepositoryContent + "`n" +
    $DbContextContent + "`n" +
    $AuditMapperContent + "`n" +
    $AuditFilterContent

$RequiredTokens = @(
    "P5.10 Patient Module Closure",
    "Patient module backend controlled milestone: CLOSED_PENDING_REAL_ENVIRONMENT_EVIDENCE",
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SourceBrigadeId",
    "LocalPatientId",
    "ClientOperationId",
    "IdempotencyKey",
    "SyncStatus",
    "DataCaptureSource",
    "CreatedAtAction",
    "FindExistingIdempotentPatientAsync",
    "PatientCreateIdempotencyUniqueIndexNames",
    "IX_patients_OrganizationId_IdempotencyKey_UQ",
    "IX_patients_OrganizationId_ClientOperationId_UQ",
    "IX_patients_OrganizationId_SourceBrigadeId_LocalPatientId_UQ",
    "PatientClinicalRecordTimelineEventDto",
    "IReadOnlyCollection<PatientClinicalRecordTimelineEventDto> Timeline",
    "BuildTimeline",
    "AuditActionCodes.PatientCreate",
    "CreatedAtActionResult",
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
    "Real SQL Server migration execution"
)

foreach ($Token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllContent -Token $Token
}

$ForbiddenTokens = @(
    "Backend production readiness: APPROVED",
    "Backend production readiness: READY",
    "Patient module backend controlled milestone: PRODUCTION_READY",
    "production ready",
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

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllContent -Token $Token
}

Write-Host "P5.10 patient module closure verifier passed from repo root: $RepoRoot"