# Container Image Scanning and SBOM Baseline

## Objetivo

Evitar que una imagen de API con vulnerabilidades críticas o altas conocidas avance sin revisión.

## Controles activos en CI

- Construcción de imagen Docker.
- Escaneo de imagen con Trivy.
- Falla automática en vulnerabilidades CRITICAL,HIGH.
- Generación de SBOM en formato SPDX JSON.
- Publicación del SBOM como artifact del workflow.

## Regla de bloqueo

Si Trivy encuentra vulnerabilidades CRITICAL,HIGH no aceptadas, la imagen no debe desplegarse.

## SBOM

El SBOM permite conocer dependencias, paquetes del sistema y componentes incluidos en la imagen.

Artifact esperado:

- caritas-brigadas-api-sbom

## Excepciones

Toda excepción debe estar documentada con:

- CVE.
- Severidad.
- Paquete afectado.
- Justificación.
- Riesgo aceptado por TI.
- Fecha de expiración.

## No hacer

- No ignorar vulnerabilidades críticas permanentemente.
- No desplegar imagen sin SBOM.
- No usar latest en producción.
- No publicar imágenes con secretos.

## Producción

Antes de producción, TI debe definir si el escaneo oficial será:

- GitHub Actions + Trivy.
- Microsoft Defender for Cloud.
- Azure Container Registry scanning.
- Otra herramienta institucional.
## Trivy cache

El cache interno de Trivy Action se mantiene desactivado en CI para evitar annotations heredadas de actions/cache mientras GitHub fuerza acciones Node 20 a Node 24.

Decisi�n actual:

- cache: 'false'
- escaneo reproducible por ejecuci�n
- menor complejidad de CI
- sin dependencia del cache interno de actions/cache

Si TI necesita optimizar tiempos o evitar rate limits, se debe revaluar con una estrategia de cache actualizada y sin warnings de runtime.