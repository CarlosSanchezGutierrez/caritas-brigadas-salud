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
    "docs/qa/P3_20_CLIENT_API_CONTRACT_TEST_HARNESS_BASELINE.md",
    "docs/qa/P3_20_CROSS_CLIENT_CONTRACT_TEST_SCENARIOS.md",
    "docs/web/P3_20_WEB_CONTRACT_TEST_HARNESS.md",
    "docs/mobile/P3_20_IOS_CONTRACT_TEST_HARNESS.md",
    "docs/mobile/P3_20_ANDROID_CONTRACT_TEST_HARNESS.md",
    "docs/api/P3_20_SCHEMA_DRIFT_AND_BREAKING_CHANGE_GATE.md",
    "docs/runbooks/P3_20_CONTRACT_TEST_HARNESS_RUNBOOK.md",
    "docs/operations/templates/P3_20_CONTRACT_TEST_HARNESS_TEMPLATE.md",
    "scripts/verify-p3-20-client-api-contract-test-harness.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Client API Contract Test Harness Baseline",
    "cross-client contract test scenarios",
    "Web contract test harness",
    "iOS contract test harness",
    "Android contract test harness",
    "schema drift detection",
    "breaking change detection",
    "API contract version",
    "endpoint id",
    "request schema",
    "response schema",
    "request metadata model",
    "response metadata model",
    "standard error envelope model",
    "authentication requirement",
    "authorization role",
    "organization id",
    "request id",
    "correlation id",
    "audit trail reference",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id",
    "schema drift evidence",
    "breaking change evidence",
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
    "contract tests may be skipped",
    "schema drift may be accepted silently",
    "breaking changes may bypass review"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.20 client API contract test harness verifier passed from repo root: $RepoRoot"
