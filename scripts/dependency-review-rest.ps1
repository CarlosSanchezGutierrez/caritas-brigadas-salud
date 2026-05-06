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

$Repository = $env:REPOSITORY
$BaseSha = $env:BASE_SHA
$HeadSha = $env:HEAD_SHA
$FailOnSeverity = if ([string]::IsNullOrWhiteSpace($env:FAIL_ON_SEVERITY)) { "high" } else { $env:FAIL_ON_SEVERITY }

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

$RawResponse = gh api $Endpoint -H "Accept: application/vnd.github+json" -H "X-GitHub-Api-Version: 2022-11-28"

if ($LASTEXITCODE -ne 0) {
    throw "GitHub Dependency Review API request failed."
}

$Changes = @($RawResponse | ConvertFrom-Json)
$BlockingFindings = New-Object System.Collections.Generic.List[object]
$VulnerabilityCount = 0

foreach ($Dependency in $Changes) {
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
Write-Host "Added: $Added"
Write-Host "Removed: $Removed"
Write-Host "Changed: $Changed"
Write-Host "Total vulnerabilities found: $VulnerabilityCount"
Write-Host "Blocking vulnerabilities found: $($BlockingFindings.Count)"

if (-not [string]::IsNullOrWhiteSpace($env:GITHUB_STEP_SUMMARY)) {
    $SummaryLines = @(
        "# Dependency Review",
        "",
        "Dependency Review was executed using the GitHub REST API instead of the JavaScript action to avoid clean-run annotations.",
        "",
        "| Metric | Value |",
        "| --- | ---: |",
        "| Dependency changes scanned | $($Changes.Count) |",
        "| Added | $Added |",
        "| Removed | $Removed |",
        "| Changed | $Changed |",
        "| Vulnerabilities found | $VulnerabilityCount |",
        "| Blocking vulnerabilities | $($BlockingFindings.Count) |",
        "| Blocking threshold | $FailOnSeverity |"
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
