# Final Backend Verification Checklist

Este documento define la verificación final del backend local del MVP de Cáritas Brigadas de Salud.

## 1. Objetivo

Confirmar que el backend quedó en un estado local estable, demostrable y listo para handoff técnico.

Este checklist valida:

- Build.
- Tests.
- Migraciones.
- API local.
- Health.
- Swagger.
- Smoke test.
- Autorización.
- Reportes.
- Export CSV.
- Auditoría.
- Git limpio.
- Push a `origin/develop`.

## 2. Ubicación correcta

Todos los comandos de backend deben ejecutarse desde:

```powershell
C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet

La solución está en:

services/api-dotnet/Caritas.Brigadas.sln

No correr dotnet test Caritas.Brigadas.sln desde la raíz del repositorio.

3. Verificar LocalDB
sqllocaldb start MSSQLLocalDB
sqllocaldb info MSSQLLocalDB

Resultado esperado:

State: Running
4. Variables locales requeridas
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"
$env:Authentication__Mode = "Development"
$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"
5. Aplicar migraciones
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet

dotnet tool restore

dotnet tool run dotnet-ef database update `
  --context CaritasDbContext `
  --project src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj `
  --startup-project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --connection "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

Debe aplicar, como mínimo, la migración formal de auditoría:

AddAuditLogs
6. Build
dotnet build Caritas.Brigadas.sln

Resultado esperado:

Compilación correcto

Warnings aceptables:

Warnings de xUnit analyzer sobre CancellationToken.
Warnings menores no bloqueantes.

Errores no aceptables:

Error de compilación.
DLL bloqueada por API corriendo.
Referencias faltantes.
Tests project sin paquete.
Migraciones inconsistentes.
7. Tests
dotnet test Caritas.Brigadas.sln

Resultado esperado:

Total tests: todos correctos

Debe pasar:

Domain tests.
Application tests.
Infrastructure tests.
Api tests.
Auth tests.
Organization access tests.
Audit tests.
Integration auth tests.
8. Correr API local
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"
$env:Authentication__Mode = "Development"
$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

dotnet run `
  --project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --no-launch-profile `
  --urls "https://localhost:7044;http://localhost:5031"

URLs esperadas:

https://localhost:7044
http://localhost:5031
9. Health check

En otra terminal:

$baseUrl = "https://localhost:7044"

curl.exe -k -sS "$baseUrl/api/v1/health"

Resultado esperado:

{
  "success": true
}
10. Swagger

Abrir:

https://localhost:7044/swagger

Resultado esperado:

Swagger carga.
Endpoints aparecen bajo /api/v1.
Endpoints protegidos muestran autorización en comportamiento real, aunque Swagger sea visible en Development.
11. Smoke test

Desde raíz del repositorio:

cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud

powershell -ExecutionPolicy Bypass -File .\services\api-dotnet\scripts\smoke-test-local.ps1

Resultado esperado:

SMOKE TEST COMPLETED SUCCESSFULLY

Debe validar:

Health.
Organizations.
Users.
Security.
Services.
Communities.
Mobile units.
Brigades.
Brigade services.
Patients.
Patient visits.
Service encounters.
Form responses.
Consent documents.
Reports JSON.
Reports CSV.
Sync batches.
Audit logs.
Persistencia básica de auditoría.
12. Validación de autorización
Sin headers dev

Un endpoint protegido debe responder:

401 Unauthorized

Ejemplo:

$baseUrl = "https://localhost:7044"
$organizationId = "4df92032-4a1c-4cf2-b48f-15b570cd073a"

curl.exe -k -i "$baseUrl/api/v1/organizations/$organizationId/reports/summary"
Con headers dev válidos

Debe responder success:true o un 200/201 válido:

$userId = "76279895-817d-47d2-b5c2-2a1e306db4f9"

curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN" `
  -H "X-Dev-Name: Smoke Test User" `
  -H "X-Dev-Email: smoke.test@caritas.local"
13. Validación de reportes
JSON
curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN"

Debe devolver:

success: true
CSV
curl.exe -k -L -o ".\report-summary.csv" "$baseUrl/api/v1/organizations/$organizationId/reports/summary.csv" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN"

Get-Content ".\report-summary.csv"
Remove-Item ".\report-summary.csv" -Force -ErrorAction SilentlyContinue

Debe incluir:

metric,value
organizationId,...
generatedAtUtc,...
usersCount,...
14. Validación de auditoría
curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/audit-logs" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN"

Resultado esperado:

success: true
data: arreglo con eventos de auditoría

Acciones esperadas después de smoke test:

reports.summary.read
reports.summary.export
patients.create
patient-visits.create
service-encounters.create
form-responses.create
consent-documents.create
sync-batches.create
15. Validación de Git

Desde raíz:

cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud

git status

Resultado esperado:

nothing to commit, working tree clean

Validar rama:

git branch --show-current

Resultado esperado:

develop

Validar remoto:

git status

Resultado esperado:

Your branch is up to date with 'origin/develop'.
16. Problemas comunes
DLL bloqueada

Síntoma:

The process cannot access the file because it is being used by another process.

Solución:

Get-Process -Name "Caritas.Brigadas.Api" -ErrorAction SilentlyContinue | Stop-Process -Force
Solución no encontrada

Síntoma:

MSBUILD : error MSB1009: El archivo de proyecto no existe.
Modificador: Caritas.Brigadas.sln

Causa:

Estás en la raíz del repo, no en services/api-dotnet.

Solución:

cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet
401 en smoke test

Causa probable:

API no corre en Development.
Authentication__Mode no es Development.
Smoke test no está mandando headers dev.

Solución:

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"
$env:Authentication__Mode = "Development"
500 en audit logs

Causa probable:

Migración AddAuditLogs no aplicada.
API corriendo contra otra base.
Repositorio de lectura no usa tabla formal AuditLogs.

Solución:

dotnet tool run dotnet-ef database update `
  --context CaritasDbContext `
  --project src/Caritas.Brigadas.Infrastructure/Caritas.Brigadas.Infrastructure.csproj `
  --startup-project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --connection "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"
17. Criterio final de cierre

El backend puede considerarse cerrado localmente si:

Build pasa.
Tests pasan.
API corre.
Health responde.
Swagger carga.
Smoke test pasa.
Reportes funcionan.
CSV funciona.
Audit logs responde.
Audit logs contiene eventos.
Auth responde 401/403/200 según caso.
Git está limpio.
Todo está en origin/develop.
18. Siguiente fase después del cierre

Después del cierre local del backend, el siguiente paso recomendado es:

Frontend mínimo de demostración

Prioridad sugerida:

Pantalla de login/dev context.
Dashboard resumen.
Flujo de brigada.
Registro de paciente.
Registro de visita.
Atención por servicio.
Form response.
Consentimiento.
Reporte.
Auditoría básica.
