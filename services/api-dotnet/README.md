# Cáritas Brigadas — Backend ASP.NET Core

Backend principal de la plataforma institucional Cáritas Brigadas de Salud.

## Estructura

```text
services/api-dotnet/
├── Caritas.Brigadas.sln
├── src/
│   ├── Caritas.Brigadas.Api
│   ├── Caritas.Brigadas.Application
│   ├── Caritas.Brigadas.Domain
│   ├── Caritas.Brigadas.Infrastructure
│   └── Caritas.Brigadas.Contracts
└── tests/
    ├── Caritas.Brigadas.Api.Tests
    ├── Caritas.Brigadas.Application.Tests
    ├── Caritas.Brigadas.Domain.Tests
    └── Caritas.Brigadas.Infrastructure.Tests
Regla de arquitectura
Domain no depende de nadie.
Contracts no depende de nadie.
Application depende de Domain y Contracts.
Infrastructure depende de Application y Domain.
Api depende de Application, Infrastructure y Contracts.
Prohibido
Conectar clientes directo a SQL Server.
Meter lógica clínica autónoma con IA.
Registrar datos sensibles completos en logs.
Agregar secretos al repositorio.
Saltarse permisos server-side.
Estado actual

Scaffold inicial creado. La solución compila y las pruebas iniciales pasan.
