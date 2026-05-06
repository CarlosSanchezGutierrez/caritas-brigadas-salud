# Backend Architecture Overview

Este documento resume la arquitectura backend actual del MVP local de Cáritas Brigadas de Salud.

## 1. Objetivo del backend

El backend tiene como objetivo digitalizar la operación básica de brigadas de salud de Cáritas, permitiendo:

- Registrar organizaciones.
- Registrar usuarios.
- Configurar roles y permisos.
- Configurar servicios disponibles.
- Registrar comunidades y unidades móviles.
- Crear brigadas.
- Asignar servicios a brigadas.
- Registrar pacientes.
- Registrar visitas de pacientes.
- Registrar atenciones por servicio.
- Capturar respuestas de formularios.
- Guardar avisos de privacidad y consentimientos.
- Consultar reportes operativos.
- Exportar reportes en CSV.
- Recibir lotes de sincronización offline.
- Consultar auditoría si existe tabla compatible.

## 2. Stack actual

Backend:

- .NET
- ASP.NET Core Web API
- Entity Framework Core
- SQL Server / SQL Server LocalDB
- Swagger / OpenAPI
- PowerShell para automatización local
- Git / GitHub para versionamiento

Base de datos local:

- SQL Server LocalDB
- Base: `CaritasBrigadas_Local`

## 3. Arquitectura por capas

El backend está organizado en capas separadas para mantener orden, escalabilidad y separación de responsabilidades.

### 3.1 Domain

Ruta:

```text
services/api-dotnet/src/Caritas.Brigadas.Domain
```

Responsabilidad:

- Entidades centrales del negocio.
- Reglas básicas de dominio.
- Estados y enumeraciones.
- Excepciones de dominio.
- Comportamiento propio de entidades.

Ejemplos:

- Organization
- User
- Role
- Permission
- Service
- Community
- MobileUnit
- Brigade
- BrigadeService
- Patient
- PatientVisit
- ServiceEncounter
- FormTemplate
- FormResponse
- ConsentDocument
- SyncBatch

Principio:

```text
El dominio no debe depender de infraestructura, base de datos, controladores ni detalles de HTTP.
```

### 3.2 Application

Ruta:

```text
services/api-dotnet/src/Caritas.Brigadas.Application
```

Responsabilidad:

- Interfaces de repositorios.
- Contratos de entrada/salida a nivel aplicación.
- Definición de capacidades que la infraestructura implementa.

Ejemplos:

- IOrganizationReadRepository
- IUserWriteRepository
- IServiceSeedRepository
- IBrigadeReadRepository
- IPatientWriteRepository
- IReportReadRepository
- ISyncBatchWriteRepository
- IAuditLogReadRepository

Principio:

```text
Application define qué necesita el sistema, pero no cómo se implementa.
```

### 3.3 Infrastructure

Ruta:

```text
services/api-dotnet/src/Caritas.Brigadas.Infrastructure
```

Responsabilidad:

- Entity Framework Core.
- DbContext.
- Repositorios concretos.
- SQL Server.
- Migraciones.
- Implementaciones de lectura, escritura y seed.

Ejemplos:

- CaritasDbContext
- OrganizationReadRepository
- PatientWriteRepository
- FormTemplateSeedRepository
- ReportReadRepository
- AuditLogReadRepository

Principio:

```text
Infrastructure implementa los detalles técnicos.
```

### 3.4 Contracts

Ruta:

```text
services/api-dotnet/src/Caritas.Brigadas.Contracts
```

Responsabilidad:

- DTOs.
- Requests.
- Responses.
- Contratos públicos de API.

Ejemplos:

- CreatePatientRequest
- PatientSummaryDto
- CreateServiceEncounterRequest
- ServiceEncounterSummaryDto
- OrganizationReportSummaryDto
- ApiResponse
- ApiErrorResponse

Principio:

```text
Contracts define lo que la API recibe y entrega.
```

### 3.5 Api

Ruta:

```text
services/api-dotnet/src/Caritas.Brigadas.Api
```

Responsabilidad:

- Controladores HTTP.
- Swagger.
- Middleware.
- Configuración de API.
- Manejo de errores.
- CORS.
- Seguridad base.
- Inyección de dependencias.

Ejemplos:

- OrganizationsController
- UsersController
- ServicesController
- BrigadesController
- PatientsController
- ReportsController
- SyncBatchesController
- AuditLogsController

Principio:

```text
Api expone casos de uso mediante HTTP, pero no debe concentrar lógica pesada de negocio.
```

## 4. Flujo lógico del MVP

El flujo funcional principal es:

```text
Organization
  -> Users
  -> Security
  -> Services
  -> FormTemplates
  -> Communities / MobileUnits
  -> Brigades
  -> BrigadeServices
  -> Patients
  -> PatientVisits
  -> ServiceEncounters
  -> FormResponses
  -> ConsentDocuments
  -> Reports
  -> SyncBatches
  -> AuditLogs
```

## 5. Modelo funcional de atención

Una brigada representa una jornada operativa.

Una visita representa la llegada o atención de un paciente dentro de una brigada.

Una atención por servicio representa un servicio concreto recibido durante una visita.

Ejemplo:

```text
Paciente Juan Pérez
  -> Visita a Brigada Cáritas Centro
      -> Atención Medicina General
          -> Formulario GENERAL_MEDICINE_V1
          -> Respuesta capturada
          -> Consentimiento firmado
```

## 6. Decisión sobre HealthProvider

No se debe usar únicamente el término Doctor para los usuarios clínicos.

Motivo:

- Puede haber médicos.
- Puede haber estudiantes de servicio.
- Puede haber psicólogos.
- Puede haber nutriólogos.
- Puede haber optometristas.
- Puede haber trabajadores sociales.
- Puede haber voluntarios con funciones clínicas u operativas.

Rol base recomendado:

```text
HealthProvider
```

Este rol puede representar cualquier prestador de servicio de salud o atención directa.

## 7. Servicios base

Los servicios base actuales son:

```text
GENERAL_MEDICINE
DENTISTRY
OPTOMETRY
NUTRITION
PSYCHOLOGY
MEDICATION_DELIVERY
MEDICAL_REFERRAL
SOCIAL_WORK
```

Estos servicios pertenecen al catálogo de la organización.

Una brigada puede tener solo algunos servicios asignados.

## 8. Formularios versionados

Los formularios se manejan como plantillas JSON versionadas.

Ventaja:

- El frontend no necesita hardcodear todos los formularios.
- Se pueden cambiar versiones sin romper capturas anteriores.
- Se puede saber con qué versión fue capturada una respuesta.
- Se habilita compatibilidad futura con web, iOS, Android y modo offline.

Plantillas base:

```text
GENERAL_MEDICINE_V1
DENTISTRY_V1
OPTOMETRY_V1
NUTRITION_V1
PSYCHOLOGY_V1
MEDICATION_DELIVERY_V1
MEDICAL_REFERRAL_V1
```

## 9. Consentimientos y aviso de privacidad

El backend guarda documentos firmados con:

- Tipo de consentimiento.
- Versión del documento.
- Snapshot del texto aceptado.
- Firma o evidencia.
- Paciente relacionado.
- Visita relacionada si aplica.
- Usuario que capturó o firmó.
- Fecha de firma.
- Datos de tutor si aplica.

Esto es importante porque no basta con guardar que el paciente aceptó. También debe conservarse qué aceptó exactamente y bajo qué versión.

## 10. Reportes

El reporte actual entrega resumen operativo en JSON y CSV.

Endpoint JSON:

```text
GET /api/v1/organizations/{organizationId}/reports/summary
```

Endpoint CSV:

```text
GET /api/v1/organizations/{organizationId}/reports/summary.csv
```

Métricas actuales:

- Usuarios.
- Roles.
- Permisos.
- Servicios.
- Comunidades.
- Unidades móviles.
- Brigadas.
- Servicios asignados a brigadas.
- Pacientes.
- Visitas.
- Atenciones.
- Plantillas.
- Respuestas.
- Consentimientos.
- Registros clínicos.

## 11. Sincronización offline

El endpoint de sync batches permite recibir lotes de sincronización.

Uso previsto:

- Operación en campo.
- Captura sin internet.
- Envío posterior cuando haya conexión.
- Registro de lote, usuario, brigada, dispositivo y conteo de eventos.

Estado actual:

- Valida JSON.
- Registra batch.
- Registra conteo de eventos.
- No procesa evento por evento todavía.
- No persiste payload completo si la entidad actual no lo soporta.

Mejora futura:

- SyncBatchItem.
- Idempotencia por clientBatchId.
- Persistencia completa de payload.
- Estados de conflicto.
- Reintentos.
- Procesamiento por tipo de evento.
- Resolución manual de conflictos.

## 12. Auditoría

El endpoint de auditoría actual es defensivo y de solo lectura.

Si existe una tabla compatible:

- AuditLogs
- AuditEntries
- AuditEvents

entonces intenta leer eventos.

Si no existe tabla compatible:

```json
{
  "success": true,
  "data": []
}
```

Esto evita romper el MVP mientras se define la auditoría formal.

Mejora futura:

- Entidad AuditLog formal.
- Interceptor de EF Core.
- Registro automático de cambios sensibles.
- Filtros por usuario, entidad, acción y fecha.
- Exportación de auditoría.
- Separación de auditoría técnica y auditoría clínica/legal.

## 13. Seguridad actual

El backend ya contempla:

- Roles.
- Permisos.
- Asignación de roles.
- Configuración HTTPS en desarrollo.
- CORS local.
- Rate limiting configurable.
- Respuestas de error estandarizadas.
- TraceId/correlationId.

Pendiente para producción:

- Autenticación real.
- JWT / OAuth / proveedor de identidad.
- Autorización por permiso en cada endpoint.
- Cifrado de campos sensibles.
- Protección avanzada contra abuso.
- Revisión de OWASP API Security Top 10.
- Hardening de configuración.
- Secret management.
- Backups.
- Monitoreo.
- Logging estructurado.

## 14. Estrategia de pruebas

Pruebas actuales:

- Unit tests por proyectos.
- Build completo.
- Test completo.
- Smoke test local vía PowerShell.

Script principal:

```text
services/api-dotnet/scripts/smoke-test-local.ps1
```

El smoke test valida el flujo funcional completo del MVP.

## 15. Comandos locales principales

Build:

```powershell
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet
dotnet build Caritas.Brigadas.sln
```

Tests:

```powershell
dotnet test Caritas.Brigadas.sln
```

API local:

```powershell
$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

dotnet run `
  --project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --no-launch-profile `
  --urls "https://localhost:7044;http://localhost:5031"
```

Smoke test:

```powershell
cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud
powershell -ExecutionPolicy Bypass -File .\services\api-dotnet\scripts\smoke-test-local.ps1
```

## 16. Principios de diseño

El backend debe priorizar:

- Claridad.
- Modularidad.
- Separación de capas.
- Trazabilidad.
- Validación local reproducible.
- Evolución incremental.
- Preparación para modo offline.
- Preparación para seguridad real.
- Preparación para reportes.
- Preparación para integraciones futuras.

## 17. Integraciones futuras previstas

Posibles integraciones:

- Frontend web.
- App móvil.
- SQL Server institucional.
- Azure SQL.
- AWS RDS SQL Server.
- Blob/File Storage para documentos.
- API Gateway.
- LLM Gateway para módulos de asistencia futura.
- Dashboards.
- Exportación XLSX.
- Sistema de notificaciones.
- Servicio de identidad.
- Auditoría avanzada.
- Módulo de analítica.
- Módulo de investigación/data science.

## 18. Límite actual del MVP

El MVP local actual no debe venderse como producto terminado.

Sí puede presentarse como:

```text
Prototipo backend funcional local
MVP técnico validable
Base modular para sistema de brigadas de salud
Arquitectura inicial preparada para evolución
```

No debe prometer todavía:

- Seguridad productiva completa.
- Cumplimiento legal final.
- Operación offline completa.
- Escalabilidad probada.
- Pentesting.
- App móvil.
- Dashboard final.
- Autenticación institucional.
- Integración directa con sistemas existentes de Cáritas.

## 19. Siguiente evolución técnica recomendada

Orden recomendado:

1. Autenticación y autorización real.
2. Auditoría formal.
3. Cifrado y protección de datos sensibles.
4. Procesamiento real de sync batches.
5. Exportación XLSX.
6. Filtros y paginación.
7. Frontend administrativo.
8. Frontend operativo de brigada.
9. Validación UX con usuarios reales.
10. Preparación para despliegue cloud.

## 20. Criterio de arquitectura sana

La arquitectura va por buen camino si:

- Cada módulo tiene contratos, interfaces, repositorios y controller.
- Los endpoints devuelven respuestas estandarizadas.
- El build pasa.
- Los tests pasan.
- El smoke test pasa.
- No hay archivos temporales.
- La documentación está actualizada.
- Cada bloque se commitea por unidad lógica.
- No se mezclan features grandes sin validación.
21. Estado de autorización actual

La arquitectura backend ya incluye una capa de autorización aplicada sobre endpoints principales.

Componentes:

ICurrentUserContext
HttpCurrentUserContext
PermissionCodes
RoleCodes
DevelopmentAuthenticationHandler
PermissionRequirement
PermissionAuthorizationHandler
OrganizationAccessAuthorizer
OrganizationAccessActionFilter

Flujo actual:

Request
  -> Authentication
  -> ClaimsPrincipal
  -> CurrentUserContext
  -> Permission policy
  -> Organization access filter
  -> Controller
  -> Repository
  -> Database

Estado:

Implementado para MVP local.

Pendiente para producción:

JWT/OIDC real
Proveedor de identidad
Claims reales
Auditoría formal
Hardening completo

