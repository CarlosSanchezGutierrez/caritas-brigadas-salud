# ============================================================
# Cáritas Brigadas de Salud
# Generate idempotent SQL migration script
# ============================================================

Set-Location "C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet"

$outputPath = "..\..\database\migrations\sqlserver\latest_idempotent.sql"

New-Item -ItemType Directory -Force -Path "..\..\database\migrations\sqlserver" | Out-Null

dotnet tool restore

dotnet tool run dotnet-ef migrations script `
    --idempotent `
    --context CaritasDbContext `
    --project src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj `
    --startup-project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
    --output $outputPath

if ($LASTEXITCODE -ne 0) {
    Write-Host "ERROR: No se pudo generar el script SQL." -ForegroundColor Red
    exit 1
}

Write-Host "Script SQL generado en: $outputPath" -ForegroundColor Green
