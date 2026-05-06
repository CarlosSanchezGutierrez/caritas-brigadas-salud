# Authentication OIDC Baseline

Este documento define la frontera entre autenticación local de desarrollo y autenticación productiva OIDC/JWT.

## Estado actual

- Development usa headers X-Dev-* exclusivamente para desarrollo local.
- Production/Staging deben usar Authentication:Mode = JwtBearer.
- El backend ya tiene permisos, roles, current user context, organization access enforcement y JWT Bearer skeleton.
- El frontend ya no debe enviar X-Dev-* cuando NEXT_PUBLIC_AUTH_MODE = oidc.

## Backend

Variables mínimas esperadas para JwtBearer:

- Authentication__Mode=JwtBearer
- Authentication__Authority=https://login.microsoftonline.com/<tenant-id>/v2.0
- Authentication__Audience=api://<api-client-id-or-application-id-uri>
- Authentication__RequireHttpsMetadata=true
- Authentication__ValidIssuer=https://login.microsoftonline.com/<tenant-id>/v2.0
- Authentication__ValidAudiences__0=api://<api-client-id-or-application-id-uri>

## Frontend

Variables mínimas esperadas:

- NEXT_PUBLIC_API_BASE_URL=https://brigadas.caritas.example.org/api/v1
- NEXT_PUBLIC_AUTH_MODE=oidc

No configurar NEXT_PUBLIC_DEV_* en producción.

## Claims esperados por el backend

- user_id
- organization_id
- role_code
- permission_code
- name
- email
- sub como fallback si aplica

## Proveedor recomendado

Para un entorno institucional Microsoft, la ruta natural es Microsoft Entra ID / OIDC. El proveedor exacto debe definirse con TI de Cáritas/Tec.

## Pendiente de implementación

- Seleccionar proveedor real.
- Registrar aplicación cliente frontend.
- Registrar API backend / Application ID URI.
- Configurar scopes y consentimientos.
- Mapear claims reales hacia user_id, organization_id, role_code y permission_code.
- Implementar login/logout frontend.
- Implementar adquisición de access token.
- Mandar Authorization: Bearer <token> desde el frontend cuando AUTH_MODE=oidc.
- Probar token firmado real contra la API.

## Regla de seguridad

Los headers X-Dev-* jamás deben salir de modo development. Si NEXT_PUBLIC_AUTH_MODE=oidc, el frontend no los emite.
