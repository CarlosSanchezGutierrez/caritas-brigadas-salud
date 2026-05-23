# P4.1 Real Evidence Collector
# Evidence output root: artifacts/p4/p4-01-real-evidence-baseline
# Manifest: manifest.json
# Required configuration key: ConnectionStrings__SqlServer
# Captures: dotnet restore, dotnet build, dotnet test, P3 governance verifier evidence, P4 verifier evidence, API health check evidence
# Safety: sanitized evidence only, no secrets in repository, real evidence only

param(
    [string]$OutputRoot = "artifacts/p4/p4-01-real-evidence-baseline",
    [string]$ApiBaseUrl = "",
    [switch]$AllowDirty
)

$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) { $PSCommandPath } elseif ($MyInvocation.MyCommand.Path) { $MyInvocation.MyCommand.Path } else { throw "Unable to resolve script path." }
$ScriptDirectory = Split-Path -Parent $ScriptPath
$RepoRoot = Resolve-Path (Join-Path $ScriptDirectory "..\..")
Set-Location $RepoRoot

$RunStamp = Get-Date -Format "yyyyMMdd-HHmmss"
$EvidenceDir = Join-Path $RepoRoot (Join-Path $OutputRoot $RunStamp)
[System.IO.Directory]::CreateDirectory($EvidenceDir) | Out-Null

$Results = New-Object System.Collections.Generic.List[object]

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

function Convert-ToSafeFileName {
    param([Parameter(Mandatory = $true)][string]$Name)
    return ($Name -replace "[^A-Za-z0-9._-]", "_")
}

function Redact-Text {
    param([AllowEmptyString()][string]$Text)

    if ([string]::IsNullOrEmpty($Text)) {
        return ""
    }

    $Safe = $Text
    $Safe = $Safe -replace "(?i)(ConnectionStrings__SqlServer\s*[:=]\s*).+", '$1[REDACTED]'
    $Safe = $Safe -replace "(?i)(Server\s*=\s*[^;]+;.*)", "[REDACTED_CONNECTION_STRING]"
    $Safe = $Safe -replace "(?i)(User\s+ID\s*=\s*[^;]+)", "User ID=[REDACTED]"
    $Safe = $Safe -replace "(?i)(Pwd\s*=\s*[^;]+)", "Pwd=[REDACTED]"
    $Safe = $Safe -replace "(?i)(Secret\s*[:=]\s*)\S+", '$1[REDACTED]'
    $Safe = $Safe -replace "(?i)(Token\s*[:=]\s*)\S+", '$1[REDACTED]'
    return $Safe
}

function Add-Result {
    param(
        [string]$Name,
        [string]$Status,
        [int]$ExitCode,
        [string]$LogPath,
        [bool]$Required,
        [string]$Blocker
    )

    $Results.Add([pscustomobject]@{
        name = $Name
        status = $Status
        exit_code = $ExitCode
        required = $Required
        blocker = $Blocker
        log_path = $LogPath
    }) | Out-Null
}

function Invoke-EvidenceStep {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][scriptblock]$Command,
        [bool]$Required = $true
    )

    $SafeName = Convert-ToSafeFileName $Name
    $LogPath = Join-Path $EvidenceDir "$SafeName.log"

    $global:LASTEXITCODE = 0
    $Output = ""
    $ExitCode = 0
    $Status = "passed"
    $Blocker = ""

    try {
        $Output = (& $Command 2>&1 | Out-String)
        if ($null -ne $global:LASTEXITCODE) {
            $ExitCode = [int]$global:LASTEXITCODE
        }
        else {
            $ExitCode = 0
        }

        if ($ExitCode -ne 0) {
            $Status = if ($Required) { "failed" } else { "skipped_or_failed_optional" }
            $Blocker = "command exit code $ExitCode"
        }
    }
    catch {
        $Output = $_.Exception.ToString()
        $ExitCode = 1
        $Status = if ($Required) { "failed" } else { "skipped_or_failed_optional" }
        $Blocker = $_.Exception.Message
    }

    $SafeOutput = Redact-Text $Output
    Write-TextFile -Path $LogPath -Content $SafeOutput
    Add-Result -Name $Name -Status $Status -ExitCode $ExitCode -LogPath $LogPath -Required $Required -Blocker $Blocker
}

Write-TextFile -Path (Join-Path $EvidenceDir "README.txt") -Content @"
P4.1 Real Evidence Execution Baseline
Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
SQL Server is the operational source of truth.
Evidence output root: $EvidenceDir
Sanitized evidence only.
"@

$DirtyStatus = (& git status --porcelain 2>&1 | Out-String).Trim()
if (-not $AllowDirty -and -not [string]::IsNullOrWhiteSpace($DirtyStatus)) {
    Write-TextFile -Path (Join-Path $EvidenceDir "repository-dirty-state.log") -Content (Redact-Text $DirtyStatus)
    Add-Result -Name "repository clean state evidence" -Status "failed" -ExitCode 1 -LogPath (Join-Path $EvidenceDir "repository-dirty-state.log") -Required $true -Blocker "Repository has uncommitted changes. Re-run after clean checkout or pass -AllowDirty intentionally."
}
else {
    Invoke-EvidenceStep -Name "repository clean state evidence" -Command { git status -sb } -Required $true
}

Invoke-EvidenceStep -Name "git commit SHA evidence" -Command { git rev-parse HEAD } -Required $true
Invoke-EvidenceStep -Name "git branch evidence" -Command { git branch --show-current } -Required $true
Invoke-EvidenceStep -Name "dotnet info evidence" -Command { dotnet --info } -Required $true

$Solution = Get-ChildItem -Path $RepoRoot -Filter "*.sln" -File -Recurse | Select-Object -First 1
$Projects = @(Get-ChildItem -Path $RepoRoot -Filter "*.csproj" -File -Recurse)
$BuildTarget = $null

if ($Solution) {
    $BuildTarget = $Solution.FullName
}
elseif ($Projects.Count -gt 0) {
    $BuildTarget = $Projects[0].FullName
}

if ($BuildTarget) {
    Invoke-EvidenceStep -Name "dotnet restore evidence" -Command { dotnet restore $BuildTarget } -Required $true
    Invoke-EvidenceStep -Name "dotnet build evidence" -Command { dotnet build $BuildTarget --configuration Release --no-restore } -Required $true

    $TestProjects = @($Projects | Where-Object { $_.FullName -match "(?i)(test|tests)" })
    if ($TestProjects.Count -gt 0) {
        foreach ($TestProject in $TestProjects) {
            Invoke-EvidenceStep -Name ("dotnet test evidence " + $TestProject.BaseName) -Command { dotnet test $TestProject.FullName --configuration Release --no-build } -Required $true
        }
    }
    else {
        $NoTestPath = Join-Path $EvidenceDir "dotnet-test-evidence-no-test-projects.log"
        Write-TextFile -Path $NoTestPath -Content "No test projects found. This is a blocker candidate for P4 real evidence backlog evidence unless tests are intentionally out of scope for this repository slice."
        Add-Result -Name "dotnet test evidence" -Status "skipped_or_blocker_candidate" -ExitCode 0 -LogPath $NoTestPath -Required $true -Blocker "No test projects found."
    }
}
else {
    $NoDotnetPath = Join-Path $EvidenceDir "dotnet-target-missing.log"
    Write-TextFile -Path $NoDotnetPath -Content "No .sln or .csproj file found. dotnet restore evidence, dotnet build evidence, and dotnet test evidence cannot be collected."
    Add-Result -Name "dotnet restore evidence" -Status "failed" -ExitCode 1 -LogPath $NoDotnetPath -Required $true -Blocker "No .NET target found."
    Add-Result -Name "dotnet build evidence" -Status "failed" -ExitCode 1 -LogPath $NoDotnetPath -Required $true -Blocker "No .NET target found."
    Add-Result -Name "dotnet test evidence" -Status "failed" -ExitCode 1 -LogPath $NoDotnetPath -Required $true -Blocker "No .NET target found."
}

Invoke-EvidenceStep -Name "P3 governance verifier evidence" -Command { powershell -ExecutionPolicy Bypass -File "scripts/verify-p3-43-final-production-governance-evidence-index.ps1" } -Required $true
Invoke-EvidenceStep -Name "P4 verifier evidence" -Command { powershell -ExecutionPolicy Bypass -File "scripts/verify-p4-01-real-evidence-execution-baseline.ps1" } -Required $true

$SqlServerConfigPresence = if ([string]::IsNullOrWhiteSpace($env:ConnectionStrings__SqlServer)) { "ConnectionStrings__SqlServer missing" } else { "ConnectionStrings__SqlServer present but value redacted" }
$SqlServerPresencePath = Join-Path $EvidenceDir "sql-server-configuration-presence-evidence.log"
Write-TextFile -Path $SqlServerPresencePath -Content $SqlServerConfigPresence
Add-Result -Name "SQL Server configuration presence evidence" -Status "captured" -ExitCode 0 -LogPath $SqlServerPresencePath -Required $true -Blocker $(if ($SqlServerConfigPresence -match "missing") { "ConnectionStrings__SqlServer missing." } else { "" })

if (-not [string]::IsNullOrWhiteSpace($ApiBaseUrl)) {
    $HealthUrl = $ApiBaseUrl.TrimEnd("/") + "/api/v1/health"
    Invoke-EvidenceStep -Name "API health check evidence" -Command { Invoke-WebRequest -Uri $HealthUrl -UseBasicParsing | Select-Object StatusCode, Content | Format-List } -Required $true
}
else {
    $ApiSkipPath = Join-Path $EvidenceDir "api-health-check-evidence-skipped.log"
    Write-TextFile -Path $ApiSkipPath -Content "API health check evidence skipped because ApiBaseUrl was not provided."
    Add-Result -Name "API health check evidence" -Status "skipped_or_blocker_candidate" -ExitCode 0 -LogPath $ApiSkipPath -Required $false -Blocker "ApiBaseUrl not provided."
}

$OpenApiFiles = @(Get-ChildItem -Path $RepoRoot -Recurse -File -Include "*.json","*.yaml","*.yml" | Where-Object { $_.FullName -match "(?i)(openapi|swagger)" } | Select-Object -First 20)
$OpenApiPath = Join-Path $EvidenceDir "openapi-artifact-evidence.log"
if ($OpenApiFiles.Count -gt 0) {
    Write-TextFile -Path $OpenApiPath -Content (($OpenApiFiles | ForEach-Object { $_.FullName }) -join "`n")
    Add-Result -Name "OpenAPI artifact evidence" -Status "captured" -ExitCode 0 -LogPath $OpenApiPath -Required $false -Blocker ""
}
else {
    Write-TextFile -Path $OpenApiPath -Content "No OpenAPI or Swagger artifact found during P4.1 baseline scan."
    Add-Result -Name "OpenAPI artifact evidence" -Status "skipped_or_blocker_candidate" -ExitCode 0 -LogPath $OpenApiPath -Required $false -Blocker "No OpenAPI artifact found."
}

$Blockers = @($Results | Where-Object { $_.required -eq $true -and $_.status -eq "failed" })
$Manifest = [pscustomobject]@{
    phase = "P4.1 Real Evidence Execution Baseline"
    backend_production_readiness = "BLOCKED_PENDING_REAL_EVIDENCE"
    sql_server_source_of_truth = $true
    evidence_output_root = $EvidenceDir
    generated_at = (Get-Date).ToString("o")
    api_base_url_provided = -not [string]::IsNullOrWhiteSpace($ApiBaseUrl)
    blocker_count = $Blockers.Count
    results = $Results
}

$ManifestPath = Join-Path $EvidenceDir "manifest.json"
Write-TextFile -Path $ManifestPath -Content ($Manifest | ConvertTo-Json -Depth 20)

if ($Blockers.Count -gt 0) {
    Write-Host ""
    Write-Host "P4.1 evidence collection completed with required blockers."
    Write-Host ("Manifest: {0}" -f $ManifestPath)
    exit 1
}

Write-Host ""
Write-Host "P4.1 evidence collection completed."
Write-Host ("Manifest: {0}" -f $ManifestPath)
exit 0