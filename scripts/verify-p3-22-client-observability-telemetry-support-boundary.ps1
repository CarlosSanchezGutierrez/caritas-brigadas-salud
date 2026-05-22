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
    "docs/observability/P3_22_CLIENT_OBSERVABILITY_TELEMETRY_SUPPORT_BOUNDARY.md",
    "docs/web/P3_22_WEB_OBSERVABILITY_TELEMETRY_BOUNDARY.md",
    "docs/mobile/P3_22_IOS_OBSERVABILITY_TELEMETRY_BOUNDARY.md",
    "docs/mobile/P3_22_ANDROID_OBSERVABILITY_TELEMETRY_BOUNDARY.md",
    "docs/security/P3_22_PRIVACY_SAFE_CLIENT_TELEMETRY_BOUNDARY.md",
    "docs/qa/P3_22_CLIENT_OBSERVABILITY_TEST_MATRIX.md",
    "docs/runbooks/P3_22_CLIENT_OBSERVABILITY_SUPPORT_RUNBOOK.md",
    "docs/operations/templates/P3_22_CLIENT_OBSERVABILITY_TEMPLATE.md",
    "scripts/verify-p3-22-client-observability-telemetry-support-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Client Observability Telemetry and Support Boundary",
    "Web observability telemetry",
    "iOS observability telemetry",
    "Android observability telemetry",
    "Privacy Safe Client Telemetry Boundary",
    "Client Observability Test Matrix",
    "request id",
    "correlation id",
    "organization id",
    "endpoint id",
    "API contract version",
    "environment name",
    "client target",
    "build profile",
    "release channel",
    "standard error envelope",
    "audit trail reference",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id",
    "privacy-safe redaction",
    "support diagnostic evidence",
    "contract test status",
    "configuration test status",
    "telemetry redaction status",
    "No secrets in repository",
    "No direct mobile write to SQL Server",
    "No cloud dependency"
)

foreach ($Token in $RequiredTokens) { Assert-ContainsToken -Content $AllDocumentationContent -Token $Token }

$ForbiddenTokens = @(
    "ConnectionStrings__CaritasDatabase",
    "password=",
    "User ID=sa",
    "Cloud is required",
    "Azure is required",
    "AWS is required",
    "mobile clients may write directly to SQL Server",
    "frontend may bypass API",
    "repository intentionally stores secrets",
    "real patient data is committed intentionally",
    "mocked data is production evidence",
    "undocumented endpoints are allowed",
    "conflicts may be silently overwritten",
    "telemetry contains raw patient payloads",
    "client telemetry is production approval"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.22 client observability telemetry support boundary verifier passed from repo root: $RepoRoot"
