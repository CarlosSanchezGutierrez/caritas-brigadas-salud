
collect-p5-02-patient-core-readiness

param(
[string]$OutputRoot = "artifacts/p5/p5-02-patient-core-readiness"
)

$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) { $PSCommandPath } elseif ($MyInvocation.MyCommand.Path) { $MyInvocation.MyCommand.Path } else { throw "Unable to resolve script path." }
$ScriptDirectory = Split-Path -Parent $ScriptPath
$RepoRootText = git -C $ScriptDirectory rev-parse --show-toplevel

if ($LASTEXITCODE -ne 0 -or [string]::IsNullOrWhiteSpace($RepoRootText)) {
throw "Unable to resolve repo root through git."
}

$RepoRoot = Resolve-Path $RepoRootText.Trim()
Set-Location $RepoRoot

$RunStamp = Get-Date -Format "yyyyMMdd-HHmmss"
$EvidenceDir = Join-Path $RepoRoot (Join-Path $OutputRoot $RunStamp)
[System.IO.Directory]::CreateDirectory($EvidenceDir) | Out-Null

function Write-TextFile {
param(
[Parameter(Mandatory = $true)][string]$Path,
[AllowEmptyString()][string]$Content
)

$Parent = Split-Path -Parent $Path
[System.IO.Directory]::CreateDirectory($Parent) | Out-Null
$Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
[System.IO.File]::WriteAllText($Path, $Content, $Utf8NoBom)

}

function ConvertTo-RelativePath {
param([Parameter(Mandatory = $true)][string]$Path)

$ResolvedPath = Resolve-Path $Path
$Root = $RepoRoot.Path.TrimEnd("\", "/") + [System.IO.Path]::DirectorySeparatorChar
return $ResolvedPath.Path.Replace($Root, "").Replace("\", "/")

}

function Get-CodeFiles {
param([string]$Root)

if (-not (Test-Path $Root)) {
    return @()
}

return @(Get-ChildItem -Path $Root -Recurse -File -Include "*.cs","*.json","*.csproj","*.sln" -ErrorAction SilentlyContinue | Sort-Object FullName -Unique)

}

function Get-Text {
param([string]$Path)

try {
    return [System.IO.File]::ReadAllText($Path)
}
catch {
    return ""
}

}

function Find-FilesByTerms {
param(
[object[]]$Files,
[string[]]$Terms
)

$Matches = @()

foreach ($File in $Files) {
    $RelativePath = ConvertTo-RelativePath -Path $File.FullName
    $Text = Get-Text -Path $File.FullName
    $MatchedTerms = @()

    foreach ($Term in $Terms) {
        if ($RelativePath.IndexOf($Term, [System.StringComparison]::OrdinalIgnoreCase) -ge 0 -or
            $Text.IndexOf($Term, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
            $MatchedTerms += $Term
        }
    }

    if ($MatchedTerms.Count -gt 0) {
        $Matches += [pscustomobject]@{
            path = $RelativePath
            matched_terms = @($MatchedTerms | Sort-Object -Unique)
            bytes = $File.Length
        }
    }
}

return @($Matches | Sort-Object path -Unique)

}

function Test-HasAnyMatch {
param([object[]]$Matches)

return ($Matches.Count -gt 0)

}

$ApiRoot = Join-Path $RepoRoot "services/api-dotnet"
$AllFiles = Get-CodeFiles -Root $ApiRoot

$PatientTerms = @("Patient", "Paciente", "Patients", "Pacientes")
$PatientIdentityTerms = @("Curp", "CURP", "Phone", "Telefono", "Teléfono", "Identity", "Identifier", "Incomplete", "DisplayName", "GivenName", "FamilyName")
$PatientEndpointTerms = @("PatientsController", "PatientController", "MapPatients", "MapPatient", "patients", "patient")
$PatientPersistenceTerms = @("DbSet<Patient", "PatientConfiguration", "Patients", "PatientEntity", "UseSqlServer")
$PatientValidationTerms = @("PatientValidator", "ValidatePatient", "Validation", "Required", "MaxLength")
$PatientAuthorizationTerms = @("Authorize", "Permission", "Policy", "Role", "OrganizationAccess", "organizationId", "OrganizationId")
$PatientAuditTerms = @("Audit", "ClinicalWriteAudit", "OperationalWriteAudit", "PatientCreated", "PatientUpdated")
$PatientTestTerms = @("PatientTests", "PatientsTests", "PatientControllerTests", "PatientEndpointTests")
$OfflineTerms = @("Idempotency", "IdempotencyKey", "ClientOperationId", "SyncStatus", "Offline", "Outbox", "Conflict")
$LongitudinalTerms = @("Longitudinal", "Timeline", "History", "Historial", "EncounterHistory", "PatientHistory")

$PatientSurface = Find-FilesByTerms -Files $AllFiles -Terms $PatientTerms
$PatientIdentitySurface = Find-FilesByTerms -Files $AllFiles -Terms $PatientIdentityTerms
$PatientEndpointSurface = Find-FilesByTerms -Files $AllFiles -Terms $PatientEndpointTerms
$PatientPersistenceSurface = Find-FilesByTerms -Files $AllFiles -Terms $PatientPersistenceTerms
$PatientValidationSurface = Find-FilesByTerms -Files $AllFiles -Terms $PatientValidationTerms
$PatientAuthorizationSurface = Find-FilesByTerms -Files $AllFiles -Terms $PatientAuthorizationTerms
$PatientAuditSurface = Find-FilesByTerms -Files $AllFiles -Terms $PatientAuditTerms
$PatientTestSurface = Find-FilesByTerms -Files $AllFiles -Terms $PatientTestTerms
$OfflineSurface = Find-FilesByTerms -Files $AllFiles -Terms $OfflineTerms
$LongitudinalSurface = Find-FilesByTerms -Files $AllFiles -Terms $LongitudinalTerms

$Checks = @(
[pscustomobject]@{ key = "patient_domain_surface"; label = "Patient domain surface"; detected = Test-HasAnyMatch -Matches $PatientSurface },
[pscustomobject]@{ key = "patient_identity_surface"; label = "Flexible patient identity surface"; detected = Test-HasAnyMatch -Matches $PatientIdentitySurface },
[pscustomobject]@{ key = "patient_endpoint_surface"; label = "Patient endpoint surface"; detected = Test-HasAnyMatch -Matches $PatientEndpointSurface },
[pscustomobject]@{ key = "patient_persistence_surface"; label = "Patient persistence surface"; detected = Test-HasAnyMatch -Matches $PatientPersistenceSurface },
[pscustomobject]@{ key = "patient_validation_surface"; label = "Patient validation surface"; detected = Test-HasAnyMatch -Matches $PatientValidationSurface },
[pscustomobject]@{ key = "patient_authorization_surface"; label = "Patient authorization surface"; detected = Test-HasAnyMatch -Matches $PatientAuthorizationSurface },
[pscustomobject]@{ key = "patient_audit_surface"; label = "Patient audit surface"; detected = Test-HasAnyMatch -Matches $PatientAuditSurface },
[pscustomobject]@{ key = "patient_test_surface"; label = "Patient test surface"; detected = Test-HasAnyMatch -Matches $PatientTestSurface },
[pscustomobject]@{ key = "offline_patient_surface"; label = "Offline-first patient surface"; detected = Test-HasAnyMatch -Matches $OfflineSurface },
[pscustomobject]@{ key = "longitudinal_patient_surface"; label = "Longitudinal patient history surface"; detected = Test-HasAnyMatch -Matches $LongitudinalSurface }
)

$MissingChecks = @($Checks | Where-Object { -not $_.detected })

$Inventory = [pscustomobject]@{
phase = "P5.2 Patient Core Readiness"
backend_production_readiness = "BLOCKED_PENDING_REAL_EVIDENCE"
generated_at = (Get-Date).ToString("o")
source_file_count = $AllFiles.Count
checks = $Checks
patient_surface = $PatientSurface
patient_identity_surface = $PatientIdentitySurface
patient_endpoint_surface = $PatientEndpointSurface
patient_persistence_surface = $PatientPersistenceSurface
patient_validation_surface = $PatientValidationSurface
patient_authorization_surface = $PatientAuthorizationSurface
patient_audit_surface = $PatientAuditSurface
patient_test_surface = $PatientTestSurface
offline_patient_surface = $OfflineSurface
longitudinal_patient_surface = $LongitudinalSurface
}

$GapBacklog = "# P5.2 Patient Core Gap Backlognn"
$GapBacklog += "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCEnn"

if ($MissingChecks.Count -eq 0) {
$GapBacklog += "No missing patient core surfaces were detected by keyword inventory. This does not prove functional completeness.n" } else { $GapBacklog += "Detected missing or weak patient core surfaces requiring implementation:n`n"

foreach ($Check in $MissingChecks) {
    $GapBacklog += "- " + $Check.label + " | key: " + $Check.key + " | next action: implement or verify real backend code.`n"
}

}

$GapBacklog += "nRecommended next implementation PRs:n"
$GapBacklog += "- P5.3 patient entity and contracts.n" $GapBacklog += "- P5.4 patient persistence and migration.n"
$GapBacklog += "- P5.5 patient API endpoints.n" $GapBacklog += "- P5.6 patient validation and organization authorization.n"
$GapBacklog += "- P5.7 patient write audit.n" $GapBacklog += "- P5.8 longitudinal patient history linkage.n"
$GapBacklog += "- P6 offline-first patient synchronization.`n"

$Summary = "# P5.2 Patient Core Readiness Summarynn"
$Summary += "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCEnn"
$Summary += "| Area | Detected |n" $Summary += "|---|---:|n"

foreach ($Check in $Checks) {
$Summary += "| " + $Check.label + " | " + $Check.detected + " |`n"
}

$Summary += "nMissing or weak areas: " + $MissingChecks.Count + "n"

$InventoryPath = Join-Path $EvidenceDir "patient-core-surface-inventory.json"
$SummaryPath = Join-Path $EvidenceDir "patient-core-readiness-summary.md"
$GapBacklogPath = Join-Path $EvidenceDir "patient-core-gap-backlog.md"

Write-TextFile -Path $InventoryPath -Content ($Inventory | ConvertTo-Json -Depth 30)
Write-TextFile -Path $SummaryPath -Content $Summary
Write-TextFile -Path $GapBacklogPath -Content $GapBacklog

$Manifest = [pscustomobject]@{
phase = "P5.2 Patient Core Readiness"
backend_production_readiness = "BLOCKED_PENDING_REAL_EVIDENCE"
generated_at = (Get-Date).ToString("o")
evidence_output_root = $EvidenceDir
source_file_count = $AllFiles.Count
missing_or_weak_patient_surface_count = $MissingChecks.Count
patient_core_required_for_final_system = $true
offline_first_required_for_final_system = $true
longitudinal_history_required_for_final_system = $true
dashboards_required_for_final_system = $true
analytics_required_for_final_system = $true
output_files = @{
inventory = $InventoryPath
summary = $SummaryPath
gap_backlog = $GapBacklogPath
}
}

$ManifestPath = Join-Path $EvidenceDir "manifest.json"
Write-TextFile -Path $ManifestPath -Content ($Manifest | ConvertTo-Json -Depth 20)

Write-Host ""
Write-Host "P5.2 Patient Core Readiness completed."
Write-Host ("Manifest: {0}" -f $ManifestPath)
Write-Host ("Summary: {0}" -f $SummaryPath)
Write-Host ("Gap backlog: {0}" -f $GapBacklogPath)