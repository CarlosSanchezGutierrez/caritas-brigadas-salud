# Dependency Review Baseline

## Objetivo

Evitar que un pull request introduzca dependencias vulnerables en el repositorio sin generar annotations ruidosas en ejecuciones limpias.

## Control activo

- Workflow: Repository Security.
- Job: Dependency Review.
- Implementación: GitHub Dependency Review REST API vía gh api.
- Script: scripts/dependency-review-rest.ps1.
- Evento: pull request hacia develop o main.

## Configuración

- FAIL_ON_SEVERITY: high.
- Bloquea vulnerabilidades high y critical.
- No ejecuta OpenSSF Scorecard dentro del check principal.
- No usa actions/dependency-review-action para evitar annotations Node.js 20 en runs limpios.

## Decisión

Dependency Review debe enfocarse en vulnerabilidades reales. OpenSSF Scorecard de paquetes terceros puede evaluarse después como workflow separado, pero no debe ensuciar el check principal.

## Licencias

La API devuelve información de licencia cuando está disponible. La validación estricta de licencias queda pendiente hasta que Cáritas/Tec definan una política institucional formal.

## Required checks

El required check sigue llamándose Dependency Review para mantener compatibilidad con los rulesets existentes.

## No hacer

- No bypass de Dependency Review sin justificación.
- No aceptar vulnerabilidades high/critical sin aprobación técnica.
- No agregar dependencias sin revisar necesidad real.
