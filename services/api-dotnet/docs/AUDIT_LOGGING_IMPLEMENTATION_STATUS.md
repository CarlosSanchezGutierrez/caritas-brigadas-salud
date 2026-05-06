# Audit Logging Implementation Status

Este documento resume el estado actual de auditoría formal del backend de Cáritas Brigadas de Salud.

## 1. Estado general

El backend ya cuenta con una primera implementación formal de auditoría.

Estado actual:

```text
Implementado para MVP local.

La auditoría ya no depende únicamente de un endpoint defensivo de lectura. Ahora existe una entidad formal AuditLog, migración de base de datos, repositorio de escritura, servicio de logging y filtros automáticos para registrar acciones relevantes.

2. Componentes implementados
Entidad de dominio

Archivo:

src/Caritas.Brigadas.Domain/Entities/AuditLog.cs

Responsabilidad:

Representar un evento de auditoría formal.
Validar campos obligatorios.
Normalizar campos opcionales.
Guardar acción, entidad, usuario, organización, fecha, correlation id, IP, user agent y detalles.

Campos principales:

Id
OrganizationId
UserId
Action
EntityName
EntityId
DetailsJson
CorrelationId
IpAddress
UserAgent
OccurredAtUtc
CreatedAtUtc
3. Configuración EF Core

Archivo:

src/Caritas.Brigadas.Infrastructure/Persistence/Configurations/AuditLogConfiguration.cs

Tabla:

AuditLogs

Índices principales:

OrganizationId
OrganizationId + OccurredAtUtc
EntityName + EntityId
UserId
4. Migración

Migración:

AddAuditLogs

Objetivo:

Crear tabla formal AuditLogs.
Actualizar CaritasDbContextModelSnapshot.
Permitir persistencia real de auditoría.
5. DbContext

Archivo:

src/Caritas.Brigadas.Infrastructure/Persistence/CaritasDbContext.cs

DbSet agregado:

public DbSet<AuditLog> AuditLogs { get; set; } = null!;
6. Repositorio de escritura

Archivos:

src/Caritas.Brigadas.Application/Audit/CreateAuditLogCommand.cs
src/Caritas.Brigadas.Application/Audit/IAuditLogWriteRepository.cs
src/Caritas.Brigadas.Infrastructure/Audit/AuditLogWriteRepository.cs

Responsabilidad:

Crear registros de auditoría.
Validar organización.
Validar usuario si se proporciona.
Persistir AuditLog.
Devolver AuditLogSummaryDto.
7. Servicio de auditoría

Archivos:

src/Caritas.Brigadas.Application/Audit/IAuditLogger.cs
src/Caritas.Brigadas.Api/Audit/HttpAuditLogger.cs
src/Caritas.Brigadas.Api/Extensions/AuditLoggingServiceExtensions.cs

Responsabilidad:

Centralizar creación de auditoría desde la API.
Obtener usuario actual.
Obtener correlation id.
Obtener IP y user agent cuando estén disponibles.
Evitar romper el flujo principal si falla la auditoría.
Registrar warning si la auditoría falla.

Regla importante:

La auditoría no debe tirar el endpoint principal si falla el registro de auditoría.
8. Códigos de auditoría

Archivo:

src/Caritas.Brigadas.Application/Audit/AuditActionCodes.cs

Acciones actuales:

organizations.create
users.create
roles.assign
services.seed
form-templates.seed
communities.create
mobile-units.create
brigades.create
brigade-services.assign
patients.create
patient-visits.create
service-encounters.create
form-responses.create
consent-documents.create
reports.summary.read
reports.summary.export
sync-batches.create
audit-logs.read
9. Auditoría de reportes

Archivo principal:

src/Caritas.Brigadas.Api/Controllers/ReportsController.cs

Acciones auditadas:

AcciónCódigo
Consulta de resumen JSONreports.summary.read
Exportación CSVreports.summary.export

Motivo:

La consulta de reportes puede revelar información operativa.
La exportación CSV debe quedar trazada.
Las exportaciones son acciones sensibles en sistemas reales.
10. Auditoría clínica

Archivos:

src/Caritas.Brigadas.Api/Audit/ClinicalWriteAuditActionMapper.cs
src/Caritas.Brigadas.Api/Audit/ClinicalWriteAuditActionFilter.cs
src/Caritas.Brigadas.Api/Extensions/ClinicalWriteAuditServiceExtensions.cs

Acciones auditadas:

Endpoint lógicoAcción
Crear pacientepatients.create
Crear visitapatient-visits.create
Crear atención por servicioservice-encounters.create
Crear respuesta de formularioform-responses.create
Crear consentimientoconsent-documents.create

Motivo:

Estas acciones manipulan datos sensibles o clínicos.

11. Auditoría operativa

Archivos:

src/Caritas.Brigadas.Api/Audit/OperationalWriteAuditActionMapper.cs
src/Caritas.Brigadas.Api/Audit/OperationalWriteAuditActionFilter.cs
src/Caritas.Brigadas.Api/Extensions/OperationalWriteAuditServiceExtensions.cs

Acciones auditadas:

Endpoint lógicoAcción
Crear organizaciónorganizations.create
Crear usuariousers.create
Seed de seguridadroles.assign
Asignar rolroles.assign
Seed de serviciosservices.seed
Seed de plantillasform-templates.seed
Crear comunidadcommunities.create
Crear unidad móvilmobile-units.create
Crear brigadabrigades.create
Asignar servicio a brigadabrigade-services.assign
Crear sync batchsync-batches.create

Motivo:

Estas acciones modifican la configuración operativa o administrativa del sistema.

12. Filtros automáticos

Actualmente existen filtros globales para auditoría:

ClinicalWriteAuditActionFilter
OperationalWriteAuditActionFilter

Funcionamiento:

Request
  -> Controller ejecuta acción
  -> Si la respuesta fue exitosa
  -> Mapper identifica acción auditable
  -> Se extrae organizationId
  -> Se extrae entityId si existe
  -> IAuditLogger registra AuditLog
13. Lectura de auditoría

Endpoint existente:

GET /api/v1/organizations/{organizationId}/audit-logs
GET /api/v1/audit-logs/{auditLogId}

Permiso requerido:

audit-logs.read

Estado:

Protegido por autorización.
14. Pruebas agregadas

Tests actuales relacionados con auditoría:

AuditLogTests
CreateAuditLogCommandTests
AuditActionCodesTests
HttpAuditLoggerTests
ClinicalWriteAuditActionMapperTests
OperationalWriteAuditActionMapperTests

Validan:

Creación de entidad AuditLog.
Validaciones de dominio.
Comando de creación de auditoría.
Códigos de acción.
Construcción de comando desde HttpAuditLogger.
Mapeo de endpoints clínicos.
Mapeo de endpoints operativos.
15. Limitaciones actuales

La auditoría actual todavía no incluye:

Interceptor automático de EF Core.
Captura before/after.
Diferencias campo por campo.
Auditoría de updates/deletes.
Auditoría de login/logout.
Auditoría de fallos de autorización.
Auditoría de 401/403.
Filtros avanzados por fecha, usuario, entidad o acción.
Exportación de auditoría.
Retención formal.
Enmascaramiento avanzado de datos sensibles dentro de DetailsJson.
16. Riesgos pendientes

Pendientes importantes:

Evitar guardar datos sensibles crudos en DetailsJson.
Definir política de retención.
Definir quién puede leer auditoría.
Definir exportación segura.
Agregar paginación a consultas de auditoría.
Agregar filtros.
Probar persistencia real en smoke test.
Auditar intentos fallidos si se requiere.
Evaluar volumen de registros en brigadas grandes.
17. Criterio de éxito local

La auditoría formal está lista para MVP local cuando:

dotnet build pasa.
dotnet test pasa.
Migración AddAuditLogs existe.
Tabla AuditLogs existe en LocalDB.
API corre en Development.
Smoke test pasa.
Crear acciones clínicas genera registros de auditoría.
Exportar reporte genera registro de auditoría.
GET /audit-logs responde correctamente.
git status queda limpio.
18. Siguiente paso recomendado

El siguiente paso técnico recomendado es validar auditoría en el smoke test:

chore(api): include audit log validation in smoke test

Objetivo:

Confirmar que AuditLogs se llena.
Confirmar que report export queda auditado.
Confirmar que clinical writes quedan auditados.
Confirmar que audit-logs responde datos reales.
