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

function Assert-DoesNotContain {
    param(
        [string]$Content,
        [string]$Token,
        [string]$Label
    )

    if ($Content.Contains($Token)) {
        throw "$Label contains forbidden token: $Token"
    }
}

$RequestTelemetryPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Middleware/RequestTelemetryMiddleware.cs"
$HttpAuditLoggerPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Api/Audit/HttpAuditLogger.cs"
$DbContextPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/CaritasDbContext.cs"
$AuditLogConfigurationPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Configurations/AuditLogConfiguration.cs"
$GovernancePath = Join-Path $RepoRoot "scripts/validate-repo-governance-baseline.ps1"

Assert-FileExists $RequestTelemetryPath "RequestTelemetryMiddleware"
Assert-FileExists $HttpAuditLoggerPath "HttpAuditLogger"
Assert-FileExists $DbContextPath "CaritasDbContext"
Assert-FileExists $AuditLogConfigurationPath "AuditLogConfiguration"
Assert-FileExists $GovernancePath "repository governance baseline"

$RequestTelemetry = Get-Content $RequestTelemetryPath -Raw -Encoding UTF8
$HttpAuditLogger = Get-Content $HttpAuditLoggerPath -Raw -Encoding UTF8
$DbContext = Get-Content $DbContextPath -Raw -Encoding UTF8
$AuditLogConfiguration = Get-Content $AuditLogConfigurationPath -Raw -Encoding UTF8
$Governance = Get-Content $GovernancePath -Raw -Encoding UTF8

Assert-Contains $RequestTelemetry "GetSafeHttpMethodForLog(context.Request.Method)" "RequestTelemetryMiddleware"
Assert-Contains $RequestTelemetry "private static string GetSafeHttpMethodForLog(string? method)" "RequestTelemetryMiddleware"
Assert-Contains $RequestTelemetry "private static string SanitizeForLog(string? value)" "RequestTelemetryMiddleware"
Assert-Contains $RequestTelemetry "normalizedMethod is `"GET`"" "RequestTelemetryMiddleware"
Assert-Contains $RequestTelemetry "char.IsControl" "RequestTelemetryMiddleware"
Assert-Contains $RequestTelemetry "return SanitizeForLog(rawPath);" "RequestTelemetryMiddleware"
Assert-DoesNotContain $RequestTelemetry "var httpMethod = context.Request.Method;" "RequestTelemetryMiddleware"

Assert-Contains $HttpAuditLogger "using Caritas.Brigadas.Api.Extensions;" "HttpAuditLogger"
Assert-Contains $HttpAuditLogger "CorrelationId = httpContext?.GetCorrelationId()," "HttpAuditLogger"
Assert-DoesNotContain $HttpAuditLogger "CorrelationId = httpContext?.TraceIdentifier," "HttpAuditLogger"

Assert-Contains $DbContext "using Caritas.Brigadas.Infrastructure.Persistence.Configurations;" "CaritasDbContext"
Assert-Contains $DbContext "modelBuilder.ApplyConfiguration(new AuditLogConfiguration());" "CaritasDbContext"

Assert-Contains $AuditLogConfiguration "HasMaxLength(128)" "AuditLogConfiguration"
Assert-Contains $AuditLogConfiguration "auditLog.OrganizationId" "AuditLogConfiguration"
Assert-Contains $AuditLogConfiguration "auditLog.OccurredAtUtc" "AuditLogConfiguration"

Assert-Contains $Governance "verify-p3-pre-main-security-review-findings.ps1" "repository governance baseline"

Write-Host "P3 pre-main security review findings verification passed." -ForegroundColor Green
$MigrationRoot = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Migrations"

$OriginalAuditMigration = Get-ChildItem -Path $MigrationRoot -Filter "20260515055019_ApplyAuditLogConfiguration.cs" -ErrorAction SilentlyContinue
$WidenAuditMigration = Get-ChildItem -Path $MigrationRoot -Filter "*WidenAuditLogCorrelationIdTo128.cs" -ErrorAction SilentlyContinue

if (-not $OriginalAuditMigration) {
    throw "Original ApplyAuditLogConfiguration migration must be preserved."
}

if (-not $WidenAuditMigration) {
    throw "WidenAuditLogCorrelationIdTo128 migration is required."
}
