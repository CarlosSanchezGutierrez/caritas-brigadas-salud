# Docker Runtime Hardening

## Controles activos

- Multi-stage build.
- Runtime separado del SDK.
- Usuario no root mediante APP_UID.
- DOTNET_EnableDiagnostics=0.
- Healthcheck interno contra /health/live.
- appsettings.Local.json excluido del contexto de imagen.
- .env y .env.* excluidos del contexto de imagen.
- Labels OCI básicos.

## Decisiones

El contenedor escucha HTTP interno en puerto 8080. La terminación TLS se debe hacer fuera del contenedor mediante plataforma administrada, reverse proxy o load balancer.

## No hacer

- No incluir secretos en la imagen.
- No copiar appsettings.Local.json.
- No ejecutar migraciones al arrancar la API.
- No usar latest en producción.
- No correr como root.

## Healthcheck

El Dockerfile usa:

- /health/live

La plataforma de hosting debe usar:

- /health/live para liveness.
- /health/ready para readiness.

## Pendientes

- Image scanning.
- SBOM.
- Firma de imágenes si TI lo requiere.
- Registry retention policy.
- Rollback automatizado.
