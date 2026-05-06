# Deployment Baseline

Este documento define la base mínima para desplegar Cáritas Brigadas de Salud de forma reproducible.

## Estado actual

- API ASP.NET Core containerizable con Dockerfile multi-stage.
- Dockerfile endurecido con usuario no root, healthcheck y diagnósticos .NET deshabilitados.
- Build de imagen validado en GitHub Actions.
- Deployment metadata gate en CI.
- docker-compose.local.yml para pruebas locales con SQL Server en contenedor.
- Producción todavía requiere definición institucional de hosting, red, secretos, dominio, TLS y monitoreo.

## Camino Microsoft recomendado

Como Cáritas usa SQL Server de Microsoft, el camino más natural es Azure-first:

- Azure Container Registry para almacenar imágenes.
- Azure Container Apps o Azure App Service for Containers para hosting inicial.
- Azure SQL o SQL Server institucional administrado por TI.
- Azure Key Vault para secretos.
- Managed Identity para acceder a secretos y recursos Azure.
- Microsoft Defender for Cloud para postura de seguridad y análisis de vulnerabilidades.

Este repo no queda amarrado a Azure. La imagen Docker y la configuración por variables de entorno permiten migrar a otro hosting si TI lo requiere.

## Principios

- La aplicación debe desplegarse desde artefactos reproducibles.
- La imagen no debe contener secretos.
- Las variables productivas deben venir del ambiente, secret manager o plataforma de deployment.
- appsettings.Local.json no se copia a la imagen.
- El contenedor expone HTTP interno en puerto 8080.
- TLS debe terminar en reverse proxy, load balancer o plataforma administrada.
- Las migraciones no deben ejecutarse automáticamente al iniciar la API.

## Health endpoints

- Liveness: /health/live
- Readiness: /health/ready
- Health API legacy/dev: /api/v1/health

## Docker local

Para construir la imagen local:

```powershell
powershell -ExecutionPolicy Bypass -File scripts/docker-build-local.ps1
```

Para levantar API + SQL Server local con Docker Compose:

```powershell
$env:MSSQL_SA_PASSWORD = "Use-A-Strong-Local-Password-Only-123!"
docker compose -f docker-compose.local.yml up --build
```

La API quedará disponible en:

- http://localhost:8080

## Producción

Variables mínimas esperadas:

- ASPNETCORE_ENVIRONMENT=Production
- DOTNET_ENVIRONMENT=Production
- ConnectionStrings__SqlServer desde secret manager.
- Authentication__Mode=JwtBearer
- Authentication__Authority
- Authentication__Audience
- Authentication__ValidIssuer
- Authentication__ValidAudiences__0
- Cors__AllowedOrigins__0
- AllowedHosts

## Bloqueos antes de producción

- No usar Development auth.
- No usar LocalDB.
- No usar localhost como SQL Server.
- No usar TrustServerCertificate=True.
- No usar Encrypt=False.
- No usar AllowedHosts=*.
- No usar CORS con localhost.

## Siguientes pasos

1. Definir Azure Container Apps vs App Service vs AKS.
2. Definir Azure SQL o SQL Server institucional.
3. Definir Azure Key Vault y managed identity.
4. Crear pipeline de deployment.
5. Configurar dominios y TLS.
6. Configurar observabilidad.
7. Definir rollback strategy.
8. Definir backup/restore y disaster recovery.
