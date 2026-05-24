param(
    [Parameter(Mandatory = $true)][string]$ManifestPath,
    [string]$OutputDirectory = ""
)

$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) { $PSCommandPath } elseif ($MyInvocation.MyCommand.Path) { $MyInvocation.MyCommand.Path } else { throw "Unable to resolve script path." }
$ScriptDirectory = Split-Path -Parent $ScriptPath
$RepoRoot = Resolve-Path (Join-Path $ScriptDirectory "..\..")

Set-Location $RepoRoot

function Resolve-InputPath {
    param([Parameter(Mandatory = $true)][string]$Path)

    if ([System.IO.Path]::IsPathRooted($Path)) {
        return $Path
    }

    return Join-Path $RepoRoot $Path
}

function Write-Utf8NoBom {
    param(
        [Parameter(Mandatory = $true)][string]$Path,
        [Parameter(Mandatory = $true)][string]$Content
    )

    $Parent = Split-Path -Parent $Path

    if (-not [string]::IsNullOrWhiteSpace($Parent)) {
        [System.IO.Directory]::CreateDirectory($Parent) | Out-Null
    }

    $Utf8NoBom = New-Object System.Text.UTF8Encoding($false)
    [System.IO.File]::WriteAllText($Path, $Content, $Utf8NoBom)
}

function Convert-ToSafeText {
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

function Get-Category {
    param([AllowEmptyString()][string]$Name)

    $Value = $Name.ToLowerInvariant()

    if ($Value -match "repository|git branch|git commit|clean state") { return "repository" }
    if ($Value -match "dotnet|restore|build|test") { return "build-test" }
    if ($Value -match "p3 governance|p4 verifier|verifier|blocker register") { return "governance" }
    if ($Value -match "sql server|connectionstrings__sqlserver|database") { return "database-config" }
    if ($Value -match "api health|runtime|health") { return "api-runtime" }
    if ($Value -match "openapi|swagger|endpoint contract") { return "api-contract" }
    if ($Value -match "monitoring|alerting|support diagnostic") { return "observability" }
    if ($Value -match "audit|privacy|security|sanitization|secret") { return "security-privacy" }
    if ($Value -match "mobile|device|offline|sync|conflict") { return "mobile-sync" }

    return "unknown"
}

function Get-OwnerGroup {
    param([string]$Category)

    switch ($Category) {
        "repository" { return "technical owner" }
        "build-test" { return "technical owner" }
        "governance" { return "compliance owner" }
        "database-config" { return "data owner" }
        "api-runtime" { return "operations owner" }
        "api-contract" { return "technical owner" }
        "observability" { return "operations owner" }
        "security-privacy" { return "security owner" }
        "mobile-sync" { return "mobile owner" }
        default { return "technical owner" }
    }
}

function Get-RemediationType {
    param([string]$Category, [string]$Severity)

    if ($Severity -eq "PASS") {
        return "none"
    }

    switch ($Category) {
        "repository" { return "repository hygiene" }
        "build-test" { return "build or test remediation" }
        "governance" { return "governance verifier remediation" }
        "database-config" { return "configuration or SQL Server remediation" }
        "api-runtime" { return "API runtime remediation" }
        "api-contract" { return "API contract remediation" }
        "observability" { return "monitoring and alerting remediation" }
        "security-privacy" { return "security privacy remediation" }
        "mobile-sync" { return "mobile sync remediation" }
        default { return "manual triage" }
    }
}

function Get-Severity {
    param(
        [string]$Status,
        [bool]$Required,
        [int]$ExitCode,
        [string]$Blocker
    )

    $SafeStatus = if ($null -eq $Status) { "" } else { $Status.ToLowerInvariant() }
    $SafeBlocker = if ($null -eq $Blocker) { "" } else { $Blocker.Trim() }

    if ($Required -and ($SafeStatus -eq "failed" -or $ExitCode -ne 0)) { return "P0" }
    if ($Required -and ($SafeStatus -match "skipped|blocker" -or -not [string]::IsNullOrWhiteSpace($SafeBlocker))) { return "P1" }
    if (-not $Required -and ($SafeStatus -match "skipped|failed|blocker" -or -not [string]::IsNullOrWhiteSpace($SafeBlocker))) { return "P2" }
    if ($SafeStatus -match "passed|captured") { return "PASS" }

    return "UNKNOWN"
}

$ResolvedManifestPath = Resolve-InputPath -Path $ManifestPath

if (-not (Test-Path $ResolvedManifestPath)) {
    throw "Manifest path not found: $ResolvedManifestPath"
}

$ManifestText = [System.IO.File]::ReadAllText($ResolvedManifestPath)
$Manifest = $ManifestText | ConvertFrom-Json

if ($Manifest.phase -ne "P4.1 Real Evidence Execution Baseline") {
    throw "Invalid manifest phase: $($Manifest.phase)"
}

if ($Manifest.backend_production_readiness -ne "BLOCKED_PENDING_REAL_EVIDENCE") {
    throw "Manifest does not preserve Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE"
}

if ($null -eq $Manifest.results) {
    throw "Manifest is missing results."
}

$ManifestResults = @($Manifest.results)

if ($ManifestResults.Count -eq 0) {
    throw "Manifest results array is empty."
}

if ([string]::IsNullOrWhiteSpace($OutputDirectory)) {
    $OutputDirectory = Split-Path -Parent $ResolvedManifestPath
}
else {
    $OutputDirectory = Resolve-InputPath -Path $OutputDirectory
}

[System.IO.Directory]::CreateDirectory($OutputDirectory) | Out-Null

$ClassifiedItems = New-Object System.Collections.Generic.List[object]

foreach ($Result in $ManifestResults) {
    $Name = Convert-ToSafeText ([string]$Result.name)
    $Status = Convert-ToSafeText ([string]$Result.status)
    $Blocker = Convert-ToSafeText ([string]$Result.blocker)
    $LogPath = Convert-ToSafeText ([string]$Result.log_path)

    $ExitCode = 0

    if ($null -ne $Result.exit_code) {
        $ExitCode = [int]$Result.exit_code
    }

    $Required = $false

    if ($null -ne $Result.required) {
        $Required = [bool]$Result.required
    }

    $Category = Get-Category -Name $Name
    $Severity = Get-Severity -Status $Status -Required $Required -ExitCode $ExitCode -Blocker $Blocker
    $OwnerGroup = Get-OwnerGroup -Category $Category
    $RemediationType = Get-RemediationType -Category $Category -Severity $Severity

    $ClassifierDecision = "unknown classification"

    if ($Severity -eq "PASS") {
        $ClassifierDecision = "accepted evidence"
    }
    elseif ($Severity -eq "P0") {
        $ClassifierDecision = "required blocker"
    }
    elseif ($Severity -eq "P1") {
        $ClassifierDecision = "blocker candidate"
    }
    elseif ($Severity -eq "P2") {
        $ClassifierDecision = "optional evidence gap"
    }

    $ClassifiedItems.Add([pscustomobject]@{
        evidence_name = $Name
        original_status = $Status
        command_exit_code = $ExitCode
        required_blocker_flag = $Required
        blocker_text = $Blocker
        blocker_severity = $Severity
        blocker_category = $Category
        blocker_owner_group = $OwnerGroup
        remediation_type = $RemediationType
        source_log_path = $LogPath
        evidence_source = $ResolvedManifestPath
        classifier_decision = $ClassifierDecision
        sanitized_evidence_only = $true
        backend_production_readiness = "BLOCKED_PENDING_REAL_EVIDENCE"
    }) | Out-Null
}

$Summary = [pscustomobject]@{
    phase = "P4.2 Real Evidence Package Classification"
    backend_production_readiness = "BLOCKED_PENDING_REAL_EVIDENCE"
    source_manifest = $ResolvedManifestPath
    generated_at = (Get-Date).ToString("o")
    total_items = $ClassifiedItems.Count
    p0_required_blockers = @($ClassifiedItems | Where-Object { $_.blocker_severity -eq "P0" }).Count
    p1_blocker_candidates = @($ClassifiedItems | Where-Object { $_.blocker_severity -eq "P1" }).Count
    p2_optional_evidence_gaps = @($ClassifiedItems | Where-Object { $_.blocker_severity -eq "P2" }).Count
    pass_items = @($ClassifiedItems | Where-Object { $_.blocker_severity -eq "PASS" }).Count
    unknown_items = @($ClassifiedItems | Where-Object { $_.blocker_severity -eq "UNKNOWN" }).Count
    results = $ClassifiedItems
}

$JsonPath = Join-Path $OutputDirectory "p4-02-classification.json"
Write-Utf8NoBom -Path $JsonPath -Content ($Summary | ConvertTo-Json -Depth 20)

$MarkdownPath = Join-Path $OutputDirectory "p4-02-blocker-backlog.md"

$MarkdownLines = New-Object System.Collections.Generic.List[string]
$MarkdownLines.Add("# P4.2 Real Evidence Blocker Backlog") | Out-Null
$MarkdownLines.Add("") | Out-Null
$MarkdownLines.Add("Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE") | Out-Null
$MarkdownLines.Add("") | Out-Null
$MarkdownLines.Add("Source manifest: $ResolvedManifestPath") | Out-Null
$MarkdownLines.Add("") | Out-Null
$MarkdownLines.Add("| Severity | Category | Evidence | Status | Exit code | Owner | Remediation | Blocker |") | Out-Null
$MarkdownLines.Add("|---|---|---|---|---:|---|---|---|") | Out-Null

foreach ($Item in $ClassifiedItems | Sort-Object blocker_severity, blocker_category, evidence_name) {
    $BlockerText = if ([string]::IsNullOrWhiteSpace($Item.blocker_text)) { "" } else { $Item.blocker_text.Replace("|", "/") }
    $EvidenceName = $Item.evidence_name.Replace("|", "/")
    $MarkdownLines.Add("| $($Item.blocker_severity) | $($Item.blocker_category) | $EvidenceName | $($Item.original_status) | $($Item.command_exit_code) | $($Item.blocker_owner_group) | $($Item.remediation_type) | $BlockerText |") | Out-Null
}

$MarkdownLines.Add("") | Out-Null
$MarkdownLines.Add("real evidence only.") | Out-Null
$MarkdownLines.Add("sanitized evidence only.") | Out-Null
$MarkdownLines.Add("SQL Server is the operational source of truth.") | Out-Null
$MarkdownLines.Add("Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE") | Out-Null

Write-Utf8NoBom -Path $MarkdownPath -Content (($MarkdownLines -join "`n") + "`n")

Write-Host "P4.2 real evidence classification report created."
Write-Host ("JSON: {0}" -f $JsonPath)
Write-Host ("Markdown: {0}" -f $MarkdownPath)

if ($Summary.p0_required_blockers -gt 0) {
    exit 2
}

exit 0