# Contribution Paths

## Objetivo

Definir rutas de contribución para distintos perfiles sin exigir que todos entiendan todo el sistema.

## Perfil: Frontend

Puede trabajar en:

- Pantallas.
- Componentes.
- Estados de carga.
- Manejo de errores.
- Integración con servicios API.
- E2E tests.

Debe revisar:

- apps/web-next/src/app
- apps/web-next/src/components
- apps/web-next/src/lib

## Perfil: Backend

Puede trabajar en:

- Endpoints.
- DTOs.
- Validaciones.
- Servicios de aplicación.
- Tests de API.
- Auditoría.

Debe revisar:

- services/api-dotnet/src/Caritas.Brigadas.Api
- services/api-dotnet/src/Caritas.Brigadas.Application
- services/api-dotnet/tests

## Perfil: Base de datos

Puede trabajar en:

- Modelo relacional.
- Migraciones.
- Índices.
- Scripts idempotentes.
- SQL Server readiness.

Debe revisar:

- Infrastructure.
- Migration docs.
- Database deployment baseline.

## Perfil: Seguridad

Puede trabajar en:

- Threat model.
- Auth/OIDC.
- Validaciones de configuración.
- Secret scanning.
- CodeQL cuando esté activo.
- Dependency Review.
- SBOM y supply chain.

## Perfil: QA

Puede trabajar en:

- Playwright.
- k6.
- Smoke tests.
- Regression checklist.
- Performance thresholds.

## Perfil: Documentación

Puede trabajar en:

- README.
- START_HERE.
- Guías de onboarding.
- Diagramas.
- Handoff a TI.
- Guías para alumnos.

## Regla para novatos

Un contribuidor nuevo debe empezar por tareas pequeñas, con PR pequeño, sin tocar varias capas a la vez.

Buenas primeras tareas:

- Mejorar texto de una pantalla.
- Agregar empty states.
- Mejorar documentación.
- Agregar tests pequeños.
- Corregir tipos simples.

Tareas no recomendadas para novatos:

- Cambiar auth.
- Cambiar migraciones.
- Cambiar CI/CD.
- Cambiar reglas de seguridad.
- Cambiar estructura global del repo.
