$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot

function Assert-FileExists {
    param(
        [string]$Path,
        [string]$Label
    )

    if (-not (Test-Path $Path)) {
        throw "$Label file not found: $Path"
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

$BaselinePath = Join-Path $RepoRoot "docs/product/P3_OPENAPI_FRONTEND_CONTRACT_FREEZE_BASELINE.md"
$ContractPath = Join-Path $RepoRoot "docs/product/P3_OPENAPI_FRONTEND_CONTRACT_FREEZE.md"
$PatientIntakePath = Join-Path $RepoRoot "docs/product/P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md"
$ConsentPath = Join-Path $RepoRoot "docs/product/P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md"
$EmergencyInsurancePath = Join-Path $RepoRoot "docs/product/P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT.md"
$GapAuditPath = Join-Path $RepoRoot "docs/operations/P3_SECURITY_PRODUCT_READINESS_GAP_AUDIT.md"
$ProgramPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Program.cs"
$SwaggerExtensionsPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Extensions/SwaggerServiceExtensions.cs"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 OpenAPI frontend contract freeze baseline"
Assert-FileExists $ContractPath "P3 OpenAPI frontend contract freeze"
Assert-FileExists $PatientIntakePath "P3 patient intake functional contract"
Assert-FileExists $ConsentPath "P3 consent signature evidence contract"
Assert-FileExists $EmergencyInsurancePath "P3 emergency contact and insurance contract"
Assert-FileExists $GapAuditPath "P3 security and product readiness gap audit"
Assert-FileExists $ProgramPath "Program.cs"
Assert-FileExists $SwaggerExtensionsPath "SwaggerServiceExtensions.cs"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$Contract = Get-Content $ContractPath -Raw -Encoding UTF8
$PatientIntake = Get-Content $PatientIntakePath -Raw -Encoding UTF8
$Consent = Get-Content $ConsentPath -Raw -Encoding UTF8
$EmergencyInsurance = Get-Content $EmergencyInsurancePath -Raw -Encoding UTF8
$GapAudit = Get-Content $GapAuditPath -Raw -Encoding UTF8
$Program = Get-Content $ProgramPath -Raw -Encoding UTF8
$SwaggerExtensions = Get-Content $SwaggerExtensionsPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 OpenAPI Frontend Contract Freeze Baseline",
    "/openapi/v1/openapi.json",
    "/swagger",
    "NEXT_PUBLIC_API_BASE_URL",
    "NEXT_PUBLIC_ENABLE_MOCK_API",
    "NEXT_PUBLIC_ENABLE_OFFLINE_MODE",
    "Authorization",
    "X-Correlation-Id",
    "Content-Type",
    "Accept",
    "Client applications must never connect directly to SQL Server.",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 OpenAPI frontend contract freeze baseline"
}

$RequiredContractTokens = @(
    "P3 OpenAPI Frontend Contract Freeze",
    "FRONTEND_MVP_SCAFFOLD_READY",
    "UNBLOCKS_FRONTEND_MVP_SCAFFOLD",
    "/openapi/v1/openapi.json",
    "/swagger",
    "NEXT_PUBLIC_API_BASE_URL",
    "NEXT_PUBLIC_API_TIMEOUT_MS",
    "NEXT_PUBLIC_ENABLE_MOCK_API",
    "NEXT_PUBLIC_ENABLE_OFFLINE_MODE",
    "NEXT_PUBLIC_APP_ENVIRONMENT",
    "Authorization",
    "X-Correlation-Id",
    "Response envelope rules",
    "Error envelope rules",
    "Contract areas frozen for frontend MVP",
    "P3_PATIENT_INTAKE_FUNCTIONAL_CONTRACT.md",
    "P3_CONSENT_SIGNATURE_EVIDENCE_CONTRACT.md",
    "P3_EMERGENCY_CONTACT_INSURANCE_FIELDS_CONTRACT.md",
    "patient_identity_label_missing",
    "consent_signature_missing",
    "emergency_contact_relationship_missing",
    "social_security_provider_other_missing",
    "Never log",
    "PayloadJson",
    "Mock API mode is not allowed as",
    "Contract change control"
)

foreach ($Token in $RequiredContractTokens) {
    Assert-Contains $Contract $Token "P3 OpenAPI frontend contract freeze"
}

Assert-Contains $Program "AddCaritasSwagger" "Program.cs"
Assert-Contains $Program "UseCaritasSwagger" "Program.cs"
Assert-Contains $SwaggerExtensions "openapi/{documentName}/openapi.json" "SwaggerServiceExtensions.cs"
Assert-Contains $SwaggerExtensions "RoutePrefix = `"swagger`"" "SwaggerServiceExtensions.cs"
Assert-Contains $SwaggerExtensions "Cáritas Brigadas de Salud API v1" "SwaggerServiceExtensions.cs"

Assert-Contains $PatientIntake "P3-30D OpenAPI/frontend contract freeze" "P3 patient intake functional contract"
Assert-Contains $Consent "P3-30D OpenAPI/frontend contract freeze" "P3 consent signature evidence contract"
Assert-Contains $EmergencyInsurance "P3-30D OpenAPI/frontend contract freeze" "P3 emergency contact and insurance contract"
Assert-Contains $GapAudit "P3-30D OpenAPI/frontend contract freeze" "P3 security and product readiness gap audit"
Assert-Contains $Governance "verify-p3-openapi-frontend-contract-freeze.ps1" "repository governance baseline"

Write-Host "P3 OpenAPI frontend contract freeze verification passed." -ForegroundColor Green