# ============================================================
# Cáritas Brigadas de Salud
# Setup local appsettings
# ============================================================

Set-Location "C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet"

$examplePath = "src/Caritas.Brigadas.Api/appsettings.Local.example.json"
$localPath = "src/Caritas.Brigadas.Api/appsettings.Local.json"

if (-not (Test-Path $examplePath)) {
    Write-Host "ERROR: No existe appsettings.Local.example.json" -ForegroundColor Red
    exit 1
}

if (Test-Path $localPath) {
    Write-Host "appsettings.Local.json ya existe. No se sobrescribió." -ForegroundColor Yellow
    exit 0
}

Copy-Item $examplePath $localPath

Write-Host "appsettings.Local.json creado correctamente." -ForegroundColor Green
Write-Host "Revisa la cadena de conexión antes de aplicar migraciones." -ForegroundColor Cyan
