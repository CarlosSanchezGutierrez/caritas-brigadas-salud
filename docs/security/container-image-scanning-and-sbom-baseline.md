# Container Image Scanning and SBOM Baseline

## Objetivo

Evitar que una imagen de API con vulnerabilidades crÃ­ticas o altas conocidas avance sin revisiÃ³n.

## Controles activos en CI

- ConstrucciÃ³n de imagen Docker.
- Escaneo de imagen con Trivy.
- Falla automÃ¡tica en vulnerabilidades CRITICAL,HIGH.
- GeneraciÃ³n de SBOM en formato SPDX JSON.
- PublicaciÃ³n del SBOM como artifact del workflow.

## Regla de bloqueo

Si Trivy encuentra vulnerabilidades CRITICAL,HIGH no aceptadas, la imagen no debe desplegarse.

## SBOM

El SBOM permite conocer dependencias, paquetes del sistema y componentes incluidos en la imagen.

Artifact esperado:

- caritas-brigadas-api-sbom

## Excepciones

Toda excepciÃ³n debe estar documentada con:

- CVE.
- Severidad.
- Paquete afectado.
- JustificaciÃ³n.
- Riesgo aceptado por TI.
- Fecha de expiraciÃ³n.

## No hacer

- No ignorar vulnerabilidades crÃ­ticas permanentemente.
- No desplegar imagen sin SBOM.
- No usar latest en producciÃ³n.
- No publicar imÃ¡genes con secretos.

## ProducciÃ³n

Antes de producciÃ³n, TI debe definir si el escaneo oficial serÃ¡:

- GitHub Actions + Trivy.
- Microsoft Defender for Cloud.
- Azure Container Registry scanning.
- Otra herramienta institucional.
## Trivy cache

El cache interno de Trivy Action se mantiene desactivado en CI para evitar annotations heredadas de actions/cache mientras GitHub fuerza acciones Node 20 a Node 24.

Decisión actual:

- cache: 'false'
- escaneo reproducible por ejecución
- menor complejidad de CI
- sin dependencia del cache interno de actions/cache

Si TI necesita optimizar tiempos o evitar rate limits, se debe revaluar con una estrategia de cache actualizada y sin warnings de runtime.