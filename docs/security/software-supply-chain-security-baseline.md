# Software Supply Chain Security Baseline

## Objetivo

Reducir riesgo de dependencias comprometidas, imÃ¡genes inseguras y despliegues no reproducibles.

## Controles actuales

- npm audit.
- NuGet vulnerable scan.
- Docker build gate.
- Container image vulnerability scan.
- SBOM generation.
- Dependency pinning mediante lockfiles y versiones explÃ­citas.
- GitHub Actions Verify.

## Componentes cubiertos

- Frontend Next.js.
- Backend ASP.NET Core.
- NuGet packages.
- npm packages.
- Container image.
- OS packages dentro de la imagen.

## Reglas

- Mantener package-lock.json.
- Mantener versiones explÃ­citas NuGet.
- No instalar dependencias sin justificar.
- No usar imÃ¡genes base no oficiales.
- No desplegar imÃ¡genes sin escaneo.
- No desplegar sin SBOM.

## Pendientes enterprise

- Firmar imÃ¡genes.
- Publicar en registry institucional.
- Definir polÃ­tica de retenciÃ³n de imÃ¡genes.
- Definir excepciÃ³n formal para CVEs.
- Branch protection con required checks.
- Dependabot/Renovate si TI lo aprueba.

## dependency pinning

Las dependencias deben mantenerse fijadas mediante lockfiles, versiones explícitas y revisión en CI antes de aceptar cambios.
