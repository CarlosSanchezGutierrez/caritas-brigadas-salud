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
    "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientClinicalRecordDto.cs",
    "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientReadRepository.cs",
    "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Security/P3ClinicalRecordReadModelContractTests.cs",
    "docs/implementation/P5_08_PATIENT_LONGITUDINAL_HISTORY_TIMELINE.md",
    "docs/qa/P5_08_PATIENT_LONGITUDINAL_HISTORY_TIMELINE_MATRIX.md",
    "docs/runbooks/P5_08_PATIENT_LONGITUDINAL_HISTORY_TIMELINE_RUNBOOK.md",
    "scripts/verify-p5-08-patient-longitudinal-history-timeline.ps1"
)

foreach ($File in $RequiredFiles) {
    Assert-FileExists -RelativePath $File
}

$DtoContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Contracts/Patients/PatientClinicalRecordDto.cs"
$RepositoryContent = Read-RepoText -RelativePath "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Patients/PatientReadRepository.cs"
$TestsContent = Read-RepoText -RelativePath "services/api-dotnet/tests/Caritas.Brigadas.Api.Tests/Security/P3ClinicalRecordReadModelContractTests.cs"
$ImplementationContent = Read-RepoText -RelativePath "docs/implementation/P5_08_PATIENT_LONGITUDINAL_HISTORY_TIMELINE.md"
$MatrixContent = Read-RepoText -RelativePath "docs/qa/P5_08_PATIENT_LONGITUDINAL_HISTORY_TIMELINE_MATRIX.md"
$RunbookContent = Read-RepoText -RelativePath "docs/runbooks/P5_08_PATIENT_LONGITUDINAL_HISTORY_TIMELINE_RUNBOOK.md"

$AllContent = $DtoContent + "`n" + $RepositoryContent + "`n" + $TestsContent + "`n" + $ImplementationContent + "`n" + $MatrixContent + "`n" + $RunbookContent

$RequiredTokens = @(
    "IReadOnlyCollection<PatientClinicalRecordTimelineEventDto> Timeline",
    "public sealed record PatientClinicalRecordTimelineEventDto",
    "TimelineEventCount",
    "FirstTimelineEventAt",
    "LastTimelineEventAt",
    "OccurredAt",
    "EventType",
    "EntityId",
    "VisitId",
    "EncounterId",
    "Timeline = timeline",
    "BuildTimeline(",
    "EventType = ""visit""",
    "EventType = ""service-encounter""",
    "EventType = ""vital-signs""",
    "EventType = ""form-response""",
    "EventType = ""consent-document""",
    "EventType = ""medical-referral""",
    "EventType = ""medication-delivery""",
    ".OrderByDescending(item => item.OccurredAt)",
    "P5.8 Patient Longitudinal History Timeline",
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
    Assert-ContainsToken -Content $AllContent -Token $Token
}

$ForbiddenTokens = @(
    "mobile clients may write directly to SQL Server",
    "frontend may bypass API",
    "backend is production ready",
    "backend production readiness is approved",
    "Cloud is required",
    "Azure is required",
    "AWS is required",
    "User ID=sa",
    "Password=",
    "Pwd=",
    "real patient data is committed intentionally",
    "repository intentionally stores secrets"
)

foreach ($Token in $ForbiddenTokens) {
    Assert-DoesNotContainToken -Content $AllContent -Token $Token
}

Write-Host "P5.8 patient longitudinal history timeline verifier passed from repo root: $RepoRoot"