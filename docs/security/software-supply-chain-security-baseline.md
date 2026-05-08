# Software Supply Chain Security Baseline

## Objetivo

Reducir riesgo de dependencias comprometidas, imágenes inseguras y despliegues no reproducibles.

## Controles actuales

- npm audit.
- NuGet vulnerable scan.
- Docker build gate.
- Container image vulnerability scan.
- SBOM generation.
- Dependency pinning mediante lockfiles y versiones explícitas.
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
- Mantener versiones explícitas NuGet.
- No instalar dependencias sin justificar.
- No usar imágenes base no oficiales.
- No desplegar imágenes sin escaneo.
- No desplegar sin SBOM.

## Pendientes enterprise

- Firmar imágenes.
- Publicar en registry institucional.
- Definir política de retención de imágenes.
- Definir excepción formal para CVEs.
- Branch protection con required checks.
- Dependabot/Renovate si TI lo aprueba.

## dependency pinning

Las dependencias deben mantenerse fijadas mediante lockfiles, versiones expl�citas y revisi�n en CI antes de aceptar cambios.
