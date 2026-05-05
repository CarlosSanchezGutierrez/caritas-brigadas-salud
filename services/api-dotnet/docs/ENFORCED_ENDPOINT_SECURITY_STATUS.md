# Enforced Endpoint Security Status

Este documento resume qué endpoints del MVP backend ya tienen protección de autorización aplicada mediante policies de permisos.

## 1. Estado general

El backend ya cuenta con:

- `ICurrentUserContext`.
- Constantes de permisos.
- Constantes de roles.
- Claims estándar para usuario actual.
- Authentication handler de desarrollo.
- Headers de desarrollo:
  - `X-Dev-User-Id`
  - `X-Dev-Organization-Id`
  - `X-Dev-Roles`
  - `X-Dev-Permissions`
  - `X-Dev-Name`
  - `X-Dev-Email`
- Policies por permiso.
- Authorization handler por permiso.
- Endpoints protegidos gradualmente.
- Smoke test actualizado para mandar headers de desarrollo.
- Tests de comportamiento de autorización.

## 2. Endpoints públicos

Estos endpoints pueden seguir públicos en Development:

| Endpoint | Estado | Motivo |
|---|---|---|
| `GET /api/v1/health` | Público | Health check operativo. |
| `/swagger` | Público en Development | Documentación y pruebas locales. |

En producción, Swagger debe restringirse o deshabilitarse según ambiente.

## 3. Reports

| Endpoint | Permiso requerido |
|---|---|
| `GET /api/v1/organizations/{organizationId}/reports/summary` | `reports.read` |
| `GET /api/v1/organizations/{organizationId}/reports/summary.csv` | `reports.export` |

Estado:

```text
Protegido
4. Audit Logs
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/audit-logsaudit-logs.read
GET /api/v1/audit-logs/{auditLogId}audit-logs.read

Estado:

Protegido
5. Patients
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/patientspatients.read
GET /api/v1/patients/{patientId}patients.read
POST /api/v1/organizations/{organizationId}/patientspatients.write

Estado:

Protegido
6. Patient Visits
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/patient-visitspatient-visits.read
GET /api/v1/patient-visits/{visitId}patient-visits.read
POST /api/v1/organizations/{organizationId}/patient-visitspatient-visits.write

Estado:

Protegido
7. Service Encounters
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/service-encountersservice-encounters.read
GET /api/v1/service-encounters/{encounterId}service-encounters.read
POST /api/v1/organizations/{organizationId}/service-encountersservice-encounters.write

Estado:

Protegido
8. Form Templates
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/form-templatesform-templates.read
GET /api/v1/form-templates/{formTemplateId}form-templates.read
POST /api/v1/organizations/{organizationId}/form-templates/seed-defaultsform-templates.seed

Estado:

Protegido
9. Form Responses
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/form-responsesform-responses.read
GET /api/v1/form-responses/{formResponseId}form-responses.read
POST /api/v1/organizations/{organizationId}/form-responsesform-responses.write

Estado:

Protegido
10. Consent Documents
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/consent-documentsconsent-documents.read
GET /api/v1/consent-documents/{consentDocumentId}consent-documents.read
POST /api/v1/organizations/{organizationId}/consent-documentsconsent-documents.write

Estado:

Protegido
11. Services
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/servicesservices.read
GET /api/v1/services/{serviceId}services.read
POST /api/v1/organizations/{organizationId}/services/seed-defaultsservices.seed

Estado:

Protegido
12. Communities
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/communitiescommunities.read
GET /api/v1/communities/{communityId}communities.read
POST /api/v1/organizations/{organizationId}/communitiescommunities.write

Estado:

Protegido
13. Mobile Units
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/mobile-unitsmobile-units.read
GET /api/v1/mobile-units/{mobileUnitId}mobile-units.read
POST /api/v1/organizations/{organizationId}/mobile-unitsmobile-units.write

Estado:

Protegido
14. Brigades
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/brigadesbrigades.read
GET /api/v1/brigades/{brigadeId}brigades.read
POST /api/v1/organizations/{organizationId}/brigadesbrigades.write

Estado:

Protegido
15. Brigade Services
EndpointPermiso requerido
GET /api/v1/brigades/{brigadeId}/servicesbrigade-services.read
POST /api/v1/brigades/{brigadeId}/servicesbrigade-services.write

Estado:

Protegido
16. Organizations
EndpointPermiso requerido
GET /api/v1/organizationsorganizations.read
GET /api/v1/organizations/{organizationId}organizations.read
POST /api/v1/organizationsorganizations.write

Estado:

Protegido
17. Users
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/usersusers.read
GET /api/v1/users/{userId}users.read
POST /api/v1/organizations/{organizationId}/usersusers.write

Estado:

Protegido
18. Security
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/security/rolesroles.read
GET /api/v1/organizations/{organizationId}/security/permissionsroles.read
GET /api/v1/organizations/{organizationId}/security/role-permissionsroles.read
POST /api/v1/organizations/{organizationId}/security/seed-defaultsroles.assign
POST /api/v1/organizations/{organizationId}/security/user-role-assignmentsroles.assign

Estado:

Protegido
19. Sync Batches
EndpointPermiso requerido
GET /api/v1/organizations/{organizationId}/sync-batchessync-batches.read
GET /api/v1/sync-batches/{syncBatchId}sync-batches.read
POST /api/v1/organizations/{organizationId}/sync-batchessync-batches.write

Estado:

Protegido
20. Validación esperada

Con API en Development y headers válidos:

Debe responder 200/201 según operación.

Sin headers válidos:

Debe responder 401 Unauthorized en endpoints protegidos.

Con usuario autenticado pero sin permiso suficiente:

Debe responder 403 Forbidden.
21. Limitación actual

La autorización por policy ya valida permisos, pero todavía falta reforzar autorización por organización en todos los endpoints.

Pendiente importante:

currentUser.OrganizationId debe coincidir con route organizationId

Excepto para SUPER_ADMIN.

22. Siguiente paso recomendado

Después de este estado, la siguiente mejora de seguridad debe ser:

feat(api): add organization access authorization helper

Objetivo:

Evitar acceso cruzado entre organizaciones.
Centralizar validación de organizationId.
Preparar comportamiento 403.
Mantener excepción controlada para SUPER_ADMIN.
