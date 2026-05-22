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
    "docs/ci/P3_23_CLIENT_CI_CD_QUALITY_GATE_BOUNDARY.md",
    "docs/web/P3_23_WEB_BUILD_QUALITY_GATE.md",
    "docs/mobile/P3_23_IOS_BUILD_QUALITY_GATE.md",
    "docs/mobile/P3_23_ANDROID_BUILD_QUALITY_GATE.md",
    "docs/security/P3_23_CLIENT_SUPPLY_CHAIN_AND_SIGNING_BOUNDARY.md",
    "docs/qa/P3_23_CLIENT_QUALITY_GATE_TEST_MATRIX.md",
    "docs/runbooks/P3_23_CLIENT_CI_CD_QUALITY_GATE_RUNBOOK.md",
    "docs/operations/templates/P3_23_CLIENT_CI_CD_QUALITY_GATE_TEMPLATE.md",
    "scripts/verify-p3-23-client-ci-cd-quality-gate-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Client CI CD Quality Gate Boundary",
    "Web build quality gate",
    "iOS build quality gate",
    "Android build quality gate",
    "Client supply chain and signing boundary",
    "Client quality gate test matrix",
    "build reproducibility",
    "dependency review",
    "secret scan",
    "static analysis",
    "formatting check",
    "unit test gate",
    "contract test gate",
    "runtime configuration test gate",
    "observability test gate",
    "privacy-safe telemetry test gate",
    "artifact retention",
    "release channel",
    "build profile",
    "environment name",
    "API contract version",
    "OpenAPI artifact reference",
    "request id",
    "correlation id",
    "organization id",
    "standard error envelope",
    "audit trail reference",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id",
    "schema drift evidence",
    "breaking change evidence",
    "signing boundary",
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
    "secret scan may be skipped",
    "client CI CD is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.23 client CI CD quality gate boundary verifier passed from repo root: $RepoRoot"
