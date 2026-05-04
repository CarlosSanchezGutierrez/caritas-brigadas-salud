# Migraciones SQL Server

Este directorio contiene scripts SQL generados desde EF Core para revisión técnica.

## Script inicial

- `0001_initial_create.sql`

## Reglas

- No ejecutar scripts contra producción sin revisión.
- No subir backups, `.bak`, `.mdf`, `.ldf` ni datos reales.
- Las apps cliente nunca se conectan directo a SQL Server.
- Las migraciones oficiales viven en `services/api-dotnet/src/Caritas.Brigadas.Infrastructure/Persistence/Migrations`.
- Los scripts en `database/migrations/sqlserver` sirven para inspección, auditoría técnica y despliegues controlados.
