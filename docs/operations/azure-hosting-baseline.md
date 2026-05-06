# Azure Hosting Baseline

Este documento prepara el proyecto para un camino Microsoft/Azure sin generar dependencia irreversible.

## Opción recomendada inicial

Para una primera producción institucional, evaluar primero:

1. Azure Container Apps
2. Azure App Service for Containers
3. AKS solo si TI ya opera Kubernetes

## Azure Container Apps

Ventajas:

- Modelo administrado para contenedores.
- Soporta probes de startup, liveness y readiness.
- Compatible con managed identity.
- Puede integrarse con Key Vault.
- Buena opción si no se quiere operar Kubernetes.

Recomendado si:

- Cáritas/Tec quiere contenedores sin administrar cluster.
- Se busca escalabilidad y separación por revisiones.
- Se necesita integración cloud-native con Azure.

## Azure App Service for Containers

Ventajas:

- Operación sencilla.
- Health Check configurable por path.
- Buena opción para equipos que ya usan App Service.

Recomendado si:

- TI prefiere PaaS clásico.
- El tráfico inicial será moderado.
- Se quiere simplicidad operacional.

## AKS

Ventajas:

- Control avanzado de Kubernetes.
- Adecuado para ecosistemas con múltiples servicios.

No recomendado como primera opción si:

- TI no opera Kubernetes actualmente.
- La aplicación sigue siendo modular monolith/API + frontend.
- Se quiere evitar complejidad operativa innecesaria.

## Azure Container Registry

Usar tags únicos para despliegues:

- commit SHA
- build ID
- release version

No desplegar producción usando latest.

## Secrets

Usar Azure Key Vault con managed identity cuando el hosting sea Azure.

Secretos mínimos:

- ConnectionStrings__SqlServer
- Authentication__Authority
- Authentication__Audience
- Authentication__ValidIssuer
- Authentication__ValidAudiences__0

## TLS

TLS debe terminar en la plataforma Azure, reverse proxy o load balancer.

El contenedor expone HTTP interno en 8080.

## Health probes

- Startup: /health/live
- Liveness: /health/live
- Readiness: /health/ready

## Portabilidad

La aplicación se mantiene portable porque:

- Usa Docker estándar.
- Usa variables de entorno.
- No depende de SDK Azure dentro del runtime.
- No acopla dominio ni SQL Server productivo en código.
