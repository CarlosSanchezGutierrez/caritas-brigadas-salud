$ErrorActionPreference = "Stop"

$RepoRoot = Split-Path -Parent $PSScriptRoot
$SyncEventPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Domain/Entities/SyncEvent.cs"
$DbContextPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/CaritasDbContext.cs"
$SqlBaselinePath = Join-Path $RepoRoot "database/migrations/sqlserver/0001_initial_create.sql"
$MigrationsPath = Join-Path $RepoRoot "services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Migrations"

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

Assert-FileExists $SyncEventPath
Assert-FileExists $DbContextPath
Assert-FileExists $SqlBaselinePath

$SyncEvent = Get-Content $SyncEventPath -Raw -Encoding UTF8
$DbContext = Get-Content $DbContextPath -Raw -Encoding UTF8
$SqlBaseline = Get-Content $SqlBaselinePath -Raw -Encoding UTF8

Assert-Contains $SyncEvent "IdempotencyKey" "SyncEvent"
Assert-Contains $SyncEvent "MaxIdempotencyKeyLength = 250" "SyncEvent"
Assert-Contains $SyncEvent "NormalizeIdempotencyKey" "SyncEvent"

Assert-Contains $DbContext "entity.Property(x => x.IdempotencyKey).HasMaxLength(250).IsRequired()" "CaritasDbContext"
Assert-Contains $DbContext "entity.HasIndex(x => new { x.OrganizationId, x.IdempotencyKey }).IsUnique()" "CaritasDbContext"

Assert-Contains $SqlBaseline "[IdempotencyKey] nvarchar(250) NOT NULL" "SQL Server baseline"
Assert-Contains $SqlBaseline "IX_sync_events_OrganizationId_IdempotencyKey" "SQL Server baseline"
Assert-Contains $SqlBaseline "__EFMigrationsHistory" "SQL Server baseline"
Assert-Contains $SqlBaseline "AddSyncEventIdempotencyKey" "SQL Server baseline"

$MigrationFiles = Get-ChildItem $MigrationsPath -File -Filter "*_AddSyncEventIdempotencyKey.cs"

if ($MigrationFiles.Count -eq 0) {
    throw "No AddSyncEventIdempotencyKey migration was found."
}

$MigrationContent = ($MigrationFiles | ForEach-Object { Get-Content $_.FullName -Raw -Encoding UTF8 }) -join "`n"

Assert-Contains $MigrationContent "IdempotencyKey" "AddSyncEventIdempotencyKey migration"
Assert-Contains $MigrationContent "IX_sync_events_OrganizationId_IdempotencyKey" "AddSyncEventIdempotencyKey migration"
Assert-Contains $MigrationContent "defaultValueSql" "AddSyncEventIdempotencyKey migration"

Write-Host "P3 sync idempotency guardrails verification passed." -ForegroundColor Green
