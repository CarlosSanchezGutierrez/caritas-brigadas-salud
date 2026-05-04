
Cáritas Brigadas — Backend ASP.NET Core

Backend principal de la plataforma institucional Cáritas Brigadas de Salud.

Estructura
services/api-dotnet/
├── Caritas.Brigadas.sln
├── src/
│   ├── Caritas.Brigadas.Api
│   ├── Caritas.Brigadas.Application
│   ├── Caritas.Brigadas.Domain
│   ├── Caritas.Brigadas.Infrastructure
│   └── Caritas.Brigadas.Contracts
├── tests/
│   ├── Caritas.Brigadas.Api.Tests
│   ├── Caritas.Brigadas.Application.Tests
│   ├── Caritas.Brigadas.Domain.Tests
│   └── Caritas.Brigadas.Infrastructure.Tests
└── scripts/
    ├── setup-local-appsettings.ps1
    ├── update-local-database.ps1
    └── generate-idempotent-sql.ps1
Regla de arquitectura
Domain no depende de nadie.
Contracts no depende de nadie.
Application depende de Domain y Contracts.
Infrastructure depende de Application y Domain.
Api depende de Application, Infrastructure y Contracts.
Comandos básicos

Restaurar, compilar y probar:

dotnet restore Caritas.Brigadas.sln
dotnet build Caritas.Brigadas.sln
dotnet test Caritas.Brigadas.sln

Correr API local:

dotnet run --project src/Caritas.Brigadas.Api/Caritas.Brigadas.Api.csproj

Crear configuración local:

.\scripts\setup-local-appsettings.ps1

Aplicar migraciones locales:

.\scripts\update-local-database.ps1
Prohibido
Conectar clientes directo a SQL Server.
Meter lógica clínica autónoma con IA.
Registrar datos sensibles completos en logs.
Agregar secretos al repositorio.
Saltarse permisos server-side.
Usar datos reales de pacientes en desarrollo.
