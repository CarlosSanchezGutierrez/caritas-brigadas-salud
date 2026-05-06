# Repository Governance Baseline

## Objetivo

Definir reglas operativas para mantener el repositorio estable, auditable y apto para colaboración institucional.

## Principios

- Todo cambio debe entrar por pull request.
- No direct pushes a main.
- No direct pushes a develop cuando el proyecto pase a colaboración multiusuario.
- GitHub Actions Verify debe pasar antes de merge.
- CODEOWNERS debe mantenerse actualizado.
- Los cambios de seguridad, base de datos e infraestructura requieren revisión adicional.

## Ramas

- main: rama estable/release.
- develop: integración principal de desarrollo.
- feature/*: trabajo funcional.
- fix/*: correcciones.
- chore/*: infraestructura, tooling o documentación.
- security/*: cambios de seguridad.

## Pull request

Todo PR debe incluir:

- Summary.
- Risk level.
- Areas affected.
- Validation evidence.
- Security checklist.
- Database checklist si aplica.
- Deployment notes si aplica.

## CODEOWNERS

CODEOWNERS define responsables iniciales del repositorio.

Debe actualizarse cuando Cáritas/Tec asignen mantenedores formales.

## Required checks

Verify debe pasar completo antes de merge.

## No hacer

- No commitear secretos.
- No saltarse CI.
- No hacer force push sobre main.
- No mergear cambios con gates rojos.
- No agregar datos reales de pacientes al repo.
