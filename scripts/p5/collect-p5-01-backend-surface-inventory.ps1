
collect-p5-01-backend-surface-inventory

param(
[string]$OutputRoot = "artifacts/p5/p5-01-backend-surface-inventory"
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

function Get-FilesByPattern {
param(
[string]$Root,
[string[]]$Patterns
)

$Items = @()

foreach ($Pattern in $Patterns) {
    $Items += Get-ChildItem -Path $Root -Recurse -File -Filter $Pattern -ErrorAction SilentlyContinue
}

return @($Items | Sort-Object FullName -Unique)

}

function Get-MatchingFiles {
param(
[string]$Root,
[string]$Regex
)

return @(Get-ChildItem -Path $Root -Recurse -File -Include "*.cs","*.json","*.csproj","*.sln" -ErrorAction SilentlyContinue |
    Where-Object { $_.FullName -match $Regex } |
    Sort-Object FullName -Unique)

}

function To-InventoryRows {
param([object[]]$Files)

return @($Files | ForEach-Object {
    [pscustomobject]@{
        path = ConvertTo-RelativePath -Path $_.FullName
        bytes = $_.Length
    }
})

}

function Test-AnyPathOrContent {
param(
[object[]]$Files,
[string[]]$Terms
)

foreach ($File in $Files) {
    $RelativePath = ConvertTo-RelativePath -Path $File.FullName

    foreach ($Term in $Terms) {
        if ($RelativePath.IndexOf($Term, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
            return $true
        }
    }

    try {
        $Content = [System.IO.File]::ReadAllText($File.FullName)

        foreach ($Term in $Terms) {
            if ($Content.IndexOf($Term, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) {
                return $true
            }
        }
    }
    catch {
    }
}

return $false

}

$ApiRoot = Join-Path $RepoRoot "services/api-dotnet"
$ApiSrcRoot = Join-Path $RepoRoot "services/api-dotnet/src"
$ApiProjectRelativePath = "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj"
$ApiProjectPath = Join-Path $RepoRoot $ApiProjectRelativePath

$ProjectFiles = Get-FilesByPattern -Root $RepoRoot -Patterns @(".sln", ".csproj")
$SourceFiles = Get-FilesByPattern -Root $ApiRoot -Patterns @(".cs")
$JsonFiles = Get-FilesByPattern -Root $ApiRoot -Patterns @(".json")
$ControllerFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Controller.cs|Controllers[\/]|Endpoint|Endpoints[\/])"
$EndpointFiles = @(Get-ChildItem -Path $ApiRoot -Recurse -File -Include ".cs" -ErrorAction SilentlyContinue | Where-Object {
$Text = ""
try { $Text = [System.IO.File]::ReadAllText($.FullName) } catch {}
$.FullName -match "(?i)(Endpoint|Controller|Route)" -or $Text -match "(MapGet|MapPost|MapPut|MapDelete|MapGroup|Route(|HttpGet|HttpPost|HttpPut|HttpDelete)"
} | Sort-Object FullName -Unique)
$ContractFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Dto|Request|Response|Command|Query|Contract|Payload|Input|Output)"
$EntityFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Entities[\/]|Entity|Domain[\/]|Aggregate|ValueObject)"
$DbContextFiles = @(Get-ChildItem -Path $ApiRoot -Recurse -File -Include ".cs" -ErrorAction SilentlyContinue | Where-Object {
$Text = ""
try { $Text = [System.IO.File]::ReadAllText($.FullName) } catch {}
$.Name -match "DbContext" -or $Text -match "DbContext|DbSet<"
} | Sort-Object FullName -Unique)
$MigrationFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Migrations[\/]|Migration)"
$ConfigurationFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Configuration|EntityTypeConfiguration|IEntityTypeConfiguration)"
$ServiceFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Service|Services[\/]|Handler|Handlers[\/]|Repository|Repositories[\/])"
$AuthorizationFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Authorization|Permission|Policy|Role|Claims|Authentication|Security)"
$AuditFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Audit|Telemetry|Correlation|Trace|Log|Logging|Observability)"
$HealthOpenApiFiles = Get-MatchingFiles -Root $ApiRoot -Regex "(?i)(Health|Swagger|OpenApi|OpenAPI)"
$TestProjects = @(Get-ChildItem -Path $ApiRoot -Recurse -File -Filter ".csproj" -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match "(?i)(test|tests)" } | Sort-Object FullName -Unique)
$TestFiles = @(Get-ChildItem -Path $ApiRoot -Recurse -File -Include ".cs" -ErrorAction SilentlyContinue | Where-Object { $_.FullName -match "(?i)(test|tests|spec|fixture)" } | Sort-Object FullName -Unique)

$DomainDefinitions = @(
[pscustomobject]@{ key = "patient_core"; label = "Patient core"; terms = @("Patient", "Paciente") },
[pscustomobject]@{ key = "flexible_patient_identity"; label = "Flexible patient identity"; terms = @("Curp", "Phone", "Incomplete", "Identity", "Identifier", "Paciente") },
[pscustomobject]@{ key = "brigade_core"; label = "Brigade core"; terms = @("Brigade", "Brigada") },
[pscustomobject]@{ key = "service_availability"; label = "Brigade service availability"; terms = @("Service", "Servicio", "AvailableService", "ServiceAvailability") },
[pscustomobject]@{ key = "clinical_encounter"; label = "Clinical encounter"; terms = @("Encounter", "Consultation", "Consulta", "Clinical", "Medical") },
[pscustomobject]@{ key = "consent_privacy"; label = "Consent and privacy"; terms = @("Consent", "Privacy", "Privacidad", "Aviso", "Signature", "Firma") },
[pscustomobject]@{ key = "longitudinal_history"; label = "Longitudinal history"; terms = @("History", "Historial", "Timeline", "Longitudinal") },
[pscustomobject]@{ key = "offline_first"; label = "Offline-first synchronization"; terms = @("Offline", "Sync", "Outbox", "Inbox", "Idempotency", "Conflict", "ClientOperationId") },
[pscustomobject]@{ key = "dashboards"; label = "Dashboards"; terms = @("Dashboard", "Dashboards") },
[pscustomobject]@{ key = "analytics"; label = "Analytics"; terms = @("Analytics", "Metric", "Metrics", "Kpi", "Indicator", "Statistics") },
[pscustomobject]@{ key = "reports_exports"; label = "Reports and exports"; terms = @("Report", "Reports", "Export", "Csv", "Xlsx", "Excel") },
[pscustomobject]@{ key = "audit_trail"; label = "Audit trail"; terms = @("Audit", "Auditable", "AuditTrail") },
[pscustomobject]@{ key = "authorization"; label = "Authorization"; terms = @("Authorization", "Permission", "Role", "Policy") },
[pscustomobject]@{ key = "sql_server"; label = "SQL Server persistence"; terms = @("SqlServer", "ConnectionStrings__SqlServer", "UseSqlServer") }
)

$AllInspectableFiles = @($SourceFiles + $JsonFiles | Sort-Object FullName -Unique)

$DomainCoverage = @()

foreach ($Domain in $DomainDefinitions) {
$Detected = Test-AnyPathOrContent -Files $AllInspectableFiles -Terms $Domain.terms

$DomainCoverage += [pscustomobject]@{
    key = $Domain.key
    label = $Domain.label
    detected = $Detected
    terms = $Domain.terms
}

}

$MissingDomains = @($DomainCoverage | Where-Object { -not $_.detected })

$ProjectInventory = [pscustomobject]@{
api_project_relative_path = $ApiProjectRelativePath
api_project_exists = Test-Path $ApiProjectPath
solution_and_project_files = To-InventoryRows -Files $ProjectFiles
test_projects = To-InventoryRows -Files $TestProjects
}

$SourceSurfaceInventory = [pscustomobject]@{
source_file_count = $SourceFiles.Count
json_file_count = $JsonFiles.Count
controllers = To-InventoryRows -Files $ControllerFiles
endpoints = To-InventoryRows -Files $EndpointFiles
contracts = To-InventoryRows -Files $ContractFiles
entities = To-InventoryRows -Files $EntityFiles
db_contexts = To-InventoryRows -Files $DbContextFiles
migrations = To-InventoryRows -Files $MigrationFiles
configurations = To-InventoryRows -Files $ConfigurationFiles
services = To-InventoryRows -Files $ServiceFiles
authorization = To-InventoryRows -Files $AuthorizationFiles
audit_and_telemetry = To-InventoryRows -Files $AuditFiles
health_and_openapi = To-InventoryRows -Files $HealthOpenApiFiles
tests = To-InventoryRows -Files $TestFiles
}

$GapBacklog = "# P5.1 Backend Surface Gap BacklognnBackend production readiness: BLOCKED_PENDING_REAL_EVIDENCEnn"

if ($MissingDomains.Count -eq 0) {
$GapBacklog += "No missing mandatory backend domains were detected by keyword inventory. This does not prove functional completeness.n" } else { $GapBacklog += "Detected missing or weak backend domains requiring functional closure:n`n"

foreach ($Domain in $MissingDomains) {
    $GapBacklog += "- " + $Domain.label + " | key: " + $Domain.key + " | next action: inspect and implement required backend surface.`n"
}

}

$GapBacklog += "nMandatory future PR groups:n"
$GapBacklog += "- P5.2 patient core.n" $GapBacklog += "- P5.3 brigade and service availability.n"
$GapBacklog += "- P5.4 clinical encounters.n" $GapBacklog += "- P5.5 consent and privacy.n"
$GapBacklog += "- P5.6 longitudinal history.n" $GapBacklog += "- P5.7 clinical audit proof.n"
$GapBacklog += "- P5.8 reports and exports.n" $GapBacklog += "- P6 offline-first synchronization.n"
$GapBacklog += "- P7 dashboards and analytics.n" $GapBacklog += "- P8 institutional production readiness.n"

$Summary = "# P5.1 Backend Surface Summarynn"
$Summary += "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCEnn"
$Summary += "| Area | Count |n" $Summary += "|---|---:|n"
$Summary += "| Source files | $($SourceFiles.Count) |n" $Summary += "| Controllers | $($ControllerFiles.Count) |n"
$Summary += "| Endpoint files | $($EndpointFiles.Count) |n" $Summary += "| Contract files | $($ContractFiles.Count) |n"
$Summary += "| Entity files | $($EntityFiles.Count) |n" $Summary += "| DbContext files | $($DbContextFiles.Count) |n"
$Summary += "| Migration files | $($MigrationFiles.Count) |n" $Summary += "| Service files | $($ServiceFiles.Count) |n"
$Summary += "| Authorization files | $($AuthorizationFiles.Count) |n" $Summary += "| Audit and telemetry files | $($AuditFiles.Count) |n"
$Summary += "| Health and OpenAPI files | $($HealthOpenApiFiles.Count) |n" $Summary += "| Test projects | $($TestProjects.Count) |n"
$Summary += "| Test files | $($TestFiles.Count) |n" $Summary += "| Missing or weak mandatory domains | $($MissingDomains.Count) |n"

$ProjectInventoryPath = Join-Path $EvidenceDir "project-inventory.json"
$SourceSurfaceInventoryPath = Join-Path $EvidenceDir "source-surface-inventory.json"
$DomainCoveragePath = Join-Path $EvidenceDir "domain-coverage.json"
$SummaryPath = Join-Path $EvidenceDir "backend-surface-summary.md"
$GapBacklogPath = Join-Path $EvidenceDir "gap-backlog.md"

Write-TextFile -Path $ProjectInventoryPath -Content ($ProjectInventory | ConvertTo-Json -Depth 20)
Write-TextFile -Path $SourceSurfaceInventoryPath -Content ($SourceSurfaceInventory | ConvertTo-Json -Depth 20)
Write-TextFile -Path $DomainCoveragePath -Content ($DomainCoverage | ConvertTo-Json -Depth 20)
Write-TextFile -Path $SummaryPath -Content $Summary
Write-TextFile -Path $GapBacklogPath -Content $GapBacklog

$Manifest = [pscustomobject]@{
phase = "P5.1 Backend Surface Inventory"
backend_production_readiness = "BLOCKED_PENDING_REAL_EVIDENCE"
generated_at = (Get-Date).ToString("o")
evidence_output_root = $EvidenceDir
api_project_relative_path = $ApiProjectRelativePath
api_project_exists = Test-Path $ApiProjectPath
source_file_count = $SourceFiles.Count
controller_file_count = $ControllerFiles.Count
endpoint_file_count = $EndpointFiles.Count
contract_file_count = $ContractFiles.Count
entity_file_count = $EntityFiles.Count
db_context_file_count = $DbContextFiles.Count
migration_file_count = $MigrationFiles.Count
test_project_count = $TestProjects.Count
missing_or_weak_domain_count = $MissingDomains.Count
offline_first_required_for_final_system = $true
dashboards_required_for_final_system = $true
analytics_required_for_final_system = $true
longitudinal_history_required_for_final_system = $true
output_files = @{
project_inventory = $ProjectInventoryPath
source_surface_inventory = $SourceSurfaceInventoryPath
domain_coverage = $DomainCoveragePath
backend_surface_summary = $SummaryPath
gap_backlog = $GapBacklogPath
}
}

$ManifestPath = Join-Path $EvidenceDir "manifest.json"
Write-TextFile -Path $ManifestPath -Content ($Manifest | ConvertTo-Json -Depth 20)

Write-Host ""
Write-Host "P5.1 Backend Surface Inventory completed."
Write-Host ("Manifest: {0}" -f $ManifestPath)
Write-Host ("Summary: {0}" -f $SummaryPath)
Write-Host ("Gap backlog: {0}" -f $GapBacklogPath)