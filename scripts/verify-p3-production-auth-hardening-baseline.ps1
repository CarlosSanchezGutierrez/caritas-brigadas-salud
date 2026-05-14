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

$BaselinePath = Join-Path $RepoRoot "docs/security/P3_PRODUCTION_AUTH_HARDENING_BASELINE.md"
$ProductionReadinessPath = Join-Path $RepoRoot "docs/operations/P3_PRODUCTION_DEPLOYMENT_READINESS_BASELINE.md"
$ProgramPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Program.cs"
$AuthOptionsPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Options/CaritasAuthenticationOptions.cs"
$ConfiguredAuthPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Extensions/ConfiguredAuthenticationServiceExtensions.cs"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $BaselinePath "P3 production auth hardening baseline"
Assert-FileExists $ProductionReadinessPath "P3 production deployment readiness baseline"
Assert-FileExists $ProgramPath "Program.cs"
Assert-FileExists $AuthOptionsPath "CaritasAuthenticationOptions.cs"
Assert-FileExists $ConfiguredAuthPath "ConfiguredAuthenticationServiceExtensions.cs"
Assert-FileExists $GovernancePath "repository governance baseline"

$Baseline = Get-Content $BaselinePath -Raw -Encoding UTF8
$ProductionReadiness = Get-Content $ProductionReadinessPath -Raw -Encoding UTF8
$Program = Get-Content $ProgramPath -Raw -Encoding UTF8
$AuthOptions = Get-Content $AuthOptionsPath -Raw -Encoding UTF8
$ConfiguredAuth = Get-Content $ConfiguredAuthPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

$RequiredBaselineTokens = @(
    "P3 Production Authentication Hardening Baseline",
    "Production authentication must use JWT Bearer authentication.",
    "Authentication:Mode = Development",
    "Authentication:Mode = Disabled",
    "X-Dev-User-Id",
    "X-Dev-Organization-Id",
    "X-Dev-Roles",
    "X-Dev-Permissions",
    "ValidateProductionConfiguration",
    "AddConfiguredAuthentication",
    "UseAuthentication",
    "UseAuthorization",
    "Authentication:Mode = JwtBearer",
    "Authentication:Authority",
    "Authentication:Audience or Authentication:ValidAudiences",
    "Development authentication mode is only allowed in Development environment.",
    "Disabled authentication mode is not allowed outside Development environment.",
    "P3-26B is complete",
    "Acceptance criteria"
)

foreach ($Token in $RequiredBaselineTokens) {
    Assert-Contains $Baseline $Token "P3 production auth hardening baseline"
}

$RequiredProductionReadinessTokens = @(
    "P3-26B authentication and authorization hardening",
    "no local development headers in production authentication flows",
    "no development authentication mode in production",
    "Production go-live status: blocked."
)

foreach ($Token in $RequiredProductionReadinessTokens) {
    Assert-Contains $ProductionReadiness $Token "P3 production deployment readiness baseline"
}

$RequiredProgramTokens = @(
    "builder.ValidateProductionConfiguration();",
    "builder.Services.AddCaritasAuthenticationOptions(builder.Configuration);",
    "builder.Services.AddConfiguredAuthentication(builder.Configuration, builder.Environment);",
    "builder.Services.AddPermissionAuthorization();",
    "builder.Services.AddOrganizationAccessEnforcement();",
    "app.UseAuthentication();",
    "app.UseAuthorization();"
)

foreach ($Token in $RequiredProgramTokens) {
    Assert-Contains $Program $Token "Program.cs auth pipeline"
}

$ValidateIndex = $Program.IndexOf("builder.ValidateProductionConfiguration();", [System.StringComparison]::Ordinal)
$ConfiguredAuthIndex = $Program.IndexOf("builder.Services.AddConfiguredAuthentication(builder.Configuration, builder.Environment);", [System.StringComparison]::Ordinal)

if ($ValidateIndex -lt 0 -or $ConfiguredAuthIndex -lt 0 -or $ValidateIndex -gt $ConfiguredAuthIndex) {
    throw "Program.cs must validate production configuration before configured authentication registration."
}

$RequiredAuthOptionsTokens = @(
    "CaritasAuthenticationOptions",
    "public const string SectionName = ""Authentication"";",
    "ValidateForEnvironment",
    "Development authentication mode is only allowed in Development environment.",
    "Disabled authentication mode is not allowed outside Development environment.",
    "JWT Bearer authentication requires Authentication:Authority.",
    "JWT Bearer authentication requires Authentication:Audience or Authentication:ValidAudiences."
)

foreach ($Token in $RequiredAuthOptionsTokens) {
    Assert-Contains $AuthOptions $Token "CaritasAuthenticationOptions"
}

$RequiredConfiguredAuthTokens = @(
    "AddConfiguredAuthentication",
    "options.ValidateForEnvironment(environment.EnvironmentName)",
    "services.AddDevelopmentAuthentication(environment);",
    "services.AddDisabledAuthentication(environment);",
    "JwtBearerDefaults.AuthenticationScheme",
    "ValidateIssuer = true",
    "ValidateAudience = true",
    "ValidateLifetime = true",
    "ValidateIssuerSigningKey = true",
    "RoleClaimType = CurrentUserClaimTypes.RoleCode"
)

foreach ($Token in $RequiredConfiguredAuthTokens) {
    Assert-Contains $ConfiguredAuth $Token "configured authentication"
}

Assert-Contains $Governance "verify-p3-production-auth-hardening-baseline.ps1" "repository governance baseline"

Write-Host "P3 production auth hardening baseline verification passed." -ForegroundColor Green