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
    "docs/clients/P3_21_CLIENT_RUNTIME_CONFIGURATION_ENVIRONMENT_BOUNDARY.md",
    "docs/web/P3_21_WEB_RUNTIME_CONFIGURATION_BOUNDARY.md",
    "docs/mobile/P3_21_IOS_RUNTIME_CONFIGURATION_BOUNDARY.md",
    "docs/mobile/P3_21_ANDROID_RUNTIME_CONFIGURATION_BOUNDARY.md",
    "docs/security/P3_21_CLIENT_SECRET_AND_SECURE_STORAGE_BOUNDARY.md",
    "docs/qa/P3_21_RUNTIME_CONFIGURATION_TEST_MATRIX.md",
    "docs/runbooks/P3_21_RUNTIME_CONFIGURATION_RUNBOOK.md",
    "docs/operations/templates/P3_21_RUNTIME_CONFIGURATION_TEMPLATE.md",
    "scripts/verify-p3-21-client-runtime-config-environment-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Client Runtime Configuration and Environment Boundary",
    "Web runtime configuration",
    "iOS runtime configuration",
    "Android runtime configuration",
    "Client secret and secure storage",
    "Runtime configuration test matrix",
    "environment name",
    "API base URL",
    "API contract version",
    "OpenAPI artifact reference",
    "feature flag boundary",
    "telemetry toggle boundary",
    "offline mode toggle boundary",
    "sync mode toggle boundary",
    "request timeout policy",
    "retry policy",
    "secure storage boundary",
    "secret injection boundary",
    "build profile",
    "release channel",
    "request id",
    "correlation id",
    "organization id",
    "standard error envelope",
    "audit trail reference",
    "device id",
    "idempotency key",
    "client operation id",
    "contract test evidence",
    "configuration test evidence",
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
    "runtime configuration is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.21 client runtime configuration and environment boundary verifier passed from repo root: $RepoRoot"
