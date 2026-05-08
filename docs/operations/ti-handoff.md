# TI Handoff

## Objetivo

Explicar qué necesita un equipo de TI para evaluar, desplegar y operar este sistema.

## Estado actual

El repositorio tiene una base técnica preparada para revisión institucional, pero no sustituye el proceso formal de producción.

## Antes de producción

TI debe definir:

- Ambiente de hosting.
- Dominio real.
- Certificados TLS.
- Proveedor de identidad.
- Secret manager.
- SQL Server productivo.
- Backups.
- Retención de datos.
- Monitoreo.
- Alertas.
- Plan de respuesta a incidentes.

## Hosting recomendado

Por alineación natural con SQL Server y ecosistema Microsoft, Azure es una opción lógica:

- Azure App Service o Azure Container Apps.
- Azure SQL o SQL Server administrado por la institución.
- Azure Key Vault.
- Application Insights.
- Entra ID u OIDC compatible.

## Seguridad mínima antes de producción

- HTTPS público obligatorio.
- Secretos fuera del repo.
- CORS con dominios reales.
- Auth real con OIDC/JWT.
- Development auth deshabilitada.
- Logs sin PHI/PII sensible.
- Backups probados.
- Restore probado.
- Rate limiting configurado.
- Auditoría habilitada.

## Base de datos

SQL Server debe operar con:

- Usuario de aplicación con privilegios mínimos.
- Usuario separado para migraciones si aplica.
- Backups automáticos.
- Plan de recuperación.
- Monitoreo de conexiones.
- Revisión de índices.

## Operación

Antes de go-live:

- Ejecutar smoke tests.
- Validar health checks.
- Validar logs.
- Validar alertas.
- Validar flujo de auth.
- Validar exportación de reportes.
- Validar auditoría.

## Qué pedirle al equipo del proyecto

- Variables requeridas.
- Diagrama de despliegue.
- Plan de migración.
- Plan de rollback.
- Evidencia de CI verde.
- SBOM.
- Reporte de vulnerabilidades.
- Documentación de seguridad.
