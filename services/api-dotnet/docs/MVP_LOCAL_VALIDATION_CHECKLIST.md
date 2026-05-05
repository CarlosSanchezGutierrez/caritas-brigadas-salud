# MVP Local Validation Checklist

Este documento define la validación local mínima para confirmar que el MVP de Cáritas Brigadas de Salud funciona correctamente en ambiente de desarrollo.

## 1. Requisitos locales

- .NET SDK compatible con el proyecto.
- SQL Server LocalDB disponible.
- Certificado HTTPS de desarrollo confiable.
- Repositorio actualizado en `develop`.
- Base de datos local `CaritasBrigadas_Local`.

## 2. Ubicación correcta del backend

Todos los comandos de build, test, EF y ejecución de API deben correr desde:

```powershell
C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet
```

La solución está en:

```powershell
services/api-dotnet/Caritas.Brigadas.sln
```

No ejecutar:

```powershell
dotnet test Caritas.Brigadas.sln
```

desde la raíz del repositorio, porque ahí no existe la solución.

## 3. Validar LocalDB

```powershell
sqllocaldb info
sqllocaldb start MSSQLLocalDB
sqllocaldb info MSSQLLocalDB
```

Estado esperado:

```text
State: Running
```

## 4. Connection string local recomendada

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"
```

## 5. Aplicar migraciones

```powershell
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet

dotnet tool restore

dotnet tool run dotnet-ef database update `
  --context CaritasDbContext `
  --project src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj `
  --startup-project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --connection "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"
```

## 6. Build y tests

```powershell
dotnet build Caritas.Brigadas.sln
dotnet test Caritas.Brigadas.sln
```

Resultado esperado:

```text
Compilación correcta
Total tests: todos correctos
```

## 7. Correr API local

```powershell
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

dotnet run `
  --project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --no-launch-profile `
  --urls "https://localhost:7044;http://localhost:5031"
```

URLs esperadas:

```text
https://localhost:7044
http://localhost:5031
```

Para pruebas locales se recomienda usar HTTPS:

```text
https://localhost:7044
```

## 8. Validar Health

```powershell
$baseUrl = "https://localhost:7044"

curl.exe -k -sS "$baseUrl/api/v1/health"
```

Resultado esperado:

```json
{
  "success": true
}
```

## 9. Validar Swagger

Abrir en navegador:

```text
https://localhost:7044/swagger
```

Debe cargar documentación OpenAPI/Swagger en ambiente Development.

## 10. Smoke test local completo

Con la API corriendo en una terminal, ejecutar en otra:

```powershell
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud

powershell -ExecutionPolicy Bypass -File .\services\api-dotnet\scripts\smoke-test-local.ps1
```

Resultado esperado al final:

```text
SMOKE TEST COMPLETED SUCCESSFULLY
```

El smoke test debe validar:

- Health.
- Organizations.
- Users.
- Services.
- Form templates.
- Communities.
- Mobile units.
- Brigades.
- Brigade service assignments.
- Patients.
- Patient visits.
- Service encounters.
- Form responses.
- Consent documents.
- Reports summary.
- Reports summary CSV export.
- Sync batches.
- Audit logs.

## 11. Validar reporte resumen JSON

```powershell
$baseUrl = "https://localhost:7044"
$organizationId = "4df92032-4a1c-4cf2-b48f-15b570cd073a"

curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary"
```

Debe devolver:

```json
{
  "success": true,
  "data": {
    "usersCount": 1,
    "servicesCount": 8,
    "brigadesCount": 1,
    "patientsCount": 1,
    "patientVisitsCount": 1,
    "serviceEncountersCount": 1,
    "formTemplatesCount": 7,
    "formResponsesCount": 1,
    "consentDocumentsCount": 1
  }
}
```

Los conteos pueden ser mayores si ya se corrió el smoke test varias veces.

## 12. Validar export CSV

```powershell
curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary.csv"
```

Debe devolver contenido CSV:

```csv
metric,value
organizationId,...
generatedAtUtc,...
usersCount,...
servicesCount,...
clinicalRecordsCount,...
```

Descarga real:

```powershell
curl.exe -k -L -o ".\report-summary.csv" "$baseUrl/api/v1/organizations/$organizationId/reports/summary.csv"
Get-Content ".\report-summary.csv"
Remove-Item ".\report-summary.csv" -Force -ErrorAction SilentlyContinue
```

## 13. Validar Git limpio

Después de pruebas locales:

```powershell
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud
git status
```

Resultado esperado:

```text
nothing to commit, working tree clean
```

## 14. Criterio de salida del MVP local

El backend puede considerarse validado localmente cuando:

- `dotnet build` pasa.
- `dotnet test` pasa.
- La API corre en HTTPS.
- `/health` responde `success:true`.
- Swagger carga.
- El smoke test termina correctamente.
- El reporte JSON responde correctamente.
- El CSV se descarga correctamente.
- No hay archivos temporales sin limpiar.
- `git status` queda limpio.

## 15. Nota de alcance

Este checklist valida el MVP local técnico. No sustituye:

- Validación con socio formador.
- Pruebas de seguridad.
- Pruebas de carga.
- Revisión legal de aviso de privacidad.
- Validación UX con médicos, psicólogos, nutriólogos, optometristas o estudiantes de servicio.
- Validación de operación offline real en campo.
