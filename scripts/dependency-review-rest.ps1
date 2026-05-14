$ErrorActionPreference = "Stop"

function Get-SeverityRank {
    param([string]$Severity)

    switch ($Severity.ToLowerInvariant()) {
        "low" { return 1 }
        "moderate" { return 2 }
        "medium" { return 2 }
        "high" { return 3 }
        "critical" { return 4 }
        default { return 0 }
    }
}

function Get-PositiveIntFromEnvironment {
    param(
        [string]$Name,
        [int]$DefaultValue
    )

    $RawValue = [string][Environment]::GetEnvironmentVariable($Name)

    if ([string]::IsNullOrWhiteSpace($RawValue)) {
        return $DefaultValue
    }

    $ParsedValue = 0

    if ([int]::TryParse($RawValue, [ref]$ParsedValue) -and $ParsedValue -gt 0) {
        return $ParsedValue
    }

    throw "$Name must be a positive integer."
}

function Invoke-DependencyReviewApiWithRetry {
    param(
        [string]$Endpoint,
        [int]$MaxAttempts,
        [int]$InitialDelaySeconds
    )

    $Attempt = 1
    $DelaySeconds = $InitialDelaySeconds
    $LastOutput = ""

    while ($Attempt -le $MaxAttempts) {
        Write-Host "Dependency Review API attempt $Attempt of $MaxAttempts."

        $Output = & gh api $Endpoint `
            -H "Accept: application/vnd.github+json" `
            -H "X-GitHub-Api-Version: 2022-11-28" 2>&1

        $ExitCode = $LASTEXITCODE
        $OutputText = [string]::Join("`n", @($Output | ForEach-Object { [string]$_ }))
        $LastOutput = $OutputText

        if ($ExitCode -eq 0 -and -not [string]::IsNullOrWhiteSpace($OutputText)) {
            return $OutputText
        }

        Write-Warning "Dependency Review API attempt $Attempt failed with exit code $ExitCode."

        if (-not [string]::IsNullOrWhiteSpace($OutputText)) {
            Write-Warning $OutputText
        }

        if ($Attempt -lt $MaxAttempts) {
            Write-Host "Retrying Dependency Review API in $DelaySeconds seconds."
            Start-Sleep -Seconds $DelaySeconds
            $DelaySeconds = [Math]::Min($DelaySeconds * 2, 30)
        }

        $Attempt++
    }

    if (-not [string]::IsNullOrWhiteSpace($env:GITHUB_STEP_SUMMARY)) {
        Add-Content -Path $env:GITHUB_STEP_SUMMARY -Value @"
# Dependency Review API failure

Dependency Review REST API failed after $MaxAttempts attempts.

The gate failed closed because dependency review could not be completed.

Last API output:


$LastOutput

"@
    }

    throw "GitHub Dependency Review API request failed after $MaxAttempts attempts."
}

$Repository = $env:REPOSITORY
$BaseSha = $env:BASE_SHA
$HeadSha = $env:HEAD_SHA
$FailOnSeverity = if ([string]::IsNullOrWhiteSpace($env:FAIL_ON_SEVERITY)) { "high" } else { $env:FAIL_ON_SEVERITY }
$MaxAttempts = Get-PositiveIntFromEnvironment "DEPENDENCY_REVIEW_MAX_ATTEMPTS" 4
$InitialDelaySeconds = Get-PositiveIntFromEnvironment "DEPENDENCY_REVIEW_INITIAL_DELAY_SECONDS" 2

if ([string]::IsNullOrWhiteSpace($env:GH_TOKEN)) {
    throw "GH_TOKEN is required."
}

if ([string]::IsNullOrWhiteSpace($Repository)) {
    throw "REPOSITORY is required."
}

if ([string]::IsNullOrWhiteSpace($BaseSha)) {
    throw "BASE_SHA is required."
}

if ([string]::IsNullOrWhiteSpace($HeadSha)) {
    throw "HEAD_SHA is required."
}

$ThresholdRank = Get-SeverityRank $FailOnSeverity

if ($ThresholdRank -le 0) {
    throw "Unsupported FAIL_ON_SEVERITY value: $FailOnSeverity"
}

$BaseHead = "$BaseSha...$HeadSha"
$Endpoint = "repos/$Repository/dependency-graph/compare/$BaseHead"

Write-Host "=== DEPENDENCY REVIEW REST API CHECK ==="
Write-Host "Repository: $Repository"
Write-Host "Base SHA: $BaseSha"
Write-Host "Head SHA: $HeadSha"
Write-Host "Fail on severity: $FailOnSeverity"
Write-Host "Max API attempts: $MaxAttempts"
Write-Host "Initial retry delay seconds: $InitialDelaySeconds"

$RawResponse = Invoke-DependencyReviewApiWithRetry `
    -Endpoint $Endpoint `
    -MaxAttempts $MaxAttempts `
    -InitialDelaySeconds $InitialDelaySeconds

try {
    $Changes = @($RawResponse | ConvertFrom-Json -ErrorAction Stop)
}
catch {
    throw "GitHub Dependency Review API returned invalid JSON. $($_.Exception.Message)"
}

$ReviewableChanges = @($Changes | Where-Object { $_.change_type -eq "added" -or $_.change_type -eq "changed" })
$BlockingFindings = New-Object System.Collections.Generic.List[object]
$VulnerabilityCount = 0

foreach ($Dependency in $ReviewableChanges) {
    $Vulnerabilities = @($Dependency.vulnerabilities)

    foreach ($Vulnerability in $Vulnerabilities) {
        if ($null -eq $Vulnerability) {
            continue
        }

        $VulnerabilityCount++
        $Severity = [string]$Vulnerability.severity
        $SeverityRank = Get-SeverityRank $Severity

        if ($SeverityRank -ge $ThresholdRank) {
            $BlockingFindings.Add([pscustomobject]@{
                Manifest = $Dependency.manifest
                Ecosystem = $Dependency.ecosystem
                Name = $Dependency.name
                Version = $Dependency.version
                PackageUrl = $Dependency.package_url
                Severity = $Severity
                Advisory = $Vulnerability.advisory_ghsa_id
                Summary = $Vulnerability.advisory_summary
                Url = $Vulnerability.advisory_url
            }) | Out-Null
        }
    }
}

$Added = @($Changes | Where-Object { $_.change_type -eq "added" }).Count
$Removed = @($Changes | Where-Object { $_.change_type -eq "removed" }).Count
$Changed = @($Changes | Where-Object { $_.change_type -eq "changed" }).Count

Write-Host "Dependency changes scanned: $($Changes.Count)"
Write-Host "Reviewable dependency changes scanned: $($ReviewableChanges.Count)"
Write-Host "Added: $Added"
Write-Host "Removed: $Removed"
Write-Host "Changed: $Changed"
Write-Host "Total vulnerabilities found in added/changed dependencies: $VulnerabilityCount"
Write-Host "Blocking vulnerabilities found in added/changed dependencies: $($BlockingFindings.Count)"

if (-not [string]::IsNullOrWhiteSpace($env:GITHUB_STEP_SUMMARY)) {
    $SummaryLines = @(
        "# Dependency Review",
        "",
        "Dependency Review was executed using the GitHub REST API with retry/backoff hardening.",
        "",
        "| Metric | Value |",
        "| --- | ---: |",
        "| Dependency changes scanned | $($Changes.Count) |",
        "| Reviewable dependency changes scanned | $($ReviewableChanges.Count) |",
        "| Added | $Added |",
        "| Removed | $Removed |",
        "| Changed | $Changed |",
        "| Vulnerabilities found in added/changed dependencies | $VulnerabilityCount |",
        "| Blocking vulnerabilities in added/changed dependencies | $($BlockingFindings.Count) |",
        "| Blocking threshold | $FailOnSeverity |",
        "| API max attempts | $MaxAttempts |",
        "| Initial retry delay seconds | $InitialDelaySeconds |"
    )

    if ($BlockingFindings.Count -gt 0) {
        $SummaryLines += ""
        $SummaryLines += "## Blocking vulnerabilities"
        $SummaryLines += ""
        $SummaryLines += "| Package | Version | Severity | Advisory |"
        $SummaryLines += "| --- | --- | --- | --- |"

        foreach ($Finding in $BlockingFindings) {
            $SummaryLines += "| $($Finding.Name) | $($Finding.Version) | $($Finding.Severity) | $($Finding.Advisory) |"
        }
    }

    Add-Content -Path $env:GITHUB_STEP_SUMMARY -Value ($SummaryLines -join "`n")
}

if ($BlockingFindings.Count -gt 0) {
    foreach ($Finding in $BlockingFindings) {
        Write-Host "BLOCKING: $($Finding.Name) $($Finding.Version) $($Finding.Severity) $($Finding.Advisory) $($Finding.Url)"
    }

    throw "Dependency Review REST API check found blocking vulnerabilities."
}

Write-Host "Dependency Review REST API check passed without blocking vulnerabilities."