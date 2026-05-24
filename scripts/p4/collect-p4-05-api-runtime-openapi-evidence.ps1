param(
    [string]$OutputRoot = "artifacts/p4/p4-05-api-runtime-openapi-evidence",
    [string]$ApiBaseUrl = "",
    [switch]$StartLocalApi
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
$ApiProcess = $null

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
    $Safe = $Safe -replace "(?i)(Password\s*=\s*[^;]+)", "Password=[REDACTED]"
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

function Invoke-OptionalEvidenceStep {
    param(
        [Parameter(Mandatory = $true)][string]$Name,
        [Parameter(Mandatory = $true)][scriptblock]$Command
    )

    $SafeName = ($Name -replace "[^A-Za-z0-9._-]", "_")
    $LogPath = Join-Path $EvidenceDir "$SafeName.log"

    $global:LASTEXITCODE = 0
    $Output = ""
    $ExitCode = 0
    $Status = "captured"
    $Blocker = ""

    try {
        $Output = (& $Command 2>&1 | Out-String)

        if ($null -ne $global:LASTEXITCODE) {
            $ExitCode = [int]$global:LASTEXITCODE
        }

        if ($ExitCode -ne 0) {
            $Status = "skipped_or_blocker_candidate"
            $Blocker = "command exit code $ExitCode"
        }
    }
    catch {
        $Output = $_.Exception.ToString()
        $ExitCode = 1
        $Status = "skipped_or_blocker_candidate"
        $Blocker = $_.Exception.Message
    }

    Write-TextFile -Path $LogPath -Content (Redact-Text $Output)
    Add-Result -Name $Name -Status $Status -ExitCode $ExitCode -LogPath $LogPath -Required $false -Blocker $Blocker
}

try {
    Write-TextFile -Path (Join-Path $EvidenceDir "README.txt") -Content @"
P4.5 API Runtime and OpenAPI Evidence
P4.6 API Route Evidence Alignment Applied
Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE
SQL Server is the operational source of truth.
Sanitized evidence only.
"@

    $ApiProjectRelativePath = "services/api-dotnet/src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj"
    $ApiProjectPath = Join-Path $RepoRoot $ApiProjectRelativePath
    $ApiProjectEvidencePath = Join-Path $EvidenceDir "api-project-path-evidence.log"

    if (Test-Path $ApiProjectPath) {
        Write-TextFile -Path $ApiProjectEvidencePath -Content "API project found: $ApiProjectRelativePath"
        Add-Result -Name "API project path evidence" -Status "captured" -ExitCode 0 -LogPath $ApiProjectEvidencePath -Required $false -Blocker ""
    }
    else {
        Write-TextFile -Path $ApiProjectEvidencePath -Content "API project not found: $ApiProjectRelativePath"
        Add-Result -Name "API project path evidence" -Status "skipped_or_blocker_candidate" -ExitCode 0 -LogPath $ApiProjectEvidencePath -Required $false -Blocker "API project path not found."
    }

    if ($StartLocalApi -and (Test-Path $ApiProjectPath)) {
        $StartupLogPath = Join-Path $EvidenceDir "api-startup-attempt-evidence.log"

        try {
            $ApiProcess = Start-Process `
                -FilePath "dotnet" `
                -ArgumentList @("run", "--project", $ApiProjectPath, "--launch-profile", "https") `
                -WorkingDirectory $RepoRoot `
                -PassThru `
                -WindowStyle Hidden

            Start-Sleep -Seconds 18

            Write-TextFile -Path $StartupLogPath -Content "Local API startup attempted with project path: $ApiProjectRelativePath"
            Add-Result -Name "API startup attempt evidence" -Status "captured" -ExitCode 0 -LogPath $StartupLogPath -Required $false -Blocker ""
        }
        catch {
            Write-TextFile -Path $StartupLogPath -Content (Redact-Text $_.Exception.ToString())
            Add-Result -Name "API startup attempt evidence" -Status "skipped_or_blocker_candidate" -ExitCode 1 -LogPath $StartupLogPath -Required $false -Blocker $_.Exception.Message
        }
    }
    else {
        $StartupSkipPath = Join-Path $EvidenceDir "api-startup-attempt-evidence-skipped.log"
        Write-TextFile -Path $StartupSkipPath -Content "API startup attempt skipped because StartLocalApi was not provided or API project was unavailable."
        Add-Result -Name "API startup attempt evidence" -Status "skipped_or_blocker_candidate" -ExitCode 0 -LogPath $StartupSkipPath -Required $false -Blocker "StartLocalApi not provided or API project unavailable."
    }

    if (-not [string]::IsNullOrWhiteSpace($ApiBaseUrl)) {
        $HealthCandidates = @(
            "/health/live",
            "/health/ready"
        )

        foreach ($Candidate in $HealthCandidates) {
            $HealthUrl = $ApiBaseUrl.TrimEnd("/") + $Candidate
            $Name = "API health endpoint evidence " + ($Candidate -replace "[^A-Za-z0-9._-]", "_")

            Invoke-OptionalEvidenceStep -Name $Name -Command {
                Invoke-WebRequest -Uri $HealthUrl -UseBasicParsing -TimeoutSec 10 | Select-Object StatusCode, Content | Format-List
            }
        }

        $OpenApiCandidates = @(
            "/openapi/v1/openapi.json",
            "/swagger",
            "/openapi/v1.json",
            "/openapi.json",
            "/swagger.json"
        )

        foreach ($Candidate in $OpenApiCandidates) {
            $ContractUrl = $ApiBaseUrl.TrimEnd("/") + $Candidate
            $Name = "OpenAPI endpoint evidence " + ($Candidate -replace "[^A-Za-z0-9._-]", "_")

            Invoke-OptionalEvidenceStep -Name $Name -Command {
                Invoke-WebRequest -Uri $ContractUrl -UseBasicParsing -TimeoutSec 10 | Select-Object StatusCode, Content | Format-List
            }
        }
    }
    else {
        $ApiBaseUrlSkipPath = Join-Path $EvidenceDir "api-base-url-evidence-skipped.log"
        Write-TextFile -Path $ApiBaseUrlSkipPath -Content "ApiBaseUrl was not provided. API health and OpenAPI endpoint evidence were not attempted."
        Add-Result -Name "ApiBaseUrl evidence" -Status "skipped_or_blocker_candidate" -ExitCode 0 -LogPath $ApiBaseUrlSkipPath -Required $false -Blocker "ApiBaseUrl not provided."
    }

    $OpenApiFiles = @(Get-ChildItem -Path $RepoRoot -Recurse -File -Include "*.json","*.yaml","*.yml" | Where-Object { $_.FullName -match "(?i)(openapi|swagger)" } | Select-Object -First 20)
    $OpenApiScanPath = Join-Path $EvidenceDir "openapi-artifact-scan-evidence.log"

    if ($OpenApiFiles.Count -gt 0) {
        Write-TextFile -Path $OpenApiScanPath -Content (($OpenApiFiles | ForEach-Object { $_.FullName }) -join "`n")
        Add-Result -Name "OpenAPI artifact scan evidence" -Status "captured" -ExitCode 0 -LogPath $OpenApiScanPath -Required $false -Blocker ""
    }
    else {
        Write-TextFile -Path $OpenApiScanPath -Content "No OpenAPI or Swagger artifact found in repository scan."
        Add-Result -Name "OpenAPI artifact scan evidence" -Status "skipped_or_blocker_candidate" -ExitCode 0 -LogPath $OpenApiScanPath -Required $false -Blocker "No OpenAPI artifact found."
    }

    $Manifest = [pscustomobject]@{
        phase = "P4.5 API Runtime and OpenAPI Evidence"
        route_alignment = "P4.6 API Route Evidence Alignment"
        backend_production_readiness = "BLOCKED_PENDING_REAL_EVIDENCE"
        sql_server_source_of_truth = $true
        evidence_output_root = $EvidenceDir
        generated_at = (Get-Date).ToString("o")
        api_project_relative_path = $ApiProjectRelativePath
        api_base_url_provided = -not [string]::IsNullOrWhiteSpace($ApiBaseUrl)
        start_local_api = [bool]$StartLocalApi
        health_endpoint_candidates = @("/health/live", "/health/ready")
        openapi_endpoint_candidates = @("/openapi/v1/openapi.json", "/swagger", "/openapi/v1.json", "/openapi.json", "/swagger.json")
        results = $Results
    }

    $ManifestPath = Join-Path $EvidenceDir "manifest.json"
    Write-TextFile -Path $ManifestPath -Content ($Manifest | ConvertTo-Json -Depth 20)

    Write-Host ""
    Write-Host "P4.5 API runtime and OpenAPI evidence collection completed with P4.6 route alignment."
    Write-Host ("Manifest: {0}" -f $ManifestPath)
}
finally {
    if ($null -ne $ApiProcess -and -not $ApiProcess.HasExited) {
        Stop-Process -Id $ApiProcess.Id -Force
        Write-Host "Stopped API process."
    }
}