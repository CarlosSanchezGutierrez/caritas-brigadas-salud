# Dependency Review Baseline

## Objetivo

Evitar que un pull request introduzca dependencias vulnerables en el repositorio.

## Control activo

- Workflow: Repository Security.
- Job: Dependency Review.
- Action: actions/dependency-review-action@v4.9.0.
- Evento: pull request hacia develop o main.

## Configuración

- fail-on-severity: high.
- vulnerability-check: true.
- license-check: false.
- comment-summary-in-pr: always.

## Decisión

El bloqueo inicia en severidad high para evitar introducir dependencias nuevas con vulnerabilidades altas o críticas.

## Licencias

license-check queda desactivado inicialmente porque todavía no existe política institucional formal de licencias.

Cuando Cáritas/Tec definan política de licencias, se puede activar allow-licenses o reglas equivalentes.

## Required checks

Después de que este workflow corra al menos una vez en un pull request, agregar Dependency Review como required check en los rulesets.

## No hacer

- No bypass de Dependency Review sin justificación.
- No aceptar vulnerabilidades high/critical sin aprobación técnica.
- No agregar dependencias sin revisar necesidad real.
