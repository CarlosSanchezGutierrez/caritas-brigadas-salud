$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

function Assert-FileExists {
    param([string]$Path, [string]$Label)

    if (-not (Test-Path $Path)) {
        throw "$Label file not found: $Path"
    }
}

function Assert-Contains {
    param([string]$Content, [string]$Token, [string]$Label)

    if (-not $Content.Contains($Token)) {
        throw "$Label does not contain required token: $Token"
    }
}

function Assert-NotContains {
    param([string]$Content, [string]$Token, [string]$Label)

    if ($Content.Contains($Token)) {
        throw "$Label contains forbidden token: $Token"
    }
}

$ArchitecturePath = Join-Path $RepoRoot "docs/architecture/P3_7_ON_PREM_BACKEND_CLOSURE_ARCHITECTURE.md"
$DecisionRegisterPath = Join-Path $RepoRoot "docs/architecture/P3_7_ON_PREM_BACKEND_CLOSURE_DECISION_REGISTER.md"
$GapMapPath = Join-Path $RepoRoot "docs/backend/P3_7_BACKEND_FREEZE_GAP_MAP.md"
$PipelinesPath = Join-Path $RepoRoot "docs/data/P3_7_OPERATIONAL_ANALYTICAL_PIPELINES_BASELINE.md"
$SecurityMapPath = Join-Path $RepoRoot "docs/security/P3_7_ON_PREM_SECURITY_THREAT_MAP_BASELINE.md"

Assert-FileExists $ArchitecturePath "P3.7 architecture"
Assert-FileExists $DecisionRegisterPath "P3.7 decision register"
Assert-FileExists $GapMapPath "P3.7 backend freeze gap map"
Assert-FileExists $PipelinesPath "P3.7 pipelines baseline"
Assert-FileExists $SecurityMapPath "P3.7 security map"

$Architecture = Get-Content $ArchitecturePath -Raw -Encoding UTF8
$DecisionRegister = Get-Content $DecisionRegisterPath -Raw -Encoding UTF8
$GapMap = Get-Content $GapMapPath -Raw -Encoding UTF8
$Pipelines = Get-Content $PipelinesPath -Raw -Encoding UTF8
$SecurityMap = Get-Content $SecurityMapPath -Raw -Encoding UTF8

Assert-Contains $Architecture "Backend production readiness: BLOCKED_PENDING_REAL_EVIDENCE" "P3.7 architecture"
Assert-Contains $Architecture "Backend freeze status: NOT_FROZEN_PENDING_ON_PREM_EVIDENCE_AND_CONTRACTS" "P3.7 architecture"
Assert-Contains $Architecture "SQL Server is the operational source of truth" "P3.7 architecture"
Assert-Contains $Architecture "The production backend must not require cloud infrastructure to operate." "P3.7 architecture"
Assert-Contains $Architecture "Data injection" "P3.7 architecture"
Assert-Contains $Architecture "Total auditability" "P3.7 architecture"
Assert-Contains $Architecture "Longitudinal history" "P3.7 architecture"
Assert-Contains $Architecture "Offline-first sync" "P3.7 architecture"
Assert-Contains $Architecture "Operational pipeline" "P3.7 architecture"
Assert-Contains $Architecture "Analytical pipeline" "P3.7 architecture"
Assert-Contains $Architecture "KPIs, insights, dashboards, and monitoring" "P3.7 architecture"
Assert-Contains $Architecture "Vulnerability and social needs map" "P3.7 architecture"
Assert-Contains $Architecture "Advanced clinical statistics" "P3.7 architecture"
Assert-Contains $Architecture "AI API Gateway is deferred" "P3.7 architecture"
Assert-Contains $Architecture "Blockchain is deferred" "P3.7 architecture"
Assert-Contains $Architecture "Frontend clients may move fast only after" "P3.7 architecture"

Assert-Contains $DecisionRegister "P3.7-ADR-001" "P3.7 decision register"
Assert-Contains $DecisionRegister "Cloud infrastructure is optional, not required" "P3.7 decision register"
Assert-Contains $DecisionRegister "Data injection must pass through validation and audit" "P3.7 decision register"
Assert-Contains $DecisionRegister "AI API Gateway is deferred behind an adapter boundary" "P3.7 decision register"
Assert-Contains $DecisionRegister "Blockchain is deferred as crypto-audit lab readiness" "P3.7 decision register"

Assert-Contains $GapMap "P3.7-GATE-001" "P3.7 gap map"
Assert-Contains $GapMap "P3.7-GATE-016" "P3.7 gap map"
Assert-Contains $GapMap "Backend v1 cannot be frozen" "P3.7 gap map"

Assert-Contains $Pipelines "Operational pipeline" "P3.7 pipelines"
Assert-Contains $Pipelines "Analytical pipeline" "P3.7 pipelines"
Assert-Contains $Pipelines "Direction" "P3.7 pipelines"
Assert-Contains $Pipelines "Clinical monitoring" "P3.7 pipelines"
Assert-Contains $Pipelines "Social vulnerability" "P3.7 pipelines"
Assert-Contains $Pipelines "Research lab readiness" "P3.7 pipelines"

Assert-Contains $SecurityMap "On-prem threat model scope" "P3.7 security map"
Assert-Contains $SecurityMap "Social vulnerability map governance" "P3.7 security map"
Assert-Contains $SecurityMap "AI API Gateway risk boundary" "P3.7 security map"
Assert-Contains $SecurityMap "Crypto-audit risk boundary" "P3.7 security map"

Assert-NotContains $Architecture "Azure is required" "P3.7 architecture"
Assert-NotContains $Architecture "AWS is required" "P3.7 architecture"
Assert-NotContains $Architecture "Cloud is required" "P3.7 architecture"

Write-Host "P3.7 on-prem backend closure architecture verification passed." -ForegroundColor Green
