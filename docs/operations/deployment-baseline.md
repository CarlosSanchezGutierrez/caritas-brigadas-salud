# Deployment Baseline

Este documento define la base mínima para desplegar Cáritas Brigadas de Salud de forma reproducible.

## Estado actual

- API ASP.NET Core containerizable con Dockerfile multi-stage.
- Build de imagen validado en GitHub Actions.
- docker-compose.local.yml para pruebas locales con SQL Server en contenedor.
- Producción todavía requiere definición institucional de hosting, red, secretos, dominio, TLS y monitoreo.

## Principios

- La aplicación debe desplegarse desde artefactos reproducibles.
- La imagen no debe contener secretos.
- Las variables productivas deben venir del ambiente, secret manager o plataforma de deployment.
- appsettings.Local.json no se copia a la imagen.
- El contenedor expone HTTP interno en puerto 8080; TLS debe terminar en reverse proxy, load balancer o plataforma administrada.

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

## SQL Server

docker-compose.local.yml usa SQL Server solo para desarrollo local. Producción debe usar SQL Server institucional, Azure SQL o una instancia administrada aprobada por TI.

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

1. Definir proveedor de hosting.
2. Definir SQL Server productivo.
3. Definir secret manager.
4. Crear pipeline de deployment.
5. Configurar dominios y TLS.
6. Configurar observabilidad.
7. Definir rollback strategy.
8. Definir backup/restore y disaster recovery.
