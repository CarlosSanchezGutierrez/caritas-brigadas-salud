# Security Map

## Objetivo

Este documento resume los controles de seguridad actuales, los controles pendientes y las responsabilidades mínimas antes de considerar despliegue institucional.

## Controles actuales en el repositorio

- Branch protection mediante rulesets.
- Pull Requests obligatorios.
- Required checks para develop y main.
- Backend build/test con warnings as errors.
- Frontend audit, typecheck y build.
- Docker image build gate.
- Trivy container scanning.
- SBOM artifact en CI.
- Dependency Review vía REST API.
- Secret scanning y push protection en GitHub.
- CodeQL Default Setup configurado.
- Threat model documentado.
- Validación de configuración productiva.
- Security headers baseline.
- Rate limiting baseline.
- Auditoría HTTP, clínica y operativa.

## Controles pendientes de endurecimiento

- Activar Require code scanning results cuando CodeQL quede limpio.
- Configurar commit signing antes de volver a activar Require signed commits.
- Definir proveedor real de identidad con OIDC/JWT.
- Definir secret manager productivo.
- Definir observabilidad productiva.
- Definir alertas.
- Definir backup y restore productivo.
- Definir dominios reales para CORS.
- Definir TLS público obligatorio.

## Datos sensibles

El sistema puede llegar a manejar datos personales, operativos y médicos. Por eso:

- No subir datos reales al repositorio.
- No guardar secretos en archivos versionados.
- No guardar dumps de base de datos.
- No guardar documentos clínicos reales.
- No escribir PHI/PII innecesaria en logs.
- No usar capturas con datos reales en documentación.

## Principio de seguridad

Si una decisión acelera desarrollo pero debilita seguridad, debe documentarse y aprobarse explícitamente. Si no hay aprobación, no se acepta.

## Qué revisar antes de producción

- CodeQL sin alertas críticas o altas abiertas.
- Dependency Review pasando.
- Secret scanning activo.
- Push protection activo.
- CORS con dominios reales.
- Autenticación real habilitada.
- Development auth deshabilitada.
- Backups probados.
- Restore probado.
- Logs revisados para evitar datos sensibles.
- Auditoría habilitada.
- Rate limiting configurado.
