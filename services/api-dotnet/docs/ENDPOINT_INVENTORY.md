# API Endpoint Inventory

Inventario funcional de endpoints del MVP local de Cáritas Brigadas de Salud.

## Base URL local

HTTPS recomendado:

    https://localhost:7044

HTTP local alternativo:

    http://localhost:5031

## Convenciones generales

Todas las respuestas JSON principales siguen una envoltura estándar:

    success
    data
    message
    errorCode
    details
    traceId
    timestampUtc

Los endpoints de escritura devuelven normalmente:

    201 Created

Los endpoints de lectura devuelven normalmente:

    200 OK

Errores esperados:

    400 Bad Request
    404 Not Found
    409 Conflict
    503 Service Unavailable
    500 Internal Server Error

## 1. Health

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/health | Valida que la API esté viva. |

Uso local:

    curl.exe -k -sS "https://localhost:7044/api/v1/health"

## 2. Organizations

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations | Lista organizaciones. |
| GET | /api/v1/organizations/{organizationId} | Obtiene una organización por ID. |
| POST | /api/v1/organizations | Crea una organización. |

Entidad funcional principal:

    Organization

Uso MVP:

    Registrar Cáritas de Monterrey como organización principal del sistema.

## 3. Users

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/users | Lista usuarios de una organización. |
| GET | /api/v1/users/{userId} | Obtiene un usuario por ID. |
| POST | /api/v1/organizations/{organizationId}/users | Crea usuario. |

Entidad funcional principal:

    User

Uso MVP:

    Registrar colaboradores, estudiantes, coordinadores, personal operativo y usuarios internos.

Nota de modelo:

    No todos los usuarios clínicos deben llamarse doctores.
    El rol base recomendado es HealthProvider cuando aplica a médico, psicólogo, nutriólogo, optometrista u otro prestador de servicio.

## 4. Security

| Método | Ruta | Descripción |
|---|---|---|
| POST | /api/v1/organizations/{organizationId}/security/seed-defaults | Inicializa roles y permisos base. |
| GET | /api/v1/organizations/{organizationId}/security/roles | Lista roles. |
| GET | /api/v1/organizations/{organizationId}/security/permissions | Lista permisos. |
| GET | /api/v1/organizations/{organizationId}/security/role-permissions | Lista relación rol-permiso. |
| POST | /api/v1/organizations/{organizationId}/security/user-role-assignments | Asigna rol a usuario. |

Entidades funcionales principales:

    Role
    Permission
    RolePermission
    UserRoleAssignment

Uso MVP:

    Control base de acceso y separación futura de permisos por perfil.

Roles base sugeridos:

    SuperAdmin
    OrganizationAdmin
    Coordinator
    HealthProvider
    Reception
    Viewer
    Auditor

## 5. Services

| Método | Ruta | Descripción |
|---|---|---|
| POST | /api/v1/organizations/{organizationId}/services/seed-defaults | Inicializa servicios base. |
| GET | /api/v1/organizations/{organizationId}/services | Lista servicios. |
| GET | /api/v1/services/{serviceId} | Obtiene servicio por ID. |

Entidad funcional principal:

    Service

Servicios base del MVP:

    GENERAL_MEDICINE
    DENTISTRY
    OPTOMETRY
    NUTRITION
    PSYCHOLOGY
    MEDICATION_DELIVERY
    MEDICAL_REFERRAL
    SOCIAL_WORK

## 6. Communities

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/communities | Lista comunidades. |
| GET | /api/v1/communities/{communityId} | Obtiene comunidad por ID. |
| POST | /api/v1/organizations/{organizationId}/communities | Crea comunidad. |

Entidad funcional principal:

    Community

Uso MVP:

    Registrar colonias, comunidades, puntos de atención o zonas donde se realiza una brigada.

## 7. Mobile Units

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/mobile-units | Lista unidades móviles. |
| GET | /api/v1/mobile-units/{mobileUnitId} | Obtiene unidad móvil por ID. |
| POST | /api/v1/organizations/{organizationId}/mobile-units | Crea unidad móvil. |

Entidad funcional principal:

    MobileUnit

Uso MVP:

    Registrar unidades médicas móviles, vehículos, módulos o recursos físicos de operación.

## 8. Brigades

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/brigades | Lista brigadas. |
| GET | /api/v1/brigades/{brigadeId} | Obtiene brigada por ID. |
| POST | /api/v1/organizations/{organizationId}/brigades | Crea brigada. |

Entidad funcional principal:

    Brigade

Uso MVP:

    Registrar una jornada operativa de atención en una fecha, ubicación y comunidad.

## 9. Brigade Services

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/brigades/{brigadeId}/services | Lista servicios asignados a una brigada. |
| POST | /api/v1/brigades/{brigadeId}/services | Asigna servicio a una brigada. |

Entidad funcional principal:

    BrigadeService

Uso MVP:

    Definir qué servicios estarán disponibles en una brigada específica.

Ejemplo:

    Una brigada puede tener Medicina General, Nutrición, Psicología y Optometría, pero no necesariamente todos los servicios del catálogo.

## 10. Patients

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/patients | Lista pacientes. |
| GET | /api/v1/patients/{patientId} | Obtiene paciente por ID. |
| POST | /api/v1/organizations/{organizationId}/patients | Crea paciente. |

Entidad funcional principal:

    Patient

Uso MVP:

    Registrar datos básicos del paciente, permitiendo registros parciales para casos sensibles o población migrante.

Consideraciones:

    No usar datos reales en pruebas locales.
    Mantener principio de mínima información necesaria.

## 11. Patient Visits

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/patient-visits | Lista visitas. |
| GET | /api/v1/patient-visits/{visitId} | Obtiene visita por ID. |
| POST | /api/v1/organizations/{organizationId}/patient-visits | Crea visita. |

Entidad funcional principal:

    PatientVisit

Uso MVP:

    Registrar que un paciente llegó o fue atendido en una brigada específica.

Relaciones principales:

    Patient
    Brigade
    RegisteredByUser

## 12. Service Encounters

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/service-encounters | Lista atenciones por servicio. |
| GET | /api/v1/service-encounters/{encounterId} | Obtiene atención por ID. |
| POST | /api/v1/organizations/{organizationId}/service-encounters | Crea atención por servicio. |

Entidad funcional principal:

    ServiceEncounter

Uso MVP:

    Registrar una atención concreta dentro de una visita.

Ejemplo:

    Paciente X en Brigada Y recibe atención de Medicina General.

## 13. Form Templates

| Método | Ruta | Descripción |
|---|---|---|
| POST | /api/v1/organizations/{organizationId}/form-templates/seed-defaults | Inicializa plantillas base. |
| GET | /api/v1/organizations/{organizationId}/form-templates | Lista plantillas. |
| GET | /api/v1/form-templates/{formTemplateId} | Obtiene plantilla por ID. |

Entidad funcional principal:

    FormTemplate

Uso MVP:

    Definir formularios JSON versionados por servicio.

Plantillas base:

    GENERAL_MEDICINE_V1
    DENTISTRY_V1
    OPTOMETRY_V1
    NUTRITION_V1
    PSYCHOLOGY_V1
    MEDICATION_DELIVERY_V1
    MEDICAL_REFERRAL_V1

Criterio técnico:

    Los formularios no deben hardcodearse en frontend.
    Deben poder evolucionar por versión.

## 14. Form Responses

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/form-responses | Lista respuestas. |
| GET | /api/v1/form-responses/{formResponseId} | Obtiene respuesta por ID. |
| POST | /api/v1/organizations/{organizationId}/form-responses | Crea respuesta. |

Entidad funcional principal:

    FormResponse

Uso MVP:

    Guardar la respuesta clínica o administrativa capturada con base en una plantilla.

Relaciones principales:

    ServiceEncounter
    FormTemplate
    SubmittedByUser

## 15. Consent Documents

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/consent-documents | Lista consentimientos. |
| GET | /api/v1/consent-documents/{consentDocumentId} | Obtiene consentimiento por ID. |
| POST | /api/v1/organizations/{organizationId}/consent-documents | Crea consentimiento firmado. |

Entidad funcional principal:

    ConsentDocument

Uso MVP:

    Guardar aviso de privacidad, consentimiento o documento legal firmado.

Consideraciones:

    Debe incluir versión del documento.
    Debe guardar snapshot del texto aceptado.
    Debe registrar firma o evidencia.
    Debe contemplar tutor/representante si aplica.

## 16. Reports

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/reports/summary | Devuelve resumen operativo JSON. |
| GET | /api/v1/organizations/{organizationId}/reports/summary.csv | Exporta resumen operativo en CSV. |

Contrato principal:

    OrganizationReportSummaryDto

Métricas actuales:

    usersCount
    rolesCount
    permissionsCount
    rolePermissionsCount
    servicesCount
    communitiesCount
    mobileUnitsCount
    brigadesCount
    brigadeServiceAssignmentsCount
    patientsCount
    patientVisitsCount
    serviceEncountersCount
    formTemplatesCount
    formResponsesCount
    consentDocumentsCount
    clinicalRecordsCount

Uso MVP:

    Dar evidencia operativa rápida para validación local, presentación técnica y exportación básica.

## 17. Sync Batches

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/sync-batches | Lista lotes de sincronización. |
| GET | /api/v1/sync-batches/{syncBatchId} | Obtiene lote por ID. |
| POST | /api/v1/organizations/{organizationId}/sync-batches | Recibe lote de sincronización. |

Entidad funcional principal:

    SyncBatch

Uso MVP:

    Base para operación offline/sin conexión.

Estado actual:

    Registra batch, usuario, brigada, dispositivo, conteo de eventos, inicio, estado y cierre.
    El payload JSON se valida en endpoint, pero la entidad actual no persiste payload completo.

Mejora futura:

    Agregar persistencia de payload.
    Procesar eventos por tipo.
    Registrar conflictos.
    Manejar reintentos e idempotencia por clientBatchId.
    Separar SyncBatch y SyncBatchItem.

## 18. Audit Logs

| Método | Ruta | Descripción |
|---|---|---|
| GET | /api/v1/organizations/{organizationId}/audit-logs | Lista eventos de auditoría. |
| GET | /api/v1/audit-logs/{auditLogId} | Obtiene evento de auditoría por ID. |

Contrato principal:

    AuditLogSummaryDto

Uso MVP:

    Consulta de auditoría si existe tabla compatible.

Estado actual:

    Endpoint de solo lectura.
    No crea migración.
    Si no existe tabla de auditoría, responde data: [] sin romper la API.

Mejora futura:

    Definir entidad formal AuditLog.
    Registrar cambios sensibles.
    Registrar usuario, IP, traceId, acción, entidad y payload.
    Agregar filtros por fecha, acción y usuario.

## 19. Smoke Test

Script:

    services/api-dotnet/scripts/smoke-test-local.ps1

Ejecutar desde raíz del repositorio:

    powershell -ExecutionPolicy Bypass -File .\services\api-dotnet\scripts\smoke-test-local.ps1

Debe validar:

    Health
    Organizations
    Users
    Services
    Form templates
    Communities
    Mobile units
    Brigades
    Brigade services
    Patients
    Patient visits
    Service encounters
    Form responses
    Consent documents
    Reports JSON
    Reports CSV
    Sync batches
    Audit logs

Resultado esperado:

    SMOKE TEST COMPLETED SUCCESSFULLY

## 20. Swagger

Ruta local:

    https://localhost:7044/swagger

Uso:

    Validación visual de endpoints.
    Pruebas manuales.
    Revisión de contratos.
    Presentación técnica inicial.

## 21. Orden funcional recomendado para pruebas manuales

1. Health.
2. Crear/listar organization.
3. Crear/listar users.
4. Seed security.
5. Seed services.
6. Seed form templates.
7. Crear community.
8. Crear mobile unit.
9. Crear brigade.
10. Asignar service a brigade.
11. Crear patient.
12. Crear patient visit.
13. Crear service encounter.
14. Crear form response.
15. Crear consent document.
16. Consultar reports summary.
17. Exportar summary CSV.
18. Crear sync batch.
19. Consultar audit logs.

## 22. Alcance cubierto por el inventario

Este inventario cubre el MVP backend local actual.

No implica todavía:

    Autenticación productiva.
    Autorización estricta por permiso en cada endpoint.
    Cifrado avanzado de campos sensibles.
    Procesamiento real de lotes offline.
    Auditoría completa de escritura.
    Exportación XLSX.
    Frontend web.
    App móvil.
    Pruebas de carga.
    Pentesting.
    Validación legal final.
