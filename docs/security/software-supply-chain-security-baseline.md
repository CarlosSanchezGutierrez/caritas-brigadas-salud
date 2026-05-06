# Software Supply Chain Security Baseline

## Objetivo

Reducir riesgo de dependencias comprometidas, imÃƒÂ¡genes inseguras y despliegues no reproducibles.

## Controles actuales

- npm audit.
- NuGet vulnerable scan.
- Docker build gate.
- Container image vulnerability scan.
- SBOM generation.
- Dependency pinning mediante lockfiles y versiones explÃƒÂ­citas.
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
- Mantener versiones explÃƒÂ­citas NuGet.
- No instalar dependencias sin justificar.
- No usar imÃƒÂ¡genes base no oficiales.
- No desplegar imÃƒÂ¡genes sin escaneo.
- No desplegar sin SBOM.

## Pendientes enterprise

- Firmar imÃƒÂ¡genes.
- Publicar en registry institucional.
- Definir polÃƒÂ­tica de retenciÃƒÂ³n de imÃƒÂ¡genes.
- Definir excepciÃƒÂ³n formal para CVEs.
- Branch protection con required checks.
- Dependabot/Renovate si TI lo aprueba.

## dependency pinning

Las dependencias deben mantenerse fijadas mediante lockfiles, versiones explÃ­citas y revisiÃ³n en CI antes de aceptar cambios.
