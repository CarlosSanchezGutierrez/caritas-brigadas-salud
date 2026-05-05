# Authorization Implementation Status

Este documento resume el estado actual de autenticación, autorización por permisos y validación de acceso por organización en el backend de Cáritas Brigadas de Salud.

## 1. Estado general

La fase de autorización ya avanzó de documentación a implementación real.

Actualmente el backend cuenta con:

- Contexto de usuario actual.
- Constantes de roles.
- Constantes de permisos.
- Claims estándar.
- Authentication handler de desarrollo.
- Policies por permiso.
- Authorization handler por permiso.
- Protección de endpoints por `[Authorize(Policy = ...)]`.
- Validación global de acceso por organización.
- Tests unitarios de autorización.
- Smoke test actualizado con headers de desarrollo.

## 2. Componentes implementados

### Current user context

Archivos:

```text
src/Caritas.Brigadas.Application/Security/ICurrentUserContext.cs
src/Caritas.Brigadas.Api/Security/HttpCurrentUserContext.cs

Responsabilidad:

Leer usuario autenticado desde HttpContext.User.
Obtener UserId.
Obtener OrganizationId.
Obtener roles.
Obtener permisos.
Validar roles.
Validar permisos.
Permission constants

Archivo:

src/Caritas.Brigadas.Application/Security/PermissionCodes.cs

Responsabilidad:

Centralizar los códigos de permisos.
Evitar strings duplicados en controllers.
Facilitar policies y pruebas.
Role constants

Archivo:

src/Caritas.Brigadas.Application/Security/RoleCodes.cs

Roles base:

SUPER_ADMIN
ORGANIZATION_ADMIN
COORDINATOR
HEALTH_PROVIDER
RECEPTION
VIEWER
AUDITOR
Claim types

Archivo:

src/Caritas.Brigadas.Application/Security/CurrentUserClaimTypes.cs

Claims principales:

user_id
organization_id
role_code
permission_code
Development authentication handler

Archivos:

src/Caritas.Brigadas.Api/Security/DevelopmentAuthenticationDefaults.cs
src/Caritas.Brigadas.Api/Security/DevelopmentAuthenticationHandler.cs
src/Caritas.Brigadas.Api/Extensions/DevelopmentAuthenticationServiceExtensions.cs

Headers locales:

X-Dev-User-Id
X-Dev-Organization-Id
X-Dev-Roles
X-Dev-Permissions
X-Dev-Name
X-Dev-Email

Uso:

Solo debe funcionar en Development.
Permite probar endpoints protegidos sin login productivo todavía.
Permission authorization policies

Archivos:

src/Caritas.Brigadas.Api/Security/PermissionRequirement.cs
src/Caritas.Brigadas.Api/Security/PermissionAuthorizationHandler.cs
src/Caritas.Brigadas.Api/Extensions/PermissionAuthorizationServiceExtensions.cs

Funcionamiento:

Cada permiso genera una policy.
Cada endpoint protegido exige una policy.
SUPER_ADMIN puede pasar cualquier permiso.
Usuarios normales necesitan el permiso específico.
Organization access authorizer

Archivos:

src/Caritas.Brigadas.Application/Security/IOrganizationAccessAuthorizer.cs
src/Caritas.Brigadas.Api/Security/OrganizationAccessAuthorizer.cs

Regla:

SUPER_ADMIN puede acceder a cualquier organización.
Usuario normal solo puede acceder a su organization_id.
Organization access action filter

Archivos:

src/Caritas.Brigadas.Api/Security/OrganizationAccessActionFilter.cs
src/Caritas.Brigadas.Api/Extensions/OrganizationAccessServiceExtensions.cs

Funcionamiento:

Se aplica globalmente.
Busca organizationId en la ruta o argumentos del action.
Si el usuario no puede acceder a esa organización, responde 403 Forbidden.
Si el endpoint permite anonymous, deja pasar.
Si no hay organizationId, no interviene.
3. Endpoints protegidos

Ya cuentan con policies:

Reports.
Audit logs.
Patients.
Patient visits.
Service encounters.
Form templates.
Form responses.
Consent documents.
Services.
Communities.
Mobile units.
Brigades.
Brigade services.
Organizations.
Users.
Security.
Sync batches.
4. Endpoints públicos

Deben permanecer públicos o semipúblicos en local:

GET /api/v1/health
/swagger en Development
5. Smoke test

El smoke test ya debe enviar headers de desarrollo.

Archivo:

services/api-dotnet/scripts/smoke-test-local.ps1

Parámetros default esperados:

OrganizationId = 4df92032-4a1c-4cf2-b48f-15b570cd073a
UserId = 76279895-817d-47d2-b5c2-2a1e306db4f9

Headers que debe mandar:

X-Dev-User-Id
X-Dev-Organization-Id
X-Dev-Roles: SUPER_ADMIN
X-Dev-Name
X-Dev-Email
6. Tests agregados

Proyecto:

tests/Caritas.Brigadas.Api.Tests

Tests principales:

HttpCurrentUserContextTests
PermissionAuthorizationHandlerTests
OrganizationAccessAuthorizerTests
OrganizationAccessActionFilterTests

Validan:

Usuario no autenticado.
Claims de usuario.
Roles.
Permisos.
SUPER_ADMIN.
Acceso por organización.
Denegación por organización incorrecta.
Filtro global de organización.
AllowAnonymous.
7. Resultado esperado de seguridad
Sin headers

Endpoint protegido:

401 Unauthorized
Con usuario autenticado sin permiso

Endpoint protegido:

403 Forbidden
Con usuario autenticado con permiso

Endpoint protegido:

200 OK / 201 Created
Con usuario de otra organización

Endpoint con organizationId:

403 Forbidden
Con SUPER_ADMIN

Puede cruzar organizaciones:

Permitido
8. Estado actual de producción

Todavía no es autenticación productiva.

Este sistema aún necesita:

JWT/OIDC real.
Proveedor de identidad.
Expiración de tokens.
Validación de issuer/audience.
Rotación de secretos.
Claims emitidos desde backend o identity provider.
Revocación/sesiones.
Configuración por ambiente.
9. Próximo paso técnico recomendado

El siguiente paso correcto es documentar y preparar la transición de DevelopmentAuthenticationHandler hacia JWT/OIDC real.

Bloque recomendado:

docs(api): add production authentication migration plan

Después de eso, los siguientes bloques de código recomendados son:

feat(api): add jwt bearer authentication configuration skeleton
feat(api): add authorization failure response documentation
test(api): add endpoint auth integration tests
10. Riesgo pendiente

Aunque ya existe enforcement de permisos y organización, todavía falta probar integración HTTP real con WebApplicationFactory o equivalente.

Los tests actuales validan componentes internos, pero no prueban todavía todo el pipeline HTTP completo.

Pendiente:

GET protegido sin headers -> 401
GET protegido con headers incorrectos -> 403
GET protegido con headers correctos -> 200
11. Criterio de fase cerrada

Esta fase se puede considerar cerrada localmente cuando:

dotnet build pasa.
dotnet test pasa.
La API corre en Development.
Smoke test pasa completo.
Endpoints protegidos rechazan requests sin headers.
Endpoints protegidos aceptan headers dev válidos.
Documentación está actualizada.
git status queda limpio.
