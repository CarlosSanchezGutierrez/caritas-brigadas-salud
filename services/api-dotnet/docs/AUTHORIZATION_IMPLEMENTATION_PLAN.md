# Authentication and Authorization Implementation Plan

Este documento define la siguiente fase técnica del backend: autenticación real y autorización por permisos.

## 1. Objetivo

El backend ya tiene módulos funcionales suficientes para un MVP local. El siguiente paso correcto no es agregar más endpoints, sino proteger el sistema.

Objetivo de esta fase:

- Identificar quién hace la petición.
- Validar que pertenece a la organización correcta.
- Validar qué rol tiene.
- Validar qué permisos tiene.
- Bloquear acceso a endpoints sensibles.
- Preparar el backend para datos reales en una fase posterior.

## 2. Estado actual

Ya existe base para:

- Users.
- Roles.
- Permissions.
- RolePermissions.
- UserRoleAssignments.
- Seed de seguridad.
- Endpoints de seguridad.
- TraceId/correlationId.
- Respuestas estandarizadas.

Pendiente:

- Autenticación real.
- Claims del usuario autenticado.
- Policies por permiso.
- Enforcement en controllers.
- Pruebas de autorización.
- Actualización de smoke test con credenciales o bypass controlado en local.

## 3. Decisión técnica recomendada

Para esta etapa se recomienda implementar primero una autenticación controlada de desarrollo y después migrarla a un proveedor real.

Orden:

1. Development authentication handler.
2. CurrentUser context.
3. Permission constants.
4. Authorization policies.
5. Permission requirement.
6. Permission handler.
7. Decorar endpoints sensibles.
8. Actualizar smoke test.
9. Después integrar JWT/OIDC real.

## 4. No implementar todavía

No conviene meter todavía:

- Login propio con password.
- Refresh tokens.
- Recuperación de contraseña.
- MFA.
- OAuth completo.
- IdentityServer.
- UI de usuarios.
- Integración institucional.

Eso se deja para cuando exista una decisión clara del proveedor de identidad.

## 5. Estrategia para desarrollo local

En Development se puede usar un header controlado:

```text
X-Dev-User-Id
X-Dev-Organization-Id

Esto permite probar autorización sin implementar login todavía.

Regla:

Solo activo en ambiente Development.
Nunca activo en Production.
Debe estar documentado.
Debe ser reemplazable por JWT/OIDC.
6. Estrategia para producción futura

Opciones viables:

Microsoft Entra ID.
Auth0.
AWS Cognito.
Azure AD B2C.
Supabase Auth.
Proveedor institucional si Cáritas ya tiene uno.

Recomendación inicial:

OIDC/JWT con claims de userId, organizationId, roles y permissions.
7. Claims mínimos requeridos

Claims recomendados:

sub
user_id
organization_id
role_codes
permission_codes
email
name
8. CurrentUser context

Crear una abstracción:

ICurrentUserContext

Responsabilidad:

Saber si el usuario está autenticado.
Obtener UserId.
Obtener OrganizationId.
Obtener roles.
Obtener permisos.
Validar si tiene permiso.

Propiedades sugeridas:

IsAuthenticated
UserId
OrganizationId
Roles
Permissions
9. Permission constants

Crear constantes para permisos.

Ejemplos:

organizations.read
organizations.write
users.read
users.write
roles.read
roles.assign
services.read
services.seed
communities.read
communities.write
mobile-units.read
mobile-units.write
brigades.read
brigades.write
brigade-services.read
brigade-services.write
patients.read
patients.write
patient-visits.read
patient-visits.write
service-encounters.read
service-encounters.write
form-templates.read
form-templates.seed
form-responses.read
form-responses.write
consent-documents.read
consent-documents.write
reports.read
reports.export
sync-batches.read
sync-batches.write
audit-logs.read
10. Policies

Crear policies basadas en permisos.

Ejemplo conceptual:

options.AddPolicy("patients.read", policy =>
{
    policy.RequireAuthenticatedUser();
    policy.Requirements.Add(new PermissionRequirement("patients.read"));
});
11. Decoración de endpoints

Ejemplo:

[Authorize(Policy = "patients.read")]

Aplicar a:

Lectura de pacientes.
Escritura de pacientes.
Visitas.
Atenciones.
Formularios.
Consentimientos.
Reportes.
Auditoría.
Sync batches.
12. Endpoints públicos permitidos

Pueden quedar públicos:

GET /api/v1/health
GET /swagger en Development

Todo lo demás debe requerir autenticación en producción.

13. Validación por organización

Además de permisos, cada endpoint con organizationId debe validar:

currentUser.OrganizationId == route.organizationId

Salvo usuarios superadmin o permisos especiales.

Si no coincide:

403 Forbidden

No debe responder 404 para ocultar todo en esta fase local, pero puede evaluarse después para seguridad.

14. SuperAdmin

El rol SuperAdmin puede cruzar organizaciones, pero debe tratarse con cuidado.

Regla sugerida:

SuperAdmin puede consultar varias organizaciones.
OrganizationAdmin solo su organización.
HealthProvider solo brigadas/servicios asignados o su organización.
Viewer solo lectura autorizada.
Auditor solo auditoría/reportes.
15. Smoke test

El smoke test debe actualizarse para enviar headers dev:

-H "X-Dev-User-Id: ..."
-H "X-Dev-Organization-Id: ..."

O debe recibir parámetros:

-UserId
-OrganizationId

y usarlos como headers.

16. Pruebas mínimas de autorización

Casos requeridos:

Request sin usuario a endpoint protegido debe dar 401.
Usuario sin permiso debe dar 403.
Usuario con permiso debe dar 200/201.
Usuario de otra organización debe dar 403.
Health debe seguir público.
Swagger debe seguir disponible en Development.
Production no debe aceptar headers dev.
17. Orden de implementación recomendado
Bloque 1
feat(api): add current user context and permission constants

Sin bloquear endpoints todavía.

Bloque 2
feat(api): add development authentication handler

Autenticación local por headers.

Bloque 3
feat(api): add permission authorization policies

Policies y handler.

Bloque 4
feat(api): protect patient and clinical endpoints

Aplicar [Authorize] a endpoints sensibles.

Bloque 5
chore(api): update smoke test with dev auth headers

Actualizar smoke test.

Bloque 6
test(api): add authorization behavior tests

Pruebas de 401/403/200.

18. Riesgo principal

Si se protegen endpoints antes de actualizar smoke test, el smoke test va a fallar.

Por eso el orden correcto es:

infraestructura de auth primero
smoke test después
enforcement gradual al final
19. Definición de terminado

Esta fase queda lista cuando:

Health sigue público.
Swagger funciona en Development.
Endpoints sensibles requieren autenticación.
Permisos se validan por policy.
Organización se valida contra el usuario actual.
Smoke test pasa usando headers dev.
Tests pasan.
No hay bypass activo en producción.
Documentación queda actualizada.
20. Decisión final

La siguiente implementación de código debe empezar con:

feat(api): add current user context and permission constants

Ese bloque no debe romper el comportamiento actual porque solo agrega infraestructura base.
