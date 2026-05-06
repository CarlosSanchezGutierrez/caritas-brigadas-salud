# Production Readiness

Este documento define los requisitos mínimos antes de desplegar Cáritas Brigadas de Salud en un entorno institucional real.

## Estado actual

- Backend ASP.NET Core con capas Domain, Application, Infrastructure, Contracts y Api.
- SQL Server como base de datos principal.
- Frontend Next.js con consumo de API por HTTPS en desarrollo local.
- Gates locales: build, tests, npm audit, NuGet vulnerable scan, smoke funcional y smoke de seguridad.
- GitHub Actions Verify para backend y frontend.

## Bloqueos obligatorios antes de producción

1. Autenticación real mediante JWT/OIDC/Entra ID/Auth0 u otro proveedor institucional.
2. Secretos fuera del repositorio mediante GitHub Secrets, Key Vault, Secrets Manager o mecanismo equivalente.
3. SQL Server productivo con usuarios de mínimo privilegio, backups, recovery plan y migraciones controladas.
4. CORS con dominios HTTPS reales. No usar localhost en producción.
5. AllowedHosts con hosts explícitos. No usar wildcard en producción.
6. HTTPS obligatorio de extremo a extremo.
7. Observabilidad con logs estructurados, métricas, trazas y alertas.
8. Deployment reproducible mediante Docker, IaC o pipeline institucional.
9. Políticas de privacidad, retención de datos, consentimiento y clasificación de información sensible.
10. Pruebas E2E, pruebas de carga, threat model y revisión de seguridad antes de datos reales.
11. Branch protection con required checks para Verify.

## Guardrails activos en código

La API falla al arrancar en un ambiente no Development si:

- Authentication:Mode está vacío o es Development.
- ConnectionStrings:SqlServer está vacío.
- Cors:AllowedOrigins está vacío.
- Cors:AllowedOrigins usa localhost, loopback, wildcard o HTTP.
- Security:RequireHttps es false.
- AllowedHosts está vacío o usa wildcard.

## SQL Server

Para producción se debe usar una instancia SQL Server administrada o institucional. El usuario de aplicación no debe ser sysadmin ni db_owner salvo en tareas explícitas de migración controlada.

## Próximos sprints enterprise

1. Identity integration: JWT/OIDC real.
2. Secrets management.
3. SQL Server production deployment model.
4. Observability baseline.
5. Docker and deployment pipeline.
6. E2E and load tests.
7. Threat model formal.
