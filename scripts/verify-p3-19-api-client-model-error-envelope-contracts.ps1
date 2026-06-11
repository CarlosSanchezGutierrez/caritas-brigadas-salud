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
    "docs/api/P3_19_SHARED_API_CLIENT_MODEL_CONTRACTS.md",
    "docs/api/P3_19_REQUEST_RESPONSE_METADATA_MODELS.md",
    "docs/api/P3_19_STANDARD_ERROR_ENVELOPE_MODEL.md",
    "docs/api/P3_19_OFFLINE_SYNC_METADATA_MODELS.md",
    "docs/api/P3_19_AUDIT_AND_CONFLICT_CLIENT_MODELS.md",
    "docs/web/P3_19_WEB_MODEL_MAPPING.md",
    "docs/mobile/P3_19_MOBILE_MODEL_MAPPING.md",
    "docs/qa/P3_19_MODEL_CONTRACT_TEST_MATRIX.md",
    "docs/runbooks/P3_19_MODEL_CONTRACTS_RUNBOOK.md",
    "docs/operations/templates/P3_19_MODEL_CONTRACTS_TEMPLATE.md",
    "scripts/verify-p3-19-api-client-model-error-envelope-contracts.ps1"
)

foreach ($File in $RequiredFiles) { Assert-FileExists -RelativePath $File }

$DocumentationFiles = $RequiredFiles | Where-Object { $_ -like "docs/*" }
$AllDocumentationContent = ""
foreach ($File in $DocumentationFiles) { $AllDocumentationContent += "`n--- FILE: $File ---`n"; $AllDocumentationContent += Read-RepoText -RelativePath $File }

$RequiredTokens = @(
    "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE",
    "SQL Server is the operational source of truth",
    "Shared API Client Model Contracts",
    "request metadata model",
    "response metadata model",
    "standard error envelope model",
    "authentication context model",
    "authorization context model",
    "organization scope model",
    "pagination model",
    "filtering model",
    "sorting model",
    "audit reference model",
    "mobile device model",
    "offline operation model",
    "conflict model",
    "API contract version",
    "endpoint id",
    "request id",
    "correlation id",
    "organization id",
    "authorization role",
    "standard error envelope",
    "audit trail reference",
    "device id",
    "idempotency key",
    "client operation id",
    "sync status",
    "server acknowledgment",
    "conflict id",
    "model contract test evidence",
    "schema drift status",
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
    "shared API client models are production ready"
)

foreach ($Token in $ForbiddenTokens) { Assert-DoesNotContainToken -Content $AllDocumentationContent -Token $Token }

Write-Host "P3.19 API client model and error envelope contracts verifier passed from repo root: $RepoRoot"
