# Authentication Environment Variables

Este documento define las variables de entorno relacionadas con autenticación para el backend de Cáritas Brigadas de Salud.

## 1. Objetivo

Centralizar la configuración de autenticación para:

- Desarrollo local.
- Staging.
- Producción.
- Pruebas manuales.
- Smoke tests.
- Migración futura a JWT/OIDC.

El backend actualmente soporta:

```text
Development authentication por headers locales
JWT Bearer authentication skeleton
2. Variables principales
VariableDescripciónEjemplo
Authentication__ModeModo de autenticación.Development
Authentication__AuthorityIssuer/authority OIDC/JWT.https://issuer.example.com
Authentication__AudienceAudience principal del API.caritas-brigadas-api
Authentication__RequireHttpsMetadataExige metadata HTTPS en JWT/OIDC.true
Authentication__ValidIssuerIssuer válido explícito, si aplica.https://issuer.example.com
Authentication__ValidAudiences__0Audience válida adicional.caritas-brigadas-api
Authentication__ValidAudiences__1Audience válida adicional.caritas-mobile-app
3. Modos soportados
Development
$env:Authentication__Mode = "Development"

Uso:

Desarrollo local.
Smoke test local.
Pruebas manuales con headers dev.

Restricción:

Solo debe funcionar en Development.
JwtBearer
$env:Authentication__Mode = "JwtBearer"

Uso:

Staging.
Producción.
Integración futura con proveedor de identidad.

Requiere:

Authentication__Authority
Authentication__Audience o Authentication__ValidAudiences
Disabled
$env:Authentication__Mode = "Disabled"

Uso:

Solo pruebas locales muy controladas.

Restricción:

No permitido fuera de Development.
4. Configuración local recomendada

Para desarrollo local:

cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud\services\api-dotnet

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"

$env:Authentication__Mode = "Development"

$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

dotnet run `
  --project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj `
  --no-launch-profile `
  --urls "https://localhost:7044;http://localhost:5031"
5. Headers de desarrollo

Cuando Authentication__Mode = Development, se pueden usar estos headers:

HeaderDescripción
X-Dev-User-IdID del usuario local.
X-Dev-Organization-IdID de la organización local.
X-Dev-RolesRoles separados por coma.
X-Dev-PermissionsPermisos separados por coma.
X-Dev-NameNombre visible del usuario.
X-Dev-EmailEmail visible del usuario.

Ejemplo:

$baseUrl = "https://localhost:7044"
$organizationId = "4df92032-4a1c-4cf2-b48f-15b570cd073a"
$userId = "76279895-817d-47d2-b5c2-2a1e306db4f9"

curl.exe -k -sS "$baseUrl/api/v1/organizations/$organizationId/reports/summary" `
  -H "X-Dev-User-Id: $userId" `
  -H "X-Dev-Organization-Id: $organizationId" `
  -H "X-Dev-Roles: SUPER_ADMIN" `
  -H "X-Dev-Name: Smoke Test User" `
  -H "X-Dev-Email: smoke.test@caritas.local"
6. Smoke test local

El smoke test usa headers de desarrollo.

Comando:

cd C:\Users\Skere\Documents\GitHub\caritas-brigadas-salud

powershell -ExecutionPolicy Bypass -File .\services\api-dotnet\scripts\smoke-test-local.ps1

Valores default esperados:

OrganizationId = 4df92032-4a1c-4cf2-b48f-15b570cd073a
UserId = 76279895-817d-47d2-b5c2-2a1e306db4f9

También se puede ejecutar con parámetros explícitos:

powershell -ExecutionPolicy Bypass -File .\services\api-dotnet\scripts\smoke-test-local.ps1 `
  -OrganizationId "4df92032-4a1c-4cf2-b48f-15b570cd073a" `
  -UserId "76279895-817d-47d2-b5c2-2a1e306db4f9"
7. Configuración JWT Bearer para staging

Ejemplo de staging:

$env:ASPNETCORE_ENVIRONMENT = "Staging"
$env:DOTNET_ENVIRONMENT = "Staging"

$env:Authentication__Mode = "JwtBearer"
$env:Authentication__Authority = "https://issuer-staging.example.com"
$env:Authentication__Audience = "caritas-brigadas-api"
$env:Authentication__RequireHttpsMetadata = "true"

Con audiences múltiples:

$env:Authentication__Mode = "JwtBearer"
$env:Authentication__Authority = "https://issuer-staging.example.com"
$env:Authentication__ValidAudiences__0 = "caritas-brigadas-api"
$env:Authentication__ValidAudiences__1 = "caritas-mobile-app"
$env:Authentication__RequireHttpsMetadata = "true"
8. Configuración JWT Bearer para producción

Ejemplo de producción:

$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:DOTNET_ENVIRONMENT = "Production"

$env:Authentication__Mode = "JwtBearer"
$env:Authentication__Authority = "https://issuer.production.example.com"
$env:Authentication__Audience = "caritas-brigadas-api"
$env:Authentication__RequireHttpsMetadata = "true"
$env:Authentication__ValidIssuer = "https://issuer.production.example.com"

Regla:

En Production no debe usarse Authentication__Mode = Development.

Si se intenta usar Development fuera de Development, la configuración debe fallar.

9. Claims esperados

El backend espera estos claims:

ClaimUso
user_idIdentificador interno del usuario.
organization_idOrganización a la que pertenece el usuario.
role_codeRol del usuario.
permission_codePermiso del usuario.
nameNombre visible.
emailCorreo del usuario.

Ejemplo conceptual de claims:

{
  "sub": "76279895-817d-47d2-b5c2-2a1e306db4f9",
  "user_id": "76279895-817d-47d2-b5c2-2a1e306db4f9",
  "organization_id": "4df92032-4a1c-4cf2-b48f-15b570cd073a",
  "role_code": "ORGANIZATION_ADMIN",
  "permission_code": [
    "patients.read",
    "patients.write",
    "reports.read"
  ],
  "name": "Carlos Sánchez Gutiérrez",
  "email": "carlos.test@caritas.local"
}
10. Permisos relevantes

Ejemplos:

organizations.read
organizations.write
users.read
users.write
roles.read
roles.assign
services.read
services.seed
patients.read
patients.write
form-responses.read
form-responses.write
consent-documents.read
consent-documents.write
reports.read
reports.export
sync-batches.read
sync-batches.write
audit-logs.read
11. Roles relevantes

Roles base:

SUPER_ADMIN
ORGANIZATION_ADMIN
COORDINATOR
HEALTH_PROVIDER
RECEPTION
VIEWER
AUDITOR

Regla especial:

SUPER_ADMIN puede pasar cualquier permiso.
12. Validación por organización

Además del permiso, el backend valida acceso por organización:

route organizationId debe coincidir con claim organization_id

Excepción:

SUPER_ADMIN puede cruzar organizaciones.

Resultado esperado si no coincide:

403 Forbidden
13. Errores esperados
CasoResultado
Sin token/headers en endpoint protegido401 Unauthorized
Token/header inválido401 Unauthorized
Usuario autenticado sin permiso403 Forbidden
Usuario de otra organización403 Forbidden
Usuario con permiso correcto200 OK o 201 Created
14. Reglas de seguridad

Prohibido:

Usar Authentication__Mode = Development en producción.
Usar headers X-Dev-* en producción.
Guardar tokens en logs.
Pasar tokens por query string.
Desactivar validación de issuer/audience en producción.
Usar RequireHttpsMetadata=false en producción.

Obligatorio:

HTTPS en producción.
Validar issuer.
Validar audience.
Validar expiración.
Validar firma.
Mantener secretos fuera del repositorio.
Usar variables de entorno o secret manager.
15. Variables relacionadas con ambiente

Para local:

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"

Para staging:

$env:ASPNETCORE_ENVIRONMENT = "Staging"
$env:DOTNET_ENVIRONMENT = "Staging"

Para producción:

$env:ASPNETCORE_ENVIRONMENT = "Production"
$env:DOTNET_ENVIRONMENT = "Production"
16. Variables relacionadas con base de datos

Local:

$env:ConnectionStrings__SqlServer = "Server=(localdb)\MSSQLLocalDB;Database=CaritasBrigadas_Local;Trusted_Connection=True;TrustServerCertificate=True;"

Producción debe usar secret manager o variable segura:

$env:ConnectionStrings__SqlServer = "<production-secure-connection-string>"

No guardar connection strings productivas en Git.

17. Checklist local

Antes de validar local:

$env:ASPNETCORE_ENVIRONMENT = "Development"
$env:DOTNET_ENVIRONMENT = "Development"
$env:Authentication__Mode = "Development"

Luego:

dotnet build Caritas.Brigadas.sln
dotnet test Caritas.Brigadas.sln

Después correr API y smoke test.

18. Checklist staging/production

Antes de staging o production:

Authentication__Mode = JwtBearer.
Authentication__Authority configurado.
Authentication__Audience o Authentication__ValidAudiences configurado.
Authentication__RequireHttpsMetadata = true.
Connection string fuera del repo.
Swagger restringido.
Logs sin tokens.
CORS restringido.
HTTPS obligatorio.
19. Pendientes

Todavía falta:

Conectar proveedor real.
Validar token real.
Agregar transformación de claims si el proveedor usa nombres diferentes.
Agregar pruebas de integración HTTP con bearer token.
Documentar issuer/audience reales cuando se elija proveedor.
Decidir si permisos vivirán en JWT o se consultarán desde base de datos.
