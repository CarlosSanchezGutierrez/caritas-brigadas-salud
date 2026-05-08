# Folder Map

## Objetivo

Ayudar a cualquier persona nueva a saber dónde está cada cosa.

## Raíz del repositorio

```text
.github/                 Workflows, PR template y CODEOWNERS.
apps/web-next/           Frontend Next.js.
docs/                    Documentación técnica, operativa y de gobierno.
scripts/                 Scripts de validación, seguridad, Docker y operación local.
services/api-dotnet/     Backend ASP.NET Core.
tests/load/              Pruebas de carga k6.
docker-compose.local.yml Orquestación local cuando aplique.
README.md                Entrada principal del repositorio.
```

## Backend

```text
services/api-dotnet/
  src/
    Caritas.Brigadas.Api/
    Caritas.Brigadas.Application/
    Caritas.Brigadas.Contracts/
    Caritas.Brigadas.Domain/
    Caritas.Brigadas.Infrastructure/
  tests/
    Caritas.Brigadas.Api.Tests/
    Caritas.Brigadas.Application.Tests/
    Caritas.Brigadas.Domain.Tests/
    Caritas.Brigadas.Infrastructure.Tests/
```

## Frontend

```text
apps/web-next/
  src/app/          Rutas y páginas.
  src/components/   Componentes UI.
  src/lib/          Cliente API, configuración y auth headers.
  src/types/        Tipos TypeScript.
  e2e/              Playwright tests.
```

## Documentación

```text
docs/architecture/  Arquitectura y mapa del sistema.
docs/contributing/  Cómo contribuir.
docs/governance/    Reglas de repo, PR, branch protection y maintainer playbook.
docs/operations/    Deployment, release, handoff y operación.
docs/security/      Seguridad, threat model, supply chain y secret scanning.
docs/testing/       E2E, load testing y performance thresholds.
```

## Regla práctica

Si vas a tocar una pantalla, empieza en apps/web-next.

Si vas a tocar un endpoint, empieza en services/api-dotnet/src/Caritas.Brigadas.Api.

Si vas a tocar reglas de negocio, revisa Application y Domain.

Si vas a tocar persistencia, revisa Infrastructure y migraciones.

Si vas a tocar CI, revisa .github/workflows y scripts.
