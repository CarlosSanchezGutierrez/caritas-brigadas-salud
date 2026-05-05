# Cáritas Brigadas de Salud API - Documentation Index

Este índice centraliza la documentación técnica del backend del MVP local de Cáritas Brigadas de Salud.

## 1. Propósito

El objetivo de esta carpeta es mantener documentación clara, versionada y útil para:

- Desarrollo backend.
- Validación local.
- Revisión técnica.
- Presentación ante socio formador.
- Onboarding de nuevos colaboradores.
- Preparación para frontend web, app móvil y despliegue cloud.
- Evidencia profesional del proyecto en GitHub.

## 2. Documentos principales

### MVP Local Validation Checklist

Archivo:

```text
docs/MVP_LOCAL_VALIDATION_CHECKLIST.md
```

Uso:

- Validar LocalDB.
- Aplicar migraciones.
- Ejecutar build.
- Ejecutar tests.
- Correr API local.
- Validar Swagger.
- Ejecutar smoke test.
- Validar reportes JSON.
- Validar export CSV.
- Confirmar `git status` limpio.

Cuándo usarlo:

```text
Antes de decir que el backend local quedó validado.
```

### Endpoint Inventory

Archivo:

```text
docs/ENDPOINT_INVENTORY.md
```

Uso:

- Consultar endpoints disponibles.
- Revisar rutas.
- Revisar módulos funcionales.
- Entender el orden lógico de pruebas manuales.
- Confirmar alcance actual del MVP.

Incluye:

- Health.
- Organizations.
- Users.
- Security.
- Services.
- Communities.
- Mobile Units.
- Brigades.
- Brigade Services.
- Patients.
- Patient Visits.
- Service Encounters.
- Form Templates.
- Form Responses.
- Consent Documents.
- Reports.
- Sync Batches.
- Audit Logs.
- Smoke Test.
- Swagger.

### Backend Architecture Overview

Archivo:

```text
docs/BACKEND_ARCHITECTURE_OVERVIEW.md
```

Uso:

- Entender la arquitectura por capas.
- Explicar el backend a profesores, socios formadores o colaboradores.
- Revisar decisiones técnicas.
- Mantener visión de evolución del sistema.

Cubre:

- Domain.
- Application.
- Infrastructure.
- Contracts.
- Api.
- Flujo funcional del MVP.
- Formularios versionados.
- Consentimientos.
- Reportes.
- Sync offline.
- Auditoría.
- Seguridad actual.
- Integraciones futuras.
- Límites del MVP.

### Security Hardening Checklist

Archivo:

```text
docs/SECURITY_HARDENING_CHECKLIST.md
ENFORCED_ENDPOINT_SECURITY_STATUS.md
```

Uso:

- Preparar el sistema para ambientes reales.
- Evitar prometer seguridad absoluta sin validación.
- Definir ruta clara hacia producción segura.
- Identificar pendientes antes de usar datos reales.

Cubre:

- Autenticación.
- Autorización.
- Validación de entrada.
- SQL Injection.
- XSS.
- CSRF.
- CORS.
- HTTPS.
- Rate limiting.
- DDoS.
- Protección de datos sensibles.
- Logging seguro.
- Auditoría.
- Secrets management.
- Base de datos.
- Migraciones.
- Archivos y firmas.
- Sync offline.
- Reportes.
- CSV Injection.
- Headers de seguridad.
- OWASP API Security Top 10.
- Ambientes.
- Checklist previo a producción.

## 3. Scripts relevantes

### Smoke test local

Archivo:

```text
scripts/smoke-test-local.ps1
```

Uso:

```powershell
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud

powershell -ExecutionPolicy Bypass -File .\services\api-dotnet\scripts\smoke-test-local.ps1
```

Resultado esperado:

```text
SMOKE TEST COMPLETED SUCCESSFULLY
```

Este script debe probar el flujo operativo completo del MVP local.

## 4. OpenAPI / Swagger

Durante desarrollo local, abrir:

```text
https://localhost:7044/swagger
```

Uso:

- Explorar endpoints.
- Probar requests manuales.
- Revisar contratos.
- Validar documentación OpenAPI.
- Mostrar avance técnico.

## 5. Comandos esenciales

### Ubicación correcta del backend

```powershell
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet
```

### Build

```powershell
dotnet build Caritas.Brigadas.sln
```

### Tests

```powershell
dotnet test Caritas.Brigadas.sln
```

### API local

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

dotnet run `
  --project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --no-launch-profile `
  --urls "https://localhost:7044;http://localhost:5031"
```

### Health check

```powershell
curl.exe -k -sS "https://localhost:7044/api/v1/health"
```

### Reporte JSON

```powershell
$baseUrl = "https://localhost:7044"
$organizationId = "4df92032-4a1c-4cf2-b48f-15b570cd073a"

curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary"
```

### Reporte CSV

```powershell
curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary.csv"
```

## 6. Orden recomendado de lectura

Para alguien nuevo en el proyecto:

1. `BACKEND_ARCHITECTURE_OVERVIEW.md`
2. `ENDPOINT_INVENTORY.md`
3. `MVP_LOCAL_VALIDATION_CHECKLIST.md`
4. `SECURITY_HARDENING_CHECKLIST.md
ENFORCED_ENDPOINT_SECURITY_STATUS.md`

Para validar técnicamente:

1. `MVP_LOCAL_VALIDATION_CHECKLIST.md`
2. `scripts/smoke-test-local.ps1`
3. `ENDPOINT_INVENTORY.md`

Para preparar presentación:

1. `BACKEND_ARCHITECTURE_OVERVIEW.md`
2. `ENDPOINT_INVENTORY.md`
3. `SECURITY_HARDENING_CHECKLIST.md
ENFORCED_ENDPOINT_SECURITY_STATUS.md`

## 7. Estado del MVP backend local

El backend local actual cubre:

- Registro de organización.
- Usuarios.
- Roles y permisos base.
- Catálogo de servicios.
- Comunidades.
- Unidades móviles.
- Brigadas.
- Servicios por brigada.
- Pacientes.
- Visitas.
- Atenciones por servicio.
- Formularios JSON versionados.
- Respuestas de formularios.
- Consentimientos.
- Reportes JSON.
- Exportación CSV.
- Sync batches base.
- Auditoría defensiva de solo lectura.
- Smoke test local.
- Documentación técnica inicial.

## 8. Pendientes importantes

Antes de producción faltan como mínimo:

- Autenticación.
- Autorización por permiso en endpoints.
- Auditoría formal de escritura.
- Cifrado/protección de datos sensibles.
- Gestión de secretos.
- Paginación y filtros.
- Procesamiento real de sync batches.
- Exportación XLSX.
- Pruebas de carga.
- Pruebas de seguridad.
- Revisión legal de aviso de privacidad.
- Validación UX con usuarios reales.
- Despliegue cloud.
- Monitoreo y backups.

## 9. Regla de trabajo

Cada cambio debe cumplir:

```text
build pasa
tests pasan
smoke test pasa si aplica
documentación actualizada si cambia comportamiento
commit pequeño por unidad lógica
git status limpio al final
```

## 10. Nota profesional

Este backend debe presentarse como:

```text
MVP técnico local validable y base modular para sistema de brigadas de salud.
```

No debe presentarse todavía como:

```text
Sistema productivo seguro
Sistema legalmente validado
Sistema listo para datos reales
Sistema inmune a ataques
Sistema con operación offline completa
```

Authorization Implementation Status

Archivo:

docs/AUTHORIZATION_IMPLEMENTATION_STATUS.md

Uso:

Revisar el estado real de autenticación de desarrollo.
Revisar policies por permiso.
Revisar validación por organización.
Revisar tests de autorización.

Identificar pendientes para JWT/OIDC productivo.
Production Authentication Migration Plan

Archivo:

docs/PRODUCTION_AUTHENTICATION_MIGRATION_PLAN.md

Uso:

Planear migración de headers de desarrollo a JWT/OIDC.
Definir claims mínimos.
Definir variables de entorno.
Evitar que DevelopmentAuthenticationHandler llegue a producción.

Preparar integración futura con proveedor de identidad.
Authentication Environment Variables

Archivo:

docs/AUTHENTICATION_ENVIRONMENT_VARIABLES.md

Uso:

Configurar autenticación local.
Configurar JWT Bearer en staging/producción.
Documentar headers de desarrollo.
Documentar claims esperados.

Documentar variables de entorno.
