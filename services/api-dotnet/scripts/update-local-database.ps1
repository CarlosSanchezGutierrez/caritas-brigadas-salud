# ============================================================
# Cáritas Brigadas de Salud
# Apply EF Core migrations to local SQL Server
# ============================================================

Set-Location "C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet"

$localSettings = "src/Caritas.Brigadas.Api/appsettings.Local.json"

if (-not (Test-Path $localSettings)) {
    Write-Host "ERROR: No existe appsettings.Local.json." -ForegroundColor Red
    Write-Host "Primero ejecuta:" -ForegroundColor Yellow
    Write-Host ".\scripts\setup-local-appsettings.ps1" -ForegroundColor Yellow
    exit 1
}

dotnet tool restore

dotnet tool run dotnet-ef database update `
    --context CaritasDbContext `
    --project src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj `
    --startup-project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: No se pudieron aplicar las migraciones." -ForegroundColor Red
    exit 1
}

Write-Host "Base de datos local actualizada correctamente." -ForegroundColor Green
