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
    "docs/pilot/P3_25_CONTROLLED_PILOT_READINESS_BOUNDARY.md",
    "docs/web/P3_25_WEB_PILOT_READINESS_BOUNDARY.md",
    "docs/mobile/P3_25_IOS_PILOT_READINESS_BOUNDARY.md",
    "docs/mobile/P3_25_ANDROID_PILOT_READINESS_BOUNDARY.md",
    "docs/operations/P3_25_FIELD_OPERATIONS_SUPPORT_AND_TRAINING_BOUNDARY.md",
    "docs/security/P3_25_PILOT_PRIVACY_CONSENT_DATA_PROTECTION_BOUNDARY.md",
    "docs/qa/P3_25_PILOT_ACCEPTANCE_UAT_MATRIX.md",
    "docs/runbooks/P3_25_CONTROLLED_PILOT_READINESS_RUNBOOK.md",
    "docs/operations/templates/P3_25_CONTROLLED_PILOT_READINESS_TEMPLATE.md",
    "scripts/verify-p3-25-controlled-pilot-readiness-boundary.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Controlled Pilot Readiness Boundary",
    "Web pilot readiness boundary",
    "iOS pilot readiness boundary",
    "Android pilot readiness boundary",
    "Field operations support and training boundary",
    "Pilot privacy consent data protection boundary",
    "Pilot acceptance UAT matrix",
    "approved release candidate reference",
    "artifact reference",
    "deployed commit SHA",
    "environment name",
    "build profile",
    "release channel",
    "API contract version",
    "OpenAPI artifact reference",
    "pilot site or brigade scope",
    "pilot participant scope",
    "pilot device inventory",
    "UAT acceptance criteria",
    "training evidence",
    "privacy consent evidence",
    "data protection evidence",
    "contract test evidence",
    "runtime configuration test evidence",
    "observability evidence",
    "privacy-safe telemetry evidence",
    "offline field workflow evidence",
    "sync dry run evidence",
    "rollback plan",
    "incident response plan",
    "support escalation plan",
    "pilot approval state",
    "request id",
    "correlation id",
    "organization id",
    "audit trail reference",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id",
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
    "pilot approval is production approval",
    "controlled pilot is production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.25 controlled pilot readiness boundary verifier passed from repo root: $RepoRoot"
