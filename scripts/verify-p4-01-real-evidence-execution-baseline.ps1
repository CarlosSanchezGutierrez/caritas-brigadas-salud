$ErrorActionPreference = "Stop"

$ScriptPath = if (-not [string]::IsNullOrWhiteSpace($PSCommandPath)) { $PSCommandPath } elseif ($MyInvocation.MyCommand.Path) { $MyInvocation.MyCommand.Path } else { throw "Unable to resolve script path." }
$ScriptDirectory = Split-Path -Parent $ScriptPath
$RepoRoot = Resolve-Path (Join-Path $ScriptDirectory "..")

function Resolve-RepoPath { param([string]$RelativePath) return Join-Path -Path $RepoRoot -ChildPath $RelativePath }
function Assert-FileExists { param([string]$RelativePath) $AbsolutePath = Resolve-RepoPath -RelativePath $RelativePath; if (-not (Test-Path $AbsolutePath)) { throw "Missing required file: $RelativePath resolved to $AbsolutePath" } }
function Read-RepoText { param([string]$RelativePath) $AbsolutePath = Resolve-RepoPath -RelativePath $RelativePath; if (-not (Test-Path $AbsolutePath)) { throw "Cannot read missing file: $RelativePath resolved to $AbsolutePath" }; return [System.IO.File]::ReadAllText($AbsolutePath) }
function Assert-ContainsToken { param([string]$Content, [string]$Token) if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -lt 0) { throw "Missing required token: $Token" } }
function Assert-DoesNotContainToken { param([string]$Content, [string]$Token) if ($Content.IndexOf($Token, [System.StringComparison]::OrdinalIgnoreCase) -ge 0) { throw "Forbidden token found: $Token" } }

$RequiredFiles = @(
    "docs/implementation/P4_01_REAL_EVIDENCE_EXECUTION_BASELINE.md",
    "docs/implementation/P4_01_IMPLEMENTATION_READINESS_HANDOFF.md",
    "docs/qa/P4_01_REAL_EVIDENCE_ACCEPTANCE_MATRIX.md",
    "docs/runbooks/P4_01_REAL_EVIDENCE_CAPTURE_RUNBOOK.md",
    "scripts/p4/collect-p4-01-real-evidence-baseline.ps1",
    "scripts/verify-p4-01-real-evidence-execution-baseline.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) {
    $AllDocumentationContent += "`n--- FILE: $File ---`n"
    $AllDocumentationContent += Read-RepoText -RelativePath $File
}

$CollectorContent = Read-RepoText -RelativePath "scripts/p4/collect-p4-01-real-evidence-baseline.ps1"

$RequiredTokens = @(
    "P4.1 Real Evidence Execution Baseline",
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "ConnectionStrings__SqlServer",
    "P3.43 final production governance evidence index reference",
    "P4 implementation readiness handoff evidence",
    "P4 real evidence backlog evidence",
    "real evidence only",
    "sanitized evidence only",
    "evidence output root",
    "artifacts/p4/p4-01-real-evidence-baseline",
    "manifest.json",
    "command exit code",
    "git commit SHA evidence",
    "repository clean state evidence",
    "dotnet restore evidence",
    "dotnet build evidence",
    "dotnet test evidence",
    "P3 governance verifier evidence",
    "P4 verifier evidence",
    "SQL Server configuration presence evidence",
    "API health check evidence",
    "OpenAPI artifact evidence",
    "endpoint contract evidence",
    "audit trail evidence",
    "support diagnostic evidence",
    "monitoring evidence",
    "alerting evidence",
    "evidence sanitization status",
    "evidence rejection criteria",
    "real environment blocker register",
    "technical owner assignment",
    "operations owner assignment",
    "support owner assignment",
    "security owner assignment",
    "privacy owner assignment",
    "data owner assignment",
    "risk owner assignment",
    "compliance owner assignment",
    "mobile release channel evidence",
    "device fleet evidence",
    "offline sync evidence",
    "conflict resolution evidence",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id",
    "request id",
    "correlation id",
    "organization id",
    "authorization role",
    "endpoint id",
    "standard error envelope",
    "audit trail reference",
    "No secrets in repository",
    "No direct mobile write to SQL Server",
    "No cloud dependency"
)

foreach ($Token in $RequiredTokens) {
    Assert-ContainsToken -Content $AllDocumentationContent -Token $Token
}

$CollectorRequiredTokens = @(
    "P4.1 Real Evidence Collector",
    "artifacts/p4/p4-01-real-evidence-baseline",
    "manifest.json",
    "ConnectionStrings__SqlServer",
    "dotnet restore",
    "dotnet build",
    "dotnet test",
    "api/v1/health",
    "Redact-Text",
    "command exit code",
    "BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "sanitized evidence only",
    "real evidence only"
)

foreach ($Token in $CollectorRequiredTokens) {
    Assert-ContainsToken -Content $CollectorContent -Token $Token
}

$FileSpecificTokens = @{
    "docs/implementation/P4_01_REAL_EVIDENCE_EXECUTION_BASELINE.md" = @(
        "P4.1 Real Evidence Execution Baseline",
        "ConnectionStrings__SqlServer",
        "real evidence only",
        "artifacts/p4/p4-01-real-evidence-baseline",
        "manifest.json",
        "dotnet build evidence",
        "dotnet test evidence",
        "SQL Server configuration presence evidence"
    );
    "docs/implementation/P4_01_IMPLEMENTATION_READINESS_HANDOFF.md" = @(
        "P4.1 Implementation Readiness Handoff",
        "P3.43 final production governance evidence index reference",
        "P4 implementation readiness handoff evidence",
        "P4 real evidence backlog evidence",
        "technical owner assignment",
        "security owner assignment",
        "data owner assignment"
    );
    "docs/qa/P4_01_REAL_EVIDENCE_ACCEPTANCE_MATRIX.md" = @(
        "P4.1 Real Evidence Acceptance Matrix",
        "repository clean state evidence",
        "git commit SHA evidence",
        "dotnet restore evidence",
        "dotnet build evidence",
        "dotnet test evidence",
        "SQL Server configuration presence evidence",
        "real environment blocker register"
    );
    "docs/runbooks/P4_01_REAL_EVIDENCE_CAPTURE_RUNBOOK.md" = @(
        "P4.1 Real Evidence Capture Runbook",
        "scripts/p4/collect-p4-01-real-evidence-baseline.ps1",
        "artifacts/p4/p4-01-real-evidence-baseline",
        "manifest.json",
        "ConnectionStrings__SqlServer",
        "P4.1 real evidence collector"
    )
}

foreach ($Entry in $FileSpecificTokens.GetEnumerator()) {
    $FileContent = Read-RepoText -RelativePath $Entry.Key
    foreach ($Token in $Entry.Value) {
        Assert-ContainsToken -Content $FileContent -Token $Token
    }
}

$ForbiddenDocumentationTokens = @(
    "ConnectionStrings__CaritasDatabase",
    "User ID=sa",
    "Cloud is required",
    "Azure is required",
    "AWS is required",
    "mobile clients may write directly to SQL Server",
    "frontend may bypass API",
    "repository intentionally stores secrets",
    "real patient data is committed intentionally",
    "undocumented endpoints are allowed",
    "conflicts may be silently overwritten",
    "backend production readiness is approved",
    "backend is production ready"
)

foreach ($Token in $ForbiddenDocumentationTokens) {
    Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token
}

Write-Host "P4.1 real evidence execution baseline verifier passed from repo root: $RepoRoot"