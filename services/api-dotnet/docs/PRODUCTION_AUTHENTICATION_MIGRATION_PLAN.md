# Production Authentication Migration Plan

Este documento define el plan para migrar el backend de Cáritas Brigadas de Salud desde autenticación de desarrollo por headers hacia autenticación productiva basada en JWT/OIDC.

## 1. Objetivo

El backend actualmente tiene autorización real por policies y validación de acceso por organización, pero la autenticación local usa headers de desarrollo.

Objetivo de esta fase:

- Mantener el flujo local funcionando.
- Preparar autenticación productiva.
- Evitar romper smoke tests.
- Evitar dependencias prematuras con un proveedor específico.
- Dejar listo el backend para integrar JWT/OIDC.
- Eliminar cualquier bypass de desarrollo en producción.

## 2. Estado actual

Ya existe:

- `ICurrentUserContext`.
- `HttpCurrentUserContext`.
- `PermissionCodes`.
- `RoleCodes`.
- `CurrentUserClaimTypes`.
- `DevelopmentAuthenticationHandler`.
- `PermissionRequirement`.
- `PermissionAuthorizationHandler`.
- `OrganizationAccessAuthorizer`.
- `OrganizationAccessActionFilter`.
- Endpoints protegidos con `[Authorize(Policy = ...)]`.
- Smoke test con headers de desarrollo.
- Tests unitarios de autorización.

El siguiente salto es reemplazar la fuente de identidad.

Actualmente:

```text
Headers de desarrollo -> Claims -> CurrentUserContext -> Policies -> Controllers

Producción debe quedar así:

JWT/OIDC -> Claims -> CurrentUserContext -> Policies -> Controllers
3. Principio de migración

No se debe reescribir la autorización.

La autorización ya debe seguir usando:

ICurrentUserContext.
PermissionCodes.
RoleCodes.
Policies.
Organization access filter.

Solo debe cambiar la forma en la que se llena HttpContext.User.

4. Estrategia recomendada

Implementar dos modos:

Development: DevelopmentAuthenticationHandler
Production/Staging: JwtBearer authentication

Reglas:

Development puede aceptar headers dev.
Staging y Production no deben aceptar headers dev.
JWT debe ser obligatorio en ambientes reales.
Las policies deben funcionar igual en ambos modos.
El smoke test local puede seguir usando headers dev.
Tests de integración pueden usar Development auth.
5. Configuración sugerida

Agregar configuración:

{
  "Authentication": {
    "Mode": "Development",
    "Authority": "",
    "Audience": "",
    "RequireHttpsMetadata": true,
    "ValidIssuer": "",
    "ValidAudiences": []
  }
}

Modos posibles:

Development
JwtBearer
Disabled

Reglas:

Development: solo permitido en Development.
JwtBearer: requerido en Staging y Production.
Disabled: solo para casos de testing muy controlados, preferiblemente evitar.
6. Claims requeridos en JWT

El token productivo debe incluir como mínimo:

user_id
organization_id
role_code
permission_code
email
name

También puede incluir:

sub
aud
iss
exp
iat
nbf
jti
7. Claims alternativos aceptables

Para compatibilidad con proveedores externos:

Claim externoClaim interno esperado
subuser_id si no existe user_id
rolesrole_code
rolerole_code
permissionspermission_code
scopepodría mapearse a permisos
org_idorganization_id
tenant_idorganization_id si aplica

Recomendación:

Crear un claims transformation si el proveedor no emite exactamente los nombres esperados.

8. Opciones de proveedor

Opciones viables:

Microsoft Entra ID.
Auth0.
AWS Cognito.
Azure AD B2C.
Supabase Auth.
Keycloak.
Proveedor institucional de Cáritas si existe.

Recomendación inicial para MVP técnico:

Configurar esqueleto JWT/OIDC sin casarse todavía con proveedor.
9. Decisión recomendada para este proyecto

Para no sobrecomplicar:

Mantener Development headers para local.
Agregar configuración Authentication.
Agregar extensión AddConfiguredAuthentication.
En Development usar DevelopmentAuth.
En Production exigir JWT.
Preparar JwtBearerOptions.
Documentar variables de entorno.
No implementar login propio todavía.
10. Variables de entorno sugeridas
$env:Authentication__Mode = "JwtBearer"
$env:Authentication__Authority = "https://issuer.example.com"
$env:Authentication__Audience = "caritas-brigadas-api"
$env:Authentication__RequireHttpsMetadata = "true"

Para local:

$env:Authentication__Mode = "Development"
11. Validaciones de seguridad JWT

Requerido:

Validar issuer.
Validar audience.
Validar expiración.
Validar firma.
Rechazar tokens vencidos.
Rechazar tokens sin organization_id.
Rechazar tokens sin user_id o sub.
Rechazar tokens sin roles/permisos para endpoints protegidos.
Usar HTTPS.
No aceptar tokens por query string.
No guardar tokens en logs.
12. Mapeo de roles y permisos

Hay dos caminos posibles.

Opción A: roles y permisos dentro del JWT

Ventaja:

Rápido.
Menos consultas a BD.
Útil para MVP.

Desventaja:

Permisos pueden quedar desactualizados hasta renovar token.
Opción B: JWT solo identifica usuario; permisos se consultan en BD

Ventaja:

Permisos siempre actualizados.
Mejor control.

Desventaja:

Más complejidad.
Más consultas.
Requiere caching.

Recomendación:

MVP siguiente: Opción A.
Producción más robusta: Opción B con cache.
13. Flujo productivo recomendado
Cliente obtiene token del proveedor de identidad.
Cliente llama API con Authorization: Bearer <token>.
API valida token.
API construye ClaimsPrincipal.
HttpCurrentUserContext lee claims.
Policy valida permiso.
OrganizationAccessActionFilter valida organizationId.
Controller ejecuta caso de uso.
14. Endpoints que deben quedar protegidos

Todos excepto:

GET /api/v1/health
/swagger en Development

En producción, Swagger debería estar:

Deshabilitado, o
Protegido por auth, o
Permitido solo en red interna/staging.
15. DevelopmentAuthenticationHandler

Debe mantenerse, pero con reglas estrictas:

Solo environment.IsDevelopment().
No registrar en Production.
No aceptar headers dev en Production.
Documentar que es solo para pruebas locales.
Smoke test depende de este handler.
16. Riesgo principal

El mayor riesgo es dejar activo el handler de desarrollo en producción.

Mitigación:

Condición explícita por ambiente.
Tests de configuración.
Logs de startup indicando modo de auth.
Falla dura si Authentication:Mode = Development y ambiente no es Development.
17. Bloques técnicos recomendados
Bloque 1
feat(api): add authentication options configuration

Agrega:

AuthenticationOptions.
Validación básica.
Binding desde configuration.
Bloque 2
feat(api): add configured authentication extension

Agrega:

AddConfiguredAuthentication.
Selector por modo.
Development auth solo en Development.
Preparación para JwtBearer.
Bloque 3
feat(api): add jwt bearer authentication skeleton

Agrega:

Microsoft.AspNetCore.Authentication.JwtBearer.
Configuración de Authority/Audience.
TokenValidationParameters.
Mapeo básico de claims.
Bloque 4
test(api): add authentication configuration tests

Agrega pruebas de:

Development permitido en Development.
Development rechazado en Production.
JwtBearer requiere Authority/Audience.
Disabled no permitido en Production.
Bloque 5
docs(api): document auth environment variables

Documenta:

Variables locales.
Variables staging.
Variables production.
Ejemplos de headers y bearer token.
18. No hacer todavía

No hacer en esta fase:

UI de login.
Registro de usuarios final.
Passwords propios.
Refresh token propio.
MFA.
Pantallas administrativas de permisos.
Integración final con un proveedor sin confirmación del socio.
19. Impacto en smoke test

El smoke test local debe seguir usando:

X-Dev-User-Id
X-Dev-Organization-Id
X-Dev-Roles

No debe usar JWT todavía.

Cuando exista JWT real, se puede agregar parámetro opcional:

-AccessToken

Si se pasa -AccessToken, el smoke test puede usar:

Authorization: Bearer <token>

Si no se pasa, en Development usa headers dev.

20. Criterio de éxito

La migración está lista cuando:

Local sigue funcionando con headers dev.
Production no acepta headers dev.
JWT se puede configurar por ambiente.
Endpoints protegidos siguen usando policies.
Organization access sigue funcionando.
Build pasa.
Tests pasan.
Smoke test local pasa.
Documentación está actualizada.
21. Siguiente implementación recomendada

El siguiente bloque de código debería ser:

feat(api): add authentication options configuration

Ese bloque debe ser pequeño y no debe cambiar comportamiento funcional todavía.

## Estado del esqueleto JWT Bearer

El backend ya cuenta con configuración inicial de JWT Bearer mediante:

```text
src/Caritas.Brigadas.Api/Extensions/ConfiguredAuthenticationServiceExtensions.cs

Estado:

Implementado como skeleton configurable.

El modo JwtBearer ya puede configurarse con:

Authentication:Mode
Authentication:Authority
Authentication:Audience
Authentication:RequireHttpsMetadata
Authentication:ValidIssuer
Authentication:ValidAudiences

Pendiente:

Conectar proveedor real.
Definir issuer real.
Definir audience real.
Validar tokens reales.
Agregar pruebas de integración HTTP con token firmado.

Definir transformación de claims si el proveedor no emite user_id, organization_id, role_code y permission_code.
Variables de entorno documentadas

Las variables de entorno de autenticación están documentadas en:

docs/AUTHENTICATION_ENVIRONMENT_VARIABLES.md

