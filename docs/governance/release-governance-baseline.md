# Release Governance Baseline

## Objetivo

Definir cómo versionar, aprobar y liberar cambios.

## semantic versioning

Usar versionado semántico cuando el proyecto llegue a releases formales:

- MAJOR: cambios incompatibles.
- MINOR: funcionalidad nueva compatible.
- PATCH: correcciones compatibles.

## Tags

Formato recomendado:

- v0.1.0
- v0.1.1
- v0.2.0

## release notes

Cada release debe documentar:

- Cambios funcionales.
- Cambios técnicos.
- Cambios de base de datos.
- Riesgos.
- Pasos de despliegue.
- rollback.

## Ambientes

- Local.
- Staging.
- Production.

## Aprobación

Production requiere aprobación explícita de responsables técnicos/institucionales.

## rollback

Todo release debe tener estrategia de rollback antes de producción.
