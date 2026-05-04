# Configuración local del backend

El backend usa archivos `appsettings` de ASP.NET Core.

## Archivos versionados

- `appsettings.json`
- `appsettings.Development.json`
- `appsettings.Local.example.json`

## Archivo NO versionado

Para configuración local real, copiar:

    Copy-Item src/Caritas.Brigadas.Api/appsettings.Local.example.json src/Caritas.Brigadas.Api/appsettings.Local.json

`appsettings.Local.json` no debe subirse al repositorio.

## Reglas

- No guardar secretos en el repositorio.
- No guardar connection strings de producción.
- No guardar credenciales de SQL Server.
- No guardar tokens de servicios externos.
- No guardar datos reales de pacientes.
