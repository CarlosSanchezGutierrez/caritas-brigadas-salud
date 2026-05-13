$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$MatrixPath = Join-Path $RepoRoot "docs/backend/P3_OPERATIONAL_ROLES_PANELS_ANALYTICS_ACCESS_MATRIX.md"

function Assert-FileExists {
    param([string]$Path)

    if (-not (Test-Path $Path)) {
        throw "Required file not found: $Path"
    }
}

function Assert-Contains {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if (-not $Content.Contains($Token)) {
        throw "$Label does not contain required token: $Token"
    }
}

Assert-FileExists $MatrixPath

$Matrix = Get-Content $MatrixPath -Raw -Encoding UTF8

$RequiredTokens = @(
    "P3 Operational Roles, Panels, and Analytics Access Matrix",
    "Role groups",
    "Web panels",
    "SuperAdmin access",
    "OrganizationAdmin access",
    "MedicalUser access",
    "OfficeCapturer access",
    "DataAnalyst access",
    "DataEngineer access",
    "DataScientist access",
    "Developer access",
    "SystemActor access",
    "Data pipeline expectations",
    "Required analytical dimensions",
    "patient sex",
    "minor/adult status",
    "age or age band",
    "vital signs values with canonical units",
    "Admin reporting expectations",
    "Medical reporting expectations",
    "Office capture workflow expectations",
    "Programmer support material",
    "Access matrix",
    "| Capability | SuperAdmin | OrganizationAdmin | MedicalUser | OfficeCapturer | AuditReviewer | DataAnalyst | DataEngineer | DataScientist | Developer |",
    "Review tenant audit logs",
    "Review global audit logs",
    "Review data export evidence",
    "Audit evidence only",
    "Capture vital signs",
    "Export identified patient data",
    "Access raw production DB",
    "Acceptance criteria",
    "P3-05 can implement VitalSignsRecord"
)

foreach ($Token in $RequiredTokens) {
    Assert-Contains $Matrix $Token "P3 operational roles panels analytics access matrix"
}

Write-Host "P3 operational roles panels analytics access matrix verification passed." -ForegroundColor Green